> Báo cáo dưới đây ghi nhận trước đợt bổ sung. Trạng thái hiện tại: [Documentation Completion](DOCUMENTATION-COMPLETION-2026-10-14.md). ZIP đã bị xóa theo yêu cầu; các đoạn nói về ZIP là quan sát lịch sử, không là quy trình hiện hành.

> Báo cáo lịch sử ngày 2026-10-14. Trạng thái mới ngày 2026-10-15: xem [Sprint 1](sprints/SPRINT-01.md) và [FND-02 evidence](../qa/FND-02-EVIDENCE.md); không dùng các mục pending bên dưới thay trạng thái hiện tại.


# Rà soát mức sẵn sàng trước phát triển — 14/10/2026

## Kết luận

**Có thể bắt đầu các task Foundation theo hướng chốt đặc tả và prototype có giới hạn. Chưa nên giao nhiều dev triển khai gameplay song song với giả định mọi contract đã hoàn chỉnh.**

29 module đã bao phủ thiết kế lớn, và việc đóng gói giữ được nội dung chi tiết. Thiếu chủ yếu là các quyết định kỹ thuật có thể thực thi, dữ liệu mẫu có expected outputs và một số định nghĩa gate. Không cần viết thêm Module 30 hoặc làm lại toàn bộ GDD.

Điều chỉnh cách hiểu phát biểu “sẵn sàng phát triển” ở lần bàn giao trước: đủ để bắt đầu FND-02 dưới dạng **spec → implementation → verification**; chưa đủ để bỏ bước chốt spec và cho các hệ phụ thuộc code theo suy đoán.

Phạm vi lần này: kiểm tra hồ sơ/nguồn hiện có, Modules 26–29 ở các hợp đồng lõi, ownership, backlog, QA, fixture, CI và artifacts. Đây không phải tái thẩm định từng phát biểu lịch sử, chứng minh toán học mọi công thức, benchmark hay kiểm thử engine chưa triển khai. Không chọn thay các thuật toán/số balance trong lần audit.

## 1. Những gì đã có và không cần viết lại

- 29 module và phụ lục 06A; đầy đủ phần nghiên cứu đã chọn từ archive, có mapping và checksum.
- Baseline cho random character, no runtime LLM, causal simulation, knowledge isolation, domain ownership, command/commit, world time và replay.
- World Bible, profession/class/combat/economy/narrative/legacy và UI principles có nội dung chi tiết.
- Foundation logical contracts, schema concepts, vertical-slice scope, backlog và acceptance scenarios.
- 52 yêu cầu/scenario ưu tiên. Chúng không phải toàn bộ test coverage và chưa là executable tests; điều này đã được ghi đúng trong QA Plan.

Việc một primitive mới có đặc tả khái niệm không có nghĩa phải tạo thêm một “hệ thống game”. Cần phụ lục thực thi nhỏ, chính xác và gắn vào module đã có.

## 2. Những gói cần cụ thể hóa sớm nhất

| Ưu tiên | Gói tài liệu/dữ liệu | Đã có | Còn phải chốt | Trước mốc nào |
|---|---|---|---|---|
| A | Identity & Registry | Namespace/counter, phân biệt entity/definition, lifecycle | Kiểu lưu/serialize, phạm vi, giá trị invalid, comparison, allocation rollback, registry transitions | Public API FND-02 |
| A | Time & Scheduler | Integer ticks, phase, stable order, revalidation | Tick unit/calendar, priority direction, same-tick insertion/phase đã qua, event budget, skip cases | FND-03 và FND-10 |
| A | Deterministic Math & RNG | Scales, fixed-point idea, PCG32 candidate | Rounding/ties/overflow/negative division, exact PRNG variant, encoding/seeding/bounded sampling, vectors | FND-04 và numeric rules FND-08 |
| A | Command & Activity Catalog | Envelope, stages, atomicity, generic errors | Typed payload, ChangeSet fields, reserve/consume/cancel/refund, rejected receipts, per-command state transitions | FND-07/08; repair trước VS-03 |
| A | Kernel fixture & expected results | M29 §89 mô tả fixture nhỏ | File dữ liệu có item/stock/condition, ordered commands, expected deltas/rejections, version manifest | Integration FND-07/08; hashes trước FND-13 |
| A | Test execution contract | QA scenarios và nhóm thư mục test | Runner/framework/version, discovery, CI test commands, evidence format, regression/golden update policy cụ thể | Task code đầu tiên được nghiệm thu |

