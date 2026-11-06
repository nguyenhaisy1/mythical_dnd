#!/usr/bin/env python3
"""Drive a PID-bound Linux Player through real X11 input; never a human playtest."""
import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
import time

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--player', required=True, type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='sprint15-input-', dir=ROOT / 'tests/results'))
    if args.output:
        output.mkdir(parents=True, exist_ok=False)
    report = dict(scope='actual OS keyboard/mouse automation on the selected Player PID; not human playtest',
                  player_sha256=__import__('hashlib').sha256(args.player.read_bytes()).hexdigest(),
                  managed_assembly_sha256=__import__('hashlib').sha256(args.player.parent.joinpath('MythicalRepair_Data/Managed/Assembly-CSharp.dll').read_bytes()).hexdigest(),
                  utc_started=datetime.now(timezone.utc).isoformat(), status='Failed', exit_code=1, actions=[])
    process = None
    try:
        if not args.player.is_file() or not shutil.which('xdotool'):
            raise ValueError('Player executable or xdotool unavailable')
        command = [str(args.player.resolve()), '-screen-width', '1280', '-screen-height', '720', '-screen-fullscreen', '0',
                   '-logFile', str(output / 'player-runtime.log'), '-s15InputProbe', str(output / 'telemetry'),
                   '-s15SaveRoot', str(output / 'saves')]
        report['command'] = command
        with (output / 'launcher.log').open('w') as log:
            process = subprocess.Popen(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
        report['pid'] = process.pid

        def xdo(*argv, allow_failure=False):
            result = subprocess.run(['xdotool', *map(str, argv)], capture_output=True, text=True, timeout=12)
            if result.returncode and not allow_failure:
                raise ValueError('xdotool failed: ' + result.stderr.strip())
            return result.stdout.strip()

        def wait_until(predicate, description, timeout=20):
            deadline = time.monotonic() + timeout
            while time.monotonic() < deadline:
                if predicate():
                    return
                if process.poll() is not None:
                    raise ValueError('Player exited before ' + description)
                time.sleep(.15)
            raise ValueError('Timed out waiting for ' + description)

        def windows():
            return xdo('search', '--onlyvisible', '--pid', process.pid, allow_failure=True).splitlines()

        wait_until(lambda: bool(windows()), 'PID-bound visible window', 40)
        ids = windows()
        if len(ids) != 1:
            raise ValueError('Ambiguous visible windows for the Player PID')
        window = ids[0]; report['window'] = window
        xdo('windowactivate', '--sync', window, allow_failure=True)
        xdo('windowfocus', '--sync', window)

        def state():
            path = output / 'telemetry/qa.json'
            try:
                return json.loads(path.read_text()) if path.is_file() else {}
            except json.JSONDecodeError:
                return {}

        def focused():
            # Abort if the user changes focus; never type into an unrelated application.
            if xdo('getwindowpid', xdo('getwindowfocus')) != str(process.pid):
                raise ValueError('Player lost focus; OS input stopped')

        def key(value):
            focused(); previous = len(state().get('inputKeys', []))
            xdo('key', '--clearmodifiers', value)
            wait_until(lambda: len(state().get('inputKeys', [])) > previous, 'application key handler ' + value)
            time.sleep(.2)
            report['actions'].append(dict(key=value, observation=state()))

        ready = output / 'player-runtime.log'
        wait_until(lambda: state().get('inputReady') is True, 'client ready', 90)
        key('r')
        if not state().get('modalBlocksScene') or state().get('completed', {}).get('reserved') != 0:
            raise ValueError('R did not open a pure preview')
        key('Tab'); key('Return')
        if state().get('modalBlocksScene') or state().get('completed', {}).get('reserved') != 0:
            raise ValueError('Tab/Return did not activate modal Back without committing')
        key('r'); key('Return')
        wait_until(lambda: state().get('completed', {}).get('reserved') == 2, 'keyboard confirmation')
        if not state().get('lastResultSuccess') or not state().get('lastResultDurable'):
            raise ValueError('Keyboard confirmation was not durably accepted')
        key('s')
        if state().get('lastResultCode') != 'saved' or not state().get('lastResultDurable'):
            raise ValueError('Keyboard snapshot was not saved')
        key('l')
        if state().get('lastResultCode') != 'loaded' or not state().get('lastResultSuccess'):
            raise ValueError('Keyboard reload failed')
        key('space')
        if state().get('completed', {}).get('tick') != 1:
            raise ValueError('Keyboard wait did not advance one tick')
        if state().get('modalBlocksScene'):
            raise ValueError('Space activated the focused repair button')
        # Restore focus is the repair button; two native Tabs reach Cancel then Wait 1.
        key('Tab'); key('Tab'); key('space')
        if state().get('completed', {}).get('tick') != 2 or state().get('modalBlocksScene'):
            raise ValueError('Focused wait button Space must advance exactly one additional tick')
        key('c')
        expected = dict(tick=2, timber=10, reserved=0, condition=500, activity='cancelled')
        if state().get('completed') != expected:
            raise ValueError('Keyboard cancellation effects differ from the fixture')
        key('r'); key('Escape')
        if state().get('modalBlocksScene'):
            raise ValueError('Escape did not close modal')
        focused(); previous = len(state().get('inputKeys', []))
        # Coordinates are inside the visible bound boat in the 1280x720 scene, outside the sidebar.
        xdo('mousemove', '--window', window, '773', '440'); xdo('mousedown', '1')
        time.sleep(.3)
        if state().get('modalBlocksScene') or state().get('completed', {}).get('reserved') != 0:
            raise ValueError('Mouse-down alone must not open or confirm the scene preview')
        focused(); xdo('mouseup', '1')
        wait_until(lambda: len(state().get('inputKeys', [])) > previous, 'actual pointer handler')
        if not state().get('modalBlocksScene') or state().get('scenePicks', 0) != 1 or state().get('completed', {}).get('reserved') != 0:
            raise ValueError('Actual mouse click did not pick the observed boat')
        report['actions'].append(dict(pointer=[773, 440], observation=state()))
        # A separate, intentional click on Confirm must submit exactly one new repair.
        focused(); xdo('mousemove', '--window', window, '640', '440'); xdo('click', '1')
        wait_until(lambda: state().get('completed', {}).get('reserved') == 2, 'separate pointer confirmation')
        time.sleep(.3)
        if state().get('modalBlocksScene') or state().get('scenePicks') != 1 or state().get('completed') != dict(tick=2, timber=10, reserved=2, condition=500, activity='running'):
            raise ValueError('Pointer confirmation did not produce exactly one intentional repair')
        report['actions'].append(dict(pointer_confirm=[640, 440], observation=state()))
        key('r'); key('Escape'); focused(); xdo('key', '--clearmodifiers', 'F10')
        exit_code = process.wait(timeout=20)
        if exit_code != 0 or state().get('passed') is not True:
            raise ValueError('Input probe did not finish successfully')
        report.update(status='Passed', exit_code=0, player_exit_code=exit_code,
                      telemetry=state(), checks=['pure preview', 'modal native Tab/Back', 'native confirmation',
                                               'durable snapshot/reload', 'Space on repair and wait focus advances once', 'wait/cancel', 'Escape', 'held/released mouse preview stays pure', 'separate pointer confirmation'])
    except (OSError, ValueError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
        if process is not None and process.poll() is None:
            process.terminate()
            try:
                process.wait(timeout=10)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait(timeout=5)
    report['utc_finished'] = datetime.now(timezone.utc).isoformat()
    (output / 'evidence.json').write_text(json.dumps(report, indent=2) + '\n')
    print('Evidence: ' + str(output / 'evidence.json'))
    return report['exit_code']


if __name__ == '__main__':
    raise SystemExit(main())
