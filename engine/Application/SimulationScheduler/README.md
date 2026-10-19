# SimulationScheduler — FND10 in-memory

SchedulerState là thành viên bất biến của CommittedWorld, gồm jobs/creation sequence/workflows/execution trace. Commands ScheduleTransfer,CancelJob,AdvanceTime,ResolveAdvanceDecision qua CommandProcessor; DrainAdvance xử lý tối đa1000jobs/invocation, từng job commit độc lập.

Ordering clock-v1; followup phase nhỏ/hòa defer sang tick+1. Approved pure continuations tối đa32followups, reaction-cycle guard và ancestry64. State/event/receipt/clock/job status cùng publication; pending receipt không đổi khi workflow hoàn tất.

Không storage/durable restore hoặc repair/health/legacy gameplay. SchedulerState.Restore chỉ nhận pending-job fixture, không khôi phục workflow/receipts từ save. Diagnostic snapshots không là API cho player. Chi tiết docs/adr/013-in-memory-scheduler.md và docs/qa/SPRINT-04-EVIDENCE.md.
