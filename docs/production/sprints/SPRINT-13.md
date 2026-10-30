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

## Thử nghiệm05 — SPIN021

Tạo model bằng MCPHyper3D thật, kiểm vật liệu/geometry và viewer. Đầu raA lệch brief thành nhà thuyền; correction gặp lỗi submission chưa xác địnhID. [Báo cáo05](sprint-13/EXPERIMENT-05-REPORT.md) phân biệt technical pass, brief mismatch và provider state; không chốt mỹ thuật vì modelcompleted.


## Thử nghiệm 06 — SPIN-022: xem trước bằng ảnh

Theo chỉ dẫn mới 29/10, tiếp tục thử ý tưởng bằng ảnh 2D hoặc ảnh có diện mạo 3D, không tạo model 3D thật. Ba hướng cùng tình huống sửa thuyền ở Bến Vạc: tranh 2D góc chéo, giả lập 3D gần tầm người và 2.5D nhìn ngang. Ảnh được tạo riêng, không chứng minh cùng geometry hoặc gameplay. [Báo cáo 06](sprint-13/EXPERIMENT-06-REPORT.md) và approval register giữ trạng thái chưa chốt. Không gọi Hyper3D/Blender cho vòng xem trước này; quy tắc Hyper3D chỉ dùng khi được giao task model thật trong tương lai.


## Thử nghiệm 07 — SPIN-023: đổi ngôn ngữ đồ họa và bối cảnh

Theo yêu cầu thử ý tưởng khác, bao gồm cả đồ họa, vòng07 thử khắc gỗ màu phẳng, pixel art và giấy cắt nhiều lớp. Ba concept lần lượt đặt ở làng mùa nước/chuyển thóc, xưởng đúc đồng và chợ hoặc đêm hội ven sông; chưa chốt canon hoặc đặc tả gameplay. Đây không phải chỉ đổi camera/palette của cảnh sửa thuyền. Vì đổi cả bối cảnh, chưa thể tách tác động của phong cách khỏi chủ đề khi so sánh. [Báo cáo07](sprint-13/EXPERIMENT-07-REPORT.md) giữ evidence thật và giới hạn; mọi phương án Pending, không model3D thật, không tự mở Sprint14.

## Thử nghiệm08 — SPIN-024: thêm phiên bản, cùng cảnh để xem đồ họa

Theo yêu cầu nghĩ thêm và làm rõ ý nghĩa các version, vòng08 bổ sung ba hướng ảnh trên cùng tình huống hư cấu tại nhà chữa trị bên suối đồi: nét hoạt hình2D/mảng bóng gọn, thế giới như dệt bằng sợi/vải và diện mạo3D cách điệu hình khối rõ. Chỉ tạo raster xem trước; không dựng model3D, triển khai chữa trị hoặc chốt canon. Cùng một ý tưởng cảnh giúp xem đồ họa thuận tiện hơn, nhưng ảnh tạo riêng không có cùng geometry hoặc mọi biến số được kiểm soát. [Báo cáo08](sprint-13/EXPERIMENT-08-REPORT.md).

Các vòng01–08 là thư viện tham khảo cùng được giữ. Thử nghiệm mới không mặc định thay thế, loại hoặc đánh giá kém bản cũ; phản hồi/finding gắn đúng phiên bản và phạm vi. Giữ nhận xét lịch sử thật; lời khen tiến triển không tự là approval. Mọi phương án chưa được chốt vẫn Pending user approval, không đóng Sprint13 hoặc mở Sprint14.

## Thử nghiệm09 — SPIN-025: đồ họa xuyên nhân vật và UI

Theo yêu cầu táo bạo hơn và đồng bộ diện mạo toàn game, vòng09 thử ba bảng concept sơn mài sống, phù điêu đồng và mực đen/trắng+nét đỏ. Mỗi hướng cần thể hiện cùng ngôn ngữ qua thế giới, nhân vật toàn thân/chân dung, vật phẩm/icon và UI hội thoại/túi đồ/nhật ký, thay vì chỉ có phong cảnh. Gallery trung tính phục vụ xem bảng; UI raster là minh họa, không là chức năng gameplay hoặc character creator chỉnh origin. [Báo cáo09](sprint-13/EXPERIMENT-09-REPORT.md) ghi phạm vi và evidence thật. Chỉ tạo ảnh, giữ01–08 làm tham khảo, mọi hướng Pending; không tự mởSprint14.

