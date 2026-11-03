# Phản biện thư viện hình ảnh — cần đổi cách biểu diễn, không chỉ chất liệu

30/10/2026 · Sprint13 / SPIN-029 · Actor `/root/art_rethink_audit`.

Yêu cầu hiện tại là brainstorming về đồ họa game và cảm giác Văn Lang–Âu Lạc. Nhận xét dưới đây phục vụ định hướng tiếp theo, không loại bỏ phiên bản cũ, sửa lịch sử hoặc duyệt sản phẩm. Không sinh ảnh, model hoặc viết implementation trong task này.

## Phạm vi đã thực sự xem

Đã dùng `view_image` mở nguyên ảnh của bảy artifact đại diện:

| Artifact | Quan sát trực tiếp |
|---|---|
| `prototypes/sprint13/experiment-03/assets/b-gouache.png` | Cảnh bến có đường đi sáng, thuyền đang sửa, người phơi lưới; chất tranh, nhiều chi tiết tự nhiên. Đây là tranh có điểm tiếp cận rõ, nhưng chưa phải khung hiển thị gameplay đã chứng minh. |
| `prototypes/sprint13/experiment-08/assets/c-faceted.png` | Đá và lá có cạnh/facet, nhưng nhà, người, tỷ lệ và ánh sáng còn gần cảnh tự nhiên; thay mặt vật liệu chưa làm toàn thế giới trở thành hệ khối giản lược. |
| `prototypes/sprint13/experiment-09/assets/b-bronze.png` | Cảnh, người, portrait, icon, khung UI cùng chất phù điêu đồng. Medium thống nhất rõ; đồng thời cạnh sáng và hoa văn trải rộng, vật liệu đồng phủ lên mọi thứ khiến người/UI/nền tranh nhau. |
| `prototypes/sprint13/experiment-11/assets/stylized-3d.png` | Mặt người, da, vải, rổ và mái nhà tả chi tiết; camera phối cảnh và người có tỷ lệ tự nhiên. Nhãn “cách điệu” chưa được biểu hiện mạnh bằng hình khối hoặc tỷ lệ. |
| `prototypes/sprint13/experiment-11/assets/pixel-life.png` | Cụm pixel xuất hiện thật, người nhỏ có silhouette rõ hơn bản tả thực. Bố cục vẫn là làng/kho/cầu giàu chi tiết, UI panel đen viền vàng và các ô khá quen; portrait trong hộp thoại không tự làm rõ ai đang nói. |
| `prototypes/sprint13/experiment-12/assets/clay-craft.png` | Sân gốm có khối và vật liệu rất rõ, nhưng da/vải/khuôn mặt/đồ vật vẫn tiến gần miniature tả thực; blur tiền cảnh tạo cảm giác ảnh chụp mô hình. |
| `prototypes/sprint13/experiment-12/assets/graphic-animation.png` | Người nặng/gầy đối lập, portrait có biểu cảm, UI và icon giản hơn rõ. Cảnh phơi thóc còn nhiều mái, giỏ, hạt, bóng và phối cảnh; mức giản lược không đồng đều giữa người và thế giới. |

Đã đọc prompt12, art direction08, báo cáo04/05 và hướng dẫn project/skills liên quan. Không tuyên bố đã xem toàn bộ ảnh01–12 hoặc đã thao tác các viewer trong lượt audit này. Báo cáo04 ghi có scene/geometry Blender thật;05 ghi model Hyper3D thật nhưng lệch brief. Cần giữ lịch sử đó đúng, đồng thời thừa nhận rằng có geometry thật không tự đáp ứng cảm giác game mà user đang yêu cầu. Phần lớn preview từ06 trở đi là ảnh mô phỏng, không phải runtime3D.

## Đồ họa cần được tách thành các quyết định cụ thể

“Đẹp”, “3D”, “pixel”, “anime”, “đồng” đang bị dùng như các phương án cùng một cấp. Thực ra chúng trả lời những câu hỏi khác nhau. Một game3D có thể rất phẳng, ít màu, hình người méo có chủ ý; một game2D có thể tả thực và tạo chiều sâu rất mạnh. 3D không phải bậc cao hơn của mức giống ảnh thật.

