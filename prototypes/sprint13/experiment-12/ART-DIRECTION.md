# Thử nghiệm 12 — Sáu cách làm thế giới có tiếng nói riêng

Sprint 13 · SPIN-028 · 30/10/2026 · **Bổ sung thư viện tham khảo, Pending user approval**.

Chủ dự án yêu cầu ít nhất năm mẫu tiếp theo và muốn đẩy mạnh trí tưởng tượng. Vòng này chọn sáu hướng có sự khác biệt về chất liệu, hình khối, nét, không gian, ánh sáng và giao diện. Đây là brief trước khi quan sát output: các mục dưới mô tả ý định và câu hỏi cần kiểm, không tuyên bố ảnh đã đáp ứng hoặc hướng nào tốt hơn.

## Câu hỏi của vòng này

Một ngôn ngữ mỹ thuật có đủ sức đi xuyên đời sống trong game không: cảnh người dân làm việc, vóc dáng toàn thân, gương mặt biết tuổi và nghề, vật dụng dễ nhận, hội thoại, túi đồ và nhật ký? Thử nghiệm không chỉ tìm một phong cảnh bắt mắt. Mỗi hướng phải gợi được cách người chơi nhìn một cuộc đời và đọc tình huống thường ngày.

Các bảng được tạo độc lập. Có thể thay bối cảnh, góc máy và cast để khai thác thế mạnh của chất liệu; không cố giả rằng đây là sáu camera của cùng một scene. Vì nhiều biến thay đổi, phản hồi là đánh giá một tổ hợp; không kết luận thích hay không thích chỉ do palette hoặc một kỹ thuật.

## Yêu cầu chung

- Một cảnh chơi đủ lớn để thấy không gian đi lại, việc đang diễn ra và người có thể trò chuyện. Chọn công việc, gia đình, cộng đồng, trao đổi và chuẩn bị trước biến đổi của môi trường; không để combat chiếm hết danh tính RPG.
- Mẫu toàn thân và chân dung của cast trưởng thành, có khác biệt tuổi, dáng, nghề hoặc biểu cảm. Những người minh họa không phải protagonist cố định hoặc màn hình chọn origin. Giữ baseline sinh nhân vật ngẫu nhiên.
- Vật phẩm hữu dụng như công cụ, lương thực, đồ mang theo được cô đọng thành icon cùng ngôn ngữ với cảnh và nhân vật.
- UI hội thoại, túi đồ và nhật ký có hình thức riêng cho từng hướng; vùng chữ yên và hệ phân cấp rõ hơn vùng trang trí. Không dùng cùng một HUD HTML phủ trên sáu tranh để thay thế thiết kế UI.
- Bảng màu có vai trò: nền đỡ hành động, người/vật quan trọng nổi bằng hình và sáng tối; trạng thái không chỉ phân biệt bằng màu. Kích thước chữ có dấu phải được xem thực tế sau khi tạo.

Bối cảnh, trang phục, lời thoại, đồ vật và motif là hư cấu mỹ thuật lấy cảm hứng Việt cổ. Không tự công nhận là phục dựng khảo cổ hoặc bổ sung canon, nghề, sự kiện, công thức hay rule mới.

## 1. Đất biết kể — `clay-craft`

**Giả thuyết:** chất liệu đất nặn và vật thủ công làm một thế giới đầy người lao động trở nên có trọng lượng, gần gũi và có dấu tay, nhưng vẫn kể được mất mát, trách nhiệm và quan hệ trưởng thành.

Thế giới có diện mạo một phim stop-motion: khối nhà, bờ đất và đồ dùng giữ vết nặn, mép không hoàn hảo, bề mặt matte. Bố cục cần cảm giác đang ở trong làng, tránh mọi thứ chỉ là món đồ trưng bày đặt trên một chiếc bệ. Ánh sáng mềm đủ cho mặt, tay và công việc đọc được; bóng tiếp xúc giúp người/vật đứng vững. Bảng màu ưu tiên đất nung, vôi nhạt, chàm và màu cây tiết chế; không biến mọi vật liệu thành cùng một lớp nâu.

Nhân vật có bàn tay, dáng lưng, đầu và quần áo được giản lược theo khối có chủ ý. Mặt cận phải nối được với mẫu toàn thân; tuổi và nghề không chỉ là một phụ kiện gắn thêm. Icon nên giống đồ nặn nhỏ nhận được bằng silhouette. UI có thể dùng tấm đất, dấu ấn hoặc nhãn vải, nhưng giữ nền chữ phẳng, đủ khoảng thở; tránh nút quá giống kẹo hay đồ chơi trẻ nhỏ.

**Cần quan sát:** cảm giác thủ công có đi qua portrait và UI không? Người trưởng thành có biểu cảm riêng, hay bị thu thành búp bê cùng mặt? Tỷ lệ có làm nhẹ đi nội dung đời sống quá mức? Đây là ảnh mô phỏng chất liệu, chưa có model, stop-motion hoặc pipeline asset thật.

