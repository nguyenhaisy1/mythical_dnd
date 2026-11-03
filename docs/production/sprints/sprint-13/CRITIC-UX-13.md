# Phản biện hình ảnh13 — khung chơi và cuộc gặp sau workshop

30/10/2026 · Sprint13 / SPIN-030 · Reviewer độc lập `/root/s13e13_critic_visual` · **Review hình ảnh/UX minh họa, Pending user approval**.

Vòng13 đã đổi cách trình bày thành ba hướng × hai trạng thái thế giới/đối thoại. Hai hướng Khối Lạc và Trong lòng làng cùng họ tạo hình3D cách điệu, kiểm cự ly/camera; Dải đời sống kiểm cấu trúc hình phẳng theo dải. Không gọi đây là sáu phong cách độc lập hoặc một scene3D đã chạy. Review này không sinh/sửa ảnh, tạo model, thao tác Git hoặc chốt Sprint13.

## Phạm vi thực sự đã xem

Đã đọc `.agent`, HANDOFF, baseline, skill mythical-validation/game-design-review, workshop `ART-DIRECTION-RETHINK.md`, brief13, prompts và provenance. Đã dùng `view_image` mở trực tiếp cả bảy PNG trong `prototypes/sprint13/experiment-13/assets/`:

- `block-world.png`, `block-dialogue.png`.
- `near-world.png`, `near-dialogue.png`.
- `frieze-world-v1.png`, `frieze-world.png`, `frieze-dialogue.png`.

Đã mở thêm hai original đối chiếu: `prototypes/sprint13/experiment-11/assets/stylized-3d.png` và `prototypes/sprint13/experiment-06/assets/c-sideview.png`. Không tuyên bố đã xem lại toàn bộ01–12 hoặc nhận lượt xem của worker/root thành của reviewer. Các đường dẫn/kích thước/hash trong provenance phục vụ truy vết; review hình này không thay kiểm byte equality và browser của code critic.

Sau khi code critic báo lượt cuối hoàn tất trên source đã freeze, reviewer đã thực sự mở thêm ba screenshot: `tests/results/sprint13-experiment13/gallery-near-dialogue-1920.png`, `gallery-frieze-dialogue-390.png` và `text150-390.png`. Reviewer không tự chạy browser hoặc nhận automation của code critic thành lượt kiểm tương tác của mình. Các screenshot cho thấy ảnh giữ trọn khung, ba cặp và đủ sáu thumbnail hiện rõ; phần chú thích quan sát và link v1 có mặt. Bố cục mobile/HTML150 xếp dọc, chữ của gallery còn đọc được; UI raster thu rất nhỏ và không tăng cùng cỡ chữ HTML. Việc mở PNG gốc giúp xem concept, không thay kiểm cỡ chữ/icon game thật hoặc accessibility. Không đo contrast và không audit mọi thiết bị.

## Đầu ra đã nhìn thấy

| Hướng | Khung thế giới | Khung đối thoại và mức thống nhất |
|---|---|---|
| Khối Lạc | Người, thân thuyền, cây và đồ dùng có mặt khối rõ hơn bản11 tả thực. Đường đi sáng từ tiền cảnh tới nhà sàn, khoảng âm dưới sàn và người thợ cạnh thuyền đọc được; prompt Hỏi chuyện chỉ đúng người có thể tiếp cận. Không còn dải asset dưới cảnh. | Hai người quay vào nhau, người thợ đưa tay; portrait tóc/râu sáng và áo kem khớp vai đang nói. Có tên Thợ thuyền, câu Ta cần thay thanh gỗ này và ba lựa chọn tách nhau. Đối thoại và world giữ được màu áo, người, thuyền/nhà cùng họ hình; chưa là identity/geometry matching tuyệt đối. |
| Dải đời sống | V1 còn phối cảnh sâu, nhiều vệt vật liệu và các mái/cầu xiên. Bản hiện hành làm mái, sàn, đường và nước thành các dải ngang rõ; khoảng âm/cột và các nhóm người tạo nhịp. Người chính, thợ và porter vẫn nhận được qua áo, vật mang và dáng. | Người chính/người thợ lớn hơn, xoay về nhau ở cùng dải sàn. Portrait thợ, speaker, câu nói và lựa chọn đầy đủ; icon/khung UI đi cùng màu tối/kem/đỏ của cảnh. Cách vẽ phẳng được giữ rõ qua cặp ảnh, nhưng vẫn có gradient ở mái, nền và UI; chưa đạt đúng toàn bộ quy tắc cực phẳng/không chuyển sắc của prompt sửa. |
| Trong lòng làng | Người chính hiện lớn và gần hơn, thấy lưng, chân và đường tới thợ. Họ hình/cast/màu bám Khối Lạc, giúp đọc đây là nhánh camera của cùng ý tưởng. Tuy vậy, vẫn thấy nhiều mặt trên của thuyền/sàn và toàn cảnh từ cao; chưa thể hiện rõ camera ở tầm lưng như brief. | Có tiến gần thật về mặt bố cục: mặt/tay người thợ và vai/lưng người chính chiếm phần lớn cảnh. Mặt tóc/râu sáng, bàn tay, thân thuyền và cận portrait giữ cùng cách tạo khối. Có speaker, câu nói và ba lựa chọn; panel không che mặt người thợ. Khung này trả lời câu hỏi cự ly gần rõ hơn near-world. |

