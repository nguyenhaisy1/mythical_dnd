# Brainstorm17 — lịch sử thử nghiệm và khoảng nên mở thêm

02/11/2026 · Sprint13 / SPIN-034 · Actor `/root/s13e17_audit` · **Đề xuất và phản biện trước generation, Pending user approval**.

## Phạm vi audit thật

Đã đọc `.agent`, HANDOFF, baseline, skill mythical-prototype và reference game-art-direction; đọc README các vòng06–16, brief16, báo cáo phản biện hình ảnh15/16 và các event WORKLOG gần nhất. Đây là audit hồ sơ và lời quan sát đã được reviewer ghi; actor này **chưa mở PNG riêng trong giai đoạn brainstorm**, không nhận việc đọc review là trực tiếp xem artwork hoặc playtest. Các vòng01–05 được giữ theo chỉ dẫn và HANDOFF; không tuyên bố đã audit pixel của toàn bộ01–16.

Yêu cầu hiện hành là thêm mười ý tưởng bằng ảnh, được học từ bản cũ và brainstorm giữa tác nhân. Không phải chọn một style cuối, xác nhận mọi bản trước chưa đạt hoặc mở Sprint14. Phản hồi thật của user ưu tiên14/1 Đất dựng thành,14/2 Ô đất sống,14/3 Làng vòng cung; nhận xét của AI về16 không tự trở thành sở thích của user.

## Điều đã thử, để không đổi nhãn rồi gọi là mới

| Vòng / hồ sơ | Những hướng đã có | Hệ quả cho vòng17 |
|---|---|---|
|06 | Tranh2D góc chéo, ảnh có diện mạo3D gần người, sân khấu2.5D ngang | Không gọi mọi đổi góc cao/thấp là lần đầu thử2.5D/3D |
|07 | Khắc gỗ, pixel, giấy cắt nhiều lớp | Không lấy khắc nét, pixel hoặc giấy làm bằng chứng vật liệu mới hoàn toàn |
|08 | Hoạt hình vẽ tay, thế giới dệt,3D cách điệu | Dệt và nét hoạt hình đã có tiền lệ; cần đổi grammar, camera hoặc silhouette cụ thể |
|09–10 | Boards liên kết thế giới–cast–UI; men chàm và bóng rối | Gốm, xuyên sáng, silhouette sân khấu không tự mới; cũng không bắt vòng17 quay lại cùng công thức board |
|11 |2.5D,3D cách điệu, pixel, anime đời thường | Pixel cự ly gần là nhánh cải tiến khoảng cách/mật độ/tuổi/động tác, không phát minh lại pixel |
|12 | Đất nặn, nét rõ, bích họa, prerender retro, than đêm, hoạt hình mảng | Relief phải đổi chiều dày/che khuất, không chỉ phủ bích họa hoặc shader đồng lên làng cũ; mảng phẳng phải đổi người/cử chỉ/UI |
|13–14 | Ba cách nhìn đời sống; thành đất, mặt bằng, vòng cung, first-person, ký hình và trang truyện | Có cơ sở đào sâu ba hướng user thích. Đưa thêm góc ngang mạn thuyền giúp cả batch bớt cùng camera nhìn chéo cao |
|15 | Đứng, ghép đảo, cong rõ hơn, đa diện, bóng, phản chiếu | Góc cong, đa diện hoặc âm bản cần kiểm trên hình, không dựa tên. Tránh lặp nhãn rồi giữ phối cảnh thường |
|16 | Dệt mặt bằng, thanh cong có thể tích, gấp, mưa, mặt cắt, voxel | Phân biệt pixel17 với voxel16; âm bản17 với làng trắng/bóng15; nêm ghép17 với chỉ tô shader low-poly/gấp16 |

Các ghi chú trên tóm phạm vi đã được hồ sơ mô tả; không chứng nhận mỗi kỹ thuật đã đạt trong output, không xếp hạng loại bỏ các vòng cũ.

## Những bài học phải đi vào brief mười ảnh

1. **Đặt quy tắc hình trước vật liệu.** Lượt nét dây16 đầu trở về tranh có viền; lượt gấp16 đầu trở về địa hình bậc tầng. Prompt cần buộc silhouette, chiều sâu và bố cục đổi cụ thể, bớt một danh sách dài đồ vật khiến mô hình quay lại làng quen.
2. **Đừng làm mười phiên bản kem–vàng nhìn chéo.** Có thể giữ một vài nhánh thành đất/relief nhưng cần mặt bằng phẳng, người theo cử chỉ, camera thấp và pixel gần để khoảng lựa chọn rộng hơn. Bảng màu có vai trò, không tự là grammar mới.
3. **Giữ con người và việc đang làm.** Mỗi ảnh phải đọc được người đang nhập vai, người đang nói và vật/công việc giữa họ. Tuổi/nghề/thể trạng có thể đi qua vai, lưng, bàn tay và động tác; không cần mọi người giống nhân vật trẻ tròn hoặc một nam anh hùng.
4. **Tách ba vai UI bằng hình, không chỉ màu.** Review15/16 ghi marker/portrait/speaker còn mơ hồ. Brief17 nên dùng một kiểu dấu player, một kiểu speech pointer khác, xác định portrait thuộc người nào và dùng đúng icon cho hành động. Hình raster vẫn cần xem thật, không nhận prompt là bằng chứng UI đạt.
5. **Không kéo presentation thành luật game.** Vòng cung không là hành tinh; âm bản không nhìn xuyên tường; pixel không quyết định grid; địa hình chồng không tự thêm nhảy hoặc lối đi. Một frame không kiểm knowledge projection/runtime.
6. **Chất Việt cổ cần nền đời sống và nguồn khi đào sâu.** Cộng đồng, nhà/sàn, thuyền, trao vật và nghề là điểm khởi đầu; không thêm trống đồng/lông chim/nón/mái để tự chứng nhận niên đại. Audit này không nghiên cứu khảo cổ mới.

