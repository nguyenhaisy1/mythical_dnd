# Changelog

Các thay đổi đáng chú ý của mythical_dnd. Product version tại [VERSION](VERSION); quy trình branch/tag/release tại [Git Workflow](docs/runbooks/GIT-WORKFLOW.md). GDD baseline có version riêng.

Chưa có tagged release. Snapshot đầu tiên `1e45977` trên main là tài liệu/scaffold, không phải bản game đã phát hành.

## [Unreleased]

### Sprint 3 committed events — 2026-10-16

- Thêm event-v1 cho transfer: causal/correlation metadata, rule/schema version, deterministic journal và truy vấn diagnostics.
- Thêm consumer projection/checkpoint bất biến trong bộ nhớ; duplicate delivery không apply lại, collision và delivery sai thứ tự bị reject.
- Nâng runtime gate TEST-037, giữ regression và negative control; tài liệu/ADR012/evidence đồng bộ.
- Compatibility: giữ API transfer cũ; rule version transfer-resource-v1 mô tả Application invariant semantics. Chưa có save migration, durable delivery, scheduler, command replay hay Unity acceptance.

### Sprint 2 command path — 2026-10-16

- Thêm immutable kernel world/revision, typed approved rule registry và TransferResource cho actor/household ownership.
- Thêm in-memory command processor: fingerprint/envelope/auth, idempotent receipt, stale/time checks, proposal owner/conservation validation và atomic publication có fault injection.
- Thêm CLI --transfer-demo và regression/per-task CI gate; review riêng, docs/evidence/plan/worklog đồng bộ.
- Compatibility: giữ identity-v1/clock-v1/mdc1/sha256-counter-v1; DTO in-memory không là save format public. Durability/SQLite/full event model/scheduler/replay/Unity vẫn pending.


### Sprint 1 primitives — 2026-10-15

- Thêm clock-v1 WorldTick/calendar/duration checked; mdc1 encoder, exact numeric math và sha256-counter-v1 immutable RNG.
- Mở rộng runtime gate có discovery từng task, golden conformance và negative control; AI phân công và review từng implementation.
- Compatibility: giữ versions ADR-008; chưa có scheduler, world commit, persistence/replay, Unity hoặc gameplay acceptance.

### Identity primitives — 2026-10-15

- Thêm FND-02 typed IDs, immutable registry/cursors, staged allocation/abort/restore và lifecycle giữ lịch sử theo identity-v1.
- Thêm NUnit headless tests, exact package lock, CI runtime gate kiểm TRX nonzero và negative control; evidence theo ADR-009.
- Compatibility: giữ identity-v1; chưa có save format/runtime migration, Unity hoặc atomic world commit đã nghiệm thu.

### AI execution — 2026-10-15

- Chốt workflow AI thực hiện, self-review và evidence gates; mở Sprint 1 cho FND-02/03/04. Không tuyên bố có human playtest hoặc Unity conformance.

### Added

- Bộ GDD 29 module và phụ lục, source archive, contracts, governance, backlog và QA specifications.
- Scaffold C# portable core/headless host, cấu trúc client Unity và tooling kiểm tài liệu/reference fixtures.
- Whitepaper HTML cùng pipeline sinh bản đọc HTML/DOCX.
- Sáu project skills và installer liên kết vào môi trường agent.
- Quy trình main/dev/work branches/release/hotfix, Conventional Commits, product version và changelog theo release.

- Nhật ký WORKLOG append-only theo spin, helper ghi có file lock và chỉ dẫn agent báo cáo goal/phân công/kết quả.

### Compatibility

- Product version khởi đầu `0.1.0-dev`; chưa có save/runtime compatibility được nghiệm thu.
- Baseline ID, clock, RNG, codec và activity nằm trong contracts; reference vectors chưa thay cross-runtime evidence.

### Validation

- Các kết quả docs/reference/scaffold hiện có: [Validation Results](docs/qa/VALIDATION-RESULTS.md).
- Kiểm skill metadata/installer: [Project Skills](docs/runbooks/PROJECT-SKILLS.md).
- Chưa có runtime gameplay acceptance hoặc Unity compatibility pass.

### Known limitations

- Simulation core mới là scaffold; chưa có bản chơi được hoặc full slice content pack.
- Quy tắc branch trong tài liệu chưa tự thiết lập branch protection trên GitHub.
