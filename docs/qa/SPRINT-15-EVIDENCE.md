# Sprint15 — Bản thử sửa thuyền Unity 2D

## Tổng hợp dễ hiểu

Đã đưa luật sửa thuyền vào một cảnh game Unity thật tại Bến Vạc: có nền phân lớp, nhân vật sprite, thuyền và giao diện tiếng Việt. Có thể xem vật liệu/thời gian, bắt đầu sửa, chờ, hủy, lưu và mở lại. Luồng bàn phím và chuột đã được kiểm bằng input hệ điều hành thật; việc sửa/lưu dùng core và SQLite, không do giao diện tự tính kết quả.

Sprint15 / SPIN-039 bắt đầu03/11, tiếp tục04/11 và06/11/2026. Bản này dùng **nhân vật cố định**, có nhãn rõ trên màn hình. Đây là proof kỹ thuật `repair-client-2d-v1`; hướng Đất dựng thành `earth-monument-2d-v1`, G13 Accepted/Reopenable và thư viện01–18 giữ nguyên. [ADR024](../adr/024-bounded-2d-repair-client.md), [contract](../contracts/REPAIR-CLIENT-PROFILE.md) và [brief](../production/sprints/SPRINT-15.md) xác định phạm vi.

## Điều kiện đóng Sprint15 còn thiếu

| Gate bắt buộc | Bằng chứng và trạng thái |
|---|---|
|1. Safe session/Unknown/privacy/persistence|Focused31/31 đạt;25 session/boundary và6 actualSQLite; App/Infra nguồn không đổi từ focused review. Full regression cuối bên dưới.|
|2. Actual Editor/bootstrap/import/scene/LinuxMono build|Đạt: project/version/packages/meta do actual Editor tạo; bo3487zv build0errors/0warnings, actual Unity6000.6.7f1/LinuxPlayer/Mono.|
|3. Player repair/cancel/wait/save/reopen|Đạt: Running save/reopen,59→60complete; cancel/wait nhánh riêng. Đọc final entities/activity/command counts từ ba SQLite files, không chỉ tin DTO giữa lượt.|
|4. Render/occlusion/layout/modal/input|Đạt:12ảnh renderer, six cases1280×720/1920×1080×100/125/150%; root và critic đã xem. OS yc1_bzfx có18actions, bàn phím và hai gesture chuột tách biệt đúng.|
|5. Independent review/regression/docs/CI/dev|Local đạt: hai review độc lập không còn P0/P1, full593/593 và process gates đạt. Docs/export kiểm trước commit; exactHEADCI/PR/dev được ghi sau commit trong WORKLOG và bắt buộc đạt trước CLOSE.|

**Nghiệm thu kỹ thuật local06/11/2026 đạt; điều kiện Git/CI để đóng sprint phải đối chiếu GIT/CLOSE cuối trong WORKLOG.** Chưa có GIT/CLOSE đạt thì chưa kết luận đóng sprint. Git/CI sau commit được append vào [WORKLOG](../../WORKLOG.md); không sửa lịch sử để tự chứa hash. Không đưa gate chưa đạt sang backlog.

## Phân công thực tế

