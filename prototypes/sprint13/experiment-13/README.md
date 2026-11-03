# Thử nghiệm 13 — Ba cách nhìn, mỗi cách một thế giới và một cuộc gặp

Sprint13 · SPIN-030 · 30/10/2026 · **Phiên bản tham khảo, Pending user approval**.

Vòng này áp dụng workshop đồ họa: tạo khung chơi nguyên màn hình với quy tắc khối/tỷ lệ/camera rõ, rồi kiểm một trạng thái hội thoại riêng. Ba hướng là3Dkhối nhìn chéo,2.5Ddải sinh hoạt và3Dcận người. Không lặp bảng ghép cảnh65/35hay dải asset cố định. [ART-DIRECTION.md](ART-DIRECTION.md) ghi ý định, delta so với bản cũ và những dấu hiệu cần quan sát.

## Bộ frame trong brief

| Hướng | Frame thế giới | Frame đối thoại | Khác biệt đang thử |
|---|---|---|---|
| Khối nhìn chéo | `assets/block-world.png` | `assets/block-dialogue.png` | Khối lớn, tỷ lệ phóng đại, matte/ít bóng ở cả người và cảnh; góc cao vừa |
| Dải sinh hoạt | `assets/frieze-world.png` | `assets/frieze-dialogue.png` | Hình2Dphẳng, silhouette góc cạnh, cao độ và nhóm hoạt động theo dải |
| Gần tầm người | `assets/near-world.png` | `assets/near-dialogue.png` | Giữ ý định grammar khối nhưng kéo camera gần cơ thể/cuộc gặp |

Sáu frame này đã được tạo và lưu nguyên bytes tại `assets/`; tên hướng vẫn là ý định, không tự chứng nhận mức khớp brief. `prompts.json` giữ sáu prompt ban đầu và một prompt sửa; `provenance.json` giữ nguồn/kích thước/SHA, mapping prompt và chuỗi ảnh tham chiếu. Báo cáo13 ghi output đã quan sát và các giới hạn.

Cùng tình huống bến/thuyền/người thợ/lối lên sàn và cast minh họa giúp đối chiếu. Các khung là ảnh tạo riêng, một số có dùng PNG trước làm tham chiếu; chúng không phải sáu camera/state từ cùng scene hoặc geometry thật; có thể lệch identity, tỷ lệ và bố cục. Hướng nhìn chéo/cận người tìm sự thống nhất hình, không hứa matching tuyệt đối.

## Cách xem

Mở `index.html` khi gallery đã được tạo và asset đủ, hoặc preview khi server đang chạy:

<http://127.0.0.1:8016/experiment-13/>

Từ root repository có thể chạy:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Dùng điều khiển gallery để đổi hướng và frame, mở toàn khung/PNG gốc khi cần đọc chi tiết. Cỡ chữ HTML chỉ tác động trang xem; chữ trong UI raster không reflow. Gallery không là client game hoặc bằng chứng tương tác hội thoại.

## Nguồn và phạm vi

Root tạo ảnh bằng built-in `image_gen`; không dùng Hyper3D/Blender hoặc dựng model thật trong lượt này. Đây là ảnh mô phỏng diện mạo game, không phải game đang chạy, render từ scene3Dđã dựng, animation, sprite/tileset hoặc shader production.

Người phụ nữ khoảng40tuổi và người thợ lớn tuổi là cast để thử dáng/nghề/biểu cảm, không cố định protagonist và không cho chọn origin. Các nhãn, câu thoại và lựa chọn trong ảnh chỉ minh họa, không thêm canon, quest effect, repair rule hoặc tri thức runtime. Chất liệu văn hóa được dùng theo chuỗi nguồn→biến đổi mỹ thuật→dấu hiệu hình; không gọi phục trang, mái hoặc palette mới là phục dựng đã kiểm chứng.

[Workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [nghiên cứu văn hóa](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md) giữ căn cứ và giới hạn. Chưa có kiểm chứng motion, rig, camera tự do, pathing, collision, hiệu năng, chi phí sản xuất hoặc human playtest. Báo cáo/review vòng13 sẽ ghi evidence thực sau khi thực hiện, không dùng brief này để tự tuyên bố output Passed.

Giữ toàn bộ phiên bản01–12. Vòng mới không loại bản cũ, không chốt một hướng/engine, đóngSprint13 hoặc chuyểnSprint14. Quyền quyết định vẫn ở [approval register](../../../docs/production/sprints/sprint-13/APPROVALS.md).

## Trạng thái artifact sau generation

Ngày30/10 đã tạo sáu PNG đang trưng bày và một biến thể `assets/frieze-world-v1.png` được giữ riêng. Bản `frieze-world.png` là lượt sửa dùng v1 làm tham chiếu để làm phẳng cấu trúc, không phải hậu kỳ bằng code. Các frame gần/đối thoại dùng PNG trước làm tham chiếu tạo hình; mapping, prompt và SHA reference nằm trong provenance. Không khẳng định các khung là ảnh từ cùng geometry/camera thật.

Sáu ảnh1672×941RGB, riêng frieze-world cuối1671×941RGB. Tất cả sao chép nguyên bytes, không crop/hậu kỳ. Tên hướng là ý định; gallery ghi quan sát và phần lệch brief dưới từng card. Báo cáo13 và hai review phân biệt output, kiểm gallery và quyết định chủ dự án. Các đoạn mô tả ý định ban đầu ở trên không tự là kết quả đã đạt.
