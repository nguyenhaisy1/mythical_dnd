# Foundation diagnostics

Sprint9/FND15 theo ADR-018: StateDiffer so toàn bộ committed mdc1 image thành typed JSON-pointer paths; RuleInspectionHandler ghi actual transfer evaluation; CausalInspector chỉ dùng persisted links và có traversal hai chiều; SchedulerInspector nêu lý do hủy nếu có record.

Chỉ dùng developer/headless QA. Không expose omniscient state vào player projections, không authoritative mutation. Diff arrays theo index canonical, không khẳng định là entity matching. Các giới hạn/truncation và evidence package tại docs/runbooks/DIAGNOSTICS.md.
