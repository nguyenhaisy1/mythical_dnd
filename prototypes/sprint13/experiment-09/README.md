# Thử nghiệm09 — Đồ họa từ thế giới tới nhân vật và UI

Sprint13 · SPIN-025 · 29/10/2026 · **Tham khảo, chưa chốt**.

Ba hướng táo bạo được thử như một diện mạo xuyên suốt game: **A Sơn mài sống**, **B Phù điêu đồng**, **C Mực sống**. Mỗi bảng ảnh cần có cảnh chơi giả lập, nhân vật toàn thân/chân dung, vật phẩm và UI cùng ngôn ngữ; không chỉ thêm phong cảnh. [Art direction](ART-DIRECTION.md) ghi ý định và câu hỏi review, chưa thay một art bible hoặc pipeline production đã được nghiệm thu.

## Xem các phiên bản

Mở `index.html` khi gallery và ảnh đã có, hoặc dùng `http://127.0.0.1:8016/experiment-09/` nếu server xem trước đang chạy. Từ root repository có thể chạy:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Root lưu ba output tại `assets/a-lacquer.png`, `assets/b-bronze.png`, `assets/c-ink.png`, cùng prompt và provenance sau generation. Các đường dẫn này mô tả sản phẩm được giao; chưa tự chứng minh ảnh đã tạo hoặc được kiểm. Gallery là khung xem trung tính; UI được đánh giá về phong cách nằm trong từng bảng ảnh, không nhận cùng một HUD HTML làm bằng chứng ba hướng UI khác nhau.

## Phạm vi

Đây là ảnh raster do built-inimage_gen tạo để xem ý tưởng, không phải game chạy, model3D, render từ Unity hoặc UI tương tác production. Không gọi Hyper3D/Blender cho vòng này. Chữ và icon trong bitmap có thể chưa chính xác; việc xem được ảnh không chứng minh typography/accessibility, navigation, knowledge isolation hoặc gameplay. Các ý định chuyển động trong brief chưa có animation thực tế.

Cảnh cộng đồng mùa nước, nhân vật và các mẫu hội thoại/túi đồ/nhật ký là hư cấu mỹ thuật. Không chốt canon, rule, nội dung chữa trị hoặc phục dựng khảo cổ; không cho người chơi lựa chọn lại origin hay tạo nhân vật thủ công. Các hành động trong UI ảnh chỉ minh họa cách trình bày.

Các vòng01–08 và hồ sơ nguồn được giữ nguyên để tham khảo, quay lại hoặc kết hợp. Phiên bản mới không mặc định thay thế, loại hoặc đánh giá thấp bản trước. [Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) vẫn giữ quyền quyết định của chủ dự án; kỹ thuật/review không là final approval hoặc cho phép chuyển Sprint14.

## Ảnh và nguồn

Ba board1672×941RGB đã lưu trong assets; [prompts](prompts.json) là prompt đầy đủ đã dùng với built-inimage_gen, [provenance](provenance.json) ghi SHA256/kích thước/output gốc. PNG sao chép nguyên bytes, không hậu kỳ. Mở ảnh gốc từ modal để zoom; chữ150% trên gallery chỉ đổi textHTML, không reflow chữUI trong bitmap. Các nút/vật phẩm trong ảnh chưa có tương tác game.

## Kiểm trang xem ảnh

`python3 prototypes/sprint13/experiment-09/browser-check.py` dùng Python Playwright/Pillow và Chrome tại `/usr/bin/google-chrome`. Code critic đã chạy đạt desktop/mobile, provenance/ảnh gốc, nút/phím/modal/focus/linkPNG và lịch sử; chữ150% chỉ áp dụng HTML. Evidence `tests/results/sprint13-experiment09/` bị ignore. Không dùng browser pass để chốt UI game hoặc mỹ thuật.
