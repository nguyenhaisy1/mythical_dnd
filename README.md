# Mythical DnD — Hồng Bàng / Mạch Lạc

Living World Narrative RPG lấy chất liệu Văn Lang–Âu Lạc. Nội dung, nhân quả, nghề nghiệp, quan hệ và cuộc đời nhân vật là trọng tâm; gameplay runtime chạy offline bằng code/rules. Tên game hiện là working title.

**Trạng thái:** đã khôi phục GDD chi tiết và khởi tạo repository. Đã có primitives Sprint 1 và path TransferResource in-memory Sprint 2; Sprint3 có event journal/causal trace và idempotent consumer; Sprint4 có scheduler/AdvanceTime in-memory cùng demo thực thi. Sprint5 có SQLite current-state commit/reopen headless; snapshot/recovery và replay headless đã bổ sung Sprint6–7; full gameplay simulation chưa được nghiệm thu. Sprint15 đã có bản thử client2D chạy và thao tác thật; phạm vi nghiệm thu và điều kiện đóng sprint nằm ở báo cáo sprint.

**Sprint13 — chốt ngày03/11/2026:** hướng **Đất dựng thành, nền2D có cảm giác khối + nhân vật sprite**, profile `earth-monument-2d-v1`. [Baseline mỹ thuật và cổng G13](docs/production/sprints/sprint-13/ART-DIRECTION-BASELINE.md) giữ trạng thái **Accepted / Reopenable**: có thể mở lại hướng mỹ thuật mà giữ engine, luật, dữ liệu và tiến độ độc lập đã làm. Đây là chốt định hướng; sprite/scene/UI, chi tiết Reveal và hiệu năng vẫn cần triển khai/kiểm chứng, chưa là playable acceptance và chưa tự mở Sprint14. [Quyết định ADR-022](docs/adr/022-earth-monument-2d-and-reopen-gate.md).

## Sprint15 hiện hành

[Sprint15](docs/production/sprints/SPRINT-15.md) đưa luật sửa thuyền lên một cảnh Unity2D tại Bến Vạc với nhân vật cố định, nền phân lớp và sprite riêng. [ADR024](docs/adr/024-bounded-2d-repair-client.md), [client contract](docs/contracts/REPAIR-CLIENT-PROFILE.md) và [runbook](docs/runbooks/REPAIR-CLIENT.md) chốt dữ liệu theo nhân vật, preview/command/save boundaries và năm điều kiện đóng sprint. Ngày06/11/2026 đã kiểm actual Unity6000.6.7f1/LinuxMono Player: build0lỗi/0cảnh báo,12ảnh renderer, sáu tổ hợp độ phân giải/cỡ chữ và ba SQLite files có kết quả sửa/hủy/chờ đúng. Kiểm bàn phím/chuột hệ điều hành riêng đã đạt: Tab/Enter/Space, lưu/mở lại, hủy, nhấn–thả thuyền chỉ mở preview và cú nhấp xác nhận riêng mới bắt đầu sửa. [Evidence Sprint15](docs/qa/SPRINT-15-EVIDENCE.md) ghi năm gate; regression cuối, review/docs và Git/CI theo revision phải đủ trước đóng sprint, không suy từ kết quả build đơn lẻ.

Bản thử luôn ghi rõ nhân vật cố định; chưa đại diện random-life onboarding/fullVS07 hoặc final approval của UI/art. Hướng `earth-monument-2d-v1` và G13 Reopenable giữ nguyên.

## Sprint14 đã nghiệm thu

[Sprint14](docs/production/sprints/SPRINT-14.md) đã nghiệm thu luật sửa thuyền headless: giữ vật liệu, chờ đúng thời gian, hoàn tất/hủy và lưu/tải/replay. [Kiểm chứng](docs/qa/SPRINT-14-EVIDENCE.md):562/562 tests,113 repair,0failed/skipped và process gates đạt. Hướng Đất dựng thành2D/sprite đã chốt ởG13 được giữ; chưa Unitydemo hoặc sprite chuyển động được nghiệmthu. [ADR023](docs/adr/023-bounded-boat-repair-profile.md) và [runbook](docs/runbooks/BOAT-REPAIR.md) ghi scope/profile/commands.

## Bắt đầu đọc

- [Bàn giao cho đội phát triển](docs/production/HANDOFF.md).
- [GDD — mục lục 29 module và phụ lục](docs/gdd/README.md).
- [Master GDD đầy đủ](docs/gdd/MASTER-GDD.md).
- [Baseline áp dụng](docs/governance/APPLICABLE-BASELINE.md), [quyết định](docs/governance/DECISIONS.csv), [việc còn mở](docs/governance/OPEN-DECISIONS.csv).
- [Kiến trúc repository](ARCHITECTURE.md), [ownership](docs/contracts/OWNERSHIP.md), [Foundation contract](docs/contracts/FOUNDATION-CONTRACT.md).
- [Kế hoạch triển khai](docs/production/IMPLEMENTATION-PLAN.md), [backlog](docs/production/BACKLOG.csv), [QA](docs/qa/QA-PLAN.md).
- [Toàn bộ tài liệu và công cụ](docs/README.md), [hướng dẫn export/đóng gói](docs/runbooks/DOCUMENTATION.md).

## Build và kiểm tra

