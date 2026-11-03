# Thử nghiệm14 — Sáu hệ hình ảnh để mở rộng khoảng tìm kiếm

30/10/2026 · Sprint13 · SPIN-031 · **Brief trước output, Pending user approval**.

Chủ dự án nhận xét sáu ảnh vòng13 tốt nhưng còn gần nhau, thiếu khác biệt và bước tiến đủ lớn. Vòng14 chọn sáu hướng riêng, mỗi hướng là một màn hình chơi có thể tưởng tượng được. Các hướng cùng thay camera, cấu trúc hình, tỷ lệ, bối cảnh, cast và UI. Đây là phép tìm kiếm rộng, không phải thử nghiệm chỉ đổi một biến; một ảnh được thích chưa cho biết nguyên nhân là palette, camera hay chất văn hóa.

Root tạo ảnh mới bằng built-in image_gen, không dùng ảnh13 làm tham chiếu để tránh tự giữ lại cùng bố cục và tạo hình. Văn bản này ghi **ý định và điều cần quan sát**, chưa nhận artifact đã được tạo hoặc đạt. Không có model3D, scene, camera, shader, gameplay hay UI runtime trong brief này. Giữ toàn bộ01–13 cùng workshop để quay lại hoặc kết hợp; nhận xét mới không tự loại bản cũ.

## Bài toán khác vòng13 ở đâu?

Vòng13 giữ cùng bến sông, nhóm nhân vật, màu áo và họ UI để so cự ly/camera. Điều đó hữu ích cho một câu hỏi hẹp nhưng cũng giữ các ảnh trong một vùng thị giác. Vòng14 chủ động mở các trục khác:

| Key / hướng | Không gian và camera dự kiến | Hình người / vật liệu dự kiến | Bối cảnh và ngôn ngữ UI dự kiến |
|---|---|---|---|
| `earth-monument` — Đất dựng thành | Quy mô thành đất, khối lớn góc cạnh, nhìn chéo mạnh | Người dài, khoảng5–7nhịp đầu; mặt phẳng matte, ít mảng sáng tối | Loa Vực; oxide/bone/charcoal; dải UI đứng, dấu chọn rõ |
| `living-grid` — Ô đất sống | Mặt bằng2D nhìn thẳng90độ; không horizon, không isometric | Người/vật thành hình tròn–vuông và footprint đọc từ trên | Hải Môn; nước ultramarine, nền kem, nhấn đỏ; UI mô-đun sạch |
| `round-world` — Làng vòng cung | Cảnh đồi với phép nhìn cuộn/cong, cảm giác không gian mềm | Người trưởng thành khoảng2,5nhịp đầu; khối tròn trơn, rất ít chi tiết | Nhà chữa trị vùng Tản Viên; celadon/lilac/cream; UI nhẹ và tròn |
| `craft-first-person` — Đôi tay làm nghề | Góc nhìn thứ nhất qua tay và bàn làm việc | Đồ họa2D in màu phẳng, cử chỉ lớn, đường chéo khỏe | Xưởng Đồng Đỏ; deepblue/white/kilnorange; các động từ xếp dọc |
| `glyph-world` — Ký hình | Cả địa bàn được diễn đạt bằng cụm ký hiệu2D | Người, đồ, chân dung và UI cùng hệ pictogram | Đầm Đại Trạch; black/ivory/red; thông tin cô đọng theo nhóm dấu |
| `comic-life` — Mái nhà nhiều giọng | Trang truyện là màn hình chơi, chia khung theo cuộc gặp | Nét mực dao động và mảng in riso; dáng người dài, biểu cảm mạnh | Nội thất gia đình ba thế hệ; violet/acidgreen/ivory/coral; lựa chọn thành ô lời/nhãn truyện |

Các tỷ lệ và màu trên là tham số thử mỹ thuật, không là production budget, chuẩn khảo cổ hoặc điều kiện auto-pass. Không bắt mọi hướng có cùng số portrait/icon hoặc cùng panel đối thoại. Riêng trang truyện có khung vì đó là đề xuất presentation của game; không áp lệnh cấm split-panel của các ảnh gameplay đơn trước đây vào ý tưởng này.

## 1. Đất dựng thành — `earth-monument`

