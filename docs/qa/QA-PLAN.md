# QA và nghiệm thu phát triển

[Ma trận 52 yêu cầu/scenario](TEST-MATRIX.csv) là bộ kiểm tra ưu tiên từ toàn bộ GDD, đặc biệt Modules 25–29. Đây **không phải danh sách exhaustive mọi requirement của game**. [Section Index](../governance/SECTION-INDEX.csv) giữ truy vết từng mục để mở rộng QA khi thực hiện subsystem. Module chi tiết còn các quality gates, ví dụ và test scenario chưa chuyển thành executable tests.

## Phân loại bằng chứng

- Documentation integrity: checksum nguồn, source-to-module completeness, generated-file consistency, links/anchors và archive manifest. Kiểm tra việc đóng gói, không chứng minh rule/game đúng.
- Build/host smoke: core projects build và CLI tự báo trạng thái. Chứng minh scaffold nối đúng, không chứng minh simulation/replay đã có.
- Unit/property: invariant, RNG vectors, formula boundaries, resource conservation. Thực hiện khi code domain/rules có mặt.
- Integration/recovery: cross-domain transaction, retry, crash, jobs và compatibility.
- Scenario/replay: “Bến qua mùa nước”, no-intervention, alternative choices, skip/legacy và command rerun.
- Client/playtest/performance: knowledge leakage qua UI, trải nghiệm nghề, readability combat, cross-runtime golden tests, frame/memory/throughput measurements.

## Gate theo milestone

FND-01 scaffold: build sạch, dependency graph đúng, no runtime dependency on Unity/network, tooling docs chạy được. Bản khởi tạo chỉ chứng minh gate này ở mức cấu trúc.

FND-02–10 kernel: typed identity/time/RNG, staged changes/invariants, idempotency và scheduler có test. Chưa mở rộng nhiều nghề cho tới khi hành động nhỏ transactional hoạt động.

FND-11–16 durability: fault injection, compatible save/load, replay, approved content registry và cross-runtime tests. Không gọi là Foundation complete khi thiếu gate này.

VS-02–06 living slice: work/resources/routes → knowledge/threads/autonomy → combat/health → long-term consequences/legacy. NPC phải có kế hoạch hợp lệ khi player không can thiệp.

VS-07–10 playable proof: Unity UI sử dụng public projections, playtest narrative comprehension, historical proof, ecology/belief proof và measured scale. Full production là quyết định sau evidence, không sau đếm số file.

## Cách ghi kết quả

Mỗi test run ghi commit/build ID, content/rule version, seed, fixture ID, platform/runtime, command line, expected/actual, artifact/log path. Test matrix không tự chuyển thành Passed do đã viết spec. [VALIDATION-RESULTS.md](VALIDATION-RESULTS.md) chỉ ghi các check thực sự chạy trong lần đóng gói.

Khi fixture cho thấy bug: giữ command log, manifest, snapshot/state diff và causal trace; tạo regression trước khi thay expected output. Không cập nhật golden hash chỉ để làm test xanh mà không review nguyên nhân.

## Gates và execution bổ sung

[Test Execution Contract](TEST-EXECUTION-CONTRACT.md) phân biệt reference checks, runtime tests và cross-runtime evidence. VS-01 chỉ nghiệm thu headless FND-02–15; Foundation complete thêm FND-16. Framework lock/CI discovery FND-02 theo ADR-009 và FND-02-EVIDENCE; Unity adapter còn gate riêng. Không thay TEST trạng thái runtime Pending bằng reference pass. Fixtures codec/math/RNG và smoke nằm tại [kernel-smoke](../../content/fixtures/kernel-smoke/README.md).
