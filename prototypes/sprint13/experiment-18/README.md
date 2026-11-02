# Thử nghiệm18 — Mười lăm phương án mở rộng thư viện

02/11/2026 · Sprint13 / SPIN-035 · **Brief trước generation, Pending user approval**.

User giao thêm khoảng15ý tưởng sau vòng17. Root chọn15gói sau brainstorming/trao đổi chéo của ba worker: sáu trục camera–không gian, sáu trục cấu tạo người/UI và ba gói audit mở range. Mỗi gói vẫn cần một ngôn ngữ xuyên cảnh, thân người, mặt, vật và UI; không cùng shader đổi màu/góc, cũng không tuyên bố mọi kỹ thuật chưa từng có.

## Bộ15 được giao

| # | Hướng / key | PNG trưng bày | Câu hỏi cần xem trên output |
|---|---|---|---|
|01|Sân chéo mở / `open-yard`|`assets/open-yard.png`|Khoảng âm/ít mái có làm người và công việc nổi hơn panorama?|
|02|Ba dải đường / `three-bands`|`assets/three-bands.png`|Ba lớp outdoor có occlusion/lối nối thay cutaway toàn tri?|
|03|Qua vai người thợ / `shoulder-craft`|`assets/shoulder-craft.png`|Tay/vật che mép có đưa camera vào nghề mà không che người nói?|
|04|Đường chéo dài / `long-diagonal`|`assets/long-diagonal.png`|Lối cắt frame có đổi bố cục/body/UI, khác chỉ xoay sân01?|
|05|Rừng là thân cột / `column-forest`|`assets/column-forest.png`|Khối cây có dựng không gian quanh công việc mà giữ người làm trọng tâm?|
|06|Sân phẳng, người nổi / `plan-profile`|`assets/plan-profile.png`|Hybrid có quy ước thống nhất body/prop/shadow và điểm đặt chân?|
|07|Nét mặt ba sắc / `cel-adult`|`assets/cel-adult.png`|Mặt ba vùng có giữ tuổi/cảm xúc và cùng grammar với body/cảnh?|
|08|Khối liền mềm / `soft-mass`|`assets/soft-mass.png`|Khối bo liền có tỷ lệ adult, tránh chibi/khớp rời/texture đất?|
|09|Mực đặc / `ink-mass`|`assets/ink-mass.png`|Khoảng rỗng có cấu tạo gesture, thay một tranh chỉ tô tối?|
|10|Người bằng dải / `ribbon-body`|`assets/ribbon-body.png`|Dải có trực tiếp tạo thân/tay/chân và occlusion?|
|11|Đường đồng mức sống / `contour-step`|`assets/contour-step.png`|Lớp cao độ có đi xuyên silhouette người/vật/UI thay contour trang trí?|
|12|Pixel hình lớn / `square-gesture`|`assets/square-gesture.png`|Cụm vuông lớn có đọc tuổi/cử chỉ mà giảm mật độ17?|
|13|Một-bit có tuổi / `one-bit`|`assets/one-bit.png`|Hai sắc/khoảng trắng có giữ mặt/tay/tuổi, không chỉ grayscale?|
|14|Sân khấu một nền / `single-stage`|`assets/single-stage.png`|Một floor nông/người lớn có khác02ba lớp và vẫn là nơi nhập vai?|
|15|Khối răng cưa / `retro-facet`|`assets/retro-facet.png`|Facet/bước cạnh có tạo silhouette thật, không texture retro phủ cảnh mịn?|

Đã có15frame chính và hai bản đầu `ribbon-body-v1.png`, `contour-step-v1.png` giữ qua link riêng. Tổng17PNG RGB,30,511,159bytes:15ảnh1672×941, riêng `plan-profile.png` và `contour-step-v1.png`1672×940. Tất cả tạo02/11 bằng built-in image_gen, không ảnh input, copy nguyên bytes/không hậu kỳ. [Prompts](prompts.json) giữ15prompt đầu và2iteration; [provenance](provenance.json) ghi nguồn/SHA/mode/dims/bytes/mapping. Hai iteration là tạo mới từ prompt khác, không ảnh tham chiếu. V1 còn body/scene thường nên root thử lại cấu trúc; vẫn giữ cả hai, không user bác bỏ. Gallery ghi điều quan sát được và phần chưa đạt, không nhận15tên là15grammar đã thành công. [ART-DIRECTION](ART-DIRECTION.md) giữ đầy đủ camera/body/face/props/UI, lineage/delta và rủi ro.

## Brainstorm và thư viện được giữ

[Không gian](brainstorm/01-SPACE.md), [nhân vật–UI](brainstorm/02-CAST-UI.md) và [audit lịch sử](brainstorm/03-RANGE-AUDIT.md) ghi đóng góp/trao đổi thật. Root gộp cel/soft/ink trùng trước chọn15; Khối liền mềm/hybrid là đề xuất từng giữ trong brainstorm17, không bị user bác bỏ. Pixel/dệt/than/retro và một số cấu trúc có tiền lệ, delta phải xem bằng ảnh chứ không suy từ nhãn vật liệu.

Giữ toàn bộ01–17/biến thể để xem lại/kết hợp. Ba hướng14/1–3 tiếp tục ưu tiên nghiên cứu từ phản hồi thật; root chọn phạm vi batch18 không phải chốt style thay user.

## Xem gallery

Mở [gallery](index.html) tại <http://127.0.0.1:8016/experiment-18/>. Có thể phục vụ từ root repository:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Trang chỉ chọn/xem/phóng ảnh. UI trong PNG là raster minh họa, chưa có text component hoặc handler game. Tăng chữ HTML không làm chữ raster reflow; mở PNG gốc khi cần đọc chi tiết. Các liên kết01–17 giữ thư viện cũ, không đánh dấu bị thay hoặc kém.

## Ranh giới và quyết định

Chỉ ảnh mô phỏng game, có thể mang diện mạo2D/2.5D/3D; không model/scene/render từ geometry đã dựng, Hyper3D/Blender, gameplay/runtime hoặc canon mới. Một-bit/pixel/retro là intent thị giác, không tự là bit depth/lưới asset/engine hoặc polygon count đã kiểm. Cast là ví dụ cuộc đời nhận ngẫu nhiên, không character creator hoặc protagonist cố định.

Portrait phải thuộc selected player và khớp body; bracket mặt đất khác bubble NPC, không chỉ khác màu. Mặt cần mắt/mày/mũi/dấu tuổi, vật mang cần khớp label; đó là chuẩn so sánh brief chưa được nhận Passed trước khi xem output. Hai hành động ngắn, không mini-map phụ. Những phép camera/bands/hybrid/steps không cho nhìn hidden truth hoặc tự đổi route/physics/authoritative state.

[Workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [hồ sơ văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md) giúp phân biệt nguồn, diễn giải và hư cấu; không chứng nhận mọi hình là phục dựng Văn Lang–Âu Lạc. Một frame chưa kiểm toàn roster/campaign/UIstate/animation hoặc human playtest.

[Approval register](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền chốt. Technical review, quan sát output và user approval riêng nhau; toàn bộ output18/Sprint13 vẫn **Pending user approval**, không tự merge dev, đóngSprint13 hoặc chuyểnSprint14.
