# Git, branches, commits và releases

Quy trình áp dụng cho source, tài liệu, content và skills của mythical_dnd. Remote chuẩn: `origin` → `git@github.com:nguyenhaisy1/mythical_dnd.git`. Đây là quy định làm việc; branch protection/required checks trên GitHub phải được cấu hình riêng, không tự tồn tại vì file này được tạo.

## 1. Branch model

| Branch | Tạo từ | Merge về | Mục đích |
|---|---|---|---|
| `main` | lịch sử ổn định hiện có | — | Các mốc đã nghiệm thu; tag release trỏ vào commit đã kiểm ở đây |
| `dev` | `main` lúc bootstrap | qua `release/*` vào `main` | Tích hợp công việc cho milestone kế tiếp |
| `feature/<task-id>-<slug>` | `dev` mới nhất | `dev` | Feature, ví dụ `feature/fnd-02-typed-identity` |
| `fix/<task-id-or-slug>` | `dev` | `dev` | Sửa lỗi chưa phát hành |
| `docs/<slug>`, `chore/<slug>` | `dev` | `dev` | Tài liệu, skills, tooling; không cần bịa task ID |
| `spike/<slug>` | `dev` | thường không merge nguyên nhánh | Thử nghiệm; đưa kết luận/code đạt chuẩn về branch riêng nếu cần |
| `release/<X.Y.Z>` | commit `dev` được chọn | `main`, rồi đồng bộ lại `dev` | Đóng phạm vi, sửa blocker, hoàn thiện changelog/version/evidence |
| `hotfix/<X.Y.Z>-<slug>` | tag stable đang được hỗ trợ | `main`, rồi đồng bộ `dev` và release đang mở | Sửa khẩn cấp bản đã phát hành |

Tên lowercase/kebab-case, một branch cho một mục tiêu; không dùng tên người hoặc `temp`, `final-final`. Không làm feature trực tiếp trên main/dev. Nhánh release không nhận feature mới; thay đổi scope phải quay lại quyết định milestone.

Bootstrap một lần: tạo dev từ main hiện có. Commit đầu tiên `1e45977` là snapshot tài liệu/scaffold, không phải game release. Không tạo tag giả cho lịch sử này. Đợt thiết lập quy trình tạo local dev và docs/git-release-workflow; không mặc định các branch đó đã xuất hiện trên remote.

## 2. Trước khi sửa và trước khi commit

Kiểm `git status`, branch hiện tại, remote URL và upstream. Khi cần dựa trạng thái remote, fetch trước, không suy từ cache cũ. Working tree có thay đổi của người dùng thì giữ nguyên, không reset/clean/stash hoặc mang sang branch khác một cách âm thầm. Dùng worktree khi cần tách công việc.

Ví dụ khi dev đã tồn tại trên remote và working tree sạch:

```bash
git fetch origin
git switch dev
git merge --ff-only origin/dev
git switch -c feature/fnd-02-typed-identity
```

Nếu dev diverged, kiểm các commits trước khi merge; không reset về origin. Branch feature đã push thì sync dev bằng merge để giữ lịch sử; chỉ rebase commit chưa chia sẻ khi phù hợp, không force-push để làm PR đẹp.

Một commit là một thay đổi logic có thể review/revert. Code, tests, docs/fixtures bị ảnh hưởng phải đi cùng thay đổi; tránh commit implementation một phiên bản và contract phiên bản khác. Stage đúng file/hunk của task; chỉ `git add --all` khi đã kiểm toàn bộ diff đều thuộc phạm vi được giao.

Trước commit: kiểm staged diff, thông tin nhạy cảm, file lớn/binary và .gitignore; chạy `git diff --cached --check` cùng checks phù hợp. Không đưa credentials, bin/obj, Unity caches, generated exports hoặc SDK vào Git. Whitepaper HTML và generated GDD trong docs được track theo cấu trúc hiện tại; artifacts HTML/DOCX vẫn ignored. Không tạo ZIP.

## 3. Commit messages

Dùng cấu trúc Conventional Commits: `type(scope): mô tả kết quả`. Type: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `ci`, `chore`, `perf`, `revert`. Scope ngắn như `identity`, `scheduler`, `content`, `ui`, `docs`, `skills`, `release`. Subject rõ, mục tiêu khoảng 72 ký tự; không dùng “update”, “fix stuff” làm toàn bộ mô tả.

Ví dụ:

