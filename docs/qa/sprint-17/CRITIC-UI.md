# Sprint17 — phản biện độc lập client/UI và bằng chứng Player

10/11/2026 · SPIN-041 · Actor `/root/s17_ui_review` · skill `mythical-validation`.

## Trạng thái và phạm vi

**Đã đối chiếu actual Player/render/native OS; không còn P0/P1 UI được biết. Full final regression còn chờ.** Đã đọc AGENTS, `.agent`, HANDOFF, APPLICABLE-BASELINE, PERSONAL-WEAVING-PROFILE, Sprint17 brief, TEST-EXECUTION-CONTRACT, TEST-MATRIX và hướng dẫn game-design review. Actor này chỉ sửa report này, không sửa production/tests/runners/assets/Editor hoặc thao tác Git. Root chịu tích hợp, chạy actual gates và ghi WORKLOG.

Lượt source đầu chưa thấy P0/P1; actual native input sau đó đã phát hiện UI05 bên dưới. Không dùng callback/render Passed để thay native gate hoặc đóng sprint.

## Quan sát nguồn

- Bốn file Unity mới dùng namespace Sprint17 và scene/build list riêng; background/font/theme reuse từ Sprint15. Bootstrap là trusted composition, load cohort16/parameters17 và tạo gateway dưới root `personal-weaving-v1`. QA seed/publication barrier chỉ dùng khi có autoplay/input probe; không có bộ chọn nhân vật trên UI.
- View và Autoplay chỉ nhận Application ports/DTO. `WeavingWorkView`/`WeavingPreview` immutable và không mang seed/raw IDs/world/registry/private revision/SQL/path. Material/output/tick đọc từ session; presenter không tự cộng vật liệu, sản phẩm hoặc thời gian.
- World cho thấy sợi cá nhân, phần giữ chỗ, dây đã đan, kỹ năng cần40 và thời điểm hoàn tất. Thông báo sáu xuất thân chưa được hỗ trợ không nói họ không có nghề hoặc yêu cầu reroll. UI nêu starter kit là vật liệu cá nhân của bản thử mới, không quyền dùng kho hộ; thao tác dùng tay, chưa mô phỏng dụng cụ/progression.
- Preview chỉ gọi session.Preview; modal disable worldPanel, focus mặc định nút quay lại. Confirm native có guard chờ press sau frame mở modal; callback QA `ConfirmPreview()` là đường kiểm riêng, không phải bằng chứng native gesture. Save/Reload/Advance/Cancel chặn khi modal mở. Escape ưu tiên đóng modal/hủy generation. Tab/Enter/Space dựa native Button, Update chỉ quan sát telemetry.
- Header có GeometryChanged reserve content dưới header; Reveal dùng ScrollView/focus-scroll, footer cố định và kiểm bounds. Preview có scroll thân và footer riêng. Cần nhìn actual sáu layout để kết luận chữ và control không bị cắt.
- Session preview pin private revision/tick bằng token opaque; Start lookup original intent trước stale-token check. Start/Cancel/Advance retry giữ original envelope qua receipts; UI giữ request/token khi session/storage error. Reload/Continue resume accepted Advance bằng DrainAdvance, không tạo target mới. Đây review nguồn, không thay fault/restart tests.
- Gateway root riêng, exact new profile, pending→verify→flush→create-new publish; không scan save16 để enrich hoặc lấy scene repair15 cho origin17.

## Điểm cần đối chiếu

| ID | Loại | Quan sát / yêu cầu | Trạng thái |
|---|---|---|---|
|S17-UI-01|Evidence gate3/4|Autoplay gọi `ConfirmPreview()` trực tiếp và `ShowMenu→Continue` trong cùng process. Phải có native OS preview và Confirm bằng hai gesture, actual Player process mới Continue cùng saveRunning root, rồi đọc final SQLite độc lập.|Đã đối chiếu g714cvsc hai PID/SaveRunning/Continue/complete và SQLite cuối; resolved trong bounded scope.|
|S17-UI-02|Evidence layout|`PreviewLayoutFits` kiểm card/footer/Confirm; không tự chứng minh toàn bộ chữ trong ScrollView đọc được ở150%. Xem actual1280×720/1920×1080×100/125/150%, bao gồm Preview, Reveal top/bottom và World.|Đã đọc final matrix6cases/seed và nhìn samples Preview/World150; root bổ sung Worldmatrix thực. Không còn clip/control blocker tại samples đã xem.|
|S17-UI-03|Evidence input|`FinishInput.passed` chỉ là đã nhận ít nhất một input marker. Root driver phải kiểm chuỗi action/focus/result/tick/material và DB, không dùng flag telemetry thay gate.|Đã đọc48nativeactions, current work snapshots và readonly SQLite, không chỉ dựa flag telemetry.|
|S17-UI-04|Gap diễn đạt contract/factory|`WeavingCampaignGateway.LoadVerified` reject current actor location khác origin home, input/output owner/location khác genesis. Contract cần phân biệt bounded factory admissibility với pin immutable metadata.|Đã làm rõ trong contract §Persistence: factory17 chỉ own-at-home/hai observed lots, không claim Travel/rights/Health end-to-end; Domain/history vẫn phải kiểm revalidation riêng. Không là lỗi UI xác định.|
|S17-UI-05|P1 native craft input, confirmed behavior|`sprint17-input-uurtz1tv` nhận pointer telemetry/focus Confirm nhưng modal vẫn mở, work idle8/0/0; driver timeout separate native Confirm. Người dùng chưa bắt đầu được craft qua đường pointer được kiểm.|Resolved: nguồn TrickleDown/NavigationSubmit guard đã review; g714cvsc pointer Confirm và Return Confirm đều tạo đúng Running, final new Assembly/render/legacy/sourceguard khớp.|

