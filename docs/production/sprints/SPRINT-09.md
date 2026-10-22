# Sprint 9 — Tìm nguyên nhân lỗi và chạy lại bằng chứng

## Tổng hợp dễ hiểu

Sprint này bổ sung công cụ cho biết game đã kiểm tra điều kiện nào, dữ liệu nào đổi và sự kiện/công việc nào dẫn tới kết quả đó. Khi một kết quả khác với dự kiến, gói bằng chứng giúp chạy lại và chỉ ra bước đầu tiên sai cùng giá trị cụ thể trước/sau.

Trạng thái: đạt headless scope ngày2026-10-22;338/338tests và16diagnostic subprocesses. SPIN-012; FND-15. Công cụ phục vụ phát triển/kiểm thử, chưa là tính năng giao diện cho người chơi hoặc bản game chơi thử.

## Phạm vi và phân công

Input: committed states, actual evaluated rules, replay actions và exact versions/content/seed. Output: full field diff, rule/command traces, causal/job inspector, bounded reproducible evidence package. Quyết định [ADR-018](../../adr/018-foundation-diagnostics.md).

| Task | Actor/ownership | Acceptance |
|---|---|---|
| S9-01 | /root: contract/Domain checks/replay observer | Actual checks, không thêm lần evaluate hoặc đổi gameplay |
| S9-02 | /root/s9_diagnostics: Infrastructure/Diagnostics | Full typed diff, actual rule trace, causal/job views |
| S9-03 | /root/s9_tests rồi /root: Fnd15 tests | Bounds/immutability/retry/early-stop/observer/causal/cancel |
| S9-04 | /root/s9_review | Independent source và delta review |
| S9-05 | /root: host/fixture/process gate | Hai process tái hiện fixed fault, negative controls |
| S9-06 | /root: docs/exports/Git | Full regression, evidence, PR vào dev sau CI |

## Exit gates và giới hạn

Reproduce injected failure từ package, baseline phải khớp artifact/expected image trước fault. Field diff không suy từ hash; rule checks không được bịa PASS; correlation không là causation. Các giới hạn có dropped/truncated rõ. Full regression315tests, content/replay/process crash gates trước đó phải tiếp tục đạt.

Không tự mở Sprint10, release, Unity hoặc toàn bộ vertical slice. VS01 vẫn cần milestone acceptance riêng; Foundation complete cần FND16. Báo cáo tiếng Việt dễ hiểu trước chi tiết.

## Kết quả

[Báo cáo nghiệm thu](../../qa/SPRINT-09-EVIDENCE.md): independent review và process reproduction đạt, toàn bộ regression315cases giữ xanh. Root tiếp quản tests/integration sau agents quota. VS01 milestone review và FND16 Unity còn riêng.
