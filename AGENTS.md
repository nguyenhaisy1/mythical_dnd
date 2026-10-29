# Làm việc trong repository mythical_dnd

Đọc `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` trước. Giao tiếp và tài liệu ưu tiên tiếng Việt; code identifiers theo tiếng Anh ổn định.

- Tài liệu chi tiết nằm ở `docs/gdd/modules/`, nguồn bất biến ở `docs/source/`. Không thay nghiên cứu dài bằng summary hoặc sửa archive để khớp thiết kế mới.
- Các module generated: sửa `tools/docs/integration-notes.json` cho ghi chú áp dụng và thêm ADR/source amendment khi đổi thiết kế. Chạy build_docs và kiểm tra lại, không sửa Master export đơn lẻ.
- Random character, no runtime LLM, deterministic/causal rules, knowledge isolation và owner/commit boundaries là nền bắt buộc.
- Chỉ triển khai task được yêu cầu, có requirement, input/output và acceptance rõ. Khi thiếu đặc tả, ghi design gap/ADR; không tự chuyển unknown thành engine behavior.
- Domain/Foundation không phụ thuộc Unity, DB/network hoặc wall-clock/global random để quyết định gameplay. UI/narrative không ghi authoritative state.
- Phân biệt documentation check, scaffold build và runtime acceptance. Không đánh dấu test game Passed vì chưa có implementation; không bịa benchmark hoặc Unity verification.
- Các lệnh validation và build ở README. Run check phù hợp thay đổi; cập nhật evidence nếu kết quả hoặc phạm vi thay đổi.
- Gói export là build artifact; sửa source documents rồi regenerate. Không commit bin/obj, Unity Library/Temp hoặc SDK tạm.

Yêu cầu hiện tại đã cho phép init cấu trúc project và hoàn thiện tài liệu; giới hạn “chỉ nhập .agent” của phiên trước đã được thay thế. Không tự triển khai toàn bộ gameplay từ một yêu cầu chuẩn bị tài liệu/scaffold.

## Skills của project

Source tại `tools/skills/`, hướng dẫn và cách dùng ở [PROJECT-SKILLS](docs/runbooks/PROJECT-SKILLS.md). Chọn skill đúng nhiệm vụ, không nạp tất cả. Skill không thay baseline/contract và không cấp quyền triển khai ngoài yêu cầu.

## Git, commit/push và release

Đọc `docs/runbooks/GIT-WORKFLOW.md` trước thao tác Git; đây là nguồn chuẩn cho branch/version/changelog.

- `main` giữ mốc ổn định; `dev` tích hợp; feature/fix/docs/chore/spike tách từ dev. Tên feature: `feature/<task-id>-<slug>`. Không làm feature trực tiếp trên main/dev.
- Feature → PR vào dev; release/X.Y.Z từ dev → main; hotfix từ tag stable → main. Đồng bộ release/hotfix trở lại dev, và release đang mở nếu có.
- Commit dạng `type(scope): mô tả kết quả`, một thay đổi logic, kèm task/test khi có. Stage đúng scope; đọc diff và chạy checks phù hợp trước commit. Không commit secrets, caches, generated artifacts bị ignore hoặc ZIP.
- Commit/push khi đã có chỉ dẫn cho phạm vi đang làm; không hỏi lại quyền đã có. Yêu cầu viết/sửa tài liệu không tự là lệnh publish. Push branch đích rõ, không force push/all branches/all tags; kiểm remote SHA sau push.
- Merge main, tạo/push tag và publish release là bước riêng, chỉ thực hiện khi được giao. Không sửa lịch sử đã chia sẻ hoặc di chuyển tag release.
- VERSION là product version, CHANGELOG.md có Unreleased và từng release dated; tags vX.Y.Z (candidate có suffix). GDD/schema/content versions độc lập. Ghi compatibility, evidence, limitations; không bịa release date hoặc gameplay readiness.
- Branch protection là cấu hình GitHub riêng; không tuyên bố đã bật chỉ vì có hướng dẫn. Giữ thay đổi người dùng, không reset/clean/stash âm thầm.

## Nhật ký công việc theo spin

`WORKLOG.md` ở root là nguồn báo cáo công việc, chỉ append. Khi bắt đầu/tiếp tục công việc, đọc spin đang mở và các events gần nhất; xác định goal, scope, assignment và trạng thái thực tế trước khi sửa. Mục tiêu mới mở SPIN kế tiếp; cùng mục tiêu tiếp tục spin cũ. Không sửa/xóa lịch sử; đính chính bằng CORRECTION dẫn event cũ.

