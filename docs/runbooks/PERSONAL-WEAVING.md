# Build và kiểm tra đan dây cá nhân trên xuất thân

## Mục đích và phạm vi

`personal-weaving-v1` là subset chế tác Sprint17: New → Reveal → World → xem trước → xác nhận → giữ nguyên liệu → chờ nhận dây; có hủy, lưu khi đang làm, actual Player restart và Continue. [ADR026](../adr/026-personal-weaving-on-origin.md), [contract](../contracts/PERSONAL-WEAVING-PROFILE.md), [brief17](../production/sprints/SPRINT-17.md) và [evidence17](../qa/SPRINT-17-EVIDENCE.md) là nguồn quyết định/gates/kết quả. Lệnh trong runbook không là bằng chứng đã chạy hoặc Passed; evidence phải ghi revision, nguồn, artifacts và actor thật.

Nguồn skills là cohort16: Vạc Lâm weaving43 và Bà Nhu61 đủ threshold40. Sáu người khác vẫn được nhận ngẫu nhiên bình thường; UI ghi bản thử chưa hỗ trợ công việc phù hợp xuất thân này, không bảo họ không có nghề, cấp weaving/boat-repair giả hoặc ép tạo lại. Không chọn/chỉnh/reroll ở Reveal. Tạo campaign mới là thao tác riêng từ menu, không xóa campaign trước.

Mỗi campaign17 có8fiber cá nhân mới của bản thử và0cord, hai ResourceLots thuộc chính actor tại home; không quyền quản lý hộ hoặc kho hộ. Đây prototype-authored genesis theo ADR026, không suy từ lịch sử/wealth hay dữ kiện khảo cổ. Recipe dùng tay2fiber/30ticks/1cord, không tool item/workshop mới, không balance cuối hoặc profession/class/progression hoàn chỉnh. Full ba nghề VS03/fullVS02/VS07 và full generator/viability vẫn riêng.

## Chuẩn bị

Chạy từ repository root với .NET SDK theo global.json; actual Unity6000.6.7f1 đã kích hoạt, LinuxStandaloneSupport/Mono và DISPLAY Linux x64. Đóng Editor đang giữ cùng `client/UnityProject` trước build. Host net10 phục vụ tests/diagnostics; Unity chạy portable netstandard2.1 DLL trên Mono, không load host executable. Native OS driver cần X11/xdotool và focus đúng Player process.

Dùng thư mục kết quả/save mới riêng cho mỗi lượt; không xóa dữ liệu người dùng để thử. Runner giữ cả lượt lỗi, logs/PNG/SQLite/ready markers và report để điều tra. Imports/DLL/native.so/Player/SDK/caches/results là artifacts ignored, không commit. Không tạo ZIP hoặc sửa ảnh để che clipping.

## Source, runtime và actual-process checks

```sh
python3 tools/checks/check_weaving_client.py
/path/to/dotnet restore MythicalDnd.sln --locked-mode -m:1 -nr:false
/path/to/dotnet build MythicalDnd.sln -c Release --no-restore -m:1 -nr:false
python3 tools/tests/check_weaving.py --dotnet /path/to/dotnet --output tests/results/weaving-process-report.json
```

Source checker kiểm parameter/resource/canonical profile, safe presenter boundaries và các reference yêu cầu; không chạy C# hoặc Unity. Build thành công không thay runtime tests. Chạy full suite/negative/process theo README và test runner của repository trên nguồn cuối; focused tests chỉ nghiệm thu các cases đã thực thi.

Process driver gọi trusted headless CLI thật trên SQLite files riêng: Lâm/Nhu complete, Lâm cancel, Bến Kiên blocked, same-seed genesis/effects qua fresh requests/paths, invalid seed không publish; SIGKILL New ở after_initialize/before_publish/after_publish giữ campaign cũ; SIGKILL Start và completion trước/sau durable commit, retry sau response loss. Nó đọc raw crash state riêng trước Continue: trước completion commit có Running/tick29/hold2; Continue drain workflow target30 đã được nhận và trả Completed6fiber/0reserved/1cord đúng một lần. Sau completion commit cũng phải finish accepted workflow để UI không mắc ADVANCE_IN_PROGRESS. Không gọi kết quả này là power-loss hoặc command replay.

