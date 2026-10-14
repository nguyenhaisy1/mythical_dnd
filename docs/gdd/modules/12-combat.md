# Module 12 — Combat theo lượt, Zone và Local Position Slots

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T031](../../source/conversation.md#t031)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Chọn Zone + Local Position Slots; Pure Zone là trường hợp đơn giản. Combat xử lý hit/impact, Health xử lý wound; 0 Vitality không đồng nghĩa chết. Major/Minor/Reactions theo Module 27.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Encounter space, equipment, capability, perception, morale và health. |
| Đầu ra / bên sở hữu | Tactical proposals, injury requests và outcomes; Application commit xuyên domain. |
| Đọc cùng | 09–11, 13, 15, 27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Surrender/retreat/objective hoàn thành kết thúc được trận; hậu quả tồn tại sau combat. |
| Việc chưa được chứng minh | Damage, action cost, AI weights và battle-size maximum. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T031 BEGIN -->

Dựa trên toàn bộ GDD đã xây, combat **không nên được thiết kế như một module độc lập**, vì như vậy rất dễ rơi vào tình trạng narrative game một kiểu nhưng vào trận lại biến thành tactical RPG thuần túy. Tôi sẽ phân tích nhiều mô hình trước, rồi chọn một kiến trúc combat phù hợp nhất và vẫn để lại các option thay thế nếu sau prototype thấy chưa ổn.

<a id="m12-t031-s001"></a>

## GDD MODULE 12 — COMBAT SYSTEM

<a id="m12-t031-s002"></a>

## 1. VAI TRÒ CỦA COMBAT TRONG GAME

Combat không phải gameplay loop trung tâm duy nhất.

Game có các trục ngang hàng:

- Narrative;
- Exploration;
- Social;
- Profession;
- Investigation;
- Crafting;
- Combat.

Combat cần phục vụ câu chuyện thay vì phá nhịp câu chuyện.

Một trận đánh tốt phải trả lời ít nhất một câu:

> Tại sao trận này xảy ra?

> Player đang cố đạt điều gì?

> Có nhất thiết phải giết đối phương không?

> Nếu thắng thì thế giới thay đổi gì?

> Nếu thua nhưng sống thì chuyện tiếp tục ra sao?

Không nên tồn tại encounter kiểu:

> đi 20 mét → gặp 5 bandit → giết → loot → đi tiếp.

---

<a id="m12-t031-s003"></a>

## 2. CORE COMBAT FANTASY

Combat của game nên tạo cảm giác:

> **nguy hiểm, có tính người, có chiến thuật và có hậu quả.**

Người chơi không phải siêu anh hùng chém 40 người mỗi trận.

Một ngọn giáo vẫn nguy hiểm.

Một mũi tên trúng đúng chỗ có thể kết thúc trận.

Một người bị thương nặng có thể mất nhiều tuần hồi phục.

Một companion chết có thể thay đổi campaign.

Vì vậy chiến thắng không chỉ là:

> giảm HP đối phương về 0.

Chiến thắng có thể là:

- giữ được cây cầu;
- đưa người bị thương thoát;
- bắt sống thủ lĩnh;
- khiến đối phương bỏ chạy;
- câu giờ;
- vượt qua tuyến phòng thủ;
- lấy vật phẩm rồi rút;
- bảo vệ dân;
- không để ai chết;
- đánh tan đội hình;
- khiến phe kia đầu hàng.

---

<a id="m12-t031-s004"></a>

## 3. TẦN SUẤT COMBAT

Combat nên tương đối **ít nhưng có trọng lượng**.

Định hướng ban đầu:

- encounter nhỏ: thường xuyên vừa phải;
- combat thực sự nguy hiểm: không quá dày;
- trận lớn: hiếm;
- chiến tranh quy mô lớn: event đặc biệt.

Không nên dùng trash mob để kéo thời lượng.

Một trận 4 vs 4 có thể quan trọng hơn một trận 4 vs 30 nếu mỗi NPC đều có identity và consequence.

---

<a id="m12-t031-s005"></a>

## 4. CÁC MÔ HÌNH COMBAT CÓ THỂ DÙNG

Trước khi chốt, cần đánh giá khách quan.

---

<a id="m12-t031-s006"></a>

## OPTION A — TURN-BASED GRID TACTICAL

Ví dụ về triết lý:

ô vuông hoặc hex;

movement theo ô;

turn từng nhân vật;

cover;

reach;

formation.

<a id="m12-t031-s007"></a>

### Ưu điểm

Rất rõ ràng.

Dễ đọc tactical state.

Hợp với:

- spear reach;
- shield;
- choke point;
- formation;
- terrain.

Tạo chiều sâu combat lớn.

Rất gần trải nghiệm tactical CRPG.

<a id="m12-t031-s008"></a>

### Nhược điểm

Chi phí content cao.

Mỗi encounter cần map tốt.

Map xấu khiến combat nhàm.

Grid dễ đẩy game thành:

> combat puzzle

thay vì narrative RPG.

Đòi hỏi nhiều asset môi trường hơn.

Có nguy cơ player mất 20 phút để xử lý một trận không quan trọng.

<a id="m12-t031-s009"></a>

### Đánh giá với game

**Phù hợp: 8/10.**

Rất tốt về tactical.

Nhưng hơi nặng so với định hướng:

> content-first, graphics-secondary.

---

<a id="m12-t031-s010"></a>

## OPTION B — ZONE-BASED TURN-BASED

Battlefield chia thành các **Zone**, không phải hàng trăm ô.

Ví dụ:

```text
Bờ cao
│
Ruộng ngập
│
Bờ đê
│
Làng
```

Hoặc:

```text
Mũi thuyền
Giữa thuyền
Đuôi thuyền
Thuyền đối phương
Nước
```

Nhân vật đứng trong Zone.

Khoảng cách được biểu diễn:

- cùng Zone;
- liền kề;
- xa;
- ngoài tầm.

<a id="m12-t031-s011"></a>

### Ưu điểm

Giữ tactical depth.

Giảm mạnh chi phí map.

Dễ tạo encounter procedural.

Rất hợp với:

- thuyền;
- rừng;
- làng;
- hang;
- đầm.

Có thể tập trung vào:

position;

terrain;

objective

thay vì đếm từng ô.

Combat nhanh hơn.

Rất phù hợp game content-heavy.

<a id="m12-t031-s012"></a>

### Nhược điểm

Ít precision hơn grid.

Một số người thích tactical rất sâu có thể thấy thiếu.

Thiết kế Zone phải rất tốt để tránh trở thành:

> hàng trước / hàng sau đơn giản.

<a id="m12-t031-s013"></a>

### Đánh giá

**Phù hợp: 9.5/10.**

Đây là phương án tôi đề xuất làm **Core Combat Model**.

---

<a id="m12-t031-s014"></a>

## OPTION C — WEGO / SIMULTANEOUS TURN

Player ra lệnh cho party.

Enemy cũng quyết định.

Sau đó mọi hành động thực thi gần đồng thời.

Ví dụ:

Mân chạy sang trái.

Vũ bắn.

Enemy cũng charge.

Tất cả resolve theo Reaction/Speed.

<a id="m12-t031-s015"></a>

### Ưu điểm

Cảm giác hỗn loạn chiến đấu rất tốt.

Planning quan trọng.

Information và prediction cực mạnh.

Trinh Binh, Thủ Lĩnh có giá trị cao.

<a id="m12-t031-s016"></a>

### Nhược điểm

Khó đọc.

Khó balance.

Khó giải thích cho người mới.

AI phải tốt.

Debug interaction phức tạp.

<a id="m12-t031-s017"></a>

### Đánh giá

**Phù hợp: 7.5/10.**

Rất thú vị nhưng rủi ro implementation cao.

Có thể dùng một phần:

**Simultaneous Reaction Layer**

thay vì toàn bộ combat.

---

<a id="m12-t031-s018"></a>

## OPTION D — REAL-TIME WITH PAUSE

<a id="m12-t031-s019"></a>

### Ưu điểm

Combat nhanh.

Đông người dễ xử lý.

Cảm giác chiến trường tự nhiên.

<a id="m12-t031-s020"></a>

### Nhược điểm

Không hợp character psychology và decision-heavy game bằng turn-based.

Khó quan sát:

injury;

formation;

social surrender;

special objective.

Player phải micromanage.

<a id="m12-t031-s021"></a>

### Đánh giá

**Phù hợp: 5.5/10.**

Không khuyến nghị.

---

<a id="m12-t031-s022"></a>

## OPTION E — ABSTRACT / THEATER OF THE MIND

Combat giống tabletop narrative.

Player chọn:

Advance.

Protect.

Flank.

Shoot.

Retreat.

Battlefield được mô tả thay vì thể hiện chi tiết.

<a id="m12-t031-s023"></a>

### Ưu điểm

Chi phí đồ họa cực thấp.

Narrative cực mạnh.

Có thể generate encounter vô hạn.

<a id="m12-t031-s024"></a>

### Nhược điểm

Thiếu tactical satisfaction.

Equipment/weapon/class rất khó phát huy hết.

Player dễ cảm thấy kết quả dựa trên text roll.

<a id="m12-t031-s025"></a>

### Đánh giá

**Phù hợp: 7/10.**

Rất tốt cho:

- large battles;
- offscreen combat;
- secondary encounters.

Không nên dùng làm combat chính.

---

<a id="m12-t031-s026"></a>

## 5. ĐỀ XUẤT CHÍNH

Sử dụng:

<a id="m12-t031-s027"></a>

## ZONE-BASED TACTICAL TURN-BASED

Kết hợp một số yếu tố:

- Reaction System;
- Formation;
- Morale;
- Injury;
- Objective Combat;
- Preparation;
- Environmental interaction.

Large-scale war dùng **Abstract Battle Layer**.

Tức là:

```text
Normal Combat
→ Zone Tactical

Large Battle
→ Strategic/Abstract Layer
→ Tactical Moment nếu player trực tiếp tham chiến
```

Đây là hybrid phù hợp nhất với game.

---

<a id="m12-t031-s028"></a>

## 6. BATTLEFIELD ZONE

Mỗi battlefield có khoảng:

**3–9 Zone.**

Không nên quá nhiều.

Ví dụ trận trong rừng:

```text
[Đường mòn]
     │
[Bụi thấp] ─ [Bãi đá]
     │
[Rừng dày]
     │
[Khe suối]
```

Mỗi Zone có property.

Ví dụ:

**Bụi thấp**

Concealment cao.

Movement chậm.

Fire risk cao.

---

<a id="m12-t031-s029"></a>

## 7. ZONE PROPERTY

Zone có thể có:

Terrain.

Elevation.

Cover.

Visibility.

Movement Cost.

Capacity.

Hazard.

Noise.

Water Depth.

Structural Stability.

Mạch Condition.

Ví dụ:

```text
ZONE: Ruộng Ngập

Movement: Difficult
Visibility: Normal
Footing: Poor
Water: Knee-depth
Fire: Impossible
Spear Stability: Reduced
Water Skill Advantage: Active
```

---

<a id="m12-t031-s030"></a>

## 8. ZONE CAPACITY

Zone không chứa vô hạn character.

Ví dụ:

cầu hẹp:

Capacity 2–3.

Thuyền nhỏ:

Capacity 4.

Bãi rộng:

Capacity cao.

Nhờ vậy choke point tự nhiên có giá trị.

---

<a id="m12-t031-s031"></a>

## 9. DISTANCE

Không cần meter chính xác.

Có bốn range band:

**Engaged**

Đang giao chiến gần.

**Near**

Cùng zone hoặc zone kề.

**Far**

Cách vài zone.

**Extreme**

Ngoài combat range thông thường.

Weapon định nghĩa effective band.

---

<a id="m12-t031-s032"></a>

## 10. ENGAGEMENT

Hai character trong cùng Zone chưa chắc đã Engaged.

Engaged nghĩa là:

> một bên đang trực tiếp kiểm soát movement/combat space của bên kia.

Rời Engagement có risk.

---

<a id="m12-t031-s033"></a>

## 11. ACTION ECONOMY

Mỗi character mỗi lượt có:

<a id="m12-t031-s034"></a>

### 1 Major Action

Ví dụ:

Attack.

Heavy Attack.

Treat Wound.

Use Technique.

Full Movement.

Manipulate Object.

---

<a id="m12-t031-s035"></a>

### 1 Minor Action

Ví dụ:

Short Move.

Draw weapon.

Change stance.

Interact đơn giản.

Call instruction.

---

<a id="m12-t031-s036"></a>

### Reaction

Ngoài turn.

Phụ thuộc:

Reaction availability.

Technique.

Position.

---

<a id="m12-t031-s037"></a>

## 12. TẠI SAO KHÔNG 2 ACTION TỰ DO HOÀN TOÀN?

Nếu dùng kiểu:

2 Action bất kỳ

rất linh hoạt nhưng dễ tạo combo tối ưu hóa khó đọc.

Major + Minor rõ hơn:

player hiểu giá trị hành động.

Technique có thể phá rule này ở những Class đặc biệt.

---

<a id="m12-t031-s038"></a>

## 13. TURN ORDER

Không nên dùng Initiative roll một lần rồi giữ cứng suốt trận.

Đề xuất:

<a id="m12-t031-s039"></a>

## PHASED INITIATIVE

Mỗi Round:

<a id="m12-t031-s040"></a>

#### Awareness Phase

Ai nhận ra threat?

<a id="m12-t031-s041"></a>

#### Intent Phase

Player chọn action.

<a id="m12-t031-s042"></a>

#### Action Phase

Resolve theo:

Reaction

+

Action Speed

+

Situation.

Không cần tất cả đồng thời như WEGO.

---

<a id="m12-t031-s043"></a>

## 14. ACTION SPEED

Action chia:

Fast.

Standard.

Slow.

Ví dụ:

Quick stab:

Fast.

Normal attack:

Standard.

Heavy spear thrust:

Slow.

Bandage:

Slow.

Character Reaction + Technique có thể thay đổi thứ tự.

Nhờ đó:

initiative có ý nghĩa mà không roll lại hỗn loạn.

---

<a id="m12-t031-s044"></a>

## 15. SURPRISE

Surprise không phải:

> mất toàn bộ turn mặc định.

Có mức:

Aware.

Unprepared.

Surprised.

Completely Unaware.

Giác + Scout + Knowledge ảnh hưởng.

Một Trinh Binh tốt có thể biến:

Completely Unaware

→ Unprepared.

---

<a id="m12-t031-s045"></a>

## 16. STANCE

Character có thể chọn Stance.

Ví dụ:

<a id="m12-t031-s046"></a>

### Balanced

Mặc định.

<a id="m12-t031-s047"></a>

### Aggressive

Attack tốt hơn.

Defense/Reactions kém hơn.

<a id="m12-t031-s048"></a>

### Guarded

Defense tốt.

Movement thấp.

<a id="m12-t031-s049"></a>

### Mobile

Movement/Disengage tốt.

Attack pressure thấp.

Class có thể mở stance đặc biệt.

---

<a id="m12-t031-s050"></a>

## 17. KHÔNG SPAM STANCE

Đổi stance dùng Minor Action hoặc Technique.

Player phải có commitment chiến thuật.

---

<a id="m12-t031-s051"></a>

## 18. ATTACK RESOLUTION

Dùng nền Skill Check đã khóa:

```text
2D10
+ Weapon Skill
+ Attribute
+ Technique
+ Position
+ Weapon Quality
+ Context
```

Đối đầu:

```text
Defense Capability
```

Defense có thể là:

Dodge.

Block.

Parry.

Cover.

---

<a id="m12-t031-s052"></a>

## 19. KHÔNG DÙNG ARMOR CLASS DUY NHẤT

Attack có thể:

Miss.

Be Evaded.

Be Blocked.

Hit.

Armor Mitigates.

Các bước này nên khác nhau.

---

<a id="m12-t031-s053"></a>

## 20. DEFENSE OPTIONS

<a id="m12-t031-s054"></a>

### Dodge

Dựa:

XẢO + movement.

Tốt khi còn space.

Tệ khi formation dày.

---

<a id="m12-t031-s055"></a>

### Block

Khiên hoặc weapon phù hợp.

Tốn Stamina.

Có durability impact.

---

<a id="m12-t031-s056"></a>

### Parry

Skill cao.

Hiệu quả trước một số weapon.

Risk cao hơn block.

---

<a id="m12-t031-s057"></a>

### Brace

Đặc biệt với spear/shield.

Chuẩn bị chống charge.

---

<a id="m12-t031-s058"></a>

## 21. VŨ KHÍ KHÔNG KHÁC NHAU CHỈ BỞI DAMAGE

Đây phải là nguyên tắc lớn.

Weapon identity đến từ:

- reach;
- speed;
- penetration;
- control;
- defense;
- environment;
- handling.

---

<a id="m12-t031-s059"></a>

## 22. THƯƠNG / GIÁO

Fantasy:

**kiểm soát khoảng cách.**

Strengths:

reach;

brace;

formation;

thrust.

Weakness:

khó dùng khi bị áp sát.

Trong không gian hẹp:

bất lợi.

---

<a id="m12-t031-s060"></a>

## 23. RÌU

Fantasy:

**phá guard và gây thương tổn lớn.**

Strengths:

power;

shield pressure;

tool utility.

Weakness:

chậm hơn.

Miss có risk lớn hơn.

---

<a id="m12-t031-s061"></a>

## 24. DAO / ĐOẢN BINH

Fantasy:

**nhanh và hiệu quả khi áp sát.**

Strengths:

speed;

close quarters;

concealment.

Weakness:

reach.

---

<a id="m12-t031-s062"></a>

## 25. CUNG

Fantasy:

**distance + preparation.**

Strengths:

range;

opening attack;

target choice.

Weakness:

khi engaged rất bất lợi.

Ammunition hữu hạn.

---

<a id="m12-t031-s063"></a>

## 26. NỎ

Fantasy:

**dễ giữ thế ngắm nhưng reload chậm.**

So với cung:

Accuracy ổn định hơn với người được huấn luyện phù hợp.

Penetration cao.

Reload chậm.

Cơ cấu cần bảo trì.

Điều này đặc biệt phù hợp arc Cổ Loa.

---

<a id="m12-t031-s064"></a>

## 27. KHIÊN

Không đơn thuần:

Defense +10.

Cho phép:

Block.

Protect Adjacent.

Formation.

Cover against missile.

Shield push.

---

<a id="m12-t031-s065"></a>

## 28. VŨ KHÍ NÉM

Lao.

Dao.

Đá.

Có giá trị:

opening.

control.

hunting.

Không để player mang vô hạn.

---

<a id="m12-t031-s066"></a>

## 29. ARMOR

Không nên dùng armor như thanh Defense đơn giản.

Armor có:

Coverage.

Material.

Mobility.

Noise.

Heat.

Durability.

Protection Type.

---

<a id="m12-t031-s067"></a>

## 30. ARMOR PROTECTION

Armor có thể:

giảm severity wound;

chuyển lethal wound thành injury nhẹ hơn;

ngăn penetration.

Không chỉ giảm HP damage.

---

<a id="m12-t031-s068"></a>

## 31. HEALTH MODEL — KHÔNG NÊN CHỈ CÓ HP

Đây là quyết định quan trọng.

Đề xuất sử dụng ba lớp:

<a id="m12-t031-s069"></a>

## VITALITY

Khả năng tiếp tục chiến đấu.

<a id="m12-t031-s070"></a>

## WOUNDS

Thương tích thật.

<a id="m12-t031-s071"></a>

## CONDITION

Chảy máu, gãy, choáng...

---

<a id="m12-t031-s072"></a>

## 32. VITALITY

Vitality đại diện:

- fatigue chiến đấu;
- bruising;
- minor cuts;
- breath;
- combat readiness.

Vitality có thể hồi tương đối nhanh.

---

<a id="m12-t031-s073"></a>

## 33. WOUND

Wound là thương tích thực.

Ví dụ:

Minor.

Serious.

Critical.

Wound không hồi sau một giấc ngủ.

Cần:

treatment;

days/weeks;

rest.

---

<a id="m12-t031-s074"></a>

## 34. DAMAGE FLOW

Attack trúng.

↓

Determine Impact.

↓

Armor Mitigation.

↓

Vitality Loss.

↓

Nếu Impact vượt threshold:

Wound Check.

↓

Wound.

---

<a id="m12-t031-s075"></a>

## 35. LOW VITALITY

Khi Vitality thấp:

Reaction giảm.

Movement giảm.

Resolve check khó hơn.

Nhưng không tự động chết.

---

<a id="m12-t031-s076"></a>

## 36. WOUND LOCATION

Không cần mô phỏng cực chi tiết mọi bộ phận.

Có thể dùng:

Head.

Torso.

Arm.

Leg.

Mỗi vùng tạo consequence.

---

<a id="m12-t031-s077"></a>

## 37. ARM WOUND

Có thể:

giảm weapon handling;

mất shield;

không dùng bow.

---

<a id="m12-t031-s078"></a>

## 38. LEG WOUND

Movement giảm.

Không thể sprint.

Retreat khó.

---

<a id="m12-t031-s079"></a>

## 39. TORSO WOUND

Stamina/Vitality regeneration giảm.

Có thể bleeding.

---

<a id="m12-t031-s080"></a>

## 40. HEAD WOUND

Concussion.

Stun.

Perception giảm.

Critical risk cao.

---

<a id="m12-t031-s081"></a>

## 41. BLEEDING

Một trong những condition nguy hiểm nhất.

Combat không cần ngay lethal nếu wound gây bleeding.

Dược Nhân có giá trị lớn.

---

<a id="m12-t031-s082"></a>

## 42. DOWNED

Character không nhất thiết chết khi Vitality = 0.

Có thể:

Collapse.

Incapacitated.

Unconscious.

Surrender.

Critical Wound.

Death phụ thuộc severity.

---

<a id="m12-t031-s083"></a>

## 43. DEATH

Không nên quá dễ đến mức random arrow xoá 30 giờ chơi mà player không có agency.

Nhưng cũng không được vô nghĩa.

Đề xuất:

**Death thường là kết quả của chuỗi failure**, ví dụ:

Serious Wound

+

Bleeding

+

không retreat

+

không treatment.

Critical direct death có thể tồn tại nhưng hiếm.

---

<a id="m12-t031-s084"></a>

## 44. PLAYER CHARACTER DEATH

Tùy Game Mode.

Standard:

Downed + rescue window rộng hơn.

Hardcore:

death thực sự nguy hiểm.

Legacy/Ironman:

campaign succession.

Chi tiết khóa tại Death Module.

---

<a id="m12-t031-s085"></a>

## 45. PAIN

Có thể có một derived condition:

Pain.

Ảnh hưởng:

Focus.

Precision.

Morale.

TÂM/THỂ giúp chịu.

Không cần micromanage thành resource riêng nếu UI quá nặng.

---

<a id="m12-t031-s086"></a>

## 46. STAMINA

Dùng cho:

heavy attack;

block;

sprint;

grapple;

special physical Technique.

Stamina hồi trong combat một phần.

Không dùng mana.

---

<a id="m12-t031-s087"></a>

## 47. FOCUS

Một số Technique dùng Focus.

Focus không phải mana phép thuật.

Nó đại diện:

concentration;

mental attention;

preparation.

Dùng bởi:

Liệp Nhân;

Xảo Nhân;

Dược Nhân;

Trinh Binh.

---

<a id="m12-t031-s088"></a>

## 48. RESOLVE

Resolve đại diện willingness to continue fighting.

Phụ thuộc:

TÂM.

KHÍ.

Morale.

Leadership.

Fear.

Casualty.

---

<a id="m12-t031-s089"></a>

## 49. MORALE

Cả player party và enemy đều có Morale.

Một combat có thể kết thúc vì:

enemy chạy.

enemy đầu hàng.

party retreat.

Không cần chết hết.

---

<a id="m12-t031-s090"></a>

## 50. MORALE EVENTS

Morale thay đổi khi:

leader ngã;

ally chết;

bị flank;

bị ambush;

reinforcement tới;

objective mất;

fire xuất hiện;

supernatural entity xuất hiện;

Thủ Lĩnh rally.

---

<a id="m12-t031-s091"></a>

## 51. HUMAN ENEMY SHOULD WANT TO LIVE

Một farmer cầm giáo không đánh tới chết trong mọi tình huống.

Khi:

bị thương;

leader chết;

outnumbered;

objective thất bại

họ có thể bỏ chạy.

Điều này rất quan trọng để combat có cảm giác con người.

---

<a id="m12-t031-s092"></a>

## 52. SURRENDER

Enemy có thể đầu hàng.

Player quyết định:

thả;

bắt;

giết;

trao đổi.

Tất cả tạo narrative consequence.

---

<a id="m12-t031-s093"></a>

## 53. KILLING SURRENDERED ENEMY

Không đơn giản:

Evil +10.

Companions interpret.

Faction reacts.

Witness matters.

Rumor spreads.

---

<a id="m12-t031-s094"></a>

## 54. NON-LETHAL COMBAT

Phải hỗ trợ.

Options:

Grapple.

Disarm.

Knockdown.

Intimidate.

Restrain.

Force surrender.

Không phải mọi weapon đều phù hợp.

---

<a id="m12-t031-s095"></a>

## 55. GRAPPLE

Grapple quan trọng ở:

thuyền;

nhà nhỏ;

bắt sống.

Dựa:

THỂ + Vật.

Có states:

Grabbed.

Controlled.

Pinned.

---

<a id="m12-t031-s096"></a>

## 56. DISARM

Không nên easy universal action.

Cần:

weapon situation;

skill;

position.

Xảo/weapon expertise có thể giúp.

---

<a id="m12-t031-s097"></a>

## 57. KNOCKBACK / POSITION CONTROL

Tactical depth của Zone combat cần position interaction.

Có thể:

push sang sub-position;

đẩy khỏi cầu;

đẩy xuống nước;

ép target khỏi cover.

Không cần tile-by-tile vẫn tactical.

---

<a id="m12-t031-s098"></a>

## 58. SUB-POSITION TRONG ZONE

Để Zone không quá thô, mỗi Zone có thể có trạng thái:

Front.

Center.

Rear/Cover.

Không phải coordinate chính xác.

Ví dụ:

cùng Zone nhưng:

Shield user Front.

Archer Rear.

---

<a id="m12-t031-s099"></a>

## 59. FRONTAGE

Zone có Frontage.

Ví dụ cầu:

Frontage 1–2.

Open field:

4+.

Spear formation mạnh khi Frontage phù hợp.

---

<a id="m12-t031-s100"></a>

## 60. FORMATION SYSTEM

Formation rất hợp setting.

Không cần hàng chục formation.

Có thể có:

Loose.

Shield Line.

Spear Line.

Column.

Encircle.

Formation yêu cầu:

space;

members;

training;

leadership.

---

<a id="m12-t031-s101"></a>

## 61. FORMATION KHÔNG PHẢI BUTTON BUFF

Ví dụ Shield Line cần:

2+ shield user.

Adjacent/Front position.

Cohesion.

Nếu một người ngã:

formation có gap.

---

<a id="m12-t031-s102"></a>

## 62. FORMATION TRAINING

Party không tự phối hợp hoàn hảo.

Cần:

shared combat;

training;

Tactical Trust.

Party lâu năm formation hiệu quả hơn nhóm vừa gặp.

---

<a id="m12-t031-s103"></a>

## 63. FRIENDLY FORMATION

Lạc Binh + Hộ Vệ + Thủ Lĩnh synergy tự nhiên.

Nhưng không bắt buộc.

Party không formation vẫn chơi được.

---

<a id="m12-t031-s104"></a>

## 64. TERRAIN

Terrain phải quan trọng ngang character stat.

Ví dụ:

Mud.

Tall Grass.

Water.

Slope.

Narrow Path.

Rocks.

Trees.

Building.

Boat.

Fire.

Smoke.

---

<a id="m12-t031-s105"></a>

## 65. HIGH GROUND

High Ground có:

visibility.

missile advantage.

movement defense.

Nhưng không nên universal +20%.

Effect phụ thuộc weapon.

---

<a id="m12-t031-s106"></a>

## 66. WATER COMBAT

Đây là một signature system của game.

Nước có depth:

Shallow.

Waist.

Deep.

Swimming.

Current.

Character không có Water skill sẽ rất nguy hiểm.

Thủy Binh trở nên khác biệt rõ ràng.

---

<a id="m12-t031-s107"></a>

## 67. BOAT COMBAT

Zone map có thể là:

```text
Enemy Bow
Enemy Center
Enemy Stern
Water
Player Bow
Player Center
Player Stern
```

Actions:

Board.

Push away.

Cut rope.

Damage boat.

Jump.

Rescue.

Fire.

Không cần render hàng trăm tile.

---

<a id="m12-t031-s108"></a>

## 68. FOREST COMBAT

Features:

Visibility blocked.

Ambush.

Tree cover.

Noise.

Fire risk.

Hidden paths.

Liệp Nhân/Trinh Binh mạnh.

---

<a id="m12-t031-s109"></a>

## 69. SWAMP COMBAT

Mud.

Poison.

Hidden water.

Movement risk.

Local Knowledge cực mạnh.

Một Lạc Binh mạnh ở đồng bằng có thể cực kỳ bất lợi.

---

<a id="m12-t031-s110"></a>

## 70. VILLAGE COMBAT

Buildings.

Civilians.

Fire.

Narrow lanes.

Objective protection.

Collateral consequences.

Không nên biến làng thành arena vô hồn.

---

<a id="m12-t031-s111"></a>

## 71. NIGHT COMBAT

Visibility giảm.

Fire/light tạo detection.

GIÁC.

Memory of terrain.

Night Techniques.

Không đơn giản accuracy penalty toàn cục.

---

<a id="m12-t031-s112"></a>

## 72. WEATHER

Rain:

bow handling.

fire.

visibility.

mud.

river.

Storm:

boat.

sound.

communication.

Weather phải tác động cả tactical và narrative.

---

<a id="m12-t031-s113"></a>

## 73. PRE-COMBAT PHASE

Một trong các phần quan trọng nhất.

Nếu party biết trước confrontation:

player có thể:

Scout.

Observe.

Negotiate.

Set Trap.

Choose Terrain.

Prepare Medicine.

Choose Formation.

Hide.

Set Escape Route.

---

<a id="m12-t031-s114"></a>

## 74. PREPARATION CÓ THỂ QUYẾT ĐỊNH TRẬN

Một Liệp Nhân tốt có thể khiến trận:

4 vs 8

trở thành:

4 vs 3

rồi phần còn lại mất morale.

Combat power không chỉ nằm trong combat turn.

---

<a id="m12-t031-s115"></a>

## 75. AMBUSH

Ambush không chỉ:

free attack.

Nó có thể:

reduce morale.

split enemy.

control starting positions.

deny formation.

---

<a id="m12-t031-s116"></a>

## 76. NEGOTIATION BEFORE COMBAT

Có thể combat được tránh ngay ở Pre-combat.

Ví dụ bandit chỉ muốn food.

Player:

pay.

intimidate.

bargain.

fight.

---

<a id="m12-t031-s117"></a>

## 77. MID-COMBAT SOCIAL ACTION

Trong trận vẫn có thể:

shout warning.

demand surrender.

rally.

call someone's name.

reveal information.

Sứ Giả/Thủ Lĩnh không biến mất khi combat bắt đầu.

---

<a id="m12-t031-s118"></a>

## 78. PERSONAL CONNECTION IN COMBAT

Nếu enemy là:

former companion;

family;

known NPC

dialogue/action đặc biệt xuất hiện.

Có thể hesitate.

Không phải mọi enemy là unit vô danh.

---

<a id="m12-t031-s119"></a>

## 79. COMBAT OBJECTIVES

Combat encounter phải khai báo objective.

Ví dụ:

Eliminate.

Capture.

Escape.

Protect.

Delay.

Break Formation.

Reach Location.

Destroy Object.

Hold Zone.

Steal Object.

Rescue.

Survive X rounds.

---

<a id="m12-t031-s120"></a>

## 80. MULTIPLE OBJECTIVES

Ví dụ:

Main:

cứu trẻ.

Secondary:

bắt sống thủ lĩnh.

Optional:

không để nhà cháy.

Combat outcome có nhiều mức.

---

<a id="m12-t031-s121"></a>

## 81. VICTORY STATES

Không chỉ Victory/Defeat.

Ví dụ:

Perfect Success.

Success.

Pyrrhic Success.

Partial Success.

Failure with Survival.

Catastrophe.

Narrative Engine nhận outcome.

---

<a id="m12-t031-s122"></a>

## 82. PYRRHIC VICTORY

Bạn giữ được cầu.

Nhưng companion chết.

Về chiến thuật:

win.

Về story:

có thể là tragedy.

Game phải ghi đúng.

---

<a id="m12-t031-s123"></a>

## 83. RETREAT

Retreat luôn phải là option nếu hợp lý.

Không coi retreat là game failure.

Player có thể:

break engagement.

cover retreat.

abandon supplies.

carry wounded.

---

<a id="m12-t031-s124"></a>

## 84. CARRY WOUNDED

THỂ + movement.

Một companion bị thương có thể làm party rút chậm.

Quyết định:

bỏ lại?

mang?

chặn địch?

Tạo moral consequence thật.

---

<a id="m12-t031-s125"></a>

## 85. ESCAPE ROUTE

Nếu chuẩn bị trước:

retreat dễ hơn.

Trinh Binh có value.

---

<a id="m12-t031-s126"></a>

## 86. PURSUIT

Combat có thể chuyển thành Chase Encounter thay vì tiếp tục turn combat.

Enemy chạy.

Player chọn:

pursue.

let go.

Tracking later.

---

<a id="m12-t031-s127"></a>

## 87. CHASE SYSTEM

Không cần nằm hoàn toàn trong Combat Grid.

Dùng:

Zone/Skill challenge.

Movement.

Stamina.

Terrain.

Một fight có thể trở thành chase.

---

<a id="m12-t031-s128"></a>

## 88. ENEMY DESIGN

Không design enemy chỉ bằng:

HP.

Damage.

Enemy cần:

Goal.

Morale.

Training.

Equipment.

Knowledge.

Leadership.

---

<a id="m12-t031-s129"></a>

## 89. FARMER MILITIA

Ví dụ:

Combat Skill thấp.

Formation trung bình nếu được luyện.

Morale bảo vệ làng cao.

Không chase xa vì goal là bảo vệ nhà.

---

<a id="m12-t031-s130"></a>

## 90. BANDIT

Goal:

resource.

Không muốn chết.

Nếu resistance quá mạnh:

rút.

Có ambush knowledge.

---

<a id="m12-t031-s131"></a>

## 91. TRAINED WARRIOR

Skill cao.

Formation tốt.

Morale ổn.

Biết retreat có tổ chức.

---

<a id="m12-t031-s132"></a>

## 92. HUNTER

Không fight trực diện.

Ambush.

Range.

Terrain.

---

<a id="m12-t031-s133"></a>

## 93. SUPERNATURAL ENEMY

Không tuân hoàn toàn human morale.

Nhưng phải có:

behaviour.

need.

fear nếu có.

weakness.

Không đơn giản:

monster = HP sponge.

---

<a id="m12-t031-s134"></a>

## 94. ANIMAL COMBAT

Động vật muốn:

hunt.

defend territory.

protect young.

escape.

Animal không chiến đấu tới chết vô nghĩa.

---

<a id="m12-t031-s135"></a>

## 95. PREDATOR

Nếu bị thương nặng:

thường retreat.

Trừ khi:

trapped.

starving.

supernatural influence.

---

<a id="m12-t031-s136"></a>

## 96. BOSS DESIGN

Boss không nên:

20x HP.

Boss có thể mạnh nhờ:

Environment.

Followers.

Knowledge.

Technique.

Unique behaviour.

Objective.

---

<a id="m12-t031-s137"></a>

## 97. HUMAN BOSS

Ví dụ Chief Warrior.

Không cần siêu nhân.

Họ có:

bodyguards.

formation.

reputation.

command.

terrain.

Đánh bại mạng lưới đó mới là challenge.

---

<a id="m12-t031-s138"></a>

## 98. SUPERNATURAL BOSS

Có thể yêu cầu:

Observation.

Ritual.

Environmental action.

Knowledge.

Combat chỉ là một phần.

---

<a id="m12-t031-s139"></a>

## 99. BOSS PHASE

Phase không nên vì:

HP < 50% → rage.

Nên dựa trên:

event.

terrain change.

objective change.

morale.

ritual.

Ví dụ thủy linh:

nước dâng làm Zone thay đổi.

---

<a id="m12-t031-s140"></a>

## 100. LARGE-SCALE BATTLE

Không control 200 unit.

Dùng:

<a id="m12-t031-s141"></a>

## BATTLE LAYER

Input:

Forces.

Supply.

Morale.

Terrain.

Leadership.

Technology.

Intelligence.

---

<a id="m12-t031-s142"></a>

## 101. PLAYER ROLE TRONG LARGE BATTLE

Player nhận một **Tactical Mission** bên trong battle.

Ví dụ:

giữ cổng.

phá supply.

cứu commander.

chiếm hill.

protect engineers.

Outcome ảnh hưởng Battle Layer.

---

<a id="m12-t031-s143"></a>

## 102. BATTLE WITHOUT PLAYER

Nếu player không tham gia:

World Simulation resolve.

Không đóng băng lịch sử chờ player.

---

<a id="m12-t031-s144"></a>

## 103. WAR CASUALTY

Sau battle:

wounded.

dead.

missing.

captured.

deserter.

refugee.

Narrative Engine xử lý.

Không chỉ despawn quân.

---

<a id="m12-t031-s145"></a>

## 104. CLASS INTERACTION — LẠC BINH

Combat trực tiếp nhất.

Strength:

formation.

weapon technique.

pressure.

Không phải raw damage king.

---

<a id="m12-t031-s146"></a>

## 105. HỘ VỆ

Strength:

space protection.

reaction.

damage mitigation.

Không phải taunt magic.

---

<a id="m12-t031-s147"></a>

## 106. LIỆP NHÂN

Strength:

precombat.

ranged.

target analysis.

terrain.

Nếu bị kéo vào combat bất ngờ:

yếu hơn.

---

<a id="m12-t031-s148"></a>

## 107. TRINH BINH

Strength:

surprise.

position.

enemy information.

escape.

Không phải assassin mặc định.

---

<a id="m12-t031-s149"></a>

## 108. THỦY BINH

Strength:

water/boat.

movement.

rescue.

terrain manipulation.

---

<a id="m12-t031-s150"></a>

## 109. DƯỢC NHÂN

Trong combat:

stabilize.

reduce bleeding.

temporary treatment.

Không full heal wound giữa battle.

---

<a id="m12-t031-s151"></a>

## 110. XẢO NHÂN

Trong combat:

trap.

environment.

repair.

break structure.

improvised tools.

---

<a id="m12-t031-s152"></a>

## 111. VU CHÚC

Không spam magic attack.

Có thể:

morale.

ritual condition.

Mạch interaction.

fear.

spiritual entity-specific actions.

---

<a id="m12-t031-s153"></a>

## 112. SỨ GIẢ

Mid-combat:

surrender.

morale.

identify social leverage.

coordinate neutral NPC.

---

<a id="m12-t031-s154"></a>

## 113. THỦ LĨNH

Action efficiency.

formation.

rally.

coordinate reactions.

Không damage buff fantasy.

---

<a id="m12-t031-s155"></a>

## 114. NGƯỜI GIỮ KÝ ỨC

Combat utility phụ thuộc knowledge.

Ví dụ:

biết enemy tradition.

historic tactic.

creature story.

recognize insignia.

Ít combat trực tiếp nhưng có information leverage.

---

<a id="m12-t031-s156"></a>

## 115. SIGNATURE PASSIVE

Biography passive có thể tác động combat nhưng không nên mọi passive đều combat.

Ví dụ:

**Không Chạy Lần Nữa**

ally down → Resolve tăng nhưng retreat bị hạn chế.

Đây là combat + psychology.

---

<a id="m12-t031-s157"></a>

## 116. FEAR

Fear có source.

Human fear.

Animal fear.

Supernatural fear.

Personal Trauma.

Không phải một debuff chung.

---

<a id="m12-t031-s158"></a>

## 117. PANIC

Resolve thất bại nghiêm trọng:

character có thể:

freeze.

retreat.

act irrationally.

Response dựa Psychology.

Player character cũng có thể bị ảnh hưởng.

Nhưng không nên mất control quá thường xuyên.

---

<a id="m12-t031-s159"></a>

## 118. PLAYER AGENCY VỚI FEAR

Có thể dùng:

Preparation.

Companion reassurance.

Ritual.

Training.

Exposure.

Class Technique.

để giảm.

---

<a id="m12-t031-s160"></a>

## 119. INJURY AFTERMATH

Sau trận:

Triage.

Treatment.

Recovery.

Permanent consequence.

Một combat 5 phút có thể tạo story kéo dài 3 tháng game-time.

---

<a id="m12-t031-s161"></a>

## 120. SCARS

Một số wound tạo scar.

Có thể ảnh hưởng:

appearance.

memory.

social recognition.

Không nhất thiết stat.

---

<a id="m12-t031-s162"></a>

## 121. PERMANENT DISABILITY

Hiếm nhưng có thể.

Không phải auto game over.

Kết nối Career Transition.

Ví dụ mất khả năng dùng cung:

Liệp Nhân có thể chuyển:

tracker.

mentor.

leader.

---

<a id="m12-t031-s163"></a>

## 122. EQUIPMENT DURABILITY

Không cần weapon break spam.

Durability nên chậm.

Nhưng:

block nặng.

poor maintenance.

water.

fire.

có thể làm hỏng.

Xảo Nhân có vai trò.

---

<a id="m12-t031-s164"></a>

## 123. WEAPON QUALITY

Quality ảnh hưởng:

reliability.

handling.

durability.

Không nên damage khác nhau quá phi lý.

Một spear masterwork không gây gấp 4 damage spear thường.

---

<a id="m12-t031-s165"></a>

## 124. EQUIPMENT FIT

Weapon được làm phù hợp character có thể:

handling tốt hơn.

Nỏ Cần-style custom equipment.

Đây kết nối Crafting.

---

<a id="m12-t031-s166"></a>

## 125. AMMUNITION

Arrow/bolt hữu hạn.

Nhưng không nên inventory micromanagement cực đoan.

Có thể bundle theo quiver.

Sau trận có retrieval chance.

---

<a id="m12-t031-s167"></a>

## 126. FIRE

Fire rất mạnh nhưng nguy hiểm.

Có thể lan.

destroy homes.

cause smoke.

panic animals.

Dùng lửa trong làng có hậu quả narrative.

---

<a id="m12-t031-s168"></a>

## 127. TRAPS

Trap cần preparation.

Không cast giữa combat vô lý.

Xảo Nhân/Liệp Nhân có thể đặt nhanh một số simple device.

---

<a id="m12-t031-s169"></a>

## 128. ENVIRONMENTAL KILL

Đẩy người xuống vực/nước có thể lethal.

Không được balance bằng cách làm enemy immune.

Nhưng:

enemy cũng làm được với player.

Risk symmetric.

---

<a id="m12-t031-s170"></a>

## 129. INFORMATION BEFORE COMBAT

Player có thể biết:

enemy count.

weapon.

morale.

leader.

route.

unknown.

Trinh Binh/Liệp Nhân rất giá trị.

Không phải UI mặc định hiển thị tất cả enemy stat.

---

<a id="m12-t031-s171"></a>

## 130. FOG OF WAR

Không cần visual RTS fog quá nặng.

Nhưng enemy ngoài vision:

không có full information.

Sound clue.

Movement clue.

Scout.

---

<a id="m12-t031-s172"></a>

## 131. UNKNOWN ENEMY

Creature lạ không hiện:

HP 84/100.

Player học qua combat.

Người Giữ Ký Ức hoặc Liệp Nhân có thể mở info.

---

<a id="m12-t031-s173"></a>

## 132. HEALTH INFORMATION

Không cần exact HP enemy.

Có thể hiển thị:

Unhurt.

Winded.

Wounded.

Badly Wounded.

Collapsing.

Tăng immersion.

---

<a id="m12-t031-s174"></a>

## 133. PLAYER PARTY HEALTH

Có thể hiển thị chính xác hơn vì character hiểu bản thân/đồng đội.

Nhưng wound cần qualitative description.

---

<a id="m12-t031-s175"></a>

## 134. TURN TIMER

Không nên có realtime turn timer mặc định.

Game tập trung suy nghĩ.

Hard mode có thể hạn chế một số planning information.

---

<a id="m12-t031-s176"></a>

## 135. COMBAT SAVE SCUM

Không cần giải quyết bằng cấm save ngay trong Combat Module.

Nhưng game nên thiết kế fail-forward để player ít muốn reload.

---

<a id="m12-t031-s177"></a>

## 136. RANDOMNESS

2D10 giúp giảm swing.

Master không fail việc đơn giản.

Combat vẫn có uncertainty.

Critical result nên đến từ margin, không chỉ natural roll.

---

<a id="m12-t031-s178"></a>

## 137. CRITICAL SUCCESS

Không nhất thiết x2 damage.

Có thể:

disarm.

position advantage.

morale shock.

reduce wound severity.

open reaction.

---

<a id="m12-t031-s179"></a>

## 138. CRITICAL FAILURE

Không luôn:

weapon breaks.

Có thể:

overextension.

stumble.

lose formation.

expose ally.

Contextual.

---

<a id="m12-t031-s180"></a>

## 139. NO CRITICAL ON ROUTINE TASK

Nếu auto-success:

không roll.

Không có absurd failure.

---

<a id="m12-t031-s181"></a>

## 140. AI BEHAVIOUR

Enemy AI dựa trên:

Goal.

Training.

Morale.

Personality.

Knowledge.

Không phải một universal combat AI.

---

<a id="m12-t031-s182"></a>

## 141. UNTRAINED ENEMY

Có thể:

cluster.

panic.

chase recklessly.

---

<a id="m12-t031-s183"></a>

## 142. TRAINED ENEMY

Hold formation.

protect leader.

retreat organized.

focus objective.

---

<a id="m12-t031-s184"></a>

## 143. HUNTER AI

Avoid melee.

use cover.

withdraw.

---

<a id="m12-t031-s185"></a>

## 144. DESPERATE ENEMY

Có thể:

fight aggressively.

surrender unexpectedly.

flee.

Psychology.

---

<a id="m12-t031-s186"></a>

## 145. ENEMY KNOWLEDGE

Enemy không biết Class player tự động.

Nếu chưa từng gặp nỏ:

phản ứng khác.

Nếu biết player nổi tiếng:

target strategy có thể thay đổi.

---

<a id="m12-t031-s187"></a>

## 146. REPUTATION IN COMBAT

Một famous warrior xuất hiện:

enemy morale có thể giảm.

Hoặc:

enemy muốn chứng minh mình.

Không universal buff.

---

<a id="m12-t031-s188"></a>

## 147. DUEL

Một số conflict có thể giải bằng duel.

Không phải minigame riêng.

Dùng cùng combat system nhưng:

1v1.

social stakes.

surrender.

witness.

---

<a id="m12-t031-s189"></a>

## 148. MASSACRE PREVENTION BY DESIGN

Combat objective và morale giúp trận thường kết thúc trước khi tất cả chết.

Nếu player cố giết người đang chạy:

đó là intentional action.

Thế giới ghi nhớ.

---

<a id="m12-t031-s190"></a>

## 149. CAPTURE SYSTEM

Downed hoặc surrendered enemy có thể bị:

bound.

guarded.

released.

exchanged.

Prisoner consumes resources.

Không phải free loot.

---

<a id="m12-t031-s191"></a>

## 150. PRISONER INFORMATION

Interrogation tuân Social/Psychology system.

Không:

click Torture → truth.

Prisoner có Knowledge riêng.

Có thể nói sai.

---

<a id="m12-t031-s192"></a>

## 151. COMBAT TO NARRATIVE TRANSITION

Sau trận engine tạo:

Combat Event Record.

Ví dụ:

Participants.

Objective.

Deaths.

Wounds.

Surrender.

Witnesses.

Actions.

Loot.

Property Damage.

Player Decision.

---

<a id="m12-t031-s193"></a>

## 152. MEMORY GENERATION

Companion có thể nhớ:

“Player cứu tôi.”

“Player bỏ người chạy.”

“Player giết người đầu hàng.”

“Player giữ bình tĩnh.”

Combat trực tiếp tạo Psychology/Social Events.

---

<a id="m12-t031-s194"></a>

## 153. RUMOR GENERATION

Settlement nghe:

> “Bốn người đánh bại mười hai.”

Sự thật có thể:

enemy thực ra rút sau leader ngã.

Sau nhiều retelling:

Myth Formation.

---

<a id="m12-t031-s195"></a>

## 154. COMBAT REWARD

Không dùng combat XP lớn làm reward chính.

Reward:

Skill Practice.

Milestone.

Equipment.

Information.

Reputation.

Relationship.

Story outcome.

Class Evidence.

---

<a id="m12-t031-s196"></a>

## 155. WHY NO GENERAL COMBAT XP?

Nếu combat cho progression mạnh nhất:

player sẽ giết để farm.

Đi ngược game philosophy.

Skill growth đến từ challenge.

Class evidence đến từ meaningful action.

---

<a id="m12-t031-s197"></a>

## 156. KILL COUNT KHÔNG PHẢI PROGRESSION

Liệp Nhân không Master vì giết 100 con thú.

Lạc Binh không Master vì kill count.

Milestone phải có:

context.

difficulty.

responsibility.

---

<a id="m12-t031-s198"></a>

## 157. ENCOUNTER BUDGET

Mỗi region nên có ít combat encounter hơn RPG thông thường.

Nhưng mỗi encounter có:

terrain identity.

objective.

story context.

enemy behaviour.

---

<a id="m12-t031-s199"></a>

## 158. RANDOM ENCOUNTER

Không nên dùng:

“20% chance bandit attack”.

Nếu road insecurity cao:

bandit event eligible.

Player có thể thấy warning.

avoid.

negotiate.

fight.

Randomness chỉ chọn trong causal context.

---

<a id="m12-t031-s200"></a>

## 159. ENCOUNTER TELEGRAPHING

Các nguy hiểm thường có dấu:

tracks.

rumor.

corpse.

smoke.

silence.

scout report.

Player chuẩn bị được.

---

<a id="m12-t031-s201"></a>

## 160. UNAVOIDABLE COMBAT

Có thể tồn tại.

Nhưng hiếm.

Thường là consequence của:

surprise.

bad planning.

story event.

---

<a id="m12-t031-s202"></a>

## 161. TUTORIAL COMBAT

Không nên tutorial bằng arena giả.

Dùng Starting Origin.

Ví dụ Tam Giang:

giải cứu người trong xô xát trên thuyền.

Bãi Lạc:

xua thú.

Bắc Lộ:

phòng thủ raid.

Player học combat trong life context.

---

<a id="m12-t031-s203"></a>

## 162. DIFFICULTY

Không tăng bằng HP sponge.

Có thể thay:

Enemy Competence.

AI Coordination.

Information scarcity.

Wound severity.

Morale resilience.

Resource pressure.

---

<a id="m12-t031-s204"></a>

## 163. STORY MODE

Lower lethality.

More information.

Wider stabilization window.

Không làm enemy ngu hoàn toàn.

---

<a id="m12-t031-s205"></a>

## 164. STANDARD

Full system.

---

<a id="m12-t031-s206"></a>

## 165. TACTICAL MODE

Enemy uses formation tốt hơn.

Wound nghiêm hơn.

Preparation quan trọng hơn.

---

<a id="m12-t031-s207"></a>

## 166. LEGACY / IRONMAN

Permanent consequences.

Character death → succession.

Không necessarily harder enemy stats.

---

<a id="m12-t031-s208"></a>

## 167. ACCESSIBILITY

Zone-based giúp giảm cognitive load.

UI phải thể hiện rõ:

Zone.

Threat.

Range.

Cover.

Engagement.

Không bắt player nhớ hidden geometry.

---

<a id="m12-t031-s209"></a>

## 168. COMBAT CAMERA / VISUAL

Vì game không ưu tiên graphics:

có thể dùng:

2D illustrated battlefield;

isometric minimal 3D;

stylized 2.5D.

Zone-based rất linh hoạt.

Quan trọng là readable.

---

<a id="m12-t031-s210"></a>

## 169. LOW-BUDGET PRESENTATION OPTION

Mỗi Zone như một illustrated panel.

Characters represented bằng portrait/token.

Effects bằng animation ngắn.

Điều này phù hợp nếu development resources thấp.

---

<a id="m12-t031-s211"></a>

## 170. HIGHER-BUDGET OPTION

Isometric 3D environments.

Zone boundary mềm, không cần grid overlay.

Rules vẫn giống.

Do đó core system không phụ thuộc production budget.

---

<a id="m12-t031-s212"></a>

## 171. OPTION HYBRID GRID

Nếu prototype cho thấy Zone thiếu precision:

Có thể dùng:

Zone + Local Slots.

Ví dụ mỗi Zone có 4–6 Position Slot.

Không cần full grid.

Đây là phương án dự phòng tôi đánh giá rất cao.

---

<a id="m12-t031-s213"></a>

## 172. ZONE + SLOT MODEL

Ví dụ:

```text
ZONE: Cầu gỗ

[Front Left] [Front Right]
[Rear Left]  [Rear Right]
```

Cho phép:

front line.

rear line.

flank.

mà vẫn rẻ hơn grid.

---

<a id="m12-t031-s214"></a>

## 173. ĐỀ XUẤT CUỐI CÙNG VỀ KHÔNG GIAN

Khuyến nghị prototype theo hai mức:

<a id="m12-t031-s215"></a>

#### Prototype A

Pure Zone.

<a id="m12-t031-s216"></a>

#### Prototype B

Zone + Local Slots.

Sau 20–30 encounter test:

chọn.

Tôi nghiêng **Zone + Local Slots** nếu combat là một selling point đáng kể.

Nếu narrative là ưu tiên tuyệt đối:

Pure Zone.

---

<a id="m12-t031-s217"></a>

## 174. COMBAT LOOP

Combat loop hoàn chỉnh:

```text
Threat Detection
↓
Pre-combat Information
↓
Preparation / Negotiation
↓
Initial Position
↓
Combat Round
↓
Action / Reaction / Morale
↓
Objective State Changes
↓
Retreat / Surrender / Victory / Defeat
↓
Triage
↓
Loot / Prisoner / Property
↓
Relationship Reactions
↓
World State Update
↓
Memory / Rumor / Story Thread
```

Combat không kết thúc ở màn:

**VICTORY.**

---

<a id="m12-t031-s218"></a>

## 175. COMBAT QUALITY GATE

Mỗi authored major combat phải trả lời được:

1. Tại sao trận này xảy ra?
2. Có cách tránh không?
3. Objective là gì?
4. Terrain khác encounter khác ở đâu?
5. Enemy muốn gì?
6. Enemy có lý do retreat/surrender không?
7. Party non-combat class đóng góp thế nào?
8. Preparation thay đổi trận ra sao?
9. Failure có tiếp tục story không?
10. Victory tạo consequence gì?
11. Injury/death có được world xử lý không?
12. Có ít nhất một solution ngoài “kill everybody” không?

Nếu không trả lời được phần lớn:

combat encounter chưa đạt.

---

<a id="m12-t031-s219"></a>

## 176. NHỮNG THỨ KHÔNG NÊN LÀM

Không sử dụng:

- enemy level scaling tuyệt đối;
- HP sponge boss;
- trash mob liên tục;
- Class locked equipment vô lý;
- kill XP farming;
- full heal sau combat;
- resurrection phổ biến;
- persuasion magic giữa combat;
- enemy fight-to-death mặc định;
- companion immortal;
- environmental hazard chỉ là damage tick;
- weapon khác nhau chủ yếu bằng damage;
- 50 loại status effect khó đọc;
- 20 buff stack đồng thời.

Combat cần sâu nhờ **quyết định**, không nhờ số lượng modifier.

---

<a id="m12-t031-s220"></a>

## 177. PHƯƠNG ÁN A — “TACTICAL D&D”

Nếu sau prototype team muốn combat sâu hơn:

**Zone + Slot**

Party 4.

Movement.

Reach.

Facing-lite.

Formation.

Wound.

Morale.

Đây là phương án tactical nhất.

Ưu:

combat có thể trở thành selling point.

Nhược:

production workload cao hơn.

---

<a id="m12-t031-s221"></a>

## 178. PHƯƠNG ÁN B — “NARRATIVE TACTICAL” — KHUYẾN NGHỊ

Pure Zone.

Party 4.

Ít trận.

Objective-driven.

Wound.

Morale.

Preparation mạnh.

Tactical thông qua:

terrain;

position;

class;

resource;

information.

Đây là phương án cân bằng nhất với toàn GDD.

---

<a id="m12-t031-s222"></a>

## 179. PHƯƠNG ÁN C — “LIGHTWEIGHT TABLETOP”

Zone rất abstract.

Combat nhanh 5–10 phút.

Nhiều check narrative.

Ít animation.

Thích hợp nếu:

budget nhỏ;

content/lore là absolute priority.

Nhược:

item/class combat depth giảm.

---

<a id="m12-t031-s223"></a>

## 180. SO SÁNH CUỐI

| Tiêu chí | Grid Tactical | Zone + Slot | Pure Zone | Abstract |
|---|---:|---:|---:|---:|
| Tactical Depth | 10 | 9 | 7 | 4 |
| Narrative Flow | 6 | 8 | 9 | 10 |
| Dev Cost | 3 | 6 | 8 | 10 |
| Procedural Encounter | 5 | 8 | 9 | 10 |
| Weapon Identity | 10 | 9 | 8 | 5 |
| Party Interaction | 9 | 9 | 9 | 7 |
| Mobile/Low-spec viability | 4 | 7 | 9 | 10 |
| Replayability | 7 | 9 | 9 | 8 |
| Phù hợp GDD hiện tại | 7 | **10** | **9** | 7 |

---

<a id="m12-t031-s224"></a>

## 181. KHUYẾN NGHỊ CHÍNH THỨC CHO GDD

Tôi đề xuất khóa:

<a id="m12-t031-s225"></a>

## ZONE + LOCAL POSITION SLOT TURN-BASED COMBAT

Cấu trúc:

**3–8 Battlefield Zones**

Mỗi zone có:

**2–6 Local Position Slots** khi cần.

Những encounter đơn giản có thể bỏ Slot và dùng Pure Zone.

Điều này tạo một combat system có khả năng **scale complexity theo encounter**.

Ví dụ:

Social fight nhỏ:

3 Zone.

Không slot.

Boss/war mission:

7 Zone.

Có front/rear/high-ground slots.

Không cần hai combat engine riêng.

---

<a id="m12-t031-s226"></a>

## 182. BỐN TRỤ CỘT COMBAT CHÍNH THỨC

<a id="m12-t031-s227"></a>

### INFORMATION

Biết trước đối phương là lợi thế lớn.

---

<a id="m12-t031-s228"></a>

### POSITION

Đứng đâu quan trọng hơn một vài điểm stat.

---

<a id="m12-t031-s229"></a>

### CONDITION

Fatigue, wound, morale và equipment matter.

---

<a id="m12-t031-s230"></a>

### CONSEQUENCE

Mọi trận chiến đều để lại hậu quả.

---

<a id="m12-t031-s231"></a>

## 183. NĂM THỨ PLAYER PHẢI SUY NGHĨ MỖI TRẬN

Không phải:

> skill nào damage lớn nhất?

Mà:

**Tôi thực sự cần đạt mục tiêu gì?**

**Ai đang gặp nguy hiểm nhất?**

**Địa hình nào đang có lợi?**

**Khi nào nên rút?**

**Nếu tôi giết người này thì chuyện gì xảy ra sau đó?**

Nếu combat tạo được năm câu hỏi đó thường xuyên, hệ thống đã đạt đúng tinh thần game.

---

<a id="m12-t031-s232"></a>

## 184. INTERACTION WITH ALL PREVIOUS SYSTEMS

Combat lấy:

**Attributes**

→ khả năng nền.

**Skills**

→ competence.

**Knowledge**

→ biết enemy/terrain.

**Technique**

→ action.

**Profession**

→ preparation/utilities.

**Class**

→ tactical identity.

**Psychology**

→ fear/morale/decision.

**Party**

→ coordination.

**Relationship**

→ tactical trust.

**World**

→ terrain/weather.

**Narrative**

→ objective/consequence.

**Career**

→ injury và reputation.

**Life Event**

→ combat tạo formative memories.

Không subsystem nào đứng riêng.

---

<a id="m12-t031-s233"></a>

## 185. DEFERRED TO PRODUCTION

Chưa khóa:

- exact damage numbers;
- Vitality formula;
- exact wound thresholds;
- armor numbers;
- weapon balance;
- slot count final;
- action costs từng Technique;
- AI weights;
- exact morale formula;
- exact critical probabilities;
- animation timing;
- UI layout;
- battle size maximum.

Các phần trên phải prototype rồi mới balance.

---

<a id="m12-t031-s234"></a>

## 186. CORE COMBAT STATEMENT

Combat của game không phải:

> “Tôi mạnh đến đâu?”

Combat phải đặt câu hỏi:

> **“Tôi hiểu tình huống này đến đâu, tôi sẵn sàng mạo hiểm cái gì, và cái giá của chiến thắng là bao nhiêu?”**

Một Liệp Nhân thắng vì biết con đường.

Một Hộ Vệ thắng vì không để đồng đội ngã.

Một Dược Nhân thắng vì giữ người sống.

Một Sứ Giả có thể thắng trước khi vũ khí rút khỏi vỏ.

Một Thủ Lĩnh thắng vì người khác vẫn đứng vững khi hàng ngũ sắp vỡ.

Một người bình thường có thể thắng đơn giản vì biết lúc nào nên chạy.

Đó là combat phù hợp nhất với thế giới chúng ta đang xây.

Từ góc độ hệ thống tổng thể, tôi **khuyến nghị mạnh Zone + Local Slots**, vì nó giữ được chiều sâu của giáo, khiên, đội hình, địa hình và party nhưng không kéo game vào chi phí thiết kế bản đồ kiểu tactical RPG nặng. Nó cũng rất hợp với các chiến trường đặc trưng của game như **thuyền, bờ sông, ruộng ngập, rừng, đầm lầy, đường núi và làng**.

Bước tiếp theo nên là **Weapons + Armor + Equipment + Injury/Status** thành một cụm, vì Combat vừa khóa các khái niệm reach, armor mitigation, wound và equipment ownership nhưng chưa định nghĩa chi tiết chúng hoạt động thế nào. Sau cụm đó ta mới chuyển sang **Exploration & Travel / Dungeon**, lúc đó vòng gameplay ngoài combat bắt đầu hoàn chỉnh.

<!-- SOURCE T031 END -->

