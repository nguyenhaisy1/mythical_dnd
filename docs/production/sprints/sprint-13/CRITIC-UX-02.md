# Critic UX độc lập — Experiment 02

27/10/2026; `/root/s13_critic_ux`. Đọc ART-DIRECTION, nguồn GENERATION, HTML/CSS/campaign; xem raster gốc và ảnh browser thực. **Pending user approval.** Thử nghiệm01 bị người dùng đánh giá hình ảnh chưa đạt; review này không thay đổi đánh giá đó và không tự chốt hướng02.

## Finding

**UX02-01 — P1, cần sửa:** ở World1280, panel Quan sát bên phải che hoàn toàn người thợ tiền cảnh và phần lớn bên phải chiếc thuyền, chính là tiêu điểm được art direction yêu cầu. Text150% làm panel lớn hơn. Đã báo root/UI worker; cần chuyển panel, thu gọn thành trigger hoặc đưa thông tin ra ngoài vùng tiêu điểm. Nút xem toàn cảnh không sửa được vấn đề bố cục mặc định. Evidence `tests/results/sprint13-experiment02/world-1280.png`, `world-150-1280.png` và ảnh riêng `critic-ux/world-before-fix.png`.

## Evidence và nhận xét

Chạy thực `python3 prototypes/sprint13/experiment-02/browser-check.py`: passed2viewports1280×720/1920×1080. Kiểm decodePNG, menu/reveal/world, provenance kỹ năng, journal, cancel/Escape/focus và text150% không tràn ngang. Critic thêm browser session độc lập mở full-art bằng Enter/Escape và đọc natural/rendered dimensions; artifact `critic-ux/art-dimensions.json`. Đây là browser UX, không Unity, gameplay, human playtest hoặc approval.

Đã xem ảnh gốc: sinh hoạt nhà sàn/bến nước, gỗ có dấu sử dụng, người sửa thuyền, dây thừng/giỏ và lớp sương tạo chiều sâu rõ hơn cách mảng phẳng01. Không thấy cung điện/giáp fantasy hoặc chữ bake vào tranh. Bản sắc Việt cổ và tính chính xác hình thái nhà/thuyền/trang phục chưa được kiểm chứng; GENERATION/ART-DIRECTION công khai hư cấu. Không có cơ sở để báo historical accuracy. Ở mức ảnh toàn cảnh không thấy lỗi giải phẫu nổi bật; chưa phải kiểm anatomy từng pixel/asset production.

Full-art dùng ảnh nguyên khối `width:100%;height:auto`, không object-fit:cover; dialog cuộn được khi ảnh cao hơn viewport, nên không mất phần nguồn ảnh. Màn World dùng crop nên review tiêu điểm cần riêng. Nội dung kỹ năng/ký ức và giới hạn “chưa lưu/chưa game” còn được giữ; artwork không là actor/state authoritative.

## Trạng thái

Chưa technical sign-off bố cục World cho đến khi UX02-01 sửa và kiểm lại. Chất liệu/hướng hình ảnh/giao diện/nhân vật vẫn Pending user approval kể cả sau sửa.

## Kiểm lại cuối — 28/10/2026

**Technical reviewed trong phạm vi browser prototype; Pending user approval.** Mục trạng thái phía trên ghi nhận lần review trước khi sửa, được thay thế bởi kết quả kiểm lại này.

- UX02-01/P1 đã giải quyết: hai panel chuyển sang bên trái, vùng người thợ và thân thuyền bên phải hiện rõ. Critic chạy browser độc lập tại1280×720 với chữ100%/150%, chụp `critic-ux/final-world-100.png` và `final-world-150.png`; đã xem ảnh150% trực tiếp. Người thợ không bị panel hoặc marker che. Ở150%, trang dài hơn viewport và cần cuộn; đây không phải nội dung bị cắt mất.
- UX02-02/P2 phát sinh khi ẩn `<br>` khiến chữ “thuyềnở” dính nhau đã giải quyết bằng khoảng trắng. Browser `inner_text` hiện đúng “Chiếc thuyền ở bến” ở cả100%/150%; ảnh cuối xác nhận cách xuống dòng rõ.
- Cả hai cỡ chữ không tràn ngang. Mở hành động bằng Enter, Escape đóng và trả focus về `preview-action` đạt. Kết quả session cuối tại `tests/results/sprint13-experiment02/critic-ux/final-review.json`.
- Mẫu contrast chữ trên nền đặc đã tính độc lập: muted/actioncard6.270:1, ink/dialog13.509:1, gold/dialog8.277:1. Không suy từ các mẫu này thành WCAG audit toàn diện, đặc biệt nền ảnh biến thiên.
- Full-art trước đó kiểm natural1672×941/rendered1200×675, giữ tỷ lệ và có scroll dialog; không crop nguồn như scene mặc định.

Không còn finding kỹ thuật chặn đã biết trong phạm vi đã kiểm. Mức độ đẹp, phù hợp mỹ thuật, bản sắc lịch sử và mong muốn bước vào cuộc đời vẫn cần phản hồi thật của chủ dự án. Không chuyển Sprint14 hoặc gọi Experiment02 đã nghiệm thu.
