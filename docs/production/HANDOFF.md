# Bàn giao phát triển — 1.1-prototype-contracts

## Sprint16 — nghiệm thu kỹ thuật local09/11/2026: nhận một xuất thân có sẵn theo seed

Ngày06/11/2026 chủ dự án giao [Sprint16](sprints/SPRINT-16.md), tiếp tục09/11. [ADR025](../adr/025-seeded-origin-cohort-and-reveal.md), [origin contract](../contracts/ORIGIN-COHORT-PROFILE.md) và [runbook](../runbooks/ORIGIN-CLIENT.md) khóa VS02C `origin-cohort-v1` và một phần onboarding VS07. Sprint15 đã đóng/PR16/dev6fa3e1d; journal sau commit được giữ cho commit có ý nghĩa tiếp theo. Không giao lại VS04: Knowledge/narrative vẫn là milestone riêng.

New chọn một trong tám authored adult còn sống tại Bến Vạc, có household/home và refs history/skill/claim hợp lệ; seed chỉ đến từ trusted composition. Reveal không chọn/chỉnh/reroll; Begin chỉ đổi màn hình. Continue đọc campaign đã publish mới nhất, hiện lại đúng người/quá khứ ở tick đã lưu. UI nhận DTO bản thân; claim là nhận định có nguồn, không là sự thật toàn tri. World chỉ self record/chờ1 hoặc60phút/save/reload; không gán boat-repair cho cohort hoặc bind toàn24NPC/economy.

Origin metadata immutable nằm trong canonical image3 và snapshot/recovery, với exact rule/cohort/source profile. PhysicalSQLite schema2 giữ; image1/image2/goldens không đổi. New dùng pending DB riêng, verify/flush rồi publish create-new; cancel/fault/crash trước publish không đổi campaign Continue, retry requestId không tạo thêm hoặc bốc lại. Replay artifactv1 phải từ chối origin cho đến khi có profile replay riêng; deterministic genesis/save-reopen không là origin command replay.

**Điều kiện đóng Sprint16 còn thiếu:** Git/CI exactHEAD/PR→dev sau commit; docs/exports local đã kiểm, trạng thái đóng hiện hành ở GIT/CLOSE cuối WORKLOG. Runtime674/674/0failedskip, negative/process/SIGKILL gates, actual Unity hai seed/38captures/sixlayouts, OS20actions/hai processes và repair15 actual Player regression đã đạt09/11; reviews độc lập cuối ở [evidence Sprint16](../qa/SPRINT-16-EVIDENCE.md). Chỉ CLOSE sau đủ mọi gate, ghi Git sau commit ở WORKLOG; không chuyển gate còn thiếu sang backlog.

**Backlog dự án ngoài sprint:** full life-history generator/Character Viability/TEST001, fullVS02/VS07/world/NPC/economy/Knowledge binding, repair qualification/progression, final owner UI/art approval, performance/cross-platform/human playtest. Marker vị trí trung tính không là tạo hình hoàn chỉnh tám nhân vật. G13v1 Accepted/Reopenable và archive01–18 giữ. Không tự Sprint17/main/tag/release. Các mục Sprint15 và trước dưới đây mô tả lịch sử theo mốc của chúng.

## Sprint15 đã đóng — bản thử Unity2D đã có kiểm chứng tương tác

Chủ dự án giao Sprint15 theo workflow thường trực, tiếp tục04–06/11/2026. [Brief](sprints/SPRINT-15.md), [ADR024](../adr/024-bounded-2d-repair-client.md) và [client contract](../contracts/REPAIR-CLIENT-PROFILE.md) xác định `repair-client-2d-v1`: một nhân vật cố định tại bãi sửa thuyền Bến Vạc, direct-observation grants theo field, safe DTO/session và luật repair/SQLite Sprint14. UI phải ghi rõ nhân vật cố định, không nhận raw WorldSnapshot/receipts/seed hoặc arbitrary filesystem path. Preview pure, stale yêu cầu xem lại, retry giữ original envelope.

Actual Editor6000.6.7f1 đã tạo/import scene và build LinuxMono Player0lỗi/0cảnh báo. Lượt `sprint15-client-bo3487zv` có12ảnh renderer, sáu cases1280×720/1920×1080 ×100/125/150%, repair59→60/complete/cancel/wait/save/reopen và ba SQLite files được đọc final state/receipt counts riêng. Lượt OS-input `sprint15-input-yc1_bzfx` đạt native Tab/Back/Return, Space một lần chờ theo focus, S/L/C/Esc và mouse preview/Confirm bằng hai gesture riêng. Đây automation/technical proof, không human playtest. Focused31tests session/boundary/persistence đã đạt; không thay full regression cuối bằng focused tests.

