#!/usr/bin/env python3
"""Rebuild complete GDD from the immutable imported conversation (stdlib only)."""
import argparse, csv, hashlib, io, json, re
from pathlib import Path
ROOT = Path(__file__).resolve().parents[2]
MAP = json.loads((ROOT / 'tools/docs/module-map.json').read_text())

def turns():
    text = (ROOT / 'docs/source/conversation.md').read_text()
    marks = list(re.finditer(r'<a id="t(\d{3})"></a>\n\n## T\d{3} — [^\n]+\n\n', text))
    result = {}
    manifest = json.loads((ROOT / 'docs/source/manifest.json').read_text())
    for i, m in enumerate(marks):
        body = text[m.end():marks[i+1].start() if i+1 < len(marks) else len(text)].removesuffix('\n\n')
        n = int(m[1]); result[n] = body
        assert hashlib.sha256(body.encode()).hexdigest() == manifest['messages'][n]['text_sha256'], f'Source modified: T{n:03d}'
    return result

def clean(text):
    # Remove only presentation wrappers / unreadable old-session citation tokens.
    # Content, examples, tables, formulas, code and superseded proposals are retained.
    text = re.sub(r':::writing\{[^\n]*\}\n?', '', text)
    text = re.sub(r'</?WritingBlock\b[^>]*>', '', text)
    text = re.sub(r'^:::\s*$', '', text, flags=re.M)
    text = re.sub(r'\ue200[^\ue201]*\ue201', '', text)
    text = re.sub(r'\ue200memcite\ue201', '', text)
    return text.strip()

def csv_text(fields, rows):
    out = io.StringIO(); w = csv.DictWriter(out, fieldnames=fields, lineterminator='\n'); w.writeheader(); w.writerows(rows); return out.getvalue()

def render_body(text, mid, turn, sections):
    output=[]; fence=False; k=0
    for line in clean(text).splitlines():
        if line.startswith('```') or line.startswith('~~~'): fence=not fence
        m = None if fence else re.match(r'^(#{1,6}) (.+)$', line)
        if m:
            k += 1; sid=f'm{mid.lower()}-t{turn:03d}-s{k:03d}'
            output += [f'<a id="{sid}"></a>', '', '#' * min(len(m[1])+1, 6) + ' ' + m[2]]
            sections.append(dict(module=mid,turn=f'T{turn:03d}',section_id=sid,title=m[2]))
        else: output.append(line)
    return '\n'.join(output)

