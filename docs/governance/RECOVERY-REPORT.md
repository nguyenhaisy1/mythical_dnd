# Báo cáo khôi phục chi tiết

Phiên bản 1.1-prototype-contracts. Có 29 module và một phụ lục event, lấy từ 31 khối nguồn dài. Tổng nội dung sau khi bỏ wrapper/citation: **1,436,697 ký tự**; **6,156 đề mục** được đánh anchor. Đây là kích thước văn bản, không phải thước đo chất lượng hoặc tiến độ code.

`python3 tools/docs/build_docs.py --check` so khớp toàn bộ file sinh lại với nguồn và ghi chú tích hợp. Checksum từng thông điệp nguồn được xác minh trước khi build. So khớp này phát hiện mất nội dung do đóng gói, không chứng minh mọi công thức/thiết kế đã đúng qua prototype.

**Đã xử lý:** mất chi tiết do summary, đánh số nguồn chưa nhất quán, bản WritingBlock trùng, thiếu index, thiếu trạng thái quyết định và thiếu lối nối từ tài liệu tới module code/test.

**Giới hạn còn lại:** các file sandbox cũ không phải nguồn của bản này; game balance và lịch sử chưa được kiểm chứng thực nghiệm; runtime và full-production content chưa được triển khai. Xem [Open Decisions](OPEN-DECISIONS.csv) và [Handoff](../production/HANDOFF.md).
