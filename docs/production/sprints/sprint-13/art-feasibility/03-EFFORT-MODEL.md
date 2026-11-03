# Mô hình effort và lịch cho ba hướng thành đất

02/11/2026 · SPIN-036 · Actor `/root/art_feas_estimate` · **Đề xuất chưa hiệu chỉnh; Pending user approval**.

Cả ba hướng có thể triển khai bằng cùng một nền 3D cách điệu với camera giới hạn. Phần làm chúng tốn công khác nhau nằm ở quy mô nhìn thấy, khoảng cách tới người, số lớp đường/sàn và mức chất lượng cần giữ khi chuyển động. Khoảng công việc dùng chung cho nhân vật, UI và vòng chơi lớn hơn chênh lệch giữa ba cảnh. Vì vậy không nên hiểu ba ảnh là ba engine, cũng không cộng ba tổng dự toán để suy chi phí làm một game.

Các số dưới đây là **ngân sách lập kế hoạch do AI đề xuất**, chưa phải số đo tốc độ GPT-6.1 Sol high, chi phí thực của một lượt Hyper3D hay lịch cam kết của Codex Plus. Độ tin cậy hiện thấp; cần kiểm chứng một chuỗi asset và một đoạn tích hợp trước khi chốt ngày.

## Bằng chứng đã xem và điểm xuất phát

Reviewer đã mở trực tiếp đúng ba PNG: [Đất dựng thành](../../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png), [Hành lang thành đất](../../../../../prototypes/sprint13/experiment-17/assets/earth-corridor.png), [Đường chéo dài](../../../../../prototypes/sprint13/experiment-18/assets/long-diagonal.png). Đây là ảnh mô phỏng, chưa có model, rig, clip, collision hoặc scene tương ứng để tái sử dụng ngay.

[Client hiện tại](../../../../../client/UnityProject/README.md) vẫn là layout README, chưa có ProjectVersion/Packages/scene của playable client. Kernel conformance đã có không chứng minh client chơi được. Characters và actor projection trong engine còn nhiều README; các primitives/command/scheduler/persistence đã có phải được tận dụng, song không được coi generator cuộc đời, repair domain hoặc projection cho game đã hoàn tất. [Roadmap](../../../PLAYABLE-ROADMAP.md) ghi demo nhỏ còn các gate riêng và Sprint13 chưa được chủ dự án chốt.

| Ảnh | Quan sát ảnh thật ảnh hưởng effort |
|---|---|
| Đất dựng thành | Panorama công trường rộng, nhiều người ở xa, vách/hào và cầu đi qua nhiều cao độ. Chi phí lớn khi giữ mật độ sống, che khuất và chọn đúng tầng; không phải chỉ xây một bức tường đỏ. |
| Hành lang thành đất | Ít khối kiến trúc chính, hai người ở gần và mặt/ngón tay/vật mang nhìn rõ. Geometry môi trường rẻ hơn nhưng lỗi rig, mặt, tay và clipping vải bị lộ mạnh hơn. |
| Đường chéo dài | Cầu, trụ và vách lặp được theo module; người nhỏ hơn trong khung, bối cảnh tiết chế. Dễ giới hạn art proof, nhưng ray picking/đường đi theo cao độ và mép camera vẫn phải kiểm. |

## Định nghĩa effort, năng lực theo ngày và phần chờ

Một **giờ công AI** ở tài liệu này là một giờ một tác nhân thực sự phân tích, viết/sửa, điều khiển công cụ, kiểm kết quả hoặc sửa lỗi trong phạm vi tác vụ. Nếu ba tác nhân cùng hoạt động một giờ thì tổng là ba giờ công AI. Đây là đơn vị kế hoạch; chưa có hệ đo ghi nhận đủ các giờ ấy cho pipeline này.

Không tính thời gian hàng đợi dịch vụ, render/import không cần giám sát hoặc chờ chủ dự án thành công AI hoạt động. Ghi chúng riêng thành thời gian chờ. Công đã dành cho review và sửa kết quả không biến mất vì cùng dùng một model. Các bảng đã có khoảng cho sửa trong từng work package và kiểm tổng thể; không cộng thêm một hệ số rework tùy ý lên tổng lần nữa.

