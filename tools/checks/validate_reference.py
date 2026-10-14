#!/usr/bin/env python3
"""Validate reference fixture against the small schema subset it uses, plus references.
Not a general JSON Schema implementation or a runtime content compiler.
"""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
def validate(value,schema,path='$'):
    supported={'$schema','$id','title','type','const','enum','properties','required','additionalProperties','items','minItems','minLength'}
    assert not set(schema)-supported, f'Unsupported schema keyword at {path}: {set(schema)-supported}'
    if 'const' in schema: assert value==schema['const'], f'{path}: wrong constant'
    if 'enum' in schema: assert value in schema['enum'], f'{path}: outside enum'
    kind=schema.get('type')
    types={'object':dict,'array':list,'string':str,'integer':int}
    if kind: assert type(value) is types[kind], f'{path}: expected {kind}'
    if kind=='object':
        assert set(schema.get('required',[]))<=value.keys(),f'{path}: missing fields'
        if schema.get('additionalProperties') is False:assert value.keys()<=schema.get('properties',{}).keys(),f'{path}: unknown fields'
        for k,v in value.items():validate(v,schema['properties'][k],path+'.'+k)
    if kind=='array':
        assert len(value)>=schema.get('minItems',0),f'{path}: too few items'
        for i,v in enumerate(value):validate(v,schema['items'],f'{path}[{i}]')
    if kind=='string':assert len(value)>=schema.get('minLength',0),f'{path}: too short'
def check(data):
    validate(data,json.loads((ROOT/'content/schemas/design-fixture.schema.json').read_text()))
    ids=[]
    for k in ['regions','settlements','households','actors','routes','threads']:ids.extend(x['id'] for x in data[k])
    assert len(ids)==len(set(ids)),'Duplicate IDs'
    regions={x['id'] for x in data['regions']};settlements={x['id']:x for x in data['settlements']};households={x['id'] for x in data['households']}
    for s in settlements.values():assert s['region_id'] in regions,'Missing parent region'
    for h in data['households']:assert h['settlement_id'] in settlements,'Missing household settlement'
    for a in data['actors']:assert a['household_id'] in households,'Missing actor household'
    pairs=set()
    for r in data['routes']:
        assert r['from'] in settlements and r['to'] in settlements,'Dangling route endpoint'
        assert r['from']!=r['to'],'Self edge not intended in this fixture'
        pair=(r['from'],r['to']);assert pair not in pairs,'Duplicate directed edge';pairs.add(pair)
    assert settlements['settlement.pc-02']['region_id']=='region.phong-chau','Bến Hạc canon conflict'
    assert settlements['settlement.pc-02']['role']=='external-boundary','Slice scope conflict'
if __name__=='__main__':
    import copy
    data=json.loads((ROOT/'content/fixtures/vertical-slice.reference.json').read_text());check(data)
    # Meaningful negative controls prevent false confidence in the validator.
    for mutate in [lambda d:d['actors'].append(copy.deepcopy(d['actors'][0])),lambda d:d['routes'][0].update(to='missing'),lambda d:d['settlements'][3].update(region_id='region.tam-giang'),lambda d:d.update(status='runtime-ready')]:
        broken=copy.deepcopy(data);mutate(broken)
        try:check(broken)
        except AssertionError:pass
        else:raise AssertionError('Validator accepted invalid fixture')
    print('Reference fixture: schema subset + semantic references pass; 4 invalid controls rejected.')
