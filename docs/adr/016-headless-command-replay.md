# ADR-016 — Replay hành động host trên headless kernel

Ngày2026-10-20, Sprint7/FND13, TEST009/050. Kế thừa save-v1/ADR015 nhưng không thay recovery bằng replay.

## Mục tiêu và ranh giới

Replay phải chạy lại resolution qua CommandProcessor từ canonical initial CommittedWorld. Đọc events hoặc apply committed images không chứng minh determinism. Harness chạy trong processor mới in-memory, không nối production database/notification sinks; principal trong artifact là trusted QA input, không là cơ chế authentication của live game.

Artifact ghi từng Execute(raw command envelope, principal riêng) và DrainAdvance(principal,budget). Không suy log từ receipts: duplicate/mismatch/unauthorized attempts và drain boundaries không được lưu đầy đủ ở receipts; internal scheduler commands cũng không thể gửi như external Execute.

## Artifact và compatibility

replay-v1 chứa manifest versions/content/rule/RNG/optional campaign seed, initial image mdc1 và ordered actions/checkpoints. Actual rule catalog version hiện tại transfer-resource-v1+scheduler-v1; host cấp expected manifest và compiled registries độc lập với artifact. Artifact không chọn script hoặc code loader.

Compatibility phải được kiểm trước processor/actions. Campaign seed do host cung cấp hoặc unavailable rõ ràng; per-context seeds/draw indexes nằm trong initial image. Codec strict canonical, giới hạn input/actions và validate cấu trúc trước sử dụng. Không tự migrate artifact hoặc thay content gần đúng.

## Hash và kết quả

Checkpoint sau mỗi host action: returned receipt/workflow hoặc stable known pipeline error; gameplay hash loại metadata receipts, full-image integrity hash giữ metadata; journal và jobs hash giúp phân loại sai khác. Hash bind manifest versions/content để không gọi hai runtime khác nhau là cùng state chỉ vì dữ liệu tình cờ giống.

Replay thực thi action rồi so checkpoint, dừng ở sai khác đầu tiên. Báo action index, expected/actual revision, category và hashes. Đây là diagnostic debug omniscient; không truyền trực tiếp cho player UI. Không catch mọi exception rồi xem là kết quả gameplay hợp lệ; unexpected runtime error phải fail.

## Golden và acceptance

Golden artifact checked-in, expected checkpoint cố định; tests không regenerate để tự pass. Host chạy artifact ở process riêng và output report; có negative control thay checkpoint/rule để chứng minh divergence được phát hiện. Tests chứng minh handler được gọi lại, compatibility refusal trước handler, duplicate/mismatch/rejected/stale/noop/auth, scheduler/drain, dictionary/culture independence và malformed artifact.

RNG hiện transfer/scheduler không tiêu thụ gameplay draws. Sprint7 chỉ chứng minh contexts/counters được bảo toàn và primitive golden vectors tiếp tục đạt; không claim stochastic gameplay đã replay. Unity conformance, production content registry, broader gameplay/rule trace và scale benchmark vẫn gates riêng.

Diagnostics Sprint7 chỉ category/hashes/revisions/action index; full state-field diff và rule inspector dành FND15. Không nhận đã đáp ứng full diagnostics chỉ vì tìm được checkpoint đầu lệch.