def build():
    source=turns(); outputs={}; coverage=[]; sections=[]
    addenda=json.loads((ROOT/'tools/docs/integration-notes.json').read_text())
    for module in MAP:
        mid=module['id']; path=module['path']; note=addenda[mid]
        front=f'''# Module {mid} — {module['title']}

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** {', '.join(f'[T{t:03d}](../../source/conversation.md#t{t:03d})' for t in module['turns'])}  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

{note['baseline']}

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | {note['inputs']} |
| Đầu ra / bên sở hữu | {note['outputs']} |
| Đọc cùng | {note['read_with']} |
| Cổng nghiệm thu | {note['acceptance']} |
| Việc chưa được chứng minh | {note['open']} |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

'''
        bodies=[]
        for turn in module['turns']:
            before=len(sections)
            body=render_body(source[turn],mid,turn,sections)
            bodies += [f'<!-- SOURCE T{turn:03d} BEGIN -->\n',body,f'\n<!-- SOURCE T{turn:03d} END -->\n']
            coverage.append(dict(module=mid,source_turn=f'T{turn:03d}',file=path,source_characters=len(source[turn]),normalized_characters=len(clean(source[turn])),normalized_sha256=hashlib.sha256(clean(source[turn]).encode()).hexdigest(),sections=len(sections)-before))
        outputs[path]=front+'\n'.join(bodies)+'\n'
    for s in sections:
        mod=next(m for m in MAP if m['id']==s['module']);s['file']=mod['path'];s['link']=mod['path']+'#'+s['section_id']
    outputs['docs/governance/SOURCE-COVERAGE.csv']=csv_text(list(coverage[0]),coverage)
    outputs['docs/governance/SECTION-INDEX.csv']=csv_text(list(sections[0]),sections)
    outputs['docs/governance/SOURCE-MAPPING.md']='''# Mapping và bảo toàn nguồn

01–06 được chuẩn hóa từ các phần Concept/World Bible, không giả định chúng vốn là sáu file module có sẵn. Module 02 giữ cả world premise và 12 vùng. Phụ lục 06A giữ toàn bộ thư viện nguồn mang tên **05A**; đổi vị trí vì nó thuộc Life Event, không thuộc Psychology. Modules 07–29 giữ số của bản chi tiết cuối.

Các đoạn T051/T052, T055/T056… là bản giới thiệu/bao ngoài có lặp WritingBlock. Chọn T053, T057, T061, T065, T069, T073, T077, T081 làm nguồn module đầy đủ. Toàn bộ bản lặp và introductory research commentary vẫn có trong archive 85 thông điệp. T019 là roadmap cũ, T083 là thông báo gói bàn giao ngắn đã bị người dùng phản hồi tại T084; không dùng hai đoạn này thay cho thiết kế chi tiết.

Không cắt nội dung nghiên cứu trong các lượt đã chọn. Chỉ bỏ wrapper `WritingBlock`/`:::writing` và token citation không hoạt động ngoài phiên cũ, thêm anchor và đổi cấp heading. Citation gốc vẫn có trong archive. Không chứng nhận lại các phát biểu sử học hoặc mọi URL nghiên cứu trong lần đóng gói này.

| Module | Nguồn | File |
|---|---|---|
'''+''.join(f'| {m["id"]} | {", ".join(f"T{t:03d}" for t in m["turns"])} | [{m["title"]}](../../{m["path"]}) |\n' for m in MAP)+f'\nTổng cộng {len(coverage)} khối nguồn, {len(sections)} mục có anchor. [Coverage CSV](SOURCE-COVERAGE.csv) ghi chiều dài và checksum cho từng khối; [Section Index](SECTION-INDEX.csv) cho phép truy vết tới từng đề mục.\n'
    intro=(ROOT/'docs/gdd/MASTER-INTRO.md').read_text()
    links='\n'.join(f'- [{m["id"]} — {m["title"]}]({Path(m["path"]).relative_to("docs/gdd")})' for m in MAP)
    outputs['docs/gdd/README.md']='# GDD chi tiết — Hồng Bàng / Mạch Lạc\n\n'+intro+'\n## Đọc theo module\n\n'+links+'\n\n[Master đầy đủ](MASTER-GDD.md) chứa toàn bộ 29 module và phụ lục; không phải executive summary.\n'
    # Combined Markdown retains unique explicit section IDs; fix relative document links.
    combined=[intro,'\n## Mục lục module\n','\n'.join(f'- [{m["id"]} — {m["title"]}](#module-{m["id"].lower()})' for m in MAP)]
    for m in MAP:
        body=outputs[m['path']].replace('](../../','](../')
        combined += [f'\n<a id="module-{m["id"].lower()}"></a>\n',body]
    outputs['docs/gdd/MASTER-GDD.md']='\n'.join(combined)+'\n'
    outputs['docs/governance/RECOVERY-REPORT.md']=f'''# Báo cáo khôi phục chi tiết

Phiên bản 1.1-prototype-contracts. Có 29 module và một phụ lục event, lấy từ {len(coverage)} khối nguồn dài. Tổng nội dung sau khi bỏ wrapper/citation: **{sum(r['normalized_characters'] for r in coverage):,} ký tự**; **{len(sections):,} đề mục** được đánh anchor. Đây là kích thước văn bản, không phải thước đo chất lượng hoặc tiến độ code.

`python3 tools/docs/build_docs.py --check` so khớp toàn bộ file sinh lại với nguồn và ghi chú tích hợp. Checksum từng thông điệp nguồn được xác minh trước khi build. So khớp này phát hiện mất nội dung do đóng gói, không chứng minh mọi công thức/thiết kế đã đúng qua prototype.

**Đã xử lý:** mất chi tiết do summary, đánh số nguồn chưa nhất quán, bản WritingBlock trùng, thiếu index, thiếu trạng thái quyết định và thiếu lối nối từ tài liệu tới module code/test.

**Giới hạn còn lại:** các file sandbox cũ không phải nguồn của bản này; game balance và lịch sử chưa được kiểm chứng thực nghiệm; runtime và full-production content chưa được triển khai. Xem [Open Decisions](OPEN-DECISIONS.csv) và [Handoff](../production/HANDOFF.md).
'''
    return outputs

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--check',action='store_true');args=parser.parse_args()
    outputs=build();bad=[]
    for rel,body in outputs.items():
        p=ROOT/rel
        if args.check:
            if not p.exists() or p.read_text()!=body:bad.append(rel)
        else:p.parent.mkdir(parents=True,exist_ok=True);p.write_text(body)
    if bad:raise SystemExit('Generated documentation differs: '+', '.join(bad))
    print(f'{"Verified" if args.check else "Built"} {len(outputs)} documentation files from source.')
if __name__=='__main__':main()
