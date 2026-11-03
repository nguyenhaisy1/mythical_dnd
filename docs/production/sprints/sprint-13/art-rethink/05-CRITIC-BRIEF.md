# Phản biện brief — từ workshop tới ảnh có thể kiểm được

30/10/2026 · Sprint13 / SPIN-029 · Reviewer độc lập `/root/art_rethink_critic_brief` · **Review tài liệu/ý tưởng, Pending user approval**.

Workshop đã thay câu hỏi đúng hướng: cách tổ chức một màn hình chơi, hình khối và thông tin được đặt trước lớp vật liệu. Sáu phương án khác nhau về camera, tỷ lệ người/cảnh và mức biểu đạt; không chỉ là sáu palette. Chưa có output mới nên chưa kết luận chúng đã cho cảm giác game hoặc vibe phù hợp.

## Phạm vi thực sự kiểm

Đã đọc `.agent`, HANDOFF, baseline áp dụng, AGENTS hiện hành và skill mythical-validation/game-design-review. Đã đọc bản tổng hợp `ART-DIRECTION-RETHINK.md`, audit01, nghiên cứu văn hóa02, sáu hệ thể hiện03 và phần bổ sung reference art-direction. Không tái xác minh toàn bộ nguồn khảo cổ bên ngoài trong review này; kiểm cách brief tách thông tin có nguồn với diễn giải và chỗ chưa biết.

Đã dùng `view_image` mở hai original:

- `prototypes/sprint13/experiment-11/assets/stylized-3d.png`: người, da, vải và kiến trúc đều nhiều chi tiết tự nhiên; dải portrait/icon phía dưới và UI tối chiếm phần lớn sự chú ý. Có chiều sâu và UI hội thoại nhưng chưa thấy quy tắc giản lược mạnh tương ứng nhãn3D cách điệu.
- `prototypes/sprint13/experiment-12/assets/graphic-animation.png`: cast và portrait đã có mảng hình, dáng người đối lập và biểu cảm; cảnh phơi lúa vẫn dày vật liệu, mái và hạt thóc. Nhân vật hành động lớn ở tiền cảnh cùng dải asset dưới làm ảnh đọc gần một bảng minh họa. UI bên phải là menu lựa chọn, chưa có một câu NPC nói.

Đã đọc nguyên prompt12 của `clay-craft` và `graphic-animation`: cả hai dùng chung khung65/35, hai mẫu toàn thân/ba portrait/sáu icon, rồi mới thay style. Đây là căn cứ cụ thể cho việc sửa thứ tự brief, không suy mọi phương án cũ đều giống nhau hoặc đã bị loại. Không thao tác gallery, chạy browser, Unity, runtime hoặc playtest trong review này.

## Những phần brief đã dùng được

- Main§1 tách tổ chức không gian, hình/tỷ lệ, render, chuyển động và UI. 3D không bị đánh đồng với photoreal, cũng không đòi camera tự do để chứng minh một game3D. Cách gọi preview không ngụ ý đã có model thật; lịch sử04/05 có geometry được giữ đúng.
- Main§4 và03 đưa ra quyết định hình cụ thể: nhìn chéo khối lớn/bóng gọn; dải ngang/cao độ; top-down hình học; sprite2D trong chiều sâu; camera gần người; hai cấp địa bàn/cuộc gặp. Các hướng là cấu trúc có thể vẽ khác nhau ngay ở frame đầu.
- 03 nêu tỷ lệ người/khung dưới dạng giả thuyết thử, không là thông số camera đã duyệt hoặc benchmark. Hướng A/C ưu tiên đường đi và cộng đồng; E ưu tiên sự hiện diện gần người. Có cơ sở để lựa chọn thứ tự nghiên cứu khác nhau.
- Main§5 bỏ việc nhồi toàn bộ cast/icon/UI vào mọi ảnh. Màn hình thế giới và trạng thái hội thoại được phân biệt; cùng ngôn ngữ qua các màn hình vẫn phải kiểm. Điều này giữ yêu cầu xuyên game của chủ dự án mà tránh lặp boardcatalog.
- Đã giữ các ranh giới: nhân vật nhận ngẫu nhiên, UI chỉ đọc tri thức được phép, camera không cấp quyền biết mọi bí mật. Đề xuất dải ngang không tự đổi game thành platformer; bản đồ không tự đổi route/time state; không có yêu cầu tạo model làm điều kiện cho ảnh xem trước.

