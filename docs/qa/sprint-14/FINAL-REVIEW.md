# Sprint14 — phản biện tích hợp cuối

03/11/2026 · SPIN-038 · actor `/root/s14_final_critic`.

## Phạm vi và kết luận hiện tại

Đây là lượt review độc lập mới sau hai báo cáo ban đầu; actor này không nhận công viết các test `BoatRepairCausalityTests` hoặc `BoatRepairIndependentTests` của critic trước. Đã đọc baseline, ADR023, Boat Repair Profile, implementation Domain/Application/scheduler, codec/store/recovery/replay, Host JSON/probe và Python gate. Áp dụng `mythical-validation`; không sửa production, không chạy build/test song song với root, không commit.

Source review tìm thêm một **P1 về binding lịch sử scheduler của automatic cancellation**, đã gửi root trước khi chốt runtime. Root đã sửa shared guard; focused suite sau sửa đạt113/113. Chưa kết luận toàn Sprint14 Passed trước full runtime gate. Các kết quả runtime cuối phải đối chiếu artifact do root thực thi sau source freeze. Headless profile không chứng minh Unity/Player, sprite, world binding, hệ nghề đầy đủ, full VS-03, hiệu năng hoặc human playtest.

## Findings

| ID | Mức | Lỗi cụ thể và hệ quả | Bằng chứng / xử lý |
|---|---|---|---|
| S14-FINAL-01 | P1 | `ValidateRepairHistory` nhánh `ExecuteScheduledRepair` ban đầu kiểm payload `workflow_id` là string và khớp Job fact, nhưng không buộc một `Scheduler.Executions` thực và workflow có cùng ID tồn tại. Codec chỉ đi từ những execution đang có sang receipt/workflow; reverse check bắt buộc execution chỉ áp Completed jobs. Vì thế automatic cancellation có thể giữ receipt/facts/activity/job, bỏ execution và bỏ workflow hoặc chỉnh count về0, rồi vượt kiểm lịch sử. | Root chạy ba case trước sửa: cả3 Failed và cho thấy hai đường restore còn nhận aggregate hỏng. Root bổ sung shared guard: đúng một execution khớp job/command/due/reason, workflow thực cùng ID/target và accepted-pending Advance request có issuer/source/correlation/target/order phù hợp; Running/external cancel không được có execution. Critic đã đọc patch và tự đọc focused TRX113/113 Passed, trong đó cả3case mới Passed. Finding đóng ở source/focused runtime; fullgate ghi dưới. |
| S14-STORAGE-01 | P1 trước đó | Host gọi sai API trả `CommittedWorld` bằng `.Load()`. | Đã đọc bản sửa dùng returned state và mở output SQLite riêng để xác minh; không còn call sai trong source. Cần build/probe cuối để đóng runtime. Finding do critic storage trước phát hiện. |
| S14-STORAGE-02 | P1 trước đó | Activity/job tự khớp nhưng thiếu accepted StartRepair receipt hoặc fact pin phù hợp. | Đã đọc guard shared trong constructor, Publish và codec; kiểm start/terminal receipt/fact, revision/lifecycle, recipe, material, due, cause/correlation. Tám malformed tests của critic trước giữ riêng; kết quả cuối chờ root. |

Đây là lỗi nhất quán causal/reference trong restore, không phải tuyên bố SQLite hash là chữ ký chống người sửa file. Không yêu cầu load phải replay toàn lịch sử; cần các liên kết đã được contract quy định có thật và nhất quán.

## Các ranh giới đã đọc và giữ đúng trong source

- Pure rules không cấp ID, ghi state, gọi Unity/IO/wall-clock hoặc RNG. Application giữ quyền publication; start proposal được kiểm độc lập theo target, item/capability/activity metadata và vật liệu.
- Completion dùng due-tick input chưa publish riêng, revalidate actor/rights/qualification/tool/worksite/boat; cancel release reservation, giữ OnHand. Pin worksite/tool/recipe không tự đổi theo state hiện hành.
- Cancellation từ external command có correlation mới hợp lệ; completion nội bộ vẫn pin correlation từ start. Generic CancelJob không kết thúc repair job để tránh rò hold; CancelActivity được nhận khi Advance đang mở.
- Image2 chỉ xuất hiện khi extension repair explicit, kể cả empty. Null giữ shape image1. Decode canonical roundtrip làm unknown fields không bị bỏ qua âm thầm; tagged job không dùng TransferResource giả.
- SQLite schema2 là version vật lý riêng với image2. Store có rule/content profile rõ; mirrors gồm item/activity/capability; output recovery/migration dùng đúng profile. Recovery đọc committed images, replay thực thi lại Execute/Drain.
- Host probe dùng file SQLite mới, reopen/recovery và hai process; Python kiểm numeric outcomes từ scenario độc lập và negative authoring controls. Việc code có checks không thay bằng chứng các checks đã được chạy.

