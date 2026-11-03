# Phản biện code14 — sáu hướng độc lập và trang xem ảnh

30/10/2026 · Sprint13 / SPIN-031 · Reviewer độc lập `/root/s13e14_critic_code` · **Technical reviewed trong phạm vi PNG/provenance/gallery, Pending user approval**.

## Phạm vi thực sự kiểm

Đã đọc hướng dẫn repository, `.agent`, HANDOFF, baseline áp dụng, skill mythical-validation/game-design-review và TEST-EXECUTION-CONTRACT. Đã đọc HTML/JS14, prompts/provenance, dùng checker13 làm cơ sở rồi bỏ các giả định không còn áp dụng về ba cặp frame, biến thể và chuỗi ảnh tham chiếu. Reviewer chỉ tạo `browser-check.py` và hồ sơ này; không sửa gallery, artwork hay thực hiện publish.

Sáu lựa chọn là sáu hướng riêng: `earth-monument`, `living-grid`, `round-world`, `craft-first-person`, `glyph-world`, `comic-life`. Source có sáu card, sáu thumbnail và sáu nút chọn. Các ghi chú quan sát của root nằm trong từng card. Không có canvas/model-viewer hoặc HTML HUD game phủ lên artwork; tương tác thực chỉ thuộc trang chọn và phóng lớn ảnh.

## PNG và truy vết nguồn

Checker thực sự mở/verify sáu PNG cùng các original đang có dưới generated_images:

- Tất cả **1672×941 RGB**; số bytes, kích thước, mode và SHA256 khớp provenance.
- Sáu hash khác nhau; từng original tồn tại và **giống từng byte** với bản ở assets.
- Sáu key/prompt khác nhau, từng `prompt_key` gắn đúng một output. `reference_assets` của mỗi ảnh/prompt và danh sách reference cấp hồ sơ đều rỗng.
- Metadata mô tả built-in image_gen, không model3D mới hoặc bằng chứng runtime/geometry. Không áp kiểm DAG/biến thể của13 lên một batch không có reference input.

Đây là kiểm tính nhất quán của bản sao và metadata; không tự chứng minh mọi lời yêu cầu trong prompt đã được công cụ thực hiện. Nhãn camera cong, planview hoặc ký hình vẫn cần review output riêng, không được checker xác nhận bằng tên file.

## Lượt browser thực tế

Lệnh do reviewer tạo và chạy:

```bash
python3 prototypes/sprint13/experiment-14/browser-check.py
```

**Lượt chạy đầu exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**, mở bằng `file://`. JSON giữ thời điểm bắt đầu UTC, Python/OS/architecture và fingerprint nguồn.

- Sáu nút trên, sáu card/thumbnail và phím1–6 đều đổi đúng ảnh; chỉ một lựa chọn có aria-pressed=true. Ảnh chính cùng thumbnail decode đúng kích thước và không crop (`contain`).
- Modal mở đúng bản, link PNG gốc đúng ảnh; Escape và nút Đóng đều đóng modal và trả focus về nút mở. Ảnh trong modal dùng contain.
- HTML100/150% không tràn ngang ở ba viewport. Reviewer thực sự xem `text150-390.png`: đủ sáu thumbnail, các nút và nội dung nằm trong bề rộng; tên một số nút xuống dòng nhưng số thứ tự không bị tách/cắt.
- Mười ba history links01–13 đúng thứ tự và trỏ tới file tồn tại. Các href nội bộ và PNG hiện hành không hỏng.
- Không pageerror, failed request hoặc request HTTP/HTTPS ngoài trong lượt filelocal. Source fingerprint trước/sau run giống nhau; reviewer kiểm lại hash sau khi run vẫn khớp.

Harness đặt thumbnail thành eager rồi chờ decode với timeout trước full-page capture để ảnh lazy ngoài viewport không thành ô trống. Đây là chính sách thu evidence, không sửa source hoặc mô phỏng gameplay. Không có lượt checker14 lỗi/dừng trước khi đạt trong review này.

Evidence ignored: `tests/results/sprint13-experiment14/browser.json`, 18 ảnh `gallery-{key}-{1280,1920,390}.png` và ba ảnh `text150-{1280,1920,390}.png`, tổng **21 screenshot**. Đây là ảnh evidence, không phải21gameplaytests.

## Fingerprint nguồn đã kiểm

| Source | SHA256 |
|---|---|
| index.html | e98aafcc12cac6a22e5816055af3eab9fbc29e55b6340f168d6c0563186a645a |
| style.css | 4bbc68051e41987eff03e5a23db2339890e09899d279c0c334487f711e8f052f |
| gallery.js | 2c15b15e7630bf2f6e67a4e546ab076a46a529ba032945cff7ee84917555d800 |
| prompts.json | 85f167b2c06cfd59fb97c0eccb6c846accb9620110a68b37586e3871927df3c7 |
| provenance.json | 2f4784c1dcc40465567289fb690a00cd53fae21fe842fed0b578d0c83e8b0a34 |

## Kết luận và giới hạn

Không phát hiện blocker kỹ thuật cản việc xem sáu hướng trong phạm vi đã kiểm. Reviewer không nhận việc xem screenshot thumbnail thành review sáu original về mỹ thuật; phần đó thuộc critic hình ảnh riêng. Chưa xác nhận chủ dự án thấy đủ khác biệt hoặc hướng nào tốt hơn.

HTML150 chỉ tăng chữ trang xem, **không làm chữ UI raster reflow**. Trên mobile, ảnh giữ nguyên khung nên chi tiết/chữ game nhỏ; mở PNG gốc để zoom khi xem concept. Chưa OCR từng dấu tiếng Việt, đo contrast hoặc audit accessibility/usability của UI game.

Không có game/Unity, scene/model/geometry mới, camera cong vận hành, chuyển động, parallax, hiệu năng hoặc human playtest trong review này. Ký hình không được chứng nhận là chữ viết cổ; trang truyện không chứng minh tương tác hoặc thời gian diễn tiến; hình thành đất không được chứng nhận là phục dựng. Cast không thay random-origin, thông tin minh họa không ghi authoritative state hoặc canon. Giữ01–13; không chọn/loại hướng, đóngSprint13, merge dev hoặc cho phép Sprint14. Mọi sản phẩm vẫn **Pending user approval**.
