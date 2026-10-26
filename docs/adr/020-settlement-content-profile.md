# ADR-020 — Settlement content profile v1 (Sprint11)

Ngày2026-10-23. VS02A là phạm vi Sprint11: compiler/immutable registry cho authored settlement data, chưa khởi tạo authoritative WorldState hoặc chạy routine/event. VS02 full còn8–12event templates/eligibility và integration gates.

## Contract v1

Pack JSON strict, <=1MiB, depth<=20, duplicate/unknown/missing fields rejected. Envelope chỉ gồm `pack_id` (slice.settlement), `version` (1), `schema_version` (1), `profile` (settlement-content-v1), `definitions` (array), `localization` (object key→Vietnamese text). Không dependencies hoặc executable handler trong profile này; kernel-content-v1 không thay đổi.

Mỗi definition có đúng common fields: `definition_id` (DefinitionId syntax kind.slug@1), `version`1, `schema_version`1, `kind`, `source` (module/section hoặc prototype-authored), `classification` game-canon, `owner_domain`, `display_key`. Thêm fields theo kind dưới đây; keys ngoài danh sách bị từ chối. Nested objects cũng strict. String nonempty<=2048, arrays bounded<=256, definitions<=512, integers không float.

| kind / owner_domain | Fields riêng |
|---|---|
| community / Settlements | `region` (tam-giang/phong-chau), `external` bool |
| household / Households | `community_id`, `member_ids` array actor refs, `lots` array {lot_id,commodity,quantity,unit}, `rights` array {lot_id,holder_id,permission}, `obligations` array {to_household_id,description} |
| actor / Characters | `household_id`, `age`0..100, `alive`bool, `attributes` {the,xao,giac,tri,tam,khi} integers1..10, `history` array {history_id,at_age,description}, `skills` array {skill_id,value,history_id} value0..100, `known_facts` array {fact_id,history_id}, `needs` array strings, `routine` {description,community_id,skill_id,duration_ticks,executable}, `viability` {description,caregiver_id} |
| fact / Knowledge | `claim` text, `provenance` text; là authored perceived claim, không cấp World Truth/player knowledge |
| relationship / Social | `from_actor_id`, `to_actor_id`, `trust` integer -100..100, `description`; có hướng, không auto reverse |
| route / Travel | `from_community_id`, `to_community_id`, `mode` water/land, `duration_ticks` integer1..1000000, `capacity`integer1..1000000, `open_seasons` array unique dry/wet; có hướng, không auto reverse |

Lot_id và history_id/skill_id là stable local tokens regex `[a-z][a-z0-9_-]*`; lot_id global unique across households; skill_id ví dụ boat-repair, transport, medicine, farming, fishing, weaving, trading. Quantity integer0..1000000, commodity grain/timber, unit count; đây là prototype fixtures không historic quantities. Rights holder_id phải actor thành viên household sở hữu lot; permission use/manage. Obligations không self và target household tồn tại, descriptive không tự tạo debt transaction.

Actor membership hai chiều đúng một household; household ở local community. Alive/dead explicit, routine.executable luôn false trong profile này (không có runtime scheduler handler). Routine location local, skill_id có trong actor.skills, duration_ticks positive<=1440. Skills và known_facts trỏ history_id của chính actor, at_age<=age, history_id unique; history age không giảm. Known facts trỏ fact typed ref; không auto cấp cho actor khác. Needs>=1, history>=1, skills>=1, known facts>=1. Viability.caregiver_id dùng actor ref hoặc null; children age<16 phải có alive adult caregiver cùng household; dead không làm caregiver. Không tạo random protagonist từ roster này.

Community canon: community.ben-vac@1/community.con-may@1/community.van-nuoc@1 thuộc tam-giang externalfalse, community.ben-hac@1 thuộc phong-chau externaltrue. Exactly3local+1external; households6..8, actors20..30, routes6..10; relationship>=1 cho mỗi actor outgoing; không selfrelationship; duplicate directed pair rejected; route selfloops/duplicate endpoint+mode rejected; mỗi local community có incoming/outgoing edge và ít nhất một household. Closed season chỉ không có edge trong query; không tạo reverse, không giả arrival/info propagation.

## Output và determinism

Portable Infrastructure compiler nhận cây primitives Dictionary<string,object?>/object[] (host JSON adapter parse strict integers/no duplicate). Compiler tự kiểm shape/semantics dù gọi trực tiếp bỏ qua host. Trả immutable registry với ContentHash trên canonical mdc1 chứa profile/version, definitions sort ordinal definition_id và localization ordinal; giữ thứ tự nested arrays (history chronological, authored order của các arrays khác được bind trong hash). Không giữ mutable input aliases. Public query định nghĩa/by-kind và directed route seasonal filter read-only; không write WorldState. Không public player projection; caller UI chưa được quyền đọc registryomniscient.

Schema JSON và CLI `--settlement-compile <pack>` phải cùng profile; report hash/counts và không in hidden claims. Tests: real rostercompile, stablehash across processes/order of definitions, no alias mutation, reject malformed/unknown/duplicate/refs/wrongcanon/children/capabilityprovenance/rights/route/routine, no synthesized reverse edges, knowledge claims remain peractor. Whole VS02 stayspartial; event AST/handlers/threads/gameplay runtime là design gaps cho sprint sau, không tự gán unknown thành behavior.

## Rà contract trước implementation

Owner domains dùng đúng OWNERSHIP: Settlements/Households/Characters/Knowledge/Social/Travel. Lots/rights nhúng trong household là authored seed data do Assets sở hữu khi khởi tạo world về sau; compiler không chuyển quyền mutation sang Households. Profile này không ghi authoritative state. Definition prefix phải bằng kind và suffix@1 bằngversion. Mỗi lot ít nhất một right, duplicate(lot_id,holder_id,permission) bị từ chối. Actors chết được giữ như metadata lịch sử, routine luôn nonexecutable; child-caregiver requirement chỉ áp dụng alivechild, dead không là caregiver hoặc candidate sinh hoạt. Relationships là authored directed ties, không hành vi executable.

## Làm rõ dữ liệu authoring

Capacity có đơn vị prototype_load_unit cố định trong profile; chưa quy đổi sang cân nặng/người hoặc sử dụng để quyết định travel/trade trước VS03. Mỗi household có ít nhất một descriptive obligation; actors tham chiếu nghĩa vụ hộ qua household_id, chưa có actor-specific obligation runtime và không coi đây VS02 fullcomplete. Canonical hash chuyển localization thành array {key,text} sortordinal vì mdc1 không cho dấu chấm trong objectkeys; lookup Localization giữ map readonly. Nested arrays giữ authoredorder như trên.
