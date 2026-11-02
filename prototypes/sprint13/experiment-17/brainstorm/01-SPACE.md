# Brainstorm17 — Không gian, tỷ lệ và cách nhìn

02/11/2026 · Sprint13 / SPIN-034 · Worker `/root/s13e17_direction` · **Đề xuất trước tạo ảnh, Pending user approval**.

User cho phép học hỏi và improve từ các ý tưởng trước, yêu cầu thêm mười phương án. Bốn gói bên dưới là đề xuất đầu vào cho root chọn bộ mười, chưa phải bốn output đã có hoặc bốn hướng mới hoàn toàn. Phạm vi worker này chỉ là hồ sơ brainstorming; không tạo ảnh/model, sửa gallery hoặc chốt mỹ thuật thay chủ dự án.

Ba mẫu14/1–3 được user ưu tiên nghiên cứu; đó là căn cứ đào sâu, không đồng nghĩa duyệt style cuối. Các bản01–16 và biến thể tiếp tục giữ. Các nhận xét về giới hạn16 được lấy từ [review16](../../../../docs/production/sprints/sprint-13/CRITIC-UX-16.md); không gán ý kiến reviewer thành phản hồi user.

## 1. Hành lang thành đất — `earth-passage`

**Nguồn ý tưởng:** Đất dựng thành14/1, tầng sinh hoạt15/1 và mặt lớn tối giản của Làng gấp16v2. Giữ sự chênh lệch quy mô người–công trình nhưng kéo cuộc gặp về đủ gần để nhập vai một con người.

- Camera3/4 thấp vừa, nhìn dọc một lối công cộng có vách đất lớn và ramp rộng. Người gần cao khoảng một phần ba chiều cao cảnh; không thu thành phố rồi dán một portrait lên.
- Chỉ bốn đến sáu khối công trình lớn: vách, ramp, hiên, cột, sàn. Mảng sáng/tối có biên rõ; không phủ chi tiết rạ/gỗ/dây/viên đá lên mọi mặt.
- Hai người trưởng thành có silhouette dài–thấp rộng khác nhau; tay mang bình/giỏ tạo một cuộc gặp có nghề. Thân, mặt, công cụ đều dùng những khối cắt lớn, cùng ngôn ngữ với đất đắp.
- UI dọc gọn như một mảng khối cắt có nền chữ yên, portrait góc cạnh và hai icon đặc. Dấu player ở chân và đuôi thoại của NPC khác hình; không chỉ khác màu.

**Delta phải thấy:** từ cảnh quan/quy mô khổng lồ sang cuộc gặp ở trong cấu trúc lớn, với ít khối hơn và người đọc được hơn. Nếu ảnh chỉ là làng nhiều tầng nhiều texture thì chưa đạt. Khác `nested-portals` của worker systems: hướng này dựa vào quy mô đặc của vách đất và khoảng đi; portals dựa vào các khung che khuất gần, không lấy quy mô thành làm trọng tâm.

**Câu hỏi/giới hạn:** có thấy nơi đặt chân và việc đang làm trước khi thấy kiến trúc đẹp? Ramp/vách là phóng đại mỹ thuật, không chứng nhận kiến trúc Cổ Loa, route graph hoặc khả năng leo tầng runtime.

## 2. Bờ liền nét — `continuous-bank`

**Nguồn ý tưởng:** Ô đất sống14/2 và mặt bằng dải đan16. Dệt không còn là vật liệu; dùng hình dương–âm của bờ và nước để tổ chức cảnh2D.

- Góc90° nhìn thẳng từ trên, hoàn toàn không horizon, tường bên, mái đứng hoặc mặt người nhìn chính diện trong không gian. Những mảng đất lớn liên tục ôm mặt nước như các hình khóa vào nhau.
- Bỏ đường lưới và mật độ ô nhỏ. Một nhánh bờ nối một bến và chỗ gặp, có đường tiếp cận rõ. Khoảng trống nước lớn là cấu trúc, không chèn thêm nhà để lấp khung.
- Người cũng nhìn từ trên: đỉnh đầu, vai, lưng và bàn tay mang vật. Lưng rộng, tóc bạc/búi tóc, thế chống sào giúp phân biệt tuổi/vai; không dán sprite người đứng chính diện lên map.
- Một portrait nhỏ ở UI có thể nhìn mặt, nhưng cùng mặt phẳng màu và hình cắt; nó là portrait riêng, không giả cùng phép chiếu với body. Hai hành động nằm ở vùng âm yên, không thành dashboard điều hành cả làng.

