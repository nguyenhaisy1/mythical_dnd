# Thử nghiệm15 — Sáu cách đẩy xa hơn ba hướng được thích

30/10/2026 · Sprint13 · SPIN-032 · **Thư viện tham khảo, Pending user approval**.

Chủ dự án đánh giá rất tốt **Đất dựng thành, Ô đất sống, Làng vòng cung** của vòng14 và cho phép đào sâu, đồng thời yêu cầu ý tưởng mới táo bạo hơn. Vòng15 có ba nhánh từ nhóm ưu tiên ấy và ba hướng biểu đạt mới. Đó là ghi nhận sở thích/ưu tiên nghiên cứu, chưa chọn một style cuối hoặc duyệt Sprint13. Giữ tất cả01–14 để xem lại hoặc kết hợp.

## Xem ý tưởng

Mở [gallery](index.html); preview tại <http://127.0.0.1:8016/experiment-15/> khi server đang chạy. Từ root repository có thể chạy:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

| Hướng | PNG được giao | Câu hỏi cần kiểm |
|---|---|---|
| Thành dựng đứng | `assets/vertical-earth.png` | Tổ chức tầng/hiên/đường theo chiều đứng có giữ đời sống con người giữa quy mô lớn? |
| Ghép đảo thủy triều | `assets/tidal-mosaic.png` | Địa bàn ghép bằng mảnh đất–nước có đọc được một cuộc đời, không thành bảng điều hành? |
| Làng trên vành trống | `assets/rolling-world.png` | Phép cong có thể hiện trên cả trường cảnh, thay vì chỉ đường đi uốn? |
| Địa hình đa diện | `assets/cubist-space.png` | Nhiều mặt nhìn có tạo cách đọc riêng mà vẫn rõ protagonist, nơi đứng và cuộc gặp? |
| Làng của bóng | `assets/shadow-world.png` | Khối gần trắng và bóng có thay đổi cách nhìn không gian/người, không chỉ đổi palette? |
| Thủy kính | `assets/mirror-water.png` | Phản chiếu có thành cấu trúc màn hình chơi mà không mất hướng hoặc lộ hidden truth? |

Đã tạo sáu frame trưng bày và giữ `assets/cubist-space-v1.png` qua liên kết riêng. Bảy PNG1672×941RGB được tạo mới bằng built-in image_gen, không ảnh input/reference; quan hệ shortlist là nguồn ý tưởng. `prompts.json` giữ sáu prompt đầu và một iteration mới; `provenance.json` giữ original/SHA/bytes/mapping prompt. Các PNG sao chép nguyên bytes, không hậu kỳ. Tên hướng là ý định, không chứng nhận mọi camera hoặc quy tắc hình đã đạt; gallery ghi giới hạn dưới từng card. [Art direction](ART-DIRECTION.md) mô tả cách tạo hình, nguồn→biến đổi, delta và đánh đổi. Báo cáo cùng review sau generation phân biệt ý định với đầu ra nhìn thấy.

Gallery chỉ chọn/xem/phóng ảnh. UI trong PNG là raster minh họa; chữ/nút trên ảnh chưa có tương tác. Tăng cỡ chữ HTML không làm chữ raster reflow. Một màn hình mỗi hướng chưa là toàn bộ UI túi đồ/nhật ký, roster, animation hoặc bộ asset production.

## Phạm vi và giới hạn

Đây là ảnh mô phỏng game, không model/scene3D thật, render từ scene đã dựng, shader hoặc gameplay implementation. Những phép chiều đứng, hình ghép, cong, đa góc nhìn, bóng và phản chiếu là đề xuất presentation. Chúng không tự thay world geometry, route/time, collision, tri thức, physical truth hoặc canon. Ô hình không là luật hex; vành trống không là hành tinh hoặc thế giới nằm trên trống; bóng không là ma thuật; mặt nước không là portal/remote-view biết bí mật.

Căn cứ văn hóa nằm trong [workshop](../../../docs/production/sprints/sprint-13/ART-DIRECTION-RETHINK.md) và [hồ sơ ngữ pháp](../../../docs/production/sprints/sprint-13/art-rethink/02-CULTURAL-GRAMMAR.md). Tỷ lệ, palette, camera, hình nhà/công cụ/trang phục là lựa chọn mỹ thuật cần nguồn khi đào sâu, chưa là phục dựng lịch sử được xác nhận. Cast là ví dụ cuộc đời được nhận ngẫu nhiên; không character creator hoặc protagonist production cố định. UI chỉ có thể trình bày thông tin nhân vật được phép biết.

Các ảnh cố ý đổi nhiều trục để tìm khoảng mới. Phản hồi trước tiên đánh giá từng tổ hợp; chưa quy sở thích riêng cho camera, medium, màu hoặc căn cứ văn hóa. Không dùng tên hướng hoặc số file làm bằng chứng đã “điên rồ hơn”, có vibe hoặc được duyệt.

[Approval register](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền quyết định của chủ dự án. Ba hướng14/1–3 được ưu tiên đào sâu; mọi output15 và các mục chưa chốt vẫn Pending. Không tự merge dev, đóngSprint13, chuyểnSprint14 hoặc phát hành.
