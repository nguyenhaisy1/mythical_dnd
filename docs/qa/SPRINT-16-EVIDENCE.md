# Sprint16 — Xuất thân ngẫu nhiên và cuộc đời được lưu

## Tổng hợp dễ hiểu

Sprint16 bổ sung nhận một người ngẫu nhiên trong tám người trưởng thành đã biên soạn tại Bến Vạc. Màn hình xuất thân hiển thị quá khứ, gia đình, kỹ năng và nhận định của chính người đó; vào cảnh, chờ, lưu và mở lại phải giữ nguyên người và tiến độ. Không có lựa chọn/chỉnh xuất thân, không cấp nghề sửa thuyền giả.

Sprint16 / SPIN-040 bắt đầu06/11, tiếp tục09/11/2026. Phạm vi VS02C và partialVS07 theo [ADR025](../adr/025-seeded-origin-cohort-and-reveal.md), [contract](../contracts/ORIGIN-COHORT-PROFILE.md), [brief](../production/sprints/SPRINT-16.md). Đây nhánh chọn nhân vật có sẵn của Module07, không phải toàn bộ bộ sinh lịch sử cuộc đời hoặc Character Viability/TEST001. G13 Accepted/Reopenable, profile earth-monument-2d-v1 và thư viện01–18 giữ nguyên.

## Điều kiện đóng Sprint16 còn thiếu

Nghiệm thu kỹ thuật local09/11/2026 đạt. Docs/exports local đã kiểm; Git/CI đúng HEAD/PR→dev còn phải hoàn tất trước CLOSE; trạng thái sau commit ghi trong WORKLOG, không tự nhận đã tích hợp dev từ kết quả local.

| Gate | Trạng thái hiện tại |
|---|---|
|1. Determinism/admission/privacy/idempotency|Đạt: focused92/92 (81mới+11cũ), final full674/674/0failedskip và source guard khớp nguồn cuối.|
|2. SQLite atomic publish/cancel/fault/crash/compatibility|Đạt: focused cùng final full process21child calls/3SIGKILL; exact profile/immutability/legacy/refusal checks.|
|3. Actual Unity6000.6.7f1/LinuxMono/two seeds/restart|Actual run8fpgghix Passed: hai seed/19captures mỗi lượt, legacy15 regression đạt; OS two-process restart22lns4xz Passed, cùng assignment/cursor/tick61.|
|4. Render/layout/scroll/modal/OS và repair15 regression|Renderer sáu layouts mỗi seed và repair15 actual Player đạt; OS22lns4xz Passed với20actions và hai process; không lấy automation thay human playtest.|
|5. Independent final review/docs/exports/CI/PR/dev|Hai final reviews độc lập noP0/P1 trong scope và full regression đạt. Docs/exports local đạt; còn Git/CI/PR/dev sau commit, đối chiếu GIT/CLOSE cuối trong WORKLOG.|

Không chuyển gate đã nhận sang backlog để đóng sprint. Kết quả kỹ thuật cuối ở tài liệu này; exactHEADCI/PR/dev và CLOSE sau commit ghi append trong [WORKLOG](../../WORKLOG.md).

## Phân công và nguồn bằng chứng

