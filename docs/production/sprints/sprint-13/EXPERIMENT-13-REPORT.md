# Sprint13 — Thử nghiệm13: khung chơi cách điệu sau workshop

30/10/2026 · SPIN-030 · **Đã có output để xem, Pending user approval**.

Chủ dự án giao tạo thêm ý tưởng sau [workshop đồ họa](ART-DIRECTION-RETHINK.md). Vòng13 thử ba cách nhìn bằng **màn hình chơi nguyên khung**, sau đó có một khung đối thoại riêng cho mỗi hướng. Mục tiêu là thấy rõ thay đổi về camera, hình khối, tỷ lệ người/cảnh và cách đọc tương tác; không tiếp tục dùng công thức cảnh65% cùng dải asset35% để kiểm mọi phương án.

**Kết quả:** đã tạo sáu khung đang trưng bày thuộc ba hướng và giữ một biến thể thế giới nhìn ngang trước lượt làm phẳng. Các cặp thế giới/đối thoại dùng ảnh riêng, không board65/35. Output có thay đổi hình và cự ly, nhưng chưa khớp hoàn toàn mọi ý định camera/tỷ lệ/mức chi tiết. Lời khuyến khích và yêu cầu tiếp tục không là duyệt workshop, chọn hướng hoặc kết thúc Sprint13.

## Phạm vi và câu hỏi cần trả lời

- Chỉ tạo raster bằng built-in `image_gen` để mô phỏng game có thể nhìn như thế nào; không dựng model, scene3D, Hyper3D/Blender, animation hoặc UI runtime.
- Thư mục làm việc: `prototypes/sprint13/experiment-13/`. Prompt, provenance, PNG gốc và gallery đã được lưu theo kết quả thực tế.
- **Khối Lạc:** ý định diện mạo3D khối lớn, nhìn chéo từ cao; người và cảnh cùng được giản lược mạnh, bề mặt matte và mảng bóng gọn.
- **Dải đời sống:** ý định2D/2.5D theo dải sinh hoạt, hình người biểu tượng/cách điệu, cao độ sàn–sân–bến–nước đọc bằng lớp và khoảng trống; tránh chỉ làm panorama nhìn ngang giàu chi tiết như trước.
- **Trong lòng làng:** camera gần người trong cùng họ hình khối3D cách điệu dự kiến với Khối Lạc; thử cảm giác hiện diện và đọc mặt/tay trong một cuộc gặp. Đây là đối chiếu cự ly camera, không mặc định một medium hoàn toàn khác.

3D và tả thực là hai trục riêng. Các ảnh có thể đề xuất mặt khối và che khuất nhưng chưa chứng minh geometry, camera hoặc model thật. Không gán tên hướng thành kết quả hình ảnh; cần xem trực tiếp người, nhà, thuyền, cây và UI có cùng quy tắc hay chỉ đổi một lớp phủ.

## Sáu frame được giao

| Key | Hướng / trạng thái | Ý định cần quan sát |
|---|---|---|
| `block-world` | Khối Lạc — thế giới | Một khung chơi nhìn chéo đủ đọc người chính, đường lên nhà sàn, bến/thuyền, công việc và khối cao thấp; không tách dải asset bên dưới |
| `block-dialogue` | Khối Lạc — đối thoại | Speaker, một câu đang nói và các lựa chọn tách rõ; mặt/người/công cụ và UI giữ quy tắc khối của frame thế giới |
| `frieze-world` | Dải đời sống — thế giới | Các dải/cao độ sinh hoạt và pose người dẫn bố cục; thế giới cùng giản lược, không chỉ camera ngang trên phong cảnh tự nhiên |
| `frieze-dialogue` | Dải đời sống — đối thoại | Cử chỉ, người nói và UI cùng nhịp hình phẳng; vùng chữ yên, không biến toàn cảnh thành một tấm trang trí có menu |
| `near-world` | Trong lòng làng — thế giới | Cự ly gần/tầm người cho thấy thể tích, khoảng tiếp cận và sự hiện diện; không kéo lại da/vải/vật liệu kiểu ảnh thật |
| `near-dialogue` | Trong lòng làng — đối thoại | Đọc được mặt, tay và trạng thái nói/nghe trong camera gần; cùng họ tạo hình với frame thế giới và hướng khối nhìn chéo |