## Các điểm cần làm rõ khi biến workshop thành prompt

### P2 — bản sắc cần một bước chuyển từ nguồn sang hình nhìn thấy

**Vị trí:** main§3 và§5; nghiên cứu02, phần quy tắc hình ảnh.

Nguồn đã được phân loại và các ý như mái/sàn, nhịp băng, vòng, động tác người đã được nêu. Tuy vậy, nếu prompt tiếp theo chỉ gom lại các từ này, một làng cách điệu phổ quát vẫn có thể xuất hiện. Độ cụ thể về cấu trúc camera hiện cao hơn độ cụ thể về cách biến đổi tham chiếu văn hóa.

Trước khi sinh ảnh, nên ghi một bảng ngắn **nguồn cụ thể → lựa chọn biến đổi của họa sĩ → vị trí nhìn thấy trong frame**. Ví dụ cần được gắn nguồn thật và luôn ghi là đề xuất: cô đọng hình mái được tham khảo thành hai mảng có silhouette rõ; dùng điểm đỡ/tay/chuyển trọng tâm của một tư thế cặp người để thiết kế động tác tương tác; chuyển nhịp băng thành cách nhóm hoạt động hoặc vùng thông tin, không thành texture phủ nền. Những phép biến đổi này không chứng nhận kết cấu nhà, tỷ lệ người hoặc giao diện có thật trong quá khứ.

Đây là khoảng cần cụ thể hóa của brief tạo ảnh, **không là blocker cho việc bàn giao workshop** và không phải yêu cầu nghiên cứu lại toàn bộ29module. Không cần tự chọn một costume hoặc palette làm chuẩn toàn campaign.

### P2 — so sánh A/B phải được gọi là so hai hệ, không suy nguyên nhân đơn

**Vị trí:** main§4–5 và03§4–5.

Ưu tiên A/B có lý vì đổi mạnh cách nhìn và đọc hành động. Tuy nhiên, A và B cùng đổi camera, tỷ lệ người, cách dàn nhóm và chiều sâu. Nếu một ảnh được thích hơn, chưa thể kết luận đó là do3D, do ngữ pháp văn hóa hoặc do mức giản lược.

Nên giữ vai nhân vật, việc đang xảy ra, bộ đồ vật chính, mức chi tiết mục tiêu và điều kiện ánh sáng gần nhau; ghi những phần cố ý thay đổi và những lệch phát sinh của ảnh độc lập. Kết luận bước đầu là chọn một tổ hợp để đào sâu. Khi cần biết nguyên nhân cụ thể mới thử cặp giữ camera/cast rồi đổi riêng hình khối hoặc bề mặt. Không đòi kiểm soát geometry tuyệt đối giữa ảnh sinh độc lập và không cần dựng model để làm việc này.

### P3 / câu hỏi ưu tiên — A/B chưa kiểm mạnh cảm giác ở gần một người

Root ưu tiên1/2, dùng3 làm đối chứng là lựa chọn có lập luận, chưa là lựa chọn được user duyệt. A từ trên cao và B nhìn ngang giúp đánh giá cảnh sống; chúng có thể chưa trả lời ý nghĩa “cảm giác3D” nếu người xem muốn cảm giác hiện diện, đi quanh cột hoặc thấy người trong tầm tay.

Hướng E cận người đáng giữ làm đối chứng bổ sung nếu A vẫn đọc như tranh hoặc mô hình nhìn xa. Đây là đề nghị về nhánh nghiên cứu khi có phản hồi, không yêu cầu tạo thêm mẫu ngay và không loại A/B. Preview của E cũng chỉ kiểm hình dung một góc; không chứng minh camera, rig hoặc chuyển động.

