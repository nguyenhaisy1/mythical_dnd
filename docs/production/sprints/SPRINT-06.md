# Sprint 6 — Điểm lưu và khôi phục bản lưu

## Tổng hợp dễ hiểu

Sprint này làm phần bảo vệ tiến trình đã lưu: tạo điểm khôi phục định kỳ, lưu thủ công mà không làm thời gian trong game chạy thêm, và thử khôi phục khi một phần bản lưu bị hỏng. Khi đổi định dạng lưu, hệ thống giữ bản gốc và tạo bản sao dự phòng. Đây là nền tảng bảo vệ dữ liệu; chưa phải màn hình Save/Load hoặc bản game chơi thử.

Trạng thái: đạt phạm vi headless sau256/256tests và6SIGKILLcases mới; review độc lập không còn blocker. SPIN-009; mở2026-10-20 theo yêu cầu user.

## Goal và task

FND12 headless snapshot/recovery/migration schema1→2 theo ADR015; TEST035/036/051 subset storage. Input CommittedWorld/schema1 của Sprint5. Output schema2 manifest/snapshots/recovery frames, manual snapshot và validated new-file recovery/migration. FND13 rule replay/Unity UI/atomic platform replace ngoài scope.

| Task | Actor/ownership | Acceptance |
|---|---|---|
| S6-01 baseline/ADR/report | /root; docs/worklog | ADR015, phạm vi, quy tắc báo cáo mới |
| S6-02 store/save manager | /root/s6_storage; Infrastructure/Persistence | Atomic snapshots, strict complete suffix, safe migration |
| S6-03 integration tests | /root/s6_tests; SaveRecoveryTests.cs | Cadence/manual invariance/corruption/migration/scheduler cases |
| S6-04 independent review | /root/s6_design_review; read-only | Findings sửa trước final gate; không giả review là tests |
| S6-05 host/crash/gates | /root; Headless/tools | Processkill thật, regression và negativecontrol |
| S6-06 docs/Git | /root | Đồng bộ source/exports/changelog; PR/dev sau CI |

## Điều kiện nghiệm thu

Snapshot khởi tạo và mỗi100accepted mutation commits, không đếm metadata/noop/duplicate; manual không reset cadence hay thay canonical state. Fallback chỉ thành công khi dựng đúng committed head gồm receipts; thiếu/hỏng suffix phải từ chối. Migration giữ source/backup; không overwrite existing paths. Versions/content mismatch bị chặn. Actual tests, independent review và CI phải đạt; pending gates ghi rõ.

## Kết quả

32FND12cases +224regression,0skip; negativecontrol đúng;6save/migrationSIGKILL và3current-stateSIGKILL đạt. /root tiếp quản sau storage/tests agents quota; source review và delta review bởi /root/s6_design_review. [Báo cáo nghiệm thu](../../qa/SPRINT-06-EVIDENCE.md) ghi evidence/giới hạn. Git/CI cuối tại WORKLOG; không tự mởSprint7.
