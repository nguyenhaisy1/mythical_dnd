# Bàn giao phát triển — 1.1-prototype-contracts

## Những gì đã có

29 module GDD chi tiết và phụ lục Life Event, source archive, source/section index, baseline/decision register, open decisions, ownership/contracts, QA matrix, scenario specifications và implementation backlog. Repository có .NET solution với portable core primitives, Domain world/rules, Application in-memory pipeline và headless transfer demo. Client Unity có layout chuẩn bị, chưa được Unity Editor bootstrap.

Nội dung nghiên cứu không bị thay bằng bản tóm tắt. Phần mô tả áp dụng được đặt đầu mỗi module để dev biết đoạn đề xuất cũ nào không còn hiệu lực. Các export tập hợp cùng nội dung làm việc, không dựa vào gói ZIP 13 trang của phiên cũ.

## Cách đọc theo vai trò

| Vai trò | Đọc trước | Đọc sâu khi thực hiện |
|---|---|---|
| Tech lead | ARCHITECTURE, Applicable Baseline, Ownership, Backlog | M25–29, Foundation Contract, QA |
| Game designer | M01/07/22, Decision Register | M08–21 theo feature; M27 cho công thức |
| Narrative/world | M02–06, 06A, Canon Register | M16/17/19/21, content guides |
| Gameplay engineer | Ownership, M09/12/13, M27 | Feature-specific module + transactions |
| UI/client | M23, knowledge constraints, Unity bootstrap | M11/14/16/17/21/22 |
| QA | Requirements, Test Matrix, scenarios | Module quality gates, replay/recovery |

## Sẵn sàng bắt đầu ở đâu?

Sprint 1 có code/tests identity/time/codec/RNG; xem [sprint evidence](../qa/SPRINT-01-EVIDENCE.md) và backlog. Sprint2 có WorldSnapshot/rules/command/invariant path in-memory, xem [evidence Sprint2](../qa/SPRINT-02-EVIDENCE.md). Sprint3 có event-v1/causal journal và idempotent diagnostic consumer, xem [evidence Sprint3](../qa/SPRINT-03-EVIDENCE.md). Sprint4 có scheduler/AdvanceTime in-memory, xem [evidence Sprint4](../qa/SPRINT-04-EVIDENCE.md). Sprint5 có headless SQLite current-state commit/reopen, xem [evidence Sprint5](../qa/SPRINT-05-EVIDENCE.md). Sprint6 bổ sung snapshot/recovery/migration schema1→2 sang file mới; xem [evidence Sprint6](../qa/SPRINT-06-EVIDENCE.md) trước khi nhận task. Sprint7 bổ sung headless replay QA từ initial state và host actions; xem [evidence Sprint7](../qa/SPRINT-07-EVIDENCE.md). Sprint8 bổ sung compiler/registry commodity/transfer-template và exact content/save compatibility; xem [evidence Sprint8](../qa/SPRINT-08-EVIDENCE.md). Sprint9 có actual checks/full field diff/causal-job inspector và fixed evidence reproduction; xem [evidence Sprint9](../qa/SPRINT-09-EVIDENCE.md). Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. OPEN03 còn Player packaging và platform atomic replacement. Không có P0 ở mức “thiếu toàn bộ thiết kế” buộc phải viết thêm module; các lựa chọn kỹ thuật còn mở phải được xử lý trong task tương ứng trước khi merge feature phụ thuộc.

Foundation chỉ được nghiệmthu ở kernel subset và Editor profile đã kiểm; chưa có playable client, full game balance hoặc full-production content đủ. Xem `OPEN-DECISIONS.csv` và kết quả kiểm tra thực tế. Khả năng 1.000 nhân vật là mục tiêu sản phẩm, không là test đã pass.

## Cập nhật tài liệu

Giữ archive nguyên bản. Sửa baseline/ADR hoặc integration notes nếu thay quyết định; thêm source amendment khi viết lại body lớn. `build_docs.py` kiểm tra mapping và bảo toàn nguồn; nó không phải công cụ để tự xóa phần dài. Regenerate exports sau khi docs validation pass. Không sửa Master export riêng rồi bỏ quên module source.

## Điểm dừng thiết kế

