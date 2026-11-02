# Sprint13 — Thử nghiệm17: mười hướng học từ các vòng trước

02/11/2026 · SPIN-034 · **Đã có mười frame, một biến thể và hai review; Pending user approval**.

Vòng17 được giao mười phương án để mở rộng lựa chọn: ba nhánh phát triển từ thành đất, mặt bằng và vòng cung mà chủ dự án đã thích, cùng bảy cách thể hiện khác hoặc cải tiến từ thư viện. Các tác nhân đã trao đổi để giảm trùng ý tưởng trước khi tạo ảnh. Mục tiêu là khác nhau ở cách nhìn không gian, dáng người, việc đang làm và giao diện, không chỉ thay màu hoặc bề mặt.

**Kết quả:** mười frame chính và một bản Vành sân đầu tiên được giữ, cùng prompt/provenance/gallery. Mức khác biệt và mức đạt brief chưa đồng đều; ghi riêng quan sát thật, kiểm kỹ thuật và quyết định chủ dự án. Review chỉ ghi hoàn tất sau khi có evidence. Giữ01–16, các biến thể và ưu tiên14/1–3; lời động viên/yêu cầu thêm ý tưởng không là final art approval.

## Yêu cầu, đầu vào và sản phẩm được giao

- User02/11 cho phép học hỏi/improve từ các ý tưởng cũ, brainstorming giữa tác nhân và yêu cầu **thêm10ý tưởng**. Đây là phạm vi thử ảnh trong Sprint13, không chuyểnSprint14 hoặc chọn style cuối.
- Đầu vào: chỉ dẫn hiện hành, [workshop](ART-DIRECTION-RETHINK.md), thư viện01–16, phản hồi thật của chủ dự án và findings đã ghi. Ba ưu tiên thật là14/1 Đất dựng thành,14/2 Ô đất sống,14/3 Làng vòng cung; không biến lời nhận xét của AI về16 thành sở thích user.
- Đầu ra được giao: mười frame chính ở `prototypes/sprint13/experiment-17/assets/{key}.png`, prompt/provenance, [gallery](../../../../prototypes/sprint13/experiment-17/index.html), ART-DIRECTION/README, ba note brainstorm và hai review sau output. Root đã chạy mười một lượt built-in image_gen mới, không ảnh input; bản Vành sân đầu tiên được giữ và bản thử lại dùng prompt iteration riêng.
- Một frame mỗi hướng cần thể hiện quan hệ giữa cảnh, toàn thân/cận mặt, vật hữu dụng, icon và UI ở khoảng cách chơi dự kiến. Không ép công thức board65/35, hai người/ba portrait/sáu icon hoặc cùng một HUDHTML cho mọi hướng.
- Chỉ ảnh mô phỏng diện mạo game, không model/scene3D thật, Hyper3D/Blender, engine/shader production, animation/camera động hoặc gameplay implementation.

## Brainstorm giữa các tác nhân — kết quả thực trước generation

| Actor | Note sở hữu / đóng góp đã có |
|---|---|
| `/root/s13e17_direction` | [01-SPACE](../../../../prototypes/sprint13/experiment-17/brainstorm/01-SPACE.md): thành đất gần người, mặt bằng phẳng, vòng sân và relief có chiều dày; nhận phản biện chéo |
| `/root/s13e17_systems` | [02-CHARACTER-UI](../../../../prototypes/sprint13/experiment-17/brainstorm/02-CHARACTER-UI.md): cử chỉ/tuổi, khung kiến trúc lồng, nêm ghép và quan hệ cast–UI; chủ động chỉ ra relief trùng direction |
| `/root/s13e17_audit` | [03-HISTORY-CRITIQUE](../../../../prototypes/sprint13/experiment-17/brainstorm/03-HISTORY-CRITIQUE.md): audit hồ sơ tiền lệ, failure modes và range; đề xuất camera ngang mạn, pixel gần, âm bản và phương án dự phòng |
| `/root` | Tổng hợp, gộp hai đề xuất relief thành một, chọn batch mười hướng để tạo ảnh và đặt chuẩn vai UI |

