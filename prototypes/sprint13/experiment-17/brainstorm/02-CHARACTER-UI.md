# Brainstorm17 — người, cử chỉ và giao diện cùng một ngữ pháp

02/11/2026 · SPIN-034 · Actor `/root/s13e17_systems` · **Đề xuất trước tạo ảnh, Pending user approval**.

Đây là bốn đề xuất gửi root, không là bốn hướng đã chốt hoặc bốn output đã tạo. Phù điêu trùng với đề xuất của worker không gian và đã được thống nhất gộp, không đếm thành hai ý tưởng. Chưa sinh ảnh, model hoặc gameplay trong phần việc này.

## Điều học từ các vòng trước

Đã đọc baseline, HANDOFF, skill mythical-prototype và hướng dẫn art direction; đối chiếu hồ sơ vòng11/12 và review16. Vòng16 nét dây đầu tiên quay về tranh có viền, gấp đầu tiên quay về làng bậc tầng: tên chất liệu không ép được quy tắc hình. Review16 cũng giữ vấn đề hai diamond chỉ khác màu, portrait không rõ là người chơi hay người nói và mini-map chưa là projection thật. Đây là nguồn hồ sơ, chưa nhận đã trực tiếp mở mọi ảnh cũ trong lượt brainstorm này.

Mỗi hướng cần một người nhập vai có silhouette do tuổi/thể trạng/công việc, một người đối thoại và một việc cục bộ; tránh nam anh hùng mặc định hoặc mọi người cùng chibi. Một dấu chân có hình riêng nhận diện player; speech pointer có đuôi chỉ rõ NPC. Portrait phải có vai trò xác định bằng quan hệ với người trong cảnh, không thêm chân dung chỉ để đủ checklist. Các action chip phải dùng icon và chữ, không giao nghĩa riêng cho màu.

## 1. Phù điêu đang sống — `carved-relief`, gộp với nhánh relief của worker không gian

**Quy tắc:** camera chính diện, không gian sâu rất nông. Thân người, dụng cụ và mép nhà nhô lên từ một mặt chung; cạnh dày, khe lõm và che khuất có thật trong hình, không chỉ texture đồng hay tranh vẽ lên tường. Mặt người nghiêng, cánh tay đưa giỏ và thân cúi làm việc tạo những tầng lồi khác nhau. Một người già thấp rộng và một người trưởng thành dáng dài cho hai nhịp thân rõ.

**UI:** một khoang âm ở mép dưới, cùng cạnh lồi với cảnh; portrait là đầu nhô khỏi khoang, icon là hình nổi lớn. Chữ đặt trên mặt yên và không khắc chìm đến mất dấu. Các lựa chọn có khối/mép riêng, không parchment fantasy dán lên tác phẩm.

**Delta:** vòng12 có bích họa khoáng màu; đây thử bề dày điều khiển hình người/cảnh/UI. Prompt phải bắt thấy cạnh và khe, không chỉ nói “relief”. **Cần xem:** ảnh đọc như nơi đang nhập vai hay vật bảo tàng được chụp? Không dùng đế, khung trưng bày, lớp patina thay quy tắc. Phương án này đã trùng và được gộp với `Một nét cắt` của direction; root chỉ nên tính một hướng.

## 2. Người bằng cử chỉ — `gesture-flat`

**Quy tắc:** 2D phẳng, không viền, không texture. Hình cơ thể bất đối xứng mạnh do hành động: vai một bên nhô, lưng tạo một cung lớn, hai bàn tay thành mảng nặng quanh vật đang mang; không dùng một khuôn avatar đổi màu áo. Người già làm nghề có lưng và điểm tựa khác người khỏe gánh giỏ. Không tách mặt thành đầu to dễ thương trong khi thân giống nhau.

Không gian cũng theo nhịp cử chỉ: lối đi cong lớn tiếp nối trục lưng, mái là một mảng nghiêng tiếp nối tay, vật đặt đất thành khoảng âm để đọc vị trí. Vẫn giữ nơi đặt chân và lối tiếp cận, tránh biến động tác thành sơ đồ biểu tượng.

**UI:** các mặt bất đối xứng nhưng không méo chữ; portrait lấy cùng silhouette tóc/mũi/đầu với người được xác định. Action chip có đầu lớn–đuôi nhỏ cùng nhịp tay/vai, icon mảng đặc lớn. Không dùng cùng một thanh HUD đối xứng cho mọi hướng.

**Delta:** vòng12/Gió mảng màu đã thử hoạt hình phẳng. Đề xuất này chỉ có ích nếu silhouette thay mạnh theo tuổi, sức nặng và việc làm, không chỉ palette đẹp hoặc vẽ phẳng lại. **Cần xem:** khi thu nhỏ còn phân biệt được ai mang vật, ai nói và hướng trao không? Không khẳng định đã có animation squash/stretch; đây là staging của ảnh tĩnh.

## 3. Cửa trong cửa — `nested-portals`

**Quy tắc:** diện mạo3D ở tầm người, ba khung mái/cột lồng nhau có kích thước giảm rõ. Mép tối gần che một phần khung thứ hai; khung thứ ba ôm cuộc gặp sáng và gần. Độ sâu đến từ occlusion, khoảng trống và tỷ lệ, không HDR, sương hay god rays. Một người lớn đang trả dụng cụ nhìn thấy toàn thân trong aperture giữa; người già ở hiên đối diện có cử chỉ dễ đọc.

