# Phản biện hình ảnh06 — ba ảnh mô phỏng Bến Vạc

29/10/2026 · Sprint13/SPIN-022 · Reviewer độc lập `/root/s13e6_critic_visual`. **Pending user approval.** Phạm vi là hình ảnh concept; không gọi imagegen/Hyper3D, không dựng model hoặc sửa gallery.

## Bằng chứng đã quan sát

Đã đọc applicable baseline, skill mythical-validation và hướng dẫn game-design-review, art direction06, prompt A/B/C, metadata provenance và HTML/CSS gallery. Đã thực sự mở cả ba PNG bằng công cụ xem ảnh: `assets/a-oblique.png` (1672×941), `assets/b-grounded.png` (1672×941), `assets/c-sideview.png` (1672×940), tại `prototypes/sprint13/experiment-06/`. Các nhận xét dưới đây dựa trên những ảnh đó; không nhận automation của root/code critic là lượt kiểm do mình chạy.

Ba ảnh là diễn giải độc lập của cùng tình huống, không phải ba camera dùng chung geometry. Việc thấy chiều sâu trong ảnh B không chứng minh có scene, model hoặc renderer3D thực. Provenance ghi built-in image_gen, chưa nghiệm thu và không phải evidence runtime.

## Những gì đọc được từ ảnh

| Hướng | Quan sát thực tế | Đánh đổi còn thấy |
|---|---|---|
| A — Góc chéo | Lối đất từ nhân vật tới người thợ, thuyền trên hai giá và bến giao hàng cùng hiện trong một khung; sinh hoạt phụ ở nhà sàn bên trái hỗ trợ cảm giác cộng đồng. Nước xám lạnh khác đất/gỗ và mảng lá xanh. | Nhiều vân gỗ, mái, đá, cỏ và dây cùng cạnh tranh độ sắc nét. Người thợ tương đối nhỏ so với toàn cảnh. |
| B — Qua vai | Nhân vật tiền cảnh khiến người xem cảm thấy đang bước vào xưởng. Lòng thuyền, sườn, tấm ván mới, thợ và người đứng ở bến đều đọc được. Vật liệu đất ướt/gỗ/vải/nước phân biệt rõ. | Tiền cảnh nhân vật và bàn chiếm diện tích lớn; tuyến tiếp cận bị che một phần. Diện mạo gần ảnh chụp hơn một game cách điệu. |
| C — Nhìn ngang | Ba nhân vật có vị trí và tư thế riêng trên một dải hoạt động; thợ, thuyền và người chờ dễ đọc bằng silhouette. Trời sáng và sông xám tạo khoảng nghỉ sau công việc. | Tiền cảnh đậm/nhòe và đất vàng ấm tạo cảm giác minh họa có xử lý chiều sâu. Một màn hình ngang chưa cho thấy khả năng khám phá nhiều hướng. |

Cả ba ảnh đều cho thấy **thuyền lòng mở đặt trên giá kê, mái xưởng tách khỏi thân thuyền**. Không còn lỗi gộp thành nhà thuyền của đầu ra05 trong phạm vi ba ảnh đã xem. Điều này chỉ chứng minh ngữ nghĩa minh họa đọc được, không chứng minh cấu tạo thuyền khả thi hoặc khảo cổ chính xác.

Ba camera khác nhau rõ rệt; đây không chỉ là đổi bộ lọc màu. Trọng tâm vẫn là công việc và người dân, không thêm vũ khí/phép thuật để ép cảm giác RPG. Không phát hiện lỗi hình ảnh mức P1 khiến người xem không thể so sánh ba ý tưởng trong phạm vi quan sát. Chi tiết bàn tay/công cụ ở kích thước nhỏ chưa được xem như asset production hoặc kiểm topology; không suy từ việc không thấy lỗi lớn thành nghiệm thu anatomy toàn bộ.

## Findings và câu hỏi còn mở

