---
name: mythical-task
description: "Execute or review scoped mythical_dnd backlog tasks using requirements, ownership, dependencies and acceptance evidence. Use for Foundation/vertical-slice work or preparing a concrete implementation task; not for unrelated repositories or automatically starting the entire roadmap."
---

# Thực hiện task Mythical

## Phạm vi và nguồn chuẩn

Chỉ áp dụng cho repository mythical_dnd / Hồng Bàng — Mạch Lạc. Xác định project root từ working directory và kiểm có `.agent`, `MythicalDnd.sln`, `docs/governance/APPLICABLE-BASELINE.md`; không áp dụng vào repo khác vì tên file giống nhau. Nếu được gọi ngoài project và chưa biết đường dẫn, hỏi đường dẫn trước khi sửa. Các đường dẫn bên dưới tính từ project root, không từ thư mục skill hay thư mục cài symlink.

Đọc `AGENTS.md`, `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` khi bắt đầu phiên hoặc khi chúng thay đổi; tái dùng context còn hiệu lực. Chỉ đọc module/contract liên quan, không nạp toàn bộ GDD mỗi task. Yêu cầu hiện tại của người dùng có ưu tiên; skill không tự cấp quyền mở rộng scope, triển khai game, cài công cụ hoặc chạy multi-agent. Không thêm bước xin duyệt cho việc đã được cho phép.

## Từ yêu cầu tới phạm vi thực hiện

- Phân biệt yêu cầu lập kế hoạch/review với yêu cầu hiện thực. Review chỉ tạo findings; không tự viết implementation. Nếu user chỉ định task ID, tra đúng hàng trong `docs/production/BACKLOG.csv`, rồi nối `docs/governance/REQUIREMENTS.csv` và `docs/qa/TEST-MATRIX.csv`. Task có thể chứa acceptance ngoài các TEST được map; kiểm cả hai.
- Nếu user mô tả feature không có ID, tìm task gần nhất rồi xác định scope nhỏ nhất đáp ứng yêu cầu. Chỉ thêm task/register khi công việc thực sự mới, không tạo cả hệ epic/story song song với CSV hiện có.
- Kiểm trạng thái dependencies bằng artifacts và evidence, không chỉ nhãn Done. Thiếu dependency: làm phần độc lập đã được cho phép; nêu rõ phần bị chặn. Không triển khai toàn dependency tree chỉ vì nó cần cho task hiện tại. Spike có thể dùng stub nhưng phải ghi rõ giới hạn, không nghiệm thu task tích hợp bằng stub.
- Chốt trong context: requirement/task IDs, allowed files/assemblies, owned state, inputs/outputs, acceptance, non-goals và các quyết định còn mở. Đọc contract chi tiết trước khi thay public API. Không bắt buộc tạo một file kế hoạch cho thay đổi nhỏ.

## Thực hiện và đóng task

Theo `ARCHITECTURE.md` và `docs/contracts/OWNERSHIP.md`; dùng `docs/production/IMPLEMENTATION-PLAN.md` để phân biệt VS-01 headless gate với Foundation complete có FND-16. Không đánh đồng .NET SDK của host với Unity runtime.

Làm thay đổi được giao, chạy kiểm tra phù hợp theo `docs/qa/TEST-EXECUTION-CONTRACT.md`. Khi có thay đổi contract, đồng bộ consumer và tài liệu trong cùng công việc; workflow chi tiết ở `tools/skills/mythical-change-impact/SKILL.md`, đọc khi cần. Chỉ cập nhật status tương ứng bằng chứng đã có. Không ghi ngày hoàn thành hoặc số test nếu chưa chạy.

Bàn giao bằng kết quả, files chính, checks thực sự đã chạy và phần còn pending. Nếu acceptance chưa đạt, ghi partial/blocked đúng phạm vi thay vì đóng task. Không tự commit/push chỉ để có mã revision; evidence có thể dùng source fingerprint theo contract.

## Git handoff

Theo `docs/runbooks/GIT-WORKFLOW.md`: work branch từ dev; cập nhật CHANGELOG cho thay đổi đáng chú ý; commit/push đúng phạm vi đã được giao. Không tự merge main/tag/release khi chỉ được yêu cầu implement. Quyền commit/push đã có thì tiếp tục, không hỏi lại.

## Theo dõi công việc

Đọc `WORKLOG.md` theo AGENTS khi nhận/tiếp tục task; append goal/assignment/progress/result/close cho spin tương ứng bằng `tools/worklog.py`. Ghi task ID, actor thật, evidence và Git state; task status trong backlog vẫn cần acceptance riêng, không tự Done vì spin đã đóng.
