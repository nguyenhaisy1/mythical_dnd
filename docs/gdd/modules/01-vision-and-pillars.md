# Module 01 — Tầm nhìn, trụ cột và định vị game

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T003](../../source/conversation.md#t003)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Áp dụng yêu cầu mới: nhân vật random, gameplay runtime thuần code. Đoạn tạo nhân vật bằng mô tả hoặc chọn class trong concept ban đầu chỉ còn giá trị lịch sử thiết kế. Tên Hồng Bàng là working title.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Yêu cầu người dùng, material culture, giới hạn sản xuất. |
| Đầu ra / bên sở hữu | Pillars và tiêu chí sản phẩm; Design sở hữu, không có state mutation. |
| Đọc cùng | 07, 22, 24–29; ADR-001/002. |
| Cổng nghiệm thu | Một vòng đời có nghề, quan hệ và hậu quả tồn tại được ngoài combat. |
| Việc chưa được chứng minh | Tên thương mại, art budget, platform và playtest product fit. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T003 BEGIN -->

Concept này có tiềm năng rất lớn nếu xác định ngay từ đầu rằng đây không phải là “D&D khoác skin Việt Nam”, mà là một **CRPG/role-playing game lấy cấu trúc nhập vai kiểu tabletop làm nền, nhưng toàn bộ thế giới, class, thần thoại, quái vật, trang bị, tín ngưỡng và câu chuyện được xây lại từ chất liệu Văn Lang – Âu Lạc và kho tàng truyền thuyết Việt Nam**.

Điểm quan trọng nhất tôi đồng ý với bạn là: **content phải là tài sản cốt lõi của game**. Đồ họa có thể đơn giản, nhưng thế giới phải đủ sâu để người chơi cảm giác rằng mỗi làng, mỗi nhân vật, mỗi vật phẩm, mỗi miếu thờ đều có lịch sử riêng.

Từ đây, khi cùng bạn thiết kế game, tôi sẽ ưu tiên nguyên tắc **“chi tiết trước, khái quát sau”**: nếu một chi tiết có thể giải thích được thì chúng ta sẽ thiết kế nó; chỉ để abstraction ở những nơi thực sự không cần thiết.

---

<a id="m01-t003-s001"></a>

## 1. Định vị game ban đầu

Tôi hình dung game ở dạng:

> **Narrative CRPG + Dungeon RPG + Character Building + Vietnamese Mythology**

Người chơi không điều khiển một nhân vật có sẵn.

Họ **tạo nhân vật của chính mình**, tương tự tabletop RPG.

Ví dụ người chơi nhập:

> “Tôi là một thanh niên sinh ra trong một làng chài ven sông, cha mất trong một trận lũ, được một thầy mo nuôi lớn. Tôi khỏe nhưng ít nói, biết săn cá và sử dụng lao. Tôi rất sợ những thứ liên quan đến ma nước.”

Hệ thống có thể phân tích description đó và đề xuất:

| Thành phần | Kết quả |
|---|---|
| Xuất thân | Dân chài |
| Quê quán | Làng ven sông |
| Thiên hướng | Thể chất / Sinh tồn |
| Điểm mạnh | Sức khỏe, bơi lội, lao |
| Điểm yếu | Tâm lý trước thủy quái |
| Knowledge | Sông nước, đánh cá |
| Background perk | Con của sông |
| Class đề xuất | Thủy Binh / Liệp Nhân |
| Class phụ tiềm năng | Vu Chúc |
| Trait | Ít nói |
| Trauma | Ám ảnh nước dữ |

Nhưng người chơi **không bắt buộc phải nhận đề xuất của game**.

Họ vẫn có thể tự phân phối chỉ số và chọn hướng phát triển.

Đây sẽ là một trong những đặc điểm nhận diện của game.

---

<a id="m01-t003-s002"></a>

## 2. Triết lý quan trọng nhất: không chia class theo fantasy phương Tây

Tôi không muốn trong thế giới này xuất hiện:

> Warrior  
> Mage  
> Rogue  
> Paladin  
> Cleric

rồi chỉ đổi tên sang tiếng Việt.

Class nên sinh ra từ chính xã hội Văn Lang.

Ví dụ một hệ thống class sơ khai có thể xuất hiện những archetype như:

| Archetype | Vai trò |
|---|---|
| **Lạc Binh** | Chiến đấu trực diện |
| **Liệp Nhân** | Săn bắn, truy dấu |
| **Sơn Khách** | Sinh tồn núi rừng |
| **Thủy Binh** | Sông nước |
| **Vu Chúc** | Giao tiếp thế giới tâm linh |
| **Dược Nhân** | Thảo dược, độc, chữa trị |
| **Xảo Nhân** | Chế tác, cơ quan, công cụ |
| **Nhạc Sư** | Nghi lễ, trống đồng, âm luật |
| **Sứ Giả** | Ngoại giao, thuyết phục |
| **Hộ Vệ** | Bảo vệ cộng đồng |

Sau đó mới phát triển thành những nhánh cao cấp.

Chẳng hạn:

**Vu Chúc → Đồng Nhân → Linh Sứ → Người Giữ Cửa Âm Dương**

hoặc:

**Liệp Nhân → Sơn Liệp → Dị Thú Liệp Giả → Người Săn Linh Thú**

Như vậy skill tree sẽ rất Việt Nam mà không có cảm giác reskin D&D.

---

<a id="m01-t003-s003"></a>

## 3. Thế giới không chỉ có “Vua Hùng”

Đây là điểm cực kỳ quan trọng.

Nếu game chỉ xoay quanh:

> Vua Hùng → Sơn Tinh Thủy Tinh → Thánh Gióng → bánh chưng bánh dày

thì nội dung sẽ hết rất nhanh.

Chúng ta nên xây dựng **một thế giới Văn Lang hoàn chỉnh**, trong đó những truyền thuyết quen thuộc chỉ là phần nổi.

Ví dụ:

<a id="m01-t003-s004"></a>

#### Trung tâm

Phong Châu.

Nơi đặt quyền lực của Hùng Vương.

Có:

Lạc hầu.

Lạc tướng.

Quân đội.

Thợ thủ công.

Thương nhân.

Vu chúc.

Các bộ tộc.

Các gia tộc.

Các miếu thờ.

---

<a id="m01-t003-s005"></a>

#### Vùng núi

Có các bộ lạc săn bắn.

Hang động.

Mỏ đồng.

Thần núi.

Các giống thú lớn.

Những cộng đồng không hoàn toàn phục tùng Hùng Vương.

---

<a id="m01-t003-s006"></a>

#### Vùng sông

Các cộng đồng sống quanh sông lớn.

Thủy quái.

Giao thương.

Cướp sông.

Các nghi lễ cầu nước.

Truyền thuyết về giao long.

---

<a id="m01-t003-s007"></a>

#### Vùng biển

Lạc Việt ven biển.

Ngư dân.

Đảo.

Thủy thần.

Sinh vật biển.

Thương nhân phương xa.

---

<a id="m01-t003-s008"></a>

#### Đầm lầy

Một biome rất phù hợp với dark fantasy Việt Nam.

Sương.

Rừng ngập nước.

Đỉa.

Rắn.

Cá sấu.

Ma trơi.

Thần đầm.

Làng bị bỏ hoang.

---

Như vậy chỉ riêng **geography** đã tạo ra hàng trăm quest.

---

<a id="m01-t003-s009"></a>

## 4. Chúng ta cần phân biệt ba lớp lore

Đây sẽ là nguyên tắc xuyên suốt toàn bộ project.

<a id="m01-t003-s010"></a>

#### Lớp 1 — Lịch sử

Những gì chúng ta có căn cứ khảo cổ hoặc sử liệu.

Ví dụ:

Văn hóa Đông Sơn.

Đồ đồng.

Nông nghiệp lúa nước.

Nhà sàn.

Thuyền.

Trống đồng.

Vũ khí đồng.

---

<a id="m01-t003-s011"></a>

#### Lớp 2 — Truyền thuyết

Những thứ thuộc mythology.

Ví dụ:

Lạc Long Quân.

Âu Cơ.

Sơn Tinh.

Thủy Tinh.

Thánh Gióng.

Chử Đồng Tử.

Mỵ Châu – Trọng Thủy.

---

<a id="m01-t003-s012"></a>

#### Lớp 3 — Game canon

Đây là nơi chúng ta sáng tạo.

Ví dụ:

Tại sao Thủy Tinh năm nào cũng trở lại?

Trong mythology thông thường:

> vì không lấy được Mỵ Nương.

Nhưng trong game có thể sâu hơn.

Có thể tồn tại một **Giao Ước Sơn Hà** cổ xưa.

Sơn thần và thủy thần phải giữ cân bằng.

Một sự kiện trong quá khứ đã phá vỡ giao ước đó.

Thủy Tinh không đơn giản là kẻ ghen tuông.

Ông ta có thể đại diện cho một thế lực cổ đại nằm dưới hệ thống sông Hồng.

Người chơi dần phát hiện:

> câu chuyện dân gian mà họ được nghe chỉ là một phiên bản đơn giản hóa của sự kiện thật.

Đây chính là cách biến folklore thành **deep lore**.

---

<a id="m01-t003-s013"></a>

## 5. Story nên vận hành giống một campaign tabletop

Game không nên có một chuỗi quest tuyến tính.

Một campaign có thể được tổ chức thành:

**Adventure → Chapter → Quest → Encounter → Event**

Ví dụ:

<a id="m01-t003-s014"></a>

#### Campaign

**Thời đại nước dâng**

<a id="m01-t003-s015"></a>

#### Chapter

Mùa mưa thứ nhất.

<a id="m01-t003-s016"></a>

#### Quest

Làng Đồng Mộc mất tích ba người.

<a id="m01-t003-s017"></a>

#### Investigation

Người chơi tìm dấu chân.

Nói chuyện với dân làng.

Kiểm tra miếu.

Điều tra bờ sông.

<a id="m01-t003-s018"></a>

#### Discovery

Những người mất tích không chết.

Họ đang làm lễ cho một thần nước cổ.

<a id="m01-t003-s019"></a>

#### Dungeon

Hang ngầm dưới lòng sông.

<a id="m01-t003-s020"></a>

#### Boss

Không nhất thiết phải giết.

Có thể là:

Thủy thú.

Vu chúc phản loạn.

Một linh thể.

Hoặc chính thần sông.

<a id="m01-t003-s021"></a>

#### Resolution

Người chơi lựa chọn:

Giết linh thể.

Khôi phục nghi lễ.

Hy sinh vật phẩm.

Thương lượng.

Hoặc bỏ mặc.

Tất cả tạo ra hậu quả khác nhau.

Đó mới là tinh thần D&D.

---

<a id="m01-t003-s022"></a>

## 6. Dungeon không nhất thiết phải là “hầm ngục”

Trong game này, khái niệm **Dungeon** nên hiểu rộng.

Dungeon có thể là:

Một hang động.

Một rừng thiêng.

Một ngôi làng bị nguyền.

Một mộ cổ.

Một đảo giữa sông.

Một đầm lầy.

Một khu chiến trường.

Một giấc mơ.

Một thế giới linh hồn.

Một ngọn núi.

Một mê cung tre.

Một thành cổ.

Thậm chí:

> **một lễ hội kéo dài ba ngày**

cũng có thể hoạt động giống dungeon.

Mỗi ngày là một “floor”.

NPC khác nhau.

Event khác nhau.

Mystery khác nhau.

Cuối cùng phát hiện bí mật.

---

<a id="m01-t003-s023"></a>

## 7. Combat không nên chiếm phần lớn game

Với concept bạn mô tả, tôi đề xuất tỷ lệ trải nghiệm về lâu dài khoảng:

**Combat ~30%**

**Exploration ~20%**

**Dialogue / Investigation ~25%**

**Character / Equipment / Crafting ~15%**

**World events / Narrative ~10%**

Điều này giúp game tận dụng được thứ mạnh nhất của nó:

> **nội dung.**

Một nhân vật có Charisma cao có thể kết thúc một quest mà không đánh nhau.

Một Vu Chúc có thể nói chuyện với linh hồn.

Một Liệp Nhân có thể phát hiện dấu vết mà người khác không nhìn thấy.

Một Dược Nhân có thể nhận ra:

> “Người này không bị ma nhập. Đây là trúng độc.”

Ngay lập tức quest chuyển hướng.

Đây là thứ làm CRPG có chiều sâu.

---

<a id="m01-t003-s024"></a>

## 8. Skill Check sẽ là xương sống của gameplay

Ví dụ người chơi bước vào một ngôi nhà.

Game có thể tự kiểm tra:

**Quan Sát 12**

Bạn nhìn thấy bùn dưới chân người đàn ông.

**Sinh Tồn 15**

Loại bùn này không xuất hiện quanh làng.

**Tri Thức Sơn Lâm 14**

Nó chỉ có ở khu vực phía bắc núi.

**Tâm Linh 18**

Trên người ông ta có dấu vết của một nghi lễ.

Nếu nhân vật không có skill đó:

Game **không nói cho người chơi biết thông tin đó tồn tại**.

Điều này tạo replay value rất lớn.

---

<a id="m01-t003-s025"></a>

## 9. Equipment cũng phải kể chuyện

Không nên chỉ có:

> Bronze Sword  
> Damage +12

Mà nên có:

**Đoản Qua Đồng Đen**

Nguồn gốc:

Được tìm thấy trong một ngôi mộ của chiến binh bộ Chu Diên.

Thuộc tính:

Sát thương: 7–11

+2 khi chiến đấu trong đội hình.

Trait:

**Ký ức chiến trận**

Nếu người sử dụng có trait “Huyết Chiến”, có xác suất nhìn thấy ký ức của chủ nhân cũ.

Sau đó thậm chí mở ra quest:

> **Người chiến binh không có mộ**

Tức là equipment cũng trở thành **content delivery mechanism**.

---

<a id="m01-t003-s026"></a>

## 10. Nhân vật phải có cả Character Sheet

Không chỉ:

STR  
DEX  
INT.

Chúng ta có thể có bốn tầng.

**Attributes**

Thể

Nhanh

Trí

Tâm

Khí

---

**Skills**

Săn bắn

Bơi

Leo núi

Thảo dược

Chế tác

Thuyết phục

Dọa nạt

Nghi lễ

Lịch sử

Sinh tồn

Quan sát

V.v.

---

**Traits**

Can đảm.

Đa nghi.

Tham lam.

Trọng lời hứa.

Sợ ma.

Kính thần.

Ghét quyền lực.

---

**Reputation**

Danh tiếng không phải một con số duy nhất.

Ví dụ:

Phong Châu: +20

Bộ Dâu: -15

Vu Chúc: +40

Thương nhân: +5

Sơn dân: +30

Điều này ảnh hưởng trực tiếp đến quest và dialogue.

---

<a id="m01-t003-s027"></a>

## 11. Một hệ thống tôi đặc biệt muốn đưa vào: Đức Tin và Điềm Báo

Bởi vì thế giới này gắn chặt với tín ngưỡng.

Người chơi có thể:

Thờ thần núi.

Thờ thần sông.

Thờ tổ tiên.

Không theo thần nào.

Tin vào linh vật.

Kính trời đất.

Nhưng thần trong game không đơn giản là “buff”.

Có thể bạn cầu Thủy thần:

> + khả năng di chuyển trên sông.

Nhưng một Sơn thần lại không thích bạn.

Hoặc bạn phá một miếu cổ.

Không có thông báo:

> Reputation -10.

Thay vào đó vài ngày sau:

chim không hót khi bạn đi vào rừng.

Rồi thú tránh bạn.

Sau đó bắt đầu xuất hiện những giấc mơ.

Đây sẽ là **narrative system**, không chỉ stat system.

---

<a id="m01-t003-s028"></a>

## 12. Game có thể có một trục thời gian rất lớn

Nếu chúng ta làm dài hơi, hoàn toàn có thể bao phủ:

**Khởi nguyên**

Lạc Long Quân – Âu Cơ.

↓

**Các đời Hùng Vương**

↓

Sơn Tinh – Thủy Tinh.

↓

Thánh Gióng.

↓

Các cuộc xung đột Văn Lang.

↓

Thục Phán.

↓

**Âu Lạc**

↓

Cổ Loa.

↓

Nỏ thần.

↓

Mỵ Châu – Trọng Thủy.

↓

Sự sụp đổ của Âu Lạc.

Tuy nhiên không nhất thiết player sống xuyên hàng trăm năm.

Ta có thể chia thành **Campaign / Era**.

Một campaign là một nhân vật.

Nhưng hành động của campaign trước có thể trở thành truyền thuyết trong campaign sau.

Đây là một ý tưởng cực kỳ thú vị.

Ví dụ nhân vật campaign đầu tiên tên:

> Đan.

300 năm sau NPC kể:

> “Ngày xưa có người tên Đan từng giết con giao long ở sông.”

Nhưng player biết:

> mình không hề giết nó.

Mình đã thương lượng với nó.

Qua hàng trăm năm câu chuyện bị biến đổi thành truyền thuyết.

Đây là một hệ thống rất phù hợp với chủ đề game.

---

<a id="m01-t003-s029"></a>

## 13. Tôi đề xuất nguyên tắc thiết kế tổng thể

Từ bây giờ, mỗi thành phần chúng ta thiết kế sẽ cố gắng trả lời đồng thời ba câu hỏi:

**Nó có gameplay gì?**

**Nó có lore gì?**

**Nó tạo ra câu chuyện gì?**

Ví dụ một cây giáo không chỉ là weapon.

Nó có:

combat mechanic.

người chế tạo.

nguồn nguyên liệu.

vùng văn hóa.

lịch sử.

NPC liên quan.

quest tiềm năng.

Tương tự với một cái làng.

Không chỉ:

> Làng A có blacksmith và shop.

Mà phải biết:

làng hình thành từ đâu.

sống bằng nghề gì.

thờ ai.

quan hệ với làng bên cạnh.

ai cai quản.

đang có vấn đề gì.

có bí mật gì.

người dân sợ điều gì.

lễ hội gì.

truyền thuyết gì.

---

<a id="m01-t003-s030"></a>

### Kiến trúc thiết kế game mà tôi đề xuất

Chúng ta có thể phát triển toàn bộ game theo khoảng **12 hệ thống lớn**:

| Hệ thống | Nội dung |
|---|---|
| World | Văn Lang – Âu Lạc |
| Timeline | Các thời kỳ / campaign |
| Character | Tạo nhân vật |
| Attributes | Chỉ số |
| Class | Class + subclass |
| Skill | Skill tree |
| Combat | Chiến đấu |
| Equipment | Trang bị |
| Exploration | Khám phá |
| Dialogue | Hội thoại / skill check |
| Quest | Quest / story / dungeon |
| Mythology | Thần linh / linh vật / quái |
| Progression | Level / reputation / progression |
| Economy | Tiền / barter / crafting |
| Faction | Bộ lạc / triều đình / giáo phái |
| Narrative Engine | Event / choice / consequence |

Và tôi nghĩ **chưa nên bắt đầu bằng combat**.

Thứ đầu tiên chúng ta nên thiết kế thật sâu là:

> **WORLD & LORE BIBLE của Văn Lang – Âu Lạc phiên bản game.**

Bởi vì khi thế giới đã được định nghĩa rõ, **class, skill, quái vật, dungeon, equipment, faction, quest và main story gần như tự sinh ra từ thế giới đó**.

Sau đó chúng ta có thể đi cực sâu từng phần, chẳng hạn riêng **Character Creation** cũng có thể thiết kế đến mức có hàng chục quê quán, dòng dõi, nghề nghiệp cha mẹ, biến cố thời thơ ấu, niềm tin, trauma, personality, starting relationship và hidden trait.

<!-- SOURCE T003 END -->

