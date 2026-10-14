# Module 24 — Deterministic Engine và AI-assisted Content Pipeline

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T061](../../source/conversation.md#t061)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Áp dụng baseline cuối Module 27: LLM chỉ hỗ trợ content offline; không tích hợp runtime LLM vào slice. Nội dung phải qua schema/semantic validation và approved handlers. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Seed, versioned rules/content, world state và authorized commands. |
| Đầu ra / bên sở hữu | Deterministic simulation và validated content pack; runtime core authoritative. |
| Đọc cùng | 25–29; ADR-002/003.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Gameplay và content loading hoạt động offline; không có network/LLM dependency trong core. |
| Việc chưa được chứng minh | Content compiler implementation, RNG cross-runtime conformance và content budget; algorithm prototype đã chọn ADR-008. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T061 BEGIN -->

<a id="m24-t061-s001"></a>

## GDD MODULE 24
<a id="m24-t061-s002"></a>

## DETERMINISTIC WORLD ENGINE, AI-ASSISTED CONTENT PIPELINE & NARRATIVE SIMULATION ARCHITECTURE

**Trạng thái:** Kiến trúc lõi — đề xuất khóa chính thức  
**Phạm vi:** Sinh nhân vật, sinh sự kiện, World Simulation, Character AI, Rule Engine, Randomization, Story Thread, Narrative Content, Knowledge Graph, World Director, lưu trữ, kiểm thử và công cụ sản xuất nội dung.  
**Nguyên tắc cao nhất:** **LLM có thể hỗ trợ tạo nguyên liệu thiết kế. Code Engine mới là nơi tạo và quyết định sự thật của thế giới.**

---

<a id="m24-t061-s003"></a>

## PHẦN I — QUYẾT ĐỊNH KIẾN TRÚC NỀN TẢNG

<a id="m24-t061-s004"></a>

### 1. Phân biệt AI hỗ trợ sáng tạo và AI quyết định gameplay

Trong hệ thống này, từ “AI” rất dễ gây nhầm lẫn giữa ba loại công nghệ có nhiệm vụ khác nhau.

**AI hỗ trợ sản xuất nội dung** là các mô hình ngôn ngữ được sử dụng bởi đội phát triển để tạo bản nháp nhân vật, địa điểm, sự kiện, đối thoại hoặc dữ liệu nền.

**Game AI** là thuật toán được lập trình để điều khiển quyết định của NPC, phe phái, động vật và các hệ thống khác. Những thuật toán này có thể sử dụng Utility AI, Finite State Machine, Behavior Tree, Goal Planning hoặc các công thức xác suất.

**LLM trình bày nội dung** là một lớp tùy chọn, được phép diễn đạt dữ liệu đã xác nhận thành lời văn nhưng không có quyền thay đổi kết quả mô phỏng.

Ba loại này phải được phân tách ngay trong kiến trúc.

Không được thiết kế một service gọi LLM rồi giao cho nó đồng thời các quyền:

- sinh ra sự kiện;
- xác định sự kiện có hợp lệ;
- quyết định ai tham gia;
- quyết định thành công/thất bại;
- thay đổi thuộc tính NPC;
- viết hội thoại;
- cập nhật database.

Mô hình như vậy ban đầu có vẻ linh hoạt nhưng rất khó bảo đảm tính nhất quán, tính tái hiện và khả năng kiểm thử sau hàng chục năm game-time.

<a id="m24-t061-s005"></a>

### 2. Quyết định runtime

**Đề xuất khóa: Bản game hoàn chỉnh phải có khả năng vận hành toàn bộ gameplay mà không gọi bất kỳ LLM nào.**

Điều này có nghĩa:

- Character Generation chạy bằng code và dữ liệu.
- NPC Simulation chạy bằng code.
- NPC Decision chạy bằng thuật toán có quy tắc.
- Event Generation chạy bằng code và các Event Template đã được phê duyệt.
- Quest Progression chạy bằng code.
- Combat, Injury và Death chạy bằng code.
- Economy, Faction, Ecology và Aging chạy bằng code.
- Dialogue Logic chạy bằng code và Social State.
- Historical Anchor chạy bằng code và dữ liệu campaign.
- World Director chạy bằng thuật toán điều phối có giới hạn.

Để đạt chất lượng nội dung, những dữ liệu này có thể được tạo hoặc mở rộng bởi LLM trong quá trình phát triển.

Nhưng dữ liệu đã được chuẩn hóa và phê duyệt trước khi đưa vào phiên bản game.

Trong runtime, engine chỉ đọc dữ liệu đó và xử lý theo quy tắc.

<a id="m24-t061-s006"></a>

### 3. LLM realtime có cần thiết hay không?

Với yêu cầu hiện tại, **không cần thiết**.

Tôi khuyến nghị bản chính thức sử dụng:

**Authored Content + AI-Assisted Offline Content + Rule-Based Runtime + Template/Grammar-Based Narrative Presentation.**

Có thể nghiên cứu tính năng tạo biến thể câu chữ bằng LLM trong tương lai, nhưng phải là lớp trình bày tùy chọn, có thể tắt hoàn toàn và không phụ thuộc vào gameplay.

Tính năng đó không nằm trên đường xử lý quyết định chính thức.

Nếu LLM gặp lỗi, bị ngắt mạng hoặc không tồn tại trên máy, game vẫn hoạt động đầy đủ.

---

<a id="m24-t061-s007"></a>

## PHẦN II — KIẾN TRÚC TỔNG THỂ

<a id="m24-t061-s008"></a>

### 4. Hai pipeline hoàn toàn khác nhau

Hệ thống nên tách thành:

**Content Production Pipeline** — tạo tài nguyên trước khi phát hành.

**Game Runtime Pipeline** — vận hành thế giới khi player chơi.

<a id="m24-t061-s009"></a>

#### Content Production Pipeline

```text
GAME DESIGN DOCUMENT
         |
         v
Content Specifications
         |
         +-----------------------+
         |                       |
         v                       v
Human Designers          Offline LLM Assistance
         |                       |
         +-----------+-----------+
                     |
                     v
            Draft Content Data
                     |
                     v
           Schema Validation
                     |
                     v
        Semantic / Causal Validation
                     |
                     v
         Human Review & Curation
                     |
                     v
          Simulation Testing
                     |
                     v
          Approved Content Pack
                     |
                     v
           Versioned Game Assets
```

LLM không xuất bản trực tiếp dữ liệu vào runtime.

Một nội dung mới phải đi qua Validation Pipeline.

<a id="m24-t061-s010"></a>

#### Game Runtime Pipeline

```text
PLAYER ACTION
      |
      v
Command Validation
      |
      v
AUTHORITATIVE RULE ENGINE
      |
      +--> World State
      +--> Character State
      +--> Rule Definitions
      +--> Content Templates
      +--> Deterministic RNG
      |
      v
Action Resolution
      |
      v
Domain Events
      |
      v
State Validation
      |
      v
Transactional State Commit
      |
      +--> Event Journal
      +--> Updated World State
      +--> Event Scheduler
      +--> Knowledge Propagation
      +--> Story Thread Update
      |
      v
World Director / Knowledge Filter
      |
      v
Narrative Presentation
      |
      v
GAME UI
```

**World Director không nằm giữa Action Resolution và State Commit để tự sửa kết quả.**

Nó chỉ được sử dụng những kết quả đã xác lập để tổ chức trải nghiệm.

---

<a id="m24-t061-s011"></a>

## PHẦN III — HỆ THỐNG CÓ HÀNG TRIỆU NHÁNH

<a id="m24-t061-s012"></a>

### 5. Không viết hàng triệu nhánh bằng tay

Có hai cách tạo một hệ thống nhiều nhánh.

<a id="m24-t061-s013"></a>

#### Cách thứ nhất — Scripted Branching

```text
IF player_is_smith AND region_is_tam_giang:
    Event A

IF player_is_smith AND region_is_tan_vien:
    Event B

IF player_is_hunter AND region_is_tam_giang:
    Event C
```

Cách này dẫn tới số lượng điều kiện ngày càng lớn, rất khó bảo trì.

Một thay đổi nhỏ trong hệ thống nghề có thể yêu cầu sửa hàng trăm sự kiện.

<a id="m24-t061-s014"></a>

#### Cách thứ hai — Compositional Rule System

Các yếu tố được lưu thành những thuộc tính độc lập có thể kết hợp:

- Region;
- Character Attributes;
- Skills;
- Profession;
- Relationships;
- Household;
- Faction;
- Season;
- Economy;
- World State;
- Story Thread;
- Knowledge;
- Recent Events.

Một Event Template mô tả điều kiện chung và những biến thể được phép.

Engine đánh giá thế giới hiện tại để quyết định những đối tượng nào phù hợp.

Cách thứ hai là kiến trúc cần sử dụng.

<a id="m24-t061-s015"></a>

### 6. Sự bùng nổ tổ hợp

Giả sử hệ thống có:

- 12 vùng;
- 20 nhóm tình huống;
- 8 loại áp lực;
- 10 cấu trúc quan hệ;
- 5 trạng thái kinh tế;
- 11 kiểu kết quả theo ngữ cảnh.

Số tổ hợp lý thuyết đã vượt một triệu.

Nhưng điều quan trọng là:

**Một triệu tổ hợp không đồng nghĩa một triệu tình huống hay.**

Nhiều tổ hợp không hợp lệ.

Nhiều tổ hợp chỉ khác rất ít về nội dung.

Nhiều tổ hợp có thể dẫn tới cùng một kết quả về mặt cơ học.

Vì vậy, hệ thống cần:

1. Cơ chế kết hợp thuộc tính.
2. Bộ lọc điều kiện hợp lệ.
3. Bộ giải quyết xung đột.
4. Bộ xác định kết quả.
5. Bộ chọn cách trình bày.
6. Bộ kiểm tra chất lượng nội dung.

Mục tiêu không phải tạo số nhánh lớn nhất.

Mục tiêu là tạo **nhiều trạng thái khác biệt có ý nghĩa và có thể giải thích được**.

<a id="m24-t061-s016"></a>

### 7. Phân biệt State Space và Authored Content

Cần khóa ba khái niệm.

**State Space:** Tập hợp những trạng thái thế giới có khả năng tồn tại.

**Decision Space:** Những hành động và lựa chọn người chơi hoặc NPC có thể thực hiện từ một trạng thái.

**Narrative Content:** Những nội dung được trình bày để player hiểu các trạng thái và quyết định đó.

State Space có thể cực lớn.

Decision Space được tạo theo các quy tắc hành động.

Narrative Content sử dụng template, biến thể văn bản, mô tả có điều kiện và nội dung authored.

Không cần tạo một đoạn hội thoại riêng cho mọi trạng thái có thể tồn tại.

<a id="m24-t061-s017"></a>

### 8. Branch Compression

Nhiều hành động khác nhau có thể dẫn tới cùng một dạng hệ quả nhưng giữ lại provenance riêng.

Ví dụ ba phương pháp sửa cầu:

- dùng gỗ mới;
- sử dụng vật liệu cũ;
- gia cố tạm.

Cả ba đều có thể tạo trạng thái:

`BridgeOperational = true`.

Nhưng các đặc điểm khác nhau vẫn được lưu:

- độ bền;
- vật liệu tiêu hao;
- người tham gia;
- chất lượng thi công;
- nghĩa vụ thanh toán;
- thời gian duy trì dự kiến.

Nhờ đó không cần viết ba campaign hoàn toàn riêng.

Các event tương lai đọc trực tiếp thuộc tính của cây cầu và người liên quan.

---

<a id="m24-t061-s018"></a>

## PHẦN IV — DETERMINISTIC RANDOMIZATION

<a id="m24-t061-s019"></a>

### 9. Random không đồng nghĩa vô nguyên tắc

Game vẫn cần ngẫu nhiên để tạo:

- nhân vật khác nhau;
- xuất thân khác nhau;
- tính cách khác nhau;
- phân bố năng lực;
- biến thể tình huống;
- những kết quả chưa chắc chắn;
- các diễn biến không được định trước.

Nhưng tất cả những lần random phải nằm trong điều kiện hợp lệ.

Ví dụ một đứa trẻ sinh ra trong gia đình thợ đúc có thể mang các tiềm năng khác nhau.

Tuy nhiên engine không được random nghề nghiệp, tài sản và tri thức theo cách khiến một đứa trẻ mười tuổi lập tức là Master đúc đồng mà không có lịch sử phù hợp.

**Randomization chọn giữa những khả năng hợp lệ. Nó không tạo ra khả năng vô căn cứ.**

<a id="m24-t061-s020"></a>

### 10. Master Seed

Mỗi New Game có một World Seed.

Seed này dùng để khởi tạo các nguồn ngẫu nhiên có kiểm soát.

Tuy nhiên, không nên dùng một RNG toàn cục duy nhất rồi để mọi subsystem liên tục rút số từ đó.

Bởi nếu thêm một lần random nhỏ vào Economy, toàn bộ các kết quả sau đó có thể thay đổi ngoài dự kiến.

Đề xuất sử dụng những **RNG Stream độc lập theo domain**, hoặc một cơ chế random xác định theo khóa.

Ví dụ:

```text
WorldSeed
    |
    +-- CharacterGeneration
    +-- HouseholdGeneration
    +-- EventEligibility
    +-- Combat
    +-- Economy
    +-- Ecology
    +-- NPCDecision
    +-- WorldHistory
```

Việc tách stream giúp một thay đổi ở phần combat ít có khả năng ảnh hưởng tới quá trình sinh NPC ở vùng khác.

<a id="m24-t061-s021"></a>

### 11. Random Context

Với những quyết định quan trọng, random có thể được xác định từ:

```text
RandomKey = Hash(
    WorldSeed,
    RulesetVersion,
    Domain,
    EntityID,
    EventID,
    DecisionType,
    AttemptIndex
)
```

Đây là mô hình đề xuất về mặt kiến trúc, không phải công thức hash cuối cùng.

Ví dụ:

Kết quả một thợ rèn thử nghiệm khuôn phụ thuộc vào:

- world state;
- skill;
- vật liệu;
- công cụ;
- điều kiện;
- quy tắc giải quyết;
- random key tương ứng.

Nếu player load lại cùng trạng thái và thực hiện đúng cùng một hành động trong cùng ruleset, kết quả có thể được tái hiện.

Nhưng nếu player thực sự thay đổi vật liệu, cách chuẩn bị hoặc hoàn cảnh, hệ thống có quyền cho kết quả khác vì đầu vào cơ học đã thay đổi.

<a id="m24-t061-s022"></a>

### 12. Seed không đủ bảo đảm determinism

Đây là một yêu cầu kỹ thuật quan trọng.

Determinism còn cần:

- thuật toán PRNG được khóa;
- phiên bản rule được lưu;
- thứ tự xử lý event ổn định;
- Entity ID ổn định;
- thứ tự duyệt collection ổn định;
- xử lý số học nhất quán;
- time step được xác định;
- quy tắc giải quyết đồng thời;
- không sử dụng thời gian máy để quyết định kết quả gameplay sau khi world đã khởi tạo.

Nếu cần bảo đảm replay nhất quán trên nhiều hệ điều hành, nên ưu tiên những phép tính số nguyên hoặc fixed-point cho các quy tắc quan trọng, đồng thời có test xác nhận kết quả trên các nền tảng mục tiêu.

Không nên coi việc cùng dùng một seed là đã giải quyết xong bài toán.

<a id="m24-t061-s023"></a>

### 13. Công thức bí mật với player, minh bạch với developer

Có thể giữ bí mật các dữ liệu như:

- trọng số sinh sự kiện;
- tiềm năng ẩn;
- điều kiện một Class Emergence;
- một số ngưỡng phản ứng NPC;
- phân bố biến thể khi sinh character.

Nhưng đội phát triển phải truy vết được:

- rule nào đã chạy;
- tham số nào được sử dụng;
- random value nào đã được lấy;
- điều kiện nào đạt hoặc không đạt;
- kết quả được ghi vào đâu.

Không nên coi việc giấu công thức là một cơ chế bảo mật thực sự: game chạy trên máy người chơi vẫn có thể bị phân tích hoặc sửa đổi.

Mục tiêu của công thức ẩn là bảo vệ trải nghiệm khám phá, không phải che các lỗi thiết kế hoặc ngăn người chơi hiểu tại sao một kết quả xảy ra.

---

<a id="m24-t061-s024"></a>

## PHẦN V — CONTENT DATA MODEL

<a id="m24-t061-s025"></a>

### 14. Những dữ liệu nào cần được định nghĩa trước?

Đề xuất phân thành các nhóm.

| Nhóm nội dung | Vai trò | Nguồn tạo |
|---|---|---|
| Region Definition | Địa lý, môi trường, văn hóa, tài nguyên | Designer + nghiên cứu |
| Settlement Archetype | Cấu trúc cộng đồng, kinh tế và môi trường | Designer + LLM hỗ trợ |
| Household Archetype | Các dạng gia đình và quan hệ | Designer + LLM hỗ trợ |
| Character Archetype | Personality, background, profession distribution | Designer + LLM hỗ trợ |
| Event Template | Trigger, participants, options, outcomes | Designer + LLM hỗ trợ |
| Dialogue Template | Lời nói gắn với intent và state | Writer + LLM hỗ trợ |
| Quest/Story Template | Cấu trúc vấn đề và những cách can thiệp | Narrative Designer |
| Myth/Ritual Content | Nội dung tín ngưỡng và game-canon | Researcher + Writer |
| Historical Anchor | Các biến cố lõi của campaign | Lead Narrative Designer |
| Ruleset | Công thức và quy tắc cập nhật trạng thái | Game/System Designer + Programmer |

LLM có thể đề xuất hoặc soạn bản nháp cho nhiều nhóm.

Nhưng **Ruleset và Historical Anchor có quyền phê duyệt cao nhất**, không được tự động thay đổi vì nội dung do LLM mới sinh có ý tưởng hấp dẫn.

<a id="m24-t061-s026"></a>

### 15. Content Pack

Nội dung sau khi được duyệt sẽ đóng thành Content Pack.

Ví dụ:

```text
content/
    regions/
    settlements/
    households/
    character_archetypes/
    professions/
    classes/
    techniques/
    event_templates/
    dialogue_templates/
    quest_templates/
    factions/
    creatures/
    rituals/
    myths/
    historical_anchors/
```

Mỗi content entity phải có:

- stable ID;
- schema version;
- tags;
- allowed conditions;
- dependency references;
- historical/game-canon classification;
- authoring metadata;
- trạng thái phê duyệt.

Không cho phép runtime đọc một file văn bản do LLM tạo ra rồi tự hiểu nó thành quy tắc mới.

---

<a id="m24-t061-s027"></a>

## PHẦN VI — CHARACTER GENERATION ENGINE

<a id="m24-t061-s028"></a>

### 16. Character Generation phải do engine quyết định

Tại New Game, code thực hiện:

**World Seed → Region → Settlement → Household → Lineage/Family → Individual → Childhood → Current Age → Life History → Attributes/Skills/Knowledge → Viability Validation → Playable Character.**

Mỗi bước tạo ra những ràng buộc cho bước kế tiếp.

Ví dụ một character sinh trong household chuyên nghề vận tải:

- có khả năng tiếp xúc với thuyền;
- dễ có network nghề sông;
- có thể học một số kỹ năng liên quan từ nhỏ;
- có thể mang trách nhiệm với household.

Nhưng không có nghĩa mọi người trong gia đình đều trở thành Thủy Binh.

<a id="m24-t061-s029"></a>

### 17. Hai lớp sinh nhân vật

<a id="m24-t061-s030"></a>

#### Structural Generation

Do code quyết định:

- tuổi;
- quan hệ;
- gia đình;
- nơi sinh;
- điều kiện sống;
- thuộc tính;
- kỹ năng;
- nghề;
- các sự kiện cuộc đời;
- tính cách và động cơ;
- trạng thái hiện tại.

<a id="m24-t061-s031"></a>

#### Narrative Realization

Từ cấu trúc đã xác lập, hệ thống chọn:

- tên phù hợp;
- cách diễn đạt lý lịch;
- các câu giới thiệu;
- chi tiết mô tả;
- giọng nói hoặc đặc điểm hành vi theo content tags.

Narrative Realization không được sửa dữ liệu cấu trúc.

Ví dụ một character có lý lịch:

> Mất cha vì tai nạn trên sông lúc 12 tuổi.

Content Layer có thể trình bày sự kiện này theo văn phong phù hợp.

Nhưng không được tự chuyển thành:

> Cha bị một faction giết, tạo nên mối thù bí mật.

Trừ khi World Event và Family History đã có sự kiện đó.

<a id="m24-t061-s032"></a>

### 18. Life History Generator

Một nhân vật trưởng thành cần một lịch sử đủ sức giải thích tình trạng hiện tại.

Generator có thể xây chuỗi:

**Birth → Family Environment → Childhood Events → Education/Work Exposure → Major Life Events → Current Occupation → Current Relationships.**

Các event phải kết nối nhau.

Ví dụ:

Một người 43 tuổi có Skill Đúc Khuôn cao.

Engine cần tạo được những dữ liệu hợp lý như:

- từng học từ ai;
- thời gian thực hành;
- workshop đã làm;
- những milestone có ý nghĩa;
- nghề hiện tại.

Không nhất thiết tạo đủ 43 năm sự kiện chi tiết.

Nhưng phải có các **causal milestones** đủ giải thích năng lực và quan hệ hiện tại.

<a id="m24-t061-s033"></a>

### 19. Character Viability

Đối với protagonist, quy tắc **không reroll theo lựa chọn người chơi** vẫn giữ nguyên.

Generator phải sinh nhân vật trong một không gian điều kiện hợp lệ.

Nếu một cấu hình dẫn tới character không thể chơi, hệ thống xử lý tại bước constraint sampling hoặc validation theo quy trình có giới hạn và có thể tái hiện.

Điều này không phải người chơi được bốc lại character cho tới khi có build tốt.

Một nhân vật nghèo, già, bị thương hoặc thiếu kỹ năng chiến đấu vẫn có thể hợp lệ nếu có con đường sống và phát triển hợp lý.

Viability không đồng nghĩa với tối ưu hóa sức mạnh.

<a id="m24-t061-s034"></a>

### 20. NPC Generation theo Simulation Tier

Để phù hợp quy mô đã thiết kế:

**Tier S:** Khoảng 100–150 NPC có vai trò narrative đặc biệt. Cần biên tập kỹ, liên kết campaign và kiểm tra lịch sử sâu.

**Tier A:** Khoảng 800–1.000 NPC có identity và quan hệ nổi bật. Có thể kết hợp dữ liệu authored, content template và sinh tự động được kiểm tra.

**Tier B:** Khoảng 3.000–5.000 NPC phục vụ Household, Profession và Community Simulation. Sử dụng rule-based generation với dữ liệu đủ để nâng cấp khi cần.

**Tier C:** Population aggregate, chỉ instantiate cá thể khi có lý do.

Những quy mô này là mục tiêu thiết kế hiện có, không phải yêu cầu phải instantiate toàn bộ đồng thời khi runtime bắt đầu.

Đề xuất sử dụng lazy materialization kết hợp persistent seeds và background population state, miễn sao việc sinh chi tiết về sau không mâu thuẫn lịch sử đã ghi.

---

<a id="m24-t061-s035"></a>

## PHẦN VII — EVENT GENERATION ENGINE

<a id="m24-t061-s036"></a>

### 21. Event có cấu trúc, không phải đoạn truyện ngẫu nhiên

Mỗi Event Template phải định nghĩa:

1. Nguồn gốc hoặc áp lực.
2. Điều kiện kích hoạt.
3. Những actor hợp lệ.
4. Địa điểm và thời điểm hợp lệ.
5. Mục tiêu của actor.
6. Những lựa chọn hoặc hành động có thể có.
7. Logic kết quả.
8. State Changes.
9. Follow-up Events.
10. Quy tắc hiển thị thông tin.

Nội dung hiển thị chỉ được tạo sau khi engine xác định những phần này.

<a id="m24-t061-s037"></a>

### 22. Event Template mẫu

Ví dụ một sự kiện tranh chấp lương thực:

```json
{
  "event_id": "food_dispute_001",
  "version": 1,
  "category": "household_conflict",
  "scope": "settlement",
  "triggers": [
    {
      "type": "resource_pressure",
      "resource": "food",
      "operator": "below",
      "threshold_ref": "food.shortage_threshold"
    }
  ],
  "participants": {
    "requester": {
      "role": "household_head",
      "requirements": [
        "alive",
        "resident_of_target_settlement"
      ]
    },
    "opposing_household": {
      "role": "household",
      "requirements": [
        "has_food_stock",
        "distinct_from_requester"
      ]
    }
  },
  "resolution_rules": [
    "negotiate_food_transfer",
    "provide_external_supply",
    "invoke_local_custom",
    "refuse_intervention"
  ],
  "state_effects": [
    "resource_transfer",
    "obligation_update",
    "relationship_update",
    "possible_followup_thread"
  ]
}
```

Đây là mẫu cấu trúc khai báo.

Các chuỗi `resolution_rules` và `state_effects` là ID tham chiếu đến những handler đã được code và kiểm thử.

Không phải những câu lệnh tự do mà LLM hoặc file JSON có thể bắt engine thi hành.

<a id="m24-t061-s038"></a>

### 23. Event Eligibility

Một template được xem là đủ điều kiện khi:

- áp lực tồn tại;
- actor phù hợp tồn tại;
- actor có điều kiện tham gia;
- location hợp lệ;
- tài nguyên hoặc yếu tố liên quan có thật;
- thời điểm nằm trong cửa sổ hợp lệ;
- các điều kiện chính trị/lịch sử không mâu thuẫn;
- không vi phạm các bất biến của thế giới.

Nếu thiếu một người có quyền sở hữu kho, event không được tự bịa người ấy.

Nếu kho đã bị phá, event không được sử dụng hàng hóa trong kho.

Nếu requester đã chết, event phải chọn một vai trò hợp lệ khác hoặc trở nên không còn khả thi.

<a id="m24-t061-s039"></a>

### 24. Event Selection

Không phải mọi event đủ điều kiện đều xảy ra.

Engine có thể chọn từ candidate pool bằng:

- pressure;
- probability;
- actor initiative;
- season;
- resource availability;
- recent event history;
- constraints;
- deterministic RNG.

Các event bắt buộc vì hậu quả đã xác lập phải đi qua Event Scheduler.

Các event tùy chọn có thể được chọn hoặc không.

World Director chỉ quyết định mức độ phơi bày và cơ hội tương tác theo giới hạn đã khóa ở Module 22.

<a id="m24-t061-s040"></a>

### 25. Event Resolution

Một Event Instance cần lưu:

```text
EventInstance
    EventID
    TemplateID
    RulesetVersion
    OriginEventID
    TriggerContext
    Participants
    Location
    Time
    InitialState
    Player/ActorCommands
    RNGReferences
    Outcome
    StateChanges
    FollowUpEventIDs
```

Đây là nền để truy ngược nhân quả.

Một event không chỉ lưu:

> “Cầu đã hỏng.”

Mà cần biết:

> “Cầu bị hỏng sau khi mực nước vượt điều kiện chịu tải, dẫn tới Structural Failure, đóng tuyến vận tải và tạo nhu cầu sửa chữa.”

---

<a id="m24-t061-s041"></a>

## PHẦN VIII — NPC DECISION ENGINE

<a id="m24-t061-s042"></a>

### 26. NPC không được hành động theo lời LLM realtime

NPC có thể có hành vi phức tạp mà không dùng mô hình ngôn ngữ.

Đề xuất kết hợp:

**Goal-Based Planning** cho những mục tiêu dài hạn.

**Utility AI** để lựa chọn giữa các hành động hợp lệ.

**Behavior Tree hoặc State Machine** cho quy trình hành động cụ thể.

**Social Rules** để xử lý ranh giới, lời hứa và quan hệ.

**Deterministic RNG** cho những lựa chọn có yếu tố bất định.

Không bắt buộc sử dụng tất cả cho mọi NPC.

Tier C không cần một bộ planner chi tiết như Tier S.

<a id="m24-t061-s043"></a>

### 27. Mô hình hành động NPC

Một NPC có:

- nhu cầu hiện tại;
- mục tiêu dài hạn;
- nghề nghiệp;
- nguồn lực;
- nghĩa vụ;
- quan hệ;
- personality;
- tri thức riêng;
- moral boundary.

Engine tạo danh sách các hành động hợp lệ.

Sau đó đánh giá mức phù hợp của từng hành động.

Ví dụ một thợ đúc đang thiếu lương thực có thể:

- tiếp tục công việc;
- thương lượng tiền công;
- bán một vật dụng;
- vay người thân;
- tìm nguồn hàng;
- tạm ngừng sản xuất.

Những hành động này được quyết định dựa trên điều kiện thực tế.

Không phải LLM sinh bất ngờ:

> “Người thợ đúc quyết định làm phản rồi đốt cả làng.”

<a id="m24-t061-s044"></a>

### 28. Utility Score

Một dạng tổng quát có thể là:

```text
Utility(action) =
    NeedFit
  + GoalFit
  + RelationshipFit
  + ProfessionFit
  + ExpectedBenefit
  - ExpectedCost
  - Risk
  - ObligationConflict
```

Đây là biểu diễn khái niệm, không phải công thức balance cuối.

Moral Boundary và hard preconditions phải được xử lý trước hoặc ngoài Utility Score.

Không được để một lợi ích đủ cao khiến NPC tự động vượt qua ranh giới đạo đức mà character chưa có diễn biến đủ hợp lý.

<a id="m24-t061-s045"></a>

### 29. NPC không có thông tin toàn tri

Planner chỉ được sử dụng:

**NPC Knowledge + NPC Belief + Perceived World State.**

Không sử dụng hidden truth để tối ưu quyết định.

Ví dụ một thủ lĩnh tin có quân địch ở phía bắc nhưng thực tế quân đã rút.

Nếu chưa nhận tin mới, quyết định điều quân của ông phải dựa trên thông tin cũ.

Chính việc NPC có thể hành động sai từ thông tin sai tạo ra narrative có chiều sâu.

---

<a id="m24-t061-s046"></a>

## PHẦN IX — SOCIAL, DIALOGUE & KNOWLEDGE ENGINE

<a id="m24-t061-s047"></a>

### 30. Dialogue Logic do code điều khiển

Một cuộc hội thoại được giải thành:

**Speaker Intent + Available Knowledge + Relationship + Context + Social State → Eligible Dialogue Acts.**

Engine xác định:

- NPC có biết thông tin không;
- NPC có muốn nói không;
- có bị ràng buộc bởi lời hứa không;
- player có authority không;
- một yêu cầu có vượt Moral Boundary không;
- một cuộc thương lượng có vùng thỏa thuận không.

Sau đó Presentation Layer chọn câu chữ thích hợp.

<a id="m24-t061-s048"></a>

### 31. Authored Dialogue Grammar

Để tránh phụ thuộc LLM runtime, sử dụng **Dialogue Grammar**.

Ví dụ một NPC từ chối đưa thuyền có thể có một Dialogue Act:

```text
Intent: RefuseLoan
Reason: FamilyLivelihood
Emotion: Concern
Relationship: Familiar
```

Hệ thống chọn từ các câu thoại đã được authored/LLM hỗ trợ tạo offline và gắn tags.

Ví dụ:

> “Ta muốn giúp ngươi, nhưng nhà ta còn phải dùng thuyền ngày mai.”

Hoặc:

> “Không phải ta không tin ngươi. Nếu thuyền hỏng, cả nhà ta sẽ không có kế sinh nhai.”

Những biến thể này cùng truyền đạt một ý định xã hội.

Chúng không thay đổi trạng thái hội thoại chỉ vì câu chữ khác nhau.

<a id="m24-t061-s049"></a>

### 32. Dynamic Dialogue

Không thể viết sẵn mọi câu có tên nhân vật, địa điểm, vật phẩm và thời gian.

Vì vậy Dialogue System cần hỗ trợ các biến được kiểm soát:

```text
"Tôi đã gặp {person_name}
ở {known_location}
vào {known_time}."
```

Các biến được engine lấy từ dữ liệu hợp lệ.

Không được cho LLM tự chọn một người, địa điểm hoặc thời điểm mới.

Đối với tiếng Việt, template phải hỗ trợ biến thể xưng hô, vai vế, giới, tuổi, quan hệ và ngữ cảnh văn hóa; không chỉ thay chuỗi tên đơn giản rồi giả định câu văn luôn tự nhiên.

<a id="m24-t061-s050"></a>

### 33. Knowledge Projection

Khi một NPC nói, engine xác định:

- claim được phép truyền;
- source;
- tình trạng biết/tin;
- độ chắc chắn;
- thái độ người nói;
- khả năng nói dối có chủ đích.

Nếu NPC nói một điều sai:

đó phải xuất phát từ Belief hoặc Deception State.

Không phải lỗi LLM hallucination.

Nhờ đó investigation và rumor giữ được sự phân biệt giữa:

**Truth, Belief, Claim và Public Story.**

---

<a id="m24-t061-s051"></a>

## PHẦN X — WORLD SIMULATION & DOMAIN RULES

<a id="m24-t061-s052"></a>

### 34. Các hệ thống không được cập nhật state tùy ý của nhau

Đề xuất chia thành các domain:

**Character & Life:** Attributes, Skills, Aging, Health, Death.

**Social & Household:** Relationships, Family, Debt, Promise.

**Economy & Settlement:** Production, Consumption, Inventory, Infrastructure.

**Faction & Politics:** Membership, Influence, Leadership, Conflict.

**Travel & Ecology:** Route, Weather, Habitat, Population.

**Narrative & History:** Events, Story Threads, Historical Anchors, Chronicle.

**Belief & Mạch:** Ritual, Mạch State, Memory and Myth.

Mỗi domain có Rule Engine/Handler chịu trách nhiệm về những dữ liệu mình sở hữu.

Ví dụ Combat không trực tiếp cập nhật bảng Reputation.

Combat phát sự kiện:

`PersonKilled`, `WitnessObservedViolence` hoặc `PrisonerReleased`.

Social/Knowledge Systems xử lý thông tin và danh tiếng khi sự kiện đó được biết.

<a id="m24-t061-s053"></a>

### 35. Domain Events

Đề xuất sử dụng những event có ý nghĩa nghiệp vụ game:

```text
CharacterInjured
CharacterDied
HouseholdMemberBorn
FoodStockReduced
BridgeDamaged
BridgeRepaired
PromiseCreated
PromiseBroken
FactionLeaderChanged
TechniqueTransmitted
NPCWitnessedEvent
RumorPropagated
StoryThreadResolved
```

Không nên chỉ ghi những log thay đổi field:

```text
npc.hp = 0
bridge.condition = 0.2
relationship.trust = -3
```

Các event có tên và nguyên nhân giúp game giải thích tại sao thế giới thay đổi.

<a id="m24-t061-s054"></a>

### 36. Transaction & Consistency Boundary

Một hành động có thể tác động nhiều domain.

Ví dụ player giao năm đơn vị lương thực để đổi quyền dùng thuyền.

Hệ thống cần bảo đảm:

- bên giao còn đủ lương thực;
- bên nhận có quyền nhận;
- hàng được chuyển một lần;
- Agreement được tạo;
- quyền sử dụng thuyền được ghi nhận;
- event liên quan được commit nhất quán.

Không được xảy ra:

lương thực bị trừ nhưng quyền mượn thuyền không có, hoặc một vật phẩm đồng thời ở hai inventory.

Nếu một giao dịch không thể xác nhận đầy đủ, engine phải từ chối hoặc thực hiện cơ chế xử lý lỗi đã định nghĩa.

Không được nhờ LLM viết thêm câu chuyện để che một trạng thái dữ liệu lỗi.

---

<a id="m24-t061-s055"></a>

## PHẦN XI — EVENT JOURNAL, SNAPSHOT & SAVE

<a id="m24-t061-s056"></a>

### 37. Chọn kiến trúc Hybrid State + Event Journal

Game này cần:

- World State hiện tại để gameplay nhanh;
- lịch sử event để tái hiện và giải thích;
- Snapshot để lưu/tải;
- Chronicle để trình bày.

Tôi đề xuất:

**Authoritative Current State + Versioned Critical Event Journal + Periodic Snapshots + Rebuildable Read Projections.**

Không bắt buộc mọi chỉ số nhỏ của hàng nghìn NPC đều trở thành một event-sourced aggregate đầy đủ.

Việc lưu event quá chi tiết có thể làm tăng đáng kể chi phí thiết kế, dung lượng lưu và bảo trì.

Những thay đổi quan trọng về lịch sử, sở hữu, quan hệ, lời hứa, cái chết, Story Thread và tài nguyên lớn cần được lưu với độ tin cậy cao.

Những trạng thái aggregate thường lệ có thể được lưu bằng snapshot và bản ghi thay đổi tổng hợp đủ để tái hiện.

<a id="m24-t061-s057"></a>

### 38. Save Data cần gì?

Một Save File/Save Database cần tham chiếu:

```text
SaveMetadata
    WorldSeed
    RulesetVersion
    ContentPackVersion
    SimulationVersion
    WorldTime
    ActiveCharacterID
    CurrentWorldSnapshot
    RNGStatesOrCounters
    ScheduledEvents
    CommittedEventJournal
    KnowledgeState
    StoryThreadState
    HistoricalAnchorState
```

Không chỉ lưu position và inventory.

Đối với game kéo dài 25 năm, sự nhất quán lịch sử là thành phần của save.

<a id="m24-t061-s058"></a>

### 39. Versioning & Migration

Nếu patch thay đổi một công thức, hai tình huống cần được phân biệt:

**Save cũ tiếp tục chơi:** Cần migration policy để bảo toàn thế giới và lịch sử đã xác lập.

**Debug replay:** Cần sử dụng đúng ruleset/content version để tái hiện sự kiện ban đầu.

Không thể đảm bảo cùng một seed tái hiện toàn bộ campaign nếu code hoặc dữ liệu gốc đã thay đổi mà không có versioning.

Các bản cập nhật không nên tự diễn giải lại cái chết, tuổi tác hay kết quả điều tra đã lưu bằng một công thức mới.

<a id="m24-t061-s059"></a>

### 40. Historical Event Audit

Một event lớn có thể có bản ghi:

```text
Event: BridgeCollapse
Time: Year 8, Day 104
Location: BenHac
Cause: FloodPressureExceeded
Rule: BridgeStructuralFailure.v3
AffectedEntities:
  - bridge_benhac
  - route_benhac
  - household_trieu
FollowUp:
  - TradeRouteDisruption
  - RepairNeedCreated
```

Nhờ đó developer có thể hiểu:

- nguyên nhân gốc;
- quy tắc;
- kết quả;
- hậu quả;
- những nơi cần kiểm tra khi có bug.

---

<a id="m24-t061-s060"></a>

## PHẦN XII — WORLD DIRECTOR KHÔNG NẮM QUYỀN SINH SỰ THẬT

<a id="m24-t061-s061"></a>

### 41. World Director chỉ điều phối exposure và pacing

Quyết định đã khóa từ Module 22 tiếp tục áp dụng.

World Director có thể:

- chọn Story Thread nào được giới thiệu qua một nguồn hợp lệ;
- chọn thời điểm trình bày một tin tức;
- ưu tiên một số optional encounter đủ điều kiện;
- tránh spam thông báo;
- tóm tắt background events.

World Director không được:

- tự tạo hung thủ;
- quyết định random một NPC phải chết;
- làm cầu hỏng để player có việc;
- điều chỉnh tỷ lệ thành công craft;
- tạo faction mới không có nguồn;
- thay đổi lịch sử chỉ để tạo drama.

Nếu cần tạo một biến cố mới, nó phải được thực hiện bởi Event Engine với trigger, actor, tài nguyên và điều kiện đầy đủ.

<a id="m24-t061-s062"></a>

### 42. Historical Anchor Controller là hệ riêng

Historical Anchor Controller có quyền giới hạn campaign theo những sự kiện lõi đã authored.

Nhưng không được “gian lận” bằng cách sinh vật liệu hoặc người mới từ hư không.

Tốt nhất các Anchor phải được xây với nhiều kịch bản nhân quả hợp lệ.

Ví dụ Cổ Loa có thể được xây với những mức độ:

- tiến độ khác nhau;
- tổ chức lao động khác nhau;
- mức thiệt hại khác nhau;
- mạng nghề nghiệp khác nhau;
- quan hệ chính trị khác nhau.

Nếu một kịch bản làm cho sự kiện lõi bất khả thi, đó là lỗ hổng thiết kế campaign cần được phát hiện trong kiểm thử và sửa bằng logic kịch bản, không phải bằng cách trao quyền ứng biến vô hạn cho Director hoặc LLM.

---

<a id="m24-t061-s063"></a>

## PHẦN XIII — LLM-ASSISTED CONTENT GENERATION

<a id="m24-t061-s064"></a>

### 43. LLM nên được sử dụng ở đâu?

LLM đặc biệt phù hợp với những việc cần độ đa dạng ngôn ngữ hoặc gợi ý sáng tạo:

- bản nháp biography;
- personality contradiction;
- các dạng conflict;
- quan hệ gia đình;
- lời kể NPC;
- biến thể hội thoại;
- motif địa phương;
- sự kiện nghề nghiệp;
- ý tưởng quest;
- phiên bản rumor;
- câu chuyện tổ tiên;
- mô tả địa điểm;
- diễn đạt Chronicle.

Tuy nhiên, đầu ra phải thuộc một **content schema xác định trước**.

Ví dụ một Event Idea được yêu cầu trả lời:

- Trigger;
- Participants;
- Goals;
- Constraints;
- Preconditions;
- Actions;
- Outcome Types;
- World Consequences.

Nếu chỉ trả về một đoạn văn hấp dẫn mà không thể chuyển thành dữ liệu kiểm chứng được, nó chưa phải Event Template có thể triển khai.

<a id="m24-t061-s065"></a>

### 44. Offline Generation Workflow

Một quy trình sản xuất nội dung chuẩn:

**Bước 1 — Content Request:** Designer xác định loại nội dung cần bổ sung và các ràng buộc.

**Bước 2 — Context Package:** Hệ thống cung cấp cho LLM những dữ liệu đã khóa: vùng, nghề, sự kiện lịch sử, ruleset và nội dung cần tránh trùng lặp.

**Bước 3 — Structured Draft:** LLM sinh nội dung theo schema.

**Bước 4 — Syntax & Schema Validation:** Kiểm tra cấu trúc, ID, reference và field bắt buộc.

**Bước 5 — Semantic Validation:** Kiểm tra nội dung có hợp lý về vùng, nghề, thời gian, quan hệ và lịch sử hay không.

**Bước 6 — Causal Validation:** Kiểm tra precondition, available actions, outcome và hậu quả.

**Bước 7 — Similarity & Diversity Review:** Kiểm tra nội dung lặp, đổi tên nhưng cùng cấu trúc, hoặc bị áp cùng một motif quá thường xuyên.

**Bước 8 — Human Approval:** Nội dung quan trọng được biên tập và phê duyệt.

**Bước 9 — Simulation Tests:** Cho event chạy thử trên nhiều trạng thái thế giới.

**Bước 10 — Content Pack Release:** Đóng gói và version hóa.

Không được bỏ các bước kiểm tra chỉ vì LLM trả về JSON đúng cú pháp.

<a id="m24-t061-s066"></a>

### 45. Một ví dụ sinh event bằng LLM offline

Designer yêu cầu:

> Tạo một Event Template cho người thợ đúc ở Đồng Đỏ gặp vấn đề với nguồn nhiên liệu, cho phép giải bằng ít nhất ba hướng, có hậu quả kinh tế và xã hội.

LLM tạo ý tưởng:

- mỏ nhiên liệu bị ngập;
- người vận tải không thể tới;
- một faction đề nghị bán nhiên liệu với điều kiện đổi quyền cung ứng;
- player có thể tìm nguồn khác, thương lượng hoặc giảm sản lượng.

Sau đó engine/content validator kiểm tra:

- vùng có nguồn nhiên liệu phù hợp không;
- tuyến vận tải có tồn tại không;
- faction có khả năng cung ứng không;
- quan hệ và quyền sở hữu hợp lệ không;
- các state effects đã có handler chưa.

Nếu faction không có hàng hóa tương ứng, template không được tự cho họ năng lực cung ứng mới.

Designer có thể sửa template hoặc loại bỏ phương án đó.

<a id="m24-t061-s067"></a>

### 46. Độ sâu nhân vật do LLM hỗ trợ nhưng dữ liệu phải hợp lệ

LLM có thể đề xuất:

> “Một người thợ giỏi nhưng không muốn truyền bí quyết cho con vì sợ mất nghề.”

Ý tưởng này có giá trị.

Nhưng engine phải chuyển nó thành những biến có thể vận hành:

- nghề nghiệp;
- Skill;
- quan hệ cha–con;
- Fear;
- Professional Identity;
- Knowledge Ownership;
- Transfer Willingness;
- Recent Events.

Nếu chỉ lưu một đoạn văn biography, NPC Decision Engine sẽ không thể vận hành nhất quán từ nó.

Narrative text là cách thể hiện.

**Structured Character Model mới là dữ liệu điều khiển hành vi.**

---

<a id="m24-t061-s068"></a>

## PHẦN XIV — BẢO VỆ CHẤT LƯỢNG NỘI DUNG

<a id="m24-t061-s069"></a>

### 47. Causal Validation

Mỗi Event Template cần vượt qua kiểm tra:

- nguyên nhân có tồn tại;
- actor có động cơ;
- yêu cầu nguồn lực phù hợp;
- hành động có thể thực hiện;
- hậu quả liên kết với hành động;
- không vi phạm Historical Anchor;
- không tự tạo knowledge;
- có hướng xử lý khi actor chết hoặc rời địa điểm;
- có kết quả nếu player không can thiệp.

Nếu không có điều kiện cuối, event có nguy cơ trở thành quest đứng chờ player vô hạn.

<a id="m24-t061-s070"></a>

### 48. Narrative Validation

Ngoài logic, nội dung phải đạt chất lượng câu chuyện.

Cần kiểm tra:

- nhân vật có contradiction phù hợp không;
- các phía có mục tiêu riêng không;
- sự kiện có meaningful choice không;
- có quá giống một template trước không;
- có tạo quá nhiều bi kịch vô cớ không;
- có thể dẫn tới một thay đổi lâu dài không;
- player có thể hiểu tình huống qua UI không.

Một event hoàn toàn hợp lệ về mặt code vẫn có thể là nội dung nhàm chán.

<a id="m24-t061-s071"></a>

### 49. Historical & Cultural Validation

Mọi nội dung về tín ngưỡng, nghề, vật liệu, động vật hoặc tổ chức xã hội cần phân loại:

- có căn cứ lịch sử/khảo cổ;
- giả thuyết hợp lý;
- truyền thuyết;
- game-canon.

Một LLM có thể trộn khái niệm nhiều thời kỳ, tạo thiết chế hiện đại hoặc dùng tên gọi chưa phù hợp.

Validator không nên coi việc văn bản nghe cổ kính là bằng chứng về tính xác thực.

Với nội dung quan trọng, vẫn cần người nghiên cứu và biên tập kiểm tra.

<a id="m24-t061-s072"></a>

### 50. Content Provenance

Mỗi nội dung nên lưu:

```text
ContentID
ContentVersion
OriginalDesignRequest
GeneratedBy
ReviewedBy
ResearchClassification
ValidationStatus
Dependencies
RevisionHistory
```

Thông tin này phục vụ team phát triển, không cần hiển thị trong gameplay.

Khi một template gây lỗi, team có thể xác định nó từ đâu đến và đã được kiểm thử với ruleset nào.

---

<a id="m24-t061-s073"></a>

## PHẦN XV — PERFORMANCE & SCALABILITY

<a id="m24-t061-s074"></a>

### 51. Không mô phỏng chi tiết mọi NPC mọi thời điểm

Với quy mô NPC và settlement đã đề xuất, một cách tiếp cận naïve sẽ tốn tài nguyên rất lớn.

Nếu mỗi NPC tự đánh giá mọi mục tiêu, mọi lời hứa, mọi quan hệ và mọi action mỗi frame, hệ thống sẽ không khả thi về chi phí.

Đề xuất sử dụng **Simulation LOD — Level of Detail**.

**Active Tier:** NPC đang hiện diện hoặc ảnh hưởng trực tiếp tới player được xử lý chi tiết.

**Relevant Tier:** Gia đình, companion ở xa và các faction liên quan được xử lý theo event và các khoảng thời gian phù hợp.

**Background Tier:** NPC và household ít liên quan được xử lý qua lịch sinh hoạt, trạng thái tổng hợp và event sampling có điều kiện.

**Aggregate Tier:** Các nhóm dân cư chưa cần cá thể hóa được xử lý bằng thống kê.

<a id="m24-t061-s075"></a>

### 52. Event-Driven Simulation thay vì Polling toàn bộ

Không cần mỗi ngày quét mọi NPC để hỏi:

> “Bạn có muốn kết hôn không?”

Có thể đăng ký hoặc tính toán eligibility khi:

- nhân vật tới giai đoạn phù hợp;
- một quan hệ phát triển;
- household thay đổi;
- một event xã hội xảy ra;
- một time interval có liên quan kết thúc.

Cách tiếp cận này giảm số lần tính toán vô ích.

Tương tự:

Một hợp đồng chỉ cần được kiểm tra khi tới hạn hoặc có event ảnh hưởng.

Một lời hứa không cần được đánh giá lại mỗi giây.

<a id="m24-t061-s076"></a>

### 53. Caching & Indexing

Các truy vấn thường gặp:

- NPC nào trong settlement đang thiếu food;
- household nào có nghề thợ mộc;
- ai biết một rumor;
- ai có quan hệ với player;
- faction nào kiểm soát route;
- event nào đủ điều kiện trong vùng.

Cần thiết kế các index và cache phù hợp.

Không nên để Event Engine mỗi lần tạo candidate đều duyệt toàn bộ World State.

Các cache phục vụ truy vấn phải có cơ chế invalidation khi World State thay đổi.

<a id="m24-t061-s077"></a>

### 54. LOD không được thay đổi lịch sử đã xác lập

Ví dụ khi player rời Bến Vạc, NPC có thể chuyển sang Relevant Simulation.

Sau năm năm quay lại, engine có thể tạo chi tiết từ những trạng thái đã lưu.

Nhưng không được:

- làm trẻ em đổi cha mẹ;
- xóa một lời hứa đã có;
- tạo lại một NPC đã chết;
- cho tài sản trở lại chủ cũ;
- xóa vết thương hoặc quan hệ quan trọng.

LOD thay độ chi tiết mô phỏng, không được thay sự thật.

---

<a id="m24-t061-s078"></a>

## PHẦN XVI — KIỂM THỬ HỆ THỐNG

<a id="m24-t061-s079"></a>

### 55. Unit Test cho Rule Engine

Các handler quan trọng cần được kiểm tra độc lập.

Ví dụ:

**Combat:** Đòn đánh và Defense được xử lý theo đúng công thức; thương tích hợp lệ; không tạo vật phẩm hay trạng thái trái quy tắc.

**Economy:** Resource transfer bảo toàn số lượng; không có inventory duplication; giá được tính từ dữ liệu hợp lệ.

**Social:** Moral Boundary có hiệu lực; promise được ghi đúng chủ thể; knowledge không tự lan.

**Character:** Skill phát triển từ điều kiện hợp lệ; tuổi và life history nhất quán.

**Death:** Không có character chết vẫn tham gia work/combat như đang sống.

Các test cần bao gồm trường hợp biên, không chỉ luồng thành công thông thường.

<a id="m24-t061-s080"></a>

### 56. Deterministic Replay Test

Quy trình:

1. Tạo World Seed.
2. Chạy một chuỗi command cố định.
3. Ghi World State Hash và Event Journal.
4. Khởi tạo lại với cùng version.
5. Chạy lại chính chuỗi command.
6. So sánh kết quả.

Nếu kết quả khác, cần xác định nguồn nondeterminism.

Một số sai lệch Presentation-only có thể được bỏ qua nếu không làm thay đổi state.

Nhưng gameplay state, RNG decision và các event quan trọng phải tái hiện nhất quán trong phạm vi hỗ trợ đã xác định.

<a id="m24-t061-s081"></a>

### 57. Property-Based Testing

Ngoài test case viết tay, cần tự sinh nhiều trạng thái đầu vào để tìm lỗi bất biến.

Ví dụ:

- Character chết không thể có trạng thái Alive sau commit.
- Một vật phẩm không thể cùng lúc có hai chủ sở hữu độc quyền.
- Food Stock không được âm khi không có cơ chế ghi nợ tài nguyên hợp lệ.
- Successor không tự thừa hưởng Skill của predecessor.
- NPC không thể biết sự kiện chưa truyền tới họ.
- Một event không thể hoàn thành nếu precondition cốt lõi không còn đúng.
- Một Historical Anchor không thể vừa Completed vừa Pending.

Các kiểm thử này rất quan trọng vì hệ thống có nhiều tổ hợp mà team không thể liệt kê hết bằng tay.

<a id="m24-t061-s082"></a>

### 58. Headless Simulation

Bản simulation không cần đồ họa phải có thể chạy:

- một vài tháng;
- một năm;
- nhiều năm;
- toàn bộ 25 năm chiến dịch.

Các metric quan trọng:

- population ổn định hoặc biến đổi có lý do;
- household không bị rơi vào trạng thái vô lý;
- economy không tự sinh vô hạn resource;
- faction có biến đổi hợp lý;
- event không lặp vô hạn;
- quan hệ không bị đứt references;
- Story Thread không mắc kẹt;
- Historical Anchors hợp lệ;
- số lượng event trong ngưỡng xử lý được;
- Chronicle vẫn nhất quán.

<a id="m24-t061-s083"></a>

### 59. Branch Coverage

Không thể bảo đảm kiểm thử tất cả trạng thái tổ hợp nếu số lượng cực lớn.

Thay vào đó sử dụng nhiều lớp:

**Rule Coverage:** Mỗi rule/handler đều được kiểm thử.

**Condition Coverage:** Các precondition quan trọng có cả trường hợp đạt và không đạt.

**Pairwise/Combinatorial Testing:** Tập trung vào những tổ hợp điều kiện có khả năng gây lỗi.

**Property-Based Testing:** Sinh dữ liệu rộng để kiểm tra invariant.

**Monte Carlo Simulation:** Chạy nhiều world seed và thu thập phân bố kết quả.

**Narrative Review:** Đánh giá những chuỗi diễn biến đã tạo ra có hợp lý và hấp dẫn không.

Một hệ thống có hàng triệu trạng thái hợp lệ không thể được chứng minh tốt chỉ bằng việc QA chơi vài giờ.

<a id="m24-t061-s084"></a>

### 60. Content Quality Testing

Cần bổ sung các tiêu chí không thuần kỹ thuật:

- tần suất lặp motif;
- tỷ lệ Story Thread không giải được;
- số event có nguyên nhân bị thiếu;
- các trường hợp NPC làm trái personality;
- tỷ lệ event tự kết thúc hợp lệ khi player không can thiệp;
- sự phân bố hoạt động theo nghề;
- mức độ đa dạng kết quả;
- số trường hợp false information được trình bày nhầm thành truth.

Mục tiêu là tránh một hệ thống đúng về code nhưng tạo ra những câu chuyện vô lý hoặc đơn điệu.

---

<a id="m24-t061-s085"></a>

## PHẦN XVII — END-TO-END EXAMPLE: TỪ THẾ GIỚI ĐẾN MỘT SỰ KIỆN

<a id="m24-t061-s086"></a>

### 61. Bối cảnh

Tam Giang đang ở thời điểm nước cao.

Một settlement có tuyến vận tải phụ thuộc vào bến cũ.

Một household làm nghề vận chuyển.

Player đang ở gần đó.

<a id="m24-t061-s087"></a>

### 62. World State

```text
Region: Tam Giang
Season: High Water
RiverPressure: High

Settlement: BenVac
TransportDemand: High
FoodReserve: Declining

Household: NhaTrieu
Profession: RiverTransport
BoatCondition: Poor
```

Các dữ liệu này được World Simulation cập nhật bằng code.

Không do LLM tạo ngay lúc player bước tới.

<a id="m24-t061-s088"></a>

### 63. Event Eligibility

Event Engine tìm những template liên quan:

- BoatRepairNeed;
- TransportDelay;
- HouseholdDebt;
- FloodEvacuation;
- CargoDispute.

Mỗi template có điều kiện riêng.

Giả sử `TransportDelay` đủ điều kiện vì nhu cầu vận tải tăng nhưng thuyền đang hỏng.

<a id="m24-t061-s089"></a>

### 64. Actor Selection

Engine chọn những actor hợp lệ:

- chủ thuyền;
- người cần thuê;
- người có khả năng sửa;
- player nếu có liên hệ và ở phạm vi có thể tiếp xúc.

Việc chọn actor dùng dữ liệu có thật.

Không sinh một “ông lái thuyền già” bất ngờ chỉ vì đoạn truyện nghe hay.

<a id="m24-t061-s090"></a>

### 65. NPC Decision

Chủ thuyền cần thu nhập nhưng lo thuyền không an toàn.

Utility AI có thể chọn:

- tự sửa;
- nhờ thợ;
- tìm người giúp;
- hoãn chuyến;
- chấp nhận rủi ro.

Giả sử người đó chọn tìm người giúp vì thiếu khả năng sửa.

Điều này tạo một Opportunity hoặc Request hợp lệ.

<a id="m24-t061-s091"></a>

### 66. World Director

World Director thấy player:

- đang ở gần;
- có liên hệ với nghề vận tải;
- chưa có quá nhiều event urgent;
- có thể nhận đề nghị từ một người quen.

Director ưu tiên expose tình huống bằng cuộc gặp tại bến.

Director không quyết định thuyền hỏng và không quyết định chủ thuyền muốn gì.

<a id="m24-t061-s092"></a>

### 67. Narrative Presentation

Presentation Layer chọn câu thoại phù hợp:

> “Mạn thuyền lại lỏng rồi. Nếu không sửa được trước khi nước lên nữa, chuyến hàng này khó mà đi nổi.”

Câu này thể hiện trạng thái đã có.

Nó không tạo event mới.

<a id="m24-t061-s093"></a>

### 68. Player Action

Player có thể:

- giúp sửa;
- tìm thợ khác;
- thương lượng một chuyến khác;
- không can thiệp.

Nếu player sửa, Rule Engine kiểm tra:

- công cụ;
- vật liệu;
- skill;
- thời gian;
- tình trạng thuyền.

Kết quả được giải bằng code.

<a id="m24-t061-s094"></a>

### 69. State Change

Giả sử sửa thành công nhưng sử dụng vật liệu không bền.

Engine cập nhật:

```text
BoatOperational = true
BoatRepairQuality = Temporary
MaterialsConsumed = recorded
RepairEvent = committed
```

Chủ thuyền có thể cảm ơn và tạo một Favor.

Favor chỉ được tạo theo Social Rule hợp lệ.

<a id="m24-t061-s095"></a>

### 70. Delayed Consequence

Sau một thời gian, nếu thuyền tiếp tục sử dụng và mạn không được sửa hoàn chỉnh:

Structural Condition có thể xấu đi.

Nếu chủ thuyền biết việc sửa chỉ là tạm, họ có thể chủ động sửa tiếp.

Nếu không, có thể xuất hiện rủi ro mới.

Người chơi có thể gặp lại chiếc thuyền nhiều năm sau.

Tất cả những diễn biến đó đến từ trạng thái thuyền, quyết định actor và thời gian.

**Không có bước nào cần LLM realtime quyết định chuyện gì sẽ xảy ra.**

---

<a id="m24-t061-s096"></a>

## PHẦN XVIII — HỆ THỐNG SINH TRUYỀN THUYẾT

<a id="m24-t061-s097"></a>

### 71. Có thể sinh myth bằng code không?

Có thể tạo **quá trình hình thành truyền thuyết** bằng code, nhưng cần phân biệt hai phần.

**Myth Evolution Logic:** Code quyết định câu chuyện được lưu truyền, ai kể, phiên bản nào lan rộng, thông tin nào biến dạng.

**Myth Presentation:** Các đoạn kể được chọn hoặc tạo từ các mẫu văn bản đã duyệt.

Ví dụ player cứu một đoàn người trong lũ.

Engine lưu:

- người tham gia;
- số người được cứu;
- địa điểm;
- người chứng kiến;
- mức độ nguy hiểm;
- những câu chuyện đã truyền đi.

Khi rumor lan qua nhiều mạng xã hội, một số yếu tố có thể bị cường điệu theo quy tắc.

<a id="m24-t061-s098"></a>

### 72. Myth Transformation Grammar

Ví dụ quy tắc:

```text
Observed Event:
Three people rescued travelers during flood.

Rumor Variant:
A group from BenVac saved many travelers.

Heroic Retelling:
Vac Man led the rescue through the flood.

Mythic Retelling:
Vac Man crossed the raging river
when no other boat dared move.
```

Không cần LLM realtime để quyết định ai được cứu.

Những biến thể kể chuyện có thể được sinh offline hoặc bằng Narrative Grammar đã phê duyệt.

Quan trọng là **Story Claim luôn lưu liên hệ với Origin Event**, dù nội dung đã bị biến dạng.

<a id="m24-t061-s099"></a>

### 73. Giới hạn Myth Generation

Không cho phép engine ngẫu nhiên biến mọi nhân vật nổi tiếng thành thần linh.

Myth Formation cần:

- event đủ đáng nhớ;
- mạng truyền thông tin;
- thời gian;
- biểu tượng;
- social interpretation;
- cultural compatibility.

Với những myth lớn như Kim Quy và Nỏ thần, nội dung lõi cần authored và historical/game-canon review riêng.

---

<a id="m24-t061-s100"></a>

## PHẦN XIX — ĐÁNH GIÁ CÁC PHƯƠNG ÁN KIẾN TRÚC

<a id="m24-t061-s101"></a>

### 74. Phương án A — LLM điều khiển toàn bộ thế giới

**Cách hoạt động:** LLM nhận World State và quyết định event, hành vi NPC, kết quả hành động.

**Ưu điểm:** Nhanh tạo prototype hội thoại và tình huống mới.

**Nhược điểm:**

- không bảo đảm tái hiện;
- dễ tạo mâu thuẫn;
- khó cân bằng;
- khó kiểm thử;
- tốn inference;
- phụ thuộc model;
- khó xử lý lịch sử dài hạn;
- khó bảo đảm actor không biết bí mật.

**Kết luận:** Không chọn.

<a id="m24-t061-s102"></a>

### 75. Phương án B — Code thuần với toàn bộ nội dung viết thủ công

**Cách hoạt động:** Engine dùng code; toàn bộ character, event và câu chữ do designer viết.

**Ưu điểm:**

- kiểm soát chặt;
- dễ kiểm thử hơn;
- chất lượng các cảnh chính dễ bảo đảm.

**Nhược điểm:**

- chi phí authored content lớn;
- khó bao phủ nhiều biến thể nghề, địa phương và quan hệ;
- dễ có lặp lại khi world simulation tạo nhiều tình huống.

**Kết luận:** Có thể hoạt động với game phạm vi nhỏ nhưng không tối ưu cho quy mô GDD hiện tại.

<a id="m24-t061-s103"></a>

### 76. Phương án C — LLM offline + Deterministic Engine

**Cách hoạt động:** LLM hỗ trợ sản xuất nội dung có cấu trúc; developer kiểm chứng và phê duyệt; runtime chạy bằng code.

**Ưu điểm:**

- khả năng tái hiện và debug tốt;
- độc lập inference trong runtime;
- mở rộng thư viện nội dung;
- giữ quyền kiểm soát historical canon;
- tương thích Save/Load và long-term simulation;
- dễ quản lý version và regression test hơn.

**Nhược điểm:**

- cần Content Pipeline nghiêm ngặt;
- yêu cầu thiết kế schema và rule language tốt;
- việc tạo nhiều biến thể có ý nghĩa vẫn tốn công biên tập;
- văn bản quá máy móc nếu grammar nghèo;
- cần đầu tư công cụ debug và validation.

**Kết luận:** **Chọn làm kiến trúc chính thức.**

<a id="m24-t061-s104"></a>

### 77. Phương án D — Hybrid với LLM realtime chỉ diễn đạt câu chữ

**Cách hoạt động:** Game Engine vẫn quyết định hoàn toàn gameplay; LLM chỉ tạo biến thể trình bày từ một payload đã được lọc.

**Ưu điểm:** Hội thoại có thể phong phú về ngôn ngữ hơn.

**Nhược điểm:** Chi phí, độ trễ, kiểm soát phong cách, hallucination, tính nhất quán giữa lần chơi và yêu cầu vận hành.

**Kết luận:** Không cần cho bản chính thức. Chỉ xem xét như một tính năng thử nghiệm sau này nếu có validator, fallback và không có bất kỳ quyền ghi World State nào.

---

<a id="m24-t061-s105"></a>

## PHẦN XX — CÔNG CỤ DÀNH CHO ĐỘI PHÁT TRIỂN

<a id="m24-t061-s106"></a>

### 78. Content Editor

Cần công cụ để designer tạo:

- Event Template;
- Character Archetype;
- Dialogue Act;
- Story Thread;
- Rule Reference;
- Location/Actor Requirement.

Các điều kiện được lựa chọn từ những field và operator được hỗ trợ.

Không để writer tự viết code tùy ý hoặc đưa một biểu thức không kiểm soát vào JSON.

<a id="m24-t061-s107"></a>

### 79. Rule Inspector

Công cụ cho phép chọn một World Snapshot rồi xem:

- những event nào đủ điều kiện;
- event nào bị loại;
- vì sao bị loại;
- actor nào phù hợp;
- random decision nào đã xảy ra;
- state changes dự kiến.

Ví dụ:

```text
Event: BoatRepairNeed

Eligible: YES

Reason:
- Boat condition below threshold
- Transport demand exists
- Owner is alive
- Valid location

Selected actor:
Trieu Moc

Result:
Request created
```

Đây là công cụ rất quan trọng để debug các event không xuất hiện hoặc xuất hiện sai.

<a id="m24-t061-s108"></a>

### 80. NPC Life Inspector

Cho phép developer xem:

- Birth;
- Childhood Events;
- Skill Sources;
- Profession;
- Relationships;
- Personal Goals;
- Known Facts;
- Current Utility Decisions;
- Current Obligations;
- Death/Succession State.

Nếu một người 18 tuổi có Mastery quá cao, tool phải giúp tìm ra chính xác event hoặc rule đã tạo ra điều đó.

<a id="m24-t061-s109"></a>

### 81. World Timeline Inspector

Developer có thể tìm:

- event theo năm;
- các thay đổi settlement;
- vòng đời NPC;
- faction transformation;
- resource flow;
- Historical Anchors.

Cần có khả năng so sánh hai World Snapshot.

Ví dụ:

**Năm 8 → Năm 13**

Settlement Bến Vạc:

- dân số;
- sản lượng;
- nguồn hàng;
- đường vận tải;
- gia đình;
- faction;
- các event quan trọng.

Đây là công cụ xác minh hiệu quả của World Simulation trong một campaign dài hạn.

<a id="m24-t061-s110"></a>

### 82. Content Validation Dashboard

Hiển thị:

- schema errors;
- missing references;
- unreachable events;
- impossible preconditions;
- duplicate or near-duplicate content;
- unbounded event loops;
- violations of historical constraints;
- missing failure states;
- unapproved content;
- regression failures.

Tất cả Content Packs phải qua kiểm tra tự động trước khi được đưa vào build.

---

<a id="m24-t061-s111"></a>

## PHẦN XXI — PHƯƠNG ÁN TRIỂN KHAI THEO GIAI ĐOẠN

<a id="m24-t061-s112"></a>

### 83. Phase 1 — Deterministic Kernel

Xây:

- World State Model;
- Entity IDs;
- Command Handler;
- Rule Engine;
- Deterministic RNG;
- Event Journal;
- Snapshot;
- Validation.

Mục tiêu: Một hành động tạo được State Change hợp lệ, lưu được và tái hiện được.

<a id="m24-t061-s113"></a>

### 84. Phase 2 — Character & Household Simulation

Xây:

- Character Generator;
- Household Generator;
- Life History;
- Skills;
- Profession;
- NPC Goals;
- Time progression.

Mục tiêu: Một settlement nhỏ có cư dân sống và thay đổi theo thời gian mà không cần LLM runtime.

<a id="m24-t061-s114"></a>

### 85. Phase 3 — Event & Social Engine

Xây:

- Event Templates;
- Eligibility;
- NPC Decision;
- Social Graph;
- Knowledge Propagation;
- Dialogue Act;
- Story Threads.

Mục tiêu: Một vấn đề trong world được sinh từ điều kiện thật, có actor hành động và có thể được player can thiệp.

<a id="m24-t061-s115"></a>

### 86. Phase 4 — Cross-Domain Simulation

Xây:

- Economy;
- Travel;
- Ecology;
- Faction;
- Combat/Injury;
- Crafting;
- World Director.

Mục tiêu: Một biến cố có thể lan qua nhiều domain theo các rule kiểm chứng được.

<a id="m24-t061-s116"></a>

### 87. Phase 5 — AI Content Production

Xây:

- Content Schemas;
- Offline LLM Workflow;
- Validators;
- Content Editor;
- Test Runner;
- Narrative Grammar;
- Content Pack Versioning.

Mục tiêu: Thư viện sự kiện và nhân vật có thể mở rộng mà không biến runtime thành hệ thống ra quyết định bằng LLM.

Công cụ authoring cơ bản nên được phát triển sớm song song với các phase trước; việc hoàn thiện dây chuyền LLM chỉ nên mở rộng sau khi schema và rule ổn định.

<a id="m24-t061-s117"></a>

### 88. Phase 6 — Historical & Legacy Simulation

Xây:

- Historical Anchors;
- Time Skip;
- Aging;
- Death;
- Succession;
- Legacy;
- Chronicle;
- Myth Evolution.

Mục tiêu: Một thế giới có thể vận hành qua toàn bộ giai đoạn lịch sử mà dữ liệu và quan hệ vẫn nhất quán.

---

<a id="m24-t061-s118"></a>

## PHẦN XXII — CÁC QUYẾT ĐỊNH KHÓA CHÍNH THỨC

<a id="m24-t061-s119"></a>

### 89. Quyền quyết định

1. Runtime Gameplay Engine hoạt động bằng code và dữ liệu đã được kiểm chứng.
2. LLM không quyết định World State.
3. LLM không quyết định kết quả action.
4. LLM không sinh NPC hoặc event có hiệu lực gameplay một cách tự do trong runtime.
5. LLM có thể hỗ trợ sinh dữ liệu và nội dung offline.
6. Nội dung LLM cần validation và approval trước khi sử dụng.
7. Runtime không bắt buộc có LLM service.
8. Dialogue Intent và Social Outcome do engine xác định.
9. Narrative Presentation không được thay đổi facts.
10. World Director không được quyền ghi các kết quả mô phỏng tùy ý.

<a id="m24-t061-s120"></a>

### 90. Rule & Randomization

11. Không triển khai hàng triệu nhánh bằng `if/else` thủ công.
12. Sử dụng compositional rule system.
13. Event Templates khai báo điều kiện và tham chiếu handler.
14. Rule Handlers phải được code và kiểm thử.
15. Random chỉ xảy ra trong tập khả năng hợp lệ.
16. Random sử dụng seed và ngữ cảnh được kiểm soát.
17. RNG algorithm và state cần được version hóa.
18. Có khả năng deterministic replay trong phạm vi đã cam kết.
19. Công thức gameplay có thể ẩn với player nhưng phải truy vết được bởi developer.
20. Không dùng công thức ẩn để gian lận hoặc che kết quả thiếu nhân quả.

<a id="m24-t061-s121"></a>

### 91. World State & Narrative

21. Character History phải có provenance.
22. NPC chỉ quyết định dựa trên Knowledge/Belief của chính họ.
23. Event phải có Trigger, Participants, Resolution và Consequences.
24. Domain State Changes phải tuân ownership và transaction rules.
25. Story Threads bắt nguồn từ World Events/Unresolved States.
26. Historical Anchors được quản lý riêng.
27. Quest không tạo World Truth.
28. Myth có thể được hình thành bằng rule-based transformation.
29. World History được lưu độc lập với player-facing Chronicle.
30. Knowledge Filter là bắt buộc.

<a id="m24-t061-s122"></a>

### 92. Scale & Quality

31. Simulation dùng nhiều mức chi tiết.
32. Background Population không cần full NPC AI mỗi tick.
33. Relevant NPC được nâng Simulation Tier khi cần.
34. Một thay đổi ở LOD không được xóa lịch sử đã xác lập.
35. Event-Driven Scheduling được ưu tiên cho các hoạt động ít thay đổi.
36. Save phải lưu World State, version và thông tin RNG cần thiết.
37. Có Event Journal cho các thay đổi quan trọng.
38. Có snapshot và khả năng replay/debug.
39. Có automated content validation.
40. Có headless simulation và property-based testing.

---

<a id="m24-t061-s123"></a>

## PHẦN XXIII — QUALITY GATES CUỐI CÙNG

<a id="m24-t061-s124"></a>

### 93. Character Generation Quality Gate

Một character được xem là hợp lệ khi:

- có nguồn gốc gia đình và địa phương;
- tuổi và quá trình học tập phù hợp;
- Attributes, Skills và Profession có nguyên nhân;
- Personality có cơ sở từ tính khí và trải nghiệm;
- các mối quan hệ tham chiếu NPC tồn tại;
- không có tri thức không nguồn;
- có khả năng sống và phát triển;
- không vi phạm các điều kiện lịch sử;
- có thể tái hiện từ seed, phiên bản và lịch sử phát sinh tương ứng.

<a id="m24-t061-s125"></a>

### 94. Event Engine Quality Gate

Một event hợp lệ khi:

- có trigger;
- có actor đủ điều kiện;
- có location/time phù hợp;
- có mục tiêu và nguồn lực;
- có action/outcome handlers;
- có state changes kiểm chứng được;
- có failure/ignored outcome nếu cần;
- không sinh item, NPC hoặc thông tin vô căn cứ;
- không tạo vòng event vô hạn;
- có thể giải thích được bằng audit log.

<a id="m24-t061-s126"></a>

### 95. World Simulation Quality Gate

Một thế giới đạt yêu cầu khi:

- không có dữ liệu mâu thuẫn sau các lần cập nhật;
- các hoạt động background có nhân quả;
- những hệ kinh tế và xã hội thực sự liên kết;
- NPC Aging và Death nhất quán;
- faction có thể phát triển theo rule;
- resource không tự sinh vô hạn;
- không có actor hiện diện đồng thời ở các vị trí bất khả thi;
- Historical Anchors đạt trạng thái hợp lệ;
- World State tái hiện được;
- performance nằm trong mục tiêu thiết kế.

<a id="m24-t061-s127"></a>

### 96. LLM Pipeline Quality Gate

Một nội dung LLM tạo ra chỉ được chấp nhận nếu:

- đúng schema;
- không vi phạm game-canon;
- đúng phân loại lịch sử;
- có thể map vào Rule Engine;
- reference hợp lệ;
- không lặp nội dung quá mức;
- có meaningful decision khi là encounter;
- có causal consequence;
- có thể chạy simulation test;
- được phê duyệt theo mức quan trọng của nội dung.

---

<a id="m24-t061-s128"></a>

## PHẦN XXIV — TUYÊN NGÔN KIẾN TRÚC

<a id="m24-t061-s129"></a>

### 97. Thế giới có thể vô cùng phong phú mà không cần LLM quyết định mọi thứ

Một con người được tạo ra từ:

gia đình;

vùng đất;

di truyền;

môi trường sống;

sự kiện tuổi thơ;

nghề nghiệp;

quan hệ;

tâm lý;

cơ hội;

lựa chọn;

thời gian.

Một sự kiện được tạo ra từ:

áp lực;

nhu cầu;

mâu thuẫn;

thông tin;

tài nguyên;

địa lý;

lịch sử;

những người tham gia.

Một câu chuyện được hình thành khi các nhân vật tương tác với những sự kiện đó và kết quả tiếp tục để lại hậu quả.

Không cần một LLM nghĩ ra toàn bộ thế giới sau mỗi lần player nhấn nút.

Cần một **simulation engine đủ chặt chẽ để các câu chuyện có thể tự xuất hiện từ những tương tác hợp lệ**.

<a id="m24-t061-s130"></a>

### 98. LLM là công cụ mở rộng sức sáng tạo, không phải nguồn sự thật

LLM có thể giúp designer nghĩ ra:

- một người thợ có quá khứ đặc biệt;
- một lời hứa bị bỏ quên;
- một tập tục khác nhau giữa hai vùng;
- một mâu thuẫn gia đình có chiều sâu;
- một vụ tranh chấp tài nguyên;
- một cách kể truyền thuyết.

Nhưng sau khi được phê duyệt, những nội dung đó trở thành:

**Structured Data + Rule References + Narrative Assets.**

Game Engine mới là hệ thống vận hành chúng.

<a id="m24-t061-s131"></a>

### 99. Engine không cần biết kể một câu chuyện hay. Engine cần biết sự thật và nhân quả.

Đây là sự phân vai quan trọng nhất.

**Rule Engine trả lời:**

- Điều gì xảy ra?
- Vì sao xảy ra?
- Ai tham gia?
- Những điều kiện nào được thỏa?
- Điều gì thay đổi?
- Điều gì có thể xảy ra tiếp?

**Narrative Presentation trả lời:**

- Character cảm nhận điều đó thế nào?
- Người khác kể lại ra sao?
- Giao diện nên diễn đạt điều gì?
- Thông tin nào player được biết?
- Câu chuyện được trình bày bằng văn phong nào?

Đặt đúng ranh giới này sẽ giúp game vừa có chiều sâu mô phỏng, vừa có nội dung giàu cảm xúc.

<a id="m24-t061-s132"></a>

### 100. Kết luận thiết kế

Một game theo kiến trúc này không nhất thiết phải có hàng triệu đoạn truyện authored.

Nó cần:

- một hệ thống nhân vật có tính nhân quả;
- một bộ quy tắc được thiết kế tốt;
- dữ liệu nội dung có chất lượng;
- các sự kiện có điều kiện và hậu quả;
- một thế giới ghi nhớ những gì đã xảy ra;
- những thuật toán xử lý quyết định nhất quán;
- một hệ thống trình bày đủ tốt để người chơi cảm nhận được câu chuyện.

Khi những phần đó vận hành cùng nhau, mỗi playthrough có thể hình thành những chuỗi sự kiện khác biệt.

Không phải vì một LLM đang tùy tiện sáng tác số phận.

Mà vì:

> **Cùng một thế giới có thể tạo ra những cuộc đời rất khác nhau khi xuất thân, hoàn cảnh, thời gian và lựa chọn của con người thay đổi.**

**LLM hỗ trợ tạo ra những khả năng.**

**Engine quyết định khả năng nào thực sự được hiện thực hóa theo quy tắc.**

**Player quyết định họ sẽ hành động như thế nào trong những khả năng ấy.**

**World State lưu lại kết quả.**

**Lịch sử được hình thành từ đó.**

<a id="m24-t061-s133"></a>

### Đánh giá kiến trúc sau Module 24

Tôi đánh giá **phương án C — Offline LLM Content Generation + Deterministic Rule Engine** là phương án phù hợp nhất với GDD hiện tại.

<box gap={2}>
  <row align="center" justify="between">
    **LLM quyết định gameplay tại runtime**
    <badge color="danger">Không sử dụng</badge>
  </row>
  <divider color="subtle"/>
  <row align="center" justify="between">
    **LLM hỗ trợ sinh nội dung trước phát hành**
    <badge color="success">Sử dụng</badge>
  </row>
  <divider color="subtle"/>
  <row align="center" justify="between">
    **Code Rule Engine + deterministic RNG**
    <badge color="success">Bắt buộc</badge>
  </row>
  <divider color="subtle"/>
  <row align="center" justify="between">
    **LLM tạo lời văn trong runtime**
    <text color="secondary" weight="medium" size="sm">Không cần trong bản chính thức</text>
  </row>
</box>

Hai cơ sở kỹ thuật đáng chú ý từ tài liệu hiện hành: Unity có API cho phép lưu/khôi phục trạng thái bộ sinh số giả ngẫu nhiên để hỗ trợ tái hiện kết quả; còn hướng dẫn kiến trúc của Microsoft mô tả rõ vai trò của Event Sourcing, Snapshot và Materialized View, đồng thời cảnh báo chi phí của việc áp dụng Event Sourcing không chọn lọc. Vì thế tôi nghiêng về **Hybrid State + Event Journal** hơn là bắt buộc event-source mọi thay đổi nhỏ. <Cite refs={["turn936207search2","turn936207search6","turn111937search1","turn111937search2"]}/>

**Bước tiếp theo nên là Module 25 — GDD Architecture Review & System Dependency Audit**, thay vì lập tức bổ sung thêm hệ thống. Chúng ta cần kiểm tra chéo 24 module để tìm xung đột logic, cơ chế trùng lặp, những phần khó triển khai, các phụ thuộc vòng, các vấn đề làm giảm trải nghiệm và xác định một **MVP/Vertical Slice Architecture** thật sự khả thi. Đây là bước cần thiết trước khi chuyển GDD thành tài liệu phát triển và giao cho AI coding agent hoặc đội lập trình.

<!-- SOURCE T061 END -->

