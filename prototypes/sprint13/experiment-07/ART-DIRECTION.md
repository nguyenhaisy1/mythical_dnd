# Thử nghiệm 07 — Ba ngôn ngữ đồ họa

Sprint 13 · SPIN-023 · 29/10/2026 · **Pending user approval**.

Mục tiêu là mở lựa chọn về hình khối, nét, màu và chất liệu, vượt khỏi nhóm tranh tả thực của vòng06. Ba ảnh raster độc lập thử ba lát cắt đời sống trong thế giới Việt cổ hư cấu; không tạo model3D hoặc triển khai gameplay.

| Hướng | Ý tưởng hình ảnh | Điều muốn người xem nhận ra | Câu hỏi cần xem |
|---|---|---|---|
| A — Khắc gỗ | Mảng phẳng khoảng năm màu, nét khắc và khoảng trống rõ; làng mùa nước, người chuyển lương thực lên kho cao. | Người, đường vận chuyển và mực nước bằng silhouette; nghề nghiệp và sự phụ thuộc cộng đồng. | Nét/mảng có tạo bản sắc riêng mà vẫn đọc được việc đang xảy ra? Có thành tranh trang trí quá nhiều không? |
| B — Pixel art | Các cụm pixel và cạnh bậc thang nhìn thấy rõ, màu có vai trò; góc nhìn bao quát một xưởng đúc đồng. | Người học nghề, người thợ và các khu công việc; vật liệu khác nhau mà không cần texture tả thực. | Pixel có thật sự nhất quán và đọc rõ ở kích thước xem game? Người và dụng cụ có lẫn vào nền không? |
| C — Giấy cắt | Silhouette như giấy màu cắt tay, lớp cảnh chồng lên nhau và bóng tiếp giáp tiết chế; chợ hoặc đêm hội ven sông. | Những nhóm người, nơi gặp gỡ và lối đi; cảm giác cộng đồng và chuyện kể. | Chất liệu giấy có đủ khác hai hướng kia? Chiều sâu có giúp đọc cảnh hay khiến cảnh giống mô hình trưng bày? |

Đây là **thử đồng thời phong cách và bối cảnh**, không phải phép so sánh kiểm soát chỉ thay một yếu tố. Nếu thích một ảnh, chưa thể biết nguyên nhân là đồ họa, hoạt động, ánh sáng hay địa điểm. Sau khi có hướng gần ý người dùng, có thể thử chính ngôn ngữ đó trên cùng một tình huống với hướng đối chiếu.

Các hoạt động kho lương, xưởng đúc và sinh hoạt cộng đồng là đề xuất minh họa theo các trụ cột nghề nghiệp/xã hội của game. Bố trí, nhân vật và nghi lễ trong ảnh chưa là canon mới, phục dựng khảo cổ hoặc cam kết mở thêm phạm vi vertical slice. Không đưa phép thuật hiển nhiên vào ảnh rồi coi đó là quy tắc thế giới đã chốt.

## Cách review

Xem ảnh gốc và trong gallery, có thể ẩn HUD minh họa. Đối chiếu điều thực sự nhìn thấy với brief: hình khối, ngôn ngữ nét/pixel/giấy, phân lớp sáng tối, màu vật liệu và trọng tâm con người. Không gọi việc đổi palette là đổi phong cách nếu nét và bề mặt vẫn như cũ. Không coi tên phong cách trong prompt là bằng chứng công cụ đã thể hiện đúng.

Ảnh A/B/C dự kiến tại `assets/a-woodcut.png`, `assets/b-pixel.png`, `assets/c-papercut.png`. Root lưu prompt, nguồn và kết quả generation; người viết brief chưa xem output ở thời điểm lập tài liệu, nên chưa kết luận hướng nào đạt hoặc đẹp hơn. Mọi hướng vẫn chờ chủ dự án chốt; giữ nguyên01–06.