Có trao đổi thật giữa ba worker, không phải root tự viết rồi gán thành cuộc họp. Note audit ghi phạm vi đọc hồ sơ, không nhận đã mở toàn bộ PNG cũ. Root ban đầu cân nhắc Khối liền mềm; sau khuyến nghị tăng range camera, thay bằng Sát mặt nước. Khối liền mềm và Đồng bằng–người nổi vẫn ở brainstorm, chưa đưa vào mười frame; không gán user loại chúng.

Chọn mười hướng là quyết định phạm vi thử ảnh của AI, không quyền chốt art direction thay chủ dự án. Ba worker và hai critic hoạt động theo lượt trong giới hạn bốn concurrent; không có sáu tác nhân chạy đồng thời.

## Mười hướng và delta cần nhìn thấy

| Thứ tự / key / tên | Liên hệ ý tưởng trước | Quy tắc cần thử và phần chưa được phép suy thành gameplay |
|---|---|---|
|01 · `earth-corridor` — Hành lang thành đất | Nhánh ưu tiên14/1; học quy mô15đứng và giản lược16gấp | Góc thấp3/4, người đủ lớn giữa vài khối thành đất và lối sinh hoạt; giảm texture. Không kiến trúc Cổ Loa đã xác minh hoặc navigation graph mới |
|02 · `river-negative` — Bờ liền nét | Nhánh ưu tiên14/2; đối chiếu mặt bằng16dệt | Mặt bằng90độ, đường đất/nước là hình đặc/âm liên tục, người overhead cùng phép nhìn. Không chỉ lưới, palette hoặc ô di chuyển/luật thủy triều |
|03 · `gentle-arc` — Vành sân | Nhánh ưu tiên14/3, học phép cong nhìn thấy ở15/3 | Giữ vùng gặp và người trưởng thành đọc rõ, để đường cong chủ yếu ở xa; UI theo nhịp cung có tiết chế. Không camera động hoặc thế giới hình cầu |
|04 · `living-relief` — Phù điêu đang sống | Gộp direction Một nét cắt và systems carved-relief; có tiền lệ bích họa12 | Chiều dày nông và mép slab tổ chức người, nhà, vật và UI, không shader đồng phủ lên cảnh thường. Không phục dựng nghệ thuật/hiện vật lịch sử |
|05 · `gesture-flat` — Người bằng cử chỉ | Học mảng phẳng/hoạt hình12 và cự ly người13 | Vai, tay, lưng và dáng mang khác theo tuổi/việc; hình phẳng cứng và UI bất đối xứng cùng nhịp. Không cùng avatar đổi màu hoặc NPC production đã được định hình |
|06 · `nested-frames` — Cửa trong cửa | Học cự ly13gần, tương phản lớp15–16 và khoảng trống UI | Ba khung mái/cột thật trong hình tạo che khuất và chiều sâu quanh cuộc gặp ở tầm người; UI neo vào khoảng âm. Không portal, camera xuyên vách hoặc cổng dịch chuyển |
|07 · `tangram-people` — Khối ghép đời thường | Đối chiếu đa diện15, gấp và voxel16 | Ít nêm/lăng trụ lớn cấu tạo cả cơ thể, vật và công trình; tuổi/cử chỉ còn rõ. Không chỉ low-poly shader, voxel grid hoặc phép ghép người trong gameplay |
|08 · `low-water` — Sát mặt nước | Học gần người13 và góc nhìn nghề14/4, đổi trục camera thay vì lặp góc cao | Camera ngang mạn, người/thuyền lớn ở tiền cảnh, bến và hiên thành vài lớp; UI theo cạnh mạn nhưng chữ thuận. Không underwater, reflection/remote view15 hoặc nhân vật có đặc quyền tri thức |
|09 · `close-pixel` — Pixel cự ly gần | Cải tiến pixel07/11 về khoảng cách và mật độ; phân biệt voxel16 | Sprite2D lớn đủ đọc tuổi/lưng/tay, cụm pixel tiết chế và UI có dấu; không glow hoặc filter pixel trên tranh. Không xác nhận grid asset, frame animation hoặc pixel engine |
|10 · `negative-space` — Âm bản sống | Học giới hạn làng trắng/bóng15 và khoảng rỗng16 | Một hoặc hai mặt/khối sâu nông được khoét khoảng trống để tổ chức lối và cuộc gặp; người là hình đặc. Không chỉ recolor, wireframe x-ray hoặc nhìn hidden truth |

