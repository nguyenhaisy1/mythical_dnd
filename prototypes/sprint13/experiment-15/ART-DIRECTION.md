# Thử nghiệm15 — Ba nhánh đào sâu và ba cách nhìn mới

30/10/2026 · Sprint13 · SPIN-032 · **Brief trước output, Pending user approval**.

Chủ dự án đánh giá rất tốt ba ý tưởng đầu vòng14: **Đất dựng thành, Ô đất sống và Làng vòng cung**, cho phép đào sâu, đồng thời muốn thêm những ý tưởng táo bạo hơn. Vòng15 ghi nhận chúng là **nhóm ưu tiên đào sâu theo phản hồi thật**, không suy thành chọn một art direction cuối, loại các bản khác hoặc chốt Sprint13.

Sáu màn hình được giao gồm ba nhánh mở từ nhóm ưu tiên và ba ngôn ngữ mới. Mỗi ảnh cần cho thấy cách thế giới, nhân vật, vật dụng và UI cùng vận hành về mặt thị giác. Root tạo sáu ảnh mới bằng built-in image_gen, không dùng ảnh vòng14 làm input; quan hệ kế thừa dưới đây là quan hệ ý tưởng, không phải chứng cứ identity/geometry matching. Tài liệu này mô tả ý định và câu hỏi kiểm, chưa tuyên bố ảnh đã được tạo hoặc đạt.

## Câu hỏi và khác biệt cần thử

| Key / tên | Quan hệ với vòng14 | Thay đổi chính cần thấy |
|---|---|---|
| `vertical-earth` — Thành dựng đứng | Mở từ14/1 Đất dựng thành | Công trình đất và đời sống đọc theo chiều đứng, tầng/sàn/hiên mở và đường lên xuống; giữ con người có vai riêng giữa quy mô lớn |
| `tidal-mosaic` — Ghép đảo thủy triều | Mở từ14/2 Ô đất sống | Địa bàn nhìn từ trên thành các mảnh hình không đều, khoảng nước và lối qua; vật/người/UI được thiết kế theo cùng bộ hình ghép |
| `rolling-world` — Làng trên vành trống | Mở từ14/3 Làng vòng cung | Độ cong của phép nhìn phải hiện qua toàn trường cảnh, thay vì chỉ một con đường uốn; khối mềm đi xuyên người, mái, cây và UI |
| `cubist-space` — Địa hình đa diện | Hướng mới | Cảnh2D có nhiều mặt nhìn được ghép có chủ ý; dáng người và UI cùng ngôn ngữ mặt phẳng, vẫn có nơi đứng và hướng tiếp cận đọc được |
| `shadow-world` — Làng của bóng | Hướng mới | Khối3D gần trắng được nhận bằng mặt và bóng tiếp xúc; bóng trở thành phương tiện đọc hình chứ không phải chỉ màu phủ hoặc texture mực |
| `mirror-water` — Thủy kính | Hướng mới | Góc nhìn qua mặt nước/phản chiếu làm cấu trúc màn hình; nhân vật, vật và vùng thông tin được tổ chức để không mất hướng và người đang nhập vai |

Camera, tỷ lệ, bố cục, cast, palette và UI có thể cùng đổi để tìm khoảng mới. Một ảnh được thích trước tiên cho biết một **tổ hợp** đáng đào sâu; chưa xác định riêng tác dụng của độ cong, chất liệu, màu hoặc căn cứ văn hóa. Không có thang điểm, production budget hoặc ngưỡng kỹ thuật được chốt từ brief này.

## 1. Thành dựng đứng — `vertical-earth`

**Giả thuyết:** có thể đẩy sức nặng của Đất dựng thành thành một nơi cư trú/công trường được đọc theo chiều đứng, nơi một cuộc gặp riêng vẫn nổi giữa quy mô cộng đồng.

Bố cục thử các mặt đất lớn, hiên/sàn mở, bậc và đường lên xuống được nén/phóng đại thành một cấu trúc rõ. Nhịp cao–thấp phải đổi cách nhìn, không chỉ vẽ lại vòng14/1 rồi kéo cao ảnh. Những nơi nhìn vào là khoảng công cộng mở; không dùng mặt cắt để lộ nội thất, đồ vật hay NPC bí mật sau tường. Không tự biến Loa Vực thành một thành phố trên vách núi có thật hoặc bản phục dựng Cổ Loa.