## Ba ca độc lập bổ sung

`tests/MythicalDnd.Foundation.Tests/BoatRepairWorkflowBindingTests.cs` do actor này viết theo phân công root; production/test cũ giữ nguyên.

1. Xóa cả workflow và execution khỏi automatic cancellation đã commit.
2. Xóa execution nhưng giữ workflow, chỉnh CommittedJobs về0 và cursor null để không chỉ thất bại do count thừa.
3. Giữ execution nhưng xóa workflow sở hữu; codec cũ có một chiều kiểm tra, constructor cũng phải từ chối aggregate này.

Mỗi case kiểm aggregate gốc được cả processor constructor và codec nhận, rồi đòi cả hai đường restore từ chối bản hỏng bằng repair-history diagnostic. Trusted qualification invalidation chỉ là fixture revalidation, không chứng minh có profession mutation command.

## Runtime evidence và fingerprint

Critic không tự chạy build/test. Đã dùng Python stdlib tự parse counters và từng UnitTestResult của artifact root chạy:

| Artifact | Executed / Passed / Failed / skipped | Nhận định |
|---|---:|---|
| `tests/results/sprint14-workflow-before/workflow.trx` |3 /0 /3 /0|Control trước sửa tái hiện finding; không gọi suite này Passed |
| `tests/results/sprint14-final-focused/boat-repair.trx` |113 /113 /0 /0|Cả ba workflow-binding case mới, tám causality case trước và hai independent pin cases đều Passed |

Full-suite/Host evidence đang chạy dưới điều phối root; sẽ kiểm artifact và source manifest trước khi đóng report. Không dùng thông báo Pass thay artifact hoặc nhận focused suite là full legacy regression.

SHA256 source đã review sau fix (không bao gồm report này):

```text
4984f08fccf733d2189ffbb7c7effc01662b05dc1dfe2177a4bb375ad4863989  engine/Domain/Repair/RepairState.cs
049bf969c5741080cee9d313ddb0d55f961e0e07f3e956b9dac77dc2865a785f  engine/Domain/Repair/BoatRepairRules.cs
e985eeacf749e2d3828a08b52c71cdd8dc0fc1b3d9022e872d263aead710ceb1  engine/Domain/Kernel/WorldSnapshot.cs
03faa62da7d753ac70a441a62020c9f4a46b31a73fdab6e53744d109d9217262  engine/Application/CommandPipeline/CommandProcessor.cs
c79c4276af6cf58d27ed4aad5a650ac3a3e110a0fa72b3c1602668aec5380ea7  engine/Application/CommandPipeline/CommandProcessor.Repair.cs
3798333d5108d0cb2500312d3570139bf86045c98ad47385d9edb828a0adcd56  engine/Application/CommandPipeline/CommandProcessor.RepairHistory.cs
be4f0c4f8219504574562cc5e5f3befb00bc27d9bf0e6bb67ae3b473f1fa1d8d  engine/Application/SimulationScheduler/CommandProcessor.Scheduler.cs
22f8989733946a2e689af57afd27e157ceb0216508a4a9cd9c5567ede07196cd  engine/Application/SimulationScheduler/SchedulerState.cs
af09e2c167bd656237291328922963854d3cff0aa935e8080a892725682255ab  engine/Infrastructure/Persistence/CommittedWorldCodec.cs
90d5afd9756beeff2e6342d8a491430879c38b7e2ad5d48595eb1683d3a99f9b  engine/Infrastructure/Persistence/SqliteWorldStore.cs
d24eced225d7b88b381886d6eac8cf35dac56ac995c8aba8dacd47fb543d2c73  engine/Infrastructure/Persistence/SqliteWorldStore.Saves.cs
7a67ec0355a9a9ce0e392d47217d4949862155fd8d9de6267e1ac4c1207257b5  engine/Infrastructure/Repair/RepairContentRegistry.cs
b19c8885378edef9c6d2d992b198b5bfcdfd408f67d620c24050ab6f730e32cc  engine/Infrastructure/Replay/ReplayHarness.cs
21d9c278ca93e3ce5072c7053e255a6795053ef9c938c125cfb207dc77e185e0  engine/HeadlessRunner/BoatRepairProbe.cs
91d77429c10fa94b92113aa33dd63b76b956146ebddcdf66a5d15099ba044f6f  engine/HeadlessRunner/BoatRepairJson.cs
3d6ef5e8fb649ea7d88f443d9fc32ac08fefc50961830348f1816f7ec5867f36  tools/tests/check_boat_repair.py
a6caa3e60f61d254628fc73f59395755fb1c19beaf8517971cff63de9c712466  tests/MythicalDnd.Foundation.Tests/BoatRepairWorkflowBindingTests.cs
```

