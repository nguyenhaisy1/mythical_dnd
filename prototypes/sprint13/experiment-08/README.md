# Thử nghiệm 08 — Thêm phiên bản hình ảnh để tham khảo

Sprint 13 · SPIN-024 · 29/10/2026 · **Chờ chủ dự án chốt**.

Ba hướng cùng thử cảnh một nhà chữa trị bên suối sau mưa: A nét hoạt hình vẽ tay với mảng bóng gọn; B thế giới dệt bằng sợi/vải; C diện mạo3D cách điệu với hình khối rõ. Cả ba đều là ảnh raster độc lập, không phải camera của cùng một model.

**Vòng mới là thêm lựa chọn, không mặc định thay thế hoặc loại bản cũ.** Các phiên bản01–07 được giữ nguyên như thư viện tham khảo. Chưa cần xếp hạng hoặc chọn một bản thắng để tiếp tục khám phá.

## Xem sản phẩm

Mở `index.html` trong trình duyệt khi gallery và ảnh đã có. Nếu dùng server xem trước, mở `http://127.0.0.1:8016/experiment-08/`. Có thể tự chạy từ root repository:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Bộ ảnh dự kiến là `assets/a-cel.png`, `assets/b-woven.png`, `assets/c-faceted.png`. Prompt, provenance và kết quả tạo ảnh/kiểm gallery do root ghi sau khi thực sự có output; các đường dẫn dự kiến không phải bằng chứng generation đã hoàn tất. Giao diện chỉ phục vụ xem ảnh và hình dung cách trình bày, không thực thi hành động trong game.

[Art direction](ART-DIRECTION.md) ghi tình huống chung và điều cần quan sát. Giữ cùng cảnh giúp so đồ họa thuận tiện hơn, nhưng các ảnh tạo riêng có thể khác bố cục/nhân vật/ánh sáng nên không cô lập được hoàn toàn nguyên nhân sở thích.

## Phạm vi

Ảnh dùng built-in image_gen; không tạo model3D, gọi Hyper3D hoặc dựng Blender. Nhà chữa trị, nhân vật và hoạt động trong ảnh là đề xuất minh họa hư cấu, chưa là canon mới, gameplay chữa trị hoặc tư vấn y học. Không có human playtest, Unity/gameplay acceptance hoặc kiểm chứng khảo cổ trong phạm vi này. Không suy khả năng sản xuất hoặc hiệu năng từ phong cách ảnh.

[Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) vẫn quyết định nghiệm thu Sprint13. Lời khen hoặc yêu cầu nghĩ thêm không tự duyệt hướng A/B/C, không cho phép đóngSprint13 hoặc chuyểnSprint14.

## Nguồn ảnh

[Prompts](prompts.json) lưu nguyên văn ba prompt đã dùng với built-inimage_gen; [provenance](provenance.json) có tên output gốc, SHA256 và kích thước. Ba PNG1672×941 được sao chép nguyên bytes vào assets, không hậu kỳ hoặc tạo model3D thật. Các vòng01–07 giữ nguyên để xem lại, không bị thay thế bởi vòng08.

## Kiểm trang xem ảnh

`python3 prototypes/sprint13/experiment-08/browser-check.py` dùng Python Playwright/Pillow và Chrome tại `/usr/bin/google-chrome`. Critic code đã kiểm desktop/mobile, ảnh/provenance, HUD/phím/modal/focus/toggle/chữ150% và bảy liên kết lịch sử. Evidence `tests/results/sprint13-experiment08/` bị ignore. Kết quả chỉ kiểm gallery, không xếp hạng đồ họa hoặc kiểm gameplay.