| Trục | Câu hỏi phải trả lời | Ảnh concept có thể cho thấy | Chưa chứng minh bằng một ảnh |
|---|---|---|---|
| Không gian biểu diễn | Thế giới phẳng, các lớp2.5D hay khối3D? | Occlusion, cấu trúc hình, cảm giác chiều sâu được đề xuất | Geometry thật, collision, camera tự do |
| Camera và tỷ lệ | Nhìn cao/thấp, gần/xa; người lớn bao nhiêu trong khung? | Một framing cụ thể, silhouette tại kích thước đó | Camera giữ được thông tin khi di chuyển |
| Quy tắc hình khối | Đầu, thân, bàn tay, mái, cây và thuyền được giản lược/phóng đại thế nào? | Sự nhất quán của một khung/mẫu cast | Bộ asset tái sử dụng, rig, animation |
| Cách tạo bề mặt và ánh sáng | Màu phẳng, ít cấp bóng, nét, pixel hay texture/PBR? | Mức tả thực và hierarchy nhìn thấy | Shader/runtime lighting hoặc grid production |
| Ngôn ngữ văn hóa | Cái gì khiến hình có nguồn gốc riêng ngoài trang trí? | Nhịp hình, silhouette và mối quan hệ không gian do art direction đề xuất | Độ chính xác lịch sử; phải đối chiếu nguồn riêng |
| Cách đọc hành động | Biết đi đâu, nói với ai, chọn gì, thấy hậu quả thế nào? | Một trạng thái cụ thể và vị trí feedback | Input thật, thay đổi state thật hoặc knowledge isolation |
| Chuyển động | Nhịp bước, cử chỉ, nước/lúa và phản hồi ra sao? | Pose hoặc các keyframe có chủ ý | Animation hoặc game feel khi vận hành |
| UI và dữ liệu | Thông tin nào hiện lúc nào và thuộc tri thức của ai? | Kiểu bố cục/chữ/icon và ví dụ trạng thái | Dữ liệu thật, chữ reflow, accessibility hoặc UX runtime |

Bước tiếp theo nên gọi tên đề xuất như “camera cao orthographic + nhân vật khối lớn + hai cấp bóng + UI phẳng”, thay vì chỉ “bản3D đẹp hơn”. Với ảnh mô phỏng, ghi rõ đang minh họa diện mạo của một hướng3D; không dùng tên đó để ám chỉ đã dựng scene hoặc pipeline.

## Vì sao các hướng vẫn dễ quay về cùng một cảm giác

**1. Brief chung kéo hình về chủ nghĩa tự nhiên.** Prompt12 liên tục yêu cầu gương mặt có tuổi, bàn tay lao động, vật liệu thủ công, môi trường sống đầy đủ và vật dụng nhận được. Đây là các ý tốt, nhưng khi ghép với khung cảnh nhiều đồ và portrait cận, generation dễ ưu tiên nếp da/vải/tre hơn quy tắc tạo hình. “Người trưởng thành có đời sống” không bắt buộc giải phẫu hoặc bề mặt gần ảnh thật. Tuổi có thể thể hiện bằng dáng lưng, độ nghiêng đầu, tỷ lệ, nhịp bước và hai nét mặt.

**2. Hình khối mặc định vẫn gần tự nhiên.** Facet chỉ xuất hiện trên đá/lá ở08C; vật thể lớn, vóc người và phối cảnh còn khá cũ. Clay12 đổi xúc giác, chưa đổi độ lớn đầu/tay, silhouette hay cấu trúc vật. Nếu hình thể không thay, những medium khác nhau dễ giống một cảnh được render bằng nhiều vật liệu.

**3. Bối cảnh lặp làm thời đại bị hòa tan.** Bến sông, mái rạ, tre, giỏ, áo tối và nón tạo một cảm giác làng nhiệt đới quen thuộc. Chúng có thể hữu ích cho đời sống, nhưng riêng tổ hợp đó chưa làm người xem nhận ra Văn Lang–Âu Lạc. Không nên lấy những ảnh này làm chứng cứ cho trang phục/kiến trúc chính xác. Cần nghiên cứu ngôn ngữ gốc và cách chuyển hóa thành hình game, không chỉ thêm chim/trống vào góc UI.

**4. Board toàn hệ thống đang lấn câu hỏi khung chơi.** Cấu trúc cảnh trên65%, dải cast/portrait/icon/UI dưới35% cho biết có chung medium; nó cũng giảm diện tích và sự chú ý dành cho khung chơi. Việc đếm đủ hai người/ba portrait/sáu icon không trả lời người chơi đang điều khiển ai hoặc vùng nào cần chú ý. Board này nên là hồ sơ theo sau một key screen thuyết phục, không phải định dạng bắt buộc của mọi lượt ý tưởng.

**5. UI thường thay da hơn thay cách cung cấp thông tin.** Bản đồng09 có nhiều biên, vòng và sáng cạnh;11/12 thường quay về hộp portrait + ba lựa chọn + grid túi đồ. Có thể dùng cấu trúc quen thuộc nếu phục vụ đọc, nhưng vật liệu đồng/vải không tự cho UI bản sắc hoặc làm rõ speaker. Cần thiết kế một trạng thái có mục đích, chẳng hạn đang nghe tin từ một người và phân biệt lời nghe với điều đã tự quan sát; dữ liệu bí mật engine vẫn phải được giấu.

