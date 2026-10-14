---
name: mythical-content
description: "Author or audit mythical_dnd NPC, household, route, event, recipe and parameter data with canon provenance and semantic validation. Use when converting GDD research into content packs or checking content consistency; not for generating raster assets or writing runtime LLM behavior."
---

# Content Mythical

## Phạm vi và nguồn chuẩn

Chỉ áp dụng cho repository mythical_dnd / Hồng Bàng — Mạch Lạc. Xác định project root từ working directory và kiểm có `.agent`, `MythicalDnd.sln`, `docs/governance/APPLICABLE-BASELINE.md`; không áp dụng vào repo khác vì tên file giống nhau. Nếu được gọi ngoài project và chưa biết đường dẫn, hỏi đường dẫn trước khi sửa. Các đường dẫn bên dưới tính từ project root, không từ thư mục skill hay thư mục cài symlink.

Đọc `AGENTS.md`, `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` khi bắt đầu phiên hoặc khi chúng thay đổi; tái dùng context còn hiệu lực. Chỉ đọc module/contract liên quan, không nạp toàn bộ GDD mỗi task. Yêu cầu hiện tại của người dùng có ưu tiên; skill không tự cấp quyền mở rộng scope, triển khai game, cài công cụ hoặc chạy multi-agent. Không thêm bước xin duyệt cho việc đã được cho phép.

## Từ nghiên cứu tới record

Đọc `docs/content-guides/AUTHORING.md`, `VERTICAL-SLICE-CONTENT-CONTRACT.md`, `docs/governance/CANON-REGISTER.md` và `docs/contracts/DATA-MODEL-AND-SCHEMAS.md` theo loại content. Tìm module/section bằng SECTION-INDEX và SOURCE-COVERAGE; đọc đủ ngữ cảnh đoạn được dùng, không lấy summary làm toàn bộ nghiên cứu.

Xác định authored definitions, prototype/reference fixtures và compiled runtime output. Ghi source module/section hoặc prototype-authored, schema/version, owning domain, IDs và localized keys theo contract. Không dùng display names làm runtime identity; không tự đoán alias→entity mapping khi compiler chưa có.

Nếu schema/compiler còn thiếu và task chỉ author dữ liệu, tạo dữ liệu tham chiếu có nhãn rõ, ghi gate thiếu; không công bố loadable runtime pack. Nếu user giao cả schema/compiler, thực hiện trong phạm vi đó và thêm semantic tests phù hợp. Không tự mở full roster khi user chỉ yêu cầu một event/recipe.

## Kiểm tra ngữ nghĩa theo loại

- NPC/household: capability/history/knowledge có provenance; quan hệ có hướng; nghĩa vụ và tài nguyên hữu hạn; người chết không có work plan executable.
- Route: endpoints/parent region đúng canon, cost units rõ, directional; travel/trade/rumor dùng shared graph, không tự tạo reverse edge.
- Recipe/resource: owner/rights, units/ranges, inputs/tools/time/outputs và reserve/consume policy theo catalog. Parameters versioned là một nguồn, không hardcode bản sao vào mọi record.
- Event/thread: eligibility trước RNG; condition và approved handler references; ít nhất xét eligible/ineligible, no-player và resource/death failure liên quan. Không embed executable script hoặc cho narrative spawn resources tùy ý.
- Lore: tách historical/traditional/game-canon; nguồn chưa xác minh ghi rõ. Bến Hạc thuộc Phong Châu, household Vạc Tòng khác character Vạc Mộc; nếu canon đổi phải theo decision mới, không sửa archive.

## Kết quả và nghiệm thu

Dữ liệu đặt đúng `content/definitions/`, `content/fixtures/`, `rules/parameters/` hoặc localization theo contract. `content/compiled/` chỉ là build output; không tạo manually rồi nói compiler đã chạy. Dùng validators thực sự hỗ trợ schema đó; `validate_reference.py` chỉ kiểm design fixture đã định nghĩa, không validator phổ quát cho mọi pack.

Tổng kết records thực tế, provenance, refs/units/invariants đã kiểm và phần chưa kiểm. Record count đạt target không đủ chứng minh gameplay. Thay coefficients/canon/handler/schema phải đồng bộ fixtures, scenarios, docs và consumer tests; không tự overwrite golden để làm dữ liệu mới có vẻ đúng.
