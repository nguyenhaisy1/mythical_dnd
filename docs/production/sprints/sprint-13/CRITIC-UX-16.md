# Phản biện hình ảnh16 — cách dựng thế giới, người và giao diện

02/11/2026 · Sprint13 / SPIN-033 · Reviewer độc lập `/root/s13e16_critic_visual` · **Visual reviewed trong phạm vi đã xem; Pending user approval**.

## Phạm vi quan sát thực

Đã mở trực tiếp đủ tám PNG vòng16: sáu ảnh trưng bày `woven-world`, `line-volume`, `folded-village`, `rain-sculpt`, `living-section`, `voxel-life` và hai bản đầu `line-volume-v1`, `folded-village-v1`. Đã mở thêm ba ảnh cũ để đối chiếu cụ thể: vòng08 `b-woven.png`, vòng07 `c-papercut.png`, vòng11 `pixel-life.png`. Đọc brief ART-DIRECTION, ghi chú quan sát gallery, baseline và hướng dẫn mythical-validation; không lấy mô tả prompt làm bằng chứng output đã đạt.

Đã mở ba screenshot cuối do critic code tạo trong `tests/results/sprint13-experiment16/`:

- `gallery-folded-village-1920.png` — desktop, hướng gấp được chọn.
- `gallery-rain-sculpt-1280.png` — desktop, hướng mưa được chọn.
- `text150-390.png` — mobile, HTML150%, hướng dệt đang chọn.

Screenshot mobile dài được công cụ hiển thị thu từ390×11387 xuống205×6000; dùng để quan sát bố cục tổng thể, không đo kích thước chữ hoặc khẳng định từng chữ nhỏ đọc tốt. Đã đọc `browser.json` để phân biệt bằng chứng kỹ thuật của critic code; **không tự chạy browser automation, không xem toàn bộ21 screenshot, không tự thao tác modal/zoom**. Hash/byte/prompt mapping và tương tác gallery thuộc scope critic code, không được nhận thành thao tác của visual reviewer. Không sửa ảnh/source hoặc tạo model.

## Điều khác biệt thực sự nhìn thấy

