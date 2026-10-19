# SQLite provider spike — Sprint5/headless

Ngày2026-10-19. Mục tiêu chọn provider để kiểm atomic transaction trên Linux headless, so sánh hướng Unity nhưng không kết luận khi chưa chạy Editor/backend.

| Khía cạnh | Microsoft.Data.Sqlite10.0.13 | gilzoide/unity-sqlite-net1.3.3 |
|---|---|---|
| Evidence | NuGet nuspec/lock restore, build và runtime file tests | Repository README/package/source review, chưa runtime |
| License | MIT của Microsoft binding; SQLitePCLRaw Apache-2.0, SQLite native public-domain upstream | Repo MIT; native SQLite3 Multiple Ciphers có license riêng phải audit khi đóng gói |
| Binding | ADO.NET trên SQLitePCLRaw | sqlite-net với native plugins cho Unity |
| Packaging | Exact package + transitive lock; bundle_e_sqlite3/core/provider/lib2.1.13; runtime này Linux x64 | UPM tag1.3.3, plugin binaries theo platform; không cài vào client trong sprint |
| Quyết định | Provisional headless adapter | Candidate để kiểm native/Unity cross-format sau |

Sources chính: [Microsoft custom SQLite versions](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/custom-versions), [transactions](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/transactions), [Unity candidate source](https://github.com/gilzoide/unity-sqlite-net), [SQLite synchronous](https://sqlite.org/pragma.html#pragma_synchronous). Microsoft provider package nuspec10.0.13 được đọc trực tiếp từ NuGet; netstandard2.0 asset phù hợp compile core target, không suy Unity runtime.

Môi trường: /tmp/mythical-dotnet/dotnet10.0.401/Linuxx64. Không tìm thấy Unity executable trong PATH hoặc các thư mục Editor thông dụng đã kiểm; không chạy Mono/IL2CPP/native mobile. OPEN03 chưa đóng: cần chạy target client đọc cùng format, transactions/rollback/native loading và packaging license review.

Runtime tests/crash evidence cuối xem [Sprint5 evidence](SPRINT-05-EVIDENCE.md). SQLite engine version thật lấy từ SELECT sqlite_version trong subprocess report; không suy từ package number. Tests actual busy/full, injected table failures và SIGKILL có phạm vi khác nhau; không phải power-loss hoặc performance benchmark.
