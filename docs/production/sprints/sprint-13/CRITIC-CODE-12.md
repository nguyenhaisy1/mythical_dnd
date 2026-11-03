# Phản biện code12 — gallery sáu ngôn ngữ mỹ thuật

30/10/2026 · Sprint13/SPIN-028 · Reviewer độc lập `/root/s13e12_critic_code`. **Technical reviewed trong phạm vi provenance và trang xem ảnh; Pending user approval.** Reviewer không tạo ảnh, sửa gallery, thực hiện Git hoặc nghiệm thu mỹ thuật.

## Nguồn và phạm vi

Đã đọc hướng dẫn repository, baseline/HANDOFF, mythical-validation, HTML/CSS/JS12, prompts và provenance; đối chiếu phạm vi evidence với TEST-EXECUTION-CONTRACT. Sáu lựa chọn là các PNG riêng: `clay-craft`, `clear-line`, `mineral-fresco`, `retro-prerender`, `charcoal-night`, `graphic-animation`. Source không có canvas/model-viewer hoặc HTML game HUD phủ lên artwork; chỉ các điều khiển của trang xem là tương tác.

Checker xác nhận sáu prompt khác nhau, sáu key khớp provenance và sáu PNG có SHA256 khác nhau. Từng ảnh decode/verify được, RGB, **1672×941**, số byte/hash khớp metadata. Cả sáu original trong thư mục generated_images thực sự tồn tại và **giống từng byte** với bản ở assets. Metadata ghi built-in image_gen, không ảnh tham chiếu, không model thật hoặc bằng chứng runtime/geometry. Đây là kiểm truy vết bản sao và tính nhất quán của hồ sơ, không chứng minh mức độ đạt phong cách của từng prompt.

## Lượt browser thực tế

Reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-12/browser-check.py
```

Lượt cuối **exit0**, Chrome **156.0.8078.47**, Python/OS/architecture và thời điểm bắt đầu UTC được lưu trong JSON. Ba viewport: **1280×720, 1920×1080, 390×844**, mở bằng `file://`.

- Sáu ảnh chính và sáu thumbnail decode đúng kích thước. Nút chọn, sáu thumbnail và phím1–6 đều đổi đúng bản; chỉ một lựa chọn có aria-pressed=true.
- Ảnh chính/thumbnail/modal dùng contain. Modal mở đúng ảnh, link PNG gốc trỏ đúng bản; Escape và nút Đóng đều đóng modal và trả focus về nút mở.
- HTML100/150% không tràn ngang ở ba viewport. Reviewer đã thực sự xem ảnh cuối `text150-390.png`: sáu thumbnail đều hiện, các phần nội dung/nút/thư viện nằm trong bề rộng. Ở mobile150%, số thứ tự trang trí một vài nút xuống hai dòng; tên mẫu vẫn đọc được. Đây là hạn chế trình bày nhỏ, không cản chọn mẫu.
- Mười một liên kết lịch sử01–11 đúng thứ tự và trỏ tới file tồn tại. Các liên kết nội bộ/PNG hiện hành không hỏng; source ghi rõ giữ mọi phiên bản.
- Không pageerror, failed request hoặc request HTTP/HTTPS ngoài khi mở filelocal. Không dùng CSS pixelated để giả đầu ra pixel, không có engine/model controls trong gallery.

Evidence ignored: `tests/results/sprint13-experiment12/browser.json`, 18 ảnh `gallery-{key}-{1280,1920,390}.png` và ba ảnh `text150-{1280,1920,390}.png`. **21 screenshot cuối** được ghi lại sau khi root freeze source và thêm ghi chú quan sát cho sáu card.

### Các lượt chưa đạt của checker

Hai lượt đầu **exit1** vì assertion câu mô tả còn dùng wording cũ rồi một câu reviewer nhập chưa khớp; đã sửa checker theo nội dung12, không sửa gallery để làm test xanh. Một lượt sau bị reviewer chủ động dừng **SIGTERM/exit143**: checker gọi `decode()` vào thumbnail lazy ngoài viewport, promise chờ ảnh chưa được yêu cầu tải. Đây là lỗi harness, không là bằng chứng gallery đổi ảnh bị treo. Checker cuối chuyển thumbnails sang eager và chờ decode trước full-page capture; mọi ảnh trong screenshot cuối đã tải. Không tính các lượt dừng này là Passed.

## Revision của lượt cuối

| Source | SHA256 |
|---|---|
| index.html | 3117b00f92fee0c0f64f1cb7f5cc9adcf5e5e5b57c80cc9c18c8d6f4b8f7a19f |
| style.css | 62f1d60a9c983ad2b15e68b128be400dc7d6c706b029dd24a7bf3e85612fd679 |
| gallery.js | 88f3cc6c66e1ad91b0890a0cf034c95c23a5c919b6a4c3725a7909b7c70ba8e7 |
| prompts.json | 2602907f73de5dd39d1fd590c793b502f84de1a63c3604002c6e92c4fc07deaa |
| provenance.json | 6e20f88c77159e484e5976f4d44dbea1f9d2bf93342d05c0e58c7e26b15cc557 |

## Kết luận có giới hạn

Không phát hiện blocker kỹ thuật cản việc xem sáu mẫu trong phạm vi đã kiểm. **HTML150% chỉ tăng chữ của gallery; UI raster không reflow hoặc tự tăng cỡ chữ.** Board giữ toàn khung nên chữ/icon game rất nhỏ trên điện thoại, cần mở PNG gốc để zoom. Chưa audit OCR từng dấu tiếng Việt, contrast hoặc accessibility/usability của UI game; số lượng screenshot không phải số gameplay tests.

Việc tên mẫu là đất nặn, fresco hoặc retro không tự chứng minh output đạt chất liệu, mức giản lược, độ phân giải hay pipeline sản xuất đó. Không có runtime/Unity, scene3D, model, rig, animation, camera tự do, performance hoặc human playtest evidence. Cast minh họa không cố định protagonist hay thay baseline random-origin. Review không loại01–11, chốt mỹ thuật, đóng Sprint13 hoặc cho phép Sprint14.
