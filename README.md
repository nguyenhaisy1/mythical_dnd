# Mythical DnD — Hồng Bàng / Mạch Lạc

Living World Narrative RPG lấy chất liệu Văn Lang–Âu Lạc. Nội dung, nhân quả, nghề nghiệp, quan hệ và cuộc đời nhân vật là trọng tâm; gameplay runtime chạy offline bằng code/rules. Tên game hiện là working title.

**Trạng thái:** đã khôi phục GDD chi tiết và khởi tạo repository. Đã có primitives Sprint 1 và path TransferResource in-memory Sprint 2; Sprint3 có event journal/causal trace và idempotent consumer; Sprint4 có scheduler/AdvanceTime in-memory cùng demo thực thi. Full gameplay simulation, persistence, replay và Unity client chưa được triển khai.

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

`--status` trả `simulationImplemented: false`. Lệnh simulate/replay không được giả làm thành công khi chưa triển khai. FND-02–10 có primitive/transfer/event/scheduler implementation và headless gates; bước tiếp theo **FND-11 persistence adapter**. Xem [nghiệm thu Sprint4](docs/qa/SPRINT-04-EVIDENCE.md) cho giới hạn in-memory. Unity conformance vẫn pending FND-16; xem backlog trước khi mở rộng.

## Bố trí dự án

```text
engine/        Foundation, Domain, Application, Infrastructure, HeadlessRunner
client/        Unity presentation workspace (chưa bootstrap Editor)
content/       schemas, reference fixtures, definitions, narrative, localization
assets/        vị trí source art/audio, chưa sản xuất asset
rules/         vị trí specification/parameter authoring, chưa có runtime pack
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
