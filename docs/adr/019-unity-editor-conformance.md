# ADR-019 — Profile kiểm chứng Unity Editor Linux

Ngày 2026-10-23. Phạm vi FND16/TEST016, Sprint10/SPIN013. Quyết định khóa môi trường sau actual run được ghi trong [evidence Sprint10](../qa/SPRINT-10-EVIDENCE.md); ADR không tự thay kết quả kiểm thử.

## Bối cảnh và quyết định

Chủ dự án cài Unity `6000.6.7f1`. Profile kiểm chứng là Linux x64, Editor Mono, API compatibility `NET_Standard`, portable libraries target netstandard2.1. Không yêu cầu đổi về 6.3 LTS chỉ vì đề xuất ban đầu; dùng exact Editor thật và kiểm cùng known answers trên .NET host. Đây là lựa chọn cho prototype; không cam kết vòng đời hỗ trợ hoặc compatibility của Player/IL2CPP.

`RuntimeConformance` chạy từ cùng DLL ở host và Editor: codec tiếng Việt, RNG vector, replay15actions, content compiler hash, SQLite commit/reopen/snapshot/refusal. Mọi kết quả phải khớp fixed fingerprint; không regenerate golden cho qua test. SQLite provider giữ Microsoft.Data.Sqlite10.0.13/SQLitePCLRaw2.1.13, không đổi save/schema/content versions.

Runner tạo project bằng Editor thật, lưu ProjectVersion, executable hash, import inventory, source hashes, commands và timestamp. Stage vào `Assets/Plugins/MythicalRuntime`, chạy import riêng trước khi thêm entry script. Thử nghiệm đường dẫn cũ `GameCore` thất bại tìm namespace, rename cho phép chạy probe; không kết luận nguyên nhân nội bộ Xbox/platform importer khi chưa có bằng chứng đó.

API enum có alias: actual Editor có thể hiển thị `NET_Standard_2_0` cho giá trị bằng `NET_Standard`. Entry kiểm enum equality trước khi chạy, xuất nhãn chuẩn cùng raw enum name/value. Không nới comparator để chấp nhận API khác.

## Acceptance và giới hạn

Actual successful run cho phép nghiệmthu FND16 ở profile Editor này và kết hợp FND02–15 nghiệmthu VS01 kernel. Năm semantic checks không phải 338 Unity tests. Full regression338 là host .NET; automated probe không phải human playtest.

OPEN04 khóa exact patch/profile khi evidence đạt. OPEN03 chỉ xác nhận provider commit/reopen/snapshot/refusal trong Editor; Player packaging, Unity crash/recovery/migration, platform atomic replacement và IL2CPP vẫn chưa kiểm. Client game vẫn layout, chưa có scene/UI chơi được. Nền tảng được nghiệmthu chỉ trong kernel subset; không suy toàn bộ GDD hoặc game production-ready.

Nâng Editor/provider phải chạy lại profile và review inventory/hash; giữ source archive bất biến, cập nhật notes của modules26/29 và các tài liệu bàn giao có liên quan.
