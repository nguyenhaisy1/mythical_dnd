# Thử nghiệm 13 — Thiết kế hình để nhìn ra một game

Sprint13 · SPIN-030 · 30/10/2026 · **Ý định mỹ thuật để thử, Pending user approval**.

Vòng này áp dụng [workshop đặt lại định nghĩa đồ họa](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md): thay cách tổ chức hình, tỷ lệ và camera trước khi thêm chất liệu. Chủ dự án giao tiếp tục tạo ý tưởng; lời ghi nhận tiến triển không phải quyết định chọn một hướng hoặc nghiệm thu Sprint13. Các bản01–12 vẫn được giữ để xem lại hoặc kết hợp.

## Câu hỏi cần trả lời

Ba cách nhìn có tạo ra những màn hình mà người xem hình dung được việc điều khiển một nhân vật, tiếp cận một người và hiểu một cuộc gặp không? Chúng có khác về cấu trúc hình thay vì chỉ khác bề mặt? Tạo hình thế giới, người và UI có còn thống nhất khi chuyển từ khoảng cách khám phá sang hội thoại?

Đầu ra của brief là **ba cặp ảnh**: mỗi hướng một frame thế giới nguyên khung và một frame đối thoại riêng. Không chia cảnh65/35, không ghép dải asset, không dùng số ô chân dung/icon như bằng chứng đã có một hệ đồ họa. Sáu frame là ảnh mô phỏng diện mạo game bằng built-in image_gen; không phải game đang chạy, scene được dựng, model3D, camera thực hay animation.

## Những yếu tố giữ chung để đối chiếu

**Tình huống:** ở một bến sông, một người thợ làm việc cạnh chiếc thuyền, có lối lên khu nhà sàn; một người mang hàng đi ngang. Nhân vật người chơi đến quan sát hoặc trò chuyện. Không tự thêm deadline, repair rule, quest effect hoặc kết quả hành động vào runtime.

**Cast minh họa:**

- Người phụ nữ khoảng40tuổi, thân chắc và vai rộng; áo/quấn màu đất đỏ, phần quấn chàm, băng đầu trắng ngà, tóc ngắn có vệt bạc, túi vải mang theo. Đây là ví dụ một cuộc đời được nhận ngẫu nhiên, không phải protagonist cố định hoặc màn hình chọn origin.
- Người thợ thuyền lớn tuổi, gầy, tóc xám/trắng, mặt vuông; vải màu tự nhiên và phần quấn xanh đậm. Giữ được sự khác nhau của hai người qua dáng, mặt và tư thế, không chỉ tóc hoặc một phụ kiện.
- Một người mang hàng trưởng thành làm rõ nhịp đời sống. Không cần thêm đám đông nếu làm mất trọng tâm người chơi/người thợ/lối đi.

Giữ vai, công việc, thuyền, lối đi và các mảng màu nhận diện càng ổn định càng tốt. Góc máy và grammar hình khác có chủ đích; các frame ảnh riêng vẫn có thể lệch cast, đồ vật hoặc bố cục dù có dùng PNG tham chiếu. Phản hồi ban đầu so được **tổ hợp**, chưa thể quy nguyên nhân chỉ cho camera,2D/3D hay yếu tố văn hóa. Không hứa sáu ảnh là các camera hoặc state của cùng geometry.

Các màu, phục trang và tỷ lệ này là lựa chọn mỹ thuật mới để kiểm tính nhận diện. Chúng chưa là bộ phục trang, thuốc nhuộm hoặc kiến trúc Văn Lang–Âu Lạc đã được xác minh.

## 1. Khối nhìn chéo — `block-world` / `block-dialogue`

**Giả thuyết:** một ngôn ngữ khối3D cách điệu rõ ở cả người và môi trường sẽ cho cảm giác game mạnh hơn khi chỉ vài đạo cụ có facet hoặc khi texture làm phần lớn công việc.

### Frame thế giới

