# Thử nghiệm10 — Men chàm và Bóng rối

Sprint13 · SPIN-026 · 29/10/2026 · **Tham khảo, chưa chốt**.

Hai hướng hình ảnh được đào sâu thay vì cố định ba concept: **Men chàm** thử thế giới gốm trắng ngà/xanh cobalt; **Bóng rối** thử silhouette xuyên sáng và lớp sân khấu. Cả hai cần nối cảnh chơi với tạo hình nhân vật, chân dung, vật phẩm và UI hội thoại/túi đồ/nhật ký. [Art direction](ART-DIRECTION.md) ghi ý định, ma trận nhất quán và câu hỏi review; không tự chứng minh output đã đạt.

## Xem sản phẩm

Mở `index.html` khi gallery/ảnh đã được tạo, hoặc dùng `http://127.0.0.1:8016/experiment-10/` nếu server xem trước đang chạy. Từ root repository:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Ảnh được giao tại `assets/men-cham.png` và `assets/bong-roi.png`; root lưu prompt/provenance sau generation thật. Đường dẫn trong brief chưa tự là bằng chứng ảnh đã có hoặc được kiểm. Gallery trung tính chỉ phục vụ đổi/xem ảnh, không phủ cùng một HUD để thay phần UI riêng trong từng bảng.

## Phạm vi

Đây là ảnh raster tạo bằng built-inimage_gen để xem ý tưởng; không tạo model3D, không gọi Hyper3D/Blender và không triển khai UI/gameplay. Chữ, biểu tượng và lựa chọn trong bitmap là minh họa, có thể chưa chính xác; không reflow như textHTML hoặc có tương tác game. Cần xem ảnh gốc và kiểm lại typography/trạng thái khi làm UI thật.

Các ẩn dụ gốm men và bóng rối không là khẳng định truyền thống lịch sử, phục dựng khảo cổ hoặc canon mới. Cảnh sinh hoạt và cast là hư cấu mỹ thuật; không cho người chơi tạo/chỉnh origin thủ công. Giả thuyết chuyển động trong brief chưa có animation hoặc runtime evidence, chưa có đo đạc cost/performance.

Giữ mọi vòng01–09 và nguồn cũ để tham khảo, quay lại hoặc kết hợp. Phiên bản10 bổ sung thư viện, không thay thế, loại hoặc đánh giá thấp bản trước. Kết quả review phải dựa trên artifact thực; root bổ sung ảnh, provenance và evidence sau khi có. [Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền quyết định của chủ dự án; không dùng kiểm gallery để nghiệmthu mỹ thuật hoặc chuyển Sprint14.

## Nguồn thực tế

[Prompts](prompts.json) giữ đầy đủ hai prompt cho built-inimage_gen; [provenance](provenance.json) ghi PNG gốc1672×941, SHA256/bytes và không hậu kỳ. Hai ảnh được sao chép nguyên bytes vào assets. Mở ảnh gốc từ modal để zoom UI; chữ150% chỉ áp dụng textHTML, không reflow chữ trong PNG. Tên/lời thoại trong board là minh họa, không thêm canon hoặc gameplay.

## Kiểm trang xem ảnh

`python3 prototypes/sprint13/experiment-10/browser-check.py` dùng Python Playwright/Pillow và Chrome tại `/usr/bin/google-chrome`. Code critic đã chạy đạt desktop/mobile, provenance/ảnh gốc, nút/phím/modal/focus/linkPNG và lịch sử; chữ150% chỉ áp dụng HTML. Evidence `tests/results/sprint13-experiment10/` bị ignore. Không dùng browser pass để chốt UI game hoặc mỹ thuật.
