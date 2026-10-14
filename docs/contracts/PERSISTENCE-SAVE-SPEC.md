# Persistence & Save Specification — save-v1

Áp dụng FND-11/12/13; TEST-006/034/035/036/050/051. Hướng SQLite là baseline kiến trúc; **provider binding/native library và Unity target chưa được chọn qua spike**. Đây là schema/transaction contract để làm spike, không là thông báo runtime storage đã có.

## Logical physical layout

Một database cho một world. Tables tối thiểu: `world_meta(world_id PK, revision, tick, schema_version, codec_version, rule_version, content_hash, rng_version)`; `entities(id PK, type, lifecycle, payload BLOB, updated_revision)`; `allocator(namespace PK, next_counter TEXT, exhausted)`; `jobs(id PK, due_tick, phase, priority, creation_sequence TEXT, payload BLOB, status)`; `rng_contexts(context_hash PK, draw_index TEXT)`; `events(id PK, revision, ordinal, payload BLOB, UNIQUE(revision,ordinal))`; `commands(command_id PK, fingerprint, envelope BLOB, outcome, receipt BLOB, revision NULL)`; `snapshots(revision PK, manifest BLOB, payload BLOB, sha256)`.

BLOB payload dùng codec mdc1. UInt64 dùng fixed16 hex TEXT theo Identity Spec khi SQLite signed integer không biểu diễn được; due_tick/revision nằm Int64. Reject overflow, không để driver tự coerce float. SQL schema migration phải thêm NOT NULL, CHECK và indexes theo contract khi provider được chọn; bảng này không được tuyên bố DDL runnable. `commands` chứa cả rejected receipt metadata; canonical gameplay snapshot chỉ chứa accepted mutation receipts. Integrity hash riêng cho rejected/no-op receipts. Không hash database file (page layout không canonical).

## Atomic boundary

Single writer, một explicit transaction cho entity deltas + allocator + tick + jobs + rng + events + envelope/receipt + revision. Commit adapter chỉ báo success khi durable commit hoàn tất; sau đó mới publish state/notifications. Reader không thấy staged state. Không event gửi trước commit. Duplicate command nhận receipt đã tồn tại; unique key bảo vệ race cùng ID. Unknown outcome do connection mất sau commit phải lookup command_id khi mở lại trước retry.

Snapshot mỗi 100 accepted mutation commits và trước manual save/quit thành công; snapshot là consistent revision. Manual save không tạo gameplay command hoặc tiêu thụ RNG. Autosave tại breakpoint dùng cùng policy. Snapshot failure giữ snapshot cũ và journal, báo save error, không nói đã lưu. Không prune journal ở prototype v1; disk-full handling hiển thị lỗi, giữ recovery point, không xóa ngầm dữ liệu cũ.

## Compatibility và recovery

| Tình huống | Hành vi |
|---|---|
| Tất cả versions/hash khớp | restore snapshot, verify integrity, replay command suffix theo revision |
| Thiếu pack / sai content hash / khác rule hoặc RNG version | reject trước gameplay với INCOMPATIBLE_SAVE; không tự thay bằng pack gần nhất |
| Schema cũ có migrator được đăng ký | tạo backup bất biến, migrate sang file mới, validate rồi atomic replace theo platform adapter |
| Schema mới hơn hoặc không migrator | giữ nguyên file, refuse với UNSUPPORTED_SCHEMA |
| Snapshot checksum hỏng | thử snapshot trước + command log còn hợp lệ; nếu thiếu thì fail rõ, không đoán state |
| Crash sau commit trước response | receipt tồn tại; trả outcome cũ, không consume thêm resources |

Recovery replay không republish side effects đã delivered; projection có thể rebuild từ events theo consumer cursor. Cần phân biệt live recovery (khôi phục committed commands) với QA command replay (rerun từ initial snapshot và so hashes). Manifest chứa world seed, versions, content hashes, snapshot revision/tick, last included command/event sequence và checksums; không include secrets vào player UI.

## Provider spike và exit evidence

Owner Infrastructure, FND-11: so sánh một managed binding tương thích headless và một Unity-compatible native binding, ghi license/version/runtime IDs, packaging, parameterized SQL, transaction rollback, busy/disk-full và crash behavior. Chốt provider ADR sau khi cả host/target client mở cùng format. Không dùng provider API trong Foundation/Domain. Editor/backend/OS phải ghi từ môi trường chạy thật; không suy từ .NET build.

Fault injection points: trước BEGIN; sau từng table mutation; trước COMMIT; sau COMMIT trước publish; giữa snapshot write; giữa migration copy. Expected: tất cả hoặc không có commit, không orphan job/reservation/receipt, không mất last good save. Chạy trên process bị kill thật ngoài unit rollback. Backup retention tối thiểu2 bản migration gần nhất; UI cho biết đường dẫn backup và nguyên nhân fail. Chính sách prototype không cam kết save tương thích các schema chưa có migrator.
