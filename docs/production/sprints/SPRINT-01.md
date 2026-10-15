# SPRINT-01 — Nền tảng kiểm thử và tính xác định

Mở ngày 2026-10-15. Trạng thái: In progress. Mô hình thực hiện: AI theo [AI Execution](../../runbooks/AI-EXECUTION.md); actor hiện có /root, không có subagent. Không đặt thời hạn giả định theo nhân lực con người.

## Mục tiêu và phạm vi

Tạo primitive C# portable có kiểm thử thực thi để các task world/command có thể sử dụng. Input: ADR-008, identity/time/RNG specs, REQ-027/008/007, vectors kernel-smoke và scaffold FND-01 đã build.

| Task | Output / acceptance | Trạng thái |
|---|---|---|
| Baseline vận hành | Quy trình AI, Git/worklog, commit local kiểm được | Đạt local baseline 94b0e53; chưa publish |
| FND-02 | Typed entity/definition IDs, staged allocator/registry/lifecycle, restore; TEST-027 headless, runner pin/lock, CI nonzero + negative control | Headless accepted — SPIN-002, /root; 32 tests + negative control; Unity pending |
| FND-03 | WorldClock/lịch 360 ngày, range/boundary tests | Ready sau conventions; chưa làm |
| FND-04 | sha256-counter-v1/mdc1, khớp golden, không dùng global RNG | Ready sau conventions; chưa làm |

Allowed scope: engine/Foundation, tests primitive, tooling/CI và tài liệu consumers. Không triển khai command pipeline, world generator, save SQLite, replay engine, Unity client hoặc full slice. FND-02 chỉ trả staged snapshot để Application tương lai commit; không tuyên bố đã có durable atomic world commit.

## Gate kết thúc

Build Release + tests C# thực được phát hiện, không zero/skipped; negative control fail đúng mong đợi; IDs không tái sử dụng sau commit/retirement, abort/retry và restore có evidence; clock boundaries và RNG/codec golden đạt. Docs/backlog/log đồng bộ, diff được self-review, artifacts có source fingerprint và command. Unity conformance giữ pending FND-16, không giả nghiệm thu bằng .NET build.

Chuẩn bị prototype Bến Vạc là hướng tiếp theo tùy kết quả nền tảng; chưa cam kết gameplay playable trong sprint này. Không cần người làm code review thường lệ; trải nghiệm người chơi thật vẫn là gate riêng chưa kiểm.

## Thứ tự thực hiện

Chốt baseline local → FND-02 và test gate → review/evidence → FND-03 → FND-04 → đánh giá sprint. Mỗi spin ghi assignment/result trong [WORKLOG](../../../WORKLOG.md). Chưa tự mở toàn dependency tree hoặc release.

Evidence hiện tại: [FND-02](../../qa/FND-02-EVIDENCE.md). Bước kế tiếp FND-03; chưa đóng sprint.
