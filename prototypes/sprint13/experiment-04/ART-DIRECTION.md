# Thử nghiệm04 — Bến Vạc như một không gian 3D

SPIN-020, Sprint13. **Pending user approval.** Ảnh B-gouache của thử nghiệm03 là tham chiếu bố cục và đời sống ven sông, không là bản vẽ đo đạc hoặc hướng mỹ thuật đã được chủ dự án chốt.

## Câu hỏi của vòng thử

Một mô hình thật có giữ được quan hệ nhà–đường xuống bến–khu sửa thuyền–mặt nước khi đổi camera không? So sánh góc cao đọc không gian, góc đứng bên bờ và góc gần thân thuyền trên cùng geometry. Không dùng ba ảnh sinh độc lập rồi gọi là ba camera của một scene.

## Tổ chức cảnh và hình thức

Nhà sàn ở đất cao, sàn có cột chống, cầu thang nhiều bậc đi xuống đất. Mái hai dốc được tạo từ các lớp bó nhỏ chồng lên nhau; tường/ván sàn/bến có cấu trúc riêng. Bến kéo ra mặt nước với cột, dầm và thanh giằng; thuyền sửa có vỏ cong mở lòng, lớp trong/ngoài và sườn, đặt trên hai giá. Thuyền nhỏ trên nước có mái che thấp, ghế và mái chèo. Dây, giỏ, chum, lưới, ván nguyên liệu và ba hình người làm rõ tỷ lệ và hoạt động.

Đây là diorama có chủ ý: bank và khối nước được cắt thành phạm vi nhỏ để nhìn mô hình. Vật liệu mờ, gỗ xám/nâu, đất trầm, nước xám xanh và ít vải đỏ gạch. Ánh sáng tán xạ trung tính, không tạo đèn màu vàng–xanh để phủ toàn cảnh. Blender dùng AgX và key/fill mềm; viewer có ánh sáng riêng nên sắc độ render và realtime có thể khác dù cùng màu vật liệu/geometry.

Ba người là hình tỷ lệ giản lược, không phải character art cuối; cây lá và bề mặt cũng còn cách điệu. Kết quả có thể gần miniature hơn tranh gouache tham chiếu. Đây là một đánh đổi cần nhìn trực tiếp, không được ghi “đạt phong cách” chỉ vì đã xuất GLB.

## Camera và khả năng tái tạo

`build_scene.py` tạo scene từ seed cố định13042026, không tải asset ngoài. Blender source dùng Z-up; GLB và viewer dùng Y-up với mapping `(x,z,-y)`. Camera xuất theo cùng vị trí/target, tiêu cự và verticalFOV được tính từ sensor ngang36mm cùng tỷ lệ render1280/900. Viewer thay aspect theo cửa sổ nhưng giữ verticalFOV mỗi preset. Góc đứng đặt cao khoảng1,7m trên mặt bờ tại điểm nhìn; góc sửa thuyền cho phép crop ngôi nhà để tập trung hình học thân thuyền.

`scene.blend` là scene authoring; `ben-vac.glb` và `scene-data.js` chứa cùng GLB bytes (base64 cho offline file://). Geometry được batch theo26vật liệu để tránh hàng nghìn draw calls. Noise/bump của Blender không được bake sang GLB; viewer nhận màu nền và roughness, không giả rằng có toàn bộ texture painterly hoặc ánh sáng baked. Three ảnh render cũng dùng cùng mô hình, không thêm người/đồ vật bằng hậu kỳ.

## Nguồn và giới hạn

Mọi geometry được dựng bằng script của project, không dùng ảnh hay model bên thứ ba. Tham chiếu do chủ dự án gửi tương ứng `experiment-03/assets/b-gouache.png`, ảnh tạo bằng built-inimagegen ở vòng trước. Vòng này dùng Blender thực, không gọi Hyper3D hoặc imagegen để giả lập mô hình3D.

Nhà, thuyền, trang phục, đồ nghề và cây là hư cấu thị giác cho Bến Vạc/Tam Giang, chưa có chứng cứ khảo cổ riêng. Không coi kích thước mesh là canon, không gameplay/navigation/physics/Unity acceptance. Chủ dự án chốt hướng hình ảnh, camera và bước tiếp theo; technical review không chuyển trạng thái Pending.
