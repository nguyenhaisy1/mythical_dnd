# Thử nghiệm16 — Sáu quy tắc tạo hình khác nhau

02/11/2026 · Sprint13 / SPIN-033 · **Thư viện tham khảo, Pending user approval**.

Vòng16 được giao thêm những ý tưởng mới. Sáu hướng dưới thử thay cấu trúc hình ảnh ở cả thế giới, con người và UI: dải đan, nét có thể tích, nếp gấp, mưa/khoảng âm, mặt cắt ngang và hạt khối. Giữ toàn bộ01–15; ba mẫu14/1–3 tiếp tục ưu tiên đào sâu, chưa chọn style cuối.

Dệt/giấy/pixel từng có ở08/B,07 và11; đây không phải tuyên bố sáu chất liệu hoàn toàn chưa xuất hiện. Delta lần này nằm ở footprint/thân người bằng dải đan, các mặt gấp tạo địa bàn và hạt khối có thể tích, cần kiểm bằng output thật.

## Bộ ảnh đã tạo

| Hướng | PNG được giao | Câu hỏi cần kiểm bằng ảnh thật |
|---|---|---|
| Thế giới dệt | `assets/woven-world.png` | Đường/bờ/thân người cùng được cấu tạo bằng dải đan, hay chỉ có texture vải? |
| Nét dây không gian | `assets/line-volume.png` | Nét, mặt đặc và che khuất có tạo thể tích mà vẫn đọc được người/vật? |
| Làng gấp | `assets/folded-village.png` | Nếp gấp đổi không gian và hình người, hay chỉ là cutout/paper texture? |
| Mưa khắc hình | `assets/rain-sculpt.png` | Mưa/khoảng âm đổi silhouette và lớp sâu, hay chỉ đổi màu/thời tiết? |
| Mặt cắt đời sống | `assets/living-section.png` | Góc ngang giữ một cuộc đời dễ đọc mà không thành quản lý nhà toàn tri? |
| Hạt khối | `assets/voxel-life.png` | Hạt khối có thể tích xuyên cảnh/người/UI, hay chỉ là filter pixel2D? |

Đã có sáu frame chính và hai bản đầu được giữ qua link riêng: `line-volume-v1.png`, `folded-village-v1.png`. Tám lượt built-in image_gen mới, không ảnh input; PNG sao chép nguyên bytes, không hậu kỳ. Bảy ảnh1672×941RGB, riêng `line-volume.png`1672×940RGB; tổng16,985,863bytes. [Prompts](prompts.json) giữ sáu prompt đầu và hai iteration; [provenance](provenance.json) ghi nguồn/hash/bytes/kích thước/mapping. Bản đầu nét dây còn là tranh có viền, bản đầu làng gấp còn là địa hình tầng; root thử lại để đổi cấu trúc và vẫn giữ cả hai, không gán user đã loại. Không dùng tên hướng làm kết luận thay ảnh. [ART-DIRECTION](ART-DIRECTION.md) giữ grammar, delta, ví dụ đúng/tránh và ranh giới từng hướng; gallery ghi quan sát thực dưới từng card.

## Xem gallery

Mở [gallery](index.html) tại <http://127.0.0.1:8016/experiment-16/>. Có thể phục vụ từ root repository:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Trang chỉ chọn/xem/phóng ảnh. UI bên trong PNG là raster minh họa; chưa có nút game hoặc text component. Tăng chữ HTML không làm chữ raster reflow; nên mở PNG gốc khi cần đọc chi tiết.

## Phạm vi

Chỉ ảnh mô phỏng game bằng built-in image_gen, không model/scene3D thật, render từ scene đã dựng, Hyper3D/Blender, shader/camera runtime hoặc implementation. Diện mạo3D là mục tiêu thị giác, không bằng chứng geometry thật. Các phép dệt/gấp/mặt cắt/khối hạt không thêm luật di chuyển, x-ray, quản lý toàn tri, vật lý hoặc xây/phá block.

Cast là ví dụ một cuộc đời nhận ngẫu nhiên; không character creator hoặc protagonist cố định. UI chỉ có thể trình bày điều nhân vật được phép biết khi triển khai. Căn cứ văn hóa ở [workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [hồ sơ ngữ pháp](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md); sáu phương pháp là sáng tạo hiện tại, chưa là phục dựng lịch sử được chứng nhận.

Một frame mỗi hướng chưa kiểm toàn roster, các vùng, UI túi đồ/nhật ký, chuyển động hoặc độ đọc khi chơi. Những quan sát, kiểm kỹ thuật và quyết định chủ dự án là các lớp riêng. [Approval register](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền chốt; mọi output16 vẫn Pending, không tự merge dev, đóngSprint13 hoặc chuyểnSprint14.
