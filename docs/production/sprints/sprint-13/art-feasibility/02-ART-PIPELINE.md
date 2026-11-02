# Khả thi mỹ thuật — ba hướng thành đất, hành lang và đường chéo

02/11/2026 · Sprint 13 / SPIN-036 · Actor `/root/art_feas_art` · **Phân tích để quyết định, Pending user approval**.

Ba hướng có thể dùng chung một hệ asset và render cách điệu. Khác biệt lớn về effort nằm ở số lớp không gian, khoảng cách nhìn người và mức chi tiết cần giữ. Đề xuất của nhánh mỹ thuật là kiểm chứng **Đường chéo dài trước**, sau đó thử cùng người/kit ở cự ly Hành lang; Đất dựng thành là bài kiểm tra mở rộng quy mô sau. Đây là chọn thứ tự đo rủi ro, chưa chọn style thay chủ dự án.

## Artifact đã trực tiếp xem

Reviewer đã mở đủ ba PNG nguyên gốc trong project, không suy từ prompt hoặc tên phương pháp:

| Hướng | Quan sát thật trên ảnh | Phần trở thành chi phí khi dựng game |
|---|---|---|
| [Đất dựng thành — 14/earth-monument](../../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png) | Công trường khổng lồ, tầng đất/hào, đường/cầu/giáo chồng lớp, nhiều nhóm người ở xa; tiền cảnh có người mang bình, người nhận và người đào. Hình người góc cạnh, màu đất đỏ và khối tối giữ nhịp đồ họa. | Khối lượng cảnh, che khuất theo tầng, biến thể kit, bố trí người làm việc và khoảng cách chi tiết. Một đoạn tường đơn giản không tái hiện được toàn bộ giá trị của ảnh. Crowd đang có trong tranh không chứng minh NPC autonomy hoặc dân số runtime. |
| [Hành lang thành đất — 17/earth-corridor](../../../../../prototypes/sprint13/experiment-17/assets/earth-corridor.png) | Ít khối lớn: tường đỏ, ramp, sàn/cột gỗ và cửa nhìn ra sông. Người mang hai thanh gỗ và người già chỉ đường rất gần; da, tóc, vải và tay có nhiều chi tiết hơn môi trường. | Hình học môi trường rẻ hơn, nhưng khuôn mặt, tuổi, bàn tay giữ vật, da/vải và động tác bị soi gần. Giảm texture hoặc giản mặt để dùng lại pipeline của Đường chéo là một thay đổi hình ảnh cần được user xem/chốt. |
| [Đường chéo dài — 18/long-diagonal](../../../../../prototypes/sprint13/experiment-18/assets/long-diagonal.png) | Một cầu xiên lớn đi xuyên hai mép, cọc/lan can/ván lặp, vách đá thành các mặt lớn; hai người ở một điểm gặp, UI theo cạnh xiên. | Kit lặp và mặt phẳng ít texture thuận lợi hơn cho proof đầu. Cần xử lý chân đặt trên cầu, tay tựa cọc, vật sau lưng, va vào lan can và che khuất; khối vách đơn giản không tự làm navigation hoặc picking đúng. |

Cả ba artifact hiện là **PNG được sinh để mô phỏng diện mạo game**, không chứa mesh, UV, skeleton, animation clip, colliders hoặc bản đồ độ sâu đủ dùng. Không có pipeline “đưa nguyên ảnh vào Unity là thành game 3D”. Chúng cũng chưa chứng nhận phục trang, mái, công cụ hoặc kiến trúc là phục dựng Văn Lang–Âu Lạc.

## Chọn cách sản xuất: 3D chung, camera giới hạn

Đề xuất mỹ thuật: **world và nhân vật 3D cách điệu, camera cố định hoặc chỉ cho dịch/zoom giới hạn; UI 2D riêng**. Có thể gọi trải nghiệm là 2.5D vì cách nhìn bị giới hạn, nhưng phần hình học/rig vẫn 3D. Orthographic, axon hoặc perspective không quyết định một asset là 2D hay 3D; phải lựa camera sau khi đối chiếu cùng scene thử.

Một nền 2D dựng sẵn có thể tạo bản xem tĩnh rẻ hơn. Tuy nhiên chi phí tăng khi phải thay góc, trang phục, vật mang, ánh sáng, độ cao/chỗ đứng hoặc tình trạng công trình. Mỗi phương án lại cần mask che khuất, điểm tựa, shadow và góc nhân vật khớp. Đây là suy luận sản xuất cho yêu cầu thế giới thay đổi của dự án, không benchmark đã đo. Hybrid: actor 3D trên nền 2D vẫn phải giải bài toán độ sâu và đồng nhất ánh sáng; nó không xóa effort nhân vật.

