# Critic UX độc lập — Experiment03

28/10/2026; actor `/root/s13_critic_ux`; SPIN-019. Đã đọc art direction/gallery, mở trực tiếp cả ba PNG bằng view_image và xem screenshot browser ở1280/chữ150%. **Technical reviewed về khả năng trình bày so sánh; mọi hướng vẫn Pending user approval.** Nhận xét màu “AI” của chủ dự án về bản02 được giữ nguyên, không bị thay bằng đánh giá tự động.

## Đối chiếu ảnh thực

| Hướng | Khác biệt thật với02 | Đánh đổi/cần chú ý |
|---|---|---|
| A | Camera gần ngang người dưới mái bến, thợ ở trái, thuyền lớn giữa cảnh; trời xám sáng, nước nâu xám, gỗ/vải ít bão hòa. Đổi hình học và ánh sáng rõ, không chỉ grade ảnh02. | Output **gần ảnh chụp/tả thực**, không có chất cọ painterly rõ dù đó là ý định prompt. Chi tiết gỗ/vật liệu dày và tương đối sắc khắp ảnh vẫn có thể tạo cảm giác render. Nón/trang phục/khung nhà chưa xác thực lịch sử. |
| B | Góc chéo cao, bến và lối đất tách khối, mặt nước lam xám; mảng cọ gouache nhìn thấy ở cây/đất/nước. Bố cục gần mô hình không gian, khác hẳn02/A. | Người sửa nhỏ; trọng tâm phân tán sang nhà, lưới, thuyền dưới phải. Độ rõ kết cấu bến thuận lợi để khảo sát layout nhưng không chứng minh collision/navigation hoặc kết cấu thi công hợp lý. |
| C | Mảng than/giấy/đỏ đất, nét khắc hướng theo gỗ/lưới, không có nắng vàng và bóng xanh điện ảnh. Camera gần mặt bến, nhiều hoạt động. | Nét đen nhỏ dày trên lưới, ván và mái dễ tranh chấp với silhouette nhân vật ở thumbnail. Cụm tay/dụng cụ của thợ chưa đủ rõ để chốt asset animation. Có thể hợp minh họa chương/key art; chưa chứng minh đọc lâu ở World tốt. Không gọi đây là tranh khắc Việt cổ xác thực. |

Ở kích thước hiển thị đã xem, không phát hiện chi thừa hoặc hình người đứt gãy đủ rõ để chặn gallery. Đây không phải kiểm anatomy từng pixel. Ba phương án có khác biệt camera/chất liệu/palette đủ để chủ dự án so sánh; không có kết luận “không nhận ra AI”, đẹp hơn hoặc hướng nào thắng thay người dùng.

## Finding cần diễn đạt đúng

**UX03-01/P2:** nhãn chất liệu A “Tranh quan sát” dễ khiến người xem hiểu output đã có chất tranh. Đã báo root đổi thành mô tả kết quả thật như “Tả thực, gần ảnh”; giữ art direction như giả thuyết, không viết lại prompt để khớp output. Không cần tạo lại ảnh chỉ để che chênh lệch này.

## Browser evidence

Chạy thật `python3 prototypes/sprint13/experiment-03/browser-check.py`: passed2viewports1280/1920 và HTTP đối chiếu02. Kiểm decode cả3ảnh, chọn tab/arrow/Home/End, mở toàn ảnh/Escape/focus, so sánh3+hình02, không filter màu, text150% không tràn ngang. Evidence `tests/results/sprint13-experiment03/browser.json` cùng screenshot. Đã xem trực tiếp `compare-150-1280.png` và `single-150-1280.png`: nền trung tính, không HUD che thợ/thuyền, caption và control ngoài ảnh; so sánh dùng toàn tỷ lệ nguồn. Trang dài và cuộn bình thường. Thumbnail C dày nét như nhận xét trên; nút toàn ảnh giúp xem chi tiết.

Gallery chỉ giới thiệu hướng hình ảnh, không tự lưu vote, không nhận nút chuyển tab làm approval. Link02 giữ được artwork/cảnh cũ để so sánh. Không nghiệm thu Unity/gameplay hoặc human playtest. Chưa chốt production direction, tính chính xác lịch sử, cảm giác màu đã đạt hay bước sang Sprint14.

### Kiểm bản sửa UX03-01

Worker đã sửa cả HTML ban đầu và dữ liệu JS của A thành “Tả thực, gần ảnh”, mô tả mở bằng “Cảnh tả thực, gần ảnh…”. Critic đối chiếu trực tiếp hai file sau sửa; không đổi ảnh/B/C. UX03-01 resolved. Không còn finding kỹ thuật chặn đã biết trong phạm vi gallery; chất lượng mỹ thuật vẫn chờ chủ dự án chốt.
