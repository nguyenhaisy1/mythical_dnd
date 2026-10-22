# Bootstrap Unity ở bước FND-16 / VS-07

Hiện repository chỉ có thư mục Assets dự kiến; không có Unity Editor/runtime trong bằng chứng bàn giao. Trước khi tạo client thật:

1. Chọn Unity editor patch, target platform, backend (Mono/IL2CPP) và ghi ADR/OPEN-04. Đối chiếu hỗ trợ .NET Standard 2.1 và ngôn ngữ C# với Editor đã chọn.
2. Tạo project thật qua Unity Hub/Editor tại `client/UnityProject` (giữ hoặc import layout Assets/Game). Commit ProjectSettings/ProjectVersion.txt, Packages manifest/lock và source .meta do Editor tạo. Không commit Library/Temp/UserSettings.
3. Build core libraries Release/netstandard2.1. Import DLL Foundation/Domain/Application cùng dependency cần thiết vào Assets/Plugins/GameCore bằng quy trình tái lập; không import headless net10 executable.
4. Tạo Unity adapter/ViewModels gọi public Application API và actor-scoped queries. Composition root inject adapters; MonoBehaviour chỉ orchestration input/render, không là nguồn rule/world truth.
5. Chạy golden RNG/serialization/command replay trên host và Unity selected backend. Chỉ đánh dấu FND-16 đạt khi hash và side-effect behavior khớp trong phạm vi version đã chốt.
6. Làm scene/UI tối thiểu cho vertical slice, kiểm hidden knowledge qua search/map/journal/tooltips và test accessibility/readability. Không mở full art production trước khi flow hoạt động.

Nguồn công nghệ: [Unity .NET compatibility](https://docs.unity.com/en-us/engine/6000.7/manual/programming-environment/overview-of-dot-net-in-unity/dotnet-profile-support). Đây là đường dẫn tài liệu tham chiếu, không phải cam kết chọn 6000.7 cho project.

## Prototype UX và early spike

[Client UX Baseline](../production/CLIENT-UX-BASELINE.md) quy định desktop keyboard/mouse, Reveal/action/save/succession flows. Có thể spike DLL primitive vectors sau FND-02/03/04; final FND-16 vẫn cần command replay FND-13. Không ghi ProjectVersion chưa lấy từ Editor thực tế.

## Sprint10: isolated conformance preparation

[Bộ chạy conformance](UNITY-CONFORMANCE.md) tạo project kiểm chứng riêng bằng Editor thật khi có cài đặt/license. Máy hiện chưa cài/kích hoạt Unity; client layout chưa bootstrap và FND16 vẫn NotRun. Không dùng DLL build .NET hoặc validator synthetic thay Editor evidence.
