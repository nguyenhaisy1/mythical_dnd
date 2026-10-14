# Module 18 — Kinh tế, thương mại, chế tác và settlement

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T043](../../source/conversation.md#t043)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Commodity/Item/Unique Object có abstraction khác nhưng cùng ledger quyền sở hữu. Production/trade bị giới hạn thời gian, capacity và stock; không feedback vô hạn trong một tick. Bổ sung 14/10/2026: ADR-008 và contract specs có hiệu lực trước các lựa chọn còn mở/ví dụ cũ trong body nghiên cứu; chưa phải runtime implementation.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Lots, workforce, tools, rights, route graph, demand, season và recipes. |
| Đầu ra / bên sở hữu | Production/transfer/craft proposals; Assets sở hữu quantities/rights; Economy sở hữu plans/prices. |
| Đọc cùng | 03, 08, 13, 14, 26–27.; ADR-008; docs/production/DOCUMENTATION-COMPLETION-2026-10-14.md. |
| Cổng nghiệm thu | Sửa thuyền tiêu thụ đúng vật liệu; failed commit không mất/nhân đôi stock; route closure tác động trade. |
| Việc chưa được chứng minh | Economic coefficients, commodity catalog và physical SQLite schema. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T043 BEGIN -->

Phần này sẽ được thiết kế với một nguyên tắc rất quan trọng: **Economy, Trade, Crafting và Settlement Interaction phải là gameplay thật**, không phải những hệ phụ để mua bình máu hoặc chế đồ. Một người chơi có thể xây cả một cuộc đời quanh nghề thủ công, thương mại, quản lý cộng đồng hay sản xuất mà vẫn trải nghiệm đầy đủ campaign lịch sử.

<a id="m18-t043-s001"></a>

## GDD MODULE 18 — ECONOMY, TRADE, CRAFTING & SETTLEMENT INTERACTION SYSTEM

<a id="m18-t043-s002"></a>

## PART I — TẦM NHÌN TỔNG THỂ

<a id="m18-t043-s003"></a>

### 1. DESIGN PHILOSOPHY

Nền kinh tế của game phải tạo cảm giác rằng:

> **Mọi vật đều đến từ đâu đó, được ai đó làm ra, được vận chuyển bằng một con đường nào đó và được một người nào đó cần đến.**

Một cây giáo không xuất hiện trong shop chỉ vì player đạt Level 10.

Nó cần:

- quặng;
- nhiên liệu;
- người luyện;
- người đúc;
- gỗ làm cán;
- người chế tác;
- tuyến vận chuyển;
- người có nhu cầu mua hoặc cấp phát.

Một trận lũ có thể vì vậy ảnh hưởng giá vũ khí.

Một cuộc chiến có thể làm thiếu nông cụ.

Một thợ giỏi chết có thể khiến cả settlement mất khả năng sản xuất một loại vật phẩm.

Kinh tế phải là một phần của **World Simulation**.

---

<a id="m18-t043-s004"></a>

## 2. KINH TẾ KHÔNG PHẢI MÔ PHỎNG MỌI HẠT GẠO

Game không cần simulation từng hạt lúa, từng con cá hay từng đồng tiền.

Thay vào đó sử dụng ba mức abstraction.

<a id="m18-t043-s005"></a>

#### Commodity Layer

Hàng hóa số lượng lớn:

- lương thực;
- gỗ;
- quặng;
- than;
- muối;
- vật liệu.

Được lưu theo **Lot / Stock**.

<a id="m18-t043-s006"></a>

#### Item Layer

Vật phẩm cá nhân:

- vũ khí;
- công cụ;
- thuyền nhỏ;
- đồ nghi lễ;
- đồ gia truyền.

Có identity riêng.

<a id="m18-t043-s007"></a>

#### Unique Object Layer

Vật có lịch sử đặc biệt:

- heirloom;
- masterpiece;
- artifact;
- historical object.

Có provenance đầy đủ.

Nhờ đó kinh tế vẫn sâu mà không trở thành database hàng triệu vật thể.

---

<a id="m18-t043-s008"></a>

## 3. BỐN TẦNG KINH TẾ

Toàn bộ hệ thống chia thành bốn quy mô.

<a id="m18-t043-s009"></a>

### Household Economy

Một hộ gia đình sản xuất và tiêu thụ gì?

<a id="m18-t043-s010"></a>

### Settlement Economy

Một settlement đang thừa và thiếu gì?

<a id="m18-t043-s011"></a>

### Regional Economy

Các settlement trao đổi với nhau ra sao?

<a id="m18-t043-s012"></a>

### Political Economy

Faction, chiến tranh, công trình lớn và quyền lực tác động kinh tế thế nào?

Player có thể tương tác ở cả bốn tầng.

---

<a id="m18-t043-s013"></a>

## 4. ECONOMY IS PHYSICAL

Resource phải có vị trí.

Nếu Bãi Lạc có surplus food nhưng tuyến sang Phong Châu bị chặn:

Phong Châu vẫn có thể thiếu food.

Không được:

> Global Food = 20,000 → mọi nơi dùng chung.

Location và logistics có ý nghĩa thật.

---

<a id="m18-t043-s014"></a>

## 5. ECONOMY IS TEMPORAL

Hàng hóa không xuất hiện đều quanh năm.

Có:

- mùa thu hoạch;
- mùa đánh cá;
- mùa dễ khai thác;
- thời gian chế tác;
- thời gian vận chuyển;
- thời gian xây dựng.

Do đó giá và scarcity thay đổi theo calendar.

---

<a id="m18-t043-s015"></a>

## 6. ECONOMY IS SOCIAL

Không phải mọi giao dịch là thị trường vô danh.

Một người có thể nhận food vì:

- là họ hàng;
- đang mắc nợ;
- cùng lineage;
- được chief hỗ trợ;
- có lời hứa;
- là guest.

Một người giàu resource nhưng bị cả làng ghét vẫn có thể khó mua một thứ quan trọng.

---

<a id="m18-t043-s016"></a>

## 7. ECONOMY IS POLITICAL

Control resource = power.

Ví dụ:

- ai kiểm soát water gate;
- ai kiểm soát mine;
- ai kiểm soát ferry;
- ai giữ grain storage;
- ai biết trade secret.

Economic system vì vậy nối trực tiếp Faction System.

---

<a id="m18-t043-s017"></a>

## 8. ECONOMIC VALUE KHÔNG PHẢI CHỈ LÀ PRICE

Một vật có nhiều loại value:

<a id="m18-t043-s018"></a>

#### Use Value

Dùng được vào việc gì?

<a id="m18-t043-s019"></a>

#### Exchange Value

Đổi được lấy gì?

<a id="m18-t043-s020"></a>

#### Scarcity Value

Hiếm tới đâu?

<a id="m18-t043-s021"></a>

#### Social Value

Ai sở hữu nó có địa vị gì?

<a id="m18-t043-s022"></a>

#### Emotional Value

Có ý nghĩa cá nhân không?

<a id="m18-t043-s023"></a>

#### Ritual Value

Có ý nghĩa nghi lễ không?

<a id="m18-t043-s024"></a>

#### Historical Value

Có gắn sự kiện không?

Một con dao cũ có thể Use Value thấp nhưng Family Value cực cao.

---

<a id="m18-t043-s025"></a>

## PART II — RESOURCE MODEL

<a id="m18-t043-s026"></a>

## 9. RESOURCE CATEGORIES

Economy sử dụng các nhóm chính:

<a id="m18-t043-s027"></a>

#### Food

Lương thực và thực phẩm.

<a id="m18-t043-s028"></a>

#### Natural Materials

Gỗ, sợi, đất, đá, cây thuốc.

<a id="m18-t043-s029"></a>

#### Metal Resources

Quặng, kim loại, scrap.

<a id="m18-t043-s030"></a>

#### Fuel

Than, củi và fuel production phù hợp.

<a id="m18-t043-s031"></a>

#### Construction Materials

Gỗ, đất, đá, vật liệu kết cấu.

<a id="m18-t043-s032"></a>

#### Craft Inputs

Mold material, binding, resin, leather-like material tùy research.

<a id="m18-t043-s033"></a>

#### Tools

Dụng cụ sản xuất.

<a id="m18-t043-s034"></a>

#### Transport Capacity

Boat, carrier capacity.

<a id="m18-t043-s035"></a>

#### Labor

Thời gian và năng lực con người.

<a id="m18-t043-s036"></a>

#### Knowledge

Technique và professional knowledge.

<a id="m18-t043-s037"></a>

#### Land / Resource Access

Quyền sử dụng ruộng, bến, rừng, mỏ.

---

<a id="m18-t043-s038"></a>

## 10. COMMODITY LOT

Commodity không được lưu thành 1.000 item riêng.

Ví dụ:

```text
Lot:
Dried Fish

Quantity:
12 Household-Day Equivalent

Quality:
Good

Origin:
Vạn Nước

Owner:
Nhà Triều

Age:
6 days
```

Quantity có thể dùng unit phù hợp từng commodity nhưng UI ưu tiên abstraction dễ hiểu.

---

<a id="m18-t043-s039"></a>

## 11. QUALITY

Commodity có quality.

Core levels:

- Poor;
- Usable;
- Good;
- Excellent;
- Exceptional.

Quality ảnh hưởng:

- hiệu quả sử dụng;
- shelf life;
- craft outcome;
- trade demand.

Không phải rarity.

---

<a id="m18-t043-s040"></a>

## 12. PERISHABILITY

Hàng hóa có:

Non-perishable.

Slow-perishable.

Perishable.

Highly-perishable.

Food spoilage tác động logistics.

Một settlement ven biển có surplus fish nhưng không nhất thiết xuất đi xa nếu không có preservation.

---

<a id="m18-t043-s041"></a>

## 13. PRESERVATION

Professional techniques có thể tăng shelf life:

- drying;
- smoking;
- salting;
- storage.

Preservation tạo Economic Value.

Một người giỏi bảo quản food có thể quan trọng ngang hunter.

---

<a id="m18-t043-s042"></a>

## 14. BULK & TRANSPORT COST

Hàng hóa có Bulk.

Ví dụ:

food grain:

giá trị trên khối lượng trung bình.

ore:

nặng.

prestige craft:

giá trị cao nhưng nhẹ.

Do đó trade route tự nhiên ưu tiên loại hàng khác nhau.

---

<a id="m18-t043-s043"></a>

## 15. OWNERSHIP

Mỗi resource quan trọng có owner context.

Possible owner:

- individual;
- household;
- lineage;
- workshop;
- settlement;
- faction;
- communal.

Không phải mọi thứ đều có private ownership hiện đại.

---

<a id="m18-t043-s044"></a>

## 16. ACCESS RIGHTS

Một resource có thể không thuộc riêng ai nhưng quyền sử dụng bị giới hạn.

Ví dụ:

- khu rừng săn;
- fishing ground;
- communal water;
- sacred forest;
- grazing area.

Player khai thác trái custom có thể tạo Social Event.

---

<a id="m18-t043-s045"></a>

## PART III — HOUSEHOLD ECONOMY

<a id="m18-t043-s046"></a>

## 17. HOUSEHOLD AS BASIC ECONOMIC UNIT

Household là đơn vị simulation kinh tế nhỏ nhất cần thiết.

Mỗi Household có:

```text
Members
Labor Capacity
Primary Profession
Secondary Profession
Food Production
Consumption
Tools
Property
Debt
Trade Links
Seasonal Obligations
Stored Resources
```

---

<a id="m18-t043-s047"></a>

## 18. HOUSEHOLD LABOR

Không phải mọi member có cùng khả năng.

Một household có thể có:

2 adult workers.

1 elder skilled craftsperson.

2 children.

Khi một adult chết:

economic capacity giảm thật.

---

<a id="m18-t043-s048"></a>

## 19. DEPENDENCY RATIO

Household có:

Productive Capacity

so với

Consumption Burden.

Một household nhiều child/elder có thể dễ bị shock hơn.

Không cần hiện con số chi tiết cho player.

---

<a id="m18-t043-s049"></a>

## 20. FAMILY PROFESSION EFFECT

Family tradition tạo lợi thế:

- tools;
- knowledge;
- customer;
- reputation;
- apprenticeship.

Con thợ đúc có access nghề tốt hơn người ngoài.

Nhưng không auto có skill.

---

<a id="m18-t043-s050"></a>

## 21. HOUSEHOLD PRODUCTION

Mỗi household có production profile.

Ví dụ Nhà Triều:

Primary:

fishing.

Secondary:

river transport.

Outputs:

fish.

boat service.

Inputs:

boat repair.

nets/tools.

food khác.

---

<a id="m18-t043-s051"></a>

## 22. HOUSEHOLD CONSUMPTION

Household tiêu thụ:

food.

fuel.

tools.

repair.

ritual/social expenses.

Không cần mô phỏng từng ngày khi stable.

Simulation chỉ cần aggregate.

---

<a id="m18-t043-s052"></a>

## 23. SURPLUS

Nếu production > need:

Surplus xuất hiện.

Có thể:

- store;
- trade;
- gift;
- tribute;
- invest.

---

<a id="m18-t043-s053"></a>

## 24. DEFICIT

Nếu need > production:

Household phải:

- dùng storage;
- trade;
- borrow;
- seek aid;
- change profession;
- migrate.

Economic pressure sinh narrative.

---

<a id="m18-t043-s054"></a>

## 25. HOUSEHOLD STORAGE

Storage giúp chống shock.

Một household giàu không chỉ có “money”.

Có thể giàu nhờ:

food store.

land.

boat.

tools.

network.

---

<a id="m18-t043-s055"></a>

## 26. HOUSEHOLD WEALTH

Wealth là multi-dimensional.

Một farmer có nhiều đất và food nhưng ít portable wealth.

Merchant có portable goods.

Smith có tools/workshop.

Leader có political access.

---

<a id="m18-t043-s056"></a>

## 27. HOUSEHOLD DEBT

Debt có thể sinh từ:

- failed harvest;
- wedding;
- medical crisis;
- boat repair;
- craft investment.

Debt tạo Story Thread tự nhiên.

---

<a id="m18-t043-s057"></a>

## 28. HOUSEHOLD INVESTMENT

Khi surplus đủ:

household có thể:

- mua tool;
- mở workshop;
- sửa thuyền;
- thuê labor;
- học nghề;
- mở land.

World tự phát triển.

---

<a id="m18-t043-s058"></a>

## 29. HOUSEHOLD ECONOMIC DECLINE

Nếu gặp nhiều shock:

- mất người lao động;
- mất property;
- debt;
- crop failure,

household có thể:

- bán tool;
- rời nghề;
- chuyển settlement;
- phụ thuộc lineage.

Không chỉ Wealth -10.

---

<a id="m18-t043-s059"></a>

## PART IV — SETTLEMENT ECONOMY

<a id="m18-t043-s060"></a>

## 30. SETTLEMENT ECONOMIC STATE

Mỗi settlement có:

```text
Population
Food Stock
Food Production
Craft Capacity
Labor Capacity
Material Stock
Trade Access
Storage Capacity
Transport Capacity
Wealth Distribution
Market Activity
Security
Infrastructure
```

---

<a id="m18-t043-s061"></a>

## 31. EXISTING SETTLEMENT VARIABLES

Module này dùng các variables đã khóa:

- Prosperity;
- Food;
- Population;
- Security;
- Health;
- Authority;
- Faith;
- Trade;
- Environment.

Nhưng chúng là high-level indicators.

Bên dưới có resource simulation.

---

<a id="m18-t043-s062"></a>

## 32. LOCAL PRODUCTION PROFILE

Mỗi settlement có Economic Identity.

Ví dụ:

<a id="m18-t043-s063"></a>

#### Lò Cốc

Metal Processing mạnh.

Food self-sufficiency thấp.

<a id="m18-t043-s064"></a>

#### Làng Dâu

Food surplus lớn.

Craft specialization thấp hơn.

<a id="m18-t043-s065"></a>

#### Bến Vạc

Transport + fishing mạnh.

<a id="m18-t043-s066"></a>

#### Vạn Muối

Salt production mạnh.

---

<a id="m18-t043-s067"></a>

## 33. ECONOMIC SPECIALIZATION

Specialization tạo lợi ích:

production efficiency.

knowledge concentration.

trade reputation.

Nhưng tăng vulnerability.

Lò Cốc phụ thuộc food import.

Nếu trade route đứt:

khủng hoảng nhanh.

---

<a id="m18-t043-s068"></a>

## 34. AUTARKY VS TRADE DEPENDENCE

Settlement tự cung cao:

ổn định nhưng ít specialization.

Settlement phụ thuộc trade:

prosperous hơn nhưng dễ shock.

Không có mô hình tối ưu duy nhất.

---

<a id="m18-t043-s069"></a>

## 35. SETTLEMENT STOCKPILE

Settlement có communal/private stock abstraction.

Ví dụ Food Reserve.

Normal:

4 tháng.

After flood:

1 tháng.

Player có thể biết tương đối tùy role.

---

<a id="m18-t043-s070"></a>

## 36. STORAGE INFRASTRUCTURE

Storage quyết:

spoilage.

resilience.

capacity.

Một settlement giàu food nhưng storage kém vẫn dễ mất mùa.

---

<a id="m18-t043-s071"></a>

## 37. ECONOMIC SHOCK

Core shocks:

- flood;
- drought;
- disease;
- war;
- route closure;
- fire;
- leadership crisis;
- mine collapse;
- loss of craft master.

---

<a id="m18-t043-s072"></a>

## 38. SHOCK PROPAGATION

Ví dụ:

Đồng Đỏ mine collapse.

↓

Ore supply giảm.

↓

Metal output giảm.

↓

Tool price tăng.

↓

Bãi Lạc khó thay nông cụ.

↓

Agricultural efficiency giảm.

Một sự kiện có thể tạo chain nhiều vùng.

---

<a id="m18-t043-s073"></a>

## 39. SETTLEMENT DEMAND

Demand đến từ:

Population.

Profession.

Construction.

War.

Season.

Culture.

Không phải random price fluctuation.

---

<a id="m18-t043-s074"></a>

## 40. CONSTRUCTION DEMAND

Cổ Loa có thể tạo massive demand:

- labor;
- food;
- wood;
- tools;
- metal;
- transport.

Điều này làm thay đổi economy quanh Loa Vực.

---

<a id="m18-t043-s075"></a>

## 41. SETTLEMENT LABOR POOL

Labor không vô hạn.

Nếu nhiều người bị huy động xây thành:

farm labor giảm.

Harvest có thể bị ảnh hưởng.

Đây là political-economic trade-off.

---

<a id="m18-t043-s076"></a>

## 42. LABOR SKILL MIX

100 laborer không thay được 10 skilled smith.

Settlement lưu:

General Labor.

Skilled Labor categories.

Loss of expert matters.

---

<a id="m18-t043-s077"></a>

## 43. UNEMPLOYMENT / UNDEREMPLOYMENT

Nếu nghề mất demand:

people phải:

- switch work;
- migrate;
- join military;
- become illicit actors.

Economic change tạo NPC arcs.

---

<a id="m18-t043-s078"></a>

## 44. MIGRATION ECONOMICS

People migrate vì:

- food;
- work;
- safety;
- land;
- debt.

Migration tăng labor nơi đến nhưng cũng tăng food/housing pressure.

---

<a id="m18-t043-s079"></a>

## PART V — MARKET & EXCHANGE

<a id="m18-t043-s080"></a>

## 45. NO UNIVERSAL SHOP INVENTORY

Merchant không spawn toàn bộ catalog.

Inventory đến từ:

local production.

trade arrivals.

personal network.

season.

---

<a id="m18-t043-s081"></a>

## 46. MARKET TYPES

Game có nhiều hình thức exchange.

<a id="m18-t043-s082"></a>

#### Household Exchange

Giao dịch trực tiếp.

<a id="m18-t043-s083"></a>

#### Local Market

Hàng phổ thông.

<a id="m18-t043-s084"></a>

#### Specialist Workshop

Professional item.

<a id="m18-t043-s085"></a>

#### Traveling Trader

Goods from elsewhere.

<a id="m18-t043-s086"></a>

#### Faction Supply

Restricted goods.

<a id="m18-t043-s087"></a>

#### Informal / Black Market

Illegal or sensitive trade.

---

<a id="m18-t043-s088"></a>

## 47. VALUE ABSTRACTION

Bên dưới simulation có thể dùng:

**Internal Value Unit**

để engine so sánh.

Nhưng player-facing không nhất thiết thấy “gold”.

Exact exchange media theo từng thời kỳ/khu vực được xác định ở historical-production pass.

---

<a id="m18-t043-s089"></a>

## 48. BARTER

Player có thể đổi:

food.

material.

service.

item.

favor.

Không cần mọi giao dịch hiện màn barter puzzle.

Routine transaction có thể convert qua relative value.

---

<a id="m18-t043-s090"></a>

## 49. REFERENCE VALUE

Market có Local Reference Value.

Ví dụ:

một tool được định giá tương đối theo:

food equivalent.

metal scarcity.

craft labor.

Player không cần tính từng hạt.

---

<a id="m18-t043-s091"></a>

## 50. PRICE FORMATION

Conceptual:

```text
Local Price =
Base Production Cost
× Scarcity
× Demand
× Transport Cost
× Risk
× Quality
× Seller Context
× Relationship
```

Không dùng random ±20%.

---

<a id="m18-t043-s092"></a>

## 51. SCARCITY

Scarcity phát sinh khi:

stock thấp.

production giảm.

route bị chặn.

demand tăng.

---

<a id="m18-t043-s093"></a>

## 52. TRANSPORT PREMIUM

Hàng xa có giá cao vì:

time.

boat/carrier.

risk.

food cho người vận chuyển.

---

<a id="m18-t043-s094"></a>

## 53. SECURITY PREMIUM

Route nguy hiểm:

trade cost tăng.

Nếu player làm route an toàn:

giá có thể giảm sau thời gian.

---

<a id="m18-t043-s095"></a>

## 54. SOCIAL PRICE

Người quen có thể:

cho giá tốt.

Người ghét player:

từ chối.

Nhưng merchant cần sống.

Không biến relationship thành coupon -30% mọi lúc.

---

<a id="m18-t043-s096"></a>

## 55. EMERGENCY PRICE

Trong famine:

food value tăng.

Nhưng custom/authority có thể:

cấm đầu cơ.

requisition.

ration.

Economy và Politics giao nhau.

---

<a id="m18-t043-s097"></a>

## 56. PRICE GOUGING

Player có thể lợi dụng scarcity.

Nhưng settlement nhớ.

Reputation có thể:

“kiếm lợi từ mùa đói”.

Không cấm mechanically.

---

<a id="m18-t043-s098"></a>

## 57. MARKET LIQUIDITY

Không phải merchant mua vô hạn.

Một village không thể mua:

50 expensive swords.

Demand/capital giới hạn.

---

<a id="m18-t043-s099"></a>

## 58. LARGE TRANSACTION

Muốn bán large commodity lot:

cần:

buyer.

market.

transport.

Không click Sell All ở tiny hamlet.

---

<a id="m18-t043-s100"></a>

## 59. BUY ORDER / REQUEST

NPC/workshop có thể có real demand.

Ví dụ:

Need 20 units charcoal.

Player có thể:

supply.

broker.

ignore.

Đây có thể tạo economic quest.

---

<a id="m18-t043-s101"></a>

## PART VI — CREDIT, DEBT & FINANCE

<a id="m18-t043-s102"></a>

## 60. CREDIT

Character có thể nhận goods trước, trả sau nếu:

Trust.

Reputation.

Collateral.

Relationship.

---

<a id="m18-t043-s103"></a>

## 61. CREDITWORTHINESS

Không có credit score universal.

Creditor đánh giá:

- family reputation;
- profession;
- assets;
- history;
- guarantees.

---

<a id="m18-t043-s104"></a>

## 62. GUARANTOR

Một third party có thể bảo lãnh.

Nếu player default:

guarantor chịu consequence.

Social Graph trở thành financial mechanism.

---

<a id="m18-t043-s105"></a>

## 63. COLLATERAL

Possible:

tool.

boat share.

land-use claim.

valuable item.

Không phải mọi collateral có thể bán ngay.

---

<a id="m18-t043-s106"></a>

## 64. DEFAULT

Nếu không trả:

Creditor có thể:

renegotiate.

claim collateral.

publicly shame.

seek authority.

create feud.

Không auto bounty.

---

<a id="m18-t043-s107"></a>

## 65. DEBT INHERITANCE

Tùy custom/contract:

một số debt có thể gắn household hơn cá nhân.

Exact inheritance rule theo region/family.

---

<a id="m18-t043-s108"></a>

## 66. FAVOR ECONOMY

Favor không convert trực tiếp thành Value Unit.

Một elder's favor có thể vô giá trong political context nhưng không mua food từ stranger.

---

<a id="m18-t043-s109"></a>

## PART VII — TRADE NETWORK

<a id="m18-t043-s110"></a>

## 67. TRADE PHILOSOPHY

Trade là:

> **dịch chuyển hàng hóa, con người, thông tin và rủi ro qua Route Network.**

Merchant gameplay không chỉ:

mua thấp → fast travel → bán cao.

---

<a id="m18-t043-s111"></a>

## 68. TRADE ROUTE

Mỗi route có:

Capacity.

Time.

Risk.

Season.

Political Control.

Transport Mode.

Tolls/Obligations.

Information Flow.

---

<a id="m18-t043-s112"></a>

## 69. TRADE FLOW

Settlement surplus tìm demand qua network.

Không cần mô phỏng từng trader trong background.

Aggregate flow xử lý phần lớn.

Named traders xuất hiện khi narrative relevance cao.

---

<a id="m18-t043-s113"></a>

## 70. TRANSPORT CAPACITY

Boat có economic importance lớn vì bulk cargo.

Một river route có thể cho phép trade volume cao hơn trail.

---

<a id="m18-t043-s114"></a>

## 71. CARGO

Player caravan/boat có:

Capacity.

Bulk.

Value.

Perishability.

Ownership.

Cargo choice tác động risk.

---

<a id="m18-t043-s115"></a>

## 72. HIGH-VALUE CARGO

Metal.

prestige goods.

weapons.

Có thể thu hút:

- theft;
- taxation;
- faction attention.

---

<a id="m18-t043-s116"></a>

## 73. LOW-VALUE BULK GOODS

Food.

wood.

Need high transport capacity.

Trade profit margin thấp nhưng social importance cao.

---

<a id="m18-t043-s117"></a>

## 74. TRADING AS PROFESSION

Merchant progression mở:

- supplier network;
- credit;
- market information;
- storage;
- contract;
- convoy coordination.

Không chỉ Charisma bonus.

---

<a id="m18-t043-s118"></a>

## 75. MARKET INFORMATION

Merchant không biết mọi price real-time.

Information có age.

Ví dụ:

> “Muối đang đắt ở Phong Châu — tin 12 ngày trước.”

Khi tới nơi:

situation có thể khác.

---

<a id="m18-t043-s119"></a>

## 76. INFORMATION ADVANTAGE

Trade Knowledge chính là lợi thế.

Player có thể:

- hỏi messenger;
- maintain contacts;
- use faction network.

---

<a id="m18-t043-s120"></a>

## 77. ARBITRAGE

Cho phép player kiếm lời từ chênh lệch giá.

Nhưng phải trả:

- time;
- transport;
- risk;
- spoilage;
- capital.

Không infinite instant arbitrage.

---

<a id="m18-t043-s121"></a>

## 78. ROUTE DISCOVERY

Tìm shortcut có thể:

giảm transport cost.

Tăng trade flow.

Settlement phát triển.

Exploration → Economy.

---

<a id="m18-t043-s122"></a>

## 79. TRADE ROUTE SECURITY

Nếu player diệt một bandit group:

không auto route safe forever.

Security cải thiện nếu:

threat source thật sự removed;

authority can maintain route.

---

<a id="m18-t043-s123"></a>

## 80. TOLLS / CONTRIBUTIONS

Faction controlling route có thể yêu cầu:

resource.

service.

permission.

Không nhất thiết coin toll.

---

<a id="m18-t043-s124"></a>

## 81. SMUGGLING

Smuggling xuất hiện khi:

- goods restricted;
- toll high;
- political border;
- secret technology.

Requires:

hidden route.

contacts.

stealth.

---

<a id="m18-t043-s125"></a>

## 82. BLACK MARKET

Black market không phải shop riêng.

Nó là network actor.

Access cần:

Trust.

Introduction.

Reputation.

---

<a id="m18-t043-s126"></a>

## 83. ILLEGAL GOODS

Không chỉ contraband item.

Có thể là:

- weapon during ban;
- sacred object;
- stolen property;
- restricted metal;
- political message.

---

<a id="m18-t043-s127"></a>

## 84. SMUGGLING CONSEQUENCE

Nếu phát hiện:

faction relation.

reputation.

confiscation.

debt.

Potential prison.

Không generic wanted stars.

---

<a id="m18-t043-s128"></a>

## PART VIII — PRODUCTION SYSTEM

<a id="m18-t043-s129"></a>

## 85. PRODUCTION PHILOSOPHY

Production không chỉ dành cho player.

Settlement economy dùng cùng logic abstraction.

Core transformation:

```text
Inputs
+ Labor
+ Tools
+ Knowledge
+ Time
+ Process
→ Outputs
+ Waste
+ Risk
```

---

<a id="m18-t043-s130"></a>

## 86. PRODUCTION CHAIN

Một sản phẩm có thể trải nhiều profession.

Ví dụ bronze spear:

Ore Extraction

→ Ore Selection

→ Smelting

→ Alloy Preparation

→ Mold Production

→ Casting

→ Finishing

→ Shaft Production

→ Assembly.

Không một character mặc định thành master toàn chain.

---

<a id="m18-t043-s131"></a>

## 87. PRODUCTION BOTTLENECK

Output bị giới hạn bởi weakest critical input.

Có ore nhưng thiếu fuel:

production giảm.

Có metal nhưng mất mold master:

quality giảm.

---

<a id="m18-t043-s132"></a>

## 88. WASTE

Crafting không 100% material-efficient.

Skill và process tốt giảm waste.

Xảo Nhân có thể cải tiến.

---

<a id="m18-t043-s133"></a>

## 89. BYPRODUCT

Một số process sinh byproduct có thể reuse.

Không cần quá chi tiết nếu không tạo gameplay.

---

<a id="m18-t043-s134"></a>

## 90. PRODUCTION SCALE

Có ba scale.

<a id="m18-t043-s135"></a>

#### Personal Craft

Một character.

<a id="m18-t043-s136"></a>

#### Workshop

Vài người/apprentices.

<a id="m18-t043-s137"></a>

#### Organized Production

Nhiều workshop/labor group.

Cổ Loa sử dụng lớp ba mạnh.

---

<a id="m18-t043-s138"></a>

## PART IX — CRAFTING SYSTEM

<a id="m18-t043-s139"></a>

## 91. CRAFTING PHILOSOPHY

Crafting không được biến thành:

> Loot 3 Bronze + 2 Wood → Click Craft Spear.

Crafting output được tạo từ:

**Design + Material + Components + Process + Tools + Skill + Knowledge + Time + Environment.**

---

<a id="m18-t043-s140"></a>

## 92. DESIGN

Design là knowledge object mô tả:

- cấu trúc;
- tỷ lệ;
- mục đích;
- process requirement.

Player phải:

- học;
- quan sát;
- reverse-engineer;
- phát triển.

---

<a id="m18-t043-s141"></a>

## 93. DESIGN VS RECIPE

Recipe nói:

“cần 2 X + 1 Y”.

Design nói:

> vật này phải giải quyết chức năng nào và cấu trúc ra sao.

Một design có thể dùng nhiều material variant.

---

<a id="m18-t043-s142"></a>

## 94. MATERIAL PROPERTIES

Material có thuộc tính chức năng.

Ví dụ conceptual:

- hardness;
- toughness;
- flexibility;
- weight;
- workability;
- durability.

Không cần player xem metallurgical spreadsheet mặc định.

---

<a id="m18-t043-s143"></a>

## 95. MATERIAL KNOWLEDGE

Character giỏi nghề có thể nhìn:

> “Thứ kim loại này giòn.”

Người không biết chỉ thấy:

> “Một mảnh kim loại.”

---

<a id="m18-t043-s144"></a>

## 96. MATERIAL SUBSTITUTION

Nếu thiếu material:

player có thể thay.

Outcome thay đổi.

Không phải recipe invalid ngay.

---

<a id="m18-t043-s145"></a>

## 97. COMPONENT SYSTEM

Complex item chia component.

Ví dụ bow:

body.

string.

binding.

finish.

Spear:

head.

shaft.

binding/socket.

---

<a id="m18-t043-s146"></a>

## 98. COMPONENT QUALITY

Final item quality không đơn giản bằng average.

Critical component failure có thể giới hạn toàn bộ.

Ví dụ:

excellent spearhead + bad shaft

→ weapon unreliable.

---

<a id="m18-t043-s147"></a>

## 99. TOOL REQUIREMENT

Một số craft cần:

basic tool.

specialized tool.

workshop infrastructure.

Player không đúc bronze hoàn chỉnh bên campfire.

---

<a id="m18-t043-s148"></a>

## 100. TOOL CONDITION

Tool hỏng:

precision giảm.

risk tăng.

Không cần repair sau mỗi use.

---

<a id="m18-t043-s149"></a>

## 101. WORKSPACE

Crafting environment quan trọng.

Possible:

Field.

Household workspace.

Professional workshop.

Specialized furnace.

Major construction site.

---

<a id="m18-t043-s150"></a>

## 102. FIELD CRAFT

Cho phép:

repair đơn giản.

improvised item.

basic medicine.

Không cho full professional manufacturing.

---

<a id="m18-t043-s151"></a>

## 103. WORKSHOP

Workshop có:

Facilities.

Tools.

Storage.

Workers.

Knowledge Tradition.

Reputation.

---

<a id="m18-t043-s152"></a>

## 104. WORKSHOP OWNERSHIP

Có thể thuộc:

family.

master.

lineage.

settlement.

faction.

Player không tự nhiên được dùng.

---

<a id="m18-t043-s153"></a>

## 105. WORKSHOP ACCESS

Qua:

profession.

relationship.

employment.

rental.

membership.

authority.

---

<a id="m18-t043-s154"></a>

## 106. CRAFTING TIME

Một số action:

minutes.

Một số:

hours.

Một số:

days.

Một con thuyền:

weeks/months.

Time thật sự chạy.

---

<a id="m18-t043-s155"></a>

## 107. CRAFTING TASK ABSTRACTION

Routine production được abstract.

Player chỉ trực tiếp ra decision khi:

- unusual material;
- difficult commission;
- innovation;
- failure;
- shortage.

---

<a id="m18-t043-s156"></a>

## 108. CRAFTING PROCESS STAGES

Một major craft có thể gồm:

Preparation.

Primary Process.

Finishing.

Inspection.

Không phải minigame reflex.

---

<a id="m18-t043-s157"></a>

## 109. CRAFTING CHECK

Check chỉ khi:

quality uncertain.

process difficult.

material unusual.

time pressure.

Routine work của expert auto-success.

---

<a id="m18-t043-s158"></a>

## 110. CRAFTING OUTCOME

Không chỉ Success/Fail.

Possible:

<a id="m18-t043-s159"></a>

#### Failed

Không usable.

<a id="m18-t043-s160"></a>

#### Flawed

Dùng được nhưng có defect.

<a id="m18-t043-s161"></a>

#### Serviceable

Đạt yêu cầu.

<a id="m18-t043-s162"></a>

#### Good

Above normal.

<a id="m18-t043-s163"></a>

#### Excellent

High craft quality.

<a id="m18-t043-s164"></a>

#### Exceptional

Rare result có đặc điểm đặc biệt.

---

<a id="m18-t043-s165"></a>

## 111. FLAW

Flaw phải cụ thể.

Ví dụ:

- poorly balanced;
- brittle section;
- loose binding;
- heavy;
- uneven edge.

Không generic:

Quality -10.

---

<a id="m18-t043-s166"></a>

## 112. EXCEPTIONAL ITEM

Không tự thành magical.

Có thể:

lighter.

perfectly fitted.

unusually durable.

excellent balance.

historically significant later.

---

<a id="m18-t043-s167"></a>

## 113. CRAFTING AND FIT

Player có thể làm custom-fit item cho character.

Cần:

measure/use feedback.

time.

Relationship nếu là commission.

---

<a id="m18-t043-s168"></a>

## 114. CRAFTING FAILURE LEARNING

Failure tạo Insight nếu:

character hiểu nguyên nhân.

Một failed casting có thể giúp progression nghề.

---

<a id="m18-t043-s169"></a>

## 115. CRAFTING ACCIDENT

Chỉ trong:

risky process.

bad condition.

time pressure.

Không random injury để gây khó chịu.

---

<a id="m18-t043-s170"></a>

## 116. EXPERIMENTATION

Player có thể thử:

new ratio.

new shape.

new process.

new component.

Experiment consumes resource/time.

---

<a id="m18-t043-s171"></a>

## 117. EXPERIMENT RESULT

Could:

fail.

produce unusual result.

reveal principle.

create prototype.

---

<a id="m18-t043-s172"></a>

## 118. INNOVATION

Innovation cần:

Need.

Knowledge A.

Knowledge B.

Experiment.

Resources.

Real-world validation.

Không unlock bằng tech point.

---

<a id="m18-t043-s173"></a>

## 119. PROTOTYPE

Prototype chưa auto thành mass production.

Need:

repeatability.

training.

tools.

standardization.

---

<a id="m18-t043-s174"></a>

## 120. STANDARDIZATION

Một innovation được standardize khi:

- design stable;
- process documented/transmitted;
- tools available;
- workers trained;
- quality predictable.

Cổ Loa arc phụ thuộc cực mạnh.

---

<a id="m18-t043-s175"></a>

## 121. MASS PRODUCTION

Không phải factory hiện đại.

Trong game nó nghĩa:

nhiều worker/workshop dùng process tương đối chuẩn.

Output tăng.

Variation giảm.

---

<a id="m18-t043-s176"></a>

## 122. STANDARDIZATION TRADE-OFF

Ưu:

volume.

repairability.

training.

supply.

Nhược:

less customization.

traditional masters may resist.

Political/economic conflict phát sinh.

---

<a id="m18-t043-s177"></a>

## 123. CRAFTING KNOWLEDGE PROVENANCE

Technique có nguồn.

Ví dụ:

`Khuôn Hai Mảnh — học từ Cấn Lửa`.

Nếu truyền ra ngoài:

Family/Profession network phản ứng.

---

<a id="m18-t043-s178"></a>

## 124. TRADE SECRET

Một số process được bảo vệ.

Player có thể:

earn access.

marry into family.

steal.

reverse engineer.

---

<a id="m18-t043-s179"></a>

## 125. KNOWLEDGE THEFT

Không chỉ lấy một recipe scroll.

Có thể cần:

observe.

kidnap worker.

bribe apprentice.

steal mold.

---

<a id="m18-t043-s180"></a>

## 126. KNOWLEDGE LOSS

Master chết không truyền:

technique biến mất khỏi active production.

World quality giảm.

---

<a id="m18-t043-s181"></a>

## PART X — REPAIR, MAINTENANCE & SALVAGE

<a id="m18-t043-s182"></a>

## 127. REPAIR PHILOSOPHY

Repair phải quan trọng hơn endless replacement.

Một item tốt có thể sống lâu.

---

<a id="m18-t043-s183"></a>

## 128. REPAIR TYPES

<a id="m18-t043-s184"></a>

#### Maintenance

Routine.

<a id="m18-t043-s185"></a>

#### Minor Repair

Binding, sharpening.

<a id="m18-t043-s186"></a>

#### Structural Repair

Replace component.

<a id="m18-t043-s187"></a>

#### Rebuild

Item gần như tái chế lại.

---

<a id="m18-t043-s188"></a>

## 129. REPAIR QUALITY

Poor repair có thể:

usable now.

lower longevity.

Good repair:

restore full functionality.

---

<a id="m18-t043-s189"></a>

## 130. SALVAGE

Broken item cho:

components.

material.

information.

Một bronze head quý có thể tái dùng.

---

<a id="m18-t043-s190"></a>

## 131. RECYCLING ECONOMY

Metal có value cao vì có thể recast/reuse.

War battlefield có salvage value.

Nhưng ownership/moral issues tồn tại.

---

<a id="m18-t043-s191"></a>

## 132. SALVAGE RIGHTS

Ai có quyền lấy đồ sau battle?

Army?

family?

victor?

local settlement?

Context matters.

---

<a id="m18-t043-s192"></a>

## PART XI — COMMISSION & PROFESSIONAL WORK

<a id="m18-t043-s193"></a>

## 133. COMMISSION

Client có thể yêu cầu:

specific function.

deadline.

budget.

material.

symbolic requirement.

---

<a id="m18-t043-s194"></a>

## 134. COMMISSION NEGOTIATION

Player có thể negotiate:

payment.

deadline.

quality.

material responsibility.

---

<a id="m18-t043-s195"></a>

## 135. CLIENT EXPECTATION

Một leader cần ceremonial dagger khác hunter cần practical spear.

Craft gameplay phản ánh context.

---

<a id="m18-t043-s196"></a>

## 136. FAILED COMMISSION

Possible:

- client accepts discount;
- reputation drops;
- redo;
- dispute;
- debt.

Không chỉ item deleted.

---

<a id="m18-t043-s197"></a>

## 137. MASTER COMMISSION

High-reputation craftsman được giao:

- rare item;
- political gift;
- ritual object;
- military prototype.

Career opens narrative.

---

<a id="m18-t043-s198"></a>

## 138. CUSTOM ORDER

Player cũng có thể thuê NPC craft.

Need:

relationship.

material.

time.

money/resource.

---

<a id="m18-t043-s199"></a>

## 139. WAITING FOR CRAFT

World time runs.

Player có thể đi làm việc khác.

Không cần đứng cạnh workshop.

---

<a id="m18-t043-s200"></a>

## PART XII — APPRENTICES & WORKSHOP ORGANIZATION

<a id="m18-t043-s201"></a>

## 140. APPRENTICE LABOR

Apprentice:

- contributes production;
- learns;
- can make mistakes.

Không phải free worker.

---

<a id="m18-t043-s202"></a>

## 141. TASK DELEGATION

Master chooses:

simple work.

critical work.

training work.

Risk khác nhau.

---

<a id="m18-t043-s203"></a>

## 142. OVERWORK

Workshop chạy quá sức:

fatigue.

accident risk.

quality decline.

Relationship problem.

---

<a id="m18-t043-s204"></a>

## 143. WORKSHOP CULTURE

Một workshop có:

strict.

experimental.

family-centered.

commercial.

Culture affect apprentice psychology/career.

---

<a id="m18-t043-s205"></a>

## 144. WORKSHOP SUCCESSION

Master retires/dies.

Possible successors:

child.

apprentice.

co-owner.

faction appointee.

Conflict can occur.

---

<a id="m18-t043-s206"></a>

## 145. PLAYER-OWNED WORKSHOP

Possible late enough in career.

Player manages at abstraction level:

Focus.

Workers.

Contracts.

Investment.

Không cần city-builder micromanagement.

---

<a id="m18-t043-s207"></a>

## 146. WORKSHOP POLICY

Player can emphasize:

Quality.

Volume.

Innovation.

Training.

Profit.

Each has trade-off.

---

<a id="m18-t043-s208"></a>

## 147. WORKSHOP AS SOCIAL HUB

Customers.

apprentices.

suppliers.

rivals.

Faction agents.

Workshop naturally generates stories.

---

<a id="m18-t043-s209"></a>

## PART XIII — SETTLEMENT INTERACTION

<a id="m18-t043-s210"></a>

## 148. SETTLEMENT IS NOT JUST A SHOP HUB

Khi player vào settlement, họ có thể tương tác với:

- households;
- professions;
- market;
- authority;
- ritual spaces;
- storage;
- infrastructure;
- factions.

Settlement phải có economic life.

---

<a id="m18-t043-s211"></a>

## 149. SETTLEMENT ACCESS

Player access phụ thuộc:

reputation.

relationship.

faction.

time.

Một outsider không tự nhiên vào mọi kho/workshop.

---

<a id="m18-t043-s212"></a>

## 150. HOSPITALITY

Có thể ở:

family house.

guest house/context equivalent.

camp outside.

professional contact.

Hospitality có social meaning.

---

<a id="m18-t043-s213"></a>

## 151. LODGING

Không cần inn fantasy ở mọi làng.

Accommodation đến từ:

relationship.

payment.

community.

camp.

---

<a id="m18-t043-s214"></a>

## 152. STORAGE

Player có thể gửi đồ tại:

family.

trusted household.

rented store.

workshop.

Settlement storage có ownership.

---

<a id="m18-t043-s215"></a>

## 153. STORAGE RISK

Trusted household:

low theft.

war/flood:

still risk.

Không magical global stash.

---

<a id="m18-t043-s216"></a>

## 154. LOCAL SERVICES

Settlement services phụ thuộc profession population.

Ví dụ:

no healer → medical service hạn chế.

no smith → repair metal khó.

---

<a id="m18-t043-s217"></a>

## 155. SERVICE QUALITY

NPC skill thật quyết quality.

Không generic vendor tier.

---

<a id="m18-t043-s218"></a>

## 156. SETTLEMENT PROJECT

Player có thể hỗ trợ project:

bridge.

ferry.

storage.

workshop.

water channel.

defense.

---

<a id="m18-t043-s219"></a>

## 157. PROJECT REQUIREMENTS

Major project cần:

Design.

Materials.

Labor.

Authority.

Time.

Maintenance Plan.

Không chỉ pay gold.

---

<a id="m18-t043-s220"></a>

## 158. PROJECT STAKEHOLDERS

Ai được lợi?

Ai mất?

Ví dụ bridge:

traders lợi.

ferryman household mất income.

Project tạo social consequence.

---

<a id="m18-t043-s221"></a>

## 159. PUBLIC WORKS

Một leader có thể tổ chức.

Player Thủ Lĩnh/Xảo Nhân mạnh.

But needs legitimacy.

---

<a id="m18-t043-s222"></a>

## 160. INFRASTRUCTURE EFFECT

Bridge:

travel time giảm.

Trade ↑.

Migration ↑.

Bandit route may shift.

Không cosmetic.

---

<a id="m18-t043-s223"></a>

## 161. FERRY PROJECT

Một ferry mới:

route opens.

Family may gain profession.

Settlement trade changes.

---

<a id="m18-t043-s224"></a>

## 162. STORAGE PROJECT

Granary/storage:

Food resilience tăng.

But:

someone controls store.

Political power changes.

---

<a id="m18-t043-s225"></a>

## 163. WATER PROJECT

Canal/dyke:

production tăng.

Environmental cost possible.

Water politics shift.

---

<a id="m18-t043-s226"></a>

## 164. WORKSHOP PROJECT

New workshop:

craft capacity.

jobs.

resource demand.

professional faction.

---

<a id="m18-t043-s227"></a>

## 165. DEFENSIVE PROJECT

Wall/ditch/watchpost:

security ↑.

labor/material cost.

Trade access may slow.

---

<a id="m18-t043-s228"></a>

## PART XIV — PLAYER PROPERTY

<a id="m18-t043-s229"></a>

## 166. PLAYER CAN OWN PROPERTY

Possible:

house.

workshop.

boat.

storage.

land-use right.

Not mandatory.

---

<a id="m18-t043-s230"></a>

## 167. PROPERTY IS LOCATION-SPECIFIC

House in Bến Vạc does not give storage in Phong Châu.

---

<a id="m18-t043-s231"></a>

## 168. PROPERTY MAINTENANCE

Routine maintenance abstracted.

Only becomes gameplay when:

neglect.

damage.

war.

flood.

---

<a id="m18-t043-s232"></a>

## 169. PROPERTY OCCUPATION

If player travels years:

family/apprentice may use property.

Not frozen empty building.

---

<a id="m18-t043-s233"></a>

## 170. PROPERTY CONFLICT

Inheritance.

debt.

land claim.

political requisition.

Story potential.

---

<a id="m18-t043-s234"></a>

## 171. LAND RIGHTS

Avoid modern absolute land ownership assumptions.

Game can model:

Use Right.

Family Claim.

Communal Right.

Authority Grant.

Exact cultural implementation researched later.

---

<a id="m18-t043-s235"></a>

## 172. LAND DISPUTE

Can involve:

old boundary.

water.

inheritance.

migration.

Political development.

---

<a id="m18-t043-s236"></a>

## PART XV — PLAYER ECONOMIC GAMEPLAY

<a id="m18-t043-s237"></a>

## 173. ECONOMIC PLAYSTYLE

A player can build career around:

- craft;
- trade;
- logistics;
- workshop;
- settlement projects.

Without becoming combat hero.

---

<a id="m18-t043-s238"></a>

## 174. MERCHANT PLAYTHROUGH

Possible progression:

small carrier.

↓

trusted trader.

↓

route broker.

↓

storage owner.

↓

regional trade coordinator.

↓

political-economic actor.

---

<a id="m18-t043-s239"></a>

## 175. CRAFTSMAN PLAYTHROUGH

Apprentice.

↓

practitioner.

↓

master.

↓

workshop head.

↓

innovator.

↓

legacy school.

---

<a id="m18-t043-s240"></a>

## 176. LOGISTICS PLAYTHROUGH

Boatman.

↓

carrier.

↓

convoy organizer.

↓

route authority.

↓

wartime logistics leader.

---

<a id="m18-t043-s241"></a>

## 177. PLAYER DOES NOT NEED TO BECOME TYCOON

A simple respected master craftsperson is valid endgame.

---

<a id="m18-t043-s242"></a>

## PART XVI — ECONOMIC RISK & CRIME

<a id="m18-t043-s243"></a>

## 178. THEFT

Items have ownership.

Stealing creates:

Evidence.

Witness.

Reputation.

Not automatic detection.

---

<a id="m18-t043-s244"></a>

## 179. FENCING STOLEN GOODS

Recognizable items harder to sell.

Black market network needed.

---

<a id="m18-t043-s245"></a>

## 180. COUNTERFEIT / LOW-QUALITY GOODS

Player/NPC can misrepresent quality.

Deception + appraisal gameplay.

---

<a id="m18-t043-s246"></a>

## 181. FRAUD

Examples:

fake material.

short measure.

false provenance.

broken promise.

Economic crime uses Social/Knowledge systems.

---

<a id="m18-t043-s247"></a>

## 182. BANDITRY ECONOMY

Bandits need:

food.

fence.

safe base.

information.

They are not infinite spawn.

Destroy support network can reduce threat more effectively than kill patrols.

---

<a id="m18-t043-s248"></a>

## 183. RAID

Faction can raid:

grain.

boats.

metal.

Not purely military territory control.

---

<a id="m18-t043-s249"></a>

## PART XVII — WAR ECONOMY

<a id="m18-t043-s250"></a>

## 184. MOBILIZATION

War consumes:

Food.

Weapons.

Labor.

Transport.

---

<a id="m18-t043-s251"></a>

## 185. REQUISITION

Authority may take:

grain.

boat.

metal.

labor.

Player can:

comply.

negotiate.

hide resources.

resist.

---

<a id="m18-t043-s252"></a>

## 186. CIVILIAN COST

Military production can reduce:

farm labor.

craft availability.

household security.

---

<a id="m18-t043-s253"></a>

## 187. WAR DEMAND

Weapon demand rises.

But tools may become scarce.

Economic trade-off.

---

<a id="m18-t043-s254"></a>

## 188. REFUGEE ECONOMY

Refugees:

increase labor.

increase food need.

bring skills.

bring knowledge.

Not just Population +X.

---

<a id="m18-t043-s255"></a>

## 189. POST-WAR ECONOMY

After conflict:

weapon demand falls.

repair demand rises.

labor shortage.

widowed households.

salvage.

reconstruction.

---

<a id="m18-t043-s256"></a>

## PART XVIII — SEASONAL ECONOMY

<a id="m18-t043-s257"></a>

## 190. AGRICULTURAL CYCLE

Labor allocation changes:

planting.

maintenance.

harvest.

processing.

Player asking farmers to leave during harvest has higher cost.

---

<a id="m18-t043-s258"></a>

## 191. FISHING CYCLE

Fishing output varies by:

season.

water.

weather.

Professional Knowledge.

---

<a id="m18-t043-s259"></a>

## 192. FORESTRY & GATHERING

Availability changes by season.

---

<a id="m18-t043-s260"></a>

## 193. TRADE SEASON

Some routes easier in certain periods.

Market supply changes.

---

<a id="m18-t043-s261"></a>

## 194. SEASONAL PRICE

Price movements should be predictable to experts.

Not random.

Merchant Knowledge gives forecast.

---

<a id="m18-t043-s262"></a>

## PART XIX — FACTION ECONOMY

<a id="m18-t043-s263"></a>

## 195. FACTION ECONOMIC BASE

Every meaningful faction should know:

What sustains it?

Food?

Trade?

Land?

Craft?

Tribute?

---

<a id="m18-t043-s264"></a>

## 196. ECONOMIC VULNERABILITY

A faction dependent on one route has strategic weakness.

Player can:

protect.

exploit.

negotiate.

---

<a id="m18-t043-s265"></a>

## 197. ECONOMIC POWER

Faction may influence through:

loan.

food distribution.

work access.

not only soldiers.

---

<a id="m18-t043-s266"></a>

## 198. PROFESSIONAL NETWORK AS FACTION

Smith network can:

control knowledge.

set standards.

refuse work.

support politician.

---

<a id="m18-t043-s267"></a>

## 199. ECONOMIC SANCTION

Faction can:

deny access.

stop trade.

refuse service.

More interesting than immediate war.

---

<a id="m18-t043-s268"></a>

## PART XX — CỔ LOA ECONOMIC TRANSFORMATION

<a id="m18-t043-s269"></a>

## 200. LOA VỰC AS ECONOMIC CASE STUDY

Loa Vực must visibly transform through economy.

Early:

farm households.

wetlands.

small craft.

Later:

survey.

labor camps.

supply depots.

specialized workshops.

military production.

---

<a id="m18-t043-s270"></a>

## 201. LABOR DEMAND

Construction draws labor from nearby settlements.

Consequences:

wage/opportunity ↑.

farm labor ↓.

migration ↑.

housing pressure ↑.

---

<a id="m18-t043-s271"></a>

## 202. FOOD DEMAND

Large worker population requires imported food.

Bãi Lạc becomes more strategically important.

---

<a id="m18-t043-s272"></a>

## 203. WOOD DEMAND

Construction and fuel increase pressure on Tản Viên/Lâm U.

Environmental tension.

---

<a id="m18-t043-s273"></a>

## 204. METAL DEMAND

Đồng Đỏ gains political importance.

Smith networks gain leverage.

---

<a id="m18-t043-s274"></a>

## 205. TRANSPORT DEMAND

Tam Giang boatmen become critical.

Trade routes expand.

---

<a id="m18-t043-s275"></a>

## 206. STANDARDIZATION DEMAND

Large military/state project creates incentive:

uniform component.

repeatable design.

worker training.

---

<a id="m18-t043-s276"></a>

## 207. SOCIAL CONSEQUENCE

Some families become rich.

Some lose land.

Some change profession.

Some oppose state.

Cổ Loa must feel like economic transformation, not scenery change.

---

<a id="m18-t043-s277"></a>

## PART XXI — DETAILED EXAMPLE: BRONZE SPEAR SUPPLY CHAIN

<a id="m18-t043-s278"></a>

## 208. STAGE 1 — ORE

Mỏ Sương Đỏ produces ore.

Output depends:

labor.

mine condition.

tools.

---

<a id="m18-t043-s279"></a>

## 209. STAGE 2 — TRANSPORT

Ore moves to Lò Cốc.

Need:

carrier.

route.

food.

---

<a id="m18-t043-s280"></a>

## 210. STAGE 3 — FUEL

Suối Than produces fuel.

Bad weather reduces output.

---

<a id="m18-t043-s281"></a>

## 211. STAGE 4 — SMELTING

Requires:

Ore Knowledge.

Fuel.

Furnace.

Skilled labor.

---

<a id="m18-t043-s282"></a>

## 212. STAGE 5 — CASTING

Làng Khuôn supplies mold expertise.

If master dies:

quality/throughput drops.

---

<a id="m18-t043-s283"></a>

## 213. STAGE 6 — SHAFT

Wood arrives from another region/local forest depending design.

---

<a id="m18-t043-s284"></a>

## 214. STAGE 7 — ASSEMBLY

Professional craft.

Final fit depends purpose.

---

<a id="m18-t043-s285"></a>

## 215. STAGE 8 — DISTRIBUTION

Weapon goes to:

hunter.

militia.

faction.

trade.

---

<a id="m18-t043-s286"></a>

## 216. SUPPLY SHOCK EXAMPLE

Bandits block charcoal route.

↓

Fuel shortage.

↓

Metal output down.

↓

Tool + weapon price rises.

↓

Authority redirects production to weapon.

↓

Farmers cannot replace tools.

↓

Agricultural pressure next season.

This is the level of systemic connection desired.

---

<a id="m18-t043-s287"></a>

## PART XXII — DETAILED EXAMPLE: PLAYER AS SMITH

<a id="m18-t043-s288"></a>

## 217. START

Player generated as:

Cấn family apprentice.

Access:

workshop.

basic knowledge.

low authority.

---

<a id="m18-t043-s289"></a>

## 218. EARLY PLAY

Player performs:

repair.

simple casting.

material sorting.

Routine work abstracted.

---

<a id="m18-t043-s290"></a>

## 219. FIRST CAREER CRISIS

Master receives rare commission.

Player is trusted with critical mold.

Choices:

follow known method.

experiment.

ask rival workshop.

---

<a id="m18-t043-s291"></a>

## 220. FAILURE

Mold cracks.

Material partially lost.

Player can:

hide mistake.

admit.

repair.

Each creates different relationship event.

---

<a id="m18-t043-s292"></a>

## 221. MID CAREER

Player becomes practitioner.

Can accept own commission.

Needs:

material source.

workshop access.

clients.

---

<a id="m18-t043-s293"></a>

## 222. ECONOMIC PRESSURE

War raises metal price.

Player can:

serve military.

prioritize farmers.

raise prices.

ration output.

No universal correct answer.

---

<a id="m18-t043-s294"></a>

## 223. LATE CAREER

Player becomes Master/Xảo Nhân.

Can:

train apprentices.

create process.

join Cổ Loa production.

resist standardization.

---

<a id="m18-t043-s295"></a>

## 224. LEGACY

Technique spreads.

Workshop survives after player death.

Economic history preserves contribution.

---

<a id="m18-t043-s296"></a>

## PART XXIII — DETAILED EXAMPLE: PLAYER AS TRADER

<a id="m18-t043-s297"></a>

## 225. START

Character owns no large capital.

Carries small goods between Bến Vạc and Phong Châu.

---

<a id="m18-t043-s298"></a>

## 226. KNOWLEDGE ADVANTAGE

Learns:

Bến Vạc fish surplus.

Phong Châu demand.

Profit exists.

---

<a id="m18-t043-s299"></a>

## 227. LIMITATION

Walking capacity low.

Player partners with boat family.

Creates relationship/debt.

---

<a id="m18-t043-s300"></a>

## 228. FIRST SHOCK

Flood blocks route.

Goods spoil risk.

Choices:

sell locally cheap.

preserve.

find alternate route.

wait.

---

<a id="m18-t043-s301"></a>

## 229. NETWORK GROWTH

Player gains:

supplier.

buyer.

boat access.

credit.

---

<a id="m18-t043-s302"></a>

## 230. MID CAREER

Player can coordinate larger cargo.

But:

route security.

debt.

political tolls

become important.

---

<a id="m18-t043-s303"></a>

## 231. POLITICAL INVOLVEMENT

A faction requests player stop trading with rival settlement.

Choices:

comply.

smuggle.

negotiate exemption.

---

<a id="m18-t043-s304"></a>

## 232. LATE CAREER

Player may influence:

routes.

market information.

faction supply.

Trade becomes political power.

---

<a id="m18-t043-s305"></a>

## PART XXIV — DETAILED EXAMPLE: SETTLEMENT PROJECT

<a id="m18-t043-s306"></a>

## 233. PROBLEM

Bến Nếp loses ferry after flood.

Trade slows.

---

<a id="m18-t043-s307"></a>

## 234. OPTION A — REBUILD FERRY

Requirements:

boat craft.

wood.

labor.

operator household.

Effects:

cheap.

fast.

creates ferry profession.

---

<a id="m18-t043-s308"></a>

## 235. OPTION B — BUILD BRIDGE

Higher:

material.

labor.

time.

Requires Xảo Nhân/authority.

Effects:

more reliable.

changes transport.

---

<a id="m18-t043-s309"></a>

## 236. STAKEHOLDER CONFLICT

Old ferry household prefers ferry.

Merchants prefer bridge.

Local authority considers cost.

---

<a id="m18-t043-s310"></a>

## 237. PLAYER CHOICE

Could:

support ferry.

bridge.

temporary raft.

do nothing.

---

<a id="m18-t043-s311"></a>

## 238. LONG-TERM

Bridge may increase trade.

Settlement grows.

Ferry family changes profession or declines.

Infrastructure improvement has human cost.

---

<a id="m18-t043-s312"></a>

## PART XXV — ECONOMY & QUEST SYSTEM

<a id="m18-t043-s313"></a>

## 239. ECONOMIC QUEST GENERATION

Quest can emerge when:

Demand > Supply threshold.

Resource access blocked.

Contract fails.

Debt overdue.

Workshop loses expert.

---

<a id="m18-t043-s314"></a>

## 240. NO FETCH QUEST WITHOUT CAUSE

“Healer needs herbs” exists because:

medicine stock low.

Not because template selected herb quest.

---

<a id="m18-t043-s315"></a>

## 241. SOLUTION VARIETY

Medicine shortage:

gather herbs.

trade.

find substitute.

reduce demand.

bring another healer.

---

<a id="m18-t043-s316"></a>

## 242. QUEST OUTCOME UPDATES ECONOMY

If player supplies herbs:

stock rises.

Not just quest completed.

---

<a id="m18-t043-s317"></a>

## PART XXVI — ECONOMY & CHARACTER SYSTEMS

<a id="m18-t043-s318"></a>

## 243. ATTRIBUTES

THỂ affects:

heavy labor.

transport.

XẢO:

fine craft.

GIÁC:

quality inspection.

TRÍ:

process/problem solving.

TÂM:

teaching/customer understanding.

KHÍ:

leadership/negotiation.

---

<a id="m18-t043-s319"></a>

## 244. SKILLS

Skill defines competence.

No generic “Craft” skill.

Professional skills remain distinct.

---

<a id="m18-t043-s320"></a>

## 245. PROFESSION

Profession determines:

economic role.

network.

career opportunity.

---

<a id="m18-t043-s321"></a>

## 246. CLASS

Class changes method.

Xảo Nhân:

innovation.

Sứ Giả:

trade negotiation.

Thủ Lĩnh:

organization.

But merchant/craftsman does not need class to work.

---

<a id="m18-t043-s322"></a>

## 247. PARTY

Party can distribute:

cargo.

tasks.

contacts.

Not all companions willing to become labor.

---

<a id="m18-t043-s323"></a>

## 248. INJURY

Smith hand injury affects production.

Boatman leg injury affects transport.

Economic consequence makes injury meaningful.

---

<a id="m18-t043-s324"></a>

## 249. REPUTATION

Professional reputation affects:

commission.

credit.

quality expectations.

---

<a id="m18-t043-s325"></a>

## 250. FACTION

Faction controls:

resource.

route.

contracts.

---

<a id="m18-t043-s326"></a>

## 251. TRAVEL

Transport time creates actual economic geography.

---

<a id="m18-t043-s327"></a>

## 252. DUNGEON

Mine/workshop/storage/battlefield can all become dungeon contexts with economic meaning.

---

<a id="m18-t043-s328"></a>

## 253. QUEST

Economic pressure becomes Story Thread.

---

<a id="m18-t043-s329"></a>

## PART XXVII — ECONOMIC UI

<a id="m18-t043-s330"></a>

## 254. PLAYER ECONOMY UI

Do not show giant stock-market dashboard by default.

Player sees:

- carried goods;
- property;
- debt;
- current local prices;
- known trade information;
- contracts.

---

<a id="m18-t043-s331"></a>

## 255. PRICE DESCRIPTION

Can display numeric exchange value for usability.

But accompany context:

> High — grain shortage.

> Low — recent fishing surplus.

---

<a id="m18-t043-s332"></a>

## 256. PRICE HISTORY

Merchant-skilled character may see:

usual range.

trend.

---

<a id="m18-t043-s333"></a>

## 257. SUPPLY INFORMATION

Generic character:

> “Salt seems expensive.”

Trader:

> “Price is nearly twice what it was last season because coastal shipments stopped.”

Capability changes UI.

---

<a id="m18-t043-s334"></a>

## 258. SETTLEMENT ECONOMY UI

Player sees only what they know.

Basic:

Prosperous / struggling.

Expert:

food reserve.

trade dependency.

production bottleneck.

---

<a id="m18-t043-s335"></a>

## 259. WORKSHOP UI

Shows:

Current Work.

Workers.

Material.

Tool Condition.

Orders.

Risks.

Not hundreds of sliders.

---

<a id="m18-t043-s336"></a>

## 260. CRAFTING UI

Player selects:

Design.

Material.

Components.

Quality Target.

Time/Process.

Relevant Technique.

System explains likely trade-off.

---

<a id="m18-t043-s337"></a>

## 261. NO UNKNOWN STAT TRAP

If material substitution will make spear brittle and character knows that:

UI warns.

If character lacks knowledge:

may not know.

---

<a id="m18-t043-s338"></a>

## PART XXVIII — SIMULATION ARCHITECTURE

<a id="m18-t043-s339"></a>

## 262. FULL SIMULATION IS NOT REQUIRED EVERYWHERE

Use Simulation Tiers.

<a id="m18-t043-s340"></a>

#### Tier 1 — Active Settlement

Detailed stocks and actors.

<a id="m18-t043-s341"></a>

#### Tier 2 — Relevant Region

Aggregate production/trade.

<a id="m18-t043-s342"></a>

#### Tier 3 — Distant World

Coarse trend simulation.

---

<a id="m18-t043-s343"></a>

## 263. PROMOTION

When player travels to distant settlement:

system expands coarse state into detailed believable state.

---

<a id="m18-t043-s344"></a>

## 264. DETERMINISTIC ECONOMIC RULES

Core economy uses deterministic/rule-based logic.

LLM does not decide:

food production.

price.

stock.

---

<a id="m18-t043-s345"></a>

## 265. LLM ROLE

LLM can express:

merchant dialogue.

market description.

rumor.

But values come from simulation.

---

<a id="m18-t043-s346"></a>

## 266. EVENT ENGINE ROLE

Economic threshold generates:

Pressure.

Event eligibility.

Not automatic narrative text.

---

<a id="m18-t043-s347"></a>

## PART XXIX — ANTI-EXPLOIT DESIGN

<a id="m18-t043-s348"></a>

## 267. NO SHOP RESET EXPLOIT

Inventory changes through supply/time.

Not reload/day reset.

---

<a id="m18-t043-s349"></a>

## 268. NO INFINITE BUY/SELL LOOP

Buy/sell spread + demand limits.

Merchant capacity.

---

<a id="m18-t043-s350"></a>

## 269. NO INFINITE CRAFT PROFIT

Craft profit limited by:

demand.

material.

labor.

time.

transport.

---

<a id="m18-t043-s351"></a>

## 270. NO INFINITE HARVEST

Resources regenerate seasonally/ecologically.

Overharvest damages environment.

---

<a id="m18-t043-s352"></a>

## 271. NO FAST TRAVEL ARBITRAGE WITHOUT TIME

Fast travel consumes real time.

Market may change.

---

<a id="m18-t043-s353"></a>

## 272. NO MASS PRODUCTION BY ONE PERSON

Output bounded by:

time.

workshop.

labor.

---

<a id="m18-t043-s354"></a>

## 273. NO ECONOMIC LEVEL SCALING

Poor village does not suddenly sell superior gear because player high-level.

---

<a id="m18-t043-s355"></a>

## PART XXX — ECONOMIC FAILURE & FAIL-FORWARD

<a id="m18-t043-s356"></a>

## 274. PLAYER BANKRUPTCY

No game over.

Can:

borrow.

sell property.

work.

join faction.

change career.

---

<a id="m18-t043-s357"></a>

## 275. WORKSHOP FAILURE

Can lead to:

debt.

loss of worker.

career change.

Not save reload requirement.

---

<a id="m18-t043-s358"></a>

## 276. TRADE LOSS

Boat sinks/cargo lost.

Creates debt and story.

---

<a id="m18-t043-s359"></a>

## 277. SETTLEMENT ECONOMIC COLLAPSE

Can lead:

migration.

crime.

faction change.

abandonment.

World continues.

---

<a id="m18-t043-s360"></a>

## 278. RECOVERY

Economy can recover through:

new route.

good harvest.

investment.

political reform.

player action.

---

<a id="m18-t043-s361"></a>

## PART XXXI — HISTORICAL & CULTURAL SAFEGUARD

<a id="m18-t043-s362"></a>

## 279. AVOID MEDIEVAL-FANTASY ECONOMY COPY

Do not automatically import:

- universal coinage;
- adventurer guild;
- standardized inns;
- armor shops;
- merchant guild structures

without historical/game-canon justification.

---

<a id="m18-t043-s363"></a>

## 280. HISTORICAL RESEARCH TAG

Economic content receives:

**Archaeologically Supported**

**Historically Plausible**

**Tradition-Inspired**

**Game-Canon**

Especially for:

exchange medium.

land rights.

workshop organization.

trade institution.

---

<a id="m18-t043-s364"></a>

## 281. GAMEPLAY OVER FALSE PRECISION

If evidence is uncertain:

use plausible abstraction.

Do not claim exact historical system without evidence.

---

<a id="m18-t043-s365"></a>

## PART XXXII — QUALITY GATE: ECONOMY

<a id="m18-t043-s366"></a>

## 282. ECONOMIC SYSTEM QUALITY QUESTIONS

Every major commodity must answer:

1. Where does it come from?
2. Who produces it?
3. Who consumes it?
4. How is it transported?
5. What limits production?
6. What happens when supply fails?
7. Which settlements specialize?
8. What stories can scarcity create?

---

<a id="m18-t043-s367"></a>

## 283. MARKET QUALITY GATE

A market must reflect:

local production.

route access.

season.

security.

demand.

If every shop has same price/inventory:

system has failed.

---

<a id="m18-t043-s368"></a>

## 284. TRADE QUALITY GATE

A trade route must have:

cargo reason.

transport method.

cost.

risk.

political context.

Seasonality where relevant.

---

<a id="m18-t043-s369"></a>

## 285. CRAFT QUALITY GATE

Every meaningful craft family must define:

Design.

Inputs.

Critical Tools.

Skill.

Process.

Time.

Quality dimensions.

Failure mode.

Repair model.

Innovation space.

---

<a id="m18-t043-s370"></a>

## 286. WORKSHOP QUALITY GATE

Workshop must have:

owner.

workers.

tools.

specialization.

resource source.

customer.

social relationships.

---

<a id="m18-t043-s371"></a>

## 287. SETTLEMENT PROJECT QUALITY GATE

Every project needs:

problem.

stakeholders.

resource.

labor.

authority.

time.

benefits.

costs.

long-term maintenance.

---

<a id="m18-t043-s372"></a>

## 288. ECONOMIC QUEST QUALITY GATE

Economic quest must originate from:

real deficit.

surplus.

debt.

contract.

route.

labor problem.

Not arbitrary item collection.

---

<a id="m18-t043-s373"></a>

## PART XXXIII — LOCKED DESIGN DECISIONS

<a id="m18-t043-s374"></a>

## 289. CORE DECISIONS

1. Economy is location-based.
2. Economy is time-dependent.
3. Household is basic simulation unit.
4. Settlement stocks are aggregated.
5. Important items remain instantiated.
6. Goods have quality, origin and ownership.
7. Transport cost matters.
8. No universal infinite merchant inventory.
9. Prices react to supply, demand, transport and risk.
10. Exchange is not forced into generic fantasy gold model.
11. Credit, favor and debt are meaningful economic mechanisms.
12. Trade routes carry goods and information.
13. Player may pursue merchant career.
14. Crafting is Design + Material + Process + Tool + Skill + Time.
15. Crafting is not recipe-clicking alone.
16. Materials can be substituted with trade-offs.
17. Item quality is multidimensional.
18. Repair is more important than loot replacement treadmill.
19. Workshops have owners, workers and traditions.
20. Apprentices are real NPCs.
21. Innovation requires experimentation and need.
22. Prototype and mass production are different.
23. Standardization is a major Cổ Loa theme.
24. Settlement projects alter world simulation.
25. Infrastructure creates winners and losers.
26. Player can own property but location matters.
27. Land/resource access is contextual, not simplified modern property.
28. War redirects labor and resources.
29. Economic shocks propagate between regions.
30. Economic failure creates narrative rather than game over.
31. Economy is connected to Quest, Faction, Profession and World Simulation.
32. LLM never controls economic truth or price logic.

---

<a id="m18-t043-s375"></a>

## PART XXXIV — DEFERRED TO PRODUCTION

<a id="m18-t043-s376"></a>

## 290. CONTENT TO BUILD LATER

Production phase will determine:

- exact commodity catalog;
- exact units;
- full production chains;
- historical exchange media;
- numerical production rates;
- local price ranges;
- seasonal multipliers;
- transport capacities;
- exact workshop facilities;
- craft designs;
- material-property tables;
- full trade-route network;
- exact player property options;
- economic UI;
- simulation optimization;
- procedural market events;
- historical terminology.

Framework is locked now.

Content volume and balance come later.

---

<a id="m18-t043-s377"></a>

## PART XXXV — CORE ECONOMIC FANTASY

<a id="m18-t043-s378"></a>

## 291. PLAYER EXPERIENCE

Một RPG thông thường có thể khiến player nghĩ:

> “Tôi cần farm thêm tiền để mua thanh kiếm cấp cao hơn.”

Game này nên khiến player nghĩ:

> “Tại sao tháng này không ai ở Lò Cốc bán công cụ?”

Sau khi hỏi:

> “Vì Suối Than ngừng gửi nhiên liệu.”

Sau khi điều tra:

> “Đường vận tải bị chặn.”

Sau khi giải quyết:

> production hoạt động lại.

Hai tháng sau:

> tool price giảm.

Một năm sau:

> workshop tuyển thêm apprentice.

Năm năm sau:

> một apprentice player từng cứu trở thành master.

Đó là economic storytelling.

---

<a id="m18-t043-s379"></a>

## 292. CORE CRAFTING FANTASY

Crafting không phải:

> “Tôi có đủ nguyên liệu chưa?”

Nó phải hỏi:

> “Tôi đang cố làm thứ gì?”

> “Vật liệu này có phù hợp không?”

> “Ai biết cách làm?”

> “Tôi có workshop phù hợp không?”

> “Tôi muốn nhanh hay tốt?”

> “Tôi có sẵn sàng thử một cách chưa ai chắc chắn không?”

> “Nếu thành công, tôi sẽ giữ kỹ thuật hay truyền nó?”

Crafting trở thành:

> **tri thức vật chất của thế giới.**

---

<a id="m18-t043-s380"></a>

## 293. CORE TRADE FANTASY

Trading không phải:

> “mua rẻ bán đắt”.

Nó là:

> **biết nơi nào cần thứ gì, biết ai có thể cung cấp, và đưa nó qua một thế giới đầy khoảng cách, mùa vụ, quan hệ và chính trị.**

Một merchant giỏi không chỉ giàu.

Họ biết:

- ai đang thiếu grain;
- route nào đang nguy hiểm;
- chief nào sắp huy động quân;
- workshop nào cần metal;
- settlement nào sắp gặp crisis.

Merchant trở thành một trong những người hiểu thế giới nhất.

---

<a id="m18-t043-s381"></a>

## 294. CORE SETTLEMENT FANTASY

Một settlement không tồn tại để player:

> mua đồ → nhận quest → đi.

Player có thể quay lại sau mười năm và thấy:

cây cầu mình từng giúp xây.

workshop mình từng mở.

family từng giàu nay suy tàn.

apprentice đã thành master.

route mới đã khiến làng đông hơn.

một wetland đã biến mất vì công trình.

giá food thay đổi.

người dân nhớ quyết định của player.

Settlement trở thành:

> **một cộng đồng có lịch sử kinh tế và xã hội thật.**

---

<a id="m18-t043-s382"></a>

## 295. MODULE POSITION IN THE COMPLETE GAME

Sau module này, vòng simulation của game có thể chạy gần như đầy đủ:

**Environment**

→ tạo Resource.

**Household**

→ tạo Labor + Production + Consumption.

**Settlement**

→ tạo Surplus / Deficit.

**Trade Network**

→ dịch chuyển Goods + Information.

**Profession**

→ biến Resource thành Product/Service.

**Crafting**

→ biến Knowledge + Material thành Item/Technology.

**Faction**

→ kiểm soát hoặc tranh giành Resource.

**Travel**

→ tạo Logistics Cost.

**World Event**

→ tạo Shock.

**Quest**

→ expose Economic Pressure cho player.

**Player Intervention**

→ thay route, production, ownership, agreement hoặc technology.

**Settlement State**

→ thay đổi.

**NPC Career / Migration / Family**

→ thay đổi theo.

**History**

→ tiếp tục.

Điểm quan trọng nhất:

> **Kinh tế không phải nền phía sau gameplay.**

Nó chính là một trong những engine tạo ra gameplay.

Sau module này, các subsystem vật chất của game đã khá đầy đủ. Bước tiếp theo tôi đề xuất chuyển sang **Religion + Belief + Mythology + Mạch Lạc + Supernatural**, bởi chúng ta đã sử dụng nghi lễ, Vu Chúc, sacred site, taboo, ancestor, Mạch Stability và các truyền thuyết ở rất nhiều module nhưng vẫn chưa định nghĩa chính xác **cái gì là niềm tin của con người, cái gì là hiện tượng game thật sự, cái gì cố tình giữ mơ hồ, và supernatural can thiệp gameplay đến mức nào**. Đây sẽ là module quyết định bản sắc lớn nhất để game không biến thành fantasy phương Tây khoác tên Việt.

<!-- SOURCE T043 END -->