[Runbook](../runbooks/REPAIR-CLIENT.md) hướng dẫn staging/build/run. Năm gate đóng sprint và bằng chứng/review/regression cuối ở [evidence Sprint15](../qa/SPRINT-15-EVIDENCE.md); commit/push/PR/CI/integration sau commit được append vào WORKLOG. Chỉ đóng khi đủ cả năm gate trên revision cuối, không chuyển gate chưa đạt thành backlog hoặc suy CI đã đạt từ kiểm tra local.

G13/profileearth-monument-2d-v1 và thư viện01–18 giữ nguyên. Full random life/knowledge/world binding/three professions/fullVS03/VS07, final user UI/art approval, performance/balance/human playtest là backlog dự án ngoài sprint. Báo cáo phải tách chúng khỏi **Điều kiện đóng Sprint15 còn thiếu**. Không tự Sprint16/main/tag/release.

## Sprint14 đã nghiệm thu — luật sửa thuyền headless

Chủ dự án đã giao Sprint14 ngày03/11/2026; [sprintbrief](sprints/SPRINT-14.md), [ADR023](../adr/023-bounded-boat-repair-profile.md) và [repairprofile](../contracts/BOAT-REPAIR-PROFILE.md) chốt VS-03A bounded. Thực thi reserve2timber/60tick/+200conditioncap1000, cancel/revalidation và atomiccommand/scheduler; explicitrepairimage2/rule/contentprofile, SQLitephysicalschema2/reopen/recovery/replay. [Evidence](../qa/SPRINT-14-EVIDENCE.md) ghi nghiệm thu VS-03A headless:562/562 runtime tests (113repair),0failed/skipped, negative controls và SQLite/replay process gates đạt. Git/CI integration theo WORKLOG, không mở rộng thành playable acceptance.

Sprint13 đã kết thúc vòng chọn hướng/G13Reopenable; A02/A03UI/originUX chuyển follow-up, không cản sprint được giao. Core thay đổi ở Sprint14 là phát triển gameplay mới được giao, không lấy core cũ từ checkpointG13. Hướng2D/sprite giữ nguyên, chưaUnity/spriteproduction/Player/fullprofession/randomorigin/actorprojection hoặc fullVS03. Không tự mởSprint15 hoặc release. Các đoạn ngày trước bên dưới là bàn giao lịch sử theo mốc của chúng.

## Mốc Sprint13 — đã chốt định hướng, G13 có thể mở lại

Ngày03/11/2026 chủ dự án chốt **Đất dựng thành + nền2D có cảm giác khối + nhân vật sprite**, profile `earth-monument-2d-v1`. Sprint13 **Accepted / Reopenable** ở phạm vi định hướng; [baseline/cổng G13](sprints/sprint-13/ART-DIRECTION-BASELINE.md) và [approval](sprints/sprint-13/APPROVALS.md) là nguồn hiện hành. S13-A02/A03 được chuyển thành follow-up implementation chưa nghiệm thu, không giả UI/originflow Passed. [ADR022](../adr/022-earth-monument-2d-and-reopen-gate.md) giữ core/luật/tri thức/dữ liệu độc lập với route đồ họa.

CổngG13 cho phép nghiên cứu lại mỹ thuật trên code tích hợp mới nhất; giữ tiến độ Sprint1–12 và về sau, không quay cả repository về một commit cũ. [Checkpointv1](sprints/sprint-13/checkpoint-v1.json) ghi sourceSHA/input trees và fingerprint thư viện để đối chiếu/khôi phục có chọn lọc. Thư viện01–18 giữ nguyên, các báo cáo Pending ngày trước là lịch sử. Chưa tạo layeredassets/sprite/scene/rig hoặc nghiệm thu Unity gameplay trong việc chốt này; ROM3D ngày02/11 không áp sang2D. Chưa mở Sprint14 hoặc release.

## Những gì đã có

