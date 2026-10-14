# Bản đọc sinh từ tài liệu project

Chạy `python3 tools/docs/export.py`, sau đó `python3 tools/checks/check_exports.py`.

Master GDD, năm volume và Development Handbook có Markdown/HTML/DOCX. Nguồn làm việc ở `docs/`; không sửa artifact thay source. Không tạo ZIP. EXPORT-MANIFEST ghi fingerprints của inputs và hashes của outputs để phát hiện bản đọc cũ. Bản build lớn không track mặc định, có thể tái tạo từ repository.
