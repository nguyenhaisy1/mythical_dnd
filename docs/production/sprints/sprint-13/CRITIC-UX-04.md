# Phản biện hình ảnh04 — diễn biến và review độc lập bổ sung

Trạng thái hiện tại: đã có phần review độc lập của /root/review ở cuối tài liệu; các đoạn đầu giữ sự kiện quota và root review trước đó.

28/10/2026. Đã giao `/root/s13_critic_ux`, nhưng tác nhân chạm usage quota trước khi trả kết quả review. **Chưa có nghiệm thu độc lập visual/UX của bản04.** Không dùng nhận xét của root để thay tên người review.

## Kiểm tra lớp đầu của root

Root đã xem render-overview và render-eye thật, cùng ảnh viewer trong lượt browser check. Hình học có mái nhà ghép lớp, ván/cọc bến, thuyền lòng mở và hình người cùng scene. Camera cao thuận lợi đọc mặt bằng; camera thấp lộ rõ tỷ lệ và chi tiết ở khoảng cách gần.

Giới hạn nhìn thấy: phong cách diorama khá giản lược, người chưa có mặt/rig, bề mặt GLB phẳng hơn render Blender do noise/bump chưa bake; camera thấp ban đầu khá chật và có người cắt ở mép; root đã đổi vị trí/lens, render lại để mở rộng góc nhìn. Cảnh vẫn mang nền studio trống ở phía xa. Đây là lỗi/đánh đổi cần đánh giá thêm, không kết luận đã đạt mỹ thuật theo ảnh B03. Không khẳng định chính xác lịch sử, chất lượng gameplay hoặc human playtest.

Browser tự động của root đã chạy hai viewport, xem model3D thật và ảnh render, keyboard/orbit/zoom/preset/wireframe/modal/text150. Điều này chứng minh khả năng xem/tương tác trong môi trường kiểm; không thay đánh giá thẩm mỹ độc lập hoặc chốt của user.

## Trạng thái

Bản04 có thể đưa user xem để góp ý với ghi chú review còn thiếu. Giữ draftPR, không merge dev/đóngSprint13. Lượt critic visual độc lập cần được nối lại khi quota khả dụng; chưa ghi Passed cho gate đó.

## Review độc lập bổ sung — /root/review — 28/10/2026

Tác nhân `/root/review` được giao thay lượt `/root/s13_critic_ux` đã chạm quota; không kế thừa một kết quả review chưa chạy. Đã áp dụng mythical-validation, đọc art direction/README/UI source và thực sự xem bằng view_image: ba ảnh `render-overview.png`, `render-eye.png`, `render-repair.png`; ảnh tham chiếu `experiment-03/assets/b-gouache.png`; browser screenshots `full-repair-1920.png`, `view-eye-1280.png`, `view-repair-1920.png`. Đối chiếu browser.json hiện có (GLB SHA256 ca94bd77b5377223ae38ad0facc42de7733bd250f0fbf61624e0f8995212a25d). Reviewer không chạy browser tương tác riêng; không nhận kết quả automation của root làm lượt test của mình.

### Kết quả nhìn trực tiếp

- Góc cao đọc được quan hệ nhà/cầu thang/bờ/thuyền sửa/bến/thuyền nước. Ba góc giữ vị trí và hình dạng đồ vật nhất quán; camera gần cho thấy sườn và lòng thuyền, đủ để đánh giá cấu trúc không gian của vòng thử.
- UI có ba preset tên rõ, active state tương phản tối, hướng dẫn thao tác và reset ở sát khung xem. Ảnh tĩnh nằm riêng dưới viewer; chữ cuối trang nói rõ chưa mô phỏng hành động. Các screenshot đã xem không có text chồng nút hoặc nội dung quan trọng bị modal che ngoài ý định.
- Không phát hiện blocker visual/UX khiến người dùng không thể xem và phản hồi về giả thuyết không gian hiện tại. Đây không phải xác nhận đầy đủ keyboard/mobile/accessibility hoặc gameplay.

