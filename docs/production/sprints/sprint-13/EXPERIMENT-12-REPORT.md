# Sprint13 — Thử nghiệm12: sáu ngôn ngữ mỹ thuật mới

Vòng12 được giao ngày30/10/2026 để tạo ít nhất năm mẫu xem trước. Phạm vi là **sáu hướng mỹ thuật toàn game**: Đất biết kể, Nét trong, Vách thời gian, Ký ức CD-ROM, Đêm than bạc và Gió mảng màu. Mỗi hướng cần giúp chủ dự án hình dung cảnh chơi, nhân vật/toàn thân/chân dung, vật phẩm và giao diện có cùng bản sắc. Đây là bổ sung thư viện tham khảo, chưa chốt hướng hoặc kết quả Sprint13.

**Kết quả:** đã tạo sáu PNG ngày 30/10/2026 bằng built-in image_gen và gallery riêng để chủ dự án xem, phóng lớn, so sánh. Mỗi board có cảnh, hai người toàn thân khác nhau, ba chân dung, sáu icon hữu dụng và UI hội thoại/túi đồ/nhật ký. Chưa phải cả sáu đều đạt chính xác ý định chất liệu; các chỗ lệch được ghi bên dưới. Tất cả tiếp tục **Pending user approval**.

## Phạm vi và sản phẩm

- Sprint13 / SPIN-028, ảnh concept bằng built-in image_gen.
- Thư mục: `prototypes/sprint13/experiment-12/`; gallery, PNG gốc, prompt đầy đủ và provenance lưu tại đây.
- Mỗi bảng hướng tới cảnh, cast người trưởng thành, chân dung, vật phẩm/icon và UI hội thoại/túi đồ/nhật ký.
- Không tạo model thật, không gọi Hyper3D/Blender hoặc đổi engine; tên chất liệu và “dựng sẵn” chỉ mô tả diện mạo ảnh.
- Không triển khai gameplay/animation/UI runtime hoặc thêm canon, rule, clock, inventory state. Cast minh họa không cố định protagonist và không đổi baseline xuất thân ngẫu nhiên.

| Key | Tên hướng | Ý định khác biệt cần thể hiện |
|---|---|---|
| `clay-craft` | Đất biết kể | Khối đất nặn thủ công, vết tay và ánh sáng xúc giác cùng đi qua cảnh, người, icon và khung UI; tránh mặc định thành đồ chơi trẻ em |
| `clear-line` | Nét trong | Nét viền sạch có phân cấp, hình rõ và cách kể bằng khung truyện; giữ tuổi, nghề, biểu cảm và độ đọc của sinh hoạt |
| `mineral-fresco` | Vách thời gian | Mảng khoáng màu, bề mặt vách và bố cục bích họa làm quy tắc hình ảnh toàn game; không chỉ phủ texture lên cùng phong cảnh |
| `retro-prerender` | Ký ức CD-ROM | Diện mạo cảnh dựng sẵn và UI RPG cổ điển cuối thập niên1990; thử tỷ lệ người/cảnh và giao diện đặc trưng mà chưa có scene3D thật |
| `charcoal-night` | Đêm than bạc | Than chì/nét bạc, tương phản và vùng sáng dẫn đọc đời sống ban đêm; người, vật phẩm và UI cùng hệ giá trị sáng tối |
| `graphic-animation` | Gió mảng màu | Hình phẳng không viền, silhouette và nhịp mảng lớn theo hoạt hình đồ họa; UI và biểu cảm phải cùng logic giản lược |

Các hướng có thể tham khảo hoặc kết hợp; không là sáu lựa chọn engine loại trừ nhau. Bối cảnh, camera và cast có thể thay đổi giữa board, vì vậy sở thích không tự quy hoàn toàn về chất liệu. Các ý tưởng lấy cảm hứng Việt cổ là minh họa sáng tạo, chưa xác minh khảo cổ hoặc chốt thành game-canon.

## Phân công thực tế

