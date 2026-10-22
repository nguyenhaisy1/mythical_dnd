# Đối chiếu runtime .NET và Unity — candidate Linux x64

## Mục đích và trạng thái

`kernel-conformance-v1` chạy cùng `RuntimeConformance` từ portable Infrastructure trên host .NET và Editor Mono. Đây là công cụ chuẩn bị FND16; chưa có kết quả Unity thật. Client hiện vẫn là layout, công cụ không sửa `client/UnityProject` và không tự tạo ProjectVersion giả.

Chuẩn bị dependencies và chạy host:

```bash
python3 tools/unity/run_conformance.py --dotnet /path/to/dotnet --prepare-only
```

Sau khi cài Unity Hub, cài Editor Unity 6 và kích hoạt license hợp lệ, dùng đường dẫn executable thật:

```bash
python3 tools/unity/run_conformance.py --dotnet /path/to/dotnet --editor /path/to/Editor/Unity
```

Runner tạo project tạm bằng chính Editor trong `tests/results/unity-*`, đặt API .NET Standard, nhập DLL Release cùng SQLite native Linux x64, rồi chạy probe. Không nhập code vào project đang làm việc. Output phải là thư mục mới; không tái sử dụng evidence hoặc database. Caches, DLL và project tạm đều là artifacts, không commit.

## Dữ liệu và acceptance

1. Codec tiếng Việt và RNG đối chiếu known-answer cố định.
2. Replay fixture bất biến chạy 15 actions, kiểm final committed image hash.
3. Content compiler kiểm hash registry của pack kernel.
4. SQLite thực thi command, mở lại database và kiểm lượng tài nguyên 7/3, revision2; snapshot count tăng, state giữ nguyên; từ chối content không tương thích mà không đổi file.
5. Comparator yêu cầu đúng profile, fingerprint cố định, năm checks và SQLite version giống host; actual Editor version phải khớp ProjectVersion do Editor tạo, backend Editor-Mono/LinuxEditor/.NET Standard.

`evidence.json` lưu host result, fixture hash, import hashes, trạng thái Unity, exit code và đường dẫn logs. Locked restore kiểm dependency graph; staging so metadata contentHash với lock và archive SHA512 với sidecar cache, sau đó ghi SHA256 từng DLL. Đây là kiểm tính nhất quán cache, không phải chứng minh cache chống kẻ sửa đồng thời cả metadata. Assembly System.* do Editor cung cấp được ghi đường dẫn/hash thay vì nhập trùng, nhưng vẫn phải vượt actual parity.

| Kết quả | Ý nghĩa |
|---|---|
| Prepared / exit0 | Host probe và staging đạt; Unity vẫn NotRun |
| NotRun / exit2 | Không có Editor; không tạo project hoặc báo Passed |
| Failed / exit1 | Build/import/license/runtime/parity lỗi; đọc logs |
| Passed / exit0 | Cả host và Editor đã chạy đúng candidate profile; không chứng minh Player/IL2CPP |

Không bỏ qua lỗi native load, compilation hoặc license để đổi thành Passed. Khi actual run đạt, lưu exact patch và cấu hình vào ADR/OPEN04, quyết định provider OPEN03 từ bằng chứng và mở gate Player riêng nếu cần. Tests validator dùng báo cáo synthetic chỉ kiểm công cụ, không được đưa vào thống kê Unity runs.

Hướng dẫn chính thức: [cài Editor bằng Hub](https://docs.unity.com/en-us/hub/add-editor), [quản lý license](https://docs.unity.com/en-us/engine/6000.3/manual/get-started/install-and-upgrade/licenses-and-activation/managing-your-unity-license). Chủ dự án thực hiện đăng nhập/kích hoạt; không đưa tài khoản/license vào repository.
