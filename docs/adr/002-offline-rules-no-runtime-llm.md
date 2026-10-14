# ADR-002 — Runtime bằng code; AI chỉ author offline

Ngày: 2026-10-13. Trạng thái: baseline áp dụng cho lần bàn giao. Nguồn: DEC-002/017; M24/27/29.

## Bối cảnh

LLM sinh sự thật realtime khó bảo đảm consistency, reproducibility và causal limits. Người dùng yêu cầu engine code/formulas, kể cả có hàng triệu tổ hợp.

## Quyết định

Không thêm runtime LLM service. Characters/events/outcomes/NPC planning do approved rules/data. Offline AI output phải qua authoring review, schema, semantic validation và versioned pack.

## Hệ quả và kiểm chứng

Templates chỉ gọi handler IDs. No arbitrary code/SQL từ text. Text presentation không grant item/skill/knowledge. Content volume là production work riêng; engine không được lấp gap bằng LLM.

## Phương án khác

Bác bỏ AI dungeon master sửa state; runtime narration LLM từng là optional idea không nằm trong baseline triển khai hiện tại.
