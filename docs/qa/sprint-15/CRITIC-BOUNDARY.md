# Sprint15 — phản biện độc lập ranh giới dữ liệu và session

04/11/2026 · SPIN-039 · actor `/root/s15_critic_boundary`.

## Phạm vi và kết luận hiện tại

Đã đọc baseline `.agent`/HANDOFF/APPLICABLE, ADR024, `REPAIR-CLIENT-PROFILE`, brief Sprint15 và test execution contract; áp dụng `mythical-validation`. Review độc lập source Application projection/session, Infrastructure fixed-fixture composition và tests tương ứng. Critic không triển khai production, không chạy build/test song song với root, không thực hiện Git hoặc sửa WORKLOG.

Không phát hiện P0/P1 trong bounded fixed-worksite profile đã đọc. Một P2 về gate quan sát recipe phụ thuộc text đã được root sửa, critic đọc patch và tự đối chiếu raw TRX trước/sau. Không còn finding mở trong phạm vi review này. Kết luận bao gồm source review và audit focused runtime artifacts; chưa tự nghiệm thu Player, UI/input/visual hoặc full Sprint15. Các cổng đó cần actual evidence của revision cuối.

## Findings

| ID | Mức | Lỗi và tác động | Trạng thái |
|---|---|---|---|
| S15-BOUNDARY-01 | P2, Closed | `PreviewRepair` ban đầu xác định `recipeKnown` bằng `scope.Grants(Recipe) && view.LocationLabel == scope.LocationLabel`. Scope hợp lệ có nhãn `Chưa rõ`; khi actor ở ngoài worksite, Project trả sentinel cùng text nên preview vẫn đưa cost/time/gain/cap dù observation source là unknown. Factory hiện dùng nhãn Bến Vạc nên ca mặc định không bị ảnh hưởng, nhưng API gate không được phụ thuộc bản dịch/nhãn. | Root đổi predicate sang `state.World.GetActor(scope.Principal.ActorId).Location.Equals(scope.Worksite)` cùng grant. Critic đọc đúng patch và tự parse hai ca độc lập trước sửa1fail/1pass; focused sau sửa31/31Passed gồm cả hai ca. |

Đây là kiểm soát direct-observation theo field trong profile kỹ thuật, không mở rộng scope sang hệ knowledge/perception đầy đủ hoặc bảo mật chống caller cố tình dùng reflection.

## Các đường source đã đối chiếu

| Ràng buộc | Quan sát |
|---|---|
| Port an toàn | `IRepairClientSession` không nhận actor/filepath tùy ý, không trả processor/world/receipt/envelope/journal/jobs. DTO chỉ primitive, nullable và immutable properties; recipe/client/art profile được phân biệt. |
| Explicit grants | Scope là trusted composition; ownership không tự thành knowledge. Projection chỉ đưa field được grant tại worksite vào DTO; Available cần cả OnHand và Reserved. Actor/private boat/lot khác không được enumerate trong DTO/picking data. |
| Activity scope | Own activity lọc actor và bound boat trước chọn activity mới nhất; không adopt activity thuyền khác. |
| Read/preview | Không gọi Execute hoặc ghi authoritative state/RNG. Preview token/counter chỉ là private session state; revision/tick pin không được trả ra UI. Gate recipe sau sửa dùng worksite identity thay text. |
| Stale và idempotency | Confirm dùng private pinned revision/tick; stale không tự reprice. Request ID không nhận slash/đường dẫn. Intent cache giữ original envelope; ID đổi intent trả mismatch. Fresh Retry chỉ adopt receipt đúng principal/source/correlation/version và bound payload/activity, không đọc arbitrary actor commands. |
| Advance/cancel | Advance gửi WorldTick command rồi Drain trusted scheduler; retry pending workflow cùng ID không tạo lượt tua thời gian mới. Cancel bind own running activity và lý do hợp lệ; không có numeric effects do UI cấp. |
| Error boundary | Session map rejection sang vocabulary hữu hạn và catch exceptions thành session/storage error, không trả exception.Message/SQL/private payload. |
| Durability | Factory inject processor có actual SQLite store, exact repair rule/content registry và durableCommands=true. SaveSnapshot callback gọi store.CreateSnapshot; Reload load+validate+restore thành công rồi mới đổi processor. Unit callbacks không được dùng thay real file acceptance. |
| Fixture binding | Chỉ initialize khi file chưa tồn tại; hiện có corrupt/incompatible file không bị reset. Load kiểm world15/fixture seed/profile, stable refs/worksite/tool capability trước bind actor; không tự adopt arbitrary compatible world. |

