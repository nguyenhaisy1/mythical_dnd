# Sprint16 — Review độc lập tiếp nối và nghiệm thu artifacts

09/11/2026 · SPIN-040 · Actor `/root/s16_final_review`.

## Phạm vi và kết luận hiện tại

Reviewer tiếp nối sau hai báo cáo source/focused ngày06/11; đã đọc AGENTS, `.agent`, HANDOFF, APPLICABLE-BASELINE, ADR025, ORIGIN-COHORT-PROFILE và skill mythical-validation cùng hướng dẫn game-design review. Chỉ sở hữu báo cáo này, không sửa production/test/runner, không thực thi Editor/Player hoặc thao tác Git. Root và worker chạy kiểm chứng; reviewer tự đọc source, dữ liệu, logs, ảnh và DB để đối chiếu.

**Technical reviewed: không còn P0/P1 đã biết trong source và final runtime/render/OS/process evidence đã kiểm.** Lượt cuối ngày09/11 được đối chiếu ở các phần cuối báo cáo; các ghi chú “còn chờ” ở phần ban đầu ghi trạng thái lúc review, không phủ quyết kết quả cuối. Hai lượt Unity đầu vẫn là failure artifacts. Kết luận chỉ áp dụng bounded VS02C/partialVS07 và revision/artifacts được ghi dưới đây; không full generator, Character Viability/TEST001, fullVS02/VS07 hoặc human playtest. Root còn phải chốt tài liệu/exports/CI/Git theo workflow trước CLOSE Sprint16; báo cáo này không tự xác nhận chúng.

## Kết quả review ban đầu

| Mục | Quan sát độc lập | Kết luận |
|---|---|---|
| Lượt `sprint16-client-4g2elcmq` | Editor Configure lỗi CS1061 `IResolvedStyle.overflow`. Source hiện dùng `item.style.overflow.value`; đây không là runtime pass. | Giữ failure; lượt sau mới xác nhận compile. |
| Lượt `sprint16-client-j0jw0ab7` | Editor import/build và hai Player process đã chạy, nhưng runner exit1 tại expected display name seed1. `actor.vac-nhu@1` trong derived resource có tên hiển thị **Bà Nhu**, tuổi77; QA initial/current đúng Bà Nhu, tick0→61, record fingerprint không đổi. | Lỗi literal kỳ vọng của runner, chưa thấy selector drift từ artifacts này. Root sửa expected name, phải chạy final run mới; không sửa dữ liệu/golden để làm test đạt. |
| Source/session/privacy | DTO self-only, immutable copies; bootstrap giữ registry/seed/store; View chỉ safe ports. Skills có episode provenance, claims có nguồn nhận định bản thân và disclaimer. No editable origin/reroll trong Reveal. | Không thấy P0/P1 từ các boundary đã đọc. Đây source review, không thay runtime. |
| Gateway/SQLite | Pending create-new, writer lock, verify/flush trước move, strict catalog order/duplicate refusal; retry request đã publish không xin entropy lại. Store pin toàn bộ canonical Origin trước writer. Old profile refusal và origin replay-v1 refusal explicit. | Phù hợp contract từ source; actual crash/final process gates còn chờ. |
| Ảnh seed1 1280×720/150% top và bottom ở j0jw0ab7 | Header/card tách nhau; nút Begin/Menu và scope note nhìn thấy; biography dài được clip trong scroll viewport và đoạn cuối đọc được sau scroll. Bà Nhu/77tuổi/nhà Vạc Tòng hiện rõ. | Quan sát ảnh thực tế không thấy overlap chặn thao tác ở hai frame đã xem; final run cần đối chiếu lại source fingerprint. |
| Ảnh seed0 1920×1080 World initial ở j0jw0ab7 | Tên Vạc Lâm, phút0, neutral marker “Bạn”, các nút chờ/lưu/mở lại/nhật ký, dòng giới hạn chức năng. | Không mô tả marker là sprite đủ8nhân vật hoặc scene đầy đủ NPC. Đây technical preview, không owner art approval. |

