# Sprint 3 — nghiệm thu event/causal model in-memory

Nghiệm thu 2026-10-16, SPRINT-03/SPIN-006. **Đạt phạm vi FND-09 cho transfer journal và diagnostic consumer in-memory**. Không nghiệm thu scheduler, persistence, full gameplay event catalog hoặc Unity.

## Phân công và review

/root lập phạm vi/ADR012, điều phối, tích hợp CLI/runner, chạy gate cuối và đồng bộ tài liệu/Git. /root/s3_events triển khai event/consumer/tests. /root/s3_review review nguồn độc lập với implementer; đã phát hiện thiếu cross-world guard, yêu cầu sửa và thêm regression. Review cuối không còn blocker. Reviewer không tự chạy một suite riêng; kết quả runtime bên dưới do /root thực thi trên nguồn cuối.

## Runtime evidence

Lệnh: `python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`.

- SDK 10.0.401, Linux x64; locked restore; Release build 6 projects, 0 warnings/errors.
- **138 discovered, 138 passed, 0 failed, 0 skipped**: 124 regression Sprint1/2 + 14 TEST-037 cases.
- Negative control riêng: 1 discovered/1 failed/exit1 đúng mong đợi; toàn runtime gate exit0.
- Artifacts: `tests/results/sprint03-dy8e1032/evidence.json`, `sources.json`, TRX, restore/build/runtime/negative/demo logs. Đây là generated evidence ignored, không commit binary/logs vào source.
- Source fingerprint: `sha256:5d9cf70e6f003f7b06dfe3c927a0ca4917253a12b0e0c8cba2c3828b51a4ff08`.
- CLI transfer demo: 10/0 → 7/3, revision2, 1 event/receipt; retry trả receipt gốc. Giao event hai lần: projectionTransferred3, duplicateDeliveryApplied=false, consumerCheckpoints1.

## Hành vi được chứng minh

Event metadata có identity, aggregate/source lot, tick, commit sequence/index, command causation, correlation, type/rule/envelope/payload version và hai lot/quantity. Envelope/payload/journal/consumer state đều copy collections và không cho mutate qua exposed views. Journal giữ thứ tự commit và queries causal/correlation/aggregate; snapshot cũ không đổi khi có commit mới.

Rollback trước publication không phát event hoặc tiêu thụ event ID. Reject/noop không emit; retry command hoặc mất response sau commit không tạo fact thứ hai. Consumer idempotent cả concurrent delivery, duplicate cũ sau checkpoint mới và callback failure trước publication. Lỗi sau publication giữ checkpoint nên retry không apply lại. Payload/version/rule/correlation collision cùng event ID bị reject. Event mới sai thứ tự/cùng vị trí, khác world hoặc tick lùi bị chặn. Reentrant consumer không publish nested change.

TEST-037 đạt cho cơ chế projection/checkpoint này; gameplay reward, rumor hoặc scheduled job consumer chưa có implementation nên không suy đã nghiệm thu các domain đó.

## Giới hạn và bước tiếp theo

Các API là trusted omniscient diagnostics, không phải knowledge-safe UI/NPC projection. Causation hiện command→event; chưa là graph parent-event/job hoàn chỉnh. Rule version transfer-resource-v1 là semantics được Application invariant gate cưỡng chế, không là registry handler version tùy ý. Pure projection callback không được gây external side effect; atomic guarantee không bao phủ code vi phạm contract đó.

State/checkpoint trong bộ nhớ, chưa restore hoặc process-crash recovery. Journal/receipt/fingerprint lưu toàn lịch sử và copy khi publication; chưa tối ưu retention hay có benchmark. FND-10 tiếp theo chốt scheduler/fan-out/cycle/time advance; durable storage/recovery FND-11/12 và replay FND-13 tiếp sau. Không mở Sprint4 tự động.

## Tài liệu và Git

ADR012, sprint brief, Foundation contract, backlog/QA matrix, architecture/handoff/README, agent guidance, changelog và export tooling được đồng bộ. Các checks và Git SHA/CI/PR kết thúc ghi ở [WORKLOG](../../WORKLOG.md); source fingerprint trên chứng minh code kiểm tra, không thay bằng chứng remote CI.

Sprint1/2 đã tích hợp qua [PR1](https://github.com/nguyenhaisy1/mythical_dnd/pull/1) và [PR2](https://github.com/nguyenhaisy1/mythical_dnd/pull/2), dev190d8d9 trước Sprint3. Sprint3 làm trên feature/sprint-03-causal-events; main/tag/release không thay đổi.
