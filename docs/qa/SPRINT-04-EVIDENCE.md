# Sprint 4 — nghiệm thu scheduler và AdvanceTime in-memory

Mở2026-10-16, nghiệm thu2026-10-19 sau lần gián đoạn usage limit; SPRINT-04/SPIN-007. **Đạt scope FND-10 phần transfer scheduler/AdvanceTime trong bộ nhớ**. Tên backlog Durable scheduler không có nghĩa storage/recovery đã được nghiệm thu.

## Phân công và review

/root điều phối, ADR013/docs/CLI/runner/Git và nghiệm thu. /root/s4_scheduler hiện thực Application models/pipeline; /root/s4_tests viết tests riêng. /root/s4_review review nguồn độc lập, root cũng kiểm integration. Hai findings đã sửa: catch quá rộng có thể tạo rejection sau commit thật; internal job correlation không thống nhất với job. Regression bổ sung xác minh trước/sau publication ArgumentException và job correlation khác advance correlation. Root final contract check cũng sửa completed workflow để clear cursor, giữ execution history; regression kiểm chính xác. Final reviewer không còn blocker, không tự chạy suite riêng; root chạy final gate dưới đây.

## Runtime evidence

Lệnh `python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`.

- SDK10.0.401/Linux x64; locked restore, Release build6projects,0warning/0error.
- **184discovered/184passed/0failed/0skipped**:138 regression +46 scheduler cases. Runner nhóm FND-10 có9cases, FND-10/workflow có37cases; prefix TEST_028 không tự chứng minh mọi domain của requirement đã xong.
- Negative control riêng1test failed/exit1 đúng mong đợi; toàn gateexit0.
- Source fingerprint `sha256:947ad738d68380bbf58643ea169314d4d18f040291aa65c32dfad6f5797c31f8`.
- Artifacts `tests/results/sprint04-dbhhrenr/evidence.json`,sources.json,TRX và restore/build/runtime/negative/transfer-demo/scheduler-demo logs. Generated artifacts ignored; report source này được track.
- Transfer demo vẫn7/3 và duplicate consumer chỉ apply1lần. Scheduler demo schedule2jobs tại10/20; advance30 → clock30,revision7,lots5/5,2executions,Completed,originalPendingReceipt retry unchanged.

## Acceptance thực thi

Ordering due/phase/priority/creation sequence được kiểm với shuffled restored insertion; immutable old snapshot không đổi. Job ID là tie-break cuối nhưng sequence unique trong world, duplicate sequence bị reject. Followup phase nhỏ/hòa được defer sang tick+1 và giữ trace; higher phase cùng tick hợp lệ. Past/negative/max-tick overflow, sequence exhaustion và reserved internal namespace đều có tests.

Advance no-job một clock commit; cùng tick no-job metadata no-op. Có jobs: request pending, từng job commit riêng, closing commit kể cả job cuối tại target. Retry đi trước stale; fingerprint mismatch không poison original; source spoof/auth khác owner bị chặn. Active workflow chặn mutation mới ngoài cancel/resolve; PausedDecision không tự bỏ qua bởi DrainAdvance.

1001jobs fixture: drain đầu dừng sau1000, không drop job/nhảy clock; drain sau hoàn tất. Cancellation terminal không chuyển tài nguyên. Transfer job revalidate committed actor/authority/lot location/available−reserved; dead actor fixture không được hành động. Fanout vượt32, reaction cycle, past followup và depth64 bị chặn atomically, không publish một phần. Depth test chạy64jobs/2080units rồi dừng ở giới hạn, retry lỗi không đổi publication.

Fault trước request/job/close giữ last committed state; mất response sau commit không chạy lại side effects. Scheduler/clock/identity/receipts/events cùng immutable aggregate; reentrant drain bị chặn. Unexpected ArgumentException không bị lưu thành terminal rejection. Job events dùng actor/correlation của job và workflow_id riêng; internal namespace không bị external command chiếm receipt.

## Giới hạn và readiness

- Không SQLite/disk durability, process-crash recovery hoặc restore interrupted workflow. SchedulerState.Restore chỉ validated pending-job fixture; không khôi phục receipts/history/workflow từ save.
- Transfer job không là repair/activity implementation. Dead-actor fixture chứng minh revalidation, chưa có Health death/party/repair cancellation integration. Reserved counter không thay activity reservation lifecycle.
- Breakpoint là compiled continuation yêu cầu owner acknowledgement; chưa là giải quyết illness/death/combat/historical/legacy. TEST033 full VS06 còn pending.
- Fanout/cycle/depth áp dụng approved transfer continuations, chưa là content event template engine. Pure handlers không có quyền external side effect/world mutation. Full canonical replay FND13 chưa có.
- State/journal/receipts/jobs/history giữ trong bộ nhớ và copy publication; chưa có retention/performance benchmark. No Unity/player UI/playtest acceptance.

Bước tiếp theo FND11 provider spike/persistence adapter và durable atomic commit; FND12 recovery/save và FND13 replay phụ thuộc sau. Không tự mở Sprint5 chỉ vì Sprint4 hoàn tất.

## Documentation và Git

[ADR013](../adr/013-in-memory-scheduler.md), [Sprint4](../production/sprints/SPRINT-04.md), Time/Command/Foundation contracts, backlog/matrix, README/architecture/handoff/agent, workflow thường trực, runner/CI/changelog và export inputs được đồng bộ. Checks/source freshness/Git SHA/PR/CI cuối ghi ở [WORKLOG](../../WORKLOG.md). Local runtime evidence không thay remote CI proof; main/tag/release giữ ngoài scope.
