# Sprint 8 — Kiểm tra và nạp dữ liệu nội dung

## Tổng hợp dễ hiểu

Sprint này xây đường đưa dữ liệu đã soạn vào game: kiểm tra định dạng, tìm tham chiếu thiếu, từ chối nội dung gọi hành vi chưa được cho phép và bảo đảm bản lưu dùng đúng bộ dữ liệu ban đầu. Mục tiêu là phát hiện lỗi nội dung trước khi chúng làm thay đổi thế giới.

Trạng thái: đạt headless scope ngày2026-10-22;315/315tests và46content subprocesses. Mở ngày2026-10-21. SPIN-011. Phạm vi FND-14, TEST-017/036. Đây là nền tảng cho sản xuất nội dung, chưa phải bộ nhân vật, nhiệm vụ hoặc bản đồ hoàn chỉnh của VS-02.

## Phạm vi và đầu vào/đầu ra

Input: JSON pack có phiên bản, nguồn, phân loại canon, định nghĩa commodity và transfer-template, dependency pin chính xác. Output: runtime registry bất biến, hash nội dung ổn định, lỗi có mã và definition/path; host thực thi transfer từ template đã kiểm định và mở lại save với đúng registry.

Chỉ hỗ trợ entity/rule contracts đã tồn tại. Loại definition, handler hoặc trường chưa được hỗ trợ phải bị từ chối; không suy diễn thành behavior. Nguồn nghiên cứu và reference fixture không biến thành runtime pack bằng đổi nhãn. Chi tiết quyết định ở [ADR-017](../../adr/017-runtime-content-registry.md).

| Task | Actor và ownership | Acceptance |
|---|---|---|
| S8-01 | /root: contract, schema, host parser | JSON strict, bounded input, diagnostic rõ |
| S8-02 | /root/s8_compiler: Infrastructure/Content | Compiler, immutable registry, typed handler, semantic refs, stable hash |
| S8-03 | /root/s8_tests: Fnd14ContentTests | Positive/negative runtime cases, exact compatibility và world refs |
| S8-04 | /root/s8_review: independent review | Review thiết kế/code, giải quyết findings trước nghiệm thu |
| S8-05 | /root: host/fixture/gates | Actual transfer/save reopen, invalid content trước mutation, process checks |
| S8-06 | /root: docs/exports/Git | Đồng bộ matrix/backlog/evidence, PR vào dev sau CI |

## Exit gates

TEST-017 từ chối unknown handler và executable field trước world mutation, chỉ rõ definition lỗi. TEST-036 từ chối absent/mismatched exact pack hash/version, không tự thay thế. Hash không lệ thuộc thứ tự authored records; metadata và dependency pin được hash. Registry không cho sửa sau compile. World commodity phải có trong registry trước sử dụng.

Giữ toàn bộ hồi quy Sprint 7, gồm replay hai process, negative controls và chín SIGKILL cases. Không nghiệm thu Unity, full production roster, balance hoặc hiệu năng quy mô lớn. FND-15 diagnostics là bước kế tiếp; không tự mở Sprint 9 hoặc release.

## Kết quả

[Báo cáo nghiệm thu](../../qa/SPRINT-08-EVIDENCE.md): compiler/parser/host và save compatibility đạt profile kernel; independent review không còn blocker. Root tiếp quản docs và final run sau agents quota. FND-15/Unity/VS-02 vẫn có gate riêng.
