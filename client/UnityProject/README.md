# Unity2D repair client workspace

Project này được Unity6000.6.7f1 tạo thật trong Sprint15. ProjectSettings/ProjectVersion.txt, Packages manifest/lock và source `.meta` có provenance Editor; Library/Temp/build/DLL staging là artifacts ignored. Kết quả actual import/build/Player và các điều kiện đóng sprint xem [evidence Sprint15](../../docs/qa/SPRINT-15-EVIDENCE.md), không suy từ sự hiện diện của source.

Theo [ADR024](../../docs/adr/024-bounded-2d-repair-client.md) và [client contract](../../docs/contracts/REPAIR-CLIENT-PROFILE.md), đây là một worksite/nhân vật cố định tại Bến Vạc. UI phải hiện nhãn bản thử kỹ thuật; không có chọn/chỉnh origin hoặc reroll. Presenter chỉ nhận `IRepairClientSession`/safe DTO; Bootstrap là composition point bind Infrastructure. Rules/time/resources/save truth ở core Sprint14, không ở MonoBehaviour.

## Build và chạy

Từ root repository:

```sh
python3 tools/unity/run_repair_client.py --dotnet /path/to/dotnet --editor /path/to/Editor/Unity
```

[Runbook](../../docs/runbooks/REPAIR-CLIENT.md) mô tả dependencies, DISPLAY, output mới, actual Editor methods, LinuxMono Player, autoplay/captures và OS-input probe. Runner stage portable DLL/native SQLite vào `Assets/Plugins/MythicalRuntime` trước khi import; không commit hoặc chép headless net10 executable vào project. Khi clone mới, chạy staging/build runner trước khi mở project trong Editor vì imported binaries không nằm trong Git.

Scene được Editor tạo tại `Assets/Game/Scenes/Sprint15Repair.unity`. Runtime code ở `Assets/Game/Presentation`; configure/import/scene/build hooks ở `Assets/Game/Editor`. Profile authoring resource phải khớp parameterpack; font NotoSans có LICENSE.txt đi cùng. Không reinitialize hoặc overwrite existing save để xử lý incompatibility.

## Mỹ thuật và phạm vi

`earth-monument-2d-v1` dùng base2D có cảm giác khối, actor sprite, boat variants và foreground occluder riêng; metadata nguồn ở `assets/production/sprint15`. Camera cố định, local footpoint chỉ là presentation trong một worksite; không Travel hoặc physics gameplay. Không tạo model3D.

[G13](../../docs/production/sprints/sprint-13/ART-DIRECTION-BASELINE.md) vẫn Accepted/Reopenable, giữ core/rules/content/stableIDs và thư viện01–18 khi nghiên cứu lại mỹ thuật. Bản này chưa đại diện random-life onboarding/fullVS03/fullVS07, final UI/art approval hoặc human playtest. Những phần đó là backlog dự án ngoài năm gate đóng Sprint15.
