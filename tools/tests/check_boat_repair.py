#!/usr/bin/env python3
"""Exercise real repair/save/reopen/replay in independent C# processes."""
import hashlib
import json
from pathlib import Path
import subprocess

ROOT = Path(__file__).resolve().parents[2]


def run(dotnet, destination):
    destination = Path(destination)
    profile = ROOT / 'rules/parameters/boat-repair-v1.json'
    scenario = ROOT / 'content/fixtures/boat-repair/scenario.json'
    expected = json.loads(scenario.read_text())['expectations']
    host = ROOT / 'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    runs = []
    flags = ('startValid', 'retryValid', 'beforeValid', 'completeValid', 'forbiddenValid',
             'reloadValid', 'recoveryValid', 'profileRefusedWithoutWrite', 'cancelValid',
             'replayValid', 'negativeDetected')
    fields = {
        'timberAfterStart': 'on_hand_after_start', 'reservedAfterStart': 'reserved_after_start',
        'dueTick': 'due_tick', 'conditionBeforeDue': 'condition_before_due',
        'timberAfterCompletion': 'on_hand_after_completion', 'reservedAfterCompletion': 'reserved_after_completion',
        'conditionAfterCompletion': 'condition_after_completion', 'timberAfterCancel': 'on_hand_after_cancel',
        'reservedAfterCancel': 'reserved_after_cancel', 'conditionAfterCancel': 'condition_after_cancel',
    }
    for index in range(2):
        db = destination.parent / f'boat-repair-{index + 1}.db'
        command = [dotnet, str(host), '--repair-probe', str(profile), str(scenario), str(db)]
        result = subprocess.run(command, capture_output=True, text=True, timeout=120)
        (destination.parent / f'boat-repair-{index + 1}.stdout.log').write_text(result.stdout)
        (destination.parent / f'boat-repair-{index + 1}.stderr.log').write_text(result.stderr)
        if result.returncode != 0:
            raise ValueError('Repair probe process failed: ' + result.stderr)
        report = json.loads(result.stdout)
        if report.get('passed') is not True or not all(report.get(key) is True for key in flags):
            raise ValueError('Repair probe did not pass every required check')
        if any(report.get(key) != expected[value] for key, value in fields.items()):
            raise ValueError('Repair effects differ from independently authored scenario expectations')
        if report.get('imageVersion') != 2 or report.get('sqliteSchemaVersion') != 2 or report.get('replayActions', 0) < 6:
            raise ValueError('Repair profile/image/schema/replay scope missing')
        if report.get('ruleVersion') != 'transfer-resource-v1+scheduler-v1+boat-repair-v1':
            raise ValueError('Wrong repair rule compatibility')
        if runs and report != runs[0]['report']:
            raise ValueError('Repair outcomes/hashes differ across independent processes')
        runs.append({'process': index + 1, 'exit_code': result.returncode, 'command': command, 'report': report})

    duplicate = destination.parent / 'boat-repair-duplicate.json'
    duplicate.write_text('{"schema_version":1,' + profile.read_text().lstrip()[1:])
    unknown = destination.parent / 'boat-repair-unknown.json'
    unknown_data = json.loads(profile.read_text())
    unknown_data['unapproved_handler'] = 'must-not-be-accepted'
    unknown.write_text(json.dumps(unknown_data))
    negatives = []
    for label, malformed, diagnostic in (
            ('duplicate', duplicate, 'REPAIR_JSON_DUPLICATE'),
            ('unknown', unknown, 'REPAIR_CONTENT_FIELDS')):
        db = destination.parent / f'boat-repair-invalid-{label}.db'
        command = [dotnet, str(host), '--repair-probe', str(malformed), str(scenario), str(db)]
        result = subprocess.run(command, capture_output=True, text=True, timeout=120)
        if result.returncode != 1 or diagnostic not in result.stderr or db.exists():
            raise ValueError('Repair authoring negative control failed: ' + label)
        negatives.append({'case': label, 'exit_code': result.returncode, 'diagnostic': result.stderr.strip(),
                          'created_database': db.exists(), 'command': command})
    outcome = {
        'scope': 'headless repair-v1 real SQLite/reopen/recovery and command replay; not Unity/full profession/playtest',
        'profile_source_sha256': hashlib.sha256(profile.read_bytes()).hexdigest(),
        'scenario_source_sha256': hashlib.sha256(scenario.read_bytes()).hexdigest(),
        'runs': runs, 'negative_controls': negatives,
    }
    destination.write_text(json.dumps(outcome, indent=2) + '\n')
    return outcome
