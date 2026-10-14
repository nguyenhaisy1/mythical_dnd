# ADR-006 — Portable core, .NET host và Unity adapter

Ngày: 2026-10-13. Trạng thái: baseline áp dụng cho lần bàn giao. Nguồn: DEC-016/023; M28/29.

## Bối cảnh

Dự án ưu tiên nội dung và cần headless tests/multi-year simulation; rule phụ thuộc MonoBehaviour sẽ khó replay/test.

## Quyết định

Khởi tạo Foundation/Domain/Application/Infrastructure libraries netstandard2.1 và net10.0 host, SDK 10.0.401. Client Unity giữ layout riêng, bootstrap bằng Editor thật ở FND-16. SQLite chưa chọn provider.

## Hệ quả và kiểm chứng

Build core chạy ngoài Unity nhưng chưa chứng minh Unity backend compatibility. C#9 dùng ở scaffold; public contracts cần cân nhắc portable APIs. Thêm domain logical folders, chưa chia mỗi subsystem thành assembly.

## Phương án khác

Bác bỏ Unity monolith chứa rule UI; không chuyển dự án thành web app/backend vì công cụ có sẵn.
