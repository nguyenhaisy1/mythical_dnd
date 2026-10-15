# Build, export và kiểm tra bộ tài liệu

## Nguồn và file sinh ra

`docs/source/conversation.md` + manifest là archive bất biến. `tools/docs/module-map.json` chọn các source turns đầy đủ; `integration-notes.json` ghi baseline đầu từng module. `docs/gdd/MASTER-INTRO.md` là phần dẫn nhập authored. `build_docs.py` sinh 29 module, appendix, Master và coverage/index/report. Không sửa generated output rồi bỏ quên inputs.

Governance/ADR/contracts/QA/production/runbooks là tài liệu authored hoặc CSV có thể sửa trực tiếp. Khi đổi quyết định, cập nhật source/requirement/task/test references và integration notes liên quan. Nội dung mới cần provenance rõ, không chỉnh archive gốc cho thuận tiện.

```bash
python3 tools/docs/build_docs.py
python3 tools/checks/check_handoff.py
python3 tools/docs/export.py
python3 tools/checks/check_exports.py
```

Export cần Pandoc 3.x. Sinh Master đầy đủ, năm volume và Development Handbook ở `artifacts/`; mỗi loại có Markdown/HTML/DOCX. Handbook đưa cả register/matrix CSV vào bảng để người đọc không phải suy đoán các task. Master giữ toàn bộ nội dung 29 module + appendix, không chuyển thành overview.

HTML có mục lục và tìm bằng trình duyệt; DOCX có heading/TOC để mở trong Word/LibreOffice. Chưa xuất PDF vì môi trường không có PDF renderer được kiểm chứng. Không tạo ZIP. Tài liệu được duy trì trực tiếp trong repository; HTML/DOCX là bản đọc phụ trợ. EXPORT-MANIFEST lưu cả input fingerprints và output hashes; checker phát hiện source thay đổi sau export.

## Kiểm tra mức đầy đủ

`build_docs.py --check` so output với nguồn và ghi chú hiện tại. Source message hashes được xác minh trước khi tạo module. `SOURCE-COVERAGE.csv` báo từng khối gốc; `SECTION-INDEX.csv` ghi từng heading/anchor. `check_handoff.py` kiểm đủ 29+06A, source anchors, register/task references, dependency DAG và authored links.

Export giữ citation lịch sử ở archive; token phiên cũ bị bỏ khỏi body đọc được. Old narrative suggestions vẫn giữ để không mất rationale nhưng bị override bằng Applicable Baseline. Cần đọc baseline khi triển khai, không trích một câu cũ ngoài context.

## Khi cập nhật project skills

`tools/skills/` là source được duy trì trong repo, không phải generated output. Giữ hướng dẫn tại [PROJECT-SKILLS](PROJECT-SKILLS.md) khớp với skill routing và installer. Export fingerprints bao gồm source skills để Handbook không giữ thông tin workflow cũ. Không sửa archive/module nghiên cứu chỉ vì thay hướng dẫn agent.

## Live worklog

[WORKLOG](../../WORKLOG.md) là nhật ký vận hành, chỉ append và được track bằng Git. Không nhúng toàn văn hoặc đưa live journal vào export fingerprints: việc ghi kết quả export không được làm chính export stale. Bản đọc chỉ dẫn tới journal hiện tại; thay đổi quy trình ghi log trong AGENTS/runbooks vẫn phải regenerate exports như các source docs khác.