## Bổ sung nghiệm thu của root sau full gate — 03/11/2026

Actor `/root`, không phải lượt review độc lập tiếp theo. Critic `/root/s14_final_critic` chạm giới hạn sử dụng sau khi ghi source review và tự parse focused113/113; không thực hiện đối chiếu full-run bên dưới. Giữ phần review trước như lịch sử tại thời điểm viết.

Root tự đọc `tests/results/sprint14-2bdf5f7o/evidence.json` và raw TRX:562/562 Passed,113 repair,0failed/skipped; exit0, build0warnings/errors. Shared source fingerprint `sha256:f4eab1e00e4785939da5af0d8b742e99b70a9015c5f300d104f7c4cd2b2c22d4`; source manifest không đổi trong run. Cả13 ca độc lập (2pin+8causality+3workflow) Passed trong full TRX. Hai Host repair processes đạt11checks, reopening/recovery/replay thật, cùng hashes và hai authoring negatives trước tạoDB; legacy gates/goldens đạt. Không Unity hoặc human playtest.

Root đóng disposition của cả ba P1 dựa trên source fixes đã được independent source review, failure-before/regression-after và full evidence thực: S14-FINAL-01 binding đã chặn; S14-STORAGE-01 Host recovery đã build và chạy; S14-STORAGE-02 causal guard đã chạy các malformed tests. Không còn known blocker cho bounded VS-03A headless. Đây là nghiệm thu lớp đầu theo workflow, không gán root kiểm full artifacts thành independent review. [Bằng chứng và giới hạn](../SPRINT-14-EVIDENCE.md).

## Audit độc lập bổ sung sau khi actor tiếp tục — 03/11/2026

Actor `/root/s14_final_critic` đã tiếp tục sau giới hạn sử dụng và gửi kết quả audit mới cho root; root ghi lại kết quả tại đây. Phần trên giữ đúng việc audit full chưa xảy ra tại lúc viết, không nhận audit mới này là việc đã làm trước quota.

Critic tự parse raw full TRX:562 rows/executed/Passed,113 VS-03A,0failed/notExecuted; negative TRX đúng1Failed và log ghi intentional failure, build0warnings/errors. Tự hash170paths từ sources.json có0mismatch, recompute manifest hash `f4eab1e00e4785939da5af0d8b742e99b70a9015c5f300d104f7c4cd2b2c22d4` khớp evidence;17review fingerprints không đổi. Tự đọc hai Host stdout JSON, xác nhận khớp evidence/với nhau, stderr rỗng,11flags true, numeric outcomes đúng,6replayactions, image2/SQLite schema2; reload/recovery/replay negative đều đạt. Hai authoring negatives exit1 đúng diagnostic và không tồn tại outputDB.

Kết luận độc lập cuối: cả ba P1 đã đóng bằng source và focused/full runtime evidence; không có P0/P1 mở trong VS-03A headless đã review. Critic không chạy build/test ở lượt audit này, không edit report đang export; không kết luận Unity/Player/fullVS03/performance/human playtest. Root đồng bộ báo cáo sau message của critic và regenerate exports để giữ freshness.
