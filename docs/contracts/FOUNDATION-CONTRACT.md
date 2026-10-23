# Foundation contract — điểm bắt đầu triển khai

Nguồn chi tiết: Modules 26, 27, 29. Tài liệu này thống nhất cách đọc và thứ tự xây, không thay phần thuật toán trong module. Xem [Ownership](OWNERSHIP.md), [Decision Register](../governance/DECISIONS.csv), [Open Decisions](../governance/OPEN-DECISIONS.csv).

## 1. Identity, versions và time

`EntityId` định danh instance đã tồn tại trong world; `DefinitionId` định danh authored content. Không dùng display name hoặc ID tự tạo từ wall-clock làm identity replay. Entity ID allocator thuộc transaction; rollback không được để authoritative state đi tiếp với một allocation chỉ tồn tại nửa chừng. Codec identity-v1 và allocation vectors đã có trong [Identity Spec](IDENTITY-SPEC.md); C# primitive implementation và headless evidence đã có ở Sprint1.

`WorldTick` là integer time authoritative; `WorldRevision` tăng khi commit. Tick unit prototype là một phút; lịch 360 ngày/năm theo [Time/Scheduler Spec](TIME-SCHEDULER-SPEC.md). FND-03 hiện thực và kiểm conversion/range/boundaries. Render frame time không làm world tiến lên nếu không có time command/scheduler step.

Manifest của một save/run phải đủ xác định schema version, content pack IDs/hashes, rule version, RNG algorithm version, initial state/seed và compatibility policy. “Cùng seed” một mình không đảm bảo replay nếu content, rules hoặc ordering đã đổi.

## 2. Command và result

Envelope tối thiểu theo M29 §44:

| Field | Semantic |
|---|---|
| command_id | Idempotency key cho cùng logical action/retry |
| command_type | Registry route tới typed handler |
| issuer_id | Actor/system issuer hợp lệ |
| source_type | Player/NPC/Scheduler; không bỏ qua rule vì là system |
| world_tick | Tick yêu cầu; validate policy trước xử lý |
| expected_revision | Bắt stale state theo command contract |
| payload_version / payload | Typed, versioned; reject unknown/incompatible fields theo codec policy |
| correlation_id | Toàn chuỗi action/workflow |

Cùng ID và cùng canonical payload: trả receipt đã lưu. Cùng ID khác payload: `CMD_PAYLOAD_MISMATCH`, không execute lại. Stale input phải reject/revalidate rõ ràng, không âm thầm dùng kết quả preview cũ.

Handler trả `Rejected`, `ProposedChangeSet` hoặc `ScheduledPlan`. Không trả SQL string/script/narrative text như source of truth. Gameplay rejection khác system failure: thiếu vật liệu là rejection; invariant violation/persistence error phải dừng publish và ghi diagnostic.

## 3. Transaction và event semantics

1. Nhận single-writer slot và committed revision.
2. Kiểm envelope, actor capability/authority, references, resources và hard constraints.
3. Evaluate rules trên read-only snapshot; RNG context được định danh trước draw.
4. Stage owned-field mutations, affected job changes, ID allocations, RNG state và event candidates.
5. Kiểm cross-domain invariants, expected revision và resource conservation.
6. Commit atomic: state, scheduled jobs, RNG, committed events, receipt, revision.
7. Chỉ sau durable commit mới publish in-memory state và notifications.

Mọi lỗi trước commit không thay authoritative state. Failed persistence không được biến thành “nhân vật xui xẻo”; retry policy phải trả kết quả có thể giải thích. Side effects chưa committed không được đến projection consumers.

Domain Event diễn tả việc **đã xảy ra**. Có event ID, event type, aggregate ID, world time, commit sequence, causation ID, correlation ID, rule version, payload/schema version. Một proposed event trong ChangeSet chưa được emit như fact. Consumers idempotent theo event identity/version; follow-up được xếp lịch có thứ tự ổn định và có giới hạn fan-out/cycle detection.

## 4. RNG, numbers và replay

RNG interface không có global singleton. Context gồm world seed, algorithm version, domain, decision type, occurrence ID, entity ID, draw index và stream ID nếu cần. UI, preview, search, log formatting không draw stream gameplay. Retries không được reroll cho đến khi có kết quả mong muốn.

PCG32 trong nguồn là ứng viên cũ; ADR-008 chọn sha256-counter-v1 cho prototype, cùng codec mdc1 và bounded rejection sampling trong [Math/RNG Spec](DETERMINISTIC-MATH-RNG-SPEC.md). FND-04 đã đối chiếu C# vectors trong Sprint1; Unity còn pending. Không viết `System.Random` rồi gọi là cross-runtime deterministic.

Numeric rules có rounding/clamping/order rõ. Critical formulas cần integer/fixed-point hoặc một policy deterministic được chứng minh. Mọi coefficient prototype nằm trong versioned parameters, không rải hardcode trong content. Kiểm tra M27 trước khi đổi scale.

Command replay = chạy lại commands từ initial state với đúng versions rồi so state hashes/event ordering. Event inspection = đọc journal cũ. Hai thao tác không chứng minh cùng điều. FND-13 phải rerun resolution, không chỉ in lại journal.

## 5. Scheduler và save/recovery

ScheduledJob có identity, due tick, order keys, command/rule reference, payload version, causation và lifecycle. Quy tắc tie-break phải deterministic. Time skip xử lý breakpoint/jobs trong khoảng, không chỉ nhảy clock rồi trừ resources một lần. Actor chết trước job completion: job revalidate/cancel theo rule, không cho người chết hoàn tất lao động.

