# Workshop đồ họa — sáu hệ thể hiện để nhìn ra một game

30/10/2026 · Sprint13/SPIN-029 · Tác nhân `/root/art_rethink_systems` · **Đề xuất để thảo luận, Pending user approval**.

Mục tiêu của tài liệu là thay đổi cách đặt câu hỏi cho ảnh xem trước. Chủ dự án muốn một game lấy cảm hứng Văn Lang–Âu Lạc có bản sắc, không cần giống ảnh thật. Phương án dưới đây mô tả cách người chơi nhìn người, vật, không gian và thông tin; chưa tạo ảnh, model, scene, animation hoặc UI runtime. Chúng không đổi engine, camera đã được chốt, canon hoặc luật gameplay vì những quyết định này chưa được thông qua.

Các gợi ý văn hóa trong file là diễn giải mỹ thuật cần đối chiếu với nghiên cứu của worker văn hóa, không là kết luận khảo cổ. Một mái nhà, kiểu tóc hoặc chất liệu được mô tả ở đây không tự thành phục dựng chính xác.

## 1. Câu hỏi cần đổi

“Vẽ đẹp hơn bằng chất liệu gì?” mới trả lời một phần nhỏ. Một hệ đồ họa game cần trả lời đồng thời:

| Trục | Quyết định phải nhìn thấy trong preview |
|---|---|
| Không gian | Người đi ở đâu; cái gì trước/sau, cao/thấp; vật nào che người; nhà có thể tiếp cận từ phía nào |
| Camera và tỷ lệ | Nhìn từ trên, nhìn ngang hay gần vai; người lớn bao nhiêu so với khung hình; người chơi đang đọc cả làng hay một cuộc gặp |
| Hình khối | Cơ thể, nhà, cây, thuyền được giản lược theo quy tắc gì; silhouette nào nhận được khi thu nhỏ |
| Bề mặt và ánh sáng | Dùng màu phẳng, mặt sáng/tối, nét viền hay texture; bóng giúp đọc khối và hành động thế nào |
| Cử chỉ và nhịp | Ai đang chờ, làm việc, cản đường, lắng nghe; chuyển động dự kiến nhanh/chậm, nặng/nhẹ ra sao |
| Thông tin | Thứ gì là vật trong thế giới, thứ gì là UI; ai đang nói; cái gì nhân vật biết; lựa chọn và phản hồi nằm ở đâu |
| Ngôn ngữ văn hóa | Cấu trúc cư trú, thân thể, công cụ, vật liệu, nhịp cộng đồng và biểu tượng có quan hệ gì; bỏ họa tiết trang trí ra còn nhận bản sắc không |

**3D là cách tổ chức không gian và tài sản; tả thực là cách diễn tả.** Có thể đề xuất người và nhà bằng những khối màu đơn giản trong một thế giới3D; có thể vẽ tranh2D giống ảnh thật. Pixel, anime, khắc nét hoặc màu phẳng có thể đi cùng nhiều tổ chức không gian. Vì vậy, nhãn một dòng không đủ làm brief.

Ở giai đoạn này artifact vẫn là **PNG mô phỏng cách game có thể trông như thế nào**. PNG có mặt khối, bóng và phối cảnh không chứng minh tồn tại mesh, camera quay được hoặc ánh sáng động. Tuy nhiên, người xem vẫn có quyền yêu cầu một ảnh mang ý tưởng3D phải cho thấy rõ quy tắc khối và không gian, thay vì chỉ có chiều sâu của một tranh phong cảnh. Chưa cần tạo model thật để thử khả năng hình dung đó.

## 2. Thoát khỏi công thức tờ asset

Prompt vòng12 đã khóa phần trên khoảng65% cho cảnh và35% cho hai mẫu toàn thân, ba portrait, sáu icon cùng nhật ký/túi đồ. Cấu trúc đó hữu ích để kiểm độ đồng bộ, nhưng nếu lặp ở mọi lượt, tác nhân tạo ảnh sẽ tiếp tục tối ưu một **bảng minh họa**, còn màn hình người chơi thực sự nhìn thì bị thu nhỏ.

Không nên dùng số portrait/icon làm chỉ báo rằng đã tìm ra đồ họa toàn game. Trong workshop mới, nên thử theo thứ tự:

