# Bản đồ tài liệu

## Sprint17 — đan dây cá nhân từ xuất thân — nghiệm thu kỹ thuật local10/11/2026

Kết quả kỹ thuật local10/11:770/770runtime tests,0failed/skipped; actual Unity6000.6.7f1/LinuxMono ba xuất thân/116ảnh mới và31ảnh regression15–16;48native actions/hai process SaveRunning→restart→complete một lần. Hai final reviews độc lập không còn P0/P1 trong scope. Docs/exports và exactHEADCI/PR→dev phải đạt trước CLOSE; Git sau commit theo WORKLOG.

Chủ dự án giao [Sprint17](production/sprints/SPRINT-17.md) ngày10/11/2026, SPIN-041, sau Sprint16 đã đóng qua PR17/dev`e7dfd9b`. Nhân vật đã biết đan có thể xem nguyên liệu cá nhân, xác nhận làm dây, chờ nhận sản phẩm hoặc hủy; lưu khi đang làm rồi mở ứng dụng lại phải giữ đúng tiến trình và không nhân đôi đồ. [ADR026](adr/026-personal-weaving-on-origin.md), [contract](contracts/PERSONAL-WEAVING-PROFILE.md), [runbook](runbooks/PERSONAL-WEAVING.md) và [evidence](qa/SPRINT-17-EVIDENCE.md) khóa phạm vi và kết quả theo revision.

`personal-weaving-v1` dùng tay,2fiber/30ticks/1cord, yêu cầu weaving40; Vạc Lâm43 và Bà Nhu61 đủ điều kiện. Tám xuất thân vẫn được chọn ngẫu nhiên như16; sáu người khác được ghi rõ bản thử chưa hỗ trợ công việc phù hợp, không grant nghề/quyền giả hoặc ép reroll. Mỗi campaign17 mới có8fiber cá nhân của bản thử, không lấy kho hộ hoặc diễn giải thành tài sản lịch sử. Đây subset chế tác trên origin, chưa hoàn tất nghề/ba nghề VS03 hoặc fullVS02/VS07.

Origin+Weaving dùng canonical image4, exact rule/content và save root riêng; không tự nâng cấp/enrich save16. Legacy image1/2/3, scenes15/16, goldens/archive/01–18 và G13`earth-monument-2d-v1` giữ. Replay artifactv1 vẫn từ chối origin/weaving; save/reopen không là command replay. Cổng đóng gồm core/history/rights, SQLite crash-recovery/idempotency, actual Unity6000.6.7f1/LinuxMono cho Lâm/Nhu/nonweaver cùng actual Player regression15–16, renderer/sixlayouts/nativeOS/actual restart, independent reviews/fullsuite/docs/exports/exactHEADCI/PR→dev. Source/focused tests không thay actual acceptance; chỉ CLOSE khi đủ cả năm gate, xem evidence và WORKLOG. Không tự Sprint18/main/tag/release.

Các mục Sprint16 và trước bên dưới là lịch sử tại thời điểm viết; CLOSE Sprint16 đã ghi ở WORKLOG ngày09/11, không còn gate16 treo. Phần còn thiếu của Sprint17 phải báo riêng với backlog toàn dự án.

## Sprint16 — nghiệm thu kỹ thuật local09/11/2026: xuất thân ngẫu nhiên từ cohort

[Sprint16](production/sprints/SPRINT-16.md), [ADR025](adr/025-seeded-origin-cohort-and-reveal.md) và [origin contract](contracts/ORIGIN-COHORT-PROFILE.md) bổ sung New campaign → Generating/cancel → Reveal → World → chờ/lưu/Continue. Hệ thống chọn theo seed một trong tám người trưởng thành đã biên soạn tại Bến Vạc; giữ cùng quá khứ, kỹ năng và nhận định của bản thân khi mở lại. Đây là nhánh chọn nhân vật có sẵn theo M07, chưa phải bộ sinh lịch sử cuộc đời hoặc full Character Viability/TEST-001.

