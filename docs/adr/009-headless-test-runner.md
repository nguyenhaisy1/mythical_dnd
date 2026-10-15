# ADR-009 — Headless test runner và identity staging API

Ngày quyết định: 2026-10-15. Scope: FND-02, REQ-027/TEST-027. Không đổi identity-v1 wire, namespace hoặc allocation semantics của ADR-008.

## Quyết định

Headless tests target net10.0, dùng NUnit 4.4.0, NUnit3TestAdapter 5.0.0, Microsoft.NET.Test.Sdk 17.14.1, exact PackageReference ranges và packages.lock.json commit vào repository. Chọn các phiên bản cụ thể đã restore và sẽ kiểm thực thi tại gate, không khẳng định là phiên bản mới nhất. Portable Foundation không có package dependency mới.

[Hướng dẫn NUnit chính thức](https://docs.nunit.org/articles/nunit/getting-started/dotnet-core-and-dotnet-standard.html) mô tả adapter + Test SDK để chạy dotnet test. Dùng VSTest/TRX hiện tại; không bật MTP mode. NUnit trong Unity là môi trường khác, phải chọn adapter/backend qua spike FND-16; headless NUnit 4 không tự chứng minh Unity dùng được cùng test assembly.

Lệnh chuẩn `python3 tools/tests/run_runtime.py` restore locked, build Release, chạy suite và negative control riêng. Gate từ chối zero tests, skipped, failed hoặc thiếu TRX; negative control phải có đúng một assertion thất bại và exit khác 0. JSON evidence cùng logs/TRX/source manifest nằm dưới tests/results; CI upload kể cả khi fail.

Không tạo integration project rỗng để lấy số lượng. Integration storage/command tests sẽ được thêm khi implementation tương ứng FND-07/11 tồn tại; FND-02 kiểm registry và allocator trong bộ primitive thực.

## API và ranh giới

EntityId và DefinitionId là immutable reference value objects, EntityId<T> dùng closed marker vocabulary cho typed API; null được biểu diễn bằng nullable reference. Không có default struct mang ID zero. Definition version giữ decimal string theo regex, không tự thêm giới hạn Int32 ngoài spec.

IdentitySnapshot giữ cursors cho đủ 10 namespaces và registry records immutable, trả danh sách stable-sorted. Restore kiểm duplicates, loại/world/revision/counter không nhất quán. Counter đã bỏ qua không tự giảm khi restore. GetHashCode chỉ dùng lookup nội bộ, không dùng cho save/replay/hash canonical.

BeginStage(targetRevision) → Allocate/Retire/Tombstone → PrepareSnapshot hoặc Abort. Candidate snapshot không tự publish. FND-05/08 phải kiểm source revision và commit cả world cùng candidate; không dùng API này để tuyên bố có atomic durable commit. Stage đã seal/abort không dùng lại. Lifecycle Staged tồn tại trong stage, không được serialize; Retired/Tombstone giữ canonical ID và revisions. Caller cung cấp retention eligibility; persistence policy chưa triển khai.

## Consequences và evidence

Có thể nghiệm thu headless primitive riêng, Unity còn pending. Cùng AI viết code và review được ghi là self-review. Xem [evidence FND-02](../qa/FND-02-EVIDENCE.md). Quyết định không làm thay đổi GDD archive hoặc golden; consumer docs, CI, backlog và host status phải đồng bộ.
