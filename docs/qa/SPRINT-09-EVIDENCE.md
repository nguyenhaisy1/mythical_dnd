# Sprint9 — Báo cáo truy nguyên và tái hiện lỗi

## Tổng hợp dễ hiểu

Sprint này bổ sung công cụ giúp tìm đúng nơi game tính sai: xem từng điều kiện đã được kiểm tra, kết quả luật trả về, dữ liệu thay đổi và chuỗi sự kiện/công việc liên quan. Gói bằng chứng có thể được chạy lại ở process mới để chỉ ra bước sai đầu tiên cùng giá trị cụ thể.

Tình huống nghiệm thu cố ý làm lệnh chuyển ba đơn vị gỗ bị từ chối. Công cụ phát hiện ngay bước đó: lẽ ra tài nguyên là7/3 nhưng thực tế vẫn10/0, đồng thời chỉ ra khác biệt revision, receipts và events. Đây là công cụ phục vụ phát triển; chưa phải bản game chơi thử hoặc giao diện cho người chơi.

**Đạt FND15 headless developer diagnostics**, nghiệm thu ngày2026-10-22. Sprint9/SPIN-012, FND15 headless developer diagnostics.

## Phân công và review

/root điều phối, ADR018, Domain actual checks, replay observer, host/package/gates/docs/Git. /root/s9_diagnostics triển khai Infrastructure diagnostics trước quota. /root/s9_tests để lại20cases trước quota; root sửa một testcase dùng payload key không hợp lệ mdc1 và bổ sung actual-check early-stop, observer unchanged và failed-job reason:23/23Debug đạt.

/root/s9_review independent source/delta review, yêu cầu nhãn baseline/actual và command context đầy đủ; root đã sửa. Reviewer tự chạy subprocess checker16processes/14negativecases exit0, evidence /tmp/s9-independent-diagnostics.json. Không còn blocking finding trong scope. Review không thay full runtime regression/Unity evidence.

## Runtime evidence cuối

Lệnh: `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`.

- SDK10.0.401/Linux x64; locked restore và Release build0warnings/0errors.
- **338/338 tests đạt, 0failed/0skipped**:315 hồi quy và23FND15 cases. Runtime negative control riêng đúng1fail/exit1; toàn gate exit0.
- Diagnostics16processes/14negativecases; content46processes/15invalidcases; hai demo/replaygolden/negativecontrol và9SIGKILL hồi quy đạt.
- Evidence `tests/results/sprint09-s40q56gp/evidence.json`, fingerprint `sha256:2b3591e211b883a698699ea9279eb79707843d7488f5e415ef00c942f8907e2c`; source freshness đã kiểm.
- Golden replay Sprint7 không đổi; observer và checks không thay authoritative hashes.

## Acceptance và giới hạn bằng chứng

- Rule wrapper gọi actual compiled handler đúng một lần; retry và rejection trước handler không có evaluation giả. Checks chỉ evaluated predicates, không bịa PASS/roll; proposal khác committed result. Transfer hiện không consume RNG.
- State diff toàn bộ committed codec gồm identity/cursors/actors/lots/RNG/accepted/metadata/events/jobs/workflows/executions. Typed missing/null/numeric/string, ordinal paths, explicit total/truncation. Arrays theo index canonical, không entity-aware matching.
- Causal edges từ persisted references; correlation không là parent. API traversal hai chiều, unresolved/ambiguous source rõ. Scheduler giữ actual execution/terminal event; thiếu cancellation reason là not-recorded, actual failed job có error code đúng.
- Package fixed15actions dùng exact content registry hash; expected image tại action1 được baseline replay xác nhận. Fault compiled reject-transfer-v1 evaluate thật rồi cố ý từ chối. Expected7/3revision2 so actual10/0revision1; first index1/categoryresult.
- Hai process phải có report bytes/hash giống nhau.14negative controls gồm unknown fault, schema/content/rule/seed/hash, expected action/category/image mismatch, malformed artifact, duplicate/unknown fields và input limit. CI không record lại fixture.

Golden package checksum2f0d55d04ef61f954997107f6e6d6d3ce9c38a505b1d0ccf29a256920d98b6d7; report checksum9747f17fc3dfeccbdabf460bd55df85a9122f255979fa61f05906c3ccd05597d. Các hash lần đầu xuất từ implementation rồi pin; kết quả vật chất/revision được đối chiếu riêng, không giả mọi hash có oracle độc lập.

## Compatibility và bước tiếp theo

Không đổi decisions/gameplay, save schema2/mdc1/replay-v1/RNG/content profile. Diagnostic package version riêng, không sửa golden replay cũ. Checks không persist vào authoritative state. QA observer có thể fail run nếu callback ném lỗi, không gắn production processor. Không DB mutation/LLM/reflection/scripts.

Omniscient report không được dùng làm player projection; baseline graph/jobs gắn nhãn rõ. Output bounds không là streaming/memory/performance guarantee. Chưa full gameplay traces/Unity/scale/player UX. VS01 cần milestone review riêng và Foundation complete cần FND16. Không tự mởSprint10/release. Git/CI cuối tại [WORKLOG](../../WORKLOG.md).
