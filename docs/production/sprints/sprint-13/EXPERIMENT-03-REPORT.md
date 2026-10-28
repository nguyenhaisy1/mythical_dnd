# Thử nghiệm03 — Một bến sông, ba cách nhìn

Sprint13, SPIN-019, ngày28/10/2026. **Chờ chủ dự án chốt.** User nhận xét bản02 đẹp hơn nhiều nhưng màu có cảm giác AI dễ nhận ra và muốn thử nhiều góc độ. Lời khen không được hiểu là duyệt bản02.

## Kết quả nhìn thấy được

Đã tạo ba concept mới và [gallery chạy offline](../../../../prototypes/sprint13/experiment-03/index.html), có xem từng hướng, đặt cạnh nhau và thêm bản02 để đối chiếu trên cùng nền trung tính. Ảnh được hiển thị nguyên tỷ lệ và màu gốc, không bị CSS tint hoặc cắt khung. [README](../../../../prototypes/sprint13/experiment-03/README.md) có cách mở và chạy checks.

| Hướng | Thay đổi chính | Đánh đổi cần user xem |
|---|---|---|
| A — Trời âm | Ngang tầm người trong xưởng; gỗ xám/nước nâu, ánh sáng trung tính | Gần đời sống nhưng output tả thực gần ảnh, chưa đạt nét cọ rõ như prompt định hướng |
| B — Nhìn chéo | Góc cao thấy nhà, đường đất, bến và thuyền; mảng gouache, nước xám lam | Dễ đọc không gian game hơn nhưng con người nhỏ, có nguy cơ cảm giác mô hình trưng bày |
| C — Nét khắc | Nét in đậm, mảng màu giới hạn, tổ chức người/thuyền thành nhịp đồ họa | Bản sắc mạnh hơn nhưng dày nét, có thể mỏi mắt hoặc khó đọc tương tác khi thu nhỏ |

Root nghiêng về **B để thử camera và không gian chơi**, A là đối chiếu hữu ích về màu vật liệu, C đáng xem cho key art/chương truyện hoặc UI. Đây là nhận xét thiết kế của AI, không là quyết định của chủ dự án hay kết quả playtest.

## Cách tạo và giới hạn hình ảnh

Ba ảnh PNG rộng1672px, cao940px (A)/941px (B/C) tạo mới bằng built-in image_gen, không dùng CLI/APIkey, không chỉnh ảnh02 hoặc áp filter hậu kỳ. [Prompt đầy đủ](../../../../prototypes/sprint13/experiment-03/assets/GENERATION.md) và [Art Direction](../../../../prototypes/sprint13/experiment-03/ART-DIRECTION.md) phân biệt ý đồ với output thật. Tổng PNG khoảng10MB; giữ asset trong repository, không để consumer trỏ vào cache ngoài.

Đổi ánh sáng/camera/chất liệu giúp có phương án để so sánh, không chứng minh hình không còn dấu hiệu AI. A vẫn có chi tiết văn hóa/trang phục cần nghiên cứu; B còn kết cấu bến/thuyền cần review trước khi biến thành asset; C nhiều nét và hình vật liệu có thể hòa lẫn. Không thấy lỗi giải phẫu chặn rõ ở kích thước kiểm, nhưng không phải kiểm anatomy/asset production toàn diện. Không khẳng định phục dựng lịch sử, đồng bộ địa hình giữa ba concept hoặc có sẵn collision/mesh/layers.

Hyper3D đã setup ởSPIN018 nhưng chưa hiện trong tool catalog của thread hiện tại; không dùng MCP đó cho ba ảnh này và không báo đã sinh3D. Deliverable của vòng này là art direction/gallery, không mở thêm gameplay hoặc Unity milestone.

## Sáu vai trò thực tế

- /root/s13e3_direction: ba hypotheses, art direction, tradeoffs và criteria.
- /root/s13e3_gallery: HTML/CSS/JS so sánh trung tính, fullview và keyboard.
- /root/s13e3_checks: browser-check/README và thực thi checks sau khi ảnh có.
- /root/s13_critic_code: review độc lập mã, ảnh, trạng thái gallery và HTTP links.
- /root/s13_critic_ux: review trực tiếp ba ảnh, so với02 và kiểm visual/UX.
- /root: prompts và tạo ảnh, lưu assets, tích hợp, xem screenshot, tài liệu/Git và kiểm lớp đầu.

Ba workers song song, critics lượt sau do giới hạn4concurrent. [Critic code](CRITIC-CODE-03.md), [critic UX](CRITIC-UX-03.md). Không bịa human playtest hoặc tác nhân đã tạo tranh thủ công.

## Bằng chứng và trạng thái

Worker chạy `python3 prototypes/sprint13/experiment-03/browser-check.py` exit0:1280×720/1920×1080, ba ảnh decode/chuyển đúng, Arrow/Home/End, fullview/Escape/focus, compare3+reference02, chữ150% không tràn ngang, không art filter và HTTP02 trả200. Critic code chạy kiểm độc lập bằng Chrome156 trênHTTP, không pageerror, hashes nguồn ghi trong report. Root đã nhìn screenshot so sánh1920 thật và ba output; không gọi DOM assertion là quan sát thị giác.

Evidence và screenshots local: tests/results/sprint13-experiment03/. Những lỗi ban đầu của script checker được sửa trước final exit0; không gán chúng là lỗi sản phẩm. Các thay đổi label nhỏ được tái kiểm trước push. Không sửa kernel nên không coi CI kernel là bằng chứng mỹ thuật.

Giữ01/02nguyênvẹn. A01 cần user chọn/đề nghị hướng, A02/A03/A04 chưa chốt. Không merge dev, đóngSprint13 hay sangSprint14. Các vòng sau có thể phát triển hướng đã chọn hoặc ghép yếu tố cụ thể, nhưng chưa được suy ra lựa chọn từ im lặng.