**Giả thuyết:** khối đất, khoảng trống và quy mô công trình có thể tạo một thế giới nhập vai mạnh về hình học, trong đó con người dài, sắc và có trọng lượng; tránh quay lại làng xanh/vàng với nhân vật tròn của vòng13.

Một đường đi bám công trình đất đắp tại Loa Vực là điểm quan sát chính. Tường, dốc, hào và mặt đất được tổ chức thành vài khối lớn; cạnh cắt và bóng tạo hướng. Người vẫn là nhân vật được nhập vai, không là chấm lao động để người chơi điều quân hoặc ra lệnh xây thành. Khối đất có thể rất phóng đại nhưng phải cho thấy nơi chân đứng và lối tiếp cận.

Cast thử tỷ lệ khoảng5–7nhịp đầu, tay/chân dài, mặt thành vài mặt phẳng, dáng khác nhau theo tuổi và việc. Palette oxide, xương sáng và than làm phân cấp lớn; không dùng texture đất/da chi li để chứng minh chất lượng. UI dải đứng dùng icon có góc và khoảng trắng cùng nhịp hình, không bê lại hàng nút tròn navy/kem của13.

**Cần nhìn thấy:** khi bỏ màu, mặt khối, người dài và nhịp dốc vẫn khác13; nhân vật chính/lối đi/đối tượng tiếp cận nhận được. **Nguy cơ:** công trình thành fantasy bê tông hoặc giao diện chiến lược toàn tri; nhân vật quá nhỏ mất cuộc đời riêng; palette chỉ làm cảnh cũ thành đỏ. Hình khối thành đất là diễn giải sáng tạo, không là bản phục dựng mặt bằng Cổ Loa.

## 2. Ô đất sống — `living-grid`

**Giả thuyết:** một mặt bằng2D rất cô đọng có thể cho cảm giác khám phá địa bàn và gặp người, không cần phối cảnh tự nhiên hay thêm cạnh đa giác lên tranh.

Dùng tình huống lao động/trao đổi quanh các ô đất, nước và lối qua vùng Hải Môn. Camera nhìn thẳng xuống90độ; không thấy mặt đứng nhà theo phối cảnh, không núi/horizon. Nước, nền, mái và đồ nghề thành các hình phân biệt được; người nhận bằng đầu–vai, đồ mang, hướng và dáng footprint. Một khu vực cục bộ có một protagonist rõ, không map điều hành toàn settlement.

Palette nước ultramarine/nền kem/điểm đỏ có vai trò; UI mô-đun gọn dùng cùng hình tròn–vuông với chân dung/icon. Một lời trò chuyện hoặc nhãn quan sát chỉ trình bày thông tin mẫu đã biết. Bản đồ không tự mở quan hệ, kho hàng hoặc vị trí bí mật của mọi NPC.

**Cần nhìn thấy:** kiến trúc/địa hình và người thật sự được thiết kế lại cho plan view, không thu nhỏ một ảnh isometric; nghề và lựa chọn hiện bằng nhóm đồ/hành động. **Nguy cơ:** thành bản đồ quản lý hoặc puzzle vô danh, người mất tuổi/cá tính; việc giản lược chỉ còn màu ô. Bố trí ô sản xuất và công cụ là proposal, chưa xác minh kỹ thuật làm muối lịch sử hoặc thêm gameplay kinh tế.

## 3. Làng vòng cung — `round-world`

**Giả thuyết:** một camera gợi mặt đất cuộn và bộ khối tròn có thể tạo sự hiện diện khác hẳn khối góc cạnh lẫn dải ngang, trong khi nhân vật vẫn có tuổi và nghĩa vụ.

Thử khoảng sân nhà chữa trị vùng đồi Tản Viên. Cảnh có bờ, lối, cây và mái uốn quanh trường nhìn; khối môi trường tròn, đơn giản, không vết sợi hay mặt đa giác nhỏ. Người trưởng thành khoảng2,5nhịp đầu có mũi, lông mày, tóc và tư thế riêng; đầu lớn/chi ngắn không được tự đọc thành nhân vật trẻ em. Tuổi cần thể hiện bằng thiết kế và hành động, không thêm da tả thực để cứu tỷ lệ.

