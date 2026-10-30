# Critic 1 — Code và tích hợp hình ảnh, Experiment 02

Actor `/root/s13_critic_code`, ngày27/10/2026. Review độc lập code, nội dung và ảnh hiện tại. **Technical review không là user approval**; Sprint13 còn mở.

## Findings

- **P2 C02 — hướng dẫn HTTP làm hỏng liên kết so sánh.** README ban đầu serve riêng `experiment-02`; đường dẫn `../experiment-01/index.html` trỏ ra ngoài root server nên không mở được bản01. File local không gặp lỗi này. Đề nghị serve `prototypes/sprint13` và mở `/experiment-02/`. Đã gửi root; đây là kiểm phân giải đường dẫn, chưa chạy server HTTP độc lập.
- **P2 C03 — lời quan sát không khớp cảnh trung tâm.** PNG thực cho thấy chiếc thuyền tiền cảnh kê trên giá ở bờ, người đang sửa bên cạnh; panel lại nói “Một mạn thuyền sẫm màu dưới mặt nước.” Đề nghị lời quan sát trung tính khớp hình, vẫn không khẳng định hỏng hóc hoặc runtime truth. Đã gửi root.

Không thấy P0/P1 trong scope code kiểm lần này. Chưa kết luận người dùng thích hướng mới hoặc artwork chính xác khảo cổ.

## Evidence thực

- `node --test prototypes/sprint13/experiment-02/campaign.test.js`:5passed,0failed,0skipped. `diff -q` campaign.js giữa02/01 exit0, model giữ nguyên. Không có runtime LLM hoặc production RNG/save binding; thuật toán thử nghiệm vẫn tách production.
- Browser độc lập bằng Python Playwright/Chrome156.0.8078.47, file local; `/tmp/s13-critic-browser02.py` exit0, không pageerror. Kiểm cancel generation không Reveal muộn, New→Reveal→World, hai claim, Escape trả focus, hủy Preview không kết quả, confirm có nhãn mock, campaign mới xóa status cũ, thiếu module có thông báo và quay menu được.
- `/tmp/s13-critic-art02.py` bổ sung mở toàn cảnh từ menu, ảnh complete/naturalWidth>0, Escape trả focus menu-art; toàn bộ luồng trên chạy lại exit0, không pageerror.
- Đọc app/index/artdirection/README/provenance; xem PNG thật qua view_image. DTO chỉ được render bằng textContent; kỹ năng có nguồn history, claims có nguồn ký ức; không suy ảnh thành authoritative inventory hoặc NPC state. Dialog mới dùng chung native modal/focus handling. Cảnh chỉ là concept; chưa collision/Unity gameplay.

Chưa kiểm toàn bộ accessibility, mọi browser, production projection, performance hoặc human playtest. Root và critic2 có evidence phạm vi khác; không gộp thành kiểm đã tự chạy.

## Tái kiểm C02/C03 — resolved

README đã sửa serve `prototypes/sprint13` và URL `/experiment-02/`. Critic chạy HTTP server local cùng thư mục root, cổng tạm37377 và Chrome Playwright: mở02 rồi click `.compare-link`; trang `/experiment-01/index.html` HTTP200, nhãn giao diện01 hiển thị đúng, các asset01 HTTP200; exit0. **C02 resolved bằng browser HTTP thực.**

Đọc lại index hiện tại: câu quan sát đã thành “Một chiếc thuyền kê bên bến. Chưa rõ cần sửa những gì.”, phù hợp vị trí thuyền trong PNG đã xem và không xác nhận hỏng hóc/knowledge mới. **C03 resolved.** Đây là tái kiểm hai finding cụ thể; không tuyên bố đã nghiệmthu toàn bộ UX hoặc finding bố cục do critic2 phụ trách. Mọi sản phẩm vẫn Pending user approval.
