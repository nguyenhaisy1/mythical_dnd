# Phản biện độc lập — persistence, compatibility và replay sửa thuyền

03/11/2026 · Sprint14 / SPIN-038 · Actor `/root/s14_critic_storage`.

## Phạm vi thực sự đã kiểm

Đã đọc `.agent`, `AGENTS.md`, HANDOFF, Applicable Baseline, skills `mythical-validation`/`mythical-kernel`, test execution contract, [ADR023](../../adr/023-bounded-boat-repair-profile.md) và [Boat Repair Profile](../../contracts/BOAT-REPAIR-PROFILE.md).

Đã đọc source codec/store/save-recovery, repair compiler, ReplayHarness, các test persistence/replay/helper mới, Host `BoatRepairProbe`/`BoatRepairJson` và Python gate. Review này phân biệt code đã đọc, artifact test do worker khác chạy và phép kiểm do critic thực hiện. Không nhận một build hoặc TRX cũ là evidence áp cho source sau khi sửa.

Không chạy build hoặc full suite trong lúc Domain/Application/root còn sửa source. Không chạy Unity, Player, rig/sprite, benchmark, human playtest hoặc SIGKILL cho profile repair. Source/code-runtime freeze và lượt chạy cuối do root điều phối.

## Findings đã gửi cho root

| Mức | Finding và hệ quả | Xử lý hiện tại |
|---|---|---|
| P1 | Host gọi `RecoverToNewFile(...).Load()` dù API trả `CommittedWorld`. Lỗi tích hợp này khiến Host mới không biên dịch, dù worker đã build trước khi Host xuất hiện. | Root đã sửa source: dùng returned state và mở output store riêng với đúng profile/content để xác minh durability. Critic đã đọc bản sửa; cần build/probe ở source cuối, không dùng build cũ để gọi Passed. |
| P1 | Restore image2 ban đầu chỉ đối chiếu activity↔job; không buộc `StartCommandId` trỏ accepted StartRepair receipt và RepairStarted fact khớp dữ liệu pin. Một aggregate có activity/hold/job tự nhất quán nhưng thiếu nguyên nhân hợp lệ có thể vượt các vòng kiểm đó. Terminal activity cũng cần fact/receipt phù hợp. | Root đã thêm shared repair-history validation trong hai constructor, Publish và codec. Critic đã đọc helper và hook: start/terminal receipt/fact, pin, cause, tick, revision và retirement được đối chiếu. Trạng thái chưa đóng bằng runtime cho đến khi test source cuối chạy. |

Finding thứ hai là kiểm tính nhất quán causal/reference khi restore, không phải tuyên bố hash SQLite là chữ ký chống kẻ sửa file. Hash/inventory không thay kiểm domain provenance. Cũng không yêu cầu replay toàn bộ lịch sử để load: chỉ cần những cross-reference và payload đã được contract ràng buộc.

## Adversarial cases độc lập đã viết

[BoatRepairCausalityTests](../../../tests/MythicalDnd.Foundation.Tests/BoatRepairCausalityTests.cs) gồm tám case mới, dùng fixture start/complete/cancel thật rồi dựng malformed aggregate bằng reflection vào constructor nội bộ chỉ trong test:

1. Activity và job cùng trỏ một start cause không có accepted receipt.
2. Start event material quantity khác pinned activity.
3. Start event recipe hash sai.
4. Start event due tick sai.
5. Start event aggregate sai loại/đối tượng.
6. Start event rule version sai.
7. Completed activity không có RepairCompleted fact tương ứng.
8. Cancelled activity có terminal reason khác cancellation fact.

Mỗi case kiểm cả constructor processor và codec decode; receipt/event references chung vẫn được giữ để không chỉ vô tình rơi vào một lỗi fingerprint khác. Không thêm friend-assembly production chỉ để test; không sửa source hoặc test của worker khác. Root đã chạy integration suite; critic tự đọc TRX và đối chiếu đủ tám case này Passed. Lượt integration đầu có110 executed /109 Passed /1 Failed ở test due-revalidation dead actor của critic khác (`NO_ADVANCE_WORKFLOW`); không quy lỗi đó sang tám ca causality hoặc gọi toàn suite Passed. Kết quả cuối chờ root xử lý và rerun.

## Những ranh giới được source hiện tại giữ đúng

