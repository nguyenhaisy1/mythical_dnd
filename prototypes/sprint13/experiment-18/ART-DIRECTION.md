# Thử nghiệm18 — Mười lăm cách dựng một cuộc gặp

02/11/2026 · Sprint13 / SPIN-035 · Worker `/root/s13e18_space` · **Brief trước generation, Pending user approval**.

User giao thêm khoảng15ý tưởng. Root chọn15phương án sau brainstorming của ba worker: sáu gói ưu tiên cách nhìn không gian, sáu gói ưu tiên cấu tạo người/UI và ba gói audit bổ sung range. Mỗi gói vẫn phải có quy tắc cho cả cảnh, thân người, mặt, vật và giao diện; sáu camera không được trở thành cùng một shader đổi góc. Không tuyên bố15kỹ thuật đều chưa từng thử hoặc sẽ đạt brief.

Đây là kế hoạch tạo ảnh mô phỏng, không kết quả quan sát. Root dự kiến15frame mới bằng built-in image_gen, không ảnh input; số lượt/PNG, metadata và những phần lệch brief phải lấy từ output thật sau đó. Không model/scene3D/runtime, Hyper3D/Blender, gameplay hoặc canon mới.

## Nghiên cứu và trao đổi đã có

[Không gian](brainstorm/01-SPACE.md), [nhân vật–UI](brainstorm/02-CAST-UI.md) và [audit lịch sử](brainstorm/03-RANGE-AUDIT.md) giữ đề xuất và actual exchange. Cel, soft và ink có đề xuất trùng giữa cast–audit nên được gộp, không đếm hai lần. Khối liền mềm/hybrid từng nằm trong brainstorm17 mà chưa vào ảnh;18được phép thử tiếp, không diễn giải thành user đã bác bỏ rồi được khôi phục.

Tiền lệ cần giữ đúng: pixel/giấy/dệt/than/retro đều đã có; relief và âm bản17 còn trở lại chiều sâu/vật liệu thường; nhiều ảnh chưa giản lược được như prompt; portrait tóc xám chưa truyền tuổi; NPC vô diện và prop gần bát trong khi label ghi giỏ. Lượt18 đặt quy tắc cấu trúc trước texture và giữ cả giới hạn output nếu tái diễn.

## Quy ước dùng để so sánh batch

- Cast minh họa những cuộc đời được nhận ngẫu nhiên; không cùng một hero, không màn tạo origin hoặc một roster đã chốt. Tuổi/thể trạng đi qua mặt, vai, lưng, tay và cách mang vật, không chỉ tóc xám.
- Portrait thuộc **người đang nhập vai** và khớp tuổi, tóc, áo thấy trong cảnh. Bracket góc vuông ở mặt đặt chân khác bubble bo tròn có đuôi tại NPC nói; không phân vai chỉ bằng màu. Các hình dấu được biến đổi theo phong cách nhưng phải còn khác silhouette.
- Mặt giữ mắt, mày, mũi và vài dấu tuổi ở mức cách điệu; không mặc định bỏ hết nét mặt là giản lược tốt. Props chính có tên/hình khớp hành động, không bát mang label giỏ. Hai hành động ngắn, mặt chữ yên; không thêm mini-map hoặc HUD phụ để lấp chỗ.
- Một cuộc gặp/công việc cục bộ, người đủ lớn, chân có mặt tựa và lối tiếp cận có thể đọc. Chỉ sân, hiên, đường, công xưởng mở hoặc rừng được phép thấy trong tình huống; không x-ray phòng kín hoặc lịch NPC.
- Không cố định board65/35, số ô asset hoặc một HUDHTML chung phủ lên mọi PNG. Mỗi UI raster phải chia sẻ hình/cạnh/khối với cảnh và người; gallery chỉ là vỏ trung tính xem ảnh.
- Màu, vật liệu và bối cảnh dưới là lựa chọn sáng tạo hiện hành. Chúng không chứng nhận lịch sử Văn Lang–Âu Lạc hoặc tự nhập vào canon/content.

## Bộ15 theo thứ tự root chọn

