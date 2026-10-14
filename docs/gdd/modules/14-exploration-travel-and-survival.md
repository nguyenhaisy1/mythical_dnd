# Module 14 — Khám phá, di chuyển, sinh tồn và bản đồ

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T035](../../source/conversation.md#t035)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

World Route Graph là nguồn route duy nhất. RouteKnowledge khác route truth. Supply Days dẫn xuất từ CommodityLot; không lưu như kho food độc lập.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Route edges, season, direction, load, actor knowledge, tools, health và lots. |
| Đầu ra / bên sở hữu | Travel plan, scheduled arrival, resource use, observation; Travel/Assets/Knowledge sở hữu. |
| Đọc cùng | 02, 03, 13, 15, 18, 27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Tin cũ có thể dự báo sai; đi và về cùng sông có cost khác; tiêu thụ food đúng một lần. |
| Việc chưa được chứng minh | Full map edges, time units cụ thể và route balancing. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T035 BEGIN -->

Đây là một module rất quan trọng vì nó quyết định **thế giới có thực sự “được sống” hay chỉ là danh sách điểm fast travel**. Tôi sẽ khóa World Map, Travel, Exploration và Survival thành một hệ thống thống nhất; Dungeon/Encounter về sau sẽ cắm trực tiếp vào hệ này.

<a id="m14-t035-s001"></a>

## GDD MODULE 14 — EXPLORATION, TRAVEL, SURVIVAL & WORLD MAP SYSTEM

<a id="m14-t035-s002"></a>

## 1. DESIGN PHILOSOPHY

Exploration không phải:

> mở bản đồ → chạy tới dấu `?` → nhặt rương.

Travel không phải:

> chọn địa điểm → loading screen.

Survival không phải:

> cứ 5 phút ăn một miếng thịt.

World Map không phải:

> menu chọn stage.

Toàn bộ hệ thống phải tạo cảm giác:

> **địa lý thực sự có ý nghĩa; khoảng cách có giá; kiến thức địa phương có sức mạnh; mỗi chuyến đi là một quyết định.**

Người chơi phải quan tâm:

- đi đâu;
- đi bằng đường nào;
- vào mùa nào;
- đi cùng ai;
- mang gì;
- biết gì về đường;
- có đủ thức ăn không;
- địa phương có an toàn không;
- chiến tranh có cắt tuyến không;
- nước sông có đổi không;
- party có người hiểu vùng đó không;
- và nếu quay về muộn thì thế giới đã thay đổi gì.

---

<a id="m14-t035-s003"></a>

## 2. CORE EXPLORATION FANTASY

Một hành trình thành công không phải:

> “Tôi chạy đến đích mà không chết.”

Mà là:

> “Tôi đã chọn đúng tuyến, hiểu được vùng đất, thích nghi với những gì xảy ra và tới nơi với cái giá chấp nhận được.”

Một Liệp Nhân có thể nhìn rừng và thấy đường.

Một Thủy Binh nhìn cùng bản đồ nhưng nghĩ bằng dòng nước.

Một Sứ Giả nhìn bản đồ thành mạng lưới con người.

Một thương nhân nhìn bằng luồng hàng.

Một Vu Chúc nhìn những điểm thiêng.

Một người bình thường chỉ thấy:

> “Từ đây tới đó rất xa.”

---

<a id="m14-t035-s004"></a>

## 3. BỐN TẦNG KHÔNG GIAN

Game chia không gian thành bốn cấp.

<a id="m14-t035-s005"></a>

### WORLD LAYER

12 đại vùng và tuyến kết nối.

Dùng để:

- lập kế hoạch;
- di chuyển xa;
- theo dõi chiến tranh;
- economy;
- weather;
- route condition.

---

<a id="m14-t035-s006"></a>

### REGION LAYER

Một đại vùng.

Ví dụ:

Tam Giang.

Hiển thị:

- settlement;
- river;
- forest;
- routes;
- landmarks;
- hazards;
- local stories.

---

<a id="m14-t035-s007"></a>

### LOCAL AREA

Một khu vực có thể trực tiếp khám phá.

Ví dụ:

Bến Vạc + vùng ven sông.

Một đoạn rừng.

Một thung lũng.

Một cụm ruộng.

Một bãi chiến trường.

---

<a id="m14-t035-s008"></a>

### SITE

Không gian có cấu trúc rõ.

Ví dụ:

- cave;
- settlement interior;
- sacred grove;
- tomb;
- ruined workshop;
- battlefield;
- swamp island.

Dungeon System sau này chủ yếu hoạt động ở cấp Site.

---

<a id="m14-t035-s009"></a>

## 4. WORLD MAP KHÔNG HOÀN TOÀN CHÍNH XÁC

Player không bắt đầu với GPS.

World Map là:

> **những gì character biết về thế giới.**

Có thể biết:

Phong Châu ở hướng nào.

Có thể không biết:

đường cụ thể.

Map Knowledge được xây dần.

---

<a id="m14-t035-s010"></a>

## 5. MAP KNOWLEDGE

Mỗi vùng bản đồ có Knowledge State.

<a id="m14-t035-s011"></a>

#### Unknown

Không biết tồn tại.

<a id="m14-t035-s012"></a>

#### Heard Of

Được nghe kể.

<a id="m14-t035-s013"></a>

#### Approximate

Biết vị trí tương đối.

<a id="m14-t035-s014"></a>

#### Known

Biết tuyến chính.

<a id="m14-t035-s015"></a>

#### Familiar

Biết đường phụ, seasonal route.

<a id="m14-t035-s016"></a>

#### Intimate

Hiểu địa hình sâu.

---

<a id="m14-t035-s017"></a>

## 6. CHARACTER-SPECIFIC MAP

Hai character có map khác nhau.

Character Tam Giang:

biết nhiều bến sông.

Character Tản Viên:

biết đèo, khe, đường săn.

Do đó random origin ảnh hưởng exploration ngay từ đầu.

---

<a id="m14-t035-s018"></a>

## 7. MAP SHARING

Party member có thể chia sẻ Map Knowledge.

Nhưng không tự động copy hoàn toàn.

Ví dụ guide nói:

> “Qua con suối rồi đi về phía cây đá trắng.”

Player nhận:

Approximate Route.

Đi route nhiều lần:

Knowledge tăng.

---

<a id="m14-t035-s019"></a>

## 8. MAP INFORMATION CÓ ĐỘ TIN CẬY

Một NPC có thể chỉ sai.

Route information có:

Source.

Confidence.

Age.

Season.

Một con đường đúng năm trước có thể:

bị lũ cuốn.

bị quân chặn.

mọc rậm.

---

<a id="m14-t035-s020"></a>

## 9. MAP KHÔNG NÊN HIỂN THỊ TẤT CẢ RESOURCE

Không:

> icon herb x 24.

Map hiển thị:

- biome;
- landmarks;
- settlement;
- known resource area.

Việc tìm resource cụ thể cần Exploration.

---

<a id="m14-t035-s021"></a>

## 10. WORLD ROUTE NETWORK

Các địa điểm nối bằng Route Edge.

Mỗi Route có:

```text
Route Type
Distance
Travel Time
Terrain
Seasonality
Security
Traffic
Supply Access
Information Speed
Capacity
Political Control
Knowledge Requirement
```

---

<a id="m14-t035-s022"></a>

## 11. ROUTE TYPES

Core route:

<a id="m14-t035-s023"></a>

#### River Route

Nhanh cho cargo.

Phụ thuộc nước/thuyền.

---

<a id="m14-t035-s024"></a>

#### Major Land Route

Dễ đi hơn.

Nhiều người.

Dễ gặp authority.

---

<a id="m14-t035-s025"></a>

#### Local Path

Ngắn nhưng ít dịch vụ.

---

<a id="m14-t035-s026"></a>

#### Mountain Trail

Khó đi.

Knowledge rất quan trọng.

---

<a id="m14-t035-s027"></a>

#### Forest Trail

Có thể biến mất theo mùa.

---

<a id="m14-t035-s028"></a>

#### Swamp Passage

Chỉ một số người biết.

---

<a id="m14-t035-s029"></a>

#### Sea/Coastal Route

Xa nhưng vận tải lớn.

Weather risk.

---

<a id="m14-t035-s030"></a>

#### Hidden Route

Không có trên common map.

---

<a id="m14-t035-s031"></a>

## 12. ROUTE KHÔNG PHẢI CHỈ TỪ A ĐẾN B

Một route có identity.

Ví dụ:

**Bến Vạc → Phong Châu**

River.

Busy.

Trader traffic cao.

Information speed nhanh.

Bandit thấp nếu authority ổn.

Trong chiến tranh:

checkpoint tăng.

---

<a id="m14-t035-s032"></a>

## 13. ROUTE STATE

Route có thể:

Open.

Impaired.

Dangerous.

Blocked.

Controlled.

Unknown.

Seasonally Closed.

State thay đổi theo world simulation.

---

<a id="m14-t035-s033"></a>

## 14. ROUTE BLOCK

Nguyên nhân:

- flood;
- landslide;
- war;
- destroyed bridge;
- bandit;
- sacred taboo;
- disease quarantine;
- political closure.

Block không chỉ buộc player đi vòng.

Nó ảnh hưởng:

trade.

food.

migration.

information.

---

<a id="m14-t035-s034"></a>

## 15. TRAVEL TIME

Travel time dựa trên:

Distance

+

Route Quality

+

Terrain

+

Weather

+

Party Speed

+

Load

+

Transport

+

Knowledge.

Không cần mô phỏng bước chân.

---

<a id="m14-t035-s035"></a>

## 16. TRAVEL DAY

Đơn vị simulation chính cho hành trình xa:

**Travel Segment / Travel Day**

Một ngày có thể chia:

Morning.

Midday.

Evening.

Night Camp.

Player không cần chơi từng giờ nếu không có event.

---

<a id="m14-t035-s036"></a>

## 17. TRAVEL PLANNING PHASE

Trước chuyến đi, player chọn:

- destination;
- route;
- companions;
- supplies;
- equipment;
- travel speed;
- transport;
- optional stops;
- risk tolerance.

---

<a id="m14-t035-s037"></a>

## 18. ROUTE COMPARISON

Ví dụ:

Phong Châu → Tam Giang.

<a id="m14-t035-s038"></a>

#### Route A — River

2 ngày.

Need boat.

Low food use.

Flood risk.

<a id="m14-t035-s039"></a>

#### Route B — Land

4 ngày.

No boat.

More food.

Bandit risk.

<a id="m14-t035-s040"></a>

#### Route C — Forest Shortcut

2.5 ngày.

Needs local knowledge.

High navigation risk.

Player chọn.

---

<a id="m14-t035-s041"></a>

## 19. TRAVEL SPEED

Ba mode cơ bản:

<a id="m14-t035-s042"></a>

### Cẩn trọng

Chậm.

Tăng detection.

Giảm ambush.

Tăng gathering.

---

<a id="m14-t035-s043"></a>

### Bình thường

Default.

---

<a id="m14-t035-s044"></a>

### Khẩn hành

Nhanh.

Fatigue tăng.

Observation giảm.

Equipment strain tăng.

Không thể duy trì lâu.

---

<a id="m14-t035-s045"></a>

## 20. FORCE MARCH

Party có thể ép đi nhanh trong crisis.

Cost:

Fatigue.

Injury risk.

Morale.

Food/water.

Có thể cứu được người nếu tới kịp.

Travel trở thành meaningful choice.

---

<a id="m14-t035-s046"></a>

## 21. TIME PRESSURE

Không phải mọi quest có timer.

Nhưng một số sự kiện thật sự chạy theo thời gian.

Ví dụ:

người mất tích.

disease.

war.

flood.

Player có thể chọn:

route nhanh nguy hiểm

hay

route an toàn chậm.

---

<a id="m14-t035-s047"></a>

## 22. NO FAKE URGENCY

Nếu NPC nói:

> “Hãy nhanh lên!”

nhưng quest chờ vô hạn:

immersion mất.

Urgency phải được World Simulation hỗ trợ thật.

---

<a id="m14-t035-s048"></a>

## 23. TRANSPORT MODES

Core modes:

Walking.

Boat.

Raft.

Large River Boat.

Coastal Boat.

Carrying group/porter.

Các phương tiện kéo/gia súc chỉ thêm khi setting research xác nhận phù hợp gameplay.

---

<a id="m14-t035-s049"></a>

## 24. WALKING

Ưu:

linh hoạt.

Không phụ thuộc phương tiện.

Nhược:

carry limit.

slower cargo movement.

fatigue.

---

<a id="m14-t035-s050"></a>

## 25. BOAT

Một trong những transport system quan trọng nhất.

Boat có:

Capacity.

Draft.

Speed.

Stability.

River Suitability.

Cargo.

Condition.

Crew Requirement.

---

<a id="m14-t035-s051"></a>

## 26. BOAT OWNERSHIP

Boat là tài sản lớn.

Có Owner.

Crew.

Repair history.

Không spawn miễn phí tại mọi river bank.

---

<a id="m14-t035-s052"></a>

## 27. BORROW / HIRE BOAT

Có thể:

thuê.

mượn.

đi nhờ.

mua.

trộm.

Relationships/economy matters.

---

<a id="m14-t035-s053"></a>

## 28. BOAT ROUTE

Không phải mọi nước đều navigable.

Factors:

depth.

current.

season.

obstruction.

boat type.

Thủy Binh/boatman biết.

---

<a id="m14-t035-s054"></a>

## 29. PORTAGE

Một số đoạn:

phải kéo thuyền.

chuyển cargo.

đi đường bộ.

Tạo logistics decision.

---

<a id="m14-t035-s055"></a>

## 30. WEATHER SYSTEM

Weather không chỉ visual.

Core variables:

Rain.

Heat.

Wind.

Visibility.

Storm.

Water Level.

---

<a id="m14-t035-s056"></a>

## 31. WEATHER FORECAST

Character không xem app.

Có thể dự đoán bằng:

- local knowledge;
- animal behavior;
- clouds;
- wind;
- professional experience;
- omen interpretation.

Forecast có uncertainty.

---

<a id="m14-t035-s057"></a>

## 32. WEATHER INFORMATION

UI có thể nói:

> “Mân cho rằng chiều nay nước sẽ lên.”

Không:

`70% rain 15:00`.

---

<a id="m14-t035-s058"></a>

## 33. RAIN EFFECT

Có thể:

- giảm visibility;
- tạo mud;
- tăng river;
- làm dấu vết biến mất;
- ảnh hưởng fire;
- giảm travel speed;
- tạo crop benefit.

---

<a id="m14-t035-s059"></a>

## 34. HEAVY RAIN

Travel risk tăng.

Một route có thể từ:

Open

→ Dangerous

→ Blocked.

---

<a id="m14-t035-s060"></a>

## 35. HEAT

Tăng:

water need.

fatigue.

armor burden.

---

<a id="m14-t035-s061"></a>

## 36. WIND

Quan trọng với:

boat.

fire.

ranged weapon.

coastal travel.

---

<a id="m14-t035-s062"></a>

## 37. FOG

Đặc biệt:

Tam Giang.

Đại Trạch.

Giảm navigation/visibility.

Nhưng người địa phương có thể ít bị ảnh hưởng.

---

<a id="m14-t035-s063"></a>

## 38. SEASON SYSTEM

Thế giới có seasonal rhythm.

Không nhất thiết dùng bốn mùa kiểu fantasy châu Âu cứng.

GDD nên dùng:

**Seasonal State** dựa trên:

rainfall;

water;

agricultural cycle;

wind;

regional ecology.

---

<a id="m14-t035-s064"></a>

## 39. REGION SEASON PROFILE

Mỗi vùng phản ứng khác nhau.

Tam Giang:

Water Level rất quan trọng.

Bãi Lạc:

farming phase.

Hải Môn:

wind/storm.

Tản Viên:

mountain access.

Đại Trạch:

water + disease + insects.

---

<a id="m14-t035-s065"></a>

## 40. SEASONAL ROUTES

Ví dụ:

Cồn Mây chỉ dễ tiếp cận mùa nước thấp.

Một swamp path chỉ tồn tại mùa khô.

Một river shortcut chỉ navigable mùa nước cao.

World Map thay đổi theo thời gian.

---

<a id="m14-t035-s066"></a>

## 41. SEASONAL CONTENT

Không chỉ route.

Có:

- festival;
- migration;
- hunting;
- fishing;
- herbs;
- war campaigning;
- construction.

Travel timing có strategic value.

---

<a id="m14-t035-s067"></a>

## PART II — SURVIVAL

<a id="m14-t035-s068"></a>

## 42. SURVIVAL DESIGN GOAL

Survival tồn tại để tạo:

**planning + pressure + story**

không phải housekeeping.

Không muốn player:

ăn mỗi 8 phút.

uống mỗi 3 phút.

---

<a id="m14-t035-s069"></a>

## 43. CORE SURVIVAL RESOURCES

Chỉ dùng những resource thật sự cần:

<a id="m14-t035-s070"></a>

#### Food

<a id="m14-t035-s071"></a>

#### Water

<a id="m14-t035-s072"></a>

#### Rest

<a id="m14-t035-s073"></a>

#### Shelter

<a id="m14-t035-s074"></a>

#### Warmth/Heat Management khi context cần

<a id="m14-t035-s075"></a>

#### Medical Supplies

Không cần 12 thanh sinh tồn.

---

<a id="m14-t035-s076"></a>

## 44. FOOD

Food tính theo:

**Rations / Party Days**

Ví dụ:

`Food: 4 party-days`

Dễ đọc hơn:

17 quả + 3 cá + 4 hạt.

Chi tiết food item vẫn có thể tồn tại khi cần.

---

<a id="m14-t035-s077"></a>

## 45. FOOD QUALITY

Có:

Poor.

Adequate.

Good.

Special.

Ảnh hưởng:

recovery.

morale.

health.

Không buff combat lớn.

---

<a id="m14-t035-s078"></a>

## 46. FOOD SPOILAGE

Chỉ một số thực phẩm.

Player có thể:

dry.

salt.

smoke.

Không cần simulation vi mô tất cả đồ.

---

<a id="m14-t035-s079"></a>

## 47. LOCAL FOOD

Region có cuisine/resources riêng.

Tạo:

identity.

conversation.

trade.

---

<a id="m14-t035-s080"></a>

## 48. HUNGER STAGES

Không có:

0 hunger → chết.

Stages:

Fed.

Hungry.

Underfed.

Starving.

Effects tăng dần qua nhiều ngày.

---

<a id="m14-t035-s081"></a>

## 49. WATER

Water thường abstract khi route có nguồn đáng tin.

Chỉ trở thành resource quan trọng nếu:

- dry region;
- contaminated water;
- siege;
- unknown route;
- long sea travel.

Không bắt player carry gallons trong Tam Giang khi nước có khắp nơi, nhưng phải xét water safety.

---

<a id="m14-t035-s082"></a>

## 50. WATER SAFETY

Không phải nước nào uống được.

Knowledge/heating/treatment matters.

Disease risk.

---

<a id="m14-t035-s083"></a>

## 51. REST

Character cần:

Sleep Quality.

Rest Duration.

Safety.

Một đêm ngủ trên đất khô gần làng khác:

sleep trong swamp dưới mưa.

---

<a id="m14-t035-s084"></a>

## 52. REST STATES

Rested.

Tired.

Fatigued.

Exhausted.

Đồng bộ Combat Fatigue.

---

<a id="m14-t035-s085"></a>

## 53. SHELTER

Shelter ảnh hưởng:

Rest.

weather exposure.

fire.

privacy.

security.

---

<a id="m14-t035-s086"></a>

## 54. CAMP QUALITY

Camp rating được tạo từ:

location.

tools.

skill.

weather.

time.

Không phải mini base.

---

<a id="m14-t035-s087"></a>

## 55. CAMP SETUP

Routine camp được auto.

Chỉ expose decisions khi:

bad weather;

danger;

lack equipment;

sick companion;

special location.

---

<a id="m14-t035-s088"></a>

## 56. CAMPFIRE

Benefits:

warmth.

food.

morale.

light.

Risks:

visibility.

fire.

smoke.

Enemy detection.

Có thể chọn:

Fire / No Fire.

---

<a id="m14-t035-s089"></a>

## 57. FIREWOOD

Không track từng khúc củi.

Chỉ có scarcity nếu:

wet.

swamp.

treeless area.

---

<a id="m14-t035-s090"></a>

## 58. FORAGING

Không phải click bushes.

Party có thể dành Travel Time cho:

food.

herbs.

firewood.

water.

Costs:

speed.

Risk.

---

<a id="m14-t035-s091"></a>

## 59. FORAGING OUTCOME

Dựa:

Skill.

Knowledge.

Season.

Biome.

Time.

Không spawn resource từ hư không.

---

<a id="m14-t035-s092"></a>

## 60. HUNTING

Hunting có thể được abstract khi routine.

Nhưng large/rare hunt có thể trở thành Encounter.

---

<a id="m14-t035-s093"></a>

## 61. HUNTING COST

Time.

noise.

ammunition.

risk.

Hunt không luôn là free food exploit.

---

<a id="m14-t035-s094"></a>

## 62. FISHING

Ở Tam Giang/Hải Môn:

routine profession.

Party không cần mini-game mỗi ngày.

Nếu character có profession:

food acquisition có thể được abstract.

---

<a id="m14-t035-s095"></a>

## 63. PROFESSIONAL SURVIVAL

Một boatman/hunter/farmer khác adventurer generic.

Profession giảm survival burden đúng context.

---

<a id="m14-t035-s096"></a>

## 64. SUPPLY SOURCES

Party có thể resupply:

Settlement.

Household.

Market.

Hunting.

Fishing.

Foraging.

Faction camp.

Friendly family.

---

<a id="m14-t035-s097"></a>

## 65. REPUTATION & SUPPLIES

Một village ghét player:

có thể không bán.

Một family mắc nợ player:

cho food.

Survival kết nối social system.

---

<a id="m14-t035-s098"></a>

## 66. STARVATION KHÔNG NÊN LÀ COMMON FAILURE

Nếu player hết food ở vùng đông dân:

hậu quả thường là:

mất tiền.

xin giúp.

steal.

debt.

Không phải chết đói sau 24h.

Starvation chủ yếu ở:

wilderness.

war.

disaster.

---

<a id="m14-t035-s099"></a>

## 67. SURVIVAL FAILURE TẠO STORY

Hết food có thể buộc:

săn thú thiêng.

ăn grain reserved for village.

vay.

steal.

quay lại.

Tạo moral choice.

---

<a id="m14-t035-s100"></a>

## PART III — EXPLORATION

<a id="m14-t035-s101"></a>

## 68. EXPLORATION LOOP

Core loop:

**Observe**

↓

**Choose Direction**

↓

**Move**

↓

**Discover Information**

↓

**Interpret**

↓

**Decide**

↓

**Record Knowledge**

Không phải:

walk → collectible.

---

<a id="m14-t035-s102"></a>

## 69. LANDMARK SYSTEM

Landmarks giúp:

navigation.

memory.

lore.

Ví dụ:

tree.

stone.

river bend.

old grave.

abandoned furnace.

hill.

---

<a id="m14-t035-s103"></a>

## 70. LANDMARK KNOWLEDGE

Player có thể biết:

Location.

Name.

Meaning.

History.

khác nhau.

Một stone có thể:

đã thấy

nhưng không biết nó là boundary marker.

---

<a id="m14-t035-s104"></a>

## 71. DISCOVERY TYPES

Exploration discovery chia:

<a id="m14-t035-s105"></a>

#### Geographic

Route, cave, river.

<a id="m14-t035-s106"></a>

#### Social

Hidden settlement, camp.

<a id="m14-t035-s107"></a>

#### Resource

Ore, herb, fishing ground.

<a id="m14-t035-s108"></a>

#### Historical

Ruins, grave, battlefield.

<a id="m14-t035-s109"></a>

#### Narrative

Clue, personal trace.

<a id="m14-t035-s110"></a>

#### Spiritual

Sacred site, Mạch anomaly.

<a id="m14-t035-s111"></a>

#### Economic

Trade shortcut.

---

<a id="m14-t035-s112"></a>

## 72. DISCOVERY KHÔNG LUÔN TẠO QUEST

Một abandoned kiln có thể chỉ là:

world detail.

Sau này mới liên quan story.

Không mọi landmark cần marker.

---

<a id="m14-t035-s113"></a>

## 73. OBSERVATION

GIÁC giúp notice.

Skill giúp hiểu.

Ví dụ:

GIÁC cao:

thấy cây bị cắt.

Mộc Knowledge:

biết bị cắt bằng tool gì.

Tracking:

biết mới hay cũ.

Regional Knowledge:

biết ai thường lấy loại cây đó.

---

<a id="m14-t035-s114"></a>

## 74. PASSIVE DISCOVERY

Nếu capability đủ cao:

game tự reveal.

Không roll mọi 10 giây.

---

<a id="m14-t035-s115"></a>

## 75. ACTIVE SEARCH

Player có thể chủ động:

Search Area.

Tốn:

Time.

Fatigue.

Risk.

Dùng khi nghi có clue/resource.

---

<a id="m14-t035-s116"></a>

## 76. SEARCH SCALE

Local search:

minutes.

Area search:

hours.

Missing person:

days.

Time thật sự trôi.

---

<a id="m14-t035-s117"></a>

## 77. NO PIXEL HUNT

Clue quan trọng không yêu cầu camera tìm một vật nhỏ.

Skill/character determines detection.

Player decision tập trung:

đâu cần tìm?

tìm bao lâu?

ai tìm?

---

<a id="m14-t035-s118"></a>

## 78. TRACKING

Tracking là Exploration sub-loop.

Target leaves Trace Objects.

Ví dụ:

footprint.

broken plant.

ash.

discarded item.

boat mark.

witness.

---

<a id="m14-t035-s119"></a>

## 79. TRAIL DEGRADATION

Trace giảm theo:

time.

rain.

traffic.

water.

deliberate concealment.

---

<a id="m14-t035-s120"></a>

## 80. TRAIL CONFIDENCE

Player không luôn biết trail đúng.

Có confidence:

Weak.

Likely.

Strong.

Confirmed.

---

<a id="m14-t035-s121"></a>

## 81. FALSE TRAIL

Có thể xảy ra vì:

similar trace.

deliberate deception.

bad interpretation.

Không random cheat.

---

<a id="m14-t035-s122"></a>

## 82. EXPLORATION RISK

Risk types:

Getting Lost.

Weather.

Injury.

Animal.

Hostile people.

Disease.

Resource loss.

Spiritual anomaly.

---

<a id="m14-t035-s123"></a>

## 83. GETTING LOST

Không:

screen says "Lost".

Character có Position Confidence.

<a id="m14-t035-s124"></a>

#### Certain

<a id="m14-t035-s125"></a>

#### Likely

<a id="m14-t035-s126"></a>

#### Uncertain

<a id="m14-t035-s127"></a>

#### Lost

Player có thể chưa biết chính xác khi nào route sai.

---

<a id="m14-t035-s128"></a>

## 84. NAVIGATION

Dựa:

Landmarks.

Direction.

Terrain.

Knowledge.

Weather.

Guide.

---

<a id="m14-t035-s129"></a>

## 85. LOST DOES NOT TELEPORT PLAYER

Nếu lost:

player tiếp tục ở world.

Có thể:

find new landmark.

backtrack.

camp.

climb high.

follow river.

ask someone.

---

<a id="m14-t035-s130"></a>

## 86. GETTING LOST CAN BE REWARDING

Có thể tìm:

hidden site.

settlement.

resource.

Không luôn punishment.

---

<a id="m14-t035-s131"></a>

## 87. ROUTE MEMORY

Một route đi thành công:

Knowledge tăng.

Sau nhiều lần:

travel được abstract nhiều hơn.

Player không phải replay same journey mãi.

---

<a id="m14-t035-s132"></a>

## 88. FAMILIAR ROUTE FAST TRAVEL

Fast Travel tồn tại nhưng phải được kiếm.

Conditions:

Route Known/Familiar.

Route currently safe enough.

No urgent local event.

Transport available.

---

<a id="m14-t035-s133"></a>

## 89. FAST TRAVEL VẪN TỐN WORLD TIME

Chỉ bỏ gameplay repetition.

Không teleport time.

Food.

weather.

world simulation

vẫn chạy.

---

<a id="m14-t035-s134"></a>

## 90. FAST TRAVEL INTERRUPT

Nếu major event xảy ra:

travel có thể dừng:

route blocked.

attack.

companion crisis.

flood.

Không nên interrupt bằng trash encounter.

---

<a id="m14-t035-s135"></a>

## 91. VERY SAFE ROUTE

Nếu route familiar + stable:

auto travel hoàn toàn.

Không random wolf attack vô nghĩa.

---

<a id="m14-t035-s136"></a>

## PART IV — REGIONAL IDENTITY

<a id="m14-t035-s137"></a>

## 92. MỖI REGION CẦN EXPLORATION RULE RIÊNG

Không chỉ đổi màu map.

---

<a id="m14-t035-s138"></a>

## 93. PHONG CHÂU

Exploration Identity:

**Con người quan trọng hơn địa hình.**

Dense roads.

Political checkpoints.

markets.

ritual routes.

Challenges:

access.

reputation.

information.

crowds.

---

<a id="m14-t035-s139"></a>

## 94. TAM GIANG

Identity:

**Thế giới thay đổi theo nước.**

Routes biến theo water level.

Boat knowledge mạnh.

Flood.

Islands.

river settlements.

Navigation by water.

---

<a id="m14-t035-s140"></a>

## 95. TẢN VIÊN

Identity:

**Vertical terrain + sacred wilderness.**

Elevation.

Mountain trail.

Weather.

Hunting tracks.

Sparse supplies.

Sacred paths.

---

<a id="m14-t035-s141"></a>

## 96. BÃI LẠC

Identity:

**Human-modified landscape.**

Fields.

Canals.

dykes.

villages.

Water control.

Travel dễ hơn nhưng social land boundaries quan trọng.

---

<a id="m14-t035-s142"></a>

## 97. ĐỒNG ĐỎ

Identity:

**Resource landscape.**

Mines.

charcoal route.

workshops.

pollution.

heavy cargo.

collapsed tunnels.

---

<a id="m14-t035-s143"></a>

## 98. LÂM U

Identity:

**Navigation uncertainty.**

Dense forest.

landmarks khó.

old trails.

plants.

animal signs.

Cultural taboo.

---

<a id="m14-t035-s144"></a>

## 99. THƯỢNG ÂU

Identity:

**Mountain networks.**

Passes.

clan territory.

weather.

messenger routes.

political access.

---

<a id="m14-t035-s145"></a>

## 100. ĐẠI TRẠCH

Identity:

**Survival + orientation pressure.**

Mud.

fog.

disease.

hidden channels.

raised ground.

local guides cực quan trọng.

---

<a id="m14-t035-s146"></a>

## 101. HẢI MÔN

Identity:

**Wind + tide/coastal navigation.**

Ports.

salt.

boats.

storms.

outsider trade.

---

<a id="m14-t035-s147"></a>

## 102. VIỆT THƯỜNG

Identity:

**Cultural frontier.**

Unknown language.

plants.

routes.

trade.

less reliable map.

---

<a id="m14-t035-s148"></a>

## 103. BẮC LỘ

Identity:

**Security & information.**

Patrol.

war.

signals.

refugees.

checkpoints.

roads may change control.

---

<a id="m14-t035-s149"></a>

## 104. LOA VỰC

Identity:

**Landscape transformation.**

Same map changes across Acts.

Fields.

survey markers.

construction.

earthworks.

work camps.

military roads.

Cổ Loa.

Exploration itself tells history.

---

<a id="m14-t035-s150"></a>

## PART V — DYNAMIC WORLD MAP

<a id="m14-t035-s151"></a>

## 105. MAP CHANGES OVER YEARS

Campaign dài nhiều thập kỷ.

Map phải thay đổi.

---

<a id="m14-t035-s152"></a>

## 106. SETTLEMENT GROWTH

Hamlet có thể:

grow.

decline.

abandon.

be absorbed.

---

<a id="m14-t035-s153"></a>

## 107. NEW ROUTES

Cổ Loa construction có thể tạo:

road.

canal.

military route.

---

<a id="m14-t035-s154"></a>

## 108. ROUTE DISAPPEARANCE

Old trail có thể:

abandon.

flood.

forest reclaim.

war destroy.

---

<a id="m14-t035-s155"></a>

## 109. LANDSCAPE MEMORY

Map có thể hiển thị old layer nếu character biết.

Ví dụ:

> “Nơi này từng là ruộng nhà Chạ.”

Hiện tại:

earth wall.

---

<a id="m14-t035-s156"></a>

## 110. PERSONAL MAP MEMORY

Player character từng sống ở đây:

dialogue/description riêng.

---

<a id="m14-t035-s157"></a>

## 111. MIGRATION

Refugee movement tạo:

temporary camps.

new settlement.

abandoned villages.

Travel discovers social consequence.

---

<a id="m14-t035-s158"></a>

## 112. WAR FRONT

World Map có:

Control Zone.

Not exact RTS territory.

Information may be outdated.

---

<a id="m14-t035-s159"></a>

## 113. INTELLIGENCE DELAY

Player chỉ biết chiến trường theo:

messenger.

rumor.

faction network.

Không global real-time map.

---

<a id="m14-t035-s160"></a>

## 114. INFORMATION AGE

Map marker có Timestamp.

Ví dụ:

> “Bắc Lộ checkpoint — information from 18 days ago.”

Khi tới:

có thể đã đổi.

---

<a id="m14-t035-s161"></a>

## 115. SIGNAL NETWORK

Khe Cờ và similar sites affect:

information speed.

Nếu destroyed:

map intelligence chậm.

---

<a id="m14-t035-s162"></a>

## 116. PLAYER CAN IMPROVE MAP NETWORK

Through:

messenger.

faction.

guides.

trade network.

Không bằng buying magical map upgrade.

---

<a id="m14-t035-s163"></a>

## PART VI — TRAVEL ENCOUNTER SYSTEM

<a id="m14-t035-s164"></a>

## 117. ENCOUNTER DOES NOT EQUAL COMBAT

Travel encounters gồm:

Social.

Environmental.

Discovery.

Resource.

Narrative.

Combat.

Spiritual.

---

<a id="m14-t035-s165"></a>

## 118. ENCOUNTER ELIGIBILITY

Phụ thuộc:

Route.

Region.

World State.

Time.

Weather.

Party.

Story Threads.

Không universal random table.

---

<a id="m14-t035-s166"></a>

## 119. ENCOUNTER SCORE

Conceptual:

```text
Encounter Weight =
World Relevance
× Route Relevance
× Party Relevance
× Story Thread Relevance
× Novelty
× Current Pressure
```

---

<a id="m14-t035-s167"></a>

## 120. NO ENCOUNTER SPAM

Travel có Encounter Budget.

Một chuyến 3 ngày không cần 7 random events.

Có thể không xảy ra gì.

Điều này làm event có giá trị hơn.

---

<a id="m14-t035-s168"></a>

## 121. BACKGROUND TRAVEL EVENTS

Những việc nhỏ có thể được summarize:

> “Mưa nhẹ trong phần lớn ngày. Đoàn mất thêm vài giờ vì bờ sông lầy.”

Không cần interactive encounter.

---

<a id="m14-t035-s169"></a>

## 122. INTERACTIVE ENCOUNTER

Chỉ expose khi có:

choice.

risk.

story.

meaningful discovery.

---

<a id="m14-t035-s170"></a>

## 123. EXAMPLE SOCIAL ENCOUNTER

Gặp nhóm người di cư.

Player có thể:

talk.

trade.

help.

ignore.

Ask information.

Companion có thể nhận ra relative.

---

<a id="m14-t035-s171"></a>

## 124. EXAMPLE ENVIRONMENTAL

Cầu bị cuốn.

Options:

repair.

ford.

boat.

detour.

wait.

Local help.

Không combat.

---

<a id="m14-t035-s172"></a>

## 125. EXAMPLE DISCOVERY

Khói từ rừng.

Could be:

charcoal camp.

fire.

bandit.

ritual.

Need investigate.

---

<a id="m14-t035-s173"></a>

## 126. EXAMPLE PERSONAL

Companion nhận ra:

grave marker family.

Travel becomes companion story.

---

<a id="m14-t035-s174"></a>

## 127. ENCOUNTER CONSEQUENCE

Can change:

Travel Time.

Supplies.

Health.

Relationship.

Knowledge.

Route State.

Story Thread.

---

<a id="m14-t035-s175"></a>

## PART VII — TRAVEL & PARTY

<a id="m14-t035-s176"></a>

## 128. PARTY COMPOSITION

Exploration strongly depends on party.

Example:

Liệp Nhân.

Dược Nhân.

Sứ Giả.

Xảo Nhân.

Có thể rất mạnh ngoài combat.

---

<a id="m14-t035-s177"></a>

## 129. TRAVEL ROLES

Recommended roles:

<a id="m14-t035-s178"></a>

#### Route Lead

Navigation.

<a id="m14-t035-s179"></a>

#### Scout

Hazard detection.

<a id="m14-t035-s180"></a>

#### Quartermaster

Supplies.

<a id="m14-t035-s181"></a>

#### Forager

Resource.

<a id="m14-t035-s182"></a>

#### Medical Lead

Health.

<a id="m14-t035-s183"></a>

#### Camp Lead

Shelter/rest.

Một người có thể giữ nhiều role.

---

<a id="m14-t035-s184"></a>

## 130. ROLE FAILURE

Không phải auto catastrophe.

Ví dụ Route Lead fail:

delay.

wrong turn.

new discovery.

resource loss.

---

<a id="m14-t035-s185"></a>

## 131. ROUTE LEAD CAN CHANGE

Water:

boatman dẫn.

Mountain:

hunter dẫn.

Political road:

Sứ Giả biết checkpoint.

Context leader như Party Module.

---

<a id="m14-t035-s186"></a>

## 132. PARTY SIZE EFFECT

Larger group:

More supplies.

More visible.

More capability.

Safer.

Smaller:

Stealthy.

Faster.

Less food.

Vulnerable.

---

<a id="m14-t035-s187"></a>

## 133. PARTY SOCIAL FOOTPRINT

4 armed travelers:

settlement reaction khác:

elder + healer + child.

Travel party composition matters socially.

---

<a id="m14-t035-s188"></a>

## 134. COMPANION OBLIGATION

Một 10-day trip có thể:

delay family work.

miss festival.

miss harvest.

Career/social consequences.

---

<a id="m14-t035-s189"></a>

## 135. RETURN DATE

Player can estimate:

> “Nếu đi tuyến này, chúng ta có thể quay lại trước mùa gặt.”

World planning.

---

<a id="m14-t035-s190"></a>

## PART VIII — SURVIVAL & PROFESSION

<a id="m14-t035-s191"></a>

## 136. HUNTER

Reduces:

food pressure.

navigation difficulty.

environmental uncertainty.

---

<a id="m14-t035-s192"></a>

## 137. BOATMAN

River travel.

Boat maintenance.

water hazard.

---

<a id="m14-t035-s193"></a>

## 138. FARMER

Reads:

weather.

crop.

rural landscape.

food condition.

---

<a id="m14-t035-s194"></a>

## 139. HERBALIST

Finds:

medicine.

poison.

water safety clues.

---

<a id="m14-t035-s195"></a>

## 140. MERCHANT

Knows:

safe stops.

trade routes.

prices.

hospitality networks.

---

<a id="m14-t035-s196"></a>

## 141. MESSENGER

Knows:

fast paths.

outposts.

political boundaries.

---

<a id="m14-t035-s197"></a>

## 142. RITUALIST

Knows:

taboo areas.

sacred sites.

ritual hospitality.

---

<a id="m14-t035-s198"></a>

## 143. CRAFTSMAN

Can:

repair tools.

boat.

bridge.

camp equipment.

---

<a id="m14-t035-s199"></a>

## PART IX — EXPLORATION & CLASS

<a id="m14-t035-s200"></a>

## 144. LIỆP NHÂN

Strength:

tracking.

navigation.

terrain.

hunting.

---

<a id="m14-t035-s201"></a>

## 145. TRINH BINH

Strength:

route scouting.

danger detection.

hidden routes.

stealth travel.

---

<a id="m14-t035-s202"></a>

## 146. THỦY BINH

Strength:

river.

boat.

flood.

rescue.

---

<a id="m14-t035-s203"></a>

## 147. DƯỢC NHÂN

Strength:

food/water safety.

disease.

medicine.

---

<a id="m14-t035-s204"></a>

## 148. XẢO NHÂN

Strength:

route repair.

bridge.

boat.

tools.

---

<a id="m14-t035-s205"></a>

## 149. VU CHÚC

Strength:

sacred geography.

Mạch anomaly.

ritual routes.

---

<a id="m14-t035-s206"></a>

## 150. SỨ GIẢ

Strength:

social route.

hospitality.

border.

language.

---

<a id="m14-t035-s207"></a>

## 151. THỦ LĨNH

Strength:

large group travel.

supply.

morale.

---

<a id="m14-t035-s208"></a>

## 152. NGƯỜI GIỮ KÝ ỨC

Strength:

old routes.

place names.

oral maps.

historical landmarks.

---

<a id="m14-t035-s209"></a>

## 153. LẠC BINH / HỘ VỆ

Strength:

security.

escort.

dangerous route.

Not automatically navigation experts.

---

<a id="m14-t035-s210"></a>

## PART X — WORLD MAP UI

<a id="m14-t035-s211"></a>

## 154. WORLD MAP VISUAL

Không nên giống tactical satellite map.

Phong cách:

hand-drawn / symbolic / illustrated geography.

Map evolves with knowledge.

---

<a id="m14-t035-s212"></a>

## 155. MAP UNCERTAINTY VISUAL

Unknown:

blank/faded.

Approximate:

broad region.

Known:

route line.

Familiar:

landmarks.

Intimate:

minor routes.

---

<a id="m14-t035-s213"></a>

## 156. NO UNIVERSAL QUEST MARKER

Quest can say:

> “Người ta nhìn thấy ông lần cuối gần đoạn sông phía tây Cồn Mây.”

Map shows:

Search Area.

Không exact golden diamond.

---

<a id="m14-t035-s214"></a>

## 157. SEARCH AREA

Knowledge reduces area.

Better information:

smaller area.

---

<a id="m14-t035-s215"></a>

## 158. PLAYER NOTES

Có thể cho player pin:

suspected place.

resource.

danger.

personal note.

Rất phù hợp investigation-heavy game.

---

<a id="m14-t035-s216"></a>

## 159. CHARACTER-GENERATED NOTES

Guide may annotate:

> “Không đi đường này khi nước lên.”

Map is social knowledge object.

---

<a id="m14-t035-s217"></a>

## 160. OUTDATED MARKERS

Marker can remain but tagged:

Old.

Unconfirmed.

Example:

“Abandoned camp?”

---

<a id="m14-t035-s218"></a>

## 161. MAP LEGEND

Player discovers cultural symbols.

Không need overwhelming UI.

Core icons:

Settlement.

Route.

Water.

Landmark.

Danger.

Sacred.

Camp.

Known Resource.

Political.

---

<a id="m14-t035-s219"></a>

## 162. MAP FILTERS

Optional:

Geographic.

Political.

Trade.

Spiritual.

Historical.

Personal.

Class/profession can unlock better filters.

---

<a id="m14-t035-s220"></a>

## 163. TRADE MAP

Merchant sees:

goods flow.

market.

route disruption.

---

<a id="m14-t035-s221"></a>

## 164. POLITICAL MAP

Shows known:

influence.

checkpoints.

conflicts.

Not perfect border lines.

---

<a id="m14-t035-s222"></a>

## 165. SPIRITUAL MAP

Vu Chúc may record:

sacred sites.

Mạch anomalies.

taboo areas.

Others may see only shrine.

---

<a id="m14-t035-s223"></a>

## 166. HISTORICAL MAP

Người Giữ Ký Ức:

old settlement.

battle.

old route.

migration.

This could be one of the game's strongest lore interfaces.

---

<a id="m14-t035-s224"></a>

## PART XI — DISCOVERY REWARD

<a id="m14-t035-s225"></a>

## 167. NO EXPLORATION XP FARM

Discovering location can give:

Knowledge.

Route.

Story.

Resource.

Reputation.

Career insight.

Not general XP shower.

---

<a id="m14-t035-s226"></a>

## 168. FIRST DISCOVERY

Can produce Chronicle:

> “Mân first reached Miếu Rễ in year 12.”

---

<a id="m14-t035-s227"></a>

## 169. CARTOGRAPHIC VALUE

A new route can be sold/shared.

Information is resource.

---

<a id="m14-t035-s228"></a>

## 170. ROUTE INFORMATION ECONOMY

A trader may pay for:

safe route.

water source.

pass condition.

Player can:

share freely.

sell.

withhold.

---

<a id="m14-t035-s229"></a>

## 171. DISCOVERY CAN CHANGE WORLD

Find ore:

miners may come.

Find route:

trade shifts.

Find sacred ruin:

ritual/political conflict.

Exploration can create future settlement change.

---

<a id="m14-t035-s230"></a>

## PART XII — HIDDEN LOCATIONS

<a id="m14-t035-s231"></a>

## 172. NOT ALL LOCATIONS EXIST FROM GAME START AS ACTIVE CONTENT

Some can be:

seeded.

activated later.

revealed by event.

---

<a id="m14-t035-s232"></a>

## 173. LOCATION DISCOVERY RULE

Need one or more:

Physical exploration.

Rumor.

Map.

Knowledge.

Tracking.

Event.

Mạch perception.

---

<a id="m14-t035-s233"></a>

## 174. SECRET LOCATION IS NOT MAGICALLY INVISIBLE

It is hidden because:

remote.

forgotten.

concealed.

seasonal.

restricted.

---

<a id="m14-t035-s234"></a>

## 175. PROCEDURAL MINOR LOCATIONS

Can generate:

temporary camp.

hunting shelter.

flood debris.

small grave.

abandoned field.

Not major lore dungeon unless validated.

---

<a id="m14-t035-s235"></a>

## 176. AUTHORED MAJOR LOCATIONS

Need handcrafted history.

Major sacred site.

Cổ Loa.

Important tomb.

Historical battlefield.

---

<a id="m14-t035-s236"></a>

## PART XIII — ENVIRONMENTAL STORYTELLING

<a id="m14-t035-s237"></a>

## 177. LOCATION TELLS STORY WITHOUT TEXT LOG

Examples:

abandoned rice field.

broken boat high in tree after flood.

reused bronze fragments.

burned house foundation.

new fortification.

---

<a id="m14-t035-s238"></a>

## 178. CHARACTER SKILL CHANGES DESCRIPTION

Generic character:

> “Một căn nhà bỏ hoang.”

Builder:

> sees roof failure pattern.

Hunter:

> sees recent animal use.

Historian:

> recognizes older construction style.

---

<a id="m14-t035-s239"></a>

## 179. MULTI-LAYER ENVIRONMENT INFORMATION

Every major site can have:

Visual Layer.

Professional Layer.

Historical Layer.

Spiritual Layer.

Social Layer.

Different characters reveal different layers.

---

<a id="m14-t035-s240"></a>

## PART XIV — TRAVEL EVENTS & TIME

<a id="m14-t035-s241"></a>

## 180. TIME IS REAL RESOURCE

Travel consumes campaign time.

Therefore:

distance matters.

---

<a id="m14-t035-s242"></a>

## 181. TIME SKIP

Player can choose:

Wait.

Recover.

Work.

Travel.

World continues.

---

<a id="m14-t035-s243"></a>

## 182. MISSED EVENTS

Player may arrive after:

funeral.

battle.

wedding.

flood.

Story changes.

Not always failure.

---

<a id="m14-t035-s244"></a>

## 183. LATE ARRIVAL CONTENT

Important principle:

> arriving late should generate different content, not “quest failed” only.

Example:

Need rescue village.

Player arrives late.

Now:

survivor search.

burial.

investigation.

revenge.

rebuilding.

---

<a id="m14-t035-s245"></a>

## 184. EARLY ARRIVAL

Can allow preparation.

Warn village.

build defenses.

evacuate.

---

<a id="m14-t035-s246"></a>

## PART XV — SURVIVAL PRESSURE MODES

<a id="m14-t035-s247"></a>

## 185. STORY MODE

Supplies more forgiving.

Route information clearer.

Getting lost less punishing.

---

<a id="m14-t035-s248"></a>

## 186. STANDARD

Full intended system.

---

<a id="m14-t035-s249"></a>

## 187. EXPLORER MODE

Less map information.

Higher environmental consequence.

More manual route planning.

---

<a id="m14-t035-s250"></a>

## 188. LEGACY / IRONMAN

World time, wounds and supplies fully persistent.

Still no tedious hunger spam.

---

<a id="m14-t035-s251"></a>

## PART XVI — FAILURE DESIGN

<a id="m14-t035-s252"></a>

## 189. GETTING LOST

Failure becomes:

time loss.

new area.

supply pressure.

not instant death.

---

<a id="m14-t035-s253"></a>

## 190. RUNNING OUT OF FOOD

Leads to:

forage.

debt.

retreat.

social dependence.

crime possibility.

---

<a id="m14-t035-s254"></a>

## 191. WEATHER FAILURE

Could:

damage cargo.

injure party.

block route.

force shelter.

---

<a id="m14-t035-s255"></a>

## 192. BOAT FAILURE

Need:

repair.

abandon cargo.

seek settlement.

swim.

rescue.

---

<a id="m14-t035-s256"></a>

## 193. NAVIGATION FAILURE

Can uncover wrong place.

Narrative possibility.

---

<a id="m14-t035-s257"></a>

## 194. SURVIVAL CATASTROPHE

Death should usually be final result of:

bad preparation

+

ignored warning

+

severe condition

+

failed adaptation.

Not one invisible random roll.

---

<a id="m14-t035-s258"></a>

## PART XVII — EXAMPLE JOURNEY

<a id="m14-t035-s259"></a>

## 195. SCENARIO

Party:

Mân — Thủy Binh / Liệp Nhân.

Miên — Dược Nhân.

Nghi — Sứ Giả.

Vũ — Liệp Nhân.

Goal:

Bến Vạc → Mường Thạch.

---

<a id="m14-t035-s260"></a>

## 196. ROUTE A

River to Bến Hạc.

Then land route west.

6 days.

Safe settlements.

High political visibility.

---

<a id="m14-t035-s261"></a>

## 197. ROUTE B

River shortcut.

Forest passage.

4 days.

Need local knowledge.

High environmental risk.

---

<a id="m14-t035-s262"></a>

## 198. PLAYER CHOOSES ROUTE B

Reasons:

urgent.

Mân knows river.

Vũ knows forest.

---

<a id="m14-t035-s263"></a>

## 199. DAY 1

River travel.

Water level unexpectedly high.

Mân predicts current danger.

Player chooses:

continue fast

or

wait half day.

---

<a id="m14-t035-s264"></a>

## 200. CONTINUE

Boat condition worsens.

No combat.

Creates Equipment State.

---

<a id="m14-t035-s265"></a>

## 201. DAY 2

Route reaches forest.

Vũ finds unusual broken branches.

Possible:

travelers.

animal.

pursuer.

Player decides search.

---

<a id="m14-t035-s266"></a>

## 202. SEARCH

Finds injured messenger.

Now choice:

treat.

ignore.

carry.

Miên treats.

Travel delayed.

But messenger gives:

warning about blocked pass.

---

<a id="m14-t035-s267"></a>

## 203. NEW INFORMATION

Original route now dangerous.

Player reroutes.

Delay avoided.

Travel encounter created meaningful information.

---

<a id="m14-t035-s268"></a>

## 204. DAY 3

Food pressure moderate.

Vũ hunts.

Finds sacred deer traces.

Could kill for food.

But local taboo known.

Party chooses forage instead.

Less food.

No combat.

Cultural decision.

---

<a id="m14-t035-s269"></a>

## 205. DAY 4

Rain.

Route confidence falls.

Mân's river knowledge no longer useful.

Vũ leads.

Party finds old trail marker.

Người Giữ Ký Ức companion would have gained additional information if present.

---

<a id="m14-t035-s270"></a>

## 206. DAY 5

Reach Mường Thạch.

Party:

arrives one day later than fastest possible.

Boat damaged.

Food low.

Messenger saved.

Knows pass is blocked.

New relationship.

No random combat occurred.

Yet journey generated story.

Đây là mục tiêu Travel System.

---

<a id="m14-t035-s271"></a>

## PART XVIII — EXAMPLE SAME JOURNEY WITH DIFFERENT PARTY

<a id="m14-t035-s272"></a>

## 207. PARTY

Lạc Binh.

Hộ Vệ.

Thủ Lĩnh.

Xảo Nhân.

Strong combat.

Weak navigation.

---

<a id="m14-t035-s273"></a>

## 208. CONSEQUENCE

They choose safe major road.

Slower.

Checkpoint interaction.

Political story.

Xảo Nhân repairs bridge.

Bandit sees armed group and avoids attack.

Same destination.

Completely different journey.

---

<a id="m14-t035-s274"></a>

## PART XIX — WORLD SIMULATION INTERACTION

<a id="m14-t035-s275"></a>

## 209. FOOD NETWORK

Travel route disrupted:

Bãi Lạc grain cannot reach Phong Châu.

Price rises.

---

<a id="m14-t035-s276"></a>

## 210. METAL NETWORK

Đồng Đỏ route blocked:

weapon/tool production falls.

---

<a id="m14-t035-s277"></a>

## 211. INFORMATION NETWORK

Messenger route blocked:

political information delay.

---

<a id="m14-t035-s278"></a>

## 212. MIGRATION NETWORK

Danger route:

people reroute.

settlements along alternative path grow.

---

<a id="m14-t035-s279"></a>

## 213. PLAYER CAN ALTER ROUTE NETWORK

Repair bridge.

Remove bandit threat.

Negotiate passage.

Build ferry.

Discover shortcut.

Destroy road.

Actions have economic consequences.

---

<a id="m14-t035-s280"></a>

## 214. ROUTE PROJECT

Some major interventions require:

labor.

authority.

materials.

time.

Player cannot single-handedly build major road overnight.

---

<a id="m14-t035-s281"></a>

## 215. XẢO NHÂN / THỦ LĨNH SYNERGY

Xảo Nhân designs.

Thủ Lĩnh organizes labor.

Settlement supplies.

World map gains new route.

Class creates persistent world change.

---

<a id="m14-t035-s282"></a>

## PART XX — MẠCH LẠC & EXPLORATION

<a id="m14-t035-s283"></a>

## 216. MẠCH DOES NOT REPLACE GEOGRAPHY

Spiritual anomalies sit on real environment.

Player still needs route, water, shelter.

---

<a id="m14-t035-s284"></a>

## 217. MẠCH ANOMALY

Can alter:

dream.

animal behavior.

sound.

memory.

water.

navigation perception.

Not random teleport magic by default.

---

<a id="m14-t035-s285"></a>

## 218. SPIRITUAL ROUTE

Some paths may be culturally avoided.

Could be:

actually dangerous.

only taboo.

both.

Player decides.

---

<a id="m14-t035-s286"></a>

## 219. SACRED SITE ACCESS

May require:

ritual permission.

community permission.

knowledge.

Not just find cave entrance.

---

<a id="m14-t035-s287"></a>

## 220. DESECRATION

Player can force access.

Creates:

social.

spiritual.

narrative consequences.

---

<a id="m14-t035-s288"></a>

## PART XXI — FAST TRAVEL PHILOSOPHY

<a id="m14-t035-s289"></a>

## 221. FAST TRAVEL IS ABSTRACTED REPEATED KNOWLEDGE

Not teleport.

---

<a id="m14-t035-s290"></a>

## 222. UNLOCK CONDITIONS

Route known.

No severe current instability.

Party can plausibly travel.

---

<a id="m14-t035-s291"></a>

## 223. COSTS STILL APPLY

World time.

Food.

transport.

Some fatigue.

---

<a id="m14-t035-s292"></a>

## 224. INTERRUPT CONDITIONS

Major Story Thread.

Route State Change.

Companion event.

Catastrophe.

---

<a id="m14-t035-s293"></a>

## 225. NO INTERRUPT FOR LOW-VALUE CONTENT

No:

> wolves again.

---

<a id="m14-t035-s294"></a>

## PART XXII — WORLD MAP & HISTORICAL CAMPAIGN

<a id="m14-t035-s295"></a>

## 226. ACT-BASED MAP TRANSFORMATION

<a id="m14-t035-s296"></a>

#### Early Văn Lang

More local routes.

less centralized infrastructure.

<a id="m14-t035-s297"></a>

#### Fragmentation

security declines.

political checkpoints.

migration.

<a id="m14-t035-s298"></a>

#### Rise of Âu Lạc

new alliances/routes.

<a id="m14-t035-s299"></a>

#### Cổ Loa Construction

Loa Vực changes rapidly.

<a id="m14-t035-s300"></a>

#### Late Crisis

military routes dominate.

border danger.

---

<a id="m14-t035-s301"></a>

## 227. REVISITING LOCATIONS IS REQUIRED

Game should intentionally send player back to places years later.

Payoff:

environment change.

NPC aging.

family change.

construction.

politics.

---

<a id="m14-t035-s302"></a>

## 228. NO STATIC HUB

Phong Châu itself changes.

A place is part of timeline.

---

<a id="m14-t035-s303"></a>

## PART XXIII — UI / PLAYER INFORMATION

<a id="m14-t035-s304"></a>

## 229. TRAVEL PLANNER

Before travel shows:

Estimated duration.

Known hazards.

Supply estimate.

Known stops.

Route confidence.

---

<a id="m14-t035-s305"></a>

## 230. DO NOT SHOW HIDDEN RNG

Not:

`18% ambush`.

Instead:

> “Bandit reports have increased on this road.”

---

<a id="m14-t035-s306"></a>

## 231. PARTY ADVICE

Companions comment:

Mân:

river route safer.

Vũ:

forest route faster.

Nghi:

road checkpoint may recognize us.

Advice creates decision.

---

<a id="m14-t035-s307"></a>

## 232. ADVICE CAN CONFLICT

No system chooses "best".

Player judges.

---

<a id="m14-t035-s308"></a>

## 233. ROUTE CONFIDENCE

Display:

High.

Moderate.

Low.

Unknown.

---

<a id="m14-t035-s309"></a>

## 234. SUPPLY ESTIMATE

Example:

> Expected 4–5 days.

> Food carried: approximately 7 party-days.

Useful without spreadsheet overload.

---

<a id="m14-t035-s310"></a>

## 235. TRAVEL LOG

Chronicle records only meaningful items:

route discovered.

major encounter.

companion event.

not every meal.

---

<a id="m14-t035-s311"></a>

## PART XXIV — EXPLORATION REPLAYABILITY

<a id="m14-t035-s312"></a>

## 236. WORLD SEED

Changes:

minor route condition.

resource.

life events.

settlement state.

NPC availability.

Not major geography anchors.

---

<a id="m14-t035-s313"></a>

## 237. FIXED GEOGRAPHY + DYNAMIC HISTORY

Important mountains/rivers/settlements mostly fixed.

What changes:

who controls.

who lives.

route safety.

resources.

story.

---

<a id="m14-t035-s314"></a>

## 238. WHY NOT FULL PROCEDURAL WORLD

Because:

lore quality.

historical identity.

meaningful geography.

Need handcrafted backbone.

Procedural layer fills life, not replaces world design.

---

<a id="m14-t035-s315"></a>

## PART XXV — EXPLORATION QUALITY GATE

<a id="m14-t035-s316"></a>

## 239. REGION QUALITY

Every region must answer:

1. What makes movement here unique?
2. What skill matters most?
3. What profession has an advantage?
4. What environmental risk exists?
5. What seasonal change matters?
6. What social navigation matters?
7. What route type dominates?
8. What exploration story can only happen here?
9. How does the region change over 20 years?

---

<a id="m14-t035-s317"></a>

## 240. ROUTE QUALITY

Every major route must have:

Purpose.

Terrain.

Users.

Economic role.

Risk.

Seasonal behavior.

Political context.

At least one reason to choose it.

At least one reason not to.

---

<a id="m14-t035-s318"></a>

## 241. TRAVEL ENCOUNTER QUALITY

An encounter is interactive only if:

there is a meaningful decision

or

new meaningful information

or

persistent consequence.

Otherwise summarize it.

---

<a id="m14-t035-s319"></a>

## 242. SURVIVAL QUALITY

Survival mechanic must create:

choice.

Not repetitive maintenance.

If a resource creates constant clicking but no decisions:

abstract it.

---

<a id="m14-t035-s320"></a>

## 243. MAP QUALITY

World Map must answer:

what player knows

not

what game engine knows.

---

<a id="m14-t035-s321"></a>

## PART XXVI — INTERACTION WITH PREVIOUS SYSTEMS

<a id="m14-t035-s322"></a>

## 244. CHARACTER GENERATION

Origin decides initial:

Map Knowledge.

Region Knowledge.

Travel Skill.

Route contacts.

---

<a id="m14-t035-s323"></a>

## 245. PSYCHOLOGY

Fear:

deep water.

forest.

isolation.

can affect travel.

Curiosity can encourage exploration.

---

<a id="m14-t035-s324"></a>

## 246. PROFESSION

Career creates:

route knowledge.

network.

travel purpose.

---

<a id="m14-t035-s325"></a>

## 247. CLASS

Class changes:

how player sees and solves environment.

---

<a id="m14-t035-s326"></a>

## 248. PARTY

Companions provide:

knowledge.

roles.

social access.

---

<a id="m14-t035-s327"></a>

## 249. EQUIPMENT

Load.

weather protection.

boat.

tools.

weapons.

affect travel.

---

<a id="m14-t035-s328"></a>

## 250. INJURY

Leg wound:

travel speed.

Need transport.

Recovery.

---

<a id="m14-t035-s329"></a>

## 251. NARRATIVE ENGINE

Travel events come from Story Threads and World State.

---

<a id="m14-t035-s330"></a>

## 252. ECONOMY

Routes move goods.

---

<a id="m14-t035-s331"></a>

## 253. FACTIONS

Control routes.

Checkpoints.

safety.

---

<a id="m14-t035-s332"></a>

## 254. REPUTATION

Provides:

hospitality.

information.

access.

---

<a id="m14-t035-s333"></a>

## 255. COMBAT

Preparation/terrain often comes from exploration.

Combat can start from travel encounter.

---

<a id="m14-t035-s334"></a>

## 256. LEGACY

Player can leave:

route.

bridge.

ferry.

settlement.

map knowledge.

---

<a id="m14-t035-s335"></a>

## PART XXVII — DEFERRED TO PRODUCTION

Production will later define:

- actual geographic scale;
- exact travel-day duration;
- exact ration consumption;
- all weather probabilities;
- exact season calendar;
- full route network;
- boat statistics;
- resource spawn tables;
- full regional ecology;
- detailed map art;
- procedural minor site generation;
- fast-travel thresholds;
- survival balance;
- local flora/fauna catalogs;
- all route events.

---

<a id="m14-t035-s336"></a>

## 257. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

World Map phản ánh character knowledge, không omniscient.

World có 4 spatial layers.

Travel dùng route network.

Routes có dynamic state.

Travel consumes real campaign time.

Player lập kế hoạch route/supply/party.

Travel có speed modes.

Weather và season có gameplay effect.

Party roles tác động hành trình.

Survival được abstract để tránh micromanagement.

Food dùng Party-Day abstraction.

Water chỉ chi tiết khi context cần.

Rest/Fatigue liên kết combat.

Camp là social + recovery point, không base-builder.

Foraging/hunting dùng time trade-off.

Exploration dựa vào Observation + Skill + Knowledge.

Không pixel hunt.

Tracking có persistent traces.

Getting lost là gameplay state, không teleport punishment.

Repeated familiar travel được abstract thành fast travel.

Fast travel vẫn tiêu tốn world time.

Không spam random encounter.

Travel encounter không đồng nghĩa combat.

12 vùng phải có exploration identity khác nhau.

World Map thay đổi qua nhiều năm.

Loa Vực phải biến đổi rõ rệt nhất.

Political/economic route changes được mô phỏng.

Class/Profession mở cách nhìn địa hình khác nhau.

Player có thể tạo hoặc thay đổi route.

Exploration discovery có thể thay đổi economy/world state.

Mạch Lạc tồn tại trên nền địa lý thật.

Survival failure ưu tiên fail-forward.

Historical anchors thay đổi bản đồ theo thời gian.

---

<a id="m14-t035-s337"></a>

## 258. CORE EXPLORATION STATEMENT

Exploration của game không nên hỏi:

> “Có bao nhiêu icon trên bản đồ?”

Mà phải hỏi:

> “Tôi thực sự biết gì về vùng đất này?”

Travel không nên hỏi:

> “Tôi có muốn xem animation đi đường không?”

Mà:

> “Tôi sẵn sàng mất bao nhiêu ngày, mang theo ai và chấp nhận rủi ro nào để tới nơi?”

Survival không nên hỏi:

> “Thanh đói còn bao nhiêu phần trăm?”

Mà:

> “Nếu ở lại đây thêm hai ngày để tìm người mất tích, chúng tôi có đủ thức ăn để quay về không?”

World Map không phải menu.

Nó là:

> **bản ghi về mối quan hệ giữa character và thế giới.**

Lúc đầu bản đồ có rất nhiều khoảng trống.

Sau vài chục năm:

nó chứa những tuyến player đã đi.

những nơi companion đã chết.

những làng không còn tồn tại.

những cây cầu player đã xây.

những bãi chiến trường.

những nơi từng là ruộng trước khi Cổ Loa xuất hiện.

Khi đó bản đồ không chỉ thể hiện địa lý.

Nó thể hiện:

> **một cuộc đời đã đi qua thế giới như thế nào.**

Với module này, vòng gameplay lớn hiện đã kết nối khá đầy đủ: **Character → Party → Travel → Exploration → Encounter → Combat/Social → Consequence → World Change → Travel tiếp**. Bước hợp lý nhất tiếp theo là **Dungeon + Encounter System**, nhưng tôi sẽ thiết kế “dungeon” rất rộng: hang, mộ, rừng, đầm lầy, chiến trường, làng bị bỏ hoang, lễ hội, giấc mơ/Mạch Lạc… với cấu trúc mục tiêu, tension, discovery và nhiều lối giải quyết, chứ không phải chuỗi phòng có quái.

<!-- SOURCE T035 END -->

