# Phản biện hình ảnh14 — sáu cách biểu đạt độc lập

30/10/2026 · Sprint13 / SPIN-031 · Reviewer độc lập `/root/s13e14_critic_visual` · **Review hình ảnh/UX minh họa, Pending user approval**.

Vòng14 mở rộng khác biệt về cấu trúc hình, điểm nhìn, cast, cảnh và giao diện. Sáu original đã cho thấy các cách biểu đạt khác nhau, không chỉ sáu màu của cùng bến sông. Tuy vậy, “vượt bậc”, phù hợp sở thích hoặc đủ làm đồ họa xuyên game vẫn là câu hỏi cần chủ dự án đánh giá; review không dùng số ảnh hay số agent đồng ý để chứng nhận điều đó.

## Phạm vi thực sự đã kiểm

Đã đọc hướng dẫn repository, `.agent`, HANDOFF, baseline áp dụng, skill mythical-validation/game-design-review, workshop `ART-DIRECTION-RETHINK.md`, brief14, prompts và provenance. Đã dùng `view_image` mở trực tiếp sáu PNG tại `prototypes/sprint13/experiment-14/assets/`:

- `earth-monument.png`, `living-grid.png`, `round-world.png`.
- `craft-first-person.png`, `glyph-world.png`, `comic-life.png`.

Đã mở thêm `prototypes/sprint13/experiment-13/assets/block-world.png` và `frieze-world.png` để đối chiếu. Không tuyên bố đã xem lại toàn bộ01–13 hoặc đã kiểm mọi ảnh tham chiếu văn hóa.

Sau khi code critic báo lượt browser cuối hoàn tất, đã thực sự mở ba screenshot: `tests/results/sprint13-experiment14/gallery-comic-life-1280.png`, `gallery-glyph-world-390.png` và `text150-390.png`. Các screenshot cho thấy đủ sáu thumbnail, các ghi chú quan sát, điều khiển và thư viện01–13 trong khung xem. Reviewer không tự chạy browser, OCR hoặc đo contrast; không nhận automation của code critic thành lượt kiểm của mình. Kiểm hash/original/PNG decode thuộc hồ sơ code critic riêng.

## Khác biệt đã nhìn thấy

| Hướng | Cách đọc cảnh/người khác nhau | Sự thống nhất với UI và giới hạn |
|---|---|---|
| Đất dựng thành | Khoảng công trường rất lớn với đường dốc, hố sâu, giàn và khối đất; thân người dài/góc cạnh, bóng đổ mạnh. Nhân vật mang vò có dấu chọn và tiếp cận một người thợ; vẫn có cuộc gặp ở quy mô cá nhân giữa đám lao động. | Dải thao tác đứng, portrait và icon sắc cạnh hợp với mảng đen/xương/đỏ đất của cảnh. Cảnh còn texture đất/gỗ, nhiều mặt nhỏ và người nền, chưa chỉ vài khối/ba màu thuần. Lời gọi trong bong bóng do ảnh thêm ngoài các nhãn ngắn; không nhập thành script/canon. Quy mô hố và công trình là hư cấu mỹ thuật, không chứng nhận mặt bằng Cổ Loa. |
| Ô đất sống | Ruộng muối và đường bờ tạo mạng lối đi nhìn từ cao, một người mang rổ được đánh dấu góc đỏ; nhịp cảnh dựa vào mặt bằng và khoảng trắng thay cho mái nhà/nhân vật lớn tiền cảnh. | Portrait, vật mang và UI trắng viền xanh có hình thức riêng, đọc rõ ở original. Người, mái và vật vẫn cho thấy mặt xiên/bóng/khối; mặt ruộng và đất có texture, nên chưa là phép chiếu90độ hoặc2D hình học tuyệt đối của brief. Không suy mặt bằng này thành kỹ thuật làm muối lịch sử đã xác minh. |
| Làng vòng cung | Khối cây/mái/người tròn, ông già thấp với đầu và mũi lớn, người nhận bó lá cao hơn; khoảng sân chữa trị và khung đồi mềm tạo cảm giác khác họ góc cạnh13 và mẫu thành đất. | Portrait ông già, icon tròn và hai đĩa thao tác sáng đi cùng hình mềm. Có lời chào và cử chỉ nhận đồ. Tuy nhiên, cảnh chủ yếu đọc như đường đồi uốn trong phối cảnh; chưa thấy rõ một trường nhìn cuộn/cong. Đồ tre, cây nhỏ, mái và ánh sáng vẫn nhiều chi tiết hơn ý định rất ít texture. Tỷ lệ giữa hai người cũng không đồng nhất một công thức2,5nhịp đầu. |
| Đôi tay làm nghề | Góc nhìn qua hai tay và mặt bàn là thay đổi trực tiếp về vị trí người chơi; người thợ đối diện, khuôn/công cụ và tay tạo trọng tâm nghề nghiệp. Không còn một hình avatar đứng xa trong panorama. | Mảng cobalt/cam/đen/xương, chữ động từ đứng và các icon công cụ cùng hình cắt mạnh. Mặt người, bàn tay và nền cùng hệ in đồ họa; đây là khác biệt cấu trúc, không chỉ palette. Vệt in/hạt/nét nền vẫn dày; các icon chỉ điểm cho đối tượng, chưa chứng minh chức năng hay quy trình chế tác nào đã hoạt động. |
| Ký hình | Nhà, thuyền, sậy, nước, người và đường đều được cấu tạo bằng dấu hình lớn. Nhân vật đỏ tiếp cận người mang bó sậy trên một chỗ giao; nền ít mô phỏng vật liệu, đổi cách người xem giải mã không gian. | Portrait, icon và biên UI cùng bộ đen/xương/dấu đỏ. Mái nhà và người dùng hình trắc diện/ba phần tư trong một bố cục địa bàn, không phải top-down thuần. Đỏ còn có ở sậy nên chưa là tín hiệu riêng của protagonist; dấu người đỏ vẫn phân biệt được như một hình người, không phải hoàn toàn mất nhận diện. Đây là hệ đồ họa mới, không chữ viết cổ. |
| Mái nhà nhiều giọng | Một khung lớn nhìn nhóm gia đình, một khung cận mặt và một khung trao bát làm cuộc gặp trở thành cấu trúc màn hình. Người tóc bạc, người nam và người nữ trẻ có nét mặt/tư thế khác; cảm xúc và lắng nghe được nhấn hơn cảnh ngoài trời. | Caption, nét mặt, đồ vật và icon ở viền cùng mực tím/lime/coral. Ba lựa chọn có vùng chữ yên riêng; các khung liên hệ cùng tình huống, không phải catalogue asset. Nền, quần áo và mái rất dày nét, tranh nhau trong khung lớn. Một ảnh chưa chứng minh trình tự đọc, nhịp chuyển panel hoặc tương tác; số người có tín hiệu tuổi không tự chứng minh roster hoặc ba thế hệ canon đã hoàn chỉnh. |

