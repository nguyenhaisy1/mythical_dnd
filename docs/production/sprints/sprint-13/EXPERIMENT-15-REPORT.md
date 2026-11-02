# Sprint13 — Thử nghiệm15: ba hướng ưu tiên và ba cách nhìn mới

Mở/tạo ảnh30/10/2026 · Hoàn thiện bàn giao02/11/2026 · SPIN-032 · **Đã có output để xem, Pending user approval**.

Chủ dự án đánh giá rất tốt ba hướng đầu của vòng14 — **Đất dựng thành, Ô đất sống và Làng vòng cung** — và cho phép đào sâu, đồng thời muốn xem thêm những ý tưởng táo bạo hơn. Vòng15 được giao sáu ảnh: ba nhánh mở từ các hướng được thích và ba ngôn ngữ hình ảnh mới. Mục tiêu là thử xa hơn ở cấu trúc không gian, camera, sự hiện diện của nhân vật và quan hệ giữa UI với thế giới.

**Kết quả:** sáu frame chính và một biến thể đa diện được giữ, cùng prompt/provenance/gallery đã lưu. Có phép cuộn thể hiện rõ hơn vòng14, bố cục đứng và các cách đọc mới; một số hướng còn lệch camera/độ giản lược/độ đọc. Ba ưu tiên14/1–3 là tín hiệu định hướng có thật, không được bỏ qua hoặc nâng thành final art approval.

## Phản hồi đã ghi nhận và ranh giới quyết định

| Hướng vòng14 | Phản hồi của chủ dự án ngày30/10 | Trạng thái hiện hành |
|---|---|---|
| 14/1 — Đất dựng thành (`earth-monument`) | Rất tốt, có thể triển khai thêm | Ưu tiên đào sâu bằng ảnh xem trước; chưa chốt art cuối hoặc triển khai gameplay/model |
| 14/2 — Ô đất sống (`living-grid`) | Rất tốt, có thể triển khai thêm | Ưu tiên đào sâu bằng ảnh xem trước; chưa chốt camera/engine hoặc hệ thống ô di chuyển |
| 14/3 — Làng vòng cung (`round-world`) | Rất tốt, có thể triển khai thêm | Ưu tiên đào sâu bằng ảnh xem trước; camera cong còn là ý định cần kiểm |

Các hướng14/4–6 và mọi bản trước tiếp tục nằm trong thư viện. User chưa nói loại chúng hoặc chọn một hướng duy nhất. Quyền chốt S13-A01–A04 vẫn ở chủ dự án; không suy lời khen ba mẫu thành duyệt Sprint13 hoặc quyền chuyểnSprint14.

## Phạm vi và sản phẩm được giao

- Thư mục làm việc: `prototypes/sprint13/experiment-15/`; sáu PNG mới bằng built-in `image_gen`, mỗi hướng một màn hình chơi đề xuất.
- Sáu frame chính ở `assets/{key}.png`, đi cùng prompt thực tế, provenance và gallery. Không dùng ảnh tham chiếu trong sáu lượt tạo mới được giao; quan hệ với shortlist là quan hệ ý tưởng, không phải bằng chứng reuse hoặc matching asset.
- Kiểm ngôn ngữ chung qua cảnh, nhân vật/portrait, vật hữu dụng hoặc dấu hành động và UI trong từng hướng; không áp công thức board65/35 hoặc số ô asset cố định.
- Chỉ tạo ảnh để xem trước. Không model/scene3D thật, Hyper3D/Blender, animation, camera runtime, shader production hoặc gameplay implementation.
- Cast là ví dụ một cuộc đời được nhận ngẫu nhiên, không character creator hoặc protagonist cố định. UI chỉ là minh họa, không ghi authoritative state hoặc mở hidden truth.

## Sáu hướng cần thử