Không nhận việc gate public methods dùng trusted constructors/callbacks là bằng chứng mọi gameplay domain hoặc mọi actor knowledge đã có. Presentation/Unity assembly chỉ nên được inject interface này; audit UI source/runtime là cổng review khác.

## Hai ca độc lập bổ sung

File `tests/MythicalDnd.Foundation.Tests/RepairClientBoundaryTests.cs` do critic viết theo quyền root cấp, không sửa test hay source của worker:

1. Dựng actor ngoài worksite, grant All và nhãn worksite `Chưa rõ`: recipe fields phải Unknown, preview không được confirm và canonical state/receipt không thay đổi.
2. Cùng nhãn `Chưa rõ` nhưng actor thực sự ở worksite: direct observation hợp lệ vẫn cung cấp cost2/duration60. Ca này ngăn cách sửa bằng cấm nhãn hoặc luôn giấu recipe thay vì kiểm identity.

Chỉ trusted fixture đổi vị trí để kiểm gate; không nhận đã hiện thực Travel command. Critic không tự chạy hai ca này; root thực thi, critic tự parse các artifact trước/sau ghi bên dưới.

## Artifact đã tự đọc

Critic dùng Python stdlib parse `tests/results/sprint15-focused-initial/client-session.trx` do root thực thi:29 rows/total/executed/Passed,0failed/notExecuted. Trong đó23 S15-01 và6 S15-04, không có result row thất bại. Đây là lượt trước hai regression mới và trước fix gate recipe; không dùng nó thay source/runtime evidence cuối.

| Artifact do root thực thi, critic tự đọc | Total/executed/Passed/Failed/notExecuted | Kết luận |
|---|---:|---|
| `tests/results/sprint15-boundary-before/boundary.trx` |2/2/1/1/0|Ca actor ngoài worksite thất bại đúng ở cost2 thay Unknown; same-worksite control đã Passed trước sửa |
| `tests/results/sprint15-focused-fixed/client-session.trx` |31/31/31/0/0|25 session/boundary và6 real SQLite fixture tests; cả hai independent regression cases Passed |

Root báo Release build0warnings/errors ở lượt focused cuối; critic tự parse raw TRX, không nhận đã tự thực thi build. Full suite và actual Player/build/render/input còn phải đối chiếu ở gates tương ứng. Không có Unity/Player, performance, human playtest, final art/UI approval, fullVS03/VS07 hoặc random-life acceptance trong report này.

## Source fingerprint đã review sau sửa

SHA256 tại lúc chốt review; nếu các file này đổi, phần kết luận liên quan cần đối chiếu lại:

