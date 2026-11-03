# Khả thi sản xuất: Đất dựng thành, Hành lang thành đất, Đường chéo dài

> Cập nhật hiện hành03/11/2026: chủ dự án đã chốt **Đất dựng thành nền2D có cảm giác khối + nhân vật sprite**, theo [baselineG13](ART-DIRECTION-BASELINE.md) và [approval](APPROVALS.md). Nội dung dưới giữ trạng thái/evidence tại thời điểm nghiên cứu; route và dự toán3D không là quyết định hoặc dự toán2D hiện hành.

02/11/2026 · Sprint13 / SPIN-036 · Điều phối `/root` · **Đề xuất để quyết định, Pending user approval**.

## Kết luận bằng ngôn ngữ dễ hiểu

Cả ba hướng đều có đường triển khai khả thi về mặt kỹ thuật, còn cần qua kiểm tra asset và hiệu năng. Đề xuất chung là **thế giới 3D cách điệu, nhân vật 3D có xương chuyển động, giao diện 2D và góc máy giới hạn**. Không cần đuổi theo ảnh thật. Có thể giữ khối đất lớn, hình người giản lược, ít texture và màu được thiết kế theo vai trò. Ba ảnh hiện nay vẫn chỉ là PNG mô phỏng; chưa có scene hay model tương ứng.

Đề xuất thử **Đường chéo dài trước**, dùng **Hành lang thành đất để kiểm nhân vật cận**, rồi dùng **Đất dựng thành để kiểm quy mô và nhiều tầng**. Đây là thứ tự kiểm kỹ thuật, không phải lựa chọn mỹ thuật thay chủ dự án. Ba hướng có thể cùng tồn tại như những cách quay cảnh khác nhau trong một game nếu sau này thống nhất tỷ lệ người, vật liệu và UI; hiện chúng chưa tự khớp thành một bộ asset.

Phần tốn công nhất không nằm ở dựng vài bức tường. Đó là tạo nhân vật giữ được bản sắc khi chuyển động, sửa rig/tay/vải, biến ảnh thành không gian chọn và đi được, nối thông tin đúng nhân vật với luật game thật và kiểm save/load trong Unity. Core có sẵn giúp giảm việc làm lại nền tảng, nhưng client chơi được vẫn chưa có.

Với giả định **toàn bộ nhóm AI tạo ra tổng cộng 8 giờ công hữu dụng mỗi ngày**, ngân sách quy đổi cho một cảnh thử mỹ thuật có chuyển động khoảng **11–27 ngày hoạt động** tùy hướng; cho demo gameplay nhỏ khoảng **40–90 ngày hoạt động**. Đây là khoảng lập kế hoạch chưa hiệu chỉnh, **chưa cộng phần chờ dịch vụ, phụ thuộc tuần tự vượt năng lực đó hoặc chờ chốt của chủ dự án**. Không thể hứa ngày hoàn thành chỉ từ tên gói Plus. Bước đo thử có giới hạn trước khi cam kết lịch được nêu ở cuối.

## 1. Buổi brainstorming đã thực hiện

| Actor thật | Trách nhiệm và đầu ra |
|---|---|
| `/root/art_feas_architecture` | [01 — Kiến trúc](art-feasibility/01-ARCHITECTURE.md): xem ba PNG, so 2D/hybrid/3D, kiểm client/core, camera và quyền sở hữu state |
| `/root/art_feas_art` | [02 — Pipeline mỹ thuật](art-feasibility/02-ART-PIPELINE.md): xem ba PNG, asset/rig/clip/UI, chi phí cự ly gần và tái sử dụng kit |
| `/root/art_feas_estimate` | [03 — Effort](art-feasibility/03-EFFORT-MODEL.md): xem ba PNG, gộp WBS dùng chung/phần riêng, mốc, lịch và đường phụ thuộc |
| `/root/art_feas_critic_estimate` | [06 — Phản biện effort](art-feasibility/06-CRITIC-ESTIMATE.md): lượt đầu bị usage limit; sau khi tiếp tục đã đọc báo cáo/nguồn và kiểm WBS sửa cuối |
| `/root/art_feas_critic_tech` | [05 — Phản biện độc lập](art-feasibility/05-CRITIC-TECH-ART.md): kiểm tech/art và kiểm số bổ sung trong cùng một lượt review |
| `/root` | Xem ba PNG, kiểm repo/hardware/tool metadata, tra official docs, tổng hợp và [04 — tự kiểm số/phạm vi](art-feasibility/04-ROOT-REVIEW.md). Self-review không thành independent review thứ hai |

