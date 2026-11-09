#!/usr/bin/env python3
"""PID-bound real X11 input and actual two-process Player restart for origin onboarding."""
import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
import time

from run_origin_client import inspect_database
from run_repair_client import sha

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--player', required=True, type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='sprint16-input-', dir=ROOT / 'tests/results'))
    if args.output:
        output.mkdir(parents=True, exist_ok=False)
    report = dict(scope='actual OS keyboard/mouse and actual Player restart; not human playtest',
                  status='Failed', exit_code=1, utc_started=datetime.now(timezone.utc).isoformat(), phases=[])
    process = None
    try:
        binary = args.player.resolve()
        if not binary.is_file() or not shutil.which('xdotool'):
            raise ValueError('Player executable or xdotool unavailable')
        report['player_sha256'] = sha(binary)
        report['managed_assembly_sha256'] = sha(binary.parent / 'MythicalOrigin_Data/Managed/Assembly-CSharp.dll')
        saves = output / 'saves'
        original_record = None
        def phase_run(number):
            nonlocal process, original_record
            folder = output / ('phase-' + str(number)); folder.mkdir()
            telemetry = folder / 'telemetry'
            phase = dict(actions=[], folder=str(folder), requested_resolution=[1280, 720])
            command = [str(binary), '-screen-width', '1280', '-screen-height', '720', '-screen-fullscreen', '0',
                       '-logFile', str(folder / 'player-runtime.log'), '-s16InputProbe', str(telemetry),
                       '-s16SaveRoot', str(saves), '-s16QaSeed', '0' * 64]
            if number == 1:
                command += ['-s16QaHoldFirstPublication', 'yes']
            phase['command'] = command
            with (folder / 'launcher.log').open('w') as log:
                process = subprocess.Popen(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
            phase['pid'] = process.pid
            def state():
                try:
                    return json.loads((telemetry / 'qa.json').read_text())
                except (FileNotFoundError, json.JSONDecodeError):
                    return {}
            def wait(predicate, label, timeout=30):
                end = time.monotonic() + timeout
                while time.monotonic() < end:
                    if predicate():
                        return
                    if process.poll() is not None:
                        raise ValueError('Player exited before ' + label)
                    time.sleep(.1)
                raise ValueError('Timeout: ' + label)
            def xdo(*argv, allow_failure=False):
                r = subprocess.run(['xdotool', *map(str, argv)], text=True, capture_output=True, timeout=12)
                if r.returncode and not allow_failure:
                    raise ValueError('xdotool failed: ' + r.stderr.strip())
                return r.stdout.strip()
            wait(lambda: state().get('inputReady') is True, 'actual InputProbe readiness', 90)
            windows = xdo('search', '--onlyvisible', '--pid', process.pid, allow_failure=True).splitlines()
            if len(windows) != 1:
                raise ValueError('Expected one visible window belonging to the Player PID')
            window = windows[0]; phase['window'] = window
            xdo('windowactivate', '--sync', window, allow_failure=True); xdo('windowfocus', '--sync', window)
            def focused():
                if xdo('getwindowpid', xdo('getwindowfocus')) != str(process.pid):
                    raise ValueError('Player lost focus; input stopped before typing into another app')
            def key(value):
                focused(); old = len(state().get('inputKeys', []))
                xdo('key', '--clearmodifiers', value)
                wait(lambda: len(state().get('inputKeys', [])) > old, 'application key handler ' + value)
                time.sleep(.2)
                phase['actions'].append(dict(key=value, observation=state()))
            def focus(name):
                for _ in range(40):
                    if state().get('focus') == name:
                        return
                    key('Tab')
                raise ValueError('Could not reach native focus control ' + name)
            def click(name):
                focus(name); current = state()
                target = next((t for t in current.get('targets', []) if t['name'] == name), None)
                if target is None:
                    raise ValueError('No enabled public UI bounds for pointer target ' + name)
                bounds = target['boundsRect']; x = bounds['x'] + bounds['width'] / 2; y = bounds['y'] + bounds['height'] / 2
                if not 0 < x < current['width'] or not 0 < y < current['height']:
                    raise ValueError('Focused pointer control is outside actual drawable: ' + name)
                old = len(current.get('inputKeys', [])); focused()
                xdo('mousemove', '--window', window, round(x), round(y)); xdo('click', '1')
                wait(lambda: len(state().get('inputKeys', [])) > old, 'application pointer handler ' + name)
                time.sleep(.2)
                phase['actions'].append(dict(pointer_target=name, position=[x, y], observation=state()))
            def frame():
                return state().get('current')
            def same_record():
                return frame() is not None and {k: v for k, v in frame().items() if k != 'tick'} == original_record
            if number == 1:
                if state().get('screen') != 'Menu' or state().get('hasCampaign'):
                    raise ValueError('First process must start at an empty campaign menu')
                focus('NewCampaign'); key('Return')
                wait(lambda: state().get('publicationHeld') is True, 'real pending DB publication barrier')
                if list(saves.glob('origin-*.db')):
                    raise ValueError('Uncommitted campaign appeared before cancellation')
                key('Escape')
                wait(lambda: state().get('screen') == 'Menu' and state().get('lastResultCode') == 'cancelled', 'cancel actual pending creation')
                if state().get('hasCampaign') or list(saves.glob('origin-*.db')):
                    raise ValueError('Cancellation published a campaign')
                focus('NewCampaign'); key('Return')
                wait(lambda: state().get('screen') == 'Reveal' and frame() is not None, 'native New to Reveal')
                if frame()['actorName'] != 'Vạc Lâm' or frame()['tick'] != 0:
                    raise ValueError('Actual seeded Reveal differs from authored known answer')
                original_record = {k: v for k, v in frame().items() if k != 'tick'}
                focus('BeginWorld'); key('Return')
                wait(lambda: state().get('screen') == 'World', 'native Begin to World')
                if not same_record() or frame()['tick'] != 0:
                    raise ValueError('Begin changed the origin record')
                click('WaitOne')
                if frame()['tick'] != 1:
                    raise ValueError('Native mouse wait did not advance once')
                focus('WaitSixty'); key('space')
                if frame()['tick'] != 61:
                    raise ValueError('Native Space submit did not advance exactly sixty')
                click('Save')
                if state().get('lastResultCode') != 'saved' or not state().get('lastResultDurable'):
                    raise ValueError('Actual pointer Save did not durably save')
                focus('Reload'); key('Return')
                if state().get('lastResultCode') != 'loaded' or frame()['tick'] != 61:
                    raise ValueError('Native Reload changed progress')
                focus('SelfRecord'); key('Return')
                wait(lambda: state().get('recordOpen') is True, 'native self record')
                if any(t['name'] in ['WaitOne', 'WaitSixty', 'Save'] for t in state().get('targets', [])):
                    raise ValueError('Background action remains enabled in modal')
                focus('CloseRecord'); key('space')
                wait(lambda: not state().get('recordOpen'), 'native modal close')
                if frame()['tick'] != 61:
                    raise ValueError('Modal input mutated the world clock')
                focus('BackMenu'); key('Return')
                wait(lambda: state().get('screen') == 'Menu' and state().get('hasCampaign'), 'return to compatible campaign menu')
            else:
                if state().get('screen') != 'Menu' or not state().get('hasCampaign'):
                    raise ValueError('Restart must find the previously published compatible campaign')
            focus('ContinueCampaign'); key('Return')
            wait(lambda: state().get('screen') == 'Reveal' and frame() is not None, 'native Continue after saved campaign')
            if not same_record() or frame()['tick'] != 61:
                raise ValueError('Continue rerolled or reset the saved origin')
            click('BeginWorld')
            wait(lambda: state().get('screen') == 'World', 'pointer Begin from continued Reveal')
            if not same_record() or frame()['tick'] != 61:
                raise ValueError('Continued World differs from saved record')
            focused(); xdo('key', '--clearmodifiers', 'F10')
            code = process.wait(timeout=30)
            final = state()
            if code != 0 or final.get('passed') is not True:
                raise ValueError('InputProbe did not exit successfully')
            phase.update(player_exit_code=code, telemetry=final, status='Passed',
                         actual_resolution=[final['width'], final['height']])
            report['phases'].append(phase)
        phase_run(1)
        first = inspect_database(saves, '0' * 64)
        phase_run(2)
        second = inspect_database(saves, '0' * 64)
        if first != second:
            raise ValueError('Actual Player restart/Continue changed the persisted campaign')
        report.update(status='Passed', exit_code=0, final_database=second,
                      checks=['pending creation cancelled before publish', 'native New/Reveal/Begin', 'mouse and Space wait',
                              'durable Save/Reload', 'modal inertness/native close', 'menu Continue', 'actual second-process restart same origin/cursor/tick'])
    except (OSError, ValueError, KeyError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
        if process is not None and process.poll() is None:
            process.terminate()
            try:
                process.wait(timeout=10)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait(timeout=5)
    report['utc_finished'] = datetime.now(timezone.utc).isoformat()
    (output / 'evidence.json').write_text(json.dumps(report, indent=2) + '\n')
    print('Evidence: ' + str(output / 'evidence.json'), flush=True)
    return report['exit_code']


if __name__ == '__main__':
    raise SystemExit(main())
