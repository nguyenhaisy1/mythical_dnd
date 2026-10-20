# Vận hành save manager headless

## Tổng hợp dễ hiểu

Hệ thống giữ bản lưu hiện tại, các điểm khôi phục và dữ liệu thay đổi đã lưu giữa chúng. Khi bản lưu gặp lỗi, có thể tạo một file mới chứa đúng tiến trình đã lưu gần nhất. Nó không tự quay về điểm cũ rồi coi là thành công; nếu thiếu dữ liệu cần thiết thì từ chối khôi phục. Bản gốc được giữ lại để kiểm tra.

Đây là API dành cho host và công cụ phát triển, chưa phải màn hình Save/Load của game. Giới hạn nghiệm thu xem báo cáo Sprint6; quy tắc format tại ADR015.

## Tạo và lưu

Dùng SqliteWorldStore với file path/content hash như Sprint5. Initialize(initial) vẫn tạo schema1, không có snapshot manager. InitializeSave(initial, worldSeed) tạo schema2 với initial snapshot. Seed cấp campaign chỉ đưa vào khi host biết thật; legacy/fixture có thể null, không suy từ một RNG context.

Mọi CommandProcessor mutation phải inject chính store đó. Schema2 ghi recovery frame cùng transaction current state/receipts/events/jobs/RNG. Snapshot tự động mỗi100accepted mutations từ init/migration, không dựa revision modulo. Rejected/no-op metadata có frame riêng nhưng không tăng counter periodic; duplicate không tạo publication mới.

Lưu thủ công: lấy committed snapshot từ host composition đáng tin cậy rồi gọi CreateSnapshot(expected). Full-image CAS chặn snapshot từ state đã cũ, kể cả khi revision giống nhau nhưng metadata receipt thay đổi. Manual snapshot không thay gameplay/RNG/time hoặc reset counter100. Nếu exception xảy ra, host báo save error và không tuyên bố quit/save thành công. Các API này không được đưa toàn World Truth cho UI.

GetSaveInfo trả schema version, sequence, accepted mutation count, snapshot count và optional seed. Sequence là số publication từ bootstrap recovery chain; khác world revision. Recovery output reanchor sequence0 nhưng giữ cumulative accepted count để cadence không đổi. AcceptedSinceSnapshot là modulo100 của counter periodic, không là số mutations từ manual snapshot gần nhất.

## Mở lại và khôi phục

1. Normal Load kiểm versions/hash/current-image/mirrors và schema2 manifest. SQLite xử lý transaction chưa commit khi mở file; không coi lỗi current-image là permission tự overwrite file.
2. Khi cần recovery, gọi RecoverToNewFile(destination) với path mới. Source được đọc bằng read-only connection; không sửa file đang chẩn đoán.
3. Chọn snapshot hợp lệ gần nhất, xác minh chuỗi suffix liên tục và đúng committed head hash. Snapshot hỏng có thể bỏ qua; suffix thiếu/hỏng mà không có đường đầy đủ tới head phải fail.
4. Output được validate trước khi method trả thành công. Host chỉ chọn output mới sau success; không tự thay nguồn. Recovery không rerun rules, không consume RNG hay phát lại notifications.
5. Nếu output thất bại giữa chừng, file chưa hợp lệ có thể tồn tại để chẩn đoán; lần tiếp theo dùng path mới hoặc xử lý file lỗi có chủ đích. Không tự overwrite, xóa nguồn hoặc backup.

Full-image frames bảo toàn receipt metadata, paused scheduler và RNG. Sau recovery host compose processor với đúng approved handler registry để tiếp tục jobs. Đây không là QA command replay; FND13 còn cần chạy lại resolver và so hashes riêng.

## Nâng cấp schema1→2

Gọi MigrateToNewFile(destination, backupPath, worldSeed) với hai paths chưa tồn tại. Migrator validate source/content, tạo SQLite-consistent backup từ cùng source read transaction, validate backup và tạo schema2 output. Mọi exception giữ nguồn; backup hoàn tất trước bước viết output. File backup không nhất thiết byte-identical với source vì SQLite page metadata/layout, nhưng canonical committed state phải giống nhau.

Dùng paths riêng cho các lần migration; không prune backups, giữ ít nhất hai bản gần nhất. Không hỗ trợ schema tương lai hoặc arbitrary migrators, không in-place replace, không tự nâng cấp lúc Load. UI cần báo đường dẫn backup và lỗi khi được triển khai; hiện API chỉ headless.

## Kiểm tra và giới hạn

Lệnh chuẩn: python3 tools/tests/run_runtime.py --dotnet /path/to/dotnet. Bao gồm unit/integration runtime, negative control và subprocess SIGKILL. SaveProbe trong HeadlessRunner chỉ là fixture cho crash harness, không dùng làm production gameplay frontend.

Không có power-loss test, Unity lifecycle wiring, platform atomic replace, storage benchmark hoặc retention optimization. Full-image recovery tốn dung lượng theo số publication và kích thước world; chưa phù hợp để hứa hiệu năng cho1.000NPC.