## Evidence cần bàn giao trước kết luận cuối

1. Actual Unity6000.6.7f1/LinuxMono configure/scene/build/run, measured versions/backend, pre/post source manifests.
2. Lâm43 và Bà Nhu61 craft từ starter8: Running8/2/0 tại0, trước due8/2/0 tại29, completed6/0/1 tại30; cancel không consume/output; nonweaver unsupported giữ8/0/0. Origin record và draw1 giữ nguyên.
3. Raw QA và actual rendered captures sáu resolution/textscale cases, own record/preview/modal/World/running/output/unsupported; độc lập nhìn ảnh và phân biệt bounds với khả năng đọc.
4. Native OS actions có Tab/Enter/Space/Escape/pointer, preview và Confirm riêng, modal nền inert, wait/cancel/saveRunning và hai Player PID/restart thực cùng save; SQLite cuối và receipts/jobs/holds giữ đúng, không double input hoặc double effect.
5. Actual scene15 repair và scene16 origin Player regression; full runtime/negative/process final source guard. CI/PR/dev do root chứng minh, không suy từ local artifacts.

Không claim human playtest/final owner art/UI approval, full generator/Character Viability/fullVS03, complete cast art, performance hoặc crossplatform. G13/profilev1/01–18 giữ baseline.

## Fingerprints lúc source review

```text
fd38c2ad34e02c2601a81e8b90795363df8828736feae9f93e1608139d9196f8  WeavingClientView.cs
eb29f410fc672a40df93481cfbef74ef44b8fd4993d54b55068ddcb715d76ab0  WeavingClientBootstrap.cs
503c8a0767ee8d651f2b27d3e5ef7296ca95e5ac20e29911a429d467b050be1e  WeavingAutoplay.cs
7aab3018e1ec81d84fe85d1ce02774186f45b6b38b243841d2831d079f84447d  WeavingClientBuild.cs
0dd70139bfebd2006b28294ffdae0668582a35a217d5437f7d8d2958cd5eeccc  WeavingClientContracts.cs
20ad464c4a457beb0d36fe0a05b0c19b1f2ba5115759e80013aab9fca1b8c03e  WeavingCampaignSession.cs
```

Các file có cùng tên ở đường dẫn Presentation/Editor/Application Weaving đã nêu trong scope task; source thay đổi sau review phải đối chiếu lại phần bị ảnh hưởng.

## Lượt actual đầu để chẩn đoán — chưa đạt

Tự đọc `tests/results/sprint17-client-vhwrph7p/evidence.json`, `autoplay-lam/qa.json` và log: actual Unity6000.6.7f1/LinuxPlayer/Mono build được, Lâm Reveal sáu layout có fits/bottomVisible; đến Preview loop đổi từ1920 fullscreen về1280 windowed thì thực tế1280×683, QA dừng `InvalidOperationException: RESOLUTION` trước Preview capture hoặc Start. Lượt này Failed/exit1, không chứng minh craft/restart/native input và không dùng làm final acceptance. Root cũng đã báo nguồn/tools đang sửa nên cần fresh final run sau freeze, không bỏ source guard.

Đã tự nhìn bốn ảnh raw của lượt này: `capture-1280-reveal-150-top.png`, `capture-1280-reveal-150-bottom.png`, `capture-1920-world-initial.png`, `capture-1920-self-record.png`. Header/card/footer tách rõ; Reveal150 có scroll chủ ý, cuối own-record đọc được và Begin vẫn hiện; World có quantities8/0/0, skill43/cần40, starter-kit/dùng tay/marker note; record modal làm nền tối/inert theo nguồn. Chưa thấy lỗi chữ hoặc chồng control tại các ảnh đã xem. Không suy Preview150 hoặc các origin khác đã đạt từ bốn ảnh này.

## Actual Player/render trước bản sửa native

