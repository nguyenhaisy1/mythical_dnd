# ADR-010 — Sprint 1 portable primitives và nghiệm thu

Ngày 2026-10-15; FND-03/04. Giữ clock-v1, mdc1, sha256-counter-v1 theo ADR-008; không thay golden hoặc nghiên cứu gốc.

## Phạm vi và trách nhiệm

WorldTick và calendar là pure immutable primitives; chỉ nhận input tường minh, không tự advance theo wall-clock. Work duration dương; phép cộng duration và chuyển calendar có kiểm range TIME_RANGE. Scheduler ordering/time skip/recovery vẫn FND-10; TEST-008 đầy đủ trong matrix chưa được nghiệm thu bằng clock tests.

RNG context/state immutable, mỗi draw trả value và next state. Preview/abort bỏ next state; chỉ Application tương lai mới persist state trong world commit. Same context/index luôn cùng word; không trộn UI vào gameplay stream. UInt64 counter không được wrap. Exact numeric intermediates dùng BigInteger và kiểm kết quả Int64.

mdc1 là canonical encoder, không phải JSON parser hoặc content schema validator. Chấp nhận primitive types được định nghĩa, từ chối floating point, keys không canonical, duplicates và unpaired surrogates. Caller phải validate schema/unknown fields trước hash; object keys sort ordinal, arrays giữ thứ tự. Chưa có full-world StateHash/replay khi chưa có snapshot/runtime đầy đủ.

## Evidence và tích hợp

NUnit và package lock theo ADR-009. Runtime runner nghiệm thu cả Sprint 1: mỗi nhóm TEST_027/TEST_008/TEST_007 phải được discover và pass; kiểm tổng không skipped, và negative-control process phải thất bại đúng một test. Golden files copy từ nguồn committed, không tạo lại từ code đang test.

/root/clock thực hiện FND-03, /root/random thực hiện FND-04; /root review/tích hợp và nghiệm thu. Reviewer cùng hệ AI không thay nghiệm thu Unity hay người chơi thật. Evidence cuối nằm tại [Sprint 1 evidence](../qa/SPRINT-01-EVIDENCE.md). Local acceptance không suy GitHub CI đã chạy hoặc feature đã merge dev.

Boundary policy: DrawIndex UInt64.MaxValue là exhausted cursor; Draw reject NUMERIC_OVERFLOW trước khi phát sinh word không thể commit next cursor. Context fields ngoài entity kiểm nonblank UTF8; grammar nghiệp vụ là caller responsibility, được giữ trong OPEN-01.
