# Thử nghiệm10 — Hai ngôn ngữ hình ảnh: Men chàm và Bóng rối

Sprint13/SPIN-026, ngày29/10/2026. **Hai bảng concept đã tạo và có review độc lập; mọi phương án Pending user approval.** Bảng concept ảnh-only để xem trước đồ họa toàn game, không tạo model3D hoặc triển khai gameplay/UI runtime.

## Mục tiêu dễ hiểu

Chủ dự án muốn tiếp tục thêm ý tưởng và cho AI tự quyết số lượng concept, không bắt buộc ba phương án A/B/C. Vòng10 chọn hai hướng đủ khác về chất liệu, hình khối và ánh sáng để đào sâu: Men chàm và Bóng rối. Mỗi hướng cần thể hiện đồng bộ thế giới, nhân vật toàn thân/chân dung, vật phẩm/icon và UI hội thoại, túi đồ, nhật ký.

| Hướng đã thử | Ngôn ngữ hình ảnh | Điều cần quan sát |
|---|---|---|
| Men chàm | Gốm men xanh/trắng, nét cobalt và mặt men được tiết chế | Nhân vật có cá tính thay vì thành đồ chơi/trang trí; chữ và icon có rõ trên chất liệu gốm? |
| Bóng rối | Silhouette cắt/bóng, lớp sáng xuyên và sắc hổ phách | Người có biểu cảm riêng, lớp cảnh và UI có đọc rõ mà không hòa thành một màn bóng tối? |

Đây là ẩn dụ mỹ thuật hư cấu, không là khẳng định lịch sử về gốm, rối hoặc canon mới. Không suy chất liệu mới thành cơ chế gameplay. UI raster là minh họa, không có nút game hoạt động hoặc text component responsive; nhân vật mẫu không phải màn hình chọn/chỉnh origin thủ công.

## Phạm vi và nguồn thực tế

Root đã dùng built-inimage_gen hai lần để tạo hai bảng raster độc lập, lưu tại `prototypes/sprint13/experiment-10/assets/men-cham.png` và `bong-roi.png`. Cả hai1672×941RGB, sao chép nguyên bytes, không hậu kỳ và không truyền ảnh tham chiếu. [Prompts](../../../../prototypes/sprint13/experiment-10/prompts.json) lưu nguyên văn; [provenance](../../../../prototypes/sprint13/experiment-10/provenance.json) ghi output gốc, SHA256, kích thước/bytes. [Gallery10](../../../../prototypes/sprint13/experiment-10/index.html) chỉ phục vụ xem/đổi ảnh và phóng lớn. Không phủ HUD HTML chung lên hai UI concept; không gọi Hyper3D/Blender hoặc dựng geometry.

Root đã thực sự xem hai output. Men chàm nối lam–trắng, nét cobalt, ô gốm và icon giữa cast/portrait/UI; cảnh nhà tre và da mặt còn thiên hiện thực, chưa biến mọi vật thể thành gốm như một ý định cực đoan của brief. Bóng rối dùng silhouette, chi tiết đục lỗ và nền sáng qua nhân vật/cảnh/icon; portrait có thêm sắc độ và cảm giác hoàng hôn, không hoàn toàn là hình cắt xuyên sáng phẳng. Đây là quan sát để hiểu từng version, không loại bỏ hoặc tự kết luận hướng nào tốt hơn.

Cả hai board có cảnh chơi, nhân vật toàn thân, chân dung, sáu icon vật phẩm, hội thoại/lựa chọn, túi đồ và nhật ký. UI dùng ngôn ngữ riêng trong PNG, không lấy HTML gallery trung tính làm bằng chứng UI của game. Tên NPC như Ông Lành, Cô Hòa, Bà Lành và lời thoại trong ảnh chỉ là nội dung minh họa; chưa được đưa vào canon, content definitions hoặc rule. Không có character creator chọn/chỉnh origin. Bối cảnh/cast thay đổi giữa hai ảnh nên chưa cô lập được sở thích do riêng đồ họa.

## Phân công

| Actor | Phạm vi được giao |
|---|---|
| /root/s13e10_direction | Brief/ma trận nhất quán và hướng dẫn xem hai concept |
| /root/s13e10_gallery | Gallery ảnh-only, điều khiển xem và thư viện lịch sử |
| /root/s13e10_docs | Lưu nguyên tắc số lượng linh hoạt và đồng bộ hồ sơ |
| /root | Tạo ảnh/prompt/provenance, tích hợp, kiểm lớp đầu và tổng hợp |

Ba workers10 đã bàn giao. `/root/s13e10_critic_code` thực sự kiểm cả09/10, `/root/s13e10_critic_visual` xem năm PNG và một screenshot mỗi vòng, viết bốn report riêng. Đây là actor mới hoàn tất review09 sau phiên bị ngắt, không nhận báo cáo hoặc automation của actor trước. Critics ở lượt sau ba workers, không claim sáu actor cùng chạy. Không có human playtest, Unity/gameplay acceptance hoặc audit typography/accessibility đầy đủ trong phạm vi này.

## Evidence và đánh đổi

`python3 prototypes/sprint13/experiment-10/browser-check.py` đã được code critic chạy exit0 trên Chrome156.0.8078.47 ở1280×720/1920×1080/390×844. Hai ảnh decode đúng và bytes/hash/kích thước khớp PNG gốc; main/thumbnail contain, hai tên concept/nút/phím1/2/M/R, modal/Escape/focus/linkPNGgốc và chín historylinks01–09 đạt. Không pageerror hoặc requestngoài ởfilelocal; chữHTML150% không tràn ngang. Evidence ignored: `tests/results/sprint13-experiment10/browser.json` và screenshots. Root đã xem screenshot Bóng rối, không nhận automation của critic là lượt tự chạy.

[Code review10](CRITIC-CODE-10.md) xác nhận kỹ thuật gallery, không OCR/a11y/UI runtime. [Visual review10](CRITIC-UX-10.md) xác nhận có đủ nhóm mẫu xuyên game, cùng các đánh đổi: Men chàm chưa gốm hóa toàn bộ nhà/da mặt, trang phục còn chất phiêu lưu; Bóng rối có lớp bóng và cử chỉ riêng nhưng portrait bán tả thực/ánh chiều khác ý định màn xuyên sáng thuần. Không thấy P1 cản xem ý tưởng; không xếp hạng loại bỏ. ChữUI raster không reflow ở150%, cần zoom PNG cho chữ nhỏ; animation và đa dạng cast còn chưa có evidence.

## Thư viện và bàn giao

Vòng10 bổ sung thư viện01–10; các ảnh, nguồn và hồ sơ cũ được giữ để xem lại hoặc phối hợp. Số lượng theo ý tưởng không phải yêu cầu tạo vô hạn, tự mở sprint hoặc chốt thay user. [Danh sách cần chốt](APPROVALS.md) giữ S13-A01–A04 Pending/Changes requested. Không merge dev, đóngSprint13 hoặc chuyểnSprint14.

Policy được bổ sung vào AGENTS.md, .agent và art-direction reference; HANDOFF, sprint brief, approval register, changelog và export source cùng trỏ vòng10. Root regenerate/check export từ nguồn sau khi ảnh/review/evidence hoàn thiện. Không sửa archive, generated GDD, canon/rule, save/schema/content versions hoặc product VERSION.
