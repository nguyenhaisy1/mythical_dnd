# Sprint13 — Thử nghiệm18: mười lăm cách thể hiện để so sánh

02/11/2026 · SPIN-035 · **Đã có15frame,2bản đầu và hai review; Pending user approval**.

Chủ dự án muốn thêm khoảng mười lăm ý tưởng sau vòng17. Vòng18 được chia để thử rộng hơn về camera, cách tạo khối, dáng người, nét mặt và UI, đồng thời đưa thêm rừng, sân và hiên nghề vào phạm vi thay vì tất cả lại là cùng một bến sông. Một số hướng tiếp tục câu hỏi chưa đạt ở vòng trước; không tuyên bố mọi kỹ thuật hoặc vật liệu đều mới hoàn toàn.

**Kết quả:**15frame trưng bày và hai bản đầu được giữ, cùng prompt/provenance/gallery. Có khác biệt nhìn thấy giữa camera, tạo khối và render, song mức đạt brief không đồng đều. Review/checks chỉ ghi hoàn tất khi có bằng chứng thực. Giữ01–17, các biến thể và ba ưu tiên14/1–3; yêu cầu thêm ảnh/lời động viên không là final art approval.

## Yêu cầu, đầu vào, đầu ra và cách đánh giá

- User02/11 yêu cầu “thêm cho tôi tầm15ý tưởng nữa”. Root chọn phạm vi15 frame chính để thực hiện trong Sprint13; đây không phải giao chuyểnSprint14 hoặc chốt style cuối.
- Đầu vào gồm chỉ dẫn hiện hành, [workshop](ART-DIRECTION-RETHINK.md), hồ sơ01–17, findings đã ghi và brainstorming giữa ba worker. Không biến lựa chọn của AI ở vòng17 thành sở thích mới của chủ dự án.
- Đầu ra được giao:15 PNG riêng tại `prototypes/sprint13/experiment-18/assets/{key}.png`, prompt/provenance, [gallery](../../../../prototypes/sprint13/experiment-18/index.html), ART-DIRECTION/README, ba note brainstorm và hai review sau output. Root đã tạo17lượt built-in image_gen mới không ảnh input; hai hướng dải thân/đồng mức có iteration mới và giữ bản đầu.
- Mỗi frame cần cho xem cách tổ chức cảnh, toàn thân/cận mặt, vật hữu dụng, hành động và UI có quan hệ tạo hình. Không ép bảng asset65/35 hoặc cùng số ô chân dung/icon ở mọi hướng. Tên gọi và prompt không là bằng chứng đầu ra đã đạt.
- Chỉ ảnh mô phỏng diện mạo game. Không tạo model, scene3D thật, Hyper3D/Blender, camera động, shader/engine production, animation hoặc gameplay implementation.

Acceptance của lượt tìm ý tưởng là có đủ bộ ảnh/hồ sơ để chủ dự án xem và so sánh, giữ rõ lineage, phần đạt/chưa đạt, nguồn ảnh và giới hạn bằng chứng. Kiểm gallery hoặc review nội bộ không thay quyết định mỹ thuật của chủ dự án.

## Brainstorm thực giữa các tác nhân

| Actor | Note sở hữu và đóng góp |
|---|---|
| `/root/s13e18_space` | [01-SPACE](../../../../prototypes/sprint13/experiment-18/brainstorm/01-SPACE.md): sáu câu hỏi camera/không gian, từ sân mở và dải đường đến qua vai, đường chéo, rừng và hybrid mặt bằng–profile |
| `/root/s13e18_cast` | [02-CAST-UI](../../../../prototypes/sprint13/experiment-18/brainstorm/02-CAST-UI.md): sáu grammar thân người/nét mặt/UI, kiểm tuổi qua hơn một dấu hiệu và phân biệt vai người đang nhập vai/người nói |
| `/root/s13e18_audit` | [03-RANGE-AUDIT](../../../../prototypes/sprint13/experiment-18/brainstorm/03-RANGE-AUDIT.md): audit source/reviews chọn lọc, sáu đề xuất range, chỉ ra trùng và góp ý khác biệt thật cần giữ |
| `/root` | Gộp đề xuất, chọn15 tổ hợp để tạo ảnh, viết prompt/sinh ảnh/provenance và tổng hợp; không chốt art direction thay user |