Màu celadon/lilac/cream cùng nền sáng thử cảm giác mát và nhẹ. UI tròn, sáng, khoảng thở lớn; icon đồ chữa/đồ mang cùng hệ khối mềm. Độ cong là phép nhìn/presentation đề xuất, **không phải một hành tinh nhỏ thật, thay đổi địa lý thế giới hay cơ chế ma thuật**.

**Cần nhìn thấy:** đường cong của trường nhìn và khối mềm xuyên người/cây/nhà/UI; hình vẫn rõ ở cỡ chơi. **Nguy cơ:** bị hiểu thành ảnh mô hình đồ chơi trên đế, nhân vật cùng một mặt, camera làm lối đi khó đọc; palette đổi mà hình vẫn như13. Chưa có bằng chứng camera động, collision trên mặt cong hoặc hệ chữa trị mới.

## 4. Đôi tay làm nghề — `craft-first-person`

**Giả thuyết:** đặt bàn tay và vật đang làm vào điểm nhìn chính có thể đưa nghề nghiệp thành trải nghiệm thị giác trực tiếp, không cần biến RPG thành hình nhân vật đứng trước phong cảnh.

Góc nhìn thứ nhất qua đôi tay, công cụ và bàn làm việc trong xưởng Đồng Đỏ. Một người thợ khác có mặt để giữ yếu tố xã hội; tương tác có thể là quan sát, hỏi hoặc dừng công việc. Các lựa chọn là minh họa của khung, không là quyền tự triển khai một minigame chế tác hoặc một action-handler mới.

Ngôn ngữ in2D phẳng dùng tay/công cụ lớn, đường chéo và khoảng âm khỏe; deepblue, trắng và kilnorange chia nhóm rõ. Không có ảnh tay thật đặt lên nền render, ánh lửa điện ảnh hoặc hạt bắn chi tiết. UI động từ xếp dọc có nhịp góc cạnh/stencil, icon là hình công cụ cùng nét. Chân dung hoặc biểu cảm người đối diện, nếu xuất hiện, phải cùng hệ hình; không ép thêm một dải nhân vật toàn thân.

**Cần nhìn thấy:** tay có pose, điểm tựa và quan hệ với vật; vùng việc và vùng lựa chọn tách nhau; người thợ đối diện vẫn là một con người. **Nguy cơ:** chỉ còn ảnh quảng cáo nghề, tay quá lớn che công việc, toàn UI nói về thông số lò hoặc craft chưa được đặc tả. Quy trình, hình công cụ và nguồn nhiệt cụ thể là mỹ thuật hư cấu cần nguồn riêng nếu phát triển tiếp.

## 5. Ký hình — `glyph-world`

**Giả thuyết:** thế giới có thể được nhìn qua một bộ dấu hình tự thiết kế, tạo game đọc bằng nhịp dấu và khoảng trống thay vì cảnh vẽ tự nhiên. Cần đổi cả cấu trúc thế giới, không chỉ chuyển tranh sang đen/trắng.

Một địa bàn nhỏ ở đầm Đại Trạch được ghép bằng cụm dấu: lối, bờ, sậy, nước, thuyền, người và vật dụng có quy tắc nhất quán. Hình người và công việc còn nhận được qua tổ hợp dấu; protagonist được phân biệt rõ. Chân dung, đồ mang và UI dùng cùng bộ phận tạo hình, không đặt portrait tả thực vào một map ký hiệu.

Black/ivory/red chia nền, hình và lựa chọn. Chữ tiếng Việt phục vụ đọc game có thể nằm ở vùng yên; pictogram là thiết kế đương đại, **không tuyên bố đã phát hiện hoặc tái hiện chữ viết Văn Lang–Âu Lạc**. Dấu gợi người/vật được biết, không biến mọi bí mật thế giới thành mã để người chơi đọc từ xa.

**Cần nhìn thấy:** toàn cảnh được cấu tạo bằng dấu có công năng, khác khắc gỗ/than/mực của các vòng cũ; icon/UI/portrait không đổi hệ. **Nguy cơ:** thành terminal kỹ thuật hoặc bản trang trí khó hiểu, mọi người như nhau, quá nhiều ký hiệu phải học, đơn sắc nhưng bố cục cũ. Chưa có font, grammar ký hiệu, parser hoặc cơ chế tri thức production.

## 6. Mái nhà nhiều giọng — `comic-life`

