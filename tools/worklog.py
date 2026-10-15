#!/usr/bin/env python3
"""Append a timestamped spin event to WORKLOG.md (POSIX file locking)."""
import argparse
from datetime import datetime
import fcntl
import json
import os
from pathlib import Path
import re
ROOT=Path(__file__).resolve().parents[1]
EVENTS=('OPEN','ASSIGN','PROGRESS','RESULT','BLOCKED','PAUSED','RESUME','HANDOFF','GIT','CORRECTION','REPORT','CLOSE')
def append_event(path, *, spin, new_spin, event, actor, summary, details):
    # a+ only: never truncate or rewrite prior records. Lock ID allocation and append together.
    with path.open('a+',encoding='utf-8') as f:
        fcntl.flock(f,fcntl.LOCK_EX)
        f.seek(0);old=f.read()
        if not old.strip():raise ValueError('Initialize WORKLOG.md conventions before appending events')
        records=[json.loads(m) for m in re.findall(r'^<!-- worklog: (.+) -->$',old,re.M)]
        if new_spin:
            if event!='OPEN':raise ValueError('--new-spin requires --event OPEN')
            spin=f'SPIN-{max([int(r["spin"].split("-")[1]) for r in records]+[0])+1:03d}'
        if not spin or not re.fullmatch(r'SPIN-[0-9]{3,}',spin) or int(spin.split('-')[1])==0:
            raise ValueError('Use a positive SPIN-NNN or --new-spin; SPIN-000 is retrospective only')
        previous=[r for r in records if r['spin']==spin]
        if event=='OPEN' and previous:raise ValueError(f'{spin} already exists; append RESUME or another event')
        if event!='OPEN' and not previous:raise ValueError(f'{spin} has not been opened')
        if not actor.strip() or not summary.strip():raise ValueError('Actor and summary must be nonempty')
        if any(c in actor+summary for c in '\r\n'):raise ValueError('Actor and summary must be one line; use details for multiline')
        if re.search(r'^<!-- worklog:',details,re.M):raise ValueError('Details must not contain worklog metadata records')
        seq=max([int(r['id'].split('-')[1]) for r in records]+[0])+1
        record={'id':f'EVT-{seq:05d}','spin':spin,'event':event,'at':datetime.now().astimezone().isoformat(timespec='seconds'),'actor':actor}
        block='\n\n### '+record['id']+' | '+spin+' | '+event+' — '+summary+'\n\n'
        block+='<!-- worklog: '+json.dumps(record,ensure_ascii=False)+' -->\n'
        block+='- Ghi lúc: '+record['at']+'\n- Actor: '+actor+'\n\n'+details.strip()+'\n'
        f.write(block);f.flush();os.fsync(f.fileno())
        return record

def main():
    p=argparse.ArgumentParser(description=__doc__)
    group=p.add_mutually_exclusive_group(required=True)
    group.add_argument('--spin');group.add_argument('--new-spin',action='store_true')
    p.add_argument('--event',choices=EVENTS,required=True)
    p.add_argument('--actor',required=True);p.add_argument('--summary',required=True)
    p.add_argument('--details-file',type=Path,required=True)
    args=p.parse_args()
    try:
        if not (ROOT/'WORKLOG.md').is_file() or not (ROOT/'.agent').is_file():raise ValueError('Project WORKLOG.md/.agent missing')
        record=append_event(ROOT/'WORKLOG.md',spin=args.spin,new_spin=args.new_spin,event=args.event,actor=args.actor,summary=args.summary,details=args.details_file.read_text())
    except (ValueError,OSError,json.JSONDecodeError) as e:p.exit(1,str(e)+'\n')
    print(json.dumps(record,ensure_ascii=False))
if __name__=='__main__':main()
