#!/usr/bin/env python3
"""Check scaffold assembly boundary declarations; runtime invariants have separate tests."""
from pathlib import Path
import xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[2]
allowed={'Game.Foundation':set(),'Game.Domain':{'Game.Foundation'},'Game.Application':{'Game.Foundation','Game.Domain'},'Game.Infrastructure':{'Game.Foundation','Game.Domain','Game.Application'},'Game.Headless':{'Game.Application','Game.Infrastructure'}}
for p in sorted((ROOT/'engine').glob('*/*.csproj')):
    xml=ET.parse(p).getroot();name=p.stem
    refs={Path(n.attrib['Include']).stem for n in xml.iter('ProjectReference')}
    assert refs==allowed[name],f'{name}: unexpected reference graph {refs}'
    for n in xml.iter('ProjectReference'):assert (p.parent/n.attrib['Include']).exists(),'Missing project'
    tfm=xml.findtext('./PropertyGroup/TargetFramework')
    assert tfm==('net10.0' if name=='Game.Headless' else 'netstandard2.1'),f'Wrong framework: {name}'
    assert not list(xml.iter('PackageReference')),'New package requires architecture review/check update'
for p in (ROOT/'engine').rglob('*.cs'):
    if any(k in p.parts for k in ['bin','obj']):continue
    if p.relative_to(ROOT/'engine').parts[0] in ['Foundation','Domain']:
        text=p.read_text()
        for forbidden in ['using UnityEngine','using Microsoft.Data.Sqlite','using System.Net.Http','DateTime.Now','DateTime.UtcNow','new Random(']:assert forbidden not in text,f'{p}: {forbidden}'
print('Assembly reference graph and portable target boundaries pass (scaffold scope).')