1. **Khung chơi chính nguyên màn hình:** camera, người, lối đi, vật tương tác và feedback hình ảnh đọc được ở kích thước dự kiến.
2. **Khung thứ hai cùng ngữ pháp:** đổi góc hoặc trạng thái xã hội/công việc để xem quy tắc có giữ được không. Hai ảnh được tạo độc lập vẫn phải ghi là concept độc lập, không gọi là hai frame từ cùng scene.
3. **Khung hội thoại riêng:** người nói, câu nói, lựa chọn và ngữ cảnh tri thức rõ; portrait chỉ dùng khi thực sự giúp nhận người.
4. **Tờ kiểm hình riêng khi cần:** toàn thân/portrait/vật dụng để kiểm nhất quán, không ép nó vào mọi màn hình chơi.

Sự thống nhất xuyên game vẫn phải được kiểm, nhưng có thể bằng một bộ màn hình có mục đích khác nhau. Không bắt UI phải làm bằng cùng “chất liệu giả” với cảnh. UI có thể rất gọn nếu tỷ lệ, hình, nhịp và ký hiệu cùng bản sắc; chữ phải đọc được trước khi mang cảm giác đất/đồng/vải.

## 3. Sáu phương án cấu trúc

Các tỷ lệ người/khung dưới đây là **đề xuất để dựng ảnh so sánh**, tính theo chiều cao nhân vật chính trên chiều cao khung hình chơi. Chúng chưa phải camera specification, benchmark hoặc quyết định production. Cần điều chỉnh khi xem artifact thật.

### A. Khối Lạc — 3D khối lớn, nhìn chéo

**Hình dung:** nhìn xuống một cụm nhà cao khỏi mặt đất, bờ đất và đường nước như một không gian có thể đi vào; mọi thứ được tạo từ khối lớn, cạnh rõ, hai hoặc ba mặt sáng tối. Một người có đầu, vai, bàn tay và công cụ hơi phóng đại để nhận được nghề và hành động. Chất liệu nằm ở màu và vài dấu đặc trưng, không dùng vết nứt/da/vải li ti phủ kín.

- **Camera/tỷ lệ:** góc ba phần tư nhìn từ cao, định hướng trực giao hoặc phối cảnh rất nhẹ; người khoảng6–8% chiều cao màn hình. Thấy đủ một khoảng sân, căn nhà và đường ra bến, không thu cả làng thành đồ trang trí trên một chiếc đế.
- **Hình và vật liệu:** mái là một hoặc vài khối lớn; cột và sàn có khoảng âm rõ; thuyền có thân chắc và phần rỗng đọc được; cây là cụm tán có silhouette riêng. Nhân vật giữ thể tích bằng mặt khối; bỏ texture nhiễu và độ bóng kiểu ảnh sản phẩm. Bóng tiếp xúc xác định chân/vật chạm đất.
- **Con người/chuyển động dự kiến:** dáng già hơi khom, dáng khỏe rộng vai, dáng trẻ nhanh hơn; cùng quy tắc hình nhưng khác tỷ lệ và tư thế. Nhịp bước, đặt đồ, kéo dây có trọng lượng rõ. Đây mới là ý định, chưa phải animation đã kiểm.
- **UI:** dấu mục nhỏ ở người/vật đang được quan sát; hội thoại dùng một bảng chữ yên ở cạnh, xác định speaker bằng tên/portrait phù hợp thông tin đã biết. Nhật ký sắp theo việc/người/nơi, không phủ hoa văn lên mọi ô.
- **Vì sao có thể ra chất game:** đường đi, vật cản, mặt cao thấp và hành động có cấu trúc; người xem hình dung được điều khiển một nhân vật trong sân thay vì đứng ngoài ngắm tranh.
- **Vibe được đề xuất:** nhấn kiến trúc nâng khỏi nền, đời sống bến nước, công cụ và nhịp cộng đồng bằng cấu trúc khối. Quy tắc mái/đồ dùng cụ thể cần nguồn văn hóa, không mặc định mọi làng giống nhau hoặc ai cũng mang đồ lễ.
- **Đánh đổi:** hình tròn, tỷ lệ đầu lớn và khung cảnh trên đế dễ thành đồ chơi. Giữ dấu tuổi, nghề và quan hệ bằng dáng, nét mặt và tư thế, kể cả khi đầu lớn hoặc chi ngắn; không lấy tỷ lệ phóng đại làm bằng chứng nhân vật là trẻ con. Địa hình tiếp tục vượt ra ngoài mép khung. Ảnh tĩnh chưa chứng minh quay camera hoặc xử lý che khuất.

