# ADR-012 — Committed event model cho transfer kernel

Ngày 2026-10-16. Trạng thái: áp dụng trong Sprint3/FND-09, prototype in-memory. Bổ sung Foundation contract; không thay archive nghiên cứu.

## Bối cảnh

Sprint2 chỉ có CommittedTransfer tối thiểu. FND-09 cần metadata nhân quả có version, ordering và consumer không áp cùng event hai lần. Chưa có persistence/scheduler nên không thể nghiệm thu durable delivery hoặc scheduled side effects.

## Quyết định

- Event envelope `event-v1` bất biến, được tạo từ accepted transfer đã validate. Identity cấp staged cùng world update/receipt; publication cùng một immutable aggregate. Proposed events không được giao consumer trước commit.
- Event type ResourceTransferred; aggregate là source lot. World tick lấy từ commit; commit sequence là world revision, event index bắt đầu 0 trong commit. Transfer hiện sinh đúng một event; revision có thể có khoảng trống khi workflow khác thêm commit không event trong tương lai.
- Causation ID là command ID đã nhận; correlation ID giữ từ command, phục vụ nhóm trace. Đây là command-to-event causality; không tự gán parent-event link chưa được xác minh. Event type/schema và rule version là metadata riêng; command fingerprint/retry vẫn theo ADR011. Rule version `transfer-resource-v1` định danh semantics được invariant gate của Application cưỡng chế, không là version tùy ý của injected handler; registry hiện chưa có rule-version metadata.
- Payload giữ source lot, destination lot, quantity typed, bất biến. Journal cung cấp ordering deterministic và query command/correlation cho trusted diagnostics. Không đưa omniscient journal cho UI/NPC như knowledge projection.
- Consumer prototype chỉ publish immutable projection và checkpoint cùng một reference trong bộ nhớ. Không dùng callback gây external side effect rồi ghi checkpoint để tuyên bố exactly-once. Duplicate identity/version có payload giống giữ nguyên kết quả; collision phải lỗi, không âm thầm bỏ qua. Lỗi trước publication giữ projection/checkpoint để retry. Hàm tính proposed projection phải pure; atomic guarantee chỉ bao phủ state/checkpoint do consumer sở hữu, không bao phủ external side effects của callback vi phạm contract. Event mới đến sai thứ tự hoặc khác world phải reject; host chịu trách nhiệm giao journal theo thứ tự. Cặp `(commit sequence, event index)` là vị trí duy nhất; hai ID khác nhau cùng vị trí bị reject. Tick không được lùi. Version envelope hiện chỉ nhận 1; schema payload khác được fingerprint riêng, handler consumer phải kiểm schema nó hỗ trợ.
- Consumer là minh chứng cơ chế idempotency, chưa triển khai reward, rumor, scheduler hoặc side effects ngoài process. Fan-out/cycle detection khi sinh follow-up jobs thuộc FND-10. Crash recovery/durable checkpoints thuộc FND-11/12; command replay thuộc FND-13.

## Compatibility và tác động

API transfer đang dùng được giữ tương thích khi có thể; metadata mới không đổi gameplay quantity/authority/RNG. Chưa có released save format để migrate. Envelope version này không tuyên bố tương thích Unity hoặc SQLite. Consumers: Application pipeline/journal, event tests, runtime runner, backlog/QA matrix, README/architecture/handoff và exports. Không đổi GDD body, canonical content hoặc reference RNG golden.

## Kiểm chứng

TEST-037 được nghiệm thu trong scope consumer in-memory sau khi runtime gate thực sự đạt. Test cần cover metadata/version, journal ordering/query, immutable views, rollback/noop/reject, duplicate command/delivery, collision và failure/retry. Sprint3 evidence ghi kết quả thực tế; ADR không tự là bằng chứng test pass.
