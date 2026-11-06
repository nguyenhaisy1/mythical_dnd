# Sprint15 — phản biện độc lập client, UI và bằng chứng Player

04/11/2026 · SPIN-039 · Actor `/root/s15_critic_client`.

## Phạm vi và trạng thái

Đã đọc baseline `.agent`/HANDOFF/APPLICABLE, ADR024, client contract, Sprint15brief, skill `mythical-validation` và hướng dẫn game-design review. Review source Unity presentation/bootstrap/build, Python runner, asset manifest/QC. Actor này chỉ sở hữu report này; không sửa production, không chạy Editor/build/test, không tạo ảnh hoặc thao tác Git.

**Draft: chưa nghiệm thu Player/visual.** Run đầu do root thực thi `tests/results/sprint15-client-puu3o6ny` đã dừng ở compilation; chưa có Player screenshot trong lượt này. Findings source cần sửa và kiểm lại; callback code có checks không thay actual run.

## Findings đã gửi root

| ID | Mức | Quan sát và tác động | Trạng thái |
|---|---|---|---|
| S15-CLIENT-01 | P1 | Preview modal phủ pointer và giới hạn shortcuts trong Update, nhưng panel buttons phía sau vẫn enabled/focusable; Tab focus ring có thể tới Wait/Save/Cancel phía sau overlay. Callbacks không guard ModalOpen nên submit bằng keyboard có thể thực thi action phía sau; current autoplay chỉ kiểm ScenePointer. Vi phạm modal chặn action phía sau. | Open source finding. Cần disable/inert underlying panel và modal focus containment/guard, rồi kiểm Tab/Enter trong modal. Chưa gọi là lỗi đã tái hiện qua OS. |
| S15-CLIENT-02 | Build blocker | Tự đọc configure-editor.log lượt đầu: bảy CS0234 vì Game.Application shadow UnityEngine.Application tại Bootstrap/Autoplay (unityVersion/platform/persistentDataPath/Quit). Compilation exit1; không có Player evidence. | Open tại lúc review; root/worker cần qualify UnityEngine.Application và chạy actual Editor lại. |
| S15-CLIENT-03 | P2 evidence gap | textScaleValid chỉ so property==1.5; chưa đo bounds/Confirm clipping. Capture dự kiến chỉ1280-preview150 và1920-completed125, trong khi contract yêu cầu100/125/150 ở cả hai resolutions. | Cần matrix preview/textscale geometry và visual review; không kết luận layout hỏng khi chưa thấy render. |
| S15-CLIENT-04 | P2 claim | Actor luôn dùng hai idle frames, Render chỉ đổi boat variant; chưa thấy working pose theo ActivityStatus dù một câu contract nói idle/working. | Chỉ claim two-frame idle/boat feedback đến khi có implementation/evidence tương ứng. |
| S15-CLIENT-05 | P2 evidence gap | Report backend ban đầu là literal Mono, runner chỉ yêu cầu passed và capture sizes. | Nên đo actual Mono runtime và assert actual LinuxPlayer/version/backend trong runner; không suy từ label đơn lẻ. |

## Source boundaries đã đọc

Presenter/view/scene/autoplay không nhận raw WorldSnapshot, processor, receipts hoặc journal; bootstrap là trusted composition duy nhất biết Infrastructure/registry/save root. Frame time chỉ dùng chuyển frame/footpoint presentation; WorldTick đi qua session.AdvanceBy. Normal UI không có reset/reroll/path input. QA branches tạo database riêng chỉ khi CLI autoplay, không overwrite world của normal flow.

Read-only safe DTO/known-preview, command retry và SQLite composition còn có critic boundary riêng. Report này không tự mở rộng kết luận đó sang full Knowledge, random-life generator hoặc fullVS07.

## Asset/provenance audit thực tế

Đã tự hash sáu runtime PNG và năm raw sources: tất cả khớp manifest; kích thước PNG đúng declarations; năm prompt files tồn tại. Metadata phân biệt dressed-reference là planning-only; runtime dùng base sạch cộng foreground/boat/actor riêng. QC ghi alpha/edge containment và normalization; không coi JSON QC là chứng minh Unity đã render đúng.

