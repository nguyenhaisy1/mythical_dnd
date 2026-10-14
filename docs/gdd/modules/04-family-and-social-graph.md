# Module 04 — Dòng họ, gia đình, household và mạng xã hội

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T011](../../source/conversation.md#t011)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Lineage, Family và Household khác nhau. Quan hệ là graph có hướng, nhiều chiều; succession và inheritance không copy toàn bộ profile.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Birth context, household membership, kinship, marriage, death, memories. |
| Đầu ra / bên sở hữu | Family/household graph; Household sở hữu membership; Social sở hữu relationships. |
| Đọc cùng | 05, 07, 16, 21, 26. |
| Cổng nghiệm thu | Một người chết không còn làm việc nhưng quan hệ và lịch sử không biến mất. |
| Việc chưa được chứng minh | Luật xã hội theo vùng và bộ fixture genealogy production. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T011 BEGIN -->

<a id="m04-t011-s001"></a>

## WORLD BIBLE 03 — FAMILY, HOUSEHOLD & SOCIAL GRAPH

Đây là lớp tôi coi là **xương sống của toàn bộ hệ thống nhân vật**.

Từ thời điểm này, game sẽ không còn sinh “NPC”. Game sẽ sinh:

> **con người thuộc một gia đình, sống trong một household, thuộc một cộng đồng, có lịch sử và có quan hệ với những người khác.**

Sau đó NPC chỉ là cách game “instantiate” một con người khi người chơi thực sự gặp họ.

---

<a id="m04-t011-s002"></a>

## I. Ba khái niệm phải tách riêng

<a id="m04-t011-s003"></a>

### 1. Dòng họ — Lineage

Đây là mạng huyết thống lớn.

Ví dụ:

**Họ Vạc**

có thể có 70–100 người sống rải rác tại:

Bến Vạc,

Cồn Mây,

Bến Hạc,

thậm chí Phong Châu.

Dòng họ tạo ra:

danh tiếng,

lịch sử,

nghề truyền thống,

thù truyền đời,

taboo,

bí quyết nghề nghiệp,

quyền sở hữu,

các mối hôn nhân chính trị.

---

<a id="m04-t011-s004"></a>

### 2. Gia đình — Family

Đơn vị quan hệ trực tiếp.

Ví dụ:

Cha.

Mẹ.

Ba con.

Bà ngoại.

Một người con nuôi.

Một người anh cha khác mẹ.

Những người này **không nhất thiết sống chung**.

---

<a id="m04-t011-s005"></a>

### 3. Household — Hộ

Những người thực tế sống và chia sẻ tài nguyên với nhau.

Ví dụ household có thể gồm:

một góa phụ;

hai con;

em trai của chồng đã mất;

một người học việc không có quan hệ huyết thống.

Đây là điểm cực kỳ quan trọng.

**Family ≠ Household.**

Nhờ vậy xã hội tự nhiên hơn rất nhiều.

---

<a id="m04-t011-s006"></a>

## II. FAMILY GENOME — 32 thành phần

Mỗi family khi được tạo sẽ có tối thiểu 32 trường nền.

| # | Genome | Ý nghĩa |
|---:|---|---|
| 1 | Region | vùng văn hóa |
| 2 | Settlement | nơi sinh sống |
| 3 | Lineage | dòng họ |
| 4 | Household Type | kiểu hộ |
| 5 | Household Size | số người |
| 6 | Social Status | địa vị |
| 7 | Primary Occupation | nghề chính |
| 8 | Secondary Occupation | nghề phụ |
| 9 | Resource Base | tài sản sinh tồn |
| 10 | Wealth | mức giàu nghèo |
| 11 | Prestige | uy tín |
| 12 | Political Influence | ảnh hưởng |
| 13 | Religious Standing | vị trí tín ngưỡng |
| 14 | Family Reputation | danh tiếng |
| 15 | Family Shame | điều bị xấu hổ |
| 16 | Family Pride | điều tự hào |
| 17 | Family Taboo | điều cấm |
| 18 | Traditional Skill | kỹ năng truyền |
| 19 | Family Secret | bí mật |
| 20 | Historical Trauma | biến cố quá khứ |
| 21 | Current Crisis | khủng hoảng hiện tại |
| 22 | External Ally | đồng minh |
| 23 | External Rival | đối thủ |
| 24 | Debt | món nợ |
| 25 | Favor | món ân |
| 26 | Marriage Strategy | cách nhìn hôn nhân |
| 27 | Inheritance Rule | thừa kế |
| 28 | Child Expectation | kỳ vọng với con |
| 29 | Elder Authority | quyền người già |
| 30 | Religious Practice | tín ngưỡng |
| 31 | Migration History | lịch sử di cư |
| 32 | Future Pressure | thứ có thể phá vỡ gia đình |

Chỉ riêng Family Genome đã khiến hai người cùng:

> Fisherman / Tam Giang / 25 tuổi

có thể khác hoàn toàn.

---

<a id="m04-t011-s007"></a>

## III. Household Type

Không nên mặc định mọi nhà đều:

> cha + mẹ + con.

Game cần nhiều dạng household.

| Type | Ví dụ |
|---|---|
| Hộ hạt nhân | cha mẹ + con |
| Hộ nhiều thế hệ | ông bà + cha mẹ + con |
| Hộ góa | một cha/mẹ + con |
| Hộ anh em | anh chị em sống chung |
| Hộ nghề | thợ cả + gia đình + học việc |
| Hộ thuyền | nhiều người cùng sống trên thuyền |
| Hộ dòng họ | nhiều nhánh nhỏ cùng chung khu |
| Hộ nhận nuôi | con không cùng huyết thống |
| Hộ tái hôn | con riêng nhiều phía |
| Hộ nghi lễ | Vu Chúc + học trò |
| Hộ quân sự | gia đình + quân nhân tạm trú |
| Hộ di dân | nhiều gia đình ghép lại |

Điều này rất quan trọng với procedural storytelling.

Một người có thể gọi một NPC là “anh” nhưng không cùng máu.

---

<a id="m04-t011-s008"></a>

## IV. Family Reputation không phải một số

Ví dụ dòng họ **Cấn** tại Đồng Đỏ.

Tại Lò Cốc:

> +70 Reputation.

Vì họ là thợ giỏi.

Nhưng tại Suối Than:

> -30.

Vì từng ép giá than.

Tại Phong Châu:

> +15.

Tại một cộng đồng Vu Chúc:

> -50.

Vì họ từng nấu chảy một vật nghi lễ cổ.

Vì vậy reputation luôn là:

> **Entity A → perception → Entity B**

chứ không phải:

`Reputation = 57`.

---

<a id="m04-t011-s009"></a>

## V. Social Graph cũng không dùng một thanh “relationship”

Một quan hệ phải có nhiều dimension.

Hai NPC A và B có thể có:

| Dimension | A → B |
|---|---:|
| Affection | +70 |
| Trust | +20 |
| Respect | +80 |
| Fear | +10 |
| Obligation | +60 |
| Jealousy | +45 |
| Attraction | +30 |
| Resentment | +35 |

B nhìn A lại có thể:

| Dimension | B → A |
|---|---:|
| Affection | +40 |
| Trust | +85 |
| Respect | +45 |
| Fear | 0 |
| Obligation | +10 |
| Jealousy | 0 |
| Attraction | +75 |
| Resentment | +5 |

Như vậy:

A yêu B theo kiểu người thân và rất kính trọng B.

B lại thực sự yêu A theo nghĩa tình cảm.

**Hai người cùng có relationship cao nhưng đang sống trong hai câu chuyện hoàn toàn khác nhau.**

Đây là nguồn quest rất mạnh.

---

<a id="m04-t011-s010"></a>

## VI. Một quan hệ có 5 lớp

Tôi muốn mọi edge trong Social Graph có dạng:

> **Relation = Type + Emotion + History + Obligation + Knowledge**

Ví dụ:

**Mộc → Lâm**

Type:

Anh em.

Emotion:

Yêu thương nhưng ghen tị.

History:

Mộc từng cứu Lâm khỏi chết đuối.

Obligation:

Lâm tin rằng mình nợ anh cả đời.

Knowledge:

Lâm biết một bí mật mà Mộc nghĩ không ai biết.

Ngay lập tức ta có story.

---

<a id="m04-t011-s011"></a>

## VII. Knowledge Graph nằm bên trong Social Graph

Đây là hệ thống rất đáng làm.

Không phải mọi NPC đều biết cùng một sự thật.

Ví dụ:

Cha của Mộc không chết vì tai nạn.

Thực tế:

A biết cha Mộc bị sát hại.

B nghĩ đó là tai nạn.

C biết hung thủ.

D nghĩ Mộc chính là hung thủ.

Mộc không biết gì.

Do đó “Truth” và “Belief” phải tách riêng.

Game world có:

**Objective Event**

và mỗi NPC có:

**Perceived Event.**

Điều này cho phép:

tin đồn,

hiểu nhầm,

vu oan,

tuyên truyền,

ký ức sai,

bí mật gia đình.

---

<a id="m04-t011-s012"></a>

## VIII. Memory

Character không chỉ lưu Relationship.

Họ còn lưu **Memory Event**.

Ví dụ:

> Player cho bà cụ thức ăn khi làng đói.

NPC không đơn giản nhận:

`Player Reputation +5`.

Bà cụ lưu:

**Memory**

Ai: player.

Việc gì: cho gạo.

Khi nào: mùa đói năm 8.

Context: player bản thân cũng thiếu lương thực.

Emotional weight: cực cao.

15 năm sau nếu bà vẫn sống:

bà vẫn có thể nhớ.

Con bà cũng có thể biết chuyện nếu bà kể lại.

Do đó reputation có khả năng **truyền qua social graph**.

---

<a id="m04-t011-s013"></a>

## IX. Quan hệ truyền đời

Đây là điểm cực kỳ hợp với campaign dài.

Dòng họ A từng cứu dòng họ B.

Ba thế hệ sau:

con cháu B vẫn nói:

> “Nhà ta mắc nợ người họ A.”

Nhưng một người trẻ B có thể phản ứng:

> “Đó là món nợ của ông tổ, không phải của ta.”

Ngay trong cùng một dòng họ đã có conflict giữa:

**collective obligation**

và

**individual agency**.

---

<a id="m04-t011-s014"></a>

## X. Marriage System

Hôn nhân không chỉ là romance.

Trong thế giới này nó có thể là:

tình yêu;

liên minh gia đình;

giải quyết tranh chấp;

trao đổi tài sản;

kết nối hai settlement;

hòa giải hai dòng họ;

củng cố quyền lực;

hoặc đơn giản là hai người muốn sống cùng nhau.

Game không nên khẳng định tất cả hôn nhân thời đó vận hành giống một mô hình duy nhất.

Thay vào đó từng culture có convention riêng.

---

<a id="m04-t011-s015"></a>

## XI. Marriage tạo ra Cultural Hybrid

Ví dụ:

Cha Tam Giang.

Mẹ Tản Viên.

Con có thể được sinh với:

River Knowledge mức 2.

Forest Knowledge mức 2.

Không giỏi bằng native chuyên sâu.

Nhưng có cơ hội nhận passive hybrid.

Ví dụ:

<a id="m04-t011-s016"></a>

#### Đọc Nước Qua Cây

Nhân vật nhìn vegetation ven sông để suy ra biến động mực nước trong thời gian gần.

Passive này không thể xuất hiện ở người chỉ có một culture.

Đây là cách marriage trực tiếp tạo gameplay diversity.

---

<a id="m04-t011-s017"></a>

## XII. Birth Order

Thứ tự sinh phải có tác động.

Không phải theo deterministic rule.

Nhưng nó tạo **pressure**.

Ví dụ con cả có thể:

được mong kế nghề;

phải chăm cha mẹ;

được thừa kế nhiều;

chịu áp lực danh dự.

Con giữa có thể:

ít được chú ý;

tự tìm vai trò.

Con út:

được bảo vệ;

hoặc bị coi là gánh nặng nếu gia đình nghèo.

Một character không chỉ là:

> “Con trai của X.”

Mà là:

> “Con thứ ba, nhưng hai anh trước đã chết.”

Điều đó hoàn toàn thay đổi kỳ vọng lên họ.

---

<a id="m04-t011-s018"></a>

## XIII. Adoption

Nhận nuôi phải là mechanic lớn.

Vì nó tạo character cực mạnh.

Ví dụ:

Một đứa trẻ Âu Việt được gia đình Bãi Lạc nuôi.

Nó có:

appearance/cultural origin từ một vùng.

Ngôn ngữ từ vùng khác.

Lòng trung thành với gia đình nuôi.

Nhưng biological lineage chưa biết.

Sau này khi chiến tranh nổ ra:

identity conflict xuất hiện.

Không cần “chosen one”.

Chỉ riêng đời sống con người đã đủ tạo story.

---

<a id="m04-t011-s019"></a>

## XIV. Mentor Relationship

Không phải skill nào cũng học từ level-up menu.

Một số skill cần **người dạy**.

Ví dụ:

Mộc biết chèo thuyền từ cha.

Nhưng học đọc dòng nước từ ông ngoại.

Skill Tree ghi:

**Đọc Dòng**

Source:

`Mentor: Vạc Tòng`

Nếu mentor chết:

skill vẫn còn.

Nhưng một số advanced technique có thể mất mãi nếu không kịp học.

Điều này khiến NPC có giá trị gameplay thực sự.

---

<a id="m04-t011-s020"></a>

## XV. Inheritance

Thừa kế không chỉ có item.

Có ít nhất sáu loại:

| Loại | Ví dụ |
|---|---|
| Vật chất | ruộng, thuyền, công cụ |
| Nghề | bí quyết đóng thuyền |
| Danh tiếng | con của anh hùng |
| Ô nhục | con của kẻ phản bội |
| Nghĩa vụ | nợ của cha |
| Bí mật | lời thề gia tộc |

Một character có thể “thừa kế” cả enemy.

---

<a id="m04-t011-s021"></a>

## XVI. Family Secret

Mỗi Tier A family nên có ít nhất:

**0–2 bí mật.**

Nhưng bí mật phải chia cấp.

<a id="m04-t011-s022"></a>

#### Personal Secret

Một thành viên biết.

<a id="m04-t011-s023"></a>

#### Family Secret

Nhiều người trong gia đình biết.

<a id="m04-t011-s024"></a>

#### Lineage Secret

Chỉ trưởng nhánh biết.

<a id="m04-t011-s025"></a>

#### Community Secret

Cả làng biết nhưng không nói với người ngoài.

Đây là nguồn investigation quest khổng lồ.

---

<a id="m04-t011-s026"></a>

## XVII. Life Event Engine

Đây là tầng biến family history thành character history.

Mỗi character trước khi game bắt đầu có khoảng:

**3–12 Life Events**, tùy tuổi.

Event không random độc lập.

Một event thay đổi probability của event sau.

Ví dụ:

`Father Dies`

↓

`Household Wealth -30`

↓

tăng khả năng:

`Child Begins Work Early`

↓

giảm khả năng:

`Receive Specialist Training`

↓

tăng khả năng:

`Join River Crew`

↓

có thể gặp:

`Mentor`

↓

sinh passive khác.

Một sự kiện lúc 8 tuổi có thể quyết định character ở tuổi 30.

---

<a id="m04-t011-s027"></a>

## XVIII. Một số nhóm Life Event

Ta có thể phát triển sau thành 300–500 event, nhưng framework sẽ gồm:

| Nhóm | Ví dụ |
|---|---|
| Sinh | sinh non, song sinh, con út |
| Gia đình | cha chết, mẹ tái hôn |
| Kinh tế | mất ruộng, được thừa kế |
| Thiên tai | lũ, hạn, cháy |
| Xã hội | bị đuổi khỏi làng |
| Nghề | nhận học việc |
| Bạo lực | chứng kiến giết người |
| Tình cảm | yêu, phản bội |
| Tín ngưỡng | nhìn thấy điềm |
| Di cư | chuyển settlement |
| Chiến tranh | bị bắt, nhập quân |
| Thành tựu | cứu người |
| Tội lỗi | gây ra tai nạn |
| Bí mật | phát hiện thân thế |
| Supernatural | gặp linh thể |
| Political | gia đình đổi phe |

---

<a id="m04-t011-s028"></a>

## XIX. Life Event phải có Interpretation

Hai người trải qua cùng một chuyện không thành cùng character.

Cha chết.

Character A:

> “Ta phải bảo vệ gia đình.”

Character B:

> “Thần linh đã bỏ rơi chúng ta.”

Character C:

> “Ta phải tìm kẻ chịu trách nhiệm.”

Character D:

> “Không được yêu ai quá nhiều.”

Do đó:

> **Event + Personality + Age + Context → Interpretation**

Interpretation mới thay đổi character.

---

<a id="m04-t011-s029"></a>

## XX. Personality Formation

Personality nên được hình thành dần chứ không random cuối pipeline.

Một character bắt đầu bằng một số temperament seed.

Sau mỗi formative event:

trait probability thay đổi.

Ví dụ:

Bị người thân phản bội:

Trust ↓

Suspicion ↑

Independence ↑

Nhưng nếu sau đó được một người lạ cứu:

không nhất thiết trở nên đa nghi tuyệt đối.

Có thể hình thành contradiction:

> **không tin người thân nhưng rất tin người ngoài.**

Đây là kiểu character thú vị.

---

<a id="m04-t011-s030"></a>

## XXI. Family Conflict Engine

Gia đình sống lâu phải có tension.

Một household có thể có:

**Resource tension**

Ai được ăn nhiều hơn?

**Inheritance tension**

Ai nhận thuyền?

**Marriage tension**

Con muốn cưới người ngoài.

**Occupation tension**

Con không muốn theo nghề.

**Belief tension**

Cha kính thần, con không tin.

**Political tension**

Hai anh em theo hai faction.

**Secret tension**

Một người biết sự thật.

Những thứ này tự sinh quest.

---

<a id="m04-t011-s031"></a>

## XXII. Player can enter Social Graph

Player cũng là một node.

Ví dụ player cứu con của A.

A:

Obligation +80.

Nhưng người B ghét A.

B có thể giảm Trust với player vì:

> “Kẻ đứng cùng kẻ thù cũng là kẻ thù.”

Không cần player trực tiếp làm gì B.

Social consequences propagate.

Nhưng phải có falloff để tránh cả thế giới tự động yêu/ghét player.

---

<a id="m04-t011-s032"></a>

## XXIII. Rumor Propagation

Thông tin truyền theo graph.

NPC A thấy player giết người.

A nói với B.

B nói với C.

Nhưng mỗi lần truyền có thể:

mất chi tiết;

thêm interpretation;

bị bóp méo.

Sau 5 lần:

> “Player giết một tên cướp để cứu trẻ em.”

có thể thành:

> “Có một kẻ lạ giết người ngay giữa đường.”

Đây là mechanic rất hợp world narrative.

---

<a id="m04-t011-s033"></a>

## XXIV. Social Distance

Không phải tin tức đi toàn map ngay lập tức.

Khoảng cách xã hội phụ thuộc:

địa lý;

nghề;

thương mại;

quan hệ họ hàng;

route.

Một tin ở Tam Giang có thể tới Bến Hạc rất nhanh vì thuyền.

Nhưng tới một bản sâu Tản Viên cực chậm.

Nhờ vậy:

**reputation có geography.**

---

<a id="m04-t011-s034"></a>

## XXV. Death thực sự làm graph thay đổi

NPC chết không chỉ:

`NPC.active = false`.

Ví dụ trưởng gia đình chết.

Ngay lập tức game recalculates:

Household leader.

Inheritance.

Debt holder.

Orphan status.

Marriage pressure.

Economic production.

Social influence.

Faction vote.

Có thể một cái chết duy nhất tạo ra năm quest mới.

---

<a id="m04-t011-s035"></a>

## XXVI. Sinh con

Nếu campaign kéo dài 20–25 năm, birth system rất đáng làm.

NPC có thể:

kết hôn;

có con;

con trưởng thành.

Player gặp một đứa trẻ ở Act I.

Act VI:

nó đã 19 tuổi.

Và nhớ player.

Nếu player từng cứu cha nó:

Relationship Seed tích cực.

Nếu player giết cha nó:

câu chuyện hoàn toàn khác.

---

<a id="m04-t011-s036"></a>

## XXVII. 20 FAMILY TEST CASES

Đây là batch đầu để test system.

---

<a id="m04-t011-s037"></a>

### F01 — NHÀ VẠC TÒNG

**Bến Vạc — Tam Giang**

Household: 5.

Tòng, 58 — người đưa đò.

Mộc, 31 — con trưởng.

Mai, 28 — con gái.

Lâm, 22 — con út.

Bà Nhu, 77 — mẹ Tòng.

Cha của Tòng từng sở hữu ba thuyền.

Một trận lũ phá hai chiếc.

Gia đình từ khá giả xuống trung bình.

Mộc tin thần linh chỉ là cách người ta giải thích điều mình không hiểu.

Bà Nhu cực kỳ sùng tín.

Lâm bí mật theo học người của Miếu Tam Thủy.

<a id="m04-t011-s038"></a>

#### Core Conflict

Ba thế hệ sống cùng nhà nhưng nhìn dòng sông theo ba cách hoàn toàn khác nhau.

<a id="m04-t011-s039"></a>

#### Secret

Cha Tòng không chết vì lũ.

Tòng biết.

Nhưng chưa từng nói với con.

---

<a id="m04-t011-s040"></a>

## F02 — NHÀ LẠC CẢNH

**Phong Châu**

Một nhánh nhỏ của gia đình quyền lực.

Household: 7.

Cha đang giữ một chức vụ nghi lễ.

Con cả được chuẩn bị kế vị.

Con thứ rất giỏi nhưng không được chọn.

Con gái út thân thiết với người hầu hơn anh chị.

<a id="m04-t011-s041"></a>

#### Conflict

Family Expectation vs Individual Ability.

<a id="m04-t011-s042"></a>

#### Hook

Người con thứ bắt đầu cung cấp information cho phe cải cách.

Không vì phản bội cha.

Mà vì tin mình đang cứu gia đình.

---

<a id="m04-t011-s043"></a>

## F03 — NHÀ NẬU

**Làng Dâu**

Một trong ba dòng họ sở hữu nhiều ruộng.

<a id="m04-t011-s044"></a>

#### Pride

Từng cứu cả làng khỏi mất mùa.

<a id="m04-t011-s045"></a>

#### Shame

Nguồn đất ban đầu của họ có thể đã bị chiếm từ một dòng họ khác.

<a id="m04-t011-s046"></a>

#### Conflict

Người con gái kế nghiệp muốn đào kênh mới.

Cha phản đối vì nó làm mất quyền kiểm soát nước của gia đình.

---

<a id="m04-t011-s047"></a>

## F04 — NHÀ CẤN LỬA

**Lò Cốc**

Bốn đời thợ đúc.

<a id="m04-t011-s048"></a>

#### Tradition

Con đầu phải học nghề.

<a id="m04-t011-s049"></a>

#### Problem

Con cả bị run tay.

Con thứ lại có tài thiên bẩm.

Cha giấu chuyện này vì sợ con cả mất mặt.

<a id="m04-t011-s050"></a>

#### Signature story

Không có villain.

Chỉ có:

truyền thống,

tình yêu,

xấu hổ,

tài năng.

---

<a id="m04-t011-s051"></a>

## F05 — NHÀ SƯƠNG

**Trại Sương**

Ba thế hệ thợ săn đầm.

Một thành viên bị mất thị lực.

Anh ta lại phát triển khả năng nhận dạng người bằng âm thanh cực mạnh.

<a id="m04-t011-s052"></a>

#### Passive Seed

**Người Không Cần Thấy Mặt.**

---

<a id="m04-t011-s053"></a>

## F06 — NHÀ KHE

**Khe Cao — Thượng Âu**

Gia đình đưa tin.

Cha từng phục vụ một chief.

Con trai lại chuyển thư cho phe đối địch.

Nhưng cả hai không coi mình phản bội.

Mỗi người tin rằng mình phục vụ:

> sự sống còn của bản.

---

<a id="m04-t011-s054"></a>

## F07 — NHÀ ĐÀN

**Gò Trống**

Gia đình giữ một trống nghi lễ.

<a id="m04-t011-s055"></a>

#### Taboo

Không thành viên nào được rời Phong Châu quá một mùa.

<a id="m04-t011-s056"></a>

#### Problem

Con gái lớn muốn theo một thương nhân xuống Hải Môn.

<a id="m04-t011-s057"></a>

#### Core Question

Truyền thống có quyền quyết định cuộc đời một con người đến đâu?

---

<a id="m04-t011-s058"></a>

## F08 — NHÀ VÂN

**Mũi Gió**

Cha mất trên biển.

Mẹ tiếp tục làm hoa tiêu.

Con trai sợ biển.

Con gái lại muốn đóng thuyền đi xa hơn cha từng đi.

<a id="m04-t011-s059"></a>

#### Contradiction

Người thừa kế kiến thức tốt nhất lại chính là người muốn rời quê.

---

<a id="m04-t011-s060"></a>

## F09 — NHÀ CÒ

**Gò Cò**

Gia đình làm tang lễ nhiều đời.

Người ngoài sợ họ.

Trong làng họ rất được tôn trọng.

<a id="m04-t011-s061"></a>

#### Secret

Một tổ tiên của họ từng tráo hai thi thể để tránh chiến tranh giữa hai dòng họ.

Hậu quả vẫn tồn tại tới hiện tại.

---

<a id="m04-t011-s062"></a>

## F10 — NHÀ MỘC

**Khe Mộc**

Thợ gỗ.

<a id="m04-t011-s063"></a>

#### Conflict

Phong Châu trả giá cực cao cho một cây cổ.

Nếu bán:

gia đình thoát nghèo.

Nếu giữ:

tuân luật tổ tiên.

Người cha muốn giữ.

Mẹ muốn bán.

Con lại phát hiện cây có dấu hiệu bệnh.

---

<a id="m04-t011-s064"></a>

## F11 — NHÀ BÌNH

**Đồng Sương**

Gia đình refugee.

Cha từ Bắc Lộ.

Mẹ từ Bãi Lạc.

Con đầu sinh trước khi chạy nạn.

Con út sinh tại Đồng Sương.

<a id="m04-t011-s065"></a>

#### Identity

Hai đứa trẻ cùng cha mẹ nhưng xem “quê nhà” là hai nơi khác nhau.

---

<a id="m04-t011-s066"></a>

## F12 — NHÀ TRIỀU

**Vạn Nước**

Không có đất.

Ba thế hệ sống trên thuyền.

<a id="m04-t011-s067"></a>

#### Crisis

Người con cả cưới một cô gái Bãi Lạc muốn sống trên đất.

Gia đình lần đầu phải cân nhắc:

> mua đất.

Đối với họ đây gần như thay đổi toàn bộ identity.

---

<a id="m04-t011-s068"></a>

## F13 — NHÀ THẠCH

**Mường Thạch**

Hunter lineage nổi tiếng.

<a id="m04-t011-s069"></a>

#### Shame

Con trưởng từng giết nhầm một animal được coi là linh vật.

Không ai ngoài family biết.

Kể từ đó hunting luck của họ suy giảm.

Supernatural hay psychological?

Game không xác nhận.

---

<a id="m04-t011-s070"></a>

## F14 — NHÀ HƯƠNG

**Bến Quế**

Gia đình thương nhân.

Cha nói bốn phương ngữ.

Mẹ đến từ cộng đồng phía nam.

Con cái lớn lên trong nhiều culture.

<a id="m04-t011-s071"></a>

#### Strength

Trade.

Language.

Diplomacy.

<a id="m04-t011-s072"></a>

#### Weakness

Không nơi nào hoàn toàn coi họ là “người mình”.

---

<a id="m04-t011-s073"></a>

## F15 — NHÀ NỎ

**Gò Cánh Cung**

Hai anh em làm cung.

Anh cực giỏi craft.

Em cực giỏi thử vũ khí.

<a id="m04-t011-s074"></a>

#### Future

Khi Cổ Loa phát triển, hai người trở thành một phần technology revolution.

Nhưng một người muốn làm vũ khí hàng loạt.

Người kia tin mỗi vũ khí phải được làm riêng cho chủ.

---

<a id="m04-t011-s075"></a>

## F16 — NHÀ LAU

**Bãi Lau**

Gia đình hái thuốc.

<a id="m04-t011-s076"></a>

#### Secret Knowledge

Biết một loại cây có thể cứu người nếu dùng ít nhưng gây chết người nếu dùng nhiều.

Một thành viên bắt đầu bán nó như poison.

Những người khác không biết.

---

<a id="m04-t011-s077"></a>

## F17 — NHÀ HẠC

**Bến Hạc**

Gia đình vận chuyển giàu.

<a id="m04-t011-s078"></a>

#### Public image

Đáng tin.

<a id="m04-t011-s079"></a>

#### Reality

Một nhánh đang smuggle weapon.

<a id="m04-t011-s080"></a>

#### Internal Conflict

Trưởng nhà biết.

Không đồng ý.

Nhưng tố cáo đồng nghĩa đẩy cháu mình vào chỗ chết.

---

<a id="m04-t011-s081"></a>

## F18 — NHÀ RÙA

**Đảo Rùa**

Gia đình giữ một câu chuyện truyền miệng về “con rùa mang vật lạ từ biển”.

<a id="m04-t011-s082"></a>

#### Generational split

Người già coi là lịch sử.

Người trẻ coi là chuyện trẻ con.

Vài thập kỷ sau câu chuyện này có thể nhập vào mythology lớn hơn của Âu Lạc.

---

<a id="m04-t011-s083"></a>

## F19 — NHÀ ĐÁ

**Bản Đá Chồng**

Hai nhánh chung tổ.

Hai chief candidate đều thuộc cùng lineage.

<a id="m04-t011-s084"></a>

#### Conflict

Civil war không phải giữa hai dòng họ.

Nó là **một gia đình đang tự xé đôi chính mình**.

Một người player kết bạn Act II có thể phải chiến đấu với cousin của mình Act III.

---

<a id="m04-t011-s085"></a>

## F20 — NHÀ CHẠ

**Xóm Chạ → Loa Vực**

Gia đình nông dân.

<a id="m04-t011-s086"></a>

#### Act I

Có ruộng tốt.

<a id="m04-t011-s087"></a>

#### Act III

Đất được chọn cho công trình Cổ Loa.

<a id="m04-t011-s088"></a>

#### Act IV

Phải di dời.

Cha căm ghét nhà nước mới.

Con trai lại làm công cho công trình.

Con gái kết hôn với thợ từ Đồng Đỏ.

<a id="m04-t011-s089"></a>

#### Act VI

Gia đình này trở thành microcosm của toàn bộ quá trình chuyển:

> **Văn Lang → Âu Lạc.**

---

<a id="m04-t011-s090"></a>

## XXVIII. Kết quả test

20 family trên đã tạo ra rất nhiều loại conflict mà **không cần quái vật**:

cha–con;

anh–em;

truyền thống–tài năng;

quê hương–di cư;

tín ngưỡng–hoài nghi;

nghề–cá nhân;

love–duty;

family–state;

culture–identity;

thừa kế;

bí mật;

nợ tổ tiên;

technology change.

Đây chính là điều tôi muốn.

Bởi nếu game có 1.000 nhân vật mà 80% quest của họ đều là:

> “Quái vật bắt người nhà tôi.”

thì procedural generation đã thất bại.

World này phải sinh được drama **ngay cả khi supernatural hoàn toàn biến mất**.

---

<a id="m04-t011-s091"></a>

## XXIX. Công thức Character Generator bây giờ đã hoàn chỉnh thêm một tầng

Hiện tại pipeline của chúng ta trở thành:

**Era**

→ Region

→ Settlement

→ Lineage

→ Household

→ Parents

→ Birth circumstances

→ Siblings

→ Childhood environment

→ Family expectation

→ Family reputation

→ Local history

→ Formative events

→ Mentor

→ Occupation

→ Friendship

→ Romance

→ Rivalry

→ Debt/Obligation

→ Belief

→ Fear

→ Desire

→ Contradiction

→ Secret

→ Skills

→ Signature Passive

→ Current Goal

→ Potential Arc.

Với kiến trúc này, **1.000 character chi tiết không còn là vấn đề về số lượng**.

Thách thức tiếp theo là làm mỗi người có một **cái tôi đủ rõ**.

---

<a id="m04-t011-s092"></a>

## Bước tiếp theo: CHARACTER PSYCHOLOGY BIBLE

Đây là bước tôi đề xuất làm ngay tiếp theo trước class và combat.

Chúng ta sẽ xây hệ thống personality không dựa vào vài tag như:

`brave / kind / greedy`.

Thay vào đó một character sẽ có khoảng **15–20 trục tâm lý**, bao gồm giá trị sống, ham muốn, nỗi sợ, đạo đức, thái độ với quyền lực, phản ứng khi bị stress, attachment, cách nói dối, khả năng tha thứ, sự tham vọng, nhu cầu được công nhận và đặc biệt là **Contradiction Engine**.

Sau đó kết hợp:

> **Family Genome + Life Events + Psychology**

để sinh ra **Signature Passive thực sự độc nhất**.

Đây cũng là bước mà chúng ta có thể bắt đầu lấy 20 gia đình trên và sinh ra khoảng **40–60 character hoàn chỉnh**, đủ biography, tính cách, motivation, relationship và unique passive để kiểm chứng xem generator có bắt đầu cho cảm giác “con người thật” hay chưa.

<!-- SOURCE T011 END -->

