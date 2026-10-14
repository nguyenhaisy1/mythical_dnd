# Module 07 — Sinh nhân vật người chơi ngẫu nhiên

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T021](../../source/conversation.md#t021)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Không point-buy, nhập mô tả để quyết định profile hoặc chọn origin/class. Sinh biography trước năng lực cuối; reroll UX prototype được bổ sung trong ADR-008: không reroll ở Reveal; đây là quyết định biên soạn mới, không gán cho nguồn cũ. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Seed, world graph, family, aptitude, life events, current conditions. |
| Đầu ra / bên sở hữu | Playable character hợp lệ; Characters ghi qua transaction; Knowledge có provenance. |
| Đọc cùng | 04–06, 08–10, 24, 27.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Cùng seed/version tái hiện được; tất cả starting capabilities có nguồn và có con đường sống khả thi. |
| Việc chưa được chứng minh | Onboarding viability và usability cần playtest; policy prototype một actor/campaign, không Reveal reroll theo CLIENT-UX-BASELINE. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T021 BEGIN -->

Đúng, vậy tôi sẽ **đổi hẳn triết lý Character Creation**. Người chơi không “tạo” nhân vật theo nghĩa D&D truyền thống nữa, mà **được thế giới sinh ra một nhân vật hoàn chỉnh và phải sống cuộc đời của người đó**.

Tôi nghĩ quyết định này còn hợp với game hơn concept ban đầu, vì nó làm cho toàn bộ Family / Settlement / Life Event / Psychology Engine chúng ta vừa xây trở thành gameplay thật sự, thay vì chỉ phục vụ NPC.

<a id="m07-t021-s001"></a>

#### GDD Module 07 nên đổi tên thành: **Player Character Generation**

Nguyên tắc cốt lõi sẽ là:

> **Người chơi không chọn mình là ai. Người chơi chỉ quyết định mình sẽ trở thành ai từ những gì cuộc đời đã trao cho mình.**

Đây có thể trở thành một trong những Game Pillar quan trọng nhất.

Một lượt chơi mới bắt đầu bằng việc Narrative Engine chọn một character hợp lệ từ world seed, hoặc sinh mới character từ chính World Graph.

Ví dụ game có thể sinh:

> **Vạc Mân — nữ, 17 tuổi**  
> Sinh tại Cồn Mây, Tam Giang. Con thứ hai trong gia đình bốn anh chị em. Cha làm nghề đánh cá, mẹ sửa lưới và chế biến cá. Anh cả mất tích trong trận lũ sáu năm trước. Gia đình tin anh đã chết nhưng chưa tìm thấy xác. Mân biết bơi rất giỏi, ít nói, có trí nhớ tốt về đường nước nhưng sợ vùng nước sâu không nhìn thấy đáy. Cô rất thương cha nhưng âm thầm trách ông vì đã ngừng tìm anh mình.  
>
> **Starting Trait:** Cẩn trọng  
> **Flaw:** Ám ảnh nước sâu  
> **Skill Exposure:** Bơi, Đánh cá, Quan sát, Chèo thuyền  
> **Signature Passive:** *Nước Giữ Dấu* — sau khi quan sát một vùng nước từng có người hoặc vật đi qua gần đây, Mân có khả năng nhận biết một số dấu hiệu bất thường mà người khác bỏ qua.  
> **Unresolved Narrative Debt:** Anh trai mất tích.  
> **Starting Relationship:** cha +62 affection / -28 resentment; mẹ +74 affection; em trai +51 protectiveness.

Người chơi **không được đổi**:

quê quán;

giới tính;

gia đình;

tuổi;

quá khứ;

trait;

flaw;

chỉ số khởi đầu;

passive;

starting skill;

quan hệ;

social status.

Và tôi còn nghiêng về việc **không cho reroll vô hạn**.

Nếu cho reroll tới khi ra đúng build mong muốn thì cuối cùng người chơi vẫn đang chọn character, chỉ là chọn bằng slot machine.

Có ba mức có thể thiết kế:

| Chế độ | Cơ chế |
|---|---|
| Casual | xem character rồi có thể random lại |
| Standard | mỗi campaign được 3 nhân vật để chọn |
| **Core Vision** | sinh đúng 1 người và bắt đầu |
| Ironman | 1 nhân vật + hậu quả vĩnh viễn |

Với vision hiện tại, tôi chọn **Core Vision: 1 character, không chọn** làm luật mặc định.

---

<a id="m07-t021-s002"></a>

### Nhưng randomness không được là RNG mù

Đây là chỗ rất quan trọng.

Game không được làm:

`STR = random(1,20)`

`Class = random`

`Background = random`

Mà phải:

**World → Region → Settlement → Family → Birth → Childhood → Life Events → Psychology → Skill Exposure → Attributes → Passive**

Ví dụ:

Sinh trong gia đình chèo thuyền Tam Giang.

↓

từ nhỏ phụ cha.

↓

Physical Coordination tăng.

↓

Swimming exposure cao.

↓

năm 11 tuổi sống sót sau lũ.

↓

Risk Tolerance giảm.

↓

Observation tăng do luôn theo dõi mặt nước.

↓

sinh:

**DEX tốt + Water Skill cao + Deep Water Fear.**

Mọi stat đều có **causal explanation**.

---

<a id="m07-t021-s003"></a>

## Không chọn Attribute

Tôi cũng muốn sửa module Attributes theo quyết định này.

Người chơi **không phân điểm STR/DEX/INT** lúc bắt đầu.

Initial Attributes được hình thành từ:

**genetic seed**

+

**childhood nutrition**

+

**occupation**

+

**life events**

+

**training**

+

**injury**

+

**psychology**

Ví dụ character:

cha mẹ khỏe;

lớn lên thiếu ăn;

làm mỏ từ 12 tuổi;

bị thương chân năm 15 tuổi.

Có thể ra:

Strength cao.

Endurance tốt.

Agility thấp.

Pain tolerance cao.

Không có point-buy.

---

<a id="m07-t021-s004"></a>

## Không chọn Class ban đầu

Thậm chí tôi đề xuất đi xa thêm:

> **Character bắt đầu không có Class.**

Character chỉ có:

Background.

Attributes.

Skills đã biết.

Traits.

Passive.

Relationships.

Class hình thành **sau khi chơi**.

Ví dụ character sinh là:

> con thợ săn.

Có:

Bow 4.

Tracking 5.

Survival 4.

Herbalism 2.

Sau đó player chơi theo hướng:

săn thú

→ **Liệp Nhân**

hoặc:

học nghi lễ rừng

→ **Sơn Vu**

hoặc:

gia nhập quân

→ **Trinh Binh**

hoặc:

học thuốc

→ **Dược Nhân**.

Đây là distinction cực kỳ quan trọng:

<a id="m07-t021-s005"></a>

#### Origin do game quyết định.

<a id="m07-t021-s006"></a>

#### Destiny do player quyết định.

Tôi nghĩ đây phải trở thành câu slogan thiết kế nội bộ:

> **You do not choose your beginning. You choose what you become.**

---

<a id="m07-t021-s007"></a>

## Đây cũng giải quyết một vấn đề lớn của replayability

Playthrough 1:

Con nhà thợ đúc.

Playthrough 2:

Con của Vu Chúc.

Playthrough 3:

Trẻ mồ côi Bắc Lộ.

Playthrough 4:

Con gái Lạc tướng.

Playthrough 5:

Người Vạn Nước.

Playthrough 6:

Con lai Thượng Âu – Bãi Lạc.

Player buộc phải nhìn thế giới qua một lăng kính khác.

Không còn:

> “Lần nào tôi cũng chơi Rogue.”

Thậm chí một player thích combat có thể lần này sinh thành:

> một người thể chất yếu nhưng cực giỏi quan sát.

Và họ phải **thích nghi**.

Đây rất đúng tinh thần tabletop emergent storytelling.

---

<a id="m07-t021-s008"></a>

## Social Status cũng random

Player có thể sinh ra là:

con nhà nghèo;

người tự do;

học việc;

gia đình khá giả;

dòng họ có uy tín;

gia đình bị khinh;

con người quyền lực;

con nuôi;

refugee;

người ngoài cộng đồng.

Nhưng tất cả đều cần **trade-off**.

Ví dụ:

<a id="m07-t021-s009"></a>

#### Con Lạc tướng

Ưu:

network;

education;

equipment.

Nhược:

family obligation;

political enemy;

ít tự do;

high expectation.

---

<a id="m07-t021-s010"></a>

#### Con nhà đánh cá nghèo

Ưu:

tự do hơn;

survival skill;

local relationships.

Nhược:

ít tài sản;

ít education;

social influence thấp.

Không origin nào được là **objectively best origin**.

---

<a id="m07-t021-s011"></a>

## Character Generation cần Fairness Layer

Random character không có nghĩa tất cả phải mạnh bằng nhau.

Thậm chí tôi nghĩ **không nên cân bằng tuyệt đối**.

Nhưng game phải bảo đảm character:

<a id="m07-t021-s012"></a>

#### Playable

Không sinh character bất khả dụng.

<a id="m07-t021-s013"></a>

#### Interesting

Có ít nhất một strength.

<a id="m07-t021-s014"></a>

#### Problematic

Có ít nhất một weakness.

<a id="m07-t021-s015"></a>

#### Connected

Có quan hệ với world.

<a id="m07-t021-s016"></a>

#### Motivated

Có một hoặc nhiều vấn đề ban đầu.

<a id="m07-t021-s017"></a>

#### Open-ended

Không bị khóa chỉ còn một hướng build.

Tôi gọi đây là **Character Viability Gate**.

Một character chỉ được đưa cho player nếu vượt qua:

```text
1 meaningful strength
1 meaningful weakness
1 unresolved personal thread
3+ meaningful relationships
2+ skill development possibilities
1 signature passive
1 contradiction
1 possible social route
1 possible combat/survival route
```

---

<a id="m07-t021-s018"></a>

## Không random “chosen one”

Đây cũng nên là luật cứng.

Character generator không được sinh:

> hậu duệ bí mật của thần.

> người duy nhất cứu được thế giới.

> người mang huyết mạch đặc biệt bắt buộc main story.

Player có thể sinh là một người cực kỳ bình thường.

Ví dụ:

> 19 tuổi, con thứ ba của một gia đình làm ruộng.

Và chính hành động của player mới biến họ thành người được lịch sử nhớ tới.

Điều đó làm ending có trọng lượng hơn rất nhiều.

---

<a id="m07-t021-s019"></a>

## Starting Story cũng phải khác nhau

Main campaign không được mở đầu cùng một scene cho mọi character.

Thay vào đó có **Origin Entry Point**.

Ví dụ:

<a id="m07-t021-s020"></a>

#### Tam Giang

Game mở:

player đang chèo thuyền trong mưa.

<a id="m07-t021-s021"></a>

#### Đồng Đỏ

đang phụ một mẻ đúc.

<a id="m07-t021-s022"></a>

#### Phong Châu

đang tham gia đại lễ.

<a id="m07-t021-s023"></a>

#### Bắc Lộ

đang chạy khỏi một cuộc đột kích.

<a id="m07-t021-s024"></a>

#### Bãi Lạc

đang thu hoạch.

<a id="m07-t021-s025"></a>

#### Tản Viên

đang theo dấu thú.

Sau khoảng 1–3 giờ:

các origin thread bắt đầu kết nối với **Main Historical Thread**.

Như vậy lần chơi mới thực sự khác ngay từ phút đầu.

---

<a id="m07-t021-s026"></a>

## Random Character tạo Difficulty tự nhiên

Không cần tất cả playthrough có độ khó như nhau.

Một character sinh:

nhà giàu + khỏe + network tốt

sẽ có early game dễ hơn.

Nhưng political obligations rất nặng.

Một orphan:

early game khó.

Nhưng gần như không có family obligation.

Một Vu Chúc:

social/spiritual rất mạnh.

Nhưng combat kém.

Một chiến binh biên giới:

combat tốt.

Nhưng có trauma và faction enemy.

Độ khó vì vậy trở thành **một phần câu chuyện**, không chỉ enemy HP.

---

<a id="m07-t021-s027"></a>

## Death cũng trở nên thú vị hơn

Quyết định random character mở ra một khả năng rất mạnh mà ta nên để dành thiết kế sau:

Nếu player chết vĩnh viễn, chưa chắc campaign phải kết thúc.

Có thể chuyển quyền điều khiển sang:

con;

em;

companion;

apprentice;

một character khác trong Social Graph.

Ví dụ:

Bạn chơi Mộc 25 giờ.

Mộc chết trong chiến tranh.

Campaign tiếp tục bằng:

> **Lâm — em trai Mộc.**

Lâm thừa hưởng:

một phần tài sản;

một số reputation;

đồng minh;

kẻ thù;

nợ;

và quan trọng nhất:

**ký ức về Mộc.**

Từ đó game có thể trở thành không chỉ:

> câu chuyện của một hero,

mà là:

> **câu chuyện của nhiều thế hệ sống qua sự chuyển đổi Văn Lang → Âu Lạc.**

Đây là một hướng rất mạnh, nhưng chúng ta có thể khóa chi tiết sau khi tới module Death/Legacy.

---

<a id="m07-t021-s028"></a>

### Điều chỉnh roadmap GDD

Với quyết định vừa chốt, tôi sẽ sửa chuỗi thiết kế tiếp theo thành:

**Random Player Generation**

→ **Attributes được sinh từ cuộc đời**

→ **Background & Origin**

→ **Trait / Flaw / Signature Passive**

→ **Skill Exposure**

→ **Class Emergence**

→ **Skill Tree**

→ **Progression**

→ **Party**

→ **Combat**

Tức là chúng ta **không còn thiết kế Character Creation Screen kiểu RPG truyền thống**.

Screen đầu tiên có thể gần giống:

<a id="m07-t021-s029"></a>

## “Bạn sinh ra là ai?”

Sau một animation/ngắn narrative generation, game reveal từng phần:

**Năm sinh**

→ **Quê quán**

→ **Gia đình**

→ **Tuổi thơ**

→ **Biến cố**

→ **Con người hiện tại**

→ **Character Sheet**

→

**Bắt đầu cuộc đời.**

Không có nút sửa.

Tôi nghĩ quyết định này làm concept của game **đặc trưng hơn rất nhiều** và tận dụng gần như toàn bộ nền Narrative Simulation mà chúng ta đã xây.

<!-- SOURCE T021 END -->

