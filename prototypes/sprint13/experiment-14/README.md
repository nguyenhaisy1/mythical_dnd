# Thử nghiệm14 — Sáu hướng hình ảnh độc lập

30/10/2026 · Sprint13 · SPIN-031 · **Thư viện tham khảo, Pending user approval**.

Vòng14 đáp ứng nhận xét của chủ dự án: các ảnh vòng13 đã tốt hơn nhưng còn gần nhau. Sáu hướng mới mở rộng đồng thời camera, cấu trúc hình, tỷ lệ người, bối cảnh, cast, màu và UI. Mỗi hướng có một ảnh màn hình chơi; không dùng ảnh13 làm tham chiếu để giữ cùng họ tạo hình, không lặp công thức ba hướng × hai cự ly.

## Xem và đối chiếu

Mở [gallery](index.html); preview dùng <http://127.0.0.1:8016/experiment-14/> khi server đang chạy. Từ root repository có thể chạy:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

| Hướng | PNG đã lưu | Câu hỏi khác biệt |
|---|---|---|
| Đất dựng thành | `assets/earth-monument.png` | Khối góc cạnh, người dài và quy mô đất đắp có đổi cách đọc thế giới không? |
| Ô đất sống | `assets/living-grid.png` | Plan view2D90độ có tạo một vùng khám phá có người riêng, thay vì ảnh isometric thu nhỏ? |
| Làng vòng cung | `assets/round-world.png` | Khối mềm và phép nhìn cuộn có giữ tuổi/nghề và lối đi rõ không? |
| Đôi tay làm nghề | `assets/craft-first-person.png` | Tay, công cụ, người đối diện và lựa chọn có thành một màn hình chơi nghề nghiệp? |
| Ký hình | `assets/glyph-world.png` | Thế giới, người, vật và UI có thể đọc bằng cùng một bộ dấu hình? |
| Mái nhà nhiều giọng | `assets/comic-life.png` | Trang truyện có lựa chọn có diễn đạt được không gian, người nói và đời sống gia đình? |

Sáu PNG đã tạo ngày30/10/2026 bằng built-in image_gen, mỗi ảnh1672×941RGB, sao chép nguyên bytes. Không có ảnh tham chiếu hoặc lấy ảnh13 làm input. `prompts.json` giữ nguyên sáu prompt; `provenance.json` ghi nguồn/kích thước/mode/bytes/SHA. Tên hướng là ý định, không chứng nhận mọi camera hoặc mức giản lược đã đạt. Gallery ghi quan sát riêng dưới từng card. [Art direction](ART-DIRECTION.md) ghi quy tắc hình, delta với phương pháp cũ, rủi ro và các câu hỏi quan sát; báo cáo/review sau đó phải căn cứ output thật.

Gallery chỉ phục vụ chọn/xem/phóng lớn ảnh. UI vẽ trong PNG là raster minh họa; nút/ô thoại trên ảnh chưa có interaction. Chữ HTML và chữ trong artwork là hai lớp khác nhau; tăng cỡ chữ trang xem không làm chữ raster reflow. Một màn hình mỗi hướng chưa chứng minh mọi màn hình túi đồ/nhật ký, animation, icon hoặc roster production.

## Phạm vi và nguồn

Đây là ảnh mô phỏng game bằng built-in image_gen, không model3D thật, render từ scene đã dựng, UI runtime hoặc gameplay implementation. Camera cong của Làng vòng cung là đề xuất phép nhìn, không một hành tinh nhỏ hoặc thay đổi world geometry. Ký hình dùng dấu hình đương đại, không nhận là chữ viết cổ. Trang truyện của Mái nhà nhiều giọng là một ý tưởng presentation, không tự đổi game thành visual novel hoặc chứng minh chuỗi state đã chạy.

Căn cứ và giới hạn văn hóa nằm trong [workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) cùng [nghiên cứu ngữ pháp](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md). Bối cảnh theo vùng, trang phục, palette, tỷ lệ, kiến trúc, công cụ và lời thoại vẫn là đề xuất sáng tạo; không tự thêm canon hoặc chứng nhận phục dựng lịch sử.

Các ảnh thay nhiều biến có chủ ý để tìm khoảng mới. Nếu thích một bản, phản hồi ban đầu chọn được một tổ hợp để đào sâu, chưa xác định được nguyên nhân riêng do camera, màu hay yếu tố văn hóa. Cast chỉ minh họa cuộc đời được nhận ngẫu nhiên; không có character creator hoặc quyền chọn/chỉnh origin. UI không được mở hidden truth hoặc ghi authoritative state.

Giữ toàn bộ01–13 và workshop để so sánh/kết hợp; vòng mới không tự loại hoặc chê các bản cũ. [Approval register](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyết định của chủ dự án. Mọi hướng vẫn Pending user approval, chưa đóngSprint13, merge dev hoặc chuyểnSprint14.
