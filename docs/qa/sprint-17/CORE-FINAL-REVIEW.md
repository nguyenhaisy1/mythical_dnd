# Sprint17 — Review độc lập core, lịch sử và lưu trữ

10/11/2026 · SPIN-041 · Actor `/root/s17_core_final` · skill `mythical-validation`.

## Trạng thái review

**Review kỹ thuật core và final runtime/process evidence đạt trong phạm vi `personal-weaving-v1`; không còn P0/P1 đã biết sau hai bản sửa được kiểm lại.** Reviewer đã tự đọc source, TRX/negative, manifests, process JSON và SQLite bằng decoder độc lập. Đây không tự đóng sprint hoặc thay actual Unity/OS/docs/exports/Git/CI/dev gates.

Reviewer đọc AGENTS/.agent/HANDOFF/APPLICABLE, ADR026, PERSONAL-WEAVING-PROFILE, Sprint17 và execution contract. Ownership ban đầu chỉ report này; sau khi reviewer tái hiện lỗi lịch sử chồng công việc, root giao thêm duy nhất `WeavingOverlapAdversarialTests.cs`. Reviewer không sửa production, không chạy Editor/Player hoặc commit/push/merge Git. Test qualification do critic trước viết và root sửa plumbing; không gán nó là sản phẩm của reviewer hiện tại hoặc claim critic trước đã hoàn tất review.

## Findings và kiểm lại

### S17-CORE-01 — P1: qualification có thể bị bỏ qua khi restore lịch sử đồng bộ

`WeavingAdversarialTests` lấy history Running hoặc Completed hợp lệ của Lâm, chuyển toàn bộ typed world IDs sang genesis thật của Bến Kiên, giữ Origin/seed/RNG của Kiên, sửa effective skill facts thành0 và tính lại receipt fingerprints. Mọi phần đồng bộ nhưng nhân vật chưa bao giờ đủ weaving40.

Reviewer tự đọc `tests/results/sprint17-adversarial-valid-before/adversarial.trx`: **2/2 Failed**, expected FormatException nhưng actual null, xác nhận codec đã nhận lịch sử bất khả thi. Lượt `sprint17-adversarial-before` cũ lỗi InvalidCast trong setup, không phải bằng chứng lỗ hổng. Root sửa Array/List plumbing, giữ cả lịch sử thất bại thật; không đổi goldens.

Guard `START_QUALIFICATION` trong `ValidateWeavingHistory` yêu cầu có activity thì immutable skill phải đủ minimum. Nó áp dụng cả publication và decode/restore. Reviewer xác nhận guard phù hợp với contract immutable skill/genesis hiện tại, không pin current alive/location/owner vào Origin. Focused-final trước khi phát hiện overlap: **92/92 Passed**, không failed/skipped, gồm cả hai ca adversarial.

### S17-CORE-02 — P1: trạng thái cuối hợp lệ nhưng lịch sử hai công việc chồng nhau

Reviewer tạo hai chu kỳ Start/Cancel thật cùng tick0, canonical round-trip hợp lệ: start1@revision2, cancel1@3, start2@4, cancel2@5. Probe độc lập `/tmp/s17-busy-review/Program.cs` đổi thứ tự thành start1@2, start2@3, cancel1@4, cancel2@5; đồng bộ before/after/expected revisions, fingerprints, event commit sequences và entity creation/retirement revisions. Final vẫn8fiber/0reserved/0cord, hai activity Cancelled và job sequences hợp lệ.

Probe in `ACCEPTED forged historical overlap: start1@2,start2@3,cancel1@4,cancel2@5`. Đây lỗi thực: start2 lúc activity1 còn Running phải bị từ chối, dù snapshot cuối không còn Running. Reviewer gửi finding ngay và thêm ca `S17_04_CoherentHistoricalOverlapCannotRestoreEvenWhenBothActivitiesAreCancelled` theo ownership root cấp; root sở hữu bản sửa production và capture trước/sau. Reviewer đã tự đọc repo TRX `tests/results/sprint17-overlap-before/overlap.trx`: **1/1Failed**, ExpectedFormatException/actualnull tại final assertion, không phải setup lỗi. Root sửa `HISTORICAL_OVERLAP`: sort start theo accepted AfterRevision, yêu cầu start tiếp theo sau terminal revision trước và không có activity trước còn Running. Reviewer kiểm patch dùng revision thay vì tick, giữ start/cancel/start cùng tick hợp lệ. `tests/results/sprint17-focused-second-fix/weaving.trx` đạt **96/96Passed,0failed/skipped**. Source finding đã sửa; final full suite nguồn cuối770/770Passed có ca regression này.

## Các ranh giới source đã đối chiếu

