# Changelog

Các thay đổi đáng chú ý của mythical_dnd. Product version tại [VERSION](VERSION); quy trình branch/tag/release tại [Git Workflow](docs/runbooks/GIT-WORKFLOW.md). GDD baseline có version riêng.

Chưa có tagged release. Snapshot đầu tiên `1e45977` trên main là tài liệu/scaffold, không phải bản game đã phát hành.

## [Unreleased]

### Sprint13 — thử nghiệm chưa được duyệt

- Thử nghiệm03: ba hướng camera/chất liệu/màu khác nhau và gallery so sánh trung tính sau phản hồi màu bản02; built-inimagegen và prompt/provenance đầy đủ, vẫn chờ user chốt.

- Thử nghiệm02: painterly raster Bến Vạc tạo bằng built-in imagegen, UI ưu tiên cảnh và xem toàn tranh; giữ bản01 để so sánh sau phản hồi đồ họa chưa ưng ý. Vẫn Pending user approval.

- Bổ sung art direction và game design review vào mythical-prototype/mythical-validation; adaptation chọn lọc có pin nguồn và giữ MIT notices, không cài orchestration upstream.

- Thử nghiệm01 hình ảnh Bến Vạc, giao diện offline và mô hình xuất thân phục vụ UX được tách trong prototypes/sprint13; không import vào production hoặc coi là Unity gameplay.
- Chủ dự án chốt từng sản phẩm và kết thúc sprint; thêm approval register và quy tắc nhắc pending trước khi chuyển sprint.


### Sprint12 — event eligibility

- Thêm tám template sự kiện và compiler/evaluator declarative tách physical/perceived conditions; Unknown fail closed, readonly registry và exact settlement binding. Candidate chưa là occurrence hoặc gameplay effect.
- Thêm CLI/fixtures/checks và forecast hình ảnh Sprint13, demo nhỏ Sprint15–16 tại docs/production/PLAYABLE-ROADMAP.md. Không đổi save schema, settlement pack hoặc product version.


- Sprint11 VS02A:24nhân vật/6hộ/8tuyến có hướng, claims và quan hệ authoring với compiler strict/immutable registry. Không khởi tạo world, không chạy routine/event hoặc sinh protagonist; fullVS02 vẫnpartial.

- Sprint10: VS01 kernel và FND16 Unity6000.6.7f1/LinuxEditor Mono conformance đạt. Sửa staging/import và API enum alias; 5shared checks khớp host. Player/IL2CPP và playable game chưa nghiệmthu; không phát hành.

### Sprint9 — truy nguyên và tái hiện lỗi
- Actual rule checks, QA command observations, full typed state-field diff, causal traversal và scheduler inspection với missing/truncation rõ.
- Fixed diagnostic package chạy baseline rồi compiled injected rejection; report phân biệt baseline/actual và chỉ rõ first divergence/field changes. Hai process và negative controls giữ fixture cố định.
- Compatibility: diagnostic-v1 riêng; không đổi gameplay decisions/save/replay/RNG/content hashes. Developer omniscient tools chưa là player UI, Unity hoặc full Foundation acceptance.

### Sprint8 — kiểm tra và nạp nội dung
- Strict JSON authoring parser, portable semantic compiler và immutable registry cho commodity/transfer-template; pin dependency/version/hash, approved typed handler, diagnostic definition/path.
- Host chạy transfer từ template và save/reopen với exact content hash; invalid content bị từ chối trước mutation.
- Compatibility: profile kernel-content-v1 mới, không đổi save schema/replay/RNG; không tự chuyển save fixture cũ. Full roster VS-02 và Unity vẫn pending.

### Sprint7 — kiểm tra chạy lại hành động
- Replay QA ghi initial state, explicit versions/content/seed và ordered Execute/Drain attempts; thực thi lại processor và phát hiện checkpoint đầu sai khác.
- Golden15actions cố định,2subprocess replay và negativecontrol; gameplay hash tách metadata khỏi full-image integrity.
- Compatibility: replay-v1 riêng, không đổi save schema2; không live auth/production store, Unity, stochastic gameplay hoặc full field diff FND15.


### Sprint6 — snapshot và khôi phục bản lưu
- Schema2 manifest và periodic snapshots mỗi100accepted mutation commits; lưu thủ công không thay gameplay state hoặc cadence.
- Khôi phục đúng committed head từ snapshot và full-image suffix; migration1→2 tạo backup/output mới, giữ nguồn.
- Quy tắc báo cáo sprint mở đầu bằng tổng hợp tiếng Việt dễ hiểu, sau đó mới chi tiết kỹ thuật/evidence/Git.
- Compatibility: schema1 giữ API legacy; upgrade explicit. Không rule replay, Unity save UI hoặc atomic replace nguồn; xem ADR015 và Sprint6 evidence.


### Sprint 5 persistence — 2026-10-19

- Thêm Application durability port, original-envelope receipt retention và NeedsRecovery; mọi mutation/metadata/scheduler publish sau commit.
- Thêm mdc1 current-state codec và SQLite file store, full-image CAS, relational integrity mirrors, busy/full/rollback/unknown-outcome và actual SIGKILL harness.
- Pin Microsoft.Data.Sqlite10.0.13 và package locks; provider provisional Linux headless. ADR014 dùng UInt64 hex revision, full current image chưa là periodic snapshots/migrations/replay hoặc Unity provider acceptance.

### Sprint 4 scheduler — 2026-10-19

- Thêm immutable scheduled transfer jobs và AdvanceTime workflow cùng CommittedWorld publication; request/job/closing commits, cancellation, revalidation và idempotent retry.
- Stable ordering, phase deferral, budget1000jobs, fanout32, causal depth64/cycle guard và prototype decision acknowledgement; pending workflow chặn mutation ngoài scope.
- Thêm --scheduler-demo, scheduler runtime gate, regression cho publication exceptions/correlation và review độc lập; đồng bộ workflow thường trực, contracts/docs/exports.
- Compatibility: giữ identity-v1/clock-v1/RNG, thêm scheduler-v1 event metadata. Chưa có durable storage/recovery, full repair/Health/legacy/combat hoặc Unity acceptance.

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