Ba worker trao đổi chéo thật: xếp hạng môi trường ban đầu là Hành lang < Đường chéo < Đất dựng thành; nhánh art/estimate phản biện rằng fidelity mặt/tay gần có thể đảo hai hướng đầu. Kết luận được sửa thành hai tiêu chí riêng: **ít geometry** và **ít effort để giữ chất lượng ảnh khi chạy**. Phần nhân vật dùng chung được tính một lần, không cộng lại ở từng nhánh. Báo cáo giữ giới hạn này thay vì chọn một số ngày chính xác giả tạo.

Cấu hình được đánh giá là **Codex Plus + GPT-6.1 Sol high** như user yêu cầu. Không tuyên bố mọi actor vừa chạy đã dùng hoặc được benchmark bằng cấu hình đó; không thay model/account settings trong buổi này. Tối đa bốn slot đồng thời, các vai làm theo lượt; không ghi sáu người chạy cùng lúc.

## 2. Đọc đúng ba mẫu và phân loại 2D/3D

| Mẫu nguồn | Điều thực sự nhìn thấy | Cách làm đề xuất nếu triển khai | Nút thắt |
|---|---|---|---|
| [Đất dựng thành — vòng14](../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png) | Công trường panorama, tầng đất/hào, cầu và giàn, nhiều người ở các cự ly | Mesh 3D modular; camera cao, perspective hạn chế; người gần rig đầy đủ, người xa giảm chi tiết sau đo | Quy mô, chọn đúng cao độ, che khuất, số hoạt động nền; nhiều trạng thái công trường |
| [Hành lang thành đất — vòng17](../../../../prototypes/sprint13/experiment-17/assets/earth-corridor.png) | Ít khối tường/dốc lớn nhưng thân, mặt, tay và vải hai người rất gần | Mesh 3D nền gọn; nhân vật rig; camera perspective gần có giới hạn | Mặt/tuổi, tay giữ gỗ, tiếp xúc, vải, góc máy đụng tường; chất lượng cận |
| [Đường chéo dài — vòng18](../../../../prototypes/sprint13/experiment-18/assets/long-diagonal.png) | Cầu/vách theo đường chéo, cột/ván lặp, người vừa/nhỏ, mảng hình tiết chế | Kit cầu/vách 3D; thử orthographic và perspective tiêu cự dài trước khi chốt | Chọn đúng mặt cầu, chuyển động trên cao, framing hai đầu, kit lặp quá rõ |

**Ảnh 2D không quyết định engine phải 2D.** Nhãn “2.5D” trong đề xuất này nói về cách nhìn hạn chế của một scene có geometry 3D. Camera orthographic cũng có thể nhìn mesh 3D; không suy từ PNG rằng Đường chéo dài đã dùng isometric chính xác. Các camera trên là giả thuyết để thử. [Unity Camera.orthographic](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/camera/orthographic).

Cả ba vẫn có đường 2D: nền vẽ/bake, nhân vật sprite, vùng đi/collision và lớp che khuất author riêng. Cách đó hợp một vài shot cố định và trạng thái ít đổi. Nhưng muốn nhiều dáng người, vật cầm, góc quay, cầu nhiều tầng và cảnh biến đổi sẽ cần nhiều sprite/mask/variant; không mặc định rẻ hơn. Hybrid nền bake + người 3D là phương án dự phòng có ích cho cảnh xa, đổi lại phải khớp depth, bóng và điểm tiếp xúc. [01](art-feasibility/01-ARCHITECTURE.md) so sánh đủ ba đường; các band effort dưới **chỉ dành cho đường 3D đề xuất**, chưa phải dự toán thay thế cho 2D.

