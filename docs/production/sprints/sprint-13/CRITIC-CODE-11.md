# Phản biện code11 — gallery bốn cách thể hiện

30/10/2026 · Sprint13/SPIN-027 · Reviewer độc lập `/root/s13e11_critic_code`. **Technical reviewed trong phạm vi trang xem ảnh; Pending user approval.** Không gọi imagegen/Hyper3D, không chỉnh gallery hoặc thực hiện Git.

## Nguồn và provenance đã kiểm

Đã đọc baseline/HANDOFF, skill mythical-validation, HTML/CSS/JS11 và metadata. Gallery chọn giữa bốn PNG, không có canvas/model-viewer hoặc game controls chạy trong board. UI game nằm trong raster; chỉ các nút đổi/xem ảnh trên trang có tương tác. Không có HUD HTML phủ lên ảnh để giả UI của các hướng.

Đã kiểm SHA256, bytes, mode RGB và kích thước1672×941 của từng PNG với provenance; bốn ảnh có hash khác nhau. Một phép kiểm Python riêng xác nhận **cả bốn file output gốc tồn tại và giống từng byte với bản trong assets**. Provenance ghi ngày tạo30/10 và prompt được chuẩn bị29/10; không coi thời điểm brief là ngày đã có artwork. Đây là bằng chứng bản sao/truy vết, không là art acceptance hoặc bằng chứng model3D/parallax/pixel-grid production.

## Browser thực sự đã chạy

Reviewer tạo checker từ phạm vi phù hợp của10 và chạy:

```bash
python3 prototypes/sprint13/experiment-11/browser-check.py
```

Lượt cuối: **exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720,1920×1080,390×844**, mở bằng `file://`.

- Bốn ảnh decode được với kích thước thực; đổi đúng bằng nút và phím1–4, trạng thái aria-pressed theo bản đang xem. Đã thử chọn lại ảnh đầu qua thumbnail.
- Ảnh chính/thumbnail dùng contain; modal mở đúng ảnh, linkPNGgốc trỏ đúng file theo bản, Escape đóng và trả focus về nút mở.
- ChữHTML150% không tràn ngang ở cả ba viewport. Reviewer đã thực sự xem `text150-390.png`: nút xuống hàng, text và thư viện vẫn nằm trong bề rộng. Board thu nhỏ nguyên khung nên chữ/iconUI trong ảnh rất nhỏ, cần mởPNGgốc để zoom.
- Mười liên kết lịch sử01–10 tồn tại đúng thứ tự; trang ghi rõ giữ mọi phiên bản và không tự thay thế/đánh giá thấp bản trước.
- Không pageerror hoặc requestHTTP/HTTPS ngoài khi mở filelocal. Không có HTMLHUD, engine hoặc network gameplay trong phạm vi source đã kiểm.

Lượt checker đầu dừng do assertion còn giữ nguyên một câu của gallery10; reviewer sửa assertion cho đúng nội dung thư viện11, không sửa gallery. Một lượt giữa bịSIGTERM143 trước kết quả cuối, nguyên nhân chưa xác định; không tính là Passed. Kết luận trên dựa vào lượt chạy sau đó thực sự exit0 và evidence được ghi đầy đủ.

Evidence ignored: `tests/results/sprint13-experiment11/browser.json`, `gallery-{key}-{1280,1920,390}.png`, `text150-{1280,1920,390}.png`. Đây là lượt reviewer chạy, không tự trở thành root đã chạy trực tiếp.

## Revision và kết luận có giới hạn

| Source | SHA256 tại lượt kiểm cuối |
|---|---|
| index.html | 9c8a53ca77caa444dc1053ec3ae9f746f8c3627bfd25a335968a32395121994c |
| style.css | 0badcfa1acfc482d9938c582c506c4f9ff35ac5db81c06c6ac7381141a1b6072 |
| gallery.js | dd66d9aa1ff4e4af209777ecccc2841b13725059e2bd20be3cca25566369ffad |

Không phát hiện blocker kỹ thuật trong phạm vi đã kiểm. **150% chỉ đổi chữHTML của trang xem; UI bitmap không reflow hoặc tự tăng cỡ.** Chưa có OCR/audit đầy đủ từng dấu tiếng Việt, contrast, gameUI usability hoặc mọi browser/thiết bị. Chưa có model, camera tự do, parallax, animation, tileset/spritesheet chuẩn, benchmark hoặc quyết định chuyển engine từ ảnh tĩnh.

2.5D/3D và pixel/anime là các trục có thể kết hợp, không là bốn phương án công nghệ loại trừ nhau. Mẫu nữ lao động trưởng thành trong board không sửa baseline random-origin. Review này không chốt mỹ thuật, game/Unity/humanplaytest, không loại bản cũ hoặc nghiệmthu Sprint13/chuyểnSprint14.
