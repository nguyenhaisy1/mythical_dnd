#!/usr/bin/env python3
"""Independent authored parameters/known results and safe-source checks; not runtime acceptance."""
import hashlib
import json
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / 'tools/checks'))
from check_contract_vectors import canonical
from check_origin_client import expected_document


def expected_profile():
    parameters = json.loads((ROOT / 'rules/parameters/personal-weaving-v1.json').read_text())
    keys = {'profile', 'recipe_id', 'material_id', 'output_id', 'fiber_cost', 'duration_ticks', 'cord_yield', 'minimum_skill', 'starter_fiber', 'tool_mode', 'source'}
    assert set(parameters) == keys
    assert (parameters['profile'], parameters['recipe_id'], parameters['material_id'], parameters['output_id'], parameters['fiber_cost'], parameters['duration_ticks'], parameters['cord_yield'], parameters['minimum_skill'], parameters['starter_fiber'], parameters['tool_mode']) == (
        'personal-weaving-v1', 'recipe.hand-braid-cord@1', 'commodity.fiber@1', 'commodity.cord@1', 2, 30, 1, 40, 8, 'hands')
    cohort_hash = hashlib.sha256(canonical(expected_document())).hexdigest()
    return parameters, cohort_hash, hashlib.sha256(canonical(parameters)).hexdigest(), hashlib.sha256(canonical(dict(profile='personal-weaving-v1', cohort_hash=cohort_hash, parameters=parameters))).hexdigest()


def main():
    parameters, cohort, recipe, profile = expected_profile()
    resource = ROOT / 'client/UnityProject/Assets/Game/Resources/Sprint17/personal-weaving-v1.json'
    assert resource.read_bytes() == (ROOT / 'rules/parameters/personal-weaving-v1.json').read_bytes(), 'Stale trusted parameter resource'
    expected = json.loads((ROOT / 'content/fixtures/personal-weaving/expected.json').read_text())
    assert (expected['recipe_hash'], expected['cohort_hash'], expected['content_hash']) == (recipe, cohort, profile)
    assert expected['expected'] == dict(initial=[8, 0, 0, 0], running=[8, 2, 0, 0], before_due=[8, 2, 0, 29], completed=[6, 0, 1, 30], cancelled=[8, 0, 0, 0])
    eligible = [(r['actor_id'], s['value']) for r in expected_document()['records'] for s in r['skills'] if s['id'] == 'weaving' and s['value'] >= parameters['minimum_skill']]
    assert eligible == [('actor.vac-lam@1', 43), ('actor.vac-nhu@1', 61)]
    for name in ['WeavingClientView.cs', 'WeavingAutoplay.cs']:
        text = (ROOT / 'client/UnityProject/Assets/Game/Presentation' / name).read_text()
        assert not re.search(r'using Game\.(Infrastructure|Domain|Foundation)|WorldSnapshot|CommandProcessor|WeavingContentRegistry|Resources.Load<TextAsset>', text), 'Raw dependency in presenter: ' + name
    print(json.dumps(dict(scope='authored reference and source only; not Unity or gameplay acceptance', content_hash=profile, recipe_hash=recipe, qualified=eligible, exact_parameters=True), ensure_ascii=False, indent=2))


if __name__ == '__main__':
    main()
