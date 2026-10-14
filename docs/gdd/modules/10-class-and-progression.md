# Module 10 — Class, Specialization, Skill Tree và Multiclass

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T027](../../source/conversation.md#t027)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Class xuất hiện từ evidence cuộc đời; không chỉ check một ngưỡng Skill. Người chơi có thể nhận hoặc không nhận con đường đã mở; không chọn class ở New Game.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Skills, techniques, profession history, repeated behavior, recognition, mentors. |
| Đầu ra / bên sở hữu | Class eligibility, specialization và learning plans; Characters sở hữu. |
| Đọc cùng | 08, 09, 11, 27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Hai actor cùng skill nhưng thiếu bằng chứng khác nhau không tự mở cùng class. |
| Việc chưa được chứng minh | Node catalog, multiclass balance và visibility UX. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T027 BEGIN -->

Đây là cụm hệ thống quyết định **player agency** của game: nhân vật được sinh ngẫu nhiên, nhưng từ thời điểm người chơi bắt đầu điều khiển, chính hành vi của họ sẽ dần tạo ra Class, Specialization và phong cách chơi. Tôi sẽ khóa nó ở mức đủ đưa thẳng vào GDD, còn số lượng node/balance cụ thể để production mở rộng.

<a id="m10-t027-s001"></a>

## GDD MODULE 10 — CLASS EMERGENCE, SPECIALIZATION, SKILL TREE & MULTICLASS SYSTEM

<a id="m10-t027-s002"></a>

## 1. DESIGN PHILOSOPHY

Game không sử dụng mô hình:

> Tạo nhân vật → chọn Fighter → Level 2 → Level 3 → chọn Subclass.

Thay vào đó:

> Character được sinh ra → sống → học → làm nghề → giải quyết vấn đề → hình thành phong cách → được người khác nhận biết → Class xuất hiện.

Class không phải thứ character chọn trước khi sống.

Class là:

> **một danh tính gameplay được hình thành từ lịch sử hành động của character.**

Nguyên tắc trung tâm:

**Origin is given.**

**Profession is lived.**

**Class is earned.**

**Specialization is demonstrated.**

**Legacy is remembered.**

---

<a id="m10-t027-s003"></a>

## 2. PROFESSION KHÁC CLASS

Hai hệ thống tuyệt đối không được gộp.

Ví dụ:

Character có Profession:

**Thợ đúc.**

Nhưng Class có thể là:

**Xảo Nhân**

nếu dùng kỹ nghệ để giải quyết vấn đề.

Hoặc:

**Lạc Binh**

nếu sau chiến tranh character trở thành chiến binh.

Hoặc:

**Thủ Lĩnh**

nếu điều hành cả cộng đồng thợ.

Hoặc:

không có Class rõ ràng.

Một farmer vẫn có thể trở thành Liệp Nhân.

Một hunter vẫn có thể trở thành Sứ Giả.

Một Vu Chúc nghề nghiệp có thể không phát triển Class Vu Chúc nếu gameplay của player không tập trung vào nghi lễ/tâm linh.

---

<a id="m10-t027-s004"></a>

## 3. CLASS KHÔNG XUẤT HIỆN NGAY

Mọi character bắt đầu ở trạng thái:

**Unformed**

Character chỉ có:

Attributes.

Skills.

Knowledge.

Techniques cơ bản.

Profession.

Personality.

Life History.

Không có Class Level.

---

<a id="m10-t027-s005"></a>

## 4. CLASS CANDIDATE

Khi hành vi player bắt đầu tạo pattern, engine sinh:

**Class Candidate.**

Ví dụ:

Player thường xuyên:

theo dấu;

quan sát;

chuẩn bị bẫy;

tránh giao tranh trực diện;

sử dụng cung;

nghiên cứu thú.

Engine bắt đầu theo dõi:

`Candidate: Liệp Nhân`

Nhưng player chưa phải Liệp Nhân.

---

<a id="m10-t027-s006"></a>

## 5. CLASS EVIDENCE MODEL

Mỗi Class được hình thành từ năm loại Evidence.

<a id="m10-t027-s007"></a>

### Capability Evidence

Character thực sự có Skill cần thiết không?

Ví dụ Liệp Nhân:

Truy Dấu.

Săn & Bẫy.

Quan Sát.

một weapon hoặc survival skill.

---

<a id="m10-t027-s008"></a>

### Behaviour Evidence

Player có thực sự sử dụng những capability đó không?

Có Skill 70 nhưng chưa bao giờ săn không đủ.

---

<a id="m10-t027-s009"></a>

### Milestone Evidence

Player đã làm được chuyện đủ ý nghĩa chưa?

Ví dụ:

theo dấu một người mất tích qua hai ngày;

hạ một thú nguy hiểm;

dẫn party qua rừng;

tìm được con mồi bị người khác mất dấu.

---

<a id="m10-t027-s010"></a>

### Social Evidence

Người khác có công nhận character theo identity đó không?

Ví dụ:

hunter khác hỏi ý kiến.

Settlement thuê player dẫn đường.

Mentor công nhận.

NPC bắt đầu gọi player là người săn giỏi.

---

<a id="m10-t027-s011"></a>

### Identity Evidence

Character có duy trì con đường đủ lâu để trở thành một phần con người họ không?

Một lần cầm cung không biến thành Liệp Nhân.

---

<a id="m10-t027-s012"></a>

## 6. CLASS EMERGENCE KHÔNG DÙNG XP

Engine không dùng:

`Liệp Nhân XP = 900/1000`

Thay vào đó lưu:

```text
Skill Pattern
Behaviour Pattern
Milestones
Recognition
Identity Duration
```

Class chỉ hình thành khi đủ nhiều loại Evidence khác nhau.

Điều này ngăn:

> grind một hành động duy nhất để unlock class.

---

<a id="m10-t027-s013"></a>

## 7. CLASS STAGES

Mỗi Class có năm trạng thái.

| Stage | Ý nghĩa |
|---|---|
| Candidate | con đường đang hình thành |
| Emerging | gameplay pattern đã rõ |
| Recognized | character được công nhận |
| Established | Class trở thành identity ổn định |
| Mastered | đạt trình độ bậc thầy |
| Legacy | Class để lại truyền thừa |

Legacy là trạng thái đặc biệt sau Mastered.

---

<a id="m10-t027-s014"></a>

## 8. CANDIDATE STAGE

UI không cần thông báo trực tiếp ngay.

Player có thể nhận hint:

> “Bạn ngày càng quen với việc đọc dấu vết trong tự nhiên.”

Hoặc NPC nói:

> “Mắt ngươi nhìn rừng giống người đi săn hơn người đi đường.”

Game không nói:

`Liệp Nhân 42%`.

---

<a id="m10-t027-s015"></a>

## 9. EMERGING STAGE

Khi pattern đủ rõ:

Character Sheet xuất hiện:

<a id="m10-t027-s016"></a>

#### Con đường đang hình thành

**Liệp Nhân**

Player không bấm:

> Accept Class.

Nếu tiếp tục hành động phù hợp:

con đường mạnh lên.

Nếu bỏ:

Candidate dần im đi.

Không bị penalty.

---

<a id="m10-t027-s017"></a>

## 10. RECOGNIZED STAGE

Class trở thành chính thức khi character có:

Capability.

Behaviour.

Milestone.

và ít nhất một dạng Recognition.

Recognition có thể đến từ:

mentor;

community;

faction;

peer;

hoặc exceptional self-proven milestone.

Chronicle ghi:

> “Từ mùa săn năm ấy, người tại Khe Đá Trắng bắt đầu coi Mân như một Liệp Nhân.”

Đây là Class Unlock thật sự.

---

<a id="m10-t027-s018"></a>

## 11. ESTABLISHED STAGE

Player đã có:

nhiều Techniques;

một Specialization;

reputation phù hợp;

nhiều milestone.

Class bắt đầu ảnh hưởng:

dialogue;

quest exposure;

NPC expectation;

career;

party role.

---

<a id="m10-t027-s019"></a>

## 12. MASTERED STAGE

Master không chỉ có số lớn hơn.

Master có khả năng:

dạy;

sáng tạo Technique;

đánh giá người khác;

mở solution đặc biệt;

truyền lại Class identity.

---

<a id="m10-t027-s020"></a>

## 13. LEGACY STAGE

Character đã tạo ra thứ vượt khỏi bản thân.

Ví dụ:

một học trò nổi tiếng;

một trường phái săn;

một kỹ thuật mới;

một chiến thuật;

một nghi lễ;

một phương pháp chữa bệnh.

Character có thể chết.

Legacy vẫn tồn tại.

---

<a id="m10-t027-s021"></a>

## 14. CLASS KHÔNG THAY ATTRIBUTES

Unlock Liệp Nhân không tự cho:

GIÁC +2.

Class chủ yếu mở:

Techniques.

Doctrines.

Synergies.

Reactions.

Narrative access.

Không biến character thành người khác một cách nhân tạo.

---

<a id="m10-t027-s022"></a>

## 15. CLASS TREE ARCHITECTURE

Mỗi Class Tree gồm năm loại node.

<a id="m10-t027-s023"></a>

### Technique Node

Mở action mới.

---

<a id="m10-t027-s024"></a>

### Reaction Node

Mở phản ứng đặc biệt.

---

<a id="m10-t027-s025"></a>

### Mastery Node

Thay đổi cách Skill đã biết hoạt động.

---

<a id="m10-t027-s026"></a>

### Doctrine Node

Định hình phong cách.

Character chỉ duy trì một số Doctrine active cùng lúc.

---

<a id="m10-t027-s027"></a>

### Legacy Node

Tác động tới:

student;

party;

settlement;

profession;

world.

Đây là endgame progression.

---

<a id="m10-t027-s028"></a>

## 16. KHÔNG DÙNG GENERIC SKILL POINT

Player không lên level rồi nhận:

`+1 Class Point`.

Node yêu cầu:

Skill.

Class Stage.

Milestone.

Training.

Insight.

Một số node còn cần:

mentor;

item;

cultural knowledge;

event.

---

<a id="m10-t027-s029"></a>

## 17. PLAYER VẪN CÓ QUYỀN LỰA CHỌN

Class xuất hiện từ hành vi.

Nhưng khi nhiều Technique trở nên eligible:

player quyết định học cái nào.

Ví dụ Liệp Nhân đủ điều kiện học:

**Bám Dấu Không Nghỉ**

hoặc:

**Một Mũi Đợi Lâu**

Player chọn đầu tư thời gian vào một trong hai.

Agency nằm trong quá trình sống.

Không nằm ở màn hình character creation.

---

<a id="m10-t027-s030"></a>

## 18. SPECIALIZATION — CHUYÊN HƯỚNG

Subclass trong GDD được gọi là:

**Specialization / Chuyên hướng.**

Nó cũng không được chọn tùy ý từ menu.

Specialization cần:

Class Established

+

Skill Pattern

+

Behaviour Pattern

+

Milestone đặc thù.

Ví dụ Liệp Nhân thường:

theo dấu người;

điều tra mất tích;

truy tìm kẻ chạy trốn.

Có thể hình thành:

**Tầm Tung**

Trong khi người chuyên:

cung;

chờ;

đánh một mục tiêu từ xa

hình thành:

**Cung Liệp.**

---

<a id="m10-t027-s031"></a>

## 19. SPECIALIZATION KHÔNG KHÓA TOÀN BỘ NHÁNH KHÁC

Player có thể học Technique ngoài specialization.

Nhưng:

Specialization làm một số synergy mạnh hơn.

Không dùng mô hình:

> chọn nhánh A → mất vĩnh viễn B và C.

Đời người linh hoạt hơn như vậy.

---

<a id="m10-t027-s032"></a>

## 20. 11 CORE CLASS ARCHETYPES

Bộ Class nền ban đầu gồm:

| Class | Fantasy gameplay |
|---|---|
| Lạc Binh | chiến đấu có kỹ thuật và đội hình |
| Hộ Vệ | bảo vệ người khác, giữ tuyến |
| Liệp Nhân | săn, theo dấu, kiểm soát môi trường |
| Trinh Binh | trinh sát, xâm nhập, thông tin |
| Thủy Binh | sông nước, thuyền, chiến đấu thủy vực |
| Dược Nhân | y thuật, chẩn đoán, độc, hồi phục |
| Xảo Nhân | chế tác, sửa chữa, công trình, cơ cấu |
| Vu Chúc | nghi lễ, Mạch, linh giới |
| Sứ Giả | ngoại giao, thương lượng, ngôn ngữ |
| Thủ Lĩnh | chỉ huy, tổ chức, morale, quyền lực |
| Người Giữ Ký Ức | lịch sử, điều tra, luật tục, truyền khẩu |

Đây là working names game-canon.

Không phải khẳng định lịch sử có hệ thống Class như vậy.

---

<a id="m10-t027-s033"></a>

## 21. LẠC BINH

<a id="m10-t027-s034"></a>

### Fantasy

Người sống bằng kỹ thuật chiến đấu có tổ chức.

Không phải generic Fighter.

Lạc Binh mạnh nhất khi hiểu:

vũ khí;

khoảng cách;

đồng đội;

địa hình;

formation.

---

<a id="m10-t027-s035"></a>

### Emergence Requirements

Một hoặc nhiều weapon skill đủ tốt.

Combat experience thực tế.

Tactics exposure.

Nhiều encounter giải quyết bằng chiến đấu trực tiếp.

Ít nhất một combat milestone.

---

<a id="m10-t027-s036"></a>

### Ba Chuyên hướng

<a id="m10-t027-s037"></a>

#### Trận Binh

Mạnh trong đội hình.

Techniques tập trung:

formation;

đồng đội;

giữ tuyến.

<a id="m10-t027-s038"></a>

#### Kích Binh

Tấn công chủ động.

Spear/axe pressure.

Phá thế đối phương.

<a id="m10-t027-s039"></a>

#### Du Binh

Chiến binh linh hoạt ngoài đội hình lớn.

Dùng terrain.

Chuyển vị trí.

Đánh rồi rút.

---

<a id="m10-t027-s040"></a>

### Sample Techniques

**Giữ Mũi**

Nếu đồng minh bên cạnh cùng giữ formation, cả hai khó bị đẩy lùi.

**Đổi Thế**

Sau khi né hoặc parry thành công, thay stance mà không mất action chính.

**Ép Khoảng**

Dùng weapon reach buộc đối thủ lựa chọn lùi hoặc chịu disadvantage khi tiến.

**Nhìn Trận Tuyến**

Quan sát một formation đủ lâu để nhận biết điểm yếu tương đối.

---

<a id="m10-t027-s041"></a>

## 22. HỘ VỆ

<a id="m10-t027-s042"></a>

### Fantasy

Không phải “tank có nhiều HP”.

Hộ Vệ kiểm soát:

ai bị đánh;

đường tiếp cận;

space;

risk.

---

<a id="m10-t027-s043"></a>

### Emergence

Protect ally actions.

Shield/block skill.

Nhiều lần chấp nhận rủi ro để bảo vệ người khác.

High responsibility evidence.

---

<a id="m10-t027-s044"></a>

### Chuyên hướng

<a id="m10-t027-s045"></a>

#### Khiên Thủ

Tập trung block và tuyến phòng thủ.

<a id="m10-t027-s046"></a>

#### Hộ Chủ

Bảo vệ một người cụ thể.

Escort.

Bodyguard.

<a id="m10-t027-s047"></a>

#### Trấn Tuyến

Kiểm soát choke point và vị trí.

---

<a id="m10-t027-s048"></a>

### Techniques

**Đứng Sau Ta**

Chủ động chuyển threat từ ally sang bản thân nếu positioning hợp lệ.

**Không Qua Đây**

Đối thủ muốn vượt vị trí phải vượt control check.

**Khiên Che Hai Người**

Hy sinh mobility để tăng protection cho ally cạnh bên.

**Chịu Một Bước**

Cho phép bị đẩy nhẹ thay vì mất formation hoàn toàn.

---

<a id="m10-t027-s049"></a>

## 23. LIỆP NHÂN

<a id="m10-t027-s050"></a>

### Fantasy

Liệp Nhân thắng trước khi combat bắt đầu.

Họ:

tìm;

hiểu;

chọn địa hình;

chờ;

đánh đúng lúc.

---

<a id="m10-t027-s051"></a>

### Emergence

Tracking.

Observation.

Hunting.

Environment Knowledge.

Repeated hunting/investigation behaviour.

---

<a id="m10-t027-s052"></a>

### Chuyên hướng

<a id="m10-t027-s053"></a>

#### Tầm Tung

Theo dấu người và vật.

Tìm mất tích.

Truy tìm.

<a id="m10-t027-s054"></a>

#### Cung Liệp

Khai thác ranged precision và preparation.

<a id="m10-t027-s055"></a>

#### Dị Thú Liệp

Hiểu hành vi của thú nguy hiểm và sinh vật bất thường.

---

<a id="m10-t027-s056"></a>

## 24. LIỆP NHÂN — SAMPLE TREE CHI TIẾT

Đây là Class được dùng làm chuẩn thiết kế Skill Tree.

<a id="m10-t027-s057"></a>

#### FOUNDATION

**Đọc Dấu**

Truy Dấu + GIÁC.

Cho phép xác định:

hướng;

số lượng tương đối;

tốc độ sơ bộ.

---

**Chọn Chỗ Đợi**

Sau khi quan sát terrain, Liệp Nhân xác định một vị trí phục kích hiệu quả.

---

<a id="m10-t027-s058"></a>

#### BRANCH — TẦM TUNG

**Dấu Người Mang Nặng**

Nhận biết target đang mang tải.

↓

**Dấu Cố Che**

Có thể phát hiện trail bị cố ý xóa nếu kỹ thuật che không vượt expertise.

↓

**Theo Dấu Qua Người Khác**

Dùng lời khai + vật dụng + route để tái tạo một phần hành trình ngay cả khi mất physical trail.

↓

**Không Mất Dấu**

Signature Technique.

Khi đã có đủ ba loại clue độc lập về target, việc mất một loại clue không reset investigation.

---

<a id="m10-t027-s059"></a>

#### BRANCH — CUNG LIỆP

**Đợi Hơi Thở**

Nếu không hành động trong một khoảng ngắn và target chưa biết vị trí, ranged accuracy tăng.

↓

**Một Mũi Chọn Chỗ**

Cho phép nhắm vào mục tiêu chức năng:

chân;

tay;

dây;

vật đang cầm

thay vì chỉ gây damage.

↓

**Không Bắn Khi Chưa Chắc**

Nếu player tự bỏ một shot có accuracy thấp, nhận Focus cho shot kế tiếp.

↓

**Một Mũi Đợi Lâu**

Signature.

Một lần tấn công được chuẩn bị cực kỹ có thể bỏ qua một phần defense do positioning, không phải damage multiplier đơn giản.

---

<a id="m10-t027-s060"></a>

#### BRANCH — DỊ THÚ

**Đọc Phân & Lông**

Xác định condition sơ bộ của animal.

↓

**Biết Khi Nó Sợ**

Nhận biết trạng thái fight/flight.

↓

**Không Phải Mọi Thứ Đều Là Con Mồi**

Mở non-combat solutions với một số animal encounter.

↓

**Thợ Săn Của Thứ Không Có Tên**

Cho phép sử dụng accumulated observation đối với creature lạ để tạo temporary bestiary knowledge.

---

<a id="m10-t027-s061"></a>

## 25. TRINH BINH

<a id="m10-t027-s062"></a>

### Fantasy

Information trước confrontation.

Trinh Binh giỏi:

đến;

nhìn;

hiểu;

rời đi.

---

<a id="m10-t027-s063"></a>

### Emergence

Stealth.

Observation.

Navigation.

Scouting behaviour.

Information delivery.

Avoid unnecessary confrontation.

---

<a id="m10-t027-s064"></a>

### Chuyên hướng

<a id="m10-t027-s065"></a>

#### Tiền Trạm

Recon.

Route.

Threat detection.

<a id="m10-t027-s066"></a>

#### Dạ Hành

Infiltration.

Stealth.

Night movement.

<a id="m10-t027-s067"></a>

#### Mật Thám

Information network.

Deception.

Faction intelligence.

---

<a id="m10-t027-s068"></a>

### Techniques

**Đi Trước Một Quãng**

Scout tách party an toàn hơn và gửi signal.

**Nhớ Lối Rút**

Sau khi quan sát route, giảm nguy cơ bị kẹt khi retreat.

**Không Để Dấu Không Cần Thiết**

Giảm trace của party.

**Nhìn Mà Không Để Người Nhìn Lại**

Observation từ concealment ít tăng detection.

---

<a id="m10-t027-s069"></a>

## 26. THỦY BINH

<a id="m10-t027-s070"></a>

### Fantasy

Không đơn giản là warrior trên thuyền.

Class này làm chủ:

dòng nước;

balance;

boat positioning;

rescue;

combat thủy vực.

---

<a id="m10-t027-s071"></a>

### Emergence

Swimming.

Boat handling.

Water Knowledge.

Repeated water encounters.

Một milestone quan trọng trên sông/biển.

---

<a id="m10-t027-s072"></a>

### Chuyên hướng

<a id="m10-t027-s073"></a>

#### Đạo Thủy

Navigation và đọc nước.

<a id="m10-t027-s074"></a>

#### Thủy Kích

Combat trên thuyền, bờ nước và crossing.

<a id="m10-t027-s075"></a>

#### Cứu Thủy

Rescue, survival và support.

---

<a id="m10-t027-s076"></a>

### Techniques

**Cắt Dòng**

Đổi hướng thuyền hiệu quả trong dòng xiết.

**Chân Trên Ván**

Giảm penalty combat trên bề mặt chuyển động.

**Người Rơi Nước**

Reaction cứu ally rơi khỏi thuyền.

**Dòng Nước Cũng Là Địa Hình**

Cho phép sử dụng current như tactical tool.

---

<a id="m10-t027-s077"></a>

## 27. DƯỢC NHÂN

<a id="m10-t027-s078"></a>

### Fantasy

Dược Nhân không phải Cleric.

Không spam heal.

Sức mạnh nằm ở:

diagnosis;

preparation;

medicine;

toxin;

recovery.

---

<a id="m10-t027-s079"></a>

### Emergence

Herbalism.

Diagnosis.

Treatment.

Nhiều lần chữa người.

Medicine milestone.

---

<a id="m10-t027-s080"></a>

### Chuyên hướng

<a id="m10-t027-s081"></a>

#### Y Thương

Trauma và battlefield care.

<a id="m10-t027-s082"></a>

#### Dược Sư

Thuốc, thảo dược và preventive care.

<a id="m10-t027-s083"></a>

#### Độc Dược

Poison, antidote và toxicology.

---

<a id="m10-t027-s084"></a>

### Techniques

**Cầm Máu Đúng Chỗ**

Tăng khả năng ổn định severe wound.

**Chẩn Trước Trị Sau**

Sau Diagnosis thành công, treatment hiệu quả hơn.

**Liều Là Thuốc**

Cho phép thay đổi liều một số substance để chuyển hiệu ứng.

**Nhìn Triệu Chứng, Không Nhìn Lời Đồn**

Giảm ảnh hưởng false cultural diagnosis lên medical reasoning.

---

<a id="m10-t027-s085"></a>

## 28. XẢO NHÂN

<a id="m10-t027-s086"></a>

### Fantasy

Giải quyết vấn đề bằng:

vật liệu;

công cụ;

cấu trúc;

cơ chế.

---

<a id="m10-t027-s087"></a>

### Emergence

Craft Skills.

Repair.

Engineering.

Repeated crafted solutions.

Một công trình hoặc sản phẩm milestone.

---

<a id="m10-t027-s088"></a>

### Chuyên hướng

<a id="m10-t027-s089"></a>

#### Binh Khí

Weapon/tool construction.

<a id="m10-t027-s090"></a>

#### Công Tạo

Công trình, cầu, kè, thành.

<a id="m10-t027-s091"></a>

#### Cơ Tác

Bẫy, cơ cấu, mechanism, cải tiến.

---

<a id="m10-t027-s092"></a>

### Techniques

**Nhìn Vật Biết Chỗ Yếu**

Inspection reveal structural weak point.

**Sửa Để Dùng Được**

Field repair cho phép item broken trở lại trạng thái usable tạm thời.

**Dùng Thứ Đang Có**

Thay vật liệu với penalty nhỏ hơn.

**Từ Một Làm Thành Nhiều**

Khi process đủ chuẩn hóa, tăng hiệu suất production.

Technique này đặc biệt quan trọng cho Cổ Loa.

---

<a id="m10-t027-s093"></a>

## 29. VU CHÚC

<a id="m10-t027-s094"></a>

### Fantasy

Vu Chúc đứng ở giao điểm:

community;

ritual;

belief;

Mạch;

death;

memory.

Không phải Wizard.

---

<a id="m10-t027-s095"></a>

### Emergence

Ritual Skill.

Cultural Knowledge.

Repeated ritual participation.

Spiritual encounters.

Cảm Mạch không bắt buộc với mọi nhánh.

---

<a id="m10-t027-s096"></a>

### Chuyên hướng

<a id="m10-t027-s097"></a>

#### Người Giữ Lễ

Nghi lễ cộng đồng.

Legitimacy.

Morale.

<a id="m10-t027-s098"></a>

#### Dẫn Vong

Tang lễ, death, unresolved spirit narratives.

<a id="m10-t027-s099"></a>

#### Cảm Mạch

Dành cho character thực sự có Mạch sensitivity.

---

<a id="m10-t027-s100"></a>

### Techniques

**Lễ Không Chỉ Có Lời**

Phát hiện một ritual thất bại do sequence, vật phẩm hoặc participant sai.

**Gọi Đúng Tên Người Chết**

Tang nghi có tên và identity rõ ràng giảm spiritual/social unrest.

**Nghe Chỗ Mạch Gãy**

Cảm nhận anomaly tại một sacred location.

Không cho biết nguyên nhân.

**Mượn Niềm Tin Của Đám Đông**

Một nghi lễ thành công có thể ổn định morale settlement dù không có supernatural outcome.

---

<a id="m10-t027-s101"></a>

## 30. SỨ GIẢ

<a id="m10-t027-s102"></a>

### Fantasy

Sứ Giả không phải “Charisma Class”.

Họ hiểu:

người;

lợi ích;

ngôn ngữ;

thứ bậc;

lời hứa;

thông tin.

---

<a id="m10-t027-s103"></a>

### Emergence

Negotiation.

Language.

Custom.

Multiple conflict resolutions.

Travel between communities.

---

<a id="m10-t027-s104"></a>

### Chuyên hướng

<a id="m10-t027-s105"></a>

#### Hòa Sứ

Đàm phán và hòa giải.

<a id="m10-t027-s106"></a>

#### Phiên Sứ

Ngôn ngữ và cross-cultural mediation.

<a id="m10-t027-s107"></a>

#### Mật Sứ

Sensitive information, secret negotiation, political message.

---

<a id="m10-t027-s108"></a>

### Techniques

**Tìm Điều Cả Hai Cùng Sợ**

Trong negotiation, phát hiện một shared negative outcome.

**Không Dịch Chỉ Bằng Từ**

Giảm cultural misunderstanding.

**Lời Hứa Có Giá**

Có thể formalize một promise thành Narrative Obligation rõ ràng.

**Nói Với Người Không Có Mặt**

Khi truyền thông điệp, giảm distortion qua một bước intermediary.

---

<a id="m10-t027-s109"></a>

## 31. THỦ LĨNH

<a id="m10-t027-s110"></a>

### Fantasy

Không phải class “buff party”.

Thủ Lĩnh điều khiển:

coordination;

morale;

responsibility;

organization.

---

<a id="m10-t027-s111"></a>

### Emergence

Leadership.

Repeated command.

Others voluntarily follow.

Responsibility for group outcome.

Major leadership milestone.

---

<a id="m10-t027-s112"></a>

### Chuyên hướng

<a id="m10-t027-s113"></a>

#### Trận Lĩnh

Command trong combat.

<a id="m10-t027-s114"></a>

#### Dân Lĩnh

Settlement/community organization.

<a id="m10-t027-s115"></a>

#### Hội Lĩnh

Faction building, coalition và negotiation giữa nhiều nhóm.

---

<a id="m10-t027-s116"></a>

### Techniques

**Người Nào Việc Nấy**

Phân công đúng Skill giúp group task hiệu quả.

**Ta Chịu Trách Nhiệm**

Nhận một phần reputation consequence thay subordinate.

**Không Để Tin Đồn Chạy Nhanh Hơn Lệnh**

Giảm information distortion trong organization.

**Giữ Người Khi Hàng Ngũ Vỡ**

Cho phép attempt morale recovery trong crisis.

---

<a id="m10-t027-s117"></a>

## 32. NGƯỜI GIỮ KÝ ỨC

<a id="m10-t027-s118"></a>

### Fantasy

Class đặc biệt quan trọng đối với game content-heavy.

Đây là người:

ghi nhớ;

so sánh;

truyền lại;

điều tra;

giữ luật tục;

nhận ra lịch sử đang bị bóp méo.

Không nhất thiết biết chữ.

---

<a id="m10-t027-s119"></a>

### Emergence

Ghi Nhớ & Truyền Khẩu.

Suy Luận.

Luật Tục.

Historical Knowledge.

Repeated investigation/history behaviour.

---

<a id="m10-t027-s120"></a>

### Chuyên hướng

<a id="m10-t027-s121"></a>

#### Truyền Sử

Oral history và genealogy.

<a id="m10-t027-s122"></a>

#### Giải Tích

Investigation, evidence, contradiction.

<a id="m10-t027-s123"></a>

#### Giữ Luật

Custom, obligation, inheritance và arbitration.

---

<a id="m10-t027-s124"></a>

### Techniques

**Hai Câu Chuyện Không Giống Nhau**

Khi nghe hai version cùng event, highlight contradiction.

**Tên Người Không Nên Mất**

Giữ identity chain tốt hơn qua nhiều generation.

**Nhớ Ai Đã Nói Gì**

Giảm rumor confusion trong investigation.

**Chuyện Được Kể Khác Đi**

Nhận biết một số dấu hiệu Myth Formation hoặc political rewriting.

Đây là Class cực mạnh về narrative nhưng gần như không có combat power trực tiếp.

---

<a id="m10-t027-s125"></a>

## 33. CLASS ROLE MATRIX

Không class nào phải mạnh mọi mặt.

| Class | Combat | Exploration | Social | Knowledge/Craft | Narrative Access |
|---|---:|---:|---:|---:|---:|
| Lạc Binh | Rất cao | Trung bình | Thấp | Thấp | Trung bình |
| Hộ Vệ | Cao | Trung bình | Trung bình | Thấp | Trung bình |
| Liệp Nhân | Cao | Rất cao | Thấp | Trung bình | Cao |
| Trinh Binh | Trung bình | Rất cao | Trung bình | Trung bình | Rất cao |
| Thủy Binh | Cao | Cao | Trung bình | Trung bình | Cao |
| Dược Nhân | Thấp | Trung bình | Cao | Rất cao | Rất cao |
| Xảo Nhân | Trung bình | Trung bình | Thấp | Rất cao | Cao |
| Vu Chúc | Thấp–TB | Trung bình | Cao | Cao | Rất cao |
| Sứ Giả | Thấp | Trung bình | Rất cao | Trung bình | Rất cao |
| Thủ Lĩnh | Trung bình | Trung bình | Rất cao | Cao | Rất cao |
| Người Giữ Ký Ức | Thấp | Trung bình | Cao | Rất cao | Cực cao |

Combat thấp không có nghĩa class yếu.

Game content phải bảo đảm các trục ngoài combat có giá trị thực.

---

<a id="m10-t027-s126"></a>

## 34. CLASS TECHNIQUE DESIGN RULE

Class Technique ưu tiên:

> mở khả năng mới

hơn:

> tăng 10% stat.

Tốt:

**Không Qua Đây**

mở zone control.

Kém:

`Shield Defense +5%.`

Tốt:

**Hai Câu Chuyện Không Giống Nhau**

mở contradiction analysis.

Kém:

`Investigation +10%.`

---

<a id="m10-t027-s127"></a>

## 35. PASSIVE VÀ CLASS TECHNIQUE KHÁC NHAU

Signature Passive:

đến từ biography.

Unique cho character.

Class Technique:

học được qua con đường Class.

Nhiều người có thể học.

Ví dụ Mân:

Signature Passive:

**Nước Giữ Dấu.**

Sau khi trở thành Liệp Nhân, học:

**Dấu Cố Che.**

Hai hệ bổ trợ nhưng không thay thế nhau.

---

<a id="m10-t027-s128"></a>

## 36. DOCTRINE SYSTEM

Doctrine là lựa chọn phong cách mạnh.

Character có thể biết nhiều Doctrine nhưng chỉ duy trì một số ít cùng lúc.

Ví dụ Lạc Binh:

<a id="m10-t027-s129"></a>

#### Chủ Động Ép Thế

Ưu tiên pressure.

<a id="m10-t027-s130"></a>

#### Giữ Đội Hình

Ưu tiên coordination.

Không thể tối đa cả hai trong cùng một khoảnh khắc.

---

<a id="m10-t027-s131"></a>

## 37. TẠI SAO CẦN DOCTRINE LIMIT

Nếu mọi passive class đều active:

multiclass sẽ thành bài toán stack bonus.

Doctrine Limit buộc player quyết định:

> hôm nay mình chiến đấu như người nào?

Technique vẫn giữ.

Identity vẫn giữ.

Nhưng style active phải có trọng tâm.

---

<a id="m10-t027-s132"></a>

## 38. SPECIALIZATION EMERGENCE

Ví dụ Liệp Nhân Established.

Player có lịch sử:

12 lần tracking người.

5 missing-person resolutions.

Tracking Mastery cao.

Ít dùng cung.

Engine không hỏi:

> Chọn Tầm Tung hay Cung Liệp?

Nó ghi nhận:

> **Tầm Tung đang hình thành.**

Nếu player thay đổi mạnh sau đó:

một specialization thứ hai có thể hình thành về sau.

---

<a id="m10-t027-s133"></a>

## 39. SPECIALIZATION MILESTONE

Để Specialization Established cần một event đặc trưng.

Ví dụ Tầm Tung:

truy tìm thành công một target qua nhiều environment.

Dược Sư:

chế được một treatment mới hoặc cứu một case khó.

Hòa Sứ:

ngăn được conflict mà cả hai bên đều sẵn sàng dùng bạo lực.

Cảm Mạch:

sống sót và hiểu đúng một Mạch anomaly lớn.

---

<a id="m10-t027-s134"></a>

## 40. MASTER SPECIALIZATION

Master Specialization có thể tạo:

Signature Technique.

Không unique như biography passive nhưng rất hiếm.

Ví dụ Master Tầm Tung:

**Người Đi Qua Để Lại Câu Chuyện**

Sau khi thu thập đủ environmental evidence, tái dựng được chuỗi hành động tương đối của target tại một location.

---

<a id="m10-t027-s135"></a>

## 41. MULTICLASS KHÔNG DÙNG CLASS LEVEL

Không có:

`Fighter 5 / Ranger 3`.

Character đơn giản có nhiều Class Identity.

Ví dụ:

**Liệp Nhân — Established**

**Dược Nhân — Recognized**

**Vu Chúc — Candidate**

---

<a id="m10-t027-s136"></a>

## 42. MULTICLASS ENTRY

Class thứ hai vẫn phải đủ Evidence.

Class đầu tiên không cho shortcut tùy tiện.

Nhưng Skill overlap có thể giúp.

Ví dụ Liệp Nhân:

Observation 70.

Herbalism 50.

Survival 70.

Nếu bắt đầu trị thương và học thuốc:

Dược Nhân emergence dễ hơn người không có nền.

---

<a id="m10-t027-s137"></a>

## 43. MULTICLASS KHÔNG BỊ PHẠT XP

Không dùng:

> học class thứ hai thì XP chia đôi.

Natural cost đã có:

Time.

Practice.

Mentor.

Opportunity.

Milestone.

Một đời người không đủ để Master mọi con đường.

---

<a id="m10-t027-s138"></a>

## 44. ANTI-DIPPING RULE

Không cho phép:

> mở Class → lấy passive Tier 1 cực mạnh → bỏ.

Biện pháp:

Class node mạnh cần:

Behaviour Evidence liên tục.

Milestone.

Class Stage.

Không có universal stat buff dễ lấy.

Node đầu tập trung utility phù hợp class.

---

<a id="m10-t027-s139"></a>

## 45. HYBRID TECHNIQUE

Khi hai Class đều Established, một số combination có thể mở **Hybrid Technique**.

Không phải mọi cặp đều có.

---

<a id="m10-t027-s140"></a>

## 46. LIỆP NHÂN + DƯỢC NHÂN

Possible hybrid identity:

**Sơn Dược**

Không nhất thiết trở thành Class riêng.

Hybrid Techniques:

**Tìm Thuốc Theo Dấu Thú**

Animal behaviour giúp tìm medicinal plants.

**Đọc Vết Thương Ngoài Rừng**

Phân biệt injury do animal/environment/human.

---

<a id="m10-t027-s141"></a>

## 47. TRINH BINH + SỨ GIẢ

Hybrid:

**Mật Sứ**

Techniques:

thu thập information kín;

di chuyển giữa faction;

truyền message bí mật;

đánh giá rumor source.

---

<a id="m10-t027-s142"></a>

## 48. LẠC BINH + THỦ LĨNH

Hybrid:

**Trận Lĩnh**

Không đồng nghĩa Lạc Tướng.

Lạc Tướng là social/political title.

Trận Lĩnh là gameplay identity.

Techniques:

formation order;

morale;

battlefield command.

---

<a id="m10-t027-s143"></a>

## 49. DƯỢC NHÂN + VU CHÚC

Hybrid:

**Vu Y**

Character hiểu cả:

medical cause

và

ritual interpretation.

Điểm mạnh lớn nhất:

phân biệt khi nào bệnh thật sự cần medicine và khi nào cultural/spiritual belief đang ảnh hưởng bệnh nhân.

Không biến healing thành magic.

---

<a id="m10-t027-s144"></a>

## 50. XẢO NHÂN + LẠC BINH

Hybrid focus:

**Binh Khí**

Player hiểu cả weapon construction và use.

Có thể:

đánh giá equipment enemy;

tối ưu gear;

thiết kế weapon phù hợp tactics.

---

<a id="m10-t027-s145"></a>

## 51. NGƯỜI GIỮ KÝ ỨC + SỨ GIẢ

Hybrid:

**Người Giữ Lời**

Chuyên:

treaty;

promise;

genealogy;

historical obligation.

Cực mạnh trong political campaign.

---

<a id="m10-t027-s146"></a>

## 52. CLASS CONFLICT

Một số doctrine không tương thích cùng lúc.

Ví dụ:

Trinh Binh:

**Không Để Ai Biết Mình Có Mặt.**

Thủ Lĩnh:

**Mọi Người Phải Biết Ai Đang Chỉ Huy.**

Character có thể biết cả hai.

Nhưng context quyết định doctrine active.

Đây là role tension có chủ ý.

---

<a id="m10-t027-s147"></a>

## 53. PSYCHOLOGY KHÔNG KHÓA CLASS

Character nhút nhát vẫn có thể thành Thủ Lĩnh.

Nhưng character arc sẽ khác.

KHÍ thấp có thể làm con đường khó hơn.

Không phải impossible.

---

<a id="m10-t027-s148"></a>

## 54. CLASS CÓ THỂ THAY ĐỔI CHARACTER

Khi sống lâu trong một Class:

nó có thể tạo Life Events và Psychology Pressure.

Ví dụ Hộ Vệ liên tục mất người mình bảo vệ:

Guilt tăng.

Control Need tăng.

Hoặc character học:

> không thể cứu tất cả.

Class progression vì vậy cũng là character development.

---

<a id="m10-t027-s149"></a>

## 55. CLASS DECAY

Class identity không mất vì vài tháng không dùng.

Nhưng có thể có:

**Rust**

Technique tinh vi giảm hiệu quả tạm thời.

Social Recognition cũng có thể phai nếu character biến mất hàng chục năm.

Knowledge vẫn tồn tại.

---

<a id="m10-t027-s150"></a>

## 56. LEAVING A CLASS PATH

Player không cần nút:

> Respec.

Họ chỉ ngừng sống theo con đường đó.

Techniques đã học không biến mất.

Nhưng progression dừng.

Một identity mới dần mạnh lên.

---

<a id="m10-t027-s151"></a>

## 57. RESPEC KHÔNG PHÙ HỢP GAME

Không cho:

> trả vàng → reset toàn bộ Skill Tree.

Vì Techniques là kinh nghiệm sống.

Player có thể:

học thêm;

thay Doctrine;

đổi phong cách.

Nhưng không xóa quá khứ.

---

<a id="m10-t027-s152"></a>

## 58. CLASS REPUTATION

Một Class có thể được nhìn khác nhau theo region.

Ví dụ Vu Chúc:

Gò Trống:

tôn trọng cao.

Phong Châu:

có vị trí nghi lễ.

Một cộng đồng thực dụng:

có thể nghi ngờ.

Một Class identity vì vậy tác động social context.

---

<a id="m10-t027-s153"></a>

## 59. CLASS NAME CÓ THỂ KHÁC THEO CULTURE

Gameplay archetype nội bộ:

`HunterClass`.

Nhưng NPC có thể gọi:

Liệp Nhân.

Người săn.

Kẻ lần dấu.

Người đi rừng.

Tên Class UI dùng game-canon chuẩn.

Dialogue dùng cultural vocabulary.

---

<a id="m10-t027-s154"></a>

## 60. CLASS KHÔNG PHẢI SOCIAL TITLE

Không được nhầm:

**Hùng Vương**

**Lạc tướng**

**trưởng bến**

**trưởng dòng họ**

với Class.

Đây là Position/Title.

Một Lạc tướng có thể có Class:

Thủ Lĩnh + Lạc Binh.

Hoặc:

Thủ Lĩnh + Sứ Giả.

Hoặc thậm chí:

Sứ Giả + Người Giữ Ký Ức.

---

<a id="m10-t027-s155"></a>

## 61. CLASS KHÔNG PHẢI FACTION

Gia nhập quân đội không tự unlock Lạc Binh.

Gia nhập miếu không tự unlock Vu Chúc.

Joining faction tạo:

opportunity.

Không tạo competence.

---

<a id="m10-t027-s156"></a>

## 62. CLASS KHÔNG PHẢI EQUIPMENT

Cầm cung không biến thành Liệp Nhân.

Cầm khiên không biến thành Hộ Vệ.

Equipment chỉ là công cụ.

Identity đến từ:

cách sử dụng.

---

<a id="m10-t027-s157"></a>

## 63. CLASS QUEST ACCESS

Một số Story Thread sẽ expose khác nhau.

Ví dụ một làng mất người.

Liệp Nhân:

được nhờ truy dấu.

Vu Chúc:

được hỏi liệu có linh thể.

Dược Nhân:

được yêu cầu kiểm tra người sống sót.

Sứ Giả:

được nhờ giải quyết xung đột giữa hai làng đang đổ lỗi cho nhau.

Người Giữ Ký Ức:

nhận ra vụ mất tích giống câu chuyện 30 năm trước.

Cùng một Story Thread.

Năm cửa vào.

---

<a id="m10-t027-s158"></a>

## 64. CLASS SOLUTION ACCESS

Class không chỉ mở quest.

Nó mở giải pháp.

Ví dụ:

Cầu bị phá.

Xảo Nhân:

sửa cầu.

Thủy Binh:

đưa party qua nước.

Liệp Nhân:

tìm đường vòng.

Sứ Giả:

thuyết phục người địa phương cho mượn thuyền.

Thủ Lĩnh:

tổ chức dân sửa cầu.

Không cần mọi puzzle có một lời giải.

---

<a id="m10-t027-s159"></a>

## 65. CLASS & PARTY

Party không bắt buộc:

Tank + DPS + Healer.

Thay vào đó mỗi thành viên mang:

Capability.

Class.

Knowledge.

Relationships.

Một party gồm:

Sứ Giả + Dược Nhân + Xảo Nhân

vẫn phải chơi được.

Họ sẽ tránh hoặc giải quyết encounter khác party:

Lạc Binh + Hộ Vệ + Liệp Nhân.

---

<a id="m10-t027-s160"></a>

## 66. CLASS SYNERGY

Party synergy dựa trên hành động.

Ví dụ:

Liệp Nhân tìm weak route.

Trinh Binh kiểm tra.

Thủ Lĩnh lập kế hoạch.

Lạc Binh thực hiện assault.

Không chỉ:

`+5% damage when ranger present`.

---

<a id="m10-t027-s161"></a>

## 67. CLASS-SPECIFIC FAILURE

Class cũng có failure fantasy.

Lạc Binh:

đánh giá sai battlefield.

Hộ Vệ:

không cứu được người.

Liệp Nhân:

mất dấu.

Dược Nhân:

chẩn đoán sai.

Xảo Nhân:

công trình hỏng.

Vu Chúc:

nghi lễ thất bại.

Sứ Giả:

đàm phán đổ vỡ.

Thủ Lĩnh:

người dưới quyền mất niềm tin.

Người Giữ Ký Ức:

truyền lại một câu chuyện sai.

Failure tạo story.

Không chỉ penalty.

---

<a id="m10-t027-s162"></a>

## 68. CLASS MASTERY TRIAL

Không có generic:

> Level 20 Quest.

Mastery Trial sinh từ world.

Ví dụ Dược Nhân:

một outbreak khó.

Sứ Giả:

một conflict đa faction.

Xảo Nhân:

công trình lớn.

Liệp Nhân:

một hunt/investigation chưa ai giải được.

Trial có thể thất bại.

Không mất Class.

Chỉ chưa được công nhận Master.

---

<a id="m10-t027-s163"></a>

## 69. MASTER RECOGNITION

Có nhiều nguồn.

Peer Recognition.

Community Recognition.

Student Recognition.

Faction Recognition.

Historical Recognition.

Một character có thể là Master thực tế nhưng không nổi tiếng.

Một người khác nổi tiếng hơn competence.

Class system giữ hai thứ riêng.

---

<a id="m10-t027-s164"></a>

## 70. CLASS LEGACY TREE

Sau Mastery, progression đổi bản chất.

Không còn:

> mạnh hơn bản thân.

Mà chuyển thành:

> thay đổi thế giới.

Ví dụ Liệp Nhân:

dạy school.

Xảo Nhân:

chuẩn hóa technique.

Dược Nhân:

truyền phương thuốc.

Sứ Giả:

xây diplomatic network.

Người Giữ Ký Ức:

tạo tradition lưu giữ lịch sử.

---

<a id="m10-t027-s165"></a>

## 71. LEGACY NODE EXAMPLE

<a id="m10-t027-s166"></a>

#### Xảo Nhân — Khuôn Được Truyền Lại

Một production technique player phát triển có thể được workshop khác học.

Settlement production tăng.

Sau khi player chết:

effect vẫn tồn tại.

---

<a id="m10-t027-s167"></a>

## 72. CLASS AND MYTH FORMATION

Character Master/Legacy có thể trở thành myth.

Một Liệp Nhân giết một thú khổng lồ.

50 năm sau:

story exaggeration.

100 năm sau:

> “Người ấy bắn một mũi tên xuyên ba thân cây.”

Game biết:

Objective Event khác Myth.

---

<a id="m10-t027-s168"></a>

## 73. CLASS UI

Character Sheet hiển thị:

<a id="m10-t027-s169"></a>

#### Con đường chính

Liệp Nhân — Established.

<a id="m10-t027-s170"></a>

#### Con đường phụ

Dược Nhân — Emerging.

<a id="m10-t027-s171"></a>

#### Con đường đang hình thành

Vu Chúc — Candidate.

Player có thể click để xem:

Evidence.

Milestones.

Techniques.

Doctrine.

Specialization.

Không hiển thị progress bar kiểu MMO.

---

<a id="m10-t027-s172"></a>

## 74. EVIDENCE UI

Thay vì:

`73% Liệp Nhân`

hiển thị:

**Bạn đã được công nhận là người lần dấu đáng tin cậy.**

**Bạn đã nhiều lần dẫn nhóm qua vùng hoang dã.**

**Khả năng Săn & Bẫy của bạn vẫn còn hạn chế.**

Player hiểu thiếu gì mà không thấy checklist máy móc.

---

<a id="m10-t027-s173"></a>

## 75. SKILL TREE UI

Tree không hoàn toàn giống cây node game truyền thống.

Có thể biểu diễn như:

**Con đường đã sống.**

Node đã học gắn với:

mentor;

event;

milestone.

Ví dụ:

`Dấu Cố Che`

> Học từ Thạch Vũ sau cuộc săn tại Khe Đá Trắng, năm 12.

Skill Tree đồng thời là Chronicle.

---

<a id="m10-t027-s174"></a>

## 76. HIDDEN TECHNIQUES

Không phải toàn bộ Technique hiển thị trước.

Một số chỉ được reveal khi:

gặp mentor;

thấy người khác dùng;

đạt milestone;

khám phá culture mới.

Điều này giữ exploration.

---

<a id="m10-t027-s175"></a>

## 77. UNIQUE TECHNIQUES

Một số Master tạo Technique mới trong simulation.

Không nằm trong fixed tree.

Nếu player học:

character có một ability mà playthrough khác có thể không tồn tại.

Đây là lớp procedural trên authored system.

---

<a id="m10-t027-s176"></a>

## 78. CLASS GENERATION CHO NPC

NPC sử dụng cùng rule.

Không spawn:

`NPC_Ranger_Level_10`.

NPC biography:

hunter 24 năm.

Tracking 78.

Bow 70.

Known Techniques.

Relevant milestones.

Community Recognition.

→ Liệp Nhân Established/Mastered.

Nếu NPC chưa có evidence:

không có Class dù stat cao.

---

<a id="m10-t027-s177"></a>

## 79. NPC CLASS EVOLUTION

Một farmer NPC có thể:

bị chiến tranh cuốn vào militia.

↓

sống sót nhiều trận.

↓

develop spear skill.

↓

bảo vệ settlement.

↓

trở thành Hộ Vệ.

Player gặp lại sau 10 năm:

NPC đã khác.

---

<a id="m10-t027-s178"></a>

## 80. COMPANION CLASS EVOLUTION

Player không trực tiếp “build” companion hoàn toàn.

Companion học từ:

hành động của chính họ;

party role;

mentor;

player influence.

Player có thể khuyến khích.

Không được bấm menu và biến companion thành class vô lý với biography.

---

<a id="m10-t027-s179"></a>

## 81. PLAYER INFLUENCE ON COMPANION

Ví dụ companion đang hình thành:

Dược Nhân.

Player thường bắt họ chiến đấu.

Họ có thể:

phát triển Lạc Binh phụ;

hoặc phản đối;

hoặc rời party nếu trái identity quá mạnh.

Class System nối trực tiếp Psychology.

---

<a id="m10-t027-s180"></a>

## 82. STARTING OLDER CHARACTER

Random character 42 tuổi có thể đã có Class.

Ví dụ:

Profession:

thợ săn.

Life History:

25 năm săn rừng.

Skills cao.

Milestones.

Recognition.

Game có thể khởi đầu:

**Liệp Nhân — Established**

không bắt player xây lại từ zero.

Đây là lý do random age tạo playthrough rất khác.

---

<a id="m10-t027-s181"></a>

## 83. YOUNG CHARACTER

Character 16 tuổi:

có thể không Class.

Ưu điểm:

player định hình gần như toàn bộ tương lai.

Nhược:

ít competence.

Hai kiểu campaign đều hợp lệ.

---

<a id="m10-t027-s182"></a>

## 84. PRIMARY CLASS

Game xác định Primary Class bằng:

Identity Strength.

Current Behaviour.

Recognition.

Không phải player chọn checkbox.

Primary Class chủ yếu dùng:

UI;

NPC expectation;

story exposure.

Không buff riêng chỉ vì Primary.

---

<a id="m10-t027-s183"></a>

## 85. CLASS IDENTITY SHIFT

Ví dụ:

20–30 tuổi:

Lạc Binh Primary.

Sau chiến tranh:

Dược Nhân ngày càng mạnh.

40 tuổi:

Dược Nhân trở thành Primary.

Lạc Binh vẫn Established secondary.

Chronicle ghi lại chuyển đổi cuộc đời.

---

<a id="m10-t027-s184"></a>

## 86. CROSS-GENERATION CLASS

Nếu player có child/apprentice:

họ có thể được Exposure từ Class của player.

Nhưng không auto inherit Class.

Con của Master Hunter có:

access tốt.

Không đảm bảo talent hoặc desire.

---

<a id="m10-t027-s185"></a>

## 87. CLASS ACCESSIBILITY

Mọi Core Class phải có ít nhất:

combat hoặc crisis contribution;

exploration hoặc world interaction;

narrative access;

party utility;

legacy route.

Không class nào được chỉ sống trong một subsystem nhỏ.

---

<a id="m10-t027-s186"></a>

## 88. CLASS POWER PHILOSOPHY

Class strength không cân bằng bằng DPS.

Cân bằng dựa trên:

**Opportunity Value.**

Ví dụ Sứ Giả có thể tránh một trận chiến khiến party khỏi mất tài nguyên.

Dược Nhân cứu NPC quan trọng khỏi chết.

Người Giữ Ký Ức phát hiện sự thật thay đổi faction relationship.

Xảo Nhân xây cầu mở trade route.

Đây đều là power.

---

<a id="m10-t027-s187"></a>

## 89. CLASS OPPORTUNITY BUDGET

Production phải đảm bảo mỗi campaign region có content cho nhiều Class.

Không để:

80% quest ưu tiên combat.

Mỗi major region cần meaningful opportunities cho:

Martial.

Exploration.

Social.

Craft/Knowledge.

Spiritual.

Leadership.

---

<a id="m10-t027-s188"></a>

## 90. CLASS FAILURE PHILOSOPHY

Fail Class action không phải luôn reload.

Ví dụ Sứ Giả thất bại:

hai faction không thỏa thuận.

Story chuyển hướng.

Xảo Nhân thất bại:

cầu chịu được nhưng chỉ tạm thời.

Dược Nhân thất bại:

patient sống nhưng có permanent injury.

Fail-forward giữ narrative.

---

<a id="m10-t027-s189"></a>

## 91. CLASS AND DIFFICULTY

Difficulty không được chỉ tăng enemy HP.

Harder mode có thể:

hạn chế information;

tăng consequence;

giảm preparation time;

làm social opposition thông minh hơn;

tăng resource pressure.

Mọi Class đều bị thử thách.

---

<a id="m10-t027-s190"></a>

## 92. CLASS CAP

Không có hard cap:

`max 2 classes`.

Nhưng Natural Constraint gồm:

thời gian;

tuổi;

practice;

mentor;

milestone;

obligation.

Master 4 Class trong một đời phải gần như huyền thoại.

---

<a id="m10-t027-s191"></a>

## 93. CLASS SATURATION

Nếu character spread quá rộng:

họ trở thành Generalist.

Đây không phải punishment.

Generalist mạnh về:

adaptation;

cross-skill solution;

party flexibility.

Nhưng khó đạt Legacy trong một Class.

---

<a id="m10-t027-s192"></a>

## 94. GENERALIST CÓ THỂ TRỞ THÀNH IDENTITY RIÊNG

Không cần Class “Adventurer”.

Nhưng game có thể gắn Trait:

**Người Biết Nhiều Đường**

Nếu character có nhiều Class Recognized nhưng không Class nào sâu.

Đây là emergent identity, không core class.

---

<a id="m10-t027-s193"></a>

## 95. CLASS TREE CONTENT TARGET

Production target cho mỗi Core Class có thể là:

20–30 authored Techniques.

8–12 Reactions/Mastery nodes.

5–8 Doctrines.

3 Specializations.

3–5 Master Techniques.

3–5 Legacy Nodes.

Không cần GDD viết toàn bộ ngay.

11 Class có thể tạo khoảng:

400–500 authored class nodes khi production hoàn tất.

---

<a id="m10-t027-s194"></a>

## 96. PROCEDURAL TECHNIQUE LAYER

Ngoài authored nodes:

Master characters có thể tạo procedural Technique variants từ:

Base Action

+

Environment

+

Weapon/Profession

+

Personal Passive.

Ví dụ:

Liệp Nhân + Tam Giang + Nước Giữ Dấu

có thể hình thành Technique cá nhân:

**Dấu Trôi Ngược**

Chuyên phân tích dấu vết bị dòng nước làm biến dạng.

---

<a id="m10-t027-s195"></a>

## 97. CLASS QUALITY GATE

Một Class chỉ được giữ trong game nếu trả lời được:

**Gameplay fantasy là gì?**

**Player làm gì khác khi có Class này?**

**Class giải quyết vấn đề ngoài combat thế nào?**

**Class nhìn world khác người khác ở đâu?**

**Class có ít nhất ba specialization thực sự khác nhau không?**

**Class có narrative failure riêng không?**

**Class có legacy path không?**

**Class có overlap quá lớn với Profession không?**

Nếu không trả lời được, merge hoặc bỏ.

---

<a id="m10-t027-s196"></a>

## 98. MULTICLASS QUALITY GATE

Một Hybrid chỉ đáng viết nếu:

Combination tạo gameplay mới.

Không đơn giản:

Class A bonus + Class B bonus.

Ví dụ tốt:

Liệp Nhân + Dược Nhân

→ dùng animal/ecological knowledge cho medicine.

Ví dụ kém:

Lạc Binh + Hộ Vệ

→ Damage +10%, Armor +10%.

---

<a id="m10-t027-s197"></a>

## 99. INTERACTION WITH PROFESSION

Profession tạo:

opportunities;

practice;

network.

Class tạo:

gameplay identity.

Ví dụ một người đóng thuyền:

Profession: Shipwright.

Class: Xảo Nhân.

Secondary Class: Thủy Binh.

Specialization: Công Tạo.

Character này không giống shipwright:

Profession: Shipwright.

Class: Thương? Không có core trade class; có thể Sứ Giả nếu chuyên thương lượng và network.

Do đó cùng nghề vẫn có gameplay khác.

---

<a id="m10-t027-s198"></a>

## 100. INTERACTION WITH PSYCHOLOGY

Class progression có thể đối đầu Psychology.

Ví dụ:

KHÍ thấp.

Fear of Failure cao.

Nhưng player liên tục buộc character lãnh đạo.

Thủ Lĩnh Candidate xuất hiện.

Nếu thành công:

character arc:

Confidence ↑.

Nếu liên tục thất bại:

Shame hoặc avoidance.

Class Emergence không tách khỏi con người.

---

<a id="m10-t027-s199"></a>

## 101. INTERACTION WITH LIFE EVENT ENGINE

Class tạo event eligibility.

Dược Nhân:

PatientSeeksHelp.

Xảo Nhân:

WorkshopRequest.

Sứ Giả:

FactionNegotiation.

Liệp Nhân:

MissingPerson.

Thủ Lĩnh:

LeadershipChallenge.

Người Giữ Ký Ức:

HistoricalDispute.

Class càng được công nhận:

world càng tìm tới character vì capability đó.

---

<a id="m10-t027-s200"></a>

## 102. INTERACTION WITH REPUTATION

NPC không chỉ biết:

“Player reputation 80.”

Họ có thể biết:

> “Người này là Liệp Nhân giỏi.”

hoặc:

> “Nghe nói hắn là Vu Chúc nhưng nghi lễ lần trước thất bại.”

Class Reputation có context.

---

<a id="m10-t027-s201"></a>

## 103. INTERACTION WITH EQUIPMENT

Class không khóa equipment tuyệt đối.

Dược Nhân vẫn có thể dùng spear.

Liệp Nhân vẫn dùng axe.

Nhưng Technique có requirement.

Ví dụ:

`Giữ Mũi`

cần weapon có reach phù hợp.

Không phải:

> You cannot equip this item because wrong class.

---

<a id="m10-t027-s202"></a>

## 104. INTERACTION WITH COMBAT

Combat Class mở tactical vocabulary.

Không phải raw stats.

Lạc Binh:

formation.

Hộ Vệ:

protection.

Liệp Nhân:

preparation.

Trinh Binh:

information.

Thủy Binh:

terrain/water.

Các Class ngoài combat vẫn đóng góp qua:

medicine;

leadership;

craft;

preparation;

morale.

---

<a id="m10-t027-s203"></a>

## 105. INTERACTION WITH DUNGEON

Dungeon solution phụ thuộc Class.

Ví dụ mộ thuyền ngập nước:

Thủy Binh:

tiếp cận.

Xảo Nhân:

xử lý structure.

Người Giữ Ký Ức:

đọc funeral context.

Vu Chúc:

hiểu ritual.

Liệp Nhân:

nhận dấu người đã vào trước.

Không có một “Dungeon Class”.

---

<a id="m10-t027-s204"></a>

## 106. CHARACTER EXAMPLE — VẠC MÂN

Starting character:

17 tuổi.

Tam Giang.

Không Class.

Skills:

Bơi tốt.

Quan sát tốt.

Điều thuyền khá.

Combat yếu.

---

<a id="m10-t027-s205"></a>

### PLAYTHROUGH A

Player:

thường xuyên tìm người mất tích;

truy dấu;

săn;

học cung.

↓

Liệp Nhân Candidate.

↓

theo dấu một child mất tích qua hai ngày.

↓

Recognition.

↓

**Liệp Nhân — Recognized.**

↓

chuyên tìm người.

↓

**Tầm Tung.**

---

<a id="m10-t027-s206"></a>

## 107. VẠC MÂN — PLAYTHROUGH B

Player:

tham gia vận tải sông;

học spear;

cứu người;

chiến đấu trên thuyền.

↓

**Thủy Binh.**

↓

đặc biệt tập trung rescue.

↓

**Cứu Thủy.**

Same character.

Different life.

---

<a id="m10-t027-s207"></a>

## 108. VẠC MÂN — PLAYTHROUGH C

Sau khi người thân bệnh:

học Herbalism.

Làm apprentice healer.

Sử dụng Observation để diagnosis.

↓

**Dược Nhân.**

Sau đó vẫn giữ strong River Knowledge.

Có thể phát triển hybrid:

Dược Nhân + Thủy knowledge.

---

<a id="m10-t027-s208"></a>

## 109. VẠC MÂN — PLAYTHROUGH D

Gặp một Mạch anomaly.

Bắt đầu học ritual.

Signature Passive Nước Giữ Dấu giúp nhận ra khác biệt giữa dòng nước tự nhiên và abnormal.

↓

Vu Chúc Candidate.

↓

Nếu Cảm Mạch thức tỉnh:

có thể theo **Cảm Mạch**.

Character không bị khóa bởi starting background.

Nhưng mọi con đường đều phải được sống thật.

---

<a id="m10-t027-s209"></a>

## 110. ENDGAME CHARACTER EXAMPLE

Mân 52 tuổi:

**Liệp Nhân — Mastered**

Specialization:

Tầm Tung.

**Dược Nhân — Established**

Specialization:

Dược Sư.

**Thủy Binh — Recognized**

Profession:

River Guide.

Current Profession:

Teacher / Search Specialist.

Signature Passive:

Nước Giữ Dấu.

Created Technique:

Dấu Trôi Ngược.

Students:

4.

Historical Reputation:

người tìm được 27 người mất tích trong Đại Thủy.

Đây là character build.

Không cần:

Level 50 Ranger/Cleric.

---

<a id="m10-t027-s210"></a>

## 111. DEFERRED TO PRODUCTION

GDD chưa cần khóa:

con số Evidence threshold;

exact Skill requirement;

toàn bộ Technique;

exact Doctrine slot count;

animation;

damage;

cooldown;

resource cost;

toàn bộ Hybrid Techniques;

toàn bộ Mastery Trial.

Production sẽ balance sau.

---

<a id="m10-t027-s211"></a>

## 112. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

Class không được chọn ở Character Generation.

Class xuất hiện từ gameplay.

Class không dùng XP Level truyền thống.

Profession và Class là hai hệ khác nhau.

Attributes không tăng tự động từ Class.

Class progression dựa vào Evidence và Milestone.

Specialization cũng emerge từ hành vi.

Player chọn Technique eligible, không chọn destiny trước khi sống.

Không instant respec.

Multiclass không có hard Class Level.

Không có XP penalty cho multiclass.

Thời gian và cuộc đời là natural progression constraint.

Skill overlap hỗ trợ cross-class.

Anti-dipping bằng Milestone/Behaviour requirement.

Hybrid Technique phải tạo gameplay mới.

Class không khóa equipment cứng.

Class ngoài combat phải có power thật.

NPC và player dùng cùng Class System.

Class có thể thay đổi Primary identity theo cuộc đời.

Older random character có thể bắt đầu với Class đã hình thành.

Companion tự hình thành Class theo biography và hành vi.

Master progression chuyển từ personal power sang Legacy.

Class có thể tồn tại trong lịch sử sau khi character chết.

---

<a id="m10-t027-s212"></a>

## 113. CORE DESIGN STATEMENT

Hệ progression cuối cùng hình thành chuỗi:

**Sinh ra**

→

**Có một quá khứ**

→

**Học kỹ năng**

→

**Làm nghề**

→

**Player hành động**

→

**Hình thành phong cách**

→

**Thế giới nhận ra phong cách đó**

→

**Class xuất hiện**

→

**Specialization hình thành**

→

**Mastery**

→

**Truyền nghề / ảnh hưởng người khác**

→

**Legacy**

→

**Lịch sử**

→

**Truyền thuyết.**

Đây chính là vòng cung progression lớn nhất của character.

Một character không tiến từ:

> Level 1 → Level 50.

Họ tiến từ:

> **một con người vô danh → một con người đã sống một cuộc đời.**

Với module này, phần **character progression cốt lõi** đã khá hoàn chỉnh: Random Origin → Psychology → Profession → Attributes/Skills → Class → Specialization → Multiclass → Legacy. Phần cần thiết nhất tiếp theo là biến những con người này thành một nhóm chơi thực sự, trước khi thiết kế combat chi tiết.

Bạn muốn tôi đi tiếp **Party & Companion** (quan hệ, tuyển người, loyalty, xung đột nội bộ) hay vào thẳng **Combat** (turn, action economy, vị trí, vũ khí, thương tích)?

<!-- SOURCE T027 END -->

