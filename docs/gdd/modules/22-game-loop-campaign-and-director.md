# Module 22 — Game Loop, Campaign, World Director và chế độ chơi

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T053](../../source/conversation.md#t053)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

World Director exposure-only. Không đổi truth hoặc bắt buộc crisis cho pacing. Story/Standard/Tactical khác persistence Legacy/Ironman và assistance.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Valid threads/opportunities, current knowledge, recent experience và player intent. |
| Đầu ra / bên sở hữu | Ranked presentations; chỉ action/authorized workflow mới commit world changes. |
| Đọc cùng | 06, 17, 21, 23–27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Tắt Director không ngăn mandatory world consequences; đổi preset không sửa facts đã commit. |
| Việc chưa được chứng minh | Playtime/pacing targets, difficulty tuning và onboarding playtest. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T053 BEGIN -->

<a id="m22-t053-s001"></a>

## GDD MODULE 22
<a id="m22-t053-s002"></a>

## GAME LOOP, CAMPAIGN STRUCTURE, WORLD DIRECTOR, DIFFICULTY & GAME MODES

**Trạng thái:** Thiết kế hệ thống lõi — đề xuất khóa vào GDD  
**Thể loại:** Narrative RPG / Living World Simulation / Emergent Character Development  
**Phạm vi:** Core Gameplay Loop, Campaign Flow, World Director, Onboarding, Pacing, Challenge, Difficulty, Game Modes, Replayability và Production Validation.

---

<a id="m22-t053-s003"></a>

## PHẦN I — ĐỊNH VỊ TRẢI NGHIỆM GAME

<a id="m22-t053-s004"></a>

### 1. Game thực sự muốn người chơi trải nghiệm điều gì?

Đây là một RPG đặt con người vào một giai đoạn lịch sử đang chuyển mình, nơi nhân vật không phải người được chọn để cứu thế giới.

Người chơi nhận một cuộc đời do Character Generation tạo ra, với xuất thân, tuổi tác, gia đình, hoàn cảnh, năng lực và những hạn chế riêng.

Sau đó, người chơi tự quyết định mình dành cuộc đời ấy cho điều gì.

Một cuộc đời có thể xoay quanh chiến tranh. Cuộc đời khác có thể xoay quanh nghề đúc đồng, vận tải đường sông, chữa bệnh, khám phá, ngoại giao hoặc duy trì một gia đình.

Tất cả đều phải có khả năng đưa người chơi tiếp xúc với những thay đổi lịch sử lớn, nhưng từ những góc nhìn khác nhau.

Do đó, trải nghiệm cốt lõi không phải:

> Nhận nhiệm vụ → chiến đấu → lên cấp → nhận trang bị → chiến đấu khó hơn.

Mà là:

> **Sống trong một hoàn cảnh → nhận ra cơ hội hoặc vấn đề → lựa chọn cách can thiệp → sử dụng năng lực và quan hệ → chấp nhận kết quả → phát triển bản thân → chứng kiến thế giới thay đổi.**

Một vòng chơi thành công không nhất thiết có combat, cũng không nhất thiết có quest hoàn thành.

Nó cần tạo được ít nhất một sự thay đổi có ý nghĩa đối với nhân vật, hiểu biết hoặc thế giới.

<a id="m22-t053-s005"></a>

### 2. Năm trụ cột trải nghiệm

| Trụ cột | Cảm giác muốn tạo ra | Câu hỏi người chơi thường đặt ra |
|---|---|---|
| **Agency — Quyền tự quyết** | Lựa chọn của mình thật sự có giá trị | Tôi muốn trở thành người như thế nào? |
| **Discovery — Khám phá** | Thế giới luôn có điều đáng tìm hiểu | Tôi còn chưa biết gì về vùng đất, con người và lịch sử này? |
| **Belonging — Sự gắn bó** | Nhân vật thuộc về một mạng lưới người sống | Tôi có trách nhiệm với ai, và ai thật sự quan tâm tới tôi? |
| **Mastery — Sự trưởng thành** | Năng lực đến từ luyện tập và trải nghiệm | Tôi đã học được điều gì mà trước đây không thể làm? |
| **Consequence — Hậu quả** | Thế giới ghi nhận hành động theo thời gian | Quyết định hôm nay sẽ thay đổi điều gì sau nhiều năm? |

Cả năm trụ cột phải xuất hiện trong gameplay thường xuyên, nhưng không nhất thiết xuất hiện đồng thời trong mọi hoạt động.

Một cuộc săn có thể tập trung vào Discovery và Mastery.

Một cuộc thương lượng có thể tập trung vào Agency, Belonging và Consequence.

Một giai đoạn truyền nghề có thể tập trung vào Mastery và Legacy.

Đây là cơ sở để kiểm tra xem các hoạt động ngoài combat có thực sự hấp dẫn hay không.

<a id="m22-t053-s006"></a>

### 3. Những điều game tuyệt đối không nên trở thành

**Không phải MMORPG offline:** Không có vòng lặp farm vô hạn chỉ để tăng sức mạnh nhân vật.

**Không phải survival sandbox thuần túy:** Đói khát, dụng cụ và thời tiết tạo quyết định, nhưng không buộc player chăm sóc hàng chục thanh chỉ số liên tục.

**Không phải visual novel tuyến tính:** Player phải có khả năng tác động lên World State, không chỉ lựa chọn đoạn hội thoại để đọc những kết thúc khác nhau.

**Không phải grand strategy:** Người chơi có thể tác động chính trị và kinh tế, nhưng không mặc định sở hữu quyền điều khiển toàn bộ faction, quân đội hay settlement.

**Không phải CRPG combat-centric:** Chiến đấu quan trọng, nhưng nhân vật không chiến đấu vẫn có thể có một campaign phong phú.

**Không phải mô phỏng đời sống hoàn toàn tự động:** Thế giới có thể tự vận hành, nhưng player luôn cần những cơ hội lựa chọn, học hỏi và hành động có ý nghĩa.

---

<a id="m22-t053-s007"></a>

## PHẦN II — KIẾN TRÚC GAME LOOP

<a id="m22-t053-s008"></a>

### 4. Bốn cấp độ vòng lặp gameplay

Game cần có bốn vòng lặp lồng nhau.

| Cấp | Phạm vi | Hoạt động chính | Kết quả |
|---|---|---|---|
| Micro Loop | Vài phút đến một tình huống | Quan sát, quyết định, hành động, phản hồi | Thay đổi tình huống |
| Activity Loop | Một chuyến đi, công việc hoặc nhiệm vụ | Chuẩn bị, thực hiện, xử lý trở ngại, kết thúc | Kỹ năng, thông tin, tài sản, quan hệ |
| Life Loop | Nhiều tháng đến nhiều năm | Nghề nghiệp, gia đình, phe phái, phát triển bản thân | Thay đổi vai trò và cuộc đời |
| Historical Loop | Nhiều năm đến toàn chiến dịch | Biến động Văn Lang–Âu Lạc, Cổ Loa, khủng hoảng | Thay đổi thời đại và di sản |

Điều quan trọng là các vòng lặp này phải **nối tiếp về nhân quả**, không phải bốn hệ thống riêng biệt.

Ví dụ:

Player quan sát một con thuyền hỏng.

Ở Micro Loop, họ phát hiện nguyên nhân.

Ở Activity Loop, họ sửa thuyền và đưa người về.

Ở Life Loop, họ dần có uy tín của một người thợ giỏi.

Ở Historical Loop, kinh nghiệm đó có thể đưa họ vào mạng sản xuất và vận tải phục vụ Cổ Loa.

Một hoạt động nhỏ có thể trở thành nền của một cuộc đời.

<a id="m22-t053-s009"></a>

### 5. Micro Gameplay Loop

Đây là vòng lặp xuất hiện trong Dialogue, Combat, Exploration, Crafting, Dungeon và nhiều hoạt động khác.

**Bước 1 — Tiếp nhận tình huống.** Người chơi quan sát những gì character có thể nhận biết: địa hình, biểu hiện NPC, dấu vết, trang bị hoặc một sự kiện đang diễn ra.

**Bước 2 — Diễn giải.** Attribute, Skill, Knowledge, Profession và companion cung cấp những cách hiểu khác nhau. Hệ thống không nhất thiết kết luận thay người chơi.

**Bước 3 — Lựa chọn mục tiêu.** Player quyết định muốn đạt điều gì, không chỉ lựa chọn kỹ năng có xác suất thành công cao nhất.

**Bước 4 — Thực hiện hành động.** Rule Engine kiểm tra điều kiện, nguồn lực, kỹ năng và các yếu tố bất định.

**Bước 5 — Nhận kết quả.** Hành động có thể thành công, thành công với cái giá, thất bại hoặc làm phát sinh một tình huống mới.

**Bước 6 — Cập nhật trạng thái.** World State, Character State, Knowledge, Relationship và Chronicle được cập nhật tương ứng.

Ví dụ một người thợ phát hiện khuôn đúc có vết nứt.

Họ có thể sửa, thay khuôn, hạ yêu cầu chất lượng hoặc chấp nhận rủi ro.

Hệ thống xử lý kết quả vật lý thực sự.

Nếu họ giấu lỗi trước người thầy, đây còn là Social Event.

Không cần combat hoặc quest để vòng lặp đó có ý nghĩa.

<a id="m22-t053-s010"></a>

### 6. Activity Loop

Một Activity là **một chuỗi hành động có mục tiêu và thời lượng tương đối rõ**, nhưng không nhất thiết là quest.

Đề xuất cấu trúc chung:

**Xác định mục tiêu → Chuẩn bị → Thực hiện → Đối mặt tình huống phát sinh → Điều chỉnh → Kết thúc hoặc rút lui → Xử lý hậu quả.**

Ví dụ với nghề vận tải:

- Nhận vận chuyển một lô hàng.
- Kiểm tra thuyền, thời tiết và tuyến.
- Thương lượng với chủ hàng.
- Chọn companion hoặc người hỗ trợ.
- Di chuyển.
- Phát hiện bến trung chuyển bị đóng.
- Quyết định đi vòng, chờ hoặc tìm người có quyền cho đi.
- Giao hàng.
- Giải quyết tiền công, thiệt hại, lời hứa và quan hệ.

Trong ví dụ này, gameplay được tạo từ chính logic của hoạt động.

Không cần hệ thống ép phát sinh cướp đường để chuyến đi hấp dẫn.

<a id="m22-t053-s011"></a>

### 7. Life Loop

Đây là vòng lặp đặc trưng khiến game khác RPG thông thường.

Life Loop gồm:

**Hoàn cảnh hiện tại → Mục tiêu cuộc đời → Hoạt động lặp lại và biến cố → Phát triển kỹ năng/quan hệ → Thay đổi vai trò → Trách nhiệm mới → Đánh giá lại mục tiêu.**

Ví dụ một người Dược Nhân:

Ban đầu chỉ giúp chữa thương trong làng.

Sau vài năm, họ trở nên có uy tín.

Người ở các vùng khác bắt đầu tìm đến.

Họ có thể mở nơi chữa bệnh, nhận học trò, tham gia xử lý dịch bệnh hoặc trở thành người có tiếng nói trong hội đồng.

Đến một thời điểm, việc tiếp tục nhận nhiều bệnh nhân có thể xung đột với trách nhiệm gia đình.

Progression của họ lúc này không phải tăng Healing Skill vô hạn.

Nó là **sự thay đổi về cơ hội, trách nhiệm, uy tín và lựa chọn cuộc đời**.

<a id="m22-t053-s012"></a>

### 8. Historical Loop

Historical Loop không yêu cầu player trực tiếp tham dự mọi sự kiện lịch sử.

Nó vận hành qua:

**Historical Conditions → Faction Pressure → Economic/Social Change → Historical Event → Local Aftermath → New Political Order.**

Player có thể tương tác với một hoặc nhiều mắt xích.

Ví dụ Cổ Loa:

Một người làm vận tải có thể tham gia đưa vật liệu.

Một người thợ đúc tham gia chuẩn hóa công cụ.

Một người làm nông bị ảnh hưởng bởi nhu cầu lương thực.

Một Sứ Giả tham gia giải quyết tranh chấp đất đai.

Một Lạc Binh tham gia bảo vệ công trình.

Sự kiện lớn vẫn có cùng vị trí trong lịch sử chiến dịch, nhưng trải nghiệm cá nhân hoàn toàn khác.

---

<a id="m22-t053-s013"></a>

## PHẦN III — HOẠT ĐỘNG HẰNG NGÀY VÀ LỰA CHỌN CỦA PLAYER

<a id="m22-t053-s014"></a>

### 9. Player làm gì khi không có quest?

Đây là câu hỏi thiết kế quan trọng.

Nếu game chỉ hấp dẫn khi có Story Thread mới, World Director sẽ bị buộc liên tục tạo vấn đề để giữ người chơi bận rộn.

Thay vào đó, game phải có **Ordinary Life Gameplay**.

Các hoạt động có thể xuất phát từ:

- nghề nghiệp;
- nhu cầu của household;
- việc duy trì tài sản;
- học tập và huấn luyện;
- quan hệ;
- khám phá;
- giao thương;
- nghiên cứu;
- dự án cộng đồng;
- nghỉ ngơi và hồi phục.

Mỗi hoạt động cần có ít nhất một giá trị: phát triển năng lực, duy trì sinh kế, xây dựng quan hệ, thu nhận thông tin hoặc chuẩn bị cho tương lai.

Hoạt động thường lệ không cần tạo tình huống tương tác mỗi lần.

Một người thợ rèn giỏi làm cùng một sản phẩm quen thuộc có thể hoàn thành qua Downtime.

Nhưng khi họ nhận đơn hàng khó, thiếu vật liệu hoặc thử một thiết kế mới, Activity Loop trở nên tương tác.

<a id="m22-t053-s015"></a>

### 10. Personal Intent — Mục tiêu do player tự theo đuổi

Bên cạnh Quest System, player nên có khả năng đặt **Personal Intent**.

Đây không phải nhiệm vụ mà World Director phát.

Nó là mục tiêu tự xác định của nhân vật.

Ví dụ:

- Học thành thạo một nghề.
- Mở một xưởng riêng.
- Đi tới một vùng chưa biết.
- Tìm người có thể dạy một Technique.
- Khôi phục một món đồ gia truyền.
- Hàn gắn quan hệ với người thân.
- Xây dựng một tuyến thuyền.
- Trở thành người có uy tín trong cộng đồng.

Hệ thống có thể hỗ trợ theo dõi tiến độ và những điều kiện đã biết.

Tuy nhiên, nó không được đảm bảo kết quả.

Player đặt mục tiêu mở xưởng không có nghĩa engine sẽ tự sinh vật liệu, khách hàng và thợ phụ để đáp ứng.

<a id="m22-t053-s016"></a>

### 11. Ba loại tiến triển

Mỗi phiên chơi nên có cơ hội tạo ít nhất một loại tiến triển, nhưng không phải bằng cách phát thưởng giả tạo.

<a id="m22-t053-s017"></a>

#### Capability Progress

Nhân vật giỏi hơn hoặc hiểu sâu hơn.

Ví dụ: học được cách đánh giá chất lượng khuôn.

<a id="m22-t053-s018"></a>

#### Situation Progress

Một vấn đề cụ thể thay đổi.

Ví dụ: tìm ra ai đang giữ nguồn than.

<a id="m22-t053-s019"></a>

#### Life/World Progress

Một quan hệ, nghề nghiệp, cơ sở vật chất hoặc cộng đồng thay đổi.

Ví dụ: apprentice bắt đầu được giao các công việc quan trọng.

Các loại tiến triển này cần được hệ thống nhận biết để hỗ trợ phản hồi tốt hơn, không phải quy đổi thành một thanh XP chung.

<a id="m22-t053-s020"></a>

### 12. Một ngày bình thường vẫn có thể là gameplay tốt

Ví dụ player là một thợ gốm ở Bãi Lạc.

Buổi sáng họ kiểm tra một đơn hàng.

Phát hiện nguyên liệu bị ẩm nên phải điều chỉnh kế hoạch.

Buổi trưa họ gặp người cung cấp đất và biết tuyến vận chuyển đang bị ảnh hưởng bởi mưa lớn.

Buổi chiều người học việc đề nghị sử dụng một phương pháp mới.

Player chọn thử nghiệm trên một phần nhỏ nguyên liệu.

Cuối ngày, kết quả chưa tạo ra biến cố lịch sử nào.

Nhưng:

- player học thêm về vật liệu;
- quan hệ với người học việc thay đổi;
- tình trạng đơn hàng được cập nhật;
- họ biết một vấn đề mới trên tuyến vận chuyển.

Nếu phần lớn các ngày thường lệ tạo được giá trị như vậy mà không cần bị kịch tính hóa, game sẽ có nền gameplay bền vững.

---

<a id="m22-t053-s021"></a>

## PHẦN IV — CAMPAIGN STRUCTURE

<a id="m22-t053-s022"></a>

### 13. Campaign có hai trục độc lập nhưng liên kết

Game sử dụng hai trục tiến trình.

**Historical Campaign Track:** Thời gian và những Historical Anchors của thời đại.

**Personal Life Track:** Cuộc đời, nghề nghiệp, quan hệ, mục tiêu và các Story Thread của player.

Hai trục này không khóa cứng với nhau.

Player không cần hoàn thành 15 nhiệm vụ trước khi Âu Lạc hình thành.

Ngược lại, họ cũng không bắt buộc phải bỏ nghề ngay khi một Historical Anchor bắt đầu.

World Simulation vẫn tiến.

<a id="m22-t053-s023"></a>

### 14. Cấu trúc sáu hồi

Chiến dịch sử dụng sáu hồi đã thống nhất, với tổng thời lượng khoảng 20–25 năm trong game.

Các khoảng năm là mốc thiết kế tương đối và phải được hiệu chỉnh khi xây dựng chiến dịch, không phải niên đại lịch sử được khẳng định.

| Hồi | Chủ đề | Áp lực lịch sử | Vai trò của player |
|---|---|---|---|
| I — Trống không vang | Vết rạn đầu tiên | Nghi lễ, ký ức và quyền lực địa phương xuất hiện bất ổn | Tiếp xúc với những dấu hiệu đầu tiên |
| II — Những lời thề đã quên | Những quan hệ cũ | Lời thề, quyền sử dụng đất/nước, quan hệ cộng đồng bị thử thách | Giải quyết hoặc làm sâu thêm các xung đột |
| III — Hai con đường | Chuyển tiếp chính trị | Các lực lượng quanh Văn Lang, Âu Việt và Thục Phán thay đổi | Lựa chọn quan hệ, nghĩa vụ và vị trí |
| IV — Thành xoáy ốc | Kiến tạo trật tự mới | Cổ Loa hình thành qua lao động, tài nguyên, kỹ thuật và chính trị | Tham gia hoặc chịu tác động từ quá trình xây dựng |
| V — Nỏ | Công nghệ và quyền lực | Tri thức kỹ thuật, tổ chức sản xuất và sức mạnh quân sự | Bảo vệ, truyền, phát triển hoặc tranh chấp tri thức |
| VI — Lông ngỗng | Sự đổ vỡ và ký ức | Khủng hoảng cuối thời Âu Lạc và các lớp truyền thuyết | Cố bảo vệ những điều quan trọng và để lại di sản |

<a id="m22-t053-s024"></a>

### 15. Hồi I — Trống không vang

Mục tiêu gameplay không phải đưa player ngay vào cuộc điều tra trung tâm ở Phong Châu.

Vì nhân vật khởi đầu được tạo ngẫu nhiên trong những điều kiện khác nhau, một người ở Tam Giang hoặc Lâm U có thể chưa biết chuyện chiếc trống bị gián đoạn và người giữ trống bị sát hại.

Thay vào đó, hồi đầu vận hành ở ba tầng.

**Tầng cá nhân:** Giới thiệu hoàn cảnh sinh sống, nghề nghiệp, gia đình, năng lực và vấn đề gần gũi.

**Tầng địa phương:** Một biến cố nhỏ nhưng có hậu quả thật, giúp player hiểu cách Social Graph, Exploration hoặc Profession vận hành.

**Tầng lịch sử:** Tin tức hoặc dấu hiệu của khủng hoảng lớn bắt đầu lan truyền theo các mạng thông tin hợp lệ.

Một người ở Phong Châu có thể chứng kiến trực tiếp sự kiện trống.

Một người ở Tam Giang chỉ nghe tin sau nhiều ngày.

Một người sống xa mạng giao thương có thể biết rất muộn, khi các hậu quả chính trị đã bắt đầu xuất hiện.

Cả ba đều là khởi đầu hợp lệ.

<a id="m22-t053-s025"></a>

### 16. Hồi II — Những lời thề đã quên

Trọng tâm là làm rõ thế giới được duy trì bằng các mạng lưới nghĩa vụ, tập tục, tín ngưỡng và quyền lực như thế nào.

Gameplay nên thúc đẩy:

- quan hệ giữa settlement;
- các tuyến đường có ý nghĩa kinh tế và xã hội;
- lời hứa và nợ;
- xung đột tài nguyên;
- niềm tin và nghi lễ;
- các phương án giải quyết không dùng bạo lực.

Một player có thể dành phần lớn hồi này cho nghề nghiệp nhưng vẫn chứng kiến sự thay đổi của mạng lưới thương mại hoặc quyền sử dụng đất.

Một người khác có thể trở thành Sứ Giả hoặc Người Giữ Ký Ức.

Hồi này cần tạo cảm giác rằng **trật tự cũ từng có lý do để tồn tại, nhưng không còn giải quyết được mọi vấn đề mới**.

<a id="m22-t053-s026"></a>

### 17. Hồi III — Hai con đường

Đây là giai đoạn các lựa chọn chính trị trở nên có sức nặng lớn hơn.

Nhưng không nên chia thế giới thành hai đội:

> Phe Hùng Vương tốt — phe Thục Phán xấu.

Hoặc ngược lại.

Các faction có thể khác nhau về:

- lợi ích;
- cách tổ chức;
- quyền lãnh đạo;
- quan hệ miền núi–đồng bằng;
- khả năng bảo vệ;
- thương mại;
- các nghĩa vụ cũ.

Player có thể tham gia thông qua nghề nghiệp, quan hệ gia đình, quân sự hoặc ngoại giao.

Một số nhân vật sẽ thay đổi allegiance.

Một số giữ lập trường cũ.

Một số tìm cách thích nghi mà không gia nhập phe lớn nào.

Hồi này cũng là thời điểm thích hợp để những mâu thuẫn trong party bắt đầu trở nên nghiêm trọng nếu các companion có giá trị và nghĩa vụ đối lập.

<a id="m22-t053-s027"></a>

### 18. Hồi IV — Thành xoáy ốc

Đây nên là hồi có sự thay đổi **vật chất và địa lý** rõ nhất.

Loa Vực phải thay đổi qua thời gian:

- ruộng, vùng nước và xóm cũ;
- nơi khảo sát;
- công trường;
- bến vận tải;
- khu tập kết vật liệu;
- xưởng sản xuất;
- tường và hệ thống phòng thủ;
- cộng đồng mới.

Người chơi không cần tham gia tất cả công đoạn.

Nhưng nếu đã đi qua một vùng vài năm trước, khi trở lại họ phải nhận thấy sự khác biệt có nhân quả.

Một player Xảo Nhân có thể ảnh hưởng công nghệ và phương pháp thi công.

Một Dược Nhân xử lý bệnh tật hoặc thương tích của lao động.

Một Sứ Giả giải quyết nghĩa vụ giữa các cộng đồng.

Một Thủ Lĩnh tổ chức lao động và nguồn lực.

Một character bình thường có thể chỉ cố bảo vệ đất đai và gia đình.

Không nên có một lối chơi được xem là “thực sự tham gia hồi IV” còn các lối chơi khác bị coi là phụ.

<a id="m22-t053-s028"></a>

### 19. Hồi V — Nỏ

Trọng tâm là mối quan hệ giữa:

**Knowledge → Craft → Standardization → Logistics → Organization → Political Power → Myth.**

Các Story Thread phải phản ánh rằng công nghệ không chỉ thuộc về một thiên tài.

Nó phụ thuộc vào:

- người tìm vật liệu;
- thợ làm khuôn;
- thợ đúc;
- người thử nghiệm;
- người huấn luyện;
- người vận tải;
- người quản lý;
- người bảo vệ bí mật.

Đây là cơ hội để mọi tuyến nghề nghiệp đã phát triển từ đầu game có thể giao với Historical Campaign.

Những người liên quan trực tiếp đến bí mật kỹ thuật có thể có nguy cơ chính trị lớn.

Những người ở xa có thể cảm nhận giai đoạn này qua sự thay đổi của nhu cầu quân sự, giá kim loại và quyền lực địa phương.

<a id="m22-t053-s029"></a>

### 20. Hồi VI — Lông ngỗng

Không nên biến hồi kết thành một cuộc đánh boss bắt buộc cho mọi playthrough.

Các biến cố lớn gắn với Mỵ Châu–Trọng Thủy và sự suy tàn của Âu Lạc tạo áp lực lên:

- lòng tin;
- thông tin;
- hôn phối và nghĩa vụ;
- bí mật;
- quyền lực;
- sự sống còn;
- ký ức lịch sử.

Một character có thể tham gia trận chiến.

Một người khác có thể tìm cách đưa người thân thoát khỏi nguy hiểm.

Một thợ thủ công cố giữ lại kỹ thuật trước khi workshop bị phá.

Một Sứ Giả cố tìm cơ hội thương lượng.

Một người cao tuổi có thể dành hồi cuối để lựa chọn người kế tục.

**Historical Outcome** và **Personal Outcome** phải được đánh giá riêng.

Player không nhất thiết thay đổi được kết cục lớn của thời đại, nhưng phải có thể tác động sâu sắc tới những cuộc đời cụ thể.

---

<a id="m22-t053-s030"></a>

## PHẦN V — LÀM THẾ NÀO ĐỂ KHÔNG RAILROAD CAMPAIGN?

<a id="m22-t053-s031"></a>

### 21. Historical Anchors phải vận hành bằng điều kiện, không phải nhiệm vụ chính tuyến tính

Một Anchor cần định nghĩa:

**Kết quả lịch sử cần giữ:** Ví dụ Cổ Loa được hình thành.

**Nguồn lực và lực lượng thực hiện:** Những faction, cộng đồng, nghề và tài nguyên liên quan.

**Các kịch bản hợp lệ:** Nhiều con đường khác nhau dẫn tới kết quả lớn.

**Biên độ can thiệp:** Player có thể thay đổi những kết quả nào.

**Điều kiện chuyển hồi:** Thời gian và trạng thái lịch sử, không phải chỉ số nhiệm vụ đã hoàn thành.

Nếu player giết một nhân vật quan trọng nằm trong kịch bản, game phải xử lý theo những quy tắc đã thiết kế.

Không được vừa cho phép sự kiện xảy ra vừa tự ý hồi sinh nhân vật trong cutscene.

Đối với những nhân vật hoặc sự kiện đã được khóa bởi Historical Anchor, game phải thiết kế rõ phạm vi tương tác trước khi đưa vào gameplay.

Không dùng các giới hạn không được giải thích như:

> “NPC này không nhận damage vì là nhân vật lịch sử.”

<a id="m22-t053-s032"></a>

### 22. Historical Anchor Controller

Đề xuất một bộ điều phối lịch sử riêng, không gộp vào World Director.

Nó có trách nhiệm:

- theo dõi tiến trình các hồi;
- xác nhận điều kiện lịch sử;
- điều phối những sự kiện lớn đã authored;
- kiểm tra các giới hạn bất biến;
- cập nhật những kịch bản kết quả có thể xảy ra;
- không cho một biến cố background làm hỏng toàn bộ cấu trúc lịch sử.

Ví dụ khi một faction cung ứng sắt/đồng gặp khủng hoảng, quá trình phát triển công nghệ ở Cổ Loa có thể chậm lại hoặc chuyển sang mạng nguồn lực khác đã tồn tại.

Nhưng controller không được tự sinh vô hạn vật liệu và thợ giỏi chỉ để bảo đảm một cutscene xảy ra đúng ngày.

Các phương án lịch sử thay thế phải có cơ sở và được kiểm định từ thiết kế campaign.

---

<a id="m22-t053-s033"></a>

## PHẦN VI — ONBOARDING: 15 PHÚT ĐẦU

<a id="m22-t053-s034"></a>

### 23. Vấn đề đặc biệt của game

Character Generation đã được khóa:

- người chơi không chọn quê;
- không chọn tuổi khởi đầu;
- không chọn nghề;
- không chọn gia đình;
- không chọn Attribute;
- không chọn Skill;
- không reroll tự do để tối ưu build.

Điều này tạo replayability mạnh nhưng gây thách thức onboarding.

Một character sinh ra là người học việc 16 tuổi ở Tam Giang rất khác một thợ đúc 38 tuổi ở Đồng Đỏ hay một người giữ nghi lễ 55 tuổi ở Tản Viên.

Nếu xây một tutorial cố định cho tất cả, game sẽ phá vỡ tính chân thực của xuất thân.

Giải pháp là:

<a id="m22-t053-s035"></a>

## ORIGIN-ADAPTIVE ONBOARDING

Tutorial được lựa chọn dựa trên điều kiện của character, không thay đổi character để phù hợp tutorial.

<a id="m22-t053-s036"></a>

### 24. Character Viability Gate

Trước khi bắt đầu, hệ thống phải xác nhận nhân vật được tạo có khả năng tham gia gameplay hợp lý.

Các điều kiện cần kiểm tra:

**Survival Viability:** Nhân vật có hoàn cảnh sinh sống và các cơ hội thích nghi cơ bản.

**Action Viability:** Có những hoạt động phù hợp với thể trạng và năng lực.

**Social Viability:** Có ít nhất một số quan hệ hoặc mạng xã hội có khả năng tạo tương tác.

**Learning Viability:** Có cơ hội phát triển, học nghề hoặc thay đổi cuộc sống.

**Campaign Reachability:** Có thể tiếp xúc với Historical Campaign qua ít nhất một mạng thông tin, đường đi hoặc quan hệ có căn cứ.

**Narrative Viability:** Có mâu thuẫn hoặc mục tiêu đời sống đủ để tạo khởi đầu hấp dẫn.

Viability không có nghĩa nhân vật khởi đầu mạnh hoặc giàu.

Một nhân vật nghèo, yếu hoặc bị hạn chế vẫn có thể hợp lệ nếu thế giới cung cấp những con đường sống và phát triển thực tế.

<a id="m22-t053-s037"></a>

### 25. Kịch bản 15 phút đầu

Đề xuất mục tiêu trải nghiệm như sau.

<a id="m22-t053-s038"></a>

#### Phút 0–3: Bước vào một cuộc đời

Game giới thiệu nhân vật thông qua một hoàn cảnh đang diễn ra.

Không nên mở bằng màn hình dài giải thích sáu Attribute, mười một Class, mười hai vùng và hàng chục hệ thống.

Ví dụ một người học nghề thuyền đang được yêu cầu chuẩn bị chuyến đi.

Qua môi trường và hội thoại, player hiểu:

- mình là ai;
- đang ở đâu;
- những người gần gũi;
- đang làm công việc gì.

<a id="m22-t053-s039"></a>

#### Phút 3–8: Quyết định đầu tiên

Nhân vật gặp một tình huống nhỏ có nhiều cách xử lý.

Đó có thể là:

- khách muốn chở hàng quá tải;
- một công cụ bị hỏng;
- người thân đề nghị giúp đỡ;
- dấu hiệu thời tiết xấu;
- bất đồng về một quy tắc cộng đồng.

Mục đích là giúp player hiểu:

**Observe → Interpret → Choose → Consequence.**

Không bắt buộc phải dùng combat.

<a id="m22-t053-s040"></a>

#### Phút 8–15: Hậu quả đầu tiên

Player thấy quyết định vừa rồi làm thay đổi điều gì.

Ví dụ:

Nếu từ chối chở quá tải, chủ hàng tức giận nhưng chuyến đi an toàn.

Nếu nhận chở, player có thể kiếm thêm tiền nhưng chịu rủi ro thực.

Một companion hoặc người thân có thể phản ứng theo tính cách của họ.

Game ghi sự kiện vào Character Chronicle.

Đồng thời, một dấu hiệu rộng hơn của thế giới bắt đầu xuất hiện: tin tức, người lạ, thay đổi giá hàng, căng thẳng nghi lễ hoặc một biến cố liên quan địa phương.

Người chơi cần kết thúc 15 phút đầu với cảm giác:

> “Tôi đã bắt đầu sống trong thế giới này và lựa chọn đầu tiên của tôi có ý nghĩa.”

Không phải:

> “Tôi đã học xong 12 menu.”

---

<a id="m22-t053-s041"></a>

## PHẦN VII — MỘT GIỜ ĐẦU

<a id="m22-t053-s042"></a>

### 26. Mục tiêu của giờ đầu tiên

Trong khoảng một giờ đầu, player cần hiểu được bốn điều:

1. Character có cuộc đời và khả năng riêng.
2. Có nhiều hoạt động ngoài combat đáng làm.
3. Những người xung quanh có ý chí và thông tin riêng.
4. Thế giới rộng hơn có những biến động không phụ thuộc player.

Không cần giải thích toàn bộ GDD.

<a id="m22-t053-s043"></a>

### 27. Tiến trình gợi ý

**Giai đoạn đầu:** Hoàn thành một tình huống nhỏ tại nơi xuất thân.

**Giai đoạn kế tiếp:** Được mở quyền tự do hành động trong phạm vi local area hoặc settlement.

**Giai đoạn tiếp theo:** Tiếp xúc với một Story Thread hoặc Personal Intent có nhiều cách tiếp cận.

**Giai đoạn cuối giờ đầu:** Nhận thấy một dấu hiệu của Historical Campaign hoặc một biến động vùng.

Tùy xuất thân, các bước có thể diễn ra khác thứ tự.

Một thợ săn có thể ra rừng trước khi gặp nhiều NPC.

Một Sứ Giả trẻ có thể dành phần lớn thời gian đầu cho hội thoại.

Một nhân vật đã lớn tuổi có thể bắt đầu với những vấn đề trong gia đình hoặc nghề nghiệp.

<a id="m22-t053-s044"></a>

### 28. Những hệ thống nào nên xuất hiện trong giờ đầu?

Nên giới thiệu bằng hành động thực tế:

- Character Skills;
- Observation;
- Dialogue;
- Time;
- Inventory;
- Map Knowledge;
- Consequence;
- Chronicle.

Những hệ thống sau không cần xuất hiện ngay nếu không liên quan:

- Class Tree;
- Mạch Lạc;
- Faction Politics;
- Workshop Management;
- Army Logistics;
- Succession;
- Myth Formation.

Ví dụ nếu character có Class sẵn từ lịch sử trước khi game bắt đầu, UI vẫn cần cho xem và sử dụng các Technique phù hợp; chỉ không cần mở toàn bộ bài giảng về Class Emergence.

Tutorial phải phản ứng với **những gì người chơi thực sự đang sử dụng**, không theo một lịch hướng dẫn bất biến.

---

<a id="m22-t053-s045"></a>

## PHẦN VIII — MƯỜI GIỜ ĐẦU

<a id="m22-t053-s046"></a>

### 29. Mục tiêu

Sau khoảng 10 giờ chơi, một player thông thường nên có:

- ít nhất một nhóm quan hệ đáng quan tâm;
- sự hiểu biết cơ bản về vùng xuất thân và một phần thế giới rộng hơn;
- một hướng phát triển nghề nghiệp hoặc năng lực;
- một số quyết định có hậu quả;
- trải nghiệm ít nhất một Activity Loop đủ sâu;
- nhận thức rõ hơn về những biến động lịch sử.

Đây là **mục tiêu kiểm thử trải nghiệm**, không phải điều kiện cứng để mở khóa campaign.

Một player dành 10 giờ chỉ làm nghề vẫn có thể hợp lệ.

Nhưng gameplay nghề của họ phải đủ phong phú để không trở thành 10 giờ lặp một thao tác.

<a id="m22-t053-s047"></a>

### 30. Tiến trình phát triển nhân vật

Trong 10 giờ đầu, character có thể:

- củng cố nghề hiện có;
- tìm mentor;
- phát triển Skill;
- bắt đầu thể hiện evidence của một Class;
- mở rộng mạng quan hệ;
- hình thành Personal Intent;
- trải qua một biến cố làm thay đổi góc nhìn.

Không được đảm bảo mọi character đều đạt Class mới sau 10 giờ.

Một người đã trưởng thành có thể có Class ngay từ đầu.

Một người khác cần nhiều năm game-time để hình thành identity.

<a id="m22-t053-s048"></a>

### 31. Tiến trình khám phá thế giới

Player nên bắt đầu hiểu:

**Không phải mọi vùng nguy hiểm vì combat.**

Một tuyến đường có thể nguy hiểm vì nước.

Một khu rừng vì không biết đường.

Một settlement vì quan hệ chính trị.

Một workshop vì vật liệu hoặc quy trình.

Một nghi lễ vì tập tục.

Mỗi vùng và hoạt động cần tạo được một dạng thách thức đặc trưng.

<a id="m22-t053-s049"></a>

### 32. Tiến trình campaign

Player không nhất thiết phải tới Phong Châu trước giờ chơi thứ 10.

Nhưng cần có những dấu hiệu hợp lý cho thấy thế giới đang vận động.

Ví dụ:

- thương nhân kể chuyện quyền lực đang thay đổi;
- một người di cư tới làng;
- giá một loại vật liệu tăng;
- một thủ lĩnh gửi người tìm kiếm đồng minh;
- companion nhắc lại một lời thề;
- một nghi lễ bị thay đổi.

World Director có thể ưu tiên **độ rõ của các dấu hiệu có thật**, nhưng không được tự tạo biến cố lịch sử giả chỉ để bảo đảm player “bắt kịp cốt truyện”.

---

<a id="m22-t053-s050"></a>

## PHẦN IX — CẤU TRÚC MỘT PHIÊN CHƠI

<a id="m22-t053-s051"></a>

### 33. Session Loop

Một phiên chơi bình thường có thể dài ngắn khác nhau.

Game nên hỗ trợ player dừng sau khi đạt được một kết quả có ý nghĩa mà không bắt buộc hoàn thành toàn bộ quest.

Cấu trúc điển hình:

**Nhận biết tình trạng hiện tại → Chọn việc đáng làm → Thực hiện hoạt động → Nhận kết quả → Quyết định bước tiếp theo hoặc nghỉ.**

Ví dụ một phiên 30–45 phút:

- Xem những việc đang quan tâm.
- Chọn đi tới Bến Hạc.
- Chuẩn bị và lên đường.
- Giải quyết một sự kiện môi trường.
- Tới nơi và gặp người cần tìm.
- Nhận được thông tin.
- Nghỉ tại settlement.

Player kết thúc với một thay đổi rõ, dù không có một màn hình “Quest Complete”.

<a id="m22-t053-s052"></a>

### 34. Điểm dừng tự nhiên

Game nên có những **Natural Stopping Points**:

- trở về settlement;
- hoàn thành chuyến đi;
- sau một cuộc thương lượng;
- sau combat;
- trước một Time Skip lớn;
- khi kết thúc một ngày hoặc Activity;
- sau khi một Story Thread chuyển trạng thái.

Nên cho phép lưu tại nhiều thời điểm hợp lệ.

Không cần cấm lưu chỉ để tạo cảm giác Hardcore.

Đối với Legacy/Ironman, quy tắc lưu và tải sẽ chặt chẽ hơn, nhưng đây là một lựa chọn chế độ.

---

<a id="m22-t053-s053"></a>

## PHẦN X — THIẾT KẾ WORLD DIRECTOR

<a id="m22-t053-s054"></a>

### 35. Định nghĩa

World Director là:

> **Hệ điều phối cách các tình huống có thật trong World Simulation được lựa chọn, ưu tiên và trình bày để tạo ra trải nghiệm có nhịp độ, đa dạng, dễ tiếp cận nhưng không phá vỡ tính nhân quả.**

Nó không phải thần linh trong lore.

Không phải Mạch Lạc.

Không phải NPC.

Không phải LLM nắm quyền tạo sự thật.

Nó là một subsystem kỹ thuật phục vụ gameplay.

<a id="m22-t053-s055"></a>

### 36. World Director được phép làm gì?

World Director có thể:

- ưu tiên phơi bày Story Thread phù hợp;
- chọn kênh truyền đạt thông tin có thật;
- ưu tiên một số Encounter Eligibility đã tồn tại;
- kiểm soát số lượng lời mời tương tác xuất hiện cùng lúc;
- phân bổ nhịp giữa hoạt động căng thẳng và yên bình;
- tránh lặp lại quá nhiều một loại tình huống;
- đề xuất cơ hội nghỉ hoặc Downtime thông qua UI;
- quyết định khi nào tóm tắt một sự kiện nhỏ;
- chọn hình thức trình bày sự kiện phù hợp Character Knowledge;
- kích hoạt các template sự kiện tùy chọn **chỉ khi toàn bộ precondition nhân quả được thỏa mãn và Event Engine cho phép**.

Ví dụ player vừa trải qua một trận chiến nặng.

Trong vùng hiện có ba nội dung đủ điều kiện phơi bày:

- một cuộc tranh chấp nhỏ;
- một hoạt động lễ hội;
- một tin đồn về người mất tích.

Director có thể ưu tiên một hoạt động có nhịp nhẹ hơn.

Nhưng không được quyết định lễ hội tồn tại nếu settlement chưa từng có lễ hội hoặc các điều kiện tổ chức không hợp lệ.

<a id="m22-t053-s056"></a>

### 37. World Director không được làm gì?

Đây là các giới hạn kiến trúc bắt buộc:

| Hành vi bị cấm | Lý do |
|---|---|
| Tự giết NPC để tạo bi kịch | Phá tính nhân quả và quyền tự chủ NPC |
| Tăng HP enemy vì player chiến thắng quá dễ | Phá world consistency |
| Sinh vật phẩm quý vào inventory để cứu tiến độ | Phá Economy và Ownership |
| Di chuyển NPC tức thời để player dễ gặp | Phá World Timeline |
| Tự tạo bí mật hoặc đổi hung thủ | Phá Knowledge/Investigation |
| Ép companion rời party để tạo drama | Phá Companion Autonomy |
| Làm chiến tranh dừng chờ player | Phá Historical Simulation |
| Tự giảm thời gian hồi thương tích sau một thất bại | Phá Injury System |
| Bắt buộc tạo combat khi phiên chơi quá yên bình | Phá triết lý content-first |
| Cho NPC biết sự thật chưa có nguồn thông tin | Phá Knowledge Graph |

World Director chỉ được tổ chức trải nghiệm từ những trạng thái và khả năng hợp lệ.

---

<a id="m22-t053-s057"></a>

## PHẦN XI — KIẾN TRÚC ĐIỀU PHỐI ĐỀ XUẤT

<a id="m22-t053-s058"></a>

### 38. Phân tách các subsystem

```text
WORLD STATE
    |
    +--> World Simulation
    |       - Economy
    |       - Ecology
    |       - Population
    |       - Faction
    |       - Household
    |
    +--> World Event Engine
    |       - Pressure
    |       - Event Eligibility
    |       - Scheduled Events
    |       - Causal Consequence
    |
    +--> Historical Anchor Controller
    |       - Campaign Era
    |       - Major Historical Conditions
    |       - Act Transitions
    |
    +--> Story Thread Registry
            - Existing Conflicts
            - Actor Goals
            - Unresolved Outcomes
            |
            v
       WORLD DIRECTOR
            |
            +--> Exposure Selection
            +--> Pacing Control
            +--> Encounter Prioritization
            +--> Novelty / Repetition Control
            +--> Recovery & Downtime Opportunities
            |
            v
       PLAYER KNOWLEDGE FILTER
            |
            v
       QUEST / JOURNAL / DIALOGUE / ENCOUNTER UI
            |
            v
       PLAYER ACTION
            |
            v
       DETERMINISTIC RULE ENGINE
            |
            v
       WORLD STATE UPDATE
```

Bất kỳ LLM nào được dùng để mô tả cảnh hoặc tạo hội thoại đều nằm ở lớp trình bày, sau bước lọc kiến thức.

Không được cho LLM đi tắt tới World State Update.

<a id="m22-t053-s059"></a>

### 39. World Simulation và Director khác nhau ở đâu?

Ví dụ một vùng có nguy cơ lũ.

**World Simulation** xác định:

- lượng mưa;
- mực nước;
- tuyến sông;
- khả năng vỡ đê;
- những settlement có thể bị ảnh hưởng.

**Event Engine** xác định:

- biến cố nào đủ điều kiện xảy ra;
- diễn biến theo thời gian;
- các actor tham gia;
- hậu quả tự nhiên.

**World Director** xác định:

- player có thể nhận cảnh báo qua nguồn nào;
- việc player đang làm có phù hợp để hiển thị thông tin hay không;
- cảnh báo nên được trình bày như một lời khuyên, một tin đồn hay dấu hiệu môi trường.

Nếu lũ đã bắt đầu, Director không được làm nước ngừng dâng để player kịp tới nơi.

Nếu player không có nguồn thông tin, Director không được tự cho họ biết chính xác sự cố ở một làng xa.

<a id="m22-t053-s060"></a>

### 40. Hai loại sự kiện cần phân biệt

<a id="m22-t053-s061"></a>

#### Sự kiện bắt buộc bởi World State

Ví dụ:

Một nhân vật đã bị thương nặng và tình trạng xấu đi.

Một con đê đã vượt ngưỡng chịu tải.

Một cuộc họp chính trị đã được ấn định.

Những sự kiện này được xử lý theo Event Scheduler. World Director không được trì hoãn vì muốn giảm nhịp căng thẳng.

<a id="m22-t053-s062"></a>

#### Sự kiện tùy chọn đã đủ điều kiện

Ví dụ:

Một người thợ có thể mời player tham gia đánh giá công cụ.

Một thương nhân có thể đề nghị hợp tác.

Một hunter có thể kể về dấu vết bất thường.

Nếu các điều kiện nhân quả đều hợp lệ, Director có thể chọn thời điểm thuận lợi để expose hoặc kích hoạt tình huống trong cửa sổ thời gian được phép.

Nếu cửa sổ hết hạn, cơ hội có thể mất hoặc được xử lý ngoài màn hình.

Đây là ranh giới quan trọng để Director không biến thành công cụ điều khiển thế giới tùy tiện.

---

<a id="m22-t053-s063"></a>

## PHẦN XII — PACING SYSTEM

<a id="m22-t053-s064"></a>

### 41. Tension không phải một thanh nguy hiểm duy nhất

Một lỗi thường gặp là coi tất cả căng thẳng như cùng một đại lượng.

Nhưng combat, grief, nghèo đói và xung đột quan hệ tạo ra những trải nghiệm rất khác nhau.

Đề xuất dùng **Pacing Vector** gồm các chiều:

| Chiều | Ý nghĩa | Ví dụ tăng |
|---|---|---|
| Physical Pressure | Nguy hiểm thể chất trực tiếp | Combat, lũ, thương tích |
| Emotional Pressure | Mất mát và xung đột cá nhân | Companion qua đời, gia đình tan vỡ |
| Cognitive Load | Số vấn đề người chơi phải phân tích | Nhiều manh mối, nhiệm vụ chồng chéo |
| Social Pressure | Nghĩa vụ và kỳ vọng của người khác | Lời hứa, thương lượng, faction |
| Resource Pressure | Thiếu thời gian hoặc vật chất | Hết lương thực, nợ, vật liệu |
| Historical Pressure | Biến động thời đại | Chiến tranh, chuyển quyền lực |

World Director đánh giá các chiều này riêng, thay vì cộng tất cả thành một số rồi tự phát ra “sự kiện nhẹ”.

Ví dụ sau một funeral, nhân vật có thể không bị nguy hiểm thể chất nhưng vẫn đang ở mức Emotional Pressure cao.

Nếu Director lập tức tạo một sự kiện hài hước chỉ vì không còn combat, trải nghiệm sẽ thiếu tinh tế.

<a id="m22-t053-s065"></a>

### 42. Sáu trạng thái nhịp trải nghiệm

Đề xuất sử dụng sáu trạng thái thiết kế.

<a id="m22-t053-s066"></a>

#### Ordinary Life — Đời sống thường lệ

Player tập trung vào nghề nghiệp, gia đình, khám phá hoặc mục tiêu cá nhân.

Director ưu tiên những cơ hội đã tồn tại phù hợp với lối chơi hiện tại.

Không cần tạo khủng hoảng chỉ vì player đang yên ổn.

<a id="m22-t053-s067"></a>

#### Curiosity — Tò mò

Một dấu hiệu bất thường xuất hiện.

Có thể là tin đồn, vật lạ, người mới đến hoặc sự thay đổi ở settlement.

Player được quyền tìm hiểu hoặc bỏ qua.

<a id="m22-t053-s068"></a>

#### Rising Pressure — Áp lực tăng

Các vấn đề liên kết trở nên rõ ràng.

Một sự kiện có deadline; tài nguyên thiếu; các bên bất đồng.

Player có nhiều cơ hội chuẩn bị.

<a id="m22-t053-s069"></a>

#### Crisis — Khủng hoảng

Một kết quả quan trọng sắp được xác định.

Combat, tai nạn, xung đột, thương lượng hoặc quyết định lớn có thể xảy ra.

<a id="m22-t053-s070"></a>

#### Aftermath — Hậu quả

World Simulation xử lý thương tích, quan hệ, tổn thất, tin đồn và thay đổi cộng đồng.

Không được vội vã chuyển ngay sang khủng hoảng tiếp theo chỉ vì player vừa hoàn thành nhiệm vụ.

<a id="m22-t053-s071"></a>

#### Consolidation — Củng cố

Player có cơ hội tận dụng hoặc xử lý kết quả:

- nghỉ ngơi;
- thay đổi kế hoạch;
- gặp người thân;
- nhận công việc;
- xây dựng quan hệ;
- truyền nghề;
- thực hiện Downtime.

Đây là trạng thái cần được xem có giá trị ngang Crisis.

<a id="m22-t053-s072"></a>

### 43. Pacing không đồng nghĩa điều khiển cảm xúc

World Director không thể biết chính xác người chơi ngoài đời đang buồn, chán hay hứng thú.

Nó chỉ có thể quan sát những tín hiệu gameplay:

- loại sự kiện vừa trải qua;
- số nhiệm vụ khẩn cấp đang hiển thị;
- số lần chiến đấu gần đây;
- tình trạng party;
- thời gian dành cho hoạt động nào;
- số lần player chủ động tìm một loại gameplay;
- mức độ thay đổi World State.

Không nên suy ra những trạng thái tâm lý cá nhân quá chính xác từ hành vi chơi game.

Một người chơi dành một giờ làm nghề có thể đang rất thích, không phải bị kẹt.

<a id="m22-t053-s073"></a>

### 44. Recovery Window

Sau các biến cố nặng, Director có thể giảm mức ưu tiên phơi bày **những tình huống tùy chọn** gây thêm áp lực.

Nhưng Recovery Window không có nghĩa:

- mọi enemy tạm biến mất;
- thương tích tự hồi nhanh;
- chiến tranh ngừng;
- người thân không thể gặp biến cố;
- deadline bị dừng.

Những sự kiện có nhân quả độc lập vẫn xảy ra.

Recovery Window chủ yếu là nguyên tắc **không chủ động chất thêm sự kiện không cần thiết lên player**.

---

<a id="m22-t053-s074"></a>

## PHẦN XIII — STORY EXPOSURE & CONTENT PRIORITIZATION

<a id="m22-t053-s075"></a>

### 45. Story Thread không đồng nghĩa nội dung phải hiển thị ngay

Trong thế giới luôn có nhiều vấn đề:

- tranh chấp gia đình;
- thiếu vật liệu;
- người mất tích;
- xung đột faction;
- lời hứa;
- các hoạt động cộng đồng.

Không phải mọi vấn đề đều cần xuất hiện trên giao diện.

World Director cần xác định **Exposure Priority**.

Các yếu tố đề xuất:

**Causal Eligibility:** Thread có thật và điều kiện phơi bày hợp lệ.

**Knowledge Access:** Có kênh thông tin cho player hay không.

**Character Relevance:** Nó liên quan thế nào tới xuất thân, nghề, quan hệ và lịch sử character.

**Intervention Value:** Player hiện có thể tạo ra sự khác biệt gì.

**Temporal Relevance:** Thread có còn cơ hội can thiệp hay không.

**Novelty:** Nó có lặp lại loại trải nghiệm gần đây không.

**Pacing Fit:** Tình huống có phù hợp nhịp hiện tại không.

Tất cả là yếu tố ưu tiên, **không phải công thức cộng điểm có thể vượt qua điều kiện tồn tại thực tế của thread**.

<a id="m22-t053-s076"></a>

### 46. Exposure Budget

Đề xuất prototype giới hạn mức độ chú ý mà UI yêu cầu người chơi.

Trong một thời điểm:

- một mối quan tâm chính có thể được nhấn mạnh;
- một vài mối quan tâm khác được trình bày nhẹ;
- các rumor hoặc cơ hội ít khẩn cấp nằm trong Journal/World;
- những vấn đề không liên quan tiếp tục tồn tại nhưng không tạo notification dồn dập.

Các con số cụ thể phải được kiểm thử, không nên khóa cứng theo cách khiến player không thể nhận nhiều vấn đề quan trọng cùng lúc.

Nếu ba người thân gặp nguy hiểm thật trong cùng ngày, UI không được giấu hai người chỉ vì vượt giới hạn notification.

<a id="m22-t053-s077"></a>

### 47. Novelty Control

Không nên dùng một thuật toán chống lặp đơn giản làm mất logic thế giới.

Ví dụ player đang sống trong vùng có dịch bệnh thật.

Các sự kiện liên quan dịch bệnh xuất hiện thường xuyên là hợp lý.

Director không thể nói:

> “Đã có một quest chữa bệnh rồi, cần chuyển sang bandit.”

Thay vào đó, cần chống lặp ở **hình thức tương tác**.

Cùng một cuộc khủng hoảng dịch bệnh có thể tạo:

- khám bệnh;
- tìm nguồn nước;
- tranh chấp về nơi cách ly;
- tổ chức vận chuyển thuốc;
- xung đột thông tin;
- chăm sóc người thân.

Causal theme vẫn thống nhất nhưng gameplay đa dạng.

<a id="m22-t053-s078"></a>

### 48. Discovery vs Notification

Một trong những nguyên tắc quan trọng nhất:

**Không phải mọi nội dung đáng chơi đều cần được thông báo trực tiếp.**

Một player thích khám phá có thể tìm một dấu tích trong rừng trước khi bất kỳ NPC nào nhắc đến nó.

Một người làm nghề có thể phát hiện dấu hiệu khan hiếm vật liệu trước khi xuất hiện quest kinh tế.

Một Sứ Giả có thể chủ động hỏi chuyện faction.

World Director phải tôn trọng việc player tự khám phá và không làm mọi con đường cuối cùng trở thành một dấu chấm trên bản đồ.

---

<a id="m22-t053-s079"></a>

## PHẦN XIV — WORLD DIRECTOR DECISION FLOW

<a id="m22-t053-s080"></a>

### 49. Quy trình ra quyết định

Đề xuất logic điều phối:

```text
1. Read current World Snapshot

2. Collect:
   - Existing Story Threads
   - Eligible Optional Events
   - Scheduled Mandatory Events
   - Character/Party State
   - Current Knowledge
   - Recent Gameplay History

3. Separate mandatory consequences
   from discretionary presentation opportunities

4. Validate candidate eligibility:
   - Actors exist and are available
   - Location is valid
   - Time window is valid
   - Required resources exist
   - Information route exists
   - Historical constraints hold

5. Score valid opportunities:
   - Relevance
   - Intervention value
   - Pacing fit
   - Novelty
   - Current player intent

6. Choose:
   - Surface a thread
   - Present an ambient clue
   - Offer an eligible interaction
   - Summarize background activity
   - Present nothing new

7. Apply Player Knowledge Filter

8. Send presentation request to UI/LLM

9. Record Director Decision

10. Wait for actual Player Action
    before modifying World State
```

Điểm số ở bước 5 chỉ dùng để **xếp hạng cơ hội hợp lệ**.

Nó không thể làm một sự kiện không hợp lệ trở thành hợp lệ.

<a id="m22-t053-s081"></a>

### 50. Director Decision Log

Mỗi quyết định quan trọng cần có log để debug.

Ví dụ:

```json
{
  "director_state": "consolidation",
  "player_region": "Tam Giang",
  "recent_pressure": {
    "physical": "high",
    "emotional": "moderate",
    "cognitive": "low"
  },
  "candidate_thread": "repair_ferry_017",
  "eligibility": "valid",
  "exposure_channel": "known_boatman",
  "decision": "surface_as_optional_conversation",
  "reason": [
    "existing_world_need",
    "relevant_to_profession",
    "non_urgent",
    "valid_information_source"
  ]
}
```

Dữ liệu trên chỉ là mẫu cấu trúc, không phải schema kỹ thuật cuối cùng.

Mục đích là khi có lỗi, team có thể trả lời:

> Tại sao player nhận được thông tin này?

> Tại sao Director ưu tiên event đó?

> Có actor hoặc nguồn thông tin hợp lệ không?

Không nên phải suy đoán quyết định của một LLM hộp đen.

---

<a id="m22-t053-s082"></a>

## PHẦN XV — DIFFICULTY PHILOSOPHY

<a id="m22-t053-s083"></a>

### 51. Độ khó không chỉ là combat

Với GDD này, thách thức xuất hiện trên nhiều chiều:

| Chiều khó | Ví dụ | Năng lực đối phó |
|---|---|---|
| Combat | Đội hình, thương tích, phản ứng | Weapon Skill, Tactics, Preparation |
| Exploration | Đường đi, dấu vết, địa hình | Navigation, GIÁC, Knowledge |
| Survival | Thời gian, lương thực, thời tiết | Planning, Profession, Resources |
| Social | Niềm tin, nghĩa vụ, ranh giới | Relationship, Evidence, Negotiation |
| Economic | Thiếu nguồn lực, vận tải, nợ | Trade, Craft, Network |
| Investigation | Thông tin không đầy đủ | Observation, Reasoning, Knowledge |
| Strategic | Nhiều mục tiêu xung đột | Priority, Time, Alliances |
| Legacy | Truyền nghề, thừa kế, sự hữu hạn | Preparation, Relationships, Succession |

Không thể định nghĩa một chế độ khó chỉ bằng:

> Enemy Damage +50%.

<a id="m22-t053-s084"></a>

### 52. Challenge phải đến từ hoàn cảnh

Một cây cầu bị lũ cuốn không trở nên bền hơn vì player có Skill cao.

Một thợ rèn không tự có thêm vật liệu vì player đang thiếu tài nguyên.

Một thủ lĩnh không trở nên dễ thuyết phục chỉ vì player gặp thất bại ba lần.

Thách thức được hình thành từ:

**World State + Character Capability + Knowledge + Preparation + Consequence.**

Vì vậy game cần tránh hệ thống level scaling tuyệt đối.

Một người giỏi có thể giải quyết một tình huống nhanh chóng vì đã đầu tư nhiều năm vào nghề.

Điều đó là phần thưởng đúng nghĩa của Mastery.

<a id="m22-t053-s085"></a>

### 53. Khó không đồng nghĩa bất công

Một thử thách khó có thể hợp lý nếu:

- có dấu hiệu nhận biết nguy hiểm;
- tồn tại cách chuẩn bị;
- có lựa chọn rút lui;
- hậu quả nhất quán;
- nhân vật có thể học từ thất bại;
- hệ thống không bí mật thay quy tắc.

Ngược lại, một sự kiện chết ngay không báo trước và không có agency có thể là bất công dù xảy ra hiếm.

Game cần đánh giá **Fairness** riêng với Difficulty.

---

<a id="m22-t053-s086"></a>

## PHẦN XVI — CÁC GAME MODES ĐỀ XUẤT

<a id="m22-t053-s087"></a>

### 54. Cấu trúc chế độ chơi

Tôi đề xuất phân biệt:

**Difficulty Preset:** Điều chỉnh mức độ rủi ro và hỗ trợ.

**Interface Assistance:** Điều chỉnh cách trình bày thông tin, không thay đổi sự thật.

**Persistence Rule:** Điều chỉnh quy tắc lưu/tải và tính không thể đảo ngược.

**World/Campaign Settings:** Các thiết lập liên quan simulation và cấu trúc chiến dịch, chỉ tùy chỉnh khi không phá game-canon.

Cách phân tách này tốt hơn tạo quá nhiều mode đóng gói những thay đổi không liên quan.

<a id="m22-t053-s088"></a>

### 55. Story Mode — Trải nghiệm câu chuyện

**Đối tượng:** Người thích khám phá thế giới, character development, quan hệ và lịch sử nhưng không muốn rủi ro cơ học quá cao.

**Định hướng:**

Combat vẫn có chiến thuật nhưng hậu quả thể chất dễ xử lý hơn.

Dấu hiệu nguy hiểm dễ đọc hơn.

Thông tin hướng dẫn được trình bày rõ khi character có đủ nguồn kiến thức.

Các cửa sổ cứu chữa và ổn định thương tích có thể rộng hơn theo những quy tắc đã cấu hình.

Survival ít gây áp lực vật chất.

**Những gì không thay đổi:**

- NPC vẫn có ý chí;
- lời hứa vẫn có hậu quả;
- faction vẫn hoạt động;
- lịch sử vẫn tiến;
- không có vật phẩm xuất hiện vô điều kiện;
- không có Persuasion tự động phá Moral Boundary.

Story Mode phải nhẹ về rủi ro nhưng không làm thế giới giả tạo.

<a id="m22-t053-s089"></a>

### 56. Standard Mode — Trải nghiệm chuẩn

Đây là mức độ được dùng làm **design baseline**.

Hệ thống đầy đủ:

- combat có rủi ro thật;
- injury cần điều trị;
- survival được abstract vừa phải;
- NPC và World Simulation hoạt động bình thường;
- death/succession có thể xảy ra;
- lời hứa và deadline có hậu quả;
- không có enemy level scaling.

Standard Mode là cấu hình cần được cân chỉnh đầu tiên, không phải một mode ở giữa được thiết kế sau Story và Tactical.

<a id="m22-t053-s090"></a>

### 57. Tactical Mode — Thử thách hệ thống

**Đối tượng:** Người muốn các quyết định về combat, hành trình, vật tư và kế hoạch có sức nặng hơn.

Khác Standard ở:

- khả năng xử lý tình huống của đối thủ có huấn luyện được thể hiện đầy đủ hơn;
- các cơ chế formation, reaction, cover và resource pressure được áp dụng nghiêm hơn;
- rủi ro của những quyết định thiếu chuẩn bị cao hơn;
- những dấu hiệu không được character phát hiện sẽ không tự xuất hiện trong UI;
- các lỗi chiến thuật có thể gây hậu quả khó hồi phục hơn.

Nhưng không được:

- tăng HP địch vô cớ;
- làm thú tự nhiên hung dữ phi lý;
- cho quân lính biết player đang ở đâu khi chưa có intelligence;
- ép mọi encounter thành combat.

Tactical Mode tăng mức yêu cầu **hiểu và vận dụng hệ thống**, không đơn giản tăng số.

<a id="m22-t053-s091"></a>

### 58. Legacy / Ironman Mode — Cuộc đời không thể viết lại

Đây là một chế độ persistence và hậu quả dài hạn.

Định hướng:

- một World Timeline liên tục;
- thương tích, tử vong và kết quả lớn được giữ theo quy tắc lưu nghiêm ngặt;
- Succession trở thành trọng tâm trải nghiệm;
- những quyết định đã thực thi không dễ đảo ngược bằng reload.

Có thể kết hợp với mức rủi ro Standard hoặc Tactical tùy thiết kế cuối.

Một điểm cần khóa sớm: nếu sử dụng Ironman thực sự, quy tắc lưu phải bảo vệ khỏi mất dữ liệu do lỗi kỹ thuật, mất điện hoặc crash, chứ không được đồng nhất lỗi phần mềm với hậu quả gameplay.

<a id="m22-t053-s092"></a>

### 59. Explorer Assistance — Cấu hình hỗ trợ độc lập

Explorer Assistance không nhất thiết là difficulty mode riêng.

Nó có thể gồm:

- gợi ý tuyến đường từ kiến thức nhân vật;
- nhấn mạnh dấu hiệu tương tác có thể nhận biết;
- hiển thị nguồn thông tin và độ tin cậy rõ hơn;
- tóm tắt các manh mối đã biết;
- cải thiện cách giải thích trạng thái thương tích;
- hỗ trợ quản lý thông tin trong Journal.

Quan trọng: những lựa chọn này **không mở khóa thông tin mà character chưa biết**.

Chẳng hạn, chế độ hỗ trợ có thể nhắc:

> “Vũ từng nói có một lối đi cũ phía bắc.”

Nhưng không được vẽ chính xác lối đi nếu Vũ chỉ biết vị trí tương đối.

---

<a id="m22-t053-s093"></a>

## PHẦN XVII — SO SÁNH CÁC CHẾ ĐỘ

<a id="m22-t053-s094"></a>

### 60. Ma trận đề xuất

| Tiêu chí | Story | Standard | Tactical | Legacy/Ironman |
|---|---|---|---|---|
| Lethality | Thấp hơn | Chuẩn | Nghiêm hơn | Theo mức rủi ro đã chọn |
| Injury Persistence | Có, dễ hồi phục hơn | Đầy đủ | Đầy đủ, hệ quả nghiêm hơn | Đầy đủ và khó đảo ngược |
| Combat Complexity | Hỗ trợ rõ | Chuẩn | Khai thác sâu | Theo mức rủi ro đã chọn |
| Survival Pressure | Nhẹ | Cân bằng | Cao hơn theo bối cảnh | Theo mức rủi ro đã chọn |
| Information Assistance | Rõ hơn | Theo kiến thức nhân vật | Ít gợi ý ngoài quan sát | Tùy cấu hình |
| Social Autonomy | Đầy đủ | Đầy đủ | Đầy đủ | Đầy đủ |
| World Simulation | Đầy đủ | Đầy đủ | Đầy đủ | Đầy đủ |
| Historical Anchors | Giống nhau | Giống nhau | Giống nhau | Giống nhau |
| Permadeath | Giảm nguy cơ | Có thể xảy ra | Nguy hiểm hơn | Hậu quả không dễ đảo ngược |
| Save Rule | Linh hoạt | Linh hoạt | Linh hoạt | Hạn chế theo chế độ |

Legacy được trình bày như cấu hình có thể kết hợp thay vì chỉ là cấp khó thứ tư xếp tuyến tính.

**Đề xuất khóa:** Story, Standard và Tactical là ba preset thử thách; Legacy/Ironman là lựa chọn persistence bổ sung, có thể đặt sẵn các cấu hình nghiêm ngặt.

---

<a id="m22-t053-s095"></a>

## PHẦN XVIII — DYNAMIC DIFFICULTY ADJUSTMENT

<a id="m22-t053-s096"></a>

### 61. Có nên dùng Dynamic Difficulty Adjustment?

**Có, nhưng giới hạn rất rõ.**

Không dùng DDA để bí mật thay đổi sức mạnh và luật thế giới.

Sử dụng **Adaptive Presentation & Opportunity Pacing**.

Hệ thống có thể thích ứng ở:

- cách hiển thị thông tin hợp lệ;
- thời điểm giới thiệu một cơ chế;
- tần suất encounter tùy chọn;
- mức độ gợi ý các nguồn hỗ trợ đã tồn tại;
- khả năng đề xuất nghỉ ngơi, chuẩn bị hoặc hỏi người có chuyên môn.

Không thích ứng ở:

- kết quả xúc xắc đã được xác định;
- danh tính hung thủ;
- tài sản thật;
- sức mạnh enemy đã tồn tại;
- sự kiện lịch sử bắt buộc;
- cái chết đã xảy ra;
- thông tin mà NPC không biết.

<a id="m22-t053-s097"></a>

### 62. Ví dụ player liên tục thua combat

Một DDA thông thường có thể tự giảm HP enemy.

Game này không nên làm vậy.

Thay vào đó, World Director có thể nhận thấy player đang gặp khó trong một tuyến xung đột.

Nếu có nguồn hợp lệ, game có thể giúp player nhận ra:

- companion biết một lối đi vòng;
- một người huấn luyện đang ở settlement;
- thông tin về đội hình đối phương;
- khả năng rút lui;
- những dụng cụ hoặc phương án chuẩn bị có sẵn.

Không có gì bảo đảm player phải thắng.

Nhưng thất bại có thể tạo ra **cơ hội học hỏi rõ ràng hơn**.

<a id="m22-t053-s098"></a>

### 63. Ví dụ player giỏi vượt trội

Một Master Liệp Nhân xử lý dễ dàng những chuyến săn thường lệ.

Director không được liên tục sinh thú to hơn để giữ độ khó.

Thay vào đó:

- những hoạt động routine được abstract nhanh hơn;
- nghề nghiệp mở ra những yêu cầu khác về phạm vi, trách nhiệm và tổ chức;
- người khác tìm tới nhờ hướng dẫn;
- có cơ hội nhận apprentice;
- một số Story Thread đòi hỏi hiểu biết cao hơn có thể trở nên tiếp cận được.

Challenge phát triển theo **vai trò cuộc đời**, không chỉ sức mạnh của đối thủ.

<a id="m22-t053-s099"></a>

### 64. Khi nào tuyệt đối không thích ứng?

Nếu player biết một khu vực rất nguy hiểm nhưng vẫn chủ động vào, game không được bí mật giảm rủi ro chỉ vì họ còn yếu.

Nếu player chọn sống yên bình nhiều năm, game không được phạt bằng cách ép phát sinh kẻ thù mạnh.

Nếu một thảm họa đã được xác lập bởi World State, Director không được hủy nó vì thấy player vừa có nhiều mất mát.

Sự nhất quán phải là giới hạn trên của adaptive pacing.

---

<a id="m22-t053-s100"></a>

## PHẦN XIX — NO-COMBAT PLAYTHROUGH VIABILITY

<a id="m22-t053-s101"></a>

### 65. Vì sao cần kiểm tra riêng?

Game đã thiết kế nhiều Profession và Class ngoài chiến đấu.

Nếu cuối cùng chỉ có thể hoàn thành các hồi chính bằng cách đánh bại một trận boss, toàn bộ triết lý nghề nghiệp sẽ trở nên hình thức.

Vì vậy cần kiểm thử riêng một **No-Combat-Oriented Run**.

Đây không phải lời hứa rằng nhân vật có thể tránh mọi nguy hiểm vật lý trong mọi hoàn cảnh.

Nó có nghĩa:

> Một nhân vật không phát triển chiến đấu làm năng lực chính vẫn có những con đường hợp lý để trải nghiệm và tác động tới toàn bộ chiến dịch.

<a id="m22-t053-s102"></a>

### 66. Những cơ chế bảo đảm

**Alternate Resolution:** Nhiều Story Thread quan trọng có hướng giải thông qua quan hệ, tri thức, tổ chức hoặc kỹ thuật.

**Protection by Others:** Player có thể thuê, thuyết phục hoặc đồng hành cùng người có năng lực chiến đấu.

**Avoidance:** Những rủi ro có thể quan sát và tránh khi hợp lý.

**Delegation:** Character có thể giao những việc phù hợp cho người đồng ý thực hiện, với thời gian và chi phí thật.

**Aftermath Intervention:** Nếu không thể ngăn xung đột, player có thể tham gia cứu chữa, tái thiết, điều tra hoặc hòa giải sau đó.

**Role-Relevant Historical Content:** Mỗi Historical Anchor phải có cửa tham gia cho các nghề nghiệp khác nhau.

Đây là yêu cầu của Campaign Design, không phải giải pháp tạm thời từ World Director.

---

<a id="m22-t053-s103"></a>

## PHẦN XX — ĐẶC TRƯNG GAMEPLAY CỦA 12 VÙNG

<a id="m22-t053-s104"></a>

### 67. Regional Gameplay Identity

Để Campaign có replayability, xuất thân ở các vùng phải mang lại **môi trường học gameplay khác nhau**, không chỉ thay màu địa hình hoặc NPC.

| Vùng | Gameplay thường trực | Áp lực đặc trưng | Hướng phát triển tự nhiên |
|---|---|---|---|
| Phong Châu | Quan hệ, thông tin, nghi lễ, chính trị | Uy tín, tập tục, quyền lực | Sứ Giả, Thủ Lĩnh, Giữ Ký Ức |
| Tam Giang | Vận tải, đánh cá, cứu nạn, đi sông | Mực nước, tuyến đi, thuyền | Thủy Binh, Liệp Nhân, Xảo Nhân |
| Tản Viên | Săn, đi núi, rừng, nơi thiêng | Địa hình, cô lập, tập tục | Liệp Nhân, Vu Chúc, Trinh Binh |
| Bãi Lạc | Nông nghiệp, thủy lợi, tranh chấp cộng đồng | Đất, nước, mùa vụ | Xảo Nhân, Thủ Lĩnh, Sứ Giả |
| Đồng Đỏ | Luyện kim, chế tác, vận tải vật liệu | Nhiên liệu, nghề, bí quyết | Xảo Nhân, Giữ Ký Ức |
| Lâm U | Rừng sâu, sinh thái, dấu vết | Phương hướng, nguồn lực, tín ngưỡng | Liệp Nhân, Trinh Binh, Dược Nhân |
| Thượng Âu | Đường núi, liên minh, mạng cộng đồng | Địa hình, quan hệ vùng | Sứ Giả, Trinh Binh, Thủ Lĩnh |
| Đại Trạch | Đầm lầy, sức khỏe, đường nước | Sương, nền đất, bệnh tật | Thủy Binh, Dược Nhân, Liệp Nhân |
| Hải Môn | Ven biển, muối, thương mại, tàu thuyền | Gió, tuyến biển, giao thương | Thủy Binh, Sứ Giả, Xảo Nhân |
| Việt Thường | Biên văn hóa, giao thương, khám phá | Kiến thức chưa đầy đủ, tập tục khác biệt | Sứ Giả, Giữ Ký Ức, Liệp Nhân |
| Bắc Lộ | Tuyến phòng thủ, người di cư, tin tức | An ninh, faction, chiến tranh | Lạc Binh, Hộ Vệ, Trinh Binh |
| Loa Vực | Công trình, xưởng, tổ chức lao động | Đất đai, sản xuất, quyền lực | Xảo Nhân, Thủ Lĩnh, Sứ Giả |

Đây là những trọng tâm gameplay, không phải hệ khóa Class theo vùng.

Một Dược Nhân ở Đồng Đỏ hoặc một chiến binh ở Phong Châu vẫn có thể phát triển hợp lý.

<a id="m22-t053-s105"></a>

### 68. Tại sao regional identity quan trọng với World Director?

Director không được sử dụng một thư viện event thống nhất cho mọi vùng.

Ví dụ khi cần phơi bày một sự kiện liên quan nguồn lực:

Tam Giang có thể xuất hiện vấn đề vận tải.

Đồng Đỏ có thể xuất hiện vấn đề nhiên liệu.

Bãi Lạc có thể xuất hiện xung đột phân phối nước.

Phạm vi sự kiện được chọn từ những vấn đề thực sự tồn tại.

Nhờ đó cùng một World State tổng quát như Resource Pressure vẫn tạo trải nghiệm phù hợp văn hóa, nghề nghiệp và địa lý.

---

<a id="m22-t053-s106"></a>

## PHẦN XXI — BA VÍ DỤ PLAYTHROUGH KHÁC NHAU

<a id="m22-t053-s107"></a>

### 69. Playthrough A — Người học nghề thuyền ở Tam Giang

**Xuất thân:** 17 tuổi, gia đình làm nghề vận tải.

**15 phút đầu:** Player xử lý một chuyến thuyền có nguy cơ quá tải.

**Giờ đầu:** Đi tuyến sông ngắn, gặp người quen, nhận biết sự thay đổi của dòng nước và các mối quan hệ ở bến.

**10 giờ đầu:** Bắt đầu có cơ hội học kỹ năng dẫn thuyền, tham gia cứu nạn và nghe tin về những biến động xa hơn.

**Giữa campaign:** Thủy Binh có thể hình thành; player tham gia mạng vận tải lớn.

**Hồi IV:** Nhu cầu hậu cần Cổ Loa tạo cơ hội nghề nghiệp.

**Hồi VI:** Player có thể dùng tuyến thuyền và các quan hệ để cứu người, vận chuyển hàng hoặc tham gia nhiệm vụ nguy hiểm.

Trục phát triển:

**Từ người học việc → người hiểu sông → người có trách nhiệm với mạng lưới vận tải.**

<a id="m22-t053-s108"></a>

### 70. Playthrough B — Thợ đúc 31 tuổi ở Đồng Đỏ

**Xuất thân:** Có nghề ổn định, vài quan hệ nghề nghiệp, một số nghĩa vụ gia đình.

**15 phút đầu:** Một đơn hàng gặp vấn đề chất lượng.

**Giờ đầu:** Player tìm nguyên nhân, thương lượng vật liệu và xử lý quan hệ với người cùng nghề.

**10 giờ đầu:** Học sâu hơn về production chain, nhận một commission khó, có thể mở quan hệ với một workshop khác.

**Giữa campaign:** Giá kim loại và nhu cầu quân sự thay đổi.

**Hồi IV:** Player có cơ hội tham gia hoặc từ chối dự án sản xuất phục vụ Cổ Loa.

**Hồi V:** Công nghệ nỏ và chuẩn hóa sản xuất trở thành xung đột nghề nghiệp, đạo đức và chính trị.

**Hồi VI:** Player có thể cố bảo vệ học trò, workshop, bí quyết hoặc một người thân trong thời kỳ khủng hoảng.

Trục phát triển:

**Từ người thợ có tay nghề → người quyết định tri thức và sản phẩm của mình sẽ phục vụ ai.**

Không cần bắt người thợ trở thành chiến binh.

<a id="m22-t053-s109"></a>

### 71. Playthrough C — Người giữ lễ 54 tuổi ở Tản Viên

**Xuất thân:** Đã có uy tín nhất định, hiểu tập tục, sức khỏe không hoàn hảo.

**15 phút đầu:** Một gia đình có tranh chấp liên quan nghi lễ hoặc nơi chôn cất.

**Giờ đầu:** Player sử dụng Knowledge, Social Standing và Ritual Skill để hiểu các phía.

**10 giờ đầu:** Bắt đầu gặp những mâu thuẫn giữa tập tục cũ và sự thay đổi cộng đồng.

**Giữa campaign:** Những lời thề và truyền thống địa phương bị đặt vào hoàn cảnh mới.

**Hồi IV:** Việc xây dựng và di chuyển dân cư có thể ảnh hưởng các địa điểm thiêng.

**Hồi V:** Vai trò của ký ức, nghi lễ và tính chính danh chính trị trở nên quan trọng.

**Hồi VI:** Nếu player qua đời hoặc không còn khả năng đi lại, một apprentice hoặc người kế tục có thể tiếp nhận một phần những Story Thread họ để lại.

Trục phát triển:

**Từ người gìn giữ truyền thống → người quyết định truyền lại, thay đổi hay chấm dứt một truyền thống.**

Playthrough này có thể ngắn hơn về đời sống nhân vật, nhưng phải không kém về chiều sâu lựa chọn.

---

<a id="m22-t053-s110"></a>

## PHẦN XXII — KHI PLAYER KHÔNG BIẾT PHẢI LÀM GÌ

<a id="m22-t053-s111"></a>

### 72. Phân biệt tự do và thiếu định hướng

Một sandbox tốt tạo ra nhiều lựa chọn có ý nghĩa.

Một sandbox kém khiến player mở bản đồ, không hiểu đâu là việc đáng làm và bắt đầu đi lòng vòng.

Vì vậy game cần hỗ trợ **situational guidance**, không phải waypoint bắt buộc.

<a id="m22-t053-s112"></a>

### 73. Current Concerns

UI có thể hiển thị các mối quan tâm hiện tại:

- một công việc còn dang dở;
- một lời hứa;
- một người cần gặp;
- một cơ hội nghề nghiệp;
- một sự kiện sắp diễn ra;
- một điều player muốn tìm hiểu.

Nội dung dựa trên Character Knowledge và Story Thread đã biết.

Nếu không có quest nào, player vẫn có thể nhìn thấy những Personal Intent và hoạt động phù hợp.

<a id="m22-t053-s113"></a>

### 74. Companion Advice

Companion có thể đưa gợi ý tùy khả năng và mối quan tâm của họ.

Ví dụ:

Một Liệp Nhân đề xuất tìm dấu vết ở tuyến rừng.

Một Dược Nhân đề nghị trở về khi có người bị thương.

Một Sứ Giả cho rằng nên tìm gặp trưởng bến trước khi thương lượng.

Lời khuyên có thể xung đột.

Không có companion nào được coi là giọng nói chính thức của game.

<a id="m22-t053-s114"></a>

### 75. Không được tạo “Emergency Quest” để cứu sự nhàm chán

Nếu player dành nhiều thời gian làm nghề, Director không nên mặc định tạo một vụ cướp hoặc người mất tích.

Trước tiên cần phân biệt:

- player đang chủ động tận hưởng routine gameplay;
- player đang chờ một công trình hoặc điều kiện;
- player không hiểu cơ chế tiếp theo;
- player thiếu thông tin;
- một Activity đã không còn tạo ra quyết định.

Với ba tình huống sau, giải pháp ưu tiên là cải thiện UI, phản hồi và thiết kế Activity.

Không dùng biến cố bạo lực để che một gameplay loop yếu.

---

<a id="m22-t053-s115"></a>

## PHẦN XXIII — KHI THẾ GIỚI CÓ QUÁ NHIỀU BIẾN CỐ

<a id="m22-t053-s116"></a>

### 76. Vấn đề Narrative Saturation

World Simulation có thể tạo ra nhiều thay đổi cùng lúc.

Ví dụ:

- cha player bị bệnh;
- faction yêu cầu một chuyến hàng;
- companion muốn về quê;
- settlement bị thiếu food;
- một nghi lễ lớn sắp diễn ra.

Nếu game đưa tất cả lên UI với mức ưu tiên giống nhau, player sẽ bị quá tải.

<a id="m22-t053-s117"></a>

### 77. Phân loại thông tin

**Immediate:** Những gì cần quyết định ngay vì character đang trực tiếp tham gia.

**Time-Sensitive:** Những vấn đề có cửa sổ can thiệp đáng quan tâm.

**Current Concerns:** Những việc player đang chủ động theo đuổi.

**Background Developments:** Những thay đổi đã biết nhưng chưa cần hành động.

**Historical Record:** Những sự kiện đã qua.

World Director có trách nhiệm tổ chức sự chú ý giữa các nhóm.

Nhưng phân loại UI không được làm thời gian thế giới tạm dừng.

<a id="m22-t053-s118"></a>

### 78. Khi các nghĩa vụ thật sự xung đột

Player có thể phải lựa chọn.

Ví dụ:

Cha đang ốm ở quê.

Faction yêu cầu đưa hàng trước mùa nước cao.

Một companion có việc khẩn cấp.

Game không cần tạo một giải pháp hoàn hảo cho mọi lựa chọn.

Player có thể:

- ưu tiên gia đình;
- gửi người khác;
- thương lượng gia hạn;
- rút khỏi hợp đồng;
- chấp nhận mất quan hệ;
- hoặc tìm một cách phối hợp nếu đủ điều kiện.

Đây là gameplay có giá trị.

Director không được âm thầm giải quyết một nghĩa vụ chỉ để player tránh phải đưa ra lựa chọn khó.

---

<a id="m22-t053-s119"></a>

## PHẦN XXIV — WORLD DIRECTOR VÀ SUCCCESSION

<a id="m22-t053-s120"></a>

### 79. Sau khi character chết, nhịp campaign phải thay đổi tự nhiên

Succession là một bước ngoặt lớn.

Nếu character cũ chết và ngay sau đó game tiếp tục trình bày tất cả nhiệm vụ y hệt như trước, người chơi sẽ cảm thấy việc thay nhân vật chỉ là cơ chế kỹ thuật.

World Director phải chuyển sang **Transition Pacing**.

Giai đoạn này ưu tiên:

- hậu quả cái chết;
- người successor đang ở đâu;
- những gì họ biết;
- quan hệ với nhân vật đã chết;
- trách nhiệm mà họ có thể nhận hoặc từ chối.

Nhưng không được tự đặt cho họ mục tiêu báo thù.

Một successor có thể muốn tiếp tục nghề nghiệp hoặc bảo vệ gia đình thay vì truy tìm nguyên nhân cái chết.

<a id="m22-t053-s121"></a>

### 80. Không reset độ khó vì succession

World State vẫn tồn tại.

Nếu character mới yếu hơn trong combat, kẻ thù không tự giảm sức mạnh.

Tuy nhiên, player vẫn có các con đường tương ứng với năng lực của successor.

World Director có thể ưu tiên giới thiệu lại những công cụ phù hợp với character mới:

- mạng quan hệ;
- nghề nghiệp;
- nguồn thông tin;
- người có thể hợp tác;
- điều kiện đi lại;
- hoạt động thường lệ.

Những cơ hội này phải vốn tồn tại hoặc đủ điều kiện nhân quả, không phải phần thưởng an ủi được tạo từ hư không.

---

<a id="m22-t053-s122"></a>

## PHẦN XXV — CONTENT PIPELINE & EVENT TEMPLATES

<a id="m22-t053-s123"></a>

### 81. World Director không thể sửa một thư viện nội dung kém

Nếu mỗi encounter đều là:

> gặp người lạ → họ nhờ lấy đồ → nhận thưởng,

Director dù tốt đến đâu cũng không thể tạo một RPG sâu.

Vì vậy nội dung cần được authored với những thành phần rõ ràng.

<a id="m22-t053-s124"></a>

### 82. Event Template

Một Event Template cần:

```text
Event Identity
Causal Origin
Required World State
Eligible Actors
Actor Goals
Eligible Locations
Time Window
Information Sources
Player Entry Methods
Available Approaches
Risk Dimensions
Potential Outcomes
Persistent Changes
Follow-up Threads
Presentation Variants
Pacing Profile
```

Trong đó **Pacing Profile** chỉ là metadata giúp Director lựa chọn và trình bày.

Nó không định nghĩa sự kiện có thật hay không.

<a id="m22-t053-s125"></a>

### 83. Event Families cần đủ chiều sâu

Để cân bằng các lối chơi, thư viện nội dung cần hỗ trợ những họ sự kiện sau.

**Đời sống và gia đình:** Hôn nhân, học nghề, bệnh tật, mâu thuẫn giữa thế hệ, đón người thân trở về, chia tách household, thừa kế. Trọng tâm là quan hệ, nghĩa vụ, ký ức và sự thay đổi vai trò; không phải mọi tình huống đều được biến thành quest có thưởng.

**Nghề nghiệp và sản xuất:** Hợp đồng, lỗi kỹ thuật, thiếu vật liệu, cạnh tranh workshop, truyền bí quyết, thử nghiệm, tổ chức lao động. Những tình huống này phải sử dụng Production, Crafting và Economy thật, đồng thời có khả năng tạo kết quả vật chất tồn tại lâu dài.

**Khám phá và hành trình:** Tuyến mới, sai lệch bản đồ, thay đổi thời tiết, người lạ trên đường, dấu vết thú, địa điểm bỏ hoang. Gameplay xoay quanh thông tin, thời gian, thiết bị và lựa chọn hành trình, không mặc định biến mọi chuyến đi thành một trận đánh.

**Xã hội và chính trị:** Lời hứa, quyền sử dụng đất, tố cáo, nghi lễ, tranh chấp thương mại, faction, legitimacy. NPC tham gia phải có động cơ và ranh giới riêng; kết quả có thể là thỏa thuận có điều kiện hoặc xung đột dài hạn.

**Nguy hiểm và chiến đấu:** Bảo vệ, chạy trốn, bắt sống, giữ vị trí, cứu người, chiến tranh, săn thú nguy hiểm. Mọi tình huống phải có mục tiêu và nguyên nhân, đồng thời hỗ trợ retreat hoặc giải pháp khác khi hoàn cảnh cho phép.

**Mạch Lạc và ký ức:** Giấc mơ, truyền thuyết, địa điểm thiêng, nghi lễ bị gián đoạn, lời thề cũ, hiện tượng khó giải thích. Những tình huống này phải giữ các lớp diễn giải song song, không biến thành chuỗi đánh quái siêu nhiên.

Mỗi họ sự kiện cần có cả phiên bản nhỏ, thường lệ và phiên bản quan trọng, chứ không chỉ vài major quest được đầu tư rồi để phần còn lại ở dạng sơ sài.

---

<a id="m22-t053-s126"></a>

## PHẦN XXVI — METRICS & PLAYTEST

<a id="m22-t053-s127"></a>

### 84. Không chỉ đo thời gian chơi

Một game có thể giữ player trong 50 giờ nhờ hệ thống tốn thời gian nhưng vẫn không tạo cảm giác cuộc đời có ý nghĩa.

Vì vậy cần đo những khía cạnh khác nhau.

| Nhóm đánh giá | Câu hỏi kiểm thử |
|---|---|
| Agency | Player có hiểu vì sao các lựa chọn tạo kết quả khác nhau không? |
| World Consistency | Có sự kiện nào xảy ra không có nguyên nhân hoặc trái World State không? |
| Activity Diversity | Người chơi ngoài combat có đủ việc có ý nghĩa không? |
| Narrative Clarity | Player có phân biệt được sự thật, tin đồn và giả thuyết không? |
| Pacing | Có quá nhiều khủng hoảng hoặc quá nhiều thời gian trống vô nghĩa không? |
| Difficulty Fairness | Khi thất bại, player có nhận ra nguyên nhân và khả năng cải thiện không? |
| Relationship Value | Player có nhớ và quan tâm đến một số NPC cụ thể không? |
| Long-Term Consequence | Việc quay lại một nơi sau nhiều năm có tạo khác biệt thật không? |
| Succession Quality | Character mới có identity riêng và gameplay khả thi không? |
| Historical Integration | Các nghề nghiệp khác nhau có thể tương tác với các hồi lịch sử không? |

<a id="m22-t053-s128"></a>

### 85. Kiểm thử theo các mốc thời gian

<a id="m22-t053-s129"></a>

#### Sau 15 phút

Tester phải hiểu nhân vật đang sống trong hoàn cảnh nào và đã đưa ra ít nhất một quyết định có hậu quả dễ nhận thấy.

<a id="m22-t053-s130"></a>

#### Sau một giờ

Tester phải hiểu có nhiều cách hành động, biết sử dụng các giao diện cơ bản và nhận ra thế giới có những biến động rộng hơn.

<a id="m22-t053-s131"></a>

#### Sau 10 giờ

Tester phải xác định được hướng phát triển nhân vật và ghi nhớ một số quan hệ, địa điểm hoặc sự kiện có ý nghĩa.

<a id="m22-t053-s132"></a>

#### Sau một vòng đời được mô phỏng

Tester phải nhìn thấy thay đổi về tuổi tác, nghề nghiệp, gia đình, cộng đồng và thông tin.

<a id="m22-t053-s133"></a>

#### Sau succession

Tester phải cảm nhận được sự chuyển đổi góc nhìn mà không thấy toàn bộ progression trước đó bị xóa bỏ vô nghĩa.

Các mốc này dùng để đánh giá trải nghiệm, không phải hard requirement ép người chơi theo tốc độ cố định.

---

<a id="m22-t053-s134"></a>

## PHẦN XXVII — PRODUCTION STRATEGY

<a id="m22-t053-s135"></a>

### 86. Không triển khai 12 vùng và toàn bộ simulation ngay từ đầu

GDD hiện có tham vọng rất lớn:

- hàng nghìn NPC;
- 12 đại vùng;
- nhiều loại nghề và Class;
- World Simulation;
- Narrative Engine;
- Combat;
- Economy;
- Faction;
- Aging;
- Succession;
- Myth Formation.

Triển khai tất cả đồng thời sẽ tạo rủi ro kỹ thuật và chi phí nội dung rất cao.

Vì vậy cần xây dựng một **Vertical Slice có đủ hệ thống kết nối**, thay vì một bản demo có nhiều hệ nhưng không tương tác thật.

<a id="m22-t053-s136"></a>

### 87. Vertical Slice đề xuất

Phạm vi đầu tiên nên tập trung vào:

**Một vùng chính:** Tam Giang.

**Một cụm settlement có liên hệ kinh tế và gia đình:** Bến Vạc và những địa điểm lân cận được chọn từ World Design.

**Một tuyến đường:** Có cả giao thông đường sông và đường bộ.

**Một nhóm nghề:** Vận tải, đánh cá, chữa bệnh và một nghề chế tác liên quan.

**Một nhóm xung đột:** Vấn đề vận tải, thiếu nguồn lực hoặc tranh chấp cộng đồng.

**Một dungeon:** Không gian ven sông hoặc một địa điểm ngập nước có nhiều hướng giải.

**Một combat scenario:** Có thể giải bằng chiến đấu, chuẩn bị hoặc rút lui khi hợp lý.

**Một Personal Story Thread:** Có liên hệ với family/companion.

**Một World Event:** Ví dụ sự thay đổi mực nước tác động đến route và economy.

Vertical Slice cần chứng minh:

Player Action → World State Change → NPC Reaction → Later Consequence.

Đây mới là cột sống gameplay.

<a id="m22-t053-s137"></a>

### 88. Simulated 25-Year Test

Song song với playable vertical slice, team phải có một **Headless Simulation Test**.

Chạy một World State qua 25 năm game-time mà không cần render toàn bộ gameplay.

Kiểm tra:

- NPC births/deaths;
- household transitions;
- economy;
- route changes;
- faction evolution;
- profession and apprenticeship;
- succession;
- Event Scheduler;
- Historical Anchors;
- Chronicle consistency.

Nếu simulation sau 25 năm tạo ra một thế giới vô lý, không nên tiếp tục mở rộng số lượng authored quests.

<a id="m22-t053-s138"></a>

### 89. Three-Origin Test

Sau khi có baseline, cần kiểm thử ít nhất ba xuất thân rất khác nhau:

**Character A:** Người trẻ, kỹ năng thấp, ở Tam Giang.

**Character B:** Người trưởng thành có nghề, ở Đồng Đỏ.

**Character C:** Người lớn tuổi có quan hệ xã hội mạnh, ở Tản Viên.

Cả ba phải:

- có một onboarding hợp lý;
- có ít nhất một Activity Loop sâu;
- có con đường tới Historical Campaign;
- có cơ hội phát triển identity;
- không buộc phải thành chiến binh.

Đây là cách kiểm tra chất lượng Character Generation thực tế.

---

<a id="m22-t053-s139"></a>

## PHẦN XXVIII — RỦI RO KIẾN TRÚC

<a id="m22-t053-s140"></a>

### 90. Rủi ro: World Director trở thành công cụ gian lận

Nếu director có quyền sinh tài nguyên, thay NPC và điều chỉnh enemy theo ý muốn, mọi hệ thống còn lại sẽ mất ý nghĩa.

**Biện pháp:** Giới hạn quyền truy cập, chia rõ simulation và presentation, log quyết định, xác thực precondition.

<a id="m22-t053-s141"></a>

### 91. Rủi ro: Có quá nhiều hoạt động nhưng thiếu chiều sâu

Game có thể có 20 Profession nhưng mỗi nghề chỉ gồm một nút “làm việc” và thanh tiến độ.

**Biện pháp:** Mỗi nghề cần Activity Loop với ít nhất một số quyết định thực tế về tri thức, nguồn lực, quan hệ, rủi ro và thành tựu.

Không cần mọi nghề có minigame hoàn toàn khác, nhưng không được đổi tên cùng một công thức vô nghĩa.

<a id="m22-t053-s142"></a>

### 92. Rủi ro: Main Campaign quá xa player

Nếu character sống ở vùng xa mà các sự kiện lớn chỉ xảy ra tại Phong Châu/Cổ Loa, player có thể cảm thấy mình đang chơi một game phụ.

**Biện pháp:** Historical Anchors phải tạo các hiệu ứng kinh tế, xã hội, thông tin và nghề nghiệp lan theo mạng vùng.

Player không cần có mặt ở trung tâm quyền lực để lịch sử ảnh hưởng cuộc đời mình.

<a id="m22-t053-s143"></a>

### 93. Rủi ro: Player bị buộc phải làm quá nhiều việc

Nhiều lời hứa, deadline và Story Thread có thể khiến người chơi thấy tội lỗi khi chọn sống bình thường.

**Biện pháp:** Phân biệt nghĩa vụ player tự nhận với vấn đề world tự phát sinh. Có nội dung tích cực và giai đoạn yên ổn. Không dùng notification để ép mọi biến cố thành trách nhiệm của player.

<a id="m22-t053-s144"></a>

### 94. Rủi ro: Campaign trở nên quá dễ khi nhân vật giàu và giỏi

Một Master nghề hoặc nhân vật có mạng quan hệ lớn có thể vượt qua nhiều bài toán nhỏ dễ dàng.

**Biện pháp:** Thay đổi quy mô trách nhiệm và cơ hội. Không tăng các chỉ số đối thủ giả tạo.

Một thợ đúc Master không cần phải gặp “quặng cấp 50”.

Họ có thể đối diện câu hỏi liệu nên truyền bí quyết cho một faction, tổ chức sản xuất cho nhiều người hay giữ truyền thống của gia đình.

<a id="m22-t053-s145"></a>

### 95. Rủi ro: Simulation rất sâu nhưng player không nhìn thấy

Nếu một thay đổi lớn trong World State không có biểu hiện trực quan, hội thoại hoặc gameplay, người chơi sẽ không cảm nhận được.

**Biện pháp:** Thiết kế Presentation Projection tương ứng:

- môi trường thay đổi;
- NPC nhắc tới hậu quả;
- hàng hóa và dịch vụ thay đổi;
- tuyến đường thay đổi;
- vật phẩm mang lịch sử;
- Chronicle ghi lại những gì đã biết.

Thế giới phải vừa có sự thật vừa có khả năng cho người chơi cảm nhận sự thật ấy.

---

<a id="m22-t053-s146"></a>

## PHẦN XXIX — QUALITY GATES

<a id="m22-t053-s147"></a>

### 96. Core Game Loop Quality Gate

Một Activity chỉ đạt chất lượng nếu xác định được:

1. Vì sao character có thể hoặc muốn thực hiện?
2. Player quyết định điều gì?
3. Những năng lực nào có ý nghĩa?
4. Tài nguyên/thời gian có giá trị ra sao?
5. Có cách điều chỉnh khi gặp trở ngại không?
6. Kết quả làm thay đổi điều gì?
7. Khi thực hiện lại nhiều lần, hoạt động còn chiều sâu không?
8. Khi đã trở thành routine, hoạt động có thể abstract không?

<a id="m22-t053-s148"></a>

### 97. Campaign Quality Gate

Mỗi Historical Act phải có:

1. Chủ đề rõ.
2. Historical Anchor và giới hạn can thiệp rõ.
3. Tác động tới nhiều vùng.
4. Gameplay relevance cho combat và non-combat.
5. Mối liên hệ với ít nhất một số hệ nghề nghiệp.
6. Biến đổi World State quan sát được.
7. Hậu quả tiếp tục sang hồi sau.
8. Cách diễn ra hợp lệ khi player không tham gia trực tiếp.

<a id="m22-t053-s149"></a>

### 98. World Director Quality Gate

Mỗi quyết định expose nội dung phải trả lời được:

1. Sự kiện có tồn tại thật không?
2. Precondition nào đã được thỏa?
3. NPC nào truyền thông tin?
4. Vì sao họ biết?
5. Vì sao player có thể nhận?
6. Vì sao tình huống được ưu tiên lúc này?
7. Có nội dung gần đây bị lặp không?
8. Director có đang tạo hậu quả mới không được phép?
9. Quyết định có thể debug không?
10. Nếu Director không expose, thế giới sẽ diễn biến thế nào?

<a id="m22-t053-s150"></a>

### 99. Difficulty Quality Gate

Một thử thách khó chỉ đạt tiêu chuẩn khi:

- quy tắc nhất quán;
- rủi ro có dấu hiệu hoặc có thể tìm hiểu;
- kiến thức và chuẩn bị tạo lợi thế;
- các Profession/Class liên quan có giá trị;
- thất bại có nguyên nhân;
- các phương án rút lui hoặc chấp nhận hậu quả hợp lý được hỗ trợ;
- game không dùng hidden cheating để tạo kết quả mong muốn.

<a id="m22-t053-s151"></a>

### 100. Game Mode Quality Gate

Mỗi chế độ phải:

- có mục tiêu người chơi rõ;
- khác biệt thực sự có ý nghĩa;
- không phá world logic;
- không làm một Class/Profession mất đường phát triển;
- không dùng độ khó cao làm cớ cho thông tin giả;
- có thể kiểm thử riêng;
- giữ các Historical Anchors và những quy luật cốt lõi.

---

<a id="m22-t053-s152"></a>

## PHẦN XXX — CÁC QUYẾT ĐỊNH ĐỀ XUẤT KHÓA

<a id="m22-t053-s153"></a>

### 101. Gameplay & Campaign

1. Game sử dụng bốn cấp vòng lặp: Micro, Activity, Life và Historical.
2. Player không cần combat để có progression có ý nghĩa.
3. Ordinary Life Gameplay là một phần chính thức của trải nghiệm.
4. Personal Intent tồn tại song song Quest System.
5. Campaign có sáu hồi lịch sử đã thống nhất.
6. Historical Track và Personal Life Track vận hành song song.
7. Player không cần tham gia trực tiếp mọi Historical Anchor.
8. Các nghề nghiệp có cửa tương tác với các hồi lịch sử.
9. Không khóa Campaign Progression theo số quest hoàn thành.
10. Thời gian thế giới tiếp tục vận hành dù player không làm main quest.

<a id="m22-t053-s154"></a>

### 102. Onboarding & Pacing

11. Onboarding thích nghi theo xuất thân, tuổi và năng lực.
12. Character Generation không bị sửa để phục vụ tutorial.
13. Mỗi starting character phải qua Viability Gate.
14. 15 phút đầu cần một quyết định có hậu quả dễ nhận biết.
15. Một giờ đầu phải giới thiệu gameplay bằng hành động, không lore dump.
16. 10 giờ đầu hướng tới hình thành identity và mối quan hệ đáng nhớ.
17. Không bắt buộc Class Emergence theo thời gian chơi cố định.
18. Game có Natural Stopping Points.
19. Pacing dùng nhiều chiều áp lực, không chỉ combat intensity.
20. Ordinary Life, Aftermath và Consolidation có giá trị ngang Crisis.

<a id="m22-t053-s155"></a>

### 103. World Director

21. World Director không sở hữu World Truth.
22. World Simulation quyết định trạng thái thế giới.
23. Event Engine xử lý nguyên nhân và sự kiện.
24. Historical Anchor Controller quản lý cấu trúc lịch sử.
25. Director điều phối exposure, ưu tiên và presentation.
26. Director chỉ chọn giữa những cơ hội hợp lệ.
27. Mandatory World Events không bị dừng vì pacing.
28. Knowledge Filter bắt buộc trước khi hiển thị thông tin.
29. Director không tự sinh tài nguyên, hồi sinh NPC hoặc thay kết quả xúc xắc.
30. Director Decision cần có log và khả năng kiểm thử.

<a id="m22-t053-s156"></a>

### 104. Difficulty & Game Modes

31. Không dùng level scaling tuyệt đối.
32. Challenge đến từ World State, Capability và Preparation.
33. Fairness được đánh giá riêng với Difficulty.
34. Story, Standard và Tactical là ba preset thử thách.
35. Legacy/Ironman là lựa chọn Persistence.
36. Assistance có thể tùy chỉnh độc lập.
37. Adaptive Difficulty chỉ điều chỉnh presentation và cơ hội tùy chọn trong giới hạn nhân quả.
38. Không thay đổi hidden truth để cân bằng.
39. Mastery giúp routine trở nên dễ và nhanh hơn một cách hợp lý.
40. Succession không tự reset World Difficulty.

<a id="m22-t053-s157"></a>

### 105. Production & Validation

41. Phát triển Vertical Slice kết nối nhiều hệ thống trước khi mở rộng toàn thế giới.
42. Có Headless Simulation Test cho khoảng 25 năm.
43. Có Three-Origin Test cho các kiểu nhân vật khác nhau.
44. Có No-Combat-Oriented Run Test.
45. Có kiểm thử Historical Anchor khi player không can thiệp.
46. Có kiểm thử sự nhất quán của Director.
47. Nội dung interactive phải có quyết định hoặc thay đổi có ý nghĩa.
48. Không dùng biến cố ngẫu nhiên để che một Activity Loop yếu.
49. Không lấy thời lượng chơi làm thước đo chất lượng duy nhất.
50. Trải nghiệm cần chứng minh được cả Agency, Belonging, Mastery và Consequence.

---

<a id="m22-t053-s158"></a>

## PHẦN XXXI — DEFERRED TO PRODUCTION

<a id="m22-t053-s159"></a>

### 106. Những vấn đề chưa nên khóa số liệu

Production sẽ cần xác định:

- tổng thời lượng chơi dự kiến;
- thời lượng trung bình mỗi hồi;
- số lượng Starting Origin Template;
- phân bố nghề/tuổi trong Character Generation;
- Activity Density;
- số Story Thread active khuyến nghị;
- mức thời gian cho Recovery Window;
- Pacing Vector Weights;
- Event Eligibility Weights;
- tần suất exposure;
- mức độ hỗ trợ theo từng preset;
- combat/injury balance;
- time-skip simulation performance;
- save system cho Legacy/Ironman;
- UI cho Current Concerns;
- quy mô vertical slice;
- bộ chỉ số playtest;
- công cụ replay và debug World Director.

Các tham số trên cần được khóa thông qua prototype và kiểm thử người chơi, không nên tự đưa ra những con số cứng khi chưa có bằng chứng thực tế.

---

<a id="m22-t053-s160"></a>

## PHẦN XXXII — CORE DESIGN STATEMENT

<a id="m22-t053-s161"></a>

### 107. Vòng chơi không được biến thành một dây chuyền làm nhiệm vụ

Một RPG thông thường có thể khiến người chơi nghĩ:

> “Tôi phải làm quest nào tiếp theo?”

Game này nên khiến họ nghĩ:

> “Tôi muốn dành ngày hôm nay để làm gì?”

Họ có thể muốn tìm một người mất tích.

Có thể muốn học nghề.

Có thể muốn trở về quê.

Có thể muốn đi đến một vùng xa mà mình chỉ mới nghe kể.

Có thể muốn giúp một cộng đồng.

Cũng có thể chỉ muốn sống cùng gia đình thêm một mùa trước khi quyết định điều gì quan trọng.

<a id="m22-t053-s162"></a>

### 108. World Director không phải người viết số phận

World Director không được quyết định:

> “Người chơi cần một bi kịch, vậy hãy giết companion.”

Hoặc:

> “Người chơi đang buồn, vậy hãy tạo một người bạn mới.”

Nó chỉ được nhìn vào thế giới có thật và tìm ra:

> “Trong những gì đang diễn ra, điều nào đáng để người chơi có cơ hội nhận biết và can thiệp lúc này?”

Còn kết quả phải đến từ:

- thế giới;
- con người;
- thời gian;
- hành động;
- các quy luật đã thiết kế.

<a id="m22-t053-s163"></a>

### 109. Chiến dịch không phải hành trình từ yếu tới mạnh

Một người có thể bắt đầu yếu và trở thành bậc thầy.

Một người khác có thể bắt đầu là Master và dần mất sức khỏe.

Một người trở nên giàu có.

Một người mất tất cả rồi xây dựng lại.

Một người có thể qua đời sớm.

Người kế tục của họ có thể đi một con đường hoàn toàn khác.

Vì vậy Campaign Progression không nên được hiểu đơn giản là:

> Nhân vật càng về cuối càng mạnh.

Nó phải là:

> **Nhân vật càng sống lâu và trải nghiệm nhiều, cuộc đời càng mang nhiều dấu ấn, trách nhiệm, hiểu biết và hậu quả.**

<a id="m22-t053-s164"></a>

### 110. Tuyên ngôn kết thúc

Game không nên hỏi người chơi:

> “Bạn có đủ mạnh để thay đổi lịch sử không?”

Nó nên hỏi:

> **“Trong một thời đại mà bạn không thể kiểm soát toàn bộ, bạn sẽ lựa chọn sống như thế nào?”**

Có người cầm giáo.

Có người giữ thuyền.

Có người chữa bệnh.

Có người đúc nỏ.

Có người giữ lời thề.

Có người dạy trẻ nhỏ.

Có người bảo vệ một ngôi làng mà sử sách sau này không bao giờ nhắc tới.

Tất cả đều có thể là nhân vật chính của một cuộc đời.

**Thế giới không tồn tại để phục vụ riêng player. Nhưng những lựa chọn của player phải thực sự có chỗ đứng trong thế giới ấy.**

Đó là mục tiêu cao nhất của Game Loop, Campaign Structure và World Director.

---

<a id="m22-t053-s165"></a>

### Đánh giá sau Module 22

Với module này, chúng ta đã xác định không chỉ *game có những hệ thống gì*, mà cả **cách những hệ thống đó trở thành một trải nghiệm chơi thống nhất**.

Có ba quyết định tôi đặc biệt khuyến nghị giữ nguyên khi phát triển:

1. **Không sử dụng AI Director kiểu điều chỉnh số phận và sức mạnh đối thủ theo người chơi.** Dùng World Director giới hạn ở điều phối trải nghiệm trên dữ liệu thế giới hợp lệ.
2. **Không đặt combat làm vòng lặp tiến triển bắt buộc.** Profession, Social, Exploration, Crafting và Legacy phải có chiều sâu độc lập.
3. **Không triển khai toàn bộ 12 vùng trước khi chứng minh được vertical slice.** Cần xác thực vòng nhân quả từ hành động cá nhân tới biến đổi World State trước khi mở rộng nội dung.

<a id="m22-t053-s166"></a>

#### Bước tiếp theo đề xuất

**Module 23 — UI/UX + Information Architecture + Journal/Codex/Chronicle + Player Feedback System.**

Đây là bài toán đặc biệt khó của GDD hiện tại: người chơi phải quản lý một thế giới có rất nhiều dữ liệu, trong khi character **không được biết toàn bộ sự thật**, NPC có thể nói sai, bản đồ có thông tin cũ, quan hệ không phải thanh điểm và các sự kiện có thể tồn tại hàng chục năm.

Module kế tiếp cần thiết kế đầy đủ giao diện cho **Character, Party, Map, Dialogue, Quest, Knowledge, Profession, Crafting, Faction, Calendar, Time Skip, Succession và Legacy**, đồng thời xác định rõ *thông tin nào được hiển thị, thông tin nào phải ẩn, và hệ thống giải thích hành động/hậu quả thế nào mà không phá immersion*.

<!-- SOURCE T053 END -->