Ghi OPEN/ASSIGN khi nhận việc; PROGRESS/RESULT cho kết quả đáng kể và checks thật; BLOCKED/PAUSED/RESUME/HANDOFF khi trạng thái đổi; GIT cho commit/push/merge/release; CLOSE khi kết thúc với finished/partial/cancelled, evidence và bước tiếp theo. Ghi actor/task thật, không gọi skill là nhân sự hoặc bịa agent/team chưa chạy. Không cần log từng tool call, không ghi secrets.

Dùng `python3 tools/worklog.py --new-spin --event OPEN --actor /root --summary "..." --details-file /path/to/details.md` hoặc `--spin SPIN-NNN` cho event tiếp theo. Helper chỉ khóa file/cấp ID/timestamp/append, không tự giám sát thay đổi hoặc thay backlog. Agent điều phối ghi kết quả của cộng tác viên sau khi có bằng chứng; không tự mở multi-agent vì log có cột phân công.

Khi user hỏi báo cáo, tổng hợp goal, spin, ai đang/đã làm gì, kết quả, blockers, evidence và Git state từ log; đối chiếu trạng thái hiện tại khi cần. Không append một báo cáo trùng dài nếu không có thông tin mới. Log local chưa commit phải nói rõ; log commit/push đưa vào commit kế tiếp, không amend hoặc commit vô hạn để tự chứa hash. WORKLOG không nhúng vào exports, tránh vòng lặp ghi log làm stale export.

## Thực hiện bằng AI — 2026-10-15

Chủ dự án xác nhận toàn bộ phát triển do AI thực hiện. Theo `docs/runbooks/AI-EXECUTION.md`: role backlog là trách nhiệm của AI, không chờ nhân sự con người; tự implement/test/review trong scope đã giao, ghi actor và evidence thật. Self-review không là independent review; automated playthrough không là human playtest. Sprint hiện tại: `docs/production/sprints/SPRINT-01.md`; yêu cầu hiện tại đã cho phép bắt đầu nền tảng, thay giới hạn chỉ tài liệu trước đây.

Chủ dự án đã giao AI điều phối, chia/giao task và nghiệm thu toàn bộ Sprint 1 (2026-10-15). Được dùng subagent cho các task đã chia trong phạm vi sprint; ghi actor, ownership, review và evidence thật. Không mở sprint mới hoặc phát hành chỉ vì Sprint 1 đã hoàn tất.

## Sprint 2 được giao — 2026-10-15

Yêu cầu mới đã cho phép push Sprint1 và tự điều phối planning/breaktask/giao agent/triển khai/nghiệmthu Sprint2. Sprint hiện tại `docs/production/sprints/SPRINT-02.md`, scope FND05–08 in-memory command path; không tự nghiệmthu SQLite/Unity hoặc release. Sprint1 đã push origin/feature/sprint-01-primitives; PR/merge cần GitHub API auth, không bypass branch policy.

## Sprint 3 được giao — 2026-10-16

User giao sprint mới với cùng workflow điều phối/chia task/subagents/triển khai/nghiệm thu/báo cáo. Sprint hiện hành [Sprint3](docs/production/sprints/SPRINT-03.md), SPIN-006, scope FND-09 event/causal model và consumer idempotency in-memory; FND-10 scheduler dành sprint tiếp theo. Sprint1/2 đã tích hợp dev qua PR#1/#2; GitHub API auth đã kiểm chứng. Không tự mở Sprint4 hoặc release.

## Workflow sprint thường trực — 2026-10-16

Chủ dự án xác nhận không cần nhắc lại workflow. Mỗi khi giao một sprint, AI tự đọc bàn giao/baseline, chốt phạm vi và acceptance, lập kế hoạch/break task, giao subagents với ownership rõ, triển khai, review độc lập, chạy checks, sửa lỗi, đồng bộ tài liệu/exports/changelog/worklog, commit/push feature branch, tạo PR và tích hợp vào dev sau khi review/CI đạt, rồi báo cáo kết thúc. Đây là ủy quyền cho sprint được giao; không tự mở sprint kế tiếp, merge main, tạo tag hoặc release. Không xin lại quyền routine trong phạm vi này.

Sprint4 hiện hành: docs/production/sprints/SPRINT-04.md, SPIN-007, FND10 scheduler/AdvanceTime in-memory. Durability FND11 và gameplay repair/health/legacy vẫn có acceptance riêng; không đánh dấu complete vì có mock hoặc fixture.

## Sprint5 — 2026-10-19

User giao Sprint5 theo workflow thường trực. Scope FND11 headless SQLite current-state commit/reopen, ADR014; periodic snapshot/recovery/replay và Unity provider còn gates riêng. SPIN008; docs/production/sprints/SPRINT-05.md và docs/qa/SPRINT-05-EVIDENCE.md ghi actor/evidence thực, gồm việc agents quota và root tiếp quản. NextFND12, không tự mởSprint6.

## Sprint6 và định dạng báo cáo — 2026-10-20

