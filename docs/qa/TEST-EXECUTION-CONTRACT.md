# Test Execution & Evidence Contract

Ngày 2026-10-14. Áp dụng FND-02–16, VS-01/10. Các TEST trong matrix là acceptance requirements; chưa có C# implementation nên không được báo chúng Passed.

## Hai cấp thực thi

**Reference/documentation checks hiện có** chạy bằng Python stdlib: `python3 tools/checks/check_contract_vectors.py`, `validate_reference.py`, `check_handoff.py`, `check_architecture.py`. Lệnh đầu kiểm codec/math/RNG vectors và fixture expectation arithmetic; không chạy command processor C#. Evidence của chúng có scope `documentation-reference`.

**Runtime tests phải tạo cùng FND-02**: thêm `tests/MythicalDnd.Foundation.Tests/` và integration project theo ownership; .NET SDK theo global.json. Chọn một framework có runner CLI tương thích SDK và Unity adapter khi spike thực tế, pin exact packages + lockfile; không để wildcard. Đây còn là setup gate, chưa có package/runner được restore và kiểm chứng trong lượt bổ sung docs. Lệnh nghiệm thu dự kiến `dotnet test MythicalDnd.sln --configuration Release --no-build --logger trx`; chỉ bật gate sau khi test projects được đưa vào solution, runner phát hiện test count>0 và negative control cố tình fail khiến exit≠0. Không coi exit0 với zero tests là pass.

## Cấu trúc và naming

Unit tests kiểm primitives và pure rules, không cần DB/Unity. Integration tests dùng temp world + real persistence adapter, fault injection/process restart. Cross-runtime suite dùng cùng golden files trên headless và Unity, không re-generate expected khi chạy. Test names `TEST_027_Allocation_Abort_ReusesUncommittedCounter` hoặc equivalent; map requirement/test/task IDs vào output.

Fixtures trong `content/fixtures/`; golden vectors có phiên bản, provenance, sha và được review như contract. Thay implementation không tự overwrite golden để làm test xanh. Nếu contract đổi, ADR + version bump + giải thích before/after + cập nhật consumer/test/docs; giữ migration compatibility decision. Seed cố định cho golden; property tests lưu failing seed/minimized inputs.

## Evidence bắt buộc

Mỗi run lưu JSON ở `tests/results/` (ignored generated output): `scope`, `task_ids`, `test_ids`, `source_revision` (commit hoặc source fingerprint nếu chưa commit), `utc_started`, `os`, `architecture`, `runtime_version`, `unity_version` nullable, `backend` nullable, `fixture_versions`, `content_hash`, `rule_version`, `rng_version`, `seed`, `discovered`, `passed`, `failed`, `skipped`, `exit_code`, `artifact_paths`. Count khớp runner report; skipped không tính passed. Log lỗi phải nêu first divergent revision, state diff, event/job order và seed; không chỉ screenshot console xanh.

Không thu performance numbers từ documentation checks. Benchmark phải ghi hardware, build mode, world scale, warm-up, sample count, p50/p95/max, RAM, elapsed simulation years và invariants. Budget chỉ chốt sau measurements.

## CI và gates

CI hiện giữ docs/reference checks và scaffold build. Runtime test task còn pending phải hiện trong backlog/QA; thêm CI test stage ở FND-02, fail nếu discovery0. VS-01 yêu cầu kernel tests FND-02–15 và reference fixtures; Foundation complete thêm FND-16 cross-runtime; VS-10 cần gameplay regression+manual accessibility+performance evidence. Không đổi TEST status do chỉ bổ sung tài liệu.
