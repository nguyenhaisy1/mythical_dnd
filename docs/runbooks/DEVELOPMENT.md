# Môi trường phát triển

Cài .NET SDK theo `global.json`; [download chính thức](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). `dotnet --version` phải resolve SDK tương thích. Toolchain lần bàn giao được cài tạm ngoài repository để build, không được đóng gói cùng project.

Python 3.10+ chạy tooling stdlib, không cần API key hoặc package server. Pandoc 3.x là tùy chọn cho export HTML/DOCX, không cần để build engine. Khi làm việc trong repository:

```bash
python3 tools/docs/build_docs.py --check
python3 tools/checks/validate_reference.py
python3 tools/checks/check_architecture.py
python3 tools/checks/check_handoff.py
dotnet build MythicalDnd.sln -c Release
dotnet run --project engine/HeadlessRunner/Game.Headless.csproj -c Release --no-build -- --status
```

Các test runtime tương lai nằm theo nhóm trong tests; chưa cài test framework để tránh diễn đạt một empty suite như game đã pass. Khi task đầu tiên cần test, chọn/pin framework trong tooling ADR và CI, bảo đảm test có assertions vào invariants thật.

Git đã có initial commit và origin trỏ git@github.com:nguyenhaisy1/mythical_dnd.git. Branch/commit/push/version/changelog theo [Git Workflow](GIT-WORKFLOW.md); không coi yêu cầu implement là quyền publish release. Các build/export artifacts không track mặc định; nguồn tài liệu nằm trực tiếp trong repository.

## Nhật ký và báo cáo công việc

[WORKLOG](../../WORKLOG.md) là file append-only chung. Mỗi spin có goal, assignment, tiến độ, evidence, blockers và trạng thái Git; theo hướng dẫn AGENTS khi mở/tiếp tục/đóng spin. `tools/worklog.py` hỗ trợ append có file lock trên môi trường POSIX/Linux hiện tại; agent vẫn phải chủ động ghi khi có thay đổi.

Quy trình áp dụng cho AI: [AI Execution](AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope Sprint 1 nằm trong sprint brief được liên kết tại đó.
