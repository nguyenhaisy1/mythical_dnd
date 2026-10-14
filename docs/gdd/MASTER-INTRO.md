# HỒNG BÀNG — MẠCH LẠC: Game Design & Development Baseline

Phiên bản **1.1-prototype-contracts**, ngày 2026-10-14. Working title; chưa chốt tên phát hành.

Bộ này khôi phục **toàn bộ nội dung dài được chọn làm nguồn cho 29 module**, cộng thư viện Life Event nguyên bản. Các rule, ví dụ, bảng, công thức, pseudo-code, acceptance tests và phần deferred đều được giữ. Mỗi module có hợp đồng tích hợp và ghi chú áp dụng; các thay đổi chuẩn hóa của lần bàn giao này được ghi riêng trong governance/ADR.

Đây là hồ sơ để bắt đầu phát triển theo milestone, không phải chứng nhận game đã hoàn thành. Foundation có thể bắt đầu từ contracts và backlog; các lựa chọn kỹ thuật phải kiểm chứng được đặt thành task có đầu ra rõ. Không mở rộng production hàng nghìn NPC trước khi kiểm chứng vertical slice.

## Cách sử dụng

1. Đọc `docs/governance/APPLICABLE-BASELINE.md`, `docs/production/HANDOFF.md` và `ARCHITECTURE.md` trước khi giao việc.
2. Designer đọc module chi tiết theo hệ thống; programmer đọc thêm Modules 26–29 và `docs/contracts/`; QA dùng ma trận yêu cầu và scenario trong `docs/qa/`.
3. Mỗi thay đổi code/content ghi requirement ID, owner domain, contract version và test evidence. Khi gặp thiếu đặc tả, tạo Design Gap; không tự cho UI, quest hoặc narration quyền sửa World State.
4. Khi nghiên cứu cũ và baseline sau khác nhau, áp dụng bảng quyết định có dẫn nguồn. Nội dung cũ được giữ để bảo toàn công sức và lý do thiết kế, không phải hai specification cùng có hiệu lực.
5. Bản Markdown theo module là nguồn làm việc. Master Markdown/HTML/DOCX là bản đọc được sinh ra; không tạo ZIP; tái tạo bằng `tools/docs/` khi nguồn hoặc ghi chú áp dụng thay đổi.

## Ranh giới bàn giao

Nguyên tắc được chốt: nhân vật random; narrative có nhân quả; gameplay offline bằng rules/code; core độc lập Unity; authoritative mutation có kiểm soát; knowledge isolation; world vẫn sống khi player vắng mặt. Hệ số balance, performance, platform và công cụ persistence cụ thể còn phải prototype. Tài liệu nghiên cứu lịch sử giữ nguồn gốc nhưng không được coi là tái thẩm định khảo cổ.

Bổ sung 14/10/2026: ADR-008 và `docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md` ghi contracts mới cùng trạng thái triển khai. Các ví dụ còn mở trong nguồn cũ phải đọc theo lớp quyết định này.
