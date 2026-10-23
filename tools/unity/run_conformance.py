#!/usr/bin/env python3
"""Candidate Linux Unity6 Editor-Mono conformance runner. Missing Editor is NotRun, never Passed."""
import argparse
import base64
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import subprocess
import sys
import tempfile
import zipfile

ROOT = Path(__file__).resolve().parents[2]
EXPECTED_FINGERPRINT = 'd47e710ae21268d937742dc3fda208ae5e58b220309eeb2b276697a5367372f9'
TEMPLATES = Path(__file__).resolve().parent / 'templates'


def digest(path, algorithm='sha256'):
    h = hashlib.new(algorithm)
    with path.open('rb') as f:
        for block in iter(lambda: f.read(1024 * 1024), b''):
            h.update(block)
    return h.digest()


def stage(destination, editor=None):
    destination.mkdir(parents=True, exist_ok=False)
    manifest = []
    names = set()
    lock = json.loads((ROOT / 'engine/Infrastructure/packages.lock.json').read_text())['dependencies']['.NETStandard,Version=v2.1']
    package_root = Path(os.environ.get('NUGET_PACKAGES', str(Path.home() / '.nuget/packages')))

    def write(name, data, source, external=None):
        target = destination / name
        if name in names or target.exists():
            raise ValueError('Duplicate import basename: ' + name)
        names.add(name)
        sha = hashlib.sha256(data).hexdigest()
        if external:
            manifest.append(dict(name=name, source=source, package_sha256=sha, mode='editor-provided', editor_path=str(external), sha256=digest(external).hex()))
        else:
            target.write_bytes(data)
            manifest.append(dict(name=name, source=source, mode='staged', sha256=sha))

    for assembly in ['Foundation', 'Domain', 'Application', 'Infrastructure']:
        path = ROOT / f'engine/{assembly}/bin/Release/netstandard2.1/Game.{assembly}.dll'
        write(path.name, path.read_bytes(), str(path.relative_to(ROOT)))
    for name, info in sorted(lock.items()):
        if 'resolved' not in info:
            continue
        package = package_root / name.lower() / info['resolved']
        archive = package / (name.lower() + '.' + info['resolved'] + '.nupkg')
        # NuGet lock contentHash can differ from the signed archive hash.
        metadata = json.loads((package / '.nupkg.metadata').read_text())
        archive_hash = (package / (archive.name + '.sha512')).read_text().strip()
        if metadata.get('contentHash') != info['contentHash'] or base64.b64encode(digest(archive, 'sha512')).decode() != archive_hash:
            raise ValueError('NuGet cache/lock checksum mismatch: ' + name)
        with zipfile.ZipFile(archive) as z:
            files = [p for p in z.namelist() if p.startswith('lib/netstandard2.0/') and p.endswith('.dll')]
            if name == 'SQLitePCLRaw.lib.e_sqlite3':
                files = ['runtimes/linux-x64/native/libe_sqlite3.so']
            for item in files:
                basename = Path(item).name
                external = None
                if editor and basename.startswith('System.'):
                    # Prefer actual runtime-provided assembly over a duplicate NuGet assembly; parity remains mandatory.
                    roots = [editor.parent / 'Data/MonoBleedingEdge/lib/mono/unityjit', editor.parent / 'Data/Managed']
                    candidates = [r / basename for r in roots if (r / basename).is_file()]
                    if len({digest(c) for c in candidates}) > 1:
                        raise ValueError('Ambiguous Editor runtime dependency: ' + basename)
                    external = candidates[0] if candidates else None
                write(basename, z.read(item), name + '@' + info['resolved'] + '/' + item, external)
    return manifest