Ba cặp cel/soft/ink trùng giữa cast và audit được gộp vào grammar cast, không đếm thành sáu ý tưởng riêng. Phạm vi cuối là6space +6cast +3audit: one-bit, single-stage và retro-facet. Ba worker đã trao đổi trực tiếp; note ghi actor và giới hạn đọc source, không nhận đã mở lại toàn bộ artwork01–17.

Audit nhắc hai cặp dễ giống nhau: Sân chéo mở/Đường chéo dài cần thay tổ chức đường nhìn, Ba dải đường/Sân khấu một nền cần giữ khác biệt ba lớp occlusion với một floor nông. Pixel hình lớn/one-bit cũng phải khác quy tắc cụm hình, khoảng rỗng và cách dùng dither, không chỉ palette. Root chọn15 slot là quyết định phạm vi thử ảnh, chưa chứng minh15 ngôn ngữ đã hoàn chỉnh.

## Mười lăm hướng và delta cần kiểm

| # / key / tên | Lineage hoặc tiền lệ | Quy tắc cần nhìn thấy; giới hạn |
|---|---|---|
|01 · `open-yard` — Sân chéo mở | Học góc chéo13 và cự ly17; không chỉ lặp bến nhiều đồ | Sân công cộng ở góc3/4 mở, khoảng đi và cuộc gặp lớn, công trình ít khối; khác hành lang/vòm lồng. Không navigation graph đã có |
|02 · `three-bands` — Ba dải đường | Học dải13 và nhìn ngang16 | Ba dải ngoài trời có che khuất/độ sâu, người và lối tiếp cận vẫn đọc rõ; không cutaway nhà hoặc ba timeline, không nhìn xuyên vách |
|03 · `shoulder-craft` — Qua vai người thợ | Học13near và14first-person, đối chiếu17nested/low-water | Vai/tay/vật làm việc chiếm một mép gần, người đối diện và khoảng UI còn rõ; không chỉ đổi crop một phong cảnh hoặc camera runtime đã nghiệm thu |
|04 · `long-diagonal` — Đường chéo dài | Học axon/quy mô14 và khối17 | Một đường chéo tổ chức toàn frame, hướng người/vật và khoảng gặp phụ thuộc nhịp ấy; khác Sân chéo mở, không chỉ đặt tên mới cho cùng góc nhìn |
|05 · `column-forest` — Rừng là thân cột | Học framing17 nhưng mở bối cảnh khỏi bến sông | Thân/rễ cây thành khối/cột dẫn mắt quanh một công việc địa phương; vẫn có người và useful objects, không chỉ tranh rừng hoặc sinh thái/canon mới |
|06 · `plan-profile` — Sân phẳng, người nổi | Học vấn đề phép chiếu16woven/17river | Mặt bằng phẳng và body profile nổi dùng quy ước hybrid có chủ ý, portrait/UI cùng hệ; không tự gọi đây là đã sửa mọi mixed projection hoặc xác nhận geometry thật |
|07 · `cel-adult` — Nét mặt ba sắc | Học anime11, cự ly13 và tuổi/face ở17 | Khối có vài bậc sáng rõ, outline chọn lọc; mắt/mày/mũi/posture diễn tuổi trưởng thành, không trẻ hóa cast hoặc shader điện ảnh phủ cảnh thường |
|08 · `soft-mass` — Khối liền mềm | Clay12; đề xuất17 từng giữ trong brainstorm chưa tạo ảnh | Vài thể tích bo liền xuyên người/cảnh/UI, bỏ rod/khớp16 và texture thủ công; không chibi một khuôn hoặc model sculpt thật |
|09 · `ink-mass` — Mực đặc | Nét/than12 và silhouette17 | Hình đặc, khoảng rỗng và che khuất tạo chiều sâu bằng mảng cứng, ít grain; không chỉ chuyển tranh thành trắng–đen hoặc hiểu bóng là ma thuật |
|10 · `ribbon-body` — Người bằng dải | Dệt08/16 chưa xuyên toàn body | Dải tạo silhouette, cánh tay và che khuất của người/vật/UI; mặt còn mảng đặc đọc tuổi, không chỉ áo quấn/texture vải |
|11 · `contour-step` — Đường đồng mức sống | Đối chiếu relief/gấp/voxel16–17 | Các lớp cao độ uốn và che nhau tạo thể tích xuyên body/world/UI; không decorative contour, scan/x-ray hoặc quyền biết bí mật |
|12 · `square-gesture` — Pixel hình lớn | Pixel07/11/17; phản biện density và tuổi | Cụm vuông lớn/màu đặc tạo thân và dụng cụ, đủ dấu mắt/mày/tuổi, không hatching vụn; không voxel3D hoặc asset grid/animation đã kiểm |
|13 · `one-bit` — Một-bit có tuổi | Pixel và charcoal có tiền lệ, không kỹ thuật mới hoàn toàn | Silhouette đặc/khoảng rỗng lớn, stipple tiết chế ở vài vùng chuyển, nét mặt ít nhưng có tuổi; không dither mọi grayscale hoặc phủ filter ảnh |
|14 · `single-stage` — Sân khấu một nền | Nhìn ngang16 và gesture17 | Một floor công cộng nông, người lớn và cử chỉ đầy đủ; khác Ba dải đường ở số lớp/occlusion, không nhà cutaway/quest platformer đã chốt |
|15 · `retro-facet` — Khối răng cưa | Retro-prerender12 và khối17 | Bước cạnh/coarse facet xuất hiện trên silhouette và planar face, dither chọn lọc, camera/UI gọn; không dither lên photoreal, renderer/geometry thật hoặc benchmark |

