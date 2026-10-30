# Sprint13 — Thử nghiệm14: sáu hướng hình ảnh độc lập

30/10/2026 · SPIN-031 · **Đã có output để xem, Pending user approval**.

Chủ dự án ghi nhận các ảnh vòng13 tốt nhưng còn giống nhau, chưa có nhiều khác biệt và bước tiến rõ, rồi giao thử tiếp. Vòng14 mở rộng phạm vi bằng **sáu hướng riêng, mỗi hướng một màn hình chơi**, thay vì tiếp tục dùng cùng bến sông, cast, bảng màu và UI để tạo các cặp camera. Sáu output đã được tạo để mở rộng không gian ý tưởng; độ khác biệt và mức phù hợp theo cảm nhận chủ dự án vẫn chờ phản hồi. Không suy thay nhiều trục thành tiến bộ đã được duyệt.

**Kết quả:** sáu PNG mới, một màn hình chơi mỗi hướng, cùng gallery/prompt/provenance đã lưu. Không dùng frame13 làm ảnh tham chiếu. Các trường hợp không đạt brief như camera cong, top-down phẳng hoặc dấu protagonist riêng được ghi rõ bên dưới. Lời khen vòng13 không là final approval cho vòng13, một hướng mới hoặc Sprint13.

## Phạm vi và sản phẩm được giao

- Tạo ảnh raster bằng built-in `image_gen`, lưu ở `prototypes/sprint13/experiment-14/`; mỗi ảnh ở `assets/{key}.png`.
- Sáu khung chơi mới được tạo riêng; không dùng ảnh13 làm tham chiếu và không giữ một UI/cast/palette chung làm khuôn cho mọi hướng.
- Mỗi hướng phải nối cách nhìn thế giới với con người, vật dụng hoặc dấu hành động và UI phù hợp cách biểu đạt. Không bắt tất cả ảnh có dải asset, hai mẫu toàn thân hay số icon cố định.
- Chỉ đề xuất diện mạo để xem trước; không tạo model/scene3D thật, Hyper3D/Blender, animation, UI runtime hoặc đổi engine.
- Không sửa canon, rule, world geometry, route/time, knowledge isolation hoặc content registry. Nhân vật là ví dụ của cuộc đời được nhận ngẫu nhiên, không màn hình chọn/chỉnh origin.

## Sáu hướng và sự khác biệt cần kiểm

| Key | Hướng | Ý định về không gian, hình, bối cảnh và UI |
|---|---|---|
| `earth-monument` | Đất dựng thành | Diện mạo3D góc cạnh, quy mô khối thành đất/công trường Loa Vực; người có tỷ lệ kéo dài, mảng oxide–xương–than và UI dải đứng. Cần thấy khác thế giới khối nhỏ quanh một bến. |
| `living-grid` | Ô đất sống | Camera2D nhìn thẳng từ trên, mặt bằng hình học gợi không gian làm muối Hải Môn; người/vật thành hình dễ nhận, màu nước lam–kem–đỏ và UI mô-đun rõ. Không tự đổi game thành quản lý toàn tri. |
| `round-world` | Làng vòng cung | Diện mạo3D tròn mềm, tỷ lệ phóng đại, bối cảnh chữa trị vùng đồi gợi Tản Viên; camera cong và UI sáng/bo tròn. Độ cong là phép trình bày đề xuất, không khẳng định thế giới là một hành tinh nhỏ hoặc đổi địa lý canon. |
| `craft-first-person` | Đôi tay làm nghề | Góc nhìn thứ nhất qua bàn tay/công việc trong xưởng gợi Đồng Đỏ; hình2D đồ họa mạnh, đường chéo/cử chỉ lớn, màu xanh sâu–trắng–lửa và UI động từ theo cột. Không nhập thêm cơ chế chế tác hoặc thao tác authoritative state. |
| `glyph-world` | Ký hình | Thế giới2D bằng cụm hình/ký hiệu trong địa bàn đầm lầy gợi Đại Trạch; cảnh, người, portrait và UI cùng ngôn ngữ dấu hình. Ký hiệu là sáng tạo đương đại cho game, không tuyên bố phát hiện hay phục dựng chữ viết cổ. |
| `comic-life` | Mái nhà nhiều giọng | Đề xuất màn hình game như trang truyện nhiều khung trong một tình huống gia đình; nét mực/riso, nhân vật biểu cảm và caption/lựa chọn cùng bố cục. Các khung là ý tưởng presentation của game, không là bảng catalogue asset hay bằng chứng tương tác đã có. |

Các tên vùng nối với thế giới đã có, không tự tạo settlement, công trình, nghề hoặc sự kiện canon mới. Kiến trúc, phục trang, dụng cụ, palette và ký hiệu cụ thể vẫn là hư cấu mỹ thuật cần nguồn phù hợp khi đào sâu. [Workshop](ART-DIRECTION-RETHINK.md) và hồ sơ nguồn ở đó cung cấp cách tách dữ kiện khỏi phép biến đổi sáng tạo; không chứng nhận sáu cảnh là phục dựng Văn Lang–Âu Lạc.

