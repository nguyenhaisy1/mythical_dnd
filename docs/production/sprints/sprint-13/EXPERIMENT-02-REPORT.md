# Thử nghiệm02 — Bến Vạc, chất liệu và đời sống

Sprint13, SPIN-017, mở ngày27/10/2026; kiểm lại bản sửa28/10/2026. **Pending user approval.** User nhận xét đồ họa bản01 chưa ưng ý và yêu cầu một bản khác; đây là phản hồi thật về hình ảnh, không suy thành từ chối toàn bộ UI hoặc mô hình nhân vật.

## Thay đổi nhìn thấy được

Bản02 dùng tranh raster tạo bằng built-in image_gen: thuyền và người sửa ở tiền cảnh, nhà sàn/sinh hoạt ở trung cảnh, sương và núi phía xa. Ánh sáng, vật liệu gỗ/nước/mái lá và chi tiết lao động thay cho mảng vector phẳng của bản01. Đây là concept hư cấu lấy cảm hứng bối cảnh game, không phục dựng khảo cổ.

Giao diện dùng nền mực tối/chữ ngà/điểm nhấn đồng, cảnh toàn nền và bảng nổi. Có nút xem tranh đầy đủ không crop và liên kết so sánh bản01. Không gọi raster nền là scene3D hoặc gameplay Unity.

[Mở bản02](../../../../prototypes/sprint13/experiment-02/index.html), [bản01 giữ nguyên](../../../../prototypes/sprint13/experiment-01/index.html). Mở bằng Chrome hoặc xem [hướng dẫn](../../../../prototypes/sprint13/experiment-02/README.md). [Prompt và nguồn ảnh](../../../../prototypes/sprint13/experiment-02/assets/GENERATION.md) ghi built-in tool, asset copy trong repo và giới hạn sử dụng; không cần API key.

## Phân công

- /root/s13e2_art: art direction, tiêu chí composition, vật liệu, crop và UI; không tự nhận đã tạo ảnh.
- /root/s13e2_ui: HTML/CSS/JS mới, giữ các ràng buộc nhận nhân vật/knowledge, sửa bố cục theo critics.
- /root/s13e2_checks: script browser, README và Node checks. Browser ban đầu pending PNG; root chạy khi ảnh đã có.
- /root/s13_critic_code: phản biện độc lập code/state/content integration, Node/browser thật.
- /root/s13_critic_ux: phản biện độc lập ảnh thực, camera/crop, UI che focal point, readability.
- /root: tạo và lưu ảnh bằng imagegen, copy campaign baseline, tích hợp, browser/ảnh và nghiệm thu lớp đầu. Ba workers chạy song song; critics lượt sau trong giới hạn4concurrent.

## Findings và xử lý

1. Hướng dẫn HTTP ban đầu chỉ serve thư mục02 nên link về01 không truy cập được. Sửa serve thư mục sprint13 và URL/experiment-02/.
2. Copy cũ nói mạn thuyền dưới nước, trong khi ảnh mới thuyền kê trên bờ. Sửa mô tả quan sát theo hình, vẫn giữ nhu cầu sửa là chưa rõ.
3. Panel quan sát bên phải che người thợ/thuyền tại1280. Chuyển các panel sang trái trên vùng nước, giữ khu sửa thuyền bên phải; không dùng nút toàn cảnh để bỏ qua lỗi bố cục mặc định.

Chi tiết và tái kiểm tại [critic code](CRITIC-CODE-02.md), [critic UX](CRITIC-UX-02.md). Kết quả kỹ thuật không thay quyền chốt user.

## Kiểm tra và giới hạn

Campaign.js và tests kế thừa nguyên từ01;5/5Node tests đạt. Mô hình vẫn ba hoàn cảnh/sáu nhánh, không generatorproduction/coreRNG. Browser-check chạy Chrome/file:// tại1280×720 và1920×1080: ảnh decode và được dùng, Menu/Reveal/World, kỹ năng có nguồn, actor nhất quán, journal, modal/Escape/focus, chữ150%, liên kết so sánh và xem toàn cảnh. Evidence/screenshots ở tests/results/sprint13-experiment02/; không humanplaytest.

Ảnh gốc1672×941, PNG khoảng3,2MB. Đây là background concept đơn, không layer/collision/navigation. Chưa WorldState, save/load, repair thật hoặc Unity client. Không sửa bản01 hay engine. Không thay pending khác bằng Approved vì user chỉ phản hồi đồ họa.

[Danh sách cần chốt](APPROVALS.md): bản02 cần user đánh giá hướng hình ảnh và bố cục; A03/A04 chưa chốt. Chưa đóngSprint13, chưa mởSprint14, khôngmerge dev.

## Kiểm lại lớp đầu — 28/10

Root tái chạy5/5Node và browser hai viewports, xem screenshot World1280 saufix: người thợ/thuyền không còn bị panel phải che. Sửa khoảng trắng heading khi bỏ linebreak. Docs36generated/601links vàdiffcheck đạt; bộHTML/DOCX regenerate trướcpush. Các nhận xét vàapproval vẫn do user quyết định.
