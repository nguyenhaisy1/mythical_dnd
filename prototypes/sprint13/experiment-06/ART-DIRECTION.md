# Thử nghiệm06 — Ba cách nhìn Bến Vạc

Sprint13 · SPIN-022 · 29/10/2026 · **Pending user approval**.

## Câu hỏi cần trả lời

Nếu cùng một tình huống được trình bày bằng ba ngôn ngữ hình ảnh khác nhau, hướng nào giúp chủ dự án hình dung rõ nhất game muốn làm? Đây là ảnh mô phỏng màn hình game để chọn ý tưởng, không phải yêu cầu dựng model hoặc chứng minh một renderer đã hoạt động.

Cảnh chung: bến sông Bến Vạc, Tam Giang; một chiếc thuyền lòng mở được kê để sửa, người thợ đang làm việc, nhân vật quan sát và vài dấu vết sinh hoạt ven bến. Thuyền, giá kê và mái xưởng nếu có phải đọc thành những cấu trúc riêng. Lao động và quan hệ cộng đồng là trọng tâm; không thêm combat hoặc phép thuật để tạo cảm giác “RPG”. Chi tiết Việt cổ ở đây là diễn giải mỹ thuật theo game-canon, chưa được nghiệm thu khảo cổ.

## Ba phương án

| Hướng | Camera và cách tạo hình | Màu, ánh sáng và vật liệu | Giả thuyết cần xem qua ảnh | Đánh đổi cần đánh giá |
|---|---|---|---|---|
| A — Tranh2D góc chéo | Điểm nhìn cao vừa phải, thấy đường đi, lòng thuyền và quan hệ giữa người với xưởng. Gouache và nét mực chọn lọc, hình khối rõ ở khoảng cách chơi. | Xám nước, đất nhạt, xanh lá tiết chế; điểm nhấn nhỏ ở người hoặc công việc. Ánh sáng tản, mảng sáng tối phân lớp, không phủ vàng toàn cảnh. | Người xem nhận ra việc đang diễn ra và các nơi có thể tới mà không phải phóng lớn. | Góc cao có thể làm con người quá nhỏ; chi tiết dày dễ cạnh tranh với hành động. |
| B — Ảnh giả lập3D gần nhân vật | Góc nhìn gần tầm người hoặc qua vai, giữ cả người quan sát, người thợ và thuyền trong bố cục. Ảnh tạo cảm giác một cảnh3D có chiều sâu. | Ánh sáng tự nhiên trung tính, gỗ cũ bạc xám khác ván mới, đất ẩm khác nước. Bề mặt có dấu tay làm, tránh bóng nhựa và màu cam–xanh điện ảnh. | Người xem cảm thấy mình đang đứng trong cộng đồng và hiểu có thể tiếp cận ai. | Góc gần có thể che lòng thuyền, tuyến đi hoặc các lựa chọn bên ngoài khung. Một ảnh không chứng minh camera xoay được. |
| C — Sân khấu2.5D nhìn ngang | Các lớp tiền cảnh, mặt hoạt động và hậu cảnh; đường di chuyển nhìn ngang có khoảng trống. Hình vẽ tay, silhouette chọn lọc. | Mảng màu có chủ đích, độ sáng tách lớp; mật độ chi tiết tập trung quanh công việc, phần còn lại thở. | Người xem đọc được nhịp sinh hoạt và điểm tiếp cận bằng hình dáng, không cần phủ marker lên mọi vật. | Chiều sâu và nhiều nhánh di chuyển có thể khó diễn tả; ảnh không chứng minh parallax hoặc khả năng khám phá. |

Ba ảnh là ba diễn giải độc lập của cùng tình huống, **không phải ba camera của một model chung**. Khác biệt vị trí và chi tiết nhỏ giữa các ảnh phải được xem là giới hạn của so sánh concept.

## Quy tắc trình bày

- Ảnh là sản phẩm chính. HTML chỉ hỗ trợ xem lớn, chuyển phương án và thử lớp giao diện minh họa.
- Dùng typographyHTML cho tiếng Việt; tránh giao ảnh sinh chữ dài hoặc dùng chữ giả làm thông tin gameplay. LớpUI phải ghi rõ là mô phỏng và có thể tắt để xem tranh.
- Tách màu vật liệu thay vì phủ một bộ lọc lên toàn cảnh. Không dùng độ bão hòa cao hoặc ánh sáng huyền ảo để che cấu trúc khó đọc.
- Không tự bổ sung thông tin bí mật của NPC vàoUI. Nếu có lời nhắc, chỉ mô tả điều thấy được trong ảnh hoặc ghi rõ dữ liệu minh họa.
- Giữ01–05 để đối chiếu. Không gọiHyper3D, không dựngBlender hoặc tạo asset3D thật trong vòng06.

## Tiêu chí review sau khi có ảnh

1. Nhìn ở kích thước màn hình: nhận ra thuyền đang được sửa, người liên quan và hướng tiếp cận chưa?
2. Thuyền lòng mở, giá kê và mái xưởng có bị gộp sai thành nhà thuyền không?
3. Ba hướng có khác về camera, hình khối và chất liệu, hay chỉ đổi màu?
4. Màu cục bộ có phân biệt gỗ, đất, nước và quần áo; điểm nhìn chính có đủ rõ?
5. UI minh họa có che công việc, làm chữ quá nhỏ hoặc khiến ảnh tĩnh bị hiểu là game đã chạy không?

Đây là tiêu chí quan sát, chưa phải kết quả. Nhận xét mỹ thuật củaAI cần ghi như diễn giải; chỉ chủ dự án chốt hướng. Chưa có bằng chứng humanplaytest, performance, Unity, navigation hoặc gameplay.
