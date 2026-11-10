#!/usr/bin/env python3
"""Check authored client sources/assets; this does not execute Unity or accept gameplay."""
import hashlib
import json
from pathlib import Path
import re
import struct

ROOT = Path(__file__).resolve().parents[2]


def main():
    project = ROOT / 'client/UnityProject'
    version = (project / 'ProjectSettings/ProjectVersion.txt').read_text()
    assert 'm_EditorVersion: 6000.6.7f1\n' in version, 'Unexpected project version'
    json.loads((project / 'Packages/manifest.json').read_text())
    json.loads((project / 'Packages/packages-lock.json').read_text())
    assert hashlib.sha256((ROOT / 'rules/parameters/boat-repair-v1.json').read_bytes()).digest() == hashlib.sha256(
        (project / 'Assets/Game/Resources/Sprint15/boat-repair-v1.json').read_bytes()).digest(), 'Stale trusted profile resource'
    manifest = json.loads((ROOT / 'assets/production/sprint15/manifest.json').read_text())
    assert manifest['art_profile'] == 'earth-monument-2d-v1' and manifest['client_profile'] == 'repair-client-2d-v1'
    expected = {'base', 'foreground', 'boat-damaged', 'boat-repaired', 'actor-idle-0', 'actor-idle-1'}
    assert {a['id'] for a in manifest['runtime_assets']} == expected
    for asset in manifest['runtime_assets']:
        path = ROOT / asset['file']; data = path.read_bytes()
        assert data[:8] == b'\x89PNG\r\n\x1a\n'
        assert hashlib.sha256(data).hexdigest() == asset['sha256'], 'Stale art hash: ' + asset['id']
        assert list(struct.unpack('>II', data[16:24])) == asset['size'], 'Wrong dimensions'
        assert path.with_name(path.name + '.meta').is_file(), 'Missing Editor-generated asset meta'
        if asset['id'] != 'base':
            assert data[25] in (4, 6) and asset['has_transparent_pixels'] and not asset['alpha_edge_touch']
            assert asset['anchor_top_left_pixels'] and asset['visible_bbox']
    for source in manifest['sources']:
        assert hashlib.sha256((ROOT / source['file']).read_bytes()).hexdigest() == source['sha256']
        assert (ROOT / source['prompt_file']).is_file()
    presentation = project / 'Assets/Game/Presentation'
    forbidden = r'\b(WorldSnapshot|CommittedWorld|CommandProcessor|CommandReceipt|InspectCommitted)\b|using\s+Game\.(Infrastructure|Domain)'
    checked = []
    for path in sorted(presentation.glob('*.cs')):
        assert path.with_name(path.name + '.meta').is_file(), 'Missing source meta'
        # Only explicit trusted composition roots may load adapters; all presenters remain checked.
        if path.name not in {'RepairClientBootstrap.cs', 'OriginClientBootstrap.cs', 'WeavingClientBootstrap.cs'}:
            assert not re.search(forbidden, path.read_text()), 'Raw engine dependency in presenter: ' + path.name
            checked.append(path.name)
    assert {'RepairClientView.cs', 'RepairSceneView.cs', 'RepairAutoplay.cs'}.issubset(checked)
    assert (project / 'Assets/Game/Scenes/Sprint15Repair.unity').is_file()
    assert (project / 'Assets/Game/Resources/Sprint15/Fonts/LICENSE.txt').is_file()
    print(json.dumps({'scope': 'authored source/asset/layout checks only; not Unity runtime or gameplay acceptance',
                      'art_assets': len(expected), 'presenters_checked': checked,
                      'profile_resource_matches': True, 'source_provenance': len(manifest['sources'])}, indent=2))


if __name__ == '__main__':
    main()
