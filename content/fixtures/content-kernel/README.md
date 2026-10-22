# Runtime kernel content fixture

`pack.json` là JSON có thể compile/load thật cho profile kernel-content-v1 theo ADR-017. Hai definitions: commodity.timber@1 và transfer.timber@1 (chuyển 3 đơn vị). Source prototype-authored, classification game-canon; không phải production roster VS-02 hoặc historical evidence.

Expected compiled hash: `e45fa90669459e4516323a71ff959d7f9b6dd637d7e33c148306ba0cb9808b72`. Hash canonical bind metadata và semantic fields, không là checksum bytes của JSON. Đổi thứ tự definitions/formatting không đổi hash. Khi cố ý sửa fixture, review expectation độc lập trước cập nhật; CI không tự ghi lại golden.

Host demo tạo world riêng 10/0 rồi chuyển theo template, save/reopen cho kết quả 7/3 revision2. Schema ở `content/schemas/kernel-content-v1.schema.json`; CLI, diagnostics và giới hạn ở `docs/runbooks/CONTENT-PIPELINE.md`.