Mười lăm tổ hợp có thể đổi nhiều trục đồng thời; phản hồi đầu tiên so sánh tổ hợp, chưa quy thích/không thích riêng cho palette, camera hoặc vật liệu. Đừng gọi đủ tên thành đủ15đột phá. Nếu output còn là cảnh thường với texture đổi, cần ghi đúng thay vì sửa lời mô tả để tự coi đã đạt.

## Nhân vật, giao diện và bản sắc

Tuổi cần nhiều dấu hiệu: tỉ lệ, nét mắt/mày/mũi, lưng, vai và dáng làm việc; tóc xám đơn lẻ không đủ. Giản lược không bắt buộc bỏ toàn bộ nét mặt. Portrait cần đại diện đúng người đang nhập vai và khớp body/tóc/trang phục ở frame; bracket player và bubble người nói phải khác hình, không chỉ khác màu. UI mỗi hướng có grammar riêng, không cùng HUD trung tính phủ lên mọi ảnh.

Nghề nghiệp, cộng đồng và quan hệ giữ trọng tâm qua việc đang làm và vật đang dùng. Cast minh họa một cuộc đời nhận ngẫu nhiên, không character creator hoặc protagonist production cố định. UI raster chưa là text component/handler và chưa ghi authoritative state.

Khung lấy cảm hứng Văn Lang–Âu Lạc không đồng nghĩa đã xác minh phục trang, kiến trúc, công cụ hoặc motif. Workshop là nền phân biệt hiện vật, diễn giải và hư cấu; batch này chưa có nghiên cứu khảo cổ mới. Rừng/cột, hybrid, contour, dải và mặt cắt là presentation, không thêm canon, route, physics, thời tiết hoặc nguồn hidden truth.

## Phân công triển khai và review

| Actor / lớp | Ownership được giao |
|---|---|
| `/root/s13e18_space` | ART-DIRECTION/README và note01 |
| `/root/s13e18_cast` | HTML/CSS/JS gallery15 và note02 |
| `/root/s13e18_audit` | Draft report này, HANDOFF/sprint/APPROVALS/changelog/export inputs và note03 |
| `/root` | Prompt/generation/provenance/tích hợp/kiểm lớp đầu, WORKLOG và điều phối review |
| `/root/s13e18_critic_code` | Kiểm độc lập17PNG/prompt/provenance/gallery và ghi evidence thật |
| `/root/s13e18_critic_visual` | Mở artwork/đối chiếu nguồn ảnh/screenshot, review hình/UX trong phạm vi đã xem |

