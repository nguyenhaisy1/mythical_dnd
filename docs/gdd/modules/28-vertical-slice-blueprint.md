# Module 28 — Vertical Slice Implementation Blueprint

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T077](../../source/conversation.md#t077)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Scope slice tập trung Tam Giang với Bến Hạc/Phong Châu là external boundary node cho trade, không dời canon. Task milestones và acceptance gates giữ nguyên; số content là budget thử nghiệm. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Modules 01–27, minimal world fixture và implementation backlog. |
| Đầu ra / bên sở hữu | Vertical-slice milestones, task contracts và QA evidence; chưa phải build đã chạy. |
| Đọc cùng | 29; VERTICAL-SLICE, BACKLOG, QA-PLAN.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Chứng minh nghề–world–knowledge–narrative–time–legacy liên kết và player không phải tác nhân duy nhất. |
| Việc chưa được chứng minh | Full slice roster/compiled content, three professions, client and playtest còn pending; xem Slice Content Contract. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T077 BEGIN -->

<a id="m28-t077-s001"></a>

## GDD MODULE 28
<a id="m28-t077-s002"></a>

## VERTICAL SLICE IMPLEMENTATION BLUEPRINT
<a id="m28-t077-s003"></a>

### Kế hoạch hiện thực hóa, triển khai và nghiệm thu bản thử nghiệm đầu tiên

**Dự án:** Living World Narrative RPG — thế giới hư cấu lịch sử cuối Văn Lang, chuyển tiếp Âu Lạc và Cổ Loa  
**Phiên bản:** 1.0 — Implementation Planning Baseline  
**Trạng thái:** Đặc tả triển khai đề xuất, chưa có bản build được kiểm chứng  
**Đối tượng sử dụng:** Technical Lead, Game Architect, Gameplay Programmer, Simulation Engineer, Narrative Designer, QA Engineer và AI Coding Agent  
**Tài liệu nền:** GDD Modules 01–27  
**Nguyên tắc bắt buộc:** Runtime gameplay do code, dữ liệu và rule quyết định; không sử dụng LLM để sinh quyết định hoặc thay đổi World State theo thời gian thực.

---

<a id="m28-t077-s004"></a>

## PHẦN I — VERTICAL SLICE PHẢI CHỨNG MINH ĐIỀU GÌ?

<a id="m28-t077-s005"></a>

### 1. Phân biệt Technical Prototype, Vertical Slice và MVP

Ba khái niệm này không được sử dụng thay thế nhau.

<a id="m28-t077-s006"></a>

#### Technical Prototype

Mục tiêu của Technical Prototype là chứng minh một số giả thuyết kỹ thuật.

Ví dụ:

- Một command có thể thay đổi World State.
- Cùng seed và command sequence tạo cùng kết quả.
- Một NPC có thể tự quyết định hành động.
- Một event có thể tạo hậu quả liên domain.
- Save/Load không làm sai lịch sử.

Technical Prototype có thể chỉ cần giao diện debug.

Nó chưa cần là một trải nghiệm chơi hấp dẫn.

<a id="m28-t077-s007"></a>

#### Vertical Slice

Vertical Slice là một phần nhỏ nhưng đủ chiều sâu của game hoàn chỉnh.

Nó phải có:

- một nhân vật có xuất thân và lịch sử;
- một không gian thế giới có thể khám phá;
- những người sống trong cộng đồng;
- hoạt động nghề nghiệp;
- quan hệ và hội thoại;
- lựa chọn của người chơi;
- hậu quả;
- sự tiến triển của thời gian;
- giao diện đủ sử dụng;
- presentation cơ bản.

Vertical Slice không cần nhiều nội dung, nhưng những nội dung được chọn phải liên kết thực sự.

<a id="m28-t077-s008"></a>

#### MVP

MVP là phạm vi sản phẩm nhỏ nhất đủ để kiểm chứng khả năng duy trì trải nghiệm qua nhiều phiên chơi.

Nó đòi hỏi nhiều content và hệ thống progression hơn Vertical Slice.

**Quyết định:** Phát triển Technical Prototype trước, sau đó xây Vertical Slice trên chính engine đó. Không làm một bản demo riêng rồi bỏ toàn bộ code khi bắt đầu game thật.

<a id="m28-t077-s009"></a>

### 2. Giả thuyết sản phẩm quan trọng nhất

Vertical Slice phải trả lời:

> Một nhân vật được tạo ra từ hoàn cảnh sống có thể tham gia một sự kiện có thật trong thế giới, đưa ra quyết định, phát triển năng lực và quan hệ, rồi chứng kiến hậu quả tiếp tục tồn tại mà không cần một kịch bản tuyến tính hoặc LLM realtime điều khiển hay không?

Đây là câu hỏi cốt lõi.

Nếu câu trả lời là không, việc xây thêm 12 vùng, hàng nghìn NPC và sáu hồi lịch sử sẽ không giải quyết được vấn đề.

<a id="m28-t077-s010"></a>

### 3. Sáu bằng chứng phải thu được

**Bằng chứng về nhân quả:** Một vấn đề có nguyên nhân từ World State, không chỉ xuất hiện vì player tới gần.

**Bằng chứng về agency:** Ít nhất ba cách can thiệp có kết quả cơ học khác nhau, và lựa chọn không can thiệp cũng có diễn biến hợp lệ.

**Bằng chứng về đời sống:** Nhân vật có hoạt động có ý nghĩa ngoài combat.

**Bằng chứng về NPC autonomy:** NPC có thể thực hiện kế hoạch riêng khi player vắng mặt.

**Bằng chứng về tính lâu dài:** Hậu quả của sự kiện còn tồn tại sau một khoảng thời gian game đáng kể.

**Bằng chứng về kiến trúc:** Có thể save, load, replay, test và debug mà không phụ thuộc LLM hoặc presentation.

Không được tuyên bố Vertical Slice hoàn thành chỉ vì một quest có thể được chơi từ đầu đến cuối.

---

<a id="m28-t077-s011"></a>

## PHẦN II — PHẠM VI CHÍNH THỨC

<a id="m28-t077-s012"></a>

### 4. Địa lý được đưa vào bản thử nghiệm

**Vùng:** Tam Giang.

Đây là vùng phù hợp để kiểm thử vì gameplay đường sông liên kết được:

- vận tải;
- môi trường;
- kinh tế;
- nghề nghiệp;
- tài sản;
- quan hệ;
- cứu hộ;
- thương tích;
- điều tra;
- biến động theo thời gian.

Đề xuất một mạng địa điểm nhỏ.

| Địa điểm | Vai trò | Gameplay |
|---|---|---|
| Bến Vạc | Settlement trung tâm | Nghề nghiệp, household, thương mại, quan hệ |
| Bãi Lau | Cộng đồng phụ | Sản xuất, chữa bệnh, quan hệ gia đình |
| Bến Hạc | Điểm trung chuyển | Vận tải, thỏa thuận, tuyến đường |
| Cồn Mây | Site khám phá | Dấu vết, địa hình, nguy hiểm môi trường |
| Khúc Nước Xiết | Encounter Area | Điều khiển thuyền, cứu hộ, nguy hiểm |
| Đường Rừng Ven Sông | Tuyến thay thế | Khám phá, thông tin tuyến, supplies |

Các tên địa điểm này là **fixture của bản thử nghiệm**, không mặc nhiên xác nhận chúng đã nằm trong danh mục 60 settlement chính thức của World Design.

Trước khi chuyển sang full game, cần đối chiếu và chuẩn hóa với World Geography Canon.

<a id="m28-t077-s013"></a>

### 5. World Graph thử nghiệm

Đề xuất:

```text
                      [Bãi Lau]
                          |
                       Đường bộ
                          |
[Bến Vạc] ---- Sông ---- [Bến Hạc]
    |                       |
    |                       |
  Rừng ven sông          Đường mòn
    |                       |
    +------- [Cồn Mây] -----+
                 |
          [Khúc Nước Xiết]
```

Graph này phải có:

- directed route edges khi cần;
- travel time;
- capacity;
- environment condition;
- route availability;
- actor knowledge;
- nguồn thông tin về tuyến.

Không dùng một hình ảnh bản đồ tĩnh làm nguồn sự thật.

<a id="m28-t077-s014"></a>

### 6. Quy mô nội dung đề xuất

| Thành phần | Phạm vi Vertical Slice |
|---|---|
| Vùng | 1 |
| Settlement/community | 3 |
| Site/encounter area chính | 2–3 |
| Route edges | 6–10 cạnh có hướng |
| NPC có identity | Khoảng 20–30 |
| Household | 6–8 |
| Faction hoặc nhóm lợi ích địa phương | 2–3 |
| Profession triển khai sâu | 3 |
| Companion có autonomy rõ | 1–2 |
| Story Thread chính cho demo | 3 |
| Environmental/Event Templates | 8–12 |
| Combat Encounter | 1 loại đủ kiểm thử |
| Multi-year simulation | Có, qua headless và Time Skip |
| Succession | Một chuỗi thử nghiệm hoàn chỉnh |

Đây là **ngân sách nội dung khởi đầu**, không phải con số tối ưu đã được chứng minh.

Nếu 20 NPC chưa tạo đủ mạng quan hệ có ý nghĩa, cần điều chỉnh cách thiết kế NPC trước khi tăng số lượng.

<a id="m28-t077-s015"></a>

### 7. Những hệ thống bắt buộc có phiên bản hoạt động

Vertical Slice phải có phiên bản vận hành của:

**Foundation:** WorldClock, Entity Registry, Rule Engine, Command Processor, RNG, Event Journal, Persistence.

**Character:** Origin, Life History, Attributes, Skills, Profession, Health.

**World:** Settlement, Household, Route, Environment, Economy tối thiểu.

**NPC:** Goals, Knowledge, Utility Decision, Scheduling.

**Social:** Relationship, Dialogue Act, Promise/Agreement.

**Narrative:** Event Template, Story Thread, Quest Projection, Chronicle.

**Gameplay:** Travel, Repair/Crafting, Treatment, một Combat Encounter, Local Exploration.

**Long-Term:** Time Skip, Aging, Death, Succession, Legacy tối thiểu.

**Presentation:** HUD, Character, Dialogue, Map, Journal, Inventory và Combat UI.

Không cần làm đầy đủ mọi tính năng nâng cao của các hệ này.

Nhưng các subsystem được chọn phải thực sự trao đổi trạng thái, không phải màn hình giả lập đứng độc lập.

<a id="m28-t077-s016"></a>

### 8. Những thứ không đưa vào nội dung chơi chính của Vertical Slice

Chưa triển khai đầy đủ:

- toàn bộ 12 vùng;
- 60 settlement;
- hàng nghìn NPC;
- mọi Class và Technique;
- hệ quân sự quy mô quốc gia;
- cả sáu hồi campaign;
- toàn bộ Mạch Lạc;
- Kim Quy và Nỏ thần dưới dạng gameplay hoàn chỉnh;
- mọi loại Dungeon;
- mọi dây chuyền Crafting;
- toàn bộ hệ Myth Formation.

Tuy nhiên phải có **kiểm thử hợp đồng kỹ thuật** cho Historical Anchor, Mạch và Legacy để bảo đảm khả năng tích hợp sau này.

Không thể bỏ hoàn toàn những interface này rồi kỳ vọng thêm vào sau mà không refactor.

---

<a id="m28-t077-s017"></a>

## PHẦN III — TECHNOLOGY STACK & RUNTIME TOPOLOGY

<a id="m28-t077-s018"></a>

### 9. Stack tham chiếu

Đề xuất:

| Thành phần | Công nghệ | Vai trò |
|---|---|---|
| Simulation Core | C# library, hướng tới .NET Standard 2.1 | Rules, entities, state và deterministic simulation |
| Headless Runner | Ứng dụng .NET console | Test, replay, world simulation |
| Game Client | Unity | Rendering, input, audio, UI |
| Persistence | SQLite qua adapter | Current State, event journal, snapshot metadata |
| Content Authoring | JSON + JSON Schema | Definitions và Event Templates |
| Automated Tests | .NET testing framework và Unity Test Framework | Core tests, Edit Mode, Play Mode |
| Debug Tools | CLI trước, UI công cụ sau | Rule Inspector, Event Trace, State Diff |
| Version Control | Git | Code, schema, content và ADR |
| CI | Hệ CI chạy được .NET build/test | Regression, content validation, benchmark |

Không cần một web backend hay microservice cho gameplay offline.

<a id="m28-t077-s019"></a>

### 10. Cách tách Simulation Core khỏi Unity

Simulation Core không được import:

```text
UnityEngine
UnityEditor
MonoBehaviour
ScriptableObject
```

để quyết định World State.

Presentation Layer mới sử dụng các API này.

Sơ đồ:

```text
               UNITY APPLICATION
                      |
                Input Adapter
                      |
                      v
             Application Commands
                      |
                      v
             SIMULATION CORE
                (C# Library)
                      |
          +-----------+-----------+
          |                       |
      World State             Domain Rules
          |                       |
          +-----------+-----------+
                      |
                 Commit Result
                      |
                      v
               Read Projections
                      |
                      v
                  Unity UI
```

Headless Runner gọi cùng Command Processor.

Không được có hai cách giải quyết khác nhau cho cùng một hành động, một ở Unity và một ở headless.

<a id="m28-t077-s020"></a>

### 11. Quy tắc tương thích thư viện

Với core dùng chung:

- Tránh API chỉ tồn tại trên runtime .NET mới nhưng không được Unity hỗ trợ.
- Không giả định package SQLite bất kỳ đều chạy được trên mọi nền tảng Unity.
- Kiểm tra serialization compatibility.
- Kiểm tra các thư viện cần native binary.
- Không phụ thuộc trực tiếp vào hệ scene hoặc Unity lifecycle.

Trước khi xây domain lớn, phải có **Compatibility Spike**:

1. Build core library.
2. Reference trong Headless Runner.
3. Reference cùng library trong Unity.
4. Thực hiện một command giống nhau.
5. Kiểm tra output/state hash.
6. Save/load một state nhỏ qua persistence adapter.

Nếu không đạt, giải quyết ngay tại Foundation, không để đến khi có hàng chục subsystem.

---

<a id="m28-t077-s021"></a>

## PHẦN IV — REPOSITORY & PROJECT STRUCTURE

<a id="m28-t077-s022"></a>

### 12. Cấu trúc repository

Đề xuất cấu trúc có khả năng mở rộng.

```text
living-world-rpg/
│
├── README.md
├── CONTRIBUTING.md
├── ARCHITECTURE.md
├── Directory.Build.props
│
├── docs/
│   ├── gdd/
│   ├── adr/
│   ├── contracts/
│   ├── runbooks/
│   ├── qa/
│   └── content-guides/
│
├── engine/
│   ├── Foundation/
│   │   ├── Identity/
│   │   ├── Time/
│   │   ├── Random/
│   │   ├── Commands/
│   │   ├── Events/
│   │   ├── Rules/
│   │   └── Validation/
│   │
│   ├── Domain/
│   │   ├── Characters/
│   │   ├── Households/
│   │   ├── Social/
│   │   ├── Knowledge/
│   │   ├── Assets/
│   │   ├── Economy/
│   │   ├── Travel/
│   │   ├── Health/
│   │   ├── Combat/
│   │   ├── Narrative/
│   │   └── Legacy/
│   │
│   ├── Application/
│   │   ├── CommandPipeline/
│   │   ├── SimulationScheduler/
│   │   ├── Projection/
│   │   └── WorldDirector/
│   │
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   ├── ContentLoading/
│   │   └── Diagnostics/
│   │
│   └── HeadlessRunner/
│
├── client/
│   └── UnityProject/
│       └── Assets/
│           ├── Game/
│           │   ├── Input/
│           │   ├── Scenes/
│           │   ├── UI/
│           │   ├── Presenters/
│           │   ├── ViewModels/
│           │   ├── Animation/
│           │   └── Audio/
│           └── Tests/
│
├── content/
│   ├── schemas/
│   ├── definitions/
│   ├── fixtures/
│   ├── narrative/
│   ├── localization/
│   └── compiled/
│
├── tools/
│   ├── ContentValidator/
│   ├── SeedExplorer/
│   ├── RuleInspector/
│   ├── ReplayDebugger/
│   └── SimulationBenchmark/
│
└── tests/
    ├── Unit/
    ├── Integration/
    ├── Properties/
    ├── Replay/
    ├── Content/
    ├── Scenarios/
    └── Performance/
```

Tên assembly và project cuối cùng có thể điều chỉnh theo conventions của team.

Điều không được thay đổi là hướng phụ thuộc.

<a id="m28-t077-s023"></a>

### 13. Dependency Direction

```text
Foundation
    ↑
Domain
    ↑
Application
    ↑
Infrastructure Adapters / Headless Host / Unity Adapter
```

Đây là dependency rule về code, không phải thứ tự thời gian xử lý.

Domain có thể định nghĩa interface mà Infrastructure triển khai.

Không được có:

`Domain → Unity UI`.

Không được có:

`Combat → Journal UI`.

Không được có:

`Content JSON → Arbitrary Code Execution`.

<a id="m28-t077-s024"></a>

### 14. Quy ước folder và naming

Mỗi domain nên có cấu trúc:

```text
Domain/Assets/
    Entities/
    ValueObjects/
    Commands/
    Rules/
    Events/
    Queries/
    Contracts/
    Validation/
```

Không bắt buộc tạo những thư mục rỗng.

Một feature chỉ nên được thêm folder khi thực sự có class tương ứng.

Quy ước:

**Command:** `RepairBoatCommand`.

**Handler:** `RepairBoatHandler`.

**Domain Event:** `BoatRepairedEvent`.

**Projection:** `BoatStatusView`.

**Rule:** `BoatRepairRule`.

**Validation:** `BoatRepairValidator`.

Một thao tác không nên có ba tên khác nhau ở các project.

<a id="m28-t077-s025"></a>

### 15. Không tạo một GameManager toàn năng

Đây là một anti-pattern cần cấm.

Không được có:

```text
GameManager
    GenerateNPC()
    ProcessCombat()
    UpdateEconomy()
    AdvanceQuest()
    SaveWorld()
    UpdateRelationships()
    GenerateStory()
    ...
```

Application Layer phải điều phối qua contracts.

Domain logic thuộc về domain.

Một `WorldSession` hoặc `SimulationHost` có thể giữ lifecycle và gọi Application Services, nhưng không nên chứa toàn bộ luật gameplay.

---

<a id="m28-t077-s026"></a>

## PHẦN V — HỢP ĐỒNG KỸ THUẬT NỀN TẢNG

<a id="m28-t077-s027"></a>

### 16. Command Pipeline

Vertical Slice cần một pipeline duy nhất.

```text
Player / NPC / Scheduler
          |
          v
     CommandEnvelope
          |
          v
   Authorization Check
          |
          v
    Hard Preconditions
          |
          v
   Deterministic Resolution
          |
          v
     Proposed Mutations
          |
          v
    Invariant Validation
          |
          v
      Atomic Commit
          |
          +----> Current State
          +----> Domain Events
          +----> Event Journal
          +----> Scheduled Follow-ups
          |
          v
      Read Projections
```

Mọi game action phải sử dụng pipeline này hoặc một workflow được định nghĩa rõ dựa trên nó.

<a id="m28-t077-s028"></a>

### 17. Command Envelope tham chiếu

```csharp
public sealed class CommandEnvelope<T>
{
    public string CommandId { get; init; }
    public string IssuerId { get; init; }
    public long ExpectedRevision { get; init; }
    public long WorldTick { get; init; }
    public T Payload { get; init; }
}
```

Đây là ví dụ interface-level, chưa phải class production hoàn chỉnh.

Cần xác định:

- null validation;
- ID type;
- serialization;
- authorization;
- idempotency;
- error handling.

<a id="m28-t077-s029"></a>

### 18. Command Result

```csharp
public enum CommandStatus
{
    Committed,
    Rejected,
    Scheduled,
    Conflict,
    Failed
}

public sealed class CommandResult
{
    public string CommandId { get; init; }
    public CommandStatus Status { get; init; }
    public long NewRevision { get; init; }
    public IReadOnlyList<string> EventIds { get; init; }
    public IReadOnlyList<string> ErrorCodes { get; init; }
}
```

Một command bị từ chối không được làm thay đổi authoritative state.

Một command được schedule không đồng nghĩa outcome cuối đã thành công.

Ví dụ:

`StartTravel` có thể thành công về mặt bắt đầu hành trình.

`ArriveAtDestination` chỉ xảy ra khi các điều kiện travel thực tế được xử lý.

<a id="m28-t077-s030"></a>

### 19. Invariant Validation

Các invariant P0 cần có ngay từ Foundation:

**Identity:** EntityID duy nhất.

**Life:** Người đã chết không thể tiếp tục action bình thường.

**Location:** Một physical entity không thể cùng lúc có hai vị trí authoritative.

**Ownership/Custody:** Một item không được nhân đôi.

**Resource:** Commodity transactions phải có source và sink hợp lệ.

**Time:** Scheduled event không commit hai lần.

**Knowledge:** Không tự tạo nguồn thông tin không tồn tại.

**History:** Committed event có cause và sequence.

Các domain có thể bổ sung invariant riêng.

Tất cả invariant phải được thực hiện tự động trong test pipeline.

<a id="m28-t077-s031"></a>

### 20. Event Journal

Mỗi committed event quan trọng có:

```json
{
  "event_id": "evt_000183",
  "event_type": "BoatRepaired",
  "world_tick": 10340,
  "commit_sequence": 183,
  "causation_id": "cmd_000092",
  "correlation_id": "activity_boat_repair_01",
  "rule_version": "boat_repair.v1",
  "payload": {
    "boat_id": "item_boat_01",
    "actor_id": "npc_vac_man",
    "repair_quality": "temporary"
  }
}
```

Ví dụ trên minh họa shape dữ liệu.

Để bảo đảm payload khớp với thay đổi thật, event phải được tạo cùng transaction với mutations tương ứng.

Không được để Event Journal ghi đã sửa thuyền trong khi BoatCondition chưa thay đổi do một lỗi commit.

---

<a id="m28-t077-s032"></a>

## PHẦN VI — THIẾT KẾ DỮ LIỆU THẾ GIỚI MẪU

<a id="m28-t077-s033"></a>

### 21. World Fixture

Đề xuất một world fixture chuẩn:

```text
Fixture ID: tamgiang_vertical_slice_v1
Seed: 824731
Ruleset: prototype_rules_v1
Content Pack: tamgiang_content_v1
Historical Era: Early Campaign Test Era
World Region: Tam Giang
```

**Seed 824731 chỉ là seed chuẩn để QA tái hiện tình huống**, không được thiết kế thành seed đặc biệt để kết quả nào cũng thuận lợi.

Bên cạnh seed chuẩn, cần chạy nhiều seed độc lập nhằm phát hiện lỗi phân phối.

<a id="m28-t077-s034"></a>

### 22. World State khởi tạo

World Fixture cần có:

- ba cộng đồng;
- một số household;
- nguồn thực phẩm;
- công cụ và phương tiện vận tải;
- route graph;
- water/environment state;
- quan hệ;
- những actor có công việc;
- một số Story Thread chưa active;
- ít nhất một environmental pressure sắp đạt điều kiện.

Không nên khởi tạo mọi NPC đang đứng chờ player với dấu chấm than trên đầu.

Các Event Engine và NPC Scheduler phải đủ khả năng tự tạo một số opportunity sau khi thời gian bắt đầu chạy.

<a id="m28-t077-s035"></a>

### 23. Settlement: Bến Vạc

Bến Vạc được chọn làm nơi chơi chính.

Những thành phần cần có:

**Bến thuyền:** Nơi phát sinh hoạt động vận tải.

**Khu sinh sống:** Household, quan hệ, một số NPC có lịch sinh hoạt.

**Nơi làm nghề:** Dụng cụ, vật liệu, công việc sửa chữa.

**Khu trao đổi hàng:** Giao dịch, vận tải, thông tin thị trường.

**Một nơi chăm sóc người bị thương:** Chữa trị và hồi phục.

**Một không gian cộng đồng:** Hội thoại, thương lượng, tranh chấp.

Không cần tạo quá nhiều nhà có thể vào nếu bên trong không có gameplay.

Mỗi địa điểm được chọn phải hỗ trợ ít nhất một hệ thống cần kiểm thử.

<a id="m28-t077-s036"></a>

### 24. Nhóm NPC mẫu

Đề xuất xây một nhóm NPC đủ đa dạng nhưng có quan hệ thật.

| NPC | Vai trò | Động cơ | Hệ thống kiểm thử |
|---|---|---|---|
| Vạc Mân | Người học nghề vận tải | Muốn chứng minh năng lực, giúp gia đình | Character, Skill, Profession |
| Tòng Mộc | Chủ thuyền | Cần duy trì thu nhập | Assets, Economy, Obligation |
| Miên | Người có kiến thức chữa trị | Quan tâm người bị thương và gia đình | Health, Companion, Social |
| Nậu Khôn | Người quản lý bến | Muốn bảo vệ quy tắc phân phối | Authority, Negotiation |
| Hạc Dư | Người đánh cá | Muốn giữ sinh kế | Knowledge, Rumor |
| Triều Sâm | Người sửa thuyền | Muốn duy trì uy tín nghề | Crafting, Mentor |
| Vạc Lâm | Người trẻ trong làng | Muốn rời khỏi công việc gia đình | Personal Story Thread |
| Nậu Thư | Người tham gia hội đồng | Ưu tiên ổn định cộng đồng | Faction, Governance |
| Một thương nhân | Vận chuyển hàng | Cần giao đúng hẹn | Trade, Route, Contract |
| Một người học nghề | Thực hành công việc | Muốn có mentor | Training, Long-term progression |

Những nhân vật còn lại có thể là:

- thành viên household;
- người đưa tin;
- người làm nghề;
- background residents;
- người có quan hệ với nhóm chính.

Các NPC này là fixture thiết kế, không tự động trở thành canon cuối cùng của game.

<a id="m28-t077-s037"></a>

### 25. Mỗi NPC phải có dữ liệu nào?

Một NPC được đưa vào playable slice phải có:

```text
Identity
Age / Life Stage
Household
Location
Profession
Relevant Skills
Knowledge
Goals
Needs
Relationships
Moral Boundaries
Current Commitments
Available Actions
```

Không chấp nhận NPC chỉ có:

```text
Name
Dialogue Text
Quest ID
```

Nếu một người đang chờ thuyền, phải biết:

- vì sao họ chờ;
- thuyền thuộc về ai;
- người đó có thể tự tìm cách giải quyết không;
- điều gì xảy ra nếu player không xuất hiện.

<a id="m28-t077-s038"></a>

### 26. Origin Fixtures

Để thử Origin-Adaptive Onboarding, cần ba profile kiểm thử.

<a id="m28-t077-s039"></a>

#### Origin A — Người học nghề vận tải

Tuổi trẻ, kinh nghiệm hạn chế, có mentor.

Kiểm thử:

- onboarding;
- luyện nghề;
- navigation;
- quan hệ mentor;
- progression.

<a id="m28-t077-s040"></a>

#### Origin B — Người trưởng thành có nghề

Đã có Skill đáng kể, nghĩa vụ gia đình, tài sản hoặc công việc.

Kiểm thử:

- không cần tutorial cho mọi thao tác cơ bản;
- routine work;
- resource responsibility;
- social obligation;
- career development.

<a id="m28-t077-s041"></a>

#### Origin C — Người lớn tuổi

Có tri thức và quan hệ, nhưng thể trạng hoặc thời gian sống có giới hạn.

Kiểm thử:

- onboarding phù hợp;
- truyền nghề;
- vai trò xã hội;
- aging;
- succession.

Trong QA có thể chọn trực tiếp các fixture để tái hiện bài kiểm thử.

**Trong gameplay chính thức, người chơi không được chọn hoặc reroll nhân vật tùy ý để tối ưu build.**

Hai cơ chế này cần tách bằng Debug/Test Mode.

---

<a id="m28-t077-s042"></a>

## PHẦN VII — THREE-PROFESSION VERTICAL SLICE

<a id="m28-t077-s043"></a>

### 27. Vì sao chọn ba nghề?

Một sai lầm thường gặp là đưa mười hai nghề vào Vertical Slice nhưng mỗi nghề chỉ có một nút `Work`.

Tôi đề xuất triển khai sâu ba hoạt động vì chúng tạo được các loại quyết định khác nhau.

**Vận tải** kiểm tra thời gian, môi trường, route, tài sản và quan hệ.

**Sửa chữa/chế tác** kiểm tra vật liệu, tool, Skill, chất lượng và bảo trì.

**Chữa trị** kiểm tra thương tích, tri thức, thời gian và hậu quả cơ thể.

Ba nhóm này cũng tạo được mối liên kết tự nhiên trong cùng một kịch bản.

<a id="m28-t077-s044"></a>

### 28. Activity: Vận tải đường sông

Một hoạt động vận tải cần các bước:

**Nhận yêu cầu:** Ai cần vận chuyển gì, tới đâu và vì sao.

**Kiểm tra quyền:** Player có thể sử dụng thuyền nào.

**Chuẩn bị:** Cargo, capacity, dụng cụ, food.

**Lập tuyến:** Thời tiết, dòng nước, checkpoint, travel time.

**Di chuyển:** WorldClock tiến; route/environment rules xử lý.

**Phát sinh trở ngại:** Chỉ khi đủ causal eligibility.

**Kết thúc:** Hàng tới nơi, hư hại, hao hụt, giao dịch hoặc thất bại.

Một chuyến đi không có biến cố vẫn là một kết quả hợp lệ.

Nếu đã trở thành routine, game có thể hỗ trợ rút gọn thao tác.

<a id="m28-t077-s045"></a>

### 29. Activity: Sửa chữa thuyền

Workflow:

1. Quan sát hư hại.
2. Đánh giá mức độ bằng Skill.
3. Chọn phương án sửa.
4. Kiểm tra quyền truy cập và công cụ.
5. Chọn vật liệu.
6. Dành thời gian.
7. Rule Engine tính kết quả.
8. Cập nhật BoatCondition, Material Stock và Practice Evidence.
9. Xử lý Agreement/Favor nếu có.

Đề xuất có ba phương án:

**Sửa tạm:** Ít vật liệu, nhanh, độ bền giới hạn.

**Sửa hoàn chỉnh:** Nhiều thời gian và vật liệu hơn.

**Thay cấu kiện:** Yêu cầu chuyên môn và công cụ tốt hơn.

Không cần một minigame phức tạp trong slice đầu.

Điều quan trọng là các phương án tạo ra sản phẩm vật lý khác nhau.

<a id="m28-t077-s046"></a>

### 30. Activity: Chữa trị

Một người bị thương sau tai nạn.

Player hoặc companion có thể:

- kiểm tra tình trạng;
- cầm máu;
- cố định vết thương;
- dùng vật tư phù hợp;
- đưa về nơi an toàn;
- nhờ người có chuyên môn;
- để người khác xử lý.

Health Domain quyết định trạng thái.

Không phải Dialogue Engine xác định nhân vật sống hay chết.

Một hành động điều trị đúng có thể tăng khả năng hồi phục nhưng không bảo đảm hồi phục tức thời.

Một vết thương nghiêm trọng phải tiếp tục ảnh hưởng:

- đi lại;
- lao động;
- combat;
- quan hệ;
- thời gian.

<a id="m28-t077-s047"></a>

### 31. Activity Integration Test

Kịch bản:

Một chuyến vận tải gặp tình huống thuyền hỏng.

Player sửa tạm để tiếp tục.

Một người bị thương trong quá trình xử lý.

Companion có kiến thức chữa trị tham gia.

Sau đó:

- vật liệu bị tiêu hao;
- thuyền có chất lượng sửa tạm;
- người bị thương có Wound;
- companion có Practice Evidence;
- chủ thuyền có thể ghi nhận việc giúp đỡ;
- shipment bị chậm;
- WorldClock đã tiến.

Tất cả phải xuất phát từ cùng committed event chain.

Đây là test thể hiện giá trị của Vertical Slice rõ hơn ba màn hình nghề nghiệp tách biệt.

---

<a id="m28-t077-s048"></a>

## PHẦN VIII — MAIN VERTICAL SLICE SCENARIO

<a id="m28-t077-s049"></a>

### 32. Scenario chính: “Bến qua mùa nước”

Đây là kịch bản trung tâm để tích hợp các hệ thống.

**Bối cảnh:** Bến Vạc đang bước vào giai đoạn nước cao. Tuyến vận tải tới Bến Hạc có vai trò quan trọng đối với nguồn cung của một số household.

Một chiếc thuyền thuộc household Tòng Mộc bị xuống cấp.

Một hợp đồng vận chuyển có thời hạn.

Trong khi đó, nguồn food tại cộng đồng đang chịu áp lực.

Player có thể biết một phần hoặc toàn bộ vấn đề tùy xuất thân, nghề nghiệp và quan hệ.

Không bắt buộc mọi player đều được NPC gọi tới ngay.

<a id="m28-t077-s050"></a>

### 33. Causal Chain ban đầu

```text
Seasonal Water Increase
          |
          v
River Risk Increases
          |
          +--> Travel Cost Changes
          |
          +--> Boat Condition Pressure
          |
          +--> Transport Capacity Reduced
                           |
                           v
                   Shipment Delay Risk
                           |
                           v
                    Food Supply Pressure
                           |
                 +---------+---------+
                 |                   |
                 v                   v
           Household Need      Trade Obligation
                 |                   |
                 +---------+---------+
                           |
                           v
                    Story Opportunities
```

Những thay đổi này được tính bằng rule.

Không phải một Story Script tự set tất cả biến vì player đến đúng địa điểm.

<a id="m28-t077-s051"></a>

### 34. Bốn hướng can thiệp chính

<a id="m28-t077-s052"></a>

#### Nhánh A — Player sửa thuyền

Player sử dụng Crafting/Repair.

Có thể:

- sửa tạm;
- sửa tốt;
- nhờ mentor;
- thiếu vật liệu;
- từ chối công việc.

**World Consequences:**

BoatCondition thay đổi.

Material Stock thay đổi.

Thời gian trôi.

Relationship có thể thay đổi.

Shipment có thể tiếp tục.

<a id="m28-t077-s053"></a>

#### Nhánh B — Player tổ chức phương án vận tải thay thế

Player thương lượng với household khác.

Có thể:

- thuê thuyền;
- đổi hàng;
- nhờ người quen;
- chuyển một phần hàng qua đường bộ.

**World Consequences:**

Asset Rights thay đổi trong thời hạn hợp đồng.

Cargo có route mới.

Transport Cost và Time thay đổi.

Một số household được hưởng lợi.

Household khác có thể mất cơ hội thu nhập.

<a id="m28-t077-s054"></a>

#### Nhánh C — Player điều tra nguyên nhân sự cố

Có người cho rằng thuyền bị phá.

Một người khác cho rằng do nước và bảo trì kém.

Player có thể:

- quan sát;
- hỏi người biết chuyện;
- dùng Skill;
- tìm vật chứng;
- đưa ra giả thuyết;
- bỏ qua.

**World Consequences:**

Knowledge thay đổi.

Rumor có thể lan.

Social Relations có thể bị ảnh hưởng nếu player công khai cáo buộc.

Không tự thay đổi Objective Cause của sự cố.

<a id="m28-t077-s055"></a>

#### Nhánh D — Player không can thiệp

Player có thể tập trung vào nghề nghiệp hoặc việc gia đình.

NPC tiếp tục:

- tìm người sửa;
- thương lượng;
- hoãn shipment;
- chấp nhận rủi ro;
- thay đổi kế hoạch.

**World Consequences:**

Có thể có outcome tốt hoặc xấu.

Không có quy tắc rằng nếu player không tham gia thì mọi người đều thất bại.

NPC Autonomy phải đủ để họ tự giải quyết vấn đề trong khả năng của mình.

<a id="m28-t077-s056"></a>

### 35. Environmental Crisis

Một điều kiện nước cao có thể tạo thêm tình huống nguy hiểm nếu trigger đạt.

Ví dụ:

Thuyền đang đi qua đoạn sông khó.

Environment Rule đánh giá:

- nước;
- tải trọng;
- BoatCondition;
- Skill;
- phương án di chuyển;
- thời tiết.

Nếu tai nạn thật sự xảy ra:

- Cargo có thể bị mất;
- BoatCondition giảm;
- người trên thuyền có thể bị thương;
- xuất hiện Rescue Opportunity;
- shipment bị ảnh hưởng.

Nếu điều kiện không đạt, không được ép tai nạn chỉ vì scenario cần một đoạn cao trào.

<a id="m28-t077-s057"></a>

### 36. Các outcome cần hỗ trợ

| Outcome | Điều kiện điển hình | Hậu quả |
|---|---|---|
| Vận tải thuận lợi | Sửa hoặc đổi tuyến hợp lệ | Hàng đến, hợp đồng được xử lý |
| Vận tải chậm | Thiếu thời gian/nguồn lực | Nghĩa vụ có thể bị ảnh hưởng |
| Sửa tạm | Vật liệu hoặc thời gian giới hạn | Thuyền hoạt động nhưng cần bảo trì |
| Chuyển sang tuyến khác | Có route và capacity | Chi phí và quan hệ thay đổi |
| Tai nạn | Environmental/Equipment Rules | Injury, Loss, Rescue Thread |
| Tranh chấp | Các bên diễn giải khác nhau | Social Thread, Rumor |
| NPC tự xử lý | Player không tham gia | World tiếp tục, player có thể biết sau |
| Vấn đề kéo dài | Không có phương án đủ điều kiện | Economy và household tiếp tục chịu áp lực |

Không xây một bản kết thúc duy nhất rồi biến mọi nhánh thành đường vòng dẫn tới đó.

---

<a id="m28-t077-s058"></a>

## PHẦN IX — STORY THREAD IMPLEMENTATION

<a id="m28-t077-s059"></a>

### 37. Ba Story Thread thử nghiệm

<a id="m28-t077-s060"></a>

#### Thread 1 — Con thuyền của nhà Tòng

**Origin:** Boat Condition + Contract Pressure.

**Actors:** Chủ thuyền, worker, trader.

**Player Entry:** Quan sát, được đề nghị, tự kiểm tra hoặc biết qua người khác.

**Outcomes:** Repair, alternative transport, delay, loss, abandonment.

<a id="m28-t077-s061"></a>

#### Thread 2 — Người rời Bến Vạc

**Origin:** Một Personal Life Event đã được xác lập của Vạc Lâm.

**Actors:** Gia đình, người quen, người từng nhìn thấy Lâm.

**Player Entry:** Household relation, rumor, gặp nhân chứng.

**Outcomes:** Liên lạc lại, tìm được tung tích, hiểu nhầm, mất dấu hoặc thread tiếp tục ngoài tầm player.

<a id="m28-t077-s062"></a>

#### Thread 3 — Quyền sử dụng bến

**Origin:** Resource/Route pressure và conflicting interests.

**Actors:** Người quản lý bến, household vận tải, người đánh cá, trader.

**Player Entry:** Contract, Negotiation hoặc chứng kiến tranh chấp.

**Outcomes:** Thỏa thuận, chia ca, đổi quyền sử dụng, tranh chấp kéo dài hoặc faction intervention.

Ba thread sử dụng chung World State nhưng không cần luôn diễn ra đồng thời.

<a id="m28-t077-s063"></a>

### 38. Thread State không thay vật lý

Ví dụ:

Một lời hứa được hoàn thành.

Social Domain commit `ObligationFulfilled`.

Narrative Domain cập nhật Thread.

Journal Projector cập nhật player-facing status.

Không được có:

```text
QuestComplete = true
-> Automatically Give Boat
```

Nếu reward là một vật phẩm hoặc quyền sử dụng thuyền, reward phải thực hiện qua Asset/Trade Rules hợp lệ.

<a id="m28-t077-s064"></a>

### 39. Event Template tham chiếu

Ví dụ nội dung authoring:

```json
{
  "template_id": "tamgiang.boat_repair_request.v1",
  "category": "profession_opportunity",
  "trigger": {
    "type": "boat_condition_below_threshold"
  },
  "participants": {
    "requester": "boat_custodian",
    "worker": "eligible_repair_actor"
  },
  "constraints": [
    "participants_alive",
    "same_reachable_area",
    "boat_exists",
    "requester_has_authority"
  ],
  "supported_commands": [
    "inspect_boat",
    "negotiate_repair",
    "repair_boat",
    "decline_request"
  ],
  "presentation_id": "dialogue.boat_repair_request.v1"
}
```

Các giá trị trên tham chiếu tới types/handlers đã đăng ký.

Không cho phép Content Pack tự định nghĩa một script chạy code ngoài phạm vi.

<a id="m28-t077-s065"></a>

### 40. Validation của Event Template

Template phải bị loại khi:

- referenced handler không tồn tại;
- participant role không có logic binding;
- actor không đủ quyền;
- route/location không hợp lệ;
- output effect không có domain owner;
- outcome tạo item không có source;
- event không xử lý trường hợp player không can thiệp dù actor độc lập cần tiếp tục;
- có nguy cơ tự sinh vô hạn.

Content Validator phải xuất ra error code có thể hiểu được.

Ví dụ:

```text
CONTENT-E014:
Template tamgiang.boat_repair_request.v1
references unknown handler:
boat_repair_without_material
```

Không chỉ in:

`Invalid content`.

---

<a id="m28-t077-s066"></a>

## PHẦN X — NPC AUTONOMY IMPLEMENTATION

<a id="m28-t077-s067"></a>

### 41. NPC Decision phải được triển khai trước hệ Quest phong phú

Nếu Quest được viết trước, dễ xuất hiện NPC chỉ hành động khi player kích hoạt quest.

Điều đó trái với triết lý Living World.

Vì vậy thứ tự triển khai phải là:

**NPC Needs/Goals → Available Actions → Rule Validation → NPC Commands → Domain Events → Story Thread.**

Quest và UI là bước sau.

<a id="m28-t077-s068"></a>

### 42. NPC AI tối thiểu nhưng có ý nghĩa

Mỗi NPC có:

- primary goal;
- một số needs;
- profession;
- available resources;
- current commitment;
- social relationships;
- knowledge;
- moral boundary.

Utility AI chỉ xét các action thuộc loại actor có thể thực hiện.

Ví dụ chủ thuyền thiếu vật liệu:

**Action A:** Tự sửa.

**Action B:** Tìm thợ.

**Action C:** Thuê thuyền khác.

**Action D:** Hoãn giao hàng.

Không cho NPC bốc ngẫu nhiên một action không liên quan từ toàn bộ game.

<a id="m28-t077-s069"></a>

### 43. Replanning

NPC không cần tính lại mục tiêu mỗi frame.

Replanning xảy ra khi:

- công việc hoàn thành;
- điều kiện vật chất thay đổi;
- nhận tin mới;
- có sự kiện khẩn cấp;
- commitment đến hạn;
- bị ngăn thực hiện action.

Một NPC đang sửa thuyền không được liên tục bỏ việc mỗi tick vì action khác vừa có Utility cao hơn một điểm.

Cần Commitment Stability.

<a id="m28-t077-s070"></a>

### 44. Test: Player hoàn toàn không can thiệp

Headless Runner chạy cùng world trong một khoảng thời gian thử nghiệm.

Kỳ vọng:

- chủ thuyền có thể tự tìm phương án;
- trader có thể tìm route khác;
- food supply thay đổi theo logistics;
- agreement có thể được tạo giữa NPC;
- Story Thread có thể resolved, escalated hoặc transformed;
- không có NPC nào đứng chờ player mãi nếu world đã có điều kiện xử lý.

Đây là một trong những bài kiểm thử quan trọng nhất toàn bộ Vertical Slice.

---

<a id="m28-t077-s071"></a>

## PHẦN XI — COMBAT VERTICAL SLICE

<a id="m28-t077-s072"></a>

### 45. Phạm vi Combat

Không xây mọi loại vũ khí.

Đề xuất:

- giáo;
- một vũ khí ngắn;
- khiên;
- một công cụ phòng thủ tạm;
- armor/garment cơ bản.

Combat Space:

**3–5 Tactical Zones**, một số Local Position Slots khi cần.

Action Budget:

- 1 Major;
- 1 Minor;
- Reaction có giới hạn.

<a id="m28-t077-s073"></a>

### 46. Encounter chọn để kiểm thử

Một tình huống bảo vệ hoặc cứu người gần bến.

Objective:

> Đưa hai người bị thương ra khỏi khu vực nguy hiểm hoặc giữ lối thoát tới khi họ được cứu.

Có thể có actor đối kháng hợp lệ từ một tranh chấp hoặc hành động đã đủ điều kiện.

Không cần tự tạo một nhóm cướp chỉ để mở tutorial combat.

Encounter phải hỗ trợ:

- chiến đấu;
- thương lượng hoặc dừng xung đột khi hợp lệ;
- rút lui;
- hoàn thành mục tiêu mà không giết hết đối thủ.

<a id="m28-t077-s074"></a>

### 47. Combat Rule Chain

```text
Choose Action
    |
    v
Validate Actor/Position
    |
    v
Validate Action Budget
    |
    v
Resolve Attack/Defense
    |
    v
Resolve Impact
    |
    v
Health Domain Evaluates Wound
    |
    v
Commit Result
    |
    v
Re-evaluate Tactical Objectives
```

Không để `CombatView` tự trừ HP.

Không để animation hit frame tự quyết định enemy đã chết.

Animation chỉ thể hiện kết quả authoritative.

<a id="m28-t077-s075"></a>

### 48. Combat Acceptance Tests

Một combat prototype đạt yêu cầu khi:

- Same Commands + Same Seed tạo cùng outcome.
- Position thay đổi hiệu quả hành động thật.
- Armor có vai trò trong tác động vật lý.
- Injury tiếp tục tồn tại sau encounter.
- Companion có thể từ chối action trái moral boundary.
- Retreat là một kết quả hợp lệ.
- Objective không phụ thuộc việc giết sạch đối thủ.
- Actor chết hoặc incapacitated không tiếp tục hành động.
- Combat không tự sửa Quest World State ngoài domain events.

---

<a id="m28-t077-s076"></a>

## PHẦN XII — KNOWLEDGE & INFORMATION INTEGRITY

<a id="m28-t077-s077"></a>

### 49. Một tình huống có hai lớp sự thật

Ví dụ thuyền hỏng vì bảo trì kém.

**World Truth:**

`BoatDamageCause = MaintenanceFailure`.

**NPC Belief:**

Một nhân chứng cho rằng thuyền bị phá.

**Player Knowledge:**

Player mới nghe lời kể.

Journal phải ghi:

> “Hạc Dư nghi có người phá thuyền.”

Không được ghi:

> “Có người phá thuyền.”

<a id="m28-t077-s078"></a>

### 50. Knowledge Query API

Đề xuất:

```text
GetKnownPeople(observerId)
GetKnownLocations(observerId)
GetKnownClaims(observerId, subjectId)
GetPerceivedRoute(observerId, routeId)
GetKnownThreadState(observerId, threadId)
```

Các API này trả về player/actor-scoped projections.

Không được cho NPC Planner gọi trực tiếp hidden `GetActualWorldFacts()` để quyết định ý định.

World Truth vẫn được domain rules sử dụng để xử lý hậu quả vật lý.

<a id="m28-t077-s079"></a>

### 51. Rumor Propagation Test

Một tin được truyền:

Hạc Dư → Tòng Mộc → Nậu Khôn.

Kiểm tra:

- từng transmission có time/source;
- actor chưa nghe không biết;
- nội dung có thể bị biến đổi theo rule;
- không tự tạo World Truth mới;
- tin được lặp từ một nguồn không tự trở thành ba chứng cứ độc lập.

Nếu UI ở Bãi Lau xuất hiện thông tin ngay khi tin vừa phát tại Bến Vạc mà không có communication channel, đó là P0 defect.

<a id="m28-t077-s080"></a>

### 52. Search Leak Test

Journal/Codex/Search không được tiết lộ:

- NPC bí mật;
- hidden location;
- future event;
- số lượng bí mật chưa biết;
- người phản bội;
- giải pháp quest chưa có nguồn.

Hệ thống tìm kiếm phải chạy trên Knowledge Projection đã được lọc.

Không được search toàn bộ database rồi chỉ ẩn phần text kết quả.

---

<a id="m28-t077-s081"></a>

## PHẦN XIII — TIME SKIP & LONG-TERM CONSEQUENCES

<a id="m28-t077-s082"></a>

### 53. Time Skip trong Vertical Slice

Không cần cho player chơi từng ngày của hai năm.

Nhưng phải có khả năng chọn:

> Tiếp tục làm nghề và sinh sống tại Bến Vạc trong một khoảng thời gian.

Sau đó engine xử lý:

- profession;
- household;
- economy;
- relationship;
- health;
- scheduled commitments;
- environmental changes;
- actor decisions;
- Story Threads.

Không được chỉ tăng tuổi và Skill bằng một công thức thưởng cố định.

<a id="m28-t077-s083"></a>

### 54. Time Skip Processing

Pseudo-flow:

```text
AdvanceTo(targetTick):
    while currentTick < targetTick:
        nextBoundary = Scheduler.NextRelevantBoundary()

        effectiveTarget =
            min(targetTick, nextBoundary)

        ProcessAggregateActivitiesUntil(effectiveTarget)
        AdvanceClock(effectiveTarget)

        ProcessDueEventsInStableOrder()
        ValidateWorldInvariants()

        if PlayerDecisionRequiredAndKnown():
            PauseTimeSkip()
            return

    return TimeSkipCompleted
```

Một số hệ có thể dùng bước thời gian tổng hợp.

Nhưng không được bỏ qua event đến hạn có thể làm thay đổi điều kiện của bước tiếp theo.

<a id="m28-t077-s084"></a>

### 55. Báo cáo Time Skip

Sau Time Skip, hệ thống có thể hiển thị:

**Nhân vật:** Kỹ năng và sức khỏe thay đổi.

**Gia đình:** Thành viên, công việc, biến cố đã biết.

**Nghề nghiệp:** Những gì đã học hoặc hoàn thành.

**Cộng đồng:** Những biến đổi player có nguồn nhận biết.

**Lời hứa:** Hoàn thành, vi phạm hoặc đang chờ.

**Những câu chuyện còn dang dở:** Thread updates.

Không được tự tiết lộ việc một người ở xa đã chết nếu thông tin chưa tới character.

<a id="m28-t077-s085"></a>

### 56. Long-Term Infrastructure Test

Một cây cầu hoặc bến đò được sửa.

Sau một năm, cần xác định:

- hiện còn sử dụng không;
- condition ra sao;
- ai duy trì;
- đã có event nào tác động;
- route có thay đổi;
- người dân có còn ghi nhớ đóng góp của player không.

Nếu công trình đã hỏng tiếp, cần truy được nguyên nhân.

Không được reset condition về mặc định chỉ vì Local Area được load lại.

---

<a id="m28-t077-s086"></a>

## PHẦN XIV — DEATH, SUCCESSION & LEGACY PROTOTYPE

<a id="m28-t077-s087"></a>

### 57. Không buộc một cái chết giả vào câu chuyện chính

Vertical Slice cần test Succession.

Nhưng không nên ép NPC hoặc protagonist chết ở phút cuối demo chỉ để cho thấy tính năng.

Đề xuất:

**Playable Scenario:** Có thể tiếp tục sống bình thường; tử vong chỉ xảy ra nếu rules và hành động thực sự dẫn tới.

**QA Succession Fixture:** Có một trạng thái dựng sẵn từ lịch sử nhân quả hợp lệ để kích hoạt Death/Succession test.

**Headless Long-Term Test:** Kiểm tra aging/mortality/succession qua mô phỏng thời gian.

Ba bài kiểm thử sử dụng cùng logic engine.

<a id="m28-t077-s088"></a>

### 58. Succession Candidate

Một character phải:

- tồn tại thực sự;
- còn sống;
- có Life History;
- có liên hệ với predecessor/campaign;
- có khả năng chơi;
- có Knowledge riêng.

Ví dụ Miên có thể là successor nếu hệ thống đã tạo được quan hệ và điều kiện phù hợp.

Không được tự tạo Miên từ không khí chỉ vì protagonist chết.

<a id="m28-t077-s089"></a>

### 59. Asset & Knowledge Transfer

Đối với predecessor:

- personal items xử lý theo quyền sở hữu;
- borrowed items trả về chủ hoặc tiếp tục tranh chấp;
- household assets giữ theo household;
- secrets chưa truyền không tự chuyển;
- Techniques chỉ truyền khi đã có teaching event.

Khi successor bắt đầu:

- nhận Character State của chính mình;
- không nhận copy Character State cũ;
- có những Story Thread liên quan theo Knowledge và nghĩa vụ riêng.

<a id="m28-t077-s090"></a>

### 60. Legacy Demo

Chọn một di sản cụ thể:

**Một kỹ thuật sửa thuyền đã được truyền lại.**

Quy trình:

1. Mân học Technique.
2. Mân thực hành.
3. Mân có cơ hội truyền cho apprentice.
4. Teaching Event diễn ra.
5. Apprentice phát triển năng lực theo rules.
6. Mân qua đời hoặc rời nghề.
7. Apprentice vẫn giữ kiến thức đã học.
8. Nếu họ tiếp tục sử dụng, technique legacy tồn tại.

Không cần dựng toàn bộ Myth System trong slice.

Một chuỗi truyền nghề đầy đủ đã đủ kiểm tra:

- Skill;
- Knowledge;
- Social;
- Teaching;
- Time;
- Legacy;
- Succession.

---

<a id="m28-t077-s091"></a>

## PHẦN XV — HISTORICAL ANCHOR PROOF

<a id="m28-t077-s092"></a>

### 61. Vì sao Vertical Slice vẫn cần Historical Contract?

Campaign cuối cùng có sáu hồi với các mốc lịch sử quan trọng.

Nếu engine chỉ được kiểm tra ở settlement nhỏ nhưng không có cách kết nối với Historical Anchor, về sau có thể phải viết lại kiến trúc.

Do đó cần một **Headless Historical Integration Test** riêng.

Không cần dựng Cổ Loa hoàn chỉnh trong demo.

<a id="m28-t077-s093"></a>

### 62. Historical Fixture

Đề xuất một **abstract historical pressure**:

Một trung tâm chính trị bên ngoài Tam Giang tăng nhu cầu vận tải và vật liệu.

World State có:

- một external demand source;
- supply obligations;
- route dependencies;
- resource constraints;
- expected institutional action.

Player có thể ảnh hưởng một tuyến supply địa phương.

Engine phải:

- cập nhật logistics;
- đánh giá available scenarios;
- không tự sinh cargo;
- không cần LLM;
- báo thiếu scenario nếu không còn đường hợp lệ.

<a id="m28-t077-s094"></a>

### 63. Hard Anchor Test

Một Test Anchor có:

**Protected Invariant:** Một sự kiện macro được định nghĩa là phải xảy ra trong phạm vi kịch bản thử nghiệm.

**Flexible Conditions:** Nguồn cung, thời điểm, mức chi phí.

**Scenario Library:** Ít nhất hai cách hợp lệ để hoàn thành.

Test:

- phá một route;
- giảm vật liệu;
- cho một NPC quan trọng rời hệ thống;
- tăng nhu cầu.

Engine phải chọn một kịch bản hợp lệ đã khai báo hoặc phát hiện trạng thái không thể giải.

Không được gian lận.

Đây là kiểm thử kiến trúc, chưa phải nội dung lịch sử được đưa lên màn chơi.

---

<a id="m28-t077-s095"></a>

## PHẦN XVI — MINIMUM PLAYER-FACING UI

<a id="m28-t077-s096"></a>

### 64. Những màn hình thực sự cần thiết

Đề xuất xây tám màn hình/nhóm giao diện.

| Giao diện | Vai trò | Bắt buộc chứng minh |
|---|---|---|
| Main HUD | Trạng thái và tương tác | Không overload |
| Character | Attribute, Skill, Health | Derived values rõ ràng |
| Party | Companion, role, condition | Autonomy và injury |
| World Map | Place/Route knowledge | Không lộ hidden state |
| Journal | Claims, threads, promises | Nguồn thông tin |
| Dialogue | Choice, intent, agreement | Actor autonomy |
| Inventory/Assets | Items, cargo, ownership | Không nhân đôi |
| Combat View | Zone, action, objective | Chiến thuật và thương tích |

Không cần UI đẹp như final product ngay từ prototype kỹ thuật.

Nhưng trong Vertical Slice cuối, các màn hình phải đủ dễ hiểu để người ngoài team tự chơi.

<a id="m28-t077-s097"></a>

### 65. HUD theo ngữ cảnh

Ở settlement:

- vị trí;
- ngày/mùa;
- health quan trọng;
- interaction;
- current concern tùy chọn.

Khi travel:

- route;
- progress;
- supply;
- party;
- tình trạng đã biết của tuyến.

Khi combat:

- Tactical Zones;
- vị trí;
- action budget;
- stamina;
- objective;
- condition.

Không tạo một HUD luôn hiển thị tất cả 20 hệ thống.

<a id="m28-t077-s098"></a>

### 66. Journal và Fact Provenance

Ví dụ:

```text
CHUYỆN CHIẾC THUYỀN

Đã chứng kiến:
- Mạn thuyền có vết nứt.

Được kể:
- Hạc Dư nghi có người phá thuyền.

Suy luận:
- Vết nứt có thể liên quan tới bảo trì.

Việc đang quan tâm:
- Tìm cách giúp nhà Tòng vận chuyển hàng.

Lần cập nhật:
- Ngày 18, mùa nước cao.
```

Thông tin phải đến từ Knowledge Records.

Không được hardcode cùng một đoạn Journal cho tất cả player.

<a id="m28-t077-s099"></a>

### 67. UI Không được tự quyết định cơ chế

Ví dụ nút `Repair`.

UI chỉ:

- xây command;
- hiển thị thông tin dự báo hợp lệ;
- gửi command;
- chờ result;
- cập nhật projection.

UI không tự:

- trừ vật liệu;
- tăng Skill;
- sửa thuyền;
- hoàn thành Quest;
- tăng Trust.

Nếu chạy headless cùng command, kết quả phải giống nhau.

---

<a id="m28-t077-s100"></a>

## PHẦN XVII — MILESTONE IMPLEMENTATION ROADMAP

<a id="m28-t077-s101"></a>

### 68. Tổng quan milestone

Đề xuất chia thành tám milestone.

| Milestone | Nội dung | Phụ thuộc chính | Exit Gate |
|---|---|---|---|
| M0 | Repository & Architecture Contract | Không | Build/test được |
| M1 | Deterministic Kernel | M0 | Replay được |
| M2 | Living Settlement | M1 | NPC tự vận hành |
| M3 | Knowledge & Narrative | M2 | Thread phát sinh từ world |
| M4 | Physical Gameplay | M2–M3 | Travel/Craft/Combat liên thông |
| M5 | Time & Legacy | M2–M4 | Hậu quả qua nhiều năm |
| M6 | Unity Playable Slice | M3–M5 | Có thể chơi end-to-end |
| M7 | Validation & Hardening | M0–M6 | Pass Go/No-Go |

Có thể thực hiện một số công việc song song sau khi contracts đã ổn định.

Không nên phân công mọi module cho các coding agent khác nhau ngay từ ngày đầu.

<a id="m28-t077-s102"></a>

### 69. Milestone 0 — Repository & Architecture Contract

**Mục tiêu:** Tạo nền cho nhiều người hoặc coding agent làm việc mà không giẫm lên nhau.

Công việc:

**M0-01 — Repository Bootstrap:** Tạo solution, project, thư mục, build configuration.

**M0-02 — Architecture Rules:** Định nghĩa dependencies, domain ownership, naming, versioning.

**M0-03 — Shared Contracts:** Entity ID, WorldTick, Result, Error, Command Envelope.

**M0-04 — Test Infrastructure:** Unit test và headless test runner.

**M0-05 — Content Schema Baseline:** Quy ước Definition ID, Event Template và references.

**M0-06 — CI:** Build, static checks, tests, content validation.

<a id="m28-t077-s103"></a>

#### Deliverables

- Source repository build thành công.
- Headless Runner chạy được.
- Shared core được reference bởi Unity stub.
- Architecture/Contract documentation.
- Một số unit tests nền.

<a id="m28-t077-s104"></a>

#### Exit Gate

Không có compile error.

Không có domain reference trực tiếp tới Unity.

Các interfaces nền được thống nhất.

Có thể chạy test từ command line.

Không bắt đầu M2 khi M0 chưa đủ rõ.

<a id="m28-t077-s105"></a>

### 70. Milestone 1 — Deterministic Kernel

**Mục tiêu:** Có một core thực thi command và lưu được World State.

Công việc:

**M1-01 — Entity Registry:** Stable IDs, lifecycle và references.

**M1-02 — WorldClock:** Absolute ticks, scheduling primitives.

**M1-03 — RNG Service:** Versioned deterministic PRNG, named streams/contexts.

**M1-04 — Rule Registry:** Mapping rule ID tới handler.

**M1-05 — Command Processor:** Validation, resolution, commit.

**M1-06 — Event Journal:** Committed event schema và causation references.

**M1-07 — Persistence:** Save/load current state, scheduler, RNG.

**M1-08 — Replay Runner:** Replay từ fixture snapshot và recorded commands.

**M1-09 — Invariant Framework:** Kiểm tra identity, resources và revision.

<a id="m28-t077-s106"></a>

#### Demo bắt buộc

Có một Item.

Character A sở hữu và đang giữ Item.

Gửi command chuyển item cho Character B.

Engine:

- kiểm tra quyền;
- commit custody/ownership theo điều kiện;
- phát event;
- save;
- load;
- replay;
- so sánh state hash.

<a id="m28-t077-s107"></a>

#### Exit Gate

Cùng seed và commands cho cùng authoritative state trong môi trường thử nghiệm đã định nghĩa.

Retry command không chuyển item hai lần.

Transaction thất bại không để lại state dở dang.

Đây là **P0 Architecture Gate**.

<a id="m28-t077-s108"></a>

### 71. Milestone 2 — Living Settlement

**Mục tiêu:** Một settlement có thể tự hoạt động mà không cần player.

Công việc:

**M2-01 — Settlement/Household Entities.**

**M2-02 — Character Identity & Life State.**

**M2-03 — Character Generator với Life History.**

**M2-04 — Profession & Basic Work.**

**M2-05 — Resource/Commodity Ledger.**

**M2-06 — Needs & Goals.**

**M2-07 — NPC Utility Planner.**

**M2-08 — Actor Scheduler.**

**M2-09 — Basic Environmental Pressure.**

**M2-10 — Headless Settlement Simulator.**

<a id="m28-t077-s109"></a>

#### Demo bắt buộc

Bến Vạc được khởi tạo.

Các household có:

- người;
- nghề;
- tài nguyên;
- nhu cầu;
- quan hệ cơ bản.

Engine chạy một khoảng thời gian.

NPC:

- làm việc;
- tiêu thụ resource;
- nghỉ ngơi;
- chuyển việc nếu đủ điều kiện;
- xử lý nhu cầu mới;
- duy trì commitments.

<a id="m28-t077-s110"></a>

#### Exit Gate

Không có NPC vừa chết vừa làm việc.

Không có resource âm trái quy tắc.

Một người không đồng thời làm việc ở hai địa điểm bất khả thi.

Không cần player để world tiến triển.

Có event trace giải thích những biến đổi quan trọng.

<a id="m28-t077-s111"></a>

### 72. Milestone 3 — Knowledge & Narrative

**Mục tiêu:** World Event trở thành một vấn đề có thể được player biết và can thiệp.

Công việc:

**M3-01 — Observation & Claim Records.**

**M3-02 — Actor Knowledge Store.**

**M3-03 — Knowledge Propagation.**

**M3-04 — Event Template Eligibility.**

**M3-05 — Story Thread State Machine.**

**M3-06 — Quest/Journal Projection.**

**M3-07 — Dialogue Acts.**

**M3-08 — Social Relationship & Obligation.**

**M3-09 — World Director Exposure Rules.**

<a id="m28-t077-s112"></a>

#### Demo bắt buộc

BoatCondition giảm.

NPC nhận biết tình trạng thuyền.

NPC quyết định tìm người giúp.

Một người đưa tin truyền thông tin.

Player có thể biết qua hội thoại hoặc quan sát.

Journal ghi đúng nguồn.

Player có thể nhận hoặc bỏ qua request.

<a id="m28-t077-s113"></a>

#### Exit Gate

Nếu player chưa nhận tin, Journal không hiển thị event như sự thật đã biết.

NPC không biết hidden information.

Nếu player bỏ qua, NPC vẫn tiếp tục kế hoạch.

Quest Projection không được ghi trực tiếp World State.

<a id="m28-t077-s114"></a>

### 73. Milestone 4 — Physical Gameplay

**Mục tiêu:** Player hành động vật lý với hậu quả tồn tại trong simulation.

Công việc:

**M4-01 — World Route Graph.**

**M4-02 — Travel Planner & Route Resolution.**

**M4-03 — Boat/Item Condition.**

**M4-04 — Repair/Crafting Rules.**

**M4-05 — Health/Wound State.**

**M4-06 — Treatment Rules.**

**M4-07 — Tactical Zone & Slot Model.**

**M4-08 — Combat Action Resolution.**

**M4-09 — Non-Lethal Objective & Retreat.**

**M4-10 — Multi-Domain Activity Scenario.**

<a id="m28-t077-s115"></a>

#### Demo bắt buộc

Player:

- nhận công việc;
- lập tuyến;
- sử dụng thuyền;
- sửa hư hại;
- tiêu hao vật liệu;
- gặp trở ngại nếu causal trigger đạt;
- điều trị người bị thương nếu có;
- giao hàng hoặc thất bại.

<a id="m28-t077-s116"></a>

#### Exit Gate

Inventory, Health, Travel, Economy và Narrative phản ánh cùng một sự kiện.

Một Wound tồn tại sau combat.

Một cargo không xuất hiện đồng thời ở hai nơi.

Một route không được sử dụng nếu thực sự không thể đi.

<a id="m28-t077-s117"></a>

### 74. Milestone 5 — Time & Legacy

**Mục tiêu:** Một cuộc đời và các hậu quả có thể tiếp tục qua nhiều năm.

Công việc:

**M5-01 — Long-Interval Simulation.**

**M5-02 — Practice & Skill Growth.**

**M5-03 — Career/Profession Progression.**

**M5-04 — Aging.**

**M5-05 — Mortality/Death Transaction.**

**M5-06 — Succession Eligibility.**

**M5-07 — Ownership/Knowledge Transfer.**

**M5-08 — Chronicle.**

**M5-09 — Legacy Persistence.**

**M5-10 — Historical Anchor Contract Test.**

<a id="m28-t077-s118"></a>

#### Demo bắt buộc

Một character:

- làm nghề;
- học kỹ thuật;
- truyền lại kỹ thuật;
- trải qua Time Skip;
- rời nghề hoặc qua đời theo fixture hợp lệ.

Người kế tục:

- có identity riêng;
- chỉ biết những điều đã được truyền hợp lệ;
- sở hữu tài sản theo Asset Rules;
- tiếp tục tồn tại trong cùng World Timeline.

<a id="m28-t077-s119"></a>

#### Exit Gate

Không copy Skill/Knowledge từ predecessor.

Một sự kiện cũ còn được truy xuất sau nhiều năm.

World State không reset.

Time Skip không bỏ qua các event quan trọng.

Historical Test không sử dụng tài nguyên hoặc actor sinh miễn phí.

<a id="m28-t077-s120"></a>

### 75. Milestone 6 — Playable Unity Vertical Slice

**Mục tiêu:** Chuyển core đã hoạt động thành trải nghiệm có thể chơi.

Công việc:

**M6-01 — Scene & Input Framework.**

**M6-02 — Character View.**

**M6-03 — Settlement Exploration.**

**M6-04 — Dialogue UI.**

**M6-05 — Map & Travel UI.**

**M6-06 — Repair/Treatment Interaction.**

**M6-07 — Journal & Knowledge Display.**

**M6-08 — Combat View.**

**M6-09 — Save/Load UX.**

**M6-10 — Time Skip Report.**

**M6-11 — Audio/Visual Feedback.**

**M6-12 — Origin-Adaptive Onboarding.**

<a id="m28-t077-s121"></a>

#### Demo bắt buộc

Một tester không thuộc team:

1. Khởi tạo game.
2. Hiểu xuất thân nhân vật.
3. Đi lại trong Bến Vạc.
4. Gặp một NPC.
5. Nhận biết vấn đề.
6. Tự chọn cách can thiệp.
7. Thực hiện activity.
8. Nhận hậu quả.
9. Mở Journal để hiểu điều đã xảy ra.
10. Tiếp tục hành trình hoặc Time Skip trong phạm vi demo.

<a id="m28-t077-s122"></a>

#### Exit Gate

Tester không cần dùng debug console để hoàn thành trải nghiệm chính.

Gameplay không phụ thuộc một thứ tự interaction duy nhất.

Có thể quay lại save.

Core không cần biết Unity scene hiện tại để giải quyết gameplay.

<a id="m28-t077-s123"></a>

### 76. Milestone 7 — Validation & Hardening

**Mục tiêu:** Đánh giá khả năng mở rộng từ slice sang full game.

Công việc:

**M7-01 — Regression Test Pass.**

**M7-02 — Multi-Seed Headless Simulation.**

**M7-03 — Long-Term State Integrity.**

**M7-04 — Performance Profiling.**

**M7-05 — Content Coverage Review.**

**M7-06 — Accessibility & Usability Test.**

**M7-07 — Save/Load Crash Recovery.**

**M7-08 — Historical Constraint Stress Test.**

**M7-09 — Technical Debt Review.**

**M7-10 — Go/No-Go Decision.**

<a id="m28-t077-s124"></a>

#### Exit Gate

Không còn P0 blocker.

Các P1 liên quan khả năng mở rộng có hướng xử lý đã được kiểm chứng.

Có benchmark và playtest report.

Team có thể giải thích giới hạn của slice và những công việc phải làm trước full production.

---

<a id="m28-t077-s125"></a>

## PHẦN XVIII — WORK BREAKDOWN CHO CODING AGENTS

<a id="m28-t077-s126"></a>

### 77. Vì sao không giao toàn bộ GDD cho một agent rồi yêu cầu “build game”?

GDD có quá nhiều domain phụ thuộc lẫn nhau.

Nếu một coding agent được giao:

> Hãy xây toàn bộ World Engine với NPC, Event, Quest, Combat, Economy và UI.

Nó có thể tạo ra nhiều class và màn hình trông hợp lý nhưng:

- domain ownership không rõ;
- code trùng logic;
- state được ghi trực tiếp;
- random không deterministic;
- save/load không đủ;
- quest tự spawn item;
- NPC chỉ có dialogue giả;
- UI chứa game rules.

Đây là nguy cơ đặc biệt cao khi yêu cầu sinh code một lần với phạm vi quá lớn.

Giải pháp là **contract-first implementation**, chia task theo dependency.

<a id="m28-t077-s127"></a>

### 78. Cách phân chia agent

Đề xuất các vai trò triển khai.

<a id="m28-t077-s128"></a>

#### Agent A — Foundation Engineer

Sở hữu:

- identity;
- clock;
- RNG;
- commands;
- events;
- commit pipeline;
- validation.

Không viết Quest hoặc UI.

<a id="m28-t077-s129"></a>

#### Agent B — World Simulation Engineer

Sở hữu:

- settlement;
- household;
- resource;
- environment;
- work;
- NPC scheduling.

Phụ thuộc contracts của Agent A.

<a id="m28-t077-s130"></a>

#### Agent C — Character/Social Engineer

Sở hữu:

- character;
- life history;
- health;
- social;
- knowledge;
- obligations.

Không tự viết persistence framework khác.

<a id="m28-t077-s131"></a>

#### Agent D — Gameplay Engineer

Sở hữu:

- travel;
- repair;
- treatment;
- combat;
- tactical positions.

Sử dụng authoritative Rule Engine.

<a id="m28-t077-s132"></a>

#### Agent E — Narrative & Content Engineer

Sở hữu:

- event templates;
- Story Thread;
- Quest Projection;
- narrative grammar;
- content schemas.

Không tự thay World State ngoài command/domain event hợp lệ.

<a id="m28-t077-s133"></a>

#### Agent F — Unity Presentation Engineer

Sở hữu:

- UI;
- interaction;
- scene;
- animations;
- view model adapters.

Không viết lại simulation rules.

<a id="m28-t077-s134"></a>

#### Agent G — QA & Tooling Engineer

Sở hữu:

- tests;
- replay;
- seed explorer;
- rule inspector;
- CI;
- state diff;
- content validation.

Các vai trò này không nhất thiết tương ứng bảy người hoặc bảy agent chạy song song liên tục.

Ở giai đoạn đầu, một số nhiệm vụ phải được làm tuần tự để khóa contracts.

<a id="m28-t077-s135"></a>

### 79. Task Contract bắt buộc

Mỗi task giao agent phải có:

```text
TASK ID
Module
Objective
Inputs
Allowed Files/Directories
Dependencies
Required Interfaces
Forbidden Changes
Implementation Requirements
Acceptance Tests
Expected Deliverables
```

Ví dụ:

```text
TASK: M4-04 RepairBoatRule

OBJECTIVE:
Implement deterministic boat repair resolution.

INPUT:
RepairBoatCommand
BoatCondition
MaterialInventory
CharacterCapability
RuleContext

OUTPUT:
Committed BoatRepair events and state mutations.

MUST:
- Validate ownership/access
- Validate materials
- Resolve outcome through shared RNG
- Consume materials atomically
- Persist repair condition
- Emit domain events

MUST NOT:
- Modify Quest status directly
- Call LLM
- Access UnityEngine
- Modify inventory outside Asset Domain
- Use System.Random directly

TESTS:
- No materials -> Rejected
- Valid repair -> Condition changes
- Same seed -> Same result
- Retry -> No duplicate consumption
- Save/load -> State preserved
```

Cấu trúc này giảm khả năng agent tự đoán những quyết định chưa được giao.

<a id="m28-t077-s136"></a>

### 80. Quy tắc Pull Request/Code Review

Mỗi task chỉ được merge khi:

- Build thành công.
- Unit tests đạt.
- Không vi phạm dependency rules.
- Có test cho các invariant liên quan.
- Không thêm library hoặc framework không có lý do.
- Không thay đổi contract domain khác nếu chưa được phê duyệt.
- Không thêm LLM runtime dependency.
- Không đưa logic gameplay vào presentation.

Đối với thay đổi có thể ảnh hưởng determinism:

Phải chạy Replay Tests.

Đối với thay đổi schema:

Phải cập nhật content fixtures và migration policy nếu cần.

<a id="m28-t077-s137"></a>

### 81. Quy tắc khi coding agent gặp thiếu đặc tả

Nếu agent phát hiện cần một rule chưa được thiết kế:

Không được tự tạo công thức gameplay quan trọng rồi coi là canon.

Phải ghi:

```text
DESIGN GAP:
Missing rule for ...
Affected modules: ...
Proposed options: ...
Impact on existing contracts: ...
```

Có thể tạo một placeholder có guard trong nhánh prototype riêng nếu không cản trở test khác.

Nhưng placeholder phải:

- được đánh dấu rõ;
- không âm thầm hoạt động trong production;
- có test xác nhận chưa hỗ trợ;
- không tự trở thành quyết định GDD.

Với các thay đổi nhỏ không ảnh hưởng gameplay semantics, agent có thể triển khai theo conventions đã có.

---

<a id="m28-t077-s138"></a>

## PHẦN XIX — DEFINITION OF DONE THEO NHÓM

<a id="m28-t077-s139"></a>

### 82. Foundation Definition of Done

Một Foundation task hoàn thành khi:

- có public contract;
- có implementation;
- có error model;
- có validation;
- có unit tests;
- có logging/diagnostic;
- không phụ thuộc Unity;
- có ví dụ sử dụng.

Không chấp nhận chỉ có interface và TODO.

<a id="m28-t077-s140"></a>

### 83. Domain Definition of Done

Một Domain task hoàn thành khi:

- xác định authoritative owner;
- có entity/state schema;
- có commands;
- có rule handlers;
- có domain events;
- có invariants;
- có tests;
- có persistence mapping khi cần.

Ví dụ Health Domain không được coi hoàn tất nếu chỉ có một thanh Health trên UI.

Phải có Wound State và hậu quả đủ để Travel hoặc Combat sử dụng.

<a id="m28-t077-s141"></a>

### 84. Narrative Definition of Done

Một Story Thread hoàn thành khi:

- có World Origin;
- actor/role bindings hợp lệ;
- nhiều hướng can thiệp;
- player có thể bỏ qua;
- NPC có thể tiếp tục;
- Quest View phản ánh Knowledge;
- các outcome có state changes thật;
- sau Time Skip thread không mắc kẹt.

Không chấp nhận quest chỉ chạy đúng khi player nói chuyện với NPC theo một thứ tự scripted duy nhất.

<a id="m28-t077-s142"></a>

### 85. Presentation Definition of Done

Một màn hình hoàn thành khi:

- nhận dữ liệu từ projection đúng;
- hiển thị thông tin có nguồn;
- phát command đúng;
- xử lý rejected/conflict/scheduled result;
- không tự tạo authoritative state;
- có keyboard/mouse navigation phù hợp;
- không leak hidden information;
- hỗ trợ text scaling ở mức prototype.

Không chấp nhận màn hình đẹp nhưng không phản ánh game state thật.

---

<a id="m28-t077-s143"></a>

## PHẦN XX — AUTOMATED TEST MATRIX

<a id="m28-t077-s144"></a>

### 86. Test Pyramid

| Test Layer | Phạm vi | Mục tiêu |
|---|---|---|
| Unit | Rule và Value Object | Đúng thuật toán |
| Integration | Nhiều domain | Đúng hợp đồng |
| Property-Based | Nhiều trạng thái đầu vào | Giữ invariant |
| Deterministic Replay | Seed + Commands | Tái hiện outcome |
| Scenario | Gameplay xuyên hệ | Đúng nhân quả |
| Content Validation | Templates và references | Không có content invalid |
| Unity Edit/Play Mode | UI, adapters, scene | Không có logic lệch core |
| Headless Simulation | Nhiều năm và nhiều seeds | Ổn định |
| Performance | CPU, RAM, load/save | Khả thi |

Mỗi nhóm có exit criteria riêng.

<a id="m28-t077-s145"></a>

### 87. P0 Automated Test Cases

<a id="m28-t077-s146"></a>

#### T01 — Stable Entity Identity

Khởi tạo World.

Sinh entity.

Save/load.

Identity không đổi.

Không có duplicate.

<a id="m28-t077-s147"></a>

#### T02 — Atomic Resource Transfer

Chuyển vật liệu từ Household A sang B.

Một lần thành công.

Gửi lại cùng Command ID.

Không chuyển lần thứ hai.

<a id="m28-t077-s148"></a>

#### T03 — Dead Actor Prevention

Một actor chết.

Tới thời điểm work assignment.

Scheduler không cho actor tiếp tục làm việc.

<a id="m28-t077-s149"></a>

#### T04 — Knowledge Isolation

Một event xảy ra ở vùng khác.

Actor chưa nhận tin.

NPC Planner và Journal không biết event.

<a id="m28-t077-s150"></a>

#### T05 — Deterministic Repair

Cùng Repair Command, Snapshot và Ruleset.

Outcome, material consumption và event hash giống nhau.

<a id="m28-t077-s151"></a>

#### T06 — Time Skip Correctness

Chạy cùng khoảng thời gian bằng bước nhỏ và bằng Time Skip aggregate.

So sánh các authoritative outputs thuộc phạm vi yêu cầu tương đương.

Nếu có khác biệt do approximation LOD, phải nằm trong chính sách đã thiết kế và không phá named-entity invariants.

<a id="m28-t077-s152"></a>

#### T07 — Route Availability

Cầu hỏng.

Cargo chưa tới.

Travel Path không được phép đi qua cầu như đang mở.

<a id="m28-t077-s153"></a>

#### T08 — Quest Independence

Một Event đã xảy ra.

Player chưa phát hiện.

World State và Story Thread tồn tại.

Journal chưa hiển thị nội dung ẩn.

<a id="m28-t077-s154"></a>

#### T09 — Succession Knowledge

Predecessor biết bí mật.

Successor chưa được truyền.

Không copy Knowledge.

<a id="m28-t077-s155"></a>

#### T10 — Historical Constraint

Supply Chain bị phá.

Historical Scenario Evaluator chọn đường hợp lệ hoặc báo không khả thi.

Không sinh resource miễn phí.

<a id="m28-t077-s156"></a>

#### T11 — Save Recovery

Mô phỏng lỗi trong giai đoạn commit/save.

Sau khi khôi phục, dữ liệu ở một committed state nhất quán.

<a id="m28-t077-s157"></a>

#### T12 — No Runtime LLM

Ngắt toàn bộ network access.

Game vẫn thực hiện Character Generation, NPC AI, Event, Combat, Dialogue Template, Time Skip và Save/Load.

<a id="m28-t077-s158"></a>

### 88. Combat & Health Tests

Bài test phải kiểm tra:

- action budget;
- position;
- reach;
- cover;
- armor;
- stamina;
- wound;
- retreat;
- incapacitation;
- morale;
- objective.

Ví dụ:

Một actor bị Serious Leg Wound.

Sau combat, Travel Capability giảm theo rule.

Sau treatment và Time Skip phù hợp, condition được cập nhật.

Không có bước nào tự reset wound khi đổi scene.

<a id="m28-t077-s159"></a>

### 89. Social & Knowledge Tests

Bài test phải kiểm tra:

- một lời hứa với deadline;
- lời hứa hoàn thành muộn;
- người hứa chết;
- tin đồn nguồn gốc chung;
- hai lời kể mâu thuẫn;
- một accusation sai;
- NPC không biết hidden truth;
- companion từ chối action;
- Faction authority trong Agreement.

Mỗi trường hợp cần xác định chính xác:

- ai biết;
- ai có quyền;
- event nào đã commit;
- phản ứng nào được phép xảy ra.

---

<a id="m28-t077-s160"></a>

## PHẦN XXI — PERFORMANCE BENCHMARK PLAN

<a id="m28-t077-s161"></a>

### 90. Không bịa target performance trước benchmark

Các mục tiêu như:

> “Mô phỏng 5.000 NPC trong 3ms”

không có cơ sở nếu chưa có implementation, dữ liệu và thiết bị mục tiêu.

Cần phân biệt:

**Engineering Budget:** Mức giới hạn đề ra để thiết kế hướng tới.

**Measured Performance:** Kết quả thực tế.

**Release Requirement:** Mức được chấp nhận sau profiling và kiểm thử.

Vertical Slice phải tạo đủ dữ liệu để đặt target thực tế cho full game.

<a id="m28-t077-s162"></a>

### 91. Benchmark Scenarios

<a id="m28-t077-s163"></a>

#### B1 — Local Settlement

Sử dụng fixture Bến Vạc.

Đo:

- NPC decisions;
- event eligibility;
- social graph;
- resource update;
- command execution.

<a id="m28-t077-s164"></a>

#### B2 — Multi-Settlement

Ba cộng đồng kết nối.

Đo:

- travel;
- shipments;
- economy;
- information propagation;
- environmental changes.

<a id="m28-t077-s165"></a>

#### B3 — Population Scale Simulation

Sử dụng generated synthetic world lớn hơn playable slice.

Có thể thử các mức tăng dần tới mục tiêu thiết kế hàng nghìn NPC.

Không cần dựng animation hoặc scene cho toàn bộ.

Đo:

- CPU;
- memory;
- event throughput;
- scheduler queue;
- serialization size.

<a id="m28-t077-s166"></a>

#### B4 — Long Time Skip

Chạy nhiều năm simulation.

Đo:

- số event;
- duration;
- save size;
- allocations;
- memory growth;
- consistency errors.

<a id="m28-t077-s167"></a>

#### B5 — Burst Events

Tạo một tình huống có nhiều event cùng thời điểm:

- lũ;
- nhiều shipment bị chậm;
- household thiếu nguồn lực;
- actor decisions;
- rumor propagation.

Đo:

- event fan-out;
- queue stability;
- memory;
- số event duplicate;
- transaction throughput.

<a id="m28-t077-s168"></a>

### 92. Metrics bắt buộc ghi lại

```text
SimulationTicksProcessed
WallClockDuration
EventsCommitted
CommandsProcessed
RejectedCommands
ActiveNPCCount
BackgroundNPCCount
SchedulerQueueSize
PeakMemory
AllocatedBytes
SaveSize
SaveDuration
LoadDuration
InvariantViolations
ReplayHashMismatches
```

Đối với benchmark, log quá chi tiết có thể ảnh hưởng hiệu năng.

Nên có hai chế độ:

**Functional Trace Mode:** Ghi đủ để debug.

**Performance Mode:** Chỉ ghi metric tổng hợp và các error nghiêm trọng.

<a id="m28-t077-s169"></a>

### 93. Tiêu chí đánh giá mở rộng

Không chỉ hỏi:

> “Game có đạt FPS không?”

Phải hỏi:

- Simulation có ổn định khi tăng NPC không?
- Thời gian xử lý có tăng gần tuyến tính hay tăng bùng nổ?
- Social Graph có trở thành nút thắt không?
- Event Eligibility có quét quá nhiều entity không?
- Save Data có tăng vô hạn không?
- Time Skip có thể hoàn thành trong thời gian trải nghiệm chấp nhận được không?
- Headless và Unity có dùng cùng rules không?

Đây là lý do cần benchmark trước khi mở rộng lên toàn bộ 12 vùng.

---

<a id="m28-t077-s170"></a>

## PHẦN XXII — CONTENT PRODUCTION STRATEGY

<a id="m28-t077-s171"></a>

### 94. Content phải có pipeline riêng

Không để narrative designer trực tiếp sửa game code cho mỗi Event Template.

Quy trình:

```text
Content Design
      |
      v
Structured Template
      |
      v
Schema Validation
      |
      v
Reference Validation
      |
      v
Causal Validation
      |
      v
Simulation Test
      |
      v
Review / Approval
      |
      v
Compiled Content Pack
```

LLM có thể hỗ trợ ở bước tạo Structured Template.

Không có LLM ở bước runtime resolution.

<a id="m28-t077-s172"></a>

### 95. Content Compiler

Đầu ra của compiler cần:

- IDs đã chuẩn hóa;
- references hợp lệ;
- rule handlers đã đăng ký;
- không có duplicate definitions;
- content version;
- localization references;
- warning cho unreachable content.

Có thể sinh một manifest:

```json
{
  "pack_id": "tamgiang_vertical_slice_v1",
  "schema_version": 1,
  "ruleset": "prototype_rules_v1",
  "regions": ["region_tamgiang"],
  "event_templates": 10,
  "dialogue_sets": 12,
  "validation_status": "passed"
}
```

Các số lượng là minh họa cho cấu trúc manifest, không khẳng định content đã được tạo.

`validation_status` phải do pipeline thật tính, không do LLM tự tuyên bố.

<a id="m28-t077-s173"></a>

### 96. Content QA

Content cần bị đánh dấu lỗi khi:

- một actor không có motive;
- một required role không thể binding;
- event tạo resource không nguồn;
- event tự hoàn thành khi player click dialogue;
- không có outcome nếu player bỏ qua;
- source knowledge không hợp lệ;
- historical/game-canon classification sai;
- tên hoặc nội dung lặp quá mức;
- outcome mô tả một thương tích nhưng Health State không tạo wound.

Không nên chỉ kiểm tra JSON đúng cú pháp.

<a id="m28-t077-s174"></a>

### 97. Event Authoring Tool tối thiểu

Ở Vertical Slice đầu tiên, có thể dùng:

- schema-validated JSON;
- CLI Validator;
- Rule Inspector;
- event trace;
- một màn hình debug đơn giản.

Chưa cần xây ngay một Visual Node Editor phức tạp kiểu kéo-thả.

Visual Editor chỉ có giá trị khi data contract và event semantics đã đủ ổn định.

Xây quá sớm sẽ khiến team phải sửa editor mỗi lần đổi schema.

---

<a id="m28-t077-s175"></a>

## PHẦN XXIII — SOURCE-OF-TRUTH CHO CODING AGENT

<a id="m28-t077-s176"></a>

### 98. Bộ tài liệu phải đưa cho agent

Không nên gửi nguyên 28 module mà không có mục lục hoặc thứ tự ưu tiên.

Đề xuất tạo một **Development Context Pack** gồm:

**PROJECT_VISION.md:** Triết lý, nguyên tắc bất biến, bản sắc gameplay.

**GAMEPLAY_CANON.md:** Những quyết định gameplay đã khóa.

**ARCHITECTURE.md:** Modular Monolith và dependency boundaries.

**DOMAIN_OWNERSHIP.md:** Nguồn authoritative của từng loại dữ liệu.

**COMMAND_EVENT_CONTRACT.md:** Mutation pipeline.

**RULE_SPEC.md:** Công thức và trạng thái đã khóa.

**CONTENT_SCHEMA.md:** Event Templates và definitions.

**VERTICAL_SLICE_SCOPE.md:** Phạm vi đang triển khai.

**TASKS.md:** Backlog và thứ tự dependencies.

**TEST_PLAN.md:** Acceptance tests.

**DECISION_LOG.md:** Các thay đổi thiết kế được phê duyệt.

GDD đầy đủ vẫn được giữ để tra cứu, nhưng các file trên giúp agent tìm nhanh quyết định cần thiết.

<a id="m28-t077-s177"></a>

### 99. Instruction cho coding agent

Một instruction nền nên quy định:

> Implement only within the assigned domain and contract. Gameplay decisions must be deterministic and rule-driven. Do not introduce runtime LLM calls. Do not modify World State directly outside the authoritative command/commit pipeline. Do not invent new gameplay mechanics or formulas when requirements are missing. Add tests for invariants, domain behavior, replay and cross-domain integration. Preserve compatibility with the headless simulation core.

Nội dung này có thể chuyển thành `AGENTS.md` hoặc cấu hình tương đương tùy công cụ lập trình.

Quy tắc quan trọng hơn tên file.

<a id="m28-t077-s178"></a>

### 100. Agent Handoff

Mỗi agent hoàn thành task cần bàn giao:

- danh sách file đã tạo/sửa;
- public interfaces;
- test cases;
- kết quả test;
- những quyết định triển khai;
- các design gaps;
- các dependencies mới;
- những hạn chế chưa xử lý.

Không chấp nhận chỉ bàn giao:

> “Done, implementation complete.”

Một task được xem hoàn thành khi có bằng chứng kỹ thuật kiểm chứng được.

---

<a id="m28-t077-s179"></a>

## PHẦN XXIV — VERTICAL SLICE PLAYTEST DESIGN

<a id="m28-t077-s180"></a>

### 101. Playtest không được chỉ đánh giá UI và combat

Cần đánh giá các giả thuyết:

**Agency:** Player có cảm thấy mình thực sự quyết định không?

**Belonging:** Player có quan tâm tới ít nhất một NPC hoặc household không?

**Mastery:** Hoạt động nghề nghiệp có thú vị và đáng phát triển không?

**Discovery:** Player có muốn tìm hiểu thêm về địa điểm, thông tin và con người không?

**Consequence:** Player có nhận ra kết quả hành động thay đổi World State không?

**Clarity:** Player có hiểu điều gì chắc chắn và điều gì chỉ là tin đồn không?

<a id="m28-t077-s181"></a>

### 102. Playtest Session

Một bài test có thể chia thành các phần.

**Khởi đầu:** Tester vào game với một Origin Fixture.

**Tự do khám phá:** Không ép họ làm ngay Thread chính.

**Tình huống có vấn đề:** Một opportunity được phơi bày qua kênh hợp lệ.

**Lựa chọn:** Tester tự quyết định cách can thiệp.

**Hậu quả:** Kiểm tra họ hiểu điều gì đã thay đổi.

**Nhật ký:** Yêu cầu tester tìm lại nguồn của một thông tin.

**Tiếp tục:** Xem tester có tự xác định được mục tiêu tiếp theo không.

Không dùng moderator giải thích cơ chế liên tục.

Nếu tester không hiểu game, cần xem đó là vấn đề UX hoặc onboarding thay vì mặc định cho rằng họ chơi sai.

<a id="m28-t077-s182"></a>

### 103. Những câu hỏi cần quan sát

- Người chơi có nhận ra mình đang sống trong một hoàn cảnh riêng không?
- Họ có thấy NPC thực sự có mục tiêu không?
- Khi họ không can thiệp, có diễn biến nào dễ hiểu xảy ra không?
- Họ có phân biệt được lời kể và bằng chứng không?
- Công việc sửa chữa/vận tải có tạo lựa chọn hay chỉ là bấm nút?
- Thất bại có thể hiểu nguyên nhân không?
- Họ có quan tâm tới việc quay lại settlement sau một thời gian không?
- Có cảm thấy World Director đang ép họ gặp biến cố không?

<a id="m28-t077-s183"></a>

### 104. Chỉ số UX đề xuất

Các metric nên thu thập:

- số người hoàn thành onboarding mà không cần trợ giúp;
- thời gian tìm được thông tin trong Journal;
- số trường hợp hiểu sai một claim thành fact;
- số người không biết nên làm gì tiếp;
- tỷ lệ player tự chọn noncombat solution;
- số người nhận ra NPC đã tự hành động;
- mức độ ghi nhớ NPC sau phiên chơi;
- số trường hợp không hiểu vì sao action thất bại.

Các chỉ số này cần được so sánh với ghi chú quan sát và phỏng vấn playtest.

Không nên biến chúng thành một điểm chất lượng duy nhất.

---

<a id="m28-t077-s184"></a>

## PHẦN XXV — RISK REGISTER CHO VERTICAL SLICE

<a id="m28-t077-s185"></a>

### 105. Rủi ro P0 — Core không thực sự deterministic

**Biểu hiện:** Cùng save và command sequence tạo kết quả khác.

**Nguyên nhân có thể:**

- `System.Random` không được version hóa;
- thứ tự duyệt dữ liệu không ổn định;
- dependency vào Unity frame;
- multithread ghi state;
- thời gian hệ điều hành được dùng làm gameplay input.

**Giải pháp:** Seeded/Versioned RNG, command log, stable event ordering, canonical state hash.

**Gate:** Không merge các domain lớn trước khi Replay Test đạt.

<a id="m28-t077-s186"></a>

### 106. Rủi ro P0 — Nhiều domain cùng sửa state

**Biểu hiện:** Inventory và Quest cùng cấp vật phẩm; Health và Combat lưu wound riêng.

**Giải pháp:** Domain Ownership Contract và authoritative commit pipeline.

**Gate:** Architecture tests kiểm tra dependencies; integration tests kiểm tra event effects.

<a id="m28-t077-s187"></a>

### 107. Rủi ro P0 — Narrative tồn tại độc lập World Simulation

**Biểu hiện:** Quest chạy nhưng NPC không có nhu cầu thật; sự kiện chỉ được tạo khi player tới.

**Giải pháp:** World Pressure và NPC Action phải tạo State trước Story Thread.

**Gate:** Headless Player-Absent Test.

<a id="m28-t077-s188"></a>

### 108. Rủi ro P0 — Knowledge leakage

**Biểu hiện:** Journal biết hung thủ; NPC biết sự kiện không chứng kiến.

**Giải pháp:** Scoped Knowledge API, explicit provenance, projection filters.

**Gate:** Hidden Event Isolation Test.

<a id="m28-t077-s189"></a>

### 109. Rủi ro P0 — Time Skip làm mất sự kiện

**Biểu hiện:** Lời hứa đến hạn không được xử lý; NPC chết nhưng vẫn làm nghề.

**Giải pháp:** Scheduled Event Boundaries và validation.

**Gate:** Compare direct progression và Time Skip theo hợp đồng tương đương.

<a id="m28-t077-s190"></a>

### 110. Rủi ro P1 — Scope phình quá nhanh

**Biểu hiện:** Thêm hàng chục Class/Profession/Settlement trước khi core hoạt động.

**Giải pháp:** Scope Freeze theo milestones; chỉ mở rộng sau Go Gate.

**Gate:** Không mở rộng nội dung trước khi Vertical Slice đạt các tiêu chí nhân quả.

<a id="m28-t077-s191"></a>

### 111. Rủi ro P1 — Profession gameplay nhàm chán

**Biểu hiện:** Crafting chỉ có một nút tiến độ; vận tải chỉ là fast travel.

**Giải pháp:** Mỗi Activity có preparation, risk, trade-offs và hậu quả.

**Gate:** Playtest có ý nghĩa ngay cả khi không xuất hiện combat.

<a id="m28-t077-s192"></a>

### 112. Rủi ro P1 — Coders xây nhiều lớp abstraction nhưng thiếu gameplay

**Biểu hiện:** Repository có hàng trăm interfaces nhưng không có end-to-end scenario chạy được.

**Giải pháp:** Mỗi milestone phải có một vertical test.

**Gate:** Không chỉ nghiệm thu class hoặc code coverage; cần có workflow thực thi hoàn chỉnh.

---

<a id="m28-t077-s193"></a>

## PHẦN XXVI — GO / NO-GO DECISION

<a id="m28-t077-s194"></a>

### 113. Các điều kiện bắt buộc trước khi mở rộng Full Production

Vertical Slice chỉ được coi là thành công khi đạt các điều kiện sau.

<a id="m28-t077-s195"></a>

#### Architecture Gate

- Headless core chạy độc lập.
- Commands và events có cấu trúc đúng.
- Không có P0 invariant violation.
- Save/Load hoạt động.
- Deterministic replay đạt trong phạm vi đã định nghĩa.
- Không cần runtime LLM.

<a id="m28-t077-s196"></a>

#### World Simulation Gate

- Settlement có thể tự vận hành.
- NPC ra quyết định hợp lệ.
- Household/resource thay đổi có nguyên nhân.
- Player vắng mặt không làm World Time đóng băng.

<a id="m28-t077-s197"></a>

#### Gameplay Gate

- Có hoạt động nghề ngoài combat có chiều sâu.
- Travel sử dụng route thực.
- Inventory/Assets không nhân đôi.
- Combat dùng Tactical Zones và có hậu quả Health.
- Có nhiều con đường giải một tình huống.

<a id="m28-t077-s198"></a>

#### Narrative Gate

- Story Thread có origin từ World State.
- Knowledge có source.
- Journal không lộ hidden truth.
- Player có thể bỏ qua Story Thread.
- NPC vẫn có thể giải quyết hoặc làm biến đổi vấn đề.

<a id="m28-t077-s199"></a>

#### Long-Term Gate

- Time Skip xử lý đúng các event.
- Character có progression theo điều kiện.
- Death/Succession không phá World State.
- Legacy có khả năng tồn tại sau nhiều năm.

<a id="m28-t077-s200"></a>

#### Production Gate

- Content Pipeline có validation.
- Team có Rule Inspector/Event Trace.
- Có benchmark thực tế.
- Có Playtest Report.
- Có backlog Technical Debt được phân loại.

<a id="m28-t077-s201"></a>

### 114. Điều kiện No-Go

Không nên mở rộng toàn bộ thế giới nếu:

- Replay liên tục sai;
- NPC chỉ hoạt động khi player nhìn thấy;
- Quest tự tạo vật phẩm hoặc NPC;
- World Director phải gian lận để tạo trải nghiệm;
- Time Skip phá quan hệ/tài sản;
- Succession chỉ là copy character;
- Crafting/Profession thiếu quyết định thực sự;
- Performance không có khả năng mở rộng;
- team không thể debug nguyên nhân một event.

Trong trường hợp này, ưu tiên sửa Foundation hoặc Core Loop.

Không bổ sung thêm narrative content để che vấn đề.

---

<a id="m28-t077-s202"></a>

## PHẦN XXVII — DELIVERABLES CUỐI CÙNG

<a id="m28-t077-s203"></a>

### 115. Source Code Deliverables

Bản bàn giao cần có:

**Simulation Core:** Build được, chạy headless.

**Unity Client:** Có thể chơi scenario mẫu.

**Persistence Adapter:** Save/Load được.

**Content Pack:** World Fixture và Event Templates.

**Automated Tests:** Unit, integration, replay, scenario.

**Developer Tools:** CLI hoặc UI debug đủ để truy nguồn nhân quả.

**CI Pipeline:** Tự build/test/validate.

<a id="m28-t077-s204"></a>

### 116. Documentation Deliverables

Các file tài liệu tối thiểu:

```text
README.md
ARCHITECTURE.md
DOMAIN_OWNERSHIP.md
COMMAND_EVENT_CONTRACT.md
RULES_BASELINE.md
CONTENT_SCHEMA.md
VERTICAL_SLICE_SCOPE.md
BUILD_AND_RUN.md
TEST_PLAN.md
REPLAY_DEBUG_GUIDE.md
KNOWN_LIMITATIONS.md
DECISION_LOG.md
```

Đây không phải documentation hình thức.

Ví dụ `REPLAY_DEBUG_GUIDE.md` phải chỉ được:

- lấy seed ở đâu;
- load snapshot nào;
- chạy command log;
- so sánh state hash;
- xem event trace;
- tìm rule gây sai lệch.

<a id="m28-t077-s205"></a>

### 117. QA Evidence Package

Cần có:

- danh sách tests và kết quả;
- replay fixtures;
- sample save;
- headless benchmark reports;
- playtest notes;
- content validation reports;
- kiến trúc dependency report;
- danh sách lỗi mở;
- các mục tiêu release còn chưa đạt.

Không được chỉ dựa trên video demo để chứng minh tính đúng của engine.

---

<a id="m28-t077-s206"></a>

## PHẦN XXVIII — QUYẾT ĐỊNH ĐỀ XUẤT KHÓA

<a id="m28-t077-s207"></a>

### 118. Scope Decisions

1. Vertical Slice tập trung tại Tam Giang.
2. Bến Vạc là địa điểm thử nghiệm chính, cần đối chiếu world-canon khi mở rộng.
3. Ba Activity cốt lõi: Vận tải, Sửa chữa, Chữa trị.
4. Có một Combat Encounter không bắt buộc tiêu diệt toàn bộ đối thủ.
5. Có ba Story Thread liên kết nhưng độc lập về trạng thái.
6. Có ít nhất một vấn đề NPC tự giải quyết được khi player vắng mặt.
7. Có Time Skip và Succession qua các test fixture hợp lệ.
8. Có Historical Anchor proof ở headless, không cần dựng đủ Cổ Loa.
9. Chưa triển khai toàn bộ 12 vùng và sáu hồi.
10. Không dùng số lượng content làm tiêu chí hoàn thành chính.

<a id="m28-t077-s208"></a>

### 119. Technical Decisions

11. Simulation Core độc lập với Unity.
12. C#/.NET Standard 2.1 là baseline tham chiếu.
13. Unity là presentation engine tham chiếu.
14. SQLite qua persistence adapter, chưa khóa là lựa chọn vĩnh viễn.
15. Authoritative mutation đi qua Command/Commit Pipeline.
16. Có Entity ID ổn định.
17. Có Event Journal và Snapshot.
18. Có deterministic RNG được version hóa.
19. Có controlled simulation clock.
20. Không sử dụng runtime LLM decision.
21. Content dùng approved rule handlers.
22. UI không tự ghi World State.
23. NPC dùng perceived knowledge.
24. Story Thread không tạo physical truth.
25. Headless Test Runner là thành phần bắt buộc.

<a id="m28-t077-s209"></a>

### 120. Production Decisions

26. Contract-first development.
27. Coding agents phải làm theo scoped tasks.
28. Mỗi task có acceptance tests.
29. Không cho agent tự phát minh công thức quan trọng.
30. Mỗi milestone có end-to-end demo.
31. CI phải kiểm tra architecture, tests và content.
32. Multi-seed testing trước khi scale NPC.
33. Playtest ngoài team trước khi Go/No-Go.
34. Không mở rộng full production nếu còn P0 blocker.
35. Những tuning values của Module 27 vẫn ở trạng thái prototype cho tới khi có simulation results.

---

<a id="m28-t077-s210"></a>

## PHẦN XXIX — TIÊU CHÍ THÀNH CÔNG CUỐI CÙNG

<a id="m28-t077-s211"></a>

### 121. Vertical Slice phải chứng minh một cuộc đời, không phải một quest

Một player bắt đầu với hoàn cảnh riêng.

Họ làm việc, học hỏi và xây dựng quan hệ.

Một vấn đề xảy ra trong thế giới.

Họ có thể can thiệp hoặc bỏ qua.

Họ có thể thành công hoặc thất bại.

Những người khác tiếp tục cuộc sống.

Thời gian trôi.

Một công trình, vật phẩm, kỹ thuật hoặc quan hệ có thể thay đổi.

Một người có thể trưởng thành hoặc qua đời.

Một người khác có thể tiếp tục câu chuyện.

Và những gì đã xảy ra không bị xóa chỉ vì player chuyển scene, load lại game hoặc đổi protagonist.

Nếu Vertical Slice thực sự làm được điều đó, chúng ta đã chứng minh được nền gameplay cốt lõi.

<a id="m28-t077-s212"></a>

### 122. Hàng triệu nhánh chỉ xuất hiện sau khi một nhánh hoạt động đúng

Không nên bắt đầu bằng mục tiêu:

> Sinh được một triệu event combinations.

Bắt đầu bằng:

> Một event được sinh từ đúng nguyên nhân.

Sau đó:

> Một event có nhiều kết quả có ý nghĩa.

Tiếp theo:

> Kết quả của event ảnh hưởng các actor và domain khác nhau.

Rồi:

> Những domain đó tạo ra vấn đề mới theo quy tắc.

Cuối cùng:

> Các chuỗi này có thể tiếp tục trong nhiều năm mà không phá tính nhất quán.

Khi engine làm đúng việc đó, số lượng tổ hợp có thể tăng rất lớn mà không cần một triệu nhánh authored.

<a id="m28-t077-s213"></a>

### 123. Tuyên ngôn triển khai

**Không bắt đầu bằng việc xây một thế giới rộng.**

Bắt đầu bằng việc xây một thế giới nhỏ nhưng có thật.

**Không bắt đầu bằng hàng nghìn NPC.**

Bắt đầu bằng vài chục người có mục tiêu, quan hệ, nghề nghiệp và khả năng hành động độc lập.

**Không bắt đầu bằng hàng trăm quest.**

Bắt đầu bằng một số vấn đề có nguyên nhân và nhiều con đường giải quyết.

**Không bắt đầu bằng hệ thống AI tạo truyện realtime.**

Bắt đầu bằng một Rule Engine có thể giải thích vì sao từng sự kiện xảy ra.

**Không bắt đầu bằng một cutscene lịch sử hoành tráng.**

Bắt đầu bằng một chiếc thuyền có chủ, một người có trách nhiệm với nó, một tuyến vận tải phụ thuộc vào nó và những hậu quả xảy ra nếu nó không thể hoạt động.

Nếu chiếc thuyền đó có thể tồn tại, được sửa, bị hỏng, được trao quyền sử dụng, trở thành một phần ký ức và tiếp tục ảnh hưởng cộng đồng sau nhiều năm, chúng ta đã chứng minh được mô hình ở quy mô nhỏ.

Từ đó có thể mở rộng sang:

- nhiều household;
- nhiều nghề;
- nhiều settlement;
- nhiều vùng;
- các faction;
- Cổ Loa;
- chiến tranh;
- lời thề;
- ký ức;
- truyền thuyết.

**Vertical Slice không phải phiên bản thu nhỏ về số lượng của toàn bộ game.**

**Nó là phiên bản thu nhỏ về quy mô nhưng đầy đủ về nguyên lý vận hành của game.**

---

<a id="m28-t077-s214"></a>

## PHẦN XXX — KẾT LUẬN MODULE 28

<a id="m28-t077-s215"></a>

### 124. Trạng thái kỹ thuật sau Module 28

Sau Module 28, dự án đã có ở cấp độ đặc tả:

- mục tiêu Vertical Slice;
- phạm vi nội dung;
- stack tham chiếu;
- kiến trúc repository;
- các domain cần triển khai;
- hợp đồng command/event;
- dữ liệu fixture;
- kịch bản thử nghiệm xuyên hệ;
- thiết kế NPC Autonomy;
- yêu cầu gameplay nghề;
- travel/combat/injury integration;
- Time Skip/Succession proof;
- Historical Anchor test;
- backlog milestone;
- task contract cho coding agents;
- automated test matrix;
- performance benchmark plan;
- Go/No-Go criteria.

**Điều chưa có:** Source code, game assets, content packs hoàn chỉnh, benchmark thực đo và playable build.

Các kết quả này phải được tạo và kiểm chứng trong giai đoạn implementation thực tế.

<a id="m28-t077-s216"></a>

### 125. Bước chuyển giao đề xuất

Bước tiếp theo không nên là mở rộng thêm một module gameplay lớn.

Nên chuyển sang:

<a id="m28-t077-s217"></a>

## MODULE 29 — ENGINE FOUNDATION IMPLEMENTATION SPECIFICATION

Đây là bước cụ thể hóa M0 và M1 thành các artifacts kỹ thuật có thể lập trình ngay.

Nội dung cần có:

**Entity Registry:** Stable IDs và lifecycle.

**WorldClock:** Tick, calendar, scheduler boundaries.

**Deterministic RNG:** PRNG versioning, random context, replay.

**Command Processor:** Validation, authorization, handler, commit.

**Domain Event Bus:** Event semantics, subscribers, causation.

**World State Store:** Authoritative state, revisions, transaction.

**Persistence:** Save, snapshot, event journal và recovery.

**Content Loader:** Schema, definitions, references.

**Test Harness:** Headless runner, state hash, fixtures, replay.

**Development Contracts:** Interfaces, class structures, error codes và test cases.

Module 29 phải có đủ chi tiết để một coding agent bắt đầu tạo repository và viết mã nguồn Foundation theo những task rõ ràng, không phải tự suy đoán kiến trúc.

Sau khi Foundation hoạt động, các module triển khai tiếp theo mới nên phát triển Living Settlement và Character Simulation.

---

<a id="m28-t077-s218"></a>

### TUYÊN NGÔN CUỐI CÙNG

**Chúng ta đang xây dựng một hệ thống mô phỏng có thể tạo ra câu chuyện, không xây dựng một hệ thống kể chuyện giả lập mô phỏng.**

Thế giới phải hoạt động trước khi câu chuyện được trình bày.

Con người phải có mục tiêu trước khi trở thành NPC đưa nhiệm vụ.

Sự kiện phải có nguyên nhân trước khi trở thành Quest.

Một kết quả phải được engine xác lập trước khi được biến thành đoạn hội thoại.

Một di sản phải có vật mang hoặc người tiếp nối trước khi được ghi vào Chronicle.

Nếu những nguyên tắc này được chứng minh bằng Vertical Slice, dự án có cơ sở kỹ thuật thực sự để phát triển thành một Living World Narrative RPG quy mô lớn.

---

<a id="m28-t077-s219"></a>

### Kết luận và thứ tự hành động

Tôi khuyến nghị **phê duyệt Module 28 làm phạm vi phát triển bản thử nghiệm đầu tiên**, với một lưu ý: các tên địa điểm, số lượng NPC, số lượng template và các tham số hiệu năng vẫn là dữ liệu fixture hoặc ngân sách thiết kế, chưa phải canon hay benchmark đã xác nhận.

Trước khi giao agent viết nhiều module song song, nên hoàn thành đúng thứ tự: **M0 Architecture Contracts → M1 Deterministic Kernel → M2 Living Settlement → M3 Knowledge/Narrative → M4 Physical Gameplay → M5 Long-Term Simulation → M6 Playable Client → M7 Validation**.

Về nguồn kỹ thuật, có thể đối chiếu trực tiếp <Link url="https://docs.unity.com/en-us/engine/7000.0/manual/programming-environment/overview-of-dot-net-in-unity/dotnet-profile-support" title="Unity — .NET API Compatibility"/>, <Link url="https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-random" title="Microsoft — Random Reproducibility"/>, <Link url="https://www.sqlite.org/transactional.html" title="SQLite — Transaction Guarantees"/> và <Link url="https://json-schema.org/draft/2020-12/" title="JSON Schema Specification"/>.

**Bước phù hợp tiếp theo là Module 29 — Engine Foundation Implementation Specification:** chuyển kiến trúc này thành đặc tả source code, API, class/interface, luồng transaction, PRNG và test cụ thể để bắt đầu xây engine mà không đánh mất những quyết định thiết kế đã khóa.

<!-- SOURCE T077 END -->