- Domain portable, không Unity/DB/IO/global random; recipe2fiber/30ticks/1cord/min40 và genesis8fiber mới được chốt strict ở compiler. Lâm43/Nhu61 lấy skill authored qua immutable dictionary, không grant profession/household authority.
- Start recheck actor/skill/location/rights/commodities/available/output overflow và busy; reserve2 cùng activity/job/fact/receipt trong một publication. ID staging không publish khi reject. Cancel own Running release đúng hold; direct CancelJob không bypass activity. External CompleteWeaving/ExecuteScheduledWeaving bị chặn.
- Due dùng immutable input tại due tick; publication gộp consume2/release2/output1/terminal activity+job/events và receipt. Revalidation có alive/location/rights/skill/commodities/overflow; trusted recomposition tests không bị nhận nhầm là Travel/Health/rights command acceptance.
- History ràng buộc typed pairing, source/cause/correlation/recipe/effective quantities, start/terminal receipts/events, execution/workflow, lifecycle revisions, sequence và bounded material balance. TransferResource/ScheduleTransfer bị chặn riêng profile17. Historical busy cũng được kiểm theo accepted revision sau bản sửa S17-CORE-02.
- Codec image4 chỉ Origin+Weaving, strict canonical round-trip/unknown fields; không thêm null weaving keys vào image1/2/3. Store exact rule/content và pin canonical origin/genesis metadata trước writer; physicalSQLite schema2 giữ. Replay artifactv1 từ chối origin/weaving, không claim command replay từ save/recovery.
- Safe session chỉ own self record và hai own observed lots. Token presentation opaque, preview không thay RNG/state; same request kiểm original durable envelope trước stale token, đổi token/intent mismatch. Safe errors hữu hạn không lộ exception. DTO không raw IDs/seed/path/processor/private revision/world stock.
- Open/Reload chỉ DrainAdvance cho Player intent đã được chấp nhận và thuộc principal; không tạo target/Advance mới. Factory cố ý giới hạn actor ở home/hai lot own observed, không auto-grant rights hoặc tự enrich save16. Gateway pending/verify/flush/create-new publish trong root17 riêng giữ request/catalog/cancel/crash semantics.

## Evidence reviewer đã tự kiểm

Reviewer tự thực thi:

```text
/home/synh/.cache/mythical-dnd/dotnet/dotnet test tests/MythicalDnd.Foundation.Tests/MythicalDnd.Foundation.Tests.csproj --configuration Release --no-build --no-restore --filter 'FullyQualifiedName~WeavingAdversarialTests|FullyQualifiedName~WeavingHistoryTests' --logger 'trx;LogFileName=reviewer.trx' --results-directory tests/results/sprint17-core-reviewer
```

Exit0, **27 discovered/executed/Passed,0failed/skipped**, `reviewer.trx`. Đây lượt trước bản sửa overlap; không coi nó thay final suite. Reviewer tự chạy lại sau fix với filter thêm `FullyQualifiedName~WeavingOverlapAdversarialTests`, output `tests/results/sprint17-core-reviewer-after-overlap/reviewer.trx`: exit0, **28discovered/executed/Passed,0failed/skipped**. Lượt này gồm valid same-tick sequential control và impossible-overlap rejection; không chỉnh test/golden để làm xanh.

Reviewer đọc `tests/results/sprint17-process-first.json`: Passed, **32child calls/7SIGKILL** (3New publication,2Start,2completion). Checker quan sát ready barrier trước SIGKILL; raw inspect riêng trước Continue. Reviewer tự viết bộ giải mã MDC1 Python, mở read-only **13published SQLite files**, kiểm image SHA/image4/exact rule/draw1 và đếm accepted commands/executions:

| Trường hợp | Tick | Fiber/Reserved/Cord | Scheduler execution |
|---|---:|---|---:|
| Lâm/Nhu complete, cùng seed khác path/request |30|6/0/1|1|
| Lâm cancel rồi chờ |60|8/0/0|0|
| Bến Kiên không đủ skill |0|8/0/0|0, không accepted command|
| Start crash trước commit |0|8/0/0|0|
| Start crash sau commit |0|8/2/0|0, chỉ một Start|
| Completion crash trước/sau commit, sau Continue |30|6/0/1|1, chỉ một internal completion và một CloseAdvance|

Raw completion-before_commit là Running/tick29/8/2/0; raw after_commit là Completed/tick30/6/0/1. Sau Continue cả hai Completed/tick30, không thêm Advance target hoặc RNG. New-before-publication giữ Lâm cũ và pending orphan; after-publish giữ thêm Nhu, không ghi đè campaign trước. Lượt process-first trước fix overlap là evidence ban đầu, không thay final source-bound process run.

## Lượt full chưa đạt được giữ lại

Reviewer tự parse `tests/results/sprint17-5xcxpfyd`: runtime TRX **770/770Passed,0failed/skipped** (S17-01=43, S17-02=47, S17-04=6;674regression), negative TRX đúng1Failed/childexit1, Release build0warnings/errors. Tuy nhiên runner `evidence.json` có **exit1**, `weaving.json` Failed sau20childcalls, error `Wrong actual fault hook`; không được gọi lượt tổng này Passed.