Các vai trò chạy theo lượt trong giới hạn4concurrent, không có sáu tác nhân chạy đồng thời. Assignment khác completed, root self-review khác independent review, automation khác human playtest.

## Output và quan sát thực tế

Mười bảy PNG **RGB**, tổng **30,511,159bytes**, tạo02/11 bằng built-in image_gen. Mười lăm ảnh1672×941; riêng `plan-profile.png` và `contour-step-v1.png`1672×940. Không resize/crop/hậu kỳ để ép về một kích thước; copy nguyên bytes từ generated_images. Mười lăm frame trưng bày và hai bản đầu `ribbon-body-v1.png`, `contour-step-v1.png` giữ qua link riêng.

[Prompts](../../../../prototypes/sprint13/experiment-18/prompts.json) giữ15prompt đầu và2iteration; [provenance](../../../../prototypes/sprint13/experiment-18/provenance.json) ghi original filename/SHA/mode/dims/bytes/mapping. Hai bản dải thân/đồng mức trưng bày dùng iterations/ribbon-body-v2 và contour-step-v2; v1 dùng prompt chính;13hướng khác dùng prompt chính cùng key. Cả17lượt mới không ảnh input, kể cả iteration. Lineage là quan hệ ý tưởng, không chứng minh matching cast/geometry hoặc reuse ảnh trước.

Prompt được soạn theo nhóm trước khi chạy; JSON cuối giữ đúng chuỗi thực gửi. Một số lượt độc lập được gọi theo nhóm bằng Promise.allSettled; không suy thời gian tool thành tốc độ hay benchmark provider. Root đã mở cả17output trong lượt tạo. V1 dải thân/đồng mức còn gần người mặc áo/cảnh thường nên root rebrief cấu trúc, giữ v1 không gán user bác bỏ.