User nêu cấu hình **Codex Plus + GPT-6.1 Sol high**; đây là cấu hình giả định cần đánh giá, không là tuyên bố rằng mọi actor vừa chạy đã được benchmark trong cấu hình ấy. Root đã kiểm official docs về model/limits. Không có dashboard quota thực của tài khoản trong đánh giá này. Các giá trị 4/8/12 giờ công AI hữu dụng mỗi ngày bên dưới chỉ là kịch bản năng lực tổng hợp, không quyền lợi mà Plus bảo đảm. Bốn slot đồng thời trong phiên cũng không thành bốn hay sáu gói quota độc lập.

## Các mốc có cùng định nghĩa để so sánh

| Mốc | Phạm vi giả định để lập kế hoạch | Điều chưa được nhận là hoàn tất |
|---|---|---|
| M0 — hiệu chỉnh | Work cap 12–24 giờ công AI, kiểm một module môi trường và một chuỗi character import/rig/pose; camera/UI tối thiểu | Không hứa có hai nhân vật đẹp, game20phút hoặc một scene đạt chất lượng ngay trong cap. Nếu asset gate chưa qua thì báo phần chưa làm được và cập nhật dự toán. |
| M1 — art proof | Một cảnh nhỏ, hai actor tiền cảnh có hai dáng người, 3–4 clip ngắn dùng chung, một vật cầm, portrait/UI đọc được, camera có giới hạn và bản chạy xem được | Không có repair/random-origin/save gameplay acceptance; panorama/crowd trong ảnh14 chỉ được kiểm bố cục ở mức giới hạn, chưa toàn công trường sống. |
| M2 — demo chơi thử nhỏ | Tái sử dụng M1; khoảng20phút trải nghiệm mục tiêu, một địa bàn giới hạn, một PC cộng tối đa4NPC hoạt động nhìn thấy; hai body archetype/four looks, 6–8 clip dùng chung và khoảng20–30 module/props. Luồng nhận nhân vật ngẫu nhiên có nhân quả trong profile giới hạn → quan sát actor-scoped → sửa thuyền hoặc chờ → hậu quả → save/load | Không gọi chọn ngẫu nhiên một hero sẵn là full life generator. Không đủ vertical slice ba nghề/combat/legacy, chưa NPC autonomy toàn chiến dịch. “20phút” là cap nội dung cần playtest, không thời lượng vui chơi đã đo. |
| M3 — bộ style có thể mở rộng | Tái sử dụng M2; đề xuất3–4 dáng body,12–20look có provenance,10–14clip có retarget,40–60module/props, quy ước material/camera/UI, validation và hướng dẫn sản xuất | Không đồng nghĩa1.000nhân vật có ý nghĩa,12vùng, gameplay/campaign/content đã hoàn thiện. |

Các số asset là trần giả định để so sánh. Một module được đếm là một asset độc lập có thể tái dùng; không đếm từng bản sao cột/ván thành một asset mới. Bốn looks không tự là bốn rig. Bản art proof có thể dùng mock actors/pose để kiểm hình; M2 phải thay bằng projections từ trạng thái thật theo baseline và phải có acceptance riêng.

Root đã kiểm kê máy hiện tại: i5-8400, khoảng19GiB RAM và Intel UHD630. Chưa có đo URP/Player trên cảnh này. Đề xuất kiểm trước ở720p với30fps là mục tiêu tạm để tìm cấu hình, không Passed hoặc cam kết hiệu năng. Capture tham chiếu độ phân giải cao hơn cần được phân biệt với tốc độ game đang chạy. Các bands effort không được hiểu là đã chứng minh phần cứng đủ cho panorama/crowd ảnh14.

## WBS M1 — art proof

Phần chung được làm một lần nếu so cả ba hướng bằng cùng thân người/rig/material/UI contract.

