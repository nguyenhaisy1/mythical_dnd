# ADR-011 — Sprint 2 in-memory command boundary

Ngày2026-10-15; FND05–08. Giữ kernel-v1 semantics, không thay archive hoặc save schema. Sprint2 là implementation in-memory có failure injection; durable persistence/atomic database commit vẫn FND11.

## Quyết định

WorldRevision là UInt64 checked theo identity revision hiện tại. WorldSnapshot Domain gom identity/time, actor/lot records immutable và RNG states; không đưa Assets vào Foundation. Snapshot trả readonly collections và không nhận mutation từ rule. Snapshot builder/constructor kiểm ref world/type/lifecycle và numbers; Application tạo candidate và publish một aggregate thay cho mutate dictionaries công khai.

Approved typed RuleRegistry ánh xạ command type/version/payload type; không load script từ JSON. TransferResource là pure rule; trả proposed lot changes hoặc rejection/noop. Application kiểm owner/field/expected-before/conservation và authority boundary trước publish. Snapshot trong API này là internal omniscient domain input, không được expose như player projection.

Command envelope fingerprint mdc1 gồm toàn bộ semantic fields: command_id/type, issuer/source, tick/revision,payloadversion/payload,correlation. Transport/auth session không thuộc fingerprint, nhưng authority phải được kiểm riêng trước tiết lộ receipt. Caller không thể tự khai Scheduler/NPC để vượt quyền; Player/NPC/Scheduler đều chỉ được nhận từ trusted host-issued ExecutionPrincipal, dùng cùng Transfer rules và không có privileged bypass; chưa triển khai authentication/session service. Malformed ID không lưu; unknown/type/version reject. Duplicate cùng envelope trả receipt gốc trước stale check, mismatch không overwrite receipt.

Accepted mutation: candidate world, identity allocations, RNG state, event IDs, receipt và revision cùng aggregate publication. Rejected/noop receipt chỉ metadata; không đổi gameplay snapshot/revision, không draw/allocate. System exception là failure có thể retry, không terminal gameplay rejection. Single writer lock bảo vệ read-check-apply-publish; độc giả chỉ thấy immutable snapshot hoàn chỉnh.

Minimal ResourceTransferred records đủ phục vụ receipt và test boundary; không gọi đó là full FND09 causal event model. Transfer không có jobs hoặc RNG draws, giữ nguyên RNG state; các claims atomicity chỉ cho fields đã có trong prototype. Failure injection before publish không thay store; after publish response failure không rollback commit và retry không duplicate.

## Phạm vi chưa có

SQLite/process-crash recovery, durable receipts, schema/migration/full-statehash, gameplay rulepack đầy đủ, scheduler, replay và Unity đều pending. In-memory tests không nghiệm thu TEST006 durability. TEST029/039 chỉ kiểm conservation/material reservation trên path transfer hiện có; chưa có repair/production lifecycle.

Xem [Sprint2](../production/sprints/SPRINT-02.md) và [evidence](../qa/SPRINT-02-EVIDENCE.md) để biết API thực tế, errors bổ sung và kết quả thực thi.

## Chi tiết error và metadata

INVALID_ENVELOPE, INVALID_PAYLOAD, COMMAND_UNKNOWN, PAYLOAD_VERSION_UNSUPPORTED, COMMODITY_MISMATCH, REF_NOT_ACTIVE bổ sung diagnostic cho prototype. PROPOSAL_* / REVISION_EXHAUSTED / REENTRANT_COMMAND / COMMIT_CONFLICT là system failures, không persisted như gameplay rejection.

Malformed command_id hoặc failed trusted-principal match trả ephemeral rejection, không lưu metadata để tránh attacker chiếm commandID. Command có validID/auth nhưng invalidpayload/ruleprecondition mới lưu rejection metadata. Auth được kiểm trước receipt lookup; duplicate fingerprint được kiểm trước stale/time. Mismatch không thay receipt gốc.

Source=destination chỉ no-op sau validation quantity>0,authority,alive,available. Transfer cần hai lot cùng location, không thêm actor-colocation ngoài catalog. Controls household là authoritative explicit list, không suy từ tên.

WorldSnapshot ở đây là subset actor/lot/identity/time/RNG, không fullgame schema. RNG ContextKey là host label opaque, có thể alias cùng canonical context; cần enforce mapping/uniqueness trước stochastic command integration. Sprint2 chỉ giữ RNG states không draw; không gọi đó là RNG transaction behavior đã nghiệmthu.
