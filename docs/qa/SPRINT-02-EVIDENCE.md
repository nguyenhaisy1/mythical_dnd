# Sprint 2 — nghiệm thu command path in-memory

Nghiệm thu 2026-10-16; planning/implementation bắt đầu2026-10-15. SPRINT-02/SPIN-004. Kết luận **đạt scope Sprint2**: FND05–08 WorldSnapshot/rule/command/ChangeSet cho TransferResource. Không phải fullFoundation hoặc durable persistence.

## Phân công, planning và review

/root điều phối, chốt API, runner/CLI/docs/Git và nghiệmthu. /root/s2_world triển khai FND05/06 (13tests); /root/s2_pipeline triển khai FND07/08 (41tests). /root/s2_review review read-only độc lập với hai implementers, không phát hiện blocker trong scope; root review/tích hợp final. Không có human reviewer/playtest giả.

Planning chọn path nhỏ để kiểm đầy đủ failure semantics, ownership actor/household và no-op/rejection. Chi tiết breakdown/decisions tại [Sprint2](../production/sprints/SPRINT-02.md), [ADR011](../adr/011-in-memory-command-pipeline.md). Reviewer đã yêu cầu coverage nonemptyRNG, metadata retries sau revision mới, unauthorized poisoning và malicious handler authority; các case đó hiện diện trong suite cuối.

## Lệnh và kết quả local

`python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`

- SDK10.0.401, Linux x64; 4portable libraries netstandard2.1, headless/tests net10.0. Locked restore; build6projects Release0warnings/0errors.
- Discovered124, passed124, failed0, skipped0. Gồm70Sprint1regression +13world/rules +41pipeline.
- Negativecontrol process riêng1test failed/exit1 đúng mong đợi; toàn gateexit0.
- Source fingerprint `sha256:8fd2e5e3b2c819214513a5b0bdf8602e7817c2a2ededafab8a8c6bc5fbde261e`. Artifacts `tests/results/sprint02-_g7_ufsp/evidence.json`, sources.json, runtime.trx, negative.trx, transfer-demo.log và restore/build/testlogs (ignored generated output).

| Nhóm discovery | Discovered | Passed |
|---|---|---|
| FND-02 | 32 | 32 |
| FND-03 | 25 | 25 |
| FND-04 | 13 | 13 |
| FND-05 | 7 | 7 |
| FND-06 | 6 | 6 |
| FND-07/idempotency | 3 | 3 |
| FND-07/mismatch | 17 | 17 |
| FND-07/stale | 6 | 6 |
| FND-08 | 15 | 15 |

Tên nhóm TEST_024/025/026 là routing gate của suite; một method có thể kiểm nhiều error/retry semantics. Matrix acceptance dựa assertions thực tế và phạm vi, không suy chỉ từ prefix.

## Hành vi đã chứng minh

WorldSnapshot copies collections, stable ordering, typed references active trong cùng world, actor/household controls, checked quantities/reservations/revision. Registry chỉ nhận approved compiled handlers, reject duplicate/unknown/version/type spoof; rule không ghi world.

Transfer10/0→7/3 với quantity3; world+acceptedreceipt+eventID+revision publish cùng một immutable aggregate. Retry cùng envelope trước stalecheck giữ nguyên result, trả đúng receipt cũ. Fingerprint gồm toàn envelope; mismatch không replace originalreceipt. Rejected/noop metadata không tăng revision/event/ID hoặc consumeRNG. Trusted principal được kiểm độc lập source_type trong payload; unauth attempt không poison commandID.

Lỗi trước publication giữ state/receipt/events/allocator/RNG; lỗi response sau publication retry không chuyển thêm. Metadata failure/retry cũng có tests. Concurrent duplicate chỉ commit1lần; competingrevision chỉ1winner; reentry bị chặn. Independent validator chặn malicious proposal saiowner/path/expected-before/negative/reservation/conservation/overflow hoặc bypass deadactor/authority.

Demo CLI `dotnet run --project engine/HeadlessRunner/Game.Headless.csproj -c Release --no-build -- --transfer-demo` được runner thực thi và kiểm: passed=true,durable=false,sourceAfter7,destinationAfter3,revision2,events1,acceptedReceipts1,retryReturnsOriginalReceipt=true.

## Scope chưa nghiệm thu

Không SQLite, process-crash recovery, durable receipts, job scheduler, full causal journal/event schema, replay, UI/player projections, Unity hoặc balance/performance. TEST006durability, TEST029craft/salvage, TEST039stochastic/material integration còn pending ngoài transfer subset. RNG states được giữ nguyên trong transfer, chưa có stochastic command integration. ContextKey alias có thể trỏ cùng canonical context; caller mapping/uniqueness phải chốt trước stochastic commands. WorldSnapshot là internal kernel subset, không full-world save schema hoặc player-safe DTO.

## Git và CI

Sprint1 đã push origin/feature/sprint-01-primitives tại0315c66f7dc1f1162babbb117ebe9e92cfa750fa; [GitHub CI](https://github.com/nguyenhaisy1/mythical_dnd/actions/runs/38700799347) success trên đúng SHA. origin/dev là baseline94b0e53. GitHub CLI chưa đăng nhập nên chưa tạo/merge PR; không directpush implementation vào dev.

Sprint2 trên feature/sprint-02-command-pipeline kế thừa Sprint1; implementation commits72e1d12 và1227a4a. Push/remoteSHA/CI của revision cuối được append vào [WORKLOG](../../WORKLOG.md) sau commit; xem event mới nhất, không dùng local test như remoteCI proof. Main/tag/release không đổi.

## Bước tiếp theo

FND09 full event/causal envelope rồi FND10scheduler; FND11 storage/atomic durability sau dependency. Sprint3 chưa mở. PR integration cần GitHub API auth; điều này không làm mất evidence local nhưng vẫn là bàn giao Git chưa hoàn tất.
