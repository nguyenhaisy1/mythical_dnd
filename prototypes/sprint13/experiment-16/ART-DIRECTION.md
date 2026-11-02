# Thử nghiệm16 — Sáu quy tắc tạo hình mới

02/11/2026 · Sprint13 / SPIN-033 · Brief của `/root/s13e16_direction` · **Đề xuất để tạo ảnh, Pending user approval**.

Người chơi vẫn sống một cuộc đời giữa những người khác, có nghề, gia đình và việc cần quyết định. Vòng16 thử thay cách hình ảnh được cấu tạo: dải đan, nét có thể tích, nếp gấp, nêm sáng–mưa, mặt cắt ngang và hạt khối. Chất liệu chỉ có ích nếu thay cả silhouette, không gian và UI. Không dùng cùng một làng rồi phủ sáu texture.

Ba hướng14/1–3 vẫn được ưu tiên đào sâu theo phản hồi thật của chủ dự án. Sáu hướng mới bổ sung thư viện, không thay hoặc loại chúng và01–15. Brief này chưa có kết quả quan sát; tên phương pháp không là bằng chứng ảnh tạo ra sẽ đạt phương pháp ấy.

Không coi sáu nhãn là sáu chất liệu chưa từng thử. Vòng08/B đã có hướng dệt, vòng07 đã thử giấy và vòng11 đã thử pixel. Vòng16 quay lại một số liên tưởng đó để hỏi cấu trúc mới: dải đan dựng footprint/thân người; nếp gấp quay các mặt không gian; hạt lập phương tạo thể tích thô. Chỉ output thực tế mới cho biết delta có đạt; vật liệu quen không tự là thất bại, vật liệu mới cũng không tự là đột phá.

## Mỗi frame cần trả lời được gì?

Mỗi hướng dự kiến một màn hình ở tỷ lệ chơi: một người đang được nhập vai, một cuộc gặp hoặc công việc cục bộ, nơi đặt chân/vật hữu dụng và lựa chọn có thể đọc. Cảnh, toàn thân/cận mặt, icon và UI cùng họ hình trong frame; không ép mọi hướng thành bảng65/35 hoặc số ô asset cố định. Cast là ví dụ cuộc đời được nhận ngẫu nhiên, không nhân vật chính cố định hoặc character creator.

| Key / hướng | Phép nhìn và quy tắc hình | Delta cần thấy so với thư viện cũ |
|---|---|---|
| `woven-world` — Thế giới dệt | Mặt bằng gần trực giao; đường, bờ, người và đồ được cấu tạo bằng những dải đan lớn | Dải đan quyết định footprint và thân thể; không chỉ thêm sợi lên thế giới ký hình/pixel có sẵn |
| `line-volume` — Nét dây không gian | Góc nhìn chéo thấp vừa; các đường cong có bề dày ôm một số mặt đặc, khoảng rỗng có vai trò | Có thể tích và che khuất qua nét, khác khắc nét phẳng hoặc làng trắng/bóng |
| `folded-village` — Làng gấp | Mặt đất/sàn có các nếp gấp kiểu đàn xếp, những mặt quay khác hướng tạo độ sâu có nhịp | Nếp gấp tổ chức địa bàn, khác chỉ cắt nhân vật giấy dựng trên một nền phẳng |
| `rain-sculpt` — Mưa khắc hình | Không gian góc cạnh chia thành các nêm sáng, dải mưa và khoảng âm; ba lớp sâu rõ | Thời tiết tham gia staging và silhouette, khác chỉ đổi tranh ban ngày thành cảnh tối có mưa |
| `living-section` — Mặt cắt đời sống | Góc ngang gần trực giao, hiên và sân công cộng xếp theo cao độ; người khối nặng đọc ngang | Đọc nơi chốn như một dải không gian có chiều sâu nông, khác tranh truyện nhiều ô hoặc thành đứng nhìn từ trên |
| `voxel-life` — Hạt khối | Góc chéo có khối; các hạt lập phương thô tạo người, nhà, thuyền, vật và UI | Bậc khối nhìn thấy trên silhouette/thể tích, khác filter pixel2D hoặc khối đất nặn mềm |

## 1. Thế giới dệt — `woven-world`

