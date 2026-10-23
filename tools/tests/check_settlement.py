"""Real-process authoring compiler gate; not simulation or player-knowledge acceptance."""
import copy
import hashlib
import json
from pathlib import Path
import subprocess
import tempfile
ROOT=Path(__file__).resolve().parents[2]
EXPECTED_HASH='60ce05463edcc1c2e4abbeae8f30159d430f57613b902ec6d48bb775db193ec8'

def run(dotnet,destination):
    pack=ROOT/'content/definitions/vertical-slice/pack.json'
    source=json.loads(pack.read_text())
    calls=[]
    def invoke(path,ok):
        p=subprocess.run([str(dotnet),str(ROOT/'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'),'--settlement-compile',str(path)],capture_output=True,text=True,timeout=30)
        if p.returncode!=(0 if ok else 1):raise ValueError('Settlement exit mismatch: '+p.stderr)
        r=json.loads(p.stdout if ok else p.stderr)
        if r.get('passed')!=ok:raise ValueError('Settlement success marker mismatch')
        calls.append(dict(exit_code=p.returncode,error=r.get('error')))
        return r
    first=invoke(pack,True)
    if first['contentHash']!=EXPECTED_HASH or first!=invoke(pack,True):raise ValueError('Settlement differs from reviewed fixture or across processes')
    if first['counts']!={'actor':24,'household':6,'community':4,'route':8,'fact':24,'relationship':24}:raise ValueError('Roster counts differ')
    with tempfile.TemporaryDirectory(prefix='settlement-gate-') as tmp:
        folder=Path(tmp)
        def emit(name,value):
            p=folder/(name+'.json');p.write_text(json.dumps(value));return p
        reordered=copy.deepcopy(source);reordered['definitions'].reverse()
        if invoke(emit('order',reordered),True)!=first:raise ValueError('Definition order affects registry')
        renamed=copy.deepcopy(source);renamed['localization']['slice.actor.vac-moc']='Tên sửa'
        changed=invoke(emit('rename',renamed),True)
        if changed['definitionIds']!=first['definitionIds'] or changed['contentHash']==first['contentHash']:raise ValueError('Display rename changed identity or failed to bind hash')
        for label,mutate in [
            ('unknown',lambda x:x.update(script='arbitrary')),
            ('duplicate-id',lambda x:x['definitions'].append(copy.deepcopy(x['definitions'][0]))),
            ('missing',lambda x:x.pop('profile')),
            ('canon',lambda x:x['definitions'][0].update(region='phong-chau')),
            ('wrong-owner',lambda x:x['definitions'][0].update(owner_domain='Assets')),
            ('bad-version',lambda x:x.update(version=2)),
            ('float',lambda x:x.update(version=1.5)),
            ('null-def',lambda x:x['definitions'].append(None)),
            ('empty',lambda x:x.update(definitions=[])),
            ('long-string',lambda x:x.update(pack_id='x'*2049))]:
            invalid=copy.deepcopy(source);mutate(invalid);invoke(emit(label,invalid),False)
        for label,text in [('duplicate-key','{"profile":"x","profile":"y"}'),('root-array','[]'),('truncated','{'),('deep','['*25+'0'+']'*25),('oversize',' '*1048577),('fractional-integral',pack.read_text().replace('"version": 1,','"version": 1.0,',1))]:
            p=folder/(label+'.json');p.write_text(text);invoke(p,False)
        invoke(folder/'absent-file.json',False)
    # Independent Python mdc1 reference: localization serialized as key/text tuples, definitions sorted.
    import sys
    sys.path.insert(0,str(ROOT/'tools/checks'))
    from check_contract_vectors import digest
    canonical=copy.deepcopy(source);canonical['definitions'].sort(key=lambda x:x['definition_id'])
    canonical['localization']=[dict(key=k,text=v) for k,v in sorted(source['localization'].items())]
    if digest(canonical)!=first['contentHash']:raise ValueError('Independent canonical hash mismatch')
    result=dict(scope='settlement authoring registry only',processes=len(calls),negative_cases=sum(x['exit_code']!=0 for x in calls),counts=first['counts'],content_hash=first['contentHash'],fixture_sha256=hashlib.sha256(pack.read_bytes()).hexdigest(),calls=calls)
    destination.write_text(json.dumps(result,indent=2)+'\n');return result
