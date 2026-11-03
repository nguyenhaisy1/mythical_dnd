# Workshop — Đồ họa game và bản sắc Văn Lang–Âu Lạc

30/10/2026 · Sprint13 · SPIN-029 · **Đề xuất để thảo luận, Pending user approval**.

Chủ dự án nhận xét các vòng trước thiên về ảnh thật, chưa làm rõ đồ họa game và chưa tạo được cảm giác Văn Lang–Âu Lạc phù hợp. Workshop này thay cách đặt bài toán: xác định một hệ hình ảnh phục vụ chơi, rồi mới tìm chất liệu. Không tạo batch ảnh mới, model3D hoặc đổi engine trong workshop. Các bản01–12 được giữ nguyên làm thư viện; phản hồi này không tự loại toàn bộ công sức cũ.

Kết luận làm việc của root: **cần thử những thế giới có hình khối được thiết kế riêng cho game, thay vì tiếp tục giữ cảnh làng gần tả thực rồi đổi bề mặt tranh.** Đây là đề xuất mỹ thuật, chưa là lựa chọn được duyệt. Bối cảnh cổ không đòi cách vẽ giống ảnh thật; cách điệu cũng không mặc định biến nhân vật thành trẻ con hoặc nội dung thành nhẹ nhàng.

## 1. Đồ họa ở đây là gì?

Đồ họa là cách game làm cho người chơi **nhìn thấy không gian, nhận ra người và việc, hiểu thông tin, hành động và đọc hậu quả**. Một tranh đẹp chỉ kiểm được một phần. Cần nói rõ từng trục:

| Trục | Câu hỏi cần trả lời | Ví dụ quyết định có nghĩa |
|---|---|---|
| Tổ chức không gian | Ta nhìn thế giới từ đâu; trước/sau, cao/thấp, trong/ngoài được đọc thế nào? | Nhìn chéo từ cao, mặt cắt ngang, nhìn từ trên, cận vai; camera cố định hay thay đổi là ý định cần ghi riêng |
| Hình khối và tỷ lệ | Người, nhà, cây, thuyền được giản lược/phóng đại ra sao? | Đầu/bàn tay lớn vừa đủ đọc, mái thành một khối cong rõ, thân cây ít mảng; giữ tuổi và nghề bằng dáng khác nhau |
| Cách vẽ/render | Mặt khối nhận sáng, bóng, màu, nét và texture theo quy tắc nào? | Màu phẳng, ít bậc bóng, bề mặt matte; không cần vết da, sợi vải hoặc phản chiếu như ảnh chụp |
| Chuyển động và phản hồi | Một bước đi, động tác nghề, câu đáp hoặc thay đổi trạng thái trông ra sao? | Pose chuẩn bị–thực hiện–kết thúc, chuyển trọng tâm, dấu chọn, phản hồi nguồn tin; ảnh tĩnh chỉ mô tả ý định |
| Ngôn ngữ nhân vật/vật dụng | Có nhận ra cùng một người và vật ở xa, cận mặt, icon không? | Đường vai, lưng, đầu, nét mặt và công cụ nghề giữ thống nhất; không dùng một mẫu thân người thay tóc |
| Giao diện và thông tin | Người chơi biết đang nói với ai, có thể làm gì, điều gì đã biết? | Speaker rõ, vùng chữ yên, trạng thái lựa chọn; nhật ký/quan hệ chỉ dùng tri thức được phép của nhân vật |

“Low-poly”, “pixel”, “anime”, “3D”, “màu cổ” không thay được các quyết định trên. 2D/3D là trục không gian hoặc cách tạo asset; hiện thực/cách điệu là trục tạo hình khác. Một game3D có thể rất phẳng và giản lược; một tranh2D có thể trông như ảnh chụp. Camera cố định hoặc orthographic vẫn có thể dùng trong game3D; không bắt buộc quay tự do mới gọi3D.

