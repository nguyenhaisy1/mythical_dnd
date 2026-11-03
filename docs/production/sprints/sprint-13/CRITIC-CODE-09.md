# Phản biện code/provenance 09 — gallery style boards

29/10/2026 · Sprint13/SPIN-025 · Reviewer độc lập `/root/s13e10_critic_code`. **Technical reviewed trong phạm vi gallery; Pending user approval.** Không gọi generation, provider, Git hoặc chỉnh gameplay.

Đây là lượt review mới sau khi phiên09 bị ngắt trước lúc ghi report. Reviewer đã tự chạy lại checker hiện có; không nhận lượt kiểm của actor cũ là của mình.

## Phạm vi và nguồn

Đã đọc chỉ dẫn/baseline/HANDOFF, skill mythical-validation, source gallery và provenance. Sơn mài sống / Phù điêu đồng / Mực sống là PNG raster; UI game nằm trong ảnh, không có HUD HTML phủ lên hoặc thao tác authoritative state. Các control HTML chỉ phục vụ xem ảnh. Không có canvas/model-viewer, lệnh tạo model hoặc API game trong source đã kiểm.

Đã đối chiếu SHA256, bytes, RGB/kích thước1672×941 của từng PNG với provenance và kiểm riêng từng byte với đủ file gốc tương ứng trong generated_images. Đây là bằng chứng truy vết bản sao; không chứng minh typography, lịch sử hoặc mỹ thuật đã đạt.

## Kiểm đã chạy thật

`python3 prototypes/sprint13/experiment-09/browser-check.py`: **exit0**, Chrome **156.0.8078.47**, **1280×720,1920×1080,390×844**, mở `file://`.

- Ba bản đổi đúng qua nút, thumbnail và phím1/2/3 hoặc A/B/C. Ảnh được decode thành công, `aria-pressed` theo bản đang xem.
- Ảnh chính, thumbnail và modal dùng `contain`, không cắt board. Link “Mở ảnh gốc” trong modal trỏ đúng PNG đã chọn và file tồn tại.
- Escape đóng modal, focus trả về nút mở. HTML ở150% không tràn ngang. Reviewer đã mở screenshot mobile150 để đối chiếu bố cục.
- Có đủ liên kết lịch sử01–08, đúng đường dẫn file hiện có; nhãn thư viện nói giữ mọi phiên bản, không tự loại hoặc chốt thay user.
- Không có pageerror hoặc request HTTP/HTTPS ngoài trong lượt mở file local. Không có HUD HTML che phần UI đã vẽ trong board.

Evidence: `tests/results/sprint13-experiment09/browser.json`, `gallery-*-{1280,1920,390}.png`, `text150-{1280,1920,390}.png`; các file nằm dưới thư mục ignored. Đây là lượt chạy của reviewer này, không tự trở thành root đã trực tiếp chạy.

## Revision và giới hạn

| Source | SHA256 tại lượt kiểm |
|---|---|
| index.html | 3e60a30f86af394b3234f316fbdf50526bc3580fe5e2c94f788a3797b8f6f6da |
| style.css | 89dab0bd7921f473c04055452a3f212051d51c2e3199a9b20f7d858e0a23c241 |
| gallery.js | b38907b280f0d3c0e9dd2fc243a66dc1edf21f50179bb71bc83c00abce4c5912 |

Không phát hiện blocker kỹ thuật trong phạm vi đã kiểm. **150% chỉ đổi chữ HTML của trang xem; chữ UI trong bitmap không reflow hoặc tự tăng cỡ.** Trên mobile, board được thu nhỏ nguyên khung nên chữ/icon nhỏ cần mở PNG gốc để zoom. Không coi browser pass là game UI accessibility/contrast audit, OCR chính xác từng dấu, usability playtest hoặc khả năng triển khai mọi màn hình.

Không nghiệmthu Unity, gameplay, animation, art production, canon/khảo cổ hoặc các runtime controls được minh họa trong PNG. Không xếp hạng/loại các bản cũ; mọi phiên bản tiếp tục là tham khảo, quyền chốt thuộc chủ dự án.