| Frame | Điều thực sự nhìn thấy và phần còn mở |
|---|---|
| 01 — Sân chéo mở | Sân mở và người ở cự ly gần, mặt/marker/UI rõ. Người, đồ phơi và sàn còn nhiều nét/texture; camera chưa là orthographic giản lược hoàn toàn. |
| 02 — Ba dải đường | Hai tầng lớn và người nền đã tách, cử chỉ chỉ đường dễ đọc. Chưa đủ ba dải liên kết rõ; dải trên như thanh treo, không thấy portrait player riêng. |
| 03 — Qua vai người thợ | Vai, tay, mái chèo và mặt người hướng dẫn thành trọng tâm. Player bị cắt ở mép khung, chưa có portrait riêng; vật liệu còn nhiều sắc độ hơn cel tối giản. |
| 04 — Đường chéo dài | Lối cầu xiên xuyên hai mép, địa hình thành khối lớn, khác bến làng. Người còn nhỏ và portrait tóc xám chưa rõ tuổi; chưa xác nhận axon chính xác. |
| 05 — Rừng là thân cột | Thân/rễ cây lớn tạo che khuất quanh công việc buộc bó, người và mặt đọc được. Lá/nền và phục trang vẫn dày chi tiết, chưa chỉ vài khối. |
| 06 — Sân phẳng, người nổi | Body nghiêng và portrait có họ nét thống nhất, cuộc gặp rõ. Cảnh vẫn phối cảnh nhà/bến thường, chưa sân mặt bằng90° và body-profile theo quy ước riêng như brief. |
| 07 — Nét mặt ba sắc | Nét mặt, vai/tay và tuổi hai người rõ, UI cùng tông xanh. Sắc độ/đám đông nhiều hơn ba giá trị mỗi khối; portrait lặp ở cả hai nút. |
| 08 — Khối liền mềm | Thân dài–thấp rộng, cột/bình/UI cùng khối bo liên tục. Vẫn có texture, nếp/khối áo và ánh sáng vật liệu; chưa bỏ hết cảm giác đất nặn. |
| 09 — Mực đặc | Đặc–rỗng và dấu đỏ tạo thứ bậc, mặt người và hành động rõ. Background/nét lá vẫn dày, có trung gian sắc độ; chưa thuần hai tông ít chi tiết. |
| 10 — Người bằng dải | V2 đã có torso/tay/chân bằng dải rộng có khoảng rỗng, UI và nền cùng dải. Tính trừu tượng tăng, bản sắc nơi chốn/tuổi cần đào sâu; chưa là topology hoặc chuyển động thật. |
| 11 — Đường đồng mức sống | V2 thấy lớp bậc trên đầu/thân/vật/cột/thuyền/UI, khác v1 gần cảnh thường. Cỡ lớp chưa đều, dáng như tượng ghép; UI lặp portrait thay icon Nhật ký. |
| 12 — Pixel hình lớn | Cụm vuông lớn hơn và portrait tóc xám có nét tuổi rõ; body/props/UI cùng diện mạo pixel. Vẫn nhiều chi tiết/sắc độ, không chứng minh palette12màu hoặc grid asset chuẩn. |
| 13 — Một-bit có tuổi | Đen–trắng, cụm chấm và lưng/mặt gầy giúp đọc tuổi; khung/nút cùng nhịp raster. Là diện mạo một-bit trên PNG RGB, chưa xác nhận file1-bit hay engine. |
| 14 — Sân khấu một nền | Một sàn chính nhìn ngang, cuộc gặp/người/vải/loom rõ, khác bản dải tầng. Còn phối cảnh/đồ phụ và nền nhiều chi tiết hơn staging cực gọn. |
| 15 — Khối răng cưa | Mặt khối/nhân vật góc cạnh và lối lên đọc được. Cạnh răng cưa/dither chưa rõ, còn gần low-poly minh họa có texture; không thấy portrait player riêng. Không renderer retro đã kiểm. |
| Người bằng dải v1 — giữ riêng | Có palette/UI mềm và người cầm chiếu, nhưng thân vẫn là người mặc áo bình thường. Chưa đạt body tạo từ dải; root thử lại, không user loại. |
| Đồng mức v1 — giữ riêng | Vật/bờ/áo có lớp và khối mềm, nhưng người/cảnh còn gần một cảnh CG thông thường; chưa đồng mức xuyên cấu trúc. Root thử lại, giữ bản gốc. |

Đặc biệt, Một-bit có tuổi là diện mạo đồ họa; file thực là PNG RGB, chưa literal1-bit hoặc pipeline binary. Tên cel ba sắc/pixel/dither cũng là ý định cần đối chiếu, không khẳng định đã đếm palette hay có engine tương ứng. Một số camera/portrait chưa đạt brief; không lấy15tên thành15đột phá. Hai biến thể mới có cấu trúc rõ hơn nhưng tăng trừu tượng, tuổi/nơi chốn/bản sắc còn cần đào sâu.

Không nhập cast/props/kiến trúc này vào canon/content pack hoặc xác nhận niên đại Văn Lang–Âu Lạc. Không có model/scene/camera runtime/animation mới. Rừng, cầu, sân, xưởng và các khối/dải là bối cảnh/grammar minh họa, không rule thu hoạch/xây/phá/đi tầng/tri thức mới.

## Kiểm chứng và phản biện

Worker gallery có hai lượt preasset harness. Lượt1 dừng vì checker tạm kiểm PNG gốc tồn tại khi chưa có assets; evidence failed được giữ ở `tests/results/sprint13-experiment18/worker-gallery-attempt1.json`. Chỉ checker tạm được chỉnh, source gallery không đổi giữa hai lượt. Lượt2 controls/layout exit0 Chrome156.0.8078.79 tại1280×720/1920×1080/390×844, HTML100/125/150; evidence `worker-gallery.json`. Đây không là kiểm PNG decode/hash/provenance, không nhận missing-asset requests thành assetcheck Passed.

