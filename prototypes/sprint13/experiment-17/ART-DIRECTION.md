# Thử nghiệm17 — Mười cách nhìn tiếp nối nghiên cứu trước

02/11/2026 · Sprint13 / SPIN-034 · Worker định hướng `/root/s13e17_direction` · **Brief tạo ảnh, Pending user approval**.

User giao thêm mười ý tưởng, cho phép học hỏi/improve từ bản cũ và brainstorming giữa tác nhân. Bộ này gồm ba nhánh đào sâu14/1–3 đã được user ưu tiên và bảy gói khác hoặc cải tiến. Không tuyên bố bảy gói là vật liệu chưa từng xuất hiện hoặc mười hướng đều sẽ đạt đột phá; mức khác biệt phải nhìn trên output.

Đây là brief, không kết quả generation. Root dự kiến tạo mười frame mới bằng built-in image_gen, không ảnh input, giữ prompt thực/provenance và quan sát từng đầu ra. Mỗi frame là một tình huống ở camera/tỷ lệ chơi đề xuất, không bảng asset cố định, model/scene3D thật hoặc game đang chạy.

## Điều học từ thư viện và trao đổi tác nhân

[Không gian](brainstorm/01-SPACE.md) ghi bốn gói spatial cùng trao đổi thật; [người và UI](brainstorm/02-CHARACTER-UI.md) ghi cử chỉ, khung lồng và khối ghép; [audit lịch sử](brainstorm/03-HISTORY-CRITIQUE.md) kiểm tiền lệ và phản biện mức khác biệt. Root tổng hợp sau phản biện chéo: relief trùng được gộp một hướng; Khối liền mềm/hybrid chưa vào batch được giữ trong notes, không gán user bác bỏ. Ba ưu tiên14/1–3 vẫn là phản hồi user, khác với nhận xét/sở thích của agent về16.

Các vòng trước cho thấy tên phương pháp không đủ ép output: nét dây16v1 quay về tranh có viền, gấp16v1 quay về tầng đất; woven16 chưa xuyên thân người; nhiều marker/portrait chưa rõ vai; có ảnh giống3D nhưng không có geometry thật. Lượt17 cần ghi quy tắc nhìn được, giảm chi tiết không giúp đọc hành động và giữ cả phần lệch brief nếu xảy ra.

## Quy ước chung để so sánh, không phải UI production đã chốt

- **Một cuộc đời được nhận:** cast là ví dụ xuất thân ngẫu nhiên, không protagonist cố định, class/origin selector hoặc mọi người cùng khuôn anh hùng. Tuổi, lưng, vai, tay, thể trạng và tư thế làm việc giúp phân biệt người.
- **Người chơi khác người nói:** portrait thuộc người đang được chọn nhập vai, phải khớp tóc/tuổi/cơ thể thấy trong cảnh. Dấu player là góc vuông/bracket ở chân; NPC nói dùng bubble bo có đuôi chỉ đúng người. Hình dấu có thể theo phong cách, nhưng không giao khác biệt vai chỉ cho màu.
- **Chữ và icon có việc:** một đến hai hành động ngắn dễ đọc; portrait/icon/action cùng họ hình với người và cảnh. Không thêm mini-map, bảng secret stats hoặc chức năng để lấp chỗ trống. Giao diện là raster minh họa, chưa phải state/handler runtime.
- **Giữ đường và việc:** có điểm đặt chân, lối tiếp cận và cử chỉ/vật đang trao. Người chơi cần nhận người/việc trước chi tiết mái/rạ/sợi. Không yêu cầu cùng cảnh/cast ở cả mười gói vì đây là lượt mở rộng tổ hợp.
- **Màu theo vai trò:** nền và người đủ tách sáng/tối, nhấn tương tác có thêm hình; mặt chữ yên. Mười gói không buộc cùng bảng màu nâu–ngà hoặc cùng HUD. Không dùng số màu hay tỷ lệ như metric đã đo.
- **Nguồn đúng:** các lineage dưới là quan hệ ý tưởng, không ảnh reference, matching asset hoặc cùng geometry/cast đã được chứng minh. Giữ toàn bộ01–16 và biến thể, không tự thay/loại.

## Bộ mười theo thứ tự trưng bày