Người có dáng góc cạnh, tay/chân và điểm tựa biểu đạt việc mang, nâng, nghỉ tay hoặc trao đổi. Dấu của protagonist phải rõ ở cự ly chơi; các nhóm người không trở thành lực lượng để điều quân/xây thành. Khối đất, công cụ, portrait và icon dùng cùng nhịp cạnh cắt. Có thể kế thừa sức mạnh đất đỏ/than/xương của14/1, nhưng phải thay tổ chức không gian và hierarchy, không dựa vào đổi màu để gọi hướng mới.

UI có thể sắp thành một dải đứng có nhịp bậc và khoảng thở, giữ chữ tiếng Việt đọc thuận. Dấu cao độ chỉ là cách trình bày đề xuất, không tự thêm tầng navigation hoặc action-handler. **Cần quan sát:** người chính, chỗ chân đứng, lối tiếp cận và đối tượng giao tiếp còn nhận được không? **Nguy cơ:** thành ảnh kiến trúc khổng lồ hoặc giao diện city-builder, tỷ lệ nuốt mất cuộc đời con người, texture đất làm mảng khối mất rõ.

## 2. Ghép đảo thủy triều — `tidal-mosaic`

**Giả thuyết:** thay mặt bằng gần tự nhiên bằng một thế giới ghép từ các hình đất–nước có nhịp riêng có thể mở rộng Ô đất sống mà vẫn giữ khả năng nhập vai một người.

Thử camera nhìn từ trên và các mảnh không đều: bờ, khoảng nước, nơi cập thuyền, khoảng làm việc và đường qua tạo thành một địa bàn cục bộ. Không bắt buộc ô lục giác hoặc lưới đều. Nếu có đường ghép nhìn giống tile/hex, chúng là lựa chọn mỹ thuật, chưa là luật di chuyển, collision hoặc combat mới. World Route Graph, khoảng cách và thời gian chưa thay đổi.

Người, vật mang, nhà và thuyền cần được thiết kế cho cách nhìn này, không thu nhỏ hình người tả thực rồi đặt lên ô. Giữ một protagonist và những người có thể tiếp cận bằng dáng/đồ mang/hướng; cận mặt hoặc portrait cùng hình giúp giữ tuổi và cá tính. UI dùng mảnh hình, đường chia và icon chung ngôn ngữ, không hiển thị bảng quản lý toàn settlement. Các mảng màu phân công năng, lựa chọn và vùng yên, không rải màu ngang cấp khắp ảnh.

Tên “thủy triều” gợi nhịp đất–nước trong hình, **không chứng minh đã có simulation nước, lịch triều, forecast hoặc quyền nhìn tương lai**. Chỉ trình bày địa bàn/thông tin mà nhân vật có thể biết; vùng chưa biết không được vẽ chính xác vì camera ở trên. **Cần quan sát:** đây có là một thế giới người có thể đi/gặp, hay chỉ là một bàn puzzle đẹp? **Nguy cơ:** hình ghép tranh chú ý với người, nhân vật mất identity, bố cục vô tình gợi game quản lý toàn tri.

## 3. Làng trên vành trống — `rolling-world`

**Giả thuyết:** phép nhìn cuộn/cong thật rõ cùng bộ khối mềm có thể tạo cách hiện diện riêng, đồng thời sửa đúng khoảng chưa đạt của Làng vòng cung14/3.

Độ cong cần thể hiện trên trường cảnh: đường chân trời, mặt đất, hướng dựng của mái/cây và các lớp xa cùng góp phần. Một con đường ngoằn ngoèo giữa panorama bình thường chưa đủ trả lời câu hỏi camera. Cảnh vẫn liên tục vượt ngoài mép ảnh, không biến thành món mô hình đặt trên đế hoặc một quả cầu lơ lửng trong vũ trụ.

“Vành trống” là phép liên tưởng bố cục và nhịp vòng, **không phải thế giới vật lý nằm trên một trống đồng, một hành tinh nhỏ hoặc cosmology mới**. Hình mái, sàn nâng, người và sinh hoạt có thể được cách điệu mạnh bằng khối tròn, màu địa phương có vai trò, mặt sáng/tối gọn. Tỷ lệ đầu lớn/chi ngắn vẫn cho phép người già, người trẻ trưởng thành và thể trạng khác nhau; tuổi/nghề đọc bằng mặt, dáng và cách mang vật, không phải da tả thực.

UI có thể theo nhịp cung/vòng nhưng vùng chữ và lựa chọn cần ổn định, không liên tục xoay hoặc nghiêng chữ chỉ để chứng minh phong cách. Portrait, useful-object icons và dấu tương tác dùng cùng hình mềm. **Cần quan sát:** có thực sự khác14/3 về phép nhìn, người/lối đi còn rõ? **Nguy cơ:** chỉ ra ảnh “tiny planet”, hình cong làm mất hướng, mọi người thành cùng khuôn mặt, camera có vẻ lạ nhưng không hỗ trợ đọc cuộc gặp. Chưa có camera động, rig hoặc collision trên mặt cong.

