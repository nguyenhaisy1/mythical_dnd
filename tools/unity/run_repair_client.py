#!/usr/bin/env python3
"""Build and run the bounded Unity repair client; record actual Editor/Player evidence."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import sqlite3
import struct
import subprocess
import sys
import tempfile

from run_conformance import stage

ROOT = Path(__file__).resolve().parents[2]
PROJECT = ROOT / 'client/UnityProject'


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def sources():
    files = {}
    for folder in ['engine', 'tools/unity', 'client/UnityProject/Assets/Game',
                   'client/UnityProject/Packages', 'client/UnityProject/ProjectSettings']:
        for parent, dirs, names in os.walk(ROOT / folder):
            dirs[:] = sorted(d for d in dirs if d not in ['bin', 'obj', '__pycache__', 'Library', 'Temp'])
            for name in sorted(names):
                p = Path(parent) / name
                if p.suffix not in ['.dll', '.pdb', '.so']:
                    files[str(p.relative_to(ROOT))] = sha(p)
    return files


def png_size(path):
    data = Path(path).read_bytes()
    if data[:8] != b'\x89PNG\r\n\x1a\n':
        raise ValueError('Capture is not a PNG: ' + str(path))
    return struct.unpack('>II', data[16:24])


def decode_mdc1(raw):
    """Read-only evidence decoder, independent of the C# runtime serializer."""
    if not raw.startswith(b'MDC1'):
        raise ValueError('Unexpected persisted evidence codec')
    position = 4

    def read():
        nonlocal position
        tag = raw[position:position + 1]; position += 1
        if tag in (b'n', b't', b'f'):
            return {b'n': None, b't': True, b'f': False}[tag]
        if tag not in (b'i', b's', b'a', b'o'):
            raise ValueError('Invalid evidence codec tag')
        end = raw.index(b';' if tag == b'i' else b':', position)
        count = int(raw[position:end]); position = end + 1
        if tag == b'i':
            return count
        if count < 0 or count > len(raw):
            raise ValueError('Invalid evidence codec length')
        if tag == b's':
            value = raw[position:position + count].decode('utf-8'); position += count
            return value
        if tag == b'a':
            return [read() for _ in range(count)]
        value = {}
        for _ in range(count):
            key = read()
            if not isinstance(key, str) or key in value:
                raise ValueError('Invalid evidence codec object key')
            value[key] = read()
        return value

    value = read()
    if position != len(raw):
        raise ValueError('Trailing or truncated evidence codec bytes')
    return value