| Gói dùng chung | Giờ công AI thấp–cao | Đầu ra và giới hạn |
|---|---:|---|
| Chốt scope, quy tắc hình, tham chiếu văn hóa và asset contract | 6–12 | Camera/scale, target chất lượng, source/provenance; chưa khảo cổ toàn campaign |
| Unity client/URP bootstrap, camera và scene harness | 8–16 | Project/pipeline thực, render loop/input xem scene; package cần kiểm đúng version |
| Hai actor usable, material, rig,3–4clip và tiếp xúc vật cầm | 36–80 | Bao gồm import/kiểm mesh, sửa/retarget, clipping và contact cơ bản; không giả model sinh ra sẵn production-ready |
| Portrait/icon/UI mẫu | 10–18 | Portrait khớp actor, typography Việt và hai hành động minh họa; không handler gameplay |
| Build, capture, kiểm hình và vòng sửa tổng thể | 8–16 | Build thật và evidence; chưa benchmark/full accessibility acceptance |
| **Tổng dùng chung M1** | **68–142** | Không cộng lại phần này cho mỗi ảnh khi dùng chung nền |

| Phần riêng M1 | Environment/layout | Đặc thù thêm | Tổng riêng | M1 tổng cộng |
|---|---:|---:|---:|---:|
| Đường chéo dài | 12–28 | 6–12 cho height/picking/camera-edge proof | **18–40** | **86–182** |
| Hành lang thành đất | 8–18 | 16–36 cho mặt/tay/vật cầm ở cự ly gần;4–8 cho camera/occlusion | **28–62** | **96–204** |
| Đất dựng thành | 20–45 | 12–28 cho bố cục cao độ/che khuất và cảm giác quy mô | **32–73** | **100–215** |

M1 của Đất dựng thành là một đoạn có thể kiểm, không tái tạo nguyên panorama với mọi người đang lao động. Nếu yêu cầu bằng đúng mật độ ảnh gốc hoặc tương tác mọi cao độ thì phải tăng actor/clip/content/occlusion budget và tính lại. Một art proof dừng ở hình tĩnh hoặc không rig cần dự toán nhỏ hơn riêng; không dùng nó để nhận M1 có chuyển động ở bảng này đã xong.

Khoản36–80h character đã nằm trong tổng chung68–142h; các khoản environment cũng đã nằm trong phần riêng từng hướng. Không cộng lại khi dùng số tổng. Phụ phí mặt/tay của Hành lang là polish để chịu được cự ly ảnh17, không một cặp rig thứ hai hoặc lip-sync mới. Shared6–8clip của M2 có thể cần12–16lần kiểm tương thích trên hai rig mapping, không tự thành12–16clip asset độc lập. QA chung kiểm import/rig/style; phần riêng kiểm camera/cao độ/contact theo hướng, tránh dùng cùng một việc hai lần.

## WBS M2 — phần cộng thêm để có gameplay thật

M2 là **tổng lũy kế từ hiện trạng hiện nay**, không phần cộng vào M1 lần thứ hai. Các con số dưới đây không viết lại kernel từ đầu; chúng gồm sản xuất mỹ thuật bổ sung và khoản nối/triển khai những phần gameplay, client còn thiếu. M1 có khoảng12–18unique environment/props; M2 đạt tổng20–30 trong kit được chọn, không thêm20–30 lên trên kit M1.

| Gói cộng thêm chung | Giờ công AI thấp–cao | Gate phải qua |
|---|---:|---|
| Hai look bổ sung trên hai body/rig sẵn | 6–12 | Đủ tổng4looks, gồm trang phục/tóc/material và kiểm silhouette, không thêm hai body/rig mới |
| Clip bổ sung để từ3–4 lên tổng6–8categories | 12–28 | Khoảng2–5clip còn thiếu tùy scope, không mặc định source clip đã usable; không thêm lip-sync/cloth simulation |
| UI art bổ sung cho origin reveal, repair/outcome và save/load | 4–8 | Icon/portrait/state presentation dùng lại library; handler/binding ở gói Unity riêng bên dưới |
| Import/retarget/look-clip compatibility và art acceptance bổ sung | 6–12 | Kiểm asset mới/variants trên hai rig, không lặp toàn bộ QA M1; không chứa gameplay E2E |
| Chốt profile character demo, generator có nhân quả, viability/reveal và tests | 30–60 | Không chọn hero/reroll; unknown phải thành design gap/contract trước code |
| Actor-scoped read model/projection, chọn mục tiêu và thông tin biết được | 30–60 | UI không giữ omniscient snapshot rồi tự giấu |
| Repair/wait/time/material/ownership outcome và tests | 35–70 | Rule thật, command/atomic commit, cancel/stale/thiếu tài nguyên |
| Bind Unity input/UI/actor animation/feedback tới command/query | 32–64 | Hình không tự ghi authoritative state; keyboard và duplicate-submit |
| Save/load client integration và hồi phục trạng thái hiển thị | 20–40 | Tận dụng persistence có sẵn nhưng kiểm provider/Unity path thật |
| End-to-end checks, sửa lỗi, đóng phạm vi nội dung/build bàn giao | 35–70 | Luồng chạy từ seed đến save/load; không bỏ qua failed/skipped |
| **Tổng cộng thêm chung M2** | **210–424** | Art expansion28–60 + gameplay/client182–364; test/review có ranh giới từng gói, không nhân thêm rework mặc định |