| Cổng | Đối chiếu source |
|---|---|
| Image1 và image2 | Encode giữ dictionary image1 cũ khi Repair null; chỉ thêm image2/repair khi extension explicit, kể cả arrays rỗng. Decode kiểm supported version và canonical re-encode, nên unknown fields không bị bỏ qua âm thầm. |
| Typed job | Repair job có discriminator và activity reference thật; không dùng dummy TransferResource. Legacy transfer shape giữ nguyên. |
| Recipe pin | Recipe immutable/fingerprint lưu trong state/activity. Compiler strict tách khỏi kernel-content-v1; metadata/handler/material/version có allowlist và tham gia content hash. |
| SQLite profile | Constructor default legacy, allowlist rule version; initialize/commit/load đối chiếu explicit repair profile và content hash. Các path snapshot/recovery/migration truyền rule profile vào destination/backup store. |
| Schema | Image version2 khác physical SQLite schema2; không thêm DDL chỉ vì state mở rộng. Test migration schema1→2 dùng image2 là profile kỹ thuật mới, không tự nâng world legacy thành repair world. |
| Mirrors | Item/activity/capability được thêm vào entities BLOB khi có repair extension. Load đối chiếu mirror và canonical image; legacy entity payload không thêm các key này. |
| Recovery | Đọc nguồn read-only, kiểm rule/content metadata trước xét recovery; output create-new và xác minh lại. Recovery đọc committed images, không được gọi là rerun gameplay. |
| Replay | Record/Replay khởi tạo processor mới và Execute/Drain action; manifest/recipe/profile được đối chiếu. Checkpoint gameplay/integrity chứa repair state, jobs hash chứa tagged job. Negative hash/control và thay initial material có thể phân biệt rerun với đọc final journal. |

Đường legacy vẫn giữ constants conformance cho artifact hash `bc82e042…`, state/replay `f89945aa…`, content `e45fa906…` trong source. Chỉ đọc constants hoặc đường branch không chứng minh compatibility đã Passed ở bản cuối; full regression phải thực thi và không đổi expected để che lỗi.

## Artifact test đã tự đọc, không phải critic chạy

Đã dùng Python stdlib đọc counters và từng UnitTestResult của các TRX tại `tests/results/sprint14-infra/`:

| Artifact | Discovered / executed / passed | Failed / skipped | Giới hạn |
|---|---:|---:|---|
| `sprint14-infra.trx` |28 /28 /28|0 /0|Có tám case replay mới trong tổng28; đây là lượt worker thực hiện trước sửa causal-history của root |
| `sprint14-storage-legacy.trx` |139 /139 /139|0 /0|Tổng139 gồm tám replay repair mới; không gọi139 là legacy-only |

Đã đối chiếu counters với số result rows (28 và139). Không dùng các TRX này thay full-suite gate, source fingerprint sau sửa hoặc Host mới.

Critic cũng đã tự parse `tests/results/sprint14-integration/boat-repair-integration.trx`:110 result rows,110 executed,109 Passed,1 Failed; cả tám case `RestoreRejects` độc lập Passed. Failure còn lại là due-revalidation `dead` với `NO_ADVANCE_WORKFLOW`, đã gửi root; integration chưa Passed ở lượt này. Đây vẫn là artifact do root chạy, không phải critic thực thi.

## Trạng thái bàn giao

Source review ban đầu tìm được hai P1 và đã báo root sớm. Hai finding đã có source fix được critic đọc. Helper repair-history đối chiếu accepted start/terminal receipt và fact; sau góp ý, root bổ sung expected/before/after revision, external cancellation tick và source vocabulary. Còn phải chạy tests và build/probe ở source cuối trước khi critic đóng finding. Không có quyết định nghiệm thu Unity, full profession, full VS-03, save compatibility mọi phiên bản hoặc playable demo trong report này.

Root chịu trách nhiệm integration/source freeze, full regression và negative controls, raw evidence, docs/exports, CI và Git. Critic chỉ sở hữu report này và file adversarial tests được giao bổ sung; không commit/push/merge hoặc sửa production.

## Disposition bổ sung bởi root — 03/11/2026

Report phía trên giữ kết luận và evidence tại lượt actor ban đầu; các câu chờ full run là trạng thái lịch sử. Root đã đối chiếu full gate562/562 Passed (113 repair),0failed/skipped và Host/process gates ở revision cuối; [evidence Sprint14](../SPRINT-14-EVIDENCE.md) và [final review/disposition](FINAL-REVIEW.md) là nguồn nghiệm thu hiện hành. Shared history source có bản siết workflow sau fingerprint của critic commands; phần sửa đó được critic cuối review độc lập. Không nhận root đối chiếu artifacts là việc critic ban đầu tự thực hiện.