### B. Dải sông — 2D/2.5D nhìn ngang, không gian thành lớp sinh hoạt

**Hình dung:** bờ sông thành một dải đi lại có người đang sống: chân nhà ở lớp sau, sàn nhà cao, lối đất ở giữa, nước và cọc bến thấp hơn. Người có hình lớn, tư thế rõ; nhịp nhóm người và khoảng trống quan trọng hơn phối cảnh sâu. Mỗi lớp vừa tạo chiều sâu vừa có vai trò đọc tình huống.

- **Camera/tỷ lệ:** ngang hoặc hơi xiên, nhân vật khoảng12–16% chiều cao khung. Một đoạn bến/sân là một đơn vị quan sát; không nhồi toàn làng vào hậu cảnh.
- **Hình và vật liệu:** mặt người/tay và công cụ chọn các nét chủ chốt; áo là mảng có trọng lượng, mái và tán cây thành hình lớn. Chỉ vài mặt tối để phân lớp, không tô từng sợi rơm. Có thể dùng sprite/ảnh2D trong nhiều lớp ở implementation tương lai; ảnh hiện tại chưa chứng minh parallax.
- **Con người/chuyển động dự kiến:** hành động lao động có dáng đầu/giữa/cuối nhận được; hai người quay vào nhau, người nghe ngừng tay, người đang bận tiếp tục việc. Người già/trẻ khác nhịp và cách giữ thăng bằng, không chỉ đổi chiều cao hoặc tóc.
- **UI:** hội thoại có thể ở vùng đáy gọn, người nói được nối rõ với nhân vật; phần lựa chọn không che tay/công cụ. Nhật ký và quan hệ dùng cùng hệ hình đơn giản của nhân vật, không cần cả màn hình giả thành cuộn giấy.
- **Vì sao có thể ra chất game:** mắt hiểu vùng di chuyển, hướng tiếp cận, tầng sàn và ai có thể gặp; hành động cơ thể lớn nên các lựa chọn đời thường có trọng lượng thị giác.
- **Vibe được đề xuất:** thử nhịp lặp, dáng người và khoảng trống lấy cảm hứng hình kể chuyện theo dải sau khi đối chiếu nguồn; không biến mọi cảnh thành lễ hội. Một cuộc sửa thuyền cũng có thể có nhịp hình mạnh.
- **Đánh đổi:** nhìn ngang dễ làm tuyến khám phá thành hành lang, khó diễn đạt nhiều lối giao nhau. Camera này là phương án presentation cần thử cùng gameplay, không mặc nhiên đổi game thành platformer hoặc thay Zone/Local Position Slots.

### C. Làng ôm nước — 2D nhìn từ trên, hình học và biểu tượng sống

**Hình dung:** vùng nước là khoảng màu mở; nhà, sân, ruộng và đường thành các hình rõ quanh nó. Người nhỏ nhưng khác silhouette, đồ mang và cử chỉ. Chi tiết quan trọng được phóng đại có chủ đích, còn mặt nền được để yên. Nhìn vào ảnh phải thấy một nơi có thể khám phá, không phải bản vẽ địa hình kín ký hiệu.

- **Camera/tỷ lệ:** nhìn từ trên hoặc góc hơi nghiêng; người khoảng4–6% khung. Đầu và vai nhận được hướng nhìn. Cận hội thoại có thể dùng chân dung riêng với tỷ lệ lớn hơn, giữ đúng mặt/trang phục.
- **Hình và vật liệu:** cây, mái, bờ, nước có nhóm hình khác nhau; tối giản texture; đồ dùng quan trọng lớn hơn vật trang trí. Mặt đường có độ tương phản ổn định. Không dùng ô vuông lộ như bàn cờ nếu đó không phải thông tin gameplay.
- **Con người/chuyển động dự kiến:** mang giỏ, chống sào, chờ ở bến, bước từ sân vào nhà là những dấu hiệu nhận người. Cast nhiều tuổi cần khác dáng/cách giữ công cụ; portrait bổ sung nét tuổi mà sprite nhỏ không đủ diễn đạt.
- **UI:** tương tác hiện theo vật/người đã nhận biết; một dải thông tin ngắn ghi “đang nhìn ai/cái gì” thay vì tràn icon lên cả làng. Sơ đồ quan hệ/đường đi chỉ gồm thông tin nhân vật thực sự biết, không dùng góc nhìn từ trên để lộ tất cả bí mật thế giới.
- **Vì sao có thể ra chất game:** tỷ lệ và vùng tương tác minh bạch; người chơi đọc được mạng lối đi và các lựa chọn không gian trước khi nhấn. Một nghề hoặc quan hệ có thể hiện qua người/đồ ở nhiều vị trí.
- **Vibe được đề xuất:** sinh hoạt quanh nước, hình sàn/nhà/công cụ theo nguồn phù hợp, nhịp vòng/đường cong như quy tắc bố cục mới. Không dùng một motif trống đồng to ở nền để thay việc thiết kế không gian cư trú.
- **Đánh đổi:** dễ giống game quản lý làng nếu mọi người chỉ còn chấm tượng trưng. Cần giữ một nhân vật được nhập vai rõ, cuộc gặp và mặt người có sức nặng, tránh UI toàn tri hoặc cảm giác ra lệnh cho cả cộng đồng.