| Artifact | Quan sát độc lập | Phần chưa đạt hoặc chưa biết |
|---|---|---|
| Thế giới dệt | Địa bàn nhìn gần mặt bằng, các dải đất vàng và bến ngà tạo nhịp thẳng/giao nhau trên nước indigo. Thuyền nhìn từ trên, icon thuyền và giỏ nối với hai hành động. Người tóc trắng có vòng chọn dưới chân và portrait cùng nhóm màu. | Thân người vẫn là hình vẽ mặc áo có viền; người chèo được nhìn xiên hơn thuyền/mặt bằng. Dải đan quyết định nhiều hơn texture nhưng chưa xuyên toàn bộ cơ thể/portrait. Chưa chứng minh một phép chiếu nhất quán hoặc walk graph. |
| Nét dây không gian — v2 | Tay chân/cột/khung thuyền và UI có tiết diện thanh tròn, các mảng áo đỏ và mặt ngà làm điểm đặc. Có độ dày, che khuất và silhouette khác v1; cuộc trao ván còn rõ. | Hình người gợi tượng khớp hoặc con rối, nhà/nền vẫn đầy và có texture. Khối dây chưa biến cả thế giới thành mạng nét/khoảng rỗng; không gọi đây là wireframe renderer hoặc scene dựng thật. Dấu chọn player tại chân như brief chưa thấy rõ. |
| Làng gấp — v2 | Mặt sàn/vách lớn, ramp thẳng, thuyền và người cùng mặt góc cạnh; ít chi tiết vụn hơn v1. Portrait bà tóc xám và hai lá UI có nếp giữa nối được ngôn ngữ mặt phẳng. | Chưa buộc đọc là một dải đàn xếp liên tục; cũng có thể là các khối/plane kiến trúc tối giản. Hai dấu kim cương gần giống nhau, chỉ khác màu. Portrait thuộc bà già nhưng chưa có ngữ nghĩa nhất quán đây là người nói hay người đang nhập vai. |
| Mưa khắc hình | Lưng người chiếm tiền cảnh, người già ở hiên thành điểm gặp, mặt tay và patch sàn sáng giúp đọc cử chỉ. Khối góc cạnh, dấu chọn vàng rỗng, bubble trắng và UI chéo đen–vàng có thứ bậc riêng. | Có nhiều vệt mưa và phản chiếu vật liệu; chưa là ba lớp khoảng âm cô đọng như brief. Các mặt áo đen nhập nhau khi thu nhỏ. Đây là lựa chọn staging/camera tĩnh, chưa chứng minh visibility/weather hoặc độ đọc trong chuyển động. |
| Mặt cắt đời sống | Hai tầng sinh hoạt, cầu thang trung tâm và bến phía dưới tạo một cách đọc ngang rõ. Người trẻ mang cuộn vải đối diện bà dệt; người dưới làm giỏ riêng. Vòng portrait và dải hành động nằm dọc mép sàn, không che cầu thang trong ảnh. | Hình người/vật liệu vẫn nhiều nét chi tiết, chưa là khối chân tay thô như brief. Có chiều sâu nghiêng ở sàn/thuyền, không xác nhận orthographic tuyệt đối. Một ảnh không chứng minh navigation hoặc quyền truy cập mọi tầng; phòng có cửa đóng không trở thành hidden truth được phép xem. |
| Hạt khối | Thể tích lập phương có mặt trước/trên/bên nhìn thấy ở mặt người, tóc, thân, cây, bờ và thuyền; khác rõ một lớp pixel hóa phẳng. Hai người già/trưởng thành và việc cùng nâng giỏ vẫn nhận ra; player có ngoặc trắng ở chân, NPC có bubble. | Mật độ hạt còn dày và đơn vị không đều giữa mặt/giỏ/kiến trúc. Một số icon/UI là mặt phẳng mượt hơn cảnh; mini-map và các nút chức năng chưa có ngữ nghĩa hoặc dữ liệu runtime để kiểm. Không xác nhận voxel engine, asset grid hoặc phá/xây block. |
| Nét dây v1 — giữ riêng | Cuộc trao ván, người nói, icon và portrait khá rõ; nét đen đồng nhất quanh hình. | Đây còn là tranh có viền, không phải thể tích cấu tạo bằng thanh. So với v2, khác biệt cấu trúc có thật; v1 tiếp tục là bản tham khảo, không phải ảnh user bác bỏ. |
| Làng gấp v1 — giữ riêng | Tầng đất, sân chiếu và lối lên nhà cùng có chiều sâu; giao diện gấp hai lá. | Địa hình vẫn là các bậc đất thường, nhiều texture/đồ vật; chưa thành nếp đàn xếp. v2 đã giản lược và đổi tỷ lệ mặt mạnh hơn, nhưng không suy thành cùng geometry/cast hoặc một bản bị loại bởi user. |

Đối chiếu ảnh cũ xác nhận khác biệt có giới hạn rõ. Dệt08 là cả cảnh phối cảnh được cấu tạo bằng hạt/sợi với nhiều chất liệu; dệt16 đổi sang mặt bằng và dải lớn, không chỉ thay màu. Giấy07 có nhiều lớp silhouette/cảnh đêm; gấp16v2 đổi sang các mặt lớn có ramp và người đa diện, không đơn thuần lặp giấy cắt. Pixel11 dùng trường cảnh/UI/sprite theo raster nhỏ; voxel16 cho thấy bước khối ở silhouette và độ sâu. Các so sánh này không chứng minh mọi yếu tố16 mới hoàn toàn hoặc tốt hơn những bản cũ.

## Findings còn mở

**P2 — Một số tên phương pháp rộng hơn điều output đã chứng minh.** Dệt chưa xuyên hình người, nét dây chưa dựng cả thế giới bằng khoảng rỗng, gấp chưa thành một dải liên tục, mưa chưa đạt phép chia ba lớp âm. Gallery đã ghi các khoảng cách này; không có lý do biến chúng thành blocker xem thư viện hoặc ép thêm model/render. Khi giới thiệu cần giữ cả ý định và điều thực sự nhìn thấy, không đếm sáu tên thành sáu đột phá.

