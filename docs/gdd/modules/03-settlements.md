# Module 03 — Settlement và mô phỏng cộng đồng

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T009](../../source/conversation.md#t009)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

60 settlement là thiết kế canon, chưa phải 60 scene hay 60 entity runtime đã tạo. Lưu identity và trạng thái thay đổi theo thời gian; không xây inventory shop từ hư không.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Region, household, hạ tầng, nguồn lực, mùa và xung đột. |
| Đầu ra / bên sở hữu | Settlement state, infrastructure, production capacity; Settlement phối hợp Economy/Assets. |
| Đọc cùng | 02, 04, 14, 18, 26. |
| Cổng nghiệm thu | Đứt route hoặc mất worker làm thay đổi năng lực sản xuất và hậu quả có thể trace. |
| Việc chưa được chứng minh | Dân số và topology production; aggregate resolution cần benchmark. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T009 BEGIN -->

<a id="m03-t009-s001"></a>

## WORLD BIBLE 02 — HỆ THỐNG SETTLEMENT

Từ bước này, tôi coi toàn bộ tên là **game canon** trừ những địa danh lịch sử được nói rõ như Phong Châu/Cổ Loa. Mục tiêu không phải phục dựng một bản đồ khảo cổ chính xác tuyệt đối, mà tạo ra một thế giới fantasy-lịch sử có logic nội tại, dựa trên nền vật chất phù hợp thời Đông Sơn.

Bằng chứng khảo cổ cho phép chúng ta xây settlement theo hướng rất thú vị: nhà sàn phù hợp vùng thấp và sông nước, có cộng đồng sống/giao thông bằng thuyền, nông nghiệp lúa nước, nghề luyện kim phát triển, nhiều loại công cụ–vũ khí–nhạc khí; tại Cổ Loa còn có dấu vết khuôn đúc mũi tên và nông cụ đồng. 

Các con số hộ dân bên dưới là **tham số thiết kế game**, không phải khẳng định dân số lịch sử.

---

<a id="m03-t009-s002"></a>

## 1. Quy tắc chung của settlement

Mỗi settlement tồn tại dưới dạng một entity thực sự trong World State:

**Settlement**
→ households  
→ families  
→ individual characters  
→ occupations  
→ resources  
→ beliefs  
→ relationships  
→ conflicts  
→ secrets  
→ historical events.

Ví dụ không có chuyện game tạo NPC:

> “Lan, 22 tuổi, người Bến Vạc, cha đã chết.”

Nếu Lan tồn tại thì database phải có hoặc từng có:

**Cha của Lan → nhân vật cụ thể → nghề → ngày chết → nguyên nhân → người dự tang → tài sản để lại → những NPC bị ảnh hưởng bởi cái chết đó.**

Đây sẽ là nguyên tắc rất quan trọng.

---

<a id="m03-t009-s003"></a>

## 2. Năm cấp settlement

| Cấp | Quy mô game | Chức năng |
|---|---:|---|
| Regional Hub | 300–700+ hộ | trung tâm cả vùng |
| Major Settlement | 150–350 hộ | thương mại/chính trị/nghề |
| Settlement | 70–180 hộ | cộng đồng thông thường |
| Hamlet | 20–70 hộ | cộng đồng chuyên biệt |
| Outpost/Camp | 5–30 hộ | săn bắn, mỏ, quân sự... |

Settlement có thể tăng hoặc giảm cấp theo World State.

Một Hamlet hoàn toàn có thể trở thành thị trấn quan trọng sau 15 năm.

---

<a id="m03-t009-s004"></a>

## I. PHONG CHÂU

Phong Châu có mật độ NPC xã hội cao nhất. Đây sẽ là region mạnh nhất cho **politics, dialogue, intrigue, information gameplay**.

---

<a id="m03-t009-s005"></a>

### PC-01 — PHONG CHÂU

**Regional Hub — khoảng 600–700 hộ đầu campaign**

Không phải một thành phố có tường kín mà là một quần thể cư trú nối với nhau bằng đường đất, bến nước và các khu chức năng.

<a id="m03-t009-s006"></a>

#### Khu vực

Trung tâm nghi lễ.

Khu cư trú của các gia đình quyền lực.

Khu thợ thủ công.

Kho lương.

Bến.

Khu dành cho khách ngoại vùng.

Các làng vệ tinh.

<a id="m03-t009-s007"></a>

#### Quyền lực

Hùng tộc không trực tiếp kiểm soát từng người.

Quyền lực được chia giữa:

Hùng Vương.

Lạc hầu.

Những dòng họ lâu đời.

Người giữ nghi lễ.

Các Lạc tướng khi họ có mặt tại kinh vực.

<a id="m03-t009-s008"></a>

#### Kinh tế

Phong Châu chủ yếu **tiêu thụ và phân phối**.

Gạo từ Bãi Lạc.

Kim loại từ Đồng Đỏ.

Dược liệu từ Tản Viên/Lâm U.

Muối từ Hải Môn.

Da thú từ miền núi.

<a id="m03-t009-s009"></a>

#### Current Conflict

Hùng Vương già.

Các Lạc tướng bắt đầu ít đến dự hội.

Kho lương thiếu hụt không giải thích được.

Một số vật phẩm nghi lễ biến mất.

<a id="m03-t009-s010"></a>

#### Secret

Một nhóm trong nội bộ Phong Châu đang cố tình khiến các địa phương tin rằng Hùng triều đã mất thiên mệnh.

Nhưng nhóm này không thống nhất mục tiêu.

Một số muốn cải cách.

Một số muốn thay vua.

Một số muốn phá bỏ toàn bộ cấu trúc cũ.

<a id="m03-t009-s011"></a>

#### Character Pool

Con nhà quyền lực.

Con tin chính trị.

Thư lại nguyên thủy/người giữ dấu.

Người hầu.

Vệ sĩ.

Sứ giả.

Vu Chúc.

Nghệ nhân.

Thương nhân.

Kẻ trộm thông tin.

---

<a id="m03-t009-s012"></a>

## PC-02 — BẾN HẠC

**Major Settlement — 280 hộ**

Cửa ngõ sông nước của Phong Châu.

Gần như mọi thứ lớn muốn vào Phong Châu đều phải qua đây.

<a id="m03-t009-s013"></a>

#### Cấu trúc

Ba bến chính.

Khu sửa thuyền.

Kho hàng.

Khu ngủ tập thể của phu vận chuyển.

Chợ nổi nhỏ.

<a id="m03-t009-s014"></a>

#### Faction

**Hội Bảy Mái Chèo** kiểm soát phần lớn vận tải.

Không phải gangster.

Họ giống một guild nghề nghiệp truyền đời.

<a id="m03-t009-s015"></a>

#### Taboo

Không huýt sáo khi thuyền đang rời bến.

Người dân tin đó là tiếng gọi gió ngược.

<a id="m03-t009-s016"></a>

#### Conflict

Thuyền chở đồng liên tục bị mất hàng nhưng không có dấu hiệu bị cướp.

<a id="m03-t009-s017"></a>

#### Secret

Một số phu bến đang vận chuyển vũ khí cho phe đối lập trong Phong Châu.

<a id="m03-t009-s018"></a>

#### Character Seed

Một cậu bé bốc hàng biết toàn bộ ký hiệu bí mật trên các kiện hàng nhưng không biết chúng có nghĩa gì.

Có thể trở thành NPC cực quan trọng nếu player chú ý.

---

<a id="m03-t009-s019"></a>

## PC-03 — ĐỒNG VĂN

**Major Craft Settlement — 190 hộ**

Nơi tập trung những nghệ nhân danh tiếng.

“Văn” ở đây là tên game canon, không mang nghĩa chữ viết theo cách hiện đại.

<a id="m03-t009-s020"></a>

#### Nghề

Khắc hoa văn.

Đồ gỗ.

Trang sức.

Chế tạo vật nghi lễ.

Sửa trống.

Trang trí cán vũ khí.

<a id="m03-t009-s021"></a>

#### Văn hóa

Nghệ nhân đánh giá người khác bằng **tay nghề**, không phải xuất thân.

Một người quyền lực nhưng không biết gì về nghề có thể bị coi thường.

<a id="m03-t009-s022"></a>

#### Conflict

Một nghệ nhân nổi tiếng chết ngay trước khi hoàn thành vật phẩm cho đại lễ.

<a id="m03-t009-s023"></a>

#### Secret

Ông cố tình không hoàn thành nó.

Ông phát hiện một hoa văn nghi lễ đã bị thay đổi.

<a id="m03-t009-s024"></a>

#### Character Pool

Nghệ nhân.

Học việc.

Người tìm nguyên liệu.

Collector.

Kẻ làm giả đồ cổ.

---

<a id="m03-t009-s025"></a>

## PC-04 — GÒ TRỐNG

**Sacred Hamlet — 55 hộ**

Không phải settlement bình thường.

Các gia đình sống ở đây nhiều đời để bảo quản trống, nhạc cụ và vật nghi lễ.

<a id="m03-t009-s026"></a>

#### Luật

Không ai được đánh đại trống nếu không có lễ.

Vi phạm bị coi là xúc phạm tổ tiên.

<a id="m03-t009-s027"></a>

#### Current Event

Một đêm giữa mùa khô:

**một chiếc trống tự phát ra âm thanh.**

Ít nhất bảy người tuyên bố đã nghe thấy.

Ba người khác nói họ không nghe gì.

<a id="m03-t009-s028"></a>

#### Gameplay

Investigation.

Religion.

Perception.

Social deduction.

Mạch Lạc.

---

<a id="m03-t009-s029"></a>

## PC-05 — CỬA RỪNG

**Frontier Settlement — 110 hộ**

Nằm giữa Phong Châu và đường lên Tản Viên.

<a id="m03-t009-s030"></a>

#### Vai trò

Điểm chuyển hàng.

Nơi thuê guide.

Nơi thợ săn bán da.

Nơi thương nhân vùng cao nghỉ.

<a id="m03-t009-s031"></a>

#### Conflict

Người miền núi cáo buộc thương nhân Phong Châu gian lận cân.

Người Phong Châu cáo buộc sơn dân cướp hàng.

Cả hai đều có phần đúng.

<a id="m03-t009-s032"></a>

#### Character Pool

Phiên dịch.

Guide.

Thương nhân lai văn hóa.

Thợ săn.

Người bỏ quê.

Đây là một trong những nơi tốt nhất để sinh character có **hai cultural identities**.

---

<a id="m03-t009-s033"></a>

## II. TAM GIANG

Tam Giang sinh character mạnh về **navigation, observation, survival, transportation, weather và water lore**.

---

<a id="m03-t009-s034"></a>

## TG-01 — BẾN VẠC

**Regional Hub — 420 hộ**

Một network nhà sàn, bến và đảo nhỏ.

Không có “trung tâm” cố định vì dòng nước thay đổi.

<a id="m03-t009-s035"></a>

#### Đặc điểm

Một số căn nhà phải di chuyển vài năm một lần.

Một số gia đình sở hữu thuyền nhưng gần như không sở hữu đất.

<a id="m03-t009-s036"></a>

#### Chính trị

Hội đồng gồm các trưởng bến.

Người nhiều đất chưa chắc nhiều quyền.

Người có nhiều thuyền mới thật sự có ảnh hưởng.

<a id="m03-t009-s037"></a>

#### Conflict

Dòng chính đang dịch chuyển.

Nếu tiếp tục, Bến Vạc mất vai trò trung tâm.

<a id="m03-t009-s038"></a>

#### Lựa chọn

Đào luồng mới.

Di chuyển settlement.

Xin người Phong Châu giúp.

Làm nghi lễ.

Hoặc để tự nhiên quyết định.

Không lựa chọn nào miễn phí.

---

<a id="m03-t009-s039"></a>

## TG-02 — CỒN MÂY

**Seasonal Settlement — 120–200 hộ**

Quy mô thay đổi theo mùa.

Mùa khô là một làng lớn.

Mùa nước cao hơn nửa dân cư rời đi.

<a id="m03-t009-s040"></a>

#### Economy

Cá.

Chim.

Sậy.

Rau vùng bãi.

<a id="m03-t009-s041"></a>

#### Secret

Dưới cồn là một nghĩa địa cổ.

Mỗi lần nước đổi dòng lại để lộ thêm mộ.

<a id="m03-t009-s042"></a>

#### Dungeon

**Mộ dưới Cồn Mây.**

Dungeon chỉ mở vào một khoảng thời gian nhất định trong năm.

---

<a id="m03-t009-s043"></a>

## TG-03 — VẠN NƯỚC

**Nomadic River Community — khoảng 90 thuyền/hộ**

Không có vị trí cố định.

<a id="m03-t009-s044"></a>

#### Culture

Người Vạn Nước không hỏi:

> “Nhà anh ở đâu?”

Họ hỏi:

> “Thuyền nhà anh đi dòng nào?”

<a id="m03-t009-s045"></a>

#### Reputation

Người sống trên đất thường coi họ là khó kiểm soát.

Họ lại coi dân làng là:

> “người bị đất giữ chân.”

<a id="m03-t009-s046"></a>

#### Passive Seed

Không Cần Bờ.

Ngủ Theo Sóng.

Mắt Tìm Bến.

Biết Mùi Nước.

---

<a id="m03-t009-s047"></a>

## TG-04 — BÃI SẬY

**Fishing Settlement — 160 hộ**

Nổi tiếng với bẫy cá và đánh bắt trong vùng nước nông.

<a id="m03-t009-s048"></a>

#### Conflict

Cá biến mất.

Người dân đổ lỗi cho thủy thần.

<a id="m03-t009-s049"></a>

#### Truth possibilities

Tùy World Seed, nguyên nhân có thể là:

dòng nước thay đổi;

một loài săn mồi tăng mạnh;

chất thải luyện kim;

nghi lễ thật sự bị phá;

hoặc nhiều nguyên nhân cùng lúc.

Đây là loại quest mà **sự thật không cố định giữa các playthrough**.

---

<a id="m03-t009-s050"></a>

## TG-05 — NGÃ BA LINH

**Sacred River Hamlet — 48 hộ**

Ba dòng nước gặp nhau.

<a id="m03-t009-s051"></a>

#### Belief

Linh hồn người chết trên sông được cho là thường đi ngang nơi này.

<a id="m03-t009-s052"></a>

#### Nghề độc đáo

**Người Dẫn Vong.**

Gia đình chuyên thực hiện nghi thức cho người chết mất xác.

<a id="m03-t009-s053"></a>

#### Character Seed

Một character sinh tại đây có thể hoàn toàn bình tĩnh trước corpse nhưng cực kỳ sợ người chết không có tên.

---

<a id="m03-t009-s054"></a>

## III. TẢN VIÊN

---

<a id="m03-t009-s055"></a>

## TV-01 — MƯỜNG THẠCH

**Regional Hub — 260 hộ**

Settlement lớn nhất vùng núi.

Nhà nằm thành từng cụm trên nhiều cao độ.

<a id="m03-t009-s056"></a>

#### Economy

Dược liệu.

Da.

Mật.

Gỗ.

Đá.

<a id="m03-t009-s057"></a>

#### Social Structure

Không có một chief tuyệt đối.

Các dòng họ có quyền theo lĩnh vực:

săn;

rừng;

nghi lễ;

đường núi.

<a id="m03-t009-s058"></a>

#### Conflict

Một phái muốn tăng thương mại với Phong Châu.

Một phái cho rằng người dưới đồng bằng đang mua rẻ tài nguyên.

---

<a id="m03-t009-s059"></a>

## TV-02 — KHE ĐÁ TRẮNG

**Hunter Settlement — 95 hộ**

Nơi sinh ra nhiều hunter giỏi.

<a id="m03-t009-s060"></a>

#### Rite of passage

Thanh niên không phải giết thú để trưởng thành.

Họ phải:

**theo dấu một con thú trong ba ngày mà không để nó phát hiện.**

Triết lý:

> Săn giỏi trước hết là hiểu con vật.

<a id="m03-t009-s061"></a>

#### Passive Pool

Bước Không Gãy Cành.

Đọc Phân Thú.

Ngửi Gió.

Không Đuổi Con Mồi Đang Sợ.

---

<a id="m03-t009-s062"></a>

## TV-03 — TRẠI MẬT

**Forest Hamlet — 42 hộ**

Chuyên tìm mật.

<a id="m03-t009-s063"></a>

#### Unique Knowledge

Người ở đây có hệ thống ký hiệu trên thân cây để đánh dấu:

tổ ong;

gấu;

nguồn nước;

cây thuốc;

đường nguy hiểm.

<a id="m03-t009-s064"></a>

#### Quest

Ai đó bắt đầu thay đổi ký hiệu.

Nhiều người đi rừng chết.

Không rõ đó là sabotage hay một người không biết luật.

---

<a id="m03-t009-s065"></a>

## TV-04 — BẢN SỪNG

**Craft/Hunter Settlement — 76 hộ**

Chuyên xử lý:

sừng;

xương;

da.

Tạo công cụ nhỏ, đồ nghi lễ, trang sức.

<a id="m03-t009-s066"></a>

#### Conflict

Một loài hươu linh thiêng xuất hiện.

Thợ săn muốn săn.

Vu Chúc cấm.

Người đói cần thịt.

---

<a id="m03-t009-s067"></a>

## TV-05 — ĐỈNH VÂN

**Sacred Outpost — 18 hộ**

Một trong những nơi cao nhất player có thể tới.

<a id="m03-t009-s068"></a>

#### Residents

Người giữ đường.

Vu Sơn.

Người quan sát thời tiết.

<a id="m03-t009-s069"></a>

#### Gameplay

Weather forecasting.

Vision quest.

Sơn Tinh lore.

<a id="m03-t009-s070"></a>

#### Secret

Một hang bị phong kín chứa bằng chứng cho thấy danh hiệu “Sơn Tinh” đã tồn tại qua nhiều thế hệ.

Nhưng bằng chứng không chứng minh đó là thần hay người.

---

<a id="m03-t009-s071"></a>

## IV. BÃI LẠC

Đây sẽ là vùng có **Social Graph dày nhất**.

---

<a id="m03-t009-s072"></a>

## BL-01 — LÀNG DÂU

**Regional Hub — 510 hộ**

Một trong những cộng đồng nông nghiệp lớn nhất.

<a id="m03-t009-s073"></a>

#### Economy

Lúa.

Gia súc.

Cá đồng.

Dệt.

Trao đổi lương thực.

<a id="m03-t009-s074"></a>

#### Power

Ba dòng họ kiểm soát phần lớn ruộng tốt.

<a id="m03-t009-s075"></a>

#### Conflict

Dòng họ thứ tư mới nổi muốn đào kênh.

Nếu đào thành công:

hàng trăm người hưởng lợi.

Nhưng đất của hai dòng họ lớn sẽ mất giá.

<a id="m03-t009-s076"></a>

#### Gameplay

Politics không cần vua chúa.

Chỉ cần **nước và ruộng**.

---

<a id="m03-t009-s077"></a>

## BL-02 — ĐỒNG MẠ

**Farming Settlement — 220 hộ**

Nổi tiếng chọn giống và gieo mạ.

<a id="m03-t009-s078"></a>

#### Cultural Trait

Họ tính thời gian không bằng tháng.

Mà bằng:

mùa cấy;

mùa cá;

mùa chim;

mùa nước.

<a id="m03-t009-s079"></a>

#### Character Pool

Người xem thời tiết.

Người giữ giống.

Nông dân.

Người quản nước.

Healer.

---

<a id="m03-t009-s080"></a>

## BL-03 — GÒ LÚA

**Storage Settlement — 135 hộ**

Nhiều kho lương.

<a id="m03-t009-s081"></a>

#### Strategic importance

Không giàu vì sản xuất.

Giàu vì **giữ và phân phối**.

<a id="m03-t009-s082"></a>

#### Conflict

Lương thực mất mỗi đêm.

Không dấu chân.

<a id="m03-t009-s083"></a>

#### Truth

Một mạng lưới người trong làng đang lén chuyển gạo cho cộng đồng bị nạn đói mà chính quyền địa phương từ chối cứu.

Kẻ trộm có thể lại là người có đạo đức nhất quest.

---

<a id="m03-t009-s084"></a>

## BL-04 — BẾN NẾP

**Market Settlement — 170 hộ**

Điểm gặp giữa nông dân Bãi Lạc và người Tam Giang.

<a id="m03-t009-s085"></a>

#### Identity

Food culture mạnh.

Nhiều lễ hội.

Nhiều loại đồ ăn nghi lễ.

<a id="m03-t009-s086"></a>

#### Character

Cook.

Fermenter.

Trader.

Brewer.

Herbalist.

<a id="m03-t009-s087"></a>

#### Passive độc đáo

**Nếm Ra Mùa** — từ thực phẩm có thể đoán chất lượng vụ mùa và vùng nguyên liệu.

---

<a id="m03-t009-s088"></a>

## BL-05 — CHẰM SEN

**Wetland Farming Hamlet — 66 hộ**

Người dân sống giữa vùng ruộng và đầm.

<a id="m03-t009-s089"></a>

#### Conflict

Một kế hoạch thoát nước sẽ tăng diện tích ruộng.

Nhưng phá hoàn toàn vùng đầm.

<a id="m03-t009-s090"></a>

#### Consequence

Nếu player ủng hộ:

lương thực tăng.

dược liệu giảm.

chim biến mất.

một spirit có thể biến mất theo.

World ecology thực sự thay đổi.

---

<a id="m03-t009-s091"></a>

## V. ĐỒNG ĐỎ

---

<a id="m03-t009-s092"></a>

## DD-01 — LÒ CỐC

**Regional Metallurgy Hub — 330 hộ**

Trung tâm luyện kim lớn.

<a id="m03-t009-s093"></a>

#### Districts

Lò luyện.

Bãi than.

Kho quặng.

Khu thợ đúc.

Chợ công cụ.

<a id="m03-t009-s094"></a>

#### Political reality

Ai điều khiển Lò Cốc có khả năng tác động đến cả:

nông nghiệp;

quân sự;

nghi lễ.

<a id="m03-t009-s095"></a>

#### Conflict

Sản lượng đồng giảm.

Không ai biết quặng ít đi hay quặng đang bị giấu.

---

<a id="m03-t009-s096"></a>

## DD-02 — MỎ SƯƠNG ĐỎ

**Mining Settlement — 88 hộ**

Ở sâu trong vùng đồi.

<a id="m03-t009-s097"></a>

#### Working condition

Công việc nguy hiểm nhất region.

Sập hầm.

Khí độc.

Nước ngầm.

<a id="m03-t009-s098"></a>

#### Belief

Thợ mỏ không gọi tên thật của núi khi ở dưới lòng đất.

<a id="m03-t009-s099"></a>

#### Secret

Một đường đào mới xuyên vào một hang cực cổ.

Trong đó không có quặng.

Có **dấu vết người**.

---

<a id="m03-t009-s100"></a>

## DD-03 — LÀNG KHUÔN

**Specialist Settlement — 105 hộ**

Chuyên tạo khuôn và chuẩn hóa một số sản phẩm đồng.

<a id="m03-t009-s101"></a>

#### Social prestige

Một người làm khuôn giỏi có địa vị gần như master craftsman.

<a id="m03-t009-s102"></a>

#### Conflict

Một bộ khuôn vũ khí mới bị đánh cắp.

Đây có thể trở thành precursor cho technology race về sau.

---

<a id="m03-t009-s103"></a>

## DD-04 — SUỐI THAN

**Fuel Settlement — 72 hộ**

Cung cấp than cho các lò.

<a id="m03-t009-s104"></a>

#### Ecological Conflict

Rừng quanh đây đang biến mất quá nhanh.

Nếu sản xuất dừng:

cả Đồng Đỏ bị ảnh hưởng.

Nếu tiếp tục:

region suy thoái.

Player không thể giải quyết bằng “giết boss”.

---

<a id="m03-t009-s105"></a>

## DD-05 — GÒ BÚA

**Craft Hamlet — 51 hộ**

Nổi tiếng sửa công cụ hơn là đúc mới.

<a id="m03-t009-s106"></a>

#### Philosophy

> “Vật còn dùng được thì chưa chết.”

<a id="m03-t009-s107"></a>

#### Gameplay

Repair.

Salvage.

Artifact identification.

<a id="m03-t009-s108"></a>

#### Character Seed

Một thợ già có thể nhìn vết sửa trên weapon và đoán được ít nhất ba chủ nhân trước.

---

<a id="m03-t009-s109"></a>

## VI. LÂM U

---

<a id="m03-t009-s110"></a>

## LU-01 — BẢN KHÓI XANH

**Regional Hub — 145 hộ**

Settlement lớn nhất Lâm U nhưng vẫn nhỏ hơn nhiều nơi khác.

<a id="m03-t009-s111"></a>

#### Specialty

Dược liệu.

Khói xua côn trùng.

Thuốc.

Poison knowledge.

<a id="m03-t009-s112"></a>

#### Belief

Mọi cây thuốc đều có “cây anh em” gây tác dụng ngược.

Một healer giỏi phải biết cả hai.

---

<a id="m03-t009-s113"></a>

## LU-02 — KHE MỘC

**Woodworker Settlement — 63 hộ**

Không chặt cây tùy tiện.

<a id="m03-t009-s114"></a>

#### Rule

Muốn lấy một cây lớn phải chỉ ra:

nó được dùng làm gì.

Người già trong làng có quyền từ chối.

<a id="m03-t009-s115"></a>

#### Conflict

Phong Châu đặt đơn hàng gỗ cực lớn.

Tiền đủ giúp settlement giàu.

Nhưng phải phá luật cổ.

---

<a id="m03-t009-s116"></a>

## LU-03 — TRẠI LÁ ĐEN

**Herbalist Camp — 29 hộ**

Nơi nghiên cứu các cây độc.

<a id="m03-t009-s117"></a>

#### Reputation

Người ngoài coi họ như witch.

Họ tự coi mình là healer.

<a id="m03-t009-s118"></a>

#### Philosophy

> Không biết độc thì không biết chữa.

<a id="m03-t009-s119"></a>

#### Character Pool

Poisoner.

Healer.

Botanist.

Outcast.

---

<a id="m03-t009-s120"></a>

## LU-04 — RỪNG CHÍN GỐC

**Sacred Settlement — 37 hộ**

Chín cây cổ lớn mọc thành vòng.

<a id="m03-t009-s121"></a>

#### Rule

Không được xây nhà bên trong vòng cây.

<a id="m03-t009-s122"></a>

#### Mystery

Một cây chết.

Không ai nhớ đã từng thấy cây chết trước đây.

Từ đó wildlife quanh vùng thay đổi.

---

<a id="m03-t009-s123"></a>

## LU-05 — MIẾU RỄ

**Semi-abandoned Hamlet — 14 hộ**

Từng có gần 100 hộ.

<a id="m03-t009-s124"></a>

#### History

Dân lần lượt bỏ đi.

Không có đại thảm họa.

Họ chỉ...

**bắt đầu mơ cùng một giấc mơ.**

<a id="m03-t009-s125"></a>

#### Dungeon

Underground root network / memory dungeon.

Đây là một trong những địa điểm supernatural mạnh nhất game.

---

<a id="m03-t009-s126"></a>

## VII. THƯỢNG ÂU

---

<a id="m03-t009-s127"></a>

## TA-01 — MƯỜNG GIÓ

**Regional Hub — 310 hộ**

Không phải “thủ đô của người Âu”.

Chỉ là settlement có ảnh hưởng lớn.

<a id="m03-t009-s128"></a>

#### Economy

Chăn nuôi.

Săn.

Trao đổi.

Đường núi.

<a id="m03-t009-s129"></a>

#### Politics

Nhiều dòng họ.

Alliance thay đổi thông qua hôn nhân.

<a id="m03-t009-s130"></a>

#### Campaign role

Một trong những nơi player lần đầu hiểu rằng khái niệm “Âu Việt” đơn giản hóa một mạng lưới cộng đồng phức tạp.

---

<a id="m03-t009-s131"></a>

## TA-02 — KHE CAO

**Mountain Settlement — 115 hộ**

Known for couriers.

<a id="m03-t009-s132"></a>

#### Culture

Một người trẻ được đánh giá cao nếu có thể đưa tin vượt hai dãy núi trong thời gian ngắn.

<a id="m03-t009-s133"></a>

#### Passive

**Chân Qua Đèo** — fatigue giảm nhanh khi di chuyển địa hình cao.

---

<a id="m03-t009-s134"></a>

## TA-03 — BẢN ĐÁ CHỒNG

**Fortified Settlement — 170 hộ**

Xây quanh các formation đá tự nhiên.

<a id="m03-t009-s135"></a>

#### Gameplay

Defensive warfare.

Clan politics.

<a id="m03-t009-s136"></a>

#### Conflict

Hai dòng họ đều tuyên bố quyền lãnh đạo.

Không bên nào đủ mạnh để thắng.

Một cái chết có thể phát động civil war.

---

<a id="m03-t009-s137"></a>

## TA-04 — ĐÈO ƯNG

**Military/Trade Outpost — 32 hộ**

Ai giữ nơi này kiểm soát một tuyến đường quan trọng.

<a id="m03-t009-s138"></a>

#### Story

Có thể đổi chủ nhiều lần trong campaign.

Phong Châu.

Local clans.

Thục Phán.

Bandits.

Player faction.

Settlement này là ví dụ điển hình của **dynamic ownership**.

---

<a id="m03-t009-s139"></a>

## TA-05 — HỒ XÁM

**Sacred Lake Settlement — 54 hộ**

Nước hồ có màu đặc biệt do khoáng chất.

<a id="m03-t009-s140"></a>

#### Belief

Không ai được dùng lưới ở giữa hồ.

<a id="m03-t009-s141"></a>

#### Conflict

Nạn đói khiến dân muốn phá taboo.

<a id="m03-t009-s142"></a>

#### Question

Taboo là mê tín?

Hay là quy tắc sinh thái được thần thoại hóa?

Game không trả lời ngay.

---

<a id="m03-t009-s143"></a>

## VIII. ĐẠI TRẠCH

---

<a id="m03-t009-s144"></a>

## DT-01 — BÃI LAU

**Regional Hub — 205 hộ**

Nhà trên cọc cao.

Có lối đi bằng thân cây và bè.

<a id="m03-t009-s145"></a>

#### Economy

Cá.

Lau.

Thuốc.

Rắn.

<a id="m03-t009-s146"></a>

#### Character

Tracker.

Hunter.

Healer.

Boat builder.

Swamp guide.

---

<a id="m03-t009-s147"></a>

## DT-02 — CỒN RẮN

**Hunter Settlement — 61 hộ**

Nổi tiếng bắt rắn.

<a id="m03-t009-s148"></a>

#### Rite

Trẻ em được dạy nhận dạng rắn trước khi được phép đi xa khỏi nhà.

<a id="m03-t009-s149"></a>

#### Passive

**Nhìn Vảy Biết Độc.**

Không nhất thiết miễn độc.

Chỉ biết nguy hiểm trước khi chạm vào.

---

<a id="m03-t009-s150"></a>

## DT-03 — TRẠI SƯƠNG

**Isolated Hamlet — 41 hộ**

Sương gần như quanh năm.

<a id="m03-t009-s151"></a>

#### Social consequence

Người dân nhận nhau bằng:

giọng;

bước chân;

tiếng ho.

Không chỉ bằng mặt.

<a id="m03-t009-s152"></a>

#### Passive Seed

**Nhớ Bước Chân.**

Có thể nhận ra một character quen thuộc trong darkness/fog.

---

<a id="m03-t009-s153"></a>

## DT-04 — BẾN ĐEN

**River Outpost — 70 hộ**

Nước rất tối do vật chất hữu cơ.

<a id="m03-t009-s154"></a>

#### Warning

Người địa phương:

> “Nước đen không đáng sợ. Nước đen mà trong mới đáng sợ.”

<a id="m03-t009-s155"></a>

#### Dungeon

Một vùng nước đột nhiên trở nên trong veo.

Toàn bộ cá biến mất.

---

<a id="m03-t009-s156"></a>

## DT-05 — GÒ CÒ

**Burial Settlement — 39 hộ**

Đất cao hiếm hoi giữa đầm.

Nhiều cộng đồng mang người chết tới đây.

<a id="m03-t009-s157"></a>

#### Society

Một phần dân sống bằng hoạt động liên quan tang lễ.

<a id="m03-t009-s158"></a>

#### Character Pool

Grave keeper.

Ritualist.

Boat coffin maker.

Bone reader.

Historian-by-oral-tradition.

---

<a id="m03-t009-s159"></a>

## IX. HẢI MÔN

---

<a id="m03-t009-s160"></a>

## HM-01 — CỬA SÒ

**Regional Hub — 390 hộ**

Một trong những trading centers lớn nhất.

<a id="m03-t009-s161"></a>

#### Goods

Muối.

Cá khô.

Sò.

Gỗ.

Đồ đồng.

Hàng ngoại lai.

<a id="m03-t009-s162"></a>

#### Culture

Open hơn Phong Châu với người lạ.

Người ta quan tâm:

> Anh mang gì tới?

hơn:

> Cha anh là ai?

---

<a id="m03-t009-s163"></a>

## HM-02 — VẠN MUỐI

**Salt Settlement — 180 hộ**

Muối là strategic resource.

<a id="m03-t009-s164"></a>

#### Conflict

Một faction muốn Phong Châu quản lý nguồn muối.

Người địa phương phản đối.

<a id="m03-t009-s165"></a>

#### Gameplay

Economics.

Smuggling.

Tax.

Political pressure.

---

<a id="m03-t009-s166"></a>

## HM-03 — BẾN DÀI

**Shipbuilding Settlement — 155 hộ**

Chuyên thuyền đi biển và thuyền tải lớn.

<a id="m03-t009-s167"></a>

#### Cultural prestige

Master shipwright cực kỳ được kính trọng.

<a id="m03-t009-s168"></a>

#### Secret

Một nhóm thợ đang thử nghiệm thân thuyền khác hoàn toàn truyền thống.

Một số tai nạn đã xảy ra.

---

<a id="m03-t009-s169"></a>

## HM-04 — ĐẢO RÙA

**Island Settlement — 73 hộ**

Chỉ đi được thuận lợi vào một số mùa.

<a id="m03-t009-s170"></a>

#### Belief

Rùa biển là messenger.

<a id="m03-t009-s171"></a>

#### Story potential

Đây có thể là một trong những nguồn hình thành nhiều phiên bản truyền thuyết về **Kim Quy**.

Không cần khẳng định Rùa Vàng lịch sử thực sự xuất thân từ đây.

---

<a id="m03-t009-s172"></a>

## HM-05 — MŨI GIÓ

**Navigation Outpost — 27 hộ**

Những người giỏi nhất về gió biển.

<a id="m03-t009-s173"></a>

#### Passive

**Đếm Gió Bằng Da.**

Nhân vật có thể dự báo thay đổi gió mà không cần công cụ.

<a id="m03-t009-s174"></a>

#### Quest

Một mùa, mọi dấu hiệu quen thuộc đều báo sai.

---

<a id="m03-t009-s175"></a>

## X. VIỆT THƯỜNG

Đây là frontier văn hóa game canon. Không sử dụng như một tuyên bố rằng settlement cụ thể này tồn tại trong lịch sử.

---

<a id="m03-t009-s176"></a>

## VT-01 — BẾN QUẾ

**Regional Hub — 240 hộ**

Trading settlement.

<a id="m03-t009-s177"></a>

#### Goods

Hương liệu.

Gỗ thơm.

Sản vật rừng.

Hàng đến từ xa hơn phía nam.

<a id="m03-t009-s178"></a>

#### Character strength

Multicultural NPC generation.

---

<a id="m03-t009-s179"></a>

## VT-02 — THUNG CHIM

**Hunter/Agricultural Settlement — 105 hộ**

Một thung lũng nằm trên tuyến chim di cư.

<a id="m03-t009-s180"></a>

#### Belief

Loài chim đầu tiên xuất hiện mỗi mùa quyết định điềm năm đó.

<a id="m03-t009-s181"></a>

#### Gameplay

Hunting.

Observation.

Seasonal events.

---

<a id="m03-t009-s182"></a>

## VT-03 — XÓM ĐÁ NÓNG

**Craft Settlement — 83 hộ**

Gần một nguồn địa nhiệt game canon.

<a id="m03-t009-s183"></a>

#### Culture

Bathing/healing rituals.

<a id="m03-t009-s184"></a>

#### Mystery

Nước bắt đầu nguội dần.

Một tín ngưỡng địa phương coi đây là dấu hiệu vùng đất đang chết.

---

<a id="m03-t009-s185"></a>

## VT-04 — CỬA LAM

**River Trade Settlement — 190 hộ**

Điểm giao lưu giữa nhiều cultural group.

<a id="m03-t009-s186"></a>

#### Linguistic gameplay

Có NPC không nói cùng ngôn ngữ/phương ngữ với player.

Interpreter trở thành class utility thật sự.

---

<a id="m03-t009-s187"></a>

## VT-05 — BÃI HƯƠU

**Pastoral/Hunting Settlement — 64 hộ**

Có quan hệ đặc biệt với đàn hươu địa phương.

<a id="m03-t009-s188"></a>

#### Conflict

Người ngoài đến săn để lấy sừng.

Dân địa phương coi việc đó là xúc phạm.

Một resource conflict nhanh chóng biến thành cultural conflict.

---

<a id="m03-t009-s189"></a>

## XI. BẮC LỘ

---

<a id="m03-t009-s190"></a>

## BN-01 — ĐỒN TRE

**Regional Military Hub — 270 hộ**

Không hoàn toàn là pháo đài.

Phần lớn cư dân vẫn:

trồng trọt;

sửa vũ khí;

nuôi gia súc;

vận chuyển.

<a id="m03-t009-s191"></a>

#### Character Pool

Soldier.

Scout.

Quartermaster.

Smith.

Refugee.

Interpreter.

Spy.

---

<a id="m03-t009-s192"></a>

## BN-02 — KHE CỜ

**Watch Settlement — 49 hộ**

Một tuyến tín hiệu bằng:

khói;

trống;

cờ;

lửa.

<a id="m03-t009-s193"></a>

#### Gameplay

Communication network.

Nếu một node bị phá:

information trên toàn region chậm lại.

World State thay đổi thực sự.

---

<a id="m03-t009-s194"></a>

## BN-03 — BẾN QUAN

**Border Trade Settlement — 145 hộ**

Buôn bán vẫn tiếp diễn ngay cả khi hai bên đang có xung đột.

<a id="m03-t009-s195"></a>

#### Core theme

> Người lính có thể đánh nhau ngày mai trong khi thương nhân hai bên giao dịch hôm nay.

<a id="m03-t009-s196"></a>

#### Quest density

Smuggling.

Prisoners.

Spies.

Forbidden relationships.

---

<a id="m03-t009-s197"></a>

## BN-04 — ĐỒNG SƯƠNG

**Refugee/Farming Settlement — 125 hộ**

Dân đến từ nhiều nơi.

Không có “dòng họ gốc” duy nhất.

<a id="m03-t009-s198"></a>

#### Character generator value

Extremely high.

Đây là nơi sinh:

orphans;

mixed families;

displaced nobles;

deserters;

widows;

adopted children.

---

<a id="m03-t009-s199"></a>

## BN-05 — CỬA ẢI

**Mountain Passage — 67 hộ**

Không có tường đá khổng lồ.

Thế phòng thủ dựa trên:

địa hình;

rào;

hào;

đường hẹp;

chướng ngại.

<a id="m03-t009-s200"></a>

#### Campaign role

Một trận đánh lớn có thể diễn ra ở đây.

Nhưng player có thể chuẩn bị vùng này từ hàng chục giờ trước.

---

<a id="m03-t009-s201"></a>

## XII. LOA VỰC

Đây là region duy nhất mà settlement map **biến đổi gần như hoàn toàn theo campaign**.

Khảo cổ Cổ Loa cho chúng ta nền đặc biệt tốt cho game: nông cụ đồng, mũi tên, khuôn đúc mũi tên và bằng chứng kỹ thuật luyện kim cho phép phát triển hợp lý một arc về công nghiệp quân sự và tổ chức sản xuất. 

---

<a id="m03-t009-s202"></a>

## LV-01 — XÓM CHẠ

**Pre-Cổ Loa Settlement — 205 hộ**

Khi game bắt đầu đây chỉ là một settlement nông nghiệp khá lớn.

<a id="m03-t009-s203"></a>

#### Act I

Ruộng.

Nhà.

Bến.

Làng.

<a id="m03-t009-s204"></a>

#### Act III

Đội khảo sát đến.

<a id="m03-t009-s205"></a>

#### Act IV

Một phần settlement bị di dời.

<a id="m03-t009-s206"></a>

#### Act V

Nó trở thành district của Cổ Loa.

<a id="m03-t009-s207"></a>

#### Emotional payoff

Player từng ngủ trong nhà của một NPC.

Mười năm sau:

căn nhà đó nằm dưới một đoạn thành.

---

<a id="m03-t009-s208"></a>

## LV-02 — GÒ CÁNH CUNG

**Craft Hamlet — 46 hộ**

Ban đầu chế tạo:

cung;

dây;

mũi tên săn.

<a id="m03-t009-s209"></a>

#### Evolution

Sau khi Âu Lạc hình thành:

→ workshop quân sự.

→ thử nghiệm nỏ.

→ production center.

<a id="m03-t009-s210"></a>

#### NPC arc

Một NPC player gặp lúc còn là thợ săn có thể trở thành master weapon maker.

---

<a id="m03-t009-s211"></a>

## LV-03 — ĐẦM VỌNG

**Wetland Settlement — 84 hộ**

Nằm tại vùng sẽ bị ảnh hưởng bởi công trình thủy lợi/thành.

<a id="m03-t009-s212"></a>

#### Conflict

Dân không muốn rời.

Nhà nước cần đất.

<a id="m03-t009-s213"></a>

#### Narrative

Cổ Loa là thành tựu lớn với lịch sử.

Nhưng player thấy trực tiếp:

> thành tựu lớn luôn có những người phải trả giá.

---

<a id="m03-t009-s214"></a>

## LV-04 — BÃI ĐẤT ĐỎ

**Labor Settlement — ban đầu 30 hộ**

Act IV tăng lên hơn 300 hộ tạm trú.

<a id="m03-t009-s215"></a>

#### Population

Laborers.

Engineers/craft masters.

Transporters.

Soldiers.

Merchants.

Sexual/family relationships hình thành giữa người từ nhiều vùng.

<a id="m03-t009-s216"></a>

#### Character generator

Một trong những nơi tốt nhất toàn game.

Vì chỉ sau vài năm xuất hiện:

trẻ em có cha mẹ đến từ hai region khác nhau.

---

<a id="m03-t009-s217"></a>

## LV-05 — KHE LÒ

**Metallurgy Hamlet → Military Industrial Quarter**

Ban đầu:

67 hộ.

Sau này:

hàng trăm workers.

<a id="m03-t009-s218"></a>

#### Story

Lò luyện được mở rộng.

Khuôn mới được phát triển.

Quy trình sản xuất thay đổi.

<a id="m03-t009-s219"></a>

#### Secret

Không tồn tại một thiên tài duy nhất chế tạo “nỏ thần”.

Thay vào đó có thể là:

thợ mỏ Đồng Đỏ;

người làm khuôn;

thợ nỏ Loa Vực;

thợ gỗ Tản Viên;

quân nhân thử nghiệm;

người tổ chức sản xuất.

**Một công nghệ huyền thoại được tạo bởi cả một hệ thống con người.**

Sau nhiều thế hệ, ký ức tập thể đơn giản hóa tất cả thành:

> “Nỏ thần.”

Đây chính xác là theme của game.

---

<a id="m03-t009-s220"></a>

## 3. Tổng cộng hiện tại

Chúng ta đã có:

**12 regions × 5 settlement = 60 settlement chủ chốt.**

Chưa tính:

trại săn;

mỏ nhỏ;

miếu;

trạm đường;

nhà đơn độc;

đảo;

lều mùa vụ;

camp quân sự;

settlement procedural.

60 settlement này sẽ đóng vai trò **Fixed World Nodes**.

Sau này chúng ta có thể sinh thêm khoảng **100–150 Minor Nodes** procedural mà vẫn hợp lore.

---

<a id="m03-t009-s221"></a>

## 4. Settlement không đứng yên

Đây là phần tôi muốn đưa vào core engine ngay từ đầu.

Mỗi settlement có trạng thái:

`Prosperity`

`Food`

`Population`

`Security`

`Health`

`Authority`

`Faith`

`Trade`

`Environment`

`Faction Control`

`Mạch Stability`

Ví dụ:

<a id="m03-t009-s222"></a>

#### Bến Vạc năm 1

Population: 420 hộ  
Food: 78  
Trade: 90  
Security: 70  
Mạch: 75

Sau một trận lũ:

Population: 392  
Food: 43  
Trade: 55  
Security: 52  
Mạch: 61

Sau đó player quyết định không giúp.

Hai năm sau:

Population: 305.

Một số gia đình chuyển tới Phong Châu.

Một số sang Cồn Mây.

Một nhóm gia nhập Vạn Nước.

---

<a id="m03-t009-s223"></a>

## 5. Và NPC thực sự di chuyển

Đây là chỗ hệ thống bắt đầu mạnh.

Ví dụ:

**Gia đình A**

sống tại Bến Vạc.

Có:

cha;

mẹ;

hai con;

ông ngoại.

Lũ xảy ra.

Cha chết.

Ruộng mất.

Mẹ đưa hai con tới Bến Hạc.

Ông ngoại từ chối rời quê.

10 năm sau:

Con lớn trở thành phu bến.

Con nhỏ học nghề thương nhân.

Ông ngoại trở thành một trong số rất ít người còn lại tại khu cũ.

Player có thể gặp **cả ba câu chuyện trong các thời điểm khác nhau**.

Không phải ba quest.

Mà là **một gia đình đang sống trong world simulation**.

---

<a id="m03-t009-s224"></a>

## 6. Điều này giải quyết bài toán 1.000 character như thế nào?

Giả sử chỉ 60 settlement có trung bình:

**150 hộ.**

Ta đã có:

~9.000 household.

Nếu trung bình 4 người/hộ:

~36.000 con người tiềm năng.

Chúng ta không cần instantiate toàn bộ.

Game chỉ cần chia thành:

**Tier S NPC**  
Major narrative character.

Khoảng 100–150.

**Tier A NPC**  
Có biography + relationship + quest potential.

Khoảng 800–1.000.

**Tier B NPC**  
Có family + occupation + personality + basic history.

Khoảng 3.000–5.000.

**Tier C Population**  
Chỉ tồn tại dưới dạng household/world simulation cho đến khi cần instantiate.

Như vậy mục tiêu **1.000 nhân vật cực kỳ chi tiết** hoàn toàn khả thi mà không biến game thành một database khổng lồ vô nghĩa.

---

<a id="m03-t009-s225"></a>

## 7. Quan trọng hơn: 1.000 người này không random độc lập

Một character sẽ đi qua pipeline:

**World Era**

↓

**Region**

↓

**Settlement**

↓

**Household**

↓

**Family**

↓

**Birth Order**

↓

**Childhood**

↓

**Local Historical Event**

↓

**Education / Mentor**

↓

**Occupation**

↓

**Relationships**

↓

**Beliefs**

↓

**Personality**

↓

**Desire**

↓

**Fear**

↓

**Contradiction**

↓

**Life Events**

↓

**Skill**

↓

**Signature Passive**

↓

**Current Goal**

↓

**Secret**

↓

**Potential Future Arc**

Ví dụ cuối cùng chúng ta có thể sinh được:

> **Mộc**, nam, 31 tuổi. Sinh tại Bến Vạc. Con thứ hai trong gia đình ba đời làm nghề đưa đò. Cha chết trong Đại Hồng Thủy năm Mộc 12 tuổi. Mộc cứu được em gái nhưng không cứu được cha. Sau sự kiện đó, Mộc ghét những người nói “mọi việc đều do thần quyết định”. Anh trở thành một trong những người đọc dòng nước giỏi nhất Bến Vạc nhưng tuyệt đối không thực hiện nghi lễ trước khi ra sông. Người ngoài nghĩ anh vô thần; thực tế Mộc vẫn bí mật đặt một bát cơm bên bến vào ngày giỗ cha. Mục tiêu của anh là tích đủ tài sản để đưa mẹ rời khỏi Bến Vạc. Nỗi sợ lớn nhất lại chính là phải sống xa sông.

**Signature Passive — Nước Không Nói Dối**

> Sau khi quan sát một dòng nước đủ lâu, Mộc có thể nhận ra một thay đổi bất thường do tác động vật lý. Anh không thể dùng khả năng này để phát hiện hiện tượng thuần linh giới.

Đây mới là mức character tôi muốn chúng ta hướng tới.

Không phải:

> Fisherman + Water Affinity +5%.

---

<a id="m03-t009-s226"></a>

## Bước tiếp theo: FAMILY & SOCIAL GRAPH BIBLE

Bây giờ chúng ta **không cần thêm địa danh nữa**.

Lớp tiếp theo mới thật sự quyết định 1.000 character có sống hay không:

**Household → Family → dòng họ → hôn nhân → anh chị em → mentor → tình yêu → thù hận → nợ → nghĩa → inheritance → social status.**

Ở bước đó tôi đề xuất xây một **Family Genome khoảng 25–30 thành phần**, kèm hệ thống **Life Event** và **Social Graph**, để từ 60 settlement này bắt đầu sinh thử **20–30 gia đình hoàn chỉnh** và kiểm tra xem character có thực sự khác nhau hay chưa.

Đặc biệt, tôi muốn thiết kế sao cho **mỗi quan hệ đều có hai chiều**: A có thể yêu B nhưng B chỉ coi A là bạn; A mắc ơn B nhưng B lại cảm thấy mình đã hại A. Chính sự bất đối xứng này sẽ là nguồn sinh story tự nhiên cực lớn cho game.

<!-- SOURCE T009 END -->