| Hướng | Art môi trường Bến Vạc bổ sung | Tích hợp/kiểm đặc thù M2 | **M2 tổng lũy kế** |
|---|---:|---:|---:|
| Đường chéo dài | 18–40 | 6–16 | **320–662 giờ công AI** |
| Hành lang thành đất | 16–36 | 8–20 | **330–684 giờ công AI** |
| Đất dựng thành | 22–48 | 14–32 | **346–719 giờ công AI** |

Art môi trường Bến Vạc gồm asset mới và chuyển ngôn ngữ hình sang bến sửa thuyền: một thuyền với biểu hiện hỏng/đã sửa theo state, module cầu/bờ bến, vật liệu/công cụ repair và bố trí cảnh đủ vòng chơi. Dự kiến khoảng8–12unique definitions bổ sung hoặc thay asset không hợp bối cảnh để tổng kit demo giữ20–30; water/lighting/material và collision-prefab art phải cùng kiểm. Không coi tường/cầu công trường M1 là toàn bộ bến/thuyền đã có. Band Đường chéo giữ khối/chi tiết tiết chế; Hành lang tận dụng camera gần nhưng kiểm prop ở cự ly ấy; Đất dựng thành cần điều chỉnh nhịp khối/cao độ. Nếu cần một bộ bến hoàn toàn độc lập hoặc thêm nhiều thuyền/công trình/state hình học thì định lượng lại thay vì ép vào trần này.

Tích hợp/kiểm đặc thù riêng là đường/click theo tầng, contact và feedback ở cự ly dùng thật hoặc visibility theo chiều cao. Gói art mới không chứa logic repair; gói Unity binding không sản xuất thêm clip/model; E2E không làm quỹ mỹ thuật ẩn. M2 vẫn giới hạn5actor hoạt động nhìn thấy; chưa cộng production crowd của cả công trường. Các khoảng chồng nhau rất nhiều: đây là tín hiệu phần chung và các unknown đang chi phối, không căn cứ tuyên bố một hướng chắc chắn nhanh hơn đúng vài ngày.

Nếu chốt profile demo nhỏ hơn hoặc bỏ gameplay để chỉ xem art, bỏ các gói tương ứng và đổi tên mốc. Nếu repair/origin/projection nhận thêm yêu cầu ngoài profile đã chốt, tăng scope rồi re-estimate; không âm thầm ép unknown thành một script để giữ lịch đẹp.

## M3 và phạm vi toàn game

Ngân sách sơ bộ cho M3 có thể cộng **120–240 giờ công AI dùng chung** vào M2 cho mở rộng kit/variants/retarget/style checks, cộng phần riêng Đường chéo16–40, Hành lang24–48, Đất dựng thành32–64. Tổng lũy kế tương ứng **456–942 / 474–972 / 498–1.023 giờ công AI**. Các khoản M3 là phần mở rộng sau khi đã có đủ asset M2, không làm lại bến/thuyền, looks hoặc clip đã được tính. Đây là ROM gộp, chưa có WBS chi tiết cho thêm body/look, clip/retarget, environment và hướng dẫn/validation; phải chia gói và hiệu chỉnh trước khi cam kết lịch M3. Band này chỉ cho bộ style giới hạn đã định nghĩa, độ tin cậy thấp hơn M2; không là ước tính hoàn thiện game.

