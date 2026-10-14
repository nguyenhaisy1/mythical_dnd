# Module 09 — Attributes, Skills, Knowledge và Techniques

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T025](../../source/conversation.md#t025)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Thể/Xảo/Giác/Trí/Tâm/Khí; Attribute 1–10, Skill 0–100; 2d10 là baseline. Dùng công thức và trạng thái Prototype ở Module 27 nếu ví dụ cũ khác. Knowledge không thay thế quyền vật chất.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Biography, attributes, practice evidence, technique prerequisites và domain knowledge. |
| Đầu ra / bên sở hữu | Capabilities, progress, check outcome; Characters sở hữu skill; Knowledge sở hữu thông tin. |
| Đọc cùng | 07, 08, 10, 27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Routine task đủ điều kiện không thất bại vô lý; skill cao không tự tạo công cụ hoặc quyền truy cập. |
| Việc chưa được chứng minh | Difficulty bands, growth coefficients, soft caps và phân phối kết quả. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T025 BEGIN -->

Đây là một module cần khóa rất kỹ vì nó sẽ ảnh hưởng trực tiếp đến **nghề nghiệp, class, combat, exploration, dialogue, crafting và character generation**. Tôi sẽ thiết kế theo nguyên tắc: **Attributes quyết định “con người có nền tảng gì”; Skills quyết định “con người đã học được gì”; Knowledge quyết định “họ biết gì”; Techniques quyết định “họ biết thực hiện kỹ thuật đặc biệt nào”.**

<a id="m09-t025-s001"></a>

## GDD MODULE 09 — ATTRIBUTES, SKILLS & CHARACTER CAPABILITY SYSTEM

<a id="m09-t025-s002"></a>

## 1. DESIGN GOAL

Hệ thống năng lực nhân vật phải đáp ứng đồng thời các yêu cầu:

- Player không tự phân điểm lúc tạo nhân vật.
- Chỉ số ban đầu phải giải thích được bằng biography.
- Một người giỏi nghề không nhất thiết khỏe hoặc thông minh toàn diện.
- Kỹ năng nghề phải có khả năng chuyển đổi sang nghề khác.
- Combat không được độc quyền progression.
- Social, exploration, investigation và crafting phải có chiều sâu ngang combat.
- Character yếu về thể chất vẫn phải có nhiều con đường chơi hiệu quả.
- Không tồn tại một Attribute kiểu “Charisma giải quyết mọi dialogue”.
- Không tồn tại một Skill kiểu “Survival giải quyết toàn bộ wilderness”.
- Không khuyến khích grind vô nghĩa.
- Tuổi tác, chấn thương và cuộc sống có thể thay đổi năng lực.
- NPC và player sử dụng cùng hệ thống.
- Character build phải hình thành từ cuộc đời và hành động, không từ màn hình point-buy.

---

<a id="m09-t025-s003"></a>

## 2. BỐN TẦNG NĂNG LỰC

Mọi khả năng của character được xây từ bốn tầng.

<a id="m09-t025-s004"></a>

### Tầng 1 — ATTRIBUTE

Năng lực nền tảng của con người.

Ví dụ:

sức mạnh;

khéo léo;

khả năng nhận biết;

tư duy;

ổn định tâm lý;

sức hiện diện.

Attributes thay đổi chậm.

---

<a id="m09-t025-s005"></a>

### Tầng 2 — SKILL

Kỹ năng học được.

Ví dụ:

bơi;

truy dấu;

thương lượng;

đúc kim loại;

dùng cung.

Skills có thể luyện và phát triển đáng kể.

---

<a id="m09-t025-s006"></a>

### Tầng 3 — KNOWLEDGE DOMAIN

Character biết **bối cảnh cụ thể nào**.

Ví dụ hai người đều có Survival cao.

Một người:

> Survival + Tam Giang Knowledge.

Người kia:

> Survival + Tản Viên Knowledge.

Trong đầm lầy Đại Trạch, cả hai có thể đều gặp khó khăn.

---

<a id="m09-t025-s007"></a>

### Tầng 4 — TECHNIQUE

Kỹ thuật cụ thể đã học.

Ví dụ:

Skill:

`Điều thuyền`

Technique:

`Cắt dòng ngang`

`Cập bến trong nước xiết`

`Đi thuyền không đèn`

Skill thể hiện năng lực tổng quát.

Technique mở **hành động cụ thể**.

---

<a id="m09-t025-s008"></a>

## 3. KHÔNG DÙNG STR / DEX / INT / CHA TRUYỀN THỐNG

Game sử dụng sáu Core Attributes:

<a id="m09-t025-s009"></a>

## THỂ — XẢO — GIÁC — TRÍ — TÂM — KHÍ

Đây là sáu trục nền tảng.

---

<a id="m09-t025-s010"></a>

## 4. THỂ — THỂ CHẤT

Đại diện cho:

- sức mạnh;
- sức bền;
- khả năng chịu tải;
- khả năng phục hồi;
- sức chịu đựng cơ thể.

THỂ không chỉ là “damage melee”.

Một character THỂ cao có lợi khi:

mang vật;

chèo lâu;

leo;

đào;

kéo;

vật;

chịu mệt;

lao động nặng;

đi hành quân dài;

phục hồi sau exertion.

<a id="m09-t025-s011"></a>

#### THỂ không đại diện cho:

khả năng đánh trúng;

phản xạ;

kỹ thuật chiến đấu;

khả năng chịu đau tâm lý.

---

<a id="m09-t025-s012"></a>

## 5. XẢO — KHÉO LÉO & ĐIỀU KHIỂN CƠ THỂ

XẢO đại diện cho:

- phối hợp tay mắt;
- độ chính xác;
- phản xạ;
- thăng bằng;
- thao tác tinh;
- điều khiển cơ thể.

Ứng dụng:

bắn cung;

ném lao;

né tránh;

craft tinh;

leo những vị trí khó;

giữ thăng bằng trên thuyền;

xử lý công cụ nhỏ.

Character khỏe không mặc định Xảo cao.

Một thợ đúc có thể:

THỂ 7

XẢO 4.

Một thợ khắc:

THỂ 3

XẢO 8.

---

<a id="m09-t025-s013"></a>

## 6. GIÁC — CẢM QUAN & NHẬN BIẾT

GIÁC đại diện:

- thị giác;
- thính giác;
- nhận biết thay đổi;
- khả năng tập trung vào môi trường;
- pattern recognition cảm quan.

GIÁC sử dụng trong:

tracking;

ambush detection;

investigation;

hunting;

weather observation;

đọc biểu cảm;

kiểm tra vật phẩm;

phát hiện bất thường.

GIÁC cao không có nghĩa character “thông minh hơn”.

Họ đơn giản **nhận thấy nhiều thứ hơn**.

---

<a id="m09-t025-s014"></a>

## 7. TRÍ — TƯ DUY & HỌC HỎI

TRÍ thể hiện:

- reasoning;
- khả năng liên kết thông tin;
- học quy tắc mới;
- ghi nhớ có cấu trúc;
- giải quyết vấn đề;
- suy luận.

TRÍ ảnh hưởng:

investigation;

engineering;

craft knowledge;

strategy;

language learning;

medicine;

historical reconstruction.

Một character TRÍ cao không tự động có kiến thức.

Nếu chưa bao giờ học luyện kim:

TRÍ 9

không khiến họ biết đúc đồng.

TRÍ chỉ giúp họ **học nhanh và suy luận tốt hơn**.

---

<a id="m09-t025-s015"></a>

## 8. TÂM — NỘI TÂM & HIỂU NGƯỜI

TÂM không phải “Wisdom”.

Nó kết hợp:

- self-awareness;
- emotional regulation;
- empathy;
- patience;
- capacity for emotional understanding.

Ứng dụng:

đọc người;

an ủi;

chữa trauma;

giữ bình tĩnh;

resist manipulation;

social mediation;

ritual participation;

chịu áp lực tâm lý.

Một character TÂM cao có thể hiểu rằng:

> NPC đang giận vì sợ.

Nhưng không nhất thiết biết cách thuyết phục NPC.

---

<a id="m09-t025-s016"></a>

## 9. KHÍ — Ý CHÍ & SỨC HIỆN DIỆN

KHÍ đại diện cho:

- sức ý chí;
- confidence;
- presence;
- khả năng áp đặt bản thân;
- courage under pressure;
- command presence.

Ứng dụng:

lãnh đạo;

dọa nạt;

nói trước đám đông;

giữ đội hình;

chống panic;

duy trì quyết tâm;

gây ấn tượng.

KHÍ không đồng nghĩa đẹp hoặc nói hay.

Một người ít nói vẫn có thể KHÍ rất cao.

---

<a id="m09-t025-s017"></a>

## 10. CORE ATTRIBUTE SCALE

Attributes dùng thang:

**1–10**.

Đây là thang con người bình thường, không phải level.

| Giá trị | Ý nghĩa |
|---:|---|
| 1 | suy giảm nghiêm trọng |
| 2 | rất yếu |
| 3 | dưới trung bình |
| 4 | hơi thấp |
| 5 | trung bình |
| 6 | khá |
| 7 | nổi bật |
| 8 | xuất sắc |
| 9 | cực hiếm |
| 10 | đỉnh cao tự nhiên |

Phần lớn adult population nằm khoảng:

**4–6.**

Một character có Attribute 8 đã thực sự đặc biệt.

9 cực hiếm.

10 không được random thường xuyên.

---

<a id="m09-t025-s018"></a>

## 11. KHÔNG CÓ ATTRIBUTE TOTAL CÂN BẰNG CỨNG

Không phải mọi character đều có tổng Attribute giống nhau.

Một character có thể được sinh ra:

```text
THỂ 7
XẢO 7
GIÁC 6
TRÍ 6
TÂM 5
KHÍ 6
```

Người khác:

```text
THỂ 3
XẢO 4
GIÁC 6
TRÍ 8
TÂM 7
KHÍ 3
```

Không cần ép tổng bằng nhau.

Random character **không hoàn toàn cân bằng**.

Nhưng Character Viability Gate phải đảm bảo:

> mọi character đều có ít nhất một con đường phát triển hợp lệ.

---

<a id="m09-t025-s019"></a>

## 12. ATTRIBUTE GENERATION

Initial Attribute không random đơn giản.

Công thức khái niệm:

```text
Initial Attribute =
Innate Potential
+ Childhood Development
+ Nutrition
+ Physical Lifestyle
+ Training
+ Life Events
- Injury
- Illness
± Age
```

---

<a id="m09-t025-s020"></a>

## 13. INNATE POTENTIAL

Mỗi character có hidden:

`Attribute Potential`.

Ví dụ:

Current THỂ:

5

Potential:

8.

Một character chưa từng lao động/training có thể chưa phát huy hết potential.

Player không thấy Potential chính xác.

Có thể nhận feedback:

> “Cơ thể có khả năng thích nghi tốt với lao động nặng.”

Không hiện:

`Strength Potential = 8`.

---

<a id="m09-t025-s021"></a>

## 14. ATTRIBUTE GROWTH

Attributes tăng rất chậm.

Không có:

> Level Up → +1 THỂ.

Ví dụ THỂ tăng cần thời gian:

lao động;

training;

nutrition;

recovery.

XẢO tăng:

repeated precision training.

GIÁC tăng:

observation-intensive lifestyle.

TRÍ:

learning + problem solving.

TÂM:

experience + reflection + relationships.

KHÍ:

leadership + overcoming pressure.

---

<a id="m09-t025-s022"></a>

## 15. ATTRIBUTE SOFT CAP

Potential không phải hard cap tuyệt đối.

Nhưng khi gần potential:

progress cực chậm.

Ví dụ:

Current 4 / Potential 7

→ tương đối dễ phát triển.

Current 7 / Potential 7

→ rất khó.

Current 8 / Potential 7

→ chỉ đạt được nhờ exceptional life circumstances.

---

<a id="m09-t025-s023"></a>

## 16. TUỔI TÁC

Age ảnh hưởng Attributes khác nhau.

<a id="m09-t025-s024"></a>

#### Tuổi trẻ

THỂ đang phát triển.

XẢO phát triển nhanh.

TRÍ học nhanh.

KHÍ thường chưa ổn định.

<a id="m09-t025-s025"></a>

#### Trưởng thành

Thể chất đạt đỉnh.

Skills ổn định.

Social capabilities mạnh.

<a id="m09-t025-s026"></a>

#### Trung niên

THỂ/XẢO có thể giảm nhẹ.

TRÍ/TÂM/KHÍ thường vẫn mạnh.

Knowledge tăng đáng kể.

<a id="m09-t025-s027"></a>

#### Tuổi già

Physical decline rõ hơn.

Nhưng:

knowledge;

reputation;

authority;

technique;

social network

có thể cực kỳ cao.

Do đó một Master 65 tuổi vẫn có gameplay value rất lớn.

---

<a id="m09-t025-s028"></a>

## 17. DERIVED STATS

Attributes tạo ra các chỉ số thứ cấp.

<a id="m09-t025-s029"></a>

### Sinh lực — Vitality

Phụ thuộc:

THỂ + Health + tuổi + injuries.

---

<a id="m09-t025-s030"></a>

### Thể lực — Stamina

THỂ + conditioning + profession.

Dùng cho:

combat;

travel;

labor.

---

<a id="m09-t025-s031"></a>

### Sức mang — Carry Capacity

THỂ + body condition.

---

<a id="m09-t025-s032"></a>

### Cân bằng — Balance

XẢO + relevant skill + terrain familiarity.

---

<a id="m09-t025-s033"></a>

### Phản ứng — Reaction

XẢO + GIÁC.

Dùng cho initiative và hazard response.

---

<a id="m09-t025-s034"></a>

### Nhận biết — Awareness

GIÁC + state + environment skill.

---

<a id="m09-t025-s035"></a>

### Tập trung — Focus

TRÍ + TÂM.

Dùng cho:

craft;

ritual;

investigation;

learning.

---

<a id="m09-t025-s036"></a>

### Ý chí — Resolve

TÂM + KHÍ + personality + current morale.

---

<a id="m09-t025-s037"></a>

### Hiện diện — Presence

KHÍ + Reputation + Context.

Một trưởng làng KHÍ 5 nhưng reputation rất cao vẫn có Presence lớn trong làng mình.

---

<a id="m09-t025-s038"></a>

## 18. CẢM MẠCH KHÔNG PHẢI CORE ATTRIBUTE

Game có thêm một chỉ số đặc biệt:

<a id="m09-t025-s039"></a>

## CẢM MẠCH

Nhưng nó không nằm trong sáu Core Attributes.

Phần lớn nhân vật:

`Cảm Mạch = 0`.

Một số:

1–2.

Người hiếm:

3+.

Cảm Mạch có thể xuất hiện qua:

- birth condition;
- lineage;
- spiritual exposure;
- ritual training;
- Mạch event;
- supernatural encounter.

Điều này tránh việc mọi nhân vật đều có một “Magic Stat”.

---

<a id="m09-t025-s040"></a>

## 19. CẢM MẠCH KHÔNG ĐỒNG NGHĨA PHÉP THUẬT

Cảm Mạch cao không cho phép:

> bắn fireball.

Nó cho phép:

cảm nhận abnormality;

nhận biết ritual resonance;

trải nghiệm dream;

phân biệt một số hiện tượng;

tương tác với một số spiritual entities.

Skill và Technique vẫn cần được học.

---

<a id="m09-t025-s041"></a>

## 20. SKILL SYSTEM

Attributes là nền.

Skills là thứ cuộc sống tạo ra.

Skill sử dụng thang hidden:

**0–100 Mastery.**

Player UI hiển thị theo Rank.

---

<a id="m09-t025-s042"></a>

## 21. SKILL RANK

| Rank | Mastery | Ý nghĩa |
|---|---:|---|
| Không biết | 0–9 | gần như chưa học |
| Quen việc | 10–24 | có tiếp xúc |
| Biết nghề | 25–44 | dùng được |
| Thành thạo | 45–64 | đáng tin cậy |
| Tinh thông | 65–79 | chuyên gia |
| Bậc thầy | 80–92 | master |
| Tuyệt kỹ | 93–100 | cực hiếm |

Không phải mọi Skill đều nên đạt 100.

---

<a id="m09-t025-s043"></a>

## 22. SKILL KHÔNG TĂNG BẰNG XP CHUNG

Không tồn tại:

`Character XP → Spend Skill Points`.

Skill tiến bộ thông qua:

**Exposure**

→ biết kỹ năng tồn tại.

**Practice**

→ thao tác.

**Challenge**

→ giải quyết việc khó.

**Feedback**

→ biết mình sai ở đâu.

**Insight**

→ hiểu nguyên lý.

**Mentorship**

→ học từ người giỏi.

**Application**

→ sử dụng trong tình huống thật.

---

<a id="m09-t025-s044"></a>

## 23. DIMINISHING PRACTICE

Một player không thể:

> bơi qua bơi lại 300 lần để max Bơi.

Routine practice có diminishing returns.

Ví dụ:

Mastery 12:

routine practice hữu ích.

Mastery 50:

routine gần như không tăng.

Muốn lên cao phải:

tăng challenge;

học technique;

mentor;

gặp môi trường mới.

---

<a id="m09-t025-s045"></a>

## 24. FAILURE CŨNG DẠY SKILL

Thất bại có thể tạo:

**Learning Value.**

Nhưng chỉ nếu character:

nhận biết vì sao thất bại

hoặc

có người giải thích.

Fail mà không hiểu gì:

progress rất thấp.

Do đó:

> TRÍ, mentor và observation giúp biến failure thành learning.

---

<a id="m09-t025-s046"></a>

## 25. CÁC SKILL FAMILY

GDD sử dụng 8 nhóm kỹ năng chính.

---

<a id="m09-t025-s047"></a>

## FAMILY A — THỂ CHẤT & VẬN ĐỘNG

<a id="m09-t025-s048"></a>

### A01 — SỨC BỀN

Chạy.

Chèo dài.

Hành quân.

Lao động lâu.

Chống fatigue.

Primary:

THỂ.

Secondary:

TÂM.

---

<a id="m09-t025-s049"></a>

### A02 — LEO TRÈO

Vách đá.

Cây.

Công trình.

Primary:

THỂ/XẢO tùy tình huống.

---

<a id="m09-t025-s050"></a>

### A03 — BƠI & LẶN

Primary:

THỂ.

Secondary:

XẢO.

Knowledge Domain nước ảnh hưởng rất lớn.

---

<a id="m09-t025-s051"></a>

### A04 — THĂNG BẰNG

Thuyền.

Cầu hẹp.

Địa hình trơn.

Primary:

XẢO.

---

<a id="m09-t025-s052"></a>

### A05 — VẬT & KHỐNG CHẾ

Grappling.

Kéo.

Giữ.

Khóa đối phương.

Primary:

THỂ.

Secondary:

XẢO.

---

<a id="m09-t025-s053"></a>

## FAMILY B — SINH TỒN & KHÁM PHÁ

<a id="m09-t025-s054"></a>

### B01 — ĐỊNH HƯỚNG

Hiểu phương hướng.

Landmarks.

Đọc địa hình.

Primary:

GIÁC.

Secondary:

TRÍ.

---

<a id="m09-t025-s055"></a>

### B02 — TRUY DẤU

Dấu chân.

Cành gãy.

Phân.

Bùn.

Dấu thuyền.

Primary:

GIÁC.

Secondary:

TRÍ.

---

<a id="m09-t025-s056"></a>

### B03 — SĂN & BẪY

Hiểu prey.

Đặt bẫy.

Chờ.

Tiếp cận.

Không đồng nghĩa weapon skill.

---

<a id="m09-t025-s057"></a>

### B04 — ẨN NẤP

Giảm khả năng bị phát hiện.

Dựa vào:

XẢO + GIÁC + Environment Familiarity.

---

<a id="m09-t025-s058"></a>

### B05 — TÌM KIẾM

Tìm:

người;

vật;

dấu vết;

tài nguyên.

Khác Truy Dấu ở chỗ không nhất thiết có trail.

---

<a id="m09-t025-s059"></a>

### B06 — SINH TỒN

Nước.

Lửa.

Chỗ trú.

Food safety.

Weather response.

Đây là **general survival logic**, không thay thế Knowledge Domain.

---

<a id="m09-t025-s060"></a>

### B07 — THU HÁI

Nhận biết và thu tài nguyên tự nhiên mà không phá hỏng chúng.

---

<a id="m09-t025-s061"></a>

## FAMILY C — COMBAT

<a id="m09-t025-s062"></a>

### C01 — THƯƠNG & MÂU

Đâm.

Giữ khoảng cách.

Formation use.

---

<a id="m09-t025-s063"></a>

### C02 — CUNG

Aim.

Draw control.

Range estimation.

---

<a id="m09-t025-s064"></a>

### C03 — NỎ

Load.

Aim.

Mechanism handling.

Maintenance.

Cung và Nỏ tách nhau vì technical behaviour khác biệt.

---

<a id="m09-t025-s065"></a>

### C04 — RÌU & VŨ KHÍ CHẶT

Power.

Hooking.

Armor/tool interaction.

---

<a id="m09-t025-s066"></a>

### C05 — ĐOẢN BINH

Dao.

Đoản kiếm.

Các vũ khí một tay ngắn tương tự.

---

<a id="m09-t025-s067"></a>

### C06 — KHIÊN

Block.

Guard.

Formation protection.

Protect ally.

---

<a id="m09-t025-s068"></a>

### C07 — NÉM

Lao.

Đá.

Vũ khí ném.

---

<a id="m09-t025-s069"></a>

### C08 — CHIẾN THUẬT

Không tăng damage trực tiếp.

Cho phép:

đọc đội hình;

địa hình;

thế đánh;

ý định đối thủ.

Primary:

TRÍ/GIÁC.

---

<a id="m09-t025-s070"></a>

## FAMILY D — XÃ HỘI

<a id="m09-t025-s071"></a>

### D01 — THUYẾT PHỤC

Khiến người khác đồng ý vì họ thấy lập luận hoặc lợi ích hợp lý.

Primary:

KHÍ.

Secondary phụ thuộc nội dung.

---

<a id="m09-t025-s072"></a>

### D02 — THƯƠNG LƯỢNG

Tìm thỏa thuận.

Giá.

Nợ.

Lợi ích.

Political compromise.

Primary:

TRÍ + TÂM.

---

<a id="m09-t025-s073"></a>

### D03 — DỌA NẠT

Tạo sức ép.

Không chỉ KHÍ.

Reputation, equipment, social position và context cực quan trọng.

---

<a id="m09-t025-s074"></a>

### D04 — LỪA DỐI

Tạo hoặc duy trì false belief.

Primary:

TRÍ/KHÍ.

---

<a id="m09-t025-s075"></a>

### D05 — THẤU HIỂU

Đọc cảm xúc, động cơ và contradiction.

Primary:

TÂM.

Secondary:

GIÁC.

Không phải mind reading.

---

<a id="m09-t025-s076"></a>

### D06 — LỄ TỤC

Biết cư xử đúng trong social context.

Đòi hỏi Cultural Knowledge.

Character có thể rất thuyết phục nhưng vẫn xúc phạm một cộng đồng vì không biết luật tục.

---

<a id="m09-t025-s077"></a>

### D07 — LÃNH ĐẠO

Điều phối nhiều người.

Giữ morale.

Phân việc.

Tạo niềm tin.

Primary:

KHÍ.

Secondary:

TÂM/TRÍ.

---

<a id="m09-t025-s078"></a>

## FAMILY E — THỦ CÔNG & KỸ NGHỆ

<a id="m09-t025-s079"></a>

### E01 — MỘC TÁC

Gỗ.

Nhà.

Công cụ.

Thuyền ở mức basic construction.

---

<a id="m09-t025-s080"></a>

### E02 — LUYỆN KIM

Lò.

Quặng.

Nhiệt.

Hợp kim.

---

<a id="m09-t025-s081"></a>

### E03 — ĐÚC & LÀM KHUÔN

Tách khỏi luyện kim.

Một người có thể giỏi furnace nhưng không giỏi mold.

---

<a id="m09-t025-s082"></a>

### E04 — CHẾ TÁC TINH

Trang sức.

Khắc.

Đồ nghi lễ.

Các thao tác chính xác.

---

<a id="m09-t025-s083"></a>

### E05 — SỬA CHỮA

Đánh giá damage.

Phục hồi tool.

Salvage.

Skill này transferable rất cao.

---

<a id="m09-t025-s084"></a>

### E06 — ĐÓNG THUYỀN

Profession-heavy Skill.

Sử dụng:

Mộc tác + Water Knowledge.

---

<a id="m09-t025-s085"></a>

### E07 — XÂY DỰNG

Nhà.

Công trình đất.

Hào.

Kè.

Công trình lớn.

---

<a id="m09-t025-s086"></a>

## FAMILY F — DƯỢC & CHĂM SÓC

<a id="m09-t025-s087"></a>

### F01 — THẢO DƯỢC

Nhận biết.

Thu.

Bảo quản.

Phối dược liệu.

---

<a id="m09-t025-s088"></a>

### F02 — CHẨN ĐOÁN

Nhận biết nguyên nhân bệnh/chấn thương.

Primary:

GIÁC + TRÍ.

---

<a id="m09-t025-s089"></a>

### F03 — TRỊ THƯƠNG

Vết thương.

Cầm máu.

Băng bó.

Chăm sóc.

---

<a id="m09-t025-s090"></a>

### F04 — CHỈNH XƯƠNG

Chấn thương cơ-xương.

---

<a id="m09-t025-s091"></a>

### F05 — ĐỘC HỌC

Poison identification.

Dosage.

Antidote knowledge.

---

<a id="m09-t025-s092"></a>

### F06 — CHĂM SINH

Pregnancy.

Birth.

Postpartum care.

Không gộp vào generic healing.

---

<a id="m09-t025-s093"></a>

## FAMILY G — TRI THỨC & ĐIỀU TRA

<a id="m09-t025-s094"></a>

### G01 — QUAN SÁT

Không phải GIÁC.

GIÁC = năng lực cảm quan.

Quan sát = biết **nhìn vào đâu**.

Một người GIÁC cao nhưng chưa học investigation có thể thấy nhiều nhưng không biết chi tiết nào quan trọng.

---

<a id="m09-t025-s095"></a>

### G02 — SUY LUẬN

Nối clue.

Loại bỏ khả năng.

Primary:

TRÍ.

---

<a id="m09-t025-s096"></a>

### G03 — THẨM ĐỊNH

Đánh giá:

chất lượng;

nguồn gốc;

giá trị;

authenticity.

Cần Domain Knowledge phù hợp.

---

<a id="m09-t025-s097"></a>

### G04 — GHI NHỚ & TRUYỀN KHẨU

Nhớ chính xác:

genealogy;

story;

route;

ritual;

agreement.

Rất quan trọng trong xã hội game.

---

<a id="m09-t025-s098"></a>

### G05 — NGÔN NGỮ & PHƯƠNG NGỮ

Không phải một Skill duy nhất dùng mọi ngôn ngữ.

Skill phản ánh khả năng học/giao tiếp.

Mỗi Language/Regional Speech là Knowledge Domain.

---

<a id="m09-t025-s099"></a>

### G06 — LUẬT TỤC

Hiểu:

quyền;

nghĩa vụ;

taboo;

inheritance;

arbitration.

Domain-specific.

---

<a id="m09-t025-s100"></a>

### G07 — QUẢN LÝ

Kho.

Nhân lực.

Supply.

Record memory.

Production coordination.

Sau này cực quan trọng tại Cổ Loa.

---

<a id="m09-t025-s101"></a>

## FAMILY H — NGHI LỄ & MẠCH

<a id="m09-t025-s102"></a>

### H01 — NGHI LỄ

Biết trình tự và ý nghĩa ritual.

Primary:

TRÍ/TÂM.

---

<a id="m09-t025-s103"></a>

### H02 — ĐỌC ĐIỀM

Interpret omen theo tradition.

Không đảm bảo omen thật.

---

<a id="m09-t025-s104"></a>

### H03 — CẢM MẠCH

Skill sử dụng chỉ khi character có Cảm Mạch >0 hoặc exposure phù hợp.

Cho phép diễn giải perception liên quan Mạch.

---

<a id="m09-t025-s105"></a>

### H04 — DẪN VONG / TANG NGHI

Funerary practice.

Social + Spiritual.

---

<a id="m09-t025-s106"></a>

### H05 — TRẤN TĨNH TÂM LINH

Giúp người khác xử lý fear/panic trong spiritual context.

Không phải phép “remove fear”.

---

<a id="m09-t025-s107"></a>

## 26. TẠI SAO KHÔNG TẠO HÀNG TRĂM SKILL?

Vì gameplay depth không đến từ số Skill.

Depth đến từ tổ hợp:

> Attribute + Skill + Knowledge + Technique + Equipment + Context.

Ví dụ:

Hai người cùng:

`Truy Dấu 60`

Một người:

Tam Giang Knowledge 5.

Người kia:

Tản Viên Knowledge 5.

Đi tìm người ở bờ sông:

người thứ nhất rất mạnh.

Trong rừng núi:

người thứ hai mạnh hơn.

Không cần tạo:

`River Tracking`

`Mountain Tracking`

`Swamp Tracking`

thành ba Skill riêng.

---

<a id="m09-t025-s108"></a>

## 27. KNOWLEDGE DOMAIN SYSTEM

Knowledge không hoàn toàn giống Skill.

Nó trả lời:

> “Character có quen với lĩnh vực/bối cảnh này không?”

Domain dùng thang:

0–5.

| Rank | Ý nghĩa |
|---:|---|
| 0 | không biết |
| 1 | từng nghe/tiếp xúc |
| 2 | quen thuộc |
| 3 | hiểu rõ |
| 4 | chuyên sâu |
| 5 | authority-level |

---

<a id="m09-t025-s109"></a>

## 28. REGIONAL KNOWLEDGE

Ví dụ:

Tam Giang.

Tản Viên.

Bãi Lạc.

Đồng Đỏ.

Lâm U.

Thượng Âu.

Đại Trạch.

Hải Môn.

...

Regional Knowledge bao gồm:

terrain;

custom;

food;

route;

danger;

speech;

local reputation.

---

<a id="m09-t025-s110"></a>

## 29. ENVIRONMENT KNOWLEDGE

River.

Mountain.

Forest.

Swamp.

Sea.

Agricultural Plain.

Urban/Ritual Hub.

---

<a id="m09-t025-s111"></a>

## 30. CULTURAL KNOWLEDGE

Không đồng nghĩa Region Knowledge hoàn toàn.

Một character có thể sống tại Phong Châu nhưng hiểu culture Tam Giang vì mẹ đến từ đó.

---

<a id="m09-t025-s112"></a>

## 31. PROFESSIONAL KNOWLEDGE

Metallurgy.

Boatbuilding.

Agriculture.

Trade.

Military organization.

Medicine.

Ritual history.

---

<a id="m09-t025-s113"></a>

## 32. HISTORICAL KNOWLEDGE

Character không có universal history database.

Knowledge có provenance.

Ví dụ:

> “Tòng kể rằng trận lũ năm đó bắt đầu sau tiếng trống.”

Đó là information object.

Không nhất thiết truth.

---

<a id="m09-t025-s114"></a>

## 33. KNOWLEDGE CÓ NGUỒN

Mỗi knowledge quan trọng lưu:

Source.

Reliability.

Confidence.

Ví dụ:

```text
Knowledge:
Mộ cổ nằm dưới Cồn Mây

Source:
Grandmother

Reliability:
Unknown

Character Confidence:
High
```

Điều này kết nối thẳng với Investigation Engine.

---

<a id="m09-t025-s115"></a>

## 34. TECHNIQUE SYSTEM

Technique là ability cụ thể.

Không phải generic +5%.

Ví dụ Skill:

`Truy Dấu`

Techniques:

**Dấu Trong Mưa**

Giảm mất clue khi trời mưa.

**Dấu Người Mang Nặng**

Nhận biết target đang mang tải lớn.

**Dấu Cố Che**

Phát hiện một số nỗ lực cố tình xóa trail.

---

<a id="m09-t025-s116"></a>

## 35. TECHNIQUE HỌC BẰNG CÁCH NÀO?

Có năm nguồn.

<a id="m09-t025-s117"></a>

#### Mentor

Người khác truyền.

<a id="m09-t025-s118"></a>

#### Experience

Character tự phát triển sau repeated challenge.

<a id="m09-t025-s119"></a>

#### Observation

Quan sát người khác.

<a id="m09-t025-s120"></a>

#### Knowledge Combination

Hai lĩnh vực kết hợp.

<a id="m09-t025-s121"></a>

#### Discovery

Exceptional event.

---

<a id="m09-t025-s122"></a>

## 36. TECHNIQUE CÓ THỂ CÓ PROVENANCE

Ví dụ:

> **Cắt Dòng Ngang**

Learned from:

Vạc Tòng.

Tòng học từ cha.

Cha học từ một người Vạn Nước.

Technique có genealogy.

Sau 100 năm nó có thể trở thành tradition.

---

<a id="m09-t025-s123"></a>

## 37. SKILL CHECK SYSTEM

Để GDD có nền rõ ràng, tôi đề xuất resolution mặc định:

<a id="m09-t025-s124"></a>

## 2D10 + Capability

Lý do dùng 2D10 thay vì D20:

D20 rất swingy.

Một Master vẫn có thể thất bại trivial check quá thường xuyên.

2D10 tạo bell curve nhẹ, khiến skill và preparation đáng tin cậy hơn nhưng vẫn giữ tabletop feeling.

---

<a id="m09-t025-s125"></a>

## 38. CHECK FORMULA

Conceptual formula:

```text
2D10
+ Primary Attribute
+ Secondary Attribute Contribution
+ Skill Rank Modifier
+ Knowledge
+ Technique
+ Equipment
+ Context
```

Không nhất thiết UI hiển thị toàn bộ phép tính.

---

<a id="m09-t025-s126"></a>

## 39. PRIMARY / SECONDARY ATTRIBUTE THAY ĐỔI THEO CÁCH GIẢI QUYẾT

Cùng Skill có thể dùng Attribute khác.

Ví dụ Intimidation.

<a id="m09-t025-s127"></a>

#### Threat bằng sức mạnh

THỂ + Dọa Nạt.

<a id="m09-t025-s128"></a>

#### Threat bằng địa vị

KHÍ + Dọa Nạt.

<a id="m09-t025-s129"></a>

#### Threat bằng thông tin

TRÍ + Dọa Nạt.

<a id="m09-t025-s130"></a>

#### Threat lạnh lùng tâm lý

TÂM/KHÍ + Dọa Nạt.

Điều này tạo build flexibility.

---

<a id="m09-t025-s131"></a>

## 40. CÙNG ACTION, NHIỀU CÁCH GIẢI QUYẾT

Một cánh cửa kẹt.

Player có thể:

THỂ + vận động

→ phá.

XẢO + repair

→ tháo cơ cấu.

TRÍ + observation

→ tìm cách mở.

Social

→ tìm người có chìa.

Không tồn tại một “đáp án skill đúng duy nhất”.

---

<a id="m09-t025-s132"></a>

## 41. KHÔNG ROLL CHO VIỆC HIỂN NHIÊN

Một Master Boatman không roll để:

> cập một bến bình thường trong thời tiết tốt.

Check chỉ xảy ra nếu tồn tại:

Risk.

Uncertainty.

Pressure.

Opposition.

Time constraint.

Narrative consequence.

---

<a id="m09-t025-s133"></a>

## 42. COMPETENCE FLOOR

Nếu capability vượt Difficulty đủ xa:

**Auto Success.**

Không dice.

Điều này khiến expertise thật sự có giá trị.

---

<a id="m09-t025-s134"></a>

## 43. DIFFICULTY BANDS

Design target:

| Mức | Ý nghĩa |
|---|---|
| Routine | người có nghề gần như chắc thành công |
| Simple | người mới có thể làm |
| Standard | cần competence |
| Challenging | người giỏi vẫn phải tập trung |
| Hard | expert task |
| Exceptional | master challenge |
| Legendary | rất hiếm người thực hiện được |

Final numerical values deferred to balancing.

---

<a id="m09-t025-s135"></a>

## 44. KHÔNG CHỈ SUCCESS / FAIL

Checks có 5 outcome band.

<a id="m09-t025-s136"></a>

### Critical Failure

Không chỉ thất bại.

Có complication đáng kể.

---

<a id="m09-t025-s137"></a>

### Failure

Không đạt mục tiêu.

---

<a id="m09-t025-s138"></a>

### Success With Cost

Đạt mục tiêu nhưng:

mất thời gian;

tài nguyên;

bị phát hiện;

injury;

relationship cost.

---

<a id="m09-t025-s139"></a>

### Clean Success

Đạt mục tiêu.

---

<a id="m09-t025-s140"></a>

### Exceptional Success

Đạt mục tiêu + phát hiện/lợi ích bổ sung.

---

<a id="m09-t025-s141"></a>

## 45. SUCCESS WITH COST RẤT QUAN TRỌNG

Narrative RPG không nên:

> fail check → reload save.

Ví dụ:

Player muốn leo vách.

Kết quả trung gian:

leo được,

nhưng làm rơi equipment.

Story tiếp tục.

---

<a id="m09-t025-s142"></a>

## 46. FAIL FORWARD

Major story check không được khiến game dead-end.

Failure phải tạo:

new problem;

different route;

cost;

lost information;

relationship consequence.

Không phải:

> “Bạn fail Investigation. Quest không thể tiếp tục.”

---

<a id="m09-t025-s143"></a>

## 47. OPPOSED CHECK

Khi chống NPC:

Capability của hai bên so sánh.

Ví dụ:

Player Deception

vs

NPC Thấu Hiểu.

Player Stealth

vs

NPC Observation.

Player Grapple

vs

NPC THỂ + Vật.

---

<a id="m09-t025-s144"></a>

## 48. SOCIAL CHECK KHÔNG ĐƯỢC BIẾN NPC THÀNH ROBOT

Thuyết phục thành công không có nghĩa:

NPC làm mọi thứ.

Mỗi NPC có:

Desire.

Fear.

Moral Boundary.

Relationship.

Interest.

Check chỉ thay đổi quyết định **trong phạm vi character có thể chấp nhận**.

Không thể persuasion check để:

> người cha bình thường tự nguyện giết con mình

nếu trái hoàn toàn identity.

---

<a id="m09-t025-s145"></a>

## 49. HARD SOCIAL BOUNDARY

Nếu request vượt Moral Boundary:

Dialogue option có thể vẫn xuất hiện.

Nhưng Difficulty có thể:

Impossible.

Player cần thay đổi context trước.

Ví dụ:

cung cấp evidence;

đe dọa family;

thay đổi relationship;

hoặc tạo một event khác.

---

<a id="m09-t025-s146"></a>

## 50. INFORMATION CHECK

Skill không tự tạo thông tin.

Ví dụ Investigation thành công chỉ có thể phát hiện:

**clue thực sự tồn tại.**

Không được:

> roll cao → engine invent clue mới trái world state.

Narrative consistency luôn cao hơn dice.

---

<a id="m09-t025-s147"></a>

## 51. SKILL SYNERGY

Một số Skill kết hợp.

Ví dụ:

Observation + Metallurgy

→ nhận ra weapon defect.

Observation + Medicine

→ nhận ra triệu chứng.

Tracking + River Knowledge

→ hiểu dấu tại bờ nước.

Negotiation + Law Custom

→ tìm loophole hợp lệ.

---

<a id="m09-t025-s148"></a>

## 52. TEAM SKILL

Party member được phép đóng góp.

Không phải mọi check chỉ dùng player character.

Ví dụ:

Player nói chuyện.

Companion hiểu luật tục.

Companion cung cấp contextual bonus hoặc unlock line.

Điều này làm Party System quan trọng ngoài combat.

---

<a id="m09-t025-s149"></a>

## 53. ASSIST ACTION

Character A có Skill cao.

Character B làm action.

A có thể:

hướng dẫn;

chuẩn bị;

cảnh báo.

Nhưng Assist chỉ hợp lệ nếu:

A có knowledge;

có thời gian;

có khả năng communication.

---

<a id="m09-t025-s150"></a>

## 54. CHARACTER GENERATION → STARTING SKILLS

Starting Skill Profile được tạo từ:

```text
Region
+ Family
+ Household
+ Childhood
+ Profession Exposure
+ Mentor
+ Life Events
+ Age
```

Không dùng point-buy.

---

<a id="m09-t025-s151"></a>

## 55. EXAMPLE: VẠC MÂN

17 tuổi.

Cồn Mây.

Gia đình đánh cá.

Từ nhỏ phụ cha.

Anh chết trong lũ.

Starting Attributes:

```text
THỂ 5
XẢO 6
GIÁC 7
TRÍ 5
TÂM 5
KHÍ 3
```

Starting Skills:

```text
Bơi & Lặn — Thành thạo thấp
Định hướng — Biết nghề
Quan sát — Thành thạo
Điều thuyền — Biết nghề
Truy dấu — Quen việc
Thuyết phục — Quen việc
Combat — gần như chưa học
```

Knowledge:

```text
Tam Giang 4
River 4
Cồn Mây 5
Fishing 3
Phong Châu 1
```

Không phải game random từng số.

Tất cả được suy ra từ biography.

---

<a id="m09-t025-s152"></a>

## 56. STARTING IMBALANCE LÀ TÍNH NĂNG

Một character có thể bắt đầu không biết combat.

Game không được coi đó là bad roll.

Họ có thể:

tránh fight;

thuê người;

dùng party;

học weapon;

dùng social;

dùng environment.

Đây chính là replayability.

---

<a id="m09-t025-s153"></a>

## 57. LEARNING NEW SKILL

Để học Skill từ 0:

character cần **Exposure**.

Ví dụ chưa bao giờ dùng nỏ.

Không thể spend point để có Nỏ 25.

Cần:

weapon;

người dạy hoặc experimentation;

thời gian.

Sau Exposure:

Skill mở.

---

<a id="m09-t025-s154"></a>

## 58. SELF-LEARNING

Cho phép.

Nhưng:

chậm;

nguy hiểm;

có thể hình thành bad habit.

Mentor giúp:

tăng learning rate;

giảm risk;

unlock technique.

---

<a id="m09-t025-s155"></a>

## 59. CROSS-SKILL TRANSFER

Một số skills có transfer.

Ví dụ:

Cung → Nỏ

có một phần transfer về aim.

Mộc tác → Đóng thuyền.

Thảo dược → Độc học.

Leadership → Military Command.

Nhưng không chuyển toàn bộ.

---

<a id="m09-t025-s156"></a>

## 60. SKILL DECAY

Không muốn game punitive quá mức.

Do đó Skill không decay mạnh chỉ vì không dùng.

Nhưng Technique cực tinh có thể trở nên:

`Rusty`.

Character vẫn biết nhưng chịu penalty nhỏ cho lần đầu quay lại sau nhiều năm.

---

<a id="m09-t025-s157"></a>

## 61. INJURY & SKILL

Injury có thể thay đổi cách dùng Skill.

Ví dụ tay phải bị thương.

Archer skill vẫn 70.

Nhưng khả năng thực hiện giảm.

Character có thể:

phục hồi;

học tay trái;

đổi weapon;

đổi nghề.

Skill knowledge không biến mất.

---

<a id="m09-t025-s158"></a>

## 62. PSYCHOLOGY & SKILL

Psychology ảnh hưởng performance trong context.

Ví dụ:

Mân Bơi 60.

Nhưng có Fear: Deep Water.

Ở sông cạn:

full performance.

Giữa vùng nước sâu:

Resolve check.

Fail:

Skill vẫn 60,

nhưng performance bị giảm vì panic.

Điều này nối Psychology với gameplay thật.

---

<a id="m09-t025-s159"></a>

## 63. REPUTATION & SKILL

Professional reputation không làm character giỏi hơn.

Nhưng ảnh hưởng:

access;

mentor;

client;

equipment;

opportunity.

Một nobody Skill 80 vẫn giỏi.

Nhưng có thể không được giao công việc lớn.

---

<a id="m09-t025-s160"></a>

## 64. SKILL SPECIALIZATION

Khi Skill cao, character có thể phát triển specialty.

Ví dụ Truy Dấu:

Human Tracker.

Large Animal Tracker.

River Trace.

Night Tracking.

Không tăng mọi context.

---

<a id="m09-t025-s161"></a>

## 65. MASTERY KHÔNG PHẢI ENDGAME

Master Skill mở gameplay mới:

dạy người khác;

đánh giá người khác;

innovation;

create technique;

professional authority.

Một Master không chỉ có:

+20% success.

---

<a id="m09-t025-s162"></a>

## 66. TEACHING

Khả năng truyền Skill phụ thuộc:

Skill Mastery

+

Communication

+

TÂM

+

Teaching Experience.

Một thiên tài không nhất thiết là thầy giỏi.

---

<a id="m09-t025-s163"></a>

## 67. CHARACTER CAN INVENT TECHNIQUE

Nếu:

Skill cao

+

repeated unique challenge

+

Insight

+

appropriate Personality

character có thể tạo Technique mới.

Technique mang tên game-canon từ history.

Sau đó có thể truyền cho apprentice.

---

<a id="m09-t025-s164"></a>

## 68. PLAYER-CREATED LEGACY

Ví dụ player sáng tạo:

một phương pháp cập bến trong nước lớn.

Dạy ba người.

Một học trò trở thành Master.

30 năm sau technique được gọi theo:

tên player;

tên làng;

hoặc một tên méo mó do truyền miệng.

Đây là giao điểm:

**Skill → Career → Legacy → Myth Formation.**

---

<a id="m09-t025-s165"></a>

## 69. CLASS EMERGENCE INPUT

Class sau này không nhìn một Skill duy nhất.

Nó xem:

<a id="m09-t025-s166"></a>

#### Attribute Pattern

character thiên về gì?

<a id="m09-t025-s167"></a>

#### Skill Cluster

đã luyện gì?

<a id="m09-t025-s168"></a>

#### Profession

đã sống bằng gì?

<a id="m09-t025-s169"></a>

#### Techniques

đã học gì?

<a id="m09-t025-s170"></a>

#### Behaviour

player thường giải quyết vấn đề thế nào?

<a id="m09-t025-s171"></a>

#### Milestones

đã làm được gì?

---

<a id="m09-t025-s172"></a>

## 70. EXAMPLE CLASS EMERGENCE

Character:

THỂ 6

XẢO 7

GIÁC 8

Tracking 70

Stealth 60

Spear 45

Hunting 75

Tản Viên Knowledge 5

Profession:

Hunter.

Behaviour:

thường quan sát trước khi đánh.

Có thể unlock archetype candidate:

> **Liệp Nhân**

Nhưng không bắt player nhận ngay.

---

<a id="m09-t025-s173"></a>

## 71. MỘT CHARACTER KHÁC

TRÍ 7

TÂM 8

KHÍ 6

Medicine 65

Herbalism 75

Ritual 45

Thấu Hiểu 70

Profession:

Healer.

Có thể xuất hiện nhiều hướng:

Dược Nhân.

Vu Y.

Người Chăm Sinh.

Ritual Healer.

Class Emergence sẽ xử lý module sau.

---

<a id="m09-t025-s174"></a>

## 72. ATTRIBUTE/SKILL UI

Character Sheet không nên chỉ là bảng số.

Ví dụ:

<a id="m09-t025-s175"></a>

#### THỂ 6 — Khá

> Cơ thể quen lao động nặng và chèo thuyền lâu.

<a id="m09-t025-s176"></a>

#### GIÁC 7 — Nổi bật

> Rất nhạy với những thay đổi nhỏ trong môi trường.

Skill:

<a id="m09-t025-s177"></a>

#### Truy Dấu — 48 / Thành thạo

Source:

- tuổi thơ săn cá;
- học từ cha;
- kinh nghiệm trận lũ.

Specialty:

River Traces.

Technique:

Nước Giữ Dấu.

Player nhìn được **vì sao mình có Skill này**.

---

<a id="m09-t025-s178"></a>

## 73. CHARACTER HISTORY LINK

Click vào Skill có thể mở:

**Skill Chronicle.**

Ví dụ:

```text
Năm 8 tuổi:
Học phân biệt dấu cá từ cha.

Năm 11:
Bắt đầu theo thuyền.

Năm 13:
Sống sót Đại Thủy.

Năm 14:
Học đọc bùn ven bờ.

Năm 16:
Tìm thấy người mất tích nhờ dấu nước.
```

Progression trở thành story.

---

<a id="m09-t025-s179"></a>

## 74. KHÔNG CÓ “BEST ATTRIBUTE”

Mỗi Core Attribute phải có giá trị xuyên game.

THỂ:

combat + labor + travel.

XẢO:

combat + craft + movement.

GIÁC:

exploration + combat + investigation.

TRÍ:

knowledge + strategy + craft.

TÂM:

social + morale + psychology.

KHÍ:

leadership + social + fear resistance.

Nếu một Attribute trở thành dump stat phổ biến, design thất bại.

---

<a id="m09-t025-s180"></a>

## 75. KHÔNG CÓ “BEST SKILL”

Game content phải hỗ trợ nhiều solution.

Ví dụ Investigation quest phải có thể giải qua:

Observation.

Social.

Trade Knowledge.

Tracking.

Medicine.

Professional knowledge.

Không phải mọi quest đều cần tất cả.

Nhưng toàn game phải đảm bảo mỗi Skill có meaningful content.

---

<a id="m09-t025-s181"></a>

## 76. SKILL CONTENT COVERAGE

Production phải theo dõi metric:

```text
Skill Usage Count
Unique Quest Usage
Unique Exploration Usage
Career Usage
Combat Usage
Dialogue Usage
```

Nếu một Skill gần như không xuất hiện:

merge hoặc redesign.

---

<a id="m09-t025-s182"></a>

## 77. NPC CŨNG SỬ DỤNG SKILL THẬT

Blacksmith NPC không có tag:

`Blacksmith=true`.

Họ có:

Metallurgy.

Casting.

Repair.

Professional Knowledge.

Techniques.

Do đó nếu master chết:

không thể spawn generic replacement rồi coi như giống nhau.

Settlement thật sự mất competence.

---

<a id="m09-t025-s183"></a>

## 78. SETTLEMENT SKILL ECONOMY

Settlement có aggregate capability.

Ví dụ:

Bến Vạc:

Boatbuilding: cao.

Metalworking: thấp.

Medicine: trung bình.

Nếu master đóng thuyền chết:

local Boatbuilding capability giảm.

Giá sửa thuyền tăng.

Trade bị ảnh hưởng.

Career opportunities xuất hiện.

Player skill trở thành một phần World Simulation.

---

<a id="m09-t025-s184"></a>

## 79. KNOWLEDGE LOSS

Nếu một Technique chỉ có một Master biết.

Master chết mà chưa truyền:

Technique có thể biến mất.

Không phải unlock vĩnh viễn trong global tech tree.

Sau này player có thể:

tìm artifact;

oral record;

descendant memory;

để rediscover.

---

<a id="m09-t025-s185"></a>

## 80. KNOWLEDGE PRESERVATION

Player có thể trở thành người bảo tồn knowledge.

Thông qua:

apprentice;

story;

ritual;

artifact;

workshop;

family tradition.

Đây sẽ liên kết cực mạnh với Legacy System.

---

<a id="m09-t025-s186"></a>

## 81. ATTRIBUTE CHECK KHÔNG THỂ THAY SKILL HOÀN TOÀN

THỂ 9 không khiến người chưa học bơi tự nhiên bơi qua sông lớn giỏi.

TRÍ 9 không khiến character biết chữa bệnh.

KHÍ 9 không khiến mọi người nghe lời.

Attributes cho **potential**.

Skills cho **competence**.

---

<a id="m09-t025-s187"></a>

## 82. SKILL KHÔNG THỂ THAY KNOWLEDGE HOÀN TOÀN

Một Tracker Master chưa từng thấy đầm Đại Trạch vẫn có nguy cơ đọc sai dấu.

Skill giúp thích nghi.

Nhưng local guide vẫn có giá trị.

---

<a id="m09-t025-s188"></a>

## 83. KNOWLEDGE KHÔNG THỂ THAY SKILL

Một scholar biết lý thuyết đóng thuyền chưa chắc đóng được.

Knowledge 5.

Mộc tác 10.

→ vẫn là người thiếu hands-on competence.

---

<a id="m09-t025-s189"></a>

## 84. TECHNIQUE KHÔNG THỂ DÙNG NẾU NỀN QUÁ YẾU

Một Technique cao cấp có minimum Skill requirement.

Character có thể **biết lý thuyết** nhưng chưa đủ khả năng thực hiện.

Ví dụ học Master Technique qua observation nhưng Skill thấp.

Technique status:

`Known / Cannot Reliably Execute`.

---

<a id="m09-t025-s190"></a>

## 85. EQUIPMENT KHÔNG THỂ THAY CHARACTER

Weapon tốt hỗ trợ.

Nhưng:

nỏ tốt + Skill Nỏ 5

không biến character thành marksman.

Tool tốt + Smith Skill thấp

không tạo masterwork.

Equipment là multiplier/context, không phải toàn bộ build.

---

<a id="m09-t025-s191"></a>

## 86. REST & CONDITION

Performance còn phụ thuộc trạng thái.

Fatigue.

Hunger.

Injury.

Illness.

Fear.

Morale.

Weather.

Do đó Skill sheet không đảm bảo performance cố định.

---

<a id="m09-t025-s192"></a>

## 87. PREPARATION BONUS

Ngược lại, player có thể vượt skill limitation bằng preparation.

Ví dụ:

thiết bị phù hợp;

đồng đội;

thời gian;

knowledge;

địa hình;

planning.

Đây là phần quan trọng của D&D-style problem solving.

---

<a id="m09-t025-s193"></a>

## 88. TEMPORARY EXPERTISE

NPC có thể cho:

Advice.

Ví dụ Master Smith nói:

> “Nếu thấy màu lửa chuyển thế này thì dừng.”

Player chưa tăng Metallurgy Skill.

Nhưng được contextual bonus cho task cụ thể.

Knowledge khác Skill.

---

<a id="m09-t025-s194"></a>

## 89. CHARACTER BUILD KHÔNG BỊ KHÓA BỞI RANDOM ORIGIN

Một character xuất phát:

THỂ thấp.

Combat gần 0.

Vẫn có thể trở thành warrior.

Nhưng con đường khó hơn:

training;

mentor;

physical conditioning.

Và kết quả có thể tạo character story rất mạnh.

Game không nói:

> “Bạn sinh sai build.”

---

<a id="m09-t025-s195"></a>

## 90. NHƯNG ORIGIN PHẢI CÓ TRỌNG LƯỢNG

Ngược lại không được để player trong vài giờ erase toàn bộ quá khứ.

Người lớn lên 20 năm làm thợ đúc phải có lợi thế dài hạn so với người mới học.

Random origin phải tiếp tục có ý nghĩa.

---

<a id="m09-t025-s196"></a>

## 91. NATURAL SPECIALIZATION

Do thời gian đời người hữu hạn:

player không thể Master 30 Skill.

Không cần hard cap.

Muốn Master một skill:

mất nhiều năm;

mentor;

challenge;

practice.

Do đó specialization xuất hiện tự nhiên.

---

<a id="m09-t025-s197"></a>

## 92. GENERALIST BUILD

Generalist vẫn hợp lệ.

Nhiều Skill Rank 2–3.

Không Master gì.

Lợi thế:

adaptability.

Đây có thể trở thành một archetype riêng trong Class Emergence.

---

<a id="m09-t025-s198"></a>

## 93. CHARACTER VIABILITY GATE MỞ RỘNG

Generated player character phải có:

- ít nhất 1 Attribute ≥6 hoặc tổ hợp capability tương đương;
- ít nhất 2 Skill sử dụng được;
- ít nhất 1 meaningful Knowledge Domain;
- tối thiểu một đường học nghề khả thi;
- tối thiểu một cách tự bảo vệ hoặc tránh nguy hiểm;
- không có tổ hợp injury/illness khiến game gần như bất khả thi trừ special mode;
- tối thiểu hai hướng phát triển khác nhau.

---

<a id="m09-t025-s199"></a>

## 94. KHÔNG ÉP COMBAT SKILL

Viability không bắt buộc:

weapon skill.

Một character có thể bắt đầu hoàn toàn không biết chiến đấu.

Party/social/evasion phải hỗ trợ điều này.

---

<a id="m09-t025-s200"></a>

## 95. INTERACTION WITH PROFESSION

Profession yêu cầu một **Skill Profile**, không phải một Skill duy nhất.

Ví dụ Master Boatman:

```text
Bơi & Lặn
Định hướng
Quan sát
Thăng bằng
Sức bền
River Knowledge
Boat Techniques
```

Smith:

```text
Luyện kim
Đúc
Quan sát
Thẩm định
Sửa chữa
Metal Knowledge
Forge Techniques
```

---

<a id="m09-t025-s201"></a>

## 96. INTERACTION WITH NARRATIVE

Skill quyết định player nhận thấy phần nào của story.

Một xác chết:

Healer → injury.

Hunter → track.

Smith → weapon.

Ritualist → ritual mark.

Merchant → rare imported object.

Mọi build nhìn **cùng thế giới qua bộ lọc khác nhau**.

---

<a id="m09-t025-s202"></a>

## 97. INTERACTION WITH PSYCHOLOGY

Psychology có thể:

enhance;

suppress;

redirect

skill performance.

Nhưng personality không được overwrite competence.

Một người sợ vẫn có thể rất giỏi.

Họ chỉ khó sử dụng skill trong trigger context.

---

<a id="m09-t025-s203"></a>

## 98. INTERACTION WITH CAREER

Career tạo opportunity sử dụng skill.

Skill tạo khả năng thăng career.

Hai hệ tạo feedback loop:

```text
Career
→ Challenge
→ Skill Growth
→ Career Opportunity
→ Harder Challenge
```

---

<a id="m09-t025-s204"></a>

## 99. INTERACTION WITH CLASS

Class sẽ không sở hữu Skill độc quyền tuyệt đối.

Class chủ yếu:

- tổ chức các Skill thành gameplay style;
- mở Technique đặc biệt;
- tạo synergies;
- định hình combat/social role.

Một character không có Class vẫn sử dụng Skill bình thường.

---

<a id="m09-t025-s205"></a>

## 100. DEFERRED TO PRODUCTION

Chưa cần khóa ở GDD:

- con số chính xác Skill gain;
- numerical Difficulty;
- Attribute growth rate;
- skill mastery curve;
- toàn bộ Technique;
- toàn bộ Specialization;
- equipment modifiers;
- exact fatigue formulas;
- toàn bộ Knowledge Domains;
- animation cho từng action.

Nhưng kiến trúc trên phải được giữ.

---

<a id="m09-t025-s206"></a>

## 101. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

1. Có 6 Core Attributes: **THỂ, XẢO, GIÁC, TRÍ, TÂM, KHÍ**.
2. Player không phân Attribute.
3. Attribute ban đầu sinh từ biography.
4. Attribute thay đổi chậm trong suốt đời nhân vật.
5. Skill học bằng cuộc sống, không mua bằng generic XP.
6. Skill dùng Mastery nội bộ + Rank hiển thị.
7. Skill, Knowledge và Technique là ba khái niệm riêng.
8. Cảm Mạch không phải Core Attribute.
9. Không phải character nào cũng có Cảm Mạch.
10. Skill check chỉ xảy ra khi có uncertainty hoặc consequence.
11. Người giỏi đủ mức được auto-success việc routine.
12. Hệ thống ưu tiên fail-forward.
13. Social check không phá Moral Boundary tùy tiện.
14. Roll không được tạo ra clue không tồn tại.
15. Local Knowledge cực kỳ quan trọng.
16. NPC và player dùng chung hệ năng lực.
17. Skills có thể chuyển giao giữa profession.
18. Technique có nguồn gốc và có thể bị thất truyền.
19. Master có thể tạo Technique mới.
20. Character build phát triển tự nhiên từ lịch sử chơi.
21. Random origin tạo lợi thế và bất lợi thật.
22. Không có origin “sai”.
23. Không có Attribute bắt buộc phải mạnh.
24. Không bắt buộc player biết combat.
25. Class sẽ được hình thành dựa trên toàn bộ capability profile chứ không được chọn lúc tạo nhân vật.

---

<a id="m09-t025-s207"></a>

## 102. CORE PLAYER FANTASY

Hệ thống này phải tạo được cảm giác:

> “Tôi không chọn được mình sinh ra khỏe hay yếu, thông minh hay vụng về, được ai nuôi lớn hay đã học nghề gì. Nhưng sau khi cuộc đời này được trao cho tôi, cách tôi dùng những gì mình có là lựa chọn của tôi.”

Đây chính là cầu nối giữa:

**Random Character Generation**

và

**Player Agency**.

Game quyết định:

> Bạn bắt đầu là ai.

Player quyết định:

> Bạn trở thành ai.

---

<a id="m09-t025-s208"></a>

## 103. MODULE TIẾP THEO — CLASS EMERGENCE SYSTEM

Sau khi đã có:

**Attributes**

+

**Skills**

+

**Knowledge**

+

**Techniques**

+

**Profession**

+

**Psychology**

chúng ta đủ dữ liệu để thiết kế Class.

Nhưng Class của game này không nên hoạt động như D&D:

> Level 1 → chọn Fighter.

Nó nên là:

> **một danh tính gameplay được thế giới công nhận sau khi character thật sự sống theo con đường đó.**

Ví dụ một người ban đầu chỉ là thợ săn.

Sau nhiều năm:

Truy Dấu cao.

Cung cao.

Animal Knowledge cao.

Thường giải quyết encounter bằng stalking.

Đã hạ một con thú nguy hiểm.

Có mentor hunter.

Được cộng đồng công nhận.

Khi đó mới xuất hiện:

> **Con đường Liệp Nhân đã được mở.**

Từ thời điểm đó player có thể **chấp nhận, từ chối hoặc tiếp tục để một archetype khác hình thành**.

Đây sẽ là module kế tiếp cần thiết kế rất sâu, bởi nó sẽ quyết định toàn bộ **class, subclass, multiclass và skill tree** của game.

Bước tiếp theo tôi đề xuất làm liền một cụm **Class Emergence → Archetype/Class → Specialization/Subclass → Skill Tree → Multiclass**, vì bốn phần này phụ thuộc chặt vào nhau. Khi cụm đó hoàn thành, chúng ta mới thực sự có cấu trúc progression nhân vật hoàn chỉnh để chuyển sang Party và Combat.

<!-- SOURCE T025 END -->

