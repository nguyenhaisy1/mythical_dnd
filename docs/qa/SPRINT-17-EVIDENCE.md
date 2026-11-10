# Sprint17 — Công việc đan dây và lưu tiến trình

## Tổng hợp dễ hiểu

Sprint17 nối việc làm đồ vào xuất thân đã nhận. Vạc Lâm và bà Nhu đã có kỹ năng đan trong dữ liệu; họ có thể xem trước chi phí, xác nhận, giữ nguyên liệu và chờ nhận dây. Hủy trả phần đang giữ, không tạo sản phẩm. Lưu giữa công việc rồi mở ứng dụng lại phải tiếp tục đúng việc đó. Sáu xuất thân khác được ghi rõ là bản thử chưa có công việc phù hợp, không bị đổi nghề hoặc ép bốc lại.

SPIN-041 mở10/11/2026 sau Sprint16CLOSE/PR17/deve7dfd9b. [ADR026](../adr/026-personal-weaving-on-origin.md), [contract](../contracts/PERSONAL-WEAVING-PROFILE.md), [brief](../production/sprints/SPRINT-17.md) và [runbook](../runbooks/PERSONAL-WEAVING.md) khóa phạm vi trước code. Starter8fiber là vật liệu cá nhân mới của bản thử, không suy tài sản/quyền hộ từ history. Recipe dùng tay,2fiber/30ticks/1cord/minskill40 là tham số engineering, chưa balance. Đây subset chế tác, không fullVS03 hoặc bộ sinh lịch sử cuộc đời.

## Điều kiện đóng Sprint17 còn thiếu

Nghiệm thu kỹ thuật local10/11/2026 đạt, docs/exports local đã kiểm; Git/CI/PR→dev phải hoàn tất trước CLOSE. Trạng thái sau commit ở WORKLOG, không tự nhận đã merge từ kết quả local.

| Gate | Trạng thái hiện tại |
|---|---|
|1. Core/history/rights/determinism|Đạt:96/96focused và final full770/770/0failedskip, source guard khớp nguồn cuối.|
|2. SQLite/crash/recovery/idempotency/profile|Đạt: final full lặp32calls/7SIGKILL, snapshot/recovery/idempotency/immutability/refusal và source guard.|
|3. Actual Unity ba origins/saveRunning/restart/legacy15–16|Đạt actual8f9zo2ld: ba origins, legacy15–16 regression; OSg714cvsc48actions/hai process restart tiếp tục Running→Completed.|
|4. Renderer/layout/focus/modal/native gestures|Đạt:116origin-work PNG, six Reveal/World/preview cases, scroll/focus/modal; native pointer và Return Confirm riêng, không là human playtest.|
|5. Independent final review/docs/exports/CI/PRdev|Hai final reviews độc lập noP0/P1 trong scope, full770/770/process/sourceguard và docs/exports local đạt. Git/CI/PR/dev sau commit theo WORKLOG, chưa CLOSE nếu gate đó thiếu.|

Không chuyển các gate trên sang backlog để đóng. Kết quả kỹ thuật cuối cập nhật tại đây; Git/CI đúng HEAD và CLOSE sau commit ghi append trong [WORKLOG](../../WORKLOG.md).

## Phân công thực tế

