# Brainstorm18 — Nhân vật, vật hữu dụng và giao diện

02/11/2026 · Sprint13 / SPIN-035 · Actor `/root/s13e18_cast` · **Đề xuất để thử ảnh; Pending user approval**.

## Phạm vi và bài học đầu vào

Đã đọc .agent, HANDOFF, baseline áp dụng, mythical-prototype/art-direction và phản biện hình ảnh17; đối chiếu chuỗi prompt pixel/anime11. Đây là audit từ source/brief/review; trong nhiệm vụ này chưa mở lại PNG cũ để tự nhận một lượt review hình độc lập. Những quan sát về texture, tuổi và nét mặt17 bên dưới lấy từ hồ sơ đã ghi, không gán thành phản hồi mới của chủ dự án.

Vòng17 còn texture và chiều sâu thường ở nhiều hướng; Vành sân v1 trượt về tả thực, v2 mất nét mặt NPC; pixel có tóc xám nhưng portrait còn trẻ. Vòng16 từng mơ hồ portrait thuộc player hay speaker. Bài học không phải ép mọi người có khuôn mặt nhiều chi tiết: cần vài dấu mặt có chủ đích, tư thế và bàn tay cùng truyền tuổi/nghề, trong khi hình đủ gọn ở khoảng cách chơi. Tóc trắng đơn lẻ không chứng minh tuổi già.

Mỗi đề xuất dưới cần đổi hình người, vật và UI cùng lúc, không chỉ đổi shader/chất liệu. Cast là ví dụ một cuộc đời được nhận ngẫu nhiên; không character creator, protagonist cố định hoặc production roster đã chốt. Một frame không chứng minh toàn bộ cast/UI/animation.

## Sáu gói tạo hình được đề xuất

| Key / tên | Quy tắc thân người và mặt | Thế giới / vật / UI cùng họ | Delta và lỗi cần tránh |
|---|---|---|---|
| `cel-adult` — Nét mặt ba sắc | Thân gợi3D chia rất ít mảng sáng–trung–tối; đầu theo tỷ lệ người lớn, mắt/mày/mũi và hai nét khóe mặt đủ đọc biểu cảm/tuổi | Cột, mái, vật cầm cũng dùng mảng sáng tối quyết định; portrait cùng tuổi/tóc/áo với người có bracket; UI là chip phẳng cạnh rõ, vùng chữ yên | Có tiền lệ anime11. Thử đẩy độ gọn và biểu cảm nhìn ở cự ly game, không mặc định anime nghĩa là trẻ/đẹp đồng khuôn hoặc mặt vô diện; tránh da/fabric có hàng trăm nét |
| `soft-mass` — Khối liền mềm | Torso, tay và chân là vài thể tích bo liền, không khớp/rod; đầu nhỏ so với thân trưởng thành, bàn tay lớn, lưng/khối vai phân biệt già–trẻ và thể trạng | Nhà/đồ là vài khối nối rõ; portrait mềm nhưng có mắt–mày/sống mũi/vết má; hai thùy UI lớn với mặt chữ phẳng | Là nhánh Khối liền mềm từng được giữ ở brainstorm17 nhưng chưa vào batch ảnh. Không tuyên bố chưa từng có chất khối mềm:12clay/14round/15rolling có tiền lệ. Tránh chibi, texture đất nặn hoặc con rối khớp16 |
| `ink-mass` — Mực đặc |2D dùng silhouette đặc và khoảng rỗng ở khuỷu/lưng để đọc cử chỉ; nét nội bộ ưu tiên mặt và vật hữu dụng, không vạch mọi nếp áo | Cảnh cũng là mảng đen–trắng có thứ tự che khuất; portrait đọc bằng vài dấu mặt; UI âm/dương có nền chữ đứng và icon silhouette riêng | Có tiền lệ bóng rối10, charcoal12 và mảng phẳng17. Delta cần nằm ở hình đặc–rỗng và thứ bậc nét, không đổi cảnh thành đen/trắng hoặc phủ grain/khắc vân |
| `ribbon-body` — Người bằng dải | Dải rộng thực sự cấu tạo thân, cánh tay và chân, có bề rộng/che khuất; mặt vẫn là mảng đặc với vài nét tuổi | Cảnh và đồ dùng cũng có nhịp dải lớn; portrait giữ cấu tạo dải quanh một mặt đọc được; UI dùng đầu dải trơn, không bắt chữ uốn theo mọi nếp | Đối chiếu dệt16 chưa xuyên toàn thân. Tránh hình người thường chỉ quấn thêm vải hoặc texture đan; không suy hình dải thành ma thuật tháo/gập cơ thể |
| `contour-step` — Đường đồng mức sống | Vài lớp cao độ có cạnh và che khuất tạo thể tích cơ thể, cùng cỡ bậc; mặt có vùng đặc để giữ mắt/mày/mũi và tuổi | Bậc/lớp xuyên nhà, đường, vật cầm và portrait; UI dùng hai khoang bậc có mặt chữ phẳng | Khác voxel16 ở lớp uốn liền, khác relief17 ở logic cao độ xuyên mọi hình. Tránh chỉ vẽ contour trang trí, lớp quét nhìn xuyên/x-ray hoặc biến da thành bản đồ kỹ thuật |
| `square-gesture` — Pixel hình lớn | Cụm vuông màu đặc lớn tạo dáng cơ thể và vật; tuổi cần tư thế/khóe mặt/bàn tay, không chỉ tóc xám | Cảnh ít cụm nhỏ, vật có silhouette lớn; portrait cùng cỡ cụm và tuổi, UI viền pixel nhưng chữ Việt đủ dấu | Pixel07/11/17 có tiền lệ. Thử camera và nhịp cụm lớn, không chỉ tăng pixel filter. Tránh density17, voxel3D, hatching/dither đồng đều hoặc mọi người thành cùng sprite nhỏ |