**Giả thuyết:** một trang truyện có lựa chọn có thể làm gia đình, quan hệ và nhịp lắng nghe thành cấu trúc màn hình chơi, thay vì chỉ làm portrait đẹp trong một panel hội thoại quen.

Bối cảnh là nội thất một household có ba thế hệ cùng hiện diện. Một khung lớn cho không gian/động tác chung, các khung nhỏ có thể dẫn cận mặt hoặc lượt nói. Chúng là đề xuất cách game trình bày một cuộc gặp, không là layout trưng asset, storyboard trailer hoặc nhiều concept không liên quan trên một tờ.

Nét mực dao động, dáng người dài và biểu cảm mạnh; màu riso violet/acidgreen/ivory/coral dùng theo vai trò. Nền, đồ dùng, người và caption có chung nét/mảng. Lựa chọn có thể nằm trong nhãn/ô lời với người nói và thứ tự đọc rõ. Hành vi chọn một câu chưa được triển khai; nhiều ô không tự chứng minh sequence đúng, thời gian đã tiến hoặc world state thay đổi.

**Cần nhìn thấy:** mắt đi qua không gian–người nói–lựa chọn có chủ ý; ba thế hệ khác nhau bằng dáng/mặt, không chỉ chữ ghi tuổi. **Nguy cơ:** thành poster truyện hoặc giao diện quá dày chữ, trình tự mơ hồ, các panel làm lộ suy nghĩ/bí mật người khác. Đây là một hướng mới cần kiểm cách nối khám phá với trang xã hội sau này, chưa tự chuyển toàn game sang visual novel.

## Căn cứ văn hóa và phép biến đổi

Nguồn của vòng này là [workshop định hướng](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [nghiên cứu ngữ pháp văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md). Không có nghiên cứu khảo cổ mới hoặc phân tích ảnh hiện vật mới trong task viết brief này.

- Khối đất/hào và tổ chức lao động gợi bài toán hình cho Đất dựng thành; phối cảnh, tỷ lệ, mặt cắt và palette đều là lựa chọn của ta.
- Nhịp hình học, người, công cụ và nhóm sinh hoạt được chuyển thành footprint/dấu/hệ UI cho Ô đất sống và Ký hình; không sao hoa văn thành bảng chữ cổ.
- Dáng tương tác, điểm tựa và chuyển trọng tâm gợi cách đặt tay/người trong Đôi tay làm nghề và Mái nhà nhiều giọng. Chúng không phải hành động được hiện vật hoặc GDD xác nhận chính xác từng pose.
- Làng vòng cung thay phép nhìn và tỷ lệ, không thay địa lý/cosmology. Các vùng được mượn làm bối cảnh mở rộng thư viện, chưa thêm settlement, NPC, recipe hoặc event vào canon.

## Cách nhận xét sau khi có ảnh

Xem thực tế ở cỡ hiển thị dự kiến, rồi ghi cho từng artifact: camera có thật sự khác không; hình người/vật có cùng quy tắc; người chính/công việc/người nói/lựa chọn có đọc được; UI có bản sắc riêng mà vẫn rõ; dấu hiệu văn hóa nào hiện thành hình và phần nào vẫn chung chung. Kiểm các trường hợp dễ lặp: đổi màu nhưng giữ cảnh, đổi texture nhưng giữ người, khác bối cảnh nhưng cùng panel, hoặc đặt nhiều nhãn mà cấu trúc chơi chưa đổi.

Không dùng đủ sáu file làm bằng chứng khác biệt. Không bịa điểm số, benchmark, chi phí, feedback playtest hoặc sự hài lòng của chủ dự án. Root lưu prompt/provenance/ảnh nguyên bytes và observations; hai critic kiểm artifact đã có với phạm vi thật. Một frame mỗi hướng chưa chứng minh đầy đủ mọi màn hình, cast, animation hoặc asset toàn game; nếu có hướng được muốn đào sâu, kiểm tính xuyên suốt bằng artifact thích hợp ở lượt được giao tiếp.

Random-origin, no-runtime-LLM, deterministic/causal rules, knowledge isolation và owner boundaries giữ nguyên. Mọi hướng tiếp tục **Pending user approval**; technical review không tự chốt mỹ thuật, đóngSprint13, merge dev hoặc mởSprint14.
