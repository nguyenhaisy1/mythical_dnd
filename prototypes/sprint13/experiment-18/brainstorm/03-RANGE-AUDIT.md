# Audit phạm vi hình ảnh — vòng18

02/11/2026 · Sprint13 / SPIN-035 · Actor `/root/s13e18_audit` · **Brainstorm trước generation; Pending user approval**.

## Phạm vi đã đọc

Đã đọc bối cảnh `.agent`, HANDOFF và baseline áp dụng, events gần nhất trong WORKLOG, skill mythical-prototype và hướng dẫn art direction. Audit liệt kê key/title từ prompt source các vòng07–17, đọc phần findings của [review16](../../../../docs/production/sprints/sprint-13/CRITIC-UX-16.md) và [review17](../../../../docs/production/sprints/sprint-13/CRITIC-UX-17.md). Đây là audit chọn lọc trên source/brief/review, **chưa mở lại toàn bộ artwork01–17**, chưa có output18 để đánh giá.

User muốn thêm khoảng15ý tưởng và cho phép học từ bản cũ. Ba ưu tiên14/1–3 vẫn là phản hồi thật, nhưng không cần ép mọi batch lặp lại cả ba nhánh. Những nhận xét của AI về16/17 không tự là sở thích mới của user. Mọi phiên bản cũ, biến thể và đề xuất chưa vào batch vẫn được giữ.

## Tiền lệ và bài học cần mang theo

- Pixel, anime, lớp2.5D và diện mạo3D đã có ở11; pixel cũng có ở07. Không gọi quay lại một kỹ thuật là phát minh mới.
- Dệt08/16, giấy07/16, clay/flat/fresco/retro12, puppet10 và geometry17 đều đã được đề xuất. Vòng18 cần chỉ rõ thay đổi camera, silhouette, che khuất hoặc vai trò UI, không chỉ đổi vật liệu/bảng màu.
- Review17 ghi texture và bối cảnh thường lấn quy tắc hình; Vành sân v1 trở về tả thực, v2 giảm khối nhưng NPC vô diện. Đặt grammar lên đầu prompt và giữ một tình huống cục bộ đơn giản để giảm cạnh tranh.
- Tóc xám không đủ chứng minh tuổi. Mặt, mày, mũi, posture và thể trọng cần tạo tuổi khác nhau; bỏ toàn bộ nét mặt để giản lược có thể làm mất phần này.
- Mười lăm mẫu cùng bến–hai người–nhà nâu vẫn có thể giống nhau dù tên style khác. Cần thử cả rừng, sân trong, nghề trong hiên hoặc không gian một sàn công cộng; thay bối cảnh chỉ có ích khi camera/hình cũng có chủ ý.
- Giữ portrait khớp selected player và bracket khác NPC bubble. Mini-map, cutaway hay khoảng rỗng không được dùng để suy quyền biết thông tin bí mật.

## Sáu đề xuất audit trước khi gộp

| Đề xuất | Grammar / delta cần kiểm bằng output | Nguy cơ |
|---|---|---|
| Cel ba sắc | Diện mạo3D nhưng từng khối chỉ vài bậc sáng, outline chọn lọc, mặt người trưởng thành rõ; học anime11 và cự ly13 | Trượt thành người anime trẻ hoặc shader điện ảnh phủ lên làng thường |
| Nét đen–mảng trắng |2Dhai tông và một điểm nhấn, độ sâu nhờ hình đặc/khoảng trống/che khuất, cạnh cứng thay grain than | Chỉ biến tranh có sẵn thành trắng–đen, không đổi silhouette |
| Một-bit có tuổi | Silhouette đặc và khoảng rỗng lớn, stipple theo cụm tiết chế ở vài vùng, nét mặt ít nhưng có tuổi; UI cùng nhịp | Mọi grayscale bị dither thành một lớp texture dày, không còn đọc tay/người |
| Khối liền mềm | Người và công trình từ vài thể tích bo liền, không khớp/rod/chibi; nối clay12 nhưng bỏ dấu tay/ảnh vật liệu | Cả cast thành trẻ con hoặc tượng đồ chơi cùng một thân |
| Sân khấu một nền | Camera nhìn ngang, một sàn công cộng nông, người lớn và đầy đủ cử chỉ, UI trong khoảng âm mép sàn | Trùng cutaway16 hoặc Ba dải đường nếu không khác tổ chức che khuất |
| Khối răng cưa | Diện mạo3D coarse facets/bước cạnh và dither kiểm soát, camera/người gần, UI gọn; học retro12 | Dither phủ cảnh tả thực; chỉ gọi low-poly mà silhouette vẫn phức tạp |

