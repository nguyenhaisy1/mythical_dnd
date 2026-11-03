# Phản biện hình ảnh18 — mười lăm cách thể hiện và hai bản đầu được giữ

02/11/2026 · Sprint13 / SPIN-035 · Reviewer độc lập `/root/s13e18_critic_visual` · **Visual reviewed trong phạm vi đã xem; Pending user approval**.

## Phạm vi quan sát thực

Đã mở trực tiếp đủ17PNG tại `prototypes/sprint13/experiment-18/assets/`:15frame trưng bày từ `open-yard` đến `retro-facet`, cùng `ribbon-body-v1` và `contour-step-v1`. Đọc ART-DIRECTION, ghi chú output của gallery, provenance, baseline áp dụng và hướng dẫn mythical-validation. Nhận xét dưới đây căn cứ ảnh đã mở, không lấy nhãn phương pháp hoặc prompt làm bằng chứng kết quả đạt.

Đã mở thêm đúng ba PNG cũ để đối chiếu: vòng12 `clay-craft.png`, vòng16 `line-volume.png` và vòng17 `close-pixel.png`. Không nhận đây là rà lại toàn bộ thư viện01–17. Ảnh clay12 là board với khung cảnh/asset/UI; khác biệt với một frame18 còn gồm bố cục và cast, không quy mọi thay đổi cho riêng shader hoặc camera.

Đã mở ba screenshot cuối do critic code tạo trong `tests/results/sprint13-experiment18/`:

- `scene-ribbon-body-1280.png` — capture riêng scene desktop, thấy15controls và artwork đủ gần.
- `scene-one-bit-390.png` — capture riêng scene mobile,15controls xuống hai cột và toàn khung một-bit.
- `gallery-column-forest-1920.png` — full page desktop, thấy ảnh rừng,15card, hai link v1, ghi chú và lịch sử01–17.

Full-page desktop được công cụ hiển thị thu từ1920×5875 xuống1819×5568. Dùng ảnh đó để xem bố cục tổng thể, không đo typography/contrast hoặc khẳng định đã đọc mọi chữ nhỏ. Đã đọc `browser.json` (status passed, Chrome156.0.8078.79,50screenshot) để phân biệt scope kỹ thuật. **Không tự chạy browser automation, không xem toàn bộ50screenshot, không tự thao tác modal/zoom hoặc playtest.** Hash/byte/mapping và controls thuộc phép kiểm critic code; review này không nhận chúng thành thao tác của mình.

## Điều thực sự nhìn thấy trong từng phương án