- `/root/s16_origin_audit`: audit Module07/cohort rồi triển khai Domain selector/model/snapshot propagation và30cases.
- `/root/s16_runtime_audit`: registry compiler, safe session, gateway/pending publish, codec image3/SQLite profile và các tests compiler/persistence.
- `/root/s16_unity`: Bootstrap/View/Autoplay/EditorBuild riêng Sprint16; không sửa scene/proof15.
- `/root/s16_critic_core`: review độc lập source và4ca boundary bắt lỗi sửa metadata origin; [report](sprint-16/CRITIC-CORE.md).
- `/root/s16_critic_ui`: source/early-Editor review và findings UI/error/layout/restart; [draft report](sprint-16/CRITIC-UI.md). Bị usage quota trước final artifact review, không ghi đã hoàn thành.
- `/root/s16_docs`: đọc baseline/impact trước khi bị usage quota; không nhận là đã hoàn tất docs.
- `/root/s16_docs_finish`: tiếp quản consumer docs/registers/integration notes/runbook/export inputs ngày09/11.
- `/root/s16_process_gates`: process gates/SIGKILL/full runtime integration/CI ngày09/11.
- `/root/s16_final_review`: độc lập tiếp quản source/actual artifacts/final review ngày09/11; [report](sprint-16/FINAL-INDEPENDENT-REVIEW.md).
- `/root/s16_core_final_review`: kiểm độc lập nguồn core/regression/process artifacts cuối; [report](sprint-16/CORE-FINAL-REVIEW.md).
- `/root`: ADR/contract/brief/DTO, independent Python vectors, Headless CLI, runners, actual Editor/Player/OS, regression/integration/docs/evidence/exports/Git.

Tối đa4actor đồng thời kể cả root; các actor trước và sau chạy theo đợt. AI automation không là human playtest; self-review không là independent review.

## Focused runtime và lỗi đã sửa

| Artifact | Kết quả |
|---|---|
|`tests/results/sprint16-focused-initial/origin.trx`|82/82Passed trước expanded catalog/boundary/profile cases.|
|`tests/results/sprint16-boundary-before/boundary.trx`|4/4Failed đúng vì durable commit nhận thay actor/household/source/draw metadata sau genesis.|
|`tests/results/sprint16-focused-fixed/origin.trx`|92/92Passed,0failed/skipped sau IMMUTABLE_ORIGIN check trước writer; reviewer độc lập tự parse before/after và review patch.|
|`tests/results/sprint16-probe-first/`|Actual headless New/idempotency/wait1+60/save/reopen/retry, Vạc Lâm tick61 draw1; không là Unity proof.|
|`tests/results/sprint16-client-4g2elcmq/`|Configure FailedCS1061: IResolvedStyle.overflow không có; sửa sang exact IStyle.overflow API. Không Player acceptance.|
|`tests/results/sprint16-client-j0jw0ab7/`|Actual Editor/build và hai callback Player QA chạy; overall runner Failed vì expected Vạc Nhu thay authored display_name Bà Nhu. Giữ failure; không sửa canon/fixture để khớp test.|

## Actual Editor và Player cuối

Raw `tests/results/sprint16-client-8fpgghix/`, runner exit0/Passed. Root chạy `python3 tools/unity/run_origin_client.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet --editor /home/synh/Unity/Hub/Editor/6000.6.7f1/Editor/Unity`. Actual Editor Configure/PrepareScene/Build thành công, S16_BUILD_RESULT Succeeded errors0/warnings0; runtime Unity6000.6.7f1/LinuxPlayer/Mono. Source manifest206paths trước/sau Build/Player giữ nguyên, fingerprint `54b10aaf2ebc6f84f9e455a5e558e2f82004faecb28ca0368fdd504d8d69f6ba`. Managed Assembly-CSharp SHA256 `30f5810c127951530fa98e466b278764e6c4ff01e6916550e8d8ad16071d35f8`.

Hai fresh Player QA:

| Seed QA | Nhân vật và world | Final persisted state |
|---|---|---|
|0×64|Vạc Lâm22tuổi, actor.vac-lam@1;206f8ff78714777f|tick61, draw1, image3, hai AdvanceTime commands|
|1×64|Bà Nhu77tuổi, actor.vac-nhu@1;c9dd2b91ea1d91c0|tick61, draw1, image3, hai AdvanceTime commands|

Mỗi lượt19PNG, six layouts1280×720/1920×1080 ×100/125/150%, top/bottom scroll, header/card/footer/Begin bounds, self-record modal inert/focus, cancellation pending thật và duplicate New blocked.19requiredflags true, keyboard/pointer external events0 trong callback mode. Root và reviewer độc lập xem actual captures; neutral marker được ghi rõ trên UI, không nhận đó là đủ tạo hình mọi nhân vật. Sau Player exit, host mở cùng DB để so safe record fingerprint; đây host reopen, actual Player restart có OS gate riêng.

