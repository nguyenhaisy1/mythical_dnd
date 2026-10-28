# Thử nghiệm03 — Ba cách nhìn Bến Vạc

Sprint13 / SPIN-019 · **Pending user approval.** Người dùng nhận xét bản02 đẹp hơn nhưng cách dùng màu vẫn tạo cảm giác “AI”. Đây là phản hồi thẩm mỹ thật cần kiểm bằng phương án mới; không đồng nghĩa mọi hình được tạo bằng AI đều có một dấu hiệu màu cố định hay có thể chứng minh “không nhận ra AI”. `/root/s13e3_direction` đề xuất hướng; `/root` tạo ảnh và tổng hợp.

## Câu hỏi chung

Giữ Bến Vạc, hoạt động sửa thuyền và đời sống ven sông để đối chiếu. Thay đồng thời góc nhìn, tổ chức khối, chất liệu thể hiện và cách phân bố màu; không chỉ phủ bộ lọc lên bản02. Ba hình là concept độc lập, chưa bảo đảm cùng một mặt bằng có thể đo đạc hoặc ghép thành bản đồ runtime.

## A — Sáng âm, ngang tầm người

**Giả thuyết:** ánh sáng trời phủ đều vừa phải, màu cục bộ của gỗ cũ/vải/sợi/nước và một góc đứng gần người lao động có thể làm cảnh bớt cảm giác được “chấm màu điện ảnh” toàn ảnh. Camera ngang tầm mắt ở bến, bố cục lệch tự nhiên; người đang sửa và cấu trúc thuyền là trọng tâm. Trời nhiều mây, nước phản chiếu màu trời xám nhạt, bóng không nhuộm xanh ngọc; gỗ nâu xám, mái lá bạc, cây có nhiều sắc xanh xỉn. Không tự động dùng tia nắng vàng, viền sáng quanh mọi vật hoặc sương như phông trang trí.

**Khả năng đọc và đánh đổi:** khác biệt sáng/tối giữa mặt gỗ, dụng cụ và tay đang làm việc giúp đọc nghề nghiệp, không cần đẩy saturation. Giữ chi tiết vừa đủ ở điểm làm việc, cho phép vật liệu khác có độ sắc khác nhau. Hướng này gần trải nghiệm sinh hoạt và quan sát nhưng có thể ít “huyền thoại”, ít thấy mạng đường/bố cục làng. Cần xem ở kích thước game xem thợ, mép thuyền và lối đi còn rõ khi không có ánh sáng kịch tính nâng đỡ.

## B — Gouache khoáng, góc chéo từ cao

**Giả thuyết:** một góc nhìn oblique cao thấy rõ bến, thuyền, nhà và khoảng nước, thể hiện bằng gouache có nét cọ và lớp phủ đục, tạo được ngôn ngữ hình ảnh game riêng mà không cần cảm giác render chân thực của bản02. Màu tổ chức theo chất liệu: vải lanh ngà, bùn đất xám đỏ, nước xám lam, lá xanh rêu; các khoảng màu lớn có chủ đích. Nét cọ và độ đậm nhạt giữ dấu tác giả, tránh gradient bóng mượt chạy đều trên từng đồ vật. Ánh ban ngày trung tính, không gắn mặc định cặp vàng–xanh vào vùng sáng và vùng tối.

**Khả năng đọc và đánh đổi:** mái, sàn, đường bờ và thân thuyền phải tách thành silhouette rõ; công việc và tuyến tiếp cận đọc được qua hình khối trước texture. Hướng này thuận lợi cho bố cục khám phá hoặc màn bản đồ có tương tác, nhưng người lao động nhỏ hơn nên ít cảm giác thân mật và có nguy cơ thành mô hình trưng bày. Camera concept chưa phải camera Unity đã chốt, phối cảnh trong tranh chưa là dữ liệu độ cao/collision; cần tránh hứa khả năng di chuyển mà một hình nền không cung cấp.

## C — Tranh khắc màu, dấu nét thủ công

**Giả thuyết:** hạn chế số sắc chính và dùng nét khắc, vết in cùng mảng tối có tiết tấu có thể tạo bản sắc mạnh hơn việc tăng chi tiết painterly. Bảng thử gồm đen than, giấy ngà, đỏ đất và chàm xỉn; màu đi theo vật và nhịp bố cục, không dùng chàm làm mọi bóng và đỏ làm mọi highlight. Góc nhìn hơi thấp hoặc ngang bến, dùng dây, cọc, mạng lưới và đường ván tạo nhịp; người và thuyền là cụm hình chính. Dấu khắc/vết in cần có hướng theo khối, không là lớp noise phủ ảnh; không quay lại các mảng vector sạch của bản01.

**Khả năng đọc và đánh đổi:** nét đen, khoảng giấy và tương phản khối giúp đọc người/thuyền trong bảng màu ít sắc; hình dạng và nhãn UI phải mang thông tin chứ không dựa màu đơn độc. Hướng này có thể hợp key art, chương truyện hoặc ngôn ngữ UI riêng, nhưng dễ mất chi tiết vật liệu, trở nên áp phích hơn không gian có thể sống trong đó. Chưa xem sản phẩm thì chưa kết luận nó phù hợp màn World dài hạn; cần kiểm độ mỏi mắt và chi tiết nhỏ ở kích thước hiển thị thật.

## Cách so sánh và kiểm

Gallery đặt ba ảnh trên cùng nền trung tính, giữ tỷ lệ nguồn; không thêm tint, vignette hoặc color grading để “hòa UI”. Cho xem nguyên ảnh, phóng vừa khung và quay lại bản02. Nếu sau này thử HUD lên ảnh, không đặt bảng trên tiêu điểm người/thuyền như finding của bản02. Tiêu chí đối chiếu: cảm giác vật liệu/màu có chủ đích; bản sắc; điều người xem nhìn đầu tiên; việc sửa thuyền có đọc được; mức muốn ở trong cảnh và chi phí duy trì phong cách qua nhiều asset.

Root/reviewer phải nhìn ảnh đã tạo để kiểm anatomy, kết cấu thuyền, đồ vật dính/nhòe, ánh sáng tự mâu thuẫn, chi tiết giống nhau lặp lại và mức phù hợp từng hướng. Không có số đo ảnh hoặc nhận xét user thì giữ kết quả chưa biết; contrast giao diện không chứng minh chất lượng mỹ thuật. Không ghi “AI-undetectable”, “authentic” hoặc “historically accurate” từ việc đổi palette.

Bến Vạc/Tam Giang và sinh hoạt hộ gia đình/nghề nghiệp theo [bối cảnh project](../../../.agent). Kiểu nhà, dụng cụ, trang phục, ánh sáng và nét in là hư cấu thị giác chưa có kiểm chứng khảo cổ riêng. Ảnh mới do root tạo offline bằng công cụ; provenance/prompt/kích thước thực ghi khi có output. Chúng chưa là layer/mesh/collision/animation, không chứng minh Unity gameplay hoặc human playtest. Giữ nguyên bản01/02; mọi hướng và lựa chọn kết thúcSprint13 vẫn chờ chủ dự án chốt.