| Actor | Phạm vi đã thực hiện |
|---|---|
|`/root/s17_design_audit`|Readonly content audit rồi compiler/gateway/session/codec/store/mirrors/infra tests; bị usage quota trước hoàn tất test/review, root tiếp quản.|
|`/root/s17_arch_audit`|Readonly architecture audit rồi DomainWeaving/commands/scheduler/history/core tests; actual41cases đầu đạt, bị quota trước nghiệm thu cuối.|
|`/root/s17_unity`|Bốn source Unity mới/QA/build hooks, safe retry và fresh Confirm guard; không tự chạy Editor/Player.|
|`/root/s17_critic_core`|Source audit độc lập phát hiện candidate thiếu qualification restore, viết WeavingAdversarialTests; bị quota trước report/repro cuối.|
|`/root/s17_core_final`|Tiếp quản độc lập source/adversarial/process review; xác nhận qualification repro và phát hiện historical overlap; thêm regression riêng và tự chạy reviewer subset. [Report](sprint-17/CORE-FINAL-REVIEW.md).|
|`/root/s17_ui_review`|Review độc lập safe UI/session/runners, actual captures/layout/OS khi có. [Report](sprint-17/CRITIC-UI.md).|
|`/root/s17_docs`|14consumer sources/runbook/DEC033/registers/matrix/integration notes/export inputs, không generated edits hoặc Git.|
|`/root`|Điều phối, contracts/ADR/brief/ports/parameter pack, independent Python known answers, CLI/probes/runners, tiếp quản và sửa findings/test plumbing, actual Editor/Player/OS, fullsuite/docs/exports/evidence/Git.|

Actors theo đợt, tối đa4 đồng thời kể cả root. Hai reviewer cuối dùng context mới gọn để tiếp tục sau quota; không tự nhận actor đã thất bại là đã hoàn thành review. AI automation/review không là human playtest hoặc owner final art approval.

## Focused tests và các lỗi đã tái hiện

| Artifact | Kết quả lúc thực thi |
|---|---|
|`sprint17-core-first/core.trx`|41/41Passed trước hai core cases bổ sung.|
|`sprint17-infra-first/weaving-infra.trx`|47cases:46pass/1fail do test cast List thành object[], chưa là lỗi gameplay.|
|`sprint17-adversarial-before/adversarial.trx`|Hai lỗi setup InvalidCast; không lấy làm chứng cứ qualification bug.|
|`sprint17-adversarial-valid-before/adversarial.trx`|Sau root sửa recursive List handling:2Failed ExpectedFormatException/actualnull. Coherent Running và Completed được retarget sang Bến Kiên skill0 vẫn được codec nhận.|
|`sprint17-focused-fixed/weaving.trx`|91/92Passed; qualification cases đã đạt, một test giả định DrainAdvance budget1 phải dừng ở tick30 sai: khi không còn job thì nó đóng target60.|
|`sprint17-focused-final/weaving.trx`|92/92Passed sau dùng afterCommit quan sát job publication30 thật, không đổi gameplay scheduler để khớp test.|
|`sprint17-overlap-before/overlap.trx`|Independent case1Failed ExpectedFormatException/actualnull; valid sequential control đạt. Coherent Start1/Start2/Cancel1/Cancel2 cùng tick vượt busy constraint lịch sử dù final state không còn Running.|
|`sprint17-focused-second-fix/weaving.trx`|96/96Passed,0failed/skipped sau occupancy check theo accepted revisions.43core/47infra/6integration-adversarial cases.|
|`sprint17-core-reviewer-after-overlap/reviewer.trx`|Reviewer độc lập tự chạy28history/adversarial cases,28Passed/0failedskip; không thay fullsuite.|

Raw paths dưới `tests/results/`. Root sửa qualification bằng START_QUALIFICATION: có activity phải có skill>=minimum trong profile skill immutable. Busy history kiểm start.AfterRevision lớn hơn terminal trước và không có hoạt động trước còn Running; không so tick vì cancel/restart cùng tick hợp lệ. Reviewer độc lập đã tự đọc before/after và source patch. Không thay legacy goldens để che failure.

## Known answers và profile

Independent Python MDC1 reference ở `content/fixtures/personal-weaving/expected.json`:

- Cohort hash `3b4560e57726f3693a483020924f33121fbfa554892ff912029dd50bdf95c8aa` giữ như16.
- Recipe hash `3001a817410943cc233bc064badf0c68493e6e9f52e99e48535e07ca05bd2df7`.
- Profile hash `f9d850edf58c22f525a185e8ba0e3430a0e189e0ccfe76531a088173a404a3bc`.

