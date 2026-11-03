# Kiến trúc đề xuất cho ba hướng hình ảnh

SPIN-036 · Sprint 13 · 02/11/2026 · Actor: `/root/art_feas_architecture`
Trạng thái: **Planning / Pending user approval**. Đây là đánh giá từ ba PNG và repository, chưa tạo model, scene, package manifest hay chạy Unity trong spin này.

## 1. Kết luận để ra quyết định

Ba hướng có thể dùng **chung một pipeline Unity 6000.6.7f1 + URP, thế giới và nhân vật 3D cách điệu, camera cố định hoặc di chuyển có giới hạn**. Khác biệt chủ yếu nằm ở bố cục, khoảng cách camera, độ lớn công trình, mật độ người và mức chi tiết nhân vật. Không cần ba engine hoặc ba hệ gameplay.

Theo nghĩa sản xuất, đây là **3D thật nếu được triển khai bằng mesh/rig/scene**. Có thể gọi cách trình bày camera hạn chế là “2.5D”, nhưng nhãn này không biến geometry thành 2D. Ba PNG hiện tại chỉ minh họa diện mạo; chúng không chứng minh đã có model, scene 3D, camera toán học chính xác hay game chạy được.

Khuyến nghị làm **Đường chéo dài** thành proof kỹ thuật đầu tiên: hình khối đơn giản hơn, mô-đun cầu lặp rõ, nhân vật không quá cận. **Hành lang thành đất** thích hợp kiểm chất lượng nhân vật gần và tương tác tay–đạo cụ. **Đất dựng thành** thích hợp kiểm quy mô, tầng cao và mật độ người sau khi pipeline cơ bản ổn định. Thứ tự này là đề xuất thử kỹ thuật, không chọn/bỏ phương án mỹ thuật thay chủ dự án.

## 2. Bằng chứng hiện có và giới hạn

Đã mở trực tiếp cả ba ảnh để đối chiếu, không chỉ suy luận từ tên:

| Artifact | Điều nhìn thấy trong ảnh | Hệ quả triển khai — suy luận, chưa đo |
| --- | --- | --- |
| [Đất dựng thành](../../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png) | Công trường rộng; khối đất nhiều tầng; cầu, giàn giáo, nhiều người xa và một nhóm tương tác gần | Nặng về bộ môi trường, phân lớp che khuất, chọn đúng tầng, nhiều hoạt ảnh nền và kiểm độ đọc ở khoảng xa |
| [Hành lang thành đất](../../../../../prototypes/sprint13/experiment-17/assets/earth-corridor.png) | Hành lang lớn; tường đất và dốc; ít nhân vật nhưng thân, mặt, vải và tay ôm gỗ thấy rõ | Geometry nền có thể gọn; chất lượng rig, bàn tay, tiếp xúc đạo cụ và camera gần mới là nút thắt |
| [Đường chéo dài](../../../../../prototypes/sprint13/experiment-18/assets/long-diagonal.png) | Cầu gỗ chéo dài, vách đất tạo khối lớn; cọc/ván lặp; người ở tỷ lệ vừa/nhỏ | Tái dùng kit tốt; cần giải đúng cao độ cầu, thứ tự chọn mục tiêu và framing dọc đường chéo |

Camera trong ảnh chưa đủ để xác định chính xác orthographic, tiêu cự hoặc vị trí. Đường chéo dài gợi bố cục axonometric; Đất dựng thành gợi góc cao; Hành lang gợi perspective gần hơn. Đây là **giả thuyết camera cần so sánh trong scene**, không là thông số đã trích xuất.

Repository hiện có portable core C# `.NET Standard 2.1` trong Foundation, Domain, Application và Infrastructure. [Client README](../../../../../client/UnityProject/README.md) xác nhận `client/UnityProject` mới là layout với `.gitkeep`, chưa có `ProjectVersion.txt`, manifest/lock hoặc scene. Sprint 10 đã kiểm chứng kernel profile trên Unity 6000.6.7f1/Linux x64/Editor Mono; kết quả đó không thay cho playable client, Player build hoặc mỹ thuật của ba hướng này.

