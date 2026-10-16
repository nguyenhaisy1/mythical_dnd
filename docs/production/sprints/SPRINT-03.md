# SPRINT-03 — Sự kiện đã commit và truy vết nhân quả

Mở 2026-10-16; SPIN-006. Chủ dự án giao sprint mới với cùng workflow AI điều phối, chia task, triển khai, review và báo cáo hoàn tất. Scope FND-09: event envelope, journal ordering, truy vết command/correlation và consumer idempotency trong bộ nhớ. FND-10 scheduler, persistence, replay và Unity không thuộc cam kết sprint này.

## Planning và phân công

Đầu vào: Sprint2 có 124 tests và transfer pipeline atomic in-memory. PR #1/#2 đã tích hợp Sprint1/2 vào dev tại `190d8d9`; branch `feature/sprint-03-causal-events` từ dev này. Merge commit giữ ancestry của hai sprint phụ thuộc, không thay main/release.

| Task | Actor | Input/output | Acceptance |
|---|---|---|---|
| S3-01 contract | /root | Foundation contract + pipeline → ADR012 | Version, ordering, causal semantics và giới hạn rõ |
| S3-02 FND-09 events | /root/s3_events | Accepted transfer → immutable committed envelope/journal | Đủ metadata; rollback/reject/noop không emit; retry giữ event gốc |
| S3-03 FND-09 consumers | /root/s3_events | Committed journal → immutable projection/checkpoint | Duplicate không apply lại; payload collision reject; failure có thể retry an toàn |
| S3-04 integration | /root | Tests + CLI + docs → runtime gate/evidence | Có discovery TEST-037, regression, negative control và demo |
| S3-05 review | /root/s3_review | Final implementation/tests → findings | Kiểm adversarial; sửa blocker trước nghiệm thu |
| S3-06 handoff | /root | Evidence → backlog/matrix/changelog/export/Git | Đồng bộ nguồn; kiểm exports; push/CI/PR đúng revision |

Không có cuộc họp người thật; planning được ghi bằng quyết định và trao đổi giữa các actor AI thực tế. S3-* là breakdown của FND-09, không thay backlog cấp Foundation.

## Quyết định phạm vi

[ADR012](../../adr/012-committed-event-model.md) chốt event-v1 và consumer in-memory. Causation trong path hiện tại là command tạo event; correlation nhóm trace không cấp quyền đọc cho player. Journal/queries dành trusted diagnostics, chưa là knowledge-safe player projection. Không triển khai consumer gameplay reward/rumor hoặc scheduled job khi domain đó chưa có contract/runtime.

## Definition of Done

Release build, toàn suite có discovery và không skipped, negative control fail đúng dự kiến. Event tests chứng minh metadata/order/immutability, committed-only visibility, rollback, duplicate commands và duplicate delivery, collision, consumer failure/retry. Reviewer độc lập đọc code/tests; blockers được sửa và retest. Evidence phân biệt runtime in-memory với persistence/scheduler chưa làm. Docs/reference/architecture/export checks đạt; Git state và CI ghi thực tế trong WORKLOG.

## Kết quả nghiệm thu

Hoàn thành scope headless/in-memory ngày2026-10-16:138/138tests,0skip; negativecontrol1fail/exit1; demo chuyển3 và duplicate delivery chỉ apply1lần. /root/s3_review review độc lập, cross-world findings đã sửa và không còn blocker. [Evidence](../../qa/SPRINT-03-EVIDENCE.md) ghi fingerprint, artifacts và giới hạn. FND10 scheduler là bước tiếp theo, Sprint4 chưa mở. Git/CI/PR cuối ghi trong WORKLOG.
