# Command & Activity Catalog — kernel-v1

Áp dụng FND-07/08/09/10 và VS-03. Envelope và pipeline ở [Foundation Contract](FOUNDATION-CONTRACT.md); [time](TIME-SCHEDULER-SPEC.md), [identity](IDENTITY-SPEC.md), [numeric](DETERMINISTIC-MATH-RNG-SPEC.md) là contract bắt buộc. Chỉ catalog phạm vi kernel/repair; vận tải, điều trị và game đầy đủ có cổng authoring riêng.

## Quy tắc chung

Commands typed, payload_version=1; unknown fields/type/version reject. issuer/source có quyền được Application xác thực, không tin `source_type=Scheduler` từ UI. world_tick phải bằng committed tick khi nhận command thông thường. expected_revision phải bằng revision hiện tại; duplicate check đi trước stale check. Fingerprint gồm tất cả envelope trừ transport metadata; retry phải gửi đúng envelope ban đầu. Cùng command_id khác fingerprint → CMD_PAYLOAD_MISMATCH. Client muốn thay input phải phát command_id mới.

Accepted mutation tăng revision đúng 1, có ordered events và receipt atomic. Accepted no-op/rejected command lưu receipt trong transaction metadata riêng, không tăng world revision, không consume RNG/ID và không phát domain event; receipt metadata bị loại khỏi gameplay snapshot projection, nhưng hash riêng trong save integrity manifest. **Chỉ receipts của accepted mutation nằm trong canonical state**. Điều này làm rõ “command receipts” của numeric spec; rejection registry vẫn phải restore để giữ idempotency. Malformed envelope không đủ valid ID không persisted, chỉ diagnostic. System failure không ghi terminal rejection; có thể retry cùng ID. Không gọi infrastructure error là gameplay outcome.

Errors cố định: CMD_PAYLOAD_MISMATCH, STALE_REVISION, TIME_MISMATCH, REF_NOT_FOUND, REF_TYPE_MISMATCH, ACTOR_DEAD, NOT_AUTHORIZED, LOCATION_MISMATCH, INSUFFICIENT_RESOURCE, ITEM_UNAVAILABLE, ACTIVITY_STATE, RESERVATION_LOST, NUMERIC_OVERFLOW. UI nhận localized code + visible details; không lộ hidden owner/location khi caller không có quyền biết. Internal diagnostics có trace riêng.

## Catalog

| Command / payload required | Điều kiện | Proposed changes và events |
|---|---|---|
| TransferResource(source_lot_id,destination_lot_id,quantity) | quantity>0; cùng commodity; issuer điều khiển source; hai lot cùng location; available≥quantity | Assets giảm source, tăng destination; không đổi tổng; ResourceTransferred |
| TransferItemRights(item_id,new_owner_id) | owner hiện cho phép; recipient tồn tại; item không có activity/reservation | Assets đổi owner; ItemRightsTransferred; không tự đổi knowledge |
| StartRepair(actor_id,item_id,material_lot_id,recipe_id) | actor=issuer alive, đủ nghề/tool, cùng nơi; có quyền sửa; item damaged không destroyed; available đủ | Assets reserve 2 timber units; Economy tạo activity Running; Application job CompleteRepair ở tick+60 phase3; RepairStarted |
| CompleteRepair(activity_id) | internal scheduler với causation job hợp lệ; actor alive/cùng nơi, item còn, quyền còn, reservation nguyên vẹn | consume reserved2, condition +200 max1000; activity Completed; job Retired; RepairCompleted |
| CancelActivity(activity_id,reason) | issuer điều khiển actor hoặc system xử lý death/invalid reservation | release remaining reservation; activity Cancelled; job Retired; ActivityCancelled |
| AdvanceTime(target_tick) | target≥current, không unresolved player decision | workflow theo Time Spec; jobs đi qua commands; TimeAdvanceCompleted/Paused receipt |

Prototype repair recipe deterministic; chưa roll 2d10 trong fixture để tách test transaction khỏi balance. Add stochastic quality sau phải version recipe và golden. `quantity`, `condition`, `duration` không nhận tùy ý từ payload client: tra recipe/parameter pack version.

## Activity state machine và ownership