Ba ảnh đã trực tiếp xem: `autoplay-one/capture-1280-reveal-150-top.png`, `autoplay-one/capture-1280-reveal-150-bottom.png`, `autoplay-zero/capture-1920-world-initial.png` trong thư mục j0jw0ab7. Chưa coi toàn bộ capture set đã được quan sát chỉ vì manifest liệt kê chúng.

## Gate còn phải đọc trước kết luận cuối

1. Final actual Unity6000.6.7f1/LinuxMono hai seed, source manifests trước/sau build/run, managed assembly và actual SQLite image3/cursor/tick.
2. Actual OS native keyboard/mouse, cancel pending creation và hai process Player riêng dùng cùng save root; record/cursor/tick61 giữ, modal inert. Callback autoplay và host reopen không thay Player restart.
3. Actual repair15 Player regression/image2/ba SQLite final states trên portable core mới.
4. Full runtime TRX với zero failed/skipped, negative control thực, SIGKILL process publication gates, source freeze cuối; docs/exports và CI/Git do root chốt riêng.

Backlog ngoài sprint: full life generation/viability, world/NPC/economy binding, nghề repair cho cohort, owner art/UI approval, IL2CPP/crossplatform/performance/balance/human playtest. Không dùng backlog này để giấu gate Sprint16 còn thiếu.

## Đối chiếu process gates và source tiếp nối

Đã tự đọc `tools/tests/check_origin_cohort.py` và `tests/results/sprint16-origin-process-first.json` cùng raw logs trong `tests/results/origin-process-q7b6k78n`. Report ghi21calls; ba child processes bị SIGKILL thật (`exit_code=-9`) tại `after_initialize`, `before_publish`, `after_publish`, sau ready-file barrier. Hai case trước publish kiểm byte hashes của campaign cũ và orphan pending không đổi qua Continue/retry; retry tạo campaign thứ hai, Continue chuyển đúng actor mới. Case sau publish mở được campaign mới tick0/cursor1. Đây process crash, không là mô phỏng mất điện hoặc power-loss durability.

Hai positive process seed0 khác đường dẫn và publication request giữ cùng initial/final canonical hashes; seed1 khác actor/world/hash. Negative controls từ chối seed/request sai và latest corrupt, không fallback tới campaign cũ, không đổi published bytes. Export C# khớp derived resource8records. Final nested process report trong full runtime vẫn cần đọc để pin nguồn cuối.

Đã đối chiếu cả11 SHA trong CRITIC-CORE.md với source hiện tại, tất cả khớp, gồm independent boundary tests. Review focused trước vẫn có giá trị cho phần nguồn không đổi. Số92focused không được cộng toàn bộ vào593test cũ: root xác định81cases mới và11legacy trong focused filter; full count dự kiến674, phải lấy số thực từ TRX cuối.

Patch `tools/checks/check_repair_client.py` chỉ cho phép chính xác `RepairClientBootstrap.cs` và `OriginClientBootstrap.cs` làm trusted composition roots. Các file khác trong Presentation vẫn bị kiểm forbidden dependency; không nới thành pattern `*Bootstrap`. Origin View/Autoplay còn được check riêng bằng origin checker. Không thấy gate privacy bị bỏ để đạt kiểm tĩnh.

## Lượt actual Unity mới — `sprint16-client-8fpgghix`

Reviewer tự so206paths trong `sources.json` với SHA256 hiện tại:0changed. Managed `Assembly-CSharp.dll` SHA256 `30f5810c127951530fa98e466b278764e6c4ff01e6916550e8d8ad16071d35f8`. Tại lượt kiểm này hai origin Player và host reopen đã xong; legacy15 đang chạy nên chưa gọi toàn runner Passed.

