# Baseline áp dụng và giải quyết xung đột

Phiên bản 1.1-prototype-contracts. Đây là lớp quyết định chung cho toàn bộ bộ hồ sơ, kể cả khi mở một đoạn nghiên cứu cũ từ Master GDD. [Decision Register](DECISIONS.csv) ghi nguồn, trạng thái và nội dung được thay thế; [Open Decisions](OPEN-DECISIONS.csv) ghi điều kiện cần chốt.

## Thứ tự hiệu lực

1. Yêu cầu hiện tại của chủ dự án; hai yêu cầu lịch sử rõ ràng: random character và no runtime LLM decision.
2. Quyết định tích hợp ghi trong register này, nhằm giải quyết mâu thuẫn khi khởi tạo repository; chúng là lựa chọn bàn giao hiện tại, không giả rằng người dùng đã duyệt từng chi tiết trong hội thoại cũ.
3. Đặc tả chi tiết cuối ở Modules 26–29 và các sửa đổi có chỉ rõ ở Module 25.
4. Nội dung hệ thống Modules 01–24 và phụ lục. Những phương án thảo luận, ví dụ có chữ “đề xuất”, số prototype và roadmap cũ không tự thành yêu cầu phát hành.

Nếu gặp xung đột chưa được register giải quyết, tạo Design Gap với nguồn cụ thể, tác động, phương án và test trước khi thay contract. Không chọn ngẫu nhiên đoạn mới hơn nếu đoạn đó chỉ là ví dụ.

## Ranh giới sản phẩm

Một narrative RPG offline, có thế giới sống, lấy chất liệu Văn Lang–Âu Lạc; character random và class hình thành theo cuộc đời. Gameplay social/profession/exploration ngang hàng combat. Runtime có quyền quyết định nằm trong code/rules/data; không phụ thuộc online service, không có LLM DM. Người viết content có thể dùng AI offline, nhưng runtime không load script tự do.

Narrative và World Director không tạo ra physical truth tùy ý. Một quest hoàn tất khi các facts hợp lệ đáp ứng điều kiện, không phải khi một đoạn văn nói đã hoàn tất. Niềm tin và truyền thuyết có thể sai; committed history không được sửa để khớp lời kể.

## Ranh giới kỹ thuật

World State chỉ có một nguồn có thẩm quyền; mỗi field có owner. Hành động đi qua validation, rule evaluation, proposed changes, invariants và commit. Quy tắc thời gian, RNG, IDs, versions, ordering, save/replay phải được kiểm chứng trước khi thêm nhiều content. UI chỉ nhận projections, không nhận snapshot omniscient rồi tự giấu text.

Simulation core có thể biên dịch độc lập Unity. .NET 10 SDK chỉ phục vụ build/host, không biến .NET 10 thành runtime của Unity. Các thư viện portable target .NET Standard 2.1. SQLite nằm sau adapter; chưa cài một provider rồi coi cross-runtime problem đã giải quyết.

## Quyết định địa lý và fixture

Bến Vạc = TG-01/Tam Giang. Bến Hạc = PC-02/Phong Châu. Vertical slice tập trung playable content tại Tam Giang; Bến Hạc là external boundary node để kiểm tra trade, travel time và tin tức. “Một vùng” nói về phạm vi playable sâu, không có nghĩa mọi endpoint phải nằm trong một vùng.

Bản nguồn M28 nhắc household “Tòng Mộc”; M04/M05 có gia đình Vạc Tòng và nhân vật Vạc Mộc. Fixture dùng ID riêng không phụ thuộc tên; household Vạc Tòng, Vạc Mộc là thành viên. Canonical display names có thể được narrative team rà lại mà không phá ID. Đây là sửa lỗi tích hợp đã ghi DEC-019, không thay nội dung archive.

## Ranh giới hoàn thành

“Đủ tài liệu để bắt đầu Foundation” không đồng nghĩa “không còn quyết định kỹ thuật”, “đã balance” hay “đã có 1.000 NPC runtime”. Gói này cung cấp scope, nguồn chi tiết, quyền sở hữu, thứ tự thực hiện và exit gates. Các lựa chọn còn mở có chủ đề và task chốt; không cần viết thêm Module 30 để bắt đầu kiểm chứng.

## Bổ sung baseline prototype 1.1 — 14/10/2026

[ADR-008](../adr/008-prototype-contract-baseline.md) và DEC-024–027 có hiệu lực tại lớp quyết định tích hợp: identity-v1; tick một phút; lịch simulation360 ngày; sha256-counter-v1; codec mdc1; reserve/consume repair; onboarding prototype; gates tách headless/Unity. Đây là quyết định biên soạn mới để lấp gaps, không sửa archive hoặc giả rằng nguồn cũ đã chốt chúng. Chi tiết và trạng thái thực hiện ở [báo cáo bổ sung](../production/DOCUMENTATION-COMPLETION-2026-10-14.md). Tài liệu duy trì trực tiếp trong project, không tạo ZIP.


## Mỹ thuật và cổng mở lại — quyết định chủ dự án03/11/2026

[ADR022](../adr/022-earth-monument-2d-and-reopen-gate.md), DEC-028/029 và [baselineG13](../production/sprints/sprint-13/ART-DIRECTION-BASELINE.md) có hiệu lực trước các đề xuất3D hoặc trạng tháiPending Sprint13 cũ. Chốt hướng Đất dựng thành bằng nền2D có cảm giác khối/nhân vật sprite (`earth-monument-2d-v1`); Sprint13Accepted/Reopenable theo phạmviđịnhhướng. S13-A02/A03UI/UX chi tiết vẫn là follow-up chưa nghiệmthu, không cản lệnh đóng phạm vi của user và không thành runtime acceptance.

G13 mở lại mỹ thuật bằng candidate/profile revision trên code mới nhất, giữ core/rules/content/stable IDs/save contracts và tiến độ. Không reset/revert toàn bộ lịch sử Sprint1–12/vềsau; không đổi schema/domain chỉ để hợp camera. Nếu ảnh hưởng contract/save thật phải có impact/migration/acceptance riêng. Thư viện/source/checkpoint được giữ. Những dòngPending lịch sử không là trạng thái hiện hành; không coi approval này tự mởSprint14 hoặc tạo release.


## Bổ sung thực thi Sprint14 — 03/11/2026

[ADR023](../adr/023-bounded-boat-repair-profile.md), DEC-030 và [Boat Repair Profile](../contracts/BOAT-REPAIR-PROFILE.md) chốt implementation bounded VS-03A được chủ dự án giao riêng sau Sprint13: deterministic reserve/time/complete/cancel, trusted capability/tool inputs và explicit repair image/rule/content profile. Scope2timber/60ticks/+200condition/cap1000 là prototype engineering, không balance hoặc full profession.

Committed image2 khi có Repair extension, legacy image1 giữ bytes; physical SQLite schema2 không đổiDDL. Save/replay phải có exact rule/content/profile/recipe, không auto-convert world cũ. Khôi phục phải giữ receipt/fact causality và activity/job/hold consistency. Acceptance thực tại [evidence Sprint14](../qa/SPRINT-14-EVIDENCE.md); không suy đã nghiệm thu Unity/client2D/fullVS03 từ contract. G13/mỹ thuật đã chốt và tiến độ trước giữ nguyên; thay core ở sprint được giao không là rollback về checkpoint13.
