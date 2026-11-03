# Thử nghiệm 02 — Bến Vạc, hướng mỹ thuật hội họa

**Pending user approval — chưa được chủ dự án nghiệm thu.** Sprint 13, SPIN-017. Phương án mới đáp lại phản hồi chưa ưng ý đồ họa Thử nghiệm 01. Giữ nguyên [Thử nghiệm 01](../experiment-01/index.html) để so sánh.

Mở `index.html` bằng Chrome trên máy. Hoặc chạy `python3 -m http.server 8013 --directory prototypes/sprint13` rồi mở `http://localhost:8013/experiment-02/`. Các tệp dùng tại chỗ, không cần API key. Cảnh raster ở `assets/ben-vac-painterly.png`; đây là minh họa mỹ thuật thử nghiệm, không xác nhận tính chính xác khảo cổ hay canon mới.

Phạm vi: cảnh Bến Vạc, bố cục màn hình, giới thiệu nhân vật ngẫu nhiên với kỹ năng có nguồn gốc trải nghiệm, nhật ký ký ức của nhân vật và xem trước thao tác. Mô hình nhân vật và các kiểm thử được kế thừa nguyên từ Thử nghiệm 01; phương án này tập trung vào hướng hình ảnh khác. Chưa nối WorldState, luật hành động, lưu/tải hay Unity. Xác nhận thao tác chỉ trình diễn UX, không tiêu vật liệu hay thời gian game. Không dùng ảnh minh họa để khẳng định sự kiện hoặc thông tin nhân vật đã biết.

Kiểm tra tự động:

- `node --test prototypes/sprint13/experiment-02/campaign.test.js`
- `python3 prototypes/sprint13/experiment-02/browser-check.py` (Python Playwright và Google Chrome tại `/usr/bin/google-chrome`).

Browser check kiểm tra hai khổ 1280×720 và 1920×1080, luồng nhận nhân vật, nguồn gốc kỹ năng, nhật ký, modal/Escape/focus, chữ 150%, ảnh nạp được và liên kết đối chiếu Thử nghiệm 01. Ảnh chụp và JSON kết quả ghi vào `tests/results/sprint13-experiment02/`, không commit cache hoặc evidence ảnh. Tự động hóa không thay đánh giá thẩm mỹ, human playtest hay chốt của chủ dự án; kết quả chạy thật nằm trong báo cáo thử nghiệm.

Cần chủ dự án chốt riêng hướng hình ảnh, giao diện và cách giới thiệu nhân vật; xem [APPROVALS](../../../docs/production/sprints/sprint-13/APPROVALS.md). Sprint 13 vẫn mở cho đến khi các mục được chốt rõ.

Nguồn ảnh và prompt đầy đủ: [GENERATION](assets/GENERATION.md). Ảnh tạo bằng built-in image_gen, không dùng CLI/API key.