“A” là ưu tiên gần đầu pipeline, không có nghĩa phải hoàn tất cả sáu gói trước khi viết dòng code đầu tiên. Ví dụ có thể implement/test identity sau khi chốt identity spec mà chưa cần database schema hay full roster.

Chi tiết từng gap, evidence, nơi đề nghị đặt file và điều kiện đóng nằm ở [Readiness Gap Register](../governance/READINESS-GAPS-2026-10-14.csv). Các đường dẫn `proposed_location` trong CSV là đề nghị tạo trong công việc tiếp theo, không phải file đã tồn tại.

### Bằng chứng cụ thể tránh nhầm “không có” với “chưa khóa”

**Identity:** M26 §12 cho phép domain prefix + counter hoặc UUID nếu có replay contract. M29 §11 thu hẹp về counter theo world/namespace nhưng vẫn yêu cầu khóa rollback policy. Thiếu wire-level contract, không thiếu ý tưởng về identity.

**Numbers:** M27 §4–5 có scales và `RoundDeterministic(...)`, nhưng yêu cầu phương thức làm tròn “phải được khóa”. Chưa có một lựa chọn cụ thể để mọi domain dùng giống nhau. Đây là việc cần trước numeric implementation, không thể để tới cuối save compatibility.

**Scheduler:** M26 §76 minh họa `(DueAt, Phase, Priority, StableEntityID, ScheduleSequence)`; M29 §24 đề xuất `(DueTick, Phase, Priority, CreationSequence, ScheduleID)`. Baseline đã ưu tiên đặc tả sau, nên không cần coi hai ví dụ là hai hệ cùng có hiệu lực. Tuy nhiên cần viết contract cuối có chiều sort, phase policy và same-tick behavior để có expected ordering duy nhất.

**Activity:** SCN-01 hiện nói “reserve/consume theo contract đã chọn”. Tài liệu chưa chứa lựa chọn đó cho repair. Nếu hai người triển khai khác nhau, một bên có thể trừ nguyên liệu lúc bắt đầu, bên kia lúc hoàn tất; cancel/death/save sẽ cho kết quả khác dù đều tưởng mình theo GDD.

**Fixture:** `vertical-slice.reference.json` có 2 actor, 1 household, 4 settlement (gồm boundary), 6 directed routes. Routes mang `cost_status: to-author-before-runtime`; không có thuyền/item, commodity quantities, command sequence hoặc expected state. Đây là reference thiết kế đúng như nhãn của nó, không phải golden replay fixture.

## 3. Các phần có thể hoàn thiện theo milestone

| Gói | Cần trước | Không cần trước |
|---|---|---|
| Runtime content schemas + slice pack | VS-02 completion; feature load tương ứng | Identity/time prototype |
| Versioned prototype parameters | Công thức đầu tiên dùng chúng, đặc biệt VS-03 | Toàn bộ final balance |
| Persistence/save schema + provider spike | FND-11/12; exact replay về sau | Core primitives độc lập storage |
| Platform/Editor + cross-runtime spike | FND-16; nên kiểm sớm sau primitive prototype | Final art/audio production |
| Wireframes/flows và reroll decision | Onboarding/Unity client VS-07 | Headless kernel |
| Full campaign anchors, full world content | Mở rộng campaign/production; slice cần proof nhỏ VS-08 | Toàn bộ 12 vùng trước slice |
| Historical source verification/art/localization budget | Public content và production tương ứng | Foundation prototype |

Không thiếu “Combat GDD” hoặc “Economy GDD” như một module. Các hệ đó đã có; thiếu phần dữ liệu/contract cụ thể đúng mức cần cho feature được giao.

## 4. Các mốc đang cần chỉnh rõ trong kế hoạch

