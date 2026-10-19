# Sprint 5 — nghiệm thu SQLite current-state persistence

Nghiệm thu 2026-10-19, SPIN-008. **Đạt phạm vi FND11 headless SQLite atomic commit/reopen**. OPEN03 provider cho Unity còn pending; periodic snapshots, fallback recovery và replay thuộc FND12/13.

## Phân công và review

/root điều phối, tiếp quản implementation codec/SQLite adapter, CLI crash harness, integration tests, tài liệu và Git. /root/s5_codec bắt đầu store port/raw receipt/prepublication bridge rồi bị usage limit. /root/s5_sqlite ban đầu bị quota trước implementation; khi tiếp tục đã bổ sung 14 test cases và chạy targeted Debug 40/40. /root/s5_review thực hiện review nguồn độc lập sau khi tiếp tục; không tự chạy build/test.

Review tìm thiếu binding giữa restored workflow và original AdvanceTime receipt, execution và internal command/job/workflow payload. Root sửa semantic validation và thêm mutation coverage. Owner mutation cuối dùng actor2 tồn tại để kiểm đúng mismatch với receipt, thay vì chỉ kiểm dangling actor. Reviewer xác nhận không còn blocker trong scope; nghiệm thu runtime dựa Release gate của root.

## Runtime evidence cuối

Lệnh: `python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`.

- SDK10.0.401, Linux x64; locked restore và Release build thành công, 0 warning/0 error.
- **224 discovered / 224 passed / 0 failed / 0 skipped**: 184 regression + 40 FND11 cases.
- Negative control riêng: đúng 1 test fail, exit1; toàn gate exit0.
- Transfer và scheduler demo đều đạt; scheduler vẫn clock30/revision7/2executions/Completed.
- Source fingerprint: `sha256:88ed1f6aa58b56f80b0f5e25d1ae4073de32794250cfb410457bc9e8bf49f2aa`.
- Evidence: `tests/results/sprint05-dgsslqle/evidence.json`, sources.json, TRX, restore/build/runtime/negative/demo logs và persistence-crash.json. Artifacts ignored; report này được track.

## Durable acceptance

Canonical mdc1 current-image giữ identities/lifecycles/cursors, actors/lots, nonempty RNG context và draw index, original envelopes, receipts, events, jobs, workflows và executions. UInt64.MaxValue revision roundtrip. Malformed/truncated/deep/noncanonical/unknown-field input và semantic links sai bị từ chối; accepted transfer convenience receipt phải khớp event.

SQLite transaction ghi image và relational mirrors atomically; full-image CAS chặn cả race giữa metadata receipts cùng revision. Fault injection tại 9 điểm trước COMMIT giữ toàn bộ committed state cũ; actual SQLITE_BUSY và SQLITE_FULL được kiểm trên file thật. Store failure đặt NeedsRecovery, cấm tiếp tục commands/drain cho đến khi host mở lại processor. Lỗi mất acknowledgement sau COMMIT mở lại original receipt, retry không nhân đôi effect.

Corruption/hash/schema/content/rule/RNG version hoặc mirror mismatch bị từ chối, không ghi đè save. Pending jobs, PausedBudget, PausedDecision và Completed workflow mở lại được; decision pause cần resolve rõ ràng. NoOp/stale/duplicate/mismatch semantics tồn tại sau reopen.

### Process crash thực

Runner tạo child process riêng, đợi READY barrier rồi gửi SIGKILL (-9), sau đó mở file bằng process mới:

| Điểm kill | Trước retry sau reopen | Sau retry |
|---|---|---|
| after_entities | revision1, source10 | revision2, source7, 1receipt |
| before_commit | revision1, source10 | revision2, source7, 1receipt |
| after_commit | revision2, source7 | revision2, source7, 1receipt |

Cả 3 đạt. Runtime báo SQLite3.53.3, Microsoft.Data.Sqlite10.0.13.0. Đây là Linux process termination evidence, không là thử mất điện, filesystem failure hay Unity acceptance.

## Compatibility, giới hạn và bước tiếp theo

[ADR014](../adr/014-headless-sqlite-persistence.md) quy định UInt64 revision fixed16hex TEXT, current-image mdc1, DELETE journal/FULL synchronous và full-image CAS. Initialize chỉ tạo file mới; không overwrite save cũ. Initialize thất bại có thể để lại file mới không hợp lệ; load phải từ chối. Không có migration path cho layout khác.

Full image/table rewrite mỗi commit là implementation nền tảng; chưa có benchmark, incremental persistence hay retention. snapshots table reserved, chưa có periodic snapshot100commits, fallback recovery, backups, migration hoặc deterministic replay. Content hash caller-supplied chưa là full production content registry. [Provider spike](SQLITE-PROVIDER-SPIKE.md) chỉ xác minh headless provider; Unity/IL2CPP/native target chưa chạy.

Bước kế tiếp FND12 save/snapshot/recovery, sau đó FND13 replay. Chưa có bản game chơi thử hoặc Unity UI; không tự mở Sprint6.

## Đồng bộ tài liệu và Git

Contracts/ADR, backlog/matrix, implementation plan, handoff/agent, README/architecture, runner/CI/changelog và export inputs được cập nhật. Kết quả documentation/export checks và commit/PR/CI cuối ghi tại [WORKLOG](../../WORKLOG.md). Main/tag/release ngoài scope.
