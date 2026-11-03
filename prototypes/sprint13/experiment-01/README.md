# Experiment01 — Bến Vạc, buổi sáng trước mùa nước

**Chưa được chủ dự án nghiệm thu.** Đây là thử nghiệm hình ảnh/UX trong Sprint13, SPIN-016; không phải game Unity hoặc bản demo gameplay đã tích hợp.

Mở `index.html` bằng trình duyệt trên máy. Không cần dịch vụ mạng hoặc API key. Nếu dùng server local: chạy `python3 -m http.server 8013 --directory prototypes/sprint13/experiment-01`, rồi mở `http://localhost:8013`.

Cần xem và chốt: hình ảnh Bến Vạc; bố cục/thao tác; cách giới thiệu một nhân vật có xuất thân ngẫu nhiên. Các sản phẩm được giữ tại đây để thử và sửa, production không import. Luật hành động, WorldState, save/load thật và Unity adapter chưa nối.

Kiểm chứng tự động sẽ ghi trong báo cáo thử nghiệm; bot/browser automation không là human playtest. Chưa có phản hồi của chủ dự án.

Danh sách chốt: [APPROVALS](../../../docs/production/sprints/sprint-13/APPROVALS.md).

Kiểm tra mô hình: `node --test prototypes/sprint13/experiment-01/campaign.test.js`. Kiểm tra browser: `python3 prototypes/sprint13/experiment-01/browser-check.py` (cần Python Playwright và Google Chrome tại `/usr/bin/google-chrome`). Ảnh/evidence được ghi dưới `tests/results/sprint13-experiment01/`, không commit screenshots hoặc cache.