Vì vậy, chi phí còn gồm **bootstrap client thực, camera/input/UI, adapter projection, asset/rig, build và kiểm thử tương tác**. Không được lấy việc core có sẵn để kết luận chỉ cần “đưa ảnh vào Unity”.

## 3. So sánh ba cách dựng

Các đánh giá dưới đây áp dụng khi muốn giữ ý đồ của từng PNG và cho người chơi tương tác, không chỉ mở một ảnh tĩnh.

| Phương án | Đất dựng thành | Hành lang thành đất | Đường chéo dài |
| --- | --- | --- | --- |
| **2D prerender**: nền và người là ảnh/sprite; collision, vùng đi và depth author riêng | Làm một khung đẹp nhanh; nhiều tầng/cầu/người che nhau khiến mask và state variant tốn công. Đổi công trường làm nền mất đồng bộ | Nền ít biến thể dễ hơn; người cận cần nhiều pose/hướng và tiếp xúc tay đúng vị trí. Phóng to lộ giới hạn sprite | Một camera cầu cố định khả thi; cầu phía trên và nền phía dưới vẫn cần mặt đi/collision/depth riêng. Đổi góc phải làm thêm bộ nền |
| **Hybrid 2.5D**: nền bake + nhân vật/đạo cụ tương tác 3D | Giữ cảnh rộng đẹp nhưng khó khớp bóng, cao độ, che khuất nhiều lớp và trạng thái xây dựng | Có thể hợp một encounter cố định; người 3D phải khớp ánh sáng, nền và điểm tiếp xúc | Khá phù hợp một shot cầu đóng; cần proxy depth/collision và neo vị trí chính xác cho từng đoạn |
| **3D thật + camera hạn chế**: mesh môi trường, nhân vật rig, UI 2D | Đầu tư kit/crowd/occlusion cao nhất; tái dùng được tầng, giàn giáo, góc nhìn và thay đổi môi trường | Ít khối nền; khó nhất ở người gần, mặt/vải/tay và framing khi đi | Cân bằng tốt nhất cho proof; kit lặp và shading đơn giản, kiểm được height/picking/đọc người |

**Đề xuất chung: hàng thứ ba.** 2D prerender vẫn đáng chọn nếu chủ dự án chốt game dùng rất ít camera, môi trường ít đổi và nhân vật chủ yếu hiển thị như sprite. Hybrid là phương án dự phòng cho cảnh xa hoặc cảnh nền ít đổi sau khi đo trong proof; không mặc định dùng nó để tiết kiệm.

Lý do ưu tiên 3D không phải muốn giống ảnh thật. Mesh đơn giản, tỷ lệ cường điệu, bảng màu hạn chế, ít texture và ánh sáng có kiểm soát vẫn tạo game 3D rất cách điệu. Với random origin, cùng rig/model có thể phục vụ nhiều diện mạo, chuyển động, chân dung và góc nhìn, giảm việc duy trì các bộ sprite rời bị lệch danh tính. Dù vậy, nhiều cơ thể/trang phục vẫn cần kiểm skinning và clipping; dùng chung rig không tự giải quyết mọi biến thể.

## 4. Stack chung — đề xuất, chưa cài trong client

