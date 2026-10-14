# Module 19 — Tín ngưỡng, thần thoại, Mạch Lạc và supernatural

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T045](../../source/conversation.md#t045)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Tách internal ontology và điều nhân vật biết. Mạch là subsystem có điều kiện/giới hạn; không được dùng để hợp thức hóa teleport, resurrection hay thay hung thủ tùy ý.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Belief, ritual authority, materials, sites, oath history, environment và Mạch state. |
| Đầu ra / bên sở hữu | Ritual proposals, permitted effects và knowledge-limited observations; Belief/Mạch owners. |
| Đọc cùng | 02, 05, 16, 20, 25–27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Cùng hiện tượng có diễn giải khác nhưng committed physical facts không đổi theo narration. |
| Việc chưa được chứng minh | Metaphysical content specifics, cultural review và ambiguity presentation. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T045 BEGIN -->

Đây là module quan trọng nhất về **bản sắc thế giới**. Nếu làm sai, toàn bộ hệ thống đã xây rất dễ trượt thành “CRPG fantasy phương Tây nhưng đổi tên thần, class và quái vật sang tiếng Việt”. Vì vậy tôi sẽ khóa khá chặt cả **bản thể học của thế giới, niềm tin xã hội, cơ chế nghi lễ, Mạch Lạc, thần linh, truyền thuyết, dream, omen và giới hạn supernatural**. 

<a id="m19-t045-s001"></a>

## GDD MODULE 19  
<a id="m19-t045-s002"></a>

## RELIGION, BELIEF, MYTHOLOGY, MẠCH LẠC & SUPERNATURAL SYSTEM

---

<a id="m19-t045-s003"></a>

## PART I — DESIGN PHILOSOPHY

<a id="m19-t045-s004"></a>

### 1. MỤC TIÊU

Hệ thống tín ngưỡng và supernatural phải đạt đồng thời sáu mục tiêu:

1. Mang bản sắc Việt cổ rõ ràng về **cách con người nhìn thế giới**, không chỉ thay tên spell.
2. Không tạo hệ “mana–phép thuật–pháp sư” truyền thống.
3. Cho phép supernatural có tác động gameplay thật.
4. Giữ được sự mơ hồ giữa:
   - tự nhiên;
   - tâm lý;
   - xã hội;
   - tín ngưỡng;
   - Mạch Lạc.
5. Biến nghi lễ, tổ tiên, lời thề, địa điểm thiêng và truyền thuyết thành gameplay.
6. Không khẳng định các yếu tố game-canon là sự thật lịch sử.

---

<a id="m19-t045-s005"></a>

## 2. BA LỚP PHẢI LUÔN TÁCH BIỆT

Mọi nội dung tín ngưỡng trong Content Database phải thuộc ít nhất một trong ba lớp:

<a id="m19-t045-s006"></a>

#### LỚP A — LỊCH SỬ / KHẢO CỔ

Những gì có căn cứ vật chất hoặc nghiên cứu đủ tin cậy.

Ví dụ:

- hình thức mai táng;
- đồ đồng;
- trống;
- môi trường cư trú;
- dấu tích settlement;
- công cụ;
- vật phẩm nghi lễ nếu có căn cứ.

Đây là **Historical Backbone**.

---

<a id="m19-t045-s007"></a>

#### LỚP B — TRUYỀN THỐNG / TRUYỀN THUYẾT

Những câu chuyện được lưu truyền về sau:

- Lạc Long Quân – Âu Cơ;
- Sơn Tinh – Thủy Tinh;
- Thánh Gióng;
- Lang Liêu;
- Kim Quy;
- Mỵ Châu – Trọng Thủy;
- các motif linh vật, núi, nước, tổ tiên.

Game có thể sử dụng.

Nhưng UI/lore không được nói:

> “Đây chắc chắn là lịch sử đã xảy ra như vậy.”

---

<a id="m19-t045-s008"></a>

#### LỚP C — GAME-CANON

Những yếu tố được tạo ra để thế giới game vận hành thống nhất:

- Mạch Lạc;
- Ước Sơn Hà;
- một số Vu Chúc;
- một số linh thể;
- các nghi lễ;
- nhân vật hư cấu;
- cơ chế Mạch;
- sự kiện supernatural.

Game phải tự nhận thức rằng đây là **fictional worldbuilding**.

---

<a id="m19-t045-s009"></a>

## 3. PRINCIPLE OF MYTHIC AMBIGUITY

Game không nên liên tục giải thích:

> “Truyền thuyết thực ra là X.”

Ví dụ không nên biến Sơn Tinh thành:

> “một kỹ sư thủy lợi và tất cả câu chuyện thần linh đều sai.”

Cũng không nên khẳng định:

> “Sơn Tinh chắc chắn là một vị thần bất tử đang sống trên núi.”

Tốt hơn:

Người chơi có thể gặp:

- một lineage tự nhận kế tục Sơn;
- một nghi lễ cổ;
- một địa điểm có Mạch bất thường;
- câu chuyện về một người từng chống lũ;
- dấu tích công trình;
- một experience khó giải thích.

Các lớp này **cùng tồn tại**.

---

<a id="m19-t045-s010"></a>

## 4. SUPERNATURAL PHẢI CÓ GIỚI HẠN

Không được để late-game biến thành:

> chiến binh phóng sét, Vu Chúc ném cầu lửa, healer hồi sinh người chết.

Supernatural chủ yếu tác động:

- perception;
- memory;
- dream;
- fear;
- environment;
- ritual;
- coincidence;
- collective behaviour;
- Mạch;
- một số entity.

Nó mạnh vì:

> **thay đổi cách con người và thế giới liên hệ với nhau.**

Không phải vì DPS.

---

<a id="m19-t045-s011"></a>

## PART II — EPISTEMIC MODEL: THẾ GIỚI BIẾT GÌ?

<a id="m19-t045-s012"></a>

### 5. NĂM TẦNG “SỰ THẬT”

Mỗi hiện tượng quan trọng có thể lưu năm lớp.

<a id="m19-t045-s013"></a>

#### Objective Event

Điều engine biết đã xảy ra.

<a id="m19-t045-s014"></a>

#### Physical Evidence

Thứ còn lại trong thế giới vật chất.

<a id="m19-t045-s015"></a>

#### Witness Experience

Người tham gia thực sự trải nghiệm gì.

<a id="m19-t045-s016"></a>

#### Cultural Interpretation

Họ giải thích chuyện đó bằng belief nào.

<a id="m19-t045-s017"></a>

#### Historical Version

Nhiều năm sau xã hội kể lại ra sao.

---

<a id="m19-t045-s018"></a>

## 6. VÍ DỤ

Objective Event:

Một đợt lũ bất thường xảy ra.

Physical Evidence:

bờ sông vỡ.

Witness Experience:

người dân nghe tiếng trống trước khi nước dâng.

Interpretation A:

Thủy thần nổi giận.

Interpretation B:

người giữ trống đánh sai lễ.

Interpretation C:

mưa trên thượng nguồn.

Historical Version 40 năm sau:

> “Năm ấy nước dâng vì lời thề cũ bị phá.”

Không nhất thiết chỉ một interpretation đúng hoàn toàn.

---

<a id="m19-t045-s019"></a>

## 7. SUPERNATURAL CERTAINTY LEVEL

Các hiện tượng có thể được phân loại nội bộ:

<a id="m19-t045-s020"></a>

#### Natural-Consistent

Có thể giải thích hoàn toàn bằng tự nhiên.

<a id="m19-t045-s021"></a>

#### Ambiguous

Có cả explanation vật lý và supernatural.

<a id="m19-t045-s022"></a>

#### Mạch-Responsive

Engine xác nhận Mạch State có phản ứng.

<a id="m19-t045-s023"></a>

#### Entity-Driven

Một supernatural actor thực sự tham gia.

<a id="m19-t045-s024"></a>

#### Mythic Anchor

Sự kiện đặc biệt mà game cố tình không giải thích hoàn toàn.

Player không được nhìn thấy classification này trực tiếp.

---

<a id="m19-t045-s025"></a>

## PART III — BELIEF SYSTEM CỦA CHARACTER

<a id="m19-t045-s026"></a>

### 8. BELIEF KHÔNG PHẢI MỘT STAT “FAITH”

Mỗi character có một **Belief Profile**.

Bao gồm:

```text
Cosmology
Ancestor Orientation
Ritual Trust
Spirit Belief
Omen Belief
Mạch Interpretation
Skepticism
Tradition Attachment
Personal Experiences
Sacred Obligations
Taboos
```

---

<a id="m19-t045-s027"></a>

## 9. BELIEF KHÔNG BINARY

Không có:

Believer / Atheist.

Một người có thể:

- tin tổ tiên;
- không tin Vu Chúc;
- tin điềm mơ;
- cho rằng linh vật chỉ là symbol;
- vẫn giữ taboo vì tôn trọng cộng đồng.

Belief phải có cấu trúc.

---

<a id="m19-t045-s028"></a>

## 10. BELIEF SOURCE

Belief hình thành từ:

- family;
- region;
- profession;
- childhood;
- mentor;
- ritual experience;
- trauma;
- unexplained event;
- political ideology.

---

<a id="m19-t045-s029"></a>

## 11. BELIEF CONFIDENCE

Mỗi belief có:

Low.

Moderate.

Strong.

Identity-Level.

Một belief Identity-Level bị thách thức có thể gây:

- conflict;
- fear;
- anger;
- identity crisis.

---

<a id="m19-t045-s030"></a>

## 12. BELIEF EVOLUTION

Belief thay đổi chậm.

Một supernatural encounter không tự động:

> skeptic → believer.

Character có thể:

- rationalize;
- reinterpret;
- reject;
- accept partly.

---

<a id="m19-t045-s031"></a>

## 13. BELIEF CONTRADICTION

Character có thể tin những thứ mâu thuẫn.

Ví dụ:

> “Vu Chúc toàn lợi dụng người khác.”

nhưng đồng thời:

> “Không được bỏ tang lễ.”

Con người không phải logic engine hoàn hảo.

---

<a id="m19-t045-s032"></a>

## 14. BELIEF & SOCIAL RELATIONSHIP

Khác belief không tự gây thù.

Conflict phụ thuộc:

Tolerance.

Identity.

Context.

Một skeptic vẫn có thể kính trọng ritualist.

---

<a id="m19-t045-s033"></a>

## 15. BELIEF & FEAR

Một người không tin ma vẫn có thể sợ grave site.

Vì:

cultural conditioning.

trauma.

darkness.

unknown.

Do đó Fear và Belief tách nhau.

---

<a id="m19-t045-s034"></a>

## PART IV — TÍN NGƯỠNG NHƯ MẠNG XÃ HỘI

<a id="m19-t045-s035"></a>

### 16. KHÔNG DÙNG MÔ HÌNH “CHURCH”

Game không nên tự động tạo một tôn giáo tập trung kiểu fantasy medieval.

Thay vào đó có:

- household rites;
- lineage rituals;
- village traditions;
- sacred locations;
- ritual specialists;
- seasonal festivals;
- ancestor practices;
- oath networks.

---

<a id="m19-t045-s036"></a>

## 17. RITUAL COMMUNITY

Một Ritual Community có:

```text
Participants
Sacred Places
Shared Stories
Ritual Calendar
Taboos
Specialists
Objects
Obligations
Local Interpretation
```

Không nhất thiết có leader duy nhất.

---

<a id="m19-t045-s037"></a>

## 18. REGIONAL VARIATION

Cùng một motif có thể khác theo vùng.

Tam Giang:

water-oriented interpretation.

Tản Viên:

mountain/forest.

Đồng Đỏ:

fire/metal/craft symbolism.

Hải Môn:

wind/sea.

Không homogenize toàn bộ Văn Lang thành một religion thống nhất.

---

<a id="m19-t045-s038"></a>

## 19. BELIEF DIFFUSION

Beliefs lan qua:

- marriage;
- migration;
- trade;
- ritual;
- storytellers;
- political integration.

Giống rumor nhưng bền hơn.

---

<a id="m19-t045-s039"></a>

## 20. BELIEF SYNCRETISM

Hai cộng đồng gặp nhau có thể:

- giữ riêng;
- hòa nhập;
- reinterpret;
- tranh chấp.

Ví dụ một linh vật địa phương có thể được đồng nhất dần với một figure rộng hơn.

---

<a id="m19-t045-s040"></a>

## PART V — TỔ TIÊN & DÒNG HỌ

<a id="m19-t045-s041"></a>

### 21. ANCESTOR SYSTEM

Tổ tiên không được xử lý như NPC ghost mặc định.

Ancestor tồn tại qua bốn lớp:

<a id="m19-t045-s042"></a>

#### Biological Ancestor

Quan hệ thực.

<a id="m19-t045-s043"></a>

#### Remembered Ancestor

Người family còn nhớ.

<a id="m19-t045-s044"></a>

#### Ritual Ancestor

Được nhắc trong nghi lễ.

<a id="m19-t045-s045"></a>

#### Mythologized Ancestor

Câu chuyện đã vượt khỏi biography thật.

---

<a id="m19-t045-s046"></a>

## 22. ANCESTOR MEMORY

Family lưu:

- name;
- relation;
- remembered deeds;
- death;
- objects;
- burial place;
- reputation.

Qua generation:

một số detail biến mất.

---

<a id="m19-t045-s047"></a>

## 23. ANCESTOR PRESTIGE

Ancestor nổi tiếng có thể tạo:

- pride;
- expectation;
- legitimacy;
- pressure.

Con cháu không tự nhận stat bonus.

---

<a id="m19-t045-s048"></a>

## 24. ANCESTOR SHAME

Family có thể che:

crime.

betrayal.

illegitimate claim.

Discovery tạo Story Thread.

---

<a id="m19-t045-s049"></a>

## 25. BURIAL LOCATION

Burial có:

- identity;
- family claim;
- ritual significance;
- geographical relation.

Di chuyển/xâm phạm grave có consequence.

---

<a id="m19-t045-s050"></a>

## 26. ANCESTOR RITE

Một rite có thể nhằm:

- remembrance;
- legitimacy;
- grief;
- reconciliation;
- inheritance;
- oath.

Không phải summon ghost.

---

<a id="m19-t045-s051"></a>

## 27. ANCESTOR DREAM

Người đang grief có thể mơ thấy ancestor.

Engine có thể classify:

psychological.

Mạch-responsive.

ambiguous.

Player không luôn biết.

---

<a id="m19-t045-s052"></a>

## PART VI — MẠCH LẠC

<a id="m19-t045-s053"></a>

### 28. ĐỊNH NGHĨA GAME-CANON

Mạch Lạc là:

> **mạng liên kết phi vật chất giữa địa hình, nước, cộng đồng, ký ức, người chết, nghi lễ và những sự kiện để lại dấu ấn sâu.**

Mạch Lạc không phải mana.

Không phải ley line theo fantasy phương Tây.

Không phải năng lượng có thể đo bằng “100 điểm”.

---

<a id="m19-t045-s054"></a>

## 29. MẠCH LẠC KHÔNG TÁCH KHỎI VẬT CHẤT

Một Mạch không “bay trong không khí”.

Nó gắn với:

- river;
- mountain;
- burial;
- settlement;
- battlefield;
- ancient tree;
- workshop;
- road;
- oath place.

---

<a id="m19-t045-s055"></a>

## 30. MẠCH FORMATION

Mạch có thể mạnh khi:

- con người sống lâu tại nơi đó;
- nhiều thế hệ chôn người ở đó;
- ritual lặp lại;
- major event xảy ra;
- collective memory tập trung;
- natural geography đặc biệt.

---

<a id="m19-t045-s056"></a>

## 31. MẠCH STABILITY

Mỗi relevant site có thể có:

<a id="m19-t045-s057"></a>

#### Stable

Quan hệ giữa place–people–memory đang cân bằng.

<a id="m19-t045-s058"></a>

#### Strained

Có xung đột.

<a id="m19-t045-s059"></a>

#### Disturbed

Ritual/social/environmental rupture.

<a id="m19-t045-s060"></a>

#### Fractured

Mạch relation đứt mạnh.

<a id="m19-t045-s061"></a>

#### Reconfigured

Một trật tự mới hình thành.

Không dùng “Good/Corrupted”.

---

<a id="m19-t045-s062"></a>

## 32. TẠI SAO KHÔNG DÙNG “CORRUPTION”

“Corruption” dễ kéo game sang motif fantasy:

> năng lượng đen xấu xa.

Mạch Lạc không có morality intrinsic.

Một Mạch Disturbed có thể đơn giản vì:

- river changed;
- village migrated;
- graves moved;
- old oath no longer fits society.

---

<a id="m19-t045-s063"></a>

## 33. MẠCH PRESSURE

Mạch có thể phản ứng với:

- ecological change;
- mass death;
- oath breaking;
- destruction;
- ritual mismatch;
- forced displacement;
- forgotten burial.

Nhưng không phải mọi construction đều “giận thần”.

---

<a id="m19-t045-s064"></a>

## 34. MẠCH RECONFIGURATION

Đây là mechanic rất quan trọng.

Khi Cổ Loa hình thành:

Mạch cũ không nhất thiết bị phá.

Có thể:

> **được tái cấu trúc thành một mạng mới.**

Điều này phản ánh:

- landscape;
- population;
- political power;
- memory.

---

<a id="m19-t045-s065"></a>

## 35. MẠCH & CỔ LOA

Loa Vực là case lớn nhất.

Early:

farm/wetland Mạch.

Construction:

mass disturbance.

Later:

new roads;

walls;

burials;

workshops;

ritual centers

tạo một Mạch mới.

Game có thể hỏi:

> một thành phố mới có thể tạo ký ức thiêng mới hay không?

---

<a id="m19-t045-s066"></a>

## PART VII — CẢM MẠCH

<a id="m19-t045-s067"></a>

### 36. CẢM MẠCH LÀ NĂNG LỰC HIẾM

Cảm Mạch đã được xác định ngoài Core Attributes.

Phần lớn population:

không có.

Một số:

sensitivity thấp.

Rất ít:

sensitivity cao.

---

<a id="m19-t045-s068"></a>

## 37. CẢM MẠCH KHÔNG PHẢI “MAGIC POWER”

Nó cho phép:

- nhận biết bất thường;
- cảm nhận emotional residue;
- dream linkage;
- ritual sensitivity;
- entity perception.

Không cho:

telekinesis;

fireball;

healing spell.

---

<a id="m19-t045-s069"></a>

## 38. CẢM MẠCH EXPERIENCE

Player có thể nhận sensory information dạng:

> “Bạn có cảm giác tiếng nước không khớp với dòng chảy.”

Không:

> Mạch anomaly detected +35.

---

<a id="m19-t045-s070"></a>

## 39. FALSE POSITIVE

Một người Cảm Mạch vẫn có thể:

- hiểu sai;
- bị tâm lý ảnh hưởng;
- bị ritual context tác động.

Sensitivity ≠ omniscience.

---

<a id="m19-t045-s071"></a>

## 40. TRAINING

Vu Chúc có thể giúp:

phân biệt perception.

record patterns.

control fear.

Không tăng “magic damage”.

---

<a id="m19-t045-s072"></a>

## 41. OVEREXPOSURE

Tiếp xúc mạnh có thể gây:

- dream disturbance;
- fatigue;
- sensory overload;
- identity confusion.

Không dùng mana depletion.

---

<a id="m19-t045-s073"></a>

## PART VIII — NGHI LỄ

<a id="m19-t045-s074"></a>

### 42. RITUAL PHILOSOPHY

Nghi lễ là:

> **hành động có cấu trúc dùng để tạo hoặc tái khẳng định quan hệ giữa người, cộng đồng, ký ức, nơi chốn và điều họ tin là thiêng.**

Nghi lễ có hiệu quả:

- xã hội;
- tâm lý;
- chính trị;
- đôi khi Mạch.

---

<a id="m19-t045-s075"></a>

## 43. RITUAL COMPONENTS

Một ritual có thể gồm:

```text
Purpose
Participants
Authority
Location
Timing
Sequence
Objects
Words / Song / Rhythm
Offerings
Witnesses
Taboos
Expected Outcome
```

Không ritual nào cần tất cả.

---

<a id="m19-t045-s076"></a>

## 44. PURPOSE

Các loại purpose:

- funeral;
- ancestor remembrance;
- oath;
- harvest;
- water;
- healing;
- initiation;
- leadership legitimacy;
- boundary;
- reconciliation;
- protection.

---

<a id="m19-t045-s077"></a>

## 45. RITUAL AUTHORITY

Ai có quyền thực hiện?

Có thể là:

- family elder;
- lineage keeper;
- village specialist;
- Vu Chúc;
- political leader.

Skill không luôn đủ.

---

<a id="m19-t045-s078"></a>

## 46. RITUAL CORRECTNESS

Correctness phụ thuộc:

- local tradition;
- participants;
- sequence;
- object;
- intent;
- authority.

Không có universal spell formula.

---

<a id="m19-t045-s079"></a>

## 47. RITUAL FAILURE

Có nhiều loại:

<a id="m19-t045-s080"></a>

#### Social Failure

Cộng đồng không chấp nhận.

<a id="m19-t045-s081"></a>

#### Procedural Failure

Sai trình tự.

<a id="m19-t045-s082"></a>

#### Psychological Failure

Participants mất niềm tin.

<a id="m19-t045-s083"></a>

#### Political Failure

Authority bị challenge.

<a id="m19-t045-s084"></a>

#### Mạch Failure

Mạch không phản ứng hoặc phản ứng lệch.

---

<a id="m19-t045-s085"></a>

## 48. RITUAL SUCCESS

Cũng đa lớp.

Một ritual có thể:

socially successful

nhưng supernatural outcome không rõ.

Hoặc:

community thinks failed

nhưng Mạch stabilizes.

---

<a id="m19-t045-s086"></a>

## 49. OFFERING

Offering không phải payment cho thần.

Nó có thể mang:

- symbolic value;
- sacrifice of resource;
- remembrance;
- redistribution.

---

<a id="m19-t045-s087"></a>

## 50. COST

Ritual cost có thể là:

- food;
- labor;
- prestige item;
- time;
- social commitment.

Đây là economic gameplay.

---

<a id="m19-t045-s088"></a>

## PART IX — ƯỚC SƠN HÀ

<a id="m19-t045-s089"></a>

### 51. ĐỊNH NGHĨA

Ước Sơn Hà là game-canon:

> **mạng lưới lâu đời của những lời hứa, hôn phối, nghi lễ, boundary, burial, quyền sử dụng đất/nước và nghĩa vụ giữa nhiều cộng đồng.**

Không phải một document bí mật duy nhất.

---

<a id="m19-t045-s090"></a>

## 52. CHỨC NĂNG LỊCH SỬ GAME

Nó từng giúp:

- giảm xung đột;
- phân quyền sử dụng nước;
- định boundary;
- duy trì ritual cooperation;
- kết nối community.

---

<a id="m19-t045-s091"></a>

## 53. TẠI SAO ƯỚC SUY YẾU

Không phải vì “tà thần”.

Mà vì:

- population change;
- political centralization;
- ecological change;
- old families decline;
- trade grows;
- new settlements;
- war;
- old obligations become impractical.

---

<a id="m19-t045-s092"></a>

## 54. GAMEPLAY

Player có thể:

- discover fragments;
- restore;
- reinterpret;
- replace;
- exploit loophole;
- reject.

Không có objective:

> restore all ancient oaths = good ending.

---

<a id="m19-t045-s093"></a>

## 55. OLD ORDER IS NOT AUTOMATICALLY GOOD

Một old covenant có thể:

- preserve peace;
- nhưng bất công với household mới;
- exclude outsiders;
- block necessary waterworks.

Đây là thematic tension lớn.

---

<a id="m19-t045-s094"></a>

## PART X — ĐIỀM, DẤU & OMEN

<a id="m19-t045-s095"></a>

### 56. OMEN PHILOSOPHY

Omen là:

> một event được con người đọc như dấu hiệu có ý nghĩa.

Không phải notification từ thần.

---

<a id="m19-t045-s096"></a>

## 57. OMEN SOURCES

- animal behavior;
- dream;
- weather;
- birth;
- sound;
- fire;
- unusual natural event;
- ritual anomaly.

---

<a id="m19-t045-s097"></a>

## 58. OMEN INTERPRETATION

Cùng omen:

Vu Chúc A:

bad harvest.

Hunter:

storm coming.

Skeptic:

normal animal migration.

Player quyết định tin ai.

---

<a id="m19-t045-s098"></a>

## 59. OMEN CAN BE USEFUL EVEN IF NOT SUPERNATURAL

Animal movement thật sự có thể báo:

weather.

predator.

flood.

Do đó folk knowledge có practical value.

---

<a id="m19-t045-s099"></a>

## 60. OMEN ENGINE

Engine tạo:

Physical Event.

Witness.

Cultural Interpretation.

Possible Mạch Link.

Không tạo “prophecy” tùy tiện.

---

<a id="m19-t045-s100"></a>

## 61. PROPHECY

Prophecy cực hiếm.

Nếu có phải:

- ambiguous;
- interpretable;
- self-fulfilling possible.

Không dùng để railroad player.

---

<a id="m19-t045-s101"></a>

## PART XI — TABOO

<a id="m19-t045-s102"></a>

### 62. TABOO PHILOSOPHY

Taboo là:

> một hành vi bị cộng đồng coi là vượt ranh giới.

Có thể có nguồn:

- practical;
- ecological;
- social;
- spiritual.

---

<a id="m19-t045-s103"></a>

## 63. EXAMPLE

Không săn một loại thú ở mùa sinh sản.

Community giải thích:

> thú thiêng.

Ecological effect:

giúp population phục hồi.

Hai layer cùng tồn tại.

---

<a id="m19-t045-s104"></a>

## 64. BREAKING TABOO

Consequences có thể:

- reputation;
- family conflict;
- ritual response;
- environmental impact;
- Mạch disturbance.

Không automatic curse.

---

<a id="m19-t045-s105"></a>

## 65. UNKNOWN TABOO

Outsider có thể không biết.

Lễ Tục/Regional Knowledge giúp.

---

<a id="m19-t045-s106"></a>

## PART XII — SACRED GEOGRAPHY

<a id="m19-t045-s107"></a>

### 66. SACRED SITE

Một nơi trở thành sacred vì:

- burial;
- myth;
- ritual;
- natural feature;
- historical event;
- Mạch.

---

<a id="m19-t045-s108"></a>

## 67. SACRED STATUS IS SOCIAL

Một rock không cần phát sáng.

Nó sacred vì:

community relation với nó.

---

<a id="m19-t045-s109"></a>

## 68. SACRED SITE MECHANICS

Có:

Access Rule.

Ritual Calendar.

Taboo.

Caretakers.

Historical Memory.

Mạch State.

---

<a id="m19-t045-s110"></a>

## 69. CONTESTED SACRED SITE

Hai community có thể:

cùng claim.

interpret khác.

Political quest phát sinh.

---

<a id="m19-t045-s111"></a>

## 70. DESECRATION

Không chỉ phá vật lý.

Có thể:

- wrong burial;
- logging;
- inappropriate construction;
- theft;
- public insult.

---

<a id="m19-t045-s112"></a>

## 71. SACRED SITE TRANSFORMATION

Một site có thể:

lose significance.

gain new meaning.

merge traditions.

Cổ Loa có thể tạo sacred geography mới.

---

<a id="m19-t045-s113"></a>

## PART XIII — LINH THỂ & ENTITY

<a id="m19-t045-s114"></a>

### 72. ENTITY PHILOSOPHY

Supernatural entity không được thiết kế trước tiên như combat monster.

Mỗi entity phải có:

```text
Domain
Origin
Behavior
Need
Relationship to Place
Relationship to Humans
Perception Rules
Communication
Boundaries
Fear
Possible Resolution
```

---

<a id="m19-t045-s115"></a>

## 73. ENTITY CATEGORIES

Không phải “monster types” cứng.

Có thể chia theo relationship.

<a id="m19-t045-s116"></a>

#### Residual Presence

Dấu ấn của event/death.

<a id="m19-t045-s117"></a>

#### Place-Bound Entity

Gắn địa điểm.

<a id="m19-t045-s118"></a>

#### Ancestral Presence

Liên quan lineage/memory.

<a id="m19-t045-s119"></a>

#### Natural Intelligence

Gắn river/mountain/animal motif.

<a id="m19-t045-s120"></a>

#### Constructed Ritual Entity

Hình thành từ repeated collective practice.

<a id="m19-t045-s121"></a>

#### Mythic Entity

High-tier, khó xác định ontology.

---

<a id="m19-t045-s122"></a>

## 74. RESIDUAL PRESENCE

Không phải ghost có personality đầy đủ.

Có thể là:

- tiếng;
- dream;
- repeated memory;
- sensory anomaly.

---

<a id="m19-t045-s123"></a>

## 75. PLACE-BOUND ENTITY

Không rời khỏi domain dễ dàng.

Có goal liên quan:

preserve.

repeat.

warn.

feed on attention?

Nếu dùng concept cuối phải cẩn thận, tránh horror cliché.

---

<a id="m19-t045-s124"></a>

## 76. ANCESTRAL PRESENCE

Chỉ một số trường hợp.

Không phải người chết nào cũng tồn tại như NPC.

Có thể được tạo bởi:

strong memory + ritual + Mạch.

---

<a id="m19-t045-s125"></a>

## 77. NATURAL INTELLIGENCE

Ví dụ river presence.

Không nhất thiết có hình dạng người.

Có thể được con người nhân cách hóa.

---

<a id="m19-t045-s126"></a>

## 78. MYTHIC ENTITY

Rất hiếm.

Kim Quy-type manifestation có thể thuộc nhóm này.

Không được farm.

Không có loot table.

---

<a id="m19-t045-s127"></a>

## PART XIV — SUPERNATURAL ENCOUNTER

<a id="m19-t045-s128"></a>

### 79. CÁC PHASE

Một encounter thường có:

<a id="m19-t045-s129"></a>

#### Manifestation

Dấu hiệu xuất hiện.

<a id="m19-t045-s130"></a>

#### Interpretation

Party tranh luận.

<a id="m19-t045-s131"></a>

#### Engagement

Observe / ritual / dialogue / flee / attack.

<a id="m19-t045-s132"></a>

#### Consequence

Physical/social/Mạch.

---

<a id="m19-t045-s133"></a>

## 80. COMBAT IS OPTIONAL

Một entity có thể:

không thể “giết” bằng weapon.

Hoặc đánh manifestation chỉ giải quyết tạm thời.

---

<a id="m19-t045-s134"></a>

## 81. ENTITY RESOLUTION MODES

Possible:

- understand;
- appease;
- restore relationship;
- relocate;
- break ritual;
- contain;
- destroy manifestation;
- leave;
- exploit.

---

<a id="m19-t045-s135"></a>

## 82. KILLING ENTITY

Nếu có thể:

không tự = good solution.

Có thể làm Mạch unstable.

---

<a id="m19-t045-s136"></a>

## PART XV — DREAM SYSTEM

<a id="m19-t045-s137"></a>

### 83. DREAM PHILOSOPHY

Dream không phải random cutscene.

Nó có thể đến từ:

- psychology;
- memory;
- illness;
- Mạch;
- entity;
- ritual.

---

<a id="m19-t045-s138"></a>

## 84. DREAM INPUT

Engine sử dụng:

Recent Memories.

Fear.

Guilt.

Belief.

Location.

Mạch State.

---

<a id="m19-t045-s139"></a>

## 85. DREAM SYMBOL

Symbol phải lấy từ character knowledge.

Một fisher có dream imagery khác smith.

---

<a id="m19-t045-s140"></a>

## 86. DREAM KNOWLEDGE

Dream không tự cho Fact chắc chắn.

Journal lưu:

> Dream Experience.

Không:

Confirmed Truth.

---

<a id="m19-t045-s141"></a>

## 87. SHARED DREAM

Rất hiếm.

Nếu nhiều character có pattern giống nhau:

tăng khả năng Mạch/entity involvement.

Nhưng vẫn cần investigation.

---

<a id="m19-t045-s142"></a>

## 88. DREAM DUNGEON

Có thể xảy ra khi:

Mạch condition cực mạnh.

Không dùng thường xuyên.

Rule:

logic biểu tượng phải nhất quán.

---

<a id="m19-t045-s143"></a>

## PART XVI — HEALING & SUPERNATURAL

<a id="m19-t045-s144"></a>

### 89. NO MAGIC HEALING

Vu Chúc không thay Dược Nhân.

Nghi lễ có thể:

- reduce panic;
- help grief;
- increase treatment cooperation;
- affect Mạch condition.

Không đóng vết thương tức thì.

---

<a id="m19-t045-s145"></a>

## 90. PSYCHOSOMATIC EFFECT

Belief có thể ảnh hưởng recovery.

Nếu patient tin ritual:

stress giảm.

Nếu ritual thất bại:

fear tăng.

Game không cần gọi đó là “placebo” một cách phá immersion.

---

<a id="m19-t045-s146"></a>

## 91. MẠCH-RELATED ILLNESS

Một số condition có thể thật sự liên quan Mạch.

Dược Nhân xử lý physical symptom.

Vu Chúc xử lý relation/context.

Hai nghề có thể cần phối hợp.

---

<a id="m19-t045-s147"></a>

## PART XVII — DEATH, FUNERAL & AFTERLIFE AMBIGUITY

<a id="m19-t045-s148"></a>

### 92. DEATH

Game không xác nhận một afterlife universal.

Các community tin khác nhau.

---

<a id="m19-t045-s149"></a>

## 93. FUNERAL FUNCTIONS

Funeral có:

- xử lý grief;
- social transition;
- inheritance;
- ancestor status;
- body handling;
- Mạch relation.

---

<a id="m19-t045-s150"></a>

## 94. UNBURIED DEAD

Có thể tạo:

social concern.

disease/ecological issue.

psychological fear.

Mạch instability.

Không auto zombie.

---

<a id="m19-t045-s151"></a>

## 95. BODY RECOVERY

Party Module đã xác định body matters.

Religion module bổ sung:

cách xử lý corpse ảnh hưởng lineage/ritual.

---

<a id="m19-t045-s152"></a>

## 96. MASS DEATH

Battlefield có thể tạo Mạch rất mạnh.

Nhiều năm sau:

dream.

taboo.

ritual.

memory.

---

<a id="m19-t045-s153"></a>

## PART XVIII — MYTHOLOGY SYSTEM

<a id="m19-t045-s154"></a>

### 97. MYTH KHÔNG PHẢI LORE DATABASE CỐ ĐỊNH

Myth là:

> **câu chuyện xã hội dùng để giải thích, ghi nhớ và truyền ý nghĩa.**

---

<a id="m19-t045-s155"></a>

## 98. MYTH OBJECT

Có:

```text
Core Motif
Origin Region
Known Versions
Historical Associations
Ritual Use
Faction Use
Current Believers
Contradictions
Evolution
```

---

<a id="m19-t045-s156"></a>

## 99. MULTIPLE VERSIONS

Một myth có thể có:

Tam Giang Version.

Phong Châu Version.

Tản Viên Version.

Không cần một canonical dialogue duy nhất.

---

<a id="m19-t045-s157"></a>

## 100. MYTH AS POLITICS

Faction có thể dùng myth để:

- claim legitimacy;
- unite people;
- justify land;
- support leader.

---

<a id="m19-t045-s158"></a>

## 101. MYTH AS KNOWLEDGE

Một câu chuyện tưởng myth có thể chứa:

- route;
- flood memory;
- ecological pattern;
- historical event.

Người Giữ Ký Ức có gameplay lớn.

---

<a id="m19-t045-s159"></a>

## 102. MYTH FORMATION FROM PLAYER

Hệ đã xây trước đó nối vào đây.

Player action:

Event.

↓

Rumor.

↓

Retelling.

↓

Symbol.

↓

Myth.

---

<a id="m19-t045-s160"></a>

## 103. MYTH DISTORTION

Ví dụ truth:

Player killed a giant boar with three hunters.

Later:

> “Người ấy một mình giết con lợn lớn bằng căn nhà.”

Không cần magical transformation.

---

<a id="m19-t045-s161"></a>

## 104. MYTH FEEDBACK

Khi myth lan:

future NPC có expectation.

Player successor chịu reputation.

Một myth có thể bắt đầu ảnh hưởng reality xã hội.

---

<a id="m19-t045-s162"></a>

## PART XIX — LEGENDARY CYCLES

<a id="m19-t045-s163"></a>

### 105. LẠC LONG QUÂN – ÂU CƠ

Trong game:

đây là **deep ancestral myth**.

Không nên cho player gặp literal characters nếu campaign diễn ra nhiều thế hệ sau.

Gameplay uses:

- lineage identity;
- origin stories;
- political legitimacy;
- sea/mountain duality.

---

<a id="m19-t045-s164"></a>

## 106. SƠN TINH – THỦY TINH

Không xác nhận một version.

Could represent:

- persons;
- titles;
- powers;
- natural cycles;
- political memory;
- Mạch entities.

Different regions preserve different interpretations.

---

<a id="m19-t045-s165"></a>

## 107. THÁNH GIÓNG

Là older heroic myth.

Can influence:

- warrior ideals;
- relic claims;
- ritual;
- place names.

Không cần literal current NPC.

---

<a id="m19-t045-s166"></a>

## 108. LANG LIÊU

Có thể hoạt động như:

- food/culture story;
- legitimacy story;
- agricultural identity.

Không nhất thiết quest about finding magic rice cake.

---

<a id="m19-t045-s167"></a>

## 109. KIM QUY

Late campaign đặc biệt quan trọng.

Không nên xử lý như:

> quest NPC turtle gives legendary bow.

Kim Quy có thể là intersection giữa:

- old water/turtle traditions;
- Loa Vực sacred geography;
- engineering success;
- political legitimacy;
- Mạch manifestation;
- later myth formation.

---

<a id="m19-t045-s168"></a>

## PART XX — “NỎ THẦN” & MYTH FORMATION

<a id="m19-t045-s169"></a>

### 110. BỐN LỚP

<a id="m19-t045-s170"></a>

#### Layer 1 — Technology

Crossbow design.

<a id="m19-t045-s171"></a>

#### Layer 2 — Production

Standardization.

<a id="m19-t045-s172"></a>

#### Layer 3 — Organization

Training, ammunition, logistics.

<a id="m19-t045-s173"></a>

#### Layer 4 — Myth

People later compress complex system into:

> “Nỏ thần.”

---

<a id="m19-t045-s174"></a>

## 111. SUPERNATURAL OPTION

Game vẫn có thể cho một Mạch/Kim Quy event thật sự xảy ra.

Nhưng event không cần tự tay tạo weapon.

Nó có thể:

- legitimate project;
- inspire design interpretation;
- protect knowledge;
- create powerful symbolic memory.

---

<a id="m19-t045-s175"></a>

## PART XXI — VU CHÚC

<a id="m19-t045-s176"></a>

### 112. VU CHÚC KHÔNG PHẢI WIZARD

Vu Chúc là specialist ở giao điểm:

- ritual;
- belief;
- death;
- community;
- omen;
- Mạch.

---

<a id="m19-t045-s177"></a>

## 113. VU CHÚC POWER SOURCES

Không có mana.

Sức mạnh của họ đến từ:

- knowledge;
- authority;
- preparation;
- ritual;
- social trust;
- Cảm Mạch nếu có.

---

<a id="m19-t045-s178"></a>

## 114. VU CHÚC KHÔNG CẦN CẢM MẠCH

Một ritual master có thể rất giỏi dù sensitivity thấp.

Ngược lại người Cảm Mạch cao có thể không biết nghi lễ.

---

<a id="m19-t045-s179"></a>

## 115. VU CHÚC TECHNIQUE

Technique nên kiểu:

**Đọc Lệch Nhịp**

Nhận ra nghi lễ bị thay đổi.

**Giữ Vòng Người Sống**

Tổ chức participants giữ bình tĩnh quanh death event.

**Nghe Mạch Sau Lễ**

Observe post-ritual changes.

Không:

Spirit Bolt.

---

<a id="m19-t045-s180"></a>

## 116. FRAUDULENT VU CHÚC

Có thể tồn tại.

Nhưng hệ không mặc định mọi Vu Chúc là fraud.

Một người có thể:

skill thấp nhưng tin thật.

Một người có thể:

không tin nhưng diễn.

Một người có thể:

thật sự Cảm Mạch.

---

<a id="m19-t045-s181"></a>

## PART XXII — SKEPTICISM

<a id="m19-t045-s182"></a>

### 117. SKEPTIC IS NOT “CORRECT CLASS”

Game phải tránh trope:

> rational skeptic = thông minh, believer = ngu.

Skeptic có thể:

đúng.

sai.

partial.

---

<a id="m19-t045-s183"></a>

## 118. SKEPTIC GAMEPLAY

Có lợi:

- resist superstition-based manipulation;
- seek physical cause;
- challenge ritual authority.

Bất lợi:

- ignore cultural meaning;
- lose community trust;
- miss Mạch phenomena.

---

<a id="m19-t045-s184"></a>

## 119. BELIEVER GAMEPLAY

Có lợi:

- ritual access;
- cultural understanding;
- community trust.

Risk:

- interpret physical threat spiritually;
- manipulated by authority.

---

<a id="m19-t045-s185"></a>

## PART XXIII — RELIGION & SOCIAL POWER

<a id="m19-t045-s186"></a>

### 120. RITUAL LEGITIMACY

Political leader có thể cần ritual recognition.

Không phải because gods mechanically buff throne.

Mà vì:

people accept authority.

---

<a id="m19-t045-s187"></a>

## 121. RITUAL SPECIALIST POWER

Người giữ nghi lễ có:

information.

access.

social trust.

Power không chỉ supernatural.

---

<a id="m19-t045-s188"></a>

## 122. CONFLICT

Political authority vs ritual authority có thể xung đột.

Ví dụ:

construction needs sacred land.

---

<a id="m19-t045-s189"></a>

## 123. SACRED ECONOMY

Ritual consumes:

food.

objects.

labor.

Could redistribute wealth.

Economic system connects.

---

<a id="m19-t045-s190"></a>

## PART XXIV — RELIGION & FACTION

<a id="m19-t045-s191"></a>

### 124. RITUAL NETWORK AS FACTION-LIKE ENTITY

Một network có thể có:

- practitioners;
- sacred sites;
- authority;
- resources.

Nhưng không nhất thiết centralized.

---

<a id="m19-t045-s192"></a>

## 125. INTERNAL DISAGREEMENT

Vu Chúc không đồng quan điểm.

Một nhóm:

restore old rites.

Nhóm khác:

adapt them.

---

<a id="m19-t045-s193"></a>

## 126. POLITICAL USE

Faction có thể:

support ritual.

control it.

suppress.

reinterpret.

---

<a id="m19-t045-s194"></a>

## PART XXV — RELIGION & ECONOMY

<a id="m19-t045-s195"></a>

### 127. SACRED RESOURCE

Một resource có thể bị hạn chế vì taboo.

Ví dụ old-growth wood.

Kinh tế có thể gây pressure lên belief.

---

<a id="m19-t045-s196"></a>

## 128. PILGRIMAGE / FESTIVAL FLOW

Ritual event kéo:

people.

trade.

food demand.

information.

---

<a id="m19-t045-s197"></a>

## 129. FUNERAL ECONOMY

Death tạo nhu cầu:

labor.

objects.

travel.

kin attendance.

Không gamify thành shop buff.

---

<a id="m19-t045-s198"></a>

## PART XXVI — RELIGION & EXPLORATION

<a id="m19-t045-s199"></a>

### 130. SACRED MAP LAYER

Player có thể biết:

shrines.

graves.

taboo forests.

Mạch sites.

---

<a id="m19-t045-s200"></a>

## 131. LOCAL KNOWLEDGE

Một trail không đi vì:

practical danger

hay

taboo?

Player cần hiểu.

---

<a id="m19-t045-s201"></a>

## 132. SPIRITUAL LANDMARK

Một unusual stone có thể:

geographic landmark.

ritual site.

Mạch node.

Ba layers.

---

<a id="m19-t045-s202"></a>

## PART XXVII — RELIGION & DUNGEON

<a id="m19-t045-s203"></a>

### 133. RITUAL DUNGEON

Đã khóa trước.

Module này bổ sung:

ritual state phải persistent.

---

<a id="m19-t045-s204"></a>

## 134. SACRED TOMB

Player có thể giải bằng:

- archaeology/investigation;
- social negotiation;
- ritual;
- engineering.

Không bắt fight undead.

---

<a id="m19-t045-s205"></a>

## 135. DREAM DUNGEON

Phải lấy symbol từ:

character memory.

local myth.

Mạch.

Không random surreal room.

---

<a id="m19-t045-s206"></a>

## PART XXVIII — SUPERNATURAL & COMBAT

<a id="m19-t045-s207"></a>

### 136. SUPERNATURAL COMBAT PHILOSOPHY

Nếu combat entity xảy ra:

combat phải dựa trên understanding.

Không HP sponge.

---

<a id="m19-t045-s208"></a>

## 137. ENTITY DEFENSE

Một manifestation có thể:

- không bị weapon thường;
- bị disrupt bởi environment;
- bị yếu khi ritual relation thay đổi.

---

<a id="m19-t045-s209"></a>

## 138. FEAR

Entity often attacks:

morale/perception

trước physical body.

---

<a id="m19-t045-s210"></a>

## 139. PHYSICAL ENTITY

Một mythic beast vẫn có thể bị thương vật lý.

Nhưng hành vi và context quan trọng hơn HP.

---

<a id="m19-t045-s211"></a>

## 140. NO LOOT TABLE

Không:

Spirit Essence x3.

Rare Soul Fragment.

Nếu có material remain:

cần world logic.

---

<a id="m19-t045-s212"></a>

## PART XXIX — SUPERNATURAL & QUEST

<a id="m19-t045-s213"></a>

### 141. QUEST ENTRY

Một supernatural thread có thể bắt đầu như:

- disease;
- missing person;
- rumor;
- environmental anomaly.

Không label ngay:

“Haunting Quest.”

---

<a id="m19-t045-s214"></a>

## 142. MULTIPLE HYPOTHESES

Journal có thể lưu:

- poisoning;
- psychological panic;
- ritual disturbance;
- entity.

Player điều tra.

---

<a id="m19-t045-s215"></a>

## 143. RESOLUTION WITHOUT TRUTH

Có thể solve practical problem mà không biết ontology.

Ví dụ:

move burial.

flood stops.

Was it natural timing?

Unknown.

---

<a id="m19-t045-s216"></a>

## PART XXX — “MIRACLE” DESIGN

<a id="m19-t045-s217"></a>

### 144. MIRACLE PHILOSOPHY

Miracle phải cực hiếm.

Nếu miracle xảy ra hàng tuần:

nó thành magic system.

---

<a id="m19-t045-s218"></a>

## 145. MIRACLE REQUIREMENTS

Possible convergence:

- powerful Mạch;
- major collective ritual;
- historical moment;
- strong symbolic act;
- extreme circumstance.

---

<a id="m19-t045-s219"></a>

## 146. MIRACLE OUTCOME

Không nên:

+500 HP.

Có thể:

- impossible-seeming survival;
- environmental shift;
- shared vision;
- entity manifestation.

---

<a id="m19-t045-s220"></a>

## 147. MIRACLE INTERPRETATION

Different characters react differently.

Game không hiện:

**DIVINE MIRACLE CONFIRMED.**

---

<a id="m19-t045-s221"></a>

## PART XXXI — MYTH & POLITICS

<a id="m19-t045-s222"></a>

### 148. LEGITIMACY MYTH

Leader có thể claim descent.

Player có thể:

- investigate;
- support;
- disprove;
- ignore.

---

<a id="m19-t045-s223"></a>

## 149. MYTH MANIPULATION

Faction can reshape story.

Narrative warfare.

---

<a id="m19-t045-s224"></a>

## 150. COUNTER-MYTH

Another community may preserve competing version.

No single codex truth unless evidence supports.

---

<a id="m19-t045-s225"></a>

## PART XXXII — CODEX & CHRONICLE

<a id="m19-t045-s226"></a>

### 151. CODEX STRUCTURE

Myth entry should show:

<a id="m19-t045-s227"></a>

#### Known Versions

What player heard.

<a id="m19-t045-s228"></a>

#### Sources

Who told.

<a id="m19-t045-s229"></a>

#### Associated Places

<a id="m19-t045-s230"></a>

#### Player Notes

<a id="m19-t045-s231"></a>

#### Evidence

Not:

“True Lore.”

---

<a id="m19-t045-s232"></a>

## 152. HISTORICAL LAYER

If player discovers physical evidence:

entry updates.

---

<a id="m19-t045-s233"></a>

## 153. CONTRADICTION

Codex can display:

> “Version from Tam Giang differs from Phong Châu.”

No forced resolution.

---

<a id="m19-t045-s234"></a>

## PART XXXIII — UI

<a id="m19-t045-s235"></a>

### 154. NO FAITH METER

Character sheet should not say:

Faith 62%.

Could say:

> Strongly attached to ancestor rites.

> Skeptical of omen readers.

---

<a id="m19-t045-s236"></a>

## 155. MẠCH UI

No exact numeric bar normally.

Display:

Quiet.

Uneasy.

Disturbed.

Fractured.

Only if player has knowledge/sensitivity.

---

<a id="m19-t045-s237"></a>

## 156. RITUAL UI

Show:

Purpose.

Known requirements.

Participants.

Known risks.

Not spell hotbar.

---

<a id="m19-t045-s238"></a>

## 157. BELIEF DIALOGUE

Character beliefs unlock:

interpretation.

Not always action bonus.

---

<a id="m19-t045-s239"></a>

## PART XXXIV — CONTENT AUTHORING FRAMEWORK

<a id="m19-t045-s240"></a>

### 158. EVERY BELIEF ENTRY MUST DEFINE

1. Region.
2. Community.
3. Origin if known.
4. Practice.
5. Social function.
6. Associated taboo.
7. Alternative interpretations.
8. Historical support level.
9. Game-canon additions.

---

<a id="m19-t045-s241"></a>

## 159. EVERY RITUAL MUST DEFINE

1. Purpose.
2. Participants.
3. Authority.
4. Timing.
5. Objects.
6. Sequence.
7. Cultural meaning.
8. Social outcome.
9. Possible Mạch interaction.
10. Failure modes.

---

<a id="m19-t045-s242"></a>

## 160. EVERY ENTITY MUST DEFINE

1. Why it exists.
2. Where it is bound.
3. What it wants.
4. What humans think it is.
5. How it communicates.
6. What it fears/avoids.
7. How encounter ends.
8. Whether combat is meaningful.
9. What remains unresolved.

---

<a id="m19-t045-s243"></a>

## 161. EVERY MYTH MUST DEFINE

1. Core motif.
2. Versions.
3. Geography.
4. Ritual relation.
5. Political use.
6. Possible historical layer.
7. Mạch relation.
8. How it can evolve.

---

<a id="m19-t045-s244"></a>

## PART XXXV — PROCEDURAL GENERATION

<a id="m19-t045-s245"></a>

### 162. WHAT MAY BE PROCEDURAL

- minor omen;
- local ritual variation;
- dream imagery;
- rumor versions;
- minor taboo;
- ancestor story detail.

---

<a id="m19-t045-s246"></a>

## 163. WHAT MUST BE AUTHORED OR STRICTLY VALIDATED

- major mythology;
- historical legend adaptation;
- major Mạch event;
- core ritual tradition;
- mythic entity;
- Cổ Loa/Kim Quy material.

---

<a id="m19-t045-s247"></a>

## 164. PROCEDURAL BELIEF MUST HAVE CAUSE

No random:

> Village worships blue rabbit.

Need connection to:

ecology.

history.

family.

place.

---

<a id="m19-t045-s248"></a>

## PART XXXVI — LLM ARCHITECTURE

<a id="m19-t045-s249"></a>

### 165. RULE ENGINE OWNS

- Mạch state;
- entity existence;
- objective event;
- ritual outcome;
- belief profile;
- knowledge.

---

<a id="m19-t045-s250"></a>

## 166. LLM MAY GENERATE

- prayer-like wording;
- ritual speech;
- myth retelling;
- dream description;
- dialogue.

---

<a id="m19-t045-s251"></a>

## 167. LLM MAY NOT INVENT

- new god;
- new entity;
- new miracle;
- new historical fact;
- new ritual mechanic.

Without Content Validation.

---

<a id="m19-t045-s252"></a>

## PART XXXVII — EXAMPLE 1: TAM GIANG — “TIẾNG TRỐNG TRƯỚC LŨ”

<a id="m19-t045-s253"></a>

### 168. INITIAL EVENT

Một ritual drum sounds at night.

Không ai nhận đã đánh.

Ngày sau water begins rising unusually.

---

<a id="m19-t045-s254"></a>

## 169. INTERPRETATIONS

Elder:

> water spirit warning.

Boatman:

> upper river flood.

Vu Chúc:

> ritual rhythm was incomplete.

Skeptic:

> someone played prank.

---

<a id="m19-t045-s255"></a>

## 170. PLAYER INVESTIGATION

Can inspect:

drum.

river.

footprints.

weather.

ritual history.

---

<a id="m19-t045-s256"></a>

## 171. FINDING

Có evidence:

someone touched drum.

Nhưng Mạch around confluence cũng disturbed.

Hai facts cùng đúng.

---

<a id="m19-t045-s257"></a>

## 172. RESOLUTION

Player can:

fix dyke.

warn villages.

perform ritual.

catch intruder.

Any combination.

Ritual alone does not magically stop water.

Engineering alone may save village but not resolve cultural crisis.

---

<a id="m19-t045-s258"></a>

## PART XXXVIII — EXAMPLE 2: TẢN VIÊN — “CON THÚ KHÔNG ĐƯỢC SĂN”

<a id="m19-t045-s259"></a>

### 173. SETUP

Một sacred animal damages crops.

Villagers divided.

---

<a id="m19-t045-s260"></a>

## 174. BELIEF

Older hunters:

animal is protected.

Farmers:

crop is at risk.

Young hunter:

taboo outdated.

---

<a id="m19-t045-s261"></a>

## 175. POSSIBLE TRUTH

Animal behavior changed because forest habitat was disrupted.

Mạch site nearby also unstable.

Again:

material + spiritual layers.

---

<a id="m19-t045-s262"></a>

## 176. PLAYER SOLUTIONS

Kill.

Relocate.

Repair habitat.

Protect field.

Ritual.

Each has consequences.

---

<a id="m19-t045-s263"></a>

## PART XXXIX — EXAMPLE 3: CỔ LOA — KIM QUY THREAD

<a id="m19-t045-s264"></a>

### 177. EARLY STAGE

Worker finds unusual turtle shell motif near wetland.

Rumor spreads.

No confirmation.

---

<a id="m19-t045-s265"></a>

## 178. MID STAGE

Construction encounters repeated water/soil problems.

Several engineering solutions fail.

Ritual group interprets as land rejection.

---

<a id="m19-t045-s266"></a>

## 179. PLAYER PATHS

Xảo Nhân:

studies soil/water.

Vu Chúc:

studies Mạch.

Người Giữ Ký Ức:

finds old local stories.

Thủy Binh:

reads water.

Different knowledge converges.

---

<a id="m19-t045-s267"></a>

## 180. MANIFESTATION

At a major turning point, game may allow an ambiguous but powerful encounter associated with Kim Quy imagery.

Could be:

physical animal?

Mạch entity?

shared vision?

The game need not answer completely.

---

<a id="m19-t045-s268"></a>

## 181. OUTCOME

Engineering breakthrough still requires human work.

Kim Quy becomes:

symbol.

legitimacy.

memory.

Later generations compress story.

---

<a id="m19-t045-s269"></a>

## PART XL — EXAMPLE 4: BATTLEFIELD MẠCH

<a id="m19-t045-s270"></a>

### 182. SETUP

Years after battle:

people hear voices at night.

---

<a id="m19-t045-s271"></a>

## 183. POSSIBLE CAUSES

wind through abandoned structures.

trauma.

looters.

Mạch residue.

---

<a id="m19-t045-s272"></a>

## 184. INVESTIGATION

Bodies were never properly recovered.

Families still search missing.

---

<a id="m19-t045-s273"></a>

## 185. RESOLUTION

Recover bodies.

identify names.

funeral.

clear terrain.

Even if voices stop:

was cause psychological/social/spiritual?

Unresolved.

---

<a id="m19-t045-s274"></a>

## PART XLI — SYSTEM INTERACTION

<a id="m19-t045-s275"></a>

### 186. CHARACTER GENERATION

Origin defines:

belief baseline.

taboo knowledge.

ritual exposure.

---

<a id="m19-t045-s276"></a>

## 187. PSYCHOLOGY

Belief interacts with:

fear.

identity.

grief.

---

<a id="m19-t045-s277"></a>

## 188. FAMILY

Ancestor rites.

lineage myth.

burial.

---

<a id="m19-t045-s278"></a>

## 189. PROFESSION

Profession gives interpretations.

Hunter sees ecological signs.

Smith sees fire/material symbolism differently.

---

<a id="m19-t045-s279"></a>

## 190. CLASS

Vu Chúc strongest direct interaction.

Người Giữ Ký Ức:

myth/history.

Sứ Giả:

ritual diplomacy.

Others still participate.

---

<a id="m19-t045-s280"></a>

## 191. PARTY

Members disagree.

A spiritual encounter can fracture party interpretation.

---

<a id="m19-t045-s281"></a>

## 192. COMBAT

Supernatural entities use special rules.

Human belief affects morale.

---

<a id="m19-t045-s282"></a>

## 193. EXPLORATION

Sacred geography.

Mạch sites.

omens.

---

<a id="m19-t045-s283"></a>

## 194. DUNGEON

Ritual/dream/tomb/site logic.

---

<a id="m19-t045-s284"></a>

## 195. QUEST

Belief creates framing.

Mạch creates hidden layer.

---

<a id="m19-t045-s285"></a>

## 196. ECONOMY

Sacred resource.

ritual cost.

festival trade.

---

<a id="m19-t045-s286"></a>

## 197. FACTION

Ritual legitimacy.

myth politics.

---

<a id="m19-t045-s287"></a>

## 198. REPUTATION

Player can become known as:

ritual breaker.

skeptic.

protector of shrines.

But interpretation depends group.

---

<a id="m19-t045-s288"></a>

## 199. LEGACY

Player deeds may enter myth.

---

<a id="m19-t045-s289"></a>

## PART XLII — DIFFICULTY & GAME MODES

<a id="m19-t045-s290"></a>

### 200. STORY MODE

Supernatural clues clearer.

Fear less punishing.

---

<a id="m19-t045-s291"></a>

## 201. STANDARD

Ambiguity baseline.

---

<a id="m19-t045-s292"></a>

## 202. MYSTERY MODE

Less explicit Mạch feedback.

More reliance on interpretation.

Could be optional advanced mode.

---

<a id="m19-t045-s293"></a>

## 203. LEGACY MODE

Belief and Mạch consequences fully persistent across generations.

---

<a id="m19-t045-s294"></a>

## PART XLIII — WHAT THE GAME MUST NOT DO

<a id="m19-t045-s295"></a>

### 204. PROHIBITED DESIGN DIRECTIONS

Không nên:

- mana bars;
- fireball spells;
- resurrection economy;
- potion magic;
- deity faction like MMO;
- universal church structure;
- clearly labeled “good gods / evil gods”;
- supernatural loot farming;
- killing spirits for XP;
- skeptic always right;
- believer always right;
- myth converted into a single banal scientific explanation;
- every unknown event secretly supernatural;
- every sacred place glowing visually.

---

<a id="m19-t045-s296"></a>

## PART XLIV — QUALITY GATE

<a id="m19-t045-s297"></a>

### 205. BELIEF CONTENT QUALITY

Mỗi belief phải trả lời:

1. Ai tin?
2. Vì sao?
3. Họ làm gì vì belief đó?
4. Nó ảnh hưởng social life ra sao?
5. Có alternative interpretation không?
6. Player có thể challenge không?

---

<a id="m19-t045-s298"></a>

## 206. MẠCH QUALITY

Một Mạch site phải có:

1. Physical geography.
2. Human history.
3. Memory.
4. Current pressure.
5. Observable manifestation.
6. Multiple interpretation.

Nếu chỉ:

> magic energy node

thì không đạt.

---

<a id="m19-t045-s299"></a>

## 207. SUPERNATURAL ENCOUNTER QUALITY

Phải có:

1. Cause/context.
2. Uncertainty.
3. Non-combat option.
4. Human interpretation.
5. Persistent consequence.
6. No generic loot.

---

<a id="m19-t045-s300"></a>

## 208. MYTH QUALITY

Phải:

- có nhiều version hoặc khả năng biến đổi;
- liên kết geography/community;
- không chỉ exposition;
- có gameplay relevance.

---

<a id="m19-t045-s301"></a>

## 209. RITUAL QUALITY

Phải có:

social purpose

chứ không chỉ supernatural effect.

---

<a id="m19-t045-s302"></a>

## PART XLV — LOCKED DESIGN DECISIONS

<a id="m19-t045-s303"></a>

### 210. CÁC ĐIỂM KHÓA

1. Religion không được mô hình hóa như church system kiểu fantasy.
2. Belief là multidimensional profile.
3. Character có thể vừa tin vừa nghi ngờ những thứ khác nhau.
4. Archaeology, tradition và game-canon luôn tách.
5. Mạch Lạc là nền metaphysics chính.
6. Mạch không phải mana.
7. Mạch gắn geography + people + memory.
8. Mạch không có morality good/evil intrinsic.
9. Cảm Mạch hiếm.
10. Cảm Mạch không tạo combat magic.
11. Vu Chúc không phải wizard.
12. Vu Chúc có thể không Cảm Mạch.
13. Ritual có social/political function thật.
14. Ritual outcome có nhiều layer.
15. Ancestor không mặc định tồn tại như ghost.
16. Omen không mặc định là prophecy.
17. Taboo có thể có practical và spiritual layer cùng lúc.
18. Sacred place là social + geographical reality.
19. Supernatural entity không được thiết kế trước tiên như monster.
20. Combat với entity không mặc định là cách giải tốt nhất.
21. Dream không phải source of confirmed truth.
22. Myth có multiple versions.
23. Myth có thể trở thành political resource.
24. Player action có thể trở thành myth.
25. Kim Quy/Nỏ thần phải giữ technological + political + Mạch + myth layers.
26. Cổ Loa phải tạo Mạch mới chứ không chỉ phá Mạch cũ.
27. Skepticism và belief đều có lợi/risk.
28. Không universal afterlife confirmation.
29. Healing supernatural không thay medicine.
30. Miracle cực hiếm.
31. LLM không quyết supernatural truth.
32. Supernatural content phải persistent trong world state.

---

<a id="m19-t045-s304"></a>

## PART XLVI — DEFERRED TO PRODUCTION

<a id="m19-t045-s305"></a>

### 211. NỘI DUNG SẼ XÂY SAU

Production cần nghiên cứu và viết sâu:

- regional ritual catalog;
- burial traditions;
- ancestor practices;
- festival calendar;
- regional belief variants;
- sacred animal traditions;
- full myth variant library;
- major supernatural entities;
- Vu Chúc techniques;
- ritual items;
- omen catalog;
- Mạch map;
- Dream symbolic library;
- Kim Quy thread;
- Sơn Tinh/Thủy Tinh interpretations;
- Thánh Gióng memory sites;
- Lang Liêu cultural material;
- historical terminology.

Không cần khóa hàng trăm mục đó ở GDD core.

---

<a id="m19-t045-s306"></a>

## PART XLVII — CORE RELIGION FANTASY

<a id="m19-t045-s307"></a>

### 212. PLAYER EXPERIENCE

Game không nên khiến player hỏi:

> “Thần nào cho buff mạnh nhất?”

Mà:

> “Tại sao người làng này không chịu đi qua khu rừng đó?”

> “Họ thật sự sợ một linh vật, hay taboo này đang bảo vệ điều gì?”

> “Lễ này có ý nghĩa với ai?”

> “Nếu tôi phá nó vì thấy vô lý, tôi sẽ phá cả điều gì khác?”

> “Điều tôi vừa thấy là gì?”

---

<a id="m19-t045-s308"></a>

## 213. CORE SUPERNATURAL FANTASY

Supernatural mạnh nhất khi player **không hoàn toàn chắc mình vừa chứng kiến điều gì**.

Không phải vì game giấu thông tin tùy tiện.

Mà vì thế giới cho phép nhiều lớp nguyên nhân tồn tại cùng lúc.

Một tiếng gọi trong rừng có thể là:

người.

chim.

ký ức.

Mạch.

hoặc thứ gì đó thực sự đang gọi.

Và việc player tin nó là gì có thể quan trọng không kém bản thân sự thật.

---

<a id="m19-t045-s309"></a>

## 214. CORE MẠCH LẠC FANTASY

Mạch Lạc đặt một câu hỏi xuyên suốt game:

> **Một vùng đất là gì nếu tách khỏi những con người đã sống, chết và ghi nhớ nó?**

Một con sông không chỉ là nước.

Nó là:

route.

food.

grave.

flood.

story.

promise.

border.

memory.

Mạch Lạc chính là cách game nối tất cả những lớp đó thành một thế giới duy nhất.

---

<a id="m19-t045-s310"></a>

## 215. CORE MYTHOLOGY FANTASY

Ở đầu campaign:

player nghe truyền thuyết về người xưa.

Cuối campaign:

player bắt đầu hiểu một điều khác:

> **truyền thuyết không chỉ thuộc về quá khứ.**

Những gì player và companion làm hôm nay—

một trận lũ;

một lời thề;

một cây nỏ;

một cái chết;

một thành xoáy ốc;

một người được cứu;

một người bị phản bội—

có thể trở thành câu chuyện mà thế hệ sau kể lại sai đi, đẹp lên, đáng sợ hơn, thiêng hơn.

Và đến cuối game, player có thể nhận ra:

> **mình không chỉ sống trong thần thoại.**

> **mình đang chứng kiến thần thoại được sinh ra.**

Sau module này, **bản sắc thế giới** đã được khóa tương đối chắc. Bước tiếp theo hợp lý nhất là **Creature / Bestiary + Ecology + Human Threats**, vì giờ chúng ta đã đủ nền để phân biệt rõ: **động vật thật, thú nguy hiểm, sinh vật truyền thuyết, linh thể, manifestation của Mạch, và con người** — đồng thời thiết kế chúng theo ecology/behavior thay vì “mob level”. Sau đó chúng ta mới sang **Time / Calendar / Aging / Death / Legacy**, là mảnh còn thiếu để hoàn chỉnh campaign kéo dài 20–25 năm.

<!-- SOURCE T045 END -->