Đã xem bằng image viewer `assets/production/sprint15/qa/layered-preview-front.png`: mảng đất/sông góc cạnh và palette kiềm chế; actor/boat/post riêng có tỷ lệ đọc được, post tạo vùng che một phần nhân vật. Đây là composed preview, **không phải screenshot Unity**. Phần UI/font/focus/cost/Confirm phải xem capture Player sau build.

## Runtime/artifact evidence còn chờ

Root cần cung cấp run actual thành công, build log, Player log, qa.json, screenshots ở1280×720/1920×1080 và before/behind occluder; actor này sẽ tự đọc JSON/counters/hash/dimensions và xem ảnh trước kết luận. OS-input telemetry cần root thực thi với process/window cụ thể; callback automation không là OS input hoặc human playtest.

## Fingerprints source tại draft review

```text
dbc6d283d70ac443a9d436f3c27c3a065659356ae8827f8b1fcedd667841b779  client/UnityProject/Assets/Game/Presentation/RepairClientBootstrap.cs
44bcdde99407357d9cd510367f52d601883b380460a21eb2e79eccfa5d410458  client/UnityProject/Assets/Game/Presentation/RepairClientView.cs
fd6abcc3aba3f73a073213524177a363094ff8356d5c2bc2db8c562f80989ffb  client/UnityProject/Assets/Game/Presentation/RepairSceneView.cs
6df4a50ad20c49282a55f4324867e664ac05f534f716917e18ba8cec6bdfc999  client/UnityProject/Assets/Game/Presentation/RepairAutoplay.cs
76a3e9e56fc49d703e5704802045f3775810062303c30fcd22e999c5efaad075  client/UnityProject/Assets/Game/Editor/RepairClientBuild.cs
c37fbab4dcff6c8c117e746682b3afca84b399125a631a007f8304ae1a42ad65  tools/unity/run_repair_client.py
6df631e69772d94cba8fc9a1e18da3145db5de4b09f886737fde1fb136ef5f1c  assets/production/sprint15/manifest.json
e4e8fc9e813cf67fb5e9149ca3fccef94c1a41895dd9e3b82de1ec702fa87e51  assets/production/sprint15/asset-qc.json
```

Khi source thay đổi sau draft, chỉ reuse kết luận cho phần còn khớp; review lại fix và cập nhật evidence cuối. Không có quyết định final UI/art user approval, production performance/balance, PlayerIL2CPP/cross-platform hoặc human playtest trong report này.

## Review source v2 và audit actual Player lượt đầu — 04/11/2026

Phần draft ở trên giữ findings và trạng thái tại lúc viết. Root/worker đã sửa source, sau đó root thực thi run mới `tests/results/sprint15-client-4_4rkhzh`. Actor critic này không chạy build/Player; đã tự đọc các artifact sau.

- `build-player-editor.log` ghi `S15_BUILD_RESULT Succeeded errors=0 warnings=0`; `player-runtime.log` ghi READY và AUTOPLAY_PASSED True. `evidence.json` exit0/statusPassed, actual qa `unityVersion=6000.6.7f1`, `platform=LinuxPlayer`, `backend=Mono` do runtime probe lấy.
- Tự hash180paths từ sources.json:0mismatch tại lượt audit; canonical manifest fingerprint `cf6246600d67c502ef33d4d9e4dd7456c10a3fafa9ce80947473f5b9b96226ac` khớp evidence.
- Tự đọc tất cả flags:19checks ngoài passed đều true, gồm modalActions/Focus/BackgroundSubmit, workingFeedback và layouts. Frames đúng: initial0/10/0/500; start0/10/2/500; beforeDue59/10/2/500; completed60/8/0/700; cancel0/10/0/500; waited60/10/0/500 (tick/timber/reserved/condition).
- Tự đọc sáu layout samples: hai resolution1280×720/1920×1080 × scale1/1.25/1.5, tất cảfits true. Tự kiểm11PNG headers/dimensions và SHA256. Tự mở ba SQLite files bằng connection read-only: world15/profile/rule/content/seed đúng, ticks world60/cancel0/wait60; đây là files Player tạo, không dựa chỉ qa self-report.
- Đã xem ảnh thật:1280initial,1280preview150,1920preview150,1920completed125 và1920occlusionfront/behind. Cost/time/Confirm tiếng Việt đọc đủ tại hai extreme captures; trước/sau post có thay đổi che khuất thực, chân nhân vật bám cùng mặt đất. Cảnh/frame không được gọi là concept mockup.

