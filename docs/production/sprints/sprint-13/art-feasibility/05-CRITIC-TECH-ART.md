# Phản biện độc lập — kỹ thuật, mỹ thuật và phạm vi ước lượng

02/11/2026 · Sprint 13 / SPIN-036 · Actor `/root/art_feas_critic_tech`.

Đã mở trực tiếp đúng ba PNG và đọc ba báo cáo worker. Kết luận: đề xuất dùng chung thế giới/nhân vật 3D cách điệu với camera giới hạn là **hợp lý để đem đi kiểm chứng**. Chưa có bằng chứng để gọi pipeline usable, hiệu năng đạt hoặc lịch hoàn thành được bảo đảm. Không có finding P0/P1 trong cách ba báo cáo đang phân biệt đề xuất với kết quả thực; các điều kiện triển khai và clarification dưới đây phải được giữ khi tổng hợp.

Đây là báo cáo của **một critic độc lập**. Ở lượt đầu, critic estimate riêng gặp usage limit và chưa có report; phần kiểm số bổ sung dưới đây vẫn do cùng actor này thực hiện, không được gọi thành hai lượt review độc lập. Khi chốt, root thông báo critic estimate đã tiếp tục chạy và tạo report riêng06; báo cáo này không nhận phần việc đó là của mình hoặc claim đã đọc06. Root tổng hợp/self-review là lớp khác, không biến thành critic thứ hai.

## Phạm vi thực sự đã quan sát

- Đọc `.agent`, `docs/production/HANDOFF.md`, `docs/governance/APPLICABLE-BASELINE.md`, skill `mythical-validation` và reference game-design-review.
- Đọc trọn [01 — Kiến trúc](01-ARCHITECTURE.md), [02 — Pipeline mỹ thuật](02-ART-PIPELINE.md), [03 — Effort](03-EFFORT-MODEL.md); đối chiếu [client README](../../../../../client/UnityProject/README.md), project targets và [roadmap](../../../PLAYABLE-ROADMAP.md).
- Mở [Đất dựng thành](../../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png), [Hành lang thành đất](../../../../../prototypes/sprint13/experiment-17/assets/earth-corridor.png), [Đường chéo dài](../../../../../prototypes/sprint13/experiment-18/assets/long-diagonal.png) bằng image viewer, không chỉ đọc tên/prompt.
- Tự liệt kê client: file không bị ignore chỉ có README; các thư viện Foundation/Domain/Application/Infrastructure target `netstandard2.1`, host target `net10.0`. Không tìm thấy cơ sở để gọi client bootstrap hoặc playable client đã xong.
- Tính lại WBS và `ceil(E/Q)` bằng Python độc lập từ từng khoản trong report03; không dùng số runtime/benchmark giả.
- Chưa chạy Unity, tạo model, kiểm OAuth/credits/asset Hyper3D, đo GPU, thử rig/clip, benchmark model hoặc kiểm dashboard quota. Thông tin máy và metadata công cụ trong ba báo cáo là root kiểm kê, không là phép đo riêng của critic.

## Findings và yêu cầu giữ khi tổng hợp

### P2 — nối bối cảnh art proof với demo sửa thuyền phải được nói rõ

**Bằng chứng:** M1 của ba hướng bám công trường/thành đất hoặc cầu giữa vách. M2 của report03 lại dùng vòng chơi nhận nhân vật → quan sát → sửa thuyền hoặc chờ tại profile demo đã được roadmap định hướng. Chuyển art style không tự chuyển geography hoặc nghề nghiệp.

**Tác động:** Nếu người đọc hiểu M2 là tái tạo nguyên cảnh thành đất rồi sửa thuyền tại đó, ước lượng có thể che phần môi trường bến/thuyền hoặc âm thầm đổi canon/gameplay. Không thể lấy một bridge kit đã xong để coi toàn scene bến đã xong.

**Hướng xử lý:** Báo cáo tổng hợp cần ghi M1 kiểm ngôn ngữ/camera từ ba PNG, còn M2 áp dụng ngôn ngữ đã chọn vào **Bến Vạc**; bộ 20–30 module/props phải tính cả phần bến và thuyền phù hợp. Nếu muốn demo tại đúng công trường hoặc cầu-vách trong PNG, cần scope/contract riêng và re-estimate; không tự dời địa danh hoặc thay repair bằng xây thành. Đây là clarification phạm vi, không bác bỏ route3D.

### P2 — calendar là phép quy đổi năng lực, chưa là forecast có đường tuần tự đã đo

**Bằng chứng:** Report03 ghi đúng công thức có `C/S` và waits nhưng bảng ngày mới tính `ceil(E/Q)`. Chưa có số đo Q, critical-path capacity hoặc asset acceptance rate.