Root bổ sung15observednotes/2v1links và sửa tên hiển thị plan-profile về **Sân phẳng, người nổi** để khớp prompts/README trước run critic cuối. Critic code phát hiện lệch tên trước khi chạy, không tự sửa source; root báo freeze mới. Worker fingerprints preasset không được dùng để nhận source cuối đã assetcheck. Root kiểm gallery+17PNGHTTP200; appopen preview trảqueued không là user approval.

[Code critic18](CRITIC-CODE-18.md) đã chạy `python3 prototypes/sprint13/experiment-18/browser-check.py` thật trên source sau fix tên: lượt đầu exit0, Chrome156.0.8078.79. Mười bảy PNG giống nguyên bytes với original, mode/dims/bytes/hash và17mapping (15+2)/norefs đạt. Mười lăm card/switch, digits1–9+0=10, ArrowLeft/Right chuyển vòng, Home/End, contain/modal/PNG gốc/Escape/close/focus,17historylinks và2v1links đạt tại1280×720/1920×1080/390×844. HTML100/125/150 tương ứng16/20/24px không tràn ngang; không JS error/request ngoài/failed request; năm fingerprint trước/sau và root đối chiếu cuối khớp.

Evidence ignored: `tests/results/sprint13-experiment18/browser.json` và50screenshot (45gallery+3text150+2scene gọn). Harness preload/decode thumbnail trước capture, không sửa source. Root đã mở hai capture `scene-ribbon-body-1280.png`, `scene-one-bit-390.png`: controls sắp trong bề rộng, artwork toàn khung; UI raster vẫn thu nhỏ trên mobile. Root đã mở17output và kiểm gallery+17PNGHTTP200, không nhận browserrun của critic code là run do root thực hiện. Lượt checker độc lập này khác hai lượt preasset của worker; không che lượt harness lỗi trước hoặc gọi nó là lỗi gallery đã được kiểm assets.

[Visual critic18](CRITIC-UX-18.md) đã hoàn tất: mở trực tiếp17PNG18, ba ảnh cũ (12clay,16line-volume,17close-pixel) và ba screenshot cuối (scene-ribbon1280,scene-one-bit390,gallery-column-forest1920). Reviewer đọc browserJSON nhưng không tự chạy automation hoặc mở hết50screenshot. Fullpage được công cụ thu nhỏ khi xem, nên không nhận OCR/đo contrast hay đọc hết chữ nhỏ. Không thấy P1 cản xem; P2 về mức đạt hybrid/ba dải/retro, portrait thiếu hoặc lặp, mật độ/raster mobile và tuổi/bản sắc ở dải/lớp được giữ trong review và bảng output. V2 dải/lớp đổi cấu trúc rõ; pixel18 nét tuổi rõ hơn17 trong phạm vi đối chiếu, không quy mọi thay đổi cho một biến. Không nhận quan sát này là user approval hoặc nghiệm thu toàn game.

Một ảnh không kiểm toàn roster, mọi vùng/nghề, motion hoặc mọi UI state. Tăng chữ HTML không làm chữ raster reflow; độ đọc khi thu nhỏ cần quan sát, chưa OCR từng dấu/đo contrast. Không có gameplay/Unity/runtime/knowledge isolation/navigation/benchmark hoặc human playtest mới chỉ từ ảnh/DOM/docs checks.

## Đồng bộ và bàn giao

HANDOFF, Sprint13, APPROVALS, CHANGELOG và export inputs đã thêm vòng18 theo scope được giao. Export source dự kiến thêm report18 và hai review18, giữ các note brainstorm trong project qua liên kết; root regenerate khi hồ sơ cuối có đủ. Build_docs --check, check_handoff, export.py/check_exports và checks phù hợp gallery ghi kết quả thật sau run vào WORKLOG; journal không nhúng vào exports.

Không sửa archive, generated GDD, canon/contract, engine/gameplay, save/schema/content hoặc VERSION. Không ZIP hoặc model pipeline mới. Giữ01–17, các biến thể, mọi đề xuất chưa dùng và ưu tiên14/1–3. Toàn bộ output18/S13-A01–A04/Sprint13 tiếp tục **Pending user approval**; không merge dev/main, đóngSprint13, chuyểnSprint14, tag hoặc release.
