# Kiểm điều kiện sự kiện

Profile `event-eligibility-v1` theo [ADR021](../adr/021-event-eligibility-profile.md). Tám template tại [events.json](../../content/definitions/vertical-slice/events.json), tham chiếu hash settlement registry Sprint11. Đây là công cụ developer, không phải UI hoặc bộ mô phỏng NPC.

```bash
dotnet run --project engine/HeadlessRunner/Game.Headless.csproj -c Release -- --event-eligibility content/definitions/vertical-slice/pack.json content/definitions/vertical-slice/events.json content/fixtures/event-eligibility/scenarios.json
```

Bỏ đối số scenarios để chỉ compile. Exit 0: compiler thành công và mọi expectation fixture khớp nếu có; exit 1: dữ liệu lỗi hoặc expectation không khớp; exit 2: sai số đối số. JSON stdout chứa hash, số template và kết quả từng scenario; lỗi input ở stderr. Mismatch không được ghi Passed chỉ vì chương trình có chạy.

`EventEligibilityCompiler.Compile(input, settlement)` trả registry readonly. `Evaluate(templateId, context)` kiểm actor còn sống, đúng allowlist, cooldown và hai biểu thức vật lý/nhận thức. Context được copy; không tiêu RNG hoặc ghi world/cooldown. Tập known facts phải đầy đủ cho actor, tick occurrence phải đúng template/actor; caller trusted chịu trách nhiệm provenance. Unknown physical/resource/perceived scalar không được dùng Not để biến thành eligible.

Các fixtures là tình huống kiểm chứng chủ động, không phải ảnh chụp WorldState thật. `no-player` chỉ chứng minh biểu thức không cần player flag; không chứng minh NPC tự thực thi hành động. Rumor được biết vẫn là claim, không được nâng thành truth. Chưa nối shared route delivery hoặc knowledge projection cho player.

Khi đổi pack, kiểm schema + compiler, fixtures, tests và hash. Khi đổi settlement hash, phải review refs và cập nhật binding rõ ràng; không tự chấp nhận mismatched pack. Không thêm executable handler trước khi có typed command/delta/ownership/atomic commit và acceptance cho gameplay tương ứng.

Nguồn tình huống cụ thể: [M28 §34–38](../gdd/modules/28-vertical-slice-blueprint.md#m28-t077-s055) nêu nhánh không can thiệp, khủng hoảng môi trường, các outcome và ba thread (thuyền nhà Tòng, người rời bến, quyền bến). Checklist tám slots nằm trong [Slice Content Contract](../content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md). Citation M28 §95–97 trong pack chỉ dẫn nguyên tắc compiler/QA; ngưỡng mới vẫn là prototype-authored, không gán cho nghiên cứu cũ.