### Disposition findings trước

- CLIENT-01: source v2 disable panel subtree, guard direct actions khi modal, restore focus và dùng native Button submit làm sole Return activation. Actual callback regression kiểm background NavigationSubmit, action-null/unchangedDTO và modalFocusContained đều true. **Đóng ở source + programmatic runtime**; OS Tab/Enter vẫn cần telemetry riêng, không nhận callback test là OS input.
- CLIENT-02: **đã đóng** bằng fully-qualified UnityEngine.Application và actual Editor import/Player build thành công. Giữ failed Configure đầu làm lịch sử.
- CLIENT-03: matrix sáu samples + capture thực giải quyết gate modal cost/Confirm. **Đóng cho modal layout**; không suy toàn sidebar đã đúng từ các bounds modal.
- CLIENT-04: source thêm cadence/tilt quanh foot pivot khi observed running và QA check true. Claim phù hợp là presentation activity feedback từ hai idle frames, không là authored full working pose/animation.
- CLIENT-05: actual Mono probe + runner assert version/platform/backend đã có, raw result khớp. **Đóng**.

### Finding mới từ actual visual/log — chưa đạt UI quality gate

| ID | Mức | Evidence quan sát thực | Cần sửa/kiểm lại |
|---|---|---|---|
| S15-CLIENT-06 | P1 trong gate UI1280/1920 | `player-runtime.log` có warning `No Theme Style Sheet set to PanelSettings , UI will not render properly`. 1280initial: Save button xuất hiện vượt đáy panel702 và bị cắt ở mép frame; những control sau nằm ngoài vùng panel. 1920completed125: Cỡ chữ, scale buttons và outcome chồng/cắt dưới panel. Root đồng thời quan sát cùng lỗi. | Assign runtime Theme/ScrollView viewport/scrolling đúng; giữ các action truy cập được, không giải quyết bằng giấu action. Capture lại và thử scroll/focus tới Save/Reload/scale/result. |
| S15-CLIENT-07 | P2 visual capture, gate4 chưa đủ | Tất cả ảnh1920 đã xem có69hàng pixel đen toàn bộ ở trên và36cột đen toàn bộ bên phải; title header bị che, caption có frame chỉ còn một phần.1280initial có0/0. Critic tự kiểm pixels bằng Pillow read-only. DISPLAY đang3840×1080, hai màn1920×1080; nguyên nhân WM/window resize mới là giả thuyết, không khẳng định từ PNG. | Chạy full-frame/window mode phù hợp và capture renderer lại ở1920, không retouch/crop/paint screenshot để làm evidence. |

**Kết luận tại lượt này:** rule effects/save/reload/callback behavior và actual Player build có bằng chứng đạt; runner `Passed` chưa đủ đóng visual/UI gate. Root đã giao xử lý hai findings mới và sẽ cung cấp actual run/captures tiếp theo. Chưa chốt toàn Sprint15/OS input/human playtest.

## Review độc lập tiếp quản — 06/11/2026

Actor `/root/s15_client_final_review` tiếp quản kiểm tra artifact; không nhận review cũ là công việc mình đã thực hiện. Chỉ sửa report này, không chạy Editor/Player hoặc thay production. Đã đọc baseline/contract/skill và tự audit lượt root chạy `tests/results/sprint15-client-21bx2wn3`.

