# Bộ skill làm việc với mythical_dnd

Bộ này gồm sáu workflow viết riêng theo contracts và cấu trúc project. Source duy nhất nằm tại `tools/skills/`; bản cài cá nhân là symlink tới source. Không mang Unity Editor, MCP server, engine implementation hoặc framework multi-agent vào project.

| Skill | Khi dùng | Ví dụ yêu cầu |
|---|---|---|
| [mythical-task](../../tools/skills/mythical-task/SKILL.md) | Giao/review/thực hiện task backlog | `$mythical-task triển khai FND-02 trong phạm vi task và ghi evidence` |
| [mythical-change-impact](../../tools/skills/mythical-change-impact/SKILL.md) | Đổi contract/canon/parameter và đồng bộ consumer | `$mythical-change-impact đổi duration repair theo quyết định mới, cập nhật mọi nơi ảnh hưởng` |
| [mythical-validation](../../tools/skills/mythical-validation/SKILL.md) | Kiểm tra thay đổi hoặc đánh giá milestone | `$mythical-validation đánh giá evidence hiện tại có đủ đóng FND-04 không` |
| [mythical-prototype](../../tools/skills/mythical-prototype/SKILL.md) | Plan/build prototype hoặc phân tích playtest | `$mythical-prototype lập kế hoạch thử lựa chọn sửa thuyền hoặc không can thiệp tại Bến Vạc` |
| [mythical-kernel](../../tools/skills/mythical-kernel/SKILL.md) | C# core, rules, transactions, save/replay | `$mythical-kernel review implementation RNG so với contract và golden vectors` |
| [mythical-content](../../tools/skills/mythical-content/SKILL.md) | Chuyển nghiên cứu thành records có provenance | `$mythical-content author một event thiếu lương thực có eligible/ineligible cases` |

Skill chọn theo yêu cầu; không bắt chạy cả sáu cho mọi việc. Metadata cho phép automatic discovery mặc định, nhưng agent vẫn phải kiểm đúng repository. Yêu cầu hiện tại và AGENTS có ưu tiên; skill không tự xin duyệt lại công việc đã được cho phép, không tự mở rộng scope, delegate hoặc viết game từ một yêu cầu review.

## Cài và duy trì một nguồn

```bash
python3 tools/skills/install.py
python3 tools/skills/install.py --check
```

Đích mặc định là `$CODEX_HOME/skills`, hoặc `~/.codex/skills` khi biến chưa có. Có thể chỉ định `--destination /path/to/skills`. Installer kiểm trước cả sáu tên, không ghi đè file/thư mục hoặc symlink trỏ nơi khác; chạy lại cùng source là no-op. Không có download hay remote code execution. Nếu chuyển vị trí repository, kiểm lại links; installer sẽ báo liên kết khác thay vì tự thay dữ liệu người dùng.

Máy hiện tại đã liên kết sáu thư mục tại `/home/synh/.codex/skills/mythical-*` vào source trong repository. Danh sách skill của một phiên đang mở có thể chưa refresh; xác nhận discovery bằng phiên mới trước khi tuyên bố auto-routing hoạt động. Các file có thể được đọc trực tiếp ngay; chưa có bằng chứng harness đã tự kích hoạt chúng trong phiên này.

Không sửa một bản copy riêng ở thư mục cá nhân. Không tạo ZIP. Khi sửa skill, cập nhật source, kiểm frontmatter/metadata, rà tác động và chạy các checks liên quan; các links hiện có sẽ đọc source mới.

## Các điều chỉnh dành riêng cho dự án

- Đường dẫn theo `docs/gdd`, `docs/adr`, `docs/contracts`, CSV registers và `engine/`, không ép project chuyển sang layout framework bên ngoài.
- WorldTick, canonical codec, contextual RNG và atomic pipeline theo contracts; không áp Time.deltaTime cho authoritative simulation.
- Archive nguyên bản; generated modules đổi qua integration notes/source authored. Propagation xét cả fixtures, parameters, QA, backlog và export freshness.
- Không xem Python reference checks hoặc scaffold build là runtime acceptance. Không đóng Foundation nếu thiếu FND-16.
- Prototype có effort cap theo tình huống, không bắt hủy sau số vòng cố định hay luôn viết lại mọi code thử nghiệm. Playtest report phải có dữ liệu thật.
- AI chỉ author/assist development; không thêm LLM vào runtime. Unity official skills và Coplay vẫn chưa được cài bởi thao tác này.

## Nguồn tham khảo và provenance

Các hướng dẫn trong sáu SKILL.md được viết mới từ yêu cầu chủ dự án và hồ sơ trong repository; không copy nguyên prompts, hooks, scripts hay code từ ba bộ đã khảo sát. Không đưa chúng thành dependency runtime.

- [Unity skills](https://github.com/Unity-Technologies/skills/tree/9d282100540601cfddc4bbd69f36a239d7a6f55c): tham khảo cách chia chuyên môn Editor/UI/assets. Phần Unity-specific sẽ dùng upstream khi bootstrap.
- [Coplay Unity MCP](https://github.com/CoplayDev/unity-mcp/tree/142ea0af4890337f0ff6658df61db23982313717): tham khảo vòng kiểm compile/Console/test/ảnh; không tích hợp server/package.
- [Claude Code Game Studios](https://github.com/Donchitos/Claude-Code-Game-Studios/tree/be8993bbc5a1f016bc770b2846ce06272d284526): tham khảo ý tưởng prototype, scope, evidence và change propagation; viết lại theo contracts/paths hiện có, không mang nguyên orchestration/approval hooks.

Nếu lần sau sao chép mã hoặc nội dung upstream, kiểm và giữ notice/license tương ứng; hồ sơ tham khảo này không thay nghĩa vụ đi kèm phần được sao chép.

## Kiểm chứng lần tạo — 14/10/2026

Đã chạy validator của skill-creator cho cả sáu skill: frontmatter/name/description hợp lệ. Đã kiểm UI metadata, prompt gọi đúng `$skill-name` và automatic discovery không bị tắt. Python mặc định thiếu PyYAML; validator đã chạy bằng `/usr/bin/python3` có PyYAML, không cài thêm package vào project.

Installer đã được chạy trong thư mục tạm: fresh install, repeat idempotence, check links, collision với dữ liệu có sẵn. Collision trả lỗi trước khi tạo link khác và giữ nguyên file marker. Sau đó cài thật và `--check` xác nhận sáu links đúng source.

Rà quyết định bằng sáu tình huống dưới đây là **static scenario review**, không phải independent agent execution hoặc gameplay tests:

| Tình huống | Hành vi skill đã hướng dẫn |
|---|---|
| Review FND-02 khi chưa có runtime tests | Review scope; không tự implement, không kết luận Done |
| Đổi hệ số repair khi repo chưa có Git HEAD | Dùng before/after/snapshot; vẫn tìm consumer, không bỏ qua impact vì thiếu history |
| Reference vectors pass và dotnet test không discover tests | Chỉ reference đạt; runtime chưa đạt, không đóng FND-04 |
| Lập prototype nhưng chưa có tester | Giao protocol/hypotheses, không bịa playtest report hoặc mở cả slice |
| Time.deltaTime lọt vào Domain simulation | Đối chiếu WorldTick contract; không coi quy tắc Unity real-time là baseline kernel |
| Author event khi compiler chưa có | Ghi reference/prototype data và missing gate; không tuyên bố compiled/runtime-ready |

Cần tiếp tục quan sát hiệu quả trong task thật. Validator và static review không chứng minh mô hình sẽ luôn chọn và thực hiện skill đúng.
