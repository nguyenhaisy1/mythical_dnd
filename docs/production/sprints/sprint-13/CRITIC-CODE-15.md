# Phản biện code15 — bảy PNG mới và gallery sáu hướng

30/10/2026 · Sprint13 / SPIN-032 · Reviewer độc lập `/root/s13e15_critic_code` · **Technical reviewed trong phạm vi PNG/prompt/gallery, Pending user approval**.

## Phạm vi thực sự kiểm

Đã đọc hướng dẫn project, `.agent`, HANDOFF, baseline áp dụng, skill mythical-validation và TEST-EXECUTION-CONTRACT; đọc HTML/CSS/JS15, prompts/provenance và checker13 để xây phép kiểm phù hợp cấu trúc15. Reviewer chỉ tạo `browser-check.py` và hồ sơ này; không sửa gallery/artwork/tài liệu khác, sinh ảnh/model hoặc thực hiện Git publish.

Gallery có sáu lựa chọn: vertical-earth, tidal-mosaic, rolling-world, cubist-space, shadow-world và mirror-water. Ba card đầu liên kết mẫu14/1–3 được chủ dự án đánh giá tốt để đào sâu; ba card sau là hướng mới. Biến thể cubist-space-v1 giữ nguyên qua link riêng. Liên hệ với mẫu14 là dòng ý tưởng, **không phải ảnh input của generation** và không tự là art approval. UI trong PNG là minh họa; source chỉ triển khai điều khiển xem ảnh, không có canvas/model-viewer hoặc HTML HUD game đè lên artwork.

## Kiểm nguồn PNG và mapping prompt

Checker thực sự đọc/verify cả **bảy PNG** và từng original trong generated_images:

- Bảy ảnh đều **1672×941 RGB**, có bảy SHA256 khác nhau; kích thước/mode/bytes/hash đúng metadata. Từng original thực sự tồn tại và **giống từng byte** với file trong assets. Không crop hoặc ép ảnh khác kích thước trong checker.
- Sáu prompt chính và một prompt iteration là bảy chuỗi khác nhau, bảy mapping dùng đúng mỗi prompt một lần. Bản cubist-space đang trưng bày gắn `iterations/cubist-space-v2`; bản v1 gắn `prompts/cubist-space`; năm frame còn lại gắn main prompt cùng key.
- Tất cả `reference_assets` và `reference_images` đều rỗng, metadata `conceptual_lineage_is_image_input=false`. Không dùng phép kiểm reference DAG của13 để giả rằng15 có các ảnh input hoặc dependency edges.

Đây là kiểm truy vết bản sao và tính nhất quán metadata, không chứng minh phong cách, cast, geometry, mức độ giữ hình hoặc provider provenance bằng riêng pixel. Tên công cụ trong hồ sơ và original được đối chiếu trong workspace thật; việc này không biến PNG thành model hoặc renderer runtime.

## Browser thực tế

Lệnh do reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-15/browser-check.py
```

**Lượt đầu checker15 exit0**, Chrome **156.0.8078.47**, mở `file://` tại **1280×720, 1920×1080, 390×844**, HTML100/150%. Thời điểm bắt đầu UTC, Python/OS/architecture và fingerprint nguồn nằm trong JSON.

- Sáu card/thumbnail, sáu switches và phím1–6 đổi đúng ảnh theo thứ tự UI; chỉ một switch aria-pressed=true. Tất cả ảnh chính và thumbnail decode đúng chiều rộng/cao; dùng contain ở main/thumbnail/modal.
- Modal mở đúng ảnh; link PNG gốc khớp lựa chọn, Escape và nút Đóng đều đóng và trả focus về nút mở. Thông báo image-error không hiện trong các trạng thái đã kiểm.
- HTML100/150 không tràn ngang ở ba viewport. Reviewer **thực sự mở `text150-390.png`**: đủ sáu thumbnail, các nút/tên/nội dung nằm trong bề rộng; nhãn xuống dòng trong ô. Ảnh artwork giữ trọn khung nên chữ UI raster nhỏ, không nhận đó là kiểm đọc UI game trên mobile.
- Mười bốn historylinks01–14 đúng thứ tự và trỏ file tồn tại. Ba link PNG14/1–3 đúng earth-monument/living-grid/round-world, mở target mới; v1 link trỏ cubist-space-v1 còn tồn tại. Các href nội bộ/PNG hiện hành đều hợp lệ.
- Không pageerror, HTTP/HTTPS request ngoài hoặc failed request trong lượt filelocal. Source hash trước/sau run giống nhau; đối chiếu lại khi viết report vẫn khớp.

Harness đặt loading=eager và chờ decode có timeout trước full-page capture để thumbnail lazy ngoài viewport không bị chụp trống. Đây là chính sách evidence của checker, không thay source gallery hoặc mô phỏng hành vi game. Không có lượt checker15 lỗi/dừng trước lượt đạt trong review này.

Evidence ignored: `tests/results/sprint13-experiment15/browser.json`,18 ảnh `gallery-{key}-{1280,1920,390}.png` và3 ảnh `text150-{1280,1920,390}.png`, tổng **21 screenshot**. Đây là số ảnh evidence, không phải số gameplay tests.

## Fingerprint nguồn đã kiểm

| Source | SHA256 |
|---|---|
| index.html | dc0acbc8747d5dafc4876565cd52184d29c325d6778221d9c40eed62c9b21d6e |
| style.css | e1a9267ca7d70064a576204696b157c4f131c5096e790ef6a4fc5b08b4c25cf1 |
| gallery.js | ab8b79bbe114e5f7f1f4181df354d48ea25dc30fbc2b698091746944f923e25a |
| prompts.json | 34f67db7991c05c4505af8ff69075ce6a4ecc628719795c0ccf75695edd6d9fb |
| provenance.json | 0863568b481c2606a85bdd7e88a0f2c886c81a112ee7d7aa4a48b4246d788bd0 |

## Kết luận và giới hạn

Không phát hiện blocker kỹ thuật cản việc xem sáu hướng và biến thể giữ lại trong phạm vi trên. Những chú thích actual output và các liên hệ shortlist có trong nguồn đã kiểm; review mỹ thuật phải căn cứ ảnh riêng, không được suy từ số file/card hay nhãn hướng.

HTML150 chỉ tăng chữ trang xem, không làm UI raster reflow hoặc tăng từng chữ trong PNG. Chưa OCR từng dấu tiếng Việt, đo contrast, audit accessibility/usability của UI game, runtime/Unity, model/scene, camera cuộn, physics, đường đi, phản chiếu, knowledge projection, animation, benchmark hoặc human playtest. Metadata/reference check không xác nhận hình học đa diện hoặc quang học phản chiếu là hợp lệ khi chơi. Original-byte check phụ thuộc các original đang tồn tại trong thư mục generated_images của lượt này; không gọi kết quả local là CI đã chạy trên máy khác.

Cast chỉ là ví dụ cuộc đời, không character creator; lời thoại/ký hiệu không tự ghi authoritative state, thêm canon hoặc cho phép UI thấy hidden truth. Shortlist14/1–3 được giữ đúng là ưu tiên đào sâu. Tất cả01–15 và v1 được giữ, các phương án chưa final accepted; review không đóngSprint13, merge dev hoặc cho phép Sprint14.