## 4. Địa hình đa diện — `cubist-space`

**Giả thuyết:** game có thể tổ chức một cảnh bằng nhiều mặt nhìn2D cùng tồn tại, làm người chơi đọc nơi chốn qua hình và nhịp, thay vì cố mô phỏng một camera vật lý tự nhiên.

Mặt sàn, mái, vách, lối và nhóm người được ghép thành các mặt có chủ ý. Không chỉ chẻ texture thành tam giác. Vẫn cần một vùng chân đứng và hướng tiếp cận chủ đạo để thấy đây là một trạng thái chơi, không một bức tranh lập thể vô định. Chỗ đổi mặt nhìn là ngữ pháp presentation, không tạo cầu thang bất khả thi hoặc nối hai nơi không có route hợp lệ trong world state.

Nhân vật có thể dùng mặt nghiêng/chính diện hoặc góc vai phối hợp theo nét/mảng riêng, nhưng cùng người không tự bị tách thành nhiều actor. Portrait và icon lấy cùng bộ mặt phẳng, vùng lựa chọn có khung gãy/cắt nhưng chữ không bị xoay vụn. Cần vùng yên giữa những mảng mạnh; tránh dùng mọi màu/mặt cùng mức quan trọng.

Nguồn mới chính ở đây là một phương pháp sáng tạo thị giác, không khẳng định nghệ thuật Văn Lang từng dùng đúng hệ phối cảnh này. Chi tiết mái/công cụ/động tác có thể bám hồ sơ văn hóa rồi được biến đổi. **Cần quan sát:** người chính, người đang nói và vật cần nhìn có rõ khi nhiều mặt nhìn cùng xuất hiện? **Nguy cơ:** thành collage keyart, đường đi giả dẫn tới hiểu sai, mặt cắt mở thông tin mà nhân vật chưa biết. Chưa có hệ camera hoặc walk graph mới.

## 5. Làng của bóng — `shadow-world`

**Giả thuyết:** bỏ phần lớn màu/texture vật thể, để mặt khối và bóng định nghĩa một thế giới3D gần trắng có thể tạo cách đọc không gian và con người khác những bản mực/khắc phẳng trước.

Nhà, người, cây và vật có thể gần trắng; bóng tiếp xúc, mặt tối và khoảng âm giúp nhận chân/sàn/cột và đường tiếp cận. Đây là câu hỏi về **thể tích3D gợi bằng ánh sáng**, không chỉ chuyển một tranh thành đen trắng. Một điểm màu hoặc dấu hình có thể nhận protagonist và tương tác; không dùng màu đơn độc làm nguồn nhận diện.

Người vẫn cần dáng tuổi/thể trạng/công việc và mặt có cá tính; portrait/icon cùng grammar khối–bóng. UI có thể đặt ở vùng âm/yên, phản hồi bằng dấu và chữ rõ, không trở thành bóng trang trí khó đọc. Không đặt tất cả chi tiết vào bóng đặc hoặc làm vật trắng mất vào nền trắng.

Bóng là lựa chọn rendering đề xuất, **không phải ma thuật, thân thể bóng, cơ chế vô hình hoặc cách đọc hidden truth**. Phải giữ ranh giới cái đang thấy và cái chưa biết; không vẽ bóng của NPC sau tường như một lời giải bí mật. **Cần quan sát:** có đọc được người/vật và chiều sâu mà không cần texture? **Nguy cơ:** giống ảnh mô hình kiến trúc, mất gương mặt, phụ thuộc một góc nắng duy nhất. Một frame ban ngày chưa chứng minh diện mạo ban đêm hoặc thời tiết.

## 6. Thủy kính — `mirror-water`

**Giả thuyết:** cấu trúc màn hình dựa trên mặt nước và phản chiếu có thể tạo một lối quan sát mới, nơi cách giữ hướng và nhận người quan trọng ngang vẻ đẹp hình.

Thử góc nhìn qua mặt nước ở một không gian sông/đầm địa phương. Phản chiếu là phần chính của bố cục, người/vật được cô đọng bằng mảng/nét riêng thay vì phản xạ photoreal. Có thể giữ một dấu trực tiếp ở mép cảnh để nối protagonist với hình phản chiếu, giúp biết ai đang nhập vai và hướng tới đâu. UI chữ đứng thuận, không lật ngược toàn bộ lựa chọn chỉ vì hình phản chiếu.

