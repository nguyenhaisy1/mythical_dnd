# Phản biện độc lập effort, phạm vi và lịch — ba hướng mỹ thuật

02/11/2026 · SPIN-036 · Actor `/root/art_feas_critic_estimate` · **Documentation review; Pending user approval**.

## Kết luận theo phạm vi đã kiểm

Mô hình sau đính chính có thể dùng để so sánh và xác định bước đo thử tiếp theo. Phần cộng thêm từ cảnh thử sang demo đã tính cả diện mạo, clip, UI art và bộ bến–thuyền; tổng effort và phép làm tròn ngày được tính lại độc lập. Không phát hiện P1 mới trong phần này sau sửa. Kết quả đó xác nhận tính nhất quán của dự toán, **không xác nhận ngân sách đủ, tốc độ Sol high hoặc ngày hoàn thành thực tế**.

Dự toán vẫn có độ tin cậy thấp, nhất là M3. Q4/8/12 là kịch bản tổng năng lực hữu dụng mỗi ngày của cả nhóm; các bảng chỉ quy đổi effort, chưa là lịch đã cân tài nguyên/đường phụ thuộc. Cần M0 hiệu chỉnh trước khi cam kết lịch. Mọi hướng và Sprint13 tiếp tục chờ chủ dự án chốt.

## Nguồn và giới hạn kiểm tra

Đã đọc [báo cáo tổng hợp](../ART-FEASIBILITY-THREE-DIRECTIONS.md), [01 — kiến trúc](01-ARCHITECTURE.md), [02 — pipeline mỹ thuật](02-ART-PIPELINE.md) và bản đính chính [03 — effort model](03-EFFORT-MODEL.md); đối chiếu `.agent`, HANDOFF, applicable baseline và execution contract. Đã đọc client README, Characters README và target frameworks trong các project core để kiểm giả định điểm xuất phát. Client vẫn là layout; `ProjectionState` của event consumer không tự là actor knowledge projection.

Phạm vi critic này là coverage công việc, phép cộng, mốc lũy kế, quy đổi ngày, đường phụ thuộc và giả định thuê bao. **Không mở ba PNG trực tiếp trong lượt review này**, không nhận xét độc lập mỹ thuật/camera quan sát từ ảnh. Những quan sát ảnh do tác giả01–03/critic05 thực hiện không được nhận thành quan sát của critic06. Thông số i5-8400/UHD630/khoảng19GiB được root kiểm kê, không phải phép đo GPU do critic này chạy.

