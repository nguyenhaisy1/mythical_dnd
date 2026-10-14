# Module 06 — Life Event và sinh diễn biến có nhân quả

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T015](../../source/conversation.md#t015)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

World pressure tạo candidate; hard eligibility trước weighting. Committed Event khác Event Template, Story Thread và Quest. Lớp LLM Presentation từng đề xuất không nằm trong baseline runtime.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | World snapshot, pressure, actor eligibility, resources, clock và knowledge routes. |
| Đầu ra / bên sở hữu | Proposed commands/jobs; commit qua Application; Narrative theo dõi thread từ facts. |
| Đọc cùng | 06A, 17, 22, 24–29.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Chạy không có player vẫn tạo chuỗi nhân quả; không spawn vật chất để hoàn thành quest. |
| Việc chưa được chứng minh | Độ phong phú thư viện và thresholds novelty cần tuning. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T015 BEGIN -->

Đúng. Đây là phần nếu làm sai thì dù có 1.000 NPC, 60 settlement và rất nhiều lore, game vẫn chỉ là một tập hợp content rời rạc. Vì vậy tôi sẽ coi **Life Event + Story Generation Engine** là tầng lõi nằm dưới quest system, character progression và cả main story.

Mục tiêu cuối cùng là:

> **Mọi chuyện xảy ra trong game đều có nguyên nhân. Mọi hậu quả đều để lại dấu vết. Mọi NPC quan trọng đều có thể giải thích được vì sao họ trở thành con người hiện tại.**

---

<a id="m06-t015-s001"></a>

## WORLD BIBLE 05 — LIFE EVENT & STORY GENERATION ENGINE

<a id="m06-t015-s002"></a>

### 1. Không có “random event” theo nghĩa thông thường

Engine không được làm như:

`5% → cháy nhà`

`10% → NPC kết hôn`

`3% → có chiến tranh`

Mỗi event bắt buộc phải có:

| Thành phần | Ý nghĩa |
|---|---|
| Cause | tại sao chuyện này có thể xảy ra |
| Preconditions | điều kiện bắt buộc |
| Pressure | điều gì đang thúc đẩy nó |
| Participants | ai liên quan |
| Trigger | cái gì khiến sự kiện bắt đầu |
| Event | chuyện gì thực sự xảy ra |
| Interpretation | mỗi người hiểu chuyện đó thế nào |
| Immediate Effects | hậu quả trực tiếp |
| Secondary Effects | hậu quả dây chuyền |
| Memories | ai ghi nhớ điều gì |
| Knowledge | ai biết sự thật |
| Rumor | người ngoài nghe phiên bản nào |
| Unresolved State | thứ gì còn dang dở |
| Future Hooks | chuyện gì có thể xảy ra tiếp |
| Historical Trace | sự kiện để lại dấu vết gì cho thế giới |

Đây là **Causal Event Model**.

---

<a id="m06-t015-s003"></a>

## 2. Kiến trúc tổng quát

Toàn bộ narrative simulation chạy theo chuỗi:

**WORLD STATE**

↓

**PRESSURE**

↓

**EVENT ELIGIBILITY**

↓

**EVENT SELECTION**

↓

**PARTICIPANT BINDING**

↓

**EVENT EXECUTION**

↓

**CHARACTER INTERPRETATION**

↓

**STATE CHANGE**

↓

**MEMORY / KNOWLEDGE / RUMOR**

↓

**UNRESOLVED CONSEQUENCE**

↓

**NEW EVENT PRESSURE**

↓

**STORY THREAD**

↓

**QUEST / CHARACTER ARC / WORLD EVENT**

Đây là vòng lặp chính của game.

---

<a id="m06-t015-s004"></a>

## 3. World State là nguyên nhân đầu tiên

Không được sinh event trực tiếp.

Engine trước hết nhìn trạng thái thế giới.

Ví dụ Bến Vạc:

| State | Giá trị |
|---|---:|
| Food | 61 |
| Wealth | 55 |
| Security | 72 |
| Health | 80 |
| Trade | 89 |
| Authority | 60 |
| Social Cohesion | 65 |
| Faith Stability | 57 |
| Environment | 41 |
| Mạch Stability | 69 |

Dòng sông đổi hướng.

Environment:

`41 → 26`

Trade:

`89 → 71`

Food:

`61 → 52`

Không có quest nào sinh ra ngay.

Thay vào đó nó tạo **Pressure**.

---

<a id="m06-t015-s005"></a>

## 4. Pressure System

Tôi đề xuất tám pressure chính.

| Pressure | Sinh ra những loại chuyện |
|---|---|
| Food Pressure | đói, trộm, di cư, tranh ruộng |
| Economic Pressure | nợ, buôn lậu, đổi nghề |
| Social Pressure | xung đột gia đình, kỳ thị |
| Political Pressure | tranh quyền, nổi loạn |
| Security Pressure | cướp, chiến tranh, phòng thủ |
| Environmental Pressure | lũ, hạn, mất rừng |
| Spiritual Pressure | taboo, nghi lễ, linh giới |
| Personal Pressure | tình yêu, tham vọng, trauma |

Pressure không nhất thiết sinh event ngay.

Ví dụ:

Food Pressure = 20

→ gần như bình thường.

Food Pressure = 60

→ giá thực phẩm tăng.

Food Pressure = 80

→ household nghèo bắt đầu thiếu ăn.

Food Pressure = 95

→ settlement có thể xảy ra:

trộm kho;

di cư hàng loạt;

bạo loạn;

bán con/học việc bắt buộc;

phá taboo môi trường;

xung đột với settlement khác.

Như vậy cùng một “mất mùa” có thể tạo hàng chục câu chuyện khác nhau tùy world state.

---

<a id="m06-t015-s006"></a>

## 5. Event Scope

Mỗi event có phạm vi.

| Scope | Ví dụ |
|---|---|
| Individual | bị thương |
| Relationship | hai người chia tay |
| Household | mất nguồn thu |
| Family | tranh thừa kế |
| Settlement | mất mùa |
| Inter-settlement | tranh nguồn nước |
| Regional | lũ lớn |
| Faction | đổi thủ lĩnh |
| World | hình thành Âu Lạc |
| Historical Anchor | Cổ Loa được xây dựng |

Điều này rất quan trọng cho propagation.

Một Individual Event không được tự nhiên làm cả vùng thay đổi.

Nhưng một Individual Event **có thể leo thang**.

Ví dụ:

Một người bị sát hại.

↓

Người đó là con chief.

↓

Dòng họ cáo buộc dòng khác.

↓

Hai family trả thù.

↓

Settlement chia phe.

↓

Faction can thiệp.

↓

Regional conflict.

Một cái chết từ Individual Scope đã leo thành Regional Story.

---

<a id="m06-t015-s007"></a>

## 6. Event State Machine

Mỗi event instance phải trải qua trạng thái rõ ràng:

**Dormant**

Chưa đủ điều kiện.

↓

**Eligible**

Đủ điều kiện nhưng chưa xảy ra.

↓

**Seeded**

Engine đã lựa chọn.

↓

**Scheduled**

Có thời điểm dự kiến.

↓

**Active**

Đang diễn ra.

↓

**Resolved**

Có kết quả chính.

↓

**Aftermath**

Hậu quả đang lan truyền.

↓

**Historical**

Không còn active nhưng trở thành quá khứ.

Ví dụ:

`Flood_00821`

Active: mùa mưa.

Resolved: nước rút.

Aftermath: đói + người mất nhà.

Historical: 5 năm sau.

Nhưng NPC vẫn nhớ:

> “Trận Đại Thủy năm đó.”

---

<a id="m06-t015-s008"></a>

## 7. Event Definition và Event Instance phải tách nhau

<a id="m06-t015-s009"></a>

### Event Definition

Là thiết kế chung:

`Flood`

<a id="m06-t015-s010"></a>

### Event Instance

Là một trận lũ cụ thể:

`Flood_BenVac_Year08`

Ví dụ Definition:

```text
EVENT_TYPE: Major_Flood

Scope:
Settlement / Region

Required:
RiverEnvironment = true

Drivers:
Rainfall
UpstreamWater
Deforestation
RiverModification
MạchInstability

Potential Effects:
CropLoss
HouseDamage
Death
Migration
Disease
TradeDisruption

MemoryTags:
Disaster
Loss
Survival

FutureHooks:
Debt
Rebuilding
Blame
Migration
ReligiousInterpretation
```

Instance:

```text
EVENT_INSTANCE:
Flood_BenVac_08

Location:
Bến Vạc

Causes:
HeavyRain
UpstreamDiversion

Participants:
392 households

Deaths:
17

Missing:
9

DestroyedBoats:
23

CropLoss:
41%

PlayerPresent:
true
```

---

<a id="m06-t015-s011"></a>

## 8. Event Selection không random hoàn toàn

Mỗi eligible event có một **Narrative Score**.

Có thể tư duy như:

```text
EventScore =
BaseWeight
× Pressure
× CharacterRelevance
× WorldRelevance
× RelationshipRelevance
× Novelty
× NarrativeNeed
× Timing
× CulturalFit
```

Ví dụ hai event cùng eligible:

**A — một người bị cảm nhẹ**

Score 5.

**B — con trưởng của gia đình đang nợ biến mất ngay trước lễ cưới**

Score 76.

Engine ưu tiên B cho Tier A simulation.

Nhưng A vẫn có thể xảy ra background.

---

<a id="m06-t015-s012"></a>

## 9. Novelty Factor

Đây là cơ chế chống game cứ kể một chuyện.

Nếu settlement vừa có ba vụ:

> mất tích.

Thì event MissingPerson tiếp theo bị giảm weight.

Nếu một character đã có:

cha chết,

mẹ chết,

anh chết,

mentor chết,

thì engine phải tránh biến character thành:

> “mọi người quanh tôi đều chết.”

trừ khi đó thực sự là deliberate character arc.

Đây là **Narrative Repetition Control**.

---

<a id="m06-t015-s013"></a>

## 10. Causal Requirement

Các event lớn cần nguyên nhân mạnh.

Ví dụ không được:

> Một chief bỗng phản Hùng Vương.

Cần chuỗi:

quyền lực suy yếu

+

đóng góp tăng

+

Phong Châu không cứu khi mất mùa

+

con chief chết ở Bắc Lộ

+

phe cải cách tác động

→

`PoliticalLoyalty -45`

↓

Eligible:

**Refuse Tribute**

↓

Nếu bị trừng phạt:

**Open Rebellion**

Như vậy phản loạn là **kết quả của lịch sử**, không phải random quest.

---

<a id="m06-t015-s014"></a>

## 11. Character Interpretation Engine

Đây có lẽ là thành phần quan trọng nhất.

Một event chỉ có **một Objective Reality**.

Nhưng có thể tạo ra 20 phiên bản trong đầu 20 người.

Công thức:

> **Interpretation = Facts Known + Personality + Existing Belief + Relationship + Trauma + Current State**

Ví dụ trận lũ giết cha của ba đứa trẻ.

<a id="m06-t015-s015"></a>

#### Người A

Spirituality cao.

> “Thủy thần đang trừng phạt chúng ta.”

Result:

Faith ↑

---

<a id="m06-t015-s016"></a>

#### Người B

Spirituality thấp.

Authority thấp.

> “Người quản nước đã thất bại.”

Result:

Anti-authority ↑

---

<a id="m06-t015-s017"></a>

#### Người C

Attachment cực cao.

> “Ta đã không cứu được cha.”

Result:

Guilt ↑

Protectiveness ↑

---

Không phải trận lũ tự tạo personality.

**Cách người đó hiểu trận lũ mới tạo personality.**

---

<a id="m06-t015-s018"></a>

## 12. Blame System

Sau event lớn, nhân vật cố tìm nguyên nhân.

Possible blame target:

Self.

Family member.

Enemy.

Leader.

Faction.

Settlement.

God.

Nature.

Chance.

Player.

Ví dụ mất mùa.

Người nông dân A:

> trời.

Người B:

> Lạc tướng.

Người C:

> làng thượng nguồn.

Người D:

> cha mình đã chọn giống sai.

Từ blame này sinh ra hành động khác nhau.

---

<a id="m06-t015-s019"></a>

## 13. Event tạo Memory

Một memory có:

```text
Event
Participants
Emotion
Importance
Interpretation
Blame
Lesson
SecretLevel
RetellingCount
Accuracy
```

Ví dụ:

> “Năm 13 tuổi, Mộc nhìn thấy cha bị nước cuốn.”

Emotion:

Fear 90.

Guilt 65.

Grief 95.

Interpretation:

“I was powerless.”

Lesson:

“Always understand water before trusting it.”

Đây chính là một phần nguồn sinh passive:

**Nước Không Nói Dối.**

---

<a id="m06-t015-s020"></a>

## 14. Memory không hoàn toàn chính xác

Memory có:

**Accuracy**

và

**Confidence**.

Hai thứ độc lập.

Một NPC có thể:

Accuracy = 35

Confidence = 95.

Tức là:

> nhớ sai nhưng cực kỳ chắc mình đúng.

Đây là nguồn investigation tuyệt vời.

---

<a id="m06-t015-s021"></a>

## 15. Năm tầng sự thật

Mỗi sự kiện quan trọng có thể tồn tại ở năm tầng.

| Layer | Ví dụ |
|---|---|
| Objective Truth | cha Mộc bị sát hại |
| Witness Knowledge | Tòng nhìn thấy hung thủ |
| Personal Belief | Mộc nghĩ cha chết do lũ |
| Public Rumor | ông chết khi cứu người |
| Historical Version | anh hùng chết giữa Đại Thủy |

Hai mươi năm sau:

phiên bản thứ năm có thể là thứ mọi người tin.

Player mới là người có khả năng tìm về lớp đầu.

---

<a id="m06-t015-s022"></a>

## 16. Rumor Engine

Tin tức không copy chính xác.

Một rumor truyền:

A → B → C → D.

Mỗi lần truyền có thể:

bỏ chi tiết;

thêm phán đoán;

thêm bias;

nhầm người;

phóng đại.

Ví dụ:

Truth:

> “Một thuyền hàng mất tích.”

Sau ba bước:

> “Cướp sông đã đánh chìm ba thuyền.”

Sau năm bước:

> “Tam Giang đang nổi loạn.”

Và tin đồn đó có thể khiến Phong Châu điều quân.

Tức là:

> **tin giả cũng có thể tạo hậu quả thật.**

---

<a id="m06-t015-s023"></a>

## 17. Story Thread

Một event đơn lẻ chưa phải story.

Story là chuỗi causal event có chung **Unresolved Core**.

Ví dụ:

```text
THREAD_ID:
FamilyDebt_017

Root:
CropFailure

UnresolvedCore:
Household cannot repay grain debt

Actors:
Family A
Creditor B

CurrentStage:
Escalation
```

Story Thread tồn tại cho đến khi:

được giải quyết;

bị hấp thụ vào thread lớn hơn;

hoặc trở thành historical unresolved issue.

---

<a id="m06-t015-s024"></a>

## 18. Story Grammar

Một story tự nhiên thường có sáu pha.

**Seed**

Một vấn đề xuất hiện.

↓

**Pressure**

Không được giải quyết.

↓

**Escalation**

Hậu quả lan ra.

↓

**Crisis**

Không thể tiếp tục như cũ.

↓

**Resolution**

Có lựa chọn/kết quả.

↓

**Aftermath**

Thế giới sống với hậu quả.

Không phải story nào cũng đi đủ sáu bước.

Một số chết từ Seed.

Một số không bao giờ Resolution.

---

<a id="m06-t015-s025"></a>

## 19. Ví dụ cực chi tiết: một gia đình từ mất mùa tới 20 năm sau

Giả sử:

**Nhà Bình ở Làng Dâu.**

Năm 1.

<a id="m06-t015-s026"></a>

### EVENT 01 — Mưa ít

Environmental Pressure ↑.

Chưa phải quest.

---

<a id="m06-t015-s027"></a>

### EVENT 02 — Mất mùa

Food:

70 → 39.

Nhà Bình mất 55% thu hoạch.

Household wealth:

42 → 27.

---

<a id="m06-t015-s028"></a>

### EVENT 03 — Vay gạo

Cha vay 12 giỏ gạo của nhà Nậu.

Relationship:

Obligation +55.

Debt created.

---

<a id="m06-t015-s029"></a>

### EVENT 04 — Không trả đúng hạn

Năm sau vụ mùa trung bình.

Nhưng debt + interest vẫn còn.

Relationship:

Trust ↓.

Resentment ↑.

---

<a id="m06-t015-s030"></a>

### EVENT 05 — Gia đình cần giảm người ăn

Con trai 13 tuổi:

Bình Hòa.

Eligible:

**Early Apprenticeship.**

Một người vận tải Tam Giang nhận Hòa làm học việc.

Cha đồng ý.

Mẹ phản đối.

Family Conflict.

---

<a id="m06-t015-s031"></a>

### EVENT 06 — Hòa rời quê

Identity:

Làng Dâu ↓.

Tam Giang exposure ↑.

Relationship với cha:

Respect -10.

Relationship với mẹ:

Attachment +20.

---

<a id="m06-t015-s032"></a>

### EVENT 07 — Gặp mentor

Ở Bến Vạc:

Vạc Mộc dạy Hòa đọc nước.

Skill:

Navigation.

Passive seed:

River.

---

<a id="m06-t015-s033"></a>

### EVENT 08 — Tai nạn trên sông

Hòa cứu một cô gái Vạn Nước.

Relationship created.

---

<a id="m06-t015-s034"></a>

### EVENT 09 — Friendship

↓

Romance.

Nhưng:

Family Bình coi người Vạn Nước là không ổn định.

Nhà cô gái coi dân ruộng là:

> “người bị đất trói.”

---

<a id="m06-t015-s035"></a>

### EVENT 10 — Marriage Conflict

Player có thể gặp câu chuyện ở đây.

Nhưng chú ý:

**quest này đã có 9 event causal trước khi player thấy nó.**

Đây là nguyên tắc quan trọng.

---

<a id="m06-t015-s036"></a>

### EVENT 11 — Kết hôn

Nếu thành công:

Hybrid Household.

Tam Giang + Bãi Lạc.

---

<a id="m06-t015-s037"></a>

### EVENT 12 — Sinh con

Năm 10.

Con gái:

Bình Lưu.

Cultural exposure:

River = 2.

Agriculture = 2.

---

<a id="m06-t015-s038"></a>

### EVENT 13 — Cha Hòa chết

Khoản nợ nhà Nậu chưa hoàn tất.

Inheritance:

Debt chuyển sang Hòa.

Nhưng Hòa sống Tam Giang.

---

<a id="m06-t015-s039"></a>

### EVENT 14 — Dispute

Nhà Nậu yêu cầu đất gia đình Bình.

Hòa phải chọn:

bán thuyền;

mất ruộng quê;

hoặc chống lại khoản nợ.

---

<a id="m06-t015-s040"></a>

### EVENT 15 — Player can intervene

Có thể:

trả nợ;

điều tra sổ nợ;

thương lượng;

giúp tăng thu nhập;

chứng minh debt đã bị tính sai;

đe dọa;

giết creditor.

Mỗi cách tạo hậu quả khác.

---

<a id="m06-t015-s041"></a>

### Năm 20

Bình Lưu đã 10 tuổi.

Nếu player giúp gia đình:

cô bé lớn lên nghe:

> “Có người lạ từng cứu nhà ta.”

Player Relationship Seed:

+30.

Nếu player ép bán ruộng:

> “Có một kẻ đã lấy quê của ông nội.”

Relationship Seed:

-40.

**Một quest của cha tạo thái độ của con 10 năm sau.**

Đây chính là narrative continuity.

---

<a id="m06-t015-s042"></a>

## 20. Unresolved State — thứ sinh story mạnh nhất

Sau mỗi event, engine hỏi:

> **Có cái gì vẫn chưa giải quyết?**

Ví dụ:

Debt unpaid.

Corpse missing.

Promise broken.

Murder unsolved.

Child missing.

Marriage forbidden.

Land disputed.

Taboo violated.

Leader illegitimate.

Artifact lost.

Spirit offended.

Inheritance unclear.

Người chơi không cần quest writer nghĩ:

> “Tạo quest gì tiếp?”

Engine nhìn các **Unresolved States**.

Đó là story seed tự nhiên.

---

<a id="m06-t015-s043"></a>

## 21. Tôi gọi chúng là Narrative Debt

Ví dụ:

Cha mất xác.

Game tạo:

`NarrativeDebt: Body_Not_Recovered`

Nó có thể nằm im 12 năm.

Sau đó:

nước rút.

Một mộ thuyền lộ ra.

Một bộ xương được tìm thấy.

Narrative debt cũ sống lại.

Quest mới xuất hiện.

Game có cảm giác:

> thế giới nhớ chuyện cũ.

---

<a id="m06-t015-s044"></a>

## 22. Event Library nên có khoảng 280 Base Events

Không cần viết 5.000 event riêng.

280 event có cấu trúc tốt có thể kết hợp thành hàng triệu lịch sử.

Tôi đề xuất phân bổ:

| Nhóm | Base Events |
|---|---:|
| Birth & Childhood | 18 |
| Family | 22 |
| Love & Marriage | 22 |
| Death & Health | 22 |
| Profession | 25 |
| Economy | 20 |
| Agriculture | 16 |
| Environment | 18 |
| Migration | 15 |
| Crime | 18 |
| Social Conflict | 20 |
| Religion | 16 |
| Supernatural | 18 |
| Politics | 18 |
| Warfare | 22 |
| Exploration | 10 |

Tổng khoảng:

**300 event definition** nếu mở rộng đầy đủ.

---

<a id="m06-t015-s045"></a>

## 23. Birth & Childhood Events

Ví dụ:

Birth.

Difficult birth.

Twin birth.

Parent absent at birth.

Child adopted.

Mother dies.

Father dies.

Family migration.

Sibling born.

Sibling death.

Child illness.

Child lost temporarily.

Child witnesses disaster.

Child begins work early.

Child receives mentor.

Child excluded by peers.

Child saves someone.

Child commits serious mistake.

Mỗi event lại có nhiều parameter.

Ví dụ:

`ChildWitnessesDeath`

không phải một story cố định.

Target có thể là:

parent;

sibling;

stranger;

enemy;

animal;

mentor.

Context thay đổi hoàn toàn psychological result.

---

<a id="m06-t015-s046"></a>

## 24. Profession Events

Career không phải chọn một lần.

Có:

First Work.

Apprenticeship.

Failed Apprenticeship.

Mentor Conflict.

Master Dies.

Occupational Injury.

Breakthrough.

Public Recognition.

Professional Shame.

Trade Secret Learned.

Trade Secret Stolen.

Forced Career Change.

Business Failure.

Resource Shortage.

Innovation.

Rival Appears.

Student Taken.

Student Betrays Mentor.

Retirement.

Return To Profession.

Nhờ đó:

một thợ đúc 50 tuổi có **career history**, không chỉ nghề hiện tại.

---

<a id="m06-t015-s047"></a>

## 25. Relationship Events

Quan hệ cũng phải có lifecycle:

First Meeting.

Shared Danger.

Favor.

Broken Promise.

Secret Shared.

Conflict.

Reconciliation.

Jealousy.

Attraction.

Rejected Love.

Hidden Love.

Marriage Proposal.

Marriage Opposition.

Separation.

Infidelity.

Forgiveness.

Permanent Estrangement.

Sacrifice.

Death of Partner.

New Attachment.

Cùng hai character có thể đi qua nhiều phase.

---

<a id="m06-t015-s048"></a>

## 26. Crime Events

Crime phải có motive.

Không random sinh “thief”.

Có thể:

Food Theft.

Debt Theft.

Revenge Theft.

Smuggling.

Illegal Hunting.

Grave Robbery.

Assault.

Murder.

Accidental Killing.

Forgery.

Fraudulent Trade.

Kidnapping.

Political Assassination.

Sabotage.

Poisoning.

Espionage.

Evidence Destruction.

False Accusation.

Quan trọng:

**crime event khác discovery event.**

Một murder có thể xảy ra năm 3.

Body được tìm năm 5.

Hung thủ bị nghi năm 6.

Truth lộ năm 12.

---

<a id="m06-t015-s049"></a>

## 27. Supernatural Event không được phá causal logic

Fantasy không đồng nghĩa random.

Ví dụ Spirit Appears phải có logic.

Có thể cần:

MạchInstability > 60

hoặc

SacredSite disturbed

hoặc

Repeated ritual

hoặc

Mass death

hoặc

Ancient artifact activated.

Không có chuyện:

> đang đi đường tự nhiên ma xuất hiện vì random encounter table.

---

<a id="m06-t015-s050"></a>

## 28. Supernatural Ambiguity

Rất nhiều supernatural event nên có ba interpretation path:

**Natural**

Có giải thích vật lý.

**Human**

Do con người gây ra.

**Supernatural**

Thực sự có linh giới.

Một số quest game **không bao giờ xác nhận hoàn toàn**.

Ví dụ cá chết.

Có thể:

nước đổi dòng;

độc từ luyện kim;

một người đầu độc;

Mạch Lạc bất ổn;

hoặc hai nguyên nhân cùng tồn tại.

---

<a id="m06-t015-s051"></a>

## 29. Political Events

Politics không chỉ dành cho king.

Có các tầng:

Household.

Lineage.

Settlement.

Faction.

Region.

World.

Ví dụ:

Elder Dies.

Leadership Vacancy.

Disputed Succession.

Vote/Consensus Failure.

Political Marriage.

Refusal of Tribute.

Tax Increase.

Faction Split.

Alliance.

Betrayal.

Rebellion.

Peace Negotiation.

Hostage Exchange.

Coup.

Amnesty.

Purge.

Centralization.

Nhờ vậy quá trình Văn Lang → Âu Lạc có thể tác động xuống từng family.

---

<a id="m06-t015-s052"></a>

## 30. War Engine phải dùng narrative state

Chiến tranh không đơn giản spawn army.

Cần:

Casus / Pressure.

Mobilization.

Supply.

Information.

Morale.

Leadership.

Terrain.

Local Support.

Sau đó chiến tranh tạo:

Recruitment.

Desertion.

Casualty.

Refugees.

Widows.

Orphans.

Missing people.

Looting.

Hero myths.

War crimes.

Land abandonment.

New elites.

20 năm sau vẫn còn hậu quả.

---

<a id="m06-t015-s053"></a>

## 31. Event Consequence Graph

Một event có thể có nhiều consequence.

Ví dụ:

<a id="m06-t015-s054"></a>

#### Battle

→ Death.

→ Injury.

→ Missing.

→ Captivity.

→ Refugee.

→ Widowhood.

→ Inheritance.

→ Labor shortage.

→ Food shortage.

→ Child starts work.

→ Apprentice migration.

→ Interregional marriage.

→ New lineage branch.

Tức là:

> **chiến tranh năm 2 có thể tạo một character sinh năm 10.**

---

<a id="m06-t015-s055"></a>

## 32. Player không phải trung tâm mọi story

Một nguyên tắc phải giữ rất mạnh.

Nếu player bỏ qua quest:

**story vẫn chạy.**

Ví dụ:

Hai dòng họ tranh nước.

Player không can thiệp.

Sau 20 ngày:

đánh nhau.

Sau 40 ngày:

một người chết.

Sau 2 tháng:

settlement chia phe.

Sau 6 tháng:

một family di cư.

Player quay lại:

thế giới đã khác.

---

<a id="m06-t015-s056"></a>

## 33. Nhưng không được biến game thành FOMO simulator

Không phải mọi quest biến mất nhanh.

Event có:

**Urgency**

và

**Persistence**.

Ví dụ:

Trẻ đang chết đuối:

Urgency cực cao.

Tranh thừa kế:

Persistence cao.

Murder mystery:

có thể tồn tại nhiều năm.

Ancient secret:

gần như không timeout.

---

<a id="m06-t015-s057"></a>

## 34. Quest không phải Story Thread

Đây là distinction rất quan trọng.

**Story Thread**

là chuyện đang xảy ra trong world.

**Quest**

là cửa sổ cho player can thiệp.

Một thread có thể:

không bao giờ trở thành quest;

sinh một quest;

sinh năm quest;

hoặc player chỉ nhìn thấy aftermath.

---

<a id="m06-t015-s058"></a>

## 35. Quest Exposure Engine

Một thread trở thành quest khi player có khả năng biết về nó.

Possible exposure:

NPC trực tiếp nhờ.

Nghe rumor.

Tìm clue.

Có relationship.

Có nghề phù hợp.

Có passive phù hợp.

Đi ngang đúng địa điểm.

Được faction giao nhiệm vụ.

Một vật phẩm dẫn tới nó.

Ví dụ:

Một người chết.

Warrior character:

chỉ thấy corpse.

Dược Nhân:

nhận ra poison.

Vu Chúc:

cảm thấy Mạch bất thường.

Thợ đúc:

nhận ra dao trên corpse không phải đồ địa phương.

**Character build quyết định player nhìn thấy story nào.**

---

<a id="m06-t015-s059"></a>

## 36. Quest Structure Generator

Một quest sinh từ Story Thread sẽ có:

| Component | Vai trò |
|---|---|
| Need | vấn đề cần giải quyết |
| Requester | ai muốn giải |
| Stakeholder | ai bị ảnh hưởng |
| Opponent | ai chống |
| Unknown | điều player chưa biết |
| Constraint | thứ giới hạn |
| Deadline | nếu có |
| Locations | nơi cần đến |
| Clues | bằng chứng |
| Solutions | cách xử lý |
| Consequences | hậu quả |

Không cần mọi quest có “enemy”.

---

<a id="m06-t015-s060"></a>

## 37. Multi-solution Rule

Major quest cần ít nhất ba hướng khả thi, ví dụ:

**Force**

Combat / intimidation.

**Social**

Negotiation / deception.

**Knowledge**

Investigation / expertise.

Ngoài ra có thể:

Craft.

Stealth.

Ritual.

Economic.

Political.

Environmental.

Một build không combat vẫn chơi được story sâu.

---

<a id="m06-t015-s061"></a>

## 38. Character Arc Engine

Character Arc không phải scripted questline.

Nó bắt đầu bằng:

**Wound**

+

**Need**

+

**Contradiction**

Ví dụ Mộc:

Wound:

không cứu được cha.

Need:

chấp nhận rằng không phải mọi thứ đều kiểm soát được.

Contradiction:

không tin thần nhưng vẫn giữ nghi lễ cha.

Engine sẽ ưu tiên những event **thách thức contradiction đó**.

---

<a id="m06-t015-s062"></a>

## 39. Arc Pressure

Nếu character quá lâu không gặp event liên quan arc:

ArcPressure tăng.

Khi đạt threshold:

engine tăng weight của các event phù hợp.

Không phải để ép story.

Mà để character không đứng yên 20 năm.

---

<a id="m06-t015-s063"></a>

## 40. Character Arc có thể có nhiều kết cục

Ví dụ Mộc gặp hiện tượng supernatural thật.

Possible outcomes:

<a id="m06-t015-s064"></a>

#### Acceptance

Anh chấp nhận rằng thế giới có thứ mình không hiểu.

<a id="m06-t015-s065"></a>

#### Denial

Anh càng cực đoan chống tín ngưỡng.

<a id="m06-t015-s066"></a>

#### Integration

Anh tin linh giới tồn tại nhưng vẫn giữ tư duy thực nghiệm.

<a id="m06-t015-s067"></a>

#### Collapse

Niềm tin về thế giới vỡ hoàn toàn.

Từ đó passive có thể tiến hóa khác nhau.

---

<a id="m06-t015-s068"></a>

## 41. Passive Evolution

Ví dụ ban đầu:

<a id="m06-t015-s069"></a>

#### Nước Không Nói Dối

Đọc hiện tượng vật lý.

Sau Arc Integration:

<a id="m06-t015-s070"></a>

#### Nước Có Hai Tiếng

Có thể phân biệt:

bất thường vật lý

và

bất thường Mạch.

Nhưng không biết supernatural anomaly là gì.

Hoặc nếu Denial:

<a id="m06-t015-s071"></a>

#### Chỉ Tin Dấu Vết

Khả năng phân tích physical clue mạnh hơn rất nhiều nhưng Spiritual check bị penalty.

Mechanic phản ánh character development.

---

<a id="m06-t015-s072"></a>

## 42. Death không được xóa Story Thread

Nếu character chết:

engine chọn **Narrative Successor**.

Có thể là:

con;

vợ/chồng;

anh em;

mentor;

học trò;

rival;

creditor;

enemy;

faction.

Ví dụ người điều tra murder chết.

Thread:

`Murder_102`

không biến mất.

Notebook/clue có thể sang học trò.

Hoặc hung thủ tưởng an toàn.

10 năm sau con người điều tra tiếp tục.

---

<a id="m06-t015-s073"></a>

## 43. Succession Score

Successor được chọn dựa trên:

Relationship.

Knowledge.

Obligation.

Emotional Weight.

Access.

Motivation.

Không phải random.

Một người con không biết gì chưa chắc là successor.

Một rival biết toàn bộ vụ án có thể phù hợp hơn.

---

<a id="m06-t015-s074"></a>

## 44. Historical Anchor System

Như đã chốt, một số sự kiện lịch sử là anchor.

Ví dụ:

**Văn Lang suy yếu.**

**Thục Phán nổi lên.**

**Âu Lạc hình thành.**

**Cổ Loa được xây dựng.**

**Xung đột dẫn tới bi kịch Mỵ Châu – Trọng Thủy.**

Anchor quy định:

**What must happen.**

Không quy định toàn bộ:

**How it happens to every person.**

Ví dụ Cổ Loa phải hình thành.

Nhưng:

nhà Chạ Lâm có bị dời hay không;

ai xây đoạn thành nào;

ai chết;

ai giàu lên;

ai trở thành thợ nỏ;

player đứng bên nào

là dynamic.

---

<a id="m06-t015-s075"></a>

## 45. Anchor Preparation

Anchor không nên xuất hiện đột ngột.

Trước đó engine tạo pressure.

Ví dụ trước khi Âu Lạc hình thành:

Political fragmentation ↑.

Security threat ↑.

Âu–Lạc contact ↑.

Hùng authority ↓.

Trade disruption ↑.

Thục faction influence ↑.

Khi anchor tới:

player cảm thấy:

> “Điều này đã được xây dựng từ rất lâu.”

Không phải cutscene nói:

> “Ba năm sau Âu Lạc ra đời.”

---

<a id="m06-t015-s076"></a>

## 46. Time Simulation

Để engine chạy khả thi, ta không mô phỏng mọi người mỗi giây.

Có nhiều cấp tick.

<a id="m06-t015-s077"></a>

#### Active Area

NPC quanh player.

Simulation chi tiết:

phút / giờ / ngày.

<a id="m06-t015-s078"></a>

#### Known Important NPC

Tier S/A.

Daily hoặc weekly tick.

<a id="m06-t015-s079"></a>

#### Settlement

Weekly / monthly.

<a id="m06-t015-s080"></a>

#### Offscreen Household

Monthly.

<a id="m06-t015-s081"></a>

#### Population

Seasonal aggregate.

<a id="m06-t015-s082"></a>

#### Historical Period

Annual.

Khi time skip nhiều năm:

engine chạy **batch simulation**, nhưng vẫn lưu các event quan trọng.

---

<a id="m06-t015-s083"></a>

## 47. Event Importance

Mỗi event có Importance.

**0–20**

Background.

**21–40**

Personal memory.

**41–60**

Character defining.

**61–80**

Community significant.

**81–100**

Historical.

Engine không cần lưu chi tiết vô hạn các event Importance 3.

Nhưng event Importance 90 phải tồn tại mãi.

---

<a id="m06-t015-s084"></a>

## 48. Event Compression

Ví dụ một farmer trải qua 12 mùa bình thường.

Không cần lưu:

12 event riêng.

Có thể compress thành:

> `Stable Farming Period: Years 4–8.`

Nhưng năm mất mùa:

lưu riêng.

Nhờ vậy simulation hàng chục nghìn người vẫn khả thi.

---

<a id="m06-t015-s085"></a>

## 49. Background Population không cần instantiate

30.000 người có thể tồn tại aggregate.

Khi cần tạo NPC:

engine nhìn lại:

Settlement state.

Household history.

Family.

Event history.

Sau đó instantiate character nhất quán với quá khứ.

Ví dụ player nói chuyện với một farmer chưa từng tồn tại trước đó.

Engine biết:

Làng này mất mùa năm 5.

Ông 46 tuổi.

Gia đình nghèo.

→ ông phải có ký ức phù hợp.

Không được sinh biography mâu thuẫn.

---

<a id="m06-t015-s086"></a>

## 50. Story Thread Merge

Hai thread có thể nhập thành một.

Ví dụ:

`FamilyDebt`

và

`ForbiddenMarriage`

Hai family chính là creditor và debtor.

Merge:

> marriage có thể xóa nợ.

Hoặc bị hiểu như:

> bán con trả nợ.

Story lập tức phức tạp hơn.

---

<a id="m06-t015-s087"></a>

## 51. Thread Collision là nguồn content cực mạnh

Ví dụ ba thread:

**SmugglingWeapons**

**FamilyHonor**

**PoliticalRebellion**

Tất cả gặp nhau tại nhà Hạc.

Ta có:

Hạc Tâm muốn giữ danh dự.

Hạc Lưu vận chuyển vũ khí.

Phe nổi loạn cần hàng.

Không cần writer tạo villain.

Conflict tự xuất hiện.

---

<a id="m06-t015-s088"></a>

## 52. Story Density Control

Không được để một làng 70 hộ có:

7 murder,

3 cult,

2 civil wars,

4 thần xuất hiện

trong cùng tháng.

Settlement có:

**Narrative Heat.**

Event lớn tăng Heat.

Heat cao:

giảm probability event lớn khác.

World cần có thời gian **thở**.

---

<a id="m06-t015-s089"></a>

## 53. Normal Life rất quan trọng

Không phải mọi event là drama.

Engine phải có:

Wedding.

Festival.

Good Harvest.

Successful Birth.

Apprentice Graduation.

Fishing Success.

House Construction.

Reconciliation.

Trade Success.

Return Home.

Community Feast.

Nếu chỉ có tragedy:

thế giới không còn gì đáng bảo vệ.

---

<a id="m06-t015-s090"></a>

## 54. Seasonal Rhythm

Events cũng phụ thuộc mùa.

Mùa cấy:

agriculture.

Mùa nước:

flood / river.

Mùa khô:

travel / construction / war.

Mùa lễ:

religious / social.

Mùa biển:

fishing / storm.

Mỗi region có seasonal calendar riêng.

Điều này khiến world cycle có cảm giác sống.

---

<a id="m06-t015-s091"></a>

## 55. Event Chain chống vô lý

Một số transition bị cấm.

Ví dụ:

Character 9 tuổi

→ military commander.

Không hợp lệ.

Người chưa từng rời Tam Giang:

→ biết rõ đường Tản Viên.

Không hợp lệ.

Gia đình vừa chết đói:

→ tự nhiên trở thành wealthy trader.

Không có causal bridge.

Một settlement bị phá hoàn toàn:

→ tháng sau population tăng 200%.

Không hợp lệ.

Engine cần **State Validation Rules**.

---

<a id="m06-t015-s092"></a>

## 56. Knowledge cũng phải hợp logic

NPC chỉ có thể biết thông tin nếu:

chứng kiến;

được kể;

đọc dấu vết;

nghề cho phép suy ra;

tình báo;

rumor;

supernatural ability.

Không có global omniscience.

Ngay cả faction leader cũng có thể nhận thông tin sai.

---

<a id="m06-t015-s093"></a>

## 57. Geography Rule

Event propagation phải đi theo network.

Tam Giang → Bến Hạc:

nhanh.

Tam Giang → Tản Viên sâu:

chậm.

Phong Châu → Bắc Lộ:

theo messenger.

War phá tuyến messenger:

information delay tăng.

Như vậy đường sá/bến/trạm tín hiệu thực sự có gameplay importance.

---

<a id="m06-t015-s094"></a>

## 58. Cultural Rule

Cùng event nhưng culture khác sẽ sinh response khác.

Ví dụ death without body.

Tam Giang:

cực kỳ nghiêm trọng về nghi lễ.

Đồng Đỏ:

có thể tập trung nhiều hơn vào inheritance/tool ownership.

Gò Cò:

có professional funerary response.

Không dùng một quest template cho mọi region.

---

<a id="m06-t015-s095"></a>

## 59. Narrative Fingerprint

Để 1.000 character không có cùng biography, mỗi người có Event Fingerprint.

Ví dụ:

```text
Migration
> ParentLoss
> EarlyWork
> Mentor
> Romance
> Debt
```

Nếu Tier A NPC khác có sequence quá giống:

Novelty penalty.

Engine ưu tiên branch khác.

Không cấm hai người cùng mất cha.

Cấm việc **cả cuộc đời họ giống nhau**.

---

<a id="m06-t015-s096"></a>

## 60. Quality Gate cho Story Thread

Một generated story quan trọng chỉ được expose cho player nếu trả lời được:

| Kiểm tra | Câu hỏi |
|---|---|
| Cause | Tại sao chuyện bắt đầu? |
| Actor | Ai thực sự muốn gì? |
| Opposition | Tại sao chưa giải quyết? |
| Stakes | Có gì để mất? |
| History | Nó liên quan quá khứ nào? |
| Choice | Player có lựa chọn thật không? |
| Consequence | Các lựa chọn khác nhau thế nào? |
| World Fit | Có hợp region/culture không? |
| Character Fit | NPC hành động có đúng tâm lý không? |
| Future | Sau quest còn hậu quả không? |

Nếu không trả lời được đủ, story bị hạ xuống background event hoặc generator tạo lại.

---

<a id="m06-t015-s097"></a>

## 61. Một ví dụ thứ hai — từ mất thuyền thành khủng hoảng chính trị

Tam Giang.

Một thuyền đồng biến mất.

**Truth:**

thuyền mắc tại nhánh sông đổi dòng.

Một số phu lấy hàng rồi bỏ chạy vì sợ bị xử phạt.

Nhưng chưa ai biết.

↓

Hội Bảy Mái Chèo nghi cướp sông.

↓

Cờ Lau bị cáo buộc.

↓

Cờ Lau phủ nhận.

↓

Một người Cờ Lau bị đánh chết.

↓

Gia đình trả thù.

↓

Thêm hai thuyền bị cướp thật.

↓

Phong Châu nhận rumor:

> Tam Giang đang nổi loạn.

↓

Điều quân xuống.

↓

Người Tam Giang coi quân xuất hiện là:

> Phong Châu muốn chiếm bến.

↓

Political Pressure tăng.

Player có thể bắt đầu quest ở bất kỳ điểm nào.

Nếu đến sớm:

tìm thuyền.

Nếu đến muộn:

điều tra murder.

Muộn hơn:

hòa giải faction.

Rất muộn:

tham gia một conflict gần như nội chiến.

**Cùng một Story Thread nhưng gameplay hoàn toàn khác dựa vào thời điểm player gặp nó.**

---

<a id="m06-t015-s098"></a>

## 62. Một ví dụ supernatural

Bãi Sậy.

Cá biến mất.

Objective causes có thể được world seed chọn:

**Cause A**

Nước đổi dòng.

**Cause B**

Pollution Đồng Đỏ.

**Cause C**

Một linh thể thực sự rời khu vực.

**Cause D**

A + C.

Dân làng nghĩ:

> thần nước giận.

Một Vu Chúc yêu cầu nghi lễ.

Một fisherman phản đối.

Một merchant muốn che chuyện pollution.

Player có thể giải quyết symptom mà không tìm truth.

Ví dụ tổ chức nghi lễ.

Dân an tâm.

Nhưng nếu cause là pollution:

cá vẫn chưa quay lại.

Quest “hoàn thành”, nhưng story chưa kết thúc.

Điều này rất quan trọng:

> **Quest completion ≠ Problem solved.**

---

<a id="m06-t015-s099"></a>

## 63. Main Story sử dụng chính engine này

Main story không được là một hệ riêng hoàn toàn.

Ví dụ:

**Ước Sơn Hà đang tan rã**

không chỉ là lore.

Nó thay đổi:

Spiritual Pressure.

Environmental anomaly.

Faction interpretation.

Ritual failure.

Regional superstition.

Political legitimacy.

Tất cả event địa phương đều có khả năng kết nối trở lại main thread.

Như vậy main story được cảm nhận khắp thế giới.

---

<a id="m06-t015-s100"></a>

## 64. Game cần một Causal Graph thật sự

Về kiến trúc dữ liệu, mỗi event instance nên lưu:

```text
event_id
event_type
timestamp
location
scope

cause_event_ids[]
cause_state_ids[]

participants[]
witnesses[]

objective_facts[]
perceived_facts_by_actor[]

immediate_effects[]
generated_memories[]
generated_rumors[]

unresolved_states[]
child_event_ids[]

story_thread_id
importance
historical_name
```

Chỉ cần cấu trúc này được giữ sạch, chúng ta luôn có thể hỏi:

> Tại sao NPC này ghét Phong Châu?

Engine có thể truy ngược:

NPC ghét Phong Châu

← cha chết tại Bắc Lộ

← unit thiếu lương

← supply route bị cắt

← Tam Giang conflict

← thuyền đồng mất tích.

Tức là một cảm xúc hiện tại có thể truy ngược qua **5 sự kiện causal**.

Đây mới thực sự là “mạch truyện”.

---

<a id="m06-t015-s101"></a>

## 65. Logic tick cơ bản

Về mặt engine có thể hình dung:

```text
1. Update World State

2. Calculate Pressures

3. Find Eligible Events

4. Reject invalid events

5. Score remaining events

6. Apply event budget / Narrative Heat

7. Bind characters and locations

8. Execute or schedule events

9. Generate interpretations

10. Apply world and relationship effects

11. Generate memories

12. Propagate knowledge and rumors

13. Create unresolved states

14. Attach events to Story Threads

15. Merge related threads

16. Evaluate quest exposure

17. Evaluate character arcs

18. Store important history
```

Đây sẽ là **heartbeat của narrative simulation**.

---

<a id="m06-t015-s102"></a>

## 66. Ba tầng generation

Để kiểm soát chất lượng, tôi không muốn AI tự do viết mọi thứ.

<a id="m06-t015-s103"></a>

#### Tier 1 — Deterministic Rules

Quyết định:

ai có thể làm gì;

điều kiện;

state;

relationship;

consequence;

timeline.

Đây là logic cứng.

<a id="m06-t015-s104"></a>

#### Tier 2 — Procedural Narrative

Quyết định:

event nào phù hợp;

thread nào merge;

character nào liên quan.

<a id="m06-t015-s105"></a>

#### Tier 3 — LLM Presentation

Chỉ viết:

dialogue;

description;

rumor;

memory narration;

quest journal.

Tức là:

> **LLM không được quyết định logic thế giới.**

LLM chỉ kể lại logic mà simulation engine đã tạo.

Đây là quyết định kiến trúc tôi rất muốn khóa ngay.

Nếu để LLM tự quyết định cả sự kiện, game sớm muộn sẽ mất consistency.

---

<a id="m06-t015-s106"></a>

## 67. Quy mô event ban đầu

Tôi nghĩ MVP narrative engine không cần ngay 300 definition.

Ta có thể đi theo:

**80 Core Events**

đủ chạy simulation.

Sau đó:

**150 Events**

đủ alpha.

Cuối cùng:

**250–300 Base Events**

cho production.

Mỗi base event lại có parameter và outcome branch.

300 definition có thể tạo ra **hàng trăm nghìn causal combinations**.

---

<a id="m06-t015-s107"></a>

## 68. Tiêu chí cuối cùng của engine

Một save chơi 70 giờ phải có khả năng xuất ra một tài liệu giống như:

> **Biên niên sử của thế giới người chơi.**

Ví dụ:

**Năm 3**

Đại thủy Tam Giang.

**Năm 4**

Nhà Vạc mất hai thuyền.

**Năm 6**

Mộc rời Bến Vạc.

**Năm 7**

Xung đột Hội Bảy Mái Chèo – Cờ Lau.

**Năm 9**

Quân Phong Châu tới Tam Giang.

**Năm 11**

Làng Dâu mở kênh mới.

**Năm 13**

Thục Phán kiểm soát Thượng Âu.

**Năm 15**

Nhà Chạ bị di dời.

**Năm 17**

Công trình Loa Vực mở rộng.

**Năm 19**

Cấn Son trở thành thợ đúc lớn.

Và nếu click vào bất kỳ dòng nào:

ta truy được:

**ai → tại sao → hậu quả → người nào bị ảnh hưởng → chuyện gì tiếp theo.**

Nếu engine đạt được điều này, chúng ta không còn chỉ có một RPG với nhiều quest.

Chúng ta đã có một **thế giới có lịch sử riêng cho từng playthrough**.

---

<a id="m06-t015-s108"></a>

### Phần nên làm tiếp theo

Ở thời điểm này tôi chưa muốn chuyển sang class hay combat. Bước hợp lý nhất là **biến framework trên thành Event Library thật sự**: tôi sẽ xây **80 Core Life Events đầu tiên** ở mức production design, mỗi event có `precondition`, `trigger`, `participants`, `outcomes`, `state changes`, `psychology effects`, `memory`, `rumor`, `follow-up events` và `quest hooks`.

Đó sẽ là bộ “gene” đầu tiên để thử chạy chuỗi lịch sử của **một settlement trong 20 năm**, sau đó chúng ta có thể kiểm chứng trực tiếp xem engine có sinh được cốt truyện logic hay vẫn còn cảm giác procedural.

<!-- SOURCE T015 END -->