**6. Không có nhịp hành động trong chứng cứ ảnh.** Các pose làm việc và cảnh sinh hoạt gợi câu chuyện, nhưng chưa chỉ ra hình chuyển thế nào khi nhặt đồ, quay sang nói chuyện hoặc chờ nước đổi. Đây là khoảng trống của loại artifact hiện tại, không phải bằng chứng một hướng không thể làm game. Chưa cần dựng engine; có thể đề xuất các keyframe kế tiếp và nói rõ đó là ý định chuyển động.

**7. Mức chi tiết dễ được dùng thay cho mức hoàn thiện.** Nhiều nan tre, cọng rạ, nếp áo và texture giúp tranh có cảm giác công phu. Với game, hoàn thiện còn nằm ở việc nhân vật tách nền, lựa chọn đọc được và một trạng thái thay đổi được nhận ra. Đừng dùng việc ảnh ít chi tiết làm lý do tự gọi nó sơ sài; cần xem những đường nét còn lại đã được chọn có chủ ý chưa.

## Những cách thể hiện đáng đưa vào tranh luận

Các hướng dưới đây là đề xuất workshop, chưa là thiết kế chốt hay bằng chứng Văn Lang–Âu Lạc chính xác. Chất liệu văn hóa cần đối chiếu hồ sơ nghiên cứu riêng. Không bắt mọi hướng phải dùng toàn bộ trò chơi giống nhau; có thể phân vai giữa world, ký ức/chronicle và UI mà vẫn cùng quy tắc hình.

| Hướng | Quy tắc thế giới/người | Camera/UI/feedback dự kiến | Vì sao khác các vòng cũ | Phản biện cần giải quyết |
|---|---|---|---|---|
| **Khối lớn, bóng gọn** | Người đầu/thân được nén có chủ ý, bàn tay/công cụ lớn vừa đủ đọc nghề; mái/cây/thuyền gồm ít khối rõ. Đề xuất hai hoặc ba cấp sáng, gần như không vân da/vải. | Camera cao gần orthographic; nền chia vùng yên, UI phẳng sáng với icon cùng silhouette. Cử chỉ quay đầu, cúi, nhấc đồ được phóng đại vừa phải. | 3D appearance được nhận qua thể tích/che khuất/contact shadow, còn cá tính đến từ thiết kế khối; không chạy theo mặt thật hoặc shader đá facet riêng lẻ. | Có nguy cơ thành đồ chơi nếu ánh sáng studio, đế trưng bày hoặc tỷ lệ đầu quá lớn. Nội dung trưởng thành phải nằm ở pose/nhịp/quan hệ, không kéo lại texture để “nghiêm túc hóa”. |
| **Dáng người thành ký hiệu sống** | Người nhìn nghiêng, dáng nghề và cử chỉ đọc bằng silhouette; mặt ít nét. Cảnh gồm dải nền/lớp chồng có nhịp, vật dụng thành dấu hình mạnh. | Camera ngang/chếch rất nông; đường đi được thiết kế như một dải có các điểm tương tác. UI dùng nhịp vạch, dấu chọn và khoảng trắng, không bọc toàn cảnh trong đồng. | Chuyển cấu trúc biểu đạt của hình tượng thành quy tắc thiết kế, thay vì phủ texture đồng lên cảnh phối cảnh tả thực. | Hạn chế độ sâu/đi lại có thể làm thế giới ít mở; cần phân biệt nghệ thuật biểu tượng với việc người dân thật trông như thế. NPC vẫn cần cá tính và khả năng đọc tâm trạng. |
| **Bản đồ cư trú có người sống** | Top-down gần phẳng; nhà, bờ nước, ruộng và đường là mảng lớn. Nhân vật nhỏ nhưng đầu/tay/dụng cụ có nhịp rõ, dùng màu vai trò tiết chế. | World view ưu tiên đường đi/cộng đồng, hội thoại chuyển sang portrait đồ họa có cùng khối. Trạng thái công việc đổi bằng pose hoặc vật đặt ở nơi đã biết, không bật mọi bí mật thế giới. | Thay góc nhìn và lượng thông tin theo gameplay; không cần vẽ phong cảnh như qua ống kính. | Có thể giống game quản lý nếu UI biểu đồ tràn lan; phải giữ việc người chơi là một cuộc đời cụ thể, không là quản đốc toàn tri. |
| **Sân khấu hình khối và khoảng trống** | Vật thể có chủ ý phóng đại hoặc lược bỏ theo câu chuyện: mái lớn, bến dài, người và đồ nghề gọn. Cảnh chỉ giữ vài hình chủ đạo; thiên nhiên là lực hình/nhịp thay vì vô số lá. | Góc máy cố định theo từng khu, dùng mảng trước/sau để đi vào chiều sâu. UI nhỏ, không cố đeo khung trang trí. Có thể đề xuất gió/nước theo nhịp hình, chưa tạo animation trong workshop. | Tính game đến từ staging và sự tương phản giữa nơi đi được/nơi nhìn xa; không phải thay gouache bằng một bề mặt khác. | Nếu phóng đại tùy ý có thể thành đẹp nhưng khó suy khoảng cách. Phải kiểm đường đi, che khuất và đọc người ở cùng kích thước hiển thị. |

