# Phản biện hình ảnh08 — nét, sợi và hình khối

29/10/2026 · Sprint13/SPIN-024 · Reviewer độc lập `/root/s13e8_critic_visual`. **Các phiên bản tham khảo bổ sung; Pending user approval.** Review này không chọn bản thắng, loại bản cũ hoặc kết luận một ý tưởng kém vì chưa được chọn.

## Bằng chứng đã xem

Đã đọc baseline áp dụng, hướng dẫn mythical-validation/game-design-review, brief08 và cách trình bày gallery. Đã thực sự mở ba ảnh gốc `assets/a-cel.png`, `assets/b-woven.png`, `assets/c-faceted.png` trong `prototypes/sprint13/experiment-08/` bằng công cụ xem ảnh; cả ba1672×941. Đã xem thêm `tests/results/sprint13-experiment08/gallery-b-1280.png` và `hud-off-1280.png`.

Đây là quan sát ảnh và screenshot, không phải lượt browser automation do reviewer tự chạy. Không có kiểm nghiệm model, geometry, y học, khảo cổ, Unity hoặc gameplay. Diện mạo3D ở C không chứng minh model thật tồn tại.

## Điều thấy được và giá trị tham khảo

| Phiên bản | Quan sát từ ảnh thực | Điều có thể tham khảo tiếp |
|---|---|---|
| A — Nét hoạt hình | Nét viền đọc rõ ở nhân vật, giỏ và các thanh tre; bóng trên áo/quần chia mảng tương đối dứt khoát. Nhân vật mang giỏ lớn ở tiền cảnh dẫn mắt tới ba người trên hiên. | Cách dùng nét để giữ cử chỉ và cá tính người trong một môi trường còn giàu chi tiết. |
| B — Thế giới dệt | Cây, lá, mái, nhà, đường và nước đều có cấu trúc sợi/mũi thêu nhìn thấy được; lối đi sáng tạo khoảng nghỉ giữa mảng cây dày. Người vẫn có tư thế riêng, không đọc như những búp bê nhồi bông tròn. | Một ngôn ngữ vật liệu khác mạnh: toàn thế giới có thể gợi cảm giác được dệt thành, thay vì chỉ thêm texture vào nền. |
| C — Hình khối | Đá, thân cây và một số lá có mặt phẳng/đổi hướng sáng rõ; đường đất, bậc lên nhà và cầu tạo chiều sâu dễ nhận. Nhân vật giữ tỷ lệ người và hoạt động đời thường. | Cách diễn tả không gian bằng khối và giá trị sáng tối, không cần nét viền đậm ở mọi vật. |

Cả ba ảnh giữ được tình huống chung: người mang giỏ đi tới nhà, một người sắp xếp đồ trên hiên, người cao tuổi và người nhà đang đợi; suối và cầu nằm bên phải. Không có hiệu ứng chữa lành hoặc phép thuật hiển nhiên. Điều này giúp so sánh cách cảm nhận cảnh, không chứng minh hoạt động chữa trị hay tuyến đường đã được lập trình.

## Đánh đổi và câu hỏi thiết kế

**A và C hiện gần nhau hơn B.** Cả hai cùng có mái vàng, cây xanh, đường đất ấm và bề mặt khá nhiều chi tiết. A có nét/bóng rõ hơn; C nhấn mặt khối ở đá/cây. Nếu muốn khám phá khoảng cách xa hơn giữa hai hướng, một phiên bản tiếp theo có thể thử nền A giản lược rõ hơn hoặc các khối C lớn, ít mặt hơn. Đây là hướng mở rộng thư viện, không phải yêu cầu thay hoặc sửa mất hai ảnh hiện tại.

**A chưa hoàn toàn là cảnh hai–ba mảng bóng gọn.** Nền cây, đá, mái và nước vẫn nhiều vệt/mảng nhỏ, nên nét hoạt hình nổi ở người hơn ở toàn môi trường. Có thể giữ đặc điểm này nếu muốn pha nhân vật vẽ tay với nền giàu chất liệu; nếu muốn đồ họa phẳng hơn, cần thử mức giản lược riêng.

**B nghiêng về thêu nổi và đan sợi hơn một tấm dệt phẳng.** Các nút lá, thân cây và lớp mái có độ nổi; toàn cảnh dày vật liệu. Đây có thể là nét hấp dẫn của hướng này, đồng thời đặt câu hỏi về việc giữ người và khuôn mặt nổi bật khi ảnh bị thu nhỏ. Ở thumbnail gallery, người trên hiên nhỏ hơn nhiều so với mảng mái/cây. Không suy từ độ nổi thành lỗi plush hoặc tuyên bố sợi thật đã được sản xuất.

**C không đồng nhất mức cách điệu giữa mọi vật.** Đá/thân cây có mặt phẳng rõ, nhưng mặt người, tre và mái mềm, chi tiết hơn. Người xem có thể thấy đây là một cách pha trộn dễ tiếp cận; nếu muốn bản sắc hình khối mạnh hơn, nên thử độ nhất quán trên cả người lẫn môi trường. Không có dữ liệu chi phí, hiệu năng hoặc tính khả thi runtime để xếp hạng hướng nào dễ làm hơn.

Không phát hiện vấn đề hình ảnh mức P1 ngăn xem và so sánh ý tưởng trong phạm vi quan sát. Các điểm trên là độ khớp brief, đánh đổi và câu hỏi thị giác; không phải quyết định chê hoặc loại một phiên bản.

## Trình bày trong thư viện

Hai screenshot đã xem cho thấy tranh chiếm phần chính, nhãn ảnh mô phỏng rõ; HUD trung tính không hứa đã chữa trị. Screenshot tắt HUD giữ nguyên ảnh gốc, cho xem trọn lối đi. Mục “Thư viện tham khảo — mọi phiên bản được giữ lại” và liên kết01–07 thể hiện đúng chỉ dẫn mới của chủ dự án; không có nhãn bản cũ bị bỏ hoặc xếp hạng ngầm trong phần đã quan sát. Chưa tự thao tác mobile, phím tắt hoặc modal; phần đó thuộc evidence riêng của critic code.

Một ý tưởng kết hợp để tham khảo: lấy cách phân biệt người bằng nét ở A, nhịp sợi chọn lọc từ B và cách tổ chức khối/lối đi ở C. Chưa thử kết hợp nên chưa kết luận sẽ thành công; cũng không cần kết hợp nếu chủ dự án muốn giữ ba hướng độc lập.

Các ảnh đủ để bổ sung thư viện tham khảo, không có đề nghị thay thế01–07. Mọi hướng vẫn chờ quyết định rõ của chủ dự án; lời khen hoặc mở vòng mới không tự chốt mỹ thuật, đóng Sprint13 hay chuyển Sprint14.