Đã mở trực tiếp [official OpenAI pricing](https://learn.chatgpt.com/docs/pricing) ngày02/11/2026: số local messages cho Sol Plus trong năm giờ là khoảng ước tính thay đổi theo tác vụ; local/cloud dùng chung allowance, có thể có giới hạn tuần. Không có số giờ công/ngày được bảo đảm. Không mở dashboard quota tài khoản, không xác minh account entitlement hoặc benchmark cấu hình model. Không dùng API pricing để suy quota Plus.

## Findings và xử lý

| Mục | Bằng chứng và tác động | Xử lý / trạng thái |
|---|---|---|
| Thiếu coverage mỹ thuật M2 ở bản nháp trước | M1 có hai looks/3–4clip, M2 cần bốn looks/6–8clip và bối cảnh sửa thuyền. Chỉ cộng gameplay/client khiến art expansion không có ngân sách rõ. Root đã phát hiện trước lượt critic tiếp tục này. | **Đã sửa, kiểm lại:** common art28–60h và art Bến Vạc riêng18–40/16–36/22–48h. Không ghi critic06 là người phát hiện đầu tiên. |
| Ranh giới tránh đếm trùng | UI art khác Unity binding; asset/import/look QA khác gameplay E2E. Hai look bổ sung không là hai rig mới; tăng từ3–4 lên6–8clip là thêm2–5categories, còn compatibility checks trên hai rig là công QA. | **Đã làm rõ trong03**, không cộng lại baseline36–80h cho character M1. Bến/thuyền là increment/thay thế khoảng8–12definitions để tổng demo20–30, không tạo thêm20–30asset ngoài bộ cũ. |
| M3 chưa WBS chi tiết | Common120–240h và phần riêng chỉ là ROM gộp, chưa phân bổ rõ cho thêm body/look, clip, kit, hướng dẫn/validation. Không đủ để chốt task budget hoặc deadline M3. | **Unknown được ghi rõ trong03, không chặn brainstorming.** Trước triển khai/lịch cam kết phải chia gói incremental sauM2 và đo lại; không làm lại asset M2 bằng ngân sách M3. |
| Rủi ro hiểu bảng ngày là lịch giao hàng | `ceil(E/Q)` chưa biết năng lực lane tuần tự, dịch vụ chờ và ngày nào thật sự cóQ. | **Đã nêu giới hạn** trong03/main; chỉ dùng làm kịch bản. Không suy nhanh gấp sáu từ sáu vai hoặc bốn slot. |

Không coi khác biệt số tạm thời giữa main và03 trong lúc root đang đồng bộ là một lỗi sản phẩm cuối. Kiểm đồng bộ cuối được ghi ở phần evidence bên dưới.

## Tính lại WBS và tổng lũy kế

Đơn vị là giờ công AI hoạt động tổng hợp, chưa đo thực tế. Các đầu mút là ROM judgment; không phải percentile, khoảng tin cậy thống kê hoặc worst-case bảo đảm.

- Chung M1: `(6–12)+(8–16)+(36–80)+(10–18)+(8–16) = 68–142`.
- Gameplay/client cộng thêm M2: `(30–60)+(30–60)+(35–70)+(32–64)+(20–40)+(35–70) = 182–364`.
- Art mở rộng chung M2: `(6–12)+(12–28)+(4–8)+(6–12) = 28–60`; chung M2 mới là `210–424`.
- M2 mỗi hướng = M1 lũy kế + chung M2 + art Bến Vạc riêng + tích hợp/kiểm riêng. M3 mỗi hướng = M2 + `120–240` chung + riêngM3. Không cộng các tổng lũy kế M1/M2/M3 với nhau.

| Hướng | M1 riêng | M1 lũy kế | Bến Vạc M2 riêng | Kiểm/tích hợp M2 riêng | M2 lũy kế | M3 riêng | M3 lũy kế |
|---|---:|---:|---:|---:|---:|---:|---:|
| Đường chéo dài |18–40|86–182|18–40|6–16|**320–662**|16–40|**456–942**|
| Hành lang thành đất |28–62|96–204|16–36|8–20|**330–684**|24–48|**474–972**|
| Đất dựng thành |32–73|100–215|22–48|14–32|**346–719**|32–64|**498–1.023**|

Nếu làm cả ba M1 với common dùng lại hoàn toàn, phép cộng lý tưởng là `68–142 + 18–40 + 28–62 + 32–73 = 146–317h`, trước công ghép/cross-review bổ sung. Đây không phải dự toán hoàn chỉnh để làm cả ba; mức fidelity/camera khác nhau có thể phá giả định dùng chung. Tổng của ba phương án thay thế không là chi phí của một game.

## Tính lại quy đổi ngày

Mỗi ô là `ceil(low/Q)–ceil(high/Q)`. Cả27ô/54đầu mút sau đây đã tính bằng Python stdlib; M3 được tính bổ sung để đối chiếu con sốQ8 của main. Phép làm tròn là chính xác, đầu vào effort vẫn là ước lượng.

| Hướng/mốc | Q4 | Q8 | Q12 |
|---|---:|---:|---:|
| Đường chéo M1 |22–46|11–23|8–16|
| Hành lang M1 |24–51|12–26|8–17|
| Đất dựng thành M1 |25–54|13–27|9–18|
| Đường chéo M2 |80–166|40–83|27–56|
| Hành lang M2 |83–171|42–86|28–57|
| Đất dựng thành M2 |87–180|44–90|29–60|
| Đường chéo M3 |114–236|57–118|38–79|
| Hành lang M3 |119–243|60–122|40–81|
| Đất dựng thành M3 |125–256|63–128|42–86|

VớiC là effort trên đường tuần tự vàS là năng lực lane đó, khung `ceil(max(E/Q,C/S)) + phần chờ không chồng lấp` chỉ là lower-bound để tổ chức lịch. Chưa có task DAG/resource allocation định lượng; không gọi đây là lịch CPM đã nghiệm thu. Không có căn cứ đổi ngay các ô thành ngày tháng cụ thể hoặc tuần làm việc năm/bảy ngày. Rework nằm trong work packages nhưng retry/gate kéo dài có thể vượt đầu cao; không cộng thêm hệ số rework chung lần nữa, cũng không bỏ phần chờ ngoài.

## Assumptions, unknowns và risks

| Loại | Điều kiện phải giữ / bước xử lý |
|---|---|
| Assumption phạm vi | M1 là cảnh nhỏ có chuyển động và hai actor, Earth chỉ fragment. M2 chuyển grammar về Bến Vạc, tối đa5actor nhìn thấy, hai body/bốn looks/6–8clip/20–30definitions; không phải tái tạo nguyên panorama. M3 là bộ style có giới hạn, không toàn game. |
| Assumption tái sử dụng | Reuse core hiện có và usableM0/M1 artifacts. M0cap12–24h có thể hết khi asset gate chưa đạt; không cộng hai lần công M0 usable, phải dự toán lại failure/rework. |
| Unknown triển khai | Random-origin có nhân quả, actor projection, repair và Unity save/provider/client path còn việc thật. Budget phải có contract/acceptance cho profile giới hạn, không chuyển unknown thành chọn hero/scripted outcome. |
| Unknown năng lực | Q, usable-asset acceptance rate, số vòng retarget/contact, FPS/memory và lịch dịch vụ chưa đo. Tên gói/model không giải đáp các đại lượng này. |
| Risk chặn lịch | Asset source/credits/license/OAuth/job/import có thể chưa usable; một body không rig được có thể chặn mọi biến thể. Metadata tool và namespace visibility không là preflight. Không chuyển sang generator khác để giữ lịch. |
| Risk phối hợp | Scene/Library và asset import cần owner; song song có ích sau contract nhưng sửa cùng scene hoặc tranh tài nguyên có thể tăng công. Không sáu actor nhân sáu throughput. |
| Risk chất lượng | Corridor cận mặt/tay có thể đắt hơn diagonal dù ít geometry; Earth crowd/scale vượt fragment cần scope tăng. Mức văn hóa/provenance và target performance cần đo/duyệt riêng. |

## Evidence và disposition

- Đã chạy Python stdlib tính common/increments/tổngM1–M3 và54đầu mút `ceil`Q4/8/12; exit0. Bảng trong báo cáo này lưu kết quả, không tạo benchmark hoặc runtime test output.
- Sau khi root đồng bộ main, đã đọc lại các bảng/phần kết luận: M2Q8 là40–83/42–86/44–90ngày, M3Q8 là57–118/60–122/63–128ngày; main và03 khớp tổng mới. Bản03 được review có SHA256 `bf2ad02e90ee0042db95006b9182c6cc93a376cdd96888922baebcebcea2f712`; main lúc đối chiếu là `fe1909c2d265e34fbc18e719a5cb0b21ebd06cf4739ee2c396ac0283b92aa401`. Chỉnh sửa sau các hash này cần xác định phạm vi review lại.
- Check Python cho tổng trong03/main/06,27ô quy đổi ngày trong06, UTF-8/newline/trailing-whitespace và local Markdown links: exit0. Không chạy whole-repository validation hoặc docs export; root phụ trách integration checks.
- Đã dùng `rg`/đọc file để đối chiếu client layout, Characters placeholder, event consumer và target frameworks; đây là repository/documentation inspection, không chứng minh đầy đủ mọi gameplay module.
- Đã đọc official pricing trực tiếp; không đăng nhập hoặc kiểm quota tài khoản. Không đánh giá khả dụng từng model cho tài khoản thật.
- Không chạy Unity, tạo model/ảnh, Hyper3D preflight, code/gameplay test, GPU benchmark, playtest, Git hoặc sửa file ownership của worker/root. Không cập nhật acceptance matrix.
- Chỉ tạo file06 này. Root xử lý WORKLOG, mainreport và integration checks. Lượt cũ của critic bị usage limit vẫn là lịch sử đúng; lượt tiếp tục này mới có đầu ra độc lập thực tế.

**Disposition:** technical/documentation reviewed cho phần effort sau đính chính; chưa xác nhận throughput, lịch cam kết, runtime/art acceptance hoặc Final accepted. Không đóngSprint13 hay mởSprint14.