**UX06-V01 — P2, độ tiết chế của A chưa khớp mục tiêu.** Brief yêu cầu gouache/nét mực chọn lọc và những vùng ít đánh dấu. Ảnh thực lại dày chi tiết bề mặt ở cả mái, đất, cỏ, lá và nước. Màu vật liệu có phân biệt nhưng hierarchy giữa hành động chính và đồ vật phụ còn yếu. Khi thử tiếp hướng A, nên giảm chi tiết và tương phản cục bộ ngoài khu người thợ/thuyền, thay vì chỉ giảm saturation toàn ảnh. Đây là finding art-direction, không phải lỗi code hoặc bằng chứng user không thích A.

**UX06-V02 — P2, B thiên hiện thực hơn mức cách điệu đã định.** Vải, da, gỗ và phản xạ đất ướt trong B có độ tả gần ảnh chụp; ánh sáng cũng có nắng rõ hơn ý định diffuse trung tính trong prompt. Ảnh hữu ích để chọn khoảng cách camera/nhập vai, nhưng chưa đủ để chốt ngôn ngữ “3D với vật liệu vẽ tay”. Nếu chọn B để đi tiếp, cần một lượt thử độ đơn giản hình khối/bề mặt; không dùng ảnh này tự hứa chất lượng bản game hoặc chi phí sản xuất.

**UX06-V03 — P2, C chưa hoàn toàn là những mảng phẳng tiết chế.** Đất vàng nâu chiếm dải giữa và tiền cảnh tối bị nhòe, trong khi prompt muốn tempera/pastel phẳng, ít dấu và đất trung tính. Bầu trời trắng, nước xám và lá tối giúp tránh một lớp vàng phủ toàn bộ, nhưng chưa có căn cứ nói yêu cầu “không cảm giác màu AI” đã được giải quyết. Thử tiếp C có thể giảm vàng đất, bỏ blur tiền cảnh và gom vật liệu thành mảng dứt khoát hơn.

**UX06-U01 — resolved trong phạm vi quan sát cuối.** Source HTML ban đầu có HUD minh họa cố định và nút xem ảnh toàn khung, trong khi art direction yêu cầu tắtUI ngay trên tranh. Reviewer đã báo đề nghị; root thêm checkbox `#show-hud` và cập nhật CSS/JS. Sau sửa, reviewer thực sự mở `tests/results/sprint13-experiment06/gallery-c-1280.png` và `hud-off-1280.png`: ảnh đầu có checkbox và HUD được ghi rõ là minh họa, ảnh sau checkbox bỏ chọn và khung tranh không còn HUD. Điều này đủ xác nhận cách trình bày hiện/ẩn đáp ứng đề nghị trong hai ảnh chụp đã xem. Reviewer không tự chạy browser hoặc nhận phần kiểm sự kiện/phím/focus ở các viewport khác là automation của mình.

Trong ảnh desktop cuối đã quan sát, tranh chiếm phần chính; lớpHUD nằm dưới trái, không đè lên người thợ hoặc thân thuyền chính của C. Tên phương án, nhãn “Ảnh mô phỏng · Chưa phải game chạy · Chờ bạn chốt” và giải thích ba ảnh độc lập hiện rõ. Với A có thể tắt HUD để nhìn lối đi toàn vẹn. Chưa kết luận layout mọi thiết bị từ hai ảnh desktop; phạm vi browser của code critic có evidence riêng.

## Ý kiến định hướng của reviewer

Tôi nghiêng về **A để tiếp tục kiểm góc nhìn cho khám phá** và **C để tiếp tục kiểm trọng tâm nhân vật/đối thoại**. B nên giữ làm đối chứng về cảm giác đứng trong thế giới, vì tính hiện thực và bố cục qua vai khác hẳn hai hướng còn lại. Đây là nhận xét của reviewer, không phải quyết định của chủ dự án hoặc bằng chứng hướng nào dễ làm hơn đã được đo.

Có thể đưa cả ba ảnh cho chủ dự án xem ngay như những lựa chọn có đánh đổi rõ. Không cần dựng model thật để trả lời câu hỏi của vòng này. Cả ba vẫn **Pending user approval**; không có human playtest, Unity/gameplay, navigation, performance hoặc khảo cổ acceptance trong review này. Không đề nghị đóng Sprint13 hoặc chuyển Sprint14.
