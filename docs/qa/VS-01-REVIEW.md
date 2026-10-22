# VS-01 — Rà soát nền tảng headless

Ngày 2026-10-22, SPIN-013. Kết luận: đã có evidence cho kernel subset FND02–15; chờ nghiệm thu tích hợp Sprint10 trước cập nhật VS01 accepted. Không đồng nghĩa toàn bộ hành vi dự kiến trong GDD đã hiện thực hoặc Foundation complete.

| Nhóm | Evidence có thật | Giới hạn |
|---|---|---|
| IDs, clock, codec, RNG | Sprint1: rollback IDs, boundary/calendar và fixed vectors | Unity/performance riêng |
| State, rules, command, invariants | Sprint2: immutable snapshots, staged atomic commit, retry/revision | TransferResource kernel subset |
| Event/causal/consumer | Sprint3: journal, ordering, idempotent consumer | Consumer chẩn đoán, chưa player knowledge |
| Scheduler | Sprint4: stable due ordering, budget/cancel/AdvanceTime | Chưa full gameplay breakpoints |
| Persistence | Sprint5: SQLite commit/reopen và crash processes | Headless, không chứng minh mất điện/platform khác |
| Snapshot/recovery | Sprint6: schema2, fallback images, migration sang file mới | Không atomic replace source hoặc Unity UI |
| Replay | Sprint7: Execute/Drain chạy lại từ initial image | Không nhầm recovery images với replay |
| Content | Sprint8: typed compiler/registry, compatible saves | Commodity/transfer-template, chưa roster VS02 |
| Diagnostics | Sprint9: actual traces, full field diff, causal/jobs | Developer QA, không player-safe projection |

Các báo cáo SPRINT-01 đến SPRINT-09 trong cùng thư mục lưu evidence cụ thể. Baseline tích hợp Sprint9: dev `eacf076807cb26459fbce9fb55c2b6d5ad1c0c44`, 338/338 Release tests, không skip, replay/content/diagnostics và 9 SIGKILL regression đạt; PR9/dev CI đã đạt. Source fingerprint cuối Sprint9: `2b3591e211b883a698699ea9279eb79707843d7488f5e415ef00c942f8907e2c`.

Public boundaries giữ Foundation/Domain không phụ thuộc Unity/IO; Application điều phối; Infrastructure adapters; UI/narrative không có quyền ghi authoritative state. Stable ở đây nghĩa contracts đã có regression evidence cho kernel profile, không cam kết save API thương mại v1.0.

Bước tiếp theo: chạy shared conformance trong Unity thật cho FND16, rồi giao VS02 riêng khi được yêu cầu. Không dùng số tests để suy game chơi được hoặc gameplay thú vị.
