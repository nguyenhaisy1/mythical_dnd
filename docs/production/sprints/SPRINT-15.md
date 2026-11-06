# Sprint15 — Bản thử Unity 2D sửa thuyền

03–06/11/2026 · SPIN-039 · Nghiệm thu kỹ thuật local đạt; đóng sprint sau exactHEADCI/PR/dev theo WORKLOG.

## Tổng hợp dễ hiểu

Đưa phần sửa thuyền Sprint14 vào một cảnh game thật: thấy nhân vật/thuyền, xem vật liệu và thời gian, xác nhận sửa hoặc hủy, chờ kết quả, lưu rồi mở lại. Cảnh dùng nền2D có cảm giác khối và nhân vật sprite theo Đất dựng thành. Bản thử ghi rõ nhân vật cố định; không giả là game đầy đủ hoặc xuất thân ngẫu nhiên đã làm xong.

## Scope và phân công

[ADR024](../../adr/024-bounded-2d-repair-client.md), [client contract](../../contracts/REPAIR-CLIENT-PROFILE.md) và BOAT-REPAIR-PROFILE là đầu vào. Core Sprint14/G13v1/old goldens giữ nguyên; full milestone VS03/VS07 chưa được đóng chỉ bằng sprint này.

| Task | Actor | Input → output | Acceptance |
|---|---|---|---|
| S15-01 | /root/s15_projection_audit | Readonly audit rồi Application projection/session và tests | Explicit field grants/denydefault, pure preview, safe command/persistence port, hidden negative cases |
| S15-02 | /root/s15_unity_audit | Readonly audit rồi Unity presentation/editor/build source | Scene/UI từ DTO, modal/focus/text scale, sprite sorting; root chạy Editor/Player thật |
| S15-03 | /root/s15_art | G13 style→foundation/foreground/boat/actor assets và provenance | Asset2D độc lập, alpha/anchor/QC, không3D hoặc baked concept thành runtime |
| S15-04 | /root | Infra composition/SQLite, runner/fixtures/fonts/actual build/run/captures | Real save/reload/repair Player, evidence phân loại đúng và regression |
| S15-QA | Critics giao sau worker freeze | Source/artifacts→findings/review | Independent review, fix/recheck không P0/P1; ghi đúng actor khi chạy thật |

Tối đa4actor cùng chạy kể cả root, critics theo lượt. Không gọi số vai là sốactor chạy đồng thời. Actors tiếp quản/fix/review thực tế và phạm vi từng người được ghi đầy đủ trong [evidence](../../qa/SPRINT-15-EVIDENCE.md), không đồng nhất assignment ban đầu với người hoàn thành sau gián đoạn. Root chịu tích hợp/docs/exports/commitpushPRdev theo workflow được giao; không main/tag/release/Sprint16.

## Điều kiện đóng Sprint15

Đủ cả năm gate trong client contract: safe projection/session tests; actual Editor/bootstrap/build; actual LinuxMono Player effects/save/reload; rendered/interaction evidence; independent review và full regression/docs/CI. Chỉ đóng khi ghi rõ từng gate đạt và evidence tương ứng.

Kết quả local06/11:593/593runtime tests/0failed-skipped, actualUnity6000.6.7f1/LinuxMono build/run,12captures/sixlayoutcases, finalSQLite và OSkeyboard/mouse đạt. Hai review độc lập không còn P0/P1 trong scope. [Evidence](../../qa/SPRINT-15-EVIDENCE.md) ghi failures/fixes, hashes và actors; exactHEADCI/PR/dev/CLOSE sau commit được append ở [WORKLOG](../../../WORKLOG.md), không nhận CI đã đạt chỉ vì local đạt.

## Backlog dự án ngoài sprint

Full random life, NPC/world knowledge và settlement binding, nghề vận tải/chữa trị, fullVS03/VS07, final user art/UI approval, production performance/balance/human playtest không thuộc điều kiện đóng sprint. Báo cáo cuối tách danh sách này khỏi “Điều kiện đóng Sprint15 còn thiếu”, tránh hiểu các sprint đã đóng còn treo nghiệm thu.
