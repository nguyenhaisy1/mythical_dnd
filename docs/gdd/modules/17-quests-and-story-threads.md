# Module 17 — Quest, Story Thread và can thiệp của người chơi

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T041](../../source/conversation.md#t041)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Event tồn tại độc lập Quest. Story Thread phản ánh facts; Quest là góc nhìn/cam kết có knowledge filter. Không thưởng bằng tài sản không có nguồn.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Committed events, player knowledge, promises, actors và intervention opportunities. |
| Đầu ra / bên sở hữu | Thread state, objective projections, scheduled consequences; Narrative sở hữu thread. |
| Đọc cùng | 06, 16, 22–27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Không nhận quest thì world vẫn vận hành; đối tượng chết/mất làm objective thay đổi hợp lệ. |
| Việc chưa được chứng minh | Thread catalog và coverage của các nhánh production. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T041 BEGIN -->

<a id="m17-t041-s001"></a>

## GDD MODULE 17 — QUEST, STORY THREAD & PLAYER INTERVENTION SYSTEM

<a id="m17-t041-s002"></a>

## 1. DESIGN PHILOSOPHY

Quest trong game này không phải:

> NPC có dấu `!` → player nhận nhiệm vụ → objective xuất hiện → hoàn thành → nhận thưởng.

Quest được định nghĩa là:

> **Một cửa sổ can thiệp mà qua đó player nhận biết và có khả năng tác động vào một Story Thread đang tồn tại trong thế giới.**

Story Thread tồn tại độc lập với player.

Quest chỉ tồn tại khi player:

- biết thread;
- có đủ thông tin để hiểu rằng mình có thể can thiệp;
- hoặc một actor chủ động yêu cầu player can thiệp.

Do đó:

**Story Thread ≠ Quest.**

---

<a id="m17-t041-s003"></a>

## 2. STORY THREAD VÀ QUEST

Ví dụ:

Một đứa trẻ mất tích.

Story Thread bắt đầu từ lúc:

trẻ rời nhà.

Player chưa biết.

Thread vẫn chạy:

gia đình tìm kiếm.

trời mưa.

dấu vết mất dần.

người trong làng lan tin.

Một hunter tìm thấy đồ của đứa trẻ.

Chỉ khi player:

nghe rumor;

gặp gia đình;

thấy dấu vết;

hoặc companion biết chuyện,

Quest mới được **exposed**.

---

<a id="m17-t041-s004"></a>

## 3. QUEST KHÔNG TẠO WORLD STATE

Không được:

player nhận quest

→ game mới spawn đứa trẻ mất tích.

Ngược lại:

đứa trẻ mất trước.

World Simulation chạy.

Quest UI chỉ phản ánh:

> player đã biết gì về sự kiện đó.

---

<a id="m17-t041-s005"></a>

## 4. QUEST DATA MODEL

Mỗi Quest Instance có:

```text
Quest ID
Parent Story Thread
Discovery Source
Player Knowledge
Known Actors
Unknown Actors
Current Goals
Optional Goals
Known Locations
Unknown Locations
Constraints
Urgency
Persistence
Known Risks
Known Rewards
Promises
Debts
Faction Links
Companion Links
Historical Links
Current State
Resolution State
Aftermath
```

---

<a id="m17-t041-s006"></a>

## 5. QUEST KHÔNG CẦN CÓ “QUEST GIVER”

Một Quest có thể bắt đầu từ:

- NPC request;
- rumor;
- item;
- corpse;
- environment clue;
- companion;
- profession;
- faction;
- personal memory;
- map discovery;
- accident;
- world event.

---

<a id="m17-t041-s007"></a>

## 6. QUEST DISCOVERY SOURCES

Quest exposure có 10 nhóm chính.

<a id="m17-t041-s008"></a>

### Direct Request

NPC chủ động nhờ.

<a id="m17-t041-s009"></a>

### Rumor

Player nghe chuyện.

<a id="m17-t041-s010"></a>

### Observation

Player tự phát hiện tình huống.

<a id="m17-t041-s011"></a>

### Relationship

Người quen chia sẻ.

<a id="m17-t041-s012"></a>

### Profession

Công việc expose vấn đề.

<a id="m17-t041-s013"></a>

### Class

Capability cho phép nhìn thấy thread.

<a id="m17-t041-s014"></a>

### Item

Một vật mở ra câu hỏi.

<a id="m17-t041-s015"></a>

### Location

Khám phá nơi bất thường.

<a id="m17-t041-s016"></a>

### Faction

Faction giao hoặc tiết lộ.

<a id="m17-t041-s017"></a>

### Historical Event

Biến cố lớn tạo nhiều cửa can thiệp.

---

<a id="m17-t041-s018"></a>

## 7. SAME THREAD — DIFFERENT QUEST ENTRY

Ví dụ một vụ giết người.

<a id="m17-t041-s019"></a>

#### Entry A

Gia đình nạn nhân:

> “Tìm hung thủ.”

<a id="m17-t041-s020"></a>

#### Entry B

Người bị nghi:

> “Chứng minh ta vô tội.”

<a id="m17-t041-s021"></a>

#### Entry C

Player tìm thấy thi thể.

<a id="m17-t041-s022"></a>

#### Entry D

Faction muốn che chuyện.

<a id="m17-t041-s023"></a>

#### Entry E

Companion biết nạn nhân.

Cùng một Story Thread.

Quest framing khác nhau.

---

<a id="m17-t041-s024"></a>

## 8. QUEST FRAMING KHÔNG ĐỊNH NGHĨA SỰ THẬT

Nếu Quest bắt đầu:

> “Giết con quái vật ở hồ.”

Không có nghĩa:

game xác nhận có quái vật.

Đó chỉ là cách NPC hiểu vấn đề.

Quest Journal phải phân biệt:

**What Player Knows**

và

**What Someone Claims.**

---

<a id="m17-t041-s025"></a>

## 9. QUEST TITLE

Quest title không nên spoil truth.

Ví dụ tốt:

> **Tiếng gọi dưới nước**

Không:

> **Giết con thủy quỷ giả do tên trưởng làng dựng lên**

---

<a id="m17-t041-s026"></a>

## 10. OBJECTIVE PHILOSOPHY

Objective không phải checklist tuyệt đối.

Objective là:

> mục tiêu hiện tại mà character tin rằng có ích để tiến gần tới vấn đề.

Objective có thể:

- thay đổi;
- biến mất;
- bị chứng minh là sai;
- được thay bằng objective mới.

---

<a id="m17-t041-s027"></a>

## 11. OBJECTIVE TYPES

Core objective types:

<a id="m17-t041-s028"></a>

#### Find

Tìm người/vật/nơi/thông tin.

<a id="m17-t041-s029"></a>

#### Reach

Đến địa điểm.

<a id="m17-t041-s030"></a>

#### Investigate

Giảm uncertainty.

<a id="m17-t041-s031"></a>

#### Convince

Thay đổi quyết định.

<a id="m17-t041-s032"></a>

#### Protect

Giữ actor/object/location an toàn.

<a id="m17-t041-s033"></a>

#### Rescue

Đưa actor khỏi danger.

<a id="m17-t041-s034"></a>

#### Deliver

Đưa vật/người/thông tin.

<a id="m17-t041-s035"></a>

#### Obtain

Có được resource/permission/item.

<a id="m17-t041-s036"></a>

#### Prevent

Ngăn event.

<a id="m17-t041-s037"></a>

#### Survive

Sống qua event.

<a id="m17-t041-s038"></a>

#### Build/Repair

Thay đổi physical world.

<a id="m17-t041-s039"></a>

#### Decide

Đưa ra lựa chọn.

---

<a id="m17-t041-s040"></a>

## 12. OBJECTIVE CÓ THỂ KHÔNG BIẾT CÁCH THỰC HIỆN

Ví dụ:

> **Tìm Vạc Lâm.**

Không marker.

Player biết:

lần cuối thấy ở Bến Vạc.

Từ đó tự điều tra.

Quest không nói:

> Follow footprints 1/3.

---

<a id="m17-t041-s041"></a>

## 13. KNOWN LEADS

Thay checklist objective vi mô bằng:

**Known Leads.**

Ví dụ:

- Lâm cãi nhau với cha trước khi mất tích.
- Một fisherman thấy người giống Lâm đi về phía hạ lưu.
- Một chiếc mái chèo bị bỏ gần Cồn Mây.

Player chọn lead nào theo.

---

<a id="m17-t041-s042"></a>

## 14. LEAD CAN BE FALSE

Một witness có thể nhầm.

Một rumor có thể sai.

Quest UI không đánh dấu:

> Fake Clue.

Chỉ ghi source.

---

<a id="m17-t041-s043"></a>

## 15. LEAD CONFIDENCE

Character có thể đánh giá:

Weak.

Plausible.

Strong.

Confirmed.

Tùy skill/knowledge.

---

<a id="m17-t041-s044"></a>

## 16. QUEST KNOWLEDGE GRAPH

Quest không lưu một chuỗi tuyến tính.

Nó lưu:

```text
Known Facts
Claims
Suspicions
Contradictions
Unknowns
Relationships
Locations
Objects
```

Player có thể giải theo nhiều thứ tự.

---

<a id="m17-t041-s045"></a>

## 17. QUEST STATE

Quest có thể ở:

<a id="m17-t041-s046"></a>

#### Hidden

Thread tồn tại nhưng player không biết.

<a id="m17-t041-s047"></a>

#### Rumored

Player chỉ nghe tin.

<a id="m17-t041-s048"></a>

#### Known

Player hiểu có vấn đề.

<a id="m17-t041-s049"></a>

#### Investigating

Đang chủ động can thiệp.

<a id="m17-t041-s050"></a>

#### Escalating

Thread đang xấu đi.

<a id="m17-t041-s051"></a>

#### Dormant

Chưa có hành động mới nhưng chưa kết thúc.

<a id="m17-t041-s052"></a>

#### Resolved

Core tension kết thúc.

<a id="m17-t041-s053"></a>

#### Failed

Một desired outcome không còn khả thi.

<a id="m17-t041-s054"></a>

#### Transformed

Thread trở thành vấn đề khác.

<a id="m17-t041-s055"></a>

#### Historical

Đã trở thành event quá khứ.

---

<a id="m17-t041-s056"></a>

## 18. QUEST KHÔNG TỰ ĐỘNG FAIL KHI PLAYER KHÔNG NHẬN

Thread tiếp tục.

Có thể sau này:

player nghe kết quả.

gặp hậu quả.

thấy mộ.

gặp người sống sót.

Quest trở thành Historical Thread.

---

<a id="m17-t041-s057"></a>

## 19. QUEST URGENCY

Không phải quest nào cũng có deadline.

Có bốn cấp:

<a id="m17-t041-s058"></a>

#### No Urgency

Có thể chờ lâu.

<a id="m17-t041-s059"></a>

#### Soft Urgency

Delay làm conditions đổi.

<a id="m17-t041-s060"></a>

#### Active Urgency

Có event deadline thực.

<a id="m17-t041-s061"></a>

#### Critical Urgency

Hours/days matter.

---

<a id="m17-t041-s062"></a>

## 20. DEADLINE LÀ WORLD EVENT

Không dùng timer chỉ vì game design.

Deadline phải có nguyên nhân.

Ví dụ:

lũ đến.

đoàn người rời.

người bệnh xấu đi.

trial diễn ra.

quân tới.

---

<a id="m17-t041-s063"></a>

## 21. NO FAKE DEADLINE

Nếu quest nói:

> “Hãy cứu hắn ngay!”

thì player đi 2 năm quay lại không được thấy hắn vẫn đứng nguyên.

Nếu design không muốn timer:

dialogue không được giả urgency.

---

<a id="m17-t041-s064"></a>

## 22. DEADLINE INFORMATION

Player không luôn biết deadline chính xác.

Có thể:

> “Ông ấy khó sống qua vài ngày.”

Dược Nhân giỏi:

ước lượng tốt hơn.

---

<a id="m17-t041-s065"></a>

## 23. QUEST PROGRESSION KHÔNG DỰA SỐ OBJECTIVE HOÀN THÀNH

Progress = thay đổi:

- knowledge;
- access;
- relationship;
- world state;
- leverage.

Một conversation có thể thay quest nhiều hơn 3 combat encounter.

---

<a id="m17-t041-s066"></a>

## 24. MULTIPLE SOLUTION PRINCIPLE

Major Quest phải hỗ trợ tối thiểu ba loại approach khi context cho phép:

<a id="m17-t041-s067"></a>

#### Force

Combat/coercion.

<a id="m17-t041-s068"></a>

#### Social

Negotiation/relationship.

<a id="m17-t041-s069"></a>

#### Knowledge

Investigation/information.

Ngoài ra có thể:

- Craft;
- Stealth;
- Economic;
- Political;
- Ritual;
- Environmental.

---

<a id="m17-t041-s070"></a>

## 25. KHÔNG ÉP MỌI QUEST CÓ TẤT CẢ APPROACH

Một người đang chết đuối không cần:

Political Solution.

Nhưng toàn game phải phân bố giải pháp đa dạng.

---

<a id="m17-t041-s071"></a>

## 26. SOLUTION LÀ CONSEQUENCE, KHÔNG PHẢI BUTTON

Ví dụ family debt.

Không có menu:

> [Economic Solution]

Player thực sự phải:

kiếm resource;

thương lượng;

tạo income;

chứng minh debt invalid;

hoặc tìm arrangement khác.

---

<a id="m17-t041-s072"></a>

## 27. QUEST BRANCHING

Quest branching không nên viết như cây khổng lồ:

A → B1/B2/B3 → C1/C2/C3...

Thay vào đó:

Quest đọc World State.

Story Thread phản ứng.

Điều này giảm exponential authored content.

---

<a id="m17-t041-s073"></a>

## 28. STATE-BASED QUEST DESIGN

Ví dụ:

Target Alive?

Target Found?

Faction Hostile?

Item Exists?

Player Knows Secret?

Promise Broken?

Quest dialogue dựa trên state.

Không cần viết mọi combination thành riêng branch tree.

---

<a id="m17-t041-s074"></a>

## 29. QUEST MERGE

Hai quest có thể được nhận riêng nhưng sau đó player phát hiện cùng source.

Ví dụ:

**Người mất tích**

và

**Đồng bị đánh cắp**

đều liên quan một smuggling network.

Hai Quest UI có thể merge thành một Story Thread lớn.

---

<a id="m17-t041-s075"></a>

## 30. QUEST COLLISION

Hai quest có thể xung đột.

Ví dụ:

Quest A:

đưa prisoner về Phong Châu.

Quest B:

giúp family prisoner trốn.

Không thể hoàn thành cả hai như thiết kế checkbox.

Player phải chọn hoặc tìm solution khác.

---

<a id="m17-t041-s076"></a>

## 31. QUEST INTERFERENCE

Player không chủ động chọn quest B nhưng hành động trong quest A có thể phá B.

World thống nhất.

---

<a id="m17-t041-s077"></a>

## 32. PERSONAL QUEST

Không gọi đơn giản là “Companion Quest”.

Companion có Personal Story Threads.

Quest chỉ xuất hiện khi player đủ access.

Ví dụ:

companion muốn tìm cha.

Hoặc player tự phát hiện secret mà companion chưa muốn giải quyết.

---

<a id="m17-t041-s078"></a>

## 33. COMPANION QUEST OWNERSHIP

Đây là câu chuyện của companion.

Player không mặc định quyết tất cả.

Companion có thể:

- từ chối;
- tự lựa chọn;
- thay đổi objective.

---

<a id="m17-t041-s079"></a>

## 34. COMPANION CAN RESOLVE WITHOUT PLAYER

Nếu player không can thiệp nhưng companion có khả năng:

họ có thể tự xử lý.

Outcome phụ thuộc simulation.

---

<a id="m17-t041-s080"></a>

## 35. FAMILY QUEST

Family System tự tạo nhiều thread:

- inheritance;
- marriage;
- debt;
- missing person;
- illness;
- feud;
- migration.

Không cần NPC phát quest formal.

---

<a id="m17-t041-s081"></a>

## 36. PROFESSION QUEST

Sinh từ Career System.

Ví dụ thợ đúc:

- material shortage;
- difficult commission;
- apprentice problem;
- workshop accident;
- trade secret theft.

Quest làm nghề trở thành narrative gameplay.

---

<a id="m17-t041-s082"></a>

## 37. FACTION QUEST

Không dùng bảng:

> Faction Quest #12.

Faction chỉ yêu cầu player khi:

có Goal

+

Pressure

+

Reason to use player.

---

<a id="m17-t041-s083"></a>

## 38. FACTION REQUEST CAN BE MORALLY NEUTRAL OR UNCOMFORTABLE

Faction player thích có thể yêu cầu:

trưng thu grain.

do chiến tranh.

Player phải cân nhắc.

Không chia quest theo:

Good Faction / Evil Faction.

---

<a id="m17-t041-s084"></a>

## 39. SETTLEMENT QUEST

Sinh từ Settlement State.

Ví dụ:

Food thấp.

Water conflict.

Disease.

Bandit pressure.

Leadership dispute.

Không random “collect 5 herbs”.

---

<a id="m17-t041-s085"></a>

## 40. RESOURCE REQUEST

Nếu healer cần herbs:

Quest chỉ tạo nếu:

herb shortage thật.

Player có thể giải bằng:

- gather;
- buy;
- trade;
- substitute;
- find another healer.

Không nhất thiết fetch quest.

---

<a id="m17-t041-s086"></a>

## 41. EXPLORATION QUEST

Sinh từ:

rumor.

map.

landmark.

missing route.

Examples:

- tìm đường cũ;
- xác minh shrine;
- khám phá source của smoke.

---

<a id="m17-t041-s087"></a>

## 42. DUNGEON QUEST

Quest đưa player vào dungeon nhưng dungeon không bị quest sở hữu.

Player có thể đi vào:

trước quest.

sau quest.

vì lý do khác.

---

<a id="m17-t041-s088"></a>

## 43. INVESTIGATION QUEST

Core progression là:

uncertainty ↓.

Không phải location chain cố định.

Có thể giải bằng:

- witness;
- material evidence;
- social network;
- professional analysis;
- confession.

---

<a id="m17-t041-s089"></a>

## 44. INVESTIGATION WRONG CONCLUSION

Player có thể kết luận sai.

Game không buộc correct truth trước khi cho action.

Player có thể tố sai người.

World phản ứng.

---

<a id="m17-t041-s090"></a>

## 45. ACCUSATION

Accusation là high-stakes social action.

Requires:

target.

claim.

audience.

evidence.

authority context.

Nếu sai:

reputation/family/faction consequence.

---

<a id="m17-t041-s091"></a>

## 46. CASE CAN REMAIN UNSOLVED

Không phải mọi mystery bắt buộc có closure.

Player có thể:

thiếu evidence.

target chết.

time destroys clue.

Sau nhiều năm:

new clue xuất hiện.

---

<a id="m17-t041-s092"></a>

## 47. RESCUE QUEST

Không đơn giản:

reach NPC → escort back.

Rescue có thể yêu cầu:

- locate;
- stabilize;
- create route;
- negotiate captor;
- carry;
- deal with injury.

---

<a id="m17-t041-s093"></a>

## 48. ESCORT QUEST

Không dùng mô hình NPC đi theo chậm và bị attack random.

Escort challenge đến từ:

route;

health;

politics;

supplies;

target personality.

---

<a id="m17-t041-s094"></a>

## 49. TARGET MAY NOT WANT RESCUE

Một người mất tích có thể:

tự bỏ đi.

không muốn trở về.

Player phải quyết:

ép?

nói?

nói dối family?

Đây mới là narrative quest.

---

<a id="m17-t041-s095"></a>

## 50. DELIVERY QUEST

Không phải chỉ chạy A→B.

Delivery object có:

owner.

importance.

time.

security.

knowledge.

Player có thể:

mở?

bán?

thay?

mất?

---

<a id="m17-t041-s096"></a>

## 51. MESSAGE QUEST

Message có exact content.

Player có thể:

- deliver unchanged;
- alter;
- withhold;
- reveal to third party.

Knowledge consequences rất mạnh.

---

<a id="m17-t041-s097"></a>

## 52. PROTECTION QUEST

Không nhất thiết combat.

Protect có thể gồm:

- secrecy;
- route;
- health;
- reputation;
- political protection.

---

<a id="m17-t041-s098"></a>

## 53. BUILD/REPAIR QUEST

Sinh từ World State.

Ví dụ:

bridge destroyed.

Solution cần:

Design.

Material.

Labor.

Authority.

Time.

Xảo Nhân không tự xây cả cầu trong một action.

---

<a id="m17-t041-s099"></a>

## 54. DIPLOMATIC QUEST

Không phải:

Talk to A → Talk to B → success.

Phải dùng Negotiation System.

Actors có:

goals.

red lines.

leverage.

Player có thể thất bại mà tạo state mới.

---

<a id="m17-t041-s100"></a>

## 55. WAR QUEST

Không phải kill-count.

Examples:

- secure supply route;
- scout army;
- convince settlement;
- evacuate civilian;
- capture bridge;
- recover wounded;
- negotiate surrender.

---

<a id="m17-t041-s101"></a>

## 56. LARGE BATTLE QUEST

Player nhận Tactical Objective.

Battle Layer chạy song song.

Nếu player fail:

battle không nhất thiết auto lost.

Outcome bị ảnh hưởng theo trọng số.

---

<a id="m17-t041-s102"></a>

## 57. RITUAL QUEST

Không đơn giản collect 3 ritual items.

Ritual có:

purpose.

participants.

sequence.

social meaning.

Player có thể:

- support;
- investigate;
- alter;
- oppose.

---

<a id="m17-t041-s103"></a>

## 58. MẠCH QUEST

Phải giữ ambiguity.

Objective ban đầu có thể:

> tìm nguyên nhân giấc mơ chung.

Outcome có thể:

natural;

social;

spiritual;

unresolved.

---

<a id="m17-t041-s104"></a>

## 59. MORAL QUEST

Không gắn màu.

Một decision có:

Stakeholders.

Values.

Short-term consequence.

Long-term consequence.

Không có reward cao hơn cho “good choice”.

---

<a id="m17-t041-s105"></a>

## 60. ECONOMIC QUEST

Examples:

- trade route;
- price shock;
- debt;
- supply shortage;
- craft contract.

Economic solution phải thay simulation.

---

<a id="m17-t041-s106"></a>

## 61. CRIME QUEST

Player có thể:

investigate;

participate;

cover up;

profit.

Crime thread không mặc định player = police.

---

<a id="m17-t041-s107"></a>

## 62. REVENGE QUEST

Không nên tự gắn goal:

> kill murderer.

Character có thể muốn:

truth;

justice;

compensation;

revenge;

forgiveness.

Player chooses involvement.

---

<a id="m17-t041-s108"></a>

## 63. MAIN STORY QUEST

Main Story sử dụng cùng framework.

Không được có “main story rules” hoàn toàn khác.

Historical Anchors tạo:

large Story Threads.

Player thấy một phần tùy:

location.

relationships.

faction.

profession.

---

<a id="m17-t041-s109"></a>

## 64. HISTORICAL ANCHOR

Anchor định nghĩa:

<a id="m17-t041-s110"></a>

#### Must Happen

Ví dụ Cổ Loa được hình thành ở mức lịch sử campaign cần.

<a id="m17-t041-s111"></a>

#### Flexible Outcomes

Ai tham gia.

ai mất đất.

công nghệ phát triển ra sao.

faction nào ủng hộ.

player có vai trò gì.

---

<a id="m17-t041-s112"></a>

## 65. MAIN QUEST KHÔNG BẮT PLAYER LÀ CHOSEN ONE

Player có thể tham gia historical thread với vai trò:

- smith;
- messenger;
- soldier;
- healer;
- negotiator;
- local farmer;
- investigator.

---

<a id="m17-t041-s113"></a>

## 66. MAIN STORY EXPOSURE

Không bắt đầu bằng:

> “Cứu cả Văn Lang.”

Player bắt đầu local.

Local thread dần nối regional/political.

---

<a id="m17-t041-s114"></a>

## 67. QUEST SCALE

Có sáu scale.

<a id="m17-t041-s115"></a>

#### Personal

Một người.

<a id="m17-t041-s116"></a>

#### Household

Một gia đình.

<a id="m17-t041-s117"></a>

#### Settlement

Một cộng đồng.

<a id="m17-t041-s118"></a>

#### Regional

Nhiều settlement.

<a id="m17-t041-s119"></a>

#### Faction

Political/economic network.

<a id="m17-t041-s120"></a>

#### Historical

Thay đổi thời đại.

---

<a id="m17-t041-s121"></a>

## 68. QUEST SCALE CAN GROW

Một missing person:

→ smuggling network

→ political weapon trade

→ regional faction conflict.

Không cần biết trước từ lúc quest bắt đầu.

---

<a id="m17-t041-s122"></a>

## 69. QUEST SCALE CAN SHRINK

Historical war vẫn tạo personal quest:

tìm em trai.

chôn cha.

cứu workshop.

Macro event không xóa micro story.

---

<a id="m17-t041-s123"></a>

## 70. QUEST ACTORS

Mỗi major quest phải xác định:

Requester nếu có.

Primary Stakeholder.

Secondary Stakeholder.

Opponent.

Neutral Actor.

Affected Population.

Không nhất thiết tất cả xuất hiện trực tiếp.

---

<a id="m17-t041-s124"></a>

## 71. OPPONENT KHÔNG LUÔN LÀ VILLAIN

Ví dụ:

player muốn mở đê.

Opponent muốn giữ vì downstream risk.

Cả hai có lý.

---

<a id="m17-t041-s125"></a>

## 72. ENVIRONMENT CAN BE OPPOSITION

Flood.

Disease.

Time.

Terrain.

Không phải quest nào cần human antagonist.

---

<a id="m17-t041-s126"></a>

## 73. QUEST CONSTRAINTS

Possible:

Time.

Resource.

Secrecy.

Law.

Relationship.

Location.

Weather.

Moral boundary.

Political authority.

---

<a id="m17-t041-s127"></a>

## 74. CONSTRAINT CREATES GAMEPLAY

Ví dụ:

Need rescue prisoner.

Constraint:

không được làm faction biết.

Stealth/social solution trở nên quan trọng.

---

<a id="m17-t041-s128"></a>

## 75. QUEST RESOURCE COST

Can consume:

Time.

Food.

Money/resource.

Reputation.

Favor.

Promise.

Equipment.

Health.

Relationship.

Không phải mọi cost material.

---

<a id="m17-t041-s129"></a>

## 76. REWARD PHILOSOPHY

Quest reward không chủ yếu là XP/gold.

Reward có nhiều loại:

- Material;
- Knowledge;
- Relationship;
- Reputation;
- Access;
- Faction Favor;
- Career Opportunity;
- Technique;
- Route;
- Property;
- Political Influence;
- Historical Impact.

---

<a id="m17-t041-s130"></a>

## 77. REQUESTER DOES NOT ALWAYS PAY

Một bà mẹ nghèo không phát:

500 đồng + sword.

Reward có thể là:

gratitude.

family relationship.

information.

future shelter.

---

<a id="m17-t041-s131"></a>

## 78. PLAYER CAN ASK FOR PAYMENT

Negotiation trước quest.

Nếu player là professional:

điều này hợp lý.

Có thể:

salary;

share;

favor;

training.

---

<a id="m17-t041-s132"></a>

## 79. QUEST CONTRACT

Một số quest có explicit agreement.

Fields:

Task.

Expected Result.

Payment.

Deadline.

Conditions.

Risk.

Witness.

Nếu player làm khác:

requester đánh giá.

---

<a id="m17-t041-s133"></a>

## 80. UNSOLICITED INTERVENTION

Player có thể tự can thiệp không ai nhờ.

Sau đó:

NPC có thể biết ơn.

hoặc tức giận.

Ví dụ player “giải cứu” một người không muốn được cứu.

---

<a id="m17-t041-s134"></a>

## 81. QUEST SUCCESS IS PERSPECTIVE-DEPENDENT

Objective:

mở kênh nước.

Player thành công.

Làng A:

Success.

Làng B:

Disaster.

Game không nhất thiết hiện:

**QUEST SUCCESS!**

---

<a id="m17-t041-s135"></a>

## 82. SYSTEM RESOLUTION

Quest result nên diễn đạt:

> “Đồng Mạ đã nhận được nước. Nậu Khôn tuyên bố thỏa thuận bất hợp lệ.”

Natural state summary tốt hơn binary rating.

---

<a id="m17-t041-s136"></a>

## 83. PLAYER-DEFINED SUCCESS

Một số quest player tự quyết mục tiêu.

Ví dụ:

family asks kill thief.

Player tìm thief và tha.

Requester coi fail.

Player có thể coi resolved.

Thread state ghi cả hai.

---

<a id="m17-t041-s137"></a>

## 84. PARTIAL SUCCESS

Ví dụ:

cứu 4/7 người.

Không dùng:

Objective 4/7 completed

là toàn bộ ý nghĩa.

Game lưu:

ai sống.

ai chết.

family reactions.

---

<a id="m17-t041-s138"></a>

## 85. PYRRHIC SUCCESS

Player đạt goal nhưng:

companion chết.

village burns.

promise broken.

Không gọi “Perfect Victory”.

---

<a id="m17-t041-s139"></a>

## 86. FAILURE WITH DISCOVERY

Player không cứu được target.

Nhưng tìm ra truth.

New thread:

funeral.

justice.

family consequence.

---

<a id="m17-t041-s140"></a>

## 87. FAILURE WITH ESCALATION

Bandit leader escapes.

Thread becomes:

pursuit.

retaliation.

Không reset encounter.

---

<a id="m17-t041-s141"></a>

## 88. FAILURE BY DELAY

Player đến muộn.

Không chỉ:

Quest Failed.

World now has:

death.

migration.

new leader.

New intervention opportunities.

---

<a id="m17-t041-s142"></a>

## 89. ABANDON QUEST

Player có thể chủ động ngừng.

Quest không biến mất.

Thread tiếp tục.

Journal chuyển:

> Không còn theo đuổi.

---

<a id="m17-t041-s143"></a>

## 90. RETURN TO OLD THREAD

Nếu còn relevant:

player có thể quay lại.

Không cần “re-accept”.

---

<a id="m17-t041-s144"></a>

## 91. QUEST MEMORY

NPC nhớ:

player promised.

player failed.

player ignored.

Không chỉ completed quest count.

---

<a id="m17-t041-s145"></a>

## 92. HIDDEN CONSEQUENCE

Không phải mọi consequence hiện ngay.

Ví dụ:

player tha smuggler.

3 năm sau:

người đó thành trader hoặc criminal leader.

---

<a id="m17-t041-s146"></a>

## 93. DELAYED REWARD

Giúp apprentice hôm nay.

10 năm sau:

họ là Master và giúp lại.

Không quest chest ngay.

---

<a id="m17-t041-s147"></a>

## 94. DELAYED PUNISHMENT

Player phá bridge để escape.

Năm sau:

trade route suy giảm.

Food price tăng.

World remembers.

---

<a id="m17-t041-s148"></a>

## 95. QUEST CHAIN

Không pre-author toàn bộ chain nhất thiết.

Thread generates follow-up based on unresolved states.

Ví dụ:

Debt

→ Default

→ Family Conflict

→ Migration

→ Career Shift.

---

<a id="m17-t041-s149"></a>

## 96. AUTHORED QUEST CHAIN

Main story/major companion arcs vẫn cần authored structure.

Nhưng nodes phải state-aware.

---

<a id="m17-t041-s150"></a>

## 97. PROCEDURAL QUEST

Minor procedural quest chỉ được tạo từ real pressure.

Không random:

> Gather 8 flowers.

Nếu healer needs 8 units medicine:

system có economic reason.

---

<a id="m17-t041-s151"></a>

## 98. PROCEDURAL QUEST TEMPLATE

Template includes:

Need.

Actor.

Cause.

Resource/Action.

Constraints.

Potential Solutions.

Outcome.

Follow-up.

---

<a id="m17-t041-s152"></a>

## 99. PROCEDURAL QUEST VALIDATION

Generator phải kiểm:

- actor tồn tại;
- need thật;
- target/resource tồn tại;
- route hợp lệ;
- reward actor có thể cung cấp;
- không conflict với world state.

---

<a id="m17-t041-s153"></a>

## 100. NO DUPLICATE QUEST SPAM

Novelty control.

Nếu settlement vừa có:

missing child thread,

không spawn thêm ba missing children vô lý trừ larger cause.

---

<a id="m17-t041-s154"></a>

## 101. QUEST HEAT

Story Director theo dõi số thread player đang active.

Không expose 40 urgent quests cùng lúc.

---

<a id="m17-t041-s155"></a>

## 102. QUEST EXPOSURE BUDGET

Không phải mọi vấn đề thế giới đều nói với player.

Ưu tiên expose theo:

- proximity;
- relationship;
- relevance;
- capability;
- narrative heat.

---

<a id="m17-t041-s156"></a>

## 103. FOMO CONTROL

World vẫn sống nhưng game không được biến thành:

> nếu không kiểm mọi làng mỗi ngày sẽ bỏ lỡ content.

Dùng:

Persistence.

Rumor.

Late-stage intervention.

Consequences.

---

<a id="m17-t041-s157"></a>

## 104. PERSISTENT THREAD

Một số thread kéo dài nhiều năm.

Ví dụ:

family feud.

political rivalry.

craft tradition.

Không cần player theo liên tục.

---

<a id="m17-t041-s158"></a>

## 105. TRANSIENT THREAD

Một số event ngắn:

storm rescue.

temporary camp.

Nếu bỏ lỡ:

xảy ra offscreen.

Đó là okay.

---

<a id="m17-t041-s159"></a>

## 106. QUEST JOURNAL PHILOSOPHY

Journal không phải danh sách checklist.

Nó là:

> **sổ ghi những vấn đề, con người, lời hứa và điều chưa hiểu mà character đang theo dõi.**

---

<a id="m17-t041-s160"></a>

## 107. JOURNAL SECTIONS

Core:

<a id="m17-t041-s161"></a>

#### Current Threads

Vấn đề đang quan tâm.

<a id="m17-t041-s162"></a>

#### Leads

Điều có thể kiểm tra.

<a id="m17-t041-s163"></a>

#### People

Actors liên quan.

<a id="m17-t041-s164"></a>

#### Places

Locations liên quan.

<a id="m17-t041-s165"></a>

#### Promises & Debts

Cam kết.

<a id="m17-t041-s166"></a>

#### Rumors

Thông tin chưa xác nhận.

<a id="m17-t041-s167"></a>

#### Resolved Histories

Thread đã kết thúc.

---

<a id="m17-t041-s168"></a>

## 108. JOURNAL NATURAL LANGUAGE

Ví dụ:

**Vạc Lâm mất tích**

> Lâm rời Bến Vạc ba ngày trước sau khi cãi nhau với cha. Một fisherman cho rằng đã thấy cậu đi xuống hạ lưu, nhưng trời tối và ông không chắc.

**Các manh mối**

> Một mái chèo của nhà Tòng được tìm thấy gần Cồn Mây.

Không:

`Go to marker X`.

---

<a id="m17-t041-s169"></a>

## 109. JOURNAL SOURCE

Có thể click claim:

> Nguồn: Hạc Dư, fisherman, Bến Vạc.

Cho investigation rõ ràng.

---

<a id="m17-t041-s170"></a>

## 110. CONTRADICTION DISPLAY

Nếu player có capability:

Journal có thể highlight:

> Lời của Hạc Dư mâu thuẫn với thời điểm Tòng Mộc nói đã thấy Lâm ở nhà.

Không tự kết luận ai nói dối.

---

<a id="m17-t041-s171"></a>

## 111. MANUAL NOTES

Cho phép player thêm:

- suspicion;
- map pin;
- reminder.

Đặc biệt hữu ích game investigation-heavy.

---

<a id="m17-t041-s172"></a>

## 112. QUEST TRACKING

Player có thể Focus một vài thread để UI ưu tiên.

Không làm các thread khác đóng băng.

---

<a id="m17-t041-s173"></a>

## 113. NO AUTOMATIC WAYPOINT FOR UNKNOWN TARGET

Nếu target unknown:

map chỉ hiển thị:

search region.

known location.

---

<a id="m17-t041-s174"></a>

## 114. WAYPOINT WHEN LOGICALLY KNOWN

Nếu NPC nói:

> “Nhà tôi ở cạnh Bến Hạc.”

và player biết Bến Hạc:

marker hợp lý.

Không anti-marker cực đoan.

---

<a id="m17-t041-s175"></a>

## 115. QUEST COMMUNICATION

Player có thể nhận update qua:

messenger.

rumor.

companion.

Không cần quay lại NPC quest giver để “turn in”.

---

<a id="m17-t041-s176"></a>

## 116. NO TURN-IN REQUIREMENT

Nếu player cứu người rồi người đó tự về làng:

requester biết khi họ về.

Reward/reaction xảy ra khi network truyền information.

---

<a id="m17-t041-s177"></a>

## 117. INFORMATION DELAY

Nếu player hoàn thành xa:

NPC có thể chưa biết.

Quest technically resolved nhưng reputation chưa cập nhật cho đến khi news arrives.

---

<a id="m17-t041-s178"></a>

## 118. FALSE CLAIM OF COMPLETION

Player có thể nói:

> “Ta đã giết con thú.”

dù chưa.

If believed:

temporary social resolution.

Truth may surface later.

---

<a id="m17-t041-s179"></a>

## 119. CLAIMING CREDIT

Player có thể:

claim.

share.

give credit away.

Reputation system xử lý.

---

<a id="m17-t041-s180"></a>

## 120. COMPANION CREDIT

Companion có thể thực sự làm phần chính.

World may credit them.

Party history phản ánh.

---

<a id="m17-t041-s181"></a>

## 121. QUEST REWARD OWNERSHIP

Nếu quest reward là family heirloom:

player nhận quyền sở hữu thật.

Nếu chỉ “mượn”:

không được bán như item thường.

---

<a id="m17-t041-s182"></a>

## 122. REWARD NEGOTIATION

Trước hoặc sau quest có thể negotiate.

Sau khi hoàn thành mà requester không trả:

new Debt/Conflict Thread.

---

<a id="m17-t041-s183"></a>

## 123. BETRAYED CONTRACT

Requester có thể phản bội.

Cần motive.

Không dùng trope ngẫu nhiên để shock.

---

<a id="m17-t041-s184"></a>

## 124. PLAYER BETRAYS REQUESTER

Có thể:

take item.

join opponent.

lie.

Thread biến đổi.

Không hard-fail content.

---

<a id="m17-t041-s185"></a>

## 125. THIRD-PARTY INTERVENTION

Trong quest, actor khác có thể xen vào do world simulation.

Ví dụ:

đang đàm phán debt

→ creditor chết.

Quest thay đổi thành inheritance dispute.

---

<a id="m17-t041-s186"></a>

## 126. ACTOR DEATH DURING QUEST

Quest không được crash logic.

Narrative Successor được tìm:

family.

faction.

heir.

witness.

---

<a id="m17-t041-s187"></a>

## 127. TARGET MIGRATION

NPC có thể rời settlement khi quest đang active.

Player phải track.

Không freeze NPC.

---

<a id="m17-t041-s188"></a>

## 128. TARGET CHANGES MIND

Một NPC ban đầu muốn revenge.

Sau event:

không muốn nữa.

Quest objective phải cập nhật.

---

<a id="m17-t041-s189"></a>

## 129. QUEST AND AGING

Long-term thread có thể trải nhiều năm.

Child missing becomes adult.

Debt inherits.

Family leader changes.

Thread evolves.

---

<a id="m17-t041-s190"></a>

## 130. GENERATIONAL QUEST

Một oath không hoàn thành có thể truyền sang con.

Player successor có thể đối mặt với consequence từ đời trước.

---

<a id="m17-t041-s191"></a>

## 131. LEGACY QUEST

Late-game thread có thể liên quan:

- truyền nghề;
- chọn successor;
- bảo tồn technique;
- sửa lỗi quá khứ;
- bảo vệ family legacy.

Progression chuyển từ power sang legacy.

---

<a id="m17-t041-s192"></a>

## 132. POST-DEATH QUEST CONTINUATION

Nếu player character chết:

active threads không reset.

Successor biết một số thread tùy Knowledge Transfer.

---

<a id="m17-t041-s193"></a>

## 133. KNOWLEDGE LOSS ON SUCCESSION

New character không tự biết mọi điều player cũ biết.

Knowledge được truyền nếu:

- witnessed;
- told;
- journal recovered;
- shared party memory.

Đây là cơ chế rất quan trọng.

---

<a id="m17-t041-s194"></a>

## 134. LOST JOURNAL

Nếu player chết và journal vật lý/game abstraction bị mất trong context hợp lý:

một số knowledge có thể khó recover.

Không nhất thiết dùng hardcore mechanic mặc định.

---

<a id="m17-t041-s195"></a>

## 135. QUEST AND REPUTATION

Reputation không đến từ “quest completed”.

Nó đến từ:

action witnessed.

result known.

rumor.

Ai đó có thể không biết player giải quest.

---

<a id="m17-t041-s196"></a>

## 136. QUEST AND FACTION

Helping one faction can:

improve relations.

create obligations.

hurt rivals.

Nhưng không generic:

Faction +10.

Specific claims generated.

---

<a id="m17-t041-s197"></a>

## 137. QUEST AND PROFESSION

Profession opens:

- solution;
- credibility;
- payment;
- new career milestone.

Một thợ đúc giải sabotage furnace khác warrior.

---

<a id="m17-t041-s198"></a>

## 138. QUEST AND CLASS

Class creates approach.

Không hard-lock main quest.

Example:

Liệp Nhân:

trail.

Sứ Giả:

witness.

Người Giữ Ký Ức:

historical contradiction.

---

<a id="m17-t041-s199"></a>

## 139. QUEST AND PARTY

Party members có:

opinions;

personal stakes;

knowledge;

boundaries.

Quest decision có thể chia party.

---

<a id="m17-t041-s200"></a>

## 140. QUEST AND INJURY

Player injury có thể:

delay quest.

change approach.

force companion to lead.

Quest không pause healing system.

---

<a id="m17-t041-s201"></a>

## 141. QUEST AND TRAVEL

Distance/time là cost thật.

Travel route decisions can determine outcome.

---

<a id="m17-t041-s202"></a>

## 142. QUEST AND DUNGEON

Quest không reset dungeon state.

Nếu player đã khám phá dungeon trước:

knowledge persists.

---

<a id="m17-t041-s203"></a>

## 143. QUEST AND ECONOMY

Solving resource problem changes prices/supply.

Không reward economy độc lập.

---

<a id="m17-t041-s204"></a>

## 144. QUEST AND SETTLEMENT

Settlement variable updates after outcome.

Example:

Food.

Security.

Authority.

Health.

Trade.

---

<a id="m17-t041-s205"></a>

## 145. QUEST AND MẠCH

Spiritual resolution can affect:

Mạch Stability.

belief.

ritual faction.

But truth may remain ambiguous.

---

<a id="m17-t041-s206"></a>

## 146. QUEST & MYTH FORMATION

High-importance quest becomes:

memory.

rumor.

story.

myth.

Later generations may know distorted version.

---

<a id="m17-t041-s207"></a>

## 147. QUEST DIFFICULTY

Không hiển thị:

Quest Level 17.

Difficulty comes from:

- capability;
- knowledge;
- time;
- opposition;
- distance;
- politics;
- resources.

---

<a id="m17-t041-s208"></a>

## 148. SOFT DIFFICULTY WARNING

Party can infer:

> “Vũ cho rằng bốn người sẽ khó giữ được cổng này.”

Không skull icon arbitrary.

---

<a id="m17-t041-s209"></a>

## 149. IMPOSSIBLE CURRENTLY

Một approach có thể chưa khả thi.

Game nên cho biết **vì sao** nếu character hiểu.

> “Không ai trong nhóm biết đủ về cấu trúc để sửa trụ cầu.”

Player có thể tìm expert.

---

<a id="m17-t041-s210"></a>

## 150. QUEST DOES NOT SCALE TO PLAYER

Nếu player yếu:

threat vẫn mạnh.

Có thể avoid.

prepare.

seek allies.

---

<a id="m17-t041-s211"></a>

## 151. QUEST CAN BECOME EASIER OVER TIME

Enemy leaves.

player gains ally.

weather changes.

But can also become harder.

---

<a id="m17-t041-s212"></a>

## 152. QUEST AUTHORING TEMPLATE

Mỗi authored quest cần:

```text
THREAD ORIGIN
CAUSE
CURRENT PRESSURE
ACTORS
ACTOR GOALS
OBJECTIVE TRUTH
PLAYER ENTRY POINTS
KNOWN INFORMATION
POSSIBLE LEADS
CONSTRAINTS
CLOCKS
APPROACHES
STATE VARIABLES
POSSIBLE ESCALATIONS
RESOLUTION STATES
AFTERMATH
FOLLOW-UP THREADS
WORLD CHANGES
```

---

<a id="m17-t041-s213"></a>

## 153. QUEST AUTHORING RULE — START WITH PROBLEM, NOT TASK

Bad:

> Player must collect medicine.

Good:

> Settlement disease pressure increased because healer lost medicine supply.

Từ problem mới tạo possible actions.

---

<a id="m17-t041-s214"></a>

## 154. QUEST AUTHORING RULE — NO REQUIRED SINGLE SOLUTION

Nếu solution buộc:

> find key → open door → kill boss,

đó là tuyến tính action script.

Chỉ dùng khi context thực sự yêu cầu.

---

<a id="m17-t041-s215"></a>

## 155. QUEST AUTHORING RULE — OPPONENT HAS GOAL

Bandit doesn't exist for fight.

Smuggler wants profit.

Leader wants control.

Family wants safety.

---

<a id="m17-t041-s216"></a>

## 156. QUEST AUTHORING RULE — OUTCOME CHANGES STATE

Nếu sau quest:

world giống hệt trước

trừ player có XP,

quest không đạt tiêu chuẩn.

---

<a id="m17-t041-s217"></a>

## 157. QUEST AUTHORING RULE — REWARD MUST COME FROM WORLD

NPC nghèo không trả treasure vô lý.

Reward source must exist.

---

<a id="m17-t041-s218"></a>

## 158. QUEST AUTHORING RULE — INFORMATION HAS SOURCE

Không journal magically knows truth.

---

<a id="m17-t041-s219"></a>

## 159. QUEST AUTHORING RULE — FAILURE IS AUTHORED

Team phải design:

What if player:

- arrives late;
- loses target;
- kills wrong person;
- retreats;
- requester dies?

Không chỉ success path.

---

<a id="m17-t041-s220"></a>

## 160. QUEST AUTHORING RULE — IGNORING IS A VALID INPUT

Mỗi significant thread phải biết:

> nếu player không can thiệp thì sao?

---

<a id="m17-t041-s221"></a>

## 161. QUEST AUTHORING RULE — PLAYER CAN OVER-SOLVE

Ví dụ request:

stop thief.

Player dismantles whole smuggling network.

World should recognize broader impact.

---

<a id="m17-t041-s222"></a>

## 162. QUEST AUTHORING RULE — PLAYER CAN CREATE NEW PROBLEM

Solution A may create Thread B.

This is desirable.

---

<a id="m17-t041-s223"></a>

## 163. PROCEDURAL QUEST QUALITY GATE

Procedural quest chỉ xuất hiện nếu:

1. Có real cause.
2. Actor có real need.
3. Objective object/actor exists.
4. Có ít nhất hai meaningful approaches hoặc một strong situational challenge.
5. Outcome changes state.
6. Reward is plausible.
7. Does not duplicate recent content.
8. Fits region/culture.
9. Has failure continuation.
10. Can be ignored.

---

<a id="m17-t041-s224"></a>

## 164. AUTHORED MAJOR QUEST QUALITY GATE

Major quest thêm:

11. Multiple stakeholders.
12. Hidden/contested information.
13. At least one meaningful irreversible consequence.
14. Companion/faction reaction.
15. Long-term aftermath.
16. Potential historical trace.

---

<a id="m17-t041-s225"></a>

## 165. QUEST CONTENT TIER

<a id="m17-t041-s226"></a>

#### Tier C — Ambient Thread

Small local issue.

No deep scripting.

<a id="m17-t041-s227"></a>

#### Tier B — Local Quest

Household/settlement impact.

<a id="m17-t041-s228"></a>

#### Tier A — Major Quest

Multiple actors, branching consequence.

<a id="m17-t041-s229"></a>

#### Tier S — Historical Quest

Regional/world impact.

---

<a id="m17-t041-s230"></a>

## 166. TIER DOES NOT EQUAL LENGTH

Một 15-minute decision có thể Tier A nếu consequence lớn.

Một long exploration may be Tier B.

---

<a id="m17-t041-s231"></a>

## 167. QUEST VOLUME PHILOSOPHY

Game không cần 500 shallow quests.

Tốt hơn:

fewer active threads

nhưng:

persistent.

intersecting.

revisitable.

---

<a id="m17-t041-s232"></a>

## 168. TARGET CONTENT DENSITY

Production có thể hướng tới:

- nhiều Ambient Threads;
- moderate Local Quests;
- fewer Major Questlines;
- limited Historical Anchors.

Exact counts deferred.

---

<a id="m17-t041-s233"></a>

## 169. DAILY LIFE QUESTS

Không phải tất cả quest là tragedy.

Examples:

- wedding preparation;
- apprentice test;
- boat launch;
- trade festival;
- family reunion;
- hunting competition.

Positive stories matter.

---

<a id="m17-t041-s234"></a>

## 170. CELEBRATION QUEST

Player có thể:

participate.

help.

mediate.

learn culture.

Không cần danger.

World needs joy.

---

<a id="m17-t041-s235"></a>

## 171. ROMANCE THREAD

Không quest hóa thành:

> Bring 3 gifts.

Romantic development comes from relationship.

Quest exposure chỉ khi external problem intersects relationship.

---

<a id="m17-t041-s236"></a>

## 172. FRIENDSHIP THREAD

Likewise:

shared journey.

helping family.

career.

Not affection checklist.

---

<a id="m17-t041-s237"></a>

## 173. QUEST WITHOUT REWARD

Hoàn toàn hợp lệ.

Player giúp vì:

relationship.

belief.

curiosity.

---

<a id="m17-t041-s238"></a>

## 174. QUEST WITH NEGATIVE PERSONAL PAYOFF

Player có thể:

mất tiền.

bị thương.

mất reputation.

Nhưng đạt goal có ý nghĩa.

---

<a id="m17-t041-s239"></a>

## 175. QUEST AS INFORMATION ENGINE

Quest chính là cách player dần hiểu:

World.

History.

Faction.

NPC.

Mạch.

Không dùng lore dump thay thế.

---

<a id="m17-t041-s240"></a>

## 176. QUEST AS CHARACTER ENGINE

Quest decisions create:

Traits.

Memories.

Fear.

Contradictions.

Class Evidence.

Career shifts.

---

<a id="m17-t041-s241"></a>

## 177. QUEST AS WORLD ENGINE

Resolution changes:

Settlement.

Route.

Faction.

Economy.

Population.

Technology.

---

<a id="m17-t041-s242"></a>

## 178. QUEST AS LEGACY ENGINE

Late actions become:

stories.

institutions.

items.

techniques.

myths.

---

<a id="m17-t041-s243"></a>

## 179. EXAMPLE COMPLETE QUEST — “NGƯỜI KHÔNG TRỞ VỀ”

<a id="m17-t041-s244"></a>

### Thread Origin

Vạc Lâm rời Bến Vạc sau một cuộc cãi vã với cha.

Không ai thấy cậu trở về.

Objective truth ban đầu:

Lâm tự ý rời làng.

Không bị bắt cóc.

---

<a id="m17-t041-s245"></a>

## 180. DAY 1 — HIDDEN THREAD

Family nghĩ:

Lâm chỉ bỏ đi một lúc.

Không quest.

---

<a id="m17-t041-s246"></a>

## 181. DAY 2 — PRESSURE

Mẹ bắt đầu lo.

Cha vẫn nói:

> “Nó sẽ quay về.”

Family Conflict tăng.

---

<a id="m17-t041-s247"></a>

## 182. DAY 3 — QUEST EXPOSURE A

Nếu player thân family:

mẹ nhờ tìm.

Known Facts:

Lâm mất ba ngày.

Cãi cha.

Mang theo dao.

---

<a id="m17-t041-s248"></a>

## 183. QUEST EXPOSURE B

Nếu không biết family:

player nghe rumor tại bến.

Journal:

> “Một thanh niên nhà Vạc đã mất tích.”

Không automatic objective.

---

<a id="m17-t041-s249"></a>

## 184. QUEST EXPOSURE C

Player khám phá mái chèo.

Nếu biết family mark:

liên kết.

Nếu không:

chỉ item clue.

---

<a id="m17-t041-s250"></a>

## 185. LEADS

<a id="m17-t041-s251"></a>

#### Father

Biết nguyên nhân cãi nhưng giấu.

<a id="m17-t041-s252"></a>

#### Sister

Biết Lâm nói muốn đi Hải Môn.

<a id="m17-t041-s253"></a>

#### Fisherman

Thấy một boat đi downstream.

<a id="m17-t041-s254"></a>

#### Mud Trace

Cho thấy ít nhất hai người.

---

<a id="m17-t041-s255"></a>

## 186. FALSE ASSUMPTION

Family tin Lâm đi một mình.

Trace gợi ý hai người.

Investigation opens new possibility.

---

<a id="m17-t041-s256"></a>

## 187. ACTUAL THREAD

Lâm gặp một smuggler contact.

Muốn rời nhà để tham gia vận chuyển hàng.

Nhưng sau đó bị mắc trong một conflict không dự kiến.

---

<a id="m17-t041-s257"></a>

## 188. PLAYER APPROACH — TRACKING

Theo bờ sông.

Find camp.

Potential encounter.

---

<a id="m17-t041-s258"></a>

## 189. PLAYER APPROACH — SOCIAL

Hỏi boat network.

Merchant contacts.

Sứ Giả/Thủy Binh mạnh.

---

<a id="m17-t041-s259"></a>

## 190. PLAYER APPROACH — FAMILY

Thuyết phục cha nói truth về cuộc cãi.

Learn Lâm stole small family item.

---

<a id="m17-t041-s260"></a>

## 191. TIME EFFECT

Nếu nhanh:

Lâm vẫn ở smuggler camp.

Có thể nói chuyện.

Nếu chậm:

camp moves.

Nếu rất chậm:

violent event may occur.

---

<a id="m17-t041-s261"></a>

## 192. POSSIBLE OUTCOME A — LÂM RETURNS

Player thuyết phục.

Family reunion.

Nhưng original conflict chưa tự giải quyết.

Follow-up:

father-son relationship.

---

<a id="m17-t041-s262"></a>

## 193. OUTCOME B — LÂM REFUSES

Player có thể tôn trọng.

Family reaction:

anger/grief.

Player decides what to tell.

---

<a id="m17-t041-s263"></a>

## 194. OUTCOME C — LÂM JOINS SMUGGLERS

New thread.

Player may:

report.

ignore.

help.

---

<a id="m17-t041-s264"></a>

## 195. OUTCOME D — LÂM INJURED

Rescue changes relationship.

Career may change.

---

<a id="m17-t041-s265"></a>

## 196. OUTCOME E — LÂM DIES

Quest transforms:

Body Recovery.

Funeral.

Blame.

Smuggling investigation.

---

<a id="m17-t041-s266"></a>

## 197. OUTCOME F — PLAYER LIES

Player tells family:

> Lâm died

even though alive.

Creates Knowledge divergence.

Years later:

Lâm can return.

Huge consequence.

---

<a id="m17-t041-s267"></a>

## 198. QUEST REWARD

No generic reward necessary.

Potential:

family trust.

boat access.

relationship.

smuggling knowledge.

Lâm as future NPC/companion.

---

<a id="m17-t041-s268"></a>

## 199. WORLD CONSEQUENCE

Depending outcome:

smuggling network remains.

family changes.

Lâm career changes.

rumor spreads.

This is a proper quest because thread survives beyond journal completion.

---

<a id="m17-t041-s269"></a>

## 200. EXAMPLE HISTORICAL QUEST — “ĐẤT DƯỚI THÀNH”

<a id="m17-t041-s270"></a>

### Historical Context

Loa Vực construction begins expanding.

Several households must relocate.

Nhà Chạ directly affected.

---

<a id="m17-t041-s271"></a>

## 201. STAKEHOLDERS

Chạ Lâm:

wants preserve land.

Chạ Minh:

sees opportunity in construction.

Local households:

divided.

Authority:

needs earthworks completed.

Xảo Nhân network:

needs labor/material.

---

<a id="m17-t041-s272"></a>

## 202. QUEST EXPOSURE

No single giver.

Player may:

know Chạ family.

work construction.

represent authority.

hear dispute.

---

<a id="m17-t041-s273"></a>

## 203. POSSIBLE PLAYER GOALS

Player chooses.

Could:

help family keep land.

negotiate compensation.

organize relocation.

support construction.

investigate alternate route.

do nothing.

---

<a id="m17-t041-s274"></a>

## 204. ENGINEERING SOLUTION

Move part of earthwork.

Cost:

more material/time.

May affect defense design.

Not universally available.

---

<a id="m17-t041-s275"></a>

## 205. POLITICAL SOLUTION

Negotiate special compensation.

Creates precedent.

Other households may demand same.

---

<a id="m17-t041-s276"></a>

## 206. FORCE SOLUTION

Authority removes residents.

Construction faster.

Resentment increases.

Future rebellion/rumor.

---

<a id="m17-t041-s277"></a>

## 207. COMMUNITY SOLUTION

Move settlement collectively.

New hamlet forms.

Social Graph preserved more strongly.

---

<a id="m17-t041-s278"></a>

## 208. DELAY

Player does nothing.

Construction eventually advances.

Family outcome simulated.

Quest later may expose:

displacement.

poverty.

career shift.

---

<a id="m17-t041-s279"></a>

## 209. LONG-TERM PAYOFF

Years later player revisits.

Old field is now earthwork.

Chạ Lâm may:

hate player.

respect compromise.

be dead.

Chạ Minh may:

become construction authority.

Same location narrates historical transition.

---

<a id="m17-t041-s280"></a>

## 210. QUEST UI DESIGN

Main journal view should show:

**Thread title**

Current understanding.

Important actors.

Known leads.

Promises/deadlines.

Recent changes.

No mandatory giant objective checklist.

---

<a id="m17-t041-s281"></a>

## 211. QUEST UPDATE STYLE

Avoid:

`QUEST UPDATED +250 XP`

Prefer subtle:

> **Người Không Trở Về**  
> Mộc finally admitted that Lâm had spoken about leaving for Hải Môn.

---

<a id="m17-t041-s282"></a>

## 212. QUEST COMPLETION STYLE

Instead of:

**QUEST COMPLETE**

Use:

> **Thread resolved: Vạc Lâm returned to Bến Vạc.**  
> The dispute with his father remains unresolved.

This distinction is extremely important.

---

<a id="m17-t041-s283"></a>

## 213. RESOLVED THREAD CAN LEAVE OPEN DEBT

Core problem solved.

Unresolved State remains.

New Story Thread may emerge later.

---

<a id="m17-t041-s284"></a>

## 214. ARCHIVE

Resolved thread moves to Chronicle.

Shows:

How discovered.

Key actions.

Outcome.

Long-term aftermath as known.

---

<a id="m17-t041-s285"></a>

## 215. CHRONICLE UPDATES LATER

If years later consequence emerges:

old entry gains new section.

History isn't frozen.

---

<a id="m17-t041-s286"></a>

## 216. QUEST FILTERS

Player can filter by:

Personal.

Family.

Settlement.

Faction.

Historical.

Urgent.

Promises.

Not conventional Main/Side only.

---

<a id="m17-t041-s287"></a>

## 217. MAIN/SIDE LABEL

Could be avoided or de-emphasized.

Historical Thread naturally important.

Personal thread can be equally meaningful.

---

<a id="m17-t041-s288"></a>

## 218. PLAYER PRIORITY

Player can pin:

up to a few Current Concerns.

UI then prioritizes.

No effect on simulation.

---

<a id="m17-t041-s289"></a>

## 219. AI STORY DIRECTOR

Quest exposure is controlled by Story Director.

Inputs:

- narrative heat;
- current region;
- player relationships;
- unresolved threads;
- recent content types;
- urgency.

---

<a id="m17-t041-s290"></a>

## 220. STORY DIRECTOR DOES NOT CREATE TRUTH

It chooses:

which existing thread to expose.

when.

through which source.

It doesn't invent causally unsupported problems.

---

<a id="m17-t041-s291"></a>

## 221. STORY DIRECTOR AVOIDS REPETITION

If player just completed:

three missing-person stories,

system suppresses similar exposure unless major causal event explains cluster.

---

<a id="m17-t041-s292"></a>

## 222. STORY DIRECTOR BALANCES MOOD

After:

war tragedy.

Could expose:

festival.

family reunion.

career opportunity.

Not constant misery.

---

<a id="m17-t041-s293"></a>

## 223. STORY DIRECTOR SUPPORTS CLASS

Without forcing.

If player is Dược Nhân:

health threads slightly more likely to become visible because people seek them out.

World not magically spawns more disease.

---

<a id="m17-t041-s294"></a>

## 224. FAME CHANGES QUEST FLOW

Unknown player:

must seek problems.

Famous player:

problems seek them.

A renowned mediator gets invited to negotiations.

A master healer gets patients.

Progression changes content delivery.

---

<a id="m17-t041-s295"></a>

## 225. POWER CREATES RESPONSIBILITY

High reputation increases:

requests.

obligations.

political pressure.

Becoming powerful is not only convenience.

---

<a id="m17-t041-s296"></a>

## 226. PLAYER CAN REFUSE

Refusing is legitimate.

Requester remembers depending context.

No invisible morality penalty.

---

<a id="m17-t041-s297"></a>

## 227. TOO MANY REQUESTS

Player cannot save everyone.

This is intentional but must be balanced to avoid exhaustion.

Persistent systems should make choices meaningful, not create guilt simulator.

---

<a id="m17-t041-s298"></a>

## 228. QUEST SYSTEM QUALITY GATE

The complete system is successful if:

1. Problems exist before player learns them.
2. Quests can be discovered through multiple sources.
3. Quest framing can be biased or wrong.
4. Objectives can change as knowledge changes.
5. Major quests have multiple approaches.
6. Failure creates state rather than dead end.
7. Ignoring a quest has simulated outcome.
8. NPCs do not freeze during active quest.
9. Time pressure is real when stated.
10. Rewards come from world logic.
11. Quest resolutions alter relationships or world state.
12. No generic XP dependence.
13. Investigation can reach wrong conclusions.
14. Requests are different from Story Threads.
15. Faction, family and profession generate quests naturally.
16. Main story uses same framework.
17. Companion stories can progress without player.
18. Threads can merge and collide.
19. Quest history survives character death.
20. Major quests leave long-term aftermath.

---

<a id="m17-t041-s299"></a>

## 229. DEFERRED TO PRODUCTION

Production phase will define:

- exact Story Director scoring;
- number of active exposed threads;
- procedural quest library;
- journal UX;
- quest notification frequency;
- exact persistence windows;
- authored quest database;
- main campaign quest graph;
- companion quest content;
- procedural narrative templates;
- objective wording;
- localization;
- LLM narrative summarization;
- quest testing tools;
- telemetry for abandoned threads;
- quest balancing by region.

---

<a id="m17-t041-s300"></a>

## 230. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

Quest là player-facing intervention window vào Story Thread.

Story Thread exists without player.

Quest giver is optional.

NPC requests are perspectives, not truth.

Quest can begin from rumor, clue, location, profession, faction or relationship.

Objectives represent current understanding and may change.

Known Leads replace excessive micro-objective checklists.

Deadlines must be simulated.

No fake urgency.

Major quests support multiple approaches where logically possible.

Quest outcomes are state-based, not simple success/failure.

Partial and pyrrhic resolutions are valid.

Threads can merge, collide and transform.

Ignored quests continue.

NPCs continue living while quest active.

Actor death does not invalidate world logic.

Companion story threads belong to companions.

Faction quests arise from faction pressure.

Profession quests arise from actual career needs.

Procedural quests require real causes.

Quest rewards are contextual.

No generic XP reward is required.

Quest completion does not directly generate reputation unless information spreads.

Player can lie about outcomes.

Quest history persists in Chronicle.

Historical campaign uses the same Quest Engine.

Player never needs to be “chosen one”.

Story Director controls exposure, not truth.

Late arrival creates new content instead of only failure.

Quest System must support multi-generation campaign continuity.

---

<a id="m17-t041-s301"></a>

## 231. CORE QUEST STATEMENT

Một RPG thông thường hỏi:

> “Tôi còn bao nhiêu quest chưa làm?”

Game này phải khiến player hỏi:

> “Trong những chuyện tôi biết, chuyện nào đáng để tôi can thiệp?”

> “Tôi có thật sự hiểu điều đang xảy ra không?”

> “Người nhờ tôi có đang nói toàn bộ sự thật không?”

> “Nếu tôi đi giải quyết chuyện này, tôi sẽ bỏ lỡ điều gì khác?”

> “Tôi đang giúp ai?”

> “Ai sẽ chịu thiệt?”

> “Nếu tôi bỏ đi, chuyện này sẽ tự diễn biến như thế nào?”

> “Tôi có cần giải quyết vấn đề này hay chỉ cần sống với hậu quả của nó?”

---

<a id="m17-t041-s302"></a>

## 232. QUEST SYSTEM’S PLACE IN THE WHOLE GAME

Toàn bộ vòng lặp giờ hình thành:

**World State**

→ tạo Pressure.

**Life Event Engine**

→ tạo Event.

**Event**

→ tạo Unresolved State.

**Unresolved States**

→ hình thành Story Thread.

**Social / Travel / Exploration / Profession / Faction**

→ expose thread cho player.

**Quest System**

→ diễn giải phần player biết và những cửa can thiệp.

**Player Action**

→ dùng Dialogue / Exploration / Dungeon / Combat / Craft / Politics.

**Outcome**

→ cập nhật Character + Social Graph + Settlement + Faction + Economy + World.

**Memory & Rumor**

→ tạo Reputation / History.

**Unresolved Consequence**

→ sinh Story Thread mới.

Vòng lặp tiếp tục.

Quest không nằm ở trung tâm thế giới.

**Thế giới nằm ở trung tâm.**

Quest chỉ là cách player nhìn thấy một phần của nó.

---

<a id="m17-t041-s303"></a>

## 233. FINAL DESIGN PRINCIPLE

Nếu một quest có thể được mô tả hoàn chỉnh như:

> “NPC A bảo player đi tới B, giết C, mang D về và nhận E”

thì team phải hỏi lại:

> **Tại sao A cần việc đó?**

> **C muốn gì?**

> **D thuộc về ai?**

> **Nếu player không làm thì sao?**

> **Có cần giết C không?**

> **Nếu D bị phá thì chuyện gì xảy ra?**

> **A lấy đâu ra E?**

> **Ai biết player đã làm việc đó?**

> **Ba năm sau chuyện này còn để lại điều gì?**

Nếu GDD không trả lời được những câu đó:

> **đó chưa phải một quest hoàn chỉnh của game này.**

Với **Quest System**, gần như toàn bộ “lõi chơi” đã nối được với nhau. Bước tiếp theo hợp lý nhất là **Economy + Trade + Crafting + Settlement Interaction**, vì hiện chúng ta đã dùng rất nhiều khái niệm như tài nguyên, workshop, thợ thủ công, scarcity, tuyến thương mại, debt, property và công trình nhưng chưa khóa cơ chế kinh tế tổng thể. Đây cũng là module quyết định liệu Profession thực sự là một lối chơi ngang combat hay chỉ tồn tại trên giấy.

<!-- SOURCE T041 END -->

