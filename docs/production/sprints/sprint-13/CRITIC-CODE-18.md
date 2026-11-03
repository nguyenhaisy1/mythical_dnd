# Phản biện code18 — mười lăm phương án và hai bản đầu được giữ

02/11/2026 · Sprint13 / SPIN-035 · Reviewer độc lập `/root/s13e18_critic_code` · **Technical reviewed trong phạm vi PNG/prompt/gallery; Pending user approval**.

## Phạm vi thực sự kiểm

Đã đọc chỉ dẫn project, `.agent`, HANDOFF, baseline áp dụng, skill mythical-validation và TEST-EXECUTION-CONTRACT; đối chiếu HTML/CSS/JS18, prompt/provenance và checker17 trước khi tạo phép kiểm riêng cho18. Reviewer chỉ tạo `prototypes/sprint13/experiment-18/browser-check.py` và hồ sơ này; không sửa artwork/gallery, tài liệu khác, tạo ảnh/model, thực hiện Git hoặc ghi WORKLOG.

Mười lăm hướng theo thứ tự gallery: open-yard, three-bands, shoulder-craft, long-diagonal, column-forest, plan-profile, cel-adult, soft-mass, ink-mass, ribbon-body, contour-step, square-gesture, one-bit, single-stage, retro-facet. Hai bản đầu `ribbon-body-v1.png` và `contour-step-v1.png` được giữ qua link riêng. Gallery đã có ghi chú output trước run. Tên hiển thị plan-profile được root đồng bộ thành “Sân phẳng, người nổi” trước khi reviewer lấy fingerprint và chạy checker; reviewer không tự sửa nguồn ấy.

Nguồn chỉ triển khai điều khiển xem ảnh. Không có canvas/model-viewer, HUD HTML game phủ lên PNG hoặc logic gameplay. Kiểm này không dùng số tên, labels3D/one-bit/contour hoặc số PNG để kết luận các phương pháp đã đạt mỹ thuật.

## Nguồn ảnh và mapping prompt

Checker thực sự đọc/verify **17 PNG** trong project và 17 original tương ứng còn tồn tại trong generated_images:

- **15 ảnh1672×941 RGB; riêng `plan-profile.png` và `contour-step-v1.png`1672×940 RGB**. Tổng **30,511,159 bytes**, 17 SHA256 khác nhau. Kích thước/mode/bytes/hash khớp metadata; mỗi file project giống từng byte với original. Không crop, resize hoặc sửa ảnh để khớp phép kiểm.
- Có15 prompt chính và2 iteration, 17 chuỗi riêng và17 mapping dùng mỗi prompt đúng một lần. Hai bản ribbon-body/contour-step trưng bày dùng iterations/ribbon-body-v2 và contour-step-v2; hai bản v1 dùng prompt chính tương ứng;13 hướng còn lại dùng prompt chính cùng key.
- `reference_images` ở prompt/provenance và `reference_assets` từng asset đều rỗng; `conceptual_lineage_is_image_input=false`. Các prompt entry không có field `reference_assets` riêng; checker không giả rằng field ấy phải tồn tại, đồng thời kiểm field nếu được khai báo.
- `one-bit.png` thực tế là **RGB**, như mọi ảnh khác. “Một-bit” là tên mục tiêu diện mạo; không được đổi kết quả mode hoặc nhận tên hướng thành file/renderer1-bit thật.

Đây là kiểm tính nhất quán metadata, prompt mapping và truy vết bản sao original trong workspace. Pixel/hash không độc lập chứng minh lịch sử provider, mức đạt grammar, tuổi/cast, topology hoặc geometry. Không nhận ảnh có chiều dày hoặc thể tích là model/scene đã dựng thật.

## Browser thực tế

