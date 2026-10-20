# ADR-015 — Snapshot và khôi phục headless bằng committed images

Ngày 2026-10-20. Sprint6/FND12, kế thừa ADR014. Đây là amendment triển khai cho save-v1; không sửa archive.

## Vấn đề và phạm vi

Sprint5 lưu current image và mirrors nhưng không có recovery point độc lập. Event journal hiện tại chưa chứa đủ deltas để dựng lại mọi state. Command rerun (FND13) chưa triển khai, không được coi việc đọc image là replay luật game.

Sprint6 bổ sung snapshot và recovery frames: mỗi publication đã commit giữ full canonical image, previous hash và sequence. Khôi phục chọn snapshot hợp lệ rồi xác minh suffix liên tục tới committed head độc lập với current image. Không âm thầm bỏ các commits mới khi suffix thiếu hoặc hỏng, kể cả metadata receipts cùng revision. Không gọi rule handlers hoặc phát lại notification khi khôi phục.

## Schema/API và tính tương thích

Physical schema2 dành save manager; API Initialize(initial) giữ schema1 của Sprint5. InitializeSave(initial, worldSeed) tạo schema2, initial recovery point và manifest trong transaction. Load/Commit hỗ trợ cả1/2; schema2 thiếu save tables không được coi là legacy hợp lệ. CreateSnapshot(expected) kiểm full-image CAS và không thay gameplay state.

Manifest lưu versions/content/world identity, sequence/head hash và thông tin snapshot. World seed là metadata host cung cấp rõ ràng, có thể unavailable cho legacy/fixture chưa có seed cấp campaign. Không bịa seed từ world ID hoặc lấy tùy ý một RNG context. Per-context RNG seeds vẫn được bảo toàn trong image. Manifest không là player-facing knowledge projection.

Recovery frame sequence khác world revision: metadata rejection/no-op cũng tạo publication cần giữ. Counter periodic tăng theo accepted mutation commits từ init/migration, không dùng revision modulo; mỗi100 tạo snapshot atomically cùng commit. Manual snapshot không reset counter periodic, không command/RNG/revision mutation. Không prune frames/snapshots/journal trong prototype.

## Failure và migration

Snapshot write failure rollback transaction, giữ last good point. Manual save báo thành công chỉ sau COMMIT; host chỉ quit thành công sau save trả success. Autosave breakpoint dùng cùng explicit snapshot API; chưa có Unity quit/UI wiring. Recovery source chỉ đọc; RecoverToNewFile tạo file mới, validate rồi trả state. Source không bị sửa hoặc ghi đè.

Migrator được đăng ký trong scope là schema1→2. MigrateToNewFile tạo backup nhất quán bằng SQLite backup API, destination mới, validate và báo đường dẫn; không overwrite source/destination/backup đã tồn tại. Không xóa backup tự động, vì vậy giữ được tối thiểu2 lần migration khi caller cấp paths riêng. Không có generic schema migrator hoặc tự thay file gốc qua atomic platform rename; caller chọn output mới sau thành công. Schema mới/không hỗ trợ phải refuse.

## Evidence và giới hạn

Kiểm cadence100, manual invariance/CAS, snapshot/suffix corruption, incompatible versions/content, real SQLite faults, migration fault, scheduler restore/resume và process SIGKILL tại snapshot/migration boundaries. Tests và source review phải thực hiện thật trước nghiệm thu.

Full-image frames tăng chi phí lưu trữ, chưa có performance/retention benchmark. Không chứng minh mất điện, Unity/native targets, deterministic rule rerun FND13 hoặc production content registry FND14. Recovery integrity phát hiện corruption, không là chữ ký chống người dùng cố tình sửa mọi hashes.
