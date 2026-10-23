# Sprint10 — Nền tảng đã chạy được trong Unity Editor

## Tổng hợp dễ hiểu

Đã đưa cùng bộ nền tảng sang Unity6000.6.7f1 và kiểm tra kết quả với chương trình .NET. Dữ liệu tiếng Việt, kết quả ngẫu nhiên, phát lại chuỗi hành động, nội dung và lưu/mở lại dữ liệu đều khớp. Đã rà nền tảng của chín sprint trước và đồng bộ trạng thái backlog.

VS01 kernel và FND16 đạt trong profile Linux x64/Unity Editor Mono. Đây là nền để tiếp tục xây nội dung và giao diện; **chưa có bản game chơi thử**. Player/IL2CPP, packaging, Unity crash/recovery/migration và full gameplay còn gates riêng.

## Phân công và review

SPIN-013, branch `feature/sprint-10-unity-conformance`. /root implement probe/host/tooling/tests/docs và tích hợp. s10_audit/s10_unity/s10_review đóng góp thiết kế ban đầu trước quota; không tính là source review. s10_final_review phát hiện ba lỗi preparation (parity giả giống nhau, output parent, snapshot chưa kiểm count); root sửa và reviewer kiểm lại đạt.

Sau khi user cài Editor, /root chạy actual import, sửa staging sang MythicalRuntime và tách import trước entry script; chuẩn hóa API enum label sau equality guard, giữ raw name/value. /root/s10_actual_review review độc lập lần cuối: tự recompute parity, source fingerprint,13import hashes, Editor binary hash và đọc338-test evidence; không còn finding chặn tích hợp trong scoped acceptance.

## Kết quả thật

| Gate | Artifact | Kết quả |
|---|---|---|
| Headless Release regression | `tests/results/sprint10-nbc5jibw/evidence.json` | 338/338,0failed,0skipped,exit0 |
| Actual host–Unity | `tests/results/unity-hmn0_eqs/evidence.json` | 5semantic checks ở cả host và Editor,exit0 |
| Host/tooling negatives | Trong runtime evidence | 4host processes,10parity rejection cases,missingEditor exit2,13imports |

Headless source fingerprint `sha256:b0a9a67ed78cc24c21e1b5da08efe2de85763d64d5a07a63109dbdc267baaeac`. Actual conformance source fingerprint `2077904913f2dc32eaf1e5bf54ecf9b5257b011e1e9cb5f7cc0874afe11bbb01`. Run ghi parent revision `f5f1ebeda7a064a7b9c30e6fc42d917ca9a3dfd8` cùng hash từng source có thay đổi chưa commit; fingerprint là provenance của source thực thi, không giả rằng parent commit đã chứa bản sửa.

Actual Editor `6000.6.7f1`, ProjectVersion do Editor tạo; backend Editor-Mono, LinuxEditor, API enum value6/raw `NET_Standard_2_0` bằng `NET_Standard` theo guard. Host .NET10.0.12 và cả hai SQLite3.53.3. API label là chuẩn hóa tên alias, không đổi giá trị kiểm. Rule/codec/RNG/content/save versions không đổi; codec mdc1, sha256-counter-v1, world seed64số0, replay rule TransferResource+scheduler v1.

Năm kiểm chứng: known-answer codec tiếng Việt;8RNGwords; replay15actions và final image hash; compiled content hash; SQLite command commit/reopen tồn kho nguồn7/đích3/revision2, snapshot count tăng và compatible-state giữ nguyên, wrong-content refusal không đổi database. Fingerprint chung `d47e710ae21268d937742dc3fda208ae5e58b220309eeb2b276697a5367372f9`. Fixture SHA256 `bc82e0428cfd260d93b6a62a4eb09e472e73c2e15f8b7dab2b2f27d536423d2b`. Năm semantic checks không phải338 Unity tests.

Locked restore/build0warnings/errors. Negative control cố ý thất bại đúng một test; diagnostics/content/replay regressions,2demos và9SIGKILL checks đạt trên headless. CI chỉ chạy host/tooling; actual Unity chạy local, không gán cho GitHub CI.

## Lỗi tìm được và phạm vi chưa kiểm

Hai actual attempts x72o4op9/v1rcfl4g thất bại CS0246 khi stage trong Plugins/GameCore; giữ nguyên evidence thất bại. Rename experiment sang MythicalRuntime chạy probe thành công, sau đó full run mới hmn0_eqs đạt. Chưa khẳng định nguyên nhân nội bộ của Unity importer. Enum.ToString trả alias khác tên canonical; guard enum không đổi, output giữ cả raw value/name.

Preparation cũ ku8vaxft từng338/338 đạt; cn93gg2e bị freshness gate từ chối do source sửa lúc chạy. Chỉ dùng nbc5jibw/hmn0_eqs cho source cuối. License/raw Editor logs ở artifacts ignored, không commit hoặc đưa vào exports; báo cáo duy trì không chứa tài khoản/license.

[ADR019](../adr/019-unity-editor-conformance.md) khóa exact profile. [VS01 audit](VS-01-REVIEW.md) nghiệmthu kernel subset FND02–15. OPEN04 resolved cho Editor; OPEN03 còn Player packaging/native distribution, Unity crash/recovery/migration. Không có Player/IL2CPP, accessibility/performance hay human playtest evidence; client game vẫn layout.

## Bàn giao

Tích hợp PR10 vào dev theo standing workflow sau review/CI. Git SHA và trạng thái merge thực tế ghi append trong WORKLOG. Không đổi main, tag hoặc release; không tự mởSprint11. Bước tiếp theo khi được giao là VS02 living settlement content, không cần thêm module nghiên cứu để thay implementation.
