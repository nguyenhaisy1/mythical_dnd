# SPRINT-02 — Một command thay đổi world có kiểm soát

Mở 2026-10-15, Completed headless/in-memory scope ngày2026-10-16; SPIN-004. User giao AI họp, lập kế hoạch, chia/giao task, triển khai và nghiệm thu. Sprint 1 đã push lên origin/feature/sprint-01-primitives tại0315c66; dev hiện baseline94b0e53. gh chưa đăng nhập nên PR/merge vẫn pending; nhánh Sprint2 tạo từdev và fast-forward nhận Sprint1 làm dependency, không bypass PR bằng directpush dev.

## Biên bản planning bằng AI

Đầu vào: Sprint1 70tests passed; backlog FND05–08; Foundation/Command/Identity/Numeric/Ownership contracts. Participants thực: /root điềuphối, /root/s2_world và /root/s2_pipeline thống nhất API qua messages. Không có cuộc họp người thật hoặc người ký duyệt giả.

Quyết định: hiện thực vertical path TransferResource trong bộ nhớ để chứng minh read-only state → approved rule → ChangeSet → invariant → atomic publication → receipt. Một action đủ nhỏ để kiểm mọi failure boundary. WorldSnapshot thuộc Domain vì chứa Actors/Assets; Foundation chỉ WorldRevision, identity/time/RNG. Application sở hữu single writer/receipts/commit coordinator.

Không triển khai SQLite, durable jobs, full events/replay, Unity/UI, repair và cả gameplay economy. Các giới hạn được giữ trong nghiệm thu từng task; không dùng mock in-memory để báo durability pass.

## Task breakdown và assignment

| Task / phần việc | Actor | Dependency | Acceptance |
|---|---|---|---|
| S2-01 / FND-05 | /root/s2_world | FND02–04 | Immutable WorldSnapshot, revision checked, refs/lifecycle/quantity/rng context hợp lệ, không writable collection leak |
| S2-02 / FND-06 | /root/s2_world | S2-01 | Typed approved registry, duplicate/unknown handler reject, pure TransferResource proposal, authority/available/commodity/location checks |
| S2-03 / FND-07 | /root/s2_pipeline | API S2-01/02 | Envelope fingerprint đầy đủ, trusted caller context, duplicate-before-stale, payload mismatch, receipt rejection/noop không tăng revision |
| S2-04 / FND-08 | /root/s2_pipeline | S2-03 | Owner/field allowlist, expected-before, conservation/reservation, atomic in-memory candidate publication; lỗi trước commit không đổi gì |
| S2-05 / regression/evidence | /root | S2-01–04 | Per-task discovery; negative control; 70 Sprint1 regression; postcommit-response retry không execute lại; source manifest/log/TRX |
| S2-06 / review/nghiệm thu | /root + reviewer AI riêng | implementation + tests | Review adversarial diffs, sửa blocker, đồng bộ docs/backlog/changelog/exports, ghi RESULT/CLOSE |

S2-* là work breakdown trong sprint, không thay task IDs của BACKLOG.csv. Code agents chỉ sửa vùng được giao; root quản lý csproj/runner/docs/Git. Reviewer được ghi tên thực khi spawn.

## Kịch bản nghiệm thu

World gồm actor sống, settlement, household/actor ownership và hai commodity lots cùng loại. Chuyển3 từ10 sang0 →7/3, revision tăng đúng1, event/receipt cùng commit. Retry envelope gốc giữ7/3 và cùng receipt kể cả expected_revision cũ. CùngID khác bất kỳ fingerprint field trả CMD_PAYLOAD_MISMATCH không thay receipt gốc.

Sai quyền, deadactor, reference/world/type, tick/revision, thiếu tài nguyên hoặc reserved material không được chuyển. source=destination là no-op sau validation hợp lệ. Reject/noop lưu metadata receipt, không consume identity/RNG hoặc event/revision. Unexpected exception không bị ghi terminal gameplay rejection.

Failure injection trước publication giữ snapshot/ID/RNG/receipt/events nguyên trạng; sau publication nhưng trước response thì retry chỉ trả receipt đã commit. Mutation proposal sai owner/path, negative/overflow, stale before-value hoặc mất conservation bị invariant gate chặn độc lập handler. Readers thấy snapshot cũ hoặc mới hoàn chỉnh.

## Definition of Done và bàn giao

Release build + runtime gate có discovery tất cả task và no skips; golden/regression không đổi. Evidence ghi scope in-memory và source fingerprint; reviewer không còn blocker. Docs/source/reference/architecture/exportchecks đạt. Ghi Git SHA/push/PR thật; thiếu API auth không suy remoteCI/merge thành công.

Deliverable: code/tests, ADR011, [evidence](../../qa/SPRINT-02-EVIDENCE.md), worklog. Khi scope đạt đóng Sprint2 và báo cáo; bước sau FND09events/FND10scheduler, không tự mở sprint mới.

## Kết quả đóng sprint

S2-01–06 hoàn tất:124/124Release tests (70regression+13worldrules+41pipeline),no-skip; negativecontrol1fail/exit1; demo10/0→7/3 vàretry receipt giữ nguyên. /root/s2_review review độc lập implementers không phát hiện blocker. [Evidence](../../qa/SPRINT-02-EVIDENCE.md) ghi source fingerprint,limitations và artifacts. Không nghiệmthu fullFoundation/durability.

Sprint1 code đãpush và CI GitHub success tại0315c66. Sprint2 commit/push cuối và CI được ghi vào WORKLOG; PR/merge vào dev vẫn pending GitHub API auth. Main/release không đổi. Next FND09; không tự triển khai Sprint3.
