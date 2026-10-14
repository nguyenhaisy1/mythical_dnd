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

Git đã được init local trong lần chuẩn bị; không remote, commit hoặc push thay người dùng. Các build/export artifacts không track mặc định; nguồn tài liệu nằm trực tiếp trong repository.
