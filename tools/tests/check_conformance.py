"""Host probe and validator tests only. Synthetic reports are NOT Unity evidence."""
import copy
import importlib.util
import json
from pathlib import Path
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[2]
spec = importlib.util.spec_from_file_location('unity_runner', ROOT / 'tools/unity/run_conformance.py')
runner = importlib.util.module_from_spec(spec)
spec.loader.exec_module(runner)


def run(dotnet, evidence):
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    fixture = ROOT / 'content/fixtures/replay-kernel/golden.mdc'
    calls = []
    with tempfile.TemporaryDirectory(prefix='conformance-') as tmp:
        folder = Path(tmp)
        def invoke(source, db, expected):
            p = subprocess.run([str(dotnet), str(host), '--conformance', str(source), str(db)], capture_output=True, text=True, timeout=60)
            if p.returncode != expected:
                raise ValueError('Unexpected conformance exit: ' + p.stderr)
            calls.append(p.returncode)
            return json.loads(p.stdout if expected == 0 else p.stderr)
        first = invoke(fixture, folder / 'a.db', 0)
        second = invoke(fixture, folder / 'b.db', 0)
        if first != second or first['fingerprint'] != runner.EXPECTED_FINGERPRINT:
            raise ValueError('Probe is not deterministic or differs from expected results')
        before = (folder / 'a.db').read_bytes()
        if invoke(fixture, folder / 'a.db', 1)['error'] != 'CONFORMANCE_REQUIRES_NEW_DATABASE' or (folder / 'a.db').read_bytes() != before:
            raise ValueError('Existing database was not preserved')
        bad = folder / 'bad.mdc'; bad.write_bytes(b'corrupt')
        if invoke(bad, folder / 'bad.db', 1)['error'] != 'CONFORMANCE_FIXTURE_HASH' or (folder / 'bad.db').exists():
            raise ValueError('Corrupt fixture was not rejected before writes')
        # Validator unit tests, deliberately synthetic: never label these Editor runs.
        unity = dict(first, unityVersion='6000.3.0f1', backend='Editor-Mono', platform='LinuxEditor', api='NET_Standard')
        version = 'm_EditorVersion: 6000.3.0f1\n'
        runner.parity(first, unity, version)
        negatives = 0
        for key, value in [('status','NotRun'), ('profile','unknown'), ('checks',['x']*5), ('fingerprint','fake'), ('sqliteVersion','other'), ('backend','IL2CPP'), ('platform','WindowsEditor'), ('api','NET_4_6'), ('unityVersion','6000.3.1f1')]:
            candidate = copy.deepcopy(unity); candidate[key] = value
            try: runner.parity(first, candidate, version)
            except ValueError: negatives += 1
            else: raise ValueError('Validator accepted mismatch: ' + key)
        both = dict(first, checks=['x']*5, fingerprint='fake')
        try: runner.parity(both, dict(unity, checks=['x']*5, fingerprint='fake'), version)
        except ValueError: negatives += 1
        else: raise ValueError('Validator accepted matching fabricated results')
        missing = subprocess.run([sys.executable, str(ROOT / 'tools/unity/run_conformance.py'), '--editor', str(folder/'missing'), '--output', str(folder/'absent')], capture_output=True, text=True, timeout=30)
        report = json.loads(missing.stdout)
        if missing.returncode != 2 or report['status'] != 'NotRun' or report['unity_status'] != 'NotRun' or (folder/'absent/UnityProject').exists():
            raise ValueError('Missing Editor was not explicitly NotRun')
        imports = runner.stage(folder/'imports')
        if not any(x['name'] == 'libe_sqlite3.so' for x in imports):
            raise ValueError('Native SQLite import missing')
    result = dict(scope='host and tooling only', unity_status='NotRun', host_processes=len(calls), host_exit_codes=calls, parity_negative_cases=negatives, missing_editor_exit=2, import_count=len(imports), fingerprint=first['fingerprint'])
    evidence.write_text(json.dumps(result, indent=2)+'\n')
    return result
