# Nhật ký công việc mythical_dnd

File báo cáo công việc duy nhất theo **spin**, duy trì bằng cách **append vào cuối file**. Không sửa/xóa bản ghi đã có; sai sót được đính chính bằng bản ghi mới dẫn ID cũ. CHANGELOG mô tả thay đổi của sản phẩm theo release; WORKLOG ghi quá trình làm việc, phân công và evidence, không thay thế nhau.

## Quy ước đọc và ghi

- **Spin** là một đợt công việc có mục tiêu và đầu ra rõ; không đồng nghĩa một tool call, một ngày, sprint cố định hoặc một agent. Dùng `SPIN-001`, `SPIN-002`… tăng dần. Yêu cầu bổ sung cùng mục tiêu tiếp tục spin đang mở; mục tiêu mới tạo spin mới. Khi resume, ghi thêm RESUME/HANDOFF, không viết lại lịch sử.
- `SPIN-000` dưới đây chỉ là lịch sử hồi cứu trước khi có nhật ký. Không bịa số spin, thời lượng hoặc phân công cho giai đoạn đó.
- Mỗi event có ID, thời điểm ghi có timezone, spin, actor thật, loại event và nội dung. Goal ghi ở OPEN; phân công ghi ở ASSIGN; tiến độ/kết quả/failures/commit/push ghi ở event tiếp theo. Assignment thể hiện trạng thái tại lúc ghi; kết quả sau đó có thể thay đổi trạng thái.
- Actor dùng định danh thật: chủ dự án, `/root`, tên thành viên hoặc agent ID thực sự được tạo. Skill là công cụ/workflow, không phải nhân sự; không ghi tên một “team” chưa chạy. Người điều phối tổng hợp event của các agent để tránh báo kết quả trước khi nhận evidence.
- Trạng thái suy từ event mới nhất, không sửa một bảng “current” ở đầu file. OPEN/RESUME → đang làm; BLOCKED → vướng; PAUSED → tạm dừng theo yêu cầu; CLOSE → kết thúc theo kết quả ghi trong body. Khi CLOSE vẫn ghi rõ finished / partial / cancelled, không biến partial thành Done.
- Ghi khi bắt đầu, nhận/chuyển việc, đạt kết quả đáng kể, có lỗi/blocker/quyết định ảnh hưởng, chạy checks, commit/push/merge/release, bàn giao và kết thúc. Không cần ghi mọi lệnh tìm kiếm hoặc dòng chat. Không đưa secrets/private keys/token hoặc log thô khổng lồ vào đây.
- Chứng cứ ưu tiên task/requirement ID, đường dẫn file, lệnh + exit/result, CI/test artifact, commit/branch/remote SHA. Phân biệt source/reference/build/runtime/Unity/playtest; không biến dự định thành kết quả.
- Khi chủ dự án hỏi báo cáo: đọc goal và các events của spin liên quan; tổng hợp ai làm gì, đạt/chưa đạt, evidence, blockers, trạng thái Git và bước tiếp theo. Dùng current Git/test state để xác minh nếu cần, append CORRECTION khi phát hiện log cũ sai. Không coi absence of log là task đã xong. Có thể trả báo cáo trong hội thoại; không tạo thêm file report trùng nguồn nếu không cần.
- Có thể ghi nối bằng `python3 tools/worklog.py --spin SPIN-001 --event PROGRESS --actor /root --summary "..." --details-file /path/to/details.md`. Mở spin mới bằng `--new-spin --event OPEN`, body ghi goal/scope/acceptance. Helper khóa file khi append, cấp event ID và timestamp; không tự chạy nhiệm vụ hay sửa trạng thái backlog.
- WORKLOG là live journal, không nhúng vào Handbook/export snapshot. Link tới file gốc để không phát sinh vòng lặp “export xong → ghi log → export stale”. Các log commit/push sau commit đương nhiên tạo diff mới; đưa vào commit kế tiếp, không amend/force-push hoặc commit lặp vô hạn để log tự chứa hash của chính nó.

## SPIN-000 — Hồi cứu trước khi có nhật ký

Ghi hồi cứu ngày 2026-10-14 (Asia/Ho_Chi_Minh) từ hội thoại, tài liệu hiện có và Git. Những ngày/giờ không có evidence không được suy đoán. Participants xác nhận được: chủ dự án giao yêu cầu và assistant thực hiện; không có nhật ký assignment riêng để phân rã nhân sự các bước trước.

### HIST-001 — Nhập nguồn và khôi phục thiết kế

- Goal: bảo toàn toàn bộ nghiên cứu thay vì thay bằng GDD tóm tắt.
- Work/result: lưu 85 thông điệp user/assistant nguồn; khôi phục 31 khối dài thành 29 module và phụ lục 06A; 1.436.697 ký tự normalized và 6.156 source section anchors. Tạo module/section mapping và baseline giải quyết xung đột; giữ archive bất biến.
- Evidence: `docs/source/manifest.json`, `docs/governance/RECOVERY-REPORT.md`, `SOURCE-COVERAGE.csv`, `SECTION-INDEX.csv`, `tools/docs/build_docs.py`.
- Limits: văn bản nghiên cứu không phải runtime content pack hoặc 1.000 NPC đã sản xuất.

### HIST-002 — Init cấu trúc project và tài liệu phát triển

- Goal: chuẩn bị repo để bắt đầu implementation theo ownership, requirements và milestones.
- Work/result: portable core Foundation/Domain/Application/Infrastructure, headless host, layout client Unity; contracts/ADR/governance/backlog/QA, content fixture, build/check/export tooling. Có 52 requirements và 28 backlog tasks.
- Evidence: `ARCHITECTURE.md`, `MythicalDnd.sln`, `docs/production/HANDOFF.md`, `docs/qa/VALIDATION-RESULTS.md` ghi build SDK10.0.401, 5 projects, 0 warnings/errors ở lần kiểm ban đầu.
- Limits: core mới scaffold, chưa có command engine/save/replay gameplay; Unity chưa được bootstrap bằng Editor. FND-02 trở đi không tự được đánh Done vì docs/build pass.

