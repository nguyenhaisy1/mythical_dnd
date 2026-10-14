# Domain ownership và quy tắc phụ thuộc

Bảng này cụ thể hóa M25 §10, M26 và M29 cho scaffold hiện tại. Một domain là ranh giới logic, không bắt buộc một assembly. Đường gọi đọc dùng read-only contract; thay đổi xuyên domain phải được Application gom thành ChangeSet và kiểm invariants. Domain không gọi presentation, DB hoặc LLM.

| Domain | State sở hữu | Yêu cầu thay đổi | Invariant chủ chốt |
|---|---|---|---|
| Foundation | EntityId, DefinitionId, WorldTick, WorldRevision, RNG context | Application | Không tái dùng ID; monotonic revision; rng draws có context |
| Characters | Identity profile, capabilities, class evidence, life state, party membership | Training, life events, party actions | Actor chết không action; skill có nguồn; party capacity hợp lệ |
| Households | Membership, genealogy links, household obligations | Birth, marriage, migration, death | Membership có thời hạn; không worker chết đang available |
| Social | Directed relationship dimensions, social commitments, reputation claims | Dialogue, testimony, life events | Không scalar relationship thay tất cả; claim có provenance |
| Knowledge | Personal facts/claims, provenance, confidence, transmission history | Observation, teaching, testimony | Không secret leak; rumor loop không tạo độc lập nguồn giả |
| Assets | Item identity/condition, CommodityLot quantities, ownership/rights, custody | Production, repair, trade, loot, inheritance | Không negative/double-spend; owner khác holder; conserve transfers |
| Economy | Production plans, contracts, demand, price estimates | Work/craft/trade commands | Plan không sinh stock trước completion; time/capacity bounded |
| Travel | Current strategic location, RouteEdge truth, travel plans | Travel, migration, route closure | No teleport; directed costs; one route graph for trade/rumor |
| Settlements | Infrastructure identity/condition, community indicators | Construction, disaster, authorized repair | Resource/time constraints; aggregates không ghi đè named facts |
| Health | Injuries, bleeding, recovery, functional limits, vitality | Impact, accident, disease, treatment | One injury source; death request tới life state trong same critical commit |
| Combat | Encounter turns, tactical zones/slots, morale context, objectives | Combat actions/reactions | Không duplicate health; actor incapacitated không dùng action trái rule |
| Narrative | Story threads, event definitions references, historical-anchor state | Committed facts + narrative rules | Thread không sinh physical truth; no feedback recursion unbounded |
| Factions | Membership, leadership, policies, plans | Recruitment, politics, succession | Faction không là NPC omniscient; use institutional knowledge |
| Ecology | Population pools, habitats, persistent exceptional creatures | Hunt, migration, environment | Persistent death không respawn; aggregate/named population consistent |
| Belief | Belief profiles, ritual state, constrained Mạch state | Ritual and approved environmental commands | Ontology không phải knowledge; Mạch không ngoại lệ vô hạn |
| Legacy | Succession case, inheritance process, legacy links | Death, successor selection, valid transfers | Không copy profile; estate không gồm tài sản đang mượn |
| Application | Command receipts, schedule/jobs, commit coordination | Hosts and internal workflows | Single writer; atomic publish; idempotency; bounded fan-out |
| Projections | Quest view, Journal, Codex, Chronicle, UI read models | Committed event consumption | Rebuildable, idempotent, filtered by active actor; no authority |

## Giao dịch nhiều domain

Sửa thuyền: Economy xác lập work/recipe, Characters cung cấp skill, Assets kiểm quyền dùng thuyền/công cụ/vật liệu và ghi condition/stock, WorldClock/Scheduler quản lý thời lượng. Narrative chỉ nhận sự kiện đã commit. Không gọi thẳng Quest để báo hoàn thành.

Death: Health xác định tử vong cần xảy ra; Characters life state và party action eligibility phải đổi cùng critical transaction. Jobs của actor được cancel/revalidate; inventory vẫn có identity/custody hợp lệ. Estate/rumor/grief ở xa có thể xử lý qua durable jobs sau đó, không cần gom cả thế giới vào một transaction.

Household food: Economy lập consumption proposal; Assets ghi lot quantity. Survival dự báo supply từ lots, không tự trừ một counter food khác. Trade route dùng Travel read model, không tạo graph riêng.

## Dependency rule

Foundation không phụ thuộc domain. Domain chỉ phụ thuộc Foundation. Application phụ thuộc Domain/Foundation. Infrastructure triển khai ports của Application/Domain. Headless host compose Application và Infrastructure. Unity adapter chỉ gọi Application public API; composition root có thể inject Infrastructure qua adapter, không đưa SQLite vào UI/domain.

Factions, Settlements, Ecology và Belief bổ sung folder logic để không mất các subsystem đã có trong GDD. Chưa tách thành assembly riêng trước khi có lý do thực tế.

## Activity fields bổ sung

Repair activity/production plan thuộc Economy; lot on_hand/reservations và item condition/rights thuộc Assets. Application sở hữu scheduled jobs/command receipts; Foundation sở hữu allocator/tick/RNG. Rejection metadata là Infrastructure persistence dưới Application policy. Xem [Command Catalog](COMMAND-ACTIVITY-CATALOG.md); không để handler sửa thuyền trực tiếp sửa Health hoặc dùng Economy làm kho vật chất thứ hai.
