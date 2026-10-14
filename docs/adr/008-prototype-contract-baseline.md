# ADR-008 — Baseline đặc tả prototype 1.1

Ngày: 2026-10-14. Trạng thái: **áp dụng cho đặc tả prototype; implementation chưa có**.

Yêu cầu hiện tại là hoàn thiện tài liệu ngay trong project và đồng bộ mọi nơi bị ảnh hưởng. Các lựa chọn dưới đây là quyết định biên soạn mới, không được gán ngược cho 29 module hay coi là kết quả playtest. Nguồn gốc gap: [audit](../production/PRE-DEVELOPMENT-AUDIT-2026-10-14.md), Modules 26–29.

| Phạm vi | Quyết định áp dụng | Hệ quả |
|---|---|---|
| Identity | ID dạng world/type/counter, số nguyên 64-bit không dấu, cấp trong transaction | Codec và tombstone theo Identity Spec; fixture địa lý cũ vẫn là authored definition aliases |
| Time | Một tick = một phút; lịch simulation 360 ngày/năm | Không tuyên bố đây là lịch sử Việt cổ; calendar presentation tách biệt |
| RNG | `sha256-counter-v1`, contextual counter và rejection sampling | Thay PCG32 candidate trong nghiên cứu cho prototype; phải benchmark trước mở rộng thế giới |
| Numbers/hash | Int64 checked; fixed-point scale 1000; codec `mdc1` | Không hash JSON mặc định hoặc dùng float; schema đổi phải tăng version |
| Activity | Reserve vật liệu khi bắt đầu, consume khi hoàn tất thành công | Cancel không hoàn nguyên tổn thất đã commit; completion revalidate actor/rights |
| Save | SQLite qua adapter, một transaction cho toàn commit; snapshot ở mỗi 100 commits | Provider/editor cụ thể vẫn phải chọn bằng spike, không giả lập bằng tài liệu |
| UX | Desktop keyboard/mouse, tiếng Việt; một nhân vật được sinh cho mỗi campaign | Không reroll trong màn reveal; campaign mới dùng seed mới, không ngăn người chơi tự tạo lại campaign |
| Milestones | VS-01 là headless kernel gate; Foundation complete cần FND-16 | Không cho slice UI nghiệm thu khi thiếu cross-runtime evidence |

Các hợp đồng chi tiết tại [mục lục bổ sung](../production/DOCUMENTATION-COMPLETION-2026-10-14.md) có hiệu lực trước ví dụ cũ trong module. Duy trì no-runtime-LLM, random character, causal simulation và knowledge isolation. Thay đổi thuật toán sau này yêu cầu ADR, version mới, vectors mới, compatibility decision và cập nhật mọi consumer. Không diễn giải “prototype” thành quyền bỏ invariant.

Provider SQLite, Unity patch/backend, framework test package và production budgets chưa được chứng minh trong môi trường này. Những lựa chọn đó có tiêu chí ra quyết định trong các spec và vẫn được theo dõi ở Open Decisions. Không cài thêm runtime hoặc viết engine trong đợt bổ sung tài liệu.
