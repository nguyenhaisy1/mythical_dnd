# Sprint14 — Bằng chứng luật sửa thuyền headless

## Tổng hợp dễ hiểu

Công việc sửa thuyền đã có trạng thái và hậu quả thật trong engine: bắt đầu giữ chỗ hai đơn vị gỗ; sau 60 phút trong game mới tiêu thụ gỗ và tăng tình trạng thuyền. Hủy công việc chỉ bỏ giữ chỗ, không sinh thêm gỗ. Lưu khi đang làm, mở lại, gửi lặp lệnh và chạy lại lịch sử đều có kiểm tra riêng.

Phạm vi là **VS-03A bounded headless**, theo [ADR023](../adr/023-bounded-boat-repair-profile.md) và [contract](../contracts/BOAT-REPAIR-PROFILE.md); chưa phải bản chơi có hình ảnh. Sprint13 đã chốt `earth-monument-2d-v1`; G13 Accepted/Reopenable và thư viện01–18 được giữ nguyên. Full VS02/VS03, client Unity, nhân vật ngẫu nhiên, projection theo hiểu biết của nhân vật và nghề nghiệp đầy đủ vẫn có cổng riêng.

Ngày 03/11/2026 · SPIN-038. **Trạng thái: nghiệm thu headless VS-03A sau full gate và review; Git/CI tích hợp ghi riêng.** Không lấy sự tồn tại của file code/test làm nghiệm thu.

## Phân công và review thực tế

| Actor | Công việc và bằng chứng |
|---|---|
| `/root/s14_repair_domain` | Immutable repair state, pure rules, WorldSnapshot extension và rule tests. |
| `/root/s14_scope_audit` | Audit rồi triển khai Application/pipeline/scheduler, guards độc lập và adversarial tests. Root sở hữu CommandTests. |
| `/root/s14_storage_audit` | Codec image2, strict registry, SQLite profile/mirrors/save/recovery, ReplayHarness và persistence/replay tests. Worker đã chạy 28/28 mới và 139/139 subset; tổng139 có tám ca repair replay, không phải139 ca legacy. |
| `/root/s14_critic_commands` | Review source độc lập Domain/command/scheduler và hai ca pin worksite/tool. Không tự chạy test. [Báo cáo](sprint-14/CRITIC-COMMANDS.md). |
| `/root/s14_critic_storage` | Review storage/replay/Host; phát hiện hai P1, viết tám ca causal-history và tự đọc TRX do root/worker chạy. [Báo cáo](sprint-14/CRITIC-PERSISTENCE.md). |
| `/root/s14_final_critic` | Review độc lập cuối, phát hiện thiếu reverse binding workflow/execution, viết ba ca regression và tự đối chiếu raw evidence. [Kết luận cuối](sprint-14/FINAL-REVIEW.md). |
| `/root/s14_final_docs` | Đồng bộ catalog, persistence/replay runbooks, registers, backlog/matrix, integration notes và export inputs. Không thực thi runtime/build/export. |
| `/root` | Điều phối, Host/JSON/probe/process gate, fixtures, command tests, shared history validator và fixes; integration/checks/docs/evidence/exports/Git, nghiệm thu lớp đầu trong scope được giao. |

Actors chạy theo đợt, tối đa bốn kể cả root. Hai worker ban đầu bị quota rồi tiếp tục; actor cũ không còn live sau lần chuyển context, root giao reviewer/docs mới. Không gán self-review là review độc lập hoặc automated tests là human playtest.

## Hành vi và compatibility được kiểm