## 2. Nét trong — `clear-line`

**Giả thuyết:** nét rõ và mảng màu sạch biến một thế giới nhiều quan hệ, vật dụng và lựa chọn thành trang truyện có thể đọc nhanh, đồng thời giữ cá tính từng người.

Đường viền chọn lọc, trọng lượng nét có trật tự; kiến trúc và người dùng cùng quy tắc hình. Không gian được tạo bằng phối cảnh, lớp chồng và các mảng sáng tối gọn, không phủ texture giả cổ lên mọi mặt. Palette phân vùng bằng màu phẳng, ưu tiên vài màu có vai trò thay vì rải màu bão hòa khắp ảnh. Cảnh nên đọc rõ một hành động hoặc cuộc gặp trước khi mắt đi tìm chi tiết hậu cảnh.

Toàn thân và portrait dùng cùng tỷ lệ đầu, mũi, cử chỉ và nét áo. Cast trưởng thành có thể thanh mảnh, nặng người, thấp, cao hoặc lưng hơi còng; tránh một khuôn mặt thay tóc. Icon giống hình minh họa vật dụng rõ nét. Hội thoại có thể mang nhịp khung truyện, nhật ký có bố cục trang, túi đồ có ô gọn; vẫn phải phân biệt người đang nói và các lựa chọn, không biến cả màn hình thành bong bóng chồng chéo.

**Cần quan sát:** vùng nét có quá dày khi thu nhỏ không? Nhân vật có tách khỏi hậu cảnh mà không cần viền phát sáng? Chân dung người nói có đúng với cảnh? Chất truyện tranh có giữ được bầu không khí sống, thay vì mọi thứ giống sơ đồ? Chưa có công nghệ nét động hoặc UI runtime.

## 3. Vách thời gian — `mineral-fresco`

**Giả thuyết:** thế giới như bích họa khoáng màu đang sống có thể làm cuộc đời cá nhân gắn với ký ức cộng đồng, mà không biến người dân thành những biểu tượng không cảm xúc.

Bố cục có nhịp lớn, hình khối chắc và mảng màu khoáng trên bề mặt vôi. Thử sắc đất đỏ, vàng khoáng, xanh đá tiết chế và trắng phấn; vết mòn hoặc nứt là điểm nhấn có chọn lọc, không phủ ngang chữ và mặt. Chiều sâu có thể nông và chủ ý, với đường đi và vị trí hành động vẫn rõ; không dùng kích thước biểu tượng để suy ra khoảng cách gameplay.

Người được vẽ thành dáng có sức nặng, bàn tay và tư thế nói được công việc; portrait cần thêm đời sống và nét riêng, tránh chỉ lặp gương mặt nghi lễ. Icon dùng mảng đặc hoặc nét khắc đơn giản trên nền yên. UI có thể chia theo ô bích họa, dòng kẻ và dấu mục; sự trang trọng nằm ở nhịp và khoảng trống, không phải một khung hoa văn khổng lồ che cảnh.

**Cần quan sát:** nhìn ở kích thước game còn phân biệt đường đi, người và vật tương tác không? Chất liệu vách có lan vào UI nhưng vẫn đủ contrast? Nhân vật còn đọc được tâm trạng thường ngày? Motif chỉ là đề xuất mỹ thuật, không chứng minh lịch sử hoặc cơ chế ký ức runtime.

## 4. Ký ức CD-ROM — `retro-prerender`

**Giả thuyết:** diện mạo RPG máy tính cuối thập niên 1990 có thể cho cảnh làng cảm giác cụ thể, giàu khám phá, đồng thời tạo một bản sắc tiết chế hơn ảnh 3D trơn và quá bóng.

Cảnh mang cảm giác nền dựng sẵn với góc máy cố định, ánh sáng được dàn vào không gian, bề mặt và mép hình có độ thô có chủ đích. Không chỉ đặt một bộ lọc pixel lên tranh hiện đại. Đường đi và nhân vật cần đủ tương phản với chi tiết nền; dùng vùng tối để gom hình, vùng sáng để dẫn mắt, không rải hiệu ứng rực khắp cảnh. Tỷ lệ nhà, người và đồ dùng phải giữ cảm giác đời thường.

Toàn thân có diện mạo khối 3D đời cũ, pose nhận được ở kích thước nhỏ. Portrait có thể trau chuốt hơn nhưng vẫn giữ mặt, tuổi và trang phục của mẫu toàn thân. Icon vật dụng có ánh sáng và khối nhất quán. UI thử panel gọn, font rõ và cửa sổ nhật ký có nhịp đọc, tránh bê một HUD fantasy combat không liên quan vào làng.

