# Canonical data model và mức độ schema

Đọc toàn bộ Module 26 khi hiện thực hóa; nó giữ các bảng và logical records đã nghiên cứu. Đọc Module 29 cho envelope, job, save manifest và registry. Không tự dịch từng logical record thành bảng SQL một cách máy móc.

| Nhóm | Records / dữ liệu trọng tâm | Owner | Contract kiểm tra |
|---|---|---|---|
| World | Region, Settlement, LocalArea, RouteEdge, time/revision | World/Travel/Settlements | IDs ổn định; route endpoints tồn tại; directed cost |
| People | Character, LifeHistory, Household, Lineage, capability/evidence | Characters/Households | skills có nguồn; life state nhất quán |
| Perception | KnowledgeEntry, Claim, Observation, Memory, transmission | Knowledge/Social | provenance; actor visibility; rumor dedupe |
| Resources | ItemInstance, CommodityLot, ownership/custody/rights | Assets | quantity không âm; quyền dùng khác quyền sở hữu |
| Work | Profession, WorkPlan, Recipe, contract, completion jobs | Economy/Characters | thời gian/vật liệu/capacity; không instant output |
| Conflict | EncounterSpace, TacticalZone, Slot, action, morale | Combat | chỉ đọc wound từ Health; mục tiêu và surrender |
| Health | Wound, treatment, recovery, incapacitation | Health | dài hạn và trong combat cùng một state |
| Narrative | EventDefinition, EventOccurrence, StoryThread, Anchor | Narrative | eligible khác committed; no physical retcon |
| Persistence | Receipt, EventEnvelope, Snapshot, Manifest, command log | Application/Infrastructure | consistent revision, version/hash, idempotency |

## Trạng thái các JSON trong repository

`content/fixtures/vertical-slice.reference.json` là fixture **thiết kế**, giúp thống nhất IDs, địa lý và các nhánh cần thực hiện. Schema của nó chỉ mô tả đúng fixture tham chiếu, không giả làm schema của cả game. Nó chưa có đủ roster, genealogy, events và resource recipes để chạy full simulation.

`content/schemas/design-fixture.schema.json` dùng JSON Schema để mô tả cấu trúc. Validator Python kèm theo kiểm tra tập invariant cần cho fixture hiện tại: duplicate IDs, parent regions, endpoints và actor-household references. Nó không phải runtime content compiler và không chứng minh causal eligibility toàn bộ event.

Runtime pack phải có manifest chứa pack ID, version/schema, dependencies, definition IDs, rule references, historical classification, content hash và validation metadata (M29 §76). Chỉ làm runtime schemas khi implementing entity contracts tương ứng để tránh hai bộ schema không đồng bộ.

## Contract baseline 1.1

Codec và numbers theo [Math/RNG Spec](DETERMINISTIC-MATH-RNG-SPEC.md); identity theo [Identity Spec](IDENTITY-SPEC.md); storage tables/compatibility theo [Save Spec](PERSISTENCE-SAVE-SPEC.md). Kernel fixture là projection contract, không full snapshot. [Slice Content Contract](../content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md) xác định records, units, invariants và gate trước compile. Runtime JSON Schemas/DDL sinh cùng FND-14/11 phải khớp specs; hiện không được gọi reference schema là production validator.
