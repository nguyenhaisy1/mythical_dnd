# Thử nghiệm 02 — Bến sống trong sương

**Sprint 13 / SPIN-017 · Pending user approval.** Người dùng chưa hài lòng đồ họa Thử nghiệm 01; bản này thử một hướng khác về chất liệu, chiều sâu và cách dành không gian cho cảnh. AI `/root/s13e2_art` đề xuất hướng; `/root` tạo ảnh và tổng hợp; `/root/s13e2_ui` tích hợp giao diện. Technical review không phải nghiệm thu mỹ thuật.

## Giả thuyết cần xem bằng sản phẩm

Một bến sông có chất vẽ tay, người lao động và vật liệu nhìn rõ sẽ gợi cảm giác đang bước vào một cuộc đời hơn minh họa mảng phẳng đi cùng nhiều khung thông tin. So sánh cùng tình huống Bến Vạc và chiếc thuyền; không đổi nội dung gameplay để khiến phương án mới có vẻ tốt hơn.

Kết quả cần nhìn được là cảnh sửa thuyền và những hoạt động thường ngày. Không lấy cung điện, vũ khí, hiệu ứng phép hoặc nhân vật anh hùng làm điểm hút mắt. Người dùng đánh giá bản sắc, mức độ muốn bước vào cảnh và khả năng đọc hành động; chưa có phản hồi thì giữ các kết luận này là chưa biết.

## Bố cục và chất liệu

- **Camera:** phối cảnh nghiêng từ bến, hơi cao hơn tầm người; nhìn được sàn gỗ và mặt nước. Đây là camera concept, chưa chốt camera runtime, tỷ lệ mét hay navigation.
- **Ba lớp:** tiền cảnh có gỗ, dây và mép bến; trung cảnh có chiếc thuyền và hoạt động lao động; hậu cảnh là nhà sàn, bờ cây và sương nước. Không thêm chi tiết nhỏ đồng đều khắp ảnh.
- **Điểm chú ý:** thuyền đang được chăm sửa và người làm việc, bằng tương phản sáng/tối của gỗ với nước; khoảng sông và sương nghỉ mắt. Không dựng khuôn mặt người lao động như chân dung của protagonist vì nhân vật được nhận ngẫu nhiên.
- **Chất liệu:** nét vẽ painterly có khối và texture gỗ ẩm, sợi đan, mái lá, mặt nước. Tránh đường viền vector đều và mảng màu phẳng lớn như bản trước; cũng không yêu cầu ảnh photoreal hay chứng minh độ chính xác lịch sử bằng texture.
- **Ánh sáng:** bình minh ấm xuyên sương lạnh, sáng dịu ở xa; mặt bến đủ rõ để thấy việc đang làm. Không bloom phủ trắng, viền vàng mọi vật hoặc tối cả cảnh để cứu contrast chữ.

| Vai trò | Hướng màu thử nghiệm | Cách dùng |
|---|---|---|
| Nước, bóng, xa | Xanh lục xám, than xanh | Giữ không khí ẩm, phân lớp chiều sâu |
| Lao động và vật liệu | Gỗ nâu ấm, mái rơm xỉn, sợi gai | Vật dụng có trọng lượng và dấu sử dụng |
| Điểm sáng | Ánh ngà vàng | Hút mắt có tiết chế quanh vùng hoạt động |
| UI | Than xanh `#142321`, ngà `#F1E8D3`, đồng xỉn `#BDA46E` | Nền chữ đủ đặc; đồng là điểm nhấn, không mặc định dùng cho mọi chữ nhỏ |

Các mã màu là gợi ý, không phải giá trị đã đo contrast. UI phải kiểm cặp màu hiển thị thực; icon, nhãn và trạng thái chữ đi cùng màu để không phụ thuộc nhận biết màu.

## Giao diện hòa cùng cảnh

Màn đầu dành phần lớn diện tích cho cảnh; tên game, một lời dẫn ngắn và một hành động chính đặt ở khoảng ít chi tiết. Khi vào bến, scene vẫn là trung tâm; thông tin nhân vật, nhật ký và xem trước hành động mở bằng panel gọn nền than thay các cột thẻ kem lớn. Chữ tiếng Việt phải rõ ở 100%, 125% và 150%; dùng focus nhìn thấy được và không chỉ có hover.

Không bake chữ, biểu tượng nút hoặc dấu tương tác vào raster. Artwork là một hình minh họa, không phải state có thẩm quyền: thấy giỏ, người hoặc gỗ trong tranh không chứng minh tài nguyên/actor đó đang tồn tại trong engine. Marker nếu có cần nhãn thao tác và thử vị trí ở từng viewport; không dùng tọa độ ảnh bị crop làm hitbox entity thật. Ở màn hẹp, ưu tiên ảnh và panel xếp dọc để không ép chữ lên người/thuyền.

Ảnh nguyên khối không có layer nhân vật, alpha, pivot, collision hoặc animation. Chưa đặt pixels-per-unit hay coi đây là asset gameplay đã nhập Unity. Cần giữ đường dẫn file gốc, kích thước thực và provenance trong README/manifest khi tích hợp; không báo thông số ảnh trước khi nhận kết quả tạo ảnh.

## Nguồn, suy diễn và giới hạn

Bến Vạc thuộc Tam Giang; nghề, hộ gia đình, thuyền, bến và giao thương là nền từ [bối cảnh project](../../../.agent) và [Module 03](../../../docs/gdd/modules/03-world-bible.md). Hình một cụm bến không đại diện toàn bộ quy mô cộng đồng. Random origin, no runtime LLM và knowledge isolation vẫn theo baseline; image generation ở đây là bước soạn asset offline.

Cách dựng nhà, trang phục, dụng cụ, sắp xếp bến, giờ sáng và ánh sáng là **hư cấu thị giác cho thử nghiệm**. Chưa có đối chiếu khảo cổ riêng cho artwork; không giới thiệu như phục dựng Văn Lang–Âu Lạc chính xác. Nhà sàn/mái lá cũng không tự chứng minh bản sắc Việt cổ: cần người dùng đánh giá và đối chiếu tư liệu trước khi chốt production art.

Ảnh dự kiến do `/root` tạo bằng công cụ image generation; tài liệu này không khẳng định ảnh đã được tạo, đã kiểm hoặc đã có quyền sử dụng độc quyền. Provenance thao tác và kết quả thực do người tích hợp ghi lại. Không dùng ảnh tải từ tác giả ngoài làm nền mà thiếu nguồn/quyền sử dụng.

## Kiểm tra và quyết định

Xem screenshot tích hợp ở desktop và màn hẹp, không chỉ xem ảnh gốc phóng lớn. Kiểm thuyền/người có đọc được, chi/đồ vật có lỗi sinh ảnh lộ rõ, panel che điểm chú ý, crop, chữ có dấu, contrast và trạng thái hành động. Reviewer ghi lỗi cụ thể kèm vị trí/evidence; không thay ý kiến người dùng bằng điểm số mỹ thuật tự đặt.

Chủ dự án cần chốt riêng: hướng painterly và chất liệu; độ sáng/mật độ chi tiết; tỷ lệ cảnh với UI. Mọi mục vẫn **Pending user approval**. Giữ nguyên Thử nghiệm 01 để so sánh; đây không phải gameplay demo Unity, human playtest hoặc sản phẩm mỹ thuật đã nghiệm thu.