| # / key | Hướng | Nhánh nghiên cứu và delta chủ đạo |
|---|---|---|
|01 `earth-corridor`|Hành lang thành đất|Từ14/1,15/1 và mặt lớn16: đưa người đủ gần vào quy mô kiến trúc lớn, ít khối và ít texture hơn|
|02 `river-negative`|Bờ liền nét|Từ14/2 và woven16: mặt bằng90° thống nhất người–cảnh, đất/nước liền mảng, bỏ grid/texture|
|03 `gentle-arc`|Vành sân|Từ14/3 và15/3: giữ độ cong nhìn thấy, thêm tỷ lệ trưởng thành và giảm chi tiết, không thành hành tinh chibi|
|04 `living-relief`|Phù điêu đang sống|Gộp hai đề xuất worker; từ fresco12: bề dày slab/che khuất điều khiển cả người–cảnh–UI|
|05 `gesture-flat`|Người bằng cử chỉ|Đào sâu graphic-animation12: thân bất đối xứng theo tuổi/việc và trọng lượng, không chỉ tô phẳng|
|06 `nested-frames`|Cửa trong cửa|Khung gần tầm người tạo ba aperture/occlusion, khác quy mô đặc của thành đất và phong cảnh điện ảnh|
|07 `tangram-people`|Khối ghép đời thường|Từ nghiên cứu mặt góc16: ít nêm lớn xuyên cơ thể/vật/UI, không lưới voxel hoặc low-poly vụn|
|08 `low-water`|Sát mặt nước|Camera trực tiếp thấp ngang mạn, người/thuyền lớn trước bến/hiên; khác nhìn qua phản chiếu15|
|09 `close-pixel`|Pixel cự ly gần|Nhánh từ11: kéo cự ly lên đủ thấy body/cử chỉ, grammar raster rõ, không đổi thành voxel16|
|10 `negative-space`|Âm bản sống|Khối đặc và khoang âm một đến hai lớp sâu; đổi silhouette/occlusion, khác plan view2D của02 và trắng hóa16|

## 01 — Hành lang thành đất

Camera3/4 thấp vừa nhìn dọc lối công cộng có vách lớn, ramp rộng và hiên. Người gần đủ cao để đọc tuổi/tay mang đồ; tòa thành không nuốt mất cuộc gặp. Bốn đến sáu khối kiến trúc chủ đạo, biên sáng/tối rõ; tránh lấp từng mặt bằng viên đá, lá, dây và rạ. Hai body dài–thấp rộng khác nhau nhưng cùng khối cắt lớn với công trình. UI dọc gọn, portrait/action/icon như mặt cắt có nền chữ yên.

**Cần thấy:** con người trong cấu trúc lớn và lối nối dễ đọc, khác một panorama công trình. **Lệch thường gặp:** cảnh làng bậc tầng đầy texture rồi portrait phóng to ở góc. Quy mô/vách/ramp là phóng đại mỹ thuật, không phục dựng Cổ Loa hoặc walk graph thật.

## 02 — Bờ liền nét

Góc90° nhìn thẳng từ trên, không horizon/tường bên/mái đứng. Đất và nước thành các mảng khóa nhau liên tục, một nhánh bờ nối một bến/công việc; bỏ grid và ô nhỏ. Body cũng từ trên: đỉnh đầu, vai, lưng, tay mang vật; không dán người chính diện vào map. Portrait riêng được phép nhìn mặt nhưng dùng cùng hình phẳng và palette. UI đặt ở vùng âm yên, hai icon/nhãn lớn thay dashboard.

**Cần thấy:** phép chiếu thống nhất và footprint liền mảng, khác woven16 chỉ bỏ texture. **Lệch thường gặp:** boat top-down nhưng người front/xiên hoặc đất ghép thành ô quản lý. Chỉ trình bày sân/bến mở; góc cao không cấp quyền biết hidden truth, luật hex hoặc bản đồ toàn tri.

## 03 — Vành sân

Mép sân, trục nhà/cột và đường cùng tham gia cung rộng nhìn được; không chỉ một con đường uốn xa. Hai thân người gần tương đối thẳng, chân có chỗ đặt, làm điểm neo đọc hành động giữa cảnh cuộn. Tỷ lệ trưởng thành với vai/lưng/bụng/tay khác nhau; đầu không phóng quá lớn thành trẻ con. Khối mềm mặt lớn, ít chi tiết vật liệu. UI theo một phần cung đáy, portrait mềm nhưng đúng tuổi, chữ đứng thuận và chỉ hai hành động.

