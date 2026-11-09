# Build và kiểm tra nhận xuất thân trong Unity2D

## Mục đích và phạm vi

`origin-cohort-v1` là proof Sprint16 cho đường New campaign → Generating/cancel → Reveal → World → tự xem bản thân/chờ/lưu → menu/Continue. [ADR025](../adr/025-seeded-origin-cohort-and-reveal.md), [contract](../contracts/ORIGIN-COHORT-PROFILE.md), [brief](../production/sprints/SPRINT-16.md) và [evidence](../qa/SPRINT-16-EVIDENCE.md) là nguồn scope/gates/kết quả. Runbook mô tả cách chạy; lệnh có trong tài liệu không tự là bằng chứng đã đạt.

M07 cho phép chọn existing character hoặc sinh mới từ World Graph. Sprint này chọn một trong tám authored adult còn sống, có home tại Bến Vạc và refs hợp lệ trong settlement pack. Đây không là full life-history generator/Character Viability/TEST001; chưa bind toàn24NPC/economy/Knowledge hoặc sinh truyện bằng LLM. Người chơi không nhập seed, chọn stat/origin/class, sửa biography hoặc reroll ngay trong Reveal. Tạo campaign mới từ Menu là thao tác mới riêng, giữ campaign trước.

Cohort không có kỹ năng boat-repair. Không cấp nghề/quyền giả để cho người mới sửa thuyền. Proof Sprint15 dùng fixed trusted actor/worksite ở scene/profile riêng và phải regression actual Player; xem [Repair Client](REPAIR-CLIENT.md). Origin image3 không nhập vào repair factory/image2.

## Chuẩn bị và build acceptance

Từ repository root, cần .NET SDK theo global.json, Unity6000.6.7f1 đã kích hoạt cùng LinuxStandaloneSupport/Mono và DISPLAY Linux x64. Đóng Editor đang giữ cùng project trước khi chạy. SDK host net10 chỉ phục vụ build/tests; portable assemblies target netstandard2.1 và chạy trên Unity Mono. Không tải host executable vào Unity.

```sh
python3 tools/checks/check_origin_client.py
python3 tools/unity/run_origin_client.py --dotnet /path/to/dotnet --editor /path/to/Editor/Unity
```

`check_origin_client.py` kiểm source/reference: resource derived exact từ settlement pack, canonical hashes, selector vectors độc lập và safe presenter boundaries. Nó không chạy C#/Unity và không thay gate runtime.

Runner actual tạo thư mục mới `tests/results/sprint16-client-*`; `--output /path/to/new-directory` yêu cầu chưa tồn tại. Các phase:

1. Locked restore/build Release và source/reference check. Stage portable core/pinned SQLite dependencies/native Linux theo `run_conformance.py`; imports/DLL/native.so là ignored artifacts, không commit chúng.
2. Chạy `RepairClientBuild.Configure`, `OriginClientBuild.PrepareScene`, `OriginClientBuild.Build` trong actual Editor. Scene mới `Assets/Game/Scenes/Sprint16Origin.unity`; binary `<run>/Player/MythicalOrigin.x86_64`. Không regenerate scene15 để biến regression thành scene mới.
3. Khóa source manifest trước/sau build và Player; ghi Editor/binary/managed assembly hashes. Source đổi giữa lượt làm evidence không hợp lệ và phải chạy lại phần chịu ảnh hưởng.
4. Chạy hai Player QA riêng với hai seed known-answer: seed64ký tự0 → Vạc Lâm/world206f8ff78714777f; seed64ký tự1 → Bà Nhu/worldc9dd2b91ea1d91c0. Cả hai giữ draw cursor1. Seed QA là trusted CLI, không xuất hiện trong normal UI.
5. Kiểm New/cancel-beforepublish/duplicate New, Reveal, Begin không mutate, self-record modal, chờ1+60phút, save/reload, menu/Continue cùng người/tick61. Chụp ma trận1280×720 và1920×1080 ×100/125/150%, top/bottom scroll và World/self-record. Expected19PNG mỗi seed; kiểm cả geometry và actual ảnh, không chỉ telemetry cờ Passed.
6. Sau Player thoát, trusted host mở lại cùng SQLite để đối chiếu safe record/final image3/profile/seed/cursor/command counts. Đây host reopen sau Player exit, chưa phải actual Player process restart.
7. Build/chạy lại repair15 từ cùng staged core; đọc final SQLite effects/cancel/wait và captures thật để phát hiện regression image2/factory/client. Bằng chứng15 cũ không thay actual regression16.