Lệnh reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-18/browser-check.py
```

**Checker18 của critic lượt đầu exit0**, Chrome **156.0.8078.79**, mở filelocal tại **1280×720, 1920×1080, 390×844**. Thời điểm UTC bắt đầu, Python/OS/architecture và fingerprint nguồn nằm trong JSON.

- 15 card/thumbnail và15 switch đúng thứ tự; mọi thumbnail và ảnh chính decode đúng kích thước thật, chỉ một switch `aria-pressed=true`. Main/thumbnail/modal dùng contain, không ép hai ảnh940px thành941px.
- Phím1–9 chọn01–09, phím0 chọn10; **ArrowLeft/ArrowRight chuyển đủ15 hướng và wrap hai đầu**, Home chọn01, End chọn15. Mỗi card đổi đúng ảnh, gồm cả11–15 không có phím số đơn riêng. Không báo đã dùng phím “11” như một shortcut.
- Modal mở đúng ảnh, PNG gốc khớp lựa chọn; Escape và nút Đóng đóng modal rồi trả focus về nút mở. Không hiện image-error trong trạng thái đã kiểm.
- HTML100/125/150 tương ứng rootfont16/20/24px không tràn ngang tại cả ba viewport. Reviewer **thực sự mở `scene-ribbon-body-1280.png` và `scene-one-bit-390.png`**: desktop có15 nút và artwork nguyên khung; mobile hai cột, nhãn dài xuống dòng, mẫu15 ở hàng cuối. UI raster trong ảnh bị thu nhỏ; không nhận đây là bằng chứng UI game/mobile đọc tốt.
- 17 historylinks01–17 đúng thứ tự và trỏ file tồn tại. Hai link v1 đúng asset, mở target mới có noopener. Các href nội bộ/PNG hiện hành đều hợp lệ.
- Không pageerror, HTTP/HTTPS request ngoài hoặc failed request trong lượt filelocal. Năm source hash trước/sau giống nhau và đối chiếu lại trước bàn giao vẫn khớp.

Harness đặt thumbnail loading=eager và chờ decode có timeout trước full-page capture. Đây là chính sách evidence của checker, không sửa source gallery hoặc mô phỏng gameplay. **50 screenshot** gồm45 ảnh gallery (15 hướng×3viewport),3 ảnh text150 và2 scene capture gọn nêu trên. Reviewer không tự nhận đã mở toàn bộ50 ảnh; hai scene capture được xem trực tiếp, các trạng thái khác có kiểm tự động.

Evidence ignored: `tests/results/sprint13-experiment18/browser.json` và50 screenshot. Đã đọc thêm `worker-gallery-attempt1.json` và `worker-gallery.json`: **preasset worker lượt1 failed** vì harness tạm đòi file PNG chưa tạo, lượt2 passed sau sửa riêng phạm vi checker tạm, gallery không đổi. Đó là lịch sử preasset worker, không phải lần chạy checker độc lập của critic; không giấu lượt failed hoặc gộp nó thành run asset đã đạt.

## Fingerprint nguồn đã kiểm

| Source | SHA256 |
|---|---|
| index.html | 7bcdc36b23a9a67c2b24e3b3bf63454ea0a3df09e88155b11413f8e2087d47f3 |
| style.css | c0c4e55ac09a6e70812422e533a11e3f11bf013c163feef6f3e15b4344d093ec |
| gallery.js | fe85ddfa8ad4ed7fd1c9130503710d7da6e959a0aab0ce19e3c53b78f2d7ff91 |
| prompts.json | e2ceb487f2b035de155abfe8c33cc948362e70dbd6de493e963d1fca3b9c4edb |
| provenance.json | 12b0a51a792a2107d5dd708f2dae0fd442fdb8fa480cc54a90cb602bfa7bfab0 |

## Kết luận và giới hạn

Không phát hiện blocker kỹ thuật cản xem và so sánh15 hướng cùng hai bản đầu được giữ trong phạm vi trên. Review mỹ thuật cần trực tiếp mở artwork và kiểm delta; controls/metadata không xác nhận hybrid, dải thân, khối đồng mức, cel ba sắc, pixel hoặc one-bit đã đạt brief.

Chưa OCR từng dấu tiếng Việt, đo contrast, audit accessibility/usability đầy đủ, kiểm gameplay/Unity, model/scene, camera động, navigation/physics, knowledge projection, animation, benchmark hoặc human playtest. UI raster không reflow khi tăng chữ HTML. Original-byte check cần các original local còn tồn tại; không nhận run này là GitHub CI đã chạy hoặc có thể chạy nguyên vẹn trên máy thiếu original.

Cast là ví dụ cuộc đời nhận ngẫu nhiên, không character creator. Các phép hybrid/khối/dải không tự thêm canon, quyền nhìn xuyên hoặc tri thức bí mật. Ba ưu tiên14/1–3, toàn bộ phiên bản cũ và hai bản v1 vẫn được giữ; v1 không bị gán là user bác bỏ. **Toàn bộ output18 và Sprint13 tiếp tục Pending user approval**; review không cho phép merge dev, đóngSprint13 hoặc chuyểnSprint14.