**UI:** dùng các vùng âm bên rìa khung làm chỗ neo portrait và hai action. Phần UI phải còn nhìn như giao diện màn hình, không tự biến thành chữ có thật trên cột nhà. Icon và portrait dùng cùng mảng đặc/bo góc của aperture; để trống lối và tay đang trao.

**Delta:** khác quy mô hành lang thành đất bằng khung gần, tầm người và che khuất ba lớp; không lấy công trình khổng lồ làm nội dung chính. **Cần xem:** ba aperture thật sự dẫn mắt đến cùng cuộc gặp hay chỉ là cửa nhà thông thường? Tránh ảnh phong cảnh phong cách điện ảnh; không nhận một frame là camera dolly hoặc chuyển cảnh đã chạy.

## 4. Khối ghép đời thường — `tangram-volume`

**Quy tắc:** cảnh và người được ghép từ rất ít nêm/lăng trụ lớn, nhìn thấy mặt trên/mặt bên ở camera chéo gọn. Một thân người có thể chỉ gồm đầu nêm, thân hình thang, cánh tay thành hai khối to; mũi/tóc/bàn tay vẫn có chỗ để đọc tuổi và thao tác. Không thêm áo vải drapery hoặc hàng chục tam giác nhỏ để quay về low-poly thông thường. Tỷ lệ khối phải khác giữa người lưng cong và người mang giỏ.

**UI:** portrait dùng cùng vài khối nêm, icon là lăng trụ lớn có silhouette khác nhau; action chip là mặt cắt ngang của họ khối ấy nhưng chữ đứng thuận. Chọn player bằng bracket ở chân, NPC bằng pointer chứ không một cặp diamond đổi màu.

**Delta:** khác voxel16 vì không có lưới lập phương; khác folded16 vì không cần một dải sàn gấp liên tục. Nó chỉ khác thật nếu cả cơ thể lẫn dụng cụ là vài khối ghép khổ lớn, không chỉ background faceted. **Cần xem:** có mất biểu cảm/nghề vì thân quá trừu tượng không? Không suy tạo hình ghép thành cơ chế puzzle, xây/đặt block hoặc model3D thật.

## Phản biện chéo đã xảy ra

- `/root/s13e17_direction` đề xuất Hành lang thành đất, Bờ liền nét, Vành sân và Một nét cắt. Tôi phản hồi relief trùng để gộp; đề nghị relief chính diện thấy cạnh dày/khe, người overhead ở Bờ liền nét phải cùng phép nhìn với cảnh; Vành sân cần trục nhà/cột/lối theo cung, không chỉ cong đường xa.
- Direction đồng ý gộp relief và xác nhận đổi Vành sân sang trục nhà/cột/lối cong nhìn thấy, giữ hai thân người gần thẳng theo quy ước camera. Direction phản biện nested-portals cần ép ba aperture giảm kích thước/occlusion rõ, không god rays; gesture-flat phải khác graphic-animation12 bằng silhouette tuổi/việc; tangram phải đổi cả thân người chứ không riêng nền.
- `/root` xác nhận relief được gộp, yêu cầu gesture-flat nhấn bất đối xứng do mang/vá/dệt; nested-portals tách bằng framing tầm người; tangram cần rất ít nêm lớn và ghi rủi ro gần folded16. Các điểm đó đã đưa vào rule/delta ở trên.
- `/root/s13e17_audit` đồng ý gộp relief; lưu ý gesture-flat phải khác12 bằng tỷ lệ/cử chỉ/tuổi và UI cùng nhịp body, nested-portals phải có ba khung kiến trúc che khuất thật trong hình thay viền trang trí hoặc portal đi xa, tangram phải dựng người/đồ bằng ít nêm thay low-poly shader. Audit cũng nêu Âm bản sống/Khối liền mềm/Pixel cự ly gần đang được root cân nhắc: mềm cần liền khối không joint/toy16 và camera đường thẳng để tách Vành sân; pixel gần là nhánh11 tăng độ đọc, không voxel16. Đây là phản biện nhận được, chưa có ảnh chứng minh.
- Sau đó audit và direction xác nhận root đổi batch thử ảnh: Khối liền mềm chưa đưa vào mười frame, thay bằng Sát mặt nước để mở thêm camera. Ý tưởng mềm/hybrid vẫn được giữ trong brainstorm, không bị gán user bác bỏ. Batch giao gallery gồm `earth-corridor`, `river-negative`, `gentle-arc`, `living-relief`, `gesture-flat`, `nested-frames`, `tangram-people`, `low-water`, `close-pixel`, `negative-space`; đây là chốt phạm vi tạo ảnh, không chốt mỹ thuật cuối. Direction chuyển rõ quy tắc UI mà root yêu cầu: portrait thuộc selected player và khớp tuổi/tóc, player dùng corner brackets, NPC dùng speech bubble có đuôi, không mini-map.

## Ranh giới

Nguồn văn hóa hiện hành là workshop và baseline của project; không có khảo cổ hoặc nguồn hiện vật mới trong brainstorm. Các quy tắc không gian/thân thể/UI là sáng tạo presentation, chưa phải phục dựng Văn Lang–Âu Lạc. Không sửa canon, route, tri thức, vật lý, engine hoặc character origin. Tất cả01–16 được giữ; ba ưu tiên14/1–3 vẫn là phản hồi thật của user, không được thay bằng sở thích của agent về vòng16. Mọi phương án17 tiếp tục **Pending user approval**.
