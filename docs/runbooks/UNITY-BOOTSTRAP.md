# Bootstrap Unity ở bước FND-16 / VS-07

Sprint15 đã dùng Editor6000.6.7f1 tạo project thật tại `client/UnityProject`. [ADR024](../adr/024-bounded-2d-repair-client.md) và [Repair Client runbook](REPAIR-CLIENT.md) là đường thực thi bounded2D/fixedactor hiện hành; actual build/run LinuxMono Player và UI/input gates theo [evidence Sprint15](../qa/SPRINT-15-EVIDENCE.md). Sprint10 chỉ có evidence kernel/Editor profile riêng. Không tạo đè project hiện có; quy trình bootstrap và mở rộng client gồm:

1. Chọn Unity editor patch, target platform, backend (Mono/IL2CPP) và ghi ADR/OPEN-04. Đối chiếu hỗ trợ .NET Standard 2.1 và ngôn ngữ C# với Editor đã chọn.
2. Tạo project thật qua Unity Hub/Editor tại `client/UnityProject` (giữ hoặc import layout Assets/Game). Commit ProjectSettings/ProjectVersion.txt, Packages manifest/lock và source .meta do Editor tạo. Không commit Library/Temp/UserSettings.
3. Build core libraries Release/netstandard2.1. Import DLL Foundation/Domain/Application/Infrastructure cùng pinned SQLite dependencies/native Linux cần thiết vào Assets/Plugins/MythicalRuntime bằng quy trình tái lập; không import headless net10 executable.
4. Tạo Unity adapter/ViewModels gọi public Application API và actor-scoped queries. Composition root inject adapters; MonoBehaviour chỉ orchestration input/render, không là nguồn rule/world truth.
5. Chạy golden RNG/serialization/command replay trên host và Unity selected backend. Chỉ đánh dấu FND-16 đạt khi hash và side-effect behavior khớp trong phạm vi version đã chốt.
6. Làm scene/UI tối thiểu cho vertical slice, kiểm hidden knowledge qua search/map/journal/tooltips và test accessibility/readability. Không mở full art production trước khi flow hoạt động.

Nguồn công nghệ: [Unity .NET compatibility](https://docs.unity.com/en-us/engine/6000.7/manual/programming-environment/overview-of-dot-net-in-unity/dotnet-profile-support). Đây là đường dẫn tài liệu tham chiếu, không phải cam kết chọn 6000.7 cho project.

## Prototype UX và early spike

[Client UX Baseline](../production/CLIENT-UX-BASELINE.md) quy định desktop keyboard/mouse, Reveal/action/save/succession flows. Có thể spike DLL primitive vectors sau FND-02/03/04; final FND-16 vẫn cần command replay FND-13. Không ghi ProjectVersion chưa lấy từ Editor thực tế.

## Sprint10: isolated conformance preparation

[Bộ chạy conformance](UNITY-CONFORMANCE.md) tạo project kiểm chứng riêng bằng Editor thật khi có cài đặt/license. Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. Tại mốc Sprint10 client game còn là layout; Sprint15 hiện thực client riêng theo runbook phía trên. Không dùng DLL build .NET hoặc validator synthetic thay Editor evidence.


## Profile2D đã chốt và khả năng mở lạiG13

Ngày03/11/2026 chủ dự án chốt [earth-monument-2d-v1](../production/sprints/sprint-13/ART-DIRECTION-BASELINE.md): nền2D phân lớp có cảm giác khối + nhân vật sprite. Task client phải kiểm renderer/import tương thích bằng Editor, vùng đi/cao độ/occluder/camera và asset mapping riêng; không suy package đã cài từ quyết định. Khi cần đổi hướng, theoG13 trên core/commands/actor projection hiện hành, giữ gameplay/content/save contracts; không quay cả project về mốc Sprint13. UI/runtime/asset rights và hiệu năng vẫn cần acceptance riêng; quyết định này không là lệnh bootstrap trong task chốt.

## Bounded client profile Sprint15

Scene `Sprint15Repair` dùng camera orthographic cố định, SpriteRenderer và UI Toolkit built-in; profile asset/anchor/pivot nằm trong manifest Sprint15. `IRepairClientSession` giữ actor binding/observation grants, pure preview và safe result; presenter không nhận raw world/receipt/jobs. Physical SQLite schema2 và repair image2 giữ theo Sprint14, không thay schema để phù hợp camera.

Actual Editor bootstrap, LinuxMono build/run, rendered captures và OS-input là gates của Sprint15. Source/layout checker hoặc headless suite không thay chúng. Toàn bộ fullVS07 onboarding/map/journal/knowledge, final UX/art approval và human playtest vẫn là backlog khác, không trộn vào điều kiện đóng sprint giới hạn.
