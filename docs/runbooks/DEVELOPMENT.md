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

Runtime FND02–08 primitive/in-memory path: `python3 tools/tests/run_runtime.py` (thêm `--dotnet /path/to/dotnet` nếu SDK ngoài PATH). Runner restore locked/build Release, kiểm TRX nonzero và negative control; evidence dưới tests/results. Framework pin tại ADR-009; Unity/storage/replay còn pending. Local có lỗi MSBuild child-node MSB4166 khi chạy parallel; runner dùng `-m:1 -nr:false`, build tuần tự đã kiểm.

Git đã có initial commit và origin trỏ git@github.com:nguyenhaisy1/mythical_dnd.git. Branch/commit/push/version/changelog theo [Git Workflow](GIT-WORKFLOW.md); không coi yêu cầu implement là quyền publish release. Các build/export artifacts không track mặc định; nguồn tài liệu nằm trực tiếp trong repository.

## Nhật ký và báo cáo công việc

[WORKLOG](../../WORKLOG.md) là file append-only chung. Mỗi spin có goal, assignment, tiến độ, evidence, blockers và trạng thái Git; theo hướng dẫn AGENTS khi mở/tiếp tục/đóng spin. `tools/worklog.py` hỗ trợ append có file lock trên môi trường POSIX/Linux hiện tại; agent vẫn phải chủ động ghi khi có thay đổi.

Quy trình áp dụng cho AI: [AI Execution](AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope Sprint 1 nằm trong sprint brief được liên kết tại đó.

Demo thực thi: `dotnet run --project engine/HeadlessRunner/Game.Headless.csproj -c Release --no-build -- --transfer-demo`; chỉ in-memory transfer+retry, không game simulation/replay. Runtime gate cũng chạy và xác minh demo này.