| # / key | Hướng | Trục riêng phải nhìn thấy |
|---|---|---|
|01 `open-yard`|Sân chéo mở|Khoảng âm lớn, ít mái chắn rìa, cự ly người đọc được trong sân nghề|
|02 `three-bands`|Ba dải đường|Ba dải outdoor có che khuất và một đường nối, không nhà cutaway|
|03 `shoulder-craft`|Qua vai người thợ|Vai/tay/vật che một mép và tạo cự ly nhập vai nghề|
|04 `long-diagonal`|Đường chéo dài|Lối xiên cắt hai mép, độ sâu qua khối chắn và dáng người theo hướng lối|
|05 `column-forest`|Rừng là thân cột|Thân/rễ cây lớn dựng framing quanh một việc lao động, không bến sông|
|06 `plan-profile`|Sân phẳng, người nổi|Hybrid chủ động: footprint phẳng, body/prop profile cùng baseline/bóng|
|07 `cel-adult`|Nét mặt ba sắc|Khối adult3D-look với ba vùng sáng/tối và mặt đủ tuổi/cảm xúc|
|08 `soft-mass`|Khối liền mềm|Thân/tay/chân bo liền, không khớp rời/chibi/texture đất nặn|
|09 `ink-mass`|Mực đặc|Silhouette và khoảng rỗng2D quyết định cơ thể/cảnh/UI, không grain than|
|10 `ribbon-body`|Người bằng dải|Dải rộng cấu tạo cả thân/tay/chân, không người thường quấn vải|
|11 `contour-step`|Đường đồng mức sống|Các lớp cao độ có cạnh và che khuất đi xuyên người/vật/công trình/UI|
|12 `square-gesture`|Pixel hình lớn|Cụm vuông lớn tạo dáng/cử chỉ/tuổi, bỏ mật độ props17|
|13 `one-bit`|Một-bit có tuổi|Diện mạo hai sắc, khoảng trắng lớn và dấu tuổi; chưa phải file/runtime1bit|
|14 `single-stage`|Sân khấu một nền|Một floor công cộng nông, người rất lớn; khác02ba lớp depth|
|15 `retro-facet`|Khối răng cưa|Khối facet thô ảnh hưởng silhouette, dither chọn lọc và UI tiết chế|

## 01 — Sân chéo mở

Camera orthographic3/4, sân nghề rộng ở đất liền; mái/cột chỉ chắn vài rìa, không quần thể mái bao kín. Khoảng âm lớn làm người mang vật và người đối diện tách nhau. Hình người có vai/tay lớn, đầu tỷ lệ adult và mặt vài nét rõ; mặt khối màu gọn nối với công trình, đồ chính là một cuộn/vật có silhouette riêng.

UI là hai khối gọn ở mép khoảng âm, portrait cùng mặt cắt/sáng tối và đúng người trên bracket. **Lineage/delta:** học sân/lối14–17 nhưng dùng độ rỗng và mật độ thay panorama. **Cần kiểm:** người/việc có được đọc trước mái/đồ? **Tránh:** một làng isometric đầy props chỉ đổi tên thành sân mở.

## 02 — Ba dải đường

Nhìn ngang ba dải sinh hoạt ngoài trời, mỗi dải khác cao độ/depth và được bờ/lan can gần che một phần; một đoạn dốc công cộng nối chúng. Người ở dải chơi lớn, thân bẹt có cạnh dày nhẹ, mặt/tay vẫn đọc tuổi/việc; người nền chỉ là hoạt động được phép thấy, không nhãn secret state. Vật và mép đường cùng logic hình ngang/cạnh ngắn.

UI dùng một thanh có đầu mút rõ, không ba bảng điều khiển tương ứng ba tầng. **Lineage/delta:** học dải13/cutaway16 nhưng chuyển sang outdoor occlusion liên tục. **Cần kiểm:** khác mẫu14 single-stage của batch18 ở ba lớp sâu, không chỉ chia màn thành ba ô. **Tránh:** platformer ba lane đồng quyền hoặc nhìn xuyên nhà.

## 03 — Qua vai người thợ