### D. Người phẳng, làng có chiều sâu — sprite trong sân khấu khối giản lược

**Hình dung:** nhân vật là hình2D rất có cá tính trong một không gian nền có mặt sàn, cột, bậc và khối nhà rõ. Sự tương phản giữa hình người và thể tích sân là chủ ý thiết kế. Không cần làm họ trông như giấy thật, con rối vật lý, hay tấm bìa có mép sợi; trọng tâm là biểu cảm và bố trí không gian.

- **Camera/tỷ lệ:** góc ba phần tư thấp hơn A, người khoảng8–12% khung; khoảng sân gần chiếm phần lớn màn hình. Nếu tương lai muốn camera đổi góc thì sprite phải có quy tắc hướng nhìn/che khuất; chưa được chứng minh bằng ảnh.
- **Hình và vật liệu:** người dùng nét/mảng sạch; nền dùng khối mặt lớn và bóng đơn giản, giảm bề mặt nhiễu. Bóng chân và vị trí tay/vật giữ liên hệ với sàn; không để sprite như sticker dán nổi vô cớ.
- **Con người/chuyển động dự kiến:** ít pose nhưng chọn đúng dáng chủ chốt cho đứng chờ, làm nghề, trao đổi, lo lắng và lắng nghe. Người cao tuổi, người trẻ và người có thể trạng khác nhau giữ một hệ nét chung. Pose và nét mặt có thể thay biểu cảm, chưa có frame animation thật.
- **UI:** hội thoại dùng chính hình người/cận mặt đang ở cảnh hoặc portrait cùng nét; bảng chữ gọn. Mẫu vật phẩm lấy quy tắc mảng của sprite, các mặt khối của nền không ép icon thành ảnh render.
- **Vì sao có thể ra chất game:** người như quân nhập vai có hướng và vị trí trong một không gian sâu; đồng thời biểu cảm vẫn là điểm mạnh của2D. Đây là tổ hợp có logic, không chỉ đổi chất liệu phong cảnh.
- **Vibe được đề xuất:** thiết kế thân thể, mái và vật dụng qua ngữ pháp hình được nghiên cứu, rồi phối lại thành không gian sân/nhà/bến; không mặc định dùng một làng phổ quát rồi thay người thành áo cổ.
- **Đánh đổi:** người phẳng và nền khối có thể lệch nhau về ánh sáng/tỷ lệ. Ảnh đẹp từ một góc chưa đủ chứng minh hệ hướng sprite, đi vào nhà hoặc người bị cột che vẫn đọc được.

### E. Mặt người, khối làng — 3D cách điệu gần tầm người

**Hình dung:** thay panorama bằng một cuộc gặp trong tầm tay: thấy vai người chơi, đường đi cạnh sàn nhà, bàn tay người thợ, thân thuyền và phần nhà phía sau. Người có khuôn mặt hình học được thiết kế, tuổi qua mảng trán/má/lưng/tư thế, không qua lỗ chân lông. Vật liệu dùng màu, hình và độ phản sáng rất tiết chế.

