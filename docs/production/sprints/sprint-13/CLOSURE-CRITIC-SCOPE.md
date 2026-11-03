# Phản biện độc lập — phạm vi chốt Sprint13 và cổng G13

03/11/2026 · SPIN-037 · Actor `/root/s13close_critic_scope` · **Documentation / decision-scope review**.

## Kết luận trong phạm vi đã kiểm

Quyết định hiện hành ghi đúng yêu cầu của chủ dự án: chốt **Đất dựng thành bằng nền2D có cảm giác khối + nhân vật sprite**, profile `earth-monument-2d-v1`, kết thúc vòng chọn hướng Sprint13 và giữ **G13 Accepted / Reopenable**. Không phát hiện P0/P1 trong cách phân định approval, nghĩa vụ triển khai tiếp và phạm vi mở lại mỹ thuật ở các tài liệu đã đọc.

Đây là kiểm phạm vi tài liệu, không phải AI duyệt thay chủ dự án. Approval đến từ lời chốt thật ngày03/11/2026. Review này không xác nhận chất lượng sprite, scene, animation, Unity runtime, thời gian hoàn thành hoặc human playtest. Không coi sự tồn tại của cổng G13 là bắt Sprint13 tiếp tục Pending vô hạn.

## Các nguồn thực sự đã đọc

- `.agent`, `AGENTS.md`, [HANDOFF](../../HANDOFF.md), [Applicable Baseline](../../../governance/APPLICABLE-BASELINE.md); skill `mythical-validation`, reference game-design-review và Test Execution Contract.
- [APPROVALS](APPROVALS.md), [SPRINT-13](../SPRINT-13.md), [ART-DIRECTION-BASELINE](ART-DIRECTION-BASELINE.md), [ADR022](../../../adr/022-earth-monument-2d-and-reopen-gate.md).
- [Roadmap](../../PLAYABLE-ROADMAP.md), [Client UX](../../CLIENT-UX-BASELINE.md), [Implementation Plan](../../IMPLEMENTATION-PLAN.md), README root và docs index; ghi chú áp dụng module23/28 trong `tools/docs/integration-notes.json`.
- Nguyên văn yêu cầu chủ dự án trong context được giao; đối chiếu với trích dẫn ở approval/baseline/ADR. Không suy từ lời khen hoặc “tiếp tục” ở các vòng trước.

Không mở lại ảnh concept trong lượt review này vì câu hỏi được giao là phạm vi quyết định/closure và bảo toàn kiến trúc. Không nhận các quan sát hình ảnh của worker hoặc root thành quan sát mỹ thuật độc lập của critic này. Đã đọc metadata/policy ở đầu checkpoint-v1.json để đối chiếu scope lịch sử, chưa kiểm mọi file thư viện, blob Git hoặc khôi phục checkpoint; phần đó do critic recovery/root xử lý riêng.

## Ma trận kiểm phạm vi

| Điều cần bảo đảm | Kết quả đối chiếu |
|---|---|
| A01 chốt đúng route | Approved art direction lấy mẫu14, nền2D/sprite; không biến nghiên cứu3D SPIN-036 thành route đang chọn hoặc số giờ2D |
| A04 kết thúc sprint có cổng | Approved direction closure with G13; hướngv1 có hiệu lực và khả năng quay lại còn sẵn, không một request Reopened giả tạo |
| A02/A03 chưa được duyệt chi tiết | Deferred implementation gates; register ghi rõ đây là diễn giải phạm vi của AI. UI layout/Reveal usability/full generator chưa Approved hoặc Passed, phải đưa vào task client/onboarding khi được giao |
| Không chặn lệnh chốt bằng pending cũ | Approval và sprint brief có phần hiện hành ở đầu, lịch sử cũ đặt dưới banner ngày03/11. Các câu Pending ở phần lịch sử không phủ quyết quyết định mới |
| V1 và phương án mới cùng được giữ | Candidate có revision riêng, v1 tiếp tục baseline đến khi chủ dự án chốt revision thay thế; giữ thư viện01–18 và không tự coi các phương án còn lại bị chê/bỏ |
| Mở lại giữ tiến độ mới | G13 dùng code tích hợp hiện tại, khoanh ảnh hưởng art/camera/UI/presenter. Không reset/revert toàn repository hoặc dùng engine cũ đè lên công việc Sprint1–12 và về sau |
| Luật và dữ liệu không thuộc rendering | Stable IDs, WorldTick, rules/content, actor-scoped projection, command/receipt và persistence contracts được giữ; callback animation không ghi world truth hoặc tiêu tài nguyên/RNG |
| Thay contract thật phải được nhận diện | Nếu art candidate đòi schema/gameplay/save change, phải impact/ADR/migration và acceptance riêng; không miễn kiểm compatibility bằng nhãn “đổi art” |
| Baseline sản phẩm | Sinh nhân vật ngẫu nhiên có nhân quả, không chọn/chỉnh origin, không Reveal reroll; offline rules, không runtime LLM decision; UI không nhận raw omniscient truth |
| Canon và scale | Ngôn ngữ công trường được áp về Bến Vạc/Tam Giang, không dời địa danh hoặc đổi repair thành xây thành; PNG không chứng nhận phục dựng khảo cổ |
| Sprint tiếp theo và lịch | Không tự mở Sprint14; forecast15–16 là lịch sử, cần re-estimate2D và đo. Không hứa tỷ lệ tiết kiệm, ngày hoàn thành hoặc FPS từ ảnh |
| Evidence chưa có | Không nhận asset production, UI, rig/sprite quality, save Unity/Player, gameplay, performance hoặc human playtest đã đạt từ lệnh chốt mỹ thuật |

