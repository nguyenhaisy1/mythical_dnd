# Lộ trình hình ảnh và bản chơi thử

Cập nhật 2026-10-26 khi giao Sprint12. Đây là **forecast mới**, chuyển roadmap milestone sang các mốc sprint dự kiến; tài liệu trước đó chưa cam kết số sprint cho hình ảnh/demo. Sprint13 đã được giao theo chế độ thử nghiệm, chỉ chủ dự án được chốt nghiệm thu. Các sprint sau được mở khi chủ dự án giao theo workflow thường trực.

| Sprint dự kiến | Kết quả nhìn thấy được | Điều kiện nghiệm thu |
|---|---|---|
| 12 | Bộ tám tình huống và bộ kiểm tra điều kiện xuất hiện | Dữ liệu hợp lệ, điều kiện nhân quả và hiểu biết tách biệt; không coi candidate là sự kiện đã xảy ra |
| 13 | Định hướng hình ảnh Bến Vạc, bảng màu/bố cục, giao diện mẫu; khởi tạo vòng đời campaign và nhân vật ngẫu nhiên trong phạm vi demo | Chốt hướng 2D/2.5D prototype và quyền dùng asset; Reveal không chỉnh origin/reroll; mẫu hình ảnh chưa là gameplay |
| 14 | Vòng chơi sửa thuyền chạy bằng luật thật, có thời gian, vật liệu, quyền sử dụng và hậu quả | Command/atomic commit, hủy/lỗi/tài nguyên thiếu và save/reload được kiểm; không dùng text để giả kết quả |
| 15–16 | **Demo chơi thử nhỏ trong Unity** tại Bến Vạc: nhận nhân vật → quan sát → chọn sửa thuyền hoặc chờ → thấy hậu quả → lưu/tải | Scene/UI thật, thông tin đúng nhân vật, input/feedback, bản chạy trên máy hiện tại và kiểm chứng end-to-end |
| Sau demo nhỏ | Mở rộng tới vertical slice “Bến qua mùa nước” đầy đủ | VS03–06/08–09 và VS07/10 còn gates riêng; không cam kết số sprint khi chưa đo khối lượng tích hợp |

## Hình ảnh được làm ở mức nào?

Sprint13 dự kiến bắt đầu **thiết kế mỹ thuật và giao diện prototype**: cảnh bến sông, nhân vật minh họa, hệ màu, chữ và trạng thái tương tác. Không cần đợi tất cả hệ simulation hoàn thành mới thử bố cục. Asset dùng để kiểm đọc và trải nghiệm; chưa sản xuất hàng loạt toàn bộ nhân vật, vùng đất hay animation cuối cùng.

Sản xuất mỹ thuật đầy đủ chỉ mở sau khi vòng chơi đã chạy và hướng hình ảnh được đánh giá. Thực hiện theo [Client UX](CLIENT-UX-BASELINE.md) và [Unity Bootstrap](../runbooks/UNITY-BOOTSTRAP.md); bằng chứng Unity conformance của Sprint10 chưa phải client chơi được.

## Demo nhỏ khác vertical slice đầy đủ

Demo nhỏ ưu tiên một vòng chơi có lựa chọn và hậu quả thật, với phạm vi cảnh và hành động hạn chế. Chưa hứa đủ ba nghề, combat, companion, succession, lịch sử, ecology hoặc NPC autonomy toàn diện. Nội dung chưa triển khai phải được giới hạn rõ trong UI; không dùng nút giả hoặc kết quả scripted thay luật rồi ghi nghiệm thu gameplay.

VS07 đầy đủ vẫn phụ thuộc VS03/04/05 theo [Implementation Plan](IMPLEMENTATION-PLAN.md). Demo sớm là client prototype giới hạn được phép thử trước milestone đó, không đổi các acceptance của VS07 hay VS10.

## Khi nào phải điều chỉnh mốc?

Mục tiêu là **bắt đầu hình ảnh Sprint13; chơi thử nhỏ Sprint15–16**, không là bảo đảm phát hành. Sprint13 có thể gồm nhiều spin thử nghiệm; không chuyển Sprint14 khi [các mục cần chốt](sprints/sprint-13/APPROVALS.md) chưa được chủ dự án duyệt. Vì vậy forecast demo phải xét lại sau khi chốt Sprint13. Cuối mỗi sprint đối chiếu: generator/viability, luật repair và save, actor-scoped projection, Unity scene/input/build. Nếu một gate chưa đạt, ghi nguyên nhân và forecast mới trong sprint report trước khi chuyển mốc; không âm thầm đổi định nghĩa “demo” thành ảnh hoặc video. Chưa quy đổi sang ngày vì chưa có thời lượng sprint cố định và còn rủi ro tích hợp.