Vật liệu và kỹ thuật có tiền lệ; không gọi bảy hướng sau là bảy thứ hoàn toàn chưa từng thử. Mười tổ hợp có thể thay nhiều trục cùng lúc. Phản hồi ban đầu đánh giá tổ hợp, chưa quy thích/không thích riêng cho camera, chất liệu hoặc palette. Tên gọi mô tả intent, không tự là output đã đạt hoặc mười đột phá.

## Bài học UI và bản sắc phải giữ khi tạo ảnh

Root đặt chuẩn cho batch: portrait đại diện **người đang nhập vai**, khớp tuổi/tóc/thể trạng của mẫu đó; corner brackets cho player khác hình bubble tròn của NPC đang nói; không thêm mini-map minh họa để giả có content projection. Chuẩn này cần kiểm trên PNG thực, không nhận prompt thành Passed. Các hướng vẫn có khung/icon/chất hình riêng, không dùng cùng một HUD trung tính phủ lên mọi ảnh.

Nghề nghiệp, gia đình và cộng đồng tiếp tục là nội dung trung tâm qua cử chỉ, vật hữu dụng và cuộc gặp. Một khung không kiểm toàn roster, mọi vùng hoặc trạng thái UI. Cast minh họa cuộc đời nhận ngẫu nhiên; không character creator hoặc nhân vật chính production cố định.

Các hướng là diễn giải/hư cấu mỹ thuật lấy cảm hứng Việt cổ. Workshop/ngữ pháp văn hóa giúp phân biệt nguồn hiện vật, diễn giải và sáng tạo; chưa có khảo cổ mới trong batch này. Nhà sàn, thuyền, nón, hoa văn hoặc màu đất không tự chứng nhận phục trang/kiến trúc/nghề đúng niên đại Văn Lang–Âu Lạc. Không thêm canon, route graph, thời tiết, vật lý hoặc quyền xem thông tin nhân vật chưa biết.

## Phân công sau brainstorm

| Actor / lớp | Ownership và acceptance được giao |
|---|---|
| `/root/s13e17_direction` | ART-DIRECTION.md/README: lineage/delta, camera/hình/cast/UI và câu hỏi review; giữ note01 |
| `/root/s13e17_systems` | HTML/CSS/JS gallery mười hướng, controls/fullframe/modal/lịch sử01–16; giữ note02 |
| `/root/s13e17_audit` | Draft report này, HANDOFF/sprint/APPROVALS/changelog/export inputs; giữ note03 |
| `/root` | Prompt/generation/provenance/tích hợp/kiểm lớp đầu, ghi WORKLOG và điều phối hai critic sau output |
| `/root/s13e17_critic_code` | Kiểm độc lập PNG/prompt/provenance/gallery, tạo evidence và report theo thao tác thực |
| `/root/s13e17_critic_visual` | Mở artwork/đối chiếu ảnh cũ/screenshot, phản biện mức đạt grammar và UX trong ảnh |

Self-review root khác independent review; kiểm tự động gallery khác human playtest. Assignment và đường dẫn dự kiến chưa tự là sản phẩm hoàn tất.

## Output và quan sát thực tế

Mười một PNG **1672×941 RGB**, tổng **22,092,490bytes**, tạo02/11 bằng built-in image_gen. Mười frame trưng bày và `gentle-arc-v1.png` giữ qua link riêng, sao chép nguyên bytes từ generated_images; không crop/resize/hậu kỳ. [Prompts](../../../../prototypes/sprint13/experiment-17/prompts.json) giữ mười prompt đầu và một iteration; [provenance](../../../../prototypes/sprint13/experiment-17/provenance.json) ghi original filename/SHA/bytes/mode/dims cùng mapping. Bản Vành sân trưng bày dùng iterations/gentle-arc-v2, v1 dùng prompts/gentle-arc; chín hướng còn lại dùng prompt chính cùng key. Cả mười một lượt mới không ảnh input, kể cả iteration. Lineage là quan hệ ý tưởng, không chứng minh ảnh tham chiếu hoặc giữ identity/geometry/cast.

