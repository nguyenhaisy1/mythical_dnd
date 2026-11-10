# Baseline áp dụng và giải quyết xung đột

## Bổ sung personal weaving Sprint17 — 10/11/2026

[ADR026](../adr/026-personal-weaving-on-origin.md), DEC-033 và [Personal Weaving Profile](../contracts/PERSONAL-WEAVING-PROFILE.md) chốt subset personal handcraft trên origin đã nhận. `personal-weaving-v1`: dùng tay2fiber/30ticks/1cord/minweaving40; Lâm43/Nhu61 đủ điều kiện, sáu xuất thân khác chưa có công việc phù hợp trong bản thử. Không sửa origin/random selection hoặc grant nghề/household rights;8starterfiber thuộc cá nhân là prototype genesis mới, không tài sản lịch sử hoặc balance cuối.

Origin+Weaving canonical image4 dùng exact new rule/content/root; origin cohort/source hash giữ nghĩa cũ, metadata/genesis immutable; image1/2/3/goldens/scenes15–16 và physicalSQLite schema2 giữ. Không upgrade/enrich save16 âm thầm; replay artifactv1 vẫn từ chối. Typed activity/job Start/reserve→due consume/output hoặc cancel, receipt/causal facts/holds/tick commit cùng nhau, due revalidation và original-envelope retry; Continue drain workflow Advance đã nhận, không tạo intent mới. Safe session chỉ self lots/profile được quan sát explicit, không raw world/seed/IDs/private revision; ownership không cấp general Knowledge. Factory client bounded own actor tại home/hai lot, không là full Travel/rights/Health support.

Năm gates trong contract/evidence17 bắt buộc actual core/SQLite crash/recovery/idempotency, actual Unity6000.6.7f1/LinuxMono hai qualified+một nonweaver, actual scene15–16 Player regression, six renderer layouts/nativeOS/two-process restart, independent reviews/fullsuite/docs/exports/exactHEADCI/PRdev trước CLOSE. [Runbook](../runbooks/PERSONAL-WEAVING.md) mô tả lệnh; [evidence](../qa/SPRINT-17-EVIDENCE.md)/WORKLOG ghi trạng thái thực. Không suy Passed từ source/focused tests hoặc chuyển gate chưa đạt sang backlog. Không full professions/three-profession VS03/fullgenerator/CharacterViability/worldNPCbinding; G13v1/01–18 giữ, không tự Sprint18/main/tag/release. Sprint16 đã đóng09/11/PR17/dev`e7dfd9b`; các lệnh không tự Sprint17 dưới đây là giới hạn lịch sử đã được yêu cầu mới thay thế.

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

## Bổ sung client2D Sprint15 — 03–04/11/2026

[ADR024](../adr/024-bounded-2d-repair-client.md), DEC-031 và [Repair Client Profile](../contracts/REPAIR-CLIENT-PROFILE.md) chốt proof kỹ thuật `repair-client-2d-v1` tại Bến Vạc. Trusted composition cấp actor/worksite/boat/lot/tool và direct-observation grants theo field; ownership không tự là knowledge. DTO deny-default, Unknown thật, preview chỉ dùng known inputs, opaque token giữ private revision/tick, retry giữ original envelope. Presenter không có WorldSnapshot/raw receipts/seed hoặc quyền ghi authoritative state. Local footpoint/animation/camera chỉ là presentation, không Travel/WorldTick.

Một actor cố định phải được ghi rõ trên UI; đây là phạm vi kỹ thuật, không thay yêu cầu random-life của sản phẩm hoặc mở cho chọn origin. G13/profileearth-monument-2d-v1 giữ nguyên. Actual Editor6000.6.7f1 bootstrap và LinuxMono Player build/run/captures/input là gates sprint; acceptance thực xem [evidence Sprint15](../qa/SPRINT-15-EVIDENCE.md). Full Knowledge/Perception/world binding/three professions/fullVS03/VS07, final UI/art approval/performance/human playtest là backlog dự án ngoài sprint. Báo cáo tách **Điều kiện đóng Sprint15 còn thiếu** khỏi backlog; không đẩy gate đã nhận ra ngoài scope để tự đóng.

## Bổ sung origin cohort Sprint16 — 06/11/2026

[ADR025](../adr/025-seeded-origin-cohort-and-reveal.md), DEC-032 và [Origin Cohort Profile](../contracts/ORIGIN-COHORT-PROFILE.md) chọn nhánh M07 lấy một existing authored adult theo world seed: `origin-cohort-v1`, tám candidate tại Bến Vạc. Không point-buy/chọn origin/reroll ở Reveal; Begin là presentation, Continue giữ assignment và cursor. Đây VS02C + partialVS07, chưa full life-history generation/Character Viability/TEST001 hoặc VS04 Knowledge/narrative.

Metadata origin immutable trong canonical image3, exact cohort/source/rule profile; physicalSQLite2 và legacy image1/repair image2 giữ. Trusted gateway tạo pending/verify/flush/create-new publish, giữ older campaign và idempotent requestId; cancel/fault/crash trước publish không đổi Continue, corrupt/incompatible latest fail closed. Không sidecar assignment. Replay artifactv1 từ chối origin cho đến khi có profile riêng; save/recovery không là command replay.

Safe DTO chỉ own history/skills/claims/needs, không seed/raw IDs/registry/world truth. Cohort không có boat-repair: không cấp qualification/rights giả hoặc dùng repair factory15 cho origin world. World chỉ self-record/time/save; scene15 có actual Player regression riêng. Giữ G13v1/assets15, marker trung tính không nhận đủ tạo hình tám người. Năm gate16 tại [evidence](../qa/SPRINT-16-EVIDENCE.md) cần đủ actual runtime/Unity/OS/render/independentreview/regression/docs/exports/CI/PRdev trước đóng; gate thiếu không thành backlog. Full generator/world binding/finalart/humanplaytest còn riêng, không tự Sprint17/main/tag/release.