## Thử nghiệm10 — SPIN-026: số lượng theo ý tưởng

Theo yêu cầu tiếp tục sáng tạo và tự quyết số lượng, vòng10 chọn hai bảng concept có tên riêng thay vì mặc định A/B/C: Men chàm và Bóng rối. Hai ngôn ngữ cùng được thử xuyên thế giới, nhân vật/chân dung, vật phẩm/icon và UI; hình ảnh là đề xuất sáng tạo hư cấu, chưa chốt canon, UI chạy hoặc quy trình sản xuất. [Báo cáo10](sprint-13/EXPERIMENT-10-REPORT.md).

Số hướng phụ thuộc giá trị ý tưởng và phạm vi được giao, không là yêu cầu tạo vô hạn. Thư viện01–10 được giữ; vòng mới và lời khuyến khích không tự là approval hay loại bản trước. Hồ sơ09 đã được hai critic mới kiểm lại và viết report riêng sau gián đoạn; vòng10 được review cùng lượt, các phiên bản cùng được giữ. Không model3D/Hyper3D/Blender, không chuyểnSprint14 khi còn pending.

## Thử nghiệm11 — SPIN-027: 2.5D,3D,pixel và anime

Theo yêu cầu thử thêm cách thể hiện, vòng11 thử bốn tổ hợp toàn game: cảnh2.5D nhiều lớp, diện mạo3D cách điệu, pixel art và anime vẽ tay. Không coi bốn nhãn là lựa chọn loại trừ nhau: trục tổ chức không gian2.5D/3D có thể kết hợp với trục mỹ thuật pixel/anime. [Báo cáo11](sprint-13/EXPERIMENT-11-REPORT.md) theo dõi output và review thực tế.

Tình huống chung là chợ lương thực/kho cao/cầu trước mùa nước, với phụ nữ lao động trưởng thành làm cast minh họa để xem tuổi, nghề và biểu cảm xuyên cảnh/chân dung/UI. Không đổi baseline sinh nhân vật ngẫu nhiên. Vòng mở29/10, tiếp tục30/10 sau khi các worker trước gặp giới hạn; artifact và actor thực phải được ghi theo kết quả, không nhận phân công làm hoàn thành. Chỉ tạo ảnh, không chuyển engine/model/parallax/animation hoặc nghiệmthu pixel assets. Thư viện01–11 được giữ, mọi hướng Pending; không tự mởSprint14.

## Thử nghiệm12 — SPIN-028: sáu mẫu đồ họa xuyên suốt game

Ngày30/10 chủ dự án yêu cầu ít nhất năm mẫu mới và thử nghiệm mạnh hơn. Vòng12 chọn sáu ngôn ngữ: Đất biết kể/đất nặn thủ công, Nét trong/truyện tranh nét rõ, Vách thời gian/bích họa khoáng màu, Ký ức CD-ROM/diện mạo RPG dựng sẵn cuối thập niên1990, Đêm than bạc/than chì và nét bạc ban đêm, Gió mảng màu/hoạt hình mảng phẳng không viền. Đây là các hướng mỹ thuật toàn game, cần thể hiện cảnh, cast người trưởng thành/toàn thân/chân dung, vật phẩm và UI trong cùng ngôn ngữ. [Báo cáo12](sprint-13/EXPERIMENT-12-REPORT.md) giữ output, phân công và evidence thật sau khi thực hiện.

Ba worker phụ trách định hướng, gallery và hồ sơ; root sinh ảnh/tổng hợp/kiểm tra lớp đầu, hai critic kiểm kết quả sau đó trong giới hạn bốn tác nhân cùng hoạt động. Phân công không tự là hoàn thành hoặc independent review. Chỉ tạo ảnh concept bằng built-in image_gen, không model thật/Hyper3D/Blender hoặc đổi engine. Chữ và UI trong ảnh chưa là UI runtime; ví dụ cast không thay baseline xuất thân ngẫu nhiên. Thư viện01–12 cùng được giữ, không ngầm loại bản cũ; mọi sản phẩm vẫn Pending user approval và không tự chuyểnSprint14.

