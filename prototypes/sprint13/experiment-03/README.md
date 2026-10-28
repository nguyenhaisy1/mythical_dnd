# Thử nghiệm03 — Ba hướng nhìn Bến Vạc

**Pending user approval.** Sprint13, SPIN-019. Bản so sánh hình ảnh đáp lại nhận xét của chủ dự án: bản02 đẹp hơn nhưng màu còn tạo cảm giác AI. Ba phương án thay cả ánh sáng, góc nhìn và cách thể hiện, không chỉ dùng bộ lọc màu trên cùng một ảnh.

- **A — Ánh sáng âm, ngang tầm người:** màu vật liệu và sinh hoạt bến sông trong ánh sáng trung tính.
- **B — Gouache, góc chéo từ cao:** xem cấu trúc không gian bằng ngôn ngữ tranh màu khoáng.
- **C — Đồ họa tranh khắc:** bảng màu giới hạn, ưu tiên dấu nét và tương phản hình khối.

Mở `index.html` bằng Chrome. Có thể chạy `python3 -m http.server 8013 --directory prototypes/sprint13` rồi mở `http://localhost:8013/experiment-03/`; cách serve này giữ liên kết đến [bản02](../experiment-02/index.html) hoạt động. Không cần tài khoản hay API key để xem artifact đã tạo.

Đây là gallery chọn hướng mỹ thuật, không bổ sung gameplay hoặc Unity scene. Các ảnh là concept hư cấu của Bến Vạc, chưa kiểm chứng khảo cổ hoặc asset production. Đánh giá màu và tính hấp dẫn cần chủ dự án xem; browser checks không xác định hình ảnh có còn mang cảm giác AI hay không.

Kiểm tra giao diện: `python3 prototypes/sprint13/experiment-03/browser-check.py` (Python Playwright và Google Chrome tại `/usr/bin/google-chrome`). Script kiểm ảnh nạp được, chuyển phương án, so sánh, toàn cảnh, Escape/focus, chữ150% tại1280/1920 và liên kết HTTP. Evidence/screenshots local nằm `tests/results/sprint13-experiment03/`; không là human playtest.

Nguồn tạo ảnh và prompt đầy đủ tại [GENERATION](assets/GENERATION.md); ba ảnh do root tạo bằng built-in image_gen. Không dùng giấy phép MIT của các skill cho ảnh generated. Các sản phẩm01/02 được giữ để đối chiếu. [Các mục cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) vẫn do chủ dự án quyết định.