Camera nhìn chéo từ cao vừa, trực giao hoặc phối cảnh nhẹ; một khoảng bến/sân và đường lên sàn nhà là phạm vi chính. Nhân vật người chơi có vị trí và hướng nhìn nhận được, không chiếm tiền cảnh như mẫu chụp chân dung. Đường tiếp cận người thợ và khoảng âm dưới sàn phải đọc được. Camera này không tự cấp cho người chơi thông tin NPC hoặc vật trong nhà mà nhân vật chưa biết.

Người có khoảng4–4,5nhịp đầu như **tham số thử tỷ lệ**, đầu/tay đủ lớn để thấy cử chỉ, mặt thành ít mặt phẳng, bàn tay thành khối đơn giản. Tuổi được diễn đạt bằng đường vai, lưng, mảng mặt và động tác; đầu lớn/chi ngắn không tự có nghĩa là trẻ con. Không dùng lỗ chân lông, nếp da li ti hoặc sợi vải để thay thiết kế người.

Mái là ít mảng có silhouette riêng; thân nhà, cột và sàn giữ cấu trúc rõ; thuyền có khối rỗng nhìn được; cây và địa hình dùng nhóm hình lớn. Bề mặt matte, ít cấp bóng, mép khối có chủ ý. Không gắn người giản lược vào một hậu cảnh mái rạ/lá cỏ/giỏ đan tả thực. Ánh sáng giữ mặt khối và bóng tiếp xúc đọc được; không dùng blur/ống kính để tạo cảm giác chụp mô hình.

UI trong frame thế giới chỉ cần một dấu cho biết trọng tâm quan sát/tương tác và thông tin ngắn có ích. Không mở đồng thời nhật ký, túi đồ và hội thoại như một màn hình quảng cáo các tính năng.

### Frame đối thoại

Một cuộc gặp cụ thể với người thợ: có speaker, một câu nói minh họa và các lựa chọn của người chơi tách rõ. Người thợ trong cảnh và portrait nhỏ nếu có phải giữ mặt vuông, tóc, màu và grammar khối. Người chơi vẫn có cảm giác đứng ở bến, không chuyển sang một tờ nhân vật trên nền trắng. Panel sạch và hình icon/đồ vật nếu xuất hiện cùng quy tắc mảng/khối với người và cảnh.

**Cần quan sát:** khối có đi qua cả mặt người, áo, thuyền, mái và cây không? Khi bỏ nhãn3D, người xem có nhận ra một không gian khối cách điệu? World view có đọc được vị trí người chơi và lối đi? Hội thoại có giữ đúng người thay vì một chân dung tả thực khác hẳn?

## 2. Dải sinh hoạt — `frieze-world` / `frieze-dialogue`

**Giả thuyết:** một thế giới2D rất phẳng, tổ chức thành các dải hoạt động và cao độ, có thể tạo một cách nhìn đời sống riêng thay cho panorama tự nhiên chỉ đổi sang góc ngang.

### Frame thế giới

Camera ngang hoặc hơi xiên, bố cục bằng lớp rõ: gầm/sàn nhà cao, lối đất giữa, bến và nước thấp. Thể hiện mặt cắt hoặc giản lược kiến trúc là **phép trình bày mỹ thuật mới**, không bản vẽ phục dựng. Tổ chức khoảng trống và lối tiếp cận theo tình huống; không biến mọi đường đi thành một hành lang gameplay đã chốt.

Nhân vật là silhouette góc cạnh, thân và tay thành mảng; hướng đầu, khuỷu tay, bàn tay và điểm tựa giúp đọc đang nghe, làm việc hoặc mang đồ. Giữ độ khác dáng của nữ vai rộng và người thợ gầy, không sao hai cơ thể tự nhiên rồi phủ một lớp nét lên. Mặt gần dùng cùng grammar hình với mẫu nhỏ, thêm vài nét có chủ ý cho tuổi và thái độ.

Nhà, thuyền, nước và cây là hình phẳng, bảng màu có vai trò, bóng hạn chế để phân lớp. Không có panorama sông–núi phối cảnh sâu, texture sợi rơm hoặc độ mờ tiền cảnh làm nền chính. Một nhịp cột–người–thuyền và khoảng trống có thể nối sinh hoạt, nhưng không bắt địa hình thành vòng tròn hoặc mọi người vào một đoàn nghi lễ.

