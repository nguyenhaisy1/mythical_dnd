# Kế hoạch triển khai từ bộ tài liệu

## Sprint16 — nghiệm thu kỹ thuật local09/11/2026: VS02C và một phần onboarding VS07

Kết quả local09/11:674/674runtime tests,0failed/skipped; actual Unity6000.6.7f1/LinuxMono hai seed,38ảnh origin/sixlayouts mỗi seed, OS20actions/hai process restart và actual repair15 regression đạt. Full evidence1rngpl9l, renderer8fpgghix, OS22lns4xz; docs/exports và exactHEADCI/PR/dev vẫn bắt buộc trước CLOSE, trạng thái Git cuối ở WORKLOG.
[Brief16](sprints/SPRINT-16.md), [ADR025](../adr/025-seeded-origin-cohort-and-reveal.md), [contract](../contracts/ORIGIN-COHORT-PROFILE.md) và [runbook](../runbooks/ORIGIN-CLIENT.md) chia S16-01 typed selector/origin/RNG; S16-02 cohort registry/canonical image3/SQLite/session; S16-03 Unity menu/Generating/Reveal/World; S16-04 integration/process/Player/input/regression/docs/Git; S16-QA independent reviews. Ownership thực và kết quả ở WORKLOG, không coi vai trò backlog là nhân sự đã chạy.

M07 cho phép chọn existing authored character theo world seed. Admission hiện tại chỉ alive/adult/home Bến Vạc/refs hợp lệ; tám candidates không tương đương full Character Viability. Đây không là VS04 hoặc full VS02/VS07. Cohort không có boat-repair nên không grant giả; scene15/factory/image2 giữ riêng, actual Player regression là gate16. Origin world chỉ self-record/time/save/Continue, neutral marker và assets15 theo G13v1.

Five-gate acceptance gồm known answers/privacy/idempotency; exact image3/SQLite cancel-fault-crash/pending publish; actual Unity6000.6.7f1/LinuxMono hai seed/restart; six layout/scroll/focus/modal/OS keyboard-mouse và repair15 Player regression; independent source/artifact reviews/full regression/docs/exports/CI/PR→dev. Kết quả cuối ở [evidence16](../qa/SPRINT-16-EVIDENCE.md). Gate chưa đạt phải nằm ở Điều kiện đóng Sprint16 còn thiếu; fullgenerator/viability/worldbinding/finalart/performance/humanplaytest thuộc Backlog dự án ngoài sprint. Không tự Sprint17/main/tag/release.

Dùng [Backlog](BACKLOG.csv) làm danh sách task có dependency và acceptance. FND-01 và DOC-01 đã có từ lần khởi tạo. Sprint 1 đã triển khai FND-02/03/04; Sprint2 có FND05–08 in-memory TransferResource; Sprint3 đã có event/causal journal và idempotent consumer; Sprint4 triển khai scheduler in-memory; Sprint5 có SQLite atomic current-state commit/reopen cho headless. Sprint6 triển khai FND12 snapshot/recovery/new-file migration; Sprint7 triển khai FND13 headless replay QA; Sprint8 triển khai FND14 content registry profile kernel; Sprint9 triển khai FND15 diagnostics; Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Nghiệm thu Sprint10 chỉ bao phủ kernel Editor, không Player/IL2CPP hoặc full gameplay. Bounded LinuxMono repair Player đã nghiệm thu riêng Sprint15; origin client16 theo gates/evidence riêng. Unity crash/recovery/migration rộng hơn, IL2CPP và full campaign vẫn chưa nghiệm thu. Headless evidence và các gate còn pending được ghi trong backlog. Module 28 giữ blueprint đầy đủ và Module 29 giữ 16 task Foundation nguyên bản.

## Sprint13 đã chốt hướng; cổng G13 giữ khả năng mở lại — 03/11/2026

Hướng hiện hành là **`earth-monument-2d-v1`: Đất dựng thành, nền 2D có cảm giác khối và nhân vật sprite**. Đọc [baseline mỹ thuật/G13](sprints/sprint-13/ART-DIRECTION-BASELINE.md), [ADR-022](../adr/022-earth-monument-2d-and-reopen-gate.md) và [approval register](sprints/sprint-13/APPROVALS.md). Chốt định hướng không xác nhận sprite/scene/UI runtime, origin generator, save trong Unity hay human playtest đã đạt. S13-A02/A03 là follow-up UI/Reveal chưa nghiệm thu; giữ random origin, không chỉnh xuất thân và không reroll tại Reveal.