Storage adapter chịu atomic transaction. Save snapshot là một consistent revision với manifest và checksum; recovery restore rồi replay phần tương thích. Cần test crash trước/giữa/sau commit, duplicate delivery, missing content/rule và corrupted snapshot. Migration có source/target version, backup và failure policy; chưa cam kết save compatibility giữa các prototype.

## 6. Read models và knowledge

World rules có thể đọc physical truth cần cho resolution. Actor decision dùng PerceivedState; query API cho UI/search/filter/notification bị giới hạn theo actor từ đầu. Knowledge entry ghi claim/fact, nguồn, thời điểm, confidence và disclosure/permission phù hợp. Tránh DTO chứa `hiddenTruth` rồi hy vọng UI không dùng.

Succession: archive của player có thể giữ trải nghiệm; character mới không tự có secret, skill, quyền sử dụng hoặc chứng cứ đối thoại của nhân vật cũ. Chỉ transfer có nguồn hợp lệ mới cập nhật knowledge/asset owner.

## 7. Contract change và definition of done

Mỗi task ghi owned state, allowed commands, emitted/consumed events, read dependencies, invariants, failure modes và persistence requirements. Breaking changes cập nhật ADR, schema/contract version, fixtures và replay expectations trong cùng thay đổi.

Foundation hoàn thành khi FND-02–16 có evidence phù hợp. Skeleton build, placeholder host hoặc một bản JSON có đủ field không thay cho transaction/replay/Unity tests. [QA Plan](../qa/QA-PLAN.md) chỉ rõ hiện đã chạy gì và runtime tests nào chưa có.

## 8. Đặc tả áp dụng bổ sung

[Identity](IDENTITY-SPEC.md), [Time/Scheduler](TIME-SCHEDULER-SPEC.md), [Math/RNG/Codec](DETERMINISTIC-MATH-RNG-SPEC.md), [Command/Activity Catalog](COMMAND-ACTIVITY-CATALOG.md), [Persistence/Save](PERSISTENCE-SAVE-SPEC.md) cụ thể hóa contract này theo ADR-008. Accepted mutation receipts thuộc state commit; rejected/no-op receipts được persist riêng, không tăng world revision. Chi tiết fingerprint/retry/hash ở catalog. Không để mỗi domain tự chọn reserve/refund hoặc rounding khác nhau.

## Trạng thái Sprint2

[ADR011](../adr/011-in-memory-command-pipeline.md): path TransferResource đã hiện thực trong bộ nhớ; rule/snapshot/receipt/invariant contracts được kiểm bằng tests. Mục transaction durable, jobs/events đầy đủ và knowledge projections phía trên vẫn là mục tiêu, chưa nghiệmthu. Xem [evidence](../qa/SPRINT-02-EVIDENCE.md).

## Sprint3 event-v1

[ADR012](../adr/012-committed-event-model.md) cụ thể hóa envelope/journal cho TransferResource và consumer projection/checkpoint in-memory. Causation trỏ command; correlation chỉ phục vụ trusted trace. Fan-out/cycle scheduling và durable publication vẫn pending FND-10/11; consumer này không thay knowledge projection hoặc gameplay reward/rumor.

## Sprint4 — jobs và workflow trong publication

[ADR013](../adr/013-in-memory-scheduler.md): CommittedWorld chứa SchedulerState và journal chung. Mỗi job/internal command publication atomic với clock, identity, state, receipt và events; lỗi trước publication giữ last committed state. Durability/persistence và restore interrupted workflow vẫn pending FND11/12. Fan-out/cycle guards áp dụng approved transfer continuations; chưa là gameplay event template engine hoàn chỉnh.

## Sprint5 — headless durable publication

[ADR014](../adr/014-headless-sqlite-persistence.md) đặt ICommittedWorldStore trước mọi publication, gồm metadata và scheduler. SQLite adapter commit full current image/mirrors atomically; lỗi store buộc reopen trước xử lý tiếp. Full periodic save/recovery/replay và Unity provider chưa xong; không coi current-image restore là toàn Foundation ready.

## Replay QA Sprint7

[ADR016](../adr/016-headless-command-replay.md) hiện thực FND13 headless transfer/scheduler replay: initial image + explicit manifest + ordered Execute/Drain attempts. Không dựng từ receipts hoặc apply saved after-images. Result/gameplay/integrity/journal/jobs hashes bind manifest; first divergence có action/category/revisions. Golden cố định chạy subprocess; Sprint9 bổ sung diagnostics profile FND15 theo ADR018; Unity và stochastic gameplay còn gates riêng.

## Diagnostics không có authority — Sprint9

[ADR018](../adr/018-foundation-diagnostics.md) bổ sung RuleResult.Checks từ actual TransferResourceRule evaluation, QA replay observer và Infrastructure state/causal/job inspectors. Checks không persist vào state/receipt/journal hoặc thay hash. Chỉ các điều kiện đã chạy được ghi, proposal khác commit. Omniscient diagnostics không được đưa vào player knowledge projection. Replay-v1 hashes không đủ suy field diff, evidence package có expected image riêng và baseline kiểm lại trước fault.

## Cập nhật Sprint10 — 2026-10-23

Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. Xem [ADR019](../adr/019-unity-editor-conformance.md) và báo cáo Sprint10. Các đoạn mô tả sprint trước giữ phạm vi evidence tại thời điểm đó.
