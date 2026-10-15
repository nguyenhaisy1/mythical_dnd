# FND-02 — Headless identity evidence

> Báo cáo lịch sử SPIN-002. Regression và trạng thái sprint mới xem [Sprint 1 evidence](SPRINT-01-EVIDENCE.md).

Ngày 2026-10-15; SPRINT-01 / SPIN-002; actor /root. Scope REQ-027/TEST-027. Kết quả: **headless accepted**, Unity conformance pending FND-16. Sprint 1 vẫn đang làm; FND-03/04 chưa triển khai.

## Lệnh và kết quả

`python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`

- .NET SDK 10.0.401, Linux x64; core netstandard2.1, tests net10.0. Framework/adapter/SDK pinned và locked restore theo ADR-009.
- Release build 6 projects: 0 warnings, 0 errors.
- Runtime: discovered **32**, passed **32**, failed **0**, skipped **0**, exit 0.
- Negative control process riêng: discovered 1, passed 0, failed 1, skipped 0, exit 1 đúng yêu cầu; lỗi assertion mong đợi counter 2 nhưng thực tế 1. Toàn gate exit 0.
- Source revision: `sha256:ca3efcb9cf658b08212af7333b522a5eb3f9c7dba9c613ff7331462f1e501442`. Manifest bao gồm engine/tests/runner/fixture/config/CI, không chỉ Git HEAD trước thay đổi.
- Artifacts local ignored: `tests/results/fnd02-jy9izu9m/evidence.json`, `sources.json`, `runtime.trx`, `negative.trx`, restore/build/runtime/negative logs. CI workflow đã thêm gate và upload artifacts; chưa push nên **chưa có GitHub CI run**.

## Coverage có ý nghĩa

Parser strict/roundtrip (bao gồm 7 ID thật trong kernel-smoke), zero/uppercase/whitespace/unknown namespace reject, DefinitionId tách biệt, typed equality, ordinal ordering dưới culture tr-TR; wrong kind/world/missing ref error codes.

Allocate abort/retry giữ counter; prepare không thay snapshot nguồn; restore từ cursors/registry; 100 vòng aborted allocation rồi restore vẫn có chuỗi ID liên tục; đủ 10 kind và world độc lập. Cấp UInt64.MaxValue một lần rồi exhausted không wrap. Lifecycle retire/tombstone giữ ID/creation/retirement và không tái dùng; retention gate và invalid transition reject. Restore từ missing/duplicate/cross-world/revision/counter không nhất quán reject.

Lượt self-review riêng đã kiểm public API, bất biến snapshot, staged publication, namespace ordering, lockfile, CI và phân biệt candidate với world commit. Bổ sung 2 test về đủ namespaces/world và lifecycle abort/revision guards sau lượt đầu 30 tests; lượt cuối 32 tests ở trên áp source sau bổ sung. Không có independent reviewer/subagent.

## Giới hạn và lỗi môi trường

Lượt build parallel ban đầu lỗi MSBuild child-node MSB4166; chưa xác định nguyên nhân host. Runner dùng build tuần tự `-m:1 -nr:false`, đã thành công. Không quy lỗi đó thành bug game hoặc bỏ qua compile gate.

Không có SQLite/recovery qua process restart, atomic world commit/stale revision concurrency, command replay, Unity/IL2CPP hoặc gameplay/playtest. PrepareSnapshot chỉ là candidate của identity; FND-05/08/11 tích hợp cùng state/jobs/events/receipts. Restore ở đây là pure in-memory reconstruction từ persisted-shape primitives, không là durability acceptance.

Không đổi golden hoặc archive để làm test xanh. Không tạo integration project rỗng; storage integration được giao cùng FND-11.
