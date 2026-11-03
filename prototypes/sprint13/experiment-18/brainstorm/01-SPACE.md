# Vòng18 — Brainstorm không gian và camera

02/11/2026 · Sprint13 / SPIN-035 · Actor `/root/s13e18_space` · **Intent để thử ảnh, Pending user approval**.

## Phạm vi tham chiếu thực

Đã đọc `.agent`, HANDOFF, baseline áp dụng, các event cuối WORKLOG và skill mythical-prototype/game-art-direction. Đọc ART-DIRECTION17 và các findings camera, texture, marker/portrait trong CRITIC-UX17; không mở lại PNG cũ hoặc ảnh mới trong lượt brainstorm này. Các lineage dưới là tham chiếu hồ sơ và quan hệ ý tưởng, không so sánh pixel trực tiếp hoặc ảnh reference cho generation.

User giao thêm khoảng15ý tưởng; root đã mở SPIN-035. Sáu gói dưới đóng góp trục không gian cho bộ15, chưa là kết luận mỹ thuật đạt. Giữ01–17 và các biến thể. Ưu tiên14/1–3 là phản hồi user đã có, không phải yêu cầu mọi vòng phải lặp ba nhánh ấy.

## Sáu gói không gian được đề xuất

### 01 — Sân chéo mở / `open-yard`

Orthographic3/4 trên một sân nghề công cộng rộng; khoảng đất trống chiếm vai trò cấu trúc, mái/cột chỉ chắn vài rìa. Một người mang vật đủ lớn để đọc vai và tay, người đối diện ở cùng mặt sân, một lối tiếp cận rõ. Thế giới dùng các mảng ranh lớn; thân người và UI có mặt sáng/tối gọn, không lấp mặt sân bằng đá, giỏ hoặc vệt đất.

**Delta cần nhìn thấy:** khoảng âm và mật độ thay cấu trúc một cảnh sân, khác panorama/bến/lối thành. **Rủi ro:** nếu vẫn nhiều nhà khối và đồ nhỏ, đây sẽ chỉ là ảnh isometric quen. UI nên là hai khối gọn ở mép khoảng âm, portrait đúng người được chọn; không dashboard quản lý sân.

### 02 — Ba dải đường / `three-bands`

Nhìn ngang ba dải sinh hoạt ngoài trời ở cao độ và độ sâu khác nhau, nối bằng một đoạn dốc công cộng. Dải gần che một phần dải xa bằng bờ đất/lan can và vài silhouette; không nhìn xuyên tường. Người ở dải đang chơi lớn hơn, tay/vật/cử chỉ còn đọc được; các người nền không có nhãn thông tin riêng.

**Delta cần nhìn thấy:** ba lớp occlusion như một không gian liên tục, khác nhà cutaway16 hoặc trang truyện chia ô14. Body dùng hình bẹt có cạnh dày cùng dải đường; UI là một đoạn thanh ngang có đầu mút rõ, không ba bảng điều khiển cho ba tầng. **Rủi ro:** giống platformer hoặc quản lý mọi NPC nếu bố cục thành ba lane đồng quyền.

### 03 — Qua vai người thợ / `shoulder-craft`

Camera gần thấp phía sau vai và bàn tay/vật đang mang của một adult; cánh tay và vật che một mép khung, người đối diện và động tác nhận/đặt làm điểm tới. Không dùng khung cửa lồng hoặc mạn thuyền làm foreground để phân biệt nested/low-water17. Một mảng bàn/lối công cộng đủ thấy mặt tựa và khoảng đứng.

**Delta cần nhìn thấy:** nghề nghiệp được đọc qua vật lớn trong cự ly nhập vai, không chỉ chân dung phóng bên một phong cảnh. Body/face nên có vài mặt cel sắc rõ, bàn tay không biến thành bàn tay tả thực trên nền vẽ; UI ở khoảng âm đối diện tay/vật. **Rủi ro:** cánh tay che mất người nói hoặc trở lại POV nghề14 mà không đổi cấu trúc.

### 04 — Đường chéo dài / `long-diagonal`

Một lối/cầu công cộng xiên mạnh xuyên màn hình từ gần sang xa, cắt hai mép thay vì nằm giữa một quảng trường. Camera axon cho thấy mặt lối và ba lớp độ sâu bằng vài khối chắn, foreground adult đủ lớn làm neo. Không cần nhiều mái nhà để tạo chiều sâu.

