# Sprint13 — Thử nghiệm hình ảnh và trải nghiệm khởi đầu

Sprint đang mở; mọi sản phẩm **chưa nghiệm thu bởi chủ dự án**. Kiểm tra kỹ thuật và phản biện của AI không thay quyết định của người dùng.

## Mục tiêu

Tạo sản phẩm nhìn và tương tác được để chọn hướng hình ảnh, bố cục và cảm giác nhận một cuộc đời ngẫu nhiên. Thử nghiệm đầu tiên: “Bến Vạc — buổi sáng trước mùa nước”, SPIN-016, tại [thư mục sản phẩm](../../../prototypes/sprint13/experiment-01/).

Hypothesis: cảnh sông nước có đời sống lao động và giao diện giàu thông tin vừa đủ giúp người chơi hiểu mình là ai, đang biết gì và có thể chọn làm gì. Chưa có human playtest hoặc nhận xét chủ dự án, không kết luận giả thuyết đã đúng.

## Phân công sáu vai trò

| Vai trò | Actor | Sản phẩm / trách nhiệm |
|---|---|---|
| Worker1 | /root/s13_art | Mỹ thuật SVG tự viết và art direction |
| Worker2 | /root/s13_ui | Giao diện browser chạy offline, điều hướng và tương tác |
| Worker3 | /root/s13_campaign | Mô hình origin→history→derived capability giới hạn cho UX, tests |
| Critic1 | /root/s13_critic_code | Bắt lỗi code, determinism, state/knowledge, tests |
| Critic2 | /root/s13_critic_ux | Bắt lỗi mỹ thuật/UX/canon/accessibility và diễn giải quá mức |
| Điều phối | /root | Tích hợp, kiểm tra lớp đầu, báo cáo cho user và quản lý mục chưa chốt |

Ba workers chạy song song. Critics chạy độc lập sau khi có sản phẩm; không tuyên bố sáu tác nhân cùng chạy khi môi trường chỉ cho phép bốn kể cả root.

## Quy tắc thử nghiệm và chốt

Mỗi phương án mới là spin mới, có hypothesis, artifact, findings, thay đổi và quyết định. Không sửa/xóa nhật ký thử nghiệm cũ. Sửa lỗi cùng phương án có thể tiếp tục spin hiện tại. [Danh sách cần chốt](sprint-13/APPROVALS.md) giữ trạng thái Pending/Changes requested/Approved cùng nguyên văn quyết định hoặc tham chiếu tin nhắn thật; im lặng hoặc “tiếp tục” không phải duyệt.

Không merge dev, đóng sprint hoặc chuyển Sprint14 trước khi chủ dự án chốt. Nếu user yêu cầu sprint mới trong khi còn pending, nhắc lại toàn bộ mục còn thiếu trước khi chuyển. Commit/push nhánh thử nghiệm và draftPR được dùng để review, không đồng nghĩa nghiệm thu.

## Phạm vi thử nghiệm01

Standalone HTML/CSS/JS/SVG, không production import, không Unity game hoặc implementation generator cuối. Luồng New→Reveal→World→Preview/Journal kiểm UX với dữ liệu prototype. Một nhân vật/campaign; không chỉnh origin và không reroll tại Reveal. Hệ origin nhỏ phải giải thích được capability từ history; không giả rằng chọn một NPC trong roster đã là full life generator.

Không giả lập lưu bền, sửa thuyền có conservation, NPC autonomy hoặc runtime knowledge adapter. Preview/result minh họa phải có nhãn UX. Không dùng LLM runtime. Điều kiện đạt lớp đầu: chạy được, tests phù hợp, không lỗi chặn, UI đã quan sát nếu có công cụ browser; thiếu quan sát phải ghi thật. Điều kiện final: **user chốt**.

## Thử nghiệm02 — SPIN017

Theo phản hồi đồ họa bản01 chưa ưng ý, thử hướng painterly raster và giao diện ưu tiên cảnh. Giữ bản01 để đối chiếu; [báo cáo02](sprint-13/EXPERIMENT-02-REPORT.md). Root dùng imagegen tạo tranh, ba workers art direction/UI/browser-check và hai critics code/UX; Pending user approval.

## Thử nghiệm03 — SPIN019

Theo phản hồi màu bản02 có cảm giác AI, tạo ba hướng khác camera, chất liệu và cách tổ chức màu; gallery trung tính cho so sánh cùng bản02. [Báo cáo03](sprint-13/EXPERIMENT-03-REPORT.md). Phần này chỉ thử art direction, không mở thêm gameplay hoặc tựduyệt sản phẩm.

## Thử nghiệm04 — SPIN020

Thử góc nhìn từ mô hình3D thật bằng Blender và browser viewer, cùng geometry cho ba camera. [Báo cáo04](sprint-13/EXPERIMENT-04-REPORT.md). Dùng ảnh B03 làm tham chiếu, không coi đó là user approval.
