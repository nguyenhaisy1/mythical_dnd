# Sprint10 — Báo cáo phần chuẩn bị kiểm chứng Unity

## Tổng hợp dễ hiểu

Đã xây bộ kiểm tra chung để xác nhận dữ liệu và kết quả xử lý giống nhau giữa chương trình nền và Unity. Có công cụ chuẩn bị các thư viện cần nhập, tạo project kiểm tra riêng và báo rõ khi thiếu Editor. Đồng thời rà lại nền tảng của chín sprint, cập nhật các trạng thái cũ còn ghi chờ phần đã hoàn thành.

**Sprint10 còn mở:** máy chưa cài/kích hoạt Unity theo xác nhận của chủ dự án. Chưa có bản chơi thử, chưa nghiệmthu FND16/Foundation complete. Phần đã chạy chỉ là .NET và kiểm thử công cụ.

## Phạm vi, phân công và review

SPIN-013; branch `feature/sprint-10-unity-conformance`. Root implement portable probe, host CLI, Python staging/batch runner, validator tests, audit/docs. s10_audit/s10_unity/s10_review đóng góp đề xuất trước quota; không có implementation review từ họ. s10_final_review review độc lập tìm ba vấn đề: comparator nhận malformed reports giống nhau, thiếu thư mục output ở checkout mới, snapshot check chưa chứng minh snapshot được lưu. Root sửa cả ba; /root/s10_final_review kiểm lại và chạy checker độc lập: 4 host processes, 10 parity negative cases, missing Editor exit2, 13 imports. Không còn blocker cho preparation-only draft PR; không duyệt FND16 hoặc Sprint10 complete.

Input immutable replay fixture hash `bc82e0428cfd260d93b6a62a4eb09e472e73c2e15f8b7dab2b2f27d536423d2b`. Probe fingerprint cố định `d47e710ae21268d937742dc3fda208ae5e58b220309eeb2b276697a5367372f9` gồm năm kết quả semantic. Không đổi rule/schema/content versions hay gameplay behavior.

## Evidence và giới hạn

Kết quả host ban đầu: .NET10.0.12, SQLite3.53.3, codec/RNG/replay/content/commit-reopen đúng known answers. Preparation đầu phát hiện cách so hash archive NuGet với lock không đúng với cache hiện tại; sửa để kiểm metadata against lock và archive against SHA512 sidecar riêng. Không hạ kiểm chứng thành bỏ qua checksum.

Final Release gate `tests/results/sprint10-ku8vaxft/evidence.json`: exit0, 338/338 tests, 0 skipped; fingerprint `sha256:07f8e53efa21074e7b82c7edf59fabf0b28abf7de6ad7ea3c3fb29d3e0ce45a6`. Locked restore/build không warnings/errors; negative control thất bại đúng một test. Regression diagnostics/content/replay, hai demos và 9 SIGKILL checks đạt. Preparation: 4 host processes, 10 parity rejection cases, missing Editor exit2 NotRun, 13 imports. Lượt trước `sprint10-cn93gg2e` bị freshness gate từ chối vì source sửa trong lúc chạy; không dùng lượt đó để nghiệmthu. Synthetic parity reports chỉ là validator tests, không có Unity invocation thành công. Không có Player/IL2CPP evidence, benchmark hoặc human playtest. [VS01 review](VS-01-REVIEW.md) giới hạn ở kernel subset; [runbook](../runbooks/UNITY-CONFORMANCE.md) mô tả cách tiếp tục sau cài Editor.

## Bàn giao

Independent preparation review đạt; docs/reference/architecture checks đạt. Nhánh preparation được push và mở draft PR vào dev, chưa merge vì actual Unity gate chưa chạy. Sprint10/SPIN-013 tiếp tục khi Editor được cài/kích hoạt; không đóng sprint hoặc mở sprint mới. Chi tiết Git thực tế ghi append trong WORKLOG.