Các preview token/correlation được sinh riêng là command inputs khác; same seed không yêu cầu hash toàn bộ command history giống nhau khi token khác. Genesis/selection/material effects và RNG cursor phải khớp known answers; không dùng final hash khác để che sai effects.

## Build và chạy actual Unity

```sh
python3 tools/unity/run_weaving_client.py --dotnet /path/to/dotnet --editor /path/to/Editor/Unity
```

Tùy chọn `--output /path/to/new-directory` yêu cầu thư mục chưa tồn tại; mặc định tạo `tests/results/sprint17-client-*`. Runner thực hiện:

1. Locked restore/build Release, source/reference checker, stage portable/pinned SQLite managed/native dependencies qua conformance pipeline.
2. Actual Editor chạy `RepairClientBuild.Configure`, `WeavingClientBuild.PrepareScene`, `WeavingClientBuild.Build`; scene mới `Assets/Game/Scenes/Sprint17Weaving.unity` và binary `<run>/Player/MythicalWeaving.x86_64`. Không regenerate scene15/16 để giả regression bằng scene17.
3. Khóa source manifest trước/sau build/Player, ghi Editor/binary/managed Assembly-CSharp hashes. Nguồn đổi giữa lượt làm evidence cần chạy lại phần bị ảnh hưởng.
4. Chạy ba actual Player QA processes tách save root: seed64ký tự0→Vạc Lâm, seed64ký tự1→Bà Nhu, seed`01234567` lặp8→Bến Kiên. Seed chỉ trusted QA CLI, không normal UI. Tất cả giữ origin assignment/draw1; chỉ hai qualified craft, nonweaver không có activity/job/receipt craft.
5. Kiểm base New/cancel/Reveal/Begin/self-record/wait/save/reload/menu/Continue, qualified preview pure/Start/saveRunning/reopen/beforeDue/completion/cancel; nonweaver denial. Ma trận actual renderer1280×720/1920×1080 ×100/125/150%, scroll top/bottom, World/work/self-record/preview modal theo QA capture set. Dự kiến42PNG mỗi qualified và32PNG nonweaver; counts/flags/geometry không thay việc mở ảnh kiểm chữ/che khuất/scroll/modal/focus.
6. Sau Player exit, đọc SQLite bằng Python độc lập: canonical image4/exact rule-content/schema2, seed/world/origin/cohort/source/cursor, authored skill/recipe/own rights, quantities/holds/activities/typed jobs, mirrors/commands/WeavingCompleted đúng due. Qualified hoàn thành một và hủy một:6fiber/0reserved/1cord/tick91; nonweaver8/0/0/tick61. Trusted host reopen đối chiếu own-record fingerprint; host reopen không phải actual Player restart.
7. Build/chạy actual scene16 origin Player và scene15 repair Player từ cùng staged core. Kiểm scene16 image3/record/tick61/cursor1 và scene15 image2/repair effects/cancel/wait/three DBs/captures. Không thay bằng evidence15/16 cũ hoặc dotnet-only regression.

`evidence.json`, `sources.json`, Editor/runtime logs, QA JSON/PNG và DB dưới cùng run phải liên kết được. Runner exit0 chỉ bao phủ assertions thật trên source fingerprint đó; independent review, native OS restart, full suite/docs/exports/CI là gates bổ sung.

## Mở binary và thao tác bình thường

Chạy `<run>/Player/MythicalWeaving.x86_64`. Root mặc định dưới `Application.persistentDataPath/personal-weaving-v1`; trusted CLI `-s17SaveRoot /path/to/separate-demo-root` tách demo. UI không có ô path/seed/candidate, không enumerate root16.