1. **OPEN-12 quá gộp:** numeric semantics/canonical encoding cần ở Foundation sớm; migration/save compatibility có thể chốt ở FND-12. Không giữ tất cả dưới một gate cuối.
2. **OPEN-05 cần hai lần chốt:** hệ số prototype ban đầu phải tồn tại khi code VS-03; tối ưu balance cuối có thể tới VS-10. “Chưa final balance” không đồng nghĩa “chưa có bộ tham số để chạy”.
3. **Foundation gate có hai nghĩa:** BACKLOG cho VS-01 phụ thuộc FND-02–15, không có FND-16. QA/Foundation contract đòi cross-runtime proof trước Foundation complete. Đề nghị gọi riêng “headless kernel gate” và “Foundation cross-runtime gate”; không trì hoãn toàn bộ headless work vì thiếu Unity, cũng không báo complete quá sớm.
4. **Time type dependency:** FND-07 có `world_tick`, nhưng dependency graph không dẫn tới FND-03. Cần xác định primitive time nằm trong prerequisite nào trước khi chia việc. DAG không có chu trình vẫn có thể thiếu dependency nghiệp vụ.
5. **Toolchain nghiệm thu:** CI hiện chạy docs checks, build và host status. Cần thêm test runner/command khi task kernel đầu tiên bắt đầu có logic; status smoke không thay invariant tests.

Lần audit này ghi nhận các sửa đổi đề nghị, chưa tự thay backlog/baseline hoặc chọn công thức.

## 5. Kết quả kiểm tra thực tế

Đã chạy lại ngày 14/10/2026:

- `build_docs.py --check`: 36 generated files khớp inputs và source message hashes.
- `check_handoff.py`: đủ 29 module + phụ lục, 6.156 source sections, 52 requirements, 28 tasks, DAG hợp lệ và 292 authored links tại thời điểm trước khi thêm báo cáo này.
- `validate_reference.py`: reference fixture qua schema subset/reference checks; 4 negative controls bị từ chối.
- `check_exports.py`: 14 HTML/DOCX artifacts khớp export manifest, đủ 30 module/appendix titles, 6.156 source anchors; Master có 123 bảng.

Đây là kiểm tra tính toàn vẹn/tổ chức, **không chứng minh đủ contract hoặc engine correctness**. Không chạy lại build/runtime gameplay vì lần rà soát này không đổi code, và runtime chưa tồn tại để nghiệm thu.

### Lưu ý phiên bản bộ tải về

So với manifest của ZIP lần trước: `docs/README.md` đã thay đổi và whitepaper chưa nằm trong gói. Các file được đóng gói trước vẫn tồn tại; khác biệt này là snapshot cũ, không phải ZIP bị hỏng. Báo cáo audit hiện tại cũng được tạo sau snapshot ấy.

`check_exports.py` so outputs với manifest của outputs, chưa so toàn bộ authored inputs hiện tại với fingerprint lúc export. Vì vậy “export check pass” không tự có nghĩa “ZIP chứa mọi cập nhật mới nhất”. Khi phát hành bộ cập nhật sau khi đóng gaps, cần regenerate và ghi release/input fingerprints. Lần audit không ghi đè gói cũ để tránh diễn đạt rằng các gaps đã được giải quyết.

## 6. Đề nghị bước tiếp theo

1. Bổ sung **Identity Spec và Test Execution Contract** cho FND-02, rồi thực hiện task này có tests.
2. Chốt **Time/Scheduler** và **Math/RNG** bằng ADR, examples/vectors và prototype kiểm chứng.
3. Viết **command catalog nhỏ + kernel fixture** đủ transfer, advance time, staged work; kiểm rejection/retry/rollback trước khi tăng content.
4. Sau kernel ổn định, đi tiếp persistence, replay, content pack và vertical slice đúng dependencies đã làm rõ.

Đây là đợt **khóa hợp đồng thực thi**, không phải một vòng mở rộng ý tưởng game. Tài liệu hiện tại đủ để tiến hành đợt này ngay; chưa nên bỏ qua nó và đi thẳng vào production gameplay.
