# Time & Scheduler Specification — clock-v1

Áp dụng FND-03/07/10, TEST-008/028/033/038. Nguồn M26 §74–77 và M29 §21–28; [ADR-008](../adr/008-prototype-contract-baseline.md) giải quyết ordering khác nhau trong ví dụ nghiên cứu.

## Tick và calendar

`WorldTick` là Int64 trong [0, 9223372036854775807]. Một tick = một phút simulation; tick 0 là đầu ngày 1 tháng 1 năm 1 của campaign. Lịch simulation có 12 tháng × 30 ngày, 24 giờ/ngày, 60 phút/giờ. Đây là lịch tính toán hư cấu, không khẳng định niên đại/lịch sử. UI có thể trình bày mùa và mốc truyện từ metadata, không đổi số tick đã lưu.

`minute=t%60; hour=(t/60)%24; day=(t/1440)%30+1; month=(t/43200)%12+1; year=t/518400+1`, phép chia nguyên không âm. Tick 1439 → ngày1 23:59; 1440 → ngày2 00:00; 43199 → tháng1 ngày30 23:59; 43200 → tháng2 ngày1; 518400 → năm2. Cộng duration phải checked; âm hoặc overflow reject `TIME_RANGE`. Duration công việc tối thiểu 1 tick; instant command không đổi tick. Clock không tự tiến vì render frame/wall-clock.

## Ordering và cùng tick

Total order `(DueTick ASC, Phase ASC, Priority ASC, CreationSequence ASC, JobId ASC)`. Sequence UInt64 cấp staged toàn world, persisted, không tái dùng sau commit; hết range thì reject. Không thêm StableEntityId tie-break như ví dụ M26. Priority Int32: số nhỏ chạy trước; mặc định 100. Phase cố định:

| Code | Phase | Ví dụ |
|---|---|---|
| 0 | Boundary | ranh ngày/mùa đã có lịch |
| 1 | DueObligation | deadline, nghĩa vụ |
| 2 | Environment | lũ/nước, vật liệu bị hủy |
| 3 | ProductionHealth | completion, điều trị, death resolution |
| 4 | ActorDecisions | lựa chọn NPC trên perceived state |
| 5 | InformationNarrative | truyền tin, story observation |
| 6 | ValidationProjection | kiểm tra và invalidation projection |

Phase sắp lịch cho chính tick đang chạy chỉ hợp lệ nếu phase mới **lớn hơn** phase hiện tại. Cùng/nhỏ hơn phải đổi DueTick thành current+1 trước commit và ghi `deferred_from_tick` để trace. Job quá khứ bị reject, không âm thầm chạy trễ. Jobs đã có cùng tick/phase xếp theo priority/sequence/ID. Ví dụ phase2 hủy thuyền và phase3 sửa xong cùng tick: completion nhìn thấy thuyền đã hủy, cancel, không hồi sinh thuyền.

## Advance, skip và recovery

`AdvanceTime(target_tick)` là workflow: ghi target rồi xử lý lần lượt các job due ≤ target. Mỗi job là internal command riêng, atomic; không giữ một SQL transaction qua toàn bộ time skip. Sau job cuối, commit clock tới target. Nếu crash giữa skip, save chứa tick/job cursor/target và các receipt đã commit; resume không chạy lại side effects. Request envelope dùng revision lúc nhận; internal commands nhận revision hiện tại, không reuse expected_revision cũ.

Prototype budget mỗi invocation: 1.000 committed jobs, tối đa 32 follow-up jobs từ một command. Budget hết thì trả `PausedBudget` cùng current tick, remaining target, cursor; không drop job hay tự nhảy tới target. Actor death, player decision, combat start và historical blocking condition tạo breakpoint `PausedDecision`; chỉ resume sau command giải quyết hợp lệ. No-job advance vẫn commit revision và clock. Target < current reject; target = current không có due jobs là accepted no-op receipt.

Mọi job revalidate actor alive, resource reservation, authority và location lúc thực thi. Actor chết làm activity cancel qua cùng pipeline, giải phóng phần reserve còn tồn tại; scheduler không giả completion thành công. Tick tiến đơn điệu; nhiều commits cùng tick hợp lệ. Fault injection phải dừng/resume ở từng phase, kiểm ordering không đổi khi dictionary insertion đảo thứ tự.

## Commit workflow cụ thể

Advance có jobs: commit yêu cầu/target (revision+1, giữ tick); mỗi due job commit tick=DueTick và effects trong một transaction; cuối workflow commit tick=target và clear cursor (revision+1). Dù job cuối đúng target vẫn có closing commit để kết thúc workflow. Request receipt ban đầu outcome AcceptedPending trỏ workflow; terminal workflow status là dữ liệu riêng, không sửa fingerprint/receipt cũ. Retry cùng request trả receipt gốc và có thể query progress. Trong kernel smoke một job tương ứng ba commits advance: request, completion, close. Bảng expected outcome Accepted của scenario chỉ trạng thái workflow đã hoàn tất khi harness drain; không phải response đồng bộ nhận ban đầu. No-job advance chỉ một commit clock+terminal receipt; target=current no-job là no-op metadata receipt.