```text
fe80f503cf0e35434a1850b3e6046486437dee2bcc47f4e6f7a2e4391015be8a  engine/Application/Projection/RepairClientContracts.cs
4489ad68a41524517048743df89b40a2bf1e8b667c98abd85a435f3cf175cdb9  engine/Application/Projection/RepairObservationScope.cs
030079b518175fb73ec632f726767e4925b5152d73ad21314906f91a9cadbe9d  engine/Application/Projection/RepairClientSession.cs
b31f7f29fd88898fc97be32889b7bfa5a347a47198b94268765a57da48025b03  engine/Infrastructure/Repair/RepairDemoFactory.cs
c79ddfe144b5411deb6d236ea17dc7220689fdc91fde4a1a8ca757cd3c4cbbdb  tests/MythicalDnd.Foundation.Tests/RepairClientSessionTests.cs
82a78dfff67c8830b93b23bbfbcf2dc0307a1880617b76793a267cf8cf556683  tests/MythicalDnd.Foundation.Tests/RepairDemoPersistenceTests.cs
c4c5b56cd859fa2c59c9f6ecf91b5ad646595330d26114a018af03ff60a2bd48  tests/MythicalDnd.Foundation.Tests/RepairClientBoundaryTests.cs
```

## Tiếp nhận review độc lập cuối — 06/11/2026

Actor mới `/root/s15_final_boundary_review` tiếp nhận audit cuối sau ngắt phiên; không nhận là actor đã viết hai ca độc lập hoặc thực thi các lượt test trước. Actor này chỉ append report, đọc source và artifact; root vẫn thực thi Editor/Player/full regression. Đã đọc lại `.agent`, HANDOFF, APPLICABLE, ADR024, client contract, test execution contract và skill `mythical-validation`.

Tự hash lại bảy file trong bảng fingerprint bên trên: **7/7 khớp nguyên bytes**. Đã đọc lại grants/Unknown, activity scope, stale/private pin, bound receipt retry, error vocabulary và actual SQLite fixed-fixture binding. Không có P0/P1 mới trong ranh giới App/Infrastructure đã đọc. Đây vẫn là bounded direct-worksite profile; không mở rộng thành full Knowledge/Perception, random life hoặc fullVS07.

### Kiểm checker final SQLite sau finding client

Client review phát hiện lượt `sprint15-client-21bx2wn3` có thêm StartRepair sau sample `qa.completed`. Actor boundary đã độc lập đọc bản hardened `tools/unity/run_repair_client.py` SHA256 `8759098fcfe3c8a6a014a35a25dc94f7574f8844b08ce9d0d415cf79d9893397`: runner giờ đối chiếu actual final boat condition, lot on-hand/reserved, số activity/status/hold và số StartRepair receipt của cả ba database, ngoài metadata/seed/schema/rule. Runner cũng yêu cầu explicit modal/input/working/scroll/stable-completion flags, sáu layout cases chính xác và đủ mười hai tên capture.

Đã tự thực thi **read-only artifact checks**, không chạy lại C# hoặc Player:

- Independent Python MDC1 reader đọc đúng **4/4 codec golden vectors** hiện có, không sửa golden.
- `inspect_final_database` đối với world.db cũ `21bx2wn3` **từ chối đúng** với `Final SQLite repair/material/command state differs: world.db`; không lấy cờ runner Passed cũ làm acceptance mới.
- Hai database cùng lượt `qa-cancel.db` và `qa-wait.db` được checker nhận đúng: cancel500condition/10timber/0reserved/1start/1cancelled activity; wait500/10/0/0start/0activity. Các giá trị hidden boat321/private lot917 giữ nguyên trong phạm vi checker đối chiếu.

### Full regression trung gian — chưa phải evidence source cuối

Tự parse raw TRX do root chạy tại `tests/results/sprint15-r365u194`: runtime593total/executed/Passed,0failed/notExecuted và593result rows khớp counters. S15-01 gồm25cases, S15-04 gồm6cases, VS03A gồm113cases, tất cả Passed. Negative control có đúng1total/executed/Failed (`TEST_027_RunnerNegativeControl`). Lượt này chạy trong lúc source UI/tooling tiếp tục sửa; vì vậy **không dùng thay full regression có manifest cuối khớp**. Root sẽ cung cấp run sau freeze để review tiếp; actual Player/input/Git/CI và đóng Sprint15 chưa được kết luận từ đoạn audit này.

### Final Player artifact — source freeze đạt