Góc gần thấp sau vai; một bàn tay và vật lớn của player che mép gần, người đối diện nhận/đặt là đích nhìn. Một mảng bàn/lối công xưởng mở cho biết mặt tựa; không khung cửa lồng hoặc mạn thuyền làm foreground. Thân/mặt/tay dùng vài mặt cel rõ, có mắt/mày/mũi và dấu tuổi; tay không tả thực riêng trong một cảnh hoạt hình.

UI ở khoảng âm đối diện tay/vật; portrait cùng tóc/áo/tuổi và hai icon có hình vật đúng. **Lineage/delta:** từ near13/POVnghề14, thử vật/khối tay làm occluder thay cảnh nền. **Cần kiểm:** còn thấy người nói và thao tác? **Tránh:** cắt hết fullbody mà không biết đang nhập vai ai, hoặc lặp nested/low-water17 chỉ bằng gần camera.

## 04 — Đường chéo dài

Lối/cầu công cộng xiên mạnh từ gần tới xa và cắt hai mép frame; các khối chắn ít nhưng chia ba lớp, không phải quảng trường giữa nhiều nhà. Camera axon làm mặt lối đọc được. Player adult ở gần có thân nghiêng/trọng tâm theo vật mang; mặt góc, tay và công cụ như vài nêm lớn, không cùng khối mềm của01.

UI là hai mặt nêm hoặc đoạn gãy đặt ngoài vùng đi; chữ đứng thuận, portrait dùng cùng cạnh. **Lineage/delta:** học lối/tầng cũ nhưng trục dài và hướng silhouette phải điều khiển bố cục. **Cần kiểm:** khác01 ở lối cắt frame, không chỉ xoay cùng sân. **Tránh:** nhà khối/đất nâu và một đường chéo trang trí không ảnh hưởng người/UI.

## 05 — Rừng là thân cột

Khối thân/rễ lớn gần che rìa và dẫn tới khoảng mở có việc hái, lựa hoặc buộc bó. Không bến sông, không hàng trăm lá nhỏ; cây có silhouette đặc và khoảng giữa cây là đường nhìn. Người có mặt/tay đặc, dáng cúi/mang bó khác theo tuổi; vật được gom thành khối có công năng, không loot phát sáng.

UI nằm ở khe yên, dùng mảng đặc–rỗng của khối cây; mặt chữ không bị đọc như chữ khắc thật trên thân. **Lineage/delta:** mượn framing17 nhưng chuyển cấu trúc từ cột nhà sang sinh thái và công việc, không chỉ đổi phông. **Cần kiểm:** con người còn trọng tâm giữa cây? **Tránh:** bóng rối10 recolor, photoreal foliage hoặc thêm ma thuật/hidden truth để làm lạ.

## 06 — Sân phẳng, người nổi

Footprint sân/đường là2D phẳng; body và useful props là profile dày nông dựng trên cùng baseline, cùng hướng nghiêng/bóng. Hai người được đọc qua vai, mũi, lưng và vật; không chỉ một sprite frontview dán lên map90°. Thế giới công cộng gọn và một lối nối đủ rõ, không map toàn settlement.

UI dùng thẻ profile/mặt yên cùng họ, portrait thuộc selected player; các prop liên quan phải theo cùng phép nhìn. **Lineage/delta:** hybrid từng brainstorm17 chưa tạo ảnh, nay thử thành quy ước xuyên body/prop/UI. **Cần kiểm:** chân đặt và occlusion có đọc được? **Tránh:** gọi đây là pure90° đã sửa river17, hoặc quân cờ/map quản lý có thông tin toàn tri.

## 07 — Nét mặt ba sắc

Khối3D-look của cơ thể chỉ có khoảng ba mảng sáng/tối lớn, ranh màu cứng, không skin shading hoặc drapery nhỏ. Camera đủ gần để mắt/mày/mũi và vài nét khóe mặt truyền tuổi/cảm xúc; người trưởng thành có đầu nhỏ tương đối, vai/lưng/khối tay khác nhau. Công trình/vật cũng cùng phân vùng ánh sáng gọn, không background tả thực.

Portrait dùng cùng tuổi và ba mảng, action chip cạnh rõ trên nền phẳng. **Lineage/delta:** học anime11 và faceted17 nhưng giữ adult/tuổi và bỏ skin detail. **Cần kiểm:** body và portrait nói cùng một tuổi/người? **Tránh:** tóc xám trên gương mặt thiếu niên, anime-eye formula hoặc chỉ poster khuôn mặt đẹp.

