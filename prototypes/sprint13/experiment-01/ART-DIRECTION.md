# Thử nghiệm 01 — Sớm nước, bến nhà

**Sprint 13 / SPIN-016 — đề xuất để chủ dự án duyệt, chưa nghiệm thu.** Tác giả thực hiện: AI `/root/s13_art`. Sản phẩm cụ thể: [minh họa Bến Vạc](assets/ben-vac-dawn.svg), SVG tự viết 1600×1000. Đây là concept art và phông thử giao diện, chưa phải scene Unity hay bản đồ có collision.

## Ý đồ thị giác

Bước vào một buổi sớm nơi đời sống vẫn đang tiếp diễn: nhìn thấy bến, việc làm và khoảng nước trước khi nhìn thấy một mục tiêu phiêu lưu. Cảm xúc chính là thân thuộc nhưng mong manh: ánh sáng ấm trên một dòng nước rộng, những vật dụng nhỏ cạnh thuyền cho thấy công sức con người. Góc nhìn nghiêng từ bờ, không dùng lưới tactical hoặc camera chính xác để giả thành thiết kế navigation đã chốt.

Minh họa gồm ba lớp: doi đất và nhà sàn xa; dòng nước với thuyền nhỏ ở giữa; bến sửa thuyền, gỗ và lau tiền cảnh. Khoảng nước giữ nhiều không gian thoáng để UI không phải đè lên chi tiết. Nhà sàn và bến thuộc một cụm nhỏ của Bến Vạc, không minh họa toàn bộ 420 hộ trong GDD. Các thuyền/hình người trong artwork là trang trí, chưa là entity tương tác.

## Ngôn ngữ hình và màu

| Thành phần | Quy tắc thử nghiệm | Màu mẫu |
|---|---|---|
| Nước | Mảng cong dài, ít gợn, sáng ở xa và đậm gần người xem | `#8EA99A`, `#567F79`, `#244D50` |
| Ánh sáng | Vàng nhạt; không bloom mạnh; sáng sớm từ bên phải | `#F8E7BB`, `#F5DB98` |
| Nhà và đồ nghề | Mái thấp, cọc gỗ, thuyền có sườn; cạnh hơi lệch nhưng dễ đọc | `#B39760`, `#74573B`, `#453F30` |
| Cây và bờ | Xanh xám; cụm hình lớn, không chi tiết lá dày đặc | `#536C4B`, `#344E40` |
| UI đề xuất | Nền giấy sáng hoặc than xanh đậm; chữ có tương phản độc lập với cảnh | `#F4ECD9`, `#233E3B` |

Màu không là kênh duy nhất để báo tin đồn, nguy cơ hoặc nút bị khóa. Texture hiện dùng nét gợn và đường mái native SVG, không dùng nhiễu động hoặc texture bên ngoài. Không nhấp nháy, parallax hay chuyển động bắt buộc trong thử nghiệm đầu.

Chữ giao diện ưu tiên sans-serif hệ thống có dấu tiếng Việt đầy đủ; tiêu đề có thể dùng serif hệ thống để tạo cảm giác kể chuyện. SVG không chứa chữ hiển thị nên không phụ thuộc font. Chưa chọn hay đóng gói font thương mại; typography này là thử nghiệm, font production còn cần chốt coverage, license và kiểm đọc 100/125/150%.

## Nguồn và mức chắc chắn

- Canon dự án: `.agent`, baseline hiện hành và Module03, mục TG-01: Bến Vạc thuộc Tam Giang, là mạng nhà sàn/bến/đảo nhỏ, dòng chính dịch chuyển, thuyền và quyền bến quan trọng. SVG thể hiện tinh thần đó, không vẽ một trung tâm cố định hay đền cung điện.
- Sáng tạo mới cho thử nghiệm: thời điểm buổi sớm trước mùa nước lớn; góc nhìn; cách xếp nhà/bến; màu mái; vật liệu bề mặt; thuyền cụ thể; hình người và đồ nghề. Đây là hư cấu thị giác, không phải nghiên cứu khảo cổ được kiểm chứng hoặc thay đổi lịch runtime.
- Không khẳng định mái nhà, trang phục, cấu trúc thuyền hay cảnh quan này chính xác cho một niên đại Văn Lang–Âu Lạc. Chưa dùng nguồn lịch sử bên ngoài để tránh gán bằng chứng cho thiết kế suy diễn. Cần bước đối chiếu tư liệu trước khi chốt art production.
- Artwork do agent tự viết bằng các primitive/path SVG trong repository; không chép asset tải về, không dùng stock, font nhúng, API ảnh hay mẫu bitmap. Đây là provenance quy trình, không phải bảo đảm pháp lý về quyền tác giả. Không có tài nguyên mạng, script hoặc tracking trong SVG.

## Cách dùng và điều kiện đánh giá

Có thể mở trực tiếp SVG hoặc dùng `<img>` trong prototype; giữ tỷ lệ 8:5, dùng `object-fit: cover` nếu container khác tỷ lệ nhưng không đặt hotspot dựa vào tọa độ crop. Cảnh không chứa thông tin gameplay có thẩm quyền. Tên địa điểm, các điều nhân vật biết và hành động phải đến từ lớp projection của giao diện, không suy diễn từ việc nhìn thấy một hình vẽ.

Chủ dự án cần chốt: có thích hướng minh họa mảng phẳng này không; có muốn tăng chất vẽ tay/texture; độ sáng ấm hay u tĩnh; tỷ lệ cảnh so với chữ; mức chi tiết nghề nghiệp. Chưa có quyết định nào được xem là đã duyệt. Nếu đổi hướng, giữ thử nghiệm01 làm lịch sử và làm thử nghiệm mới; không sửa nguồn nghiên cứu để hợp thức hóa artwork.

Kiểm tra sản phẩm tại bước bàn giao: XML parse, ID/reference nội bộ đầy đủ, không tài nguyên/script ngoài. Kiểm đọc UI và bố cục thực tế do prototype tích hợp kiểm chứng riêng; artwork không chứng minh gameplay hoặc tính dễ tiếp cận của toàn client.