Các prompt04–10 đưa quy tắc đồ họa lên trước bối cảnh trước khi chạy; JSON lưu đúng chuỗi gửi thực. Root đã mở cả mười một output trong lượt tạo, không lấy tên prompt làm bằng chứng. Bản Vành sân đầu trượt về tả thực và chưa cong; root thử lại với ít khối/mặt hơn, giữ v1 để đối chiếu chứ không gán user đã loại.

| Frame | Quan sát thật và phần còn mở |
|---|---|
| 01 — Hành lang thành đất | Tường đất và ramp lớn đưa người về cự ly gần; marker, portrait tóc ngắn và NPC chỉ đường cùng đọc được. Da, vải, gỗ và nền còn nhiều texture/độ tả thực hơn brief giản lược. |
| 02 — Bờ liền nét | Đất–nước gần mặt bằng, người nhìn từ trên và dấu chọn khớp portrait tóc xám. Cảnh vẫn có mạn thuyền, cột, bóng và cây nhiều nét; chưa phải hình phẳng liên tục90° hoàn toàn. |
| 03 — Vành sân | Biến thể trưng bày giảm mạnh texture; cột/nhà ở rìa nghiêng cùng bố cục, hai thân người giữ đứng và UI theo cung. Chưa chứng minh một phép cong thống nhất; NPC thiếu nét mặt, vật mang gần bát hơn giỏ và marker ôm cả thân. |
| 04 — Phù điêu đang sống | Người và vài mép vật/UI có cạnh dày, bóng và khe rõ. Cảnh vẫn có chiều sâu phối cảnh/vật tròn/texture dày, chưa toàn bộ là một mặt relief nông chung. |
| 05 — Người bằng cử chỉ | Vai, tay, lưng và giỏ tạo silhouette rõ; cảnh, portrait và UI cùng mảng lớn. Có chuyển sắc/khối nhẹ, chân dung gần vô diện nên tuổi/biểu cảm chưa kiểm đầy đủ; icon đồ mang theo là một vật cụ thể. |
| 06 — Cửa trong cửa | Khung cột gần che rìa, nhiều lớp cửa mở dẫn qua cuộc gặp ra sông; portrait tóc xám khớp người được chọn. Cảnh/người vẫn có texture và phối cảnh gần minh họa, chưa xác nhận đủ ba khung hình học hoặc navigation. |
| 07 — Khối ghép đời thường | Nêm/thân dài–thấp rộng và mặt góc cạnh khác nhau rõ, UI cũng thành khối nêm. Nhà, giỏ và nền còn chi tiết hơn quy tắc rất ít lăng trụ; không tangram/voxel engine thật. |
| 08 — Sát mặt nước | Mạn thuyền cắt tiền cảnh, người trong thuyền và người ở bến có mặt tựa khác, portrait/marker cùng người mang bình. Camera vẫn thấy rõ sàn từ trên và còn nhiều phản chiếu/vật liệu, chưa thật sát cao độ nước. |
| 09 — Pixel cự ly gần | Nhân vật và cuộc gặp lớn hơn trong frame, có diện mạo pixel và khung UI cùng họ. Cụm chi tiết/đồ vẫn dày, tuổi trên portrait tóc xám chưa rõ như brief; chưa kiểm grid pixel/animation/asset ready. |
| 10 — Âm bản sống | Những khoang lớn có bề dày và UI nằm trong hốc tạo nhịp nhìn khác; người là hình đặc. Bên trong vẫn là bến nhiều đồ/nhà tả chi tiết, chưa chỉ hai mặt/ít silhouette; kiến trúc âm bản là hư cấu mỹ thuật. |
| Vành sân v1 — giữ riêng | Cảnh làng/người gần tả thực, UI chạy cung nhưng chưa thấy phép cong toàn trường rõ; nhiều vật liệu/chi tiết. Là output thật chưa đạt brief, không bị xóa hoặc gán chủ dự án bác bỏ. |