[Runbook origin](runbooks/ORIGIN-CLIENT.md) hướng dẫn build và kiểm tra. Runtime/Unity/OS và regression local đã đạt09/11:674/674tests, hai seed,38origin captures/sixlayouts, actual Player restart và repair15 regression. Docs/exports/Git/CI phải hoàn tất trước CLOSE; [evidence Sprint16](qa/SPRINT-16-EVIDENCE.md) là nguồn kết quả theo revision cuối, không coi source hoặc focused tests là nghiệm thu toàn sprint. World này chỉ có bản thân/chờ/lưu; cohort không có nghề sửa thuyền, không được cấp nghề hoặc quyền giả để nối demo. Scene sửa thuyền Sprint15 giữ riêng và phải regression actual Player. G13 Accepted/Reopenable, legacy image1/repair image2 và thư viện01–18 giữ nguyên. Không tự mở Sprint17/main/tag/release.

**Sprint15 — bản thử đã chạy kiểm chứng Unity/OS input ngày06/11/2026:** [client2D repair proof](production/sprints/SPRINT-15.md), [ADR024](adr/024-bounded-2d-repair-client.md), [contract](contracts/REPAIR-CLIENT-PROFILE.md) và [cách build/run](runbooks/REPAIR-CLIENT.md). Đây là một worksite/nhân vật cố định với projection giới hạn và luật sửa thuyền thật. Actual LinuxMono Player,12ảnh/sáu layout cases, ba SQLite files và bàn phím/chuột riêng đã có evidence; năm gate đóng sprint, regression/review/docs/Git/CI theo revision tại [evidence](qa/SPRINT-15-EVIDENCE.md) và WORKLOG tách khỏi backlog random-life/fullVS07/finalart; các trạng thái lịch sử dưới đây giữ phạm vi tại mốc viết.

**Mốc mỹ thuật03/11/2026:** Sprint13 đã chốt hướng `earth-monument-2d-v1` — Đất dựng thành bằng nền2D có cảm giác khối và nhân vật sprite. [Baseline mỹ thuật/cổng G13](production/sprints/sprint-13/ART-DIRECTION-BASELINE.md), [ADR-022](adr/022-earth-monument-2d-and-reopen-gate.md) và [approval register](production/sprints/sprint-13/APPROVALS.md) ghi phạm vi **Accepted / Reopenable**, các follow-up UI/Reveal và cách đổi mỹ thuật giữ phần core/luật/dữ liệu đã có. Lịch sử thử nghiệm01–18 tiếp tục được giữ; chốt định hướng chưa là runtime/gameplay acceptance hoặc mởSprint14.

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

[Sprint4 planning](production/sprints/SPRINT-04.md), [ADR013](adr/013-in-memory-scheduler.md) và [evidence](qa/SPRINT-04-EVIDENCE.md): scheduler/AdvanceTime in-memory; persistence FND11 là bước tiếp theo.

[Sprint5](production/sprints/SPRINT-05.md), [ADR014](adr/014-headless-sqlite-persistence.md), [provider spike](qa/SQLITE-PROVIDER-SPIKE.md), [evidence](qa/SPRINT-05-EVIDENCE.md): headless SQLite commit/reopen, OPEN03 Unity còn pending.

[Sprint6](production/sprints/SPRINT-06.md), [ADR015](adr/015-snapshot-and-recovery-frames.md), [evidence](qa/SPRINT-06-EVIDENCE.md): snapshots/manual save, committed-image recovery và migration1→2 sang file mới.

[Vận hành save/recovery headless](runbooks/SAVE-RECOVERY.md): API, lỗi, backup và giới hạn.

[Sprint7](production/sprints/SPRINT-07.md), [ADR016](adr/016-headless-command-replay.md), [báo cáo](qa/SPRINT-07-EVIDENCE.md): replay QA thực thi lại hành động, fixed golden và first-divergence report.

[Hướng dẫn replay QA](runbooks/REPLAY-QA.md): cách chạy, đọc divergence và giữ golden.

[Sprint14 — luật sửa thuyền headless](production/sprints/SPRINT-14.md), [repairprofile](contracts/BOAT-REPAIR-PROFILE.md) và [ADR023](adr/023-bounded-boat-repair-profile.md): đã nghiệm thu headless với562/562 tests (113repair), reserve/time/cancel/complete và SQLite/replay profile riêng; [evidence](qa/SPRINT-14-EVIDENCE.md), chưa client2D hoặc fullVS03.
