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

## Xem trước ý tưởng bằng ảnh — chỉ dẫn chủ dự án 29/10/2026

Ở giai đoạn chọn ý tưởng hiện tại, tạo **ảnh mô phỏng game**, có thể là tranh 2D hoặc ảnh mang diện mạo 3D; không tạo model 3D thật chỉ để trình bày ý tưởng. Chỉ dẫn này thay cách triển khai bằng model ở Experiment 05, không yêu cầu xử lý tiếp lượt sửa Hyper3D đang chưa xác định ID. Dùng ảnh concept để chủ dự án xem bối cảnh, góc nhìn, phong cách và giao diện minh họa trước; ghi rõ ảnh không phải game đang chạy hoặc render từ một scene 3D đã dựng. Không tự gọi Hyper3D/Blender hay xây pipeline 3D cho yêu cầu thử hình ảnh.

Quy tắc 28/10 về MCP Hyper3D chỉ áp dụng khi chủ dự án giao lại task **tạo model 3D thật** trong tương lai; không buộc dùng Hyper3D cho ảnh có vẻ 3D. Giữ các thử nghiệm cũ và nguồn gốc thật. Experiment 06/SPIN-022 dùng ảnh mô phỏng và gallery để so sánh; mọi phương án Sprint 13 vẫn Pending user approval. Việc đổi cách xem trước không phải duyệt mỹ thuật hoặc cho phép chuyển Sprint 14.

## Thư viện phiên bản tham khảo — làm rõ ngày 29/10/2026

Mỗi spin/thử nghiệm hình ảnh bổ sung một phiên bản để chủ dự án tham khảo. Mở vòng mới không có nghĩa phiên bản cũ bị bỏ, bị thay thế hoặc bị đánh giá kém; giữ artwork, nguồn và hồ sơ cũ để xem lại, so sánh hoặc kết hợp ý tưởng. Chỉ ghi loại/thay thế/chọn khi chủ dự án nói rõ. Nhận xét và findings phải gắn đúng phiên bản, artifact và phạm vi đã quan sát; một lỗi cụ thể không tự là kết luận toàn bộ ý tưởng dở. Giữ nguyên phản hồi lịch sử thật, gồm phản hồi đồ họa bản01 chưa ưng ý. Lời khen tiến triển hoặc yêu cầu nghĩ thêm không là final approval cho một hướng hay Sprint13.