| Thao tác | Kết quả mong đợi |
|---|---|
| Menu → cuộc đời mới / Generating → Hủy hoặc Esc | Một request New; cancel trước atomic publication không thay campaign Continue; sau publication giữ commit |
| Reveal → Bắt đầu | Chỉ đổi màn hình, không RNG/mutate origin |
| World → xem trước đan dây | Hiện giá2fiber/thời gian30phút/sản phẩm1dây, token opaque; chưa giữ hoặc mất đồ |
| Preview → Xác nhận | Gesture riêng; revalidate actor/skill/location/rights/available/busy/output range; reserve2,8OnHand/2Reserved/0cord |
| Running → chờ29 rồi1phút | Trước due không output; due consume2/release2/create1cord cùng tick/history/job receipt commit |
| Running → Hủy công việc | Release đúng2hold, không cộng OnHand hoặc cord; không double khi retry |
| Running → Lưu / Mở lại / menu/Continue | Giữ công việc/due/holds/record; latest durable state, không rollback slot |
| Nonweaver | Xem bản thân/chờ/lưu/Continue bình thường; craft disabled, thông báo phạm vi chưa hỗ trợ công việc phù hợp |
| Tab, Enter/Space, Esc, chuột | Focus native; một submit/gesture chỉ một hành động; modal khóa nền và trả focus khi đóng |

Presenter chỉ nhận safe origin/work DTO: own quantities/skill/status/tick/due/scope note, không world/processor/registry/raw IDs/seed/private revision/SQL/path/other actors. Starter quantities có explicit observation và own committed outcomes cập nhật grants; ownership không cấp tri thức tổng quát. Preview thuần state/RNG; stale cần xem lại. Start/Cancel/Advance retries giữ original envelope trước stale-token check; changed intent trả lỗi hữu hạn, không exception details. UI không tự cộng đồ hoặc WorldTick.

Ảnh/font/assets15 và G13v1 giữ; neutral location marker không là đủ cast art. Camera/footpoint là presentation, không Travel/collision/world location mutation. Không tạo asset/image/model3D mới trong Sprint17.

## Native OS input và actual Player restart

```sh
python3 tools/unity/run_weaving_input.py --player /path/to/Player/MythicalWeaving.x86_64
```

Driver phải gửi phím/chuột vào đúng PID/window sau inputReady, dừng nếu mất focus sang app khác; gọi trực tiếp callbacks không là OS input. Preview mở và Confirm là hai gesture riêng. Kiểm keyboard/pointer focus/modal/scroll, wait/cancel/save/reload và input once bằng actual committed results; không suy từ marker `passed` đơn lẻ.

Driver hiện kiểm phase1: cancel New pending → New/Reveal/Begin → mở preview rồi Esc giữ idle → preview và Confirm riêng → hủy releasehold → Start mới → chờ1 → saveRunning → mở/đóng self-record không action nền. Sau process1 phải có8fiber/2reserved/0cord/tick1, một cancelled và một Running. Phase2 khởi động binary thành process mới thật trên cùng save root, Continue cùng origin/job/hold/due → Begin → nativeSpace chờ60 đến tick61, nhận6fiber/0reserved/1cord → save/reload/menu/Continue giữ completed.  Đọc final SQLite độc lập trước/sau restart để thấy không mất/nhân đôi vật liệu, sản phẩm hoặc receipt. Ghi hai PIDs, actual resolution/textscale/action trace, assembly hash khớp renderer và DB hashes. Drawable resolution thực có thể khác yêu cầu window manager; không đổi wording để giả khớp ma trận renderer. Đây automation kỹ thuật, không human playtest.

`-s17Autoplay <new-output>` và `-s17InputProbe <new-output>` là trusted QA modes; `-s17QaSeed <64lowerhex>`/barriers chỉ được bật trong QA theo bootstrap. Không đưa QA seed, pause hooks hoặc raw diagnostics vào normal UI.

## Trusted headless diagnostics

Headless CLI dùng pack và parameters authored, không là player API. Sau Release build:

```sh
/path/to/dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --weaving-open content/definitions/vertical-slice/pack.json rules/parameters/personal-weaving-v1.json /path/to/separate-profile17-root
/path/to/dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --weaving-inspect content/definitions/vertical-slice/pack.json rules/parameters/personal-weaving-v1.json /path/to/separate-profile17-root
```

