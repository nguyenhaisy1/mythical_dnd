# SPRINT-05 — Durable commit cho headless kernel

Mở2026-10-19, SPIN-008, branch feature/sprint-05-persistence từ dev9fdb1ef. User giao sprint theo workflow thường trực. Goal FND11 headless subset: SQLite atomic commit/reopen trên toàn CommittedWorld hiện có; không coi provider đã được kiểm Unity.

## Breakdown và actor thực

| Task | Assignment và thực tế | Acceptance |
|---|---|---|
| S5-01 provider/spec | /root | Pin provider, license/native assets, ADR014; OPEN03 giữ Unity gate |
| S5-02 port/codec | /root/s5_codec bắt đầu port/receipt retention rồi dừng usage limit; /root tiếp quản | Mọi publication qua durable port; mdc1 full state roundtrip/validation |
| S5-03 SQLite/tests | /root/s5_sqlite ban đầu bị usage limit; /root implementation, agent tiếp tục bổ sung14tests | Real file transactions, rollback/busy/full/CAS/unknown outcome |
| S5-04 review | /root/s5_review đã resume và review nguồn độc lập, không còn blocker | Ghi đúng phần review đã làm; không thay independent review bằng self-review |
| S5-05 crash/integration | /root | SIGKILL subprocess trước/sau COMMIT; reopen/retry đúng; regression/negative control |
| S5-06 docs/Git | /root | Đồng bộ nguồn/export/evidence; chỉ PR/dev sau gate/review đạt |

## Phạm vi và quyết định

[ADR014](../../adr/014-headless-sqlite-persistence.md) chốt provisional managed provider, whole current-image mdc1 + relational mirrors trong cùng transaction và UInt64 fixed16hex. Store phải CAS toàn image bao gồm metadata receipts; revision không tăng cho rejection nên CAS revision đơn thuần không đủ.

Input: immutable world/rules/events/scheduler của Sprint4. Output: file store mở lại được toàn committed state, receipt duplicate trước stale và pending/budget workflow tiếp tục được. Mọi lỗi store trước publication giữ in-memory state cũ và đặt NeedsRecovery; host tạo processor mới từ store.Load, không tiếp tục với stale snapshot.

## Acceptance và non-goals

File-backed rollback fault tại tables/COMMIT, actual SQLITE_BUSY/SQLITE_FULL, corruption/version/content refusal, metadata race, nonempty RNG/cursors/UInt64, scheduler resume và process SIGKILL phải có bằng chứng thật. Không mô phỏng unit exception rồi gọi processcrash pass.

Không periodic snapshots100commit, snapshot fallback, migration backup, command replay, full content registry hoặc Unity runtime. `snapshots` table reserved; current-image reopen không thay FND12 save manager. Provider chỉ provisional Linux headless; FND11 full provider gate/OPEN03 vẫn pending Unity/native comparison execution.

## Kết quả nghiệm thu — 2026-10-19

Đạt headless subset:224/224tests,40FND11cases,negativecontrol đúng và3SIGKILL subprocess đạt. Review độc lập không còn blocker sau sửa workflow/execution restore binding. Xem [evidence](../../qa/SPRINT-05-EVIDENCE.md). FND12/13 và OPEN03 Unity còn pending; không tự mởSprint6. Git/checks cuối ghi WORKLOG.
