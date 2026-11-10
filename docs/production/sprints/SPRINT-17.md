# Sprint17 — Đan dây từ xuất thân và giữ tiến trình công việc

10/11/2026 · SPIN-041 · Chủ dự án giao tiếp tục sau Sprint16 đã đóng; nghiệm thu kỹ thuật local10/11/2026, Git/CLOSE saucommit theoWORKLOG.

## Tổng hợp dễ hiểu

Nhân vật đã biết đan có thể xem vật liệu cá nhân, xác nhận làm dây, chờ và nhận sản phẩm thật; hủy trả lại phần vật liệu đang giữ. Lưu giữa lúc làm rồi mở ứng dụng lại phải tiếp tục đúng công việc, không mất hoặc nhân đôi đồ. Hai người đã có kỹ năng là Lâm và bà Nhu; các xuất thân khác được ghi rõ công việc phù hợp chưa có trong bản thử, không đổi nghề của họ.

[ADR026](../../adr/026-personal-weaving-on-origin.md) và [contract](../../contracts/PERSONAL-WEAVING-PROFILE.md) là input trước code. Scope là personal craft subset trên origin; không fullVS03/CharacterViability hoặc world/NPC binding. Nguyên liệu mới là prototype personal starter kit, không lấy quyền hộ gia đình. ArtG13/01–18 và profiles15/16 giữ.

| Task | Output và acceptance | Ownership |
|---|---|---|
|S17-01|DomainWeaving, WorldSnapshot propagation, commands/jobs/history/invariants, core tests|Core worker sau readonly audit|
|S17-02|Strict parameters/profile compiler, codecimage4/SQLite, atomic gateway, safe session/DTO, persistence tests|Runtime worker sau audit|
|S17-03|Scene17/View/Bootstrap/QA/Build, preview+Confirm+running/output/nonweaver UI|Unity worker|
|S17-04|Independent probes/process/static runners, actual Unity/OS/legacy regression/fullsuite/docs/CI/Git|/root|
|S17-QA|Independent core/rights/privacy/history và actual UI/artifact reviews; sửa findings|Critics luân phiên|

Max4actors đồng thời kể cả root; ownership/actors thực ghi WORKLOG. Cả năm gates trong contract phải đạt mới CLOSE. Báo cáo Việt dễ hiểu trước, tách điều kiện đóng còn thiếu và backlog ngoài sprint. Commit/pushfeature/PRdev/integrate sau exactHEADCI theo workflow, không tự Sprint18/main/tag/release.

## Kết quả kỹ thuật local10/11/2026

Kết quả kỹ thuật local10/11:770/770runtime tests,0failed/skipped; actual Unity6000.6.7f1/LinuxMono ba xuất thân/116ảnh mới và31ảnh regression15–16;48native actions/hai process SaveRunning→restart→complete một lần. Hai final reviews độc lập không còn P0/P1 trong scope. Docs/exports và exactHEADCI/PR→dev phải đạt trước CLOSE; Git sau commit theo WORKLOG.

[Evidence](../../qa/SPRINT-17-EVIDENCE.md) ghi rawpaths/hashes/actors/failures/fixes/independent reviews. Backlog ngoài sprint vẫn riêng, không chuyển gate đã giao thành backlog để đóng.