| Seed QA | Actual self record | World | Tickframes | DB |
|---|---|---|---|---|
|`0`×64|Vạc Lâm,22tuổi, actor.vac-lam@1|`206f8ff78714777f`|0→61→61→61|image3,SQLite2,cursor1,2commands|
|`1`×64|Bà Nhu,77tuổi, actor.vac-nhu@1|`c9dd2b91ea1d91c0`|0→61→61→61|image3,SQLite2,cursor1,2commands|

Cả hai QA có actual version6000.6.7f1/LinuxPlayer/Mono, passed=true/error rỗng,0keyboard/0pointer events trong callback autoplay. Reviewer đọc actual SQLite readonly: world/tick/rule/cohort hash đúng, canonical image SHA khớp stored hash, một actor/không lots/không repair. Hai record fingerprints lần lượt `03d13971ae1654752595ab7f2226bf6955e8721ad05bba3ef21ce27c843dc747` và `ba83fd7fd98644e63ad6548c6e5f51878f5cec9aa22806c2a94d43574ad3972f`, không đổi giữa initial/wait/Continue/current.

Mỗi seed có19PNG và6layout cases đúng1280×720/1920×1080 ×100/125/150%; geometry card nằm dưới header17–21px, fits và bottomVisible đều true. Reviewer trực tiếp xem thêm bốn fresh captures: seed0 `capture-1280-reveal-150-top.png`; seed1 `capture-1920-reveal-150-bottom.png`, `capture-1920-self-record.png`; seed0 `capture-1280-cancelled.png`. Header/card/footer không giao nhau tại các frame này, biography cuộn có nội dung cuối/disclaimer, modal nổi rõ và backdrop làm mờ nút nền, cancelled menu hiển thị đã hủy và Continue bị disable. Không kết luận đã trực tiếp xem toàn38ảnh; completeness/geometry được đối chiếu bằng metadata riêng.

Lượt8fpgghix đã hoàn tất: overall `Passed/exit0`, source fingerprint `54b10aaf2ebc6f84f9e455a5e558e2f82004faecb28ca0368fdd504d8d69f6ba`. Editor logs của origin và repair15 đều ghi BuildResult Succeeded/0errors/0warnings; log dịch vụ Editor có curl certificate error, không gọi nó là gameplay failure hoặc giấu nó bằng claim mọi dòng log không có chữ error.

## Actual repair15 regression trên core mới

Reviewer tự đọc legacy15 QA,12capture entries/sixlayout cases,3actual SQLite files và hai ảnh `capture-1280-preview-150.png`, `capture-1920-completed-125.png`. Cả23flags bắt buộc true, actual6000.6.7f1/LinuxPlayer/Mono, không keyboard/scene-pick input ngoài autoplay. Assembly SHA giống originPlayer. Dòng nhân vật cố định, chi phí2gỗ/60phút/+200/cap1000 và kết quả hoàn tất700/8 hiện đọc được ở hai ảnh trực tiếp xem.

| DB | Tick | Condition/timber/reserved | Start receipts/activity | Image |
|---|---:|---|---|---|
|world.db|60|700/8/0|1/completed|2/noOrigin|
|qa-cancel.db|0|500/10/0|1/cancelled|2/noOrigin|
|qa-wait.db|60|500/10/0|0/none|2/noOrigin|

Reviewer đọc lại canonical repair items: hidden fixture item condition321 giữ, exactrepair rule/content profile giữ. Root runner còn kiểm hidden entities/receipt counts riêng; không lấy ảnh UI làm bằng chứng duy nhất về SQLite.

## Actual OS input và restart — `sprint16-input-22lns4xz`

Reviewer tự parse evidence/actions/telemetry, xem `phase-1/telemetry/input-initial.png`, `phase-2/telemetry/input-final.png`, và mở final DB readonly. Overall Passed/exit0; phase1 PID2264372 và phase2 PID2264524 là hai actual Player process khác nhau, đều exit0; window handle được hệ thống tái dùng không phải bằng chứng cùng process. Cả hai actual drawable1280×720, không dùng lại số683từSprint15. Driver PID-bound kiểm focus trước input; key/click qua xdotool/X11, không gọi action callbacks trực tiếp.