```text
feat(identity): add transactional entity ID allocation

Task: FND-02
Tests: TEST-027; allocator rollback and restore cases
```

Breaking contract dùng `!` và footer `BREAKING CHANGE:` mô tả ảnh hưởng, migration và compatibility. Không gắn feat cho scaffolding chưa có behavior; không ghi Tests: passed nếu chỉ thêm spec. Tên commit/PR không tự chứng minh task đã Done.

## 4. Commit, push và merge

Yêu cầu implement không tự đồng nghĩa publish. Thực hiện commit/push khi người dùng đã yêu cầu hoặc có chỉ dẫn áp dụng cho phạm vi hiện tại; không hỏi lại quyền đã có. Lệnh “commit/push thay đổi này” cho phép hoàn tất checks, commit và push branch phù hợp, nhưng không tự cho phép merge main, tạo tag hoặc phát hành. Nếu chỉ được yêu cầu viết rule/docs, giữ thay đổi để review và báo trạng thái Git chính xác.

Khi được giao push:

```bash
git push -u origin <work-branch>
```

Push branch đích rõ ràng; không `--all`, `--mirror`, `--tags` hoặc force push. Không đổi remote sang URL khác để né lỗi quyền. Sau push so remote branch SHA với local HEAD, kiểm working tree và báo branch/commit; khi lỗi phân biệt commit local đã có với remote chưa cập nhật.

Feature/fix/docs/chore → dev qua PR. Mặc định squash merge thành commit mô tả đầy đủ; task có nhiều commits độc lập đáng giữ có thể merge commit với lý do. PR ghi task/scope, hành vi trước/sau, checks và tác động save/schema/content. Review và CI phải áp lên revision mới nhất; sau thay đổi tiếp thì kiểm lại phần evidence mất hiệu lực.

Release/hotfix → main dùng merge commit hoặc fast-forward giữ ancestry; không squash release rồi mất quan hệ với dev. Sau main update, merge main trở lại dev qua PR/sync branch và xử lý xung đột; không chép file rời hoặc quên hotfix. Nếu release branch đang mở, đưa hotfix vào đó trước lần candidate tiếp theo. Chỉ xóa work branch khi đã merge và việc dọn branch được cho phép; không xóa remote branch có thay đổi chưa tích hợp.

Branch protection đề xuất: main/dev cấm direct push và force push/delete; merge qua PR, required CI `validate`. Điều kiện approval theo số người review thực tế, không yêu cầu self-approval bất khả thi cho solo owner. File này chưa cấu hình server-side rules; kiểm GitHub settings khi được giao thiết lập.

## 5. Version và compatibility

Root `VERSION` chứa version sản phẩm kế tiếp đang làm, không prefix v. Khởi đầu `0.1.0-dev`, nghĩa là chưa phát hành. Không tự đổi AssemblyVersion/Unity bundleVersion theo file này khi pipeline chưa triển khai; task release phải wire/check những nơi dùng product version thực tế.

- Trong giai đoạn `0.x`: tăng MINOR cho milestone mới hoặc breaking contract; PATCH cho sửa tương thích trên milestone đã phát hành. Không hứa save compatibility chỉ vì chưa lên 1.0.
- Từ `1.0.0`: MAJOR cho phá compatibility của public contracts đã cam kết; MINOR cho chức năng tương thích; PATCH cho sửa lỗi tương thích. Mốc 1.0 cần quyết định readiness riêng, không sinh từ số lượng tài liệu.
- Candidate dùng `X.Y.Z-alpha.N`, `X.Y.Z-beta.N` hoặc `X.Y.Z-rc.N`; stable dùng `X.Y.Z`. Git tag thêm prefix: `v0.1.0-rc.1`, `v0.1.0`.
- Build thử trên branch có thể báo `<VERSION>+<short-sha>` nhưng không là release/tag.
- Tách product version khỏi GDD baseline `1.1-prototype-contracts`, identity/codec/RNG/schema/content versions. Release note phải liệt kê những version compatibility thực sự thay đổi; không đồng loạt bump các hệ để nhìn giống nhau.

Cùng version đã phát hành là bất biến: không retag, sửa nội dung dưới tag cũ hoặc rebuild rồi thay binary cũ. Sửa bằng version mới. Doc-only release cũng phải ghi rõ docs/tooling-only, không ngụ ý có game playable.

## 6. Quy trình release