Có thể kết hợp **quy mô và khối đất của 14 + kit/facet tiết chế của 18 + cự ly cuộc gặp của 17** trong cùng ngôn ngữ. Để giữ effort thấp, đề nghị bản thử đầu giản da/vải/tóc của 17 theo mức chi tiết 18, không hứa tái hiện nguyên mức cận mặt của PNG17 ngay. Không cần ba engine hoặc ba bộ rig khác nhau chỉ vì ba ảnh có camera khác nhau.

## Phạm vi đếm asset để ước tính công bằng

Đây là ngân sách đề xuất, cần đo lại sau spike; **không phải asset hiện đã có**. “Unique” là một mesh/prop definition phải tạo và kiểm; đặt lặp 50 cọc không phải 50 model mới. Scene instances, vật liệu dùng lại, biến thể trang phục và clip compatibility phải ghi riêng.

| Mốc | Ngân sách mỹ thuật có giới hạn | Chưa được suy rộng |
|---|---|---|
| Probe thông lượng asset | Một module cảnh, một người, một vật cầm; thử import/rig/đặt chân/giữ vật ở camera gần và xa | Chưa gọi đó là styleproof hoàn chỉnh, demo hoặc model pipeline đã ổn định |
| Art proof | Một scene nhỏ, hai actor từ hai body archetype; khoảng 12–18 unique environment/prop definitions; 3–4 clip categories dùng lại; một layout hội thoại, hai portrait từ cùng nhân vật, vài icon đủ hai hành động | Không tái hiện đủ panorama/crowd 14; không full roster hoặc gameplay acceptance |
| Demo khoảng 20 phút | Hai body archetype, bốn look đã kiểm; 6–8 clip categories; tổng 20–30 unique environment/prop definitions; tối đa 5 actor hiện diện/hoạt động cùng lúc; UI thật của phạm vi demo | Không đủ 1.000 nhân vật, nhiều vùng, dân cư công trường hoặc toàn hệ animation |

Clip categories là idle/walk/carry/point/interact theo scope; không mặc định đã có mọi clip trên mọi rig. Hai rig với tám clip categories cần tới 16 kiểm tương thích, không tự thành 16 clip nguồn riêng. Không thêm lip-sync, motion capture, cloth simulation hoặc camera quay tự do vào mốc này mà vẫn giữ cùng ước tính.

Giữ cùng phạm vi art proof khi so sánh: Đất dựng thành chỉ là một mảng đại diện có tầng và nhịp lao động, chưa phải toàn công trường trong ảnh. Nếu bắt tái hiện panorama với crowd có hoạt động thật, phải mở scope và ước tính lại. Demo chỉ hiển thị actor/thông tin được phép từ projection; crowd trang trí không dùng để tuyên bố có mô phỏng dân cư.

### Kit riêng và phần dùng chung

| Nhóm | Dùng chung | Đất dựng thành | Hành lang thành đất | Đường chéo dài |
|---|---|---|---|---|
| Kiến trúc/địa hình | Chuẩn tỷ lệ, pivot, palette, vật liệu, cách nối module | Khối đất, mép hào, ramp, sàn giáo, cọc/cầu, nhóm silhouette xa | Tường/đầu tường, ramp, góc sàn, cột/giằng và khoảng mở | Ván/cọc/lan can/nút cầu, đầu cầu, nhóm vách đá có cạnh lớn |
| Nhân vật | Archetype/rig, tóc/trang phục theo look, pose quy ước, portrait từ cùng model | Thêm chất lượng nhóm xa và loop lao động nếu scope yêu cầu | Ưu tiên mặt/tuổi, tay ôm gỗ, vai/cổ và tiếp xúc nhìn gần | Ưu tiên chân trên cầu, pose tựa cọc, giỏ sau lưng và dáng nghiêng |
| Props | Bình/giỏ/vật mang, socket, scale và material family | Công cụ đào/mang và vật liệu công trường | Hai thanh gỗ với pose giữ nhất quán | Giỏ và các điểm tựa trên kit cầu |
| UI | Font Việt, semantic role player/speaker, icon library, focus/state | Nhịp khối và cột thông tin nếu được chọn | Khung lớn dễ đọc trong cuộc gặp gần | Cạnh xiên ở chrome, chữ vẫn thẳng và đủ rộng |

Không copy chữ/nút từ PNG làm UI production. Portrait nên render từ cùng model/pose/ánh sáng đã duyệt để giảm trôi identity giữa người trong scene và mặt trong UI. Cách này vẫn cần review chất lượng, không tự bảo đảm khuôn mặt đúng tuổi hoặc hợp văn hóa.

## Pipeline AI có cổng kiểm tra

