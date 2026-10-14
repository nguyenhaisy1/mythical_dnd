# ADR-005 — Knowledge isolation và exposure-only Director

Ngày: 2026-10-13. Trạng thái: baseline áp dụng cho lần bàn giao. Nguồn: DEC-004/005/013; M16/21/23/25–29.

## Bối cảnh

Actor không biết mọi truth engine cần cho physics. UI/search và succession dễ vô tình tiết lộ thông tin hoặc grant abilities từ ký ức player.

## Quyết định

PerceivedState/KnowledgeProjection là boundary cho NPC decisions và player UI. Claims có nguồn; Director chỉ xếp hạng exposure hợp lệ. Player Experience Archive khác Active Character Knowledge.

## Hệ quả và kiểm chứng

Query APIs không trả hidden fields rồi nhờ UI tự giấu. Rumor dedupe xét origin lineage, không coi mỗi lần nghe là một chứng cứ độc lập. Succession không clone knowledge/assets.

## Phương án khác

Bác bỏ omniscient query ở decision/UI và Director sửa facts để cân pacing.
