#!/usr/bin/env python3
"""Export complete source-derived GDD and developer handbooks using Pandoc."""
import csv,hashlib,html,json,re,shutil,subprocess,tempfile,zipfile
from pathlib import Path
from urllib.parse import urlsplit,unquote
from export_inputs import fingerprints
ROOT=Path(__file__).resolve().parents[2];OUT=ROOT/'artifacts'
CSS='''<style>
:root{color-scheme:light;--ink:#202c32;--muted:#586972;--line:#d6dfe3;--accent:#17635e}
body{margin:0 auto;padding:2.5rem;max-width:1000px;background:#fbfcfc;color:var(--ink);font:17px/1.65 system-ui,"Segoe UI",sans-serif}
h1,h2,h3,h4{line-height:1.25;color:#183f3b;scroll-margin-top:1.5rem}h1{font-size:2.05rem;border-bottom:3px solid var(--accent);padding-bottom:.6rem;margin-top:3rem}h2{margin-top:2.2rem}a{color:var(--accent)}
#TOC{padding:1rem 1.5rem;background:#edf3f2;border:1px solid var(--line);border-radius:8px}#TOC ul{padding-left:1.5rem}
pre{padding:1rem;overflow:auto;background:#edf1f3;border-radius:5px;font-size:.87rem}code{font-family:Consolas,monospace}table{display:block;overflow:auto;border-collapse:collapse;width:100%;font-size:.88rem;margin:1.2rem 0}th,td{border:1px solid var(--line);padding:.6rem;vertical-align:top}th{background:#e5efec}blockquote{border-left:4px solid #96b5ab;padding:.2rem 1rem;color:#425950;margin-left:0}.subtitle,.date{color:var(--muted)}
@media(max-width:700px){body{padding:1rem;font-size:16px}h1{font-size:1.6rem}}@media print{body{max-width:none;padding:0;font-size:10pt}h1{break-before:page}pre,table{font-size:8pt}a{color:inherit}}
</style>'''

def relocate(text,source):
    def repl(m):
        value=m[1];u=urlsplit(value)
        if u.scheme or not u.path or value.startswith('#'):return m[0]
        # Only rewrite real local files, leaving research snippets as authored.
        target=(source.parent/unquote(u.path)).resolve()
        if not target.exists():return m[0]
        try:rel=target.relative_to(ROOT)
        except ValueError:return m[0]
        return ']('+('../'+rel.as_posix())+('#'+u.fragment if u.fragment else '')+')'
    return re.sub(r'\]\(([^\n)]+)\)',repl,text)

def read(rel):
    p=ROOT/rel;return relocate(p.read_text(),p)

def csv_table(path):
    with path.open() as f:rows=list(csv.reader(f))
    def safe(x):return x.replace('|','\\|').replace('\n','<br>')
    return '\n'.join(['| '+' | '.join(map(safe,rows[0]))+' |','| '+' | '.join(['---']*len(rows[0]))+' |']+['| '+' | '.join(map(safe,r))+' |' for r in rows[1:]])

