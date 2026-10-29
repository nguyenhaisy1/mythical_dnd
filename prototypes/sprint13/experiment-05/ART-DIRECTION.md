# Thử nghiệm05 — Xưởng sửa thuyền có chất liệu

Sprint13 / SPIN-021. **Chờ chủ dự án chốt.** `/root/s13e5_direction` soạn định hướng và [model brief](model-brief.json); `/root` phụ trách gọi provider, lưu kết quả và tích hợp. Tài liệu này mô tả mục tiêu trước khi tạo, chưa xác nhận model Hyper3D đã thành công hoặc đạt phong cách.

## Câu hỏi của vòng thử

Bản04 cho phép xem cùng hình học ở nhiều camera nhưng còn cảm giác diorama đơn giản; một số điểm tiếp xúc trông lơ lửng và bản realtime thô hơn render. Lần này thử một asset nhỏ có vật liệu và chi tiết phong phú hơn từ **MCP Hyper3D**, để xem chất gỗ, mái lá, dây và đồ nghề có làm nơi làm việc thuyết phục hơn không. Không mặc định model sinh ra sẽ tốt hơn; phải nhìn output, tiếp xúc, silhouette và mặt khuất thật.

## Một cụm công việc, không cả làng

Chủ thể là một chiếc thuyền gỗ sông nhỏ có lòng mở, sườn bên trong và vài mảng ván vá, đặt trên hai giá chắc chắn. Một mái lá bốn cột che nửa sau khu sửa để thân thuyền vẫn đọc được từ bên ngoài. Cụm đặt trên sàn ván mỏng, có một bàn thấp, một cuộn dây sợi tự nhiên, hai giỏ đan và một chum đất. Các chân cột, giá thuyền và vật đặt sàn phải có chỗ tiếp xúc nhìn thấy được, không chỉ phủ bóng để che việc nổi hoặc xuyên nhau.

Không tạo người trong asset này: việc đánh giá tập trung vào kết cấu xưởng và vật liệu, tránh hình người dính vào thuyền/cột hoặc lộ giải phẫu giả. Người có thể được thử thành asset riêng ở một vòng được giao sau; không tự mở rộng thành settlement, tàu thuyền di chuyển hoặc hệ nghề nghiệp đang chạy.

## Màu và bề mặt

Dùng màu cục bộ theo vật liệu: gỗ cũ bạc xám/nâu nhẹ; màu gỗ tươi chỉ ở ván vá hoặc cạnh cắt; lá khô nhạt; dây xỉn; chum màu đất; một mảnh vải đỏ gạch trầm làm dấu công việc. Các vật phải phân biệt bằng độ sáng, cấu trúc và phản ứng ánh sáng, không chỉ đổi màu. Tránh nhuộm mọi vùng sáng vàng và mọi bóng xanh, độ bóng nhựa hoặc noise giống nhau trên tất cả bề mặt.

Hình thức hướng tới hiện thực cách điệu, đủ rõ cấu tạo nhưng không cần tối đa chi tiết ở mọi nơi. Thân thuyền và giá đỡ là điểm nhìn chính; texture không được lấn át silhouette hoặc khiến mọi ván gỗ sắc cạnh như kim loại. Prompt đề nghị màu trung tính và không bake ánh sáng, nhưng đây là yêu cầu cho provider, không là bằng chứng output thực sự có PBR sạch.

## Camera và so sánh

Xem cùng model ở ba cách: góc cao chéo để hiểu mái–thuyền–lối tiếp cận; gần ngang tầm người để cảm nhận tỷ lệ và các điểm tiếp xúc; góc gần thân thuyền để đọc sườn, ván vá và bề mặt. Bố trí camera sau khi đo bounding box thật; nếu quy đổi về mét hoặc chuẩn hóa kích thước, ghi rõ phép biến đổi thay vì gán scale provider thành dữ kiện canon. Góc thấp không cần nhét toàn bộ asset vào khung, nhưng không để mái hoặc cột che mất câu hỏi đang kiểm.

So sánh trực tiếp với [bản04](../experiment-04/index.html) ở ánh sáng trung tính tương tự. Kiểm cả mặt trước, cạnh và phía sau; texture đẹp từ một góc chưa chứng minh asset xoay được. Giữ một bản màu gốc của provider để phân biệt cải thiện của model với tác động của lighting, camera hoặc hậu kỳ. Không dùng filter để làm như texture/model đã được sửa.

## Nguồn và cổng quyết định

Bến Vạc thuộc Tam Giang và hoạt động sửa thuyền theo [bối cảnh project](../../../.agent); hình dáng cụ thể của mái, thân thuyền, đồ nghề và vật liệu trong brief là hư cấu mỹ thuật prototype. Không gọi đây là phục dựng Việt cổ hoặc tư liệu lịch sử. Ảnh B của vòng03 cung cấp bối cảnh thẩm mỹ đã có trong project, chưa phải hướng được user duyệt.

Model phải được tạo qua MCP Hyper3D theo chỉ dẫn user. Blender được dùng sau đó cho bố trí, camera, kiểm tra và render; không thay mesh sinh thất bại bằng mesh thủ tục rồi giữ nhãn Hyper3D. Root ghi generation ID, kết quả và nguồn tải sau khi có trả lời thật. Worker này không gọi provider; `model-brief.json` là brief, không là response hoặc model đã nghiệm thu. Một request ban đầu, không tự gửi lại khi timeout hoặc trạng thái còn mơ hồ.

Đủ đưa ra review khi output thực đã nạp được, các mặt cần xem có geometry, vật liệu có nguồn và ba camera đang dùng cùng model. Các vấn đề về topology, vật dính/xuyên, contact, texture bake ánh sáng và khoảng chênh render/realtime phải ghi thành findings nếu quan sát được. Technical checks hoặc lời nhận xét của AI không chuyển trạng thái user approval. Chưa kiểm thì ghi chưa kiểm; không bịa human playtest, chất lượng production, collision, Unity hoặc gameplay acceptance.
