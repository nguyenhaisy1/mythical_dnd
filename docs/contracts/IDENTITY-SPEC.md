# Identity & Registry Specification — identity-v1

Áp dụng: FND-02/05/08/12, TEST-027. [ADR-008](../adr/008-prototype-contract-baseline.md) chọn contract từ các phương án M26 §12, M29 §9–14. Đặc tả được hiện thực ở portable Foundation; trạng thái kiểm chứng xem evidence FND-02.

## Representation và codec

Entity wire ID: `w0000000000000001/actor/0000000000000001`. World part là `w` + 16 lowercase hexadecimal digits, namespace là một tag đăng ký, counter là 16 lowercase hexadecimal digits. World và counter trong [1, 2^64−1]; zero bị cấm. Tag v1: `actor`, `household`, `item`, `lot`, `activity`, `job`, `event`, `route`, `settlement`, `thread`. Không dùng tên hiển thị trong ID. API C# dùng typed wrappers, serializer mới chuyển sang string.

Definition ID: `kind.slug@version`, regex `^[a-z][a-z0-9-]*\.[a-z][a-z0-9-]*@[1-9][0-9]*$`; ví dụ `commodity.timber@1`. Version là integer decimal, không leading zero. Một definition đã publish không được sửa nội dung mà giữ nguyên version/hash. Fixture `TG-01` là authored alias, không phải EntityId; compiler lưu mapping alias → DefinitionId → instance trong manifest.

Null reference dùng JSON null ở field được schema cho phép; không dùng empty string, zero hoặc ID giả. Unknown required reference gây `REF_NOT_FOUND`; sai type gây `REF_TYPE_MISMATCH`; cross-world gây `REF_WORLD_MISMATCH`. Decode strict: uppercase hex, whitespace, escape không canonical, namespace chưa đăng ký đều bị từ chối. So sánh tuple `(world UInt64, namespace ordinal ASCII, counter UInt64)`; không phụ thuộc locale. Không trộn DefinitionId với EntityId.

## Allocation và transaction

Allocator lưu `next_counter` riêng theo `(world,namespace)` trong authoritative snapshot. Stage allocate lấy next, tăng trong staged state. Abort loại cả ID và counter staged; retry cùng committed snapshot cho cùng ID. Sau commit ID không bao giờ tái dùng, kể cả entity tombstoned. Allocation chạm UInt64.MaxValue cấp ID cuối rồi đánh dấu exhausted; lần tiếp theo trả `ID_EXHAUSTED`, không wrap. ID chưa commit không được xuất UI/event/receipt thành công.

| Given | Action | Expected |
|---|---|---|
| next actor=7 | allocate rồi abort | next=7; không có actor 7 |
| next actor=7 | retry và commit | actor 7 tồn tại; next=8 |
| actor 7 đã commit | tombstone, allocate | actor 8; actor 7 vẫn tra history được |
| snapshot next=8 | load | allocation kế tiếp là 8 |
| next item=7 | allocate actor | item next không đổi |

Registry giữ type, lifecycle, creation revision, retirement revision và canonical ref. Entity lifecycle `Staged → Active → Retired → Tombstone`; Staged không persisted, không cho Tombstone→Active. Actor death là trạng thái Health/Characters của entity Active, không tự xóa actor khỏi registry. Dead actor vẫn có history và inheritance; quyền thực hiện action bị rule từ chối. Lot exhausted và job completed được retire khi các references đã có lịch sử/event; tombstone chỉ loại payload nặng khi snapshot/replay retention cho phép. Không cascade delete household hoặc relative vì actor chết.

## Acceptance

Parser roundtrip ở cả headless và Unity; invalid examples phải reject: `w0000000000000000/actor/0000000000000001`, namespace `Actor`, counter `1`, reference actor trong field yêu cầu lot. Test allocator rollback/restore/exhaustion và typed equality. Golden fixture nhỏ ở [kernel-smoke](../../content/fixtures/kernel-smoke/README.md). Kiểm tra headless C# được ghi riêng tại [FND-02 evidence](../qa/FND-02-EVIDENCE.md); Unity còn pending FND-16. Reference data không thay runtime acceptance.

## API triển khai

Theo [ADR-009](../adr/009-headless-test-runner.md): immutable typed wrappers, IdentitySnapshot và IdentityStage. PrepareSnapshot chỉ tạo candidate để Application commit; không publish UI hoặc thay atomic world transaction. Restore bắt buộc đủ cursors cho 10 namespaces; bỏ cursor không được reset về 1. Registry trả historical refs cho cả Retired/Tombstone; action eligibility vẫn thuộc rule.