- Hash183source paths hiện tại:0mismatch; fingerprint canonical `a6922dafdfc1d1989f494daa1a79baa894c38000f75a1374c9c3ae893b323174` khớp evidence. 12PNG hashes/kích thước khớp manifest; mỗi ảnh có0hàng đen toàn bộ trên và0cột đen toàn bộ phải.
- Đã trực tiếp xem1280initial/scroll-bottom/preview150,1920preview150/completed125/occlusionfront/behind. CLIENT-06 theme/overflow được sửa: Save/Reload/Retry/scales/outcome truy cập được bằng scroll, panel clip đúng. CLIENT-07 dải đen được sửa ở captures thực; không retouch screenshot. Sáu samples1280×720/1920×1080 ×100/125/150% đều boundsfits và modal cost/Confirm đọc đủ ở hai mức150% đã xem.
- Runtime báo Unity6000.6.7f1/LinuxPlayer/Mono; tất cả21callbackchecks kể cảpassed vàmainPanelScrollValid true. Nhưng không dùng những flags này để bỏ qua finding mới dưới đây.

### S15-CLIENT-08 — P1 evidence/state divergence, đang mở

`capture-1920-completed-125.png` hiện tick60/condition700/timber8 **nhưng reserved2, Đang sửa, hẹn phút120**. Tự mở `saves/world.db` read-only xác nhận revision7 và **hai StartRepair receipts**: lượt đầu tick0 và lượt `client/b39c0f1b27614e02abfb6a7f8a2df8fb` tick60 sau completion. Do đó đây không chỉ là UI cũ: authoritative save đã có một lượt sửa mới, trong khi `qa.completed` còn báo reserved0/completed từ thời điểm trước. Runner vẫn Passed vì chỉ kiểm frame tức thời và SQLite metadata tick/profile, chưa kiểm final repair state/receipt count.

`player-runtime.log` có `S15_INPUT_KEY Return` trong callback autoplay, qa.keyboardEvents=1. Đây bằng chứng OS key lọt vào lượt callback; chưa xác định ai/phát sinh gì gửi key, không gán Space hoặc human action khi không có bằng chứng. Cần ngăn/abort input ngoài luồng khi autoplay, thêm delayed/final state và exact persisted effects/receipt validation rồi chạy mới. Giữ nguyên run này như bằng chứng có divergence; không sửa JSON/ảnh/DB để chuyển thành sạch. OS-input proof riêng vẫn phải chạy bằng Player normal input path.

Tại thời điểm này CLIENT-06/07 đóng trong phạm vi visual đã xem; CLIENT-08 chặn nghiệm thu final Player/visual. Chờ lượt sửa/chạy mới và OS input evidence. Không có final user art approval/human playtest/fullVS07 trong kết luận này.

## Sửa input isolation và lượt freeze đầu — 06/11/2026

Đã đọc lại fix source: chỉ callback autoplay bật `IsolateAutoplayInput`; normal play/InputProbe giữ đường input thật. Root trickle handlers chặn key/native navigation/pointer/click/wheel trong autoplay, legacy Update cũng dừng; synthetic background-submit probe được miễn đúng lúc để vẫn kiểm guard modal thật. Delayed state check sau captures và durable reload, cùng read-only decoder kiểm actual SQLite lots/items/activities/StartRepair receipt count, bổ sung phần runner trước bỏ sót.

Lượt trung gian `sprint15-client-gl6e8m11` không dùng làm final evidence vì source thay trong build; giữ FailedSourcesChanged dù callbacks/DB đã đạt. Lượt freeze mới `sprint15-client-0zqlba64` root thực thi exit0/Passed; critic tự kiểm183hashes không mismatch, canonical fingerprint `42e2cbd9ad47c1a9f3691dca7af4ba146857c9ab35cbf37d2846c35892870435` khớp; log actual build0errors/0warnings, actual Unity6000.6.7f1/LinuxPlayer/Mono. 23requiredcallbackflags đều true, inputReady=false đúng vì không phải InputProbe; keyboardEvents0/intercepted0. Sáu layoutcases đủ và12PNG đúnghash/kích thước,0blackrows/columns. Đã xem lại bảy captures1280initial/scroll/preview150 và1920completed125/preview150/front/behind.