| Bước | AI tạo/chuẩn hóa gì | Gate trước bước tiếp |
|---|---|---|
|1. Hợp đồng hình ảnh| Chốt camera bands, tỷ lệ adult, mức texture/face, palette theo vai trò, kit list và nguồn văn hóa | User có artifact so sánh cụ thể; unknown không đổi thành canon |
|2. Preflight công cụ| Kiểm actual MCP/account/job/result khi task tạo asset thật được giao | Root đã thấy metadata quảng bá 9 tool Hyper3D lúc kiểm kê; metadata chưa chứng minh OAuth, generation, rig hoặc chất lượng usable mesh đã chạy |
|3. Sinh nguồn 3D| Model generation qua MCP Hyper3D theo chỉ dẫn dự án; ghi prompt, job ID, source và kết quả thật | Mesh tải/import được; không nhận preview render là model đã usable. Không tự chuyển sang procedural Blender/engine generation khi MCP không dùng được |
|4. Cleanup/retopology/UV| Blender hoặc công cụ đã được phép để xử lý model nguồn, chỉnh normals/tỷ lệ/pivot/UV/material, giảm phần ẩn và sửa topology | Pose-test cho thấy khớp không méo/hở, texture không kéo sai, feet/origin và scale ổn. Không âm thầm xây asset mới từ đầu để né đường generation đã quy định |
|5. Rig/skinning/clip| Kiểm skeleton và skin weights, map vào Unity Humanoid nếu cấu trúc phù hợp; chuẩn hóa clip/socket vật | Mỗi body/clip pair pass kiểm tay/chân/cổ/vải. Auto-rig advertised hoặc clip preset không đồng nghĩa mọi body đã đạt |
|6. Kit/prefab/camera| Bố trí asset đã duyệt, collider/pivot, light/material tier và mức chi tiết theo khoảng cách | Không hở mép module, không xuyên lan can, vật mang/đổ bóng/che khuất đọc được ở camera target |
|7. UI/portrait/icon| Component text thật, portrait cùng actor, icon cùng style; semantics player/speaker tách rõ | Tiếng Việt và focus/hover/disabled đọc được; UI không mở hidden truth hoặc ghi authoritative state |
|8. Review/build| Ảnh chụp cùng checkpoint, turntable/pose test, clip/contact checks và build trên máy target | Technical review, art approval và gameplay acceptance là ba gate riêng; lưu failures/rework thay vì chỉ ảnh đẹp |

Unity mô tả modeling, rigging và skinning là các công đoạn khác nhau; topology và scale phải được kiểm để model biến dạng đúng khi animation. Vì vậy raw output đẹp không đủ làm actor production. [Unity 6.6 — Creating models for animation](https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/asset-types/models/creating-dccassets/using-humanoid-chars).