Seed0×64 nhận Lâm43; seed1×64 nhận Bà Nhu61; seed01234567×8 nhận Kiên/transport52, weaving0. Tất cả genesis cursor1; craft không draw. Initialfiber8/hold0/cord0; Start8/2/0; beforedue29 vẫn8/2/0; due30 là6/0/1; cancel là8/0/0 nếu chưa craft khác.

Image4 Origin+Weaving và exact new rule/content/root riêng; không enrich save16, không gắn Repair vào Origin, physicalSQLite schema2 giữ. Mirror weaving_activity và lot effects phải khớp canonical image. Replay artifactv1 từ chối Origin/WeavingRuleVersion; không gọi save/recovery là command replay.

## Process và crash đầu

`tests/results/sprint17-process-first.json`, raw `tests/results/weaving-process-ebgmkubm/`:32child calls/7SIGKILL exit−9. Ba publication phases after_initialize/before_publish/after_publish; bốn action phases Start/Complete trước/sau commit. Prepublish giữ campaign cũ và orphan pending; postpublish giữ campaign mới. Response-loss Start retry không nhân đôi hold.

Raw completion-beforecommit là Running29/hold2, sau Continue session drain đúng durable accepted advance→Completed30/fiber6/cord1. Completion-aftercommit raw đã Completed30; Continue chỉ đóng workflow còn pending, không consume thêm. Đây hai quan sát khác nhau, không sửa raw state để coi rollback đã tạo sản phẩm. Reviewer độc lập tự decode13publishedDBs/hash/image4/rule/cursor/effects; final full phải lặp gates trên nguồn cuối.

Same seed giữ genesis hash và gameplay outcome; opaque random preview token là presentation input trong correlation của command khác nhau nên không tuyên bố final command-history hashes giống nhau giữa hai phiên token khác nhau. Không global RNG trong domain hoặc runtime LLM.

## Unity lần đầu và hướng sửa

`tests/results/sprint17-client-vhwrph7p/`: actual Unity6000.6.7f1/LinuxMono Configure/PrepareScene/Build succeeded0errors/warnings; Player FailedRESOLUTION khi preview matrix quay1920fullscreen→1280window, actual1280×683.16captures trước failure chỉ provisional, không nghiệm thu bằng runner xanh.

Root đổi thứ tự QA: tại1280 kiểm Reveal và preview cùng ba cỡ chữ, sau đó mới1920; các transition trung gian không thay state và vẫn so origin/tick/stock. Không giảm assert720/1080, không crop ảnh. Reviewer UI đã review patch nguồn; actual fresh run còn chờ. Scene15/16 giữ source, build explicit scene lists khi regression, không PrepareScene cũ để đổi IDs.

Lượt `sprint17-client-bbsmed0g` đã chạy cả Lâm/Nhu/Kiên cùng actual scene16/15 Player regression và đọc finalSQLite đúng (qualified6fiber/1cord; nonweaver8fiber/0cord). Overall FailedSourcesChanged vì root bổ sung World-panel layout/focus/scroll coverage trong khi lượt chạy; không dùng nó làm final revision acceptance. Lượt cuối cần42PNG mỗi qualified,32PNG nonweaver: thêm top/bottom World ở sáu cases, cùng Reveal và preview; QA focus hooks không là native input. Script vẫn reject source changes, không waive gate.

## Actual Unity và native input cuối

Root chạy `python3 tools/unity/run_weaving_client.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet --editor /home/synh/Unity/Hub/Editor/6000.6.7f1/Editor/Unity`. Raw `tests/results/sprint17-client-8f9zo2ld/`, overall exit0/Passed, actual Unity6000.6.7f1/LinuxPlayer/Mono, build0errors/warnings.230source paths trước/sau Build/Player giữ fingerprint `2369663f9e869b89d7f5c69ef6b9069064ffc071d95b4e497acfc15d330d70e3`. Managed Assembly-CSharp SHA256 `474694cb8f9646ad0ede7a182f8469ab9c18fa4c996e71ad5b261b2f81406d18`.