So với hai ảnh13 đã xem, sáu mẫu14 không còn buộc cùng nhóm người, dáng thuyền, mặt nhà, dải màu hay vị trí UI. Trong chính14, hai hướng có vẻ3D khác nhau ở tỷ lệ/đường nét/giá trị sáng tối; các hướng nhìn từ trên, nhìn qua tay, ký hình và truyện khung còn đổi cách tổ chức thông tin. Vì thế có căn cứ để nói khoảng tìm kiếm đã rộng hơn. Không thể từ phép so nhiều biến này kết luận một yếu tố riêng làm người xem thích hơn.

Một số chất liệu có họ hàng với ý tưởng cũ như nét in hoặc ký hiệu; điều khác được quan sát ở đây là cách dùng toàn cảnh và UI. Không cần tuyên bố mỗi hướng hoàn toàn chưa từng xuất hiện trong lịch sử thư viện mới có giá trị tham khảo. Không xếp hạng, loại bản hoặc nhận một quan sát tích cực thành phản hồi của chủ dự án.

## Findings và câu hỏi cần giữ khi đào sâu

### P2 — camera được đặt tên chưa phải camera đã được thể hiện đầy đủ

`round-world` đã có khối tròn nhưng chưa thể hiện rõ rolling/curved camera; đường đi uốn và đồi xa không đủ chứng minh phép nhìn đó. `living-grid` và `glyph-world` đọc địa bàn từ trên nhưng còn trộn hình chiếu, nên không gọi là planview90độ đã đạt. Gallery notes đã nêu các giới hạn này. Khi có yêu cầu đào sâu, có thể tách riêng câu hỏi camera khỏi chất liệu bằng ảnh đối chiếu; không cần tạo model thật làm điều kiện ở bước chọn ý tưởng.

### P2 — Ký hình chưa có mã chọn protagonist chỉ dựa vào đỏ

Vị trí: sậy đỏ rải nhiều nơi quanh người đỏ ở giữa `glyph-world.png`. Màu đỏ cùng tham gia trang trí/môi trường và nhận diện nhân vật, khác yêu cầu chỉ đánh dấu người chơi. Nếu phát triển hướng này, cần thêm dấu hình hoặc trạng thái chọn riêng, không yêu cầu người chơi học “mọi thứ đỏ đều tương tác”. Đây là vấn đề về quy tắc đọc, chưa là lỗi input/runtime và không chặn xem concept hiện tại.

