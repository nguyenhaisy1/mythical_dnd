# Báo cáo chốt Sprint13 — Đất dựng thành 2D và cổng mở lại G13

03/11/2026 · SPIN-037 · Điều phối `/root`.

## Tổng hợp dễ hiểu

Chủ dự án đã chọn hướng **Đất dựng thành bằng nền2D có cảm giác khối + nhân vật sprite**. Vòng chọn hướng Sprint13 kết thúc, đồng thời giữ **cổngG13** để trở lại nghiên cứu mỹ thuật khi triển khai không đạt mong muốn. Việc quay lại chỉ xem lại art/camera/sprite/UI/presenter theo phạm vi, tiếp tục dùng engine, luật và dữ liệu mới nhất; không đưa toàn bộ dự án về Sprint1 hoặc source cũ.

Đã ghi quyết định, cập nhật hồ sơ áp dụng và tạo checkpoint có fingerprint thư viện01–18. Đây là chốt hướng và cơ chế quản lý thay đổi, chưa tạo nền phân lớp, sprite chuyển động, sceneUnity, game/demo hoặc cơ chế đổi profile runtime. A02/A03 chi tiếtUI/onboarding tiếp tục có cổng thực hiện và nghiệm thu riêng.

## Quyết định hiện hành

- **Profile:** `earth-monument-2d-v1`; nguồn mỹ thuật là [ảnh Đất dựng thành14](../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png).
- **Sprint13:** Accepted ở phạm vi định hướng. **G13:** Accepted / Reopenable, sẵn khả năng mở lại chứ không để sprint Pending mãi.
- **A01/A04:** chủ dự án chốt hướng/kết thúc vòng chọn, giữ quyền mở lại. **A02/A03:** Deferred implementation gates, không Approved/Passed cho pixel/layout/Revealonboarding cụ thể. Đây là diễn giải phạm vi bàn giao của AI, không gán cho user quyết định chi tiết chưa nói.
- **Nguồn áp dụng:** [baseline/G13](ART-DIRECTION-BASELINE.md), [approval](APPROVALS.md), [ADR022](../../../adr/022-earth-monument-2d-and-reopen-gate.md), DEC-028/029.
- Giữ BếnVạc/TamGiang và sửa thuyền; thành đất là ngôn ngữ khối/bố cục, không tự biến địa bàn thành công trường hoặc xác nhận phục dựng khảo cổ.

## Quay lại G13 mà giữ tiến độ

1. Ghi lý do/evidence và khoanh asset, sprite, camera, occluder, UI/presenter cần đổi. Chỉ gate phần phụ thuộc thực sự.
2. Dùng `dev` hiện hành làm nền candidate, giữ profilev1 và toàn bộ thư viện. Không reset/revert cả sprint hoặc checkout engine/data cũ đè lên công việc mới.
3. Tạo version/candidate riêng, so ở camera/tỷ lệ thật và kiểm sprite/contact/che khuất/UI. Core/rules/content/IDs/query-command contracts vẫn tách biệt.
4. Nếu chạm contract/save thật, có ADR/impact/migration và acceptance riêng. Không hứa tất cả asset/adapter hoặc save đều tự tương thích chỉ nhờ profile version.
5. Chủ dự án chốt candidate rồi cập nhật baseline/register/handoff. Candidate chưa chốt không tự thayv1; sửa art không tự mất nghiệm thu kernel đúng phạm vi đã kiểm.

Đây là quy trình quản trị/kiến trúc đã ghi, chưa là chức năng runtime hot-swap hoặc restore đã chạy. Hướng khác có thể cần làm lại view/asset; phần đó được ghi rõ thay vì hứa không có rework.

## Checkpoint và bảo toàn

[checkpoint-v1.json](checkpoint-v1.json) dùng source commit trước quyết định **`1e4332f0a122528b9bed850377529ffee1efb7da`**, inventory **268file /275.285.127bytes** của đủ18experiment. Có98visualassets bất biến, gồm93PNG và các SVG/GLB/BLEND đã có. Mọi byte/blob/SHA256 được đối chiếu với Git source;267file còn khớp worktree, README14 có banner mới nên được đánh dấu `source_commit_only` với blob cũ được giữ.

Manifest ghi bốn tree audit engine/content/rules/tests tại source, không tự hash manifest hoặc nhúng WORKLOG/exports. Các tree đó là dấu mốc lịch sử, không khóa phát triển sau này. Lần mởG13 tương lai phải lấy core hiện hành trước/sau thay art để đối chiếu; không phục hồi core cũ lên phần tiến độ mới.

