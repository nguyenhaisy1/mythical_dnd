# Settlement authoring registry

Sprint11/VS02A dùng profile độc lập `settlement-content-v1`, không mở rộng ngầm `kernel-content-v1`. Author [pack và provenance](../../content/definitions/vertical-slice/README.md), kiểm schema và semantic compiler trước khi dùng dữ liệu.

```bash
dotnet run --project engine/HeadlessRunner -c Release -- --settlement-compile content/definitions/vertical-slice/pack.json
python3 tools/tests/run_runtime.py --dotnet /path/to/dotnet
```

CLI exit0 khi compile thành công, exit1 khi dữ liệu sai, exit2 khi thiếu args. JSON output chỉ counts/IDs/hash/profile; không ghi database, không khởi tạo WorldState, không thực thi code từ nội dung. Duplicate JSON keys, integer fractions, file>1MiB, depth>20, fields/kinds lạ bị từ chối. Errors hiện là category SETTLEMENT_*; chưa có editor UI highlight recordpath.

Portable API `Game.Infrastructure.SettlementContent.SettlementCompiler.Compile(Dictionary<string,object?>)` kiểm lại cả shape/semantic khi caller không qua JSON. Registry trả Definitions/Get/ByKind/Localization readonly, deepcopies nested objects/arrays. OutgoingRoutes(communityId,season) chỉ lọc directed authored edges theo dry/wet, không tự reverse hoặc chạy travel. Không dùng query này thay world truth sau khi world đã thay đổi.

Canonical mdc1 giữ profile/version/toàn bộ nội dung; definitions sortordinal ID, localization sortordinal chuyển thành key/text tuples; nested array order được bind. Đổi displaytext giữ identity/reference nhưng đổi ContentHash. Exact compiled hash không phải savecompatibility gate cho pack này vì worldinitialization/savebinding chưa được triển khai.

[ADR020](../adr/020-settlement-content-profile.md) có requiredfields/ownership. [Sprint11](../production/sprints/SPRINT-11.md) và evidence phân biệt authoringcompiler tests với gameplay/Unity acceptance. Không load registryomniscient vào playerUI; fact record là claim có nguồn, không worldtruth. Không có pendingevent handler được coi là implemented chỉ vì xuất hiện trong văn bản routine.