### P2 — mật độ hình và tín hiệu tương tác cần được kiểm trong từng hướng

Đất dựng thành, Đôi tay làm nghề và Mái nhà nhiều giọng vẫn dày texture/nét/đồ nền hơn các mức giản lược được đề xuất. Ở craft, chữ lớn và mặt/tay còn nổi; trong comic, vùng caption/lựa chọn yên hơn cảnh nên đọc được lời chính ở original. Tuy nhiên, khi thu nhỏ, nhiều vật và chi tiết mất phân cấp. Không dùng việc tất cả đã khác palette/camera làm bằng chứng readability trong chuyển động đã đạt.

Không yêu cầu sáu hướng dùng chung HUD; điều đó sẽ triệt tiêu mục tiêu khác biệt của lượt này. Thay vào đó, nếu một hướng được muốn đào sâu, cần kiểm chính ngữ pháp của hướng đó qua các trạng thái: hover/chọn, người nói, vật có thể tương tác, journal/inventory thật và các cự ly. Một ảnh chỉ cho thấy một phần của hệ, không chứng minh toàn game xuyên suốt.

### P3 / câu hỏi nhập vai — một số frame chưa xác định đầy đủ vai người chơi và người nói

Earth có dấu chọn và bong bóng hướng vào người thợ; grid có góc chọn; first-person có tay/portrait; glyph có người đỏ; round có portrait ông già và cuộc nhận lá. Những tín hiệu này đủ để thảo luận vị trí người chơi, chưa là đầy đủ state giao tiếp. Comic có bong bóng gắn với khung cận và nhãn chung “Người trong nhà”, nhưng chưa xác định rõ người chơi đang nhập vai ai nếu không đọc brief. Điều này cần được cụ thể hóa khi thử flow, không buộc mọi frame concept phải thêm tên NPC hoặc một hộp thoại mẫu giống nhau.

Đầu lớn/chi ngắn ở round vẫn có dấu tuổi qua tóc/mày/râu và tư thế. Không coi hình thức tròn hoặc dễ thương là bằng chứng nội dung trẻ em; cũng không coi một ông già và một người trẻ đã kiểm được độ phủ mọi cuộc đời ngẫu nhiên.

### Câu hỏi văn hóa — nhận diện thời đại chưa được xác nhận chỉ bằng sự đa dạng

Các cảnh đã mở sang tổ chức lao động/công trình, ruộng muối, chữa trị, xưởng, đầm và gia đình, phù hợp việc tìm bản sắc đời sống rộng hơn một bến. Nhưng dạng nhà, công cụ, lò/khuôn, nón, đồ đựng, cây trồng, kết cấu thành/hố và motif trên cột vẫn là hư cấu cần provenance cụ thể khi phát triển. Đặc biệt không gọi hố công trường là mặt bằng Cổ Loa xác thực, lò/khuôn là quy trình đúc đã được kiểm, hay ký hình là văn tự cổ. Review này không thực hiện nghiên cứu khảo cổ mới hoặc xác nhận phục dựng.

## Gallery đã nhìn thấy và ranh giới kết luận

Trong screenshot desktop và hai screenshot mobile đã xem, sáu ảnh/thumbnail đều hiện nguyên khung, các notes phân biệt ý định với output có mặt; không thấy HTML game HUD đè lên artwork. Ở mobile/HTML150, nội dung gallery xếp dọc, nhãn điều khiển còn đọc được và thư viện01–13 hiện đủ. Chữ/UI trong raster rất nhỏ khi toàn ảnh co vào390px; cần mở original để xem ý tưởng. Điều khiển cỡ chữ HTML không làm chữ trong ảnh reflow. Không nhận ảnh full-page thành audit accessibility hoặc toàn bộ interaction.

Không phát hiện **P1 cản việc xem và so sánh sáu ý tưởng** trong phạm vi original/screenshot đã quan sát. Có khác biệt thị giác thực để chủ dự án đánh giá; chưa có bằng chứng để chốt mức “vượt bậc”, hướng được thích hay mỹ thuật toàn game đã hoàn thiện. Các P2 là giới hạn/việc cần thử khi đào sâu, không phải yêu cầu loại mẫu hoặc tạo thêm ảnh ngay trước bàn giao.

Không có bằng chứng model/scene3D/mesh/rig mới, curved-camera runtime, parallax, animation, gameplay, benchmark, chi phí production hoặc human playtest trong review. UI raster không ghi authoritative state hay tạo quyền mới cho NPC/worlddirector. Không thay random-origin hoặc nhập thoại/costume/motif thành canon. Giữ toàn bộ01–13 cùng workshop và các bản14; tất cả tiếp tục **Pending user approval**, không tự đóngSprint13, merge dev hoặc chuyểnSprint14.
