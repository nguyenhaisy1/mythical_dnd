# CommandPipeline — FND07/08 transfer scope

CommandEnvelope copy scalar payload, canonical fingerprint toàn semantic fields. ExecutionPrincipal phải do trusted host tạo, không đọc source_type từ UI rồi cấp quyền. CommandProcessor single-writer, duplicate trước stale, independent proposal validation, immutable CommittedWorld publication. InspectCommitted chỉ cho trusted diagnostic/composition, không player query.

BeforeCommit failure không publish hoặc lưu terminal rejection; AfterCommit failure giữ commit để retry nhận receipt gốc. Accepted receipt/eventID/world cùng publication; rejection/noop metadata không đổi world/identity/RNG/revision. Prototype chỉ có TransferResource, không SQL, jobs, full event journal/replay hoặc external notifications. Commands khác được reject rõ.

Xem docs/contracts/COMMAND-ACTIVITY-CATALOG.md, ADR011 và Sprint2 evidence. Không dùng constructor principal như authentication service đã triển khai.
