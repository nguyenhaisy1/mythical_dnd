# Định hướng mỹ thuật cho thử nghiệm game

Đọc khi task cần chọn/đổi visual direction, concept scene hoặc UI game. Đây là adaptation chọn lọc Art Director của Claude Code Game Studios và Game Art của Agentic Awesome Skills; [nguồn pin và license](../../upstream/DESIGN-SOURCES.md). Không thay GDD hoặc quyền chốt của user.

## Từ trải nghiệm tới hình ảnh

Đọc pillars và đoạn GDD liên quan trước. Xác định cảm giác muốn tạo, việc người chơi cần nhận ra và lựa chọn họ phải hiểu. Với mythical_dnd, nghề nghiệp/gia đình/quan hệ phải có chỗ trong hình ảnh; không mặc định RPG nghĩa là combat, giáp và loot. Phân biệt canon, dữ kiện có nguồn và hư cấu mỹ thuật; cảnh Việt cổ không tự trở thành phục dựng lịch sử.

Khi hướng chưa chốt, tạo vài phương án khác nhau có chủ đích nếu so sánh giúp quyết định: khác camera, mật độ chi tiết, chất liệu, hình khối, mức cách điệu hoặc tổ chức UI. Giải thích lợi ích/đánh đổi bằng cùng một cảnh và tình huống. Không chỉ đổi palette rồi gọi đó là hướng mới; không buộc luôn có nhiều phương án khi user đã chọn hướng hoặc chỉ giao sửa nhỏ.

Đưa ra artifact nhìn được: concept image/scene, key screen hoặc mockup tương tác theo yêu cầu. Bảng màu và tài liệu mô tả là phần hỗ trợ, không thay artwork khi user giao thiết kế hình ảnh. Chọn SVG/code khi cần hình vector hoặc UI có cấu trúc; dùng imagegen/generate2dmap/generate2dsprite đúng loại khi cần tranh, map hay sprite bitmap. Đọc skill của công cụ tương ứng trước khi dùng; không mặc định vẽ mọi concept bằng CSS/SVG cho tiện.

## Art direction đủ để tái tạo

Ghi vừa đủ để người làm asset khác giữ được cùng phong cách:

- Camera, scale, silhouette và tỷ lệ nhân vật/đồ vật; điểm nhìn ở khoảng cách gameplay thật.
- Bảng màu theo vai trò: môi trường, điểm tương tác, nguy hiểm, thông tin chưa chắc; độ sáng giúp phân lớp ngay cả grayscale.
- Chất liệu, ánh sáng, hình khối và mật độ chi tiết; foreground/background không tranh điểm chú ý với hành động.
- Typography tiếng Việt, kích thước hiển thị, trạng thái focus/hover/disabled và phản hồi hành động. Không dùng màu đơn độc truyền trạng thái.
- Animation/motion nếu trong scope: anticipation, timing, staging và feedback giúp đọc hành động. Không áp số frame chung như tiêu chuẩn bắt buộc.
- Asset output phù hợp engine: kích thước, pivot/layer/alpha, đơn vị hoặc pixels-per-unit khi đã chọn; provenance/license và phần còn thử nghiệm. Không suy DPI metadata là kích thước trên màn hình.

Mỗi quy tắc nên có một ví dụ đúng/sai hoặc artifact chứng minh khi cần, thay vì viết art bible dài nhưng không thể review.

## Cổng kiểm tra thực tế

Xem artifact ở kích thước target, không chỉ zoom lớn. Kiểm silhouette, focal point, chữ có dấu, contrast, UI che cảnh, mất thông tin khi crop và các trạng thái action. Với UI tương tác, thử bàn phím, cỡ chữ và thông tin nhân vật được biết; ảnh đẹp không chứng minh gameplay/knowledge isolation.

So sánh phương án bằng tiêu chí của game: bản sắc, khả năng đọc lựa chọn/hậu quả, mức nhất quán và chi phí sản xuất. Trình bày nhận xét mỹ thuật là nhận xét, không giả metric hay phản hồi playtest. Những mục chưa có bằng chứng giữ Unknown/Not assessed.

Áp dụng approval register hiện hành. Riêng Sprint13, mọi phương án vẫn Pending user approval dù technical checks đạt; nhắc pending khi user muốn sang sprint mới. Không nhập policy hỏi phép trước từng lần viết file từ upstream: quyền tạo/sửa đã giao vẫn có hiệu lực. Giữ bản thử cũ, log spin thật; skill không tự mở sprint hoặc sinh thêm phương án ngoài mục tiêu đang giao.

## Tạo mô hình3D — chỉ dẫn chủ dự án28/10/2026

Từ lần tạo mô hình3D tiếp theo, dùng MCP Hyper3D đã cài làm đường tạo model. Blender/engine có thể dùng cho bố trí cảnh, camera, kiểm tra và render sau đó. Nếu MCP chưa hiện hoặc không truy cập được, ghi rõ tình trạng và khôi phục kết nối; không âm thầm thay bằng procedural mesh hoặc công cụ sinh model khác. Chủ dự án có thể chỉ định ngoại lệ rõ ràng. Experiment04/SPIN020 đang dựng bằng Blender trước chỉ dẫn này được tiếp tục hoàn thiện theo yêu cầu user, phải ghi đúng nguồn; không gán là modelHyper3D.