Trong cùng runner, build/chạy scene15 đã commit với staged core mới, không regenerate scene15: actual LinuxMono repair QA23requiredflags đạt,12captures và ba DB được đọc độc lập. world.db condition700/timber8/hold0/completed; qa-cancel.db500/10/0/cancelled; qa-wait.db500/10/0/noactivity. Cả ba image2/noOrigin; hidden fixtures giữ nguyên. Đây regression Player thực tại `legacy15/`, không chỉ dẫn lại report Sprint15 cũ.

## OS-input và khởi động lại ứng dụng thật

Raw `tests/results/sprint16-input-22lns4xz/`, exit0/Passed. Root chạy `python3 tools/unity/run_origin_input.py --player tests/results/sprint16-client-8fpgghix/Player/MythicalOrigin.x86_64`. Native X11 keyboard/mouse chỉ gửi vào đúng Player PID, abort nếu mất focus. Hai processes2264372 và2264524, cùng binary/managed assembly ở trên, đều exit0. Actual drawable cả hai1280×720,20recordedactions (18+2); không sao chép kích thước683 của Sprint15.

Phase1: Return New→pending publication barrier; Esc cancel không tạo publishedcampaign. New lần mới→RevealVạcLâm22/tick0; Return Begin không thay origin. MouseWaitOne tăng1, SpaceWaitSixty tăng60 đúng một lần; mouseSave saved/durable, ReturnReload loaded/tick61. SelfRecord mở modal, actions nền không có enabled pointer targets; SpaceClose không tăng tick. BackMenu/Continue→cùng Reveal61→pointerBegin World61. F10 capture/quit. Phase2 khởi động binary mới thật, Menu có campaign; Continue không gọi New, cùng Reveal/record/tick61→pointerBeginWorld; F10 quit.

Driver so persisted DB trước/sau phase2 bằng nhau: SHA256 `db4e15c016cabf6cc347d75e84495b96ae5d9c13f6e8cb7e25b6e681c6108a05`, actor.vac-lam@1/world206f8ff78714777f/tick61/draw1/image3/physicalSQLite2, đúng hai AdvanceTime commands. Reviewer độc lập tự đọc actions/current/DB và actual initial/final captures; InputProbe.passed đơn lẻ không thay chuỗi asserts. Đây OS automation, không human playtest.

## Process publication và SIGKILL

Lượt độc lập ban đầu `tests/results/sprint16-origin-process-first.json`, raw `tests/results/origin-process-q7b6k78n/`:21child calls,3SIGKILL exit−9 tại after_initialize/before_publish/after_publish. Hai fresh seed0 dùng path/request khác có cùng initial/final canonical hashes; seed1 khác actor/world. Invalid seed/request không publish; corrupt latest fail closed và giữ bytes, không fallback.

Hai prepublish crash giữ campaign cũ/tick61 và pending orphan nguyên hash; Continue bỏ qua pending, retry mới tạo campaign thứ hai và không xóa orphan/campaign trước. Afterpublish crash giữ một DB đã commit; Continue ra Bà Nhu tick0/draw1. Đây process crash evidence, không tuyên bố power-loss durability hoặc origin command replay. Final full-run1rngpl9l đã lặp gate trên nguồn cuối và đạt; nested report origin-cohort.json, raw origin-process-17kwnzwg.

## Contract và known answers độc lập

Source settlement canonical hash `60ce05463edcc1c2e4abbeae8f30159d430f57613b902ec6d48bb775db193ec8`; derived cohort hash `3b4560e57726f3693a483020924f33121fbfa554892ff912029dd50bdf95c8aa`.

