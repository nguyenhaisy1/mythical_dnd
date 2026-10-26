"""Developer-observation eligibility gate; no autonomous occurrence/gameplay claims."""
import copy
import json
from pathlib import Path
import subprocess
import tempfile
import sys
ROOT = Path(__file__).resolve().parents[2]

def run(dotnet, destination):
    pack = ROOT/'content/definitions/vertical-slice/events.json'
    settlement = ROOT/'content/definitions/vertical-slice/pack.json'
    fixtures = ROOT/'content/fixtures/event-eligibility/scenarios.json'
    source = json.loads(pack.read_text())
    scenarios = json.loads(fixtures.read_text())
    calls = []
    def invoke(path, fixture=None, expected=0):
        cmd = [str(dotnet), str(ROOT/'engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll'), '--event-eligibility', str(settlement), str(path)]
        if fixture is not None: cmd.append(str(fixture))
        p = subprocess.run(cmd, capture_output=True, text=True, timeout=30)
        if p.returncode != expected: raise ValueError('Event eligibility exit mismatch: '+p.stdout+p.stderr)
        result = json.loads(p.stdout or p.stderr)
        if result.get('passed') != (expected == 0): raise ValueError('Event success marker mismatch')
        calls.append({'exit_code':p.returncode,'error':result.get('error')})
        return result
    first = invoke(pack, fixtures)
    if first != invoke(pack, fixtures): raise ValueError('Event evaluation differs across processes')
    if first['templateCount'] != 8 or len(first['outcomes']) != 23: raise ValueError('Missing event fixtures')
    if any(o['eligible'] != o['expectedEligible'] for o in first['outcomes']): raise ValueError('Fixture mismatch')
    with tempfile.TemporaryDirectory(prefix='event-eligibility-gate-') as tmp:
        folder=Path(tmp)
        def emit(name,data):
            p=folder/(name+'.json');p.write_text(json.dumps(data));return p
        reordered=copy.deepcopy(source);reordered['templates'].reverse()
        if invoke(emit('order',reordered),fixtures) != first: raise ValueError('Template order affects evaluation/hash')
        for label, mutate in [
            ('unknown',lambda x:x.update(script='arbitrary')),
            ('version',lambda x:x.update(version=2)),
            ('settlement',lambda x:x.update(settlement_hash='0'*64)),
            ('handler',lambda x:x['templates'][0].update(handler='spawn-resources')),
            ('owner',lambda x:x['templates'][0].update(owner_domain='Assets')),
            ('actor',lambda x:x['templates'][0].update(actor_ids=['actor.absent@1'])),
            ('empty',lambda x:x.update(templates=[])),
            ('perceived-truth',lambda x:x['templates'][0].update(perceived_condition={'op':'Compare','field':'boat_condition','comparison':'lte','value':400})),
            ('newline-id',lambda x:x['templates'][0].update(definition_id=x['templates'][0]['definition_id']+'\n'))]:
            bad=copy.deepcopy(source);mutate(bad);invoke(emit(label,bad),expected=1)
        for label, text in [('duplicate','{"profile":"x","profile":"y"}'),('root','[]'),('truncated','{'),('oversize',' '*1048577),('float',pack.read_text().replace('"version": 1,','"version": 1.0,',1))]:
            p=folder/(label+'.json');p.write_text(text);invoke(p,expected=1)
        mismatch=copy.deepcopy(scenarios);mismatch['scenarios'][0]['expected_eligible']=not mismatch['scenarios'][0]['expected_eligible']
        failed=invoke(pack,emit('mismatch',mismatch),1)
        if not failed.get('outcomes'): raise ValueError('Mismatch did not exercise evaluator')
        extra=copy.deepcopy(scenarios);extra['scenarios'][0]['context']['player_nearby']=True
        invoke(pack,emit('extra-context',extra),1)
        empty=copy.deepcopy(scenarios);empty['scenarios']=[]
        invoke(pack,emit('empty-fixture',empty),1)
    sys.path.insert(0,str(ROOT/'tools/checks'))
    from check_contract_vectors import digest
    canonical=copy.deepcopy(source);canonical['templates'].sort(key=lambda x:x['definition_id'])
    canonical['localization']=[{'key':k,'text':v} for k,v in sorted(source['localization'].items())]
    if digest(canonical)!=first['contentHash']: raise ValueError('Independent event mdc1 hash mismatch')
    result={'scope':'trusted observations; eligibility only','processes':len(calls),'negative_cases':sum(c['exit_code']!=0 for c in calls),'scenario_count':len(first['outcomes']),'content_hash':first['contentHash'],'outcomes':first['outcomes'],'calls':calls}
    destination.write_text(json.dumps(result,indent=2)+'\n')
    return result
