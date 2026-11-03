# Thử nghiệm09 — Ba ngôn ngữ hình ảnh cho toàn game

Sprint13/SPIN-025, ngày29/10/2026. **Ba bảng concept đã tạo và có review độc lập; mọi phương án Pending user approval.** Bảng concept ảnh-only để xem trước, không tạo model3D thật hoặc triển khai UI/gameplay.

## Mục tiêu dễ hiểu

Chủ dự án yêu cầu ý tưởng táo bạo hơn và nhắc đồ họa sẽ xuyên suốt game, từ tạo hình nhân vật đến giao diện. Vòng09 vì vậy thử một diện mạo thống nhất cho cảnh chơi, nhân vật toàn thân/chân dung, vật phẩm/icon và các màn hình hội thoại, túi đồ, nhật ký. Bảng concept có cảnh minh họa cùng mẫu cast/UI giúp thấy cả hệ thống hình ảnh thay vì suy từ một tranh phong cảnh.

| Hướng đã thử | Ngôn ngữ chung | Điều cần quan sát trên bảng |
|---|---|---|
| A — Sơn mài sống | Đen sâu, đỏ son, vàng/khảm sáng và đường nét có chủ đích | Người, không gian và khung UI có dùng cùng quy tắc mảng/nét, chữ có đọc được trên vật liệu tối? |
| B — Phù điêu đồng | Khối nổi/khắc, ánh đồng và patina | Nhân vật còn có cá tính, vật phẩm/icon và UI có phân biệt rõ thay vì hòa vào một bề mặt trang trí? |
| C — Mực sống | Mực đen/trắng, khoảng âm và nét đỏ biểu cảm | Silhouette, chân dung, hình cảnh và UI có chung nhịp nét mà vẫn đọc rõ nội dung? |

Đây là hướng sáng tạo hư cấu, không là khẳng định về phong cách lịch sử/khảo cổ hoặc canon mới. Bối cảnh sinh hoạt sông nước và quan hệ/nghề nghiệp giữ vai trò trong cảnh, không biến RPG thành bắt buộc chiến đấu. Bảng nhân vật chỉ minh họa cast/diện mạo; không thêm character creator chọn/chỉnh origin trái baseline ngẫu nhiên.

## Phạm vi và nguồn thực tế

Root đã gọi built-inimage_gen ba lần để tạo ba bảng raster độc lập. PNG gốc được sao chép nguyên bytes vào `prototypes/sprint13/experiment-09/assets/a-lacquer.png`, `b-bronze.png`, `c-ink.png`; cả ba1672×941RGB. Không truyền ảnh tham chiếu, không hậu kỳ hoặc cắt ghép. [Prompts](../../../../prototypes/sprint13/experiment-09/prompts.json) lưu nguyên văn; [provenance](../../../../prototypes/sprint13/experiment-09/provenance.json) ghi tên output gốc, SHA256, bytes và kích thước. [Gallery09](../../../../prototypes/sprint13/experiment-09/index.html) giữ khung xem trung tính, không phủ một HUD HTML chung để che UI đã thiết kế trong ảnh.

UI vẽ trong bảng là **raster minh họa**, không là text component, bố cục responsive hoặc interaction đã thực thi. Nhãn chữ có thể còn sai dấu/ký tự hoặc nhỏ; phải quan sát đầu ra thật và ghi giới hạn, không lấy việc tải ảnh thành công để nghiệmthu UI toàn game. Không gọi Hyper3D/Blender, không tạo geometry thật, không đưa concept vào authoritative state.

## Phân công và evidence

| Actor | Phạm vi được giao |
|---|---|
| /root/s13e9_direction | Brief/ma trận nhất quán qua thế giới, cast, vật phẩm và UI |
| /root/s13e9_gallery | Gallery xem bảng concept, nhãn ảnh-only và thư viện lịch sử |
| /root/s13e9_docs | Lưu chỉ dẫn tổng thể và đồng bộ hồ sơ liên quan |
| /root | Tạo ảnh/prompt/provenance, tích hợp, kiểm lớp đầu và tổng hợp |

Ba workers đã bàn giao và lượt phản biện09 ban đầu đã bắt đầu, nhưng phiên bị ngắt trước khi lưu hai report. Trong lượt tiếp tục, `/root/s13e10_critic_code` thực sự chạy lại checker09 và `/root/s13e10_critic_visual` xem lại ba PNG09 cùng screenshot, rồi viết hai report09 dưới tên actor mới. Không gán report này cho actor cũ. Root tạo ảnh trong lúc ba workers làm việc; critics theo lượt sau, không claim sáu actor cùng chạy.