| Key | Hướng | Nhánh và thay đổi cần nhìn thấy | Giới hạn không được diễn giải thành luật game |
|---|---|---|---|
| `vertical-earth` | Thành dựng đứng | Mở từ14/1: khối công trình/đất đắp được kéo theo phương đứng, tầng và hiên mở xếp lớp; người và UI phải còn đọc được trong quy mô lớn | Phóng đại/bố cục đứng là lựa chọn thị giác, không xác nhận cấu trúc khảo cổ, địa hình canon hoặc đường đi có thật |
| `tidal-mosaic` | Ghép đảo thủy triều | Mở từ14/2: nhìn từ trên vào các mảng ghép địa bàn/đảo/kênh và cộng đồng quanh thuyền; tỷ lệ người, footprint và UI theo cùng hệ hình | Mảng/ô hình là tổ chức mỹ thuật, không tự thêm hex movement, tile collision, luật thủy triều hoặc quyền quản lý toàn tri |
| `rolling-world` | Làng trên vành trống | Mở từ14/3: khối mềm và phép nhìn cuộn/cong phải hiện rõ hơn qua horizon, hướng khối/người và UI | Đây là đề xuất camera/presentation; không thế giới hình cầu, mặt trống vật lý, mô hình thiên văn hoặc world geometry mới |
| `cubist-space` | Địa hình đa diện | Hướng mới: nhiều cách nhìn cùng được tổ chức trong một cảnh2D, mặt phẳng/người/UI có grammar đa diện rõ | Không coi phối cảnh bất thường là phép di chuyển bất khả thi đã hợp lệ, bản đồ thật hoặc thay World Route Graph |
| `shadow-world` | Làng của bóng | Hướng mới: khối3D gần trắng và bóng tiếp xúc làm rõ cấu trúc, nhân vật, vật và UI; thử vai trò của khoảng trống/ánh sáng | Không biến bóng thành thực thể ma thuật, truth detector hoặc quyền biết vị trí/nguyên nhân mà nhân vật chưa biết |
| `mirror-water` | Thủy kính | Hướng mới: camera/game view tổ chức qua mặt nước/phản chiếu với ngôn ngữ đồ họa riêng, người và hành động vẫn nhận ra | Không portal, remote camera, tiên tri hoặc nguồn thông tin bí mật; hiệu ứng hình không tạo canon hay vật lý mới |

Tên gọi và hình ảnh là đề xuất sáng tạo. [Workshop đồ họa](ART-DIRECTION-RETHINK.md) và phần nguồn ở đó cung cấp cách tách hiện vật, diễn giải và hư cấu; không chứng nhận mọi phục trang, công cụ, nhà hoặc cảnh trong vòng này là phục dựng Văn Lang–Âu Lạc. Cần giữ đời sống, nghề, gia đình và quan hệ trong hình ảnh, không để thử nghiệm không gian xóa con người khỏi RPG.

## Cách đối chiếu

Ba hướng đầu có quan hệ ý tưởng với shortlist vòng14. Review cần chỉ ra điều giữ lại, điều biến đổi và chi tiết mới thực sự nhìn thấy; không dùng tên giống hoặc lời hứa camera để báo đã đào sâu thành công. Đặc biệt,14/3 từng chưa thể hiện rõ camera cong: vòng15 phải được quan sát trực tiếp thay vì tự coi từ “rolling” là bằng chứng.

Ba hướng sau mở thêm cách nhìn. Chúng có thể thay nhiều trục đồng thời như camera, bố cục, tỷ lệ, ánh sáng, cast, palette và UI. Phản hồi ban đầu chọn được một tổ hợp để tiếp tục nghiên cứu, chưa quy thích/không thích cho riêng một trục. Không cần giữ cùng cảnh/cast của vòng13, cũng không mặc định những hướng ngoài shortlist đã bị loại.

Với các phép nhìn khác thường, cần phân biệt hình thức diễn đạt với thông tin được phép và không gian gameplay có thẩm quyền. Một khung gương/bóng/đa diện chỉ có giá trị xem ý tưởng khi vẫn đọc được ai đang được nhập vai, người có thể tiếp cận, việc đang diễn ra và thông tin UI minh họa. Chưa có implementation để kiểm projection hoặc navigation thật.

## Phân công được giao

| Actor | Phạm vi |
|---|---|
| `/root/s13e15_direction` | ART-DIRECTION.md và README; quy tắc hình, liên hệ shortlist, delta và câu hỏi review |
| `/root/s13e15_gallery` | Gallery sáu hướng, trace ba nhánh/ba hướng mới, ảnh toàn khung, controls và thư viện01–14 |
| `/root/s13e15_docs` | Hồ sơ draft này, HANDOFF, sprint, approval register, changelog và export inputs |
| `/root` | Prompt, tạo ảnh, provenance, tích hợp, kiểm lớp đầu và điều phối hai critic sau workers |

