# Đất dựng thành 2D — định hướng đã chốt và cổng mở lại G13

Ngày 03/11/2026 · SPIN-037 · Profile **`earth-monument-2d-v1`** · **Approved direction / Accepted, Reopenable**.

Chủ dự án chốt Sprint13 theo hướng **nền 2D có cảm giác khối + nhân vật sprite**, lấy ngôn ngữ hình ảnh của Đất dựng thành vòng14 làm tham chiếu. Sprint13 kết thúc ở phạm vi lựa chọn định hướng; cổng G13 tiếp tục tồn tại để xem xét lại phần mỹ thuật nếu kết quả phát triển không đạt mong muốn. Mở lại G13 không đưa toàn dự án về Sprint1.

## Nguồn quyết định và giới hạn approval

Yêu cầu thật ngày03/11/2026:

> chốt sprint 13 theo hướng này nhé, nhưng hãy để cổng 13 để nếu mình phát triển thấy không ưng ý có thể quay về sprint 13 để làm lại theo hướng khác, tránh mất hết những gì từ sprint 13 quay trở về sprint 1

“Hướng này” nối trực tiếp với trao đổi về **Nền 2D có cảm giác khối + nhân vật sprite cho mẫu Đất dựng thành**. Nguồn hình là [Đất dựng thành — Experiment14](../../../../prototypes/sprint13/experiment-14/assets/earth-monument.png); quyết định kiến trúc tại [ADR022](../../../adr/022-earth-monument-2d-and-reopen-gate.md), các mục chốt tại [APPROVALS](APPROVALS.md).

| Đã được chốt | Còn phải đặc tả, triển khai hoặc nghiệm thu |
|---|---|
| Hướng hình ảnh và cách sản xuất 2D có khối + sprite; giữ khả năng quay lại nghiên cứu mỹ thuật | Sprite thực, animation, phân lớp/collision/path và hiệu năng trên máy target |
| Kết thúc vòng lựa chọn Sprint13 với profilev1 và giữ thư viện01–18 | Layout Reveal/World/Journal/Preview cụ thể, chữ/icon/âm thanh/quyền sử dụng asset và accessibility |
| Cổng G13 có thể mở lại riêng phần trình bày | Random-origin generator, repair, actor projection, save/load và playable client chưa được nghiệm thu bởi quyết định mỹ thuật |

Không hiểu việc chốt thành duyệt tất cả ảnh cũ hoặc mọi chi tiết UI trong PNG. Các phương án còn lại được giữ để tham khảo/kết hợp; không tự gọi là bị loại hoặc kém. Các kết quả review và Pending trong báo cáo cũ là trạng thái lịch sử tại thời điểm của chúng, không ghi lại thành Passed hiện tại.

## Profile `earth-monument-2d-v1`

| Thành phần | Quy ước định hướng | Việc phải kiểm trong task sản xuất |
|---|---|---|
| Không gian | Tranh nền có phối cảnh và cảm giác khối; mảng đất lớn, nhịp sáng tối và tỷ lệ gần–xa tạo quy mô | Giữ độ đọc người/đường đi; không chuyển thành ảnh tả thực mặc định. Mã màu, độ chi tiết và độ phân giải là tham số cần thử |
| Lớp nền | Tách hậu cảnh, mặt đi, công trình và các lớp tiền cảnh; chỉ dùng parallax vừa phải khi phù hợp shot | Vẽ đủ phần hiện ra khi người/vật di chuyển. Cắt PNG hiện tại không cung cấp sẵn mặt sàn hoặc phần bị che |
| Che khuất/cao độ | Mặt cầu, mặt đất và từng tầng đi có định danh trình bày, đường nối và lớp che riêng | Chọn đúng tầng, người bị cột/lan can che đúng; sắp theo Y đơn thuần không giải được mọi giao cắt cầu–đất |
| Camera | Góc nhìn cố định theo cảnh; pan/zoom có giới hạn được khai báo và kiểm trước | Không hứa quay tự do quanh cảnh. Chuyển shot phải có điểm nối, framing và input contract rõ |
| Nhân vật | Sprite độc lập với nền, silhouette góc cạnh và tỷ lệ phù hợp ngôn ngữ khối của mẫu | Hướng đi/pose/đạo cụ giữ identity; lựa chọn frame animation hay rig2D, số hướng và frame chưa được chốt ở đây |
| Nhân vật xa | Biểu diễn giản lược và hoạt ảnh dùng lại khi hợp khoảng cách | Phân biệt actor của mô phỏng với chi tiết trang trí; một loop làm việc không chứng minh NPC autonomy |
| Đạo cụ/trạng thái cảnh | Các mảnh ảnh hoặc biến thể trạng thái theo dữ liệu trình bày | Vật đang cầm, đặt xuống, thuyền hỏng/đã sửa và phần công trình đổi phải có nguồn/trạng thái phù hợp, không tự sinh luật |
| Chân dung/icon/UI | Cùng ngôn ngữ hình, nhận ra cùng nhân vật; UI dùng text/font thật có dấu Việt | Không dùng chữ raster từ PNG làm nhãn runtime. Layout, focus, text scale, license và accessibility có acceptance riêng |