**Delta phải thấy:** thống nhất phép chiếu người–cảnh và làm bờ liền mảng thay lưới/mosaic/dải dệt. Chỉ bỏ texture khỏi16 mà giữ body xiên chưa đạt câu hỏi. Không gọi đây là luật ô mới hoặc một bản đồ có thẩm quyền.

**Câu hỏi/giới hạn:** tư thế và vật mang có đủ làm người trở thành người, thay vì các icon quản lý? Nhìn từ trên không tự cho phép biết người, đồ hay bí mật sau tường; scene minh họa dùng sân/bến công cộng mở.

## 3. Vành sân — `courtyard-arc`

**Nguồn ý tưởng:** Làng vòng cung14/3 và độ cong thực sự đã xuất hiện trong15/3. Giữ cảm giác gần người và nhịp cuộn, thử dàn hình trưởng thành hơn và ít chi tiết hơn; không chê hoặc loại bản15.

- Mái, trục cột, mép sân và hướng đường phải cùng tham gia một cung rộng có thể nhìn thấy. Không chỉ vẽ đường làng cong hoặc dùng chữ “rolling” để nhận đã có phép nhìn khác.
- Hai thân người gần được giữ gần thẳng, chân có điểm đặt rõ; họ là điểm neo đọc hành động giữa trường cảnh cong. Đây là quy ước presentation hai lớp đề xuất, chưa chứng minh camera động hoặc geometry nhất quán.
- Tỷ lệ người trưởng thành: vai, bụng, lưng, bàn tay và nét già khác nhau; đầu không quá lớn, không mặc định khối mềm là trẻ con. Một người mang giỏ đối diện người trao lá đủ để diễn đạt nghề/sinh hoạt.
- Khối mềm vẫn có mặt lớn và cạnh rõ, không thêm nan giỏ/lá/rạ vụn. UI chạy theo một phần cung ở đáy với portrait mềm nhưng già, chỉ hai hành động. Không bẻ chữ theo mọi điểm của cung.

**Delta phải thấy:** đọc tuổi/cử chỉ tốt hơn, ít mật độ vật liệu hơn, phép cong phối hợp ở nhiều yếu tố. Giữ sự cong đủ mạnh để khác14/3; không dồn toàn độ cong ra xa đến mức không còn nhận ra. Tránh làm cả bản đồ thành một hành tinh tí hon.

**Câu hỏi/giới hạn:** cuộc gặp có còn tự nhiên khi nhà/cột cuộn mà thân gần đứng? Không suy curvature thành thế giới hình cầu, gravity, mặt trống vật lý hoặc camera runtime đã thực hiện.

## 4. Phù điêu đang sống — `living-relief` (gộp với đề xuất systems)

**Nguồn ý tưởng:** độ sâu nông/bích họa12 và tổ chức ngang16, nhưng đổi toàn thể tích người–nhà–vật thành những slab nhô/lõm có bề dày, thay vì vẽ một tranh lên tường.

- Camera gần chính diện với một góc xiên rất nhẹ, vừa đủ thấy cạnh dày và khe lõm. Cảnh mở công cộng như hiên trao đồ, không toàn cảnh nhà nhìn3/4 bình thường.
- Toàn thân người, tay, giỏ và cấu trúc hiên là các mặt đặc dày. Cạnh nổi, khe đen và bóng tiếp xúc nông phải góp phần tạo silhouette; không chỉ phủ texture đồng/đá/gỗ.
- Dáng người có vai–lưng–tay khác tuổi/thể trạng trong cùng một hệ slab. Tay đưa đồ hơi vươn ra khỏi mặt phẳng để đọc quan hệ trước/sau; không biến tất cả thành cùng icon nghi lễ.
- Portrait nổi thấp và các chip hành động có cạnh dày chung quy tắc. Chữ ở mặt yên, hai pictogram vật dụng/thoại tách nghĩa, không dùng chuỗi ký hiệu giả chữ cổ.