## Nhận xét và các gate còn phải thực hiện

Không có finding P0/P1 yêu cầu thay quyết định hiện hành. Những mục sau đã được ghi là nghĩa vụ tương lai, không phải lỗi cần giả Passed để đóng Sprint13:

1. **S13-A02/A03:** client/onboarding task sau phải nêu acceptance về focus/modal/chữ Việt/tri thức và cách trình bày biography; source prototype chưa thành bằng chứng full generator hoặc usability.
2. **2D production:** vùng đi, surface/tầng, sắp lớp, sprite directions/animation, camera/picking/contact phải được đặc tả và kiểm khi được giao implementation. V1 là hướng, chưa hệ hot-swap profile đã code.
3. **Reopening:** evidence/gap của AI cho phép đề xuất hoặc điều tra trong scope đang giao; không tự duyệt style mới. Quyết định final về revision thay thế vẫn thuộc chủ dự án.
4. **Compatibility:** bảo toàn tiến độ là quy tắc kiến trúc/quy trình, không lời hứa mọi asset cũ dùng lại được hoặc mọi save tương lai tự tương thích. Phần trình bày bị thay cần evidence mới; phần core độc lập không bị xóa acceptance cũ vô cớ.

## Đọc lại bản cuối sau bổ sung checkpoint và retention

Đã đọc [báo cáo đóng sprint](CLOSURE-REPORT.md), đoạn retention thêm ở baseline/ADR022 và [Git workflow](../../../runbooks/GIT-WORKFLOW.md), cùng phần scope/policy trong checkpoint. Không có P0/P1 mới trong các thay đổi này.

- Closure report vẫn tách chốt hướng khỏi runtime/UI/onboarding và không tự mởSprint14. Không biến manifest thành lời hứa mọi asset tái sử dụng được hoặc restore/save compatibility đã kiểm.
- SourceSHA trong checkpoint là nguồn trước quyết định; profile/approval hiện hành nằm ở tài liệu có phiên bản riêng. Core trees chỉ là dấu audit lịch sử: reopening phải so core hiện hành trước/sau sửa art, không ép code tương lai về trees cũ.
- Retention đề xuất merge commit để giữ ancestry khi policy cho phép, giữ source branch, có fallback anchor trước tích hợp nếu server chỉ cho squash/linear. Đây là quy trình bảo toàn; chưa nhận merge/CI/ancestry đã xong, không tạo release tag hoặc bypass protection.
- Các số inventory và GitHub preflight trong closure report là kết quả root/critic preservation chịu trách nhiệm kiểm. Critic scope này không nhận đã tự kiểm blob/count/server settings; chỉ đối chiếu chúng không bị dùng thành gameplay acceptance hoặc restore guarantee.

## Evidence kiểm tài liệu

Python stdlib lượt cuối đã đọc15file cửa vào/approval/canonical/consumer, thêm closure report và Git workflow; kiểm UTF-8, newline và trailing whitespace: exit0. Đối chiếu222liên kết local trong các file đó: mọi đích tồn tại. Lượt trước có ba tham chiếu checkpoint chưa tạo, nay đã được root tạo và liên kết đã đối chiếu lại. Đây là subset link check, không tuyên bố whole-repository links/exports đã Passed; root phụ trách integration checks.

Fingerprint SHA256 tại thời điểm đối chiếu:

| File | SHA256 |
|---|---|
| ART-DIRECTION-BASELINE.md | `ce141aa3df551b9a2f658e06555cdf8d327394bb437fe3d9e717d0b129cfc692` |
| ADR022 | `1e305428d6995804019661d410888a9bd9ccfec9bf4716bf458d9a3c7dbd49de` |
| APPROVALS.md | `40ac9259ed70dd9f9410447083f1fd3472197eba03e9a1e7cf961bdf5758147a` |
| SPRINT-13.md | `de0eff85363d30b761d327f8cb7b82c1956b042e46dfcf71fdff53abf8c0ba29` |

Không chạy Unity, code/runtime tests, benchmark, browser/playtest, Hyper3D, image generation, Git, exports hoặc thay acceptance matrix. Chỉ viết report này; root append WORKLOG và integration checks. Nếu các file thay đổi sau fingerprint trên, xác định phạm vi đọc lại trước khi nhận review cho bản cuối.

**Freeze scope review:** các bổ sung cuối về closure/retention đã được đọc; không còn finding cản bàn giao quyết định trong phạm vi này. Các checks tích hợp/exports/Git thực tế do root ghi sau run; không được dùng lời freeze này thay runtime hoặc restore evidence.