Python reference tạo bốn vectors ở content/fixtures/origin-cohort/selector-vectors.json, không lấy đáp án từ selector C#. Seed0×64 chọn actor.vac-lam@1/Vạc Lâm, world206f8ff78714777f; seed1×64 chọn actor.vac-nhu@1/Bà Nhu, worldc9dd2b91ea1d91c0. Cursor sau chọn là1. Read/Begin/Continue/save/reload không draw lại. Các resource/known answers được kiểm độc lập từ full source pack bởi tools/checks/check_origin_client.py; đây source/reference check, không runtime acceptance.

Canonical image3 chỉ Origin/noRepair; image1/2/goldens và physicalSQLite schema2 giữ nguyên. Exact origin rule/content/source/seed binding; metadata genesis immutable. Replay artifactv1 từ chối Origin/image3, không gọi deterministic genesis/save/recovery là full origin command replay. Published campaign nằm trong DB, không sidecar biography; pending không xuất hiện trong Continue trước atomic publish.

## Full regression và nghiệm thu độc lập cuối

Root thực thi `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`, raw `tests/results/sprint16-1rngpl9l/`: overall exit0,674discovered/executed/Passed,0failed/skipped. Source manifest286paths trước/sau khớp, fingerprint `6b3de7c07749c127d0c26262b9d27497644e6aad699060ae42b1c8fd7ae03665`. Build Release0warnings/errors.81ca mới gồm S16-01=30, S16-02=48 (gồm4independent boundary cases), S16-04=3.593regression cũ gồm113repair cases; focused92 trước đó gồm11ca cũ, không gọi92ca mới.

Negative control đúng một test phát hiện/executed/Failed,childexit1; harness kiểm failure này rồi tiếp tục. Transaction/save SIGKILL, rule-rerun replay/content/diagnostics/conformance/settlement/event-eligibility/repair process gates đạt; origin final21calls/3SIGKILL đã kiểm riêng. Không đổi legacy vectors/parameter packs/archive/01–18. Final docs-only edits không thay runtime manifest.

Hai final reviewers09/11: [Core final](sprint-16/CORE-FINAL-REVIEW.md) kiểm286hashes/11priorcorefingerprints/TRX/negative/process và đọc năm actual crash DBs bằng PythonMDC1 độc lập; [Client và artifacts final](sprint-16/FINAL-INDEPENDENT-REVIEW.md) kiểm source/freshUnity/hai seed/captures/legacy15/OS two-process restart/SQLite/fullsuite. Không còn P0/P1 đã biết trong bounded scope. Hai draft reviews06/11 giữ findings/provenance và trạng thái thời điểm viết, không giả họ đã làm final review sau quota.

## Đồng bộ tài liệu và exports

Đã cập nhật nguồn .agent/AGENTS/ADR025/contract/brief/runbook, README/HANDOFF/plan/roadmap/backlog, baseline/DEC032/open decisions/test matrix và integration notes07/23/26/28/29; không sửa archive nghiên cứu. Regenerate36docs từ nguồn, check_handoff1039authoredlinks/52requirements/28tasks đạt. Source/reference/contract vectors/architecture/safe origin+repair source-assets checks,4offlineMCP tests và6project skilllinks đạt.

Bản xuất local7HTML+7DOCX được regenerate, check_exports301sourceinputs/14containers/6156sourceanchors/6251headings/123tables và navigation đạt. Đây kiểm cấu trúc/nội dung/container, không là visual DOCX render hoặc game acceptance. Exports/results/Player/DLL/native/cache ignored, không commitZIP hoặc artifact biên dịch. Git exactHEADCI/PR→dev sau commit theo WORKLOG; chỉ CLOSE khi các gate đó đạt. Không tự Sprint17/main/tag/release.

## Backlog dự án ngoài sprint

Full causal life generation/Character Viability/TEST001, toàn NPC/world/economy/Knowledge binding, professions/progression/boat repair cho cohort, fullVS02/VS07, tạo hình đầy đủ tám nhân vật/final owner art/UI approval, IL2CPP/cross-platform/performance/balance/human playtest. Neutral marker trong proof16 không được gọi là đủ sprite mọi tuổi/giới. Không tự mở Sprint17/main/tag/release.