**Delta phải thấy:** độ dày và che khuất trực tiếp quyết định người/cảnh/UI. Nếu chỉ là bích họa với chất kim loại thì vẫn là biến thể bề mặt, chưa đủ grammar mới. Không tự nhận một chất liệu, motif hoặc technique thuộc Văn Lang–Âu Lạc.

**Câu hỏi/giới hạn:** độ sâu nông có giữ được đường đi và cuộc gặp, hay thành vật trang trí bảo tàng? Đây là ảnh mô phỏng diện mạo2.5D, không mesh/scene/render thật hoặc luật di chuyển theo mặt phẳng.

## Trao đổi chéo thực tế

- `/root/s13e17_systems` nhận thấy relief hai bên trùng, đề nghị gộp một hướng thay vì đếm thành hai ý tưởng. Worker này đồng ý; root cũng yêu cầu gộp tên “Phù điêu đang sống”. Systems nhấn mạnh cạnh dày/khe lõm và camera gần chính diện để tránh ra tranh thường có texture nổi.
- Systems phản biện Vành sân: nếu chỉ cong xa, có thể quay về giới hạn14/3. Worker này tiếp thu bằng quy tắc mái/cột/đường cùng tham gia cung, thân người gần làm điểm neo đọc; phần quy ước hai lớp vẫn phải được kiểm trên ảnh thật.
- Systems phản biện Bờ liền nét: đầu/vai phải nhìn từ trên, không người đứng front pasted vào. Worker này đã đưa vào brief. UI chỉ một đến hai nhãn ở góc âm, không dashboard.
- Worker này góp ý lại systems: `nested-portals` cần ba aperture giảm kích thước và occlusion rõ, tránh god-rays hoặc chỉ hành lang nhiều cột; `gesture-flat` cần phóng đại body theo tuổi/việc đủ khác graphic-animation12; `tangram-volume` cần người cũng có lăng trụ, không chỉ background. Đây là trao đổi thiết kế, chưa phải kết quả ảnh hoặc quyết định user.
- `/root/s13e17_audit` đồng ý ba nhánh14 và gộp relief, nhắc giữ độ dày nông xuyên body/world/UI thay shader đồng trên làng thường. Audit chỉ ra nguy cơ trùng với gói “Âm bản sống” của batch: Bờ liền nét cần giữ mặt bằng90° phẳng/footprint, trong khi Âm bản thử khoang âm trong một đến hai lớp sâu. Khối liền mềm không dùng camera cong nên khác Vành sân ở phép chiếu; pixel cự ly gần phải khác voxel16. Worker ghi nhận phân ranh này để root không đếm một grammar hai lần.

## Bộ mười root chọn để tạo ảnh sau trao đổi

Root thông báo chọn `earth-corridor` / Hành lang thành đất; `river-negative` / Bờ liền nét; `gentle-arc` / Vành sân; `living-relief` / Phù điêu đang sống; `gesture-flat` / Người bằng cử chỉ; `nested-frames` / Cửa trong cửa; `tangram-people` / Khối ghép đời thường; `low-water` / Sát mặt nước; `close-pixel` / Pixel cự ly gần; `negative-space` / Âm bản sống. Các key ở bốn phần brainstorm đầu là nhãn làm việc; bộ key trong danh sách này là giao diện batch được root dùng tiếp.

Audit cập nhật sau đó: root thay gói Khối liền mềm bằng Sát mặt nước để mở thêm range camera. Sát mặt nước nhìn thấp ngang mạn, người/thuyền lớn ở tiền cảnh và bến/hiên phân lớp; UI theo mạn. Nó khác Thủy kính15 ở chỗ nhìn trực tiếp từ gần mặt nước, không tổ chức cảnh qua ảnh phản chiếu. Khối liền mềm/hybrid còn trong brainstorming, chưa được chọn vào bộ mười, không bị gán là user bác bỏ.

Đây là chọn phạm vi sản xuất ảnh của root trong task, không chốt art direction hoặc quyền bỏ bản cũ. Tên “Sát mặt nước” chưa chứng minh camera/runtime/model thật; mỗi output vẫn cần nhìn trực tiếp để ghi delta và lệch brief.

Không có ảnh/model/runtime mới trong hồ sơ này. Root đã chọn phạm vi mười gói của batch, không chốt mỹ thuật cuối; mọi phương án được chọn tạo ảnh vẫn **Pending user approval**.
