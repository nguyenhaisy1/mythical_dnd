# Quy trình thực hiện dành cho AI

Áp dụng từ 2026-10-15 theo yêu cầu chủ dự án: toàn bộ công việc phát triển do AI thực hiện. Các vai trò Foundation/QA/Design trong backlog là trách nhiệm chuyên môn, không phải nhân sự đang chờ tuyển hay người phải duyệt.

## Vòng thực hiện một task

1. Đọc baseline, handoff, worklog; kiểm branch và thay đổi đang có. Chọn task nhỏ nhất được giao, kiểm dependency bằng evidence.
2. Ghi task/requirement/test IDs, input, allowed files, output, acceptance, non-goals và actor thật vào sprint/worklog. AI tự quyết định chi tiết kỹ thuật trong contract; gap có ảnh hưởng contract phải ghi quyết định và đồng bộ consumers.
3. Thực hiện trên work branch; code, tests và tài liệu ảnh hưởng đi cùng. Không chuyển toàn roadmap thành một task.
4. Chạy gate phù hợp; sửa lỗi và chạy lại phần mất hiệu lực. Runtime gate phải có discovery > 0, không skipped, assertions thực và negative control chứng minh failure được phát hiện.
5. Thực hiện một lượt review riêng: đọc diff cuối, tìm lỗi ở boundary, invalid input, ownership, rollback/restore, dependency và rò knowledge. Cùng AI tự review phải ghi đúng là self-review, không gọi independent review. Multi-agent chỉ dùng khi được cho phép rõ.
6. Cập nhật backlog, evidence, changelog và worklog; commit/tích hợp theo Git Workflow. Không chờ người ký duyệt cho việc đã được giao. Push/main/release vẫn theo phạm vi ủy quyền.

Task dùng các trạng thái Ready → In progress → Verification → Headless accepted hoặc Done; Blocked chỉ khi có trở ngại cụ thể. Headless accepted không thay Unity acceptance. Sprint đóng khi mọi mục cam kết có evidence hoặc được ghi rõ chưa hoàn tất; không suy Done từ số giờ hay số commit.

## Nghiệm thu và giới hạn

AI chịu trách nhiệm viết code/content, tự động hóa kiểm tra và tổng hợp evidence. Không yêu cầu con người làm code review thường lệ. CI/test output là điều kiện nghiệm thu kỹ thuật; lời tự khẳng định của AI không thay bằng chứng.

AI có thể kiểm layout, chạy scripted playthrough và mô phỏng hành vi. Những kiểm tra đó được ghi là automated evaluation, không bịa phản hồi người chơi hay kết luận game vui. Nếu chưa có người chơi thật, gate trải nghiệm vẫn chưa được kiểm; không chặn các task kỹ thuật độc lập.

Thiếu Unity/DB/provider/quyền truy cập thì ghi command, lỗi, phần bị chặn và bước mở khóa; tiếp tục phần độc lập. Không cài công cụ hoặc truy cập dịch vụ ngoài phạm vi chỉ để giả hoàn tất gate.

## Sprint và spin

Sprint là phạm vi milestone; spin là đợt thực hiện có goal. Một sprint có nhiều spin. WORKLOG ghi actor AI thật và kết quả; không tự bịa một studio gồm nhiều agent. Không dùng velocity/person-days của đội người để ước lượng. Theo dõi deliverables đạt gate, blockers, số vòng sửa lỗi và evidence; chỉ báo thời gian thực tế đã quan sát.

Xem [Sprint 1](../production/sprints/SPRINT-01.md), [Git Workflow](GIT-WORKFLOW.md) và [Test Execution](../qa/TEST-EXECUTION-CONTRACT.md).

Sprint hiện hành: [Sprint7](../production/sprints/SPRINT-07.md), scope FND13 headless command replay. User đã giao điều phối/subagents/nghiệmthu; ghi từng actor thật. Các sprint đã đóng giữ evidence lịch sử, không rewrite nhật ký.

## Ủy quyền workflow thường trực

Từ 2026-10-16, user không cần lặp workflow ở mỗi yêu cầu sprint: AI tự planning/breaktask, giao implementation/test/review có ownership, chạy nghiệm thu, cập nhật tài liệu và exports, commit/push nhánh feature, PR vào dev và merge sau review/CI đạt, rồi báo cáo. Ghi trạng thái/evidence và actor thật. Chỉ áp dụng sprint được giao, không tự mở sprint tiếp theo hoặc main/tag/release.

## Sprint6 và định dạng báo cáo — 2026-10-20

User giao Sprint6 theo workflow thường trực; SPIN-009, docs/production/sprints/SPRINT-06.md, FND12 headless snapshot/recovery/migration theo ADR015. Không tự mởSprint7 hoặc release.

Từ nay báo cáo sprint phải bắt đầu bằng phần tổng hợp tiếng Việt dễ hiểu: sprint làm những việc gì, kết quả giúp ích gì và phần còn thiếu. Sau đó mới trình bày chi tiết kỹ thuật, phân công, kiểm thử/evidence, blockers và Git. Không dùng thuật ngữ kỹ thuật làm phần tổng hợp khó hiểu.

## Sprint7 — 2026-10-20

User giao tiếp tục sprint sau Sprint6. Sprint7/SPIN-010, FND13 headless command replay theo ADR016 và docs/production/sprints/SPRINT-07.md. Rerun Execute/Drain từ initial image, không đánh đồng đọc events/recovery images với replay. Không tự mởSprint8 hoặc release; giữ báo cáo tổng hợp tiếng Việt dễ hiểu trước chi tiết.
