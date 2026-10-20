#!/usr/bin/env python3
"""Kill processes during real snapshot/migration writes; verify last good state."""
import json
import select
import subprocess
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def run(dotnet, destination):
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    results = []
    for operation, point in [
        ('snapshot', 'after_snapshot'), ('snapshot', 'before_commit'),
        ('snapshot', 'after_commit'), ('migrate', 'migration_after_backup'),
        ('migrate', 'migration_before_commit'), ('migrate', 'migration_after_copy'),
    ]:
        with tempfile.TemporaryDirectory(prefix='mythical-save-crash-') as folder:
            db = Path(folder) / 'world.db'

            def invoke(path, mode):
                result = subprocess.run([dotnet, str(host), '--save-probe', str(path), mode],
                                        check=True, capture_output=True, text=True)
                return json.loads(result.stdout)

            before = invoke(db, 'init' if operation == 'snapshot' else 'init-legacy')
            original = db.read_bytes()
            child = subprocess.Popen([dotnet, str(host), '--save-probe', str(db), operation, point],
                                     stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
            try:
                ready, _, _ = select.select([child.stdout], [], [], 30)
                if not ready or child.stdout.readline().strip() != 'READY:' + point:
                    raise RuntimeError('Save crash barrier not reached: ' + point)
                child.kill()
                child.wait(timeout=10)
                if child.returncode != -9:
                    raise RuntimeError('Expected SIGKILL')
            finally:
                if child.poll() is None:
                    child.kill()
                    child.wait()
            after = invoke(db, 'inspect')
            if after['stateHash'] != before['stateHash'] or after['revision'] != 2 or after['source'] != 7:
                raise RuntimeError('State changed during save operation')
            if operation == 'snapshot':
                expected = 2 if point == 'after_commit' else 1
                if after['snapshots'] != expected:
                    raise RuntimeError('Partial snapshot publication')
            else:
                if db.read_bytes() != original:
                    raise RuntimeError('Migration modified original')
                backup = invoke(Path(str(db) + '.backup'), 'inspect')
                if backup['stateHash'] != before['stateHash'] or backup['schema'] != 1:
                    raise RuntimeError('Migration backup invalid')
                if point == 'migration_before_commit':
                    incomplete = subprocess.run(
                        [dotnet, str(host), '--save-probe', str(db) + '.migrated', 'inspect'],
                        capture_output=True, text=True)
                    if incomplete.returncode == 0:
                        raise RuntimeError('Incomplete migration accepted')
                if point == 'migration_after_copy':
                    migrated = invoke(Path(str(db) + '.migrated'), 'inspect')
                    if migrated['stateHash'] != before['stateHash'] or migrated['schema'] != 2:
                        raise RuntimeError('Migration output invalid')
            results.append(dict(operation=operation, point=point, signal=-9, passed=True, **after))
    destination.write_text(json.dumps({
        'scope': 'Linux SIGKILL snapshot/migration; not power loss, atomic source replacement or rule replay',
        'cases': results
    }, indent=2) + '\n')
    return results
