# Client Target & Playable UX Baseline

Prototype specification theo ADR-008; nguồn M07/23/28. Phạm vi desktop offline, keyboard/mouse, tiếng Việt, 1280×720 tối thiểu và 1920×1080 mục tiêu kiểm đọc. Không có touch/controller acceptance ở slice đầu. .NET Standard2.1 core giữ độc lập. Editor6000.6.7f1 có kernel conformance theo ADR-019; client/Player, provider và release platform cần evidence riêng, không suy từ Editor profile. Linux là môi trường phát triển hiện tại.

## Định hướng mỹ thuật đã chốt — 03/11/2026

Sprint13 được chủ dự án chốt với profile **`earth-monument-2d-v1` — Đất dựng thành, nền 2D có cảm giác khối + nhân vật sprite**. [Baseline và G13](sprints/sprint-13/ART-DIRECTION-BASELINE.md), [ADR-022](../adr/022-earth-monument-2d-and-reopen-gate.md) và [approval register](sprints/sprint-13/APPROVALS.md) xác định phạm vi. Camera theo cảnh cố định với giới hạn dịch/zoom được thiết kế; nền, lớp che khuất và sprite độc lập phải cùng giữ điểm đặt chân, tỷ lệ gần–xa và độ đọc. Thông số camera, số hướng sprite, animation, surface/tầng và quy tắc picking còn cần task/contract cụ thể trước implementation, không tự lấy từ PNG hoặc physics.

G13 ở trạng thái **Accepted / Reopenable**: có thể thử hướng mới và thay lớp trình bày bằng profile có phiên bản, giữ v1/thư viện và các thành quả engine, rules, content, stable IDs cùng save contracts. Không rollback toàn repository về một sprint cũ. Render vị trí/animation không được tự ghi world state hoặc quyết định thời gian/hậu quả; presenter dùng actor-scoped projection và gửi command qua Application như các screen contract dưới đây.

Chốt hướng không là nghiệm thu layout, typography, asset licenses, animation quality hoặc human playtest. **S13-A02/A03** được theo dõi như việc chi tiết UI/Reveal còn cần triển khai và kiểm chứng; không gán Approved cho từng màn hình. Mẫu công trường chỉ là nguồn ngôn ngữ hình ảnh: demo sửa thuyền vẫn tại Bến Vạc, Tam Giang, không đổi địa lý/canon hay gameplay thành xây thành.

## Screen states và transitions

| Screen/state | Input và dữ liệu hiển thị | Chuyển tiếp/lỗi |
|---|---|---|
| Main menu | New campaign, Continue, Load, Settings; save metadata không secrets | Continue chỉ bật với save compatible; corrupt/incompatible dẫn lỗi có backup path |
| Generating | progress theo generator stage; cancel trước commit | thành công commit world rồi Reveal; failure giữ menu, không save nửa world |
| Reveal | identity, household và biography nhân vật biết; aptitude/skills đã được lộ hợp lệ | một nhân vật/campaign, không chỉnh origin/stat/class, không nút reroll; Begin → World |
| World | header ngày/giờ/vị trí; map trung tâm; observed entities; panel party; action tray | chỉ query actor-scoped projection, không raw world truth |
| Action preview | target visible, prerequisites known, cost/time, known risks và uncertainty | Confirm gửi command với revision preview; Cancel không RNG/mutation |
| Action pending | disable duplicate submit, giữ command_id cho retry | receipt success refresh projection; stale yêu cầu preview lại với ID mới; timeout lookup receipt |
| Journal | facts/claims có source/time/confidence; active threads; chronicle | rumor không trình bày như truth; search áp filter trước query result |
| Save/load | slot, revision, date, compatibility, save progress | chỉ báo Saved sau durable write; lỗi không thoát mất session |
| Death/succession | cause được biết, hậu quả, danh sách successor hợp lệ và quyền/knowledge transfer | không successor → campaign ended; chọn hợp lệ → reveal nhân vật mới qua Legacy command |

Prototype không có reroll tại Reveal; người chơi vẫn có thể bắt đầu campaign mới từ menu. Không triển khai anti-restart hoặc monetized reroll. Seed/debug tools chỉ ở developer mode tách UI release. Class không chọn ở onboarding.

## Wireframe bằng bố cục — đề xuất chưa nghiệm thu chi tiết

Bố cục sau là prototype specification để kiểm; quyết định mỹ thuật03/11 không phải user approval cho các tỷ lệ này. World desktop: header full width (time/location/save state); cột trái 22% party và needs; giữa 52% map/zone; phải26% observations/action detail; chân màn journal/command result. Ở1280px panel phụ có thể collapse; phím Tab chuyển focus theo reading order; Esc đóng overlay trên cùng, không tự hủy committed activity. Không cho input đi xuyên modal. Text scale100/125/150%; layout scroll khi overflow, không cắt chi phí hay Confirm.

Projection fields: visible entity id/display_name/location_label, perceived_condition, claim/source/confidence, allowed_actions và known_constraints. Hidden actor plans, private inventories, omniscient truth, exact hidden probability không có trong DTO. Unknown hiện “chưa rõ”; không lấy số thật rồi thay text.

## Acceptance và asset gates

Playtest phải làm được New→Reveal→World→Preview→Repair→Advance→Result→Save→Load và Death→Succession chỉ bằng keyboard. Focus visible, không chỉ màu để phân biệt injury/rumor/rejection; âm thanh không là kênh duy nhất thông báo. Confirm nêu cost/time trước commit; stale preview không âm thầm đổi giá. Combat zone+slots đọc được ở grayscale. Đường hình ảnh hiện hành đã chốt là nền2D có khối + sprite theo ADR-022; asset source/licensing, audio budget/font licenses, accessibility, sprite/contact/camera quality và platform release còn gates OPEN-11/04 trước production. Placeholder hoặc PNG preview chưa là asset/gameplay acceptance. Whitepaper là tài liệu giới thiệu, không đại diện UI playable đã có.