### Frame đối thoại

Dáng người quay vào nhau cùng một dấu nối speaker đủ rõ; câu nói và lựa chọn nằm ở vùng chữ yên. Panel, portrait nếu có và icon dùng mảng/nét phẳng như cảnh. Tránh hộp thoại sao từ hướng3D khiến2Dchỉ còn là background thay thế. Không che tay/công cụ đang giúp hiểu người thợ và công việc.

**Cần quan sát:** có đọc được sàn/gầm/bến và nhóm hoạt động ở độ sâu nông không? Hình người và nhà có cùng mức giản lược? Bố cục có nhịp riêng ngay khi bỏ hoa văn? Người nói có rõ mà không cần bảng nhân vật khổng lồ?

## 3. Gần tầm người — `near-world` / `near-dialogue`

**Giả thuyết:** giữ grammar khối của hướng1nhưng kéo camera gần cơ thể có thể tăng cảm giác hiện diện, nếu góc nhìn cao vẫn bị đọc như tranh hoặc mô hình nhìn xa.

### Frame thế giới

Camera sau vai hoặc ba phần tư gần tầm người. Nhận ra người phụ nữ đang được nhập vai qua đầu/vai, áo đỏ, phần quấn chàm và túi; thuyền, người thợ, chân cột và lối lên sàn tạo quan hệ gần–xa rõ. Cự ly gần không phải lý do quay lại da, vải hoặc ánh sáng giống ảnh thật.

Dùng khoảng4–4,5nhịp đầu, bàn tay khối và mặt phẳng giản lược cùng hướng1. Mái, cột, tán cây và đồ dùng ở gần vẫn ít khối, texture rất tiết chế. Các mặt bên và lớp che khuất diễn đạt thể tích; không cần bokeh, sương điện ảnh, lens flare hoặc phản chiếu ảnh sản phẩm. Góc gần là biến thử chính, chưa có camera bám/né vật cản hoặc quyền đi vòng model.

### Frame đối thoại

Người thợ ở cự ly đủ đọc mặt và tư thế, giữ nhận diện như world frame. Có speaker và lời nói; lựa chọn nằm cùng ngôn ngữ UI của hướng1nhưng không cần chèn portrait nếu khuôn mặt trong cảnh đã đủ ích. Tránh biến NPC thành người mẫu tả thực đứng trước một thế giới hoạt hình. Cảnh vẫn gợi bến và vật đang làm, không đổi thành nền trang trí vô nghĩa.

**Cần quan sát:** khối ở mặt gần có thật sự cách điệu hay bị tăng chi tiết tự nhiên? Người xem có cảm giác đứng ở một cuộc gặp? Camera gần có che lối đi/người thợ hoặc làm mất ngữ cảnh công việc? Cặp frame có giữ tuổi, dáng và màu nhận diện không?

## Khác biệt phải kiểm so với các bản gần nhất

| Tham chiếu cũ | Thay đổi vòng13 cần thể hiện | Dấu hiệu vẫn chỉ lặp nhãn |
|---|---|---|
| 08C faceted,11diện mạo3D,12clay | Quy tắc ít khối/ít bóng và tỷ lệ phóng đại áp dụng đồng thời cho người, mặt, áo, nhà, cây và thuyền; camera ở cự ly chơi | Chỉ đá/lá có facet; người, vật liệu và ánh sáng vẫn như ảnh tự nhiên hoặc ảnh chụp miniature |
| 06C nhìn ngang | Dải hoạt động/cao độ và hình phẳng thay chiều sâu panorama; dáng góc cạnh, khoảng trống có vai trò | Vẫn phong cảnh sâu, áo/người/nhà tả thực, chỉ chuyển camera sang ngang |
| Các board11–12 | World frame nguyên khung và dialogue frame riêng; mỗi frame chỉ mang thông tin cần cho trạng thái đó | Lặp dải chân dung/icon dưới tranh hoặc mở nhiều bảng để đủ danh sách thành phần |
| Hướng1và3trong vòng này | Cố giữ grammar khối, cast, vật chính và màu nhận diện để so cự ly camera | Coi chúng là cùng một scene chính xác, hoặc quy mọi sở thích cho camera khi cast/ánh sáng/vật liệu cũng thay nhiều |