1. Khi được giao chuẩn bị release, chọn scope/target X.Y.Z và commit dev đã có evidence; tạo release/X.Y.Z. Đóng feature mới.
2. Cập nhật VERSION candidate, CHANGELOG và checklist. Chạy checks/build/tests đúng phạm vi release; thiếu Unity/runtime evidence phải ghi rõ và không vượt gate. Candidate tag tùy chọn trên release branch, chỉ tạo/push khi có yêu cầu phát hành candidate; mỗi candidate có version/changelog dated riêng.
3. Khi được giao phát hành stable, cập nhật VERSION=X.Y.Z và mục CHANGELOG ngày thực tế; kiểm nội dung stable trên revision cuối, merge release vào main. Kiểm CI main commit trước khi tag.
4. Tạo annotated tag vX.Y.Z đúng commit main đã kiểm, push riêng tag đó. GitHub Release, nếu được yêu cầu, dùng nội dung CHANGELOG và evidence; không tự tạo/upload ZIP. Nếu không có binary playable, ghi rõ loại release.
5. Xác nhận remote tag peeled commit khớp commit dự định; phân biệt tag đã push với GitHub Release đã publish. Ghi commit, version, ngày, evidence và giới hạn trong release note.
6. Đồng bộ main về dev. Trên dev qua chore branch/PR, đặt VERSION cho mục tiêu tiếp theo `X.Y.Z-dev` và tiếp tục Unreleased. Không sửa VERSION trên main sau release nếu chưa có release mới.

Hotfix cũng cần changelog/version/tests và tag mới; dùng patch mới nếu tương thích. Rollback triển khai về tag cũ không rewrite Git; rollback source dùng revert qua branch/PR và phát hành version sửa phù hợp.

## 7. Changelog theo từng bản

`CHANGELOG.md` là nguồn duy nhất cho nội dung release note; không dùng git log thô thay mô tả. Trong phát triển, thêm mục vào Unreleased trong cùng thay đổi đáng ghi nhận. Typo/nội bộ không có tác động đáng kể có thể bỏ qua, nêu lý do ở PR khi cần.

Mỗi release có `## [X.Y.Z] - YYYY-MM-DD`, loại/phạm vi release và các nhóm không rỗng: Added, Changed, Fixed, Deprecated, Removed, Security. Bắt buộc có Compatibility, Validation, Known limitations khi thông tin đó ảnh hưởng sử dụng. Validation dẫn evidence/CI/build cụ thể; Known limitations không được dùng để che một gate bắt buộc chưa đạt.

Khi chốt release, chuyển đúng các mục thuộc scope từ Unreleased vào version; giữ phần chưa release ở Unreleased. Không ghi ngày phát hành dự kiến như đã phát hành. Candidate và stable có notes riêng nếu đã publish cả hai; stable tổng hợp thay đổi từ stable trước, không chỉ diff từ rc cuối. Với release đầu, ghi initial baseline và commit nguồn; không gán version ngược cho commit chưa từng được tag.

Nguồn quy ước: [Conventional Commits 1.0](https://www.conventionalcommits.org/en/v1.0.0/), [SemVer 2.0](https://semver.org/spec/v2.0.0.html), [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). Branch model và chính sách 0.x ở đây là lựa chọn riêng của project.

## Nhật ký Git theo spin

Append branch/commit/push/merge/tag/release và kết quả xác minh thực tế vào [WORKLOG](../../WORKLOG.md), phân biệt local/remote. Ghi event sau commit/push làm journal có diff mới là bình thường; đưa event vào commit tiếp theo, không amend hoặc tạo chuỗi commit chỉ để tự ghi hash. Khi báo working tree, kiểm sau khi ghi log và nêu phần journal chưa commit nếu có.

## Baseline AI và bootstrap dev local — 2026-10-15

Dự án do AI thực hiện theo [AI Execution](AI-EXECUTION.md). AI tự chạy checks và review diff; không bắt buộc người duyệt thường lệ. Không ghi self-review như review độc lập, không né required checks/server protection.

Ngoại lệ bootstrap một lần cho dev **chưa publish**: khi được giao chốt baseline, commit bộ docs/workflow đã kiểm trên docs branch, rồi fast-forward dev local tới commit đó trước khi tạo feature branch. Ghi SHA và checks vào WORKLOG. Không áp dụng ngoại lệ cho dev remote đã có hoặc main; các lần tích hợp sau vẫn qua PR. Việc chốt baseline cho phép commit local; không mặc định push/tag/release.
