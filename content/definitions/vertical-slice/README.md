# Bến qua mùa nước — settlement-content-v1

`pack.json` là authored data được compiler C# kiểm và tạo immutable registry trong bộ nhớ. Không phải WorldState, save game hoặc mô phỏng NPC. Không có executable routine, event handler, random protagonist hoặc player projection trong profile này.

## Nội dung đã author

| Hộ | Nơi thuộc về | Nhân vật | Vai trò thiết kế |
|---|---|---|---|
| Vạc Tòng | Bến Vạc | Tòng, Mộc, Mai, Lâm, Bà Nhu | Sinh kế đò, trách nhiệm và khác biệt niềm tin ba thế hệ |
| Triều Khê | Vạn Nước | Khê, Lục, Hiên, Bé | Đời sông nước và nhu cầu ổn định gia đình |
| Mây Đan | Cồn Mây | Đan, Rìu, Sợi, Cháu | Học nghề sửa thuyền, chất lượng và áp lực nhận việc |
| Bến Sen | Bến Vạc | Sen, Hà, Kiên, Ngô | Chăm sóc, riêng tư, vận chuyển người cần giúp |
| Cồn Sậy | Cồn Mây | Sậy, Lan, Út, Bà | Bãi mùa vụ, thu dọn khi nước cao và người muốn đi xa |
| Nước Thoa | Vạn Nước | Thoa, Nghiêm, Nhỏ | Lời hẹn giao thương, tải hữu hạn và truyền lời đúng |

24actors/6households/4communities (3local+externalBếnHạc)/8directedroutes/24claims/24directedrelationships. Mỗi actor có2lifeepisodes, skill với historyref,1claim có nguồn riêng, nhu cầu, routine mô tả và viability/caregiver. Đây là authored prototype roster, không phải cơ chế sinh nhân vật ngẫu nhiên TEST001.

## Nguồn và canon

- M03 TG01–03: Bến Vạc, Cồn Mây theo mùa, Vạn Nước là cộng đồng sống trên thuyền. Node Vạn Nước biểu diễn identity cộng đồng, không ấn định địa điểm vĩnh viễn; graph là snapshot authoring, không tự xử lý migration.
- M04 XXVII/F01: năm thành viên nhà Vạc Tòng và tuổi58/31/28/22/77; M05 F01 bổ sung mâu thuẫn Tòng/Mộc.
- M05 XX/23–24: Triều Khê35 và mẹ Triều Lục61; các phần mở rộng gia đình, lịch sử, skill/attributes và số lượng đều ghi prototype-authored.
- Bến Hạc thuộc Phong Châu/external theo baseline DEC019. Không di chuyển Bến Hạc sang Tam Giang cho tiện nối tuyến.
- Toàn bộ record classification game-canon; không có khẳng định khảo cổ/lịch sử mới. Source field phân biệt nguồn ý tưởng và chi tiết author mới, không gán số balance cho nghiên cứu.

Claims chỉ được gắn cho actor trong known_facts; provenance là ký ức/nhận định, không tạo world truth. Localization và registry dùng cho authoring/QA, không tự an toàn để đưa toàn bộ vào UI. Trust là một trục dữ liệu prototype, không thay mọi chiều quan hệ Social của GDD.

Lots/rights trong hộ là seed data của Assets khi world initialization được đặc tả; inventory hữu hạn, không có thứ hai authoritative stock. Capacity của route là prototype_load_unit, chưa có quy đổi hay transport rules; duration là tickphút theo clock-v1. Nghĩa vụ đang ở cấp hộ, không tự khởi tạo hợp đồng/debt hoặc actor-specific obligations.

## Compile và kiểm tra

```bash
dotnet run --project engine/HeadlessRunner -c Release -- --settlement-compile content/definitions/vertical-slice/pack.json
```

Schema: `content/schemas/settlement-content-v1.schema.json`. [ADR020](../../../docs/adr/020-settlement-content-profile.md) định nghĩa compiler semantics vượt JSON Schema: membership hai chiều, caregiver, rights, provenance, typedrefs, canon và directedgraph. CLI chỉ in IDs/counts/hash, không in nội dung claims. Artifact compiled không commit.

ContentHash reviewed baseline: `60ce05463edcc1c2e4abbeae8f30159d430f57613b902ec6d48bb775db193ec8`. Không overwrite expected hash để làm test qua khi chưa review thay đổi nội dung. Nested arrays giữ authored order; definitions và localization order không đổi hash.

VS02 còn thiếu8–12eventtemplates/causaleligibility, actor-specificobligations và random protagonist; VS03/04 mới triển khai nghề, travel, knowledge truyền tin và NPCautonomy. Đủ records không là gameplay acceptance.