**Tác động:** Cắt bảng ngày ra khỏi chú thích dễ biến 35–71 ngày thành lời hứa trên một tài khoản Plus. Band effort còn có thể bị vượt nếu asset/rig gate thất bại nhiều lần; đầu cao không phải worst-case được bảo đảm.

**Hướng xử lý:** Giữ nhãn “ngày có năng lực giả định, chưa cộng phần phụ thuộc/chờ” ngay sát bảng tổng hợp; không chỉ để cuối phụ lục. Không mặc định 7 ngày/tuần, 8 giờ/day entitlement hoặc chia tổng công cho sáu actor. M0 là cap nghiên cứu, có thể kết thúc chưa usable; sau đó mới re-estimate.

### P2 — proof Đất dựng thành phải giữ đúng điều kiện fragment

**Bằng chứng ảnh14:** Có ít nhất ba người cận và nhiều nhóm xa trên nhiều tầng; quy mô/crowd là phần quan trọng của cảm giác ảnh. M1 dự toán chỉ hai actor tiền cảnh và một mảng đại diện.

**Tác động:** Dựng tường đỏ nhỏ có thể kiểm asset nhưng chưa chứng minh toàn giá trị panorama hoặc crowd sống. Bands ba hướng gần nhau chỉ hợp scope được thu nhỏ, không chứng minh full Earth scene gần effort hai hướng còn lại.

**Hướng xử lý:** Gắn “fragment hai actor, chưa full panorama/crowd” vào hàng Earth của bảng kết luận. Một cảnh scale/crowd khác cần ngân sách và acceptance riêng. Báo cáo01–03 đã nêu điều này; không làm mất khi rút gọn.

### P2 — việc giản lược Hành lang là quyết định hình ảnh thật

**Bằng chứng ảnh17:** Mặt người già, tóc, bàn tay chỉ hướng, vai/cổ người mang gỗ và vải hiện rõ. Ảnh18 nhỏ người hơn, dùng mặt khối/mảng rộng; dùng chung base rig không tự cho cùng chất lượng ở hai cự ly.

**Tác động:** Giữ Corridor như PNG17 cần polish cận, còn giản theo18 có thể mất phần user đang thích. Không nên gọi đó là tối ưu không ảnh hưởng hình ảnh.

**Hướng xử lý:** Giữ hai nhánh fidelity khi chốt scope; không tuyên bố Corridor rẻ nhất từ số mesh môi trường. Rig/clip reuse vẫn phải kiểm contact/age/portrait ở đúng screen size. Report01–03 đã sửa xếp hạng geometry-only khác total fidelity.

## Các ranh giới kỹ thuật đã được báo cáo phân biệt đúng

| Vấn đề kiểm | Kết quả phản biện |
|---|---|
| PNG và 3D | Cả ba hiện là ảnh raster. Không ảnh nào chứng minh geometry, depth, rig, scene hoặc runtime. Camera Đường chéo gợi axon nhưng không thể rút projection/tiêu cự chính xác từ PNG. Đề xuất 3D thật là route tương lai. |
| 2.5D | Camera hạn chế có thể tạo cách chơi được gọi 2.5D; không biến world/actor mesh3D thành sprite2D. Ba báo cáo không buộc dùng một nhãn để né effort asset. |
| Unity/URP/UI | Unity6000.6.7f1 đã có kernel evidence riêng; URP, Input System, uGUI/TMP và camera package vẫn là đề xuất cho client chưa bootstrap. Chọn package tương thích rồi pin manifest/lock; không tự nhận đã cài hoặc đã kiểm Player. |
| Simulation authority | UI đọc actor-scoped projection và gửi intent; vị trí mesh, animation, collider, NavMesh hoặc frame time không hoàn thành repair, tiêu RNG hay ghi world truth. Presentation surface/anchor mapping còn phải thành contract rõ trước code. |
| Hyper3D | Root thấy metadata không đủ chứng minh OAuth/credits/generation/usable mesh/rig. Child không thấy namespace cũng không chứng minh cả client disconnected. Không gọi generation chỉ để hoàn tất bài phân tích; task model thật tương lai theo MCP đã quy định. |
| Phần cứng | UHD630/i5-8400/~19GiB là kiểm kê, không benchmark. 720p/30fps là mục tiêu thử; panorama crowd, shadow/overdraw và Player path chưa đo. |
| M2/M3 và toàn game | Demo5actor cùng lúc khác full roster/simulation; style library M3 khác1.000nhân vật,12vùng và29module gameplay hoàn chỉnh. Forecast Sprint15–16 cũ không thành ngày cam kết hoặc quyền qua Sprint14. |
| Văn hóa | Thành đất, gỗ, áo/quần và mái trong PNG là diễn giải nghệ thuật; chưa là phục dựng Văn Lang–Âu Lạc được xác minh. Trước mass-production cần provenance và phân lớp dữ kiện/diễn giải/hư cấu. |

