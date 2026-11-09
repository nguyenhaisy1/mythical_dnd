#!/usr/bin/env python3
"""Actual origin-cohort processes, deterministic save/reopen and SIGKILL publication gates."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import tempfile
import time

ROOT = Path(__file__).resolve().parents[2]
SEED_ZERO = '0' * 64
SEED_ONE = '1' * 64
REQUEST_A = 'a' * 32
REQUEST_B = 'b' * 32


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def published(root):
    return {p.name: sha(p) for p in sorted(root.glob('origin-*.db'))}


def run(dotnet, destination):
    destination = Path(destination)
    destination.parent.mkdir(parents=True, exist_ok=True)
    folder = Path(tempfile.mkdtemp(prefix='origin-process-', dir=destination.parent))
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    pack = ROOT / 'content/definitions/vertical-slice/pack.json'
    base = [str(dotnet), str(host)]
    report = dict(scope='headless origin-cohort-v1 actual processes/SQLite/SIGKILL publication; not full life generation, origin command replay, Unity or power-loss',
                  status='Failed', artifact_root=str(folder.relative_to(ROOT)), calls=[], positive_runs=[], crash_cases=[], negative_controls=[])

    def invoke(mode, arguments, label, success=True, stderr=False):
        command = base + [mode, str(pack)] + list(map(str, arguments))
        result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, timeout=120)
        (folder / (label + '.stdout.json')).write_text(result.stdout)
        (folder / (label + '.stderr.json')).write_text(result.stderr)
        report['calls'].append(dict(label=label, command=command, exit_code=result.returncode))
        if result.returncode != (0 if success else 1):
            raise ValueError('Origin process exit mismatch: ' + label + ': ' + result.stderr)
        value = json.loads(result.stderr if stderr else result.stdout)
        if mode != '--origin-export' and value.get('passed') != success:
            raise ValueError('Origin success marker mismatch: ' + label)
        return value

    def probe(root, seed, request, label, count=1):
        value = invoke('--origin-probe', [root, seed, request], label)
        vector = json.loads((ROOT / 'content/fixtures/origin-cohort/selector-vectors.json').read_text())['vectors']
        answer = next(v for v in vector if v['seed'] == seed)
        flags = ['binding', 'idempotentNew', 'pureRead', 'retained', 'foreignRefused', 'unchanged']
        if (any(value.get(flag) is not True for flag in flags)
                or (value['actor'], value['world'], value['drawIndex'], value['tick'], value['candidateCount'], value['publishedFiles'], value['seedCalls'])
                != (answer['selected'], answer['world'], answer['draw_index'], 61, 8, count, 1)):
            raise ValueError('Origin probe differs from reviewed independent answer: ' + label)
        return value

    def opened(root, label, expected):
        before = published(root)
        value = invoke('--origin-open', [root], label)
        truth = value['truth']
        if any(truth.get(k) != v for k, v in expected.items()) or before != published(root):
            raise ValueError('Origin Continue mutated or returned a different campaign: ' + label)
        return value

    def kill_at(root, seed, request, point, label):
        ready = folder / (label + '.ready')
        command = base + ['--origin-crash', str(pack), str(root), seed, request, point, str(ready)]
        with (folder / (label + '.stdout.log')).open('w') as stdout, (folder / (label + '.stderr.log')).open('w') as stderr:
            child = subprocess.Popen(command, cwd=ROOT, stdout=stdout, stderr=stderr)
            try:
                deadline = time.monotonic() + 45
                while not ready.exists():
                    if child.poll() is not None:
                        raise ValueError('Crash process exited before observed barrier: ' + point)
                    if time.monotonic() >= deadline:
                        raise ValueError('Origin crash barrier timeout: ' + point)
                    time.sleep(0.02)
                if ready.read_text() != point:
                    raise ValueError('Unexpected crash barrier content')
                child.kill()
                child.wait(timeout=10)
                if child.returncode != -9:
                    raise ValueError('Origin process was not terminated by SIGKILL')
                report['calls'].append(dict(label=label, command=command, exit_code=child.returncode))
            finally:
                if child.poll() is None:
                    child.kill()
                    child.wait(timeout=10)
        return child.returncode

    try:
        exported = invoke('--origin-export', [], 'export')
        resource = ROOT / 'client/UnityProject/Assets/Game/Resources/Sprint16/origin-cohort-v1.json'
        if exported != json.loads(resource.read_text()):
            raise ValueError('C# derived export differs from committed Unity resource')
        report['resource'] = dict(records=len(exported['records']), source_hash=exported['source_hash'], resource_sha256=sha(resource))
        first = probe(folder / 'seed-zero-a', SEED_ZERO, REQUEST_A, 'zero-a')
        second = probe(folder / 'seed-zero-b', SEED_ZERO, REQUEST_B, 'zero-b')
        different = probe(folder / 'seed-one', SEED_ONE, REQUEST_A, 'one')
        if (first['initialHash'], first['finalHash']) != (second['initialHash'], second['finalHash']):
            raise ValueError('Same seed differs across fresh process, path or publication request')
        if first['actor'] == different['actor'] or first['world'] == different['world'] or first['initialHash'] == different['initialHash']:
            raise ValueError('Different known-answer seeds did not produce different authored origins')
        report['positive_runs'] = [first, second, different]
        opened(folder / 'seed-zero-a', 'open-zero', dict(actor=first['actor'], tick=61, drawIndex=1, hash=first['finalHash'], seed=SEED_ZERO))

        invalid_root = folder / 'invalid-seed'
        invalid = invoke('--origin-probe', [invalid_root, 'A' * 64, REQUEST_A], 'invalid-seed', success=False, stderr=True)
        if published(invalid_root) or list(invalid_root.glob('.pending-*.db')):
            raise ValueError('Invalid seed published or left pending a campaign')
        report['negative_controls'].append(dict(case='invalid-seed', error=invalid['error'], published_files=0))
        invalid_request_root = folder / 'invalid-request'
        invalid = invoke('--origin-probe', [invalid_request_root, SEED_ZERO, 'not-a-request'], 'invalid-request', success=False, stderr=True)
        if invalid_request_root.exists():
            raise ValueError('Invalid request changed the filesystem')
        report['negative_controls'].append(dict(case='invalid-request', error=invalid['error'], created_root=False))

        corrupt_root = folder / 'corrupt-latest'
        baseline = probe(corrupt_root, SEED_ZERO, REQUEST_A, 'corrupt-baseline')
        (corrupt_root / ('origin-0000000002-' + REQUEST_B + '.db')).write_bytes(b'not a SQLite campaign')
        before = published(corrupt_root)
        refused = invoke('--origin-open', [corrupt_root], 'corrupt-refused', success=False)
        if refused.get('code') != 'storage_error' or refused.get('truth') is not None or before != published(corrupt_root):
            raise ValueError('Corrupt latest was mutated, exposed or silently fell back')
        report['negative_controls'].append(dict(case='corrupt-latest-fail-closed', code=refused['code'], published_unchanged=True, files=before))

        for point in ['after_initialize', 'before_publish']:
            root = folder / ('crash-' + point)
            baseline = probe(root, SEED_ZERO, REQUEST_A, point + '-baseline')
            before = published(root)
            signal = kill_at(root, SEED_ONE, REQUEST_B, point, point + '-kill')
            pending = {p.name: sha(p) for p in root.glob('.pending-*.db')}
            if len(pending) != 1 or published(root) != before:
                raise ValueError('Prepublish crash changed published catalog or lacked pending artifact: ' + point)
            opened(root, point + '-open-old', dict(actor=baseline['actor'], tick=61, drawIndex=1, hash=baseline['finalHash'], seed=SEED_ZERO))
            if pending != {p.name: sha(p) for p in root.glob('.pending-*.db')}:
                raise ValueError('Continue altered crash orphan pending file')
            retry = probe(root, SEED_ONE, REQUEST_B, point + '-retry', count=2)
            after = published(root)
            if not all(after.get(name) == digest for name, digest in before.items()) or pending != {p.name: sha(p) for p in root.glob('.pending-*.db')}:
                raise ValueError('Retry overwrote prior campaign or deleted crash orphan')
            opened(root, point + '-open-new', dict(actor=retry['actor'], tick=61, drawIndex=1, hash=retry['finalHash'], seed=SEED_ONE))
            report['crash_cases'].append(dict(phase=point, signal=signal, baseline_unchanged=True, pending_ignored_and_preserved=True,
                                              retry_published_files=len(after), baseline_actor=baseline['actor'], retry_actor=retry['actor']))

        root = folder / 'crash-after_publish'
        signal = kill_at(root, SEED_ONE, REQUEST_B, 'after_publish', 'after_publish-kill')
        if len(published(root)) != 1 or list(root.glob('.pending-*.db')):
            raise ValueError('Crash after publication lost committed campaign or left pending')
        value = opened(root, 'after_publish-open', dict(actor=different['actor'], tick=0, drawIndex=1, hash=different['initialHash'], seed=SEED_ONE))
        report['crash_cases'].append(dict(phase='after_publish', signal=signal, published_files=1, actor=value['truth']['actor'], tick=0, draw_index=1))
        report['status'] = 'Passed'
    except (ValueError, OSError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
        raise
    finally:
        destination.write_text(json.dumps(report, indent=2) + '\n')
    return report


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    print(json.dumps(run(args.dotnet, args.output), indent=2))
