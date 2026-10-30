# Thử nghiệm11 — Bốn cách thể hiện cùng một đời sống

Sprint13 · SPIN-027 · Brief bắt đầu29/10/2026, tiếp tục thực hiện30/10/2026 · **Phiên bản tham khảo, Pending user approval**.

Vòng này thử cách tổ chức không gian lẫn ngôn ngữ tạo hình cho cả game. Mỗi bảng cần có cảnh chơi, hai mẫu nhân vật toàn thân, ba chân dung, sáu icon vật phẩm và UI hội thoại/túi đồ/nhật ký cùng phong cách. Đây là brief để tạo và review ảnh, chưa là kết luận chất lượng hoặc quyết định chuyển công nghệ.

## Hai trục có thể kết hợp

**2.5D/3D mô tả cách tổ chức không gian; pixel/anime mô tả cách tạo hình.** Chúng không phải bốn lựa chọn loại trừ nhau. Một game có thể dùng cảnh3D cùng nhân vật2D vẽ tay, hoặc cảnh nhiều lớp với sprite pixel. Bốn bảng dưới là các tổ hợp đại diện giúp xem trước, không phải phép thử chỉ thay một biến.

Các ảnh được tạo riêng, không dùng chung geometry hoặc bảo đảm camera/cast khớp tuyệt đối. Không suy từ độ sâu trong ảnh thành camera tự do, parallax hay pipeline đã triển khai; không suy phong cách pixel thành một bộ asset có grid production đã chuẩn hóa.

## Tình huống chung và tạo hình

Cảnh chợ lương thực, kho kê cao và cầu tre trước mùa nước. Nhân vật đang tới gặp người giữ kho để trao đổi công việc; người dân xung quanh tiếp tục sinh hoạt. Lối đi và quan hệ giữa người, hàng hóa, kho và cầu cần đọc được ở khoảng cách xem game.

Nhân vật minh họa chính là **phụ nữ lao động khoảng45tuổi**, áo chàm đơn giản, đai màu đất, giỏ công cụ; dáng đứng, gương mặt và bàn tay thể hiện đời sống nghề nghiệp. Người giữ kho lớn tuổi và hàng xóm có tuổi/vóc dáng/cử chỉ riêng. Tránh làm mọi cast thành thiếu niên đẹp cùng khuôn mặt hoặc trang phục chiến đấu. Đây là mẫu tạo hình để mở khả năng biểu đạt, không phải người chơi được chọn giới tính/tuổi/nghề/origin; baseline sinh nhân vật ngẫu nhiên không đổi.

## Ma trận tạo hình xuyên game

| Hướng | Không gian và thế giới | Nhân vật/portrait/icon | UI và điều cần quan sát |
|---|---|---|---|
| **2.5D nhiều lớp** — `layered-25d.png` | Góc ngang hoặc chéo nông; tiền cảnh, đường đi, công việc và hậu cảnh thành các lớp tranh vẽ tay. Dải đi lại rõ, có chiều sâu qua chồng lớp và giá trị sáng tối. | Dáng sprite2D đọc rõ trên cảnh; gương mặt cận giữ cùng quy tắc nét/mảng, tuổi và nghề không mất khi giản lược. Icon là vật thể vẽ tay cô đọng. | Panel phẳng, nét/màu cùng tranh, chữ trên vùng yên. Muốn xem lớp cảnh có làm rõ vị trí và cuộc gặp hay khiến đường đi/đồ tương tác lẫn vào trang trí. Parallax chỉ là giả thuyết chưa chạy. |
| **3D cách điệu** — `stylized-3d.png` | Góc nhìn nâng cao; khối và bóng cho thấy chiều cao kho, mặt cầu và khoảng đi lại. Vật liệu matte có vẻ được vẽ tay, tỷ lệ người bình thường, không là mô hình đồ chơi trên bệ. | Cơ thể/đầu có thể tích và pose rõ; portrait và icon giữ cách giản lược mặt phẳng/vật liệu, không ghép ảnh người thật vào cảnh cách điệu. | UI sạch, phân lớp bằng khoảng cách/viền và nhóm giá trị, không dùng texture khối lấn chữ. Muốn xem người/vật có phân biệt bằng silhouette mà không cần mọi thứ phát sáng. Ảnh chưa chứng minh mesh, rig hoặc camera quay được. |
| **Pixel đời làng** — `pixel-life.png` | Cụm pixel, cạnh bậc thang và vùng màu có chủ đích ở cả kiến trúc, cây, người và nền. Lối đi yên hơn khu hoạt động. | Sprite, portrait và icon dùng chung logic cluster/độ giản lược; tuổi, dáng và biểu cảm không chỉ dựa vào đổi màu áo. | Khung và icon theo nhịp pixel; chữ có dấu phải đọc được, không dùng glyph trang trí thay nội dung. Muốn xem độ sắc và nhịp chi tiết có nhất quán ở kích thước mục tiêu. PNG concept chưa là tileset/spritesheet, palette giới hạn hay grid kỹ thuật đã kiểm. |
| **Anime đời thường** — `anime-life.png` | Cảnh vẽ tay, nét chọn lọc và mảng sáng tối rõ; bố cục dẫn tới người đang nói thay vì chỉ tạo vista đẹp. | Cast trưởng thành có nét mặt, tuổi và vóc dáng riêng; nếp áo/cử chỉ phục vụ nghề và cảm xúc, không mặc định nhân vật chính trẻ hoặc chiến đấu. Portrait dùng cùng thiết kế với mẫu toàn thân. | Chữ/icon/panel theo ngôn ngữ nét và mảng của cảnh nhưng có hierarchy riêng; không phủ bộ UI fantasy bất kỳ lên tranh. Muốn xem sức biểu cảm có giữ được việc đọc tình huống và đa dạng cuộc đời. Chưa có animation hoặc biểu cảm chuyển trạng thái thực. |

## Cách quan sát và giới hạn

- Xem board nguyên khung và PNG gốc: cùng quy tắc hình, nét, vật liệu có đi qua cảnh, toàn thân, portrait, vật phẩm và UI không? Tuổi/nghề/cử chỉ của người có đọc được mà không cần caption giải thích hết?
- Phân biệt **mức khớp ý định**, **đánh đổi thẩm mỹ** và **lỗi chặn xem**. Một điểm chưa như brief không tự loại toàn bộ hướng; vòng mới không chê hoặc thay thế các bản01–10.
- Các lớp cảnh, chuyển động, quay camera, native pixel grid, số màu, chi phí và performance đều chưa có evidence từ một ảnh tĩnh. Chữ/nút UI trong board là raster, không reflow hoặc tương tác như component thật; cần kiểm chữ nhỏ bằng ảnh gốc.

Bối cảnh, trang phục, cast, lời thoại và icon là hư cấu mỹ thuật lấy cảm hứng Việt cổ, chưa chốt canon hoặc phục dựng khảo cổ. Không thêm nghề/rule/event hoặc quyết định chuyển Unity/engine từ các bảng concept. Root tạo bốn PNG bằng built-inimage_gen rồi lưu prompt/provenance và kết quả quan sát thật. Vòng này không tạo model3D, không gọi Hyper3D/Blender và không làm gameplay. Mọi hướng tiếp tục bổ sung thư viện tham khảo; chủ dự án giữ quyền chốt.