**Giả thuyết:** một cảnh sống có thể đọc như cấu trúc đan, nơi đường nước–đường đất và thân người dùng chung nhịp vượt qua/nằm dưới. Không cần ảnh một tấm thảm được chụp trên bàn.

Nhìn gần thẳng từ trên; dùng ít dải rộng và khoảng trống lớn. Các đoạn giao dải phải tạo ranh bờ, lối và vị trí đứng có thể hiểu; tránh hàng nghìn sợi vụn. Người không chỉ là một icon nằm trên thảm: vai, tay mang vật và thân cũng tạo từ dải, nhưng tuổi và dáng vẫn phân biệt bằng tỷ lệ/hình mặt. Chân dung có thể dùng cùng dải rộng, không quay lại portrait vẽ tả thực.

UI dùng các đầu dải, nút thắt hoặc mảng đan làm khung, để chữ Việt ở mặt yên. Icon hữu dụng có silhouette riêng và nhãn, không giao toàn bộ nghĩa cho màu hoặc một họ hoa văn. **Ví dụ đúng:** đường kênh và vai người đều có logic giao dải. **Tránh:** một screenshot thường chỉ được phủ texture vải.

**Cần xem:** có nhận người đang nhập vai, người đối diện và đường tiếp cận trước khi nhận chất liệu không? **Rủi ro:** thành đồ trang trí/puzzle hoặc bản đồ quản lý toàn tri. Dải giao nhau là quy tắc hình, không luật ô, passage hoặc dây liên kết xã hội đã tồn tại.

## 2. Nét dây không gian — `line-volume`

**Giả thuyết:** đường nét có bề dày và vài mặt đặc có thể dựng một nơi có khối, nhẹ và thoáng, nơi cử chỉ quan trọng hơn texture.

Thử góc chéo thấp vừa, mái/cột/thuyền được cô đọng bằng cung và cạnh có thể tích. Các mặt đang thấy che nét phía sau đúng thứ bậc; không hiển thị mọi cạnh như wireframe kỹ thuật. Mặt người, bàn tay, vật đang cầm và điểm tựa cần có mảng đặc để đọc tuổi/nghề; nét rỗng hoàn toàn dễ biến mọi người thành cùng bộ xương. Khoảng rỗng của rendering không là quyền nhìn xuyên công trình hoặc cơ thể.

Khung hội thoại có thể là đường ôm với một mảng nền nhỏ; portrait/icon dùng nét dày và mảng đặc cùng tỷ lệ. Chỉ một vùng dấu nhấn cho cuộc gặp, chữ đứng thuận và tách khỏi mạng nét cảnh. **Ví dụ đúng:** nét mái có cạnh khuất, mặt người vẫn đọc được. **Tránh:** lưới neon, blueprint, scanline hoặc màn chẩn đoán toàn scene.

**Cần xem:** cảm giác có thể tích có đến từ che khuất và cấu trúc nét, hay chỉ từ contour trên một tranh phẳng? **Rủi ro:** rối khi nhiều người, lẫn vật/đường đi, hiểu nhầm thành x-ray. Đây là ảnh đề xuất diện mạo3D, chưa có mesh, shader hoặc scene thật.

## 3. Làng gấp — `folded-village`

**Giả thuyết:** một địa bàn liên tục có thể được diễn đạt bằng các mặt đất/sàn gấp nhịp như đàn xếp, làm nơi chốn có chiều sâu riêng thay vì tái hiện phối cảnh tự nhiên.

Mặt trước–mặt ngang–mặt nghiêng luân phiên ở vài nếp lớn; nếp gấp phải thay bố cục lối và nhịp nhà. Người và vật cũng có mặt cắt/gấp có chủ ý, nhưng chân có điểm đặt và tay đang làm việc. Dáng dài, thấp rộng hoặc lưng cong giúp phân biệt người trưởng thành/già mà không cần giải phẫu tả thực. Cảnh kéo dài ngoài mép ảnh; không đế mô hình, mặt bàn, khung sách pop-up hoặc một hàng cutout giống hệt.