`Absent → Running → Completed | Cancelled | Failed`; terminal không quay lại Running. Cancel sau Completed trả ACTIVITY_STATE; retry cùng command_id lấy receipt cũ. Start reserve không consume, available=on_hand−reserved. On_hand bao gồm reserved. Mỗi reservation có activity_id, lot_id, quantity; tổng reserve≤on_hand. Complete consume on_hand và xóa reservation trong một commit. Cancel release không cộng on_hand (vật liệu chưa bị consume).

Resource destruction phải ghi inventory loss và giảm/hủy reservations cùng transaction; activity bị ảnh hưởng được Failed, job cancelled, reason RESERVATION_LOST. Không tạo lại vật liệu khi cancel. Actor death trước completion: cancel/release tồn tại, không cộng condition. Rights transfer trên item đang repair bị ITEM_UNAVAILABLE; rule khác đổi quyền cưỡng chế phải invalidate activity trong cùng commit. Tool mất/location thay đổi trước completion cũng cancel với lý do cụ thể, không tự teletransport actor.

ChangeSet field paths allowlist: Assets `lots.on_hand`, `lots.reservations`, `items.owner_id`, `items.condition`; Economy `activities.status/actor_id/item_id/recipe_id`; Application `jobs`, receipt, revision; Foundation tick/allocator/RNG. Owner review theo [Ownership](OWNERSHIP.md); handlers không SQL hoặc viết field của owner khác mà không proposal được validate. Health death là input đã commit, repair không sửa Health.

## Receipt và acceptance

Receipt: command_id, fingerprint, outcome, error_code nullable, before_revision, after_revision, committed_tick, ordered_event_ids, activity/job IDs nếu visible. Không chứa omniscient snapshot. Transfer mất điện trước commit không đổi hai lot; sau commit trước response retry không chuyển thêm lần nữa. Start→cancel giữ on_hand ban đầu, hết reservations/jobs. Start→complete mất đúng2 timber và tăng condition đúng200. Replay từ snapshot+commands phải tính lại result, không chép expected.json vào state. Kernel smoke data định nghĩa cả positive và negative cases.

## Sprint 2 implementation — in-memory TransferResource

[ADR-011](../adr/011-in-memory-command-pipeline.md) quy định API/diagnostics của path TransferResource. WorldSnapshot/RuleRegistry/Application pipeline có tests; các command repair/rights/advance còn pending. Auth principal do trusted host cấp (Player/NPC/Scheduler đều không bypass rule), không deserialize từ envelope. Source/destination cùng lot là validated no-op; household control dùng explicit authoritative owner refs.

Prototype bổ sung INVALID_ENVELOPE/INVALID_PAYLOAD/COMMAND_UNKNOWN/PAYLOAD_VERSION_UNSUPPORTED/COMMODITY_MISMATCH/REF_NOT_ACTIVE. PROPOSAL_* và commit failures là system failures; không terminal rejection. Command_id/correlation token giới hạn ASCII alphanumeric đầu, tiếp theo alphanumeric hoặc `_ . : / -`, tối đa128ký tự; không dùng như entityID. Invalid commandID/auth rejection không persisted để tránh poisoning; valid authenticated input reject/noop lưu metadata. Public persistence schema/migration chưa được chốt từ DTO in-memory này.

[Evidence Sprint2](../qa/SPRINT-02-EVIDENCE.md) tách atomic in-process publication khỏi durability FND11. Minimal transfer event records chưa là fullcausal/eventjournal FND09.

## Sprint4 — scheduler prototype

[ADR013](../adr/013-in-memory-scheduler.md) thêm ScheduleTransfer/CancelJob/AdvanceTime/ResolveAdvanceDecision, payload_version1 và approved optional handler_key. DrainAdvance là trusted bounded host workflow. AcceptedPending request receipt giữ nguyên, query workflow để biết trạng thái; prefix scheduler/ bị chặn tại external ingress để tránh chiếm internal command ID. Khi workflow active chỉ cho retry, CancelJob và ResolveAdvanceDecision có quyền. StartRepair/CompleteRepair/CancelActivity/rights vẫn chưa implemented; không suy job cancellation là activity reservation release.
