# Phản biện code17 — mười phương án và một bản đầu được giữ

02/11/2026 · Sprint13 / SPIN-034 · Reviewer độc lập `/root/s13e17_critic_code` · **Technical reviewed trong phạm vi PNG/prompt/gallery; Pending user approval**.

## Phạm vi thực sự kiểm

Đã đọc hướng dẫn project, `.agent`, HANDOFF, baseline áp dụng, skill mythical-validation và TEST-EXECUTION-CONTRACT; đối chiếu HTML/CSS/JS17, prompt/provenance và checker16 trước khi tạo phép kiểm riêng cho17. Reviewer chỉ tạo `prototypes/sprint13/experiment-17/browser-check.py` và hồ sơ này; không sửa artwork/gallery, tài liệu khác, tạo ảnh/model hoặc thực hiện Git publish.

Mười hướng theo thứ tự gallery: earth-corridor, river-negative, gentle-arc, living-relief, gesture-flat, nested-frames, tangram-people, low-water, close-pixel, negative-space. `gentle-arc-v1.png` được giữ qua link riêng. Gallery đã có ghi chú output trước run. Ba hướng đầu là nhánh nghiên cứu từ14/1–3; bảy hướng sau có thể mới hoặc cải tiến, không dùng số tên để suy mười đột phá đã đạt. Nguồn không có canvas/model-viewer, HUD HTML game phủ lên PNG hoặc logic gameplay.

## Nguồn ảnh và mapping prompt

Checker thực sự đọc/verify **11 PNG** trong project và 11 original tương ứng còn tồn tại trong generated_images:

- Tất cả **1672×941 RGB**, tổng **22,092,490 bytes**, 11 SHA256 khác nhau. Kích thước, mode, bytes và hash đúng metadata; từng file project giống từng byte với original. Không crop, resize hoặc sửa ảnh để khớp kiểm tra.
- Mười prompt chính và một iteration là 11 chuỗi riêng, mỗi prompt được map đúng một lần. Bản `gentle-arc` trưng bày dùng `iterations/gentle-arc-v2`; bản v1 dùng `prompts/gentle-arc`; chín hướng còn lại dùng prompt chính cùng key.
- `reference_images` của prompt/provenance và `reference_assets` ở từng asset đều rỗng; `conceptual_lineage_is_image_input=false`. Prompt entry không có field reference riêng bắt buộc; checker không giả field ấy tồn tại. Lineage là quan hệ ý tưởng, không bằng chứng có ảnh input, matching cast hoặc cùng geometry.

Đây là kiểm truy vết bản sao và tính nhất quán metadata/mapping trong workspace thật. Pixel/hash không tự chứng minh phong cách, mức đạt brief, lịch sử provider hoặc tính khả thi geometry; không biến ảnh có diện mạo3D thành model/scene/render thật.

## Browser thực tế

Lệnh do reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-17/browser-check.py
```

**Lượt đầu checker17 exit0**, Chrome **156.0.8078.79**, Python3.14.3/Linux x86_64. Mở filelocal tại **1280×720, 1920×1080, 390×844**. Thời điểm bắt đầu UTC và fingerprint nguồn có trong JSON.

- Mười switch/card/thumbnail đúng thứ tự và decode đủ ảnh. Phím **1–9 chọn mẫu01–09, phím0 chọn mẫu10**; chỉ một switch aria-pressed=true. Không giả phím hai ký tự “10”. Main/thumbnail/modal dùng contain, không crop artwork.
- Modal mở đúng ảnh, link PNG gốc đúng ảnh đang chọn; Escape và nút Đóng đều đóng và trả focus về nút mở. Image-error không hiện trong các trạng thái đã kiểm.
- HTML100/125/150 tương ứng font gốc16/20/24px ở cả ba viewport; không tràn ngang. Đây chỉ tăng chữ trang xem, không làm UI raster reflow hoặc chứng minh chữ game đọc tốt trên điện thoại.
- Mười sáu historylinks01–16 đúng thứ tự, href và file tồn tại. Một link giữ `gentle-arc-v1.png` đúng target mới/noopener; các href nội bộ/PNG hiện hành tồn tại.
- Không pageerror, HTTP/HTTPS request ngoài hoặc failed request trong lượt filelocal. Năm source hash trước/sau run giống nhau; reviewer đối chiếu lại sau khi mở screenshot vẫn khớp.

Harness đặt loading=eager và chờ decode thumbnail có timeout trước full-page capture để ảnh lazy ngoài viewport không bị chụp trống. Đây là chính sách capture của checker, không sửa source gallery hoặc hành vi game. Không có lượt checker17 lỗi/dừng trước lượt đạt này.

Evidence ignored tại `tests/results/sprint13-experiment17/browser.json` và **35 screenshot**: 30 ảnh `gallery-{key}-{1280,1920,390}.png`, ba ảnh `text150-{width}.png`, hai ảnh gọn `scene-gesture-flat-1280.png` và `scene-close-pixel-390.png`. Hai ảnh gọn chụp locator `#scene`, nên kích thước file không nhất thiết bằng viewport; không phải crop/hậu kỳ PNG artwork. Reviewer **đã thực sự mở hai ảnh gọn này**: desktop giữ ảnh và vùng chọn tách khỏi HUD raster; mobile giữ toàn ảnh nhưng chữ/cụm đồ trong artwork vẫn nhỏ. Không tự nhận đã mở cả35 screenshot hoặc audit độ đọc UI game. 35 là số ảnh evidence, không số gameplay tests.

## Fingerprint nguồn đã kiểm

| Source | SHA256 |
|---|---|
| index.html | 9800170eccbe062de2216dbee62a8f8feea0d2d0c7ec2ea717d043a34b4b94a7 |
| style.css | 87c315b48e6f62648b4bed37758004007032f2cc07882de7989c2110d643fefa |
| gallery.js | e846387578700327897353971c51aeb2208a46441d4135346fd7485dfb33c9d4 |
| prompts.json | 51391eefd31f47f7197c7131b5c9228304e5ed8287faf657afe1c8635b5d8146 |
| provenance.json | 2be5dd21e536d317ee492042283a69cf11e76f9e81012c8da8f0a1523b35ff45 |

## Kết luận và giới hạn

Không phát hiện blocker kỹ thuật cản xem và so sánh mười hướng cùng bản đầu giữ lại trong phạm vi trên. Review mỹ thuật cần trực tiếp mở ảnh và kiểm delta so với lineage; kiểm controls/metadata không xác nhận relief, phép cong, pixel, âm bản hoặc camera đã đạt brief.

Chưa OCR từng dấu tiếng Việt, đo contrast, audit accessibility/usability đầy đủ, kiểm gameplay/Unity, model/scene, camera động, weather, navigation/physics, knowledge projection, animation, benchmark hoặc human playtest. Bản đồ/cửa/cắt khối trong PNG không chứng minh hệ thống truy vấn an toàn. Original-byte check cần original local còn tồn tại, không nhận run này là GitHub CI đã chạy.

Cast vẫn là ví dụ cuộc đời nhận ngẫu nhiên, không character creator. Những phép nhìn không tự thêm canon, luật di chuyển, nhìn xuyên vách hoặc tri thức bí mật. Ba ưu tiên14/1–3, tất cả bản cũ và v1 tiếp tục được giữ; v1 không bị gán là user bác bỏ. **Toàn bộ output17 và Sprint13 vẫn Pending user approval**; review này không cho phép merge dev, đóngSprint13 hoặc chuyểnSprint14.
