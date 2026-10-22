# Infrastructure/ContentLoading

Compiler và immutable registry Sprint 8 đã triển khai tại `../Content/ContentRegistry.cs`. JSON parser ở HeadlessRunner là host adapter; không thêm JSON/Unity dependency vào core. Profile kernel-content-v1 hỗ trợ commodity và transfer-template, exact dependency hashes và world-reference validation; xem `docs/runbooks/CONTENT-PIPELINE.md`. Folder này giữ đường dẫn scaffold cũ, không là implementation thứ hai.