`--weaving-inspect` đọc raw durable state, không resume; `--weaving-open` đi qua gateway/factory/Continue và drain accepted Advance. `--weaving-new <pack> <parameters> <root> <seed64lowerhex> <request32lowerhex>` tạo campaign riêng; `--weaving-probe` cùng arguments thêm `complete|cancel|blocked|running|before-due`. `--weaving-retry-start <pack> <parameters> <root>` kiểm original retry sau response-loss. Crash modes `--weaving-crash` và `--weaving-action-crash` cùng ready-marker/probe phases là fault-injection của process driver, không chạy trên save người dùng. CLI stdout có trusted IDs/hashes/diagnostics; không chuyển thẳng sang UI.

## Resource, compatibility và giới hạn mở save

Nguồn là `content/definitions/vertical-slice/pack.json` cùng `rules/parameters/personal-weaving-v1.json`; derived Unity resource `client/UnityProject/Assets/Game/Resources/Sprint17/personal-weaving-v1.json` phải khớp strict compiler/source hash. Không hand-edit cohort hoặc pack/archive để làm như fiber đã có trong lịch sử. Origin.CohortContentHash vẫn là cohort16 hash; profile content hash mới bind cohort và parameters, recipe fingerprint là canonical hash parameter document.

Exact rule `transfer-resource-v1+scheduler-v1+origin-cohort-v1+personal-weaving-v1`; canonical image4 chỉ cho Origin+Weaving, physicalSQLite schema2 giữ. Legacy image1/2/3 bytes/goldens/profile/factories/scene15–16 giữ. Root17 riêng, không nhập root16/auto-upgrade/enrich hoặc fallback factory; replay artifactv1 vẫn từ chối Origin/WeavingRuleVersion. Deterministic genesis/save-reopen/đọc images không là command replay.

Atomic New giữ pending→verify/reopen/flush→create-new publish; `origin-<10digits>-<32lowerhex-requestId>.db` chỉ trong root17. Pending/orphans không là Continue, không xóa foreign files. Same requestId mở lại campaign đã publish, không seed/draw lại; incompatible/corrupt latest fail closed và giữ file để điều tra, không tự fallback/reroll/overwrite.

Factory17 hiện chỉ nhận bounded own actor còn ở home, đúng hai personal lots với own authority; không tự mở rộng tri thức/quyền khi foreign/current owner/location thay đổi. Domain không pin current alive/location/owner vào Origin; trusted tests phải kiểm revalidation/cancel khi chúng đổi. Đây không là Travel/rights/Health commands end-to-end hoặc client hỗ trợ mọi world variant. Metadata Origin và weaving genesis/recipe/skill/IDs immutable; activities/holds/history hợp lệ thay đổi qua commands. TransferResource không hỗ trợ trong profile17 và bị từ chối, các profile cũ giữ.

## Năm gate và báo cáo đóng sprint

1. Core rules/history/invariants/rights/qualification/busy/overflow/revalidation/determinism/no extra RNG và independent adversarial/full regression.
2. Actual SQLite Running/save/reopen→complete/cancel, original retries/response loss/fault/SIGKILL, atomic New, snapshots/recovery/immutability/exact refusal; legacy images/source/goldens giữ.
3. Actual Unity6000.6.7f1/LinuxMono Lâm/Nhu/nonweaver, real DB effects/due/receipts, saveRunning/actual restart/Continue không double; actual scenes15–16 Player regression.
4. Six renderer resolution/textscale cases đọc được; native OS focus/modal/preview+Confirm/wait/cancel/save/restart/input once; final SQLite độc lập.
5. Independent source/artifact reviews không còn P0/P1, final source guard/full runtime/negative/process/docs/exports/exactHEADCI và feature→PR→dev.

Chỉ CLOSE khi đủ cả năm trên nguồn cuối. Báo cáo **Điều kiện đóng Sprint17 còn thiếu** riêng với **Backlog dự án ngoài sprint**; không chuyển gate thiếu ra backlog để đóng. Backlog gồm full professions/three-profession VS03, full generator/viability/TEST001, world/NPC/Knowledge, trading/transport/medicine, tools/quality/progression, final owner art/UX/humanplaytest/performance/cross-platform và replay profile mới. Không tự Sprint18/main/tag/release.