Đất dựng thành cung cấp **ngôn ngữ mỹ thuật**, không quyết định rằng Bến Vạc phải là đại công trường thành đất. Khi áp vào demo sửa thuyền vẫn giữ Bến Vạc/Tam Giang, địa lý, nghề nghiệp và nhân quả đã có. Công cụ, trang phục và kiến trúc PNG chưa tự thành phục dựng khảo cổ hoặc game-canon.

Đường sản xuất của profile này là asset2D phân lớp và sprite. Không cần tạo model3D chỉ để thực hiện hướng đã chốt; nếu một task3D thật được giao trong tương lai, quy định MCP Hyper3D vẫn có hiệu lực. Unity tiếp tục là lớp trình bày; quyết định này không đổi engine hoặc yêu cầu runtime LLM.

## Ranh giới để không mất nền tảng đã làm

| Phần được giữ | Ranh giới khi đổi mỹ thuật |
|---|---|
| Foundation/Domain/Application, rules, determinism và WorldTick | Không đưa pixel, animation phase, frame delta hoặc camera vào luật authoritative |
| Entity/definition IDs, content và quan hệ nhân quả | Định danh asset/look/surface thuộc presentation; đổi tranh không đổi identity nhân vật hoặc sự kiện |
| Actor-scoped projection và command/receipt | View chỉ đọc dữ liệu được phép và gửi intent. Animation callback không hoàn thành repair, tiêu vật liệu, tiến thời gian hoặc RNG |
| Persistence/schema và lịch sử đã commit | Đổi art không tự đổi save schema. Nếu phát sinh thay đổi contract thật, phải có ADR/impact/migration và tests riêng trước khi tích hợp |
| Nghiên cứu/GDD và thư viện thử nghiệm | Giữ archive, artwork, prompts, provenance, review và lịch sử. Không ghi đè bản v1 bằng candidate mới |

Đây là hợp đồng tách lớp cho các task tiếp theo, **chưa phải cơ chế đổi skin/profile runtime đã được code**. View, asset hoặc adapter đặc thù một hướng có thể phải sửa hoặc làm lại; không hứa mọi asset đều tái sử dụng được. Source/evidence cũ vẫn được bảo toàn để tham chiếu và so sánh.

## G13 — chốt được nhưng mở lại được

Trạng thái hiện tại: **Accepted / Reopenable**, profile đang áp dụng là `earth-monument-2d-v1`. “Reopenable” là khả năng quay lại, không phải approval còn treo hoặc yêu cầu giữ Sprint13 mở mãi.

```text
Accepted / Reopenable (v1)
    → Reopened: có yêu cầu hoặc evidence cần xem lại
    → Candidate: phương án mới có phạm vi, artifact và review
    → Accepted / Reopenable (v2): chủ dự án chốt rõ
```

Nếu candidate chưa được chốt, v1 tiếp tục là baseline; có thể sửa hoặc giữ candidate để tham khảo. Không tự thay v1 vì một kết quả test đẹp, CI xanh hoặc agent thích một phương án khác.

### Quy trình quay lại Sprint13

