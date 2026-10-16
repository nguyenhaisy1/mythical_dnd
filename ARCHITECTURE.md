# Kiến trúc project

Baseline từ Modules 24–29, cụ thể hóa trong [Foundation Contract](docs/contracts/FOUNDATION-CONTRACT.md) và [Ownership](docs/contracts/OWNERSHIP.md). Đây là modular monolith offline, không phải microservices. Cùng simulation core chạy headless và được Unity gọi qua adapter.

## Các project hiện có

| Project | Framework | Vai trò | Được tham chiếu |
|---|---|---|---|
| Game.Foundation | netstandard2.1 | IDs, time, deterministic primitives | Không |
| Game.Domain | netstandard2.1 | Entities, value objects, rule contracts | Foundation |
| Game.Application | netstandard2.1 | Command/scheduler, projections, ports | Foundation, Domain |
| Game.Infrastructure | netstandard2.1 | Persistence/content/diagnostics adapters | Foundation, Domain, Application |
| Game.Headless | net10.0 | Composition root, CLI | Application, Infrastructure |

Foundation có identity/time/math/RNG/revision; Domain có immutable kernel snapshot và typed transfer rule; Application có command pipeline/in-memory atomic publication, event-v1 journal và idempotent diagnostic projection consumer. Infrastructure vẫn scaffold. Client Unity chưa có assembly chạy; FND-16 tạo adapter và xác nhận backend compatibility.

## Luồng thực thi đích

```text
Player/NPC/Scheduler Command
  -> Application: validate envelope, actor, revision
  -> Domain Rules: read-only snapshot, hard constraints, deterministic resolution
  -> Proposed ChangeSet
  -> Application: ownership + cross-domain invariants
  -> Infrastructure transaction: state/jobs/RNG/events/receipt/revision
  -> publish committed events
  -> idempotent projections + actor knowledge filter
  -> UI / Director exposure / Chronicle
```

Host có --transfer-demo thực thi path TransferResource trong bộ nhớ. Luồng durable storage/jobs/gameplay events/knowledge projections ở trên vẫn là kiến trúc đích; không coi demo là persistence/replay acceptance. Rule chỉ đề nghị changes; save adapter không tạo rule; UI không biết toàn bộ hidden truth. Query/preview không draw gameplay RNG. Phân biệt scheduled request, committed fact và exposed information ở mọi API.

## Ranh giới nội dung

JSON authoring không phải scripting engine. Approved rule handlers được code và kiểm thử; content chỉ tham chiếu ID/parameters. AI có thể hỗ trợ author offline nhưng không đi vào runtime decision path. Compiler phải kiểm schema, references, semantic eligibility và version/hash trước khi tạo pack.

`content/fixtures` hiện chứa design fixture để thống nhất canon và scope; không có compiled pack. `content/compiled` là build output. `rules` dành cho parameter/registry authoring sau khi contract cụ thể; rule implementation thuộc Domain/Foundation, không copy logic thành text executable.

## Các mở rộng so với cây tham chiếu M28

Thêm logical folders Settlements/Factions/Ecology/Belief để các subsystem đã có GDD có nơi triển khai rõ; vẫn chung Domain assembly. Thêm governance và production docs cho quyết định/traceability/task handoff. Không tạo hàng chục assembly hoặc GameManager toàn quyền từ đầu.

SDK 10.0.401 được chọn để kiểm tra scaffold ở lần bàn giao này; core vẫn .NET Standard 2.1 để đi theo baseline portable library. [Microsoft .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) và [Unity API compatibility](https://docs.unity.com/en-us/engine/6000.7/manual/programming-environment/overview-of-dot-net-in-unity/dotnet-profile-support) là tài liệu toolchain tham chiếu. Build .NET thành công chưa chứng minh Mono/IL2CPP tương thích; gate FND-16 yêu cầu test thật.
