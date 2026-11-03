# Lộ trình hình ảnh và bản chơi thử

## Trạng thái hiện hành — chốt ngày 03/11/2026

Chủ dự án đã chốt Sprint13 theo **Đất dựng thành: nền 2D có cảm giác khối + nhân vật sprite**, profile `earth-monument-2d-v1`. Đây là chốt định hướng để phát triển; chuyển động, asset production, UI chi tiết và bản game chạy thật vẫn cần acceptance riêng. [Baseline mỹ thuật và cổng G13](sprints/sprint-13/ART-DIRECTION-BASELINE.md), [ADR-022](../adr/022-earth-monument-2d-and-reopen-gate.md) và [approval register](sprints/sprint-13/APPROVALS.md) là nguồn hiện hành.

**G13 = Accepted / Reopenable:** nếu phát triển thấy chưa phù hợp, mở lại thử nghiệm mỹ thuật bằng profile/candidate mới, giữ hướng v1 và thư viện01–18 để đối chiếu. Thay đổi art, sprite, camera hoặc presenter phải bảo toàn core, luật, stable IDs, content và save theo contract; không quay repository về Sprint1 hoặc bỏ các phần đã làm ở sprint sau. Đổi schema/gameplay thật sự cần đánh giá tác động và migration riêng.

| Mốc tiếp theo khi được giao | Kết quả cần kiểm | Phần còn mở |
|---|---|---|
| Hiệu chỉnh đường 2D | Một phần cảnh có lớp nền/che khuất, nhân vật sprite, điểm đặt chân và tương tác nhỏ; đo công tạo asset và sửa thực tế | Đường đi, surface/tầng, sắp lớp, camera cố định theo cảnh và giới hạn pan/zoom phải có contract; chưa có bằng chứng chạy |
| UI và xuất thân — S13-A02/A03 | Luồng Reveal/World/Journal/Preview đọc được và thông tin đúng nhân vật; nhận xuất thân ngẫu nhiên có nhân quả | UI pixel/layout, usability và generator implementation chưa được nghiệm thu; giữ no origin editing/no Reveal reroll |
| Vòng sửa thuyền/demo nhỏ | Áp ngôn ngữ mỹ thuật đã chọn vào Bến Vạc, Tam Giang; luật vật liệu/thời gian/quyền sở hữu, projection và save/load thật | Cảnh công trường tham chiếu không dời Bến Vạc hoặc thay nghề sửa thuyền thành xây thành; gates gameplay/Unity vẫn riêng |

Yêu cầu chốt Sprint13 **không tự mở Sprint14**. Mốc demo Sprint15–16 dưới đây là forecast cũ, chưa có thời lượng sprint cố định hoặc ngày giao hàng được cam kết. Trước khi chốt lịch triển khai phải chia lại công việc cho đường **2D/sprite**, đo thử và cập nhật nguồn lực; các khoảng **86–215h art proof / 320–719h demo của nghiên cứu 3D không áp dụng nguyên sang hướng này**. Không suy mức tiết kiệm phần trăm hay deadline mới khi chưa hiệu chỉnh. Phần luật game, projection và persistence không tự biến mất khi đổi render route.

## Lịch sử forecast trước quyết định 03/11/2026

Các mục từ đây giữ dự báo và trạng thái tại lúc viết, không thay quyết định hiện hành ở trên. Nội dung kế hoạch từng gắn generator/client với Sprint13 không được dùng để suy chúng đã hoàn tất khi chốt mỹ thuật.

Cập nhật 2026-10-26 khi giao Sprint12: lần đầu chuyển roadmap milestone sang các mốc sprint dự kiến. Khi đó Sprint13 đang thử nghiệm và chờ chủ dự án chốt; các sprint sau chỉ được mở khi chủ dự án giao theo workflow thường trực.

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


## Nghiên cứu lịch sử cho đường 3D — 02/11/2026

[Báo cáo SPIN-036](sprints/sprint-13/ART-FEASIBILITY-THREE-DIRECTIONS.md) đề xuất route3D cách điệu/camera giới hạn cho ba mẫu14/17/18; đây chưa là quyết định thay baseline hay chốtSprint13. M1artproof khoảng86–215giờ công AI tùy hướng, M2demo nhỏ320–719giờ lũy kế, theo scope cụ thể trong báo cáo; Earth chỉfragment, M2áp grammar vào BếnVạc với bến/thuyền trong assetbudget. Q4/8/12 chỉ giả định tổng năng lực hữu dụng của nhóm, chưa đo trênPlus/Solhigh; bảng ngày chưa cộng phụ thuộc/chờ. Không thay forecastSprint15–16 thành ngày hứa hoặc tự mởSprint14.

Trước khi chốt lịch cần M0cap12–24giờ công kiểm đường asset/rig/import/camera và ghi active/wait/retry thật; cap có thể hết mà chưa usableasset. Việc này mới được đề xuất, chưa giao triển khai. Client/scenes/Player persistence, generator có nhân quả, actor projection và repair vẫn có acceptance riêng; hình ảnh đẹp không thay các gate đó.
