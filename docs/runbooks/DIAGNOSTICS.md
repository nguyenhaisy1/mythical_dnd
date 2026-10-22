# Chẩn đoán và tái hiện lỗi Foundation

[ADR-018](../adr/018-foundation-diagnostics.md), [Sprint9](../production/sprints/SPRINT-09.md). Công cụ omniscient cho developer QA, không player-facing projection.

## Chạy fixture được giữ cố định

```bash
dotnet build MythicalDnd.sln -c Release
dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --diagnose content/fixtures/diagnostic-kernel/package.json content/fixtures/content-kernel/pack.json
```

Exit0 nghĩa là baseline đã đạt và lỗi cố ý chèn được tái hiện đúng expected action/category; không nghĩa fault-run gameplay đúng. Exit1 là package/compatibility/baseline/reproduction không đạt và có JSON diagnostic. CLI chỉ đọc input, không mở store hoặc mutate save. Expected registry hash pin trong package; loader profile này không nhận dependency file arguments, thiếu dependency từ chối rõ.

Package version1 gồm artifact base64/checksum, expected state base64/checksum, expected action/category, fault ID và exact content/rule/seed. Giới hạn16MiB/depth8/256actions. Không nhận unknown/duplicate fields, fault script hoặc version/hash thay thế. Chỉ fault `reject-transfer-v1` đã biên dịch: actual TransferResourceRule evaluate rồi trả rejectionDIAGNOSTIC_INJECTED.

Fixture15actions chạy baseline; fault lệch ở index1/categoryresult. Expected lots7/3 revision2, actual10/0 revision1. State diff chỉ rõ `/lots/0/on_hand` và `/lots/1/on_hand`; còn metadata/receipts/journal/revision cũng được so. Baseline có hai jobs completed; đó không là state fault run. Golden hashes giữ cố định ở content/fixtures/diagnostic-kernel/golden.json; CI không tạo lại.

Lệnh `--record-diagnostic-fixture <new-file> <content-pack> <source-replay-artifact>` chỉ dùng khi cố ý author fixture, create-new không overwrite. Phải review mathematical outcomes rồi pin expectation; không dùng để sửa test đỏ. Các checksum giúp integrity/regression, không là chữ ký xác thực artifact.

## Đọc report và APIs

- `divergence`: action/category/expected-actual revision/hash đầu tiên lệch.
- `expectedActualDiff`: diff hai committed images, typed before/after và JSON-pointer paths; arrays positional theo codec canonical, không entity identity mapping. Missing khác Null, kiểu giá trị được giữ. Có TotalChanges/Truncated, cap1000 mặc định; không chỉ so resource totals.
- `baselineCommands`/`actualCommands`: action kind, envelope id/type/issuer/source/correlation/tick/payload/principal, revisions/outcome/error, actual evaluations, proposal effects, committed event IDs và before-after diff. Drain có envelope null; các internal evaluations thuộc action drain, job-execution edges trỏ command nội bộ thật.
- Checks chỉ những điều kiện đã chạy; omitted không là PASS. Proposal và outcome command tách nhau. Reject trước rule hoặc idempotent retry không tự tạo evaluationtrace. Transfer hiện không tiêu thụ RNG, report ghi rõ; không bịa roll/skill modifier.
- `baselineCausal`/`baselineJobs`: graph/job view của baseline chạy hết; `actualCausal` của lượt fault. API `CausalInspector.Traverse` hỗ trợ upstream/downstream; CLI xuất graph, chưa có traversal selector. Correlation không thành edge; upstream thiếu/mơ hồ được đánh dấu unresolved. Truncated graph không được coi complete.
- `SchedulerInspector`: pending/terminal job fields, actual execution/terminal event; reason chỉ khi có record, thiếu là not-recorded. CLI xuất subset đọc được; API giữ record đầy đủ.
- `RuleInspectionHandler`: capacity1000 mặc định, Traces immutable và DroppedCount. `StateDiffer`, graph output cap1..100000; đọc toàn bộ input để tính total nên không coi output cap là memory benchmark.

Diagnostics không sửa authoritative state, không dùng runtime LLM. Checks không persist vào save/replay hashes. Callback observer của QA có thể làm QA run fail nếu ném exception; không gắn vào production commit path. Gameplay/Unity/scale/UX vẫn có acceptance riêng.
