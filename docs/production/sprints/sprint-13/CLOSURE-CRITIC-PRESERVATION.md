# Phản biện độc lập — bảo toàn tiến độ và cổng G13

03/11/2026 · SPIN-037 · Actor `/root/s13close_critic_preserve` · **Documentation / source-integrity review**.

## Kết luận trong phạm vi đã kiểm

Cơ chế G13 được mô tả đúng mục tiêu: đóng vòng chọn hướng Sprint13, giữ profile `earth-monument-2d-v1`, và tạo candidate mỹ thuật từ code tích hợp hiện tại khi cần xem lại. Không quay engine hoặc cả repository về commit cũ. Không phát hiện P1 cản chốt định hướng trong phạm vi preservation đã kiểm.

Đây là kiểm tài liệu, Git diff và tính toàn vẹn file. Không có runtime hot-swap, restore drill, save migration, gameplay, asset usability, hiệu năng hoặc human playtest được chạy bởi critic này. Không mở ảnh để đánh giá lại mỹ thuật; việc đọc byte/hash không được nhận thành visual review.

## Nguồn đã đọc

Đọc AGENTS hiện hành, `.agent`, HANDOFF, Applicable Baseline, Git Workflow, skill mythical-validation cùng reference game-design-review và test execution contract. Đọc [ADR022](../../../adr/022-earth-monument-2d-and-reopen-gate.md), [baseline/G13](ART-DIRECTION-BASELINE.md), current sections của [APPROVALS](APPROVALS.md), [Sprint13](../SPRINT-13.md) và toàn [checkpointv1](checkpoint-v1.json) ở dạng JSON.

Reviewer kiểm các trường manifest và toàn bộ file nguồn của nó; không chỉ đối chiếu một manifest với metadata do manifest tự khai. Lệnh Git trong lượt review là read-only; không commit/push/merge hoặc sửa ngoài file review này.

## Finding đã xử lý — giữ checkpoint source reachable sau tích hợp

**P2 ban đầu:** default squash merge không đưa commit nguồn `1e4332f0a122528b9bed850377529ffee1efb7da` thành ancestor của squash commit trên dev. Chuỗi SHA/hash ghi trong tài liệu không tự là backup, và có thể mất đường truy nguyên nếu sau đó xóa mọi nhánh giữ commit cũ.

**Đã báo root và đối chiếu sửa:** ADR022, baseline và Git Workflow hiện ghi dùng merge commit cho PR13 khi policy server cho phép, xác minh ancestry sau merge, đồng thời giữ feature branch trong công việc này. Nếu server yêu cầu squash/linear thì phải giữ anchor nguồn và ghi retention thực, không bypass protection hoặc tự tạo release tag. Checkpoint cũng ghi cùng policy.

**Disposition:** tài liệu đã xử lý finding. Chưa kiểm server policy, kết quả merge hay ancestry remote sau merge; root phải xác minh bằng evidence thực khi thực hiện. Reviewer không nhận việc dự định merge thành việc đã merge.

## Kiểm source/library độc lập

Input Git được kiểm: `1e4332f0a122528b9bed850377529ffee1efb7da`, là HEAD trước quyết định. `git ls-tree -r -l` cho **268 file** trong `prototypes/sprint13`, thuộc đủ Experiment01–18. Đọc byte hiện tại và tính Git blob hash độc lập cho từng file:

- Không file bị mất.
- **267 file** khớp nguyên byte với nguồn; khác duy nhất `experiment-14/README.md`, do thêm banner chỉ trạng thái hiện hành. Manifest phân loại đúng file này là `source_commit_only`.
- **98 visual assets** được giữ nguyên byte:93PNG,1SVG,2GLB,2BLEND. Lịch sử actual3D cũ vẫn được giữ; lựa chọn2D mới không sửa chúng thành nguồn2D.
- Các file còn lại gồm44Markdown nguyên byte,26JS,20Python,18HTML,18CSS,39JSON,2MJS và2TXT. README14 lịch sử vẫn lấy lại được từ Git blob đã ghi.

Đã kiểm `git diff` từ input SHA cho engine, content, rules, tests, docs/source, global.json, solution và .github: không có tracked diff, đồng thời không có untracked file mới trong các phạm vi đó ở thời điểm kiểm. Đây là evidence rằng lần chốt này chưa viết lại core, rules, dữ liệu hay archive; không chứng minh code gameplay đã được triển khai hoặc mọi file sẽ bất biến ở các sprint tương lai.

## Kiểm checkpoint độc lập

Đọc [checkpoint-v1.json](checkpoint-v1.json), schema `sprint13-art-checkpoint-v1`, profile `earth-monument-2d-v1`, gateG13. Python stdlib và `git cat-file --batch` đọc thật từng blob từ source commit, rồi tính SHA256/length và kiểm path:

| Phép kiểm | Kết quả thực |
|---|---|
| Set paths, unique paths và số file so với Git source |268/268, đủ và không trùng |
| Git blob ID, SHA256 và byte length từng file |268/268 khớp |
| Tổng bytes |275.285.127, khớp manifest |
| Phân bố18experiment |Khớp từng count trong manifest |
| Rows `source_commit_and_worktree` |267/267 nguyên byte |
| Row `source_commit_only` |Chỉ README14 đã thêm banner |
| Immutable visual assets |98/98 nguyên byte |
| Source tree IDs engine/content/rules/tests |4/4 khớp `git rev-parse source:path` |

