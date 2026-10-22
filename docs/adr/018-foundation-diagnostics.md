# ADR-018 — Diagnostics từ thực thi thật và evidence package

Ngày:2026-10-22. Trạng thái: quyết định triển khai Sprint9/FND-15; nghiệm thu theo sprint evidence.

## Bối cảnh

M29§80–85 yêu cầu command/rule/scheduler/causal trace và field diff. Replay-v1 chỉ lưu hashes; không thể suy expected state fields từ hash. Acceptance FND-15 là tái hiện lỗi được chèn có chủ đích từ evidence package. Scope hiện có transfer/scheduler, không tự tạo boat/health/social rules để minh họa.

## Quyết định

Domain RuleResult bổ sung immutable checks cho các điều kiện thực sự đánh giá trong TransferResourceRule. Kiểm tra dừng tại failure; điều kiện không xuất hiện nghĩa là chưa chạy. RuleInspectionHandler bọc handler thật, gọi đúng một lần, giữ input/result bất biến; proposal không đồng nghĩa commit. Unknown custom handler không được bịa checks. Checks không là authoritative state, không ghi vào event/receipt/save/hash và không đổi rule version vì quyết định gameplay không đổi.

Replay Record/Replay có observer tùy chọn chỉ dùng QA. Mỗi action đã thực thi phát một observation gồm action/context/before/after/outcome/error. Observer không được đăng ký vào production command processor. Lỗi bất ngờ không bị chuyển thành gameplay rejection; callback exception chấm dứt QA run. Có regression chứng minh observer không đổi artifact/checkpoints.

StateDiffer encode whole CommittedWorld bằng codec hiện hành rồi so typed leaves/container markers, gồm identity/cursors/actors/lots/RNG/receipts/events/jobs/workflows/executions. Path theo JSON pointer; array index là thứ tự canonical, không giả entity-aware matching. Missing khác null; integer khác string. Sort ordinal, số invariant. Entry cap có total/truncated; việc tính total vẫn đọc full images, chưa là performance optimization.

CausalInspector xây explicit receipt-event, event/job causation và job-execution edges. Correlation chỉ là metadata, không dùng suy parent. Cause không tìm thấy hoặc mơ hồ có nhãn external/unresolved. API traversal upstream/downstream giữ cảnh báo source truncation. SchedulerInspector đọc pending/terminal jobs, execution và terminal event; cancellation reason chỉ từ execution.ErrorCode, thiếu thì not-recorded, không đoán lý do.

## Evidence package v1

Gói JSON độc lập chứa replay artifact base64, checksum, exact content/rule/seed, expected action/category và expected committed image/checksum. Host compile registry profile hiện có và resolve exact hash trước execution; validates world refs. Replay baseline phải khớp tất cả checkpoints và expected image. Sau đó replay cùng initial/actions với fault allowlist `reject-transfer-v1`: evaluate TransferResourceRule thật, rồi cố ý trả DIAGNOSTIC_INJECTED rejection; không scripts hoặc reflection.

Report chứa first divergence, full expected-versus-actual diff, command traces, evaluated checks/proposals/committed events, graph và scheduler view. Baseline graph/jobs được gắn nhãn rõ; fault graph riêng. Report có thể chứa hidden truth nên chỉ dành developer QA, không player UI. Không mở SQLite store hoặc publish production effects. Hai process phải tạo cùng report; CI dùng fixture cố định, không regenerate golden.

Giới hạn host: package16MiB, JSONdepth8, tối đa256actions, trace capacity1000, diff/edge output1000 mặc định. Truncation/drop phải hiện rõ; giới hạn output không là benchmark hay memory streaming guarantee. Không giữ trace vô hạn trong production.

## Compatibility và phạm vi

Không đổi mdc1/save schema2/replay-v1/RNG hoặc content-v1. Diagnostic package có version riêng. Fixture diagnostic dùng content hash registry mới, không sửa replay golden cũ. Expected hashes lần đầu xuất từ implementation rồi pin; kết quả numeric/resource/revision được đối chiếu riêng, không giả độc lập mọi hash.

FND15 không tự nghiệm thu VS01 hoặc Foundation complete: VS01 cần review gate FND02–15 riêng, FND16 cần Unity execution. Chưa có full gameplay rule traces hoặc UI/player knowledge projections. Khi thêm fields vào committed codec, generic diff thấy chúng; khi thêm rule cần actual check instrumentation của rule đó.