| Actor | Công việc |
|---|---|
| `/root/s15_projection_audit` | Audit rồi viết phần đầu safe projection/session và tests; chưa chạy build |
| `/root/s15_unity_audit` | Audit Editor/modules/localAPI rồi viết SceneView/Build hooks ban đầu; chưa chạy Editor |
| `/root/s15_art` | Base/reference và các lượt art ban đầu trước gián đoạn |
| `/root/s15_session_finish` | Tiếp quản App sau ngắt phiên, siết bound activity/retry,23cases; source freeze |
| `/root/s15_unity_finish` | Bootstrap/View/Autoplay, namespace fix, modal/focus/layout/runtime feedback và actual backend detection; root chạy Editor/Player |
| `/root/s15_art_finish` | Phục hồi4generation cũ, tạo thêm1actor sheet, xuất6PNG/provenance/QC; không gọi composed preview là Unity render |
| `/root/s15_critic_boundary` | Review độc lập App/Infra, hai ca regression observation gate và đọc actual TRX; [report](sprint-15/CRITIC-BOUNDARY.md) |
| `/root/s15_critic_client` | Review độc lập Unity/runner/assets và runtime/visual evidence; [report](sprint-15/CRITIC-CLIENT.md) |
| `/root/s15_docs_sync` | Đồng bộ16nguồn docs/registers/backlog/matrix/integration notes/export inputs; không chạy build/export/runtime |
| `/root` | Điều phối, ADR/contract/brief, Infra fixture+SQLite,6persistence tests, font/license, runner/staging/static CIcheck, actual Editor/Player/input/visual QA, regression/evidence/exports/Git |
| `/root/s15_input_fix` | Sửa input isolation/Space/readiness và mouse completed-gesture sau actual failure; không tự claim compile/runtime |
| `/root/s15_client_final_review` | Review độc lập tiếp quản sau ngắt phiên: hashes/captures/layout/SQLite/OS actions, xác minh fixes; chỉ sửa report |
| `/root/s15_final_boundary_review` | Kiểm lại7fingerprints App/Infra/tests, final SQLite checker/codec known-answers và runtime artifacts; chỉ sửa report |
| `/root/s15_docs_final` | Đồng bộ8consumer docs/registers/integration notes với kết quả Unity/OS cuối; không sửa generated modules |

Actors chạy theo đợt tối đa4 kể cả root. Sau ngắt phiên actors trước không còn live, source dở được tiếp quản bằng actor mới; không ghi họ đã hoàn thành phần chưa làm. AI review/automation không phải human playtest.

## Actual Editor/Player cuối

Raw output: `tests/results/sprint15-client-bo3487zv/`. Lệnh root thực thi:

```sh
python3 tools/unity/run_repair_client.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet --editor /home/synh/Unity/Hub/Editor/6000.6.7f1/Editor/Unity
```

Exit0/Passed; `S15_BUILD_RESULT Succeeded errors=0 warnings=0`. Actualversion6000.6.7f1, revisionb22767f04f04, platformLinuxPlayer, backendMono đo từ runtime. Configure/PrepareScene/Build chạy actual Editor; portableDLL/nativeSQLite staging có pinned imports/hash. Assets/Scenes/Sprint15Repair.unity, ProjectSettings, Packages lock và source .meta là dữ liệu Editor thật; DLL/native/Library/Temp/Player/results ignored.

Source fingerprint trước/sau build và Player giữ nguyên183paths:

```text
51564bd28a32a630279353015603d1deed1711805414d10ec56d23b6c680fb41
```

Actual managed `Assembly-CSharp.dll` SHA256 (khớp OS-input build):

```text
a507b552b10a726b191cfdfca33f66c41897b3d6d17aad537b88740aa67090ef
```

Callback QA kiểm23requiredflags,6layouts và12capture names; `inputReady=false` đúng vì đây không phải OS probe. Autoplay isolate physical/native events, keyboardEvents0 và interceptedInputEvents0 ở lượt cuối. Explicit modal/background action/NavigationSubmit guards vẫn được kiểm qua callback probe; không lấy isolation để suy normal input đã đúng.

| Frame | Tick | Gỗ đang có | Giữ chỗ | Condition | Activity |
|---|---:|---:|---:|---:|---|
|Initial|0|10|0|500|none|
|Start|0|10|2|500|running|
|Trước hạn|59|10|2|500|running|
|Complete|60|8|0|700|completed|
|Cancel branch|0|10|0|500|cancelled|
|Wait branch|60|10|0|500|none|

