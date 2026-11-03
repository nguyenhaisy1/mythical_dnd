# Phản biện code và provenance 06 — /root/s13e6_critic_code

29/10/2026, Sprint 13 / SPIN-022. Reviewer độc lập `/root/s13e6_critic_code`. **Technical reviewed trong phạm vi gallery ảnh; Pending user approval.** Không gọi imagegen, Hyper3D hoặc Blender, không sửa implementation gallery hoặc thực hiện Git.

## Phạm vi và nguồn thực tế

Đã đọc chỉ dẫn dự án, baseline, HANDOFF, skill mythical-validation, source HTML/CSS/JS và provenance của Experiment 06. Gallery chỉ chọn giữa ba PNG raster; không import engine/Three.js, không có canvas hoặc model viewer, không có lệnh tạo model 3D hay gọi dịch vụ gameplay. HUD là nội dung HTML minh họa, các lựa chọn nghề nghiệp là `span`, không giả nút đã thực hiện sửa thuyền. Chỉ các điều khiển xem ảnh có tương tác.

Một phép kiểm Python thực sự đối chiếu SHA256, số bytes, kích thước và mode ảnh với `provenance.json`. Cả ba ảnh khác hash; A/B là 1672×941, C là 1672×940, RGB. Đã đối chiếu từng byte với ba file gốc có tên tương ứng trong thư mục generated_images trên máy này. Đây là xác nhận bản sao ảnh/prompt/metadata có thể truy vết; không suy hash thành chất lượng mỹ thuật hoặc kiểm chứng lịch sử. Không coi ảnh B có chiều sâu như bằng chứng có scene 3D thật.

## Kiểm tra trình duyệt đã chạy

Reviewer tạo checker có phạm vi phù hợp ảnh raster rồi chạy:

```bash
python3 prototypes/sprint13/experiment-06/browser-check.py
```

Lượt cuối sau khi root thêm công tắc HUD: **exit 0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**, tải trực tiếp `file://`. Không gọi API tạo ảnh hoặc dịch vụ ngoài trong checker.

- Ảnh A/B/C được decode thành công với kích thước thực, chuyển đúng bằng nút, thumbnail và phím `1/2/3`, `a/b/c`; trạng thái `aria-pressed` theo ảnh đang xem.
- Dialog hiện đúng ảnh đã chọn, dùng contain, Escape đóng và trả focus về nút mở ảnh.
- Công tắc HUD ẩn được nội dung minh họa mà vẫn giữ nguyên ảnh; Space tại checkbox bật lại được HUD. Đây là kiểm thao tác người xem, không là gameplay.
- Ở 150% cỡ chữ, cả ba viewport không tràn ngang. Reviewer đã thực sự mở ảnh chụp mobile390/150 để kiểm bố cục; chữ và các phần xuống dòng trong bề rộng hiện có. Hình trên mobile có khoảng nền trung tính quanh ảnh do min-height; không phát hiện mất cạnh ảnh.
- Không có `pageerror` hoặc request HTTP/HTTPS trong chế độ file local. Mọi anchor trong gallery trỏ tới file local tồn tại, gồm thử nghiệm03 và05.
- Nhãn “Ảnh mô phỏng · Chưa phải game chạy” và “GIAO DIỆN MINH HỌA” hiện diện; không có control gameplay hoạt động trong HUD.

Evidence và ảnh chụp tại `tests/results/sprint13-experiment06/browser.json`, `gallery-{a,b,c}-{1280,1920,390}.png`, `hud-off-{1280,1920,390}.png`, `text150-{1280,1920,390}.png`; các file này nằm dưới thư mục ignored. Root có thể dùng ảnh chụp để kiểm lớp đầu; việc reviewer chạy không tự trở thành root đã trực tiếp chạy.

## Revision được kiểm và findings

SHA256 source tại lượt kiểm cuối:

| File | SHA256 |
|---|---|
| index.html | c4716a713f9d6441cdad158b3ceac989004117764cbe05d150e55f284943acee |
| style.css | a5b406e893d592e449b70e3d923e97f735703656f99cc5612240118eed8818e2 |
| gallery.js | 33921a98b863eb53587e8126ea3004be23e356dacbefc40225fcc935aaef0a20 |

Không phát hiện blocker kỹ thuật trong phạm vi đã kiểm. Công tắc ẩn HUD do root bổ sung theo phản biện hình ảnh đã được reviewer thực sự chạy lại; không chỉ đọc code để đóng finding. Nhận xét về màu, mật độ chi tiết, camera và cảm giác “AI” thuộc review hình ảnh và quyết định của chủ dự án, không được thông qua bằng browser pass.

Giới hạn: kiểm local-file trong Chrome hiện tại, không phải mọi trình duyệt/thiết bị, Unity, game runtime, human playtest hoặc accessibility audit đầy đủ. Kiểm file liên kết tồn tại không phải chạy lại toàn bộ các thử nghiệm cũ. Không chốt A/B/C, không nghiệm thu Sprint 13 hoặc cho phép chuyển Sprint 14.
