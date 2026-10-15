# Bổ sung tài liệu sau audit — 14/10/2026

> Báo cáo lịch sử ngày 2026-10-14. Trạng thái mới ngày 2026-10-15: xem [Sprint 1](sprints/SPRINT-01.md) và [FND-02 evidence](../qa/FND-02-EVIDENCE.md); không dùng các mục pending bên dưới thay trạng thái hiện tại.


Đáp ứng yêu cầu duy trì tài liệu trực tiếp trong project: đã bỏ ZIP, manifest/report của ZIP và công cụ tạo ZIP. HTML/DOCX vẫn là bản đọc có thể tái tạo; chúng được kiểm tra với source fingerprints để phát hiện nội dung cũ. Archive hội thoại và toàn văn nghiên cứu được bảo toàn.

## Tài liệu đã bổ sung

| Gap | Tài liệu/data mới | Trạng thái sau bổ sung |
|---|---|---|
| READY-01 | [Identity Spec](../contracts/IDENTITY-SPEC.md) | Có wire format, ranges, allocation/rollback/lifecycle; C# tests còn FND-02 |
| READY-02 | [Time/Scheduler Spec](../contracts/TIME-SCHEDULER-SPEC.md) | Có tick/calendar/order/budget/skip/death policy; implementation FND-03/10 |
| READY-03 | [Math/RNG/Codec Spec](../contracts/DETERMINISTIC-MATH-RNG-SPEC.md) | Có algorithm/version/rounding/encoding/vectors; cross-runtime chưa chứng minh |
| READY-04 | [Command/Activity Catalog](../contracts/COMMAND-ACTIVITY-CATALOG.md) | Có payload, constraints, ownership, reserve/consume/cancel, receipts/errors cho kernel/repair |
| READY-05 | [Kernel smoke](../../content/fixtures/kernel-smoke/README.md) | Có projection snapshots, command sequence, expected arithmetic/hashes, negative specifications; full runtime snapshot/replay còn FND-05/13 |
| READY-06 | [Test Execution Contract](../qa/TEST-EXECUTION-CONTRACT.md) | Có discovery/evidence/golden/CI gates; runtime framework/project pin cần thực hiện FND-02 |
| READY-07 | [Slice Content Contract](../content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md) và [parameters](../../rules/parameters/kernel-v1.json) | Có authoring fields/validation/gates và kernel coefficients; full roster/compiled pack và transport/medicine values còn VS-02/03 |
| READY-08 | [Save Spec](../contracts/PERSISTENCE-SAVE-SPEC.md) | Có table contract, atomic/crash/compatibility/backup policy; provider/DDL/spike/migration implementation còn FND-11/12 |
| READY-09 | [Client UX Baseline](CLIENT-UX-BASELINE.md) | Có desktop/input/screen states/knowledge projection/reroll/accessibility; exact Editor/backend và playable proof còn FND-16/VS-07 |
| READY-10 | [Implementation Plan](IMPLEMENTATION-PLAN.md), [Backlog](BACKLOG.csv) | Đã sửa dependency FND-07→FND-03 và tách headless gate khỏi Foundation complete |
| READY-11 | [Documentation Runbook](../runbooks/DOCUMENTATION.md) và export tooling | Bỏ ZIP; manifests kiểm input freshness và output integrity |

Các lựa chọn prototype mới có provenance tại [ADR-008](../adr/008-prototype-contract-baseline.md), DEC-024–027 và Applicable Baseline. Chúng không phải nguyên văn quyết định đã có trong hội thoại, cũng không phải kết quả balance hay benchmark.

## Đồng bộ tác động

Đã cập nhật `.agent`, README/mục lục, Foundation/Ownership/Data Model, governance/open decisions/readiness register, backlog/gates, QA matrix/scenarios/execution, content/Unity/documentation runbooks, CI reference checks và integration notes ở các module bị ảnh hưởng. Module/Master được sinh lại từ inputs thay vì sửa tay. Development Handbook bao gồm specs mới và register readiness.

Audit cũ giữ nguyên các quan sát ở thời điểm trước bổ sung, có banner dẫn tới trang này. Open Decisions còn giữ những câu hỏi thực sự cần code/spike/playtest, không đóng bằng một tuyên bố “đủ tài liệu”. Whitepaper nói về sản phẩm, không quảng bá runtime implementation chưa tồn tại; các thay đổi nội bộ này không đổi bối cảnh/cốt truyện đã giới thiệu.

## Có thể bắt đầu và còn phải làm

Có thể bắt đầu FND-02/03/04 từ contract cụ thể và reference vectors, tiếp tục theo DAG. Trong task đầu phải pin test framework/runner và thêm executable tests. Provider SQLite và exact Unity target cần lựa chọn qua spike; không có kết quả compatibility giả trong hồ sơ.

Chưa có full slice runtime data, engine transaction/replay, migration hoặc gameplay đã pass. Những phần này là công việc implementation/content production với input, output và gate đã ghi rõ; không cần kéo dài nghiên cứu bằng Module30. Không tuyên bố toàn bộ 11 readiness gaps đã đóng kỹ thuật: READY-10/11 là thay đổi quy trình đã hoàn tất; phần còn lại có spec/data tham chiếu và exit evidence tương ứng đang chờ thực hiện.

Kết quả kiểm tra thực tế được ghi tại [Validation Results](../qa/VALIDATION-RESULTS.md). Reference checks chỉ kiểm tài liệu/vectors, không nghiệm thu engine.