Cần Python 3.10+ cho tooling stdlib và .NET SDK 10.0.401 (hoặc patch tương thích theo `global.json`). Export HTML/DOCX cần Pandoc 3.x. Core libraries target **netstandard2.1**, host target **net10.0**; không hiểu host TFM là Unity runtime.

```bash
python3 tools/docs/build_docs.py --check
python3 tools/checks/validate_reference.py
python3 tools/checks/check_architecture.py
python3 tools/checks/check_handoff.py
python3 tools/tests/run_runtime.py
dotnet run --project engine/HeadlessRunner/Game.Headless.csproj --configuration Release --no-build -- --status
```

`--status` trả `simulationImplemented: false`. Lệnh simulate/replay không được giả làm thành công khi chưa triển khai. Sprint6 bổ sung FND12 snapshot/recovery/migration headless; trạng thái nghiệm thu ở [báo cáo Sprint6](docs/qa/SPRINT-06-EVIDENCE.md). Sprint7 bổ sung replay QA; xem [báo cáo Sprint7](docs/qa/SPRINT-07-EVIDENCE.md). Sprint8 bổ sung compiler/registry nội dung; xem [Content Pipeline](docs/runbooks/CONTENT-PIPELINE.md) và [evidence Sprint8](docs/qa/SPRINT-08-EVIDENCE.md). Sprint9 bổ sung [diagnostics](docs/runbooks/DIAGNOSTICS.md) và [evidence](docs/qa/SPRINT-09-EVIDENCE.md). Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. Bước tiếp theo là VS02 khi được giao; xem backlog trước khi mở rộng.

## Bố trí dự án

```text
engine/        Foundation, Domain, Application, Infrastructure, HeadlessRunner
client/        Unity2D workspace được Editor6000.6.7f1 tạo; runtime gates theo Sprint15
content/       schemas, reference fixtures, definitions, narrative, localization
assets/        source art/provenance; bounded Sprint15 2D assets, chưa full production
rules/         specifications và versioned parameter packs cho các profile đã triển khai
docs/          GDD, ADR, contracts, governance, production, QA, runbooks, sources
tests/         Unit, Integration, Properties, Replay, Content, Scenarios, Performance
tools/         documentation build/export, checks, vị trí các công cụ engine tương lai
artifacts/     bản đọc HTML/DOCX sinh từ tài liệu, không sửa bằng tay
```

Nguồn hội thoại lưu bất biến tại [docs/source](docs/source/INDEX.md). Không dùng Master summary phiên cũ thay thế nội dung nghiên cứu. `.agent` là phần giới thiệu ngắn; `AGENTS.md` hướng dẫn agent khi làm việc trong repository.

Tài liệu bổ sung sau audit: [trạng thái và đường dẫn](docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md). Kiểm vectors bằng `python3 tools/checks/check_contract_vectors.py`. Không tạo ZIP; cập nhật source và regenerate bản đọc khi thay tài liệu.

[Bộ skill riêng của project](docs/runbooks/PROJECT-SKILLS.md) hỗ trợ triển khai theo contracts và đồng bộ thay đổi. Cài liên kết bằng `python3 tools/skills/install.py`; kiểm bằng `python3 tools/skills/install.py --check`.

[Quy trình Git và release](docs/runbooks/GIT-WORKFLOW.md): feature → dev → release → main; version sản phẩm ở [VERSION](VERSION), thay đổi từng bản ở [CHANGELOG](CHANGELOG.md).

[Nhật ký công việc theo spin](WORKLOG.md) ghi goal, phân công, tiến độ, kết quả và evidence. Các bản ghi mới nối vào cuối file; dùng làm nguồn báo cáo khi cần.

Quy trình áp dụng cho AI: [AI Execution](docs/runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope Sprint 1 nằm trong sprint brief được liên kết tại đó.

## Sprint10 — chuẩn bị Unity conformance

[Bộ kiểm chứng và cách chạy](docs/runbooks/UNITY-CONFORMANCE.md) đối chiếu host với Editor thật. Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. Tại Sprint10 client game còn là layout; project conformance được tạo riêng bằng Editor thật. Client Sprint15 có scope/evidence riêng phía trên.

## Sprint11 — VS02A: dữ liệu dân cư và mạng đường

[Compiler và cách dùng](docs/runbooks/SETTLEMENT-CONTENT.md) kiểm24actors/6households/4communities/8routes/24claims/24directedrelationships, tạo immutable authoring registry. VS02 full vẫnpartial: chưa8–12eventtemplates/eligibility, actor-specificobligations, randomprotagonist, routine execution hoặc gameplay. Không coi đủrecordcount là worldsimulation.

## Sprint12 — điều kiện sự kiện

[Tám tình huống và evaluator](docs/runbooks/EVENT-ELIGIBILITY.md) kiểm điều kiện vật chất/nhận thức trên observations có kiểu; chưa tạo event effects hoặc NPC autonomy. [Lộ trình hình ảnh/demo](docs/production/PLAYABLE-ROADMAP.md): dự kiến hình ảnh Sprint13, demo nhỏ Sprint15–16 là forecast cần hiệu chỉnh theo đường2D sau chốtSprint13, chưa là deadline. [Sprint12](docs/production/sprints/SPRINT-12.md) ghi phạm vi hiện tại.
