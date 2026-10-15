---
name: mythical-change-impact
description: "Propagate mythical_dnd design, contract, schema, parameter or canon changes across dependent docs, fixtures, code, QA and generated exports. Use when updating shared decisions or auditing consistency; not for unrelated cosmetic edits."
---

# Đồng bộ thay đổi Mythical

## Phạm vi và nguồn chuẩn

Chỉ áp dụng cho repository mythical_dnd / Hồng Bàng — Mạch Lạc. Xác định project root từ working directory và kiểm có `.agent`, `MythicalDnd.sln`, `docs/governance/APPLICABLE-BASELINE.md`; không áp dụng vào repo khác vì tên file giống nhau. Nếu được gọi ngoài project và chưa biết đường dẫn, hỏi đường dẫn trước khi sửa. Các đường dẫn bên dưới tính từ project root, không từ thư mục skill hay thư mục cài symlink.

Đọc `AGENTS.md`, `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` khi bắt đầu phiên hoặc khi chúng thay đổi; tái dùng context còn hiệu lực. Chỉ đọc module/contract liên quan, không nạp toàn bộ GDD mỗi task. Yêu cầu hiện tại của người dùng có ưu tiên; skill không tự cấp quyền mở rộng scope, triển khai game, cài công cụ hoặc chạy multi-agent. Không thêm bước xin duyệt cho việc đã được cho phép.

## Xác định thay đổi thực sự

Lấy before/after từ diff giới hạn đúng files. Nếu chưa có Git HEAD/history, dùng nội dung trước khi sửa hoặc snapshot người dùng cung cấp; không coi thiếu Git history là không có tác động. Với yêu cầu audit chưa đổi gì, báo findings và đề xuất; không tự đổi thiết kế.

Ghi changed facts: identity, owner, units, formula, timing, schema/version, canonical name hoặc acceptance. Tìm references bằng `rg` với identifiers/parameter keys/task IDs; tìm cả mô tả ngữ nghĩa có thể không dùng cùng từ khóa. Dùng `tools/docs/module-map.json`, `docs/governance/SECTION-INDEX.csv`, requirements/backlog/test matrix để khoanh source. Kết quả grep bằng zero không chứng minh không có consumer.

## Theo dấu các consumer liên quan

| Loại thay đổi | Nơi cần xét |
|---|---|
| Thiết kế/contract | Applicable Baseline, DECISIONS/OPEN-DECISIONS, ADR, contracts/ownership, integration notes, code consumers |
| Numeric/schema/command | rules/parameters, content schemas/definitions/fixtures, codec/golden versions, save compatibility, validation tools, runtime tests |
| Canon/địa lý | CANON-REGISTER, content IDs/refs, module notes, slice/scenarios và whitepaper nếu mô tả sản phẩm bị đổi |
| Acceptance/milestone | REQUIREMENTS, BACKLOG dependencies/status, TEST-MATRIX, QA plan, implementation plan, handoff |
| Trạng thái/tooling | .agent, README/docs index, runbooks, CI, evidence và export inputs nếu thêm nguồn mới |

Chỉ sửa nơi thực sự bị ảnh hưởng. Phân biệt thiết kế mới, hiệu chỉnh lỗi và kết quả thực nghiệm. Có thể ghi impact table ngắn `changed fact → consumers → sửa/không ảnh hưởng + lý do`; không tạo một báo cáo dài cho mỗi typo.

## Giữ nguồn và build artifacts nhất quán

`docs/source/` là archive bất biến. Các module, appendix và Master được sinh: đổi `tools/docs/integration-notes.json`, `docs/gdd/MASTER-INTRO.md` hoặc nguồn authored phù hợp; không sửa body nghiên cứu để giả rằng thiết kế mới có trong hội thoại. Breaking design cần ADR/amendment và version/compatibility decision phù hợp.

Golden chỉ đổi khi contract thay đổi có lý do, không regenerate để che implementation sai. Tách vector/projection hash tham chiếu khỏi full runtime state hash. Giữ audit lịch sử với banner trỏ trạng thái mới; không sửa kết quả test cũ thành Passed cho phiên bản mới.

Theo `docs/runbooks/DOCUMENTATION.md`: regenerate nếu generated inputs đổi, kiểm docs/refs/DAG, hoàn tất cập nhật evidence trước export, regenerate HTML/DOCX và check input fingerprints sau cùng. Không tạo ZIP. Nếu thiếu công cụ export, hoàn thành source và nói rõ exports stale; không tự cập nhật manifest để giả freshness.

Tổng kết impacted files đã xử lý và pending thật sự. Khi user yêu cầu đồng bộ, hoàn thành các cập nhật trong phạm vi được phép; không chỉ liệt kê rồi dừng.

Thay đổi đáng chú ý cần xét `CHANGELOG.md`; product release VERSION/tag tách khỏi contract/GDD versions. Branch/commit/push và release propagation theo `docs/runbooks/GIT-WORKFLOW.md`, không tự phát hành vì đã đồng bộ docs.
