# Quy trình đóng góp

1. Chọn task trong `docs/production/BACKLOG.csv`; đọc module đầy đủ và requirement/tests tương ứng.
2. Ghi rõ owner domain, contract và files cần đổi. Nếu design gap ảnh hưởng public contract, cập nhật decision/open-issue và ADR trước implementation phụ thuộc.
3. Code core portable, changes qua application commit path; content theo approved handler/schema, UI qua actor-filtered projections.
4. Run documentation/architecture/content checks cùng build và runtime tests liên quan đã có. Kết quả test mới ghi versions/seed/fixture.
5. Review mô tả hành vi trước/sau, ảnh hưởng state/save/knowledge, acceptance evidence và gaps còn lại. Không viết “hoàn tất” nếu mới tạo interface hay stub.

Không sửa nguồn hội thoại nhập. Sửa các ghi chú tích hợp/governance khi quyết định đổi; dùng source amendment rõ provenance cho nội dung mới. Mọi export được dựng lại từ source.
