# Thử nghiệm17 — Mười ý tưởng tiếp nối và mở rộng

02/11/2026 · Sprint13 / SPIN-034 · **Thư viện tham khảo, Pending user approval**.

User giao thêm mười ý tưởng và cho phép học hỏi/improve từ bản cũ, brainstorming giữa tác nhân. Bộ17 gồm ba nhánh từ14/1–3 được ưu tiên nghiên cứu và bảy gói khác/cải tiến; không tuyên bố mọi vật liệu lần đầu xuất hiện hoặc mọi ý định đều đã đạt. Giữ01–16, các biến thể và những đề xuất brainstorm chưa vào batch.

## Mười frame được giao

| # | Hướng / key | PNG trưng bày | Điều cần nhìn thấy |
|---|---|---|---|
|01|Hành lang thành đất / `earth-corridor`|`assets/earth-corridor.png`|Người đủ gần trong quy mô kiến trúc lớn, ít khối/texture, lối và cử chỉ rõ|
|02|Bờ liền nét / `river-negative`|`assets/river-negative.png`|Mặt bằng90° thống nhất body/cảnh, đất–nước liền mảng thay grid/dệt|
|03|Vành sân / `gentle-arc`|`assets/gentle-arc.png`|Cung đi qua nhà/cột/lối, giữ người trưởng thành gần và UI đọc được|
|04|Phù điêu đang sống / `living-relief`|`assets/living-relief.png`|Cạnh dày/khe nông tổ chức người/cảnh/UI, không chỉ texture nổi|
|05|Người bằng cử chỉ / `gesture-flat`|`assets/gesture-flat.png`|Thân bất đối xứng do tuổi/việc/trọng lượng, không cùng avatar đổi màu|
|06|Cửa trong cửa / `nested-frames`|`assets/nested-frames.png`|Ba aperture gần che khuất, cuộc gặp tầm người làm điểm tới|
|07|Khối ghép đời thường / `tangram-people`|`assets/tangram-people.png`|Rất ít nêm/lăng trụ lớn xuyên thân, vật và UI, không voxel/low-poly vụn|
|08|Sát mặt nước / `low-water`|`assets/low-water.png`|Nhìn trực tiếp ngang mạn, người/thuyền lớn trước lớp bến/hiên|
|09|Pixel cự ly gần / `close-pixel`|`assets/close-pixel.png`|Cụm pixel/tuổi/tay/vật trao đọc rõ ở cự ly gần hơn11, không3D voxel|
|10|Âm bản sống / `negative-space`|`assets/negative-space.png`|Khoang âm trong khối một–hai lớp sâu thay silhouette, không làng tô đen/trắng|

Đã có mười frame trưng bày và `assets/gentle-arc-v1.png` giữ qua link riêng. Mười một PNG1672×941RGB, tổng22,092,490bytes, tạo mới02/11 bằng built-in image_gen, không ảnh input. [Prompts](prompts.json) giữ mười prompt đầu và một iteration; [provenance](provenance.json) ghi nguồn/SHA/mode/bytes/kích thước/mapping. PNG sao chép nguyên bytes, không hậu kỳ. Bản Vành sân đầu gần tả thực/chưa cong nên root thử lại bằng khối giản lược; giữ v1 để đối chiếu, không gán user đã bác bỏ. Gallery ghi quan sát và phần lệch brief dưới từng card. Mỗi hướng một frame ở camera/tỷ lệ chơi đề xuất, không công thức bảng asset cố định. [ART-DIRECTION](ART-DIRECTION.md) ghi rules/delta/lineage/risks cụ thể.

## Brainstorm có tác nhân thật

[Không gian](brainstorm/01-SPACE.md), [nhân vật–UI](brainstorm/02-CHARACTER-UI.md) và [audit lịch sử](brainstorm/03-HISTORY-CRITIQUE.md) giữ đề xuất, tiền lệ và trao đổi chéo. Relief trùng được gộp, nhánh Khối liền mềm/hybrid chưa vào batch được giữ; không ghi đó là user bác bỏ. Root chọn phạm vi tạo mười ảnh sau brainstorming, không chốt mỹ thuật cuối thay chủ dự án.

Để sửa vấn đề marker/portrait ở vòng16, batch này dùng quy ước so sánh: portrait thuộc selected player và khớp tóc/tuổi/body; bracket góc vuông ở chân khác bubble NPC có đuôi. Mỗi hướng vẫn tự thiết kế hình/icon/UI theo grammar riêng, không cùng một HUD phủ lên mọi tranh. Không mini-map thêm để lấp khoảng trống. Đây là brief UI raster, chưa là contract/runtime production đã được user duyệt.

## Xem gallery

Mở [gallery](index.html) tại <http://127.0.0.1:8016/experiment-17/>. Có thể phục vụ từ root repository:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Trang chỉ chọn/xem/phóng ảnh. UI trong PNG là raster minh họa, chưa có nút game hoặc text component. Chữ HTML tăng không làm chữ raster reflow; nên mở PNG gốc để xem chi tiết. Các link01–16 phục vụ giữ thư viện, không đánh dấu bản cũ bị thay thế.

## Nguồn và ranh giới

Cả mười một lượt dùng built-in image_gen không ảnh input; nguồn copy nguyên bytes/hash và mapping đã lưu trong provenance. Lineage là quan hệ ý tưởng, không ảnh reference hoặc bằng chứng cùng cast/geometry. Diện mạo2D/2.5D/3D là câu hỏi thị giác, không model/scene/render từ geometry đã dựng. Không Hyper3D/Blender, pipeline model, gameplay, camera động, voxel engine hoặc canon mới.

Cast là ví dụ cuộc đời nhận ngẫu nhiên, không character creator/protagonist cố định. Curvature, relief, khối ghép, khung lồng, góc nước và khoang âm không tự đổi route/physics/tri thức, cho nhìn xuyên vách hoặc tạo teleport. [Workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [hồ sơ văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md) là nền tham chiếu, chưa chứng nhận phục dựng lịch sử các hình cụ thể.

Mọi phương án tiếp tục bổ sung thư viện tham khảo. Technical review, quan sát hình và quyết định user là ba lớp riêng; một ảnh chưa kiểm toàn game hoặc human playtest. [Approval register](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền chốt; Sprint13 vẫn **Pending user approval**, không tự merge dev, đóngSprint13 hoặc chuyểnSprint14.