Lượt kiểm trên exit0. SHA256 checkpoint được review là `12234b36fec256b7401a4dd1ae809d3e026aa391a91dbc58e789c6e5fd7c94c2`.

Manifest không liệt kê chính nó, WORKLOG, exports hoặc các file approval/ADR/closure mới; không tạo vòng lặp hash–commit. `source_commit` là đầu vào lịch sử, không được gọi là commit chốt mới hay release. Tree core ở checkpoint chỉ là mốc audit: quy tắc G13 đã nói rõ so current-before/current-after khi đổi mỹ thuật về sau, không khôi phục tree cũ đè tiến độ mới.

Có inventory/hash và Git source reachable là cơ sở truy nguyên; chưa phải bằng chứng đã thực hành phục hồi hoặc mọi save mới sẽ tương thích với mọi candidate.

## Kiểm boundaries và tác động

- Candidate/revision mới tách art/sprite/camera/UI/presenter, giữ profilev1 và thư viện. Quy trình xuất phát từ `dev` hiện hành; không reset, revert toàn sprint/merge hoặc checkout engine cũ để đổi hình ảnh.
- Entity/definition IDs, rule/content versions, WorldTick, command/receipt và actor-scoped projection giữ ranh giới độc lập. Pixel/surface/asset/look mapping là presentation; animation callback không được ghi truth, hoàn thành repair hoặc tiêu gameplay RNG.
- Không hứa mọi asset hoặc view adapter có thể giữ nguyên. Nếu thật sự cần đổi contract/schema/save thì có ADR/impact/migration/acceptance riêng; không âm thầm dùng thay đổi mỹ thuật để bỏ các sprint trước.
- Bến Vạc/Tam Giang và vòng sửa thuyền không chuyển thành công trường xây thành chỉ vì chọn art14. PNG vẫn là concept, chưa layer/collision/path hoặc runtime scene.
- Chốt hướng không biến A02/A03 thành Passed UX, không tự tạo Sprint14 hoặc release. G13 là Accepted/Reopenable, không giữ vòng chọn hướng Pending vô hạn.
- ROM3D của SPIN-036 được giữ dưới dạng lịch sử, không áp các số giờ/ngày vào route2D vừa được chọn.

## Đối chiếu hồ sơ cuối và ranh giới tích hợp

Đã đọc trọn [CLOSURE-REPORT](CLOSURE-REPORT.md) sau khi file được tạo, rồi đọc lại retention cuối ở ADR022, baseline và Git Workflow. Report khớp inventory268file/275.285.127bytes/98visualassets,267worktree/source-onlyREADME14; không tự nhận manifest là backup ngoàiGit, restore drill hoặc save compatibility test. Report nói rõ preflight merge policy do root thực hiện và chưa phải merge/ancestry đã hoàn tất. Critic này không tự kiểm GitHub settings hoặc kết quả merge remote.

Baseline và report đều giữ candidate từ dev hiện hành, current-before/current-after để bảo toàn tiến độ về sau, không restore bốn tree cũ; không tự nghiệm thu runtime hoặc dùng ROM3D làm estimate2D. Các yêu cầu xử lý finding retention đã được ghi rõ, không còn finding bảo toàn cản bàn giao ở phạm vi đã đọc.

Fingerprint các tài liệu được review cuối:

- `docs/production/sprints/sprint-13/CLOSURE-REPORT.md`: SHA256 `938d170f50357c31959f8db75c07990383d4d29b5ab9091157c8053db1dac8d2`.
- `docs/production/sprints/sprint-13/ART-DIRECTION-BASELINE.md`: SHA256 `ce141aa3df551b9a2f658e06555cdf8d327394bb437fe3d9e717d0b129cfc692`.
- `docs/adr/022-earth-monument-2d-and-reopen-gate.md`: SHA256 `1e305428d6995804019661d410888a9bd9ccfec9bf4716bf458d9a3c7dbd49de`.
- `docs/production/sprints/sprint-13/checkpoint-v1.json`: SHA256 `12234b36fec256b7401a4dd1ae809d3e026aa391a91dbc58e789c6e5fd7c94c2`.

Sau cập nhật retention/report, đã đối chiếu lại Git diff và untracked files của engine/content/rules/tests/docs-source/global.json/solution/.github: vẫn không thay đổi. Python kiểm manifest và file formatting/link của critic exit0. Root phụ trách source/archive coverage checks, generated modules, exports, Git/CI và ancestry/anchor sau tích hợp; critic không nhận các phép kiểm root chưa chạy thành kết quả của mình.

**Disposition cuối:** technical reviewed cho contract bảo toàn, hồ sơ quyết định và inventory/hash sau sửa retention; không có P1 còn mở trong scope. Asset/runtime/restore/save compatibility chưa được nghiệm thu. Quyết định chốt Sprint13 đến từ user, không đến từ review này.
