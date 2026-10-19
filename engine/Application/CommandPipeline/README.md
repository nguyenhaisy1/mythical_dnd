# CommandPipeline — FND07/08 transfer scope

CommandEnvelope copy scalar payload, canonical fingerprint toàn semantic fields. ExecutionPrincipal phải do trusted host tạo, không đọc source_type từ UI rồi cấp quyền. CommandProcessor single-writer, duplicate trước stale, independent proposal validation, immutable CommittedWorld publication. InspectCommitted chỉ cho trusted diagnostic/composition, không player query.

BeforeCommit failure không publish hoặc lưu terminal rejection; AfterCommit failure giữ commit để retry nhận receipt gốc. Accepted receipt/eventID/world cùng publication; rejection/noop metadata không đổi world/identity/RNG/revision. Prototype có TransferResource, event journal, scheduler và AdvanceTime theo ADR013. CommittedWorld giữ scheduler cùng publication; không SQL/durability/replay hoặc external notifications. Commands ngoài catalog implemented được reject rõ.

Xem docs/contracts/COMMAND-ACTIVITY-CATALOG.md, ADR011 và Sprint2 evidence. Không dùng constructor principal như authentication service đã triển khai.
