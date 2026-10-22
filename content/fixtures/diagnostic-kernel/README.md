# Diagnostic evidence fixture

`package.json` là gói bằng chứng diagnostic-v1 gồm replay artifact, manifest/seed/content hash, expected committed image tại action1 và fault ID đã biên dịch `reject-transfer-v1`. Không có script hoặc đường dẫn máy gốc. Registry dùng `../content-kernel/pack.json`; actions lấy từ fixture replay15actions, record một lần với content hash mới.

`golden.json` pin checksum package và report stdout. CI chỉ đọc, không regenerate. Hash được tạo từ implementation lần đầu và review; kết quả vật chất đối chiếu riêng: baseline action1 chuyển3, lots7/3 revision2; fault cố ý từ chối sau actual evaluation nên lots10/0 revision1. Lệch đầu index1/categoryresult. Baseline chạy đủ15actions/hai jobs, fault dừng sau2actions. Các graph/jobs baseline được gắn nhãn rõ, không giả là state của fault run.

Rule inspector lấy checks/effects từ lần Evaluate thật. State diff so expected image với actual image, không suy từ hash. Đây là QA omniscient, không player view và không sửa production save.