| Origin thực | Render/callback | Final SQLite |
|---|---|---|
|Vạc Lâm22, weaving43|42PNG, đủ sáu Reveal/World/preview cases|tick91/fiber6/hold0/cord1;2Start/1Completed/1Cancelled|
|Bà Nhu77, weaving61|42PNG, cùng ma trận|tick91/fiber6/hold0/cord1;2Start/1Completed/1Cancelled|
|Bến Kiên37, transport52/weaving0|32PNG, sáu Reveal/World cases; craft disabled đúng scope|tick61/fiber8/hold0/cord0;noactivity/noStart|

116ảnh origin/work thực, không ảnh concept. Reveal và preview kiểm body/footer; World có top/bottom ở sáu1280×720/1920×1080 ×100/125/150% cùng focus/ScrollTo kiểm mọi control cần có. QA focus hook không là native keyboard proof. Root/reviewer đã mở ảnh cỡ150%, own-record, Running/completed/nonweaver để kiểm chữ/buttons/scroll thật. Delayed Reload sau captures và independent persisted image/hash/command/job/activity/fact/mirror check chặn effects phát sinh sau sample. Origin/self-record fingerprint và RNGdraw1 giữ; completion fact tick30 dù finaltick91.

Cùng staged core mới build/chạy scene16 và15 bằng explicit scene list, không regenerate scene cũ: scene16 actual19captures/19requiredflags, image3/tick61/noWeaving/noLots; scene15 actual12captures/23flags/3DBimage2/noOrigin/noWeaving, repair complete700condition/8timber/0hold, cancel500/10/0, wait500/10/0. Đây actual legacy regression trong `legacy16/` và `legacy15/`, không chỉ dẫn evidence sprint cũ.

Root chạy `python3 tools/unity/run_weaving_input.py --player tests/results/sprint17-client-8f9zo2ld/Player/MythicalWeaving.x86_64`, raw `tests/results/sprint17-input-g714cvsc/`, exit0/Passed. Hai actual PID2544008/2544229,48native actions(35+13), cùng assembly ở trên. Actual drawable phase1 **1280×683**, phase2 **1280×720**, được ghi đúng; renderer ma trận720/1080 có evidence riêng.

Phase1: New pending→Esc cancel không publish, New→Reveal→Begin; pointer mở preview chỉ xem, Esc Back không giữ/tiêu đồ. Cú pointer Confirm mới bắt đầu; ReturnCancel trả8/0/0. Lần hai dùng Return trên Confirm, chờ1, SaveRunning và self-record modal không kích background, F10quit. Phase2 actual process mới→ContinueReveal vẫn Running/tick1/hold2→Begin→SpaceWait60→Completed/tick61/fiber6/cord1; Save/Reload/menuContinue giữ record và kết quả. SQLite cuối SHA256 `90abf6cc9bb94ad440d46a0e85974d380ae100635a2ce2acb9dbd8be2ab2f268`, đúng7commands:2Start/1Cancel/2AdvanceTime/1ExecuteScheduledWeaving/1CloseAdvance; completion+JobCompleted ở tick30, cursor1. Đây automation kỹ thuật, không human playtest.

## Các lượt thất bại sau đó và sửa native/QA

- `sprint17-client-_rrf0hsi`: callback/render/legacy toàn bộ Passed trên source trước sửa nativeConfirm; không nhận làm bản cuối sau khi View thay đổi.
- `sprint17-input-uurtz1tv`: Failedtimeout nativeConfirm; preview/focus đúng, nativepointer đã tới nhưng chưa bắt đầu,8/0/0. Đây bug hành vi P1 thật; callback QA gọi ConfirmPreview trực tiếp không bắt lỗi này. Root đổi fresh PointerDown sang capture phase trước Button Clickable và thêm targeted NavigationSubmit capture; nguyên nhân event ordering là inference nguồn, proof cuối là pointer+Return đều chạy đúng ở g714cvsc.
- `sprint17-5xcxpfyd`:770/770C#Passed và negative đúng nhưng overall exit1 vì ready-file tồn tại còn rỗng trước File.WriteAllText hoàn tất; không phải SIGKILL đã tới hook hoàn chỉnh. Đồng thời source UI thay đổi nên không final. Giữ raw empty marker. Root sửa process driver chờ exact nonempty phase trong deadline45s, wrong phase vẫn fail closed; `sprint17-process-ready-fixed.json` rerun32calls/7SIGKILLPassed. Final full cần lặp trên nguồn sau mọi sửa.

