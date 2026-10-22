#!/usr/bin/env python3
"""Replay a fixed diagnostic evidence package in separate processes; never regenerate it."""
import base64
import copy
import hashlib
import json
from pathlib import Path
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[2]


def run(dotnet, destination):
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    folder = ROOT / 'content/fixtures/diagnostic-kernel'
    content = ROOT / 'content/fixtures/content-kernel/pack.json'
    package = folder / 'package.json'
    golden = json.loads((folder / 'golden.json').read_text())
    if hashlib.sha256(package.read_bytes()).hexdigest() != golden['package_sha256']:
        raise ValueError('Diagnostic package checksum changed')
    calls = []

    def invoke(path, expected=0, marker=None):
        p = subprocess.run([str(dotnet), str(host), '--diagnose', str(path), str(content)], capture_output=True, timeout=30)
        if p.returncode != expected:
            raise ValueError(f'Diagnostic exit {p.returncode}, expected {expected}: {p.stderr.decode()}')
        r = json.loads(p.stdout if expected == 0 else p.stderr)
        if expected and (r['reproduced'] is not False or marker and marker not in r['error']):
            raise ValueError('Diagnostic failure was not explicit')
        calls.append(dict(exit_code=p.returncode, error=r.get('error')))
        return p.stdout, r

    first, report = invoke(package)
    second, _ = invoke(package)
    if first != second or hashlib.sha256(first).hexdigest() != golden['report_sha256']:
        raise ValueError('Diagnostic report differs across process or fixed expected report')
    d = report['divergence']
    if (d['Index'], d['Category'], d['ExpectedRevision'], d['ActualRevision']) != (1, 'result', 2, 1):
        raise ValueError('Wrong first divergence')
    diffs = {x['Path']: x for x in report['expectedActualDiff']['Entries']}
    for index, wanted in enumerate([(7, 10), (3, 0)]):
        row = diffs[f'/lots/{index}/on_hand']
        if (row['Before']['Value'], row['After']['Value']) != wanted:
            raise ValueError('Field diff hides resource discrepancy')
    if len(report['baselineCommands']) != 15 or len(report['actualCommands']) != 2 or len(report['baselineJobs']) != 2 or report['traceDropped']:
        raise ValueError('Missing command/job trace or dropped evaluation')
    actual = report['actualCommands'][1]
    if actual['error'] != 'DIAGNOSTIC_INJECTED' or len(actual['evaluations']) != 1 or not actual['evaluations'][0]['checks']:
        raise ValueError('No actual injected evaluation trace')
    # Scheduler and causal evidence is explicitly labelled baseline, not the fault state.
    if report['actualCausal']['Edges'] or not report['baselineCausal']['Edges']:
        raise ValueError('Baseline/actual causation mixed')
    original = json.loads(package.read_text())
    with tempfile.TemporaryDirectory(prefix='mythical-diagnostic-') as temporary:
        path = Path(temporary) / 'bad.json'
        cases = [
            ('fault', 'script:eval()', 'DIAGNOSTIC_PROFILE'),
            ('package_version', 2, 'DIAGNOSTIC_PROFILE'),
            ('content_hash', '0' * 64, 'CONTENT_INCOMPATIBLE'),
            ('rule_version', 'future', 'DIAGNOSTIC_RULE_VERSION'),
            ('seed', '1' * 64, 'REPLAY_MANIFEST_INCOMPATIBLE'),
            ('artifact_sha256', '0' * 64, 'DIAGNOSTIC_CHECKSUM'),
            ('expected_state_sha256', '0' * 64, 'DIAGNOSTIC_CHECKSUM'),
            ('expected_action', 999, 'DIAGNOSTIC_ACTION_LIMIT'),
            ('expected_category', 'wrong', 'DIAGNOSTIC_NOT_REPRODUCED'),
        ]
        for field, value, marker in cases:
            changed = copy.deepcopy(original); changed[field] = value
            path.write_text(json.dumps(changed)); invoke(path, 1, marker)
        # A different internally-valid expected action image must not pass just because its checksum is valid.
        changed = copy.deepcopy(original); changed['expected_action'] = 0
        path.write_text(json.dumps(changed)); invoke(path, 1, 'DIAGNOSTIC_BASELINE_MISMATCH')
        changed = copy.deepcopy(original); changed['artifact_base64'] = base64.b64encode(b'not mdc1').decode()
        changed['artifact_sha256'] = hashlib.sha256(b'not mdc1').hexdigest()
        path.write_text(json.dumps(changed)); invoke(path, 1)
        path.write_text(package.read_text().replace('"package_version":1', '"package_version":1,"package_version":1', 1))
        invoke(path, 1, 'DIAGNOSTIC_FIELD')
        changed = copy.deepcopy(original); changed['script'] = 'eval()'
        path.write_text(json.dumps(changed)); invoke(path, 1, 'DIAGNOSTIC_FIELD')
        path.write_bytes(b' ' * (16 * 1024 * 1024 + 1)); invoke(path, 1, 'DIAGNOSTIC_SIZE')
    result = dict(scope='headless omniscient diagnostics; fixed evidence and injected rejection; not player UI',
                  package_hash=golden['package_sha256'], report_hash=golden['report_sha256'], first_divergence=d,
                  processes=len(calls), negative_cases=len(calls)-2, checks=calls)
    destination.write_text(json.dumps(result, indent=2) + '\n')
    return result