**Cần thấy:** độ cong phối hợp nhưng con người đọc gần, ít nan/lá/rạ hơn15/3. **Lệch thường gặp:** trở lại14/3 chưa đủ cong hoặc tạo hành tinh tí hon/chibi. Đây là đề xuất quy ước camera hai lớp, chưa chứng minh geometry/camera động hoặc gravity.

## 04 — Phù điêu đang sống

Gần chính diện, chỉ xiên nhẹ đủ thấy cạnh dày và khe lõm. Thân người, bàn tay đang trao đồ, giỏ và hiên là các slab nổi nông; độ dày/che khuất tạo silhouette trực tiếp. Khung hình là nơi sinh hoạt mở kéo dài ngoài biên, không có đế, khung bảo tàng hoặc tác phẩm trên bàn. Portrait nổi thấp và chip action có cạnh dày cùng họ, chữ đặt mặt yên; không giả chữ cổ.

**Cần thấy:** người–cảnh–UI cùng có bề dày nông. **Lệch thường gặp:** tranh vẽ lên vách, shader đồng trên làng3/4 thường, hoặc các icon nghi lễ giống nhau. Relief là sáng tạo cách trình bày, không xác nhận kỹ thuật/chất liệu/motif thời Văn Lang–Âu Lạc và chưa là mesh/scene thật.

## 05 — Người bằng cử chỉ

2D phẳng không viền/texture, thân bất đối xứng rõ theo việc: vai nhô, lưng thành cung, hai tay tạo mảng nặng quanh vật đang mang. Body người già có điểm tựa khác người khỏe; không một avatar đổi màu áo. Cảnh đi theo nhịp cử chỉ bằng vài mảng mái/lối nghiêng lớn nhưng vẫn có chân đặt và khoảng đứng. Portrait giữ silhouette tóc/mũi/đầu; action chip đầu lớn–đuôi nhỏ, icon đặc cùng nhịp body mà chữ không méo.

**Cần thấy:** cử chỉ/tuổi/trọng lượng đọc được khi thu nhỏ, khác graphic-animation12 chỉ ở palette. **Lệch thường gặp:** hình phẳng đẹp nhưng người cân xứng cùng khuôn hoặc thân biến thành ký hiệu. Đây là staging tĩnh, chưa có squash/stretch, animation hoặc action feedback.

## 06 — Cửa trong cửa

Tầm người, ba khung cột/mái lồng có kích thước giảm rõ và che khuất thật trong ảnh. Mép tối gần che một phần khung thứ hai; aperture thứ ba giữ cuộc gặp nhưng người vẫn đủ lớn. Độ sâu từ khoảng trống/tỷ lệ/occlusion, không HDR, sương hay god rays. Người có khối gọn, bàn tay/vật trao rõ; icon/portrait cùng mặt đặc và nhịp bo của aperture. UI neo ở vùng âm rìa, không biến thành chữ có thật khắc trên cột.

**Cần thấy:** ba lớp khung dẫn vào cùng một cuộc gặp, khác01 dựa vào công trình khổng lồ. **Lệch thường gặp:** viền trang trí lồng nhau, portal đi xa hoặc ảnh nhà có ánh điện ảnh thông thường. Không suy một frame thành camera dolly, teleport hoặc nhiều không gian authoritative mới.

## 07 — Khối ghép đời thường

Người/cảnh/vật từ rất ít nêm, hình thang và lăng trụ lớn, thấy mặt bên/trên ở góc chéo gọn. Mỗi cơ thể có cấu trúc riêng do lưng, vai, thân mang vật; mũi/tóc/bàn tay đủ để đọc tuổi và thao tác. Không phủ hàng trăm tam giác hoặc áo drapery để quay về low-poly thường. Portrait và icon là cùng vài khối nêm; action chip như mặt cắt khối nhưng chữ đứng thuận.

**Cần thấy:** ít khối lớn chi phối cả thân người và công cụ. **Lệch thường gặp:** chỉ background faceted, người vẽ bình thường hoặc coarse voxel. Không cần một dải sàn gấp như16, cũng không ngầm thêm puzzle lắp ghép, xây/đặt block hoặc asset grid.

