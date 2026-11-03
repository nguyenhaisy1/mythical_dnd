# Phản biện code16 — tám PNG và gallery sáu hướng

02/11/2026 · Sprint13 / SPIN-033 · Reviewer độc lập `/root/s13e16_critic_code` · **Technical reviewed trong phạm vi ảnh/nguồn/gallery, Pending user approval**.

## Phạm vi thực sự kiểm

Đã đọc chỉ dẫn project, `.agent`, HANDOFF, baseline áp dụng, skill mythical-validation và TEST-EXECUTION-CONTRACT; đọc HTML/CSS/JS16 cùng prompt/provenance và checker15 để xây phép kiểm riêng cho16. Reviewer chỉ tạo `prototypes/sprint13/experiment-16/browser-check.py` và hồ sơ này; không sửa gallery/artwork/tài liệu khác, sinh ảnh/model, commit hoặc publish.

Sáu hướng trưng bày theo thứ tự: woven-world, line-volume, folded-village, rain-sculpt, living-section, voxel-life. Hai bản đầu line-volume-v1 và folded-village-v1 còn qua link riêng. Các mô tả intent và ghi chú quan sát thực đã có trong gallery trước run. Nguồn không có canvas/model-viewer, HUD HTML game phủ lên PNG hoặc logic gameplay. Dệt/giấy/pixel có tiền lệ được ghi rõ; kiểm này không dùng tên phương pháp hoặc số file để kết luận sáu ý tưởng đều đột phá.

## Kiểm nguồn ảnh và prompt

Checker thực sự đọc/verify tám PNG trong assets và tám original tương ứng tại generated_images:

- **Bảy ảnh1672×941RGB; riêng `line-volume.png`1672×940RGB**, đúng metadata và original. Không ép ảnh về941 hoặc crop/resize để hợp phép kiểm. Tổng **16,985,863bytes**, tám SHA256 khác nhau, bytes/mode/kích thước/hash đều khớp. Mỗi file project **giống từng byte** với original còn tồn tại.
- Sáu prompt chính và hai iteration là tám chuỗi riêng; tám mapping dùng mỗi prompt đúng một lần. Bản line-volume/folded-village trưng bày dùng iterations/line-volume-v2 và folded-village-v2; hai bản v1 dùng prompt chính tương ứng. Bốn hướng còn lại dùng prompt chính cùng key.
- Top-level `reference_images` trong prompt/provenance và `reference_assets` ở từng asset đều rỗng; `conceptual_lineage_is_image_input=false`. Các prompt entry16 không có field `reference_assets` riêng; checker không giả rằng field ấy tồn tại. Hồ sơ mô tả tám lượt tạo mới không ảnh input, không có DAG ảnh tham chiếu hoặc reuse geometry được chứng minh.

Đây là kiểm tính nhất quán metadata, prompt mapping và truy vết bản sao original trong workspace; pixel/hash không tự chứng minh provider history, phong cách, cast, geometry hoặc mức đạt brief. Không nhận ảnh có thể tích là model/scene3D đã dựng.

## Browser thực tế

Lệnh do reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-16/browser-check.py
```

**Lượt đầu checker16 exit0**, Chrome **156.0.8078.79**, filelocal tại **1280×720, 1920×1080, 390×844**. Không có lượt checker16 lỗi hoặc dừng trước run đạt trong review này. Thời điểm bắt đầu UTC, Python/OS/architecture và fingerprints nguồn nằm trong JSON.

- Sáu card/thumbnail, switches và phím1–6 đổi đúng ảnh theo thứ tự; chỉ một switch aria-pressed=true. Tất cả ảnh chính và thumbnail decode đúng kích thước riêng của mỗi PNG. Main/thumbnail/modal dùng contain, giữ trọn ảnh.
- Modal mở ảnh đã chọn; ảnh decode, kích thước và link PNG gốc đều khớp. Escape và nút Đóng đóng modal, trả focus về nút mở. Thông báo image-error không hiện trong các trạng thái kiểm.
- Chữ HTML **100/125/150%** thực sự đổi root font **16/20/24px** và không tràn ngang ở cả ba viewport. Reviewer đã **mở trực tiếp `text150-390.png`**: đủ sáu thumbnail, nút chọn xuống hai cột, tên/ghi chú và thư viện còn hiện. Artwork giữ toàn khung nên chữ raster vẫn nhỏ; không nhận đây là bằng chứng UI game đọc tốt trên điện thoại.
- Mười lăm historylinks01–15 đúng thứ tự, đường dẫn và file tồn tại. Hai link retained đúng PNGv1, target mới và noopener. Các href local/PNG hiện hành đều trỏ file có thật. Ghi chú ba hướng14/1–3 vẫn được ưu tiên có trong nguồn, không bị đổi thành approval.
- Không pageerror, request HTTP/HTTPS ngoài hoặc failed request trong run filelocal. Fingerprints năm source trước/sau run khớp.

Harness đổi loading=eager và đợi thumbnail decode với timeout trước chụp full-page để ảnh ngoài viewport không bị trống. Đây chỉ là chính sách capture, không sửa source gallery. Evidence ignored ở `tests/results/sprint13-experiment16/browser.json`,18 ảnh `gallery-{key}-{1280,1920,390}.png` và3 ảnh `text150-{1280,1920,390}.png`: **21 screenshot**, không phải số gameplay tests. Worker preasset controls/layout là một nguồn evidence khác; reviewer không nhận run đó thành thao tác của mình.

## Fingerprints nguồn đã kiểm

| Source | SHA256 |
|---|---|
| index.html | 8d6cd70103e1235114d168010c7490414c339d4083d71747c4acdf2dc9330ff7 |
| style.css | db860400228df3537b5c60bd0d3b98877730085702594bdcfeb290af8c27c605 |
| gallery.js | 5f72ede2b0ebf57c818d25f3672185217c22c3f23be911bcd6908e5cea072df5 |
| prompts.json | ee987289c27b2995ef386d0de659ff4cc5a42e431fa944721f43d07d6a45a335 |
| provenance.json | a8da922950e91f1bc7e8527ba8cfac26b84e6398e109b927b3e8092027652b2c |

## Kết luận và giới hạn

Không phát hiện blocker kỹ thuật cản việc xem sáu hướng và hai bản đầu được giữ trong phạm vi trên. Review mỹ thuật cần mở ảnh riêng và đối chiếu intent với những gì thực sự xuất hiện; kiểm metadata/control không xác nhận hình dệt, accordion, nét dây hoặc phép chiếu đã đạt.

Chưa OCR từng dấu tiếng Việt, đo contrast, audit accessibility/usability đầy đủ của UI game, kiểm gameplay/Unity, model/scene, camera động, physics, navigation, weather, voxel engine, knowledge projection, animation, benchmark hoặc human playtest. UI raster không reflow khi tăng chữ HTML. Mini-map trong PNG không là dữ liệu world query đã triển khai. Original-byte check cần các original local còn tồn tại; không gọi run local là GitHub CI đã chạy.

Cast vẫn là minh họa cuộc đời nhận ngẫu nhiên, không character creator. Những phép nét rỗng/mặt cắt/gấp/voxel không tự thêm canon, x-ray, nhìn thông tin bí mật hoặc ghi authoritative state. Ba ưu tiên14/1–3 và tất cả phiên bản cũ được giữ; hai bản v1 không bị gán là user bác bỏ. Toàn bộ vòng16 và Sprint13 vẫn **Pending user approval**. Review này không cho phép merge dev, đóngSprint13 hoặc chuyểnSprint14.
