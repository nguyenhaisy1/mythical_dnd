# Module 26 — Technical System Design và Canonical Data Model

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T069](../../source/conversation.md#t069)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Logical schema là contract, chưa phải physical SQLite schema. Core thuần C#; single authoritative writer. Canonical naming/ownership của gói này có hiệu lực với scaffold. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Design modules, entity identities, temporal semantics và domain boundaries. |
| Đầu ra / bên sở hữu | Canonical data contracts, command/events, read projections và storage ports. |
| Đọc cùng | 25, 27–29; docs/contracts.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. ADR-019 và docs/runbooks/UNITY-CONFORMANCE.md cho exact Editor profile và giới hạn nghiệmthu. ADR020 và docs/runbooks/SETTLEMENT-CONTENT.md: Sprint11 VS02A authored roster/graph compiler, không WorldState/routineexecution. ADR021 và docs/runbooks/EVENT-ELIGIBILITY.md: VS02B declarative candidate eligibility trên trusted observations. |
| Cổng nghiệm thu | Stable IDs, no hidden knowledge leakage, nonnegative stock và revision consistency có test trước feature breadth. |
| Việc chưa được chứng minh | Sprint10/ADR019: VS01 kernel và FND16 Unity6000.6.7f1 LinuxEditor Mono đã có actual conformance evidence. Player/IL2CPP, Unity crash/recovery/migration, production gameplay/content và performance còn gates riêng; xem docs/qa/SPRINT-10-EVIDENCE.md. VS02A có24actors/6households/8routes compiled authoring; VS02 full còn world/perception binding và event effects, randomprotagonist và actor-specificobligations. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T069 BEGIN -->

<a id="m26-t069-s001"></a>

## GDD MODULE 26
<a id="m26-t069-s002"></a>

## TECHNICAL SYSTEM DESIGN & CANONICAL DATA MODEL

**Dự án:** Living World Narrative RPG — cuối Văn Lang, chuyển tiếp Âu Lạc, Cổ Loa  
**Loại tài liệu:** Technical Architecture Specification / Canonical Data Model / Internal Contract Design  
**Phiên bản tài liệu:** 1.0 — Architecture Baseline  
**Trạng thái:** Đề xuất khóa kiến trúc để chuyển sang prototype  
**Module phụ thuộc:** Toàn bộ GDD Module 01–25  
**Module kế tiếp:** Module 27 — Rule Engine & Formula Specification

---

<a id="m26-t069-s003"></a>

## PHẦN I — ARCHITECTURAL BASELINE

<a id="m26-t069-s004"></a>

### 1. Mục tiêu kỹ thuật của engine

Hệ thống phải vận hành một thế giới có các đặc điểm:

- Nhiều nghìn nhân vật với mức độ mô phỏng khác nhau.
- Các cộng đồng, nghề nghiệp, faction và quan hệ có khả năng thay đổi.
- Người chơi có thể tham gia hoặc không tham gia một sự kiện.
- Character có lịch sử, tri thức, tài sản và những giới hạn riêng.
- Sự kiện có nguồn gốc nhân quả và tác động lâu dài.
- Thế giới có lịch sử kéo dài khoảng 20–25 năm trong một campaign.
- Nhân vật có thể chết và quyền điều khiển chuyển sang successor.
- Nội dung có thể được mở rộng bằng các Content Packs.
- Gameplay không yêu cầu gọi LLM trong runtime.
- Save/Load và các hành động quan trọng có thể được kiểm thử tái hiện.

**Engine không cần mô phỏng từng hành vi của mọi người theo từng giây.** Nó cần mô phỏng đủ chính xác những thay đổi có giá trị về gameplay, lịch sử và nhân quả.

Đây là nguyên tắc quyết định cấu trúc dữ liệu, event scheduler và simulation levels.

<a id="m26-t069-s005"></a>

### 2. Các thuộc tính kiến trúc bắt buộc

| Thuộc tính | Yêu cầu | Cách kiểm chứng |
|---|---|---|
| Consistency | Không có committed state mâu thuẫn | Invariant tests |
| Determinism | Cùng đầu vào tạo cùng authoritative state trong phiên bản tương thích | Replay tests |
| Causal Traceability | Sự kiện quan trọng có nguồn gốc và quy tắc xử lý | Event audit |
| Scalability | Có thể tăng quy mô thế giới bằng simulation tiers | Headless benchmarks |
| Extensibility | Thêm content không yêu cầu sửa core cho từng event | Content pack tests |
| Modularity | Domain có quyền sở hữu dữ liệu và public contracts | Architecture tests |
| Persistence | Lưu/tải không làm mất hoặc nhân đôi sự kiện | Recovery tests |
| Knowledge Integrity | Actor không tự biết thông tin chưa có nguồn | Information-flow tests |
| Narrative Consistency | Presentation không thay đổi sự thật gameplay | Projection validation |
| Offline Operation | Game vận hành không cần LLM hoặc backend online | Offline acceptance test |

Các thuộc tính này phải được kiểm tra tự động ở những mức phù hợp, không chỉ đánh giá qua cảm nhận khi chơi.

<a id="m26-t069-s006"></a>

### 3. Architectural Decision Record tổng quát

| ID | Quyết định | Trạng thái |
|---|---|---|
| ADR-026-01 | Modular Monolith cho game runtime | Đề xuất khóa |
| ADR-026-02 | Headless authoritative simulation core | Đề xuất khóa |
| ADR-026-03 | Single ordered gameplay mutation pipeline | Đề xuất khóa |
| ADR-026-04 | Canonical entity identities | Đề xuất khóa |
| ADR-026-05 | Một authoritative owner cho mỗi loại state | Đề xuất khóa |
| ADR-026-06 | Event-driven cross-domain integration | Đề xuất khóa |
| ADR-026-07 | Deterministic RNG và ruleset versioning | Đề xuất khóa |
| ADR-026-08 | Hybrid state, event journal và snapshot | Đề xuất khóa |
| ADR-026-09 | Actor knowledge tách khỏi World Truth | Đề xuất khóa |
| ADR-026-10 | LLM chỉ hỗ trợ sản xuất nội dung offline | Đã thống nhất về nguyên tắc |
| ADR-026-11 | Content schema và validation bắt buộc | Đề xuất khóa |
| ADR-026-12 | Headless testing trước khi mở rộng content | Đề xuất khóa |

Những quyết định về ngôn ngữ lập trình, engine đồ họa và hệ quản trị lưu trữ cần được xác nhận qua prototype kỹ thuật, không nên nhầm với các quy tắc gameplay không thể thay đổi.

---

<a id="m26-t069-s007"></a>

## PHẦN II — LỰA CHỌN TECH STACK

<a id="m26-t069-s008"></a>

### 4. Ba phương án phù hợp

<a id="m26-t069-s009"></a>

#### Phương án A — Unity + C# Simulation Core

Cấu trúc:

**C# Domain Libraries → Simulation Core → Unity Presentation Adapter.**

Ưu điểm:

- Một ngôn ngữ chính cho engine và gameplay.
- Có thể tổ chức các thư viện .NET thuần để chạy test không cần Unity.
- Unity phù hợp với game 2D, UI và tactical scene.
- Có thể tách simulation khỏi scene lifecycle.

Rủi ro:

- Dễ để gameplay logic phụ thuộc MonoBehaviour nếu thiếu kỷ luật kiến trúc.
- Physics, animation timing và các phép toán số thực cần được cô lập khỏi những quyết định đòi hỏi determinism.
- Một số API framework có thể thay đổi theo phiên bản engine.

**Đánh giá:** Phù hợp nếu team có năng lực C# và Unity.

<a id="m26-t069-s010"></a>

#### Phương án B — Godot + C# hoặc GDScript

Cấu trúc:

**Headless Simulation Core → Godot Application/Rendering Layer.**

Ưu điểm:

- Phù hợp triển khai giao diện 2D và các scene nhẹ.
- Có khả năng tách gameplay logic khỏi rendering.
- Linh hoạt với quy mô prototype nhỏ.

Rủi ro:

- Cần xác nhận pipeline build và các nền tảng mục tiêu.
- Nếu dùng C# và GDScript đồng thời, phải tránh chia nguồn sự thật gameplay sang hai ngôn ngữ.
- Các công cụ authoring chuyên sâu vẫn phải tự phát triển.

**Đánh giá:** Khả thi, đặc biệt với đội nhỏ có kinh nghiệm Godot.

<a id="m26-t069-s011"></a>

#### Phương án C — Core C++/Rust + Presentation Engine

Ưu điểm:

- Kiểm soát sâu về data layout, memory và performance.
- Có thể thiết kế simulation runtime độc lập.
- Phù hợp nếu hệ thống headless simulation có yêu cầu đặc biệt cao.

Rủi ro:

- Tăng chi phí phát triển.
- FFI và serialization giữa core và presentation cần nhiều công sức.
- Độ phức tạp có thể không cần thiết cho giai đoạn đầu.

**Đánh giá:** Chưa có cơ sở để ưu tiên.

<a id="m26-t069-s012"></a>

### 5. Khuyến nghị

**Kiến trúc tham chiếu: C# headless domain core + Unity presentation.**

Tôi chọn hướng này làm mẫu kỹ thuật cho các hợp đồng tiếp theo, không phải vì GDD chỉ có thể triển khai trên Unity.

Nếu đội phát triển sử dụng Godot hoặc một engine khác, các hợp đồng Domain, Command, Event và State Model phải giữ nguyên về mặt ngữ nghĩa.

Điểm cần tránh là viết simulation trực tiếp trong các script UI hoặc object scene.

**Scene không được là nguồn sự thật của World State.**

Một NPC không được chỉ tồn tại vì GameObject của họ đang được load.

Một settlement không được ngừng mô phỏng vì scene chưa được mở.

Một item không được mất khỏi inventory chỉ vì object hình ảnh bị destroy.

<a id="m26-t069-s013"></a>

### 6. Đề xuất hệ lưu trữ

Đối với single-player offline prototype:

**SQLite** là một ứng viên phù hợp để lưu dữ liệu có quan hệ, snapshot metadata, event journal và những state cần truy vấn.

Content có thể được xây dựng từ JSON/YAML rồi được kiểm tra, compile và đóng gói thành dữ liệu runtime tối ưu.

Không nên coi JSON text là định dạng tốt nhất cho toàn bộ gameplay runtime.

Cũng không nên tạo PostgreSQL Server, Redis, Kafka và hàng chục service chỉ để chạy một world simulation đơn người chơi.

Nếu về sau có multiplayer, cloud synchronization hoặc cộng đồng mod lớn, kiến trúc lưu trữ có thể cần thêm những giải pháp tương ứng.

Nhưng hiện tại không nên thêm chúng vào critical path.

---

<a id="m26-t069-s014"></a>

## PHẦN III — SOURCE CODE ARCHITECTURE

<a id="m26-t069-s015"></a>

### 7. Cấu trúc repository đề xuất

```text
game/
│
├── docs/
│   ├── gdd/
│   ├── architecture/
│   ├── adr/
│   ├── contracts/
│   └── testing/
│
├── src/
│   ├── Foundation/
│   │   ├── Identity/
│   │   ├── Time/
│   │   ├── Random/
│   │   ├── Commands/
│   │   ├── Events/
│   │   ├── Rules/
│   │   ├── Validation/
│   │   └── Serialization/
│   │
│   ├── Domain/
│   │   ├── World/
│   │   ├── Characters/
│   │   ├── Households/
│   │   ├── Social/
│   │   ├── Knowledge/
│   │   ├── Inventory/
│   │   ├── Economy/
│   │   ├── Settlement/
│   │   ├── Faction/
│   │   ├── Travel/
│   │   ├── Ecology/
│   │   ├── Combat/
│   │   ├── Narrative/
│   │   ├── Belief/
│   │   └── Legacy/
│   │
│   ├── Application/
│   │   ├── CommandProcessor/
│   │   ├── SimulationScheduler/
│   │   ├── EventDispatcher/
│   │   ├── ProjectionService/
│   │   ├── SaveLoad/
│   │   └── WorldDirector/
│   │
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   ├── ContentLoader/
│   │   ├── ContentCompiler/
│   │   ├── Logging/
│   │   └── Diagnostics/
│   │
│   ├── Presentation/
│   │   ├── GameClient/
│   │   ├── UI/
│   │   ├── WorldMap/
│   │   ├── CombatView/
│   │   ├── DialogueView/
│   │   └── NarrativeRenderer/
│   │
│   └── HeadlessRunner/
│
├── content/
│   ├── definitions/
│   ├── characters/
│   ├── settlements/
│   ├── professions/
│   ├── events/
│   ├── dialogue/
│   ├── historical_anchors/
│   └── localization/
│
├── tools/
│   ├── ContentEditor/
│   ├── RuleInspector/
│   ├── NPCInspector/
│   ├── WorldTimelineInspector/
│   └── ReplayDebugger/
│
└── tests/
    ├── Unit/
    ├── Integration/
    ├── Property/
    ├── DeterministicReplay/
    ├── ContentValidation/
    ├── HeadlessSimulation/
    └── Performance/
```

Đây là cấu trúc logical. Không có yêu cầu mỗi thư mục phải trở thành một assembly.

Tuy nhiên, cần thiết lập automated architecture tests để bảo đảm các domain không import Presentation hoặc các implementation details bị cấm.

<a id="m26-t069-s016"></a>

### 8. Dependency Rules

**Foundation:** Không phụ thuộc UI hoặc nội dung game cụ thể.

**Domain:** Chứa entities, value objects, domain rules và hợp đồng cần thiết; không import Unity scene, renderer hoặc LLM client.

**Application:** Điều phối commands, transactions, scheduling và domain integrations.

**Infrastructure:** Cung cấp implementation cho persistence, content loading, logging, file system.

**Presentation:** Đọc read models, gửi commands và hiển thị kết quả.

**Tools:** Sử dụng các interfaces phù hợp để tạo, kiểm tra và quan sát state.

Không được có dependency:

`Domain → Presentation`.

Không được có:

`Rule Engine → World Director → LLM → World State Mutation`.

<a id="m26-t069-s017"></a>

### 9. Điều kiện để core chạy headless

Một bản build chỉ chứa Foundation, Domain, Application và Infrastructure cần có khả năng:

1. Khởi tạo thế giới.
2. Sinh character.
3. Thực hiện commands.
4. Xử lý world time.
5. Chạy NPC decisions.
6. Sinh và giải quyết events.
7. Lưu/tải state.
8. Xuất diagnostic report.
9. Thực hiện deterministic replay.

Không cần load texture, scene, animation hoặc audio.

Nếu simulation chỉ hoạt động khi Unity scene đang mở, kiến trúc chưa đạt yêu cầu.

---

<a id="m26-t069-s018"></a>

## PHẦN IV — CANONICAL DATA MODEL: CÁC QUY ƯỚC NỀN

<a id="m26-t069-s019"></a>

### 10. Một entity phải có identity ổn định

Mọi đối tượng cần tồn tại qua thời gian hoặc được tham chiếu bởi domain khác phải có ID ổn định.

Ví dụ:

- Character;
- Household;
- Settlement;
- Faction;
- Route;
- Item;
- Story Thread;
- Historical Event;
- Ownership Claim.

Tên hiển thị không được dùng làm khóa chính.

Hai người có thể cùng tên.

Một người có thể đổi tên.

Một settlement có thể đổi tên sau nhiều năm.

Một item có thể được đặt tên mới khi truyền qua các thế hệ.

Identity phải độc lập với cách gọi.

<a id="m26-t069-s020"></a>

### 11. Phân biệt Definition ID và Instance ID

Đây là quy tắc rất quan trọng.

**Definition ID:** Xác định loại nội dung đã được thiết kế.

Ví dụ:

`weapon.spear.bronze_01`

`event.transport_damage_01`

`profession.river_transport`

**Instance ID:** Xác định một đối tượng thực sự trong world.

Ví dụ:

`item.00002831`

`event.00001872`

`character.00000931`

Một loại giáo có thể có hàng nghìn item instance.

Một Event Template có thể được sử dụng nhiều lần.

Nếu không tách Definition ID và Instance ID, game rất dễ nhầm một mẫu nội dung với một đối tượng đã tồn tại.

<a id="m26-t069-s021"></a>

### 12. ID Generation

Đề xuất sử dụng một cơ chế cấp ID xác định bởi engine.

Ví dụ:

`EntityID = DomainPrefix + PersistentCounter`

Counter cần được lưu và cấp trong cùng authoritative mutation pipeline.

Ưu điểm:

- dễ debug;
- ổn định;
- không cần nguồn random ngoài;
- có thứ tự tạo entity hợp lệ.

Có thể sử dụng UUID hoặc những phương án khác nếu xác định rõ cách cấp và replay.

Không nên dựa trên timestamp hệ điều hành để quyết định identity của gameplay entity.

<a id="m26-t069-s022"></a>

### 13. Entity, Value Object, Reference và Projection

**Entity:** Có identity và vòng đời.

Ví dụ Character.

**Value Object:** Được định nghĩa bằng giá trị, không có identity riêng.

Ví dụ một khoảng thời gian, tọa độ hoặc số lượng commodity.

**Reference:** ID trỏ tới entity khác.

**Projection:** Một góc nhìn được dựng từ dữ liệu authoritative.

Ví dụ:

- Quest View;
- NPC Profile;
- Map Knowledge;
- Chronicle;
- Settlement Summary.

Projection không được tạo một bản sự thật riêng cạnh tranh với World State.

<a id="m26-t069-s023"></a>

### 14. Những trường dữ liệu nền khuyến nghị

Với các persistent entity:

```text
EntityID
EntityType
StateVersion
CreatedAtWorldTime
LifecycleState
```

Tùy domain có thể thêm:

```text
DefinitionID
CurrentLocationID
LastUpdatedWorldTime
OriginEventID
```

Không phải entity nào cũng phải chứa mọi trường.

Ví dụ một Relationship Edge không nhất thiết có `CurrentLocationID`.

Các trường phải phục vụ quy tắc thực tế, không ép mọi object dùng một superclass khổng lồ.

<a id="m26-t069-s024"></a>

### 15. State Version và Ruleset Version khác nhau

**Entity StateVersion:** Phiên bản thay đổi của một entity, dùng trong kiểm tra stale write hoặc diagnostics.

**SchemaVersion:** Phiên bản cấu trúc dữ liệu.

**RulesetVersion:** Phiên bản các công thức/handler gameplay.

**ContentPackVersion:** Phiên bản các definitions, templates và văn bản.

**SaveFormatVersion:** Phiên bản định dạng lưu trữ.

Các version này phải được quản lý độc lập.

Một sửa đổi câu chữ không nhất thiết làm RulesetVersion thay đổi.

Một thay đổi công thức combat không nhất thiết làm SchemaVersion thay đổi.

---

<a id="m26-t069-s025"></a>

## PHẦN V — WORLD & TEMPORAL DOMAIN

<a id="m26-t069-s026"></a>

### 16. WorldRoot

`WorldRoot` xác định danh tính campaign và những tham chiếu nền.

Schema logical:

```text
WorldRoot
    WorldID
    WorldSeed
    WorldDefinitionID
    RulesetVersion
    ContentPackManifestID
    SimulationVersion
    ClockID
    HistoricalTimelineID
    CurrentStateRevision
    ActiveProtagonistID
    GameModeSettings
```

`WorldRoot` không nên chứa trực tiếp mọi NPC, item hoặc settlement trong một object lớn.

Nó là aggregate root ở cấp danh tính thế giới và metadata điều hành, không phải một JSON lồng nhau chứa hàng triệu object.

<a id="m26-t069-s027"></a>

### 17. WorldClock

```text
WorldClock
    AbsoluteTick
    CampaignYear
    DayInYear
    TimeSegment
    CalendarDefinitionID
    CurrentHistoricalEraID
```

**AbsoluteTick** là giá trị thời gian tuyệt đối có thứ tự, nên dùng kiểu số nguyên.

Những khái niệm như Year, Season hoặc TimeOfDay được chuyển đổi qua Calendar Rules.

Không được giả định mọi năm có cấu trúc lịch hiện đại.

<a id="m26-t069-s028"></a>

### 18. Vì sao cần thời gian tuyệt đối?

Nhiều domain cần so sánh chính xác:

- thời điểm thương tích;
- ngày hẹn;
- hành trình;
- sự kiện đã lên lịch;
- tuổi của character;
- dữ liệu lịch sử;
- thời điểm một tin được truyền.

Nếu mỗi domain dùng một dạng ngày riêng, rất dễ xảy ra lỗi.

Ví dụ:

Quest dùng ngày thứ 100.

Economy dùng mùa thứ 2.

NPC dùng năm thứ 3.

Nếu chúng không quy về cùng một trục thời gian, thứ tự nhân quả có thể sai.

<a id="m26-t069-s029"></a>

### 19. Simulation Clock và Presentation Clock

Cần phân biệt:

**Simulation Time:** Thời gian thật của thế giới.

**Presentation Time:** Thời gian animation, transition, dialogue display.

Người chơi mở Journal 10 phút ngoài đời không tự làm WorldClock tiến thêm 10 phút.

Một combat turn có thể chiếm một lượng thời gian mô phỏng xác định nhưng animation diễn ra nhanh hoặc chậm tùy thiết lập.

WorldClock chỉ thay đổi qua những commands hoặc scheduled processes hợp lệ.

---

<a id="m26-t069-s030"></a>

## PHẦN VI — SPATIAL & GEOGRAPHY MODEL

<a id="m26-t069-s031"></a>

### 20. Hệ phân cấp không gian chuẩn

Đề xuất phân biệt:

```text
WorldRegion
    |
    +-- Settlement
    |
    +-- LocalArea
            |
            +-- Site
                   |
                   +-- EncounterSpace
                           |
                           +-- TacticalZone
                                   |
                                   +-- PositionSlot
```

Đây là quan hệ phân loại chính; trong thực tế một Settlement cũng có thể chứa LocalArea/Site.

Không nên áp một cây cha–con duy nhất cho mọi cấu trúc địa lý nếu thế giới có vùng chồng lấn, tuyến đường xuyên biên và quyền kiểm soát faction thay đổi.

<a id="m26-t069-s032"></a>

### 21. Spatial Entity

Mỗi đối tượng không gian quan trọng có thể lưu:

```text
SpatialEntity
    SpatialID
    SpatialType
    RegionReference
    ParentReference
    GeometryOrMapReference
    EnvironmentProfileID
    AccessRules
    LifecycleState
```

`AccessRules` có thể được đánh giá qua những rule handlers liên quan.

Ví dụ:

- đường bị ngập;
- cổng bị khóa;
- faction checkpoint;
- địa điểm thiêng không mở cho người ngoài.

Không nên hardcode toàn bộ access trong UI.

<a id="m26-t069-s033"></a>

### 22. Location của Character

Character cần một vị trí authoritative.

Có thể là:

```text
CharacterLocation
    CharacterID
    LocationScope
    SpatialEntityID
    SubPosition
    TravelState
```

Một character có thể:

- đang trong settlement;
- đang travel;
- đang trong dungeon;
- đang trong tactical encounter.

Nhưng không được đồng thời có hai active physical locations.

Khi chuyển từ Exploration sang Combat, engine phải tạo liên kết rõ giữa LocalArea và EncounterSpace.

Không được cho combat scene tự sở hữu vị trí độc lập rồi quên cập nhật nơi nhân vật đang ở.

<a id="m26-t069-s034"></a>

### 23. RouteEdge

```text
RouteEdge
    RouteID
    OriginNodeID
    DestinationNodeID
    TravelMode
    BaseTravelCost
    Capacity
    TerrainProfile
    SeasonalModifiers
    EnvironmentalState
    FactionControl
    AvailabilityState
```

Một tuyến có thể có hướng hoặc hai chiều tùy điều kiện.

Đặc biệt với sông:

Đi xuôi dòng và ngược dòng không nhất thiết có cùng thời gian hoặc rủi ro.

Route Planner phải tính trên Route Graph authoritative.

<a id="m26-t069-s035"></a>

### 24. RouteKnowledge là dữ liệu riêng

```text
RouteKnowledge
    KnowledgeHolderID
    RouteID
    KnowledgeLevel
    LastObservedAt
    LearnedAt
    SourceID
    Confidence
    KnownConditions
```

Một RouteEdge thực sự đã bị đóng có thể vẫn được người chơi tưởng là đang mở.

Đây là kết quả hợp lệ.

Không được đồng bộ RouteKnowledge ngay khi RouteEdge thay đổi nếu không có information propagation.

---

<a id="m26-t069-s036"></a>

## PHẦN VII — CHARACTER CANONICAL MODEL

<a id="m26-t069-s037"></a>

### 25. Không thiết kế Character như một class có hàng trăm field

Character liên kết tới nhiều domain:

- identity;
- family;
- physical condition;
- skills;
- profession;
- class;
- personality;
- relationships;
- knowledge;
- faction;
- inventory;
- memories.

Nếu nhét tất cả vào một mutable `Character` object, rất khó giữ quyền sở hữu.

Đề xuất Character Identity là gốc, còn những hệ chuyên biệt sở hữu dữ liệu của mình.

<a id="m26-t069-s038"></a>

### 26. Character Identity

```text
CharacterIdentity
    CharacterID
    BirthTime
    BiologicalParents
    OriginHouseholdID
    BirthLocationID
    NameRecords
    IdentityTags
    CurrentLifecycleState
```

Quan hệ cha mẹ có thể không được biết công khai.

Engine vẫn có thể lưu các dữ liệu objective cần thiết khi thiết kế xác lập.

Tuy nhiên, không được giả định mọi character biết chính xác nguồn gốc của mình.

<a id="m26-t069-s039"></a>

### 27. Character Life State

```text
CharacterLifeState
    CharacterID
    AgeDerivedFromWorldClock
    LifeStage
    BiologicalCondition
    AliveState
    DeathEventID
    CurrentFunctionalLimitations
```

Tuổi hiện tại nên được suy ra từ BirthTime và WorldClock, không lưu như một con số có thể bị tăng không nhất quán.

Nếu dùng cache, cache phải được xác định rõ là derived state.

<a id="m26-t069-s040"></a>

### 28. Character Attributes

```text
CharacterAttributes
    CharacterID
    THỂ
    XẢO
    GIÁC
    TRÍ
    TÂM
    KHÍ
    PermanentModifiers
    TemporaryModifiers
    DevelopmentHistoryRefs
```

Giá trị nền của sáu Attributes thuộc Character Development.

Những ảnh hưởng do Injury không nên trực tiếp ghi đè Mastery hoặc Attribute base.

Derived Capability phải kết hợp:

- base attributes;
- skill;
- equipment;
- injury;
- fatigue;
- environment;
- applicable techniques.

Các công thức cụ thể thuộc Module 27.

<a id="m26-t069-s041"></a>

### 29. SkillRecord

```text
SkillRecord
    CharacterID
    SkillDefinitionID
    Mastery
    PracticeHistory
    TrainingSources
    LearnedTechniques
    LastMeaningfulUse
```

Skill Mastery có thể nằm trong thang 0–100 như GDD đã xác định.

Tuy nhiên một số thông tin về việc học nên được nén thành milestone và aggregate practice records để không phải lưu từng lần thao tác nhỏ.

<a id="m26-t069-s042"></a>

### 30. ProfessionRecord

```text
ProfessionRecord
    CharacterID
    ProfessionDefinitionID
    CurrentRole
    ExperienceEvidence
    WorkplaceID
    MentorReferences
    CareerMilestones
    CurrentCommitments
```

Một character có thể có nhiều Profession Experience nhưng chỉ có một số occupation đang hoạt động.

Không được nhầm:

**Có Skill nghề** với **đang hành nghề**.

Ví dụ một thợ đúc về già vẫn biết kỹ thuật dù không còn làm việc ở lò.

<a id="m26-t069-s043"></a>

### 31. ClassRecord

```text
ClassRecord
    CharacterID
    ClassDefinitionID
    EmergenceState
    EvidenceRecords
    RecognitionEvents
    Techniques
    DoctrineState
    LegacyTransmission
```

`EmergenceState` có thể gồm:

- Candidate;
- Emerging;
- Recognized;
- Established;
- Mastered;
- Legacy.

Class Domain chỉ được cập nhật bởi các event và rules liên quan identity development.

Không phải một Skill tăng lên vượt threshold là tự động tạo Class.

<a id="m26-t069-s044"></a>

### 32. Personality & Motivation

```text
PersonalityProfile
    CharacterID
    Temperament
    Values
    FearSources
    IdentityCommitments
    MoralBoundaries
    LongTermGoals
    ShortTermNeeds
    InterpretationPatterns
```

Đối với NPC, đây là input cho decision engine.

Đối với protagonist, một số thuộc tính có thể ảnh hưởng phản ứng nội tâm, opportunity và cách diễn đạt, nhưng không được hoàn toàn tước quyền lựa chọn của player.

`MoralBoundaries` không nên chỉ là một danh sách cấm tuyệt đối vĩnh viễn; một số ranh giới có thể thay đổi qua các Life Events rất đặc biệt.

Nhưng sự thay đổi phải có nguyên nhân, không được xảy ra vì Utility Score cao bất thường.

---

<a id="m26-t069-s045"></a>

## PHẦN VIII — HEALTH, INJURY & STATUS MODEL

<a id="m26-t069-s046"></a>

### 33. Health là authoritative owner của Wound

Combat không tự lưu một bản Wound riêng.

Schema:

```text
HealthState
    CharacterID
    Vitality
    StaminaCondition
    FatigueCondition
    ActiveWounds
    IllnessConditions
    LongTermConditions
    FunctionalLimitations
    StabilityState
```

<a id="m26-t069-s047"></a>

### 34. Wound Entity

```text
Wound
    WoundID
    CharacterID
    OriginEventID
    BodyRegion
    Severity
    DamageTypes
    BleedingState
    InfectionRisk
    StabilityState
    TreatmentHistory
    RecoveryState
    PersistentEffects
```

`BodyRegion` phải tương thích cả Equipment Coverage và Combat Targeting.

Nếu armor bảo vệ vai trái, Combat và Health phải dùng cùng taxonomy vùng cơ thể.

<a id="m26-t069-s048"></a>

### 35. Wound State Machine

Đề xuất:

```text
NEW
 |
 v
ACTIVE
 |
 +--> STABILIZED
 |       |
 |       v
 |    RECOVERING
 |       |
 |       +--> HEALED
 |       |
 |       +--> PERSISTENT_IMPAIRMENT
 |
 +--> DETERIORATING
         |
         +--> CRITICAL
         |
         +--> STABILIZED
         |
         +--> FATAL_OUTCOME
```

Đây là state machine khái quát.

Không phải vết thương nào cũng có khả năng đi qua mọi trạng thái.

Một vết thương nhẹ có thể hồi phục không cần điều trị.

Một thương tích nguy kịch có thể dẫn tới tử vong nếu không cứu chữa được.

Quy tắc y khoa cơ học cụ thể cần cân chỉnh ở Module 27.

<a id="m26-t069-s049"></a>

### 36. Status Effect

Không nên sử dụng một hệ thống Status chung có thể tùy tiện gắn mọi thứ lên character.

Phải phân loại:

**Physical:** Choáng, mất thăng bằng, kiệt sức.

**Tactical:** Guard, Exposed, Restrained.

**Environmental:** Ướt, khói, nóng, lạnh.

**Psychological:** Sợ hãi, hoảng loạn hoặc mất bình tĩnh theo mô hình đã thiết kế.

**Medical:** Bleeding, Poisoning, Infection.

**Rare Supernatural:** Chỉ với những status được phép trong Mạch Domain.

Mỗi status có:

- owner;
- source;
- onset;
- duration hoặc exit condition;
- stacking rule;
- effect handler.

Không cho phép effect trùng nhau vô hạn chỉ vì hai module cùng áp một trạng thái.

---

<a id="m26-t069-s050"></a>

## PHẦN IX — HOUSEHOLD & SOCIAL DATA MODEL

<a id="m26-t069-s051"></a>

### 37. Household Entity

```text
Household
    HouseholdID
    SettlementID
    MemberReferences
    HouseholdRoles
    ResourceAccounts
    SharedAssets
    CurrentObligations
    LivelihoodProfile
    LifecycleState
```

Một Household có thể đổi thành viên theo thời gian.

Không được lấy một danh sách thành viên ban đầu làm cấu trúc bất biến.

<a id="m26-t069-s052"></a>

### 38. Family và Household không đồng nghĩa

**Family/Lineage:** Quan hệ huyết thống, hôn phối, nhận chăm sóc, tổ tiên và những liên kết thân thuộc.

**Household:** Những người cùng vận hành một đơn vị sinh sống/kinh tế.

Một người có thể thuộc Family A nhưng đang sống ở Household B.

Nếu gộp hai cấu trúc, các sự kiện kết hôn, học nghề, di cư, chia nhà và inheritance sẽ khó xử lý.

<a id="m26-t069-s053"></a>

### 39. Relationship Edge

```text
Relationship
    RelationshipID
    SourceCharacterID
    TargetCharacterID
    RelationshipTypes
    EmotionalState
    TrustState
    RespectState
    Obligations
    ImportantSharedEvents
    LastInteractionAt
```

Các quan hệ cảm xúc có thể **có hướng**.

Mân có thể tin Miên nhiều hơn Miên tin Mân.

Quan hệ huyết thống có thể được biểu diễn bằng Family Graph, không cần nhân đôi mọi cạnh trong Social Graph.

<a id="m26-t069-s054"></a>

### 40. Sparse Social Graph

Chỉ lưu explicit relationships cho những quan hệ có ý nghĩa.

Các quan hệ nền có thể suy ra từ:

- cùng household;
- cùng profession network;
- cùng settlement;
- cùng faction;
- sự kiện đã chứng kiến.

Không tạo explicit edge giữa mọi cặp NPC trong một vùng.

Khi một quan hệ trở nên quan trọng, nó được nâng thành persistent relationship.

<a id="m26-t069-s055"></a>

### 41. Promise & Obligation

```text
Obligation
    ObligationID
    DebtorOrResponsibleParty
    Beneficiary
    ObligationType
    OriginEventID
    CreatedAt
    DueAt
    Terms
    TransferRules
    State
```

States:

- Proposed;
- Accepted;
- Active;
- Fulfilled;
- Breached;
- Renegotiated;
- Impossible;
- Disputed;
- Expired.

Không phải mọi lời hứa đều cần đủ các trạng thái trên.

`TransferRules` giải quyết việc nghĩa vụ có được chuyển khi một bên chết hay không.

Đây là dữ liệu Social/Agreement, không phải một Quest Flag đơn giản.

---

<a id="m26-t069-s056"></a>

## PHẦN X — KNOWLEDGE, MEMORY & INFORMATION PROPAGATION

<a id="m26-t069-s057"></a>

### 42. Truth, Claim và Belief phải là ba loại khác nhau

**Truth:** Một trạng thái hoặc sự kiện objective.

**Claim:** Điều một actor phát biểu hoặc truyền đạt.

**Belief:** Điều một actor hiện chấp nhận là đúng hoặc có khả năng đúng.

Ví dụ:

Truth:

Cầu hỏng vì nước lũ.

Claim:

Một người cho rằng cầu bị phá hoại.

Belief:

Một số cư dân tin có kẻ phá cầu.

Cả ba có thể tồn tại cùng lúc.

<a id="m26-t069-s058"></a>

### 43. KnowledgeRecord

```text
KnowledgeRecord
    KnowledgeID
    HolderEntityID
    SubjectEntityID
    ClaimID
    KnowledgeKind
    SourceReference
    ObservedAt
    LearnedAt
    Confidence
    LastValidatedAt
    ValidityState
```

`HolderEntityID` có thể là Character hoặc một organizational knowledge repository đã được thiết kế.

Không phải toàn bộ faction tự chia sẻ tức thời những gì một thành viên biết.

<a id="m26-t069-s059"></a>

### 44. ClaimRecord

```text
ClaimRecord
    ClaimID
    PropositionType
    SubjectReferences
    ClaimedValue
    OriginalSource
    OriginTime
    CurrentVersion
    EvidenceReferences
```

Một Claim là một proposition có cấu trúc.

Ví dụ:

`PersonSeenAtLocation(VacLam, LowerRiver, Day103)`

Không chỉ là chuỗi:

> “Lâm đã đi xuống hạ lưu.”

Dùng proposition có cấu trúc giúp:

- so sánh lời kể;
- tìm mâu thuẫn;
- cập nhật tin;
- truyền qua NPC;
- diễn đạt ra nhiều câu chữ khác nhau.

<a id="m26-t069-s060"></a>

### 45. Information Propagation

Một tin được truyền khi có:

- sender;
- receiver;
- kênh liên lạc;
- thời gian;
- nội dung;
- ý định truyền;
- điều kiện tiếp nhận.

Việc truyền tin có thể:

- chính xác;
- thiếu chi tiết;
- bị hiểu sai;
- cố ý xuyên tạc.

Nhưng sự biến dạng phải do rule kiểm soát.

Không được để LLM tự thêm một bí mật vào lời kể chỉ vì nó hợp văn phong.

<a id="m26-t069-s061"></a>

### 46. Memory

MemoryRecord có thể lưu:

```text
MemoryRecord
    MemoryID
    CharacterID
    OriginEventID
    PersonalInterpretation
    EmotionalSignificance
    RecallStrength
    LastReinforcedAt
    RelatedEntities
```

Không phải mọi sự kiện nhỏ đều tạo một MemoryRecord riêng.

Các memory có ý nghĩa lâu dài cần được lưu đầy đủ hơn.

Những trải nghiệm thường lệ có thể được tóm tắt theo giai đoạn.

<a id="m26-t069-s062"></a>

### 47. Knowledge Scope

Mọi actor decision query phải sử dụng scoped knowledge.

Ví dụ:

`GetPerceivedRouteState(characterId, routeId)`

Không cho NPC planner gọi:

`GetActualRouteState(routeId)`

trừ khi logic của một hoạt động trực tiếp cần xử lý thế giới vật lý theo rule phù hợp.

Một guard không biết kẻ địch đang giấu ở đâu.

Nhưng khi guard di chuyển tới một vị trí có người ẩn, Detection Rules có thể xử lý va chạm, quan sát hoặc phát hiện.

Đây là cách tách perception khỏi objective collision.

---

<a id="m26-t069-s063"></a>

## PHẦN XI — ASSET, INVENTORY & OWNERSHIP

<a id="m26-t069-s064"></a>

### 48. Vật phẩm và quyền sở hữu là hai khái niệm độc lập

Một item tồn tại vật lý.

Quyền sở hữu hoặc quyền sử dụng tồn tại về mặt xã hội/pháp lý theo custom của cộng đồng.

Ví dụ cây thuyền:

- Item/Asset tồn tại;
- người sở hữu là một household;
- player đang giữ;
- một companion có quyền sử dụng trong một chuyến đi;
- một người khác đang có yêu sách.

Không được lưu một trường `ownerId` rồi mặc định giải quyết mọi tình huống.

<a id="m26-t069-s065"></a>

### 49. ItemInstance

```text
ItemInstance
    ItemID
    ItemDefinitionID
    MaterialComposition
    QualityProfile
    Condition
    ComponentReferences
    PhysicalLocation
    HistoricalProvenance
    LifecycleState
```

`ItemDefinitionID` xác định loại item.

`ItemID` xác định vật phẩm thực sự.

Các vật phẩm quan trọng có provenance bền vững.

<a id="m26-t069-s066"></a>

### 50. AssetRight

```text
AssetRight
    RightID
    AssetID
    SubjectID
    RightType
    ValidFrom
    ValidUntil
    OriginEventID
    DisputeState
```

Các `RightType` có thể gồm:

- Ownership;
- Custody;
- Use;
- Management;
- Claim.

Chúng có thể đồng thời tồn tại nếu rule cho phép.

Ví dụ người thuê thuyền có quyền sử dụng nhưng không sở hữu.

<a id="m26-t069-s067"></a>

### 51. CommodityLot

```text
CommodityLot
    LotID
    CommodityDefinitionID
    Quantity
    Quality
    StorageLocationID
    CustodianID
    OwnershipReference
    ProductionOrigin
    SpoilageState
```

Commodity Lot phục vụ:

- food;
- wood;
- ore;
- fuel;
- cloth;
- other bulk resources.

Các item độc đáo không nên bị gom vào CommodityLot nếu làm mất lịch sử, condition hoặc identity.

<a id="m26-t069-s068"></a>

### 52. Supply Days là derived metric

Một party gồm bốn người mang một lượng food cụ thể.

Engine tính số ngày supply dựa trên:

- usable food quantity;
- số người;
- nhu cầu tiêu thụ;
- chất lượng/bảo quản;
- điều kiện hành trình.

Nếu party giảm từ bốn xuống ba người, Supply Days có thể thay đổi.

Không được tạo hoặc xóa food chỉ vì giá trị Supply Days đổi.

<a id="m26-t069-s069"></a>

### 53. Resource Conservation

Mọi resource transaction cần thuộc một trong các nhóm:

**Transfer:** Chuyển nguồn lực giữa các nơi/chủ thể.

**Transformation:** Chuyển nguyên liệu thành sản phẩm/phụ phẩm.

**Consumption:** Tiêu hao bởi người, động vật hoặc hoạt động.

**Production:** Tạo từ hoạt động sản xuất hoặc nguồn tự nhiên hợp lệ.

**Loss/Decay:** Hỏng, mất, đổ vỡ.

**Correction/Migration:** Điều chỉnh dữ liệu có audit.

Không được có resource tăng lên mà không thuộc một cơ chế có nguồn hợp lệ.

Đối với sản xuất nông nghiệp, không thể dùng công thức bảo toàn khối lượng đơn giản giữa hạt giống và sản lượng. Nhưng engine vẫn cần ghi nhận quá trình tăng sản lượng từ đất, thời gian, lao động, điều kiện môi trường và production rules.

---

<a id="m26-t069-s070"></a>

## PHẦN XII — ECONOMY & SETTLEMENT MODEL

<a id="m26-t069-s071"></a>

### 54. SettlementState

```text
SettlementState
    SettlementID
    PopulationAggregate
    HouseholdReferences
    InfrastructureReferences
    ResourceAccounts
    LivelihoodComposition
    ProsperityIndicators
    HealthIndicators
    SecurityIndicators
    EnvironmentalConditions
    FactionInfluence
```

Không nên coi các chỉ số như Prosperity hoặc Security là nguồn gốc của mọi quyết định.

Chúng có thể là aggregate/derived indicators từ những dữ liệu cụ thể hơn.

Ví dụ:

Settlement có thể tương đối giàu nhưng thiếu food do tuyến vận tải bị cắt.

Một Prosperity Score cao không được làm mất tình trạng thiếu lương thực đó.

<a id="m26-t069-s072"></a>

### 55. ProductionProcess

```text
ProductionProcess
    ProcessID
    ProcessDefinitionID
    WorkplaceID
    LaborAllocation
    InputRequirements
    OutputDefinitions
    Duration
    QualityModifiers
    CurrentState
```

Một process chỉ được hoàn thành nếu có đủ điều kiện.

Ví dụ một lò đúc cần:

- vật liệu;
- nhiên liệu;
- công cụ;
- lao động;
- thời gian;
- môi trường vận hành.

Không được chỉ tick một thanh progress rồi sinh item khi thiếu toàn bộ input.

<a id="m26-t069-s073"></a>

### 56. Work Assignment

```text
WorkAssignment
    AssignmentID
    WorkplaceID
    CharacterOrLaborPoolID
    Role
    StartTime
    ExpectedEnd
    CommitmentState
```

Một NPC không thể làm việc toàn thời gian ở hai workshop cách nhau nhiều ngày đường trong cùng thời gian.

Simulation cần kiểm tra:

- location;
- availability;
- health;
- other commitments;
- workload.

<a id="m26-t069-s074"></a>

### 57. TradeShipment

```text
TradeShipment
    ShipmentID
    CargoReferences
    OriginID
    DestinationID
    RoutePlanID
    TransportCapacity
    ResponsibleActors
    DepartureTime
    EstimatedArrival
    ActualState
```

Không cho phép hàng hóa xuất hiện tại destination ngay khi shipment được tạo.

Trade Shipment phải đi qua Travel/Logistics Rules.

Tùy Simulation LOD, hành trình có thể được tổng hợp, nhưng vẫn phải có duration, rủi ro và các điều kiện đủ.

---

<a id="m26-t069-s075"></a>

## PHẦN XIII — FACTION & POLITICAL MODEL

<a id="m26-t069-s076"></a>

### 58. Faction Identity

```text
Faction
    FactionID
    FactionDefinitionID
    FoundingEventID
    CurrentLeadership
    MembershipRules
    OrganizationalStructure
    Goals
    ResourceControl
    InfluenceState
    LifecycleState
```

Faction không nhất thiết là một nhà nước hoặc tổ chức tập trung.

Có thể là:

- mạng thương mại;
- liên minh cộng đồng;
- nhóm quân sự;
- dòng họ quyền lực;
- một tập hợp nghề nghiệp;
- tổ chức tín ngưỡng.

Cần hỗ trợ cấu trúc tổ chức linh hoạt.

<a id="m26-t069-s077"></a>

### 59. Membership

```text
FactionMembership
    MembershipID
    CharacterID
    FactionID
    Role
    JoinedAt
    MembershipState
    AuthorityScope
```

Không chỉ có `isMember`.

Một character có thể:

- là thành viên;
- là người cộng tác;
- đại diện tạm thời;
- bị đình chỉ;
- đã rời;
- có quyền nhưng không còn hoạt động.

Đặc biệt, `AuthorityScope` quyết định ai có quyền sử dụng tài nguyên hoặc đưa ra cam kết thay tổ chức.

<a id="m26-t069-s078"></a>

### 60. Faction Decision

Faction Decision Engine cần sử dụng:

- mục tiêu;
- nguồn lực;
- thông tin đã biết;
- áp lực chính trị;
- leadership;
- internal conflict;
- khả năng thực hiện.

Không được tự huy động một đội quân nếu faction không có nguồn nhân lực, vật tư hoặc thời gian.

Một quyết định faction quan trọng cần tạo event, commitment và resource reservation phù hợp.

---

<a id="m26-t069-s079"></a>

## PHẦN XIV — EVENT ENGINE CANONICAL CONTRACT

<a id="m26-t069-s080"></a>

### 61. Event Template và Event Instance

Phân biệt:

**EventTemplate:** Một cấu trúc sự kiện có thể được sử dụng nhiều lần.

**EventCandidate:** Một tổ hợp event và actor đang đủ điều kiện để xem xét.

**ScheduledEvent:** Một event có thời điểm xử lý hoặc điều kiện chờ cụ thể.

**CommittedDomainEvent:** Một kết quả đã được Rule Engine xác lập và commit.

**NarrativeProjection:** Cách event được biết và được trình bày.

Các loại này không được dùng chung một class mutable có state mơ hồ.

<a id="m26-t069-s081"></a>

### 62. EventTemplate

```text
EventTemplate
    TemplateID
    Version
    Domain
    TriggerDefinitions
    ParticipantRoleDefinitions
    LocationConstraints
    TemporalConstraints
    RequiredRuleHandlers
    OutcomeDefinitions
    FollowUpDefinitions
    ContentTags
```

Template không trực tiếp chứa code thực thi tùy ý.

Nó tham chiếu tới Rule Handler Registry đã kiểm thử.

<a id="m26-t069-s082"></a>

### 63. EventCandidate

```text
EventCandidate
    TemplateID
    BoundParticipants
    BoundLocation
    TriggeringStateRevision
    EligibilityEvidence
    SelectionContext
```

Candidate là đối tượng tạm.

Không phải committed event.

World Director có thể xem những candidate được đánh dấu là eligible optional opportunities nhưng không được giả định chúng đã xảy ra.

<a id="m26-t069-s083"></a>

### 64. CommittedDomainEvent

```text
CommittedDomainEvent
    EventID
    EventType
    AggregateID
    WorldTime
    CommitSequence
    CausationID
    CorrelationID
    RuleVersion
    Payload
    SchemaVersion
```

**CausationID:** Command hoặc event trực tiếp gây ra kết quả.

**CorrelationID:** Liên kết toàn bộ chuỗi công việc.

**CommitSequence:** Thứ tự commit authoritative.

Các trường này rất quan trọng khi debug một event tạo nhiều follow-up.

<a id="m26-t069-s084"></a>

### 65. Causal Chain

Ví dụ:

```text
FloodPressureIncreased
    |
    v
BridgeStructureFailed
    |
    +--> RouteClosed
    |
    +--> TradeShipmentDelayed
    |
    +--> RepairNeedCreated
    |
    +--> WitnessObservation
             |
             v
       RumorTransmission
```

Mỗi bước là một sự kiện hoặc tác động hợp lệ.

Không được coi tất cả là một event lớn cần update toàn bộ game world trong một hàm duy nhất.

<a id="m26-t069-s085"></a>

### 66. Event Depth & Loop Protection

Một event có thể tạo follow-up events.

Nhưng hệ thống cần giới hạn và phát hiện vòng lặp bất thường.

Ví dụ:

`TrustChanged → ReactionChanged → TrustChanged → ...`

Nếu mỗi bước tạo phản ứng mới không ngừng, engine có thể kẹt.

Đề xuất:

- cùng một causal chain có giới hạn xử lý đồng bộ;
- handlers có idempotency;
- các phản ứng cần thời gian hoặc actor initiative được scheduled sang bước thích hợp;
- phát hiện lặp event signature;
- diagnostics khi vượt mức cho phép.

Ngưỡng cụ thể sẽ được benchmark.

Không được xử lý bằng cách âm thầm bỏ event quan trọng rồi tiếp tục như không có lỗi.

---

<a id="m26-t069-s086"></a>

## PHẦN XV — COMMAND CONTRACT & RULE RESOLUTION

<a id="m26-t069-s087"></a>

### 67. Command là yêu cầu, không phải sự thật

Ví dụ:

`MoveCharacter`

`AttackTarget`

`TransferItem`

`AcceptAgreement`

`StartTravel`

`BeginCrafting`

`TrainWithMentor`

`AdvanceTime`

Một Command có thể thành công, bị từ chối hoặc thất bại trong thực thi theo rule.

Command chưa được commit không được làm thay đổi authoritative state.

<a id="m26-t069-s088"></a>

### 68. Command Envelope

```text
CommandEnvelope
    CommandID
    CommandType
    IssuerID
    TargetEntityIDs
    IssuedAtWorldTime
    ExpectedStateRevision
    Payload
    Origin
```

`Origin` cho biết command xuất phát từ:

- player;
- NPC AI;
- scheduled system;
- historical controller;
- validated script.

Các nguồn đều phải qua authorization và rule validation tương ứng.

Không có nguồn nào được bypass invariants chỉ vì là system command.

<a id="m26-t069-s089"></a>

### 69. Command Result

```text
CommandResult
    CommandID
    Status
    ValidationErrors
    CommittedEventIDs
    NewStateRevision
    PublicFeedbackRefs
```

Trạng thái có thể gồm:

**AcceptedAndCommitted**

**Rejected**

**Conflict/StaleState**

**FailedBeforeCommit**

**ScheduledForLater**

Một command `StartCrafting` có thể commit việc bắt đầu công việc nhưng chưa hoàn thành sản phẩm.

Cần phân biệt rõ Accepted, Scheduled và Completed.

<a id="m26-t069-s090"></a>

### 70. Internal Interface mẫu

Ví dụ C# mang tính hợp đồng:

```csharp
public interface ICommand
{
    string CommandId { get; }
    string IssuerId { get; }
}

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    CommandPlan Evaluate(
        TCommand command,
        IReadOnlyWorldSnapshot snapshot,
        IRuleContext context);
}
```

`Evaluate` phải lập kế hoạch trên trạng thái chỉ đọc hoặc staged state, không tự thực hiện IO.

`CommandPlan` gồm:

- rule validations;
- proposed mutations;
- proposed events;
- resource reservations;
- scheduled follow-ups.

Application Layer xác nhận và commit.

Domain Handler không tự gọi UI hoặc ghi file save.

<a id="m26-t069-s091"></a>

### 71. Quy trình xử lý chuẩn

```text
Command Received
       |
       v
Schema Validation
       |
       v
Issuer / Capability Validation
       |
       v
Read Authoritative Snapshot
       |
       v
Rule Preconditions
       |
       v
Deterministic Resolution
       |
       v
Proposed State Changes
       |
       v
Cross-Domain Invariant Validation
       |
       v
Atomic Commit
       |
       +--> Current State
       +--> Event Journal
       +--> Scheduled Follow-Ups
       |
       v
Projection Update
       |
       v
Player Feedback
```

Tất cả những thay đổi bắt buộc để một hành động có tính nhất quán phải nằm trong atomic boundary.

Các hoạt động kéo dài hoặc phụ thuộc tương lai được tạo dưới dạng scheduled follow-up, không giả vờ đã hoàn tất.

<a id="m26-t069-s092"></a>

### 72. Transaction Example — Chuyển Item

Player muốn giao một cây giáo cho companion.

Preconditions:

- item tồn tại;
- player hoặc chủ thể đưa đồ có quyền giao;
- item đang ở vị trí có thể trao;
- companion có khả năng nhận;
- item không bị reservation trái phép.

Nếu mọi điều kiện hợp lệ:

- custody thay đổi;
- ownership có thể giữ nguyên hoặc thay đổi tùy ý định;
- inventory projection cập nhật;
- transfer event được commit.

Nếu giao dịch thất bại:

- không có item nào bị dịch chuyển;
- không có ownership event;
- không có relationship reward giả.

<a id="m26-t069-s093"></a>

### 73. Concurrency

Bản MVP nên dùng **single ordered mutation executor** cho authoritative simulation.

Các tác vụ nặng có thể tính trên immutable snapshot ở luồng khác:

- route search;
- simulation forecast;
- content query;
- UI projection.

Nhưng kết quả được commit phải kiểm tra lại World State Revision.

Không được để một kết quả tính trên snapshot cũ ghi đè thay đổi mới.

---

<a id="m26-t069-s094"></a>

## PHẦN XVI — TIME SCHEDULER & PROCESSING ORDER

<a id="m26-t069-s095"></a>

### 74. ScheduledEvent

```text
ScheduledEvent
    ScheduleID
    DueAt
    Phase
    Priority
    InitiatingEntityID
    EventHandlerID
    Preconditions
    Payload
    LifecycleState
```

`DueAt` dùng World Time.

`Phase` giúp định nghĩa thứ tự xử lý trong cùng một time step.

`Priority` dùng trong phạm vi phase, không phải quyền tùy ý bỏ qua causal order.

<a id="m26-t069-s096"></a>

### 75. Simulation Phase

Đề xuất các phase khái niệm:

**A — Time Boundary:** Xác lập mốc thời gian cần xử lý.

**B — Due Commitments:** Những nghĩa vụ hoặc sự kiện đến hạn đã được lên lịch.

**C — Environmental Progress:** Những biến đổi môi trường theo thời gian.

**D — Economic & Biological Progress:** Sản xuất, tiêu thụ, hồi phục, sinh thái, aging.

**E — Actor Decisions:** NPC/faction lựa chọn hành động hợp lệ.

**F — Narrative/Knowledge Progress:** Story Thread và thông tin được cập nhật từ các event.

**G — Validation & Projection:** Kiểm tra consistency và chuẩn bị view models.

Đây không phải thứ tự cứng áp dụng cho mọi sự kiện của mọi ngày.

Trong thực tế, scheduler phải xử lý các dependency được định nghĩa rõ.

Ví dụ một người chết ở Phase B không được tiếp tục được phân công lao động ở Phase D.

<a id="m26-t069-s097"></a>

### 76. Event Ordering

Với những event cùng thời điểm và phase, thứ tự cần ổn định.

Có thể sử dụng:

```text
(DueAt, Phase, Priority, StableEntityID, ScheduleSequence)
```

Không dùng thứ tự duyệt dictionary không bảo đảm ổn định.

Mọi tie-breaker phải xác định.

Khi một event tạo event mới với thời gian bằng hiện tại, engine phải có quy tắc rõ nó được xử lý ngay trong phase phù hợp hay chuyển sang step tiếp theo.

Không được tạo chuỗi recursive vô hạn.

<a id="m26-t069-s098"></a>

### 77. Time Skip

Time Skip nhiều tháng/năm không có nghĩa chỉ cộng một con số vào WorldClock.

Nó phải xử lý:

- events đến hạn;
- travel;
- resource production/consumption;
- character development;
- aging;
- mortality;
- household changes;
- political events;
- relationships;
- information propagation;
- story threads.

Những domain đủ điều kiện có thể xử lý aggregate theo bước lớn.

Những event đặc biệt hoặc ràng buộc thời gian chặt phải được xử lý ở độ phân giải cần thiết.

<a id="m26-t069-s099"></a>

### 78. Adaptive Time Resolution

Ví dụ:

Một settlement không liên quan trực tiếp player có thể được xử lý theo chu kỳ dài.

Nhưng một NPC đang chảy máu nguy kịch cần xử lý theo mốc thời gian ngắn hơn.

Đề xuất mỗi scheduled process có:

- temporal resolution;
- scope;
- criticality;
- dependency;
- aggregation eligibility.

Không áp một tick duy nhất cho tất cả domain.

<a id="m26-t069-s100"></a>

### 79. Time Skip Interruption

Một Life Interval có thể được ngắt nếu có một sự kiện đòi hỏi player đưa ra quyết định thật sự và có kênh thông tin hợp lệ.

Ví dụ:

Player đang học nghề trong một năm.

Sau ba tháng, cha gửi tin báo gia đình cần giúp.

Player có thể quyết định tiếp tục hoặc trở về.

Không nhất thiết phải ngắt Time Skip vì mọi sự kiện nhỏ.

World Director chỉ đánh giá việc expose các sự kiện đã hợp lệ, không tự tạo lý do ngắt.

---

<a id="m26-t069-s101"></a>

## PHẦN XVII — DETERMINISTIC RNG CONTRACT

<a id="m26-t069-s102"></a>

### 80. RNG là dependency được truyền vào rule

Không cho một handler gọi trực tiếp random toàn cục.

Ví dụ:

```csharp
public interface IDeterministicRandom
{
    uint NextUInt32(RandomContext context);
    int NextInt(RandomContext context, int min, int maxExclusive);
}
```

`RandomContext` cần được xác định và version hóa.

<a id="m26-t069-s103"></a>

### 81. RNG Stream

Đề xuất:

```text
WorldSeed
    +-- CharacterGeneration
    +-- EventSelection
    +-- NPCDecision
    +-- Economy
    +-- Ecology
    +-- Combat
    +-- HistoricalVariants
```

Các stream được tách để giảm coupling.

Với event quan trọng, có thể sử dụng random theo khóa gồm:

- seed;
- domain;
- stable entity IDs;
- event occurrence ID;
- decision step;
- ruleset version.

Một kết quả random phải có thể truy vết tới context đã tạo ra nó.

<a id="m26-t069-s104"></a>

### 82. Ngăn retry làm đổi kết quả không hợp lệ

Nếu một command được gửi trùng vì input hoặc retry:

- `CommandID` phải được nhận diện;
- command đã commit không được commit lần thứ hai;
- outcome không bị random lại;
- resource không bị trừ lần hai.

Idempotency cần nằm ở Application/Command Processor, không dựa vào UI tránh double click.

<a id="m26-t069-s105"></a>

### 83. Ngăn reroll do kiểm tra kết quả

Một command không được cho phép rút kết quả random rồi hủy không chi phí chỉ vì player không thích.

Đây là vấn đề thiết kế Save/Load và command commit.

Trong Standard Mode, player có thể load lại save theo quy tắc đã chọn.

Nhưng trong cùng một timeline, các lần thử một hành động cần được phân biệt bằng:

- trạng thái đã thay đổi;
- thời gian;
- tài nguyên;
- AttemptID.

Không nên chỉ thêm `AttemptIndex` sau mỗi lần click không hợp lệ để người chơi có thể tạo kết quả mới.

<a id="m26-t069-s106"></a>

### 84. Numeric Determinism

Đối với những phép tính ảnh hưởng lâu dài:

- resource quantities;
- skill progression;
- probability thresholds;
- economic allocations;

cân nhắc integer hoặc fixed-point để tránh sai lệch số thực không cần thiết.

Nếu dùng floating-point, phải kiểm thử rõ phạm vi determinism cam kết trên các nền tảng mục tiêu.

Không được giả định mọi phép tính float trong mọi game engine đều tái hiện bit-by-bit trên mọi CPU.

---

<a id="m26-t069-s107"></a>

## PHẦN XVIII — HISTORICAL ANCHOR CONTRACT

<a id="m26-t069-s108"></a>

### 85. HistoricalAnchor Entity

```text
HistoricalAnchor
    AnchorID
    HistoricalEra
    CanonLevel
    EarliestEligibility
    LatestPermittedWindow
    RequiredWorldConditions
    ValidScenarioDefinitions
    ProtectedInvariants
    AdjustableOutcomes
    CurrentState
```

`CanonLevel` gồm:

- Hard;
- Flexible;
- Emergent.

Một Hard Anchor không được hiểu là “cứ tới ngày là chạy cutscene”.

Nó cần một tập điều kiện nền và những kịch bản causal hợp lệ.

<a id="m26-t069-s109"></a>

### 86. Protected Invariants

Ví dụ:

**Cổ Loa trở thành trung tâm quan trọng của Âu Lạc.**

Protected invariant có thể là:

- một trung tâm chính trị/quân sự mới được hình thành ở Loa Vực;
- mạng xây dựng và hậu cần xuất hiện;
- công trình có ý nghĩa chính trị lớn.

Những yếu tố adjustable:

- ai tham gia;
- công nghệ hỗ trợ;
- tổn thất;
- tổ chức lao động;
- quan hệ với các cộng đồng;
- công trình phụ;
- một số timeline details.

Cần xác định rõ trong từng Anchor Definition.

<a id="m26-t069-s110"></a>

### 87. Anchor Scenario

Một Anchor có thể có nhiều scenario đã biên soạn.

Ví dụ:

**Construction Scenario A:** Nguồn lực tương đối ổn định.

**Construction Scenario B:** Thiếu vật liệu, cần mạng cung ứng thay thế.

**Construction Scenario C:** Lao động có xung đột, tiến độ chậm và chính sách điều phối thay đổi.

Mỗi scenario vẫn phải dùng resource và actor thật.

Không được tạo người hoặc vật liệu miễn phí.

<a id="m26-t069-s111"></a>

### 88. Anchor Impossibility

Nếu simulation tạo trạng thái khiến không còn scenario hợp lệ:

Không được để Historical Controller tự gian lận.

Phải:

1. Phát hiện bất khả thi tại validation.
2. Ghi rõ invariant bị vi phạm.
3. Xác định action hoặc event gây ra trạng thái.
4. Phân loại đây là lỗi content design hoặc thiếu authored scenario.
5. Sửa thiết kế hoặc giới hạn interaction từ trước khi phát hành.

Đối với bản phát hành, những hành động được phép trong game phải nằm trong không gian kịch bản đã được kiểm chứng đủ để không gây softlock lịch sử.

Đây là một nhiệm vụ testing quan trọng, không thể giải quyết bằng câu “AI tự tìm cách”.

---

<a id="m26-t069-s112"></a>

## PHẦN XIX — NARRATIVE, STORY THREAD & QUEST CONTRACT

<a id="m26-t069-s113"></a>

### 89. StoryThread

```text
StoryThread
    ThreadID
    OriginEventID
    SubjectReferences
    CurrentState
    KnownActors
    ActivePressures
    InterventionWindows
    ResolutionConditions
    HistoricalOutcomeRefs
```

Các trạng thái có thể gồm:

- Emerging;
- Active;
- Escalating;
- Transformed;
- Resolved;
- Dormant;
- Closed.

Không bắt buộc mọi thread đều được player phát hiện.

<a id="m26-t069-s114"></a>

### 90. Quest là projection

Một Quest View thể hiện:

- player biết vấn đề gì;
- có thể can thiệp bằng những cách nào;
- những lời hứa đã chấp nhận;
- những việc player đang quan tâm.

Quest không được trực tiếp sở hữu:

- người nào đã chết;
- item nào đã được chuyển;
- cầu nào đã hỏng;
- faction nào kiểm soát route.

Các sự thật đó nằm ở domain tương ứng.

<a id="m26-t069-s115"></a>

### 91. Quest State Update

Ví dụ:

Player giúp sửa cầu.

Settlement Domain commit `BridgeRepaired`.

StoryThread Engine nhận event và đánh giá:

- vấn đề cầu đã giải quyết tới đâu;
- còn tranh chấp xã hội không;
- ai cần nhận tin;
- có follow-up nào không.

Quest Projection cập nhật theo Knowledge mà player có.

Không phải UI tự set `questCompleted = true` rồi nhờ World State cập nhật ngược.

<a id="m26-t069-s116"></a>

### 92. Director Opportunity

World Director sử dụng một read model:

```text
AvailableNarrativeOpportunities
    EligibleThreads
    ValidInformationChannels
    OptionalEvents
    PacingMetadata
    PlayerContext
```

Director được phép chọn:

- hiển thị;
- tóm tắt;
- hoãn presentation trong cửa sổ hợp lệ;
- không phơi bày.

Không được chọn kết quả combat, death, economy hoặc historical event.

---

<a id="m26-t069-s117"></a>

## PHẦN XX — MẠCH & BELIEF CONTRACT

<a id="m26-t069-s118"></a>

### 93. MạchState

```text
MachSiteState
    SiteID
    MachDefinitionID
    CurrentConfiguration
    StabilityState
    RelevantMemoryRefs
    RitualConnections
    EnvironmentalPressures
    LastMeaningfulChange
```

Mạch gắn địa điểm, lịch sử, con người và các quan hệ đã thiết kế.

Không được tạo một thanh Mana toàn cục rồi để mọi ritual trừ điểm.

<a id="m26-t069-s119"></a>

### 94. RitualInstance

```text
RitualInstance
    RitualID
    RitualDefinitionID
    ParticipantReferences
    AuthorityContext
    SiteID
    RequiredObjects
    ProcedureState
    SocialOutcomeState
    MachOutcomeState
```

Phải tách Social Outcome và Mạch Outcome.

Một nghi lễ có thể thành công ở một lớp và thất bại ở lớp khác.

<a id="m26-t069-s120"></a>

### 95. Ambiguity

Game có thể không xác nhận toàn bộ ontology của một sự kiện đối với player.

Nhưng Rule Engine vẫn phải biết:

- những physical effects nào xảy ra;
- actor nào chứng kiến;
- memory nào được tạo;
- state nào bị thay đổi.

Đối với những Mythic Anchor cố tình giữ tính siêu hình chưa xác định, cần có một **Outcome Contract tối thiểu**.

Ví dụ một hiện tượng dẫn tới nhiều nhân vật có giấc mơ giống nhau.

Engine có thể giữ bản chất nguyên nhân mở, nhưng phải ghi rõ:

- những ai đã mơ;
- thời điểm;
- điều gì được họ cảm nhận;
- những vật chứng nào tồn tại;
- hậu quả gameplay nào được phép.

Mơ hồ về diễn giải không được đồng nghĩa mơ hồ về committed state.

---

<a id="m26-t069-s121"></a>

## PHẦN XXI — DEATH, SUCCESSION & LEGACY CONTRACT

<a id="m26-t069-s122"></a>

### 96. Character Death Transaction

Một Death Command/Event phải giải quyết tối thiểu:

- Alive State;
- Death Event;
- active action cancellation;
- combat participation;
- physical location/body;
- inventory custody;
- party membership;
- immediate witnesses.

Những hệ xa hơn có thể xử lý sau theo scheduler:

- family notification;
- funeral;
- inheritance dispute;
- faction succession;
- social rumor;
- chronicle;
- legacy.

<a id="m26-t069-s123"></a>

### 97. SuccessionCandidate

```text
SuccessionCandidate
    CandidateCharacterID
    PredecessorCharacterID
    RelationshipEvidence
    NarrativeConnection
    KnowledgeContinuity
    MotivationEligibility
    Availability
    PlayabilityValidation
```

Candidate phải là một character có identity thật.

Không sinh một người kế tục không có lịch sử chỉ để tránh Game Over.

<a id="m26-t069-s124"></a>

### 98. Succession State Machine

```text
PROTAGONIST_ACTIVE
        |
        v
DEATH_OR_RETIREMENT_TRIGGER
        |
        v
PREDECESSOR_FINALIZED
        |
        v
CANDIDATE_EVALUATION
        |
        +--> SUCCESSOR_AVAILABLE
        |          |
        |          v
        |   TRANSITION_CONTEXT
        |          |
        |          v
        |   NEW_PROTAGONIST_ACTIVE
        |
        +--> NO_VALID_SUCCESSOR
                   |
                   v
              EPILOGUE
```

Nếu succession cho phép khi nhân vật cũ còn sống trong một trường hợp được authored, cần một transition type riêng; không nên gọi đó là death.

<a id="m26-t069-s125"></a>

### 99. Player Control Transfer

Khi successor được xác lập:

**Được chuyển:**

- ActiveProtagonistID;
- quyền điều khiển nhân vật mới;
- UI focus;
- player-facing progression context.

**Không tự động chuyển:**

- Attributes;
- Skill;
- Class;
- inventory;
- ownership;
- personal knowledge;
- secret relationships;
- danh tiếng cá nhân.

Các vật phẩm hoặc tri thức chỉ được chuyển qua những event truyền giao hợp lệ.

<a id="m26-t069-s126"></a>

### 100. LegacyRecord

```text
LegacyRecord
    LegacyID
    OriginCharacterID
    OriginEventIDs
    LegacyCategory
    AffectedEntities
    TransmissionMedium
    CurrentPersistenceState
    KnownInterpretations
    DecayOrMaintenanceRules
```

Một legacy có thể là:

- kỹ thuật;
- vật phẩm;
- công trình;
- household;
- chính sách;
- quan hệ;
- truyền thuyết.

Không phải một thanh Fame duy nhất.

---

<a id="m26-t069-s127"></a>

## PHẦN XXII — PERSISTENCE DESIGN

<a id="m26-t069-s128"></a>

### 101. Chọn mô hình hybrid

Đề xuất lưu:

**Current State:** Dữ liệu authoritative cần gameplay.

**Critical Event Journal:** Lịch sử các biến cố quan trọng.

**Snapshots:** Điểm lưu trạng thái.

**Read Projections:** Những bảng/góc nhìn cho Journal, Map, Quest, Chronicle.

Không phải mọi object cần event sourcing đầy đủ.

Nhưng mọi event có hậu quả lớn hoặc liên quan historical continuity phải có audit trail.

<a id="m26-t069-s129"></a>

### 102. Save Manifest

```text
SaveManifest
    SaveID
    WorldID
    WorldSeed
    RulesetVersion
    ContentManifestHash
    SchemaVersion
    SimulationVersion
    SnapshotRevision
    LastCommitSequence
    WorldTime
    ActiveCharacterID
    IntegrityChecksum
```

`ContentManifestHash` giúp xác định chính xác những definitions nào được dùng.

Nếu một content pack bị sửa hoặc không còn tồn tại, game cần phát hiện trước khi load và thực hiện compatibility policy.

Không được âm thầm load một Event Template đã bị thay đổi ngữ nghĩa rồi làm biến dạng save.

<a id="m26-t069-s130"></a>

### 103. Snapshot Strategy

Snapshot có thể tạo:

- khi player lưu;
- trước một Historical Transition lớn;
- trước migration;
- theo chu kỳ phù hợp;
- tại những checkpoint recovery kỹ thuật.

Snapshot phải được tạo từ một state revision nhất quán.

Không được lấy nửa dữ liệu trước transaction và nửa dữ liệu sau transaction.

<a id="m26-t069-s131"></a>

### 104. Event Journal Strategy

Mỗi committed event quan trọng cần:

- ID;
- loại;
- origin command;
- time;
- sequence;
- rule version;
- payload;
- affected entity references.

Không nên lưu narrative text như sự thật chính.

Ví dụ:

Không chỉ lưu:

> “Vạc Mân đã chết.”

Phải lưu event có:

- character ID;
- cause;
- location;
- time;
- witness context;
- rule origin.

Presentation có thể tái dựng câu chuyện từ những dữ liệu này.

<a id="m26-t069-s132"></a>

### 105. Projection Rebuild

Những projection như:

- Quest View;
- NPC Relationship Summary;
- Journal Search Index;
- Chronicle Summary;

cần có khả năng tái dựng từ những authoritative records tương ứng.

Tuy nhiên, do không event-source toàn bộ state, không được mặc định có thể replay mọi loại projection từ Event Journal một mình.

Một số projection cần kết hợp:

- snapshot;
- materialized authoritative state;
- event history;
- knowledge records.

Phải xác định điều đó trong hợp đồng từng projection.

<a id="m26-t069-s133"></a>

### 106. Save Transaction & Recovery

Khi lưu, cần bảo đảm:

- committed state nhất quán;
- scheduler state được lưu;
- RNG state/counter được lưu;
- content/rule version được lưu;
- unfinished commands không được ghi thành completed;
- corrupted save có thể được nhận diện.

Nếu dùng SQLite, phải sử dụng transaction và cơ chế backup phù hợp, không chỉ sao chép file database đang mở một cách tùy tiện.

Việc bật WAL cũng cần quản lý checkpoint và durability settings phù hợp với yêu cầu save của game.

---

<a id="m26-t069-s134"></a>

## PHẦN XXIII — CONTENT SCHEMA & OFFLINE LLM PIPELINE

<a id="m26-t069-s135"></a>

### 107. Content Definition là dữ liệu build-time

Các file từ Content Pipeline có thể được author theo JSON hoặc format phù hợp.

Nhưng trước khi runtime sử dụng, chúng cần:

- schema validation;
- reference validation;
- semantic validation;
- causal validation;
- human review theo mức quan trọng;
- simulation test;
- content pack compilation.

<a id="m26-t069-s136"></a>

### 108. JSON Schema

Có thể sử dụng JSON Schema Draft 2020-12 để kiểm tra cấu trúc dữ liệu authoring.

Ví dụ các điều kiện:

- ID bắt buộc;
- fields đúng kiểu;
- enum hợp lệ;
- không cho property không được định nghĩa nếu schema yêu cầu;
- participant roles đúng cấu trúc;
- event outcomes có reference.

Nhưng JSON Schema chỉ kiểm tra được một phần.

Nó không thể tự chứng minh:

> NPC trong event có động cơ hợp lý.

Hoặc:

> Event này không làm Cổ Loa bất khả thi.

Vì vậy cần validator riêng cho business/gameplay semantics.

<a id="m26-t069-s137"></a>

### 109. Rule Reference Registry

Content không được chứa biểu thức code tùy tiện.

Ví dụ:

```text
RuleID: transport.boat.repair
RuleVersion: 3
```

Rule Registry map tới handler đã được compile và test.

Event Template chỉ tham chiếu các rule hợp lệ.

Không cho phép LLM trả về một đoạn script và tự cấp quyền thực thi.

<a id="m26-t069-s138"></a>

### 110. Content Approval Levels

Đề xuất:

**Level 1 — Minor Presentation Content:** Văn bản mô tả, câu thoại biến thể không thay rule.

**Level 2 — Local Systemic Content:** Event phụ, NPC archetype, profession content.

**Level 3 — Important Narrative Content:** Companion, major Story Thread, faction conflict.

**Level 4 — Canon-Critical Content:** Historical Anchors, Kim Quy, Nỏ thần, Mạch lớn, kết cục quan trọng.

Cấp cao hơn yêu cầu review sâu hơn.

Không cần cùng một quy trình thủ công nặng cho mọi câu mô tả thời tiết và mọi major character.

<a id="m26-t069-s139"></a>

### 111. Runtime Narrative Renderer

Presentation Layer nhận một payload đã được Knowledge Filter kiểm tra.

Ví dụ:

```text
NarrativeSituation
    EventType
    VisibleFacts
    KnownActors
    PerceivedEmotions
    AvailableDialogueActs
    Locale
    StyleTags
```

Renderer có thể sử dụng template hoặc grammar đã phê duyệt để tạo lời văn.

Không cần LLM runtime.

Nếu có thiếu template, engine phải có fallback trung tính và chính xác.

Không được bịa thêm một event để hoàn chỉnh câu văn.

---

<a id="m26-t069-s140"></a>

## PHẦN XXIV — PERFORMANCE & SIMULATION LEVEL OF DETAIL

<a id="m26-t069-s141"></a>

### 112. Simulation Tiers

Đề xuất:

| Tier | Đối tượng | Mức xử lý |
|---|---|---|
| Active | Party, local NPC, active encounter | Chi tiết |
| Relevant | Gia đình, companion ở xa, active story actors | Event-driven |
| Background | NPC/settlement ít liên quan | Aggregate theo chu kỳ |
| Statistical | Population chưa materialized | Population model |

Các tên tier này phục vụ kỹ thuật, cần mapping rõ với Tier S/A/B/C đã dùng trong thiết kế nội dung.

Không nên giả định Tier S luôn phải Active Simulation.

Một nhân vật lịch sử quan trọng ở xa player có thể được mô phỏng theo event schedule nhưng vẫn có dữ liệu narrative đầy đủ.

<a id="m26-t069-s142"></a>

### 113. Promotion to Persistent Entity

Một NPC background có thể trở nên quan trọng vì:

- player gặp;
- trở thành witness;
- tham gia một hợp đồng;
- gây thương tích;
- nhận quan hệ;
- trở thành successor candidate.

Khi đó entity được materialize và có ID ổn định.

Quá trình promotion phải dựa trên dữ liệu aggregate và seed phù hợp.

Không được sinh ra một cuộc đời mới mâu thuẫn với household history đã tồn tại.

<a id="m26-t069-s143"></a>

### 114. Promotion Contract

```text
MaterializationRequest
    PopulationPoolID
    RequiredRole
    RequiredConstraints
    OriginContext
    DeterministicGenerationKey
```

Output phải được validate:

- hộ gia đình;
- tuổi;
- giới hạn nghề;
- quan hệ;
- vị trí;
- tài nguyên;
- lịch sử.

Sau khi entity đã được commit, các lần load hoặc quay lại vùng phải tham chiếu entity cũ.

<a id="m26-t069-s144"></a>

### 115. NPC Scheduler

NPC không cần tính Utility Score liên tục theo frame.

Các trigger phù hợp:

- thời điểm hành động đến hạn;
- một nhu cầu thay đổi;
- nhận tin mới;
- resource/household thay đổi;
- player interaction;
- faction order;
- environmental emergency.

Kết quả của scheduler là command hợp lệ.

Không phải trực tiếp ghi state.

<a id="m26-t069-s145"></a>

### 116. Sparse Graph & Query Indices

Các index cần nghiên cứu:

- character by settlement;
- character by profession;
- household membership;
- relationship adjacency;
- known facts by holder;
- route by origin;
- event templates by pressure;
- scheduled event by due time;
- asset by custodian;
- asset rights by claimant.

Mục tiêu là tránh quét toàn bộ database ở mỗi event eligibility check.

---

<a id="m26-t069-s146"></a>

## PHẦN XXV — CANONICAL ERROR MODEL

<a id="m26-t069-s147"></a>

### 117. Vì sao cần hệ lỗi chuẩn?

Khi có nhiều subsystem, cùng một failure phải được hiểu nhất quán.

Ví dụ player không thể mượn thuyền vì:

- không có quyền;
- thuyền đang sử dụng;
- thuyền hỏng;
- người giữ thuyền không đồng ý;
- dữ liệu bị stale.

Đó là các nguyên nhân khác nhau.

Không được trả chung:

> Action failed.

<a id="m26-t069-s148"></a>

### 118. Error Categories

**ValidationError:** Payload không hợp lệ.

**AuthorizationError:** Chủ thể không có quyền thực hiện.

**PreconditionFailed:** Điều kiện gameplay không thỏa.

**ConflictError:** State đã thay đổi.

**ResourceUnavailable:** Thiếu tài nguyên hoặc capability.

**RuleInvariantViolation:** Kết quả dự kiến phá bất biến.

**ContentDefinitionError:** Template hoặc reference không hợp lệ.

**PersistenceError:** Không commit/lưu được.

**DeterminismViolation:** Replay tạo kết quả khác.

**HistoricalConstraintViolation:** Một thao tác hoặc scenario phá invariant lịch sử đã khóa.

<a id="m26-t069-s149"></a>

### 119. Player Error vs Developer Error

Một player cố dùng thuyền bị hỏng là một Precondition Failure bình thường.

Một Event Template yêu cầu item không tồn tại vì reference sai là Content Error.

Một Rule Handler làm CommodityLot âm là Invariant Violation.

Không được trình bày các lỗi lập trình như thất bại gameplay hợp lệ.

Nếu có lỗi hệ thống, cần logging và recovery.

Không dùng LLM tạo một lời kể để che việc game state sai.

---

<a id="m26-t069-s150"></a>

## PHẦN XXVI — SECURITY, INTEGRITY & MODDING BOUNDARIES

<a id="m26-t069-s151"></a>

### 120. Single-player không cần chống cheat theo mô hình server-authoritative

Bản game này dự kiến vận hành offline.

Vì vậy mục tiêu bảo mật chính không phải ngăn player sửa file save bằng mọi giá.

Mục tiêu là:

- không chạy code không tin cậy từ content;
- không làm hỏng save;
- không thực thi script tùy tiện do LLM sinh;
- phát hiện content không tương thích;
- kiểm tra dữ liệu nhập;
- cô lập các công cụ debug khỏi build phát hành khi cần.

<a id="m26-t069-s152"></a>

### 121. Rule DSL không được là arbitrary code

Nếu sau này dùng Rule DSL, nó chỉ được hỗ trợ:

- đọc những field đã khai báo;
- điều kiện logic;
- so sánh;
- tính toán giới hạn;
- gọi những handler đã đăng ký;
- tạo những effect types hợp lệ.

Không cho phép:

- arbitrary file access;
- network calls;
- reflection không kiểm soát;
- eval code;
- gọi OS;
- tự thay đổi schema.

LLM offline không được có cách đưa một script độc hại vào Content Pack.

<a id="m26-t069-s153"></a>

### 122. Secret Formulas

Các công thức ẩn có thể được đóng gói trong compiled rule modules.

Nhưng nên hiểu đúng:

- Ẩn khỏi UI không có nghĩa không thể bị phân tích.
- Không cần sử dụng các thủ thuật mã hóa làm phức tạp gameplay core.
- Tính công bằng phải được kiểm chứng qua test và simulation.
- Các tham số cần được version hóa và audit bởi team.

Giá trị thiết kế của công thức bí mật nằm ở sự khám phá, không nằm ở việc tạo cảm giác kết quả không thể giải thích.

---

<a id="m26-t069-s154"></a>

## PHẦN XXVII — TEST ARCHITECTURE

<a id="m26-t069-s155"></a>

### 123. Test Pyramid

**Unit Tests:** Rule handlers, validation, transformations.

**Integration Tests:** Nhiều domain phối hợp xử lý một command.

**Property Tests:** Các bất biến chung trên nhiều trạng thái.

**Replay Tests:** Cùng inputs phải tái hiện state.

**Headless Tests:** World chạy dài hạn không có UI.

**Content Tests:** Event, character, historical content hợp lệ.

**Presentation Tests:** Knowledge được hiển thị đúng phạm vi.

**Performance Tests:** CPU, memory, save size và loading.

Mỗi tầng có mục tiêu riêng, không thể thay thế bằng vài playthrough thủ công.

<a id="m26-t069-s156"></a>

### 124. Invariant Suite bắt buộc

<a id="m26-t069-s157"></a>

#### Entity Integrity

- Mỗi EntityID duy nhất.
- References quan trọng phải hợp lệ.
- Entity đã archived vẫn có lịch sử.
- Không materialize trùng một persistent NPC.

<a id="m26-t069-s158"></a>

#### Character Integrity

- Character không thể vừa alive vừa dead.
- BirthTime không được nằm sau DeathTime.
- Skill/Profession có history phù hợp.
- Không tự chuyển Knowledge khi succession.

<a id="m26-t069-s159"></a>

#### Spatial Integrity

- Character có một authoritative current location.
- Travel có thời gian và route hợp lệ.
- Không có actor ở hai encounter đồng thời.

<a id="m26-t069-s160"></a>

#### Asset Integrity

- Exclusive ownership không được trùng trái quy tắc.
- Custody không được nhân đôi cùng một item.
- Commodity change có source/sink/transform hợp lệ.

<a id="m26-t069-s161"></a>

#### Temporal Integrity

- WorldClock tăng theo thứ tự hợp lệ.
- Event đến hạn không bị bỏ qua vô nguyên nhân.
- Một ScheduledEvent không commit hai lần.

<a id="m26-t069-s162"></a>

#### Knowledge Integrity

- NPC không biết một claim không có nguồn.
- UI không truy cập hidden truth.
- Rumor không tự thành fact.

<a id="m26-t069-s163"></a>

#### Historical Integrity

- Hard Anchor theo đúng contract.
- Adjustable outcomes không vi phạm protected invariants.
- Không sinh resource miễn phí để cứu Anchor.

<a id="m26-t069-s164"></a>

### 125. Property-Based Test Example

Sinh hàng nghìn trạng thái Inventory ngẫu nhiên.

Cho thực hiện:

- transfer;
- trade;
- borrow;
- return;
- loss;
- inheritance.

Sau mỗi command, kiểm tra:

- một item chỉ có một physical location authoritative;
- quyền sở hữu và custody nhất quán;
- tổng resource thay đổi theo các nguồn hợp lệ;
- command bị từ chối không làm thay đổi state.

Đây là cách kiểm tra tốt hơn chỉ viết một test thành công cho mỗi thao tác.

<a id="m26-t069-s165"></a>

### 126. Replay Test Example

Input:

```text
WorldSeed: 824731
RulesetVersion: 1.0
InitialSnapshot: snapshot_001

Commands:
1. AcceptContract
2. StartTravel
3. RepairBoat
4. AdvanceTime
5. DeliverCargo
```

Expected:

- cùng committed events;
- cùng state hash;
- cùng ownership;
- cùng scheduled events;
- cùng kết quả RNG trong phạm vi replay.

Presentation text có thể không cần giống từng byte nếu không thuộc authoritative state.

Tuy nhiên bản build chính thức sử dụng content templates versioned nên vẫn có thể kiểm tra phần lớn output khi cần.

<a id="m26-t069-s166"></a>

### 127. Headless 25-Year Test

Chạy nhiều World Seeds.

Theo dõi:

- population;
- births/deaths;
- household formation;
- career changes;
- profession availability;
- resource balance;
- faction outcomes;
- major events;
- history consistency;
- number of event loops;
- snapshot size.

Các chỉ số cần được so sánh giữa các phiên bản để tìm regression.

Ví dụ sau một patch, số household không có người lớn đột nhiên tăng quá mạnh.

Cần truy được nguyên nhân thay đổi rule, không chỉ xác định game trở nên khó hơn.

---

<a id="m26-t069-s167"></a>

## PHẦN XXVIII — OBSERVABILITY & DEVELOPER TOOLS

<a id="m26-t069-s168"></a>

### 128. Rule Inspector

Một developer có thể chọn:

- World Snapshot;
- Character;
- Event Candidate;
- RuleID.

Sau đó xem:

```text
Rule: transport.boat.repair.v3

Preconditions:
PASS - Boat exists
PASS - Actor has access
PASS - Materials available
PASS - Tool available
PASS - Actor capable

Resolution:
Skill Context: Available
Material Quality: Low
Repair Type: Temporary

Outcome:
Boat condition improved
Material consumed
Repair event committed
```

Các mô tả phải lấy từ rule execution thật.

Không viết diagnostic explanation bằng LLM.

<a id="m26-t069-s169"></a>

### 129. Causal Event Trace

Từ một kết quả, developer có thể truy ngược:

**Bridge Closed**

← **Bridge Damaged**

← **Flood Pressure Increased**

← **Environmental Update**

Tương tự, một Quest có thể liên kết tới sự kiện:

**Food Dispute**

← **Food Shortage**

← **Trade Route Closed**

← **Bridge Collapse**

Đây là công cụ bắt buộc cho một thế giới nhiều domain.

<a id="m26-t069-s170"></a>

### 130. State Diff

Developer cần so sánh:

**Snapshot trước command** và **Snapshot sau command**.

Hiển thị:

- field thay đổi;
- owner domain;
- events;
- rule;
- causal source;
- affected entity IDs.

Khi có bug kiểu một vật phẩm biến mất, State Diff giúp tìm đúng transaction.

<a id="m26-t069-s171"></a>

### 131. NPC Decision Inspector

Hiển thị:

- goals;
- known facts;
- available actions;
- preconditions;
- utility factors;
- selected action;
- RNG context;
- scheduled outcome.

Một NPC không đi làm có thể do:

- đang bị thương;
- workshop đóng;
- đường bị ngập;
- thiếu nguồn lực;
- có nghĩa vụ khác.

Developer cần thấy lý do thật, không suy đoán.

<a id="m26-t069-s172"></a>

### 132. Historical Scenario Inspector

Cho một Anchor, hiển thị:

- protected invariants;
- scenarios còn hợp lệ;
- điều kiện chưa đạt;
- resource dependencies;
- player interventions đã ảnh hưởng;
- nguy cơ scenario bất khả thi.

Đây là công cụ quan trọng nhất để giải quyết xung đột giữa lịch sử và simulation.

---

<a id="m26-t069-s173"></a>

## PHẦN XXIX — END-TO-END TECHNICAL SCENARIO

<a id="m26-t069-s174"></a>

### 133. Tình huống: Player sửa thuyền, sau đó nhiều năm chuyển giao nghề

Kịch bản này kiểm tra nhiều domain.

<a id="m26-t069-s175"></a>

#### Giai đoạn 1 — Character Generation

Engine tạo Mân:

- 19 tuổi;
- household vận tải;
- biết bơi;
- có kinh nghiệm sửa thuyền cơ bản;
- có mentor.

Những dữ liệu này có origin life-history hợp lệ.

<a id="m26-t069-s176"></a>

#### Giai đoạn 2 — Nhận công việc

Một người chủ thuyền có nhu cầu sửa thuyền.

Event Engine tạo candidate từ BoatCondition và TransportNeed.

NPC AI quyết định tìm người giúp.

World Director expose qua dialogue hợp lệ.

<a id="m26-t069-s177"></a>

#### Giai đoạn 3 — Thực hiện repair

Player gửi `RepairBoatCommand`.

Engine:

1. Kiểm tra quyền tiếp cận.
2. Kiểm tra vật liệu/công cụ.
3. Kiểm tra skill.
4. Resolve kết quả.
5. Consume vật liệu.
6. Update BoatCondition.
7. Commit RepairEvent.
8. Update Social/Knowledge follow-ups.

<a id="m26-t069-s178"></a>

#### Giai đoạn 4 — Progression

Character Development ghi nhận meaningful practice.

Nếu Mân tiếp tục thực hiện các công việc liên quan qua nhiều năm, Profession Evidence được tích lũy.

Class Emergence có thể phát triển nếu đủ pattern và milestone.

Không phải một lần sửa thuyền làm unlock Class.

<a id="m26-t069-s179"></a>

#### Giai đoạn 5 — Time Skip

Mân học nghề và làm việc trong hai năm.

World Scheduler xử lý:

- mentor;
- workshop;
- nguồn vật liệu;
- nghề;
- gia đình;
- những biến cố vùng.

<a id="m26-t069-s180"></a>

#### Giai đoạn 6 — Death/Succession

Sau nhiều năm, Mân qua đời.

Một apprentice đủ điều kiện có thể trở thành successor.

Họ giữ Skill của chính mình.

Không nhận toàn bộ Skill của Mân.

Nhưng có thể giữ:

- Technique từng được dạy;
- công cụ được trao hợp lệ;
- network nghề;
- lời hứa với workshop.

<a id="m26-t069-s181"></a>

#### Giai đoạn 7 — Legacy

Nhiều năm sau, một workshop vẫn sử dụng phương pháp do Mân từng truyền.

Nếu người học trò tiếp tục dạy người khác:

Professional Legacy lan rộng.

Nếu không còn ai sử dụng:

Legacy có thể suy yếu hoặc mất.

<a id="m26-t069-s182"></a>

#### Giai đoạn 8 — Chronicle

Chronicle ghi lại:

- nguồn gốc nghề;
- các công việc quan trọng;
- mentor;
- apprentice;
- những Technique truyền lại;
- sự thay đổi của workshop.

Những entry player nhìn thấy được lọc qua Knowledge hoặc Player Experience Archive tùy ngữ cảnh.

Toàn bộ chuỗi trên vận hành bằng code và dữ liệu.

Không có bước nào cần LLM quyết định một kết quả gameplay.

---

<a id="m26-t069-s183"></a>

## PHẦN XXX — IMPLEMENTATION CONTRACTS CẦN KHÓA

<a id="m26-t069-s184"></a>

### 134. Domain Ownership Contract

Mỗi domain bắt buộc khai báo:

```text
Domain Name
Owned Entities
Owned State Fields
Allowed Commands
Emitted Events
Consumed Events
Read Dependencies
Invariants
Failure Modes
Persistence Requirements
```

Một module mới không được tích hợp vào core nếu chưa có contract này.

<a id="m26-t069-s185"></a>

### 135. Rule Handler Contract

Mỗi handler phải khai báo:

```text
RuleID
RuleVersion
Input Types
Required Read Models
Preconditions
Deterministic Random Requirements
Allowed State Effects
Emitted Events
Failure Conditions
Invariant Dependencies
```

Không để Rule Handler tùy ý tạo hoặc sửa những loại dữ liệu ngoài phạm vi cho phép.

<a id="m26-t069-s186"></a>

### 136. Event Contract

Mỗi loại event phải khai báo:

```text
EventType
PayloadSchema
OwnerDomain
Causation Requirements
Commit Semantics
Subscriber Domains
Idempotency Policy
Version
```

Event consumers không được dựa vào trình tự không được bảo đảm.

Nếu một consumer cần kết quả của domain khác, dependency đó phải được khai báo hoặc được orchestrate theo transaction workflow.

<a id="m26-t069-s187"></a>

### 137. Content Template Contract

Mỗi template phải khai báo:

```text
TemplateID
TemplateVersion
ContentCategory
RequiredRoles
EligibilityRules
RuleHandlerReferences
OutcomeDefinitions
NarrativeAssets
HistoricalClassification
ValidationLevel
```

Template phải được compile và kiểm chứng trước runtime.

<a id="m26-t069-s188"></a>

### 138. Knowledge Projection Contract

Mọi player-facing query về World phải biết:

```text
ObserverID
RequestedSubject
KnowledgeScope
WorldTime
AllowedSources
ProjectionType
```

Không được mặc định `ObserverID = omniscient`.

Ngoại lệ chỉ dành cho developer diagnostics hoặc các giao diện ngoài fiction được định nghĩa rõ, chẳng hạn phần giải thích cơ chế gameplay.

---

<a id="m26-t069-s189"></a>

## PHẦN XXXI — IMPLEMENTATION PHASING

<a id="m26-t069-s190"></a>

### 139. Phase A — Foundation

Triển khai:

- Entity Registry;
- WorldClock;
- Commands;
- Rule Registry;
- Deterministic RNG;
- Commit Pipeline;
- Event Journal;
- Save/Load.

Acceptance:

Một command có thể được kiểm thử, commit, lưu, load và replay ổn định.

<a id="m26-t069-s191"></a>

### 140. Phase B — Character & Household

Triển khai:

- Character Generator;
- Life History;
- Attributes/Skills;
- Household;
- Profession;
- Basic Relationship;
- Aging.

Acceptance:

Một settlement nhỏ có nhân vật với lịch sử hợp lệ và thay đổi theo thời gian.

<a id="m26-t069-s192"></a>

### 141. Phase C — Event & Knowledge

Triển khai:

- Event Templates;
- Eligibility;
- NPC Decision;
- Domain Events;
- Story Threads;
- Knowledge;
- Rumor;
- Journal Projection.

Acceptance:

Một NPC tự phát sinh hành động từ World State; player có thể biết và can thiệp thông qua kênh hợp lệ.

<a id="m26-t069-s193"></a>

### 142. Phase D — Physical Systems

Triển khai:

- Inventory/Asset Rights;
- Travel Graph;
- Exploration;
- Repair/Crafting;
- Health/Injury;
- Combat.

Acceptance:

Một hành động vật lý có thể tạo hậu quả lâu dài qua nhiều domain.

<a id="m26-t069-s194"></a>

### 143. Phase E — Long-Term Simulation

Triển khai:

- Time Skip;
- Household Evolution;
- Career;
- Death;
- Succession;
- Legacy;
- Chronicle.

Acceptance:

Một world chạy qua nhiều năm mà không phá data invariants; succession hợp lệ.

<a id="m26-t069-s195"></a>

### 144. Phase F — Historical & Content Scaling

Triển khai:

- Historical Anchor Controller;
- World Director;
- Content Compiler;
- Offline LLM-assisted pipeline;
- Cultural/Historical validators;
- Headless historical simulation.

Acceptance:

Một Historical Anchor có thể diễn ra theo các kịch bản hợp lệ, không cần LLM runtime hoặc tác động phi nhân quả.

---

<a id="m26-t069-s196"></a>

## PHẦN XXXII — ARCHITECTURE QUALITY GATE

<a id="m26-t069-s197"></a>

### 145. Điều kiện nghiệm thu Module 26 ở mức thiết kế

Module được xem là hoàn thành khi:

- Có sơ đồ logical architecture.
- Có phân tách Foundation, Domain, Application, Infrastructure, Presentation.
- Có Canonical Identity rules.
- Có WorldClock thống nhất.
- Có spatial taxonomy.
- Có Character, Household, Knowledge, Asset và Narrative data contracts.
- Có ownership rules cho các state quan trọng.
- Có Command Envelope và Command Result.
- Có Event Template, Candidate, Scheduled Event và Committed Event tách biệt.
- Có deterministic RNG contract.
- Có time scheduler contract.
- Có Historical Anchor contract.
- Có succession contract.
- Có persistence/versioning policy.
- Có content validation policy.
- Có test architecture.
- Có các điều kiện bắt đầu prototype.

Những nội dung trên được xác định trong tài liệu này ở cấp kiến trúc.

Chúng chưa thay thế việc viết schema thực tế và các class/interface hoàn chỉnh.

<a id="m26-t069-s198"></a>

### 146. Điều kiện nghiệm thu bằng code

Trước khi tiếp tục mở rộng full content, prototype phải chứng minh:

**Test 1:** Không thể tạo hai NPC có cùng persistent ID.

**Test 2:** Không thể chuyển cùng một item cho hai người trong hai transaction xung đột.

**Test 3:** NPC đã chết không tiếp tục hành động.

**Test 4:** Một actor không tự biết hidden event.

**Test 5:** RouteKnowledge không tự đồng bộ với RouteState.

**Test 6:** Cùng save, command và ruleset cho kết quả authoritative giống nhau.

**Test 7:** Time Skip không bỏ qua event bắt buộc.

**Test 8:** Character Generation không tạo Life History mâu thuẫn.

**Test 9:** Event Template thiếu actor/resource bị loại đúng.

**Test 10:** Quest View không thay đổi World Truth.

**Test 11:** Successor không nhận Skill, Class hoặc Knowledge không có nguồn truyền.

**Test 12:** Một Historical Anchor không tự tạo vật liệu hoặc actor miễn phí.

**Test 13:** Một domain lỗi trước commit không để lại state thay đổi dở dang.

**Test 14:** Một command retry không làm resource bị tiêu thụ hai lần.

**Test 15:** Game có thể chạy headless không cần presentation hoặc LLM.

Đây là bộ acceptance tests đầu tiên cần hiện thực hóa.

---

<a id="m26-t069-s199"></a>

## PHẦN XXXIII — NHỮNG QUYẾT ĐỊNH KHÔNG NÊN KHÓA QUÁ SỚM

<a id="m26-t069-s200"></a>

### 147. Những lựa chọn còn phải benchmark

Không nên tuyên bố đã chốt tuyệt đối:

- Unity hay Godot nếu team chưa xác định;
- SQLite hay một persistence backend khác cho toàn bộ lifecycle;
- Entity-Component-System hay object-oriented aggregate cho mọi domain;
- thuật toán PRNG cụ thể;
- tick duration chính xác;
- snapshot interval;
- số lượng NPC active tối đa;
- kích thước cache;
- scheduler batching strategy;
- numeric precision cụ thể;
- dung lượng save mục tiêu;
- toàn bộ DB index.

Những điều đó có thể quyết định qua prototype mà không làm thay đổi các quy tắc kiến trúc cốt lõi.

<a id="m26-t069-s201"></a>

### 148. Những lựa chọn phải khóa trước khi code song song

Ngược lại, cần chốt ngay:

- ai sở hữu World State;
- ai được commit mutation;
- format identity;
- command/event semantics;
- WorldClock semantics;
- knowledge visibility;
- item/resource ownership;
- event ordering contract;
- versioning policy;
- protected historical invariants;
- no-LLM-runtime decision rule.

Nếu các phần này thay đổi giữa chừng sau khi nhiều module đã viết code, chi phí refactor sẽ rất lớn.

---

<a id="m26-t069-s202"></a>

## PHẦN XXXIV — TECHNICAL DECISION SUMMARY

<a id="m26-t069-s203"></a>

### 149. Các quyết định đề xuất khóa

<a id="m26-t069-s204"></a>

#### Runtime

1. Game vận hành offline bằng deterministic rule-based engine.
2. Một authoritative simulation core chịu trách nhiệm World State.
3. Modular Monolith là hướng kiến trúc mặc định.
4. Presentation không sở hữu World Truth.
5. LLM không tham gia gameplay decision trong runtime.

<a id="m26-t069-s205"></a>

#### Data

6. Definition ID và Instance ID tách biệt.
7. Persistent entities có stable identity.
8. Domain ownership rõ ràng.
9. Physical state và social rights có mô hình riêng.
10. Character Knowledge tách khỏi World Truth.
11. Quest là player-facing projection.
12. Chronicle là historical projection.
13. Route Graph là nguồn sự thật về di chuyển chiến lược.
14. Commodity Ledger là nguồn tài nguyên vật chất.
15. Supply Days là derived metric.

<a id="m26-t069-s206"></a>

#### Events

16. Command khác Committed Event.
17. Event Candidate khác Event Instance đã được commit.
18. Rule Engine xác định và xác thực outcome.
19. Cross-domain mutation phải có consistency boundary.
20. Event ordering phải ổn định.
21. Scheduled events không được commit hai lần.
22. Follow-up events cần loop protection.
23. World Director không được thay đổi truth.

<a id="m26-t069-s207"></a>

#### Determinism

24. Random được truyền qua controlled context.
25. Không dùng RNG toàn cục tùy tiện.
26. Seed, ruleset, state và ordering phải được quản lý.
27. Replay được kiểm thử tự động.
28. Numeric determinism được xác nhận trên nền tảng mục tiêu.
29. Save phải có version và integrity policy.

<a id="m26-t069-s208"></a>

#### Long-Term Simulation

30. WorldClock thống nhất.
31. NPC dùng simulation tiers.
32. Character có lịch sử nhân quả.
33. Aging và Career đi qua Life Events.
34. Death có trạng thái và hậu quả rõ.
35. Succession không phải copy character.
36. Legacy tồn tại qua các vật mang di sản và điều kiện duy trì.
37. Historical Anchors có causal contracts.

<a id="m26-t069-s209"></a>

#### Content

38. LLM hỗ trợ offline content production.
39. Content có schema, references và validation.
40. Event Templates chỉ gọi authorized handlers.
41. Không thực thi script tùy ý từ generated content.
42. Nội dung lịch sử phải có classification.
43. Major narrative content cần human review.
44. Content Pack phải được version hóa.

<a id="m26-t069-s210"></a>

#### Quality

45. Headless simulation là bắt buộc.
46. Có test cho data invariants.
47. Có deterministic replay.
48. Có content validation.
49. Có causal trace/debug tools.
50. Vertical Slice cần được nghiệm thu trước khi mở rộng toàn bộ thế giới.

---

<a id="m26-t069-s211"></a>

## PHẦN XXXV — KẾT LUẬN MODULE 26

<a id="m26-t069-s212"></a>

### 150. Từ GDD sang Technical Architecture

Các module trước đã trả lời:

> Thế giới phải vận hành như thế nào?

Module 26 xác định:

> Những thành phần phần mềm nào sở hữu, xử lý, lưu trữ và trình bày các sự kiện đó?

Sự khác biệt là rất quan trọng.

Một GDD có thể nói:

> NPC tiếp tục sống khi player rời đi.

Nhưng kiến trúc kỹ thuật phải trả lời:

- NPC được cập nhật khi nào?
- Điều gì kích hoạt quyết định?
- Dữ liệu nào được sử dụng?
- Actor biết những thông tin gì?
- Ai được commit kết quả?
- Nếu event trùng thì xử lý thế nào?
- Khi load save thì tái dựng ra sao?
- Khi NPC chết thì những domain nào được cập nhật?
- Làm sao chứng minh không xảy ra lỗi?

Module 26 đã tạo nền để trả lời những câu hỏi này bằng hợp đồng kỹ thuật.

<a id="m26-t069-s213"></a>

### 151. Tuyên ngôn kiến trúc

Thế giới game không được vận hành bằng một tập hợp script riêng lẻ mà mỗi script tự thay đổi mọi thứ.

Nó cần một hệ thống:

**Một sự thật authoritative.**

**Nhiều domain có trách nhiệm rõ ràng.**

**Những hành động được kiểm tra trước khi commit.**

**Những sự kiện có nguồn gốc.**

**Những nhân vật có tri thức giới hạn.**

**Những quy tắc có thể tái hiện.**

**Những hậu quả được lưu dài hạn.**

**Một presentation layer đủ tốt để người chơi hiểu và cảm nhận thế giới.**

Đây là nền kỹ thuật giúp hàng triệu tổ hợp diễn biến có thể được tạo ra từ code, dữ liệu và các công thức đã thiết kế.

Không cần một LLM liên tục sáng tác điều gì sẽ xảy ra.

<a id="m26-t069-s214"></a>

### 152. Điều kiện chuyển sang Module 27

Trước khi viết công thức cụ thể, cần coi những hợp đồng dưới đây là baseline:

- Authoritative State Ownership;
- Command/Commit Pipeline;
- Event Semantics;
- Deterministic RNG Context;
- WorldClock;
- Character/Knowledge Model;
- Asset/Commodity Model;
- Historical Anchor Contract.

Module 27 sẽ định nghĩa các công thức dựa trên những hợp đồng này.

Không nên viết công thức trước rồi để mỗi domain tự diễn giải đơn vị, trọng số và điều kiện.

---

<a id="m26-t069-s215"></a>

## PHẦN XXXVI — MODULE TIẾP THEO

<a id="m26-t069-s216"></a>

### 153. Module 27 — RULE ENGINE & FORMULA SPECIFICATION

Đây là bước biến kiến trúc thành những thuật toán thực thi thực tế.

Các nhóm phải thiết kế:

**Character Generation Formulas:** Phân bố tuổi, xuất thân, aptitude, Life History và Skill Development.

**Action Resolution:** Kết quả hành động dựa trên Attributes, Skills, Knowledge, Equipment, Condition và Context.

**Class Emergence:** Điều kiện hình thành Class từ experience, identity và recognition.

**NPC Decision:** Utility, Goals, Needs, Morality và Social Obligations.

**Event Eligibility & Selection:** Trọng số, pressure, scarcity, actor selection và deterministic RNG.

**Economy:** Production, Consumption, Trade, Resource Flow và price formation.

**Travel & Ecology:** Route cost, environmental conditions, creature population, encounter eligibility.

**Combat & Injury:** Attack, Defense, Stamina, Wounds, Morale, Retreat và Recovery.

**Knowledge & Rumor:** Perception, confidence, transmission, distortion, forgetting.

**Aging & Succession:** Development, health, mortality, succession eligibility và legacy persistence.

**World Director:** Opportunity scoring, pacing và exposure priority trong giới hạn nhân quả.

Điều cần bảo vệ là **không chỉ xây công thức tạo ra kết quả có vẻ hợp lý**, mà còn chứng minh công thức:

- không tạo exploit;
- không phá invariants;
- không khiến mọi playthrough hội tụ vào một build tối ưu;
- không làm gameplay ngoài chiến đấu mất giá trị;
- không tạo event vô hạn;
- không làm thế giới sụp đổ vì hiệu ứng tích lũy không được kiểm soát;
- và có thể cân chỉnh, tái hiện, kiểm thử.

**Kết luận cuối:** Module 26 xác lập *kiến trúc và hợp đồng dữ liệu*. Module 27 phải xác lập *quy luật toán học, xác suất và thuật toán*. Hai module kết hợp mới tạo ra bản đặc tả thực thi cần thiết để bắt đầu xây dựng engine của game.

---

<a id="m26-t069-s217"></a>

### Kết luận nghiên cứu và khuyến nghị triển khai

Module 26 đã đưa ra kiến trúc tham chiếu cho **engine thế giới sống vận hành hoàn toàn bằng code**. Tôi đặc biệt khuyến nghị không bỏ qua ba phần khi giao việc cho coding agent: **Domain Ownership Contract, Command/Event Contract và Deterministic Replay Test**. Thiếu một trong ba, các module riêng lẻ có thể chạy được nhưng không có cơ sở bảo đảm toàn bộ thế giới nhất quán.

Có một điểm cần nói rõ về phạm vi hoàn thành: chúng ta đã có **bản thiết kế kiến trúc chi tiết**, nhưng chưa có bộ schema vật lý hoàn chỉnh, class definitions đã biên dịch hoặc benchmark hiệu năng. Đó là các đầu ra của bước hiện thực hóa kỹ thuật, không nên tuyên bố đã được kiểm chứng chỉ dựa trên GDD.

Các nguồn kỹ thuật dùng để đối chiếu gồm <Link url="https://json-schema.org/draft/2020-12/" title="JSON Schema Draft 2020-12"/>, <Link url="https://www.sqlite.org/foreignkeys.html" title="SQLite Foreign Key Constraints"/>, <Link url="https://www.sqlite.org/wal.html" title="SQLite Write-Ahead Logging"/>, và <Link url="https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation" title="Microsoft — Domain Events Design"/>. Những nguồn này là cơ sở tham khảo kỹ thuật; các quy tắc game và lựa chọn kiến trúc cụ thể vẫn là thiết kế của dự án.

**Bước kế tiếp:** **Module 27 — Rule Engine & Formula Specification**, nơi cần đi sâu vào công thức có thể triển khai, cách kết hợp hàng triệu nhánh hợp lệ, xác suất có điều kiện, cơ chế chống exploit và thuật toán quyết định của NPC. Đây sẽ là bước kỹ thuật quan trọng nhất để hiện thực hóa yêu cầu rằng **engine — không phải LLM — quyết định toàn bộ diễn biến gameplay**.

<!-- SOURCE T069 END -->

