# ADR-014 — SQLite persistence prototype cho headless kernel

Ngày2026-10-19. Áp dụng Sprint5/FND11 headless subset; quyết định provider toàn runtime vẫn provisional. Archive/GDD không đổi.

## Provider và ranh giới

Chọn Microsoft.Data.Sqlite10.0.13 (MIT), pin exact + packages.lock.json ở Infrastructure/Headless/tests. NuGet package metadata có netstandard2.0 asset, chạy trong Infrastructure netstandard2.1; host/tests net10.0. Dependency SQLitePCLRaw bundle/core/provider/native2.1.13. Không đưa provider vào Domain/Foundation. Xem [provider spike](../qa/SQLITE-PROVIDER-SPIKE.md).

Không có Unity Editor/backend được chạy trong sprint này; ứng viên Unity sqlite-net chỉ được so sánh từ source/packaging. Không đóng OPEN03 hoặc tuyên bố cross-runtime compatibility. FND16/native client spike vẫn phải mở cùng database và chứng minh behaviors trên target thật.

## Schema và encoding

Schema1 chứa world_meta,entities,allocator,jobs,rng_contexts,events,commands,snapshots. Bổ sung `world_meta.image` là whole current committed image mdc1 + sha256; toàn entities/actor/lot/cursors/jobs/RNG/events/commands được mirror vào tables bằng mdc1 payload. Load so mirrors với image, không âm thầm rebuild khi mismatch. Events có unique(revision,ordinal), commands có uniqueID/fingerprint/metadata flag. SQL values dùng parameters; table names whitelist hằng.

Đây là amendment layout triển khai từ logical schema save-v1: chưa dùng field-level deltas hoặc SQL due-job queries; rewrite current image/mirrors trong transaction phù hợp prototype nhỏ. Không benchmark/retention claim. `snapshots` table reserved cho FND12, không giả current image là periodic snapshot/replay system.

WorldID/revision UInt64 fixed16hex TEXT. Điều chỉnh dòng save-v1 cũ ghi revision Int64 vì kernel đã có UInt64; không thu hẹp range hoặc coerce float. Tick giữ signedInt64 không âm. Full image giữ numeric type mdc1 đúngrange, raw original command envelope, accepted/metadata receipts, scheduler workflow/executions và toàn RNG contexts. Decoder giới hạn64MiB/depth48, reject malformed/trailing/noncanonical/unknown fields qua re-encode; semantic references/order/receipt fingerprints được kiểm.

## Atomic boundary và recovery

Application ICommittedWorldStore port, không tham chiếu Infrastructure. Trước mọi publication (transfer/metadata/scheduler), store.Commit(expected,candidate) hoàn tất SQL transaction rồi mới đổi current reference. SQLite dùng DELETE rollback journal, synchronousFULL, private connections pooling=false, explicit write transaction. Không query authoritative state qua UI.

CAS so toàn mdc1 image, gồm metadata receipts. Không chỉ dùng revision vì rejection/no-op không tăng gameplay revision. Conflict/error làm processor NeedsRecovery; mọi command/drain tiếp bị chặn. Reopen store.Load → processor mới sẽ lấy receipt đã commit hoặc trạng thái cũ, an toàn khi COMMIT outcome chưa biết. Lỗi callback sau Application publication vẫn giữ state/receipt đã commit.

Initialize là thao tác explicit create-new file, không overwrite file đã tồn tại dù rỗng/incompatible. Load kiểm schema/codec/rule/content/RNG versions và hashes, refuses corruption. Failed initialize có thể để file mới chưa hợp lệ để chẩn đoán; không tự xóa/ghi đè file người dùng. Chưa migration hoặc snapshot fallback.

## Scope evidence

Real SQLITE_BUSY/FULL khác injected IO failures; SIGKILL thực sự khác power-loss/fsync guarantee. Current-image reopen và scheduler resume được kiểm trong headless, không chứng minh FND12 snapshot fallback/migration hoặc FND13 deterministic rerun. Handler registry khi reopen phải chứa đúng approved compiled keys. Provider/libs có version/license/packaging evidence riêng; Unity còn open.