UI có thể dùng một mép gấp mở ra làm vùng chữ; nội dung không bị bẻ ở mọi nếp. Portrait, icon và mục hành động chung cạnh gấp nhưng vẫn phân biệt sách/đồ mang/người nói. **Ví dụ đúng:** chiều sâu đến từ các mặt gấp và người đứng trên từng mặt. **Tránh:** nhà bình thường có texture giấy nhàu.

**Cần xem:** người xem hiểu một nơi đang được trình bày theo quy ước, hay tưởng nhiều nơi/actor bị ghép? **Rủi ro:** địa hình có vẻ bất khả thi hoặc lối nối giả. Nếp gấp không thay World Route Graph, gravity, khoảng cách hay thêm thao tác gấp thế giới.

## 4. Mưa khắc hình — `rain-sculpt`

**Giả thuyết:** các dải mưa và khoảng âm có thể chia không gian thành những nêm lớn, để người đang lao động/trao đổi nổi qua cử chỉ và cạnh khối, không dựa vào ánh sáng điện ảnh tả thực.

Chọn góc tầm người hoặc cao vừa: tiền cảnh là khối cắt lớn, vùng hành động có khoảng sáng/khô gọn, hậu cảnh chìm thành một vài mảng. Mưa là nhịp nét/dải có hướng và mật độ kiểm soát, không bức màn hạt đồng đều. Tạo hình người bằng khối góc cạnh, bàn tay/vật mang có silhouette; không để tất cả thành bóng đen không tuổi. Các nêm ánh sáng không tự là spotlight hoặc nguồn chiếu hiện đại trong thế giới.

UI có cạnh xiên và mảng đặc rõ, chữ không chạy theo dòng mưa. Portrait cũng được giản lược thành mặt phẳng; một useful-object icon đọc được trong cả vùng sáng và tối. **Ví dụ đúng:** bỏ màu vẫn thấy ba lớp sâu và việc đang làm. **Tránh:** cảnh cũ chuyển xanh đen rồi thêm rain overlay/glow vàng.

**Cần xem:** có thay cấu trúc hình và hierarchy, hay chỉ đổi palette/thời tiết? **Rủi ro:** không thấy đường/chân/người nói, mỹ thuật chỉ dùng được một thời điểm. Một frame mưa không chứng minh ngôn ngữ toàn mùa, simulation thời tiết, hành vi trú mưa hoặc shader runtime.

## 5. Mặt cắt đời sống — `living-section`

**Giả thuyết:** nhìn ngang một khoảng sinh hoạt mở làm quan hệ người–công việc–cao độ dễ đọc, trong khi giữ tỷ lệ đủ gần cho biểu cảm.

Một hiên/sân công cộng nhìn cạnh gần trực giao, vài sàn và khoảng dưới sàn có người làm việc được phép thấy. Không vẽ xuyên vách kín, đồ giấu, quan hệ bí mật hoặc NPC chưa biết chỉ để có nhiều tầng. Hình khối thân thể nặng và chân tay rõ; tuổi, thể trạng và dáng làm việc khác nhau, không cùng khuôn người tí hon. Một đường đi chính và một cuộc gặp cần nổi hơn những người nền.

UI có thể nằm trong dải âm ở đáy, portrait ngang và icon đặc cùng nhịp khối. Không phân đoạn màn hình thành nhiều comicpanel hay bảng household controls. **Ví dụ đúng:** nhìn ngang biết ai ở cùng hiên và ai đang mang vật. **Tránh:** một sơ đồ cắt nhà omniscient với nhãn bí mật trong mọi phòng.

**Cần xem:** đây là không gian đang nhập vai một người hay game điều hành cả nhà? **Rủi ro:** mất sức rộng của thế giới, các tầng nhìn như nhiệm vụ platformer. Mặt cắt là đề xuất presentation; chưa có đi tầng/nhảy, camera theo người hoặc knowledge projection được kiểm runtime.

## 6. Hạt khối — `voxel-life`

**Giả thuyết:** các khối lập phương thô có thể xây một thế giới có thể tích và cá tính, nơi tuổi/nghề đi qua nhịp silhouette hơn độ mịn.