Root sửa thứ tự kiểm layout để hoàn thành Reveal/World/Preview tại1280 trước khi sang1920, không quay fullscreen về windowed trong cùng lượt; không crop ảnh hoặc bỏ kiểm dimensions. Reviewer đã đọc patch, hooks World chỉ Focus/ScrollTo/bounds, không gọi action hoặc đổi world. Reviewer còn chỉ ra thiếu matrix World mới; root bổ sung sáu cases World top/bottom/focus, thay vì suy từ Reveal/Preview. Lượt trung gian `bbsmed0g` có callbacks/DB/legacy đạt nhưng overall Failed do SourcesChanged; giữ làm lịch sử, không dùng thay lượt final.

Lượt final `tests/results/sprint17-client-_rrf0hsi/evidence.json` tự đọc có status Passed/exit0, actual Unity6000.6.7f1/LinuxPlayer/Mono, build0errors/0warnings. Reviewer tự đối chiếu230paths trong `sources.json` với file hiện hành: không có mismatch. Source fingerprint `0a89c25a3f12027e54f3f833ac2c4e29cb1cf03f2503d7e4707c383d05e629ba`; managed Assembly `e6586694d017eff8e09c4bff69302f462ff8c8e70b088bd54b9b4a9bffce69cd`.

- QA ba origins: Lâm42PNG/Nhu42PNG/Kiên32PNG; mỗi người đủ sáu Reveal và sáu World cases1280×720/1920×1080×100/125/150%, all bounds/bottom/focus true. Qualified thêm sáu Preview captures. Modal/pure/Continue/reload/input-isolation flags đều true; đây callback/render QA, chưa là native OS.
- Tự đọc SQLite read-only qua independent MDC1 decoder: Lâm tick91/fiber6/reserved0/cord1/skill43; Nhu91/6/0/1/61; Kiên61/8/0/0/0. Hai qualified có2Start/1ExecuteScheduledWeaving/1Cancel, activity[Completed,Cancelled]; Kiên không có activity hoặc craft command. Image4/hash đúng, một RNG context draw1, actor controls own và lot owners own; không household grant.
- Tự nhìn ảnh final: Lâm1280World150 top/bottom,1280Preview150,1920Completed; Nhu1280World150 top,1920Running; Kiên1280World150 top/bottom. Bảng mới có số lượng/status/skill/due đọc được; phần dưới cuộn tới notes/scales, control không chồng chữ. Preview150 đọc đủ2sợi/30phút/1dây, dùng tay/cancel và hai nút. Nonweaver text đúng bounded unsupported, không ép reroll. Đây sampling visual có nêu cụ thể; matrix sáu cases còn được đối chiếu geometry/QA, không nhận đã nhìn từng116ảnh.
- Legacy actual scene16:19captures, image3 Origin-only/tick61/draw1; actual scene15:12captures/3DB repair image2, complete700/timber8, cancel500/timber10, wait500/timber10. Reviewer tự đọc DB extension keys để xác nhận không thêm Weaving/Origin vào repair hoặc Weaving vào16; nhìn ảnh16World-waited và15Preview1280/150, không thấy regression hiển thị trong samples. Ba builds có cùng managed Assembly hash trên.

Renderer/layout và legacy Player evidence đã được đối chiếu độc lập. Chưa dùng kết quả này thay actual native two-gesture/restart, full suite/process final hoặc Git/CI gates.

## Native input phát hiện lỗi cần sửa

Tự đọc `tests/results/sprint17-input-uurtz1tv/evidence.json` và `phase-1/telemetry/qa.json`: Failed/exit1 `Timeout: separate native Confirm`; đúng actual6000.6.7f1/LinuxPlayer/Mono và Assembly `_rrf0hsi`. Focus là `ConfirmWeaving`, `previewOpen=true`, current work vẫn idle8/0/0. Vì vậy callback direct Confirm đã đạt không chứng minh native Confirm. Lỗi UI05 là blocker cho native gate đến khi sửa và tự đối chiếu rerun.

Giả thuyết của root: Button Clickable xử lý PointerDown trước callback bubble cấp guard nên confirmPress chưa được bật; đây inference từ event ordering, chưa có trace đủ để khẳng định tuyệt đối nguyên nhân. Hướng sửa đang dự kiến dùng TrickleDown cho PointerDown và NavigationSubmit capture trên chính nút Confirm, vẫn giữ frame mở và hai gesture. Source View thay đổi cần new build/Assembly và final render/regression; không gọi `_rrf0hsi` là final source acceptance sau sửa.

## Bản sửa native và final UI evidence

Đã tự đọc patch View: PointerDown dùng TrickleDown, NavigationSubmit capture chỉ đúng target Confirm sau frame mở, KeyDown guard giữ; callback action vẫn cần confirmPress rồi reset trước gọi session. Focus mặc định Back, không có global submit hoặc đường ghi world mới. Không kết luận nguyên nhân nội bộ Clickable tuyệt đối từ patch; rerun chứng minh hành vi sửa được.

