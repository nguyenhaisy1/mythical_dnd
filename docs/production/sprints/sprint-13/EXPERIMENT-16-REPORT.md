# Sprint13 — Thử nghiệm16: sáu quy tắc tạo hình khác nhau

02/11/2026 · SPIN-033 · **Đã có ảnh và hai review; Pending user approval**.

Vòng16 được giao để chủ dự án có thêm ý tưởng mới sau vòng15. Sáu hướng thử cách tổ chức không gian, hình người, vật dụng và UI khác nhau: dệt thành thế giới, khối bằng đường dây, không gian gấp, mưa chia hình, mặt cắt đời sống và hình khối voxel. Mục tiêu là tìm khác biệt nhìn được trong màn hình chơi, thay vì chỉ đổi màu hoặc gọi một tranh minh họa là3D.

Đã tạo tám PNG: sáu frame trưng bày và hai bản đầu được giữ. Có thay đổi nhìn được ở mặt bằng, thể tích thanh cong, độ giản lược, mặt cắt và voxel; mức đạt brief chưa đồng đều. Hai review được ghi theo thao tác thực, không nhận assignment là hoàn thành. Mọi bản01–15 được giữ; ba hướng14/1–3 tiếp tục được ưu tiên nghiên cứu, chưa được chốt mỹ thuật cuối.

## Yêu cầu, đầu vào và đầu ra

- Yêu cầu user ngày02/11: “cho tôi thêm những ý tưởng mới”. Đây là giao thêm phương án trong Sprint13, không là duyệt vòng15 hoặc chuyểnSprint14.
- Đầu vào: các chỉ dẫn đồ họa hiện hành, [workshop](ART-DIRECTION-RETHINK.md), thư viện01–15 và phản hồi thật của chủ dự án. Không mặc định toàn bộ các ý tưởng cũ đã bị loại.
- Đầu ra được giao: sáu PNG riêng ở `prototypes/sprint13/experiment-16/assets/`, prompt/provenance, [gallery](../../../../prototypes/sprint13/experiment-16/index.html), brief/README và hồ sơ review sau output. Root tạo ảnh mới bằng built-in `image_gen`; cả tám lượt không dùng ảnh input.
- Từng frame ưu tiên camera/tỷ lệ chơi và một tình huống đời sống có con người; hình người, portrait hoặc dấu nhận diện, vật hữu dụng và UI phải có quan hệ tạo hình. Không áp board65/35 hay một số ô asset cố định cho mọi hướng.
- Chỉ ảnh mô phỏng diện mạo game. Không tạo model, scene3D, Hyper3D/Blender, camera động, shader production, animation hoặc gameplay implementation.

## Sáu hướng được giao và câu hỏi cần trả lời