User giao Sprint6 theo workflow thường trực; SPIN-009, docs/production/sprints/SPRINT-06.md, FND12 headless snapshot/recovery/migration theo ADR015. Không tự mởSprint7 hoặc release.

Từ nay báo cáo sprint phải bắt đầu bằng phần tổng hợp tiếng Việt dễ hiểu: sprint làm những việc gì, kết quả giúp ích gì và phần còn thiếu. Sau đó mới trình bày chi tiết kỹ thuật, phân công, kiểm thử/evidence, blockers và Git. Không dùng thuật ngữ kỹ thuật làm phần tổng hợp khó hiểu.

## Sprint7 — 2026-10-20

User giao tiếp tục sprint sau Sprint6. Sprint7/SPIN-010, FND13 headless command replay theo ADR016 và docs/production/sprints/SPRINT-07.md. Rerun Execute/Drain từ initial image, không đánh đồng đọc events/recovery images với replay. Không tự mởSprint8 hoặc release; giữ báo cáo tổng hợp tiếng Việt dễ hiểu trước chi tiết.

## Sprint 8 — 2026-10-21, tiếp tục 2026-10-22

User giao sprint tiếp theo theo workflow thường trực. Sprint8/SPIN-011, FND-14 runtime content registry theo ADR-017 và docs/production/sprints/SPRINT-08.md. Profile commodity/transfer-template, strict authoring parser, approved handlers và exact content compatibility. Full VS-02 roster/Unity còn riêng; không tự mở Sprint9 hoặc release. Báo cáo vẫn tổng hợp tiếng Việt dễ hiểu trước chi tiết.

## Sprint9 — 2026-10-22

User giao sprint mới theo workflow thường trực. Sprint9/SPIN-012, FND15 diagnostics theo ADR018 và docs/production/sprints/SPRINT-09.md. Actual rule checks, full committed field diff, causal/job inspectors và fixed evidence reproduction; chỉ developer QA, không player projection. VS01 cần milestone review riêng; Foundation complete còn FND16 Unity. Không tự mởSprint10 hoặc release; giữ báo cáo tiếng Việt dễ hiểu trước chi tiết.

## Sprint10 — 2026-10-22

User giao Sprint10/SPIN-013: VS01 review và FND16 conformance preparation. Xem docs/production/sprints/SPRINT-10.md. Chủ dự án sau đó đã cài Unity6000.6.7f1. Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. Không mở Sprint11 hoặc release.

## Sprint11 — 2026-10-23

User giao sprint tiếp theo/SPIN014. Scope VS02A settlement-content-v1 theo ADR020 và docs/production/sprints/SPRINT-11.md:24actors/6households/3local+externalBếnHạc/8routes, immutable authoring registry và strict compiler. Không WorldState/routine execution/randomprotagonist hoặc fullVS02eventeligibility. Không tự mởSprint12/release; fullVS02 cònpartial.

## Sprint12 — 2026-10-26

User giao sprint tiếp theo và yêu cầu mốc hình ảnh/demo. Sprint12/SPIN-015: VS02B event-eligibility-v1 theo ADR021 và docs/production/sprints/SPRINT-12.md. Tám template và evaluator trên trusted observations; không event effects hoặc fullVS02. Forecast mới tại docs/production/PLAYABLE-ROADMAP.md: Sprint13 định hướng hình ảnh, Sprint15–16 demo nhỏ, phụ thuộc gates; không tự mởSprint13/main/tag/release.

## Sprint13 — thử nghiệm và nghiệm thu bởi chủ dự án

User giao Sprint13 ngày26/10/2026. Cơ cấu6vai trò:3worker,2critic độc lập và/root điều phối/kiểm tra lớp đầu. Runtime chỉ4concurrent nên luân phiên worker/critic; log actor thực, không claim6chạyđồngthời.

**Riêng Sprint13: mọi sản phẩm Pending user approval cho đến khi chủ dự án chốt rõ.** Root/test/critic pass chỉ Technical reviewed, không Final accepted. Không tựmerge dev, đóngSprint13 hoặc chuyểnSprint14 khi còn mục chưa chốt. Khi user yêu cầu sangsprintmới, nhắc đầy đủ pending từ docs/production/sprints/sprint-13/APPROVALS.md và chờchốt; “tiếp tục” không là approval. Mỗi thử nghiệm/phươngán mới mở SPIN mới trongSprint13, giữ lịch sử và sản phẩm cũ; sửa cùngphươngán tiếp tụcspin. Featurecommit/push/draftPR cho review vẫn theo workflow, không main/tag/release.

Thử nghiệm01/SPIN016 tại prototypes/sprint13/experiment-01; độc lập production. Sprintbrief docs/production/sprints/SPRINT-13.md.

## Tạo mô hình3D — chỉ dẫn chủ dự án28/10/2026