29 module GDD chi tiết và phụ lục Life Event, source archive, source/section index, baseline/decision register, open decisions, ownership/contracts, QA matrix, scenario specifications và implementation backlog. Repository có .NET solution với portable core primitives, Domain world/rules, Application in-memory pipeline và headless transfer demo. Client Unity có bounded repair proof Sprint15 đã chạy actual LinuxMono Player/OS input; project do Editor6000.6.7f1 tạo, full sprint closure và giới hạn acceptance theo evidence riêng.

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

Sprint 1 có code/tests identity/time/codec/RNG; xem [sprint evidence](../qa/SPRINT-01-EVIDENCE.md) và backlog. Sprint2 có WorldSnapshot/rules/command/invariant path in-memory, xem [evidence Sprint2](../qa/SPRINT-02-EVIDENCE.md). Sprint3 có event-v1/causal journal và idempotent diagnostic consumer, xem [evidence Sprint3](../qa/SPRINT-03-EVIDENCE.md). Sprint4 có scheduler/AdvanceTime in-memory, xem [evidence Sprint4](../qa/SPRINT-04-EVIDENCE.md). Sprint5 có headless SQLite current-state commit/reopen, xem [evidence Sprint5](../qa/SPRINT-05-EVIDENCE.md). Sprint6 bổ sung snapshot/recovery/migration schema1→2 sang file mới; xem [evidence Sprint6](../qa/SPRINT-06-EVIDENCE.md) trước khi nhận task. Sprint7 bổ sung headless replay QA từ initial state và host actions; xem [evidence Sprint7](../qa/SPRINT-07-EVIDENCE.md). Sprint8 bổ sung compiler/registry commodity/transfer-template và exact content/save compatibility; xem [evidence Sprint8](../qa/SPRINT-08-EVIDENCE.md). Sprint9 có actual checks/full field diff/causal-job inspector và fixed evidence reproduction; xem [evidence Sprint9](../qa/SPRINT-09-EVIDENCE.md). Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Nghiệm thu Sprint10 chỉ bao phủ kernel Editor, không Player/IL2CPP hoặc full gameplay. Bounded LinuxMono repair Player đã nghiệm thu riêng Sprint15; origin client16 theo gates/evidence riêng. Unity crash/recovery/migration rộng hơn, IL2CPP và full campaign vẫn chưa nghiệm thu. OPEN03 còn broader Player crash/recovery/migration và platform atomic replacement; bounded repair Player packaging đã kiểm Sprint15. Không có P0 ở mức “thiếu toàn bộ thiết kế” buộc phải viết thêm module; các lựa chọn kỹ thuật còn mở phải được xử lý trong task tương ứng trước khi merge feature phụ thuộc.

Foundation được nghiệmthu ở kernel subset/Editor profile đã kiểm; bounded repair client15 đã có Player/input evidence. Full game balance và full-production content chưa đủ. Xem `OPEN-DECISIONS.csv` và kết quả kiểm tra thực tế. Khả năng 1.000 nhân vật là mục tiêu sản phẩm, không là test đã pass.

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

[Bộ kiểm chứng và cách chạy](../runbooks/UNITY-CONFORMANCE.md) đối chiếu host với Editor thật. Sprint10 đã kiểm chứng Unity6000.6.7f1/Linux x64/Editor Mono: 5 semantic checks khớp .NET; VS01 và FND16 được nghiệmthu trong kernel profile theo ADR019. Nghiệm thu Sprint10 chỉ bao phủ kernel Editor, không Player/IL2CPP hoặc full gameplay. Bounded LinuxMono repair Player đã nghiệm thu riêng Sprint15; origin client16 theo gates/evidence riêng. Unity crash/recovery/migration rộng hơn, IL2CPP và full campaign vẫn chưa nghiệm thu. Tại Sprint10 client game còn layout; project conformance tạo riêng bằng Editor thật. Client15 và origin16 có gates/evidence riêng phía trên.

## Sprint11 — VS02A: dữ liệu dân cư và mạng đường

[Compiler và cách dùng](../runbooks/SETTLEMENT-CONTENT.md) kiểm24actors/6households/4communities/8routes/24claims/24directedrelationships, tạo immutable authoring registry. VS02 full vẫn partial: Sprint12 đã bổ sung8event templates/eligibility trên trusted observations, Sprint16 đã kiểm chứng chọn protagonist từ cohort; actor-specific obligations/full viability, routine execution/event effects/world binding vẫn còn. Không coi đủrecordcount là worldsimulation.

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

## Sprint13 — vòng09: bản sắc xuyên nhân vật và giao diện

