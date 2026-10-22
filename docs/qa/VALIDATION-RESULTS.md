# Bằng chứng kiểm tra lần bàn giao 2026-10-13

Phạm vi: khôi phục tài liệu, cấu trúc project và tooling. Các runtime tests trong TEST-MATRIX chưa được thực hiện vì gameplay/kernel chưa triển khai.

| Kiểm tra | Kết quả quan sát |
|---|---|
| .NET Release build | SDK 10.0.401; 5 project build thành công; 0 warning, 0 error |
| Reference fixture validator | Cấu trúc schema subset và references hợp lệ; 4 negative controls bị từ chối |
| Assembly architecture | Dependency graph và targets khớp portable core / net10 host |
| Source preservation | 85 source message hashes; 31 khối dài khôi phục thành 29 module + phụ lục; 1.436.697 ký tự normalized; 6.156 mục có anchor |
| Generated docs / handoff check | 36 file sinh lại khớp nguồn; 52 requirement, 28 task và dependency DAG hợp lệ; 291 authored links tồn tại |
| Host status smoke | `--status` trả scaffold, `simulationImplemented: false`; lệnh simulate chưa hỗ trợ phải exit 2 |

Các lệnh tái kiểm tra, export còn có output machine-readable/log ở `artifacts/` khi chạy. Không dùng “Passed” ở đây để diễn đạt Unity, formula balance, deterministic simulation, save/recovery hay full-game playtest đã pass.

## Bổ sung tài liệu 2026-10-14 — evidence thực tế

Phạm vi lần này là specs, reference data và documentation tooling; không thay engine C# hoặc bootstrap Unity.

| Kiểm tra đã chạy | Kết quả |
|---|---|
| build_docs + check_handoff | 36 generated files; 29 module + phụ lục; 6.156 source anchors; 52 requirements, 28 tasks, 352 authored links; source preservation/DAG hợp lệ |
| check_contract_vectors | Codec bytes, SHA-256 known-answer, raw RNG vectors, rounding, bounded rejection branch, 3 invalid codec controls, smoke arithmetic/projection hashes hợp lệ |
| validate_reference | Design fixture schema subset/semantic refs hợp lệ; 4 negative controls reject |
| check_architecture | Assembly references/portable targets vẫn hợp lệ ở scope scaffold |
| ZIP workflow | Đã xóa ZIP, HANDOFF-MANIFEST/PACKAGE-REPORT và package.py; không tạo ZIP mới |

HTML/DOCX được regenerate sau khi tài liệu đồng bộ; machine-readable evidence của lần kiểm export nằm ở `artifacts/EXPORT-VALIDATION.json`, có số source fingerprints đã đối chiếu. Checker mới từ chối export nếu source inventory/hash khác manifest. Các hash của kernel fixture là **projection hashes**, không full-world runtime StateHash. Runtime acceptance tiếp tục Pending implementation; không báo C#/Unity replay hoặc persistence đã pass.

Export checker đã xác minh 14 HTML/DOCX outputs, 30 module/appendix titles, 6.156 anchors và 123 bảng trong Master DOCX. Negative control thực tế: thay source sau export khiến checker trả lỗi `Export sources changed`; sau đó regenerate bản đọc từ source mới. Chỉ kiểm container/content/navigation, không phải visual rendering hay gameplay test.

## Cập nhật 2026-10-15 — FND-02

[Evidence FND-02](FND-02-EVIDENCE.md): 6 projects Release build 0 warnings/errors; 32 headless C# tests pass, không skip; negative control 1 test fail với exit1 đúng yêu cầu. Phạm vi identity primitives, không thay các kết quả lịch sử hoặc các gate Unity/replay/gameplay chưa chạy.

## Sprint 1 — 2026-10-15

[Evidence](SPRINT-01-EVIDENCE.md): 70/70 C# Release tests, không skipped, negative control1failed/exit1 đúng yêu cầu; build6projects0warnings/errors. FND-02/03/04 headless accepted; scheduler/Unity/replay/gameplay vẫn pending.

## Sprint2 — 2026-10-16

[Evidence](SPRINT-02-EVIDENCE.md):124/124Release tests,0skip,negativecontrol1fail/exit1 đúng yêu cầu; CLI transfer+retry đạt; build6projects0warning/error. Scope in-memoryTransferResource FND05–08, không durability/Unity/replay.

## Sprint3 — 2026-10-16

[Evidence](SPRINT-03-EVIDENCE.md):138/138 Release tests (124 regression +14 event cases),0skip; negative control1fail/exit1; CLI transfer và duplicate event consumer đạt; build6projects0warning/error. FND09 transfer journal/causal trace và diagnostic projection/checkpoint trong bộ nhớ; không durability/scheduler/replay/Unity.

## Sprint4 — 2026-10-19

[Evidence](SPRINT-04-EVIDENCE.md):184/184Release tests(138regression+46scheduler),0skip; negativecontrol1fail/exit1 đúng; build6projects0warning/error. Scheduler demo2jobs tick10/20→target30,revision7,resource5/5,retryoriginalpending=true. FND10 ordering/advance/cancel/revalidation/bounds in-memory; không durable storage/recovery hoặc full gameplay breakpoint.

## Sprint5 — 2026-10-19

[Evidence Sprint5](SPRINT-05-EVIDENCE.md):224/224headless tests,0skip,40FND11cases;negativecontrol đúng;3SIGKILL subprocess trước/sau COMMIT đạt. SQLite current-state commit/reopen được nghiệm thu; periodic snapshot/recovery fallback/replay/Unity vẫn pending. Independent source review không còn blocker.

## Sprint6 — 2026-10-20

[Báo cáo Sprint6](SPRINT-06-EVIDENCE.md) mở đầu bằng tổng hợp dễ hiểu. Runtime256/256tests,0skip,32FND12cases;negativecontrol đúng;6save/migrationSIGKILL mới và3current-stateSIGKILL regression đạt. Independent source review không còn blocker. Nghiệm thu schema2 snapshot/committed-image fallback và new-file migration1→2; rule replay/Unity/platform replacement còn pending.

## Sprint7 — 2026-10-21

[Báo cáo Sprint7](SPRINT-07-EVIDENCE.md):284/284Release tests,0skip,28FND13cases;2process fixed golden15actions cùng kết quả; replay negativecontrol phát hiện index0. Runtime negativecontrol đúng và9SIGKILLregression đạt. Independent source/delta review không còn blocker. Scope transfer/scheduler QA; stochastic gameplay/Unity/full fielddiff còn pending.

## Sprint8 — 2026-10-22

[Báo cáo Sprint8](SPRINT-08-EVIDENCE.md):315/315Release tests,0skip,31FND14cases;46content subprocesses/15invalid cases, actual transfer/save reopen và exact dependency/hash refusal đạt. Replay golden/negative controls và9SIGKILL regression đạt. Independent source/delta review không còn blocker. Profile commodity/transfer-template; full VS02/Unity/playable game và FND15 còn pending.

## Sprint9 — 2026-10-22

[Báo cáo Sprint9](SPRINT-09-EVIDENCE.md):338/338Release tests,0skip,23FND15cases;16diagnostic processes/14negativecases tái hiện fixed fault và chỉ rõ fielddiff. Content/replay/negativecontrols/9SIGKILL regression đạt. Independent source/delta review và subprocess check đạt. Developer omniscient tooling, chưa VS01 milestone signoff/Unity/player UX/full gameplay acceptance.
