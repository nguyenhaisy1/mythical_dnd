# Sprint 6 — Báo cáo điểm lưu và khôi phục bản lưu

## Tổng hợp dễ hiểu

Sprint này bổ sung phần bảo vệ tiến trình đã lưu. Hệ thống có điểm khôi phục định kỳ, cho phép lưu thủ công mà không làm thời gian hay tài nguyên trong game thay đổi, và có thể tạo lại bản lưu từ dữ liệu còn hợp lệ. Khi nâng cấp định dạng lưu, bản gốc và bản sao dự phòng được giữ lại.

Khôi phục chỉ thành công khi lấy lại đúng toàn bộ trạng thái đã lưu mới nhất. Nếu thiếu dữ liệu, hệ thống báo lỗi thay vì âm thầm làm mất một đoạn tiến trình. Đây vẫn là nền tảng chạy không giao diện; chưa có màn hình Save/Load hoặc bản game chơi thử.

**Đạt phạm vi Sprint6 FND12 headless**, nghiệm thu2026-10-20, SPIN-009. 256/256tests và6caSIGKILL mới đều đạt; giới hạn ngoài scope ghi cuối báo cáo.

## Phạm vi và phân công

FND12 headless snapshot/manual save/complete-image recovery và new-file migration1→2, theo ADR015. /root điều phối/ADR/docs/CLI/crash/integration/Git. /root/s6_storage hiện thực Infrastructure và sửa findings; /root/s6_tests soạn integration tests. Hai agent chạm usage limit trước final gate; root tiếp quản. /root/s6_design_review review thiết kế và nguồn độc lập, không chạy runtime suite.

Review đã sửa checksum manifest/world binding, codec failure fallback, nguồn recovery/migration chỉ đọc, accepted receipt counter và giữ cadence khi recovery. Reviewer xác nhận không còn blocker cả sau delta bổ sung migration-before-COMMIT. Root thêm fault rollback/retry mutation100 và RNG không rỗng để tránh chỉ kiểm fixture trivial.

Debug preliminary25cases có3fail do test so raw bytes backup với source. Điều chỉnh assertion theo contract: SQLite backup có thể đổi page metadata; canonical committed state phải giống, source bytes không đổi và backup cũ không bị sửa. Không sửa gameplay implementation để khớp page bytes.

## Runtime evidence cuối

Lệnh: `python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`.

- SDK10.0.401/Linux x64; locked restore và Release build0warnings/0errors.
- **256discovered/256passed/0failed/0skipped**:224regression +32FND12cases. Runtime suite2m39s là thời lượng test quan sát được, không là benchmark game.
- Negative control riêng đúng1testfail/exit1; toàn gateexit0. Transfer/scheduler demos đều đạt.
- Source fingerprint `sha256:8fe87283c85474f8ad97fbfa889caeec80c07875f241581634affd005a0b4687`.
- Evidence `tests/results/sprint06-nf_eo33r/evidence.json`, sources.json,TRX,restore/build/runtime/negative/demo logs,persistence-crash.json,save-crash.json. Generated evidence ignored; báo cáo nguồn này được track.

## Acceptance được kiểm

- Initial snapshot/schema2 manifest/optional host seed; legacy schema1 vẫn mở và commit.
- Cadence mỗi100accepted mutation commits; không đếm rejection/noop/duplicate, manual99 không reset counter. Fault tại mutation100 rollback rồi retry tạo đúng snapshot.
- Manual save giữ full canonical bytes gồm RNG contexts/draw index, tick, identities, receipts, jobs; full-image CAS chặn metadata stale cùng revision.
- Snapshot hỏng checksum hoặc malformed nhưng checksum hợp lệ có thể fallback tới snapshot trước và complete suffix; lấy đúng head gồm metadata receipt.
- Missing middle/tail, sai hash/head/versions/content/manifest hoặc thiếu schema2 manifest bị từ chối; không âm thầm trả trạng thái cũ.
- Scheduler PausedBudget được khôi phục và tiếp tục tới Completed, không nhân đôi execution.
- Migration có immutable backup, output mới được validate, không overwrite source/backup/destination. Fault sau backup, giữa transaction output và sau output commit giữ nguồn/backup.
- Recovery giữ accepted mutation counter99; commit kế tạo periodic snapshot100, dù output reanchor recovery sequence0.

## Process crash evidence

Harness tools/tests/check_save_crash.py dùng READY barrier rồi SIGKILL child process thật. Ba ca snapshot: after_snapshot, before_commit, after_commit. Ba ca migration: migration_after_backup, migration_before_commit, migration_after_copy. Sau kill, mở lại bằng C# provider; kiểm canonical state, snapshot count, source unchanged, backup và output tương ứng. **Cả6ca đạt**. Snapshot trước COMMIT giữ1điểm, sau COMMIT có2điểm; cùng canonical hash/revision2/source7. Migration giữ nguồn schema1 và backup hợp lệ; output bị kill trước COMMIT bị từ chối, sau copy hoàn tất mở được schema2.

Ba ca current-state transaction SIGKILL Sprint5 tiếp tục chạy regression. Đây là Linux process-crash evidence, không power-loss, filesystem hardware durability hoặc Unity test.

## Compatibility và giới hạn

Schema2 là opt-in InitializeSave; Initialize vẫn schema1. MigrateToNewFile chỉ hỗ trợ1→2. Recovery/migration không thay file gốc; caller chọn file mới sau success. Khi failure giữa chừng có thể còn file output chưa hợp lệ, không tự overwrite/xóa. Backups không prune; caller cấp paths riêng để giữ ít nhất hai bản gần nhất.

World seed là optional metadata do host cung cấp; legacy unavailable không được bịa từ RNG contexts. Full-image frames tốn dung lượng và chưa benchmark; không retention optimization. Committed-image recovery không chạy lại resolver; FND13 command replay còn riêng. Unity save/quit/UI, native provider OPEN03 và platform atomic replacement chưa nghiệm thu.

Bước tiếp theo FND13 replay. Không tự mởSprint7 hoặc release. Hướng dẫn API và lỗi tại [SAVE-RECOVERY](../runbooks/SAVE-RECOVERY.md); Git/checks cuối ghi [WORKLOG](../../WORKLOG.md).
