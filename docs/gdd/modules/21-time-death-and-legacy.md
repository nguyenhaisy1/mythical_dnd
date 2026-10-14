# Module 21 — Thời gian, tuổi tác, chết, kế tục và di sản

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T049](../../source/conversation.md#t049)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

WorldTick thống nhất. Time skip xử lý jobs và breakpoints; succession đổi quyền điều khiển, không clone tri thức/tài sản. Player archive khác Active Character Knowledge. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Clock, aging/health, inheritance rights, successors, obligations và world scheduler. |
| Đầu ra / bên sở hữu | Life transitions, valid transfers, legacy/chronicle; Characters/Assets/Knowledge/Legacy owners. |
| Đọc cùng | 04, 13, 18, 22, 25–29.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Skip qua death event cập nhật membership đúng; người kế tục không tự biết secret của người chết. |
| Việc chưa được chứng minh | Mortality rates và simulation aggregation error cần prototype; tick/calendar đã có clock-v1. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T049 BEGIN -->

Module tiếp theo sẽ giải quyết một vấn đề cốt lõi của toàn bộ GDD: **thế giới không dừng lại để chờ người chơi, nhưng người chơi cũng không bị biến thành khán giả bất lực trước dòng thời gian**.

Tôi đề xuất chiến dịch chính kéo dài khoảng **20–25 năm trong game**, với những giai đoạn lịch sử được định hình trước nhưng số phận của từng cá nhân, gia đình, cộng đồng và nhiều kết quả địa phương vẫn có thể thay đổi sâu sắc. Đặc biệt, **nhân vật chính có thể qua đời trước khi campaign kết thúc**; khi đó câu chuyện có thể tiếp tục qua một người kế tục có quan hệ thực sự với cuộc đời họ.

<a id="m21-t049-s001"></a>

## GDD MODULE 21 — TIME, CALENDAR, AGING, DEATH, SUCCESSION & LEGACY SYSTEM

**Trạng thái:** Thiết kế hệ thống lõi — đề xuất khóa vào GDD  
**Phạm vi:** Thời gian thế giới, lịch mùa vụ, chu kỳ đời người, tử vong, chuyển giao nhân vật, di sản và hậu truyện  
**Phụ thuộc:** Character Generation, Psychology, Household, Social Graph, Narrative Engine, Quest, Profession, Class, Party, Combat, Injury, Economy, Faction, Mạch Lạc và Myth Formation

---

<a id="m21-t049-s002"></a>

## PHẦN I — TRIẾT LÝ THIẾT KẾ

<a id="m21-t049-s003"></a>

### 1. Tầm nhìn

Phần lớn RPG vận hành theo một giả định ngầm:

> Thế giới chỉ thay đổi khi người chơi thực hiện một nhiệm vụ.

Một NPC đứng chờ hàng tháng. Một đứa trẻ không lớn lên. Một cuộc chiến không bắt đầu cho tới khi người chơi nói chuyện với đúng nhân vật. Một thợ rèn luôn đứng ở cùng một cửa hàng.

Game này không thể sử dụng mô hình đó.

Thời gian phải là một thành phần vận hành thực sự của thế giới.

Khi người chơi đi xa, quê nhà vẫn tồn tại. Khi họ bị thương và phải nghỉ ba tháng, các gia đình vẫn làm ăn, trẻ em vẫn trưởng thành, người già có thể qua đời và các phe phái vẫn triển khai kế hoạch.

Nhưng cũng cần tránh cực đoan ngược lại: tạo ra một thế giới liên tục trừng phạt người chơi vì họ không thể có mặt ở mọi nơi.

Vì vậy, hệ thống cần thỏa mãn đồng thời bốn nguyên tắc:

1. **Time is real:** Thời gian trôi thật trong thế giới và có hậu quả.
2. **Life is finite:** Mọi nhân vật đều có một đời sống hữu hạn.
3. **Agency is meaningful:** Lựa chọn của player có thể thay đổi nhiều số phận, nhưng không kiểm soát toàn bộ lịch sử.
4. **Legacy persists:** Cái chết không nhất thiết xóa bỏ thành tựu và hậu quả của một đời người.

<a id="m21-t049-s004"></a>

### 2. Sáu câu hỏi mà hệ thống phải trả lời

| Câu hỏi | Yêu cầu thiết kế |
|---|---|
| Khi player ở nơi khác, NPC làm gì? | NPC tiếp tục sống theo simulation phù hợp |
| Khi nhiều năm trôi qua, thế giới thay đổi thế nào? | Gia đình, kinh tế, nghề nghiệp, phe phái và địa lý biến đổi |
| Khi nhân vật già đi, khả năng của họ thay đổi ra sao? | Thể chất, kinh nghiệm, quan hệ và trách nhiệm cùng phát triển |
| Khi nhân vật chết, điều gì thực sự mất? | Sự sống, năng lực hành động, một phần tri thức và mạng quan hệ |
| Ai có thể tiếp tục câu chuyện? | Người kế tục có liên hệ, động cơ và khả năng tiếp nhận hợp lý |
| Cuộc đời đã qua để lại gì? | Người thân, vật phẩm, tri thức, công trình, nợ, danh tiếng và truyền thuyết |

<a id="m21-t049-s005"></a>

### 3. Điều quan trọng nhất: không có cơ chế chuyển linh hồn

Succession không phải reincarnation.

Nếu nhân vật Mân chết và player chuyển sang điều khiển Miên, thì Miên **không trở thành Mân**.

Miên vẫn có:

- trí nhớ của chính mình;
- nghề nghiệp của chính mình;
- các giá trị và ranh giới đạo đức riêng;
- những người mà cô yêu thương hoặc căm ghét;
- cách nhìn về Mân, có thể khác cách Mân nhìn chính mình.

Player tiếp tục trải nghiệm thế giới, nhưng phải tiếp tục bằng **một con người khác**.

Đây là nguyên tắc không được phá vỡ.

---

<a id="m21-t049-s006"></a>

## PHẦN II — KIẾN TRÚC THỜI GIAN THẾ GIỚI

<a id="m21-t049-s007"></a>

### 4. Một dòng thời gian thống nhất

Toàn bộ game sử dụng một **World Timeline** duy nhất làm nguồn sự thật.

Các hệ thống không tự duy trì những chiếc đồng hồ độc lập.

Ví dụ:

- Travel cập nhật số ngày đã đi.
- Injury đọc thời gian để tính hồi phục.
- Economy xử lý sản xuất theo mùa.
- NPC Aging đọc thời gian đã trôi.
- Quest kiểm tra deadline.
- Faction kiểm tra thời điểm huy động lực lượng.
- Chronicle ghi lại sự kiện.

Tất cả cùng tham chiếu một thời gian thế giới.

<a id="m21-t049-s008"></a>

#### Mô hình dữ liệu đề xuất

```text
WorldClock
    CampaignYear
    YearPhase
    DayOfYear
    TimeOfDay
    WorldTick
    HistoricalEra
    ActiveHistoricalAnchors
```

`WorldTick` là giá trị nội bộ có thứ tự tuyệt đối, giúp engine giải quyết sự kiện chính xác và nhất quán.

`CampaignYear` là cách game xác định năm tương đối trong chiến dịch.

`HistoricalEra` xác định bối cảnh lịch sử đang diễn ra.

Các thành phần này không phụ thuộc vào ngày tháng Gregorian hiện đại.

<a id="m21-t049-s009"></a>

### 5. Năm cấp độ thời gian

Game sử dụng năm cấp xử lý khác nhau, thay vì mô phỏng mọi giây.

| Cấp thời gian | Sử dụng | Mức mô phỏng |
|---|---|---|
| Lượt/vòng | Combat, hành động khẩn cấp | Chi tiết |
| Phút/giờ | Dialogue, Dungeon, Crafting ngắn, cứu nạn | Theo hoạt động |
| Ngày | Travel, Rest, Wound, NPC Schedule | Đơn vị vận hành cơ bản |
| Mùa/giai đoạn năm | Nông nghiệp, Ecology, Trade, Faction | Tổng hợp |
| Năm | Aging, Family, Career, Historical Change | Chuyển biến dài hạn |

Ví dụ, game không cần cập nhật sản lượng lúa sau mỗi lượt combat. Nhưng một cuộc chiến kéo dài qua mùa thu hoạch phải tác động thật đến sản lượng.

<a id="m21-t049-s010"></a>

### 6. Thời gian có thể trôi theo nhiều cách

<a id="m21-t049-s011"></a>

#### Active Time

Player trực tiếp điều khiển nhân vật.

Ví dụ: khám phá khu rừng trong một buổi chiều.

<a id="m21-t049-s012"></a>

#### Compressed Time

Một hoạt động thường lệ được tóm tắt.

Ví dụ: đi tuyến đường quen thuộc trong bốn ngày.

<a id="m21-t049-s013"></a>

#### Downtime

Player chủ động dành thời gian cho nghề nghiệp, học tập, hồi phục, gia đình hoặc công trình.

<a id="m21-t049-s014"></a>

#### Historical Time Transition

Campaign chuyển sang một giai đoạn lịch sử mới, có thể sau vài năm.

Cả bốn loại đều làm WorldClock tiến lên. Điểm khác biệt nằm ở cách hệ thống mô phỏng và trình bày những gì xảy ra.

---

<a id="m21-t049-s015"></a>

## PHẦN III — CALENDAR & SEASON SYSTEM

<a id="m21-t049-s016"></a>

### 7. Lịch trong game phải phục vụ đời sống, không sao chép máy móc lịch hiện đại

Bối cảnh game lấy cảm hứng từ thời Văn Lang–Âu Lạc. Không nên tự khẳng định cư dân tất cả các vùng sử dụng một hệ lịch hoàn chỉnh, đồng nhất và giống lịch Việt hiện đại.

Đề xuất tách hai lớp:

**Lịch nội bộ:** Engine tính chính xác thời gian.

**Lịch văn hóa:** Các cộng đồng gọi và cảm nhận thời gian thông qua mùa vụ, nước, gió, lễ hội và chu kỳ sinh hoạt.

Ví dụ một người Tam Giang có thể nói:

> “Khi nước xuống mùa tới, ta sẽ trở về.”

Trong khi một người làm nông nói:

> “Ta không thể bỏ làng trước khi gặt xong.”

Hai người đang nói về cùng một dòng thời gian nhưng sử dụng những mốc đời sống khác nhau.

<a id="m21-t049-s017"></a>

### 8. Mô hình mùa sinh thái

Không nên dùng bốn mùa xuân–hạ–thu–đông với hiệu ứng giống RPG châu Âu.

Game cần hệ thống **Seasonal Phase**, có thể biểu diễn bằng những giai đoạn sinh thái như:

| Giai đoạn | Đặc trưng | Ảnh hưởng chính |
|---|---|---|
| Giai đoạn chuyển mùa | Điều kiện đang thay đổi | Lập kế hoạch, chuẩn bị |
| Giai đoạn nước/mưa tăng | Mưa, mực nước và độ ẩm tăng | Đường sông, lũ, nông nghiệp |
| Giai đoạn nước cao | Một số tuyến ngập, hoạt động thủy vực thay đổi | Travel, Fishing, Disease Risk |
| Giai đoạn nước rút | Đường cạn xuất hiện, địa hình thay đổi | Exploration, Construction |
| Giai đoạn tương đối khô | Một số tuyến đất thuận lợi hơn | Khai thác, vận tải, sản xuất |

Đây là các trạng thái thiết kế, **không nhất thiết là năm mùa có thời lượng bằng nhau**.

Mỗi vùng có Seasonal Profile riêng.

Tam Giang chú trọng mực nước. Hải Môn chú trọng gió, triều và biển. Bãi Lạc chú trọng chu kỳ sản xuất nông nghiệp. Tản Viên và Thượng Âu chú trọng đường núi và thời tiết địa phương.

<a id="m21-t049-s018"></a>

### 9. Calendar Event

Những sự kiện gắn với lịch có thể gồm:

- thời điểm gieo trồng, thu hoạch;
- lễ nghi cộng đồng;
- mùa trao đổi hàng;
- hoạt động nghề;
- sự kiện gia đình;
- thời điểm một tuyến đi lại được;
- một số hoạt động chính trị hoặc quân sự.

Chúng phải được lưu như những sự kiện có điều kiện, không phải chương trình cố định xuất hiện bất kể World State.

Ví dụ:

Một lễ hội thường niên có thể bị hủy vì dịch bệnh, thay đổi hình thức vì chiến tranh, hoặc chuyển sang settlement khác khi cộng đồng di cư.

<a id="m21-t049-s019"></a>

### 10. Time-Based Opportunity

Thời gian không chỉ đóng cửa cơ hội; nó còn mở những cơ hội mới.

Ví dụ:

Một người học nghề chỉ có mặt ở làng vào một thời kỳ.

Một mộ cổ có thể lộ ra khi nước rút.

Một gia đình có thể cần lao động trong mùa thu hoạch.

Một trưởng dòng họ có thể tham gia hội họp vào một giai đoạn nhất định.

Người chơi vì thế có lý do tìm hiểu lịch địa phương, nhưng không phải nhớ hàng trăm lịch hẹn như phần mềm quản lý công việc.

---

<a id="m21-t049-s020"></a>

## PHẦN IV — HISTORICAL CAMPAIGN TIMELINE

<a id="m21-t049-s021"></a>

### 11. Thời lượng chiến dịch

**Đề xuất khóa: Chiến dịch lịch sử chính kéo dài khoảng 20–25 năm trong game.**

Con số này là cấu trúc thiết kế của game-canon, không phải khẳng định các sự kiện truyền thuyết hoặc lịch sử thật diễn ra cách nhau đúng từng ấy năm.

Các sự kiện chính được dùng làm Historical Anchors:

- Văn Lang giai đoạn cuối;
- các quan hệ chính trị và xã hội thay đổi;
- sự hình thành Âu Lạc;
- quá trình hình thành và phát triển Cổ Loa;
- công nghệ, quyền lực và câu chuyện Nỏ thần;
- khủng hoảng cuối thời Âu Lạc, gắn với truyền thuyết Mỵ Châu–Trọng Thủy.

Những motif truyền thuyết xa hơn như Lạc Long Quân–Âu Cơ, Lang Liêu hoặc Thánh Gióng vẫn thuộc ký ức, tín ngưỡng và các lớp truyện đã được cộng đồng lưu truyền, không cần xuất hiện đồng thời dưới dạng nhân vật đang sống.

<a id="m21-t049-s022"></a>

### 12. Cấu trúc thời gian theo sáu hồi

Bảng sau là **khung minh họa cho nhịp thời gian**, chưa phải niên đại lịch sử chính thức.

| Hồi | Khoảng năm chiến dịch | Biến chuyển trung tâm |
|---|---|---|
| I — Trống không vang | 0–3 | Các bất ổn địa phương và dấu hiệu đầu tiên của khủng hoảng |
| II — Những lời thề đã quên | 3–7 | Mạng quan hệ cũ suy yếu, nhiều vùng thay đổi |
| III — Hai con đường | 7–12 | Liên minh, cải cách, xung đột chính trị và chuyển tiếp quyền lực |
| IV — Thành xoáy ốc | 12–17 | Cổ Loa biến đổi địa lý, kinh tế và con người |
| V — Nỏ | 17–21 | Công nghệ, tổ chức quân sự và quyền lực mới |
| VI — Lông ngỗng | 21–25 | Khủng hoảng, mất mát, kết thúc thời đại và hậu quả |

Các năm là giá trị tham chiếu phục vụ thiết kế pacing. Production phải kiểm thử và có thể điều chỉnh.

<a id="m21-t049-s023"></a>

### 13. Historical Anchor không đồng nghĩa cutscene bắt buộc

Mỗi Anchor có ba thành phần:

**Sự kiện lõi:** Điều phải xảy ra để chiến dịch giữ được trục lịch sử–truyền thuyết đã chọn.

**Điều kiện hình thành:** Các lực lượng, nguồn lực và biến động dẫn đến sự kiện.

**Biên độ can thiệp:** Những gì player có thể thay đổi mà không phá cấu trúc cốt lõi.

Ví dụ: Cổ Loa hình thành.

Player có thể ảnh hưởng:

- gia đình nào bị di dời;
- tuyến cung ứng nào hình thành;
- những thợ nào nổi tiếng;
- phương án công trình nào được áp dụng;
- ai nắm quyền quản lý;
- mức độ xung đột giữa các cộng đồng;
- những nghi lễ và câu chuyện nào được tạo ra.

Nhưng player không thể đơn giản làm cho Cổ Loa biến mất khỏi mọi phiên bản chiến dịch chỉ bằng một lần thuyết phục.

Điều quan trọng là **các giới hạn phải đến từ sức nặng nhân quả của thế giới**, không phải một NPC bất tử hoặc một cánh cửa kịch bản không giải thích được.

---

<a id="m21-t049-s024"></a>

## PHẦN V — TIME SKIP & CHAPTER TRANSITION

<a id="m21-t049-s025"></a>

### 14. Vì sao cần nhảy thời gian?

Nếu người chơi phải trải nghiệm trực tiếp từng ngày trong 25 năm, game sẽ trở nên khổng lồ và có nguy cơ lặp lại.

Ngược lại, nếu cứ vài nhiệm vụ lại tự động nhảy năm năm, người chơi sẽ mất cảm giác kiểm soát cuộc đời nhân vật.

Vì vậy cần một hệ thống **Time Transition có kiểm soát**, kết hợp giữa chuyển hồi theo câu chuyện và Downtime tự chọn.

<a id="m21-t049-s026"></a>

### 15. Ba loại Time Skip

<a id="m21-t049-s027"></a>

#### Loại A — Routine Skip

Dành cho thời gian ngắn hoặc hoạt động đã quen.

Ví dụ:

- nghỉ dưỡng thương một tuần;
- chế tác đơn hàng thường lệ;
- đi lại trên tuyến an toàn.

Player vẫn nhận tóm tắt các biến cố quan trọng.

<a id="m21-t049-s028"></a>

#### Loại B — Life Interval

Khoảng vài tháng đến một vài năm, khi nhân vật sống cuộc đời tương đối bình thường giữa các khủng hoảng.

Trước khi chuyển, player xác định ưu tiên của character:

- tiếp tục nghề;
- học một người thầy;
- ở với gia đình;
- xây dựng quan hệ;
- thực hiện hợp đồng;
- chăm sóc người thân;
- nghỉ ngơi, hồi phục;
- tham gia một tổ chức.

Player **không lựa chọn kết quả chắc chắn**, mà chọn cách dành thời gian.

<a id="m21-t049-s029"></a>

#### Loại C — Historical Transition

Chuyển sang hồi lịch sử tiếp theo.

Khoảng thời gian chuyển có thể kéo dài nhiều năm.

World Simulation giải quyết tất cả các hệ liên quan và trình bày những biến đổi đáng kể.

<a id="m21-t049-s030"></a>

### 16. Quy tắc xử lý nhiệm vụ dang dở

Trước Time Transition, hệ thống kiểm tra:

- deadline khẩn cấp;
- nhiệm vụ đang ở giai đoạn không thể tạm dừng;
- companion đang gặp nguy hiểm;
- tù binh hoặc người cần cứu;
- lời hứa đến hạn;
- dự án đang cần quyết định;
- người chơi đang ở khu vực không an toàn.

Không được âm thầm cho qua ba năm rồi thông báo rằng mọi người đã chết khi player chưa hiểu mình đang chấp nhận điều gì.

Đối với các sự kiện có tác động lớn, Time Transition phải có **tóm tắt những hệ quả có thể dự đoán**, dựa trên hiểu biết hiện tại của nhân vật.

Ví dụ:

> “Nếu dành hai năm tiếp theo tại Lò Cốc, bạn có thể hoàn thiện việc học nghề. Tuy nhiên, gia đình Chạ đang chờ lời hồi đáp về vấn đề đất đai, và tình hình tại Bắc Lộ vẫn bất ổn.”

Đây không phải spoiler về tương lai, chỉ là những mối quan tâm mà character đã biết.

<a id="m21-t049-s031"></a>

### 17. Time Skip không phải chọn kết quả

Một lỗi thiết kế cần tránh:

> Chọn “Luyện rèn ba năm” → nhận ngay +30 Skill, Master Class và 5 vật phẩm.

Thực tế hệ thống phải xử lý:

- character có mentor không;
- có workshop không;
- có vật liệu không;
- sức khỏe ra sao;
- việc học có bị gián đoạn không;
- có Life Event nào phát sinh không;
- nghề đã đạt tới giới hạn routine practice chưa.

Kết quả có thể rất tốt, vừa phải hoặc khác dự định.

Player được lựa chọn **hướng sống**, không được mua trước thành tựu.

<a id="m21-t049-s032"></a>

### 18. Time Skip Report

Sau mỗi khoảng thời gian dài, game trình bày một bản tổng kết gọn nhưng có chiều sâu, chia theo mức độ liên quan.

Ví dụ:

**Ba năm đã trôi qua**

**Bản thân:** Mân trở nên thành thạo hơn trong việc dẫn thuyền. Vết thương chân từ trận trước thỉnh thoảng vẫn gây đau khi trời ẩm.

**Gia đình:** Cha Mân đã nghỉ việc chở khách; người em tiếp quản thuyền.

**Bạn đồng hành:** Miên mở một nơi chữa bệnh ở Bãi Lau. Hai người ít gặp nhau hơn trước.

**Thế giới:** Một số tuyến thương mại chuyển hướng về Loa Vực. Gia đình Chạ đang tranh cãi về quyền sử dụng đất.

**Chuyện chưa kết thúc:** Lời hứa tìm lại người mất tích ở Bắc Lộ vẫn chưa được thực hiện.

Report chỉ hiển thị những điều player có khả năng biết. Những biến cố bí mật vẫn được lưu trong World State nhưng không xuất hiện như thông tin toàn tri.

---

<a id="m21-t049-s033"></a>

## PHẦN VI — NPC LIFE CYCLE

<a id="m21-t049-s034"></a>

### 19. Vòng đời nhân vật

Mỗi character có quá trình:

**Sinh ra → thời thơ ấu → trưởng thành → phát triển nghề nghiệp → các bước ngoặt đời sống → tuổi cao → qua đời.**

Không phải nhân vật nào cũng sống đủ mọi giai đoạn.

Một số chết trẻ vì chiến tranh, bệnh tật hoặc tai nạn. Một số sống lâu và có vai trò lớn trong cộng đồng.

Game không cần mô phỏng tuổi thọ một cách giật gân, cũng không nên để mọi NPC sống chính xác tới cùng một tuổi.

<a id="m21-t049-s035"></a>

### 20. Age Profile

Mỗi character lưu:

```text
CharacterAgeProfile
    BirthTime
    CurrentAge
    BiologicalCondition
    DevelopmentStage
    HealthHistory
    Injuries
    Lifestyle
    ProfessionalExposure
    FamilyResponsibilities
    LifeExpectations
    AgingModifiers
```

Tuổi theo năm là một thông tin quan trọng, nhưng **tuổi không tự động quyết định toàn bộ khả năng**.

Một người 55 tuổi làm việc nặng suốt đời có thể có thể lực rất khác người cùng tuổi bị bệnh lâu năm.

<a id="m21-t049-s036"></a>

### 21. Các giai đoạn đời người

Không dùng ngưỡng cố định để tự động cấp buff/debuff.

Thay vào đó, engine sử dụng các giai đoạn linh hoạt:

| Giai đoạn | Trọng tâm phát triển | Áp lực thường gặp |
|---|---|---|
| Thơ ấu | Hình thành temperament, family attachment, exposure | Phụ thuộc gia đình, môi trường |
| Thiếu niên | Học kỹ năng, xây dựng identity, apprenticeship | Kỳ vọng gia đình, thiếu kinh nghiệm |
| Trưởng thành sớm | Nghề nghiệp, quan hệ, vị trí xã hội | Tự lập, cam kết, rủi ro |
| Trưởng thành ổn định | Chuyên môn, trách nhiệm, uy tín | Gia đình, nghề, authority |
| Trung niên | Tích lũy tri thức, truyền nghề, chuyển vai trò | Sức khỏe, người phụ thuộc, legacy |
| Cao tuổi | Truyền thừa, ký ức, vai trò cộng đồng | Suy giảm một số chức năng, chia tay, kế tục |

Giai đoạn là xu hướng, không phải khuôn ép.

Một người trẻ có thể lãnh đạo giỏi. Một người già có thể tiếp tục săn bắn. Một người không lập gia đình vẫn có cuộc đời trọn vẹn.

<a id="m21-t049-s037"></a>

### 22. NPC Aging phải tác động tới Social Graph

Khi một NPC lớn lên hoặc già đi, engine cập nhật không chỉ chỉ số mà cả vai trò.

Ví dụ:

Một người mẹ từ tuổi 35 đến 55 có thể:

- không còn trực tiếp làm việc đồng như trước;
- trở thành người quyết định kinh tế household;
- có con đã trưởng thành;
- có thêm thông gia;
- sở hữu uy tín trong lineage;
- trở thành người giữ một lời hứa cũ.

Sức ảnh hưởng xã hội có thể tăng trong khi sức lao động thể chất giảm.

Điều này giúp Aging không trở thành hệ thống chỉ để trừ THỂ hoặc XẢO.

---

<a id="m21-t049-s038"></a>

## PHẦN VII — ATTRIBUTE, SKILL & CLASS THEO TUỔI

<a id="m21-t049-s039"></a>

### 23. Attribute Aging

Các thuộc tính đã khóa gồm:

**THỂ, XẢO, GIÁC, TRÍ, TÂM, KHÍ.**

Mỗi thuộc tính chịu tác động tuổi tác theo cách khác nhau.

**THỂ:** Sức mạnh, khả năng chịu đựng và hồi phục có thể suy giảm dần tùy lối sống và sức khỏe.

**XẢO:** Tốc độ phản xạ và sự khéo léo có thể thay đổi, nhưng kỹ năng thao tác tinh vi do kinh nghiệm vẫn có thể duy trì rất tốt.

**GIÁC:** Một số giác quan có thể suy giảm; khả năng nhận diện dấu hiệu quen thuộc vẫn có thể cực kỳ phát triển.

**TRÍ:** Không đồng nhất với trí nhớ ngắn hạn hay tốc độ phản ứng. Kiến thức tích lũy và suy luận chuyên ngành vẫn có thể tăng.

**TÂM:** Có thể phát triển qua kinh nghiệm sống, nhưng không mặc định mọi người già đều điềm tĩnh.

**KHÍ:** Uy tín, bản lĩnh và hiện diện xã hội có thể tăng hoặc giảm theo các biến cố.

Không có công thức chung kiểu:

> Qua tuổi 40: mọi Attribute -1.

<a id="m21-t049-s040"></a>

### 24. Skill và tuổi tác

Skill được tách thành hai thành phần có liên quan nhưng khác nhau:

**Knowledge/Mastery:** Những gì nhân vật đã học và hiểu.

**Execution Capability:** Khả năng thực hiện ngay lúc này.

Một Master cung 65 tuổi có thể vẫn hiểu cực rõ tư thế, góc bắn và chiến thuật săn bắn, nhưng khả năng kéo một cây cung nặng có thể giảm.

Họ không mất hết Skill.

Họ có thể chuyển sang:

- huấn luyện;
- thiết kế công cụ;
- theo dấu;
- chỉ huy cuộc săn;
- dùng một loại vũ khí hoặc kỹ thuật phù hợp hơn.

Đây là **Career Adaptation**, không phải character bị hệ thống tuyên bố “hết giá trị”.

<a id="m21-t049-s041"></a>

### 25. Class Emergence và tuổi

Class không tăng theo tuổi tự động.

Một người 45 tuổi làm nông có thể không có Class chính thức nhưng có Agricultural Skills cực cao.

Một character khác 19 tuổi có thể đã hình thành Liệp Nhân qua những trải nghiệm đặc biệt.

Người lớn tuổi có lợi thế về:

- accumulated evidence;
- reputation;
- mentors;
- techniques;
- professional recognition.

Nhưng cũng có thể gặp hạn chế về:

- thời gian học một con đường mới;
- khả năng thực hiện một số Technique;
- gia đình và trách nhiệm.

<a id="m21-t049-s042"></a>

### 26. Retirement không làm mất identity

Một Lạc Binh đã nghỉ chiến đấu không tự mất Class.

Họ vẫn có thể:

- dạy formation;
- đánh giá binh khí;
- huấn luyện militia;
- cố vấn thủ lĩnh;
- truyền kinh nghiệm chiến tranh.

UI có thể ghi:

**Lạc Binh — Mastered / Retired from Active Combat**

Identity còn, nhưng vai trò thay đổi.

---

<a id="m21-t049-s043"></a>

## PHẦN VIII — BIRTH, CHILDHOOD & NEW GENERATIONS

<a id="m21-t049-s044"></a>

### 27. Trẻ em là nhân vật của thế giới, không phải vật phẩm kế thừa

Hệ gia đình đã được thiết kế theo:

Settlement → Household → Family → Individual.

Khi một đứa trẻ được sinh ra, engine phải tạo nhân vật thông qua cùng cấu trúc đó.

Trẻ có:

- cha/mẹ hoặc người chăm sóc được xác định;
- household;
- lineage;
- điều kiện sinh sống;
- bối cảnh chính trị;
- tài sản và uy tín gia đình;
- những người có thể trở thành mentor;
- các ràng buộc và cơ hội khác nhau.

Không thể chỉ tạo một nhân vật mới bằng cách:

> Child of Player → +2 Skill của cha, +2 Skill của mẹ.

<a id="m21-t049-s045"></a>

### 28. Birth Event

Một Birth Event có thể tác động:

- dân số household;
- nhu cầu tiêu thụ;
- trách nhiệm của người chăm sóc;
- inheritance expectation;
- quan hệ họ hàng;
- tâm lý cha mẹ;
- social standing;
- future succession.

Việc sinh con phải được mô phỏng ở mức phù hợp với độ sâu của từng NPC. Không cần xử lý chi tiết mọi sự kiện cá nhân của hàng chục nghìn cư dân.

Các rủi ro sức khỏe liên quan thai sản, sinh nở và trẻ nhỏ có thể tồn tại trong bối cảnh, nhưng không nên được biến thành những cú sốc ngẫu nhiên xuất hiện quá thường xuyên chỉ để tạo bi kịch.

<a id="m21-t049-s046"></a>

### 29. Child Development

Child Genome phát triển từ:

**Temperament → Environment → Care → Exposure → Formative Events → Mentorship → Adolescence → Emerging Identity.**

Ví dụ:

Con một thợ đúc có thể lớn lên giữa tiếng búa, lò và khuôn đúc.

Điều đó tạo:

- Familiarity với nghề;
- access tới mentor;
- material knowledge;
- social expectation.

Nhưng khi trưởng thành, đứa trẻ có thể chọn nghề thuyền, gia nhập quân đội hoặc rời quê.

<a id="m21-t049-s047"></a>

### 30. Thế hệ mới không lặp lại thế hệ cũ

Một trong những cơ chế replayability lớn nhất là **Generational Divergence**.

Cha có thể trung thành với một dòng họ.

Con có thể cho rằng dòng họ đó đang ngăn cản tương lai.

Người ông có thể coi đất thiêng không thể động đến.

Cháu có thể tham gia xây Cổ Loa.

Hai thế hệ nhìn cùng một vùng đất nhưng thấy hai thế giới khác nhau.

<a id="m21-t049-s048"></a>

### 31. Child to Adult Transition

Khi trẻ trưởng thành, engine cần kiểm tra:

- khả năng tham gia nghề nghiệp;
- trách nhiệm household;
- quan hệ tình cảm;
- mentor;
- di cư;
- political allegiance;
- Class Emergence;
- sự sẵn sàng tự lập.

Không nên cho trẻ đang ở tuổi phụ thuộc trở thành combat companion đầy đủ chỉ vì player muốn một build mới.

Các nhân vật kế tục phải có độ trưởng thành và khả năng tự chủ phù hợp.

---

<a id="m21-t049-s049"></a>

## PHẦN IX — HOUSEHOLD, MARRIAGE & GENERATIONAL SOCIAL GRAPH

<a id="m21-t049-s050"></a>

### 32. Family Structure thay đổi theo thời gian

Một household không có composition cố định.

Có thể xảy ra:

- thành viên qua đời;
- người kết hôn và chuyển nhà;
- người học nghề sống cùng mentor;
- gia đình chia tách;
- người thân di cư;
- một người được nhận chăm sóc;
- household sáp nhập vì kinh tế.

Household Graph phải cập nhật.

<a id="m21-t049-s051"></a>

### 33. Quan hệ thân thuộc không đồng nghĩa quan hệ tình cảm tốt

Player có thể có con nhưng không thân thiết với con.

Một apprentice không cùng huyết thống có thể hiểu và tin player hơn người thân ruột thịt.

Vì vậy khi xây Succession, không được dùng quy tắc:

> Con ruột luôn là người kế tục tốt nhất.

Quan hệ pháp lý, tình cảm, truyền nghề và động cơ phải được xét riêng.

<a id="m21-t049-s052"></a>

### 34. Marriage và quan hệ lâu dài

Một quan hệ gắn bó có thể tạo:

- household mới;
- liên minh giữa family;
- chia sẻ tài nguyên;
- thay đổi nơi sinh sống;
- trách nhiệm chăm sóc;
- xung đột giữa hai lineage;
- quyền thừa kế;
- mạng lưới xã hội mới.

Tuy nhiên không nên ép mọi nhân vật phải kết hôn hoặc có con để nhận đầy đủ cơ chế Legacy.

Một character không có hậu duệ vẫn có thể truyền lại:

- nghề;
- học trò;
- tài sản;
- ký ức;
- công trình;
- tổ chức;
- lời hứa.

<a id="m21-t049-s053"></a>

### 35. Family Relationship theo thời gian

Một sự kiện đã qua có thể được diễn giải lại khi nhiều năm trôi qua.

Ví dụ:

Năm 5, con trai ghét cha vì bị buộc học nghề.

Năm 12, khi tự phải nuôi một household, người con bắt đầu hiểu một phần nỗi lo của cha.

Điều đó không tự động xóa bỏ tổn thương hoặc biến cha thành người đúng.

Character Interpretation Engine phải xử lý sự thay đổi quan điểm dựa trên trải nghiệm mới.

---

<a id="m21-t049-s054"></a>

## PHẦN X — HEALTH, AGING & LONG-TERM MORTALITY

<a id="m21-t049-s055"></a>

### 36. Sức khỏe dài hạn

Combat Injury đã xử lý thương tích trực tiếp.

Module này bổ sung **Long-term Health**.

Mỗi nhân vật có một Health Profile gồm:

- sức khỏe nền;
- bệnh sử;
- tình trạng hồi phục;
- chấn thương tồn tại;
- điều kiện dinh dưỡng;
- cường độ lao động;
- nghỉ ngơi;
- yếu tố tuổi tác;
- những hạn chế chức năng đã hình thành.

Game không cần mô phỏng từng cơ chế bệnh học nhỏ.

Chỉ những tình trạng có ý nghĩa gameplay hoặc narrative mới được nâng thành Persistent Condition.

<a id="m21-t049-s056"></a>

### 37. Mortality Risk

Tử vong có thể đến từ:

- combat;
- tai nạn;
- bệnh nặng;
- thiên tai;
- biến cố chính trị;
- tuổi cao và sức khỏe suy giảm;
- các sự kiện đặc biệt của thế giới.

**Không nên dùng một cơ chế random death vô điều kiện** khiến một NPC quan trọng qua đời chỉ vì đã sang năm mới.

Rủi ro cần có nguyên nhân và, khi phù hợp, các dấu hiệu có thể nhận biết.

Ví dụ:

Một thợ rèn cao tuổi đang yếu đi có thể phải nghỉ làm, giao việc cho con, tìm người học trò tiếp tục nghề.

Nếu sau đó qua đời, đó là phần kết có nhân quả của một cuộc đời, không phải sự kiện xúc xắc bất chợt.

<a id="m21-t049-s057"></a>

### 38. NPC tử vong ngoài màn hình

Các nhân vật không ở gần player vẫn có thể qua đời.

Nhưng cần phân biệt cấp độ:

**NPC background:** Xử lý theo thống kê và điều kiện settlement.

**NPC có Household/Relationship:** Tạo sự kiện gia đình và cập nhật network.

**NPC có Story Thread:** Tử vong phải đi qua Narrative State Validation, giải quyết các quan hệ và nghĩa vụ liên quan.

**Major Historical Character:** Tuân theo Historical Anchor và những điều kiện lịch sử đã thiết kế, không chịu random death làm hỏng campaign.

Quy tắc cuối không có nghĩa nhân vật lịch sử bất tử; nó có nghĩa sự sống và cái chết của họ phải được xử lý ở đúng cấp độ narrative.

<a id="m21-t049-s058"></a>

### 39. Death không phải lúc nào cũng là trừng phạt

Một nhân vật cao tuổi qua đời sau một cuộc đời dài có thể tạo:

- funeral;
- inheritance;
- chuyển giao workshop;
- reunion;
- sự thay đổi leadership;
- ký ức;
- hình thành ancestor status.

Đó có thể là một câu chuyện buồn nhưng trọn vẹn.

Game cần phân biệt bi kịch của một cái chết bất ngờ và quá trình chia tay tự nhiên khi một đời người kết thúc.

---

<a id="m21-t049-s059"></a>

## PHẦN XI — PLAYER CHARACTER DEATH

<a id="m21-t049-s060"></a>

### 40. Player có thể chết thật

Đây là một hệ quả của combat và thế giới đã thiết kế.

Không nên xây một thế giới mà NPC có thể chết vì giáo, lũ hoặc bệnh nhưng player luôn được miễn trừ vô lý.

Tuy nhiên phải bảo vệ tính công bằng của gameplay.

Player Death nên là hậu quả của những chuỗi nguyên nhân có thể hiểu được, chứ không phải một trap giấu kín giết nhân vật sau hàng chục giờ chơi mà không có dấu hiệu.

<a id="m21-t049-s061"></a>

### 41. Trình tự xử lý nguy cơ tử vong

Thông thường:

**Danger → Injury/Condition → Deterioration → Incapacitation → Intervention Window → Death hoặc Survival.**

Không phải mọi cái chết bắt buộc đi qua mọi bước.

Một số tai biến đặc biệt vẫn có thể cực kỳ nghiêm trọng.

Nhưng các rủi ro lớn phải được thể hiện bằng dấu hiệu, kiến thức, hoàn cảnh hoặc mức cảnh báo phù hợp.

<a id="m21-t049-s062"></a>

### 42. Khi player chết, World State không reset

Hệ thống phải thực hiện các bước theo thứ tự:

1. Xác nhận nguyên nhân tử vong.
2. Ghi Death Event.
3. Cập nhật trạng thái cơ thể, vật phẩm, tài sản tại hiện trường.
4. Thông báo tới các actor thực sự chứng kiến.
5. Xử lý companion đang có mặt.
6. Cập nhật quan hệ gia đình và nghĩa vụ.
7. Xác định người có thể tiếp tục các Story Thread.
8. Kiểm tra Succession Eligibility.
9. Chuyển sang Succession hoặc một hình thức kết thúc campaign thích hợp.

Không thể để player chết và thế giới vẫn xem họ là người đang sống.

<a id="m21-t049-s063"></a>

### 43. Người chưa biết về cái chết sẽ không tự biết

Nếu player chết ở Bắc Lộ, người mẹ ở Tam Giang không được nhận thông tin tức thì.

Thông tin phải di chuyển qua:

- witness;
- messenger;
- companion;
- faction;
- rumor.

Có thể xảy ra tình huống:

Gia đình tưởng character mất tích.

Nhiều tháng sau mới biết đã chết.

Hoặc nhận tin sai.

Điều này kết nối trực tiếp Knowledge Graph, Rumor và Reputation.

---

<a id="m21-t049-s064"></a>

## PHẦN XII — DEATH MODES

<a id="m21-t049-s065"></a>

### 44. Ba chế độ vận hành

<a id="m21-t049-s066"></a>

#### Story Mode

Giảm tính sát thương và tăng cơ hội cứu chữa trong những tình huống có thể hợp lý.

Nhân vật vẫn có thể qua đời trong các biến cố narrative và những trường hợp có hậu quả nghiêm trọng.

Không cần xóa toàn bộ chủ đề về cái chết.

<a id="m21-t049-s067"></a>

#### Standard Mode

Đây là baseline của GDD.

Death có thể xảy ra thật. Player được hỗ trợ bởi:

- telegraph;
- retreat;
- preparation;
- treatment;
- companion rescue.

Khi chết, Succession System có thể tiếp tục campaign.

<a id="m21-t049-s068"></a>

#### Legacy / Ironman Mode

Đặt trọng tâm vào kết quả không thể đảo ngược.

- thương tích tồn tại;
- cái chết vĩnh viễn;
- succession là một phần cốt lõi;
- hệ quả khó bị xóa qua reload.

Đây không nhất thiết là chế độ enemy có damage lớn hơn.

<a id="m21-t049-s069"></a>

### 45. Death không được khuyến khích như cách đổi character

Nếu player phát hiện tự sát hoặc lao vào combat chết là cách nhanh nhất để có character mới mạnh hơn, hệ thống đã thất bại.

Succession phải có cost:

- mất khả năng của character cũ;
- mất tri thức chưa truyền;
- mất quan hệ trực tiếp;
- gián đoạn Story Thread;
- khó khăn về tài sản hoặc thừa kế;
- trách nhiệm mà successor phải gánh.

Đây là tiếp nối câu chuyện, không phải nút đổi class.

---

<a id="m21-t049-s070"></a>

## PHẦN XIII — SUCCESSION SYSTEM

<a id="m21-t049-s071"></a>

### 46. Định nghĩa

Succession là:

> **Quá trình chuyển quyền điều khiển sang một character khác có quan hệ nhân quả hợp lý với cuộc đời, mạng xã hội hoặc những Story Thread còn dang dở của nhân vật trước.**

Không phải bất kỳ NPC nào cũng có thể được chọn.

Không nhất thiết phải cùng huyết thống.

Và không bảo đảm rằng successor muốn tiếp tục mọi mục tiêu của nhân vật đã chết.

<a id="m21-t049-s072"></a>

### 47. Nhóm Successor Candidate

Có sáu nhóm chính.

| Nhóm | Điểm mạnh | Rủi ro hoặc giới hạn |
|---|---|---|
| Con/cháu đã trưởng thành | Liên hệ gia đình, thừa kế, ký ức dòng họ | Có thể không cùng mục tiêu |
| Anh/chị/em, họ hàng | Có lịch sử chung và một số quan hệ sẵn có | Có nghĩa vụ riêng |
| Companion | Biết các sự kiện player đã trải qua | Không tự động thừa kế tài sản hoặc quyền lực |
| Apprentice | Có liên hệ nghề nghiệp và truyền thừa | Có thể thiếu tri thức chính trị hoặc quan hệ gia đình |
| Close Ally | Có động cơ tiếp tục một Story Thread | Không nhất thiết có quan hệ thừa kế |
| Người tiếp nhận di sản qua tổ chức | Có thể duy trì một dự án, faction hoặc công trình | Cần quá trình chuyển giao được xã hội thừa nhận |

Không có một thứ tự ưu tiên tuyệt đối.

<a id="m21-t049-s073"></a>

### 48. Successor Eligibility

Đề xuất đánh giá ứng viên bằng sáu tiêu chí.

<a id="m21-t049-s074"></a>

#### Narrative Connection

Ứng viên có liên hệ thật với cuộc đời player không?

<a id="m21-t049-s075"></a>

#### Continuity Knowledge

Họ có biết đủ những gì cần thiết để tiếp tục ít nhất một Story Thread không?

<a id="m21-t049-s076"></a>

#### Motivation

Họ có lý do muốn tiếp tục không?

<a id="m21-t049-s077"></a>

#### Availability

Họ có đang sống, tự chủ và ở hoàn cảnh có thể nhận vai trò không?

<a id="m21-t049-s078"></a>

#### Obligation Compatibility

Việc tiếp tục có mâu thuẫn hoàn toàn với nghĩa vụ của họ không?

<a id="m21-t049-s079"></a>

#### Identity Viability

Họ có khả năng sống và phát triển thành một playable character thú vị không?

Ứng viên không cần là người mạnh nhất.

<a id="m21-t049-s080"></a>

### 49. Điểm đặc biệt: player không được quyền chọn “nhân vật tối ưu” từ toàn thế giới

Tại Character Generation, player không được lựa chọn xuất thân hay chỉ số.

Triết lý này cũng phải chi phối Succession.

Khi character chết, game chỉ có thể đưa ra những lựa chọn có cơ sở từ cuộc đời đã diễn ra.

Nếu chỉ một companion có kiến thức và động cơ hợp lý, họ có thể là người kế tục duy nhất.

Nếu hai hoặc ba người đều phù hợp, game có thể cho player lựa chọn **góc nhìn tiếp tục chiến dịch**, với điều kiện từng ứng viên tự có động cơ chấp nhận.

Player không được biết trước toàn bộ secret và hidden potential của các ứng viên.

<a id="m21-t049-s081"></a>

### 50. Successor không nhất thiết kế thừa mục tiêu

Ví dụ:

Mân sống để tìm ra nguyên nhân một chuỗi người mất tích.

Mân chết.

Miên trở thành successor.

Miên có thể nghĩ:

> “Ta không muốn tiếp tục cuộc điều tra đã giết Mân. Ta muốn cứu những người đang bị ảnh hưởng bởi nó.”

Main Story Thread vẫn có thể tiếp diễn qua cách tiếp cận khác.

Đây là điểm khác biệt lớn với mô hình thay nhân vật nhưng giữ nguyên quest checklist.

<a id="m21-t049-s082"></a>

### 51. Nếu không có successor

Phải thiết kế trước, không để tới khi production mới phát hiện edge case.

Có ba hướng:

**Hướng A — Campaign Epilogue:** Nếu không ai có quan hệ và động cơ hợp lý, cuộc đời player kết thúc; game trình bày những gì thế giới tiếp tục làm sau đó.

**Hướng B — Existing Connected Candidate:** Nếu có một người chưa phải companion nhưng đã được thiết lập quan hệ và thông tin hợp lệ, game có thể chuyển sang họ thông qua một sự kiện tiếp nối.

**Hướng C — New Generation Continuation:** Nếu có hậu duệ hoặc người thừa kế chưa sẵn sàng, game có thể chuyển thời gian tới thời điểm họ thực sự đủ khả năng, nhưng không được bất chấp các Historical Anchor và sự kiện khẩn cấp.

**Đề xuất:** Standard Mode ưu tiên người kế tục hợp lệ, sau đó mới dùng Epilogue; không sinh ra người kế tục không có lịch sử chỉ để tránh Game Over.

---

<a id="m21-t049-s083"></a>

## PHẦN XIV — SUCCESSION TRANSFER: NHỮNG GÌ ĐƯỢC GIỮ VÀ MẤT

<a id="m21-t049-s084"></a>

### 52. Character Sheet không chuyển giao

Không được:

> Mân chết → Miên nhận Skill, Attribute và Class của Mân.

Miên tiếp tục với chính:

- tuổi;
- thể trạng;
- Attributes;
- Skills;
- Profession;
- Class;
- Personality;
- Passive;
- Relationships;
- Reputation;
- Secrets.

Chỉ những tác động đã được ghi nhận trước đó từ Mân mới còn tồn tại.

Ví dụ Miên từng được Mân dạy bơi, nên Skill bơi của Miên đã phát triển từ trước; đó không phải bonus mới phát sinh sau cái chết.

<a id="m21-t049-s085"></a>

### 53. Knowledge Transfer

Đây là một trong những cơ chế quan trọng nhất.

Tri thức được chia thành:

**Personal Knowledge:** Điều Mân từng biết nhưng chưa chia sẻ.

**Shared Knowledge:** Điều companion đã trực tiếp chứng kiến hoặc được kể.

**Recorded Knowledge:** Điều tồn tại trong vật phẩm, bản ghi, lời truyền hoặc Chronicle vật chất.

**Public Knowledge:** Điều cộng đồng biết.

Khi Mân chết, Personal Knowledge chưa truyền có thể mất theo.

Player mới không tự động được truy cập trí nhớ của Mân.

<a id="m21-t049-s086"></a>

### 54. Gameplay xử lý tri thức đã mất

Ví dụ Mân đã tìm ra nơi giấu một vật chứng nhưng chưa kịp nói cho ai.

Sau khi Mân chết:

Miên biết rằng Mân đang điều tra.

Miên có thể biết những người Mân đã gặp.

Nhưng không biết chính xác nơi giấu vật.

Muốn tiếp tục, Miên phải:

- đọc ghi chép nếu còn;
- hỏi người đã chứng kiến;
- quay lại hiện trường;
- tự suy luận.

Đây có thể trở thành Story Thread mới.

<a id="m21-t049-s087"></a>

### 55. Tài sản không tự chuyển vào inventory successor

Tài sản được xử lý qua Inheritance & Ownership System.

Một cây giáo trên thi thể Mân có thể:

- được companion nhặt;
- thất lạc;
- bị kẻ khác lấy;
- được mang về cho gia đình;
- trở thành vật chôn theo người chết;
- được trao cho apprentice.

Player không tự động có quyền điều khiển tất cả tài sản đó sau succession.

<a id="m21-t049-s088"></a>

### 56. Social Network Transfer

Quan hệ cũng không được copy.

Cha của Mân có thể tin Mân nhưng không tin Miên.

Một faction từng nợ Mân có thể:

- công nhận nghĩa vụ với người kế tục;
- chỉ công nhận với gia đình;
- từ chối;
- yêu cầu bằng chứng.

Người successor mang theo uy tín của chính họ, đồng thời chịu ảnh hưởng từ quan hệ với người đã chết.

---

<a id="m21-t049-s089"></a>

## PHẦN XV — INHERITANCE & PROPERTY

<a id="m21-t049-s090"></a>

### 57. Thừa kế là hệ thống xã hội

Không nên thiết kế thừa kế theo một luật thống nhất kiểu RPG hiện đại.

Mỗi Family/Settlement/Community có thể có quy tắc khác nhau về:

- người tiếp quản household;
- công cụ nghề;
- quyền sử dụng đất;
- vật gia truyền;
- tài sản chung;
- nghĩa vụ nợ;
- quyền quản lý nơi chôn cất;
- việc truyền kỹ thuật.

Các quy tắc này thuộc game-canon và cần được kiểm chứng về tính phù hợp lịch sử ở giai đoạn sản xuất.

<a id="m21-t049-s091"></a>

### 58. Inheritance Event

Khi character chết, engine xác định:

```text
InheritanceCase
    DeceasedCharacter
    Household
    EligibleClaimants
    Assets
    Debts
    ExistingPromises
    LocalCustom
    Witnesses
    Disputes
    ResolutionState
```

Nếu có tranh chấp, tạo Story Thread.

Nếu không, quá trình có thể được xử lý tự động.

<a id="m21-t049-s092"></a>

### 59. Vật phẩm gia truyền

Một chiếc khiên đã đi cùng ba thế hệ có thể có nhiều người cùng coi mình là người kế thừa hợp lệ.

Giá trị vật phẩm không chủ yếu nằm ở combat stats.

Nó có thể biểu thị:

- quyền lực dòng họ;
- trách nhiệm giữ nghề;
- lời thề cũ;
- sự ghi nhận một người đã chết;
- bằng chứng của một sự kiện lịch sử.

Việc trao chiếc khiên cho ai có thể là lựa chọn quan trọng hơn một phần thưởng vàng.

<a id="m21-t049-s093"></a>

### 60. Nợ và nghĩa vụ sau cái chết

Không phải mọi nghĩa vụ tự động mất.

Ví dụ:

Mân từng hứa đưa một đứa trẻ về nhà.

Sau khi Mân chết, companion có thể tự nhận tiếp tục lời hứa.

Nhưng hệ thống phải phân biệt:

**Nghĩa vụ pháp lý/tập tục:** Có thể được chuyển theo quy tắc xã hội.

**Nghĩa vụ đạo đức:** Người khác có thể tự nguyện nhận.

**Nợ vật chất:** Có thể gắn với cá nhân, household hoặc tài sản bảo đảm.

**Lời thề nghi lễ:** Có thể có cách diễn giải riêng theo community.

Không được tự động cộng toàn bộ nợ và quest của người chết cho successor.

---

<a id="m21-t049-s094"></a>

## PHẦN XVI — LEGACY SYSTEM

<a id="m21-t049-s095"></a>

### 61. Legacy không đồng nghĩa Fame

Một character có thể nổi tiếng nhưng gần như không để lại tác động bền vững.

Ngược lại, một người vô danh có thể thay đổi cuộc sống của nhiều thế hệ.

Vì vậy Legacy cần được mô hình hóa bằng **những hậu quả tồn tại sau character**, không phải thanh Legendary Reputation.

<a id="m21-t049-s096"></a>

### 62. Bảy dạng Legacy

| Dạng | Bản chất | Ví dụ |
|---|---|---|
| Family Legacy | Hậu duệ, quan hệ và cấu trúc gia đình | Một household vượt qua nạn đói |
| Professional Legacy | Kỹ năng, học trò, phương pháp | Phương pháp đúc khuôn được truyền lại |
| Material Legacy | Vật phẩm và công trình | Cây cầu, thuyền, dụng cụ, thành lũy |
| Social Legacy | Quan hệ, lời hứa, nghĩa vụ | Một cuộc hòa giải kết thúc thù hằn |
| Political Legacy | Tổ chức, chính sách, quyền lực | Một liên minh hoặc lệ làng mới |
| Knowledge Legacy | Tri thức, bản ghi, khám phá | Tuyến đường hoặc lịch sử được lưu giữ |
| Mythic Legacy | Truyền thuyết, biểu tượng, tín niệm | Người cứu lũ được kể thành nhân vật huyền thoại |

Một character có thể để lại nhiều loại cùng lúc.

<a id="m21-t049-s097"></a>

### 63. Legacy có thể tốt, xấu hoặc gây tranh cãi

Không được mặc định Legacy là phần thưởng tích cực.

Player có thể để lại:

- một tuyến thương mại thịnh vượng;
- một nghề thủ công bị thất truyền;
- một lời thề giữ hòa bình;
- một gia đình oán hận;
- một công trình gây thay đổi môi trường;
- một cuộc chiến mà nhiều người vẫn tranh cãi ai là người có lỗi.

Di sản là dấu vết tồn tại, không phải bảng tổng kết đạo đức.

<a id="m21-t049-s098"></a>

### 64. Legacy Object

Các di sản quan trọng được lưu bằng cấu trúc:

```text
LegacyRecord
    OriginCharacter
    OriginEvent
    LegacyType
    AffectedEntities
    CreatedAt
    PersistenceCondition
    TransmissionPath
    CurrentState
    PublicInterpretation
    HistoricalTrace
```

Ví dụ:

Mân mở tuyến đi an toàn qua một đoạn đầm.

Legacy ban đầu tồn tại dưới dạng Route Knowledge.

Sau đó:

- apprentice học;
- người lái thuyền sử dụng;
- thương nhân chuyển tuyến;
- một settlement phát triển.

Dù Mân chết, tuyến đường vẫn có thể tiếp tục được sử dụng.

<a id="m21-t049-s099"></a>

### 65. Legacy có thể suy yếu

Không phải mọi thứ player làm đều tồn tại vĩnh viễn.

Một cây cầu cần bảo dưỡng.

Một kỹ thuật cần người biết truyền.

Một lời thề cần cộng đồng tiếp tục công nhận.

Một trường phái có thể mất học trò.

Một câu chuyện có thể bị quên.

Legacy State có thể chuyển qua:

**Active → Maintained → Neglected → Declining → Lost**

hoặc được:

**Restored / Reinterpreted / Transformed**.

Đây là cách hệ thống giữ lịch sử sống động thay vì lưu hàng nghìn buff vĩnh viễn.

---

<a id="m21-t049-s100"></a>

## PHẦN XVII — MENTORSHIP & TRANSMISSION

<a id="m21-t049-s101"></a>

### 66. Truyền nghề là con đường Legacy quan trọng

Các nghề đã thiết kế đều có thể tạo di sản qua học trò.

Một Master có thể:

- truyền Technique;
- dạy cách đánh giá vật liệu;
- chuyển giao khách hàng;
- giới thiệu apprentice với network;
- để lại workshop;
- phát triển nghề mới.

Việc truyền nghề cần thời gian thực và quan hệ.

Không được:

> Master dies → apprentice instantly learns all Master Skills.

<a id="m21-t049-s102"></a>

### 67. Transmission Quality

Một Technique được truyền thành công phụ thuộc:

- trình độ người dạy;
- khả năng hiểu của người học;
- thời gian luyện tập;
- tài liệu hoặc vật mẫu;
- điều kiện thực hành;
- mối quan hệ giữa hai người.

Người học có thể hiểu sai hoặc cải biến phương pháp.

Đây có thể là nguồn gốc của nhiều Technique Variant theo thế hệ.

<a id="m21-t049-s103"></a>

### 68. Knowledge Loss

Nếu Master chết mà không truyền được kỹ thuật:

Skill của người chết mất khỏi Active Knowledge Pool.

Workshop có thể còn dụng cụ và vật mẫu nhưng không còn ai biết sử dụng chúng hoàn toàn.

Sau này một người khác có thể:

- tìm ghi chép;
- thử nghiệm;
- reverse-engineer;
- phát triển một phương pháp khác.

Lost Knowledge không nhất thiết bị xóa khỏi world lore.

---

<a id="m21-t049-s104"></a>

## PHẦN XVIII — HISTORY, MEMORY & MYTH FORMATION

<a id="m21-t049-s105"></a>

### 69. Ba lớp ghi nhận quá khứ

Game cần lưu riêng:

**World Chronicle:** Những sự kiện khách quan mà simulation đã xử lý.

**Character Chronicle:** Những gì từng nhân vật biết, nhớ hoặc tin về các sự kiện.

**Historical Tradition:** Những version đã lan truyền trong gia đình, cộng đồng hoặc rộng hơn.

Đây là ba nguồn khác nhau.

<a id="m21-t049-s106"></a>

### 70. Một sự kiện có thể thay đổi khi được kể lại

Ví dụ:

Sự thật:

Mân cùng ba companion cứu một đoàn người trong trận lũ.

Ký ức companion:

Miên nhớ Mân suýt chết vì liều lĩnh.

Ký ức người được cứu:

Mân là người đầu tiên quay lại.

Rumor:

Mân cứu cả làng.

Hai mươi năm sau:

> “Người Bến Vạc một mình giữ nước bằng cây giáo.”

Không cần biến event gốc thành phép thuật.

<a id="m21-t049-s107"></a>

### 71. Mythic Legacy cần điều kiện

Không phải mọi nhân vật nổi tiếng đều thành truyền thuyết.

Myth Formation cần một hoặc nhiều yếu tố:

- sự kiện gây ấn tượng lớn;
- việc kể lại qua nhiều người;
- biểu tượng dễ nhớ;
- liên hệ địa điểm hoặc cộng đồng;
- nhu cầu chính trị hoặc tín ngưỡng;
- khoảng cách thời gian;
- ký ức bị giản lược hoặc phóng đại.

Một người có Reputation rất cao nhưng không để lại câu chuyện dễ truyền có thể ít được thần thoại hóa.

<a id="m21-t049-s108"></a>

### 72. Sau khi chết, nhân vật vẫn có thể “thay đổi” trong lịch sử

Không phải họ hành động tiếp, mà là xã hội thay đổi cách hiểu về họ.

Một người từng bị gọi là phản bội có thể được minh oan nhiều năm sau.

Một người anh hùng có thể bị một phe khác coi là nguyên nhân của thảm họa.

Một công trình từng được ca ngợi có thể trở thành biểu tượng của cưỡng bức lao động.

Legacy vì vậy không có một kết luận bất biến.

---

<a id="m21-t049-s109"></a>

## PHẦN XIX — LONG-TERM WORLD SIMULATION

<a id="m21-t049-s110"></a>

### 73. Không thể mô phỏng mọi NPC theo từng ngày trong 25 năm

Thế giới đã xác định có 60 settlement cố định, các điểm nhỏ bổ sung và rất nhiều cư dân.

Nếu engine chạy đầy đủ từng quyết định cá nhân của mọi người mỗi giờ, chi phí tính toán và lượng event sẽ vượt xa nhu cầu gameplay.

Cần sử dụng **Simulation Tiers**.

<a id="m21-t049-s111"></a>

### 74. Tier 1 — Active Simulation

Áp dụng cho:

- Active Party;
- các NPC đang tương tác trực tiếp;
- dungeon/settlement hiện tại;
- Story Thread khẩn cấp.

Mô phỏng chi tiết theo sự kiện và thời gian ngắn.

Ví dụ:

Companion đang chảy máu được theo dõi sát, không thể chờ tới tick mùa.

<a id="m21-t049-s112"></a>

### 75. Tier 2 — Relevant Simulation

Áp dụng cho:

- các settlement liên quan;
- người thân;
- companion ở xa;
- faction/player projects;
- Story Thread đang diễn ra.

Mô phỏng bằng event scheduler và các bước ngày hoặc tuần phù hợp.

Ví dụ:

Workshop do player sở hữu vẫn có đơn hàng, tồn kho, lao động và các biến cố liên quan.

<a id="m21-t049-s113"></a>

### 76. Tier 3 — Aggregate Simulation

Áp dụng cho:

- vùng xa không liên quan trực tiếp;
- dân cư background;
- quần thể động vật;
- thương mại tổng hợp.

Xử lý theo chu kỳ hoặc biến động quan trọng.

Khi player quay lại, state chi tiết được tạo từ dữ liệu aggregate, không phải một thế giới mới không có quá khứ.

<a id="m21-t049-s114"></a>

### 77. Promotion & Demotion

Một NPC bình thường có thể trở nên quan trọng sau một event.

Ví dụ:

Một apprentice vô danh chứng kiến vụ phá hoại lò đúc.

Từ đó trở thành nhân chứng chính của một Story Thread.

Engine nâng cấp độ mô phỏng và bắt đầu lưu dữ liệu chi tiết hơn.

Ngược lại, khi một thread kết thúc và NPC rời khỏi phạm vi quan tâm, họ có thể trở lại chế độ mô phỏng ít tốn tài nguyên hơn.

Dữ liệu lịch sử quan trọng không được mất.

---

<a id="m21-t049-s115"></a>

## PHẦN XX — WORLD EVENT SCHEDULER

<a id="m21-t049-s116"></a>

### 78. Các event không được xử lý theo thứ tự tùy tiện

Ví dụ cùng một ngày:

- quân đội yêu cầu trưng thu lương thực;
- một nhà kho bị cháy;
- thương đoàn tới nơi;
- settlement phải phân phối gạo.

Nếu xử lý sai thứ tự, engine có thể tạo ra hàng hóa hai lần hoặc cho hai actor sở hữu cùng một vật.

Cần **Event Scheduler** có cơ chế xác định thứ tự, điều kiện và tính hợp lệ.

<a id="m21-t049-s117"></a>

### 79. Event Structure

```text
WorldEvent
    EventID
    ScheduledTime
    EventType
    Scope
    Participants
    Preconditions
    RequiredResources
    Priority
    StateChanges
    FollowUpEvents
    ChroniclePolicy
```

Mỗi event có khả năng kiểm tra lại precondition ngay tại thời điểm thực thi.

<a id="m21-t049-s118"></a>

### 80. Event Dependency

Ví dụ:

Bridge Construction Completed

chỉ xảy ra khi:

- công trình chưa bị phá hủy;
- vật liệu đã đủ;
- lao động đã thực hiện;
- thiết kế còn hợp lệ.

Nếu một trận lũ phá công trình giữa chừng, Completion Event phải được thay đổi hoặc hủy hợp lệ.

Không được để scheduler vẫn hoàn thành cầu như chưa có chuyện gì.

<a id="m21-t049-s119"></a>

### 81. Xử lý Time Skip hàng năm

Trong một khoảng nhảy thời gian, engine có thể xử lý theo thứ tự:

1. Sự kiện đã được lịch hóa và các Historical Anchors liên quan.
2. Environment, Weather và Ecology ở độ phân giải phù hợp.
3. Population, births, deaths và household transitions.
4. Production, trade, labor và resource.
5. Faction decisions, conflicts và migration.
6. Career, skill development, projects và relationships.
7. Story Thread transitions và các sự kiện phát sinh.
8. Knowledge propagation, rumor và reputation.
9. Chronicle, History và Myth cập nhật.
10. Validation trước khi lưu snapshot.

Đây là trình tự logic; implementation thực tế cần xử lý các dependency và event priority để tránh tình trạng cùng một sự kiện bị chạy nhiều lần.

---

<a id="m21-t049-s120"></a>

## PHẦN XXI — PERSISTENCE & CONSISTENCY

<a id="m21-t049-s121"></a>

### 82. Mọi sự kiện quan trọng phải có dấu vết

Một cái chết, một vụ di cư hoặc một dự án hoàn thành phải có nguồn.

Không được để engine tự tạo kết quả:

> “NPC A đã chết trong ba năm qua”

nhưng không có nguyên nhân nào trong dữ liệu.

Với NPC background, nguyên nhân có thể được biểu diễn tổng hợp hợp lý.

Với Tier A/S, cần bản ghi chi tiết hơn và có khả năng giải thích.

<a id="m21-t049-s122"></a>

### 83. Event Log & Snapshot

Đề xuất kiến trúc:

**World Event Log:** Lưu các thay đổi quan trọng có thứ tự.

**World Snapshot:** Lưu trạng thái hiện tại để load nhanh.

**Chronicle Projection:** Tạo nội dung player-facing theo những gì character biết.

Không cần replay toàn bộ hàng triệu event mỗi lần load game.

<a id="m21-t049-s123"></a>

### 84. Determinism

Cùng một World Seed, trạng thái và hành động giống nhau phải cho các kết quả mô phỏng tương thích.

Các quyết định quan trọng không được thay đổi chỉ vì reload save rồi chạy lại random khác.

Điều này đặc biệt quan trọng với:

- succession;
- aging;
- profession;
- faction;
- economic shocks;
- historical anchors.

<a id="m21-t049-s124"></a>

### 85. Consistency Validation

Một số bất biến cần kiểm tra:

- người chết không tiếp tục làm việc như đang sống;
- tài sản không cùng lúc chuyển cho hai chủ không có cơ chế đồng sở hữu;
- một đứa trẻ không già hơn cha/mẹ theo quan hệ sinh học thông thường;
- workshop không sản xuất nếu thiếu điều kiện bắt buộc;
- một companion đã rời party không xuất hiện đồng thời ở hai nơi;
- một successor không biết secret chưa có nguồn truyền;
- Historical Anchor không tự bị xóa do sự kiện background;
- các event đã thực thi không được nhân đôi khi load.

Đây là phần kỹ thuật cần có ngay từ đầu, không thể chỉ kiểm thử bằng cách chơi thủ công.

---

<a id="m21-t049-s125"></a>

## PHẦN XXII — QUEST, PROMISE & FACTION THEO THỜI GIAN

<a id="m21-t049-s126"></a>

### 86. Quest không chờ player

Quest System đã chốt rằng Story Thread tồn tại độc lập.

Module Time bổ sung ba cơ chế:

**Event Deadline:** Khi nào một điều có khả năng xảy ra.

**Intervention Window:** Khoảng thời gian player còn có thể thay đổi một outcome cụ thể.

**Aftermath Window:** Các cơ hội mới khi sự kiện đã xảy ra.

Ví dụ một người mất tích.

Sau vài ngày, cửa cứu trực tiếp có thể khó hơn.

Sau vài tuần, một số bằng chứng mất.

Nhưng nhiều năm sau có thể xuất hiện một câu chuyện khác về người đó.

Không có nghĩa toàn bộ content bị xóa ngay khi player đến muộn.

<a id="m21-t049-s127"></a>

### 87. Promise theo thời gian

Lời hứa có thể:

- đến hạn;
- được thực hiện;
- được gia hạn;
- trở nên bất khả thi;
- bị người nhận quên;
- được truyền lại;
- bị tranh cãi.

Ví dụ một lời hứa xây bến thuyền sau mùa lũ.

Nếu player chết trước khi làm, người kế tục có thể được gia đình người hứa hẹn tìm đến.

Nhưng họ không tự động có trách nhiệm phải hoàn thành.

<a id="m21-t049-s128"></a>

### 88. Faction biến đổi qua nhiều năm

Một faction có thể:

- thay thủ lĩnh;
- tái tổ chức;
- đổi mục tiêu;
- chia rẽ;
- sáp nhập;
- mất nguồn lực;
- hình thành thế hệ lãnh đạo mới.

Một alliance ký năm 5 có thể không còn phù hợp năm 15.

Một nhân vật từng là enemy có thể trở thành đồng minh vì lịch sử thay đổi.

Các thay đổi này phải dựa trên Faction Pressure, Leadership và World State, không dùng random faction reroll.

---

<a id="m21-t049-s129"></a>

## PHẦN XXIII — CAMPAIGN PACING & TIME PRESSURE

<a id="m21-t049-s130"></a>

### 89. Không được biến 25 năm thành 25 năm báo động

Nếu mỗi ngày có ba quest khẩn cấp, player không bao giờ có thời gian:

- làm nghề;
- xây dựng gia đình;
- khám phá;
- nghỉ ngơi;
- học kỹ năng;
- hình thành relationship.

Game sẽ trở thành chuỗi deadline và guilt.

Đề xuất chia campaign thành các nhịp:

**Ordinary Life → Rising Pressure → Crisis → Aftermath → Life Interval → New Pressure.**

Những giai đoạn yên bình phải thực sự tồn tại.

<a id="m21-t049-s131"></a>

### 90. Ordinary Life có giá trị

Trong thời gian không có khủng hoảng lớn, character có thể:

- làm nghề;
- ăn mừng;
- cưới hỏi;
- nuôi dạy người nhỏ tuổi;
- học nghề;
- xây dựng tình bạn;
- sửa thuyền;
- chăm vườn;
- tham gia nghi lễ.

Đây không phải filler.

Chính đời sống bình thường khiến một trận chiến hoặc cái chết sau này trở nên có trọng lượng.

<a id="m21-t049-s132"></a>

### 91. Không buộc player biết mọi biến cố

World Simulation có thể tạo rất nhiều sự kiện, nhưng Narrative Director chỉ nên expose những gì:

- player có khả năng biết;
- liên quan tới các mối quan hệ;
- tạo quyết định có ý nghĩa;
- phù hợp nhịp trải nghiệm hiện tại.

Một người thân ở xa có thể gửi tin.

Một người không quen chết tại vùng khác có thể chỉ xuất hiện trong thống kê và lịch sử cộng đồng.

Không cần hàng trăm thông báo trong một lần Time Skip.

---

<a id="m21-t049-s133"></a>

## PHẦN XXIV — VÍ DỤ CHI TIẾT: MỘT ĐỜI NGƯỜI TRONG 25 NĂM

<a id="m21-t049-s134"></a>

### 92. Nhân vật khởi đầu

**Vạc Mân — 17 tuổi**

Quê: Bến Vạc, Tam Giang.

Gia đình: Nhà Vạc Tòng.

Nghề: Người học việc trên thuyền.

Khả năng nổi bật:

- Bơi tốt;
- quan sát khá;
- biết sông;
- chiến đấu yếu.

Không có Class chính thức.

Mâu thuẫn tâm lý:

Muốn rời quê khám phá nhưng sợ mình bỏ gia đình trong lúc họ cần.

<a id="m21-t049-s135"></a>

### 93. Năm 0–3: Trưởng thành

Mân tham gia tìm người mất tích.

Học theo dấu.

Đi nhiều tuyến sông.

Có relationship với Miên và Vũ.

Một cuộc cứu nạn lớn tạo:

- Milestone;
- Shared Memory;
- Professional Reputation.

Class **Thủy Binh** bắt đầu hình thành.

Gia đình vẫn làm nghề vận tải.

<a id="m21-t049-s136"></a>

### 94. Năm 3–7: Nghề nghiệp và trách nhiệm

Mân bắt đầu dẫn những chuyến thuyền dài.

Tham gia một cuộc xung đột ở Tam Giang.

Bị thương chân ở mức Moderate.

Hồi phục nhưng để lại dấu vết.

Có thể:

- tiếp tục nghề cũ;
- học phương pháp đi thuyền khác;
- dạy người em;
- rời sông để tham gia một Story Thread chính trị.

Trong cùng giai đoạn, cha Mân già đi và muốn giao việc.

Mân phải quyết định trách nhiệm của mình với gia đình.

<a id="m21-t049-s137"></a>

### 95. Năm 7–12: Hai con đường

Các phe phái biến đổi.

Miên đã có sự nghiệp chữa bệnh riêng.

Vũ không còn là hunter trẻ như ngày trước.

Một lời hứa cũ của Mân trở nên quan trọng vì các cộng đồng chuyển allegiance.

Nếu Mân theo faction:

career có thể chuyển.

Nếu ở lại quê:

có thể trở thành trưởng nhóm vận tải.

Nếu không tham gia chính trị:

lịch sử vẫn tiến.

Những người xung quanh đưa ra quyết định riêng.

<a id="m21-t049-s138"></a>

### 96. Năm 12–17: Cổ Loa

Nhu cầu vận chuyển vật liệu tăng.

Mân có cơ hội tham gia tuyến hậu cần.

Một số người bạn hưởng lợi từ dự án.

Một gia đình Mân quen biết mất đất.

Player phải quyết định có dùng influence để giúp họ hay không.

Trong giai đoạn này, Mân có thể:

- giàu lên;
- hình thành network nghề;
- mở một tuyến thuyền;
- đào tạo người kế tục;
- trở thành người bị một cộng đồng oán trách.

<a id="m21-t049-s139"></a>

### 97. Năm 17–21: Công nghệ và chiến tranh

Mân lúc này đã khoảng 34–38 tuổi.

Không còn là một thiếu niên vô danh.

Một số người biết tên.

Một số người nhớ những lần Mân thất hứa.

Mân có thể tham gia lịch sử phát triển vũ khí bằng vai trò:

- logistics;
- đưa người;
- bảo vệ hàng;
- tham gia một mạng tin tức;
- hỗ trợ vận chuyển nguyên vật liệu.

Không cần trở thành người chế tạo Nỏ thần để có vai trò ý nghĩa.

<a id="m21-t049-s140"></a>

### 98. Năm 21–25: Khủng hoảng cuối thời đại

Mân khoảng 38–42 tuổi.

Những người từng đồng hành đã thay đổi sâu sắc.

Một số có thể đã chết.

Một số trở thành lãnh đạo.

Một số ở phía chính trị đối lập.

Mân đối diện biến cố lớn với:

- kinh nghiệm;
- những món nợ;
- những người mình yêu thương;
- các sai lầm đã phạm;
- di sản vật chất và xã hội.

Kết thúc campaign không chỉ hỏi:

> Mân có thắng trận cuối không?

Mà còn hỏi:

> Mân đã sống thành người như thế nào?

---

<a id="m21-t049-s141"></a>

## PHẦN XXV — VÍ DỤ THAY THẾ: PLAYER CHẾT Ở NĂM 9

<a id="m21-t049-s142"></a>

### 99. Tình huống

Mân chết năm thứ 9 trong một nhiệm vụ đưa người qua khu vực giao tranh.

Tại thời điểm đó:

- Miên là companion thân thiết;
- Vũ đang ở một vùng khác;
- người em của Mân ở Bến Vạc;
- một apprentice từng theo Mân học nghề sông;
- Mân có một lời hứa chưa thực hiện tại Phong Châu.

<a id="m21-t049-s143"></a>

### 100. Death Event

Miên chứng kiến Mân chết.

Cô cố cứu nhưng không thành công.

Nếu có điều kiện, party đưa thi thể về.

Gia đình Mân được thông báo sau một khoảng thời gian di chuyển hợp lý.

Miên có thể cảm thấy:

- grief;
- guilt;
- anger;
- determination.

Nhưng không mặc định tất cả.

<a id="m21-t049-s144"></a>

### 101. Candidate Succession

Có thể có ba ứng viên hợp lệ:

**Miên:** Hiểu một số Story Thread lớn và có lý do tiếp tục.

**Người em của Mân:** Có liên hệ trực tiếp với family, nghề nghiệp và tài sản.

**Apprentice:** Có động cơ nối tiếp tuyến đường hoặc nghề.

Mỗi ứng viên tạo một chiến dịch tiếp tục khác nhau.

<a id="m21-t049-s145"></a>

### 102. Chọn Miên

Player giờ điều khiển một Dược Nhân.

Miên không giỏi chiến đấu như Mân từng trở thành.

Cô có:

- nghề chữa bệnh;
- quan hệ ở Bãi Lau;
- tri thức y học;
- các giá trị cá nhân.

Main campaign tiếp tục, nhưng các vấn đề được nhìn qua góc độ của Miên.

Miên có thể tìm lại những người từng biết Mân.

Một số quý cô vì Mân.

Một số đổ lỗi cho cô về cái chết của Mân.

<a id="m21-t049-s146"></a>

### 103. Vật phẩm và tri thức

Cây giáo của Mân được trao về nhà.

Miên giữ một vật nhỏ Mân từng cho mình.

Một số thông tin Mân chưa kể đã mất.

Một vài năm sau, Miên có thể gặp người apprentice và nhận ra người này biết một phần quá khứ Mân mà cô chưa từng biết.

Người chơi lúc này đang **điều tra chính cuộc đời nhân vật mình từng điều khiển**.

Đây là một trải nghiệm đặc biệt mà Succession System có thể tạo ra.

---

<a id="m21-t049-s147"></a>

## PHẦN XXVI — VÍ DỤ VỀ LEGACY NHIỀU THẾ HỆ

<a id="m21-t049-s148"></a>

### 104. Một công trình nhỏ

Năm 4:

Player giúp Bến Nếp xây một bến thuyền mới.

Năm 8:

Thương mại ổn định hơn.

Năm 12:

Một household chuyển đến vì công việc vận tải.

Năm 17:

Người đứng đầu bến qua đời, con họ tiếp quản.

Năm 22:

Bến được sử dụng làm tuyến vận chuyển quan trọng trong thời kỳ xung đột.

Cuối campaign:

Người ta vẫn có thể nhắc tới người từng giúp dựng bến.

Nhưng không nhất thiết nhớ đúng tên hoặc công lao.

<a id="m21-t049-s149"></a>

### 105. Di sản có thể biến đổi

Nếu tuyến nước thay đổi:

Bến cũ không còn phù hợp.

Người đời sau có thể dỡ gỗ để xây một công trình mới.

Vật chất biến mất nhưng kỹ thuật, tuyến vận chuyển hoặc ký ức vẫn còn.

Legacy không cần bất tử để có ý nghĩa.

---

<a id="m21-t049-s150"></a>

## PHẦN XXVII — ENDGAME & EPILOGUE

<a id="m21-t049-s151"></a>

### 106. Ending phải có nhiều tầng

Cuối campaign cần tổng kết ít nhất năm tầng:

<a id="m21-t049-s152"></a>

#### Historical Outcome

Những biến cố lớn của thời đại.

<a id="m21-t049-s153"></a>

#### Personal Outcome

Cuộc đời player character cuối cùng.

<a id="m21-t049-s154"></a>

#### Family & Companion Outcome

Những người gắn bó với player.

<a id="m21-t049-s155"></a>

#### Settlement & Faction Outcome

Các cộng đồng và thế lực đã thay đổi.

<a id="m21-t049-s156"></a>

#### Legacy & Memory Outcome

Những gì còn tồn tại và những gì được người đời sau kể lại.

Không nên chỉ có một cutscene về nhà vua hoặc một điểm Ending Alignment.

<a id="m21-t049-s157"></a>

### 107. Kết thúc của một nhân vật có thể khác kết thúc chiến dịch

Ví dụ:

Mân chết năm 9.

Miên tiếp tục đến năm 25.

Khi game kết thúc, Epilogue phải trình bày cả hai cuộc đời.

Mân có một phần Legacy riêng.

Miên có một phần riêng.

Những đóng góp của Mân không bị ghi đè thành của Miên.

<a id="m21-t049-s158"></a>

### 108. Ending không chỉ thưởng người nổi tiếng

Một người chọn sống bình thường có thể có hậu truyện đáng nhớ.

Ví dụ:

> “Sau những biến động của Âu Lạc, Miên trở về Bãi Lau. Nơi cô chữa bệnh tiếp tục hoạt động nhiều năm sau, được những người học nghề của cô duy trì.”

Đây là ending có giá trị ngang một ending mà nhân vật trở thành tướng lĩnh nổi tiếng, nếu nó phù hợp với con đường người chơi đã sống.

<a id="m21-t049-s159"></a>

### 109. Hậu truyện nhiều thế hệ

Game có thể có một lớp Epilogue xa hơn, khi những người trực tiếp tham gia đã qua đời.

Nhưng phải phân biệt:

**Observed Future:** Tương lai được engine mô phỏng đủ căn cứ.

**Projected Future:** Xu hướng được suy ra từ World State.

**Mythic Retelling:** Câu chuyện mà thế hệ sau kể lại.

Không được dùng một đoạn kết đẹp để khẳng định chắc chắn những hậu quả dài hạn mà simulation chưa thực sự xác lập.

---

<a id="m21-t049-s160"></a>

## PHẦN XXVIII — PLAYER UI / UX

<a id="m21-t049-s161"></a>

### 110. Calendar & Timeline UI

UI nên có ba dạng.

**Current Time:** Người chơi biết đang ở năm và giai đoạn mùa nào.

**Personal Timeline:** Những mốc lớn trong cuộc đời character.

**World Chronicle:** Các mốc lịch sử, faction và settlement mà nhân vật đã biết.

Không cần một giao diện lịch dày đặc như ứng dụng đặt lịch.

<a id="m21-t049-s162"></a>

### 111. Character Life Timeline

Ví dụ:

**Vạc Mân**

- Năm 0 — Rời Bến Vạc lần đầu.
- Năm 2 — Trải qua trận lũ tại Tam Giang.
- Năm 4 — Hình thành Thủy Binh.
- Năm 6 — Bị thương chân.
- Năm 8 — Nhận một apprentice.
- Năm 9 — Qua đời tại Bắc Lộ.

Các mốc này tồn tại trong Chronicle kể cả sau Succession.

<a id="m21-t049-s163"></a>

### 112. Aging UI

Không nên hiện:

> Tuổi 40 → Strength -2.

Nên trình bày quan hệ nguyên nhân:

> “Những năm lao động trên sông giúp Mân có sức chịu đựng tốt, nhưng vết thương ở chân khiến việc đi đường dài trở nên khó khăn.”

Advanced UI có thể cho player kiểm tra những thay đổi số liệu cụ thể nếu cần.

<a id="m21-t049-s164"></a>

### 113. Succession Screen

Khi player chết, giao diện không nên giống Character Selection thông thường.

Nó cần cho thấy:

- ai biết về cái chết;
- những người có khả năng tiếp tục;
- vì sao họ có liên hệ;
- những gì player biết về cuộc đời họ;
- vấn đề họ đang đối diện.

Không nên tiết lộ hidden attributes, future story hoặc bí mật không có nguồn.

<a id="m21-t049-s165"></a>

### 114. Legacy Chronicle

Một mục riêng có thể tổng hợp:

**Những gì người này để lại.**

Bao gồm:

- người học trò;
- vật phẩm đã chế tác;
- quan hệ còn tồn tại;
- công trình;
- lời hứa chưa hoàn thành;
- những câu chuyện người khác kể về họ.

Không cần biến thành bảng điểm tổng kết.

---

<a id="m21-t049-s166"></a>

## PHẦN XXIX — DIFFICULTY, ACCESSIBILITY & FAIRNESS

<a id="m21-t049-s167"></a>

### 115. Không biến Aging thành đồng hồ ép player nghỉ game

Nếu nhân vật bắt đầu ở tuổi 55, game phải vẫn cho phép một playthrough có giá trị.

Đó có thể là campaign của:

- người thầy;
- trưởng household;
- cố vấn;
- người đã trải qua nhiều biến cố;
- một người tìm cách sửa sai trong những năm cuối đời.

Không nhất thiết phải cho họ tất cả khả năng của người 20 tuổi.

Nhưng cần cung cấp cơ hội gameplay đủ sâu.

<a id="m21-t049-s168"></a>

### 116. Không có “bad random starting age”

Vì Character Generation đã khóa là hoàn toàn ngẫu nhiên, hệ thống phải đảm bảo các nhân vật có tuổi và hoàn cảnh khác nhau đều qua được Character Viability Gate.

Nhân vật cao tuổi có thể:

- giỏi nghề;
- có network;
- có tài sản;
- có kinh nghiệm;
- biết một số Story Thread.

Nhân vật trẻ có thể:

- dễ chuyển nghề hơn;
- ít nghĩa vụ hơn;
- có thời gian phát triển dài hơn.

Không được cố cân bằng bằng cách làm hai kiểu nhân vật giống nhau.

<a id="m21-t049-s169"></a>

### 117. Cảnh báo hậu quả thời gian

Nếu player định nghỉ sáu tháng khi đang biết có một người bệnh nguy kịch, game cần cảnh báo hợp lý.

Nhưng không cần tiết lộ:

> “NPC sẽ chết đúng ngày 12.”

Dược Nhân có thể dự báo tốt hơn.

Người không có chuyên môn chỉ biết tình trạng nghiêm trọng.

---

<a id="m21-t049-s170"></a>

## PHẦN XXX — CÁC RỦI RO THIẾT KẾ VÀ BIỆN PHÁP

<a id="m21-t049-s171"></a>

### 118. Rủi ro: thế giới thay đổi quá nhanh

Nếu mỗi lần nghỉ vài tháng player mất ba người thân, hai workshop và một đồng minh, game trở thành bất công.

**Giải pháp:** Cân bằng tần suất major life event; phân biệt các rủi ro thông thường với áp lực đã được gieo mầm. Những biến cố lớn nên có chuỗi nguyên nhân đủ rõ.

<a id="m21-t049-s172"></a>

### 119. Rủi ro: Time Skip làm mất cảm xúc

Nếu player có mối quan hệ sâu với companion nhưng sau một lần nhảy năm năm chỉ nhận dòng:

> “Companion married and moved away.”

thì kết quả có thể thiếu sức nặng.

**Giải pháp:** Những quan hệ quan trọng cần có các Bridge Scenes hoặc Life Interval Events cho phép player tham gia vào một số bước ngoặt phù hợp.

Không cần dựng mọi sự kiện nhỏ thành cutscene.

<a id="m21-t049-s173"></a>

### 120. Rủi ro: Succession phá gắn bó với player character

Nếu game chuyển nhân vật liên tục, player có thể không kịp xây dựng quan hệ cảm xúc với ai.

**Giải pháp:** Tử vong nguy hiểm nhưng không quá thường xuyên; các character phải có cơ hội phát triển dài; Legacy Mode có thể nghiêm hơn nhưng Standard cần bảo đảm khả năng chuẩn bị, rút lui và cứu chữa thật sự hữu ích.

<a id="m21-t049-s174"></a>

### 121. Rủi ro: successor yếu khiến game không tiếp tục được

Nếu player chuyển từ một Lạc Binh Master sang một healer có combat skill thấp, các encounter về sau có thể trở nên bất khả thi.

**Giải pháp:** Main Story phải giữ thiết kế nhiều hướng giải quyết. World không điều chỉnh toàn bộ sức mạnh cho player, nhưng phải tồn tại các tuyến xã hội, điều tra, tuyển người, chuẩn bị và tránh chiến đấu hợp lý.

<a id="m21-t049-s175"></a>

### 122. Rủi ro: simulation tạo quá nhiều thông báo

Nếu mỗi năm có hàng trăm event, người chơi không thể theo dõi.

**Giải pháp:** Narrative Relevance Filter; thông tin được phân tầng thành biến cố cá nhân, quan hệ, cộng đồng và lịch sử. Các sự kiện nhỏ được tổng hợp, trừ khi chúng trở thành nguyên nhân của Story Thread lớn.

<a id="m21-t049-s176"></a>

### 123. Rủi ro: Historical Anchor làm player mất quyền tự chủ

Nếu mọi quyết định cuối cùng đều bị một cutscene lịch sử xóa bỏ, toàn bộ hệ Quest/Faction trở nên vô nghĩa.

**Giải pháp:** Khóa kết quả lõi của một số Anchor nhưng mở rộng tác động địa phương và cá nhân. Player có thể không thay đổi được việc một thời đại kết thúc, nhưng có thể thay đổi sâu sắc ai còn sống, cộng đồng nào được bảo vệ và những di sản nào tồn tại sau nó.

---

<a id="m21-t049-s177"></a>

## PHẦN XXXI — QUALITY GATES

<a id="m21-t049-s178"></a>

### 124. Time System Quality Gate

Một hệ thời gian đạt yêu cầu khi:

- hoạt động khác nhau tiêu tốn thời gian hợp lý;
- World State không phụ thuộc player có mặt;
- deadline thật được thực thi;
- seasonal state tác động gameplay;
- Time Skip giải quyết sự kiện có nhân quả;
- player nhận đủ thông tin trước các khoảng nhảy lớn;
- event không bị xử lý trùng lặp.

<a id="m21-t049-s179"></a>

### 125. Aging Quality Gate

Một nhân vật có quá trình già đi tốt khi:

- tuổi tác ảnh hưởng nhưng không định nghĩa toàn bộ khả năng;
- Career/Relationship cũng thay đổi;
- kinh nghiệm có thể tạo lợi thế;
- sức khỏe có diễn tiến nhân quả;
- tuổi cao mở ra gameplay truyền nghề, authority và legacy;
- không xuất hiện hard age penalty thiếu hợp lý.

<a id="m21-t049-s180"></a>

### 126. Death Quality Gate

Một Death Event quan trọng phải có:

- nguyên nhân;
- thời điểm;
- location;
- người chứng kiến;
- trạng thái cơ thể;
- tài sản liên quan;
- người cần được thông báo;
- tác động lên household;
- Story Thread còn mở;
- funeral hoặc aftermath khi phù hợp.

<a id="m21-t049-s181"></a>

### 127. Succession Quality Gate

Một successor chỉ hợp lệ nếu:

- tồn tại trước sự kiện hoặc được hình thành hợp lý từ world;
- có quan hệ nhân quả;
- có khả năng tự chủ;
- có động cơ;
- có Character Sheet riêng;
- chỉ biết những điều có nguồn;
- không tự động thừa kế Class/Skill;
- có ít nhất một con đường phát triển gameplay khả thi;
- không chỉ được sinh ra để thay thế một slot trống.

<a id="m21-t049-s182"></a>

### 128. Legacy Quality Gate

Một di sản cần xác định:

- nguồn gốc;
- ai hoặc điều gì bị ảnh hưởng;
- phương tiện lưu truyền;
- điều kiện tồn tại;
- khả năng suy yếu/biến đổi;
- ai còn biết đến nó;
- ý nghĩa khác nhau với từng cộng đồng.

---

<a id="m21-t049-s183"></a>

## PHẦN XXXII — CÁC QUYẾT ĐỊNH ĐỀ XUẤT KHÓA

<a id="m21-t049-s184"></a>

### 129. Time & Calendar

1. Toàn game sử dụng một World Timeline thống nhất.
2. Thời gian được mô phỏng ở nhiều độ phân giải.
3. Campaign chính khoảng 20–25 năm.
4. Calendar phản ánh mùa sinh thái và văn hóa địa phương.
5. Không mặc định lịch hiện đại hoặc mùa kiểu fantasy châu Âu.
6. Time Skip chia thành Routine, Life Interval và Historical Transition.
7. World Simulation tiếp tục trong Time Skip.
8. Nhiệm vụ khẩn cấp phải được kiểm tra trước khi nhảy thời gian lớn.
9. Time Skip phải có báo cáo những biến đổi quan trọng.
10. Historical Anchors có lõi ổn định nhưng local outcomes linh hoạt.

<a id="m21-t049-s185"></a>

### 130. Aging & Life Cycle

11. Mọi character đều già đi.
12. NPC không đóng băng khi player ở xa.
13. Birth, Death, Household Change và Career Change là World Events.
14. Tuổi không tạo debuff cứng cho mọi nhân vật.
15. Physical Capability và Knowledge được xử lý riêng.
16. Trẻ em phát triển dựa trên môi trường và lịch sử sống.
17. Hậu duệ không tự thừa hưởng Skill hoặc Class.
18. Retirement không xóa character identity.
19. Nhân vật lớn tuổi vẫn có gameplay path sâu.
20. Long-term Health có nhân quả và ảnh hưởng nghề nghiệp.

<a id="m21-t049-s186"></a>

### 131. Death & Succession

21. Player có thể chết thật trong các chế độ phù hợp.
22. Death là World Event vĩnh viễn khi được xác lập.
23. Knowledge về cái chết lan theo Information Network.
24. Succession không phải reincarnation.
25. Người kế tục phải có quan hệ và động cơ hợp lý.
26. Successor giữ Character Sheet của chính mình.
27. Knowledge không được chuyển giao toàn tri.
28. Tài sản chuyển theo Ownership/Inheritance System.
29. Quan hệ cá nhân không tự động chuyển sang successor.
30. Nếu không có successor hợp lệ, campaign có thể kết thúc bằng Epilogue.

<a id="m21-t049-s187"></a>

### 132. Legacy & History

31. Legacy được ghi bằng các hậu quả còn tồn tại, không phải Fame Score.
32. Legacy có thể tốt, xấu hoặc gây tranh cãi.
33. Công trình, kỹ thuật và tổ chức có thể suy yếu hoặc biến mất.
34. Truyền nghề cần thời gian và điều kiện.
35. Một nhân vật có thể nổi tiếng hơn hoặc kém hơn công lao thật.
36. World Chronicle, Character Chronicle và Historical Tradition tách biệt.
37. Myth Formation có thể phát triển sau cái chết nhân vật.
38. Ending phải bao gồm cá nhân, gia đình, cộng đồng, lịch sử và di sản.
39. Các nhân vật đã chết vẫn tồn tại trong Chronicle và Social Memory.
40. Không dùng LLM để quyết định tuổi thọ, tử vong, thừa kế hoặc lịch sử khách quan.

---

<a id="m21-t049-s188"></a>

## PHẦN XXXIII — DEFERRED TO PRODUCTION

<a id="m21-t049-s189"></a>

### 133. Những thông số chưa nên khóa

Giai đoạn production mới xác định:

- phân bố tuổi của nhân vật khởi đầu;
- tốc độ suy giảm và phát triển thuộc tính theo từng khoảng tuổi;
- bảng rủi ro sức khỏe;
- mô hình sinh/tử dân số;
- quy tắc thừa kế theo từng cộng đồng;
- độ dài cụ thể từng hồi;
- số năm nhảy thời gian;
- tần suất Life Event;
- chi phí vận hành simulation;
- threshold Succession Eligibility;
- thuật toán Historical Anchor Scheduling;
- các tình huống ngoại lệ của Time Skip;
- nội dung Bridge Scenes;
- thiết kế Ending/Epilogue UI;
- mức độ mô phỏng nhiều thế hệ sau campaign.

Những số liệu này phải được cân chỉnh sau khi đã có vertical slice đủ dài để kiểm tra một vòng đời thực tế.

---

<a id="m21-t049-s190"></a>

## PHẦN XXXIV — CORE DESIGN STATEMENT

<a id="m21-t049-s191"></a>

### 134. Fantasy của thời gian

Một game thông thường có thể khiến player hỏi:

> “Tôi cần bao nhiêu giờ để lên Level tiếp theo?”

Game này phải khiến player hỏi:

> “Tôi nên dành những năm tiếp theo của cuộc đời vào điều gì?”

Học một nghề mới?

Trở về với gia đình?

Đi giúp người bạn cũ?

Tham gia vào một cuộc biến động lịch sử?

Hay chấp nhận rằng mình không thể làm tất cả?

<a id="m21-t049-s192"></a>

### 135. Fantasy của tuổi già

Tuổi già không nên có nghĩa:

> “Character của tôi đã hết mạnh.”

Nó nên mở ra một câu hỏi khác:

> “Sau tất cả những gì đã học và đã trải qua, tôi còn muốn làm điều gì trước khi không thể nữa?”

Một người thợ có thể muốn tìm học trò.

Một chiến binh có thể muốn chấm dứt một cuộc thù hằn.

Một thủ lĩnh có thể muốn trao quyền cho người phù hợp.

Một người cha có thể muốn sửa lại mối quan hệ với con.

Một người sống cô độc có thể muốn lần đầu tìm một nơi thuộc về mình.

<a id="m21-t049-s193"></a>

### 136. Fantasy của cái chết

Cái chết không chỉ là:

> “Bạn đã thua.”

Nó có thể là:

> “Cuộc đời này đã kết thúc. Nhưng thế giới mà người ấy từng sống vẫn đang tiếp tục.”

Một companion mang thi thể về quê.

Một người mẹ nhận tin.

Một người học trò cầm lại dụng cụ của thầy.

Một lời hứa còn dang dở.

Một cây cầu vẫn có người qua.

Một kẻ thù vẫn nhớ cái tên ấy.

Không phải mọi cuộc đời đều trở thành truyền thuyết.

Nhưng mọi cuộc đời đều có thể để lại hậu quả.

<a id="m21-t049-s194"></a>

### 137. Fantasy của người kế tục

Khi player bắt đầu campaign, họ không được chọn mình sinh ra là ai.

Khi một nhân vật qua đời, người kế tục cũng không được tạo thành một bản sao tối ưu.

Người kế tục nhận lấy một thế giới đã bị thay đổi bởi những người trước.

Họ phải tự quyết định:

> “Tôi có muốn tiếp tục những gì người ấy đã làm không?”

Họ có thể giữ lại.

Có thể sửa chữa.

Có thể phản đối.

Có thể vượt qua.

Và đôi khi, lựa chọn có ý nghĩa nhất là không tiếp tục một sai lầm cũ.

---

<a id="m21-t049-s195"></a>

## PHẦN XXXV — VỊ TRÍ TRONG TOÀN BỘ GDD

<a id="m21-t049-s196"></a>

### 138. Vòng lặp lớn nhất của game

**World & Historical Era**

↓

**Character được sinh ra trong một Family và Settlement**

↓

**Childhood, Personality, Attributes, Skills**

↓

**Profession, Relationships, Class Emergence**

↓

**Player trải nghiệm Travel, Quest, Dungeon, Combat, Social & Crafting**

↓

**Life Events tạo ký ức, thương tích, lời hứa và thành tựu**

↓

**Time trôi — nhân vật và thế giới thay đổi**

↓

**Career, Family, Faction và Settlement phát triển**

↓

**Aging, Mentorship, Conflict, Death**

↓

**Succession hoặc kết thúc một đời người**

↓

**Legacy được truyền, biến đổi hoặc lãng quên**

↓

**Memory trở thành History**

↓

**History có thể trở thành Myth**

↓

**Một thế hệ khác bước vào thế giới đã thay đổi.**

---

<a id="m21-t049-s197"></a>

## 139. TUYÊN NGÔN THIẾT KẾ CUỐI CÙNG

Toàn bộ hệ Time, Aging, Death, Succession & Legacy phục vụ một triết lý duy nhất:

**Người chơi không lựa chọn mình sinh ra là ai.**

**Người chơi lựa chọn cách sống với cuộc đời đã nhận.**

**Người chơi không quyết định mọi biến cố của lịch sử.**

**Nhưng họ có thể thay đổi cuộc đời những người sống trong lịch sử đó.**

**Người chơi không thể giữ nhân vật của mình mãi mãi.**

**Nhưng những gì nhân vật làm có thể tiếp tục tồn tại sau khi họ qua đời.**

Và cuối cùng:

> **Đây không phải game về một nhân vật trở nên bất tử. Đây là game về những con người hữu hạn sống trong một thế giới có ký ức dài hơn chính họ.**

---

<a id="m21-t049-s198"></a>

### Đánh giá vị trí GDD sau module này

Đến thời điểm hiện tại, các hệ thống đã có nền liên thông từ **Character Generation → Profession → Class → Party → Combat → Exploration → Dungeon → Social → Quest → Economy → Religion/Mạch → Ecology → Time/Legacy**.

Điểm còn thiếu quan trọng nhất không phải thêm một cơ chế gameplay độc lập, mà là **cách game tổ chức toàn bộ những hệ này thành một trải nghiệm có nhịp độ, có khả năng tiếp cận và đủ ổn định để phát triển thực tế**.

Tôi đề xuất bước kế tiếp là **Game Loop + Campaign Structure + World Director + Difficulty & Game Modes**. Module đó cần trả lời một vấn đề thực tế: với hàng chục hệ thống rất sâu đang có, người chơi sẽ làm gì trong 15 phút đầu, một giờ đầu, 10 giờ đầu, và game làm thế nào để tránh quá tải thông tin, quá nhiều nhiệm vụ hoặc những khoảng thời gian không có việc gì đáng làm.

Sau đó nên chuyển sang **UI/UX + Codex/Chronicle**, rồi **AI Narrative Architecture + Content Generation Pipeline + Validation**, trước khi tổng hợp bản GDD hoàn chỉnh phục vụ phát triển.

<!-- SOURCE T049 END -->