Actor `/root/s15_final_boundary_review` tự audit lượt root thực thi `tests/results/sprint15-client-0zqlba64`: overall `Passed`, exit0. Runner cuối SHA256 `587a4a1ae05bd2238ab3fe06975441695a51e2f33837270132dd5671ca06a6cc` bổ sung guard so source trước/sau Player build và sau Player run; không chấp nhận binary build giữa lúc source đang đổi. Tự hash **183/183 source paths khớp**, canonical fingerprint `42e2cbd9ad47c1a9f3691dca7af4ba146857c9ab35cbf37d2846c35892870435`. Actual build log ghi `S15_BUILD_RESULT Succeeded errors=0 warnings=0`; runtime log ghi autoplay Passed, không có S15_INPUT_KEY. Actual version/platform/backend là6000.6.7f1/LinuxPlayer/Mono.

Tự đọc23required checks đều true, gồm delayed completion stability và autoplay input isolation/no delivered external input; `keyboardEvents=0`. `inputReady=false` ở callback autoplay là đúng: flag readiness dành cho đường OS InputProbe riêng, không được gọi mọi boolean trong JSON đều true. Sáu layout cases đủ hai resolution × ba text scales và12capture entries hiện hữu; visual/OS input chi tiết do critic client kiểm riêng.

Đã tự mở cả ba SQLite file read-only, hash khớp evidence, decode entity/commands rồi gọi checker đối chiếu:

| File Player tạo | Condition / timber / reserved | StartRepair / activity cuối |
|---|---|---|
| world.db |700 /8 /0|Đúng1StartRepair,1activity Completed(status1),0hold |
| qa-cancel.db |500 /10 /0|Đúng1StartRepair,1activity Cancelled(status2),0hold |
| qa-wait.db |500 /10 /0|0StartRepair,0activity |

world.db có đúng các command types: StartRepair, hai AdvanceTime, ExecuteScheduledRepair và CloseAdvance; **không có lượt sửa thứ hai sau capture**. Các hidden fixture fields được checker kiểm (private boat condition321/private lot on-hand917) giữ nguyên. Final `qa.completed` vẫn tick60/condition700/timber8/reserved0/completed; khớp actual persisted state sau toàn bộ captures. Đây là proof đủ cho finding delayed second-repair trong phạm vi persistence checker; không nhận autoplay isolation là bằng chứng thao tác OS bình thường đã kiểm hoặc human playtest.

Lượt trung gian `sprint15-client-gl6e8m11` có actual callback/final-DB checks đạt nhưng overall Failed vì source thay đổi trong run; giữ nguyên lỗi này và không lấy nó thay final0zqlba64. Full headless manifest cuối còn chờ lượt `sprint15-9s6r3iz2`.

## Kết luận audit cuối — 06/11/2026

Actor `/root/s15_final_boundary_review` đã đối chiếu revision cuối sau khi root sửa lỗi OS pointer: lượt callback `0zqlba64` ở mục trước là artifact đúng tại thời điểm đó, **không thay acceptance OS input của bản sau sửa**. Lượt full regression `sprint15-9s6r3iz2` có593/593tests đạt nhưng overall exit1/Sources changed during run; giữ làm evidence trung gian. Các kết luận cuối dưới đây dùng những lượt mới với source đã freeze.

### Actual Player và OS input sau sửa

`tests/results/sprint15-client-bo3487zv/evidence.json` Passed/exit0. Tự hash183source paths,0mismatch; fingerprint canonical `51564bd28a32a630279353015603d1deed1711805414d10ec56d23b6c680fb41` khớp report. Tự hash built `MythicalRepair_Data/Managed/Assembly-CSharp.dll`: `a507b552b10a726b191cfdfca33f66c41897b3d6d17aad537b88740aa67090ef`; cùng assembly với actual OS-input run `sprint15-input-yc1_bzfx`. Runner cuối SHA256 `03f27b5e21f03767adbe0e0cb00dab1053f2703177e8a344c14f40341f46213a` giữ before/after build/run guards và thêm managed-assembly fingerprint, không chỉ hash Linux launcher.