### HIST-003 — Whitepaper và rà soát readiness

- Goal: giới thiệu game và tìm khoảng trống trước phát triển.
- Work/result: whitepaper HTML độc lập; audit ghi 11 readiness gaps về contracts, fixture, QA execution, content, persistence, client và gates/freshness.
- Evidence: `docs/whitepaper/hong-bang-mach-lac.html`, `docs/production/PRE-DEVELOPMENT-AUDIT-2026-10-14.md`, `docs/governance/READINESS-GAPS-2026-10-14.csv`.
- Limits: whitepaper không phải UI/game playable; audit là trạng thái tại thời điểm ghi, có báo cáo bổ sung phía sau.

### HIST-004 — Bổ sung contracts, fixtures và bỏ ZIP

- Goal: bổ sung tài liệu còn thiếu trong project và đồng bộ mọi consumer bị ảnh hưởng.
- Work/result: xóa ZIP và packaging workflow; thêm Identity/Time/Math-RNG/Command/Save specs, test execution, client UX, content contract, kernel reference vectors/parameters và ADR-008. Đồng bộ modules qua integration notes, .agent, backlog, QA, runbooks, whitepaper; export fingerprints kiểm source freshness. Tách VS-01 headless gate khỏi full Foundation cần FND-16.
- Evidence: `docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md`, `content/fixtures/kernel-smoke/`, `rules/parameters/kernel-v1.json`. Reference/architecture/docs checks đạt; negative export control phát hiện source thay đổi; 7 HTML + 7 DOCX được kiểm container/content/navigation.
- Limits: các lựa chọn prototype không phải kết quả balance; provider/Unity/test framework, full content/runtime tests vẫn pending theo task. Không đóng mọi readiness gap chỉ bằng spec.

### HIST-005 — Định vị giai đoạn và đánh giá công cụ

- Goal: đối chiếu workflow studio lớn và chọn skill/tool phù hợp tech stack.
- Work/result: xác định project chuẩn bị prototype, thiết kế đi trước bằng chứng gameplay. Đề xuất thử vòng chơi Bến Vạc nhỏ song song kernel. Đọc Unity skills, Coplay MCP và Claude Code Game Studios; đề xuất Unity official khi bootstrap, workflow riêng được điều chỉnh và Coplay tùy kết quả thử.
- Evidence: hội thoại; provenance repo snapshots trong `docs/runbooks/PROJECT-SKILLS.md`.
- Limits: chưa chạy Unity tooling hoặc prototype/playtest; không cài Unity official/Coplay trong đợt đánh giá.

### HIST-006 — Tạo và cài sáu project skills

- Goal: workflow theo task, change impact, validation, prototype, kernel và content của mythical_dnd.
- Work/result: sáu skills tại `tools/skills/`, metadata và installer; symlink cá nhân tới source. Cập nhật AGENTS/.agent/runbooks và Handbook. Validator cả sáu đạt; installer kiểm fresh/repeat/check/collision trong thư mục tạm rồi cài thật.
- Evidence: `docs/runbooks/PROJECT-SKILLS.md`, `tools/skills/install.py`; danh mục skills hiện tại đã có sáu tên mythical-*.
- Limits: static scenario review không là independent agent execution; chưa chứng minh hiệu quả mọi workflow trong task thực tế.

### HIST-007 — Initial commit và push GitHub

- Goal: đưa toàn bộ source/tài liệu project lên repository được chỉ định.
- Work/result: commit `1e45977c0791154bb778230c06801782e0052aeb` gồm199 files; push main tới `git@github.com:nguyenhaisy1/mythical_dnd.git`, kiểm remote SHA khớp local. Commit timestamp từ Git: `2026-10-14T16:42:52+07:00`.
- Checks: source inventory không phát hiện các mẫu secret đã quét; docs/reference/architecture checks đạt. Artifacts/caches excluded theo .gitignore.
- Issue resolved: SSH host-key verification ban đầu fail; lấy host keys từ HTTPS GitHub API, kiểm kết nối rồi push thành công. Không ghi key/credential vào journal.
- Limits: initial snapshot, không tagged release; không có game build đã phát hành.

### HIST-008 — Quy trình branch, commit/push và release

- Goal: thêm chỉ dẫn Git vào agent, chia branch và changelog theo release.
- Work/result: `.agent`/AGENTS + `docs/runbooks/GIT-WORKFLOW.md`; Conventional Commits; feature/fix/docs→dev→release→main; hotfix back-merge; VERSION=`0.1.0-dev`, CHANGELOG Unreleased, PR template. Đồng bộ skills, CONTRIBUTING/runbooks/exports.
- Git: tạo local dev và docs/git-release-workflow từ main; thay đổi của bước này **chưa commit/push** tại lúc bắt đầu nhật ký. main/origin/main vẫn ở1e45977. Không cấu hình GitHub branch protection hoặc tạo tag/release.
- Checks: 372 authored links, source preservation/DAG, six skill validators và diff check đạt; export validation128 source inputs/14outputs,30module titles,6156anchors,123tables.

## Nhật ký từ khi áp dụng spin


### EVT-00001 | SPIN-001 | OPEN — Thiết lập nhật ký công việc và báo cáo theo spin