| Key / tên | Quy tắc cần thử trong output | Khác biệt cần kiểm, chưa phải kết luận đạt |
|---|---|---|
| `woven-world` — Thế giới dệt | Mặt bằng orthographic tổ chức bằng dải đan, thân người và vật dùng cùng logic mảnh/sợi; UI dệt theo vai trò thay vì dán texture vải lên cảnh thường | Vòng08 từng có thế giới dệt. Vòng16 phải làm rõ hình học/mặt bằng/silhouette mới, không tuyên bố chất liệu lần đầu xuất hiện hoặc chỉ đếm đây là ý tưởng hoàn toàn chưa từng thử |
| `line-volume` — Nét dây không gian | Khối và khoảng trống được gợi bằng đường dây có chọn lọc, mảng đặc dẫn mắt; nhân vật và UI có cùng quy tắc nét/khối | Đường phải tổ chức cảm giác thể tích và thông tin, không chỉ viền mọi vật, lộ tất cả cấu trúc bị che hoặc biến thành wireframe kỹ thuật/neon tương lai |
| `folded-village` — Làng gấp | Các mặt đứng/mặt sàn và nhịp gấp tạo chiều sâu kiểu accordion, nhân vật có silhouette có chủ ý và UI nối nhịp gấp | Vòng07 đã thử giấy cắt. Cần phân biệt quy tắc không gian gấp với một diorama nhiều lớp hoặc đổi vật liệu bề mặt; không suy ảnh gấp thành topology/đường đi runtime |
| `rain-sculpt` — Mưa khắc hình | Không gian góc cạnh, mảng phẳng và khoảng âm được chia bởi các dải mưa; cử chỉ người và lựa chọn vẫn là trọng tâm | Không chỉ đổi một cảnh làng sang mưa/tối, thêm hạt mưa hoặc tô màu than. Nhịp không gian/hình người/UI phải khác; chưa có weather hoặc luật tầm nhìn mới |
| `living-section` — Mặt cắt đời sống | Camera nhìn ngang tổ chức sân/hiên/công trình chung theo tầng mặt cắt, người nhiều tuổi/thể trạng đọc bằng khối rõ | Mặt cắt phải cho một cách đọc hoạt động và tiếp cận; không cấp quyền nhìn xuyên tường, tri thức riêng tư hoặc toàn bộ lịch NPC. Chỉ những gì được phép trình bày trong tình huống mới thuộc player view |
| `voxel-life` — Hạt khối | Hình khối lập phương thô có thể tích xuyên cảnh, cơ thể, đồ vật và UI; tỷ lệ/cử chỉ giữ được con người | Phải phân biệt voxel3D-look với ảnh2D có pixel filter, khối tròn mềm hoặc texture chấm. Chưa chứng minh model, voxel engine, phá hủy địa hình hoặc production grid |

Sáu tổ hợp có thể thay nhiều trục đồng thời. Phản hồi ban đầu đánh giá tổ hợp; chưa quy thích/không thích riêng cho chất liệu, camera hay palette. Tên hướng mô tả ý định. Cần xem artifact thật trước khi gọi mức khác biệt là mới/vượt bậc hoặc xác nhận ngôn ngữ toàn game đã thành công.

## Bản sắc thời đại và ranh giới presentation

Đời sống, nghề, gia đình và quan hệ cần hiện diện qua việc người đang làm và cách họ gặp nhau; không mặc định RPG là chiến đấu hoặc costume hoành tráng. Nguồn văn hóa của workshop giúp tách hiện vật, diễn giải và hư cấu. Dệt, khối dây, gấp, mưa hoặc voxel là đề xuất cách vẽ; không tự chứng nhận motif, mái, công cụ, phục trang hay kỹ thuật cụ thể là phục dựng Văn Lang–Âu Lạc.

Không thêm canon, route graph, vật lý, thời tiết, luật di chuyển hoặc nguồn tri thức. Hình dây/mặt cắt không tự cho xem hidden truth; phần bị che trong gameplay tương lai vẫn phải theo knowledge projection. Cast trong ảnh minh họa một cuộc đời được nhận ngẫu nhiên, không màn hình chọn origin hoặc protagonist production cố định. UI raster chưa ghi authoritative state, có thể còn vấn đề chữ/icon và chưa là UI runtime.

## Phân công và lớp kiểm tra

| Actor / lớp công việc | Phạm vi được giao |
|---|---|
| `/root/s13e16_direction` | ART-DIRECTION.md và README: camera/hình/cast/UI, khác biệt và câu hỏi so sánh |
| `/root/s13e16_gallery` | HTML/CSS/JS gallery: sáu hướng, xem toàn khung, điều khiển và thư viện01–15 |
| `/root/s13e16_docs` | Draft báo cáo này; HANDOFF/sprint/approval/changelog/export inputs |
| `/root` | Prompt, tạo ảnh, provenance, tích hợp, kiểm lớp đầu và điều phối review |
| `/root/s13e16_critic_code` | Kiểm độc lập PNG/prompt/provenance và gallery, ghi scope/evidence thực |
| `/root/s13e16_critic_visual` | Mở artwork/screenshot và phản biện hình/UX, ghi rõ giới hạn |

