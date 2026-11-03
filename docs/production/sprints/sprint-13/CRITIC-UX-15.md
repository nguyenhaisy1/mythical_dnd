# Phản biện hình ảnh15 — nhánh đào sâu và cách nhìn mới

02/11/2026 · Sprint13 / SPIN-032 · Reviewer độc lập `/root/s13e15_critic_visual` · **Visual reviewed trong phạm vi ảnh và screenshot đã xem; Pending user approval**.

## Phạm vi thực sự xem

Lượt review bắt đầu30/10, bị ngắt trước khi viết hồ sơ và tiếp tục02/11. Reviewer đã mở trực tiếp ba PNG vòng14 được chủ dự án ưu tiên: `earth-monument.png`, `living-grid.png`, `round-world.png`; mở đủ bảy PNG vòng15, gồm sáu frame đang trưng bày và `cubist-space-v1.png` giữ riêng. Khi tiếp tục đã mở lại cả bảy PNG15. Đối chiếu brief `experiment-15/ART-DIRECTION.md`, prompt/provenance, workshop ART-DIRECTION-RETHINK và ranh giới baseline/validation; nhận xét bên dưới căn cứ hình thật, không lấy prompt làm bằng chứng đầu ra đã đạt.

Reviewer thực sự mở ba screenshot cuối do critic code tạo trong `tests/results/sprint13-experiment15/`:

- `gallery-rolling-world-1280.png` — gallery desktop, hướng03 đang chọn.
- `gallery-cubist-space-1920.png` — gallery desktop, hướng04 đang chọn.
- `gallery-mirror-water-390.png` — gallery mobile, hướng06 đang chọn.

Đã đọc `browser.json` và CRITIC-CODE-15 để phân biệt bằng chứng nguồn/kỹ thuật với đánh giá hình. **Không tự chạy browser automation, không mở toàn bộ21 screenshot, không thực hiện playtest.** Kiểm hash/byte/mapping prompt và tương tác gallery thuộc critic code; review này không nhận lại các phép kiểm đó thành thao tác của mình. Hai PNG vòng14 đầu được xem ở phần trước lượt ngắt; PNG14/3 và toàn bộ PNG15/screenshot nêu trên được xem trong lượt tiếp tục. Không tạo/sửa ảnh hoặc model.

## Đối chiếu đầu ra với câu hỏi thiết kế

