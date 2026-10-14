#!/usr/bin/env python3
"""Check handoff source coverage, local navigation, cross-register IDs and DAG."""
import csv,json,re,sys
from pathlib import Path
from urllib.parse import unquote,urlsplit
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'tools/docs'))
import build_docs
outputs=build_docs.build()
for rel,body in outputs.items():assert (ROOT/rel).read_text()==body,f'Generated file differs: {rel}'
modules=json.loads((ROOT/'tools/docs/module-map.json').read_text())
assert {m['id'] for m in modules}=={f'{n:02d}' for n in range(1,30)}|{'06A'}
# Verify index anchors actually exist in each module, not just that CSV rows exist.
with (ROOT/'docs/governance/SECTION-INDEX.csv').open() as f:
 sections=list(csv.DictReader(f))
for s in sections:assert f'id="{s["section_id"]}"' in (ROOT/s['file']).read_text(),s

def read_csv(rel):
 with (ROOT/rel).open() as f:return list(csv.DictReader(f))
requirements=read_csv('docs/governance/REQUIREMENTS.csv');tests=read_csv('docs/qa/TEST-MATRIX.csv');tasks=read_csv('docs/production/BACKLOG.csv');decisions=read_csv('docs/governance/DECISIONS.csv')
for rows,key in [(requirements,'id'),(tests,'test_id'),(tasks,'task_id'),(decisions,'id')]:assert len(rows)==len({r[key] for r in rows}),'Duplicate register ID'
reqs={r['id'] for r in requirements};tids={r['test_id'] for r in tests};task_map={r['task_id']:r for r in tasks}
for r in requirements:assert r['acceptance_test'] in tids and r['implementation_task'] in task_map,r
for t in tests:assert t['requirement_id'] in reqs and t['task'] in task_map,t
for d in decisions:assert d['requirement'] in reqs,d
visited=set()
def visit(key,active):
 assert key not in active,'Backlog dependency cycle: '+key
 if key in visited:return
 active=active|{key}
 for dep in task_map[key]['dependencies'].split():
  if dep=='None':continue
  assert dep in task_map,dep
  visit(dep,active)
 visited.add(key)
for key in task_map:visit(key,set())
# Validate authored local file links. Archive and historical source blocks are immutable;
# old research URLs/sandbox references are not asserted to resolve in this repository.
errors=[];checked=0
paths=[ROOT/'README.md',ROOT/'ARCHITECTURE.md',ROOT/'CONTRIBUTING.md']+list((ROOT/'docs').rglob('*.md'))
for path in paths:
 if 'source' in path.relative_to(ROOT).parts:continue
 text=path.read_text()
 text=re.sub(r'<!-- SOURCE T\d+ BEGIN -->.*?<!-- SOURCE T\d+ END -->','',text,flags=re.S)
 text=re.sub(r'```.*?```','',text,flags=re.S)
 for link in re.findall(r'\]\(([^\n)]+)\)',text):
  link=link.strip().strip('<>');parsed=urlsplit(link)
  if parsed.scheme or not parsed.path:continue
  dest=(path.parent/unquote(parsed.path)).resolve()
  if not dest.exists():errors.append(f'{path.relative_to(ROOT)} -> {link}')
  checked+=1
assert not errors,'Broken local links:\n'+'\n'.join(errors)
print(f'Handoff verified: 29 modules + appendix, {len(sections)} source sections, {len(requirements)} requirements, {len(tasks)} tasks, {checked} authored links.')
