# Thử nghiệm nhận một cuộc đời — Sprint 13 / Experiment 01

**Trạng thái: Pending user approval.** Đây là mô hình UX độc lập để chủ dự án xem cách giới thiệu nhân vật. Không phải generator sản xuất, save game, world simulation hoặc acceptance full VS02. Actor thực hiện: `/root/s13_campaign`; root tổng hợp và critics review riêng.

## Câu hỏi cần kiểm chứng

Khi không được chọn xuất thân hay chỉnh chỉ số, người xem có hiểu mình là ai, đã trải qua điều gì và vì sao mình có những khả năng hiện tại không? Màn Reveal phải kể được mối liên hệ này, rồi cho phép bước vào giao diện quan sát. Chưa có dữ liệu người dùng trải nghiệm; kiểm thử code không trả lời thay câu hỏi hấp dẫn hay dễ hiểu.

## Phạm vi và giả định

Ba hoàn cảnh gia đình hư cấu tại Bến Vạc: nhà giữ dây neo, nhà bên mái sửa gỗ, nhà cạnh sân phơi. Mỗi hoàn cảnh có một ký ức tuổi nhỏ và hai khả năng lịch sử trưởng thành. Tất cả tên, gia đình, tuổi, hệ số và tình tiết trong catalogue này **được soạn riêng cho prototype**, không thêm vào canon hay dùng ID của NPC đã được biên soạn trong settlement pack.

Chuỗi sinh: seed → hoàn cảnh gia đình → lịch sử phù hợp → cộng các đóng góp kỹ năng/thuộc tính từ lịch sử → tri thức cá nhân có nguồn ký ức → kiểm sống và trưởng thành → DTO chỉ phục vụ nhân vật. Sáu thuộc tính có nền đồng đều 3, các trải nghiệm tăng một số thuộc tính. Nền 3 và mức tăng là giả định UX, không balance được phê duyệt. Không random chỉ số độc lập rồi viết biography cho khớp.

Tuổi 18–26 phụ thuộc tuổi kết thúc lịch sử và khoảng thời gian từ đó. Viability ở đây chỉ kiểm còn sống, trưởng thành và trình tự tuổi; nơi nương tựa/công việc được catalogue cung cấp. Chưa chứng minh khả năng sinh tồn bằng economy/health/autonomy. Không có aptitude, giới tính, uniqueness passive, genealogy đầy đủ hoặc hàng nghìn nhân vật. Sáu nhánh lịch sử có thể trùng giữa nhiều campaign; đây không phải mục tiêu diversity sản xuất đã đạt.

## API cho giao diện

Nạp `campaign.js` bằng script thường; module cung cấp `window.MythicalCampaign`. Node dùng `require('./campaign.js')`.

```js
const session = MythicalCampaign.createCampaign('opaque-seed-01', 'campaign-01');
const actor = session.reveal();
```

`seed` và `campaignId` là chuỗi khác rỗng, tối đa 256 UTF-16 code units. Caller cấp seed ở tầng session; không hiện điều khiển chọn seed trên Reveal. Campaign ID chỉ định danh, không ảnh hưởng cuộc đời được sinh. `reveal()` luôn trả cùng immutable actor của session và không rút số ngẫu nhiên lại. Muốn campaign mới phải tạo session mới từ màn menu; không có API reroll trong session. Không lưu durable: reload trang có thể mất session nếu caller không giữ; không tuyên bố Saved/Continue.

DTO gồm `id`, `name`, `age`, `alive`, `community`, `household`, `origin`, `history`, `attributes`, `skills`, `knownFacts`, `obligation`, `viability`, `status`. `historyIds` nối kỹ năng/thuộc tính với lịch sử; `sourceHistoryId` nối tri thức với trải nghiệm cá nhân. Không có hidden actor plans, private inventory của NPC, exact probability hay secret world state. Tri thức trong DTO là ký ức của nhân vật, không xác nhận tình trạng hiện tại của thuyền/tuyến đường. UI không dùng DTO này làm authoritative world state.

## Tính tái lập và giới hạn thuật toán

Version `s13-origin-experiment-v1`: mỗi lượt chọn tạo chuỗi `JSON.stringify([VERSION, seed, purpose])`, FNV-1a trên UTF-16 code units (offset 2166136261, multiplier 16777619, `Math.imul`, uint32), rồi một bước xorshift32 (left13/right17/left5); lấy unsigned modulo số lựa chọn. Purpose tách context/history/elapsed-years/name. Catalogue có thứ tự cố định trong file; không dùng clock, network, LLM hay `Math.random`.

Đây **không phải** SHA256 counter RNG của core, không claim conformance/replay giữa JS và C#, không bảo đảm phân bố đều tuyệt đối do modulo. Thay catalogue hay thuật toán phải đổi experiment/version và kiểm lại; không đưa các ID/seed này vào production save. Không dựa browser RNG để quyết định gameplay sản xuất.

## Kiểm tra và bước nghiệm thu

Chạy `node --test prototypes/sprint13/experiment-01/campaign.test.js` từ root. Năm test kiểm tái lập/no reroll, 500 mẫu trưởng thành với kỹ năng dẫn xuất đúng lịch sử, đủ 3 hoàn cảnh/6 nhánh trong tập fixture, immutable/no alias, input lỗi và browser/CommonJS parity không clock/global random. Đây là test mô hình JavaScript; không Unity/gameplay acceptance.

Để đánh giá UX, chủ dự án xem Reveal rồi trả lời: nhân vật thuộc gia đình nào; một kỹ năng bắt nguồn từ trải nghiệm nào; điều gì chỉ là ký ức chứ chưa biết ở hiện tại; có cảm giác bị buộc chọn lại vì không hiểu cơ hội hành động không. Ghi nguyên phản hồi thật trước khi sửa thử nghiệm. Chưa có user approval; không tiến Sprint14 chỉ vì test code đạt.