Chưa có cơ sở để quy ba ảnh thành ngày hoàn thành game12vùng/1.000nhân vật. Cần định lượng số settlement/biome/scene-kit, lượng actor có dáng/anim khác nhau, content/rule còn thiếu, campaign QA và bản phát hành trước. Một nghìn nhân vật có ý nghĩa là bài toán dữ liệu, đời sống và hành vi, không yêu cầu một nghìn mesh làm thủ công, cũng không được coi tổ hợp12looks là đã đạt1.000nhân vật khác biệt.

## Quy đổi ngày theo năng lực thực, không theo tên gói thuê bao

Gọi `E` là tổng effort lũy kế; `Q` là số giờ công AI hữu dụng thực thu được mỗi ngày sau quota/retry/gián đoạn; `C` là effort trên đường phụ thuộc tuần tự và `S` là năng lực mỗi ngày của lane đó. Khung tối thiểu để lập lịch:

`ngày = ceil(max(E / Q, C / S)) + chờ không chồng lấp + chờ quyết định của chủ dự án`.

Không cộng mọi thời gian chờ của các job độc lập, cũng không xóa hàng đợi Hyper3D/import/build chỉ vì AI không mệt. Không lấy số actor chia thẳng tổng ngày. Các bảng sau mới là phép quy đổi `ceil(E / Q)`, **chưa cộng đường tuần tự và thời gian chờ**; Q không được xác nhận là năng lực của một tài khoản Plus.

| Mốc/hướng | Q=4 giả định | Q=8 giả định | Q=12 giả định |
|---|---:|---:|---:|
| M1 — Đường chéo dài | 22–46 ngày | 11–23 ngày | 8–16 ngày |
| M1 — Hành lang thành đất | 24–51 ngày | 12–26 ngày | 8–17 ngày |
| M1 — Đất dựng thành | 25–54 ngày | 13–27 ngày | 9–18 ngày |
| M2 — Đường chéo dài | 80–166 ngày | 40–83 ngày | 27–56 ngày |
| M2 — Hành lang thành đất | 83–171 ngày | 42–86 ngày | 28–57 ngày |
| M2 — Đất dựng thành | 87–180 ngày | 44–90 ngày | 29–60 ngày |

Các ngày là ngày có năng lực Q như giả định, không mặc định lịch7ngày/tuần hay8giờ quyền dùng liên tục. Chưa đo Q trên tài khoản hiện tại nên không có một ngày hoàn thành đáng tin chỉ từ cụm “full Codex Plus”. Root có nguồn official về GPT-6.1 Sol high và limits; API model pricing không được dùng thay subscription quota hoặc suy số ngày sản xuất.

## Đường phụ thuộc, phần làm song song và rủi ro chặn lịch

Đường phụ thuộc chính: chốt scope/style → xác minh đường asset có thể dùng → một character usable/rig → art proof/camera → chốt bounded world/command/query contract → generator/repair/projection → Unity bind/feedback → save/load/end-to-end. Một phần scene kit và UI có thể làm song song sau khi scale/contract đã ổn. Việc giữ số actor thấp không giải quyết được một character vẫn không retarget/import được.

Unity scene/import/library là lane cần ownership; không cho nhiều worker cùng sửa một scene hoặc chung Library rồi giả sử merge tự sạch. Domain rules/projection contracts cần ổn trước khi UI binding; NavMesh hay animation không được quyết định authoritative route/time/outcome. Việc kiểm nhân vật đang cầm đồ, đi qua ramp và chọn đúng cao độ thường cần lặp scene/import/capture, khó chia độc lập hoàn toàn.

Đường tạo model3D thật phải theo chỉ dẫn Hyper3D MCP. Buổi đánh giá mới kiểm kê metadata công cụ, chưa chạy preflight hoặc một lượt tạo model để xác nhận readiness/credit/queue; không được kết luận connected-ready hay disconnected chỉ từ tên namespace. Đây là điều cần xác minh trước khi dùng forecast, không tự thay bằng procedural generation. Asset licensing, credit/queue/service lỗi, mesh/UV/rig không usable hoặc editor/provider không qua gate có thể làm lịch dừng. Không giả một model sinh ra đã có retopo/rig/LOD/cultural provenance đúng.

## Hiệu chỉnh trước khi chọn lịch