- **Camera/tỷ lệ:** góc ba phần tư sau vai hoặc gần tầm đầu; người chơi khoảng25–32% chiều cao khung khi đi, NPC gần ở tỷ lệ đủ đọc mặt/tay. Đây là brief đối lập với toàn cảnh nhìn từ cao, không khẳng định đã có camera bám nhân vật.
- **Hình và vật liệu:** nhân vật có khoảng4–6 nhịp đầu theo ý định cách điệu, bàn tay/công cụ rõ; tỷ lệ cần xem thật để giữ người trưởng thành. Thân nhà, mái, cây có ít khối lớn; hai hoặc ba nhóm sáng tối, không texture ảnh chụp hoặc DOF để làm hậu cảnh mơ hồ.
- **Con người/chuyển động dự kiến:** trò chuyện có hướng mắt, khoảng cách và tư thế; nghề nghiệp hiện qua cách giữ vật và dùng lực. Một người già đứng nghỉ tay có thể quan trọng thị giác như người đang làm việc. Không dùng cùng một body chỉ thay mặt để đại diện mọi cuộc đời.
- **UI:** lựa chọn đặt gần vùng nhìn nhưng không bọc NPC bằng panel; tên và lời người nói ở nền chữ yên. Portrait phụ chỉ khi có lợi, tránh vừa vẽ mặt lớn trong cảnh vừa chen ba chân dung không cần thiết.
- **Vì sao có thể ra chất game:** cảm giác hiện diện ở một cuộc gặp và khả năng đi quanh đồ vật được đặt làm giả thuyết chính. Khối nhân vật, các mặt bên của nhà và vật cản phải rõ, đủ khác một tranh tả người.
- **Vibe được đề xuất:** kiến trúc và cảnh lao động quyết định khoảng cách/nhịp tương tác; thiết kế khuôn mặt và vóc dáng có bản sắc riêng, không chỉ dùng shader hoạt hình cho một mô hình tả thực chung chung.
- **Đánh đổi:** gần mặt người làm độ đồng bộ cast/biểu cảm và không gian chật trở nên quan trọng. Chưa có bằng chứng animation, rig, camera tránh vật cản hoặc hiệu năng; không suy giảm texture là tự động tiết kiệm bao nhiêu chi phí.

### F. Hai nhịp của một cuộc đời — thế giới biểu tượng, cuộc gặp có thân thể

**Hình dung:** đi xa/quan sát tuyến đường bằng một thế giới2D có hình lớn và địa danh dễ nhớ; khi dừng ở một nơi, chuyển sang một cảnh xã hội nhìn ngang hoặc ba phần tư có người lớn hơn. Cả hai mức dùng cùng quy tắc silhouette, palette và ký hiệu. Không phải mở bản đồ rồi rời game: chính sự thay đổi mức chi tiết diễn đạt rằng một con đường nối tới các cuộc đời cụ thể.

- **Camera/tỷ lệ:** lớp địa điểm/tuyến đường có người khoảng2–4% khung hoặc một dấu hiện diện; lớp cuộc gặp có người khoảng15–22%. Tỷ lệ chuyển rõ, không giả chúng là một camera zoom liên tục đã được hiện thực.
- **Hình và vật liệu:** vùng đất/nước/đường là các khối2D giản lược; khi vào cuộc gặp, cùng hình mái/công cụ/người được mở thêm chi tiết. Không ép địa lý vật lý thành mạng node mới; dữ liệu route, khoảng cách và thông tin gameplay giữ theo contract.
- **Con người/chuyển động dự kiến:** ở xa, đoàn người/thuyền được diễn đạt cô đọng; ở gần, cử chỉ nghề nghiệp và thái độ có trọng lượng. Ba thế hệ trong một household cần nhận được bằng thân thể và nét mặt, không chỉ tuổi ghi trên profile.
- **UI:** nhật ký gắn các việc đã biết vào người/nơi; hội thoại ở lớp gần có đủ speaker và câu nói. Tuyến chưa biết không hiện chính xác, bí mật NPC không được vẽ thành icon lộ trên bản đồ. Đây là yêu cầu presentation theo knowledge isolation, không tạo quyền mới cho World Director.
- **Vì sao có thể ra chất game:** cho thấy một cuộc đời vừa có lựa chọn địa phương vừa di chuyển qua mạng người và nơi chốn; đồ họa chủ động nén/phóng thông tin thay vì cố mô phỏng mọi thứ ở cùng tỷ lệ vật lý.
- **Vibe được đề xuất:** bố cục địa lý và nhịp đồ hình lấy cảm hứng nguồn văn hóa đã kiểm; cảnh đời người giữ lao động/gia đình/nghĩa vụ. Dùng ký hiệu mới có quy tắc thay cho rắc hoa văn nghi lễ.
- **Đánh đổi:** chuyển mức có thể làm gián đoạn sự liên tục hoặc làm người chơi tưởng thời gian đã trôi. Khi hiện thực phải nêu rõ thao tác nào chỉ đổi view, thao tác nào là travel/AdvanceTime theo luật đã có; workshop chưa thay quyền sở hữu thời gian hay state.

