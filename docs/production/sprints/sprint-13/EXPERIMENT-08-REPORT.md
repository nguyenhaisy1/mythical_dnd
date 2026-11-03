# Thử nghiệm08 — Thêm ba phiên bản trong thư viện hình ảnh

Sprint13/SPIN-024, ngày29/10/2026. **Ba ảnh đã tạo và được review lớp kỹ thuật/hình ảnh; mọi phiên bản vẫn là tham khảo, Pending user approval.** Vòng này dùng ảnh để xem trước game, không tạo model3D thật hoặc thực thi gameplay.

## Mục tiêu dễ hiểu

Chủ dự án ghi nhận ý tưởng đang tốt dần và muốn có thêm version tham khảo. Vòng08 mở thêm ba cách thể hiện cùng một cảnh nhà chữa trị bên suối đồi, để quan sát ngôn ngữ đồ họa thuận tiện hơn. Người mang giỏ thảo mộc tới nhà sàn, người hành nghề ở hiên, một người cao tuổi cùng gia đình chờ; lối đi và cầu tre nối cảnh sau mưa. Đây là bố trí hư cấu để thử hình ảnh, chưa là canon hoặc đặc tả hoạt động chữa trị.

| Hướng đã thử | Cách thể hiện | Câu hỏi cần quan sát |
|---|---|---|
| A | Hoạt hình2D vẽ tay, nét rõ và mảng bóng gọn | Người và công việc có đọc rõ mà vẫn giữ sự sống động của cảnh? |
| B | Thế giới như được dệt từ sợi/vải, hình thành bởi cấu trúc dọc/ngang của sợi | Chất liệu có tạo một diện mạo riêng, hay làm người/lối đi khó đọc? |
| C | Ảnh diện mạo3D cách điệu, hình khối và mặt phẳng rõ | Chiều sâu/hình khối có giúp hiểu không gian mà không rơi lại vào tả thực? |

Cùng ý tưởng cảnh không có nghĩa dùng chung geometry, camera chính xác, nhân vật nhất quán hoặc kiểm soát hoàn toàn các biến số. Các ảnh tạo độc lập vẫn có thể khác bố cục, ánh sáng và chi tiết; chưa dùng chúng làm phép đo production cost/performance.

## Thư viện tham khảo, không phải loại dần

Mở spin hoặc version mới **không mặc định bỏ, thay thế hoặc đánh giá kém bản cũ**. Giữ các vòng01–08, artwork và hồ sơ nguồn để xem lại, so sánh hoặc kết hợp ý tưởng. Nhận xét/finding chỉ áp dụng đúng artifact và phạm vi đã quan sát; lỗi cụ thể không tự kết luận cả ý tưởng dở. Giữ phản hồi lịch sử thật, gồm đồ họa bản01 chưa ưng ý. Chỉ ghi chọn/loại/thay thế khi chủ dự án nói rõ.

Lời khen tiến triển và yêu cầu nghĩ thêm không là final approval cho một hướng hoặc kết quả Sprint13. [Danh sách cần chốt](APPROVALS.md) tiếp tục giữ quyền quyết định của user; không merge dev, đóng sprint hoặc chuyển Sprint14.

## Phạm vi và phân công

Root dùng built-inimage_gen để tạo raster. Ba PNG thực đã lưu trong [gallery08](../../../../prototypes/sprint13/experiment-08/index.html): `assets/a-cel.png`, `assets/b-woven.png`, `assets/c-faceted.png`. Cả ba1672×941RGB, tạo mới bằng ba lần gọi built-inimage_gen riêng, không truyền ảnh tham chiếu. [Prompts](../../../../prototypes/sprint13/experiment-08/prompts.json) lưu nguyên văn; [provenance](../../../../prototypes/sprint13/experiment-08/provenance.json) ghi output gốc, SHA256, bytes và kích thước. PNG sao chép nguyên bytes, không chỉnh màu hoặc cắt ghép hậu kỳ. Gallery chỉ phục vụ đổi ảnh, xem lớn và lớpUI minh họa. Không gọi Hyper3D/Blender, không tạo model hoặc đưa ảnh vào authoritative state.

