# Nguồn tích hợp thiết kế game — 27/10/2026

Các reference mới được biên soạn lại theo mythical_dnd, chọn lọc chuyên môn từ hai nguồn MIT. Giữ nguyên license/notice upstream trong thư mục này. [Manifest](design-sources.json) ghi exact commit, file, URL và SHA256 của nội dung đã đọc; không dùng star count như bằng chứng hiệu quả.

| Nguồn | Phần dùng | Nơi áp dụng |
|---|---|---|
| Donchitos/Claude-Code-Game-Studios: art-director.md | Visual identity, art bible, hierarchy, palette, asset specs và trade-offs | mythical-prototype/references/game-art-direction.md |
| Donchitos/Claude-Code-Game-Studios: design-review/SKILL.md | Completeness, dependencies, consistency, implementability và re-review evidence | mythical-validation/references/game-design-review.md |
| sickn33/agentic-awesome-skills: game-art/SKILL.md | Style/pipeline, silhouette, value contrast, animation và scale | mythical-prototype/references/game-art-direction.md |

[MIT — Game Studios](game-studios-LICENSE.txt), [MIT — Agentic Skills](agentic-skills-LICENSE.txt).

Không import hooks, Bash snippets, framework routing, model selection hoặc AskUserQuestion của Claude. Không cài nguyên agents/templates/tooling upstream. Không mang quy tắc xin phép ghi từng phần vào project; user authorization và workflow hiện hành có ưu tiên. Artifacts production không tự đổi ownership hoặc layout. Sáu skill hiện tại giữ nguyên identity và symlink; chỉ thêm năng lực chọn lọc, không tự sinh tác nhân mới.
