#!/usr/bin/env python3
"""Build and run runtime acceptance; reject missing/zero/skipped TRX and verify an intentional failure."""
import argparse
from check_persistence_crash import run as run_crash_checks
from check_save_crash import run as run_save_crash_checks
from check_replay import run as run_replay_checks
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import platform
import subprocess
import sys
import tempfile
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
NS = {'t': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
PROJECT = 'tests/MythicalDnd.Foundation.Tests/MythicalDnd.Foundation.Tests.csproj'


def source_manifest():
    paths = [p for folder in ('engine', 'tests', 'tools/tests', 'content/fixtures/kernel-smoke', 'content/fixtures/replay-kernel')
             for p in (ROOT / folder).rglob('*') if p.is_file()
             and not any(part in ('bin', 'obj', 'results', '__pycache__') for part in p.relative_to(ROOT).parts)]
    paths += [ROOT / p for p in ('Directory.Build.props', 'global.json', 'MythicalDnd.sln', '.github/workflows/validate.yml')]
    return {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted(paths)}


def counts(path):
    tree = ET.parse(path)
    c = tree.find('t:ResultSummary/t:Counters', NS)
    if c is None:
        raise ValueError('Missing TRX counters')
    total, passed, failed, executed = (int(c.attrib[k]) for k in ('total', 'passed', 'failed', 'executed'))
    results = tree.findall('t:Results/t:UnitTestResult', NS)
    if len(results) != total or sum(r.attrib['outcome'] == 'Passed' for r in results) != passed or sum(r.attrib['outcome'] == 'Failed' for r in results) != failed:
        raise ValueError('TRX counters/results mismatch')
    return {'discovered': total, 'passed': passed, 'failed': failed, 'skipped': total - executed}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet')
    args = parser.parse_args()
    destination = ROOT / 'tests/results'
    destination.mkdir(parents=True, exist_ok=True)
    run_dir = Path(tempfile.mkdtemp(prefix='sprint07-', dir=destination))
    started = datetime.now(timezone.utc).isoformat()
    manifest = source_manifest()
    commands = []

    def run(arguments, label, env=None):
        command = [args.dotnet] + arguments
        commands.append(command)
        process = subprocess.run(command, cwd=ROOT, env=env, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        (run_dir / (label + '.log')).write_text(process.stdout)
        print(process.stdout, flush=True)
        return process.returncode

    result = dict(scope='headless-sprint07-replay', task_ids=['FND-02', 'FND-03', 'FND-04', 'FND-05', 'FND-06', 'FND-07', 'FND-08', 'FND-09', 'FND-10', 'FND-11', 'FND-12', 'FND-13'], test_ids=['TEST-027', 'TEST-008-primitives', 'TEST-007', 'TEST-024', 'TEST-025', 'TEST-026', 'FND-08-in-memory', 'TEST-037-in-memory', 'FND-10-in-memory', 'TEST-006-durable', 'TEST-034', 'TEST-035-current-image', 'TEST-035-snapshot', 'TEST-051', 'TEST-009', 'TEST-050'], source_revision='sha256:' + hashlib.sha256(json.dumps(manifest, sort_keys=True).encode()).hexdigest(),
                  utc_started=started, os=platform.platform(), architecture=platform.machine(),
                  runtime_version=None, unity_version=None, backend=None,
                  fixture_versions={'identity': 'identity-v1', 'clock': 'clock-v1', 'codec': 'mdc1', 'rng': 'sha256-counter-v1', 'scenario': 1},
                  content_hash=manifest['content/fixtures/kernel-smoke/scenario.json'], rule_version=None, rng_version='sha256-counter-v1', seed='golden contexts in vectors.json',
                  discovered=0, passed=0, failed=0, skipped=0, exit_code=1, commands=commands)
    (run_dir / 'sources.json').write_text(json.dumps(manifest, indent=2) + '\n')
    try:
        result['runtime_version'] = subprocess.check_output([args.dotnet, '--info'], cwd=ROOT, text=True)
        if run(['restore', 'MythicalDnd.sln', '--locked-mode', '-m:1', '-nr:false'], 'restore') != 0:
            raise ValueError('Locked restore failed')
        if run(['build', 'MythicalDnd.sln', '-c', 'Release', '--no-restore', '-m:1', '-nr:false'], 'build') != 0:
            raise ValueError('Build failed')
        common = ['test', PROJECT, '-c', 'Release', '--no-build', '--no-restore', '-m:1', '-nr:false', '--results-directory', str(run_dir)]
        clean_env = dict(os.environ)
        clean_env.pop('MYTHICAL_NEGATIVE_CONTROL', None)
        code = run(common + ['--logger', 'trx;LogFileName=runtime.trx'], 'runtime', clean_env)
        result.update(counts(run_dir / 'runtime.trx'))
        result['runtime_exit_code'] = code
        test_results = ET.parse(run_dir / 'runtime.trx').findall('t:Results/t:UnitTestResult', NS)
        result['task_counts'] = {}
        for task, prefix in [('FND-02', 'TEST_027_'), ('FND-03', 'TEST_008_'), ('FND-04', 'TEST_007_'), ('FND-05', 'FND_05_'), ('FND-06', 'FND_06_'), ('FND-07/idempotency', 'TEST_024_'), ('FND-07/mismatch', 'TEST_025_'), ('FND-07/stale', 'TEST_026_'), ('FND-08', 'FND_08_'), ('FND-09', 'TEST_037_'), ('FND-10', 'FND_10_'), ('FND-10/workflow', 'TEST_028_'), ('FND-11', 'FND_11_'), ('FND-12', 'FND_12_'), ('FND-13', 'FND_13_')]:
            matched = [r for r in test_results if r.attrib.get('testName', '').startswith(prefix)]
            result['task_counts'][task] = {'discovered': len(matched), 'passed': sum(r.attrib['outcome'] == 'Passed' for r in matched)}
            if not matched or any(r.attrib['outcome'] != 'Passed' for r in matched):
                raise ValueError('Missing or unsuccessful task tests: ' + task)
        if code != 0 or result['discovered'] <= 0 or result['passed'] != result['discovered'] or result['failed'] or result['skipped']:
            raise ValueError('Runtime gate failed (nonzero, failed, skipped or zero tests)')
        control_env = dict(clean_env, MYTHICAL_NEGATIVE_CONTROL='1')
        code = run(common + ['--filter', 'TestCategory=NegativeControl', '--logger', 'trx;LogFileName=negative.trx'], 'negative', control_env)
        control = counts(run_dir / 'negative.trx')
        result['negative_control'] = dict(control, exit_code=code)
        if code == 0 or control != {'discovered': 1, 'passed': 0, 'failed': 1, 'skipped': 0}:
            raise ValueError('Negative control did not fail exactly one discovered test')
        demo_code = run(['run', '--project', 'engine/HeadlessRunner/Game.Headless.csproj', '-c', 'Release', '--no-build', '--', '--transfer-demo'], 'transfer-demo')
        demo = json.loads((run_dir / 'transfer-demo.log').read_text())
        result['transfer_demo'] = dict(demo, exit_code=demo_code)
        if demo_code != 0 or demo.get('passed') is not True or demo.get('sourceAfter') != 7 or demo.get('destinationAfter') != 3 or demo.get('events') != 1 or demo.get('revision') != 2 or demo.get('acceptedReceipts') != 1 or demo.get('retryReturnsOriginalReceipt') is not True or demo.get('projectionTransferred') != 3 or demo.get('duplicateDeliveryApplied') is not False or demo.get('consumerCheckpoints') != 1:
            raise ValueError('Headless transfer demo failed')
        scheduler_code = run(['run', '--project', 'engine/HeadlessRunner/Game.Headless.csproj', '-c', 'Release', '--no-build', '--', '--scheduler-demo'], 'scheduler-demo')
        scheduler = json.loads((run_dir / 'scheduler-demo.log').read_text())
        result['scheduler_demo'] = dict(scheduler, exit_code=scheduler_code)
        if scheduler_code != 0 or scheduler.get('passed') is not True or scheduler.get('currentTick') != 30 or scheduler.get('revision') != 7 or scheduler.get('sourceAfter') != 5 or scheduler.get('destinationAfter') != 5 or scheduler.get('executedJobs') != 2 or scheduler.get('dueTicks') != [10, 20] or scheduler.get('workflowStatus') != 'Completed' or scheduler.get('retryReturnsOriginalReceipt') is not True:
            raise ValueError('Headless scheduler demo failed')
        result['persistence_crash'] = run_crash_checks(args.dotnet, run_dir / 'persistence-crash.json')
        result['save_crash'] = run_save_crash_checks(args.dotnet, run_dir / 'save-crash.json')
        result['replay'] = run_replay_checks(args.dotnet, run_dir / 'replay.json')
        if source_manifest() != manifest:
            raise ValueError('Sources changed during run')
        result['exit_code'] = 0
    except (ValueError, OSError, subprocess.CalledProcessError, ET.ParseError) as error:
        result['error'] = str(error)
        print(error, file=sys.stderr)
    result['artifact_paths'] = [str(p.relative_to(ROOT)) for p in sorted(run_dir.iterdir())] + [str((run_dir / 'evidence.json').relative_to(ROOT))]
    (run_dir / 'evidence.json').write_text(json.dumps(result, indent=2) + '\n')
    print('Evidence: ' + str(run_dir / 'evidence.json'))
    return result['exit_code']


if __name__ == '__main__':
    sys.exit(main())
