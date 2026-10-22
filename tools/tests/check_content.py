#!/usr/bin/env python3
"""Execute the content compiler/registry/save boundary in fresh headless processes."""
import copy
import hashlib
import json
import subprocess
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def run(dotnet, destination):
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    fixture = ROOT / 'content/fixtures/content-kernel/pack.json'
    original = json.loads(fixture.read_text())
    reports = []

    def invoke(*args, success=True, diagnostic=None):
        process = subprocess.run([str(dotnet), str(host), *map(str, args)], capture_output=True, text=True, timeout=30)
        expected = 0 if success else 1
        if process.returncode != expected:
            raise ValueError(f'Content process exit {process.returncode}, expected {expected}: {process.stderr}')
        report = json.loads(process.stdout if success else process.stderr)
        if not success and (report.get('passed') is not False or diagnostic and diagnostic not in report['error']):
            raise ValueError('Content diagnostic mismatch: ' + json.dumps(report))
        reports.append(dict(mode=args[0], exit_code=process.returncode, report=report))
        return report

    with tempfile.TemporaryDirectory(prefix='mythical-content-gate-') as temporary:
        folder = Path(temporary)

        def write(name, value):
            path = folder / (name + '.json')
            path.write_text(json.dumps(value))
            return path

        compiled = invoke('--content-compile', fixture)
        if invoke('--content-compile', fixture) != compiled:
            raise ValueError('Content compilation differs between independent processes')
        if compiled['definitionIds'] != ['commodity.timber@1', 'transfer.timber@1'] or compiled['ruleReferences'] != ['TransferResource@1']:
            raise ValueError('Compiled manifest is missing exact approved IDs')
        changed_order = copy.deepcopy(original)
        changed_order['definitions'].reverse()
        if invoke('--content-compile', write('reordered', changed_order)) != compiled:
            raise ValueError('Definition authoring order changed compiled identity')
        content_hash = compiled['contentHash']
        if content_hash != 'e45fa90669459e4516323a71ff959d7f9b6dd637d7e33c148306ba0cb9808b72':
            raise ValueError('Compiled fixture hash differs from the reviewed expected value')
        database = folder / 'world.db'
        demo = invoke('--content-demo', fixture, database, content_hash)
        if demo['lots'] != [7, 3] or demo['revision'] != 2 or not demo['passed']:
            raise ValueError('Compiled template did not produce the expected committed transfer')
        before = database.read_bytes()
        if invoke('--content-open', fixture, database, content_hash) != demo:
            raise ValueError('Reopened content campaign differs from committed demo')
        invoke('--content-open', fixture, database, '0' * 64, success=False, diagnostic='CONTENT_INCOMPATIBLE')
        invoke('--content-open', fixture, database, success=False, diagnostic='CONTENT_ARGUMENTS')
        changed = copy.deepcopy(original)
        changed['definitions'][1]['quantity'] = 4
        changed_path = write('changed-content', changed)
        changed_hash = invoke('--content-compile', changed_path)['contentHash']
        if changed_hash == content_hash:
            raise ValueError('Semantic template change did not change content hash')
        invoke('--content-open', changed_path, database, changed_hash, success=False, diagnostic='INCOMPATIBLE_SAVE')
        absent = folder / 'must-not-create.db'
        invoke('--content-demo', fixture, absent, '0' * 64, success=False, diagnostic='CONTENT_INCOMPATIBLE')
        if absent.exists() or database.read_bytes() != before:
            raise ValueError('Incompatible content mutated or created a save')

        cases = []
        def invalid(name, change, diagnostic):
            value = copy.deepcopy(original)
            change(value)
            cases.append((name, write(name, value), diagnostic))

        invalid('unknown-handler', lambda x: x['definitions'][1].update(handler='RepairBridge@1'), 'CONTENT_HANDLER_UNAPPROVED: transfer.timber@1')
        invalid('embedded-script', lambda x: x['definitions'][1].update(script='world.lots[0]=0'), 'transfer.timber@1).script')
        invalid('unknown-root-field', lambda x: x.update(script='execute()'), 'CONTENT_UNKNOWN_FIELD')
        invalid('duplicate-definition', lambda x: x['definitions'].append(copy.deepcopy(x['definitions'][0])), 'CONTENT_DEFINITION_DUPLICATE')
        invalid('missing-field', lambda x: x.pop('version'), 'CONTENT_REQUIRED_FIELD')
        invalid('wrong-schema', lambda x: x.update(schema_version=2), 'CONTENT_SCHEMA_UNSUPPORTED')
        invalid('missing-reference', lambda x: x['definitions'][1].update(commodity_id='commodity.absent@1'), 'CONTENT_REFERENCE_MISSING: transfer.timber@1')
        invalid('unsupported-kind', lambda x: x['definitions'][1].update(kind='npc'), 'CONTENT_KIND')
        invalid('quantity-float', lambda x: x['definitions'][1].update(quantity=3.5), 'CONTENT_INTEGER')
        invalid('quantity-string', lambda x: x['definitions'][1].update(quantity='3'), 'CONTENT_INTEGER')
        invalid('quantity-overflow', lambda x: x['definitions'][1].update(quantity=2 ** 63), 'CONTENT_INTEGER')
        invalid('quantity-zero', lambda x: x['definitions'][1].update(quantity=0), 'CONTENT_TEMPLATE_INVALID')
        invalid('missing-world-commodity', lambda x: (x['definitions'][0].update(definition_id='commodity.stone@1'), x['definitions'][1].update(commodity_id='commodity.stone@1')), 'CONTENT_REFERENCE_MISSING: commodity.timber@1')
        duplicate = folder / 'duplicate-field.json'
        duplicate.write_text(fixture.read_text().replace('"version": 1,', '"version": 1, "version": 1,', 1))
        cases.append(('duplicate-field', duplicate, 'CONTENT_DUPLICATE_FIELD'))
        oversize = folder / 'oversize.json'
        oversize.write_bytes(b' ' * (1024 * 1024 + 1))
        cases.append(('oversize', oversize, 'CONTENT_SIZE'))
        invalid_reports = []
        for name, path, diagnostic in cases:
            # A valid-but-incomplete pack needs its own exact hash to reach world validation.
            expected_hash = invoke('--content-compile', path)['contentHash'] if name == 'missing-world-commodity' else content_hash
            report = invoke('--content-demo', path, absent, expected_hash, success=False, diagnostic=diagnostic)
            if absent.exists():
                raise ValueError(name + ' created a database before validation')
            # Parser/compiler failures must also leave an existing campaign byte-identical.
            if name != 'missing-world-commodity':
                invoke('--content-open', path, database, expected_hash, success=False, diagnostic=diagnostic)
            if database.read_bytes() != before:
                raise ValueError(name + ' altered the existing database')
            invalid_reports.append(dict(case=name, **report))

        base = copy.deepcopy(original)
        base['pack_id'] = 'kernel.base'
        base['definitions'] = base['definitions'][:1]
        base_path = write('base', base)
        base_hash = invoke('--content-compile', base_path)['contentHash']
        dependent = copy.deepcopy(original)
        dependent['definitions'] = dependent['definitions'][1:]
        dependent['dependencies'] = [dict(pack_id='kernel.base', version=1, content_hash=base_hash)]
        dependent_path = write('dependent', dependent)
        invoke('--content-compile', dependent_path, success=False, diagnostic='CONTENT_DEPENDENCY_MISSING')
        dependency_manifest = invoke('--content-compile', dependent_path, base_path)
        dependency_db = folder / 'dependency.db'
        dependency_demo = invoke('--content-demo', dependent_path, dependency_db, dependency_manifest['contentHash'], base_path)
        if dependency_demo['lots'] != [7, 3]:
            raise ValueError('Dependency-provided commodity not used in transfer')
        dependency_before = dependency_db.read_bytes()
        invoke('--content-open', dependent_path, dependency_db, dependency_manifest['contentHash'], success=False, diagnostic='CONTENT_DEPENDENCY_MISSING')
        if dependency_db.read_bytes() != dependency_before:
            raise ValueError('Missing dependency altered saved state')
        cyclic_base = copy.deepcopy(base)
        cyclic_base['dependencies'] = [dict(pack_id=dependent['pack_id'], version=1, content_hash='0' * 64)]
        invoke('--content-compile', dependent_path, write('cyclic-base', cyclic_base), success=False, diagnostic='CONTENT_DEPENDENCY_UNRESOLVED')

        result = dict(scope='headless runtime content compiler, typed transfer and SQLite compatibility; not Unity or production roster',
                      fixture_sha256=hashlib.sha256(fixture.read_bytes()).hexdigest(), content_hash=content_hash,
                      compiled=compiled, demo=demo, invalid_cases=invalid_reports,
                      dependency_demo=dependency_demo, process_count=len(reports), processes=reports)
    destination.write_text(json.dumps(result, indent=2) + '\n')
    return result