## Full regression và independent review cuối

Root chạy `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`; raw `tests/results/sprint17-wpa3bfrf/`, overall exit0,770discovered/executed/Passed,0failed/skipped.96ca mới gồm43S17-01/47S17-02/6S17-04;674regression cũ gồm113repair và81origin16. Negative control đúng1test Failed/childexit1, harness xác nhận failure rồi đi tiếp. Full transaction/save SIGKILL/replay/content/diagnostics/conformance/settlement/event-eligibility/repair/origin/weaving process gates đạt; weaving32calls/7SIGKILL với ready handshake đã sửa.

Final source manifest322paths trước/sau khớp, fingerprint `8d31740d0494ab70ee0cd92634e71e5290386ae995bb9066cab161fe999100c8`. Build Release0warnings/errors. Final nested weaving raw `tests/results/sprint17-wpa3bfrf/weaving-process-ymmz53vu`,32calls/13publishedDBs. Các lượt lỗi5xcx và standalone trước đó được giữ riêng; không dùng thay final hoặc trộn source revisions.

[Core reviewer](sprint-17/CORE-FINAL-REVIEW.md) tự parse finalTRX/negative/322hashes, tự decode13finalSQLite và crash phases/facts, chạy28history/adversarial subset riêng; hai restore findings đã đóng. [UI reviewer](sprint-17/CRITIC-UI.md) tự kiểm230hashes/renderer147ảnh mới+cũ, native48actions/twoPIDs/actual dimensions/DB và same assembly, nativeConfirm P1 đã tái kiểm. Không còn P0/P1 đã biết trong bounded scope. Mỗi report nêu phần chưa trực tiếp kiểm và để Git/CI/docs gates cho root, không biến self-review thành independent hoặc automation thành human playtest.

## Đồng bộ tài liệu và exports

Đã cập nhật nguồn .agent/AGENTS/ADR026/contract/brief/parameter pack/fixtures/runbook/evidence,14consumer docs/registers/matrix/DEC033/integration notes07/08/18/23/26/28/29 và export inputs. Regenerate36docs, source/handoff1086authoredlinks/52requirements/28tasks, reference/vectors/architecture/safe7presenters+origin/weaving/source-assets checks,4offlineMCPtests/6skilllinks và whitespace đạt. Archive29modules/appendix/source/01–18/legacygoldens giữ, không thay nghiên cứu bằng summary.

Local export7HTML+7DOCX/check_exports311sourceinputs/14containers/6156anchors/6251headings/123tables/navigation đạt. Đây container/content checks, không visual DOCX rendering/game acceptance. Artifacts/results/Player/DLL/native/cache ignored; không ZIP hoặc compiled artifacts trong commit. ExactHEADCI/PR→dev và CLOSE sau commit append ở WORKLOG; không tự Sprint18/main/tag/release.

## Backlog dự án ngoài sprint

Full ba nghề/skill progression/class/quality/tools/trade, toàn NPC/world/Knowledge binding, causal life generator/Character Viability/TEST001, fullVS02/VS03/VS07/campaign, tạo hình đủ cast/final owner UI-art/human playtest/performance/crossplatform và new command-replay profile. Factory17 chỉ own-at-home hai observed lots; core trusted revalidation fixtures không là end-to-end Travel/rights/Health commands hoặc mở mọi world variant qua client17. Không tự Sprint18/main/tag/release.