def main():
    if not shutil.which('pandoc'):raise SystemExit('Pandoc 3.x is required for HTML/DOCX exports.')
    subprocess.run(['python3',str(ROOT/'tools/docs/build_docs.py'),'--check'],check=True,cwd=ROOT)
    OUT.mkdir(exist_ok=True)
    source_inputs=fingerprints()
    modules=json.loads((ROOT/'tools/docs/module-map.json').read_text())
    products=[('00-Master-GDD','Hồng Bàng — Mạch Lạc: GDD đầy đủ',read('docs/gdd/MASTER-GDD.md'))]
    groups=[('01-World-and-Origins','Thế giới, con người và xuất thân',list(range(1,8))+['06A']),('02-Character-and-Combat','Năng lực, nghề nghiệp, party và combat',list(range(8,14))),('03-Living-World-and-Legacy','Khám phá, xã hội, kinh tế, tín ngưỡng và di sản',list(range(14,22))),('04-Experience-and-Engine','Trải nghiệm, kiến trúc, dữ liệu và công thức',list(range(22,28))),('05-Implementation-Blueprint','Vertical slice và Engine Foundation',list(range(28,30)))]
    for stem,title,nums in groups:
        ids=[f'{n:02d}' if isinstance(n,int) else n for n in nums]
        body=f'# {title}\n\nVolume thuộc baseline 1.1-prototype-contracts. Giữ toàn văn các module tương ứng. Đọc Applicable Baseline trước khi triển khai.\n\n'
        body+='\n\n'.join(read(next(m['path'] for m in modules if m['id']==i)) for i in ids)
        products.append((stem,title,body))
    handbook=['# Development Handbook — Hồng Bàng / Mạch Lạc\n\nBản bàn giao 1.1-prototype-contracts. Các chương dưới đi cùng full GDD, không thay thiết kế chi tiết.\n']
    authored=['ARCHITECTURE.md','docs/production/HANDOFF.md','docs/governance/APPLICABLE-BASELINE.md','docs/governance/CANON-REGISTER.md','docs/governance/SOURCE-MAPPING.md','docs/governance/RECOVERY-REPORT.md','docs/contracts/OWNERSHIP.md','docs/contracts/FOUNDATION-CONTRACT.md','docs/contracts/DATA-MODEL-AND-SCHEMAS.md','docs/production/IMPLEMENTATION-PLAN.md','docs/production/VERTICAL-SLICE.md','docs/qa/QA-PLAN.md','docs/qa/SCENARIO-SPECIFICATIONS.md','docs/qa/VALIDATION-RESULTS.md','docs/qa/FND-02-EVIDENCE.md','docs/qa/SPRINT-01-EVIDENCE.md','docs/content-guides/AUTHORING.md','docs/runbooks/DEVELOPMENT.md','docs/runbooks/UNITY-BOOTSTRAP.md','docs/runbooks/DOCUMENTATION.md','docs/runbooks/PROJECT-SKILLS.md','docs/runbooks/GIT-WORKFLOW.md','docs/runbooks/AI-EXECUTION.md','docs/production/sprints/SPRINT-01.md','CHANGELOG.md']
    authored += [str(p.relative_to(ROOT)) for p in sorted((ROOT/'docs/contracts').glob('*-SPEC.md'))]
    authored += ['docs/contracts/COMMAND-ACTIVITY-CATALOG.md','docs/qa/TEST-EXECUTION-CONTRACT.md','docs/content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md','docs/production/CLIENT-UX-BASELINE.md','docs/production/PRE-DEVELOPMENT-AUDIT-2026-10-14.md','docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md','content/fixtures/kernel-smoke/README.md']
    authored += [str(p.relative_to(ROOT)) for p in sorted((ROOT/'docs/adr').glob('[0-9]*.md'))]
    for rel in authored:handbook += ['\n\n',read(rel)]
    for rel in ['docs/governance/DECISIONS.csv','docs/governance/OPEN-DECISIONS.csv','docs/governance/REQUIREMENTS.csv','docs/production/BACKLOG.csv','docs/qa/TEST-MATRIX.csv','docs/governance/READINESS-GAPS-2026-10-14.csv']:
        handbook += ['\n\n# '+Path(rel).stem+'\n\n',csv_table(ROOT/rel)]
    products.append(('06-Development-Handbook','Development Handbook — contracts, governance, plan và QA',''.join(handbook)))
    manifest=[]
    with tempfile.TemporaryDirectory(prefix='mythical-doc-export-') as tmp:
        css=Path(tmp)/'style.html';css.write_text(CSS)
        for stem,title,body in products:
            md=OUT/(stem+'.md');md.write_text(body)
            common=['pandoc',str(md),'--data-dir',str(ROOT/'tools/docs/pandoc-data'),'--from=markdown+pipe_tables+fenced_code_blocks+raw_html','--standalone','--toc','--toc-depth=1','--metadata',f'title={title}','--metadata','lang=vi-VN','--metadata','date=2026-10-14']
            warnings=[]
            for extension,args in [('html',['--include-in-header',str(css),'--embed-resources']),('docx',[])]:
                target=OUT/(stem+'.'+extension)
                proc=subprocess.run(common+args+['-o',str(target)],cwd=ROOT,capture_output=True,text=True)
                if proc.returncode:raise SystemExit(proc.stderr)
                if proc.stderr:warnings.append(proc.stderr)
                if extension=='docx':
                    with zipfile.ZipFile(target) as z:
                        assert z.testzip() is None
                        xml=z.read('word/document.xml').decode();assert 'Hồng Bàng' in xml or 'MODULE' in xml or 'Foundation' in xml
                manifest.append({'file':target.name,'bytes':target.stat().st_size,'sha256':hashlib.sha256(target.read_bytes()).hexdigest()})
            log=OUT/(stem+'.pandoc.log')
            if warnings:log.write_text('\n'.join(warnings))
            else:log.unlink(missing_ok=True)
            print(f'Exported {stem}: {len(body):,} characters',flush=True)
    assert source_inputs==fingerprints(),'Sources changed during export; rerun.'
    (OUT/'EXPORT-MANIFEST.json').write_text(json.dumps({'version':'1.1-prototype-contracts','inputs':source_inputs,'files':manifest},ensure_ascii=False,indent=2)+'\n')
    print('Verified 7 HTML and 7 DOCX containers. No PDF exported.')
if __name__=='__main__':main()