Sáu file PNG trưng bày ở `assets/{key}.png`. Khung đối thoại không là asset catalogue, cũng không buộc phải có thêm sáu icon hoặc ba portrait nếu chúng không giúp kiểm nghi vấn. Scope vòng này chủ yếu kiểm thế giới–nhân vật–UI hội thoại; chưa chứng minh đủ mọi màn hình túi đồ/nhật ký hoặc bộ asset toàn game.

## Những gì giữ chung và những gì đang thay

Tình huống minh họa chung là bến có thuyền đang được người thợ làm việc, một lối lên nhà sàn và người mang hàng đi qua. Mẫu protagonist là một phụ nữ trung niên có dáng chắc/rộng vai, trang phục mảng đỏ đất/chàm và đồ mang theo; người thợ lớn tuổi dáng gầy, tóc sáng và một người khuân hàng tạo các dáng tương phản. Đây là cast nhận được để thử mỹ thuật, không cố định protagonist production hoặc tạo quyền chọn/chỉnh origin.

Khối Lạc và Trong lòng làng thử cùng họ tỷ lệ phóng đại/hình khối3D trong hai cách nhìn; Dải đời sống đổi cách tổ chức không gian và hình người mạnh hơn. Giữ vai, công việc và các vật chính giúp so sánh, nhưng mỗi frame là một ảnh riêng, dù một số dùng PNG trước làm tham chiếu, nên không hứa cùng geometry, bố trí, ánh sáng hoặc khuôn mặt tuyệt đối. Phản hồi trước tiên đánh giá từng tổ hợp. Muốn quy sở thích cho riêng camera, văn hóa hoặc medium cần phép đối chiếu có kiểm soát hơn, không suy chỉ từ một ảnh được thích.

Nguồn văn hóa và phép biến đổi lấy từ workshop: silhouette mái–sàn–cột, điểm tựa/động tác giữa người với vật/người và nhịp nhóm hình. Đây là diễn giải mỹ thuật mới; trang phục, tỷ lệ, kết cấu, công cụ và UI không được coi là phục dựng khảo cổ đã xác minh hoặc canon mới. Không đổi gameplay, route/time, knowledge isolation, ownership hoặc authoring content.

## Phân công được giao

| Actor | Phạm vi |
|---|---|
| `/root/s13e13_direction` | ART-DIRECTION.md, README; mô tả ý định, delta so với mẫu cũ và điều cần quan sát |
| `/root/s13e13_gallery` | Gallery HTML/CSS/JS để xem sáu frame theo ba hướng, nguyên khung, ảnh gốc và thư viện01–12 |
| `/root/s13e13_docs` | Hồ sơ draft này, HANDOFF, sprint, approvals, changelog và export inputs |
| `/root` | Prompt, sinh ảnh, provenance, tích hợp, kiểm lớp đầu, điều phối hai critic và bàn giao |

Ba worker đã bàn giao đúng scope; root bổ sung output/quan sát vào gallery/README/report sau generation. Hai critic `/root/s13e13_critic_code` và `/root/s13e13_critic_visual` kiểm sau workers, kết quả thực được ghi riêng. Tối đa bốn tác nhân cùng hoạt động, không có sáu vai trò đồng thời. Self-review root tách với independent review.

## Output và quan sát thực tế

Bảy PNG được tạo ngày30/10/2026 bằng built-in image_gen: sáu frame chính và `frieze-world-v1.png` được giữ để xem lại. Sáu ảnh1672×941RGB; riêng `frieze-world.png` cuối1671×941RGB. Sao chép nguyên bytes từ generated_images, không crop, sửa màu hoặc hậu kỳ bằng code. Không model hoặc scene3D mới.