Final renderer `tests/results/sprint17-client-8f9zo2ld/evidence.json`: Passed/exit0, actual Unity6000.6.7f1/LinuxMono, ba origins42/42/32PNG và sáu Reveal/World cases mỗi người; legacy16/15 actual Player19/12PNG/3repairDB đạt. Reviewer tự kiểm lại source230paths không mismatch; fingerprint `2369663f9e869b89d7f5c69ef6b9069064ffc071d95b4e497acfc15d330d70e3`; Assembly `474694cb8f9646ad0ede7a182f8469ab9c18fa4c996e71ad5b261b2f81406d18`. Tự parse final QA/DB mới: Lâm91/6/0/1/43, Nhu91/6/0/1/61, Kiên61/8/0/0/0 như expected; hash image hợp lệ. Legacy cùng Assembly, image2/3 giữ profile riêng.42ảnh Lâm byte-identical với lượt `_rrf0hsi` đã nhìn; đã nhìn lại NhuWorld1280/150top và KiênWorld1280/150top/bottom trong lượt mới. Phần native patch không làm đổi layout tại samples đã xem.

Final native `tests/results/sprint17-input-g714cvsc/evidence.json`: Passed/exit0, cùng Assembly trên. Đã tự đọc35+13=48actions và snapshots; actual PID2544008 rồi2544229, cùng save root, mỗi process exit0. Phase1 actual1280×683 (WM windowed), phase2 actual1280×720; không gọi phase1 là1280×720. Matrix renderer riêng ở trên vẫn strict720/1080.

- Phase1 New/Generating→Escape cancel giữ chưa publish; New/Reveal/Begin ra Lâm; pointer mở Preview giữ idle8/0/0, Escape về; mở lại rồi pointer Confirm action18 tạo Running8/2/0tick0. Return Cancel action20 trả cancelled8/0/0; Preview rồi Return Confirm action24 tạo Running lần hai. WaitOne→tick1, SaveRunning durable; record modal đóng bằng Space giữ8/2/0tick1.
- Phase2 là process mới Continue→Reveal vẫn cùng own-record fingerprint/Running8/2/0tick1. Begin→Space tại WaitSixty đưa tick61/completed6/0/1. Save/Reload/menu/Continue/Begin giữ assignment/tick/material, không double-input effect.
- Tự đọc SQLite cuối read-only, image hash đúng, đúng2Start/1Cancel/2Advance/1ExecuteScheduledWeaving/1CloseAdvance; WeavingCompleted và JobCompleted cùng tick30, không lệch sang61 hoặc lặp. Origin/RNG draw1. Final DB SHA256 `90abf6cc9bb94ad440d46a0e85974d380ae100635a2ce2acb9dbd8be2ab2f268`; canonical image SHA256 `fecc878e123490d4a911bc5d3dacdeae96a9266619d30c7d6d34c2709dc7b9b9`.
- Tự nhìn hai `phase-*/telemetry/input-final.png`: phase1 đang Running/scroll sau own-record, phase2 completed6/0/1/tick61 và controls hiện đúng. Không sửa/crop ảnh.

UI05 đã được tái kiểm bằng actual native pointer và keyboard, không còn blocker UI đã biết trong bounded proof. Đây automation của reviewer/driver, không human playtest. Full final runtime/process/source guard/docs/exports/CI/PRdev vẫn phải đủ riêng trước CLOSE; report này không tự đóng sprint.

## Kết luận độc lập trong ownership UI

**Technical reviewed: đạt phạm vi source/safe projection, actual renderer/layout, native input và actual SaveRunning→Player restart→Continue của client17, cùng actual regression scene15/16; không còn P0/P1 UI được biết.** UI01–03 đã có evidence, UI04 được làm rõ bounded contract, UI05 đã sửa và tái kiểm thực. Không chuyển gate UI còn thiếu sang backlog. Full final regression/process/docs/exports/exactHEADCI/PRdev là phần root và critic core nghiệm thu riêng, chưa được kết luận từ report UI này.

Final key source SHA256 (nằm trong230paths đã đối chiếu):

```text
6232fe58591727c433e7ed6ec8ec9ebc0affab5983da3e1437c3afbf16dbe0b8  WeavingClientView.cs
9bf9cdd8a2c45bd5b11b1bb2603b16f2d9b671589a4252affc4113e6dfd27ea0  WeavingAutoplay.cs
fc66cb2295458acccbf8c5c381592c7d11c9c3c89df5184a17e48ea37f62879d  run_weaving_client.py
758f005132891ba9a49b5dc8f490be9fa78f6f939b02d2512ab90a32e826ef1d  run_weaving_input.py
```