Đây là đề xuất grammar và câu hỏi, chưa phải sáu phương pháp được output chứng minh hoặc sáu kỹ thuật chưa từng tồn tại.

## Trao đổi chéo thật

`/root/s13e18_space` gửi sáu nhánh Sân chéo mở, Ba dải đường, Qua vai người thợ, Đường chéo dài, Rừng là thân cột và hybrid Sân mặt bằng–người mặt đứng. Audit phản hồi:

- Giữ Rừng là thân cột để mở bối cảnh và phép framing, tránh toàn bộ lại thành bến sông.
- Hybrid phải ghi quy ước footprint trên/body profile có chủ ý; không tự coi đó là đã sửa phép chiếu lệch ở17river.
- Qua vai cần tay/vật lớn thật và một mép bị che, tránh chỉ lặp13near/17nested-lowwater.
- Ba dải đường cần ba lớp occlusion; Sân khấu một nền cần một floor nông và thân người lớn. Hai đề xuất chỉ đáng dùng hai slot nếu output giữ khác biệt ấy.
- Sân chéo mở và Đường chéo dài dễ gần nhau nếu cùng axon/làng đất. Đường chéo phải tổ chức lối cắt frame và hướng nhìn khác, không chỉ đặt tên khác.

`/root/s13e18_cast` gửi cel-adult, soft-mass, ink-mass, ribbon-body, contour-step và square-gesture. Audit nhận ba cặp trùng cel/soft/ink với đề xuất của mình và đề nghị gộp, không đếm thành sáu ý tưởng riêng. Audit góp ý ribbon phải tạo silhouette/che khuất của thân và tay, không chỉ áo quấn texture; mặt vẫn cần vùng đặc đọc tuổi. Contour-step cần các lớp cao độ che nhau xuyên cơ thể/cảnh/UI, tránh đường trang trí hoặc scan/x-ray.

Cast đồng ý gộp ba cặp và làm rõ pixel khác one-bit: square-gesture dùng cụm vuông lớn màu đặc, không hatching; one-bit dùng silhouette/khoảng rỗng với stipple chỉ ở một vài vùng chuyển. Cast nhắc retro-facet cần thấy cạnh coarse thật trong hình, không dither lên cảnh photoreal. Đây là trao đổi có tác nhân, không là playtest hoặc quyết định user.

## Phạm vi15 hướng do root chọn để tạo ảnh

Root chốt6space +6cast +3audit không trùng: `open-yard`, `three-bands`, `shoulder-craft`, `long-diagonal`, `column-forest`, `plan-profile`, `cel-adult`, `soft-mass`, `ink-mass`, `ribbon-body`, `contour-step`, `square-gesture`, `one-bit`, `single-stage`, `retro-facet`.

Cel/soft/ink được gộp vào grammar cast; audit không tạo thêm ảnh riêng cho ba tên đồng nghĩa. Mười lăm slot là mười lăm tổ hợp để thử, chưa chứng minh mức khác biệt hoặc độ hợp toàn game. Các giới hạn trùng side-on/axon, tuổi/face và texture creep cần được xem lại trên PNG thực sau generation.

Không nghiên cứu hiện vật mới ở audit này. Rừng, nhà, nghề, phục trang và các cách tạo hình là diễn giải/hư cấu mỹ thuật cần nguồn khi đào sâu, chưa là phục dựng Văn Lang–Âu Lạc được xác nhận. Chỉ ảnh mô phỏng, không model/Hyper3D/Blender/runtime/engine/canon mới. Mọi output18/Sprint13 tiếp tục **Pending user approval**.