| Actor được giao | Phạm vi |
|---|---|
| `/root/s13e12_direction` | Đã bàn giao ART-DIRECTION.md và README.md; root bổ sung trạng thái output thật vào README |
| `/root/s13e12_gallery` | Đã bàn giao HTML/CSS/JS; kiểm controls/layout trước ảnh. Root thêm ghi chú quan sát sau sinh ảnh |
| `/root/s13e12_docs` | Đã bàn giao hồ sơ draft và đồng bộ tài liệu/export inputs; root hoàn thiện kết quả sau review |
| `/root` | Prompt, sinh sáu ảnh, provenance, tích hợp, kiểm tra lớp đầu và điều phối hai critic sau workers |

Ba worker chạy trước, hai critic chạy sau trong giới hạn tối đa bốn tác nhân cùng hoạt động. Không có sáu tác nhân đồng thời. `/root/s13e12_critic_code` phụ trách browser/provenance; `/root/s13e12_critic_visual` phụ trách artwork/UX. Review độc lập của họ tách với kiểm lớp đầu của root; kết quả cụ thể ở hai hồ sơ review.

## Output và quan sát

Cả sáu ảnh là PNG **1672×941 RGB**, được sao chép nguyên bytes từ đầu ra built-in image_gen. [prompts.json](../../../../prototypes/sprint13/experiment-12/prompts.json) giữ trọn sáu prompt; [provenance.json](../../../../prototypes/sprint13/experiment-12/provenance.json) giữ tên file nguồn, SHA256, kích thước, mode và số bytes. Không ảnh tham chiếu, không hậu kỳ, không tạo model hoặc dựng scene3D thật.

| Board | Đầu ra đã quan sát | Khác biệt với ý định / điểm cần cân nhắc |
|---|---|---|
| Đất biết kể | Sân lò gốm; người phụ nữ lớn tuổi và người đàn ông gầy; chất vật liệu, khuôn mặt và UI vải/đất cùng hướng xúc giác | Gần miniature tả thực hơn khối đất nặn giản lược. Có blur ở tiền cảnh, khác yêu cầu mọi vật quan trọng đều rõ. Không chứng minh stop-motion |
| Nét trong | Sân nhuộm chàm, nét viền, hai dáng người đối lập, chân dung cá tính; icon/UI cùng nét vẽ | Hậu cảnh vẫn dày nét/chi tiết; có lời thoại dài do generation tự thêm ngoài text brief. Chỉ là minh họa, không nhập thành canon |
| Vách thời gian | Mương nước/bậc làng; màu đất đỏ, bề mặt khoáng và texture qua cảnh, người, icon, nhật ký | Phối cảnh sâu và chất liệu dày, chưa thành bích họa phẳng/khối lớn như brief. Cần giữ vùng chữ yên nếu hiện thực |
| Ký ức CD-ROM | Bến đò mưa nhìn từ cao, ánh đèn, người trong mái trú, portrait/dialogue và panel tối gợi RPG | Output còn sắc mịn và tả thực; chưa thấy low-res/dither đặc trưng cuối1990. “Pre-render” chỉ tên diện mạo, chưa có nền được render thật |
| Đêm than bạc | Người lớn tuổi cùng người trẻ bên giếng; ánh sáng đường đi, chân dung, icon và UI gần đơn sắc | Nền tối khá dày chi tiết; điểm đỏ lặp ở cảnh, mẫu nhân vật và selection. Chưa đạt đúng một điểm đỏ như prompt, chưa kiểm độ đọc trong chuyển động |
| Gió mảng màu | Sân phơi lúa, cast nặng/người gầy có tuổi, biểu cảm và UI bo gọn; icon rõ | Cast/UI giản lược hơn nền. Background vẫn nhiều chi tiết và lớp bóng, chưa là cực giản đồng đều trên toàn game |

Hai mẫu toàn thân khác nhau và ba portrait có mặt trong mỗi board. Các portrait không đồng nghĩa ba NPC mới đã được thiết kế/canon hóa. Trong một số board chân dung thứ ba là biểu cảm/biến thể hoặc nhân vật phụ chưa xác định. Sáu icon giúp đọc ngôn ngữ vật dụng, chưa phải sprite đã tách hoặc asset chuẩn production. Nhãn hội thoại không chứng minh một lượt hội thoại đầy đủ; có board chỉ minh họa danh sách hành động. Khung nhật ký, sơ đồ, chữ và các lựa chọn raster không ghi authoritative state và không mở rộng tri thức của nhân vật runtime.

