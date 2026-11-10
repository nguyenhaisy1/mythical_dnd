#!/usr/bin/env python3
"""PID-bound real X11 input and actual two-process Player restart for personal weaving."""
import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
import time

from run_weaving_client import inspect_database
from run_repair_client import sha

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--player', required=True, type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    output = args.output.resolve() if args.output else Path(tempfile.mkdtemp(prefix='sprint17-input-', dir=ROOT / 'tests/results'))
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
        report['managed_assembly_sha256'] = sha(binary.parent / 'MythicalWeaving_Data/Managed/Assembly-CSharp.dll')
        saves = output / 'saves'
        original_record = None
        def phase_run(number):
            nonlocal process, original_record
            folder = output / ('phase-' + str(number)); folder.mkdir()
            telemetry = folder / 'telemetry'
            phase = dict(actions=[], folder=str(folder), requested_resolution=[1280, 720])
            command = [str(binary), '-screen-width', '1280', '-screen-height', '720', '-screen-fullscreen', '0',
                       '-logFile', str(folder / 'player-runtime.log'), '-s17InputProbe', str(telemetry),
                       '-s17SaveRoot', str(saves), '-s17QaSeed', '0' * 64]
            if number == 1:
                command += ['-s17QaHoldFirstPublication', 'yes']
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
            def work():
                return state().get('workCurrent')
            def same_record():
                return frame() is not None and {k:v for k,v in frame().items() if k != 'tick'} == original_record
            def assert_work(hand, held, cord, status, tick):
                value = work()
                if value is None or tuple(value[k] for k in ['fiberOnHand','fiberReserved','cordOnHand','status','tick']) != (hand,held,cord,status,tick):
                    raise ValueError('Actual input work state differs: ' + repr(value))
            def open_preview():
                click('PreviewWeaving'); wait(lambda: state().get('previewOpen') is True,'native preview')
                if any(t['name'] in ['WaitOne','WaitSixty','Save','CancelWeaving'] for t in state().get('targets',[])):
                    raise ValueError('Background action remains enabled behind preview')
            if number == 1:
                if state().get('screen') != 'Menu' or state().get('hasCampaign'):
                    raise ValueError('First process must start at empty menu')
                focus('NewCampaign'); key('Return')
                wait(lambda: state().get('publicationHeld') is True,'pending publication barrier')
                if list(saves.glob('origin-*.db')): raise ValueError('Pending campaign published early')
                key('Escape'); wait(lambda: state().get('screen') == 'Menu' and state().get('lastResultCode') == 'cancelled','native cancel')
                if state().get('hasCampaign') or list(saves.glob('origin-*.db')): raise ValueError('Cancel published a campaign')
                focus('NewCampaign'); key('Return')
                wait(lambda: state().get('screen') == 'Reveal' and frame() is not None,'New Reveal')
                if frame()['actorName'] != 'Vạc Lâm' or frame()['tick'] != 0: raise ValueError('Seeded origin differs')
                original_record = {k:v for k,v in frame().items() if k != 'tick'}
                focus('BeginWorld'); key('Return'); wait(lambda: state().get('screen') == 'World','Begin')
                assert_work(8,0,0,'idle',0)
                open_preview()
                assert_work(8,0,0,'idle',0) # the opening pointer gesture must never Confirm
                key('Escape'); wait(lambda: not state().get('previewOpen'),'Back preview')
                assert_work(8,0,0,'idle',0)
                open_preview(); click('ConfirmWeaving')
                wait(lambda: work() is not None and work()['status'] == 'running','separate native Confirm')
                assert_work(8,2,0,'running',0)
                focus('CancelWeaving'); key('Return'); assert_work(8,0,0,'cancelled',0)
                open_preview(); focus('ConfirmWeaving'); key('Return')
                wait(lambda: work()['status'] == 'running','second Start')
                assert_work(8,2,0,'running',0)
                click('WaitOne'); assert_work(8,2,0,'running',1)
                click('Save')
                if state().get('lastResultCode') != 'saved' or not state().get('lastResultDurable'): raise ValueError('Save Running not durable')
                focus('SelfRecord'); key('Return'); wait(lambda: state().get('recordOpen') is True,'self record')
                if any(t['name'] in ['WaitOne','Save','CancelWeaving','PreviewWeaving'] for t in state().get('targets',[])):
                    raise ValueError('Background controls enabled in self record')
                focus('CloseRecord'); key('space'); wait(lambda: not state().get('recordOpen'),'close record')
                assert_work(8,2,0,'running',1)
            else:
                if state().get('screen') != 'Menu' or not state().get('hasCampaign'): raise ValueError('Restart did not find compatible campaign')
                focus('ContinueCampaign'); key('Return')
                wait(lambda: state().get('screen') == 'Reveal' and frame() is not None,'Continue Running after actual restart')
                if not same_record() or frame()['tick'] != 1: raise ValueError('Restart rerolled or reset origin')
                assert_work(8,2,0,'running',1)
                click('BeginWorld'); wait(lambda: state().get('screen') == 'World','continued Begin')
                focus('WaitSixty'); key('space'); assert_work(6,0,1,'completed',61)
                if not same_record(): raise ValueError('Completion changed origin record')
                click('Save')
                if state().get('lastResultCode') != 'saved' or not state().get('lastResultDurable'): raise ValueError('Final Save not durable')
                focus('Reload'); key('Return'); assert_work(6,0,1,'completed',61)
                focus('BackMenu'); key('Return'); wait(lambda: state().get('screen') == 'Menu','Menu after completion')
                focus('ContinueCampaign'); key('Return'); wait(lambda: state().get('screen') == 'Reveal','Continue completed')
                if not same_record() or frame()['tick'] != 61: raise ValueError('Continue changed completed origin')
                click('BeginWorld'); wait(lambda: state().get('screen') == 'World','final World')
                assert_work(6,0,1,'completed',61)
            focused(); xdo('key','--clearmodifiers','F10'); code = process.wait(timeout=30); final = state()
            if code != 0 or final.get('passed') is not True: raise ValueError('InputProbe did not exit successfully')
            phase.update(player_exit_code=code,telemetry=final,status='Passed',actual_resolution=[final['width'],final['height']])
            report['phases'].append(phase)
        phase_run(1)
        running = inspect_database(saves,'0'*64,1,completed=0,cancelled=1,running=1)
        phase_run(2)
        complete = inspect_database(saves,'0'*64,61,completed=1,cancelled=1,running=0)
        if running['world'] != complete['world'] or running['actor'] != complete['actor'] or running['draw'] != complete['draw']:
            raise ValueError('Restart changed immutable assignment/RNG')
        report.update(status='Passed',exit_code=0,running_database=running,final_database=complete,
                      checks=['pending cancellation','native New/Reveal/Begin','pointer preview pure until separate Confirm',
                              'native cancel releases hold','Save Running','actual second-process Continue Running',
                              'Space crosses scheduled due once','final Save/Reload/Continue stable','modal inertness'])
    except (OSError,ValueError,KeyError,subprocess.SubprocessError) as error:
        report['error']=str(error)
        if process is not None and process.poll() is None:
            process.terminate()
            try: process.wait(timeout=10)
            except subprocess.TimeoutExpired: process.kill(); process.wait(timeout=5)
    report['utc_finished']=datetime.now(timezone.utc).isoformat()
    (output/'evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
    print('Evidence: '+str(output/'evidence.json'),flush=True); return report['exit_code']


if __name__ == '__main__': raise SystemExit(main())