## Cách so sánh trong vòng này

Vòng13 giữ nhiều yếu tố chung để đối chiếu camera/họ hình. Vòng14 cố ý đổi nhiều trục cùng lúc: camera, tỷ lệ, hình, bối cảnh, cast, màu và UI, nhằm mở rộng thư viện ý tưởng. Vì vậy phản hồi ban đầu đánh giá từng **tổ hợp**; không quy một ảnh được thích hơn cho riêng3D/2D, palette hoặc yếu tố văn hóa. Không dùng số lượng ảnh, tên hướng hoặc việc thay bối cảnh làm bằng chứng độ khác biệt đã đạt.

Review cần chỉ ra chi tiết thực sự thay đổi so với13 và giữa sáu bản: footprint/cao độ, silhouette con người/nhà/vật, mật độ chi tiết, cấu trúc giá trị sáng tối, vị trí thông tin và cách đọc hành động. Cùng một nhãn3D có thể cho những hình rất khác; một hiệu ứng camera hoặc vật liệu cũng có thể chỉ là lớp phủ nếu cấu trúc vẫn cũ.

## Phân công được giao

| Actor | Phạm vi |
|---|---|
| `/root/s13e14_direction` | ART-DIRECTION.md và README; ý định, delta và câu hỏi cần quan sát |
| `/root/s13e14_gallery` | Gallery sáu hướng, xem nguyên khung/ảnh gốc, điều khiển và thư viện01–13 |
| `/root/s13e14_docs` | Báo cáo draft này, HANDOFF, sprint, approvals, changelog và export inputs |
| `/root` | Prompt, tạo ảnh, provenance, tích hợp, kiểm lớp đầu và điều phối hai critic sau workers |

Ba worker đã bàn giao đúng scope; root thêm quan sát sau khi tạo ảnh. Hai critic thực `/root/s13e14_critic_code` và `/root/s13e14_critic_visual` kiểm sau workers, kết quả có hồ sơ riêng. Các vai trò chạy theo lượt trong giới hạn bốn tác nhân cùng hoạt động, không có sáu tác nhân đồng thời. Self-review root tách với independent review.

## Output và quan sát thực tế

Sáu PNG tạo ngày30/10/2026 bằng built-in image_gen, mỗi ảnh **1672×941 RGB**, sao chép nguyên bytes. [prompts.json](../../../../prototypes/sprint13/experiment-14/prompts.json) giữ sáu prompt khác nhau; [provenance.json](../../../../prototypes/sprint13/experiment-14/provenance.json) giữ tên file nguồn, SHA256, bytes/mode/kích thước và reference rỗng. Không hậu kỳ, không sinh model, không lấy ảnh13 làm input.

| Hướng | Đã thấy trên output | Khoảng cách với brief / câu hỏi còn mở |
|---|---|---|
| Đất dựng thành | Địa hình thành/công trường quy mô lớn, người góc cạnh/dáng dài, mảng đất đỏ và bóng than; UI dải dọc, portrait và vật dụng cùng họ khối | Mặt đất/giàn giáo vẫn nhiều texture. Không gian giống hố/lòng công trường với cầu/lũy rất lớn; không chứng nhận cấu trúc Cổ Loa thật. Generation thêm câu gọi ngoài các nhãn đã cho; chỉ minh họa |
| Ô đất sống | Ruộng muối, đường bờ và kênh làm cấu trúc nhìn từ trên rõ; một người được đánh dấu, UI sáng ở góc có portrait và công cụ | Nhân vật, vật và mái vẫn có góc xiên/khối, bóng và texture; chưa là planview2D90độ phẳng tuyệt đối. Không kiểm workflow sản xuất muối hoặc cho quyền quản lý NPC |
| Làng vòng cung | Khối người/cây/mái tròn, nhân vật già mang bó lá, người nhận phía trước; UI tròn sáng, portrait cùng hình mềm | Camera cong chưa hiện rõ, lối đi cong không chứng minh rolling-horizon. Tỷ lệ người và mật độ cây/nhà/hoa còn khác ý định cực giản; không có hành tinh/địa lý mới |
| Đôi tay làm nghề | Hai bàn tay ở tiền cảnh, người thợ đối diện và công cụ/mặt bàn rõ; chữ, người và cảnh có mảng in xanh/cam/đen mạnh | Vết in và chi tiết nền khá dày hơn brief ít texture. Khuôn/công cụ/lò là hư cấu mỹ thuật, chưa là recipe/quy trình hay thao tác chế tác đã chạy |
| Ký hình | Cảnh người, nhà, thuyền, sậy/nước và portrait/UI đều dùng dấu hình đen/kem/đỏ, đường tiếp cận đọc bằng các cụm dấu | Dạng nhà/người là biểu đạt trắc diện/hỗn hợp, không đạt top-down thuần. Đỏ xuất hiện cả ở sậy nên chưa là dấu riêng của protagonist. Không giả định đây là chữ viết cổ hoặc hệ glyph đã chuẩn production |
| Mái nhà nhiều giọng | Ba panel cùng cuộc gặp trong nhà, cận mặt/cử chỉ và bát trao tay; có lời nói, speaker, ba caption lựa chọn và icon cùng hệ mực/tím/xanh | Đồ đạc/vải/nét rất dày; khác với ý định ít detail. Người được điều khiển và speaker cụ thể vẫn cần rõ hơn khi triển khai; không tự xác nhận panel là state/timeline hoặc action đã chạy |

