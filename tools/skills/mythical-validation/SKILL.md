---
name: mythical-validation
description: "Review mythical_dnd game design, art/UX prototypes and implementation claims against actual documentation, reference, runtime, Unity and playtest evidence. Use for checks, acceptance reviews and milestone readiness; do not label reference checks as gameplay tests."
---

# Kiểm chứng Mythical

## Phạm vi và nguồn chuẩn

Chỉ áp dụng cho repository mythical_dnd / Hồng Bàng — Mạch Lạc. Xác định project root từ working directory và kiểm có `.agent`, `MythicalDnd.sln`, `docs/governance/APPLICABLE-BASELINE.md`; không áp dụng vào repo khác vì tên file giống nhau. Nếu được gọi ngoài project và chưa biết đường dẫn, hỏi đường dẫn trước khi sửa. Các đường dẫn bên dưới tính từ project root, không từ thư mục skill hay thư mục cài symlink.

Đọc `AGENTS.md`, `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` khi bắt đầu phiên hoặc khi chúng thay đổi; tái dùng context còn hiệu lực. Chỉ đọc module/contract liên quan, không nạp toàn bộ GDD mỗi task. Yêu cầu hiện tại của người dùng có ưu tiên; skill không tự cấp quyền mở rộng scope, triển khai game, cài công cụ hoặc chạy multi-agent. Không thêm bước xin duyệt cho việc đã được cho phép.

## Chọn phép kiểm tra theo thay đổi

Đọc `docs/qa/TEST-EXECUTION-CONTRACT.md`, TEST-MATRIX và acceptance của task; xem code/test projects hiện có thay vì mặc định chúng đã được tạo. Phân loại evidence trước khi kết luận:

| Scope | Chứng minh được | Không chứng minh được |
|---|---|---|
| Docs/reference | links, coverage, fixtures/vectors, formulas tham chiếu | handler C# chạy đúng, game chơi được |
| Build/scaffold | assembly biên dịch và boundaries kiểm tĩnh | runtime replay/durability/Unity compatibility |
| Runtime | hành vi thực thi trong runtime được test | runtime/platform khác chưa chạy |
| Unity/visual | Editor/Player/backend cụ thể, UI thật quan sát được | toàn bộ simulation invariants hoặc người chơi thấy vui |
| Playtest | hành vi/nhận xét tester thật trong session | đại diện toàn thị trường hoặc balance đầy đủ |

## Lệnh và evidence

- Generated/source: `python3 tools/docs/build_docs.py --check`; `python3 tools/checks/check_handoff.py`.
- Reference data: `python3 tools/checks/validate_reference.py`; `python3 tools/checks/check_contract_vectors.py`. Script thứ hai kiểm Python reference, không chạy C# command processor.
- Boundaries: `python3 tools/checks/check_architecture.py`; build solution theo `docs/runbooks/DEVELOPMENT.md` khi code/project config đổi.
- Runtime: xác nhận test project/framework/runner thật, lệnh trong repo và discovery count. Exit 0 với zero tests, skipped hoặc placeholder host không đạt acceptance. Lệnh `dotnet test` trong execution contract có thể là kế hoạch chưa implemented; không báo nó đã kiểm game khi chưa có tests.
- Export: `python3 tools/checks/check_exports.py` sau regenerate; kiểm source fingerprints lẫn outputs, không chỉ manifest tự so với file cũ.
- Client: xác định Unity Editor/backend/platform thực tế, tests và ảnh/layout đã quan sát. Compile hoặc ảnh screenshot đơn lẻ không thay knowledge-isolation/interaction tests. Thiếu Editor thì ghi Not run, không giả kết quả.

Run checks phù hợp và required CI, không chạy benchmark/toàn suite vô hạn cho thay đổi nhỏ. Với invariant trọng yếu, test outcome chứ không chỉ kiểm chuỗi code có mặt. Giữ known-answer độc lập; failure không được xử lý bằng overwrite golden.

Ghi command, exit, discovered/passed/failed/skipped, versions/seed và artifact paths theo execution contract. Đặt evidence generated ở `tests/results/` khi cần, report duy trì ở QA theo scope. Nếu source thay đổi sau run, xác định evidence nào mất hiệu lực. Chỉ cập nhật matrix/backlog status khi acceptance tương ứng đạt.

Kết luận từng gate là đạt/chưa đạt/chưa kiểm, kèm lý do. VS-01 là headless kernel; Foundation complete cần FND-16. Playtest chưa diễn ra thì chỉ tạo protocol/hypotheses, không viết phản hồi tưởng tượng.

Nghiệm thu release theo `docs/runbooks/GIT-WORKFLOW.md`: VERSION/changelog/tag khớp scope, evidence áp revision cuối; tag/CI scaffold không thay gameplay gates. Phân biệt chính sách branch trong file với protection thực tế trên GitHub.

Append kết quả checks đáng kể và phạm vi chưa kiểm vào `WORKLOG.md` theo AGENTS; không nhúng log vào exports hoặc đổi kết quả lịch sử. Khi report, đọc event mới nhất và xác minh trạng thái Git/evidence hiện hành, không suy mọi task hoàn tất từ một spin CLOSE.

## Phản biện game design và art/UX

Khi được giao review thiết kế hoặc prototype, đọc [Game Design Review](references/game-design-review.md). Đối chiếu cả ý định, dữ liệu và artifact người chơi thấy; báo lỗi, preference và câu hỏi riêng. Review không tự cấp quyền viết implementation hoặc chốt sản phẩm. Nguồn adaptation và license ở [Design Sources](../upstream/DESIGN-SOURCES.md).
