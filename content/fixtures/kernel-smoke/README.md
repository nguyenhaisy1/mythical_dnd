# Kernel smoke — tài liệu đầu vào và kết quả đối chiếu

`scenario.json` có hai actor, một household, thuyền, timber lots, route và chuỗi transfer → reserve repair → advance/complete → transfer rights. Assumptions giải thích setup về skill/tool/authority. Alias trong payload được test adapter resolve qua bảng entities trước gửi command; envelope adapter bổ sung issuer=actor_1, source_type=Player, payload_version=1, correlation_id=kernel_smoke. Dùng đúng command_id/revision/tick được ghi; retry reuse nguyên envelope.

Đây là **projection fixture cho contract**, chưa là full snapshot loadable của engine. Hash trong file chỉ chứng minh codec/projection bytes, không phải full-world StateHash hoặc replay đã pass. Full snapshot phải thêm registry/allocator, locations, rules/content manifests, command receipts/events và scheduler workflow state khi FND-05/13 được hiện thực. Route là reference tối thiểu, chưa có travel behavior trong smoke này.

`vectors.json` chứa canonical bytes, SHA-256 và raw RNG words từ Python stdlib reference. Script kiểm known-answer SHA-256, bytes viết tay, negative codec inputs, rejection branch và arithmetic expectations:

```bash
python3 tools/checks/check_contract_vectors.py
```

Không tự cập nhật golden từ implementation đang test. FND-02/04 phải đọc cùng vectors trên C#; FND-16 so Unity; FND-07/13 phải chạy scenario qua real handlers và đối chiếu từng commit. Script hiện tại không thực hiện những runtime tests đó. Negative cases trong scenario là test specifications cần triển khai, không được tính vào số runtime tests passed.