**Delta cần nhìn thấy:** hướng lối, độ dài và silhouette dẫn mắt, khác mặt sân rộng của01. Có thể tổ chức thân/đồ/UI theo những cạnh bậc hoặc hướng nêm; không chỉ cùng shader rồi xoay camera. **Rủi ro:** nếu chỉ đổi góc của cùng sân đất/nhà khối thì01 và04 sẽ gần như một ý tưởng.

### 05 — Rừng là thân cột / `column-forest`

Khối rễ/thân cây lớn ở gần cắt rìa khung; khoảng mở phía trước có việc hái, lựa hoặc buộc bó vật liệu của hai adult. Không phụ thuộc bến sông/nhà sàn làm dấu nhận diện. Cây là cấu trúc khối có silhouette đặc, khoảng giữa cây dẫn tới việc/người, không rừng photoreal đầy lá nhỏ.

**Delta cần nhìn thấy:** framing từ khối sinh thái và cự chỉ lao động, thay chuỗi làng/bến nhiều đồ. Hình người có mảng đặc và mặt/tay rõ, icon/portrait/UI giữ nhịp đặc–rỗng của thân cây; UI ở khe yên nhưng không bị nhầm là chữ khắc trong thế giới. **Rủi ro:** chỉ tô cảnh tối hoặc lặp bóng rối10; không thêm sinh vật ma thuật/tri thức bí mật để làm lạ.

### 06 — Sân phẳng, người nổi / `plan-profile`

Một hybrid2D có quy ước chủ động: footprint sân/đường là mặt bằng phẳng, trong khi người và một số useful props là profile có độ dày nông dựng trên cùng baseline. Mọi body/prop liên quan dùng cùng phép nghiêng và hướng bóng; không một sprite chính diện đột ngột đứng trên map90°. Portrait riêng giữ cùng nhân vật và logic hình.

**Delta cần nhìn thấy:** mixed projection trở thành một quy tắc xuyên cảnh/người/UI, khác phép chiếu lệch vô tình ở woven16 hoặc river17. UI dùng thẻ profile/mặt yên, không thêm một vật liệu thứ ba. **Rủi ro:** đọc như quân cờ trên map quản lý hoặc không biết chân đứng đâu. Không tuyên bố gói này đã giải quyết yêu cầu pure90° của một hướng khác.

## Trao đổi chéo thực và điều chỉnh

- `/root/s13e18_cast` phản hồi: hybrid phải thống nhất cả profile của prop và shadow, portrait cùng người đang nhập vai; dải đường cần foreground công cộng che khuất để tránh toàn tri/platformer; sân chéo cần khoảng âm lớn và ít mái. Các điểm này đã đưa vào sáu gói.
- Tôi gửi lại cast: có thể phối cel-adult với qua vai để đọc mặt/gesture gần; ribbon-body với hybrid để cùng quy ước body/prop; ink-mass với rừng nhưng phải tránh chỉ recolor bóng rối; contour-step với đường chéo để bậc đi xuyên body/UI. Đây là đề xuất phối hợp, không quyết định root hoặc output đã đạt.
- `/root/s13e18_audit` ủng hộ rừng để mở bối cảnh; lưu ý qua vai gần13near/17nested/low-water nên phải dùng tay/vật làm khối chắn, không cửa/boatframe. Audit nêu01/04 dễ giống nhau nếu cùng axon/đất/nhà khối, và Ba dải đường cần khác Single-stage của audit ở ba lớp occlusion so với một floor.
- Root thông báo batch15 cuối giữ sáu gói spatial theo key/order trên, sáu gói cast và ba gói audit; gộp cel/soft/ink trùng giữa cast–audit, không đếm hai lần. Tôi giữ cảnh báo overlap01/04 và02/single-stage để brief/output review tiếp tục kiểm, không tự nhận đã khắc phục trước generation.

## Điều kiện để gọi đây là một hướng hình ảnh

Camera chỉ là một trục; mỗi gói phải phối cùng silhouette/body/face/icon/UI có quy tắc cụ thể. Không cần cùng palette nâu–ngà hoặc cùng vị trí HUD; cần người đang nhập vai khác người nói bằng hình dấu, portrait khớp cùng người và hành động gắn vật thật đang thấy. Các gói là ảnh mô phỏng, không model/scene3D/runtime hoặc mechanic mới.

Các cao độ, lối/cầu, rừng và sân trong brief là staging mỹ thuật cần đối chiếu nguồn khi đào sâu; chưa xác minh phục dựng Văn Lang–Âu Lạc. Không sửa canon/route/physics/knowledge isolation. Technical review, quan sát output và quyết định user là ba lớp riêng; toàn bộ vòng18/Sprint13 vẫn **Pending user approval**.
