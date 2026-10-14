# Module 02 — Thế giới, lịch sử, campaign và 12 đại vùng

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T005](../../source/conversation.md#t005), [T007](../../source/conversation.md#t007)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Giữ 12 vùng và sáu hồi làm world-canon. Phân biệt lịch sử, truyền thuyết, game-canon; mốc Hard/Flexible/Emergent phải theo audit 25/27. Bến Hạc vẫn thuộc Phong Châu.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | World premise; lịch sử; địa lý; truyền thuyết. |
| Đầu ra / bên sở hữu | Region/settlement definitions và historical contracts; World/History sở hữu. |
| Đọc cùng | 03, 19, 21, 25; CANON-REGISTER. |
| Cổng nghiệm thu | Mỗi địa danh có một parent region; mọi anchor có điều kiện vật chất và phạm vi agency. |
| Việc chưa được chứng minh | World topology đầy đủ, khảo cứu nguồn và mốc campaign production. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T005 BEGIN -->

Được. Với mục tiêu của bạn, tôi nghĩ ngay từ đầu phải thiết kế theo hướng **một thế giới có khả năng “sinh ra câu chuyện”**, chứ không phải viết một main story rồi nhét 1.000 nhân vật vào đó.

Tôi đề xuất tạm gọi project là **HỒNG BÀNG**. Tên chính thức có thể đổi sau.

Điểm quan trọng nhất là: **1.000 nhân vật không được chỉ khác nhau về tên, nghề và chỉ số.** Mỗi nhân vật phải có lý do hợp lý để tồn tại trong thế giới, có quê quán, quá khứ, quan hệ, niềm tin, mâu thuẫn nội tâm, mục tiêu và ít nhất một passive mà nhân vật khác không có.

---

<a id="m02-t005-s001"></a>

## I. Chốt khung lịch sử của game

Nếu muốn gom được nhiều chất liệu nhất mà vẫn giữ một cốt truyện xuyên suốt, tôi không đặt game vào một đời Hùng Vương cụ thể.

Khung tốt hơn là:

> **Cuối thời Văn Lang → quá trình chuyển đổi sang Âu Lạc → xây dựng Cổ Loa → thời An Dương Vương → khủng hoảng dẫn đến kết cục Mỵ Châu – Trọng Thủy.**

Các truyền thuyết thuộc thời xa hơn như Lạc Long Quân – Âu Cơ, Thánh Gióng, Sơn Tinh – Thủy Tinh, Lang Liêu... tồn tại dưới dạng **lịch sử cổ, truyền thuyết, di tích, tín ngưỡng và “ký ức thế giới”**, thay vì bắt tất cả nhân vật sống cùng một thời.

Đây cũng là khoảng không rất thuận lợi để hư cấu. Bảo tàng Lịch sử Quốc gia đặt Văn Lang vào khoảng thế kỷ VII TCN trên nền văn hóa Đông Sơn, rồi sự hình thành Âu Lạc dưới Thục Phán vào khoảng thế kỷ III TCN; Phong Châu và Cổ Loa là hai trung tâm gắn với hai giai đoạn này. 

Khảo cổ Cổ Loa còn cung cấp cực nhiều nguyên liệu gameplay: cư trú nông–chài, thành đất quy mô lớn, hàng vạn mũi tên đồng, khuôn và lò đúc mũi tên.  Mộ thuyền Đông Sơn cùng đồ tùy táng như vũ khí, công cụ, nhạc khí và đồ sinh hoạt lại là nền tuyệt vời cho dungeon, nghi lễ và thế giới người chết. 

Tuy nhiên tôi muốn giữ ranh giới rất rõ:

**Khảo cổ / sử liệu = bộ xương.**

**Truyền thuyết = linh hồn.**

**Game canon = cơ thể hoàn chỉnh.**

Chúng ta không cần tuyên bố mọi chi tiết fantasy từng thật sự tồn tại.

---

<a id="m02-t005-s002"></a>

## II. Tiền đề cốt truyện lớn

Tên campaign tạm:

<a id="m02-t005-s003"></a>

## HỒNG BÀNG — MẠCH LẠC

Trong thế giới game tồn tại một khái niệm cổ gọi là:

> **Mạch Lạc**

Nó không phải mana.

Nó là cách con người thời đó gọi mối liên hệ giữa:

**đất – nước – con người – tổ tiên – thần linh – ký ức.**

Một dòng sông có Mạch.

Một ngọn núi có Mạch.

Một dòng họ có Mạch.

Một làng tồn tại nhiều đời cũng hình thành Mạch.

Một chiến trường nơi hàng trăm người chết có thể tạo ra Mạch méo mó.

Trống đồng, nghi lễ, mộ táng, vật tổ... đều có khả năng tác động đến Mạch.

Đây sẽ là nền tảng supernatural của toàn bộ game.

Nhưng không phải ai cũng tin nó tồn tại.

Một thợ đúc có thể nói:

> Trống kêu hay vì hợp kim tốt.

Vu Chúc lại nói:

> Trống kêu vì tổ tiên đáp lời.

Cả hai có thể đều đúng.

---

<a id="m02-t005-s004"></a>

## III. Bí mật lớn của thế giới

Người chơi ban đầu không biết điều này.

Trong thời kỳ xa xưa, các cộng đồng quanh sông, núi và đồng bằng liên tục xung đột.

Một hệ thống giao ước được hình thành giữa các cộng đồng.

Game gọi nó là:

<a id="m02-t005-s005"></a>

## Ước Sơn Hà

Không phải một tờ hiệp ước.

Nó là hàng trăm lời thề, nghi lễ, hôn phối, cột mốc, miếu, trống, mộ tổ và tục lệ địa phương.

Qua nhiều thế hệ, các giao ước ấy trở thành nền tảng giúp Văn Lang tồn tại.

Hùng Vương không hoàn toàn là một vị vua theo nghĩa quân chủ tập quyền sau này.

Trong game, Hùng Vương là:

> **người giữ quyền triệu tập các Lạc bộ và người đứng đầu nghi lễ Sơn Hà.**

Đây là game canon, không phải khẳng định lịch sử.

Và khi game bắt đầu...

**Ước Sơn Hà đang tan rã.**

---

<a id="m02-t005-s006"></a>

## IV. Điều đang xảy ra khi người chơi xuất hiện

Trong khoảng một thế hệ, hàng loạt biến cố diễn ra.

Lũ lớn bất thường.

Một số bộ từ chối triều cống.

Các thủ lĩnh địa phương tích trữ đồng.

Các mỏ bắt đầu trở thành mục tiêu tranh giành.

Những tuyến trao đổi muối – đồng – gạo bị cướp.

Một số miếu không còn được cúng tế.

Những ngôi mộ cổ bị đào.

Tin đồn xuất hiện rằng Hùng Vương đã mất thiên mệnh.

Ở vùng cao, một liên minh mới đang hình thành quanh người Âu Việt.

Ở phía bắc, sức ép quân sự ngày càng tăng.

Các Lạc tướng bắt đầu phải chọn phe.

Trong khi đó, những hiện tượng kỳ lạ xuất hiện ngày càng nhiều.

Không phải vì:

> “Ma vương tỉnh giấc.”

Mà bởi vì hàng trăm giao ước nhỏ đang bị con người phá vỡ.

Đây là nguyên nhân khiến **Mạch Lạc mất cân bằng**.

---

<a id="m02-t005-s007"></a>

## V. Đây sẽ không phải câu chuyện thiện – ác

Đây là trục rất quan trọng.

Hùng Vương không nhất thiết là phe thiện.

Thục Phán không phải phản diện.

Thủy Tinh không chỉ là ác thần.

Sơn Tinh không hoàn toàn chính nghĩa.

Các Vu Chúc không phải tất cả đều mê tín.

Những người không tin thần linh cũng không nhất thiết đúng.

Một Lạc tướng phản Hùng có thể làm vậy vì làng mình đang chết đói.

Một thủ lĩnh theo Thục Phán có thể thực sự tin rằng mô hình Văn Lang không còn đủ sức bảo vệ dân chúng.

Một người phá miếu có thể làm vậy để lấy đất trồng lúa cứu cả làng.

Một thần sông giết người có thể đang bảo vệ vùng sinh sản của nó khỏi con người.

Người chơi liên tục gặp vấn đề mà không có đáp án tuyệt đối.

---

<a id="m02-t005-s008"></a>

## VI. Cấu trúc thế giới

Tôi đề xuất ban đầu có **12 đại vùng**, nhưng không coi chúng là 12 tỉnh hay 12 đơn vị hành chính lịch sử.

Chúng là **Game Region**, mỗi vùng đại diện cho một hệ sinh thái và một kiểu xã hội.

| Vùng | Đặc trưng | Nguồn sống | Vấn đề | Nhân vật thường sinh ra |
|---|---|---|---|---|
| Phong Châu | trung tâm quyền lực | thủ công, trao đổi, hành chính | tranh giành quyền lực | sứ giả, vệ binh, quý tộc, thợ |
| Tam Giang | hợp lưu sông | cá, vận tải | lũ, cướp sông | ngư dân, thủy thủ, người đưa đò |
| Bãi Lạc | đồng bằng | lúa nước | đất đai, mùa vụ | nông dân, trưởng làng, dược nhân |
| Đồng Đỏ | vùng luyện kim | đồng, than, đồ đúc | tranh mỏ | thợ đúc, phu mỏ, thương nhân |
| Sơn Khê | núi thấp | săn bắt, gỗ | thú dữ, bộ tộc | thợ săn, sơn dân |
| Thượng Âu | vùng cao | săn bắt, trao đổi | chính trị Âu–Lạc | chiến binh, thủ lĩnh |
| Rừng Thiêng | rừng già | dược liệu | cấm địa | vu chúc, dược nhân |
| Đại Trạch | đầm lầy | cá, lau, lúa | bệnh, thủy quái | thợ săn đầm, người hái thuốc |
| Cửa Đông | cửa biển | muối, cá, thuyền | bão, ngoại thương | ngư dân, thương nhân |
| Hải Đảo | đảo ven bờ | đánh cá, chim, muối | cô lập | hoa tiêu, thợ lặn |
| Bắc Tuyến | biên địa | trao đổi | chiến tranh | trinh sát, lính, phiên dịch |
| Loa Vực | khu vực Cổ Loa | nông nghiệp, quân sự, luyện kim | đô thị hóa | công binh, thợ nỏ, quân nhân |

Chỉ 12 vùng này đã tạo ra 12 hệ văn hóa con.

---

<a id="m02-t005-s009"></a>

## VII. Mỗi vùng không được chỉ khác biome

Ví dụ **Tam Giang**.

Trẻ em ở đây biết bơi trước khi biết săn.

Nhà quay mặt theo dòng nước.

Người ta phân biệt nhiều loại gió.

Người chết đuối có tục tang khác người chết trên đất.

Gia đình có thể sở hữu thuyền thay vì ruộng.

Có những gia tộc sống bằng việc đưa người qua sông.

Người Tam Giang coi một người không biết đọc nước gần giống như người miền núi nhìn một kẻ không biết đường rừng.

Từ đó tự nhiên xuất hiện character passive.

> **Đọc Sắc Nước**  
> Sau khi quan sát một vùng nước trong một lượt, nhân vật biết được hướng dòng, độ sâu tương đối, nguy cơ và các dấu hiệu bất thường.

Một người sinh ở Phong Châu không thể ngẫu nhiên có passive này nếu biography không giải thích được.

Đây chính là điều chúng ta cần cho hệ thống 1.000 nhân vật.

---

<a id="m02-t005-s010"></a>

## VIII. Thế giới phải có “lịch sử trước khi player sinh ra”

Tôi muốn chia quá khứ thành nhiều **Kỷ**.

<a id="m02-t005-s011"></a>

#### Kỷ Khởi Nguyên

Lạc Long Quân – Âu Cơ.

Con Rồng – cháu Tiên.

Sự phân chia núi và biển.

Trong game không khẳng định câu chuyện nào đúng hoàn toàn.

---

<a id="m02-t005-s012"></a>

#### Kỷ Khai Đất

Các nhóm cư dân khai phá lưu vực.

Mạch Lạc đầu tiên hình thành.

Nhiều thần địa phương xuất hiện.

---

<a id="m02-t005-s013"></a>

#### Kỷ Hùng

Liên minh ngày càng lớn.

Phong Châu trở thành trung tâm.

Hình thành các nghi lễ lớn.

---

<a id="m02-t005-s014"></a>

#### Kỷ Đại Thủy

Gắn với ký ức Sơn Tinh – Thủy Tinh.

Người đời kể là tranh Mỵ Nương.

Nhưng game cho phép player khám phá phía sau truyền thuyết còn là câu chuyện về:

đê điều,

quản lý nguồn nước,

di cư,

xung đột cư dân núi – cư dân sông,

và một giao ước thất bại.

---

<a id="m02-t005-s015"></a>

#### Kỷ Thiên Binh

Gắn với truyền thuyết Thánh Gióng.

Game sẽ không nhất thiết xác nhận rằng một đứa trẻ thật sự lớn thành người khổng lồ.

Có ít nhất 4 phiên bản truyền thuyết.

Người chơi có thể tìm bằng chứng cho từng phiên bản.

---

<a id="m02-t005-s016"></a>

#### Kỷ Rạn Nứt

Thời kỳ hiện tại khi game bắt đầu.

---

<a id="m02-t005-s017"></a>

#### Kỷ Âu Lạc

Xuất hiện trong nửa sau campaign.

Cổ Loa được xây dựng.

Quyền lực tập trung mạnh hơn.

Công nghệ quân sự thay đổi.

---

<a id="m02-t005-s018"></a>

## IX. Main Story có thể kéo dài một đời nhân vật

Tôi đề xuất khoảng **20–25 năm game-time**, nhưng thời gian nhảy giữa các chapter.

<a id="m02-t005-s019"></a>

### Act I — Trống không vang

Trong một lễ hội lớn tại Phong Châu, chiếc trống chủ lễ được đánh.

Âm thanh phát ra sai.

Các Vu Chúc coi đó là điềm cực xấu.

Một số người lại cho rằng mặt trống đã nứt.

Đêm hôm đó một người giữ nghi lễ bị giết.

Không có “chosen one”.

Player chỉ vô tình bị cuốn vào sự việc theo **origin quest** riêng.

---

<a id="m02-t005-s020"></a>

## X. Act II — Những lời thề đã quên

Player đi qua các vùng.

Dần nhận ra hàng loạt cuộc khủng hoảng tưởng không liên quan thực chất đều liên quan đến các giao ước địa phương.

Ví dụ:

Một làng chặt rừng thiêng.

Một bộ thay đổi dòng sông.

Một Lạc tướng chiếm mỏ đồng.

Một dòng họ đào mộ cũ.

Một thầy tế cố tình sửa nghi lễ.

Một ngôi làng giết linh vật bảo hộ.

Không phải tất cả đều có thể sửa.

---

<a id="m02-t005-s021"></a>

## XI. Act III — Hai con đường

Văn Lang ngày càng chia rẽ.

Một luồng cho rằng:

> phải phục hồi trật tự Hùng triều.

Luồng khác cho rằng:

> chính cấu trúc cũ đã gây ra khủng hoảng.

Thục Phán dần trở thành nhân vật quan trọng.

Player bắt đầu gặp Âu Việt.

Không phải với vai trò “quân xâm lược”.

Mà là một nền văn hóa khác với:

phong tục khác,

chiến thuật khác,

quan hệ cộng đồng khác,

tín ngưỡng khác.

Một số thứ thậm chí hiệu quả hơn Văn Lang.

---

<a id="m02-t005-s022"></a>

## XII. Act IV — Thành xoáy ốc

Âu Lạc ra đời.

Cổ Loa trở thành đại công trình trung tâm.

Đây có thể là một chapter khổng lồ.

Khảo cổ học thực sự cho chúng ta một nền rất tốt cho phần này: thành đất Cổ Loa, khu cư trú, hoạt động đúc đồng và số lượng lớn mũi tên ba cạnh đều có bằng chứng vật chất. 

Player có thể gặp:

thợ xây,

thợ đúc,

người vận chuyển,

binh lính,

nông dân bị trưng dụng,

Lạc tướng,

điệp viên,

thương nhân,

dân địa phương mất đất.

Một công trình được lịch sử nhớ là vĩ đại có thể đồng thời là thảm họa đối với một số NPC.

Đây là cách tạo chiều sâu.

---

<a id="m02-t005-s023"></a>

## XIII. Act V — Nỏ

“Nỏ thần” sẽ được xử lý đặc biệt.

Không nên đơn giản là:

> móng Rùa Vàng + craft = legendary crossbow.

Ta có thể có ba lớp sự thật.

Người dân tin là thần khí.

Thợ đúc biết đó là thành tựu kỹ thuật.

Vu Chúc tin rằng kỹ thuật và linh thiêng không hề mâu thuẫn.

Player có thể tìm thấy những xưởng, khuôn, thử nghiệm thất bại và con người đứng sau hệ thống vũ khí đó.

---

<a id="m02-t005-s024"></a>

## XIV. Act VI — Lông ngỗng

Mỵ Châu – Trọng Thủy trở thành phần cuối.

Nhưng player đã sống trong thế giới này hàng chục giờ trước khi họ xuất hiện.

Vì vậy câu chuyện không còn là một truyện ngụ ngôn ngắn.

Player biết:

Mỵ Châu thích gì.

Trọng Thủy sợ gì.

Ai nuôi Mỵ Châu.

Ai dạy cô.

Ai phục vụ trong cung.

Ai chế tạo vũ khí.

Ai muốn chiến tranh.

Ai muốn hòa bình.

Ai lợi dụng cuộc hôn nhân.

Khi bi kịch xảy ra, nó tác động đến hàng trăm NPC người chơi từng biết.

---

<a id="m02-t005-s025"></a>

## XV. Bây giờ đến phần quan trọng nhất: 1.000 character creation

Không nên tạo 1.000 template.

Ta xây **Character Genome**.

Mỗi nhân vật được tạo từ nhiều lớp.

| Genome | Ví dụ |
|---|---|
| Region | Tam Giang |
| Settlement | Bến Vạc |
| Community | gia đình đánh cá |
| Family | con thứ ba |
| Childhood | mất anh trong lũ |
| Mentor | người đưa đò già |
| Occupation | người săn cá lớn |
| Social status | thường dân |
| Core value | gia đình |
| Desire | sở hữu thuyền riêng |
| Fear | nước chết |
| Weakness | mê tín |
| Contradiction | sợ sông nhưng không thể rời sông |
| Secret | từng bỏ mặc một người chết đuối |
| Relationship | mắc nợ trưởng bến |
| Belief | kính thần sông |
| Talent | cảm nhận dòng nước |
| Trauma response | đóng băng |
| Ambition | trở thành trưởng bến |
| Unique passive | Nghe Nước Đổi Giọng |

Chỉ cần nhìn bảng này là character đã bắt đầu sống.

---

<a id="m02-t005-s026"></a>

## XVI. Personality không được sinh bằng “Brave / Kind / Smart”

Tôi đề xuất personality có ít nhất **10 biến**.

Ví dụ một nhân vật:

**Giá trị cốt lõi:** gia đình.

**Điều căm ghét:** kẻ thất hứa.

**Ham muốn:** được công nhận.

**Nỗi sợ:** bị bỏ rơi.

**Cách xử lý xung đột:** tấn công trước.

**Quan hệ với quyền lực:** phục tùng ngoài mặt.

**Quan hệ với thần linh:** kính nhưng sợ.

**Khi bình thường:** hài hước.

**Khi căng thẳng:** tàn nhẫn.

**Mâu thuẫn nội tâm:** muốn được yêu nhưng không tin người khác.

Hai NPC đều “can đảm” vẫn có thể hoàn toàn khác nhau.

---

<a id="m02-t005-s027"></a>

## XVII. Một Character bắt buộc phải có Contradiction

Đây là luật rất quan trọng.

Nhân vật hay gần như luôn có nghịch lý.

Ví dụ:

> Thợ săn giỏi nhưng sợ máu.

> Vu Chúc nhưng nghi ngờ thần linh.

> Chiến binh thích thơ ca.

> Người rất trọng gia đình nhưng bỏ nhà đi.

> Kẻ nói dối giỏi nhưng ghét sự phản bội.

> Người giàu nhưng không bao giờ dùng đồ đồng.

AI generation sẽ bắt buộc sinh ít nhất một **Character Contradiction**.

Điều này làm 1.000 character không trở thành 1.000 stereotype.

---

<a id="m02-t005-s028"></a>

## XVIII. Signature Passive

Đây có thể trở thành một trong những hệ thống hay nhất game.

Mỗi character có đúng **1 Signature Passive** không nhân vật nào được sở hữu bản giống hệt.

Passive không random hoàn toàn.

Nó phải được sinh từ biography.

Công thức:

> **Signature Passive = Origin + Life Event + Personality + Expertise + Condition**

Ví dụ.

Một đứa trẻ Tam Giang.

Cha chết do nước cuốn.

Sau đó sống 10 năm bằng nghề chèo thuyền.

Có tính cực kỳ cảnh giác.

→

<a id="m02-t005-s029"></a>

#### Nghe Nước Đổi Giọng

Khi bước vào khu vực sông, hồ hoặc đầm lần đầu tiên, nhân vật có thể nhận biết một nguy hiểm môi trường mà party thông thường không phát hiện.

---

Một người khác:

Sinh trong gia đình thợ đúc.

Bị bỏng tay từ nhỏ.

Ám ảnh việc tạo ra món đồ hoàn hảo.

→

<a id="m02-t005-s030"></a>

#### Tay Đã Quen Lửa

Lần đầu thất bại khi chế tác vật phẩm đồng, không mất nguyên liệu chính và nhận thêm thông tin về nguyên nhân thất bại.

---

Một chiến binh từng bỏ chạy khỏi trận đánh.

Cả nhóm của anh ta chết.

→

<a id="m02-t005-s031"></a>

#### Không Chạy Lần Nữa

Khi đồng đội đầu tiên bị hạ trong combat, nhân vật nhận Resolve lớn.

Nhưng không thể tự chọn Retreat trong ba lượt tiếp theo.

Passive vừa mạnh vừa kể chuyện.

---

<a id="m02-t005-s032"></a>

## XIX. Làm sao bảo đảm 1.000 passive thật sự unique?

Không dùng 1.000 passive viết tay.

Ta xây **Passive Grammar**.

Ví dụ có:

25 Trigger.

30 Effect.

12 Environment modifier.

16 Personality modifier.

10 Cost/Drawback.

Chỉ riêng:

**25 × 30 × 12 × 16 × 10 = 1.440.000 cấu hình.**

Sau đó rule engine chỉ cho phép những combination phù hợp biography.

Ví dụ character không biết sông nước thì không sinh:

> Khi xuống nước...

Character không có trauma thì không sinh passive dựa vào trauma.

Character ích kỷ không tự nhiên có:

> khi đồng đội bị thương...

trừ khi biography giải thích tại sao.

Như vậy passive vừa procedural vừa **narratively coherent**.

---

<a id="m02-t005-s033"></a>

## XX. Không chỉ passive khác nhau về tên

Đây là một lỗi procedural generation rất dễ mắc.

Không được:

**Con Sông:** +5% dodge trên sông.

**Con Núi:** +5% dodge trên núi.

Đó chỉ là reskin.

Chúng ta cần các kiểu passive khác hẳn về hành vi.

Ví dụ:

**Người Không Quên Mặt**

NPC từng gặp không thể giả vờ chưa quen bạn.

---

**Mùi Khói Quê Nhà**

Có thể nhận ra loại gỗ vừa bị đốt.

---

**Đếm Nhịp Trống**

Sau khi quan sát một nghi lễ đủ lâu, có khả năng phát hiện nhịp sai.

---

**Không Quỳ Trước Trống**

Không chịu penalty từ intimidation dựa trên địa vị.

Nhưng khó gây thiện cảm với giới quyền lực.

---

**Nhớ Đường Bằng Bùn**

Sau khi đi qua một khu đầm một lần, không thể bị lạc ở đó nữa.

---

**Tai Của Người Thợ Đúc**

Nghe âm thanh kim loại có thể đoán chất lượng vật phẩm.

Tất cả đều có thể tạo **gameplay mới**, không chỉ tăng damage.

---

<a id="m02-t005-s034"></a>

## XXI. Khả năng sinh character thực tế

Nếu chỉ dùng:

12 vùng,

20 nghề,

15 childhood,

12 core values,

10 fears,

12 ambitions,

10 contradictions,

thì đã có:

**12 × 20 × 15 × 12 × 10 × 12 × 10**

= **62.208.000 combinations**.

Chưa tính:

gia đình,

mentor,

religion,

reputation,

trauma,

relationships,

skills,

passive.

Vấn đề vì thế **không phải đủ combination hay không**.

Vấn đề thực sự là:

> làm sao khiến combination có ý nghĩa.

Đó là lý do Character Genome phải có dependency rule.

---

<a id="m02-t005-s035"></a>

## XXII. Một luật nữa: NPC phải thuộc về thế giới

Không sinh một NPC rồi đặt vào làng.

Phải làm ngược lại.

Ví dụ làng có:

132 hộ.

40 hộ làm ruộng.

20 hộ đánh cá.

7 hộ làm gốm.

4 hộ làm nghi lễ.

2 gia đình giàu.

1 gia đình giữ bến.

Có tranh chấp với làng bên.

Năm ngoái bị lũ.

Một trận dịch cách đây 11 năm.

Thế hệ trẻ đang bỏ làng đến Phong Châu.

Từ **Village State** đó mới sinh character.

Như vậy nếu game tạo:

> nữ, 23 tuổi, thợ gốm, cha mất.

Hệ thống phải biết:

cha cô ấy là ai,

ông chết khi nào,

nhà nào nung gốm,

ai dạy cô,

cô bán gốm cho ai.

Đây là điểm sẽ biến game của chúng ta từ procedural RPG thành **simulated narrative world**.

---

<a id="m02-t005-s036"></a>

## XXIII. Mỗi character cần ít nhất một connection

Không có NPC cô lập.

Mỗi character phải có tối thiểu:

**1 người thân**

**1 quan hệ xã hội**

**1 quan hệ cảm xúc**

Ví dụ:

Anh trai của A.

Học nghề từ B.

Ghét C.

Nợ D.

Yêu E nhưng không nói.

Từng cứu F.

Sau một thời gian game sẽ tạo thành **Social Graph**.

Và quest có thể mọc trực tiếp từ graph này.

A mất tích.

B nhờ tìm.

C không muốn A quay lại.

D cần A trả nợ.

E bí mật đi theo party.

F biết nơi A thường đến.

Không cần writer viết quest từ đầu.

---

<a id="m02-t005-s037"></a>

## XXIV. 1.000 character vì thế sẽ không phải content chết

Một NPC có thể:

trở thành follower,

thành enemy,

thành mentor,

chết,

lập gia đình,

rời làng,

được bầu làm trưởng làng,

trở thành Vu Chúc,

gia nhập quân đội,

phản bội faction,

hoặc trở thành boss.

Character không có vai trò cố định.

Một người đánh cá level 1 ở hour 3 hoàn toàn có thể trở thành một nhân vật cực kỳ quan trọng ở hour 70.

---

<a id="m02-t005-s038"></a>

## XXV. Quy mô World Bible tôi muốn hướng đến

Nếu chúng ta làm project này đến đúng độ sâu mà bạn đang yêu cầu, phiên bản hoàn chỉnh nên có cỡ:

| Nội dung | Quy mô mục tiêu |
|---|---:|
| Đại vùng | 12 |
| Tiểu vùng | 40–60 |
| Settlement quan trọng | 60–80 |
| Làng/bến/trại nhỏ | 150+ |
| Faction lớn | 15–20 |
| Faction nhỏ | 50+ |
| Dòng họ/gia tộc đáng chú ý | 100+ |
| Nghề / social role | 60+ |
| Background | 100+ |
| Character life event | 200+ |
| Personality component | 100+ |
| Belief / taboo | 100+ |
| Unique passive component | 300+ |
| Named NPC khả dụng | 1.000+ |
| Minor procedural NPC | không giới hạn |
| Myth / legend | 100+ |
| Dungeon | 100–150 |
| Major questline | 30–50 |
| Side quest | 300+ |
| Micro-event | 1.000+ |

Như vậy **1.000 character không phải mục tiêu tối đa**. Nó chỉ là mức content đầu tiên mà world engine có thể hỗ trợ.

---

<a id="m02-t005-s039"></a>

### Một quyết định thiết kế tôi muốn khóa ngay

Tôi nghĩ thế giới nên dùng mô hình:

> **Lịch sử có “điểm neo”, nhưng câu chuyện cá nhân được tự do thay đổi.**

Ví dụ sự xuất hiện của Âu Lạc và Cổ Loa là **Anchor Event**.

Nhưng player có thể khiến:

một ngôi làng tồn tại hay biến mất,

một Lạc tướng sống hay chết,

một dòng họ hưng thịnh hay suy tàn,

một thần bị lãng quên hay tiếp tục được thờ,

một nhân vật trở thành anh hùng hay phản đồ.

Nhờ đó ta vừa kể được **Văn Lang – Âu Lạc**, vừa giữ đúng tinh thần D&D:

> **đây là câu chuyện của nhân vật mà người chơi tạo ra, không phải người chơi đứng bên cạnh nhìn các nhân vật lịch sử diễn.**

Và từ nền này, bước tiếp theo tôi nghĩ nên làm **cực sâu 12 đại vùng của thế giới**: chia bản đồ, settlement, văn hóa, kinh tế, tín ngưỡng, tập tục, xung đột, truyền thuyết, quái vật, dungeon, faction và loại nhân vật có thể sinh ra ở từng vùng. Khi 12 vùng này hoàn chỉnh, chúng ta mới thực sự có cái “máy” đủ sức sinh 1.000 nhân vật mà không có cảm giác clone.

<!-- SOURCE T005 END -->

<!-- SOURCE T007 BEGIN -->

Được. Tôi sẽ khóa **bản đồ thế giới cấp 1** trước. Mục tiêu của bước này không phải chỉ để có 12 khu vực để đi lại, mà để mỗi khu vực trở thành một **nguồn sinh quest + NPC + class + equipment + passive + faction + dungeon + truyền thuyết** riêng.

Một lưu ý quan trọng trước khi đi vào thiết kế: các tên như **Tam Giang, Đồng Đỏ, Đại Trạch... bên dưới là địa danh game canon**, không khẳng định đây là đơn vị hành chính lịch sử. Phần nền sẽ bám các điểm neo có cơ sở: truyền thống đặt Văn Lang ở Phong Châu và sự chuyển sang Âu Lạc/Cổ Loa; khảo cổ Đông Sơn cho thấy một đời sống phong phú với nông nghiệp, thuyền bè, nghi lễ, chiến binh, luyện kim; Cổ Loa có bằng chứng về lẫy nỏ, khuôn và số lượng lớn mũi tên đồng. Đồng thời, chính các nguồn nghiên cứu cũng lưu ý rằng nhiều truyền thuyết Hùng Vương được ghi chép thành văn muộn hơn rất nhiều. Vì vậy game sẽ chủ động tách **khảo cổ – truyền thuyết – game canon**. 

<a id="m02-t007-s001"></a>

## WORLD BIBLE 01 — MƯỜI HAI ĐẠI VÙNG

<a id="m02-t007-s002"></a>

### Bản đồ khái niệm

| # | Đại vùng | Fantasy cốt lõi | Gameplay đặc trưng |
|---|---|---|---|
| 01 | **Phong Châu** | Trái tim Văn Lang | chính trị, xã hội, nghi lễ |
| 02 | **Tam Giang** | Xứ của ba dòng nước | thuyền, lũ, thủy linh |
| 03 | **Tản Viên** | Núi thiêng phương Tây | leo núi, săn thú, Sơn thần |
| 04 | **Bãi Lạc** | Đồng bằng trù phú | nông nghiệp, làng xã, tranh đất |
| 05 | **Đồng Đỏ** | Đất của lửa và kim loại | luyện kim, khai mỏ, crafting |
| 06 | **Lâm U** | Rừng cổ không thuộc về con người | exploration, dược liệu, linh giới |
| 07 | **Thượng Âu** | Cao địa của người Âu | bộ tộc, chính trị, chiến tranh |
| 08 | **Đại Trạch** | Đầm nước không đáy | survival, độc, bệnh, horror |
| 09 | **Hải Môn** | Nơi sông gặp biển | hàng hải, thương mại, bão |
| 10 | **Việt Thường** | Phương Nam xa xôi | exploration, giao lưu văn hóa |
| 11 | **Bắc Lộ** | Biên địa luôn biến động | quân sự, tình báo, ngoại giao |
| 12 | **Loa Vực** | Vùng đất sẽ trở thành Cổ Loa | xây thành, công nghệ, đại chiến |

Điểm quan trọng là **không region nào thay thế region khác được**.

Một nhân vật sinh ở Tam Giang sẽ không có cảm giác giống nhân vật sinh tại Tản Viên chỉ vì đổi background.

---

<a id="m02-t007-s003"></a>

## 01. PHONG CHÂU

<a id="m02-t007-s004"></a>

### “Nơi trống đồng gọi người về”

Đây là trung tâm thế giới ở đầu game.

Không nên thiết kế Phong Châu như một “kinh thành medieval”.

Nó nên giống một **quần thể chính trị – nghi lễ – thương mại**, gồm khu cư trú, bến sông, nhà của các dòng họ quyền lực, nơi hội họp, xưởng thủ công, kho lương, khu nghi lễ và những cộng đồng xung quanh.

Phong Châu được game hóa từ truyền thống gắn trung tâm Văn Lang với vùng Phú Thọ ngày nay. 

<a id="m02-t007-s005"></a>

#### Cấu trúc xã hội

Đây là nơi tập trung:

Hùng Vương.

Các nhánh gia tộc liên quan Hùng tộc.

Lạc hầu.

Lạc tướng đến dự hội.

Người giữ trống.

Vu chúc.

Sứ giả.

Thợ thủ công giỏi.

Con tin chính trị.

Người hầu.

Thương nhân.

Dân từ các vùng khác đến kiếm sống.

Điểm độc đáo của Phong Châu là:

> **rất ít người thực sự là người Phong Châu thuần túy.**

Đây là melting pot đầu tiên của game.

---

<a id="m02-t007-s006"></a>

#### Conflict

Phong Châu tưởng mạnh nhưng đang mục từ bên trong.

Hùng Vương đã già.

Chưa có đồng thuận về người sẽ kế tục.

Một số Lạc tướng bắt đầu không còn phục tùng.

Các kho lương bị rút ruột.

Đồ cống nạp ngày càng ít.

Có người cho rằng:

> Hùng triều đã không còn được trời đất che chở.

Một nhóm khác lại nói:

> Không có thần nào cả. Chỉ là triều đình yếu đi.

Ngay ở vùng đầu tiên người chơi đã được tiếp xúc với conflict trung tâm:

**Truyền thống hay thay đổi?**

---

<a id="m02-t007-s007"></a>

#### Các địa điểm chủ chốt

**Đồi Trống** là nơi tổ chức đại lễ.

**Bến Hạc** là bến lớn nhất.

**Xóm Đồng Văn** tập trung nghệ nhân.

**Khu Nhà Dài** là nơi cư trú của các dòng họ quyền lực.

**Mộ Tổ Tây Gò** chứa các mộ cổ.

**Chợ Năm Dòng** là khu trao đổi vật phẩm từ các vùng.

**Rừng Cấm Hạc Trắng** không ai được phép săn.

**Hang Thề** là nơi các Lạc tướng từng lập lời thề với Hùng triều.

Hang Thề sau này sẽ trở thành dungeon cực kỳ quan trọng.

---

<a id="m02-t007-s008"></a>

#### Character pool

Phong Châu sinh được những character như:

con thứ của Lạc tướng;

con ngoài giá thú của gia đình quyền lực;

người giữ kho;

thợ khắc trống;

người hầu biết quá nhiều bí mật;

sứ giả;

võ sĩ nghi lễ;

người đọc điềm;

thương nhân ngoại vùng;

kẻ trộm chuyên sống quanh hội lớn.

Signature Passive đặc trưng có thể như:

**Nhớ Mặt Người Dự Hội** — những NPC từng gặp trong nghi lễ lớn khó che giấu thân phận trước nhân vật.

**Nghe Tiếng Trống Lạc Nhịp** — có khả năng nhận ra lỗi trong nghi lễ hoặc tín hiệu quân sự bằng trống.

**Sinh Ra Giữa Quyền Lực** — có thể nhận biết tương đối địa vị xã hội của NPC qua cách họ được người khác đối xử.

**Biết Cách Cúi Đầu** — chịu ít penalty hơn khi đối thoại với người có địa vị cao nhưng dễ bị NPC phản quyền lực đánh giá tiêu cực.

---

<a id="m02-t007-s009"></a>

## 02. TAM GIANG

<a id="m02-t007-s010"></a>

### “Không ai sở hữu dòng sông”

Tam Giang là mạng lưới sông, bãi, cù lao, bến nhỏ.

Không có đường biên rõ ràng.

Có những làng mùa khô đi bộ được tới.

Mùa mưa phải đi thuyền.

Một số bãi đất thậm chí biến mất hoàn toàn.

Vì vậy người Tam Giang không nghĩ về lãnh thổ giống người Phong Châu.

Họ nghĩ:

> “Nước năm nay đi đến đâu?”

---

<a id="m02-t007-s011"></a>

### Văn hóa

Trẻ con học:

bơi;

buộc thuyền;

đọc dòng nước;

nhận biết mưa;

đánh cá.

Trước khi được coi là trưởng thành.

Người chết trên đất và người chết dưới nước có tục tang khác nhau.

Người mất xác đặc biệt đáng sợ.

Bởi theo tín ngưỡng địa phương:

> Người chết mà không tìm được xác sẽ không biết đường về với tổ tiên.

Từ đây sinh ra rất nhiều quest.

---

<a id="m02-t007-s012"></a>

### Đại conflict

Nước bắt đầu lên bất thường.

Một số bến cũ biến mất.

Cá di cư.

Đàn chim thay đổi đường bay.

Có làng nói:

> Thủy thần nổi giận.

Có người cho rằng một Lạc tướng phía thượng nguồn đang cho người thay đổi dòng nước.

Có người lại tin rằng đây là dấu hiệu:

**Ước Sơn Hà đang vỡ.**

---

<a id="m02-t007-s013"></a>

### Faction

**Hội Người Đưa Đò** kiểm soát thông tin dọc các dòng sông.

**Dòng họ Vạc** nổi tiếng đánh cá lớn.

**Những Người Không Bờ** sống trên thuyền và không nhận bất kỳ làng nào làm quê.

**Miếu Tam Thủy** tập hợp những người phụng sự thần nước.

**Bọn Cờ Lau** thực chất là mạng lưới cướp sông.

---

<a id="m02-t007-s014"></a>

### Dungeon

Dungeon của Tam Giang có thể là:

thuyền chìm;

đảo nổi;

hang dưới vách sông;

ngôi làng bị ngập;

mộ thuyền;

miếu chỉ xuất hiện vào mùa nước thấp.

Mộ thuyền không hoàn toàn là fantasy tùy tiện: mộ thuyền và đồ tùy táng Đông Sơn là chất liệu khảo cổ có thật, cực kỳ phù hợp để phát triển hệ thống tang lễ và dungeon. 

---

<a id="m02-t007-s015"></a>

### Character

Đây là quê hương của:

người chèo thuyền;

đánh cá;

thợ lặn;

người dẫn đường;

thủy binh;

cướp sông;

vu nước;

người tìm xác.

Passive:

**Nghe Nước Đổi Giọng** — phát hiện thay đổi nguy hiểm của dòng nước.

**Không Chết Lần Hai** — nhân vật từng chết đuối hụt có resistance cao với panic trong môi trường nước nhưng lại chịu stress nếu bị giữ dưới nước.

**Mắt Người Chài** — quan sát mặt nước có thể phát hiện chuyển động bên dưới.

**Không Cần Bờ** — nghỉ trên thuyền gần tương đương nghỉ tại settlement.

---

<a id="m02-t007-s016"></a>

## 03. TẢN VIÊN

<a id="m02-t007-s017"></a>

### “Núi không phục tùng vua”

Tản Viên là một trong những region lore quan trọng nhất.

Đây là quê hương của rất nhiều truyền thuyết về núi và sẽ là trung tâm của arc Sơn Tinh – Thủy Tinh.

Nhưng chúng ta sẽ không biến Sơn Tinh đơn giản thành một NPC level 99.

**Sơn Tinh trước hết là một danh xưng.**

Người chơi không biết:

Có thật một người duy nhất tên Sơn Tinh?

Hay nhiều đời người giữ núi đều mang danh hiệu đó?

Hay đó thực sự là một Sơn thần?

Game không trả lời ngay.

---

<a id="m02-t007-s018"></a>

### Xã hội

Dân cư sống phân tán.

Một làng 40 hộ đã được xem là lớn.

Quan hệ dựa trên:

dòng họ;

lời thề;

trao đổi;

quan hệ với rừng.

Họ không đặc biệt quan tâm ai đang đấu đá tại Phong Châu.

Đối với họ:

> mùa săn quan trọng hơn người nào đang làm Lạc hầu.

---

<a id="m02-t007-s019"></a>

### Economy

Gỗ.

Thảo dược.

Mật ong.

Da thú.

Sừng.

Khoáng vật.

Dược liệu hiếm.

Những thứ này khiến thương nhân dưới đồng bằng thường xuyên lên núi.

Và thường xuyên biến mất.

---

<a id="m02-t007-s020"></a>

### Faction

**Người Giữ Đỉnh**

**Hội Thợ Săn Sừng Trắng**

**Dòng họ Cây Đá**

**Các Trại Mật**

**Tín đồ Sơn Chủ**

Một số hoàn toàn không công nhận quyền lực Hùng Vương.

---

<a id="m02-t007-s021"></a>

### Quái và linh thể

Hổ lớn.

Gấu.

Rắn.

Lợn rừng.

Sau đó mới tới supernatural:

Mộc tinh.

Sơn tiêu.

Linh hươu.

Người đá.

Thần khe.

Nhưng tôi muốn áp dụng một luật:

> Không phải sinh vật lạ nào cũng phải giết.

Một con hổ già có thể là boss.

Nhưng giết nó khiến đàn lợn rừng tăng mạnh và phá mùa màng.

---

<a id="m02-t007-s022"></a>

### Character

Sơn dân.

Thợ săn.

Người lấy mật.

Người tìm dược.

Kẻ sống ẩn.

Người canh đường núi.

Vu Sơn.

Passive:

**Đọc Rừng** — khu rừng để lại dấu vết như một văn bản.

**Bước Không Gãy Cành** — giảm khả năng bị phát hiện trong rừng.

**Mùi Mưa Trên Đá** — dự báo mưa ở vùng núi.

**Thú Không Sợ Ta** — động vật trung lập ít có xu hướng chủ động tấn công nhân vật.

---

<a id="m02-t007-s023"></a>

## 04. BÃI LẠC

<a id="m02-t007-s024"></a>

### “Vùng đất nuôi người và giết người”

Đây là heartland nông nghiệp.

Ở đây không có quá nhiều quái vật khổng lồ.

Nhưng lại có thể chứa **nhiều quest nhất game**.

Bởi đây là nơi đông người nhất.

---

<a id="m02-t007-s025"></a>

### Core fantasy

Ruộng lúa.

Kênh.

Bờ đất.

Làng.

Kho lương.

Trâu.

Cá đồng.

Lễ mùa.

Cưới hỏi.

Tranh ruộng.

Nợ.

Gia đình.

Đây sẽ là vùng khiến người chơi nhận ra:

> thế giới này không tồn tại để player phiêu lưu.

Người dân đang thực sự sống trong đó.

Hình ảnh trên trống Đông Sơn cho thấy những cảnh sinh hoạt, đánh trống, giã gạo, lễ hội, thuyền bè và động vật; đó là một nguồn trực quan rất tốt cho việc thiết kế đời sống thường nhật thay vì chỉ tập trung chiến tranh. 

---

<a id="m02-t007-s026"></a>

### Conflict

Một trận lũ có thể:

phá ruộng A;

nhưng bồi phù sa cho ruộng B.

Một con đê cứu làng này nhưng đẩy nước sang làng khác.

Do đó rất nhiều conflict không có đáp án đẹp.

Ví dụ quest:

> Hai làng yêu cầu player quyết định phá một bờ đất.

Nếu phá:

Làng A được cứu.

Làng B bị ngập.

Không phá:

cả hai thiệt hại nhưng nhẹ hơn.

Nếu có Engineering cao:

có giải pháp thứ ba.

Nếu có knowledge Tam Giang:

tìm được dòng thoát nước cũ.

Gameplay từ character build trực tiếp thay đổi narrative.

---

<a id="m02-t007-s027"></a>

## 05. ĐỒNG ĐỎ

<a id="m02-t007-s028"></a>

### “Nơi đá biến thành quyền lực”

Nếu Phong Châu là political power thì Đồng Đỏ là **material power**.

Ai kiểm soát đồng:

kiểm soát công cụ.

Vũ khí.

Trống.

Nông cụ.

Uy tín.

---

<a id="m02-t007-s029"></a>

### Gameplay identity

Mining.

Metallurgy.

Crafting.

Trade.

Industrial espionage.

Sabotage.

Một player thích equipment sẽ có thể dành hàng chục giờ ở đây.

---

<a id="m02-t007-s030"></a>

### Social conflict

Thợ giỏi không nhất thiết có địa vị cao.

Người sở hữu mỏ lại có quyền lực.

Xuất hiện ba tầng:

người đào quặng;

người luyện;

người tạo vật phẩm.

Mỗi tầng giữ bí mật nghề nghiệp.

Một số kỹ thuật chỉ truyền trong dòng họ.

---

<a id="m02-t007-s031"></a>

### Một lore quan trọng

Người ta tin:

> Kim loại lấy ra từ lòng đất là xương của núi.

Nếu đào quá sâu:

núi sẽ nhớ.

Đây có thể chỉ là tín ngưỡng.

Hoặc không.

---

<a id="m02-t007-s032"></a>

### Character

Thợ đúc.

Thợ than.

Người khai mỏ.

Người kiểm quặng.

Thương nhân đồng.

Kẻ ăn trộm bí quyết.

Passive:

**Tai Người Đúc** — nghe tiếng kim loại để đánh giá chất lượng.

**Mắt Tìm Quặng** — phát hiện dấu hiệu khoáng vật.

**Tay Quen Lửa** — crafting failure đầu tiên ít hậu quả hơn.

**Không Tin Ánh Đồng** — phát hiện đồ vật được đánh bóng nhằm che khuyết tật.

---

<a id="m02-t007-s033"></a>

## 06. LÂM U

<a id="m02-t007-s034"></a>

### “Có những khu rừng chưa từng thuộc về con người”

Nếu Tản Viên là wilderness có cộng đồng con người, Lâm U phải là nơi con người **không phải species thống trị**.

Rất ít settlement.

Không đường rõ ràng.

Thậm chí bản đồ có thể thay đổi.

---

<a id="m02-t007-s035"></a>

### Core gameplay

Exploration.

Navigation.

Herbalism.

Poison.

Spiritual encounter.

Psychological horror.

---

<a id="m02-t007-s036"></a>

### Quy tắc đặc biệt

Một số nơi player không được nhận minimap đầy đủ.

Nhân vật có Survival thấp có thể:

đánh dấu sai landmark.

nhớ sai đường.

đi vòng.

Nhân vật sinh ở Lâm U lại gần như miễn cơ chế này.

---

<a id="m02-t007-s037"></a>

### Lore

Người dân tin rằng cây già có ký ức.

Không phải cây nào cũng có linh.

Nhưng một cây sống qua hàng trăm mùa có thể hấp thu đủ **Mạch Lạc** để hình thành thứ gần giống ý thức.

Dungeon vì vậy có thể là:

**một cây.**

Không phải hang dưới cây.

Mà toàn bộ network rễ và ký ức của nó.

---

<a id="m02-t007-s038"></a>

## 07. THƯỢNG ÂU

<a id="m02-t007-s039"></a>

### “Không phải mọi người phương Bắc đều là kẻ thù”

Đây là region chính trị quan trọng nhất cho nửa giữa game.

Người chơi ban đầu ở Văn Lang thường được nghe:

> người Âu đang xuống phía nam.

Khi tới đây họ phát hiện:

không tồn tại một “phe Âu” thống nhất.

Có hàng chục cộng đồng.

Nhiều cộng đồng thù nhau.

Một số giao thương với Lạc Việt từ nhiều đời.

Một số có quan hệ hôn nhân.

Một số muốn chiến tranh.

Một số không hề quan tâm Văn Lang.

---

<a id="m02-t007-s040"></a>

### Gameplay

Tribal politics.

Diplomacy.

Mountain warfare.

Intercultural relations.

Player bắt đầu hiểu Thục Phán từ đây.

Không thông qua exposition.

Mà thông qua xã hội đã tạo ra ông.

---

<a id="m02-t007-s041"></a>

### Character

Chiến binh cao địa.

Người nuôi ngựa nhỏ.

Người vận chuyển đường núi.

Trinh sát.

Thủ lĩnh thị tộc.

Người phiên dịch.

Passive:

**Qua Đèo Không Nghỉ.**

**Nhớ Mùi Khói Trại.**

**Nói Hai Tiếng** — advantage trong giao tiếp liên vùng.

**Luật Của Khách** — hiểu taboo khi bước vào cộng đồng xa lạ.

---

<a id="m02-t007-s042"></a>

## 08. ĐẠI TRẠCH

<a id="m02-t007-s043"></a>

### “Nơi đất chưa quyết định mình là đất hay nước”

Một trong những region tôi muốn làm cực kỳ dark.

Đầm.

Sương.

Lau.

Bùn.

Nước đen.

Rừng ngập.

Côn trùng.

Rắn.

Cá lớn.

---

<a id="m02-t007-s044"></a>

### Horror ở đây không dựa vào jumpscare

Người chơi nghe:

“Đừng bước vào vùng nước không có côn trùng.”

Nhưng không biết tại sao.

Sau vài giờ chơi mới hiểu:

nơi côn trùng im tiếng thường có thứ săn chúng.

---

<a id="m02-t007-s045"></a>

### Gameplay

Poison.

Disease.

Navigation.

Ambush.

Resource management.

Party exhaustion.

Một party combat cực mạnh vẫn có thể chết ở Đại Trạch vì:

không có thuốc;

không biết nước nào uống được;

không nhận ra vùng bùn lún.

---

<a id="m02-t007-s046"></a>

### Character

Người bắt rắn.

Người hái thuốc.

Người dựng bè.

Thợ săn đầm.

Người đốt than lau.

Người tìm người mất tích.

Passive:

**Côn Trùng Im Tiếng.**

**Bùn Nhớ Dấu.**

**Không Uống Nước Chết.**

**Da Quen Độc.**

Tên passive chính là một mảnh culture.

---

<a id="m02-t007-s047"></a>

## 09. HẢI MÔN

<a id="m02-t007-s048"></a>

### “Phía sau đường chân trời vẫn còn người”

Đây là nơi phá bỏ cảm giác Văn Lang là toàn bộ thế giới.

Có thuyền đến từ nơi xa.

Không phải lúc nào player cũng hiểu tiếng họ.

Có vật phẩm chưa từng thấy.

Hạt.

Đá.

Trang sức.

Vỏ sò.

Kỹ thuật.

Tin đồn.

---

<a id="m02-t007-s049"></a>

### Social fantasy

Ngư dân.

Người làm muối.

Đóng thuyền.

Hoa tiêu.

Thương nhân.

Cướp biển nhỏ.

Các cộng đồng đảo.

---

<a id="m02-t007-s050"></a>

### Myth

Biển không chỉ có “thủy quái”.

Có:

Cá Ông dạng myth cổ của game.

Rùa lớn.

Rắn biển.

Đảo xuất hiện theo mùa.

Gió có tên.

Một số ngư dân tin:

> mỗi người chỉ được biển cứu một lần.

Nếu đã sống sót một lần:

không nên thử lần thứ hai.

Từ đó có passive:

<a id="m02-t007-s051"></a>

#### Biển Đã Trả Ta Lại

Một lần duy nhất trong toàn bộ character life, nhân vật có thể sống sót khỏi một catastrophe trên biển mà đáng lẽ chết.

Sau khi trigger:

passive biến mất.

Đó mới thực sự là **unique passive**, không phải +5%.

---

<a id="m02-t007-s052"></a>

## 10. VIỆT THƯỜNG

<a id="m02-t007-s053"></a>

### “Phía Nam của những câu chuyện”

Đây không cần cố xác định Việt Thường lịch sử bằng game canon. Chính các truyền thống và diễn giải về Việt Thường rất phức tạp; nguồn bảo tàng cũng trình bày những truyền thuyết khác nhau liên quan khu vực phương Nam. 

Trong game, **Việt Thường là tên người Văn Lang dùng cho một frontier văn hóa phía Nam**, không phải một tuyên bố lịch sử.

---

<a id="m02-t007-s054"></a>

### Vai trò

Đưa người chơi ra khỏi political center.

Cho thấy những cộng đồng:

ăn khác;

mặc khác;

chôn người khác;

có tín ngưỡng khác;

nhưng vẫn có rất nhiều điểm tương đồng.

Từ đây mở cửa cho expansion sau này.

---

<a id="m02-t007-s055"></a>

## 11. BẮC LỘ

<a id="m02-t007-s056"></a>

### “Chiến tranh luôn đến trước tin tức”

Đây là frontier military.

Không có một đường biên được vẽ.

Một ngôi làng hôm nay thuộc ảnh hưởng Lạc.

Năm sau đóng cống cho thế lực khác.

---

<a id="m02-t007-s057"></a>

### Gameplay

Reconnaissance.

Espionage.

Counter-intelligence.

Battlefield.

Supply.

Prisoner exchange.

Assassination.

Diplomacy.

---

<a id="m02-t007-s058"></a>

### Một cơ chế quan trọng

Ở Bắc Lộ, **information becomes currency**.

Ví dụ player biết:

một đoàn quân sẽ qua đèo sau ba ngày.

Có thể:

bán cho Lạc tướng;

bán cho thương nhân;

cảnh báo dân;

bán cho đối phương;

hoặc giữ kín.

Cùng một information tạo nhiều quest branch.

---

<a id="m02-t007-s059"></a>

## 12. LOA VỰC

<a id="m02-t007-s060"></a>

### “Một thành phố chưa tồn tại”

Đây là region đặc biệt nhất.

Khi player lần đầu đến:

**Cổ Loa chưa tồn tại ở hình dạng mà lịch sử nhớ tới.**

Chỉ có settlement.

Ruộng.

Đường nước.

Các gò.

Làng.

---

<a id="m02-t007-s061"></a>

## Sau đó thế giới thay đổi

Act III:

người khảo sát xuất hiện.

Act IV:

đào đất.

Act IV giữa:

dân lao động được đưa tới.

Act IV cuối:

thành bắt đầu thành hình.

Act V:

xưởng vũ khí.

Act VI:

Cổ Loa trở thành trung tâm quyền lực.

Player có thể đứng tại **cùng một tọa độ** ở nhiều giai đoạn game và nhìn thấy thế giới biến đổi.

Đây sẽ là một trong những điểm mạnh nhất của game.

Cổ Loa cũng là nơi archaeological anchor của chúng ta mạnh nhất: hiện vật Đông Sơn tại đây bao gồm nông cụ, lẫy nỏ, khuôn đúc mũi tên và lượng lớn mũi tên đồng; vì vậy game hoàn toàn có cơ sở để biến nơi này thành hub của kỹ thuật quân sự, luyện kim và tổ chức lao động quy mô lớn. 

---

<a id="m02-t007-s062"></a>

## Hệ thống liên kết 12 vùng

Đây mới là phần làm cho world sống.

Không vùng nào tự cung tự cấp.

Tôi muốn economy cơ bản vận hành theo mạng:

**Tản Viên / Lâm U**

→ gỗ, da, dược liệu

↓

**Đồng Đỏ**

→ kim loại, công cụ

↓

**Bãi Lạc**

→ lương thực

↓

**Tam Giang**

→ vận chuyển

↓

**Phong Châu**

→ phân phối + chính trị

↓

**Bắc Lộ / Thượng Âu**

→ trade + military

↓

**Hải Môn**

→ muối + hàng ngoại

Khi một mắt xích chết, toàn bộ world state thay đổi.

Ví dụ Tam Giang bị chiến tranh:

Đồng Đỏ không chuyển được công cụ.

Bãi Lạc thiếu nông cụ.

Giá lương thực tăng.

Phong Châu xuất hiện bất ổn.

Quân tại Bắc Lộ thiếu lương.

Không cần main quest can thiệp.

**World simulation tự tạo quest.**

---

<a id="m02-t007-s063"></a>

## Đây là nền tảng để sinh 1.000 character

Bây giờ chúng ta đã có một lợi thế lớn.

Không còn sinh:

> Human / Male / Warrior / 24 tuổi.

Mà sẽ sinh:

> **Thượng Âu → Khe Đá Trắng → dòng họ săn núi → con thứ hai → cha là người dẫn đường → mất mẹ trong mùa rét → từng sống 3 năm tại Tam Giang → nói được hai phương ngữ → không tin Vu Chúc → mắc nợ một thương nhân Phong Châu → căm ghét chiến tranh nhưng cực giỏi phục kích.**

Character này **không thể xuất hiện y nguyên lần thứ hai**.

Bởi biography đã nằm trong world graph.

---

<a id="m02-t007-s064"></a>

## Region Genome

Từ 12 vùng này, mỗi character trước tiên sẽ nhận một **Region Genome** gồm:

| Thuộc tính | Ví dụ |
|---|---|
| Birth Region | Tam Giang |
| Birth Settlement | Bến Vạc |
| Cultural Group | Dân bến |
| Environment Knowledge | River |
| Native Skill | Swimming |
| Regional Belief | Water Spirit |
| Regional Taboo | Không gọi tên người chết trên sông |
| Food Memory | Cá nướng lá |
| Childhood Game | đua bè |
| Regional Enemy | cướp sông |
| Regional Ally | người đưa đò |
| Accent/Dialect | Tam Giang |
| Local Knowledge | 4 |
| Outside Knowledge | 0 |
| Passive Seed | Water |
| Story Seed | mất anh trong lũ |

Sau đó mới chạy tiếp:

**Family Genome → Childhood → Life Event → Personality → Occupation → Belief → Relationship → Trauma → Desire → Contradiction → Signature Passive.**

Đây là pipeline tôi muốn sử dụng cho toàn bộ 1.000 character.

---

<a id="m02-t007-s065"></a>

## Và có một thay đổi tôi nghĩ nên khóa ngay

Từ giờ **NPC không được random region độc lập với nghề**.

World simulation phải sinh settlement trước.

Ví dụ:

> Bến Vạc có 86 hộ.

Trong đó:

31 hộ đánh cá.

12 hộ làm thuyền.

8 hộ chuyên vận chuyển.

4 hộ làm nghi lễ.

9 hộ trồng lúa.

6 hộ buôn.

16 hộ hỗn hợp.

Game sau đó mới sinh từng gia đình.

Rồi từ gia đình sinh:

cha.

mẹ.

con.

họ hàng.

mentor.

enemy.

lover.

debtor.

creditor.

Như vậy khi NPC nói:

> “Cha tôi đóng chiếc thuyền này.”

Game thật sự có một NPC cha.

Nếu cha đã chết:

có năm chết.

nguyên nhân.

mộ.

người dự tang.

ảnh hưởng đến những người còn sống.

Đây chính là mức độ sâu mà tôi nghĩ concept của bạn cần hướng tới.

---

<a id="m02-t007-s066"></a>

### Bước tiếp theo tôi đề xuất

**Không nên chuyển sang class ngay.**

Chúng ta đã có **12 đại vùng**, nhưng vẫn còn thiếu lớp quyết định chất lượng của 1.000 nhân vật:

> **Settlement → Family → Culture → Social Graph.**

Bước kế tiếp nên xây **hệ thống settlement của 12 vùng**: khoảng **60–80 settlement quan trọng**, mỗi nơi có dân số, nghề nghiệp, dòng họ, tín ngưỡng, lịch sử, conflict, secret, faction, dungeon liên quan và loại character có thể sinh ra.

Đó sẽ là lớp dữ liệu mà sau này character generator thực sự “ăn” vào. Khi làm đến đó, một NPC sẽ không còn được tạo từ prompt; nó sẽ được **sinh ra từ thế giới**.

<!-- SOURCE T007 END -->

