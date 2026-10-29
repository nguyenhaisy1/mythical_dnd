# Thử nghiệm 06 — Ảnh xem trước game theo ba hướng

Sprint 13/SPIN-022, ngày 29/10/2026. **Đã tạo ba ảnh và hoàn tất review kỹ thuật/hình ảnh. Mọi sản phẩm Pending user approval.** Đây là ảnh mô phỏng ý tưởng, không phải game chạy hoặc model 3D đã dựng.

## Mục tiêu dễ hiểu

Chủ dự án muốn xem game có thể trông như thế nào trước khi đầu tư vào model thật. Vòng06 dùng ảnh để thử ba cách trình bày cùng tình huống sửa thuyền ở Bến Vạc: bố cục, màu sắc, chất liệu và khoảng cách tới đời sống nhân vật. Một ảnh có thể tạo cảm giác 3D nhưng vẫn là ảnh raster được tạo riêng; không cần geometry, rig hoặc pipeline Unity.

Ba hướng đã tạo:

| Hướng | Góc nhìn và diện mạo | Câu hỏi cần xem |
|---|---|---|
| A | Tranh 2D góc chéo, nét mực/gouache, bảng màu hạn chế | Có đọc được lối đi, việc sửa thuyền và đời sống quanh bến ở khoảng cách gameplay? |
| B | Ảnh giả lập 3D gần tầm người, vật liệu tự nhiên dưới ánh sáng dịu | Có cảm giác đang ở trong thế giới, người và công việc có rõ hơn mà không lệ thuộc màu vàng/xanh phủ toàn cảnh? |
| C | 2.5D nhìn ngang, lớp cảnh như sân khấu vẽ tay | Silhouette và tương tác có rõ, có giữ được cảm giác đời sống thay vì cảnh trang trí? |

Đây là giả thuyết để quan sát, chưa là kết luận chất lượng hoặc phương án đã được user chọn. [Art direction](../../../../prototypes/sprint13/experiment-06/ART-DIRECTION.md) ghi tiêu chí và giới hạn của từng hướng; [gallery](../../../../prototypes/sprint13/experiment-06/index.html) trình bày ba ảnh, có thể đổi phương án và bật/tắt lớp giao diện minh họa.

## Phạm vi và thay đổi cách làm

- Root tạo ảnh raster bằng built-in imagegen; không gọi Hyper3D, dựng Blender hoặc tạo model mới cho vòng06.
- Gallery chỉ đổi ảnh và xem lớn; UI nếu có là minh họa, không thực thi sửa thuyền, NPC hoặc WorldState.
- Không coi các ảnh riêng là ba camera nhất quán của một scene 3D; không suy ảnh có chiều sâu thành bằng chứng model thật.
- Giữ nguyên01–05. Model A vòng 05 lệch brief và lượt sửa lỗi provider chưa xác định ID vẫn là lịch sử thật; vòng06 không sửa hoặc gửi lại request đó.
- Chỉ dẫn Hyper3D 28/10 áp dụng cho task model thật nếu chủ dự án giao lại sau này. Yêu cầu mới ưu tiên ảnh xem trước trong giai đoạn hiện tại.

## Phân công hiện tại

| Actor | Phạm vi |
|---|---|
| /root/s13e6_direction | Art direction và cách so sánh; không tự gọi generation |
| /root/s13e6_gallery | Gallery offline và nhãn phân biệt ảnh/UI minh họa |
| /root/s13e6_docs | Đồng bộ chỉ dẫn mới tới policy, handoff, sprint/approval, runbook và export inputs |
| /root | Tạo ảnh, provenance, tích hợp, kiểm tra lớp đầu và tổng hợp kết quả |

Hai critic độc lập tiếp nhận sau khi ba workers bàn giao: `/root/s13e6_critic_code` kiểm provenance và browser; `/root/s13e6_critic_visual` xem trực tiếp ba ảnh, đối chiếu brief và chất lượng. Ba workers chạy song song; critics chạy ở lượt sau, không có sáu actor đồng thời. Không có human playtest hoặc Unity acceptance trong scope này.

## Trạng thái evidence

Root đã gọi built-in `image_gen` ba lần, một lần cho mỗi phương án. Không truyền ảnh tham chiếu; mỗi ảnh được tạo mới từ prompt riêng. [Prompts](../../../../prototypes/sprint13/experiment-06/prompts.json) giữ nguyên văn prompt; [provenance](../../../../prototypes/sprint13/experiment-06/provenance.json) lưu tên output gốc, SHA256, kích thước và xác nhận không hậu kỳ. Ba PNG được sao chép nguyên bytes vào project:

