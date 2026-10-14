#!/usr/bin/env python3
"""Verify documentation reference vectors; does not execute the C# game engine."""
import hashlib,json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
def encode(value):
    if value is None:return b'n'
    if value is True:return b't'
    if value is False:return b'f'
    if isinstance(value,int):
        if not -(2**63)<=value<2**64:raise ValueError('integer range')
        return b'i'+str(value).encode('ascii')+b';'
    if isinstance(value,str):
        raw=value.encode('utf-8');return b's'+str(len(raw)).encode()+b':'+raw
    if isinstance(value,list):return b'a'+str(len(value)).encode()+b':'+b''.join(encode(x) for x in value)
    if isinstance(value,dict):
        import re
        if not all(isinstance(k,str) and re.fullmatch('[a-z][a-z0-9_]*',k) for k in value):raise ValueError('key')
        return b'o'+str(len(value)).encode()+b':'+b''.join(encode(k)+encode(value[k]) for k in sorted(value))
    raise ValueError('unsupported value')
def canonical(value):return b'MDC1'+encode(value)
def digest(value):return hashlib.sha256(canonical(value)).hexdigest()
def round_div(n,d):
    if d<=0:raise ValueError('denominator')
    q,r=divmod(abs(n),d);v=(q+(2*r>=d))*(-1 if n<0 else 1)
    if not -(2**63)<=v<2**63:raise ValueError('overflow')
    return v
def words(context,block):
    raw=hashlib.sha256(canonical(dict(context,block=block))).digest()
    return [int.from_bytes(raw[i:i+4],'little') for i in range(0,32,4)]
def bounded(draws,n):
    if not 1<=n<=2**32:raise ValueError('bound')
    limit=2**32-(2**32%n)
    for count,x in enumerate(draws,1):
        if x<limit:return x%n,count
    raise ValueError('draws exhausted')
def main():
    assert hashlib.sha256(b'abc').hexdigest()=='ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad'
    assert canonical({'b':'x','a':1})==b'MDC1o2:s1:ai1;s1:bs1:x'
    assert canonical([True,None,-2])==b'MDC1a3:tni-2;'
    assert [round_div(n,1000) for n in [1500,-1500,1499,-1499,0]]==[2,-2,1,-1,0]
    assert bounded([4294967295,17],10)==(7,2)
    assert bounded([4294967295],2**32)==(4294967295,1)
    for bad in [1.5,{'BadKey':1},2**64]:
        try:canonical(bad)
        except (ValueError,UnicodeError):pass
        else:raise AssertionError('invalid codec input accepted')
    base=ROOT/'content/fixtures/kernel-smoke'
    vectors=json.loads((base/'vectors.json').read_text())
    for v in vectors['codec']:
        assert canonical(v['value']).hex()==v['hex'];assert digest(v['value'])==v['sha256']
    for v in vectors['rng']:assert words(v['context'],v['block'])==v['words']
    fixture=json.loads((base/'scenario.json').read_text())
    p=json.loads((ROOT/'rules/parameters/kernel-v1.json').read_text())
    expected=fixture['expected_final_projection'];initial=fixture['initial_projection']
    assert initial['lots']['household']['on_hand']==10 and initial['lots']['worker']['on_hand']==0
    transfer=fixture['steps'][0]['payload']['quantity'];cost=p['repair_timber_units']
    assert expected['lots']['household']['on_hand']==10-transfer
    assert expected['lots']['worker']['on_hand']==transfer-cost
    assert expected['item']['condition']==min(p['condition_max'],initial['item']['condition']+p['repair_condition_gain'])
    assert expected['tick']==p['repair_duration_ticks'] and expected['jobs']==[]
    assert expected['item']['owner']=='actor_2' and expected['lots']['worker']['reserved']==0
    assert digest(initial)==fixture['initial_projection_hash']
    assert digest(expected)==fixture['expected_final_projection_hash']
    print('PASS documentation-reference: codec/math/RNG vectors, rejection branch, 3 negative codec controls, smoke arithmetic and projection hashes; runtime acceptance NOT executed.')
if __name__=='__main__':main()