## 08 — Khối liền mềm

Torso/tay/chân là vài khối bo liền, không rã thành khớp/rod; tỷ lệ adult với đầu nhỏ, bàn tay lớn và khác trọng tâm theo tuổi. Mặt là khối đặc có mắt/mày/sống mũi/vết má, không trơn vô diện. Đồ hữu dụng và mái/lối là khối mềm nối nhau bằng che khuất, không texture đất nặn/dấu tay/nhựa bóng.

UI gồm hai thùy lớn có mặt chữ yên, portrait cùng mặt bo và dấu tuổi. **Lineage/delta:** nhánh brainstorm17 chưa vào ảnh, khác clay12 ở cấu trúc liền/ít vật liệu và khác rod16 ở không khớp. **Cần kiểm:** còn trưởng thành thay vì baby chibi? **Tránh:** mọi cơ thể cùng avatar đầu lớn, hoặc khối mềm chỉ là shader bề mặt.

## 09 — Mực đặc

2D silhouette đặc; khoảng rỗng của khuỷu/lưng/đồ mang tham gia tạo cử chỉ. Mặt và vật chính có vài cue tương phản đen–trắng, không bỏ tuổi bằng cách làm mọi người thành bóng như nhau. Cảnh dùng ít mảng lớn có đường đứng/chân rõ; không charcoal grain, vệt cọ vụn hoặc chỉ tô tối làng thường.

UI âm/dương cùng nhịp đặc–rỗng, chữ đứng thuận, icon có silhouette khác nhau. **Lineage/delta:** có tiền lệ puppet10/charcoal12/flat17, giờ bắt khoảng rỗng cấu tạo body/prop thay texture. **Cần kiểm:** nhìn small still thấy người/vật và vai nói? **Tránh:** shadow-puppet recolor hoặc mắt/mặt mất hết vì tương phản tuyệt đối.

## 10 — Người bằng dải

Dải rộng trực tiếp cấu tạo torso, tay và chân, có chỗ đè lên/đi dưới rõ; không một người thường quấn vải hoặc dán textile texture. Mặt đặc vài nét vẫn có tuổi; bàn tay/prop phải đủ đặc để đọc việc. Cảnh là vài dải đường/sàn/mép công trình cùng độ rộng có chủ ý, không hàng nghìn sợi dệt.

UI là đầu dải trơn và portrait dùng cùng nhịp che khuất nhưng giữ mặt đọc được. **Lineage/delta:** học dệt08/16 và gấp16 nhưng mục tiêu là body/occlusion, không palette vải. **Cần kiểm:** silhouette người thay đổi thật? **Tránh:** băng quấn xác ướp, dây phép hoặc chỉ một họ texture phủ cảnh cũ.

## 11 — Đường đồng mức sống

Khoảng ba đến năm lớp cao độ có cạnh và che khuất tạo người/công trình/prop, cùng nhịp bậc xuyên silhouette. Góc xiên đủ thấy mặt lớp; mặt người vẫn là khối đặc có mắt/mày, dấu tuổi và mũi. Không dùng đường contour trang trí trên cơ thể tả thực hoặc đường scan nhìn xuyên vật.

UI/portrait là lớp có cạnh cùng cỡ bậc, chữ ở một mặt yên. **Lineage/delta:** từ gấp/voxel16 nhưng lớp thay lưới cube; đây không chỉ map địa hình. **Cần kiểm:** step nhìn được trên body và icon hay chỉ nền? **Tránh:** voxel trá hình, đường đồng mức thành dấu tri thức bí mật hoặc đường đi authoritative.

## 12 — Pixel hình lớn

Pixel2D màu có cụm vuông lớn tạo cơ thể/prop và silhouette thao tác, camera đủ gần. Tuổi cần posture, mặt và tay cùng hỗ trợ, không chỉ tóc xám; props giới hạn còn một vật chính và vài khối sân/xưởng. Không vô số cụm lá/sợi/đá nhỏ như mật độ pixel17, không3D voxel.

