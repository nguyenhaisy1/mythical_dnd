# Phản biện code và provenance08

29/10/2026 · Sprint13/SPIN-024 · `/root/s13e8_critic_code`. **Technical reviewed cho gallery ảnh; Pending user approval.** Không sửa implementation gallery, gọi generation hoặc thực hiện Git.

## Phạm vi và kết quả thật

Đã đọc baseline/chỉ dẫn và source HTML/CSS/JS08. Gallery chỉ đổi ba PNG, không engine, canvas/model-viewer hoặc API gameplay. HUD dùng chung năm dòng trung tính về nhà bên suối; các hành động là nội dung minh họa, không thực thi chữa trị. Lớp giao diện không ghi WorldState.

Đã tạo và thực sự chạy `python3 prototypes/sprint13/experiment-08/browser-check.py`: **exit0**, Chrome **156.0.8078.47**, filelocal ở **1280×720,1920×1080,390×844**.

- Cả ba PNG1672×941RGB decode được, khác hash; SHA256/bytes/kích thước/mode khớp provenance. Đã đối chiếu đủ ba file gốc generated_images hiện có, từng byte giống bản trong project; không chỉ dựa metadata.
- Nút/phím A/B/C,1/2/3 và thumbnail đổi đúng ảnh/trạng thái. Năm dòng HUD giữ đúng nội dung trung tính cho từng phiên bản.
- Dialog chứa đúng ảnh, giữ contain; Escape đóng và trả focus. Checkbox/Space ẩn hiện HUD mà không đổi artwork.
- Chữ150% không tràn ngang tại ba viewport; không pageerror hoặc request HTTP/HTTPS trong chế độ filelocal. Main artwork không bị cover/crop.
- Bảy liên kết01–07 đúng thứ tự và trỏ file tồn tại. Nhãn thư viện nói mọi phiên bản được giữ lại, mở mới không có nghĩa bản trước bị loại hoặc bị đánh giá kém. Không dùng việc kiểm này để sửa lại đánh giá lịch sử.

Đã thực sự xem screenshot `text150-390.png`: các nút và chữ xuống dòng trong khung mobile, thư viện01–07 hiện đủ. Có khoảng nền trung tính quanh ảnh do min-height; không thấy mất cạnh ảnh. Đây là quan sát một screenshot, không đại diện mọi thiết bị.

## Revision và evidence

| File | SHA256 |
|---|---|
| index.html | 0dd9f8b3cb0b107e3692d761c640ece99d5c5304dc47ef2e97c8d1f286323098 |
| style.css | 60ea35a65a592fa5943b2675a9e722a809a0b2298939ebf63c93042776010fce |
| gallery.js | 0318b09a6172e8ac984adcdfa3cd5df9e03446d4c7e28fa5eef93d394695d4a7 |

Evidence ignored: `tests/results/sprint13-experiment08/browser.json`, `gallery-{a,b,c}-{1280,1920,390}.png`, `hud-off-{1280,1920,390}.png`, `text150-{1280,1920,390}.png`.

Không phát hiện blocker kỹ thuật trong phạm vi gallery08 đã kiểm. Hash/ảnh có chiều sâu không chứng minh model thật, renderer, camera tự do hoặc gameplay. Không nghiệmthu mỹ thuật, tính chính xác khảo cổ, Unity, humanplaytest hay Sprint13. Các phiên bản cũ giữ giá trị tham khảo; review08 không loại hoặc xếp hạng chúng.
