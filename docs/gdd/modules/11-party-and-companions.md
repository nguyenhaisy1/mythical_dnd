# Module 11 — Party và Companion

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T029](../../source/conversation.md#t029)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Party baseline 4. Companion command có Accepted/Conditional/Refused/Impossible. Off-party vẫn sống; không teleport để giữ lịch hẹn hoặc ép làm trái moral boundary.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Actor goals, relationship, obligations, health, location và command context. |
| Đầu ra / bên sở hữu | Membership, willingness và orders; Party logic ở Characters/Application, Social/Health giữ owner. |
| Đọc cùng | 04, 05, 12, 16, 21, 25.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Lệnh giết người đầu hàng có thể bị từ chối trước khi mutation; companion xa tiếp tục nghề. |
| Việc chưa được chứng minh | Party control UX và tuning độ ổn định cam kết. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T029 BEGIN -->

Tôi sẽ khóa Party & Companion ở mức có thể đưa trực tiếp vào GDD. Điểm quan trọng là **player không “sở hữu” companion**; họ chỉ có thể thuyết phục những con người trong thế giới cùng đồng hành trong một giai đoạn nhất định.

<a id="m11-t029-s001"></a>

## GDD MODULE 11 — PARTY & COMPANION SYSTEM

<a id="m11-t029-s002"></a>

## 1. DESIGN PHILOSOPHY

Party trong game không phải:

> Player + Tank + Healer + DPS.

Party là:

> **một nhóm con người có mục tiêu, quan hệ, năng lực và lý do riêng để tạm thời cùng bước trên một con đường.**

Companion phải tiếp tục tồn tại như một nhân vật hoàn chỉnh ngay cả khi không ở trong party.

Mỗi companion có:

- gia đình;
- quê quán;
- nghề nghiệp;
- class;
- các mối quan hệ;
- tài sản;
- mục tiêu;
- niềm tin;
- moral boundary;
- career;
- reputation;
- secret;
- unresolved story;
- và tương lai riêng.

Player không tuyển:

> “Level 12 Healer.”

Player tuyển:

> “Một người tên Lau Miên, 44 tuổi, sống tại Bãi Lau, giỏi chữa bệnh nhưng không muốn rời em gái quá lâu.”

Đây là nguyên tắc nền tảng của toàn hệ thống.

---

<a id="m11-t029-s003"></a>

## 2. PARTY STRUCTURE

Đề xuất cấu trúc mặc định:

<a id="m11-t029-s004"></a>

### ACTIVE PARTY

**4 người**

Bao gồm:

- Player Character;
- tối đa 3 Companion.

Đây là nhóm trực tiếp:

- khám phá;
- dialogue;
- dungeon;
- combat;
- travel;
- encounter.

4 người đủ tạo synergy nhưng vẫn giữ được chiều sâu cá nhân.

---

<a id="m11-t029-s005"></a>

## 3. TẠI SAO KHÔNG 6–8 ACTIVE CHARACTER?

Vì game tập trung content và character interaction.

Nếu có 8 nhân vật thường trực:

- dialogue trở nên loãng;
- mỗi decision phải xử lý quá nhiều phản ứng;
- combat phức tạp;
- companion mất individuality;
- social graph party khó đọc.

4 active character tạo điều kiện mỗi người thực sự có vai trò.

---

<a id="m11-t029-s006"></a>

## 4. EXTENDED GROUP

Ngoài Active Party có thể tồn tại:

<a id="m11-t029-s007"></a>

### Đoàn Đồng Hành

Một nhóm lớn hơn đi cùng trong một số trường hợp.

Ví dụ:

- thương đoàn;
- quân đội;
- đoàn người di cư;
- đoàn xây dựng;
- đoàn nghi lễ.

Những NPC này không phải companion active.

Họ tồn tại thông qua:

Group Simulation.

Active Party vẫn là nhóm trung tâm.

---

<a id="m11-t029-s008"></a>

## 5. CAMP NETWORK

Player có thể có nhiều người thân thiết nhưng không phải tất cả cùng đi.

Companion không active có thể:

- ở nhà;
- làm nghề;
- theo faction;
- thực hiện công việc;
- học nghề;
- quản lý property;
- chăm người thân.

Họ tiếp tục simulation.

Không có khái niệm:

> NPC đứng bất động trong camp 5 năm chờ player.

---

<a id="m11-t029-s009"></a>

## 6. COMPANION KHÔNG PHẢI LOẠI NPC RIÊNG

Bất kỳ Tier A character phù hợp nào về nguyên tắc đều có khả năng trở thành companion.

Điều kiện:

- có lý do đi cùng;
- có đủ autonomy;
- không bị obligation ngăn cản;
- relationship phù hợp;
- mục tiêu có overlap với player.

Một số NPC có thể được authored sâu hơn và có companion content đặc biệt.

Nhưng hệ thống nền phải cho phép **emergent companion**.

---

<a id="m11-t029-s010"></a>

## 7. COMPANION ELIGIBILITY

Một NPC không tự động có nút:

> “Join Party.”

Engine đánh giá:

<a id="m11-t029-s011"></a>

#### Relationship

NPC có biết player không?

<a id="m11-t029-s012"></a>

#### Trust

Có tin player không?

<a id="m11-t029-s013"></a>

#### Goal Alignment

Việc đi cùng có giúp mục tiêu của họ không?

<a id="m11-t029-s014"></a>

#### Obligation

Gia đình/nghề nghiệp có cho phép rời đi không?

<a id="m11-t029-s015"></a>

#### Risk Tolerance

NPC có sẵn sàng chịu nguy hiểm không?

<a id="m11-t029-s016"></a>

#### Current Crisis

NPC có lý do phải ở lại không?

<a id="m11-t029-s017"></a>

#### Player Reputation

Player được nhìn nhận thế nào?

<a id="m11-t029-s018"></a>

#### Party Compatibility

NPC có conflict nghiêm trọng với người trong party không?

---

<a id="m11-t029-s019"></a>

## 8. RECRUITMENT KHÔNG PHẢI MỘT CHECK DUY NHẤT

Không có:

> Persuasion 18 → NPC đi theo.

Player không thể dùng một roll social để khiến người có cả gia đình bỏ tất cả.

Recruitment là **story outcome**.

Ví dụ muốn Lau Miên đi cùng:

NPC nói:

> “Ta không thể rời Bãi Lau khi em gái đang bệnh.”

Player có thể:

- chữa em gái;
- tìm người thay Miên;
- đợi;
- thuyết phục em gái đi cùng;
- chứng minh chuyến đi liên quan trực tiếp tới bệnh;
- hoặc từ bỏ.

Sau đó recruitment mới hợp lý.

---

<a id="m11-t029-s020"></a>

## 9. RECRUITMENT MOTIVE

Mỗi companion phải có ít nhất một motive.

Ví dụ:

<a id="m11-t029-s021"></a>

### Shared Goal

Hai người muốn cùng tìm một người mất tích.

<a id="m11-t029-s022"></a>

### Obligation

NPC mắc nợ player.

<a id="m11-t029-s023"></a>

### Friendship

Muốn giúp player.

<a id="m11-t029-s024"></a>

### Career Opportunity

Chuyến đi giúp nghề nghiệp.

<a id="m11-t029-s025"></a>

### Curiosity

Muốn khám phá.

<a id="m11-t029-s026"></a>

### Faction Duty

Được phái đi cùng.

<a id="m11-t029-s027"></a>

### Family Duty

Muốn bảo vệ một thành viên.

<a id="m11-t029-s028"></a>

### Revenge

Có chung enemy.

<a id="m11-t029-s029"></a>

### Belief

Cho rằng nhiệm vụ có ý nghĩa tâm linh.

<a id="m11-t029-s030"></a>

### Survival

Không còn nơi nào khác để đi.

---

<a id="m11-t029-s031"></a>

## 10. MOTIVE CÓ THỂ HẾT HẠN

Nếu NPC tham gia vì:

> tìm em gái.

Sau khi tìm được em gái:

Recruitment Motive resolved.

NPC không mặc định:

> “Từ giờ tôi sẽ đi theo player mãi mãi.”

Engine đánh giá:

**New Reason To Stay?**

Nếu có:

companion tiếp tục.

Nếu không:

họ sẽ đề nghị rời.

---

<a id="m11-t029-s032"></a>

## 11. PERMANENT COMPANION KHÔNG PHẢI MẶC ĐỊNH

Có ba dạng companion:

<a id="m11-t029-s033"></a>

#### Temporary Companion

Đi cùng vì một nhiệm vụ cụ thể.

<a id="m11-t029-s034"></a>

#### Long-term Companion

Có mục tiêu dài hạn chung.

<a id="m11-t029-s035"></a>

#### Life Companion

Quan hệ đủ mạnh để gắn bó nhiều năm.

Life Companion cực hiếm.

Không phải character archetype.

Nó là kết quả của lịch sử chung.

---

<a id="m11-t029-s036"></a>

## 12. COMPANION CONTRACT

Khi một character đi cùng party, hệ thống lưu một **Companion Agreement**.

Không nhất thiết là hợp đồng bằng văn bản.

Bao gồm:

- lý do đi cùng;
- kỳ vọng;
- compensation nếu có;
- destination;
- personal boundary;
- duration expectation;
- obligations bị tạm hoãn.

Ví dụ:

> “Tôi dẫn các anh qua Tản Viên. Sau đó tôi trở về.”

Player biết rõ expectation.

---

<a id="m11-t029-s037"></a>

## 13. PARTY MEMBER OWNERSHIP

Companion sở hữu đồ của chính họ.

Không được coi toàn bộ inventory party là của player.

Có:

<a id="m11-t029-s038"></a>

### Personal Equipment

Companion sở hữu.

<a id="m11-t029-s039"></a>

### Shared Supplies

Party sở hữu chung.

<a id="m11-t029-s040"></a>

### Player Property

Player sở hữu.

<a id="m11-t029-s041"></a>

### Loaned Equipment

Một người cho người khác mượn.

---

<a id="m11-t029-s042"></a>

## 14. PLAYER KHÔNG TỰ DO LỘT ĐỒ COMPANION

Nếu player muốn lấy vũ khí quý của companion:

companion có thể từ chối.

Relationship matters.

Ví dụ:

> “Đây là giáo của cha tôi.”

Không tồn tại:

> click unequip → lấy item.

Điều này giữ individuality.

---

<a id="m11-t029-s043"></a>

## 15. EQUIPMENT GIFT

Player có thể tặng equipment.

Nhưng không phải:

> Gift +10 affection.

NPC đánh giá:

- item có hữu ích không;
- nguồn gốc;
- giá trị;
- intent;
- cultural meaning.

Một món quà quá giá trị có thể khiến NPC nghi ngờ.

---

<a id="m11-t029-s044"></a>

## 16. SHARED PARTY RESOURCES

Những thứ thường dùng chung:

- thức ăn;
- nước;
- vật liệu camp;
- thuốc cơ bản;
- một số công cụ;
- loot chưa chia.

Party phải có Resource Agreement.

---

<a id="m11-t029-s045"></a>

## 17. LOOT DISTRIBUTION

Không nên mặc định:

> player lấy tất cả loot.

Có thể có nhiều nguyên tắc:

<a id="m11-t029-s046"></a>

#### Player Controls

Nhóm rất loyal.

<a id="m11-t029-s047"></a>

#### Equal Share

Mọi người chia.

<a id="m11-t029-s048"></a>

#### Need-Based

Ai cần gì lấy.

<a id="m11-t029-s049"></a>

#### Employer Contract

Player trả compensation cố định.

<a id="m11-t029-s050"></a>

#### Cultural Rule

Một số nhóm có luật riêng.

---

<a id="m11-t029-s051"></a>

## 18. LOOT CONFLICT

Nếu player luôn lấy đồ quý:

companion có thể:

- giảm Trust;
- giảm Respect;
- đòi chia;
- giấu loot;
- rời party.

Đây là social consequence.

---

<a id="m11-t029-s052"></a>

## 19. PARTY RELATIONSHIP GRAPH

Relationship không chỉ tồn tại giữa:

Player ↔ Companion.

Mà tồn tại:

Companion A ↔ Companion B.

Ví dụ party:

Mân.

Lau Miên.

Lạc Nghi.

Thạch Vũ.

Graph có 6 relationship pair khác nhau.

Mân có thể rất thích Miên.

Miên ghét Lạc Nghi.

Nghi kính trọng Vũ.

Vũ không tin Nghi.

Party vì vậy có internal dynamics.

---

<a id="m11-t029-s053"></a>

## 20. PARTY RELATIONSHIP DIMENSIONS

Sử dụng Social Graph đã thiết kế:

- Affection;
- Trust;
- Respect;
- Fear;
- Obligation;
- Jealousy;
- Attraction;
- Resentment.

Thêm hai party-specific dimension:

<a id="m11-t029-s054"></a>

### Reliability

> “Người này có giữ được việc được giao không?”

<a id="m11-t029-s055"></a>

### Tactical Trust

> “Tôi có tin người này khi nguy hiểm không?”

Một người có thể không thích companion nhưng Tactical Trust rất cao.

---

<a id="m11-t029-s056"></a>

## 21. PARTY COHESION

Party có một derived state:

<a id="m11-t029-s057"></a>

## Cohesion

Nhưng không phải một reputation bar đơn giản.

Cohesion tính từ:

- shared memories;
- relationship;
- leadership;
- unresolved conflict;
- recent success/failure;
- hunger;
- fatigue;
- ideological conflict.

Cohesion ảnh hưởng:

- teamwork;
- morale;
- conflict probability;
- willingness to take risk;
- ability to coordinate.

---

<a id="m11-t029-s058"></a>

## 22. COHESION KHÔNG PHẢI “PARTY HAPPINESS”

Party có thể cohesion rất cao nhưng affection thấp.

Ví dụ bốn veteran:

không thân.

Nhưng tin nhau hoàn toàn trong combat.

Ngược lại:

bốn người rất yêu quý nhau nhưng chưa từng gặp nguy hiểm chung.

Affection cao.

Tactical Cohesion thấp.

---

<a id="m11-t029-s059"></a>

## 23. SHARED MEMORY

Party tạo Shared Memory qua:

- combat;
- travel;
- death;
- success;
- festival;
- famine;
- argument;
- betrayal.

Ví dụ:

> “Cả nhóm sống sót sau trận lũ tại Tam Giang.”

Memory này tồn tại trong tất cả participant.

Nhưng Interpretation khác nhau.

---

<a id="m11-t029-s060"></a>

## 24. PARTY HISTORY

Sau nhiều năm, party có Chronicle riêng.

Ví dụ:

**Năm 8**

Mân gặp Miên tại Bãi Lau.

**Năm 9**

Vũ gia nhập đoàn.

**Năm 10**

Cả nhóm vượt Tản Viên.

**Năm 11**

Nghi rời nhóm sau tranh cãi.

**Năm 12**

Nghi quay lại để cảnh báo về chiến tranh.

Party tự có lịch sử.

---

<a id="m11-t029-s061"></a>

## 25. COMPANION PERSONAL GOAL

Mỗi companion luôn có:

<a id="m11-t029-s062"></a>

### Immediate Goal

Điều muốn làm hiện tại.

<a id="m11-t029-s063"></a>

### Long-term Goal

Điều muốn đạt trong nhiều năm.

<a id="m11-t029-s064"></a>

### Hidden Need

Psychological need.

Ví dụ:

Thạch Vũ:

Immediate:

kiếm thức ăn.

Long-term:

khôi phục danh dự.

Hidden Need:

tha thứ cho bản thân.

Player không nhất thiết biết Hidden Need.

---

<a id="m11-t029-s065"></a>

## 26. COMPANION GOAL KHÔNG ĐÓNG BĂNG

Life Event có thể thay đổi.

Ví dụ:

companion muốn làm master smith.

Con họ chết.

Goal có thể chuyển sang:

trả thù.

Hoặc:

bỏ nghề.

Hoặc:

bảo vệ family.

Companion không chỉ chạy một scripted questline.

---

<a id="m11-t029-s066"></a>

## 27. PERSONAL STORY THREAD

Companion không có “Personal Quest” cố định duy nhất.

Họ có:

**Personal Story Threads.**

Ví dụ:

Missing Brother.

Family Debt.

Career Rivalry.

Spiritual Doubt.

Một companion có thể có nhiều thread.

---

<a id="m11-t029-s067"></a>

## 28. COMPANION QUEST EXPOSURE

Thread chỉ trở thành quest khi:

- player biết;
- companion sẵn sàng nói;
- đúng location;
- đúng time;
- hoặc event trigger.

Ví dụ companion có family secret.

Relationship Trust thấp:

không nói.

Sau 40 giờ:

shared danger tăng Trust.

Camp dialogue:

NPC quyết định kể.

Quest mới mở.

---

<a id="m11-t029-s068"></a>

## 29. CAMP SYSTEM

Camp là nơi quan trọng về narrative.

Không cần biến thành base-builder phức tạp.

Camp có bốn chức năng:

<a id="m11-t029-s069"></a>

#### Rest

Recovery.

<a id="m11-t029-s070"></a>

#### Social

Conversation.

<a id="m11-t029-s071"></a>

#### Planning

Route, task, resource.

<a id="m11-t029-s072"></a>

#### Reflection

Character processing life events.

---

<a id="m11-t029-s073"></a>

## 30. CAMP LOCATION MATTERS

Camp ở:

rừng;

bờ sông;

settlement;

ruin;

military camp

có:

risk;

comfort;

food;

privacy khác nhau.

Một companion không nói secret nhạy cảm khi có NPC lạ xung quanh.

---

<a id="m11-t029-s074"></a>

## 31. CAMP CONVERSATION KHÔNG PHẢI CHECKLIST

Không có:

`!` trên đầu companion mỗi tối.

Dialogue được ưu tiên theo:

- recent event;
- emotional weight;
- unresolved conflict;
- relationship;
- privacy;
- fatigue.

Có thể có một đêm:

không ai nói gì đáng kể.

Điều này làm các cuộc trò chuyện có trọng lượng hơn.

---

<a id="m11-t029-s075"></a>

## 32. BANTER

Party banter phát sinh từ:

Psychology

+

Relationship

+

Current Context

+

Shared Memory.

Ví dụ đi qua lũ:

Mân:

nhớ anh.

Miên:

nhận ra Mân căng thẳng.

Vũ:

không hiểu.

Dialogue khác hoàn toàn nếu party khác.

---

<a id="m11-t029-s076"></a>

## 33. LLM ROLE TRONG BANTER

Simulation Engine quyết định:

- ai có lý do nói;
- họ biết gì;
- thái độ;
- emotional state;
- intent.

LLM chỉ viết câu thoại.

Không được để LLM tự tạo:

family history;

secret;

new facts

trái với World Graph.

---

<a id="m11-t029-s077"></a>

## 34. COMPANION AUTONOMY

Companion có khả năng:

- đề xuất;
- phản đối;
- từ chối;
- tự hành động trong một số tình huống;
- rời nhóm.

Player không điều khiển tư tưởng của họ.

---

<a id="m11-t029-s078"></a>

## 35. DIRECT CONTROL TRONG COMBAT

Để gameplay tactical rõ ràng:

Player được điều khiển action của toàn Active Party trong combat.

Nhưng có giới hạn narrative.

Một companion có thể từ chối một action nếu nó vượt:

**Hard Moral Boundary.**

Ví dụ:

player ra lệnh companion giết trẻ em.

Character có Moral Boundary:

> tuyệt đối không làm hại trẻ.

Action bị từ chối.

Relationship bị ảnh hưởng vì player đã yêu cầu điều đó.

---

<a id="m11-t029-s079"></a>

## 36. SOFT REFUSAL

Một số lệnh không vi phạm hard boundary nhưng trái personality.

Companion vẫn thực hiện nếu:

Tactical Trust cao.

Leadership cao.

Nhưng có thể:

stress;

resentment;

relationship consequence.

Ví dụ healer bị bắt lao vào tuyến đầu.

---

<a id="m11-t029-s080"></a>

## 37. COMPANION MORAL BOUNDARY

Mỗi Tier A companion có 2–5 boundary.

Ví dụ:

- không phá mộ;
- không giết người đầu hàng;
- không bỏ trẻ em;
- không phản gia đình;
- không làm hại thú thiêng.

Boundary không phải preference.

Nó là phần identity.

---

<a id="m11-t029-s081"></a>

## 38. BOUNDARY BREAK

Một companion vẫn có thể bị ép vượt boundary trong extreme condition.

Nếu xảy ra:

đó là Life Event Severity cao.

Có thể tạo:

- trauma;
- guilt;
- resentment;
- class shift;
- party departure.

Không được coi như một action bình thường.

---

<a id="m11-t029-s082"></a>

## 39. COMPANION OPINION VỀ PLAYER DECISION

Sau major decision:

mỗi companion chạy Interpretation Engine.

Ví dụ player quyết định phá đê cứu Làng A nhưng làm ngập Làng B.

Companion A:

> coi đó là quyết định cần thiết.

B:

> coi player là kẻ hy sinh người yếu.

C:

> đánh giá từ góc engineering.

Không dùng:

`Good Choice / Evil Choice`.

---

<a id="m11-t029-s083"></a>

## 40. KHÔNG CÓ GOOD/EVIL METER

Companion không đánh giá player theo Alignment chung.

Họ đánh giá theo:

- values;
- relationship;
- belief;
- consequences;
- knowledge.

Cùng một quyết định có thể:

+Respect với A.

-Trust với B.

+Fear với C.

---

<a id="m11-t029-s084"></a>

## 41. PLAYER REPUTATION TRUYỀN VÀO PARTY

Companion có thể bị ảnh hưởng bởi danh tiếng của player.

Ví dụ player bị coi là:

> kẻ phản Hùng.

Companion vẫn trung thành.

Nhưng family của họ có thể gây áp lực.

Companion phải chọn:

party hay family.

---

<a id="m11-t029-s085"></a>

## 42. PARTY INTERNAL CONFLICT

Conflict Engine theo dõi:

- value mismatch;
- ideological difference;
- jealousy;
- unresolved resentment;
- resource dispute;
- romantic tension;
- leadership conflict;
- family/faction loyalty.

Conflict không nhất thiết tiêu cực.

Nó tạo character development.

---

<a id="m11-t029-s086"></a>

## 43. CONFLICT STAGES

Party conflict có thể qua:

**Tension**

↓

**Argument**

↓

**Confrontation**

↓

**Decision**

↓

**Aftermath**

Không phải mọi tension đi đến confrontation.

---

<a id="m11-t029-s087"></a>

## 44. PLAYER CAN MEDIATE

Player có thể:

- lắng nghe;
- đứng về một bên;
- thương lượng;
- tránh;
- áp đặt;
- để họ tự xử.

Cách xử lý ảnh hưởng:

Trust.

Respect.

Authority.

Relationship giữa companions.

---

<a id="m11-t029-s088"></a>

## 45. PLAYER KHÔNG PHẢI LUÔN LÀ LEADER

Đây là điểm rất quan trọng.

Player Character có thể:

KHÍ 2.

Leadership Skill thấp.

Đi cùng một Thủ Lĩnh nổi tiếng.

Trong một số tình huống:

NPC khác nhìn companion đó là leader thực tế.

Player vẫn là character người chơi điều khiển.

Nhưng không mặc định cả thế giới coi họ chỉ huy.

---

<a id="m11-t029-s089"></a>

## 46. PARTY LEADERSHIP

Party có:

<a id="m11-t029-s090"></a>

### Nominal Leader

Ai được nhóm đồng ý là người quyết định chung.

<a id="m11-t029-s091"></a>

### Context Leader

Người có authority theo tình huống.

Ví dụ:

sông → Thủy Binh dẫn.

rừng → Liệp Nhân.

chữa bệnh → Dược Nhân.

nghi lễ → Vu Chúc.

battle → Lạc Binh/Thủ Lĩnh.

---

<a id="m11-t029-s092"></a>

## 47. CONTEXT LEADERSHIP

Nếu player cho đúng người dẫn:

performance tăng.

Nếu player insist tự dẫn dù không biết:

party có thể thất bại.

Đây là cách khiến companion expertise thực sự quan trọng.

---

<a id="m11-t029-s093"></a>

## 48. DECISION AUTHORITY

Major party decision có thể theo:

<a id="m11-t029-s094"></a>

#### Player Authority

Nếu party chấp nhận player là leader.

<a id="m11-t029-s095"></a>

#### Consultation

Các thành viên góp ý nhưng player quyết.

<a id="m11-t029-s096"></a>

#### Consensus

Một số nhóm hình thành như vậy.

<a id="m11-t029-s097"></a>

#### Contract Constraint

Một temporary companion chỉ đồng ý trong phạm vi mission.

Party structure vì vậy có thể khác giữa playthrough.

---

<a id="m11-t029-s098"></a>

## 49. PARTY ROLE KHÁC CLASS

Role là chức năng trong nhóm.

Ví dụ:

- Guide;
- Scout;
- Healer;
- Negotiator;
- Quartermaster;
- Defender;
- Hunter;
- Craft Specialist;
- Ritual Specialist.

Một character có thể giữ nhiều Role.

Role có thể đổi mà không đổi Class.

---

<a id="m11-t029-s099"></a>

## 50. PARTY ROLE ASSIGNMENT

Không bắt buộc menu cứng.

Game có thể tự suy luận.

Nhưng player được chỉ định một số trách nhiệm.

Ví dụ:

**Route Lead:** Mân.

**Medical Lead:** Miên.

**Camp Watch:** Vũ.

**Negotiation Lead:** Nghi.

Assignments tạo:

- practice;
- responsibility;
- relationship;
- event.

---

<a id="m11-t029-s100"></a>

## 51. FAILURE OF RESPONSIBILITY

Nếu người được giao Camp Watch ngủ quên:

đó không chỉ là penalty.

Có thể tạo:

Trust giảm.

Guilt.

Argument.

Memory.

Nếu hậu quả nghiêm trọng:

Life Event.

---

<a id="m11-t029-s101"></a>

## 52. COMPANION CLASS PROGRESSION

Companion progression không do player click Skill Tree thay họ.

Companion có:

- preferences;
- talents;
- goals;
- mentor;
- behaviour.

Player ảnh hưởng qua:

- task assignment;
- teaching;
- equipment;
- exposure;
- encouragement.

---

<a id="m11-t029-s102"></a>

## 53. COMPANION TECHNIQUE CHOICE

Khi companion eligible nhiều Technique:

AI quyết dựa trên:

Personality.

Current Role.

Goal.

Party Need.

Mentor influence.

Player có thể khuyên.

Không phải command tuyệt đối.

---

<a id="m11-t029-s103"></a>

## 54. TEACHING BETWEEN PARTY MEMBERS

Party members có thể dạy nhau.

Ví dụ:

Mân dạy Miên bơi.

Miên dạy Mân herbalism.

Vũ dạy tracking.

Điều này tạo:

Skill progress

+

Relationship.

---

<a id="m11-t029-s104"></a>

## 55. TEACHING REQUIRES TIME

Không có:

> click Train → unlock.

Training diễn ra:

camp;

journey;

settlement downtime.

Knowledge transfer có tốc độ thực.

---

<a id="m11-t029-s105"></a>

## 56. COMPANION SPECIALTY

Companion phải giữ một số thứ mà player khó thay thế.

Ví dụ:

Miên có Trade Secret về thuốc.

Player có thể học.

Nhưng mất nhiều thời gian.

Do đó giữ companion có giá trị.

---

<a id="m11-t029-s106"></a>

## 57. PARTY SKILL COVERAGE

UI có thể hiển thị:

<a id="m11-t029-s107"></a>

#### Nhóm hiện có thế mạnh

- River Navigation;
- Medicine;
- Tracking.

<a id="m11-t029-s108"></a>

#### Nhóm đang thiếu

- Political negotiation;
- Metallurgy;
- Ritual expertise.

Nhưng không nói:

> “Bạn cần healer.”

Player quyết định chấp nhận weakness hoặc tìm người phù hợp.

---

<a id="m11-t029-s109"></a>

## 58. PARTY KHÔNG CẦN “ĐỦ ROLE”

Một party toàn social character vẫn chơi được.

Một party martial-heavy cũng được.

World problem có nhiều solution.

Party composition thay đổi route và difficulty.

---

<a id="m11-t029-s110"></a>

## 59. TRAVEL PARTY SYSTEM

Mỗi chuyến đi có:

- Destination;
- Route;
- Duration;
- Food;
- Water;
- Weather;
- Threat;
- Party Capability.

Companions đóng góp vào Travel Score theo chuyên môn.

---

<a id="m11-t029-s111"></a>

## 60. TRAVEL TASKS

Trong hành trình có các trách nhiệm:

Navigator.

Scout.

Forager.

Cook/Provision.

Watch.

Medical.

Animal handling nếu sau này có.

Không phải mọi nhiệm vụ cần người riêng.

---

<a id="m11-t029-s112"></a>

## 61. FATIGUE LÀ PARTY STORY DRIVER

Một companion mệt:

performance giảm.

Irritability có thể tăng.

Conflict likelihood tăng.

Người có personality kiên nhẫn xử lý khác người impulsive.

Fatigue không chỉ là stamina debuff.

---

<a id="m11-t029-s113"></a>

## 62. FOOD PREFERENCE & CULTURE

Không cần đi quá sâu thành survival simulator.

Nhưng food có thể ảnh hưởng character moments.

Ví dụ người xa quê gặp món ăn quen:

Mood/Memory trigger.

Companion có thể kể chuyện childhood.

Content nhỏ nhưng làm nhân vật sống.

---

<a id="m11-t029-s114"></a>

## 63. PARTY CAMP DUTY

Ai dựng lều?

Ai nhóm lửa?

Ai nấu?

Ai canh?

Ai chăm wounded?

Duty có thể tự động nếu party đã ổn định.

Chỉ expose khi:

- conflict;
- thiếu người;
- weather;
- danger.

---

<a id="m11-t029-s115"></a>

## 64. NIGHT WATCH

Có thể tạo encounter.

Người canh có:

GIÁC.

Skill.

Fatigue.

Nếu thấy thứ bất thường:

họ phải quyết định:

đánh thức party?

tự điều tra?

bỏ qua?

Psychology ảnh hưởng.

---

<a id="m11-t029-s116"></a>

## 65. COMPANION INITIATIVE OUTSIDE PLAYER VIEW

Companion không chờ player xử lý mọi thứ.

Ví dụ Miên thấy child bị thương ở settlement.

Nếu Compassion cao:

có thể tự đi giúp.

Player tìm companion sau.

Đây là autonomy nhỏ nhưng quan trọng.

---

<a id="m11-t029-s117"></a>

## 66. AUTONOMOUS ACTION BOUNDARY

Không để NPC tự làm hành động phá kế hoạch lớn vô cớ.

Autonomous action chỉ mạnh khi:

- core value;
- hard boundary;
- immediate crisis;
- personal thread.

Ví dụ Vũ thấy người đang giết linh vật liên quan trauma của mình.

Có thể phản ứng mạnh.

---

<a id="m11-t029-s118"></a>

## 67. COMPANION CAN LEAVE TEMPORARILY

Reasons:

- family crisis;
- profession duty;
- injury recovery;
- personal investigation;
- political summons;
- ritual obligation.

Không phải departure nào cũng negative.

---

<a id="m11-t029-s119"></a>

## 68. COMPANION OFFSCREEN SIMULATION

Khi rời party:

họ tiếp tục:

career;

relationship;

event;

aging.

Player gặp lại có thể thấy:

- class tiến triển;
- kết hôn;
- có con;
- bị thương;
- đổi faction;
- giàu lên;
- thất bại.

---

<a id="m11-t029-s120"></a>

## 69. REJOINING

NPC không đóng băng relationship.

Khi gặp lại:

engine tính:

time apart;

new events;

rumor about player;

personal changes.

Có thể:

rất vui.

lạnh nhạt.

hoặc không còn phù hợp để đi cùng.

---

<a id="m11-t029-s121"></a>

## 70. DISMISSAL

Player có thể đề nghị companion rời.

Context quan trọng.

Nếu:

> “Chuyến này xong rồi, anh nên về với gia đình.”

có thể bình thường.

Nếu bỏ người bị thương giữa rừng:

đó là betrayal.

Không có một nút Dismiss trung tính trong mọi trường hợp.

---

<a id="m11-t029-s122"></a>

## 71. ABANDONMENT

Bỏ companion trong tình huống nguy hiểm tạo:

Major Relationship Event.

Có thể lan qua reputation.

Nếu họ sống:

có thể trở thành enemy.

---

<a id="m11-t029-s123"></a>

## 72. PARTY DEATH

Companion có thể chết vĩnh viễn nếu mode cho phép.

Death tạo:

grief;

inheritance;

equipment ownership;

family reaction;

party morale;

story thread.

Không spawn replacement tương đương.

---

<a id="m11-t029-s124"></a>

## 73. BODY RECOVERY

Trong culture game, xác chết có ý nghĩa.

Party có thể:

mang thi thể về;

chôn tại chỗ;

mất xác;

bỏ lại.

Quyết định ảnh hưởng:

family;

belief;

spiritual thread.

Đặc biệt ở Tam Giang hoặc Gò Cò.

---

<a id="m11-t029-s125"></a>

## 74. COMPANION FUNERAL

Nếu party member lâu năm chết:

funeral có thể trở thành major narrative event.

Ai đến?

Family.

Mentor.

Students.

Faction.

Người từng được họ cứu.

Career Legacy xuất hiện.

---

<a id="m11-t029-s126"></a>

## 75. SUCCESSION AFTER PLAYER DEATH

Đây là module kết nối Legacy.

Nếu Player Character chết:

campaign không nhất thiết kết thúc.

Possible Successor Candidate:

- active companion;
- child;
- sibling;
- apprentice;
- close ally.

---

<a id="m11-t029-s127"></a>

## 76. SUCCESSOR ELIGIBILITY

Đánh giá:

Relationship.

Knowledge of current story.

Motivation.

Physical availability.

Player Legacy connection.

Không phải companion mạnh nhất auto trở thành player.

---

<a id="m11-t029-s128"></a>

## 77. CONTROL TRANSFER

Ví dụ Mân chết.

Party còn:

Miên.

Vũ.

Nghi.

Nếu Miên có:

high relationship.

biết main thread.

motive tiếp tục.

Game có thể chuyển player control sang Miên.

Từ đó:

gameplay build thay đổi hoàn toàn.

---

<a id="m11-t029-s129"></a>

## 78. SUCCESSION KHÔNG COPY INVENTORY/CLASS

New player character giữ chính character sheet của họ.

Một số vật của người chết có thể được thừa kế.

Main story tiếp tục từ góc nhìn mới.

Điều này tạo trải nghiệm rất đặc biệt.

---

<a id="m11-t029-s130"></a>

## 79. PLAYER LEGACY TRONG COMPANION

Companion từng sống với player lưu:

Shared Memories.

Nếu trở thành successor:

họ có thể nhắc player cũ.

Player đầu tiên trở thành NPC lịch sử của chính save.

---

<a id="m11-t029-s131"></a>

## 80. ROMANCE

Romance có thể tồn tại nhưng không phải companion mechanic bắt buộc.

Không có:

> heart meter + tặng 10 món.

Romance sinh từ:

Attraction.

Compatibility.

Shared Memory.

Trust.

Timing.

Availability.

Culture.

---

<a id="m11-t029-s132"></a>

## 81. PLAYER KHÔNG THỂ ROMANCE TẤT CẢ NPC BẰNG CHECK

Attraction là two-way state.

Nếu NPC không có attraction:

Persuasion không tạo romantic desire từ con số 0.

Player có thể:

tạo closeness.

Nhưng không ép attraction.

---

<a id="m11-t029-s133"></a>

## 82. COMPANION-COMPANION ROMANCE

Hai companion có thể yêu nhau.

Không cần player là trung tâm.

Điều này có thể dẫn đến:

- marriage;
- child;
- departure;
- conflict;
- stronger party bond.

---

<a id="m11-t029-s134"></a>

## 83. JEALOUSY KHÔNG MẶC ĐỊNH

Romantic triangle chỉ sinh khi psychology phù hợp.

Không dùng drama generator đơn giản.

Character mature có thể chấp nhận rejection bình thường.

---

<a id="m11-t029-s135"></a>

## 84. FRIENDSHIP QUAN TRỌNG NGANG ROMANCE

Game phải đầu tư vào:

- friendship;
- sibling-like bond;
- mentor;
- rivalry;
- respect.

Không để romance là relationship content sâu duy nhất.

---

<a id="m11-t029-s136"></a>

## 85. RIVAL COMPANIONS

Hai người có thể:

Respect cao.

Affection thấp.

Competition cao.

Đây có thể là relationship cực mạnh.

Ví dụ hai master hunter liên tục thách thức nhau.

---

<a id="m11-t029-s137"></a>

## 86. PARTY IDEOLOGY

Qua thời gian, nhóm có thể hình thành một số norm.

Ví dụ:

> không bỏ người bị thương.

> luôn chia loot đều.

> không động vào mộ.

Các norm không cần menu chính thức.

Chúng hình thành từ repeated decisions.

---

<a id="m11-t029-s138"></a>

## 87. BREAKING PARTY NORM

Nếu player đột nhiên vi phạm:

companions phản ứng mạnh hơn vì:

> “Chúng ta chưa bao giờ làm thế.”

Shared Identity của party có ý nghĩa.

---

<a id="m11-t029-s139"></a>

## 88. PARTY NAME

Không bắt buộc.

Nếu nhóm nổi tiếng:

người ngoài có thể đặt tên.

Ví dụ:

> “Những người ở Bến Vạc.”

Sau này rumor có thể biến thành một tên khác.

Player không nhất thiết tự đặt.

---

<a id="m11-t029-s140"></a>

## 89. PARTY REPUTATION

Ngoài reputation từng cá nhân, nhóm có reputation collective.

Ví dụ:

> nhóm chuyên tìm người mất tích.

> nhóm không bao giờ giữ lời.

> đoàn của người thợ đúc.

Collective reputation ảnh hưởng recruitment.

---

<a id="m11-t029-s141"></a>

## 90. COMPANION RECRUITMENT THROUGH REPUTATION

Nếu group nổi tiếng:

NPC có thể chủ động xin đi cùng.

Ví dụ apprentice muốn học.

Warrior muốn tham gia.

Merchant muốn thuê nhóm.

Nhưng fame cũng thu hút:

enemy.

political attention.

---

<a id="m11-t029-s142"></a>

## 91. FACTION LOYALTY

Companion có thể trung thành với faction ngoài party.

Ví dụ:

Lạc Nghi đi cùng player nhưng vẫn là người Phong Châu.

Nếu party chống Phong Châu:

conflict xuất hiện.

Không có luật:

> recruited companion = faction loyalty removed.

---

<a id="m11-t029-s143"></a>

## 92. FAMILY LOYALTY

Family thường mạnh hơn party ở giai đoạn đầu.

Một companion mới không hy sinh cha mẹ cho player chỉ vì 10 affection.

Long-term shared history có thể thay đổi priority.

Nhưng không dễ.

---

<a id="m11-t029-s144"></a>

## 93. PARTY VS PERSONAL OBLIGATION

Đây là một trong những conflict engine chính.

Ví dụ:

Party cần đi Bắc Lộ.

Companion nhận tin:

mẹ bệnh ở Tam Giang.

Họ có thể yêu cầu:

quay về.

Player có thể:

đồng ý;

từ chối;

chia nhóm;

tìm người khác giúp.

Relationship consequences rất tự nhiên.

---

<a id="m11-t029-s145"></a>

## 94. PARTY SPLIT

Cho phép nhưng hạn chế.

Player có thể tạm chia:

- companion về giải quyết việc;
- scout đi trước;
- member escort NPC.

Không thiết kế tactical control hai party song song liên tục ở GDD hiện tại.

Offscreen Task Engine xử lý group phụ.

---

<a id="m11-t029-s146"></a>

## 95. OFFSCREEN TASK

Có thể giao companion không active:

- đưa thư;
- mua vật liệu;
- chăm người;
- học mentor;
- bảo vệ household.

Outcome dựa trên Skill + Context.

Không phải guaranteed success.

---

<a id="m11-t029-s147"></a>

## 96. OFFSCREEN TASK KHÔNG PHẢI MOBILE TIMER

Không:

> “12 giờ nữa quay lại nhận reward.”

Nó chạy theo world time và event system.

Character thật sự di chuyển.

Có thể gặp biến cố.

---

<a id="m11-t029-s148"></a>

## 97. COMPANION CAN FAIL OFFSCREEN

Ví dụ gửi companion mang thư.

Họ:

- tới nơi;
- bị trì hoãn;
- mất thư;
- bị bắt;
- tự quyết định thay đổi route.

Story mới có thể sinh.

---

<a id="m11-t029-s149"></a>

## 98. PARTY INFORMATION SHARING

Không phải companion tự động biết mọi thứ player biết.

Knowledge phải được:

- chứng kiến;
- player kể;
- companion khác kể.

Player có thể giữ secret khỏi party.

---

<a id="m11-t029-s150"></a>

## 99. PLAYER SECRET FROM COMPANION

Ví dụ player biết:

cha của companion là hung thủ.

Không nói.

Nếu companion phát hiện:

Trust impact phụ thuộc:

tại sao player giấu;

bao lâu;

consequence.

Knowledge Graph rất quan trọng.

---

<a id="m11-t029-s151"></a>

## 100. COMPANION SECRET FROM PLAYER

Companion cũng có quyền giấu.

High Trust không auto reveal mọi secret.

Một số chỉ lộ khi:

relevant event.

personal choice.

discovery.

---

<a id="m11-t029-s152"></a>

## 101. LIE INSIDE PARTY

Player có thể nói dối companion.

Companion cũng có thể nói dối player.

Deception được xử lý bằng cùng hệ Skill/Psychology.

Nếu bị phát hiện:

Shared Memory mạnh.

---

<a id="m11-t029-s153"></a>

## 102. PARTY PLANNING

Trước mission quan trọng, party có thể có Planning Phase.

Companions đề xuất dựa trên expertise.

Ví dụ:

Liệp Nhân:

đường vòng.

Sứ Giả:

thương lượng.

Xảo Nhân:

phá cơ cấu.

Dược Nhân:

chuẩn bị thuốc.

Player chọn approach.

---

<a id="m11-t029-s154"></a>

## 103. PLAN BUY-IN

Nếu companion góp ý nhưng player liên tục bỏ qua:

không nhất thiết giảm affection.

Nhưng Respect for Leadership có thể giảm nếu plan player thất bại.

Nếu player luôn đúng:

respect tăng.

---

<a id="m11-t029-s155"></a>

## 104. COMPANION ADVICE QUALITY

Advice không luôn đúng.

Một expert vẫn có:

limited information.

bias.

trauma.

belief.

Ví dụ Vu Chúc giải thích anomaly tâm linh.

Dược Nhân phản đối.

Player phải tự quyết định.

---

<a id="m11-t029-s156"></a>

## 105. COMPANION DISAGREEMENT IS INFORMATION

Game không đánh dấu:

> “Miên đúng.”

Player nghe nhiều perspective.

Điều này rất phù hợp game tập trung nội dung.

---

<a id="m11-t029-s157"></a>

## 106. PARTY COMBAT COHESION

Trong combat sau này, shared history có thể ảnh hưởng nhẹ.

Ví dụ:

hai character Tactical Trust cao:

- Assist dễ hơn;
- reaction bảo vệ phối hợp hơn;
- formation ổn định.

Không cho raw damage bonus lớn.

---

<a id="m11-t029-s158"></a>

## 107. COMBAT COMMAND

Player điều khiển tactical action.

Nhưng Leadership Class có thể mở:

- order;
- coordinated reaction;
- formation.

Party system cung cấp dữ liệu:

Trust.

Cohesion.

Training.

Combat system sử dụng.

---

<a id="m11-t029-s159"></a>

## 108. FRIENDLY FIRE & TRUST

Nếu player liên tục dùng tactics gây nguy hiểm cho companion:

Tactical Trust giảm.

Ví dụ:

bắn qua vị trí đồng đội.

đốt khu vực họ đang đứng.

Sau nhiều lần:

họ có thể từ chối tactic tương tự.

---

<a id="m11-t029-s160"></a>

## 109. RESCUE MEMORY

Cứu companion khỏi death tạo:

Obligation.

Attachment.

Nhưng không auto romance.

Một character pride cao có thể thậm chí:

resent vì phải được cứu.

Interpretation quan trọng.

---

<a id="m11-t029-s161"></a>

## 110. COMPANION WOUND

Permanent injury ảnh hưởng cả nhóm.

Party có thể:

- tiếp tục;
- quay về;
- tìm healer;
- để companion nghỉ.

Nếu player ép đi:

consequence.

---

<a id="m11-t029-s162"></a>

## 111. CAREER VS ADVENTURE

Companion không thể đi cùng player mãi mà không ảnh hưởng career.

Một Master Smith đi phiêu lưu 3 năm:

workshop chịu ảnh hưởng.

Apprentice có thể rời.

Career opportunity mất.

NPC phải cân nhắc.

---

<a id="m11-t029-s163"></a>

## 112. CAREER LEAVE

Một số companion agreement bao gồm:

> chỉ đi hai tháng.

Nếu player muốn họ đi lâu:

phải giải quyết career obligations.

---

<a id="m11-t029-s164"></a>

## 113. COMPANION AGE

Một companion 55 tuổi:

khác người 18 tuổi.

Người lớn tuổi có:

- knowledge;
- reputation;
- obligations;
- health risk.

Không nên game chỉ recruit người trẻ combat-ready.

---

<a id="m11-t029-s165"></a>

## 114. CHILD COMPANIONS

Không dùng trẻ em như combat party member mặc định.

Child có thể:

- travel temporarily;
- story role;
- apprentice.

Combat participation bị giới hạn tùy tuổi và context.

---

<a id="m11-t029-s166"></a>

## 115. ELDER COMPANIONS

Có thể rất mạnh ngoài combat.

Ví dụ elder ritualist.

Không chạy nhanh.

Nhưng:

knowledge cực cao.

Social authority mạnh.

Party phải bảo vệ họ.

Đây là interesting trade-off.

---

<a id="m11-t029-s167"></a>

## 116. NON-COMBAT COMPANION

Hoàn toàn hợp lệ.

Ví dụ:

interpreter.

historian.

craft master.

Không cần ép mọi người thành fighter.

Khi combat:

họ có thể:

- hỗ trợ;
- tránh;
- retreat;
- thực hiện objective phụ.

---

<a id="m11-t029-s168"></a>

## 117. COMPANION POWER BALANCE

Không balance tất cả NPC về combat.

Balance dựa trên:

**Total Opportunity Value.**

Một Sứ Giả già yếu có thể mở political route mà warrior không có.

---

<a id="m11-t029-s169"></a>

## 118. COMPANION AVAILABILITY

NPC không luôn available.

Có calendar và obligation.

Ví dụ fisherman không muốn rời vào mùa cá.

Ritual keeper không rời trước lễ.

Career tạo availability thật.

---

<a id="m11-t029-s170"></a>

## 119. RECRUITMENT COMPETITION

Hai NPC có thể không chịu đi cùng nhau.

Không phải cấm cứng ngay.

Player có thể:

hòa giải.

chọn một.

đợi conflict giảm.

Hoặc chấp nhận party tension.

---

<a id="m11-t029-s171"></a>

## 120. EXTREME INCOMPATIBILITY

Một số trường hợp thực sự không thể.

Ví dụ:

A vừa giết cha của B.

Không thể:

> Persuasion 20 → cùng party.

Cần major reconciliation arc, nếu còn khả thi.

---

<a id="m11-t029-s172"></a>

## 121. COMPANION BETRAYAL

Không random để tạo shock.

Cần causal chain:

- divided loyalty;
- fear;
- ideology;
- coercion;
- secret;
- low trust;
- high stakes.

Player có thể nhìn thấy dấu hiệu nếu đủ Knowledge/Relationship.

---

<a id="m11-t029-s173"></a>

## 122. BETRAYAL KHÔNG LUÔN EVIL

Companion có thể báo tin cho family vì:

> họ tin player sắp gây thảm họa.

Trong góc nhìn của họ:

đó là trách nhiệm.

---

<a id="m11-t029-s174"></a>

## 123. COMPANION AS FUTURE ENEMY

Một companion rời party có thể:

- trở thành faction leader;
- rival;
- enemy commander.

Shared history khiến encounter mạnh hơn.

---

<a id="m11-t029-s175"></a>

## 124. ENEMY AS FUTURE COMPANION

Ngược lại:

enemy có thể trở thành ally nếu world state thay đổi.

Không có permanent label:

`Hostile NPC`.

Relationship có history.

---

<a id="m11-t029-s176"></a>

## 125. CAPTURED ENEMY

Player có thể:

- thả;
- tuyển;
- đổi tù binh;
- giữ.

Tuyển ngay người vừa bị đánh bại rất khó.

Cần motive và trust development.

---

<a id="m11-t029-s177"></a>

## 126. PARTY RECRUITMENT FROM FAMILY

Sibling có thể đi cùng.

Nhưng family dynamics phức tạp hơn generic companion.

Ví dụ cha phản đối cả hai con rời nhà.

Party choice ảnh hưởng household.

---

<a id="m11-t029-s178"></a>

## 127. PARTY WITH SPOUSE

Spouse có thể là companion.

Nhưng romance không auto tạo combat compatibility.

Có thể gây:

protectiveness.

risk avoidance.

special relationship.

---

<a id="m11-t029-s179"></a>

## 128. PARENT/CHILD PARTY

Qua campaign dài:

con trưởng thành có thể đi cùng cha/mẹ.

Đây là một trong những relationship đặc biệt nhất.

Có thể:

mentor;

conflict thế hệ;

succession.

---

<a id="m11-t029-s180"></a>

## 129. APPRENTICE COMPANION

Master player có thể đưa apprentice theo.

Apprentice học nhanh từ real challenge.

Nhưng player chịu responsibility.

Nếu apprentice chết:

Psychological impact lớn.

---

<a id="m11-t029-s181"></a>

## 130. COMPANION LEGACY

Nếu companion trở thành Master:

họ cũng để lại:

students;

techniques;

family;

story.

Game history không chỉ xoay quanh player.

---

<a id="m11-t029-s182"></a>

## 131. COMPANION MYTH FORMATION

Một companion có thể nổi tiếng hơn player.

Ví dụ party chiến thắng.

Rumor lại ghi công cho Liệp Nhân companion.

Player có thể:

- chấp nhận;
- sửa;
- ghen;
- tận dụng.

Historical memory không công bằng tuyệt đối.

---

<a id="m11-t029-s183"></a>

## 132. PARTY MYTH

Qua nhiều năm, nhóm có thể trở thành một truyền thuyết tập thể.

Reality:

4 người.

Myth:

> “Mười hai chiến binh của Tam Giang.”

Myth Formation có thể bóp méo party size, action và motive.

---

<a id="m11-t029-s184"></a>

## 133. COMPANION UI

Character panel của companion hiển thị những gì player **biết**, không phải toàn bộ dữ liệu thật.

Ví dụ:

<a id="m11-t029-s185"></a>

#### Relationship

Tin tưởng bạn.

Kính trọng năng lực sông nước của bạn.

Không đồng tình với cách bạn đối xử với Phong Châu.

Không hiển thị:

`Trust 72`.

---

<a id="m11-t029-s186"></a>

## 134. KNOWN / UNKNOWN INFO

Player có thể biết:

Profession.

Class Reputation.

Skills đã chứng kiến.

Một số Goals.

Nhưng không biết:

Hidden Need.

Secret.

Unknown Relationships.

Cho tới khi discover.

---

<a id="m11-t029-s187"></a>

## 135. PARTY SCREEN

UI nên tập trung:

- active members;
- condition;
- current goal;
- assigned role;
- current concern;
- relationship summary;
- known expertise.

Không biến thành spreadsheet social quá nặng.

Deep data có thể nằm trong Chronicle.

---

<a id="m11-t029-s188"></a>

## 136. RELATIONSHIP UI

Thay vì heart bar:

> **Mân tin Miên trong việc chữa bệnh nhưng vẫn không thoải mái với niềm tin về độc dược của cô ấy.**

System có số phía sau.

Player nhận natural-language interpretation.

---

<a id="m11-t029-s189"></a>

## 137. PARTY CHRONICLE

Có thể xem:

> “Những người đã từng đồng hành.”

Mỗi người:

- ngày gặp;
- lý do gia nhập;
- thời gian đi cùng;
- major shared events;
- lý do rời;
- tình trạng hiện tại nếu biết.

Một người chết không bị xóa khỏi UI.

---

<a id="m11-t029-s190"></a>

## 138. COMPANION DISCOVERY

Không cần đặt mọi companion ở hub.

Player có thể gặp:

- trong dungeon;
- qua job;
- enemy camp;
- migration;
- funeral;
- trade voyage;
- war.

World tự cung cấp con người.

---

<a id="m11-t029-s191"></a>

## 139. NO “COMPANION QUEST MARKER”

Một người không đeo nhãn:

> Recruitable.

Player có thể bỏ qua một character tuyệt vời mà không biết.

Replayability tăng.

---

<a id="m11-t029-s192"></a>

## 140. COMPANION GENERATION QUALITY GATE

Một companion candidate chỉ đạt Tier A khi có:

- nguồn gốc rõ;
- ít nhất 3 meaningful relationships;
- profession;
- capability;
- current goal;
- long-term goal;
- moral boundaries;
- contradiction;
- Signature Passive;
- một unresolved thread;
- lý do có thể đi cùng;
- lý do có thể rời;
- ít nhất một cách bất đồng với player;
- future arc độc lập.

---

<a id="m11-t029-s193"></a>

## 141. PARTY QUALITY GATE

Một party 4 người tốt không chỉ cần Skill diversity.

Narrative Director kiểm tra:

<a id="m11-t029-s194"></a>

#### Capability Diversity

Có các thế mạnh khác nhau.

<a id="m11-t029-s195"></a>

#### Personality Diversity

Không phải bốn người cùng temperament.

<a id="m11-t029-s196"></a>

#### Relationship Potential

Có khả năng tạo bond/conflict.

<a id="m11-t029-s197"></a>

#### World Connection

Party kết nối với nhiều community.

<a id="m11-t029-s198"></a>

#### Story Capacity

Mỗi người mang thread riêng.

---

<a id="m11-t029-s199"></a>

## 142. PARTY RANDOMNESS

Vì player character random, companion pool cũng không được luôn giống nhau.

World Seed và Life Simulation thay đổi:

ai còn sống;

ai đang available;

ai ghét player;

ai đã kết hôn;

ai đã trở thành leader.

Do đó không phải playthrough nào cũng recruit được cùng người.

---

<a id="m11-t029-s200"></a>

## 143. AUTHORED CORE NPC

Game vẫn có thể có các character được viết tay quan trọng cho historical story.

Nhưng họ không nhất thiết luôn recruitable.

Ví dụ một historical/game-canon figure có thể:

- ally;
- mentor;
- temporary companion;
- faction leader.

Không cần biến tất cả nhân vật nổi tiếng thành party member.

---

<a id="m11-t029-s201"></a>

## 144. COMPANION AND HISTORICAL ANCHORS

Ví dụ khi Âu Lạc hình thành:

Companion Thượng Âu có thể:

ủng hộ.

Companion Phong Châu có thể:

phản đối.

Một companion khác chỉ quan tâm family.

Historical event đi xuyên qua party.

---

<a id="m11-t029-s202"></a>

## 145. PARTY FRACTURE EVENT

Một historical crisis đủ mạnh có thể xé party.

Ví dụ:

Phong Châu vs Thục Phán.

Hai companion ở hai phía.

Player không nhất thiết giữ được cả hai.

Đây không phải failure design.

Nó là narrative consequence.

---

<a id="m11-t029-s203"></a>

## 146. PARTY REUNION

Nhiều năm sau:

hai người có thể gặp lại.

Một người đã thay đổi.

Shared Memory vẫn còn.

Đây là payoff campaign dài.

---

<a id="m11-t029-s204"></a>

## 147. PARTY SESSION LOOP

Gameplay party thường đi theo:

**Chuẩn bị**

→ chọn người phù hợp.

**Travel**

→ expertise và relationship tác động.

**Encounter**

→ party xử lý problem.

**Aftermath**

→ injury, loot, opinion.

**Camp**

→ conversation, healing, reflection.

**Downtime**

→ companion trở về nghề/family.

Party không chỉ tồn tại trong combat.

---

<a id="m11-t029-s205"></a>

## 148. NO PERFECT PARTY

Một trong những mục tiêu design quan trọng:

> Không tồn tại party composition tốt nhất cho toàn game.

Martial party:

mạnh combat.

nhưng thiếu social/knowledge.

Social party:

giải quyết conflict tốt.

nhưng gặp wilderness khó.

Craft party:

mở environmental solution.

Một party luôn có blind spot.

---

<a id="m11-t029-s206"></a>

## 149. COMPANION DEPENDENCY

Không nên có character bắt buộc cho main story.

Nếu một companion chết:

story phải tìm alternative route.

NPC khác.

Knowledge khác.

Investigation khác.

Main campaign không được fail chỉ vì:

> required companion died.

---

<a id="m11-t029-s207"></a>

## 150. REPLACEMENT KHÔNG ĐỒNG NGHĨA TƯƠNG ĐƯƠNG

Nếu Master Healer chết:

player có thể tìm healer khác.

Nhưng:

skill khác.

personality khác.

knowledge khác.

Không có “healer slot replacement”.

---

<a id="m11-t029-s208"></a>

## 151. PARTY SCARCITY

Ở một số thời điểm:

player có thể chỉ có 1–2 companion available.

Game phải vẫn chơi được.

Encounter scaling chủ yếu bằng:

approach.

Không đơn giản enemy count theo party size.

---

<a id="m11-t029-s209"></a>

## 152. SOLO PLAY

Player có thể cố đi một mình.

Hợp lệ.

Ưu:

stealth;

resource;

decision autonomy.

Nhược:

skill coverage thấp;

combat risk;

không ai cứu khi downed;

ít perspective.

Không cấm.

---

<a id="m11-t029-s210"></a>

## 153. LONE WANDERER KHÔNG ĐƯỢC BUFF MIỄN PHÍ

Không có:

> Solo +30% damage.

Lợi ích solo đến tự nhiên từ:

mobility;

stealth;

less supply.

Party lợi ích:

capability;

safety;

relationships.

---

<a id="m11-t029-s211"></a>

## 154. COMPANION ECONOMY

Long-term companion cần sống.

Một số không cần tiền vì:

friendship/family.

Một số cần share.

Một professional guide có thể yêu cầu compensation.

Một noble/faction agent có thể được faction trả.

Motive quyết compensation.

---

<a id="m11-t029-s212"></a>

## 155. NO UNIVERSAL GOLD SALARY

Thế giới có barter/resource/favor.

Compensation có thể:

- gạo;
- đồng;
- đồ;
- share loot;
- favor;
- training;
- political support.

---

<a id="m11-t029-s213"></a>

## 156. PARTY PROPERTY

Nếu party lâu dài, có thể hình thành tài sản chung:

- thuyền;
- cart nếu hợp setting sau này;
- workshop;
- safehouse;
- storage.

Ownership phải có record.

Nếu party tan:

ai sở hữu?

Có thể thành story.

---

<a id="m11-t029-s214"></a>

## 157. PARTY BASE KHÔNG BẮT BUỘC

Game không cần castle-base.

Player có thể sống:

- nhà family;
- nhà companion;
- rented space;
- thuyền;
- settlement.

Content-first nên tránh base building quá lớn nếu không cần.

---

<a id="m11-t029-s215"></a>

## 158. SOCIAL SAFEHOUSE

Một companion có thể cho party ở nhờ tại quê.

Điều này tạo lợi thế local.

Nhưng nếu player gây tai tiếng:

family bị ảnh hưởng.

---

<a id="m11-t029-s216"></a>

## 159. COMPANION FAMILY REACTION

Khi NPC đi cùng:

family có opinion về player.

Nếu họ trở về:

injured.

giàu.

nổi tiếng.

hoặc chết.

Family reaction thay đổi.

Player không recruit “một cá nhân cô lập”.

---

<a id="m11-t029-s217"></a>

## 160. COMPANION RESPONSIBILITY

Nếu player dẫn con của một family đi rồi họ chết:

family có thể blame player.

Dù death không phải lỗi player.

Perception != objective truth.

---

<a id="m11-t029-s218"></a>

## 161. PARTY SOCIAL FOOTPRINT

Một group đi qua settlement tạo ấn tượng khác solo.

Bốn người vũ trang:

có thể gây sợ.

Một elder + healer + messenger:

được nhìn khác.

Party composition ảnh hưởng initial social context.

---

<a id="m11-t029-s219"></a>

## 162. DISGUISE / LOW PROFILE

Một Trinh Binh có thể giúp party giảm footprint.

Nhưng một companion quá nổi tiếng có thể phá stealth social.

Reputation cũng có downside.

---

<a id="m11-t029-s220"></a>

## 163. COMPANION FAME

Một companion nổi tiếng có thể:

mở cửa.

hoặc:

thu hút crowd/enemy.

Không phải fame luôn buff.

---

<a id="m11-t029-s221"></a>

## 164. PARTY KNOWLEDGE POOL

Trong lúc active party, Knowledge có thể chia sẻ.

Nhưng cần communication.

Ví dụ companion biết road.

Nếu unconscious:

party không tự động có knowledge trừ đã học.

---

<a id="m11-t029-s222"></a>

## 165. LEARNING ROUTE FROM COMPANION

Đi một route nhiều lần cùng guide:

player dần tăng Knowledge.

Sau đó có thể đi không cần guide.

Party tạo progression thực.

---

<a id="m11-t029-s223"></a>

## 166. COMPANION DEPENDENCE CAN DIMINISH

Early game:

player cần Mân dẫn sông.

Later:

player học đủ.

Mân không còn cần vì utility đó.

Nhưng relationship có thể khiến vẫn đi cùng.

Điều này tránh companion bị coi là “tool”.

---

<a id="m11-t029-s224"></a>

## 167. PERSONAL VALUE BEYOND UTILITY

Game phải khiến player có thể giữ một companion dù không optimal.

Lý do:

- friendship;
- history;
- family;
- narrative attachment.

Nếu người chơi chỉ xem companion như build, hệ thống character chưa đạt.

---

<a id="m11-t029-s225"></a>

## 168. COMPANION RETIREMENT

Một companion có thể nói:

> “Ta đã đi đủ rồi.”

Reasons:

age;

family;

career;

trauma;

completed goal.

Player có thể phản đối nhưng không luôn giữ được.

Retirement là natural ending.

---

<a id="m11-t029-s226"></a>

## 169. RETIRED COMPANION CONTINUES WORLD ROLE

Họ có thể:

mentor;

shopkeeper;

leader;

parent;

politician.

Player vẫn gặp.

Không phải companion rời = content biến mất.

---

<a id="m11-t029-s227"></a>

## 170. RETURN OF RETIRED COMPANION

Trong crisis cực lớn:

một retired companion có thể quay lại tạm thời.

Payoff emotional rất cao nếu causal.

Không dùng fan-service ngẫu nhiên.

---

<a id="m11-t029-s228"></a>

## 171. COMPANION AGING

Campaign 20+ năm:

visual description.

health.

career.

psychology.

relationship.

đều thay đổi.

Một party không giữ cùng hình dạng suốt game.

---

<a id="m11-t029-s229"></a>

## 172. GENERATIONAL PARTY

Late game có thể có:

player cũ/già;

companion trẻ;

con của companion;

apprentice.

Group trở thành liên thế hệ.

Điều này rất phù hợp chủ đề lịch sử.

---

<a id="m11-t029-s230"></a>

## 173. SUCCESSION OF COMPANION ROLE

Ví dụ healer già nghỉ.

Apprentice của họ có thể đi cùng.

Không giống thầy.

Nhưng mang legacy.

---

<a id="m11-t029-s231"></a>

## 174. PARTY LEGACY

Party có thể để lại:

- route;
- workshop;
- school;
- faction;
- story;
- settlement;
- ritual.

Không cần thành anh hùng chiến tranh.

---

<a id="m11-t029-s232"></a>

## 175. ENDGAME PARTY OUTCOME

Cuối campaign, game có thể trình bày:

**Mân**

trở về Tam Giang, dạy bốn người tìm dấu.

**Miên**

mở nơi chữa bệnh tại Loa Vực.

**Vũ**

không bao giờ săn linh vật nữa.

**Nghi**

trở thành người thương lượng giữa hai faction.

Party tan.

Nhưng mỗi người có hậu truyện.

---

<a id="m11-t029-s233"></a>

## 176. HISTORICAL MEMORY OF PARTY

50 năm sau:

NPC có thể nhắc.

Một số đúng.

Một số sai.

Người chơi thấy:

nhóm mình từng điều khiển đã trở thành historical object.

---

<a id="m11-t029-s234"></a>

## 177. INTERACTION WITH COMBAT SYSTEM

Combat module sau sẽ lấy từ Party:

- active members;
- positioning;
- tactical trust;
- leadership;
- morale;
- injuries;
- class techniques;
- equipment.

Combat không tự quản companion personality.

---

<a id="m11-t029-s235"></a>

## 178. INTERACTION WITH NARRATIVE ENGINE

Party tạo:

Shared Events.

Relationship Event.

Conflict Thread.

Departure Thread.

Death Thread.

Romance.

Mentorship.

Betrayal.

Narrative Engine coi party là một Social Graph mật độ cao.

---

<a id="m11-t029-s236"></a>

## 179. INTERACTION WITH PROFESSION

Companion vẫn giữ nghề.

Downtime ảnh hưởng career.

Player có thể tận dụng profession.

Nhưng việc đi cùng quá lâu có opportunity cost.

---

<a id="m11-t029-s237"></a>

## 180. INTERACTION WITH CLASS

Class Emergence chạy độc lập từng member.

Party composition tạo Exposure.

Một companion có thể học Class mới vì trải nghiệm cùng player.

---

<a id="m11-t029-s238"></a>

## 181. INTERACTION WITH REPUTATION

Player reputation lan tới companion.

Companion reputation lan tới player.

Không hoàn toàn.

Ví dụ:

> “Ta không tin ngươi, nhưng nếu Vạc Tòng đi cùng ngươi thì ta sẽ nghe.”

Social Graph tạo access.

---

<a id="m11-t029-s239"></a>

## 182. INTERACTION WITH FACTION

Party member có faction allegiance.

Faction có thể yêu cầu họ rời.

Party có thể bị coi là representative dù không muốn.

---

<a id="m11-t029-s240"></a>

## 183. INTERACTION WITH DEATH & LEGACY

Death thay đổi:

party;

family;

world;

successor.

Đây là lý do companion death phải persistent.

---

<a id="m11-t029-s241"></a>

## 184. PARTY SYSTEM QUALITY GATE

Hệ Party đạt yêu cầu nếu:

1. Companion tồn tại độc lập với player.
2. Recruitment luôn có causal reason.
3. Companion có thể hợp lý từ chối đi cùng.
4. Companion có thể rời mà game vẫn tiếp tục.
5. Relationships không chỉ là một affection bar.
6. Companion có relationships với nhau.
7. Major decision tạo reactions khác nhau.
8. Player không thể persuasion vượt mọi moral boundary.
9. Companion career tiếp tục tồn tại.
10. Offscreen companion tiếp tục sống.
11. Death có hậu quả lên family/world.
12. Party composition thay đổi gameplay ngoài combat.
13. Non-combat companion hữu ích thật.
14. Party không yêu cầu Tank/DPS/Healer.
15. Companion progression không hoàn toàn do player điều khiển.
16. Party có thể chia rẽ vì historical events.
17. Friendship sâu ngang romance.
18. Companion có thể trở thành successor.
19. NPC cũ có thể quay lại với cuộc đời đã thay đổi.
20. Party có thể trở thành một phần lịch sử save.

---

<a id="m11-t029-s242"></a>

## 185. DEFERRED TO PRODUCTION

Giai đoạn production mới cần khóa sâu:

- số lượng authored companions;
- exact relationship thresholds;
- camp dialogue volume;
- exact party cohesion formulas;
- animation banter;
- companion tactical AI nếu không direct control;
- exact food/fatigue balancing;
- romance content cụ thể;
- toàn bộ companion personal thread;
- voice/audio;
- offscreen task resolution numbers.

---

<a id="m11-t029-s243"></a>

## 186. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

Active Party mặc định gồm **4 nhân vật**, trong đó có player.

Companion không phải generic recruit.

Bất kỳ character phù hợp nào về lý thuyết có thể trở thành companion.

Recruitment dựa trên relationship + motive + obligation.

Persuasion không thể thay causal requirement.

Companion có thể chỉ đi tạm thời.

Không có permanent follower mặc định.

Companion giữ property và equipment cá nhân.

Loot distribution có social consequence.

Companions có relationship với nhau.

Party có Shared Memories.

Party có Cohesion nhưng không phải happiness bar.

Companion có Personal Goals độc lập.

Companion có Moral Boundaries.

Player direct-control combat không loại bỏ autonomy narrative.

Companion có thể từ chối extreme order.

Player không mặc định là social leader.

Context Expert có thể dẫn party trong lĩnh vực của họ.

Party Role khác Class.

Companion progression dùng cùng hệ Attributes/Skills/Class.

Companion có thể học lẫn nhau.

Offscreen companion tiếp tục simulation.

Companion có thể rời, quay lại, già đi, kết hôn và có con.

Romance là emergent relationship, không gift meter.

Friendship và mentorship phải sâu ngang romance.

Faction và family loyalty không biến mất khi recruit.

Historical event có thể chia rẽ party.

Companion có thể phản bội nếu causal chain hợp lý.

Companion có thể trở thành enemy hoặc successor.

Player death không bắt buộc game over.

Party không cần combat-role composition chuẩn.

Non-combat companion phải có giá trị thực.

Companion death là permanent world event nếu mode sử dụng permanent death.

Party có Chronicle.

Party có thể tạo Legacy và Myth riêng.

---

<a id="m11-t029-s244"></a>

## 187. CORE PARTY FANTASY

Party của game không nên khiến player nghĩ:

> “Tôi cần kiếm một healer tốt hơn.”

Mà nên khiến player nghĩ:

> “Miên muốn trở về chăm em gái. Tôi có thực sự muốn yêu cầu cô ấy tiếp tục đi cùng không?”

Không phải:

> “Vũ có DPS thấp hơn companion mới.”

Mà:

> “Vũ đã đi với tôi mười hai năm. Nhưng giờ ông ấy già và con trai đang cần ông ấy trở về.”

Không phải:

> “Nghi mất loyalty.”

Mà:

> “Nghi tin rằng lựa chọn của tôi sẽ phá hủy Phong Châu. Tôi hiểu tại sao anh ấy muốn rời đi.”

Đây chính là mục tiêu.

Party không phải bộ công cụ mà player thu thập.

Party là:

> **những con người đã lựa chọn cùng sống một phần cuộc đời với nhau.**

Và đôi khi:

> **lựa chọn đó kết thúc.**

---

<a id="m11-t029-s245"></a>

## 188. PLAYER EXPERIENCE ARC

Một playthrough có thể bắt đầu:

**17 tuổi**

Mân đi một mình.

↓

Gặp một healer.

↓

Hai người đi cùng một mùa.

↓

Gặp một hunter.

↓

Ba người sống sót khỏi một trận lũ.

↓

Một người rời về nhà.

↓

Người khác gia nhập.

↓

Nhiều năm sau nhóm cũ gặp lại.

↓

Một companion chết.

↓

Apprentice của họ xuất hiện.

↓

Player già đi.

↓

Player chết.

↓

Một companion tiếp tục campaign.

↓

Những người sống sót chứng kiến Âu Lạc hình thành.

Khi đó người chơi không chỉ nhớ:

> “build của tôi”.

Họ nhớ:

> **những người từng đi cùng mình.**

Đó phải là thước đo thành công cao nhất của Party & Companion System.

Sau module này, bước hợp lý nhất là **Combat System**. Ta đã biết ai chiến đấu, họ có Attributes/Skills/Class gì, party phối hợp ra sao và injury/death có ý nghĩa gì. Combat giờ có thể được thiết kế đúng triết lý game thay vì độc lập: **turn-based tactical, ít trận nhưng mỗi trận nguy hiểm, vũ khí thời Đông Sơn có hành vi riêng, địa hình quan trọng, morale–injury–formation quan trọng hơn việc spam skill và đánh tụt một thanh HP khổng lồ.**

<!-- SOURCE T029 END -->

