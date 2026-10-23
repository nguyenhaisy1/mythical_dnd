# Hướng dẫn sản xuất nội dung

## Từ nghiên cứu tới runtime pack

World Bible/GDD → authored definitions → schema validation → semantic/reference validation → rule eligibility tests → build manifest/hash → approved pack → runtime registry. Không load văn bản GDD trực tiếp như script. Runtime LLM không phải bước trong pipeline này.

Mỗi definition cần ID không phụ thuộc display name, type, schema/version, source module/section, lịch sử/ truyền thống/game-canon classification, references và validation status. Event cần actor roles, địa điểm/thời gian, hard preconditions, resource requirements, allowed handlers, outcomes, delayed effects, knowledge/witness policy và cooldown/novelty constraints nếu có.

## Quyền và nhân quả

Content không tự ghi arbitrary state path, gọi SQL hoặc nhúng code. Event template tham chiếu handler và typed parameters. Handler phải kiểm actor/resource/location/time/knowledge prerequisites. Nội dung “thợ sửa thuyền” không tự cấp skill/tool; asset “thuyền gia đình” không tự là tài sản cá nhân người đang lái.

Event occurrence cần ID và causal source. Story Thread tổ chức hậu quả từ committed events; Quest View theo actor knowledge. Description phải khớp facts đã có; tin đồn được đánh dấu claim, không tự ghi đè World Truth.

## Historical/source review

Địa danh/persona hư cấu phải ghi game-canon. Chất liệu truyền thuyết không được ghi chắc chắn là lịch sử. Các citation token phiên chat cũ chỉ giữ trong archive; trước xuất bản cần lập URL/source evidence thật. Không tự tải/đưa copyrighted asset vào game vì được nhắc trong nghiên cứu.

## Chuẩn bị VS-02

Reference fixture hiện giúp khóa IDs và phạm vi địa lý. Author roster/households, capability histories, rights/stock, route direction/cost, 8–12 event templates, 3 threads và ba profession activities. Kiểm tra no-intervention branch trước khi viết thêm dialogue. Không coi 20 NPC chỉ có name/stat là world simulation đủ.

## Checklist duyệt content

Schema hợp lệ; IDs/references duy nhất và tồn tại; owner domain đúng; conservation/time constraints; có provenance cho knowledge; hidden facts không nằm trong player-facing text; branch không đạt phải báo rõ; deterministic test có seed/versions; outcome đúng semantic. Final pack phải pin rule/content versions và không tự cập nhật giữa campaign.

## Gate chuyển nghiên cứu sang dữ liệu

Đọc [Slice Content Contract](VERTICAL-SLICE-CONTENT-CONTRACT.md) cho record fields, validation, provenance và parameter gate trước VS-03. Full roster/compiled pack vẫn là deliverable VS-02; kernel fixture không thay thế production content.

## Profile runtime Sprint 8

[Content Pipeline](../runbooks/CONTENT-PIPELINE.md) và [ADR-017](../adr/017-runtime-content-registry.md) xác định compiler/registry hiện có cho commodity và transfer-template. Schema `content/schemas/kernel-content-v1.schema.json`; fixture `content/fixtures/content-kernel/pack.json` là prototype-authored/game-canon chạy được. Compile tạo registry trong bộ nhớ, chưa ghi binary vào content/compiled. Các loại NPC/household/routes/recipes/event AST chưa có runtime contracts đầy đủ nên bị từ chối rõ; full production roster vẫn thuộc VS-02. Reference fixture không tự trở thành runtime pack.

## Profile Sprint11 — VS02A

[ADR020](../adr/020-settlement-content-profile.md) và [Settlement Content](../runbooks/SETTLEMENT-CONTENT.md) bổ sung profile riêng settlement-content-v1 cho communities/households/actors/claims/relationships/routes. Kernel-content-v1 vẫn từ chối các kinds này; không nới compiler cũ. Pack24actors/6households/8routes nằm content/definitions/vertical-slice; compile thành registry readonly, chưa WorldState.

Routine là mô tả nonexecutable; lot/rights là Assets seeddata, household obligations descriptive, capacity prototype_load_unit chưa transportsemantics. FullVS02 phải bổ sung8–12events/causaleligibility và actor-specificobligations; TEST001randomprotagonist chưa triển khai. Không tự dùng registryomniscient làm playerprojection hoặc NPCperception.
