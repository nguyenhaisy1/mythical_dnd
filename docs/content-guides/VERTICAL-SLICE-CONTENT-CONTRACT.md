# Vertical Slice Content & Prototype Parameter Contract

Phạm vi VS-02/03/04/08/09; OPEN-05/07/08. Nguồn M28, [canon](../governance/CANON-REGISTER.md), [slice](../production/VERTICAL-SLICE.md). Mục tiêu là chuyển nghiên cứu thành dữ liệu có thể compile; không coi danh sách tên là pack đã playable.

## Layout và delivery gate

Authoring tại `content/definitions/vertical-slice/`: manifest, households, actors, relationships, routes, resources, recipes, event-templates, threads, encounters, historical-anchors. Mỗi record có definition_id/version, source module/section hoặc `prototype-authored`, owning domain, localized display key và schema_version. Prototype coefficients tại [kernel-v1.json](../../rules/parameters/kernel-v1.json). Runtime compiler FND-14 phải validate schema/refs/handler allowlist trước tạo `content/compiled/`; chưa có compiler không gắn nhãn runtime pack.

VS-02 exit: 20–30 actor khác biệt, 6–8 household, 3 local communities Bến Vạc/Cồn Mây/Vạn Nước, 6–10 directed edges, 8–12 event templates. Bến Hạc giữ external boundary Phong Châu. Mỗi actor có household, job/routine, needs, known facts, obligation, relationship và viability; không sinh class theo random label. Full roster chưa author trong lượt bổ sung đặc tả; việc chuyển canon sang content và balance vẫn là task VS-02, không che bằng filler NPC.

## Record fields và invariants

| Record | Required data | Semantic validation |
|---|---|---|
| Household | members, location, lots, rights, obligations | actor membership có hiệu lực một household chính; inventory hữu hạn |
| Actor | age, alive, attributes6, skills, history, perceived facts, routine | capability có provenance; dead không có work plan executable |
| Route | from/to, mode, duration_ticks, capacity, seasonal_conditions | có hướng; không tự tạo reverse edge; closed route không trade/info travel |
| Resource/recipe | unit, quantity range; inputs, tools, duration, outputs, handler | conservation; positive duration; tools/capability thật; no arbitrary scripts |
| Event template | condition AST, eligible actors, resource constraints, approved handler, cooldown, information route | eligibility trước RNG; không tạo vật chất từ narrative; không event dựa player proximity |
| Thread | initiating facts, observations, success/failure predicates, independent NPC actions | quest display không sở hữu physical truth; no-player vẫn diễn tiến |
| Encounter | zones, local slots, entry/exit/surrender, actor intents, injury rules | Health owns injuries; victory không bắt buộc giết tất cả |
| Anchor | hard/flexible/emergent, prerequisites, alternatives, impossible-state policy | hard không cưỡng outcome khi prerequisites bất khả thi; trace exposure riêng |

Conditions dùng declarative approved expression nodes (All/Any/Not/Compare/HasKnownFact/HasResource), không eval C#/Python từ JSON. Compiler type-check owner read permissions; actor decision condition đọc perceived state. Text localization không dùng như predicate. Numeric values có units/ranges và reference parameter key, không copy coefficients vào nhiều records.

## Ba nghề và kịch bản tối thiểu

Sửa thuyền dùng reserve/consume contract; vận tải cần route mở, capacity, cargo lots, rights, departure/arrival jobs và spoilage/loss records; chữa trị cần consent/access, medical knowledge, supplies, Health injury và follow-up job. Trước VS-03 merge từng nghề phải có typed command/payload/errors/deltas như repair, rồi fixtures success/cancel/death/resource-loss. Không tái dùng completion handler nếu ownership khác.

8 template slots tối thiểu: deadline vận tải; thuyền xuống cấp; mưa làm gián đoạn; thiếu lương thực; rumor đến bến; tranh chấp quyền bến; thương tích cần điều trị; NPC tự thuê sửa. Mỗi slot cần condition, handler, cooldown, 1 eligible và1 ineligible fixture. Đây là production checklist, không phải event data đã tồn tại. Ba thread liên kết những facts này; chơi không can thiệp phải vẫn có logistics/repair/rumor consequences. Không viết outcome text trước rồi buộc state theo.

## Parameters và tuning

kernel-v1 chỉ có thông số đủ fixture: ticks/day1440, repair duration60, timber2, gain200, conditionmax1000, schedulerbudget1000/fanout32. Các giá trị là engineering prototype, không có khảo cứu lịch sử hoặc balance claim. Initial parameters cho transport/medicine phải author và review **trước VS-03**, không đợi VS-10. VS-10 là tune dựa measurements, không là nơi phát minh units lần đầu. Thêm field yêu cầu versioned schema, vectors, rules consumer và test expectations cùng thay đổi.