Ở cả ba dialogue, có thể nhận được người đang nói, câu minh họa và vùng lựa chọn riêng, khác việc chỉ đặt một action menu không có lời nói. Reviewer đọc được các nhãn chính ở original; chưa làm OCR từng dấu tiếng Việt hoặc kiểm font/contrast theo tiêu chuẩn UI runtime. Không xác nhận nút raster có tương tác hoặc thực hiện một hành động repair.

## Khác biệt đã có so với các bản đối chiếu

So với11/stylized-3d, bản13 dùng mặt khối, màu áo/tán cây/đất có mảng lớn hơn, nhân vật cách điệu hơn và dành trọn khung cho cảnh chơi; không dựa vào một dải mặt/người/icon/journal để tạo cảm giác đủ toàn game. UI gọn hơn và tình huống tiếp cận/thợ đang làm việc rõ hơn. Chưa có bằng chứng bộ asset hoặc mô hình đã được tạo.

So với06C, Dải đời sống cuối thay cấu trúc nhìn: độ sâu panorama được nén, mái/sàn/nước thành dải, hình người và cây giản lược; đây không chỉ là cùng tranh nhìn ngang với palette khác. Việc giữ v1 cho phép thấy chính lượt làm phẳng đã đổi cấu trúc gì. V1 vẫn là biến thể tham khảo, không bị user từ chối hoặc tự coi là ý tưởng dở.

Những cải thiện trên là quan sát có phạm vi. Không suy chúng thành chủ dự án đã hài lòng, vibe đã được duyệt hoặc gameplay đã dễ hiểu khi vận hành.

## Findings và câu hỏi còn mở

### P2 — near-world chưa kiểm được camera gần tầm người như đặt bài

Vị trí: `near-world.png`, đối chiếu `block-world.png` và prompt near-world. Người chính gần/lớn hơn nhưng góc nhìn vẫn khá cao, mặt trên của nhiều cấu trúc và toàn cảnh hậu cảnh còn giữ mạnh. Vì vậy đây mới là một biến thể cự ly/bố cục của họ khối, chưa đủ chứng minh camera ở tầm lưng có lợi gì hoặc cho cảm giác đứng trong làng như ý định. `near-dialogue.png` thể hiện rõ sự tiến gần ở mặt/tay; không dùng nó thay bằng chứng cho exploration camera.

Gallery/root notes đã nêu giới hạn này. Khi đào sâu sau phản hồi, nên tách câu hỏi hạ cao độ camera khỏi câu hỏi phóng lớn người; vẫn có thể thử bằng ảnh, không cần model thật làm điều kiện. Không yêu cầu tạo lại ảnh ngay để được bàn giao thư viện.

### P2 — mức giản lược và tỷ lệ chưa đạt đều trên toàn khung

Vị trí: các ảnh họ khối, rõ ở mái/tán cây/đường/nền hậu cảnh; frieze cuối và dialogue rõ ở chuyển sắc. Người trong block-world lớn hơn chủ ý khoảng8–10% chiều cao ảnh của prompt; không có phép đo pixel ở review này nên không gán một tỷ lệ thực chính xác. Các thân người cũng chưa đồng đều ở ý định khoảng bốn nhịp đầu. Cần mô tả chúng là hình cách điệu đã xuất hiện, không là brief cực giản đã đạt100%.

