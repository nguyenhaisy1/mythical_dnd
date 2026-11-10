#!/usr/bin/env python3
"""Actual Unity weaving profile, three origins and unchanged scene15/16 Player regression."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import shutil
import sqlite3
import subprocess
import sys
import tempfile

from run_conformance import stage
from run_repair_client import sources, sha, png_size, decode_mdc1, inspect_final_database
from run_origin_client import REQUIRED as ORIGIN_REQUIRED, record_fingerprint, inspect_database as inspect_origin

ROOT = Path(__file__).resolve().parents[2]
PROJECT = ROOT / 'client/UnityProject'
sys.path.insert(0, str(ROOT / 'tools/checks'))
from check_weaving_client import expected_profile
from check_origin_client import expected_document

BASE_FLAGS = ORIGIN_REQUIRED + ['worldLayoutValid', 'worldScrollValid', 'worldFocusValid']
WORK_FLAGS = ['previewPure', 'previewLayoutValid', 'startValid', 'saveRunningValid', 'reopenRunning', 'beforeDueValid', 'completionValid', 'cancelValid']
REPAIR_FLAGS = ['modalBlocksScene', 'modalBlocksActions', 'modalFocusContained', 'modalBackgroundSubmitBlocked',
                'purePreview', 'startValid', 'retryValid', 'snapshotValid', 'reopenRunning', 'beforeDueValid',
                'completionValid', 'cancelValid', 'waitValid', 'safePicking', 'occlusionBefore', 'occlusionBehind',
                'textScaleValid', 'renderedCaptures', 'workingFeedbackValid', 'mainPanelScrollValid',
                'autoplayInputIsolated', 'noDeliveredExternalInput', 'completionStableAfterCaptures']


def inspect_database(root, seed, expected_tick, completed=1, cancelled=1, running=0):
    paths = list(Path(root).glob('origin-*.db'))
    if len(paths) != 1:
        raise ValueError('Expected exactly one published weaving campaign')
    path = paths[0]
    parameters, cohort_hash, recipe_hash, profile_hash = expected_profile()
    answers = json.loads((ROOT / 'content/fixtures/origin-cohort/selector-vectors.json').read_text())['vectors']
    answer = next(v for v in answers if v['seed'] == seed)
    record = next(r for r in expected_document()['records'] if r['actor_id'] == answer['selected'])
    skill = next((s['value'] for s in record['skills'] if s['id'] == 'weaving'), 0)
    with sqlite3.connect(path.as_uri() + '?mode=ro', uri=True) as c:
        meta = c.execute('SELECT world_id,tick,schema_version,rule_version,content_hash,image,sha256 FROM world_meta').fetchone()
        saved_seed = c.execute('SELECT world_seed FROM save_manifest').fetchone()[0]
        image = decode_mdc1(meta[5])
        mirrors = {k: decode_mdc1(v) for k, v in c.execute('SELECT id,payload FROM entities')}
        commands = [decode_mdc1(row[0]) for row in c.execute('SELECT payload FROM commands')]
    if meta[:5] != (answer['world'], expected_tick, 2, 'transfer-resource-v1+scheduler-v1+origin-cohort-v1+personal-weaving-v1', profile_hash):
        raise ValueError('Persisted weaving profile/world/tick differs from independent answer')
    if hashlib.sha256(meta[5]).hexdigest() != meta[6] or image['image_version'] != 4 or 'repair' in image or saved_seed != seed:
        raise ValueError('Persisted image4/hash/manifest differs')
    origin, work, rng = image['origin'], image['weaving'], image['rng']
    if (origin['actor_definition'], origin['cohort_hash'], origin['source_hash'], origin['draw']) != (answer['selected'], cohort_hash, expected_document()['source_hash'], 1):
        raise ValueError('Origin metadata drift')
    if len(rng) != 1 or (rng[0]['key'], rng[0]['seed'], rng[0]['draw'], rng[0]['domain'], rng[0]['decision'], rng[0]['occurrence'], rng[0]['stream']) != (
            'origin-assignment', seed, 1, 'characters', 'origin-cohort-v1', 'genesis', 'assignment'):
        raise ValueError('Additional gameplay RNG/context drift')
    if (work['profile'], work['content_hash'], work['actor'], work['worksite'], work['skill']) != ('personal-weaving-v1', profile_hash, origin['principal'], origin['home'], skill):
        raise ValueError('Weaving genesis/skill differs')
    if work['recipe'] != dict(fiber_cost=2, duration_ticks=30, cord_yield=1, minimum_skill=40, fingerprint=recipe_hash):
        raise ValueError('Effective recipe differs from versioned source')
    lots = {l['id']: l for l in image['lots']}; actor = image['actors'][0]
    if len(lots) != 2 or len(image['actors']) != 1 or actor['id'] != origin['principal'] or actor['owners'] != [actor['id']]:
        raise ValueError('Unexpected world bindings or household controls')
    fiber, cord = lots[work['input_lot']], lots[work['output_lot']]
    if (fiber['commodity'], fiber['on_hand'], fiber['reserved'], cord['commodity'], cord['on_hand'], cord['reserved']) != (
            'commodity.fiber@1', 8 - 2 * completed, 2 * running, 'commodity.cord@1', completed, 0):
        raise ValueError('Final material conservation differs')
    if any(l['owner'] != origin['principal'] or l['location'] != origin['home'] for l in lots.values()):
        raise ValueError('Work uses ungranted household/foreign stock')
    activities, jobs = work['activities'], {j['id']: j for j in image['jobs']}
    if len(activities) != completed + cancelled + running or len(jobs) != len(activities):
        raise ValueError('Activity/job count differs')
    if [sum(a['status'] == n for a in activities) for n in [0, 1, 2]] != [running, completed, cancelled]:
        raise ValueError('Activity statuses differ')
    if skill < 40 and activities:
        raise ValueError('Nonweaver acquired an activity')
    for activity in activities:
        job = jobs[activity['job']]
        if activity['due'] - activity['started'] != 30 or activity['reserved'] != (2 if activity['status'] == 0 else 0):
            raise ValueError('Activity effective time/hold differs')
        if (job['kind'], job['handler'], job['activity'], job['due'], job['status']) != ('weaving', 'personal-weaving-v1', activity['id'], activity['due'], activity['status']):
            raise ValueError('Typed scheduled job pairing differs')
        if mirrors[activity['id']]['weaving_activity'] != activity:
            raise ValueError('SQL activity mirror differs from image')
    types = [c['envelope']['type'] for c in commands]
    if (types.count('StartWeaving'), types.count('CancelWeaving'), types.count('ExecuteScheduledWeaving')) != (len(activities), cancelled, completed):
        raise ValueError('Unexpected command effects after final capture')
    if any(t not in ['StartWeaving', 'CancelWeaving', 'AdvanceTime', 'ExecuteScheduledWeaving', 'CloseAdvance'] for t in types):
        raise ValueError('Unsupported command appeared')
    facts = image['events']
    for activity in activities:
        if activity['status'] == 1:
            finished = [e for e in facts if e['type'] == 'WeavingCompleted' and e['aggregate'] == activity['id']]
            if len(finished) != 1 or finished[0]['tick'] != activity['due']:
                raise ValueError('Completion did not commit at actual due tick')
    return dict(file=path.name, sha256=sha(path), actor=answer['selected'], world=answer['world'], skill=skill, tick=expected_tick,
                draw=1, fiber=fiber['on_hand'], reserved=fiber['reserved'], cord=cord['on_hand'], completed=completed,
                cancelled=cancelled, running=running, image_version=4, physical_schema=2, profile_hash=profile_hash, recipe_hash=recipe_hash,
                commands=len(types), command_types=types)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet'); parser.add_argument('--editor', required=True, type=Path); parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='sprint17-client-', dir=ROOT / 'tests/results'))
    if args.output: output.mkdir(parents=True, exist_ok=False)
    report = dict(scope='actual Unity6000.6.7f1 LinuxMono bounded weaving; not full profession or human playtest', status='Failed', exit_code=1, commands=[], utc_started=datetime.now(timezone.utc).isoformat(), output=str(output))
    try:
        if not args.editor.is_file() or not os.environ.get('DISPLAY'): raise ValueError('Activated Editor and graphical DISPLAY required')
        def run(argv, label, timeout=1200):
            command = list(map(str, argv)); report['commands'].append(dict(label=label, command=command))
            with (output / (label + '.log')).open('w') as log: subprocess.run(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT, check=True, timeout=timeout)
            print('Completed: ' + label, flush=True)
        run([args.dotnet, 'restore', 'MythicalDnd.sln', '--locked-mode', '-m:1', '-nr:false'], 'restore')
        run([args.dotnet, 'build', 'MythicalDnd.sln', '-c', 'Release', '--no-restore', '-m:1', '-nr:false'], 'build')
        run(['python3', 'tools/checks/check_weaving_client.py'], 'authored-resource')
        imports = stage(output / 'staged-imports', args.editor)
        destination = PROJECT / 'Assets/Plugins/MythicalRuntime'; destination.mkdir(parents=True, exist_ok=True)
        for entry in imports:
            if entry['mode'] == 'staged': shutil.copyfile(output / 'staged-imports' / entry['name'], destination / entry['name'])
        (destination / 'imports.json').write_text(json.dumps(imports, indent=2) + '\n'); report['imports'] = imports
        for method, label in [('RepairClientBuild.Configure', 'configure'), ('WeavingClientBuild.PrepareScene', 'scene')]:
            run([args.editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod', method, '-logFile', output / (label + '-editor.log')], label)
        manifest = sources(); binary = output / 'Player/MythicalWeaving.x86_64'
        run([args.editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod', 'WeavingClientBuild.Build', '-s17BuildPath', binary, '-logFile', output / 'build-editor.log'], 'build-player', 1800)
        if sources() != manifest: raise ValueError('Sources changed during Player build')
        (output / 'sources.json').write_text(json.dumps(manifest, sort_keys=True, indent=2) + '\n')
        report.update(source_fingerprint=hashlib.sha256(json.dumps(manifest, sort_keys=True).encode()).hexdigest(), managed_assembly_sha256=sha(binary.parent / 'MythicalWeaving_Data/Managed/Assembly-CSharp.dll'), player_sha256=sha(binary), editor_sha256=sha(args.editor))
        runs = []
        for label, seed, actor, qualified in [('lam', '0' * 64, 'Vạc Lâm', True), ('nhu', '1' * 64, 'Bà Nhu', True), ('kien', '01234567' * 8, 'Bến Kiên', False)]:
            qa, saves = output / ('autoplay-' + label), output / ('saves-' + label)
            run([binary, '-screen-width', 1280, '-screen-height', 720, '-screen-fullscreen', 0, '-logFile', output / ('player-' + label + '-runtime.log'), '-s17Autoplay', qa, '-s17SaveRoot', saves, '-s17QaSeed', seed], 'player-' + label, 300)
            value = json.loads((qa / 'qa.json').read_text()); tick = 91 if qualified else 61
            if not value.get('passed') or not all(value.get(f) is True for f in BASE_FLAGS) or value['qualifiedScenario'] != qualified:
                raise ValueError('Required base callbacks differ: ' + label)
            if (value['unityVersion'], value['platform'], value['backend']) != ('6000.6.7f1', 'LinuxPlayer', 'Mono') or value['keyboardEvents'] or value['pointerEvents'] or value['screen'] != 'World':
                raise ValueError('Runtime/backend or input isolation differs')
            if qualified and not all(value.get(f) is True for f in WORK_FLAGS) or not qualified and value.get('nonWeaverDenied') is not True:
                raise ValueError('Work acceptance flag differs')
            for name, expected_tick in [('initial', 0), ('afterWait', tick), ('afterContinue', tick), ('current', tick)]:
                if (value[name]['actorName'], value[name]['tick'], value[name]['location']) != (actor, expected_tick, 'Bến Vạc'):
                    raise ValueError('Authored origin/clock differs: ' + name)
            record = {k:v for k,v in value['initial'].items() if k != 'tick'}
            if any({k:v for k,v in value[n].items() if k != 'tick'} != record for n in ['afterWait','afterContinue','current']): raise ValueError('Origin record changed')
            layouts = {(x['width'],x['height'],x['scale']) for x in value['layouts'] if x['fits'] and x['bottomVisible']}
            if layouts != {(w,h,s) for w,h in [(1280,720),(1920,1080)] for s in [1,1.25,1.5]}: raise ValueError('Missing six reveal layouts')
            if {(x['width'],x['height'],x['scale']) for x in value['worldLayouts'] if x['fits'] and x['bottomVisible'] and x['controlsReachable']} != layouts: raise ValueError('Missing six actual World layout/focus/scroll cases')
            captures = [dict(file=str(p.relative_to(output)), dimensions=png_size(p), sha256=sha(p)) for p in sorted(qa.glob('*.png'))]
            if len(captures) != (42 if qualified else 32): raise ValueError('Incomplete rendered capture set')
            for item in captures:
                width = 1280 if Path(item['file']).name.startswith('capture-1280-') else 1920
                if tuple(item['dimensions']) != (width,720 if width==1280 else 1080): raise ValueError('Actual capture dimensions differ')
            truth = inspect_database(saves, seed, tick, completed=1 if qualified else 0, cancelled=1 if qualified else 0)
            run([args.dotnet, ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll', '--weaving-open', ROOT / 'content/definitions/vertical-slice/pack.json', ROOT / 'rules/parameters/personal-weaving-v1.json', saves], 'host-open-' + label)
            host = json.loads((output / ('host-open-' + label + '.log')).read_text())
            if not host['passed'] or record_fingerprint(host['origin']) != value['initial']['recordFingerprint']: raise ValueError('Host/Mono own-record mismatch')
            runs.append(dict(seed=seed, qa=value, final_database=truth, host_reopen=host, captures=captures))
        report['runs'] = runs
        legacy_reports = {}
        for number, method, filename, cli in [(16,'OriginClientBuild.Build','MythicalOrigin','s16'),(15,'RepairClientBuild.Build','MythicalRepair','s15')]:
            folder = output / ('legacy' + str(number)); folder.mkdir()
            old = folder / ('Player/' + filename + '.x86_64'); qa = folder / 'autoplay'; saves = folder / 'saves'
            run([args.editor,'-batchmode','-nographics','-projectPath',PROJECT,'-executeMethod',method,'-' + cli + 'BuildPath',old,'-logFile',folder/'build-editor.log'], 'legacy' + str(number) + '-build',1800)
            argv = [old,'-screen-width',1280,'-screen-height',720,'-screen-fullscreen',0,'-logFile',folder/'player-runtime.log','-' + cli + 'Autoplay',qa,'-' + cli + 'SaveRoot',saves]
            if number==16: argv += ['-s16QaSeed','0'*64]
            run(argv,'legacy'+str(number)+'-player',300); value=json.loads((qa/'qa.json').read_text())
            flags=ORIGIN_REQUIRED if number==16 else REPAIR_FLAGS
            if not value.get('passed') or not all(value.get(f) is True for f in flags) or (value['unityVersion'],value['platform'],value['backend']) != ('6000.6.7f1','LinuxPlayer','Mono'): raise ValueError('Actual legacy Player regression failed')
            truth = []
            if number==16: truth=[inspect_origin(saves,'0'*64)]
            else:
                for name in ['world.db','qa-cancel.db','qa-wait.db']:
                    with sqlite3.connect((saves/name).as_uri()+'?mode=ro',uri=True) as c:
                        final=inspect_final_database(c,name); image=decode_mdc1(c.execute('SELECT image FROM world_meta').fetchone()[0])
                        if image['image_version']!=2 or 'origin' in image or 'weaving' in image: raise ValueError('Repair image2 changed')
                    truth.append(dict(file=name,sha256=sha(saves/name),state=final))
            legacy_reports[str(number)] = dict(passed=True,qa=value,final_databases=truth,managed_assembly_sha256=sha(old.parent/(filename+'_Data/Managed/Assembly-CSharp.dll')),captures=[dict(file=str(p.relative_to(output)),dimensions=png_size(p),sha256=sha(p)) for p in sorted(qa.glob('*.png'))])
        report['legacy'] = legacy_reports
        if sources()!=manifest: raise ValueError('Sources changed during actual acceptance')
        report.update(status='Passed',exit_code=0)
    except (OSError,ValueError,KeyError,IndexError,sqlite3.Error,subprocess.SubprocessError) as error:
        report['error']=str(error); print(str(error),file=sys.stderr)
    report['utc_finished']=datetime.now(timezone.utc).isoformat(); (output/'evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
    print('Evidence: '+str(output/'evidence.json'),flush=True); return report['exit_code']


if __name__=='__main__': raise SystemExit(main())