## Bốn hướng do actor audit đưa vào trao đổi

| Đề xuất | Delta hình ảnh cần nhìn thấy | Liên hệ và rủi ro |
|---|---|---|
| **Sát mặt nước** | Camera thấp gần ngang mạn thuyền, thân người và mũi/mạn thuyền lớn ở tiền cảnh; bến/hiên thành hai hoặc ba lớp mảng bẹt, UI đi theo cạnh mạn nhưng chữ đứng thuận | Học từ13 gần người và14 góc nhìn nghề, khác góc cao phổ biến. Không nhìn dưới nước, không reflection/portal15; cẩn thận tiền cảnh che hết người nói |
| **Pixel cự ly gần** | Cảnh nhỏ với sprite người lớn đủ lớn để phân biệt lưng già, vai rộng/hẹp, bàn tay mang vật; cụm pixel tiết chế, UI rõ dấu Việt, bỏ mini-map/đồ vụn thừa | Nhánh cải tiến pixel07/11 về mật độ và khoảng cách, không voxel16, không HD-2D glow hoặc filter pixel trên tranh |
| **Âm bản sống** | Một hoặc hai khối/mặt sâu nông liên tục được khoét khoảng trống làm hiên, lối và điểm gặp; người là hình đặc tách khỏi lòng khoét, UI nằm trong khoảng yên cùng hệ âm–dương | Khác15 trắng/bóng nếu hình học âm thật sự tổ chức cảnh, không chỉ đổi màu; không x-ray hoặc hiện người chưa biết. Có thể trở thành tranh logo nếu mất đường đặt chân/công việc |
| **Đồng bằng–người nổi** | Mặt bằng90độ phẳng đi cùng profile người có chiều dày nông nổi có chủ ý; portrait/icon dùng cùng quy ước ghép | Hướng dự phòng, dễ gần Bờ liền nét/relief và lặp vấn đề mixed projection16. Chưa đưa vào batch10; không gán user bác bỏ |

Actor cũng nêu **Khối liền mềm** như hướng dự phòng: vài khối bo liền qua người/công trình, tránh khớp/thanh của16 và tránh chibi; nếu có dùng phải giữ camera thẳng để không đếm trùng Vành sân. Đây chỉ là ý trong trao đổi, không sản phẩm được giao ở batch cuối.

## Trao đổi chéo thực sự đã diễn ra

- `/root/s13e17_direction` gửi bốn hướng: Hành lang thành đất, Bờ liền nét, Vành sân, Một nét cắt. Audit trả lời: giữ ba nhánh14 làm lineage có thật; relief phải có thickness nông xuyên body/world/UI; Bờ liền nét giữ90độ phẳng để khác âm bản có khối sâu. Không đếm relief hai lần với đề xuất của systems.
- `/root/s13e17_systems` gửi carved-relief, gesture-flat, nested-portals, tangram-volume và chủ động nhận carved-relief trùng direction. Audit phản biện: mảng phẳng có tiền lệ12 nên phải đổi cử chỉ/tuổi; nested-portals cần khung kiến trúc tạo che khuất thật trong hình, không viền trang trí hoặc cổng dịch chuyển; tangram phải cấu tạo người/đồ bằng ít nêm ghép, không chỉ shader low-poly.
- Root ban đầu nhận bảy hướng unique từ hai worker và hỏi ba hướng thêm để tăng khoảng lựa chọn. Audit gửi Sát mặt nước, Pixel cự ly gần, Đồng bằng–người nổi và Âm bản sống, khuyến nghị **Sát mặt nước + Pixel gần + Âm bản** cho range.
- Root từng cân nhắc **Âm bản + Khối liền mềm + Pixel gần**; audit truyền quyết định tạm đó cho hai sibling. Sau khuyến nghị camera thấp, root đổi **Khối liền mềm → Sát mặt nước**. Audit đã gửi cập nhật cho cả hai sibling; không ghi quyết định tạm thành batch cuối hoặc sửa thành phản hồi của user.

## Batch mười hướng root đã chọn để thử ảnh

1. Hành lang thành đất — nhánh14/1.
2. Bờ liền nét — nhánh14/2.
3. Vành sân — nhánh14/3.
4. Phù điêu đang sống (`living-relief`) — gộp đề xuất Một nét cắt / carved-relief của direction và systems.
5. Người bằng cử chỉ / gesture-flat.
6. Cửa trong cửa / nested-portals.
7. Khối ghép đời thường / tangram-volume.
8. Sát mặt nước.
9. Pixel cự ly gần.
10. Âm bản sống.

Tên/key cuối và prompt do root chốt cho generation, có thể tinh chỉnh tên mà giữ delta. Đây là chọn phạm vi **thử ảnh** của AI, không Final accepted art direction. Những hướng dự phòng vẫn giữ trong note; các phiên bản01–16 và mọi output17 tiếp tục Pending user approval. Không có ảnh/model/scene/runtime mới, khảo cổ verification hoặc gameplay acceptance trong stage audit này.
