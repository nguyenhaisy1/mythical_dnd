# Phản biện thiết kế và sản phẩm thử nghiệm

Đọc khi được giao review gameplay/visual/UX, GDD feature hoặc kết quả prototype. Adaptation chọn lọc Design Review từ Claude Code Game Studios, [nguồn và license](../../upstream/DESIGN-SOURCES.md). Dùng cùng scope/evidence của skill chính; không tự implement khi chỉ được giao review.

## Review đúng câu hỏi

Xác định quyết định đang cần kiểm chứng và artifact/version được giao. Đọc trọn đơn vị thiết kế liên quan, dependencies được nó viện dẫn và baseline cần thiết; không nạp toàn29module. Không tự chặn vì thiếu heading của template upstream nếu nội dung đã đủ ở contract khác.

Với gameplay: người chơi muốn gì, có thể chọn gì, biết điều gì trước khi chọn, nhận feedback nào và hậu quả có ý nghĩa ra sao? Kiểm core loop và nghề/social/exploration theo pillars, không ép progression chiến đấu vào mọi RPG. Phân biệt lựa chọn có trade-off với nút chỉ đổi lời kể.

Với hệ thống: input/output, ownership, prerequisites, units/ranges, failure/cancel/death/resource-loss và acceptance có đủ để triển khai không? Công thức minh họa chưa được balance; chỉ ra assumption cần thử thay vì mặc nhiên chốt thành luật. Đối chiếu refs/canon/contracts với nguồn chuẩn của project.

Với hình ảnh/UX: đọc cả art direction, dữ liệu/mô hình và artifact hiển thị. Đặt câu hỏi từ mục tiêu trải nghiệm rồi thực sự thao tác/xem hình; kiểm rằng thông tin cần đánh giá đã được render, không chỉ có trong model. Review bản sắc, hierarchy, silhouette, contrast, typography và phản hồi. Nhận xét “tôi thấy chưa phù hợp” cần dẫn chi tiết artifact và tiêu chí, không giả là ý kiến user.

## Bằng chứng và findings

Ghi finding theo severity, vị trí, cách tái hiện/bằng chứng, tác động và hướng sửa. Ưu tiên lỗi làm sai rule, lộ knowledge, mất lựa chọn, feedback gây hiểu sai hoặc không thể thử giả thuyết. Phân biệt lỗi xác định được với preference và câu hỏi còn mở; không dùng bất đồng thẩm mỹ để giả P0 kỹ thuật.

Đọc code không thay runtime test; DOM không thay nhìn ảnh; automation không thay human playtest. Không chạy được thì ghi Not assessed cho phần đó. Reviewer độc lập phải là actor thực đã chạy, root self-review ghi đúng tên.

Sau sửa, đối chiếu artifact hiện tại và tái kiểm finding cùng những đường bị ảnh hưởng. Reuse evidence cũ chỉ khi nội dung, dependencies và phạm vi review còn phù hợp; ghi revision/hash hoặc đường dẫn artifact. Không tuyên bố đã rà lại toàn bộ nếu chỉ xác nhận một fix. Nếu còn lỗi chặn, ghi chưa đạt và remediation cụ thể.

Kết luận tách ba lớp: kiểm kỹ thuật; mức bằng chứng trải nghiệm; quyết định chủ dự án. Đối với Sprint13, Technical reviewed không bao giờ tự chuyển Approved trong approval register. “Tiếp tục” không phải chốt sản phẩm. Không tự sửa lịch sử báo cáo hoặc bỏ pending để chuyển sprint.
