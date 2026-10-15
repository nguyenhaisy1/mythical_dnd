# Deterministic Math, Codec & RNG — prototype v1

Áp dụng FND-02/04/08/12/13, TEST-007/009/027/050. Nguồn M27 §4–6, M29 §29–37; lựa chọn mới theo [ADR-008](../adr/008-prototype-contract-baseline.md). Tham khảo reference executable [check_contract_vectors.py](../../tools/checks/check_contract_vectors.py); đây không phải runtime engine.

## Số và công thức

Authoritative scalar dùng Int64 checked, trừ ID/counters là UInt64 có codec riêng. Không float/NaN/Infinity, không implicit clamp. Attributes 1–10, skills 0–100, probability 0–10000, utility/condition 0–1000; quantities nguyên theo smallest authored unit; amount âm chỉ hợp lệ cho delta, không inventory total. Invalid range reject trước RNG.

`RoundDiv(n,d)` yêu cầu d>0: chia |n| thành q,r; tăng q nếu 2r≥d, sau đó áp lại dấu. Ties away from zero: 1500/1000→2; −1500/1000→−2; 1499/1000→1; −1499/1000→−1. Intermediate tính exact bằng BigInteger hoặc checked widened operation; kết quả cuối vượt Int64 trả `NUMERIC_OVERFLOW`, không saturation. `MulFixed(a,b)=RoundDiv(a*b,1000)`; thứ tự phép tính theo rule, không gộp/tái sắp xếp. Clamp chỉ ở field contract yêu cầu, ví dụ condition repair min(1000,old+gain).

## Canonical codec mdc1

Hash input là byte prefix ASCII `MDC1` nối encode(value). Null=`n`; false=`f`; true=`t`; integer=`i` + canonical signed decimal + `;` (0, không + hoặc −0). UInt64 dùng decimal không dấu trong schema field đã khai báo; encoder dùng cùng integer token. String=`s` + số byte UTF-8 ở dạng decimal + `:` + bytes. Không Unicode normalization; reject unpaired surrogate. Object keys giới hạn ASCII `[a-z][a-z0-9_]*`, unique; sort ordinal ASCII. Object=`o`+count+`:`+encoded key/value pairs; Array=`a`+count+`:`+encoded elements theo thứ tự. Không whitespace/delimiter bổ sung. Unknown fields bị schema reject trước hash. Có thể dùng JSON để author, nhưng JSON bytes không phải hash input.

Ví dụ object `{"a":1,"b":"x"}` có exact bytes `MDC1o2:s1:ai1;s1:bs1:x`. Empty object `MDC1o0:`; `[true,null,-2]` → `MDC1a3:tni-2;`, exact hex được lưu trong vectors. StateHash = lowercase SHA-256 hex của canonical snapshot projection. Snapshot hash gồm world_id, tick, revision, entities, allocators, jobs, RNG counters, accepted mutation command receipts và version manifest; không gồm UI caches, filesystem paths, wall-clock, diagnostic logs, hoặc chính hash. Arrays tương đương sets phải sort theo schema trước encode; encoder không tự đoán.

## RNG sha256-counter-v1

Chọn thuật toán đơn giản để kiểm chứng trước, chưa có performance claim. PCG32 trong nguồn là candidate, không phải algorithm v1. Context object gồm `algorithm`, `seed` (64 lowercase hex digits), `domain`, `decision`, `occurrence`, `entity`, `stream`; các string ID được schema validate. Block k = SHA-256(canonical encode(context thêm `block`: k)), k UInt64 bắt đầu 0. Đọc digest thành tám UInt32 little-endian theo thứ tự offset0,4,…28. `draw_index` persisted trỏ word kế tiếp; block=floor(index/8), slot=index%8. Không reseed từ current time hoặc runtime hashcode.

`Uniform(n)`, n∈[1,2^32]: limit=2^32−(2^32 mod n). Đọc word x, tăng draw_index; nếu x≥limit thì discard và đọc tiếp; trả x mod n. Không modulo trước rejection. `Roll2d10` = Uniform(10)+1, hai lần, rồi cộng; probability check Uniform(10000)<p. p=0/10000 được constant-fold thành false/true không draw; phải version nếu thay draw schedule. Mỗi decision/occurrence có context riêng; không share stream ngẫu nhiên giữa UI và world.

Preview đọc cloned context/snapshot, không persist draw. Abort rollback counters, ID và changes; retry cùng command trả receipt nếu đã commit, nếu chưa commit thì cùng inputs cho cùng draws. Seed và RNG không phải bảo mật/anti-cheat. Golden vectors ghi input, canonical bytes, SHA-256 và raw words; validator kiểm literal SHA-256 known answer cho `abc`, codec bytes viết tay, rounding và rejection branch trước so vectors. Cross-runtime vectors vẫn cần FND-16.

## Trạng thái triển khai Sprint 1

FND-04 hiện thực portable math/codec/contextual RNG; nghiệm thu xem Sprint 1 evidence. Command receipts/atomic persist/replay/Unity còn gate riêng. Xem [ADR-010](../adr/010-sprint-one-primitives.md) và [evidence](../qa/SPRINT-01-EVIDENCE.md).
