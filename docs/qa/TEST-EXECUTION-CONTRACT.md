# Test Execution & Evidence Contract

Ngày 2026-10-14. Áp dụng FND-02–16, VS-01/10. Các TEST trong matrix là acceptance requirements; chỉ cập nhật theo runtime evidence từng task. FND-02–04 có primitives; FND05–08 có TransferResource in-memory; FND09 có event/consumer in-memory; Sprint4 kiểm scheduler in-memory riêng; Sprint5 kiểm SQLite current-state durability/reopen bằng file thật và SIGKILL subprocess. Periodic snapshots/recovery fallback/replay/Unity còn pending.

## Hai cấp thực thi

**Reference/documentation checks hiện có** chạy bằng Python stdlib: `python3 tools/checks/check_contract_vectors.py`, `validate_reference.py`, `check_handoff.py`, `check_architecture.py`. Lệnh đầu kiểm codec/math/RNG vectors và fixture expectation arithmetic; không chạy command processor C#. Evidence của chúng có scope `documentation-reference`.

**Runtime kernel regression** nằm ở `tests/MythicalDnd.Foundation.Tests/`, runner/package pin theo [ADR-009](../adr/009-headless-test-runner.md). Lệnh chuẩn: `python3 tools/tests/run_runtime.py` (hoặc `--dotnet /path/to/dotnet`). Tool restore locked, build solution Release, chạy dotnet test với TRX, kiểm count > 0 và không skip, rồi chạy negative control trong process riêng: đúng một test fail và exit khác 0. Không chỉ gọi dotnet test rồi suy zero tests là pass. Project hiện chứa cả primitive tests và in-memory command integration tests có Domain/Application references; Sprint5 thêm Infrastructure reference và SQLite integration tests vào cùng project; runner kiểm riêng crash subprocess. Không tạo suite rỗng.

## Cấu trúc và naming

Unit tests kiểm primitives và pure rules, không cần DB/Unity. In-memory integration tests kiểm command publication/failure. Durability integration phải dùng temp world + real persistence adapter, fault injection/process restart; không dùng in-memory pass để thay thế. Cross-runtime suite dùng cùng golden files trên headless và Unity, không re-generate expected khi chạy. Test names `TEST_027_Allocation_Abort_ReusesUncommittedCounter` hoặc equivalent; map requirement/test/task IDs vào output.

Fixtures trong `content/fixtures/`; golden vectors có phiên bản, provenance, sha và được review như contract. Thay implementation không tự overwrite golden để làm test xanh. Nếu contract đổi, ADR + version bump + giải thích before/after + cập nhật consumer/test/docs; giữ migration compatibility decision. Seed cố định cho golden; property tests lưu failing seed/minimized inputs.

## Evidence bắt buộc

Mỗi run lưu JSON ở `tests/results/` (ignored generated output): `scope`, `task_ids`, `test_ids`, `source_revision` (commit hoặc source fingerprint nếu chưa commit), `utc_started`, `os`, `architecture`, `runtime_version`, `unity_version` nullable, `backend` nullable, `fixture_versions`, `content_hash`, `rule_version`, `rng_version`, `seed`, `discovered`, `passed`, `failed`, `skipped`, `exit_code`, `artifact_paths`. Count khớp runner report; skipped không tính passed. Log lỗi phải nêu first divergent revision, state diff, event/job order và seed; không chỉ screenshot console xanh.

Không thu performance numbers từ documentation checks. Benchmark phải ghi hardware, build mode, world scale, warm-up, sample count, p50/p95/max, RAM, elapsed simulation years và invariants. Budget chỉ chốt sau measurements.

## CI và gates

CI giữ docs/reference checks, build và runtime regression cùng gate/demo của sprint hiện hành có negative control; upload JSON/log/TRX kể cả khi lỗi. Local run không chứng minh GitHub CI đã chạy. VS-01 yêu cầu kernel tests FND-02–15 và reference fixtures; Foundation complete thêm FND-16 cross-runtime; VS-10 cần gameplay regression+manual accessibility+performance evidence. Không đổi TEST status do chỉ bổ sung tài liệu.