def parity(host, unity, project_version):
    if host.get('status') != 'Passed' or unity.get('status') != 'Passed':
        raise ValueError('Runtime did not pass')
    if unity.get('backend') != 'Editor-Mono' or unity.get('platform') != 'LinuxEditor' or unity.get('api') != 'NET_Standard':
        raise ValueError('Unsupported actual Unity runtime/profile')
    version = unity.get('unityVersion', '')
    if not version.startswith('6000.') or ('m_EditorVersion: ' + version + '\n') not in project_version:
        raise ValueError('Actual Editor/project version mismatch')
    for key in ['profile', 'checks', 'fingerprint', 'sqliteVersion']:
        if host.get(key) != unity.get(key) or not host.get(key):
            raise ValueError('Host/Unity mismatch: ' + key)
    if host.get('profile') != 'kernel-conformance-v1' or not isinstance(host['checks'], list) or len(host['checks']) != 5:
        raise ValueError('Conformance checks missing')
    checks = host['checks']
    if not all(isinstance(x, str) for x in checks):
        raise ValueError('Invalid check type')
    raw = b'MDC1a5:' + b''.join(b's' + str(len(x.encode())).encode() + b':' + x.encode() for x in checks)
    if hashlib.sha256(raw).hexdigest() != EXPECTED_FINGERPRINT or host['fingerprint'] != EXPECTED_FINGERPRINT:
        raise ValueError('Unknown conformance results/fingerprint')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--editor', type=Path)
    parser.add_argument('--prepare-only', action='store_true')
    parser.add_argument('--output', type=Path, help='New output directory; never reuse existing evidence')
    args = parser.parse_args()
    (ROOT / 'tests/results').mkdir(parents=True, exist_ok=True)
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='unity-', dir=ROOT / 'tests/results'))
    if args.output:
        output.mkdir(parents=True, exist_ok=False)
    report = dict(utc_started=datetime.now(timezone.utc).isoformat(), task_ids=['FND-16'], test_ids=['TEST-016'], source_revision=subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT, text=True).strip(), status='NotRun', unity_status='NotRun', scope='candidate Linux Unity6 Editor-Mono; not Player/IL2CPP', output=str(output))
    source_paths = sorted(p for folder in ['engine', 'tools/unity'] for p in (ROOT / folder).rglob('*') if p.is_file() and not any(x in ['bin','obj','__pycache__'] for x in p.parts))
    report['source_files'] = {str(p.relative_to(ROOT)): digest(p).hex() for p in source_paths}
    report['source_fingerprint'] = hashlib.sha256(json.dumps(report['source_files'], sort_keys=True).encode()).hexdigest()
    report['fixture_versions'] = dict(codec='mdc1', rng='sha256-counter-v1', profile='kernel-conformance-v1', world_seed='0' * 64)
    report['commands'] = []
    exit_code = 2
    try:
        if platform.system() != 'Linux' or platform.machine() not in ['x86_64', 'AMD64']:
            raise ValueError('Only Linux x64 candidate runner is supported')
        if not args.prepare_only and (not args.editor or not args.editor.is_file()):
            report['reason'] = 'Unity Editor unavailable; install and activate through Unity Hub, then provide --editor'
        else:
            editor = args.editor.resolve() if args.editor else None
            if editor and not os.access(editor, os.X_OK):
                raise ValueError('Editor is not executable')
            def run(command, label):
                report['commands'].append(dict(label=label, argv=list(map(str, command))))
                with (output / (label + '.log')).open('w') as log:
                    subprocess.run(list(map(str, command)), cwd=ROOT, stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
            run([args.dotnet, 'restore', 'MythicalDnd.sln', '--locked-mode', '-m:1', '-nr:false'], 'restore')
            run([args.dotnet, 'build', 'MythicalDnd.sln', '-c', 'Release', '--no-restore', '-m:1', '-nr:false'], 'build')
            fixture = ROOT / 'content/fixtures/replay-kernel/golden.mdc'
            run([args.dotnet, ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll', '--conformance', fixture, output / 'host.db'], 'host')
            host = json.loads((output / 'host.log').read_text())
            report['host'] = host
            report['fixture_sha256'] = digest(fixture).hex()
            if args.prepare_only:
                report['imports'] = stage(output / 'candidate-imports')
                report.update(status='Prepared', reason='Host probe and import inventory only; actual Editor not run')
                exit_code = 0
            else:
                project = output / 'UnityProject'
                run([editor, '-batchmode', '-nographics', '-createProject', project, '-quit', '-logFile', output / 'create-editor.log'], 'create')
                version_path = project / 'ProjectSettings/ProjectVersion.txt'
                if not version_path.is_file():
                    raise ValueError('Editor did not create a real project')
                scripts = project / 'Assets/Editor'; scripts.mkdir(parents=True, exist_ok=True)
                shutil.copyfile(TEMPLATES / 'ConformanceBootstrap.cs', scripts / 'ConformanceBootstrap.cs')
                run([editor, '-batchmode', '-nographics', '-projectPath', project, '-executeMethod', 'ConformanceBootstrap.Configure', '-logFile', output / 'configure-editor.log'], 'configure')
                report['imports'] = stage(project / 'Assets/Plugins/MythicalRuntime', editor)
                run([editor, '-batchmode', '-nographics', '-projectPath', project, '-quit', '-logFile', output / 'import-editor.log'], 'import')
                shutil.copyfile(TEMPLATES / 'ConformanceEntry.cs', scripts / 'ConformanceEntry.cs')
                run([editor, '-batchmode', '-nographics', '-projectPath', project, '-executeMethod', 'ConformanceEntry.Run',
                     '-conformanceFixture', fixture, '-conformanceDatabase', output / 'unity.db', '-conformanceOutput', output / 'unity.json', '-logFile', output / 'run-editor.log'], 'unity')
                unity = json.loads((output / 'unity.json').read_text())
                parity(host, unity, version_path.read_text())
                report.update(status='Passed', unity_status='Passed', unity=unity, project_version=version_path.read_text(), editor_sha256=digest(editor).hex())
                report['semantic_checks_passed'] = len(unity['checks'])
                exit_code = 0
    except (OSError, ValueError, subprocess.SubprocessError, zipfile.BadZipFile, KeyError) as error:
        report.update(status='Failed', reason=str(error)); exit_code = 1
    report['utc_finished'] = datetime.now(timezone.utc).isoformat()
    report['exit_code'] = exit_code
    (output / 'evidence.json').write_text(json.dumps(report, indent=2) + '\n')
    print(json.dumps(report))
    return exit_code


if __name__ == '__main__':
    sys.exit(main())