- Recipe kỹ thuật `boat-repair-v1`: 2 timber, 60 WorldTicks (mỗi tick một phút), +200 condition, cap1000; không draw RNG. Đây là tham số prototype, chưa balance.
- Start/complete/cancel có identity, activity, job, hold, receipt và facts cùng atomic publication. Auth/stale/mismatch/retry/failure không được tạo hiệu ứng lặp. Public `CompleteRepair` bị chặn; internal wire `ExecuteScheduledRepair(job_id, workflow_id)` chỉ do Drain tạo. `CancelJob` không hủy ngang repair để rò hold.
- Completion revalidate death, qualification, quyền dùng, vật liệu, boat/tool/worksite pin; dữ liệu fixture thay đổi có kiểm soát để thử revalidation, không nhận đã hiện thực Health/Travel/profession mutation commands.
- Restore kiểm accepted cause, versioned start/terminal facts, pins, lifecycle, execution và workflow Advance thực sự; malformed aggregate không được nhận chỉ vì activity/job có vẻ khớp.
- Extension Repair explicit chọn committed image2, kể cả rỗng. Legacy Repair null giữ image1/bytes/goldens. Physical SQLite schema vẫn2; recipe/rule/content profile phải khớp, không tự nâng save cũ. Replay artifact version1 giữ format cũ, truyền explicit repair recipe.
- SQLite file thật close/reopen, failure trước/sau commit, running snapshot/recovery, mirrors/compatibility refusal; command replay chạy lại Execute/Advance/Drain từ initial state, không gọi đọc committed images là replay.

## Lượt kiểm tra và sửa lỗi

Giữ raw failures để thấy lỗi trước/sau, không overwrite golden hoặc làm nhẹ production admission:

| Artifact trong `tests/results/` | Kết quả thực và ý nghĩa |
|---|---|
| `sprint14-integration/boat-repair-integration.trx` | 110 executed,109 Passed,1 Failed: test đặt issuer chết trước Advance nên không có workflow. Tám ca causal-history độc lập đã Passed. |
| `sprint14-revalidation-fix/revalidation.trx` | 9 Failed do assertion đầu của root ghi Accepted thay AcceptedPending. Đây là lỗi test setup/assertion, không phải lý do nới luật production. |
| `sprint14-revalidation-fix2/revalidation.trx` | 9/9 Passed sau khi admit workflow trước trusted invalidation và dùng AcceptedPending đúng contract. |
| `sprint14-workflow-before/workflow.trx` | Ba ca mới đều Failed trước fix, chứng minh thiếu reverse workflow/execution binding. Hai malformed images từng được nhận; trường hợp còn lại codec đã chặn riêng nhưng processor constructor chưa chặn. |
| `sprint14-final-focused/boat-repair.trx` | 113/113 Passed,0 failed/skipped, Release build0 warnings/errors; bao gồm cả13 ca độc lập của ba critics. |
| `sprint14-probe-preflight/boat-repair.json` | Hai process probe SQLite/reopen/recovery/replay cùng JSON/hash; hai authoring negatives được từ chối trước tạoDB. Lượt này trước siết workflow cuối, không thay full gate. |

Hai P1 của critic storage: sửa Host sử dụng đúng returned CommittedWorld của RecoverToNewFile và kiểm lại outputstore.Load; thêm shared accepted-receipt/fact causal history. P1 của critic cuối: bind internal terminal với execution chính xác và accepted Advance workflow, không cho running/external cancel có execution. Ghi nhận test failures và fix trong WORKLOG; kết luận cuối dựa revision full run bên dưới.

## Full runtime gate

Lệnh thực tế `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`, **exit0**. Raw artifact `tests/results/sprint14-2bdf5f7o/evidence.json`, `sources.json`, `runtime.trx` và các log/JSON cùng thư mục.