Các dòng là điều kiện của brief, chưa là kết luận về ảnh. Khi output không khớp, ghi đúng vị trí và mức lệch; không gọi toàn bộ ý tưởng kém hoặc loại bản cũ.

## Nguồn văn hóa → phép biến đổi → dấu hiệu cần thấy

Dùng [nghiên cứu ngữ pháp văn hóa của workshop](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md). Vòng này không tự xác minh thêm một bộ phục trang hoặc mặt bằng kiến trúc.

| Căn cứ tham khảo đã ghi trong workshop | Phép biến đổi mới của ta | Dấu hiệu trong frame |
|---|---|---|
| Hình mái/sàn và tổ chức hình trên tham chiếu Ngọc Lũ | Cô đọng mái–sàn–cột thành vài mảng và khoảng âm; hướng3Dgiữ khối, hướng2Dgiữ nhịp dải | Nhận mái, sàn và chỗ lên xuống khi thu nhỏ, không cần texture rơm. Kết cấu/tỷ lệ cụ thể vẫn là giả định mỹ thuật |
| Tư thế tương tác của tượng cặp người | Thiết kế động tác nghề bằng điểm tựa, tay và chuyển trọng tâm của người với vật/người khác | Đọc được làm việc, nghe hoặc mang đồ qua pose. Không khẳng định hiện vật miêu tả việc sửa thuyền |
| Nhịp hình học lặp qua các loại đồ dùng | Chuyển nhịp thành nhóm hoạt động, dấu chia và trật tự UI ở mức tiết chế | Nhịp còn khi bỏ hoa văn bề mặt; không đúc cả thế giới thành đồng, không rắc motif lên mọi nút |

“Chất thời đại” ở đây là giả thuyết sáng tạo về một thế giới có nghề và cộng đồng đang sống. Không mặc định cổ nghĩa là nâu xám, nghèo hoang sơ, patin xanh hoặc mọi người mặc trang phục nghi lễ. Màu sáng và tạo hình phóng đại được phép; độ phù hợp cuối cùng cần chủ dự án xem và quyết định.

## Cách đọc kết quả và giới hạn

Root lưu prompt thực tế, PNG gốc và provenance sau generation. Reviewer cần mở từng ảnh thật, phân biệt **ý định / output nhìn thấy / kiểm gallery / quyết định user**. Không dùng tên file hoặc nội dung prompt để tự chứng nhận ảnh đã đạt.

- Kiểm world frame ở kích thước xem dự kiến: ai được nhập vai, người thợ ở đâu, lối tiếp cận và việc đang diễn ra có rõ không?
- Kiểm đối thoại: ai nói, câu nói và lựa chọn có phân biệt được; mặt/cast, cảnh và UI có cùng grammar không? Chữ do generation tạo có thể sai hoặc quá nhỏ, cần ghi đúng.
- Đối chiếu silhouette, tỷ lệ, mảng sáng tối và mật độ chi tiết trước texture. Khối3Dgợi trong raster không chứng minh geometry; dải2Dkhông chứng minh parallax hoặc đường đi thực.
- Trong hai frame/hướng và giữa hai hướng3D, ghi các lệch nhân vật, vật, camera và ánh sáng thật. Không hứa giữ chính xác identity/geometry chỉ vì dùng chung brief.
- Không có bằng chứng ở vòng này về animation, rig, camera tự do, collision, pathing, shader runtime, hiệu năng, production cost hoặc human playtest.

UI raster chỉ minh họa trạng thái; gallery chỉ tương tác xem ảnh. Không thay random-origin, knowledge isolation, ownership/state, canon hoặc luật gameplay. Mọi sản phẩm tiếp tục **Pending user approval**; technical review không đóng Sprint13, merge dev hoặc mở Sprint14.