Từ lần tạo mô hình3D tiếp theo, dùng MCP Hyper3D đã cài làm đường tạo model. Blender/engine có thể dùng cho bố trí cảnh, camera, kiểm tra và render sau đó. Nếu MCP chưa hiện hoặc không truy cập được, ghi rõ tình trạng và khôi phục kết nối; không âm thầm thay bằng procedural mesh hoặc công cụ sinh model khác. Chủ dự án có thể chỉ định ngoại lệ rõ ràng. Experiment04/SPIN020 đang dựng bằng Blender trước chỉ dẫn này được tiếp tục hoàn thiện theo yêu cầu user, phải ghi đúng nguồn; không gán là modelHyper3D.

## Xem trước ý tưởng bằng ảnh — chỉ dẫn chủ dự án 29/10/2026

Ở giai đoạn chọn ý tưởng hiện tại, tạo **ảnh mô phỏng game**, có thể là tranh 2D hoặc ảnh mang diện mạo 3D; không tạo model 3D thật chỉ để trình bày ý tưởng. Chỉ dẫn này thay cách triển khai bằng model ở Experiment 05, không yêu cầu xử lý tiếp lượt sửa Hyper3D đang chưa xác định ID. Dùng ảnh concept để chủ dự án xem bối cảnh, góc nhìn, phong cách và giao diện minh họa trước; ghi rõ ảnh không phải game đang chạy hoặc render từ một scene 3D đã dựng. Không tự gọi Hyper3D/Blender hay xây pipeline 3D cho yêu cầu thử hình ảnh.

Quy tắc 28/10 về MCP Hyper3D chỉ áp dụng khi chủ dự án giao lại task **tạo model 3D thật** trong tương lai; không buộc dùng Hyper3D cho ảnh có vẻ 3D. Giữ các thử nghiệm cũ và nguồn gốc thật. Experiment 06/SPIN-022 dùng ảnh mô phỏng và gallery để so sánh; mọi phương án Sprint 13 vẫn Pending user approval. Việc đổi cách xem trước không phải duyệt mỹ thuật hoặc cho phép chuyển Sprint 14.

## Thư viện phiên bản tham khảo — làm rõ ngày 29/10/2026

Mỗi spin/thử nghiệm hình ảnh bổ sung một phiên bản để chủ dự án tham khảo. Mở vòng mới không có nghĩa phiên bản cũ bị bỏ, bị thay thế hoặc bị đánh giá kém; giữ artwork, nguồn và hồ sơ cũ để xem lại, so sánh hoặc kết hợp ý tưởng. Chỉ ghi loại/thay thế/chọn khi chủ dự án nói rõ. Nhận xét và findings phải gắn đúng phiên bản, artifact và phạm vi đã quan sát; một lỗi cụ thể không tự là kết luận toàn bộ ý tưởng dở. Giữ nguyên phản hồi lịch sử thật, gồm phản hồi đồ họa bản01 chưa ưng ý. Lời khen tiến triển hoặc yêu cầu nghĩ thêm không là final approval cho một hướng hay Sprint13.

## Định hướng hình ảnh xuyên suốt game — 29/10/2026

Khi nhiệm vụ là đề xuất ngôn ngữ mỹ thuật cho toàn game, mỗi hướng cần cho thấy sự thống nhất giữa thế giới, tạo hình nhân vật toàn thân/chân dung, vật phẩm/icon và giao diện như hội thoại, túi đồ, nhật ký. Không chỉ đổi phong cảnh rồi suy ra cả game đã có bản sắc. Có thể dùng một bảng concept gồm cảnh chơi và mẫu nhân vật/UI để xem trước; yêu cầu này theo scope định hướng tổng thể, không buộc mọi batch ảnh nhỏ phải làm lại toàn bộ thành phần. UI raster trong ảnh là minh họa, cần kiểm chữ và độ đọc nhưng không là UI runtime hoặc bằng chứng tương tác. Giữ baseline xuất thân ngẫu nhiên, không diễn giải màn hình nhân vật thành cho phép chọn/chỉnh origin. Các phiên bản tiếp tục bổ sung thư viện, chưa thay quyền chốt của chủ dự án.

## Số lượng concept linh hoạt — 29/10/2026

Chủ dự án giao AI tự quyết số lượng concept theo độ khác biệt và giá trị của ý tưởng; không cố định mỗi vòng ba phương án A/B/C. Chọn phạm vi hữu ích cho yêu cầu đang làm, có thể đào sâu ít hướng hoặc mở thêm nhiều hướng; đây không phải yêu cầu tạo vô hạn hoặc tự mở sprint mới. Tiếp tục thể hiện ngôn ngữ xuyên cảnh, nhân vật và UI khi scope là định hướng toàn game; mọi phiên bản vẫn bổ sung thư viện tham khảo và cần quyết định rõ của chủ dự án để được chốt.
