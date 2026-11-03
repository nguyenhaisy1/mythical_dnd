# Phản biện hình ảnh11 — không gian và ngôn ngữ tạo hình

30/10/2026 · Sprint13/SPIN-027 · Reviewer độc lập `/root/s13e11_critic_visual`. **Các phiên bản tham khảo, Pending user approval.** Chỉ review hình ảnh, không gọi generation hoặc sửa gallery.

## Bằng chứng đã quan sát

Đã đọc baseline, HANDOFF, mythical-validation/game-design-review, art direction11 và nội dung gallery. Đã thực sự mở cả bốn PNG gốc tại `prototypes/sprint13/experiment-11/assets/`: `layered-25d.png`, `stylized-3d.png`, `pixel-life.png`, `anime-life.png`. Cũng đã mở hai screenshot `tests/results/sprint13-experiment11/gallery-pixel-life-1280.png` và `gallery-stylized-3d-1280.png`.

Reviewer không tự chạy browser, OCR hoặc đo contrast; không nhận automation của code critic là lượt kiểm của mình. Quan sát screenshot chỉ xác nhận cách trình bày đã thấy, không đại diện mọi thiết bị hoặc trạng thái tương tác.

## Sự nhất quán và khác biệt nhìn thấy

| Hướng | Quan sát xuyên cảnh, người và UI | Đánh đổi/độ khớp ý định |
|---|---|---|
| 2.5D nhiều lớp | Tiền cảnh lá/cọc tối, đường giữa có cuộc gặp, kho và sông ở các lớp sau. Dáng nữ lao động, nét mặt, icon vẽ và panel sáng cùng ngôn ngữ tranh; lối tới kho đọc được. | Có gợi ý phân lớp nhưng cảnh vẫn giàu chiều sâu phối cảnh và chi tiết. Một ảnh tĩnh không chứng minh các lớp đã tách hoặc có parallax; cần phân biệt ý tưởng bố cục với giải pháp kỹ thuật. |
| 3D cách điệu | Góc nhìn nâng cao cho thấy mặt đất, kho, cầu và nhóm đang nói. Thể tích, bóng, da/vải và icon có diện mạo gần render hiện thực; portrait và người toàn thân giữ tuổi, trang phục tương ứng. Panel tối tách khỏi cảnh. | Output thiên tả thực mạnh hơn brief matte/hình khối giản lược. Đây là một bản tham khảo cho cảm giác3D, chưa đủ để chốt mức cách điệu mong muốn hoặc nói pipeline3D đã có. |
| Pixel đời làng | Cạnh bậc thang/cụm pixel xuất hiện ở mái, người, chân dung, icon, khung và chữ. Dáng người, giỏ và hoạt động quanh kho phân biệt được; UI tối/vàng tạo nhóm thông tin rõ. | Mật độ chi tiết vẫn cao. Chưa kiểm grid/palette kỹ thuật hoặc khả năng tách thành tileset/sprite; ảnh concept không tự là bộ asset production. Portrait thoại là người nữ chính trong khi hai người đang trao đổi, nên danh tính người nói còn cần làm rõ nếu chuyển thành UI thực. |
| Anime đời thường | Góc qua vai đưa người xem gần cuộc gặp; nhân vật có tóc bạc, nét mặt trưởng thành, nếp áo và dáng làm việc. Mẫu toàn thân/portrait/icon/panel giấy dùng cùng nét vẽ và màu. Có mẫu người giữ kho toàn thân cùng nữ chính. | Palette và độ dày nền khá gần bản2.5D, nên khác biệt chủ yếu còn ở góc nhìn, nét/cách diễn mặt. Không coi nhãn anime là bằng chứng mọi trạng thái biểu cảm/animation đã được thiết kế. |

Phụ nữ trưởng thành với áo chàm, đai đất và giỏ xuất hiện rõ trong cả bốn board; người giữ kho và hàng xóm có tuổi/cử chỉ riêng. Đây là thay đổi hữu ích trong phạm vi mẫu cast, không buộc game có protagonist cố định hoặc cho người chơi chọn tuổi/nghề/origin.

Cả bốn board có cảnh chơi, mẫu toàn thân, ba chân dung, sáu vật phẩm và UI hội thoại/túi đồ/nhật ký. Tuy nhiên, **không phải cả bốn đều có hai nhân vật toàn thân riêng như brief**: bản2.5D/pixel chủ yếu cho ba góc của nữ chính, bản3D cho hai góc của nữ chính; bảnanime có nữ chính và người giữ kho. Không gọi việc có nhiều góc cùng người là đủ hai cast khác nhau. Phạm vi minh họa tổng thể đã có, thư viện cast toàn thân vẫn có thể mở rộng sau.

## Độ đọc và câu hỏi UI

Nhiều nhãn lớn, dòng thoại và lựa chọn đọc được ở ảnh gốc. Các đoạn nhật ký, số lượng vật phẩm hoặc chi tiết nhỏ cần mở PNG gốc để zoom. Chữ UI trong bitmap không reflow theo cỡ chữ HTML; chưa có audit từng dấu tiếng Việt, font, contrast hoặc accessibility game. Các nút trong artwork không hoạt động.

Ở hai screenshot desktop đã xem, board được giữ trọn, không có một HUD HTML chung phủ che thiết kế UI riêng. Gallery giữ nhãn ảnh minh họa và các liên kết thư viện cũ. Chi tiết được thu nhỏ trong khung xem nên quan sát cả thumbnail, ảnh chính và PNG gốc có mục đích khác nhau; không suy từ screenshot desktop ra độ đọc ở mọi màn hình.

Các minimap, ngày/tháng, thời tiết, lời thoại và entry trong ảnh là nội dung minh họa. Chúng không chứng minh WorldState, lịch, tri thức nhân vật hoặc trạng thái cây cầu đã được triển khai. Câu hỏi người nói ở bảnpixel là một điểm cần xác định cho UI tiếp theo, chưa là lỗi authoritative state hoặc một lý do loại concept.

## Kết luận trong phạm vi review

Không phát hiện P1 cản người xem so sánh các concept trong phạm vi đã quan sát. Những điểm trên là đánh đổi và mức khớp brief, không đánh giá thấp toàn bộ hướng hay các phiên bản01–10.

2.5D/3D mô tả cách tổ chức không gian; pixel/anime là ngôn ngữ tạo hình và có thể kết hợp. Camera, bố cục và mức giản lược cùng đổi giữa các board nên chưa thể quy sở thích chỉ cho một yếu tố. Không có bằng chứng mesh, rig, camera tự do, parallax, animation, performance, chi phí production, khảo cổ hoặc human playtest. Quyền chốt vẫn thuộc chủ dự án; review này không nghiệmthu Sprint13 hoặc chuyển Sprint14.