Ba worker và hai critic làm theo lượt trong giới hạn bốn tác nhân cùng hoạt động, không có sáu tác nhân chạy đồng thời. Assignment không tự là kết quả. Root self-review khác review độc lập; automated browser run khác human playtest.

## Output và quan sát thực tế

Tám PNG tạo02/11 bằng built-in image_gen: sáu frame chính, `line-volume-v1.png` và `folded-village-v1.png` giữ qua link riêng. Bảy ảnh1672×941RGB, riêng bản nét dây trưng bày1672×940RGB, tổng16,985,863bytes. Copy nguyên bytes từ generated_images, không crop/resize/hậu kỳ. [Prompts](../../../../prototypes/sprint13/experiment-16/prompts.json) gồm sáu prompt đầu và hai iteration mới; [provenance](../../../../prototypes/sprint13/experiment-16/provenance.json) ghi source filename/SHA/bytes/mode/dims cùng mapping. Bản nét dây/gấp trưng bày lần lượt dùng iterations/line-volume-v2 và folded-village-v2; hai bản v1 dùng prompt chính. Cả tám lượt mới không ảnh input, kể cả hai iteration; không nhận giữ identity/geometry/cast của bản trước.

| Frame | Đã nhìn thấy | Giới hạn / câu hỏi giữ lại |
|---|---|---|
| Thế giới dệt | Đất, sàn bến và nước tổ chức gần mặt bằng phẳng; UI theo dải sáng, màu indigo–ngà–ochre | Nhân vật/portrait vẫn là hình vẽ quen, chưa toàn thân bằng dải đan; góc nhìn người và cảnh chưa cùng phép chiếu. Dệt từng có ở08, không tuyên bố vật liệu mới hoàn toàn |
| Nét dây không gian — v2 trưng bày | Tay chân, thuyền và khung UI thành thanh cong có độ dày; cảm giác thể tích khác v1 có viền | Nhà/nền/mặt người còn đặc và texture; có cảm giác tượng khớp, chưa là không gian chỉ bằng nét và khoảng trống. Không thấy dấu chọn chân người như brief, chỉ ô đỏ UI; không wireframe engine/x-ray |
| Làng gấp — v2 trưng bày | Sàn/vách/ramp lớn, người và portrait mặt góc cạnh, nhiều khoảng yên hơn v1 | Chưa buộc đọc là một dải accordion liên tục. Hai dấu kim cương khác màu chưa rõ vai; portrait người già chưa rõ đại diện speaker hay protagonist; icon cuộn chiếu đi với Đồ mang theo có thể hiểu hẹp |
| Mưa khắc hình | Góc sau người sát thân, khối góc cạnh, lựa chọn/UI đen-vàng nổi | Mưa còn nhiều nét và nền phản chiếu có chi tiết, chưa ba dải khoảng âm. Áo tối/không gian nước cần đọc ở kích thước chơi; không weather/tầm nhìn runtime |
| Mặt cắt đời sống | Nhìn ngang làm rõ cuộc gặp trên sàn, việc bên dưới và cầu nối | Hình người/vật liệu còn gần minh họa chi tiết hơn brief. Không chứng minh orthographic tuyệt đối hoặc cho nhìn xuyên phòng riêng/lịch NPC |
| Hạt khối | Mặt lập phương có thể tích rõ trên người, cây, bờ, thuyền; khác filter pixel2D | Khối còn dày/không đều, UI có mặt phẳng và mini-map minh họa; không voxel engine, grid production hoặc phá địa hình |
| Nét dây v1 — giữ riêng | Cuộc gặp/người nói và giao diện rõ ở mức ảnh | Còn là tranh có nét viền, chưa đạt mục tiêu thanh/nét tạo thể tích. Root thử lại, không user bác bỏ |
| Làng gấp v1 — giữ riêng | Có tầng đất/sàn và cuộc gặp nghề chiếu | Còn là làng bậc tầng gần phối cảnh thông thường, nhiều texture, chưa accordion. Root thử lại, không user bác bỏ |

