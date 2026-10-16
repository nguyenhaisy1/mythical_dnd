# Bản đồ tài liệu

[Rà soát trước phát triển — 14/10/2026](production/PRE-DEVELOPMENT-AUDIT-2026-10-14.md): phần đã đủ, contract còn cần khóa và mốc phát triển tương ứng.

[White paper giới thiệu game — HTML độc lập](whitepaper/hong-bang-mach-lac.html): bối cảnh, ý tưởng, trải nghiệm và đặc trưng nổi bật.

| Nơi | Mục đích |
|---|---|
| [gdd](gdd/README.md) | 29 module chi tiết, phụ lục Life Event và Master đầy đủ |
| [source](source/INDEX.md) | Archive 85 thông điệp, checksum và provenance |
| [governance](governance/APPLICABLE-BASELINE.md) | Baseline, decisions, open issues, canon, requirements, source/section coverage |
| [adr](adr/README.md) | Rationale và hậu quả của các quyết định kiến trúc |
| [contracts](contracts/FOUNDATION-CONTRACT.md) | Foundation, ownership, canonical data/schema boundaries |
| [production](production/HANDOFF.md) | Handoff, implementation plan, backlog, vertical slice |
| [qa](qa/QA-PLAN.md) | 52 ưu tiên nghiệm thu, scenario và evidence thực tế |
| [content-guides](content-guides/AUTHORING.md) | Authoring, validation, historical labels và pipeline |
| [runbooks](runbooks/DOCUMENTATION.md) | Build/export/rebuild docs và bootstrap client |

Đọc nhanh không thay cho đọc module khi triển khai mechanic. CSV dùng UTF-8, có header; có thể mở bằng spreadsheet hoặc xử lý bằng tools. Decision register ghi cả supersession; section index cung cấp anchor ổn định để dẫn tới mục nghiên cứu cụ thể.

[Bổ sung tài liệu sau audit — 14/10/2026](production/DOCUMENTATION-COMPLETION-2026-10-14.md): đặc tả mới, phạm vi đã chốt và gates còn cần triển khai.

[Bộ skill của project](runbooks/PROJECT-SKILLS.md): sáu workflow cho task, prototype, change impact, kernel, content và validation; source duy trì cùng repository.

[Git Workflow](runbooks/GIT-WORKFLOW.md): branch model, commit/push, version, release/hotfix và changelog.

[WORKLOG theo spin](../WORKLOG.md): nhật ký append-only về goal, người/agent thực hiện, kết quả và trạng thái Git; khác changelog sản phẩm theo release.

Quy trình áp dụng cho AI: [AI Execution](runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope Sprint 1 nằm trong sprint brief được liên kết tại đó.

Sprint đã nghiệm thu local: [SPRINT-01](production/sprints/SPRINT-01.md). Runtime evidence đầu tiên: [FND-02](qa/FND-02-EVIDENCE.md); runner/API decision: [ADR-009](adr/009-headless-test-runner.md).

[Nghiệm thu Sprint 1](qa/SPRINT-01-EVIDENCE.md): identity/time/codec/RNG và các giới hạn.

[Sprint2 planning/acceptance](production/sprints/SPRINT-02.md), [ADR011](adr/011-in-memory-command-pipeline.md) và [evidence](qa/SPRINT-02-EVIDENCE.md) mô tả command path in-memory mới.

[Sprint3 planning/acceptance](production/sprints/SPRINT-03.md), [ADR012](adr/012-committed-event-model.md): event-v1 và consumer idempotency in-memory.

[Nghiệm thu Sprint3](qa/SPRINT-03-EVIDENCE.md): 138 headless tests, event metadata/trace và idempotent consumer; scheduler là bước tiếp theo.
