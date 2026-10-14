# Module 23 — UI/UX, Journal, Codex, Chronicle và phản hồi

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T057](../../source/conversation.md#t057)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

UI chỉ nhận knowledge-filtered projections, kể cả search/notification. Debug omniscient tách khỏi player build. Không dùng một checksum/render query để rút RNG. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Active character knowledge, capability, costs, provenance và read models. |
| Đầu ra / bên sở hữu | Views/input commands; Unity/ViewModels không sở hữu world state. |
| Đọc cùng | 14, 16, 17, 21, 22, 25–29.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Search không trả actor/location bí mật; succession archive không mở kỹ thuật/dialogue trái điều kiện. |
| Việc chưa được chứng minh | Final art/platform/audio và accessibility evidence; prototype desktop flows đã có CLIENT-UX-BASELINE. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T057 BEGIN -->

<a id="m23-t057-s001"></a>

## GDD MODULE 23
<a id="m23-t057-s002"></a>

## UI/UX, INFORMATION ARCHITECTURE, JOURNAL, CODEX, CHRONICLE & PLAYER FEEDBACK SYSTEM

**Trạng thái:** Thiết kế hệ thống lõi — đề xuất khóa vào GDD  
**Phạm vi:** HUD, Navigation, Character, Party, Map, Travel, Quest, Investigation, Dialogue, Combat, Equipment, Economy, Faction, Belief, Bestiary, Calendar, Succession, Legacy, Information Architecture, Accessibility và Feedback.  
**Định hướng:** Giao diện hỗ trợ ra quyết định trong một thế giới có thông tin không hoàn hảo, lịch sử dài hạn và nhiều con đường phát triển nhân vật.

---

<a id="m23-t057-s003"></a>

## PHẦN I — TRIẾT LÝ THIẾT KẾ

<a id="m23-t057-s004"></a>

### 1. UI là một phần của gameplay, không chỉ là giao diện hiển thị

Trong game này, người chơi có thể phải đưa ra quyết định dựa trên:

- năng lực và tình trạng bản thân;
- thông tin về địa hình;
- nguồn gốc một tin đồn;
- quan hệ giữa hai NPC;
- lời hứa đã đưa ra nhiều năm trước;
- mức độ đáng tin của người chỉ đường;
- những gì companion đã chứng kiến;
- điều kiện kinh tế của settlement;
- sự thay đổi của các faction;
- những ký ức được truyền qua nhiều thế hệ.

Nếu tất cả thông tin này bị chia thành những menu độc lập và khó truy cập, game sẽ rất khó chơi dù các hệ thống bên dưới được thiết kế tốt.

Ngược lại, nếu giao diện hiển thị tất cả sự thật và các chỉ số ẩn, những hệ thống về điều tra, niềm tin, tin đồn và quan hệ sẽ mất ý nghĩa.

Bởi vậy, UI phải đóng vai trò:

> **Giúp người chơi hiểu và sử dụng những điều nhân vật có thể biết, đồng thời bảo vệ những điều nhân vật chưa có cơ sở để biết.**

<a id="m23-t057-s005"></a>

### 2. Sáu nguyên tắc thiết kế cốt lõi

<a id="m23-t057-s006"></a>

#### Information Integrity — Tính toàn vẹn thông tin

Giao diện không tự biến lời kể thành sự thật.

Một NPC nói rằng người khác giết cha mình thì Journal phải ghi đây là lời tố cáo, không phải xác nhận hung thủ.

<a id="m23-t057-s007"></a>

#### Contextual Clarity — Rõ ràng theo ngữ cảnh

Thông tin cần được hiển thị tại nơi người chơi đang ra quyết định.

Ví dụ trong Travel Planner, người chơi cần biết tình trạng tuyến đường, không cần mở thêm năm menu để tìm sức khỏe của party hoặc lượng lương thực.

<a id="m23-t057-s008"></a>

#### Progressive Disclosure — Mở rộng dần mức chi tiết

Người mới nhìn thấy điều quan trọng trước.

Người muốn tìm hiểu sâu có thể mở nguồn thông tin, trạng thái, quy tắc và các liên kết liên quan.

Không bắt người chơi đọc toàn bộ simulation để thực hiện một hành động thường lệ.

<a id="m23-t057-s009"></a>

#### Persistent Memory — Ghi nhớ dài hạn

Game kéo dài nhiều thập kỷ nên người chơi không thể tự nhớ toàn bộ tên tuổi, lời hứa, sự kiện và các lần thay đổi địa điểm.

UI phải hỗ trợ trí nhớ mà không tự động suy luận thay người chơi.

<a id="m23-t057-s010"></a>

#### Explainable Consequences — Hậu quả có thể hiểu

Sau một hành động, player cần nhận ra những thay đổi mà nhân vật có khả năng quan sát.

Những tác động ẩn có thể xuất hiện muộn, nhưng khi xuất hiện vẫn phải có nguồn gốc và tính nhất quán.

<a id="m23-t057-s011"></a>

#### Accessible by Design — Dễ tiếp cận từ nền tảng

Khả năng đọc, điều hướng, tùy chỉnh văn bản, phụ đề, thao tác bằng bàn phím hoặc tay cầm không phải tính năng thêm sau cùng.

Chúng phải được đưa vào cấu trúc giao diện ngay từ đầu.

---

<a id="m23-t057-s012"></a>

## PHẦN II — INFORMATION ARCHITECTURE

<a id="m23-t057-s013"></a>

### 3. Phân biệt sự thật của thế giới và thông tin của người chơi

Hệ thống đã có ba nguồn dữ liệu quan trọng:

**World State:** Sự thật khách quan mà engine đang lưu.

**Character Knowledge:** Những gì từng nhân vật biết, tin, nhớ hoặc suy luận.

**Player-Facing Information:** Những gì UI được phép trình bày cho người đang chơi.

Ba lớp này không đồng nhất.

Ví dụ:

World State xác định cây cầu tại Bến Hạc bị lũ làm hỏng.

Một người dân tin cây cầu bị phá hoại.

Player chỉ gặp người dân đó và nghe lời kể.

Giao diện được phép hiển thị:

> “Nậu Khôn cho rằng cây cầu bị người lạ phá trong đêm.”

Không được hiển thị:

> “Nguyên nhân cây cầu hỏng: Lũ.”

Trừ khi player đã có bằng chứng đủ để biết nguyên nhân đó.

<a id="m23-t057-s014"></a>

### 4. Kiến trúc dữ liệu đề xuất

Hệ thống thông tin player-facing nên sử dụng một lớp trung gian:

**Knowledge Projection Layer.**

```text
WORLD STATE
    |
    +-- Objective Events
    +-- NPC State
    +-- Environment
    +-- Economy
    +-- Faction
    +-- Historical State
    |
    v
EVENT & KNOWLEDGE ENGINE
    |
    +-- Observations
    +-- Testimonies
    +-- Beliefs
    +-- Inferences
    +-- Memories
    +-- Information Propagation
    |
    v
CHARACTER KNOWLEDGE STORE
    |
    v
KNOWLEDGE PROJECTION LAYER
    |
    +-- Visibility Validation
    +-- Source Validation
    +-- Time/Freshness Validation
    +-- Confidence Presentation
    +-- Contradiction Handling
    |
    v
UI VIEW MODELS
    |
    +-- World Map
    +-- Journal
    +-- Codex
    +-- Chronicle
    +-- Dialogue
    +-- Faction
    +-- NPC Profile
    +-- Travel Planner
```

UI không được truy vấn trực tiếp hidden truth để quyết định nội dung hiển thị.

Ngoại lệ là những dữ liệu cơ học mà player có quyền biết rõ, như vật phẩm đang mang, thao tác vừa thực hiện hoặc một quy tắc gameplay công khai.

<a id="m23-t057-s015"></a>

### 5. Knowledge Record

Một mẩu thông tin quan trọng cần có cấu trúc đủ chi tiết.

```text
KnowledgeRecord
    KnowledgeID
    Subject
    Claim
    KnowledgeType
    Source
    Observer
    ObservedAt
    LearnedAt
    LocationContext
    EvidenceLinks
    Confidence
    Contradictions
    Freshness
    VisibilityScope
    KnowledgeHolder
```

Các trường cần phân biệt rõ:

**ObservedAt:** Sự kiện hoặc quan sát gốc xảy ra lúc nào.

**LearnedAt:** Nhân vật biết thông tin vào lúc nào.

**Source:** Ai hoặc điều gì cung cấp thông tin.

**KnowledgeHolder:** Character nào đang sở hữu tri thức đó.

**Confidence:** Nhân vật đánh giá mức độ đáng tin, không phải xác suất sự thật tuyệt đối.

**Freshness:** Thông tin còn có khả năng phản ánh trạng thái hiện tại hay không.

Ví dụ một tin về checkpoint được biết hôm nay nhưng nguồn thực tế đã quan sát checkpoint từ 18 ngày trước.

Nếu không có cả hai mốc thời gian, UI sẽ dễ trình bày tin cũ như thông tin mới.

<a id="m23-t057-s016"></a>

### 6. Năm loại thông tin hiển thị

Đề xuất một hệ thống phân loại thống nhất:

| Loại | Ý nghĩa | Ví dụ cách viết |
|---|---|---|
| Quan sát trực tiếp | Character tận mắt hoặc trực tiếp cảm nhận | “Bạn thấy ván cầu đã gãy.” |
| Lời kể | Thông tin do một actor cung cấp | “Hạc Dư nói đã thấy một người lạ.” |
| Suy luận | Nhận định dựa trên bằng chứng | “Vết gãy có vẻ không mới.” |
| Giả thuyết | Khả năng chưa được kiểm chứng | “Có thể người đó đi về hạ lưu.” |
| Điều đã xác minh | Kết luận được bằng chứng hiện có hỗ trợ đủ mạnh | “Chiếc mái chèo thuộc nhà Tòng.” |

**Điều đã xác minh không đồng nghĩa engine tiết lộ sự thật toàn tri.**

Một kết luận có thể được xem là đủ chắc trong phạm vi kiến thức hiện tại rồi bị một bằng chứng mới bác bỏ.

UI phải cho phép cập nhật và điều chỉnh.

<a id="m23-t057-s017"></a>

### 7. Không sử dụng một thanh Confidence chung cho tất cả thông tin

Một dòng chữ “Confidence 75%” dễ tạo cảm giác game biết chính xác xác suất một lời khai là thật.

Nhưng mức độ tin cậy của character không nhất thiết tương ứng một mô hình xác suất được hiệu chuẩn.

Đề xuất sử dụng các nhãn ngôn ngữ tự nhiên:

- Chưa kiểm chứng;
- Có cơ sở;
- Đáng tin trong phạm vi hiện biết;
- Đã được xác minh;
- Có thông tin mâu thuẫn;
- Không còn chắc chắn do thông tin đã cũ.

Nếu sau này có hệ thống suy luận xác suất thực sự, UI nâng cao có thể cung cấp các khoảng tin cậy phù hợp. Nhưng không được sinh ra các phần trăm tùy tiện cho đẹp.

<a id="m23-t057-s018"></a>

### 8. Một đối tượng có thể được nhìn từ nhiều góc độ

Ví dụ một địa điểm là **Bến Vạc**.

World Map quan tâm:

- vị trí;
- tuyến kết nối;
- các điều kiện đi lại.

Economy quan tâm:

- hàng hóa;
- nhu cầu;
- thị trường;
- năng lực vận tải.

Journal quan tâm:

- các Story Thread đang liên quan.

Chronicle quan tâm:

- những sự kiện đã xảy ra ở bến.

Social Graph quan tâm:

- người và gia đình tại đó.

Codex quan tâm:

- lịch sử, nghề nghiệp, truyền thống.

Đây không nên là sáu bản ghi Bến Vạc độc lập.

Tất cả phải tham chiếu tới cùng một **Entity ID**, sau đó lấy các projection khác nhau.

Nếu một settlement đổi tên hoặc bị phá hủy, mọi màn hình phải có khả năng phản ánh sự thay đổi hợp lệ.

---

<a id="m23-t057-s019"></a>

## PHẦN III — CẤU TRÚC ĐIỀU HƯỚNG TOÀN GAME

<a id="m23-t057-s020"></a>

### 9. Không đưa toàn bộ hệ thống lên thanh menu chính

Game có hơn hai mươi subsystem.

Nếu mỗi subsystem có một tab cấp cao nhất, người chơi sẽ phải điều hướng quá nhiều.

Đề xuất tổ chức theo **nhiệm vụ nhận thức của player**, không theo tên module kỹ thuật.

Năm khu vực chính:

| Khu vực | Mục đích của người chơi | Nội dung bên trong |
|---|---|---|
| **Nhân vật** | Tôi là ai và có thể làm gì? | Attribute, Skill, Class, Profession, Psychology, Injury |
| **Đồng hành** | Ai đang cùng tôi và chúng tôi ra sao? | Party, Relationships, Roles, Agreements |
| **Thế giới** | Tôi đang ở đâu và có thể đi đâu? | Maps, Routes, Travel, Settlements |
| **Nhật ký** | Tôi biết gì và đang quan tâm chuyện gì? | Threads, Evidence, Codex, Chronicle |
| **Tài sản** | Tôi có gì và đang quản lý gì? | Inventory, Equipment, Contracts, Property, Workshop |

Những chức năng khác xuất hiện theo ngữ cảnh.

Ví dụ:

- Faction Profile mở từ NPC, Map hoặc Journal.
- Crafting Workspace mở khi tương tác với công cụ/xưởng phù hợp.
- Travel Planner mở từ World Map.
- Time Skip Planner mở từ Rest/Downtime.
- Combat UI xuất hiện khi có encounter chiến đấu.

Không cần những chức năng này thường trực chiếm màn hình.

<a id="m23-t057-s021"></a>

### 10. Ba cấp độ giao diện

<a id="m23-t057-s022"></a>

#### Cấp 1 — HUD

Những gì cần biết ngay khi đang chơi.

Phải ít, rõ và theo tình huống.

<a id="m23-t057-s023"></a>

#### Cấp 2 — Context Panel

Thông tin và hành động tại địa điểm, NPC hoặc đối tượng đang tương tác.

Ví dụ mở thông tin thuyền, người bán hàng hoặc lối đi.

<a id="m23-t057-s024"></a>

#### Cấp 3 — Full-Screen Workspace

Dành cho hoạt động cần suy nghĩ hoặc phân tích:

- Journal;
- World Map;
- Character;
- Crafting;
- Faction;
- Time Skip.

Cấp 3 được phép sâu hơn vì player chủ động mở.

Không bắt những quyết định chiến lược phức tạp diễn ra trong một popup nhỏ giữa combat.

<a id="m23-t057-s025"></a>

### 11. Điều hướng phải có tính nhất quán

Nguyên tắc:

- Một thao tác Back luôn có ý nghĩa nhất quán.
- Mở hồ sơ một NPC từ Map hay Journal phải hiển thị cùng cấu trúc thông tin.
- Các panel có cùng loại thông tin dùng nhãn và icon tương tự.
- Có thể quay lại vị trí trước đó sau khi mở một liên kết.
- Những màn hình nhiều tầng có đường dẫn điều hướng rõ.
- Người chơi không cần nhớ đã mở một thông tin từ tab nào.

Ví dụ đang đọc một Thread về người mất tích, player chọn tên nhân chứng.

NPC Profile mở.

Từ đó player mở settlement nơi người ấy sống.

Khi quay lại, giao diện trở về Thread và vị trí đang đọc, không reset toàn bộ Journal.

---

<a id="m23-t057-s026"></a>

## PHẦN IV — HUD CHÍNH TRONG GAMEPLAY

<a id="m23-t057-s027"></a>

### 12. Mục tiêu HUD

HUD phải trả lời bốn câu hỏi nhanh:

1. Nhân vật đang ở đâu và thời gian nào?
2. Nhân vật có vấn đề khẩn cấp gì không?
3. Đối tượng hoặc hoạt động trước mặt là gì?
4. Người chơi có thể làm gì tiếp theo?

Không được biến HUD thành dashboard quản lý toàn bộ World Simulation.

<a id="m23-t057-s028"></a>

### 13. Bố cục đề xuất

Wireframe ý tưởng cho màn hình desktop:

```text
┌──────────────────────────────────────────────────┐
│ Tam Giang • Bến Vạc        Năm 3 • Nước đang rút │
│                                                  │
│                                                  │
│                  GAME WORLD                      │
│                                                  │
│         [NPC / Objects / Environment]            │
│                                                  │
│                                                  │
│ Vạc Mân                            [Tương tác]   │
│ Thể lực: Ổn                                     │
│ Chân trái: Đang hồi phục                        │
│                                                  │
│ Nhân vật  Đồng hành  Thế giới  Nhật ký  Tài sản  │
└──────────────────────────────────────────────────┘
```

Đây là bố cục chức năng, chưa khóa mỹ thuật hoặc tỷ lệ pixel.

<a id="m23-t057-s029"></a>

### 14. Contextual HUD

HUD thay đổi thông tin ưu tiên theo activity.

**Trong Settlement:** Hiển thị NPC, interaction, tình trạng character, địa điểm.

**Trong Travel:** Hiển thị tuyến, ước tính thời gian, nguồn lực liên quan, tình trạng party.

**Trong Combat:** Hiển thị actor, vị trí, Stamina, Injury, Reaction, Action và objective.

**Trong Crafting:** Hiển thị vật liệu, tool, tiến độ, quality risk và những quyết định đang chờ.

**Trong Investigation:** Hiển thị dấu vết có thể kiểm tra, vị trí tìm kiếm và thông tin đã biết có liên quan.

Sự thay đổi này phải dựa trên cùng một Design System, không biến từng activity thành giao diện của một game khác.

<a id="m23-t057-s030"></a>

### 15. Không có Quest Marker thường trực

Player có thể chủ động pin một Current Concern.

Ví dụ:

> “Tìm hiểu ai đã lấy mái chèo nhà Tòng.”

Nhưng mặc định HUD không nên hiển thị danh sách ba quest mới nhất với mũi tên chỉ tới mục tiêu.

Với những vị trí đã biết rõ, hệ thống vẫn cho phép đánh dấu đường.

Với mục tiêu chưa xác định, UI chỉ thể hiện phạm vi hoặc lead hợp lệ.

Không được tự đặt waypoint chính xác chỉ vì Quest System biết hidden target.

---

<a id="m23-t057-s031"></a>

## PHẦN V — CHARACTER UI

<a id="m23-t057-s032"></a>

### 16. Character Overview

Character Screen phải giúp player hiểu:

- bản thân đang ở tình trạng nào;
- năng lực nổi bật và hạn chế;
- nghề nghiệp và Class đã hình thành;
- những lĩnh vực đang phát triển;
- tình trạng sức khỏe;
- các ký ức hoặc thay đổi lâu dài có liên quan.

Không nên bắt đầu bằng một bảng chỉ số dày đặc.

Cấu trúc đề xuất:

**Tổng quan:** Tuổi, xuất thân, household, nghề, Class, tình trạng hiện tại.

**Năng lực:** Attribute, Skill, Knowledge, Technique.

**Sức khỏe:** Injury, Condition, Recovery, Fatigue.

**Cuộc đời:** Milestones, tâm lý đã biết, quan hệ và trách nhiệm.

<a id="m23-t057-s033"></a>

### 17. Hiển thị sáu Attributes

Sáu chỉ số:

**THỂ, XẢO, GIÁC, TRÍ, TÂM, KHÍ.**

Có thể hiển thị giá trị 1–10 vì đây là năng lực cơ học mà người chơi cần hiểu để đưa ra quyết định.

Nhưng tooltip phải giải thích theo hành vi.

Ví dụ:

**GIÁC — 7**

> Có khả năng quan sát và nhận ra những dấu hiệu nhỏ tốt hơn mức thông thường. Tuy nhiên, việc hiểu ý nghĩa của dấu vết vẫn phụ thuộc Skill và Knowledge tương ứng.

Không được khiến player hiểu lầm GIÁC cao tự cho biết sự thật.

<a id="m23-t057-s034"></a>

### 18. Skill & Mastery

Skill Screen phải phân biệt:

- mức Mastery;
- năng lực thực hiện hiện tại;
- kiến thức liên quan;
- Technique đã học;
- những điều kiện gây hạn chế.

Ví dụ:

**Thương & Mâu — Thành thạo**

**Tình trạng hiện tại:** Khó thực hiện các động tác cần xoay người nhanh do thương tích chân.

Character không tự giảm Mastery chỉ vì bị thương.

UI phải thể hiện sự khác biệt giữa “đã học được gì” và “hiện tại làm được gì”.

<a id="m23-t057-s035"></a>

### 19. Class Emergence UI

Không nên tạo thanh:

> Thủy Binh 72% — còn 28% để unlock.

Vì Class là identity hình thành qua hành động, xã hội và những cột mốc.

Đề xuất:

**Dấu hiệu hình thành:** Những hành vi và khả năng cho thấy character đang phát triển theo một hướng.

**Công nhận:** Ai hoặc tổ chức nào đã công nhận vai trò đó.

**Technique:** Các kỹ thuật character thực sự có thể sử dụng.

**Doctrine:** Triết lý hành động đã hình thành.

**Legacy:** Các Technique hoặc kinh nghiệm có thể truyền lại.

Ví dụ:

> “Bạn ngày càng được người quanh Bến Vạc tin tưởng trong những chuyến vượt nước khó. Mân đã nhiều lần dẫn đoàn vượt dòng chảy nguy hiểm.”

Không cần nói:

> “Bạn còn thiếu hai sự kiện để unlock Thủy Binh.”

<a id="m23-t057-s036"></a>

### 20. Psychology UI

Psychology là lớp nhạy cảm.

Không nên hiện:

> Trauma 56%, Anger 72%, Cowardice -4.

Thay vào đó, Character Reflection có thể diễn đạt các khuynh hướng mà character có khả năng tự nhận biết:

> “Mân thường thấy bất an khi phải vượt dòng nước lớn một mình kể từ lần gặp nạn.”

Một số động cơ hoặc xung đột nội tâm có thể chỉ xuất hiện sau các tình huống phù hợp.

Không cần ép player character hành động theo một profile cố định, nhưng cũng không nên dùng giao diện để công bố toàn bộ tâm lý của NPC khác.

---

<a id="m23-t057-s037"></a>

## PHẦN VI — PARTY & COMPANION UI

<a id="m23-t057-s038"></a>

### 21. Party Overview

Party Screen cần hỗ trợ cả chiến đấu và đời sống.

Mỗi thành viên hiển thị:

- tên và vai trò;
- tình trạng sức khỏe;
- năng lực hoặc chuyên môn nổi bật;
- vai trò hành trình;
- những mối quan tâm mà họ đã bộc lộ;
- thỏa thuận đồng hành;
- tình trạng sẵn sàng tham gia hoạt động.

Không cần hiển thị mọi Relationship Variable.

Ví dụ Miên có thể được mô tả:

> “Tin tưởng Mân trong những chuyến đi, nhưng đang lo cho em gái ở Bãi Lau.”

Thông tin chỉ xuất hiện khi Mân có cơ sở biết về mối lo đó.

<a id="m23-t057-s039"></a>

### 22. Companion Autonomy

Khi player đưa một chỉ thị, UI cần thể hiện rõ đây là:

- hành động chiến thuật có thể điều khiển;
- đề nghị;
- phân công đã thỏa thuận;
- yêu cầu vượt khỏi phạm vi thỏa thuận;
- hành động có khả năng chạm tới Moral Boundary.

Ví dụ:

**Đề nghị Vũ ở lại canh người bị thương.**

Nếu Vũ không muốn vì phải về nhà trước ngày đã hẹn, UI không được coi đây là lỗi thao tác.

Vũ có thể phản đối bằng hội thoại.

Player có thể thương lượng hoặc thay đổi kế hoạch.

<a id="m23-t057-s040"></a>

### 23. Off-Party Companion

Người đã rời Active Party không được biến mất khỏi giao diện quan hệ.

Nếu player còn biết tình trạng của họ, Companion History hiển thị:

- lần gặp gần nhất;
- nơi ở được biết;
- nghề nghiệp gần nhất được biết;
- quan hệ hiện tại;
- các lời hứa và việc chung;
- những tin tức đã nhận sau khi chia tay.

Nếu không có tin tức mới, phải ghi rõ:

> “Thông tin gần nhất: Miên đang sống ở Bãi Lau, cách đây hai năm.”

Không được cập nhật đời sống riêng của Miên theo thời gian thực nếu không có nguồn.

---

<a id="m23-t057-s041"></a>

## PHẦN VII — WORLD MAP & TRAVEL UI

<a id="m23-t057-s042"></a>

### 24. World Map không phải bản đồ toàn tri

Bản đồ phải hiển thị những gì character biết về địa lý và tuyến đường.

Các trạng thái Knowledge đã khóa:

- Unknown;
- Heard Of;
- Approximate;
- Known;
- Familiar;
- Intimate.

UI cần chuyển chúng thành ngôn ngữ hình ảnh nhất quán.

Ví dụ:

**Heard Of:** Hiển thị tên hoặc vùng tương đối, không tọa độ chính xác.

**Approximate:** Có khu vực ước lượng.

**Known:** Có vị trí và tuyến chính đủ rõ.

**Familiar:** Có đường phụ, landmark và những điều kiện đi lại được biết.

**Intimate:** Có các chi tiết địa phương chuyên sâu đã được học.

Không sử dụng fog-of-war đơn thuần che bằng màu đen như game chiến thuật.

Knowledge Map phải thể hiện cả sự không chắc chắn.

<a id="m23-t057-s043"></a>

### 25. Bốn cấp độ bản đồ

Theo Exploration System:

**World:** 12 đại vùng, tuyến liên vùng, các biến động lớn đã biết.

**Region:** Settlement, route, landscape, điểm đáng chú ý.

**Local Area:** Địa hình gần, nơi có thể khám phá, khu vực tìm kiếm.

**Site/Dungeon:** Các node, lối vào, lối ra và hazard đã được nhận biết.

Giao diện phải cho phép chuyển cấp liền mạch và giữ cùng một marker identity.

Ví dụ một mộ cổ hiển thị ở Regional Map có thể mở ra Local Area. Sau khi vào Site, player xem bản đồ các khu vực đã đi.

<a id="m23-t057-s044"></a>

### 26. Map Layers theo chuyên môn

Có thể có các lớp thông tin:

**Địa lý:** Địa hình, tuyến đường, nước, núi, rừng.

**Xã hội:** Settlement, những người quen và cộng đồng.

**Thương mại:** Nguồn hàng, tuyến vận tải, thông tin thị trường.

**Chính trị:** Faction influence, checkpoint, tình trạng an ninh đã biết.

**Lịch sử:** Các địa điểm từng xảy ra sự kiện.

**Tín ngưỡng:** Địa điểm thiêng, taboo, nơi có hiện tượng Mạch mà character đã biết.

Không phải mọi character đều có đầy đủ các layer.

Một người biết nhiều tuyến đường nhưng không hiểu chính trị có thể có dữ liệu địa lý tốt và political layer rất hạn chế.

<a id="m23-t057-s045"></a>

### 27. Travel Planner

Travel Planner cần đặt các phương án bên cạnh nhau.

Ví dụ:

**Điểm đến: Phong Châu**

| | Tuyến sông | Tuyến bộ | Lối rừng |
|---|---|---|---|
| Thời gian ước tính | 2–3 ngày | 4 ngày | Không chắc |
| Nguồn lực | Cần thuyền | Lương thực cao hơn | Cần người biết đường |
| Điều kiện | Nước đang dâng | Đường tương đối ổn | Tin tức đã cũ |
| Rủi ro đã biết | Dòng chảy mạnh | Có checkpoint | Có thể lạc |
| Chuyên gia trong party | Mân | Nghi | Vũ |

Những con số chỉ được đưa ra khi có đủ cơ sở ước lượng.

Nếu lối rừng chưa từng được khám phá, giao diện không được tự tính ra một thời gian tối ưu với độ chính xác giả tạo.

<a id="m23-t057-s046"></a>

### 28. Thông tin tuyến đường phải có thời điểm

Một route có thể từng an toàn nhưng hiện đã thay đổi.

Marker có thể hiển thị:

> **Bến Hạc — lần cuối xác nhận cách đây 18 ngày.**

Hoặc:

> “Nghe nói đường núi bị sạt sau trận mưa gần đây.”

Khi người chơi đi lại trên route, kiến thức có thể được cập nhật.

Nếu player có companion am hiểu, Travel Planner có thể bổ sung lời khuyên từ người đó.

Nhưng không tự gắn nhận định của companion thành sự thật chắc chắn.

---

<a id="m23-t057-s047"></a>

## PHẦN VIII — JOURNAL SYSTEM

<a id="m23-t057-s048"></a>

### 29. Journal là trung tâm thông tin của game

Đề xuất tên player-facing:

<a id="m23-t057-s049"></a>

## NHẬT KÝ

Journal không chỉ lưu quest.

Nó là không gian người chơi sử dụng để ghi nhớ:

- những vấn đề đang quan tâm;
- điều đã biết;
- điều đang nghi ngờ;
- con người liên quan;
- nơi chốn;
- lời hứa;
- biến cố quá khứ.

Dữ liệu không được sao chép riêng cho từng màn hình.

Journal sử dụng chung Knowledge Projection Layer và Event History.

<a id="m23-t057-s050"></a>

### 30. Cấu trúc Journal

Đề xuất năm góc nhìn chính:

<a id="m23-t057-s051"></a>

#### Những điều đang quan tâm

Current Concerns, Personal Intent và các Story Thread đã biết.

<a id="m23-t057-s052"></a>

#### Manh mối

Claims, Evidence, Contradictions, Hypotheses.

<a id="m23-t057-s053"></a>

#### Người & nơi chốn

Các entity liên quan tới những việc player đang theo dõi.

<a id="m23-t057-s054"></a>

#### Lời hứa & nghĩa vụ

Promises, Debts, Agreements, Commitments.

<a id="m23-t057-s055"></a>

#### Chuyện đã qua

Những thread đã kết thúc hoặc chuyển sang trạng thái lịch sử.

Codex và Chronicle có thể được mở từ Journal, nhưng cần giữ cấu trúc riêng rõ ràng.

<a id="m23-t057-s056"></a>

### 31. Thread Detail

Ví dụ:

**NGƯỜI KHÔNG TRỞ VỀ**

*Tình trạng: Chưa rõ tung tích.*

**Điều đã biết**

Vạc Lâm rời Bến Vạc ba ngày trước.

Mẹ của Lâm đang lo lắng.

Lâm có tranh cãi với cha trước khi rời đi.

**Những điều được kể**

Hạc Dư cho rằng đã nhìn thấy một người giống Lâm đi về phía hạ lưu.

**Manh mối cần xem xét**

Một mái chèo có dấu hiệu thuộc nhà Tòng được tìm thấy gần Cồn Mây.

**Các liên hệ**

Vạc Lâm — người mất tích.

Tòng Mộc — cha.

Hạc Dư — nhân chứng.

**Thời gian**

Lần cuối được nhìn thấy: ba ngày trước, theo lời gia đình.

Giao diện này giúp player hiểu tình hình mà không tự kết luận Lâm đã bị bắt cóc.

<a id="m23-t057-s057"></a>

### 32. Journal hỗ trợ truy xuất lâu dài

Game kéo dài nhiều thập kỷ nên cần chức năng:

- tìm theo tên nhân vật;
- tìm theo settlement;
- lọc theo khoảng thời gian;
- lọc những lời hứa chưa hoàn thành;
- xem những sự kiện liên quan một vật phẩm;
- quay về lần đầu tiên player nhận một tin;
- so sánh các lời kể mâu thuẫn.

Ví dụ player nhớ tới một người thợ đúc từng giúp mình tám năm trước nhưng quên tên.

Họ có thể mở Chronicle của Đồng Đỏ trong giai đoạn đó để tìm lại người.

Tìm kiếm phải chạy trên **những bản ghi được phép biết**, không trực tiếp trên toàn bộ hidden database.

<a id="m23-t057-s058"></a>

### 33. Player Notes

Cho phép người chơi tự ghi:

- giả thuyết;
- nhận xét;
- địa điểm nghi ngờ;
- câu hỏi;
- kế hoạch;
- lời nhắc cá nhân.

Các ghi chú do player nhập không tự trở thành World Truth.

Nếu player viết:

> “Ta tin Nậu Khôn là hung thủ.”

đó vẫn là giả thuyết của người chơi.

NPC và World Engine không được coi nó là lời tố cáo công khai trừ khi player thực sự thực hiện một hành động xã hội tương ứng.

---

<a id="m23-t057-s059"></a>

## PHẦN IX — EVIDENCE BOARD & INVESTIGATION UI

<a id="m23-t057-s060"></a>

### 34. Evidence Board không phải bảng giải đố tự động

Evidence Board dùng để tổ chức thông tin.

Nó không được tự giải án chỉ vì người chơi thu thập đủ số lượng clue.

Đề xuất hai chế độ:

**Danh sách bằng chứng:** Dễ đọc và tìm kiếm.

**Sơ đồ quan hệ:** Hiển thị những liên kết đã biết giữa con người, địa điểm, sự kiện và vật phẩm.

Không buộc mọi player phải sử dụng sơ đồ node.

<a id="m23-t057-s061"></a>

### 35. Cấu trúc một Evidence Node

Mỗi node có thể thuộc:

- Person;
- Place;
- Item;
- Event;
- Testimony;
- Observation;
- Hypothesis.

Các cạnh liên kết phải có ý nghĩa rõ:

**Được nhìn thấy tại**

**Thuộc về**

**Nói rằng**

**Có liên quan thời gian**

**Mâu thuẫn với**

**Có thể dẫn tới**

Ví dụ:

```text
[Hạc Dư]
    |
    | Nói rằng đã nhìn thấy
    v
[Người giống Vạc Lâm]
    |
    | Gần
    v
[Bến hạ lưu]

[Mái chèo]
    |
    | Có dấu hiệu thuộc về
    v
[Nhà Tòng]
```

Đây là mô hình quan hệ thông tin, không phải bằng chứng rằng Hạc Dư nói thật.

<a id="m23-t057-s062"></a>

### 36. Contradiction Handling

Nếu hai nguồn mâu thuẫn, UI cần đặt chúng cạnh nhau.

Ví dụ:

**Hạc Dư:** “Lâm đã xuống thuyền sau khi trời tối.”

**Tòng Mộc:** “Lâm vẫn ở nhà khi trời đã tối.”

Có thể hiện:

> “Hai lời kể có khác biệt về thời điểm.”

Nhưng không đánh dấu tự động:

**Hạc Dư nói dối.**

Character có Skill Suy Luận hoặc Thấu Hiểu cao có thể nhận thêm những nhận định đủ điều kiện.

Tuy nhiên, việc hiểu nguyên nhân mâu thuẫn vẫn cần bằng chứng.

<a id="m23-t057-s063"></a>

### 37. Investigation Assistance

Nên có nhiều mức hỗ trợ giao diện:

**Cơ bản:** Ghi và liên kết bằng chứng.

**Gợi ý theo Knowledge:** Nhắc những điểm đáng chú ý mà character có đủ khả năng suy luận.

**Accessibility Assist:** Làm nổi rõ mối liên hệ, tóm tắt thông tin đã biết, hỗ trợ tìm kiếm.

Không nên đưa ra lời giải có sẵn.

Ví dụ player có ba bằng chứng về thời gian, Character Insight có thể nói:

> “Các thời điểm được kể dường như chưa khớp nhau.”

Không phải:

> “Hung thủ đã thay đổi lời khai lúc 19 giờ.”

---

<a id="m23-t057-s064"></a>

## PHẦN X — CODEX SYSTEM

<a id="m23-t057-s065"></a>

### 38. Codex khác Journal như thế nào?

**Journal:** Những vấn đề cụ thể player đang quan tâm hoặc đã trải qua.

**Codex:** Tri thức tổng quát về thế giới mà character đã tiếp nhận.

Ví dụ:

Journal lưu:

> “Người giữ lễ tại Miếu Rễ đề nghị Mân tham gia nghi thức.”

Codex lưu:

> “Những gì Mân biết về nghi thức tại Miếu Rễ và các phiên bản tập tục liên quan.”

Codex có thể giải thích bối cảnh mà không biến thành sách bách khoa toàn tri.

<a id="m23-t057-s066"></a>

### 39. Các nhóm Codex

| Nhóm | Nội dung |
|---|---|
| Vùng đất | Địa lý, môi trường, lịch sử địa phương |
| Cộng đồng | Tập tục, nghề, tổ chức đời sống |
| Nghề & công nghệ | Vật liệu, quy trình, tri thức chuyên môn |
| Sinh vật & sinh thái | Bestiary, hành vi, dấu vết, nguồn lực |
| Tín ngưỡng & truyền thuyết | Các phiên bản myth, ritual, taboo |
| Lịch sử & quyền lực | Những điều đã biết về faction và biến động lịch sử |
| Khái niệm gameplay | Giải thích các cơ chế cơ bản |

Nhóm cuối là lớp trợ giúp gameplay ngoài thế giới, cần phân biệt rõ với tri thức của nhân vật.

<a id="m23-t057-s067"></a>

### 40. Historical Knowledge Classification

Với nội dung liên quan Văn Lang–Âu Lạc, database thiết kế phải đánh dấu:

**Có căn cứ khảo cổ/lịch sử**

**Giả thuyết hoặc suy luận lịch sử**

**Truyền thuyết/tín niệm**

**Game-canon**

Đề xuất có một tùy chọn **Ghi chú nghiên cứu** dành cho người chơi muốn tìm hiểu cơ sở của thế giới game.

Lớp ghi chú này có thể được truy cập ở chế độ tham khảo ngoài gameplay, không đòi nhân vật trong thế giới cổ phải biết khảo cổ học hiện đại.

Cần tách rõ:

- phần kiến thức nhân vật có thể biết;
- phần giải thích của nhà phát triển về nguồn cảm hứng lịch sử.

Đây là một giải pháp quan trọng để vừa giữ immersion vừa trung thực về lịch sử.

<a id="m23-t057-s068"></a>

### 41. Inline Glossary

Khi một thuật ngữ có khả năng xa lạ xuất hiện trong hội thoại, player có thể mở giải thích ngắn tại chỗ.

Ví dụ NPC nhắc tới một lệ làng hoặc vật phẩm nghi lễ.

UI hiển thị:

**Thuật ngữ**

Ý nghĩa trong ngữ cảnh đang biết.

Nhấn mở rộng để xem các version hoặc thông tin liên quan.

Không cần đưa người chơi ra khỏi hội thoại và mở cả cuốn Codex chỉ để hiểu một danh từ.

Tuy nhiên không được hiển thị trước nội dung bí mật chỉ vì thuật ngữ đó có bản ghi đầy đủ trong database.

---

<a id="m23-t057-s069"></a>

## PHẦN XI — CHRONICLE & HISTORICAL MEMORY

<a id="m23-t057-s070"></a>

### 42. Chronicle phải tách ba loại lịch sử

Một trong những quyết định quan trọng nhất của module này:

**Player Experience Record:** Những gì người chơi thực sự đã trải nghiệm trong các giai đoạn mình điều khiển nhân vật.

**Current Character Knowledge:** Những gì character đang điều khiển biết và có thể sử dụng.

**World Historical Record:** Những biến cố được truyền lại hoặc ghi nhận trong thế giới.

Chúng không tự động trùng nhau.

<a id="m23-t057-s071"></a>

### 43. Chronicle của một cuộc đời

Mỗi playable character có Life Chronicle.

Nội dung gồm:

- thời điểm bắt đầu điều khiển;
- xuất thân và hoàn cảnh;
- bước ngoặt nghề nghiệp;
- Class Emergence;
- các quan hệ quan trọng;
- thương tích hoặc biến cố lớn;
- những việc đã làm;
- các lời hứa;
- di sản;
- cái chết hoặc kết thúc cuộc đời.

Không ghi từng lần ăn uống hoặc đánh một con thú thông thường.

Chỉ những sự kiện có giá trị lâu dài mới được đưa vào bản tóm tắt chính.

<a id="m23-t057-s072"></a>

### 44. Chronicle theo địa điểm

Một location có thể có lịch sử riêng.

Ví dụ:

**Bến Vạc**

Năm 1: Bị ảnh hưởng bởi lũ lớn.

Năm 4: Một tuyến vận tải được mở rộng.

Năm 11: Nhiều hộ gia đình chuyển nghề.

Năm 16: Hàng hóa tới Loa Vực tăng mạnh.

Năm 23: Bến bị ảnh hưởng bởi xung đột.

Player chỉ thấy các mốc có nguồn thông tin hợp lệ trong World History hoặc những điều chính mình đã biết.

Các mốc chưa biết không được tiết lộ bằng một ô trống có tiêu đề gợi ý bí mật.

<a id="m23-t057-s073"></a>

### 45. Chronicle sau Succession

Đây là tình huống đặc biệt.

Giả sử Mân chết và player chuyển sang Miên.

Người chơi ngoài đời vẫn nhớ tất cả những gì mình đã trải nghiệm cùng Mân.

Nhưng Miên không nhất thiết biết toàn bộ các bí mật của Mân.

Đề xuất giao diện có hai lớp phân biệt rõ:

**Nhật ký nhân vật hiện tại:** Những gì Miên biết hoặc đã nhận được hợp lệ.

**Lưu trữ hành trình người chơi:** Những cảnh và sự kiện mà người chơi từng trải nghiệm cùng các protagonist trước, được đánh dấu là thông tin ngoài tri thức hiện tại.

Lớp lưu trữ thứ hai phục vụ trí nhớ của người chơi, không phải khả năng của Miên.

Player có thể tắt hoặc hạn chế lớp này nếu muốn roleplay nghiêm ngặt.

<a id="m23-t057-s074"></a>

### 46. Không cố xóa trí nhớ thật của người chơi

Đây là một vấn đề UX khó nhưng cần giải quyết rõ.

Nếu player từng biết một vị trí bí mật khi điều khiển Mân, họ có thể nhớ vị trí đó sau khi chuyển sang Miên.

Game không thể và không nên cố giả vờ người chơi ngoài đời đã quên.

Thay vào đó:

- Miên không tự có Knowledge/Skill mà Mân sở hữu.
- UI trong thế giới không tự đánh dấu bí mật ấy cho Miên.
- Nếu player chủ động tìm lại địa điểm, hành động phải tuân các điều kiện vật lý, đường đi và năng lực hiện tại.
- Các hành động cần tri thức chuyên môn vẫn đòi khả năng hoặc bằng chứng tương ứng.
- Game không được biến việc người chơi nhớ một vị trí thành cơ chế truyền tri thức siêu nhiên.

Lựa chọn tốt là hỗ trợ roleplay nhưng không tạo những rào cản vô lý ngăn player thử một hành động vật lý hợp lệ.

---

<a id="m23-t057-s075"></a>

## PHẦN XII — NPC PROFILE & SOCIAL GRAPH UI

<a id="m23-t057-s076"></a>

### 47. NPC Profile

Một NPC Profile phải thể hiện điều player biết về người đó, không phải hồ sơ nhân sự toàn tri.

Nội dung có thể gồm:

**Tên và cách nhận diện:** Nếu biết.

**Quan hệ:** Người quen, họ hàng, đồng nghiệp, rival hoặc mối quan hệ đã được xác lập.

**Nghề nghiệp:** Nếu đã biết.

**Faction:** Những affiliation đã được công khai hoặc player phát hiện.

**Thông tin gần nhất:** Nơi ở, hoạt động, tình trạng.

**Những việc chung:** Giao dịch, lời hứa, sự kiện.

**Nhận định:** Những điều player suy ra, có gắn nguồn.

Không được hiển thị:

- toàn bộ secret;
- hidden ambition;
- true affection value;
- toàn bộ thuộc tính bí mật;
- future scripted role.

<a id="m23-t057-s077"></a>

### 48. Relationship UI

Mối quan hệ không thể chỉ là một thanh điểm.

Một người có thể:

- quý player;
- tôn trọng chuyên môn;
- không tin player giữ bí mật;
- đang mắc nợ player;
- giận player vì một việc riêng.

Do đó, Relationship Summary nên dùng các mô tả có ngữ cảnh.

Ví dụ:

> “Nậu Thư đánh giá cao khả năng tổ chức của Mân nhưng vẫn chưa tin Mân có thể giữ bí mật của hội đồng.”

Cần phân biệt nhận định có bằng chứng với cảm xúc thật mà NPC chưa biểu lộ.

Nếu player chỉ suy đoán Nậu Thư đang tức giận, UI không được biến nó thành một kết luận chắc chắn.

<a id="m23-t057-s078"></a>

### 49. Social Graph

Game có mạng xã hội lớn nên không thể vẽ một đồ thị với hàng nghìn NPC cùng lúc.

Đề xuất Social Graph có phạm vi:

**Cá nhân:** Người đang được xem và các quan hệ trực tiếp đã biết.

**Household:** Gia đình, người phụ thuộc, người ở cùng.

**Community:** Những quan hệ quan trọng ở settlement.

**Faction:** Các tổ chức và nhóm lợi ích liên quan.

Player có thể mở rộng từng phần khi cần.

Các cạnh phải có nhãn:

- Họ hàng;
- Đồng nghiệp;
- Người từng giúp đỡ;
- Mắc nợ;
- Có tranh chấp;
- Được nói là có liên hệ.

Nếu quan hệ chỉ xuất phát từ rumor, giao diện phải ghi rõ.

Không được dùng đường nối giống quan hệ đã xác nhận.

---

<a id="m23-t057-s079"></a>

## PHẦN XIII — DIALOGUE UI

<a id="m23-t057-s080"></a>

### 50. Dialogue là không gian đưa ra lựa chọn xã hội

Giao diện hội thoại phải hiển thị:

- người đang nói;
- lời nói;
- bối cảnh cần thiết;
- các lựa chọn có ý định rõ;
- những yếu tố player có cơ sở nhận biết;
- companion có thể tham gia khi hợp lệ.

Không hiển thị mọi Social State nội bộ như các thanh Openness, Irritation, Fear và Trust.

Các trạng thái đó được thể hiện qua hành vi và diễn biến lời nói.

<a id="m23-t057-s081"></a>

### 51. Dialogue Option Preview

Một vấn đề thường gặp trong RPG là player chọn một câu tưởng nhẹ nhàng nhưng character lại nói hoặc thực hiện một điều rất gay gắt.

Để tránh, mỗi option quan trọng cần mô tả rõ ý định.

Ví dụ:

**[Thương lượng — đề nghị đổi phần hàng lấy quyền qua bến]**

**[Viện dẫn luật tục — yêu cầu hội đồng xem lại quyết định]**

**[Đe dọa — nhắc đến lực lượng đang chờ ngoài làng]**

**[Im lặng — không tiết lộ người đã cung cấp tin]**

Với lựa chọn rủi ro cao, có thể thêm mô tả:

> “Lời nói này có thể bị xem là thách thức quyền của trưởng bến trước đám đông.”

Đây là cảnh báo về ý nghĩa hành động mà character có thể hiểu, không phải tiết lộ hậu quả chắc chắn.

<a id="m23-t057-s082"></a>

### 52. Dialogue Check & Transparency

Không nên hiển thị sẵn:

> Persuasion Success 82%.

Vì kết quả còn phụ thuộc động cơ, ranh giới và thông tin của NPC.

Tuy nhiên player có quyền hiểu mình đang dùng loại năng lực nào.

Ví dụ:

**[Thương lượng — Thuyết Phục + hiểu biết lệ bến]**

Sau khi hành động, nếu thất bại, UI có thể diễn đạt:

> “Nậu Khôn thừa nhận đề nghị có lợi, nhưng không chấp nhận vì quyền quyết định thuộc về cả dòng họ.”

Điều này giúp player hiểu nút thắt thực sự.

Nếu có chế độ hiển thị cơ chế nâng cao, có thể cho xem những modifier hợp lệ mà player được biết, nhưng không tiết lộ các hidden motive chưa được phát hiện.

<a id="m23-t057-s083"></a>

### 53. Companion Interjection

Khi một companion xen vào:

- phải có lý do theo knowledge, relationship hoặc emotion;
- người chơi hiểu ai đang đưa ra ý kiến;
- các phản ứng khác nhau có thể tạo social state mới;
- việc companion nói thay không tự đồng nghĩa player đã chọn quan điểm đó.

Ví dụ:

Miên ngắt lời khi player yêu cầu một gia đình giao người bị thương cho quân đội.

Player có thể:

- lắng nghe;
- phản bác;
- yêu cầu cô giải thích;
- để cô thương lượng;
- kết thúc cuộc nói chuyện.

UI phải cho phép những lựa chọn phù hợp thay vì chỉ phát một đoạn companion banter rồi bỏ qua.

---

<a id="m23-t057-s084"></a>

## PHẦN XIV — COMBAT & INJURY UI

<a id="m23-t057-s085"></a>

### 54. Combat UI phải hỗ trợ tactical reasoning

Combat System đã lựa chọn chiến đấu theo lượt, nhấn mạnh:

- positioning;
- terrain;
- Stamina;
- Reaction;
- morale;
- injury;
- formation;
- objective;
- retreat.

UI phải thể hiện các yếu tố đó đủ rõ để player hiểu tình huống.

Không nên tập trung vào con số damage lớn và thanh HP, khiến các objective phi sát thương bị lu mờ.

<a id="m23-t057-s086"></a>

### 55. Combat Layout

Đề xuất có bốn vùng thông tin:

**Battlefield:** Vị trí actor, địa hình, cover, đường đi, vùng nguy hiểm được biết.

**Actor Panel:** Tình trạng của character đang được điều khiển.

**Action Panel:** Những hành động và Technique khả dụng.

**Combat Objective:** Mục tiêu thực tế của encounter.

Ví dụ:

> **Mục tiêu:** Giữ đường qua bến tới khi hai người bị thương được đưa ra ngoài.

Không phải:

> **Mục tiêu:** Giết tất cả đối thủ.

<a id="m23-t057-s087"></a>

### 56. Action Preview

Khi player chọn một hành động, UI cần cho biết những gì có thể dự đoán:

- phạm vi;
- đường di chuyển;
- Stamina Cost;
- Reaction còn lại;
- ảnh hưởng terrain;
- mục tiêu;
- điều kiện Equipment;
- các rủi ro đã nhận biết.

Không được giấu một penalty mà character chuyên môn có khả năng nhận ra.

Ví dụ chiến đấu trong bùn:

Một nhân vật có kinh nghiệm di chuyển ở đầm có thể biết mặt đất khiến phản ứng chậm đi.

Người thiếu kinh nghiệm vẫn có thể nhận được cảnh báo tổng quát nếu nền đất rõ ràng bất ổn.

<a id="m23-t057-s088"></a>

### 57. Combat Feedback

Khi một đòn đánh trúng, player cần hiểu:

- đòn đánh có trúng không;
- chắn/né có xảy ra không;
- armor có giảm tác động không;
- thương tích hoặc trạng thái gì xuất hiện;
- vị trí có thay đổi không;
- đối phương còn có thể tiếp tục hành động không.

Ví dụ:

> “Mũi giáo chạm vào phần che chắn ở vai. Nậu Đinh bị đẩy lùi một bước nhưng chưa mất khả năng cầm khiên.”

Thông tin phải khớp hoàn toàn với Combat Rule Engine.

Không được để LLM mô tả nhân vật gãy tay trong khi Injury State không ghi nhận điều đó.

<a id="m23-t057-s089"></a>

### 58. Injury & Treatment

Injury UI cần tách:

**Vị trí thương tích**

**Mức độ biểu hiện đã được đánh giá**

**Ảnh hưởng chức năng**

**Tình trạng xử lý**

**Khả năng hồi phục dự kiến**

Không phải mọi character đều chẩn đoán chính xác như Dược Nhân.

Một người không có chuyên môn có thể thấy:

> “Vết thương ở cẳng chân, chảy máu và khó đứng.”

Một Dược Nhân có thể nhận định cụ thể hơn dựa trên kiến thức và kiểm tra thực tế.

Các quyết định điều trị phải có chi phí thời gian, vật tư và rủi ro được trình bày rõ ở mức character hiểu được.

---

<a id="m23-t057-s090"></a>

## PHẦN XV — INVENTORY, EQUIPMENT & OWNERSHIP UI

<a id="m23-t057-s091"></a>

### 59. Inventory phải phân biệt mang theo và sở hữu

Game đã khóa rằng:

- companion sở hữu đồ riêng;
- household và workshop có tài sản;
- vật phẩm có thể được mượn;
- tài sản có thể có nhiều loại quyền sử dụng;
- vật phẩm có provenance.

Vì vậy Inventory không thể chỉ là một danh sách đồ thuộc về player.

Đề xuất phân loại:

**Đang mang theo**

**Tài sản cá nhân**

**Đồ dùng chung đã thỏa thuận**

**Đang mượn**

**Đang gửi giữ**

**Tài sản tại settlement/workshop**

<a id="m23-t057-s092"></a>

### 60. Item Detail

Một vật phẩm quan trọng có thể hiển thị:

- tên;
- chức năng;
- material;
- quality;
- condition;
- weight/bulk;
- người chế tác nếu biết;
- nguồn gốc;
- chủ sở hữu hoặc quyền sử dụng;
- các thay đổi từng trải qua.

Ví dụ:

**Giáo đầu đồng của Vạc Tòng**

Chế tác: Không rõ.

Người đang giữ: Mân.

Quyền sở hữu: Thuộc gia đình nhà Tòng.

Tình trạng: Cán đã sửa sau trận lũ.

Ý nghĩa: Vật được cha giao cho Mân khi rời bến.

Không nhất thiết vật phẩm này có combat stats cao.

Nhưng về narrative và ownership, nó có giá trị lớn.

<a id="m23-t057-s093"></a>

### 61. Equipment Comparison

So sánh trang bị phải theo công năng.

Ví dụ:

Giáo A có độ dài tốt hơn nhưng khó dùng ở nơi hẹp.

Giáo B nhẹ, dễ mang khi Travel, nhưng ít lợi thế giữ khoảng cách.

Không nên chỉ hiển thị:

> A: Power 24.

> B: Power 18.

Player có thể mở Advanced Stats nếu cần.

Mức độ hiển thị phải đủ để người chơi hiểu trade-off mà không bắt đọc hàng chục thông số vật liệu.

---

<a id="m23-t057-s094"></a>

## PHẦN XVI — CRAFTING, MARKET & SETTLEMENT UI

<a id="m23-t057-s095"></a>

### 62. Crafting Workspace

Crafting UI cần mô phỏng một quy trình có điều kiện, không chỉ nút chế tạo.

Một Workspace nên hiển thị:

**Thiết kế:** Muốn làm vật gì và để phục vụ mục đích gì.

**Vật liệu:** Có những lựa chọn nào, chất lượng ra sao.

**Công cụ và nơi làm:** Có đáp ứng yêu cầu không.

**Nhân lực:** Ai đang tham gia và chuyên môn gì.

**Thời gian:** Khoảng thời gian cần thiết.

**Rủi ro:** Các điểm chưa chắc chắn mà character có thể đánh giá.

**Kết quả mong đợi:** Các thuộc tính công năng và giới hạn có thể dự đoán.

Ví dụ khi chọn gỗ cho cán giáo, UI cho biết:

> “Loại gỗ này nhẹ và dễ xử lý, nhưng người thợ cho rằng nó có thể kém bền trong điều kiện ẩm.”

Nếu character không có Material Knowledge tương ứng, nhận định có thể chỉ xuất hiện khi hỏi thợ chuyên môn.

<a id="m23-t057-s096"></a>

### 63. Market UI

Market Screen phải phân biệt:

**Giá trị giao dịch hiện tại**

**Nguồn cung tại đây**

**Nguồn thông tin**

**Mức chắc chắn về giá và chất lượng**

Ví dụ:

**Muối tại Phong Châu**

Giá hiện tại: Cao so với mức thương nhân này thường giao dịch.

Lý do được biết: Những chuyến hàng từ Hải Môn tới chậm.

Nguồn: Lời người bán và một thương nhân khác.

Player có thể:

- hỏi thêm;
- kiểm tra hàng;
- thương lượng;
- mua;
- đổi vật;
- tìm nguồn cung khác.

Không cần mở Faction Screen để biết một giao dịch bình thường.

<a id="m23-t057-s097"></a>

### 64. Settlement Profile

Settlement UI cần có hai tầng.

**Tầng tổng quan:** Người chơi nắm được địa phương đang thịnh vượng, khó khăn, đông đúc hay căng thẳng như thế nào.

**Tầng chuyên sâu:** Người có Knowledge hoặc authority phù hợp có thể xem những dữ liệu sâu hơn về nguồn lực, labor, trade, infrastructure và social pressure.

Ví dụ một khách qua đường có thể biết:

> “Bến Nếp đang gặp khó khăn vì bến đò bị phá.”

Một người quản lý tuyến vận tải có thể có thêm thông tin về:

- capacity;
- hàng hóa bị chậm;
- tuyến thay thế;
- lượng người cần vận chuyển.

Một Thủ Lĩnh có thể tiếp cận dữ liệu quản lý nếu thật sự có thẩm quyền.

Không được cho tất cả player cùng một dashboard kinh tế toàn tri.

<a id="m23-t057-s098"></a>

### 65. Settlement Project UI

Dự án cần hiển thị:

- mục tiêu;
- phương án;
- tài nguyên;
- người liên quan;
- thời gian;
- lợi ích dự kiến;
- tác động có thể gây tranh cãi;
- điều kiện duy trì.

Ví dụ:

**Xây cầu Bến Nếp**

Cần nhiều vật liệu và lao động hơn phương án phục hồi bến đò.

Tuyến đi có thể ổn định hơn.

Gia đình đang sống bằng nghề đò có thể mất thu nhập.

Đây là thông tin thực tế giúp player đưa ra quyết định kinh tế và xã hội, không phải chỉ nhìn thấy ô Build Cost.

---

<a id="m23-t057-s099"></a>

## PHẦN XVII — FACTION & POLITICAL UI

<a id="m23-t057-s100"></a>

### 66. Không có Faction Reputation Bar duy nhất

Faction Profile phải phản ánh:

- identity;
- mục tiêu đã biết;
- leadership;
- internal groups;
- influence;
- relationship với player;
- những claim của faction về player;
- các lời hứa hoặc tranh chấp;
- mức độ cũ của thông tin.

Ví dụ:

Một faction có thể xem player là người hữu ích về vận tải nhưng không đáng tin về chính trị.

Không thể rút gọn thành:

> Faction Reputation: 75/100.

<a id="m23-t057-s101"></a>

### 67. Influence Map

Political Map có thể hiển thị:

- khu vực kiểm soát;
- khu vực ảnh hưởng;
- nơi có tranh chấp;
- các route liên quan;
- tình trạng checkpoint;
- các biến động đã được nghe kể.

Nếu player không có nguồn tình báo mới, lớp political influence phải phản ánh thông tin cũ.

Có thể hiển thị:

> “Thông tin về Bắc Lộ được gửi từ 12 ngày trước.”

Không được dùng bản đồ làm công cụ báo vị trí quân đội thời gian thực khi không có nguồn.

<a id="m23-t057-s102"></a>

### 68. Negotiation UI

Một cuộc thương lượng lớn cần có Workspace riêng hoặc panel mở rộng trong Dialogue.

Nội dung có thể gồm:

**Những bên tham gia**

**Điều mỗi bên đã tuyên bố muốn**

**Những điều kiện đã được đề xuất**

**Những lời hứa đang được đặt lên bàn**

**Những điểm bất đồng đã biết**

**Các phương án mà player có thể đưa ra**

Không được hiển thị hidden red lines của NPC nếu chưa được phát hiện.

Sau khi đạt thỏa thuận, hệ thống tạo Agreement Object và cho player xem lại đầy đủ những điều khoản đã cam kết.

Nếu thỏa thuận có hậu quả dài hạn, UI phải giúp người chơi hiểu mình đang hứa điều gì trước khi xác nhận.

---

<a id="m23-t057-s103"></a>

## PHẦN XVIII — BELIEF, MẠCH & BESTIARY UI

<a id="m23-t057-s104"></a>

### 69. Belief UI

Game đã khóa rằng Belief không phải một thanh Faith.

Vì vậy nhân vật có thể được mô tả:

> “Tin tưởng vào nghi thức tưởng nhớ tổ tiên, nhưng nghi ngờ những người diễn giải điềm mộng.”

Một Vu Chúc có thể có Ritual Knowledge rất cao dù Cảm Mạch thấp.

Một người Cảm Mạch cao có thể không hiểu tập tục.

UI phải giữ các khác biệt đó.

<a id="m23-t057-s105"></a>

### 70. Mạch UI

Mạch Lạc không được hiển thị như mana.

Nếu character có khả năng cảm nhận một địa điểm, UI có thể cho biết:

- cảm giác bất thường;
- dấu hiệu mà nhân vật nhận thấy;
- sự thay đổi sau một nghi lễ;
- các lời kể liên quan;
- tình trạng đã được chuyên môn đánh giá.

Ví dụ:

> “Nơi này khiến Miên nhớ lại giấc mơ đêm trước, nhưng cô không chắc điều đó có ý nghĩa gì.”

Không tự động gắn:

> “Mạch Disturbance: 82%.”

Đối với NPC không có Cảm Mạch hoặc không có nguồn tin, lớp này có thể không xuất hiện.

<a id="m23-t057-s106"></a>

### 71. Bestiary UI

Bestiary cần hỗ trợ gameplay săn bắt và sinh thái, không chỉ sưu tập quái vật.

Mỗi entry có thể gồm:

- cách nhận diện;
- habitat;
- dấu vết;
- hành vi;
- khi nào nguy hiểm;
- kỹ thuật tránh hoặc săn;
- nguồn lực có thể sử dụng;
- các câu chuyện và taboo;
- những lần player từng gặp.

Nếu người địa phương kể về một con thú không rõ, entry có thể nằm trong nhóm **Lời kể về sinh vật** cho tới khi có căn cứ phân loại.

Không tự động xác nhận nó là thần thú hoặc động vật thật.

---

<a id="m23-t057-s107"></a>

## PHẦN XIX — CALENDAR, DOWNTIME & TIME SKIP UI

<a id="m23-t057-s108"></a>

### 72. Calendar phải phản ánh thời gian sống

Game kéo dài khoảng 20–25 năm.

Calendar không nên là bảng ngày tháng hiện đại với hàng trăm lịch hẹn.

Đề xuất:

**Hiện tại:** Năm chiến dịch, giai đoạn mùa, thời gian trong ngày.

**Mốc sắp tới:** Những hoạt động và hạn chót character đã biết.

**Chu kỳ cộng đồng:** Mùa vụ, hoạt động nghề, lễ hội và lịch liên quan.

**Mốc đời người:** Sự kiện quan trọng của bản thân, gia đình và companion.

Mỗi vùng có thể diễn giải mùa theo môi trường của mình.

<a id="m23-t057-s109"></a>

### 73. Downtime Planner

Khi player chọn dành thời gian cho một hoạt động dài hạn, UI cần thể hiện:

**Định hướng:** Học nghề, hồi phục, chăm sóc gia đình, thực hiện công trình...

**Điều kiện:** Mentor, tài nguyên, nơi làm, thời gian.

**Rủi ro có thể dự đoán:** Lời hứa tới hạn, sự kiện vùng, tình trạng sức khỏe.

**Những gì chưa thể bảo đảm:** Kết quả học tập, sự thay đổi của người khác, biến cố thế giới.

Ví dụ:

**Dành sáu tháng theo học tại Lò Cốc**

Có mentor đã đồng ý.

Workshop hoạt động.

Cần chi phí sinh hoạt.

Player có một cam kết tại Bến Vạc trong ba tháng tới.

Không được hiển thị:

> “Sau sáu tháng chắc chắn đạt Mastery 62.”

<a id="m23-t057-s110"></a>

### 74. Time Skip Confirmation

Time Skip nhiều tháng hoặc nhiều năm là hành động có sức nặng lớn.

Trước khi xác nhận, cần hiện:

- khoảng thời gian dự kiến;
- hoạt động player đã chọn;
- các nghĩa vụ quan trọng sắp đến hạn mà character biết;
- các dự án đang cần quyết định;
- companion hoặc household phụ thuộc vào character;
- lời nhắc rằng thế giới sẽ tiếp tục vận hành.

Không tiết lộ biến cố tương lai chưa xảy ra.

Ví dụ:

> “Bạn dự định dành hai năm tiếp theo để tiếp tục nghề tại Lò Cốc. Trong thời gian này, các cộng đồng, phe phái và những người bạn quen biết vẫn tiếp tục cuộc sống của họ.”

Sau đó player xác nhận.

Không nên bắt người chơi giữ nút quá lâu nếu có thể dùng thao tác xác nhận rõ ràng và dễ tiếp cận.

<a id="m23-t057-s111"></a>

### 75. Time Skip Report

Sau một giai đoạn dài, báo cáo cần có cấu trúc:

**Bản thân**

**Gia đình**

**Đồng hành**

**Nghề nghiệp**

**Thế giới**

**Những vấn đề còn dang dở**

Có thể cho player mở từng mục để xem sâu hơn.

Những tin mới nhận được phải có nguồn và thời điểm.

Nếu một companion qua đời nhưng tin tức chưa tới nhân vật, report không được tiết lộ sự kiện đó.

---

<a id="m23-t057-s112"></a>

## PHẦN XX — DEATH, SUCCESSION & LEGACY UI

<a id="m23-t057-s113"></a>

### 76. Death Screen

Death Screen không nên chỉ ghi:

**GAME OVER.**

Nó cần trình bày có căn cứ:

- sự kiện cuối cùng của character;
- điều đã xảy ra mà player trực tiếp chứng kiến;
- ai đang có mặt;
- tình trạng party;
- những hậu quả trước mắt đã biết.

Những hậu quả còn lại tiếp tục được World Simulation xử lý.

Nếu nhân vật chết ở nơi không ai chứng kiến, game không được bịa ra một lễ tang ngay lập tức.

<a id="m23-t057-s114"></a>

### 77. Succession Screen

Nếu có người kế tục hợp lệ, giao diện trình bày:

**Ứng viên**

**Quan hệ với người đã chết**

**Hoàn cảnh hiện tại**

**Những điều người chơi biết về họ**

**Khả năng tiếp tục câu chuyện**

Ví dụ:

**Miên — Người chữa bệnh tại Bãi Lau**

Từng đồng hành cùng Mân trong nhiều năm.

Chứng kiến cái chết của Mân.

Có nghề nghiệp riêng và người thân cần chăm sóc.

Biết một phần những việc Mân đã làm.

Không nên hiển thị toàn bộ hidden stats, secret ambition hoặc future storyline.

<a id="m23-t057-s115"></a>

### 78. Legacy Screen

Legacy không phải bảng điểm.

Đề xuất các nhóm:

**Những người còn nhớ**

**Những gì đã truyền lại**

**Tài sản và công trình**

**Lời hứa còn tồn tại**

**Những thay đổi trong cộng đồng**

**Những câu chuyện được kể**

Mỗi entry có thể thay đổi theo thời gian.

Một cây cầu từng do player xây có thể được bảo dưỡng, đổi tên hoặc bị lũ phá sau nhiều năm.

Legacy UI phải theo dõi sự thay đổi đó, thay vì coi mọi thành tựu là vật phẩm vĩnh viễn trong hồ sơ.

---

<a id="m23-t057-s116"></a>

## PHẦN XXI — PLAYER FEEDBACK SYSTEM

<a id="m23-t057-s117"></a>

### 79. Feedback cần hoạt động theo ba khoảng thời gian

<a id="m23-t057-s118"></a>

#### Immediate Feedback

Hành động vừa xảy ra tạo ra điều gì có thể quan sát?

Ví dụ:

> “Bạn đã cố định được phần cán giáo bị lỏng.”

<a id="m23-t057-s119"></a>

#### Delayed Feedback

Hậu quả xuất hiện sau một khoảng thời gian hoặc qua mạng thông tin.

Ví dụ:

> “Một thương nhân nói rằng tuyến qua Bến Nếp đã đông hơn kể từ khi cây cầu hoàn thành.”

<a id="m23-t057-s120"></a>

#### Historical Feedback

Nhiều năm sau, kết quả trở thành một phần của thế giới.

Ví dụ:

> “Người trẻ trong làng không còn biết bến đò cũ từng ở đâu.”

Cả ba cần dùng chung nguồn dữ liệu.

<a id="m23-t057-s121"></a>

### 80. Feedback phải phân biệt hành động và cách người khác hiểu hành động

Ví dụ player lấy lương thực để cứu người bị nạn.

**Objective Action:** Đã lấy một phần grain từ kho.

**Witness Interpretation A:** Player cứu người.

**Witness Interpretation B:** Player lấy hàng không có phép.

**Faction Interpretation:** Hành vi có thể bị xem là vi phạm quyền phân phối.

Giao diện không được hiện:

> “Good Deed +20 Reputation.”

Có thể ghi:

> “Một số người biết bạn đã dùng lương thực trong kho để cứu đoàn người mắc kẹt.”

Những phản ứng cụ thể chỉ xuất hiện khi player quan sát hoặc nhận được tin.

<a id="m23-t057-s122"></a>

### 81. Action Result Explanation

Player cần được giải thích kết quả một cách công bằng.

Ví dụ một người không chịu cho mượn thuyền.

Không nên chỉ báo:

> “Thuyết phục thất bại.”

Nếu character nhận biết lý do, có thể trình bày:

> “Ông ta có vẻ không phản đối khoản tiền bạn đưa ra, nhưng vẫn lo sẽ không có thuyền để đi đánh cá cho gia đình.”

Điều này giúp player hiểu rằng vấn đề có thể nằm ở nhu cầu sử dụng thuyền, không phải Skill thiếu điểm.

Nếu NPC che giấu lý do, player không được nhìn thấy toàn bộ motive trong màn giải thích.

<a id="m23-t057-s123"></a>

### 82. Feedback không nên spam

Không cần thông báo:

- Skill +0.1;
- Relationship +1;
- Profession Progress +2%;
- Mạch +3;
- Economy Impact +0.5.

Những thay đổi nhỏ có thể được tích lũy và trình bày khi có ý nghĩa.

Ví dụ:

> “Sau nhiều tháng làm việc, Mân đã quen hơn với việc đọc dòng nước ở khúc sông này.”

Các biến đổi cơ học vẫn được lưu đầy đủ, nhưng UI chỉ expose những thông tin thực sự giúp player hiểu tiến trình.

---

<a id="m23-t057-s124"></a>

## PHẦN XXII — NOTIFICATION SYSTEM

<a id="m23-t057-s125"></a>

### 83. Phân loại thông báo

Đề xuất bốn mức.

**Critical:** Cần xử lý ngay trong tình huống đang diễn ra.

**Important:** Có thay đổi đáng kể hoặc mối quan tâm cần lưu ý.

**Informational:** Tin tức hoặc tiến triển không khẩn cấp.

**Ambient:** Sự kiện nhỏ có thể tóm tắt hoặc ghi log.

Ví dụ:

Một companion đang mất máu nghiêm trọng là Critical.

Một lời hứa sắp tới hạn là Important nếu player biết deadline.

Một thương đoàn đến bến là Informational.

Một buổi sáng mưa nhẹ là Ambient.

<a id="m23-t057-s126"></a>

### 84. Notification không được tiết lộ hidden truth

Nếu một người thân ở xa qua đời, không được tạo popup toàn màn hình:

> “Người cha đã chết!”

khi character chưa nhận tin.

Event vẫn xảy ra trong World State.

Thông báo chỉ xuất hiện khi:

- character chứng kiến;
- một người báo tin;
- bằng chứng được phát hiện;
- hoặc thông tin lan tới player theo một kênh hợp lệ.

Đây là quy tắc áp dụng cho mọi loại event.

<a id="m23-t057-s127"></a>

### 85. Notification History

Player phải có khả năng xem lại các thông báo quan trọng.

Ví dụ đang chiến đấu, một số thông tin xuất hiện nhưng player chưa đọc.

Game cần lưu chúng vào Combat Log hoặc Recent Events.

Sau khi ra khỏi combat, player có thể xem lại.

Không cần lưu vĩnh viễn mọi thông báo thời tiết nhỏ.

---

<a id="m23-t057-s128"></a>

## PHẦN XXIII — ACCESSIBILITY & READABILITY

<a id="m23-t057-s129"></a>

### 86. Text Scaling

Game dự kiến có rất nhiều văn bản.

Do đó phải hỗ trợ:

- tăng kích thước chữ;
- reflow nội dung;
- tăng khoảng cách dòng khi cần;
- font dễ đọc;
- tương phản cao;
- tránh cắt nội dung khi scale;
- không yêu cầu cuộn cả ngang lẫn dọc để đọc một đoạn văn dài.

Mục tiêu kiểm thử nên bao gồm phóng to giao diện và phụ đề tới mức lớn mà bố cục vẫn hoạt động hợp lý.

Font nghệ thuật lấy cảm hứng từ vật liệu lịch sử có thể dùng cho tiêu đề hoặc trang trí, nhưng cần có kiểu chữ dễ đọc hoàn chỉnh cho nội dung chính.

<a id="m23-t057-s130"></a>

### 87. Không dùng màu làm tín hiệu duy nhất

Ví dụ Evidence Board có các loại nguồn thông tin.

Không nên phân biệt chỉ bằng:

- xanh;
- vàng;
- đỏ.

Cần thêm:

- icon;
- nhãn;
- kiểu đường nối;
- tooltip;
- vị trí và nhóm.

Một lời kể chưa được xác nhận phải có nhãn văn bản, không chỉ một màu khác.

<a id="m23-t057-s131"></a>

### 88. Subtitle & Audio Cue

Nếu NPC nói chuyện hoặc có dấu hiệu âm thanh quan trọng, game cần hỗ trợ:

- phụ đề hội thoại;
- tên người nói;
- tùy chỉnh cỡ chữ và nền;
- caption mô tả âm thanh có giá trị gameplay;
- chỉ hướng âm thanh khi cần.

Ví dụ trong rừng:

> “[Tiếng cành cây gãy ở phía trái, sau bụi rậm.]”

Đây không phải tiết lộ có thú hoặc người nếu character chỉ nghe tiếng động.

Caption phải mô tả thông tin âm thanh đúng mức.

<a id="m23-t057-s132"></a>

### 89. Keyboard, Mouse & Controller

Vì game có nhiều menu phức tạp, việc hỗ trợ controller phải được đưa vào prototype sớm.

Các khu vực cần kiểm thử:

- di chuyển focus trong Character Sheet;
- chọn node trong Evidence Board;
- tìm người trong Social Graph;
- chọn route trên World Map;
- kéo thả hoặc lựa chọn vật liệu trong Crafting;
- điều hướng nhiều bước trong Journal.

Mọi thao tác cần có phương án không phụ thuộc duy nhất vào kéo thả hoặc hover chuột.

Với Evidence Board, một danh sách liên kết có thể truy cập bằng bàn phím/tay cầm sẽ giúp cả accessibility và usability.

<a id="m23-t057-s133"></a>

### 90. Cognitive Accessibility

Người chơi có thể nghỉ game nhiều ngày hoặc nhiều tuần ngoài đời.

Khi quay lại, game nên có **Nhắc lại hành trình gần đây**.

Bản tóm tắt gồm:

- character hiện tại;
- nơi đang đứng;
- mối quan tâm đã ghim;
- sự kiện gần nhất;
- lời hứa sắp tới hạn;
- hoạt động đang dở.

Nội dung chỉ dùng những thông tin player đã biết.

Không bắt player nhớ tên hàng chục người để tiếp tục.

<a id="m23-t057-s134"></a>

### 91. Pause, Reading Time & Confirmation

Khi người chơi mở Journal, Codex, Character hay World Map để đọc, mặc định **World Time không nên tiếp tục trôi chỉ vì người chơi đang đọc menu** trong trò chơi đơn.

Thời gian thế giới tiến khi player thực hiện các hành động có chi phí thời gian: Travel, Work, Rest, Wait hoặc những lựa chọn tương ứng.

Trong các tình huống đang có diễn biến khẩn cấp, game phải định nghĩa rõ thời gian được tạm dừng hay tiếp tục, nhưng không được dùng một timer ẩn để phạt người đọc chậm.

Đối với hành động khó đảo ngược như Time Skip nhiều năm, chuyển giao tài sản quan trọng hoặc xác nhận thỏa thuận, UI cần có cơ chế review và confirmation hợp lý.

---

<a id="m23-t057-s135"></a>

## PHẦN XXIV — MỸ THUẬT GIAO DIỆN

<a id="m23-t057-s136"></a>

### 92. Visual Identity

Giao diện cần mang hơi thở Văn Lang–Âu Lạc nhưng không cố sao chép nguyên xi hiện vật khảo cổ sang từng màn hình.

Đề xuất định hướng:

**Vật liệu:** Gỗ, sợi, đất, đồng, bề mặt tự nhiên.

**Hình khối:** Hoa văn hình học và những yếu tố được nghiên cứu từ mỹ thuật Đông Sơn khi phù hợp.

**Màu sắc:** Những bảng màu gắn với đất, nước, thực vật và kim loại.

**Bản đồ:** Minh họa địa hình mang tính biểu tượng, không giống vệ tinh.

**Journal:** Tư duy sổ ghi chép hoặc ký ức được tổ chức, nhưng không bắt buộc nhân vật thật sự biết chữ.

**Chronicle:** Nhấn mạnh thời gian, con người và dấu vết của sự thay đổi.

Các yếu tố trên là gợi ý nghệ thuật, không phải khẳng định cư dân ở thời kỳ này đã sử dụng những giao diện hay hệ bản đồ giống hình minh họa.

<a id="m23-t057-s137"></a>

### 93. Tránh hai cực đoan về visual style

**Cực đoan thứ nhất:** Dùng UI fantasy phương Tây với sách da, rune, ngọc phép, thanh mana và inventory icon quen thuộc.

Điều này làm mất bản sắc của game.

**Cực đoan thứ hai:** Cố tái tạo vật liệu cổ đến mức khó đọc, khó điều hướng và thiếu sự rõ ràng.

Điều này làm UX kém.

Hướng tốt hơn là **historically inspired modern interface**.

Hình ảnh có thể gợi bối cảnh, nhưng thông tin và thao tác phải đáp ứng nhu cầu người chơi hiện đại.

<a id="m23-t057-s138"></a>

### 94. Journal không nhất thiết là vật phẩm vật lý

Đây là ranh giới quan trọng.

Một nhân vật không biết chữ vẫn cần sử dụng Journal UI.

Do đó Journal là **công cụ hỗ trợ trí nhớ và quản lý thông tin của người chơi**, không nhất thiết là cuốn sổ mà nhân vật thật sự viết ra.

Nếu trong game có một vật phẩm ghi chép vật lý, đó là một Item riêng.

Vật phẩm ấy:

- có chủ sở hữu;
- có nội dung cụ thể;
- có thể bị mất;
- có thể được truyền;
- có thể bị người khác đọc nếu có khả năng phù hợp.

Không được đồng nhất vật phẩm đó với toàn bộ dữ liệu Journal nằm ở UI.

---

<a id="m23-t057-s139"></a>

## PHẦN XXV — UI DATA CONTRACT & LLM INTEGRATION

<a id="m23-t057-s140"></a>

### 95. UI cần dữ liệu có cấu trúc

Không nên để LLM tự viết toàn bộ nội dung màn hình từ một prompt mô tả chung.

Ví dụ, một NPC Profile View Model có thể chứa:

```json
{
  "entity_id": "npc_vac_lam",
  "display_name": "Vạc Lâm",
  "known_role": "Người nhà Vạc Tòng",
  "last_known_location": "Bến Vạc",
  "last_information_age": "3 ngày",
  "known_claims": [
    {
      "claim": "Rời nhà sau một cuộc tranh cãi",
      "source": "Mẹ của Lâm",
      "status": "lời kể"
    }
  ]
}
```

Đây chỉ là ví dụ kỹ thuật về dữ liệu đã được lọc.

Nó không chứa:

- hidden location;
- true motive;
- secret faction;
- future event.

<a id="m23-t057-s141"></a>

### 96. Presentation Layer

LLM có thể hỗ trợ:

- diễn đạt bản tóm tắt Chronicle;
- chuyển trạng thái thành lời văn tự nhiên;
- viết lời kể NPC;
- mô tả thay đổi settlement;
- tạo bản tóm tắt sau Time Skip.

Nhưng toàn bộ sự kiện, dữ liệu và quyền hiển thị phải được xác lập trước bởi Rule Engine và Knowledge Projection Layer.

Ví dụ:

Engine ghi nhận Mân đã sửa thuyền.

LLM có thể viết:

> “Mân dành phần lớn buổi chiều gia cố phần mạn thuyền đã lỏng. Đến khi mặt trời xuống thấp, con thuyền có thể tiếp tục di chuyển.”

LLM không được thêm:

> “Người chủ thuyền xúc động và quyết định nhận Mân làm con nuôi.”

nếu Social Event tương ứng chưa xảy ra.

<a id="m23-t057-s142"></a>

### 97. Consistency giữa các màn hình

Nếu Journal ghi Lâm được nhìn thấy lần cuối ba ngày trước, NPC Profile và Travel Information không được tự ghi thành hôm qua.

Nếu một cây cầu đã bị phá, Map không được hiển thị như còn sử dụng bình thường trừ khi thông tin map đã cũ và được trình bày như vậy.

Đề xuất áp dụng:

- stable Entity IDs;
- View Model Projection;
- Knowledge Validation;
- Event Timestamp;
- Cache Invalidation;
- UI Consistency Tests.

Đây là trách nhiệm của kiến trúc dữ liệu, không thể xử lý chỉ bằng prompt yêu cầu LLM “hãy nhất quán”.

---

<a id="m23-t057-s143"></a>

## PHẦN XXVI — END-TO-END UI SCENARIO

<a id="m23-t057-s144"></a>

### 98. Tình huống: Cầu Bến Hạc bị phá

<a id="m23-t057-s145"></a>

#### Bước 1 — Player nghe tin

Một người đánh cá nói:

> “Có người phá cầu Bến Hạc.”

Journal tạo một Claim Record với source.

Map chưa xác nhận cầu thật sự bị phá hoại.

Travel Planner có thể hiển thị:

> “Có tin cầu Bến Hạc không sử dụng được; nguyên nhân chưa rõ.”

<a id="m23-t057-s146"></a>

#### Bước 2 — Player đến hiện trường

Player thấy cầu hỏng.

Knowledge cập nhật:

**Quan sát trực tiếp:** Cầu bị hỏng.

**Lời kể:** Có người cho rằng cầu bị phá.

Hai entry tồn tại song song.

<a id="m23-t057-s147"></a>

#### Bước 3 — Player điều tra

Một Xảo Nhân có thể nhận ra dấu vết hư hại phù hợp với nước lũ.

Một Trinh Binh tìm được dấu chân mới.

Evidence Board có thể cho thấy các dấu hiệu khác nhau nhưng chưa đủ kết luận chúng có liên quan trực tiếp.

<a id="m23-t057-s148"></a>

#### Bước 4 — Player đưa ra quyết định

Player chọn:

- sửa cầu;
- báo cộng đồng;
- tìm người bị nghi;
- đi tuyến khác;
- thuyết phục ai đó mở ferry.

Không cần đợi hệ thống xác nhận toàn bộ sự thật mới được hành động.

<a id="m23-t057-s149"></a>

#### Bước 5 — Kết quả

Player hỗ trợ sửa cầu.

Physical World State cập nhật.

Travel Route có thể mở trở lại.

Journal ghi công việc đã làm.

Một gia đình sống bằng nghề đò có thể không vui vì cầu hoạt động trở lại.

<a id="m23-t057-s150"></a>

#### Bước 6 — Hậu quả lâu dài

Một năm sau, player trở lại.

Cầu vẫn được sử dụng nếu được duy trì.

Chronicle ghi nhận một sự kiện công trình.

Rumor về người phá cầu có thể còn tồn tại dù bằng chứng kỹ thuật đã khiến player nghi ngờ.

**Cùng một sự kiện được trình bày nhất quán ở World Map, Journal, Evidence, Economy, Social Graph và Chronicle.**

Đây là tiêu chuẩn thực sự của Information Architecture.

---

<a id="m23-t057-s151"></a>

## PHẦN XXVII — CÁC TÌNH HUỐNG BIÊN CẦN THIẾT KẾ

<a id="m23-t057-s152"></a>

### 99. NPC đổi tên, đổi vai trò hoặc chết

Một NPC có thể lấy tên gọi khác, đổi vai trò xã hội, chuyển vùng hoặc qua đời.

UI cần lưu Entity ID ổn định.

Nếu player chỉ biết một biệt danh, hồ sơ hiển thị biệt danh.

Khi phát hiện tên thật, có thể cập nhật thông tin mà không tạo NPC thứ hai.

Sau khi NPC chết, profile vẫn tồn tại với thông tin mà character được phép biết.

<a id="m23-t057-s153"></a>

### 100. Người chơi có nhiều lời kể về cùng một sự kiện

Không được ghi đè lời kể cũ bằng lời kể mới.

Phải giữ:

- từng source;
- timestamp;
- mức độ mâu thuẫn;
- trạng thái xác minh.

Một sự kiện có thể có nhiều bản kể qua nhiều năm.

Đây là nền tảng của Myth Formation.

<a id="m23-t057-s154"></a>

### 101. Player mất một vật chứng

Nếu vật chứng bị mất hoặc hỏng:

- thông tin player từng quan sát có thể vẫn còn trong Journal;
- khả năng kiểm tra lại vật có thể không còn;
- người khác có thể không tin lời player;
- Evidence State cập nhật về khả năng sử dụng bằng chứng.

Không nên xóa toàn bộ ký ức về vật phẩm chỉ vì nó không còn trong Inventory.

<a id="m23-t057-s155"></a>

### 102. Succession làm mất tri thức

Khi protagonist mới không biết một secret:

- Character Knowledge không tự nhận secret;
- in-world Map và Journal theo góc nhìn mới không tiết lộ;
- player archive có thể giữ dấu vết trải nghiệm trước;
- các hành động thực tế vẫn hợp lệ nếu player chủ động thử;
- những việc cần tri thức nhân vật vẫn kiểm tra đúng điều kiện.

Đây là một trong những test case quan trọng nhất của toàn bộ UI.

<a id="m23-t057-s156"></a>

### 103. Thông tin không đủ để dự đoán hậu quả

Nếu character không biết một khu vực có dịch bệnh, UI không được hiện cảnh báo dịch bệnh chi tiết chỉ vì engine biết.

Tuy nhiên, nếu có dấu hiệu đủ rõ như nước bẩn, người bệnh hoặc xác động vật, UI có thể đưa ra cảnh báo ở mức quan sát được.

**Không biết sự thật không đồng nghĩa UI được phép giấu những dấu hiệu mà nhân vật thực sự đã nhận thấy.**

<a id="m23-t057-s157"></a>

### 104. Không được để cơ chế trợ giúp tiết lộ bí mật

Những tính năng như:

- tìm kiếm;
- tự động gợi ý;
- danh sách chưa hoàn thành;
- số lượng collectible;
- filter “nhân vật phản bội”;
- icon quest chưa mở;

có thể làm lộ hidden truth.

Ví dụ không nên hiển thị:

> “Cồn Mây — 3/4 bí mật đã khám phá.”

nếu player không biết còn một bí mật thứ tư.

Search Index, Achievement Preview và Codex Completion cũng phải tuân Visibility Rules.

---

<a id="m23-t057-s158"></a>

## PHẦN XXVIII — QUALITY GATES

<a id="m23-t057-s159"></a>

### 105. Information Integrity Quality Gate

Mọi thông tin player-facing phải xác định được:

1. Ai biết?
2. Họ biết từ đâu?
3. Biết từ khi nào?
4. Đây là quan sát, lời kể hay suy luận?
5. Độ đáng tin có căn cứ thế nào?
6. Thông tin có thể đã cũ không?
7. Có mâu thuẫn với thông tin khác không?
8. Có được phép hiển thị cho character hiện tại không?

Nếu không trả lời được, thông tin chưa đủ điều kiện đưa lên giao diện.

<a id="m23-t057-s160"></a>

### 106. Screen Usability Quality Gate

Mỗi màn hình phải trả lời:

1. Player mở màn này để làm gì?
2. Thông tin quan trọng nhất là gì?
3. Player có thể thực hiện hành động nào?
4. Có cần mở màn khác để hoàn thành tác vụ không?
5. Có thể hiểu trạng thái trong thời gian ngắn không?
6. Có thể đọc khi tăng kích thước chữ không?
7. Có thể điều hướng không dùng chuột không?
8. Có thể quay lại nơi trước đó không?
9. Có xuất hiện thông tin không hợp lệ không?
10. Có cần giải thích một thuật ngữ ngay tại chỗ không?

<a id="m23-t057-s161"></a>

### 107. Journal & Investigation Quality Gate

Journal phải bảo đảm:

- player có thể tìm lại thông tin cũ;
- claim có source;
- contradiction không bị ghi đè;
- hypothesis không tự thành fact;
- thời gian thông tin được lưu;
- quest và Story Thread không đồng nhất;
- player có thể tự ghi note;
- người đã chết vẫn có lịch sử;
- nội dung không lộ bí mật qua search hoặc completion counter;
- dữ liệu nhất quán sau succession.

<a id="m23-t057-s162"></a>

### 108. Feedback Quality Gate

Một phản hồi gameplay quan trọng phải làm rõ:

- hành động vừa xảy ra;
- kết quả đã được xác lập;
- điều character có thể quan sát;
- điều kiện nào khiến outcome diễn ra nếu có thể hiểu;
- những hệ quả chưa rõ hoặc chưa đến thời điểm xuất hiện;
- nơi player có thể xem lại thông tin.

Không được thay giải thích cơ học bằng những đoạn văn mơ hồ chỉ để tạo cảm giác bí ẩn.

<a id="m23-t057-s163"></a>

### 109. Accessibility Quality Gate

Prototype cần kiểm tra ít nhất:

- font và UI scaling;
- tương phản;
- điều hướng bằng bàn phím;
- điều hướng bằng controller nếu nền tảng hỗ trợ;
- caption cho âm thanh quan trọng;
- xác nhận hành động không thể đảo ngược;
- khả năng xem lại câu chuyện;
- khả năng xem mục tiêu hiện tại;
- không phụ thuộc màu duy nhất;
- không có timer ẩn khi đọc menu;
- các thiết lập hỗ trợ có thể truy cập ngay từ lúc bắt đầu.

---

<a id="m23-t057-s164"></a>

## PHẦN XXIX — KẾ HOẠCH TRIỂN KHAI UI

<a id="m23-t057-s165"></a>

### 110. Prototype không nên thiết kế mọi màn hình cùng lúc

Tôi đề xuất bốn giai đoạn.

<a id="m23-t057-s166"></a>

#### Giai đoạn 1 — Information Kernel

Xây:

- Knowledge Record;
- Knowledge Projection;
- Entity Linking;
- Visibility Rules;
- Journal tối thiểu;
- Map Knowledge;
- Notification System.

Đây là nền bắt buộc.

<a id="m23-t057-s167"></a>

#### Giai đoạn 2 — Core Playable UI

Xây:

- HUD;
- Character;
- Party;
- Local/World Map;
- Travel Planner;
- Dialogue;
- Inventory;
- Combat UI.

Mục tiêu là một vertical slice có thể chơi và kiểm tra tính nhất quán.

<a id="m23-t057-s168"></a>

#### Giai đoạn 3 — Deep Simulation UI

Xây:

- Evidence Board;
- Faction;
- Social Graph;
- Crafting Workspace;
- Market;
- Settlement Projects;
- Codex;
- Calendar;
- Time Skip.

Mục tiêu là đưa các hệ thống ngoài combat thành gameplay thực sự.

<a id="m23-t057-s169"></a>

#### Giai đoạn 4 — Historical & Legacy UI

Xây:

- Life Chronicle;
- Location History;
- Succession;
- Legacy;
- Myth Evolution;
- Long-Term Event Recap.

Mục tiêu là chứng minh world simulation nhiều năm có khả năng được người chơi cảm nhận và hiểu.

---

<a id="m23-t057-s170"></a>

## PHẦN XXX — CÁC QUYẾT ĐỊNH ĐỀ XUẤT KHÓA

<a id="m23-t057-s171"></a>

### 111. Information Architecture

1. UI không được đọc hidden World Truth trực tiếp để trình bày.
2. Knowledge Projection Layer là lớp trung gian bắt buộc.
3. Mỗi Claim quan trọng phải có source và time.
4. Observation, Testimony, Inference và Hypothesis được phân biệt.
5. Confidence không mặc định là một phần trăm.
6. Thông tin cũ phải có Freshness.
7. Mọi màn hình cùng tham chiếu Entity ID ổn định.
8. Search và filter cũng phải tuân Knowledge Visibility.
9. Không dùng Completion Counter để làm lộ bí mật.
10. Player Notes không tự thay đổi World Truth.

<a id="m23-t057-s172"></a>

### 112. Navigation & Gameplay UI

11. Giao diện chính tổ chức theo nhu cầu người chơi, không theo tên subsystem.
12. Có ba cấp UI: HUD, Context Panel và Workspace.
13. HUD tối giản, thay đổi theo activity.
14. Character UI phân biệt Mastery và Execution Capability.
15. Class UI không hiển thị unlock percentage tùy tiện.
16. Companion UI không có Affection Bar toàn tri.
17. World Map phản ánh Character Knowledge.
18. Travel Planner trình bày rủi ro và thông tin theo nguồn.
19. Dialogue Option phải rõ intent.
20. Combat UI phải thể hiện objective, vị trí và hậu quả thực tế.

<a id="m23-t057-s173"></a>

### 113. Journal, Codex & Chronicle

21. Journal là trung tâm thông tin của game.
22. Quest, Evidence, Codex và Chronicle dùng chung nền dữ liệu liên kết.
23. Evidence Board không tự giải điều tra.
24. Codex tách kiến thức thế giới với giải thích ngoài gameplay.
25. Historical Content phải có phân loại căn cứ.
26. Chronicle tồn tại qua nhiều thế hệ.
27. Player Archive và Current Character Knowledge được phân biệt.
28. Succession không tự chuyển tri thức.
29. Lời hứa và nghĩa vụ có giao diện theo dõi riêng trong Journal.
30. Các sự kiện đã kết thúc vẫn có thể cập nhật hậu quả về sau.

<a id="m23-t057-s174"></a>

### 114. Feedback & Accessibility

31. Feedback có immediate, delayed và historical layers.
32. Reputation chỉ hiển thị khi player có nguồn biết.
33. Không spam notification chỉ số nhỏ.
34. Các thao tác khó đảo ngược có cơ chế xem lại và xác nhận.
35. Reading/Planning UI mặc định không làm World Time trôi.
36. Có khả năng xem lại các event quan trọng.
37. Text Scaling và Accessibility là yêu cầu nền tảng.
38. Không dùng màu làm tín hiệu duy nhất.
39. Hỗ trợ thông tin âm thanh quan trọng bằng caption.
40. LLM chỉ tạo presentation trên dữ liệu đã được xác thực.

---

<a id="m23-t057-s175"></a>

## PHẦN XXXI — DEFERRED TO PRODUCTION

<a id="m23-t057-s176"></a>

### 115. Những thông số và nội dung còn cần kiểm chứng

Các phần để lại cho production:

- cấu trúc màn hình theo độ phân giải;
- hệ icon;
- bảng màu;
- typography;
- font tiếng Việt;
- controller mappings;
- số tầng menu tối ưu;
- lượng notification phù hợp;
- độ rộng Evidence Board;
- tính năng search nâng cao;
- thuật toán Knowledge Confidence;
- kỹ thuật cache UI;
- cơ chế bảo vệ bí mật trong search index;
- bản thiết kế màn hình cho từng activity;
- UI animation;
- các accessibility presets;
- localization;
- danh sách test cases;
- thiết kế mỹ thuật chính thức.

Không nên khóa pixel-perfect layout trước khi có playable prototype.

---

<a id="m23-t057-s177"></a>

## PHẦN XXXII — CORE UX STATEMENT

<a id="m23-t057-s178"></a>

### 116. Người chơi không cần biết tất cả để đưa ra quyết định

Một RPG có thông tin hoàn hảo thường hỏi:

> “Tôi nên dùng lựa chọn nào để có kết quả tốt nhất?”

Game này nên khiến người chơi hỏi:

> “Tôi thật sự biết gì về tình huống này?”

> “Tôi đã nghe chuyện này từ ai?”

> “Tôi có đang tin nhầm một người không?”

> “Lần cuối tôi tới nơi đó là bao lâu rồi?”

> “Nếu tôi chấp nhận lời hứa này, điều gì sẽ xảy ra với cuộc sống của mình?”

Những câu hỏi ấy chỉ thú vị khi UI đủ rõ để người chơi có thể suy nghĩ, thay vì bị buộc phải đoán xem game đang giấu thông tin ở menu nào.

<a id="m23-t057-s179"></a>

### 117. Journal không phải danh sách nhiệm vụ

Một Journal tốt phải trở thành nơi người chơi nhìn lại hành trình của chính mình.

Ban đầu có thể chỉ có:

- quê nhà;
- gia đình;
- vài địa điểm quen;
- những người mình biết.

Sau nhiều năm:

- những tuyến đã đi;
- người từng đồng hành;
- các lời hứa;
- các cuộc tranh chấp;
- những mối quan hệ mất đi;
- kỹ thuật được học;
- những địa điểm đã thay đổi;
- những câu chuyện chưa tìm ra sự thật.

Và khi nhân vật qua đời, một người kế tục có thể biết rất ít về những gì người trước đã trải qua.

Nhưng người chơi ngoài đời vẫn có thể nhìn lại cuộc đời ấy.

<a id="m23-t057-s180"></a>

### 118. Chronicle không phải bảng thành tích

Chronicle phải trả lời:

> “Thế giới đã thay đổi như thế nào khi cuộc đời này đi qua?”

Không phải:

> “Bạn giết bao nhiêu kẻ địch?”

Một người có thể không giết ai.

Không trở thành thủ lĩnh.

Không sở hữu vật phẩm huyền thoại.

Nhưng vẫn để lại:

- một học trò;
- một cây cầu;
- một gia đình được cứu;
- một nghề được truyền lại;
- một lời thề được giữ;
- một câu chuyện người khác còn nhớ.

<a id="m23-t057-s181"></a>

### 119. Tuyên ngôn thiết kế cuối cùng

UI của game không nên khiến người chơi cảm thấy đang điều khiển một cơ sở dữ liệu phức tạp.

Nó phải khiến họ cảm thấy:

> **mình đang từng bước hiểu thêm về một thế giới rộng lớn hơn bản thân.**

Nhật ký không biết tất cả.

Bản đồ có thể sai.

Người quen có thể nhớ nhầm.

Một lời hứa có thể bị quên.

Một nhân vật có thể qua đời trước khi tìm được câu trả lời.

Và một câu chuyện được kể lại sau nhiều năm có thể rất khác những gì người chơi từng chứng kiến.

**Nhiệm vụ của UI không phải xóa bỏ sự không chắc chắn.**

**Nhiệm vụ của UI là giúp người chơi hiểu chính xác điều gì đang chắc chắn, điều gì chưa chắc chắn và vì sao.**

Đó là nguyên tắc phải được giữ từ màn hình đầu tiên cho tới Chronicle cuối cùng của campaign.

---

<a id="m23-t057-s182"></a>

### Đánh giá sau Module 23

Đến đây, phần trải nghiệm người chơi đã có một nền tương đối hoàn chỉnh. Các nguyên tắc quan trọng nhất cần bảo vệ khi đi vào phát triển là **nguồn gốc tri thức, tính nhất quán giữa màn hình, khả năng đọc hiểu, lịch sử nhiều thế hệ và sự phân biệt giữa dữ liệu nhân vật biết với dữ liệu mà engine biết**.

Bước tiếp theo tôi đề xuất là **Module 24 — AI Narrative Architecture + Content Generation Pipeline + World Simulation Integration + Validation & Testing**.

Đây là bước mang tính quyết định về khả năng triển khai. Chúng ta cần định nghĩa chính xác cách Rule Engine, World Simulation, Life Event Engine, World Director, Knowledge Graph, Quest Engine và LLM phối hợp; cách tạo hàng nghìn NPC và sự kiện mà không mâu thuẫn; cách quản lý dữ liệu nội dung, kiểm thử nhân quả và ngăn AI tự bịa sự thật.

Sau module đó, có thể tiến hành **GDD Architecture Review**, xác định các hệ thống quá phức tạp, chưa nhất quán hoặc cần giảm phạm vi, trước khi đóng gói bản GDD chính thức dành cho đội phát triển.

<!-- SOURCE T057 END -->