Sau toàn bộ captures/occlusion waits còn Reload và so completed frame lần nữa. Runner độc lập đọc MDC1 persisted entities/receipts: world.db có đúng1StartRepair/1completed activity, condition700/onhand8/reserved0; qa-cancel.db đúng1StartRepair/1cancelled activity,500/10/0; qa-wait.db khôngStartRepair/activity,500/10/0. Hidden boat321/private lot917 giữ nguyên. Metadata world15/ticks60,0,60/seed/profile/contenthash/physicalschema2 đúng; không auto-upgrade hoặc overwrite foreign/corrupt world.

## Render và input thật

Trong `autoplay/`: initial và scroll-bottom1280; sáu preview cases; completed125%, occlusion-front/behind và waited1920. Root và critic xem chữ Việt, cost/time/Confirm, chân nhân vật, lớp che và boat variant. ScrollView có runtime theme, clip viewport, focus-scroll và lower-controls check; không che action để làm UI vừa màn hình. Không còn dải đen trên/phải ở ma trận cuối. Hai idleframes với cadence/tilt khi Running là presentation feedback, chưa là full authored working animation.

OS raw: `tests/results/sprint15-input-yc1_bzfx/`; root chạy:

```sh
python3 tools/unity/run_repair_input.py --player tests/results/sprint15-client-bo3487zv/Player/MythicalRepair.x86_64
```

Exit0/Passed, đúng Player PID/window,18recordedactions và matched managed-assembly hash. Driver abort nếu focus sang app khác. Native Tab→Back→Return không commit, Return trên Confirm giữ2gỗ; S/L trả saved/loaded durable; Space với focus Repair rồi Wait tăng đúng1tick/lần; C hủy ởtick2 trả hold0; Esc chỉ đóng preview. Mouse-down ởthuyền chưa mở/xác nhận; release mở pure preview hold0/picks1; một cú nhấp Confirm mới mới tạo activity Running hold2, không tăng scene picks. F10 capture/quit0. Read-only final OS DB đúng5receipts: StartRepair×2,AdvanceTime×2,CancelActivity×1; hai Start đều có intent riêng trong script, không lệnh thứ ba.

Actual OS drawable **1280×683** do decorated-window/WM, không nói nó là720 chỉ vì CLI request720. Các ảnh OS là evidence thao tác bổ trợ; sáu ma trận layout1280×720/1920×1080 có renderer captures riêng.1920 autoplay dùng borderless fullscreen để đủ drawable. Callback automation, OS automation và visual review đều không là human playtest hoặc final user art/UI approval.

## Failures và cách sửa được giữ lại

| Artifact | Kết quả lúc chạy và xử lý |
|---|---|
|sprint15-bootstrap/create-editor.log|ActualcreateProject exit0/entitlement resolved, không tạo ProjectVersion giả.|
|sprint15-focused-initial/client-session.trx|29/29Passed trước hai independent regression cases.|
|sprint15-boundary-before/boundary.trx|2cases:1pass/1fail; recipe observation so displaylabel thay worksite identity, lộ cost2 khi actor ở ngoài.|
|sprint15-focused-fixed/client-session.trx|31/31Passed sau root fix identity; critic tự đọc before/after.|
|sprint15-client-puu3o6ny|FailedConfigure7CS0234: Game.Application che UnityEngine.Application; qualify references.|
|sprint15-client-4_4rkhzh|Runner logicPassed nhưng visual gateFailed: thiếu runtime theme/scroll và dải đen69hàng/36cột; không nghiệmthu chỉ vì runner xanh.|
|sprint15-client-zpthuet6|FailedREPAIR_RESOLUTION khi1920fullscreen→1280window actual683; đổi capture flow, giữ repairflow1920 sau ma trận.|
|sprint15-client-21bx2wn3|Runner cũPassed nhưng kiểm ảnh+finalDB phát hiện Return lọt vào autoplay, thêmStartRepair ởtick60/hold2 sau sample; không nghiệmthu. Thêm input isolation/delayedstate/finalDB checker. Checker mới từ chối đúng DB lỗi này.|
|sprint15-client-gl6e8m11|Callbacks/finalDB đúng nhưng overallFailedSourcesChanged lúc obsoleteAPI/runner fix; phải rebuild nguồn cuối. PreventDefault đổi sang exactAPI IgnoreEvent/StopImmediatePropagation.|
|sprint15-client-0zqlba64|Build/callback/render/finalDBPassed ở revision trước mousefix; chưa đóng OS gate.|
|sprint15-input-1fcn8ks8|Failedreadytimeout,0actions; chuyển readiness handshake sang inputReady telemetry và bounded90s wait.|
|sprint15-input-5prtp46v|Keyboard đúng nhưng boatclick tự Confirm cùnggesture; giữ Failed. Fix press-origin+matchingrelease, dispatch sau native release; modal invalidates pending gesture.|
|sprint15-client-bo3487zv + input-yc1_bzfx|Final build/callback/render/finalSQLite và normal OS-input đạt sau fixes, same managed assembly.|
|sprint15-r365u194 và9s6r3iz2|593/593C#cases và negative/process gates chạy; overallExit1SourcesChanged do sửaUI/runner, không lấy làm final revision acceptance.|