Root đã mở cả tám output trong lượt tạo. Tên phương pháp là intent, không nhận đủ sáu tên thành sáu đột phá đã đạt. Một frame chưa kiểm cả game, mọi roster hoặc mọi UI state. Kiến trúc, nghề, phục trang và công cụ trong ảnh là diễn giải/hư cấu mỹ thuật, chưa xác minh niên đại Văn Lang–Âu Lạc.

## Kiểm chứng và phản biện

Worker gallery chạy preasset controls/layout tại1280×720/1920×1080/390×844, HTML100/125/150 bằng Chrome156.0.8078.79; evidence ignored `tests/results/sprint13-experiment16/worker-gallery.json`. Đây chưa là kiểm ảnh decode/provenance, không nhận các request thiếu ảnh lúc đó là đã đạt.

[Code critic16](CRITIC-CODE-16.md) đã chạy `python3 prototypes/sprint13/experiment-16/browser-check.py` thật, lượt đầu exit0 bằng Chrome156.0.8078.79. Tám PNG nguyênbytes với original,8hash/mode/dims/bytes/mapping/norefs đạt. Sáu card/switch/phím, contain/modal/PNG gốc/Escape/close/focus,15historylinks và2linkv1 đạt tại1280×720/1920×1080/390×844; HTML100/125/150 tương ứng16/20/24px không tràn ngang. Không JS error/request ngoài/failed request; năm fingerprint source trước/sau khớp. Evidence ignored `tests/results/sprint13-experiment16/browser.json` và21 screenshot. Harness preload/decode thumbnail trước capture, không sửa source. Codecritic mở text150390; root mở gallery-folded-village1920 và gallery-voxel-life390, thấy ảnh nguyên khung/thumbnail hiện đủ, UI raster vẫn nhỏ trên mobile. Root cũng kiểm gallery+8PNG HTTP200 và đối chiếu fingerprint/hash/mapping.

[Visual critic16](CRITIC-UX-16.md) đã hoàn tất: mở trực tiếp tám PNG16, ba ảnh đối chiếu08dệt/07giấy/11pixel và ba screenshot cuối (fold1920, rain1280, text150390). Reviewer đọc browser.json, không tự chạy automation hoặc nhận run codecritic thành thao tác của mình. Không thấy P1 cản xem thư viện; P2 còn về mức đạt grammar, dấu nhập vai/speaker/portrait, độ dày chi tiết và UI raster nhỏ trên mobile. Screenshot mobile150 bị công cụ thu nhỏ khi xem nên reviewer không nhận đã đọc hết chữ. Các giới hạn được giữ trong output table/review, không coi tên brief là kết quả đạt hoặc phản biện nội bộ là user approval.

PNG không kiểm được chuyển động, geometry nhất quán nhiều góc, UI state hoàn chỉnh, toàn bộ cast hoặc toàn bộ game. Không nhận ảnh/DOM/docs check là gameplay/Unity acceptance, camera runtime, benchmark hoặc human playtest. Tăng chữ trang xem không làm chữ raster reflow; việc đọc chữ và icon cần quan sát thật, không suy từ prompt.

## Đồng bộ và bàn giao

HANDOFF, Sprint13, approval register, changelog và danh sách export đã thêm vòng16 trong phạm vi nhiệm vụ này. Source exports dự kiến gồm báo cáo và hai review16; root chỉ regenerate khi các file cuối tồn tại. Build_docs --check, check_handoff, export.py/check_exports và kiểm phù hợp gallery chạy sau tích hợp; chỉ ghi kết quả thật trong WORKLOG. WORKLOG không nhúng vào exports.

Không sửa archive, generated GDD, canon/contract, save/schema/content hoặc VERSION. Không ZIP hoặc pipeline model mới. Giữ01–15 và mọi biến thể cũ;14/1–3 tiếp tục ưu tiên đào sâu. Tất cả output16/S13-A01–A04 chưa chốt vẫn **Pending user approval**; không merge dev/main, đóngSprint13, chuyểnSprint14, tag hoặc release.