| Lớp | Đề xuất | Phạm vi và điều chưa chứng minh |
| --- | --- | --- |
| Editor/render | Unity **6000.6.7f1**, URP compatible được Editor/Package Manager giải rồi pin manifest+lock | Giữ Editor đã có evidence kernel; URP là pipeline Unity hỗ trợ nhiều loại thiết bị. Chưa có package/version URP đã pin trong client, chưa đo hiệu năng; không cần HDRP cho proof này |
| Môi trường | Mesh cách điệu, kit tường đất/dốc/cầu/cọc/ván/giàn, prefab và material dùng chung | Có snap/scale/anchor quy ước; không dùng hàng trăm material riêng để bù asset thiếu đồng nhất |
| Shading | Bắt đầu với URP Lit/Unlit phù hợp; chỉ thêm Shader Graph khi cần kiểm soát bóng/phân mảng cụ thể | Không mở đầu bằng custom render pipeline hoặc hệ outline toàn game chưa có yêu cầu. Cần kiểm màu ở camera thật, không chỉ thumbnail |
| Nhân vật | Mesh 3D + rig dùng chung trong nhóm cơ thể tương thích; Animator/clip; socket đạo cụ | Retarget, mặt, tay, quần áo và contact có gate riêng. Chân dung có thể render từ cùng bộ nhân vật để giữ nhận diện |
| Camera | Một camera điều khiển bằng script nhỏ, giới hạn framing/zoom; thử orthographic và perspective tiêu cự dài nếu hợp shot | Cinemachine là lựa chọn nếu chuyển shot/blend phát sinh nhu cầu, không phải dependency bắt buộc từ đầu |
| Input | Unity Input System, keyboard/mouse; phiên bản compatible pin khi bootstrap | Action mapping, raycast/select, focus UI và modal phải kiểm. Chưa thêm controller/touch acceptance |
| UI | **uGUI + TextMeshPro** làm lựa chọn đầu cho demo: HUD, hội thoại, nhật ký, action preview/receipt, label | Một hệ chính dễ giữ focus/scale nhất quán. UI Toolkit UXML/USS là phương án hợp lệ cho screen UI nếu proof cho thấy lợi ích; không dùng đồng thời hai hệ chỉ vì cả hai sẵn có |
| Logic | Foundation/Domain/Application portable C# hiện có + adapter Unity mỏng | MonoBehaviour, Animator và UI không sở hữu rule, random origin, thời gian causal hoặc authoritative state |
| Lưu/load và build | Adapter theo contract, validate platform/Player riêng; nhập portable DLL hoặc cách tích hợp được spike chốt | SQLite/provider trong Player chưa được nghiệm thu chỉ vì headless/Editor kernel đã qua. Chọn build target chính thức và kiểm reopen thực trước demo |

Root kiểm kê phần cứng hiện tại: Intel UHD 630, CPU i5-8400, RAM khoảng 19 GiB; chưa có benchmark Unity 3D cho ba hướng. Có thể lấy **PC Linux, 720p và mục tiêu thử 30 fps** làm budget tạm của proof, không phải cam kết hoặc kết quả đạt. Bắt đầu URP Forward, ít nguồn sáng/bóng realtime, material đơn giản; đo cảnh gần/xa, shadow, số người và tầng che khuất trước khi tăng chất lượng. Proof Đất dựng thành nếu được chọn chỉ nên dựng một mảnh đại diện với hai nhân vật chính, không mặc định toàn panorama/crowd đều chạy.

