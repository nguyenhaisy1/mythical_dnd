#!/usr/bin/env python3
"""Check authored origin resource/projection boundaries; not actual Unity or gameplay acceptance."""
import hashlib
import json
from pathlib import Path
import re
from check_contract_vectors import canonical, words, bounded

ROOT = Path(__file__).resolve().parents[2]


def expected_document():
    source = json.loads((ROOT / 'content/definitions/vertical-slice/pack.json').read_text())
    canonical_pack = dict(source)
    canonical_pack['definitions'] = sorted(source['definitions'], key=lambda d: d['definition_id'])
    canonical_pack['localization'] = [dict(key=k, text=v) for k, v in sorted(source['localization'].items())]
    source_hash = hashlib.sha256(canonical(canonical_pack)).hexdigest()
    by_id = {d['definition_id']: d for d in source['definitions']}
    label = lambda d: source['localization'][d['display_key']]
    records = []
    for actor in source['definitions']:
        if actor['kind'] != 'actor' or not actor['alive'] or actor['age'] < 18:
            continue
        home = by_id[actor['household_id']]
        community = by_id[home['community_id']]
        if community['definition_id'] != 'community.ben-vac@1':
            continue
        claims = []
        for reference in actor['known_facts']:
            fact = by_id[reference['fact_id']]
            claims.append(dict(id=fact['definition_id'], text=fact['claim'], source=fact['provenance'], history_id=reference['history_id']))
        records.append(dict(actor_id=actor['definition_id'], household_id=home['definition_id'], community_id=community['definition_id'],
                            display_name=label(actor), household_label=label(home), home_label=label(community), source=actor['source'],
                            alive=actor['alive'], age=actor['age'],
                            history=[dict(id=h['history_id'], description=h['description'], at_age=h['at_age']) for h in actor['history']],
                            skills=[dict(id=s['skill_id'], history_id=s['history_id'], value=s['value']) for s in actor['skills']],
                            claims=claims, needs=actor['needs']))
    return dict(profile='origin-cohort-v1', source_hash=source_hash, records=sorted(records, key=lambda r: r['actor_id']))


def main():
    expected = expected_document()
    resource = ROOT / 'client/UnityProject/Assets/Game/Resources/Sprint16/origin-cohort-v1.json'
    if json.loads(resource.read_text()) != expected:
        raise ValueError('Derived origin resource differs from the authored settlement source')
    vectors = json.loads((ROOT / 'content/fixtures/origin-cohort/selector-vectors.json').read_text())
    ids = [r['actor_id'] for r in expected['records']]
    if vectors['candidate_ids'] != ids or len(ids) != 8:
        raise ValueError('Unexpected bounded cohort')
    for row in vectors['vectors']:
        digest = hashlib.sha256(canonical(dict(profile='origin-cohort-v1', purpose='world-id', seed=row['seed']))).digest()
        world = int.from_bytes(digest[:8], 'big') or 1
        context = dict(algorithm='sha256-counter-v1', seed=row['seed'], domain='characters', decision='origin-cohort-v1',
                       occurrence='genesis', entity=f'w{world:016x}/actor/0000000000000001', stream='assignment')
        sequence = words(context, 0)
        index, draw = bounded(sequence, len(ids))
        if (f'{world:016x}', sequence[0], index, draw, ids[index]) != (row['world'], row['word'], row['index'], row['draw_index'], row['selected']):
            raise ValueError('Origin independent known-answer mismatch')
    for name in ['OriginClientView.cs', 'OriginAutoplay.cs']:
        path = ROOT / 'client/UnityProject/Assets/Game/Presentation' / name
        text = path.read_text()
        if re.search(r'using Game\.(Infrastructure|Domain|Foundation)|WorldSnapshot|CommandProcessor|OriginCohortRegistry|Resources.Load<TextAsset>', text):
            raise ValueError('Presenter crosses safe origin port: ' + name)
    print(json.dumps(dict(scope='authored resource and source boundaries only; not Unity/gameplay acceptance',
                          candidates=len(ids), independent_vectors=len(vectors['vectors']),
                          exact_source_hash=expected['source_hash'], derived_resource_matches=True), indent=2))
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
