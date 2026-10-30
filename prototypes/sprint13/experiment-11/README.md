# Thử nghiệm11 — 2.5D, 3D, pixel và anime

Sprint13 · SPIN-027 · Brief bắt đầu29/10/2026, tiếp tục thực hiện30/10/2026 · **Tham khảo, chưa chốt**.

Bốn bảng concept thử cùng tình huống chợ lương thực/kho cao/cầu tre trước mùa nước, với một phụ nữ lao động trưởng thành làm mẫu nhân vật chính. Mỗi bảng cần nối cảnh chơi với toàn thân, chân dung, vật phẩm và UI hội thoại/túi đồ/nhật ký. [Art direction](ART-DIRECTION.md) ghi giả thuyết, quy tắc và câu hỏi review; chưa tự chứng minh output đạt.

2.5D/3D là cách tổ chức không gian; pixel/anime là ngôn ngữ tạo hình và có thể kết hợp với chúng. Các bảng là những tổ hợp tham khảo, không phải bốn phương án loại trừ nhau hoặc một quyết định đổi techstack.

## Xem sản phẩm

Mở `index.html` để xem bốn board đã được tạo ngày30/10. Nếu server xem trước đang chạy, dùng `http://127.0.0.1:8016/experiment-11/`. Từ root repository có thể chạy:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Các đường dẫn ảnh được giao cho vòng này:

- `assets/layered-25d.png` — 2.5D nhiều lớp.
- `assets/stylized-3d.png` — diện mạo3D cách điệu.
- `assets/pixel-life.png` — pixel đời làng.
- `assets/anime-life.png` — anime đời thường.

Root dùng built-inimage_gen tạo raster và lưu prompt/provenance sau output thật. Đường dẫn trong brief không tự là bằng chứng ảnh đã tạo hoặc được kiểm. Gallery chỉ đổi/xem/phóng lớn ảnh; UI riêng của từng hướng nằm trong board, không bị thay bằng một HUDHTML chung.

Mở concept toàn khung hoặc PNG gốc để xem chữ, icon và chân dung ở kích thước lớn. Cỡ chữ của trang xem chỉ áp dụng textHTML; không phóng/reflow chữUI nằm trong ảnh.

## Kiểm trang xem ảnh

Khi root đã tạo checker và đủ bốn ảnh, chạy từ root repository:

```bash
python3 prototypes/sprint13/experiment-11/browser-check.py
```

Checker dự kiến dùng Python Playwright/Pillow và Chrome tại `/usr/bin/google-chrome`; evidence ghi trong `tests/results/sprint13-experiment11/`, không commit. Đây là lệnh kiểm gallery/provenance, chưa là kết quả đã chạy. Root và critic ghi kết quả thật sau khi artifact hoàn chỉnh; browser pass không thay review đồ họa, UI game hoặc human playtest.

## Phạm vi và thư viện

Đây là ảnh để xem trước ý tưởng. Không dựng model thật, gọi Hyper3D/Blender, chuyển engine hoặc triển khai gameplay/UI. Chữ trong bitmap có thể nhỏ/sai ký tự và không reflow theo cỡ chữHTML; nút được vẽ chưa có tương tác game. Cần mở PNG gốc để xem kỹ, rồi kiểm font/layout/trạng thái khi làm UI thật.

Ảnh tĩnh chưa chứng minh parallax, camera tự do, rig/animation, pixel grid/palette production, cost hoặc performance. Các ảnh tạo độc lập có thể khác camera, tỷ lệ, cast và chi tiết dù dùng cùng brief. Không kết luận sở thích chỉ do một yếu tố.

Phụ nữ khoảng45tuổi, người giữ kho và hàng xóm là cast minh họa; không thêm character creator chọn tuổi/giới/nghề/origin. Trang phục, bố trí và lời thoại chưa là canon hoặc bằng chứng lịch sử. Giữ toàn bộ vòng01–10 để tham khảo, quay lại hoặc phối hợp; thử nghiệm11 không tự loại hay chê bản trước. [Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền quyết định của chủ dự án; không dùng kiểm gallery để nghiệmthu mỹ thuật hoặc chuyển Sprint14.

## Nguồn ảnh thực tế

Bốn ảnh gốc đã lưu trong assets; [prompts](prompts.json) là bộ prompt chuẩn bị29/10, [provenance](provenance.json) ghi ngày tạo30/10 cùng kích thước/SHA256/bytes. Công cụ built-inimage_gen, PNG sao chép nguyên bytes, không hậu kỳ/model3D. UI/calendar/minimap trong ảnh chỉ minh họa, không thêm luật hoặc dữ liệu runtime.

## Kết quả kiểm gallery

Code critic đã chạy checker11 đạt Chrome156 ở1280×720/1920×1080/390×844, gồm chữHTML150%, decode/provenance/ảnh gốc, phím/modal/focus/linkPNG và mười lịch sử. Evidence `tests/results/sprint13-experiment11/` bị ignore. Đây là kiểm trang xem ảnh; không là nghiệmthu UI game, nativepixelgrid hoặc kỹ thuật3D/parallax. Xem [báo cáo](../../../docs/production/sprints/sprint-13/EXPERIMENT-11-REPORT.md) cùng hai review để biết các đánh đổi của từng board.