### Findings và đánh đổi còn lại

1. **P2 — tiếp xúc mặt đất chưa thuyết phục.** Trong `render-eye.png`, đá tiền cảnh có bóng tách khá xa; `render-repair.png` làm chân người và giỏ tiền cảnh trông lơ lửng. Đây là quan sát hình ảnh, chưa xác nhận penetration/gap bằng đo mesh. Nó giảm cảm giác tỷ lệ và vật nặng có mặt trên bờ. Trước bước polish nên kiểm chân/đế/đá so mặt terrain và contact shadows; không phải blocker cho so sánh camera hiện tại.
2. **Giới hạn phong cách, không phải lỗi P0:** B03 có bờ hữu cơ, nhiều lớp cây/đồ nghề, vật liệu cũ và nét gouache. B04 có nền studio trống, bờ cắt thẳng, cây dạng quạt, màu phẳng và người mannequin. Giữ được nhóm địa điểm/đồ vật nhưng chưa giữ mức độ sống động hoặc chất họa của B03. Không nên tuyên bố đạt phong cách B03; art direction đã ghi đúng giới hạn này.
3. **Đánh đổi camera:** góc ngang tầm người che phần lớn mặt nước/bến sau thân thuyền và để vùng nền trống lớn phía trên; hữu ích cho tỷ lệ thuyền/nhà nhưng yếu hơn góc cao khi đọc tuyến đi xuống bến. Không cần ép một preset làm cả hai việc; chủ dự án cần chốt góc ưu tiên khi bước sang playable.
4. **P2 — độ lệch hiển thị:** browser repair có sắc olive tối hơn và bề mặt/shadow thô hơn render Blender. Hai hình không tương đương về chất liệu/ánh sáng. Đã được tài liệu công khai; khi chốt mỹ thuật cần ưu tiên renderer mục tiêu hoặc cân lại ánh sáng/shader, không lấy render đẹp hơn để bảo chứng chất lượng realtime.

### Kết luận hiện hành

Lượt critic visual độc lập **đã hoàn tất — Technical reviewed, có các giới hạn nêu trên**; bổ sung này thay tình trạng “chưa có lượt độc lập” ở phần lịch sử phía trên, không xóa lịch sử quota. Có thể trình chủ dự án xem và quyết định; **Pending user approval vẫn giữ nguyên**. Không nghiệm thu Unity, phục dựng lịch sử, human playtest hoặc chất lượng gameplay; không đề nghị merge dev/đóng Sprint13 khi approvals còn pending. Nguồn model hiện tại là procedural Blender theo ngoại lệ được giao; lượt tạo model kế tiếp phải dùng MCP Hyper3D theo chỉ dẫn mới.

### Retest kỹ thuật hẹp C04 — /root/review

Sau review hình ảnh, root giao kiểm chứng độc lập finding C04 về projection camera. Reviewer đã thực sự chạy `python3 prototypes/sprint13/experiment-04/browser-check.py`, exit 0, status passed, hai viewport 1280×720 và 1920×1080. Checker đọc `experimentDebug.cameraFov`; source getter trả `camera.fov` thật, không chỉ preset metadata. Cả overview/eye/repair đều assert FOV khớp `BEN_VAC_SCENE.cameras[key].fov`, position và target khớp scene với tolerance 1e-7. FOV quan sát: overview 30.14244099668009°, eye 45.74733038125343°, repair 30.14244099668009°. Evidence được tái tạo tại `tests/results/sprint13-experiment04/browser.json` cùng screenshots.

**C04 đã được retest độc lập đạt trong Chrome software-WebGL tại hai viewport trên.** Đây là cross-check finding cụ thể, không phải một lượt code review mới toàn bộ. Lượt này bổ sung runtime evidence sau phần review ảnh phía trên; không rút lại các P2 hình ảnh hoặc thay Pending user approval. Không sửa implementation.