Tích hợpPR13 ưu tiên **merge commit**, theo ngoại lệ giữ lịch sử của Git workflow, để sourceSHA checkpoint nằm trong ancestry `dev`; giữ featurebranch, không tạo release tag. Root đã kiểm GitHub cho phép mergecommit, auto-deletebranch=false vàPR13base=dev. Việc merge/ancestry/CI thực tế chỉ ghi khi hoàn tất trongWORKLOG; report này không gọi preflight là đã merge. Nếu policy server chặn mergecommit phải giữ anchor nguồn và ghi fallback trước tích hợp, không bypass policy.

Fingerprint/inventory chứng minh nguồn còn truy nguyên tại thời điểm kiểm; không thay backup ngoàiGit, restore drill hoặc save compatibility test.

## Phân công và review thật

| Actor | Ownership / kết quả |
|---|---|
| `/root/s13close_approval` | APPROVALS/SPRINT13: chốtA01/A04, chuyểnA02/A03Deferred, giữ lịch sử ngày trước |
| `/root/s13close_gate` | Canonicalbaseline/ADR022: profile, ranh giới2D, stateG13 và quy trình candidate khôngrollbackcore |
| `/root/s13close_consumers` | Roadmap/clientUX/implementationplan/README/docindex: trạng thái hiện hành, ROM3D không áp2D, không mởSprint14 |
| `/root/s13close_critic_scope` | [Reviewphạmvi](CLOSURE-CRITIC-SCOPE.md): approval/reopenability/unknownUX, không giảruntimepass |
| `/root/s13close_critic_preserve` | [Reviewbảotoàn](CLOSURE-CRITIC-PRESERVATION.md): Gitobjects/inventory/coretrees/retention và tác động |
| `/root` | Tích hợp, .agent/AGENTS/HANDOFF/governance/notes/checkpoint/report, kiểm lớp đầu, checks/exports/Git |

Ba worker được chia ownership; hai critic làm sau theo slot. Không gọi sáu actor cùng chạy; self-review của root không là independentreview. Findings và fingerprint final nằm trong hai report, không suy CI xanh thành approval của user.

## Tác động đã xử lý

| Thay đổi | Consumer và xử lý |
|---|---|
| Chốt định hướng2D/sprite | .agent/AGENTS/APPLICABLE/ADR/DECISIONS/APPROVALS/SPRINT13/HANDOFF/README/docindex và module23/28notes được cập nhật; archive/body nghiên cứu giữ nguyên |
| Các nghĩa vụ còn thiếu | OPEN-11/VS07/clientUX/roadmap giữ UI/onboarding, assetrights/audio/font/accessibility và gameplay/Player gates; không nâng TEST-MATRIX Passed |
| Giữ khả năng mở lại | CanonicalG13/checkpoint/Gitworkflow/closure report; giữ sourcebranch/ancestry, profilev1/library01–18 |
| Dự toán và nguồn tham khảo | ROM3D ngày02/11 được giữ với banner hiện hành, không dùng số cũ cho route2D; report14/README14 có banner, artwork/provenance/gallerycode nguyênbytes |
| Tách lớp kỹ thuật | ClientREADME/Unitybootstrap dẫn profile; core/rules/content/schema/save/runtimecode không đổi |
| Bản đọc tổng hợp | integration-notes là source cho generatedmodules/Master; exportinputs bổ sung baseline/report/reviews; regenerate và kiểm fingerprint sau chốt nguồn |

Không thay VERSION, rule/content/saveversions, canonicalIDs, nguồn docs/source, engine/tests hoặc nghiệm thu runtime cũ. Whitepaper không cam kết route mỹ thuật cụ thể nên không cần sửa để áp hướng này. Không tạoZIP, asset mới, Hyper3D/Blender job hoặc Sprint14.

## Evidence và bước tiếp theo

Kiểm bắt buộc của scope này: generated/source coverage, link/register/DAG, architecture boundaries, checkpointbytes/Gitobjects/protectedtree và exportfingerprints/navigation. Kết quả run và Git/CI đúngrevision được ghi vàoWORKLOG sau khi chạy; không nhận report kế hoạch thành test đãPassed.

Bước phát triển đề xuất là proof2D giới hạn: một sân, một đoạn cầu, hai sprite và động tác đặt bình để kiểm layering/độ sâu/animation/contact/selection. Cần taskcontract và estimate2D riêng; chưa chạy trong lần chốt này. Chủ dự án chưa giao mởSprint14 ở yêu cầu này. CổngG13 có thể được dùng khi phát triển cần thử hướng khác, giữ tiến độ và lịch sử theo baseline trên.
