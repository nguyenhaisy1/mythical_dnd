# Module 05 — Tâm lý, cá tính và Signature Passive

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T013](../../source/conversation.md#t013)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Core/Learned/State và contradiction là cơ sở hành vi; NPC chỉ đọc perceived state. Không cho personality random độc lập với toàn bộ biography.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Life events, needs, values, memories, current stress và perceptions. |
| Đầu ra / bên sở hữu | Psychology profile và action preferences; Characters/Social theo owner contract. |
| Đọc cùng | 04, 06, 07, 16, 27. |
| Cổng nghiệm thu | Hai actor với cùng observation có thể diễn giải khác nhưng không biết hidden facts. |
| Việc chưa được chứng minh | Weights, stability và độ khác biệt passive cần simulation/playtest. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T013 BEGIN -->

<a id="m05-t013-s001"></a>

## WORLD BIBLE 04 — CHARACTER PSYCHOLOGY & PERSONALITY ENGINE

Từ bước này, ta có thể bắt đầu biến dân số của thế giới thành **những con người có cái tôi riêng**.

Tên nhân vật bên dưới đều là **tên game-canon tạm thời**, chưa phải bộ naming convention chính thức.

---

<a id="m05-t013-s002"></a>

## I. Một nguyên tắc cần khóa

Không sinh nhân vật theo kiểu:

> Brave + Kind + Loyal + Fisherman.

Thay vào đó:

> **Temperament + Family Pressure + Life Events + Interpretation + Relationships + Current Situation = Personality hiện tại**

Do đó hai anh em sinh đôi trải qua cùng một trận lũ vẫn có thể trở thành hai người hoàn toàn khác nhau.

Ví dụ:

Cha chết vì lũ.

Người thứ nhất nghĩ:

> “Ta phải mạnh hơn để bảo vệ người khác.”

Người thứ hai:

> “Không ai thật sự bảo vệ được ai.”

Cùng một event.

Hai con người.

---

<a id="m05-t013-s003"></a>

## II. Character Psychology gồm ba tầng

<a id="m05-t013-s004"></a>

### CORE

Phần tương đối ổn định.

Bao gồm:

temperament,

giá trị cốt lõi,

nhu cầu tâm lý,

moral boundary,

cách nhìn thế giới.

---

<a id="m05-t013-s005"></a>

### LEARNED

Những thứ cuộc đời dạy character.

Ví dụ:

không tin người quyền lực;

sợ nước;

tôn trọng thợ thủ công;

ghét kẻ thất hứa;

tin rằng tiền quan trọng hơn danh dự.

Phần này có thể thay đổi mạnh.

---

<a id="m05-t013-s006"></a>

### STATE

Tâm trạng hiện tại.

Ví dụ:

đói,

mệt,

đang yêu,

đang đau buồn,

hoảng loạn,

say,

đang giận.

**State không phải Personality.**

Một người hiền có thể giết người trong panic.

Một người lạnh lùng vẫn có thể cứu trẻ em.

---

<a id="m05-t013-s007"></a>

## III. 20 trục Character Psychology

Mỗi character có giá trị nội bộ từ 0–100, nhưng player không nhất thiết nhìn thấy con số.

| # | Trục | 0 | 100 |
|---|---|---|---|
| 1 | Sociability | khép kín | hướng ngoại |
| 2 | Trust | nghi ngờ | dễ tin |
| 3 | Emotionality | lạnh | cảm xúc mạnh |
| 4 | Impulsiveness | cân nhắc | hành động ngay |
| 5 | Risk Tolerance | tránh rủi ro | liều lĩnh |
| 6 | Persistence | dễ bỏ | cực kỳ lì |
| 7 | Adaptability | bám thói quen | thích nghi nhanh |
| 8 | Compassion | đặt mình trước | quan tâm người khác |
| 9 | Pride | khiêm nhường | tự tôn mạnh |
| 10 | Ambition | bằng lòng | luôn muốn tiến lên |
| 11 | Authority | phản quyền lực | tin thứ bậc |
| 12 | Tradition | thích thay đổi | giữ truyền thống |
| 13 | Spirituality | duy vật | tin linh giới |
| 14 | Forgiveness | nhớ thù | dễ tha thứ |
| 15 | Honesty | thực dụng với sự thật | cực trọng sự thật |
| 16 | Attachment | dễ buông | gắn bó sâu |
| 17 | Control Need | thuận theo | cần kiểm soát |
| 18 | Recognition Need | không cần công nhận | rất cần được nhìn nhận |
| 19 | Empathy Accuracy | khó đọc người | rất hiểu cảm xúc |
| 20 | Self-Preservation | dễ hy sinh | bảo toàn bản thân |

Nhưng **20 con số không phải personality**.

Chúng chỉ là substrate để behaviour engine sử dụng.

---

<a id="m05-t013-s008"></a>

## IV. Value System

Mỗi character có 3–5 giá trị quan trọng nhất.

Các value có thể gồm:

**Gia đình**

**Tự do**

**Danh dự**

**Lời hứa**

**Cộng đồng**

**Sự thật**

**Trật tự**

**Truyền thống**

**Tín ngưỡng**

**Tri thức**

**Quyền lực**

**Của cải**

**An toàn**

**Tình yêu**

**Danh tiếng**

**Công bằng**

**Nghề nghiệp**

**Quê hương**

Ví dụ:

Mộc:

1. Gia đình
2. Tự chủ
3. Sự thật
4. Nghề nghiệp

Nếu quest buộc chọn:

**cứu gia đình**

hay

**giữ lời hứa**

Mộc sẽ thiên về gia đình.

Một NPC khác lại có thể ngược lại.

---

<a id="m05-t013-s009"></a>

## V. Need System

Value là điều character tin quan trọng.

Need là thứ character **thiếu**.

Ví dụ:

Mộc coi gia đình quan trọng.

Nhưng nhu cầu sâu nhất lại là:

> **được chứng minh mình không giống cha.**

Các nhu cầu phổ biến:

được yêu;

được công nhận;

được an toàn;

được thuộc về;

được tự do;

được kiểm soát;

được tôn trọng;

được tha thứ;

được trả thù;

được hiểu;

được để lại dấu ấn.

Need thường không được character nói thẳng.

---

<a id="m05-t013-s010"></a>

## VI. Desire khác Need

Một character có thể nói:

> “Ta muốn mua một chiếc thuyền.”

Đó là **Desire**.

Nhưng Need thật sự:

> “Ta muốn chứng minh rằng gia đình mình không phải những kẻ thất bại.”

Nếu player chỉ giải quyết Desire:

mua thuyền.

Quest chưa chắc kết thúc emotional arc.

Nếu player giúp character nhận ra Need:

arc mới hoàn chỉnh.

---

<a id="m05-t013-s011"></a>

## VII. Fear cũng chia ba lớp

<a id="m05-t013-s012"></a>

#### Practical Fear

Rắn.

Nước.

Chiến tranh.

Đói.

---

<a id="m05-t013-s013"></a>

#### Social Fear

Mất mặt.

Bị ruồng bỏ.

Bị coi là vô dụng.

---

<a id="m05-t013-s014"></a>

#### Existential Fear

Không để lại gì.

Gia đình biến mất.

Thần linh không tồn tại.

Hoặc đáng sợ hơn:

> thần linh thực sự tồn tại.

Character thường chỉ ý thức rõ lớp đầu.

---

<a id="m05-t013-s015"></a>

## VIII. Moral Boundary

Mỗi character cần ít nhất một **ranh giới đạo đức**.

Ví dụ:

> Có thể trộm nhưng không giết.

> Có thể giết kẻ thù nhưng không giết người đã đầu hàng.

> Không bao giờ phản bội gia đình.

> Không bao giờ phá mộ.

Nếu player ép character vượt boundary:

đây là event tâm lý lớn.

Character có thể:

rời party;

mất passive;

đổi personality;

phát triển trauma;

hoặc trở nên chai lì.

---

<a id="m05-t013-s016"></a>

## IX. Conflict Style

Không phải ai cũng xử lý conflict bằng combat.

Có thể có:

**Confront**

đối đầu trực tiếp.

**Avoid**

tránh.

**Negotiate**

thương lượng.

**Manipulate**

điều khiển.

**Appease**

nhượng bộ.

**Withdraw**

cắt quan hệ.

**Endure**

chịu đựng.

**Explode**

dồn nén rồi bùng nổ.

Conflict style cực kỳ quan trọng cho dialogue.

---

<a id="m05-t013-s017"></a>

## X. Lie Style

Nhân vật nói dối cũng khác nhau.

Một số không thể nói dối trực tiếp nhưng:

> bỏ bớt sự thật.

Một số:

> đổi chủ đề.

Một số:

> kể sự thật nhưng khiến người nghe hiểu sai.

Một số:

> nói dối hoàn toàn bình thản.

Skill Insight sau này có thể học pattern riêng của NPC.

---

<a id="m05-t013-s018"></a>

## XI. Stress Response

Mỗi character có hai response:

<a id="m05-t013-s019"></a>

#### Primary

Phản ứng thường gặp.

<a id="m05-t013-s020"></a>

#### Breaking Point

Phản ứng khi vượt quá giới hạn.

Ví dụ:

Mộc bình thường:

> kiểm soát.

Nhưng breaking point:

> đóng băng.

Một chiến binh hung hăng bình thường:

> đối đầu.

Breaking point:

> bỏ chạy.

Đây mới tạo ra bất ngờ.

---

<a id="m05-t013-s021"></a>

## XII. Shame và Guilt phải khác nhau

**Guilt**

> “Ta đã làm điều xấu.”

**Shame**

> “Ta là người xấu.”

Hai thứ tạo character arc hoàn toàn khác.

Guilt có thể sửa bằng chuộc lỗi.

Shame thường cần:

identity reconstruction.

---

<a id="m05-t013-s022"></a>

## XIII. Identity Anchor

Mỗi character phải có ít nhất hai câu trả lời cho:

> “Ta là ai?”

Ví dụ:

Mộc:

**Người Bến Vạc.**

**Con của Tòng.**

Nếu Bến Vạc bị phá:

một anchor mất.

Nếu cha chết:

anchor khác mất.

Character có thể rơi vào **Identity Crisis**.

Đây là nguồn progression rất tốt.

---

<a id="m05-t013-s023"></a>

## XIV. Contradiction Engine

Mỗi Tier A trở lên bắt buộc có tối thiểu **một contradiction**.

Công thức thường là:

> **Value A + Behaviour B mâu thuẫn với A**

Ví dụ:

Rất trọng sự thật

nhưng

nói dối để bảo vệ em.

Hoặc:

Không tin thần linh

nhưng

mỗi năm vẫn cúng cha.

Hoặc:

Ghét chiến tranh

nhưng

cảm thấy mình chỉ có giá trị khi chiến đấu.

Contradiction không phải lỗi generation.

Nó chính là:

> **con người.**

---

<a id="m05-t013-s024"></a>

## XV. Personality Evolution

Personality không đóng băng.

Ví dụ character Trust = 20.

Player giữ lời với họ nhiều lần.

Trust không tự tăng thành 80 ngay.

Thay vào đó character có thể hình thành:

> **Exception Relationship**

“Tôi không tin người đời. Nhưng tôi tin anh.”

Đây thực tế hơn.

Chỉ những event lớn mới thay đổi core psychology.

---

<a id="m05-t013-s025"></a>

## XVI. Dialogue Engine sử dụng Psychology

Cùng một thông tin:

> “Quân địch đang tới.”

Character A:

> “Có bao nhiêu người?”

Control cao.

Character B:

> “Mẹ ta vẫn còn ngoài đồng!”

Family-first.

Character C:

> “Ta đã bảo chuyện này sẽ xảy ra.”

Pride.

Character D:

> “Thần đã bỏ chúng ta.”

Spiritual interpretation.

Character E:

> “Ai còn chưa biết?”

Information-oriented.

Nhờ vậy AI dialogue không cần random personality adjective.

---

<a id="m05-t013-s026"></a>

## XVII. Signature Passive Generator hoàn chỉnh

Bây giờ passive có thể lấy dữ liệu từ sáu nguồn:

**Origin**

+

**Expertise**

+

**Formative Event**

+

**Core Value**

+

**Contradiction**

+

**Psychological Cost**

=

**Signature Passive**

Ví dụ:

Thợ săn +

mất em trong rừng +

rất bảo vệ người khác +

ám ảnh không để ai mất tích

→

<a id="m05-t013-s027"></a>

#### Không Bỏ Ai Trong Rừng

Nếu một party member tách khỏi nhóm trong wilderness, character luôn biết **hướng tương đối cuối cùng** của người đó.

Nhưng:

Nếu party bỏ lại một thành viên bị thương, character chịu penalty tinh thần rất lớn.

Passive vừa có **power** vừa có **cost**.

---

<a id="m05-t013-s028"></a>

## XVIII. Passive được chia 8 nhóm

Không để hệ thống sinh toàn combat buff.

Ta cần:

**Perception Passive**

**Social Passive**

**Exploration Passive**

**Craft Passive**

**Combat Passive**

**Knowledge Passive**

**Survival Passive**

**Narrative Passive**

Nhóm cuối đặc biệt quan trọng.

Ví dụ:

<a id="m05-t013-s029"></a>

#### Người Không Quên Món Nợ

Character luôn nhớ mọi favor lớn đã nhận và có event riêng khi người ban ơn gặp nguy hiểm.

Không cộng một stat nào.

Nhưng tạo story.

---

<a id="m05-t013-s030"></a>

## XIX. Character Fingerprint chống trùng

Mỗi character có fingerprint:

`Origin + FamilyRole + TopValues + Need + Fear + MoralBoundary + ConflictStyle + Contradiction + SignaturePassive + CurrentGoal`

Hai Tier A character không được có fingerprint similarity quá cao.

Ví dụ:

hai character đều:

thợ săn + mất cha + bảo vệ gia đình

vẫn được phép tồn tại.

Nhưng nếu:

Need,

Fear,

Contradiction,

Goal,

Passive

cũng giống nhau thì generator buộc reroll.

Đây là cách thực tế để đạt mục tiêu **1.000 nhân vật không clone nhau**.

---

<a id="m05-t013-s031"></a>

## XX. Batch kiểm thử 40 character

Bây giờ lấy 20 family trước và instantiate **2 người mỗi family**.

---

<a id="m05-t013-s032"></a>

### F01 — NHÀ VẠC TÒNG

<a id="m05-t013-s033"></a>

#### 01. VẠC TÒNG — 58

**Role:** người đưa đò kỳ cựu.

**Core Value:** gia đình → trách nhiệm → trật tự.

**Need:** được các con tin rằng những lựa chọn khó khăn ông từng làm là cần thiết.

**Fear:** sự thật về cái chết của cha mình bị lộ.

**Moral Boundary:** không kéo người vô tội vào tranh chấp của mình.

**Conflict Style:** chịu đựng rất lâu rồi mới đối đầu.

**Contradiction:** luôn dạy con phải nói thật nhưng giữ bí mật lớn nhất của gia đình hơn ba mươi năm.

**Current Goal:** giữ gia đình không tan rã.

<a id="m05-t013-s034"></a>

#### Passive — **Người Đã Qua Mọi Bến**

Khi tới một settlement ven sông lần đầu, Tòng có khả năng tìm được một contact nghề nghiệp hoặc nơi trú đáng tin mà party bình thường không thấy.

**Cost:** rất khó từ chối giúp những người cùng nghề gặp nạn.

---

<a id="m05-t013-s035"></a>

#### 02. VẠC MỘC — 31

**Role:** người dẫn thuyền.

**Value:** gia đình, năng lực cá nhân, sự thật.

**Need:** chứng minh cuộc sống của mình không bị thần linh quyết định.

**Fear:** bất lực khi người thân gặp nạn.

**Contradiction:** công khai không tin thần nhưng vẫn cúng cha đều đặn.

**Stress:** kiểm soát → khi vượt ngưỡng sẽ đóng băng.

<a id="m05-t013-s036"></a>

#### Passive — **Nước Không Nói Dối**

Quan sát đủ lâu một vùng nước giúp Mộc nhận biết bất thường **do nguyên nhân vật lý**.

Không phát hiện hiện tượng thuần linh giới.

---

<a id="m05-t013-s037"></a>

## F02 — NHÀ LẠC CẢNH

<a id="m05-t013-s038"></a>

#### 03. LẠC DUY — 34

Người con cả được chọn kế nghiệp.

Giỏi giao tiếp nhưng biết mình không tài bằng em.

**Need:** chứng minh lựa chọn của cha không sai.

**Fear:** bị người khác phát hiện mình chỉ có địa vị vì sinh trước.

**Contradiction:** bảo vệ trật tự đang khiến chính mình khổ sở.

<a id="m05-t013-s039"></a>

#### Passive — **Đứng Đúng Chỗ**

Trong nghi lễ hoặc cuộc gặp có thứ bậc, Duy nhanh chóng nhận ra ai đang có quyền lực thực sự, kể cả người đó không phải người có địa vị cao nhất.

---

<a id="m05-t013-s040"></a>

#### 04. LẠC NGHI — 29

Người con thứ.

Thông minh, kiên nhẫn, có tư tưởng cải cách.

**Value:** năng lực > huyết thống.

**Fear:** sống cả đời dưới cái bóng của anh.

**Moral Boundary:** không làm hại anh mình.

**Contradiction:** đang bí mật phá hệ thống đã mang lại đặc quyền cho chính mình.

<a id="m05-t013-s041"></a>

#### Passive — **Người Trong Nhà**

Nghi có thể phát hiện contradiction giữa lời nói công khai và lợi ích thật của thành viên giới quyền lực dễ hơn người khác.

---

<a id="m05-t013-s042"></a>

## F03 — NHÀ NẬU

<a id="m05-t013-s043"></a>

#### 05. NẬU THƯ — 27

Người con gái dự kiến kế nghiệp ruộng đất.

**Value:** hiệu quả, cộng đồng.

**Need:** được công nhận như một người quản lý chứ không chỉ người thừa kế.

**Goal:** đào kênh mới.

**Fear:** trở thành giống cha.

<a id="m05-t013-s044"></a>

#### Passive — **Nước Phải Đi Đâu Đó**

Khi quan sát địa hình canh tác, Thư có thể phát hiện một phương án điều nước mà NPC thông thường không thấy.

---

<a id="m05-t013-s045"></a>

#### 06. NẬU KHÔN — 55

Cha Thư.

Không phải kẻ tham lam đơn giản.

Ông thực sự tin rằng mất quyền kiểm soát nước sẽ dẫn tới hỗn loạn.

**Contradiction:** muốn con mạnh nhưng từ chối trao quyền cho con.

<a id="m05-t013-s046"></a>

#### Passive — **Ruộng Nhớ Chủ**

Khôn biết lịch sử sở hữu và tranh chấp của hầu hết đất quanh Làng Dâu, mở dialogue đặc biệt về quyền sử dụng đất.

---

<a id="m05-t013-s047"></a>

## F04 — NHÀ CẤN LỬA

<a id="m05-t013-s048"></a>

#### 07. CẤN HẠ — 26

Con cả, tay run.

**Need:** không làm cha thất vọng.

**Fear:** em trai thay mình.

**Value:** gia đình > bản thân.

**Contradiction:** muốn em thành công nhưng mỗi thành công của em đều khiến mình đau.

<a id="m05-t013-s049"></a>

#### Passive — **Thấy Lỗi Trước Khi Đúc**

Hạ không có đôi tay tốt nhất nhưng cực giỏi đọc thiết kế; trước một craft khó, có thể phát hiện một nguy cơ thất bại.

---

<a id="m05-t013-s050"></a>

#### 08. CẤN SON — 22

Con thứ, thiên tài nghề đúc.

**Need:** được tự tạo tên mình.

**Fear:** thành công của mình phá gia đình.

<a id="m05-t013-s051"></a>

#### Passive — **Kim Loại Nhớ Lửa**

Khi kiểm tra một đồ đồng đã qua sử dụng, Son có thể suy đoán sơ bộ cách nó từng được nung/sửa đổi.

---

<a id="m05-t013-s052"></a>

## F05 — NHÀ SƯƠNG

<a id="m05-t013-s053"></a>

#### 09. SƯƠNG ĐIỀN — 43

Thợ săn bị mất thị lực.

Không bi quan.

**Value:** tự chủ.

**Fear:** bị đối xử như gánh nặng.

<a id="m05-t013-s054"></a>

#### Passive — **Không Cần Thấy Mặt**

Có thể nhận ra character từng quen qua voice, gait hoặc nhịp thở trong bóng tối/sương dày.

---

<a id="m05-t013-s055"></a>

#### 10. SƯƠNG NÁ — 19

Con gái Điền.

Bảo vệ cha thái quá.

**Need:** cảm thấy mình hữu dụng.

**Contradiction:** yêu cha nhưng chính sự bảo vệ của cô khiến ông cảm thấy bất lực.

<a id="m05-t013-s056"></a>

#### Passive — **Người Đi Trước Trong Sương**

Khi dẫn party trong visibility thấp, lần ambush môi trường đầu tiên mỗi expedition có khả năng bị phát hiện sớm.

---

<a id="m05-t013-s057"></a>

## F06 — NHÀ KHE

<a id="m05-t013-s058"></a>

#### 11. KHE DỰ — 47

Courier cũ phục vụ một chief.

**Value:** trách nhiệm.

**Fear:** thế hệ trẻ coi lời thề chẳng còn ý nghĩa.

<a id="m05-t013-s059"></a>

#### Passive — **Một Lời Đã Mang Qua Núi**

Khi đã nhận nhiệm vụ chuyển message/item, Dự chịu fatigue chậm hơn cho tới khi hoàn thành hoặc tự từ bỏ lời nhận.

---

<a id="m05-t013-s060"></a>

#### 12. KHE HÀN — 24

Con trai.

Đưa thư cho faction đối lập.

Không phải phản bội.

Hàn tin loyalty phải dành cho dân chứ không phải chief.

<a id="m05-t013-s061"></a>

#### Passive — **Đường Không Có Phe**

Khi di chuyển giữa hai faction thù địch với tư cách messenger, Hàn có khả năng mở lựa chọn neutral passage đặc biệt.

---

<a id="m05-t013-s062"></a>

## F07 — NHÀ ĐÀN

<a id="m05-t013-s063"></a>

#### 13. ĐÀN NHƯ — 25

Con gái lớn.

Muốn rời Gò Trống.

**Value:** tự do.

**Fear:** một ngày tỉnh dậy và trở thành mẹ.

**Contradiction:** muốn bỏ truyền thống nhưng hiểu nó sâu hơn bất kỳ người trẻ nào.

<a id="m05-t013-s064"></a>

#### Passive — **Nghe Khoảng Trống Giữa Hai Nhịp**

Như có khả năng phát hiện sự thay đổi nhỏ trong nhịp nghi lễ, trống báo hiệu hoặc pattern âm thanh.

---

<a id="m05-t013-s065"></a>

#### 14. ĐÀN MỆ — 51

Mẹ Như.

Không độc đoán.

Bà từng muốn bỏ đi giống con nhưng ở lại sau khi chị gái chết.

<a id="m05-t013-s066"></a>

#### Passive — **Người Ở Lại**

Khi settlement gặp crisis, Mệ có khả năng giữ morale của household tốt hơn nhưng gần như không thể tự nguyện di cư.

---

<a id="m05-t013-s067"></a>

## F08 — NHÀ VÂN

<a id="m05-t013-s068"></a>

#### 15. VÂN HẢI — 21

Sợ biển.

Lại có trí nhớ tuyệt vời về hải trình vì cha dạy từ nhỏ.

**Contradiction:** biết biển hơn phần lớn thủy thủ nhưng không muốn bước lên thuyền.

<a id="m05-t013-s069"></a>

#### Passive — **Hải Trình Trong Đầu**

Sau khi được mô tả một tuyến biển từng học, Hải có thể nhớ lại landmark và nguy cơ dù nhiều năm không đi.

---

<a id="m05-t013-s070"></a>

#### 16. VÂN GIANG — 24

Chị Hải.

Muốn đi xa.

**Need:** vượt cái bóng người cha đã chết.

<a id="m05-t013-s071"></a>

#### Passive — **Biển Đã Trả Ta Lại**

Một lần duy nhất trong đời character, một catastrophe đường biển vốn gây tử vong có thể trở thành trạng thái nguy kịch nhưng sống sót.

Sau khi kích hoạt, passive mất vĩnh viễn.

---

<a id="m05-t013-s072"></a>

## F09 — NHÀ CÒ

<a id="m05-t013-s073"></a>

#### 17. CÒ VỰC — 62

Người giữ mộ.

Ít nói nhưng rất nhớ người.

**Value:** tên tuổi người chết.

**Moral Boundary:** không bao giờ cố ý để một người bị chôn vô danh nếu có thể xác định họ.

<a id="m05-t013-s074"></a>

#### Passive — **Người Chết Cũng Có Tên**

Khi khám nghiệm một thi thể, Vực nhận được thêm một clue về identity nếu còn bất kỳ dấu vết nào có thể suy ra.

---

<a id="m05-t013-s075"></a>

#### 18. CÒ LIÊN — 30

Cháu Vực.

Không sợ người chết nhưng sợ phải sống cả đời tại Gò Cò.

<a id="m05-t013-s076"></a>

#### Passive — **Nhớ Ai Đã Khóc**

Sau funeral hoặc death event, Liên có thể nhớ chính xác những NPC nào có phản ứng cảm xúc bất thường.

Rất mạnh trong investigation.

---

<a id="m05-t013-s077"></a>

## F10 — NHÀ MỘC

<a id="m05-t013-s078"></a>

#### 19. MỘC CĂN — 58

Cha.

Tin luật lấy cây là thứ giữ rừng tồn tại.

**Need:** truyền lại nghề trước khi chết.

<a id="m05-t013-s079"></a>

#### Passive — **Cây Nào Dùng Việc Nấy**

Khi lựa chọn gỗ cho craft, Căn có thể thay nguyên liệu bằng loại phù hợp khác mà ít giảm chất lượng.

---

<a id="m05-t013-s080"></a>

#### 20. MỘC YÊN — 33

Con gái.

Thực dụng hơn cha.

Muốn bán cây cổ để cứu nhà khỏi nghèo.

Nhưng lại là người đầu tiên phát hiện cây đang bệnh.

<a id="m05-t013-s081"></a>

#### Passive — **Nghe Gỗ Bệnh**

Có thể nhận biết degradation, sâu, mục hoặc stress trên cây/gỗ trước người khác.

---

<a id="m05-t013-s082"></a>

## F11 — NHÀ BÌNH

<a id="m05-t013-s083"></a>

#### 21. BÌNH KIÊN — 29

Sinh Bắc Lộ, chạy nạn.

**Identity:** vẫn coi Bắc Lộ là quê.

**Goal:** quay về.

**Fear:** trở về rồi nhận ra quê không còn.

<a id="m05-t013-s084"></a>

#### Passive — **Đường Về Không Quên**

Kiên giữ khả năng định hướng tốt hơn trên những con đường đã đi thời thơ ấu, kể cả landscape thay đổi.

---

<a id="m05-t013-s085"></a>

#### 22. BÌNH THẢO — 17

Sinh tại Đồng Sương.

Không hiểu vì sao cha cứ gọi nơi khác là quê.

**Value:** cộng đồng hiện tại.

<a id="m05-t013-s086"></a>

#### Passive — **Người Của Nơi Mới**

Khi settlement có nhiều người di cư, Thảo dễ tạo trust với các nhóm khác nguồn gốc hơn NPC thông thường.

---

<a id="m05-t013-s087"></a>

## F12 — NHÀ TRIỀU

<a id="m05-t013-s088"></a>

#### 23. TRIỀU KHÊ — 35

Người con cả Vạn Nước.

Yêu người Bãi Lạc.

<a id="m05-t013-s089"></a>

#### Contradiction

Tin rằng đất giữ chân con người, nhưng đang nghĩ tới chuyện mua ruộng.

<a id="m05-t013-s090"></a>

#### Passive — **Nhà Có Thể Trôi**

Party có thể nhận chất lượng rest tốt hơn trên phương tiện đường sông nếu Khê chuẩn bị chỗ nghỉ.

---

<a id="m05-t013-s091"></a>

#### 24. TRIỀU LỤC — 61

Mẹ Khê.

Cả đời chưa từng sở hữu đất.

**Fear:** gia đình mất identity.

<a id="m05-t013-s092"></a>

#### Passive — **Biết Nhà Nào Đang Đi**

Nhìn cấu tạo, đồ đạc và tải trọng một thuyền cư trú, Lục có thể đoán khá chính xác tình trạng kinh tế/gia đình của chủ thuyền.

---

<a id="m05-t013-s093"></a>

## F13 — NHÀ THẠCH

<a id="m05-t013-s094"></a>

#### 25. THẠCH VŨ — 38

Hunter từng giết nhầm linh vật.

**Guilt:** cực cao.

**Spirituality:** tăng mạnh sau sự kiện.

<a id="m05-t013-s095"></a>

#### Passive — **Không Bắn Mũi Tên Thứ Hai**

Sau khi bỏ qua cơ hội tấn công đầu tiên vào một animal, Vũ có bonus lớn khi quan sát/hiểu hành vi của nó.

Passive khuyến khích restraint.

---

<a id="m05-t013-s096"></a>

#### 26. THẠCH SƠN — 18

Con Vũ.

Không biết secret.

Rất tự hào về cha.

<a id="m05-t013-s097"></a>

#### Passive — **Học Bằng Nhìn**

Lần đầu quan sát mentor thực hiện kỹ năng săn bắn thành công, Sơn nhận progression nhanh hơn nhưng rất dễ học cả thói quen xấu.

---

<a id="m05-t013-s098"></a>

## F14 — NHÀ HƯƠNG

<a id="m05-t013-s099"></a>

#### 27. HƯƠNG LAN — 32

Thương nhân đa ngôn ngữ.

**Need:** được nơi nào đó coi là người nhà.

<a id="m05-t013-s100"></a>

#### Passive — **Một Câu Có Nhiều Nghĩa**

Trong dialogue đa văn hóa, Lan có khả năng phát hiện khi misunderstanding đến từ convention/ngôn ngữ thay vì ác ý.

---

<a id="m05-t013-s101"></a>

#### 28. HƯƠNG NAM — 20

Em trai.

Không muốn nối nghề.

Muốn trở thành storyteller.

<a id="m05-t013-s102"></a>

#### Contradiction

Ghét việc cha mẹ đổi cách nói theo khách hàng nhưng chính mình lại bắt chước giọng người khác cực giỏi.

<a id="m05-t013-s103"></a>

#### Passive — **Mượn Giọng Người Khác**

Sau thời gian đủ dài ở một settlement, Nam có thể bắt chước một số speech convention địa phương, giảm social distance nhưng nếu bị phát hiện giả tạo có thể phản tác dụng.

---

<a id="m05-t013-s104"></a>

## F15 — NHÀ NỎ

<a id="m05-t013-s105"></a>

#### 29. NỎ CẦN — 34

Master craftsman.

Tin mỗi vũ khí phải hợp người dùng.

<a id="m05-t013-s106"></a>

#### Passive — **Một Cung Một Chủ**

Weapon do Cần custom cho một character nhận bonus riêng nhưng người khác dùng kém hiệu quả hơn.

---

<a id="m05-t013-s107"></a>

#### 30. NỎ XUYÊN — 29

Em trai.

Tin vào standardization.

<a id="m05-t013-s108"></a>

#### Passive — **Làm Một Thành Mười**

Khi chế tạo hàng loạt một loại vật phẩm đơn giản đã master, Xuyên giảm mạnh thời gian sản xuất nhưng không thể đạt phẩm chất độc bản như anh.

Hai passive đại diện cho hai triết lý công nghệ hoàn toàn khác nhau.

---

<a id="m05-t013-s109"></a>

## F16 — NHÀ LAU

<a id="m05-t013-s110"></a>

#### 31. LAU MIÊN — 44

Healer.

Tin thuốc và độc chỉ khác nhau ở lượng dùng.

<a id="m05-t013-s111"></a>

#### Passive — **Liều Là Thuốc**

Miên có thể điều chỉnh dosage của một số dược liệu để đổi giữa hiệu ứng chữa trị và độc tính.

Sai assessment có hậu quả lớn hơn bình thường.

---

<a id="m05-t013-s112"></a>

#### 32. LAU KHA — 26

Cháu Miên.

Bí mật bán độc.

Không phải vì thích giết người.

Muốn trả nợ gia đình.

<a id="m05-t013-s113"></a>

#### Passive — **Không Mùi Không Vị**

Kha nhận biết tốt những chất có thể che mùi vị của dược liệu, hữu ích cả trong medicine lẫn poison detection.

---

<a id="m05-t013-s114"></a>

## F17 — NHÀ HẠC

<a id="m05-t013-s115"></a>

#### 33. HẠC TÂM — 59

Trưởng nhà.

Biết hoạt động smuggling của cháu.

<a id="m05-t013-s116"></a>

#### Core Conflict

Danh dự gia đình vs sinh mạng người nhà.

<a id="m05-t013-s117"></a>

#### Passive — **Hàng Đi Qua Tay Ta**

Khi kiểm tra kho hàng hoặc manifests thủ công, Tâm dễ phát hiện chênh lệch giữa inventory thật và khai báo.

---

<a id="m05-t013-s118"></a>

#### 34. HẠC LƯU — 28

Người cháu đang vận chuyển vũ khí.

Tin rằng mình giúp phe có thể ngăn nội chiến lớn hơn.

<a id="m05-t013-s119"></a>

#### Passive — **Kiện Hàng Không Có Tên**

Lưu có khả năng giấu một vật nhỏ/medium trong legitimate cargo tốt hơn nhưng bị penalty danh dự rất nặng nếu bị chính family phát hiện.

---

<a id="m05-t013-s120"></a>

## F18 — NHÀ RÙA

<a id="m05-t013-s121"></a>

#### 35. RÙA THÂM — 70

Người giữ truyền thuyết.

Tin hoàn toàn câu chuyện rùa mang vật lạ.

<a id="m05-t013-s122"></a>

#### Passive — **Chuyện Không Được Viết**

Có thể nhớ lại oral lore liên quan một địa điểm/item nếu từng được truyền trong family, kể cả không có bằng chứng vật chất.

Thông tin có thể đúng hoặc sai.

---

<a id="m05-t013-s123"></a>

#### 36. RÙA AN — 23

Cháu.

Hoài nghi mọi chuyện.

<a id="m05-t013-s124"></a>

#### Passive — **Hỏi Lại Lần Nữa**

Khi nghe một legend, An có thể tách các chi tiết mà nhiều phiên bản câu chuyện không thống nhất, mở investigation branch đặc biệt.

Hai người cùng family nhưng một người mạnh vì tin, một người mạnh vì nghi.

---

<a id="m05-t013-s125"></a>

## F19 — NHÀ ĐÁ

<a id="m05-t013-s126"></a>

#### 37. ĐÁ HỔ — 40

Một ứng viên chief.

Charismatic.

Thực sự yêu family.

Nhưng tin chỉ mình đủ khả năng lãnh đạo.

<a id="m05-t013-s127"></a>

#### Passive — **Người Ta Nhìn Về Phía Ta**

Trong crisis không có leader rõ ràng, Hổ dễ giành quyền điều phối NPC hơn.

Nếu quyết định thất bại, reputation loss lớn hơn bình thường.

---

<a id="m05-t013-s128"></a>

#### 38. ĐÁ NHIÊN — 37

Cousin và đối thủ.

Ít charismatic hơn nhưng được các elder tin.

<a id="m05-t013-s129"></a>

#### Passive — **Nghe Trước Khi Chọn**

Nếu Nhiên chưa hành động trong lượt đầu của negotiation/crisis, lượt sau nhận thêm lựa chọn dựa trên hành vi những người khác.

---

<a id="m05-t013-s130"></a>

## F20 — NHÀ CHẠ

<a id="m05-t013-s131"></a>

#### 39. CHẠ LÂM — 52

Nông dân mất đất vì công trình Cổ Loa.

Không ghét mọi thứ mới.

Ông ghét việc không ai hỏi mình.

<a id="m05-t013-s132"></a>

#### Passive — **Biết Đất Trước Khi Có Thành**

Tại Loa Vực, Lâm biết terrain, đường nước và dấu tích trước xây dựng mà cư dân mới không biết.

---

<a id="m05-t013-s133"></a>

#### 40. CHẠ MINH — 24

Con trai Lâm.

Làm việc cho công trình mới.

Tin rằng Cổ Loa sẽ tạo ra tương lai tốt hơn.

<a id="m05-t013-s134"></a>

#### Contradiction

Xây tương lai bằng chính vùng đất cha mình đang mất.

<a id="m05-t013-s135"></a>

#### Passive — **Thấy Thành Trong Đống Đất**

Khi quan sát công trình đang xây, Minh có thể suy ra cấu trúc tương lai và phát hiện điểm yếu engineering chưa hoàn thiện.

---

<a id="m05-t013-s136"></a>

## XXI. Batch này cho chúng ta một kết luận quan trọng

40 nhân vật trên có nghề trùng.

Có trauma trùng.

Có region trùng.

Thậm chí có gia đình trùng.

Nhưng **không character nào cùng một gameplay identity**.

Ví dụ chỉ riêng “người giỏi sông nước” đã có:

Mộc → đọc bất thường dòng nước.

Tòng → network bến thuyền.

Khê → nghỉ trên thuyền.

Lục → đọc household qua thuyền.

Không ai là:

> “Water Skill +10%.”

Đây chính là hướng đúng cho 1.000 character.

---

<a id="m05-t013-s137"></a>

## XXII. Character Quality Gate

Sau này generator chỉ được chấp nhận một Tier A character nếu vượt qua **10 bài kiểm tra**:

| Check | Yêu cầu |
|---|---|
| World Fit | character hợp settlement |
| Family Fit | tồn tại hợp family |
| Occupation Fit | nghề có nguồn học |
| Psychological Cause | personality có nguyên nhân |
| Desire | có mục tiêu cụ thể |
| Fear | có thứ thật sự mất được |
| Contradiction | có ít nhất một |
| Relationship | tối thiểu 3 edge có ý nghĩa |
| Passive | xuất phát từ biography |
| Narrative Future | có ít nhất 2 hướng phát triển |

Nếu thiếu một trong các mục quan trọng, character chưa đủ chất lượng để thành **Named NPC**.

---

<a id="m05-t013-s138"></a>

## XXIII. Cấu trúc 1.000 nhân vật giờ có thể phân bổ

Tôi đề xuất mục tiêu ban đầu:

**100 nhân vật Core**

Cực kỳ sâu.

Có major story arc.

Có khả năng follower / enemy / faction leader.

**300 nhân vật Major**

Biography đầy đủ.

Unique passive.

Quan hệ phức tạp.

Có side story.

**600 nhân vật Standard Named**

Có family, history, psychology, unique passive và mục tiêu riêng nhưng lượng authored dialogue thấp hơn.

Tổng:

**1.000 Named Characters.**

Bên dưới vẫn còn population NPC procedural.

---

<a id="m05-t013-s139"></a>

## Bước tiếp theo nên chuyển sang một tầng khác

Bây giờ ta đã có:

**World**

→ **Region**

→ **Settlement**

→ **Family**

→ **Social Graph**

→ **Psychology**

→ **Individual Character**

Tôi nghĩ **chưa nên sang combat**.

Thứ còn thiếu để thế giới thực sự bắt đầu tự kể chuyện là:

<a id="m05-t013-s140"></a>

## LIFE EVENT + STORY GENERATION ENGINE

Ở đó ta sẽ xây khoảng **200–300 loại biến cố gốc** như sinh, chết, cưới, mất mùa, lũ, phản bội, phát hiện bí mật, gặp linh thể, chiến tranh, di cư, tai nạn nghề nghiệp, tình yêu, nợ, phạm tội...

Nhưng điều quan trọng hơn là thiết kế **chuỗi nhân quả**:

> mất mùa  
> → thiếu lương  
> → gia đình vay nợ  
> → con phải đi làm  
> → gặp mentor ở vùng khác  
> → yêu người ngoại vùng  
> → kết hôn  
> → hai dòng họ xung đột  
> → sinh một character hybrid  
> → 18 năm sau character đó có thể trở thành follower của player.

Khi hệ này hoàn thành, chúng ta sẽ có thứ gần giống **máy sinh lịch sử cá nhân và quest**, thay vì chỉ là character generator. Đây là bước tôi đề xuất làm tiếp.

<!-- SOURCE T013 END -->

