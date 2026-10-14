# Module 08 — Nghề nghiệp và sự nghiệp

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T023](../../source/conversation.md#t023)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Profession, Career, Class là ba khái niệm riêng. Training và lao động có thời gian, điều kiện, tài nguyên và người dạy; không dùng nghề như combat background cộng điểm. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Capabilities, tools, mentor, work opportunities, household needs và contracts. |
| Đầu ra / bên sở hữu | Work plans, learning evidence, income/obligations; phối hợp Characters/Economy/Assets. |
| Đọc cùng | 09, 10, 18, 21, 27.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Sửa thuyền và vận tải tạo thu nhập/tri thức hợp lệ mà không cần giết quái. |
| Việc chưa được chứng minh | Production nghề đầy đủ; rates và anti-grind cần tuning. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T023 BEGIN -->

Ta nên tách rất rõ **“nghề nghiệp”**, **“sự nghiệp”** và **“class”**. Với game này, nghề nghiệp không chỉ là background cộng stat; nó phải là một con đường sống thực sự, có thầy, kỹ năng, thu nhập, quan hệ, uy tín, bí quyết nghề, biến cố nghề nghiệp và khả năng để lại di sản.

<a id="m08-t023-s001"></a>

## GDD MODULE 08 — PROFESSION & CAREER SYSTEM

<a id="m08-t023-s002"></a>

### 1. Design Goal

Hệ thống Profession & Career phải trả lời được bốn câu hỏi:

1. **Nhân vật đã sống bằng nghề gì trước khi người chơi điều khiển?**
2. **Hiện tại nhân vật có thể kiếm sống bằng cách nào?**
3. **Người chơi muốn biến nhân vật đó thành người như thế nào?**
4. **Sau vài chục năm, người đời nhớ nhân vật với tư cách gì?**

Nguyên tắc cốt lõi:

> **Origin quyết định điểm xuất phát nghề nghiệp.  
> Hành động của player quyết định sự nghiệp.**

Player không chọn nghề khởi đầu.

Nhưng sau khi vào game, player có toàn quyền:

- bỏ nghề;
- học nghề mới;
- học song song;
- đổi nghề vì biến cố;
- trở thành bậc thầy;
- truyền nghề;
- mở cơ sở riêng;
- gia nhập tầng lớp quyền lực;
- hoặc sống cả đời như một người làm nghề bình thường.

---

<a id="m08-t023-s003"></a>

## 2. Phân biệt Profession, Career và Class

Ba hệ thống này liên quan nhưng không được gộp.

<a id="m08-t023-s004"></a>

### Profession — Nghề nghiệp

Là **cách nhân vật tạo giá trị trong xã hội**.

Ví dụ:

- đánh cá;
- thợ đúc;
- thợ săn;
- người đưa đò;
- người làm ruộng;
- dược nhân;
- thợ đóng thuyền;
- người truyền tin;
- người giữ mộ;
- thương nhân.

Profession trả lời:

> “Người này làm gì để sống?”

---

<a id="m08-t023-s005"></a>

### Career — Sự nghiệp

Là **toàn bộ lịch sử phát triển nghề nghiệp của một character**.

Ví dụ:

> Con người đánh cá  
> → học nghề chèo thuyền  
> → trở thành người dẫn đường  
> → tham gia đoàn vận tải  
> → trở thành trưởng bến  
> → cuối đời quản lý cả tuyến sông.

Đó là một **Career Arc**.

Career trả lời:

> “Người này đã trở thành ai trong xã hội?”

---

<a id="m08-t023-s006"></a>

### Class

Là **archetype gameplay** hình thành từ cách player giải quyết vấn đề.

Ví dụ người đưa đò có thể về sau trở thành:

- Thủy Binh;
- Trinh Sát;
- Sứ Giả;
- Thương Hành;
- hoặc Vu Thủy.

Profession không khóa class.

---

<a id="m08-t023-s007"></a>

## 3. Không có Profession Level kiểu đơn giản

Không dùng:

> Fisherman Level 1  
> Fisherman Level 2  
> Fisherman Level 3.

Mỗi nghề có **6 chiều phát triển**.

| Trục | Ý nghĩa |
|---|---|
| Competence | làm nghề giỏi đến đâu |
| Knowledge | hiểu sâu bao nhiêu |
| Reputation | xã hội đánh giá thế nào |
| Network | quen biết ai trong nghề |
| Authority | có quyền quyết định tới đâu |
| Legacy | đã để lại thứ gì cho nghề |

Ví dụ:

Một thợ đúc có thể:

Competence 90

nhưng Reputation 25.

Có nghĩa:

> cực giỏi nhưng gần như không ai biết.

Một người khác:

Competence 65

Reputation 90

Authority 80.

Không phải thợ giỏi nhất nhưng đang điều hành cả xưởng lớn.

Hai career hoàn toàn khác.

---

<a id="m08-t023-s008"></a>

## 4. Career Stage

Để UI dễ hiểu, mỗi nghề vẫn có Career Stage mang tính mô tả.

<a id="m08-t023-s009"></a>

### Stage 0 — Exposure

Đã tiếp xúc nghề nhưng chưa thực sự làm.

Ví dụ:

con thợ đúc thường xuyên nhìn cha làm việc.

---

<a id="m08-t023-s010"></a>

### Stage 1 — Novice

Có thể thực hiện việc cơ bản.

---

<a id="m08-t023-s011"></a>

### Stage 2 — Practitioner

Có thể tự kiếm sống bằng nghề.

---

<a id="m08-t023-s012"></a>

### Stage 3 — Skilled

Được cộng đồng nghề công nhận.

---

<a id="m08-t023-s013"></a>

### Stage 4 — Expert

Có kỹ năng chuyên biệt hiếm.

---

<a id="m08-t023-s014"></a>

### Stage 5 — Master

Có thể:

- đào tạo người khác;
- sáng tạo kỹ thuật;
- đánh giá chuyên môn;
- nắm bí quyết.

---

<a id="m08-t023-s015"></a>

### Stage 6 — Authority

Không còn chỉ là người giỏi nghề.

Họ có ảnh hưởng đến **cả nghề**.

Ví dụ:

Master Smith ≠ Head of Smith Guild.

Một người Authority có thể quyết định:

- ai được học bí quyết;
- giá nguyên liệu;
- tiêu chuẩn sản phẩm;
- hợp đồng lớn;
- quan hệ với quyền lực.

---

<a id="m08-t023-s016"></a>

### Stage 7 — Legacy

Không phải career nào cũng đạt.

Character đã để lại:

- phương pháp;
- học trò;
- công trình;
- sản phẩm huyền thoại;
- trường phái;
- dòng nghề;
- hoặc truyền thuyết.

Character có thể chết nhưng Career Legacy vẫn tồn tại.

---

<a id="m08-t023-s017"></a>

## 5. Profession không tăng bằng XP trực tiếp

Progress nghề đến từ:

**Practice**

+

**Challenge**

+

**Mentorship**

+

**Knowledge**

+

**Real Work**

+

**Recognition**

Player không thể:

> giết 300 con sói → tăng nghề thợ đúc.

Ví dụ thợ đúc muốn tiến từ Skilled lên Expert cần:

- đủ competence;
- đã hoàn thành ít nhất vài sản phẩm khó;
- có knowledge cần thiết;
- sở hữu hoặc tiếp cận công cụ phù hợp;
- được Master hoặc cộng đồng công nhận;
- hoặc tự chứng minh bằng một thành tựu đủ lớn.

---

<a id="m08-t023-s018"></a>

## 6. Profession Experience chia thành ba loại

<a id="m08-t023-s019"></a>

### Practice Experience

Lặp lại thao tác.

Tăng kỹ năng cơ bản.

Diminishing return rất nhanh.

---

<a id="m08-t023-s020"></a>

### Challenge Experience

Làm việc vượt ngưỡng hiện tại.

Nguồn progression chính.

---

<a id="m08-t023-s021"></a>

### Insight

Hiểu được một nguyên lý mới.

Có thể đến từ:

- mentor;
- thất bại;
- nghiên cứu đồ vật;
- quan sát người khác;
- exploration;
- experimentation.

Insight dùng mở kỹ thuật mới.

Nhờ vậy grinding nghề không phải giải pháp tối ưu.

---

<a id="m08-t023-s022"></a>

## 7. Starting Profession

Player character có thể bắt đầu ở nhiều trạng thái.

<a id="m08-t023-s023"></a>

#### Không có nghề

Ví dụ trẻ mồ côi làm việc vặt.

---

<a id="m08-t023-s024"></a>

#### Đang học nghề

Ví dụ apprentice thợ đúc.

---

<a id="m08-t023-s025"></a>

#### Có nghề ổn định

Ví dụ người đánh cá 26 tuổi.

---

<a id="m08-t023-s026"></a>

#### Đã có sự nghiệp

Character random lớn tuổi có thể bắt đầu là:

- thợ săn nổi tiếng;
- trưởng bến;
- healer giỏi;
- veteran.

Điều này rất quan trọng.

Random character 16 tuổi và random character 42 tuổi không thể bắt đầu giống nhau.

---

<a id="m08-t023-s027"></a>

## 8. Tuổi khởi đầu cũng tạo gameplay khác nhau

<a id="m08-t023-s028"></a>

### Young Character

Ưu:

- nhiều thời gian phát triển;
- dễ học;
- ít obligation.

Nhược:

- ít skill;
- ít reputation;
- ít network.

---

<a id="m08-t023-s029"></a>

### Mature Character

Ưu:

- có nghề;
- có quan hệ;
- có tài sản;
- có kinh nghiệm.

Nhược:

- nhiều obligation;
- family pressure;
- career lock-in;
- ít thời gian hơn.

---

<a id="m08-t023-s030"></a>

### Older Character

Có thể bắt đầu:

- master;
- elder;
- authority.

Nhưng:

health risk cao;

legacy pressure;

succession;

student relationship.

Đây là một dạng replay khác hoàn toàn.

---

<a id="m08-t023-s031"></a>

## 9. Career Family

Thay vì viết hàng trăm nghề riêng lẻ không cấu trúc, các nghề được gom thành **Career Family**.

Tôi đề xuất 12 nhóm.

---

<a id="m08-t023-s032"></a>

## I. NÔNG SINH — Agriculture & Land

Các nghề liên quan sản xuất từ đất.

Ví dụ:

- người trồng lúa;
- người giữ giống;
- người quản nước;
- người chăn nuôi;
- người trồng cây thuốc;
- người quản kho lương.

<a id="m08-t023-s033"></a>

#### Gameplay

Food.

Season.

Weather.

Land.

Settlement politics.

<a id="m08-t023-s034"></a>

#### Career progression

Farmer

→ Skilled Grower

→ Seed Keeper / Water Keeper

→ Field Master

→ Agricultural Authority.

<a id="m08-t023-s035"></a>

#### Unique access

Đọc mùa vụ.

Đánh giá đất.

Nhận biết mất mùa sớm.

Giải quyết water conflict.

---

<a id="m08-t023-s036"></a>

## II. THỦY NGHIỆP — River & Water

Ví dụ:

- đánh cá;
- chèo thuyền;
- đưa đò;
- vận tải sông;
- tìm xác;
- thợ lặn.

<a id="m08-t023-s037"></a>

#### Gameplay

Navigation.

Weather.

River route.

Rescue.

Trade.

<a id="m08-t023-s038"></a>

#### Advanced careers

River Guide.

Master Boatman.

Route Keeper.

Harbor Elder.

---

<a id="m08-t023-s039"></a>

## III. SƠN NGHIỆP — Hunting & Wilderness

Ví dụ:

- hunter;
- tracker;
- lấy mật;
- thu dược;
- dẫn đường núi.

<a id="m08-t023-s040"></a>

#### Gameplay

Tracking.

Trapping.

Animals.

Navigation.

Ecology.

<a id="m08-t023-s041"></a>

#### Specializations

Beast Hunter.

Trail Keeper.

Mountain Guide.

Rare Resource Seeker.

---

<a id="m08-t023-s042"></a>

## IV. KIM NGHIỆP — Metallurgy

Một career family cực quan trọng.

Các nghề:

- tìm quặng;
- khai mỏ;
- luyện quặng;
- làm khuôn;
- đúc;
- sửa đồ kim loại;
- kiểm định kim loại.

Không một character mặc định làm toàn bộ pipeline.

Ví dụ:

Miner

→ Ore Assayer.

Smelter

→ Furnace Master.

Caster

→ Weapon Smith.

<a id="m08-t023-s043"></a>

#### Career importance

Đây là một trong những hệ nối trực tiếp tới:

**Nỏ thần / Cổ Loa / technology progression.**

---

<a id="m08-t023-s044"></a>

## V. MỘC & TẠO TÁC

Bao gồm:

- thợ gỗ;
- đóng thuyền;
- làm cán vũ khí;
- làm nhà;
- làm công cụ;
- chế tác sừng/xương.

<a id="m08-t023-s045"></a>

#### Specialization

Shipwright.

House Builder.

Bow Maker.

Master Carpenter.

---

<a id="m08-t023-s046"></a>

## VI. DƯỢC NGHIỆP

Không gọi tất cả là “healer”.

Có thể phân:

người hái dược;

người chế thuốc;

người trị thương;

người chăm sinh;

người biết độc;

người chữa xương.

<a id="m08-t023-s047"></a>

#### Advanced paths

Herb Master.

Poison Specialist.

Battlefield Healer.

Midwife.

Bone Setter.

<a id="m08-t023-s048"></a>

#### Gameplay

Diagnosis quan trọng hơn healing spell.

---

<a id="m08-t023-s049"></a>

## VII. THƯƠNG NGHIỆP

Bao gồm:

merchant;

carrier;

broker;

warehouse keeper;

market intermediary;

long-distance trader.

<a id="m08-t023-s050"></a>

#### Gameplay

Price information.

Routes.

Relationships.

Credit.

Debt.

Supply.

<a id="m08-t023-s051"></a>

#### Advanced career

Regional Trader.

Trade Network Owner.

Settlement Broker.

Political Merchant.

---

<a id="m08-t023-s052"></a>

## VIII. BINH NGHIỆP

Không đồng nghĩa Combat Class.

Profession là:

> làm chiến binh như một vị trí xã hội.

Có thể gồm:

guard;

militia;

professional warrior;

scout;

messenger;

unit leader;

trainer.

<a id="m08-t023-s053"></a>

#### Career progression

Recruit

→ Experienced Warrior

→ Specialist

→ Unit Leader

→ War Leader.

Một Unit Leader có thể không phải combatant mạnh nhất.

---

<a id="m08-t023-s054"></a>

## IX. NGHI LỄ & TÂM LINH

Bao gồm:

người giữ miếu;

người dẫn tang;

Vu Chúc;

người giữ trống;

người xem điềm;

người dẫn vong.

<a id="m08-t023-s055"></a>

#### Career progression

Attendant

→ Ritual Practitioner

→ Specialist

→ Ritual Keeper

→ Regional Spiritual Authority.

<a id="m08-t023-s056"></a>

#### Important rule

Spiritual profession không chứng minh supernatural tồn tại.

Một character có thể là Master Ritualist nhưng vẫn hiểu phần lớn nghi thức dưới góc độ xã hội.

---

<a id="m08-t023-s057"></a>

## X. TRI THỨC & TRUYỀN KHẨU

Thời kỳ này không được biến thành medieval scholar fantasy.

Nhóm này xoay quanh:

- người giữ genealogy;
- storyteller;
- ritual historian;
- người nhớ luật tục;
- messenger;
- interpreter;
- người ghi dấu/ký hiệu.

<a id="m08-t023-s058"></a>

#### Gameplay

Lore.

History.

Language.

Investigation.

Memory.

Negotiation.

---

<a id="m08-t023-s059"></a>

## XI. QUẢN TRỊ & QUYỀN LỰC

Không phải nghề mà character trẻ tự nhiên chọn.

Thường là Career Transition.

Ví dụ:

- trưởng hộ;
- trưởng dòng;
- trưởng bến;
- elder;
- người quản kho;
- người điều phối lao động;
- thủ lĩnh quân;
- Lạc-level authority.

<a id="m08-t023-s060"></a>

#### Required

Reputation.

Network.

Authority.

Political support.

Không tăng bằng skill point.

---

<a id="m08-t023-s061"></a>

## XII. NGOÀI LUẬT

Không gọi là Rogue Class.

Career family gồm:

- cướp sông;
- buôn lậu;
- trộm;
- dẫn đường bất hợp pháp;
- kẻ làm giả;
- gián điệp;
- sát thủ chính trị.

<a id="m08-t023-s062"></a>

#### Progression

Petty offender

→ Specialist

→ Network Member

→ Organizer

→ Underworld Authority.

<a id="m08-t023-s063"></a>

#### Important

Đây vẫn là nghề nghiệp có:

network;

reputation;

trust;

skills;

risk.

---

<a id="m08-t023-s064"></a>

## 10. Career Graph thay vì Career Tree

Không nên dùng:

```text
Fisherman
   ↓
Better Fisherman
   ↓
Master Fisherman
```

Career vận hành dưới dạng graph.

Ví dụ:

```text
Fisher
 ├→ Boatman
 │   ├→ River Guide
 │   └→ Trade Carrier
 │
 ├→ Hunter
 │
 ├→ Water Ritualist
 │
 └→ Merchant
```

Một character có thể đi ngang giữa các nghề nếu có causal bridge hợp lý.

---

<a id="m08-t023-s065"></a>

## 11. Career Transition

Đổi nghề phải có lý do.

Possible transition drivers:

<a id="m08-t023-s066"></a>

#### Voluntary

Player muốn học.

<a id="m08-t023-s067"></a>

#### Economic

Nghề không còn sống được.

<a id="m08-t023-s068"></a>

#### Injury

Không thể tiếp tục nghề cũ.

<a id="m08-t023-s069"></a>

#### Family

Phải kế nghiệp.

<a id="m08-t023-s070"></a>

#### Political

Được bổ nhiệm.

<a id="m08-t023-s071"></a>

#### War

Bị động viên.

<a id="m08-t023-s072"></a>

#### Relationship

Theo mentor/spouse.

<a id="m08-t023-s073"></a>

#### Migration

Nghề cũ không phù hợp settlement mới.

<a id="m08-t023-s074"></a>

#### Discovery

Tìm thấy talent mới.

---

<a id="m08-t023-s075"></a>

## 12. Career Transition cần Bridge

Ví dụ:

Fisher

→ Smith

không xảy ra ngay.

Cần:

access to smith;

mentor;

time;

basic skill;

workshop.

Nhưng:

Fisher

→ Boatman

Bridge thấp hơn vì skill overlap cao.

Engine sử dụng:

**Transferable Skill Score.**

---

<a id="m08-t023-s076"></a>

## 13. Transferable Skills

Nghề không sở hữu skill riêng.

Skill tồn tại độc lập.

Ví dụ:

Observation.

Balance.

Navigation.

Metalwork.

Negotiation.

Animal Knowledge.

Một nghề dùng tổ hợp skill.

Nhờ vậy career change không reset character.

Ví dụ hunter chuyển scout:

Tracking giữ nguyên.

Observation giữ nguyên.

Stealth giữ nguyên.

Chỉ cần học:

military signaling;

formation;

enemy recognition.

---

<a id="m08-t023-s077"></a>

## 14. Profession Technique

Ngoài generic skill, từng nghề có **Technique**.

Ví dụ Boatman:

Cross Current.

Emergency Mooring.

Silent Approach.

Night Navigation.

Technique chỉ học thông qua:

mentor;

practice challenge;

discovery;

career event.

Không mua bằng generic skill point.

---

<a id="m08-t023-s078"></a>

## 15. Trade Secret

Cao hơn Technique.

Không phải ai đạt Master cũng biết tất cả.

Ví dụ một lineage thợ đúc biết:

một cách xử lý khuôn đặc biệt.

Knowledge Object có owner.

Nếu character học:

Knowledge Graph cập nhật.

Nếu truyền cho người ngoài:

có thể sinh:

FamilyConflict.

ProfessionalShame.

PoliticalInterest.

IndustrialEspionage.

---

<a id="m08-t023-s079"></a>

## 16. Career Reputation

Reputation nghề tách khỏi generic reputation.

Ví dụ:

Người dân nghĩ:

> “Cấn Son kiêu ngạo.”

Personal Reputation thấp.

Nhưng thợ đúc nói:

> “Không ai hiểu lửa như hắn.”

Professional Reputation cực cao.

Khi thuê craft:

professional reputation quan trọng hơn affection.

---

<a id="m08-t023-s080"></a>

## 17. Career Network

Mỗi profession tạo mạng quan hệ riêng.

Ví dụ một merchant giỏi có:

Supplier.

Carrier.

Buyer.

Creditor.

Debtor.

Guard.

Official Contact.

Smuggler Contact.

Information Source.

Do đó career progression đồng thời là:

> **mở rộng Social Graph.**

---

<a id="m08-t023-s081"></a>

## 18. Career Obligation

Càng lên cao càng không tự do.

Novice:

ít obligation.

Master:

phải đào tạo apprentice.

Authority:

phải xử lý dispute.

War Leader:

phải bảo vệ settlement.

Ritual Keeper:

phải có mặt ngày lễ.

Career mạnh hơn nhưng cũng tạo nhiều Narrative Debt.

---

<a id="m08-t023-s082"></a>

## 19. Công việc thường nhật không biến game thành job simulator

Player không cần:

> tự tay đúc 50 cây cuốc mỗi ngày.

Routine Work được abstract.

Player chỉ trực tiếp chơi khi xuất hiện:

Challenge.

Choice.

Risk.

Rare opportunity.

Story consequence.

Ví dụ thợ đúc routine:

automatic income.

Nhưng khi:

quặng lạ xuất hiện;

khách yêu cầu weapon đặc biệt;

lò hỏng;

apprentice phạm lỗi;

thợ khác thách thức;

thì gameplay xuất hiện.

---

<a id="m08-t023-s083"></a>

## 20. Work Session

Khi player chủ động làm nghề, có thể sử dụng một Work Session.

Input:

Time.

Tools.

Materials.

Skill.

Technique.

Environment.

Output:

Product.

Income.

Experience.

Relationship.

Potential Event.

Player không phải chơi minigame vô nghĩa cho mọi sản phẩm.

---

<a id="m08-t023-s084"></a>

## 21. Crafting Profession khác Crafting System

Profession quyết định:

> người này hiểu nghề tới đâu.

Crafting System quyết định:

> món đồ cụ thể được tạo thế nào.

Một warrior biết Metalwork thấp có thể sửa spear.

Nhưng không vì thế trở thành Smith.

---

<a id="m08-t023-s085"></a>

## 22. Career Opportunity

Job không được spawn bằng notice board vô tận.

Cơ hội đến từ world.

Ví dụ:

Bến Hạc thiếu boatman.

↓

Trade tăng.

↓

Career Opportunity:

`River Carrier Needed`.

Một mentor chết.

↓

Settlement thiếu healer.

↓

Career Opportunity:

`New Apprentice`.

Cổ Loa xây dựng.

↓

Mass labor demand.

↓

Hàng loạt nghề mới có cơ hội.

Sự nghiệp player vì vậy phản ứng với lịch sử.

---

<a id="m08-t023-s086"></a>

## 23. Career Contract

Một số công việc có formal/informal contract.

Fields:

Employer.

Worker.

Task.

Compensation.

Duration.

Risk.

Obligation.

Penalty.

Reputation Stake.

Có thể:

được trả bằng gạo;

kim loại;

đất;

favor;

training;

relationship;

political protection.

Tiền không phải universal solution.

---

<a id="m08-t023-s087"></a>

## 24. Compensation System

Reward nghề có thể gồm:

<a id="m08-t023-s088"></a>

#### Material

Food.

Tools.

Metal.

Land access.

Boat share.

---

<a id="m08-t023-s089"></a>

#### Social

Reputation.

Recommendation.

Marriage eligibility.

Faction access.

---

<a id="m08-t023-s090"></a>

#### Knowledge

Technique.

Trade Secret.

Map.

Contact.

---

<a id="m08-t023-s091"></a>

#### Political

Protection.

Status.

Authority.

Career progression vì vậy không chỉ chạy theo tiền.

---

<a id="m08-t023-s092"></a>

## 25. Mentor System

Mentor có bốn vai trò:

1. truyền skill;
2. truyền technique;
3. mở network;
4. truyền reputation.

Một apprentice của Master nổi tiếng sẽ được nhìn khác.

Nhưng cũng chịu expectation cao hơn.

---

<a id="m08-t023-s093"></a>

## 26. Mentor Compatibility

Không phải Master nào cũng dạy được character.

Phụ thuộc:

Relationship.

Culture.

Personality.

Reputation.

Lineage restriction.

Political allegiance.

Player có thể cực giỏi nhưng không được nhận vì:

> “Ngươi không phải người nhà.”

Tạo quest tự nhiên.

---

<a id="m08-t023-s094"></a>

## 27. Apprentice System

Khi player đủ trình độ:

có thể nhận apprentice.

Đây là bước đầu của Legacy.

Player lựa chọn:

dạy nhiều hay ít;

giữ bí mật hay chia sẻ;

nghiêm khắc hay tự do.

Apprentice phát triển psychology dựa trên chính player.

---

<a id="m08-t023-s095"></a>

## 28. Apprentice có thể vượt Master

Không khóa NPC dưới player.

Một học trò thiên tài có thể:

học nhanh;

phát minh;

trở thành rival.

Player có thể:

tự hào;

ghen;

đàn áp;

ủng hộ.

Character arc tự sinh.

---

<a id="m08-t023-s096"></a>

## 29. Professional Rivalry

Sinh khi:

cùng resource;

cùng clientele;

cùng master;

cùng reputation niche;

hoặc philosophical disagreement.

Không nhất thiết thù cá nhân.

Ví dụ Nỏ Cần:

> weapon phải bespoke.

Nỏ Xuyên:

> standardization mới là tương lai.

Hai người có thể yêu quý nhau nhưng trở thành **professional rivals**.

---

<a id="m08-t023-s097"></a>

## 30. Innovation System

Không phải tech tree global đơn giản.

Innovation cần:

Knowledge A

+

Knowledge B

+

Need

+

Experimentation

+

Resource.

Ví dụ:

Bowmaking

+

Metal Casting

+

Military Demand

+

Repeated Experiment

→

Improved trigger mechanism.

Innovation có inventor history.

---

<a id="m08-t023-s098"></a>

## 31. Technology Diffusion

Phát minh không tự mở cho cả thế giới.

Sau innovation:

Inventor knows.

↓

Apprentices.

↓

Workshop.

↓

Settlement.

↓

Trade route.

↓

Other regions.

Player có thể:

giữ bí mật;

bán;

chia sẻ;

bị đánh cắp.

Đây là gameplay cực quan trọng quanh Cổ Loa.

---

<a id="m08-t023-s099"></a>

## 32. Career Failure

Sự nghiệp có thể đi xuống.

Ví dụ:

mất uy tín;

bị thương;

phá sản;

bị tố gian lận;

thất bại sản phẩm;

bị chính quyền cấm;

mentor scandal.

Career Stage không luôn tăng.

---

<a id="m08-t023-s100"></a>

## 33. Professional Shame

Một Master có thể mất:

Reputation

nhưng không mất Skill.

Ví dụ:

thợ giỏi bị phát hiện dùng nguyên liệu kém.

Competence 90.

Reputation:

80 → 30.

Gameplay thay đổi:

NPC không còn thuê.

Phải:

khôi phục danh dự;

rời vùng;

đổi tên;

hoặc làm cho underground market.

---

<a id="m08-t023-s101"></a>

## 34. Retirement

Character có thể:

ngừng làm nghề.

Reasons:

age;

injury;

wealth;

politics;

family;

player choice.

Retired character vẫn giữ:

knowledge;

network;

reputation.

Có thể trở thành:

mentor;

elder;

advisor.

---

<a id="m08-t023-s102"></a>

## 35. Career Legacy

Legacy được tính qua nhiều nguồn.

<a id="m08-t023-s103"></a>

#### Students

Bao nhiêu người player đào tạo.

<a id="m08-t023-s104"></a>

#### Innovation

Đã tạo gì.

<a id="m08-t023-s105"></a>

#### Institution

Có tạo workshop/network không.

<a id="m08-t023-s106"></a>

#### Cultural Impact

Kỹ thuật có trở thành tradition không.

<a id="m08-t023-s107"></a>

#### Historical Impact

Nghề có liên quan event lớn không.

---

<a id="m08-t023-s108"></a>

## 36. Profession và Main Story

Main story không chỉ dành cho chiến binh.

Ví dụ Cổ Loa.

Nếu player là:

<a id="m08-t023-s109"></a>

#### Smith

tham gia development weapon.

<a id="m08-t023-s110"></a>

#### Merchant

tổ chức supply.

<a id="m08-t023-s111"></a>

#### Farmer

đối mặt requisition và mất đất.

<a id="m08-t023-s112"></a>

#### Boatman

chuyển nguyên liệu.

<a id="m08-t023-s113"></a>

#### Ritualist

tham gia legitimacy rituals.

<a id="m08-t023-s114"></a>

#### Scout

bảo vệ tuyến đường.

<a id="m08-t023-s115"></a>

#### Political Career

quản lý lao động.

Tức là cùng một Historical Anchor nhưng gameplay khác theo career.

---

<a id="m08-t023-s116"></a>

## 37. Career-based Quest Exposure

Một số Story Thread chỉ nghề tương ứng dễ nhìn thấy.

Ví dụ:

Thợ đúc nhận ra:

hợp kim sai.

Merchant nhận ra:

giá đồng tăng bất thường.

Miner nhận ra:

quặng đang bị giấu.

Politician nhận ra:

một faction đang tích trữ weapon.

Bốn người nhìn cùng một conspiracy từ bốn hướng.

---

<a id="m08-t023-s117"></a>

## 38. Career-based Dialogue

NPC phản ứng theo nghề player.

Ví dụ Master Smith gặp player Smith:

> nói chuyên môn.

Gặp warrior:

> nói hiệu quả weapon.

Gặp merchant:

> nói giá.

Gặp Lạc official:

> nói quota.

Cùng NPC, dialogue layer khác hoàn toàn.

---

<a id="m08-t023-s118"></a>

## 39. Career Identity

Sau nhiều năm, profession trở thành một phần Identity Anchor.

Ví dụ:

> “Ta là người đóng thuyền.”

Nếu player bị thương và không thể tiếp tục:

đây không chỉ là mất skill.

Đó là **Identity Crisis**.

Narrative Engine phải phản ứng.

---

<a id="m08-t023-s119"></a>

## 40. Multi-Profession

Cho phép player có nhiều profession.

Nhưng không thể Master mọi thứ.

Giới hạn tự nhiên đến từ:

Time.

Access.

Mentor.

Obligation.

Practice.

Một đời người hữu hạn.

Đây tốt hơn hard cap.

---

<a id="m08-t023-s120"></a>

## 41. Primary / Secondary Profession

UI có thể biểu diễn:

**Primary Career**

nghề tạo identity chính.

**Secondary Profession**

kỹ năng nghề phụ.

Ví dụ:

Primary: River Guide.

Secondary: Herbalist.

Character có thể tạo passive hybrid.

---

<a id="m08-t023-s121"></a>

## 42. Hybrid Career

Nếu hai profession có synergy mạnh:

engine có thể mở title đặc biệt.

Ví dụ:

Hunter + Herbalist

→ **Sơn Dược Nhân**.

Boatman + Trader

→ **Thương Hành Sông**.

Smith + Warrior

→ **Binh Khí Sư**.

Ritualist + Historian

→ **Người Giữ Ký Ức**.

Đây có thể là nguồn Class Emergence sau này.

---

<a id="m08-t023-s122"></a>

## 43. Career Milestone

Career progression phải ghi được Chronicle.

Ví dụ:

**Năm 4**

Học nghề từ Vạc Tòng.

**Năm 7**

Tự dẫn chuyến đầu tiên.

**Năm 9**

Cứu sáu người trong lũ.

**Năm 12**

Được Hội Bảy Mái Chèo công nhận.

**Năm 17**

Trở thành trưởng tuyến.

**Năm 24**

Dạy ba apprentice.

Sự nghiệp trở thành câu chuyện.

---

<a id="m08-t023-s123"></a>

## 44. Career Ending

Khi character kết thúc đời chơi, Career Summary có thể là:

> Sinh ra là con một người đánh cá vô danh tại Cồn Mây.  
>  
> 17 tuổi trở thành người chèo thuyền.  
>  
> 24 tuổi dẫn đoàn vận tải đầu tiên tới Phong Châu.  
>  
> 31 tuổi mở lại tuyến Tam Giang sau Đại Thủy.  
>  
> 39 tuổi trở thành trưởng Hội Bảy Mái Chèo.  
>  
> 51 tuổi từ bỏ vị trí và dạy nghề.  
>  
> Khi mất, bảy bến sông thực hiện nghi thức tiễn ông.

Đó là **career fantasy** của game.

Không cần player trở thành vua.

---

<a id="m08-t023-s124"></a>

## 45. Profession không phải “content phụ”

Một trong những nguyên tắc quan trọng nhất cần ghi vào GDD:

> **Profession là một trong các con đường chính để người chơi tương tác với thế giới.**

Combat chỉ là một trong các khả năng.

Một playthrough hoàn toàn hợp lệ có thể là:

> merchant → political broker → elder.

Hoặc:

> healer → battlefield healer → spiritual leader.

Hoặc:

> smith → master → weapon innovator.

---

<a id="m08-t023-s125"></a>

## 46. Career Viability Rule

Mọi profession chính phải hỗ trợ tối thiểu:

- một loop kinh tế;
- một loại gameplay chuyên môn;
- một loại quest exposure;
- một social advantage;
- một knowledge advantage;
- một progression path;
- một failure state;
- một legacy path.

Nếu nghề chỉ cho:

`+10% Gather`

thì nghề đó chưa đủ chất lượng.

---

<a id="m08-t023-s126"></a>

## 47. Những thứ để Production phát triển sâu

Ở giai đoạn GDD **chưa cần** định nghĩa:

- tất cả 60–100 nghề cụ thể;
- toàn bộ Technique;
- XP/balance chính xác;
- thu nhập từng nghề;
- thời gian crafting;
- hàng trăm recipe;
- từng trade secret;
- danh sách mentor;
- toàn bộ innovation tree.

Production sẽ xây những phần đó dựa trên framework này.

---

<a id="m08-t023-s127"></a>

## 48. Interaction With Other Systems

Profession kết nối trực tiếp:

**Character Generation**

→ sinh profession exposure ban đầu.

**Life Event**

→ tạo cơ hội/thất bại nghề.

**Skill**

→ nghề sử dụng skill.

**Class**

→ career behaviour góp phần hình thành archetype.

**Economy**

→ nghề tạo tài nguyên và thu nhập.

**Crafting**

→ profession quyết định expertise.

**Social Graph**

→ mentor, apprentice, rival, customer.

**Reputation**

→ professional standing.

**Quest**

→ nghề mở information và solution đặc thù.

**World Simulation**

→ nhu cầu xã hội tạo job.

**Historical Anchor**

→ chiến tranh/xây Cổ Loa tạo career opportunity.

**Legacy**

→ nghề tồn tại sau cái chết của character.

---

<a id="m08-t023-s128"></a>

## 49. Quyết định cần khóa cho GDD

Hệ thống Profession & Career sử dụng các nguyên tắc sau:

1. Player **không chọn nghề xuất thân**.
2. Nghề ban đầu sinh từ biography.
3. Player được tự do thay đổi sự nghiệp sau khi bắt đầu.
4. Profession khác Class.
5. Skill tồn tại độc lập với Profession.
6. Profession dùng Career Graph, không dùng cây tuyến tính.
7. Progress bằng thực hành, thử thách, mentor và thành tựu; không chỉ XP.
8. Career có thể tăng hoặc suy giảm.
9. NPC và player sử dụng **cùng một Career System**.
10. Nghề tạo gameplay, dialogue, social access và quest access.
11. Mỗi profession phải có lợi thế ngoài combat.
12. Multi-profession được phép nhưng bị giới hạn tự nhiên bởi thời gian.
13. Knowledge và Trade Secret là object thật trong world.
14. Innovation có inventor và lan truyền theo world simulation.
15. Master có thể nhận apprentice.
16. Apprentice có thể trở thành rival hoặc successor.
17. Career tạo obligation chứ không chỉ bonus.
18. Sự nghiệp được ghi lại trong Chronicle.
19. Career Legacy có thể tồn tại sau character death.
20. Một character bình thường có thể có một career hoàn chỉnh mà không cần trở thành anh hùng.

Bước logic tiếp theo nên là **Attributes + Skills**, vì Career System cần biết chính xác một người “giỏi nghề” được cấu thành từ những năng lực nào. Sau đó ta mới thiết kế **Class Emergence**, để class xuất hiện từ tổ hợp *Attributes + Skills + Profession + cách player hành động*, thay vì player chọn class ở màn hình đầu.

<!-- SOURCE T023 END -->