Hệ mặt, cử chỉ, icon và khung UI cần cùng ngôn ngữ nước–mảng–nét, không chỉ đặt một lớp ripple lên tranh cũ. Nhấn tương tác địa phương và người đang quan sát; phản chiếu không là portal tới vùng khác, một hệ soi tương lai hoặc nguồn cho thấy bí mật xa nhân vật. Đây là phép trình bày để thử, **không mở quyền biết thông tin ngoài projection, không thêm supernatural fact hoặc vật lý nước mới**.

**Cần quan sát:** người chính/người đang nói có nhận được, có phân biệt hình trực tiếp với phản chiếu, lối tiếp cận có đọc được? **Nguy cơ:** thành tranh đẹp nhìn qua hồ, điều khiển/hướng mơ hồ, hiệu ứng gợn phá chữ/mặt, hình vô tình kể rằng nhân vật có quyền nhìn xuyên hoặc bóp méo physical truth. Chưa có camera, reflection shader, chuyển động nước hay gameplay qua gương.

## Nguồn tham chiếu → phép biến đổi → dấu hiệu cần nhìn

Hồ sơ căn cứ là [Workshop định hướng](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [Ngữ pháp văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md). Task brief này không có nghiên cứu khảo cổ hoặc review ảnh hiện vật mới.

| Tham chiếu đã có | Phép biến đổi mới của ta | Dấu hiệu cần kiểm trong output |
|---|---|---|
| Hồ sơ khối đất/công trình, tổ chức lao động và vùng Loa Vực trong workshop/GDD | Kéo tổ chức hình theo chiều đứng, gom tầng/hiên/đường thành nhịp | Thành dựng đứng vẫn có đời sống con người, không chỉ tường/hố và texture; không nhận hình học phóng đại là khảo cổ |
| Nhịp hình học, thuyền, đường nước và useful objects trong hồ sơ ngữ pháp | Ghép footprint và khoảng nước thành những mảnh có vai trò, đưa cùng hình vào cast/icon/UI | Ghép đảo đọc được địa bàn và người; không đồng hóa mảnh hình với luật hex hoặc quyền quản lý toàn tri |
| Băng/vành/nhịp vòng làm nguồn liên tưởng bố cục, cùng hình mái–sàn–cột | Uốn phép nhìn và giữ khối tròn xuyên thế giới/UI | Độ cong ở toàn trường cảnh, không chỉ đường uốn; không biến nguồn motif thành cosmology |
| Dáng tương tác và điểm tựa cơ thể | Thử lại người bằng mặt phẳng đa góc, bóng hoặc phản chiếu | Tư thế/người nói vẫn nhận được dù medium/camera đổi; không tạo clone actor hoặc thêm thông tin bí mật |

Cubist-space, shadow-world và mirror-water là hướng sáng tạo mới được đặt trên nền game; không có nguồn nào ở đây chứng nhận chúng là phong cách lịch sử. Phục trang, hình nhà, palette, kiến trúc và dụng cụ cụ thể cần provenance riêng khi được đào sâu. Văn hóa đi qua lựa chọn hình và hành động, không chỉ hoa văn phủ bề mặt.

## Kiểm sau khi có ảnh

Reviewer cần mở artifact thật và ghi đúng ý định/đầu ra đã thấy/kiểm kỹ thuật/quyết định chủ dự án. Kiểm ở kích thước xem dự kiến: ai đang nhập vai, chân đứng ở đâu, nói với ai, vật nào hữu dụng, hướng tiếp cận nào có thể hiểu; chữ/icon không tranh với hình. Đặc biệt kiểm các nhầm lẫn: kiến trúc kỳ ảo thành world geometry mới; ô hình thành luật di chuyển; phép cong thành hành tinh; đa góc nhìn thành route bất khả thi; bóng/phản chiếu thành khả năng biết điều chưa biết.

Không dùng đủ sáu PNG hoặc đủ nhãn để tự xác nhận táo bạo/phù hợp. Một frame mỗi hướng chưa chứng minh bộ asset xuyên toàn game, mọi trạng thái UI, roster, animation, camera động hoặc trải nghiệm chơi. Root lưu prompt, original PNG và provenance; observations/reviews phải ghi chỗ lệch cụ thể, không tự loại bản cũ.

Ba hướng14/1–3 giữ ưu tiên đào sâu theo user. Các bản01–14 cùng tồn tại, những hướng còn lại không bị loại. Random-origin, no-runtime-LLM, deterministic/causal rules, knowledge isolation và ownership giữ nguyên. Tất cả sản phẩm vòng15 vẫn **Pending user approval**; không tự merge dev, đóngSprint13, mởSprint14 hoặc tạo release.