Điểm cần chốt trong task bootstrap sau này: định dạng asset nhập Unity và importer thực sự dùng; unit/pivot/skeleton/material rules; giới hạn camera/zoom và chuyển shot; chọn đúng surface khi cầu đè mặt đất; xử lý click actor bị che; Actor Perceived State cho người/đạo cụ. Không tự điền unknown bằng physics hoặc tool mặc định.

## Kiểm số độc lập của critic

Python cộng lại từ từng dòng WBS sau khi bổ sung art pack cho kết quả common M1 **68–142h**, common increment M2 **210–424h** (= gameplay/client182–364 + art28–60); các phép cộng lũy kế đều khớp report03:

| Hướng | M1 | M2 lũy kế | M3 lũy kế |
|---|---:|---:|---:|
| Đường chéo dài |86–182|320–662|456–942|
| Hành lang thành đất |96–204|330–684|474–972|
| Đất dựng thành |100–215|346–719|498–1.023|

Đã tính lại toàn bộ18cặp ngày M1/M2 với Q=4/8/12 bằng `math.ceil`, đều khớp bảng report03 sau sửa. M2 Q8 là **40–83 /42–86 /44–90ngày**, M3 Q8 là **57–118 /60–122 /63–128ngày**. Đây chỉ kiểm số học và cách dùng đơn vị, **không xác thực độ chính xác của các band ROM**, actual quota hay throughput. Band character36–80h nằm một lần trong common; Corridor polish là phần tăng chất lượng cận, không cộng lại bộ rig. Shared work chỉ tính một lần khi làm cùng nền; ba scene vẫn có effort riêng.

## Gate nên đưa vào lần hiệu chỉnh nếu sau này được giao

1. **Usable asset:** nhận model nguồn đúng đường generation, kiểm provenance/license/import; nếu không usable thì ghi lỗi và số lượt, không gọi preview đẹp là qua gate.
2. **Character/contact:** một body đi/đứng/cầm vật ở camera18 và cận17; chụp đúng checkpoint để thấy feet/hand/cloth/age/portrait. Chưa qua thì chưa nhân rộng look/clip.
3. **View interaction:** chọn đúng tầng cầu, không click xuyên actor/đạo cụ bị che; modal/focus/font Việt/scale chữ kiểm bằng UI thật.
4. **Performance:** build đúng target, ghi GPU/resolution/settings/cảnh, frame-time/draw-call/memory và chất lượng chụp; không chỉ một screenshot Editor đẹp. Chọn budget sau khi đo.
5. **M2 authority:** generator có nhân quả, repair/material/time/ownership, projection isolation và save/reopen thật; demo loop không được thay bằng animation/script giả rồi giữ tên M2.

Không chạy các gate trên trong bài review này. **Technical reviewed ở phạm vi lập luận/tài liệu và số học; runtime, asset readiness, mỹ thuật cuối và lịch cam kết chưa được nghiệm thu. Sprint13 cùng mọi phiên bản01–18 tiếp tục Pending user approval.**

## Review báo cáo tổng hợp sau khi file được tạo

Đã đọc trọn [báo cáo tổng hợp](../ART-FEASIBILITY-THREE-DIRECTIONS.md) ở lượt tiếp theo của cùng actor. Không dùng review ba worker trước đó để giả đã review một file chưa tồn tại. Bản tổng hợp đã trả lời đúng các câu hỏi user: effort, feasibility có điều kiện, thời gian theo năng lực giả định, 2D/3D, tech stack và cách triển khai từng hướng.

