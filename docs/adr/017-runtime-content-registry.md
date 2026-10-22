# ADR-017 — Runtime content registry cho kernel hiện hành

Ngày: 2026-10-21. Trạng thái: quyết định triển khai Sprint 8; acceptance theo evidence của sprint.

## Bối cảnh

FND-14 yêu cầu compiler và registry thật; validator Python của design fixture chỉ kiểm dữ liệu tham chiếu. TEST-017 yêu cầu từ chối handler/script không được phép trước mutation; TEST-036 yêu cầu pin chính xác content khi mở save. Hiện kernel có ResourceLot và TransferResource, chưa có contracts runtime cho đầy đủ NPC, route, recipe, event AST.

## Quyết định

Tạo profile runtime ban đầu cho commodity và transfer-template. Template chỉ cấu hình commodity, lượng chuyển và handler TransferResource phiên bản đã biên dịch. Source/destination/issuer là instance IDs cung cấp lúc thực hiện; template không có quyền tự tạo tài nguyên, sửa quyền sở hữu hay viết state path. Command vẫn qua pipeline authorization, constraints, invariants và atomic commit hiện có.

Host .NET đọc JSON strict với System.Text.Json có sẵn: giới hạn kích thước/depth/count, không nhận duplicate/unknown fields hoặc script, diagnostic có definition/path. Infrastructure .NET Standard nhận typed input để compile semantic và tạo registry bất biến; không thêm dependency JSON hoặc Unity vào core. Schema là hợp đồng authoring, compiler runtime vẫn kiểm độc lập.

Pack chứa identity/version/schema, dependencies pin id/version/hash và metadata nguồn/phân loại/owner/display cho mỗi definition. Hash SHA-256 canonical bind nội dung, phiên bản và dependency hashes; sắp xếp ordinal các tập hợp không có thứ tự ngữ nghĩa. Không dùng giờ hệ thống hoặc global random. Chỉ registry đã compile thành công được dùng để resolve references. Thiếu dependency, pin sai, ID trùng hoặc unknown definition type bị từ chối rõ.

Host resolve exact registry trước khi tạo/mở store cho luồng content mới. Load save kiểm content hash hiện có, sau đó kiểm commodity references của world trước khi đưa vào processor. Không tự substitute pack cùng tên nhưng khác hash, không hot reload giữa campaign. Bộ probe của các sprint cũ giữ nguyên fixture/hash phục vụ hồi quy, không được trình bày như luồng production content.

## Giới hạn và ảnh hưởng

Đây là quyết định triển khai mới, không sửa nghiên cứu archive. Không mở rộng sang roster VS-02 hoặc cho phép AST/script tổng quát. Thêm record type sau này cần runtime contract, schema, compiler, semantic tests và compatibility decision tương ứng. Save schema, replay artifact, RNG và codec không đổi; hash pack mới khác hash fixture cũ nên không tự chuyển save cũ sang pack mới.

Portable compiler có runtime headless evidence; JSON parser là host adapter. Unity loader/provider còn gate riêng FND-16. JSON source đã kiểm định được compile khi khởi động; chưa có binary pack distribution/cache. Compiled registry là output trong bộ nhớ, không phải JSON nguồn được tin cậy trực tiếp.