<!-- worklog: {"id": "EVT-00001", "spin": "SPIN-001", "event": "OPEN", "at": "2026-10-14T16:55:12+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-14T16:55:12+07:00
- Actor: /root

- Goal: tạo một file append-only ghi toàn bộ lịch sử có thể xác nhận, rồi theo dõi goal/assignment/result theo spin để báo cáo khi chủ dự án hỏi.
- Scope: WORKLOG.md, helper append, chỉ dẫn agent/skills và tài liệu dẫn tới log; giữ nguyên thay đổi Git workflow chưa commit của bước trước.
- Acceptance: lịch sử có provenance và giới hạn; event có ID/time/spin/actor; append không sửa bản ghi cũ; agent biết ghi assignment/progress/result/close và tổng hợp báo cáo; checks/docs/exports liên quan hợp lệ.
- Coordinator/implementer: /root (assistant hiện tại). Chủ dự án là người giao mục tiêu. Không có subagent được tạo cho spin này.
- Git khi mở: docs/git-release-workflow; HEAD=1e45977; Git workflow changes local chưa commit/push. Không có yêu cầu commit/push trong spin này.


### EVT-00002 | SPIN-001 | ASSIGN — Phân công và trạng thái đầu spin

<!-- worklog: {"id": "EVT-00002", "spin": "SPIN-001", "event": "ASSIGN", "at": "2026-10-14T16:55:12+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-14T16:55:12+07:00
- Actor: /root

| Người/agent | Việc được giao | Trạng thái lúc ghi |
|---|---|---|
| /root | Hồi cứu lịch sử, tạo WORKLOG/helper và quy tắc append | Đang làm; file sơ bộ đã tạo |
| /root | Đồng bộ agent/skills/runbooks, kiểm append và exports | Tiếp theo |
| Chủ dự án | Đã cung cấp mục tiêu và yêu cầu báo cáo | Không có quyết định cần chờ |

Skills chỉ hỗ trợ workflow; không có team/agent khác đang làm song song. Hồi cứu HIST-001–008 không suy ra giờ làm hoặc tên người không có evidence.


### EVT-00003 | SPIN-001 | PROGRESS — Nhật ký và chỉ dẫn đã có; checks helper và docs đạt

<!-- worklog: {"id": "EVT-00003", "spin": "SPIN-001", "event": "PROGRESS", "at": "2026-10-14T16:56:47+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-14T16:56:47+07:00
- Actor: /root

- /root đã hoàn tất bản hồi cứu HIST-001–008, quy ước append-only và helper tools/worklog.py. Source đã có WORKLOG.md, chưa commit/push.
- Đã đồng bộ AGENTS/.agent, README/docs index, HANDOFF, DEVELOPMENT/DOCUMENTATION/GIT-WORKFLOW/PROJECT-SKILLS, ba skill task/prototype/validation và CHANGELOG Unreleased.
- Helper được kiểm bằng file tạm: 16 concurrent appends, IDs duy nhất/theo thứ tự, prefix lịch sử giữ nguyên; duplicate OPEN/unknown spin/multiline header reject và không đổi bytes; mở spin tiếp theo cấp SPIN-002. Đây là kiểm helper logging, không runtime game.
- check_handoff: 29 modules + appendix,6156sections,52requirements,28tasks,379links đạt. git diff --check đạt. Sáu skill validators đạt bằng /usr/bin/python3.
- Việc còn làm: regenerate/check14HTML-DOCX sau thay đổi chỉ dẫn; append evidence và CLOSE. WORKLOG không thuộc export inputs, để ghi kết quả không làm export stale.
- Blockers: không có. Các phần engine/Unity/full content còn pending từ trước không thuộc goal spin này.


### EVT-00004 | SPIN-001 | RESULT — Hoàn thành journal và kiểm chứng liên quan

<!-- worklog: {"id": "EVT-00004", "spin": "SPIN-001", "event": "RESULT", "at": "2026-10-14T16:58:07+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-14T16:58:07+07:00
- Actor: /root

- Kết quả: /root đã hoàn thành WORKLOG.md và helper, đồng bộ chỉ dẫn agent/skills/tài liệu; hồi cứu lịch sử và event log mới được giữ trong cùng file.
- Validation cuối: check_exports.py exit0;128source inputs,14outputs,30module titles,6156anchors,123tables,internal navigation valid. git diff --check exit0. Kiểm journal metadata/IDs đạt; WORKLOG được xác nhận nằm ngoài export fingerprints.
- Acceptance đã đạt ở scope logging/docs: append helper giữ prefix, không trùng event IDs khi ghi đồng thời; có goal/assignment/progress/result; source links và bản đọc được đồng bộ.
- Limits: không có background watcher tự bắt mọi filesystem change; agent phải append theo AGENTS ở các mốc đã quy định. Kiểm skill/logging không là runtime gameplay acceptance.
- Git: nhánh docs/git-release-workflow, HEAD1e45977. Thay đổi Git workflow trước đó và spin này còn local, chưa commit/push. Không tạo tag/release hoặc cấu hình branch protection.


### EVT-00005 | SPIN-001 | CLOSE — Đóng spin thiết lập nhật ký — finished

<!-- worklog: {"id": "EVT-00005", "spin": "SPIN-001", "event": "CLOSE", "at": "2026-10-14T16:58:07+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-14T16:58:07+07:00
- Actor: /root

- Outcome: finished — mục tiêu thiết lập nhật ký/báo cáo theo spin đã hoàn tất.
- Assignment cuối: /root hoàn thành toàn bộ việc trong spin; không có subagent, không còn blocker của spin này.
- Spin hiện tại khi kết thúc: SPIN-001 đã đóng, không còn spin active. Đợt mục tiêu mới kế tiếp mở SPIN-002; bổ sung cùng mục tiêu có thể RESUME SPIN-001 nếu được yêu cầu.
- Pending ngoài spin: commit/push bộ thay đổi workflow+journal chưa được giao; Foundation runtime, Unity và content production vẫn là các tasks tương lai, không được đánh Done.
- Khi được hỏi báo cáo: dùng SPIN-000/HIST cho lịch sử trước log, EVT-00001–00005 cho đợt thiết lập này; đối chiếu Git state hiện tại trước báo trạng thái publish.


### EVT-00006 | SPIN-002 | OPEN — Khởi động Sprint 1 với baseline AI và FND-02

<!-- worklog: {"id": "EVT-00006", "spin": "SPIN-002", "event": "OPEN", "at": "2026-10-15T09:07:21+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T09:07:21+07:00
- Actor: /root

- Sprint: SPRINT-01. Goal spin: chốt baseline AI thực hiện, khởi động sprint và hiện thực FND-02/REQ-027/TEST-027 ở headless.
- Actor thực tế: /root thực hiện planning, code, kiểm tra và review pass riêng; không có subagent, không tuyên bố review độc lập.
- User ngày 2026-10-15 xác nhận toàn bộ dự án do AI làm và yêu cầu tiến hành bước tiếp theo. Chốt baseline bằng commit local; không publish/main/tag/release.
- Scope: workflow AI, sprint brief, typed identity/registry/allocation, test runner/CI/evidence và docs liên quan. FND-03/04 còn trong sprint nhưng ngoài spin triển khai đầu tiên này. Unity/storage/gameplay chưa nghiệm thu.
- Giữ nguyên thay đổi workflow/journal đang có; kiểm và đưa vào baseline trước branch feature. Dev local chưa publish: bootstrap fast-forward local sau checks, không thay PR policy của branch đã publish.


### EVT-00007 | SPIN-002 | GIT — Chốt baseline local và mở branch FND-02

<!-- worklog: {"id": "EVT-00007", "spin": "SPIN-002", "event": "GIT", "at": "2026-10-15T09:09:05+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T09:09:05+07:00
- Actor: /root

- Baseline local commit 94b0e53: workflow Git/release, append-only journal, AI execution và Sprint 1. Checks docs/source/reference/architecture/diff đạt. Export đang sinh và sẽ regenerate cuối sau implementation.
- dev local bootstrap fast-forward tới baseline; tạo feature/fnd-02-typed-identity từ dev. main/origin không đổi; chưa push.
- /root tiếp tục FND-02; self-review và runtime verification cùng actor, không có reviewer độc lập.


### EVT-00008 | SPIN-002 | PROGRESS — FND-02 đạt gate headless; đồng bộ tài liệu và hoàn tất export

<!-- worklog: {"id": "EVT-00008", "spin": "SPIN-002", "event": "PROGRESS", "at": "2026-10-15T09:15:33+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T09:15:33+07:00
- Actor: /root

- /root đã hiện thực FND-02 identity primitives, staged allocation/registry, runner exact packages + lockfile, CI nonzero/negative-control gate. Baseline local 94b0e53; feature/fnd-02-typed-identity chưa publish.
- Final runtime source sha256:ca3efcb9cf658b08212af7333b522a5eb3f9c7dba9c613ff7331462f1e501442; 32 discovered/passed, 0 failed/skipped; negative control 1 failed/exit1; build6projects 0warnings/errors. Evidence tests/results/fnd02-jy9izu9m/evidence.json.
- Self-review đã bổ sung coverage đủ10namespaces/world và lifecycle/revision; không có subagent hoặc review độc lập. Headless accepted; Unity/storage/atomic-world/replay/gameplay còn pending.
- Đồng bộ ADR009, contracts, execution/QA, backlog/matrix, sprint, handoff/readme/agent, hoststatus, changelog. Checks source36files/reference/architecture/handoff405links đạt.
- Lượt export trước từ chối source changed giữa lúc sinh; đang regenerate cuối. Build parallel có MSB4166, chuyển sequential -m:1 -nr:false đã đạt; chưa kết luận nguyên nhân host.


### EVT-00009 | SPIN-002 | RESULT — Baseline AI và FND-02 headless đạt nghiệm thu; exports đồng bộ

<!-- worklog: {"id": "EVT-00009", "spin": "SPIN-002", "event": "RESULT", "at": "2026-10-15T09:17:00+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T09:17:00+07:00
- Actor: /root

- Outcome: baseline AI và bước triển khai đầu Sprint1 hoàn tất. FND-02 headless accepted: 32/32 C# tests, 0 skipped; negative-control process 1 failed/exit1 đúng yêu cầu. Unity còn pending FND-16; Sprint1 còn FND-03/04.
- Tooling negative control bổ sung: stub runner exit0 với TRX total0 bị runtime gate từ chối exit1. Đây là tooling test, không cộng vào32runtime tests. Source fingerprint của runtime evidence vẫn khớp sau checks.
- Checks: docs36files, handoff405links/52requirements/28tasks, reference/vectors/architecture và diff đạt; hoststatus simulationImplemented=false,nextTask=FND-03. Export cuối đạt132sourceinputs,14HTML/DOCX,30titles,6156anchors,123tables/navigation. Không ZIP.
- Git local: baseline94b0e53 trên dev; feature commit257c228 trên feature/fnd-02-typed-identity. Không push, không PR/merge feature về dev, không đổi main/tag/release; CI workflow đã viết nhưng chưa có remote run. Final journal events sau commit còn local diff để đưa vào commit kế tiếp theo policy.
- Actor /root thực hiện và self-review, không có agent phụ. Không còn blocker trong scope spin. MSB4166 parallel build được tránh bằng sequential runner; không bịa nguyên nhân hoặc kết luận Unity.


### EVT-00010 | SPIN-002 | CLOSE — Đóng SPIN-002 — finished; Sprint 1 tiếp tục

<!-- worklog: {"id": "EVT-00010", "spin": "SPIN-002", "event": "CLOSE", "at": "2026-10-15T09:17:01+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T09:17:01+07:00
- Actor: /root

- Outcome: finished cho goal SPIN-002 (baseline AI + FND-02 headless). Sprint SPRINT-01 vẫn In progress; không gọi cả Foundation hoàn tất.
- Assignment /root hoàn tất, không có actor khác hoặc blocker cần chờ. Bước tiếp theo FND-03 WorldClock theo sprint, rồi FND-04 RNG/codec; ghi spin mới cho mục tiêu tiếp theo.
- Evidence chính docs/qa/FND-02-EVIDENCE.md và tests/results/fnd02-jy9izu9m/evidence.json. Branch hiện tại feature/fnd-02-typed-identity, HEAD257c228 local; WORKLOG event kết quả/close chưa commit là trạng thái chủ động theo quy tắc journal.


### EVT-00011 | SPIN-003 | OPEN — Điều phối và nghiệm thu toàn bộ Sprint 1

<!-- worklog: {"id": "EVT-00011", "spin": "SPIN-003", "event": "OPEN", "at": "2026-10-15T10:31:43+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T10:31:43+07:00
- Actor: /root

- Goal SPRINT-01/SPIN-003: hoàn tất FND-03 clock và FND-04 codec/math/RNG, regression FND-02, review + nghiệm thu sprint, cập nhật mọi consumer docs/exports.
- User giao toàn bộ điều hướng/break task/giao task/nghiệm thu, báo cáo khi Sprint1 xong. Root điều phối; subagents được giao rõ theo yêu cầu này.
- Assignment: /root/clock FND-03 Time/*.cs + TimeTests; /root/random FND-04 Random/*.cs + RandomTests; /root tích hợp runner/CI/docs, kiểm và nghiệm thu. Không có human review/playtest giả.
- Acceptance: Release build, nonzero all-pass/no-skip, negative control, clock range/calendar, mdc1/sha256 vectors, RNG preview/abort/draw schedule và exact numeric edges. Unity/storage/scheduler/gameplay ngoài scope.
- Branch tích hợp local feature/sprint-01-primitives từ dev, nhận feature FND-02 bằng ff; các task có commit riêng. Không main/tag/release; push không nằm trong bước nghiệm thu local này.


### EVT-00012 | SPIN-003 | PROGRESS — Hai task đã triển khai và review; gate 70 tests đạt

<!-- worklog: {"id": "EVT-00012", "spin": "SPIN-003", "event": "PROGRESS", "at": "2026-10-15T10:35:46+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T10:35:46+07:00
- Actor: /root

- /root/clock hoàn tất FND-03,25targetedtests pass; root review và commit c8f8449. /root/random hoàn tất FND-04,13targetedtests pass; commit79c9944. Không agent sửa docs/Git ngoài root.
- /root/review được giao review read-only riêng; không phát hiện blocker. Giới hạn: context domain/decision/occurrence/stream chỉ nonblankUTF8, caller kiểm grammar/schema; root đã ghi ADR010/OPEN01/evidence.
- Root integration Release gate70tests pass(32identity,25clock,13random),negative control1fail/exit1; mỗi task discovery >0. Source README Time đã được cập nhật sau run nên chạy lần cuối để fingerprint khớp toàn source inventory.
- Docs/source/reference/architecture checks đạt,414links; backlog FND03/04 headless accepted. TEST008scheduler vẫn pendingFND10; OPEN01Unity/performance còn pending. Không nghiệm thu fullFoundation/gameplay.
- Next: evidence fingerprint cuối, exports14HTML/DOCX, commit integration docs, close spin/sprint. Branch feature/sprint-01-primitives local; chưa publish/main/release.


### EVT-00013 | SPIN-003 | RESULT — Nghiệm thu Sprint 1 — 70 tests đạt; docs và exports đồng bộ

<!-- worklog: {"id": "EVT-00013", "spin": "SPIN-003", "event": "RESULT", "at": "2026-10-15T13:47:42+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T13:47:42+07:00
- Actor: /root

- SPRINT-01 local headless scope: COMPLETED. FND-02 identity32tests, FND-03 clock25tests, FND-04 math/codec/RNG13tests; tổng70/70 Release pass,0failed/0skipped. Negative-control process1failed/exit1 đúng yêu cầu; toàn gateexit0,build6projects0warning/error.
- Evidence cuối tests/results/sprint01-tvdcfogg/evidence.json và docs/qa/SPRINT-01-EVIDENCE.md. Source fingerprint đã đối chiếu khớp. Tooling control: stub exit0 có2nhóm pass nhưng thiếuFND04 bị gate từ chối exit1; không cộng vào game test count.
- /root/clock và /root/random hoàn tất implementation; /root/review read-only review không phát hiện blocker; /root tích hợp/review/checks/docs/nghiệm thu. Giới hạn context grammar thuộc caller đã ghi ADR010/OPEN01. Không còn agent đang làm task sprint.
- Checks cuối: generated36files, source/reference/vectors/architecture, handoff414links,52requirements,28tasks, staged diff đạt. Exports134inputs/14HTML-DOCX/30titles/6156anchors/123tables/navigation đạt. Một lượt export phát hiện source đổi khi cập nhật evidence; đã regenerate và check thành công cuối.
- Git local branch feature/sprint-01-primitives: c8f8449(clock),79c9944(RNG),081afcd(nghiệm thu/runner/CI/docs); FND02 kế thừa257c228. Chưa push, chưa PR/merge implementation vềdev, main/tag/release không đổi. Final journal events sau commit còn local theo policy.
- Giới hạn: TEST008scheduler pendingFND10; UnityFND16,worldstate/commands/storage/replay/gameplay chưa triển khai. Sprint complete không là Foundationcomplete hoặc gameplayable. Bước kế tiếp FND05; Sprint2 chưa mở.


### EVT-00014 | SPIN-003 | CLOSE — Đóng SPIN-003 và Sprint 1 — finished

<!-- worklog: {"id": "EVT-00014", "spin": "SPIN-003", "event": "CLOSE", "at": "2026-10-15T13:47:42+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T13:47:42+07:00
- Actor: /root

- Outcome finished. SPIN-003 đóng; SPRINT-01 hoàn tất scope local headless theo sprint brief và evidence.
- /root,/root/clock,/root/random,/root/review hoàn thành các assignment thực tế; không có blocker của sprint. Không còn spin active.
- Next milestone bắt đầu từ FND-05 World state/revision; chưa tự triển khai ngoài Sprint1. Báo cáo cho user theo yêu cầu khi sprint xong.
- HEAD081afcd local; chỉ WORKLOG events cuối còn chưa commit. Không commit vòng lặp chỉ để tự chứa hash.


### EVT-00015 | SPIN-004 | OPEN — Họp và triển khai Sprint 2 — command pipeline trong bộ nhớ

<!-- worklog: {"id": "EVT-00015", "spin": "SPIN-004", "event": "OPEN", "at": "2026-10-15T14:46:14+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-15T14:46:14+07:00
- Actor: /root

- User giao pushSprint1, họp/lênplan/breaktask/triểnkhai và nghiệmthu Sprint2. Scope Sprint2 FND05–08 headless/in-memory TransferResource; SQLite/eventscheduler/replay/Unity ngoài scope.
- Sprint1 pushed: origin/dev94b0e53265c148e329556125a2f4bff2649503ab (baseline), origin/feature/sprint-01-primitives0315c66f7dc1f1162babbb117ebe9e92cfa750fa; ls-remote khớp. gh auth chưa đăngnhập nên chưa PR/merge; không directpush implementation vào dev để né policy.
- Tạo feature/sprint-02-command-pipeline từdev rồi ff nhậnSprint1; nhánh phụthuộc Sprint1 chưamerge. Không đổi main/tag/release.
- Assignment /root điềuphối/API/CI/docs/QA; /root/s2_world FND05/06 WorldRevision + Domain immutable snapshot/ruleregistry/transfer + unit tests; /root/s2_pipeline FND07/08 envelope/fingerprint/receipt/atomic in-memory commit/invariant + integration tests. Reviewer riêng sẽ được giao khi code có.
- Acceptance: immutable snapshots/no writable leaks; typed approvedrules; duplicate-before-stale,mismatch/rejected/noop semantics; authority/time/revision/ref gates; conservation/ownerchecks/rollback; precommit+postcommit fault injection và exactly-once retry. RegressionSprint1, per-taskdiscovery, negativecontrol, docs/export/sourcefingerprints.


### EVT-00016 | SPIN-004 | PROGRESS — Sprint 2 đạt 124 tests và demo; chốt docs/export/push

<!-- worklog: {"id": "EVT-00016", "spin": "SPIN-004", "event": "PROGRESS", "at": "2026-10-16T08:56:38+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T08:56:38+07:00
- Actor: /root

- /root/s2_world hoàn tất FND05/06,13tests; commit72e1d12. /root/s2_pipeline hoàn tất FND07/08,41tests; commit1227a4a. /root/s2_review review đọc độc lập không phát hiện blocker; requested tests metadata/auth/nonemptyRNG/maliciousauthority đã bổ sung.
- /root tạo CLI transferdemo, nâng runner regression/per-task gate và docs/ADR011/sprintbrief/backlog/QA. Final run tests/results/sprint02-_g7_ufsp/evidence.json:124/124pass,0skip,negativecontrol1fail/exit1; build6projects0warning/error. Demo10/0→7/3,revision2,event1,receipt1,retryoriginal=true. Sourcefingerprint đã khớp sau commit.
- Checks source36files/handoff432links/52requirements/28tasks/reference/vectors/architecture/diff đạt. Export tiếp theo từ source cuối, không tạo ZIP.
- Sprint1 GitHub CI public API đã xác minh success trên0315c66: https://github.com/nguyenhaisy1/mythical_dnd/actions/runs/38700799347 . Chưa có gh API auth tạo/mergePR; dev chỉ baseline94b0e53. Sprint2 branch kế thừa Sprint1, sẽ push và xác minh riêng; main/tag/release không thay.
- Limits: in-memoryTransferResource không durability/fullcausal/scheduler/replay/Unity. RNG context alias mapping phải chốt trước stochasticcommands; transfer không draw. Không activeblocker cho scope codeSprint2; PRintegration còn thiếuGitHubauth.


### EVT-00017 | SPIN-004 | GIT — Sprint 2 đã push và CI GitHub success đúng SHA

<!-- worklog: {"id": "EVT-00017", "spin": "SPIN-004", "event": "GIT", "at": "2026-10-16T08:58:48+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T08:58:48+07:00
- Actor: /root

- Sprint2 implementation commits72e1d12(world/rules),1227a4a(command/demo),acceptance9e3d761.
- Đã push origin/feature/sprint-02-command-pipeline; git ls-remote SHA9e3d7616ecd16b8701acd350fdf794717668ef5c khớp localHEAD. CI public API xác minh completed/success trên đúngSHA: https://github.com/nguyenhaisy1/mythical_dnd/actions/runs/38837884070 .
- Sprint1 origin/feature/sprint-01-primitives0315c66 cũng đãpush/CIpass; origin/dev chỉ baseline94b0e53. gh auth status kiểm lại16/10 vẫn chưa đăngnhập. Không tạo PR/merge hoặc directpush implementation vào dev để né policy. main/tag/release không đổi.
- Các event journal ghi saucommit/push vẫn local, đưa vào commit kế tiếp theo policy, không amend hoặc commit vòng lặp để tự ghiSHA.


### EVT-00018 | SPIN-004 | RESULT — Nghiệm thu Sprint 2 — 124 tests, demo và remote CI đạt

<!-- worklog: {"id": "EVT-00018", "spin": "SPIN-004", "event": "RESULT", "at": "2026-10-16T08:58:48+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T08:58:48+07:00
- Actor: /root

- Outcome Sprint2 scope headless/in-memory TransferResource đạt. Đã planning/breaktask/giao2agent implementation/reviewagent riêng, root tích hợp và nghiệmthu. S2-01–06 hoàn tất theo sprintbrief; FND05–08 được ghi accepted đúng kernel/transfer subset.
- Runtime124/124tests pass(70Sprint1regression,13worldrule,41pipeline),0skip,negativecontrol1fail/exit1 đúngyêucầu. Releasebuild6projects0warnings/errors; demo10/0→7/3,revision2,event1,acceptedreceipt1,retryoriginal=true. Final evidence tests/results/sprint02-_g7_ufsp/evidence.json,sourcefingerprint khớp.
- /root/s2_world,/root/s2_pipeline hoàn tất vùngcode; /root/s2_review read-only source/tests review không cònblocker. Root có reviewdiff và chạy finalgate. Reviewer requestedcases authority/metadata/nonemptyRNG/malicioushandler đã có.
- Docs/ADR011/sprintbrief/architecture/baselinehandoff/backlog/testmatrix/README/changelog/runner/CI đồngbộ. Checks nguồn36files,handoff432links/52req/28tasks,reference/vectors/architecture/diff đạt. Exportcheck137sourceinputs,14HTML-DOCX,30titles,6156anchors,123tables/navigation đạt; khôngZIP.
- Git đãpush9e3d761+CIpass theoEVT Git ởtrên. PRintegration cònthiếuGitHubAPIauth; khôngđổi main/release. Đây khôngphải blocker của runtime scope nhưng còn bàn giaoGit cần hoàn tất khi cóauth.
- Limits: khôngdurability/processcrash/SQLite/fullcausaljournal/scheduler/replay/Unity hoặc gameplayplaytest. RNGstreams chỉ bảo toàn trongtransfer; contextalias mapping phải chốt trước stochasticcommands. Next FND09event/causal rồi FND10scheduler; Sprint3 chưa mở.


### EVT-00019 | SPIN-004 | CLOSE — Đóng SPIN-004 và Sprint 2 — finished

<!-- worklog: {"id": "EVT-00019", "spin": "SPIN-004", "event": "CLOSE", "at": "2026-10-16T08:58:48+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T08:58:48+07:00
- Actor: /root

- SPIN-004 finished; Sprint2 completed trong phạmvi in-memory đãchốt. Các actor /root,/root/s2_world,/root/s2_pipeline,/root/s2_review hoàn tất assignment, khôngcòn runtimeblocker hoặc task triểnkhaiđangchạy của sprint.
- Báocáo chính docs/qa/SPRINT-02-EVIDENCE.md; kếhoạch/breaktask docs/production/sprints/SPRINT-02.md. Branch feature/sprint-02-command-pipeline remoteHEAD9e3d761; chỉ journal finalevents chưa commit.
- Bước kế tiếp FND09/fullcausal events và FND10scheduler; báo kếtquả cho user, không tự mởSprint3. PRmerge vềdev chưa thực hiện do GitHubCLI chưa đăngnhập.


### EVT-00020 | SPIN-005 | OPEN — Thiết lập GitHub MCP cho Codex

<!-- worklog: {"id": "EVT-00020", "spin": "SPIN-005", "event": "OPEN", "at": "2026-10-16T09:18:34+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T09:18:34+07:00
- Actor: /root

- Goal: kết nối GitHub MCP theo yêu cầu user trước sprint kế tiếp. /root kiểm plugin state và Codex MCP CLI; GitHub plugin hiện installed=false, MCP list ban đầu chưa có server.
- Đã đối chiếu official GitHub install-codex guide và OpenAI MCP docs. Đã chạy codex mcp add github --url https://api.githubcopilot.com/mcp/ --bearer-token-env-var GITHUB_PAT_TOKEN; CLI xác nhận added global server.
- Verification sanitized: namegithub,enabledtrue,transport remote official URL,auth_statusbearer_token. GITHUB_PAT_TOKEN chưa có trong process hiện tại, chưa handshake/tool discovery hoặc authenticated repository access. Auth_status là loại cơ chế cấu hình, không bằng chứng đăng nhập thành công.
- Token không được đưa vào chat/config/repository/log. User cần tạo PAT phù hợp repository và cung cấp qua environment của process Codex mới; GitHub CLI auth là session riêng, không tự coi đã đăng nhập gh.
- Scope chỉ môi trường cá nhân và journal; không sửa runtime/game docs, không merge/push/tag/release. Phần cấu hình hoàn tất; phần xác thực/tool check chờ credential từ user ở máy cục bộ.


### EVT-00021 | SPIN-005 | RESULT — GitHub MCP đã cấu hình; xác thực chưa hoàn tất

<!-- worklog: {"id": "EVT-00021", "spin": "SPIN-005", "event": "RESULT", "at": "2026-10-16T09:18:34+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T09:18:34+07:00
- Actor: /root

- Goal: kết nối GitHub MCP theo yêu cầu user trước sprint kế tiếp. /root kiểm plugin state và Codex MCP CLI; GitHub plugin hiện installed=false, MCP list ban đầu chưa có server.
- Đã đối chiếu official GitHub install-codex guide và OpenAI MCP docs. Đã chạy codex mcp add github --url https://api.githubcopilot.com/mcp/ --bearer-token-env-var GITHUB_PAT_TOKEN; CLI xác nhận added global server.
- Verification sanitized: namegithub,enabledtrue,transport remote official URL,auth_statusbearer_token. GITHUB_PAT_TOKEN chưa có trong process hiện tại, chưa handshake/tool discovery hoặc authenticated repository access. Auth_status là loại cơ chế cấu hình, không bằng chứng đăng nhập thành công.
- Token không được đưa vào chat/config/repository/log. User cần tạo PAT phù hợp repository và cung cấp qua environment của process Codex mới; GitHub CLI auth là session riêng, không tự coi đã đăng nhập gh.
- Scope chỉ môi trường cá nhân và journal; không sửa runtime/game docs, không merge/push/tag/release. Phần cấu hình hoàn tất; phần xác thực/tool check chờ credential từ user ở máy cục bộ.


### EVT-00022 | SPIN-005 | RESULT — GitHub MCP cài xong và xác thực read-tool thành công

<!-- worklog: {"id": "EVT-00022", "spin": "SPIN-005", "event": "RESULT", "at": "2026-10-16T10:20:18+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T10:20:18+07:00
- Actor: /root

- User cung cấp credential và yêu cầu tự hoàn tất cài GitHub MCP. /root xác thực API được tài khoản nguyenhaisy1 và quyền truy cập repository mythical_dnd; không ghi token vào journal/tool output/repository.
- Cài official github/github-mcp-server v2.0.2 native Linux x86_64 ngoài repository; SHA256 archive đối chiếu release metadata. Binary ~/.local/share/mythical-tools/github-mcp-server/2.0.2/github-mcp-server.
- Credential lưu riêng ~/.config/mythical_dnd/credentials/github-token mode0600, directory0700. Launcher ~/.local/share/mythical-tools/github-mcp-launch.sh đọc credential vào env riêng của process server; config Codex chỉ chứa launcher path, không chứa token.
- Đổi github MCP entry từ remote/env-only chưa có credential sang official local stdio server, toolsets context,repos,pull_requests,actions. codex mcp get xác minh enabledtrue và đúng launcher.
- Verification thật bằng MCP initialize/notifications/tools/list/tools/call: servergithub-mcp-server2.0.2,37tools,get_me authenticatedOK,get_file_contents README.md trên nguyenhaisy1/mythical_dnd mainOK. create/mergePR tools hiện diện; chưa kiểm write bằng mutation và không tạo/mergePR.
- Token đã xuất hiện trong chat: đã khuyến nghị thu hồi/thay mới; có helper update-github-token.py dùng getpass/validate account/atomic0600save để user thay kín tại Terminal. Không coi file0600 là encrypted secret vault. Không dùng tool command-logging.
- Session Codex hiện tại chưa hot-load toolset mới; mở lại Codex/phiên mới để nạp github MCP. CLI/server đã handshake xác minh, không cần token env ngoài mỗi lần chạy. gh CLI auth là riêng, không tự tuyên bố gh đã login.


### EVT-00023 | SPIN-005 | CLOSE — Đóng SPIN-005 — GitHub MCP setup finished

<!-- worklog: {"id": "EVT-00023", "spin": "SPIN-005", "event": "CLOSE", "at": "2026-10-16T10:20:18+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T10:20:18+07:00
- Actor: /root

- Outcome finished cho cài/cấu hình/xác thực GitHub MCP trên máy. /root hoàn tất không có subagent. MCP test thành công thực tế37tools và readrepo.
- User cần mở lại Codex để session nạp cấu hình mới; cần rotate token đã gửi trong chat qua helper nhập kín. Không còn thiếu credential ở server hiện tại.
- Không thao tác PR/merge/Gitpush hoặc sprint runtime trong spin này. WORKLOG events mới còn local theo journalpolicy, sẽ đưa vào commit phù hợp tiếp theo.


### EVT-00024 | SPIN-006 | OPEN — Mở Sprint 3 — committed events và causal trace

<!-- worklog: {"id": "EVT-00024", "spin": "SPIN-006", "event": "OPEN", "at": "2026-10-16T10:26:21+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T10:26:21+07:00
- Actor: /root

- User giao sprint mới với workflow tự điều phối/triển khai/nghiệm thu/báo cáo. Sprint3 scope FND-09 committed event envelope, causal trace, ordering, idempotent in-memory consumers; FND-10 scheduler dành sprint sau. Không nghiệm thu SQLite/Unity/replay.
- /root đã kiểm evidence Sprint1/2, CI đúng head và tạo/merge PR#1, PR#2 vào dev bằng GitHub API. Dùng merge commit giữ ancestry Sprint2; dev 190d8d9. Không sửa main/tag/release.
- Branch mới feature/sprint-03-causal-events từ dev đã tích hợp. Giữ nguyên WORKLOG local của sprint/MCP trước và mang sang branch mới có chủ đích để commit trong bàn giao.
- /root điều phối/spec/docs/integration; sẽ giao implementer events và reviewer độc lập, ghi assignment sau khi spawn.


### EVT-00025 | SPIN-006 | ASSIGN — Giao FND09 và chốt phạm vi Sprint3

<!-- worklog: {"id": "EVT-00025", "spin": "SPIN-006", "event": "ASSIGN", "at": "2026-10-16T10:27:54+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T10:27:54+07:00
- Actor: /root

- /root/s3_events triển khai FND09 trong Application pipeline/events và EventTests; không sửa docs/Git. /root chốt ADR012, sprint brief, runner/docs/Git. Review độc lập sẽ giao khi code sẵn sàng.
- PR#1 merge aa3326e; PR#2 merge190d8d9. Branch mới tạo trực tiếp từ origin/dev sau khi switch local dev bị chặn do WORKLOG dirty; không stash/reset, WORKLOG được bảo toàn. Đây là xác nhận trạng thái branch sau OPEN.
- Scope consumer minh chứng projection/checkpoint in-memory; không nhận scheduler/reward/rumor chưa implemented là nghiệm thu đầy đủ.


### EVT-00026 | SPIN-006 | RESULT — Sprint3 runtime và review đạt — 138 tests

<!-- worklog: {"id": "EVT-00026", "spin": "SPIN-006", "event": "RESULT", "at": "2026-10-16T12:14:41+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T12:14:41+07:00
- Actor: /root

- /root/s3_events hoàn tất FND09 event-v1/causal journal + idempotent immutable projection/checkpoint consumer. /root/s3_review independent source review phát hiện cross-world guards, đã sửa; thêm assertions old duplicate/schema/payload. Final review không còn blocker; reviewer không tự chạy suite riêng.
- /root tích hợp CLI demo/runner/CI, ADR012/sprintbrief/backlog/matrix/handoff/README/agent/changelog. Full gate cuối tests/results/sprint03-dy8e1032/evidence.json:138/138passed (124regression+14event),0skip; negativecontrol1failed/exit1; Releasebuild6projects0warnings/errors. Fingerprint sha256:5d9cf70e6f003f7b06dfe3c927a0ca4917253a12b0e0c8cba2c3828b51a4ff08 khớp source hiện tại.
- CLI10/0→7/3,1event/receipt, retryoriginal=true; duplicate consumer delivery projection3/checkpoint1/duplicateApplied=false. Consumer collision/order/cross-world/tick/reentry/failure/retry/immutability có tests.
- Docs source36files/handoff442links/52requirements/28tasks; reference/vectors/architecture/diff đạt. Export đang regenerate từ nguồn cuối, khôngZIP. Credential scan versionablefiles sạch, không ghi secret.
- Scope accepted in-memory FND09 transfer/diagnostic consumer. Không durable delivery/processcrash, scheduler/fanout/cycle, gameplayreward/rumor, replay/Unity. Next FND10; chưa mởSprint4.


### EVT-00027 | SPIN-006 | PROGRESS — Hoàn tất docs/export và chuẩn bị tích hợp Git

<!-- worklog: {"id": "EVT-00027", "spin": "SPIN-006", "event": "PROGRESS", "at": "2026-10-16T12:15:32+07:00", "actor": "/root"} -->
- Ghi lúc: 2026-10-16T12:15:32+07:00
- Actor: /root

- Export cuối đã tái tạo7HTML+7DOCX; checker140sourceinputs,14files,30titles,6156anchors,123tables/navigationvalid. KhôngPDF/ZIP; artifactsignored. Không phải visual/gameplay acceptance.
- Sources runtime fingerprint vẫn khớp evidence cuối. Scope diff đãreview; chuẩn bị commit/push feature/sprint-03-causal-events và PR vào dev. Main/tag/release không thay.
