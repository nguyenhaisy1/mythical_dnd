# Build và kiểm tra bản thử sửa thuyền Unity2D

## Mục đích và trạng thái

`repair-client-2d-v1` là bản thử kỹ thuật tại Bến Vạc, dùng một nhân vật cố định do trusted fixture cấp. Cảnh dùng `earth-monument-2d-v1`: nền2D có cảm giác khối, actor/boat/foreground riêng và camera cố định. Luật vật liệu/thời gian và SQLite là implementation Sprint14; UI không sở hữu world state. [ADR024](../adr/024-bounded-2d-repair-client.md), [contract](../contracts/REPAIR-CLIENT-PROFILE.md), [brief](../production/sprints/SPRINT-15.md) và [evidence](../qa/SPRINT-15-EVIDENCE.md) là nguồn về phạm vi và kết quả thực.

Project đã được Editor6000.6.7f1 tạo thật. Sự hiện diện của ProjectVersion, source hoặc scene chưa tự chứng minh build/Player/interaction đã đạt; kiểm kết quả của revision cuối trong evidence. Bản này không nhận random-origin onboarding, full Knowledge/Perception, fullVS03/VS07 hoặc human playtest đã hoàn tất.

## Chuẩn bị và chạy acceptance

Từ root repository, cần .NET SDK theo global.json, Unity6000.6.7f1 đã kích hoạt với LinuxStandaloneSupport/Mono và một DISPLAY đồ họa Linux x64 đang hoạt động. Editor đã cài trên máy phát triển hiện tại tại `/home/synh/Unity/Hub/Editor/6000.6.7f1/Editor/Unity`; đường dẫn SDK có thể truyền rõ thay vì phụ thuộc PATH.

```sh
python3 tools/unity/run_repair_client.py --dotnet /path/to/dotnet --editor /path/to/Editor/Unity
```

Runner tạo thư mục mới `tests/results/sprint15-client-*`. Có thể truyền `--output /path/to/new-evidence-directory`, nhưng thư mục đó phải chưa tồn tại. Không tái sử dụng lượt cũ hoặc xóa dữ liệu người dùng để làm kiểm tra xanh. Đóng Editor đang mở cùng project trước khi chạy.

Runner thực hiện:

1. Locked restore/build solution Release; stage bốn portable core DLL và pinned SQLite dependencies/native Linux từ cùng logic đã kiểm ở `run_conformance.py`.
2. Đặt imports tại `Assets/Plugins/MythicalRuntime`, dùng assembly System.* do Editor cung cấp khi phù hợp. Không nhập host net10 executable vào Unity. DLL/PDB/native.so/import inventory là artifact ignored; source `.meta`, ProjectSettings và Packages được duy trì bằng Git.
3. So checksum resource `Assets/Game/Resources/Sprint15/boat-repair-v1.json` với `rules/parameters/boat-repair-v1.json`; bản sao stale bị từ chối.
4. Chạy actual Editor methods `RepairClientBuild.Configure`, `PrepareScene`, `Build`: .NET Standard2.1/Mono Linux, importer/pivot/sorting setup, tạo scene `Assets/Game/Scenes/Sprint15Repair.unity`, build Player vào `<run>/Player/MythicalRepair.x86_64`.
5. Chạy Player với đồ họa thật, autoplay qua cùng callback UI: preview/start/retry/save/reopen-Running, trước hạn59→hạn60, completion; nhánh QA riêng cho cancel và wait. Chụp frame renderer tại1280×720 và1920×1080, text scale và hai vị trí occlusion.
6. Ghi build/Editor/Player logs, imports, source manifest/fingerprint, binary/editor hashes, `autoplay/qa.json` và PNG captures trong run directory. Sources không được đổi trong lượt Player acceptance.

Exit0 chỉ khi actual build/run và các kiểm tra được runner yêu cầu đạt. Exit1 cần đọc `evidence.json.error` và log phase tương ứng; không đổi Failed thành Passed từ việc sửa source mà chưa chạy lại. Build/import thành công, callback automation, OS input và visual review là các loại evidence riêng; không gọi chúng là human playtest.

## Mở bản đã build và thao tác

Binary nằm tại `<run>/Player/MythicalRepair.x86_64`. Khi chạy bình thường, save root nằm dưới `Application.persistentDataPath/repair-client-2d-v1`; không có UI nhập actor, seed, origin hoặc đường dẫn tùy ý. Có thể dùng CLI trusted `-s15SaveRoot /path/to/separate-demo-directory` để tách dữ liệu thử. Existing compatible world được mở lại; incompatible/corrupt file không được reinitialize/overwrite.