Ba worker đã bàn giao đúng scope; root cập nhật quan sát/artifact sau generation. Hai critic thực `/root/s13e15_critic_code` và `/root/s13e15_critic_visual` kiểm sau workers và ghi kết quả riêng. Các vai trò chạy theo lượt trong giới hạn bốn tác nhân cùng hoạt động, không có sáu tác nhân đồng thời. Self-review root khác independent review.

## Output và quan sát thực tế

Bảy PNG **1672×941 RGB** tạo ngày30/10/2026 bằng built-in image_gen. Sáu khung trưng bày và `cubist-space-v1.png` giữ qua link riêng đều nguyên bytes từ generated_images, không crop/hậu kỳ. Sáu prompt đầu và một prompt thử lại độc lập ở [prompts.json](../../../../prototypes/sprint13/experiment-15/prompts.json); [provenance.json](../../../../prototypes/sprint13/experiment-15/provenance.json) ghi nguồn/SHA/mode/bytes cùng `prompt_collection`/`prompt_key`. Bản đa diện đang xem dùng iteration/cubist-space-v2, bản v1 dùng prompt chính/cubist-space. Cả bảy lượt không có ảnh input; “mở từ14” là quan hệ ý tưởng, không phải reference ảnh hoặc cùng geometry/cast.

| Frame | Đã quan sát | Giới hạn / câu hỏi chưa đóng |
|---|---|---|
| Thành dựng đứng | Tầng sàn, cột và lối lên tạo nhịp theo phương đứng; người dáng dài, UI dọc và quy mô nối với14/1 | Texture/gỗ/dây/đồ còn dày, kiến trúc phóng đại; không khảo cổ Cổ Loa. Câu hỏi trên callout chưa xác định rõ người nói |
| Ghép đảo thủy triều | Mảnh đất/kênh/bến rõ, cast và UI cùng màu/mặt hình | Output là góc chéo có khối, chưa top-down phẳng/mosaic như brief. Không luật hex/địa hình di chuyển/thủy triều runtime |
| Làng trên vành trống | Cung nền, horizon, hướng cột/nhà và UI cùng uốn, rõ hơn bản14/3; người già trao giỏ đọc được | Mật độ người/nhà/vật liệu còn cao; trái tim/marker chỉ minh họa, không tự thêm hệ quan hệ. Không chứng minh camera động hoặc thế giới hình cầu |
| Địa hình đa diện — biến thể trưng bày | Mặt bằng, mặt đứng, mặt nghiêng và nhân vật ở nhiều tỷ lệ ghép mạnh hơn v1; vùng UI cùng mảng | Nhân vật/gương mặt lặp ở nhiều cự ly có thể đọc như portrait/panel hơn không gian liền. Chưa chứng minh nhiều lối đi hợp lệ hoặc cùng geometry |
| Làng của bóng | Mặt trắng, bóng tiếp xúc và điểm xanh tạo phân cấp; con người/canoe/nhà/UI cùng tông | Hình và texture vẫn gần làng quen, chưa đạt tái thiết kế hình hoặc chủ yếu chỉ đọc bằng bóng như brief. Không ma thuật/truth detector |
| Thủy kính | Phản chiếu chiếm phần lớn khung, có người ở mép bờ và UI/portrait thuận chiều; nhịp gợn tạo hướng nhìn khác | Quang học và hướng người/đánh dấu còn khó đọc, icon túi đi cùng Nhật ký chưa nhất quán; không coi reflection là remote view hoặc thông tin ngoài tri thức |
| Đa diện v1 — giữ riêng | Người/vật/cảnh có mảng góc cạnh và UI cùng họ | Phối cảnh vẫn gần bình thường, root thử thêm biến thể để đổi cấu trúc. Giữ v1 để tham khảo, không gán user đã loại hoặc chê |

