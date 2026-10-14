#!/usr/bin/env python3
"""Check DOCX containers/module coverage and HTML navigation after export."""
import csv,hashlib,json,re,zipfile,sys
from html.parser import HTMLParser
from pathlib import Path
import xml.etree.ElementTree as E
ROOT=Path(__file__).resolve().parents[2];OUT=ROOT/'artifacts'
sys.path.insert(0,str(ROOT/'tools/docs'))
from export_inputs import fingerprints
class Anchors(HTMLParser):
    def __init__(self):super().__init__();self.ids=[];self.links=[]
    def handle_starttag(self,tag,attrs):
        a=dict(attrs)
        if 'id' in a:self.ids.append(a['id'])
        if tag=='a' and a.get('href','').startswith('#'):self.links.append(a['href'][1:])
manifest=json.loads((OUT/'EXPORT-MANIFEST.json').read_text())
assert manifest.get('inputs')==fingerprints(),'Export sources changed or inventory differs; rerun tools/docs/export.py'
for item in manifest['files']:
    p=OUT/item['file'];assert hashlib.sha256(p.read_bytes()).hexdigest()==item['sha256']
    if p.suffix=='.docx':
        with zipfile.ZipFile(p) as z:assert z.testzip() is None
    if p.suffix=='.html':
        a=Anchors();a.feed(p.read_text());assert len(a.ids)==len(set(a.ids)),p.name
        missing=set(a.links)-set(a.ids);assert not missing,(p.name,missing)
ns={'w':'http://schemas.openxmlformats.org/wordprocessingml/2006/main'}
with zipfile.ZipFile(OUT/'00-Master-GDD.docx') as z:root=E.fromstring(z.read('word/document.xml'))
text='\n'.join(''.join(t.text or '' for t in p.findall('.//w:t',ns)) for p in root.findall('.//w:p',ns))
for m in json.loads((ROOT/'tools/docs/module-map.json').read_text()):assert f'Module {m["id"]} — {m["title"]}' in text,m['id']
a=Anchors();a.feed((OUT/'00-Master-GDD.html').read_text())
with (ROOT/'docs/governance/SECTION-INDEX.csv').open() as f:sections=list(csv.DictReader(f))
assert all(s['section_id'] in a.ids for s in sections)
report={'source_inputs_verified':len(manifest['inputs']),'files_verified':len(manifest['files']),'module_titles_in_docx':30,'source_section_anchors_in_html':len(sections),'docx_headings':sum('Heading' in x.get('{'+ns['w']+'}val','') for x in root.findall('.//w:pStyle',ns)),'docx_tables':len(root.findall('.//w:tbl',ns)),'internal_html_navigation':'valid','scope':'container/content/navigation checks; not a visual rendering or game acceptance test'}
(OUT/'EXPORT-VALIDATION.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