## 08 — Sát mặt nước

Điểm nhìn trực tiếp thấp ngang mạn thuyền, không nhìn qua ảnh phản chiếu. Thân người/thuyền lớn ở tiền cảnh, bến và hiên tạo lớp giữa/xa bằng cột và mép sàn che khuất; mặt nước là một mảng nền có nhịp, không lấy quang học photoreal làm nội dung chính. Tư thế tay giữ mạn/trao giỏ và trọng lượng người làm neo nhập vai. UI theo mép mạn hoặc khoảng yên, portrait cùng khối/cạnh rõ, không camera phụ hoặc mini-map.

**Cần thấy:** một quan hệ người–thuyền–bến mới ở cự ly gần, khác Thủy kính15 và góc cao quen. **Lệch thường gặp:** toàn cảnh bến từ trên hoặc phim chèo thuyền có water shader đẹp. Không cho viewpoint nhìn dưới nước/đi xa hoặc thêm luật phương tiện; chưa có camera/runtime/model thật.

## 09 — Pixel cự ly gần

Raster2D có cụm pixel rõ, camera/cự ly gần hơn11 để thân người, tay và vật đang trao còn tách được. Dùng silhouette tuổi/thể trạng khác nhau, ít chi tiết trên áo; không biểu cảm chỉ nhờ portrait lớn. Cảnh có ít mảng nhà/bến và vùng đi rõ, tránh hàng trăm prop nhỏ. Portrait/icon/UI cùng lưới hiển thị có chủ ý, vùng chữ đủ sạch để giữ dấu tiếng Việt; không bắt chữ mất dấu vì nhãn “pixel”.

**Cần thấy:** pixel cluster và độ đọc ở cự ly chơi, khác pixel-filter phủ lên tranh hoặc3D voxel16. **Lệch thường gặp:** thân tí hon, vô số chấm nhiễu, UI mịn khác họ hoặc pixel giả không đều. Đây là ảnh preview raster, chưa là spritesheet, tilemap, pixel-perfect runtime hay asset sản xuất.

## 10 — Âm bản sống

Không gian từ khối đặc lớn và khoang rỗng một đến hai lớp sâu; lỗ/khe và mảng âm quyết định người, nhà, giỏ, UI. Góc chéo nhẹ hoặc gần chính diện đủ thấy độ dày khoang; khác02 là mặt bằng phẳng. Người không thành xương rỗng hoặc silhouette không tuổi: mặt/bàn tay/điểm tựa có mảng đặc, cử chỉ và vật đang cầm còn nhận ra. Portrait/icon/action chung quy tắc đặc–rỗng, chữ ở một mặt không bị khoét vụn.

**Cần thấy:** void trực tiếp thay silhouette/occlusion xuyên cả scene và cast. **Lệch thường gặp:** làng thường tô đen/trắng, hiệu ứng ánh sáng kiểu shadow16 hoặc x-ray lộ mọi vật sau vách. Khoang âm là cách tạo hình, không quyền thấy hidden truth, ma thuật, vật lý hoặc navigation mới.

## Nguồn văn hóa, việc quan sát và giới hạn

[Workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [ngữ pháp văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md) là nền đang có, không nghiên cứu hiện vật mới trong task này. Đời sống công cộng, người làm nghề, nhà/sàn/thuyền/công cụ là đầu vào hình ảnh; hình dáng cụ thể vẫn là diễn giải/hư cấu cần nguồn khi được chọn đào sâu. Không khẳng định mỗi garment, motif, nón, mái hoặc kỹ thuật làm nghề là phục dựng lịch sử.

Review cần mở từng output để ghi camera/khối/body/UI thực có, ngữ nghĩa player/speaker, vùng đặt chân, chi tiết mất khi thu nhỏ và phần lệch brief. Metadata/control check, quan sát hình và quyết định user là ba lớp khác nhau. Không nhận PNG là gameplay/Unity acceptance, chuyển động, knowledge isolation, benchmark hoặc human playtest. Mười frame chưa chứng minh full roster, mọi vùng và mọi trạng thái túi đồ/nhật ký.

Giữ thư viện cũ và brainstorm chưa vào batch. Mọi output17 cùng S13-A01–A04/Sprint13 vẫn **Pending user approval**; không tự merge dev, đóngSprint13 hoặc chuyểnSprint14.
