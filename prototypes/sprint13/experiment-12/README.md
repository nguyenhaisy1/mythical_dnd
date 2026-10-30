# Thử nghiệm 12 — Sáu hướng mỹ thuật mới

Sprint 13 · SPIN-028 · 30/10/2026 · **Phiên bản tham khảo, Pending user approval**.

Vòng này đáp ứng yêu cầu ít nhất năm mẫu tiếp theo bằng sáu hướng: chất đất nặn thủ công, truyện tranh nét trong, bích họa khoáng màu, diện mạo RPG dựng sẵn đời cũ, than bạc về đêm và hoạt hình mảng màu. Mỗi hướng thử nối cảnh chơi với nhân vật toàn thân, chân dung, vật phẩm và UI hội thoại/túi đồ/nhật ký. [Art direction](ART-DIRECTION.md) ghi ý định và câu hỏi review; brief không tự chứng minh ảnh đã đáp ứng.

## Xem bản tham khảo

Mở [gallery](index.html), hoặc dùng preview khi server đang chạy: <http://127.0.0.1:8016/experiment-12/>. Từ root repository có thể chạy:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Sáu PNG đã tạo ngày 30/10/2026, mỗi ảnh 1672×941 RGB:

| Mẫu | File ảnh | Khác biệt chủ đạo |
|---|---|---|
| Đất biết kể | `assets/clay-craft.png` | Khối đất nặn, dấu tay, ánh sáng mềm và UI thủ công. |
| Nét trong | `assets/clear-line.png` | Đường nét rõ, màu phẳng, nhân vật và cửa sổ có nhịp truyện tranh. |
| Vách thời gian | `assets/mineral-fresco.png` | Mảng khoáng màu, độ sâu nông, nhịp bích họa qua cảnh và UI. |
| Ký ức CD-ROM | `assets/retro-prerender.png` | Nền có diện mạo dựng sẵn, khối nhân vật đời cũ, cửa sổ RPG gọn. |
| Đêm than bạc | `assets/charcoal-night.png` | Than, giấy, ánh bạc và silhouette thay cho quầng màu fantasy. |
| Gió mảng màu | `assets/graphic-animation.png` | Hình không viền, mảng màu lớn, cử chỉ và icon cô đọng. |

Ảnh gốc, prompt thực tế và provenance đã lưu. Tên hướng là ý định mỹ thuật; output không khớp hoàn toàn mọi yêu cầu. Gallery ghi chú quan sát riêng cho từng bản. Xem [báo cáo12](../../../docs/production/sprints/sprint-13/EXPERIMENT-12-REPORT.md) và hai review để đối chiếu kết quả với brief và kiểm gallery thực tế.

Gallery chỉ đổi/xem/phóng lớn ảnh; UI riêng của từng hướng nằm trong raster. Mở toàn khung hoặc PNG gốc để xem chữ và icon. Cỡ chữ trang HTML không làm chữ trong artwork reflow. Cảnh, camera và cast có thể thay đổi giữa các bảng, nên đây là so sánh tổ hợp mỹ thuật, không phải thử nghiệm chỉ đổi một biến.

## Nguồn và phạm vi

Root dùng built-in `image_gen` tạo sáu ảnh, lưu bản gốc nguyên bytes trong `assets/`, bộ prompt ở `prompts.json` và thông tin nguồn/kích thước/hash ở `provenance.json`. Không thay bằng CSS/SVG, không dùng API key hoặc công cụ sinh model cho lượt xem trước này.

Đây là ảnh mô phỏng game, chưa phải game đang chạy, ảnh render từ scene 3D đã dựng, background pre-render production, sprite/tileset hoặc UI runtime. Những nút, lựa chọn, số lượng và lời thoại được vẽ trong ảnh chỉ minh họa. Chưa có evidence animation, camera tự do, parallax, lighting động, pixel grid, hiệu năng hoặc chi phí sản xuất.

Cast và tình huống đời thường giúp thử khả năng diễn đạt tuổi, nghề, gia đình và quan hệ. Không cố định protagonist hoặc tạo màn hình chọn origin; baseline nhân vật ngẫu nhiên không đổi. Trang phục, vật dụng, motif và lời thoại là hư cấu mỹ thuật lấy cảm hứng Việt cổ, chưa là quyết định canon hoặc phục dựng lịch sử.

Giữ toàn bộ vòng 01–11 để quay lại, so sánh hoặc kết hợp. Sáu mẫu mới không tự thay thế, loại hoặc chê các bản trước. [Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) giữ quyền quyết định của chủ dự án; technical review không là final acceptance hoặc quyền chuyển Sprint 14.