Yêu cầu mới ngày29/10 nhấn mạnh đồ họa phải xuyên suốt game, từ tạo hình nhân vật tới giao diện. Experiment09/SPIN-025 thử ba bảng concept tổng thể: sơn mài đen/đỏ/vàng, phù điêu đồng có patina, và mực đen/trắng với nét đỏ. Mỗi bảng hướng tới cảnh chơi cùng nhân vật toàn thân/chân dung, vật phẩm/icon và UI hội thoại/túi đồ/nhật ký trong cùng một ngôn ngữ. [Báo cáo09](sprints/sprint-13/EXPERIMENT-09-REPORT.md) giữ output/evidence thật sau khi thực hiện.

Gallery chỉ phục vụ xem ảnh; UI được minh họa trong raster, không là UI runtime hoặc lựa chọn origin thủ công. Tiếp tục ảnh-only, không model3D/Hyper3D/Blender. Thư viện01–09 cùng được giữ để tham khảo/kết hợp, không mặc định loại bản cũ hoặc chốt một hướng. Mọi mục chưa được duyệt vẫn Pending; không merge dev, đóngSprint13 hoặc chuyểnSprint14.

## Sprint13 — vòng10: hai hướng theo trí tưởng tượng

Chủ dự án cho AI tự quyết số lượng concept, không bắt buộc ba phương án A/B/C. Experiment10/SPIN-026 chọn hai hướng toàn game: **Men chàm** với ngôn ngữ gốm men xanh/trắng, và **Bóng rối** với lớp bóng/cắt sáng xuyên thấu. Mỗi bảng hướng tới thế giới, tạo hình nhân vật/chân dung, vật phẩm/icon và UI hội thoại/túi đồ/nhật ký; tất cả là ảnh raster tham khảo, không model3D hoặc UI runtime. [Báo cáo10](sprints/sprint-13/EXPERIMENT-10-REPORT.md) ghi output và evidence khi thực hiện.

Giữ thư viện01–10, không suy vòng mới là loại bản cũ. Hồ sơ09 sau phiên bị ngắt đã được hai critic mới kiểm lại và viết report09 riêng, cùng lượt review10. Ảnh/browser evidence và phản biện có phạm vi riêng, không thay quyền chốt của user. Mọi mục chưa chốt vẫn Pending; không merge dev, đóngSprint13 hoặc chuyểnSprint14.

## Sprint13 — vòng11: không gian và ngôn ngữ tạo hình

Experiment11/SPIN-027 mở ngày29/10 và tiếp tục ngày30/10 sau giới hạn tác nhân, thử bốn bảng ảnh toàn game: **2.5D nhiều lớp**, **3D cách điệu**, **Pixel đời làng** và **Anime đời thường**. Cùng tình huống chợ lương thực/kho kê cao/cầu trước mùa nước, mỗi bảng cần có cảnh, nhân vật toàn thân/chân dung, vật phẩm và UI. Phụ nữ lao động trưởng thành khoảng45tuổi là mẫu cast minh họa, không cố định protagonist runtime hoặc cho chọn origin thủ công. [Báo cáo11](sprints/sprint-13/EXPERIMENT-11-REPORT.md) ghi bốn PNG đã tạo30/10, review độc lập, browser evidence và các giới hạn quan sát.

2.5D/3D là cách tổ chức không gian; pixel/anime là ngôn ngữ tạo hình có thể kết hợp. Các ảnh không là quyết định chuyển engine, model thật, parallax/animation chạy hoặc pixel grid production. Thư viện01–11 cùng được giữ; mọi mục chưa chốt vẫn Pending, không merge dev, đóngSprint13 hoặc chuyểnSprint14.

## Sprint13 — vòng12: sáu ngôn ngữ mỹ thuật mới

Ngày30/10 chủ dự án yêu cầu ít nhất năm mẫu tiếp theo và đẩy mạnh thử nghiệm đồ họa. Experiment12/SPIN-028 có phạm vi sáu bảng concept toàn game: **Đất biết kể** (`clay-craft`), **Nét trong** (`clear-line`), **Vách thời gian** (`mineral-fresco`), **Ký ức CD-ROM** (`retro-prerender`), **Đêm than bạc** (`charcoal-night`) và **Gió mảng màu** (`graphic-animation`). Mỗi hướng thử thống nhất cảnh chơi, tạo hình người trưởng thành, chân dung, vật phẩm/icon và UI hội thoại/túi đồ/nhật ký. [Báo cáo12](sprints/sprint-13/EXPERIMENT-12-REPORT.md) phân biệt mục tiêu với output và evidence thực tế.

