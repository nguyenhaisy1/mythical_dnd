# Nạp và kiểm tra runtime content

Sprint 8/FND-14 triển khai profile `kernel-content-v1` theo [ADR-017](../adr/017-runtime-content-registry.md). Đây là compiler portable với parser JSON ở host .NET, không phải validator reference hoặc Unity loader.

## Dữ liệu và cách chạy

Authoring dùng `content/schemas/kernel-content-v1.schema.json`; mẫu chạy thật ở `content/fixtures/content-kernel/pack.json`. Root fields bắt buộc: pack_id, version, schema_version=1, rule_version=transfer-resource-v1+scheduler-v1, dependencies, definitions. Mỗi dependency pin pack_id/version/content_hash đúng tuyệt đối.

Definition có definition_id/version/kind/schema_version/source/classification/owner_domain/display_key. Hai kinds: commodity với unit=count; transfer-template với commodity_id, handler=TransferResource@1, quantity nguyên dương Int64. Namespace ID phải tương ứng commodity hoặc transfer; suffix @version phải khớp version. Pack version độc lập definition version. Ba classification là historical/traditional/game-canon; prototype-authored ghi trong source. Owner hiện chỉ Assets. Display key là token, không tự tạo localization text.

```bash
dotnet build MythicalDnd.sln -c Release
dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --content-compile content/fixtures/content-kernel/pack.json
# Truyền đúng hash đã được chấp thuận cho campaign; demo yêu cầu file mới.
dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --content-demo content/fixtures/content-kernel/pack.json /tmp/content-demo.sqlite e45fa90669459e4516323a71ff959d7f9b6dd637d7e33c148306ba0cb9808b72
dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --content-open content/fixtures/content-kernel/pack.json /tmp/content-demo.sqlite e45fa90669459e4516323a71ff959d7f9b6dd637d7e33c148306ba0cb9808b72
```

Các lệnh nhận thêm đường dẫn dependency packs cuối arguments. Host compile dependencies theo đồ thị rồi root; thứ tự file đầu vào không cấp quyền tham chiếu ngoài dependency closure. Thiếu hoặc vòng phụ thuộc bị từ chối. Compiler derived manifest xuất definitionIds, ruleReferences, validationProfile và contentHash. `definitions` trong output registry là definitions của pack; Resolve dùng cả closure đã khai báo.

## Validation và quyền thực thi

Parser từ chối unknown/duplicate/missing fields, trailing data/comments, sai kiểu và số không nguyên. Giới hạn file 1 MiB, JSON depth16, 4.096 definitions/pack, 64 dependencies trực tiếp; compiler giới hạn closure256packs. Schema phục vụ authoring; runtime parser/compiler tự kiểm định, không tải schema từ Internet.

Semantic compiler kiểm metadata, version, ID trùng, dependency pin/closure, đúng loại commodity và handler có thật. Hash SHA-256 lowercase64hex từ các fields UTF-8 length-prefixed, số decimal invariant, null marker -1, collections sắp ordinal. Hash bind toàn bộ provenance, version và dependency hashes; không phụ thuộc formatting hoặc thứ tự authored definitions. Registry tạo được chỉ qua compile thành công và không sửa được sau đó.

Content không được SQL/script/state path/eval. Parser báo path và ID khi có; semantic error có code + offending definition/pack. Transfer-template tạo typed command, vẫn qua authorization/resource/location/invariants/commit của pipeline. Không cấp authority cho narrative. Host mới resolve expected hash trước chạm store, kiểm world commodity trước processor. Save content hash mismatch bị từ chối, không thay pack bằng bản mới cùng tên.

## Giới hạn

Demo là world cố định phục vụ nghiệm thu, chưa là campaign loader/UX; người gọi phải cung cấp expected hash từ campaign selection. SQLite vẫn kiểm hash thật bên trong save trước đọc state thành công. Registry không chứa runtime entity instances và không bypass ownership. Các QA probes cũ giữ fixture hashes riêng để hồi quy, chưa chuyển save/golden cũ sang pack này.

Không có binary distribution/cache; JSON đã kiểm định compile thành registry trong bộ nhớ. Không có Unity loader, full NPC/household/routes/event AST/recipes hoặc roster VS-02. Khi thêm loại definition phải bổ sung contract/schema/compiler/semantic tests, không bỏ qua unknown fields để tương thích giả.
