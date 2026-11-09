#!/usr/bin/env python3
"""Actual Unity/LinuxMono origin cohort build, two-seed flow and renderer acceptance."""
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

ROOT = Path(__file__).resolve().parents[2]
PROJECT = ROOT / 'client/UnityProject'
sys.path.insert(0, str(ROOT / 'tools/checks'))
from check_contract_vectors import canonical
from check_origin_client import expected_document

REQUIRED = ['menuValid', 'cancelBeforePublishValid', 'duplicateNewBlocked', 'newCampaignValid',
            'beginNoMutation', 'waitOneValid', 'waitSixtyValid', 'saveValid', 'reloadValid',
            'continueSameOrigin', 'delayedStateStable', 'selfRecordPure', 'modalFocusContained',
            'modalBlocksActions', 'layoutValid', 'scrollValid', 'renderedCaptures',
            'autoplayInputIsolated', 'noDeliveredExternalInput']


def record_fingerprint(view):
    history = '\n'.join(str(h['AtAge']) + '|' + h['Description'] + '|' + h['Source'] for h in view['History'])
    skills = '\n'.join(s['Label'] + '|' + str(s['Value']) + '|' + s['SourceHistory'] for s in view['Skills'])
    claims = '\n'.join(c['Text'] + '|' + c['Source'] + '|' + str(c['AtAge']) for c in view['Claims'])
    value = '\n'.join([view['ActorName'], str(view['Age']), view['Household'], view['Location'], history, skills, claims, '\n'.join(view['Needs'])])
    return hashlib.sha256(value.encode()).hexdigest()


