# ADR-013 — Scheduler và AdvanceTime trong bộ nhớ

Ngày 2026-10-16; áp dụng prototype Sprint4/FND-10. Nguồn chuẩn: Time Scheduler Spec clock-v1, Command Activity Catalog kernel-v1 và Foundation Contract. Bổ sung API cho kernel hiện có; không sửa archive/GDD nghiên cứu.

## Bối cảnh và giới hạn

FND-10 có tên Durable scheduler nhưng FND-11 mới cung cấp storage. Sprint4 hiện thực trạng thái/thuật toán và atomic in-process publication để persistence adapter có boundary rõ. Không nghiệm thu process-crash, restore từ disk hoặc SQLite. TransferResource là command hiện có đủ nhỏ để kiểm deferred execution; không giả đã có repair, Health death, inventory reservation activity hay combat.

## Ownership và API

Application sở hữu immutable scheduler snapshot gồm jobs, staged creation sequence, workflow target/status/cursor và execution trace. Snapshot này nằm trong CommittedWorld cùng WorldSnapshot, receipts và journal; không có clock/world mutable riêng. Sequence/job ID không tiêu thụ khi staging thất bại.

Commands prototype bổ sung payload_version=1:

| Command | Payload | Kết quả |
|---|---|---|
| ScheduleTransfer | due_tick, phase, priority, source_lot_id, destination_lot_id, quantity; optional handler_key | Tạo scheduled transfer; identity và sequence staged |
| CancelJob | job_id | Chủ job hủy pending job; không tạo/hoàn vật chất |
| AdvanceTime | target_tick | Request pending hoặc no-job clock advance/no-op |
| ResolveAdvanceDecision | workflow_id | Chủ workflow acknowledge breakpoint prototype, cho phép drain tiếp |

Các command vẫn đi qua fingerprint/duplicate-before-stale, trusted principal, time/revision, payload/version và authority checks. Internal command identity có namespace riêng bị chặn ở external ingress trước khi lưu receipt, tránh client tạo rejection receipt chiếm ID của scheduler.

`DrainAdvance` là host workflow có owner principal và budget, không nhận quyền từ source_type wire. AcceptedPending request receipt bất biến; trạng thái kết thúc nằm ở workflow query. Lúc workflow chưa kết thúc, chặn command external mới TransferResource/ScheduleTransfer/AdvanceTime; vẫn cho duplicate request trả receipt gốc, CancelJob có quyền và ResolveAdvanceDecision đúng owner/status. Không cho thay workflow bằng request mới giữa skip.

## Ordering, revalidation và atomic commit

Giữ total order `(DueTick,Phase,Priority,CreationSequence,JobId)` từ clock-v1. Collection insertion order không thay kết quả khi các key đã cấp giống nhau. Phase nhỏ/hòa của followup tại tick đang chạy defer tới tick+1 và giữ deferred_from_tick; phase lớn hơn chạy tại tick hiện tại. Job quá khứ bị reject; checked overflow không wrap.

Request có jobs commit revision+1/target giữ tick; mỗi job commit tick=due cùng effects/status/events/receipt/identity; cuối workflow có closing commit ngay cả khi job cuối đúng target. No-job advance chỉ một commit; target=current không due là no-op metadata.

Transfer job đọc world hiện tại và revalidate liveness, authority, commodity, available/reserved và lot location theo rule/invariant đã có. Không thêm yêu cầu actor location mà catalog TransferResource chưa quy định. Rule rejection dẫn cancellation/result với reason và không thay quantity. System exception trước commit không trở thành terminal rejection; job vẫn sẵn sàng retry. Sau publication, mất response không rollback hoặc execute lại.

## Budget, followups và breakpoint

Mỗi invocation tối đa1000 committed jobs, mỗi command tối đa32 followups. Hết budget giữ target/queue, pause và resume được; không bỏ job hay nhảy clock tới target. Approved host handlers chỉ đề xuất immutable followups/breakpoint, không ghi world. Followups được kiểm và stage cùng parent commit; invalid proposal không được publish một phần.

Cycle detection dựa causal ancestry của reaction key ổn định, không dùng global visited để chặn mọi sự kiện lặp hợp lệ độc lập. Reaction key là canonical hash của handler key, actor, source lot, destination lot, quantity; không gồm due tick/phase. Tối đa64 ancestor keys. Cùng reaction lặp trong cùng causal chain bị chặn ngay cả khi đổi tick; đây là policy prototype, legitimate recurrence cần contract riêng. Jobs giữ ancestry keys và causation command, journal giữ scheduled_by/workflow_id để trace. Handler key phải có trong approved compiled registry, mặc định transfer-v1.

Breakpoint prototype chỉ là yêu cầu explicit acknowledgement đã được compiled handler chấp thuận. Drain không tự xóa PausedDecision; ResolveAdvanceDecision kiểm workflow/owner/status. Đây không phải implementation giải quyết player death/combat/historical blocker. Những domain đó phải cung cấp command giải quyết hợp lệ trước khi nghiệm thu VS-06.

## Evidence và compatibility

Headless tests kiểm ordering, deferral, budgets/fanout/cycle, authorization, range/overflow, immutable snapshots, cancellation/revalidation, retries và atomic fault boundaries. Validated initial scheduler snapshot phục vụ fixture có actor dead/rights thay đổi không phải save-load implementation hay gameplay Health command.

Không đổi identity-v1, clock-v1, RNG hoặc transfer quantity rules. Scheduler/publication DTO chưa là released save format. Đồng bộ consumers: Application, tests/CLI/runner/CI, Time/Command/Foundation contracts, QA matrix/backlog, architecture/handoff/agent/runbooks và exports. Full TEST028/033 domain scenarios và durability vẫn có gate riêng.

## Hiệu chỉnh từ review — 2026-10-19

Chỉ lỗi parse/admission được chuyển thành rejection; exception từ handler hoặc trước/sau publication phải propagate như system failure. Mất response sau commit không được tạo metadata receipt từ chối cho command đã accepted. Internal job command dùng actor/correlation của job; workflow_id liên kết về advance workflow riêng. Scheduler events dùng scheduler-v1, transfer events giữ transfer-resource-v1; journal chung giữ eventIndex theo thứ tự trong từng commit.