## 3. Tech stack dùng chung

| Phần | Đề xuất cho proof/demo | Trạng thái thật |
|---|---|---|
| Engine | Unity **6000.6.7f1** đã có, C#; bắt đầu URP Forward, material ít, ít đèn/bóng realtime | Editor có evidence kernel trước đây; client hiện chỉ README/layout, chưa manifest/lock/scene/playable build |
| Camera/input | Script camera nhỏ; Unity Input System. Cinemachine khi cần chuyển/blend shot | Package phải kiểm tương thích và pin bằng Editor/Package Manager, chưa cài ở client |
| Nhân vật | Model 3D cách điệu, rig/Avatar phù hợp, Animator, clip dùng lại theo body group, socket đạo cụ | Chưa có bộ nhân vật của ba mẫu; auto-rig không thay pose/contact/retarget QA |
| Môi trường | Kit tường, góc, dốc, cầu, cọc, giàn; prefab/material family; lighting có kiểm soát | Không cần mỗi bản sao ván/cọc là một asset độc lập |
| UI | **uGUI + TextMeshPro** làm hệ chính ban đầu; font Việt thật, HUD/hội thoại/nhật ký/action receipt | UI Toolkit là phương án hợp lệ nếu proof cho thấy lợi ích; không dựng hai hệ song song từ đầu |
| Simulation | Foundation/Domain/Application/Infrastructure `.NET Standard 2.1`, adapter Unity mỏng; JSON/content registry có version | Tái sử dụng nền đã có; generator cuộc đời, repair gameplay và actor query cần phần triển khai riêng |
| Persistence | Adapter theo contract; kiểm SQLite/provider/reopen trên target Unity thật | Headless persistence và Editor kernel conformance không chứng minh Player save/load đã đạt |
| Asset offline | Khi được giao tạo model thật: **Hyper3D MCP → cleanup/UV/rig kiểm trong Blender hoặc công cụ phù hợp → import Unity → prefab/clip QA** | Lúc kiểm kê đã thấy metadata chín tool; chưa xác nhận OAuth/credits/job/mesh/rig quality bằng preflight; không sinh model |