Unity hỗ trợ tái dùng animation giữa Humanoid models khi từng model được cấu hình Avatar. Đây là cơ sở chọn rig/clip dùng lại, không bằng chứng retarget của các model dự án đã đạt. [Unity 6.6 — Retarget humanoid animation](https://docs.unity.com/en-us/engine/6000.6/manual/animation-section/animation-mecanim/avatar-creationand-setup/retargeting).

## Bottleneck và effort tham chiếu cho nhánh mỹ thuật

Các band dưới là **ROM do agent suy luận từ scope**, chưa có throughput test GPT-6.1 Sol high, asset acceptance rate hoặc usable-rig benchmark. Không lấy số phút tạo PNG làm số phút tạo actor. Codex làm code/điều phối/kiểm; asset generator, rig/mesh cleanup, kiểm chất lượng và vòng sửa vẫn là công việc thật. Điều kiện Plus và lịch elapsed do nhánh ước tính/root tổng hợp.

| Thành phần art proof | Band đầu vào đã gửi nhánh estimate | Cách tránh đếm hai lần |
|---|---|---|
| Hai actor base + rig/material  + 3–4 clips/contact căn bản |36–80 AI active hours | Là phần chung; không cộng thêm nếu common WBS đã chứa nó |
| Environment/layout art riêng Đường chéo |12–28h | Chỉ phần art của increment theo hướng, không cộng ngoài increment tổng đã dùng |
| Environment/layout art riêng Hành lang |8–18h | Geometry rẻ; nếu giữ close fidelity PNG17 còn increment mặt/tay riêng |
| Environment/layout art riêng Đất dựng thành |20–45h | Bản art proof giới hạn, không cả panorama/crowd simulation |
| Polish close face/hand để gần fidelity PNG17 | Tham chiếu +16–36h | Bổ sung lên base đã có, không một bộ rig thứ hai hoặc lip-sync |
| UI/portrait/icon |10–18h | Chung một library/layout family, không làm lại cho cả ba scene |
| Art review/trace văn hóa/import/contact kiểm lại |8–16h | Phân biệt QA chung với camera/occlusion QA từng hướng |

“AI active hours” cần dùng cùng định nghĩa với WBS tổng, không là giờ máy chạy liên tục, giờ inference hoặc quyền dùng 8h/ngày của Plus. Các số trên được gửi làm input để agent estimate gộp; không phải bảng để người đọc tự cộng tất cả thành deadline. Proof phải đo lại thông lượng một asset/actor usable trước khi tăng số lượng.

Xếp hạng theo điều kiện: **geometry-only Hành lang dễ nhất; art proof theo fidelity ảnh Đường chéo thường ít rủi ro hơn Hành lang; full panorama Đất dựng thành nặng nhất**. Nếu user chấp nhận giản mặt/vải của Hành lang xuống mức Đường chéo, hai hướng có thể gần effort nhau. Đây là giả thuyết cần cùng kit/actor/camera test, không số đo hiệu suất.

## Tuổi, văn hóa và quy mô nhân vật

Random-origin khiến pipeline phải chịu được tuổi/thể trạng/giới và trang phục theo dữ liệu; không chỉ một người hùng trẻ đổi màu áo. Body archetype, rig compatibility, bàn tay giữ prop, tóc và silhouette cần giữ identity qua scene/portrait. Với camera17, chỉ tóc xám không đủ chứng minh mặt già; lỗi này đã xuất hiện trong các vòng art trước.

Mục tiêu khoảng 1.000 nhân vật khác biệt có ý nghĩa trong GDD không bằng 1.000 mesh/rig bespoke đã có hoặc bắt buộc. Có thể dùng library body/look có kiểm soát và dữ liệu cuộc đời khác nhau, nhưng số tổ hợp hợp lệ và chi phí kiểm clipping/age/rig phải đo; bốn demo looks không chứng minh đã phủ toàn quy mô.

Khối đất/hào/cầu giáo gợi ngôn ngữ không gian mạnh, nhưng cả ba ảnh còn diễn giải hư cấu về công trình, dụng cụ và trang phục. Cần provenance hiện vật/nguồn cho loại nhà, công cụ, vật liệu và silhouette được chọn trước khi mass-produce. [Workshop](../ART-DIRECTION-RETHINK.md) và [ngữ pháp văn hóa](../art-rethink/02-CULTURAL-GRAMMAR.md) giúp phân lớp tham chiếu, diễn giải và game-canon; chúng không chứng nhận từng chi tiết trong ba PNG.

## Trao đổi chéo thực trong buổi brainstorm

- `/root/art_feas_architecture` xác nhận client hiện chỉ scaffold tài liệu, khuyến nghị3D actor/world chung và camera giới hạn; phản biện xếp hạng geometry-only khác total art fidelity. Hai nhánh thống nhất art proof 2 actors / 3–4 clip categories; demo 2 archetypes / 4 looks / 6–8 clip categories / 20–30 environment-prop definitions / max 5 active actors.
- `/root/art_feas_art` gửi architecture/estimate rằng close face/hand của 17 có thể làm Hành lang đắt hơn Đường chéo dù ít khối; portrait từ cùng model và base/rig dùng lại giúp giảm identity drift, vẫn phải review.
- `/root/art_feas_estimate` phản hồi đã chứa band actor 36–80h trong common WBS và tách direction increment. Art nhắc không cộng lại baseline, không đếm 2 rig × 8 compatibility checks thành 16 clip nguồn, không hiểu increment polish là thêm lip-sync.
- `/root` đính chính tool visibility: root thấy Hyper3D metadata; child không có namespace không đủ kết luận service disconnected. Tài liệu này giữ actual connection/usable mesh/rig-quality **chưa kiểm**, không gọi đó là blocker đã được chứng minh.

Không mở model, tạo scene, chạy Unity/animation test, đo benchmark hoặc thực hiện Git trong nhánh này. Kết quả là đề xuất art pipeline và phạm vi kiểm chứng; mọi phương án/Sprint 13 tiếp tục Pending user approval.


### Làm rõ sau phản biện SPIN-036

M1 kiểm ngôn ngữ/camera từ ba PNG; M2 áp dụng ngôn ngữ được chọn vào Bến Vạc với vòng sửa thuyền. Ngân sách20–30module/props phải chứa cả bến/thuyền phù hợp, không tự dời Bến Vạc vào công trường/cầu-vách hoặc đổi repair thành xây thành. Nếu yêu cầu demo đúng môi trường trong ba PNG, cần scope/contract và re-estimate riêng. M0 dùng lại công từ các gói M1 khi kết quả usable; không cộng hai lần, nhưng failure/rework mới phải cập nhật dự toán. Đầu cao ROM không là worst-case được bảo đảm.

Dự toán M2 cuối trong [03 — Effort model](03-EFFORT-MODEL.md) đã tách thêm công mở looks/clips/UI/import và kit bến/thuyền; không ngầm coi việc tăng asset là gói Unity binding/E2E. Trần20–30pieces là tổng gồm kit M1 dùng lại, không20–30asset mới.
