# Thử nghiệm06 — Xem trước hình ảnh game

Sprint13 · SPIN-022 · 29/10/2026 · **Chờ chủ dự án chốt**.

Vòng này dùng ảnh để diễn tả game có thể trông như thế nào trước khi làm asset. Có ba cách nhìn cùng cảnh sửa thuyền ở Bến Vạc: A tranh2D góc chéo; B ảnh giả lập3D gần nhân vật; C sân khấu2.5D nhìn ngang. B là ảnh raster có cảm giác3D, không phải model hoặc cảnh3D tương tác.

## Xem sản phẩm

Mở `index.html` trong trình duyệt để so sánh các phương án. Ảnh được lưu trong project, không phụ thuộc đường dẫn ảnh riêng của công cụ:

- `assets/a-oblique.png` — hướngA.
- `assets/b-grounded.png` — hướngB.
- `assets/c-sideview.png` — hướngC.

Nếu dùngHTTP, từ root repository chạy:

```bash
python3 -m http.server 8013 --directory prototypes/sprint13
```

Sau đó mở `http://localhost:8013/experiment-06/`. Các thao tác trên gallery chỉ phục vụ xem ảnh và lớpUI minh họa; không thực hiện hành động trong thế giới game. Bỏ chọn “Hiện giao diện minh họa” để xem tranh không có lớp chữ; nút xem toàn khung cũng chỉ hiện ảnh gốc. [Prompts](prompts.json) lưu đầy đủ ba prompt tiếng Anh đã dùng; [provenance](provenance.json) ghi công cụ built-in image_gen, kích thước và SHA256 của từng ảnh. Ảnh được sao chép nguyên bytes, không chỉnh màu hoặc cắt ghép sau tạo.

## Phạm vi

Ảnh được tạo bằng built-inimagegen. Vòng06 không gọiHyper3D, không dựngBlender và không tạo model thật. Ba ảnh là ba bản concept độc lập, không có cam kết dùng chung geometry, nhân vật hoặc kích thước đồ vật. Không suy ảnh có chiều sâu thành bằng chứng renderer3D, camera tự do hoặc khả năng tương tác.

[Art direction](ART-DIRECTION.md) nêu câu hỏi và tiêu chí so sánh. Chưa có humanplaytest, Unity/gameplay acceptance hoặc kiểm chứng khảo cổ. Mọi kết quả vẫn chờ quyết định theo [danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md); không dùng review kỹ thuật để tự duyệt mỹ thuật. Các bản01–05 được giữ nguyên.

## Kiểm chứng

`python3 prototypes/sprint13/experiment-06/browser-check.py` kiểm provenance và gallery bằng Python Playwright/Pillow cùng Chrome tại `/usr/bin/google-chrome`. Critic code đã chạy đạt ba viewport desktop/mobile, đổi ảnh, phím tắt, modal/focus, bật/tắt HUD và chữ150%. Evidence nằm `tests/results/sprint13-experiment06/`, bị ignore. Đây là kiểm trang xem ảnh, không là kiểm gameplay hoặc đánh giá mỹ thuật. Xem [báo cáo](../../../docs/production/sprints/sprint-13/EXPERIMENT-06-REPORT.md) cùng hai review để biết các điểm mỹ thuật còn mở.
