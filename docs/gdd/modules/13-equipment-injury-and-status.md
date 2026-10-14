# Module 13 — Vũ khí, giáp, trang bị, thương tích và trạng thái

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T033](../../source/conversation.md#t033)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Giữ item identity, provenance, ownership và condition. Sửa/salvage bảo toàn component; chữa trị không hoàn nguyên mọi wound lập tức. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Impact, armor, materials, tool quality, medical knowledge và ownership rights. |
| Đầu ra / bên sở hữu | Equipment condition/asset changes và persistent wounds; Assets/Health có owner riêng. |
| Đọc cùng | 12, 18, 21, 25–27.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Cùng injury ảnh hưởng combat, travel, work; salvage không để item và toàn bộ nguyên liệu cùng tồn tại. |
| Việc chưa được chứng minh | Mitigation, treatment rates và historical reference review. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T033 BEGIN -->

Dưới đây là module tiếp theo ở mức đủ để đưa vào GDD. Tôi chủ động thiết kế nó sao cho **itemization không phá triết lý “một đời người”**: một cây giáo tốt có thể theo nhân vật hàng chục năm, được sửa, đổi chủ, gãy, truyền lại và trở thành vật lịch sử; không có vòng lặp “thay kiếm xanh bằng kiếm tím vì +12 damage”.

<a id="m13-t033-s001"></a>

## GDD MODULE 13 — WEAPONS, ARMOR, EQUIPMENT, INJURY & STATUS SYSTEM

<a id="m13-t033-s002"></a>

## 1. DESIGN GOAL

Hệ thống này phải làm được bốn việc đồng thời:

**Weapons**

tạo tactical identity thông qua reach, speed, control, penetration và context.

**Armor**

thay đổi hậu quả của đòn đánh, không biến nhân vật thành “túi HP”.

**Equipment**

là tài sản có lịch sử, nguồn gốc, người chế tạo và chủ sở hữu.

**Injury**

khiến combat để lại hậu quả dài hạn lên nghề nghiệp, quan hệ và cuộc đời.

Mục tiêu cuối cùng:

> Người chơi nhớ “cây giáo cha để lại”, “vết thương ở chân từ trận Bắc Lộ” và “chiếc khiên Vũ dùng để cứu Mân” hơn là nhớ Item Level.

---

<a id="m13-t033-s003"></a>

## 2. HISTORICAL GROUNDING

Thiết kế được lấy cảm hứng từ material culture Đông Sơn nhưng **không khẳng định mọi thiết kế gameplay là phục dựng lịch sử**.

Các sưu tập và chuyên khảo khảo cổ cho thấy một phổ vũ khí đa dạng gồm giáo/lao, rìu chiến, dao găm, kiếm, cung/nỏ cùng một số dạng bảo vệ thân; giáo đồng Đông Sơn cũng có bằng chứng hiện vật trực tiếp. 

Một chuỗi khảo cứu của Bảo tàng Lịch sử Quốc gia dựa trên tư liệu hình tượng chiến binh nhấn mạnh tỷ trọng lớn của nhóm rìu/qua và giáo/lao trong corpus mà tác giả khảo sát; dao găm còn có vai trò biểu trưng và địa vị trong một số bối cảnh, vì vậy game không nên mặc định “kiếm = vũ khí tốt nhất”. 

Cung/nỏ cũng có nền khảo cổ để đưa vào hệ thống, nhưng những phục dựng cụ thể về “nỏ thần” hay cơ cấu đặc biệt Cổ Loa phải được xử lý như **archaeology-inspired game interpretation**, không phải chân lý lịch sử đã đóng. 

---

<a id="m13-t033-s004"></a>

## PART I — WEAPON SYSTEM

<a id="m13-t033-s005"></a>

## 3. WEAPON PHILOSOPHY

Không tồn tại:

> Weapon Damage = 12  
> Better Weapon Damage = 19  
> Legendary Weapon Damage = 37.

Một weapon được xác định chủ yếu bởi:

| Thuộc tính | Ý nghĩa |
|---|---|
| Reach | kiểm soát khoảng cách |
| Tempo | tốc độ đưa vũ khí vào action |
| Control | khả năng ép vị trí/giữ đối phương |
| Impact | năng lượng khi đòn trúng |
| Penetration | khả năng xuyên protection |
| Guard | khả năng tự bảo vệ |
| Handling | độ khó điều khiển |
| Recovery | thời gian hồi sau action mạnh |
| Hands | một tay/hai tay |
| Concealment | khả năng mang kín |
| Environment | nơi vũ khí phát huy tốt/xấu |
| Durability | độ bền kết cấu |
| Utility | giá trị ngoài combat |

---

<a id="m13-t033-s006"></a>

## 4. DAMAGE TYPE

Không cần hàng chục loại damage.

Core chỉ có bốn kiểu cơ học:

<a id="m13-t033-s007"></a>

#### ĐÂM — Thrust

Tập trung lực vào diện nhỏ.

Ưu:

penetration.

targeted attack.

reach.

Nhược:

khó tạo control diện rộng.

---

<a id="m13-t033-s008"></a>

#### CHẶT — Chop

Lực mạnh, quỹ đạo lớn.

Ưu:

Impact.

shield/structure pressure.

serious wound.

Nhược:

recovery chậm hơn.

---

<a id="m13-t033-s009"></a>

#### CẮT — Cut

Quỹ đạo nhanh hơn chop.

Ưu:

unprotected target.

bleeding.

control.

Nhược:

protection phù hợp giảm hiệu quả mạnh.

---

<a id="m13-t033-s010"></a>

#### ĐẬP — Blunt

Không cần xuyên bề mặt để gây trauma.

Ưu:

stun.

fracture.

armor interaction.

Nhược:

thường ít bleeding.

---

<a id="m13-t033-s011"></a>

## 5. WOUND KHÔNG ĐƯỢC SUY RA TRỰC TIẾP TỪ DAMAGE TYPE

Ví dụ:

một spear thrust có thể:

trượt;

gây cut nhẹ;

xuyên đùi;

đâm torso nghiêm trọng.

Outcome phụ thuộc:

Attack Margin.

Impact.

Armor.

Target condition.

Location.

Technique.

Không dùng:

> Spear always Piercing Damage 15.

---

<a id="m13-t033-s012"></a>

## 6. WEAPON SIZE

Ba nhóm chính:

<a id="m13-t033-s013"></a>

#### Close

Dao, công cụ ngắn.

Tốt khi:

grapple;

thuyền chật;

nhà nhỏ.

---

<a id="m13-t033-s014"></a>

#### Standard

Rìu, kiếm ngắn, vũ khí một tay.

Linh hoạt.

---

<a id="m13-t033-s015"></a>

#### Reach

Giáo, mâu, lao dài.

Kiểm soát engagement.

Bất lợi trong zone rất chật.

---

<a id="m13-t033-s016"></a>

## 7. REACH MECHANIC

Reach weapon có một lợi thế quan trọng:

**First Contact.**

Nếu đối thủ dùng vũ khí ngắn tiến vào engagement:

người có giáo có cơ hội:

Brace.

Threaten.

Intercept.

Đối thủ phải:

đánh bật mũi;

dùng khiên;

né;

ép sát.

Nếu đã bị áp sát:

reach advantage giảm hoặc biến mất.

Nhờ đó giáo không đơn giản là:

> “sword nhưng damage cao hơn.”

---

<a id="m13-t033-s017"></a>

## 8. WEAPON TEMPO

Weapon actions có:

Fast.

Standard.

Slow.

Ví dụ:

dao thrust:

Fast.

rìu chop mạnh:

Slow.

giáo short thrust:

Standard/Fast tùy stance.

Nỏ reload:

Slow.

Tempo tác động initiative/action interaction.

---

<a id="m13-t033-s018"></a>

## 9. RECOVERY

Một đòn mạnh tạo **Recovery State**.

Ví dụ character chém rìu toàn lực.

Nếu miss:

mất thế.

Defense giảm tạm thời.

Đây là downside tự nhiên của high-impact action.

Không cần cooldown 3 turns.

---

<a id="m13-t033-s019"></a>

## 10. WEAPON FAMILIES

Bộ GDD ban đầu gồm:

1. Giáo/Mâu.
2. Lao.
3. Rìu chiến.
4. Qua/vũ khí móc-chặt.
5. Dao găm.
6. Kiếm/ngắn kiếm hiếm hơn.
7. Cung.
8. Nỏ.
9. Vũ khí ném.
10. Gậy/chùy/công cụ đập.
11. Vũ khí săn thủy sinh.
12. Improvised tools.

---

<a id="m13-t033-s020"></a>

## 11. GIÁO / MÂU

<a id="m13-t033-s021"></a>

### Identity

**Giữ khoảng cách và kiểm soát đường tiến.**

<a id="m13-t033-s022"></a>

#### Strength

Reach cao.

Formation tốt.

Brace tốt.

Targeted thrust tốt.

Chi phí sản xuất tương đối hợp lý vì phần kim loại tập trung ở đầu vũ khí.

<a id="m13-t033-s023"></a>

#### Weakness

Không gian chật.

Grapple.

Enemy đã áp sát.

Cán có thể bị damage.

---

<a id="m13-t033-s024"></a>

## 12. SPEAR ACTIONS

<a id="m13-t033-s025"></a>

#### Thrust

Attack chuẩn.

<a id="m13-t033-s026"></a>

#### Short Thrust

Ít Impact hơn nhưng nhanh.

<a id="m13-t033-s027"></a>

#### Brace

Chuẩn bị Reaction chống enemy advance/charge.

<a id="m13-t033-s028"></a>

#### Hold Line

Threaten một phần frontage.

<a id="m13-t033-s029"></a>

#### Push Shaft

Dùng cán để tạo khoảng cách thay vì gây wound.

<a id="m13-t033-s030"></a>

#### Hook/Trip

Chỉ áp dụng với cấu tạo hoặc technique phù hợp.

---

<a id="m13-t033-s031"></a>

## 13. SPEAR FORMATION

Hai hoặc nhiều spear user phối hợp có thể:

chồng Threat Zone.

bảo vệ người đứng sau.

ép enemy approach theo hướng nhất định.

Nhưng formation cần:

Training.

Tactical Trust.

Space.

---

<a id="m13-t033-s032"></a>

## 14. LAO

Lao nằm giữa:

melee reach weapon

và

throwing weapon.

Đặc tính:

nhẹ hơn spear chiến dài.

Có thể ném.

Ít control hơn spear.

Carry nhiều cây có cost.

---

<a id="m13-t033-s033"></a>

## 15. THROWN JAVELIN

Không chỉ gây damage.

Có thể:

làm enemy dùng khiên.

phá movement.

ép đối phương rời cover.

đánh vào animal.

Một opening volley có thể tác động morale.

---

<a id="m13-t033-s034"></a>

## 16. RÌU CHIẾN

<a id="m13-t033-s035"></a>

### Identity

**High commitment — high impact.**

Rìu đặc biệt phù hợp game vì có giá trị:

combat;

wood work;

survival;

symbolism.

<a id="m13-t033-s036"></a>

#### Strength

Impact cao.

Shield pressure.

Structure/tool utility.

<a id="m13-t033-s037"></a>

#### Weakness

Reach trung bình/ngắn.

Heavy actions recovery cao.

Precision thấp hơn thrust weapon trong targeted attack.

---

<a id="m13-t033-s038"></a>

## 17. AXE ACTIONS

<a id="m13-t033-s039"></a>

#### Quick Chop

Giảm impact để tăng tempo.

<a id="m13-t033-s040"></a>

#### Heavy Chop

Impact lớn.

Recovery lớn.

<a id="m13-t033-s041"></a>

#### Hook Shield

Nếu cấu tạo cho phép.

<a id="m13-t033-s042"></a>

#### Break Guard

Tập trung vào defense/equipment.

<a id="m13-t033-s043"></a>

#### Tool Use

Phá cửa.

chặt gỗ.

---

<a id="m13-t033-s044"></a>

## 18. QUA / HOOKING WEAPON FAMILY

Game có thể dùng một weapon family lấy cảm hứng từ những dạng vũ khí chặt/móc trong material culture.

Identity gameplay:

**Control + Hook + Chop.**

Có thể:

kéo khiên.

hook limb/weapon.

đánh từ góc.

Nhưng accuracy/handling yêu cầu Skill cao.

Tên gọi và classification khảo cổ cụ thể cần được research lại ở production; ở GDD đây là functional family.

---

<a id="m13-t033-s045"></a>

## 19. DAO GĂM

<a id="m13-t033-s046"></a>

### Identity

**Vũ khí gần, nhanh, cá nhân.**

Strength:

Fast.

Concealable.

Grapple range.

Utility.

Có thể là social/status item.

Weakness:

Reach cực thấp.

Khó tiếp cận spear line.

Armor làm giảm hiệu quả.

Material culture Đông Sơn cho phép dao găm không chỉ là vật chiến đấu mà còn có lớp biểu trưng/status đáng khai thác trong item lore. 

---

<a id="m13-t033-s047"></a>

## 20. DAGGER ACTIONS

<a id="m13-t033-s048"></a>

#### Quick Stab

Fast.

<a id="m13-t033-s049"></a>

#### Grapple Strike

Yêu cầu target đang Controlled.

<a id="m13-t033-s050"></a>

#### Cut Binding

Utility.

<a id="m13-t033-s051"></a>

#### Concealed Draw

Technique/social context.

<a id="m13-t033-s052"></a>

#### Threaten

Có thể dùng trong close negotiation/intimidation.

---

<a id="m13-t033-s053"></a>

## 21. KIẾM / SHORT BLADE

Không được biến thành universal superior weapon.

Trong game:

kiếm tốt có thể:

linh hoạt.

có cut + thrust.

guard tốt.

Nhưng:

cost cao.

rare.

status-bearing.

không vượt giáo trong formation.

không vượt rìu trong raw chop.

không vượt dao khi grapple.

Một số tư liệu khảo cổ/diễn giải cũng gợi ý kiếm và dao găm có liên hệ với status/thủ lĩnh trong những bối cảnh cụ thể, rất phù hợp để chúng ít phổ biến hơn trong game. 

---

<a id="m13-t033-s054"></a>

## 22. CUNG

<a id="m13-t033-s055"></a>

### Identity

**Rate of fire + skill expression + range.**

<a id="m13-t033-s056"></a>

#### Strength

Có thể bắn nhiều hơn nỏ trong cùng thời gian.

Flexible range.

Hunter synergy.

<a id="m13-t033-s057"></a>

#### Weakness

Đòi hỏi training.

Khi engaged gần rất yếu.

Weather/string condition quan trọng.

Arrows hữu hạn.

---

<a id="m13-t033-s058"></a>

## 23. BOW ACTIONS

<a id="m13-t033-s059"></a>

#### Snap Shot

Fast.

Accuracy thấp hơn.

<a id="m13-t033-s060"></a>

#### Aimed Shot

Dùng Focus/time.

<a id="m13-t033-s061"></a>

#### Suppressive Shot

Không cần hit trực tiếp; ép movement/position.

<a id="m13-t033-s062"></a>

#### Hunting Shot

Specialized against unaware animal.

<a id="m13-t033-s063"></a>

#### Arc Shot

Contextual, không universal fantasy rain-of-arrows skill.

---

<a id="m13-t033-s064"></a>

## 24. NỎ

<a id="m13-t033-s065"></a>

### Identity

**Prepared projectile power.**

Không thiết kế nỏ như:

> bow damage +50%.

Nỏ khác cung vì combat rhythm.

<a id="m13-t033-s066"></a>

#### Strength

Có thể giữ trạng thái loaded.

Aim stability.

Penetration cao tùy design.

Training curve khác cung.

<a id="m13-t033-s067"></a>

#### Weakness

Reload chậm.

Mechanism maintenance.

Khó phản ứng sau khi bắn.

---

<a id="m13-t033-s068"></a>

## 25. CROSSBOW STATE

Nỏ có trạng thái:

**Unloaded**

↓

**Loading**

↓

**Loaded**

↓

**Aimed**

↓

**Discharged**

Loaded weapon là một tactical resource.

Một character có thể bước vào encounter với nỏ đã lên dây nếu context hợp lý.

---

<a id="m13-t033-s069"></a>

## 26. CỔ LOA TECHNOLOGY ARC

Nỏ phải có **technology progression trong lịch sử game**.

Early campaign:

nỏ tồn tại nhưng không phải superweapon.

Mid campaign:

craft knowledge phát triển.

Loa Vực:

standardization;

mũi tên;

cơ cấu;

production;

training;

supply

bắt đầu kết nối.

Late campaign:

Cổ Loa có thể tạo một **weapon system** có hiệu suất quân sự lớn.

“Nỏ thần” trong game không nhất thiết là một cây vũ khí duy nhất.

Có thể là kết quả của:

**design + production + training + organization + myth formation.**

Đây là Game Canon, không phải khẳng định khảo cổ.

---

<a id="m13-t033-s070"></a>

## 27. NỎ THẦN KHÔNG PHẢI LEGENDARY LOOT

Player không nên loot:

> Nỏ Thần +27.

Thay vào đó đây có thể là một Historical Technology Thread.

Player có thể tham gia với vai trò:

Xảo Nhân.

thợ đúc.

thợ làm khuôn.

thợ mộc.

binh sĩ thử nghiệm.

quản lý production.

Sứ Giả bảo vệ trade secret.

---

<a id="m13-t033-s071"></a>

## 28. GẬY / CLUB / STAFF

Không được coi là starter-trash weapon.

Advantages:

cheap.

blunt damage.

control.

non-lethal options.

walking utility.

Một trained staff user vẫn rất nguy hiểm.

---

<a id="m13-t033-s072"></a>

## 29. HARPOON / FISHING WEAPON

Rất quan trọng cho Tam Giang/Hải Môn.

Identity:

thủy chiến/săn.

có thể có tether.

Use:

fish.

large aquatic target.

pull object.

boat combat.

Không phải optimal infantry weapon.

---

<a id="m13-t033-s073"></a>

## 30. IMPROVISED WEAPONS

Nông cụ/công cụ có thể dùng trong crisis:

rìu lao động.

dao.

gậy.

mái chèo.

búa.

cuốc.

Nhưng người dùng Skill nghề không tự động có Combat Skill hoàn chỉnh.

---

<a id="m13-t033-s074"></a>

## 31. PADDLE AS WEAPON

Đặc biệt cho boat combat.

Mái chèo có thể:

push.

block nhẹ.

knockback.

điều thuyền.

Không mạnh về lethal impact nhưng utility rất cao.

---

<a id="m13-t033-s075"></a>

## 32. TWO-HANDED VS ONE-HANDED

Hai tay:

control/impact cao.

không dùng shield.

Một tay:

linh hoạt.

shield.

torch/tool.

Khác biệt là tactical trade-off, không damage scaling đơn giản.

---

<a id="m13-t033-s076"></a>

## 33. WEAPON GRIP SWITCHING

Một số weapon cho phép đổi:

Long Grip.

Short Grip.

Ví dụ spear:

Long Grip:

Reach tốt.

Short Grip:

dùng trong close zone tốt hơn nhưng control giảm.

Technique có thể làm switching nhanh.

---

<a id="m13-t033-s077"></a>

## 34. WEAPON QUALITY

Quality có 5 chiều riêng thay vì rarity color:

<a id="m13-t033-s078"></a>

#### Material

Kim loại/gỗ thực tế tốt đến đâu.

<a id="m13-t033-s079"></a>

#### Craftsmanship

Trình độ chế tác.

<a id="m13-t033-s080"></a>

#### Fit

Có hợp người sử dụng không.

<a id="m13-t033-s081"></a>

#### Condition

Tình trạng hiện tại.

<a id="m13-t033-s082"></a>

#### Provenance

Lịch sử/nguồn gốc.

---

<a id="m13-t033-s083"></a>

## 35. “MASTERWORK” KHÔNG CÓ NGHĨA +5 DAMAGE

Một spear masterwork có thể:

nhẹ hơn.

cân bằng hơn.

đầu chắc hơn.

recovery tốt hơn.

durability cao.

Không gây damage phi lý.

---

<a id="m13-t033-s084"></a>

## 36. ITEM FIT

Vũ khí có thể được làm riêng cho character.

Fit tác động:

Handling.

Fatigue.

Precision.

Không dễ chuyển cho người khác.

Điều này kết nối passive **Một Cung Một Chủ** của Nỏ Cần.

---

<a id="m13-t033-s085"></a>

## 37. WEAPON CONDITION

Các trạng thái:

Excellent.

Good.

Worn.

Damaged.

Critical.

Broken.

Condition giảm từ:

heavy use.

block.

water.

poor storage.

failed craft.

Không giảm nhanh sau mỗi hit kiểu survival game.

---

<a id="m13-t033-s086"></a>

## 38. MAINTENANCE

Sau expedition:

Sharpen.

Dry.

Repair binding.

Replace shaft.

Oil/protect metal nếu production setting sử dụng phù hợp.

Routine được abstract.

Chỉ expose gameplay khi:

resource thiếu;

item quý;

damage nghiêm trọng.

---

<a id="m13-t033-s087"></a>

## 39. WEAPON BREAK

Hiếm.

Có thể xảy ra do:

Critical condition.

Massive impact.

Bad material.

Intentional destruction.

Một weapon quan trọng gãy phải là event đáng nhớ.

---

<a id="m13-t033-s088"></a>

## PART II — SHIELD & ARMOR SYSTEM

<a id="m13-t033-s089"></a>

## 40. PHILOSOPHY

Protection không làm character “không bị thương”.

Protection:

> **thay đổi xác suất và mức độ hậu quả.**

Một người có protection tốt vẫn có thể:

ngã.

choáng.

gãy xương.

mất stamina.

---

<a id="m13-t033-s090"></a>

## 41. PROTECTION LAYERS

Một character có thể có:

<a id="m13-t033-s091"></a>

#### Clothing

Base.

<a id="m13-t033-s092"></a>

#### Soft Protection

Nhiều lớp vật liệu mềm/thick.

<a id="m13-t033-s093"></a>

#### Rigid Local Protection

Chest/arm guards tùy culture/game canon.

<a id="m13-t033-s094"></a>

#### Shield

Active defense riêng.

Không nên thiết kế full plate fantasy.

---

<a id="m13-t033-s095"></a>

## 42. HISTORICAL CAUTION

Hiện vật/sưu tập có bằng chứng về các dạng che ngực bằng kim loại trong văn hóa Đông Sơn, nhưng GDD không nên suy rộng thành những bộ giáp kín kiểu thời trung đại. 

Các dạng bảo vệ bằng vật liệu hữu cơ trong game cần được ghi rõ là:

> **plausible/game-canon reconstruction**

nếu thiếu bằng chứng chắc chắn.

---

<a id="m13-t033-s096"></a>

## 43. ARMOR PROFILE

Mỗi protection có:

| Thuộc tính | Ý nghĩa |
|---|---|
| Coverage | bảo vệ vùng nào |
| Deflection | làm lệch hit |
| Absorption | giảm impact |
| Penetration Resistance | chống xuyên |
| Mobility | ảnh hưởng movement |
| Noise | ảnh hưởng stealth |
| Heat | fatigue |
| Water Burden | nguy hiểm khi ngập/bơi |
| Durability | khả năng chịu nhiều impact |

---

<a id="m13-t033-s097"></a>

## 44. COVERAGE QUAN TRỌNG

Không dùng:

> Armor Rating 15 toàn thân.

Ví dụ chest protector:

Torso Protection cao.

Arm/Leg không được bảo vệ.

Một targeted thrust vào leg vẫn nguy hiểm.

---

<a id="m13-t033-s098"></a>

## 45. ARMOR INTERACTION

Attack Resolution:

Hit Location

↓

Coverage?

↓

Deflection?

↓

Penetration?

↓

Impact remains?

↓

Vitality/Wound result.

Armor có thể biến:

Critical torso penetration

thành:

Heavy Impact + Bruising.

Đó là giá trị lớn.

---

<a id="m13-t033-s099"></a>

## 46. HEAVY PROTECTION TRADE-OFF

Protection tốt hơn thường:

Mobility thấp hơn.

Fatigue tăng.

Swimming nguy hiểm hơn.

Noise cao.

Trong Tam Giang:

armor nặng có thể trở thành liability.

---

<a id="m13-t033-s100"></a>

## 47. NO UNIVERSAL BEST ARMOR

Rừng nóng ẩm:

light protection có lợi.

Battle line:

chest protection + shield tốt.

Boat combat:

mobility/buoyancy quan trọng.

Stealth:

noise thấp.

---

<a id="m13-t033-s101"></a>

## 48. SHIELD AS ACTIVE EQUIPMENT

Khiên có:

Size.

Coverage.

Handling.

Durability.

Material.

Không chỉ +Defense.

---

<a id="m13-t033-s102"></a>

## 49. SHIELD ACTIONS

<a id="m13-t033-s103"></a>

#### Block

Reaction.

<a id="m13-t033-s104"></a>

#### Cover

Chống ranged.

<a id="m13-t033-s105"></a>

#### Protect Adjacent

Hộ Vệ.

<a id="m13-t033-s106"></a>

#### Shield Push

Control.

<a id="m13-t033-s107"></a>

#### Lock Formation

Formation action.

<a id="m13-t033-s108"></a>

#### Brace

Defensive stance.

---

<a id="m13-t033-s109"></a>

## 50. SHIELD DAMAGE

Khiên hấp thụ impact.

Có thể:

crack.

binding loose.

break.

Nhưng một hit không làm khiên biến mất kiểu game arcade.

---

<a id="m13-t033-s110"></a>

## 51. SHIELD SIZE TRADE-OFF

Large:

Coverage cao.

formation tốt.

heavy.

boat/forest awkward.

Small:

mobility.

duel.

less missile coverage.

---

<a id="m13-t033-s111"></a>

## PART III — EQUIPMENT & INVENTORY

<a id="m13-t033-s112"></a>

## 52. EQUIPMENT PHILOSOPHY

Character không phải container 200 slot.

Mỗi người có:

<a id="m13-t033-s113"></a>

#### Worn

đang mặc/mang.

<a id="m13-t033-s114"></a>

#### Ready

có thể lấy nhanh.

<a id="m13-t033-s115"></a>

#### Packed

trong túi/gùi.

<a id="m13-t033-s116"></a>

#### Stored

ở nhà/thuyền/kho.

---

<a id="m13-t033-s117"></a>

## 53. READY EQUIPMENT

Combat chỉ sử dụng dễ dàng đồ ở:

hands;

belt;

quiver;

ready pack.

Một chiếc rìu nằm dưới đáy gùi không thể:

Minor Action → Equip.

---

<a id="m13-t033-s118"></a>

## 54. ENCUMBRANCE

Không dùng:

47.2 / 60 kg

nếu không cần.

Có 5 trạng thái:

Light.

Normal.

Loaded.

Heavy.

Overburdened.

Phụ thuộc:

THỂ.

pack quality.

item bulk.

Không chỉ weight.

---

<a id="m13-t033-s119"></a>

## 55. BULK

Một spear nhẹ nhưng dài.

Do đó có:

Weight

và

Bulk.

Mang 5 cây giáo gây vấn đề Bulk dù tổng cân nặng không cực lớn.

---

<a id="m13-t033-s120"></a>

## 56. PARTY CARGO

Thuyền/pack animal nếu sau này có/porter/storage cho phép transport nhiều.

Equipment logistics trở thành:

party/world system

thay vì mỗi nhân vật có túi vô hạn.

---

<a id="m13-t033-s121"></a>

## 57. ITEM OWNERSHIP

Mỗi item quan trọng có:

Owner.

Maker.

Current Holder.

Previous Owners.

Origin.

Creation Date hoặc approximate era.

---

<a id="m13-t033-s122"></a>

## 58. ITEM HISTORY

Ví dụ:

**Giáo của Tòng**

Made by:

Cấn family.

Owner 1:

Tòng's father.

Owner 2:

Tòng.

Repaired:

Year 6.

Used in:

Flood Rescue.

Passed to:

Mộc.

Một item trở thành part of Chronicle.

---

<a id="m13-t033-s123"></a>

## 59. NAMED ITEM

Item không trở thành Named vì rarity.

Nó có thể được đặt tên khi:

có lịch sử;

được community biết;

chủ đặt;

gắn với major event.

---

<a id="m13-t033-s124"></a>

## 60. ITEM LEGEND FORMATION

Một weapon bình thường có thể thành legendary nếu lịch sử đủ lớn.

Không cần special magical stats.

Ví dụ:

> cây giáo dùng để bảo vệ Bến Vạc trong trận lũ.

Sau 40 năm:

reputation/social effect.

Sau nhiều thế hệ:

myth.

---

<a id="m13-t033-s125"></a>

## 61. HEIRLOOM

Item gia truyền mang:

emotional value.

family claim.

inheritance conflict.

Player bán heirloom có thể tạo family reaction.

---

<a id="m13-t033-s126"></a>

## 62. LOOT

Không phải mọi enemy rơi:

coins.

weapon.

armor.

Một dead farmer có:

đồ cá nhân.

Nếu player lấy:

World hiểu đó là lấy tài sản người chết.

Family có thể nhận ra.

---

<a id="m13-t033-s127"></a>

## 63. BATTLEFIELD LOOT

Sau trận:

ownership phức tạp.

Weapon enemy thuộc:

dead person;

family;

army;

victor;

community

tùy custom.

Looting không phải hành động social-neutral.

---

<a id="m13-t033-s128"></a>

## 64. LOOT QUALITY

Đồ lấy từ enemy có thể:

damaged.

poor fit.

recognizable.

stolen-status.

Không tự động là upgrade.

---

<a id="m13-t033-s129"></a>

## 65. STOLEN ITEM

Knowledge Graph lưu:

Legal/Social Owner.

Nếu player dùng đồ nổi tiếng của người bị giết:

NPC có thể nhận ra.

---

<a id="m13-t033-s130"></a>

## 66. EQUIPMENT SLOT

Không cần MMO slot dày đặc.

Core:

Head.

Torso.

Arms optional.

Legs optional.

Main Hand.

Off Hand.

Back/Long Weapon.

Belt.

Pack.

Accessory/Status Objects.

---

<a id="m13-t033-s131"></a>

## 67. ACCESSORY KHÔNG PHẢI STAT JEWELRY

Vòng, vật nghi lễ, đồ gia truyền có thể có:

social meaning.

religious meaning.

identity.

Không cần:

+3 Intelligence.

---

<a id="m13-t033-s132"></a>

## 68. SOCIAL EQUIPMENT

Trang phục/vật phẩm tác động dialogue.

Ví dụ:

trang sức family.

ritual object.

military insignia.

leader dagger.

Một item có thể mở hoặc đóng social option.

---

<a id="m13-t033-s133"></a>

## 69. DISGUISE & IDENTITY

Trang phục giúp nhưng không tạo magic disguise.

NPC có thể nhận:

face.

speech.

behaviour.

equipment.

Reputation.

---

<a id="m13-t033-s134"></a>

## 70. TOOL SYSTEM

Tools không bị gộp vào weapons.

Ví dụ:

healer kit.

smith tools.

rope.

fishing kit.

fire-making tools.

repair kit.

ritual objects.

Tools mở action.

---

<a id="m13-t033-s135"></a>

## 71. TOOL QUALITY

Tool tốt:

giảm failure.

tăng precision.

cho phép technique.

Không auto success nếu Skill thấp.

---

<a id="m13-t033-s136"></a>

## 72. CONSUMABLE PHILOSOPHY

Không spam:

Potion of Healing x30.

Consumables gồm:

bandages.

herbal preparations.

food.

poison.

oil/resin.

ammunition.

ritual material.

---

<a id="m13-t033-s137"></a>

## 73. HEALING CONSUMABLE

Không có instant:

+50 HP.

Thuốc có thể:

giảm pain.

hỗ trợ clotting.

giảm infection risk.

tăng recovery.

stabilize.

Cần thời gian.

---

<a id="m13-t033-s138"></a>

## PART IV — INJURY SYSTEM

<a id="m13-t033-s139"></a>

## 74. INJURY PHILOSOPHY

Injury phải đủ quan trọng để player sợ combat nhưng không khiến mỗi trận tạo một tháng downtime.

Do đó dùng hai lớp:

<a id="m13-t033-s140"></a>

#### Combat Attrition

Vitality.

<a id="m13-t033-s141"></a>

#### Persistent Harm

Wounds.

---

<a id="m13-t033-s142"></a>

## 75. VITALITY

Vitality bao gồm:

breath.

minor bruises.

fatigue.

near misses.

combat composure.

Hồi qua:

short rest.

food/water.

sleep.

Không phải toàn bộ là “máu”.

---

<a id="m13-t033-s143"></a>

## 76. WOUND THRESHOLD

Wound xuất hiện khi một hit có:

Impact cao

hoặc

Penetration

hoặc

target đang Vulnerable.

Không phải cứ mất đủ X Vitality mới bị wound.

---

<a id="m13-t033-s144"></a>

## 77. WOUND SEVERITY

Bốn cấp:

<a id="m13-t033-s145"></a>

### Minor

Có ảnh hưởng nhưng dễ xử lý.

Ví dụ:

cut.

bruise.

sprain.

---

<a id="m13-t033-s146"></a>

### Moderate

Ảnh hưởng gameplay vài ngày.

Ví dụ:

deep cut.

minor fracture.

muscle injury.

---

<a id="m13-t033-s147"></a>

### Serious

Có thể kết thúc expedition.

Ví dụ:

severe bleeding.

major fracture.

deep penetration.

---

<a id="m13-t033-s148"></a>

### Critical

Life-threatening.

Cần intervention nhanh.

---

<a id="m13-t033-s149"></a>

## 78. WOUND LOCATION

Core body regions:

Head.

Torso.

Left Arm.

Right Arm.

Left Leg.

Right Leg.

Không cần model từng ngón tay.

---

<a id="m13-t033-s150"></a>

## 79. HEAD INJURY

Possible:

Cut.

Concussion.

Eye injury.

Severe trauma.

Effects:

Focus.

GIÁC performance.

Balance.

Unconsciousness risk.

---

<a id="m13-t033-s151"></a>

## 80. TORSO INJURY

Possible:

Rib injury.

Deep wound.

Internal trauma.

Effects:

Stamina.

breathing.

bleeding.

life risk.

---

<a id="m13-t033-s152"></a>

## 81. ARM INJURY

Effects:

weapon handling.

shield.

craft.

Profession.

Điều này có thể ảnh hưởng nghề sau battle.

---

<a id="m13-t033-s153"></a>

## 82. LEG INJURY

Effects:

movement.

balance.

travel.

swimming.

retreat.

---

<a id="m13-t033-s154"></a>

## 83. WOUND TRAITS

Ngoài severity/location, wound có Tags.

Ví dụ:

Bleeding.

Contaminated.

Fracture.

Burn.

Crush.

Puncture.

Venom.

Mạch-affected nếu supernatural.

---

<a id="m13-t033-s155"></a>

## 84. BLEEDING

Ba cấp:

Light.

Heavy.

Critical.

Bleeding gây deterioration theo thời gian.

Không nhất thiết mỗi turn trừ HP.

Thay vào đó:

Stability giảm.

---

<a id="m13-t033-s156"></a>

## 85. STABILITY

Khi character bị Serious/Critical Wound có:

Stability.

Actions mạnh.

movement.

continued fighting

có thể làm Stability xấu đi.

Dược Nhân có thể stabilize.

---

<a id="m13-t033-s157"></a>

## 86. COMBAT STABILIZATION

Dược Nhân không “heal wound”.

Họ:

stop bleeding.

immobilize fracture.

maintain airway.

reduce shock.

Wound vẫn tồn tại sau combat.

---

<a id="m13-t033-s158"></a>

## 87. UNTREATED WOUND

Sau combat có risk:

infection.

delayed bleeding.

worsening.

poor healing.

Đây được xử lý bằng recovery simulation, không dice mỗi 5 phút.

---

<a id="m13-t033-s159"></a>

## 88. INFECTION

Không cần xuất hiện mọi lúc.

Risk tăng bởi:

dirty wound.

swamp.

poor care.

malnutrition.

delayed treatment.

Healer skill giảm.

---

<a id="m13-t033-s160"></a>

## 89. FRACTURE

Fracture cần:

immobilization.

time.

Có thể heal incorrectly nếu treatment kém.

Long-term effect.

---

<a id="m13-t033-s161"></a>

## 90. SCAR

Sau wound nhất định:

Scar generated.

Scar record gồm:

location.

event source.

visibility.

memory.

Có thể được NPC nhắc.

---

<a id="m13-t033-s162"></a>

## 91. PERMANENT IMPAIRMENT

Hiếm.

Chỉ Serious/Critical wounds + poor outcome.

Possible:

reduced mobility.

loss of eye.

chronic pain.

hand limitation.

hearing loss.

Game tuyệt đối không coi character “hỏng”.

Thay vào đó hệ:

Adaptation.

Career Transition.

Technique modification.

---

<a id="m13-t033-s163"></a>

## 92. ADAPTATION

Ví dụ archer bị thương tay kéo cung.

Possible future:

rehabilitation.

learn crossbow.

change shooting technique.

become tracker.

mentor.

Class không biến mất.

Cuộc đời chuyển hướng.

---

<a id="m13-t033-s164"></a>

## 93. PROSTHETIC / ASSISTIVE DEVICES

Nếu phù hợp game canon/material culture:

walking support.

splint.

crafted brace.

tool adaptation.

Không cần fantasy mechanical arm.

Xảo Nhân + Dược Nhân có thể phối hợp.

---

<a id="m13-t033-s165"></a>

## PART V — STATUS SYSTEM

<a id="m13-t033-s166"></a>

## 94. STATUS PHILOSOPHY

Không dùng 50 icon:

Burn 2.

Poison 3.

Weakness 4.

Curse 7.

Status chia thành vài nhóm có nghĩa rõ.

---

<a id="m13-t033-s167"></a>

## 95. PHYSICAL CONDITIONS

Fatigued.

Exhausted.

Bleeding.

Stunned.

Off-Balance.

Restrained.

Prone.

Winded.

Pain.

---

<a id="m13-t033-s168"></a>

## 96. MENTAL CONDITIONS

Afraid.

Shaken.

Panicked.

Focused.

Confident.

Confused.

Grieving không nên là combat debuff đơn giản; chủ yếu thuộc Psychology.

---

<a id="m13-t033-s169"></a>

## 97. ENVIRONMENTAL CONDITIONS

Wet.

Muddy.

Cold.

Overheated.

Smoke-affected.

Poisoned.

Venom-affected.

---

<a id="m13-t033-s170"></a>

## 98. TACTICAL STATES

Engaged.

Guarded.

Exposed.

Hidden.

Flanked.

Braced.

Aimed.

Loaded.

Controlled.

---

<a id="m13-t033-s171"></a>

## 99. SPIRITUAL STATES

Không phải tất cả character đều nhận.

Ví dụ:

Mạch Disturbed.

Spirit-Mark.

Ritually Protected.

Haunted/Disturbed.

Các status này cần narrative context.

Không phải generic magic debuff.

---

<a id="m13-t033-s172"></a>

## 100. FATIGUE

Fatigue là state dài hơn Stamina.

Stamina:

phút.

Fatigue:

giờ/ngày.

Nguồn:

travel.

hunger.

lack sleep.

heavy labor.

repeated combat.

Fatigue làm giảm:

Stamina pool/recovery.

Focus.

Resolve.

---

<a id="m13-t033-s173"></a>

## 101. EXHAUSTION

Fatigue nghiêm trọng.

Character có thể:

không sprint.

reaction giảm.

psychological stress tăng.

Combat khi Exhausted là quyết định nguy hiểm.

---

<a id="m13-t033-s174"></a>

## 102. STUN

Ngắn hạn.

Không nên khiến mất nhiều turn liên tục.

Có thể:

Reaction unavailable.

Major Action restricted.

---

<a id="m13-t033-s175"></a>

## 103. OFF-BALANCE

Rất quan trọng cho tactical positioning.

Effects:

defense thấp.

push dễ hơn.

Formation break.

Thường chỉ kéo ngắn.

---

<a id="m13-t033-s176"></a>

## 104. PRONE

Trong Zone system:

prone không yêu cầu exact tile.

Effects:

melee vulnerability tùy context.

ranged profile thấp.

movement action để đứng.

---

<a id="m13-t033-s177"></a>

## 105. RESTRAINED

Bị:

rope.

grapple.

net.

terrain

hạn chế action.

---

<a id="m13-t033-s178"></a>

## 106. POISON SYSTEM

Poison không chỉ:

Damage over Time.

Mỗi poison profile có:

Onset.

Symptoms.

Severity.

Duration.

Route.

Antidote/Treatment.

Ví dụ:

Weakness.

confusion.

vomiting.

paralysis.

bleeding.

---

<a id="m13-t033-s179"></a>

## 107. DOSE MATTERS

Toxicology dùng:

Substance

+

Dose

+

Body Condition.

Một thứ có thể:

medicine ở dose nhỏ.

poison ở dose cao.

Điều này hỗ trợ Lau Miên/Lau Kha.

---

<a id="m13-t033-s180"></a>

## 108. PLAYER KNOWLEDGE OF STATUS

Nếu không có medical knowledge:

UI có thể nói:

> “Mân run, chóng mặt và khó thở.”

Dược Nhân:

> “Dấu hiệu phù hợp với một loại độc thần kinh.”

Không tự động reveal internal status tag.

---

<a id="m13-t033-s181"></a>

## 109. FEAR STATUS

Source-specific.

Ví dụ:

Fear: Deep Water.

Fear: Spirit.

Fear: Fire.

Character có Fear Trigger từ Psychology.

Không dùng:

Fear Resistance +20 universal

quá nhiều.

---

<a id="m13-t033-s182"></a>

## 110. FEAR RESPONSE

Khi triggered:

Resolve check.

Outcome:

Controlled.

Shaken.

Freeze.

Flight.

Panic.

Psychology quyết style.

---

<a id="m13-t033-s183"></a>

## 111. MORALE KHÁC FEAR

Fear:

cá nhân trước nguy hiểm.

Morale:

ý chí tiếp tục hoàn thành objective.

Một warrior có thể:

sợ

nhưng morale cao.

---

<a id="m13-t033-s184"></a>

## PART VI — DEATH & RECOVERY INTERFACE

<a id="m13-t033-s185"></a>

## 112. DOWNED STATE

Vitality collapse có thể tạo:

Winded.

Stunned.

Unconscious.

Incapacitated.

Không auto Death.

---

<a id="m13-t033-s186"></a>

## 113. CRITICAL WOUND + DOWNED

Đây mới là vùng rất nguy hiểm.

Character có thể cần:

Stabilization.

Evacuation.

---

<a id="m13-t033-s187"></a>

## 114. COUP DE GRÂCE

Đánh người bất tỉnh có thể lethal.

Nhưng đây là conscious moral action.

Enemy personality quyết họ có làm không.

Không phải AI mặc định.

---

<a id="m13-t033-s188"></a>

## 115. RESCUE WINDOW

Không dùng arbitrary:

3 death saving throws

nếu có thể tránh.

Window phụ thuộc wound.

Critical bleeding:

ngắn.

Concussion:

dài hơn.

---

<a id="m13-t033-s189"></a>

## 116. DEATH CONDITIONS

Death có thể đến từ:

Catastrophic Injury.

Critical Bleeding.

Untreated Critical Wound.

Drowning.

Poison.

Environmental catastrophe.

Execution.

Không chủ yếu từ:

HP reaches -10.

---

<a id="m13-t033-s190"></a>

## 117. DROWNING

Đặc biệt quan trọng.

Stages:

In Water.

Struggling.

Submerged.

Unconscious.

Drowning.

Armor/load ảnh hưởng.

Water Skill cực quan trọng.

---

<a id="m13-t033-s191"></a>

## 118. FALL

Fall severity phụ thuộc:

height.

surface.

condition.

Không generic 1d6 per meter trong GDD.

Có thể tạo fracture/critical wound.

---

<a id="m13-t033-s192"></a>

## 119. FIRE/BURN

Burn severity dựa:

exposure time.

coverage.

material.

Không chỉ DoT.

Fire còn gây:

panic.

smoke.

property damage.

---

<a id="m13-t033-s193"></a>

## 120. RECOVERY PHASES

Persistent wound trải qua:

<a id="m13-t033-s194"></a>

#### Stabilization

Ngăn xấu thêm.

<a id="m13-t033-s195"></a>

#### Acute Care

Điều trị ban đầu.

<a id="m13-t033-s196"></a>

#### Rest

Ngày/tuần.

<a id="m13-t033-s197"></a>

#### Rehabilitation

Khôi phục function.

<a id="m13-t033-s198"></a>

#### Adaptation

Nếu không thể hồi hoàn toàn.

---

<a id="m13-t033-s199"></a>

## 121. RECOVERY TIME

Minor:

hours–days.

Moderate:

days–weeks.

Serious:

weeks–months.

Critical:

months hoặc permanent consequence.

Exact numbers balance sau.

---

<a id="m13-t033-s200"></a>

## 122. PLAYER KHÔNG BẮT BUỘC NẰM GIƯỜNG REAL-TIME

Downtime system cho phép:

time skip.

career activity nhẹ.

social interaction.

planning.

Nếu character bị injury 30 ngày:

game không bắt player click sleep 30 lần.

---

<a id="m13-t033-s201"></a>

## 123. TIME SKIP CONSEQUENCE

Nhưng 30 ngày trôi qua thật.

World simulation chạy.

War.

family.

debt.

harvest.

companions.

Do đó injury tạo strategic cost.

---

<a id="m13-t033-s202"></a>

## 124. PLAYING WHILE INJURED

Player có thể tiếp tục.

Nhưng:

risk worsening.

performance penalty.

story consequence.

Game không hard-lock trừ incapacitation nặng.

---

<a id="m13-t033-s203"></a>

## 125. COMPANION CARE

Party có thể quyết định:

để người bị thương lại settlement.

đi tiếp.

Ai chăm?

Companion relationship affected.

---

<a id="m13-t033-s204"></a>

## PART VII — ITEM & INJURY NARRATIVE INTEGRATION

<a id="m13-t033-s205"></a>

## 126. WEAPON KILL HISTORY

Một named weapon có thể lưu major kills/events.

Không nhất thiết mọi kill.

Ví dụ:

Used to kill:

Chief X.

Spirit creature Y.

Used to:

cut rope saving ferry.

---

<a id="m13-t033-s206"></a>

## 127. ITEM RECOGNITION

NPC chuyên môn có thể nhận:

maker style.

material origin.

repair marks.

Owner insignia.

Đây là Investigation content.

---

<a id="m13-t033-s207"></a>

## 128. FORENSIC EQUIPMENT

Wound + weapon interaction có thể tạo clue.

Ví dụ:

Wound shape.

penetration.

metal fragment.

Skill:

Medicine + Weapon Knowledge.

Không phải CSI hiện đại; chỉ inference phù hợp capability.

---

<a id="m13-t033-s208"></a>

## 129. WEAPON AS EVIDENCE

Một weapon bị bỏ lại:

owner?

maker?

region?

damage?

Blood?

Repair?

Story Thread có thể phát sinh.

---

<a id="m13-t033-s209"></a>

## 130. ARMOR AS STORY

Chest protection bị thủng có thể:

trở thành family relic.

proof of battle.

crafting case.

Không chỉ vendor trash.

---

<a id="m13-t033-s210"></a>

## 131. SCAR AS MEMORY

NPC có thể hỏi:

> “Vết này từ đâu?”

Dialogue answer phụ thuộc actual Chronicle.

Không generic background.

---

<a id="m13-t033-s211"></a>

## 132. INJURY & PSYCHOLOGY

Injury có thể tạo:

Fear.

Shame.

Pride.

Guilt.

Dependency discomfort.

Career identity crisis.

Không phải tự động.

Interpretation Engine xử lý.

---

<a id="m13-t033-s212"></a>

## 133. INJURY & RELATIONSHIP

Ai chăm character?

Ai bỏ đi?

Ai cứu?

Một recovery period có thể làm relationship sâu hơn combat.

---

<a id="m13-t033-s213"></a>

## 134. EQUIPMENT & FAMILY

Heirloom weapon có family claim.

Nếu character chết:

Inheritance System xử lý.

Companion không auto nhận.

---

<a id="m13-t033-s214"></a>

## 135. EQUIPMENT & PROFESSION

Smith nhận biết metal.

Hunter nhận biết bow fit.

Boatman đánh giá spear/paddle utility.

Different professions nhìn cùng item khác nhau.

---

<a id="m13-t033-s215"></a>

## PART VIII — RARITY & ECONOMY

<a id="m13-t033-s216"></a>

## 136. KHÔNG DÙNG COLOR RARITY LÀ TRỤ CỘT

Không nên:

Common white.

Rare blue.

Epic purple.

Legendary orange.

Thay vào đó item description nói:

Material.

Maker.

Condition.

History.

Reputation.

---

<a id="m13-t033-s217"></a>

## 137. ITEM VALUE CÓ NHIỀU DẠNG

<a id="m13-t033-s218"></a>

#### Functional Value

dùng tốt không?

<a id="m13-t033-s219"></a>

#### Material Value

đồng/kim loại quý?

<a id="m13-t033-s220"></a>

#### Craft Value

tay nghề?

<a id="m13-t033-s221"></a>

#### Social Value

ai dùng?

<a id="m13-t033-s222"></a>

#### Ritual Value

ý nghĩa?

<a id="m13-t033-s223"></a>

#### Historical Value

đã liên quan event gì?

---

<a id="m13-t033-s224"></a>

## 138. MỘT ITEM CÓ THỂ FUNCTIONALLY TỆ NHƯNG CULTURALLY VÔ GIÁ

Ví dụ:

dao găm cổ của dòng họ.

Combat:

không nổi bật.

Family Value:

cực cao.

Player bán nó sẽ gây hậu quả.

---

<a id="m13-t033-s225"></a>

## 139. REGIONAL AVAILABILITY

Không phải shop nào bán mọi weapon.

Đồng Đỏ:

metalwork tốt.

Tam Giang:

water gear.

Tản Viên:

hunting gear.

Phong Châu:

high-status craft.

Trade routes thay đổi availability.

---

<a id="m13-t033-s226"></a>

## 140. PRICE KHÔNG CHỈ DỰA ITEM TIER

War:

spear price tăng.

Flood:

boat tools tăng.

Metal shortage:

bronze weapon tăng.

Economy simulation tác động equipment.

---

<a id="m13-t033-s227"></a>

## 141. WEAPON STANDARDIZATION

Early game:

items có variation lớn.

Late Cổ Loa:

một số military item có thể được standardized.

Đây là visual/gameplay dấu hiệu state-building.

---

<a id="m13-t033-s228"></a>

## PART IX — CRAFTING INTEGRATION

<a id="m13-t033-s229"></a>

## 142. WEAPON RECIPE KHÔNG PHẢI “2 BRONZE + 1 WOOD”

Crafting sau này cần:

Design.

Material.

Components.

Process.

Skill.

Tools.

Workshop.

Knowledge.

---

<a id="m13-t033-s230"></a>

## 143. WEAPON COMPONENTS

Ví dụ spear:

Head.

Shaft.

Binding/Socket connection.

Balance.

Một component có thể thay.

Gãy cán không mất đầu giáo quý.

---

<a id="m13-t033-s231"></a>

## 144. REPAIR HISTORY

Item lưu significant repair.

Một spear có:

original head.

third shaft.

new binding.

Game có thể đặt câu hỏi triết lý:

> đó vẫn là cây giáo cũ không?

Không cần mechanic riêng nhưng rất hợp lore.

---

<a id="m13-t033-s232"></a>

## 145. SALVAGE

Broken item có thể salvage:

metal.

component.

ornament.

Không biến mất.

Gò Búa cực phù hợp hệ này.

---

<a id="m13-t033-s233"></a>

## PART X — BALANCING PRINCIPLES

<a id="m13-t033-s234"></a>

## 146. WEAPON BALANCE

Không cân bằng mọi weapon bằng DPS giống nhau.

Cân bằng dựa trên:

Opportunity.

Context.

Risk.

Cost.

Training.

Ví dụ spear tốt hơn dao ở field.

Dao tốt hơn spear trong grapple.

Đó không phải imbalance.

---

<a id="m13-t033-s235"></a>

## 147. ARMOR BALANCE

Protection mạnh phải có context cost.

Không cần:

Light Armor → Medium Armor → Heavy Armor tuyến tính.

---

<a id="m13-t033-s236"></a>

## 148. RANGED BALANCE

Cung/nỏ rất nguy hiểm nếu có preparation.

Counterplay:

cover.

shield.

terrain.

ambush.

closing distance.

weather.

Không giảm damage giả tạo chỉ để melee tồn tại.

---

<a id="m13-t033-s237"></a>

## 149. LETHALITY

Combat với người có vũ khí thật phải nguy hiểm.

Nhưng hệ Vitality + Wound đảm bảo:

nhiều hit không phải luôn fatal.

một hit tốt có thể quyết định trận.

Player có cơ hội:

retreat.

stabilize.

prepare.

---

<a id="m13-t033-s238"></a>

## 150. NO HP SPONGE

Một elite enemy khó vì:

Skill.

Defense.

position.

equipment.

discipline.

companions.

Không vì:

12,000 HP.

---

<a id="m13-t033-s239"></a>

## 151. BOSS EQUIPMENT

Boss human có thể sở hữu item tốt.

Nếu player giết họ:

item tồn tại thật.

Không có boss-only weapon magically weaker khi loot.

Nếu quá mạnh:

nó cũng phải mạnh khi player sử dụng, với các requirement thật.

---

<a id="m13-t033-s240"></a>

## PART XI — EXAMPLE COMBAT INTERACTIONS

<a id="m13-t033-s241"></a>

## 152. EXAMPLE 1 — SPEAR VS DAGGER

Open field.

Spear user:

Reach advantage.

Dagger user phải:

shield approach;

dodge intercept;

use terrain;

throw object;

hoặc retreat.

Nếu dagger user vào được grapple:

spear trở nên awkward.

Combat thay đổi hoàn toàn.

---

<a id="m13-t033-s242"></a>

## 153. EXAMPLE 2 — AXE VS SHIELD

Axe Heavy Chop.

Shield blocks.

Possible:

shield absorbs.

user loses Stamina.

shield condition worsens.

Axe user enters Recovery.

Hộ Vệ có thể counter-position.

Không chỉ:

Damage 18 - Armor 5 = 13.

---

<a id="m13-t033-s243"></a>

## 154. EXAMPLE 3 — ARROW VS CHEST PROTECTION

Arrow hits torso.

Protection covers torso.

Penetration check fails.

Character receives:

Impact.

Vitality loss.

Possible Bruised condition.

Không Wound xuyên.

Armor thực sự cứu mạng.

---

<a id="m13-t033-s244"></a>

## 155. EXAMPLE 4 — ARROW HITS LEG

Leg uncovered.

Good hit.

Moderate puncture.

Bleeding.

Movement penalty.

Enemy không chết.

Nhưng họ có thể:

surrender;

crawl;

need treatment.

Combat consequence dài hạn.

---

<a id="m13-t033-s245"></a>

## 156. EXAMPLE 5 — RIVER FIGHT

Player mặc protection nặng.

Bị đẩy khỏi thuyền.

Protection hữu ích trước weapon.

Nhưng:

Swimming check khó.

Companion phải rescue.

Một equipment choice vừa là buff vừa là risk.

---

<a id="m13-t033-s246"></a>

## PART XII — STATUS READABILITY

<a id="m13-t033-s247"></a>

## 157. UI KHÔNG NÊN HIỂN THỊ 12 ICON KHÓ HIỂU

Một character panel có thể nói:

**Mân**

Serious Leg Wound

Heavy Bleeding

Fatigued

Afraid: Deep Water

Đủ rõ.

---

<a id="m13-t033-s248"></a>

## 158. STATUS PRIORITY

UI ưu tiên:

Life-threatening.

Action-limiting.

Tactical.

Minor.

Không để `Wet` chiếm vị trí ngang `Critical Bleeding`.

---

<a id="m13-t033-s249"></a>

## 159. NATURAL LANGUAGE TOOLTIP

Ví dụ:

**Serious Leg Wound**

> Mân khó đứng vững và không thể chạy. Tiếp tục vận động mạnh có thể làm vết thương nặng hơn.

Tốt hơn:

`Movement -40%, Stability -15`.

Advanced UI vẫn có thể cho numeric detail.

---

<a id="m13-t033-s250"></a>

## PART XIII — DIFFICULTY & ACCESSIBILITY

<a id="m13-t033-s251"></a>

## 160. STORY MODE

Wound threshold cao hơn.

Bleeding chậm hơn.

Recovery tốt hơn.

Không xóa hệ injury.

---

<a id="m13-t033-s252"></a>

## 161. STANDARD

Design baseline.

---

<a id="m13-t033-s253"></a>

## 162. TACTICAL

Wound/armor interaction nghiêm hơn.

Equipment preparation quan trọng.

Enemy biết tận dụng weakness.

---

<a id="m13-t033-s254"></a>

## 163. LEGACY MODE

Permanent injury/death đầy đủ.

Không nhất thiết tăng enemy damage artificial.

---

<a id="m13-t033-s255"></a>

## 164. ACCESSIBILITY OPTION — REDUCED PERMANENT INJURY

Player vẫn có wound.

Nhưng permanent disability giảm hoặc tắt.

Không ảnh hưởng core narrative quá nhiều.

---

<a id="m13-t033-s256"></a>

## PART XIV — QUALITY GATES

<a id="m13-t033-s257"></a>

## 165. WEAPON QUALITY GATE

Mỗi Weapon Family phải trả lời được:

1. Tactical identity?
2. Range?
3. Strong context?
4. Weak context?
5. Defensive option?
6. Environmental utility?
7. Technique space?
8. Profession/cultural context?
9. Có khác weapon khác ngoài damage không?

Nếu không:

merge hoặc bỏ.

---

<a id="m13-t033-s258"></a>

## 166. ARMOR QUALITY GATE

Mỗi protection phải có:

coverage cụ thể;

loại attack nó chống tốt;

mobility/environment cost;

craft/material context.

Không được chỉ:

Defense +X.

---

<a id="m13-t033-s259"></a>

## 167. ITEM QUALITY GATE

Named item phải có ít nhất một trong:

Maker.

Owner.

History.

Cultural meaning.

Unique fit.

Technique connection.

Không đặt tên chỉ vì rarity.

---

<a id="m13-t033-s260"></a>

## 168. INJURY QUALITY GATE

Một injury nghiêm trọng phải tác động tối thiểu một trong:

Combat.

Travel.

Profession.

Relationship.

Psychology.

Time.

Nếu chỉ:

HP regen -5%

thì chưa đủ.

---

<a id="m13-t033-s261"></a>

## PART XV — INTERACTION WITH EXISTING GDD

<a id="m13-t033-s262"></a>

## 169. ATTRIBUTES

THỂ:

Impact, load, stamina.

XẢO:

handling, precision.

GIÁC:

targeting, defense reading.

TRÍ:

equipment assessment.

TÂM:

pain/fear management.

KHÍ:

morale.

---

<a id="m13-t033-s263"></a>

## 170. SKILLS

Weapon Skill quyết định competence.

Craft Skill quyết định quality/repair.

Medicine quyết định treatment.

Knowledge quyết định appraisal.

---

<a id="m13-t033-s264"></a>

## 171. PROFESSION

Smith:

craft/repair.

Hunter:

bow/spear context.

Boatman:

water gear.

Healer:

wound treatment.

---

<a id="m13-t033-s265"></a>

## 172. CLASS

Class mở cách sử dụng equipment.

Không mở equipment permission.

Lạc Binh:

formation.

Hộ Vệ:

shield.

Liệp Nhân:

bow/hunting weapon.

Xảo Nhân:

equipment modification.

---

<a id="m13-t033-s266"></a>

## 173. PARTY

Equipment ownership riêng.

Carry distribution.

Rescue.

Treatment.

Inheritance.

Companion reaction.

---

<a id="m13-t033-s267"></a>

## 174. NARRATIVE

Weapon có history.

Wound tạo memory.

Death tạo inheritance.

Loot tạo social consequence.

---

<a id="m13-t033-s268"></a>

## 175. ECONOMY

Material shortage thay giá.

War thay demand.

Craft network quyết availability.

---

<a id="m13-t033-s269"></a>

## 176. WORLD SIMULATION

Master craftsman death:

item quality region giảm.

Trade route cut:

metal shortage.

Cổ Loa development:

military standardization tăng.

---

<a id="m13-t033-s270"></a>

## 177. LEGACY

Weapon.

Technique.

Scar.

Armor.

Workshop.

Tất cả có thể tồn tại sau character.

---

<a id="m13-t033-s271"></a>

## PART XVI — DEFERRED TO PRODUCTION

Production mới cần khóa:

- số liệu Impact cụ thể;
- Penetration formula;
- exact weapon length;
- exact material table;
- wound probabilities;
- exact bleeding rates;
- exact recovery days;
- armor material statistics;
- exact inventory capacity;
- tất cả weapon variants;
- tất cả poison;
- toàn bộ craft recipes;
- historical catalog chi tiết;
- animation;
- gore level;
- item art;
- exact crossbow reconstruction.

Riêng **vũ khí, giáp và trang bị theo khảo cổ**, production cần một pass nghiên cứu chuyên biệt để phân loại:

**Archaeologically attested**

**Historically plausible**

**Legend-derived**

**Game-canon**

và lưu tag này trong Content Database.

---

<a id="m13-t033-s272"></a>

## 178. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

Weapon không phân cấp chủ yếu bằng damage.

Reach là mechanic cốt lõi.

Giáo là weapon control mạnh nhưng yếu khi bị áp sát.

Rìu có high impact/high commitment.

Dao mạnh ở close range và có social/status value.

Kiếm không phải universal best weapon.

Cung và nỏ có combat rhythm khác nhau.

Nỏ Cổ Loa được xử lý như Historical Technology Thread.

Khiên là active equipment.

Armor có Coverage.

Không có universal Armor Class.

Vitality và Wound tách biệt.

Wound dựa trên hit severity, không chỉ HP depletion.

Có wound location nhưng không simulation quá vi mô.

Bleeding và Stability là mechanic trọng tâm.

Healing trong combat chủ yếu là stabilization.

Persistent wound cần thời gian hồi.

Permanent impairment hiếm nhưng tồn tại.

Injury có thể thay đổi Career/Class trajectory.

Không có potion hồi HP tức thời làm trụ cột.

Items có ownership và history.

Looting có social consequence.

Không dùng color rarity làm core.

Named item hình thành từ lịch sử.

Equipment quality gồm material/craft/fit/condition/provenance.

NPC và player dùng cùng item rule.

Enemy weapon mạnh không tự yếu đi khi player loot.

Combat reward không dựa vào loot treadmill.

---

<a id="m13-t033-s273"></a>

## 179. CORE EQUIPMENT FANTASY

Một RPG truyền thống có thể hỏi:

> “Tôi đã tìm được thanh kiếm mạnh hơn chưa?”

Game này nên khiến player hỏi:

> “Cây giáo này còn chịu nổi một chuyến nữa không?”

> “Tôi có nên mang tấm che ngực này nếu ngày mai phải vượt sông?”

> “Tôi có thực sự muốn lấy con dao của người vừa chết khi gia đình ông ấy vẫn sống trong làng?”

> “Tôi sửa cây cung của cha hay tháo nó lấy vật liệu?”

> “Tôi nên tiếp tục chiến đấu với cái chân đang chảy máu hay rút lui?”

Và nhiều năm sau:

> “Đây vẫn là cây giáo Mộc đã dùng khi chúng tôi còn ở Bến Vạc.”

Đó là mục tiêu.

---

<a id="m13-t033-s274"></a>

## 180. CORE INJURY FANTASY

Injury không tồn tại để trừng phạt player.

Nó tồn tại để tạo:

**risk**

→ **decision**

→ **consequence**

→ **adaptation**

→ **story.**

Một vết thương có thể làm một warrior mất nghề.

Nhưng cũng có thể biến họ thành leader.

Một hunter mất một mắt có thể trở thành tracker giỏi hơn vì học cách dựa vào những dấu hiệu khác.

Một companion được player cứu khỏi bleeding có thể nhớ chuyện đó suốt đời.

Một scar có thể trở thành dấu hiệu người khác nhận ra.

Một trận chiến vì vậy không kết thúc khi enemy cuối cùng bỏ chạy.

Nó tiếp tục sống:

> **trên cơ thể những người đã tham gia.**

---

<a id="m13-t033-s275"></a>

## 181. MODULE OUTCOME

Sau Module 13, combat stack của GDD đã có cấu trúc tương đối hoàn chỉnh:

**Character**

→ Attributes / Skills

→ Class Techniques

→ Party Coordination

→ Weapon

→ Protection

→ Battlefield Position

→ Attack / Defense

→ Vitality

→ Wound / Status

→ Retreat / Victory

→ Treatment

→ Recovery

→ Career / Psychology / Relationship consequence

→ Chronicle / Legacy.

Nó tạo một vòng combat thực sự nối vào phần còn lại của game thay vì là một minigame chiến đấu độc lập.

Một điểm tôi muốn giữ đặc biệt khi sang production là **nỏ Cổ Loa không trở thành “legendary weapon quest” đơn giản**. Bằng chứng khảo cổ cho phép ta dùng cung/nỏ và các phục dựng Cổ Loa làm nền cảm hứng, nhưng chính việc biến nó thành một **hệ thống công nghệ–sản xuất–quân sự rồi dần bị ký ức tập thể cô đọng thành “Nỏ thần”** mới phù hợp nhất với toàn bộ GDD mà chúng ta đang xây. 

Bước tiếp theo nên chuyển khỏi combat sang **Exploration + Travel + Survival + World Map**, vì lúc đó chúng ta sẽ nối được character/party/equipment với 12 đại vùng. Sau module đó mới đến **Dungeon & Encounter System**; nếu làm ngược, dungeon sẽ rất dễ bị thiết kế như “bản đồ có quái” thay vì những không gian khám phá có logic riêng.

<!-- SOURCE T033 END -->

