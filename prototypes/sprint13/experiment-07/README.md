# Thử nghiệm 07 — Đổi cả đồ họa và ý tưởng cảnh

Sprint 13 · SPIN-023 · 29/10/2026 · **Chờ chủ dự án chốt**.

Vòng này thử ba ngôn ngữ đồ họa khác nhau: A tranh khắc gỗ với làng mùa nước/kho lương; B pixel art với xưởng đúc đồng; C giấy cắt nhiều lớp với sinh hoạt chợ hoặc đêm hội ven sông. Mỗi hướng là một ảnh tạo riêng, không phải bộ lọc trên tranh cũ hoặc camera của một model chung.

## Xem sản phẩm

Mở `index.html` trong trình duyệt. Nếu dùng server hiện tại, mở `http://127.0.0.1:8016/experiment-07/`; server phục vụ thư mục `prototypes/sprint13`. Có thể tự chạy từ root repository:

```bash
python3 -m http.server 8016 --bind 127.0.0.1 --directory prototypes/sprint13
```

Ba ảnh dự kiến nằm tại `assets/a-woodcut.png`, `assets/b-pixel.png`, `assets/c-papercut.png`. Prompt và provenance do root lưu sau khi tạo ảnh. Gallery chỉ phục vụ đổi ảnh, xem lớn và lớp giao diện minh họa; không thực hiện hành động trong thế giới game.

[Art direction](ART-DIRECTION.md) ghi câu hỏi và cách đối chiếu. Vì cả bối cảnh lẫn phong cách đều thay, phản hồi thích một ảnh chưa tự chứng minh người xem thích riêng kỹ thuật đồ họa đó. Không suy chi phí sản xuất cuối từ một ảnh concept.

## Giới hạn

Chỉ dùng ảnh raster tạo bằng built-in image_gen; không gọi Hyper3D, dựng Blender hoặc tạo model thật. Các cảnh là đề xuất mỹ thuật hư cấu lấy cảm hứng Việt cổ, chưa bổ sung canon/triển khai nghề nghiệp hoặc nghi lễ. Không có game runtime, human playtest, Unity hoặc khảo cổ acceptance trong phạm vi này. Kết quả generation và kiểm gallery chỉ được ghi sau khi thực sự thực hiện; brief không phải báo cáo Passed.

Các thử nghiệm01–06 giữ nguyên. [Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) vẫn quyết định việc nghiệm thu Sprint13; không tự chuyển sang Sprint14.

## Artwork và kiểm tra

Ba ảnh gốc đã lưu trong assets; [prompts.json](prompts.json) giữ prompt đầy đủ và [provenance.json](provenance.json) ghi SHA256/kích thước. Ảnh dùng built-inimage_gen, sao chép nguyên bytes, không model3D/hậu kỳ.

`python3 prototypes/sprint13/experiment-07/browser-check.py` kiểm gallery bằng Python Playwright/Pillow và Chrome tại `/usr/bin/google-chrome`. Critic code đã chạy đạt desktop/mobile, đổi ảnh/HUD theo cảnh, modal/focus/phím, hiện/ẩnUI và chữ150%. Evidence `tests/results/sprint13-experiment07/` bị ignore; chỉ kiểm trang xem ảnh, không kiểm gameplay hoặc chốt đồ họa.