1. **Ghi lý do mở lại:** user yêu cầu đổi hướng, hoặc AI ghi nhận evidence trong task đang được giao: hình không đạt mong muốn, sprite/contact không ổn, khối lượng sản xuất vượt giả định hay hiệu năng/độ đọc không đạt. AI được ghi gap và điều tra giới hạn trong scope đã giao; không tự mở toàn sprint hoặc tự duyệt style thay thế.
2. **Đóng phạm vi ảnh hưởng:** chỉ rõ art, sprite, camera, occluder, UI hoặc presenter nào cần sửa; liệt kê contract/core/content/save nào thực sự bị ảnh hưởng. Chỉ phần phụ thuộc bị giữ gate, không mặc định dừng mọi việc về luật game/dữ liệu.
3. **Lấy nền hiện tại:** tạo feature/spike từ `dev` hiện hành theo Git workflow, không lấy source engine cũ để ghi đè công việc mới. Không `reset --hard`, revert cả sprint/merge hoặc checkout toàn repository về mốc Sprint13 để đổi hình ảnh.
4. **Tạo candidate có phiên bản riêng:** ghi `supersedes`/lineage từ profilev1, đặt asset/mapping mới riêng và giữ candidate cũ. Thêm SPIN mới trong luồng nghiên cứu Sprint13; sửa cùng candidate tiếp tục spin đó. Không làm lại Sprint1–12 chỉ vì đổi style.
5. **Review và kiểm đúng phạm vi:** xem artifact ở camera/tỷ lệ thật, thử animation/che khuất/UI và chạy regression boundaries phù hợp. Nếu mapping mới chạm schema hoặc simulation behavior, dừng phần tích hợp đó để có contract/migration/acceptance riêng.
6. **Chủ dự án chốt revision mới:** cập nhật ADR/register, profile áp dụng, handoff và worklog; tích hợp qua PR/CI theo workflow. Giữ v1 và evidence cũ. Không tự tạo release tag hoặc sửa lịch sử Git đã chia sẻ.

Checkpoint tham chiếu là [checkpoint-v1.json](checkpoint-v1.json), do root ghi từ input đã kiểm. Mốc Git trước quyết định là `1e4332f0a122528b9bed850377529ffee1efb7da`; đó là mốc thư viện/nguồn trước khi ghi approval, không phải commit chốt mới, build game hoặc release tag. Manifest phải chỉ rõ scope/hash của file được giữ và tránh tự hash chính nó hay vòng lặp WORKLOG/export. Việc có manifest chứng minh inventory khi đã kiểm hash, không chứng minh restore chạy được hay save compatibility đã đạt.

**Giữ mốc truy nguyên reachable:** khi tích hợp PR13, ưu tiên merge commit để giữ source commit trên trong ancestry của `dev` theo ngoại lệ giữ lịch sử của Git workflow; không xóa feature branch trong công việc này. Nếu policy server buộc linear/squash, giữ nhánh nguồn làm anchor và ghi phương án retention thực trước khi merge; không né policy hoặc tuyên bố SHA vẫn reachable chỉ vì có chuỗi hash. Sau tích hợp phải kiểm ancestry/anchor thật. Các tree core trong checkpoint là dấu mốc audit lịch sử, không khóa công việc mới: khi mở lại G13 phải đối chiếu core ở thời điểm hiện hành trước/sau sửa art, không khôi phục tree core cũ đè lên tiến độ mới.

## Bài kiểm chứng 2D tiếp theo — đề xuất, chưa triển khai

Input: profilev1, source hình14, contract UI/projection hiện hành. Scope thử đầu gồm một sân tiền cảnh, một đoạn cầu, hai sprite và động tác cầm/đặt bình; chưa panorama/crowd hoặc đầy đủ gameplay repair.

| Câu hỏi | Evidence cần có khi task được giao |
|---|---|
| Người có đứng trong không gian hay giống hình dán? | Checkpoint đi/đứng/cầm/đặt đồ ở cùng camera; chân, scale và bóng khớp; phần nền sau người được vẽ đủ |
| Nhiều tầng có đọc và chọn được? | Điểm chọn đúng cầu/đất; người qua trước/sau cột và lan can; không click xuyên modal hoặc chọn entity bị ẩn trái projection |
| Sprite có giữ danh tính? | So các hướng/pose/portrait, tỷ lệ người trưởng thành và vật cầm; ghi lỗi và số vòng sửa thật |
| Có phù hợp máy target và UI? | Build/capture đúng cấu hình, text Việt và text scale; đo frame-time/memory khi có implementation, không suy FPS từ PNG |
| Có giữ ranh giới core? | View không ghi world state, không tiêu gameplay RNG; command/query và IDs ổn định. Tests phù hợp phải chạy khi có binding thật |

Cần ước lượng riêng cho đường2D sau khi có scope asset và số đo. Các ROM3D trong [SPIN-036](ART-FEASIBILITY-THREE-DIRECTIONS.md) được giữ như nghiên cứu lịch sử, **không áp nguyên sang profile2D hoặc biến thành deadline đã chốt**. Chưa có mức tiết kiệm phần trăm, ngày hoàn thành hay hiệu năng được xác nhận cho profile này. Nền simulation, content và save vẫn có công triển khai độc lập với lựa chọn mỹ thuật.