## 4. Tình huống chung để thử các phương án

Dùng một tình huống minh họa có sẵn trong tinh thần slice: **ở bến, một chiếc thuyền cần sửa; người thợ lớn tuổi và một người mang hàng đang trao đổi; nhân vật của người chơi đến quan sát trước khi chọn có can thiệp hay không.** Đây là brief mỹ thuật, không thêm repair rule, event effects, deadline hoặc lời thoại vào runtime. Cast chỉ minh họa cuộc đời được nhận ngẫu nhiên, không là màn hình chọn tuổi/nghề.

Giữ các yếu tố sau qua các phương án để thấy sự thay đổi của hệ thể hiện:

- Một thuyền, một bến, lối đến sân, phần nhà hoặc mái che, công cụ đang được sử dụng; giữ các vai chứ không bắt cùng pose/camera.
- Nhân vật chính, người thợ lớn tuổi, người mang hàng trưởng thành; có khác vóc dáng và tư thế. Chỉ thêm người xa khi giúp hiểu nhịp sống.
- Một dấu quan sát hoặc lựa chọn rõ ràng; không vừa chọn người vừa mở nhật ký/túi đồ khắp màn hình.
- Hội thoại minh họa có speaker, một câu đang nói và phần lựa chọn riêng. Chữ/tên phải là ví dụ hư cấu có nhãn, không làm lộ hidden truth.

Mỗi hệ chọn **một màn hình chơi lớn** làm bước đầu; sau khi đã đọc được nó mới làm màn hình tương tác thứ hai. Giữ ảnh cũ và không ghi phương án nào bị loại chỉ vì tạo phương án mới.

## 5. Đọc kết quả mà không tự báo thành công

Không dùng “đủ sáu icon” hoặc “ảnh có ánh sáng đẹp” làm acceptance duy nhất. Khi xem preview thật, ghi cụ thể:

| Câu hỏi | Có thể quan sát từ ảnh | Cần evidence khác nếu triển khai |
|---|---|---|
| Nhân vật đang ở đâu và đi về đâu? | Tỷ lệ, hướng người, đường vào, vật cản, mặt sàn | Điều khiển, pathing, collision, camera thực |
| Ai đang nói/làm việc/chờ? | Pose, hướng mặt, speaker và UI hiện có | Chuyển trạng thái, thời điểm, animation, dữ liệu projection |
| Người, nhà và vật có cùng quy tắc giản lược không? | Silhouette, số nhóm sáng tối, mật độ texture | Bộ asset nhiều góc/hướng và tính nhất quán production |
| Có còn nhìn như ảnh phong cảnh với người trang trí? | Mức ưu tiên người, action, lối đi và feedback | Người chơi hiểu/chọn ra sao trong playtest thật |
| Vibe có đứng được khi bỏ motif trang trí? | Hình nhà/người/công cụ, cấu trúc bến và nhịp cộng đồng | Nguồn văn hóa đối chiếu, kiểm nội dung theo vùng/thời đoạn |
| Một ý tưởng3D có đọc ra khối và không gian không? | Các mặt khối, occlusion, tỷ lệ và bóng hợp lý | Model/scene/camera/ánh sáng/rig thật; ảnh không chứng minh |

Trong các lựa chọn, A và B đáng được đưa ra đối chiếu sớm vì chúng thay đổi mạnh cách đọc cùng một tình huống: **khối không gian** so với **nhịp người theo dải**. Đây là đề xuất của tác nhân, chưa là quyết định chọn hai hướng, bỏ bốn hướng hoặc chốt với chủ dự án. E trả lời mạnh câu hỏi hiện diện gần người; C/F trả lời cách đọc thế giới rộng; D thử phân vai2D/3D trong cùng một hệ.

Không có benchmark, ước lượng tiết kiệm production, người chơi thử, mẫu ảnh hoặc runtime mới được tạo trong task này. Không suy workshop này thành nghiệm thu mỹ thuật hay cho phép chuyển Sprint14.