Lượt lỗi giữ nguyên logs/PNG/SQLite/evidence. Exit0 của runner chỉ có ý nghĩa cho các điều kiện nó thực sự kiểm trên source fingerprint đó; không thay independent review, OS-input, full suite, CI hoặc human playtest. Không sửa/cắt capture để che clipping.

## Mở binary và thao tác

Chạy `<run>/Player/MythicalOrigin.x86_64` bình thường để dùng Menu. Save root mặc định dưới `Application.persistentDataPath/origin-cohort-v1`. Trusted CLI `-s16SaveRoot /path/to/separate-demo-root` tách dữ liệu demo; normal UI không có textbox nhập path/seed/candidate. Dùng thư mục tách biệt, không xóa campaign người dùng để thử.

| Màn hình/thao tác | Hành vi |
|---|---|
| Menu → Bắt đầu cuộc đời mới | Async New với một requestId; khóa New trùng khi pending |
| Generating → Hủy hoặc Esc | Yêu cầu cancel trước atomic publication; sau publish phải giữ campaign đã commit |
| Reveal → Bắt đầu ở Bến Vạc | Chỉ đổi screen; không draw RNG/mutate xuất thân |
| World → Chờ1/60phút | Core AdvanceTime/Drain, không phụ thuộc frame time |
| World → Lưu / Mở lại dữ liệu đã lưu | Durable snapshot / latest committed state, không là rollback slot |
| World → Nhật ký bản thân | Cùng self-known record; modal khóa actions nền và trả focus khi đóng |
| Về menu → Tiếp tục cuộc đời đã lưu | Latest published campaign → cùng Reveal → Begin tại saved tick |
| Tab, Enter/Space | Native focus/submit nút đang focus; không có global Space wait thứ hai |
| Esc trong self-record | Đóng modal; không làm WorldTick tiến |

Reveal hiển thị own history/skills với episode nguồn, known claims và needs đã biên soạn. Claim là nhận định của nhân vật, không World Truth. Không lộ raw IDs/seed/registry/otheractor claims/hidden inventories hoặc developer notes như nguồn tri thức người chơi. UI không sở hữu authoritative state.

Cảnh giữ nền2D `earth-monument-2d-v1` và foundation/font Sprint15; marker vị trí trung tính không được gọi là tạo hình đủ tuổi/giới của tám nhân vật. Marker/camera là presentation, không Travel/collision/route simulation. G13v1 Accepted/Reopenable và thư viện01–18 giữ nguyên; chưa là owner final UI/art approval.

## Kiểm OS-input và actual Player restart

```sh
python3 tools/unity/run_origin_input.py --player /path/to/Player/MythicalOrigin.x86_64
```

Driver X11/xdotool tạo `tests/results/sprint16-input-*`, dùng hai actual Player processes cùng save root. Chỉ gửi phím/chuột vào window của đúng PID sau `inputReady`; dừng nếu mất focus sang app khác. Không dùng gọi callback làm chứng cứ OS input.

