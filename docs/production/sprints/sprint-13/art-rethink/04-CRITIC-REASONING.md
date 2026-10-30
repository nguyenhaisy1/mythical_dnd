# Phản biện workshop — lập luận và ranh giới bằng chứng

30/10/2026 · Sprint13 / SPIN-029 · Reviewer độc lập `/root/art_rethink_critic_reasoning` · **Review tài liệu, Pending user approval**.

## Phạm vi thực sự đã kiểm

Đã đọc yêu cầu hiện tại, `.agent`, HANDOFF, baseline áp dụng, chỉ dẫn mới trong AGENTS, skill mythical-validation/game-design-review, bản tổng hợp `ART-DIRECTION-RETHINK.md` và ba bài01–03 của workshop. Review này kiểm cách đặt vấn đề, nguồn và quyết định được đề xuất; không tự xem lại toàn bộ artwork hoặc nhận lượt view_image của worker audit là lượt xem của reviewer. Không tạo ảnh/model/runtime hoặc thao tác Git.

Đã mở trực tiếp hai nguồn bảo tàng đại diện: [giới thiệu Ngọc Lũ](https://baotanglichsu.vn/vi/Articles/1001/13602/gioi-thieu-bao-vat-quoc-gia-1-trong-djong-ngoc-lu-hien-vat-tieu-bieu-nhat-cua-nen-van-hoa-djong-son.html) và [Nghệ thuật Đông Sơn](https://baotanglichsu.vn/vi/Articles/3101/68748/nghe-thuat-djong-son.html). Chúng có mô tả hiện vật và hệ hình được bài02 viện dẫn; không suy từ đó ra một costume, palette hoặc hình học thống nhất cho cả campaign. Lượt tìm lại nội dung trang Cổ Loa của cơ quan di sản bị timeout; reviewer không gọi đó là nguồn đã được mình xác minh đầy đủ. Bài02 ghi rõ actor nghiên cứu đã đọc nguồn, và giới hạn kiểm nguồn của reviewer không tự phủ định bằng chứng của actor ấy. Không sử dụng bài đăng lại6271 về mái nhà để chứng nhận phục dựng khảo cổ.

## Kết luận về lập luận chính

Workshop đã trả lời đúng trọng tâm hơn cách chỉ đổi chất liệu: trục camera/không gian, tỷ lệ/khối, rendering, cử chỉ, UI và thông tin được tách thành quyết định cụ thể. 3D không bị đồng nhất với tả thực hoặc camera quay tự do. Cách gọi preview3D không ngầm chứng minh có mesh/runtime; lịch sử04/05 có geometry thật được giữ nhưng không dùng để phủ nhận cảm nhận user.

Chất Văn Lang–Âu Lạc được đặt vào câu hỏi hình, hành động, cư trú và kỹ nghệ, không chỉ trống đồng/màu patin/lông chim. Các lựa chọn phóng đại và UI tiếng Việt được ghi là sáng tạo hiện đại. Bản tổng hợp không cấm vĩnh viễn realism, không tự chốt một engine/style, không làm mới canon hoặc biến sample thành character creator. AGENTS/.agent bổ sung phù hợp với yêu cầu user và giữ Pending; không thấy mâu thuẫn chặn việc bàn giao workshop.

Sáu hướng có khác biệt thực về cấu trúc đọc: A nhấn khối và sân có thể tiếp cận; B nhấn dải hành động; C nhấn mặt bằng/hình; D phân vai người2D với nền khối; E nhấn cự ly cơ thể; F thay mức nhìn. Tuy vậy, chúng là các tổ hợp có thể giao nhau, không phải sáu phát minh loại trừ nhau. Thứ tự root ưu tiên1/2 rồi3 là giả thuyết nghiên cứu hợp lý, chưa có output chứng minh ưu thế. Việc nhiều worker đồng ý không phải bằng chứng art acceptance hoặc độc lập kiểm chứng cùng một giả thuyết.

## Findings cần xử lý trước khi dùng brief sinh ảnh

### P2-R1 — Câu chống tỷ lệ “trẻ con” có thể kéo brief về tả thực lần nữa

Vị trí ban đầu: bài03, mụcA/Đánh đổi, câu “Phải giữ người có tuổi, tỷ lệ không quá trẻ con”. Đây là điều kiện hình thể mơ hồ bên cạnh thông điệp cho phép cách điệu trưởng thành. Nó có thể lại khiến tác nhân hiểu đầu lớn/chi ngắn là không hợp lệ và quay về tỷ lệ tự nhiên, chính kiểu khóa mà audit đang phản biện.

Đề nghị thay bằng việc giữ dấu tuổi/nghề, tư thế, quan hệ và trọng lượng câu chuyện; cho phép tỷ lệ cơ thể lạ/phóng đại. Không đồng nhất phong cách dễ thương hoặc đầu lớn với nội dung trẻ em. Đây là sửa ngôn ngữ brief, không yêu cầu tạo thêm artwork.

### P2-R2 — Hướng ưu tiên cần một phép đối chiếu với mẫu gần nhất đã làm

Vị trí: bản tổng hợp mục4 và bài03/A–B. Khối nhìn chéo và2.5D nhiều lớp đã có họ hàng trong08/11 và các vòng trước. Đặt tên mới không đủ bảo đảm lượt tiếp khác. Tài liệu đã nêu quy tắc tốt, nhưng cần nối ngắn gọn **mẫu cũ gần nhất → điều cụ thể phải đổi → dấu hiệu ảnh mới vẫn chỉ lặp nhãn**.

Ví dụ:08C/11'3D' sangA phải đổi cả tỷ lệ người, khối mái/cây/thuyền và hệ bóng, không chỉ facet đá hoặc bề mặt matte; một hướng dải ngang phải đổi cách đọc cao độ, đường tiếp cận và động tác, không chỉ đặt tranh mới vào panorama ngang. Tên vòng cũ nên được ghi từ artifact/evidence thật; reviewer không yêu cầu root nhận đã xem một ảnh chưa xem. Đối chiếu này giúp kiểm được lời hứa với user mà không tuyên bố kết quả trước khi có ảnh.

## Trạng thái và giới hạn

Không phát hiện P1 chặn bàn giao cuộc brainstorming. Hai P2 là biện pháp làm brief ít mơ hồ và ngăn tái diễn phương pháp, không phải kết luận cần loại hướngA/B hoặc bỏ01–12. Khi root sửa, reviewer sẽ kiểm lại đúng nội dung liên quan và ghi trạng thái bên dưới. Mọi phương án tiếp tục Pending user approval; review lập luận không chứng minh hình ảnh mới đã có vibe, gameplay đã dễ đọc hoặc motion đã tốt. Không có runtime, Unity, benchmark hoặc human playtest trong review này.

**Đối chiếu sau sửa — hoàn tất trong workshop30/10:** đã đọc lại bài03/A và bản tổng hợp mục3–4 sau root sửa. R1 đã xử lý: câu mới giữ dấu tuổi/nghề/quan hệ ngay cả khi đầu lớn/chi ngắn, không dùng tỷ lệ phóng đại để suy trẻ con. R2 đã xử lý ở mức brief: có bảng08C/11,06C,11pixel/12animation và board11–12 → quy tắc cần đổi → dấu hiệu vẫn lặp. Root ghi rõ mình mở thêm06C; reviewer không nhận lượt xem đó là của mình. Bảng nguồn→biến đổi→dấu hiệu nhìn được còn làm cụ thể thêm yêu cầu văn hóa. Có đối chứng cự ly gần nếu hướng nhìn chéo lại ra mô hình trưng bày, đồng thời ghi giới hạn khi nhiều biến cùng đổi. Hai finding đã **được xử lý trong tài liệu**; chưa có ảnh mới để xác nhận hiệu quả mỹ thuật. Không còn yêu cầu sửa chặn việc bàn giao bản workshop đã review.
