# Thử nghiệm 07 — Ba ngôn ngữ đồ họa và bối cảnh mới

Sprint 13/SPIN-023, ngày 29/10/2026. **Ba ảnh đã tạo và hoàn tất review lớp kỹ thuật/hình ảnh; mọi phương án Pending user approval.** Vòng này tạo ảnh mô phỏng để xem trước ý tưởng, không tạo model 3D thật hoặc triển khai gameplay.

## Mục tiêu dễ hiểu

Sau vòng06, chủ dự án yêu cầu thử ý tưởng khác và nhấn mạnh phải thử cả đồ họa. Vòng07 mở rộng cách thể hiện ra ngoài tranh tả thực: thay kỹ thuật tạo hình, mảng màu, mức chi tiết và cả tình huống đời sống. Mục tiêu là giúp chọn một hướng thị giác đáng thử tiếp, không tự chốt mỹ thuật từ kết quả tạo ảnh.

| Hướng đã thử | Đồ họa | Bối cảnh concept hư cấu |
|---|---|---|
| A | Tranh khắc gỗ, khoảng năm màu in, mảng phẳng và nét rõ | Làng mùa nước, kho lương kê cao và người chuyển thóc |
| B | Pixel art với cụm pixel và hình khối đọc rõ | Xưởng đúc đồng, nghề nghiệp và sinh hoạt quanh lò |
| C | Giấy cắt nhiều lớp, silhouette và bóng lớp | Phiên chợ/đêm hội ven sông, sinh hoạt cộng đồng |

**Cả phong cách lẫn bối cảnh đều thay đổi.** Nếu thích một ảnh hơn, chưa thể quy hoàn toàn cho đồ họa: chủ đề, ánh sáng, bố cục và hoạt động cũng có thể là nguyên nhân. Sau khi chọn được hướng gần ý, có thể thử lại một bối cảnh chung để so sánh riêng phong cách; chưa tự mở lượt thử đó.

## Phạm vi và ranh giới

Root dùng built-in image_gen tạo ba ảnh raster độc lập; gallery chỉ phục vụ đổi ảnh, phóng lớn và xem lớp giao diện minh họa. Ba PNG thực đã lưu tại [thư mục thử nghiệm07](../../../../prototypes/sprint13/experiment-07/index.html): `assets/a-woodcut.png`, `assets/b-pixel.png`, `assets/c-papercut.png`. Mỗi ảnh1672×941RGB, tạo mới bằng một lần gọi built-inimage_gen riêng; không dùng ảnh tham chiếu hoặc hậu kỳ. [Prompts](../../../../prototypes/sprint13/experiment-07/prompts.json) giữ nguyên văn ba prompt; [provenance](../../../../prototypes/sprint13/experiment-07/provenance.json) lưu tên output gốc, SHA256, kích thước và bytes. Không để nguồn ảnh chỉ ở thư mục riêng của công cụ.

Các cảnh là **concept hư cấu chưa chốt canon**, lấy cảm hứng thế giới Việt cổ của dự án. Không suy ảnh thành phục dựng khảo cổ, quyết định thêm lễ hội/địa danh hoặc đặc tả hoạt động đúc đồng/chuyển thóc. Không thêm rule, NPC autonomy, WorldState hoặc nội dung runtime từ minh họa. Không gọi Hyper3D/Blender, không tạo geometry thật; không coi ba ảnh là các camera của một scene.

Các bản01–06 được giữ nguyên. Vòng05 và trạng thái chưa xác định của lượt sửa Hyper3D vẫn là lịch sử thật, không được gửi lại để phục vụ vòng07. Baseline random character, no runtime LLM và ranh giới kiến trúc không đổi.

## Phân công và evidence

| Actor | Công việc được giao |
|---|---|
| /root/s13e7_direction | Art direction và hướng dẫn xem/so sánh |
| /root/s13e7_gallery | Gallery offline, nhãn phong cách/bối cảnh và UI minh họa |
| /root/s13e7_docs | Đồng bộ report, handoff, sprint, approval, changelog và export inputs |
| /root | Tạo ảnh, lưu prompt/provenance, tích hợp, kiểm lớp đầu và tổng hợp |

Ba workers đã bàn giao, sau đó hai critic độc lập nhận việc: `/root/s13e7_critic_code` kiểm provenance và gallery, `/root/s13e7_critic_visual` xem artwork. Không claim sáu actor chạy đồng thời; root điều phối và tạo ảnh trong lúc ba workers làm việc.

Root đã thực sự xem cả ba output và screenshot gallery. Đồ họa khác rõ so với nhóm tranh tả thực06: A nét khắc đậm/màu in, B cụm pixel/cạnh bậc thang, C mép giấy/bóng lớp. Cảnh mới cho thấy vận chuyển lương thực, nghề đúc và các cuộc gặp; chúng không chứng minh các hoạt động đó đã được lập trình. Nhận xét của root: A nổi bật về bản sắc, B gần cảm giác RPG quen thuộc, C hợp không khí chuyện kể. Đây là ý kiến AI, không phải quyết định của user.

## Kiểm chứng thực tế và điều còn mở

`/root/s13e7_critic_code` đã chạy `python3 prototypes/sprint13/experiment-07/browser-check.py`: exit0 ở1280×720,1920×1080 và390×844. Ảnh decode đúng, hash/bytes/kích thước khớp provenance và PNG gốc; HUD đủ năm dòng đổi đúng từng bối cảnh; nút/phím/thumbnail, dialog/Escape/focus, hiện/ẩnHUD và chữ150% hoạt động; không tràn ngang, không script errors hoặc request mạng ngoài ởfilelocal. Evidence ignored: `tests/results/sprint13-experiment07/browser.json` cùng screenshots. Root xem screenshot, không nhận automation của critic là lượt tự chạy.

[Review code](CRITIC-CODE-07.md) chỉ kết luận kỹ thuật gallery. [Review hình ảnh](CRITIC-UX-07.md) ghi ba vấn đề còn mở: A nét sóng/cọc khá dày có thể cạnh tranh với người; B chưa là pixelasset có grid/palette production được chuẩn hóa; C texture/bóng và dải tím có thể giảm độ đọc mặt/người khi thu nhỏ. Không có P1 cản việc xem/so sánh ý tưởng được phát hiện trong scope review; không coi đó là art approval.

Khoảng năm màu của A là mục tiêu thị giác, không phải chứng minh PNG chỉ có đúng năm giá trị màu. `image-rendering:pixelated` hỗ trợ hiển thị B, không sửa grid hoặc biến ảnh thành tileset/spritesheet. Giấy cắt C là diện mạo ảnh, không phải scene giấy thật hoặc model được dựng. Chưa có bằng chứng hướng nào rẻ hơn, chạy nhanh hơn hoặc phù hợp production hơn.

Không có human playtest, Unity/gameplay acceptance hoặc kiểm chứng khảo cổ trong phạm vi này.

## Đồng bộ và quyết định

Handoff, sprint brief, approval register, changelog và export source cùng trỏ vòng07. Export source bổ sung report07 và hai review07 sau khi được tạo. Root regenerate/check exports sau khi evidence được hoàn thiện; không cập nhật manifest để giả freshness. Không thay archive, generated GDD, save/schema/content versions hoặc product VERSION.

[Danh sách cần chốt](APPROVALS.md) giữ S13-A01–A04 Pending/Changes requested. Yêu cầu thử hướng khác không tự duyệt hay hủy mọi bản trước. Không merge dev, đóng Sprint13 hoặc chuyển Sprint14 trước khi chủ dự án chốt rõ.