| Artifact | Quan sát độc lập | Giới hạn còn mở |
|---|---|---|
| `open-yard` — Sân chéo mở | Sân rộng có vùng trống, người mang khay lớn ở gần và người chỉ về chỗ phơi. Khối tường/sàn lớn, marker chân và portrait tóc ngắn cùng đại diện người mang khay. | Còn nhiều khay, hạt phơi, nét áo và sắc độ; không chỉ bốn mặt/ít texture. Chưa xác nhận phép chiếu orthographic hoặc tuổi đúng như brief từ riêng portrait. |
| `three-bands` — Ba dải đường | Cuộc gặp lớn ở lớp gần và một dải đất có hai người nhỏ phía trên; tay chỉ và bó mang trên lưng đọc rõ. | Không thấy đủ ba dải đường liên kết theo một quy ước duy nhất; dải trên còn có thể đọc như thanh đất treo. Không có portrait player riêng. Dáng/đầu thiên hoạt hình tròn, chưa chứng minh range adult xuyên game. |
| `shoulder-craft` — Qua vai người thợ | Lưng/vai, hai tay và mái chèo tạo khung gần, mặt người già hướng dẫn cùng ngón tay chỉ trở thành trọng tâm. | Camera cũng nhìn khá cao xuống sàn, player sát/cắt mép dưới; chưa giữ toàn thân và không có portrait riêng. Nền gỗ/nếp áo còn chi tiết, chưa là cel cực ít giá trị. |
| `long-diagonal` — Đường chéo dài | Cầu/lối xiên chạy xuyên hai mép, khối đá và khoảng tối đỡ nhịp sâu; người có chân đặt, cột chắn và UI cùng cạnh xiên. | Người nhỏ hơn các hướng gặp gần, portrait tóc xám chưa tự chứng minh tuổi già. Chưa đo axon hoặc xác nhận mọi đoạn cầu là navigation hợp lệ. |
| `column-forest` — Rừng là thân cột | Thân/rễ lớn che rìa tạo một khoảng mở quanh việc buộc bó; người quỳ đưa dây, người mang lá và portrait tóc xám đọc được. | Lá/áo/đất vẫn nhiều nét và mảng nhỏ; chưa chỉ vài khối cực gọn. Một frame rừng không chứng minh grammar giữ được qua mọi loại địa bàn. |
| `plan-profile` — Sân phẳng, người nổi | Hai người, khung dệt, chiếu và portrait có cùng họ nét minh họa; cuộc trao đổi rõ. | Cảnh vẫn có nhà/bến/vật trong phối cảnh thông thường, không phải footprint sân90° kèm profile dày nông theo quy ước mới. Đây là khoảng cách intent–output đáng giữ rõ, không gọi đã giải quyết hybrid projection. |
| `cel-adult` — Nét mặt ba sắc | Mắt, mày, mũi, vai và nét tuổi hai người rõ hơn kiểu vô diện; ranh sáng–tối cứng và UI xanh giữ một họ hình. | Chưa xác nhận giới hạn ba giá trị xuyên mọi khối; quần áo, đám đông và vật nền còn nhiều sắc độ/chi tiết. Portrait player lặp ở cả hai nút làm dư nghĩa thay vì một bộ icon phân vai. |
| `soft-mass` — Khối liền mềm | Người già dài, hơi còng khác thân người thấp rộng; tay/cột/bình/ghế/UI cùng dạng bo lớn, mặt người còn biểu cảm. | Vẫn thấy texture vật liệu, nếp/đường nối áo và bóng chuyển mềm; chưa thuần khối liền không vật liệu. Có cảm giác tượng/đất nặn, chưa chứng minh chuyển động hoặc body deformation. |
| `ink-mass` — Mực đặc | Khoảng đen–trắng ở tay/lưng/bó lá và bóng lớn tạo cử chỉ; dấu đỏ ở người/portrait khác bubble NPC. Mặt vẫn đọc được. | Nét lá, giỏ, người nền và bóng nội bộ dày, còn sắc trung gian; không phải hai tông cực tiết chế. Tuổi NPC rõ hơn tuổi player, không suy mọi mặt đã có đủ dải tuổi. |
| `ribbon-body` — Người bằng dải, v2 | Torso/tay/chân là dải rộng có khoảng rỗng và cạnh dày; nền và UI cũng thành dải. Khác cấu trúc body nhìn thấy được so v1. | Mặt đã có mắt/mũi/nét má, nhưng diễn đạt nơi chốn/công trình và chất Văn Lang–Âu Lạc còn ít cue hơn các bản sinh hoạt. Không suy lỗ hở tạo topology/collision hoặc khả năng nhìn xuyên thật. |
| `contour-step` — Đường đồng mức sống, v2 | Các lớp bậc đi qua tóc, thân, tay, giỏ, thuyền, cột, sàn và UI; cạnh/khe tạo thể tích rõ, khác v1. | Cỡ lớp không đều và tỷ lệ gợi tượng ghép/đồ chơi; chưa xác nhận một quy tắc cao độ chung. Portrait lặp ở cả nút Nhật ký, không có icon sách ở nút đó. |
| `square-gesture` — Pixel hình lớn | Cụm vuông lớn và người ở gần; portrait tóc xám có nếp mặt/khối má nên tuổi đọc được hơn mẫu pixel17 đã đối chiếu. Vật mang và UI cùng diện mạo raster. | Vẫn nhiều cụm nhỏ/sắc độ và thêm nhiều icon đồ; không chứng minh palette12màu, grid asset hoặc số pixel chuẩn. Cast/pose cũng đổi so17 nên không quy mức đọc tuổi riêng cho cỡ cụm. |
| `one-bit` — Một-bit có tuổi | Lưng gầy, mặt lõm, mày/trán và bàn tay của người già còn rõ trong đen–trắng; chấm/cụm và UI cùng nhịp. Dấu chân góc vuông khác bubble. | Đây là diện mạo một-bit; metadata ghi PNG RGB. Không đo palette, bit depth hoặc khả năng sản xuất asset từ ảnh này; chưa là renderer/engine một-bit. |
| `single-stage` — Sân khấu một nền | Một sàn chính, người mang vải và người ngồi dệt rất lớn, không giống dải đường ở02; portrait và hành động nằm theo mép sàn. | Vẫn có phối cảnh/đồ phụ, loom và nếp vải nhiều chi tiết. Chưa chứng minh không gian tiếp tục ngoài frame hoặc camera ngang tuyệt đối. |
| `retro-facet` — Khối răng cưa | Các mặt má/tóc/áo/đá có facet lớn; lối lên và ngón tay chỉ rõ, chọn người bằng bracket. | Bước cạnh răng cưa/dither không rõ; còn gần minh họa low-poly có texture. Không có portrait player riêng và không chứng minh renderer/độ phân giải retro. |
| `ribbon-body-v1` — bản đầu giữ lại | Khối người/áo mềm, trao chiếu và UI rõ trong một sân nghề. | Cơ thể còn là người mặc áo thường, không do dải rộng tạo silhouette. Không có bằng chứng user bác bỏ bản này; giữ để thấy vì sao root thử lại. |
| `contour-step-v1` — bản đầu giữ lại | Giỏ, tóc, áo và bến có các nhịp tầng/bo, người mang giỏ và UI đọc được. | Các vòng còn gần nếp áo/vật liệu, chưa thành cấu trúc lớp xuyên thân/vật/nền như v2. V1 tiếp tục là tham khảo, không tự bị loại. |

