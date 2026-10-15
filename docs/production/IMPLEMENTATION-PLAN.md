# Kế hoạch triển khai từ bộ tài liệu

Dùng [Backlog](BACKLOG.csv) làm danh sách task có dependency và acceptance. Tất cả runtime tasks hiện chưa làm; FND-01 và DOC-01 là phạm vi của lần khởi tạo này. Module 28 giữ blueprint đầy đủ và Module 29 giữ 16 task Foundation nguyên bản.

## Trình tự bắt đầu

1. FND-02/03/04 hiện thực identity/time/RNG theo ADR-008 và specs/vectors đã bổ sung; không chọn lại wire codec hoặc tick unit trong từng domain.
2. FND-05/06/07/08 dựng read-only state → typed rule → command → staged transaction. Dùng một hành động resource transfer nhỏ, có rejection/retry/stale/failure cases.
3. FND-09/10/11/12/13 dựng causal events, jobs, atomic persistence, recovery và command replay. Đừng mở rộng hệ class khi không thể giải thích và tái hiện một command.
4. FND-14/15/16 làm registry/diagnostics và kiểm chứng host–Unity. Đây là điều kiện trước khi tuyên bố Foundation xong, không phải việc để sát ngày phát hành.
5. VS-02–06 nối nghề, vật chất, perception, thread, companion, combat và di sản trong một vùng sâu.
6. VS-07–10 đưa qua client, playtest và scale. Phê duyệt mở full production từ bằng chứng, không từ số NPC/scene.

## Task đầu tiên đề nghị: FND-02

Scope: typed entity/definition identity, allocator state, lifecycle và serialization contract. Input: M29 §9–14, Foundation contract, reference fixture IDs. Output: C# implementation độc lập Unity, codec spec, golden vectors và tests; quyết định clear committed vs temporary ID. Không gộp world generator hoặc household simulation vào task này.

Done: identity được serialize ổn định; không tái dùng committed ID; rollback/retry không phá deterministic allocation; references khác loại bị phát hiện; tests có command và expected output. Nếu chọn byte/string representation khác blueprint ví dụ, ghi ADR và cập nhật fixtures đúng phiên bản.

## Mẫu giao task cho lập trình viên/agent

- Task ID, requirement IDs, module/section nguồn.
- Allowed files/assembly và dependency boundary.
- Owned data, read dependencies, allowed commands, emitted events.
- Invariants, rejection/errors, concurrency/retry/persistence behavior.
- Acceptance cases và cách chạy.
- Explicit non-goals, open decisions phải chốt trong task.
- Deliverables gồm code, tests, docs/ADR thay đổi và evidence.

Không giao “đọc cả GDD rồi build game” thành một task không kiểm soát. Source research vẫn là tài liệu, không phải quyền tự phát triển tất cả hệ thống trong một lượt.

## Definition of Done

Build và tests của phạm vi pass; không phá replay/contract; mọi mutation đúng owner và transactional path; content có schema/reference validation; UI không biết secrets ngoài projection; test evidence ghi versions/seed. Nếu không có Unity trong môi trường, nói rõ compatibility chưa được kiểm chứng thay vì coi dotnet build đủ thay thế.

## Gates sau rà soát 14/10/2026

- **VS-01 = Headless kernel gate:** FND-02–15; cho phép làm content/domain slice khi kernel có evidence. Không được gọi gate này là Foundation complete.
- **Foundation complete:** tất cả FND-02–16, bao gồm cùng vectors/replay trên Unity target thật. VS-07 phụ thuộc FND-16.
- **Early client spike:** sau FND-02/03/04 build được, thử import portable DLL/primitive vectors và provider packaging; không đợi replay hoàn chỉnh mới phát hiện incompatibility. FND-16 chỉ nghiệm thu cuối sau FND-13.
- **Test setup:** FND-02 phải pin framework/runner, tạo test project/CI discovery>0 theo Test Execution Contract; reference Python checks không thay điều này.
- **Data gate:** parameter pack cho từng nghề phải có trước rules VS-03. VS-10 tune dựa playtest, không hoãn thiết kế units đến đó.

Specs và trạng thái sau bổ sung: [Documentation Completion](DOCUMENTATION-COMPLETION-2026-10-14.md). Provider/editor/framework còn có gate thực nghiệm; không xem chúng là thiếu module GDD mới.

Quy trình áp dụng cho AI: [AI Execution](../runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope Sprint 1 nằm trong sprint brief được liên kết tại đó.