**P2 — Ngữ nghĩa người đang nhập vai/người nói chưa thống nhất qua hướng.** Foldv2 đặt cùng một họ dấu kim cương ở hai người, khác màu; portrait bà già có thể là người nói trong khi nhiều bản khác dùng portrait player. Linev2 có một ô đỏ cạnh portrait nhưng thiếu dấu chọn trực tiếp trên người như brief. Woven/rain/section/voxel có dấu chọn và bubble khác hình dễ phân biệt hơn. Chưa có legend hoặc trạng thái khác để chứng minh hệ ký hiệu xuyên game. Nếu một hướng được đào sâu, cần quyết định portrait thuộc vai nào và dùng hình/dấu phụ ngoài màu để phân biệt player/target/speaker; đây là câu hỏi thiết kế UI raster, chưa là bug handler runtime.

**P2 — Thu nhỏ làm mất thông tin game dù trang xem vẫn sắp được.** Trên screenshot mobile150, cả cảnh giữ trọn nhưng chân dung, bubble và nhãn trong raster rất nhỏ. Ở rain, mặt áo đen nhập nhau; ở voxel, hạt và nhiều icon nhỏ cạnh tranh chú ý. Điều này không cản mở ảnh gốc để chọn art direction, nhưng không được báo UI game đã đọc tốt trên điện thoại. Chưa OCR từng dấu, đo contrast hoặc thử thao tác gameplay.

**P3 — Độ phủ toàn game mới ở mức tình huống minh họa.** Từng frame đã có thế giới/người/vật/portrait/UI cùng họ hình ở mức khác nhau, song chưa có full roster, nhiều vùng, animation hoặc các trạng thái túi đồ/nhật ký/hội thoại để xác nhận hệ thống đủ cho toàn campaign. Không cần ép thêm bảng asset cố định để bàn giao lượt tìm ý tưởng này. Cử chỉ đời sống vẫn hiện diện qua trao ván, mang giỏ, cuộn vải và nghề dệt; không bị thay bằng bảng chọn anh hùng.

**P3 — Chất Văn Lang–Âu Lạc vẫn cần nguồn khi chọn phát triển sâu.** Nhà sàn, thuyền, giỏ, chiếu/vải và hoạt động cộng đồng cho thấy một đề xuất xã hội sông nước. Chúng không tự xác nhận niên đại mái, nón, phục trang, công cụ dệt hoặc cách đóng thuyền trong ảnh. Review không có khảo cổ/hiện vật mới; các hình dây/gấp/voxel là hư cấu mỹ thuật, không game-canon hay phục dựng được chứng nhận.

## Bố cục gallery đã quan sát và kết luận

Hai screenshot desktop giữ artwork toàn khung, các nút gallery tách khỏi HUD raster và vùng ghi chú output nằm dưới card. Hai link bản đầu giữ được đường xem lại, thư viện01–15 vẫn hiện. Screenshot mobile150 xuống một cột ở phần card, switches hai cột và phần pending/lịch sử còn được trình bày; việc kiểm không tràn ngang và nút hoạt động thuộc automation của critic code, không phải tương tác do visual reviewer tự thực hiện.

Không thấy P1 cản chủ dự án xem và so sánh sáu hướng cùng hai bản đầu. Có thay đổi về không gian/camera/silhouette thực sự nhìn thấy, đặc biệt mặt bằng dải, thanh có thể tích, mặt lớn tối giản, cách đọc ngang và voxel; mức mới/độ giản lược không đồng đều. Không tự chọn/loại hướng hoặc gọi chúng đạt art direction cuối.

PNG không kiểm được deterministic rules, knowledge isolation, authoritative state, geometry nhiều góc, route, vật lý, UI runtime, Unity, hiệu năng hay human playtest. Mini-map/cutaway/khối rỗng trong ảnh không là chứng nhận content projection an toàn. Không có model hoặc scene3D thật mới trong review này. Ba ưu tiên14/1–3 vẫn là ưu tiên đào sâu; toàn bộ01–16 và các bản đầu tiếp tục giữ để tham khảo. **Technical review, quan sát thị giác và quyết định user là ba lớp riêng; mọi output16/Sprint13 vẫn Pending user approval**, không merge dev, đóngSprint13 hoặc chuyểnSprint14.