So sánh trực tiếp với ba ảnh cũ cho thấy delta có căn cứ: soft18 chuyển xa khỏi cảnh vật liệu dày gần tả thực/board của clay12; ribbon18 dùng dải bản rộng và lỗ âm thay thanh tròn/khớp của line16; pixel18 có cụm lớn và nét tuổi khác portrait tóc xám trẻ ở pixel17. Những điều này không xác nhận mọi kỹ thuật18 mới hoàn toàn hoặc tốt hơn bản cũ theo sở thích chủ dự án. Hai cặp v1/v2 cũng đổi bố cục/cast ngoài cấu trúc, không là phép thử chỉ một biến.

## Findings cần mang theo

**P2 — Số phương án không đồng nghĩa số grammar đã đạt.** `plan-profile` chưa thể hiện được phép chiếu chủ ý; `three-bands` chưa rõ ba dải đường; `retro-facet` chưa rõ bước răng cưa/dither. `cel-adult`, `soft-mass` và `ink-mass` còn vượt giới hạn giản lược đã đặt ra. Gallery đã ghi các phần lệch chính, nên không xem chúng là blocker phát hành thư viện ảnh; cần giữ tên ở mức intent và đánh giá lại nếu một hướng được chọn sâu.

**P2 — Vai player/portrait và độ phủ UI chưa đồng đều.** Bracket ở chân khác bubble NPC giúp phân vai trong nhiều frame. Tuy nhiên three-bands, shoulder-craft và retro-facet thiếu portrait riêng; cel và contour lặp portrait ở cả hai nút. Một frame có hai action không chứng minh cùng grammar cho túi đồ/nhật ký/hội thoại và mọi trạng thái. Cần chốt vai/biểu tượng khi đào sâu, không suy avatar lặp là chọn nhân vật hoặc handler đã tồn tại.

**P2 — Một số ngôn ngữ mới đổi hình mạnh nhưng làm giảm cue nơi chốn/tuổi.** Ribbon và contour v2 đã đổi cấu trúc hơn v1, đồng thời đi gần tượng/dải trừu tượng, bối cảnh ít dấu nghề/công trình hơn. Đó là lựa chọn có giá trị để xem nhưng chưa chứng minh bản sắc toàn campaign; selected gray hair cũng không đủ một mình để xác nhận tuổi. Không cần tạo thêm model/render để bàn giao vòng này; một lượt đào sâu được giao sau có thể kiểm bằng cặp frame tĩnh hoặc một tình huống khác.

**P2 — Thu nhỏ vẫn làm mất thông tin raster.** Trên scene mobile390,15nút gallery còn trình bày trong hai cột, nhưng artwork/portrait/chữ nút game nằm trong một ảnh nhỏ. High contrast của một-bit giúp đọc mass, không chứng minh mọi chữ/dấu và thao tác game đọc tốt trên điện thoại. Không OCR từng dấu, đo contrast, audit accessibility hoặc playtest.

**P3 — Chất Việt cổ cần nguồn cụ thể khi chọn phát triển sâu.** Công việc mang khay, làm mái chèo, buộc bó, dệt, mang bình và trao đổi giữ đời sống ở giữa, không bị thay thành chiến đấu/loot. Tuy vậy mái, nhà, nón, phục trang, công cụ và motif trong ảnh chưa được review này xác minh niên đại. Thay grammar bằng dải/lớp/khối không tự tạo game-canon hoặc phục dựng Văn Lang–Âu Lạc; không suy bản sắc từ màu đất hay đồ tre đơn lẻ. Review không bổ sung nghiên cứu khảo cổ mới.

## Bố cục gallery và kết luận

Scene desktop giữ artwork toàn khung,15nút năm cột/ba hàng và caption tách khỏi HUD raster. Scene mobile giữ artwork toàn khung, hai cột nút với mẫu15 ở hàng cuối riêng. Full-page desktop cho thấy15card, hai đường xem bản đầu và lịch sử01–17; không nhận việc nhìn screenshot là đã tự thử bàn phím/modal. Các kết luận tương tác, fingerprint và không tràn ngang thuộc scope critic code.

Không thấy P1 cản chủ dự án xem và đối chiếu15phương án cùng hai bản đầu. Có thay đổi thực về địa bàn, camera, silhouette và cách xây khối, rõ ở rừng/lối xiên, dải/lớp v2 và cụm raster; mức đổi mới/giản lược không đồng đều. Review không tự chọn/loại hướng, không đổi feedback lịch sử hoặc nâng ưu tiên14/1–3 thành final approval.

PNG không kiểm deterministic rules, knowledge isolation, authoritative state, route/physics, geometry nhiều góc, animation, camera runtime, Unity, hiệu năng hoặc human playtest. Không có model/scene3D thật mới được chứng minh trong scope này. **Technical review, quan sát hình và quyết định user là ba lớp riêng.** Mọi output18/S13-A01–A04/Sprint13 vẫn **Pending user approval**; không merge dev, đóngSprint13 hoặc chuyểnSprint14.