[prompts.json](../../../../prototypes/sprint13/experiment-13/prompts.json) giữ sáu prompt đầu và prompt sửa trong `iterations`. [provenance.json](../../../../prototypes/sprint13/experiment-13/provenance.json) ghi SHA/bytes/mode/tên nguồn, `prompt_collection`/`prompt_key` và `reference_assets`/`reference_sha256`. Block-world và frieze-v1 tạo mới; frieze cuối dùng v1; near-world và block-dialogue dùng block-world; frieze-dialogue dùng frieze cuối; near-dialogue dùng near-world. Đây là chuỗi tham chiếu diện mạo thật đã truyền vào công cụ, không chứng minh cùng geometry hoặc một camera chạy xuyên scene.

| Frame | Đã thấy | Chưa đạt trọn brief / giới hạn |
|---|---|---|
| Khối Lạc — thế giới | Nhà nâng có khoảng trống dưới sàn, thuyền có khối rỗng, cây/lá có mặt lớn; người chính tiến tới thợ, đường đi và prompt hỏi chuyện rõ | Người lớn hơn tỷ lệ8–10% đề xuất; người/mái/nền còn nhiều chi tiết, chưa cùng mức cực giản4nhịpđầu. Không xác nhận góc35–40độ từ ảnh |
| Khối Lạc — đối thoại | Hai người quay vào nhau, thợ đưa tay, portrait khớp vai; có nhãn người nói, câu và ba lựa chọn rõ | Bố trí/chi tiết đã đổi so world; tỷ lệ vẫn không có kiểm chuẩn. Không gọi đây là camera chuyển thật hoặc state runtime |
| Dải đời sống — thế giới | Bản sửa làm phẳng nhà/sàn/đường/nước, profile người và khoảng trống rõ; nguyên khung dành cho tình huống chơi | Vẫn có chuyển sắc/mặt bóng và nhiều người/vật ở nền; chưa “chỉ12màu/hoàn toàn phẳng”. Nhịp dải là proposal, chưa chứng minh navigation/parallax |
| Dải đời sống — đối thoại | Giữ cách chia lớp và cast, hai người hướng vào nhau; portrait/người nói/câu/lựa chọn cùng nét | Thuyền/vật và vị trí đã khác world. Một số panel có chuyển sắc, không phải một hệ vector hoặc asset production đã chuẩn hóa |
| Trong lòng làng — thế giới | Người chính gần hơn, hình mang đồ và hướng tiến tới thợ rõ; cùng họ màu/khối với Khối Lạc | Camera vẫn nhìn chếch từ cao, chưa đúng tầm lưng/ngang như brief. Khác biệt là cự ly và bố cục, chưa là một style thứ ba hoàn toàn |
| Trong lòng làng — đối thoại | Thấy rõ mặt, bàn tay và mảng người; người nữ quay lưng/vài phía trước; speaker/câu/lựa chọn được tách | Background còn chi tiết; scene riêng chưa chứng minh camera, rig/animation hoặc mọi góc model. Không đo readability game chỉ từ PNG |
| Dải đời sống v1 — biến thể giữ riêng | Cùng tình huống nhìn ngang, người và cảnh đã mảng hóa nhưng còn phối cảnh sâu và detail | Root yêu cầu một lượt làm phẳng tiếp trong cùng spin, giữ v1 để tham khảo; không ghi user đã loại bản này |

Cả ba frame đối thoại có nhãn “Thợ thuyền”, câu “Ta cần thay thanh gỗ này.” và ba lựa chọn minh họa. Đây là văn bản ví dụ, không nhập vào canon/script production hoặc hiện thực repair effects. Ở cự ly gần có thể đọc tuổi người thợ qua tóc/râu/mặt/dáng; tóc/ngưỡng tuổi người nữ chưa thể coi đã khớp chính xác mô tả trung niên/tóc ngắn có vệt bạc. Cast nhận ra qua khăn/áo/túi là quan sát ở các ảnh này, không proof identity ổn định cho mọi animation/góc nhìn.