Đề nghị M0 work cap12–24giờ công AI, không tự chạy trong buổi brainstorm này. Đầu ra cần ghi: một asset source có provenance, import được hoặc lý do thất bại; rig/pose/contact thử trên một body nếu qua gate; camera720p và capture cao hơn khi ngân sách cho phép; pipeline log thời gian agent hoạt động/chờ/tool-error/retry; số lần sửa và lý do reject. Nếu chưa có usable character trong cap thì vẫn có kết quả hữu ích: biết gate đang chặn, không gọi art proof hoàn tất.

Sau M0 cập nhật các gói character/import/camera và Q đo được, rồi mới chốt lịch M1/M2. Chưa kiểm GPU/URP performance, animation quality hoặc throughput của Plus thì không ghi Passed. Model high có thể giúp lập luận/code/review, không là cam kết tốc độ, kỹ năng làm asset hay năng lực chạy vô hạn.

## Trao đổi chéo thật trong buổi này

- `/root/art_feas_architecture` ban đầu xếp environment Corridor < Diagonal < Earth. Reviewer effort nêu phản biện từ ảnh thật: corridor lộ mặt/tay/vải ở gần, diagonal dùng người nhỏ hơn và module lặp; kiến trúc đồng ý tách geometry-only khỏi matching-image quality. Cùng kết luận nên giữ ranges chồng nhau thay thứ hạng lịch cứng.
- Architecture đồng ý trần demo2body/fourlooks/6–8clips/20–30pieces/max5activeactors là giả định để so sánh, art proof chỉ2foregroundactor/3–4clips. Các giới hạn đó chưa được user duyệt thành scope triển khai.
- `/root/art_feas_art` cung cấp ROM art hai actor/rig/3–4clips36–80h, environment Diagonal12–28/Corridor8–18/Earth20–45h, close-Corridor16–36h, UI10–18h và visual/cultural QA8–16h. WBS ở trên dùng các khoản này một lần, thêm phần bootstrap và kiểm camera riêng có ranh giới rõ.
- Root yêu cầu Q4/8/12 chỉ là giả định, không Plus entitlement; không có dashboard quota hay benchmark throughput. Root cũng yêu cầu spike work cap có thể kết thúc khi asset gate chưa qua, không hứa xong một scene đẹp trong2ngày.

Đây là mô hình estimate của một actor để root/critic phản biện, không kế hoạch đã nghiệm thu. Không khởi tạo Unity, tạo model, đổi engine/roadmap/sprint hoặc ghi runtime acceptance trong task này.


### Làm rõ sau phản biện SPIN-036

M1 kiểm ngôn ngữ/camera từ ba PNG; M2 áp dụng ngôn ngữ được chọn vào Bến Vạc với vòng sửa thuyền. Ngân sách20–30module/props phải chứa cả bến/thuyền phù hợp, không tự dời Bến Vạc vào công trường/cầu-vách hoặc đổi repair thành xây thành. Nếu yêu cầu demo đúng môi trường trong ba PNG, cần scope/contract và re-estimate riêng. M0 dùng lại công từ các gói M1 khi kết quả usable; không cộng hai lần, nhưng failure/rework mới phải cập nhật dự toán. Đầu cao ROM không là worst-case được bảo đảm.


### Đính chính WBS sau kiểm tra coverage của root — SPIN-036

Bản nháp trước ghi cộng thêm chung M2 là182–364h, tổng M2 Đường chéo274–562/Hành lang286–588/Đất dựng thành296–611h. Root chỉ ra phạm vi M2 tăng looks/clips/kit và chuyển sang Bến Vạc nhưng WBS mới ghi gameplay/client, chưa có ngân sách art expansion minh bạch. Đây là thiếu coverage của dự toán, không sản phẩm đã triển khai hoặc chi phí phát sinh đã đo.

Đã thêm common art28–60h và art Bến Vạc theo hướng18–40/16–36/22–48h; giữ nguyên gameplay/client và QA đặc thù vốn có, không giấu art vào E2E. Tổng M2 thay bằng320–662/330–684/346–719h và M3 đổi theo đúng delta thành456–942/474–972/498–1.023h. Các bảng ngày phía trên đã tính lại; số cũ trong đoạn lineage này chỉ để truy nguyên đính chính, không dùng lập lịch. M1 không đổi. Đây vẫn là ROM thấp độ tin cậy, chưa qua calibration hoặc approval.
