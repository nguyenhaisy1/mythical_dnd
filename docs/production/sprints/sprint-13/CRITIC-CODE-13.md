# Phản biện code13 — ba hướng, sáu khung chơi và chuỗi ảnh tham chiếu

30/10/2026 · Sprint13 / SPIN-030 · Reviewer độc lập `/root/s13e13_critic_code` · **Technical reviewed trong phạm vi PNG/provenance/gallery, Pending user approval**.

## Phạm vi thực sự kiểm

Đã đọc hướng dẫn project, baseline/HANDOFF, skill mythical-validation và TEST-EXECUTION-CONTRACT; đọc HTML/CSS/JS13, prompts/provenance cùng checker12 để xây phép kiểm phù hợp cấu trúc mới. Reviewer chỉ tạo `browser-check.py` và hồ sơ này; không tạo hoặc sửa artwork, gallery, tài liệu khác hay thực hiện Git publish.

Vòng13 có **ba hướng × hai frame**, không phải sáu phong cách độc lập. Gallery trưng bày `block-world`, `block-dialogue`, `frieze-world`, `frieze-dialogue`, `near-world`, `near-dialogue`; biến thể `frieze-world-v1` còn nguyên và có liên kết riêng. Source không có canvas/model-viewer hoặc HTML HUD game phủ lên ảnh; chỉ các nút xem/phóng lớn ảnh là tương tác thật.

## Kiểm nguồn ảnh và tham chiếu

Checker thực sự đọc/verify cả **bảy PNG** cùng bản original dưới generated_images:

- Sáu ảnh1672×941RGB; riêng `frieze-world` cuối1671×941RGB. Kích thước này được giữ nguyên; checker không ép mọi ảnh về một chiều rộng hoặc crop cho khớp.
- Bảy SHA256 khác nhau; số byte, kích thước, mode và hash khớp provenance. Từng original thực sự tồn tại và **giống từng byte** với bản trong assets.
- Sáu prompt chính và một prompt iteration là bảy chuỗi khác nhau; mỗi cặp `prompt_collection`/`prompt_key` gắn đúng một output. `frieze-world-v1` gắn với prompt chính `frieze-world`; bản đang trưng bày gắn với iteration `frieze-world-v2`.
- Năm cạnh tham chiếu nội bộ đều trỏ tới một PNG được lưu, hash input khớp metadata và hash của file thực. Chuỗi không có cycle: v1→frieze-world→frieze-dialogue; block-world→near-world→near-dialogue và block-world→block-dialogue (mũi tên ở đây từ ảnh nguồn đến ảnh tạo sau).
- Không có ảnh tham chiếu bên ngoài được khai báo. Không áp assertion “mọi reference đều rỗng” của lượt12 cho lượt13; các reference nội bộ là một phần thật của hồ sơ vòng này.

Đây là kiểm tính nhất quán metadata và truy vết bản sao/input, không tự chứng minh mức độ giữ mặt/cast/geometry hoặc việc prompt đã đạt chất liệu. Nội dung provenance ghi built-in image_gen và không model/runtime; browser/provenance check không biến những ảnh đó thành scene3D thật.

## Browser thực tế

Lệnh do reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-13/browser-check.py
```

**Lượt chạy đầu của checker13 exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**, mở bằng `file://`. Python/OS/architecture/thời điểm bắt đầu UTC và fingerprint nguồn có trong JSON.

- Ba direction cards, mỗi card hai frame; sáu nút trên, sáu nút thumbnail và phím1–6 đều đổi đúng ảnh theo thứ tự UI. Chỉ một lựa chọn có aria-pressed=true.
- Sáu ảnh chính và thumbnail decode đúng chiều rộng riêng của từng PNG; ảnh chính/thumbnail/modal dùng contain. Modal có đúng ảnh/link PNG gốc; Escape và nút Đóng đều đóng và trả focus về nút mở.
- HTML100/150% không tràn ngang ở ba viewport. Reviewer thực sự xem screenshot cuối `text150-390.png`: đủ sáu thumbnail, tên nút xuống dòng trong ô nhưng không cắt số thứ tự, nội dung và liên kết nằm trong bề rộng.
- Mười hai history links01–12 trỏ đúng file; link riêng của `frieze-world-v1` tồn tại và mở ảnh gốc. Các href nội bộ/PNG hiện hành không hỏng.
- Không pageerror, failed request hoặc HTTP/HTTPS request ngoài trong lượt filelocal. Source fingerprint trước/sau run giống nhau, không có gallery thay giữa quá trình capture.

Checker chủ động đặt `loading=eager` cho thumbnail rồi chờ decode trước full-page capture, kèm giới hạn chờ decode. Đây là chính sách chụp evidence của harness để ảnh lazy ngoài viewport không thành ô trống; không sửa source gallery hoặc giả lập gameplay. Không có lượt checker13 lỗi/dừng trước khi đạt trong review này.

Evidence ignored: `tests/results/sprint13-experiment13/browser.json`, 18 ảnh `gallery-{key}-{1280,1920,390}.png` và ba ảnh `text150-{1280,1920,390}.png`, tổng **21 screenshot**. Đây là số ảnh evidence, không phải số gameplay tests.

## Fingerprint của nguồn đã kiểm

| Source | SHA256 |
|---|---|
| index.html | 3f0fffeaec0f475694a3d84d88859ebf41bb515f57db48237e51559570d888dc |
| style.css | 69fdb5d808fe2120e8eb87e38a3dbc22586f75c97f103a263dd8f76edce2a9e5 |
| gallery.js | 8fe23c227f1106c0b5f599a8bb615fbc1d08a50481d3b8d211e79384d7441098 |
| prompts.json | 87e982f6d4b9132c50798c41974812a6d2cb96e2ab2ef5730c8aef2c49f951c1 |
| provenance.json | de790a2ecda7243fca7d0eb2bba06d494a21a8c2d05519a83a60bb0f5336b8ed |

## Kết luận và giới hạn

Không phát hiện blocker kỹ thuật cản xem ba cặp ảnh và biến thể được giữ trong phạm vi đã kiểm. Các ghi chú quan sát của root có mặt trong source đã kiểm; mức đúng/sai mỹ thuật của chúng thuộc review hình ảnh riêng.

HTML150 chỉ tăng chữ của trang xem, **không làm UI raster reflow hoặc tăng cỡ chữ riêng trong ảnh**. Trên điện thoại, toàn khung giữ nguyên nên UI/game chữ nhỏ; cần mở PNG gốc để zoom. Chưa OCR từng dấu tiếng Việt, đo contrast hoặc audit toàn bộ accessibility/usability của UI game. Chưa có gameplay, Unity, model, geometry, camera tự do, parallax, rig, motion, hiệu năng hoặc human playtest trong review này.

Cast và lời thoại là minh họa, không thay random-origin, thêm canon hoặc ghi authoritative state. Giữ01–12 và v1; review không chọn/loại hướng, đóngSprint13 hoặc cho phép Sprint14. Mọi sản phẩm tiếp tục **Pending user approval**.
