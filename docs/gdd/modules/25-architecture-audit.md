# Module 25 — Architecture Review và System Dependency Audit

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T065](../../source/conversation.md#t065)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Audit này có đề xuất đã được cụ thể hóa trong 26–29. Dùng bảng ownership/decision register của gói hiện tại để giải quyết overlap; không đọc mọi vấn đề được nêu ở đây như vẫn chưa có hướng xử lý. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Toàn bộ GDD 01–24 và phạm vi slice. |
| Đầu ra / bên sở hữu | Audit findings, dependency boundaries và design decisions; không mutation runtime. |
| Đọc cùng | 26–29; APPLICABLE-BASELINE, OWNERSHIP, OPEN-DECISIONS.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Không domain nào vừa có owner khác vừa ghi cùng state trực tiếp; các historical invariants có contract. |
| Việc chưa được chứng minh | Tech validation và production content gaps còn ghi riêng. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T065 BEGIN -->

<a id="m25-t065-s001"></a>

## GDD MODULE 25
<a id="m25-t065-s002"></a>

## ARCHITECTURE REVIEW, SYSTEM DEPENDENCY AUDIT & VERTICAL SLICE BLUEPRINT

**Dự án:** Living World Narrative RPG — bối cảnh hư cấu lịch sử cuối Văn Lang, chuyển tiếp Âu Lạc và Cổ Loa  
**Loại tài liệu:** System Architecture Review / GDD Consistency Audit / Technical Feasibility Assessment  
**Trạng thái:** Đánh giá thiết kế, chưa phải chứng nhận hệ thống đã được triển khai  
**Phạm vi:** Toàn bộ các nhóm thiết kế đã xây dựng trong 24 module GDD trước đó  
**Ưu tiên:** Tính nhân quả, tính nhất quán, quyền tự quyết của người chơi, khả năng triển khai và chất lượng trải nghiệm dài hạn

---

<a id="m25-t065-s003"></a>

## PHẦN I — MỤC ĐÍCH KIỂM TOÁN

<a id="m25-t065-s004"></a>

### 1. Vì sao cần Module 25?

Sau 24 module, GDD đã xác định một thế giới rất sâu:

- Con người được sinh ra trong những gia đình và cộng đồng có lịch sử.
- Năng lực được hình thành bởi kinh nghiệm, không phải chọn class từ đầu.
- NPC có mục tiêu và quan hệ riêng.
- Kinh tế, sinh thái, chiến tranh, thương mại và nghề nghiệp vận hành theo quy tắc.
- Quest xuất hiện từ Story Thread, không tạo ra sự thật thế giới.
- Người chơi có thể sống, trưởng thành, già đi, qua đời và để lại di sản.
- World Director điều phối trải nghiệm nhưng không điều khiển tùy tiện số phận.
- Runtime Engine dùng code, rule và deterministic random; LLM chỉ hỗ trợ sản xuất nội dung ngoài runtime.

Từng module có thể hợp lý khi đọc riêng.

Nhưng khi đưa vào cùng một game, một số quyết định có thể trở nên mâu thuẫn hoặc tạo ra những phụ thuộc rất khó xử lý.

Ví dụ:

Một NPC chết trong combat.

Điều này đồng thời ảnh hưởng tới:

- Character State;
- Household;
- Profession;
- Inventory;
- Faction;
- Relationship;
- Quest;
- Knowledge;
- Rumor;
- Chronicle;
- Succession;
- Economy.

Nếu mỗi hệ thống tự xử lý cái chết theo cách riêng, game có thể sinh ra những trạng thái mâu thuẫn.

Một NPC đã chết vẫn nhận đơn hàng.

Một người con không được cập nhật tình trạng mồ côi.

Một item bị thừa kế hai lần.

Một faction vẫn sử dụng NPC đó làm thủ lĩnh.

Một quest vẫn yêu cầu gặp người ấy.

Module 25 phải giải quyết những vấn đề như vậy trước khi phát triển.

<a id="m25-t065-s005"></a>

### 2. Mục tiêu kiểm toán

Kiểm toán được chia thành sáu nhiệm vụ.

**Architectural Consistency:** Xác nhận các hệ thống không mâu thuẫn ở cấp độ thiết kế.

**Data Ownership:** Chỉ định hệ thống nào sở hữu và được phép thay đổi từng loại trạng thái.

**Dependency Analysis:** Xác định quan hệ phụ thuộc, các dependency cycle nguy hiểm và cách cắt chúng.

**Scope Assessment:** Đánh giá những hệ thống cần thiết để chứng minh gameplay và những hệ thống có thể triển khai sau.

**Technical Feasibility:** Chọn mô hình thực thi phù hợp với game đơn người chơi, tính toán, lưu trữ và debug.

**Quality Assurance:** Đưa ra các điều kiện để kiểm chứng tính chính xác trước khi mở rộng nội dung.

<a id="m25-t065-s006"></a>

### 3. Giới hạn của đợt kiểm toán

Đây là kiểm toán **thiết kế GDD**, dựa trên các quyết định đã được xây dựng trong cuộc trao đổi.

Nó chưa phải:

- code review;
- performance benchmark;
- kiểm thử một bản build;
- xác nhận niên đại khảo cổ;
- kết quả playtest thực tế;
- tài liệu ước lượng ngân sách sản xuất cuối cùng.

Vì vậy các kết luận được phân loại thành:

**Giữ nguyên:** Nguyên tắc đã rõ và có khả năng vận hành nhất quán.

**Cần chuẩn hóa:** Ý tưởng đúng nhưng thuật ngữ, giới hạn hoặc trách nhiệm chưa được thống nhất.

**Cần sửa thiết kế:** Có mâu thuẫn hoặc lỗ hổng có thể gây lỗi khi triển khai.

**Cần prototype:** Không thể xác nhận bằng lý thuyết.

**Hoãn mở rộng:** Có giá trị nhưng không nên đưa toàn bộ vào bản thử nghiệm đầu tiên.

---

<a id="m25-t065-s007"></a>

## PHẦN II — KIỂM TOÁN TÍNH NHẤT QUÁN CỦA TOÀN BỘ GDD

<a id="m25-t065-s008"></a>

### 4. Các quyết định kiến trúc nhất quán và nên giữ

Kết quả đánh giá cho thấy các nhóm hệ thống đã thống nhất được một số trụ cột rất quan trọng.

<a id="m25-t065-s009"></a>

#### Character là một cá nhân có lịch sử

Character Generation, Life Events, Skills, Professions, Classes, Party và Legacy đều cùng sử dụng triết lý:

> Nhân vật trở thành một con người thông qua những gì đã trải qua.

Điều này nhất quán.

Không cần thay đổi.

<a id="m25-t065-s010"></a>

#### World State tồn tại độc lập với người chơi

World Simulation, Economy, Ecology, Faction, Quest, Time và World Director đều hướng tới cùng một quy tắc:

> Sự kiện không chỉ tồn tại khi player nhìn thấy.

Đây là nền tảng bắt buộc phải giữ.

<a id="m25-t065-s011"></a>

#### Knowledge khác Truth

Quest, Investigation, Rumor, Belief, Codex và UI đều phân biệt lời kể, suy luận, niềm tin và sự thật.

Đây là quyết định rất tốt vì giải quyết được nhiều vấn đề logic trong game điều tra và narrative.

<a id="m25-t065-s012"></a>

#### Consequence có tính dài hạn

Injury, Profession, Economy, Relationships, Settlement, Chronicle và Legacy đều yêu cầu hậu quả tiếp tục tồn tại theo thời gian.

Điều này nhất quán với campaign kéo dài nhiều năm.

<a id="m25-t065-s013"></a>

#### Combat không phải gameplay mặc định

Profession, Crafting, Trade, Social, Travel, Ritual và Historical Campaign đều có cơ chế tham gia ngoài chiến đấu.

Đây là một đặc điểm khác biệt cần bảo vệ.

<a id="m25-t065-s014"></a>

#### LLM không phải World Authority

Module 24 đã xác lập ranh giới rõ giữa content generation, rule resolution và presentation.

Đây là điều kiện khiến dự án có thể kiểm thử, lưu/tải và phát triển như một game thực sự.

<a id="m25-t065-s015"></a>

### 5. Các điểm chưa thực sự được khóa hoàn chỉnh

Dù tầm nhìn thống nhất, một số thành phần cần tiếp tục làm rõ.

**Thứ nhất:** World State chưa có một bảng Data Ownership thống nhất cho mọi domain.

**Thứ hai:** Một số subsystem cùng đề cập đến việc sinh hoặc kích hoạt event, nhưng quyền quyết định cụ thể chưa được định nghĩa thành interface chính thức.

**Thứ ba:** Historical Anchor vừa được yêu cầu giữ nguyên, vừa phải tôn trọng quyền tự do và hệ quả hành động của player. Hai yêu cầu này cần một hợp đồng thiết kế rõ.

**Thứ tư:** Cơ chế succession chưa có quy tắc hoàn chỉnh về trạng thái chuyển tiếp giữa người chết, companion và người kế tục.

**Thứ năm:** Các hệ Economy và Survival đang sử dụng mức abstraction khác nhau; cần quy tắc chuyển đổi tài nguyên thống nhất.

**Thứ sáu:** Quy mô NPC, settlement, social graph và event rất lớn nhưng chưa có performance budget dựa trên prototype.

**Thứ bảy:** Chưa có định nghĩa chung về **cái gì là một event**, **cái gì là một quest**, **cái gì là một story thread**, **cái gì là một observation** và **cái gì chỉ là presentation** ở cấp code contract.

Những vấn đề này phải được giải quyết trước khi nhiều programmer bắt đầu phát triển song song.

---

<a id="m25-t065-s016"></a>

## PHẦN III — BẢN ĐỒ PHỤ THUỘC HỆ THỐNG

<a id="m25-t065-s017"></a>

### 6. Phân loại subsystem

Đề xuất tổ chức GDD thành tám nhóm kiến trúc.

| Nhóm | Những hệ thống liên quan | Vai trò |
|---|---|---|
| Foundation | World Clock, Entity Registry, Rules, RNG, Event Scheduler | Nền thực thi |
| World Simulation | Settlement, Household, Economy, Ecology, Faction, Travel | Thế giới vận hành |
| Character Simulation | Attributes, Skills, Profession, Class, Health, Aging, Death | Đời sống cá nhân |
| Social & Knowledge | Social Graph, Belief, Knowledge, Rumor, Memory | Nhận thức và quan hệ |
| Interactive Gameplay | Combat, Exploration, Dungeon, Crafting, Dialogue | Hành động trực tiếp |
| Narrative | Life Events, Story Threads, Quest, Historical Anchors | Tổ chức diễn biến |
| Experience | World Director, HUD, Map, Journal, Codex, Chronicle | Trình bày cho người chơi |
| Production & Operations | Content Pipeline, Validation, Save/Load, Debug Tools | Phát triển và vận hành |

Đây là những **logical modules**, không phải tám microservice.

Trong bản game đơn người chơi, chúng có thể nằm trong một ứng dụng duy nhất.

<a id="m25-t065-s018"></a>

### 7. Dependency Direction

Đề xuất quy tắc:

**Các domain rule đọc dữ liệu thông qua interface rõ ràng, tạo Domain Event và không gọi ngược UI hoặc World Director để xin quyết định gameplay.**

Ví dụ:

Combat có thể yêu cầu Character Query cung cấp kỹ năng.

Combat không được gọi trực tiếp Quest Engine và bảo:

> “Hãy đánh dấu quest hoàn thành vì tôi vừa giết NPC.”

Combat phải phát sự kiện có ý nghĩa:

`ActorIncapacitated`, `ActorSurrendered`, `ActorDied` hoặc `ObjectiveSecured`.

Quest Engine sau đó tự đánh giá sự kiện này có tác động đến Story Thread nào.

Cách đó giảm coupling.

<a id="m25-t065-s019"></a>

### 8. Architecture Map

```text
                  CONTENT PACK
             Rules / Archetypes
             Events / Dialogue
             Historical Anchors
                      |
                      v
              GAME APPLICATION
                      |
                      v
             AUTHORITATIVE CORE
        ┌─────────────┴─────────────┐
        |                           |
   Command Processor          World Clock
        |                           |
        v                           v
   Rule Resolution          Event Scheduler
        |                           |
        └─────────────┬─────────────┘
                      |
                      v
               WORLD STATE
                      |
        ┌─────────────┼─────────────┐
        |             |             |
     Character      World         Social
     Domain         Domain        Domain
        |             |             |
        └─────────────┼─────────────┘
                      |
                      v
               DOMAIN EVENTS
                      |
        ┌─────────────┼─────────────┐
        |             |             |
   Story Thread    Knowledge     History
   Projection      Projection    Projection
        |             |             |
        └─────────────┼─────────────┘
                      |
                      v
                 VIEW LAYER
                      |
              World Director
                      |
             Presentation UI
```

Sơ đồ minh họa ranh giới trách nhiệm, chưa phải sơ đồ deployment.

Trong triển khai thực tế, các domain có thể cần đọc trạng thái nhau. Điều bắt buộc là **quyền ghi và commit phải được kiểm soát**.

---

<a id="m25-t065-s020"></a>

## PHẦN IV — DATA OWNERSHIP AUDIT

<a id="m25-t065-s021"></a>

### 9. Vấn đề: nhiều hệ cùng muốn sở hữu một trạng thái

Ví dụ:

Combat tạo thương tích.

Health xử lý thương tích.

Character Profile hiển thị thương tích.

Party System cần biết người bị thương có tiếp tục hành trình không.

Quest cần biết người được cứu còn sống không.

Nếu mọi subsystem đều có biến `is_injured` riêng, chúng sẽ có nguy cơ không nhất quán.

Cần thiết kế:

**Single Authoritative Owner + Read Projections.**

<a id="m25-t065-s022"></a>

### 10. Ma trận sở hữu dữ liệu

| Dữ liệu | Authoritative Owner | Hệ thống có thể yêu cầu thay đổi | Hệ thống chỉ đọc/projection |
|---|---|---|---|
| World Time | World Clock | Time Controller | Tất cả |
| Entity Identity | Entity Registry | Authorized Factory | Tất cả |
| Age, Alive/Dead | Character Life | Combat, Health, Event Engine qua command | Quest, UI, Household, Faction |
| Injury | Health & Injury | Combat, Disease, Accident | Travel, Party, Profession |
| Skill & Knowledge Domain | Character Development | Training, Profession, Life Event | Combat, Crafting, Dialogue |
| Current Location | Spatial/Travel Domain | Travel, Combat, Migration | Map, Quest, Encounter |
| Item Ownership | Asset/Inventory Ledger | Trade, Inheritance, Loot, Crafting | Equipment, Economy, UI |
| Commodity Stock | Economy/Asset Ledger | Production, Consumption, Trade | Settlement, Quest |
| Relationship | Social Graph | Social Action, Life Event | Dialogue, Faction, Quest |
| Personal Knowledge | Knowledge System | Observation, Testimony, Teaching | UI, Dialogue, Quest |
| Faction Membership | Faction Domain | Recruitment, Expulsion, Political Event | Social, Quest, UI |
| Settlement Infrastructure | Settlement Domain | Construction, Damage, Repair | Travel, Economy, Map |
| Story Thread | Narrative State | Domain Events, Narrative Rules | Quest, Director, Journal |
| Quest View | Quest Projection | Không ghi World Truth trực tiếp | UI |
| Historical Era/Anchor | Historical Controller | Validated Historical Transition | World Director, Chronicle |
| Mạch State | Mạch Domain | Ritual, Environment, Authorized Event | Belief, Quest, Exploration |
| Chronicle | History Projection | Committed Events | UI |
| Player Notes | Player Information Layer | Player | Journal |

Ma trận này là một trong những tài sản kiến trúc quan trọng nhất của dự án.

<a id="m25-t065-s023"></a>

### 11. Quy tắc cập nhật xuyên domain

Khi một hành động ảnh hưởng nhiều domain, không cho các domain tự chạy theo một chuỗi tùy ý.

Ví dụ:

Player giết một NPC.

**Combat** xác định kết quả tấn công.

**Life Domain** xác lập tử vong.

**Inventory** đánh giá những vật phẩm vẫn ở trên cơ thể hoặc bị rơi.

**Party** cập nhật Active Membership.

**Household** phản ánh mất thành viên.

**Profession** cập nhật năng lực sản xuất nếu NPC là worker.

**Faction** đánh giá việc mất một thành viên hoặc leader.

**Knowledge** truyền thông tin tới witness.

**Social** xử lý phản ứng của những người biết chuyện.

**Narrative** cập nhật Story Thread.

**Chronicle** lưu sự kiện lịch sử.

Những thay đổi có tính bất biến bắt buộc phải được commit nhất quán. Các phản ứng phụ như tin đồn lan tới người ở xa có thể được xếp lịch sau đó.

Không phải tất cả cập nhật cần xảy ra trong cùng một transaction khổng lồ.

Nhưng không được tồn tại trạng thái đã commit mà NPC vừa được xác nhận chết lại vẫn còn hành động trong party.

---

<a id="m25-t065-s024"></a>

## PHẦN V — DEPENDENCY CYCLE AUDIT

<a id="m25-t065-s025"></a>

### 12. Dependency Cycle nguy hiểm nhất: Event ↔ Quest ↔ World Director

Các module trước từng mô tả:

- World State tạo Pressure.
- Event Engine tạo Event.
- Story Thread sinh Quest.
- World Director ưu tiên expose.
- Player Action cập nhật World State.

Đây là một **vòng phản hồi gameplay hợp lệ**.

Nhưng nếu triển khai không rõ trách nhiệm, nó có thể trở thành vòng gọi phần mềm nguy hiểm.

Ví dụ:

Quest cần event.

Event Engine hỏi World Director.

World Director hỏi Quest có gì để hiển thị.

Quest lại yêu cầu Event Engine sinh event.

Có thể dẫn tới event loop hoặc việc sinh sự kiện không có nguyên nhân.

<a id="m25-t065-s026"></a>

### 13. Cách sửa

Tách ba khái niệm:

**Eligible Event:** Một loại sự kiện đã đủ điều kiện có thể xảy ra.

**Committed Event:** Một sự kiện thực sự đã được xử lý và ghi vào World State.

**Exposed Information:** Một phần thông tin về sự kiện được phép tới player.

Luồng chuẩn:

```text
World State
    |
    v
Event Eligibility
    |
    v
Event Scheduler / Actor Action
    |
    v
Committed Event
    |
    v
Story Thread Update
    |
    v
Knowledge Propagation
    |
    v
World Director Exposure
    |
    v
Player
```

Director có thể yêu cầu danh sách eligible opportunities, nhưng không tự commit event.

Nếu một interaction chưa xảy ra, lựa chọn player mới là command kích hoạt xử lý trong Rule Engine.

<a id="m25-t065-s027"></a>

### 14. Dependency Cycle: Economy ↔ Travel ↔ Settlement

Economy cần tuyến vận chuyển.

Travel đọc chất lượng hạ tầng và tình trạng thị trường.

Settlement được cập nhật từ trade.

Nếu cả ba cập nhật lẫn nhau tức thời, có thể tạo vòng lặp:

Trade tăng → settlement tăng prosperity → route demand tăng → trade tăng → ...

Chỉ trong một tick, không có thời gian hoặc tài nguyên thực tế.

<a id="m25-t065-s028"></a>

#### Giải pháp

Sử dụng **Time-Bounded Simulation Steps**.

Ví dụ:

- Production tạo hàng trong kỳ.
- Trade phân phối trong giới hạn capacity.
- Settlement cập nhật indicators.
- Nhu cầu cho kỳ kế tiếp được tính từ trạng thái mới.

Các thay đổi không chạy vô hạn trong cùng một tick.

Một resource không thể di chuyển qua nhiều tuyến vượt thời gian đi lại chỉ vì có nhiều handler liên tiếp.

<a id="m25-t065-s029"></a>

### 15. Dependency Cycle: Social ↔ Knowledge ↔ Rumor

Một tin đồn có thể ảnh hưởng quan hệ.

Quan hệ ảnh hưởng quyết định chia sẻ tin.

Tin tiếp tục lan.

Đây là vòng phản hồi hợp lệ.

Rủi ro là cùng một rumor được truyền qua lại và mỗi lần lại tăng độ nổi tiếng vô hạn.

<a id="m25-t065-s030"></a>

#### Giải pháp

Mỗi Rumor Transmission có:

- Origin Event;
- Claim Version;
- Sender;
- Receiver;
- Transmission Time;
- Prior Exposure State.

Hệ thống cần kiểm tra một actor đã nhận phiên bản nào của claim.

Nghe lại cùng một lời kể không được mặc định tăng mức độ tin tưởng mãi mãi.

Nếu nhiều người cùng kể, đó có thể là một nguồn xác nhận bổ sung, nhưng cần xem họ có thực sự độc lập hay đều lấy tin từ một người.

<a id="m25-t065-s031"></a>

### 16. Dependency Cycle: Aging ↔ Profession ↔ Economy

Một NPC già đi.

Năng lực sản xuất thay đổi.

Household thu nhập thay đổi.

NPC chuyển nghề.

Nghề mới tác động sức khỏe và thời gian.

Đây cũng là vòng hợp lệ, nhưng phải được xử lý qua các bước thời gian và Life Event.

Không được để engine liên tục đổi nghề trong một ngày vì Utility Score của các nghề thay đổi sau mỗi phép tính.

<a id="m25-t065-s032"></a>

#### Giải pháp

Career Transition cần:

- eligibility;
- thời gian tối thiểu;
- social/professional constraints;
- commitment hoặc stability state;
- cooldown hợp lý từ hoàn cảnh.

Không phải một cooldown gamey cố định 30 ngày cho mọi người, mà là quy tắc phản ánh chi phí chuyển đổi nghề nghiệp thật.

---

<a id="m25-t065-s033"></a>

## PHẦN VI — HISTORICAL ANCHOR VS PLAYER AGENCY

<a id="m25-t065-s034"></a>

### 17. Đây là xung đột thiết kế nghiêm trọng nhất

GDD đang yêu cầu đồng thời:

**Điều A:** Những mốc lớn như sự hình thành Cổ Loa và kết thúc thời Âu Lạc phải tồn tại để giữ cấu trúc campaign.

**Điều B:** Player có thể giết NPC, phá hủy công trình, ngăn vận tải, thay đổi faction, tác động kinh tế và chọn con đường riêng.

Nếu player được tự do phá mọi điều kiện hình thành Historical Anchor nhưng Anchor vẫn bắt buộc xảy ra chính xác, engine chỉ còn hai lựa chọn sai:

- bí mật gian lận để Anchor xảy ra;
- hoặc phá vỡ trục lịch sử của campaign.

Hai điều không thể đồng thời được bảo đảm tuyệt đối nếu phạm vi can thiệp không giới hạn.

<a id="m25-t065-s035"></a>

### 18. Phương án thiết kế đề xuất

Phân Historical Anchors thành ba cấp.

<a id="m25-t065-s036"></a>

#### Hard Canon Anchor

Là sự kiện định nghĩa campaign.

Ví dụ:

- sự chuyển tiếp chính trị dẫn tới Âu Lạc;
- Cổ Loa trở thành trung tâm quan trọng;
- cuộc khủng hoảng cuối thời Âu Lạc.

Các điều kiện nền phải được xây ngay từ Campaign Design.

Những yếu tố không thể phá vỡ cấu trúc này cần được bảo vệ bằng **phạm vi tương tác được thiết kế trước**, không phải bất tử giả tạo.

<a id="m25-t065-s037"></a>

#### Flexible Historical Anchor

Sự kiện quan trọng nhưng hình thức và kết quả địa phương có thể khác.

Ví dụ:

- tuyến cung ứng tới Cổ Loa;
- hệ thống workshop phục vụ quân sự;
- quan hệ giữa hai cộng đồng;
- vị trí của một số nhân vật hư cấu;
- số người được cứu trong một biến cố.

Player có thể thay đổi đáng kể.

<a id="m25-t065-s038"></a>

#### Emergent Historical Event

Sự kiện hình thành hoàn toàn từ simulation.

Ví dụ:

- một settlement mới;
- sự nổi lên của một gia đình;
- một tuyến trade quan trọng;
- một cuộc tranh chấp được nhớ nhiều thế hệ.

Không cần biết trước kết quả.

<a id="m25-t065-s039"></a>

### 19. Không được dùng Historical Anchor để ghi đè hậu quả đã commit

Ví dụ player phá hủy một công trường phụ.

Nếu sự kiện được phép xảy ra và đã được commit:

- công trường thật sự bị hỏng;
- vật liệu đã mất;
- nhân công chịu ảnh hưởng;
- tiến độ bị chậm.

Nếu Cổ Loa vẫn phải được xây dựng, các faction phải tìm giải pháp hợp lệ:

- chuyển nguồn vật liệu;
- huy động lao động khác;
- điều chỉnh phương pháp;
- thay tiến độ;
- chấp nhận chi phí.

Nếu tất cả đường giải pháp đều không tồn tại, đó là lỗi thiết kế cần được phát hiện trước khi phát hành.

Không được cho Historical Controller tự dựng lại công trường trong một đêm.

<a id="m25-t065-s040"></a>

### 20. Historical Participation không phải Historical Control

Cần bổ sung một nguyên tắc rõ:

> **Player được tác động tới lịch sử trong phạm vi một con người có năng lực, quan hệ và nguồn lực thực tế; không có quyền điều khiển toàn bộ lịch sử chỉ vì là nhân vật được người chơi điều khiển.**

Một nông dân đơn độc không thể khiến hàng nghìn người ngừng xây thành chỉ bằng một câu hội thoại.

Một Thủ Lĩnh có mạng ảnh hưởng lớn có thể có khả năng tạo biến đổi mạnh hơn.

Agency phải tỷ lệ với:

- Capability;
- Social Leverage;
- Material Resources;
- Information;
- Timing;
- Allies;
- World Pressure.

Đây là cách bảo vệ cả tự do của người chơi lẫn sự đáng tin của Historical Campaign.

---

<a id="m25-t065-s041"></a>

## PHẦN VII — WORLD TRUTH, KNOWLEDGE & UI AUDIT

<a id="m25-t065-s042"></a>

### 21. Thông tin là một domain có quy tắc riêng

GDD đã xác định rất rõ:

World Truth khác Character Knowledge.

Tuy nhiên, Module 25 cần bổ sung quy tắc về **hiệu lực của tri thức khi ra quyết định**.

Một NPC chỉ được lập kế hoạch từ:

- điều họ trực tiếp chứng kiến;
- điều người khác truyền đạt;
- kiến thức đã học;
- suy luận mà họ có khả năng thực hiện;
- niềm tin và giả định cá nhân.

Không được để AI Decision Engine lấy dữ liệu từ một Global World State Query rồi vô tình biết:

- hung thủ thật;
- kho hàng bị giấu;
- tuyến quân bí mật;
- ai đang mang vật phẩm quan trọng.

<a id="m25-t065-s043"></a>

### 22. Knowledge Projection phải nằm trên mọi đường hiển thị và quyết định nhận thức

Có hai lớp cần tách.

**Game Rules:** Có thể cần World Truth để xác định hậu quả vật lý.

Ví dụ một kẻ giấu trong bụi vẫn tồn tại và có thể tấn công.

**Actor Decision:** Chỉ dùng Perceived State.

Ví dụ guard không được đuổi theo kẻ giấu nếu chưa phát hiện hoặc có thông tin hợp lệ.

Tương tự:

UI chỉ dùng Player/Character Knowledge Projection.

Không phải UI được thấy mọi actor có thật rồi chọn ẩn text.

Các index, search và notification cũng phải bị giới hạn nguồn dữ liệu từ đầu.

<a id="m25-t065-s044"></a>

### 23. Vấn đề Succession và ký ức của người chơi

Đây là một trường hợp biên đặc biệt.

Player đã điều khiển Mân và biết một bí mật.

Mân chết.

Player điều khiển Miên.

Miên chưa được truyền bí mật.

Nhưng người chơi ngoài đời vẫn nhớ.

<a id="m25-t065-s045"></a>

#### Quyết định đề xuất

Tách:

**Player Experience Archive:** Lưu trải nghiệm của người chơi qua nhiều protagonist.

**Active Character Knowledge:** Lưu thông tin mà character hiện tại có thể sử dụng theo cơ chế.

Nếu player nhớ vị trí một nơi và tự đi tới, hành động vật lý đó có thể được phép.

Nhưng Miên không được tự có:

- skill;
- quyền truy cập;
- technical knowledge;
- secret relationship;
- Dialogue Option cần nguồn chứng minh.

Cơ chế này cần kiểm thử đặc biệt để tránh vừa quá dễ meta-game vừa vô lý trong việc ngăn player thử những hành động hợp lệ.

<a id="m25-t065-s046"></a>

### 24. Knowledge không phải tài nguyên được sao chép không điều kiện

Một người kể cho người khác nghe một bí mật không đồng nghĩa người nhận hiểu hoàn toàn.

Có thể truyền:

- Claim;
- Observation;
- Instruction;
- Professional Knowledge;
- Location Knowledge.

Mỗi loại cần quy tắc khác nhau.

Ví dụ:

Mân nói:

> “Có một lối rừng phía bắc.”

Miên nhận thông tin về khả năng tồn tại của đường.

Miên không tự có Navigation Skill đủ để đi.

Tương tự một người thợ được kể công thức đúc không lập tức thành Master.

---

<a id="m25-t065-s047"></a>

## PHẦN VIII — CHARACTER GENERATION & LIFE HISTORY AUDIT

<a id="m25-t065-s048"></a>

### 25. Nguy cơ: sinh Attributes, Skills và Life History độc lập

Nếu engine random ba thành phần này độc lập:

- nhân vật 18 tuổi có kinh nghiệm của thợ bậc thầy;
- nhân vật chưa từng ra sông có Skill lái thuyền rất cao;
- nhân vật không có mentor nhưng sở hữu Technique cực hiếm.

Đây là lỗi thiết kế.

<a id="m25-t065-s049"></a>

#### Quyết định sửa

Character Generation cần sinh **Life History trước khi xác lập năng lực cuối cùng**.

Có thể có innate traits hoặc aptitude ở mức nền.

Nhưng Skills, Profession, social reputation và phần lớn Knowledge phải xuất phát từ trải nghiệm và môi trường.

Quy trình đề xuất:

**Birth Context → Aptitude → Formative Exposure → Life Events → Learned Knowledge & Skills → Current Condition → Viability.**

Không dùng một bộ random độc lập cho từng trường.

<a id="m25-t065-s050"></a>

### 26. Tuổi khởi đầu và campaign 20–25 năm

GDD cho phép player bắt đầu ở nhiều độ tuổi.

Nếu character bắt đầu ở tuổi 60, một campaign kéo dài 25 năm có thể khiến họ qua đời trước khi đến hồi cuối.

Đây không phải bug.

Nhưng có thể gây hai vấn đề:

- succession xảy ra quá sớm nếu player chưa có quan hệ;
- các cơ hội gameplay phù hợp tuổi cao chưa đủ phong phú.

<a id="m25-t065-s051"></a>

#### Giải pháp

Character Viability Gate không chỉ kiểm tra vài giờ đầu.

Nó cần kiểm tra **Continuity Viability**:

- character có các nguồn quan hệ hợp lý;
- có khả năng xây dựng di sản;
- có ít nhất một lối phát triển phù hợp thể trạng;
- hệ thống có khả năng tạo hoặc nhận biết successor theo thời gian;
- nếu chết trước khi tạo successor, có một kết thúc hợp lệ, không softlock.

Không được cho mọi character già sống bất thường chỉ để bảo đảm qua hết campaign.

<a id="m25-t065-s052"></a>

### 27. Character Class Emergence và Profession

Một xung đột có thể xảy ra nếu engine coi Class như tập hợp skill threshold cố định.

Ví dụ:

Thợ đúc có Skill đủ cao tự động trở thành Xảo Nhân.

Nhưng GDD đã xác định Class phải dựa trên identity, hành động dài hạn và sự công nhận.

<a id="m25-t065-s053"></a>

#### Giải pháp

Sử dụng mô hình:

**Candidate Evidence → Pattern Recognition → Milestone → Social/Personal Recognition → Class State.**

Skill đủ cao chỉ là một điều kiện có thể hỗ trợ.

Không được tự động convert nghề thành Class.

Một người có thể làm thợ cả đời nhưng không hình thành Class Xảo Nhân theo cách thiết kế của game.

<a id="m25-t065-s054"></a>

### 28. Training không thể tạo tài nguyên và thời gian miễn phí

Một nhân vật dành ba năm học nghề.

Engine cần xác định:

- họ học từ ai;
- mentor còn sống và đồng ý không;
- workshop có hoạt động không;
- có điều kiện thực hành không;
- có nguồn sinh kế không;
- có Life Event làm gián đoạn không.

Nếu không, Time Skip sẽ trở thành công cụ mua Skill miễn phí.

Downtime phải sử dụng cùng rule với hoạt động trực tiếp, chỉ khác độ phân giải simulation.

---

<a id="m25-t065-s055"></a>

## PHẦN IX — PARTY, COMPANION & AUTONOMY AUDIT

<a id="m25-t065-s056"></a>

### 29. Giới hạn Active Party

GDD đã khóa:

**Một party mặc định tối đa 4 nhân vật: protagonist + 3 companions.**

Thiết kế này phù hợp với combat theo lượt, đồng thời giữ companion đủ quan trọng về mặt quan hệ.

Không nên mở rộng party chỉ để giải quyết thiếu role.

Nếu một encounter bắt buộc có năm chuyên môn khác nhau, vấn đề nằm ở Encounter Design.

<a id="m25-t065-s057"></a>

### 30. Companion Control vs Autonomy

GDD cho phép player điều khiển phần lớn tactical action của companion.

Đồng thời companion có moral boundary và ý chí riêng.

Hai yêu cầu này cần một hợp đồng rõ.

<a id="m25-t065-s058"></a>

#### Đề xuất Command Contract

Mỗi chỉ thị có thể ở một trong bốn trạng thái:

**Accepted:** Companion đồng ý thực hiện.

**Conditional:** Cần điều kiện hoặc giải thích.

**Refused:** Companion từ chối vì lý do hợp lệ.

**Impossible:** Hành động vượt khả năng vật lý hoặc trạng thái hiện tại.

Ví dụ:

Player yêu cầu companion giết một người đã đầu hàng.

Nếu companion có Moral Boundary không cho phép:

Rule Engine từ chối hành động.

NPC không được bất ngờ thực hiện chỉ vì click đã thành công.

<a id="m25-t065-s059"></a>

### 31. Companion ở xa không được đóng băng

Off-Party Companions phải tiếp tục:

- nghề nghiệp;
- gia đình;
- quan hệ;
- sự kiện;
- tuổi tác;
- di chuyển.

Nhưng không thể mô phỏng Tier S cho mọi companion mỗi frame.

Dùng Relevant NPC Scheduling.

Các cam kết với player được giữ và xử lý theo thời gian.

Ví dụ companion hẹn sẽ gặp lại sau một mùa nhưng vì bệnh không tới được.

Đây là kết quả của event, không phải NPC teleport về điểm hẹn để bảo đảm player hài lòng.

---

<a id="m25-t065-s060"></a>

## PHẦN X — COMBAT, EQUIPMENT & INJURY AUDIT

<a id="m25-t065-s061"></a>

### 32. Combat Space và Exploration Space phải được phân biệt

Combat đã chọn:

**Zone + Local Position Slots, turn-based.**

Exploration dùng World/Regional/Local/Site Map.

Nếu hai hệ thống cùng gọi đối tượng là `Zone` mà không có phân loại, developer có thể tạo confusion.

<a id="m25-t065-s062"></a>

#### Chuẩn hóa

**WorldRegion:** Đại vùng.

**LocalArea:** Khu khám phá.

**EncounterSpace:** Không gian một encounter.

**TacticalZone:** Vùng chiến thuật trong encounter.

**PositionSlot:** Vị trí cục bộ trong TacticalZone.

Một LocalArea có thể tạo EncounterSpace.

Một TacticalZone không phải một Region trên World Map.

<a id="m25-t065-s063"></a>

### 33. Combat và Injury dùng cùng nguồn trạng thái

Combat không tự sở hữu một hệ vết thương riêng.

Combat Action tạo:

- hit result;
- impact;
- penetration;
- defense result;
- injury request.

Health Domain quyết định:

- Wound;
- Severity;
- Bleeding;
- Functional Limitation;
- Incapacitation;
- Death Risk.

Sau đó Combat đọc lại trạng thái Health mới.

Nhờ vậy cùng một thương tích có hậu quả nhất quán trong:

- trận chiến;
- travel;
- nghề nghiệp;
- downtime;
- chữa trị.

<a id="m25-t065-s064"></a>

### 34. Vitality khác Wounds

Module Equipment/Injury đã xác định Vitality ngắn hạn khác Wounds dài hạn.

Cần tránh hai bug:

**Bug A:** Vitality bằng 0 thì nhân vật tự động chết.

**Bug B:** Vitality còn nhiều thì nhân vật không thể có thương tích nghiêm trọng.

Hai điều đều trái GDD.

Combat UI cũng phải thể hiện sự khác biệt.

Một đòn xuyên nguy hiểm có thể gây Serious Wound ngay cả khi nhân vật chưa kiệt Vitality.

Ngược lại, một nhân vật kiệt sức có thể không bị thương nặng nhưng không tiếp tục chiến đấu hiệu quả.

<a id="m25-t065-s065"></a>

### 35. Equipment Condition và Economy

Item có:

- material;
- quality;
- condition;
- provenance;
- ownership.

Khi item hỏng, không được để Combat tự spawn một item mới.

Repair/Crafting Domain xử lý các thay đổi vật lý.

Asset Ledger xử lý quyền sở hữu.

Một thanh giáo có thể bị gãy cán.

Phần đầu giáo còn dùng được.

Các component phải được lưu đủ để tránh cả item cũ lẫn vật liệu salvage mới cùng tồn tại vô lý.

---

<a id="m25-t065-s066"></a>

## PHẦN XI — ECONOMY, COMMODITY & SURVIVAL AUDIT

<a id="m25-t065-s067"></a>

### 36. Resource Abstraction chưa hoàn toàn thống nhất

Survival sử dụng các đơn vị như Supply Days.

Economy sử dụng Commodity Lot.

Crafting sử dụng vật liệu và component.

Nếu thiếu quy tắc quy đổi, dễ xuất hiện:

- một đơn vị food biến thành nhiều Supply Days không hợp lý;
- một vật liệu vừa được dùng cho crafting vừa được bán;
- việc bảo quản tạo lương thực vô hạn;
- các nguồn lực giả được sinh khi chuyển abstraction.

<a id="m25-t065-s068"></a>

#### Quyết định đề xuất

**Commodity Lot là nguồn tài nguyên vật chất có thẩm quyền.**

**Supply Days là phép tính dẫn xuất** dựa trên:

- lượng thực phẩm;
- chất lượng;
- dinh dưỡng/khả năng sử dụng;
- kích thước party;
- điều kiện bảo quản;
- hoàn cảnh hành trình.

Không phải một kho tài nguyên độc lập có thể được tăng giảm tùy ý.

<a id="m25-t065-s069"></a>

### 37. Ví dụ quy đổi

Một party mang lương thực có giá trị sử dụng tương đương mười hai khẩu phần ngày theo quy tắc của game.

Với bốn người:

UI có thể hiển thị khoảng ba ngày supply, trước khi xét thêm các điều kiện khác.

Nếu một người rời party:

Số ngày supply tăng do ít người tiêu thụ hơn.

Nhưng lượng food thực tế không thay đổi.

Nếu đi qua vùng nóng ẩm hoặc hàng bị hỏng:

Usable Supply có thể giảm.

Đây là phép chiếu từ Economy/Inventory, không phải một loại item mới được tự sinh.

<a id="m25-t065-s070"></a>

### 38. Ownership phải là hệ thống thống nhất

Một vấn đề quan trọng là:

- Item có chủ;
- Commodity Lot có chủ;
- Property có quyền sử dụng;
- Workshop có người quản lý;
- Quest có thể tạo nghĩa vụ;
- Inheritance có thể chuyển tài sản.

Đề xuất **Asset & Rights Model** thống nhất, với những quyền khác nhau:

**Owner:** Chủ sở hữu.

**Holder/Custodian:** Người đang giữ.

**User:** Người có quyền sử dụng.

**Claimant:** Người đang đưa ra yêu sách.

**Controller:** Người có quyền quản lý một tài sản hoặc công trình.

Ví dụ:

Một cây thuyền thuộc nhà Triều.

Mân được mượn.

Mân đang giữ và sử dụng.

Không vì vậy thuyền trở thành tài sản cá nhân của Mân.

Khi Mân chết, thuyền không được tự động đưa vào Inheritance Case của Mân.

<a id="m25-t065-s071"></a>

### 39. Economy không cần mô phỏng toàn bộ thị trường như một ERP

Nếu cố tạo hệ quản lý từng giao dịch của hàng chục nghìn cư dân, chi phí có thể lớn hơn giá trị gameplay.

Đề xuất:

- Economy Aggregate cho sản xuất và tiêu thụ nền;
- Named Transaction cho giao dịch liên quan player hoặc Story Thread;
- Persistent Item cho vật phẩm quan trọng;
- Commodity Lot cho nguồn lực cần tracking;
- Statistical Production cho các hoạt động background.

Điều bắt buộc là aggregate phải bảo toàn mass/value constraints trong phạm vi mô hình và không tạo thành tựu từ hư không.

---

<a id="m25-t065-s072"></a>

## PHẦN XII — WORLD MAP, TRAVEL & ECOLOGY AUDIT

<a id="m25-t065-s073"></a>

### 40. World Map đã khóa 12 đại vùng nhưng topology chưa đủ chi tiết

GDD đã có:

- 12 vùng;
- 60 settlement fixed;
- khoảng 100–150 hamlet phụ;
- nhiều loại route;
- môi trường thay đổi;
- điều kiện theo mùa.

Đây là một khung tốt.

Nhưng chưa có dữ liệu topology thực tế cho:

- các nút;
- cạnh;
- river direction;
- route capacity;
- seasonal passability;
- travel time;
- cargo capacity;
- kiểm soát faction.

Nếu chưa có bảng route thật, nhiều module sẽ phải giả định địa lý theo cách riêng.

<a id="m25-t065-s074"></a>

#### Quyết định

**World Route Graph là nguồn sự thật duy nhất về kết nối và thời gian di chuyển chiến lược.**

Economy và Quest không tự tạo quãng đường riêng.

Travel, Trade, Faction Logistics và Knowledge Propagation đều tham chiếu cùng Route Graph.

<a id="m25-t065-s075"></a>

### 41. Knowledge về route khác trạng thái route

Một route có thể:

- đang hoạt động trong World State;
- nhưng player chưa biết;
- hoặc player từng biết nhưng thông tin đã cũ.

Vì vậy cần tách:

**RouteEdge:** Trạng thái thực.

**RouteKnowledge:** Thông tin actor biết.

**TravelPlan:** Kế hoạch dựa trên RouteKnowledge.

Nếu player chọn đi một tuyến từ tin cũ, engine xử lý dựa trên route thực, nhưng UI dự báo theo thông tin hiện có.

Đó là một trong những trường hợp kiểm thử quan trọng nhất.

<a id="m25-t065-s076"></a>

### 42. Ecology và Creature Encounter

Creature System đã xác định không có monster level và encounter vô cớ.

Cần khóa thêm:

**Population Pool tạo xác suất xuất hiện có căn cứ.**

**Encounter System instantiate cá thể chỉ khi cần.**

**Creature State cập nhật quần thể khi có hậu quả đáng kể.**

Ví dụ player săn một con thú trong quần thể lớn.

Điều này có thể giảm aggregate population nhỏ.

Nếu cá thể là một exceptional animal đã tồn tại lâu, phải cập nhật persistent identity và không cho nó xuất hiện lại ở một nơi khác.

---

<a id="m25-t065-s077"></a>

## PHẦN XIII — MẠCH LẠC, RITUAL & SUPERNATURAL AUDIT

<a id="m25-t065-s078"></a>

### 43. Ranh giới giữa sự thật supernatural và sự mơ hồ

GDD đã khóa:

Mạch Lạc là metaphysical foundation của game-canon.

Đồng thời nhiều sự kiện phải giữ sự mơ hồ giữa tự nhiên, tâm lý, xã hội và supernatural.

Hai quyết định không thực sự mâu thuẫn nếu tách:

**Internal Ontology:** Engine biết liệu Mạch có tham gia một event hay không, trong những trường hợp event đã được xác lập rõ.

**Player Epistemology:** Người chơi chỉ nhận được những dấu hiệu và nhận định hợp lệ.

Một sự kiện có thể có cơ chế Mạch thật nhưng nhân vật không biết.

Một sự kiện khác chỉ mang ý nghĩa tín ngưỡng mà không có supernatural effect.

Một Mythic Anchor có thể được thiết kế cố tình chưa khẳng định đầy đủ ontology, nhưng vẫn phải có các sự thật cơ học tối thiểu để engine xử lý nhất quán.

<a id="m25-t065-s079"></a>

### 44. Không cho Mạch sửa mọi quy luật khác

Một rủi ro lớn là dùng Mạch như lý do giải thích mọi bug hoặc tình huống vô lý.

Ví dụ:

- item biến mất;
- NPC đi xuyên bản đồ;
- character sống lại;
- quest đổi hung thủ;
- công trình tự hoàn thành.

Không được hợp thức hóa bằng:

> “Đó là sức mạnh bí ẩn của Mạch.”

Mạch Domain phải có:

- loại tác động cho phép;
- điều kiện;
- giới hạn;
- chi phí/hậu quả;
- event handler;
- quy tắc quan sát.

Nó là một subsystem trong engine, không phải ngoại lệ vô hạn đối với engine.

<a id="m25-t065-s080"></a>

### 45. Ritual có hai lớp hiệu lực riêng

**Social/Ritual Meaning:** Có thể thay đổi quan hệ, niềm tin, uy tín và cách cộng đồng hành động.

**Supernatural/Mạch Effect:** Chỉ xảy ra khi quy tắc Mạch và điều kiện thực tế thỏa mãn.

Một nghi lễ có thể thành công về mặt xã hội mà không tạo hiệu ứng Mạch.

Ngược lại một nghi lễ có thể tác động Mạch nhưng cộng đồng không nhận ra.

Đây là cơ chế cần được hỗ trợ bằng những kết quả tách biệt.

---

<a id="m25-t065-s081"></a>

## PHẦN XIV — CHUẨN HÓA EVENT, STATE & KNOWLEDGE

<a id="m25-t065-s082"></a>

### 46. Sáu thực thể dễ bị nhầm lẫn

| Thuật ngữ | Định nghĩa code | Có thay World State? |
|---|---|---|
| Command | Yêu cầu hành động của player/NPC/system | Chưa |
| Domain Event | Sự kiện đã được xác lập bởi rule | Có, hoặc là kết quả của commit |
| Observation | Một actor nhận biết dấu hiệu | Cập nhật Knowledge, không nhất thiết đổi physical state |
| Claim | Một actor phát biểu về điều gì đó | Cập nhật thông tin/quan hệ theo ngữ cảnh |
| Story Thread | Vấn đề hoặc chuỗi hậu quả còn liên quan | Cập nhật narrative state |
| Quest | Góc nhìn của player lên khả năng can thiệp | Không tự tạo physical truth |

Bảng này phải nằm trong Technical Glossary chung.

<a id="m25-t065-s083"></a>

### 47. Không được nhập Claim vào Event Truth

Một NPC nói:

> “Có người phá cầu.”

Đó là Claim.

Nếu engine dùng Claim này để cập nhật `BridgeCause = Sabotage`, kiến trúc đã sai.

Chỉ khi có event, evidence hoặc một resolution rule xác lập nguyên nhân, World Truth mới thay đổi.

<a id="m25-t065-s084"></a>

### 48. Không để UI gây ra gameplay event ngoài Command

Đọc một mục Journal không được tự kích hoạt:

- NPC xuất hiện;
- faction đổi quyết định;
- quest chuyển trạng thái;
- thời gian trôi;
- rumor lan.

Nếu người chơi chọn một hành động như “Gửi người báo tin”, đó là Command hợp lệ và có thể tạo event.

Nhưng thao tác đọc hồ sơ không phải một hành động trong thế giới trừ khi gameplay thiết kế rõ một bối cảnh đặc biệt, chẳng hạn đọc tài liệu thực sự mất thời gian và nhân vật chủ động thực hiện hành động ấy.

---

<a id="m25-t065-s085"></a>

## PHẦN XV — TECHNICAL ARCHITECTURE RECOMMENDATION

<a id="m25-t065-s086"></a>

### 49. Có nên sử dụng Microservices không?

Đối với một game đơn người chơi, tôi **không khuyến nghị bắt đầu với microservices**.

Một kiến trúc gồm:

- Character Service;
- Economy Service;
- Quest Service;
- Faction Service;
- Knowledge Service;
- Travel Service;

được triển khai thành các process độc lập sẽ làm tăng độ phức tạp không cần thiết.

Những vấn đề phát sinh:

- trạng thái đồng bộ;
- message ordering;
- transaction phân tán;
- latency;
- serialization;
- debugging;
- deployment;
- quản lý version.

Trong khi tất cả domain đều cần truy cập nhanh và nhất quán tới World State.

<a id="m25-t065-s087"></a>

### 50. Modular Monolith

Đề xuất:

**Một application runtime có các domain module được phân ranh giới rõ.**

Các module có thể:

- dùng chung process;
- sử dụng typed interfaces;
- trao đổi Domain Event;
- truy cập World State qua repository/aggregate phù hợp;
- được unit test độc lập;
- chạy headless không cần UI.

Không có nghĩa viết một class `GameManager` chứa toàn bộ logic.

Một Modular Monolith tốt vẫn có kiến trúc phân lớp và module rõ ràng.

<a id="m25-t065-s088"></a>

### 51. Một command processor có thẩm quyền

Đối với MVP, nên sử dụng một **Authoritative Simulation Executor** để điều phối những thay đổi gameplay theo thứ tự xác định.

Các subsystem có thể tính toán độc lập khi an toàn, nhưng kết quả có ảnh hưởng World State phải được commit theo quy tắc.

Điều này giảm đáng kể nguy cơ race condition và nondeterministic ordering.

Sau khi có benchmark, mới cân nhắc chạy song song những phần aggregate simulation không phụ thuộc trực tiếp.

<a id="m25-t065-s089"></a>

### 52. Domain Transaction

Ví dụ:

Player giao một món đồ.

Transaction cần bảo đảm:

- người giao đang giữ hợp lệ;
- người nhận có thể nhận;
- vật phẩm được chuyển một lần;
- Inventory không bị duplicate;
- quyền sở hữu được cập nhật đúng khi đây là giao dịch chuyển quyền;
- event được ghi.

Nếu một phần không hợp lệ, command không được commit dở dang.

Đây là invariant cấp code.

<a id="m25-t065-s090"></a>

### 53. Event Journal & Snapshot

Giữ phương án của Module 24:

**Authoritative Current State + Critical Event Journal + Periodic Snapshot.**

Không event-source từng thay đổi nhỏ chỉ vì tên game là Living World.

Event Journal phục vụ:

- audit;
- historical trace;
- debugging;
- replay;
- narrative history.

Current State phục vụ gameplay runtime.

Snapshot phục vụ save/load nhanh.

Các read projections có thể được dựng lại từ nguồn dữ liệu tương ứng.

Với dữ liệu cực kỳ quan trọng như ownership, death và historical events, cần log đầy đủ hơn các biến nền mang tính thống kê.

---

<a id="m25-t065-s091"></a>

## PHẦN XVI — DATA MODEL TỐI THIỂU

<a id="m25-t065-s092"></a>

### 54. Các entity nền tảng

Đề xuất schema lõi ở mức logical.

```text
World
WorldClock
EntityRegistry
Character
Household
Settlement
Region
RouteEdge
Faction
Relationship
KnowledgeRecord
WorldEvent
EventTemplate
StoryThread
Item
CommodityLot
AssetRight
WorldSnapshot
HistoricalAnchor
```

Các bảng/aggregate chuyên sâu như Wound, Technique, Ritual, CreaturePopulation và Workshop có thể bổ sung theo domain.

Không cần biến tất cả thành bảng SQL ngay trong GDD.

<a id="m25-t065-s093"></a>

### 55. Stable Identity

Một đối tượng phải giữ ID xuyên suốt vòng đời.

Ví dụ:

`character_vac_man_001`

Khi nhân vật:

- đổi nghề;
- chuyển quê;
- già đi;
- gia nhập faction;
- chết;
- trở thành đối tượng được nhắc trong truyền thuyết,

ID vẫn giữ nguyên.

Với NPC được instantiate từ background simulation, ID phải được cấp và persist trước khi một event quan trọng tham chiếu họ.

Không được chỉ dựa trên tên hiển thị vì NPC có thể trùng tên hoặc đổi tên.

<a id="m25-t065-s094"></a>

### 56. Entity Lifecycle

Đề xuất phân biệt:

**Potential:** Một entity có thể được tạo từ aggregate nhưng chưa tồn tại như cá thể trong committed world.

**Materialized:** Entity có ID và state cụ thể.

**Active:** Đang tham gia simulation chi tiết.

**Dormant:** Tồn tại nhưng xử lý ở LOD thấp.

**Deceased/Destroyed:** Không còn hoạt động, nhưng lịch sử vẫn tồn tại.

**Archived:** Được giữ để tham chiếu Chronicle, ownership và narrative.

Không được xóa vĩnh viễn entity quan trọng chỉ vì nhân vật đã chết hoặc công trình đã bị phá.

---

<a id="m25-t065-s095"></a>

## PHẦN XVII — SAVE, REPLAY & VERSION AUDIT

<a id="m25-t065-s096"></a>

### 57. Một World Seed không đủ

Module 24 đã xác định điều này.

Module 25 đề xuất save metadata tối thiểu:

- World Seed;
- Ruleset Version;
- Content Pack Version;
- Simulation Version;
- World Time;
- RNG State/Counters;
- Scheduled Events;
- Active Character ID;
- Current Snapshot;
- Critical Event Journal;
- Knowledge State;
- Historical Anchor State.

<a id="m25-t065-s097"></a>

### 58. Same Seed + Same Commands

Trong cùng phiên bản logic và cùng initial state:

**Same Seed + Same Command Sequence + Same Event Ordering → Same Authoritative World State.**

Đây là mục tiêu determinism.

Không nhất thiết phải tái hiện chính xác từng animation frame hoặc vị trí hạt bụi.

Nhưng các gameplay events quan trọng phải giống nhau.

<a id="m25-t065-s098"></a>

### 59. Version Migration

Khi phát hành patch, có thể thay đổi:

- combat formula;
- economy balance;
- AI utility;
- event template;
- content pack.

Không được mặc định replay toàn bộ 10 năm quá khứ với ruleset mới rồi thay đổi lịch sử của player.

Những committed outcomes phải được bảo toàn hoặc migrate bằng cơ chế có chủ đích.

Việc thay đổi rule tác động tới các event tương lai.

Nếu phải sửa lỗi dữ liệu lịch sử, cần một migration/compensation process rõ và có test.

---

<a id="m25-t065-s099"></a>

## PHẦN XVIII — SCOPE FEASIBILITY AUDIT

<a id="m25-t065-s100"></a>

### 60. Khối lượng GDD hiện tại rất lớn

Một hệ thống có:

- 12 đại vùng;
- 60 settlement cố định;
- hơn một trăm hamlet;
- hàng nghìn NPC;
- nhiều hệ nghề nghiệp;
- 11 Class;
- nhiều domain simulation;
- sáu hồi lịch sử;
- succession;
- myth formation;

không phải phạm vi nhỏ.

Điểm nguy hiểm không nằm chỉ ở số lượng content.

Nó nằm ở **số quan hệ giữa các content domain**.

Một NPC trong quest có thể đồng thời là:

- thành viên family;
- worker;
- trader;
- faction member;
- rumor source;
- witness;
- companion;
- heir.

Càng nhiều vai trò, số trạng thái cần validate càng lớn.

<a id="m25-t065-s101"></a>

### 61. Không lấy số lượng rule combinations làm thước đo tiến độ

Một triệu tổ hợp có thể được sinh từ vài chục trường dữ liệu.

Nhưng nếu mọi kết quả đều chỉ là:

> NPC A cần item X từ location Y,

thì game vẫn nông.

Metric cần ưu tiên:

- tỷ lệ event có nguyên nhân hợp lệ;
- số loại quyết định thực sự khác nhau;
- mức độ hậu quả dài hạn;
- độ đa dạng của hành động theo Profession;
- số chuỗi sự kiện có ý nghĩa;
- khả năng người chơi hiểu vì sao kết quả xảy ra.

<a id="m25-t065-s102"></a>

### 62. Rủi ro của Social Graph

Với vài nghìn NPC, nếu lưu mọi cặp NPC đều có quan hệ chi tiết, kích thước dữ liệu có thể tăng theo bình phương số người.

Đề xuất **Sparse Relationship Graph**:

- lưu những quan hệ có ý nghĩa;
- quan hệ gia đình được tạo từ Household Graph;
- quan hệ cộng đồng thông thường có thể suy ra từ membership và proximity;
- không tạo explicit edge cho mọi cặp người chỉ vì họ cùng sống trong vùng.

Một NPC có thể biết nhiều người ở mức xã giao nhưng không cần lưu hàng trăm vector tâm lý chi tiết.

Chỉ các quan hệ đáng kể mới được nâng lên Tier cao.

<a id="m25-t065-s103"></a>

### 63. Rủi ro của Content Explosion

Nếu mỗi Quest Template cần:

- 5 vùng;
- 6 nghề;
- 4 personalities;
- 3 outcomes;
- 2 time phases;

thì việc viết riêng toàn bộ các biến thể có thể trở nên không khả thi.

Giải pháp là:

**Shared Mechanical Resolution + Contextual Content Variants + Authored High-Impact Exceptions.**

Ví dụ công việc sửa cầu dùng một Rule Handler chung.

Nhưng:

- vật liệu;
- văn hóa;
- người sở hữu;
- political consequences;
- thời tiết;
- loại địa hình

thay đổi theo context.

Những tình huống lớn có thể được authored riêng.

Không nên tự động hóa tất cả và cũng không nên viết tay tất cả.

---

<a id="m25-t065-s104"></a>

## PHẦN XIX — RISK REGISTER

<a id="m25-t065-s105"></a>

### 64. Phân cấp rủi ro

**P0 — Architectural Blocker:** Nếu không xử lý, có thể làm sai toàn bộ mô hình.

**P1 — Major Design Risk:** Không nhất thiết chặn prototype đầu tiên nhưng phải xử lý trước khi mở rộng.

**P2 — Production/Content Risk:** Có thể hoãn đến giai đoạn phù hợp.

<a id="m25-t065-s106"></a>

### 65. Risk Register tổng hợp

| ID | Rủi ro | Mức | Hướng xử lý |
|---|---|---|---|
| AR-01 | Historical Anchor xung đột với Player Agency | P0 | Anchor Contract, authored causal scenarios |
| AR-02 | Nhiều domain cùng ghi một state | P0 | Data Ownership + Command Processor |
| AR-03 | Quest/Director/Event sinh event lặp hoặc vô căn cứ | P0 | Tách eligibility, commit và exposure |
| AR-04 | Knowledge leakage vào NPC AI hoặc UI | P0 | Knowledge Projection & scoped queries |
| AR-05 | Save/replay không deterministic | P0 | Seed, version, ordering, RNG state, replay tests |
| AR-06 | Succession làm mất hoặc tự nhân bản tài sản/tri thức | P0 | Transfer rules và inheritance state |
| AR-07 | Time Skip tạo trạng thái trái nhân quả | P0 | Event Scheduler, validation, simulation LOD |
| AR-08 | Economy/Survival/Crafting nhân đôi tài nguyên | P0 | Commodity authority và conversion rules |
| AR-09 | Social Graph và NPC planner không scale | P1 | Sparse graph, LOD, event-driven updates |
| AR-10 | Noncombat run không thể hoàn thành campaign | P1 | Multi-role Historical Anchor design |
| AR-11 | Profession bị biến thành nút wait | P1 | Activity Loop quality gate |
| AR-12 | Content procedural lặp và thiếu ý nghĩa | P1 | Offline review, novelty tests |
| AR-13 | Mạch trở thành ngoại lệ cho mọi rule | P1 | Supernatural domain contract |
| AR-14 | Hệ thống quá phức tạp đối với UI | P1 | Knowledge-centric information architecture |
| AR-15 | 12 vùng làm chi phí content vượt khả năng | P1 | Vertical slice và staged region expansion |
| AR-16 | NPC hành động sai vì dùng thông tin toàn tri | P1 | Perceived State model |
| AR-17 | GDD historical/game-canon bị trộn | P1 | Historical Research tagging |
| AR-18 | Combat tactical zone không mapping đúng LocalArea | P2 | Spatial taxonomy |
| AR-19 | Myth retelling thiếu bản sắc địa phương | P2 | Content templates và cultural validation |
| AR-20 | UI lượng thông tin quá lớn khi Time Skip | P2 | Relevance filtering |
| AR-21 | Performance của world simulation chưa biết | P1 | Headless benchmarks |
| AR-22 | Patching làm đổi kết quả quá khứ | P1 | Version migration & committed history |

Danh sách này là register ban đầu.

Khi bắt đầu lập trình, mỗi rủi ro cần có owner, tình trạng, test case và tiêu chí đóng.

---

<a id="m25-t065-s107"></a>

## PHẦN XX — MVP VS VERTICAL SLICE

<a id="m25-t065-s108"></a>

### 66. Phân biệt hai khái niệm

**MVP** trả lời:

> Sản phẩm nhỏ nhất có đủ giá trị gameplay để thử nghiệm giả thuyết cốt lõi là gì?

**Vertical Slice** trả lời:

> Một phần nhỏ của game có thể chứng minh đầy đủ trải nghiệm và sự kết nối giữa các hệ thống đến đâu?

Với dự án này, tôi đề xuất ưu tiên **Technical Vertical Slice có thể chơi**, sau đó mới mở rộng thành MVP.

Không nên bắt đầu bằng một màn hình menu sinh NPC và một bộ combat không nối với World Simulation.

Cũng không nên cố xây toàn bộ game rồi mới kiểm tra gameplay loop.

<a id="m25-t065-s109"></a>

### 67. Mục tiêu Vertical Slice

Phải chứng minh được:

> **Một nhân vật có xuất thân và nghề nghiệp thật có thể trải nghiệm một biến cố phát sinh từ World State, đưa ra lựa chọn, thay đổi thế giới, chứng kiến hậu quả theo thời gian và để lại thông tin/di sản hợp lệ.**

Câu này nên là Acceptance Goal chính.

Nếu Vertical Slice không chứng minh được điều đó, bổ sung nhiều nội dung hơn sẽ không giải quyết nguyên nhân kiến trúc.

---

<a id="m25-t065-s110"></a>

## PHẦN XXI — PHẠM VI VERTICAL SLICE ĐỀ XUẤT

<a id="m25-t065-s111"></a>

### 68. Thế giới thử nghiệm

**Vùng chính:** Tam Giang.

**Settlement:** Một settlement trung tâm như Bến Vạc và hai điểm cộng đồng lân cận.

**Địa điểm phụ:** Một bến, một khu đất sản xuất, một tuyến rừng/bờ sông, một khu vực nguy hiểm hoặc site.

**Route:** Một mạng nhỏ gồm tuyến sông và đường bộ.

**Thời tiết/môi trường:** Có ít nhất một biến động mực nước tác động route.

Không cần xây ngay 12 vùng.

Nhưng data model phải cho phép mở rộng lên 12 vùng mà không thay kiến trúc.

<a id="m25-t065-s112"></a>

### 69. NPC và Household

Đề xuất cho Vertical Slice:

- một số household có thành viên và nghề nghiệp rõ;
- nhóm NPC có social relationships;
- một vài character có Story Thread;
- một số background residents được aggregate;
- một companion có autonomy thực sự;
- một mentor hoặc expert nghề nghiệp.

Quy mô prototype cần đủ để kiểm tra:

- gia đình;
- sở hữu;
- nghề;
- quan hệ;
- động cơ;
- sự kiện;
- tin đồn.

Không cần hàng trăm NPC nếu chỉ một số ít thật sự được mô phỏng có chiều sâu.

<a id="m25-t065-s113"></a>

### 70. Profession trong Vertical Slice

Tôi đề xuất ba hoạt động đủ khác biệt:

**River Transport:** Có route, weather, cargo và negotiation.

**Basic Craft/Repair:** Có material, tool, time, quality và ownership.

**Medical/Treatment:** Có injury, diagnosis, resource và recovery.

Ba hoạt động này kiểm tra nhiều domain cùng lúc.

Chúng cũng cho phép player tham gia bằng các năng lực không tập trung vào combat.

<a id="m25-t065-s114"></a>

### 71. Combat trong Vertical Slice

Một encounter theo Zone + Local Position Slots.

Cần hỗ trợ:

- ít nhất vài tactical zones;
- vị trí và cover;
- Major/Minor Action;
- Reaction;
- Stamina;
- morale/retreat;
- vết thương có ảnh hưởng ngoài combat;
- objective không bắt buộc giết hết.

Ví dụ:

Giữ đường qua bến đủ lâu để cứu người đang bị thương.

Cùng encounter đó có thể được giải bằng chiến đấu, dùng địa hình hoặc tìm cách rút lui hợp lý.

<a id="m25-t065-s115"></a>

### 72. Dungeon & Exploration

Một site nhỏ có:

- nhiều lối tiếp cận;
- thông tin không đầy đủ;
- environmental hazard;
- item hoặc evidence;
- khả năng đi vào trước khi nhận Quest;
- trạng thái persistent sau khi rời.

Mục tiêu không phải kiểm tra số lượng phòng.

Mục tiêu là xác nhận World State và Dungeon State không bị reset khi player quay lại.

<a id="m25-t065-s116"></a>

### 73. Story Threads

Đề xuất ba loại:

**Personal Thread:** Một thành viên household gặp vấn đề.

**Economic/Environmental Thread:** Tuyến thuyền hoặc nguồn lực bị ảnh hưởng.

**Investigation Thread:** Player nghe lời kể không chắc chắn về một sự kiện.

Chúng có thể liên kết nhưng không cần biến thành một đại âm mưu.

Các thread cần hỗ trợ:

- discovered;
- ignored;
- resolved;
- transformed;
- delayed consequence.

<a id="m25-t065-s117"></a>

### 74. Thời gian và Legacy

Vertical Slice không cần trải nghiệm 25 năm bằng tay.

Nhưng cần có một **accelerated simulation test** để kiểm tra:

- NPC già đi;
- nghề nghiệp phát triển;
- household thay đổi;
- một công trình tồn tại sau nhiều năm;
- character death;
- successor hợp lệ;
- Knowledge Transfer;
- Chronicle.

Nếu một character chết sau khi để lại một vật phẩm, hệ thống phải xử lý đúng quyền sở hữu và các nhân vật liên quan.

---

<a id="m25-t065-s118"></a>

## PHẦN XXII — VERTICAL SLICE SCENARIO XUYÊN HỆ THỐNG

<a id="m25-t065-s119"></a>

### 75. Kịch bản: “Bến qua mùa nước”

Đây là một kịch bản kiểm thử đề xuất, không phải quest canonical bắt buộc.

<a id="m25-t065-s120"></a>

#### Trạng thái ban đầu

Bến Vạc có một tuyến vận tải quan trọng.

Một household sở hữu thuyền.

Nguồn food dự trữ đang giảm.

Mực nước tăng.

Một thuyền có tình trạng hỏng nhẹ.

Player có thể có nghề vận tải, chữa bệnh hoặc sửa chữa.

<a id="m25-t065-s121"></a>

#### Nhánh A — Player sửa thuyền

Cần:

- access tới thuyền;
- vật liệu;
- tool;
- thời gian;
- Skill phù hợp.

Nếu sửa tốt:

thuyền hoạt động ổn định.

Nếu sửa tạm:

thuyền hoạt động nhưng có nguy cơ hỏng tiếp.

Owner phản ứng theo hợp đồng và quan hệ.

<a id="m25-t065-s122"></a>

#### Nhánh B — Player tìm phương án vận tải khác

Có thể thương lượng với một household khác.

Điều này sử dụng:

- Dialogue;
- Social Graph;
- Economy;
- Asset Rights;
- Route Capacity.

Nếu thành công:

cargo đi qua tuyến khác.

Nếu không:

có thể trễ giao hàng.

<a id="m25-t065-s123"></a>

#### Nhánh C — Player không can thiệp

Thời gian tiếp tục.

Chủ thuyền có thể tự thuê người sửa hoặc chấp nhận rủi ro.

Economy xử lý mức độ chậm hàng.

World Director có thể expose hậu quả nhưng không được quyết định người nào phải chết.

<a id="m25-t065-s124"></a>

#### Nhánh D — Tai nạn xảy ra

Nếu state và rule tạo ra tai nạn:

- người có mặt có thể bị thương;
- cargo có thể mất;
- tài sản có thể hỏng;
- NPC có thể cần cứu;
- quest và rumor phát sinh.

Nếu player chữa trị:

Health Domain xử lý kết quả.

Nếu người chết:

Life/Household/Inheritance xử lý.

<a id="m25-t065-s125"></a>

#### Hậu quả sau một năm

World Simulation kiểm tra:

- tuyến có còn sử dụng;
- household còn tồn tại thế nào;
- thuyền đã được sửa tiếp hay chưa;
- người bị thương có hồi phục;
- các mối quan hệ thay đổi;
- player được nhớ tới ra sao.

Nếu toàn bộ kịch bản hoạt động nhất quán, đó là minh chứng mạnh rằng các domain có thể vận hành cùng nhau.

---

<a id="m25-t065-s126"></a>

## PHẦN XXIII — MẪU END-TO-END TEST BẮT BUỘC

<a id="m25-t065-s127"></a>

### 76. Test A — NPC chết giữa Quest

**Điều kiện:**

NPC A là requester.

Quest đang active.

NPC A chết vì một sự kiện khác.

**Kỳ vọng:**

- Life State chuyển Dead.
- NPC không tiếp tục hoạt động.
- Quest không crash.
- Story Thread xác định hướng tiếp diễn hợp lý.
- Family/Property được xử lý.
- Player chỉ biết cái chết khi có nguồn.
- Chronicle cập nhật đúng.

**Fail nếu:** NPC chết nhưng tiếp tục đưa dialogue hoàn thành Quest như đang sống.

<a id="m25-t065-s128"></a>

### 77. Test B — Companion từ chối lệnh

**Điều kiện:**

Companion có Moral Boundary phản đối giết người đầu hàng.

Player chọn lệnh tương ứng.

**Kỳ vọng:**

- Action không được thực thi nếu không có điều kiện thay đổi hợp lệ.
- Companion giải thích hoặc thể hiện sự từ chối.
- Combat State không phát sinh damage trái quy tắc.
- Relationship có thể thay đổi tùy phản ứng player.

**Fail nếu:** UI báo từ chối nhưng combat vẫn trừ HP mục tiêu.

<a id="m25-t065-s129"></a>

### 78. Test C — Route Knowledge đã cũ

**Điều kiện:**

Player biết một tuyến đi qua cầu.

Cầu đã bị phá trong World State nhưng tin chưa tới.

**Kỳ vọng:**

- Map vẫn hiển thị thông tin cũ với freshness.
- Travel Planner không biết chính xác cầu hỏng.
- Khi tới nơi, player phát hiện tình trạng thật.
- Knowledge cập nhật.
- Route Graph không tự mở lại.

**Fail nếu:** UI tự báo cầu hỏng ngay lập tức không có nguồn.

<a id="m25-t065-s130"></a>

### 79. Test D — Time Skip và người có lời hứa

**Điều kiện:**

Player có một lời hứa tới hạn trong mùa tới.

Player chọn nghỉ hai năm.

**Kỳ vọng:**

- UI cho biết deadline đã biết trước khi xác nhận.
- Thời gian trôi thật.
- Promise được đánh giá đúng hạn.
- NPC có phản ứng hợp lệ.
- Player không nhận thông tin về những phản ứng ở xa nếu chưa có nguồn.

**Fail nếu:** Lời hứa nằm nguyên ở trạng thái Pending sau hai năm mà không có giải thích.

<a id="m25-t065-s131"></a>

### 80. Test E — Successor không thừa kế tri thức

**Điều kiện:**

Mân biết nơi giấu một vật phẩm.

Mân chết.

Miên trở thành successor nhưng chưa được truyền tin.

**Kỳ vọng:**

- Miên không có Location Knowledge.
- Player Archive có thể lưu trải nghiệm cũ.
- UI in-world không tự đánh dấu nơi giấu.
- Nếu player tới địa điểm bằng hành động hợp lệ, hệ thống vẫn xử lý bình thường.
- Các kỹ năng hoặc quyền truy cập của Mân không được copy sang Miên.

**Fail nếu:** Miên nhận toàn bộ Journal và Secret Flags của Mân.

<a id="m25-t065-s132"></a>

### 81. Test F — Commodity Conservation

**Điều kiện:**

Một household giao một phần food cho trader.

Trader chuyển hàng tới settlement khác.

**Kỳ vọng:**

- Stock ở bên giao giảm đúng.
- Stock bên nhận tăng đúng, sau khi trừ hao hụt/tiêu thụ có quy tắc.
- Cargo không xuất hiện đồng thời ở hai vị trí.
- Supply Days được tính lại từ inventory thực.

**Fail nếu:** Giao dịch tạo thêm food ngoài nguồn hợp lệ.

<a id="m25-t065-s133"></a>

### 82. Test G — Historical Anchor bị ảnh hưởng

**Điều kiện:**

Một nguồn vật liệu của Cổ Loa bị gián đoạn do player action.

**Kỳ vọng:**

- Material availability giảm.
- Production/Construction chịu tác động.
- Faction tìm giải pháp từ những nguồn lực hợp lệ.
- Historical Anchor tiếp tục theo các phương án đã authored.
- Không có vật liệu được sinh miễn phí để lấp chỗ thiếu.

**Fail nếu:** Công trình hoàn thành đúng hạn bất chấp resource constraints và không có nguyên nhân thay thế.

<a id="m25-t065-s134"></a>

### 83. Test H — Replay

**Điều kiện:**

Cùng World Seed, Ruleset Version, initial Snapshot và Command Sequence.

**Kỳ vọng:**

- Committed Event Sequence giống nhau.
- World State Hash giống nhau.
- RNG decisions tái hiện.
- Không có khác biệt vì thứ tự iteration hoặc thời gian máy.

**Fail nếu:** Một NPC thay nghề hoặc chết khác đi chỉ vì reload save.

---

<a id="m25-t065-s135"></a>

## PHẦN XXIV — PERFORMANCE TEST STRATEGY

<a id="m25-t065-s136"></a>

### 84. Không tự đặt KPI performance khi chưa có prototype

Đối với một game sử dụng world simulation lớn, chúng ta cần benchmark trên thiết bị mục tiêu.

Chưa nên khóa các con số về:

- số NPC tối đa trên mỗi CPU core;
- thời gian simulation một năm;
- dung lượng save 25 năm;
- thời gian load;
- mức RAM;
- số Event Instance mỗi giây.

Các số liệu đó phụ thuộc trực tiếp vào data model và thuật toán được chọn.

Nhưng có thể khóa **phương pháp kiểm thử** ngay từ GDD.

<a id="m25-t065-s137"></a>

### 85. Benchmark Scenarios

<a id="m25-t065-s138"></a>

#### Scenario 1 — Một settlement

Đo:

- NPC decision;
- household updates;
- economy;
- event eligibility;
- relationship propagation.

<a id="m25-t065-s139"></a>

#### Scenario 2 — Một vùng

Đo:

- nhiều settlement;
- route graph;
- trade;
- ecology;
- faction;
- knowledge.

<a id="m25-t065-s140"></a>

#### Scenario 3 — Toàn thế giới aggregate

Đo:

- 12 vùng;
- 60 settlement;
- background hamlets;
- population simulation;
- year transitions.

<a id="m25-t065-s141"></a>

#### Scenario 4 — Time Skip dài

Đo:

- simulation nhiều năm;
- event scheduling;
- household transitions;
- Chronicle generation;
- save size.

<a id="m25-t065-s142"></a>

#### Scenario 5 — NPC/Story burst

Đo khi:

- nhiều event cùng xảy ra;
- một faction biến động;
- chiến tranh tạo nhiều thương tích/tử vong;
- tin tức lan qua Social Graph.

Kết quả benchmark dùng để quyết định mức độ LOD và batch processing.

---

<a id="m25-t065-s143"></a>

## PHẦN XXV — CONTENT SCOPE STRATEGY

<a id="m25-t065-s144"></a>

### 86. Không được cắt những hệ thống tạo bản sắc

Một số hệ thống là đặc trưng của dự án, không nên loại bỏ dù khó.

**Bắt buộc giữ:**

- character history có nhân quả;
- noncombat progression;
- World State độc lập;
- relationship;
- Knowledge/Truth separation;
- persistent consequences;
- time/aging;
- succession;
- Historical Campaign;
- Mạch Lạc ở mức bản sắc.

Nếu cắt những phần này, game có nguy cơ biến thành CRPG lịch sử thông thường.

<a id="m25-t065-s145"></a>

### 87. Có thể giảm độ sâu ở phiên bản đầu

Một số thứ có thể triển khai theo mức độ.

**Economy:** Bắt đầu với ít commodity và production chains.

**Faction:** Bắt đầu với hai hoặc ba nhóm lợi ích địa phương.

**World Director:** Bắt đầu với exposure priority đơn giản, không cần một thuật toán adaptive phức tạp.

**NPC Psychology:** Bắt đầu với một số Trait, Goal và Boundary có ý nghĩa.

**Myth Evolution:** Bắt đầu với rumor transformation và một vài canonical motifs.

**Crafting:** Bắt đầu với Repair và một vài design.

**Combat:** Bắt đầu với một encounter đủ chứng minh positioning/injury/morale.

**Legacy:** Bắt đầu với một công trình, một vật phẩm truyền lại và một succession case.

Giảm độ sâu không có nghĩa xóa interface hoặc cắt đường mở rộng.

<a id="m25-t065-s146"></a>

### 88. Không nên đưa vào Vertical Slice đầu tiên

Các nội dung nên hoãn đến khi core loop đã được chứng minh:

- toàn bộ 60 settlement;
- hàng nghìn NPC đầy đủ;
- toàn bộ 11 Class với tất cả Technique;
- hàng trăm ritual;
- tất cả mythic entities;
- toàn bộ hệ quân sự quy mô lớn;
- sản xuất hàng loạt Cổ Loa;
- full regional economy;
- procedural content không giới hạn;
- đầy đủ cả sáu hồi.

Nếu cố thực hiện đồng thời, team sẽ rất khó biết game không hấp dẫn là do thiếu nội dung, lỗi hệ thống hay vấn đề UX.

---

<a id="m25-t065-s147"></a>

## PHẦN XXVI — PHÂN CHIA GIAI ĐOẠN PHÁT TRIỂN

<a id="m25-t065-s148"></a>

### 89. Milestone 0 — Architecture Proof

**Mục tiêu:** Chứng minh rule engine hoạt động nhất quán.

Bao gồm:

- Entity Registry;
- WorldClock;
- RNG;
- Command;
- Domain Event;
- Current State;
- Save/Load;
- Replay.

**Exit Gate:** Cùng input tạo cùng state trong phạm vi engine đã xác định; transaction không làm sai ownership.

<a id="m25-t065-s149"></a>

### 90. Milestone 1 — Living Settlement

**Mục tiêu:** Một settlement tự vận hành khi player không tham gia.

Bao gồm:

- household;
- NPC goals;
- profession;
- food/resource;
- relationship;
- birth/death ở độ phân giải phù hợp;
- Event Scheduler.

**Exit Gate:** Settlement có thể chạy qua nhiều chu kỳ thời gian và tạo những thay đổi có nguyên nhân.

<a id="m25-t065-s150"></a>

### 91. Milestone 2 — Player Intervention

**Mục tiêu:** Player có thể thay đổi một vấn đề đang tồn tại.

Bao gồm:

- Character;
- Dialogue;
- Activity;
- Event;
- Story Thread;
- Knowledge;
- Quest Projection.

**Exit Gate:** Một thread có thể tiến triển khác nhau khi player can thiệp hoặc không can thiệp.

<a id="m25-t065-s151"></a>

### 92. Milestone 3 — Physical Gameplay

**Mục tiêu:** Các hệ hoạt động ngoài hội thoại có tác động thật.

Bao gồm:

- Travel;
- Local Exploration;
- Equipment;
- Crafting;
- Injury;
- Combat;
- Dungeon.

**Exit Gate:** Kết quả vật lý cập nhật World State và tạo hậu quả về thời gian, tài sản hoặc quan hệ.

<a id="m25-t065-s152"></a>

### 93. Milestone 4 — Long-Term Consequence

**Mục tiêu:** Hệ thống có ký ức dài hạn.

Bao gồm:

- Time Skip;
- Career;
- Aging;
- Death;
- Succession;
- Chronicle;
- basic Legacy.

**Exit Gate:** Một sự kiện ban đầu còn dấu vết sau nhiều năm; succession không làm mất tính nhất quán.

<a id="m25-t065-s153"></a>

### 94. Milestone 5 — Historical Integration

**Mục tiêu:** Kết nối đời sống địa phương với lịch sử lớn.

Bao gồm:

- Historical Anchor Controller;
- Faction;
- regional effects;
- major narrative exposure;
- Time Transition.

**Exit Gate:** Một Historical Anchor có thể tiến triển theo nhiều kịch bản hợp lệ mà không cần engine gian lận.

<a id="m25-t065-s154"></a>

### 95. Milestone 6 — Content Scaling

**Mục tiêu:** Mở rộng địa lý và nội dung bằng pipeline đã kiểm chứng.

Bao gồm:

- Offline LLM-assisted content;
- Event Library;
- NPC Archetypes;
- Regional Variants;
- content validation;
- automated regression.

**Exit Gate:** Thêm nội dung không làm tăng tỷ lệ lỗi logic và không bắt sửa engine cho từng event nhỏ.

---

<a id="m25-t065-s155"></a>

## PHẦN XXVII — SYSTEM DESIGN DOCUMENTS CẦN TẠO TIẾP

<a id="m25-t065-s156"></a>

### 96. GDD chưa đủ để bắt đầu code song song ở quy mô lớn

Module 25 nên dẫn tới một tập tài liệu kỹ thuật có ranh giới rõ.

<a id="m25-t065-s157"></a>

#### Technical Architecture Specification

Mô tả:

- runtime architecture;
- modules;
- dependency;
- command flow;
- event commit;
- threading;
- persistence.

<a id="m25-t065-s158"></a>

#### Canonical Data Model

Mô tả:

- entities;
- IDs;
- relationships;
- ownership;
- state machines;
- invariants.

<a id="m25-t065-s159"></a>

#### Rules & Formula Specification

Mô tả:

- Skill Checks;
- Combat Resolution;
- NPC Utility;
- Event Eligibility;
- Economy;
- Aging;
- randomization.

<a id="m25-t065-s160"></a>

#### Content Schema Specification

Mô tả:

- Event Template;
- Character Archetype;
- Dialogue;
- Quest/Story;
- historical content tagging.

<a id="m25-t065-s161"></a>

#### World Simulation Test Plan

Mô tả:

- seeds;
- scenarios;
- invariants;
- replay tests;
- simulation benchmarks.

<a id="m25-t065-s162"></a>

#### Vertical Slice Implementation Plan

Mô tả:

- feature scope;
- work breakdown;
- dependencies;
- acceptance criteria;
- implementation order.

Các tài liệu này sẽ giúp coding agents hoặc đội lập trình cùng làm việc trên một hợp đồng thiết kế thống nhất.

---

<a id="m25-t065-s163"></a>

## PHẦN XXVIII — NHỮNG QUYẾT ĐỊNH CẦN KHÓA SAU AUDIT

<a id="m25-t065-s164"></a>

### 97. Architecture Decisions

**ADR-001 — Runtime Authority**

Code Engine là nguồn quyết định gameplay. LLM không ghi World State.

**ADR-002 — Deployment Style**

Bắt đầu với Modular Monolith thay vì microservices.

**ADR-003 — World State**

Sử dụng Authoritative Current State, Critical Event Journal và Snapshot.

**ADR-004 — Command Processing**

World State mutations đi qua Command/Rule/Commit Pipeline có kiểm soát.

**ADR-005 — Data Ownership**

Mỗi loại trạng thái có một authoritative owner.

**ADR-006 — Knowledge Separation**

World Truth và Actor Knowledge là hai nguồn dữ liệu riêng.

**ADR-007 — Event Contract**

Phân biệt Eligible Event, Committed Event, Observation, Claim, Story Thread và Quest Projection.

**ADR-008 — World Director**

Director chỉ điều phối cơ hội và presentation, không thay đổi causal truth.

**ADR-009 — Historical Anchor**

Phân Hard Canon, Flexible Anchor và Emergent Event.

**ADR-010 — Resource Authority**

Commodity/Asset Ledger sở hữu nguồn lực vật chất. Supply Days là phép tính dẫn xuất.

**ADR-011 — Character Continuity**

Succession không copy Attributes, Skills, Class, Knowledge hoặc tài sản không hợp lệ.

**ADR-012 — Content Generation**

LLM offline phải đi qua schema, causal validation và approval.

**ADR-013 — Determinism**

Random được kiểm soát theo seed/version/context, có replay test.

**ADR-014 — Performance**

Sử dụng sparse relationships, simulation tiers và event-driven scheduling.

**ADR-015 — Production Strategy**

Technical Vertical Slice phải được nghiệm thu trước khi scale lên thế giới đầy đủ.

---

<a id="m25-t065-s165"></a>

## PHẦN XXIX — FINAL AUDIT QUALITY GATE

<a id="m25-t065-s166"></a>

### 98. Điều kiện kết thúc Architecture Review

Đợt audit chỉ được xem là hoàn tất ở mức thiết kế khi:

- mọi domain có owner;
- mọi state mutation có command/rule hợp lệ;
- Event và Quest không trùng trách nhiệm;
- World Director không thể sửa truth;
- Historical Anchors có contract;
- Character Generation có causal pipeline;
- NPC Decision dùng perceived state;
- Resource/Item/Ownership có một nguồn sự thật;
- Combat/Injury có một hệ thương tích;
- Travel/Trade dùng chung Route Graph;
- Succession giữ identity và knowledge boundaries;
- Save/Replay có version strategy;
- Vertical Slice có scope và exit criteria;
- các rủi ro P0 đều có thiết kế giải quyết được;
- các rủi ro P1 có kế hoạch kiểm thử;
- content pipeline có validation.

Đây là điều kiện để chuyển sang Technical Design.

Không có nghĩa các rủi ro đã được chứng minh đóng trong code.

<a id="m25-t065-s167"></a>

### 99. Điều kiện bắt đầu mở rộng nội dung

Chỉ nên mở rộng từ Vertical Slice khi có bằng chứng rằng:

**World Simulation hoạt động ổn định.**

**Một event có thể tạo nhiều kết quả có nguyên nhân.**

**NPC có thể tự hành động và tiếp tục sống ngoài tầm nhìn player.**

**Knowledge không bị lộ qua UI.**

**Một vật phẩm và tài nguyên không bị nhân đôi.**

**Một Story Thread không cần player để tiếp tục.**

**Một companion có thể từ chối lệnh trái ranh giới.**

**Một thương tích tiếp tục ảnh hưởng gameplay ngoài combat.**

**Một Time Skip tạo được hậu quả lâu dài.**

**Một successor có thể tiếp tục campaign mà không thừa hưởng vô lý dữ liệu nhân vật trước.**

**Một Historical Anchor có thể tích hợp với World State mà không cần phá quy luật engine.**

Nếu những điều đó chưa hoạt động, việc bổ sung 100 quest hoặc 10 vùng mới sẽ chỉ làm tăng chi phí sửa lỗi.

---

<a id="m25-t065-s168"></a>

## PHẦN XXX — KẾT LUẬN KIỂM TOÁN

<a id="m25-t065-s169"></a>

### 100. Đánh giá kiến trúc

**Tầm nhìn tổng thể:** Có tính nhất quán và bản sắc rõ.

**Thiết kế gameplay:** Đã có hệ thống phong phú, đặc biệt ở nghề nghiệp, quan hệ, hậu quả và đời sống nhiều thế hệ.

**Cấu trúc nội dung:** Có khả năng mở rộng nếu sử dụng content schema và rule-based composition.

**Khả năng triển khai:** Có thể triển khai từng phần, nhưng toàn bộ phạm vi hiện tại là một dự án rất lớn.

**Rủi ro lớn nhất:** Không phải thiếu LLM, mà là độ phức tạp giao thoa giữa các domain, Historical Anchors và tính nhất quán lâu dài.

**Phương án kỹ thuật nên chọn:** Offline LLM-Assisted Content Production + Modular Monolith + Authoritative Rule Engine + Controlled Deterministic Randomness + Critical Event Journal + Snapshot + Knowledge Projection + Automated Validation.

<a id="m25-t065-s170"></a>

### 101. Những điều tuyệt đối không nên thay đổi

Thế giới phải tiếp tục sống khi player không can thiệp.

Nhân vật phải có lịch sử và nguồn gốc.

Năng lực phải hình thành qua trải nghiệm.

NPC không được trở thành công cụ phục vụ kịch bản vô điều kiện.

Quest không được tạo ra sự thật thế giới.

LLM không được quyết định kết quả runtime.

Đời sống bình thường phải có giá trị ngang những sự kiện lớn.

Chiến đấu không phải con đường duy nhất.

Thời gian, thương tích, quan hệ và tài sản phải có hậu quả thật.

Succession phải là một con người khác tiếp tục sống trong thế giới cũ.

Lịch sử phải được nhớ, diễn giải và đôi khi bị kể lại sai.

<a id="m25-t065-s171"></a>

### 102. Tuyên ngôn kiến trúc cuối cùng

Sức mạnh của game này không nằm ở việc viết được hàng triệu nhánh truyện.

Nó nằm ở việc tạo được một thế giới mà:

> **Mỗi con người có lý do để tồn tại.**

> **Mỗi hành động có điều kiện để xảy ra.**

> **Mỗi biến cố có một nguồn gốc.**

> **Mỗi hậu quả có nơi để tiếp tục lan truyền.**

> **Mỗi ký ức có người mang theo.**

> **Mỗi di sản có khả năng tồn tại, thay đổi hoặc bị quên lãng.**

Nếu engine thực hiện được những nguyên tắc đó một cách nhất quán, rất nhiều câu chuyện có thể hình thành mà không cần viết trước từng đường đi.

Và khi người chơi bước vào thế giới ấy, họ không điều khiển một kịch bản được dựng ra để tôn vinh mình.

Họ đang sống một cuộc đời trong một hệ thống lịch sử và xã hội đang vận hành.

**Đó là kiến trúc cần được bảo vệ khi chuyển từ GDD sang code.**

---

<a id="m25-t065-s172"></a>

## PHẦN XXXI — KẾT QUẢ CUỐI CÙNG & BƯỚC CHUYỂN GIAO

<a id="m25-t065-s173"></a>

### 103. Trạng thái sau Module 25

Đề xuất đánh dấu:

**Conceptual GDD:** Đủ nền để chuyển sang giai đoạn thiết kế kỹ thuật.

**Cross-System Architecture:** Có kiến trúc đích và các hợp đồng quan trọng cần chuẩn hóa.

**World Simulation:** Cần prototype để kiểm chứng nhân quả, hiệu năng và persistence.

**Content Pipeline:** Có hướng triển khai, cần schema và tooling thật.

**Historical Integration:** Cần kiểm chứng các kịch bản hợp lệ cho từng Hard Canon Anchor.

**Full Production Scope:** Chưa nên khóa trước khi có kết quả Vertical Slice.

<a id="m25-t065-s174"></a>

### 104. Tài liệu tiếp theo nên được xây dựng

**Module 26 — Technical System Design & Canonical Data Model**

Đây phải là bản thiết kế kỹ thuật có thể giao cho đội phát triển, không tiếp tục chỉ dừng ở ý tưởng gameplay.

Nó cần xác định đầy đủ:

- cấu trúc source code;
- domain modules;
- entity schemas;
- IDs;
- state machines;
- interfaces;
- commands;
- domain events;
- event sequencing;
- transaction boundaries;
- RNG;
- save/load;
- content schemas;
- validation;
- test contracts.

Sau đó:

**Module 27 — Rule Engine & Formula Specification**

Xác định cụ thể các công thức gameplay, điều kiện, trọng số, cơ chế ngẫu nhiên và quy tắc tính toán.

**Module 28 — Vertical Slice Implementation Blueprint**

Chuyển các hợp đồng kỹ thuật thành kế hoạch lập trình, test và nghiệm thu một bản playable prototype.

Đây là ba bước giúp biến GDD từ một tài liệu thiết kế thế giới thành một hệ thống phần mềm có thể triển khai và kiểm chứng.

**Kết luận của Module 25:** Tôi đề xuất **giữ nguyên tầm nhìn và các cơ chế bản sắc đã khóa**, nhưng chưa mở rộng thêm nội dung. Ưu tiên tiếp theo là chuẩn hóa **Canonical Data Model, Domain Ownership, Event Contract và Historical Anchor Contract** trước khi lập trình. Bốn phần này quyết định liệu hàng triệu tổ hợp diễn biến có được xử lý nhất quán bằng code hay không.

Khi chuyển sang Module 26, chúng ta nên dùng bản kiểm toán này làm tiêu chuẩn: mỗi schema, interface hoặc rule mới đều phải chỉ rõ nó thuộc domain nào, được ai thay đổi, tạo sự kiện gì và có thể kiểm thử như thế nào.

<!-- SOURCE T065 END -->

