# Bàn giao phát triển — 1.1-prototype-contracts

## Những gì đã có

29 module GDD chi tiết và phụ lục Life Event, source archive, source/section index, baseline/decision register, open decisions, ownership/contracts, QA matrix, scenario specifications và implementation backlog. Repository có .NET solution với portable core primitives, Domain world/rules, Application in-memory pipeline và headless transfer demo. Client Unity có layout chuẩn bị, chưa được Unity Editor bootstrap.

Nội dung nghiên cứu không bị thay bằng bản tóm tắt. Phần mô tả áp dụng được đặt đầu mỗi module để dev biết đoạn đề xuất cũ nào không còn hiệu lực. Các export tập hợp cùng nội dung làm việc, không dựa vào gói ZIP 13 trang của phiên cũ.

## Cách đọc theo vai trò

| Vai trò | Đọc trước | Đọc sâu khi thực hiện |
|---|---|---|
| Tech lead | ARCHITECTURE, Applicable Baseline, Ownership, Backlog | M25–29, Foundation Contract, QA |
| Game designer | M01/07/22, Decision Register | M08–21 theo feature; M27 cho công thức |
| Narrative/world | M02–06, 06A, Canon Register | M16/17/19/21, content guides |
| Gameplay engineer | Ownership, M09/12/13, M27 | Feature-specific module + transactions |
| UI/client | M23, knowledge constraints, Unity bootstrap | M11/14/16/17/21/22 |
| QA | Requirements, Test Matrix, scenarios | Module quality gates, replay/recovery |

## Sẵn sàng bắt đầu ở đâu?

Sprint 1 có code/tests identity/time/codec/RNG; xem [sprint evidence](../qa/SPRINT-01-EVIDENCE.md) và backlog. Sprint2 có WorldSnapshot/rules/command/invariant path in-memory, xem [evidence Sprint2](../qa/SPRINT-02-EVIDENCE.md). Sprint3 có event-v1/causal journal và idempotent diagnostic consumer, xem [evidence Sprint3](../qa/SPRINT-03-EVIDENCE.md). Sprint4 có scheduler/AdvanceTime in-memory, xem [evidence Sprint4](../qa/SPRINT-04-EVIDENCE.md). Sprint5 có headless SQLite current-state commit/reopen, xem [evidence Sprint5](../qa/SPRINT-05-EVIDENCE.md). Sprint6 bổ sung snapshot/recovery/migration schema1→2 sang file mới; xem [evidence Sprint6](../qa/SPRINT-06-EVIDENCE.md) trước khi nhận task. Task tiếp theo FND-13 replay; Unity, platform atomic replace và OPEN03 client provider gate chưa hoàn thành. Không có P0 ở mức “thiếu toàn bộ thiết kế” buộc phải viết thêm module; các lựa chọn kỹ thuật còn mở phải được xử lý trong task tương ứng trước khi merge feature phụ thuộc.

Chưa sẵn sàng tuyên bố Foundation hoàn thành, Unity chạy được, full game balance ổn hoặc full-production content đủ. Xem `OPEN-DECISIONS.csv` và kết quả kiểm tra thực tế. Khả năng 1.000 nhân vật là mục tiêu sản phẩm, không là test đã pass.

## Cập nhật tài liệu

Giữ archive nguyên bản. Sửa baseline/ADR hoặc integration notes nếu thay quyết định; thêm source amendment khi viết lại body lớn. `build_docs.py` kiểm tra mapping và bảo toàn nguồn; nó không phải công cụ để tự xóa phần dài. Regenerate exports sau khi docs validation pass. Không sửa Master export riêng rồi bỏ quên module source.

## Điểm dừng thiết kế

Bộ nghiên cứu đã bao phủ các hệ chính và nền kỹ thuật. Công việc tiếp theo có giá trị là hiện thực hóa, đo đạc và kiểm chứng từng milestone. Chỉ quay lại thiết kế khi có một design gap cụ thể được ghi tác động và acceptance; không kéo dài chuỗi Module 30/31 chỉ để làm tài liệu lớn hơn.

## Đặc tả triển khai bổ sung — 1.1

Đọc [Documentation Completion](DOCUMENTATION-COMPLETION-2026-10-14.md) trước khi giao task. Identity/time/RNG/codec/activity/save/UX/test execution/content authoring đã có tài liệu bổ sung; gaps cần code/spike vẫn được giữ minh bạch trong register. VS-01 là headless kernel gate, Foundation complete cần FND-16. Không tạo ZIP; bản làm việc là repository và các export được kiểm source freshness.

## Workflow hỗ trợ bằng skills

[Bộ skill của project](../runbooks/PROJECT-SKILLS.md) dẫn từ task đến contracts và evidence, cùng workflow prototype/content/change impact. Không thay backlog hoặc gates; việc tạo/cài skills không đánh dấu runtime task hoàn tất.

## Tích hợp và phát hành

Theo [Git Workflow](../runbooks/GIT-WORKFLOW.md); product VERSION và CHANGELOG ở root. Git tag không thay milestone acceptance. main hiện có initial scaffold snapshot, chưa là game release.

## Tiếp nhận công việc

Đọc [WORKLOG](../../WORKLOG.md) và các events mới nhất trước khi tiếp tục spin; cập nhật assignment/result/close khi có thay đổi đáng kể. Hồi cứu SPIN-000 không phải runtime acceptance. Nhật ký chỉ append, không sửa kết quả cũ.

Quy trình áp dụng cho AI: [AI Execution](../runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope các sprint nằm trong sprint brief được liên kết tại đó.