Phase1 có18actions: native Return bắt đầu generation, Escape hủy pending trước publish, New→Reveal→Begin, mouse WaitOne tick1, native Space trên WaitSixty tick61, pointer Save durable/saved, Return Reload loaded, Return mở self-record và Space đóng không tăng tick, BackMenu→Continue→Reveal61→pointerBegin. Phase2 có2actions: Continue bằng Return và Begin bằng pointer từ actual cold-start Menu. Record fingerprint/tên/tuổi/nhà/location giữ cùng nhau ở hai process. Driver kiểm barrier/catalog không published trước cancel; telemetry.passed đơn lẻ không thay toàn bộ assertions này.

Readonly actual SQLite sau cả hai process có đúng1published campaign,0pending, world206f8ff78714777f/tick61/image3/SQLite2, actor.vac-lam@1 và Origin/RNG draw1; đúng2AdvanceTime payload target_tick1/61. Canonical image SHA khớp meta; final file SHA `db4e15c016cabf6cc347d75e84495b96ae5d9c13f6e8cb7e25b6e681c6108a05` khớp driver. Assembly SHA `30f5810c127951530fa98e466b278764e6c4ff01e6916550e8d8ad16071d35f8` khớp renderer run. Driver so toàn final_database trước/sau actual process2, gồm file SHA, nên Continue/Begin không viết lại hoặc bốc origin mới. Đây automation, không human playtest.

## Final headless regression và kết luận kỹ thuật

Reviewer tự parse `tests/results/sprint16-1rngpl9l/runtime.trx`:674results/674executed/674Passed/0Failed/0notExecuted, không chỉ đọc câu Passed trong summary. Phân task Sprint16:30S16-01,48S16-02,3S16-04 =81cases mới; 593legacy cases giữ, trong đó113repair/25safe-session15/6factory15. `negative.trx` có đúng1executed/1Failed/0Passed, tên TEST_027_RunnerNegativeControl, exit1 theo intentional negative control. Đây không là lỗi suite chính.

Full runner `evidence.json` exit0, không error. Các process suites transaction/snapshot/save/replay/content/diagnostics/conformance-preparation/settlement/eligibility/boat-repair hoàn tất; conformance-preparation ghi đúng host-only/UnityNotRun ở suite đó, không lấy nó thay actual Unity artifacts riêng đã nêu. Final `origin-cohort.json` có Passed,21calls/3SIGKILL−9 cùng checks orphan/published preservation, deterministic same-seed/different-seed và negative controls đúng như lượt đầu. Không gọi Origin save/recovery là origin command replay; legacy replay v1 còn explicit refusal.

Reviewer tính lại SHA256 từ286source paths và so manifest:0changed; fingerprint tự tính khớp `6b3de7c07749c127d0c26262b9d27497644e6aad699060ae42b1c8fd7ae03665` của final full run. Các core hashes trước review độc lập vẫn khớp. Unity manifest206paths cũng khớp ở lượt đối chiếu sau haiPlayer; source đổi tiếp phải xét lại phần evidence bị ảnh hưởng.

Kết luận: **không có P0/P1 còn mở trong phạm vi đã review**. S16-CORE-01 đã có independent failure→fix→final pass; UI01/02 có patch/source và actual visible layout checks, UI03 có actual second-process restart, UI04 có driver assertions/action sequence/DB riêng thay telemetry flag đơn lẻ. Có đủ evidence kỹ thuật cho luồng cohort New/Reveal/World/self-record/time/save/Continue, atomic publication và regression repair15 trong platform/profile đã nêu. Không đánh đồng với final owner approval, full game readiness, power-loss durability hoặc crossplatform/backend khác. Báo cáo này không xác nhận commit/push/PR/CI/dev hoặc quyền mởSprint17/main/tag/release.
