#!/usr/bin/env python3
"""Actual personal-weaving processes, original retries and crash/SQLite gates."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import tempfile
import time

ROOT = Path(__file__).resolve().parents[2]
ZERO, ONE, BLOCKED = '0' * 64, '1' * 64, '01234567' * 8
A, B = 'a' * 32, 'b' * 32


def files(root):
    return {p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted(root.glob('origin-*.db'))}


def run(dotnet, destination):
    destination = Path(destination); destination.parent.mkdir(parents=True, exist_ok=True)
    folder = Path(tempfile.mkdtemp(prefix='weaving-process-', dir=destination.parent))
    base = [str(dotnet), str(ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll')]
    sources = [str(ROOT / 'content/definitions/vertical-slice/pack.json'), str(ROOT / 'rules/parameters/personal-weaving-v1.json')]
    report = dict(scope='actual processes/SQLite/SIGKILL personal-weaving-v1; not command replay/Unity/power-loss', status='Failed', artifact_root=str(folder.relative_to(ROOT)), calls=[], scenarios=[], crashes=[])

    def invoke(mode, root, args, label, success=True, stderr=False):
        command = base + [mode] + sources + [str(root)] + list(map(str, args))
        p = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, timeout=120)
        (folder / (label + '.stdout.json')).write_text(p.stdout); (folder / (label + '.stderr.json')).write_text(p.stderr)
        report['calls'].append(dict(label=label, command=command, exit_code=p.returncode))
        if p.returncode != (0 if success else 1):
            raise ValueError('Unexpected child exit: ' + label + ': ' + p.stderr)
        result = json.loads(p.stderr if stderr else p.stdout)
        if result.get('passed') != success:
            raise ValueError('Unexpected child outcome: ' + label)
        return result

    def assert_work(value, expected):
        actual = value['work']
        if tuple(actual[k] for k in ['FiberOnHand', 'FiberReserved', 'CordOnHand', 'Status', 'Tick']) != expected or value['truth']['draw'] != 1:
            raise ValueError('Persisted weaving state/RNG differs: ' + repr(actual))

    def kill(mode, root, args, label, point):
        ready = folder / (label + '.ready')
        arguments = args(ready)
        command = base + [mode] + sources + [str(root)] + list(map(str, arguments))
        with (folder / (label + '.stdout.log')).open('w') as stdout, (folder / (label + '.stderr.log')).open('w') as stderr:
            p = subprocess.Popen(command, cwd=ROOT, stdout=stdout, stderr=stderr)
            try:
                end = time.monotonic() + 45
                while True:
                    try: observed = ready.read_text()
                    except FileNotFoundError: observed = ""
                    if observed == point: break
                    # File creation can precede the small readiness write. Never kill at existence alone.
                    if observed and observed != point: raise ValueError('Wrong actual fault hook: ' + observed)
                    if p.poll() is not None or time.monotonic() >= end:
                        raise ValueError('Child did not reach complete actual fault hook: ' + label)
                    time.sleep(.02)
                p.kill(); p.wait(timeout=10)
                if p.returncode != -9:
                    raise ValueError('Child was not SIGKILLed')
                report['calls'].append(dict(label=label, command=command, exit_code=p.returncode))
            finally:
                if p.poll() is None:
                    p.kill(); p.wait(timeout=10)

    try:
        for label, seed, mode, expected in [('lam-complete', ZERO, 'complete', (6, 0, 1, 'completed', 30)),
                                           ('nhu-complete', ONE, 'complete', (6, 0, 1, 'completed', 30)),
                                           ('lam-cancel', ZERO, 'cancel', (8, 0, 0, 'cancelled', 60)),
                                           ('kien-blocked', BLOCKED, 'blocked', (8, 0, 0, 'idle', 0))]:
            value = invoke('--weaving-probe', folder / label, [seed, A, mode], label)
            assert_work(value, expected); report['scenarios'].append(value)
        same = invoke('--weaving-probe', folder / 'same-seed', [ZERO, B, 'complete'], 'same-seed')
        if same['initialHash'] != report['scenarios'][0]['initialHash'] or same['work'] != report['scenarios'][0]['work']:
            raise ValueError('Same-seed genesis/gameplay outcome drift across fresh paths/requests')
        # Random presentation tokens are part of distinct command input/correlation histories;
        # do not falsely require their final canonical command histories to hash identically.
        report['deterministic_genesis_and_effects'] = True
        invalid_root = folder / 'invalid'
        invoke('--weaving-new', invalid_root, ['A' * 64, A], 'invalid', success=False, stderr=True)
        if files(invalid_root) or list(invalid_root.glob('.pending-*.db')):
            raise ValueError('Invalid seed created a campaign')
        report['invalid_seed_refused'] = True

        for phase in ['after_initialize', 'before_publish', 'after_publish']:
            root = folder / ('new-crash-' + phase)
            old = invoke('--weaving-new', root, [ZERO, A], phase + '-old'); before = files(root)
            kill('--weaving-crash', root, lambda ready: [ONE, B, phase, ready], phase + '-kill', phase)
            opened = invoke('--weaving-open', root, [], phase + '-open')
            expected_actor = 'actor.vac-nhu@1' if phase == 'after_publish' else 'actor.vac-lam@1'
            if opened['truth']['actor'] != expected_actor or not all(files(root).get(n) == h for n, h in before.items()):
                raise ValueError('Campaign publication crash changed prior campaign or lost committed result')
            pending = list(root.glob('.pending-*.db'))
            if len(pending) != (0 if phase == 'after_publish' else 1):
                raise ValueError('Pending ownership/crash evidence differs')
            report['crashes'].append(dict(operation='new', phase=phase, signal=-9, old_campaign_preserved=True, pending_files=len(pending), actor=expected_actor))

        for action in ['start', 'complete']:
            for phase in ['before_commit', 'after_commit']:
                root = folder / (action + '-' + phase)
                if action == 'start':
                    invoke('--weaving-new', root, [ZERO, A], action + phase + '-initial')
                else:
                    invoke('--weaving-probe', root, [ZERO, A, 'before-due'], action + phase + '-initial')
                kill('--weaving-action-crash', root, lambda ready: [phase, ready, action], action + phase + '-kill', phase)
                raw = invoke('--weaving-inspect', root, [], action + phase + '-raw')
                opened = invoke('--weaving-open', root, [], action + phase + '-open')
                if action == 'start':
                    assert_work(opened, (8, 2 if phase == 'after_commit' else 0, 0, 'running' if phase == 'after_commit' else 'idle', 0))
                    if phase == 'after_commit':
                        retry = invoke('--weaving-retry-start', root, [], 'response-loss-retry')
                        if not retry['unchanged']:
                            raise ValueError('Response-loss start retry duplicated effects')
                else:
                    assert_work(raw, (6, 0, 1, 'completed', 30) if phase == 'after_commit' else (8, 2, 0, 'running', 29))
                    # Continue safely drains the already accepted advance intent, once.
                    assert_work(opened, (6, 0, 1, 'completed', 30))
                report['crashes'].append(dict(operation=action, phase=phase, signal=-9, raw_work=raw['work'], work=opened['work'], truth=opened['truth']))
        report['status'] = 'Passed'
    except (ValueError, OSError, subprocess.SubprocessError) as error:
        report['error'] = str(error); raise
    finally:
        destination.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    return report


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet'); parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args(); print(json.dumps(run(args.dotnet, args.output), ensure_ascii=False, indent=2))