Mức thay đổi chưa đồng đều: cuộn có dấu hiệu thị giác rõ, khối đứng thay bố cục; mosaic và trắng/bóng chưa đổi mọi quy tắc đã đặt ra. Đây là quan sát trên output, không thay phản hồi chủ dự án hoặc chứng minh “điên rồ hơn” theo một thang điểm. Một khung không kiểm toàn bộ roster, UI túi đồ/nhật ký hoặc những tình huống đời sống khác.

Mọi con người, trang phục, công cụ, motif và địa hình là ví dụ mỹ thuật; không nhập vào canon/content pack hoặc khẳng định phục dựng. Các phép cong, đa góc, bóng, phản chiếu là đề xuất cách vẽ, không sửa physical truth/route/knowledge/ownership. Chưa có model, scene, camera/runtime, animation hay gameplay mới.

## Kiểm chứng và phản biện

[Code critic15](CRITIC-CODE-15.md) đã chạy thật `python3 prototypes/sprint13/experiment-15/browser-check.py` ngày30/10: **lượt đầu exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**, HTML100/150%. Bảy PNG1672×941RGB giống nguyên bytes với original; bảy mapping prompt (sáu chính/một iteration), hash/bytes/kích thước/mode và reference rỗng đạt. Sáu card/switch/phím1–6, contain/modal/PNG gốc/Escape/focus, mười bốn historylinks, ba link mẫu14được thích và link v1 đều đạt; không JS error/request ngoài/failed request.

Evidence ignored ở `tests/results/sprint13-experiment15/browser.json` và21 screenshot. Harness preload ảnh trước capture; không sửa gallery. Ngày02/11 root kiểm sourcehash vẫn khớp evidence cũ và bảy PNG vẫn khớp provenance, nên tái sử dụng evidence trên đúng source, không nhận là một lượt browser mới. Root xác nhận lại gallery và bảy routePNG HTTP200. Code critic đã xem mobile150; root đã xem bảy output cùng screenshot gallery-rolling-world1920 và gallery-mirror-water390 trong lượt tạo ảnh.

[Visual critic15](CRITIC-UX-15.md) hoàn tất ngày02/11 sau lượt trước gặp usage-limit. Không có report hoàn chỉnh trước lần tiếp tục, nên không ghi lượt cũ là completed. Reviewer đã xem đủ bảy PNG15, ba mẫu14 được thích và ba screenshot cuối (rolling1280, cubist1920, mirror390); không tự chạy browser automation. Không thấy P1 cản xem thư viện; các P2 về camera tidal, mức đổi hình shadow, cách đọc cubist/mirror, speaker và icon được giữ trong ghi chú output và review. Root giữ khác biệt giữa tự kiểm và review độc lập. Camera cong, đa điểm nhìn, bóng và phản chiếu chỉ được nhận ở mức nhìn thấy trong PNG, không dùng nhãn hướng để suy khả năng runtime.

UI raster không reflow khi tăng chữ HTML và cần mở PNG gốc khi xem chi tiết trên màn hình nhỏ. Không OCR từng dấu, đo contrast hoặc nghiệmthu UI game. Không có runtime/Unity, model/scene/camera động, benchmark hoặc human playtest mới trong task này. Checks tài liệu và exports chạy sau khi hồ sơ cuối hoàn thiện, kết quả thực được append trong WORKLOG; không nhận container/navigation check là visual rendering hoặc game acceptance.

## Đồng bộ và bàn giao

HANDOFF, Sprint13, approval register, changelog và export inputs đã đồng bộ vòng15, giữ đúng ưu tiên14/1–3; .agent cũng ghi phản hồi này để tiếp tục đúng nhánh. Danh sách export gồm báo cáo15 và hai review15; regenerate sau khi review tồn tại và source cuối hoàn thiện. Lệnh bàn giao: build_docs --check, check_handoff, export.py rồi check_exports; kết quả thực ghi trong WORKLOG sau run. WORKLOG do root append riêng, không đưa vào exports.

Không sửa archive, generated GDD, canon/contract, save/schema/content hoặc VERSION. Không merge dev/main, tag hoặc release. Giữ01–14, shortlist và các hướng khác để so sánh/kết hợp; mọi sản phẩm chưa chốt vẫn Pending user approval, chưa đóngSprint13 hoặc chuyểnSprint14.