def inspect_final_database(connection, filename):
    """Check actual final truth, not a projection sampled before the last rendered frame."""
    entities = {key: decode_mdc1(payload) for key, payload in connection.execute('SELECT id,payload FROM entities')}
    prefix = 'w000000000000000f/'
    boat = entities[prefix + 'item/0000000000000001']['item']
    lot = entities[prefix + 'lot/0000000000000001']['lot']
    activities = [e['activity'] for e in entities.values() if e['activity'] is not None]
    commands = [decode_mdc1(payload) for payload, in connection.execute('SELECT payload FROM commands')]
    starts = sum(c['envelope']['type'] == 'StartRepair' for c in commands)
    expected = {'world.db': (700, 8, 0, 1, 1), 'qa-cancel.db': (500, 10, 0, 1, 2),
                'qa-wait.db': (500, 10, 0, 0, None)}[filename]
    condition, on_hand, reserved, start_count, status = expected
    if (boat['condition'], lot['on_hand'], lot['reserved'], starts, len(activities)) != (condition, on_hand, reserved, start_count, start_count):
        raise ValueError('Final SQLite repair/material/command state differs: ' + filename)
    if activities and (activities[0]['status'] != status or activities[0]['reserved'] != 0):
        raise ValueError('Final SQLite activity differs: ' + filename)
    if entities[prefix + 'item/0000000000000003']['item']['condition'] != 321 or entities[prefix + 'lot/0000000000000002']['lot']['on_hand'] != 917:
        raise ValueError('Hidden fixture truth was changed: ' + filename)
    return dict(condition=condition, timber=on_hand, reserved=reserved, start_receipts=starts,
                activity_count=len(activities), activity_status=status, hidden_fixture_unchanged=True)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--editor', type=Path, required=True)
    parser.add_argument('--output', type=Path, help='New evidence directory, never reuse an old run')
    args = parser.parse_args()
    (ROOT / 'tests/results').mkdir(exist_ok=True)
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='sprint15-client-', dir=ROOT / 'tests/results'))
    if args.output:
        output.mkdir(parents=True, exist_ok=False)
    report = dict(profile='repair-client-2d-v1', status='NotRun', exit_code=2,
                  utc_started=datetime.now(timezone.utc).isoformat(),
                  scope='actual Linux Mono Player; bounded fixed-actor worksite, not full VS07/human playtest',
                  os=platform.platform(), architecture=platform.machine(), commands=[], output=str(output))
    code = 1
    try:
        editor = args.editor.resolve()
        if not editor.is_file() or not os.access(editor, os.X_OK):
            raise ValueError('Unity Editor unavailable')
        if not (PROJECT / 'ProjectSettings/ProjectVersion.txt').is_file():
            raise ValueError('Bootstrap the project with the actual Editor before this acceptance run')
        if platform.system() != 'Linux' or platform.machine() not in ['x86_64', 'AMD64']:
            raise ValueError('This candidate runner supports Linux x64 Mono only')
        if not os.environ.get('DISPLAY'):
            raise ValueError('A real graphical DISPLAY is required for rendered Player captures')

        def run(argv, label, timeout=1200):
            report['commands'].append(dict(label=label, argv=list(map(str, argv))))
            with (output / (label + '.log')).open('w') as log:
                subprocess.run(list(map(str, argv)), cwd=ROOT, stdout=log, stderr=subprocess.STDOUT,
                               timeout=timeout, check=True)
            print('Completed: ' + label, flush=True)

        run([args.dotnet, 'restore', 'MythicalDnd.sln', '--locked-mode', '-m:1', '-nr:false'], 'restore')
        run([args.dotnet, 'build', 'MythicalDnd.sln', '-c', 'Release', '--no-restore', '-m:1', '-nr:false'], 'build')
        imports = stage(output / 'staged-imports', editor)
        destination = PROJECT / 'Assets/Plugins/MythicalRuntime'
        destination.mkdir(parents=True, exist_ok=True)
        for entry in imports:
            if entry['mode'] == 'staged':
                shutil.copyfile(output / 'staged-imports' / entry['name'], destination / entry['name'])
        (destination / 'imports.json').write_text(json.dumps(imports, indent=2) + '\n')
        report['imports'] = imports
        if sha(ROOT / 'rules/parameters/boat-repair-v1.json') != sha(PROJECT / 'Assets/Game/Resources/Sprint15/boat-repair-v1.json'):
            raise ValueError('Runtime resource profile is stale')
        for method, label in [('Configure', 'configure'), ('PrepareScene', 'scene')]:
            run([editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod',
                 'RepairClientBuild.' + method, '-logFile', output / (label + '-editor.log')], label)
        before_build = sources()
        binary = output / 'Player/MythicalRepair.x86_64'
        run([editor, '-batchmode', '-nographics', '-projectPath', PROJECT, '-executeMethod', 'RepairClientBuild.Build',
             '-s15BuildPath', binary, '-logFile', output / 'build-player-editor.log'], 'build-player', timeout=1800)
        if not binary.is_file():
            raise ValueError('No actual Player binary was produced')
        manifest = sources()
        if manifest != before_build:
            raise ValueError('Sources changed during Player build; rebuild the frozen source')
        (output / 'sources.json').write_text(json.dumps(manifest, indent=2, sort_keys=True) + '\n')
        report['source_fingerprint'] = hashlib.sha256(json.dumps(manifest, sort_keys=True).encode()).hexdigest()
        report['player_sha256'] = sha(binary)
        report['managed_assembly_sha256'] = sha(binary.parent / 'MythicalRepair_Data/Managed/Assembly-CSharp.dll')
        report['editor_sha256'] = sha(editor)
        report['project_version'] = (PROJECT / 'ProjectSettings/ProjectVersion.txt').read_text()
        qa = output / 'autoplay'
        run([binary, '-screen-width', '1280', '-screen-height', '720', '-screen-fullscreen', '0',
             '-logFile', output / 'player-runtime.log', '-s15Autoplay', qa,
             '-s15SaveRoot', output / 'saves'], 'player', timeout=240)
        result_file = qa / 'qa.json'
        result = json.loads(result_file.read_text())
        report['autoplay'] = result
        if result.get('passed') is not True:
            raise ValueError('Actual Player autoplay did not pass')
        if result.get('unityVersion') != '6000.6.7f1' or result.get('platform') != 'LinuxPlayer' or result.get('backend') != 'Mono':
            raise ValueError('Actual Player runtime/profile differs from the selected target')
        required_flags = ['modalBlocksScene', 'purePreview', 'startValid', 'retryValid', 'snapshotValid',
                          'reopenRunning', 'beforeDueValid', 'completionValid', 'cancelValid', 'waitValid',
                          'safePicking', 'occlusionBefore', 'occlusionBehind', 'textScaleValid', 'renderedCaptures',
                          'modalBlocksActions', 'modalFocusContained', 'modalBackgroundSubmitBlocked',
                          'workingFeedbackValid', 'mainPanelScrollValid', 'completionStableAfterCaptures', 'noDeliveredExternalInput', 'autoplayInputIsolated']
        if not all(result.get(flag) is True for flag in required_flags):
            raise ValueError('Actual Player report is missing a required positive check')
        if result.get('keyboardEvents') != 0:
            raise ValueError('Real keyboard actions reached callback-only autoplay')
        layouts = result.get('layouts', [])
        expected_layouts = {(w, h, scale) for w, h in [(1280, 720), (1920, 1080)] for scale in [1, 1.25, 1.5]}
        if len(layouts) != 6 or {(x['width'], x['height'], x['scale']) for x in layouts if x.get('fits') is True} != expected_layouts:
            raise ValueError('Missing required six actual layout cases')
        expected_frames = {
            'initial': (0, 10, 0, 500, 'none'), 'started': (0, 10, 2, 500, 'running'),
            'beforeDue': (59, 10, 2, 500, 'running'), 'completed': (60, 8, 0, 700, 'completed'),
            'cancelled': (0, 10, 0, 500, 'cancelled'), 'waited': (60, 10, 0, 500, 'none')}
        for key, expected in expected_frames.items():
            actual = result.get(key, {})
            if tuple(actual.get(field) for field in ['tick', 'timber', 'reserved', 'condition', 'activity']) != expected:
                raise ValueError('Player effects differ from authored fixture expectations: ' + key)
        database_reports = []
        for filename, tick in [('world.db', 60), ('qa-cancel.db', 0), ('qa-wait.db', 60)]:
            db = output / 'saves' / filename
            with sqlite3.connect(db.as_uri() + '?mode=ro', uri=True) as connection:
                row = connection.execute('SELECT world_id,tick,schema_version,rule_version,content_hash FROM world_meta WHERE singleton=1').fetchone()
                seed = connection.execute('SELECT world_seed FROM save_manifest WHERE singleton=1').fetchone()
                final_state = inspect_final_database(connection, filename)
            if row is None or row[:4] != ('000000000000000f', tick, 2, 'transfer-resource-v1+scheduler-v1+boat-repair-v1') or seed != ('sprint15-fixed-worksite-v1',):
                raise ValueError('Player did not persist the expected fixed fixture/profile: ' + filename)
            database_reports.append(dict(file=filename, world_id=row[0], tick=row[1], schema=row[2], rule=row[3], content_hash=row[4], sha256=sha(db), final_state=final_state))
        report['sqlite_files'] = database_reports
        captures = []
        for path in sorted(qa.glob('*.png')):
            captures.append(dict(file=str(path.relative_to(output)), dimensions=png_size(path), sha256=sha(path)))
        if not {(1280, 720), (1920, 1080)}.issubset({tuple(c['dimensions']) for c in captures}):
            raise ValueError('Missing actual captures at both required resolutions')
        expected_captures = {f'capture-{w}-preview-{scale}.png' for w in [1280, 1920] for scale in [100, 125, 150]}
        expected_captures |= {'capture-1280-initial.png', 'capture-1280-scroll-bottom.png', 'capture-1920-completed-125.png',
                              'capture-1920-occlusion-front.png', 'capture-1920-occlusion-behind.png', 'capture-1920-waited.png'}
        if {Path(c['file']).name for c in captures} != expected_captures:
            raise ValueError('Missing rendered UI/occlusion evidence')
        report['captures'] = captures
        if sources() != manifest:
            raise ValueError('Client/runtime sources changed during Player acceptance')
        report.update(status='Passed', exit_code=0,
                      evidence_type='automated runtime callbacks and rendered captures; OS input and visual review recorded separately')
        code = 0
    except (OSError, ValueError, subprocess.SubprocessError, KeyError, IndexError, UnicodeError, sqlite3.Error) as error:
        report.update(status='Failed', exit_code=1, error=str(error))
        print(str(error), file=sys.stderr)
    report['utc_finished'] = datetime.now(timezone.utc).isoformat()
    (output / 'evidence.json').write_text(json.dumps(report, indent=2) + '\n')
    print('Evidence: ' + str(output / 'evidence.json'), flush=True)
    return code


if __name__ == '__main__':
    sys.exit(main())