**Accepted / Reopenable** cho phép trở lại bài toán mỹ thuật ở Sprint13 mà giữ tiến độ phần độc lập. Mở candidate/profile mới từ code hiện hành, chỉ thay phần asset/camera/presenter/UI chịu ảnh hưởng, bảo toàn core/rules/content/stable IDs/save theo contract. Không reset repository về Sprint1, không xóa các thử nghiệm01–18 và không lấy bản engine cũ đè lên tiến độ mới. Nếu thay đổi ý nghĩa gameplay/schema thì cần ADR/impact/migration và acceptance riêng trước khi tích hợp.

Khi được giao implementation, task đầu phải xác định contract vùng đi, mặt/tầng, che khuất, tỷ lệ sprite và camera cố định theo cảnh; query/command vẫn thuộc Application, thứ tự vẽ không tự là world truth. Áp dụng ngôn ngữ hình ảnh vào Bến Vạc/Tam Giang cho vòng sửa thuyền, không dời địa danh sang công trường trong ảnh14. Phải re-estimate đường2D sau kiểm nhỏ; ROM3D SPIN-036 không dùng làm budget/deadline2D. Quyết định này không tự mở Sprint14 hoặc thay các dependency/exit gate VS-01–10.

## Trình tự bắt đầu

1. FND-02/03/04 hiện thực identity/time/RNG theo ADR-008 và specs/vectors đã bổ sung; không chọn lại wire codec hoặc tick unit trong từng domain.
2. FND-05/06/07/08 dựng read-only state → typed rule → command → staged transaction. Dùng một hành động resource transfer nhỏ, có rejection/retry/stale/failure cases.
3. FND-09/10/11/12/13 dựng causal events, jobs, atomic persistence, recovery và command replay. Đừng mở rộng hệ class khi không thể giải thích và tái hiện một command.
4. FND-14/15/16 làm registry/diagnostics và kiểm chứng host–Unity. Đây là điều kiện trước khi tuyên bố Foundation xong, không phải việc để sát ngày phát hành.
5. VS-02–06 nối nghề, vật chất, perception, thread, companion, combat và di sản trong một vùng sâu.
6. VS-07–10 đưa qua client, playtest và scale. Phê duyệt mở full production từ bằng chứng, không từ số NPC/scene.

## Task đầu tiên Sprint 1: FND-02

Scope: typed entity/definition identity, allocator state, lifecycle và serialization contract. Input: M29 §9–14, Foundation contract, reference fixture IDs. Output: C# implementation độc lập Unity, codec spec, golden vectors và tests; quyết định clear committed vs temporary ID. Không gộp world generator hoặc household simulation vào task này.

Done: identity được serialize ổn định; không tái dùng committed ID; rollback/retry không phá deterministic allocation; references khác loại bị phát hiện; tests có command và expected output. Nếu chọn byte/string representation khác blueprint ví dụ, ghi ADR và cập nhật fixtures đúng phiên bản.

## Mẫu task để AI thực hiện

- Task ID, requirement IDs, module/section nguồn.
- Allowed files/assembly và dependency boundary.
- Owned data, read dependencies, allowed commands, emitted events.
- Invariants, rejection/errors, concurrency/retry/persistence behavior.
- Acceptance cases và cách chạy.
- Explicit non-goals, open decisions phải chốt trong task.
- Deliverables gồm code, tests, docs/ADR thay đổi và evidence.

Không giao “đọc cả GDD rồi build game” thành một task không kiểm soát. Source research vẫn là tài liệu, không phải quyền tự phát triển tất cả hệ thống trong một lượt.

## Definition of Done

Build và tests của phạm vi pass; không phá replay/contract; mọi mutation đúng owner và transactional path; content có schema/reference validation; UI không biết secrets ngoài projection; test evidence ghi versions/seed. Nếu không có Unity trong môi trường, nói rõ compatibility chưa được kiểm chứng thay vì coi dotnet build đủ thay thế.

## Gates sau rà soát 14/10/2026

- **VS-01 = Headless kernel gate:** FND-02–15; cho phép làm content/domain slice khi kernel có evidence. Không được gọi gate này là Foundation complete.
- **Foundation complete:** tất cả FND-02–16, bao gồm cùng vectors/replay trên Unity target thật. VS-07 phụ thuộc FND-16.
- **Early client spike:** sau FND-02/03/04 build được, thử import portable DLL/primitive vectors và provider packaging; không đợi replay hoàn chỉnh mới phát hiện incompatibility. FND-16 chỉ nghiệm thu cuối sau FND-13.
- **Test setup:** FND-02 phải pin framework/runner, tạo test project/CI discovery>0 theo Test Execution Contract; reference Python checks không thay điều này.
- **Data gate:** parameter pack cho từng nghề phải có trước rules VS-03. VS-10 tune dựa playtest, không hoãn thiết kế units đến đó.

