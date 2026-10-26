# Sprint12 — Điều kiện xuất hiện tình huống ở Bến Vạc

## Tổng hợp dễ hiểu

Bổ sung tám tình huống từ thiết kế vào dữ liệu có thể kiểm tra bằng code: hạn vận tải, thuyền hỏng, mưa, thiếu lương thực, tin đồn, tranh chấp bến, thương tích và nhu cầu thuê sửa. Chỉ tình huống đủ điều kiện vật chất và hiểu biết của nhân vật mới trở thành ứng viên. Sprint này chưa làm các hành động diễn ra hoặc tạo game chơi thử.

SPIN-015; VS02B; implementation/review/full runtime gate đạt; Git/CI closure ghi WORKLOG. [Lộ trình demo](../PLAYABLE-ROADMAP.md) bổ sung forecast hình ảnh Sprint13 và demo nhỏ Sprint15–16; không tự mở các sprint đó.

| Task | Actor và ownership | Acceptance |
|---|---|---|
| S12-01 | /root: contract, host, tích hợp, tài liệu, Git | Scope rõ; CLI và evidence thật; PR vào dev sau checks/review |
| S12-02 | /root/s12_content: templates/schema/fixtures | Tám tình huống có nguồn, localization, điều kiện đúng và fixtures đủ/không đủ |
| S12-03 | /root/s12_eligibility: portable compiler/evaluator/tests | Strict allowlist, immutable, hash ổn định, Unknown fail closed, không mutation/RNG |
| S12-04 | /root/s12_review: review độc lập | Đối chiếu source/content/contracts và kiểm lỗi trước merge |

Inputs: settlement registry Sprint11 và event pack, actor-scoped context do adapter/caller cung cấp. Output: registry bất biến và kết quả eligible/ineligible/unknown cho công cụ phát triển. Không trả raw truth cho UI; adapter authoritative world/perception chưa nằm trong sprint.

Non-goals: thực thi event effects, NPC autonomy, protagonist generator, Unity UI, toàn bộ VS02. Cooldown tham chiếu thời điểm occurrence đã commit do caller cung cấp, đánh giá candidate không tự ghi cooldown. Unknown không được biến thành true chỉ bằng Not.

## Kết quả

[Báo cáo evidence](../../qa/SPRINT-12-EVIDENCE.md):449/449 tests,78VS02B mới;8templates/23fixtures,20process checks và review độc lập đạt. FullVS02 vẫn partial; cần world/perception binding, effects và random protagonist. Sprint13 dự kiến kết hợp campaign prototype với định hướng hình ảnh, chỉ mở khi được giao.
