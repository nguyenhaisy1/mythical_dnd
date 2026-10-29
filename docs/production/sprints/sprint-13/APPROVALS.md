# Sprint13 — Các mục cần chủ dự án chốt

Trạng thái chung: **Pending user approval**. Chưa có quyết định duyệt nào. AI không tự chuyển Approved dựa trên test/CI hoặc nhận xét của agent.

| ID | Mục cần chốt | Phương án hiện tại | Trạng thái | Quyết định thật |
|---|---|---|---|---|
| S13-A01 | Hướng hình ảnh, màu sắc và không khí Bến Vạc | Experiment01–06 → Experiment07 A/B/C (đồ họa và bối cảnh mới) | Changes requested; các phương án mới Pending | 27/10: user nói đồ họa bản01 chưa ưng ý, yêu cầu bản khác |
| S13-A02 | Bố cục và cách tương tác Reveal/World/Journal/Preview | Experiment01 browser UI | Pending | Chưa có |
| S13-A03 | Cách trình bày xuất thân ngẫu nhiên và lịch sử đời sống | Experiment01 mô hình giới hạn cho UX | Pending | Chưa có |
| S13-A04 | Chốt kết quả Sprint13 và phạm vi chuyển sang Sprint14 | Đợi sản phẩm/review và quyết định user | Pending | Chưa có |

Khi có phương án mới, thêm dòng/tham chiếu thay thế có lịch sử, không coi phương án cũ bị từ chối nếu user chưa nói. Khi user yêu cầu sang sprint mới, nhắc các mục Pending/Changes requested; chỉ chuyển khi chốt rõ. “Tiếp tục” cho phép làm tiếp thử nghiệm, không là nghiệm thu.

## Thử nghiệm02 — SPIN017

User đã yêu cầu đổi hướng hình ảnh của bản01; không suy thành từ chối UI hoặc origin. Bản01 giữ nguyên để so sánh. Bản02: tranh raster có chiều sâu và UI ưu tiên cảnh; tất cả hình ảnh/UI thay đổi vẫn chờ chốt. [Báo cáo02](EXPERIMENT-02-REPORT.md).

## Thử nghiệm03 — SPIN019

28/10/2026 user nói bản02 đẹp hơn nhiều nhưng màu có cảm giác AI dễ nhận ra, yêu cầu thử tiếp từ nhiều góc độ. Đây là **Changes requested về hình ảnh**, không phải Approved bản02. Bản03 có ba hướng A tự nhiên trời âm, B góc chéo gouache, C đồ họa nét khắc. Giữ01/02 làm tham chiếu; ba hướng mới đềuPending. [Báo cáo03](EXPERIMENT-03-REPORT.md).

## Thử nghiệm04 — SPIN020

User yêu cầu thử tiếp và có thể tạo view từ3D, đính kèm ảnh B bản03 làm tham chiếu. Chưa chốt B/A/C. Bản04 thử actual3D scene với nhiều camera, render và viewer; Pending. [Báo cáo04](EXPERIMENT-04-REPORT.md).

## Thử nghiệm05 — SPIN021

User giao tiếp tục, dùngMCPHyper3D theo quy tắc. ModelA có output thật nhưng lệch brief nhà thuyền; lượt sửa providererror không trảID và user chỉ thấy modelA trongMine. Không coi user xác nhận link modelA là approval. ModelA/ý tưởng nhà thuyền vẫnPending, brief xưởng sửa thuyền chưa đạt. [Báo cáo05](EXPERIMENT-05-REPORT.md).


## Thử nghiệm 06 — SPIN-022

29/10 user làm rõ mục tiêu: không muốn tạo model 3D thật ở bước này; yêu cầu ảnh giả lập game để xem trước, có thể 2D hoặc diện mạo 3D. Đây là **quyết định về cách thử nghiệm**, không phải duyệt hình ảnh/camera/màu hay kết quả Sprint 13. Vòng 06 thử ba hướng ảnh A góc chéo 2D, B giả lập 3D gần tầm người, C 2.5D nhìn ngang; ảnh tạo riêng, không cùng scene 3D. [Báo cáo 06](EXPERIMENT-06-REPORT.md). S13-A01–A04 vẫn Pending/Changes requested; không mở Sprint 14.


## Thử nghiệm 07 — SPIN-023

29/10 user yêu cầu “thử ý tưởng khác”, nhấn mạnh “thử cả đồ họa”. Vòng07 thử khắc gỗ/làng mùa nước, pixel art/xưởng đồng và giấy cắt/chợ hoặc đêm hội ven sông. Đây là yêu cầu tạo phương án mới, không phải approval bản06 hoặc quyết định canon/gameplay. Đổi cả phong cách và bối cảnh nên nhận xét cần phân biệt hai yếu tố; [báo cáo07](EXPERIMENT-07-REPORT.md) ghi evidence khi có. S13-A01–A04 giữ Pending/Changes requested;01–06 được giữ nguyên.
