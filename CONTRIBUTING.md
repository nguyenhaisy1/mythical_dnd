# Quy trình đóng góp

1. Chọn task trong `docs/production/BACKLOG.csv`; đọc module đầy đủ và requirement/tests tương ứng.
2. Ghi rõ owner domain, contract và files cần đổi. Nếu design gap ảnh hưởng public contract, cập nhật decision/open-issue và ADR trước implementation phụ thuộc.
3. Code core portable, changes qua application commit path; content theo approved handler/schema, UI qua actor-filtered projections.
4. Run documentation/architecture/content checks cùng build và runtime tests liên quan đã có. Kết quả test mới ghi versions/seed/fixture.
5. Review mô tả hành vi trước/sau, ảnh hưởng state/save/knowledge, acceptance evidence và gaps còn lại. Không viết “hoàn tất” nếu mới tạo interface hay stub.

Không sửa nguồn hội thoại nhập. Sửa các ghi chú tích hợp/governance khi quyết định đổi; dùng source amendment rõ provenance cho nội dung mới. Mọi export được dựng lại từ source.

## Branch và phát hành

Theo [Git Workflow](docs/runbooks/GIT-WORKFLOW.md): work branch từ dev, PR về dev, release/hotfix về main và đồng bộ lại dev. Commit theo Conventional Commits; cập nhật [CHANGELOG](CHANGELOG.md) cho thay đổi đáng chú ý. Product version ở [VERSION](VERSION), không lấy version GDD làm version game. Kiểm staged scope và evidence trước commit/push; không tự tag/publish ngoài yêu cầu.

Quy trình áp dụng cho AI: [AI Execution](docs/runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope Sprint 1 nằm trong sprint brief được liên kết tại đó.
