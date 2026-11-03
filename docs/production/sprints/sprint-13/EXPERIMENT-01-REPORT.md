# Experiment01 — Bến Vạc, buổi sáng trước mùa nước

SPIN-016, Sprint13. Bản thử đã chạy và qua kiểm tra lớp đầu ngày27/10/2026; **chưa được chủ dự án nghiệm thu**.

## Kết quả dễ hiểu

Đã có một cảnh bến sông bằng SVG tự vẽ, giao diện có thể bấm thử từ khởi đầu đến nhận nhân vật, vào bến, đọc nhật ký và xem trước hành động. Nhân vật được dựng từ một hoàn cảnh gia đình nhỏ rồi đến trải nghiệm sống; kỹ năng hiển thị cùng trải nghiệm hình thành nên nó. Đây là sản phẩm để chọn hướng hình ảnh và cách tương tác, chưa phải game Unity hoặc hành động chạy bằng luật production.

Mở [index.html](../../../../prototypes/sprint13/experiment-01/index.html) trực tiếp bằng Chrome. [Hướng dẫn](../../../../prototypes/sprint13/experiment-01/README.md) có lệnh chạy và kiểm tra. Không cần tài khoản/API/mạng.

## Phân công thực tế

| Vai trò | Actor | Kết quả |
|---|---|---|
| Mỹ thuật | /root/s13_art | SVG bến sông, art direction, phân biệt canon và suy diễn |
| Giao diện | /root/s13_ui | HTML/CSS/JS offline; sửa findings về kỹ năng và tương phản |
| Cuộc đời mẫu | /root/s13_campaign | Ba hoàn cảnh/sáu nhánh lịch sử, API readonly và5tests |
| Bắt lỗi code | /root/s13_critic_code | Node/browser review độc lập; phát hiện thiếu kỹ năng trên Reveal |
| Bắt lỗi UX | /root/s13_critic_ux | Browser/ảnh/canon/keyboard review độc lập; phát hiện chữ phụ thiếu tương phản |
| Kiểm tra lớp đầu | /root | Tích hợp, tái chạy tests/browser, nhìn ảnh thực, tổng hợp và quản lý pending |

Ba workers chạy đồng thời; hai critics thực hiện ở lượt sau theo giới hạn bốn tác nhân kể cả root. Critics từng bị quota và được nối lại ngày27/10; không ghi những lượt chưa chạy là đã review.

## Findings và thay đổi

- **C01/P1:** giao diện ban đầu không hiển thị kỹ năng nên không đánh giá được quan hệ quá khứ→năng lực. Đã bổ sung “Điều cuộc sống đã dạy bạn”, liệt kê kỹ năng và từng trải nghiệm nguồn. Critic code tái kiểm độc lập, resolved.
- **UX01/P2:** chữ phụ thiếu tương phản trên nền giấy/thẻ. Đã làm đậm chữ, nhãn và avatar. Tỉ lệ chữ phụ trên nền giấy5,42:1, thẻ4,98:1; hotspot trường hợp nền tối4,69:1. Tái kiểm của critic ghi tại báo cáo riêng.
- Cảnh/mô hình hiện chỉ là một phương án. Chưa có dữ liệu người chơi để kết luận đẹp, dễ hiểu hoặc gắn bó nhân vật.

Báo cáo phản biện: [code](CRITIC-CODE-01.md), [UX](CRITIC-UX-01.md). Technical reviewed không là user approved.

## Kiểm tra và bằng chứng

`node --test prototypes/sprint13/experiment-01/campaign.test.js`: **5/5 passed**, không skip; kiểm500seeds, lịch sử theo tuổi, kỹ năng dẫn xuất, immutability, invalid input và browser/CommonJS parity. Thuật toán random của experiment được ghi version riêng, không tuyên bố tương đương core RNG.

`python3 prototypes/sprint13/experiment-01/browser-check.py`: Chrome headless trên file thật, không pageerror; Menu→Reveal→World, kỹ năng có nguồn, Preview/Escape trả focus, J mở hai ký ức, actor nhất quán, chữ150% tại1280×720 không tràn ngang. Root đã xem ảnh menu/world/reveal thực. Art XML và internal refs đạt; không script/tài nguyên mạng trong SVG.

Ảnh/evidence local tại `tests/results/sprint13-experiment01/`: menu.png, reveal.png, world.png, preview.png, journal.png, world-150.png và browser.json. Critic UX có bộ ảnh riêng trong critic-ux/. Đây là automation, không phải human playtest. Không cần chạy lại toàn449tests kernel vì không sửa core; CI repository vẫn áp dụng trước nhánh review.

## Giới hạn và quyết định còn thiếu

Chưa có WorldState binding, save/load, repair effects, nhân vật trong Unity, NPC autonomy hoặc generator đầy đủ. Hành động chỉ trả feedback mẫu có nhãn rõ. SVG không là bản đồ collision/navigation hoặc phục dựng khảo cổ. Chữ nhỏ ở100% và tính hấp dẫn mỹ thuật cần user xem thực tế; kiểm keyboard/contrast không là audit accessibility toàn diện.

Bốn mục [APPROVALS](APPROVALS.md) đều Pending. Root chỉ kết luận bản này đủ cụ thể để xem và góp ý, không chốt sản phẩm. Không merge dev, đóng Sprint13 hoặc mở Sprint14. Nếu cần phương án hình ảnh/UX khác, mở spin thử nghiệm mới và giữ kết quả phương án01.
