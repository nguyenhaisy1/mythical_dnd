# Client Target & Playable UX Baseline

Prototype specification theo ADR-008; nguồn M07/23/28. Phạm vi desktop offline, keyboard/mouse, tiếng Việt, 1280×720 tối thiểu và 1920×1080 mục tiêu kiểm đọc. Không có touch/controller acceptance ở slice đầu. .NET Standard2.1 core giữ độc lập. Chọn Unity LTS còn được hỗ trợ tại thời điểm spike; exact patch, Mono/IL2CPP và OS runtime ghi từ Editor thật trong FND-16. Linux là môi trường phát triển hiện tại; release platform chưa được xem là verified.

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

## Wireframe bằng bố cục

World desktop: header full width (time/location/save state); cột trái 22% party và needs; giữa 52% map/zone; phải26% observations/action detail; chân màn journal/command result. Ở1280px panel phụ có thể collapse; phím Tab chuyển focus theo reading order; Esc đóng overlay trên cùng, không tự hủy committed activity. Không cho input đi xuyên modal. Text scale100/125/150%; layout scroll khi overflow, không cắt chi phí hay Confirm.

Projection fields: visible entity id/display_name/location_label, perceived_condition, claim/source/confidence, allowed_actions và known_constraints. Hidden actor plans, private inventories, omniscient truth, exact hidden probability không có trong DTO. Unknown hiện “chưa rõ”; không lấy số thật rồi thay text.

## Acceptance và asset gates

Playtest phải làm được New→Reveal→World→Preview→Repair→Advance→Result→Save→Load và Death→Succession chỉ bằng keyboard. Focus visible, không chỉ màu để phân biệt injury/rumor/rejection; âm thanh không là kênh duy nhất thông báo. Confirm nêu cost/time trước commit; stale preview không âm thầm đổi giá. Combat zone+slots đọc được ở grayscale. Art prototype dùng placeholder 2D; final2D/2.5D, audio budget/font licenses và platform release còn OPEN-11/04, chốt trước asset production. Whitepaper là tài liệu giới thiệu, không đại diện UI playable đã có.
