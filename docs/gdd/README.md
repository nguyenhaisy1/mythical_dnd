# GDD chi tiết — Hồng Bàng / Mạch Lạc

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

## Đọc theo module

- [01 — Tầm nhìn, trụ cột và định vị game](modules/01-vision-and-pillars.md)
- [02 — Thế giới, lịch sử, campaign và 12 đại vùng](modules/02-world-and-history.md)
- [03 — Settlement và mô phỏng cộng đồng](modules/03-settlements.md)
- [04 — Dòng họ, gia đình, household và mạng xã hội](modules/04-family-and-social-graph.md)
- [05 — Tâm lý, cá tính và Signature Passive](modules/05-psychology-and-personality.md)
- [06 — Life Event và sinh diễn biến có nhân quả](modules/06-life-events-and-story-generation.md)
- [07 — Sinh nhân vật người chơi ngẫu nhiên](modules/07-player-generation.md)
- [08 — Nghề nghiệp và sự nghiệp](modules/08-profession-and-career.md)
- [09 — Attributes, Skills, Knowledge và Techniques](modules/09-attributes-skills-and-capability.md)
- [10 — Class, Specialization, Skill Tree và Multiclass](modules/10-class-and-progression.md)
- [11 — Party và Companion](modules/11-party-and-companions.md)
- [12 — Combat theo lượt, Zone và Local Position Slots](modules/12-combat.md)
- [13 — Vũ khí, giáp, trang bị, thương tích và trạng thái](modules/13-equipment-injury-and-status.md)
- [14 — Khám phá, di chuyển, sinh tồn và bản đồ](modules/14-exploration-travel-and-survival.md)
- [15 — Dungeon và Encounter](modules/15-dungeons-and-encounters.md)
- [16 — Hội thoại, thương lượng, danh tiếng và phe phái](modules/16-dialogue-social-and-factions.md)
- [17 — Quest, Story Thread và can thiệp của người chơi](modules/17-quests-and-story-threads.md)
- [18 — Kinh tế, thương mại, chế tác và settlement](modules/18-economy-trade-and-crafting.md)
- [19 — Tín ngưỡng, thần thoại, Mạch Lạc và supernatural](modules/19-belief-myth-and-mach-lac.md)
- [20 — Sinh vật, sinh thái và mối đe dọa con người](modules/20-creatures-and-ecology.md)
- [21 — Thời gian, tuổi tác, chết, kế tục và di sản](modules/21-time-death-and-legacy.md)
- [22 — Game Loop, Campaign, World Director và chế độ chơi](modules/22-game-loop-campaign-and-director.md)
- [23 — UI/UX, Journal, Codex, Chronicle và phản hồi](modules/23-ui-ux-and-information.md)
- [24 — Deterministic Engine và AI-assisted Content Pipeline](modules/24-deterministic-engine-and-content.md)
- [25 — Architecture Review và System Dependency Audit](modules/25-architecture-audit.md)
- [26 — Technical System Design và Canonical Data Model](modules/26-technical-design-and-data-model.md)
- [27 — Rule Engine và Formula Specification](modules/27-rules-and-formulas.md)
- [28 — Vertical Slice Implementation Blueprint](modules/28-vertical-slice-blueprint.md)
- [29 — Engine Foundation Implementation Specification](modules/29-engine-foundation.md)
- [06A — Core Life Event Library — nguồn mang tên Module 05A](appendices/06A-core-life-event-library.md)

[Master đầy đủ](MASTER-GDD.md) chứa toàn bộ 29 module và phụ lục; không phải executive summary.