- M1/M2/M3 và bảng ngày Q4/8/12 khớp report03. Phép tính thử ba M1 dùng chung **146–317h** cũng đúng: common68–142 cộng ba increment18–40,28–62,32–73; bản tổng hợp đã ghi chưa tính chi phí ghép/review chéo và không phải estimate triển khai cả ba.
- Các chú thích chờ/phụ thuộc và chưa đo Q nằm ngay sát bảng và trong phần đầu; không còn tình trạng chỉ có bảng ngày lẻ gây hiểu thành lịch Plus được bảo đảm. Root phải đồng bộ M2/M3 và lịch theo WBS art pack đã bổ sung dưới đây; review số revision trước không được nhận là final số mới.
- Main report đã giữ Earth fragment/crowd, Corridor fidelity, packages đề xuất, metadata Hyper3D, GPU chưa benchmark và root self-review; không tự nhận Player/gameplay/art đã đạt. Lượt đầu chỉ có critic này có output; lượt cuối main ghi critic estimate tiếp tục được và có report riêng, giữ sự cố quota cũ.
- Critic mở độc lập nguồn primary ngày02/11: [Codex pricing](https://learn.chatgpt.com/docs/pricing) xác nhận usage phụ thuộc model/task, local/cloud chia allowance và có thể có giới hạn tuần; [GPT-6.1 Sol](https://developers.openai.com/api/docs/models/gpt-6.1-sol) liệt kê high, image input và image-generation tool; [Unity6000.6 UI comparison](https://docs.unity.com/en-us/engine/6000.6/manual/uitoolkits/ui-system-compare) liệt kê uGUI runtime recommendation và UI Toolkit alternative. Các claim giới hạn ở main report được những nguồn đó hỗ trợ; không dùng API pricing để suy subscription throughput.
- Đã gửi root clarification về M2 áp dụng style vào Bến Vạc/thuyền/bến, và đề nghị câu mở dùng “có đường triển khai khả thi về mặt kỹ thuật, còn cần qua gate” để tránh giọng khẳng định feasibility đã chứng minh. Root sở hữu phần sửa main; critic không sửa main hoặc hồ sơ khác.

Không có finding P0/P1 mới sau lượt đọc main. Phần acceptance vẫn giới hạn ở nghiên cứu/tài liệu và số học; không đổi Pending thành Approved.

## Kiểm lại sau finding root về phần mỹ thuật M2 chưa được đếm

Root phát hiện trước commit rằng scope M2 tăng từ hai lên bốn looks,3–4 lên6–8clip categories và12–18 lên20–30pieces có bến/thuyền, nhưng WBS trước chỉ ghi gameplay/binding/E2E. Critic xác nhận đây là **thiếu công sản xuất asset thật**, không được ngầm lấy từ rework hoặc gọi Unity binding là đã tạo thêm model/clip. Lượt review trước kiểm phép cộng đúng nhưng chưa phát hiện scope omission này; không coi số cũ đã được xác thực đầy đủ.

Report03 hiện bổ sung bốn khoản chung: looks6–12h; clip12–28h; UI art4–8h; import/compatibility/art acceptance6–12h. Tổng **28–60h** khớp. Phần môi trường Bến Vạc thêm riêng D18–40/C16–36/E22–48h, ngoài phần integration D6–16/C8–20/E14–32h. Critic đã đọc lại đoạn WBS sửa và tính từ các dòng, ra các tổng mới ở bảng trên.

Ranh giới tránh đếm trùng hiện đủ rõ: hai look mới dùng hai body/rig đã có; clip thêm2–5category để đạt tổng6–8, không thêm lip-sync/cloth simulation; UI art dùng library còn handler/binding tính riêng; import compatibility chỉ kiểm asset mới/variants, không lặp QA M1 hoặc E2E gameplay. Boat/dock increment sản xuất asset/state biểu hiện hỏng–sửa và bố trí bến, không chứa rule repair. Khoảng8–12unique definitions bổ sung/thay thế trong kit giới hạn hợp với trần tổng20–30; không đếm từng instance cọc/ván thành asset mới.

Các band mới vẫn là ROM chưa đo, chỉ hợp giả định bổ sung nhỏ/tái dùng được. Nếu mesh/clip phải làm lại toàn bộ, nhiều body mới, thuyền có hình học trạng thái phức tạp hoặc bến độc lập lớn thì re-estimate; không dùng đầu cao làm quỹ vô hạn. Finding về scope/WBS đã được xử lý trong report03; root chịu trách nhiệm đồng bộ main/04/roadmap/log trước commit. Không phát sinh runtime/asset readiness acceptance từ lần sửa số này.

**Đối chiếu main cuối sau sửa:** critic đã đọc lại toàn bộ báo cáo tổng hợp. Bản này đã ghi M2 về Bến Vạc/thuyền/bến, tách28–60h art chung và ba increment môi trường, đồng bộ M2/M3/bảng ngày đúng số mới. Câu feasibility đã đổi thành có đường triển khai kỹ thuật và còn asset/performance gates; M0 không cộng lại công usable vào M1. Main cũng ghi namespace Hyper3D bị gỡ sau thời điểm kiểm kê, không suy ra dịch vụ disconnected hoặc lén thay generator. Các clarification trong05 đã được giữ; không còn finding cản bàn giao **nghiên cứu** trong phạm vi critic đã kiểm. Root tiếp tục chạy checks hồ sơ/exports/Git riêng; reviewer này không nhận những phép kiểm chưa chạy là kết quả của mình.