Portrait/icon có cùng nhịp pixel và tuổi, border UI pixel nhưng text Việt có thể dùng chữ rõ giữ dấu. **Lineage/delta:** tiếp pixel07/11/17 về hình lớn/mật độ và tuổi, không pixel là phát minh mới. **Cần kiểm:** gesture đọc ở cự ly chơi? **Tránh:** filter ảnh phóng, grid pixel không đều bị gọi production-ready hoặc mặt trẻ gắn tóc xám.

## 13 — Một-bit có tuổi

Diện mạo hai sắc và khoảng trắng lớn; stipple/dither chỉ tại vài vùng chuyển thay phủ mọi pixel thành xám. Silhouette và lỗ âm tạo khuỷu/lưng/vật, mặt có vài nét mắt/mày/mũi/khóe đủ gợi tuổi. Cảnh gọn, một floor/lối công cộng và ít đồ; body có tỷ lệ adult rõ.

UI khung âm/dương, portrait hai sắc cùng tuổi, hai label trên mặt trống. **Lineage/delta:** học charcoal12/pixel11/17 nhưng tổ chức bằng hai mức thị giác thay grain hoặc monochrome filter. **Cần kiểm:** tương phản không xóa mặt/tay? **Tránh:** nhiều grayscale gradient hoặc gọi PNGRGB/ảnh preview là file1bit/runtime1bit thật; metadata lấy từ output.

## 14 — Sân khấu một nền

Một floor công cộng nông nhìn gần ngang, hai người rất lớn và một thao tác trao/đặt làm trung tâm. Không ba dải depth, tầng nhà cutaway hoặc toàn settlement; khoảng đứng và lối vào/ra ở rìa vẫn hiện. Thân, tay và mặt có phóng đại khác theo tuổi; props như khối sân khấu cô đọng chứ không đồ ảnh thật.

UI tận dụng một khoảng âm rìa, portrait/icon cùng nhịp hình body, không danh sách điều khiển tất cả người. **Lineage/delta:** học side-on16/gesture17 nhưng giảm còn một nền và cự ly rất gần. **Cần kiểm:** còn cảm giác nhập vai trong nơi chốn, không chỉ hai sprite đối thoại trên phông? **Tránh:** trùng02Ba dải đường hoặc trang visual novel chỉ có mặt cắt đầu.

## 15 — Khối răng cưa

Geometry gợi3D thô với bước cạnh nhìn thấy ở silhouette, mặt phẳng lớn ở má/tóc/tay và nhà/đồ. Dither chọn lọc, màu có local role, không DOF/bloom hoặc texture phủ lên mesh mịn; tuổi đi qua khuôn mặt góc và dáng lưng. Camera3/4 giữ chân và vật trên cùng mặt tựa, không chỉ panorama retro.

UI gọn với portrait cùng mặt facet, icon ít mặt và hai action rõ, chữ Việt giữ dấu. **Lineage/delta:** học retro-prerender12/tangram17 nhưng nhấn bước cạnh/kiểm soát mật độ, không one-bit đổi màu. **Cần kiểm:** độ thô ảnh hưởng silhouette thật? **Tránh:** photoreal nền rồi posterize, voxel16 hoặc khẳng định resolution/polygon count/runtime khi chưa có model.

## Nguồn văn hóa và cổng quan sát

[Workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [hồ sơ văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md) là nền phân biệt hiện vật, diễn giải và hư cấu; brief này không bổ sung khảo cổ mới. Sân nghề, đường, rừng, hiên/xưởng mở và bến là đề xuất staging để đưa đời sống vào hình, không xác minh kiến trúc/costume/công cụ theo niên đại.

Reviewer phải mở output thật, nhận phần lệch brief và giữ tên phương pháp ở mức intent. Một frame không kiểm toàn game, mọi roster/UIstate, animation hoặc knowledge isolation. Camera/bands/hybrid/steps không tự đổi route, physics, thời tiết, x-ray hoặc tri thức. Technical review, quan sát hình và quyết định user là ba lớp riêng. Giữ01–17/biến thể/ưu tiên14/1–3; mọi output18/Sprint13 vẫn **Pending user approval**, không tự merge dev, đóngSprint13 hoặc chuyểnSprint14.