- A: `assets/a-oblique.png`, 1672×941 — góc chéo nhìn tổng thể bến sửa thuyền.
- B: `assets/b-grounded.png`, 1672×941 — góc qua vai, thợ và thuyền ở gần.
- C: `assets/c-sideview.png`, 1672×940 — góc ngang, các nhân vật tách silhouette rõ.

Root đã thực sự xem cả ba output. Thuyền mở trên giá sửa và mái xưởng tách biệt hiện diện trong từng ảnh; không dùng kết quả vòng05 hoặc mesh thay thế. Tuy nhiên, ảnh riêng không đảm bảo cùng nhân vật, kích thước bến hoặc hình học giữa các phương án.

Nhận xét lớp đầu của root: A có lối đi và hoạt động dễ quan sát nhưng texture dày hơn ý định gouache giản lược; B gần ảnh chụp, ánh sáng nắng rõ hơn brief dịu; C dễ nhìn người và cuộc đối thoại nhưng đất còn vàng ấm, tiền cảnh có độ mờ. Chưa tuyên bố giải quyết hoàn toàn cảm giác màu AI hoặc đạt phong cách cuối. Đây là đánh giá của AI, không là phản hồi hay approval của chủ dự án.

Gallery giữ tranh gốc, lớp UI bằng HTML riêng với nhãn minh họa. Checkbox cho phép ẩn toàn bộ HUD; modal chỉ hiện ảnh gốc. Các dòng hành động chỉ minh họa cách trình bày, không thực thi game.

`/root/s13e6_critic_code` đã chạy và tái kiểm độc lập `python3 prototypes/sprint13/experiment-06/browser-check.py` sau khi thêm nút hiện/ẩn HUD: exit0 trên Chrome156.0.8078.47 với 1280×720, 1920×1080 và 390×844. Ảnh decode được; hash và bytes khớp output gốc; nút/phím A/B/C, thumbnail, dialog/Escape/trả focus, HUD/Space và chữ150% hoạt động; không tràn ngang, không pageerror hay request mạng ngoài. Evidence local ignored: `tests/results/sprint13-experiment06/browser.json` và PNGs. Root đã xem screenshot desktop B và bản cuối tắt HUD, không nhận lượt automation của critic là lượt tự chạy.

[Review code](CRITIC-CODE-06.md) không phát hiện blocker kỹ thuật trong phạm vi gallery. [Review hình ảnh](CRITIC-UX-06.md) đã xem ba PNG và hai screenshot cuối; ba findings P2 còn mở: A dày texture, B thiên ảnh chụp/nắng, C đất vàng và blur tiền cảnh. Đề nghị tắt HUD được sửa và xác nhận; không dùng sửa nhãn/UI để coi các vấn đề mỹ thuật đã hết.

Nhận định của reviewer: A phù hợp để thử tiếp khả năng quan sát/khám phá, C phù hợp để thử tiếp người/đối thoại, B làm đối chứng nhập vai. Đây là ý kiến AI để hỗ trợ lựa chọn, không phải quyết định thay user. Không có P1 cản trở xem ba ý tưởng; không đồng nghĩa nghiệm thu hướng mỹ thuật hoặc xử lý xong phản hồi màu AI.

## Đồng bộ tài liệu

Chỉ dẫn mới được thêm vào AGENTS.md, .agent, art-direction reference, [Hyper3D runbook](../../../runbooks/HYPER3D-MCP.md), handoff, sprint brief, approval register và changelog. Export source bổ sung report 06 cùng hai review 06 sau khi có. Không thay nghiên cứu nguồn, generated GDD, gameplay baseline, save/schema/content versions hoặc product VERSION. Root regenerate và kiểm export từ nguồn này; kết quả chạy thật được append trong WORKLOG. Export là container kiểm nội dung/nguồn, không phải nghiệm thu layout hoặc game.

[Danh sách cần chốt](APPROVALS.md) giữ S13-A01–A04 Pending/Changes requested. Việc user chọn cách xem bằng ảnh không tự duyệt mỹ thuật, UI/origin hoặc kết quả Sprint 13. Không merge dev hoặc chuyển Sprint 14 trước quyết định rõ của chủ dự án.