| Artifact | Điều thực sự nhìn thấy | Giới hạn và quan hệ với mẫu trước |
|---|---|---|
| `vertical-earth.png` — Thành dựng đứng | Sàn/hiên xếp cao, cột lớn, thang và đường dốc tạo nhịp đứng rõ; người mang bình dài, gầy và người ngồi vóc rộng tạo cuộc gặp cục bộ. Dải UI đứng và portrait góc cạnh nối được hình người/công trình. | Khác14/1: công trường hố đất rộng chuyển thành tầng sinh hoạt cao sát người. Có đào sâu tổ chức không gian; chưa chứng minh các sàn nối thành đường đi hợp lệ. Nhiều gỗ, dây và texture; mức giản lược chưa mạnh. Người ngồi có râu, không xác nhận cast đúng mọi ý định prompt. |
| `tidal-mosaic.png` — Ghép đảo thủy triều | Mảnh đất sáng không đều, kênh xanh và bến/thuyền chia địa bàn; một người có dấu chọn cạnh người cầm sào. Portrait, túi/lưới và UI đỏ sẫm–ngà thống nhất với cảnh. | Khác14/2: lưới ruộng muối gần nhìn thẳng trên chuyển thành bến ghép mảng. Tuy nhiên camera hiện **xiên, có vách đất/mặt nhà và người có thể tích**, không phải true overhead. Nhiều chi tiết sinh hoạt làm hình bớt thuần mosaic. Không thể kết luận luật ô/triều hoặc khoảng cách. |
| `rolling-world.png` — Làng trên vành trống | Mặt đất/horizon cong rõ sang hai mép, trục cây/cột và mái tham gia phép nhìn; UI chạy cung ở đáy. Bà tóc trắng vóc thấp rộng, ông già gầy, giỏ lá và portrait giữ được một cuộc gặp gần. |14/3 có đường làng uốn nhưng không cho thấy rõ phép cuộn toàn trường cảnh;15/3 đã tạo khác biệt nhìn thấy được. Đây là **ảnh tĩnh biến dạng phép nhìn**, chưa là camera cuộn chạy thật hoặc thế giới vật lý hình cầu. Nan giỏ, mái và lá vẫn chi tiết hơn ý định khối matte gọn. |
| `cubist-space.png` — Địa hình đa diện, v2 | Sân trái gần mặt bằng nhìn từ trên, hiên giữa gần chính diện, mặt người nghiêng khổng lồ bên phải; mảng lớn/vùng yên và UI mặt phẳng đồng bộ hơn v1. Có người mang chiếu nhỏ được đánh dấu. | Có khác biệt đa tỷ lệ/mặt nhìn thật trong hình. Nhưng phần phải cũng có thể được đọc là **portrait hoặc lớp minh họa**, chưa buộc người xem hiểu cùng một không gian đã gấp. Người bán ở hiên, khuôn mặt lớn và portrait UI không chứng minh có ba actor. Lối tiếp cận sân–hiên chỉ là gợi ý hình. |
| `cubist-space-v1.png` — biến thể giữ lại | Người/thảm/mái cùng nét cắt và palette đỏ–ngà–xanh nhạt; hai người trao đổi và hoạt động dệt rõ. | Phối cảnh phần lớn còn thống nhất; đa diện chủ yếu ở tạo hình/bề mặt. v2 làm rõ khác biệt mặt nhìn hơn. v1 vẫn là bản tham khảo được giữ, không tự là sản phẩm user bác bỏ hoặc bản cần xóa. |
| `shadow-world.png` — Làng của bóng | Cảnh/người gần trắng, bóng tiếp xúc và bóng đổ cho biết chỗ đứng; điểm xanh trên người và UI giúp nhận trọng tâm. Portrait, nút đen, mái chèo và túi cùng hệ trắng–đen. | Cách đọc ánh sáng có giá trị, nhưng camera, giải phẫu và mật độ vật liệu vẫn gần cảnh bến thông thường. Novelty hiện mạnh ở palette/vật liệu hơn cấu trúc không gian. Chưa đủ bằng chứng để gọi hướng này là một bước đổi ngữ pháp lớn ngang phép cong hoặc ghép mặt nhìn. Không phải mô hình3D thật chỉ vì giống maquette. |
| `mirror-water.png` — Thủy kính | Nước/phản chiếu chiếm phần lớn khung; hai người trực tiếp ở mép dưới, hình đảo/ngắt bởi vòng gợn, focus vàng và UI chữ đứng thuận. Đây là đổi cấu trúc nhìn thấy được, không chỉ đổi màu làng. | Chưa nối rõ từng thân người thật với phản chiếu và người được focus; không xác minh quang học đúng, hướng điều khiển hoặc người đang nhập vai chỉ từ ảnh. Biểu tượng túi cạnh chữ “Nhật ký” gây nhập nhằng nghĩa. Không suy thành gương ma thuật hay truy cập vùng xa. |

Ba nhánh đầu giữ được quan hệ ý tưởng với14/1–3 nhưng đều là generation mới; không có bằng chứng giữ identity/camera/geometry của ảnh trước theo kiểu asset matching. User ưu tiên ba mẫu14 là căn cứ tiếp tục nghiên cứu; chưa xác định riêng camera, màu hay vật liệu là nguyên nhân được thích.

## Findings và câu hỏi cần mang theo

**P2 — Khoảng cách giữa tên phương pháp và output chưa đều.** `tidal-mosaic` còn góc xiên; `shadow-world` còn gần một cảnh được làm trắng, nhiều hình quen thuộc và texture. Khi giới thiệu cần giữ nhãn ý định bên cạnh quan sát thật, không đếm đủ sáu tên thành sáu đột phá đã đạt. Gallery hiện đã có chú thích các lệch này, nên đây là giới hạn kết quả mỹ thuật còn mở, không blocker xuất bản thư viện ảnh.

**P2 — Không gian đa diện và phản chiếu chưa cho một cách đọc duy nhất.** Ở `cubist-space`, hình người bán lớn bên phải có thể là portrait overlay; ở `mirror-water`, focus và thân người thật chưa được nối chắc. Nếu một hướng được chọn đào sâu, lượt kiểm tiếp nên làm rõ cùng người/cùng nơi bằng dấu hình hoặc một cặp frame tĩnh có quan hệ rõ, rồi hỏi ai đang đứng đâu/nói với ai. Đây là đề nghị nghiên cứu sau, không yêu cầu tạo model, thêm chi phí hoặc implementation hiện tại. Chưa có bằng chứng thay route, gravity, physics hay quang học gameplay.