URP là pipeline Unity hỗ trợ nhiều nền tảng; lựa chọn Forward và budget nhẹ ở đây là suy luận theo máy dự án. Unity 6000.6 liệt kê uGUI là khuyến nghị runtime, UI Toolkit là lựa chọn thay thế. [URP](https://docs.unity.com/en-us/engine/6000.6/manual/render-pipelines/universal-render-pipeline/urp-introduction), [so sánh UI](https://docs.unity.com/en-us/engine/6000.6/manual/uitoolkits/ui-system-compare).

Tài liệu package dùng để kiểm khả năng, **không là lockfile đã cài**: [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/index.html), [Cinemachine](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/index.html), [AI Navigation](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/index.html). NavMesh nếu dùng chỉ giải đường/hiển thị cục bộ theo contract; không thay route graph, WorldTick, quyền dùng vật liệu hoặc authoritative movement rule. Khi chuyển động có hậu quả gameplay, contract phải xác định điều đó trước.

Máy hiện tại được kiểm kê: **i5-8400, khoảng19GiB RAM, Intel UHD630**. Chưa benchmark Unity3D cho ba hướng. Đề xuất bắt đầu proof ở **1280×720, mục tiêu thử30fps**, ghi frame time và bộ nhớ thật; tăng chất lượng/crowd sau đo. Không hứa UHD630 chạy nguyên panorama ở chất lượng PNG. Capture độ phân giải cao không được dùng làm bằng chứng FPS.

Trong lúc hoàn thiện báo cáo, namespace Hyper3D bị gỡ khỏi inventory của phiên. Không có generation/preflight được chạy nên chưa kết luận trạng thái dịch vụ; task model thật sau này cần kiểm lại khả dụng/kết nối trước khi dùng forecast.

Không cần backend online, runtime LLM, Unreal/HDRP hoặc custom render pipeline cho mục tiêu này. Hyper3D và image generation là công cụ sản xuất offline; bản game vẫn theo baseline offline/deterministic.

## 4. Nếu triển khai, từng hướng được làm thế nào?

### Đường chéo dài: kiểm quy trình đầu tiên

Chốt tỷ lệ người–lan can–ván và hai camera ứng viên; tạo một bộ đoạn cầu, đầu nối, cọc/lan can và vách đất đủ tạo một đoạn có điểm gặp. Tái dùng kit nhưng thay cụm và silhouette để tránh cảm giác hành lang copy-paste. Một player và một NPC kiểm đứng, đi, quay, mang giỏ/cầm vật; chân không xuyên mặt cầu, tay/vật không trôi. Nhân vật được nhận ra bằng khối thân, tóc, trang phục và cử chỉ, không cần da thật.

Kế tiếp kiểm click đúng mặt cầu và đúng entity, mép màn hình, UI mở khi người đứng hai đầu; vùng nền bên dưới không nhận nhầm click. Đường chéo là bố cục camera, không ép mọi bản đồ thành một cây cầu hoặc đổi topology thế giới. Nếu nối demo sửa thuyền, phải thêm điểm bến/thuyền và lý do địa lý hợp lệ; kit cầu **không tự thay thế nội dung Bến Vạc**.

### Hành lang thành đất: kiểm chất lượng cận

Bộ nền chỉ cần vài tường, góc, ramp và sàn; đầu tư chính vào hai body/face đại diện đúng tuổi. Kiểm đi, quay, hội thoại và ôm hai thanh gỗ ở chính cự ly ảnh17. Portrait nên lấy từ cùng nhân vật để không lệch identity. Chưa yêu cầu lip-sync, cloth simulation hoặc diễn xuất điện ảnh trong band này.

Nếu muốn effort thấp hơn, có thể giản lược mặt/vải theo grammar khối của Đường chéo; đó là thay đổi target cần user chốt, không âm thầm giảm chất lượng để giữ lịch. Camera phải đọc được người khi sát tường và khi UI che một phần khung. Nền đơn giản là lợi thế, nhưng lỗi vai/cổ/ngón tay sẽ lộ rõ nhất ở hướng này.

### Đất dựng thành: kiểm quy mô sau

Chia công trường thành kit tường/góc/bậc/dốc, mặt đi, cầu nối và giàn; thử một mảnh đại diện với hai actor tiền cảnh trước. Tổ chức lớp cao độ và selection surface để không click qua cầu xuống đáy hào. Khi khối thành che người, chỉ fade/cutaway trong giới hạn thông tin được phép thấy; không làm lộ NPC bí mật.

Sau khi fragment đạt mới đo và tăng mật độ người, clip lao động và trạng thái công trường. Người xa có thể giảm chi tiết/animation rate sau kiểm độ đọc; không mặc định mọi NPC đều được mô phỏng bằng animation. Xây thành, vận chuyển đất và tiến độ công trình là scope gameplay riêng. **Ước lượng M1/M2 dưới đây không bao gồm nguyên panorama với mọi người lao động hoặc một hệ xây thành hoàn chỉnh.**

### Luồng nối luật game cho cả ba

```text
Input/UI → intent/preview/command → Application + Domain
         → validate/rules/atomic commit → receipt
         → actor-scoped query → presenter → scene/animation/UI
```

UI không nhận omniscient snapshot rồi tự giấu; animation/physics không ghi state. Random origin được sinh hợp lệ và reveal, không chọn/chỉnh xuất thân/reroll. Phải chốt movement/anchor/time/interrupt semantics trước code gameplay, không dùng camera hay raycast để tự quyết luật còn thiếu. Các baseline tiếp tục theo [Client UX](../../CLIENT-UX-BASELINE.md) và [Applicable Baseline](../../../governance/APPLICABLE-BASELINE.md).

## 5. Effort và thời gian dự kiến

**Giờ công AI** là tổng thời gian các actor thực sự làm tác vụ, kiểm và sửa. Ba actor cùng làm một giờ tính ba giờ công; thời gian chờ không cần giám sát tách riêng. Đây là đơn vị lập kế hoạch, chưa đo throughput thực của Sol high cho pipeline này. Các band đã chứa review/rework theo gói; không cộng lại một hệ số sửa lỗi mặc định. Đầu cao không phải worst-case được bảo đảm nếu gate asset thất bại kéo dài.

| Mốc lũy kế từ hiện trạng | Đường chéo dài | Hành lang thành đất | Đất dựng thành |
|---|---:|---:|---:|
| **M1 — cảnh thử mỹ thuật có chuyển động** | **86–182 giờ** | **96–204 giờ** | **100–215 giờ** |
| **M2 — demo chơi nhỏ** | **320–662 giờ** | **330–684 giờ** | **346–719 giờ** |
| **M3 — bộ style/kit mở rộng giới hạn** | **456–942 giờ** | **474–972 giờ** | **498–1023 giờ** |

- **M1:** một cảnh nhỏ, hai actor/hai dáng người,3–4clip dùng chung, một vật cầm, portrait/UI thật đọc được, camera/build xem được. Chưa luật repair/origin/save gameplay. Đất dựng thành chỉ fragment, không full panorama/crowd.
- **M2:** nội dung mục tiêu khoảng20phút cần playtest; một PC + tối đa4NPC hoạt động nhìn thấy,2body/4looks,6–8clip, khoảng20–30loại module/prop. Nhận nhân vật ngẫu nhiên có nhân quả trong profile giới hạn → quan sát đúng tri thức → sửa thuyền hoặc chờ → hậu quả → save/load thật. Chưa full life generator hoặc full vertical slice.
- **M3:** đề xuất3–4body,12–20looks,10–14clip,40–60module/prop, import/style/UI contracts và validation dùng lại. Chưa campaign29module,12vùng hoặc1.000nhân vật có ý nghĩa.

**M2 chuyển ngôn ngữ hình ảnh về Bến Vạc**, không chuyển Bến Vạc thành công trường/cầu-vách. Bộ20–30module/props phải gồm phần bến và thuyền phục vụ repair; không giả kit thành đất đã bao hết cảnh bến. Nếu muốn demo tại đúng môi trường ba PNG, cần scope/contract và re-estimate riêng.

Root/critic phát hiện WBS đầu còn thiếu công mở art pack M1→M2. Bản hiện tại đã tách thêm28–60h chung cho hai looks, clip/portrait/icon mới và import QC; thêm kit bến/thuyền theo hướng. Không nhét công sản xuất asset vào gói E2E để giữ tổng cũ. Thuyền/bến phải vừa trần khoảng20–30unique pieces tính cả phần M1 dùng lại; hai looks mới dùng hai body/rig cũ, chưa thêm body/lip-sync/cloth. Nếu không đạt các giả định đó, re-estimate.

**Cách ra tổng:** M1 chung68–142h + phần riêng18–40 /28–62 /32–73h. M2 cộng phần chung210–424h (gameplay/integration182–364 + art expansion28–60), phần riêng integration6–16 /8–20 /14–32h và chuyển kit về bến/thuyền18–40 /16–36 /22–48h. M3 cộng chung120–240h và phần riêng16–40 /24–48 /32–64h. Chi tiết từng work package ở [03 — Effort model](art-feasibility/03-EFFORT-MODEL.md); không cộng các tổng lũy kế với nhau.

Nếu thử cả ba M1 trên cùng actor/UI/harness, phép cộng lý tưởng chỉ là **146–317h** (68–142 cộng ba increment), trước chi phí ghép và review chéo chưa dự toán. Đây chưa phải một estimate triển khai cả ba, vì các target camera/fidelity khác nhau có thể đòi thêm asset. Không cộng86+96+100 rồi gọi đó là giá tối thiểu của một game.

### Quy đổi để hình dung lịch

`Q` là **tổng giờ công AI hữu dụng mỗi ngày của cả nhóm**. Không phải giờ online, số slot, entitlement của Plus hoặc6×năng lực một actor. Bảng sau chỉ là `ceil(effort/Q)`, **chưa cộng bottleneck tuần tự và phần chờ không chồng lấp**.

| Hướng/mốc | Q=4 giờ/ngày | Q=8 giờ/ngày | Q=12 giờ/ngày |
|---|---:|---:|---:|
| Đường chéo — M1 |22–46 ngày|11–23 ngày|8–16 ngày|
| Hành lang — M1 |24–51 ngày|12–26 ngày|8–17 ngày|
| Đất dựng thành — M1 |25–54 ngày|13–27 ngày|9–18 ngày|
| Đường chéo — M2 |80–166 ngày|40–83 ngày|27–56 ngày|
| Hành lang — M2 |83–171 ngày|42–86 ngày|28–57 ngày|
| Đất dựng thành — M2 |87–180 ngày|44–90 ngày|29–60 ngày|

Đây là **ngày có năng lực Q như giả định**, chưa áp lịch5/7ngày một tuần. VớiQ8, M3 quy đổi57–118 /60–122 /63–128ngày, còn điều kiện như trên. Đường phụ thuộc thực gồm asset usable → rig/clip → camera/art proof → contract gameplay → origin/repair/projection → binding → save/load/E2E. Nhiều worker vẫn phải chia quota và tránh cùng sửa scene/Library; không chia tổng lịch thẳng cho sáu.

**Độ tin cậy: thấp**, nhất là M3; phần tăng M3 chưa có WBS chi tiết, mới là khoản dự phòng quy mô để bàn kế hoạch, không lịch triển khai đã chốt. Các khoảng chồng nhau nhiều; không kết luận Đường chéo chắc chắn nhanh hơn Hành lang đúng một số ngày. Toàn game chưa đủ WBS/content throughput để có ngày hoàn thành đáng tin. Mốc Sprint15–16 trong [roadmap](../../PLAYABLE-ROADMAP.md) là forecast theo gates, không có độ dài sprint cố định để biến thành ngày; cần reforecast sau chốt Sprint13 và hiệu chỉnh pipeline.

## 6. Codex Plus + GPT-6.1 Sol high giải quyết gì?

Theo official OpenAI documentation được kiểm ngày02/11/2026, GPT-6.1 Sol hỗ trợ mức reasoning **high**. Nó hỗ trợ input ảnh, nhưng không biến một PNG thành mesh/rig production bằng chính model; image generation là công cụ riêng. Vì vậy Sol high phù hợp để phân rã việc, code, viết script chuẩn hóa, đọc ảnh, kiểm kết quả và điều phối; nguồn asset/rig và QA vẫn là các bước riêng. [GPT-6.1 Sol](https://developers.openai.com/api/docs/models/gpt-6.1-sol).

Plus có allowance chịu ảnh hưởng bởi model và độ nặng tác vụ; local/cloud dùng chung allowance, có thể có giới hạn tuần. Không có dashboard quota thực của tài khoản trong buổi này. Vì vậy **không thể suy Plus = chạy liên tục24/7 hoặc Q8/Q12 chắc chắn đạt**. Một critic thực tế đã dừng vì usage limit ở lượt đầu và tiếp tục được sau đó; gián đoạn này không phải benchmark định lượng throughput. [Codex pricing and usage](https://learn.chatgpt.com/docs/pricing).

Chưa có số liệu chứng minh high giúp hoàn thành pipeline này nhanh hơn một tỷ lệ cụ thể. Ước lượng trên là phán đoán ROM theo công việc, không kết quả benchmark model. Hyper3D credits/queue/licensing là nguồn lực bên ngoài cần xác minh khi giao asset task; không coi chúng đã được bao gồm hay đủ dùng chỉ vì có Plus. Không đề xuất mua gói khác hoặc chi ngân sách trong buổi đánh giá.

## 7. Bước hiệu chỉnh đề xuất — chưa triển khai

**M0: cap12–24giờ công AI**, trước khi chốt lịch M1/M2. M0 lấy từ các gói scope/bootstrap/asset của M1 nếu dùng lại kết quả, không cộng hai lần cùng công; nếu thất bại buộc làm lại thì cập nhật estimate. Không hứa cap này đủ ra cảnh đẹp hoặc hai actor usable. Nếu tool/mesh/rig không qua gate, kết thúc bằng evidence thất bại và estimate sửa, không tự mở rộng vô hạn.

| Task đề xuất | Input → output | Acceptance |
|---|---|---|
| T0 — đóng scope/camera | Ba mẫu, baseline, target máy → một asset/camera contract giới hạn | Chỉ rõ fidelity giữ/giản, input/output, mock so với authoritative; user chưa chọn thì không gọi target final |
| T1 — preflight asset | Hyper3D MCP + nguồn hợp lệ → job/source/provenance hoặc lỗi thật | Kiểm result tải/import được, credits/licensing; metadata tool không thay connection test; không tự thay generator |
| T2 — một body/pose | Mesh nguồn → cleanup, scale, rig/clip/contact test nếu asset usable | Kiểm hai pose, feet và vật cầm ở camera gần/xa; chưa qua gate thì ghi chưa đạt |
| T3 — scene/camera | Một mảnh kit → hai camera ứng viên, UI text thật, capture/build nếu đủ ngân sách | Click đúng tầng, đọc ở720p/tăng chữ; frame-time/memory thật, không nhận ảnh tĩnh thành playable proof |
| T4 — cập nhật lịch | Log active/wait/retry/reject → Q quan sát, estimate từng gói mới | Tách thời gian chờ, agent-hours và elapsed; số đo chỉ đại diện scope đã chạy |

Có thể tách việc song song sau contract: worker asset/character; worker scene/camera; worker UI/presenter contract. Scene/Library có một owner. Critic kiểm asset/ảnh thực và critic kiểm contract/gameplay theo lượt khi khả dụng; root kiểm lớp đầu. Nếu thiếu critic do quota thì ghi thiếu, không bịa independent pass. Toàn bộ vẫn do AI làm; không biến vai art thành yêu cầu thuê nhân sự.

## 8. Giới hạn, kết quả review và trạng thái quyết định

- Không tạo artwork/model, không gọi Hyper3D generation, không chạy Blender/Unity hoặc benchmark/runtime/playtest trong SPIN-036. Kiểm repo, phần cứng và nguồn tài liệu là evidence của phân tích, không game acceptance.
- Giá trị thẩm mỹ/chất Văn Lang–Âu Lạc trong ba ảnh là lựa chọn sáng tạo còn cần source/provenance; không xác nhận kiến trúc/trang phục PNG là phục dựng khảo cổ.
- Bounded random-origin/repair/projection/movement và Unity persistence cần đặc tả/triển khai acceptance riêng; không thay bằng scripted kết quả để giữ lịch.
- Các kết quả tự kiểm và phản biện được lưu ở [04](art-feasibility/04-ROOT-REVIEW.md), [05](art-feasibility/05-CRITIC-TECH-ART.md) và [06](art-feasibility/06-CRITIC-ESTIMATE.md). Có hai critic độc lập ở các lượt sau; actor estimate-critic gặp quota ở lượt đầu rồi tiếp tục được, không xóa sự cố cũ.
- Giữ toàn bộ01–18, biến thể và ưu tiên14/1–3. Yêu cầu đánh giá ba mẫu không là chọn/bỏ một hướng, chốt stack, Final accepted mỹ thuật hoặc cho phép chuyển Sprint14. [S13-A01–A04](APPROVALS.md) giữ trạng thái theo phản hồi thật của chủ dự án.

Đề xuất quyết định tiếp theo: nếu được giao proof, dùng Đường chéo dài để hiệu chỉnh pipeline, giữ Hành lang làm bài kiểm người cận và Đất dựng thành làm bài kiểm quy mô. Chỉ sau số đo và quyết định mỹ thuật mới chốt lịch tích hợp demo; không tự triển khai từ báo cáo này.