### P3 — tỷ lệ đọc được phải kiểm trên ảnh, chưa thể bảo đảm từ phần trăm

03 đề xuất A6–8%, C4–6% chiều cao khung; văn bản đã đánh dấu chúng là giả thuyết. Chưa có artifact để biết tuổi, tay, dụng cụ và hướng nhìn ở tỷ lệ đó có đọc được hay không. Đừng dùng các tỷ lệ này thành acceptance tự động hoặc kéo nhân vật thành người khổng lồ tiền cảnh chỉ để đạt tiêu chí biểu cảm.

Cần kiểm frame thế giới ở kích thước xem dự kiến cho đường đi, người chính và vật tương tác; kiểm mặt/người nói ở trạng thái hội thoại riêng. Một hướng có thể cố ý không cho đọc nếp mặt trong world view mà vẫn giữ cá tính qua silhouette và cận cảnh. Không có ngưỡng pixel hoặc điểm số được xác nhận trong review này.

## Đối chiếu sau sửa của root

Đã đọc lại main§3–5 sau thông báo sửa, không chỉ nhận lời bàn giao:

- **P2 nguồn→hình: đã làm rõ ở cấp tài liệu.** Main§3 thêm ba dòng nguồn cụ thể→biến đổi→dấu hiệu trong frame, ghi rõ không phải phục dựng và không bắt mọi hướng dùng đủ ba. Các dấu hiệu mái/sàn/khoảng âm, điểm tựa/cử chỉ và nhịp nhóm không còn chỉ là tên motif. Việc chọn phép biến đổi cho một prompt cụ thể vẫn là công việc của lượt tạo ảnh, chưa được coi là output.
- **P2 so sánh nhiều biến: đã làm rõ giới hạn.** Main§4 nói phản hồi so tổ hợp, cần giữ các trục còn lại ổn định khi muốn quy nguyên nhân. Tình huống và vai chung được giữ ở§5/03; sau này vẫn cần ghi các lệch thực tế của ảnh độc lập.
- **Câu hỏi về E: đã được đưa thành nhánh đối chứng có điều kiện.** Main§4 ghi cận người hữu ích nếu khối nhìn chéo vẫn bị đọc như tranh/mô hình, không tự tăng scope hay nhận đó là sở thích user.
- **Tỷ lệ: giữ đúng là giả thuyết.** §5 yêu cầu xem ở cỡ dự kiến, tách frame thế giới với đối thoại và không biến số thành production budget. Chưa có ảnh mới để đóng câu hỏi readability.

Main còn thêm bảng khác biệt so với06C/08C/11/12, giúp lần sau nhận diện trường hợp chỉ đổi tên hoặc lớp phủ. Tôi không nhận lượt root mở06C thành lượt xem của reviewer; phạm vi ảnh thực sự xem của tôi vẫn là hai file ở đầu hồ sơ.

## Kết luận có giới hạn

Không phát hiện P1 chặn bàn giao một workshop có thể dùng để viết brief mới. Các P2 đã được root làm rõ ở cấp tài liệu; cần cụ thể hóa lựa chọn theo từng prompt khi được giao lượt tạo ảnh, không xem đó là kết quả mỹ thuật đã đạt hoặc bằng chứng một hướng thất bại. Nguồn văn hóa, mức giản lược và tính nhận diện mới chỉ được lập luận, chưa được chứng minh bằng output.

Không có ảnh/model/runtime mới, benchmark, chi phí production, human playtest hoặc kết quả gameplay. Self-review của root khác review độc lập này. Tất cả01–12 được giữ; bản tổng hợp, thứ tự thử và các phương án đều **Pending user approval**, chưa đóngSprint13 hoặc chuyểnSprint14. Reviewer chỉ tạo hồ sơ này, không sửa các bài khác hay thực hiện Git publish.