Bộ nghiên cứu đã bao phủ các hệ chính và nền kỹ thuật. Công việc tiếp theo có giá trị là hiện thực hóa, đo đạc và kiểm chứng từng milestone. Chỉ quay lại thiết kế khi có một design gap cụ thể được ghi tác động và acceptance; không kéo dài chuỗi Module 30/31 chỉ để làm tài liệu lớn hơn.

## Đặc tả triển khai bổ sung — 1.1

Đọc [Documentation Completion](DOCUMENTATION-COMPLETION-2026-10-14.md) trước khi giao task. Identity/time/RNG/codec/activity/save/UX/test execution/content authoring đã có tài liệu bổ sung; gaps cần code/spike vẫn được giữ minh bạch trong register. VS-01 là headless kernel gate, Foundation complete cần FND-16. Không tạo ZIP; bản làm việc là repository và các export được kiểm source freshness.

## Workflow hỗ trợ bằng skills

[Bộ skill của project](../runbooks/PROJECT-SKILLS.md) dẫn từ task đến contracts và evidence, cùng workflow prototype/content/change impact. Không thay backlog hoặc gates; việc tạo/cài skills không đánh dấu runtime task hoàn tất.

## Tích hợp và phát hành

Theo [Git Workflow](../runbooks/GIT-WORKFLOW.md); product VERSION và CHANGELOG ở root. Git tag không thay milestone acceptance. main hiện có initial scaffold snapshot, chưa là game release.

## Tiếp nhận công việc

Đọc [WORKLOG](../../WORKLOG.md) và các events mới nhất trước khi tiếp tục spin; cập nhật assignment/result/close khi có thay đổi đáng kể. Hồi cứu SPIN-000 không phải runtime acceptance. Nhật ký chỉ append, không sửa kết quả cũ.

Quy trình áp dụng cho AI: [AI Execution](../runbooks/AI-EXECUTION.md). Vai trò chuyên môn trong tài liệu được AI đảm nhiệm; trạng thái và scope các sprint nằm trong sprint brief được liên kết tại đó.

## Sprint10 — chuẩn bị Unity conformance

[Bộ kiểm chứng và cách chạy](../runbooks/UNITY-CONFORMANCE.md) đối chiếu host với Editor thật. Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Player/IL2CPP, Unity crash/recovery/migration, full gameplay và playable client chưa nghiệmthu. Client game vẫn layout; project conformance tạo riêng bằng Editor thật.

## Sprint11 — VS02A: dữ liệu dân cư và mạng đường

[Compiler và cách dùng](../runbooks/SETTLEMENT-CONTENT.md) kiểm24actors/6households/4communities/8routes/24claims/24directedrelationships, tạo immutable authoring registry. VS02 full vẫnpartial: chưa8–12eventtemplates/eligibility, actor-specificobligations, randomprotagonist, routine execution hoặc gameplay. Không coi đủrecordcount là worldsimulation.

## Sprint12 — điều kiện sự kiện và mốc demo

[Event Eligibility](../runbooks/EVENT-ELIGIBILITY.md) mô tả tám templates, strict compiler và evaluator vật lý/nhận thức theo [ADR021](../adr/021-event-eligibility-profile.md). Chỉ trusted observations, không occurrence/effects hoặc player projection. FullVS02 còn random protagonist, actor-specific obligations và world binding. [Lộ trình chơi thử](PLAYABLE-ROADMAP.md) đặt forecast mới: hình ảnh Sprint13, demo nhỏ Sprint15–16; Sprint13 đã được giao với nghiệm thu riêng bởi chủ dự án.

## Sprint13 — thử nghiệm, chưa nghiệm thu

[Sprint13](sprints/SPRINT-13.md) đang thử hình ảnh/UX/campaign preview. [Các mục cần chốt](sprints/sprint-13/APPROVALS.md) do user quyết định; AI review không là final acceptance. Không chuyển Sprint14 hoặc merge dev trước khi user chốt. Mỗi phương án mới là spin mới.

Sprint13 đang ở Experiment03/SPIN019: ba hướng hình ảnh A/B/C sau phản hồi màu bản02, xem [report03](sprints/sprint-13/EXPERIMENT-03-REPORT.md). Approval register vẫn là nguồn chốt;01/02giữ nguyên. Hyper3D setup đã verifiedOAuth riêng nhưng tool chưa lộ trong thread hiện tại; không claim đã dùng để tạo hình03.

