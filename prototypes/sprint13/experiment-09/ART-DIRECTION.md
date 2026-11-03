# Thử nghiệm09 — Một ngôn ngữ hình ảnh cho cả game

Sprint13 · SPIN-025 · 29/10/2026 · **Phiên bản tham khảo, Pending user approval**.

Vòng này thử tính xuyên suốt: thế giới, tạo hình người, chân dung hội thoại, vật phẩm và giao diện phải cùng một cách tạo hình. Một cảnh đẹp đi kèm bộ menu bất kỳ chưa trả lời yêu cầu của chủ dự án. Ba bảng hình ảnh cần cho thấy cách mỗi hướng có thể trở thành diện mạo của một game hoàn chỉnh, chưa phải một art bible đã chốt hoặc tài sản production.

## Bố cục chung để quan sát

Phần chính của mỗi bảng là một màn hình chơi giả lập ở cộng đồng ven sông trong mùa nước: người, đường đi, lao động và một cuộc đối thoại. Vùng còn lại cho thấy tạo hình toàn thân, chân dung cùng nhân vật, vật phẩm và mẫu túi đồ/nhật ký. Không chỉ ghép tên phong cách lên một moodboard. UI trong ảnh cần có ngôn ngữ riêng theo hướng A/B/C; gallery HTML chỉ là khung xem trung tính.

Tình huống và con người là hư cấu minh họa, không bổ sung canon, sự kiện hoặc rule. Không tạo người bằng lựa chọn origin: nếu có bảng cuộc đời, đó là cách trình bày một cuộc đời đã được nhận ngẫu nhiên. Chữ trên ảnh tạo là phần concept, có thể chưa chính xác; chưa là typography tiếng Việt hoặc UI có thể dùng trong engine.

## Ma trận nhất quán

| Thành phần | A — Sơn mài sống | B — Phù điêu đồng | C — Mực sống |
|---|---|---|---|
| Thế giới | Mảng đen sâu, son đỏ, trắng vỏ trứng và vàng tiết chế; hình khối phẳng, bề mặt như lớp sơn được khắc mở. | Cảnh vật như phù điêu đúc và khắc; đồng tối, lớp xanh gỉ, mặt sáng có thứ bậc. Đường đi không chìm trong hoa văn. | Đen/trắng mạnh, khoảng trống lớn, nét mực có nhịp; đỏ chỉ là điểm nhấn có chủ đích, không phủ thành bộ lọc. |
| Nhân vật trong cảnh | Silhouette góc cạnh, tỷ lệ trưởng thành; nghề và tuổi đọc từ dáng, quần áo và đồ mang theo. | Dáng người có độ nổi như phù điêu nhưng vẫn có cử chỉ và gương mặt riêng; tránh biến mọi người thành tượng nghi lễ giống nhau. | Tư thế và nét mặt biểu cảm bằng ít nét; khối đen trang phục tách khỏi nền, không cần tô kín mọi chi tiết. |
| Chân dung hội thoại | Cùng đường nét, mảng son/vỏ trứng và cách chia mặt như nhân vật toàn thân. | Chân dung nổi khối cùng vật liệu với cast, mặt không bị lớp gỉ hay trang trí che mất. | Chân dung dùng nét mực và khoảng trắng như người trong cảnh; đỏ hỗ trợ trạng thái được gắn nhãn, không thay nét mặt. |
| Vật phẩm và icon | Dây, giỏ, chum, công cụ thành hình khắc gọn; icon giữ cùng độ dày và số mảng sáng. | Vật phẩm có silhouette khắc/nổi rõ trong ô; loại vật phẩm không chỉ phân biệt bằng sắc đồng. | Hình mực cô đọng, nhận ra ở cỡ nhỏ; âm/dương nhất quán giữa icon, đường viền và nền. |
| Panel và typography UI | Panel như mặt sơn khắc, mép gọn, điểm vàng có nhiệm vụ; chữ sáng trên vùng đen yên, không phải chữ giả cổ khó đọc. | Rãnh khắc và miếng nền sáng phân cấp; chữ ngà tương phản với đồng tối, hoa văn rút khỏi vùng đọc. | Panel tạo từ khoảng trắng/đen, đường mực và nhịp cắt; chữ sạch, không để nét bút che nội dung. |
| Trạng thái và tương tác minh họa | Lựa chọn hiện hành bằng vạch khắc, nhãn và viền, không chỉ màu son. | Viền nổi/khắc kèm nhãn; chọn, khóa và chưa biết có hình thức khác nhau, không chỉ độ sáng kim loại. | Khung hoặc dấu nét kèm chữ; nền đảo màu phải giữ nội dung và focus đọc được. |
| Chuyển động — giả thuyết chưa tạo | Có thể thử chuyển mảng và nét khắc ngắn để giữ chất sơn; chưa có animation hoặc timing evidence. | Có thể thử dịch lớp relief/cử chỉ có biên độ rõ; chưa có rig, animation hoặc camera thật. | Có thể thử nét xuất hiện/đổi pose gọn; chưa có chuyển động hay hiệu ứng runtime để đánh giá. |
| Đánh đổi cần xem | Cảnh tối và vàng có thể tranh điểm nhìn với UI hoặc làm mất người; cần kiểm vùng đọc và tương phản. | Chất đồng có thể làm mọi người/vật giống nhau; cần ưu tiên nhận diện và thông tin hơn ornament. | Tương phản mạnh có thể mỏi mắt, đỏ có thể bị lạm dụng; cần xem vùng nghỉ, chữ nhỏ và người trên nền đen/trắng. |

Đây là ý định thiết kế, chưa là kết luận rằng ba ảnh đã đáp ứng mọi ô trong bảng. Không dùng chất liệu sơn mài, đồng hoặc mực để khẳng định phục dựng lịch sử. Cũng chưa có căn cứ nói hướng nào rẻ hơn, chạy nhanh hơn hoặc dễ sản xuất hơn.

## Câu hỏi review

1. Có nhận ra cùng một người khi nhìn silhouette, toàn thân và chân dung không? Tuổi/nghề/cử chỉ có còn đa dạng?
2. Cảnh chơi, vật phẩm và menu có chung nét, hình khối, giá trị sáng tối và cách dùng chất liệu không?
3. Ở cỡ màn hình chơi, còn nhìn được người, lối đi, đối thoại và lựa chọn? Chữ tiếng Việt, contrast và trạng thái cần được kiểm riêng khi làm UI thật.
4. Hướng nào gợi được đời sống/nghề nghiệp/quan hệ bên cạnh yếu tố huyền thoại, thay vì ép mọi hình ảnh về chiến đấu?
5. Artwork thiếu hoặc lệch ý định ở đâu? Ghi đúng ảnh và phạm vi, không biến một đánh đổi thành kết luận toàn bộ ý tưởng dở.

Giữ01–08 và toàn bộ nguồn để tham khảo hoặc kết hợp. Vòng09 bổ sung thư viện, không loại bản trước. Mọi hướng vẫn chờ chủ dự án chốt; không có Unity, gameplay, human playtest hoặc final art acceptance trong tài liệu này.
