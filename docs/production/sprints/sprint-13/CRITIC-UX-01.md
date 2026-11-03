# Critic UX độc lập — Experiment 01

Ngày 27/10/2026; actor `/root/s13_critic_ux`. Phạm vi: cả ba sản phẩm art, campaign và UI tại `prototypes/sprint13/experiment-01`. Dùng hướng dẫn `mythical-validation` để phân biệt browser review với gameplay acceptance. **Pending user approval**; đây không phải human playtest hoặc nghiệm thu Sprint13.

## Findings

| ID | Mức | Kết quả | Tái hiện / ảnh hưởng |
|---|---|---|---|
| UX-01 | P2 | Đã sửa và kiểm lại | `--muted: #6e786a` trên nền giấy `#f4efdf` có tương phản 4.006:1; trên action-card `#e6e7d7` 3.681:1. Dưới 4.5:1 cho chữ thường nhỏ, ảnh hưởng nguồn ký ức và chú thích giới hạn prototype. Đề nghị làm tối màu muted và kiểm lại nền trong suốt. |
| UX-02 | P1, đối chiếu critic code | Đã kiểm bản sửa | Reveal mới có “Điều cuộc sống đã dạy bạn”, kỹ năng và các trải nghiệm đóng góp; mẫu Nâu cho thấy hai nguồn lịch sử tuổi10/19 cho việc sông nước. Không còn chỉ biography thiếu quan hệ kỹ năng. |

## Kiểm trực tiếp

Đã chạy Python Playwright với `/usr/bin/google-chrome`, mở file thật, viewport1280×800. Đi qua Menu → Generating → Reveal → World → Journal → Action; Enter kích hoạt nút, J mở nhật ký, Escape đóng và trả focus, mở hành động từ bàn phím rồi Escape trả về `preview-action`. Không phát hiện pageerror. Ở cỡ chữ150%, desktop document.scrollWidth=1280 bằng viewport; ảnh World có nội dung tiếp dưới fold nhưng không mất nút. Dialog cuộn được. Mobile390×844, text150% là kiểm bổ sung, document.scrollWidth=390; không coi là acceptance mobile sản xuất.

Ảnh và JSON độc lập: `tests/results/sprint13-experiment01/critic-ux/` gồm menu, reveal, world150, action150, journal150, mobile150 và review.json. Đã nhìn ảnh menu/reveal/world150/journal150 bằng công cụ xem ảnh, không chỉ dựa DOM assertions. Ảnh generated bị ignore, không là asset phát hành.

## Đánh giá ba sản phẩm

- Art giữ chủ đề sinh hoạt ven sông: nhà sàn, thuyền, bến sửa, nước và công việc; không thấy cung điện, giáp phương Tây hoặc biểu tượng fantasy áp đảo. Mảng phẳng ấm phù hợp thử nghiệm yên tĩnh nhưng chưa chứng minh bản sắc lịch sử độc đáo; ART-DIRECTION phân biệt rõ canon và chi tiết tự sáng tạo. SVG không phải bản đồ navigation hay bằng chứng khảo cổ. Cảnh crop ở World/Reveal giảm số nhà thấy được; vẫn giữ sông/thuyền, không có hotspot giả tọa độ entity.
- Campaign phân biệt nguồn kỹ năng theo đời sống, giới hạn ba hoàn cảnh/sáu nhánh được công khai; không có reroll ngay Reveal. Chữ “chưa tạo bản lưu” và “phiên xem thử” không giả save/load. Journal ghi tuổi trải nghiệm và “ký ức cá nhân”, nội dung tuyến đò nói chưa biết hiện tại. Không thấy UI lộ hidden NPC state. Chưa đo được người dùng có hiểu hay thích nhân vật.
- UI có phân cấp cảnh/nhân vật/quan sát và thông báo hành động chỉ mẫu. Luồng keyboard đã kiểm; không claim screen-reader audit hoặc WCAG toàn diện. Chữ nhỏ ở100% cần chủ dự án xem thực tế, dù có tăng150%; lỗi tương phản UX-01 đã được sửa và kiểm lại như dưới đây.

## Kết luận hiện tại

**Technical reviewed trong phạm vi thử nghiệm browser.** UX-01 đã sửa thành `--muted: #596451`; critic độc lập đọc computed style browser và tính lại: giấy5.424:1, action-card4.983:1, panel5.623:1, nền hotspot composited trên đen4.690:1. Ảnh mới `reveal150-fixed.png` và `journal150-fixed.png` đã được nhìn trực tiếp; `contrast-fixed.json` ghi số đo và phản hồi xác nhận hành động mẫu thực tế. Không còn finding chặn kỹ thuật đã biết trong phạm vi này. Kể cả technical review đạt, hình ảnh, UX và mô hình cuộc đời đều chưa được chủ dự án chốt; không đóng Sprint13 hoặc chuyển Sprint14.