Sprint13 chuyển sang Experiment04/SPIN020 theo yêu cầu thử tiếp view từ3D. [Báo cáo04](sprints/sprint-13/EXPERIMENT-04-REPORT.md) ghi scene Blender/viewer và evidence thực; các hướng trước chưa được duyệt. Approval register giữ quyền chốt của user.

Chỉ dẫn user28/10: từ lần tạo model3D sauExp04 dùngMCPHyper3D; Blender dùng bố trí/camera/render. Khả năng kết nối và nguồn asset phải ghi thật, không tự thay công cụ nếu MCP chưa nạp.

Exp05/SPIN021 đã dùngMCPHyper3D thật qua [client và runbook](../runbooks/HYPER3D-MCP.md). Xem [report05](sprints/sprint-13/EXPERIMENT-05-REPORT.md): modelAcompleted nhưng lệchbrief; correction submissionambiguous khôngautoresubmit. Mọi sản phẩmSprint13 vẫnchưauserchốt.


## Sprint 13 — ảnh mô phỏng trước, chưa dựng model tiếp

Chủ dự án 29/10 xác nhận chỉ cần ảnh để xem game có thể trông như thế nào; ảnh 2D hoặc diện mạo 3D đều được. **Không tiếp tục tạo model 3D thật ở giai đoạn chọn ý tưởng.** Chỉ dẫn Hyper3D 28/10 giữ cho task model thật nếu được giao sau này, không áp vào ảnh giả lập 3D. Experiment 05 và lỗi lượt sửa chưa xác định ID được giữ nguyên làm lịch sử, không tự gửi lại.

Experiment 06/SPIN-022 chuyển sang ba hướng ảnh: tranh 2D góc chéo, cảnh giả lập 3D gần tầm người và 2.5D nhìn ngang. [Báo cáo 06](sprints/sprint-13/EXPERIMENT-06-REPORT.md) ghi ba ảnh thực tế, hai review độc lập và browser evidence; đây không phải game chạy hoặc ba camera của cùng scene 3D. Approval register vẫn quyết định các mục cần chủ dự án chốt; không merge dev, đóng Sprint 13 hoặc chuyển Sprint 14.


## Sprint 13 — vòng07 thử cả ngôn ngữ đồ họa

Yêu cầu tiếp theo ngày29/10 nhấn mạnh phải thử cả đồ họa. Experiment07/SPIN-023 mở ba concept ảnh với phong cách và bối cảnh khác nhau: khắc gỗ/làng mùa nước, pixel art/xưởng đúc đồng, giấy cắt/chợ hoặc đêm hội ven sông. [Báo cáo07](sprints/sprint-13/EXPERIMENT-07-REPORT.md) ghi phạm vi và evidence khi đã thực hiện. Đây là concept hư cấu chưa chốt canon; thay cả cảnh lẫn phong cách nên không quy sở thích chỉ cho đồ họa. Giữ01–06, tiếp tục ảnh-only và Pending user approval; không merge dev hoặc chuyển Sprint14.

## Sprint13 — thư viện tham khảo và vòng08

Chủ dự án làm rõ ngày29/10: mở spin/thử nghiệm mới không mặc định bỏ hoặc chê bản cũ. Giữ các phiên bản như một thư viện tham khảo, nhận xét theo đúng artifact/phạm vi và chỉ ghi quyết định chọn/loại khi user nói rõ. Lời khen ý tưởng tốt dần không tự chốt một hướng hoặc kết thúc Sprint13. Phản hồi lịch sử thực tế, gồm bản01 chưa ưng ý, vẫn giữ nguyên.

Experiment08/SPIN-024 bổ sung ba hướng trên cùng cảnh nhà chữa trị bên suối đồi: hoạt hình2D với nét/mảng bóng gọn, thế giới dệt bằng sợi/vải, và ảnh có diện mạo3D hình khối rõ. [Báo cáo08](sprints/sprint-13/EXPERIMENT-08-REPORT.md) ghi phạm vi và evidence thực tế khi có; ảnh-only, không tạo model, không chốt canon hoặc hoạt động chữa trị. Thư viện01–08 cùng được giữ, mọi mục Sprint13 vẫn chờ chủ dự án chốt; không merge dev hoặc chuyển Sprint14.