Một số tiến bộ về cự ly, cử chỉ, phân biệt người và UI nhìn thấy được; không gọi đủ mười tên thành mười đột phá. Hình/cast không nhập vào canon hoặc content pack. Không có model, scene, camera/runtime, animation, shader hay gameplay mới. Khối thành đất, relief, khung lồng và khoang âm là hư cấu mỹ thuật, chưa xác minh kiến trúc/hiện vật Văn Lang–Âu Lạc.

## Kiểm chứng và phản biện

Worker gallery chạy preasset controls/layout bằng Chrome156.0.8078.79 tại1280×720/1920×1080/390×844 vàHTML100/125/150, evidence ignored `tests/results/sprint13-experiment17/worker-gallery.json`. Đây chưa là kiểm PNG decode/hash/provenance; không nhận các request thiếu ảnh lúc đó là assetcheck đạt. Root bổ sung outputnotes/linkv1 trước sourcefreeze độc lập và kiểm gallery+11PNGHTTP200.

[Code critic17](CRITIC-CODE-17.md) đã chạy thật `python3 prototypes/sprint13/experiment-17/browser-check.py`: lượt đầu exit0, Chrome156.0.8078.79. Mười một PNG giống nguyên bytes với original, hash/mode/dims/bytes và11mapping (10+1iteration)/norefs đạt. Mười card/switch/phím1–9và0=10, contain/modal/PNG gốc/Escape/close/focus,16historylinks và1v1link đạt tại1280×720/1920×1080/390×844; HTML100/125/150 tương ứng16/20/24px không tràn ngang. Không JS error/request ngoài/failed request; năm fingerprint source trước/sau và root đối chiếu cuối khớp.

Evidence ignored: `tests/results/sprint13-experiment17/browser.json` và35screenshot (30gallery+3text150+2scene gọn). Harness preload/decode thumbnail trước capture, không sửa source. Codecritic và root đã mở riêng `scene-gesture-flat-1280.png`, `scene-close-pixel-390.png`: artwork giữ nguyên khung, controls sắp trong bề rộng; UI raster vẫn nhỏ trên mobile. Root đã mở cả11output và kiểm gallery+11PNG HTTP200. Không nhận automation của critic thành run do root thực hiện.

[Visual critic17](CRITIC-UX-17.md) đã hoàn tất: mở trực tiếp11PNG17, năm ảnh đối chiếu (14earth/grid/round,15rolling,11pixel) và ba screenshot cuối (scene-gesture1280, scene-close-pixel390, gallery-negative1920). Reviewer đọc browser.json, không tự chạy automation. Không thấy P1 cản xem thư viện; P2 còn về mức đạt camera/relief/khoang âm, NPC vô diện/vật gần bát/marker ôm thân ở Vành sân, tuổi portrait pixel và mật độ/độ đọc khi thu nhỏ. Earth gần người hơn nhưng giảm panorama/công trường so với14; ảnh pixel11 là board nên không quy mọi delta cho riêng camera. Những quan sát này được giữ trong report/review, không chuyển thành quyết định chọn/loại hoặc user approval.

UI raster không reflow khi tăng cỡ chữ trang xem; chữ/icon có thể còn sai hoặc nhỏ. Chưa có OCR từng dấu, đo contrast, audit accessibility đầy đủ, navigation/knowledge projection/runtime, Unity, performance hoặc human playtest. Không nhận PNG/DOM/docs/reference checks là gameplay acceptance.

## Đồng bộ và bàn giao

HANDOFF, Sprint13, APPROVALS, CHANGELOG và export inputs đã thêm vòng17 trong scope được giao. Source exports dự kiến thêm report17 và hai review17; root regenerate khi hồ sơ cuối tồn tại và cập nhật output/evidence. Các lệnh build_docs --check, check_handoff, export.py/check_exports và kiểm phù hợp gallery chỉ ghi kết quả thực sau run vào WORKLOG; journal không nhúng exports.

Không sửa archive, generated GDD, canon/contract, engine/gameplay, save/schema/content hoặc VERSION. Không ZIP, model pipeline hoặc publish release. Giữ01–16, các biến thể và phương án brainstorm chưa đưa vào batch; ba ưu tiên14/1–3 giữ đúng mức. Tất cả output17/S13-A01–A04/Sprint13 vẫn **Pending user approval**; không merge dev/main, đóngSprint13, chuyểnSprint14, tag hoặc release.