Các hướng khác nhau về chất liệu, nét, hình khối, ánh sáng và cách đóng khung UI, không chỉ đổi bảng màu. Đây là ảnh mô phỏng bằng built-in image_gen; diện mạo đất nặn hoặc cảnh dựng sẵn không có nghĩa đã tạo model, dựng scene3D hoặc triển khai animation. Cast và lời thoại là ví dụ minh họa, không cố định protagonist, đổi random-origin hoặc thêm canon. Thư viện01–12 cùng được giữ để tham khảo/kết hợp. Mọi mục chưa được duyệt vẫn Pending; không merge dev, đóngSprint13 hoặc mởSprint14.

## Sprint13 — workshop đồ họa game và Văn Lang–Âu Lạc, SPIN-029

Ngày30/10 chủ dự án yêu cầu brainstorming với tác nhân vì các mẫu thiên minh họa/tả thực và nhãn3D chưa cho cảm giác game mong muốn. [Workshop định hướng](sprints/sprint-13/ART-DIRECTION-RETHINK.md) tổng hợp audit, nguồn văn hóa và sáu cách tổ chức hình ảnh; ưu tiên thử camera/hình khối/tỷ lệ/UI trước lớp phủ vật liệu. 3D và độ tả thực được tách riêng; chất văn hóa là nguồn cho grammar hình, không chỉ hoa văn/màu cũ. Đây là đề xuất Pending, không batch ảnh mới hoặc lựa chọn art/engine/canon. Các bản01–12 giữ nguyên. Chỉ dẫn tương ứng đã đưa vào AGENTS/.agent và reference art-direction; Sprint13 chưa kết thúc.

## Sprint13 — vòng13: thử khung chơi từ workshop, SPIN-030

Ngày30/10 chủ dự án giao tạo thêm ý tưởng sau workshop. Experiment13 thử ba cách nhìn bằng ảnh: **Khối Lạc** nhìn chéo với khối3D cách điệu mạnh, **Dải đời sống**2D/2.5D theo lớp sinh hoạt, và **Trong lòng làng** cự ly gần cùng họ khối3D dự kiến. Mỗi hướng có một frame thế giới nguyên khung và một frame đối thoại riêng, tổng sáu frame được giao; không dùng công thức board65/35. [Báo cáo13](sprints/sprint-13/EXPERIMENT-13-REPORT.md) phân biệt brief, artifact và evidence thực tế.

Cùng vai nhân vật/bến/thuyền/lối lên nhà sàn giúp đối chiếu cách thể hiện, nhưng ảnh sinh độc lập không bảo đảm camera của cùng scene hoặc cast/geometry tuyệt đối. Chỉ dùng built-in image_gen, không model/runtime hay chuyển engine. Nguồn văn hóa từ workshop được chuyển thành lựa chọn mỹ thuật, chưa xác minh phục dựng/canon. Thư viện01–13 và workshop được giữ; các sản phẩm vẫn Pending user approval, không merge dev, đóngSprint13 hoặc chuyểnSprint14.

## Sprint13 — vòng14: mở rộng khác biệt giữa sáu hướng, SPIN-031

Ngày30/10 chủ dự án nhận xét các ảnh13 tốt nhưng còn giống nhau, chưa có nhiều khác biệt/vượt bậc, và giao thử tiếp. Experiment14 mở sáu khung chơi thuộc sáu hướng độc lập: Đất dựng thành, Ô đất sống, Làng vòng cung, Đôi tay làm nghề, Ký hình và Mái nhà nhiều giọng. Thay đồng thời camera, hình/tỷ lệ, cast, bối cảnh, màu và UI để mở rộng thư viện; không tiếp tục ba cặp quanh cùng một cảnh hoặc dùng ảnh13 làm tham chiếu. [Báo cáo14](sprints/sprint-13/EXPERIMENT-14-REPORT.md) phân biệt brief với output/review thực tế.

Chỉ ảnh built-in image_gen, không model/scene3D/runtime/engine hoặc canon mới. Camera cong không là thế giới hình cầu, ký hình không là chữ viết cổ được xác nhận, trang truyện là đề xuất cách trình bày game chứ không bảng asset. Vì nhiều biến đổi cùng lúc, không quy sở thích cho một trục đơn. Giữ01–13 và workshop; lời khen không là approval, mọi hướng chưa chốt tiếp tục Pending và không chuyểnSprint14.