Các mô tả là intent để root tạo ảnh, không báo sáu grammar đã đạt. Không đo palette count, lưới pixel, tuổi rõ hay chi phí production từ tên prompt.

## Quy ước kiểm UI/cast cho mọi hướng

Portrait thuộc người đang nhập vai và khớp những dấu nhìn thấy ở full body; không dùng portrait NPC rồi vô tình làm nó giống player. Bracket góc vuông dưới chân player khác hình bubble có đuôi ở NPC. Thêm dấu phụ ngoài màu để phân biệt; nếu không có marker/face trong output phải ghi thiếu, không tự coi đã đạt vì prompt có yêu cầu.

Hành động và useful-object icon phải liên hệ đúng vật đang cầm: bát không tự là giỏ, cuộn chiếu không tự là túi. Hai nhãn ngắn ở mặt UI trơn có thể đủ cho lượt chọn mỹ thuật; chữ trong PNG chưa là text component hoặc interaction. UI không cung cấp hidden truth, lịch riêng của NPC hay chỉ số engine. Không thêm mini-map để lấp khoảng trống.

## Trao đổi chéo thực tế và quyết định phạm vi

- `/root/s13e18_space` gửi sáu phép nhìn: Sân chéo mở, Ba dải đường, Qua vai người thợ, Đường chéo dài, Rừng là thân cột và Sân mặt bằng–người mặt đứng. Cast phản hồi: sân mở cần khoảng âm/ít mái; ba dải cần che khuất công cộng; hybrid phải quy ước cả profile body/prop/shadow, không một front sprite bất chợt trên mặt bằng90°.
- Space góp ý trở lại: cel có thể phối với gần qua vai để đọc mặt; ribbon có thể phối hybrid; ink có thể phối rừng cột; contour có thể phối đường chéo. Đây là khả năng kết hợp trong nghiên cứu, chưa có ảnh chứng minh các combo hoặc quyết định user.
- `/root/s13e18_audit` đưa các gói Cel ba sắc, Nét đen–mảng trắng, Một-bit có tuổi, Khối liền mềm, Sân khấu một nền và Khối răng cưa. Audit/cast nhận ra ba cặp trùng cel/soft/ink và thống nhất đề nghị root gộp mỗi cặp thành một gói, không đếm thành sáu kỹ thuật mới.
- Audit phản biện ribbon phải cấu tạo silhouette/tay và có mặt đặc; contour cần cùng cỡ bậc xuyên body/world, lớp trên che lớp dưới. Cast phản hồi khác biệt giữa pixel màu và one-bit phải ở cả cụm khối màu đặc so với silhouette/khoảng rỗng/stipple được tiết chế, không chỉ thay bảng màu. Retro facet cần bước cạnh ở silhouette, không phủ dither lên cảnh tả thực.
- `/root` chọn batch15 gồm sáu space, sáu cast này và ba gói audit còn riêng: `one-bit`, `single-stage`, `retro-facet`. Đây là quyết định phạm vi tạo ảnh trong lượt18, chưa chọn style cuối. Các đề xuất/khả năng chưa vào batch tiếp tục được giữ để nghiên cứu.

## Ranh giới và bước kiểm tiếp

Chỉ ảnh mô phỏng game, không model, scene/render3D thật, camera động, engine, shader production hoặc Hyper3D. Các cách dải/bậc/khối không tự tạo luật vật lý, route hoặc quyền thấy xuyên vách. Trang phục, mái, công cụ và motif là diễn giải/hư cấu mỹ thuật cần nguồn khi phát triển sâu; không chứng nhận niên đại Văn Lang–Âu Lạc từ palette hoặc đạo cụ.

Root tạo ảnh rồi hai critic kiểm nguồn/gallery và artwork theo phạm vi thật; khi có output mới đánh giá cử chỉ, mặt, tuổi, vật, marker và UI. Mọi01–18, biến thể và các đề xuất tiếp tục trong thư viện tham khảo; ưu tiên14/1–3 không tự là final approval. Sprint13 và các mục chưa chốt vẫn Pending user approval, không merge dev hoặc chuyển Sprint14.