UI hiển thị nguồn quan sát, condition của thuyền, gỗ đang có/giữ chỗ/khả dụng và own activity. Giá/thời gian hiện trong preview trước Confirm. “Chưa rõ” là dữ liệu chưa có grant, không phải world truth được gửi xuống rồi giấu bằng text. Save snapshot chỉ báo thành công sau durable write; Reload đọc latest committed state, không khôi phục về slot trước đó.

| Phím/thao tác | Hành vi source cung cấp; cần đối chiếu actual input evidence |
|---|---|
| Chọn thuyền hoặc R | Mở preview của bound visible boat |
| Enter khi modal mở | Submit nút đang focus: Confirm hoặc Quay lại; token và requestId chỉ dùng khi Confirm |
| Esc | Đóng preview, không hủy committed activity |
| Space | Chờ1phút simulation qua command |
| C | Hủy own repair đang Running |
| S / L | Save snapshot / Reload |
| Tab | Điều hướng focus; kiểm focus visible và không input xuyên modal |
| Nút chờ59/60phút | AdvanceTime/Drain qua session, animation không tự hoàn tất repair |

Autoplay chặn input vật lý/native UI để không nhận lệnh ngoài kịch bản; callback kiểm modal vẫn được giữ. Runner kiểm lại trạng thái sau captures và đọc trực tiếp ba SQLite files: condition, material hold, activity và số StartRepair receipts, thay vì chỉ tin snapshot DTO ở giữa lượt. Sáu layout cases và12ảnh renderer phải có đủ; không sửa/cắt ảnh để che lỗi.

Chạy OS-input riêng bằng `python3 tools/unity/run_repair_input.py --player /path/to/Player/MythicalRepair.x86_64`. Driver dùng X11/xdotool, đợi `inputReady` rồi chỉ gửi phím/chuột vào đúng PID/window; nếu focus chuyển sang app khác thì dừng. Space là shortcut chờ một phút duy nhất, không đồng thời activate nút đang focus. Nhấp cảnh chỉ được dispatch sau nhấn–thả hoàn chỉnh và sau native UI release, nên gesture mở preview không thể đồng thời Confirm; xác nhận bằng chuột cần một cú nhấp mới. Lượt này kiểm native Tab/Back/Return, snapshot/reload, Space với focus Repair/Wait, cancel, Esc và chọn thuyền. Đây automation, không human playtest. Xem actual width/height của probe, không suy từ tham số yêu cầu: WM có thể giảm decorated window1280×720 xuống1280×683. Ma trận layout/capture1280×720 và1920×1080 được kiểm riêng trong autoplay;1920 dùng borderless fullscreen để không bị viền cửa sổ cắt frame.

QA input mode dùng `-s15InputProbe /path/to/new-output-directory`; F10 chụp kết quả và thoát. Markers `S15_INPUT_KEY` và output `qa.json` chỉ có giá trị OS-input khi phím thực sự được gửi tới đúng cửa sổ/PID của Player. Không lấy autoplay method calls làm bằng chứng bàn phím/chuột.

## Asset và hình học trình bày

Sáu runtime PNG ở `Assets/Game/Resources/Sprint15/Art`; nguồn, prompts, hashes, pivots, anchors và QC tại `assets/production/sprint15/manifest.json`. Base1920×1080/world16×9/PPU120, orthographicSize4.5; một ground surface và local lane được khai báo trong client contract/scene spec. Local footpoint chỉ đổi Transform/render order, không actor settlement, Travel, WorldTick hoặc quyền sửa thuyền. Geometry không được suy từ pixel để quyết định luật.

Font runtime là NotoSans-Regular tại `Resources/Sprint15/Fonts`, kèm LICENSE.txt/copyright từ nguồn font đã cài. Raster UI trong ảnh concept không được dùng làm nhãn runtime. Asset mới là hình hư cấu lấy cảm hứng Việt cổ phục vụ engineering proof, chưa là phục dựng khảo cổ hoặc final user art approval; không có model3D trong pipeline này. Thư viện01–18 và G13 v1 giữ nguyên.

## Kiểm tra source và nghiệm thu

`python3 tools/checks/check_repair_client.py` kiểm source/assets/layout và metadata; không chạy Unity. `python3 tools/tests/run_runtime.py --dotnet /path/to/dotnet` kiểm headless regression và session/privacy/persistence tests; không thay actual Player gate.

Để đóng Sprint15 phải đủ năm gate trong contract, có review độc lập và CI đúng HEAD. Ghi **Điều kiện đóng Sprint15 còn thiếu** riêng với **Backlog dự án ngoài sprint**. Full random life/world knowledge/settlement binding/three professions, fullVS03/VS07, final UI/art approval, performance/balance và human playtest không thuộc điều kiện đóng sprint này.