Với preview bằng ảnh, cách gọi đúng là **ảnh đề xuất diện mạo game3D cách điệu** nếu đó là ý định. Cần thấy rõ thể tích, tỷ lệ game, lớp che khuất và góc nhìn. Những dấu hiệu ấy giúp thảo luận cảm giác không gian nhưng **không chứng minh có mesh/scene/runtime3D**. Experiment04 từng có scene/render3D thật, Experiment05 từng có modelHyper3D; điều đó không chứng minh chúng đã đạt mỹ thuật chủ dự án muốn. Không sửa lịch sử để biến mọi thứ thành ảnh2D, cũng không dùng lịch sử kỹ thuật để bác bỏ cảm nhận hiện tại.

## 2. Điều gì đã kéo các vòng trước về tranh minh họa?

[Audit hình ảnh](art-rethink/01-VISUAL-AUDIT.md) ghi đúng những ảnh worker thực sự xem, không tuyên bố đã nhìn mọi artifact trong12vòng. Root đối chiếu thêm sáu output và prompt12 đã xem trong SPIN-028.

- **Khóa giải phẫu và mật độ chi tiết quá gần thực.** Prompt ưu tiên người trưởng thành, vật liệu và công việc là hợp lý; nhưng tôi thường diễn đạt bằng tỷ lệ tự nhiên, da/vải chi tiết, kiến trúc nhiều đồ và ánh sáng thật. Thêm chữ “clay” hay “stylized3D” không đủ thắng cấu trúc này. Cần giữ tuổi/nghề qua thiết kế hình dáng, không đồng nhất trưởng thành với tả thực.
- **Đổi lớp phủ nhiều hơn đổi hình.** Cảnh vẫn có nhịp nhà, đường, cây, người và phối cảnh gần nhau; đất nặn, đồng, texture tường, than hoặc nét viền có lúc chủ yếu thay mặt ngoài. Không coi mọi phiên bản thất bại; chúng giúp xác định điểm này.
- **Màn hình chưa đặt người chơi ở trung tâm quan sát.** Nhiều cảnh cho nhân vật làm mẫu diễn trước phong cảnh đẹp; chưa chỉ rõ ai là người đang được điều khiển, việc nào có thể làm, lối đi nào đang được thấy.
- **Khung65/35 bị dùng thành công thức.** Một bảng ghép cảnh + hai người toàn thân + ba portrait + sáu icon + nhật ký giúp kiểm độ phủ thành phần, nhưng lặp lại sẽ kéo mọi hướng thành tờ giới thiệu asset. Đủ ô không chứng minh một màn hình chơi tốt.
- **Độ đọc bị lẫn với chất lượng bề mặt.** Có đủ nếp mái, nan giỏ hay hạt thóc làm tranh phong phú nhưng chưa giúp đọc lối đi, người nói, việc đang làm hoặc trạng thái. Một số UI mới chỉ có menu lựa chọn, chưa có câu nói và speaker.

Đây là thiếu sót trong cách root đặt brief và đánh giá đầu ra. Không đổ cho công cụ sinh ảnh, không dùng số lượng ảnh/CI để thay chứng cứ mỹ thuật hoặc cảm nhận của chủ dự án.

## 3. Chất Văn Lang–Âu Lạc nên đến từ đâu?