## Sprint13 — vòng15: đào sâu shortlist và mở rộng cách nhìn, SPIN-032

Ngày30/10 chủ dự án đánh giá rất tốt14/1 **Đất dựng thành**,14/2 **Ô đất sống** và14/3 **Làng vòng cung**, cho phép đào sâu, đồng thời yêu cầu thêm ý tưởng táo bạo hơn. Ba hướng này được ghi thành ưu tiên nghiên cứu, chưa là final art approval, chưa chọn một hướng duy nhất và không loại14/4–6 hoặc các bản cũ.

Experiment15 đã có sáu frame mới: ba nhánh **Thành dựng đứng**, **Ghép đảo thủy triều**, **Làng trên vành trống** và ba cách nhìn mới **Địa hình đa diện**, **Làng của bóng**, **Thủy kính**; giữ thêm biến thể đa diện v1. Bảy PNG tạo30/10, hồ sơ và hai review hoàn tất02/11. [Báo cáo15](sprints/sprint-13/EXPERIMENT-15-REPORT.md) phân biệt brief với output và evidence thực. Chỉ built-in image_gen, cả bảy lượt không có ảnh tham chiếu; không model/runtime/engine. Phép đứng, mảng ghép, cuộn, đa diện, bóng và phản chiếu là đề xuất presentation, không thêm world geometry, luật hex, phép thuật, portal hoặc hidden knowledge. Giữ01–14/workshop và mọi mục Pending; không đóngSprint13 hoặc chuyểnSprint14.


## Sprint13 — vòng16: thêm sáu quy tắc tạo hình, SPIN-033

Ngày02/11 user giao thêm ý tưởng mới sau vòng15. Experiment16 đã có sáu frame chính: **Thế giới dệt**, **Nét dây không gian**, **Làng gấp**, **Mưa khắc hình**, **Mặt cắt đời sống** và **Hạt khối**; giữ thêm hai bản đầu nét dây/làng gấp qua link riêng. Tám lượt built-in image_gen không ảnh input, copy nguyên bytes. Các hướng thử thay cách tổ chức không gian, silhouette nhân vật và UI, không chỉ palette; mức đạt chưa đồng đều. [Báo cáo16](sprints/sprint-13/EXPERIMENT-16-REPORT.md) phân biệt brief, output và evidence thực. Dệt/giấy/pixel có tiền lệ ở các vòng cũ, nên mức đổi mới phải căn cứ quy tắc tạo hình thực sự nhìn thấy, không chỉ tên vật liệu.

Chỉ ảnh built-in image_gen được giao, không model/scene/runtime/Hyper3D/Blender hoặc engine/canon mới. Hình dây, mặt cắt, không gian gấp hoặc voxel không tự tạo khả năng nhìn xuyên, topology, thời tiết, phá hủy hay tri thức bí mật. Giữ01–15, các biến thể và ba ưu tiên14/1–3; giao thêm ý tưởng không là approval hoặc loại bản cũ. Mọi mục chưa chốt tiếp tục Pending; không đóngSprint13 hoặc chuyểnSprint14.


## Sprint13 — vòng17: brainstorming và thêm mười phương án, SPIN-034

Đã có mười frame chính và một bản Vành sân đầu tiên được giữ:11PNG1672×941RGB, copy nguyên bytes, không ảnh input. Bản thử lại Vành sân giản lược khối sau lượt đầu gần tả thực; báo cáo ghi rõ phần đạt/chưa đạt.

Ngày02/11 user giao thêm10ý tưởng, cho phép học hỏi/improve từ các bản cũ và brainstorming giữa tác nhân. Ba worker đã audit/trao đổi chéo; root gộp hai đề xuất relief trùng thành một và chọn batch thử ảnh: Hành lang thành đất, Bờ liền nét, Vành sân, Phù điêu đang sống, Người bằng cử chỉ, Cửa trong cửa, Khối ghép đời thường, Sát mặt nước, Pixel cự ly gần và Âm bản sống. Ba hướng đầu phát triển ưu tiên14/1–3; bảy hướng sau khác hoặc cải tiến từ thư viện, không tuyên bố bảy kỹ thuật chưa từng có. [Báo cáo17](sprints/sprint-13/EXPERIMENT-17-REPORT.md) và ba note brainstorm ghi lineage/delta, quyết định phạm vi và output/review theo evidence thực.