| Actor | Phạm vi được giao |
|---|---|
| /root/s13e8_direction | Art direction và hướng dẫn xem/so sánh |
| /root/s13e8_gallery | Gallery ảnh-only, nhãn phiên bản và liên kết thư viện |
| /root/s13e8_docs | Làm rõ policy thư viện và đồng bộ hồ sơ liên quan |
| /root | Tạo ảnh, lưu nguồn/prompt, tích hợp và kiểm lớp đầu |

Ba workers đã bàn giao; sau đó hai critic độc lập nhận artifact: `/root/s13e8_critic_code` kiểm provenance và gallery, `/root/s13e8_critic_visual` xem hình ảnh và đối chiếu ý định. Root tạo ảnh trong lúc ba workers làm việc; critics ở lượt sau, không có sáu actor đồng thời.

Root đã xem cả ba output. A có nét viền và mảng bóng nhưng môi trường vẫn nhiều chi tiết hơn brief cel giản lược; B có nhịp sợi, mái/cây/nước bằng cấu trúc thêu và nổi khối khá dày; C rõ mặt phẳng của đá/cây, người và vật liệu có độ chuyển mềm hơn low-poly thuần. B khác rõ về vật liệu; A/C gần nhau về bố cục và sắc xanh/đất, vẫn có thể so sánh nét vẽ với bề mặt/khối. Đây là quan sát để hiểu từng version, không xếp hạng loại bỏ. Các điểm có ích có thể được phối hợp ở một hướng sau khi chủ dự án muốn thử.

Không có human playtest, Unity/gameplay, khảo cổ hoặc hiệu quả chữa trị acceptance trong scope này.

## Kiểm trang xem ảnh và phản biện

`/root/s13e8_critic_code` đã chạy `python3 prototypes/sprint13/experiment-08/browser-check.py`: exit0 ở1280×720,1920×1080 và390×844. Ba PNG decode được, bytes/hash/kích thước khớp provenance và output gốc; HUD trung tính đủ năm dòng giữ đúng qua ba ảnh; nút/phím/thumbnail, dialog/Escape/trả focus, bật/tắtHUD và chữ150% hoạt động. Không tràn ngang, pageerror hoặc request mạng ngoài khi mở filelocal. Bảy liên kết về01–07 tồn tại và nhãn thư viện không hàm ý loại bản cũ. Evidence ignored: `tests/results/sprint13-experiment08/browser.json` và screenshots. Root xem screenshot B, không nhận automation của critic là lượt tự chạy.

[Code review](CRITIC-CODE-08.md) kết luận kỹ thuật trong phạm vi gallery. [Visual review](CRITIC-UX-08.md) xem ba PNG và ảnh trình bày, mô tả lợi ích/đánh đổi của từng hướng: A/C còn gần nhau hơn B; B nổi sợi/thêu và dày texture; A chưa giản lược nền như cel thuần, C chưa đồng nhất faceting ở mọi vật thể. Đây là nhận xét theo artifact, không là quyết định loại hoặc chê toàn bộ ý tưởng. Không có P1 cản việc xem ý tưởng được phát hiện; vẫn chưa phải final art approval.

## Đồng bộ và bàn giao

Chỉ dẫn thư viện được làm rõ trong AGENTS.md, .agent, art-direction reference, HANDOFF, sprint brief, approval register và changelog. Export source thêm report08 và hai review08 sau khi được tạo; root regenerate/check từ source đã hoàn thiện. Không chỉnh archive, generated GDD, canon/rule, save/schema/content versions hoặc product VERSION. Ảnh/prompt/provenance và nhận xét đã lưu; checks/review có evidence riêng. Export check chỉ xác nhận source/container/navigation, không chứng minh chất lượng thị giác hoặc game.
