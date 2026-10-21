#!/usr/bin/env python3
"""Run a checked-in replay golden in independent processes; never regenerate it."""
import hashlib
import json
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def run(dotnet, destination):
    fixture = ROOT / 'content/fixtures/replay-kernel'
    expected = json.loads((fixture / 'golden.json').read_text())
    artifact = fixture / 'golden.mdc'
    if hashlib.sha256(artifact.read_bytes()).hexdigest() != expected['artifact_sha256']:
        raise ValueError('Replay golden artifact changed')
    if hashlib.sha256((fixture / 'scenario.json').read_bytes()).hexdigest() != expected['content_hash']:
        raise ValueError('Replay fixture content changed')
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    reports = []
    for attempt in range(2):
        result = subprocess.run([dotnet, str(host), '--replay', str(artifact), expected['content_hash'], expected['seed']],
                                capture_output=True, text=True)
        if result.returncode != 0:
            raise ValueError('Golden replay process failed: ' + result.stderr)
        report = json.loads(result.stdout)
        if report != expected['expected'] or report['actions'] <= 0:
            raise ValueError('Golden replay diverged')
        reports.append(dict(process=attempt + 1, exit_code=result.returncode, **report))
    negative = subprocess.run([dotnet, str(host), '--replay-negative-control', str(artifact),
                               expected['content_hash'], expected['seed']], capture_output=True, text=True)
    diagnostic = json.loads(negative.stdout)
    if negative.returncode != 1 or diagnostic['passed'] or diagnostic['divergence']['Index'] != 0 or diagnostic['divergence']['Category'] != 'result':
        raise ValueError('Replay negative control did not detect first checkpoint mismatch')
    result = {
        'scope': 'headless transfer/scheduler rule rerun; not Unity or stochastic gameplay',
        'artifact_sha256': expected['artifact_sha256'],
        'runs': reports,
        'negative_control': dict(exit_code=negative.returncode, **diagnostic),
    }
    destination.write_text(json.dumps(result, indent=2) + '\n')
    return result
