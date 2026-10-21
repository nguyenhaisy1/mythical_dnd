# Golden replay-kernel-v1

Baseline tạo có chủ đích từ ReplayProbe fixture ngày2026-10-21. scenario.json mô tả dữ liệu/kết quả số học được đối chiếu riêng; SHA256 của file là content hash fixture, không production content registry.

golden.mdc giữ initial image,15ordered host attempts và expected checkpoints. golden.json cố định checksum artifact và final report. Chạy CI chỉ đọc, không record/overwrite expected. Thay golden cần giải thích contract/version và review trong cùng commit, không regenerate để che failure.

Chuỗi: Drain khi chưa có workflow; transfer3; duplicate; payload mismatch; unknown; stale; noop; unauthorized; schedule2jobs quantity2 tại10/11; advance20; unauthorizedDrain;2drains budget1; retry advance gốc.

Kết quả số học: initial10/0 → transfer7/3 → jobs3/7; revision8, tick20,2executions, RNG draw index7 không đổi. Expected state/checkpoint hashes được cố định lần đầu từ C# implementation, không tự nhận là oracle độc lập cho mọi field. Counting-rule tests chứng minh replay gọi resolver lại; changed-rule/changed-checkpoint controls phải phát hiện divergence.

Hai subprocess replay phải khớp report; negativecontrol checkpoint đầu sai phải exit1/index0/categoryresult. Không chứng minh stochastic gameplay hoặc Unity.
