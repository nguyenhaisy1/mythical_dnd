# Sprint13 — Các mục cần chủ dự án chốt

Trạng thái chung: **Pending user approval**. Chưa có quyết định duyệt nào. AI không tự chuyển Approved dựa trên test/CI hoặc nhận xét của agent.

| ID | Mục cần chốt | Phương án hiện tại | Trạng thái | Quyết định thật |
|---|---|---|---|---|
| S13-A01 | Hướng hình ảnh, màu sắc và không khí Bến Vạc | Thư viện tham khảo Experiment01–10, các phiên bản cùng được giữ | Changes requested; các phương án mới Pending | 27/10: user nói đồ họa bản01 chưa ưng ý, yêu cầu bản khác |
| S13-A02 | Bố cục và cách tương tác Reveal/World/Journal/Preview | Experiment01 browser UI; Experiment09–10 UI concept tổng thể | Pending | Chưa có |
| S13-A03 | Cách trình bày xuất thân ngẫu nhiên và lịch sử đời sống | Experiment01 mô hình giới hạn cho UX | Pending | Chưa có |
| S13-A04 | Chốt kết quả Sprint13 và phạm vi chuyển sang Sprint14 | Đợi sản phẩm/review và quyết định user | Pending | Chưa có |

Khi có phương án mới, thêm tham chiếu vào thư viện với lịch sử rõ; không mặc định thay thế, loại hoặc đánh giá kém phương án cũ nếu user chưa nói. Nhận xét/finding chỉ áp dụng đúng phiên bản và phạm vi đã quan sát. Khi user yêu cầu sang sprint mới, nhắc các mục Pending/Changes requested; chỉ chuyển khi chốt rõ. “Tiếp tục” cho phép làm tiếp thử nghiệm, không là nghiệm thu.

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

## Thử nghiệm08 — SPIN-024 và cách hiểu thư viện

29/10 user ghi nhận ý tưởng tốt dần, yêu cầu nghĩ thêm và làm rõ: các version để tham khảo; sang spin/thử nghiệm mới không có nghĩa bản cũ chắc chắn bị bỏ hoặc dở. Đây là chỉ dẫn giữ thư viện, **không phải final approval** cho A/B/C hoặc Sprint13. Phản hồi bản01 chưa ưng ý và các nhận xét lịch sử khác được giữ nguyên, không áp cho mọi phiên bản sau.

Vòng08 thêm ba hướng trên cùng cảnh nhà chữa trị bên suối đồi: A hoạt hình2D, B thế giới dệt và C ảnh diện mạo3D cách điệu. [Báo cáo08](EXPERIMENT-08-REPORT.md). Thư viện01–08 cùng tồn tại; chỉ ghi chọn/loại/thay thế khi có quyết định rõ. S13-A01–A04 vẫn Pending/Changes requested theo đúng phản hồi đã có; không chuyển Sprint14.

## Thử nghiệm09 — SPIN-025: ngôn ngữ hình ảnh toàn game

29/10 user yêu cầu ý tưởng táo bạo hơn, nhấn mạnh đồ họa xuyên suốt từ tạo hình nhân vật tới giao diện. Đây là mở rộng scope thử định hướng, không phải chọn/loại một phiên bản. Vòng09 thử bảng concept sơn mài, phù điêu đồng và mực đen/trắng+nét đỏ, gồm cảnh chơi, nhân vật/chân dung, vật phẩm/icon và UI trong ảnh. [Báo cáo09](EXPERIMENT-09-REPORT.md). UI raster chưa là UI chạy hoặc quyết định thay baseline random-origin. Thư viện01–09 được giữ; S13-A01–A04 chưa được chốt, không chuyểnSprint14.


## Thử nghiệm10 — SPIN-026: không cố định ba concept

29/10 user yêu cầu tiếp tục ý tưởng, cho AI tự quyết số lượng theo trí tưởng tượng thay vì luôn ba phương án A/B/C. Đây là quyền chọn phạm vi concept trong vòng được giao, không là final approval cho đồ họa hoặc Sprint13. Vòng10 chọn hai hướng Men chàm và Bóng rối, có thế giới/cast/vật phẩm/UI trong ảnh; [báo cáo10](EXPERIMENT-10-REPORT.md) ghi evidence thật khi có.

Thư viện01–10 cùng được giữ. Hồ sơ09 đã được hai critic mới kiểm lại và viết report riêng sau gián đoạn. Review09/10 là bằng chứng kỹ thuật/hình ảnh, không tự duyệt hay loại một phiên bản. S13-A01–A04 vẫn Pending/Changes requested theo phản hồi thật, không chuyểnSprint14.
