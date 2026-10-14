# Module 20 — Sinh vật, sinh thái và mối đe dọa con người

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T047](../../source/conversation.md#t047)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Không monster level scaling. Population pool và cá thể persistent phải nhất quán; Human Threat vẫn là Characters có đời sống, không mob archetype vô danh mặc định.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Habitat, food, weather, territory, population pressure, human activity. |
| Đầu ra / bên sở hữu | Creature behavior, eligible encounters và population deltas; Ecology sở hữu. |
| Đọc cùng | 14, 15, 18, 19, 27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Exceptional animal đã chết không xuất hiện lại; encounter có lý do sinh thái. |
| Việc chưa được chứng minh | Species data, population models và simulation-tier budgets. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T047 BEGIN -->

<a id="m20-t047-s001"></a>

## GDD MODULE 20  
<a id="m20-t047-s002"></a>

## CREATURE, BESTIARY, ECOLOGY & HUMAN THREAT SYSTEM

---

<a id="m20-t047-s003"></a>

## PART I — DESIGN PHILOSOPHY

<a id="m20-t047-s004"></a>

### 1. MỤC TIÊU

Creature System không được xây theo mô hình:

> vùng cấp 1 có sói → vùng cấp 10 có sói lớn → vùng cấp 20 có sói ma.

Thế giới không tự sinh quái để phù hợp Level của player.

Sinh vật tồn tại vì:

- môi trường;
- nguồn thức ăn;
- mùa;
- nơi sinh sản;
- lãnh thổ;
- hoạt động của con người;
- biến động khí hậu;
- chiến tranh;
- Mạch Lạc.

Người chơi có thể gặp một con thú nguy hiểm ngay từ đầu game nếu đi sai nơi.

Và đến cuối game, một con thú bình thường vẫn nguy hiểm nếu điều kiện bất lợi.

---

<a id="m20-t047-s005"></a>

## 2. BỐN LỚP CREATURE

Hệ thống chia sinh vật thành bốn tầng rõ ràng.

<a id="m20-t047-s006"></a>

### Tầng 1 — Natural Animal

Động vật bình thường của thế giới.

Chúng có ecology và hành vi tự nhiên.

<a id="m20-t047-s007"></a>

### Tầng 2 — Exceptional Animal

Một cá thể hoặc quần thể bất thường vì:

- tuổi;
- kinh nghiệm;
- kích thước;
- thương tích;
- môi trường;
- con người.

Không mặc định supernatural.

<a id="m20-t047-s008"></a>

### Tầng 3 — Mythologized Creature

Một animal/entity được cộng đồng kể thành truyền thuyết.

Sự thật và reputation có thể khác nhau.

<a id="m20-t047-s009"></a>

### Tầng 4 — Supernatural Entity / Manifestation

Thực thể thực sự có quan hệ với Mạch hoặc metaphysics.

Dùng hệ thống từ Module 19.

Không được gộp chung vào animal AI.

---

<a id="m20-t047-s010"></a>

## 3. HUMAN THREAT LÀ HỆ RIÊNG

Con người không được xếp vào bestiary như “Humanoid Enemy”.

Một nhóm cướp, quân lính hoặc thợ săn đối địch có:

- gia đình;
- faction;
- kinh tế;
- mục tiêu;
- morale;
- nguồn tiếp tế;
- lịch sử.

Họ sử dụng Character System giống player.

---

<a id="m20-t047-s011"></a>

## 4. CREATURE KHÔNG CÓ LEVEL

Không tồn tại:

`Wild Boar Level 8`.

Creature có:

```text
Species Profile
Age
Sex
Health
Condition
Experience
Territory
Behavior
Fear
Hunger
Injury
Group Structure
Environmental Familiarity
```

Difficulty sinh từ sự kết hợp này.

---

<a id="m20-t047-s012"></a>

## 5. CREATURE POWER KHÔNG SCALE THEO PLAYER

Một predator không tự tăng HP khi player mạnh lên.

Thay vào đó late-game difficulty đến từ:

- hoàn cảnh;
- số lượng;
- terrain;
- surprise;
- mục tiêu bảo vệ;
- injury;
- creature experience.

---

<a id="m20-t047-s013"></a>

## PART II — ECOLOGY SIMULATION

<a id="m20-t047-s014"></a>

### 6. ECOLOGY LÀ NỀN CỦA BESTIARY

Creature encounter phải trả lời được:

> Tại sao loài này lại ở đây?

Nếu không trả lời được:

encounter không nên tồn tại.

---

<a id="m20-t047-s015"></a>

## 7. SPECIES ECOLOGY PROFILE

Mỗi species có:

```text
Habitat
Diet
Activity Cycle
Territory
Social Structure
Reproduction
Seasonality
Migration
Predators
Prey
Human Relationship
Resource Value
Threat Profile
```

---

<a id="m20-t047-s016"></a>

## 8. HABITAT

Một loài có thể thích:

- forest;
- wetland;
- grassland;
- river;
- coastal;
- mountain;
- settlement edge.

Habitat quyết encounter eligibility.

---

<a id="m20-t047-s017"></a>

## 9. DIET

Core categories:

- Herbivore;
- Omnivore;
- Carnivore;
- Scavenger;
- Aquatic feeder.

Diet nối trực tiếp resource simulation.

---

<a id="m20-t047-s018"></a>

## 10. FOOD WEB

Nếu prey population giảm:

predator có thể:

- giảm population;
- di chuyển;
- tấn công livestock;
- tới gần settlement.

Một ecological change có thể tạo Story Thread.

---

<a id="m20-t047-s019"></a>

## 11. TERRITORY

Một số species có territory.

Đi qua territory có thể:

không gặp gì.

nhưng nếu gần:

- den;
- young;
- feeding ground,

aggression tăng.

---

<a id="m20-t047-s020"></a>

## 12. TERRITORY KNOWLEDGE

Hunter/local character có thể biết:

> “Khúc suối này thường có thú lớn uống nước vào cuối ngày.”

Đó là Knowledge, không minimap spawn marker.

---

<a id="m20-t047-s021"></a>

## 13. POPULATION UNIT

Game không cần instantiate từng con thú trên thế giới.

Mỗi region giữ Population Pool.

Ví dụ:

```text
Species: Wild Boar
Region: Tản Viên
Population State: Healthy
Pressure: Moderate Hunting
Movement: Stable
```

Cá thể chỉ được instantiate khi relevant.

---

<a id="m20-t047-s022"></a>

## 14. POPULATION STATES

Các trạng thái:

<a id="m20-t047-s023"></a>

#### Abundant

Dễ gặp.

<a id="m20-t047-s024"></a>

#### Healthy

Ổn định.

<a id="m20-t047-s025"></a>

#### Reduced

Giảm rõ.

<a id="m20-t047-s026"></a>

#### Rare

Khó gặp.

<a id="m20-t047-s027"></a>

#### Locally Absent

Không còn trong vùng.

Population không tự reset.

---

<a id="m20-t047-s028"></a>

## 15. REPRODUCTION

Quần thể tăng theo:

- breeding;
- food;
- habitat;
- hunting pressure;
- disease.

Không cần simulation từng birth.

---

<a id="m20-t047-s029"></a>

## 16. SEASON

Season ảnh hưởng:

- breeding;
- migration;
- aggression;
- feeding;
- visibility;
- hunting yield.

---

<a id="m20-t047-s030"></a>

## 17. MIGRATION

Một số animal groups có thể thay đổi vùng hoạt động theo:

- mùa;
- lũ;
- food;
- disturbance.

Điều này làm world map sống.

---

<a id="m20-t047-s031"></a>

## 18. HUMAN EXPANSION

Khi settlement mở rộng:

habitat bị thu hẹp.

Animals có thể:

- retreat;
- adapt;
- raid fields;
- disappear locally.

Cổ Loa expansion phải có ecological consequence.

---

<a id="m20-t047-s032"></a>

## 19. WAR & ECOLOGY

War tạo:

- abandoned fields;
- corpses;
- fewer hunters;
- forest regrowth ở một số nơi;
- scavenger increase;
- displaced animals.

War ecology phải khác peaceful ecology.

---

<a id="m20-t047-s033"></a>

## 20. FLOOD & ECOLOGY

Flood có thể:

- relocate animals;
- destroy nests;
- create fish abundance;
- force predators near people.

Tam Giang đặc biệt chịu ảnh hưởng.

---

<a id="m20-t047-s034"></a>

## 21. OVERHUNTING

Player và NPC hunting có cumulative effect.

Nếu hunting quá mức:

population giảm.

Không spawn lại ngày hôm sau.

---

<a id="m20-t047-s035"></a>

## 22. LOCAL EXTINCTION

Một species có thể biến mất khỏi micro-region.

Consequence:

- food web;
- economy;
- ritual;
- myth;
- hunting profession.

Đây là world consequence nghiêm túc.

---

<a id="m20-t047-s036"></a>

## 23. ECOLOGICAL RECOVERY

Nếu pressure giảm:

quần thể có thể phục hồi qua nhiều mùa/năm.

Không tức thì.

---

<a id="m20-t047-s037"></a>

## PART III — CREATURE BEHAVIOR

<a id="m20-t047-s038"></a>

### 24. BEHAVIOR PHILOSOPHY

Animal không có AI kiểu:

> thấy player → attack until dead.

Mỗi animal có motive tại thời điểm encounter.

---

<a id="m20-t047-s039"></a>

## 25. CORE BEHAVIOR STATES

Các trạng thái chính:

- Resting;
- Feeding;
- Traveling;
- Searching;
- Alert;
- Threatened;
- Territorial;
- Hunting;
- Fleeing;
- Protecting Young;
- Injured;
- Trapped.

---

<a id="m20-t047-s040"></a>

## 26. BEHAVIOR TRANSITION

Ví dụ:

Feeding

→ nghe noise

→ Alert

→ nhận diện human

→ Retreat.

Không phải:

Detection → Combat.

---

<a id="m20-t047-s041"></a>

## 27. FEAR

Animals có Fear Profile.

Human.

Fire.

Noise.

Group size.

Unknown object.

Một predator lớn vẫn không muốn fight vô ích.

---

<a id="m20-t047-s042"></a>

## 28. AGGRESSION

Aggression tăng do:

- territory;
- young;
- hunger;
- injury;
- trapping;
- disease;
- Mạch disturbance.

---

<a id="m20-t047-s043"></a>

## 29. INJURED ANIMAL

Animal bị thương có thể:

- flee;
- hide;
- become more dangerous if cornered;
- fail hunting.

Injury persists.

---

<a id="m20-t047-s044"></a>

## 30. OLD EXPERIENCED ANIMAL

Một cá thể già có thể:

- tránh trap;
- nhận hunter pattern;
- thay route.

Difficulty không cần stat inflation.

---

<a id="m20-t047-s045"></a>

## 31. HUMAN-HABITUATED ANIMAL

Một animal sống gần người lâu có thể:

- ít sợ hơn;
- biết food location;
- recognize traps.

Đây có thể trở thành settlement problem.

---

<a id="m20-t047-s046"></a>

## 32. NOCTURNAL / DIURNAL BEHAVIOR

Activity Cycle ảnh hưởng encounter.

Một route an toàn ban ngày có thể nguy hiểm ban đêm.

---

<a id="m20-t047-s047"></a>

## 33. SOUND

Animal nghe:

- bước chân;
- tiếng kim loại;
- nói chuyện;
- fire;
- boat.

Party stealth và equipment ảnh hưởng.

---

<a id="m20-t047-s048"></a>

## 34. SMELL

Một số animal có smell detection mạnh.

Wind Direction có thể tác động hunting.

Không cần simulation cực vi mô nhưng dùng trong major encounters.

---

<a id="m20-t047-s049"></a>

## 35. WEATHER

Rain:

giảm scent/trail.

Fog:

visibility.

Wind:

smell/noise.

Storm:

animal behavior thay đổi.

---

<a id="m20-t047-s050"></a>

## PART IV — SOCIAL ANIMALS

<a id="m20-t047-s051"></a>

### 36. GROUP STRUCTURE

Một số animals sống:

- solitary;
- pair;
- herd;
- pack;
- family group.

Group behavior khác solo.

---

<a id="m20-t047-s052"></a>

## 37. HERD

Herd ưu tiên:

- protection;
- escape;
- young.

Không fight tactical như soldiers.

---

<a id="m20-t047-s053"></a>

## 38. PACK

Pack animal có thể:

- probe;
- isolate weak target;
- retreat.

Nhưng không phải telepathic perfect coordination.

---

<a id="m20-t047-s054"></a>

## 39. DOMINANCE

Một group có individual leadership nhưng không nhất thiết “alpha boss”.

Production research quyết cụ thể cho từng species.

---

<a id="m20-t047-s055"></a>

## 40. YOUNG

Young animals ảnh hưởng:

- adult aggression;
- hunting morality;
- resource yield;
- season.

---

<a id="m20-t047-s056"></a>

## PART V — HUNTING SYSTEM

<a id="m20-t047-s057"></a>

### 41. HUNTING KHÔNG PHẢI COMBAT RANDOM

Hunting là một gameplay loop riêng.

```text
Locate
→ Interpret Sign
→ Approach
→ Position
→ Decide
→ Kill / Capture / Leave
→ Process
→ Transport
```

---

<a id="m20-t047-s058"></a>

## 42. LOCATE

Player cần:

- Knowledge;
- trail;
- feeding ground;
- local rumor.

Không spawn animal marker tự động.

---

<a id="m20-t047-s059"></a>

## 43. SIGN

Possible clues:

- footprint;
- droppings;
- hair;
- broken vegetation;
- feeding marks;
- sound;
- smell.

---

<a id="m20-t047-s060"></a>

## 44. IDENTIFICATION

GIÁC thấy clue.

Tracking/Hunting Skill hiểu clue.

Knowledge xác định species/context.

---

<a id="m20-t047-s061"></a>

## 45. TRAIL FRESHNESS

Trail có age.

Rain/traffic degrade.

Player có Confidence, không exact timer.

---

<a id="m20-t047-s062"></a>

## 46. STALKING

Player điều chỉnh:

- speed;
- noise;
- wind;
- route.

Stealth/hunting skill matter.

---

<a id="m20-t047-s063"></a>

## 47. SHOT / ATTACK DECISION

Hunter phải cân nhắc:

- range;
- target age;
- angle;
- escape route;
- group reaction.

Một bad shot có thể chỉ làm animal bị thương và chạy.

---

<a id="m20-t047-s064"></a>

## 48. WOUNDED PREY

Nếu prey chạy:

Tracking thread tiếp tục.

Hunter có ethical/practical incentive finish nhanh.

---

<a id="m20-t047-s065"></a>

## 49. LOST WOUNDED ANIMAL

Nếu không tìm lại:

animal có thể chết later.

Resource wasted.

Could attract scavengers.

Player action persists.

---

<a id="m20-t047-s066"></a>

## 50. PROCESSING

Kill không tự thành food.

Cần:

- butcher;
- preserve;
- carry.

Skill ảnh hưởng yield.

---

<a id="m20-t047-s067"></a>

## 51. YIELD

Yield gồm:

- food;
- hide/material;
- bone/horn;
- special parts nếu plausible.

Không mọi animal drop crafting loot.

---

<a id="m20-t047-s068"></a>

## 52. CARRY LIMIT

Large animal tạo logistics problem.

Player có thể:

process tại chỗ.

bring help.

abandon part.

---

<a id="m20-t047-s069"></a>

## 53. HUNTING FOR PROFIT

Hunters có thể sống bằng:

- food;
- hide;
- service;
- guiding.

Hunting là Career gameplay.

---

<a id="m20-t047-s070"></a>

## 54. TROPHY

Trophy có thể mang social value.

Không universal buff.

---

<a id="m20-t047-s071"></a>

## 55. SACRED HUNT

Một species có taboo/status.

Killing it có social consequence dù economically valuable.

---

<a id="m20-t047-s072"></a>

## PART VI — TRAP SYSTEM

<a id="m20-t047-s073"></a>

### 56. TRAP PHILOSOPHY

Trap là preparation tool.

Không phải instant summon object giữa combat.

---

<a id="m20-t047-s074"></a>

## 57. TRAP TYPES

Functional groups:

- Snare;
- Pit;
- Net;
- Noise Alert;
- Funnel;
- Fishing Trap.

Exact historical forms researched later.

---

<a id="m20-t047-s075"></a>

## 58. TRAP REQUIREMENTS

Need:

- design knowledge;
- material;
- time;
- correct location.

---

<a id="m20-t047-s076"></a>

## 59. TRAP PLACEMENT

Hunting Knowledge identifies:

- path;
- feeding area;
- crossing.

Bad placement gives low yield.

---

<a id="m20-t047-s077"></a>

## 60. TRAP CHECKING

Trap must be checked.

Leaving too long:

- animal suffering;
- scavenging;
- damage;
- loss.

---

<a id="m20-t047-s078"></a>

## 61. NON-TARGET CAPTURE

A trap can catch wrong animal.

Ecological/moral consequence.

---

<a id="m20-t047-s079"></a>

## 62. HUMAN TRAP

Some hunting traps may harm people.

If route changes:

danger.

Player responsible.

---

<a id="m20-t047-s080"></a>

## PART VII — FISHING & AQUATIC ECOLOGY

<a id="m20-t047-s081"></a>

### 63. FISHING SYSTEM

Routine fishing abstracted for professionals.

Major fishing/hunting events can be interactive.

---

<a id="m20-t047-s082"></a>

## 64. FISH POPULATION

Affected by:

- season;
- flood;
- water quality;
- overharvest;
- construction.

---

<a id="m20-t047-s083"></a>

## 65. WATER INFRASTRUCTURE EFFECT

Dyke/canal may:

increase agriculture

but

change fish access.

Economic/environment trade-off.

---

<a id="m20-t047-s084"></a>

## 66. AQUATIC THREATS

Could include:

- dangerous current;
- large aquatic animal;
- crocodilian-type predator where appropriate;
- venomous animals.

Threat often environment + animal combined.

---

<a id="m20-t047-s085"></a>

## PART VIII — DOMESTIC & WORKING ANIMALS

<a id="m20-t047-s086"></a>

### 67. DOMESTIC ANIMAL PHILOSOPHY

Domestic animals are not inventory items.

They are living assets with:

- owner;
- health;
- feeding;
- behavior;
- value.

---

<a id="m20-t047-s087"></a>

## 68. USES

Depending historical validation:

- food;
- agriculture;
- guarding;
- hunting;
- transport support.

Exact species/use combination determined during production research.

---

<a id="m20-t047-s088"></a>

## 69. ANIMAL HANDLING

Skill governs:

- calm;
- lead;
- train;
- treat basic behavior.

Not mind control.

---

<a id="m20-t047-s089"></a>

## 70. HUNTING DOG / WORKING DOG

If included after research:

can help:

- tracking;
- alert;
- hunting.

But:

needs food.

can be injured.

has relationship.

---

<a id="m20-t047-s090"></a>

## 71. ANIMAL DEATH

A valued animal dying can be:

economic loss.

emotional event.

Not “replace pet at vendor”.

---

<a id="m20-t047-s091"></a>

## PART IX — DISEASE & PARASITE ECOLOGY

<a id="m20-t047-s092"></a>

### 72. CREATURES CAN TRANSMIT RISK

Animals may carry:

- parasites;
- infection;
- venom.

But game should not turn every animal into disease RNG.

---

<a id="m20-t047-s093"></a>

## 73. DISEASE VECTOR

Certain ecological conditions increase disease risk:

- stagnant water;
- high population density;
- carcasses.

Dược Nhân can interpret.

---

<a id="m20-t047-s094"></a>

## 74. CARCASS MANAGEMENT

Dead animal near settlement can:

- contaminate;
- attract scavengers.

World cleanup matters.

---

<a id="m20-t047-s095"></a>

## PART X — REGIONAL ECOLOGY

<a id="m20-t047-s096"></a>

### 75. PHONG CHÂU

Ecological identity:

human-dominated mixed landscape.

Threat profile:

- livestock conflict;
- crop-raiding animals;
- displaced wildlife.

Wild threats less frequent than political/social threats.

---

<a id="m20-t047-s097"></a>

## 76. TAM GIANG

Identity:

river ecology.

Key mechanics:

- seasonal fish;
- flood displacement;
- wetland animals;
- aquatic predators;
- birds.

Human-water relationship dominates.

---

<a id="m20-t047-s098"></a>

## 77. TẢN VIÊN

Identity:

mountain and forest.

Threats:

- large animals;
- difficult terrain;
- territorial predators;
- hunting conflict.

High biodiversity.

---

<a id="m20-t047-s099"></a>

## 78. BÃI LẠC

Identity:

agricultural ecology.

Issues:

- crop pests;
- field-raiding animals;
- water birds;
- predators near settlement.

Human modification drives ecology.

---

<a id="m20-t047-s100"></a>

## 79. ĐỒNG ĐỎ

Identity:

mining/forest edge.

Issues:

- habitat damage;
- smoke/fire;
- mine animals;
- displaced wildlife.

Industrial pressure visible.

---

<a id="m20-t047-s101"></a>

## 80. LÂM U

Identity:

deep forest.

High uncertainty.

Many tracks.

Dense cover.

Plants/animals interact strongly with exploration.

---

<a id="m20-t047-s102"></a>

## 81. THƯỢNG ÂU

Identity:

mountain corridors.

Seasonal movement.

Animals may follow altitude/food changes.

---

<a id="m20-t047-s103"></a>

## 82. ĐẠI TRẠCH

Identity:

marsh ecosystem.

Threat:

- venom;
- water;
- disease;
- hidden predators;
- insects.

Environmental threat often more important than combat.

---

<a id="m20-t047-s104"></a>

## 83. HẢI MÔN

Identity:

estuary/coastal ecology.

Fishing.

Birds.

Storm effects.

Saltwater/freshwater transition.

---

<a id="m20-t047-s105"></a>

## 84. VIỆT THƯỜNG

Identity:

frontier ecology.

Player has lower knowledge confidence.

Unfamiliar species/behaviors matter more than stat difficulty.

---

<a id="m20-t047-s106"></a>

## 85. BẮC LỘ

Identity:

disturbed frontier.

War causes:

- abandoned land;
- scavengers;
- displaced wildlife.

Human threats dominate.

---

<a id="m20-t047-s107"></a>

## 86. LOA VỰC

Identity changes through campaign.

Early:

wetland/agriculture.

Construction:

habitat disruption.

Late:

dense human center.

Wildlife retreats/adapts.

Ecology visually tells political transformation.

---

<a id="m20-t047-s108"></a>

## PART XI — REPRESENTATIVE NATURAL THREAT FAMILIES

<a id="m20-t047-s109"></a>

### 87. LARGE HERBIVORE

Fantasy:

danger through mass, fear and territorial/protective behavior.

Combat identity:

- charge;
- knockback;
- trampling risk.

Solution:

- avoid;
- redirect;
- scare;
- wait.

Not monster hunt by default.

---

<a id="m20-t047-s110"></a>

## 88. WILD BOAR-LIKE THREAT

Fantasy:

short-range explosive danger.

Behavior:

usually avoids humans unless:

- wounded;
- cornered;
- protecting young.

Combat:

fast charge.

low profile.

danger in dense terrain.

---

<a id="m20-t047-s111"></a>

## 89. LARGE CAT PREDATOR

Fantasy:

ambush and fear.

Behavior:

stealth.

territorial.

rare.

Should not attack party of four routinely.

Human conflict increases if prey/habitat pressure changes.

---

<a id="m20-t047-s112"></a>

## 90. CANID / PACK SCAVENGER

Fantasy:

group pressure.

Often:

scavenging before attacking.

Can test wounded/isolated target.

Morale/fear more important than raw damage.

---

<a id="m20-t047-s113"></a>

## 91. SNAKES / VENOMOUS ANIMALS

Threat:

detection.

avoidance.

venom.

Not tactical boss.

Dược Nhân/Survival strong.

---

<a id="m20-t047-s114"></a>

## 92. AQUATIC PREDATOR

Threat combines:

water terrain

+

animal attack.

Danger strongest when character:

- swimming;
- injured;
- unaware.

---

<a id="m20-t047-s115"></a>

## 93. LARGE BIRD / SCAVENGER

Usually noncombat.

Useful ecological signal:

- carcass;
- battlefield;
- predator activity.

Observation can use bird behavior as clue.

---

<a id="m20-t047-s116"></a>

## 94. INSECT SWARM / SMALL ORGANISM

Not “enemy unit”.

Environmental condition:

- rest disruption;
- disease;
- morale.

---

<a id="m20-t047-s117"></a>

## 95. ALL CREATURE FAMILIES REQUIRE PRODUCTION RESEARCH

Exact species roster must be historically/ecologically checked later.

GDD locks behavior roles, not full zoological catalog.

---

<a id="m20-t047-s118"></a>

## PART XII — EXCEPTIONAL ANIMALS

<a id="m20-t047-s119"></a>

### 96. WHY AN INDIVIDUAL BECOMES NOTABLE

Possible causes:

- unusual size;
- old age;
- surviving hunters;
- distinctive scar;
- rare coloration;
- repeated livestock attacks;
- association with sacred site.

---

<a id="m20-t047-s120"></a>

## 97. UNIQUE IDENTITY

Exceptional Animal becomes instantiated NPC-like entity.

Has:

Name/Local Name.

History.

Territory.

Encounter Memories.

Injuries.

---

<a id="m20-t047-s121"></a>

## 98. ANIMAL LEARNS PLAYER

If player fails hunt:

animal can remember:

- human smell/context;
- trap type;
- route.

No human-like intelligence, but experience persists.

---

<a id="m20-t047-s122"></a>

## 99. LOCAL NAME

People may call it:

> “Con Lợn Sứt Tai.”

Not:

Epic Boar Boss Level 18.

---

<a id="m20-t047-s123"></a>

## 100. HUNTING STORY THREAD

Repeated conflict can become:

- economic;
- personal;
- ritual.

Example:

animal raids fields because habitat lost.

Killing it solves symptom, not ecological cause.

---

<a id="m20-t047-s124"></a>

## PART XIII — MYTHOLOGIZED ANIMAL

<a id="m20-t047-s125"></a>

### 101. PROCESS

Exceptional Animal

↓

Rumor.

↓

Repeated sightings.

↓

Exaggeration.

↓

Symbol.

↓

Mythologized Creature.

---

<a id="m20-t047-s126"></a>

## 102. EXAMPLE

Truth:

large white animal seen near sacred forest.

Rumor:

it appears before flood.

Years later:

> “Thú trắng canh cửa núi.”

Game need not introduce supernatural fact.

---

<a id="m20-t047-s127"></a>

## 103. REPUTATION AFFECTS BEHAVIOR OF HUMANS

Hunters may refuse.

Villagers leave offerings.

Faction uses story politically.

Myth affects gameplay even if animal is natural.

---

<a id="m20-t047-s128"></a>

## PART XIV — SUPERNATURAL CREATURE BOUNDARY

<a id="m20-t047-s129"></a>

### 104. NOT EVERY STRANGE ANIMAL IS SUPERNATURAL

An unusual animal can be:

- rare;
- injured;
- diseased;
- mythologized;
- Mạch-responsive.

Investigation matters.

---

<a id="m20-t047-s130"></a>

## 105. MẠCH-AFFECTED ANIMAL

Possible rare case:

animal behavior changes because site Mạch disturbed.

It is still an animal.

Not automatically a monster transformation.

---

<a id="m20-t047-s131"></a>

## 106. MANIFESTATION

If entity manifests through animal imagery:

engine treats it as Entity System.

Not species population.

---

<a id="m20-t047-s132"></a>

## PART XV — BESTIARY KNOWLEDGE

<a id="m20-t047-s133"></a>

### 107. BESTIARY KHÔNG PHẢI AUTOMATIC MONSTER CODEX

Player begins knowing only species appropriate to origin/life.

---

<a id="m20-t047-s134"></a>

## 108. BESTIARY ENTRY LAYERS

Each entry can contain:

<a id="m20-t047-s135"></a>

#### Identification

What creature is it?

<a id="m20-t047-s136"></a>

#### Habitat

Where found?

<a id="m20-t047-s137"></a>

#### Signs

How to detect?

<a id="m20-t047-s138"></a>

#### Behavior

What does it do?

<a id="m20-t047-s139"></a>

#### Danger

When dangerous?

<a id="m20-t047-s140"></a>

#### Hunting

How hunt/avoid?

<a id="m20-t047-s141"></a>

#### Uses

Food/material.

<a id="m20-t047-s142"></a>

#### Cultural Belief

Stories/taboos.

<a id="m20-t047-s143"></a>

#### Personal Observations

Player experience.

---

<a id="m20-t047-s144"></a>

## 109. KNOWLEDGE LEVEL

0 — Unknown.

1 — Heard Of.

2 — Recognizes.

3 — Practical Familiarity.

4 — Expert.

5 — Authority.

---

<a id="m20-t047-s145"></a>

## 110. SOURCE

Knowledge source recorded:

- family;
- hunter;
- observation;
- story;
- autopsy;
- encounter.

---

<a id="m20-t047-s146"></a>

## 111. FALSE BESTIARY KNOWLEDGE

Local belief may be wrong.

Entry can hold conflicting claims.

Example:

> “It always attacks red clothing.”

Source:

local rumor.

Not confirmed.

---

<a id="m20-t047-s147"></a>

## 112. OBSERVATIONAL LEARNING

Encounter can add:

> “It retreats when exposed to large fire.”

But only if player witnessed enough evidence.

---

<a id="m20-t047-s148"></a>

## 113. PROFESSIONAL INTERPRETATION

Hunter sees:

behavior.

Dược Nhân:

venom/disease.

Vu Chúc:

ritual significance.

Người Giữ Ký Ức:

myth versions.

Same creature, multiple knowledge layers.

---

<a id="m20-t047-s149"></a>

## 114. BESTIARY AS INVESTIGATION TOOL

Before hunt:

read known signs.

During hunt:

compare.

After:

update.

Not just lore collection.

---

<a id="m20-t047-s150"></a>

## PART XVI — CREATURE ENCOUNTER SYSTEM

<a id="m20-t047-s151"></a>

### 115. ENCOUNTER ELIGIBILITY

Creature encounter generated from:

```text
Species Presence
× Habitat
× Time
× Season
× Weather
× Human Pressure
× Party Noise
× Story Relevance
```

No universal random encounter rate.

---

<a id="m20-t047-s152"></a>

## 116. SIGHTING ≠ ENCOUNTER

Player may simply see animal at distance.

No interaction required.

This creates ecology without constant gameplay interruption.

---

<a id="m20-t047-s153"></a>

## 117. AMBIENT ANIMAL LIFE

Birds.

fish.

small animals.

Need not be interactable individually.

They provide:

sound.

visual.

ecological clues.

---

<a id="m20-t047-s154"></a>

## 118. MEANINGFUL ENCOUNTER

Interactive only if:

- danger;
- hunting;
- discovery;
- story;
- unusual behavior.

---

<a id="m20-t047-s155"></a>

## 119. APPROACH OPTIONS

Player can:

Observe.

Avoid.

Follow.

Hunt.

Scare.

Feed/Attract in appropriate case.

Report.

---

<a id="m20-t047-s156"></a>

## 120. FAILURE

Can result:

- injury;
- animal escapes;
- supplies lost;
- trail lost;
- territory learned.

Not always combat death.

---

<a id="m20-t047-s157"></a>

## PART XVII — CREATURE COMBAT

<a id="m20-t047-s158"></a>

### 121. COMBAT OBJECTIVE

Animal combat often has goals other than kill:

- escape;
- protect companion;
- drive animal away;
- rescue victim;
- hold position.

---

<a id="m20-t047-s159"></a>

## 122. ANIMAL MORALE

Not same as human morale.

Animal decides via:

Fear.

Pain.

Territory.

Young.

Hunger.

---

<a id="m20-t047-s160"></a>

## 123. RETREAT

Most animals should retreat if:

injured

and escape exists.

---

<a id="m20-t047-s161"></a>

## 124. CORNERED

Cornered animals become far more dangerous.

Player positioning matters.

---

<a id="m20-t047-s162"></a>

## 125. NO ANIMAL HP SPONGE

Large animal tough because:

mass.

anatomy.

momentum.

Not 10x HP arbitrarily.

---

<a id="m20-t047-s163"></a>

## 126. TARGETED WOUND

Leg injury can slow.

Torso hit can cause serious wound.

Animal wound system simplified but compatible with human Injury model.

---

<a id="m20-t047-s164"></a>

## 127. BLOOD TRAIL

Wounded animal leaves trail.

Combat can transition into Tracking Encounter.

---

<a id="m20-t047-s165"></a>

## 128. SCARING AWAY

Fire.

noise.

group formation.

Knowledge.

Can resolve without killing.

---

<a id="m20-t047-s166"></a>

## PART XVIII — HUMAN THREAT SYSTEM

<a id="m20-t047-s167"></a>

### 129. HUMAN THREAT PHILOSOPHY

Human enemies are characters in conflict, not creature archetypes.

They use:

- Psychology;
- Profession;
- Skill;
- Class;
- Faction;
- Morale;
- Equipment.

---

<a id="m20-t047-s168"></a>

## 130. HUMAN THREAT CATEGORIES ARE FUNCTIONAL, NOT IDENTITY

Useful encounter categories:

- Desperate Civilian;
- Bandit Group;
- Smuggler;
- Raider;
- Patrol;
- Militia;
- Professional Warrior;
- Faction Agent;
- Fugitive;
- Rival Hunter;
- Rival Craft Group.

These are situations, not permanent species-like types.

---

<a id="m20-t047-s169"></a>

## 131. DESPERATE CIVILIAN

Cause may be:

- famine;
- debt;
- displacement.

Threat behavior:

poor training.

high fear.

maybe unpredictable.

Player can often negotiate.

---

<a id="m20-t047-s170"></a>

## 132. BANDIT GROUP

Must have economic ecology.

Needs:

- food;
- shelter;
- fence/buyer;
- information;
- recruitment.

Without support:

group collapses.

---

<a id="m20-t047-s171"></a>

## 133. BANDIT MOTIVES

Possible:

- survival;
- profit;
- political displacement;
- former soldiers;
- feud.

Different motives change surrender and negotiation.

---

<a id="m20-t047-s172"></a>

## 134. BANDIT TERRITORY

They operate where:

- route valuable;
- authority weak;
- escape route exists.

Not spawn in random forests without reason.

---

<a id="m20-t047-s173"></a>

## 135. BANDIT EVOLUTION

Successful group can become:

- smuggling network;
- mercenary force;
- local authority;
- faction proxy.

World simulation.

---

<a id="m20-t047-s174"></a>

## 136. SMUGGLER

Avoids combat.

Values:

secrecy.

network.

routes.

Goods.

If attacked:

may flee.

Smuggler threat primarily social/economic.

---

<a id="m20-t047-s175"></a>

## 137. RAIDER

Goal:

quick resource extraction.

Needs:

mobility.

target intelligence.

exit route.

Does not want prolonged occupation.

---

<a id="m20-t047-s176"></a>

## 138. MILITIA

Community defense force.

Strength:

local knowledge.

motivation.

Weakness:

training inconsistency.

They fight hard defending home but may not pursue far.

---

<a id="m20-t047-s177"></a>

## 139. PROFESSIONAL WARRIOR

More:

discipline.

formation.

retreat organization.

But still human.

---

<a id="m20-t047-s178"></a>

## 140. PATROL

Goal:

observe/control route.

Not necessarily hostile.

Player can:

avoid.

talk.

present authority.

fight.

---

<a id="m20-t047-s179"></a>

## 141. FACTION AGENT

May engage in:

- intelligence;
- coercion;
- diplomacy;
- sabotage.

Threat can exist without combat.

---

<a id="m20-t047-s180"></a>

## 142. FUGITIVE

Could be:

criminal.

political target.

innocent accused person.

Quest framing matters.

---

<a id="m20-t047-s181"></a>

## 143. RIVAL HUNTER

Competes over:

territory.

prey.

reputation.

Can become:

ally.

enemy.

mentor.

---

<a id="m20-t047-s182"></a>

## 144. RIVAL CRAFT NETWORK

Conflict over:

- material;
- secret;
- commission.

Can escalate to sabotage, not necessarily battle.

---

<a id="m20-t047-s183"></a>

## PART XIX — HUMAN ENEMY LOGISTICS

<a id="m20-t047-s184"></a>

### 145. ENEMY GROUP NEEDS SUPPLY

A military group requires:

Food.

Weapons.

Rest.

Information.

---

<a id="m20-t047-s185"></a>

## 146. CUTTING SUPPLY

Can defeat threat without direct combat.

Example:

raiders lose boat supply.

Group withdraws.

---

<a id="m20-t047-s186"></a>

## 147. MORALE

Human group morale affected by:

- leader;
- casualties;
- food;
- purpose;
- reputation of opponent.

---

<a id="m20-t047-s187"></a>

## 148. SURRENDER

Possible when:

goal impossible.

leadership lost.

escape blocked.

Player reputation affects likelihood.

---

<a id="m20-t047-s188"></a>

## 149. DESERTION

Long war + poor supply can cause deserters.

Those deserters may:

- return home;
- become bandits;
- join other faction.

---

<a id="m20-t047-s189"></a>

## PART XX — HUMAN ENEMY KNOWLEDGE

<a id="m20-t047-s190"></a>

### 150. ENEMIES DO NOT KNOW PLAYER STATS

They know via:

- sight;
- rumor;
- scout;
- prior encounter.

---

<a id="m20-t047-s191"></a>

## 151. REPEATED ENCOUNTER

Enemy can learn:

player uses ambush.

player favors river routes.

player spares prisoners.

Strategy evolves.

---

<a id="m20-t047-s192"></a>

## 152. WRONG INTELLIGENCE

Enemy may believe false rumor.

Player can exploit.

---

<a id="m20-t047-s193"></a>

## PART XXI — PREDATOR/HUMAN INTERACTION

<a id="m20-t047-s194"></a>

### 153. HUMAN BODY CREATES ECOLOGY

Battlefield corpses attract scavengers.

This can generate later animal threat.

---

<a id="m20-t047-s195"></a>

## 154. ABANDONED SETTLEMENT

Animals move in.

Years later:

location ecology different.

---

<a id="m20-t047-s196"></a>

## 155. HUNTING PRESSURE FROM WAR

If hunters conscripted:

prey may increase.

Predator population later changes.

---

<a id="m20-t047-s197"></a>

## PART XXII — RESOURCE & ECONOMY CONNECTION

<a id="m20-t047-s198"></a>

### 156. ANIMAL RESOURCES

Animal products can include:

- food;
- hide;
- bone;
- horn;
- sinew/other functional materials where appropriate.

Exact materials production researched.

---

<a id="m20-t047-s199"></a>

## 157. NO LOOT ABSTRACTION LIKE “BEAST PART”

Products must have plausible use.

---

<a id="m20-t047-s200"></a>

## 158. RESOURCE VALUE

Value varies by region.

Fish valuable inland differently than river settlement.

---

<a id="m20-t047-s201"></a>

## 159. HUNTING MARKET

Overabundance of hunters can reduce prey and future income.

Profession has sustainability issue.

---

<a id="m20-t047-s202"></a>

## 160. RARE RESOURCE

A rare animal part may have ritual/status demand.

This can motivate poaching.

---

<a id="m20-t047-s203"></a>

## 161. POACHING

Defined relative to:

- taboo;
- communal access;
- authority rights.

Creates social conflict.

---

<a id="m20-t047-s204"></a>

## PART XXIII — SETTLEMENT–ANIMAL CONFLICT

<a id="m20-t047-s205"></a>

### 162. CROP RAID

Animals eat crops.

Possible solutions:

- kill;
- fence;
- scare;
- change planting;
- restore habitat.

---

<a id="m20-t047-s206"></a>

## 163. LIVESTOCK PREDATION

Predator kills domestic animal.

Villagers demand action.

But predator population may be small/important.

---

<a id="m20-t047-s207"></a>

## 164. HUMAN EXPANSION RESPONSIBILITY

Sometimes conflict exists because people entered territory.

Game should show this without moral lecture.

---

<a id="m20-t047-s208"></a>

## 165. COMPENSATION

Political solution:

compensate households rather than exterminate animal.

Interesting settlement policy.

---

<a id="m20-t047-s209"></a>

## PART XXIV — ECOLOGICAL STORY THREADS

<a id="m20-t047-s210"></a>

### 166. ECOLOGICAL STORY TYPES

- predator conflict;
- overhunting;
- fish decline;
- sacred animal;
- disease ecology;
- invasive/expanding species if historically plausible;
- habitat loss;
- seasonal migration disruption.

---

<a id="m20-t047-s211"></a>

## 167. THREAD ORIGIN

Must come from simulation change.

Not arbitrary quest template.

---

<a id="m20-t047-s212"></a>

## 168. LONG-TERM OUTCOME

Player choice affects:

population.

economy.

belief.

settlement.

---

<a id="m20-t047-s213"></a>

## PART XXV — EXAMPLE THREAD: “CON LỢN SỨT TAI”

<a id="m20-t047-s214"></a>

### 169. ORIGIN

Tản Viên.

An old boar repeatedly raids fields.

Locals call it:

**Sứt Tai**.

---

<a id="m20-t047-s215"></a>

## 170. HISTORY

It survived hunters for years.

One hunter lost a hand in an earlier encounter.

Reputation grows.

---

<a id="m20-t047-s216"></a>

## 171. CURRENT PROBLEM

Recent forest clearing reduced natural feeding area.

Crop raid frequency rises.

---

<a id="m20-t047-s217"></a>

## 172. VILLAGER VIEW

Farmers:

> kill it.

Older hunters:

> respect it.

A ritual specialist:

> considers it linked to local story.

---

<a id="m20-t047-s218"></a>

## 173. PLAYER INVESTIGATION

Can discover:

- new logging;
- trail;
- den;
- young animals;
- hunting history.

---

<a id="m20-t047-s219"></a>

## 174. SOLUTION A — KILL

Crop raids reduce immediately.

Hunter reputation.

But if population context matters:

ecological effects possible.

---

<a id="m20-t047-s220"></a>

## 175. SOLUTION B — DRIVE AWAY

Need coordinated effort.

May shift problem to another settlement.

---

<a id="m20-t047-s221"></a>

## 176. SOLUTION C — CHANGE FIELDS

Build barriers/change storage.

More labor/resource.

---

<a id="m20-t047-s222"></a>

## 177. SOLUTION D — REDUCE FOREST PRESSURE

Long-term.

Economic conflict with woodcutters.

---

<a id="m20-t047-s223"></a>

## 178. MYTH OUTCOME

If player kills Sứt Tai:

rumor may make animal larger in retelling.

If player spares:

could become sacred story.

Same natural animal enters mythology.

---

<a id="m20-t047-s224"></a>

## PART XXVI — EXAMPLE THREAD: “RỪNG IM TIẾNG”

<a id="m20-t047-s225"></a>

### 179. SETUP

Lâm U hunters notice fewer animal calls.

---

<a id="m20-t047-s226"></a>

## 180. HYPOTHESES

- predator;
- disease;
- hunters;
- Mạch disturbance;
- fire.

---

<a id="m20-t047-s227"></a>

## 181. INVESTIGATION

Population data indirectly visible through:

- tracks;
- carcasses;
- behavior;
- local reports.

---

<a id="m20-t047-s228"></a>

## 182. POSSIBLE TRUTH

A major predator moved into area because prey elsewhere collapsed.

Mạch site also recently disturbed, but causal relation unclear.

---

<a id="m20-t047-s229"></a>

## 183. RESOLUTION

Can:

track predator.

restore prey habitat.

move settlement activity.

ritual action.

No single “kill monster” solution required.

---

<a id="m20-t047-s230"></a>

## PART XXVII — EXAMPLE HUMAN THREAT: BANDIT CAMP

<a id="m20-t047-s231"></a>

### 184. INITIAL ASSUMPTION

Rumor:

> bandits attack travelers.

---

<a id="m20-t047-s232"></a>

## 185. ACTUAL NETWORK

Group consists of:

former displaced farmers.

two deserters.

one trader contact.

---

<a id="m20-t047-s233"></a>

## 186. ECONOMIC BASE

They steal:

food.

metal.

Then trade through intermediary.

---

<a id="m20-t047-s234"></a>

## 187. SOLUTION OPTIONS

Kill camp.

Arrest leader.

Cut fence network.

Provide resettlement.

Recruit deserters.

Negotiate route toll.

Support settlement authority.

---

<a id="m20-t047-s235"></a>

## 188. CONSEQUENCE OF KILLING CAMP

Immediate security improves.

But:

families may lose relatives.

survivors scatter.

If economic cause remains:

new group may emerge later.

Not respawn; causal recurrence.

---

<a id="m20-t047-s236"></a>

## PART XXVIII — MYTHIC BEAST DESIGN

<a id="m20-t047-s237"></a>

### 189. MYTHIC BEAST IS NOT JUST LARGE ANIMAL

A true mythic creature needs:

- mythic context;
- sacred geography;
- unusual behavior;
- Mạch connection;
- social significance.

---

<a id="m20-t047-s238"></a>

## 190. OCCURRENCE

Extremely rare.

A campaign may contain only a handful.

---

<a id="m20-t047-s239"></a>

## 191. NO FARMING

No respawn.

No loot cycle.

---

<a id="m20-t047-s240"></a>

## 192. ENCOUNTER STRUCTURE

Discovery

→ Investigation

→ Interpretation

→ Preparation

→ Contact

→ Resolution.

Combat, if any, is only one phase.

---

<a id="m20-t047-s241"></a>

## 193. MYTHIC BEAST OUTCOME

Could:

disappear.

be driven away.

be killed.

become pact/story.

change Mạch.

All have world consequence.

---

<a id="m20-t047-s242"></a>

## PART XXIX — CREATURE KNOWLEDGE & RUMOR

<a id="m20-t047-s243"></a>

### 194. FALSE MONSTER RUMOR

A “monster” story may be:

- predator;
- murderer;
- natural event;
- entity.

Quest should not reveal category immediately.

---

<a id="m20-t047-s244"></a>

## 195. RUMOR AMPLIFICATION

Each retelling increases unusual detail.

Bestiary stores versions.

---

<a id="m20-t047-s245"></a>

## 196. PLAYER CAN CORRECT RUMOR

Evidence can reduce fear.

But community may prefer mythic interpretation.

---

<a id="m20-t047-s246"></a>

## PART XXX — COMPANION INTERACTION

<a id="m20-t047-s247"></a>

### 197. COMPANION FEAR

Different companions react differently to animals.

Hunter:

calm.

Traumatized character:

panic.

Vu Chúc:

different interpretation.

---

<a id="m20-t047-s248"></a>

## 198. MORAL DISAGREEMENT

One companion may oppose killing sacred/young animal.

Another prioritizes food.

Relationship consequence.

---

<a id="m20-t047-s249"></a>

## 199. COMPANION EXPERTISE

Can:

identify tracks.

warn player.

advise retreat.

Player can ignore advice.

---

<a id="m20-t047-s250"></a>

## 200. HUNTING MEMORY

A shared hunt becomes Party Memory.

Especially if:

injury.

rare animal.

failure.

---

<a id="m20-t047-s251"></a>

## PART XXXI — CREATURE & CLASS INTERACTION

<a id="m20-t047-s252"></a>

### 201. LIỆP NHÂN

Strongest general interaction:

- signs;
- behavior;
- terrain;
- hunting.

---

<a id="m20-t047-s253"></a>

## 202. DƯỢC NHÂN

Understands:

venom.

disease.

animal injury.

Food safety.

---

<a id="m20-t047-s254"></a>

## 203. VU CHÚC

Understands:

ritual meaning.

Mạch relation.

Not animal biology automatically.

---

<a id="m20-t047-s255"></a>

## 204. NGƯỜI GIỮ KÝ ỨC

Knows:

myths.

past sightings.

older ecological memories.

---

<a id="m20-t047-s256"></a>

## 205. TRINH BINH

Detects:

danger.

movement.

route.

---

<a id="m20-t047-s257"></a>

## 206. THỦ LĨNH

Can organize:

drive hunts.

village defense.

Large human-animal conflict.

---

<a id="m20-t047-s258"></a>

## 207. XẢO NHÂN

Builds:

traps.

barriers.

fences.

tools.

---

<a id="m20-t047-s259"></a>

## 208. SỨ GIẢ

Useful in:

human conflict around animal issue.

Not direct creature persuasion magic.

---

<a id="m20-t047-s260"></a>

## PART XXXII — CREATURE & PROFESSION

<a id="m20-t047-s261"></a>

### 209. HUNTER PROFESSION

Has economic relationship to population.

Master hunter must understand sustainability.

---

<a id="m20-t047-s262"></a>

## 210. FISHER

Knows aquatic cycles.

Fish decline becomes career problem.

---

<a id="m20-t047-s263"></a>

## 211. FARMER

Observes:

crop pests.

weather.

animal raid.

---

<a id="m20-t047-s264"></a>

## 212. HERBALIST

Animal behavior can reveal plant/ecology.

---

<a id="m20-t047-s265"></a>

## 213. BOATMAN

Knows aquatic threats and seasonal migration.

---

<a id="m20-t047-s266"></a>

## PART XXXIII — CREATURE & DUNGEON

<a id="m20-t047-s267"></a>

### 214. CREATURE USE IN DUNGEONS

Animals should inhabit dungeon logically.

Cave:

den.

Abandoned village:

scavengers.

Battlefield:

birds/scavengers.

Not random monster room.

---

<a id="m20-t047-s268"></a>

## 215. CREATURE CHANGES DUNGEON STATE

A predator may move in after humans leave.

Later revisit different.

---

<a id="m20-t047-s269"></a>

## 216. ANIMAL CAN CREATE ROUTE

Game trails can reveal hidden path.

Hunter knowledge turns ecology into exploration tool.

---

<a id="m20-t047-s270"></a>

## PART XXXIV — CREATURE & TRAVEL

<a id="m20-t047-s271"></a>

### 217. ROUTE RISK

Creature risk depends on:

season.

time.

food.

Human movement.

---

<a id="m20-t047-s272"></a>

## 218. SAFE ROUTE CAN BECOME RISKY

If flood displaces animals.

World map updates via rumor.

---

<a id="m20-t047-s273"></a>

## 219. TRAVEL SIGHTINGS

Most sightings are noninteractive summaries.

Interactive only when meaningful.

---

<a id="m20-t047-s274"></a>

## PART XXXV — CREATURE & SETTLEMENT

<a id="m20-t047-s275"></a>

### 220. SETTLEMENT ECOLOGICAL FOOTPRINT

Tracks:

- hunting pressure;
- forest clearing;
- fishing pressure;
- waste;
- water modification.

---

<a id="m20-t047-s276"></a>

## 221. ECOLOGICAL KNOWLEDGE OF SETTLEMENT

A settlement may have traditional rules that act as ecological management.

Example:

seasonal hunting taboo.

---

<a id="m20-t047-s277"></a>

## 222. BREAKING LOCAL PRACTICE

Can create short-term economic benefit.

Long-term population loss.

---

<a id="m20-t047-s278"></a>

## PART XXXVI — CREATURE & MẠCH LẠC

<a id="m20-t047-s279"></a>

### 223. ANIMALS AS MẠCH INDICATORS

Certain unusual behavior may signal:

Mạch change.

But not always.

---

<a id="m20-t047-s280"></a>

## 224. MẠCH DOES NOT CONTROL ALL ANIMALS

Avoid hive-mind nature magic.

---

<a id="m20-t047-s281"></a>

## 225. ANIMAL MEMORY OF PLACE

A species may repeatedly use route/feeding ground.

Humans interpret this spiritually.

Physical + belief layer.

---

<a id="m20-t047-s282"></a>

## PART XXXVII — BESTIARY UI

<a id="m20-t047-s283"></a>

### 226. UI STRUCTURE

Bestiary shows:

Name.

Known Description.

Habitat.

Signs.

Behavior.

Risk.

Uses.

Stories.

Personal Notes.

---

<a id="m20-t047-s284"></a>

## 227. UNKNOWN INFO

Displayed as:

Unknown.

Not hidden ??? overload.

---

<a id="m20-t047-s285"></a>

## 228. CONFIDENCE

Claims can be:

Confirmed.

Likely.

Heard.

Disputed.

---

<a id="m20-t047-s286"></a>

## 229. PLAYER NOTES

Can add:

“Seen near Đầm Vọng during high water.”

---

<a id="m20-t047-s287"></a>

## 230. MAP LINK

Bestiary can show:

known sighting regions.

Not exact live positions.

---

<a id="m20-t047-s288"></a>

## PART XXXVIII — CREATURE GENERATION

<a id="m20-t047-s289"></a>

### 231. PROCEDURAL INDIVIDUAL

Minor animal instance can generate:

- age;
- condition;
- health;
- behavior.

---

<a id="m20-t047-s290"></a>

## 232. UNIQUE PROMOTION

If an animal survives meaningful encounter:

system may promote to persistent individual.

---

<a id="m20-t047-s291"></a>

## 233. PROMOTION CONDITIONS

Examples:

- injured player;
- escaped hunt;
- killed named NPC;
- associated with Story Thread.

---

<a id="m20-t047-s292"></a>

## 234. NO RANDOM UNIQUE NAME FOR EVERY ANIMAL

Name only if social relevance develops.

---

<a id="m20-t047-s293"></a>

## PART XXXIX — HUMAN THREAT GENERATION

<a id="m20-t047-s294"></a>

### 235. HUMAN GROUP GENERATION

Must originate from:

Settlement/Population/Faction state.

Never spawn generic Bandit Squad from nothing.

---

<a id="m20-t047-s295"></a>

## 236. GROUP MEMBERSHIP

Members come from real NPC pool or generated households.

---

<a id="m20-t047-s296"></a>

## 237. GROUP GOAL

Must define:

Why together?

What need?

What target?

---

<a id="m20-t047-s297"></a>

## 238. GROUP RESOURCES

Need:

food.

equipment.

safe base.

contact.

---

<a id="m20-t047-s298"></a>

## 239. GROUP BREAKUP

If goal resolved/resources gone:

group may dissolve.

---

<a id="m20-t047-s299"></a>

## PART XL — ENCOUNTER DESIGN QUALITY

<a id="m20-t047-s300"></a>

### 240. NATURAL ANIMAL ENCOUNTER MUST ANSWER

1. Why species is here?
2. What is animal doing?
3. Why does it care about party?
4. Can encounter be avoided?
5. What causes aggression?
6. When will animal retreat?
7. What ecological consequence follows?
8. What knowledge can player gain?

---

<a id="m20-t047-s301"></a>

## 241. EXCEPTIONAL ANIMAL MUST ANSWER

Additionally:

9. Why is this individual notable?
10. What history does it have?
11. Why hasn't it already been killed/left?
12. How have humans mythologized it?

---

<a id="m20-t047-s302"></a>

## 242. MYTHIC CREATURE MUST ANSWER

Additionally:

13. What myth does it connect to?
14. What Mạch context exists?
15. Why is combat meaningful or not?
16. What remains ambiguous?

---

<a id="m20-t047-s303"></a>

## 243. HUMAN THREAT MUST ANSWER

1. Who are they?
2. Where do they come from?
3. What do they want?
4. How do they eat/live?
5. Who supports them?
6. What would make them stop?
7. Can they negotiate?
8. Where would survivors go?
9. What happens if leader dies?
10. What social consequence follows violence?

---

<a id="m20-t047-s304"></a>

## PART XLI — BALANCE PHILOSOPHY

<a id="m20-t047-s305"></a>

### 244. NO BESTIARY LEVEL BANDS

Difficulty should arise naturally.

---

<a id="m20-t047-s306"></a>

## 245. NATURAL COUNTERPLAY

A large animal can be very dangerous in open ground.

But:

avoidance.

terrain.

fire.

group.

can reduce threat.

---

<a id="m20-t047-s307"></a>

## 246. INFORMATION IS POWER

Knowing behavior is equivalent to combat advantage.

---

<a id="m20-t047-s308"></a>

## 247. PREPARATION IS POWER

Right tool/route/time matters.

---

<a id="m20-t047-s309"></a>

## 248. KILLING IS NOT DEFAULT REWARD

Sometimes best outcome:

animal leaves.

settlement adapts.

---

<a id="m20-t047-s310"></a>

## PART XLII — DIFFICULTY MODES

<a id="m20-t047-s311"></a>

### 249. STORY MODE

Creature aggression telegraphed more clearly.

Tracking easier.

Venom/injury more forgiving.

---

<a id="m20-t047-s312"></a>

## 250. STANDARD

Full ecology behavior.

---

<a id="m20-t047-s313"></a>

## 251. EXPLORER / TACTICAL

Less explicit warning.

Trail degradation stronger.

Animal behavior less predictable without knowledge.

---

<a id="m20-t047-s314"></a>

## 252. LEGACY

Population and ecological consequences fully persistent across generations.

---

<a id="m20-t047-s315"></a>

## PART XLIII — LLM ARCHITECTURE

<a id="m20-t047-s316"></a>

### 253. RULE ENGINE OWNS

- species;
- population;
- behavior;
- territory;
- injury;
- ecology;
- human group state.

---

<a id="m20-t047-s317"></a>

## 254. LLM MAY GENERATE

- sensory description;
- hunter stories;
- rumor;
- bestiary prose.

---

<a id="m20-t047-s318"></a>

## 255. LLM MAY NOT INVENT

- new species;
- new supernatural creature;
- impossible behavior;
- new population state.

Without Content Validation.

---

<a id="m20-t047-s319"></a>

## PART XLIV — HISTORICAL & ECOLOGICAL SAFEGUARD

<a id="m20-t047-s320"></a>

### 256. SPECIES RESEARCH TAG

Every species/content entry should have:

<a id="m20-t047-s321"></a>

#### Historically/Ecologically Supported

<a id="m20-t047-s322"></a>

#### Plausible for Region/Period

<a id="m20-t047-s323"></a>

#### Tradition-Inspired

<a id="m20-t047-s324"></a>

#### Game-Canon

---

<a id="m20-t047-s325"></a>

## 257. NO MODERN RANGE ASSUMPTION

A species' modern distribution should not automatically be projected backward.

Production needs paleo/environmental research where necessary.

---

<a id="m20-t047-s326"></a>

## 258. NO FANTASY SPECIES FOR FILLER

Do not invent:

giant wolves;

dire boars;

random elemental animals

just to increase monster variety.

---

<a id="m20-t047-s327"></a>

## PART XLV — CREATURE CONTENT TARGETS

<a id="m20-t047-s328"></a>

### 259. BESTIARY SCALE

GDD does not need hundreds of species.

Production can target:

- broad natural fauna;
- 15–25 mechanically relevant natural threat families;
- several dozen culturally/economically relevant animals;
- small number of persistent exceptional individuals;
- very small number of true mythic manifestations.

Exact roster later.

---

<a id="m20-t047-s329"></a>

## 260. HUMAN THREAT DIVERSITY

Diversity comes from:

- motivation;
- faction;
- equipment;
- training;
- geography.

Not 20 reskins of bandit.

---

<a id="m20-t047-s330"></a>

## PART XLVI — LOCKED DESIGN DECISIONS

<a id="m20-t047-s331"></a>

### 261. CÁC ĐIỂM KHÓA

1. Creature không có Level.
2. Creature không scale theo player.
3. Encounter phải bắt nguồn từ ecology.
4. Population được simulated theo region.
5. Hunting tác động population.
6. Local extinction có thể xảy ra.
7. Ecology có khả năng hồi phục theo thời gian.
8. Animal AI theo motive, không auto attack.
9. Injury animal persists.
10. Old/experienced animals có thể học tránh danger.
11. Hunting là tracking-preparation loop.
12. Trap cần time/location/knowledge.
13. Animal products phải có plausible use.
14. Domestic animals là assets sống, không inventory item.
15. Region có ecological identity riêng.
16. Exceptional animals là persistent individuals.
17. Mythologization có thể xảy ra với animal tự nhiên.
18. Strange animal không đồng nghĩa supernatural.
19. Bestiary lưu cả facts và rumors.
20. Bestiary knowledge character-dependent.
21. Human threats không thuộc bestiary logic.
22. Human enemy dùng Character System.
23. Bandit groups cần economic support.
24. Human groups có thể dissolve/evolve.
25. Enemy logistics có gameplay value.
26. Creature encounters không mặc định combat.
27. Creature combat có retreat/fear.
28. Mythic creatures cực hiếm.
29. No mythic loot farming.
30. Ecology liên kết Economy, Travel, Settlement, Mạch và Quest.
31. LLM không quyết ecology.
32. Species roster phải có historical/ecological research pass.

---

<a id="m20-t047-s332"></a>

## PART XLVII — DEFERRED TO PRODUCTION

<a id="m20-t047-s333"></a>

### 262. NỘI DUNG CẦN LÀM SAU

Production phase sẽ xây:

- full fauna roster;
- regional habitat tables;
- species distribution;
- diet/ecology tables;
- breeding/season data;
- hunting techniques;
- animal materials;
- venom catalog;
- domesticated animal roster;
- human threat faction rosters;
- animal animation;
- sound design;
- bestiary art;
- ecological balancing;
- detailed encounter templates.

Không cần khóa hết ở GDD core.

---

<a id="m20-t047-s334"></a>

## PART XLVIII — CORE CREATURE FANTASY

<a id="m20-t047-s335"></a>

### 263. PLAYER EXPERIENCE

Game không nên khiến player nghĩ:

> “Con này level bao nhiêu?”

Mà:

> “Nó đang làm gì ở đây?”

> “Có con non gần đây không?”

> “Tại sao nó không bỏ chạy?”

> “Dấu chân này mới hay cũ?”

> “Nếu tôi giết nó, chuyện gì xảy ra với vùng này?”

---

<a id="m20-t047-s336"></a>

## 264. CORE HUNTING FANTASY

Một cuộc săn hay không bắt đầu bằng combat.

Nó bắt đầu khi player thấy:

một cành cây gãy.

một dấu chân.

một chỗ đất mới bị đào.

Từ đó:

Knowledge

→ Interpretation

→ Tracking

→ Position

→ Decision.

Đòn đánh cuối cùng chỉ là một phần rất nhỏ.

---

<a id="m20-t047-s337"></a>

## 265. CORE HUMAN THREAT FANTASY

Khi player gặp một nhóm cướp:

game không hỏi:

> “Bạn có đủ mạnh để giết họ không?”

Nó phải khiến player hỏi:

> “Họ là ai?”

> “Tại sao họ ở đây?”

> “Ai mua đồ họ cướp?”

> “Nếu giết nhóm này, route có thật sự an toàn hơn không?”

> “Nếu tha, họ sẽ đi đâu?”

---

<a id="m20-t047-s338"></a>

## 266. CORE ECOLOGY FANTASY

Một vùng đất không chỉ chứa settlement và dungeon.

Nó còn có:

animals.

fish.

forest.

wetland.

predators.

prey.

seasons.

Khi con người xây:

- đê;
- mỏ;
- thành;
- route,

hệ sinh thái cũng thay đổi.

Do đó lịch sử không chỉ là:

> người thay vua.

Nó còn là:

> **cách con người thay đổi vùng đất và cách vùng đất phản ứng trở lại.**

---

<a id="m20-t047-s339"></a>

## PART XLIX — MODULE POSITION IN THE GAME

<a id="m20-t047-s340"></a>

### 267. SYSTEM LOOP

Toàn bộ hệ có thể chạy như sau:

**Climate / Season**

→ thay Habitat & Food.

**Ecology**

→ thay Population.

**Population**

→ thay Hunting / Threat / Resource.

**Human Activity**

→ thay Habitat.

**Settlement Expansion**

→ tạo Conflict.

**Conflict**

→ tạo Story Thread.

**Quest**

→ expose vấn đề.

**Player Action**

→ Hunt / Protect / Relocate / Change Infrastructure / Negotiate.

**Population + Settlement State**

→ cập nhật.

**Rumor**

→ có thể biến một animal thành legend.

**Mạch / Myth**

→ đôi khi tạo một lớp interpretation mới.

Và vòng lặp tiếp tục.

---

<a id="m20-t047-s341"></a>

## 268. FINAL DESIGN PRINCIPLE

Nếu một creature encounter có thể được mô tả đơn giản như:

> “3 con thú xuất hiện vì player đi vào encounter zone”

thì encounter chưa đạt.

Team phải hỏi:

> Chúng sống ở đâu?

> Chúng đang kiếm gì?

> Tại sao xuất hiện lúc này?

> Chúng có thực sự muốn chiến đấu không?

> Nếu player không tới đây thì chúng sẽ làm gì?

> Nếu player giết chúng, population có thay đổi không?

> Người địa phương nhìn loài này thế nào?

Khi trả lời được những câu đó:

**creature mới thực sự thuộc về thế giới.**

Bước tiếp theo nên là **Time + Calendar + Aging + Death + Succession + Legacy**, vì đây là mảnh nối tất cả hệ thống dài hạn lại với nhau: NPC già đi, trẻ lớn lên, nghề nghiệp chuyển giao, settlement thay đổi, companion nghỉ hưu, nhân vật chết, successor tiếp tục campaign và chính những gì player làm dần trở thành lịch sử hoặc thần thoại.

<!-- SOURCE T047 END -->

