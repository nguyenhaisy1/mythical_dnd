# Sprint16 — Nhận xuất thân ngẫu nhiên và tiếp tục cuộc đời đã lưu

06/11/2026 · SPIN-040 · Được chủ dự án giao; nghiệm thu kỹ thuật local09/11/2026, đóng sau docs/Git/CI theo WORKLOG.

## Tổng hợp dễ hiểu

Từ bản thử một người cố định, bổ sung nhận một nhân vật ngẫu nhiên từ nhóm đã biên soạn tại Bến Vạc. Người chơi đọc tuổi/gia đình/quá khứ/kỹ năng/nhận định của chính mình, bắt đầu vào cảnh, chờ, lưu và mở lại đúng người đó. Không chọn xuất thân hoặc quay lại bốc người khác ngay trong Reveal; campaign mới được lưu riêng.

Đây nhánh chọn nhân vật có sẵn theo Module07, không phải toàn bộ bộ sinh lịch sử cuộc đời. Không tự cấp nghề sửa thuyền cho người chưa có nghề. Repair proof15 giữ riêng và regression; hướng Đất dựng thành/G13 giữ.

## Tasks và gates

ADR025 và ORIGIN-COHORT-PROFILE là input chuẩn trước implementation.

| Task | Output/acceptance | Ownership |
|---|---|---|
|S16-01|Typed origin model/selector/RNG, snapshot propagation, deterministic/admission tests|Core worker sau audit|
|S16-02|Derived cohort registry, image3/SQLite exact profile, safe gateway/session, atomic create/publish/Continue tests|Runtime worker sau audit|
|S16-03|Actual menu/Generating/Reveal/World/self-record, neutral marker, build/autoplay/input evidence hooks|Unity worker|
|S16-04|Host/export/process/static/runtime runners, actual Editor/Player/OS, regression/docs/Git|/root|
|S16-QA|Independent core/persistence và UI/artifact reviews, fixes/noP0P1|Critics sau worker freeze|

Max4actors đồng thời kể cả root; actor thật và assignments/results ghi WORKLOG. Đủ cả5gates trong contract mới đóng, không đưa gate còn thiếu ra backlog. Feature→PR→dev sau exactHEADCI đạt; không main/tag/release/Sprint17.

## Backlog dự án ngoài sprint

Full life generation/Character Viability/TEST001, allNPC/economy/Knowledge binding, nghề khác hoặc repairqualification cho cohort, fullVS02/VS07, fullart/ownerUXapproval/performance/humanplaytest. Báo cáo tách danh sách này khỏi Điều kiện đóng Sprint16 còn thiếu.

## Kết quả kỹ thuật local09/11/2026

674/674runtime tests,0failed/skipped,81ca mới; actual process21calls/3SIGKILL và negative control đạt. Unity6000.6.7f1/LinuxMono hai seed/38origin captures/sixlayouts mỗi lượt, OS20actions/hai process restart và repair15 actual Player regression đạt. [Evidence](../../qa/SPRINT-16-EVIDENCE.md) ghi paths/hashes/actors/failures và independent reviews. Docs/exports/exactHEADCI/PR→dev bắt buộc trước CLOSE; Git sau commit ở WORKLOG. Không tự Sprint17/main/tag/release.