**P2 — Một số ký hiệu giao tiếp/nhập vai còn mơ hồ.** Bubble “Lối lên ở đâu?” ở `vertical-earth` chỉ về người ngồi; không mặc nhiên gán câu hỏi cho người mang bình. Tim coral ở `rolling-world` có thể được hiểu là tình cảm/trạng thái hơn dấu nhập vai. `mirror-water` dùng icon túi cho Nhật ký, trong khi UI các bản khác thường dùng sách. Nên ghi đây là câu hỏi ngữ nghĩa của UI raster, chưa là bug handler hoặc quan hệ tình cảm đã tồn tại. Một thư viện icon production cần phân biệt protagonist, target tương tác và speaker; các frame này chưa có bộ quy tắc đầy đủ ấy.

**P3 — Tính thống nhất có ở từng tình huống, chưa là hệ hình toàn game đã chứng minh.** Cả sáu hướng có cảnh, người, cận mặt/dấu nhận diện, vật hữu dụng và UI cùng palette/form ở mức quan sát được. Khối mềm của rolling và mặt phẳng của cubist dễ nhận hơn; texture gỗ/nan/lá của vertical, tidal, rolling và shadow vẫn kéo hình về độ dày minh họa. Một số người nhỏ/che mặt không đủ dữ kiện tuổi; không suy roster có đủ tuổi/thể trạng chỉ từ prompt. Chưa thấy mọi trạng thái túi đồ/nhật ký/hội thoại, cast ngẫu nhiên hoặc cùng asset qua nhiều góc. Không bắt các frame phải thêm bảng asset cố định để bù cho giới hạn này.

**P3 — Bản sắc Văn Lang–Âu Lạc chưa được chứng nhận bởi việc có mái cong/hoa văn.** Sinh hoạt mang bình, chèo thuyền, trao lá/chiếu và công trình cộng đồng giữ con người/nghề trong ảnh. Đó là căn cứ thị giác phù hợp câu hỏi RPG đời sống, nhưng không đủ xác nhận niên đại phục trang, dạng mái, kiến trúc khổng lồ, nghề hoặc motif cụ thể. Workshop phân biệt tham chiếu/diễn giải/hư cấu là cần thiết; review này không có khảo cổ hoặc nguồn hiện vật mới. Những phép cong/đa diện/bóng/gương là sáng tạo presentation, không canon lịch sử mới.

## Gallery ở kích thước đã quan sát

Hai screenshot desktop cho thấy frame được giữ trọn trong trang, phần chọn sáu hướng tách khỏi HUD raster, ba nhánh đầu có đường quay lại mẫu14 tương ứng và v1 có link riêng. Thumbnail/caption giúp đối chiếu thư viện; không biến nút gallery thành nút game.

Screenshot mobile390 cho thấy chữ HTML và các card xuống một cột, phần switch hai cột, thông tin pending và lịch sử còn hiện. Artwork thu nhỏ vẫn giữ toàn khung, vì vậy mặt người và UI raster trong ảnh rất nhỏ; không nhận screenshot này là bằng chứng UI game đọc tốt trên điện thoại. Đã xem screenshot chụp sẵn, không tự thao tác modal/zoom hoặc đo contrast/OCR. Nhận định không tràn ngang và các nút hoạt động dựa vào scope automation của critic code, không phải thử tương tác do visual critic thực hiện.

## Kết luận trong phạm vi được giao

Không thấy P1 cản chủ dự án xem và so sánh sáu hướng cùng v1. Có bằng chứng tiến triển về phép cong toàn cảnh, tầng không gian và cách đặt phản chiếu/mặt nhìn; mức đổi mới/độ giản lược giữa các hướng không đồng đều. Những findings còn mở đã được ghi rõ, không tự xếp hạng loại bỏ hoặc gọi mọi phương án đạt art direction.

Không phát hiện ảnh tự hiển thị bảng tri thức bí mật hay màn chọn xuất thân trong scope đã xem. Tuy nhiên PNG không kiểm được knowledge isolation, ownership, deterministic rules hoặc hành vi authoritative state. Góc cao, bóng và phản chiếu vẫn cần dùng thông tin được phép nếu được triển khai; không suy rằng các invariant đó đã pass vì chưa thấy lỗi trong hình.

**Technical/gallery review, visual observations và quyết định user là ba lớp riêng.** Chưa có model/scene/render3D thật mới, camera động, chuyển động, gameplay/Unity acceptance, đo hiệu năng, full accessibility hoặc human playtest trong review này. Ba favorite14/1–3 giữ ưu tiên đào sâu; mọi bản cũ và v1 tiếp tục tham khảo. Toàn bộ vòng15 và Sprint13 vẫn **Pending user approval**; review này không cho phép merge dev, đóngSprint13 hoặc chuyểnSprint14.