Không đề nghị bỏ pixel, anime, tranh hoặc các bản tham khảo cũ. Một bộ quy tắc hình tốt có thể được thực hiện bằng nhiều kỹ thuật. Điều cần chọn trước là trải nghiệm nhìn/đọc thế giới; tên renderer hoặc medium theo sau.

## Đổi brief và phương pháp ngay ở lượt thử tiếp theo

1. **Làm khung chơi trước bảng catalog.** Mỗi hướng trước tiên có một gameplay keyframe riêng, toàn khung và ở cùng kích thước mục tiêu. Có nhân vật người chơi đọc được, một việc cụ thể, một người có thể tương tác, đường tiếp cận và một trạng thái UI. Chưa cần nhồi mọi màn hình vào cùng ảnh. Sau đó làm sheet cast/icon/UI để kiểm khả năng đi xuyên toàn game; sheet vẫn cần thiết, chỉ đổi thứ tự.
2. **Chỉ ra quy tắc bằng hình thức cụ thể.** Viết rõ độ lớn người trong khung, tỷ lệ đầu/thân dự kiến, số mảng bóng, mức texture, thứ bậc nét và độ rộng vùng yên. Các con số là giả thuyết của riêng thử nghiệm, không tự thành production budget. Đừng dùng mỗi tính từ “stylized”, “bold”, “beautiful”, “high-end” để giao quyết định cho công cụ.
3. **Cho phép người trưởng thành được cách điệu.** Ví dụ, người già nhận qua dáng còng/độ nghiêng đầu/cử chỉ chậm; người thợ nhận qua dụng cụ và tư thế. Không yêu cầu mọi portrait phải có từng nếp nhăn hoặc mọi bàn tay phải tả chi tiết. Không buộc nhân vật lâm vào mẫu anh hùng trẻ; tuổi và nghề vẫn đa dạng.
4. **Thử khác cấu trúc nhìn trước khi khác màu.** Giữ một tình huống nguồn để dễ so, rồi thay đúng camera/tỷ lệ/hình khối. Các ảnh độc lập vẫn có thể lệch nhau; ghi điều này thay vì tuyên bố phép thử kiểm soát hoàn toàn. Chỉ sau khi cấu trúc rõ mới thử palette và vật liệu.
5. **Dùng một mini-sequence nếu cần hỏi về game feel.** Đề xuất ba trạng thái quan sát → chọn → phản hồi, với cùng nhân vật/tình huống được mô tả. Các ảnh keyframe là hình minh họa ý định, không gọi là animation, trạng thái authoritative thật hoặc bằng chứng khả năng giữ scene3D. Không cần tự tạo model thật cho bước này.
6. **Kiểm ở kích thước xem, không chỉ mở original.** Hỏi người xem có chỉ ra người chơi, lối đi, người nói và điều mới thay đổi không. Nhận xét ở khung480/960px có giá trị về độ đọc, nhưng không thành human playtest nếu chưa có tester thật. Không lấy việc gallery tảiPNG thành gate mỹ thuật.
7. **Giữ provenance trung thực và từ ngữ chính xác.** “Ảnh gợi đồ họa3D cách điệu” là một loại preview hợp lệ; “game3D đang chạy” cần bằng chứng khác. Với workshop này không tạo model, không gọi Hyper3D/Blender và không đổi techstack. Các sample nhân vật nhận được ngẫu nhiên không biến thành màn hình chọn origin.

Một brief rút gọn có thể dùng làm đầu vào tranh luận:

> Tạo một khung giả lập gameplay với nhân vật trưởng thành có hình khối giản lược mạnh. Camera cao gần orthographic; người đọc được khi ảnh thu nhỏ. Thế giới, người và công cụ dùng cùng ngôn ngữ ít khối, hai cấp bóng, không vân da/vải. Có một lối tiếp cận rõ, một tình huống đời sống có lựa chọn, UI phẳng tiết chế thể hiện nguồn tin và người nói. Dựa trên bộ tham chiếu Văn Lang–Âu Lạc đã phân loại bằng chứng; những chi tiết sáng tạo phải ghi là đề xuất. Đây là ảnh xem trước một hướng game, không phải scene3D hoặc runtime đã dựng.

Brief này không tự chốt camera3D, tỷ lệ, lịch sử, phong cách hay hướng cuối. Nó chỉ sửa câu hỏi mà vòng thử cần trả lời. Tất cả artifact01–12 cùng được giữ; quyết định cuối thuộc chủ dự án theo approval register.