def inspect_database(root, seed, expected_tick=61):
    paths = sorted(Path(root).glob('origin-*.db'))
    if len(paths) != 1:
        raise ValueError('Expected exactly one published campaign; cancel must not publish another')
    document = expected_document()
    cohort_hash = hashlib.sha256(canonical(document)).hexdigest()
    vectors = json.loads((ROOT / 'content/fixtures/origin-cohort/selector-vectors.json').read_text())
    answer = next(v for v in vectors['vectors'] if v['seed'] == seed)
    path = paths[0]
    with sqlite3.connect(path.as_uri() + '?mode=ro', uri=True) as c:
        meta = c.execute('SELECT world_id,tick,schema_version,rule_version,content_hash,image FROM world_meta').fetchone()
        manifest_seed = c.execute('SELECT world_seed FROM save_manifest').fetchone()[0]
        image = decode_mdc1(meta[5])
        commands = [decode_mdc1(row[0]) for row in c.execute('SELECT payload FROM commands')]
    if meta[:5] != (answer['world'], expected_tick, 2, 'transfer-resource-v1+scheduler-v1+origin-cohort-v1', cohort_hash):
        raise ValueError('Final persisted profile/world/tick differs from known answer')
    origin = image['origin']; rng = image['rng']
    if image['image_version'] != 3 or image['tick'] != expected_tick or 'repair' in image or image['lots'] or len(image['actors']) != 1:
        raise ValueError('Origin image3 bounded world differs')
    if (origin['profile'], origin['actor_definition'], origin['source_hash'], origin['cohort_hash'], origin['draw']) != (
            'origin-cohort-v1', answer['selected'], document['source_hash'], cohort_hash, answer['draw_index']):
        raise ValueError('Persisted assignment differs from independent seed vector/content')
    if manifest_seed != seed or len(rng) != 1 or rng[0]['seed'] != seed or rng[0]['draw'] != answer['draw_index']:
        raise ValueError('Manifest/context/cursor drift')
    if rng[0]['key'] != 'origin-assignment' or rng[0]['domain'] != 'characters' or rng[0]['decision'] != 'origin-cohort-v1' or rng[0]['occurrence'] != 'genesis' or rng[0]['stream'] != 'assignment':
        raise ValueError('RNG context drift')
    actor = image['actors'][0]
    if actor['id'] != origin['principal'] or actor['location'] != origin['home'] or actor['owners'] != [actor['id']]:
        raise ValueError('Principal/home or ungranted household control differs')
    if len(commands) != 2 or any(x['envelope']['type'] != 'AdvanceTime' for x in commands):
        raise ValueError('Unexpected command after callback flow')
    return dict(file=path.name, sha256=sha(path), actor=answer['selected'], world=answer['world'],
                tick=expected_tick, draw_index=rng[0]['draw'], image_version=3, physical_schema=2,
                content_hash=cohort_hash, source_hash=document['source_hash'], commands=len(commands))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--editor', required=True, type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='sprint16-client-', dir=ROOT / 'tests/results'))
    if args.output:
        output.mkdir(parents=True, exist_ok=False)
    report = dict(scope='actual Unity6000.6.7f1 LinuxMono origin-cohort-v1; not full life generation or human playtest',
                  status='Failed', exit_code=1, utc_started=datetime.now(timezone.utc).isoformat(), commands=[], output=str(output))
    try:
        if not args.editor.is_file() or not os.environ.get('DISPLAY'):
            raise ValueError('Actual activated Editor and graphical DISPLAY required')
        def run(argv, label, timeout=1200):
            argv = list(map(str, argv)); report['commands'].append(dict(label=label, argv=argv))
            with (output / (label + '.log')).open('w') as log:
                subprocess.run(argv, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT, timeout=timeout, check=True)
            print('Completed: ' + label, flush=True)
        run([args.dotnet, 'restore', 'MythicalDnd.sln', '--locked-mode', '-m:1', '-nr:false'], 'restore')
        run([args.dotnet, 'build', 'MythicalDnd.sln', '-c', 'Release', '--no-restore', '-m:1', '-nr:false'], 'build')
        run(['python3', 'tools/checks/check_origin_client.py'], 'authored-resource')
        imports = stage(output / 'staged-imports', args.editor)
        destination = PROJECT / 'Assets/Plugins/MythicalRuntime'; destination.mkdir(parents=True, exist_ok=True)
        for entry in imports:
            if entry['mode'] == 'staged':
                shutil.copyfile(output / 'staged-imports' / entry['name'], destination / entry['name'])
        (destination / 'imports.json').write_text(json.dumps(imports, indent=2) + '\n')
        report['imports'] = imports
        for method, label in [('RepairClientBuild.Configure', 'configure'), ('OriginClientBuild.PrepareScene', 'scene')]:
            run([args.editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod', method,
                 '-logFile', output / (label + '-editor.log')], label)
        before = sources()
        binary = output / 'Player/MythicalOrigin.x86_64'
        run([args.editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod', 'OriginClientBuild.Build',
             '-s16BuildPath', binary, '-logFile', output / 'build-player-editor.log'], 'build-player', 1800)
        manifest = sources()
        if before != manifest:
            raise ValueError('Source changed during Player build')
        (output / 'sources.json').write_text(json.dumps(manifest, indent=2, sort_keys=True) + '\n')
        report.update(source_fingerprint=hashlib.sha256(json.dumps(manifest, sort_keys=True).encode()).hexdigest(),
                      player_sha256=sha(binary), editor_sha256=sha(args.editor),
                      managed_assembly_sha256=sha(binary.parent / 'MythicalOrigin_Data/Managed/Assembly-CSharp.dll'))
        runs = []
        for name, seed, expected_name in [('zero', '0' * 64, 'Vạc Lâm'), ('one', '1' * 64, 'Bà Nhu')]:
            qa = output / ('autoplay-' + name); saves = output / ('saves-' + name)
            run([binary, '-screen-width', 1280, '-screen-height', 720, '-screen-fullscreen', 0,
                 '-logFile', output / ('player-' + name + '-runtime.log'), '-s16Autoplay', qa,
                 '-s16SaveRoot', saves, '-s16QaSeed', seed], 'player-' + name, 300)
            result = json.loads((qa / 'qa.json').read_text())
            if not result.get('passed') or not all(result.get(key) is True for key in REQUIRED):
                raise ValueError('Missing required origin positive flag: ' + name)
            if (result.get('unityVersion'), result.get('platform'), result.get('backend')) != ('6000.6.7f1', 'LinuxPlayer', 'Mono'):
                raise ValueError('Actual target/runtime mismatch')
            if result.get('keyboardEvents') != 0 or result.get('pointerEvents') != 0 or result.get('screen') != 'World':
                raise ValueError('Callback autoplay received normal input or ended on wrong screen')
            for key, tick in [('initial', 0), ('afterWait', 61), ('afterContinue', 61), ('current', 61)]:
                frame = result[key]
                if frame['actorName'] != expected_name or frame['tick'] != tick or frame['location'] != 'Bến Vạc':
                    raise ValueError('Player record/tick differs from independent authored expectation: ' + key)
            initial_record = {k: v for k, v in result['initial'].items() if k != 'tick'}
            if any({k: v for k, v in result[key].items() if k != 'tick'} != initial_record for key in ['afterWait', 'afterContinue', 'current']):
                raise ValueError('Reveal record drift')
            expected_layouts = {(w, h, s) for w, h in [(1280, 720), (1920, 1080)] for s in [1, 1.25, 1.5]}
            layouts = result['layouts']
            if len(layouts) != 6 or {(x['width'], x['height'], x['scale']) for x in layouts if x['fits'] and x['bottomVisible']} != expected_layouts:
                raise ValueError('Missing six actual reveal layout/scroll cases')
            wanted = {f'capture-{w}-reveal-{s}-{where}.png' for w in [1280, 1920] for s in [100, 125, 150] for where in ['top', 'bottom']}
            wanted |= {'capture-1280-menu.png', 'capture-1280-cancelled.png', 'capture-1920-world-initial.png',
                       'capture-1920-self-record.png', 'capture-1920-world-waited.png', 'capture-1920-continued-reveal.png', 'capture-1920-continued-world.png'}
            captures = [dict(file=str(p.relative_to(output)), dimensions=png_size(p), sha256=sha(p)) for p in sorted(qa.glob('*.png'))]
            if {Path(c['file']).name for c in captures} != wanted:
                raise ValueError('Incomplete actual renderer capture set')
            for capture in captures:
                width = 1280 if Path(capture['file']).name.startswith('capture-1280-') else 1920
                if tuple(capture['dimensions']) != (width, 720 if width == 1280 else 1080):
                    raise ValueError('Capture dimensions mismatch')
            truth = inspect_database(saves, seed)
            run([args.dotnet, ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll',
                 '--origin-open', ROOT / 'content/definitions/vertical-slice/pack.json', saves], 'host-open-' + name)
            host = json.loads((output / ('host-open-' + name + '.log')).read_text())
            if not host['passed'] or record_fingerprint(host['reveal']) != result['initial']['recordFingerprint']:
                raise ValueError('Core/Mono safe record differs after Player exit and host reopen')
            runs.append(dict(seed=seed, qa=result, final_database=truth, host_reopen=host, captures=captures))
        report['runs'] = runs
        # Rebuild the accepted Sprint15 scene against the same newly staged portable core.
        # Its scene is already Editor-authored/committed; do not regenerate local scene IDs.
        legacy = output / 'legacy15'
        legacy.mkdir()
        old_binary = legacy / 'Player/MythicalRepair.x86_64'
        run([args.editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod', 'RepairClientBuild.Build',
             '-s15BuildPath', old_binary, '-logFile', legacy / 'build-editor.log'], 'legacy15-build', 1800)
        run([old_binary, '-screen-width', 1280, '-screen-height', 720, '-screen-fullscreen', 0,
             '-logFile', legacy / 'player-runtime.log', '-s15Autoplay', legacy / 'autoplay',
             '-s15SaveRoot', legacy / 'saves'], 'legacy15-player', 300)
        old_qa = json.loads((legacy / 'autoplay/qa.json').read_text())
        old_flags = ['modalBlocksScene', 'modalBlocksActions', 'modalFocusContained', 'modalBackgroundSubmitBlocked',
                     'purePreview', 'startValid', 'retryValid', 'snapshotValid', 'reopenRunning', 'beforeDueValid',
                     'completionValid', 'cancelValid', 'waitValid', 'safePicking', 'occlusionBefore', 'occlusionBehind',
                     'textScaleValid', 'renderedCaptures', 'workingFeedbackValid', 'mainPanelScrollValid',
                     'autoplayInputIsolated', 'noDeliveredExternalInput', 'completionStableAfterCaptures']
        if not old_qa.get('passed') or not all(old_qa.get(k) is True for k in old_flags) or (
                old_qa.get('unityVersion'), old_qa.get('platform'), old_qa.get('backend')) != ('6000.6.7f1', 'LinuxPlayer', 'Mono'):
            raise ValueError('Actual Sprint15 Player regression failed')
        legacy_states = []
        for filename in ['world.db', 'qa-cancel.db', 'qa-wait.db']:
            db = legacy / 'saves' / filename
            with sqlite3.connect(db.as_uri() + '?mode=ro', uri=True) as c:
                final = inspect_final_database(c, filename)
                image = decode_mdc1(c.execute('SELECT image FROM world_meta').fetchone()[0])
                if image['image_version'] != 2 or 'origin' in image:
                    raise ValueError('Legacy repair image2 compatibility changed')
                legacy_states.append(dict(file=filename, sha256=sha(db), final_state=final))
        report['legacy15'] = dict(passed=True, qa=old_qa, final_databases=legacy_states,
                                  managed_assembly_sha256=sha(old_binary.parent / 'MythicalRepair_Data/Managed/Assembly-CSharp.dll'),
                                  captures=[dict(file=str(p.relative_to(output)), dimensions=png_size(p), sha256=sha(p))
                                            for p in sorted((legacy / 'autoplay').glob('*.png'))])
        if sources() != manifest:
            raise ValueError('Source changed during actual Player acceptance')
        report.update(status='Passed', exit_code=0)
    except (OSError, ValueError, KeyError, IndexError, sqlite3.Error, subprocess.SubprocessError) as error:
        report['error'] = str(error); print(str(error), file=sys.stderr)
    report['utc_finished'] = datetime.now(timezone.utc).isoformat()
    (output / 'evidence.json').write_text(json.dumps(report, indent=2) + '\n')
    print('Evidence: ' + str(output / 'evidence.json'), flush=True)
    return report['exit_code']


if __name__ == '__main__':
    raise SystemExit(main())