Các bản là sáu tổ hợp riêng về bối cảnh, cast, camera và medium; không suy sở thích của chủ dự án chỉ do một biến, không xếp hạng loại bản cũ. [Gallery12](../../../../prototypes/sprint13/experiment-12/index.html) có ghi chú quan sát dưới từng card; khung HTML chỉ phục vụ xem, không phủ HUD game lên ảnh.

## Kiểm chứng và phản biện

Hai reviewer đã hoàn tất độc lập: [CRITIC-CODE-12](CRITIC-CODE-12.md) và [CRITIC-UX-12](CRITIC-UX-12.md). Code critic chạy thật `python3 prototypes/sprint13/experiment-12/browser-check.py`, lượt cuối **exit0**, Chrome **156.0.8078.47**, ba viewport **1280×720, 1920×1080, 390×844**. Kiểm sáu PNG/provenance/original byte equality, sáu nút/phím/card, contain, modal/PNG gốc/Escape/focus, HTML150 không tràn ngang, mười một link lịch sử và không lỗi JS/request ngoài/failed request. Evidence JSON cùng21 screenshot ở `tests/results/sprint13-experiment12/` (ignored).

Hai lượt assertion trước đó exit1 là lỗi wording trong checker. Một lượt bị reviewer dừng exit143 vì `decode()` chờ thumbnail lazy ngoài viewport; harness cuối preload trước capture và đạt. Không gọi các lượt đó Passed hoặc gán cho lỗi gallery. 21 screenshot cuối có đủ sáu thumbnail. Ở mobile150, số thứ tự trang trí vài nút xuống dòng; tên mẫu vẫn đọc/chọn được, ghi P3 trong review.

Visual critic thực sự xem sáu original, screenshot desktop và mobile/HTML150 cuối. Không phát hiện P1 cản xem ý tưởng trong phạm vi review. Các P2 là mức khớp medium chưa đồng đều, năm board chủ yếu action menu chưa có speaker/NPC line đầy đủ, và độ đọc UI raster khi thu nhỏ. Root thực sự xem sáu output lúc generation cùng screenshot `gallery-graphic-animation-1920.png` và `text150-390.png`; đối chiếu lại SHA nguồn với JSON cuối, không nhận automation của critic thành lượt tự chạy.

Root kiểm riêng preview HTTP200 cho gallery và sáu routePNG, `node --check` và `git diff --check` exit0. Checks tài liệu/generated docs/exports được ghi ở phần bàn giao dưới sau khi source freeze. Đây là kiểm truy vết và gallery, không phải game acceptance, human playtest, UI runtime, kiểm motion, benchmark hoặc xác nhận phong cách đã đạt hoàn toàn.

## Tác động tài liệu và bàn giao

HANDOFF, Sprint13 brief, approval register, changelog và export input đã đồng bộ vòng12. Export input gồm report12 và hai file review12; chỉ regenerate sau khi review tồn tại và source đã hoàn thiện. Các lệnh bàn giao: `python3 tools/docs/build_docs.py --check`, `python3 tools/checks/check_handoff.py`, `python3 tools/docs/export.py` rồi `python3 tools/checks/check_exports.py`; kết quả thực được append vào WORKLOG sau khi chạy, không nhúng journal vào exports. Không sửa archive, generated GDD, canon/rule hoặc save/schema/content/product versions.

Thư viện01–12 cùng được giữ. Mở vòng mới không mặc định bỏ hoặc đánh giá thấp bản cũ; chỉ ghi chọn/loại/kết hợp khi user quyết định rõ. [Approval register](APPROVALS.md) vẫn giữ S13-A01–A04 Pending/Changes requested. Không merge dev, đóngSprint13 hoặc sangSprint14.