Specs và trạng thái sau bổ sung: [Documentation Completion](DOCUMENTATION-COMPLETION-2026-10-14.md). Provider/editor/framework còn có gate thực nghiệm; không xem chúng là thiếu module GDD mới.

Quy trình áp dụng cho AI: [AI Execution](../runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope các sprint nằm trong sprint brief được liên kết tại đó.

## Sprint11 — VS02A: dữ liệu dân cư và mạng đường

[Compiler và cách dùng](../runbooks/SETTLEMENT-CONTENT.md) kiểm24actors/6households/4communities/8routes/24claims/24directedrelationships, tạo immutable authoring registry. VS02 full vẫn partial: Sprint12 đã bổ sung8event templates/eligibility trên trusted observations, Sprint16 đã kiểm chứng chọn protagonist từ cohort; actor-specific obligations/full viability, routine execution/event effects/world binding vẫn còn. Không coi đủrecordcount là worldsimulation.

## Forecast lịch sử theo sprint — 26/10/2026

[Lộ trình hình ảnh và bản chơi thử](PLAYABLE-ROADMAP.md) bổ sung mốc dự kiến Sprint13 art direction/UI prototype và Sprint15–16 demo nhỏ. Đây là dự báo tại26/10, chưa là release commitment; sau chốt hướng2D03/11 cần đo/reforecast, không coi Sprint15–16 là ngày cam kết. Client prototype giới hạn có thể làm trước VS07 đầy đủ; không thay dependencies hoặc exit gates của VS07/10.


## Sprint14 — VS-03A bounded repair implementation

User đã giao [Sprint14](sprints/SPRINT-14.md) ngày03/11/2026. [ADR023](../adr/023-bounded-boat-repair-profile.md) cho phép hiện thực repair độc lập trên trusted fixture để kiểm một hành động causal thật khi VS02full cònpartial; không bypass dependency để đóngfullVS03. Typeditem/activity/holds vàinternalcompletion, SQLite/replay/canonicalcompatibility có acceptance riêng. Corechanges là gameplay mới được giao, G13 checkpoint vẫn giữ lịch sử thay vì freeze tất cả phát triển. Scene/sprite/projection/randomlife/Unitygates và fullVS07 vẫn riêng; chưa tự mởSprint15.

VS-03A được nghiệm thu trong headless profile theo [evidence Sprint14](../qa/SPRINT-14-EVIDENCE.md):562/562 tests,113 repair,0failed/skipped. FullVS03 vẫnpartial; bước tiếp theo đề xuất là chia proof client2D và actor-scoped read model khi được giao, không tự mở sprint.

## Sprint15 — client2D repair proof giới hạn

[Sprint15](sprints/SPRINT-15.md) và [ADR024](../adr/024-bounded-2d-repair-client.md) đưa bounded repair lên Unity2D: safe actor-scoped port, preview/confirm/retry, actual SQLite và scene có lớp nền/foreground/boat/actor sprite độc lập. Một fixed trusted actor/worksite, không bỏ dependency của fullVS07 để gọi toàn milestone đã hoàn tất. Local footpoint/camera/animation chỉ là presentation, không Travel/WorldTick truth.

Ngày06/11/2026 đã có actual Unity6000.6.7f1/LinuxMono Player build0lỗi/0cảnh báo,12ảnh/sáu layout cases, final SQLite repair/cancel/wait và OS keyboard/mouse evidence. Một cú nhấn–thả thuyền chỉ mở preview; Confirm cần hành động riêng. Đây bước thực thi của hướng2D/sprite, không chỉ concept hoặc source.

Năm gate ở [client contract](../contracts/REPAIR-CLIENT-PROFILE.md) là điều kiện đóng Sprint15: session privacy tests; actual Editor/bootstrap/build; actual LinuxMono Player effects/save/reload; rendered/input evidence; independent review/regression/docs/CI. Trạng thái thực tại [evidence](../qa/SPRINT-15-EVIDENCE.md); chương trình callback automation, OS input, visual review và human playtest ghi riêng. Backlog random life/full knowledge/world binding/finalart/performance không là gate bổ sung ngoài scope sprint này.