## Full regression và review cuối

Root thực thi `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet` tại `tests/results/sprint15-l08y6ql5/`, exit0. Release build0warnings/errors;593discovered/executed/Passed,0Failed/Skipped,25S15-01 và6S15-04 cùng113repair regression đều Passed. Negative control riêng phát hiện đúng1testfail/exit khác0, không tính lỗi chủ đích này vào suite Passed.

Full source revision `sha256:60746b3f12cf8d18502517266afe451ae0870f9809cd18dd724ce14634b0c04d` giữ nguyên suốt run. Process gates actual SQLite SIGKILL/current-state/snapshot/migration, command replay, content/diagnostics/settlement/event eligibility và repair reopen/recovery/replay đều đạt cả positive/negative controls. Host conformance preparation vẫn ghi UnityNotRun đúng scope riêng; actual Unity proof Sprint15 nằm trong runnerPlayer phía trên, không tráo headless với Unity. Legacy goldens không bị overwrite.

[Boundary review](sprint-15/CRITIC-BOUNDARY.md) và [client review](sprint-15/CRITIC-CLIENT.md) ghi actor/evidence/fingerprints thật. Hai critic06/11 tự đọc artifacts do root thực thi, không giả họ tự chạy Editor/OS/test. Worker không tự là independent reviewer của chính source mình.

## Tài liệu, compatibility và Git

Nguồn ADR024/clientcontract/runbook, brief/evidence, .agent/AGENTS, README/handoff/roadmap/backlog/matrix/decision registers/integrationnotes và generated GDD/exports được đồng bộ theo actualscope. Full world simulation vẫn chưa implemented; không sửa archive, golden expectations hoặc nghiên cứu dài để khớp proof. Repair image2/rule/contentprofile và physicalSQLite schema2 giữ nguyên; noRNG/portablecore/owner boundaries giữ.

Source/asset checks và docs/export freshness phải chạy sau report cuối. CI chỉ chạy source/reference/headless; actual Unity/Player evidence là local run được ghi riêng, không tuyên bố GitHub đã chạy licensed Editor. Feature→PR→dev chỉ sau review và exactHEADCI đạt; commit/remoteSHA/PR/CI/dev và CLOSE append trong WORKLOG. Không main/tag/release/ZIP hoặc Sprint16.

## Backlog dự án ngoài Sprint15

Full random life/onboarding, full Knowledge/Perception/NPC/settlement binding, đủ ba nghề, fullVS03/VS07, PlayerIL2CPP/cross-platform, final user art/UI approval, performance/balance và human playtest vẫn là backlog dự án. Chúng không phải các điều kiện đóng Sprint15 còn thiếu. S13-A02/A03 vẫn follow-up; G13 mở lại trên code hiện tại, không rollback core hay tiến độ về checkpoint13.
