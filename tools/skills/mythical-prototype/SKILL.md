---
name: mythical-prototype
description: "Plan or build bounded mythical_dnd gameplay/UX prototypes, game art direction and technical spikes, then interpret real playtest evidence. Use to test random-origin engagement, non-combat choices or causal-world readability; not to implement the entire vertical slice automatically."
---

# Prototype Mythical

## Phạm vi và nguồn chuẩn

Chỉ áp dụng cho repository mythical_dnd / Hồng Bàng — Mạch Lạc. Xác định project root từ working directory và kiểm có `.agent`, `MythicalDnd.sln`, `docs/governance/APPLICABLE-BASELINE.md`; không áp dụng vào repo khác vì tên file giống nhau. Nếu được gọi ngoài project và chưa biết đường dẫn, hỏi đường dẫn trước khi sửa. Các đường dẫn bên dưới tính từ project root, không từ thư mục skill hay thư mục cài symlink.

Đọc `AGENTS.md`, `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` khi bắt đầu phiên hoặc khi chúng thay đổi; tái dùng context còn hiệu lực. Chỉ đọc module/contract liên quan, không nạp toàn bộ GDD mỗi task. Yêu cầu hiện tại của người dùng có ưu tiên; skill không tự cấp quyền mở rộng scope, triển khai game, cài công cụ hoặc chạy multi-agent. Không thêm bước xin duyệt cho việc đã được cho phép.

## Chọn câu hỏi và loại thử nghiệm

Dựa vào `docs/production/VERTICAL-SLICE.md` và `CLIENT-UX-BASELINE.md`, chọn câu hỏi quyết định được bước tiếp theo. Nếu user yêu cầu prototype Bến Vạc, phạm vi khởi đầu gợi ý là thuyền hỏng, deadline giao hàng, vài NPC và lựa chọn can thiệp/không can thiệp; không mặc định phải làm đủ ba nghề/combat/legacy.

Phân biệt:
- Gameplay/UX prototype: kiểm người chơi hiểu tình huống, đánh đổi và hậu quả; có thể dùng paper/UI đơn giản hoặc client placeholder.
- Technical spike: kiểm codec, replay, provider, Editor/DLL; đo tính khả thi, không kết luận game hấp dẫn.
- Vertical slice: bản tích hợp đại diện đạt quality bar, cần evidence rộng hơn; không gọi một mock là slice đã đạt.

Chốt hypothesis, phạm vi, điều không làm, cách quan sát, tiêu chí tiếp tục/sửa/thu hẹp và effort cap phù hợp nguồn lực. Không áp cứng 1 ngày hoặc ép hủy ý tưởng sau số lần lặp cố định. Đã có GDD không đồng nghĩa không cần prototype. Không bắt hoàn tất FND-01–16 trước khi thử một câu hỏi trải nghiệm độc lập.

## Làm và kiểm chứng

Chọn đường triển khai ít tốn nhất vẫn trả lời được câu hỏi. Dữ liệu mock/stub phải visible trong báo cáo; không dùng chúng chứng minh NPC autonomy, conservation hoặc persistence. Random-origin và no-runtime-LLM vẫn là product baseline, trừ khi user chủ động yêu cầu thử phương án khác và được ghi là experiment.

Prototype tách khỏi production build, có scope/assumptions ngắn tại thư mục thử nghiệm nếu tạo code. Có thể tái dùng pure core đã kiểm chứng; production không import code thử nghiệm chưa được nghiệm thu. Sau kết quả tốt, quyết định giữ/refactor/viết lại dựa chất lượng và boundaries, không bắt bỏ code vì nhãn prototype.

Khi user chỉ yêu cầu plan, trả protocol, không tự build. Khi được giao build, làm đến mức có thể chạy/quan sát; ghi setup/run steps và limitations. Chỉ dùng Unity/Sites/image tools khi thực sự cần và có sẵn; skill này không ngầm cài editor/MCP.

## Playtest và quyết định

Mỗi session cần build/version, tester thực tế, tình huống, hành vi quan sát, câu hỏi sau chơi, bugs và điểm chưa rõ. Tách lời người chơi, quan sát và diễn giải của agent. Đề nghị tình huống so sánh (can thiệp/không can thiệp; thiếu/thừa thông tin) khi phù hợp hypothesis. Bot run không thay human playtest.

Không có tester: báo chưa kiểm trải nghiệm và đưa protocol; không bịa tỷ lệ hài lòng. Có evidence: kết luận nên tiếp tục/sửa/thu hẹp/chưa đủ dữ liệu và task nhỏ tiếp theo. Nếu đổi baseline, cập nhật các consumer bị ảnh hưởng; không tự triển khai toàn roadmap hoặc rút gọn nghiên cứu gốc thành prototype spec.

Ghi hypothesis/goal, actor thực hiện, scope, observations và quyết định sau thử nghiệm vào `WORKLOG.md` theo spin hiện tại. Chỉ append sự kiện thật; thiếu playtest thì ghi chưa có evidence, không tạo phản hồi mẫu như kết quả.

## Thiết kế hình ảnh trong prototype

Khi task cần visual direction, concept scene hoặc UI game, đọc [Game Art Direction](references/game-art-direction.md). Tạo artifact để so sánh theo mục tiêu trải nghiệm; không thay hình ảnh bằng mô tả. Dùng approval register hiện hành; technical review không thay user approval. Nguồn adaptation và license ở [Design Sources](../upstream/DESIGN-SOURCES.md).