Actual completed screenshot nay khớp state: tick60, timber8, reserved0, condition700, completed. Tự mở cả ba SQLite files read-only và đọc metadata/seed; dùng read-only MDC1decoder của runner sau khi review để đọc entity/command payload: worldrevision6, một StartRepair/completed/unreserved0; cancel một StartRepair/cancelled/unreserved0; wait không StartRepair/activity; privateboat321/privatelot917 giữ nguyên. Không gọi việc dùng decoder cùng code là một serializer oracle độc lập; critic tự xem raw receipts và ảnh để đối chiếu thêm. **CLIENT-08 đóng cho callback/final-persistence acceptance ở lượt source này**; không suy OS interaction đã đạt.

### S15-CLIENT-09 — P1 actual mouse gesture commits without separate confirmation, đang mở

Root thực thi OS-input `tests/results/sprint15-input-5prtp46v` trên PID924885/window23068680, outcome Failed. Critic tự đọc toàn bộ action observations và finaltelemetry: R preview pure, native Tab+Return Back không commit; R+Return start durably; S saved/L loaded; Space với Repair focus tăng đúng1tick; TabTab tới Wait rồi Space tăng đúng1tick nữa; C hủy/reserved0; R/Esc mở/đóng pure. Nhưng clickboat(773,440) tạo scenePicks1 rồi telemetry thành modalfalse/reserved2/running: đã có StartRepair mà chưa có gesture xác nhận riêng.

Tọa độ clickboat nằm ngay trong Confirm bounds tại1280 (x415..865,y419..461); source mở modal từ legacy MouseButtonDown. Điều này phù hợp giả thuyết release của cùng gesture kích hoạt Button mới, chưa gọi là causaltrace đã instrument. Đây là lỗi actual input trong normal path, không sửa bằng chặn autoplay. Cần tách gesture mở preview khỏi activation Confirm, rồi chạy lại actual OS click và kiểm vẫn reserved0/modalopen trước một confirmation riêng. Lượt0z giữ giá trị cho callback/build scope, chưa đủ đóng gate OS input hoặc sprint.

## Final client review sau sửa mouse gesture — 06/11/2026

Actor `/root/s15_client_final_review` đã đọc fix: chỉ đánh dấu world gesture khi press ngoài UI, chờ release và kết thúc native UI dispatch bằng WaitForEndOfFrame rồi mới mở preview; sceneGestureVersion loại gesture cũ khi mở modal bằng đường khác. Pointer bắt đầu trên modal/panel không được chuyển thành scene pick khi modal đóng. Normal keyboard/OS-input vẫn bật; callback autoplay isolation chỉ dùng QA mode. Không thấy P0/P1 mới từ source đã đọc.

Actual root run **`tests/results/sprint15-client-bo3487zv` Passed/exit0** là evidence cuối cho source mousefix, thay lượt0z trong nghiệm thu cuối nhưng giữ toàn bộ lịch sử. Critic tự kiểm183paths0mismatch; canonical source fingerprint `51564bd28a32a630279353015603d1deed1711805414d10ec56d23b6c680fb41`. Runtime target6000.6.7f1/LinuxPlayer/Mono, build0errors/0warnings. Sáu layoutcases đủ,23requiredcallbackflags đều true, keyboardEvents0/intercepted0; inputReady=false là đúng autoplay mode. 12PNG hashes/dimensions khớp; biên trên/phải không còn dải đen. Đã trực tiếp xem lại1280scroll/preview150 và1920completed/front/behind trên source cuối. UI/cost/Confirm đọc được, completed sidebar hiển thị condition700/timber8/reserved0/completed, chân và occluder đổi che khuất đúng. Read-only actual SQLite final state vẫn đúng cả ba nhánh như bảng dưới.

| DB actual Player | Tick | Condition | Timber/reserved | StartRepair receipts / activity |
|---|---:|---:|---|---|
| world.db |60|700|8/0|1 / completed|
| qa-cancel.db |0|500|10/0|1 / cancelled|
| qa-wait.db |60|500|10/0|0 / absent|

