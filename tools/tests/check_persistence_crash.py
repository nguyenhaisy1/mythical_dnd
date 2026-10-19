#!/usr/bin/env python3
"""Kill real headless processes at transaction boundaries; reopen through the C# provider."""
import argparse,json,os,select,subprocess,tempfile,time
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
def run(dotnet,destination):
    host=ROOT/'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'
    results=[]
    for point in ['after_entities','before_commit','after_commit']:
        with tempfile.TemporaryDirectory(prefix='mythical-crash-') as folder:
            db=str(Path(folder)/'world.db');base=[dotnet,str(host),'--persistence-probe',db]
            subprocess.run(base+['init'],check=True,capture_output=True,text=True)
            child=subprocess.Popen(base+['crash',point],stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True)
            try:
                ready,_,_=select.select([child.stdout],[],[],30)
                if not ready or child.stdout.readline().strip()!='READY:'+point:raise RuntimeError('Crash barrier not reached: '+point)
                child.kill();child.wait(timeout=10)
                if child.returncode!=-9:raise RuntimeError('Expected SIGKILL')
            finally:
                if child.poll() is None:child.kill();child.wait()
            recovered=subprocess.run(base+['recover'],check=True,capture_output=True,text=True)
            item=json.loads(recovered.stdout)
            expected=2 if point=='after_commit' else 1
            if item['beforeRevision']!=expected or item['beforeSource']!=(7 if expected==2 else 10) or not item['passed']:raise RuntimeError('Invalid crash outcome: '+point)
            results.append(dict(point=point,signal=child.returncode,**item))
    destination.write_text(json.dumps({'scope':'Linux process-SIGKILL SQLite transaction recovery; not power-loss or FND12 snapshot fallback','cases':results},indent=2)+'\n')
    return results
if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('--dotnet',default='dotnet');p.add_argument('--output',type=Path,required=True);a=p.parse_args();print(json.dumps(run(a.dotnet,a.output)))
