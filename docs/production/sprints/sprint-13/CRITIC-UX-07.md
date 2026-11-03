# Phản biện hình ảnh07 — ba ngôn ngữ đồ họa

29/10/2026 · Sprint13/SPIN-023 · Reviewer độc lập `/root/s13e7_critic_visual`. **Pending user approval.** Chỉ review ảnh và cách trình bày; không tạo ảnh/model hoặc sửa gallery.

## Đã thực sự quan sát

Đọc baseline, HANDOFF, hướng dẫn mythical-validation/game-design-review, brief07, provenance và HTML gallery. Đã mở bằng công cụ ảnh cả ba PNG gốc `a-woodcut.png`, `b-pixel.png`, `c-papercut.png` tại `prototypes/sprint13/experiment-07/assets/` (đều1672×941), cùng hai screenshot `gallery-b-1280.png`, `gallery-c-1280.png` trong `tests/results/sprint13-experiment07/`. Không tự chạy browser hoặc nhận automation của code critic là lượt mình chạy.

## Kết quả nhìn thấy

| Hướng | Ngôn ngữ đồ họa thực tế | Trọng tâm đọc được |
|---|---|---|
| A — Khắc gỗ | Nét đen dày, vệt khắc gãy, mảng đỏ/ochre/xanh và nền giấy sáng; gần tranh in hơn nhóm tranh tả thực06. Không chỉ đổi góc máy hoặc saturation. | Người mang bao ở ngã rẽ, kho cao, gia đình chờ và thuyền vận chuyển. Cầu tre chia nhánh giúp gợi lựa chọn cộng đồng. |
| B — Pixel art | Cạnh bậc thang và cụm pixel hiện rõ ở mái, lá, đồ nghề và người; hình học/ánh sáng được giản lược theo phong cách game. | Người thợ thao tác cạnh lò, cuộc trao đổi ở giữa xưởng và khách đem dụng cụ; nghề nghiệp có chỗ rõ trong ảnh. |
| C — Giấy cắt | Mép xé/cắt, vân giấy và bóng giữa các lớp hiện rõ; cây, mây, người và bến tạo cảm giác sân khấu thủ công. Tím đêm là lựa chọn riêng của hướng này. | Các nhóm giao dịch, trò chuyện và gia đình ở bến tách thành từng cụm; lối đi ngang và cảnh sông làm nền. |

Ba hướng **khác rõ về kỹ thuật thể hiện**, đáp ứng câu hỏi thử cả đồ họa ở mức ảnh concept. Không phát hiện lỗi hình ảnh P1 cản chủ dự án xem hoặc phân biệt ba ý tưởng. Chi tiết tay/công cụ nhỏ chưa được coi là anatomy/asset production đã đạt.

## Những điểm còn mở

- **P2 / A:** nét sóng, cọc và dây khá dày, nhiều vùng có cùng độ tương phản đen. Khi thu nhỏ, chân người và mép cầu có thể lẫn vào đồ vật. Nếu tiếp tục A, nên giảm nét ở vùng phụ và giữ đường viền nhân vật/hành động rõ hơn. Chưa đếm màu; không khẳng định output đúng chính xác năm mực in.
- **B / giới hạn cần kiểm tiếp:** hình nhìn được như pixel art, nhưng chưa chứng minh toàn ảnh tuân một grid native, palette24 màu hoặc có thể cắt thành tile/sprite dùng ngay. Không gọi ảnh sinh này là bộ asset pixel hoàn chỉnh. Nếu chọn B, lượt sau cần thử nhân vật và môi trường cùng quy tắc pixel ở một màn hình kiểm soát.
- **P2 / C:** nhiều texture và bóng tiếp giáp khiến cảnh gần mô hình giấy nổi hơn mảng silhouette tối giản. Nhân vật và mặt một số người chìm trong dải tím tối; khi thu nhỏ, sắc thái hội thoại kém rõ hơn B. Có thể thử giảm vân giấy/bóng và nâng tương phản người với nền nếu tiếp tục hướng này. Không coi việc dùng tím là lỗi màu AI tự thân.

Trong hai screenshot desktop đã xem, tên hướng và nhãn ảnh mô phỏng/chưa phải game hiện rõ; HUD đổi thành xưởng hoặc phiên chợ tương ứng và không đè lên nhóm người chính. HUD là minh họa, không chứng minh hệ nghề/xã hội đã chạy. Chưa đánh giá mọi viewport, thao tác bàn phím hoặc bật/tắt HUD từ ảnh chụp; review kỹ thuật có evidence riêng.

## Nhận định

Ý kiến reviewer: **A nổi bật về bản sắc đồ họa**, **B dễ hình dung một màn hình RPG nghề nghiệp**, **C gợi chuyện kể và sinh hoạt tín ngưỡng**. Không suy bản sắc A là phục dựng truyền thống in Việt cổ, hoặc chi tiết lễ trong C thành canon đã duyệt.

Vòng này đổi cả bối cảnh, hoạt động và ánh sáng bên cạnh phong cách. Vì vậy thích một ảnh không chứng minh chỉ riêng đồ họa của nó tốt hơn; nếu cần tách nguyên nhân, nên thử hai phong cách được quan tâm trên cùng tình huống. Hiện tại có thể đưa cả ba cho chủ dự án xem ngay. Đây là nhận xét AI, không phải user approval, human playtest, Unity/gameplay hoặc kiểm chứng khảo cổ. Không chốt Sprint13 hoặc chuyển Sprint14.