Hidden boat321/lot917 không đổi; profile/schema2/rule/content/seed khớp. CLIENT-08 tiếp tục đóng trên source cuối; runner nay tự phát hiện final effects/receipt drift, không chỉ metadata/frame trước captures.

### Actual OS input và đóng CLIENT-09

Root chạy **`tests/results/sprint15-input-yc1_bzfx` Passed/exit0**, PID930005/window25165832; critic không tự chạy Player nhưng tự đọc toàn bộ action observations, log, final telemetry và DB read-only, trực tiếp xem input-initial/input-final PNG. Managed Assembly-CSharp actual SHA256 `a507b552b10a726b191cfdfca33f66c41897b3d6d17aad537b88740aa67090ef` tự hash khớp hai reports. Driver dùng window/PID focus check trước mỗi thao tác, không input vào app khác.

- R mở pure preview; Tab/Return native Back đóng mà reserved0; R/Return native Confirm tạo running/reserved2 durably. S trả saved/durable, L loaded/success.
- Space khi Repair focus tăng tick0→1; TabTab rồi Space khi Wait focus tăng1→2, mỗi lần đúng1, không native submit kép. C trả cancelled/timber10/reserved0/condition500; R/Escape không đổi state.
- Driver giữ mouse-down300ms rồi release tại boat773,440; observation sau release vẫn modalopen/reserved0/cancelled, scenePicks1. Fresh click Confirm640,440 mới tạo running/reserved2; scenePicks vẫn1. R/Escape sau đó không tạo thêm repair. **CLIENT-09 đóng bằng source fix + actual separate-gesture regression**.
- Final telemetry18markers/16keyboardEvents/1scenePick; final DBrevision6/tick2 có đúng5receipts: StartRepair,AdvanceTime,AdvanceTime,CancelActivity,StartRepair. Lượt StartRepair thứ hai này là thao tác Confirm riêng đã ghi, không là lỗi tự start như run5prtp46v.

**Kích thước thật của OS-input lượt này là1280×683**, do drawable window có decoration; không gọi ảnh/input probe này là1280×720. Acceptance layout1280×720/1920×1080 được chứng minh riêng bằng callback renderer matrix của bo3487zv, không tráo loại evidence. Hai ICU fallback warnings vẫn có trong runtime log; chữ Việt trong captures đã xem hiển thị đúng, chưa kiểm emoji/full international segmentation. Không suy warning đó là theme regression.

### Kết luận trong phạm vi client review

Không còn **P0/P1 đã biết** ở source/client/renderer/input đã review; CLIENT-01/02/03/05/06/07/08/09 đóng đúng scope evidence, CLIENT-04 được giới hạn claim thành cadence/tilt từ hai idle frames, không gán full authored working animation. Source + actual build + callback effects/persistence + rendered UI/occlusion + OS keyboard/mouse đủ cho bounded client gate của Sprint15. Full regression/docs/export/CI/Git do root nghiệm thu riêng; report này không tự đóng sprint hoặc xác nhận checks chưa có.

Đây là technical independent review và automated OS interaction; **không phải human playtest, final user art/UI approval, full random life/Knowledge/world binding/VS07, IL2CPP/cross-platform hoặc performance/balance acceptance**. G13/profilev1 và thư viện tham khảo giữ quyền chốt riêng.

Fingerprints source cuối của phần fix/runner đã review:

```text
acaacb9e72d1a961d2cc40e3443d7a5c24eda08e8cff6f2e268d0ea85ecbf0da  client/UnityProject/Assets/Game/Presentation/RepairClientView.cs
5d8ae8ae6290d7c76a7146e5a92dafc2c17309d148cc89ef43d55a7c21efa567  client/UnityProject/Assets/Game/Presentation/RepairAutoplay.cs
03f27b5e21f03767adbe0e0cb00dab1053f2703177e8a344c14f40341f46213a  tools/unity/run_repair_client.py
ac99a620953c0667ab2acf1077e017a5cc9538cff4c67ce2f1a8f477811b633d  tools/unity/run_repair_input.py
```