- **562 discovered/executed,562 Passed,0 failed/skipped**: 113 ca VS-03A mới và449 regression. Release build0 warnings/errors; locked restore đạt. .NET SDK10.0.401/Linux x64, không Unity backend mới.
- Negative-control riêng cố ý fail đúng1test, exit1; runner xác nhận trạng thái mong đợi. Không cộng negative-control failure vào kết quả suite562.
- Source fingerprint `sha256:f4eab1e00e4785939da5af0d8b742e99b70a9015c5f300d104f7c4cd2b2c22d4`; manifest không đổi trong run. Runtime sources/fixtures/parameters/CI được đóng băng; tài liệu cập nhật sau run không đổi runtime fingerprint.
- Hai process repair trả cùng JSON/hash và tất cả11checks true: start10gỗ/hold2/due60; trước hạn condition500; hoàn tất8gỗ/hold0/condition700; hủy10gỗ/hold0/condition500. Recovery/reopen/retry/profile refusal và replay6actions đạt. Hai malformed authoring negatives exit1 trước tạoDB; sai checkpoint replay được phát hiện tại action0.
- Repair content hash `5933871275b5e8725095c968c8cdd7be1804f5270cbc00e0f6e98566bbd77067`, recipe hash `5e43c42c88cdd874f32cc82c86674245871b71e6644ffc6c800c643d7fd5611d`. Final image hash `ed1fd1b734d1dc30e0268bb4c96a92a33d1db4b41aa836a87aec684662848b14`; cancelled image hash `67e018c9c3a4e22ed37bff1aa1c18dd2488eecbc411d69d81e766ab51b0ba48b`.
- Gates cũ đạt: transfer/scheduler demo; chín SIGKILL transaction/snapshot/migration cases; legacy replay hai process/15actions và hash cũ `f89945aa…`, artifact `bc82e042…`; kernel content `e45fa906…`; diagnostics16processes/14negativecases; settlement21processes/17negativecases; event eligibility20processes/17negativecases/23scenarios. SIGKILL cases thuộc kernel profile cũ, không gọi là repair-specific power-loss proof.
- Conformance-preparation chỉ host/tooling, `unity_status=NotRun`; không suy portable build là lượt Unity repair verification.

Critic cuối đã đọc source fix và tự parse113 focused tests trước khi chạm giới hạn sử dụng. Sau đó **root tự đối chiếu** full TRX/JSON và source fingerprints; không gán việc này cho critic hoặc independent full-run audit. Ba P1 đều có source fix, reproductions/regressions và actual Host/full-suite evidence; không còn finding đã biết chặn scope headless này.

**Audit bổ sung sau khi critic tiếp tục:** `/root/s14_final_critic` đã tự đối chiếu raw full TRX562/562 (113repair), negative control, build log,170source fingerprints và17review fingerprints; hai Host stdout cùng11checks/arithmetic/hash và hai malformed authoring negatives trước tạoDB đều đúng. Critic xác nhận không còn P0/P1 mở trong scope. Đây là lượt audit độc lập mới sau quota, được ghi riêng trong final review; không thay lịch sử root đối chiếu trước đó. Không có runtime source thay đổi sau full run.

## Các phạm vi chưa nghiệm thu

Không có lượt Unity/Player mới, layered background/sprite animation/client input, player knowledge projection, full settlement/world binding/random life generator, đủ ba nghề, gameplay balance/performance hoặc human playtest. Unity6000.6.7f1 Editor Mono evidence Sprint10 chỉ áp kernel profile cũ, không tự mở rộng sang repair image2. Full VS02/VS03 vẫnpartial; không mở Sprint15/main/tag/release.

Runbook: [Boat Repair](../runbooks/BOAT-REPAIR.md). Git/CI và kiểm tra docs/exports cuối ghi tiếp ở đây và append [WORKLOG](../../WORKLOG.md); raw results là local/CI artifacts ignored, không commit caches hoặc DB.

## Kiểm tra tài liệu và ranh giới

Đã chạy exit0: `build_docs.py --check` (36 generated files), `check_handoff.py` (29 modules+appendix,6156source sections,52requirements,28tasks,942authored links tại lượt kiểm), `validate_reference.py` (bốn invalid controls), `check_contract_vectors.py` (Python reference, không phải C#), `check_architecture.py`, `tools/mcp/check_client.py` (4 offline controls), `git diff --check`. Module18/26/28/29 và Master được regenerate từ integration notes; archive, old golden fixtures và thư viện01–18 không đổi.

Final report gồm source review độc lập và root disposition đã ghi giới hạn quota; không nhận self-review là independent artifact audit. Exports HTML/DOCX cần regenerate từ source cuối và freshness check trước commit; kết quả thực ghi WORKLOG cùng Git/CI, không tạo ZIP hoặc release. G13 source checkpoint vẫn phải reachable sau integration, không yêu cầu engine tree bất biến khi Sprint14 được giao phát triển gameplay.
