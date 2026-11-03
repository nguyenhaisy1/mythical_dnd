# Sprint14 — phản biện độc lập Domain, command và scheduler

03/11/2026 · SPIN-038 · actor `/root/s14_critic_commands`.

## Kết luận và phạm vi

Qua đọc implementation thực tế đối chiếu ADR023 và `docs/contracts/BOAT-REPAIR-PROFILE.md`, chưa phát hiện blocker mới trong phần Domain/command/scheduler đã review. Đây là kết luận source review, chưa tự là nghiệm thu runtime. Critic không viết production implementation, không sửa golden, không chạy build/test đồng thời với điều phối viên.

Đã áp dụng `mythical-validation` và `mythical-kernel`; đọc baseline `.agent`, HANDOFF và APPLICABLE-BASELINE. Phần lưu trữ/recovery/replay và lỗi causality ban đầu thuộc báo cáo critic storage riêng. Report này kiểm lại integration của history guard sau sửa, không nhận công phát hiện lỗi đó.

## Những đường đã đối chiếu

| Yêu cầu | Quan sát trong source và test thực |
|---|---|
| Auth, duplicate, stale | `ExecuteLocked` xác thực principal độc lập trước trả receipt; cùng ID/fingerprint lấy receipt cũ trước stale; mismatch không chạy lại. `scheduler/` bị chặn ingress, public `CompleteRepair` không thể giả completion. |
| Atomic start | Identity stage chưa publish; pure evaluator chỉ propose. Application kiểm target, worksite/tool/recipe/time/cause và hiệu ứng chính xác trước publication. Reserve tăng, OnHand giữ; receipt/event/job/activity cùng publication. |
| Job và cancellation | Tagged repair job không dùng dummy transfer; pairing kiểm activity/job/actor/due/phase/priority/cause/correlation/status/lifecycle. `CancelJob` từ chối repair job; `CancelActivity` release đúng hold, retire cả hai và được phép khi Advance đang mở. |
| Atomic completion | Input ở due tick chưa publish riêng. Completion nội bộ recheck eligibility và pins; proposal validator kiểm item/capability/activity khác không đổi, lot delta chính xác, condition gain/cap và hold. Publication chứa tick/effects/receipt/facts/retirement. |
| Revalidation và material | Actor chết, mất qualification/quyền, tool đổi/hỏng, vị trí hoặc boat không hợp lệ dẫn cancellation với reason. Cancel không cộng lại OnHand; sum repair holds không vượt Reserved và không xóa reserve của domain khác. |
| Giữ state và RNG | `WorldSnapshot` kiểm extension immutable/references/lifecycle; transfer và scheduler clone carry `Repair`. Domain không có RNG/wall-clock/Unity/IO; tests kiểm draw index không thay đổi. |
| History guard sau sửa | Start phải có accepted receipt và `RepairStarted` khớp recipe/hash/refs/due. Terminal có receipt/facts/lifecycle tương ứng. Correlation facts khớp terminal command của chính nó; internal completion vẫn dùng correlation pinned từ start. Cancel mới được phép dùng correlation mới. |
| Quyền ghi truth | `InspectCommitted` được ghi rõ trusted diagnostics/composition; source đã review không thêm player-facing projection hoặc UI ghi authoritative state. Không suy việc này là nghiệm thu knowledge isolation của client chưa tồn tại. |

Đã đọc các test `BoatRepairRuleTests`, `BoatRepairCommandTests`, `BoatRepairAdversarialTests` và `BoatRepairCausalityTests`, gồm before-due, cap, reject/auth/payload, retry/response loss, precommit failure, paused mixed-job Advance/cancel, competing actor/tool, reservation của domain khác, forged proposals và receipt/fact causality.

## Hai ca kiểm chứng độc lập bổ sung

File `tests/MythicalDnd.Foundation.Tests/BoatRepairIndependentTests.cs` do critic này viết sau thông báo cho `/root`, source đã đóng băng trước full run:

1. Tạo Running bằng accepted command thật, rồi dùng trusted fixture chuyển đồng thời actor/boat/tool/material tới settlement khác: completion vẫn phải cancel `LOCATION_MISMATCH` vì worksite ban đầu đã pin; không consume/gain, chỉ release hold.
2. Tạo Running thật, cấp thêm tool mới đang usable và đổi capability sang tool đó: activity cũ phải cancel `ITEM_UNAVAILABLE`, không tự thay tool đã pin; không consume/gain hoặc làm hỏng replacement.

Hai input biến đổi chỉ kiểm Domain revalidation trên trusted fixture; không chứng minh có Travel/tool-switch/Health/rights commands. Các ca mới chưa được critic tự thực thi; kết quả run do `/root` cung cấp và được ghi tại `docs/qa/SPRINT-14-EVIDENCE.md` khi có bằng chứng.

## Dấu vân tay source đã review

SHA256 tại thời điểm chốt source review:

```text
4984f08fccf733d2189ffbb7c7effc01662b05dc1dfe2177a4bb375ad4863989  engine/Domain/Repair/RepairState.cs
049bf969c5741080cee9d313ddb0d55f961e0e07f3e956b9dac77dc2865a785f  engine/Domain/Repair/BoatRepairRules.cs
e985eeacf749e2d3828a08b52c71cdd8dc0fc1b3d9022e872d263aead710ceb1  engine/Domain/Kernel/WorldSnapshot.cs
03faa62da7d753ac70a441a62020c9f4a46b31a73fdab6e53744d109d9217262  engine/Application/CommandPipeline/CommandProcessor.cs
c79c4276af6cf58d27ed4aad5a650ac3a3e110a0fa72b3c1602668aec5380ea7  engine/Application/CommandPipeline/CommandProcessor.Repair.cs
08fc8ffb1817249bf571012018e99c681a175c46ce7a6a49fb83d100fcb18f1e  engine/Application/CommandPipeline/CommandProcessor.RepairHistory.cs
be4f0c4f8219504574562cc5e5f3befb00bc27d9bf0e6bb67ae3b473f1fa1d8d  engine/Application/SimulationScheduler/CommandProcessor.Scheduler.cs
22f8989733946a2e689af57afd27e157ceb0216508a4a9cd9c5567ede07196cd  engine/Application/SimulationScheduler/SchedulerState.cs
8d86409b449ba1101d52a9d7218a0804ebf839a3eae6450529eef076cdf5b386  tests/MythicalDnd.Foundation.Tests/BoatRepairIndependentTests.cs
```

Nếu production source thay đổi sau fingerprint này thì phần kết luận tương ứng phải review lại. Không có kết luận Unity/Player/2D-sprite, full profession/life generator, full VS-03, performance hoặc human playtest; những gate đó ngoài Sprint14 headless profile.

## Disposition bổ sung bởi root — 03/11/2026

Report phía trên giữ kết luận và evidence tại lượt actor ban đầu; các câu chờ full run là trạng thái lịch sử. Root đã đối chiếu full gate562/562 Passed (113 repair),0failed/skipped và Host/process gates ở revision cuối; [evidence Sprint14](../SPRINT-14-EVIDENCE.md) và [final review/disposition](FINAL-REVIEW.md) là nguồn nghiệm thu hiện hành. Shared history source có bản siết workflow sau fingerprint của critic commands; phần sửa đó được critic cuối review độc lập. Không nhận root đối chiếu artifacts là việc critic ban đầu tự thực hiện.
