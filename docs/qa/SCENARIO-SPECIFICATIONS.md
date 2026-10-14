# Scenario nghiệm thu xuyên hệ thống

## SCN-01 — Sửa thuyền trước mùa nước

Given: household Vạc Tòng có thuyền xuống cấp; actor có quyền sửa, có năng lực/công cụ và lượng vật liệu xác định; hợp đồng giao hàng có deadline; route có hướng và capacity. Không mặc định actor là player hoặc có nghề tương ứng.

When: actor lập work plan; engine kiểm quyền/capability/resources, reserve/consume theo contract đã chọn, schedule completion. During work, actor hoặc thuyền có thể đổi condition; completion phải revalidate điều kiện.

Then: Assets ghi vật liệu và condition đúng một lần, skill learning có evidence, elapsed time cập nhật, Economy re-evaluate delivery feasibility; witness/claim lan theo Knowledge, Narrative phản ánh fact. UI không biết nguyên nhân sự cố nếu chưa điều tra. Cùng command retry không tạo repair thứ hai.

Variants: thiếu vật liệu; đồ đang mượn; actor chết giữa chừng; route bị cắt; DB commit fail; player không tham gia và NPC tìm thợ khác. Outcome thất bại có hệ quả hợp lý, không tự reset thuyền.

## SCN-02 — Người rời Bến Vạc và tin đồn

Given: household chịu áp lực lương thực, một actor cân nhắc di cư, người chơi chỉ có lời kể thiếu chắc chắn. Một rumor chain có origin rõ.

When: NPC lập kế hoạch bằng perceived knowledge; người chơi có thể giúp việc, hỏi nguồn tin, cản trở hoặc bỏ qua. Rumor đi qua mạng xã hội/route với độ trễ.

Then: migration phải có tuyến/thời gian/nguồn lực; reputation và relationship thay theo điều người liên quan biết. Tin lặp lại từ cùng origin không biến thành nhiều nguồn độc lập. Quest view không bật một marker “sự thật” từ global state.

## SCN-03 — Xung đột quyền sử dụng bến

Given: owner, custodian, user và claimant có thể là các bên khác nhau. Faction và household có mục tiêu; đường giao thương có giá trị thật.

When: player dùng đàm phán, bằng chứng, phương án logistics hoặc combat. Companion có thể từ chối một lệnh vi phạm đạo đức.

Then: rights chỉ đổi qua transition hợp lệ; objective hoàn thành không buộc giết hết đối phương. Wounds, captivity, property damage, witnesses và obligations tiếp tục sau encounter. Không cấp phần thưởng từ kho không có vật phẩm.

## SCN-04 — Nhiều năm và người kế tục

Given: snapshot sau một trong ba scenario trên; có scheduled work/recovery và một successor có quan hệ thật. Một bí mật chỉ protagonist biết, một tài sản protagonist đang mượn.

When: time skip qua job/death/inheritance breakpoints; đổi quyền điều khiển sang successor.

Then: không dead actor hoàn tất công việc; asset rights và knowledge truyền đúng rule; player archive vẫn đọc được nhưng không mở character-gated option. Chronicle có provenance đến committed events. Replay từ initial state tạo cùng kết quả trong cùng phiên bản.

## SCN-05 — Historical proof và no-intervention

Không cần dựng toàn Cổ Loa trong slice. Dùng fixture có một Hard Anchor và nhiều causal paths hợp lệ. Thử player không làm gì, phá một path và phá mọi path được phép. Hai trường hợp đầu cần resolve hoặc trì hoãn hợp lệ theo contract; trường hợp cuối phải được content/design validation nhận ra, không bí mật spawn tài nguyên để ép lịch sử. Phạm vi được bảo vệ phải được thiết kế trước tương tác.

## SCN-01 — lựa chọn áp dụng sau audit

Repair dùng [Command Catalog](../contracts/COMMAND-ACTIVITY-CATALOG.md): reserve2 timber khi StartRepair, consume khi CompleteRepair, cancel release phần còn tồn tại; destruction không refund vật liệu đã mất. Duration60 ticks, condition+200 là prototype parameters có version. [Kernel smoke](../../content/fixtures/kernel-smoke/README.md) ghi chuỗi và expected values; không phải runtime test đã pass.