Mái, khối cây, sàn và đồ vật trong họ3D vẫn nhiều mặt/chi tiết nhỏ hơn ý định vài mảng lớn. Dải đời sống đã giảm cấu trúc rõ nhưng vẫn dùng gradient/nhịp chi tiết ở đồ/áo/UI; không gọi nó là ảnh chỉ có đúng mười hai màu hoặc không texture nếu chưa kiểm. Đây là độ khớp brief, không là kết luận phải bỏ hướng hoặc trở về tả thực.

### P2 — UI cùng họ, chưa thành một hệ thống trạng thái thống nhất

Vị trí: ba dialogue. Cả ba dùng nền tối/kem và nhấn đỏ, nhưng kiểu chọn đầu khác nhau: Khối Lạc dùng nền kem và vòng đỏ; Dải đời sống dùng nền kem và chấm đỏ; Trong lòng làng dùng nền đỏ cùng mũi chỉ. Biểu tượng Giúp một tay trong hai bản đầu giống lời thoại, ở bản cận là bàn tay. Chưa thể dùng các ảnh làm design system hoặc asset contract đã chốt.

Trong giai đoạn ý tưởng, khác biệt này giúp thử hình thức UI. Khi chọn một họ để phát triển, cần nối icon, selected/focus/disabled và speaker thành quy tắc rõ ở UI thật; chưa yêu cầu các concept phải giống pixel hoàn toàn. Nhật ký/Túi đồ hiện mới là tab/icon trên cảnh, chưa phải nội dung hai màn hình đó đã được thiết kế hoặc kiểm.

### P3 / câu hỏi tạo hình — tuổi và phạm vi cast chưa được chứng minh đầy đủ

Áo đỏ/chàm, headband, tóc và satchel giúp nhận người nữ qua các cặp; người thợ già có tóc/râu, dáng và nét mặt khác. Tuy nhiên, dấu tóc bạc và tuổi trung niên của người nữ trong brief chưa rõ nhất quán; portrait nhỏ và gương mặt cô đọng có thể được đọc trẻ hơn. Không kết luận phải thêm nếp da tả thực. Nếu cần kiểm độ phủ nhiều cuộc đời sau này, nên thử tuổi/dáng/tư thế có chủ ý trong cùng grammar, thay vì coi một cặp cast đã chứng minh toàn roster.

### Câu hỏi văn hóa — đã có phép biến đổi, chưa có xác nhận tính riêng toàn game

Mái–sàn–cột, thân thuyền, động tác nghề và tổ chức nhóm đã hiện thành hình nhìn được. Tính cách điệu sống động và màu có vai trò khác cảm giác ảnh tư liệu tả thực. Tuy vậy, các dấu hiệu này còn rộng, chưa tự chứng minh người xem nhận đúng Văn Lang–Âu Lạc hoặc toàn bộ phục trang/nhà/công cụ có căn cứ chính xác. Motif trên cờ, thuyền, satchel và các trang trí trong ảnh là sáng tạo đề xuất, không được nhập ngầm thành khảo cổ hay canon. Review này không làm nghiên cứu lịch sử mới.

## Ranh giới kết luận

Không phát hiện **P1 cản việc xem và so sánh các concept** trong những original và screenshot desktop/mobile cuối đã quan sát. Ba cặp đủ để thảo luận sự thay đổi về cấu trúc hình, khoảng tiếp cận và hình thức đối thoại; chưa chứng minh art direction đã hoàn chỉnh hoặc tốt hơn theo sở thích của chủ dự án.

Ảnh tham chiếu giúp giữ họ tạo hình, không chứng minh cùng geometry, camera hoặc một nhân vật3D xuyên các view. Không có scene/model/mesh/rig mới, camera tự do, parallax, animation, UI runtime, benchmark, chi phí production hoặc human playtest trong review. Góc nhìn này không thay tri thức/canon/state hoặc baseline xuất thân ngẫu nhiên. Tất cả01–13 và biến thể vẫn là thư viện tham khảo **Pending user approval**; không tự đóng Sprint13, merge dev hoặc chuyển Sprint14.