**Cần quan sát:** nét thô có được dùng có chủ đích qua cảnh/cast/icon hay chỉ là ảnh nhòe? UI có quá dày làm cảnh thành khe hẹp? Nhân vật có lẫn vào nền dựng sẵn? Đây là ảnh gợi diện mạo, chưa là background pre-render thật, sprite, mesh, camera hoặc palette kỹ thuật đã chuẩn hóa.

## 5. Đêm than bạc — `charcoal-night`

**Giả thuyết:** tiết chế màu tới than, giấy và ánh bạc có thể làm sự hiện diện của con người, việc chờ đợi và cuộc trò chuyện về đêm có chiều sâu mà không cần ánh sáng màu fantasy.

Cảnh dùng mảng than lớn, nét khô và vùng giấy còn trống; ánh sáng là hình sáng có cấu trúc, không phải bloom và viền neon. Độ sâu đến từ giá trị sáng tối và cạnh mất/cạnh rõ. Chọn một nguồn sáng đọc được và giữ lối đi không chìm hoàn toàn trong đen. Nếu dùng điểm màu hiếm, nó chỉ có vai trò thị giác trong concept, không tự là dấu chỉ danger hoặc vật tương tác của engine.

Toàn thân phải giữ silhouette, tay và hướng mặt. Portrait có nét xước, vùng sáng trên má và mắt để biểu đạt tuổi/tâm trạng, không biến mọi người thành bóng vô danh. Icon cô đọng thành hình vật và khoảng sáng âm; hội thoại/túi đồ/nhật ký dùng nền giấy hoặc mảng sáng yên, đủ phân biệt chữ với texture. Không để cảm giác huyền bí lấn hết công việc và quan hệ đời thường.

**Cần quan sát:** có đọc được mặt và mục tiêu tương tác ở mức thu nhỏ? Bóng tối có gây nhầm đường đi/vật thể? UI có cùng chất nhưng dễ đọc? Ảnh chưa chứng minh trải nghiệm chơi tối, accessibility, dynamic lighting hoặc quy tắc supernatural.

## 6. Gió mảng màu — `graphic-animation`

**Giả thuyết:** hình phẳng không viền, màu phân vai và cử chỉ rõ có thể tạo một thế giới Việt cổ có sức sống hiện đại, vừa diễn đạt nhịp lao động vừa giữ tính cách người trưởng thành.

Cảnh được xây từ mảng lớn, ít lớp bóng, hình cây/nhà/nước có nhịp nhưng không lặp máy móc. Tránh texture dày, quầng sáng và chuyển sắc phủ hết tranh. Các mảng có độ sáng khác nhau để người và đường đi đứng riêng; bố cục hướng mắt theo công việc và cuộc gặp, không chỉ theo một phong cảnh hùng vĩ. Màu có thể táo bạo nhưng cần vùng nghỉ và giới hạn điểm nhấn.

Thiết kế người dựa vào silhouette, tỷ lệ và tư thế riêng; tay, vai và mặt vẫn nói được tuổi/nghề khi không có nhiều nét tả. Portrait giữ cùng hình tóc, mặt, áo và mảng bóng với toàn thân. Icon là dấu hình vật dụng dễ gọi tên. UI thử mảng phẳng, dấu mục và nhịp typography; nền chữ phải bình tĩnh hơn cảnh, không tranh nhau thành poster.

**Cần quan sát:** có khác anime/cel thông thường bằng quy tắc hình thật sự không? Nhân vật trưởng thành còn đủ khác nhau? Màu có hướng người xem hay khiến mọi điểm đều nổi? Ngôn ngữ chuyển động mới là giả thuyết; ảnh không chứng minh animation, parallax hay asset production.

## Cách review sau khi có ảnh

Xem từng board nguyên khung, rồi mở PNG gốc để kiểm portrait, vật phẩm và chữ. Tách rõ bốn loại nhận xét: phần khớp brief đã nhìn thấy; chi tiết thiếu/lệch cụ thể; đánh đổi thẩm mỹ; phần chưa thể kết luận bằng ảnh tĩnh. Không ghi đủ thành phần hoặc chữ đúng khi chưa xem thật. Không dùng checker browser để kết luận mỹ thuật hay độ vui của game.

So sánh bằng câu hỏi: bản sắc có đi xuyên năm lớp cảnh/cast/portrait/icon/UI không; người chơi có đọc được một tình huống; người thường có cá tính; và phần nào đáng thử tiếp hoặc kết hợp với các phiên bản cũ? Chưa đo chi phí, performance, độ khó sản xuất, độ chính xác khảo cổ hoặc mức yêu thích của người chơi; không bịa số liệu để xếp hạng.

Root tạo raster bằng built-in `image_gen`, lưu prompt và provenance sau output thật. Không gọi Hyper3D/Blender hoặc dựng model chỉ để trình bày ý tưởng. Thư viện 01–11 được giữ nguyên. Vòng mới không loại phiên bản cũ; root/critic chỉ technical review, chủ dự án vẫn chốt các hướng và Sprint 13.
