# Thử nghiệm11 — Bốn cách thể hiện cùng một đời sống

Sprint13/SPIN-027. Mở29/10/2026, tiếp tục30/10/2026. **Bốn ảnh đã tạo và có review độc lập; mọi phiên bản vẫn tham khảo, Pending user approval.** Đây là ảnh concept cho toàn game, không chuyển engine, tạo model3D thật hoặc triển khai gameplay.

## Mục tiêu dễ hiểu

Chủ dự án muốn thử thêm2.5D,3D,pixel,anime hoặc cách khác. Vòng11 mở bốn tổ hợp để xem cách tổ chức không gian, hình người và giao diện có thể thay đổi cảm giác game. Mỗi bảng cần có cảnh chơi, hai mẫu toàn thân, ba chân dung, sáu icon vật phẩm và UI hội thoại/túi đồ/nhật ký, thay vì chỉ là một tranh phong cảnh.

| Hướng đã thử | Ý định thể hiện | Câu hỏi cần quan sát |
|---|---|---|
| 2.5D nhiều lớp | Cảnh vẽ tay nhìn ngang, nhân vật và các lớp không gian tách rõ | Người, lối đi và hoạt động có dễ đọc; cảm giác lớp cảnh có khác một vista3D? |
| 3D cách điệu | Khối nhân vật, kiến trúc và đồ vật có thể tích, vật liệu được giản lược | Hình khối giúp hiểu không gian và giữ được cảm giác người lao động? |
| Pixel đời làng | Cụm pixel, silhouette và chữ/icon theo nhịp điểm ảnh | Tuổi/nghề/cử chỉ có đọc rõ; pixel có đồng bộ qua cảnh, portrait và UI? |
| Anime đời thường | Nét vẽ tay, mảng bóng và biểu cảm cast trưởng thành | Có cá tính và chiều sâu đời sống mà không mặc định nhân vật trẻ/chiến đấu? |

**2.5D/3D là trục tổ chức không gian; pixel/anime là trục mỹ thuật.** Có thể kết hợp chúng, không phải bốn lựa chọn công nghệ loại trừ nhau. Các bảng là tổ hợp tham khảo, chưa là phép so sánh kiểm soát chỉ một biến. Ảnh tạo riêng không dùng chung geometry/camera hoặc bảo đảm mọi chi tiết cast khớp tuyệt đối.

## Tình huống và ranh giới

Cùng ý tưởng chợ lương thực, kho kê cao và cầu tre trước mùa nước. Nhân vật mẫu là phụ nữ lao động trưởng thành khoảng45tuổi, áo chàm/đai đất/giỏ công cụ; người giữ kho lớn tuổi và hàng xóm hỗ trợ quan sát quan hệ, nghề và biểu cảm. Đây chỉ là cast minh họa một cuộc đời, không cố định protagonist runtime hoặc thêm màn hình chọn/chỉnh origin. Baseline sinh nhân vật ngẫu nhiên giữ nguyên.

Cảnh, tên, lời thoại và vật liệu là ý tưởng hư cấu, không tự thành canon, rule hoặc phục dựng khảo cổ. Cảnh sâu không chứng minh quay camera tự do; lớp cảnh không chứng minh parallax; mặt người không chứng minh rig/animation; ảnh pixel không chứng minh palette/native grid/tileset production. Chữ UI trong PNG không reflow theo cỡ chữ HTML, nút game trong ảnh không có tương tác. Không có bằng chứng chi phí/performance hoặc tính hấp dẫn từ một ảnh tĩnh.

## Phạm vi và nguồn thực tế

Root đã dùng built-inimage_gen bốn lần ngày30/10 để tạo bốn bảng raster, lưu tại `prototypes/sprint13/experiment-11/assets/` với các tên `layered-25d.png`, `stylized-3d.png`, `pixel-life.png`, `anime-life.png`. Cả bốn1672×941RGB, sao chép nguyên PNG, không hậu kỳ hoặc truyền ảnh tham chiếu. Prompt đã lưu ở [prompts.json](../../../../prototypes/sprint13/experiment-11/prompts.json); [art direction](../../../../prototypes/sprint13/experiment-11/ART-DIRECTION.md) và README có từ phần chuẩn bị trước. [Provenance](../../../../prototypes/sprint13/experiment-11/provenance.json) lưu ngày tạo thật30/10, ngày chuẩn bị prompt29/10, output gốc, kích thước/bytes và SHA256. Ngày chuẩn bị không bị dùng làm ngày generation.

[Gallery11](../../../../prototypes/sprint13/experiment-11/index.html) chỉ phục vụ xem ảnh, không phủ HUD HTML chung lên UI riêng trong board. Không gọi Hyper3D/Blender hoặc dựng geometry thật ở giai đoạn này.

## Tiếp tục sau giới hạn tác nhân

WORKLOG ghi SPIN-027 đã mở và giao ba worker ngày29/10. Sau giới hạn sử dụng, kiểm tra ngày30/10 cho thấy brief/README/prompt còn trong workspace, chưa có đầy đủ gallery/ảnh/hồ sơ. Không coi phân công hoặc các file từng phần là toàn bộ công việc đã hoàn tất. User yêu cầu tiếp tục cùng mục tiêu nên giữ SPIN-027, không mở spin mới.