Thử góc chéo rõ ba mặt; hạt đủ lớn để nhìn ra bậc trên mép mái, vai, bàn tay và mũi thuyền. Không chỉ pixelate ảnh, không làm mềm mọi cạnh thành viên đất nặn. Cỡ hạt có quy tắc xuyên cảnh/người/đồ; công cụ được cô đọng nhưng không biến thành loot cube cùng một dáng. Người già có lưng, tóc và dáng mang khác người trẻ trưởng thành, không buộc mọi người thành một avatar vuông chuẩn.

Portrait dùng các khối mặt, icon có chiều dày/bậc cùng họ; vùng chữ UI giữ gọn và đọc thuận, không bắt chữ Việt mất dấu để giống pixel. **Ví dụ đúng:** silhouette người có bậc thật trong hình gợi khối, mặt bên có sắc độ riêng. **Tránh:** sprite2D phóng to trên một nền tả thực.

**Cần xem:** không gian/người/UI cùng có logic hạt, tuổi và hành động vẫn rõ? **Rủi ro:** thành đồ chơi xây dựng phổ thông, mất bản sắc địa phương hoặc hiểu nhầm có phá/đặt block. Hình khối không cấp quyền xây, đào, destructible terrain; chưa có voxel engine hoặc model thật.

## Nguồn văn hóa và ranh giới sáng tạo

Căn cứ đang có là [workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [ngữ pháp văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md), không có nghiên cứu hiện vật mới trong brief này. Hồ sơ ghi nhịp người–nhà–thuyền, sinh hoạt và useful objects từ những nguồn cụ thể; nó không chứng nhận sáu phương pháp ở đây là nghệ thuật thời Văn Lang–Âu Lạc.

| Nền tham chiếu đã có | Phép biến đổi của ta | Chỗ chưa biết cần giữ rõ |
|---|---|---|
| Nhịp nhóm người, công việc, nhà/sàn và thuyền trong hồ sơ workshop | Chuyển thành dải, nét, mặt gấp, nêm sáng, dải ngang hoặc khối hạt | Một cách diễn đạt mới không chứng nhận cấu trúc nhà, phục trang hoặc palette lịch sử |
| Cử chỉ cầm, mang, chèo, trao vật và điểm tựa cơ thể | Giữ nghĩa qua silhouette rất giản lược ở mọi hướng | Một pose không chứng minh nghề/tuổi/địa vị của toàn roster hoặc quy trình sản xuất khảo cổ |
| Useful objects và bố cục khoảng sinh hoạt trong GDD | Chọn ít vật rõ chức năng, để người và việc dẫn mắt | Kiểu dệt, loại sợi, thuốc nhuộm, dụng cụ, dạng mái cụ thể vẫn cần nguồn khi đào sâu |

Thế giới dệt không tuyên bố người đương thời vẽ toàn thế giới như tấm dệt; nét dây/nếp gấp/mưa/mặt cắt/hạt khối là phương pháp sáng tạo hiện tại. Bản sắc cần đi qua cách sống, hình và hành động có căn cứ, không chỉ tre, nâu hoặc hoa văn mặt trống.

## Cổng kiểm sau khi root tạo ảnh

Mở đúng PNG ở cự ly chơi, ghi **ý định → điều đã nhìn thấy → giới hạn**. Kiểm người đang nhập vai, chân đứng, speaker/đối tượng, vật hữu dụng và vùng chữ trước khi khen chất liệu. Nếu woven vẫn là texture, line vẫn phẳng, fold chỉ là cutout, rain chỉ đổi màu, section thành omniscient house hoặc voxel chỉ là filter, phải ghi đúng mức chưa đạt ấy.

Không dùng đủ sáu ảnh để suy sáu đột phá hoặc độ phủ toàn game. Một frame chưa kiểm mọi vùng/ngày đêm/roster/trạng thái UI/chuyển động; chưa có gameplay, runtime/Unity, camera, shader, model3D hoặc human playtest. Root lưu prompt/provenance và reviewer ghi quan sát thật riêng sau generation. Brief này không tự hoàn tất các bước đó.

Random-origin, no-runtime-LLM, deterministic/causal rules, knowledge isolation và ownership giữ nguyên. Mọi output vòng16 vẫn **Pending user approval**, không merge dev, đóngSprint13, mởSprint14, tag hoặc release.
