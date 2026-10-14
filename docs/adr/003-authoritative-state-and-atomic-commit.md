# ADR-003 — Single owner, staged changes, atomic commit

Ngày: 2026-10-13. Trạng thái: baseline áp dụng cho lần bàn giao. Nguồn: DEC-006/009/012; M25–29.

## Bối cảnh

Nhiều hệ cùng nhắc health, stock, route và quest dễ tạo hai nguồn truth. Cross-domain action có thể để lại state nửa vời nếu handler tự ghi.

## Quyết định

Domain sở hữu field, đọc snapshot và đề xuất ChangeSet. Application điều phối single-writer validation/commit. Adapter ghi state/jobs/RNG/events/receipt/revision nhất quán trước publish. Committed event khác proposed request.

## Hệ quả và kiểm chứng

Các critical invariant cùng transaction; rumor/grief xa có thể thành durable follow-up. Cần idempotency, stale-revision và fault injection. Không cần microservices/distributed transaction cho offline game.

## Phương án khác

Bác bỏ mọi subsystem tự cập nhật DB và Quest/Director tự tạo physical truth. Hybrid persistence thay cho giả định journal luôn đủ tái hiện.
