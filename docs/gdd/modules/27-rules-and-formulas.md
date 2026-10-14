# Module 27 — Rule Engine và Formula Specification

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T073](../../source/conversation.md#t073)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Công thức gắn trạng thái locked principle hoặc prototype coefficient. Không biến giá trị ví dụ thành final balance. Constraint trước roll; routines có auto-resolution; randomness có context/version. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Typed rule input, capabilities, resources, parameters, seed context và snapshot. |
| Đầu ra / bên sở hữu | Reject/proposed ChangeSet/scheduled plan; không ghi DB trực tiếp. |
| Đọc cùng | 09, 12–14, 18, 24–26, 29.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Replay cùng inputs/version; hard constraints không bị điểm số/roll cao vượt qua; trace giải thích từng modifier. |
| Việc chưa được chứng minh | Transport/medicine prototype coefficients trước VS-03; final tuning VS-10. Numeric policy và kernel repair parameters đã có ADR-008. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T073 BEGIN -->

<a id="m27-t073-s001"></a>

## GDD MODULE 27
<a id="m27-t073-s002"></a>

## RULE ENGINE & FORMULA SPECIFICATION
<a id="m27-t073-s003"></a>

### Hệ quy tắc, thuật toán xác suất và công thức vận hành thế giới

**Dự án:** Living World Narrative RPG — cuối Văn Lang, chuyển tiếp Âu Lạc và Cổ Loa  
**Phiên bản:** 1.0 — Technical Formula Baseline  
**Trạng thái:** Đặc tả thuật toán đề xuất để triển khai prototype  
**Phụ thuộc:** GDD Modules 01–26  
**Định hướng:** Rule-based simulation, deterministic randomization, causal procedural generation và long-term systemic narrative.

---

<a id="m27-t073-s004"></a>

## PHẦN I — MỤC TIÊU CỦA RULE ENGINE

<a id="m27-t073-s005"></a>

### 1. Rule Engine phải giải quyết bài toán gì?

Rule Engine là thành phần quyết định một hành động hoặc sự kiện có thể xảy ra hay không, xảy ra với những điều kiện nào, kết quả ra sao và trạng thái thế giới thay đổi như thế nào.

Nó không phải một bộ sinh văn bản.

Nó cũng không đơn thuần là tập hợp các công thức combat.

Hệ thống phải xử lý được:

- Một đứa trẻ có thể sinh ra trong household nào.
- Quá trình trưởng thành tạo ra những kỹ năng và quan hệ gì.
- Một NPC có nhu cầu, mục tiêu và trách nhiệm sẽ chọn hành động nào.
- Một vấn đề tại settlement có đủ điều kiện trở thành event hay không.
- Một hành động cần kỹ năng được đánh giá như thế nào.
- Một lời hứa có bị vi phạm hay không.
- Một lô hàng có thể đi qua tuyến đường nào.
- Một cuộc tấn công gây thương tích ở mức nào.
- Một workshop sản xuất được bao nhiêu hàng hóa.
- Một faction có đủ nguồn lực để huy động lao động hoặc quân đội hay không.
- Một nhân vật có đủ bằng chứng để hình thành Class hay không.
- Một cuộc đời đã kết thúc có thể để lại di sản gì.

**Mục tiêu không phải có một công thức duy nhất giải quyết tất cả.**

Mục tiêu là một kiến trúc quy tắc thống nhất về:

1. Điều kiện hợp lệ.
2. Dữ liệu đầu vào.
3. Quyết định có tính xác định.
4. Bất định có kiểm soát.
5. Hậu quả.
6. Kiểm chứng.

Mỗi domain được phép có công thức chuyên biệt nhưng phải tuân cùng một hợp đồng thực thi.

<a id="m27-t073-s006"></a>

### 2. Ba tầng quyết định bắt buộc

Đề xuất Rule Engine chia thành ba tầng logic.

<a id="m27-t073-s007"></a>

#### Tầng 1 — Hard Constraints

Kiểm tra điều kiện không thể thương lượng.

Ví dụ:

Một người không thể tấn công nếu đã chết.

Một người không thể sử dụng thuyền không có ở địa điểm đó.

Một workshop không thể dùng lượng vật liệu lớn hơn số thực có sẵn.

Một NPC không thể sử dụng bí mật chưa biết để lập kế hoạch.

Một Event Template không thể ràng buộc vai trò `witness` vào NPC chưa từng có khả năng quan sát sự kiện.

Nếu một hard constraint không đạt:

**Kết quả là Ineligible hoặc Rejected.**

Không thực hiện random để xem hành động có được phép phá luật hay không.

<a id="m27-t073-s008"></a>

#### Tầng 2 — Resolution

Nếu hành động hợp lệ, engine tính kết quả.

Tùy domain, có thể sử dụng:

- công thức xác định;
- phép thử 2d10;
- phân phối xác suất;
- utility ranking;
- constraint solving;
- graph algorithm;
- state-machine transition.

Ví dụ một người có đủ điều kiện để thử sửa thuyền.

Việc sửa có thành công và đạt chất lượng nào phụ thuộc kỹ năng, vật liệu, công cụ, thời gian, trạng thái thuyền và phép thử phù hợp.

<a id="m27-t073-s009"></a>

#### Tầng 3 — Consequence Validation

Sau khi tính kết quả, engine kiểm tra:

- có vi phạm inventory conservation không;
- có tạo trạng thái NPC bất khả thi không;
- có phá ownership invariant không;
- có làm một Historical Anchor vô nghiệm không;
- có tạo hai event cùng chiếm một tài nguyên độc quyền không;
- có tạo một trạng thái Story Thread không thể tiếp tục không.

Nếu kết quả không hợp lệ do lỗi thiết kế hoặc dữ liệu:

**Không commit và ghi diagnostic.**

Không được tùy ý random lại tới khi tìm được kết quả có vẻ ổn mà không có quy trình sinh có ràng buộc được khai báo trước.

<a id="m27-t073-s010"></a>

### 3. Công thức tổng quát

Một Rule Evaluation được mô hình hóa:

**Kết quả = Resolve(Hành động, Trạng thái, Năng lực, Điều kiện, Ngẫu nhiên hợp lệ, Phiên bản quy tắc)**

Trong đó:

- `Action`: Yêu cầu hành động.
- `State`: Snapshot authoritative.
- `Capability`: Những gì actor thật sự có thể thực hiện.
- `Context`: Điều kiện vật lý, xã hội, thời gian và môi trường.
- `RandomContext`: Nguồn bất định được kiểm soát.
- `RuleVersion`: Phiên bản thuật toán.

Công thức thực thi:

```text
function Execute(command, world):
    plan = BuildPlan(command, world.readonly)

    if not plan.hardConstraintsSatisfied:
        return Rejected(plan.failures)

    result = Resolve(plan, deterministicRng)

    proposedChanges = BuildEffects(result)

    if not ValidateInvariants(proposedChanges):
        return SystemErrorWithTrace()

    CommitAtomically(proposedChanges)

    return CommittedResult()
```

Đây là logic kiến trúc; implementation cần có staged state, revision checks và quản lý những random draws đã được sử dụng để đảm bảo retry không tạo kết quả khác.

---

<a id="m27-t073-s011"></a>

## PHẦN II — HỆ GIÁ TRỊ, ĐƠN VỊ VÀ ĐỘ CHÍNH XÁC

<a id="m27-t073-s012"></a>

### 4. Chuẩn hóa đơn vị là điều kiện bắt buộc

Nếu Combat, Economy, Travel và Health dùng đơn vị không thống nhất, việc thiết kế công thức sẽ tạo lỗi.

Đề xuất dùng bảng đơn vị canonical.

| Nhóm | Đơn vị lõi | Ghi chú |
|---|---|---|
| Thời gian | World Tick số nguyên | Quy đổi sang giờ/ngày/mùa theo lịch game |
| Attributes | 1–10 | Sáu thuộc tính đã khóa |
| Skills | 0–100 | Mastery |
| Xác suất nội bộ | 0–10.000 | Đơn vị điểm xác suất, tương ứng 0–100% |
| Utility NPC | 0–1.000 | Điểm xếp hạng quyết định |
| Condition item | 0–1.000 | Mức trạng thái kỹ thuật theo item type |
| Số lượng tài nguyên | Số nguyên theo đơn vị nhỏ nhất | Tránh cộng trừ tài nguyên bằng float tùy tiện |
| Quãng đường chiến lược | Distance Unit thống nhất | Có thể có giá trị fixed-point |
| Thời gian travel | World Tick | Phải là kết quả của route |
| Vị trí tactical | Zone + Slot ID | Không dùng GPS/metric không cần thiết |

Các thang 0–1.000 và 0–10.000 là đề xuất kỹ thuật, chưa phải chỉ số người chơi phải nhìn thấy.

<a id="m27-t073-s013"></a>

### 5. Fixed-point và deterministic mathematics

Đối với các giá trị quan trọng, có thể sử dụng số nguyên fixed-point.

Ví dụ:

`0.725` được lưu thành `725` trên thang 1.000.

Một phép nhân hai giá trị dạng fixed-point cần làm tròn theo cùng một quy tắc:

```text
MulFixed(a, b) = RoundDeterministic(a * b / 1000)
```

Phải kiểm soát overflow bằng kiểu số đủ lớn ở các phép tính trung gian.

Phương thức làm tròn phải được khóa, ví dụ làm tròn về số nguyên gần nhất với cách xử lý trường hợp bằng nhau được quy định rõ.

Không để các domain sử dụng các quy tắc làm tròn khác nhau.

<a id="m27-t073-s014"></a>

### 6. Các hàm dùng chung

Rule Library cần những hàm cơ bản:

```text
Clamp(value, min, max)
Normalize(value, min, max)
FixedMul(a, b)
FixedDiv(a, b)
CompareThreshold(value, threshold)
WeightedChoice(candidates, weights, rng)
BoundedRandom(min, max, rng)
EvaluateCondition(condition, snapshot)
ResolveReference(entityId)
ValidateRange(value)
```

`WeightedChoice` phải hoạt động trên danh sách candidate được sắp theo ID ổn định.

`BoundedRandom` cần tránh sai lệch phân phối do lấy modulo không đúng với miền PRNG; có thể sử dụng rejection sampling trên số nguyên.

Không cần một thư viện xác suất quá rộng trong prototype, nhưng những hàm cốt lõi phải có test.

---

<a id="m27-t073-s015"></a>

## PHẦN III — DETERMINISTIC RNG

<a id="m27-t073-s016"></a>

### 7. Mỗi world có một nguồn ngẫu nhiên có thể tái hiện

Khi bắt đầu campaign, engine tạo:

```text
WorldSeed
RulesetVersion
RandomAlgorithmVersion
```

Mỗi domain dùng RNG context riêng.

Ví dụ:

```text
RNGContext
    WorldSeed
    DomainID
    EntityID
    EventInstanceID
    DecisionID
    DrawIndex
```

Engine không được tự lấy thời gian thực của máy để random lại một sự kiện khi player đang chơi.

<a id="m27-t073-s017"></a>

### 8. Hai phương thức RNG được đề xuất

<a id="m27-t073-s018"></a>

#### Stateful Domain Stream

Dùng cho simulation có chuỗi quyết định rõ.

Ví dụ ecological population sampling.

Engine lưu PRNG state hoặc counter.

<a id="m27-t073-s019"></a>

#### Keyed Deterministic Random

Dùng cho event cụ thể khi cần tránh việc thêm một lần random ở domain khác làm thay đổi outcome.

Ý tưởng:

```text
RandomValue = PRNG(
    WorldSeed,
    StableEventKey,
    DecisionKey,
    DrawIndex
)
```

Phải sử dụng thuật toán xác định được version hóa.

Không sử dụng `GetHashCode()` mặc định của object/string làm stable hash khi runtime không bảo đảm giá trị ấy bất biến xuyên phiên bản và môi trường.

<a id="m27-t073-s020"></a>

### 9. Thứ tự tính toán

Một quy tắc xác suất chỉ được chạy sau:

1. Validating actor.
2. Validating context.
3. Validating available actions.
4. Resolving rule inputs.
5. Establishing event identity.
6. Establishing random context.

Nếu thiếu vật liệu, việc sửa thuyền bị từ chối.

Không được rút xúc xắc trước rồi sau đó mới phát hiện không có vật liệu.

Nếu hành động bị từ chối vì hard constraint, không tiêu thụ một RNG draw của outcome.

<a id="m27-t073-s021"></a>

### 10. Chống random exploit

Một người chơi có thể tìm cách gửi cùng command nhiều lần.

Đề xuất:

- Command ID có tính duy nhất.
- Retry của cùng command trả lại kết quả đã commit.
- Attempt mới phải có trạng thái và chi phí hợp lệ.
- RNG draw không được đổi chỉ vì UI được mở/đóng.
- Preview không được tiêu thụ RNG outcome.
- Không cho player liên tục thử một hành động không có thời gian hoặc tài nguyên tiêu hao nếu logic nghề nghiệp thực tế không cho phép.

Ví dụ:

Thử chế tác một vật yêu cầu vật liệu.

Thất bại có thể tiêu hao thời gian hoặc vật liệu tùy mức độ.

Player không thể bấm lại vô hạn mà không trả giá, chờ điều kiện mới hoặc có sự thay đổi cách làm.

<a id="m27-t073-s022"></a>

### 11. Kiểm thử RNG

Các test tối thiểu:

**Replay:** Cùng input tạo cùng giá trị.

**Ordering:** Danh sách candidate có thứ tự bất kỳ vẫn được chuẩn hóa trước khi chọn.

**Distribution:** Một triệu lần lấy mẫu trong test thống kê phải đạt khoảng sai số hợp lý so với phân phối yêu cầu.

**Isolation:** Thêm random draw không liên quan trong Economy không đổi kết quả Combat của event đã định danh.

**Retry:** Retry cùng command không làm tăng inventory hoặc đổi kết quả.

**Version:** Save ghi đủ thông tin để phân biệt phiên bản RNG.

---

<a id="m27-t073-s023"></a>

## PHẦN IV — RULE DEFINITION & RULE DSL

<a id="m27-t073-s024"></a>

### 12. Không viết hàng triệu câu `if/else`

Đối với engine, mỗi rule nên có cấu trúc:

```text
RuleDefinition
    RuleID
    Version
    Domain
    Preconditions
    InputBindings
    CalculationHandler
    OutcomeMapping
    AllowedEffects
    ValidationPolicy
```

Content chỉ tham chiếu các handler đã được viết bằng code và có test.

Không cho phép event JSON định nghĩa một đoạn C# tùy ý hoặc thực hiện biểu thức không được kiểm soát.

<a id="m27-t073-s025"></a>

### 13. Phân loại điều kiện

<a id="m27-t073-s026"></a>

#### Hard Preconditions

Không đạt thì hành động không thể thực hiện.

Ví dụ:

Actor phải Alive.

Item phải tồn tại.

Route phải có phương tiện phù hợp.

<a id="m27-t073-s027"></a>

#### Contextual Modifiers

Không chặn hành động, nhưng ảnh hưởng độ khó hoặc kết quả.

Ví dụ:

Mưa lớn khiến việc dẫn thuyền khó hơn.

<a id="m27-t073-s028"></a>

#### Social Constraints

Quy định sự chấp nhận của NPC và hiệu lực của quan hệ.

Ví dụ:

NPC có đủ thẩm quyền ký hợp đồng không?

<a id="m27-t073-s029"></a>

#### Temporal Constraints

Quy định cửa sổ thời gian.

Ví dụ:

Công việc thu hoạch chỉ phù hợp khi cây đã tới giai đoạn thu hoạch.

<a id="m27-t073-s030"></a>

#### Historical Constraints

Giữ các bất biến đã khóa của campaign.

Ví dụ:

Một interaction không được hứa với player khả năng hoàn toàn xóa bỏ một Hard Historical Anchor nếu GDD không hỗ trợ.

<a id="m27-t073-s031"></a>

### 14. Thứ tự ưu tiên khi nhiều rule xung đột

Đề xuất:

**P0 — Engine Invariants:** Không được phá.

**P1 — Domain Hard Preconditions:** Không được bỏ qua bằng modifier.

**P2 — Historical Protected Constraints:** Có hiệu lực trong phạm vi Canon đã công bố.

**P3 — Actor Authority & Moral Constraints:** Được áp dụng theo character và social rules.

**P4 — Contextual Effects:** Điều chỉnh hiệu quả hành động.

**P5 — Narrative Presentation:** Không được thay đổi bất kỳ tầng trên.

Nếu hai rule cùng cấp mâu thuẫn, phải có rule resolver hoặc trả về content/design error.

Không được chọn ngẫu nhiên rule nào được ưu tiên.

---

<a id="m27-t073-s032"></a>

## PHẦN V — UNIVERSAL ACTION RESOLUTION

<a id="m27-t073-s033"></a>

### 15. Không phải mọi hành động đều cần xúc xắc

Đây là quyết định quan trọng.

Một character có năng lực phù hợp đang làm công việc đơn giản trong điều kiện tốt không cần roll liên tục.

Ví dụ:

- đi bộ trên đường quen thuộc;
- buộc dây theo thao tác đã thành thạo;
- dùng công cụ thường ngày;
- thực hiện một công việc routine phù hợp kỹ năng.

Engine có thể giải quyết bằng deterministic outcome.

Roll chỉ cần khi có:

- bất định thực sự;
- mức độ khó đáng kể;
- áp lực;
- xung đột;
- hậu quả quan trọng;
- đối tượng phản kháng;
- môi trường không ổn định.

Điều này tránh biến gameplay nghề nghiệp thành chuỗi xổ số.

<a id="m27-t073-s034"></a>

### 16. Action Resolution Formula

GDD đã xác định nền 2d10 cho những hành động có rủi ro.

Đề xuất công thức:

**Action Score = 2d10 + Skill Bonus + Attribute Bonus + Technique Bonus + Preparation Bonus + Context Modifier − Condition Penalty**

So sánh:

**Margin = Action Score − Difficulty**

Trong đó:

| Thành phần | Công thức prototype |
|---|---|
| 2d10 | Hai xúc xắc độc lập, mỗi xúc xắc từ 1 đến 10 |
| Skill Bonus | `floor(Skill / 10)` |
| Attribute Bonus | `floor((Attribute - 5) / 2)` |
| Technique Bonus | 0–3 nếu có Technique áp dụng |
| Preparation Bonus | 0–3 với điều kiện chuẩn bị thực tế |
| Context Modifier | Thông thường -4 đến +4 |
| Condition Penalty | 0–4 với trạng thái bất lợi |

Các khoảng trên là bộ tham số đầu tiên, không phải giới hạn cuối cùng cho mọi hành động đặc biệt.

Đối với hiệu ứng nghiêm trọng, thay vì chồng quá nhiều modifier tới mức méo công thức, nên kiểm tra có cần chuyển sang một loại action/resolution khác.

<a id="m27-t073-s035"></a>

### 17. Difficulty Bands

Đề xuất ban đầu:

| Difficulty | Mức thử thách | Ví dụ |
|---|---|---|
| 10 | Dễ khi đủ điều kiện | Sửa lỗi quen thuộc trong môi trường thuận lợi |
| 14 | Thông thường nhưng có bất định | Nhận biết dấu vết khá rõ |
| 17 | Khó vừa | Sửa bộ phận quan trọng khi có áp lực |
| 20 | Khó | Dẫn thuyền qua đoạn nước phức tạp |
| 24 | Rất khó | Xử lý kỹ thuật khó trong điều kiện thiếu thốn |
| 28 | Đặc biệt khó | Thành tựu chỉ người rất giỏi và chuẩn bị tốt mới có khả năng |
| 30+ | Ngoại lệ được authored | Không dùng tràn lan |

Độ khó là thuộc tính của tình huống, không tự tăng theo level player.

Một thợ giỏi có quyền thấy công việc thông thường ngày càng dễ.

<a id="m27-t073-s036"></a>

### 18. Ví dụ tính toán

Mân thử sửa một bộ phận thuyền khó.

Dữ liệu:

- Skill = 50.
- Attribute XẢO = 7.
- Technique Bonus = 0.
- Preparation Bonus = +2.
- Context Modifier = 0.
- Condition Penalty = 0.
- Difficulty = 20.

Tính:

Skill Bonus = 5.

Attribute Bonus = 1.

Preparation Bonus = 2.

Tổng modifier = +8.

Mân cần 2d10 đạt ít nhất 12 để thành công.

Với hai xúc xắc mười mặt công bằng, xác suất ra tổng từ 12 trở lên là 45%.

Nếu Mân không chuẩn bị, tổng modifier giảm còn +6, cần 2d10 ít nhất 14; xác suất thành công chỉ còn 28%.

Như vậy Preparation có giá trị thực, nhưng không bảo đảm thành công.

Đây là ví dụ cân bằng cần chạy distribution test trước khi khóa.

<a id="m27-t073-s037"></a>

### 19. Outcome Tiers theo Margin

Đề xuất:

| Margin | Kết quả mặc định |
|---|---|
| Từ +6 trở lên | Thành công tốt, có thể có lợi ích bổ sung hợp lệ |
| 0 đến +5 | Thành công |
| -1 đến -3 | Không đạt đầy đủ; có thể có tiến triển một phần nếu action hỗ trợ |
| -4 đến -6 | Thất bại |
| -7 trở xuống | Thất bại nghiêm trọng trong phạm vi hậu quả đã khai báo |

Không phải mọi action đều có đủ năm outcome.

Một thao tác như kiểm tra cửa khóa có thể chỉ thành công hoặc thất bại.

Một công việc chế tác có thể cho phép chất lượng khác nhau.

Một Negotiation có thể dẫn tới thỏa thuận một phần hoặc phát sinh điều kiện mới.

<a id="m27-t073-s038"></a>

### 20. Không sử dụng Critical Success/Failure tự động cho mọi tình huống

Không nên quy định:

- Roll thấp nhất luôn thảm họa.
- Roll cao nhất luôn thành công.

Nếu một người không biết bơi lao vào thác nguy hiểm, roll cao không thể biến họ thành chuyên gia.

Nếu một Master sửa việc quá dễ trong điều kiện tốt, roll thấp không cần khiến workshop cháy.

Các event nghiêm trọng phải có preconditions và outcome mapping tương ứng.

Roll là một phần của resolution, không có quyền phá tính khả thi vật lý.

<a id="m27-t073-s039"></a>

### 21. Opposed Actions

Khi hai actor trực tiếp đối kháng, có hai hướng:

**Đối kháng bằng điểm:** Mỗi bên roll và so sánh điểm.

**Bên chủ động đối mặt Difficulty dẫn xuất:** Difficulty được tính từ khả năng và trạng thái bên phòng thủ.

Đề xuất chọn một phương pháp cho từng action family và giữ nhất quán.

Ví dụ:

Đối với Combat Attack, có thể dùng Attack Roll chống Defensive Threshold.

Đối với một số contest như vật lộn, có thể dùng opposed roll.

Cần tránh trường hợp cùng một Skill Check vừa phải vượt Difficulty cố định vừa phải thắng roll của đối thủ, khiến cơ chế bị phạt hai lần.

---

<a id="m27-t073-s040"></a>

## PHẦN VI — CHARACTER GENERATION FORMULAS

<a id="m27-t073-s041"></a>

### 22. Character Generation là constrained causal sampling

Không sinh toàn bộ thuộc tính độc lập.

Đề xuất pipeline:

**World Seed → Origin Region → Settlement → Household → Life Stage → Childhood Conditions → Life Event Sequence → Current Skills/Knowledge → Goals/Personality → Current State → Viability Validation.**

Các bước phía sau phụ thuộc điều kiện phía trước.

Một character sinh trong household làm thuyền có exposure với nghề sông.

Một character lớn lên giữa nghề đúc có xác suất tiếp xúc với lò và nguyên liệu cao hơn.

Nhưng điều đó không bắt buộc nghề tương lai.

<a id="m27-t073-s042"></a>

### 23. Origin Sampling

Xác suất một origin có thể dựa trên:

**Origin Weight = Population Availability × Household Eligibility × Scenario Diversity Modifier**

Cần phân biệt:

- Population Availability là đặc điểm world-canon.
- Household Eligibility bảo đảm có household hợp lệ.
- Diversity Modifier chỉ có thể ảnh hưởng **chọn góc nhìn khởi đầu**, không được thay đổi dân số thực của vùng.

Nếu muốn mọi vùng có cơ hội xuất hiện hợp lý trong New Game dù dân số khác nhau, có thể định nghĩa phân phối sampling dành riêng cho protagonist.

Nhưng không được dùng cùng phân phối ấy làm population statistics của World Simulation.

Đây là một ranh giới quan trọng.

<a id="m27-t073-s043"></a>

### 24. Attribute Generation

Mỗi character có:

- aptitude nền;
- ảnh hưởng thời thơ ấu;
- training;
- health;
- life-event modifiers;
- current functional state.

Đề xuất cấu trúc:

**Base Attribute = Clamp(Aptitude + Development + Permanent Life Effects, 1, 10)**

Các yếu tố:

**Aptitude:** Được sinh có điều kiện.

**Development:** Từ môi trường và những trải nghiệm kéo dài.

**Permanent Life Effects:** Những thay đổi lâu dài hợp lệ.

Không được dùng một công thức khiến mọi character lớn tuổi tự có tất cả Attribute cao.

Người lớn tuổi có thể có nhiều Knowledge và Skill, nhưng không có lợi thế tuyệt đối ở mọi mặt.

<a id="m27-t073-s044"></a>

### 25. Correlated Attributes

Nếu tất cả Attributes được random độc lập, nhân vật có thể có những tổ hợp thiếu tự nhiên hoặc khiến build phân tán quá cực đoan.

Đề xuất sử dụng một mô hình tương quan nhẹ:

- aptitude;
- upbringing;
- profession exposure;
- major life events.

Không cần gắn chặt nghề nghiệp với Attributes.

Ví dụ một thợ đúc giỏi có thể thiên XẢO hoặc TRÍ hoặc cả hai.

Một người dẫn thuyền giỏi có thể mạnh GIÁC hoặc TÂM tùy cách phát triển.

Cần nhiều archetype khả thi, không chỉ một công thức tối ưu.

<a id="m27-t073-s045"></a>

### 26. Skill History

Skill hiện tại phải được tạo từ lịch sử học tập.

Ví dụ:

**Skill Gain = Practice + Mentorship + Meaningful Challenges + Life Events − Relevant Losses**

Đây là cơ cấu nguyên nhân; công thức phát triển thực tế được mô tả ở phần sau.

Nếu generator sinh Skill 75 cho một nhân vật, Life History cần cung cấp được giải thích đủ mạnh như:

- làm nghề lâu năm;
- có mentor;
- từng xử lý công việc khó;
- có milestones;
- đã luyện tập liên tục.

Không phải lưu chi tiết từng ngày, nhưng cần có causal evidence.

<a id="m27-t073-s046"></a>

### 27. Bounded Generation

Không sử dụng:

```text
while character is invalid:
    generate randomly again
```

không có giới hạn.

Đề xuất:

1. Xác định tập ràng buộc hợp lệ trước.
2. Sinh theo thứ tự có điều kiện.
3. Kiểm tra các điều kiện tại từng bước.
4. Sử dụng bounded retry đối với lựa chọn mềm.
5. Nếu không có cấu hình hợp lệ, trả Diagnostic Error.
6. Có fallback được định nghĩa trước nhưng không vi phạm luật.

Đối với nhóm thuộc tính nhỏ, có thể dùng constraint solver hoặc bảng phân phối có điều kiện thay vì retry mù.

<a id="m27-t073-s047"></a>

### 28. Quality Gates

Generator phải qua test:

- không có Skill xuất hiện không có exposure hợp lệ;
- tuổi và Life History tương thích;
- character có quan hệ gia đình nhất quán;
- không sinh parent trẻ hơn con ở trường hợp thông thường;
- Character Viability đạt;
- các origin không bị lệch một cách ngoài chủ đích;
- không tự động tối ưu tất cả protagonist thành build mạnh.

---

<a id="m27-t073-s048"></a>

## PHẦN VII — SKILL GROWTH & MASTERY

<a id="m27-t073-s049"></a>

### 29. Không có XP từ giết địch

GDD đã khóa:

- Skill phát triển qua sử dụng có ý nghĩa.
- Profession phát triển qua hành nghề.
- Class hình thành từ hành động và identity.
- Không sử dụng kill XP như progression phổ quát.

Đề xuất mỗi meaningful activity tạo **Practice Evidence**.

<a id="m27-t073-s050"></a>

### 30. Practice Evidence

```text
PracticeRecord
    CharacterID
    SkillID
    ActivityType
    Difficulty
    Outcome
    Duration
    Mentorship
    Novelty
    EvidenceQuality
    OriginEventID
```

Không cần ghi từng lần gõ búa.

Có thể tổng hợp practice trong một khoảng công việc phù hợp.

<a id="m27-t073-s051"></a>

### 31. Skill Growth Formula

Đề xuất:

**Progress Gain = BasePractice × ChallengeFactor × LearningCondition × NoveltyFactor × DiminishingReturn**

Trong đó:

**BasePractice:** Khối lượng luyện tập có ý nghĩa.

**ChallengeFactor:** Công việc có phù hợp trình độ hay không.

**LearningCondition:** Mentor, feedback, công cụ và sức khỏe.

**NoveltyFactor:** Mức độ mới của trải nghiệm.

**DiminishingReturn:** Giảm tốc độ phát triển khi Skill đã cao.

Để tránh lạm phát Skill, sử dụng một hàm lợi ích giảm dần.

Ví dụ:

`DiminishingReturn = 1 − 0.75 × (Skill / 100)`

Với Skill = 20, hệ số khoảng 0,85.

Với Skill = 80, hệ số khoảng 0,40.

Đây chỉ là đường cong prototype.

Không có nghĩa Mastery 80 trở lên phải chậm đúng theo hệ số đó ở bản cuối.

<a id="m27-t073-s052"></a>

### 32. Challenge Factor

Đề xuất nguyên tắc:

- Việc quá dễ, lặp lại: tiến triển rất thấp.
- Việc phù hợp năng lực: tiến triển tốt.
- Việc khó vừa phải: tiến triển cao nếu có feedback và cơ hội học.
- Việc vượt xa khả năng, không hiểu lỗi: tiến triển thấp hoặc không có.

Không nên cho điểm luyện tập cực cao chỉ vì player cố làm một công việc bất khả thi nhiều lần.

<a id="m27-t073-s053"></a>

### 33. Mentor Effect

Mentor không đơn giản là buff +50% XP.

Mentor có thể:

- giúp chọn bài tập phù hợp;
- phát hiện lỗi;
- cung cấp Technique;
- mở access tới tool/workshop;
- giải thích các kết quả thất bại;
- tạo quan hệ nghề nghiệp;
- xác nhận competence.

Một mentor rất giỏi nhưng không muốn dạy có thể không mang lại lợi ích.

Một mentor ít danh tiếng nhưng rất phù hợp với nhu cầu người học có thể mang lại kết quả tốt.

<a id="m27-t073-s054"></a>

### 34. Anti-Farming

Cần một cơ chế nhận diện repetition.

Ví dụ:

Player liên tục sửa cùng một lỗi đơn giản ở thuyền.

Sau khi đã thành thạo:

- hoạt động có thể được abstract;
- Skill Growth giảm mạnh;
- vẫn tạo giá trị nghề nghiệp hoặc thu nhập nếu có công việc thật;
- không tạo vô hạn milestones.

Có thể theo dõi `RecentPracticeSignature` cho mỗi Skill/Activity Family.

Không cần phạt player vì yêu thích nghề.

Chỉ cần tránh việc một thao tác vô nghĩa tạo progression vô hạn.

---

<a id="m27-t073-s055"></a>

## PHẦN VIII — CLASS EMERGENCE

<a id="m27-t073-s056"></a>

### 35. Class không phải phép tính Skill Threshold

Một Class như Liệp Nhân, Thủy Binh, Dược Nhân hoặc Sứ Giả cần được hình thành từ:

- Skills;
- hành vi;
- trách nhiệm;
- identity;
- những milestone;
- sự công nhận hoặc bằng chứng xã hội phù hợp.

Đề xuất dùng **Evidence-Based State Machine**.

<a id="m27-t073-s057"></a>

### 36. Class Evidence Score

Có thể tính một chỉ số nội bộ:

**Class Evidence = Competence Evidence + Behavioral Consistency + Meaningful Milestones + Recognition**

Nhưng mỗi thành phần phải là evidence thực tế, không được chỉ là điểm abstract tăng theo thời gian.

Ví dụ Thủy Binh:

**Competence:** Dẫn thuyền, định hướng, xử lý nước.

**Behavior:** Thường xuyên nhận vai trò trong hoạt động trên sông.

**Milestones:** Dẫn đoàn vượt đoạn nguy hiểm hoặc bảo vệ vận tải.

**Recognition:** Được những người có liên quan tin tưởng, giao trách nhiệm hoặc công nhận.

Một character có thể có đủ kỹ năng nhưng chưa được công nhận rộng rãi.

Một character khác có uy tín nhưng thiếu năng lực thực tế.

Hai trường hợp phải được xử lý khác nhau.

<a id="m27-t073-s058"></a>

### 37. State Machine

```text
UNFORMED
    |
    v
CANDIDATE
    |
    v
EMERGING
    |
    v
RECOGNIZED
    |
    v
ESTABLISHED
    |
    v
MASTERED
```

Không bắt buộc tuyến tính tuyệt đối trong mọi trường hợp.

Một character có thể suy giảm vai trò hoạt động nhưng không mất identity.

`MASTERED` không phải trạng thái tự mở chỉ vì tuổi cao.

<a id="m27-t073-s059"></a>

### 38. Multi-Class

Một character có thể thể hiện nhiều Class Identity.

Nhưng không nên cộng toàn bộ Technique của mọi Class để tạo build vượt trội vô hạn.

Cần phân biệt:

- identity;
- learned technique;
- active capability;
- compatible equipment;
- action cost;
- practical constraints.

Một người biết nghề chữa bệnh và chiến đấu vẫn chỉ có lượng thời gian, sức lực và hành động tương ứng.

Không cần hard-lock multi-class, nhưng phải tránh stack các modifier làm phá Action Resolution.

---

<a id="m27-t073-s060"></a>

## PHẦN IX — NPC DECISION & UTILITY AI

<a id="m27-t073-s061"></a>

### 39. NPC Decision Pipeline

NPC không random một hành động từ danh sách toàn thế giới.

Quy trình:

1. Thu thập Perceived State.
2. Cập nhật Needs/Goals.
3. Tạo action candidates hợp lệ.
4. Loại các lựa chọn vi phạm hard constraints.
5. Ước lượng lợi ích và rủi ro theo quan điểm NPC.
6. Đánh giá obligations và identity.
7. Xếp hạng Utility.
8. Chọn hành động.
9. Tạo Command.
10. Để Rule Engine xử lý kết quả thật.

NPC có thể chọn một hành động dựa trên nhận định sai.

Nhưng code phải xử lý hậu quả vật lý theo World Truth.

<a id="m27-t073-s062"></a>

### 40. Utility Components

Đề xuất:

**Utility = Goal Fit + Need Satisfaction + Social Alignment + Professional Alignment + ExpectedBenefit − ExpectedCost − PerceivedRisk − ObligationConflict**

Để các nhóm không có số lượng biến khác nhau gây lệch, mọi consideration cần được chuẩn hóa về thang 0–1.000.

Các trọng số phải được kiểm soát theo Action Family.

Không nên cộng 25 consideration cho nghề vận tải nhưng chỉ 5 consideration cho nghỉ ngơi rồi kết luận vận tải luôn tốt hơn.

<a id="m27-t073-s063"></a>

### 41. Một công thức Utility tham chiếu

Đề xuất:

`U(action) = Clamp(Σ(wᵢ × qᵢ) / Σwᵢ + CommitmentAdjustment, 0, 1000)`

Trong đó:

- `qᵢ` là consideration đã chuẩn hóa.
- `wᵢ` là trọng số.
- `CommitmentAdjustment` phản ánh mức độ ổn định và nghĩa vụ hiện tại.

Các yếu tố mang tính cấm tuyệt đối được xử lý ở bước eligibility, không dùng trọng số.

Ví dụ:

Một companion không đồng ý giết người đầu hàng.

Không được để ExpectedBenefit cao khiến Utility Score vượt Moral Boundary.

<a id="m27-t073-s064"></a>

### 42. Hysteresis & Commitment

Nếu NPC luôn chọn hành động có Utility lớn hơn 1 điểm, họ có thể đổi ý liên tục.

Ví dụ:

Buổi sáng đi làm.

Sau một tick muốn nghỉ.

Sau tick nữa lại muốn làm.

Đề xuất:

- Commitment Duration;
- Switching Cost;
- Action Stability;
- Replanning Triggers.

Một NPC chỉ nên bỏ một hoạt động đang thực hiện khi:

- hoạt động đã hoàn tất;
- điều kiện thay đổi đáng kể;
- có emergency;
- xuất hiện nghĩa vụ ưu tiên;
- hoặc lợi ích chuyển đổi đủ lớn.

Không cố định cùng một ngưỡng cho mọi nghề.

Một người đang vận chuyển hàng không thể quay về giữa đường miễn phí như người đang làm việc trong nhà.

<a id="m27-t073-s065"></a>

### 43. Ví dụ Utility NPC

Một thợ đúc có bốn lựa chọn:

| Hành động | Utility sau khi đánh giá | Ghi chú |
|---|---:|---|
| Tiếp tục đơn hàng | 720 | Thu nhập và nghĩa vụ |
| Đi tìm nguồn than mới | 810 | Workshop sắp thiếu nhiên liệu |
| Nghỉ một ngày | 430 | Sức khỏe hiện tương đối ổn |
| Tham gia tranh chấp faction | 350 | Không phù hợp mục tiêu cá nhân |

Giả sử việc tìm than cần chuẩn bị và có thể hoàn thành trong ngày.

NPC chọn đi tìm than.

Nhưng nếu họ đang có hợp đồng giao hàng buộc phải hoàn thành ngay, Obligation/Commitment có thể khiến tiếp tục đơn hàng được ưu tiên.

Nếu than không tồn tại ở bất kỳ nguồn khả thi nào mà NPC biết, hành động tìm nguồn than cụ thể có thể không đủ eligibility.

<a id="m27-t073-s066"></a>

### 44. Deterministic Variability

Không cần mọi NPC luôn chọn đúng action có Utility cao nhất tuyệt đối.

Có thể sử dụng selection trong nhóm những hành động gần tối ưu.

Ví dụ:

Nếu các action đều hợp lệ và Utility cách nhau rất ít, dùng weighted choice có seed để tạo tính đa dạng.

Nhưng không để một lựa chọn Utility 50 vượt lựa chọn 950 chỉ vì random.

Cần một **Near-Optimal Candidate Window** hoặc chính sách có trọng số giới hạn.

Các ngưỡng là tham số tuning.

<a id="m27-t073-s067"></a>

### 45. NPC Perceived Risk

NPC không biết xác suất thực của mọi nguy hiểm.

Một Liệp Nhân quen rừng có thể đánh giá lối đi tương đối đúng.

Người mới tới có thể xem nhẹ nguy hiểm.

Một NPC đã từng bị cướp có thể quá thận trọng.

Utility dùng **perceived risk**, còn Event Resolution dùng **actual risk**.

Đây là nền của những quyết định không hoàn hảo nhưng nhất quán với tính cách.

---

<a id="m27-t073-s068"></a>

## PHẦN X — SOCIAL INTERACTION & NEGOTIATION FORMULAS

<a id="m27-t073-s069"></a>

### 46. Không dùng Persuasion để ghi đè ý chí

Một lời đề nghị được đánh giá qua:

**Feasibility → Authority → Moral Boundary → Actor Need → Agreement Value → Relationship Context → Social Resolution.**

Skill Thuyết Phục chỉ là một phần.

Ví dụ player yêu cầu một người bán toàn bộ lương thực dự trữ của gia đình.

Dù rất giỏi nói chuyện, player có thể không thể thuyết phục nếu đối phương cần lương thực để sống và không có phương án thay thế.

<a id="m27-t073-s070"></a>

### 47. Reservation Value

Mỗi bên thương lượng có thể có:

- mục tiêu;
- mức tối thiểu chấp nhận;
- rủi ro;
- phương án thay thế;
- nghĩa vụ;
- lợi ích phi vật chất.

Đề xuất đánh giá:

**Perceived Deal Value = MaterialBenefit + RelationshipBenefit + GoalAlignment − MaterialCost − Risk − ObligationCost**

Nếu một đề nghị thấp hơn ngưỡng chấp nhận của actor, không tự cho phép roll biến nó thành hợp đồng.

Player có thể thay đổi đề nghị, thông tin hoặc leverage.

<a id="m27-t073-s071"></a>

### 48. Social Skill có giá trị ở đâu?

Năng lực xã hội có thể giúp:

- trình bày lợi ích rõ hơn;
- phát hiện điểm bất đồng;
- đánh giá tính hợp lệ của phương án;
- giảm misunderstanding;
- chọn cách tiếp cận phù hợp;
- thuyết phục actor xem xét một option có lợi thật nhưng chưa được nhận ra.

Không cho Skill tạo ra hàng hóa, quyền lực hay cảm xúc không có căn cứ.

<a id="m27-t073-s072"></a>

### 49. Relationship State

Một quan hệ nên có nhiều chiều riêng:

- Trust;
- Respect;
- Affection;
- Fear;
- Obligation;
- Grievance;
- SharedHistory.

Các chỉ số nội bộ có thể dùng một thang chuẩn hóa, nhưng UI không phải hiển thị tất cả.

Tác động của một event phải có **interpretation**.

Ví dụ player phá cửa kho để cứu người.

Người được cứu tăng Gratitude.

Chủ kho có thể tăng Grievance.

Một người giữ luật tục có thể đánh giá hành động khác với người làm nghề cứu nạn.

Không áp một `ReputationChange = +10` cho mọi actor.

<a id="m27-t073-s073"></a>

### 50. Promise Resolution

Một lời hứa được xem xét theo:

- terms;
- deadline;
- responsible party;
- fulfillment evidence;
- force majeure;
- transfer terms;
- actor interpretation.

Nếu hoàn thành muộn, có thể tồn tại nhiều trạng thái:

- được tha thứ;
- bị xem là vi phạm;
- được thương lượng lại;
- hoàn thành một phần.

Không có một quy tắc toàn cầu rằng cứ quá deadline là mọi NPC giảm Trust bằng nhau.

<a id="m27-t073-s074"></a>

### 51. Social Test

Các test cần gồm:

- cùng một đề nghị nhưng actor có nhu cầu khác nhau;
- nhiều người cùng tham gia negotiation;
- người ký không có authority;
- lời hứa được truyền lại sau khi người hứa chết;
- actor hiểu sai điều khoản;
- player nói thật nhưng không được tin;
- player nói dối nhưng chưa bị phát hiện;
- companion từ chối hành động trái ranh giới.

---

<a id="m27-t073-s075"></a>

## PHẦN XI — KNOWLEDGE, PERCEPTION & RUMOR

<a id="m27-t073-s076"></a>

### 52. Detection và Interpretation là hai bước khác nhau

Một nhân vật thấy vết chân không đồng nghĩa biết ai đã đi qua.

Đề xuất:

**Perception Result → Observation Record → Interpretation Candidate → Belief Update.**

Perception kiểm tra:

- giác quan;
- khoảng cách;
- môi trường;
- skill;
- thời gian quan sát.

Interpretation kiểm tra:

- kiến thức;
- kinh nghiệm;
- nguồn so sánh;
- bằng chứng khác.

Ví dụ:

Một Liệp Nhân nhận ra dấu chân mới.

Một người khác chỉ thấy đất bị xáo trộn.

Không có ai tự xác định hung thủ nếu thiếu bằng chứng.

<a id="m27-t073-s077"></a>

### 53. Perception Formula

Có thể sử dụng Action Resolution chung:

**Detection Score = 2d10 + GIÁC Bonus + Relevant Skill + Visibility + Preparation − Obscurity**

Hoặc deterministic threshold đối với những dấu hiệu quá rõ.

Không nên roll liên tục mỗi frame.

Detection được đánh giá khi:

- actor chủ động quan sát;
- môi trường thay đổi;
- một đối tượng tạo tín hiệu mới;
- khoảng cách hoặc điều kiện quan sát thay đổi đáng kể.

Điều này tránh exploit đứng chờ cho tới khi roll thành công.

<a id="m27-t073-s078"></a>

### 54. Belief Update

Một actor có thể đánh giá độ tin cậy của claim dựa trên:

- độ tin cậy của nguồn;
- tính nhất quán với điều đã biết;
- bằng chứng trực tiếp;
- khoảng cách thời gian;
- bias cá nhân;
- nguồn khác có thực sự độc lập.

Không nên chỉ cộng:

`Belief +10 mỗi lần nghe tin`.

Nếu năm người lặp lại một tin xuất phát từ cùng một người, không được mặc định coi là năm bằng chứng độc lập.

Cần provenance graph.

<a id="m27-t073-s079"></a>

### 55. Rumor Transmission

Một transmission cần đủ điều kiện:

- sender có claim;
- receiver có cơ hội nghe;
- sender muốn truyền;
- kênh thông tin phù hợp;
- thời gian đủ;
- nội dung không vượt giới hạn truyền đạt.

Đề xuất probability:

**P(transmit) = Clamp(Base × SocialContact × Motivation × Salience × Opportunity, 0, 1)**

Các hệ số có thể ở thang fixed-point.

Đây là xác suất một tin được truyền, không phải xác suất tin đúng.

<a id="m27-t073-s080"></a>

### 56. Rumor Distortion

Biến dạng có thể tác động:

- chi tiết phụ;
- tên riêng;
- số lượng ước lượng;
- mức độ chắc chắn;
- cách gán nguyên nhân;
- cách đánh giá đạo đức.

Nhưng mọi variant cần liên kết Origin Claim.

Không được tạo một event mới chỉ vì rumor biến thành câu chuyện kỳ lạ.

Một rumor:

> “Ba người cứu đoàn thuyền.”

có thể biến thành:

> “Người Bến Vạc một mình vượt lũ cứu cả đoàn.”

Đó là một claim mới với nguồn gốc từ claim cũ.

Không phải World Truth bị thay đổi.

<a id="m27-t073-s081"></a>

### 57. Knowledge Decay

Không nên cho mọi thông tin mất đi theo cùng một hàm thời gian.

**Stable Knowledge:** Kỹ thuật đã thành thạo hoặc một địa điểm lớn thường lưu lâu.

**Volatile Knowledge:** Giá hàng, checkpoint, tình trạng route nhanh cũ.

**Personal Memory:** Có thể suy yếu hoặc thay đổi interpretation.

**Recorded Knowledge:** Phụ thuộc vật mang, khả năng đọc và việc bảo quản.

Nên tập trung vào Freshness và Recall Reliability theo loại thông tin.

Ví dụ:

Tin về tuyến đường từ ba năm trước có thể vẫn chỉ ra vị trí tương đối, nhưng không đủ xác nhận tuyến còn đi được.

<a id="m27-t073-s082"></a>

### 58. Test Knowledge Leakage

Tạo một World Event bí mật mà không NPC nào trong mạng player biết.

Kiểm tra:

- NPC không dùng event để lập kế hoạch.
- Journal không hiện.
- Map không cập nhật.
- Dialogue không nhắc.
- World Director không phơi bày.
- Search/Codex không gợi ý.

Sau khi một witness truyền tin, chỉ những actor nhận được thông tin hợp lệ mới thay đổi belief.

---

<a id="m27-t073-s083"></a>

## PHẦN XII — EVENT ELIGIBILITY & PROCEDURAL GENERATION

<a id="m27-t073-s084"></a>

### 59. Event có nhiều tầng khả năng

Không được nhầm:

**Eligible:** Có điều kiện xảy ra.

**Selected:** Được actor hoặc scheduler lựa chọn để thử thực hiện.

**Committed:** Đã xảy ra và thay World State.

**Discovered:** Player đã biết.

**Intervened:** Player đã can thiệp.

Đây là những trạng thái khác nhau.

Một event có thể xảy ra và kết thúc mà player chưa bao giờ biết.

<a id="m27-t073-s085"></a>

### 60. Event Trigger

Đề xuất một trigger gồm:

```text
PressureType
Scope
ThresholdOrCondition
TimeWindow
ActorRequirements
ResourceRequirements
HistoryRequirements
```

Ví dụ:

`FoodShortagePressure`

được kích hoạt khi mức food khả dụng thấp hơn nhu cầu dự kiến trong một giai đoạn nhất định.

Nhưng việc thiếu food không tự động tạo một cuộc cướp.

Nó có thể tạo:

- tìm nguồn hàng;
- giảm tiêu thụ;
- tranh chấp;
- di cư;
- tăng giá;
- yêu cầu hỗ trợ.

Lựa chọn phụ thuộc actor và điều kiện.

<a id="m27-t073-s086"></a>

### 61. Pressure Score

Đề xuất một chỉ số áp lực theo domain:

**Pressure = Severity × Exposure × Persistence × LackOfAlternatives**

Các yếu tố được chuẩn hóa.

Nhưng cần tránh dùng Pressure Score thay thế toàn bộ World State.

Ví dụ hai settlement cùng FoodPressure 800 có thể có nguyên nhân khác:

- một nơi mất mùa;
- một nơi tuyến hàng bị cắt;
- một nơi bị trưng thu.

Các Event Templates phải nhìn cả `PressureType` và causal source.

<a id="m27-t073-s087"></a>

### 62. Candidate Generation

Pipeline:

```text
Collect Pressure Events
    |
    v
Find Templates by Tags
    |
    v
Bind Location & Actor Roles
    |
    v
Validate Hard Constraints
    |
    v
Check Historical/Temporal Constraints
    |
    v
Build Eligible Candidates
    |
    v
Select via Actor Goals / Scheduler / RNG
    |
    v
Create Event Instance
```

Không cho LLM sinh nội dung mới tại đây.

<a id="m27-t073-s088"></a>

### 63. Weighted Selection

Một event đủ điều kiện có thể có trọng số:

**Event Weight = BaseWeight × PressureFit × SeasonFit × ActorMotivation × RecentHistoryAdjustment**

Những yếu tố:

- 0 nếu không phù hợp.
- Dương nếu phù hợp.
- Bị giới hạn trong miền khai báo.

`RecentHistoryAdjustment` chỉ dùng để giảm lặp ở optional events, không chặn mandatory consequence.

<a id="m27-t073-s089"></a>

### 64. Chống event loop

Event cần có:

- OriginEventID;
- CausationID;
- correlation;
- deduplication key;
- cooldown hoặc recurrence rule khi phù hợp.

Ví dụ:

Một rumor gây tranh chấp.

Tranh chấp khiến người khác nói chuyện.

Không được tạo vô hạn bản sao của cùng event trong một ngày.

Nên sử dụng **event occurrence identity** và theo dõi trạng thái xử lý của cùng một causal chain.

<a id="m27-t073-s090"></a>

### 65. Event không có player

Một Event Template phải có cách xử lý khi player không can thiệp nếu nó mô tả vấn đề độc lập của world.

Ví dụ:

Một nhà kho thiếu nguồn lực.

NPC có thể:

- điều phối lại;
- mua thêm;
- vay;
- đóng cửa;
- không giải quyết được.

Không phải kho cứ chờ player đến rồi bắt đầu thiếu food.

<a id="m27-t073-s091"></a>

### 66. Fail-Forward khác bảo đảm thành công

Một quest có thể thất bại thật.

Fail-Forward có nghĩa câu chuyện và thế giới vẫn tiếp tục có hậu quả.

Ví dụ:

Không cứu được người mất tích.

Người ấy có thể chết hoặc rời khỏi vùng, tùy event thật.

Story Thread có thể chuyển thành:

- tìm nguyên nhân;
- xử lý tranh chấp;
- chăm sóc người thân;
- điều tra lời kể.

Không cần tạo phần thưởng an ủi hoặc giữ NPC sống vô điều kiện.

---

<a id="m27-t073-s092"></a>

## PHẦN XIII — STORY THREAD, QUEST & WORLD DIRECTOR

<a id="m27-t073-s093"></a>

### 67. Story Thread State Transition

Một Story Thread đọc domain events và cập nhật:

- active pressures;
- known actors;
- available interventions;
- clocks;
- resolution state;
- aftermath.

Không tự tạo Resource hoặc Character State.

Một thread có thể:

**Latent → Active → Escalating → Resolved/Transformed → Aftermath → Historical Memory.**

Các trạng thái phụ thuộc causal events.

<a id="m27-t073-s094"></a>

### 68. Quest Objective Derivation

Quest Objective không phải lệnh cứng:

> Giết năm người.

Nó có thể là:

**Desired State:** Đảm bảo tuyến thuyền hoạt động.

Các cách đạt:

- sửa thuyền;
- thuê thuyền khác;
- giải quyết tranh chấp;
- mở route thay thế;
- giảm khối lượng hàng.

Mỗi phương án dùng rule thật.

Quest Engine chỉ theo dõi World State và những cam kết của player.

<a id="m27-t073-s095"></a>

### 69. Dynamic Objective

Một objective có thể mất khả năng thực hiện.

Ví dụ người cần gặp chết.

Quest không được crash hoặc giữ objective bất khả thi.

Engine đánh giá:

- mục tiêu còn ý nghĩa không;
- có cách thay thế không;
- thread đã thất bại hay biến đổi;
- còn aftermath nào cần hiển thị.

Các câu trả lời dựa trên template và state transitions đã khai báo.

<a id="m27-t073-s096"></a>

### 70. World Director Scoring

World Director chỉ xếp hạng cơ hội hiển thị.

Đề xuất:

**Exposure Score = Relevance + InterventionValue + Novelty + PacingFit + KnowledgeAccess**

Trong đó:

- Relevance: Liên hệ với player, nghề, quan hệ.
- InterventionValue: Player hiện có khả năng tác động.
- Novelty: Tránh lặp presentation không cần thiết.
- PacingFit: Nhịp trải nghiệm hiện tại.
- KnowledgeAccess: Có kênh truyền thông tin hợp lệ.

**KnowledgeAccess phải là hard gate khi không có nguồn thông tin.**

Không được đặt nó chỉ là một penalty có thể bị điểm Relevance bù lại.

<a id="m27-t073-s097"></a>

### 71. Director không tạo crisis để giữ nhịp

Nếu player sống một giai đoạn yên bình, Director không được tăng xác suất một tai nạn chết người chỉ để tạo drama.

Director có thể:

- ưu tiên những opportunity đã tồn tại;
- đề nghị activity phù hợp;
- làm rõ những thông tin character biết;
- giới thiệu một sự kiện tùy chọn đủ điều kiện.

Nếu không có sự kiện mới thích hợp, kết quả hợp lệ là **không tạo thêm sự kiện**.

<a id="m27-t073-s098"></a>

### 72. Pacing Vector

Đề xuất các chiều:

- Physical;
- Emotional;
- Cognitive;
- Social;
- Resource;
- Historical.

Mỗi chiều theo thang 0–1.000.

Không dùng một `Tension` duy nhất rồi mặc định sau combat cần hài hước.

Director chỉ sử dụng vector để điều chỉnh presentation và optional exposure.

Các mandatory events vẫn diễn ra.

---

<a id="m27-t073-s099"></a>

## PHẦN XIV — COMBAT FORMULAS

<a id="m27-t073-s100"></a>

### 73. Combat giữ bốn trụ cột

Combat đã khóa:

**Information — Position — Condition — Consequence.**

Mọi công thức phải phục vụ bốn yếu tố này.

Không xây một hệ thống chỉ dựa trên Attack Power và HP.

<a id="m27-t073-s101"></a>

### 74. Combat Turn

Một actor có:

- 1 Major Action;
- 1 Minor Action;
- Reaction theo điều kiện và giới hạn;
- trạng thái Stamina;
- tactical position.

Không cho phép các Technique liên tục tạo thêm Major Action theo cách gây vòng lặp vô hạn.

Nếu một Technique cho thêm hành động, phải có giới hạn rõ ràng và được kiểm thử.

<a id="m27-t073-s102"></a>

### 75. Attack Resolution

Đề xuất:

**Attack Score = 2d10 + Weapon Skill Bonus + Attribute Bonus + Technique + Position + Preparation − Condition Penalty**

So sánh với:

**Defense Threshold = Base Defense + Guard + Cover + Positional Advantage + Defensive Technique**

Ví dụ:

Attacker Skill = 60 → +6.

Attribute Bonus = +1.

Weapon Position Bonus = +2.

Condition Penalty = 1.

Tổng modifier = +8.

Defender có Defense Threshold 19.

Attacker cần 2d10 ít nhất 11, tương ứng xác suất 55% nếu không có modifier khác.

Đây là ví dụ minh họa.

Defense Threshold cụ thể cần được cân chỉnh dựa trên năng lực phòng thủ và tactical system, không nên lấy một số cố định cho mọi đối thủ.

<a id="m27-t073-s103"></a>

### 76. Hit không đồng nghĩa Wound nghiêm trọng

Khi hit thành công:

1. Xác định vùng hoặc loại va chạm.
2. Xác định tác động vũ khí.
3. Xác định Armor/Shield interaction.
4. Xác định mức năng lượng/tác động xuyên hoặc va đập.
5. Tính Injury Severity.
6. Tạo Wound/Status nếu đủ điều kiện.

Một cú đánh trúng khiên có thể làm người phòng thủ mất thăng bằng mà không tạo vết thương.

Một cú giáo trúng phần không được che chắn có thể nguy hiểm dù Vitality còn cao.

<a id="m27-t073-s104"></a>

### 77. Impact Model

Đề xuất tách các thuộc tính:

**Impact:** Tác động cơ học.

**Penetration:** Khả năng vượt qua lớp bảo vệ.

**Control:** Khả năng giữ khoảng cách hoặc làm mất vị trí.

**Reach:** Lợi thế khoảng cách.

**Recovery:** Thời gian và khả năng tiếp tục hành động.

Một vũ khí không nên tốt hơn ở tất cả thuộc tính chỉ vì thuộc nhóm hiếm.

Ví dụ:

Giáo dài có lợi thế reach nhưng bất lợi ở không gian quá hẹp.

Rìu có impact tốt nhưng recovery khác giáo.

Khiên có guard và control nhưng làm thay đổi cách sử dụng các vũ khí khác.

<a id="m27-t073-s105"></a>

### 78. Wound Severity

Một công thức tham chiếu:

**Wound Pressure = EffectiveImpact + Exposure + HitQuality − ArmorMitigation − TargetProtection**

Các kết quả có thể tạo:

- No Wound;
- Minor;
- Moderate;
- Serious;
- Critical.

Không nên gán trực tiếp damage number thành HP loss rồi bỏ qua vị trí và giáp.

Để giữ game đủ dễ tiếp cận, có thể tách các thông số vật lý thành band thay vì mô phỏng y học và cơ học thực tế quá chi tiết.

<a id="m27-t073-s106"></a>

### 79. Injury Consequences

Ví dụ vết thương chân:

- giảm khả năng di chuyển;
- tăng khó khăn trong một số Position Change;
- ảnh hưởng Travel;
- ảnh hưởng nghề cần đi lại;
- cần thời gian hồi phục.

Combat không được sở hữu một debuff riêng mất hiệu lực khi encounter kết thúc.

Health Domain lưu trạng thái.

<a id="m27-t073-s107"></a>

### 80. Morale

Morale có thể phụ thuộc:

**Morale State = Leadership + GroupCohesion + PerceivedAdvantage + Safety − CasualtyShock − Fear − Exhaustion**

Không nhất thiết mỗi actor có một thanh Morale.

Một số NPC có thể chiến đấu vì bảo vệ người thân dù rất sợ.

Một nhóm lính có kỷ luật có thể giữ vị trí lâu hơn nhóm không được huấn luyện.

Morale Decision nên tác động:

- giữ vị trí;
- lui;
- bỏ chạy;
- đầu hàng;
- thương lượng;
- tiếp tục chiến đấu.

Không được biến Morale = 0 thành tất cả tự động chết hoặc biến mất.

<a id="m27-t073-s108"></a>

### 81. Combat Objective & Outcome

Một encounter cần xác định mục tiêu.

Ví dụ:

**Rescue Objective:** Đưa người bị thương tới vùng an toàn.

**Delay Objective:** Giữ cầu trong một số lượt.

**Capture Objective:** Bắt sống một người.

**Escape Objective:** Rời khỏi vùng nguy hiểm.

**Defense Objective:** Không để đối phương tiếp cận một khu vực.

Kết quả phải đọc objective state, không chỉ đọc số enemy đã chết.

<a id="m27-t073-s109"></a>

### 82. Combat Quality Tests

- Tấn công từ vị trí không hợp lệ bị từ chối.
- Armor Coverage áp dụng đúng vùng.
- Wound tiếp tục tồn tại ngoài combat.
- Retreat hợp lệ không bị coi là failure hệ thống.
- Companion có thể từ chối lệnh trái Moral Boundary.
- Không tạo Major Action vô hạn.
- Không có combat advantage từ hidden enemy state mà actor không biết.
- Same seed và commands tạo cùng combat outcome.

---

<a id="m27-t073-s110"></a>

## PHẦN XV — CRAFTING & EQUIPMENT FORMULAS

<a id="m27-t073-s111"></a>

### 83. Crafting không chỉ là Skill Roll

Một sản phẩm phụ thuộc:

**Design + Material + Tool + Labor + Technique + Time + Environment.**

Nếu thiếu điều kiện thiết yếu, hành động không hợp lệ.

Ví dụ:

Không thể đúc một món đồ cần lò nếu không có cơ sở nhiệt phù hợp.

Skill rất cao không làm vật liệu xuất hiện.

<a id="m27-t073-s112"></a>

### 84. Craft Quality Formula

Đề xuất:

**Craft Potential = DesignFit + MaterialQuality + ToolCapability + WorkerMastery + ProcessControl − DefectPressure**

Sau đó resolution xác định quality band.

Các thành phần cần dùng thang phù hợp để tránh cộng trực tiếp các đại lượng khác đơn vị.

Có thể dùng normalized score 0–1.000 cho từng consideration.

<a id="m27-t073-s113"></a>

### 85. Deterministic Routine Craft

Khi:

- thiết kế đã quen;
- Skill đủ cao;
- vật liệu ổn định;
- tool tốt;
- không có áp lực đặc biệt,

việc chế tạo có thể dùng deterministic output range hoặc fixed quality.

Không bắt thợ Master roll may rủi với mỗi chiếc đinh.

Random nên dùng cho:

- thử nghiệm;
- vật liệu không ổn định;
- thiết kế khó;
- điều kiện sản xuất bất thường;
- sản phẩm có chất lượng quan trọng.

<a id="m27-t073-s114"></a>

### 86. Defect Model

Defects có thể thuộc:

- Material Defect;
- Processing Defect;
- Design Defect;
- Environmental Defect;
- Maintenance Defect.

Một sản phẩm thất bại có thể còn dùng để:

- sửa;
- tái chế;
- học kỹ thuật;
- làm vật liệu phụ.

Không nên tất cả failures đều phá hủy hoàn toàn vật liệu.

Nhưng cũng không nên cho salvage hoàn toàn để player lặp sản xuất vô hạn không mất chi phí.

<a id="m27-t073-s115"></a>

### 87. Tool Wear & Repair

Tool Condition suy giảm theo:

- activity;
- material hardness;
- load;
- maintenance;
- environment.

Không cần hao mòn mỗi thao tác nhỏ theo tỷ lệ cố định nếu gây tedium.

Có thể dùng aggregate wear trong activity/downtime.

Repair tiêu tốn resource và thời gian, có thể có giới hạn phục hồi tùy loại hỏng.

Không để một item bị sửa vô hạn bằng một lượng vật liệu không đáng kể thành item hoàn hảo.

<a id="m27-t073-s116"></a>

### 88. Professional Crafting Test

Một thợ giỏi phải:

- xử lý routine hiệu quả;
- có lợi thế với những công việc phù hợp;
- vẫn phụ thuộc điều kiện sản xuất;
- có thể học và cải tiến;
- không tự động làm ra vật phẩm hoàn hảo;
- không tạo resource vô hạn;
- có thể truyền Technique cho người học đủ điều kiện.

---

<a id="m27-t073-s117"></a>

## PHẦN XVI — TRAVEL, SURVIVAL & ECOLOGY

<a id="m27-t073-s118"></a>

### 89. Travel Time Formula

Travel phải dựa trên Route Graph.

Đề xuất:

**Estimated Travel Time = BaseRouteTime × TerrainModifier × SeasonalModifier × LoadModifier × TransportCondition × PartyCondition**

Phải đảm bảo các modifier không làm thời gian trở thành 0 hoặc âm.

Một số điều kiện nên là hard block:

- cầu đã bị phá;
- sông không thể đi bằng phương tiện hiện có;
- cửa bị khóa và không có phương án mở;
- vùng đã bị kiểm soát và party không có quyền hoặc phương án vượt qua hợp lệ.

Không dùng hệ số tốc độ cực nhỏ thay cho trạng thái route không thể đi.

<a id="m27-t073-s119"></a>

### 90. Directional Travel

Tuyến sông có thể bất đối xứng.

Đi xuôi và đi ngược dòng có:

- base time khác;
- weather risk khác;
- load capacity khác;
- resource cost khác.

Route Edge phải hỗ trợ hướng.

Không tự lấy khoảng cách hình học rồi giả định mọi chiều giống nhau.

<a id="m27-t073-s120"></a>

### 91. Travel Risk

Một hành trình có thể có nhiều rủi ro:

- environmental;
- navigation;
- security;
- equipment;
- health;
- supplies.

Các rủi ro không nhất thiết độc lập.

Ví dụ mưa lớn vừa tăng water hazard vừa làm đường bộ khó đi.

Engine phải tránh nhân đôi cùng một nguyên nhân thành hai lần penalty không hợp lý.

Đề xuất đánh giá **risk factors theo causal sources** và chỉ tổng hợp khi rule khai báo được mối quan hệ.

<a id="m27-t073-s121"></a>

### 92. Supply Consumption

Supply được tính từ CommodityLot.

Đề xuất:

**Consumption = PartyRequirement × Time × ActivityModifier × ConditionModifier**

Kết quả được quy đổi sang lượng resource thực.

Nếu thiếu food:

- nhân vật có thể giảm hiệu suất;
- cần tìm nguồn;
- đổi kế hoạch;
- chịu Health/Condition hậu quả nếu thiếu kéo dài.

Không nên lập tức gây chết chỉ vì UI hiển thị 0 Supply Days trong một buổi.

<a id="m27-t073-s122"></a>

### 93. Environmental Hazard

Ví dụ đi qua đầm lầy.

Hard Preconditions:

- có lối vào;
- party có khả năng di chuyển;
- không có trở ngại tuyệt đối.

Risk phụ thuộc:

- terrain;
- water level;
- familiarity;
- equipment;
- load;
- weather;
- local guidance.

Một Liệp Nhân am hiểu địa phương có lợi thế.

Nhưng chuyên môn không làm mặt đất rắn lại.

<a id="m27-t073-s123"></a>

### 94. Ecology Model

Creature Population không cần mô phỏng mọi cá thể.

Đề xuất aggregate:

**Population Next = Population Current + Births + Immigration − Deaths − Harvest − Emigration**

Các thành phần có điều kiện:

- habitat quality;
- food availability;
- season;
- human activity;
- predation;
- environmental change.

Không sử dụng công thức tăng trưởng không giới hạn.

Phải có carrying capacity và những điều kiện gây suy giảm.

Các creature đặc biệt đã materialized cần được quản lý như persistent entities.

<a id="m27-t073-s124"></a>

### 95. Encounter Eligibility

Encounter không được xuất hiện chỉ vì đã lâu player chưa chiến đấu.

Một encounter thú có thể cần:

- habitat;
- population;
- creature behavior;
- thời gian;
- terrain;
- party exposure;
- khả năng quan sát.

Nếu con vật không sống ở vùng đó, encounter bị loại.

Nếu player đi theo đường an toàn đông người trong điều kiện thuận lợi, không cần tạo thú nguy hiểm chỉ để giữ gameplay.

---

<a id="m27-t073-s125"></a>

## PHẦN XVII — ECONOMY, PRODUCTION & TRADE

<a id="m27-t073-s126"></a>

### 96. Economy phải có tính bảo toàn tài nguyên

Các luồng chính:

**Production → Inventory → Transport → Consumption/Transformation/Storage.**

Một lô hàng phải có nguồn.

Một công trình phải tiêu tốn vật liệu và lao động phù hợp.

Một settlement có thể tăng sản lượng khi có nguồn lực và điều kiện sản xuất mới.

Không dùng Prosperity Score để tự tăng CommodityLot.

<a id="m27-t073-s127"></a>

### 97. Production Formula

Đề xuất:

**Output = BaseYield × LaborEfficiency × ToolEfficiency × MaterialAvailability × EnvironmentFactor × ProcessQuality**

Các yếu tố cần được quy định theo production type.

Ví dụ nghề trồng trọt:

- đất;
- nước;
- mùa;
- lao động;
- kỹ thuật;
- thời gian.

Nghề đúc:

- ore/metal input;
- fuel;
- tool;
- labor;
- technique;
- quality control.

Không sử dụng một công thức y hệt với các tham số đổi tên rồi cho kết quả kinh tế thiếu thực tế.

Cấu trúc tính toán có thể chung, nhưng domain function cần phù hợp.

<a id="m27-t073-s128"></a>

### 98. Material Availability

Nếu yêu cầu 100 đơn vị nguyên liệu nhưng chỉ có 70:

- có thể sản xuất một phần nếu process hỗ trợ;
- có thể trì hoãn;
- có thể đổi thiết kế;
- hoặc không thể bắt đầu.

Không cho `MaterialAvailability = 0.7` rồi tự sản xuất đủ 100% sản phẩm nếu quy trình không cho phép.

<a id="m27-t073-s129"></a>

### 99. Labor Capacity

Một worker có:

- thời gian làm việc;
- sức khỏe;
- skill;
- role;
- location;
- commitments.

Không thể đồng thời đóng góp đầy đủ cho nhiều production process.

Đối với background labor pool, engine có thể dùng aggregate labor units.

Nhưng tổng labor allocation không được vượt nguồn lực hiện có.

<a id="m27-t073-s130"></a>

### 100. Trade Logistics

Một shipment cần:

- cargo;
- transport capacity;
- route;
- time;
- responsible actor;
- risk;
- destination.

Trade không phải teleport hàng.

Nếu cầu hỏng:

- route cost thay đổi;
- shipment có thể delayed;
- hàng có thể được chuyển hướng;
- settlement có thể thiếu supply.

Những hậu quả này được giải qua cùng World Route Graph.

<a id="m27-t073-s131"></a>

### 101. Price Formation

Bối cảnh không nên mặc định có một đồng tiền chuẩn áp dụng khắp 12 vùng.

Trade có thể dựa trên:

- barter;
- commodity equivalence;
- debt;
- social obligation;
- local value conventions.

Đề xuất tính **Relative Exchange Value** thay vì Universal Gold Price.

Một công thức tham chiếu:

**Exchange Pressure = Demand / EffectiveSupply**

Sau đó sử dụng các price bands đã được cân bằng.

Giá giao dịch còn phụ thuộc:

- chất lượng;
- nơi;
- mùa;
- network;
- urgency;
- transport cost;
- thương lượng.

Không được để giá tăng vô hạn khi EffectiveSupply tiến gần 0.

Khi thiếu hàng hoàn toàn, có thể chuyển sang trạng thái unavailable thay vì một con số giá vô lý.

<a id="m27-t073-s132"></a>

### 102. Household & Settlement Stability

Household có:

- consumption;
- income/resource access;
- members;
- livelihood;
- obligations;
- assets.

Một household bị thiếu nguồn lực lâu dài có thể:

- giảm tiêu dùng;
- tìm việc;
- vay;
- đổi nghề;
- di cư;
- chia tách;
- yêu cầu hỗ trợ.

Không được tự động tạo một NPC trộm cắp chỉ vì household nghèo.

Các quyết định còn phụ thuộc Personality, Knowledge, Social Graph và các lựa chọn khả thi.

<a id="m27-t073-s133"></a>

### 103. Economy Tests

- Một CommodityLot không bị chuyển trùng.
- Production không tạo item khi thiếu input bắt buộc.
- Resource biến đổi có output/yield hợp lệ.
- Trade có travel time.
- Worker không bị allocate vượt capacity.
- Price không âm.
- Food shortage không tự xóa sau một tick.
- Supply Days luôn suy ra từ resource thật.
- Hoạt động player không thể tạo tiền/tài sản vô hạn bằng vòng mua bán đơn giản.

---

<a id="m27-t073-s134"></a>

## PHẦN XVIII — FACTION & POLITICAL DECISION

<a id="m27-t073-s135"></a>

### 104. Faction không phải NPC khổng lồ

Faction có:

- member groups;
- leadership;
- resources;
- goals;
- legitimacy;
- cohesion;
- internal dissent;
- knowledge.

Một faction có thể đưa ra quyết định nhưng không phải mọi thành viên đều nhất trí.

Đề xuất:

**Faction Decision = Eligible Proposals → Leadership Process → Internal Support → Resource Commitment → Execution.**

<a id="m27-t073-s136"></a>

### 105. Faction Utility

Một đề xuất được đánh giá theo:

- strategic goal;
- resource benefit;
- legitimacy impact;
- alliance impact;
- military risk;
- internal cohesion;
- obligations;
- perceived information.

Không dùng một thanh `Faction Power` để tự quyết định mọi hành động.

<a id="m27-t073-s137"></a>

### 106. Internal Support

Một faction có thể cần sự ủng hộ từ:

- thủ lĩnh;
- trưởng nhóm;
- household quyền lực;
- military actors;
- ritual authorities;
- professional networks.

Cấu trúc quyết định tùy faction.

Ví dụ một nhóm thợ có thể cần sự đồng ý của nhiều workshop trước khi đổi tiêu chuẩn sản xuất.

Một đội quân có command hierarchy khác.

Công thức bỏ phiếu hoặc mức đồng thuận phải được khai báo theo Faction Governance Model.

<a id="m27-t073-s138"></a>

### 107. Mobilization

Quyết định huy động lực lượng không tự sinh quân.

Cần:

- population/labor pool;
- authority;
- loyalty;
- equipment;
- supplies;
- travel;
- time.

Nếu không đủ food hoặc transport, kế hoạch có thể bị chậm hoặc thất bại.

Player có thể tác động tới một phần supply chain.

Nhưng không tự điều khiển toàn bộ faction chỉ vì có quan hệ tốt với một thành viên.

<a id="m27-t073-s139"></a>

### 108. Conflict Outcome

Với xung đột lớn ngoài màn hình, không cần mô phỏng từng đòn đánh.

Có thể dùng aggregate military resolution dựa trên:

- lực lượng thực;
- kinh nghiệm;
- tổ chức;
- morale;
- supplies;
- terrain;
- timing;
- intelligence;
- tactical objectives.

Kết quả tạo:

- casualties;
- resource loss;
- territory influence;
- migration;
- social memory.

Những named NPC liên quan phải được xử lý bằng mức simulation phù hợp, không biến mất ngẫu nhiên chỉ vì aggregate troop count giảm.

---

<a id="m27-t073-s140"></a>

## PHẦN XIX — AGING, HEALTH & MORTALITY

<a id="m27-t073-s141"></a>

### 109. Aging không phải một debuff theo năm

Một character lớn tuổi có thể:

- giảm recovery;
- thay đổi thể chất;
- tăng tri thức;
- có quan hệ rộng;
- có authority;
- chuyển sang vai trò mentor.

Do đó Aging cần hai luồng:

**Biological Progression**

và

**Life/Social Progression.**

Không chỉ trừ Attributes.

<a id="m27-t073-s142"></a>

### 110. Aging Formula

Đề xuất:

**Current Capability = Base Capability + Development − AgeRelatedLimitations − PersistentInjury + ContextualCompensation**

Trong đó:

- AgeRelatedLimitations thay đổi theo Health Profile.
- PersistentInjury chỉ áp dụng khi có.
- ContextualCompensation có thể đến từ công cụ, Technique hoặc kinh nghiệm.

Không cho mọi character tự suy giảm theo cùng một đường cong cố định.

<a id="m27-t073-s143"></a>

### 111. Mortality Risk

Mortality phải phụ thuộc:

- age band;
- health;
- injuries;
- illness;
- nutrition;
- environment;
- conflict exposure;
- care availability.

Đề xuất sử dụng **Conditional Hazard Model** thay vì:

> Mỗi năm random 1 lần xem NPC chết không.

Một hazard là nguy cơ trong một khoảng thời gian.

Các hazard khác nhau có thể là:

- background mortality;
- serious illness;
- injury deterioration;
- accident;
- combat.

Mỗi hazard có điều kiện thực tế.

<a id="m27-t073-s144"></a>

### 112. Probability Across Time

Nếu một hazard có xác suất `p` trong mỗi khoảng thời gian và các khoảng được giả định độc lập khi điều kiện không đổi, xác suất xảy ra ít nhất một lần trong `n` khoảng là:

**P = 1 − (1 − p)^n**

Tuy nhiên không được áp dụng máy móc nếu hazard thay đổi sau mỗi khoảng hoặc các sự kiện không độc lập.

Trong implementation, có thể sử dụng:

- bảng hazard đã định nghĩa theo age/condition;
- fixed-point probabilities;
- scheduled checks;
- survival factor accumulation.

Đối với NPC có Story Thread và Health State phức tạp, cần xử lý sự kiện cụ thể ở độ phân giải thích hợp.

<a id="m27-t073-s145"></a>

### 113. Death Event

Một nhân vật chỉ chuyển sang Dead khi:

- một rule có quyền xác lập tử vong;
- các điều kiện đã đạt;
- Death Event được commit.

Không được cho `Age > 60` tự động chuyển Dead.

Không cho NPC tử vong hai lần từ hai hazard được xử lý cùng tick.

<a id="m27-t073-s146"></a>

### 114. Recovery

Recovery phụ thuộc:

- wound;
- severity;
- treatment;
- nutrition;
- rest;
- infection;
- continuing physical stress.

Các trạng thái chuyển qua:

- active;
- stabilized;
- recovering;
- healed;
- persistent impairment;
- deteriorating.

Không được dùng một phép hồi phục nhanh trong Combat rồi xóa mọi Wound khi player tới settlement.

<a id="m27-t073-s147"></a>

### 115. Aging/Mortality Tests

- Người 60 tuổi vẫn có thể sống thêm nhiều năm.
- Người trẻ vẫn có thể gặp nguy hiểm nếu có nguyên nhân.
- Không có death do tuổi mà không có hazard rule.
- NPC chết một lần duy nhất.
- Time Skip không bỏ qua treatment window quan trọng.
- Health Conditions ảnh hưởng nghề và Travel.
- Successor Candidate được tính sau khi tử vong được commit hợp lệ.

---

<a id="m27-t073-s148"></a>

## PHẦN XX — SUCCESSION, LEGACY & MYTH

<a id="m27-t073-s149"></a>

### 116. Succession Eligibility

Một candidate cần thỏa hard requirements:

- đang sống;
- có identity;
- có thể điều khiển hợp lý;
- đủ mức tự chủ;
- có liên hệ nhân quả với predecessor hoặc campaign;
- không ở trạng thái bất khả dụng dài hạn không có phương án hợp lệ.

Sau đó đánh giá:

- narrative continuity;
- knowledge;
- motivation;
- relationship;
- opportunity;
- viable playstyle.

Không nhất thiết chọn candidate có điểm cao nhất.

Nếu nhiều người có cơ sở hợp lý, có thể cho player chọn góc nhìn tiếp tục.

Không dùng hidden min-max build làm tiêu chí chính.

<a id="m27-t073-s150"></a>

### 117. Knowledge Transfer

Mỗi loại tri thức phải có transmission event.

Ví dụ:

- lời kể;
- bản ghi;
- apprenticeship;
- witness observation;
- instruction.

Khi predecessor chết:

Personal Knowledge chưa được truyền không tự chuyển.

Đây là invariant.

<a id="m27-t073-s151"></a>

### 118. Property Transfer

Tài sản đi qua Inheritance/Asset Rights.

Một vật phẩm thuộc household không trở thành tài sản cá nhân của successor chỉ vì player đã chuyển quyền điều khiển.

Một apprentice có thể được trao tool bằng một event hợp lệ.

Một companion có thể giữ vật phẩm vì được mượn hoặc đang custody.

Không tự động chuyển toàn bộ Inventory.

<a id="m27-t073-s152"></a>

### 119. Legacy Persistence

Đề xuất:

**Legacy Persistence = MaterialSupport + SocialTransmission + InstitutionalSupport + CulturalSalience − DecayPressure**

Đây là công thức xếp hạng/đánh giá khả năng duy trì, không phải phép cộng raw fields tùy ý.

Các legacy khác nhau có luật riêng:

**Cầu:** Cần bảo dưỡng.

**Technique:** Cần người biết và điều kiện thực hành.

**Political Agreement:** Cần các bên tiếp tục công nhận.

**Myth:** Cần người kể và mạng truyền.

Không có một `Legacy Score` duy nhất.

<a id="m27-t073-s153"></a>

### 120. Myth Transformation

Một Myth Variant chỉ được tạo từ:

- Origin Event hoặc Claim;
- transmission network;
- cultural interpretation;
- time;
- narrative grammar;
- distortion rule.

Nó có thể thay đổi:

- biểu tượng;
- cách đánh giá;
- số lượng phóng đại;
- vai trò người tham gia.

Nhưng không sửa Origin Event.

<a id="m27-t073-s154"></a>

### 121. Mạch Lạc

Mạch có thể tác động tới một số rule đã định nghĩa trước.

Không phải mana và không phải nguồn giải thích mọi điều phi lý.

Một Ritual có hai kết quả:

**Social Outcome**

và

**Mạch Outcome**.

Hai kết quả có thể khác nhau.

Ví dụ một nghi lễ được cộng đồng chấp nhận nhưng không tạo thay đổi Mạch.

Hoặc một hiện tượng Mạch xảy ra nhưng không ai đủ khả năng hiểu.

Các công thức Mạch cụ thể cần được đóng thành Rule Handlers có giới hạn, không dùng một biến `SupernaturalPower` chung để ghi đè domain khác.

---

<a id="m27-t073-s155"></a>

## PHẦN XXI — HISTORICAL ANCHOR RESOLUTION

<a id="m27-t073-s156"></a>

### 122. Hard Anchor là ràng buộc kịch bản, không phải phép thuật

Các sự kiện lớn:

- chuyển tiếp sang Âu Lạc;
- hình thành Cổ Loa;
- công nghệ và huyền thoại Nỏ thần;
- khủng hoảng cuối Âu Lạc;

cần được thiết kế như những kịch bản có nhiều đường nhân quả hợp lệ.

Không cho Historical Controller tự sinh vật liệu, quân đội hoặc người mới.

<a id="m27-t073-s157"></a>

### 123. Anchor Eligibility

Một Historical Anchor gồm:

- earliest time;
- latest allowed window;
- required conditions;
- protected invariants;
- scenario set;
- adjustable outcomes.

Engine chọn scenario hợp lệ từ World State.

Nếu thiếu điều kiện, các actor lịch sử có thể thực hiện những hành động hợp lệ nhằm tiến tới điều kiện đó.

Nhưng không được bỏ qua resource constraints.

<a id="m27-t073-s158"></a>

### 124. Scenario Selection

Có thể sử dụng:

**Scenario Feasibility → Scenario Utility → Deterministic Selection.**

Một scenario cần:

- actor;
- nguồn lực;
- route;
- time;
- institution;
- historical compatibility.

Các kịch bản không đủ điều kiện bị loại trước random.

<a id="m27-t073-s159"></a>

### 125. Trạng thái bất khả thi

Nếu không còn scenario hợp lệ cho một Hard Anchor:

Đây là một thiết kế không được phép âm thầm xử lý bằng hack.

Trong prototype:

- ghi error;
- dump state;
- trace nguyên nhân;
- báo thiếu coverage.

Trong bản chính thức:

Cần bảo đảm các tương tác player được hỗ trợ có phương án causal continuation.

Nếu muốn cho player thực sự viết lại lịch sử ở mức xóa Hard Anchor, đó sẽ là một game mode hoặc cấu trúc campaign khác, không phải ngẫu nhiên phát sinh từ một bug.

<a id="m27-t073-s160"></a>

### 126. Anchor Validation Tests

Một kịch bản Cổ Loa phải được kiểm tra khi:

- nguồn nguyên liệu thiếu;
- tuyến vận tải đóng;
- thủ lĩnh phụ qua đời;
- một faction rút hỗ trợ;
- player phá công trường phụ;
- companion chuyển phe;
- dân cư di cư;
- một dự án khác tranh nguồn lực.

Mỗi trường hợp phải có:

- hướng tiến triển hợp lệ;
- hoặc giới hạn interaction rõ và có nguyên nhân;
- hoặc được đánh dấu là thiếu thiết kế để sửa trước release.

---

<a id="m27-t073-s161"></a>

## PHẦN XXII — DIFFICULTY & GAME MODE PARAMETERS

<a id="m27-t073-s162"></a>

### 127. Không sửa World Truth để cân bằng

Story, Standard và Tactical có thể khác nhau ở:

- cửa sổ cứu chữa;
- thông tin hỗ trợ phù hợp knowledge;
- một số tolerance của combat/survival;
- tính nghiêm khắc của failure;
- cách trình bày danger.

Nhưng không được khác nhau ở:

- danh tính hung thủ;
- nội dung hợp đồng;
- lịch sử nhân vật;
- sự tồn tại của item;
- faction knowledge;
- nguyên nhân một event đã commit.

<a id="m27-t073-s163"></a>

### 128. Difficulty Config

Đề xuất có:

```text
DifficultyProfile
    ActionRiskTuning
    InjurySeverityTuning
    RecoveryAssistance
    SurvivalPressure
    TacticalAssistance
    InformationAssistance
    SavePersistencePolicy
```

Các thông số này được version hóa và lưu trong Save.

Không cho phép một setting thay đổi hidden formula không được ghi lại.

<a id="m27-t073-s164"></a>

### 129. Adaptive Presentation

World Director có thể:

- nhấn mạnh lời khuyên hợp lệ;
- giới thiệu tutorial theo context;
- ưu tiên exposure phù hợp;
- tóm tắt thông tin player đã biết.

Không được bí mật giảm Defense của enemy sau ba lần player thua.

Nếu Story Mode giảm một số rủi ro, các thay đổi đó phải thuộc cấu hình game mode được định nghĩa rõ.

---

<a id="m27-t073-s165"></a>

## PHẦN XXIII — END-TO-END FORMULA EXAMPLE

<a id="m27-t073-s166"></a>

### 130. Kịch bản: Thợ đúc Đồng Đỏ thiếu nhiên liệu

World State:

- Một workshop đang sản xuất.
- Fuel Stock giảm.
- Một tuyến vận chuyển bị ảnh hưởng.
- Nhu cầu sản phẩm vẫn cao.
- Chủ workshop có nghĩa vụ giao hàng.

<a id="m27-t073-s167"></a>

### 131. Pressure

Economy tính:

- fuel demand;
- current fuel;
- expected supply;
- transport delay;
- production schedule.

Nếu nhiên liệu dự kiến không đủ:

`FuelShortagePressure` được tạo hoặc cập nhật.

<a id="m27-t073-s168"></a>

### 132. Event Eligibility

Event Engine kiểm tra các template:

- tìm nguồn nhiên liệu;
- thương lượng với supplier;
- giảm sản lượng;
- vay nguồn lực;
- tranh chấp phân phối.

Không phải tất cả đều hợp lệ.

Ví dụ:

Nếu không có supplier đã biết trong mạng liên hệ, hành động thương lượng với supplier cụ thể không được tạo.

NPC có thể chủ động tìm hiểu nguồn mới nếu có route và cơ hội thực tế.

<a id="m27-t073-s169"></a>

### 133. NPC Utility

Chủ workshop đánh giá:

- tiếp tục sản xuất;
- tìm nguồn;
- thương lượng;
- thay sản phẩm;
- hoãn đơn hàng.

Giả sử tìm nguồn nhiên liệu có Utility cao.

NPC tạo Command tương ứng.

<a id="m27-t073-s170"></a>

### 134. Player Exposure

Player là người có nghề liên quan hoặc quen biết workshop.

World Director phát hiện có cuộc trò chuyện hợp lệ.

NPC có thể đề nghị player hỗ trợ.

Nếu player không có mặt, NPC tiếp tục thực hiện hành động riêng.

<a id="m27-t073-s171"></a>

### 135. Player Choice

Player có thể:

- dùng quan hệ tìm supplier;
- mở tuyến mới;
- thương lượng giảm đơn hàng;
- giúp cải tiến quy trình tiêu hao;
- không can thiệp.

Các lựa chọn dùng những rule khác nhau.

Không phải năm nút hội thoại cùng dẫn tới `questComplete = true`.

<a id="m27-t073-s172"></a>

### 136. Outcome

Giả sử player tìm được một supplier mới nhưng giá cao.

Engine commit:

- Supplier Relationship;
- Trade Agreement;
- Commodity Transfer Schedule;
- Material Cost;
- Workshop Production Continuation.

Những thay đổi này có nguồn và thời gian.

<a id="m27-t073-s173"></a>

### 137. Long-term Consequence

Một vài năm sau:

Workshop có thể lệ thuộc supplier mới.

Một faction có thể giành quyền kiểm soát route.

Apprentice có thể học cách sử dụng nhiên liệu hiệu quả hơn.

Player có thể được công nhận là người giúp khôi phục sản xuất.

Khi bước vào giai đoạn xây dựng Cổ Loa, workshop có thể tham gia supply chain rộng hơn.

Không có LLM realtime quyết định điều gì xảy ra.

---

<a id="m27-t073-s174"></a>

## PHẦN XXIV — THAM SỐ HÓA VÀ BALANCING

<a id="m27-t073-s175"></a>

### 138. Không hardcode tất cả công thức vào content

Cần tách:

**Rule Algorithm:** Code có version.

**Tuning Parameters:** Data có kiểm soát.

**Content Definition:** Template, actor role, tags.

Ví dụ:

Skill Growth algorithm được viết bằng code.

Các hệ số PracticeFactor, MentorFactor và DiminishingReturn có thể nằm trong data config.

Event Template không được thay thuật toán bằng một đoạn script ngẫu nhiên.

<a id="m27-t073-s176"></a>

### 139. Parameter Registry

Đề xuất:

```text
ParameterID
Domain
Value
Unit
ValidRange
DefaultValue
RulesetVersion
Description
Owner
```

Các tham số cần khai báo miền giá trị hợp lệ.

Ví dụ một probability weight không được âm.

Một Time Cost không được nhỏ hơn 0 nếu rule không hỗ trợ.

Một production yield không được vượt giới hạn thiết kế mà không có nguyên nhân.

<a id="m27-t073-s177"></a>

### 140. Những tham số cần tuning bằng simulation

<a id="m27-t073-s178"></a>

#### Character

- age distribution;
- aptitude distribution;
- life-event frequency;
- skill growth;
- training;
- class emergence.

<a id="m27-t073-s179"></a>

#### NPC

- utility weights;
- commitment;
- replanning;
- rumor sharing;
- relationship change.

<a id="m27-t073-s180"></a>

#### Economy

- production yield;
- consumption;
- spoilage;
- route cost;
- price response;
- labor capacity.

<a id="m27-t073-s181"></a>

#### Combat

- attack/defense;
- wound severity;
- stamina;
- morale;
- recovery.

<a id="m27-t073-s182"></a>

#### World

- hazard;
- ecology;
- event frequency;
- faction action cadence;
- Time Skip aggregation.

<a id="m27-t073-s183"></a>

#### Narrative

- exposure density;
- information spread;
- thread duration;
- aftermath relevance.

<a id="m27-t073-s184"></a>

### 141. Balance không chỉ là trung bình

Cần đo phân phối kết quả.

Ví dụ:

Một công thức có mức tử vong trung bình hợp lý nhưng một số seed khiến cả settlement chết quá nhanh.

Hoặc một Profession có progression trung bình tốt nhưng 20% character gần như không có cơ hội học nghề.

Cần kiểm tra:

- median;
- percentiles;
- extreme cases;
- failure rates;
- distribution by origin;
- distribution by profession;
- concentration of wealth;
- long-term stability.

Không chỉ nhìn vào một World Seed đẹp.

---

<a id="m27-t073-s185"></a>

## PHẦN XXV — ANTI-EXPLOIT DESIGN

<a id="m27-t073-s186"></a>

### 142. Resource Duplication

Các vòng exploit thường gặp:

- mua/bán tạo chênh lệch vô hạn;
- repair hoàn nguyên item mà không tiêu hao vật liệu;
- salvage nhiều hơn nguyên liệu đầu vào;
- Time Skip tạo production không có labor;
- inheritance chuyển tài sản cho hai người;
- quest reward không bị trừ từ nguồn cung.

Phải có property-based tests cho những trường hợp này.

<a id="m27-t073-s187"></a>

### 143. Skill Farming

Nếu một hành động dễ tạo Skill Progress cao:

player có thể lặp lại không giới hạn.

Giải pháp:

- diminishing returns;
- novelty;
- meaningful challenge;
- time/resource cost;
- domain-specific practice logic.

Không giải quyết bằng hard cap mỗi ngày không có lý do nếu có thể thiết kế progression tốt hơn.

<a id="m27-t073-s188"></a>

### 144. Social Farming

Nếu player có thể liên tục tặng cùng một item để tăng Trust:

Relationship sẽ mất ý nghĩa.

Cần:

- nhận diện repetition;
- actor need;
- item value;
- expectation;
- relationship context;
- social interpretation.

Một NPC đã có đủ lương thực không nhất thiết tiếp tục tăng lòng biết ơn vô hạn vì được tặng thêm.

<a id="m27-t073-s189"></a>

### 145. Event Farming

Player không được tự tạo pressure giả vô hạn rồi nhận thưởng mỗi lần giải quyết.

Ví dụ:

Tự phá thuyền rồi sửa để nhận Profession Milestone liên tục.

Engine phải theo dõi:

- origin;
- causal responsibility;
- repetition;
- actual external value;
- event significance.

Việc sửa thuyền do mình cố ý phá vẫn có thể giúp luyện kỹ năng ở mức nào đó, nhưng không được mặc định tạo Reputation như cứu một người gặp nạn.

<a id="m27-t073-s190"></a>

### 146. Succession Exploit

Player không được có lợi vô hạn bằng cách cố tình để protagonist chết.

Các hệ quả:

- mất identity cũ;
- mất tri thức chưa truyền;
- tài sản xử lý theo ownership;
- quan hệ không copy;
- một số obligations có thể không tiếp tục;
- successor có lịch sử riêng.

Succession là chuyển góc nhìn, không phải nút reset build.

---

<a id="m27-t073-s191"></a>

## PHẦN XXVI — AUTOMATED VALIDATION

<a id="m27-t073-s192"></a>

### 147. Rule Unit Test

Mỗi Rule Handler cần kiểm tra:

- input hợp lệ;
- input không hợp lệ;
- min/max;
- trạng thái thiếu dữ liệu;
- kết quả cơ bản;
- hậu quả;
- lỗi domain;
- determinism.

Không chỉ test Happy Path.

<a id="m27-t073-s193"></a>

### 148. Property-Based Test

Các invariant:

**Character:** Người chết không thể tham gia hoạt động như đang sống.

**Inventory:** Một item không tồn tại ở hai physical locations.

**Commodity:** Resource transfer không tự tạo hàng.

**Knowledge:** NPC không biết fact không có nguồn.

**Time:** Scheduled Event không commit hai lần.

**Combat:** Không có action ngoài capability hoặc action budget.

**Historical:** Hard Anchor không bị phá bởi một scenario không được kiểm chứng.

**Succession:** Không copy Skill/Knowledge trực tiếp.

<a id="m27-t073-s194"></a>

### 149. Monte Carlo Headless Simulation

Chạy hàng nghìn World Seeds khi có khả năng tính toán.

Theo dõi:

- số NPC sinh/tử;
- career distribution;
- settlement stability;
- resource shortages;
- faction conflicts;
- event frequencies;
- Story Thread outcomes;
- succession candidates;
- historical anchor viability.

Không cần render.

Nếu một seed tạo ra world collapse bất thường, phải giữ seed và event log để debug.

<a id="m27-t073-s195"></a>

### 150. Deterministic Replay

Test:

**Same Seed + Same Initial Snapshot + Same Commands + Same Ruleset/Content + Same Ordering → Same Authoritative State.**

Kết quả có thể so sánh bằng state hashes canonical.

Cần loại khỏi hash những dữ liệu không authoritative như:

- UI animation;
- debug timestamps theo giờ máy;
- cache presentation;
- hiệu ứng hình ảnh.

<a id="m27-t073-s196"></a>

### 151. Differential Testing

Khi sửa công thức:

Chạy cùng tập World Seeds trên hai Ruleset Versions.

So sánh:

- Skill Growth;
- NPC mortality;
- Economy;
- Event Count;
- Faction Stability;
- Story Thread Resolution.

Một thay đổi combat không nên vô tình khiến Economy của các vùng xa biến động mạnh nếu không có causal linkage hợp lệ.

Khi có thay đổi lớn, cần giải thích nó xuất phát từ đâu.

---

<a id="m27-t073-s197"></a>

## PHẦN XXVII — RULE INSPECTOR & DEBUG DATA

<a id="m27-t073-s198"></a>

### 152. Rule Trace

Một Rule Evaluation quan trọng cần có trace:

```text
RuleID
RuleVersion
InputStateRevision
BoundEntities
HardConstraintResults
ResolvedModifiers
RandomContext
RandomDraws
Outcome
AppliedEffects
ValidationResults
CommittedEventIDs
```

Không cần lưu mọi lần tính nội bộ của NPC Tier C ở mức đầy đủ trong bản phát hành.

Nhưng developer build cần có chế độ trace và lấy mẫu.

<a id="m27-t073-s199"></a>

### 153. Ví dụ Rule Trace

```text
Action: RepairBoat
Actor: VacMan
Target: Boat183

Preconditions:
  ActorAlive: PASS
  ToolAvailable: PASS
  MaterialsAvailable: PASS
  AccessAllowed: PASS

SkillBonus: +5
AttributeBonus: +1
Preparation: +2
Difficulty: 20

Roll: 12
Total: 20
Margin: 0

Outcome: Success
RepairQuality: Temporary
MaterialsConsumed: Recorded
```

Trace phải được sinh từ thông số thật.

Không dùng LLM tự giải thích kết quả mà không có rule data.

<a id="m27-t073-s200"></a>

### 154. Causal Graph Debugger

Một công cụ cần truy ngược:

`WorkshopClosed`

← `FuelUnavailable`

← `TradeRouteDelayed`

← `BridgeCollapsed`

← `FloodPressureExceeded`.

Đây là công cụ thiết yếu khi world có hàng triệu tổ hợp trạng thái.

Nếu không có causal tracing, việc tìm bug sẽ rất tốn thời gian.

---

<a id="m27-t073-s201"></a>

## PHẦN XXVIII — PARAMETER BASELINE CHO PROTOTYPE

<a id="m27-t073-s202"></a>

### 155. Các giá trị đề xuất khóa để bắt đầu lập trình

| Hệ thống | Baseline đề xuất | Trạng thái |
|---|---|---|
| Attribute | 1–10 | Đã khóa thang |
| Skill | 0–100 | Đã khóa thang |
| Action Roll | 2d10 | Đã khóa định hướng |
| Skill Bonus | `floor(Skill/10)` | Prototype |
| Attribute Bonus | `floor((Attribute-5)/2)` | Prototype |
| Difficulty Bands | 10, 14, 17, 20, 24, 28 | Prototype |
| Practice Progression | Diminishing Returns | Khóa nguyên tắc, chưa khóa hệ số |
| NPC Utility | 0–1.000 | Prototype |
| RNG | Seeded + Contextual | Khóa nguyên tắc |
| Event Eligibility | Hard Constraint trước Weight | Khóa |
| Relationship | Đa chiều, có hướng | Khóa |
| Reputation | Claim-based | Khóa |
| World Director | Exposure-only | Khóa |
| Combat Actions | 1 Major + 1 Minor, Reactions giới hạn | Đã khóa |
| Combat Space | Zone + Local Position Slots | Đã khóa |
| Supply | Derived từ Commodity | Khóa |
| Travel | Route Graph + Conditional Costs | Khóa |
| Succession | Candidate Eligibility + Transfer Rules | Khóa |
| Historical Anchor | Hard/Flexible/Emergent | Khóa cấu trúc |
| Runtime LLM | Không tham gia quyết định | Đã khóa |

Các tham số prototype có thể thay đổi sau kiểm thử mà không làm thay đổi triết lý hệ thống.

---

<a id="m27-t073-s203"></a>

## PHẦN XXIX — CÁC TEST SCENARIO BẮT BUỘC

<a id="m27-t073-s204"></a>

### 156. Test 1 — Skill Progression

Character Skill 20 và Skill 80 cùng thực hiện công việc giống nhau.

Kỳ vọng:

- cả hai có thể hoàn thành nếu đủ điều kiện;
- người Skill 80 thường có khả năng xử lý tốt hơn;
- công việc quá dễ cho Master mang lại ít Skill Growth;
- người mới có thể học nhiều hơn nếu nhận được feedback;
- không có progression vô hạn.

<a id="m27-t073-s205"></a>

### 157. Test 2 — NPC Decision

NPC thiếu food và đang có một obligation.

Kỳ vọng:

- NPC đánh giá cả nhu cầu và nghĩa vụ;
- chỉ tạo các hành động hợp lệ;
- không sử dụng thông tin toàn tri;
- không đổi nghề/hành động mỗi tick vô cớ;
- tạo command và commit qua Rule Engine.

<a id="m27-t073-s206"></a>

### 158. Test 3 — Rumor

Một nhân chứng phát biểu sai.

Tin lan qua ba NPC.

Kỳ vọng:

- Origin Claim không bị mất;
- các belief có thể khác nhau;
- không tự thay đổi World Truth;
- player chỉ biết tin nếu có nguồn;
- không tăng credibility vô hạn do circular repetition.

<a id="m27-t073-s207"></a>

### 159. Test 4 — Crafting

Player thử chế tác với vật liệu thiếu.

Kỳ vọng:

- hard constraint từ chối hoặc cho phép partial production nếu recipe hỗ trợ;
- không rút outcome RNG không cần thiết;
- không tạo item miễn phí;
- không consume resource khi command bị từ chối trước commit.

<a id="m27-t073-s208"></a>

### 160. Test 5 — Combat & Injury

Một cú giáo chạm armor.

Kỳ vọng:

- impact và mitigation được xử lý;
- không tự tạo critical wound chỉ vì roll cao;
- trạng thái Wound nếu có tồn tại ngoài combat;
- equipment condition thay đổi hợp lệ;
- outcome có thể replay.

<a id="m27-t073-s209"></a>

### 161. Test 6 — Travel

Player dùng thông tin route cũ.

World Truth xác định cầu đã hỏng.

Kỳ vọng:

- Travel Planner dự báo theo knowledge;
- hành trình thực giải theo Route State;
- player nhận biết tình trạng khi tới vị trí có thể quan sát;
- information được cập nhật;
- không tự mở cầu để tránh làm khó player.

<a id="m27-t073-s210"></a>

### 162. Test 7 — Time Skip

Player nghỉ hai năm.

Có event:

- mùa vụ;
- học nghề;
- lời hứa tới hạn;
- một NPC chết;
- thay đổi trade route.

Kỳ vọng:

- scheduler xử lý đúng thứ tự;
- các actor biết tin theo mạng truyền;
- nghề phát triển theo điều kiện;
- không bỏ qua mandatory event;
- không phát sinh thông tin toàn tri trong báo cáo.

<a id="m27-t073-s211"></a>

### 163. Test 8 — Succession

Predecessor chết với secret chưa được truyền.

Kỳ vọng:

- successor giữ riêng Attributes/Skills;
- không nhận secret;
- asset rights xử lý theo thừa kế;
- party/household cập nhật;
- những quest có liên quan chuyển trạng thái hợp lệ.

<a id="m27-t073-s212"></a>

### 164. Test 9 — Historical Anchor

Player làm gián đoạn một supply chain của Cổ Loa.

Kỳ vọng:

- resource giảm thật;
- scenario được đánh giá lại;
- actor lịch sử phải tìm giải pháp hợp lệ;
- không sinh tài nguyên miễn phí;
- nếu thiếu scenario thì diagnostics phải phát hiện.

<a id="m27-t073-s213"></a>

### 165. Test 10 — Determinism

Chạy cùng một command sequence với cùng seed và phiên bản.

Kỳ vọng:

- events giống nhau;
- committed state giống nhau;
- ownership giống nhau;
- NPC decisions giống nhau;
- RNG trace giống nhau.

---

<a id="m27-t073-s214"></a>

## PHẦN XXX — QUYẾT ĐỊNH KIẾN TRÚC CUỐI CÙNG

<a id="m27-t073-s215"></a>

### 166. Những nguyên tắc phải khóa

**Rule 1:** Không có state mutation ngoài authoritative pipeline.

**Rule 2:** Hard constraints luôn chạy trước probabilistic resolution.

**Rule 3:** Random chỉ chọn trong không gian kết quả hợp lệ.

**Rule 4:** Skill không tự tạo tài nguyên hoặc quyền lực.

**Rule 5:** NPC chỉ ra quyết định từ Perceived State.

**Rule 6:** Relationship, Reputation và Belief là các hệ khác nhau.

**Rule 7:** Event tồn tại độc lập với Quest.

**Rule 8:** World Director không quyết định kết quả gameplay.

**Rule 9:** Combat tạo tác động vật lý; Health sở hữu thương tích.

**Rule 10:** Crafting và Economy sử dụng tài nguyên có nguồn.

**Rule 11:** Travel và Trade dùng chung Route Graph.

**Rule 12:** Class hình thành từ evidence, không chỉ từ threshold Skill.

**Rule 13:** Character phát triển theo lịch sử sống.

**Rule 14:** Aging không phải debuff chung theo năm.

**Rule 15:** Succession không copy nhân vật.

**Rule 16:** Myth biến đổi lời kể, không sửa sự thật đã commit.

**Rule 17:** Historical Anchors có kịch bản nhân quả hợp lệ.

**Rule 18:** Content Template chỉ gọi approved handlers.

**Rule 19:** LLM chỉ hỗ trợ nội dung offline trong kiến trúc chính thức.

**Rule 20:** Mọi quy tắc quan trọng có thể giải thích, version hóa và kiểm thử.

---

<a id="m27-t073-s216"></a>

## PHẦN XXXI — NHỮNG GÌ CẦN KIỂM CHỨNG TRƯỚC KHI KHÓA BALANCE

<a id="m27-t073-s217"></a>

### 167. Phân phối kết quả 2d10

Cần mô phỏng xác suất cho:

- các Skill band;
- từng Attribute band;
- modifier do chuẩn bị;
- condition penalties;
- Difficulty thresholds.

Đặc biệt cần kiểm tra:

- Expert có vượt quá dễ phần lớn thử thách không;
- Beginner có bị khóa khỏi quá nhiều hoạt động không;
- Preparation có đủ giá trị không;
- Injury có làm gameplay bế tắc không;
- Technique có tạo stacking quá mạnh không.

Không nên chỉnh từng Skill riêng khi công thức chung đang mất cân bằng.

<a id="m27-t073-s218"></a>

### 168. Long-Term Progression

Cần chạy simulation nhiều năm cho các profile:

- người trẻ học nghề;
- người đã có nghề;
- người trung niên đổi nghề;
- người lớn tuổi truyền nghề;
- người có ít cơ hội học;
- người sống trong settlement có tài nguyên khác nhau.

Nếu Skill Growth khiến đa số nhân vật thành Master quá sớm, phải điều chỉnh.

Nếu Skill tăng quá chậm khiến nghề nghiệp nhàm chán, cũng phải điều chỉnh.

Không lấy tốc độ progression của một character làm mẫu cho tất cả.

<a id="m27-t073-s219"></a>

### 169. NPC Stability

Cần kiểm tra:

- tỷ lệ NPC bỏ nghề;
- tỷ lệ NPC di cư;
- số lần replanning;
- mức độ thực hiện obligation;
- khả năng household tồn tại;
- số event sinh trên mỗi khoảng thời gian.

Nếu NPC liên tục đổi quyết định, Utility/Hysteresis chưa tốt.

Nếu NPC không bao giờ thay đổi, Goals và Pressure có thể quá yếu.

<a id="m27-t073-s220"></a>

### 170. World Economy Stability

Cần kiểm tra:

- food production/consumption;
- resource shortages;
- transport bottlenecks;
- trade price bands;
- wealth concentration;
- settlement decline;
- recovery after disasters.

Một world có thể có khủng hoảng, nhưng không được liên tục sụp đổ do một lỗi tích lũy trong công thức.

<a id="m27-t073-s221"></a>

### 171. Narrative Diversity

Cần đo:

- số dạng event thực sự khác nhau;
- mức độ lặp;
- tỷ lệ event có outcome nhiều hướng;
- tỷ lệ quest bất khả thi;
- tỷ lệ player bị quá tải;
- mức ảnh hưởng nghề nghiệp;
- khả năng noncombat playthrough;
- những hậu quả dài hạn quan sát được.

Đây là các metric quan trọng hơn số lượng tổ hợp tính toán lý thuyết.

---

<a id="m27-t073-s222"></a>

## PHẦN XXXII — KẾT LUẬN CHÍNH THỨC

<a id="m27-t073-s223"></a>

### 172. Hàng triệu nhánh phải đến từ hệ thống, không phải văn bản

Một triệu tổ hợp không cần một triệu đoạn thoại.

Một triệu tổ hợp có thể hình thành từ:

- một số lượng hữu hạn các rules;
- hàng nghìn entities;
- những mối quan hệ;
- thời gian;
- vị trí;
- nguồn lực;
- những sự kiện đã xảy ra;
- lựa chọn player;
- xác suất có điều kiện;
- các kết quả tiếp tục thay đổi thế giới.

Những biến này kết hợp với nhau và tạo ra các trạng thái đa dạng.

Điều quan trọng không phải mỗi trạng thái có một câu chuyện viết sẵn.

Điều quan trọng là engine biết:

**Vì sao trạng thái đó tồn tại.**

**Ai có thể biết về nó.**

**Ai có thể tác động.**

**Những gì sẽ thay đổi nếu có hành động.**

**Những gì sẽ xảy ra nếu không ai can thiệp.**

<a id="m27-t073-s224"></a>

### 173. Một thế giới có quy luật sẽ sinh ra câu chuyện

Một người thợ thiếu nhiên liệu.

Một người vận tải bị chặn tuyến.

Một household thiếu food.

Một faction cần hàng quân dụng.

Một apprentice muốn học nghề.

Một người giữ lời hứa với bạn cũ.

Những chuyện đó có thể liên kết vì họ cùng sống trong một hệ thống kinh tế, chính trị và xã hội.

Không cần một LLM quyết định rằng họ phải cùng xuất hiện để tạo drama.

Engine cần tạo được những quan hệ nhân quả đủ tốt để sự liên kết xuất hiện từ thế giới.

<a id="m27-t073-s225"></a>

### 174. Công thức bí mật không thay thế thiết kế tốt

Một công thức càng khó đoán không có nghĩa càng hay.

Một NPC đổi ý bất thường, một vật phẩm hỏng bất ngờ hoặc một trận lũ xảy ra không dấu hiệu sẽ khiến player cảm thấy thế giới ngẫu nhiên và thiếu công bằng.

Công thức ẩn phải tạo ra:

- sự không chắc chắn hợp lý;
- kết quả có thể tìm hiểu;
- những quyết định có giá trị;
- sự khác biệt giữa nhân vật;
- động cơ để khám phá và chuẩn bị.

Developer phải biết đầy đủ công thức và nguyên nhân.

Player không nhất thiết biết mọi hệ số, nhưng cần có khả năng hiểu ý nghĩa của quyết định.

<a id="m27-t073-s226"></a>

### 175. Tuyên ngôn kết thúc Module 27

**LLM có thể giúp đội phát triển nghĩ ra hàng nghìn nhân vật và sự kiện.**

**Content Pipeline chuyển những ý tưởng đó thành dữ liệu có cấu trúc.**

**Rule Engine kiểm tra điều kiện và giải quyết kết quả.**

**World State lưu những hậu quả thật.**

**Knowledge Engine quyết định ai biết điều gì.**

**World Director tổ chức cách người chơi tiếp xúc với thế giới.**

**Presentation Layer kể lại điều đã xảy ra.**

**Time Engine đưa thế giới tiến về phía trước.**

**Legacy Engine giữ lại những dấu vết đủ điều kiện tồn tại sau nhiều năm.**

Không bộ phận nào được tự ý thay thế vai trò của bộ phận khác.

> **Chúng ta không xây một AI đang viết game theo thời gian thực. Chúng ta xây một thế giới có luật lệ, trong đó vô số câu chuyện có thể hình thành từ những con người và hoàn cảnh khác nhau.**

Đây là nền để chuyển sang lập trình Rule Engine và triển khai Vertical Slice.

---

<a id="m27-t073-s227"></a>

## PHẦN XXXIII — BƯỚC TIẾP THEO

<a id="m27-t073-s228"></a>

### 176. Module 28 — Vertical Slice Implementation Blueprint

Sau Module 27, bước quan trọng nhất không phải tiếp tục tạo công thức trên giấy.

Cần xây dựng một kế hoạch triển khai có đầu ra thực tế.

**Module 28 phải xác định:**

- repository structure chính thức;
- công nghệ prototype;
- danh sách task theo milestone;
- các class/interface phải viết;
- thứ tự triển khai Foundation → Domain → Simulation → UI;
- một world seed thử nghiệm;
- một vùng và settlement mẫu;
- NPC/Household sample data;
- Event Template sample;
- Rule Handler sample;
- Quest/Story Thread sample;
- Combat/Travel/Crafting demo;
- Time Skip và Succession demo;
- deterministic replay tests;
- headless benchmark;
- nội dung cần bàn giao cho coding agent;
- tiêu chí nghiệm thu từng sprint hoặc milestone.

**Đầu ra mong muốn:** Một bản thiết kế đủ cụ thể để đội phát triển hoặc coding agent có thể bắt đầu viết code mà không phải tự suy đoán những quyết định gameplay đã được khóa trong GDD.

Module 28 cũng cần chỉ rõ phần nào là engine foundation thực sự tái sử dụng được và phần nào chỉ là demo content của Vertical Slice, tránh xây lại kiến trúc khi chuyển sang full game.

---

<a id="m27-t073-s229"></a>

### Đánh giá sau Module 27

Tôi đề xuất **khóa nguyên tắc và cấu trúc công thức**, nhưng giữ khả năng điều chỉnh hệ số qua dữ liệu có phiên bản. Cụ thể:

- **Khóa:** Hard Constraints, authoritative resolution, deterministic RNG, causal event generation, quyền sở hữu dữ liệu, NPC Perceived State, không dùng LLM runtime quyết định gameplay.
- **Dùng làm baseline:** Công thức 2d10, Skill Bonus, Attribute Bonus, Difficulty Bands, Utility Score, Skill Growth và các hệ số Pressure.
- **Bắt buộc kiểm thử trước khi khóa balance:** Phân phối kết quả, progression qua nhiều năm, ổn định kinh tế, hành vi NPC, combat lethality và xác suất phát sinh event.

Các nguồn nghiên cứu chính: *Procedural Content Generation in Games* (Shaker, Togelius & Nelson), nghiên cứu về constraint-based PCG như *CatSAT*, tài liệu *Fix Your Timestep!* của Glenn Fiedler, cùng hướng dẫn về trạng thái RNG của Unity. <Cite refs={["turn740203search4","turn740203search2","turn928237search6","turn928237search0"]}/>

**Bước tiếp theo phù hợp nhất là Module 28 — Vertical Slice Implementation Blueprint.** Ở bước đó, trọng tâm sẽ chuyển từ thiết kế công thức sang **đặc tả những thành phần code đầu tiên cần xây, thứ tự triển khai, dữ liệu thử nghiệm và bộ acceptance tests có thể chạy được**.

<!-- SOURCE T073 END -->

