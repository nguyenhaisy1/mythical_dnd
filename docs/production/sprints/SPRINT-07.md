# Sprint7 — Kiểm tra game tính lại cùng một kết quả

## Tổng hợp dễ hiểu

Sprint này xây công cụ chạy lại một chuỗi hành động đã ghi để kiểm tra game có tạo ra đúng kết quả cũ hay không. Nếu kết quả lệch, công cụ chỉ ra hành động đầu tiên xảy ra sai khác, giúp tìm lỗi trước khi phát triển thêm gameplay. Công cụ phục vụ kiểm thử nền tảng, chưa phải chức năng xem lại ván chơi cho người chơi.

Trạng thái: đạt headless scope ngày2026-10-21;284/284tests,2process golden và negativecontrol đạt. SPIN-010, mở2026-10-20.

## Scope và phân công

FND13/TEST009/050 headless transfer+scheduler replay theo ADR016. Input initial committed image + manifest + ordered Execute/Drain actions. Output immutable artifact, checked checkpoints và first divergence report. Không production store/Unity/full gameplay RNG/scale benchmark.

| Task | Actor/ownership | Acceptance |
|---|---|---|
| S7-01 contract/ADR | /root | Record attempts/drains/versions/hash scope rõ |
| S7-02 harness/codec | /root/s7_replay; Infrastructure/Replay | Rerun actual processor, strict artifact, first divergence |
| S7-03 tests | /root/s7_tests; ReplayTests.cs | Rule call proof, refusal, retry/auth/scheduler/order/malformed |
| S7-04 independent review | /root/s7_review | Findings sửa trước nghiệmthu |
| S7-05 CLI/golden/gates | /root; Headless/tools/fixture | Checked-in golden, separate processes, negative control |
| S7-06 docs/Git | /root | Source/export/evidence, PR/dev sau CI |

Giữ đầy đủ256tests và9SIGKILLcases hồi quy Sprint6. Không tự mởSprint8 hoặc release.

## Kết quả nghiệmthu

/root tiếp quản sau replayagent quota; testsagent28Debugcases, root284Releasecases và9SIGKILLregression. Independent review engine+CLI/golden không còn blocker sau boundedinput guard. [Báo cáo](../../qa/SPRINT-07-EVIDENCE.md) có tổng hợp dễ hiểu và giới hạn FND14/15/Unity; Git/CI cuối trong WORKLOG.
