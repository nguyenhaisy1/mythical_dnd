# ADR021 — Điều kiện sự kiện khai báo, chưa thực thi hậu quả

Ngày 2026-10-26. Trạng thái: quyết định triển khai prototype Sprint12/VS02B. Không thay archive hoặc acceptance đầy đủ của VS02.

## Vấn đề và quyết định

Sprint11 đã có dân cư và đường đi nhưng chưa có tám template sự kiện được kiểm bằng code. Domain chưa hiện thực vận tải, sửa thuyền, thương tích và thông tin lan truyền. Vì vậy không đăng ký handler giả để làm như các sự kiện đó đã diễn ra.

Bổ sung profile riêng `event-eligibility-v1`, biên dịch thành registry bất biến trong Infrastructure portable. Profile tham chiếu đúng hash của `settlement-content-v1`; compiler cũ không được nới để nhận loại chưa hỗ trợ. `eligibility-candidate-v1` là tên duy nhất được cho phép cho **bộ đánh giá ứng viên**, không là `IRuleHandler`, không gọi CommandProcessor hoặc tạo occurrence. Event effects và binding vào authoritative world phải có contract/acceptance riêng trước khi triển khai.

Input là primitive dictionary tree nghiêm ngặt; unknown/missing fields, version sai, script tùy ý, refs sai, sai owner, kiểu/range sai đều bị từ chối. JSON host có giới hạn kích thước/depth, không nhận duplicate keys hoặc số thực. Hash mdc1 bao gồm metadata, localization, conditions và settlement hash; templates được sắp xếp theo ID trước hash. Không đổi save schema hay kernel-content-v1; event pack chưa gắn vào save compatibility.

## AST và quyền đọc

Sáu node cho phép: `All`, `Any`, `Not`, `Compare`, `HasKnownFact`, `HasResource`. Không eval code từ content. Mỗi template có hai biểu thức độc lập:

- `physical_condition`: scalar vật lý allowlist và grain/timber; chỉ xác định prerequisites trong context đã cung cấp.
- `perceived_condition`: các cờ hiểu biết allowlist và known fact IDs của đúng actor; không được đọc scalar vật lý hoặc kho tài nguyên bí mật.

| Field | Owner dữ liệu nguồn khi adapter được triển khai | Kiểu/unit prototype |
|---|---|---|
| boat_condition | Assets | integer 0–1000 condition points |
| rain_intensity | Chưa chốt owner/adapter môi trường; chỉ fixture | integer 0–1000 intensity points |
| deadline_remaining_ticks | Economy (contract deadline) | integer -1.000.000..1.000.000 simulation minute ticks |
| injury_severity | Health | integer 0–1000 severity points |
| landing_access_disputed | Assets | bool; chỉ claim dispute flag, không cấp quyền |
| rumor_delivered | Knowledge | bool; delivery không chứng minh nội dung là sự thật |
| repair_need, transport_request, landing_dispute, treatment_request, food_shortage | Knowledge | bool thuộc nhận thức actor |
| grain, timber | Assets | integer 0–1.000.000 prototype count units |

Các scalar này là **contract của observation fixture**, chưa là field mới trong WorldState và chưa chứng minh owner adapter. Threshold/cooldown là dữ liệu prototype tập trung trong event pack có version/hash; không sao chép thành balance constant trong code. Chúng không thay thông số repair thực thi trong kernel-v1.

## Unknown, thời gian và ranh giới tin cậy

Missing scalar/resource → Unknown. Not(Unknown) vẫn Unknown; All/Any dùng logic ba giá trị. Chỉ True ở cả physical và perceived mới eligible. `KnownFactIds` là tập tri thức **đầy đủ** do caller cung cấp cho actor tại tick đang xét; vắng fact trong tập này là False, không đồng nghĩa fact sai ngoài thế giới. Caller chưa có tập đầy đủ không được dùng evaluator như một lời khẳng định đã kiểm knowledge isolation end-to-end.

Context gắn actor ID và alive do caller cung cấp; registry kiểm refs và actor trong allowlist. Adapter tương lai phải bảo đảm provenance và tính nhất quán world revision/tick, không để UI gửi tự do physical/perceived values. Đây là trusted developer API, chưa là security boundary cho player. Lý do `physical-false/unknown` chỉ dành developer QA, không chuyển nguyên trạng sang UI.

`lastOccurrenceTick` là tick occurrence đã commit của **đúng cặp template/actor**, do caller cung cấp; null nghĩa chưa có occurrence. Tick không âm, không từ tương lai; so hiệu tick để tránh overflow. Evaluate không ghi cooldown. Cần adapter persistence cho occurrence trước khi dùng runtime thực thi. Không có player-proximity flag, không RNG, không wall clock, không state write.

## Nghiệm thu và giới hạn

Tám template có fixtures đủ/không đủ điều kiện, thêm dead actor, cooldown boundary, missing observation và no-player candidate. Tests phải kiểm AST, sai quyền đọc, alias mutation, refs/hash và input malformed. Independent review và full regression trước merge.

Đạt profile này chỉ chứng minh đánh giá trên observations có kiểu đúng. Chưa chứng minh actor tự tìm việc, rumor thực sự đi qua graph, transport/repair/medicine effects, world initialization, random protagonist, player projection hay Unity gameplay. FullVS02 giữ partial.

## Tác động

Code/compiler/host/schema/content/fixtures/tests và runbook mới; cập nhật backlog, handoff, QA, integration notes và exports. Không đổi settlement pack/hash, kernel goldens, source archive, product VERSION hoặc release tags. [Lộ trình chơi thử](../production/PLAYABLE-ROADMAP.md) là forecast planning riêng, không hạ acceptance milestone.