Sáu ảnh có khác biệt trực tiếp về cách tổ chức khung, cự ly, hình người, nhóm màu và UI; các phần đó được visual critic đối chiếu cụ thể thay vì chỉ nhận tên hướng. Đây vẫn là đánh giá nội bộ trên mẫu, chưa có user quyết định hướng nào đủ khác biệt, vượt bậc hoặc phù hợp thời đại. Không xếp hạng loại bản trước.

Scope này xem mỗi ngôn ngữ trong một trạng thái: con người/cảnh/vật/UI được nhìn cùng nhau nhưng chưa có toàn bộ màn hình túi đồ/nhật ký, mọi góc cast hoặc đủ nhiều tình huống để chứng minh art direction xuyên toàn game. Bảng màu, thuyền, mái, trang phục, nhà và motif trong ảnh không tự trở thành canon hoặc phục dựng được xác nhận. Các lựa chọn và text raster không ghi authoritative state; không có character creator.

## Kiểm chứng và phản biện

[Code critic14](CRITIC-CODE-14.md) đã chạy thật `python3 prototypes/sprint13/experiment-14/browser-check.py`: **lượt đầu exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**, HTML100/150%. Sáu PNG1672×941RGB có hash/bytes/mode/kích thước đúng và giống nguyên bytes với original; sáu prompt/key và reference rỗng khớp metadata. Sáu card/switch/phím1–6, contain, modal/PNG gốc/Escape/close/focus, mười ba historylinks đều đạt; không JS error/request ngoài/failed request.

Evidence ignored ở `tests/results/sprint13-experiment14/browser.json` và21 screenshot. Harness preload trước decode/capture để không chụp lazy thumbnails chưa tải; không sửa source gallery. SHA nguồn trước/sau run và khi root đối chiếu khớp. Code critic thực sự xem mobile150; root xem sáu output lúc generation và screenshot `gallery-comic-life-1280.png`, `text150-390.png`, không nhận run browser của critic thành lượt tự chạy.

[Visual critic14](CRITIC-UX-14.md) đã xem sáu original14, hai ảnh13 đối chiếu và ba screenshot cuối: gallery-comic-life1280, gallery-glyph-world390, text150390. Không phát hiện P1 cản xem concept. Ghi nhận khác biệt cấu trúc nhìn được, đồng thời giữ P2 camera cong/phép chiếu/dấu đỏ/texture và câu hỏi người chơi–speaker ở trang truyện, nhận diện văn hóa. Không dùng số lượng hướng hoặc đồng thuận nội bộ để xác nhận “vượt bậc” theo chủ dự án; chưa có artwork mới nào tự chuyển Approved.

Root kiểm riêng preview HTTP200 cho gallery và sáu PNG, `node --check`/diff check đạt. Checks tài liệu/generated docs/exports thực hiện trên source cuối được append kết quả vào WORKLOG sau run. HTML150 chỉ tăng chữ trang xem, không làm UI raster reflow; original PNG cần zoom khi xem chi tiết trên điện thoại. Không OCR từng dấu, đo contrast, nghiệmthu UI runtime, camera, hình học, gameplay/Unity, benchmark hoặc human playtest trong task này.

## Đồng bộ và bàn giao

HANDOFF, Sprint13, approval register, changelog và export inputs đã đồng bộ vòng14. Export inputs gồm report14 và hai review14; regenerate sau khi review tồn tại và source cuối hoàn thiện. Lệnh bàn giao: build_docs --check, check_handoff, export.py rồi check_exports; kết quả thực ghi ở WORKLOG sau run. WORKLOG do root append riêng, không đưa vào exports. Không sửa archive, generated GDD hoặc VERSION/save/schema/content versions.

Giữ01–13 và workshop để tham khảo/kết hợp. Lời khen và nhận xét còn giống nhau là phản hồi thật của user cho vòng13, không bị đổi thành kết luận toàn thư viện kém hoặc quyết định duyệt/loại. [APPROVALS](APPROVALS.md) tiếp tục giữ S13-A01–A04 Pending/Changes requested. Không merge dev, đóngSprint13 hoặc chuyểnSprint14.
