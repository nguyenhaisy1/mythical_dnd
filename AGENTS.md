# Làm việc trong repository mythical_dnd

Đọc `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` trước. Giao tiếp và tài liệu ưu tiên tiếng Việt; code identifiers theo tiếng Anh ổn định.

- Tài liệu chi tiết nằm ở `docs/gdd/modules/`, nguồn bất biến ở `docs/source/`. Không thay nghiên cứu dài bằng summary hoặc sửa archive để khớp thiết kế mới.
- Các module generated: sửa `tools/docs/integration-notes.json` cho ghi chú áp dụng và thêm ADR/source amendment khi đổi thiết kế. Chạy build_docs và kiểm tra lại, không sửa Master export đơn lẻ.
- Random character, no runtime LLM, deterministic/causal rules, knowledge isolation và owner/commit boundaries là nền bắt buộc.
- Chỉ triển khai task được yêu cầu, có requirement, input/output và acceptance rõ. Khi thiếu đặc tả, ghi design gap/ADR; không tự chuyển unknown thành engine behavior.
- Domain/Foundation không phụ thuộc Unity, DB/network hoặc wall-clock/global random để quyết định gameplay. UI/narrative không ghi authoritative state.
- Phân biệt documentation check, scaffold build và runtime acceptance. Không đánh dấu test game Passed vì chưa có implementation; không bịa benchmark hoặc Unity verification.
- Các lệnh validation và build ở README. Run check phù hợp thay đổi; cập nhật evidence nếu kết quả hoặc phạm vi thay đổi.
- Gói export là build artifact; sửa source documents rồi regenerate. Không commit bin/obj, Unity Library/Temp hoặc SDK tạm.

Yêu cầu hiện tại đã cho phép init cấu trúc project và hoàn thiện tài liệu; giới hạn “chỉ nhập .agent” của phiên trước đã được thay thế. Không tự triển khai toàn bộ gameplay từ một yêu cầu chuẩn bị tài liệu/scaffold.

## Skills của project

Source tại `tools/skills/`, hướng dẫn và cách dùng ở [PROJECT-SKILLS](docs/runbooks/PROJECT-SKILLS.md). Chọn skill đúng nhiệm vụ, không nạp tất cả. Skill không thay baseline/contract và không cấp quyền triển khai ngoài yêu cầu.