Selected-state và glyph của các nút đối thoại còn khác giữa ba frame (vòng/ô sáng/ô đỏ và icon khác nhau). Chúng cùng họ màu/khung nhưng chưa là một design system UI thống nhất, kể cả giữa hai cách nhìn3D. Không suy sự giống nhau về cast thành mọi chi tiết UI đã khớp.

Cấu trúc mái, motif cờ/vải, hình thuyền và trang phục là đề xuất sáng tạo. Sự xuất hiện của chúng không tự chứng nhận Văn Lang–Âu Lạc đúng khảo cổ; cần đánh giá nhận diện với chủ dự án. Scope này kiểm thế giới/cast/đối thoại và vài icon nhỏ; các tab Nhật ký/Túi đồ chưa là màn hình đã thiết kế đầy đủ.

## Kiểm chứng và phản biện

[Code critic13](CRITIC-CODE-13.md) đã chạy thật `python3 prototypes/sprint13/experiment-13/browser-check.py`: **lượt đầu exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**, HTML100/150%. Bảy PNG giống nguyên bytes với original; bảy mapping prompt và năm cạnh tham chiếu/hash/DAG đạt. Kiểm ba card×hai frame, sáu nút/thumbnail/phím1–6 theo thứ tựUI, ảnh contain, modal/PNG gốc/Escape/close/focus, mười hai link lịch sử và link v1; không JS error, request ngoài hoặc failed request.

Evidence ignored ở `tests/results/sprint13-experiment13/browser.json` và21 screenshot. Harness preload thumbnail trước decode/capture để không chụp ô lazy trống; không sửa source gallery. SHA nguồn trước/sau run và lúc root đối chiếu vẫn khớp. Code critic xem thật screenshot mobile150; root thực sự xem `gallery-frieze-dialogue-390.png` và `gallery-near-dialogue-1280.png`, ngoài bảy output lúc generation. Root không nhận run browser của critic thành lượt tự chạy.

[Visual critic13](CRITIC-UX-13.md) xem bảy original và hai ảnh cũ11/06C để đối chiếu. Phản biện tách tiến triển đã thấy (khung chơi nguyên ảnh, hình cách điệu, frieze đổi cấu trúc, đối thoại có speaker/câu/choices) với các câu hỏi còn mở về camera, tỷ lệ, mức chi tiết, UI và độ nhận diện thời đại. Chưa có P1 cản xem concept trong phạm vi được review; không nhận reviewer thích một chi tiết là phản hồi hoặc approval của chủ dự án. Phạm vi screenshot được xem nằm trong hồ sơ critic khi hoàn tất.

Root kiểm preview HTTP200 cho gallery và bảy PNG, `node --check`/diff check đạt. Các checks tài liệu và export chạy sau khi source/review hoàn thiện được append kết quả thực vào WORKLOG. Không chạy gameplay, Unity, benchmark hoặc human playtest cho task ảnh/gallery này. HTML150 chỉ tăng chữ trang xem; UI raster không reflow và chữ trong ảnh nhỏ khi giữ toàn khung trên điện thoại. Kiểm gallery không nghiệmthu UI runtime, ngữ pháp hình hoàn chỉnh hoặc chất Văn Lang–Âu Lạc đã được chọn.

## Tác động hồ sơ và điểm bàn giao

Đã đồng bộ HANDOFF, Sprint13, approval register, changelog và export inputs. Danh sách export gồm report13 cùng hai review13; regenerate sau khi review tồn tại và root hoàn thiện output/evidence. Lệnh bàn giao: build_docs --check, check_handoff, export.py rồi check_exports; số lượng/kết quả thực ghi trong WORKLOG sau run, không nhúng journal vào exports. Không sửa archive, generated GDD hoặc VERSION/save/schema/content versions. WORKLOG do root append riêng, không đưa vào export.

Giữ toàn bộ01–12 và workshop SPIN-029 để tham khảo, so sánh hoặc kết hợp. Thư viện13 mới không tự loại bản trước hoặc chốt một hướng. [APPROVALS](APPROVALS.md) vẫn giữ S13-A01–A04 Pending/Changes requested; không merge dev, đóngSprint13 hoặc chuyểnSprint14.