Root đã xem trực tiếp cả ba output. Các thành phần đều thực sự hiện trên board:

| Thành phần | A — Sơn mài | B — Phù điêu đồng | C — Mực sống |
|---|---|---|---|
| Thế giới | Mảng sông/nhà/người đen–son, vạch sáng vàng | Bến/người/nhà bằng khối nổi–chìm và patina | Mảng đen/trắng, nét khô, điểm đỏ trên người và cảnh |
| Nhân vật toàn thân/chân dung | Dáng và nếp áo bằng mảng/nét vàng, portrait cùng ngôn ngữ | Cast và portrait cùng chất relief, phân biệt khuôn mặt/tuổi | Dáng biểu cảm, tóc/áo bằng mực, portrait theo cùng nét |
| Icon/vật phẩm | Nét khảm vàng trên ô đen | Vật phẩm nổi trong hốc đồng | Hình vẽ mực và nhãn trên nền sáng |
| Hội thoại/lựa chọn | Bảng đen, viền son, chữ sáng, nút chọn đỏ | Bảng đồng tối, dấu chọn/viền rõ và chữ sáng | Khung sáng nét cọ đen, ô chọn đỏ, nhãn/biểu tượng |
| Túi đồ/nhật ký | Cùng đen–son/vàng, minh họa nhật ký cùng chất liệu | Cùng nổi/chìm và khung kim loại | Cùng nền sáng/nét mực, sketch nhật ký đồng bộ |

Bảng này xác nhận phạm vi được minh họa, chưa chứng minh đáp ứng mọi màn hình, cast ngẫu nhiên đa dạng, animation, trạng thái UI hoặc hệ production. Các chỉ dẫn vật liệu là cảm hứng mỹ thuật hư cấu; không khẳng định sơn mài/chất liệu/motif phù hợp khảo cổ. Nhân vật và bối cảnh thay đổi giữa các board, không phải cùng geometry/cast chính xác. Chữ lớn đọc được trong quan sát của root; chữ nhỏ/nhật ký cần xem PNG gốc, không tuyên bố mọi ký tự và độ tương phản đã được audit.

Không có human playtest, Unity/gameplay acceptance hoặc typography/accessibility audit đầy đủ trong scope này.

## Kiểm chứng gallery và phản biện đã hoàn tất

Actor mới `/root/s13e10_critic_code` thực sự chạy `python3 prototypes/sprint13/experiment-09/browser-check.py`: exit0 trên Chrome156.0.8078.47, ba viewport1280×720/1920×1080/390×844. Ảnh/provenance/bytes khớp PNG gốc; main/thumbnail giữ trọn board, nút/phím/modal/Escape/focus/linkPNGgốc và tám historylinks01–08 đạt; không pageerror/requestngoài ởfilelocal. Evidence ignored: `tests/results/sprint13-experiment09/browser.json` và screenshots.

[Code review09](CRITIC-CODE-09.md) giới hạn kết luận ở trang xem ảnh. [Visual review09](CRITIC-UX-09.md) xem ba PNG và screenshot: cùng ngôn ngữ xuyên cảnh/cast/icons/UI; A/B còn mật độ nét/vật liệu cao, C tạo hình tóc/khăn và icon mái nhiều tầng là câu hỏi bản sắc/khả năng bao quát cast, chưa tự là lỗi canon. Không thấy P1 cản xem concept; không nghiệmthu mỹ thuật hoặc UI runtime. Chữ150% chỉ áp dụng HTML, UI bitmap không reflow; chữ nhỏ cần mở ảnh gốc để zoom. Không dùng kết quả tải ảnh chứng minh toàn game đã có UI usable.

## Thư viện và bàn giao

Vòng09 bổ sung các phiên bản tham khảo; giữ01–08, artwork và hồ sơ cũ để xem lại hoặc kết hợp. Không mặc định thử mới là loại hoặc đánh giá kém bản cũ. [Danh sách cần chốt](APPROVALS.md) giữ S13-A01–A04 Pending/Changes requested theo phản hồi thật; user chưa chọn một hướng hoặc nghiệmthu Sprint13.

Chỉ dẫn theo scope định hướng toàn game được thêm vào AGENTS.md, .agent và art-direction reference; HANDOFF, sprint brief, approval register, changelog và export source cùng trỏ vòng09. Không áp yêu cầu làm lại mọi thành phần cho mỗi batch ảnh nhỏ. Root regenerate/check exports từ nguồn sau khi ảnh/review/evidence được hoàn thiện. Không sửa archive, generated GDD, canon/rule, save/schema/content versions hoặc product VERSION; không merge dev, đóngSprint13 hoặc mởSprint14.
