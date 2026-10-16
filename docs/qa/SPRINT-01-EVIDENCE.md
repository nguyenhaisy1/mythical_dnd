# Sprint 1 — nghiệm thu headless primitives

Ngày 2026-10-15; SPRINT-01 / SPIN-003. Kết luận: **đạt phạm vi Sprint 1 local**, FND-02/03/04 headless primitives. Không phải Foundation complete hay game playable.

## Phân công và review

| Actor | Trách nhiệm | Kết quả |
|---|---|---|
| /root | Điều phối, runner/CI/docs, tích hợp, review và nghiệm thu | Hoàn tất gate Release và đồng bộ nguồn |
| /root/clock | FND-03 WorldTick/calendar/duration | 25 test riêng pass; 25 test Release tích hợp pass |
| /root/random | FND-04 math/codec/RNG | 13 test riêng pass; 13 test Release tích hợp pass |
| /root/review | Read-only review Time/Random và tests theo contracts | Không phát hiện blocker; ghi giới hạn context schema |

FND-02 có 32 test regression, implementation SPIN-002 bởi /root. Reviewer /root/review tách khỏi hai agent implement; không gọi đây là human review hoặc Unity verification.

## Lệnh và evidence

`python3 tools/tests/run_runtime.py --dotnet /tmp/mythical-dotnet/dotnet`

- SDK10.0.401, Linux x64; core netstandard2.1, tests net10.0. Locked restore và Release build 6projects, 0warnings/errors.
- Discovered70, passed70, failed0, skipped0; mỗi task có discovery riêng: FND-02=32, FND-03=25, FND-04=13.
- Negative control riêng: 1 discovered, 1 failed, exit1 như mong đợi; toàn gate exit0.
- Fingerprint `sha256:897f00531ceea7ae89e674556ec3c2d0f960403e6db32434bdb01bf983407b4a`; sources.json bao gồm source/runtime runner/fixtures/config/CI, không chỉ commit trước thay đổi.
- Artifacts local ignored: `tests/results/sprint01-tvdcfogg/evidence.json`, `sources.json`, `runtime.trx`, `negative.trx`, logs. Có thể tái chạy lệnh trên, mỗi run tạo folder riêng. CI cấu hình cùng gate + upload; chưa có remote run vì chưa push.

## Coverage và quyết định nghiệm thu

Identity: giữ regression parse/type/world/rollback/restore/exhaustion/lifecycle từ FND-02. Clock: boundary phút/ngày/tháng/năm, 100 năm từng ngày, 1000 mẫu toàn range, Int64.MaxValue roundtrip, invalid/default/negative/overflow TIME_RANGE, immutable tick.

Codec: đọc trực tiếp golden bytes/hash đã commit, integer extrema, strict UTF8, không normalize, culture-independent ordering, duplicates/bad keys/float reject. Math: exact intermediates, tie rounding, signed extrema và NUMERIC_OVERFLOW. RNG: cả golden blocks, actual rejection sampling, block crossing, preview/abort/retry/restored cursor, dice/probability draw schedule, exhausted UInt64 không wrap.

TEST-008 đầy đủ là scheduler ordering (FND-10), **vẫn pending**; các test TEST_008 ở sprint này chỉ phần clock primitives. TEST-007 đạt ở headless RNG primitives, chưa phải integrated command/UI test. OPEN-01/02 được cập nhật theo mức evidence, không xóa Unity/performance gate.

## Giới hạn được review

Context domain/decision/occurrence/stream hiện kiểm nonblank strict UTF8; grammar/vocabulary nghiệp vụ cần caller schema trước khi tích hợp content/commands. Encoder không thay schema validator, không kiểm unknown fields tự động. DrawIndex=UInt64.MaxValue từ chối draw do không biểu diễn được next cursor; không wrap hoặc tự reseed.

Chưa có mutable authoritative world, scheduler, atomic command/storage commit, replay, Unity/IL2CPP, performance budget hoặc human playtest. Chuẩn bị prototype Bến Vạc là hướng tiếp theo, không thuộc cam kết phải playable trong Sprint 1. Không tạo ZIP, tag hoặc release.

## Bàn giao

Next task FND-05 World state/revision, sau đó FND-06/07 theo dependency. Sprint 2 chưa tự khởi động. Code nằm trên branch tích hợp local feature/sprint-01-primitives; dev/main chưa nhận implementation qua PR, không suy local gate thành remote CI hoặc release.

Cập nhật bàn giao: Sprint1 đãpush0315c66, CI GitHub success; xem [Sprint2 evidence](SPRINT-02-EVIDENCE.md) và WORKLOG. Các thông tin chưapush trong báo cáo trên là trạng thái lúc chạy local.