| Actor/phần việc | Trạng thái tại lúc lập hồ sơ |
|---|---|
| Brief/README/prompt trước gián đoạn | File đã có, chưa tự là art acceptance |
| /root/s13e11_direction_resume | Đã audit/cập nhật brief/README thực có sau gián đoạn |
| /root/s13e11_gallery_resume | Đã bàn giao gallery bốn ảnh và thư viện01–10 |
| /root/s13e11_docs_resume | Đã đồng bộ hồ sơ và export inputs cho vòng11 |
| /root | Tạo ảnh, lưu nguồn, tích hợp, kiểm lớp đầu và tổng hợp |

Hai actor thực đã hoàn tất review sau ba worker tiếp tục: `/root/s13e11_critic_code` chạy browser/provenance và `/root/s13e11_critic_visual` xem bốn PNG cùng hai screenshot. Root xem output và screenshot pixel; không nhận automation của critic thành lượt tự chạy. Không claim sáu actor đồng thời hoặc biến self-review thành independent review. Chưa có human playtest, Unity/gameplay acceptance hay audit typography/accessibility đầy đủ.

## Quan sát lớp đầu trên output thật

Root đã xem cả bốn PNG từ công cụ tạo ảnh. Nữ nhân vật chính có tóc pha xám, áo chàm/đai đất/giỏ và dáng người lao động; các nhân vật hỗ trợ có tuổi/biểu cảm khác nhau. Đây là quan sát tạo hình, không xác định tuổi chính xác từ ảnh hoặc chốt generator runtime.

| Board | Điều thực sự nhìn thấy | Ý định chưa được chứng minh hoặc chưa khớp hoàn toàn |
|---|---|---|
| 2.5D nhiều lớp | Tiền cảnh tối, dải đường/người, kho và nền xa phân lớp; chân dung và UI có cùng chất vẽ tay | Cảnh vẫn có phối cảnh/chi tiết dày; chưa có parallax hoặc runtime tách lớp |
| 3D cách điệu | Góc nhìn chếch cao, người/đồ vật có độ khối, UI tối và icon có shading đồng bộ | Kết quả thiên hiện thực nhiều hơn mức cách điệu giản lược trong prompt; không model/rig/camera quay thật |
| Pixel đời làng | Cụm pixel ở người/cảnh/icon và chữ có nhịp điểm ảnh | Chưa kiểm native grid/palette/tileset production; portrait hội thoại là nữ chính, cần làm rõ ai nói khi hiện thực UI |
| Anime đời thường | Góc nhìn qua vai, nét và mảng bóng, cast trưởng thành cùng chân dung có biểu cảm | Palette/cảnh gần bản2.5D; chưa có animation hoặc styleguide production cho mọi độ tuổi/vóc dáng |

Cả bốn có cảnh, tạo hình toàn thân, ba chân dung, vật phẩm và UI hội thoại/túi đồ/nhật ký. Tuy nhiên ba bản đầu chủ yếu cho các góc của nữ chính thay vì đủ hai cast toàn thân riêng như ý định; bản anime có thêm người giữ kho. Không báo khớp100%prompt chỉ vì bố cục mẫu đầy đủ.

Ngày/tháng, minimap, lượng vật phẩm và lời thoại do generation vẽ chỉ là minh họa; không thêm clock, world facts, event hoặc canon. Thích một board chưa quy được riêng cho một trục vì camera, nền và độ chi tiết vẫn thay đổi. Các phiên bản có thể tham khảo/kết hợp, không xếp loại bỏ.

## Kiểm chứng và phản biện

Code critic đã thực sự chạy `python3 prototypes/sprint13/experiment-11/browser-check.py`: lượt cuối exit0 trên Chrome156.0.8078.47 với1280×720,1920×1080,390×844. Trước đó checker cần cập nhật assertion wording của thư viện từ10 sang11; một lượt dừngSIGTERM143 chưa rõ nguyên nhân, không được báo Passed. Sau retry có exit0 mới chốt kết quả kỹ thuật. Đây không phải sửa gallery để né test.

Lượt cuối kiểm bốn PNG decode, SHA256/kích thước/bytes và từng byte khớp original có thật; main/thumbnail contain; bốn nút/phím1–4, card, modal/Escape/focus, linkPNGgốc, HTML150% và mười historylinks01–10 đạt. Không pageerror hoặc requestngoài khi mở filelocal. Evidence ignored: `tests/results/sprint13-experiment11/browser.json` và screenshots. Cỡ chữ chỉ thay HTML, UI bitmap không reflow; trên mobile cần mở ảnh gốc để đọc chữ nhỏ.

[Code review11](CRITIC-CODE-11.md) giới hạn ở gallery, [visual review11](CRITIC-UX-11.md) đánh giá cả bốn board và hai screenshot. Các nhận xét về độ tả thực, layer, speaker và số mẫu cast ở trên giữ theo artifact; không có P1 cản việc xem ý tưởng được phát hiện. Không coi kiểm trình duyệt là nghiệmthu parallax/rig/animation/pixel assets/UI engine. Không xếp hạng loại hướng hoặc chốt thay user.

## Thư viện và bàn giao

Vòng11 bổ sung thư viện01–11. Mọi artwork/hồ sơ01–10 giữ để xem lại hoặc kết hợp; mở vòng mới không mặc định loại hoặc chê bản cũ. [Danh sách cần chốt](APPROVALS.md) tiếp tục giữ quyền quyết định của user, S13-A01–A04 Pending/Changes requested. Không merge dev, đóngSprint13 hoặc chuyểnSprint14.

HANDOFF, sprint brief, approval register, changelog và export source cùng trỏ vòng11. Export source thêm report11 và hai critic11 sau khi file review được tạo; root regenerate/check từ source đã hoàn thiện. Không sửa archive, generated GDD, canon/rule, save/schema/content versions hoặc product VERSION.