## Workshop SPIN-029 — đặt lại định nghĩa đồ họa

Ngày30/10 user yêu cầu các tác nhân brainstorming về cách thể hiện game, vibe Văn Lang–Âu Lạc và vấn đề ảnh đang thiên tả thực dù gắn nhãn3D. [Bản tổng hợp](sprint-13/ART-DIRECTION-RETHINK.md) cùng ba bài nghiên cứu và hai review ghi actor/evidence thật. Scope chỉ audit/nghiên cứu/đề xuất/brief: không tự tạo vòng ảnh13, model hoặc chuyển engine. Đổi cách đặt brief, tránh công thức65/35 và medium swap; giữ đủ bản sắc xuyên cảnh/cast/UI qua artifact phù hợp. Mọi phương án và thứ tự nghiên cứu root đề xuất vẫn Pending user approval; giữ01–12, không chuyểnSprint14.

## Thử nghiệm13 — SPIN-030: màn hình chơi và đối thoại riêng

User30/10 giao thử ý tưởng mới sau workshop SPIN-029. Vòng13 có ba hướng: Khối Lạc/3D khối nhìn chéo, Dải đời sống/2D–2.5D theo dải sinh hoạt, Trong lòng làng/3D cách điệu gần tầm người. Mỗi hướng được giao hai ảnh nguyên khung, thế giới và đối thoại, để kiểm camera/hình/tỷ lệ/độ đọc cùng sự thống nhất cast/UI; không dùng dải asset65/35 cố định. [Báo cáo13](sprint-13/EXPERIMENT-13-REPORT.md) ghi output/review thật sau khi có.

Ba worker làm định hướng/gallery/hồ sơ, root sinh ảnh/tổng hợp/kiểm lớp đầu và điều phối hai critic sau đó; assignment không tự là hoàn thành. Chỉ ảnh xem trước, không model/scene3D/runtime/animation hoặc thay engine/canon. Cast chung là mẫu cuộc đời ngẫu nhiên, không màn hình chọn origin. Ảnh độc lập không chứng minh geometry/camera/cast trùng tuyệt đối. Giữ01–12 và workshop; vòng13 cùng mọi sản phẩm chưa chốt tiếp tục Pending, không tự chuyểnSprint14.

## Thử nghiệm14 — SPIN-031: sáu hướng độc lập để mở rộng khác biệt

User30/10 ghi nhận ảnh13 tốt nhưng còn giống nhau, yêu cầu thử tiếp mạnh hơn. Vòng14 giao sáu màn hình chơi riêng: Đất dựng thành/3D khối thành đất; Ô đất sống/2D mặt bằng từ trên; Làng vòng cung/3D tròn mềm và camera cong; Đôi tay làm nghề/góc nhìn thứ nhất đồ họa; Ký hình/thế giới2D bằng dấu hình; Mái nhà nhiều giọng/trang truyện tương tác minh họa. Các hướng đổi camera/hình/tỷ lệ, bối cảnh, cast, màu và UI, không giữ khuôn13 hoặc dùng13 làm ảnh tham chiếu. [Báo cáo14](sprint-13/EXPERIMENT-14-REPORT.md) giữ scope/output/evidence thực khi có.

Ba worker chuẩn bị định hướng/gallery/hồ sơ, root tạo ảnh và kiểm lớp đầu, hai critic kiểm sau workers trong giới hạn bốn tác nhân cùng hoạt động. Phân công không tự là hoàn thành. Chỉ tạo ảnh xem trước; không tạo model, đổi engine/geometry/canon/gameplay. Camera cong và ký hiệu là sáng tạo presentation, không khẳng định hành tinh nhỏ hoặc chữ cổ; trang truyện không là asset catalogue. Giữ01–13/workshop, phản hồi tích cực không là final approval; không tự chuyểnSprint14.
