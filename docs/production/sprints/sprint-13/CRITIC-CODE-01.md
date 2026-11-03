# Critic 1 — Code và ranh giới dữ liệu, Experiment 01

Actor `/root/s13_critic_code`; review độc lập ngày 27/10/2026. Áp dụng skill mythical-validation. Mọi sản phẩm **Pending user approval**; kiểm kỹ thuật không là nghiệm thu Sprint13.

## Finding cần xử lý

**P1 C01 — thiếu phần trình bày năng lực và nguồn lịch sử.** `campaign.js` tạo `skills`, `attributes` và `historyIds`, nhưng bản `app.js`/`index.html` tại lần kiểm đầu không hiển thị chúng. Repro: mở cuộc đời mới → Reveal chỉ có biography → World/Journal chỉ có tri thức. Người xem không thể trả lời câu hỏi kiểm chứng “một kỹ năng bắt nguồn từ trải nghiệm nào” trong CAMPAIGN-EXPERIMENT. Đề nghị hiển thị kỹ năng cùng trải nghiệm đóng góp, ghi rõ giá trị prototype. Đã gửi root và worker UI; cần tái kiểm bản sửa.

Không phát hiện P0 hoặc lỗi ghi authoritative state trong phạm vi đọc. Không coi kết luận này là chứng minh production knowledge isolation: prototype không nạp world snapshot hoặc dữ liệu bí mật để kiểm đường projection thực.

## Kiểm tra đã chạy

- `node --test prototypes/sprint13/experiment-01/campaign.test.js`: 5 tests passed, 0 failed/skipped; gồm 500 mẫu, immutable, browser/CommonJS parity, invalid input và no reroll. Đọc code xác nhận skill gains cộng từ hai history episodes; context bất biến; lựa chọn purpose-scoped không clock/global random. Thuật toán JS khác core được công bố rõ.
- Playwright Python, Chrome `156.0.8078.47`, headless, mở file HTML local. Script độc lập `/tmp/s13-critic-browser.py` chạy exit0: hủy generation trước timeout không bật Reveal muộn; New→Reveal→World; hai claim có nguồn; Escape trả focus về nút mở; hủy Preview không hiện kết quả; xác nhận hiện nhãn chưa tiêu thời gian/vật liệu; campaign mới xóa kết quả cũ; xóa `window.MythicalCampaign` gây lỗi hiển thị có thể quay menu. Không có `pageerror` trong các luồng này.
- Đọc cả `ART-DIRECTION.md` và các nhãn UI: SVG là cảnh minh họa, không được dùng làm truth/collision; hotspot mở preview cố định, không giả simulated entity. Result nói rõ mock, không tuyên bố save hoặc hành động đã xảy ra. Không thấy mạng/runtime LLM hoặc `innerHTML` nhận DTO trong app.

Phạm vi chưa kiểm: human playtest, production C#/Unity binding, save/replay, đầy đủ accessibility, mọi browser và visual preference. Critic 2 chịu phần review UX/art riêng. Báo cáo này không duyệt A01–A04 thay chủ dự án.

## Tái kiểm C01 — đã sửa

Worker UI thêm phần “Điều cuộc sống đã dạy bạn”, liệt kê kỹ năng với tuổi và câu chuyện từ `historyIds`, không hiển thị số balance chưa chốt. Critic chạy Chrome độc lập lần nữa: Reveal hiển thị `#character-skills` và `.skill-sources`; mẫu thực hiển thị Việc sông nước từ tuổi10 và Buộc và sửa dây từ tuổi18. Đọc code xác nhận mọi historyIds được resolve vào actor.history, dữ liệu dùng textContent. **C01 resolved; không còn finding chặn trong phạm vi code đã review. Technical reviewed, vẫn Pending user approval.**