Phase1: Menu rỗng → New/hold-beforepublish → Esc cancel không published; New mới → Reveal → Begin; clickWaitOne rồi nativeSpace trên WaitSixty → tick61; save/reload; mở/đóng self-record không kích actions nền; menu/Continue → cùng Reveal/World. F10 chụp và thoát. Phase2 khởi động binary mới thật → Continue, không New → cùng record/tick61/cursor1. Đọc persisted DB độc lập trước/sau restart, giữ cùng file hash/state và số commands. `InputProbe.passed` chỉ báo có marker, không đủ thay chuỗi action/assertions của driver.

Actual drawable resolution phải ghi từ probe; window manager có thể giảm yêu cầu1280×720 xuống1280×683. Đây khác ma trận renderer1280×720/1920×1080 và không được sửa wording để giả cả hai giống nhau. Đây automation kỹ thuật, không human playtest.

QA CLI `-s16InputProbe <new-output>` và `-s16Autoplay <new-output>` mới cho phép `-s16QaSeed <64lowerhex>` hoặc `-s16QaHoldFirstPublication yes`. Barrier là hook kiểm atomic cancellation tại pending DB thật, không fake loading/gameplay. Không bật seed/barrier trong luồng normal UI.

## Resource, determinism và save compatibility

Source authored là `content/definitions/vertical-slice/pack.json`; derived resource là `Assets/Game/Resources/Sprint16/origin-cohort-v1.json`. Không hand-edit derived roster. Để tái xuất bằng C# compiler:

```sh
dotnet run --project engine/HeadlessRunner/Game.Headless.csproj -c Release --no-build -- --origin-export content/definitions/vertical-slice/pack.json
```

Lệnh in JSON ra stdout; chỉ ghi resource từ output sau khi kiểm đúng source/contract. Source canonical hash hiện `60ce05463edcc1c2e4abbeae8f30159d430f57613b902ec6d48bb775db193ec8`, cohort hash `3b4560e57726f3693a483020924f33121fbfa554892ff912029dd50bdf95c8aa`. Vectors độc lập ở `content/fixtures/origin-cohort/selector-vectors.json`; thay contract cần ADR/newprofile và cập nhật consumers, không overwrite goldens để che lỗi.

Origin metadata trong canonical image3 immutable sau genesis; every clone/commit phải giữ principal/household/home/definition IDs/source/cohort/draw. Home là lịch sử origin, không có nghĩa actor phải sống/ở nhà mãi trong domain tương lai. Bounded gateway hiện chỉ chờ nên kiểm current home trong profile hiện tại. Profile rule exact `transfer-resource-v1+scheduler-v1+origin-cohort-v1`; physicalSQLite schema2 giữ. Legacy image1 và repair image2 giữ bytes/profile riêng; không auto-convert hoặc fallback factory.

Published file: `origin-<10digit-sequence>-<32lowerhex-requestId>.db`. `.pending-<guid>.db` và orphan sau crash không là campaign; không xóa foreign files. Writer lock root ngăn concurrent create. New cùng requestId mở lại campaign đã publish, không tạo seed/draw lần nữa. Verify/reopen/flush trước File.Move create-new cùng filesystem; trước publish lỗi/cancel/crash không đổi Continue. Latest published corrupt/incompatible phải fail closed, không tự fallback hoặc reroll; giữ file để điều tra. New explicit vẫn có thể tạo campaign riêng, không overwrite dữ liệu cũ.

Snapshot/recovery giữ Origin trong image3; replay artifactv1 hiện từ chối Origin và OriginRuleVersion cho đến khi có profile mới. Không gọi deterministic genesis/save-reopen hoặc đọc stored images là origin command replay.

## Đóng sprint

Đủ năm gate trong contract, có independent source/artifact review, full headless regression/negative/process controls, docs/exports và CI đúng HEAD + PR→dev mới đóng Sprint16. Ghi Điều kiện đóng Sprint16 còn thiếu riêng với Backlog dự án ngoài sprint. Full generator/viability/worldbinding/knowledge/campaign, repair progression, final owner UI/art/humanplaytest/performance/cross-platform là backlog; các gate16 đã nhận không được đẩy vào danh sách đó.