[Nghiên cứu ngữ pháp văn hóa](art-rethink/02-CULTURAL-GRAMMAR.md) giữ nguồn và phân biệt hiện vật, diễn giải, hư cấu. Tài liệu [Nghệ thuật Đông Sơn của BTLSQG](https://baotanglichsu.vn/vi/Articles/3101/68748/nghe-thuat-djong-son.html) mô tả hình học lặp, thuyền, người và các loại đồ dùng. Đó là chất liệu tham khảo cho nhịp và hình, không phải bằng chứng rằng toàn thế giới từng có một bảng màu hay tỷ lệ chung.

**Đề xuất cảm giác:** một thế giới sông nước, cộng đồng, nghề thủ công và quyền lực đang chuyển biến; có sức sống, sự tinh xảo và những điều thiêng còn mơ hồ. Đây là diễn giải sáng tạo phù hợp pillars của game, không phục dựng khảo cổ đã được kiểm chứng.

Cụ thể hóa cảm giác bằng thiết kế thay vì trang trí:

- Không gian có **mái, sàn nâng, đường nước, bậc đất và khoảng sinh hoạt chung** với silhouette riêng. Mỗi hình dùng một vài khối đủ nhận; cấu trúc cụ thể cần nguồn theo vùng/thời điểm, không copy một làng dân tộc hiện đại rồi gắn nhãn Việt cổ.
- Con người được nhận bằng **dáng và động tác**: người chèo, người gánh, người làm nghề, người già có trọng tâm khác người trẻ. Dáng có thể phóng đại mà vẫn trưởng thành. Tư thế và tổ chức nhóm có thể là nguồn bản sắc mạnh hơn vẽ lỗ chân lông.
- **Nhịp lặp, băng, vòng, đối xứng và nét khắc** có thể gợi cách chia cảnh, chuyển cảnh hoặc tạo icon. Đây là cách vận dụng của ta, không biến cả mặt đất thành đồng đúc hoặc đặt hoa văn lên mọi nút.
- Màu phục vụ môi trường và việc cần thấy: có thể xanh lá, màu vải, đất, nước và điểm màu mạnh. Không suy “cổ” thành nâu xám hoặc màu patin xanh của đồ khai quật thành màu mọi vật lúc sử dụng.
- Đời thường và nghi lễ có mức nhấn khác nhau. Không mặc định mọi người đội lông chim, sơn mặt, cầm vũ khí hoặc sống trong sương huyền bí. Không giới hạn Văn Lang–Âu Lạc vào cảnh nghèo, hoang dã hoặc chợ ven sông lặp lại.
- UI là công cụ đọc game hiện đại, dùng tiếng Việt rõ. Có thể trừu tượng hóa nhịp hoa văn vào đường chia/cursor; không cần giả một cuốn sách châu Âu cũ, cũng không ngầm khẳng định người thời đó có nhật ký chữ Quốc ngữ hay một hệ chữ chưa kiểm chứng.

Không dùng một vài biểu tượng Đông Sơn để đại diện đồng nhất mọi địa phương và toàn campaign. Trang phục, dụng cụ, dạng mái, đồ đựng và nguồn sáng cần provenance riêng khi đào sâu. Nghiên cứu văn hóa không tự đổi canon, rule hoặc authoring pack.

### Từ nguồn tới dấu hiệu nhìn thấy trong frame

Đây là ba **phép biến đổi mỹ thuật đề xuất**, không phải quy chuẩn phục dựng hoặc sản phẩm đã tạo. Chúng giúp brief cụ thể hơn từ “vibe Việt cổ”:

| Tham chiếu | Cách biến đổi của ta | Dấu hiệu phải tìm trong ảnh thử |
|---|---|---|
| [Mô tả nhà trên trống Ngọc Lũ](https://baotanglichsu.vn/vi/Articles/1001/13602/gioi-thieu-bao-vat-quoc-gia-1-trong-djong-ngoc-lu-hien-vat-tieu-bieu-nhat-cua-nen-van-hoa-djong-son.html) | Chọn nét mái và quan hệ mái–sàn–cột để thiết kế silhouette với vài mảng lớn; phần kết cấu và tỷ lệ còn là giả định | Nhìn nhỏ vẫn phân biệt mái, sàn và khoảng âm dưới nhà; không dựa vào hàng nghìn sợi rơm. Không sao hình chạm thành mặt bằng kiến trúc thật |
| [Tượng người cõng nhau thổi khèn](https://baotanglichsu.vn/vi/Articles/1001/19790/bao-vat-quoc-gia-tuong-nguoi-cong-nhau-thoi-khen.html) | Học cách tư thế và điểm tựa nối hai người, chuyển thành thiết kế động tác trao/nâng chung một vật | Hướng thân, tay và trọng tâm làm rõ ai giúp ai ngay khi không vẽ nếp da. Hành động mới là sáng tạo, không khẳng định hiện vật mô tả việc sửa thuyền |
| Nhịp hình học trên đồ dùng được mô tả trong nguồn Nghệ thuật Đông Sơn đã dẫn | Chuyển nhịp lặp thành cách nhóm hoạt động, khoảng trống và dấu chia giao diện; không biến tất cả vật liệu thành đồng | Nhóm người, cột, thuyền hoặc UI có nhịp nhận được khi bỏ hoa văn bề mặt. Đây là cấu trúc bố cục mới, không motif được chứng nhận dùng cho menu cổ |

Không bắt mọi hướng dùng cả ba phép biến đổi cùng lúc. Chọn một vài dấu hiệu để kiểm và ghi phần nào đang chưa biết. Một ảnh có mái giống tham chiếu vẫn chưa đủ chứng minh vibe toàn game hoặc độ xác thực của cả bộ phục trang.

## 4. Những cách thể hiện thực sự khác

[Bài đề xuất hệ đồ họa](art-rethink/03-GAME-VISUAL-OPTIONS.md) phát triển camera, tỷ lệ, hình, UI và rủi ro từng hướng. Bảng dưới là tổng hợp và lựa chọn ưu tiên thử của root, chưa được duyệt.

| Hướng | Người chơi sẽ thấy gì khác hẳn? | Liên hệ bản sắc / đánh đổi |
|---|---|---|
| **3D khối biểu tượng** | Nhìn chéo từ cao; người là các khối có dáng rõ, mặt và tay đủ lớn để đọc; mái, thuyền và cây thành ít mảng lớn, bóng gọn, bề mặt matte. Thấy rõ không gian dưới sàn và vật che nhau, không có hiệu ứng ống kính ảnh chụp | Dùng silhouette và nhịp hình nghiên cứu được để thiết kế lại, thay vì phủ texture. Cần kiểm khác biệt với đồ chơi/chibi đại trà và sự thống nhất mặt gần/xa |
| **2.5D dải đời sống** | Làng như nhiều dải sân khấu; thấy sàn nâng, gầm nhà, bến và dòng nước ở cao độ khác nhau; nhân vật mặt nghiêng/ba phần tư mạnh về cử chỉ. Trong/ngoài nhà có thể dùng mặt cắt như một lựa chọn trình bày | Thử chuyển nhịp kể hình theo băng thành không gian chơi; không phải bản sao trống đồng hay bằng chứng kiến trúc. Tuyến rẽ/đi lại và combat phải được đặc tả nếu hiện thực sau |
| **2D nhìn từ trên, hình học sống** | Camera gần thẳng từ trên; vùng đất/nước, mái và đường là mảng lớn; người và công cụ có hình nhận ngay. Cảnh chơi nhiều người cùng làm việc vẫn có thứ bậc; cận mặt khi trò chuyện | Bản sắc qua bố cục và bộ silhouette, không dựa vào texture. Cần tránh biến tất cả thành quân cờ vô danh hoặc UI mô phỏng quản lý toàn tri |
| **Nhân vật2D trong không gian có chiều sâu** | Người dựng như hình minh họa cô đọng, đứng giữa các lớp không gian; camera nhìn chéo, che khuất và sàn cao/thấp làm rõ khối. UI và portrait giữ chung đường nét | Là ý tưởng hybrid, không chỉ “giấy cắt” phủ lên cùng tranh. Cần kiểm cách nhân vật quay, bề dày và hướng sáng khi có implementation; ảnh không chứng minh các việc đó |
| **3D cận người, tạo hình phóng đại** | Camera cận vai/tầm người, mặt khối và mảng màu lớn; mái cong, cột, thuyền tạo tỷ lệ có chủ ý; nhân vật có đầu, vai và nét mặt riêng. Ưu tiên biểu cảm và công việc gần người | Cảm giác sống một cuộc đời trực tiếp hơn; dễ trở lại render điện ảnh nếu không khóa grammar. Cần nhiều thiết kế cho cự ly gần và các mặt không thấy trong preview, chưa có ước lượng chi phí |
| **Hai cấp nhìn: địa bàn và cuộc gặp** | Khám phá bằng bản đồ hình học có người/đường nước; khi gặp người chuyển sang sân khấu xã hội có cử chỉ, không gian và UI cùng nét. Không bắt mọi hoạt động dùng một camera | Có thể nhấn đời sống và thông tin; thay đổi cách chuyển màn hình là đề xuất UX cần kiểm riêng. Không hiển thị NPC, quan hệ hoặc sự kiện mà protagonist chưa biết |

Không đặt tên chất liệu trước rồi mới tìm game bên trong. Hướng đầu nói rõ một thế giới3D cách điệu sẽ tổ chức hình như thế nào; hướng thứ hai đổi cấu trúc không gian; hướng thứ ba đổi cách đọc thế giới. Đó là ba mức khác biệt đáng thử trước. Các hướng4–6 vẫn là phương án để đào sâu, không bị loại.

**Root ưu tiên thử1 và2, dùng3 làm đối chứng.** Lý do: chúng kiểm trực tiếp hai nghi vấn lớn — liệu phóng đại hình khối có cho cảm giác game, và liệu nhịp hình bản địa có tạo được một cách nhìn riêng. Đây là lựa chọn thứ tự nghiên cứu của AI, chưa phải chốt art direction, engine hoặc ưu tiên được chủ dự án phê duyệt.

### Khác gì với các ý tưởng đã thử?

Các tên3D/2.5D đã từng xuất hiện, vì vậy không coi việc đặt lại tên là phát minh một hướng mới. Audit đã xem08C/11/12; root mở thêm trực tiếp `experiment-06/assets/c-sideview.png` trong workshop để đối chiếu. Những thay đổi dưới đây là **điều kiện cho brief sắp tới**, chưa là output đã đạt:

| Tham chiếu cũ | Điều cần thay cụ thể | Khi nào vẫn là lặp lại? |
|---|---|---|
| 08C faceted và11 diện mạo3D | Cùng quy tắc khối lớn phải đi qua người, nhà, cây, đồ, mặt gần và xa; mảng bóng gọn, bỏ texture tả thực; camera/cỡ nhân vật phục vụ một trạng thái chơi cụ thể | Chỉ đá/lá có cạnh, còn người/nhà/ánh sáng giữ cách tả cũ; hoặc chỉ đổi màu và gắn nhãn low-poly |
| 06C nhìn ngang | Giảm panorama tự nhiên nhiều chi tiết; tổ chức sàn nâng, gầm nhà, bến và nước thành các dải/cao độ dễ đọc; động tác người và khoảng trống giữ nhịp chính | Chỉ dịch camera ngang của cùng phong cảnh; vẫn dựa vào chiều sâu, vật liệu và blur thay cho cấu trúc lớp |
| 11pixel và12 hoạt hình | Với hướng nhìn từ trên, thiết kế lại footprint/hình đầu–vai–công cụ, tỷ lệ và vùng thông tin cho camera đó; không thu nhỏ tranh phối cảnh hiện có | Chỉ chuyển tranh sang pixel hoặc làm người đơn giản mà nền vẫn mật độ/tỷ lệ cũ |
| Các board11–12 | Màn hình chơi nguyên khung với một trọng tâm và UI cần thiết, rồi đối thoại riêng; đối chiếu cast/icon theo nhu cầu | Vẫn ghép nhiều mẫu xuống dưới khiến cảnh/camera được ngắm như keyart thay vì được kiểm như màn hình chơi |

Ưu tiên1/2 chưa kiểm đủ cảm giác **hiện diện gần người**. Nếu khối nhìn chéo vẫn bị đọc thành tranh hoặc mô hình trưng bày, hướng5 cận người là đối chứng bổ sung có giá trị: vẫn dùng cùng grammar khối nhưng chuyển cự ly camera. Đề xuất này không tự tăng scope lượt ảnh hoặc nhận camera gần là đúng ý chủ dự án. Khi nhiều biến thay đổi, phản hồi chỉ so được tổ hợp; muốn xác định tác dụng riêng của camera/văn hóa/vật liệu cần giữ những trục còn lại ổn định trong một lượt đối chiếu có chủ ý.

## 5. Lượt thử ảnh sau phải đổi cách kiểm

Đây là brief có thể dùng ở yêu cầu thử tiếp theo, chưa tạo ảnh trong workshop này.

**Tình huống chung:** một người vừa nhận cuộc đời ngẫu nhiên đang ở bến, thấy người làm nghề bên chiếc thuyền và một lối lên khu nhà sàn. Một nhân vật khác mang hàng đi ngang. Người chơi cần nhận được nơi có thể đi, người có thể hỏi và việc đang diễn ra. Không tự thêm kết quả quest, menu origin hay cơ chế repair vào runtime.

Cho mỗi hướng ưu tiên, ưu tiên **một màn hình chơi toàn khung ở cự ly camera dự kiến**. Không ghép dải asset dưới tranh và không đặt nhân vật khổng lồ diễn tiền cảnh nếu camera dự kiến là nhìn từ cao. UI chỉ gồm thông tin cần cho tình huống. Khi kiểm được camera/hình, dùng khung thứ hai cho đối thoại thực sự: có người nói, một câu nói minh họa, lựa chọn và cận mặt thống nhất. Dùng mẫu nhân vật/icon riêng khi cần kiểm chúng, không ràng mọi frame phải có đủ2/3/6mẫu.

Brief phải nêu cụ thể:

1. **Camera và tỷ lệ:** góc nhìn, phần màn hình nhân vật chiếm, khoảng không gian phải đọc; đánh dấu rõ các con số nếu là tham số thử mỹ thuật, không là production budget.
2. **Grammar hình:** đầu/thân/tay, mái/thuyền/cây được biến thành khối nào; bộ quy tắc phải xuất hiện trên vật ở cả gần và xa.
3. **Mức chi tiết:** ưu tiên silhouette, mảng sáng tối và tư thế; không dùng hạt thóc, da và sợi vải để tạo cảm giác “chất lượng cao”. Cách điệu trưởng thành có thể vui, thô, lạ hoặc phóng đại.
4. **Mức bám văn hóa:** nêu2–3nguồn tạo hình cụ thể và phần nào đang hư cấu; không diễn giải mọi mái, trang phục và vật phẩm là hiện vật đã xác minh.
5. **UI và tri thức:** nhận ra ai đang được điều khiển, speaker, hành động và thông tin đã biết; hình tượng quan hệ không mở bí mật worldstate.
6. **Ý định chuyển động:** vài pose hoặc khung liên tiếp có thể minh họa bước đi/hành động nếu cần; không gọi ảnh tĩnh là animation đã chạy hoặc các khung sinh độc lập là bằng chứng cùng geometry.

Đánh giá theo câu hỏi cụ thể: không đọc tên hướng thì có nhận ra khác về cấu trúc hình không? Thu về cỡ màn hình dự kiến có thấy người/lối đi/việc không? Cận mặt và icon có thuộc cùng thế giới không? Bỏ texture/hoa văn đi thì bản sắc còn không? Đó là các câu hỏi cần quan sát, không có thang điểm hoặc kết quả test được bịa trước.

## 6. Phân công, review và trạng thái

- `/root/art_rethink_audit`: audit mẫu đã xem và nguyên nhân lặp.
- `/root/art_rethink_culture`: nguồn văn hóa và giới hạn suy diễn.
- `/root/art_rethink_systems`: sáu cách tổ chức đồ họa, đánh đổi và brief.
- `/root`: điều phối, đối chiếu nguồn, phản biện tổng hợp và cập nhật chỉ dẫn.
- Hai reviewer độc lập `/root/art_rethink_critic_reasoning` và `/root/art_rethink_critic_brief` kiểm lập luận và tính dùng được; root sửa findings trước khi chốt hồ sơ. [Review lập luận](art-rethink/04-CRITIC-REASONING.md), [review brief](art-rethink/05-CRITIC-BRIEF.md) ghi phạm vi thực sự đã kiểm và hạn chế.

Tối đa bốn tác nhân cùng hoạt động, các vai trò chạy theo lượt. Review tài liệu/ý tưởng không là visual acceptance cho một output chưa tạo. Không chạy runtime/Unity/playtest/benchmark, không tạo model/ảnh mới hoặc đổi gameplay/canon. Chưa kết thúc Sprint13, chưa merge dev hoặc chuyểnSprint14.
