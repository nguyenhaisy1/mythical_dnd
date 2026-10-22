# Sprint 8 — Báo cáo kiểm tra và nạp nội dung

## Tổng hợp dễ hiểu

Sprint này xây bước kiểm tra dữ liệu trước khi game sử dụng. Nội dung sai định dạng, gọi hành vi không được phép hoặc tham chiếu tài nguyên chưa tồn tại sẽ bị chặn và báo vị trí lỗi. Bản lưu cũng phải mở với đúng bộ nội dung đã dùng, tránh thay đổi âm thầm khiến thế giới bị tính sai.

Luồng nghiệm thu dùng mẫu chuyển ba đơn vị gỗ: đọc dữ liệu, kiểm tra, chạy hành động thật, lưu rồi mở lại. Đây là nền tảng để sản xuất nội dung sau này; chưa có bản đồ, nhân vật và gameplay đầy đủ để chơi thử.

**Đạt FND-14 headless kernel profile**, nghiệm thu ngày2026-10-22. Sprint8/SPIN-011 mở2026-10-21. Scope FND-14/TEST-017/036, profile kernel-content-v1.

## Phân công và review

/root điều phối, ADR/schema/strict parser, host integration, documentation sync, gates và Git. /root/s8_compiler triển khai portable compiler/immutable registry. /root/s8_tests viết31 runtime cases và checker subprocess; chạy31/31Debug đạt trước quota. /root/s8_review review thiết kế và code độc lập, đóng findings sau root sửa dependency count trước file I/O và thêm assertion hash cố định. Root tiếp quản phần docs/final run sau agents quota; không ghi việc chưa chạy thành kết quả của agent.

Independent source/delta review không còn blocker; không thay runtime evidence. Đã bỏ assertion hash trùng do edits giao nhau. Không có dependency package mới.

## Runtime evidence cuối

Lệnh: `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`.

- SDK10.0.401/Linux x64; locked restore và Release build0warnings/0errors.
- **315/315 tests đạt, 0 failed/0 skipped**:284 hồi quy và31 FND14 cases. Runtime negative control riêng đúng1fail/exit1; toàn gate exit0.
- Content checker46processes,15invalid cases đạt; actual transfer/save reopen và dependency refusal đúng. Hai demo trước, hai process replay golden, replay negative control và9SIGKILL hồi quy đều đạt.
- Evidence `tests/results/sprint08-2bas9yx4/evidence.json`; source fingerprint `sha256:14d6b6f2a197b79da3da4a98b5946e76b77746c9b57e7e4ffbd01c09be1d1918`.
- Lần sprint08-epb1658_ có315tests pass nhưng gate exit1 Sources changed during run, không dùng nghiệm thu. Lần cuối chạy sau source freeze.
- Independent review đã duyệt thêm catch InvalidDataException explicit cho lỗi parser/save; subprocess xác nhận diagnostic JSON và exit1, không còn unhandled exception cho các ca kiểm tra.

## Acceptance thực thi

- Compile hai process cho cùng identity/hash; đảo thứ tự definitions không đổi identity. Typed handler là TransferResourceRule thật; không load reflection/script.
- Demo chuyển3 từ10/0 thành7/3, revision2; reopen phải khớp stateHash. Template tạo command, không bypass authority/invariants/commit.
- Unknown handler, embedded script, unknown/duplicate/missing fields, schema/ref/kind/quantity sai và input quá lớn bị từ chối với diagnostic; definition/path đi kèm lỗi record.
- Wrong expected hash, changed actual content, absent dependency bị từ chối; kiểm save bytes không đổi và file output không được tạo ở negative cases.
- Dependency exact version/hash, closure/cycles/conflicts, immutable collections và world commodity references có C# runtime tests. Parser boundary có subprocess checks.

Fixture content/fixtures/content-kernel/pack.json dùng hash cố định e45fa90669459e4516323a71ff959d7f9b6dd637d7e33c148306ba0cb9808b72. Hash lần đầu được tạo bởi implementation rồi pin để kiểm hồi quy; không giả đây là oracle độc lập. Kết quả chuyển tài nguyên7/3 và revision2 được đối chiếu riêng.

## Compatibility và giới hạn

JSON schema/kernel-content-v1 mới chỉ cho commodity và transfer-template; registry compiled trong bộ nhớ, không có binary distribution. Dependency hashes pin exact closure, không hot reload. Không đổi save schema2/replay-v1/RNG/mdc1; không tự migrate save fixture cũ sang content hash mới.

TEST-017/036 được nghiệm thu trong profile kernel, không mở rộng suy luận sang full NPC/route/event AST/recipe chưa có runtime contracts. Full roster VS-02, Unity FND-16, scale benchmark và playable game còn pending. FND-15 diagnostics là bước tiếp theo; không tự mở sprint mới hoặc release. Git/CI cuối ghi [WORKLOG](../../WORKLOG.md).