Reviewer mở raw ready barrier của lượt lỗi: `weaving-process-2732j94z/startafter_commit-kill.ready` là0bytes. `kill()` chỉ chờ exists(), có thể đọc trước khi `File.WriteAllText` ghi đủ phase; finally kill đã chấm dứt child. Đây race trong harness, không chứng minh lỗi transaction gameplay. Root cần chờ đúng phase content trong deadline rồi rerun, giữ strict actual hook và artifact lỗi. Khi reviewer kiểm lại manifest322paths,3paths UI/scene/input-tool đã thay sau snapshot; dù28reviewedcorepins còn khớp, lượt này cũng chưa thể đại diện toàn nguồn cuối. Root đã sửa chờ phase content exact trong deadline45giây; empty/missing tiếp tục chờ, nonempty sai vẫn fail closed, chỉ SIGKILL sau đúng hook. Reviewer đọc patch và `tests/results/sprint17-process-ready-fixed.json`: Passed32calls/7SIGKILL. Đây fix harness được review, không nới transaction acceptance. Lượt manifest/run mới sau freeze đã được kiểm ở phần final bên dưới.

## Final runtime/process và source manifest đã review

Lượt cuối `tests/results/sprint17-wpa3bfrf` có **runnerexit0**, runtimeexit0/errornull, **770discovered/executed/Passed,0failed/skipped**. Reviewer tự parse cả770UnitTestResult và counters; S17-01=43, S17-02=47, S17-04=6, tổng96test Sprint17 +674regression cũ, gồm113repair. Negative TRX đúng1test executed/Failed, childexit1; không nhầm failure mong đợi này với runtime failure. .NET SDK10.0.401/Linuxx64, Release build0warnings/errors. Runtime test duration3phút56giây; không là benchmark hoặc Unity result.

Reviewer tự hash **322paths** trong `sources.json`: **0missing/changed**, fingerprint tự tính khớp `evidence.source_revision`:

```text
sha256:8d31740d0494ab70ee0cd92634e71e5290386ae995bb9066cab161fe999100c8
```

28files reviewer đã kiểm riêng có snapshot `tests/results/sprint17-core-reviewer-after-overlap/source-review.json`. Production core và tests giữ hash; thay đổi duy nhất trong snapshot riêng là `check_weaving.py` do sửa readiness race đã review ở trên. Tất cả28files nằm trong manifest cuối322paths. Evidence nguồn cuối thay lượt5xcxpfyd, không sửa hoặc xóa artifacts thất bại trước đó.

Final runner thực sự bao gồm transfer/scheduler demos, persistence3SIGKILL, save snapshot/migration6SIGKILL, replay/content/diagnostics/conformance-host/settlement/event-eligibility/repair, origin21calls/3SIGKILL và **weaving32calls/7SIGKILL Passed**. Reviewer đọc các JSON và control outcomes, không chỉ TRX: conformance.json ghi rõ host/tooling và UnityNotRun; actual Unity được nghiệm thu qua run riêng, không suy từ headless conformance preparation. Replay negative controls fail đúng expected divergence; diagnostics negative controls từ chối đúng profile/checksum/size/schema; repair hai fresh-process runs giữ image2/SQLite2/recovery/cancel và command replay riêng. Không nhận weaving/origin command replay.

Reviewer lần nữa tự giải mã bằng Python MDC1 riêng và mở read-only **13published SQLite files** trong `weaving-process-ymmz53vu`: tất cả image4/schema2/exactWeavingRuleVersion, imageSHA khớp storedhash, origin/draw1 đúng; final effects giống bảng trên. Lâm/Nhu complete6/0/1; Kiên no acceptedcommand/8/0/0; cancel8/0/0; Start beforecommit idle và aftercommit Running/hold2. Hai completion crash sau Continue đều tick30/fiber6/hold0/cord1, đúng **một ExecuteScheduledWeaving + một CloseAdvance**, không double effect. Raw beforecommit Running29/8/2/0 được giữ riêng với raw aftercommit Completed30/6/0/1. Chi tiết tự parse ở `tests/results/sprint17-core-reviewer-after-overlap/final-artifact-inspection.json`.

## Gate ngoài review core và giới hạn

Core/history/rights/privacy/SQLite/runtime/process review đã đạt trong bounded profile trên nguồn cuối. Actual Unity/renderer/nativeOS/restart/regression15–16 do reviewer artifact riêng nghiệm thu; reviewer core không nhận đã tự chạy hoặc nhìn visual/human playtest. Docs/exports/exactHEADCI/PR/dev và CLOSE do root hoàn tất theo workflow; report này không thay các gate đó.

Phạm vi vẫn bounded `personal-weaving-v1`, không full professions/VS03/life generator/viability/TEST001/world/NPC/Knowledge, không command replay mới, IL2CPP/crossplatform/performance/balance/human playtest/final owner art approval. Giữ G13v1/Reopenable, archive, source, goldens và thư viện01–18; không Sprint18/main/tag/release.
