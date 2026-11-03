# ADR022 — Đất dựng thành 2D và cổng mở lại mỹ thuật G13

Ngày 03/11/2026 · Trạng thái: **Accepted cho định hướng bởi chủ dự án; G13 Reopenable** · SPIN-037.

## Bối cảnh và nguồn

Sprint13 đã tạo thư viện01–18, workshop và nghiên cứu khả thi ba hướng. SPIN-036 đề xuất route3D nhưng chưa là quyết định production; user sau đó xem xét chi phí model/rig/tích hợp và thảo luận route nền2D có cảm giác khối + nhân vật sprite cho Đất dựng thành vòng14.

Ngày03/11/2026 user yêu cầu:

> chốt sprint 13 theo hướng này nhé, nhưng hãy để cổng 13 để nếu mình phát triển thấy không ưng ý có thể quay về sprint 13 để làm lại theo hướng khác, tránh mất hết những gì từ sprint 13 quay trở về sprint 1

Quyết định áp dụng được định nghĩa đầy đủ tại [Art Direction Baseline](../production/sprints/sprint-13/ART-DIRECTION-BASELINE.md), profile `earth-monument-2d-v1`. [Approval register](../production/sprints/sprint-13/APPROVALS.md) giữ phạm vi từng mục, gồm các chi tiết UX còn phải thực hiện riêng.

## Quyết định

1. Chốt hướng sản xuất **nền2D phân lớp có phối cảnh/cảm giác khối + nhân vật sprite**, camera cố định theo cảnh với pan/zoom giới hạn. Lấy khối/tỷ lệ/nhịp sáng tối của Đất dựng thành làm tham chiếu, chưa chốt từng RGB, pose, layout hoặc thông số animation.
2. Chốt kết quả Sprint13 ở mức **lựa chọn định hướng**. Không suy thành nghiệm thu asset runtime, Unity client, gameplay, random-origin generator, save/load hoặc hiệu năng. Không tự mở Sprint14 hay cấp scope triển khai toàn demo từ ADR này.
3. Giữ **G13 Accepted / Reopenable**. Khi kết quả phát triển không đạt, mở một vòng nghiên cứu/candidate riêng trong luồng Sprint13; revision mới chỉ áp dụng sau khi chủ dự án chốt. Candidate chưa duyệt không tự thay baselinev1.
4. Giữ Foundation/Domain/Application, rule/content versions, stable IDs, projection/command boundaries và persistence tách khỏi biểu diễn. Art/presenter/camera/UI có thể thay mà không quay repository hoặc engine về mốc cũ. Animation không ghi state, tiêu tài nguyên, tiến WorldTick hoặc dùng gameplay RNG.
5. Bảo toàn artwork01–18, prompts, provenance, review và lịch sử nguồn; dùng checkpoint inventory và Git history để truy nguyên. Mở nhánh từ `dev` hiện hành, không reset/revert toàn sprint, ghi đè engine bằng checkout cũ hoặc di chuyển release tag. Không tạo tag/release chỉ để đặt checkpoint.
6. Đổi hình ảnh không tự đổi schema/save/canon. Nếu một candidate cần thay contract thật, tách ADR/impact, migration và acceptance; chỉ giữ gate cho phần bị ảnh hưởng. Những task engine/data độc lập vẫn giữ giá trị và kết quả đã nghiệm thu.

## Các phương án đã cân nhắc

| Phương án | Lý do xử lý |
|---|---|
| Dựng toàn bộ cảnh/nhân vật3D | Đã nghiên cứu nhưng không là route được chốt ở đây; vẫn giữ làm phương án tham khảo khi G13 mở lại |
| Một ảnh nền nguyên khung + nhân vật đặt lên | Không đủ cho đi sau cột/lan can, nhiều tầng hoặc phần sàn bị che; cần art phân lớp, vùng đi và mapping riêng |
| Nền2D phân lớp + sprite | Route được chọn; chi phí chuyển sang layers/variants/sprite consistency và animation, chưa có benchmark tiết kiệm |
| Quay cả repository về Sprint13 khi đổi style | Không chấp nhận vì sẽ bỏ công core/content/tích hợp về sau; thay profile và phần trình bày trên nền hiện hành |

## Hệ quả và các gate còn lại

- Bến Vạc vẫn là Bến Vạc/Tam Giang; mẫu thành đất không tự di dời địa lý hoặc đổi sửa thuyền thành xây thành. Trang phục/kiến trúc của PNG là diễn giải nghệ thuật cần provenance, không chứng nhận phục dựng khảo cổ.
- Vùng đi, tầng cầu, điểm chọn, occluders, sprite directions/animation, UI focus/text/font/license, import và performance phải được đặc tả và kiểm trong task riêng. Chưa có hot-swap art profile hoặc runtime adapter mới được implementation bởi ADR này.
- Bài proof2D nhỏ đề xuất gồm sân tiền cảnh, một đoạn cầu, hai sprite và động tác đặt bình. Phải vẽ phần nền bị che; không dùng cắt PNG để giả đã có mọi layer. Scope/protocol nằm ở baseline; chưa chạy proof hoặc nhân rộng asset trong lần chốt này.
- ROM3D của SPIN-036 không là dự toán2D. Cần scope và số đo riêng trước khi chốt thời gian; không hứa giảm một tỷ lệ công cụ thể hoặc ngày hoàn thành toàn game.
- G13 có thể được mở lại theo yêu cầu user; AI ghi evidence/gap và điều tra giới hạn trong scope được giao. Style thay thế vẫn cần approval rõ, không tự đổi vì CI xanh hoặc sở thích agent. Chi tiết state/steps và điều kiện tiếp tục v1 nằm ở baseline.

## Tác động và compatibility

Đồng bộ .agent/AGENTS, Applicable Baseline, Decision/Open Decision registers, approval/sprint/handoff/roadmap, client/Unity guidance, ghi chú áp dụng module và exports ở lớp điều phối. Không sửa archive, body nghiên cứu lịch sử hoặc nghiệm thu test cũ. Không đổi product VERSION, IDs, save schema, rule/content contracts hay runtime code bởi quyết định này. Chốt mỹ thuật không nâng trạng thái các gameplay gates.

Checkpoint input trước quyết định là commit `1e4332f0a122528b9bed850377529ffee1efb7da`; manifest tại [checkpoint-v1.json](../production/sprints/sprint-13/checkpoint-v1.json) ghi inventory/hash riêng. Commit chốt thực tế và checks thuộc WORKLOG/Git evidence, không tự nhúng hash của commit đang được tạo hoặc WORKLOG vào chính checkpoint để gây vòng lặp.

Retention khi tích hợp: dùng merge commit cho PR13 nếu policy cho phép để giữ checkpoint source commit reachable trong ancestry `dev`; giữ nhánh nguồn, không xóa branch hoặc tạo release tag trong task này. Nếu server chỉ cho squash/linear, phải giữ anchor nguồn và ghi rõ trước tích hợp, không bypass policy. Kiểm ancestry/anchor sau merge; tree core trong checkpoint chỉ là mốc audit, không là phiên bản phải đưa code tương lai trở về.
