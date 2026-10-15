# SPRINT-01 — Nền tảng kiểm thử và tính xác định

Mở ngày 2026-10-15. Trạng thái: Completed — local headless scope, 2026-10-15. Mô hình thực hiện: AI theo [AI Execution](../../runbooks/AI-EXECUTION.md); điều phối/nghiệm thu /root; FND-03 /root/clock, FND-04 /root/random trong SPIN-003. Không đặt thời hạn giả định theo nhân lực con người.

## Mục tiêu và phạm vi

Tạo primitive C# portable có kiểm thử thực thi để các task world/command có thể sử dụng. Input: ADR-008, identity/time/RNG specs, REQ-027/008/007, vectors kernel-smoke và scaffold FND-01 đã build.

| Task | Output / acceptance | Trạng thái |
|---|---|---|
| Baseline vận hành | Quy trình AI, Git/worklog, commit local kiểm được | Đạt local baseline 94b0e53; chưa publish |
| FND-02 | Typed entity/definition IDs, staged allocator/registry/lifecycle, restore; TEST-027 headless, runner pin/lock, CI nonzero + negative control | Headless accepted — SPIN-002, /root; 32 tests + negative control; Unity pending |
| FND-03 | WorldClock/lịch 360 ngày, range/boundary tests | Headless accepted — SPIN-003 |
| FND-04 | sha256-counter-v1/mdc1, khớp golden, không dùng global RNG | Headless accepted — SPIN-003 |

Allowed scope: engine/Foundation, tests primitive, tooling/CI và tài liệu consumers. Không triển khai command pipeline, world generator, save SQLite, replay engine, Unity client hoặc full slice. FND-02 chỉ trả staged snapshot để Application tương lai commit; không tuyên bố đã có durable atomic world commit.

## Gate kết thúc

Build Release + tests C# thực được phát hiện, không zero/skipped; negative control fail đúng mong đợi; IDs không tái sử dụng sau commit/retirement, abort/retry và restore có evidence; clock boundaries và RNG/codec golden đạt. Docs/backlog/log đồng bộ, diff được self-review, artifacts có source fingerprint và command. Unity conformance giữ pending FND-16, không giả nghiệm thu bằng .NET build.

Chuẩn bị prototype Bến Vạc là hướng tiếp theo tùy kết quả nền tảng; chưa cam kết gameplay playable trong sprint này. Không cần người làm code review thường lệ; trải nghiệm người chơi thật vẫn là gate riêng chưa kiểm.

## Thứ tự thực hiện

Chốt baseline local → FND-02 và test gate → review/evidence → FND-03 → FND-04 → đánh giá sprint. Mỗi spin ghi assignment/result trong [WORKLOG](../../../WORKLOG.md). Chưa tự mở toàn dependency tree hoặc release.

Evidence hiện tại: [FND-02](../../qa/FND-02-EVIDENCE.md). Sprint đã đóng theo [nghiệm thu](../../qa/SPRINT-01-EVIDENCE.md); bước kế tiếp FND-05.

Ngày 2026-10-15 chủ dự án giao toàn bộ break task/giao task/nghiệm thu cho AI, báo cáo khi xong sprint. Gate chạy trên branch tích hợp local feature/sprint-01-primitives (từ dev, fast-forward nhận FND-02), từng task có commit riêng. Không sửa main hoặc phát hành.

## Kết quả đóng sprint

70/70 Release tests pass (32 identity,25 clock,13 RNG); negative control1fail/exit1 đúng yêu cầu; reviewer /root/review không phát hiện blocker. Unity/TEST-008 scheduler/full Foundation vẫn pending. Local scope complete; publish/PR integration chưa thực hiện.
