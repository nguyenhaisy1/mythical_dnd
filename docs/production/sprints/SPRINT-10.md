# Sprint 10 — Chuẩn bị kiểm chứng nền tảng trong Unity

## Tổng hợp dễ hiểu

Đối chiếu phần nền đã xây qua chín sprint và chuẩn bị một bộ kiểm tra dùng chung cho chương trình .NET và Unity. Bộ kiểm tra phát hiện sai lệch dữ liệu, kết quả ngẫu nhiên, phát lại hành động và lưu game giữa hai môi trường.

SPIN-013. Trạng thái đang thực hiện; chưa đóng sprint vì máy chưa có Unity được cài/kích hoạt. Không có bản chơi thử, không tuyên bố Foundation complete.

## Phạm vi và acceptance

| Task | Input → output | Acceptance / actor |
|---|---|---|
| S10-01 | Evidence FND02–15 → VS01 audit | Đối chiếu giới hạn từng sprint, không suy full gameplay; /root |
| S10-02 | Fixed vectors/replay + portable core → shared probe | Known answers, commit/reopen/snapshot/refusal, host process determinism; /root |
| S10-03 | Locked dependencies + Editor path → isolated conformance project | Hash inventory, actual ProjectVersion/backend/API, same outputs; missing Editor NotRun; /root |
| S10-04 | Source + negative controls → independent review | /root/s10_final_review; findings phải được sửa trước integration |
| S10-05 | Final evidence → docs/exports/PR | Checks thật; Unity pending hiển thị rõ; /root |

Các agents s10_audit/s10_unity/s10_review đóng góp đề xuất ban đầu trước quota, không ghi họ đã implement hoặc duyệt code. Root tiếp quản. Không triển khai VS02/gameplay mới.

## Gates còn mở

FND16 chỉ được nghiệm thu sau actual Editor run và đối chiếu đúng profile, không lấy synthetic validator tests làm Unity evidence. Candidate Linux x64 Editor Mono chưa chứng minh Player/IL2CPP hoặc packaging. OPEN03/04 chưa đóng; phiên bản Editor phải lấy từ cài đặt thật. Xem [runbook](../../runbooks/UNITY-CONFORMANCE.md) và [evidence](../../qa/SPRINT-10-EVIDENCE.md).