Tự mở lại cả ba database callback read-only: world700condition/8timber/0reserved với đúng1StartRepair/1Completed; cancel500/10/0 với1StartRepair/1Cancelled; wait500/10/0,0StartRepair/0activity. Final delayed completion stable true, keyboardEvents0, không có lượt sửa ngoài kịch bản sau capture. Các metadata/source/runtime flags tiếp tục khớp profile đã giao.

Tự parse18OS actions do root thực thi trong `sprint15-input-yc1_bzfx`: preview R không giữ vật liệu; Tab/Return quay lại không commit; Return xác nhận riêng giữ2gỗ; S/L trả saved/loaded; hai Space lần lượt tăng tick0→1 và1→2 khi focus khác nhau; cancel trả hold0. Pointer thuyền mở modal vẫn hold0, rồi **một gesture Confirm riêng** mới giữ2gỗ. Đây là actual OS automation, không human playtest. Tự mở final OS world.db read-only: tick2/condition500/on-hand10/reserved2; đúng hai StartRepair, hai AdvanceTime, một CancelActivity và hai activities [Cancelled,Running]. Trạng thái Running cuối là chủ ý sau Confirm thứ hai trong kịch bản, không nhầm với delayed extra-repair cũ.

### Full headless regression có manifest cuối khớp

Lượt root thực thi `tests/results/sprint15-l08y6ql5/evidence.json` **exit0**. Actor reviewer tự parse JSON/TRX và hash lại **256/256 source paths**,0mismatch; canonical source revision `sha256:60746b3f12cf8d18502517266afe451ae0870f9809cd18dd724ce14634b0c04d`. SHA256 evidence JSON tại lúc audit `96d1d51bbfed942ff2c95d475c4317054b0291e1ba4d2292f61ce8869ca81fcb`.

- Actual Release build0warnings/errors; runtime593total/executed/Passed,0failed/notExecuted;593result rows khớp counters. Có25S15-01,6S15-04 và113VS03A, tất cả Passed.
- Negative control đúng1test được discover/execute và Failed, process exit1; không nhầm failure chủ ý với lỗi suite.
- Ba SQLite transaction crash và sáu snapshot/migration SIGKILL cases đều signal-9/Passed; scope Linux process-crash, không power-loss hoặc platform replacement.
- Replay hai process khớp stateHash, mỗi process15actions/exit0; negative first-divergence index0/exit1 được phát hiện đúng.
- Repair probe hai process exit0, report giống nhau; hai malformed-content negatives exit1 và không tạo database. Giữ profile/image2/physicalSQLite2 và các old goldens.
- Các artifact content, diagnostics, host conformance preparation, settlement và event eligibility đều được runner thực thi/kiểm theo scope riêng. Content46processes, diagnostics16/14negative, settlement21/17negative, eligibility20/17negative; negative payload được từ chối là expected behavior. Host conformance preparation ghi Unity NotRun vì chính process gate đó chỉ host/tooling; actual Unity/Player nằm ở lượt riêng bên trên, không gộp hai loại evidence.

**Không còn P0/P1 hoặc finding mở trong phạm vi độc lập App/session/Infrastructure và final-persistence checker đã review.** Seven App/Infrastructure/test fingerprints từ review đầu vẫn không đổi. Full final headless gate và actual persisted effects đủ bằng chứng trong phạm vi bounded repair-client; visual/modal/pointer source/finding disposition thuộc report client riêng. Quyết định đóng Sprint15 còn phải do root tổng hợp đủ docs/exports/CI/Git; report này không tự merge dev, mở Sprint16, chốt final user art/UI, full Knowledge/random-life/fullVS07 hoặc human playtest.