Nguồn kỹ thuật: Unity mô tả URP là prebuilt Scriptable Render Pipeline cho nhiều nền tảng ([URP 6000.6](https://docs.unity.com/en-us/engine/6000.6/manual/render-pipelines/universal-render-pipeline/urp-introduction)). Tài liệu 6000.6 liệt kê **uGUI là khuyến nghị runtime, UI Toolkit là lựa chọn thay thế**; do đó đề xuất UI Toolkit trước đây không được diễn đạt thành mặc định chính thức hoặc package đã cài ([so sánh hệ UI](https://docs.unity.com/en-us/engine/6000.6/manual/uitoolkits/ui-system-compare)). Các lựa chọn cụ thể trong bảng là suy luận phù hợp dự án, chưa là quyết định baseline.

## 5. Ranh giới mô phỏng và cách hiển thị

Luồng dự kiến:

```text
Keyboard/mouse → Input + UI → yêu cầu preview/command có identity
                              ↓
                  Application / domain contracts
                              ↓
                 committed result + receipt
                              ↓
                  actor-scoped query/projection
                              ↓
            Presenter/ViewModel → nhân vật, cảnh, UI
```

- **Đồ họa không làm authoritative state.** Vị trí mesh, Animator phase, collider hit hoặc elapsed frame không tự tiêu thụ tài nguyên, hoàn thành công việc hay quyết định NPC biết gì. UI chỉ gửi intent và trình bày projection được phép thấy.
- Mỗi view gắn stable entity ID và **presentation anchor/surface ID** được adapter ánh xạ. Đó là đề xuất dữ liệu trình bày, không tự thêm canonical tọa độ liên tục vào domain. Tầng cầu, mặt đất và điểm thao tác phải phân biệt để click không xuyên nhầm đối tượng phía dưới.
- Nếu dùng NavMesh cho đường đi cục bộ/di chuyển hình ảnh, nó không thay route graph, travel cost, scheduler hoặc quyền commit của core. Gặp chướng ngại/đường đổi cần contract xử lý rõ, không tự “đoán lại” kết quả gameplay bằng physics.
- Lớp hiện có tên `ProjectionState` của event consumer không đủ bằng chứng đã có player knowledge projection. Adapter actor-scoped thật, error/stale handling và hạn chế dữ liệu phải được nghiệm thu riêng.
- Giữ random protagonist: UI hiển thị kết quả xuất thân đã được sinh hợp lệ, không thành màn chọn/chỉnh origin hoặc reroll.
- Giữ client baseline tiếng Việt, tối thiểu 1280×720, mục tiêu kiểm đọc 1920×1080, text scale 100/125/150%. Các panel tối giản trong PNG là hướng thẩm mỹ; nếu đổi cấu trúc của [client UX baseline](../../../CLIENT-UX-BASELINE.md), cần quyết định và impact update, không copy PNG thành đặc tả mới âm thầm.

**Design gap phải chốt trước implementation gameplay:** ý nghĩa di chuyển trong vùng/giữa vùng, quan hệ presentation anchor với route domain, thời điểm thao tác bắt đầu/kết thúc, khi actor còn bận, hủy/gián đoạn, góc nhìn được chuyển khi nào, dữ liệu nào được lộ trong encounter. Không dùng art direction để biến các unknown này thành engine behavior.

## 6. Cách triển khai từng hướng

### Đất dựng thành

1. Chốt silhouette chính bằng tầng đất, tuyến dốc, hai cụm giàn và khoảng trống đọc nhân vật; kiểm ở camera chơi trước khi thêm đông người.
2. Tách kit tường thẳng/góc/bậc/dốc, mặt đi, cầu nối và giàn. Kit lặp nhưng đổi chiều/nhóm để tạo quy mô; không cần mỗi khối trong PNG thành một asset độc lập.
3. Chia tầng hiển thị và mặt chọn; khi vật lớn che người, thử cutaway/fade có giới hạn để người chơi đọc được. Không fade làm lộ NPC/thông tin mà projection không cho biết.
4. NPC xa dùng ít chi tiết/chuyển động rõ; người gần dùng cùng bộ rig đã kiểm. Chỉ mở density sau khi cảnh nhỏ chạy được và đo trên target thực.
5. Công trường đổi trạng thái cần mapping trạng thái→prefab/material/đạo cụ; asset 3D không tự khiến hệ xây dựng tồn tại. Đừng đưa “xây cả thành” vào demo chỉ vì PNG thể hiện công trường.

Rủi ro chính: scope môi trường, cùng lúc nhiều lớp cao độ, crowd mất độ đọc, khối đất lớn gây che khuất và số trạng thái công trình. Đây là hướng có phạm vi nền cảnh lớn nhất trong ba ảnh.

### Hành lang thành đất

1. Dựng tỷ lệ hành lang/tường/dốc và khoảng encounter. Camera perspective gần hơn là ứng viên đầu; kiểm góc lùi khi nhân vật đi sát tường.
2. Bộ nền nhỏ tập trung chất lượng silhouette đất–gỗ và nhịp sáng tối; tránh trộn nền quá phẳng với nhân vật da/vải quá thật.
3. Hai cơ thể đại diện kiểm ở đúng kích thước màn hình trong PNG: đi, đứng nói, quay, ôm/cầm gỗ; test xuyên tay/vải/socket và nhìn mặt tại mức zoom được phép.
4. Chốt có giữ fidelity mặt/vải gần như PNG hay giản lược sang ngôn ngữ khối của Đường chéo dài. Lựa chọn giản lược cần chủ dự án chốt vì nó thay mục tiêu hình ảnh, không là tối ưu vô hình.
5. UI encounter phải né vùng mặt/tay/đạo cụ và không làm việc chọn hành động bị lẫn với click nhân vật. Kiểm cả 720p và tăng chữ.

Rủi ro chính: **nhân vật**. Nền ít mesh không có nghĩa toàn hướng rẻ nhất; lỗi mặt/tay/contact dễ thấy hơn hai hướng còn lại.

### Đường chéo dài

1. Thử cùng một kit với camera orthographic và perspective tiêu cự dài; so silhouette, độ đọc mặt cầu và nhân vật ở hai đầu đường chéo. Chưa chốt “isometric chính xác” từ ảnh.
2. Kit cầu gồm đoạn ván, cọc, lan can, đầu nối, điểm giao/điểm đứng; kit đất gồm mảng vách và mép. Kiểm seam, scale và shadow trước khi nhân bản chiều dài.
3. Mỗi đoạn cầu có presentation surface riêng khi cần; selection ưu tiên mục tiêu được phép thấy và tầng đúng. Collider cầu/mặt đất không được trở thành nguồn travel truth.
4. Dùng tỷ lệ nhân vật, màu trang phục, vật mang và animation pose để phân biệt vai trò. Không cần mặt như ảnh thật để tạo nhân vật nhận ra được.
5. Kiểm framing tại điểm encounter, khi người đi gần mép màn hình và khi UI mở. Giữ đường chéo có ý nghĩa định hướng, không thành hành lang đẹp nhưng khó thao tác.

Rủi ro chính: chọn sai cao độ, cầu che mục tiêu, lặp kit quá lộ và khoảng trống thẩm mỹ làm giảm thông tin chơi. Phạm vi modular và mức stylization khiến đây là ứng viên proof tốt, chưa phải bảo đảm thời gian ngắn nhất trong mọi lựa chọn chi tiết.

## 7. Pipeline sản xuất nếu được giao bước triển khai

1. **Chốt target và gap:** chọn fidelity/camera, kiểm tham chiếu Văn Lang–Âu Lạc, phân loại dữ kiện–diễn giải–hư cấu. Khối thành đất, giàn, trang phục trong ảnh không tự trở thành canon lịch sử.
2. **Khởi tạo client thực:** Editor được pin, URP/input/UI tương thích được ghi manifest+lock; nhập core; một build trống kiểm platform. Chưa đưa scope gameplay vào nếu chỉ được giao proof hình ảnh.
3. **Asset proof nhỏ:** đề xuất một cảnh, hai nhân vật đại diện, 3–4 clip, một lần chọn/tương tác UI minh họa; mục tiêu là kiểm camera, scale, material, rig và tay–đạo cụ. Phải ghi UI/logic giả lập nếu chưa gọi contract thật.
4. **Nếu tạo model thật:** theo chỉ dẫn hiện hành, đi qua **MCP Hyper3D**. Root đã thấy metadata tool; chưa gọi generation/authorization preflight trong spin này nên không khẳng định sẵn sàng tạo asset. Blender chỉ xử lý cleanup, rig/bố trí/render sau đó trong scope được giao; không tự thay bằng sinh procedural model. Cần kiểm topology, UV, material, scale, giấy phép và khả năng rig trước khi nhân rộng.
5. **Chuẩn hóa asset:** ghi origin/provenance/version; đơn vị, forward/up, pivot, socket, naming, collision và import preset. Kiểm hai biến thể cơ thể/trang phục để phát hiện rig chỉ hợp một nhân vật mẫu.
6. **Nối bounded demo:** đề xuất 2 body archetype, 4 diện mạo, 6–8 clip tái dùng, 20–30 loại mảnh môi trường/đạo cụ, tối đa 5 actor hoạt động trong cảnh (PC + 4 NPC). Đây là trần scope ước lượng, không bằng chứng đã có nội dung và không thay roster đầy đủ. Demo khoảng 20 phút còn cần random origin, command path, projection, nội dung tương tác và save/reopen thật được giao riêng.
7. **Nghiệm thu:** chạy đúng target, kiểm screenshot/camera + nhập/chọn/che khuất + character motion + projection/privacy + preview/confirm/receipt + save/reopen. Lưu cấu hình và evidence. Chỉ đo FPS/memory/build time sau khi có build thực; không đặt một số benchmark giả định thành kết quả.

## 8. Phản biện giữa các tác nhân và hệ quả dự báo

- `/root/art_feas_art` chỉ ra nhân vật Hành lang thành đất chiếm tỷ lệ lớn và có tiếp xúc tay–gỗ; `/root/art_feas_estimate` phản biện xếp hạng “ít geometry ⇒ nhanh nhất”. Kết quả: tách **chi phí nền cảnh** khỏi **chi phí nhân vật đúng fidelity PNG**; không khẳng định Hành lang luôn rẻ hơn Đường chéo dài.
- Ba bên thống nhất dùng chung base pipeline và rig; không tính bootstrap ba lần khi so sánh các hướng. Ngược lại, nếu làm cả ba đạt chuẩn thì vẫn phải tính phần kit/camera/art review riêng, không lấy một scene xong là cả ba xong.
- Rủi ro tuần tự gồm: target mỹ thuật chưa chốt → asset mẫu chưa đạt → rig/contact chưa ổn → không thể nhân bộ diện mạo/animation → không thể chốt chất lượng demo. Nhiều tác nhân giúp chia code/docs/review, không làm chuỗi phụ thuộc này nhanh lên theo phép chia số người.
- Codex Plus + GPT‑6.1 Sol high là giả định công cụ do người dùng đưa. Không có benchmark riêng cho repo/asset pipeline, quota thực hoặc tốc độ sinh/chỉnh model đã đo; tài liệu này không suy ra thời lượng từ tên gói/model. Các dải effort và lịch dự kiến phải có buffer review/chờ và giữ điều kiện này trong báo cáo tổng hợp.

**Đề xuất quyết định:** cho phép một proof Đường chéo dài để kiểm pipeline 3D cách điệu; dùng Hành lang làm bài kiểm tra nhân vật gần và Đất dựng thành làm bài kiểm quy mô sau đó. Chưa có quyết định nào trong tài liệu này thay quyền duyệt mỹ thuật của chủ dự án hoặc cho phép tự mở Sprint 14.


### Làm rõ sau phản biện SPIN-036

M1 kiểm ngôn ngữ/camera từ ba PNG; M2 áp dụng ngôn ngữ được chọn vào Bến Vạc với vòng sửa thuyền. Ngân sách20–30module/props phải chứa cả bến/thuyền phù hợp, không tự dời Bến Vạc vào công trường/cầu-vách hoặc đổi repair thành xây thành. Nếu yêu cầu demo đúng môi trường trong ba PNG, cần scope/contract và re-estimate riêng. M0 dùng lại công từ các gói M1 khi kết quả usable; không cộng hai lần, nhưng failure/rework mới phải cập nhật dự toán. Đầu cao ROM không là worst-case được bảo đảm.

Dự toán M2 cuối trong [03 — Effort model](03-EFFORT-MODEL.md) đã tách thêm công mở looks/clips/UI/import và kit bến/thuyền; không ngầm coi việc tăng asset là gói Unity binding/E2E. Trần20–30pieces là tổng gồm kit M1 dùng lại, không20–30asset mới.