Mười một ảnh vòng17 đã tạo bằng built-in image_gen không ảnh input; metadata/hash thực ở báo cáo17 và provenance. Chỉ ảnh xem trước. Không model/scene/runtime/Hyper3D/Blender hoặc engine/canon mới. Camera thấp, âm bản, relief, pixel hoặc nêm ghép không tự thay geography/physics/route/knowledge. Giữ01–16, biến thể, phương án brainstorm dự phòng và ưu tiên14/1–3; không coi chọn batch của AI là approval. Mọi mục chưa chốt tiếp tục Pending; không đóngSprint13 hoặc chuyểnSprint14.


## Sprint13 — vòng18: thêm mười lăm cách thể hiện, SPIN-035

Ngày02/11 user giao thêm khoảng15ý tưởng sau vòng17. Ba worker brainstorming camera/không gian, cast–UI và audit/range; root gộp ba cặp cel/soft/ink trùng, chọn6space +6cast +3audit để thử. Bộ15 gồm Sân chéo mở, Ba dải đường, Qua vai người thợ, Đường chéo dài, Rừng là thân cột, Sân phẳng, người nổi, Nét mặt ba sắc, Khối liền mềm, Mực đặc, Người bằng dải, Đường đồng mức sống, Pixel hình lớn, Một-bit có tuổi, Sân khấu một nền và Khối răng cưa. [Báo cáo18](sprints/sprint-13/EXPERIMENT-18-REPORT.md) cùng ba note brainstorm giữ intent/delta, trao đổi thật và output/evidence khi có.

Đã có15frame chính và2bản đầu dải thân/đồng mức được giữ:17lượt built-in image_gen mới, không ảnh input, copy nguyên bytes. Bản trưng bày hai hướng này dùng iteration;17PNG RGB,30,511,159bytes, hai ảnh cao940 và15ảnh cao941 như provenance. Review/checks chỉ ghi hoàn tất theo evidence thực. Phạm vi thử thêm sân/rừng/hiên nghề, không buộc toàn bộ cùng bến sông hoặc lặp ba nhánh favored. Giữ14/1–3 là ưu tiên nghiên cứu thật, không final approval; mọi01–17 và biến thể tiếp tục được giữ.

Chỉ ảnh preview, không model/scene3D/runtime/Hyper3D/Blender, engine/canon/route/physics/knowledge mới. Hybrid/contour/khối/dither không cấp quyền nhìn xuyên hoặc biết bí mật. Mọi output18 và mục chưa chốt tiếp tục Pending; không merge dev, đóngSprint13 hoặc chuyểnSprint14.


## Sprint13 — nghiên cứu khả thi ba hướng, SPIN-036

Đã brainstorm Đất dựng thành14, Hành lang thành đất17 và Đường chéo dài18; [báo cáo so sánh](sprints/sprint-13/ART-FEASIBILITY-THREE-DIRECTIONS.md) gồm kiến trúc/art pipeline/WBS/lịch giả định và review thật. Đề xuất 3D cách điệu + UI2D + camera giới hạn, Đường chéo làm proof trước, Hành lang kiểm người cận, Đất kiểm quy mô; chưa là lựa chọn được duyệt. Ba worker trao đổi chéo; hai critic độc lập kiểm tech/art/effort ở các lượt sau; critic estimate bị usage limit lượt đầu rồi tiếp tục được, root tự rà riêng. WBS cuối đã bổ sung art expansion và kit bến/thuyền còn thiếu trong bản đầu.

M1 chỉ cảnh art nhỏ hai actor (Earth fragment, không panorama/crowd), M2 chuyển grammar về Bến Vạc và tính cả bến/thuyền, cần origin/repair/projection/save thật. Mốc40–90ngày M2 chỉ quy đổi ởQ8 giả định, chưa phụ thuộc/chờ; Plus không đảm bảoQ8, chưa benchmarkSolhigh. Client còn layout, URP/UI là đề xuất, UHD630 chưa đo, Hyper3D chỉ metadata chưapreflight. M0cap12–24giờ công là bước hiệu chỉnh được đề xuất, chưa chạy. Không ảnh/model/Unity implementation/benchmark mới; không Experiment19 hoặc đổi engine/canon. Giữ01–18 và Pending S13-A01–A04, chưa quaSprint14/merge dev/main/release.
