# Foundation/Random — FND-04

Primitives portable theo `docs/contracts/DETERMINISTIC-MATH-RNG-SPEC.md`:

- `CanonicalCodec`: encode/hash mdc1, ordinal object keys, UTF-8 strict không normalize, Int64/UInt64 không float. Input object là sequence `KeyValuePair<string, object?>`; arrays là `IReadOnlyList<object?>`. Caller kiểm schema/unknown fields và sort arrays có nghĩa set trước encode.
- `DeterministicMath`: RoundDiv ties away from zero với BigInteger intermediate, MulFixed scale1000; kết quả vượt Int64 ném `NumericOverflowException` có code `NUMERIC_OVERFLOW`.
- `RngContext`: seed/context bất biến, SHA-256 canonical block, tám UInt32 little-endian. Entity dùng identity-v1; các context identifier khác phải nonempty UTF-8 hợp lệ, schema của caller kiểm vocabulary nghiệp vụ.
- `RngState`: draw_index UInt64 bất biến; Draw/Uniform/Roll2d10/Probability trả value cùng Next. Preview/abort bỏ Next; persistence/commit thuộc Application ở task sau. Không wrap cursor; ở UInt64.MaxValue từ chối draw vì không thể biểu diễn next cursor. Probability 0/10000 không draw.
- `UniformSampling`: rejection trước modulo, hỗ trợ bound1…2^32.

C# golden tests đọc fixtures đã commit; có actual RNG rejection, block crossing, preview/retry/restore và overflow. Không chứng minh atomic world commit, command receipts, replay toàn thế giới hoặc Unity Mono/IL2CPP; các gate đó thuộc FND-07…16.
