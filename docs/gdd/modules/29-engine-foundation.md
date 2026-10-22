# Module 29 — Engine Foundation Implementation Specification

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T081](../../source/conversation.md#t081)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Module cuối foundation: giữ command/state/event/jobs/RNG/receipt nhất quán. Scaffold chỉ khởi tạo assembly/host; không tự tuyên bố foundation đã hoàn thành vì build được. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Typed IDs, clock, rules, commands, versions, scheduler và persistence contracts. |
| Đầu ra / bên sở hữu | Foundation task FND-01–16; authoritative commit, recovery và replay khi được triển khai. |
| Đọc cùng | 26–28; FOUNDATION-CONTRACT, IMPLEMENTATION-PLAN, QA-PLAN.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. ADR-017 và docs/runbooks/CONTENT-PIPELINE.md cho runtime content profile commodity/transfer-template; full VS-02/Unity vẫn có gate riêng. |
| Cổng nghiệm thu | Foundation exit gate cần replay, atomic failure recovery, content validation và cross-runtime evidence. |
| Việc chưa được chứng minh | C#/Unity implementation/conformance, provider/editor/test-runner spike và performance evidence; identity/time/RNG/codec đã có specs và Python reference vectors. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T081 BEGIN -->

<a id="m29-t081-s001"></a>

## GDD MODULE 29
<a id="m29-t081-s002"></a>

## ENGINE FOUNDATION IMPLEMENTATION SPECIFICATION
<a id="m29-t081-s003"></a>

### Đặc tả nền tảng thực thi cho Living World Narrative RPG

**Dự án:** Living World Narrative RPG — Văn Lang, Âu Lạc và Cổ Loa  
**Loại tài liệu:** Technical Design Specification / Engine Foundation Contract  
**Phiên bản:** 1.0 — Foundation Design Baseline  
**Trạng thái:** Đặc tả sẵn sàng chuyển giao để hiện thực hóa, chưa triển khai code  
**Phụ thuộc:** GDD Modules 01–28  
**Phạm vi:** Foundation Architecture, Entity Identity, WorldClock, Scheduler, Deterministic RNG, Command Processor, Rule Execution, Event Journal, Persistence, Recovery, Content Registry, Replay và Testing  
**Ngoài phạm vi:** Viết mã nguồn, tạo repository, triển khai Unity, chạy benchmark, sản xuất assets hoặc cân chỉnh gameplay bằng playtest.

---

<a id="m29-t081-s004"></a>

## PHẦN I — KIẾN TRÚC NỀN TẢNG CẦN ĐẠT

<a id="m29-t081-s005"></a>

### 1. Foundation Engine là gì?

Foundation Engine là tầng phần mềm làm cơ sở cho toàn bộ gameplay.

Nó không trực tiếp quyết định một NPC nên kết hôn, một người thợ có sửa được thuyền hay một trận chiến có người chết.

Những quyết định đó thuộc domain rules.

Foundation cung cấp cơ chế chung để các quyết định ấy:

- Đọc đúng World State.
- Được đánh giá theo quy tắc có phiên bản.
- Nhận nguồn random nhất quán nếu cần.
- Có quyền thực hiện và phạm vi ảnh hưởng rõ.
- Thay đổi dữ liệu một cách nguyên tử.
- Không vi phạm bất biến thế giới.
- Tạo các sự kiện có thể truy vết nguyên nhân.
- Được lưu và tải lại.
- Có thể tái hiện để kiểm thử.
- Có thể cung cấp thông tin đã được lọc cho các hệ thống khác.

Nếu Foundation không bảo đảm được những điều này, các hệ thống NPC, Economy, Quest, Combat và Succession sẽ phải tự xây giải pháp riêng, dẫn tới mâu thuẫn và khó bảo trì.

<a id="m29-t081-s006"></a>

### 2. Triết lý Foundation

Đề xuất khóa một nguyên tắc quan trọng:

**Foundation không biết câu chuyện game đang kể. Foundation chỉ biết cách một thay đổi hợp lệ trở thành sự thật.**

Ví dụ:

Foundation không cần biết Cổ Loa là gì.

Nó cần biết một Historical Anchor:

- có ID hợp lệ;
- có ruleset xác định;
- có dữ liệu bắt buộc;
- được đánh giá trong một World State nhất quán;
- khi thực thi sẽ tạo các sự kiện;
- không thể bị commit hai lần ngoài ý định;
- và mọi thay đổi đều có thể truy vết.

Tương tự, Foundation không cần biết Nỏ thần có ý nghĩa văn hóa gì.

Nó chỉ cần bảo đảm rằng một item, một production process và những nguồn lực sử dụng trong quá trình sản xuất được quản lý nhất quán.

<a id="m29-t081-s007"></a>

### 3. Năm tầng kiến trúc

Đề xuất Foundation được tổ chức thành năm tầng logic.

| Tầng | Vai trò | Có quyền thay đổi World State? |
|---|---|---|
| Foundation Primitives | Identity, time, deterministic arithmetic, RNG contracts | Không |
| Domain Rules | Kiểm tra và tính hậu quả theo nghiệp vụ game | Chỉ đề xuất thay đổi |
| Application Orchestrator | Điều phối command, scheduler, validation, transaction | Có quyền điều phối commit |
| State & Persistence | Lưu trạng thái và bản ghi đã commit | Có quyền ghi theo transaction |
| Query & Presentation Adapters | Cung cấp dữ liệu cho headless/UI/tools | Không ghi authoritative state |

Điểm cần đặc biệt lưu ý:

Domain Rules không được tự cập nhật database trong khi đang tính toán.

Một Rule Handler phải tạo ra **Proposed Changes**.

Application Orchestrator kiểm tra và commit những thay đổi đó.

<a id="m29-t081-s008"></a>

### 4. Runtime Topology

Mô hình runtime tham chiếu:

    Game Client / NPC AI / Scheduler
                  |
                  v
             Command API
                  |
                  v
          Command Processor
                  |
          +-------+--------+
          |                |
          v                v
    State Reader       Rule Registry
          |                |
          +-------+--------+
                  |
                  v
          Deterministic Resolver
                  |
                  v
             Change Set
                  |
                  v
          Invariant Validator
                  |
                  v
          Transaction Manager
                  |
          +-------+--------+
          |                |
          v                v
    Current State      Event Journal
          |                |
          +-------+--------+
                  |
                  v
          Projection / Scheduler
                  |
                  v
             Read Models
                  |
                  v
             Presentation

World Director, UI và LLM không nằm trong đường thực thi có quyền tùy ý ghi vào World State.

---

<a id="m29-t081-s009"></a>

## PHẦN II — PROJECT STRUCTURE & PUBLIC CONTRACTS

<a id="m29-t081-s010"></a>

### 5. Các assembly/project cần có

Đề xuất tách các logical projects:

| Project | Chức năng | Phụ thuộc được phép |
|---|---|---|
| `Game.Foundation` | ID, time, result, RNG interfaces | Không phụ thuộc gameplay |
| `Game.Domain` | Entity, Value Object, Rule Contract | Foundation |
| `Game.Application` | Command, scheduler, transaction coordination | Foundation, Domain |
| `Game.Infrastructure` | SQLite, content loader, file IO | Foundation, Domain, Application contracts |
| `Game.Headless` | Simulation host và CLI | Application, Infrastructure |
| `Game.UnityAdapter` | Input, presentation, scene integration | Application public API |
| `Game.Tests` | Unit, integration, replay | Các project cần kiểm thử |

Đây là project structure tham chiếu, không bắt buộc mọi domain đều phải là một assembly vật lý riêng.

Trong bản đầu, việc chia quá nhiều assembly có thể gây tốn công cấu hình mà chưa đem lại lợi ích.

Nhưng các dependency boundary phải rõ ngay cả khi nhiều domain cùng nằm trong một assembly.

<a id="m29-t081-s011"></a>

### 6. Những dependency bị cấm

`Game.Domain` không được tham chiếu:

- `UnityEngine`;
- presentation classes;
- SQLite connection;
- network/LLM client;
- wall-clock system time để quyết định gameplay;
- một global random singleton.

Một Rule Handler không được mở file hoặc tự ghi nhật ký save.

Nếu cần dữ liệu, nó phải nhận qua typed input hoặc read-only context.

Nếu cần gây hậu quả, nó phải trả về Change Set theo contract.

<a id="m29-t081-s012"></a>

### 7. Danh mục Public Foundation Contracts

Foundation cần định nghĩa tối thiểu:

| Contract | Nhiệm vụ |
|---|---|
| `EntityId` | Identity ổn định |
| `DefinitionId` | ID nội dung authored |
| `WorldTick` | Thời gian authoritative |
| `WorldRevision` | Phiên bản trạng thái |
| `CommandEnvelope` | Yêu cầu hành động |
| `CommandResult` | Kết quả xử lý |
| `RuleContext` | Ngữ cảnh đánh giá |
| `IRuleHandler` | Hợp đồng xử lý rule |
| `IReadOnlyWorldState` | Truy vấn trạng thái read-only |
| `ChangeSet` | Đề xuất thay đổi |
| `DomainEvent` | Kết quả đã xác lập |
| `ScheduledJob` | Công việc theo thời gian |
| `IRandomSource` | Random xác định |
| `ITransactionManager` | Commit nguyên tử |
| `IWorldRepository` | Truy cập authoritative state |
| `ISnapshotStore` | Lưu snapshot |
| `IReplayRunner` | Tái hiện chuỗi command |

Các tên này là contract design, không phải yêu cầu viết ngay toàn bộ interface thành code trong tài liệu.

<a id="m29-t081-s013"></a>

### 8. Quy tắc versioning của contract

Mỗi contract quan trọng có thể thay đổi về:

- tên trường;
- kiểu dữ liệu;
- quy tắc giá trị;
- ý nghĩa nghiệp vụ;
- cách serialize;
- thứ tự tính toán.

Không phải thay đổi nào cũng cần tăng cùng một loại version.

Cần phân biệt:

**Contract/Schema Version:** Hình dạng và ngữ nghĩa dữ liệu.

**Rule Version:** Thuật toán và cách xác lập outcome.

**Content Version:** Nội dung authored và references.

**Save Version:** Định dạng persistence.

**Random Algorithm Version:** Thuật toán và cách ánh xạ random.

**Simulation Version:** Những quy tắc tổng hợp, scheduling hoặc thứ tự xử lý có ảnh hưởng kết quả.

Mỗi save cần lưu manifest đủ để nhận diện các phiên bản liên quan.

---

<a id="m29-t081-s014"></a>

## PHẦN III — ENTITY REGISTRY & IDENTITY CONTRACT

<a id="m29-t081-s015"></a>

### 9. Những loại ID khác nhau

Không dùng cùng một tên gọi `ID` cho mọi mục đích.

Đề xuất:

**Entity ID:** Một đối tượng cụ thể trong World State.

**Definition ID:** Một loại nội dung có thể tạo nhiều instance.

**Command ID:** Một yêu cầu thực thi cụ thể.

**Event ID:** Một sự kiện được commit.

**Schedule ID:** Một công việc có thời điểm xử lý.

**Correlation ID:** Liên kết nhiều command/event thuộc cùng một chuỗi nhân quả.

**Snapshot ID:** Một ảnh chụp trạng thái.

Ví dụ:

    Definition: boat.river_transport.small
    Entity: item.000182
    Command: cmd.000931
    Event: evt.001204

Một loại thuyền có thể có nhiều item instance.

Không bao giờ sử dụng `DefinitionId` thay cho `EntityId` khi cập nhật quyền sở hữu.

<a id="m29-t081-s016"></a>

### 10. Entity Registry

Logical schema:

| Trường | Ý nghĩa | Ràng buộc |
|---|---|---|
| `entity_id` | Identity cụ thể | Unique, immutable |
| `entity_type` | Loại thực thể | Registered type |
| `definition_id` | Loại nội dung nếu có | Reference hợp lệ |
| `created_tick` | Thời gian tạo | Không lớn hơn current world time |
| `origin_event_id` | Sự kiện tạo, nếu cần | Reference hợp lệ |
| `lifecycle_state` | Vòng đời | State machine hợp lệ |
| `state_version` | Phiên bản entity | Chỉ tăng khi có thay đổi hợp lệ |

Một entity có thể chết, bị phá hủy hoặc rời simulation active, nhưng vẫn tồn tại trong registry để các event lịch sử tham chiếu.

<a id="m29-t081-s017"></a>

### 11. Cấp ID có tính xác định

Đề xuất cách đơn giản cho bản đầu:

**Persistent counter theo World + Entity Namespace.**

Ví dụ:

    character.000001
    character.000002
    item.000001
    item.000002

Những counter này thuộc authoritative state.

Quá trình tạo ID phải là một phần của transaction.

Nếu một transaction bị rollback, kết quả ID sau rollback phải tuân một chính sách xác định đã khóa. Cách đơn giản nhất cho replay là đưa allocation vào staged state và chỉ công bố khi commit thành công.

Không cấp identity bằng:

- thời gian máy;
- số ngẫu nhiên ngoài engine;
- tên nhân vật;
- thứ tự GameObject được Unity load.

<a id="m29-t081-s018"></a>

### 12. Entity Lifecycle

Các trạng thái tổng quát:

**Created:** Đã tồn tại trong committed world.

**Active:** Được mô phỏng chi tiết.

**Dormant:** Vẫn tồn tại nhưng sử dụng Simulation LOD thấp.

**Terminated:** Vòng đời hoạt động kết thúc, ví dụ character chết hoặc công trình bị phá.

**Archived:** Tiếp tục được giữ vì lịch sử.

Không phải mọi loại entity dùng cùng State Machine.

Ví dụ:

Một Character có `Alive/Dead`.

Một Shipment có `Preparing/InTransit/Delivered/Lost`.

Một Item có `Usable/Damaged/Destroyed`.

Registry chỉ theo dõi lifecycle tổng quát và identity.

Domain sở hữu trạng thái cụ thể.

<a id="m29-t081-s019"></a>

### 13. Không xóa vật lý dữ liệu quan trọng theo cách mất lịch sử

Nếu Mân qua đời:

- Character ID vẫn còn.
- Death Event vẫn còn.
- Những item từng thuộc về Mân vẫn lưu provenance.
- Một người khác có thể nhắc tới Mân.
- Chronicle vẫn tham chiếu đúng identity.

Nếu cần tối ưu dữ liệu sau nhiều năm, có thể archive hoặc compact các thuộc tính không còn cần cho simulation.

Nhưng không được làm mất thông tin lịch sử đã được các domain khác tham chiếu.

<a id="m29-t081-s020"></a>

### 14. Identity Quality Gate

Các test phải xác nhận:

- Một entity không được tạo hai lần cùng ID.
- Same seed, same event ordering tạo cùng chuỗi ID.
- Save/load giữ identity.
- NPC đã materialized không bị tạo lại khi quay lại vùng.
- Một entity chết không tự trở lại trạng thái hoạt động chỉ vì scene reload.
- Reference tới một archived entity vẫn có thể truy vấn lịch sử.

---

<a id="m29-t081-s021"></a>

## PHẦN IV — WORLD STATE & REVISION MODEL

<a id="m29-t081-s022"></a>

### 15. World State là nguồn sự thật authoritative

Đề xuất một `WorldStateRepository` ở cấp ứng dụng.

Nó cung cấp khả năng:

- đọc trạng thái tại revision hợp lệ;
- xác định entity version;
- kiểm tra preconditions;
- nhận Change Set;
- commit atomically;
- tạo snapshot;
- cung cấp state cho replay.

Không cho phép UI lấy trực tiếp một object mutable rồi sửa giá trị tùy ý.

<a id="m29-t081-s023"></a>

### 16. WorldRevision

Mỗi successful authoritative commit làm tăng WorldRevision theo thứ tự.

Ví dụ:

    Revision 100: BoatCondition = 420
    Revision 101: BoatCondition = 650

Event Journal có thể ghi:

    Event: BoatRepaired
    ResultingRevision: 101

Revision được sử dụng để:

- phát hiện stale command;
- xác định state trước/sau;
- kiểm tra save consistency;
- so sánh replay;
- xây projection.

Không nên sử dụng WorldTick làm revision.

Nhiều event có thể xảy ra trong cùng một WorldTick.

<a id="m29-t081-s024"></a>

### 17. Read-only Snapshot

Một Rule Handler nhận dữ liệu qua read-only snapshot.

Snapshot phải có một State Revision xác định.

Nếu rule được đánh giá trên revision 100 nhưng World đã commit sang revision 101, Command Processor phải kiểm tra các dữ liệu đã đọc có còn phù hợp không.

Trong single-writer engine, có thể đơn giản hóa bằng cách xử lý command tuần tự trên một ordered executor.

Dù vậy vẫn nên có `ExpectedRevision` để phát hiện:

- input từ UI đã cũ;
- kết quả tính từ background worker không còn hợp lệ;
- một command được gửi sau khi World State đã đổi.

<a id="m29-t081-s025"></a>

### 18. Staged Changes

Không nên cho Rule Handler thay đổi authoritative object rồi hy vọng rollback bằng cách sửa ngược.

Đề xuất:

    Authoritative State
           |
           v
     Read-only Snapshot
           |
           v
      Rule Evaluation
           |
           v
        Change Set
           |
           v
     Invariant Validation
           |
           v
      Durable Commit
           |
           v
     Publish New State

Nếu evaluation thất bại, bỏ Change Set.

Nếu persistence commit thất bại, authoritative state đang được công bố không được chuyển sang revision mới.

Đây là ranh giới quan trọng của tính nguyên tử.

<a id="m29-t081-s026"></a>

### 19. Change Set Contract

Một Change Set nên chứa:

- Origin Command ID.
- Read Revision.
- Affected Entities.
- Proposed Field/Entity Changes.
- Resource Transfers.
- Scheduled Jobs.
- Domain Events dự kiến.
- RNG Decisions đã tiêu thụ.
- Required Invariant Checks.

Không phải mọi thay đổi đều là một thao tác set field.

Ví dụ chuyển tài sản cần thể hiện một transaction chuyển quyền, không chỉ hai thao tác cộng/trừ inventory rời rạc.

<a id="m29-t081-s027"></a>

### 20. Transaction Scope

Đề xuất:

**Một player/NPC command được giải quyết và commit thành một logical transaction**, trong phạm vi các bất biến cần bảo đảm ngay.

Những hậu quả diễn ra sau đó được scheduled.

Ví dụ:

`AttackCommand` có thể commit ngay:

- attack outcome;
- injury state;
- equipment impact;
- incapacity/death nếu điều kiện xảy ra;
- spent actions;
- committed events.

Nhưng việc gia đình ở xa biết tin không nằm trong cùng immediate transaction.

Nó cần Communication/Scheduler Events hợp lệ.

Đây là cách tránh transaction khổng lồ kéo theo mọi domain của thế giới.

---

<a id="m29-t081-s028"></a>

## PHẦN V — WORLD CLOCK & TEMPORAL CONTRACT

<a id="m29-t081-s029"></a>

### 21. WorldClock phải có một chuẩn duy nhất

Đề xuất sử dụng một `WorldTick` số nguyên, không âm, tăng đơn điệu.

Mọi domain quy thời gian về cùng một trục.

Ví dụ:

- thời điểm character sinh;
- lúc bắt đầu một chuyến đi;
- thời điểm xảy ra Injury;
- deadline của Promise;
- thời điểm một shipment đến;
- thời gian mentor dạy nghề;
- event tới hạn.

**Tick không mặc nhiên là một giây thực.**

Thời lượng quy đổi cần được khai báo trong Calendar/Time Definition.

<a id="m29-t081-s030"></a>

### 22. Temporal Resolution

Vertical Slice có thể chọn một độ phân giải nội bộ ổn định đủ cho:

- combat;
- travel;
- healing;
- work;
- day transitions.

Nhưng một hoạt động kéo dài nhiều năm không được tạo hàng triệu vòng cập nhật vô ích nếu có thể xử lý chính xác bằng scheduled boundaries hoặc aggregate rules.

Đề xuất sử dụng:

**Canonical Absolute Time:** Một trục thời gian duy nhất.

**Domain-specific Time Step:** Độ phân giải xử lý tương ứng.

**Event Boundary:** Mốc không thể bỏ qua.

Ví dụ:

Một người đang hồi phục có lịch đánh giá mỗi giai đoạn.

Một hợp đồng tới hạn vào một ngày cụ thể.

Một shipment có thời gian đến dự kiến.

Scheduler phải xử lý đúng thứ tự những mốc này.

<a id="m29-t081-s031"></a>

### 23. WorldClock API

Các thao tác cần định nghĩa:

| Operation | Ý nghĩa |
|---|---|
| `GetCurrentTick` | Trả thời gian hiện tại |
| `GetCalendarDate` | Quy đổi tick sang thời gian hiển thị |
| `CalculateDuration` | Tính khoảng thời gian |
| `ScheduleAt` | Đăng ký event tại tick |
| `ScheduleAfter` | Đăng ký theo khoảng thời gian |
| `AdvanceTo` | Tiến world theo các event boundaries |
| `GetNextDueBoundary` | Tìm thời điểm xử lý gần nhất |

Không có `SetTimeTo(anyValue)` được gọi tùy tiện từ gameplay code.

Nếu cần debug time travel, công cụ phải sử dụng sandbox/test world riêng.

<a id="m29-t081-s032"></a>

### 24. Event Ordering

Trong cùng thời điểm, các job phải được sắp thứ tự xác định.

Đề xuất khóa thứ tự khóa:

    DueTick
    Phase
    Priority
    CreationSequence
    ScheduleID

`CreationSequence` được cấp trong authoritative transaction.

Không phụ thuộc:

- thứ tự dictionary;
- thứ tự GameObject;
- vị trí trong memory;
- thứ tự callback của Unity;
- thời điểm thread hoàn thành.

<a id="m29-t081-s033"></a>

### 25. Simulation Phase

Đề xuất các phase ban đầu:

**Boundary:** Xác lập mốc thời gian.

**Due Obligation:** Những cam kết và tác vụ đến hạn.

**Environment:** Những biến đổi thời tiết/sinh thái có lịch.

**Production & Health:** Tiến độ công việc, tiêu thụ, phục hồi.

**Actor Decisions:** NPC có điều kiện đánh giá lại hành động.

**Information & Narrative:** Xử lý phản ứng và truyền tin tới hạn.

**Validation & Projection:** Kiểm tra consistency và cập nhật read models.

Tuy nhiên, thứ tự phase không được áp dụng một cách máy móc khiến causal order sai.

Ví dụ NPC chết do một event đầu ngày thì không được tiếp tục làm việc vào cuối ngày.

Scheduler phải tái kiểm tra preconditions khi một job được thực thi.

<a id="m29-t081-s034"></a>

### 26. Time Skip Policy

Time Skip phải tuân:

1. Không bỏ qua Mandatory Scheduled Event.
2. Không giả định các điều kiện của cả giai đoạn giữ nguyên nếu có event làm thay đổi chúng.
3. Không tự tạo Skill nếu mentor đã chết giữa chừng.
4. Không tiêu thụ resource từ inventory đã không còn.
5. Không chuyển tri thức qua khoảng cách không có kênh liên lạc.
6. Không bỏ qua một event mà nhân vật phải đưa ra quyết định nếu có nguồn nhận biết phù hợp.

Các hoạt động có thể tổng hợp theo khoảng thời gian lớn nếu đã chứng minh equivalence hoặc mức sai số chấp nhận được.

<a id="m29-t081-s035"></a>

### 27. Exact Simulation vs Aggregate Simulation

Hai phương thức xử lý thời gian cần được phân biệt.

**Exact:** Những quy tắc có yêu cầu nhân quả và trạng thái chính xác ở mức actor/item.

**Aggregate:** Những biến nền có thể dùng mô hình thống kê theo thời gian.

Ví dụ:

Một named NPC đã ký lời hứa không được giải quyết bằng một phép cộng trung bình rồi mất thông tin deadline.

Nhưng sản lượng nghề nền của một nhóm dân cư không cần mô phỏng từng động tác lao động.

**Không được công bố hai phương thức luôn tạo state bit-by-bit giống nhau nếu thiết kế cho phép approximation.**

Phải xác định tập invariants và named outcomes bắt buộc tương đương.

<a id="m29-t081-s036"></a>

### 28. Temporal Quality Gate

Kiểm thử:

- Scheduler không xử lý event hai lần.
- Hai event cùng DueTick vẫn có thứ tự xác định.
- Time Skip không bỏ qua hạn chót quan trọng.
- Event bị mất điều kiện trước thời điểm chạy được hủy hoặc chuyển trạng thái hợp lệ.
- Không có event trong quá khứ được đăng ký vô ý.
- WorldClock không giảm.
- Save/load giữ được scheduler queue và trạng thái đang chờ.

---

<a id="m29-t081-s037"></a>

## PHẦN VI — DETERMINISTIC RANDOM ENGINE

<a id="m29-t081-s038"></a>

### 29. Mục tiêu của RNG

Không cần loại bỏ sự ngẫu nhiên.

Game cần randomness để tạo sự đa dạng.

Nhưng randomness phải:

- có nguồn;
- có ngữ cảnh;
- có thuật toán ổn định;
- không bị thay đổi bởi UI;
- có thể trace;
- không phá hard constraints;
- có thể tái hiện.

<a id="m29-t081-s039"></a>

### 30. PRNG Algorithm Contract

Đề xuất chọn một thuật toán PRNG đã được tài liệu hóa rõ và có thể implement đồng nhất, chẳng hạn **PCG32**, làm ứng viên cho prototype.

Cần xác định chính xác:

- state width;
- transition rule;
- stream/sequence selection;
- integer overflow semantics;
- seeding;
- bounded integer sampling;
- algorithm version.

Tên `PCG32` một mình chưa phải đặc tả đủ.

Đội triển khai phải chốt một biến thể cụ thể và bộ test vectors đi kèm.

Không dùng `System.Random` làm chuẩn tái hiện xuyên runtime/version.

<a id="m29-t081-s040"></a>

### 31. RNG Context

Logical schema:

| Trường | Vai trò |
|---|---|
| `world_seed` | Seed chiến dịch |
| `rng_algorithm_version` | Thuật toán |
| `domain_id` | Domain sử dụng |
| `decision_type` | Loại quyết định |
| `event_occurrence_id` | Sự kiện cụ thể |
| `entity_id` | Đối tượng liên quan |
| `draw_index` | Thứ tự draw trong quyết định |
| `stream_id` | Stream nếu dùng stateful RNG |

Một context phải được định danh **trước khi rút kết quả**.

Nếu hai event sửa thuyền khác nhau có cùng boat/actor, chúng vẫn cần occurrence identity khác để tránh nhận cùng random output ngoài chủ đích.

<a id="m29-t081-s041"></a>

### 32. Hai mô hình random

<a id="m29-t081-s042"></a>

#### Stateful Streams

Dùng cho chuỗi mô phỏng nền liên tục.

Ví dụ một population simulation tại settlement.

State/counter được persist.

<a id="m29-t081-s043"></a>

#### Keyed Random

Dùng cho một quyết định có ID cụ thể.

Ví dụ kết quả một Repair Action.

Input key gồm:

    WorldSeed
    Domain
    EventOccurrenceID
    DecisionType
    DrawIndex

Key phải được serialize/hashing bằng quy tắc ổn định, có domain separation và byte encoding xác định.

Không sử dụng chuỗi ghép tự do gây va chạm do thiếu delimiter/length.

<a id="m29-t081-s044"></a>

### 33. Bounded Integer Sampling

Khi cần chọn một số trong khoảng xác định, không đơn giản dùng `random % range` nếu muốn tránh modulo bias.

Đề xuất rejection sampling hoặc một thuật toán bounded sampling đã được kiểm chứng tương đương.

Đối với weighted choice:

- loại candidates không hợp lệ trước;
- sắp theo Stable ID;
- sử dụng integer weights không âm;
- tổng weights phải nằm trong giới hạn số học an toàn;
- nếu tổng bằng 0, trả `NoEligibleSelection`, không chọn bừa một candidate.

<a id="m29-t081-s045"></a>

### 34. PRNG Golden Vectors

Sau khi implementation được chọn, phải có bộ Golden Vectors gồm:

- seed;
- stream;
- số lượng draw;
- expected output;
- expected serialized state.

Chạy trên:

- Headless .NET runtime;
- Unity Editor;
- scripting backend và nền tảng mục tiêu.

Các giá trị cụ thể của Golden Vectors phải lấy từ implementation/reference đã được xác nhận, không tự bịa chúng trong GDD.

<a id="m29-t081-s046"></a>

### 35. Preview và Actual Resolution

Một preview chỉ được đọc những dữ liệu có thể biết và tính những giá trị được phép dự báo.

Không tiêu thụ outcome RNG.

Ví dụ:

UI có thể hiển thị:

> Khả năng sửa chữa chịu ảnh hưởng của vật liệu kém và tay nghề hiện tại.

Nó không được bí mật rút xúc xắc trước rồi thay đổi outcome khi player bấm nút xác nhận.

<a id="m29-t081-s047"></a>

### 36. Retry Policy

Nếu command đã commit, retry cùng Command ID phải trả về receipt của kết quả đã commit.

Nếu command bị từ chối trước execution, policy phải quy định:

- ID có được sử dụng lại không;
- có trả cùng rejection không;
- có cần Command ID mới khi trạng thái thay đổi không.

Đề xuất:

**Command ID là identity của một lần yêu cầu bất biến về payload.**

Cùng ID với payload khác phải trả lỗi conflict, không được coi là command mới.

<a id="m29-t081-s048"></a>

### 37. RNG Quality Gate

Test cần xác nhận:

- Cùng seed/context cho cùng output.
- Domain isolation hoạt động.
- Candidate ordering không làm outcome đổi.
- Retry không random lại.
- Save/load giữ RNG.
- Random không xảy ra khi precondition thất bại.
- Không có gameplay code gọi global RNG ngoài contract.

---

<a id="m29-t081-s049"></a>

## PHẦN VII — RULE HANDLER CONTRACT

<a id="m29-t081-s050"></a>

### 38. Rule Handler không phải Script tự do

Rule Handler là một thành phần code đã được phê duyệt.

Content có thể tham chiếu:

    rule.boat_repair.v1
    rule.resource_transfer.v1
    rule.skill_training.v1

Nhưng content không được đưa một đoạn chương trình mới vào engine.

Đặc biệt với nội dung LLM sinh offline, output phải qua Rule Reference Validation.

<a id="m29-t081-s051"></a>

### 39. Input Contract

Một Rule Handler cần nhận:

- typed Command Payload;
- Actor/Issuer Context;
- Read-only Snapshot;
- World Time;
- Rule Parameters;
- deterministic RNG;
- Required Capability/Authority Information.

Nếu cần truy vấn domain khác, phải qua read contract đã được cho phép.

Không để Handler tự mở database connection rồi tìm thông tin ngoài snapshot.

<a id="m29-t081-s052"></a>

### 40. Output Contract

Handler trả về:

**Rejected:** Kèm lỗi gameplay hoặc validation.

**Proposed Change Set:** Kết quả có thể commit.

**Scheduled Plan:** Hành động nhiều giai đoạn với các follow-up.

Không trả về:

- đoạn hội thoại tự do làm nguồn sự thật;
- một SQL string để execution layer chạy tùy ý;
- một yêu cầu gọi LLM để quyết định outcome.

<a id="m29-t081-s053"></a>

### 41. Mẫu hợp đồng thuật toán

    Evaluate(command, snapshot, context):
        ValidateInput()
        ValidateAuthority()
        ValidateHardPreconditions()

        ResolveOutcomeUsingRules()

        BuildProposedMutations()
        BuildDomainEvents()
        BuildScheduledFollowups()

        return CommandPlan

`Evaluate` phải là một thao tác thuần về mặt side effects đối với authoritative store.

Nếu cần RNG stateful, kết quả đánh giá phải bao gồm những RNG state changes được staged.

<a id="m29-t081-s054"></a>

### 42. Quy tắc xử lý hiệu ứng xuyên domain

Một handler không được tự ghi bất kỳ field nào nó thích.

Ví dụ RepairBoat:

- BoatCondition thuộc Equipment/Asset Condition Domain.
- Material Consumption thuộc Commodity/Asset Domain.
- Practice Evidence thuộc Character Development.
- Promise/Favor thuộc Social Domain.

Application Orchestrator phải phối hợp những operation đó theo các allowed effects đã khai báo.

Đề xuất dùng typed effect operations để validator biết owner của mỗi thay đổi.

Không dùng một dictionary `fieldName -> newValue` cho mọi loại domain mà không kiểm soát.

<a id="m29-t081-s055"></a>

### 43. Hard Constraint vs Probabilistic Result

Hai trạng thái này không được nhầm.

**Không đủ vật liệu:** Hành động bị từ chối hoặc chuyển sang một phương án được thiết kế.

**Đủ vật liệu nhưng tay nghề chưa chắc:** Hành động có thể thất bại sau resolution.

Nếu thất bại:

- tiêu hao vật liệu theo loại failure;
- mất thời gian;
- có khả năng tạo defect;
- vẫn giữ causal record.

Không rút RNG rồi trả `MissingMaterial` như một kết quả may rủi.

---

<a id="m29-t081-s056"></a>

## PHẦN VIII — COMMAND PROCESSOR

<a id="m29-t081-s057"></a>

### 44. Command Envelope

Đề xuất:

| Field | Bắt buộc | Vai trò |
|---|---|---|
| `command_id` | Có | Idempotency |
| `command_type` | Có | Định tuyến Handler |
| `issuer_id` | Có | Chủ thể phát lệnh |
| `source_type` | Có | Player/NPC/Scheduler |
| `world_tick` | Có | Thời điểm yêu cầu |
| `expected_revision` | Theo command | Kiểm tra stale state |
| `payload_version` | Có | Phiên bản schema |
| `payload` | Có | Dữ liệu hành động |
| `correlation_id` | Có | Chuỗi nguyên nhân |

`source_type` không phải giấy phép bỏ qua gameplay rules.

Một scheduled system command vẫn phải qua những invariants cần thiết.

<a id="m29-t081-s058"></a>

### 45. Command Lifecycle

Các trạng thái:

**Received**

**Validating**

**Rejected**

**Evaluating**

**Prepared**

**Committed**

**Scheduled**

**FailedBeforeCommit**

Không nên persist các trạng thái trung gian như thể là committed gameplay truth.

Những receipt đã hoàn tất cần được lưu theo transaction để xử lý retry.

<a id="m29-t081-s059"></a>

### 46. Idempotency

Ví dụ người chơi bấm xác nhận chuyển một item hai lần.

Cả hai request mang cùng Command ID nếu là retry của cùng hành động.

Lần đầu:

    TransferItem -> Committed

Lần sau:

    Same Command ID -> Return Stored Receipt

Không tiêu thụ item lần hai.

Nếu cùng Command ID nhưng payload khác:

    CommandPayloadMismatch

Không được coi đó là một transfer mới.

<a id="m29-t081-s060"></a>

### 47. Stale State

Ví dụ UI mở Inventory ở WorldRevision 100.

Trong lúc đó một scheduled event hoàn thành và vật phẩm đã được sử dụng.

Player gửi command dựa trên revision 100.

Processor phải:

- kiểm tra revision;
- đánh giá lại các điều kiện có liên quan;
- hoặc trả Conflict nếu operation không an toàn để tiếp tục.

Không được áp command trên giả định item vẫn còn.

<a id="m29-t081-s061"></a>

### 48. Error Taxonomy

Đề xuất bộ mã lỗi chuẩn.

| Mã | Ý nghĩa | Xử lý |
|---|---|---|
| `CMD_INVALID_PAYLOAD` | Payload không hợp lệ | Rejected |
| `CMD_DUPLICATE` | Command đã xử lý | Trả receipt |
| `CMD_PAYLOAD_MISMATCH` | Cùng ID, payload khác | Conflict |
| `STATE_STALE` | Revision cũ | Retry/re-evaluate theo policy |
| `ACTOR_INCAPABLE` | Actor không đủ khả năng | Rejected |
| `ACCESS_DENIED` | Không có quyền | Rejected |
| `RESOURCE_INSUFFICIENT` | Thiếu tài nguyên | Rejected |
| `RULE_NOT_FOUND` | Không tìm được handler | System/content error |
| `INVARIANT_VIOLATION` | Change Set trái luật | System error, không commit |
| `PERSISTENCE_FAILED` | Không ghi được | Không publish revision mới |
| `EVENT_CYCLE_DETECTED` | Vòng event bất thường | Stop chain, diagnostic |
| `REPLAY_MISMATCH` | Tái hiện sai | QA failure |

Thông báo player-facing phải phân biệt lỗi gameplay thông thường và lỗi kỹ thuật.

Không diễn giải một `PERSISTENCE_FAILED` thành chuyện nhân vật thất bại vì thiếu may mắn.

---

<a id="m29-t081-s062"></a>

## PHẦN IX — EVENT JOURNAL & CAUSAL TRACE

<a id="m29-t081-s063"></a>

### 49. Event là điều đã xảy ra, không phải điều được đề nghị

Đề xuất phân biệt dứt khoát:

**Command:** Yêu cầu.

**Scheduled Job:** Một hành động hoặc bước xử lý được lên lịch.

**Domain Event:** Một kết quả đã được xác lập.

**Observation:** Một actor nhận biết điều gì.

**Claim:** Một lời nói/thông tin được truyền.

**Story Thread:** Một trạng thái narrative đọc từ các event.

Cần tránh dùng cùng tên `Event` cho mọi loại.

<a id="m29-t081-s064"></a>

### 50. Domain Event Envelope

Các trường:

    EventID
    EventType
    AggregateID
    WorldTick
    CommitSequence
    CausationID
    CorrelationID
    RuleVersion
    PayloadSchemaVersion
    Payload

`CausationID` có thể trỏ tới Command ID hoặc một committed event tùy nguồn phát sinh.

`CorrelationID` nối các event thuộc cùng một chuỗi hoạt động lớn.

<a id="m29-t081-s065"></a>

### 51. Event Ordering

Trong một transaction tạo nhiều event, chúng cần thứ tự nội bộ xác định.

Ví dụ:

    BoatRepairStarted
    MaterialsConsumed
    BoatConditionChanged
    BoatRepairCompleted

Không nhất thiết phải lưu mọi step như bốn public domain events nếu một số chỉ là chi tiết kỹ thuật.

Nhưng nếu lưu nhiều event, thứ tự và quan hệ phải nhất quán với các mutations đã commit.

Một Event Journal không được tuyên bố item đã chuyển quyền trong khi Asset Rights chưa ghi nhận.

<a id="m29-t081-s066"></a>

### 52. Causal Graph

Ví dụ:

    SeasonalWaterRise
          |
          v
    RiverRiskIncreased
          |
          v
    TransportDelayed
          |
          v
    FoodShortagePressure
          |
          v
    HouseholdRequestsHelp
          |
          v
    PlayerNegotiatesShipment

Một developer phải truy được từ `HouseholdRequestsHelp` về nguyên nhân nguồn lực và đường vận chuyển.

Không cần ghi mọi biến đổi nhỏ trong journal mãi mãi, nhưng những bước nhân quả quan trọng cần có references.

<a id="m29-t081-s067"></a>

### 53. Event Fan-Out Policy

Một Domain Event có thể dẫn tới nhiều hậu quả.

Ví dụ:

`CharacterDied`

Các phản ứng tức thời bắt buộc:

- không thể hành động;
- không còn active party member;
- các hành động đang chờ bị đánh giá lại;
- physical state được xác lập.

Các phản ứng sau đó:

- Household nhận tin.
- Faction bổ nhiệm người khác.
- Inheritance diễn ra.
- Rumor lan.
- Chronicle cập nhật.

Đề xuất **không chạy đệ quy toàn bộ subscriber trong cùng call stack**.

Những reaction cần thời gian hoặc actor initiative phải được đặt vào scheduler.

<a id="m29-t081-s068"></a>

### 54. Idempotent Event Consumers

Một subscriber có thể nhận lại event khi recovery hoặc rebuild projection.

Cần định nghĩa:

- event đã xử lý được đánh dấu ra sao;
- idempotency key;
- loại effect nào có thể tái thực hiện;
- loại effect nào chỉ được commit một lần.

Ví dụ:

Event `MaterialsConsumed` không được làm kho vật liệu giảm hai lần khi projection rebuild.

Về kiến trúc, read projection có thể rebuild nhiều lần; authoritative resource mutation thì không.

<a id="m29-t081-s069"></a>

### 55. Event Journal không thay thế Command Log

Vì lựa chọn persistence là **Hybrid State + Critical Event Journal**, không được giả định có thể tái dựng mọi chi tiết World State bằng Event Journal một mình.

Nếu muốn deterministic replay, cần:

- initial snapshot;
- command sequence;
- scheduler inputs/decisions;
- RNG state hoặc deterministic context;
- ruleset/content version.

Đề xuất lưu hai loại nhật ký:

**Committed Event Journal:** Truy vết kết quả.

**Replay Command Log:** Truy vết yêu cầu và thứ tự thực thi.

Hai loại có thể được liên kết bằng Command ID, Causation ID và Commit Sequence.

---

<a id="m29-t081-s070"></a>

## PHẦN X — SCHEDULER & DURABLE WORKFLOW

<a id="m29-t081-s071"></a>

### 56. Scheduler cần quản lý gì?

Vertical Slice yêu cầu các hoạt động nhiều bước:

- đi từ Bến Vạc sang Bến Hạc;
- sửa thuyền;
- chữa trị;
- học nghề;
- hoàn thành hợp đồng;
- NPC tái đánh giá mục tiêu;
- gửi tin;
- Time Skip.

Scheduler phải hỗ trợ:

- đăng ký job;
- hủy job;
- đánh giá lại precondition;
- chạy tại DueTick;
- tạo follow-up;
- phục hồi sau save/load.

<a id="m29-t081-s072"></a>

### 57. Scheduled Job Contract

| Field | Vai trò |
|---|---|
| `schedule_id` | Identity |
| `job_type` | Loại xử lý |
| `due_tick` | Thời điểm |
| `phase` | Phase xử lý |
| `priority` | Ưu tiên trong phase |
| `creation_sequence` | Tie-break deterministic |
| `actor_id` | Actor liên quan |
| `origin_event_id` | Nguyên nhân |
| `payload` | Dữ liệu job |
| `precondition_policy` | Quy tắc đánh giá lại |
| `status` | Pending, Completed, Cancelled, Failed |

Không sử dụng timer thực của operating system làm gameplay scheduler.

<a id="m29-t081-s073"></a>

### 58. Job Execution

Khi tới DueTick:

1. Lấy job tiếp theo theo stable ordering.
2. Kiểm tra job chưa được xử lý.
3. Đọc World State hiện tại.
4. Đánh giá preconditions.
5. Tạo internal Command.
6. Chạy qua Command Processor.
7. Commit kết quả.
8. Đánh dấu job completed hoặc trạng thái phù hợp trong cùng transaction.
9. Đăng ký follow-ups nếu có.

Nếu actor đã chết, job cần bị hủy hoặc xử lý fallback đã khai báo.

Không tiếp tục thực thi chỉ vì khi job được đăng ký actor vẫn còn sống.

<a id="m29-t081-s074"></a>

### 59. Ví dụ: Training

Một character đăng ký học nghề trong ba tháng.

Scheduler lưu:

- trainee;
- mentor;
- workshop;
- learning activity;
- interval boundaries;
- chi phí thời gian;
- resource requirements.

Sau một tháng mentor qua đời.

Engine phải:

- ghi nhận Death Event;
- thay đổi khả năng training;
- đánh giá lại công việc đang chạy;
- có thể gián đoạn, tìm mentor khác hoặc chuyển sang self-practice nếu rules cho phép.

Không tự cộng đủ Skill Progress của ba tháng như điều kiện ban đầu chưa từng thay đổi.

<a id="m29-t081-s075"></a>

### 60. Scheduler Quality Gate

- Job được thực thi tối đa một lần theo semantics đã định.
- Retry không nhân đôi hậu quả.
- Thứ tự jobs ổn định.
- Job bị mất precondition được xử lý hợp lệ.
- Save/load không làm mất pending jobs.
- Time Skip không bỏ qua mandatory job.
- Chain tạo job mới không gây vòng lặp vô hạn.

---

<a id="m29-t081-s076"></a>

## PHẦN XI — PERSISTENCE ARCHITECTURE

<a id="m29-t081-s077"></a>

### 61. Persistence Strategy

Giữ phương án:

**Authoritative Current State + Critical Event Journal + Periodic Snapshots + Replay Command Log + Rebuildable Projections.**

Đây không phải full event sourcing.

Mục tiêu:

- load gameplay nhanh;
- bảo vệ tính nhất quán;
- cho phép audit;
- hỗ trợ debugging;
- đủ dữ liệu để kiểm tra các thay đổi nhiều năm.

<a id="m29-t081-s078"></a>

### 62. Logical Storage Model

Đề xuất các nhóm dữ liệu:

| Nhóm | Nội dung |
|---|---|
| World Metadata | Seed, versions, current tick/revision |
| Entity Registry | Identity và lifecycle |
| Authoritative State | Domain-owned aggregates |
| Asset/Resource Ledger | Ownership, custody, quantities |
| Scheduled Jobs | Các tác vụ còn chờ |
| RNG State | Stateful streams/counters |
| Command Receipts | Idempotency và committed outcomes |
| Event Journal | Causal history |
| Snapshots | Checkpoints |
| Knowledge Records | Tri thức theo actor |
| Projection Data | Read models có thể rebuild |

Không nhất thiết tạo một SQL table cho mỗi logical type ở prototype đầu tiên.

Nhưng logical schema phải thể hiện rõ đâu là authoritative và đâu là derived.

<a id="m29-t081-s079"></a>

### 63. SQLite Transaction Boundary

Một commit của gameplay có thể bao gồm:

- cập nhật State;
- cấp Entity ID mới;
- cập nhật WorldRevision;
- lưu RNG state/counter;
- ghi Event Journal;
- ghi Command Receipt;
- thêm/hủy Scheduled Job.

**Tất cả thành phần cần nhất quán với nhau phải nằm trong cùng một database transaction.**

Nếu một bước thất bại, không được commit phần còn lại.

<a id="m29-t081-s080"></a>

### 64. Thứ tự commit đề xuất

Pseudo-flow:

    Acquire single-writer slot
    Read committed revision
    Validate command
    Evaluate rule on read-only state
    Prepare staged mutations
    Validate invariants

    Begin database transaction
        Verify expected revision
        Write entity/aggregate changes
        Write scheduled jobs
        Write RNG state
        Write domain events
        Write command receipt
        Advance world revision
    Commit database transaction

    Publish new in-memory authoritative state
    Notify projection processors

Nếu commit thất bại, không publish revision mới.

Đối với một in-memory simulation chạy trước khi save, cần có cơ chế staging/rollback tương đương về mặt logic, không để in-memory state thay đổi rồi save thất bại nhưng engine coi transaction đã hoàn tất.

<a id="m29-t081-s081"></a>

### 65. Persistence Adapter Contract

Các thao tác chính:

- `LoadWorldMetadata`
- `LoadCurrentState`
- `BeginTransaction`
- `CommitChangeSet`
- `Rollback`
- `SaveSnapshot`
- `LoadSnapshot`
- `AppendCommittedEvents`
- `LoadCommandReceipts`
- `LoadScheduledJobs`
- `VerifyIntegrity`
- `RunMigration`

Application Layer không cần biết SQL cụ thể.

SQLite Adapter chịu trách nhiệm persistence implementation.

<a id="m29-t081-s082"></a>

### 66. SQLite Configuration Notes

Nếu chọn SQLite:

- Bật và xác minh foreign key enforcement trên các connection cần thiết.
- Sử dụng explicit transactions.
- Có chính sách xử lý `SQLITE_BUSY`.
- Xác định journal mode và durability policy.
- Tránh backup bằng cách sao chép tùy tiện file đang mở.
- Có cách kiểm tra database integrity.
- Kiểm tra provider/native library trên các platform mục tiêu.

Các tham số như `busy_timeout`, `synchronous` và WAL checkpoint policy thuộc cấu hình triển khai cần kiểm chứng, không nên khóa thành con số tùy tiện trong GDD.

<a id="m29-t081-s083"></a>

### 67. World Save vs Manual Save

Cần phân biệt:

**Simulation Commit:** Một gameplay transaction được xác lập.

**Player Save:** Một checkpoint để người chơi có thể load.

Các thay đổi trong runtime phải luôn nhất quán ngay cả khi player chưa bấm Save.

Nếu sử dụng in-memory runtime và chỉ flush định kỳ, phải có chính sách crash recovery rõ về khoảng dữ liệu có thể mất.

Không được dùng từ “đã commit” để hứa dữ liệu đã durable trên disk nếu hệ thống thực tế mới chỉ cập nhật memory.

<a id="m29-t081-s084"></a>

### 68. Save Manifest

Một Save phải ghi tối thiểu:

    SaveID
    WorldID
    WorldSeed
    SaveFormatVersion
    SchemaVersion
    RulesetVersion
    ContentManifestHash
    RandomAlgorithmVersion
    SimulationVersion
    WorldTick
    WorldRevision
    LastCommitSequence
    ActiveCharacterID
    SnapshotReference
    IntegrityMetadata

Không được để player load một save dùng content definitions đã thay đổi đáng kể mà hệ thống không phát hiện.

---

<a id="m29-t081-s085"></a>

## PHẦN XII — SNAPSHOT, RECOVERY & MIGRATION

<a id="m29-t081-s086"></a>

### 69. Snapshot là gì?

Snapshot là bản chụp authoritative state tại một revision nhất quán.

Một snapshot cần đủ dữ liệu để tiếp tục simulation:

- entities và domain state;
- WorldClock;
- scheduler;
- RNG states;
- relevant metadata;
- knowledge state;
- narrative state;
- current historical state.

Không cần snapshot các UI animations hoặc tooltip cache.

<a id="m29-t081-s087"></a>

### 70. Snapshot Interval

Không chốt số phút hoặc số event cố định khi chưa benchmark.

Thay vào đó, hỗ trợ trigger snapshot theo:

- manual save;
- checkpoint;
- historical transition;
- số transaction từ snapshot gần nhất;
- dung lượng journal;
- thời gian xử lý recovery;
- trước schema migration.

Chọn interval phải dựa trên dữ liệu thực đo về:

- size;
- write cost;
- load duration;
- recovery time.

<a id="m29-t081-s088"></a>

### 71. Recovery Flow

Khi khởi động và load:

1. Xác nhận Save Manifest.
2. Kiểm tra content/ruleset compatibility.
3. Đọc snapshot ở revision hợp lệ.
4. Đọc các committed records cần thiết.
5. Khôi phục World State.
6. Khôi phục WorldClock, scheduler, RNG.
7. Kiểm tra consistency.
8. Rebuild những read projections cần thiết.
9. Chỉ khi hợp lệ mới cho player tiếp tục.

Nếu phát hiện lỗi:

Không được âm thầm tạo lại NPC hoặc reset tài sản.

Có thể:

- từ chối load;
- thử recovery từ snapshot trước;
- tạo diagnostic report;
- đưa ra thông báo phù hợp.

<a id="m29-t081-s089"></a>

### 72. State Hash

Đề xuất dùng canonical state serialization và hash cho replay/testing.

Serialization cần:

- thứ tự entities ổn định;
- thứ tự fields ổn định;
- normalized strings;
- numeric encoding nhất quán;
- không có nondeterministic metadata;
- không phụ thuộc iteration order.

Hash phải tính trên **authoritative state**.

Không đưa vào:

- thời gian máy;
- debug log timestamps;
- animation;
- temporary cache;
- UI state không thuộc gameplay.

<a id="m29-t081-s090"></a>

### 73. Save Migration

Các loại thay đổi:

**Schema Migration:** Đổi cấu trúc lưu trữ.

**Content Migration:** ID hoặc definition được thay thế.

**Ruleset Migration:** Quy tắc mới áp dụng cho tương lai.

**Historical Data Correction:** Sửa những dữ liệu quá khứ có lỗi.

Không nên mặc định replay toàn bộ lịch sử với ruleset mới.

Một nhân vật đã chết bởi một event được commit trước đó không tự sống lại sau patch chỉ vì công thức Health được thay.

Các migration cần:

- version source;
- version target;
- preconditions;
- transformation;
- invariant validation;
- rollback/recovery policy;
- test fixtures.

<a id="m29-t081-s091"></a>

### 74. Recovery Quality Gate

- Snapshot có thể load đúng.
- Save giữa nhiều event không tạo half-committed state.
- Scheduled Jobs không mất.
- RNG tiếp tục đúng.
- Entity IDs không bị cấp trùng.
- Command Receipt tồn tại sau load.
- Không nhân đôi tài nguyên.
- Có thể phát hiện content incompatibility.
- State Hash phù hợp với expected snapshot/replay trong test.

---

<a id="m29-t081-s092"></a>

## PHẦN XIII — CONTENT REGISTRY & RULE REGISTRATION

<a id="m29-t081-s093"></a>

### 75. Tách authoring content khỏi runtime rules

Content Pack lưu:

- Event Template;
- Character Archetype;
- Profession Definition;
- Dialogue Definition;
- Settlement Definition;
- Item Definition;
- Historical Anchor Definition;
- parameters.

Runtime Rule Registry chứa những handler được compile và kiểm thử.

Một Event Template có thể tham chiếu:

    rule.boat.repair.v1

Nhưng không thể thêm một đoạn code mới vào engine chỉ bằng text.

<a id="m29-t081-s094"></a>

### 76. Content Manifest

Một Content Pack cần:

    ContentPackID
    Version
    SchemaVersion
    Dependencies
    DefinitionIDs
    RuleReferences
    HistoricalClassification
    ContentHash
    ValidationMetadata

Nếu một required rule không tồn tại, pack không được load thành công.

Nếu một character archetype tham chiếu profession chưa được khai báo, validator phải báo lỗi.

<a id="m29-t081-s095"></a>

### 77. Schema Validation và Semantic Validation

Hai bước này khác nhau.

**Schema Validation** kiểm tra:

- field;
- type;
- required attributes;
- enum;
- format;
- cấu trúc.

**Semantic Validation** kiểm tra:

- references;
- domain ownership;
- event eligibility;
- resource effects;
- actor roles;
- historical constraints;
- duplicate content;
- unreachable states.

JSON Schema có thể hỗ trợ lớp cấu trúc.

Nhưng không tự chứng minh một sự kiện có nhân quả hợp lý.

<a id="m29-t081-s096"></a>

### 78. Rule Registry API

Một registry phải cung cấp:

- `RegisterRule`
- `ResolveRule`
- `ValidateRuleReference`
- `GetRuleVersion`
- `ListAllowedEffects`

Với bản runtime chính thức, registry được dựng từ những handler đã được code.

Không có `RegisterArbitraryScriptFromLLM`.

<a id="m29-t081-s097"></a>

### 79. Content Loading Policy

Khi startup:

1. Đọc manifest.
2. Kiểm tra content version.
3. Kiểm tra schema.
4. Kiểm tra Definition IDs.
5. Resolve dependencies.
6. Validate rule references.
7. Compile/load definitions.
8. Xác nhận content hash.
9. Cho phép tạo/load world.

Nếu nội dung lỗi, báo lỗi tại content loading.

Không chờ đến khi player vào một settlement sau mười năm mới phát hiện một required rule chưa tồn tại.

---

<a id="m29-t081-s098"></a>

## PHẦN XIV — FOUNDATION OBSERVABILITY

<a id="m29-t081-s099"></a>

### 80. Vì sao debug tooling phải có từ đầu?

Trong game này, lỗi thường không biểu hiện trực tiếp tại nơi gây ra.

Ví dụ:

Một NPC không xuất hiện trong quest.

Nguyên nhân thật có thể là:

- NPC đã chết;
- NPC đang ở vùng khác;
- scheduler không kích hoạt;
- actor không biết thông tin;
- event không đủ điều kiện;
- một household không có đủ tài nguyên;
- World Director chưa expose.

Nếu thiếu trace, developer dễ sửa nhầm Quest System dù lỗi nằm ở Economy hoặc Knowledge.

<a id="m29-t081-s100"></a>

### 81. Command Trace

Mỗi command quan trọng cần tra được:

- ID;
- issuer;
- source;
- world revision;
- payload;
- preconditions;
- handler;
- random context;
- outcome;
- effects;
- committed events;
- error.

Không cần lưu toàn bộ trace chi tiết trong release build vô hạn.

Nhưng phải có khả năng bật trace trong developer/test build.

<a id="m29-t081-s101"></a>

### 82. Rule Inspector

Một rule cần giải thích được:

    Rule: boat.repair.v1
    Actor: Vạc Mân
    Target: Thuyền nhà Tòng

    Access: PASS
    Tool: PASS
    Material: PASS
    Character capable: PASS

    Skill Modifier: +5
    Attribute Modifier: +1
    Preparation: +2
    Difficulty: 20

    Roll: 12
    Total: 20
    Outcome: Success

    Boat Condition: Updated
    Materials: Consumed
    Event: Committed

Các giá trị trên là ví dụ về cách trình bày trace, không phải kết quả đã chạy.

Trace phải lấy từ evaluation thật.

<a id="m29-t081-s102"></a>

### 83. Scheduler Inspector

Cần truy được:

- pending jobs;
- due tick;
- actor;
- origin event;
- status;
- lý do hủy;
- follow-up jobs.

Ví dụ:

Một chuyến hàng chưa đến.

Developer cần biết:

- còn đang đi;
- bị hoãn do thời tiết;
- bị hủy vì phương tiện hỏng;
- hay job đã bị mất do bug.

<a id="m29-t081-s103"></a>

### 84. State Diff

Một State Diff phải cho biết:

**Before Revision → After Revision.**

Ví dụ:

    BoatCondition: 420 -> 650
    WoodStock: 20 -> 17
    CharacterPractice: increased
    WorldTick: 100 -> 106

Không chỉ in một thông báo:

    Repair complete

Nếu có resource biến mất, diff giúp xác định event nào đã thay đổi.

<a id="m29-t081-s104"></a>

### 85. Causal Trace

Một developer có thể bắt đầu từ:

`FoodShortagePressure`

và truy ngược:

    ShipmentDelayed
       ← RouteClosed
       ← BridgeDamaged
       ← FloodPressure

Ngược lại, cũng có thể xem downstream effects của `BridgeDamaged`.

Không cần đồ họa node editor trong Foundation MVP.

Một CLI xuất causal tree đã đủ kiểm chứng nguyên lý.

---

<a id="m29-t081-s105"></a>

## PHẦN XV — HEADLESS RUNNER & REPLAY HARNESS

<a id="m29-t081-s106"></a>

### 86. Headless Runner phải làm được gì?

Các chức năng:

- khởi tạo world từ seed/content fixture;
- load snapshot;
- gửi command;
- advance world time;
- xử lý scheduler;
- chạy NPC simulation;
- dump world state;
- export event log;
- replay command sequence;
- tính state hash;
- chạy consistency checks;
- chạy benchmark.

Không cần tạo window hoặc scene.

<a id="m29-t081-s107"></a>

### 87. Replay Inputs

Một Replay Fixture cần:

    InitialSnapshot
    WorldSeed
    RulesetVersion
    ContentManifestHash
    RandomAlgorithmVersion
    SimulationVersion
    OrderedCommands
    ExpectedEvents
    ExpectedStateHash

Nếu test dùng scheduler, phải ghi được cấu hình và thứ tự xử lý.

<a id="m29-t081-s108"></a>

### 88. Hai loại replay cần phân biệt

**Deterministic Command Replay:** Chạy lại cùng commands từ cùng initial state.

**Event Journal Inspection:** Xem các sự kiện đã xảy ra.

Chỉ Event Journal Inspection không chứng minh simulation deterministic.

Muốn kiểm tra rule, phải rerun resolution.

Đây là một ranh giới cần được ghi rõ trong tài liệu QA.

<a id="m29-t081-s109"></a>

### 89. Golden Replay Fixture

Đề xuất một fixture nhỏ:

- hai character;
- một household;
- một item;
- một commodity;
- một route;
- một repair command;
- một scheduled activity.

Command sequence:

1. Chuyển vật liệu.
2. Bắt đầu sửa thuyền.
3. Advance time.
4. Hoàn thành sửa.
5. Chuyển quyền sử dụng thuyền.
6. Save.
7. Load.
8. Replay.

Expected:

- item ownership đúng;
- material không nhân đôi;
- BoatCondition đúng;
- scheduler không còn job completed;
- command receipt tồn tại;
- state hash giống.

<a id="m29-t081-s110"></a>

### 90. Randomized Property Testing

Ngoài Golden Fixture cố định, cần sinh nhiều tổ hợp hợp lệ.

Ví dụ:

Hai household trao đổi vật phẩm.

Các biến thể:

- item thuộc household;
- item thuộc cá nhân;
- item đang được mượn;
- người nhận không có capacity;
- owner chết;
- command bị gửi lại;
- transaction bị ngắt.

Sau mỗi test, kiểm tra invariants.

Property testing quan trọng vì full game có state space quá lớn để liệt kê bằng tay.

---

<a id="m29-t081-s111"></a>

## PHẦN XVI — FOUNDATION ACCEPTANCE TESTS

<a id="m29-t081-s112"></a>

### 91. Nhóm A — Identity

**A01:** Entity ID duy nhất.

**A02:** Definition ID không bị nhầm với Entity ID.

**A03:** Entity ID ổn định qua save/load.

**A04:** Materialized NPC không được tạo lại.

**A05:** Entity đã chết/phá hủy vẫn truy xuất lịch sử.

**A06:** ID allocation không thay đổi bất thường khi retry một command đã commit.

<a id="m29-t081-s113"></a>

### 92. Nhóm B — Commands & Transactions

**B01:** Command hợp lệ có thể commit.

**B02:** Command thiếu precondition bị từ chối.

**B03:** Rejected command không thay state.

**B04:** Retry cùng command không tạo thêm outcome.

**B05:** Cùng Command ID nhưng khác payload trả conflict.

**B06:** Transaction thất bại không tạo partial effects.

**B07:** Mỗi changed field có domain owner hợp lệ.

**B08:** WorldRevision chỉ tăng theo successful commit policy.

<a id="m29-t081-s114"></a>

### 93. Nhóm C — Random

**C01:** Same context cho same result.

**C02:** Different domain streams không làm nhau thay đổi ngoài chủ đích.

**C03:** Same seed không phụ thuộc UI frame count.

**C04:** Preview không consume outcome RNG.

**C05:** Save/load giữ state/counter.

**C06:** Random algorithm version được lưu.

**C07:** Golden vectors khớp trên các runtime mục tiêu.

<a id="m29-t081-s115"></a>

### 94. Nhóm D — Time & Scheduler

**D01:** Event đến hạn được thực thi.

**D02:** Event chỉ được commit một lần.

**D03:** Stable ordering.

**D04:** Preconditions được kiểm tra lại.

**D05:** Actor chết làm jobs không hợp lệ bị xử lý đúng.

**D06:** Time Skip không bỏ qua event bắt buộc.

**D07:** Save/load giữ queue.

**D08:** Không tạo infinite same-tick event chain.

<a id="m29-t081-s116"></a>

### 95. Nhóm E — Persistence & Recovery

**E01:** Save chứa metadata cần thiết.

**E02:** Snapshot nhất quán.

**E03:** Event Journal khớp state revision.

**E04:** Command Receipts được giữ.

**E05:** Crash trong transaction không để lại half-commit.

**E06:** Save incompatible được phát hiện.

**E07:** Migration phải có validation.

**E08:** State Hash tái hiện trong replay.

<a id="m29-t081-s117"></a>

### 96. Nhóm F — Content & Dependency

**F01:** Rule Reference không tồn tại bị từ chối.

**F02:** Duplicate Definition ID bị phát hiện.

**F03:** Content schema invalid không được load.

**F04:** Domain không phụ thuộc Unity.

**F05:** Runtime không gọi LLM để quyết định gameplay.

**F06:** Content không được thực thi arbitrary code.

**F07:** Historical/Canon-critical content có metadata phân loại.

<a id="m29-t081-s118"></a>

### 97. Nhóm G — Integration Smoke Tests

**G01 — Repair:** Item Condition + Resource + Time + Event.

**G02 — Travel:** Route + Clock + Scheduler + Arrival.

**G03 — Death:** Character Life + Pending Work Cancellation.

**G04 — Knowledge:** Một fact không tự tới player nếu chưa có nguồn.

**G05 — Succession:** Không copy Skill, Item hoặc Knowledge trái quy tắc.

**G06 — Historical:** Một scenario không được tạo resource miễn phí.

Các nhóm G sẽ được triển khai đầy đủ sau Foundation, nhưng Foundation phải cung cấp contracts để chạy chúng.

---

<a id="m29-t081-s119"></a>

## PHẦN XVII — FOUNDATION ERROR & FAILURE RECOVERY

<a id="m29-t081-s120"></a>

### 98. Phân biệt Gameplay Rejection và System Failure

Một character không đủ vật liệu để sửa thuyền:

**Gameplay Rejection.**

Một Rule Handler thiếu reference:

**Content/System Failure.**

Một transaction không ghi được SQLite:

**Infrastructure Failure.**

Một replay cho state khác:

**Determinism Failure.**

Không nên xử lý tất cả bằng `false` hoặc exception chung.

<a id="m29-t081-s121"></a>

### 99. Error Policy

Mỗi lỗi có:

- Error Code.
- Severity.
- Domain.
- Command/Event Reference.
- Human-readable Diagnostic.
- Recovery Policy.
- Có hiển thị cho player hay chỉ developer.

Ví dụ:

`RESOURCE_INSUFFICIENT` có thể được hiển thị cho player.

`INVARIANT_VIOLATION` cần chặn commit và ghi diagnostic.

`REPLAY_MISMATCH` là QA/blocker, không phải một event gameplay.

<a id="m29-t081-s122"></a>

### 100. Lỗi khi đang commit

Nếu transaction lỗi:

1. Rollback database transaction.
2. Không publish authoritative state mới.
3. Không gửi side effects như outcome đã thành công.
4. Ghi diagnostic ngoài World Event Journal nếu cần.
5. Giữ nguyên command receipt policy.
6. Cho phép retry theo chính sách rõ.

Không được dùng narrative fallback để che state mismatch.

<a id="m29-t081-s123"></a>

### 101. Lỗi Scheduled Job

Nếu một scheduled job không còn hợp lệ vì thế giới đã thay đổi:

Đó có thể là tình huống gameplay bình thường.

Ví dụ chuyến đi bị hủy vì thuyền bị phá.

Scheduler có thể commit:

`TravelCancelledDueToUnavailableTransport`.

Nhưng nếu handler không tồn tại, đó là lỗi content.

Không được gộp hai trường hợp.

---

<a id="m29-t081-s124"></a>

## PHẦN XVIII — FOUNDATION IMPLEMENTATION TASK SPECIFICATION

<a id="m29-t081-s125"></a>

### 102. Work Breakdown

Đề xuất các gói công việc bàn giao.

| Task ID | Thành phần | Dependency | Kết quả cần bàn giao |
|---|---|---|---|
| FND-01 | Project/Assembly Contracts | Không | Dependency boundaries |
| FND-02 | Typed IDs & Entity Registry | FND-01 | Identity contract |
| FND-03 | WorldClock & Calendar | FND-01 | Canonical time |
| FND-04 | Deterministic RNG | FND-01 | PRNG version + golden tests |
| FND-05 | World State/Revision | FND-02 | Snapshot & state semantics |
| FND-06 | Rule Registry | FND-01, FND-05 | Handler resolution |
| FND-07 | Command Processor | FND-04–06 | Validation/evaluation |
| FND-08 | Change Set & Invariants | FND-07 | Transaction preparation |
| FND-09 | Domain Event Model | FND-07 | Causal event contract |
| FND-10 | Scheduler | FND-03, FND-07 | Durable temporal jobs |
| FND-11 | Persistence Adapter | FND-05, FND-08–10 | Atomic state storage |
| FND-12 | Snapshot/Recovery | FND-11 | Consistent save/load |
| FND-13 | Replay Harness | FND-04, FND-07, FND-12 | Determinism validation |
| FND-14 | Content Registry | FND-06 | Schema/reference checking |
| FND-15 | Foundation Diagnostics | FND-07–14 | Trace/state diff |
| FND-16 | Cross-Runtime Compatibility | FND-13 | Unity/headless validation |

Không nhất thiết phải triển khai tuần tự tuyệt đối theo số thứ tự.

Nhưng public contracts của những task phụ thuộc phải ổn định trước khi nhiều agent thực hiện song song.

<a id="m29-t081-s126"></a>

### 103. Task Template

Mỗi task được giao trong giai đoạn implementation phải có:

**Purpose:** Mục tiêu.

**Owned Area:** Files/modules được phép sửa.

**Required Inputs:** Contracts và dependencies.

**Required Outputs:** Public API, state, events hoặc diagnostics.

**Forbidden Dependencies:** Những thành phần không được gọi.

**Acceptance Tests:** Điều kiện tự động.

**Error Handling:** Các failure modes.

**Documentation:** Quy tắc cần ghi.

**Evidence:** Kết quả build/test.

Ví dụ FND-04 không chỉ ghi:

> Xây RNG service.

Phải ghi rõ:

- version;
- streams;
- keyed random;
- bounded sampling;
- serialization;
- golden vectors;
- retry behavior;
- test cross-runtime.

<a id="m29-t081-s127"></a>

### 104. Quy tắc dành cho coding agent

Coding agent chỉ được thực hiện công việc trong Task Contract.

Không tự:

- thêm multiplayer architecture;
- dùng microservices;
- thay C# core bằng một framework không được chấp thuận;
- thêm LLM runtime;
- cho UI sửa World State;
- thay WorldClock bằng system time;
- tạo global Random;
- thay đổi gameplay formulas ngoài scope;
- viết Quest System khi đang làm Foundation.

Nếu phát hiện thiếu thiết kế, cần tạo Design Gap Record.

<a id="m29-t081-s128"></a>

### 105. Design Gap Record

Mẫu:

    GapID
    Title
    AffectedContracts
    MissingDecision
    Options
    Impact
    RecommendedResolution
    BlockingLevel
    Owner
    Status

Một gap P0 phải được giải quyết trước khi implementation phụ thuộc tiếp tục.

Một gap về tối ưu performance có thể được để tới benchmark nếu không làm thay đổi correctness contracts.

---

<a id="m29-t081-s129"></a>

## PHẦN XIX — FOUNDATION EXIT GATE

<a id="m29-t081-s130"></a>

### 106. Điều kiện coi Foundation Design đã đủ

Ở cấp tài liệu, phải có:

- architecture boundary;
- identity semantics;
- authoritative state ownership;
- command lifecycle;
- rule execution contract;
- transaction semantics;
- event ordering;
- scheduler;
- deterministic RNG;
- persistence;
- snapshot/recovery;
- content registry;
- versioning;
- error taxonomy;
- automated test plan;
- task breakdown.

Module này cung cấp baseline cho những thành phần đó.

**Nó chưa chứng minh implementation đã đạt chất lượng.**

<a id="m29-t081-s131"></a>

### 107. Điều kiện Foundation Code được nghiệm thu sau này

Đội phát triển phải chứng minh:

Một Character A có quyền sở hữu vật phẩm.

Một Character B yêu cầu nhận vật phẩm.

Engine xử lý quyền, tạo transfer, commit, log, save, load và replay đúng.

Một command retry không nhân đôi item.

Một command khác thay đổi dữ liệu từ snapshot cũ bị xử lý an toàn.

Một scheduled job vẫn chạy đúng sau load.

Cùng input tạo cùng authoritative state.

Không sử dụng Unity scene hoặc LLM trong quá trình quyết định.

Đây là proof đầu tiên trước khi xây Living Settlement.

<a id="m29-t081-s132"></a>

### 108. Những lựa chọn chưa được tuyên bố là đã kiểm chứng

Các vấn đề cần để dành cho kỹ thuật triển khai:

- phiên bản Unity chính thức;
- .NET provider SQLite;
- PRNG implementation và golden vectors;
- physical database schema;
- transaction throughput;
- snapshot interval;
- memory allocation policy;
- exact WorldTick resolution;
- target platform matrix;
- scheduler batch size;
- performance targets.

Những điều này không cần tự bịa con số chỉ để khiến tài liệu trông hoàn chỉnh.

Cần ghi **decision criteria** và **verification plan** để đội triển khai chốt dựa trên đo đạc.

---

<a id="m29-t081-s133"></a>

## PHẦN XX — ARCHITECTURE DECISIONS ĐỀ XUẤT KHÓA

<a id="m29-t081-s134"></a>

### 109. Foundation Decisions

**ADR-029-01:** Foundation là một headless library, độc lập Unity.

**ADR-029-02:** Mọi committed entity có stable ID.

**ADR-029-03:** Definition ID và Entity ID là hai loại khác nhau.

**ADR-029-04:** Entity ID allocation thuộc authoritative transaction.

**ADR-029-05:** WorldTick là thời gian authoritative duy nhất.

**ADR-029-06:** Scheduler sử dụng stable event ordering.

**ADR-029-07:** Command là yêu cầu; Domain Event là kết quả đã commit.

**ADR-029-08:** Mọi authoritative mutation đi qua Command Processor hoặc một internal system workflow có cùng consistency guarantees.

**ADR-029-09:** Rule Handler chỉ tạo proposed changes, không tự ghi database.

**ADR-029-10:** Hard constraints được kiểm tra trước probabilistic resolution.

**ADR-029-11:** Random algorithm và context phải được version hóa.

**ADR-029-12:** UI và read projections không tiêu thụ gameplay RNG.

**ADR-029-13:** Persistence commit đồng bộ state, event, scheduler, RNG và receipt theo transaction boundary.

**ADR-029-14:** Sử dụng Hybrid Current State + Event Journal + Snapshots.

**ADR-029-15:** Replay Command Log được phân biệt với Event Journal.

**ADR-029-16:** Knowledge có authoritative owner và provenance.

**ADR-029-17:** Content chỉ được tham chiếu approved handlers.

**ADR-029-18:** Không có arbitrary code execution từ LLM-generated content.

**ADR-029-19:** Headless Runner và deterministic replay là thành phần kiểm thử bắt buộc.

**ADR-029-20:** Foundation chỉ hoàn thành khi có bằng chứng kiểm thử, không chỉ compile thành công.

---

<a id="m29-t081-s135"></a>

## PHẦN XXI — DOCUMENTATION COMPLETENESS & STOP CRITERIA

<a id="m29-t081-s136"></a>

### 110. Mục đích của bộ tài liệu

Dự án này đang ở giai đoạn **tiền sản xuất: nghiên cứu, phát triển ý tưởng, lập GDD và thiết kế kỹ thuật**.

Mục tiêu không phải liên tục tăng số Module.

Mục tiêu là tạo một bộ tài liệu để khi bàn giao, người triển khai có thể trả lời:

- Game này là gì?
- Người chơi làm gì?
- Thế giới hoạt động ra sao?
- Những gì đã được khóa?
- Những gì còn được phép điều chỉnh?
- Các hệ thống kết nối thế nào?
- Dữ liệu thuộc quyền sở hữu của ai?
- Code cần triển khai theo thứ tự nào?
- Thành công/thất bại được xác định thế nào?
- Cần kiểm thử điều gì?
- Các giới hạn lịch sử và văn hóa là gì?

Nếu những câu hỏi đó đã được giải quyết đủ sâu, việc tạo thêm tài liệu mới không còn là ưu tiên tốt.

<a id="m29-t081-s137"></a>

### 111. Không đánh đồng đủ tài liệu với đủ nội dung game

Một GDD đủ để bàn giao triển khai không nhất thiết phải viết tay:

- hàng nghìn biography;
- mọi lời thoại;
- mọi biến thể event;
- toàn bộ quest;
- mọi item description;
- mọi câu chuyện có thể xuất hiện.

Đó là Production Content, có thể được sinh và biên tập theo Content Pipeline sau này.

Nhưng GDD phải định nghĩa đầy đủ:

- schema;
- rules;
- content quality requirements;
- historical boundaries;
- narrative constraints;
- sample content;
- validation policy.

**Đủ bộ tài liệu nghĩa là không còn khoảng trống kiến trúc hoặc gameplay cốt lõi mà người lập trình buộc phải tự sáng tạo để hệ thống hoạt động.**

<a id="m29-t081-s138"></a>

### 112. Documentation Coverage Matrix

Đề xuất kiểm tra bộ GDD bằng ma trận sau.

| Nhóm hồ sơ | Câu hỏi bắt buộc được trả lời | Tình trạng ở mức thiết kế |
|---|---|---|
| Product Vision | Game có bản sắc gì? | Đã xây dựng |
| World & Historical Canon | Bối cảnh và mốc lịch sử ra sao? | Đã xây dựng; cần chuẩn hóa canon |
| Game Loop/Campaign | Người chơi làm gì qua từng giai đoạn? | Đã xây dựng |
| Character/Progression | Nhân vật được sinh và phát triển ra sao? | Đã xây dựng |
| NPC/World Simulation | Thế giới sống như thế nào? | Đã xây dựng |
| Combat/Exploration | Hành động trực tiếp hoạt động ra sao? | Đã xây dựng |
| Economy/Crafting | Tài nguyên và nghề vận hành thế nào? | Đã xây dựng |
| Social/Knowledge/Quest | Quan hệ, thông tin và nhiệm vụ liên kết ra sao? | Đã xây dựng |
| Belief/Myth/Mạch | Ranh giới truyền thuyết và siêu nhiên là gì? | Đã xây dựng |
| UI/UX | Người chơi hiểu và tương tác thế nào? | Đã xây dựng |
| Architecture Audit | Có mâu thuẫn hoặc phụ thuộc nguy hiểm không? | Đã có baseline |
| Canonical Data Model | Dữ liệu và quyền sở hữu ra sao? | Đã có baseline |
| Rule/Formulas | Engine quyết định kết quả thế nào? | Đã có baseline; hệ số cần tuning |
| Vertical Slice | Sẽ chứng minh gameplay bằng phạm vi nào? | Đã xây dựng |
| Foundation Specification | Engine core phải có contract gì? | Module 29 |
| Handoff Package | Đội triển khai nhận tài liệu nào là chuẩn? | Cần tổng hợp |
| Open Decision Register | Những quyết định nào chưa chốt? | Cần tổng hợp và rà soát |
| Traceability & Acceptance | Yêu cầu nào được nghiệm thu bằng test nào? | Cần hợp nhất thành checklist |

Ma trận trên không khẳng định đã kiểm tra từng dòng của tất cả 29 tài liệu gốc. Nó đánh giá phạm vi chủ đề đã được xây dựng, đồng thời nêu những phần cần rà soát khi hợp nhất hồ sơ.

<a id="m29-t081-s139"></a>

### 113. Phân loại các khoảng trống trước khi dừng

Một gap cần được phân loại.

<a id="m29-t081-s140"></a>

#### Loại A — Blocking Design Gap

Ví dụ:

Không rõ domain nào có quyền thay đổi một resource.

Không rõ Historical Anchor có cho phép một loại player action hay không.

Không rõ Succession chuyển Knowledge thế nào.

Đây là gap cần giải quyết trước khi đóng bộ GDD.

<a id="m29-t081-s141"></a>

#### Loại B — Non-Blocking Tuning Gap

Ví dụ:

Skill Bonus cuối cùng là +5 hay +6.

NPC utility weight có cần điều chỉnh không.

Một mức Injury có quá nghiêm khắc không.

Các giá trị này phải được chốt qua prototype và playtest.

Không cần tiếp tục viết hàng loạt module chỉ để dự đoán kết quả chưa có thực nghiệm.

<a id="m29-t081-s142"></a>

#### Loại C — Production Content Gap

Ví dụ:

Chưa có 500 Event Templates.

Chưa có lời thoại cho mọi settlement.

Chưa có asset cho tất cả NPC.

Đó là công việc sản xuất nội dung, không nhất thiết là thiếu GDD.

<a id="m29-t081-s143"></a>

#### Loại D — Technology Validation Gap

Ví dụ:

SQLite provider nào phù hợp Unity target platform.

World Simulation chạy được bao nhiêu NPC.

Snapshot interval tối ưu.

Đó là công việc kỹ thuật cần benchmark.

Không biến những câu hỏi đo đạc thành việc viết thêm GDD lý thuyết vô hạn.

<a id="m29-t081-s144"></a>

### 114. Điều kiện chính thức để nhắc dừng

Tôi đề xuất chúng ta **dừng giai đoạn viết thêm module thiết kế mới** khi đạt tất cả điều kiện sau:

1. Các nhóm gameplay chính đều có đặc tả đầy đủ về mục tiêu, mechanic, giới hạn và hậu quả.
2. Những mâu thuẫn P0 về kiến trúc và game-canon có quyết định xử lý rõ.
3. Canonical Data Model và Domain Ownership thống nhất.
4. Rule Engine và Foundation Contracts đủ để đội kỹ thuật bắt đầu.
5. Vertical Slice có scope, workflow, dữ liệu thử nghiệm và acceptance tests.
6. Có một bộ hồ sơ bàn giao thống nhất, không phải 29 đoạn tài liệu rời rạc thiếu phiên bản.
7. Các Open Decisions được phân loại Blocking, Tuning, Content hoặc Technology.
8. Không còn vấn đề cốt lõi bắt buộc phải tạo thêm một module riêng mới giải quyết được.

Khi đạt điều kiện đó, **bước đúng là chuyển sang thực thi hoặc kiểm chứng bằng prototype ở một luồng công việc khác**, không tiếp tục kéo dài chuỗi GDD.

<a id="m29-t081-s145"></a>

### 115. Giai đoạn đóng bộ tài liệu

Sau Module 29, tôi đề xuất không mặc định mở Module 30.

Trước tiên thực hiện một **Documentation Closure Review** đối với toàn bộ hồ sơ.

Đây không nhất thiết là một module gameplay mới.

Nó là hoạt động biên tập và nghiệm thu hồ sơ gồm:

**Consolidation:** Hợp nhất quyết định đã khóa.

**Conflict Resolution:** Loại bỏ những mô tả khác nhau giữa các module.

**Canonical Glossary:** Chuẩn hóa thuật ngữ.

**Decision Register:** Lưu các ADR và open decisions.

**Dependency Map:** Chốt quan hệ giữa hệ thống.

**Traceability Matrix:** Liên kết requirement → contract → test.

**Handoff Index:** Chỉ rõ đội triển khai cần đọc tài liệu nào theo thứ tự.

**Freeze:** Đánh dấu phiên bản GDD baseline để bàn giao.

Nếu sau Closure Review không có Blocking Design Gap, cần **dừng tại đó**.

Nếu còn gap P0, chỉ bổ sung một tài liệu sửa đúng khoảng trống ấy, không tiếp tục mở rộng ý tưởng không có giới hạn.

---

<a id="m29-t081-s146"></a>

## PHẦN XXII — KẾT LUẬN MODULE 29

<a id="m29-t081-s147"></a>

### 116. Foundation không phải một tầng phần mềm phụ trợ

Foundation quyết định một trong những đặc tính quan trọng nhất của game:

**Thế giới có ghi nhớ và tuân theo những gì thật sự đã xảy ra hay không.**

Một người có thể chết.

Một vật phẩm có thể được trao lại.

Một lời hứa có thể đến hạn.

Một tin đồn có thể lan đi.

Một thuyền có thể hỏng.

Một tuyến vận tải có thể đóng.

Một người học nghề có thể trở thành thợ giỏi.

Tất cả các sự kiện đó chỉ có giá trị nếu chúng:

- không mâu thuẫn;
- không tự bị ghi đè;
- không được tạo lại tùy tiện;
- không phụ thuộc việc scene còn được load;
- có thể tiếp tục theo thời gian;
- và có thể được kiểm chứng bởi đội phát triển.

<a id="m29-t081-s148"></a>

### 117. Những nguyên tắc kỹ thuật quan trọng nhất

**Một World State authoritative.**

**Một nguồn identity nhất quán.**

**Một trục World Time.**

**Một command/commit pipeline có kiểm soát.**

**Các rule có phiên bản.**

**Random có seed, context và thuật toán rõ ràng.**

**Mọi resource và ownership có nguồn.**

**Những event quan trọng có quan hệ nhân quả.**

**Snapshot và journal đủ để lưu và kiểm tra lịch sử.**

**Các domain có quyền sở hữu dữ liệu riêng.**

**Không có LLM quyết định World State trong runtime.**

<a id="m29-t081-s149"></a>

### 118. Tuyên ngôn cuối cùng

Game này không cần một hệ thống AI ngôn ngữ luôn nghĩ ra chuyện mới để thế giới trông sống động.

Nó cần một hệ thống quy tắc đủ nhất quán để:

Một người có hoàn cảnh riêng.

Một nhu cầu xuất hiện.

Một lựa chọn được đưa ra.

Một hậu quả xảy ra.

Một người khác phản ứng.

Thời gian tiếp tục.

Một ký ức hoặc di sản còn lại.

Và sau nhiều năm, thế giới mang dấu vết của những cuộc đời từng tồn tại trong nó.

**Foundation là nơi bảo đảm những dấu vết đó không chỉ là lời kể, mà là dữ liệu lịch sử nhất quán của thế giới game.**

---

<a id="m29-t081-s150"></a>

## PHẦN XXIII — DOCUMENT HANDOFF RECOMMENDATION

<a id="m29-t081-s151"></a>

### 119. Kết quả bàn giao của Module 29

Sau module này, bộ tài liệu kỹ thuật đã bao phủ:

- Foundation Architecture.
- Entity Identity.
- World State Revision.
- WorldClock và Scheduler.
- Deterministic RNG.
- Rule Handler Contracts.
- Command Processing.
- Transaction Boundaries.
- Event Journal.
- Persistence.
- Snapshot/Recovery.
- Content Registry.
- Replay/Testing.
- Developer Tooling.
- Task Breakdown.
- Documentation Stop Criteria.

Module 29 hoàn tất trách nhiệm **đặc tả nền tảng**.

Nó không đồng nghĩa Foundation đã được viết hoặc chạy thành công.

<a id="m29-t081-s152"></a>

### 120. Định hướng sau Module 29

**Ưu tiên số một không phải sinh thêm module thiết kế mới.**

Ưu tiên là đánh giá, hợp nhất và đóng gói bộ GDD đã có.

Nếu phát hiện những thiếu sót thực sự ảnh hưởng tới thiết kế cốt lõi, chúng ta sẽ sửa chính xác những phần ấy.

Nếu bộ hồ sơ đạt tiêu chí đầy đủ, cần kết thúc giai đoạn xây dựng tài liệu.

**Không để quá trình thiết kế kéo dài vô hạn chỉ vì luôn còn điều có thể mô tả chi tiết hơn.**

**Điểm kết thúc hợp lý là khi những quyết định còn lại thuộc về lập trình, cân chỉnh bằng dữ liệu thực nghiệm hoặc sản xuất nội dung — không còn thuộc về thiết kế nền tảng.**

---

<a id="m29-t081-s153"></a>

### Đánh giá sau Module 29: Đã nên dừng chưa?

**Chúng ta đang rất gần điểm dừng của giai đoạn xây dựng GDD.**

Tính đến Module 29, các nhóm thiết kế gameplay, mô phỏng thế giới, kiến trúc hệ thống, công thức, Vertical Slice và Foundation đã có nền đặc tả. Tuy nhiên, tôi **chưa khuyến nghị tuyên bố bộ tài liệu đã hoàn tất ngay**, vì còn một công việc quan trọng: hợp nhất và đối chiếu toàn bộ các quyết định đã khóa, thay vì để chúng tồn tại rải rác trong 29 module.

Tôi đề xuất bước cuối của giai đoạn tài liệu là **GDD Documentation Closure & Handoff Review**, tập trung vào ba đầu ra: một bộ GDD Master thống nhất, một sổ quyết định và vấn đề còn mở, và một danh mục hồ sơ bàn giao kèm tiêu chí nghiệm thu.

Sau bước đó, nếu không còn khoảng trống thiết kế nghiêm trọng, **tôi sẽ nhắc bạn dừng nghiên cứu và xây thêm module**, chuyển bộ tài liệu sang trạng thái *Design Baseline — Ready for Implementation*. Không cần tiếp tục viết Module 30, 31, 32… chỉ để tăng độ chi tiết.

Đây là điểm tôi sẽ chủ động kiểm soát trong các bước tiếp theo của dự án.

<!-- SOURCE T081 END -->

