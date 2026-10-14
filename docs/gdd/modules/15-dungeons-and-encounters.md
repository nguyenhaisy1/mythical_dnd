# Module 15 — Dungeon và Encounter

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T037](../../source/conversation.md#t037)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Dungeon là chuỗi tình huống có cấu trúc, không mặc định combat corridor. Encounter phải có nguồn, actor, mục tiêu và nhiều giải pháp khi thế giới cho phép.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Site state, local ecology, story context, resources, knowledge và objectives. |
| Đầu ra / bên sở hữu | Encounter state và commands; không tạo physical truth ngoài domain owner. |
| Đọc cùng | 12, 14, 17, 20.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Cùng site hỗ trợ né tránh, đối thoại hoặc điều tra nếu đủ điều kiện; hậu quả tồn tại khi rời site. |
| Việc chưa được chứng minh | Scene/encounter production và puzzle accessibility. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T037 BEGIN -->

Từ đây tôi sẽ giữ nguyên tắc: **mọi nhóm thành phần đều được mô tả ở cùng một độ sâu**, không có chuyện 2 mục đầu rất kỹ rồi các mục sau chỉ còn vài dòng. Với module này, tôi cũng sẽ khóa rõ ranh giới giữa “Dungeon”, “Encounter”, “Puzzle”, “Exploration Site” và “Story Thread” để sau này team phát triển không trộn lẫn khái niệm.

<a id="m15-t037-s001"></a>

## GDD MODULE 15 — DUNGEON & ENCOUNTER SYSTEM

<a id="m15-t037-s002"></a>

## 1. DESIGN PHILOSOPHY

Trong game này, **Dungeon không đồng nghĩa với hang động có quái**.

Dungeon được định nghĩa là:

> **Một không gian hoặc chuỗi tình huống có cấu trúc, nơi người chơi phải vượt qua nhiều lớp trở ngại liên kết với nhau để đạt một mục tiêu dưới áp lực về thời gian, thông tin, tài nguyên, nguy hiểm hoặc hậu quả.**

Theo định nghĩa này, dungeon có thể là:

- một hang núi;
- một mộ thuyền lộ ra sau lũ;
- một khu rừng cấm;
- một vùng đầm lầy;
- một làng bị bao vây;
- một chiến trường;
- một đoàn thuyền trên sông;
- một lễ hội chính trị;
- một khu công trường Cổ Loa;
- một chuỗi giấc mơ liên kết bởi Mạch Lạc;
- thậm chí một cuộc đàm phán kéo dài qua nhiều địa điểm.

Dungeon vì vậy là **cấu trúc gameplay**, không phải biome.

Encounter được định nghĩa là:

> **Một đơn vị tình huống trong đó người chơi phải đưa ra quyết định hoặc thực hiện hành động có khả năng thay đổi trạng thái của dungeon, party, story thread hoặc world state.**

Dungeon chứa nhiều encounter.

Nhưng encounter cũng có thể xảy ra ngoài dungeon.

---

<a id="m15-t037-s003"></a>

## 2. MỤC TIÊU CỦA DUNGEON SYSTEM

Dungeon System phải đạt được bảy mục tiêu chính.

| Mục tiêu | Ý nghĩa |
|---|---|
| Tension | Người chơi cảm thấy đang tiến vào một không gian có áp lực |
| Discovery | Mỗi bước tiến cung cấp thêm hiểu biết |
| Choice | Có nhiều đường và nhiều cách giải quyết |
| Cost | Mỗi lựa chọn có thể tiêu tốn thời gian, tài nguyên, sức khỏe hoặc quan hệ |
| Persistence | Dungeon nhớ những gì player đã làm |
| Re-entry | Có thể quay lại sau này và thấy thay đổi |
| Integration | Dungeon phải kết nối với world, NPC, economy, history và story thread |

Dungeon không được tồn tại như một “minigame map” tách biệt với phần còn lại của game.

---

<a id="m15-t037-s004"></a>

## 3. CẤU TRÚC CỐT LÕI CỦA MỘT DUNGEON

Mỗi Dungeon Instance có các thành phần sau:

```text
Dungeon ID
Location
Type
Origin
Current State
Main Objective
Secondary Objectives
Known Information
Unknown Information
Entry Points
Exit Points
Zones/Nodes
Connections
Hazards
Actors
Factions
Resources
Clocks
Alert/Tension State
Persistent Changes
Story Threads
Resolution States
Aftermath
```

Mọi dungeon quan trọng phải có **lý do tồn tại** trong world.

Ví dụ:

Một hang không tồn tại vì “game cần dungeon”.

Nó có thể là:

- mỏ cũ;
- hang săn;
- nơi trú khi lũ;
- nơi từng làm nghi lễ;
- hầm khai thác bị bỏ;
- địa điểm có history.

---

<a id="m15-t037-s005"></a>

## 4. DUNGEON GRAPH

Dungeon không bắt buộc phải là bản đồ phòng nối phòng.

Về logic, nó là một graph:

```text
Entry
↓
Node A
↙      ↘
B        C
↓        ↓
D ←→ E
↓
Objective
```

Node có thể là:

- địa điểm;
- encounter;
- checkpoint;
- social situation;
- hazard;
- discovery;
- transition.

Edge có thể yêu cầu:

- route knowledge;
- tool;
- skill;
- time;
- relationship;
- key object;
- social permission;
- ritual condition.

---

<a id="m15-t037-s006"></a>

## 5. NODE KHÔNG NHẤT THIẾT LÀ “PHÒNG”

Ví dụ dungeon dạng lễ hội:

Node A:

khu khách.

Node B:

nhà trưởng dòng họ.

Node C:

nơi nghi lễ.

Node D:

bến thuyền.

Node E:

cuộc họp kín.

Đây vẫn là dungeon graph.

---

<a id="m15-t037-s007"></a>

## 6. DUNGEON STATE

Mỗi dungeon có trạng thái tổng thể.

Ví dụ:

<a id="m15-t037-s008"></a>

#### Stable

Không gian chưa bị kích động.

<a id="m15-t037-s009"></a>

#### Disturbed

Player hoặc event đã làm thay đổi trạng thái.

<a id="m15-t037-s010"></a>

#### Alert

Actors biết có người xâm nhập hoặc có chuyện bất thường.

<a id="m15-t037-s011"></a>

#### Escalating

Hazard tăng, lực lượng phản ứng.

<a id="m15-t037-s012"></a>

#### Collapsing

Dungeon vật lý/xã hội đang sụp đổ.

<a id="m15-t037-s013"></a>

#### Resolved

Main tension đã kết thúc.

<a id="m15-t037-s014"></a>

#### Abandoned

Không còn actor chính.

<a id="m15-t037-s015"></a>

#### Transformed

Dungeon trở thành một loại location khác.

---

<a id="m15-t037-s016"></a>

## 7. DUNGEON CLOCKS

Một trong những mechanic cốt lõi.

Clock không phải lúc nào cũng là đồng hồ countdown cụ thể.

Các loại clock:

<a id="m15-t037-s017"></a>

### Time Clock

Ví dụ:

nước đang dâng.

---

<a id="m15-t037-s018"></a>

### Alert Clock

Guard nhận ra intruder.

---

<a id="m15-t037-s019"></a>

### Structural Clock

Hang đang sập.

---

<a id="m15-t037-s020"></a>

### Disease Clock

Người bệnh đang xấu đi.

---

<a id="m15-t037-s021"></a>

### Political Clock

Cuộc họp sẽ bỏ phiếu.

---

<a id="m15-t037-s022"></a>

### Ritual Clock

Nghi lễ đang tiến đến thời điểm không thể đảo ngược.

---

<a id="m15-t037-s023"></a>

### Pursuit Clock

Kẻ đuổi theo đang tới gần.

Clock giúp dungeon có áp lực mà không cần spam combat.

---

<a id="m15-t037-s024"></a>

## 8. CLOCK PHẢI CÓ TELEGRAPH

Không được giấu hoàn toàn.

Player phải có dấu hiệu:

- tiếng nước;
- guard activity;
- warning;
- structural noise;
- NPC advice;
- weather;
- ritual drum.

Skill có thể giúp ước lượng chính xác hơn.

---

<a id="m15-t037-s025"></a>

## 9. DUNGEON PRESSURE

Ngoài Clock, dungeon có các loại pressure:

- Time;
- Supply;
- Health;
- Information;
- Morale;
- Stealth;
- Social Legitimacy;
- Environmental Stability;
- Spiritual Instability.

Không dungeon nào cần dùng tất cả.

Mỗi dungeon nên có 2–4 pressure chính để giữ identity.

---

<a id="m15-t037-s026"></a>

## 10. DUNGEON ENTRY

Không phải mọi dungeon chỉ có một cửa vào.

Entry có thể:

- main entrance;
- secret route;
- river access;
- roof;
- social invitation;
- prisoner route;
- service passage;
- ritual entry;
- memory/dream transition.

Một số entry cần chuẩn bị từ trước.

---

<a id="m15-t037-s027"></a>

## 11. ENTRY CHOICE MATTERS

Ví dụ vào làng đang bị chiếm:

<a id="m15-t037-s028"></a>

#### Main Gate

Nhanh.

Combat risk cao.

<a id="m15-t037-s029"></a>

#### River

Cần Thủy Binh.

Stealth tốt.

<a id="m15-t037-s030"></a>

#### Hidden field path

Cần local guide.

<a id="m15-t037-s031"></a>

#### Negotiated Entry

Cần social leverage.

<a id="m15-t037-s032"></a>

#### Disguise

Cần information + equipment.

Different entry tạo dungeon state khác ngay từ đầu.

---

<a id="m15-t037-s033"></a>

## 12. DUNGEON EXIT

Exit không luôn giống Entry.

Có thể:

- retreat;
- emergency exit;
- collapse route;
- escort route;
- boat extraction;
- surrender;
- negotiated release.

Một dungeon tốt luôn nghĩ tới:

> nếu player thất bại giữa chừng, họ thoát bằng cách nào?

---

<a id="m15-t037-s034"></a>

## 13. RETREAT KHÔNG PHẢI FAIL SCREEN

Player có thể rời dungeon.

Dungeon tiếp tục simulation.

Ví dụ:

rút khỏi hang.

Ngày sau quay lại.

Có thể:

water tăng.

enemy di chuyển.

corpse biến mất.

rumor lan.

---

<a id="m15-t037-s035"></a>

## 14. PARTIAL COMPLETION

Dungeon có thể giải một phần.

Ví dụ:

cứu prisoner.

không bắt được leader.

Main Objective partially complete.

Story tiếp tục.

---

<a id="m15-t037-s036"></a>

## 15. MULTIPLE RESOLUTION STATES

Không chỉ:

Completed.

Failed.

Có thể:

<a id="m15-t037-s037"></a>

#### Clean Resolution

Mục tiêu đạt, cost thấp.

<a id="m15-t037-s038"></a>

#### Costly Resolution

Mục tiêu đạt nhưng hậu quả lớn.

<a id="m15-t037-s039"></a>

#### Partial Resolution

Một số mục tiêu đạt.

<a id="m15-t037-s040"></a>

#### Containment

Vấn đề chưa giải quyết nhưng tạm kiểm soát.

<a id="m15-t037-s041"></a>

#### Escalation

Player khiến tình hình xấu hơn.

<a id="m15-t037-s042"></a>

#### Abandonment

Player rời.

<a id="m15-t037-s043"></a>

#### Transformation

Dungeon biến thành situation mới.

---

<a id="m15-t037-s044"></a>

## PART II — DUNGEON TYPES

<a id="m15-t037-s045"></a>

## 16. NATURAL CAVE / MOUNTAIN DUNGEON

<a id="m15-t037-s046"></a>

### Identity

Không gian tự nhiên, hạn chế tầm nhìn, navigation khó, cấu trúc vertical.

Core pressures:

- orientation;
- light;
- structural danger;
- fatigue;
- limited exits.

Gameplay mạnh về:

- Xảo;
- THỂ;
- GIÁC;
- Navigation;
- Climbing;
- local knowledge.

<a id="m15-t037-s047"></a>

### Signature mechanics

<a id="m15-t037-s048"></a>

#### Vertical Route

Điểm cao/thấp.

<a id="m15-t037-s049"></a>

#### Narrow Passage

Equipment bulk matters.

<a id="m15-t037-s050"></a>

#### Sound Propagation

Enemy/animal nghe movement.

<a id="m15-t037-s051"></a>

#### Structural Stability

Không phải mọi vách đều an toàn.

<a id="m15-t037-s052"></a>

#### Air/Smoke

Fire usage có risk.

<a id="m15-t037-s053"></a>

### Encounter types phù hợp

- environmental;
- animal;
- exploration;
- collapse;
- trapped person;
- mining clue;
- Mạch disturbance.

<a id="m15-t037-s054"></a>

### Fail states

- bị kẹt;
- mất route;
- injury;
- route collapse;
- forced retreat.

<a id="m15-t037-s055"></a>

### Narrative uses

- mỏ cũ;
- burial;
- sacred cave;
- refuge;
- hidden workshop.

---

<a id="m15-t037-s056"></a>

## 17. TOMB / BURIAL DUNGEON

<a id="m15-t037-s057"></a>

### Identity

Không gian vừa vật lý vừa xã hội.

Điểm quan trọng không phải “undead”.

Mà là:

- người chết;
- quyền sở hữu;
- taboo;
- lineage;
- history;
- ritual.

Core pressures:

- preservation;
- moral boundary;
- limited access;
- unknown history;
- social consequence.

<a id="m15-t037-s058"></a>

### Signature mechanics

<a id="m15-t037-s059"></a>

#### Grave Integrity

Player phá gì?

<a id="m15-t037-s060"></a>

#### Identity Reconstruction

Ai nằm ở đây?

<a id="m15-t037-s061"></a>

#### Burial Context

Vật phẩm có ý nghĩa gì?

<a id="m15-t037-s062"></a>

#### Ownership

Ai có quyền lấy?

<a id="m15-t037-s063"></a>

#### Ritual Disturbance

Có tạo social/spiritual consequence không?

<a id="m15-t037-s064"></a>

### Encounter types

- forensic investigation;
- family dispute;
- artifact discovery;
- trapped structure;
- ritual;
- hidden crime evidence.

<a id="m15-t037-s065"></a>

### Fail states

- phá mất evidence;
- xúc phạm cộng đồng;
- collapse;
- awaken spiritual thread;
- trigger feud.

<a id="m15-t037-s066"></a>

### Example

“Mộ dưới Cồn Mây” có thể xuất hiện khi river shift lộ burial site.

---

<a id="m15-t037-s067"></a>

## 18. FOREST DUNGEON

<a id="m15-t037-s068"></a>

### Identity

Không gian mở nhưng **orientation thấp**.

Dungeon không cần tường.

Core pressure:

- navigation;
- visibility;
- time;
- animal presence;
- weather;
- taboo.

<a id="m15-t037-s069"></a>

### Signature mechanics

<a id="m15-t037-s070"></a>

#### Landmark Drift

Đường không rõ.

<a id="m15-t037-s071"></a>

#### Trail Layer

Nhiều trace chồng nhau.

<a id="m15-t037-s072"></a>

#### Sound Without Sight

Nghe nhưng chưa thấy.

<a id="m15-t037-s073"></a>

#### Resource Temptation

Rare herb/game có thể kéo player lệch route.

<a id="m15-t037-s074"></a>

#### Fire Risk

Lửa vừa công cụ vừa nguy hiểm.

<a id="m15-t037-s075"></a>

### Encounter types

- tracking;
- hunting;
- missing person;
- hostile patrol;
- sacred site;
- natural hazard.

<a id="m15-t037-s076"></a>

### Fail states

- lost;
- delayed;
- injured;
- trail lost;
- party split.

<a id="m15-t037-s077"></a>

### Narrative strength

Rất tốt cho:

Liệp Nhân.

Trinh Binh.

Dược Nhân.

Vu Chúc.

---

<a id="m15-t037-s078"></a>

## 19. SWAMP / MARSH DUNGEON

<a id="m15-t037-s079"></a>

### Identity

Không gian tạo áp lực bằng:

- footing;
- visibility;
- disease;
- hidden water;
- slow movement.

Core pressure:

- route uncertainty;
- fatigue;
- health;
- supply.

<a id="m15-t037-s080"></a>

### Signature mechanics

<a id="m15-t037-s081"></a>

#### Ground Reliability

Không phải đất nào cũng chịu được.

<a id="m15-t037-s082"></a>

#### Hidden Channels

Local guide cực mạnh.

<a id="m15-t037-s083"></a>

#### Fog Layer

Visibility thay đổi.

<a id="m15-t037-s084"></a>

#### Water Safety

Disease/poison.

<a id="m15-t037-s085"></a>

#### Load Penalty

Equipment heavy thành bất lợi.

<a id="m15-t037-s086"></a>

### Encounter types

- drowning risk;
- snake/animal;
- lost route;
- abandoned camp;
- hidden grave;
- disease;
- hostile boat.

<a id="m15-t037-s087"></a>

### Fail states

- stuck;
- lost;
- supply depletion;
- illness;
- equipment loss.

<a id="m15-t037-s088"></a>

### Narrative identity

Đại Trạch phải dùng dungeon type này thường xuyên nhưng không lặp cùng pattern.

---

<a id="m15-t037-s089"></a>

## 20. ABANDONED VILLAGE DUNGEON

<a id="m15-t037-s090"></a>

### Identity

Không gian quen thuộc nhưng vắng người.

Câu hỏi chính:

> chuyện gì đã xảy ra?

Core pressure:

- uncertainty;
- evidence;
- threat unknown;
- time.

<a id="m15-t037-s091"></a>

### Signature mechanics

<a id="m15-t037-s092"></a>

#### Environmental Evidence

Door.

Food.

Tools.

Fire.

Bodies.

Animals.

<a id="m15-t037-s093"></a>

#### Household Reconstruction

Ai sống ở đâu?

<a id="m15-t037-s094"></a>

#### Missing Population

Họ chết, chạy hay bị bắt?

<a id="m15-t037-s095"></a>

#### Hidden Occupant

Không chắc là village thực sự empty.

<a id="m15-t037-s096"></a>

### Encounter types

- investigation;
- survivor;
- looter;
- enemy scout;
- disease;
- family clue.

<a id="m15-t037-s097"></a>

### Fail states

- misread cause;
- destroy clue;
- trigger ambush;
- spread contamination.

<a id="m15-t037-s098"></a>

### Narrative value

Rất mạnh vì nối thẳng Social Graph và Household System.

---

<a id="m15-t037-s099"></a>

## 21. OCCUPIED / BESIEGED VILLAGE DUNGEON

<a id="m15-t037-s100"></a>

### Identity

Dungeon xã hội + tactical.

Core pressures:

- civilians;
- territory;
- faction;
- time;
- morale.

<a id="m15-t037-s101"></a>

### Signature mechanics

<a id="m15-t037-s102"></a>

#### Civilian Presence

Collateral consequence.

<a id="m15-t037-s103"></a>

#### Control Zones

Ai giữ phần nào.

<a id="m15-t037-s104"></a>

#### Hidden Loyalties

Không phải dân nào cũng cùng phe.

<a id="m15-t037-s105"></a>

#### Supply

Siege pressure.

<a id="m15-t037-s106"></a>

#### Evacuation

Can be main objective.

<a id="m15-t037-s107"></a>

### Encounter types

- negotiation;
- infiltration;
- combat;
- rescue;
- sabotage;
- leadership.

<a id="m15-t037-s108"></a>

### Fail states

- civilian casualties;
- village burns;
- faction escalation;
- hostage death;
- route cut.

<a id="m15-t037-s109"></a>

### Narrative use

Rất phù hợp chiến tranh Văn Lang–Âu Lạc.

---

<a id="m15-t037-s110"></a>

## 22. BATTLEFIELD DUNGEON

<a id="m15-t037-s111"></a>

### Identity

Không nhất thiết đang có battle.

Có thể là:

- trước trận;
- trong trận;
- sau trận.

Core pressures:

- danger;
- information;
- morale;
- terrain;
- casualties.

<a id="m15-t037-s112"></a>

### Signature mechanics

<a id="m15-t037-s113"></a>

#### Front Shift

Zone control thay đổi.

<a id="m15-t037-s114"></a>

#### Bodies/Wounded

Human consequence.

<a id="m15-t037-s115"></a>

#### Orders

Information delay.

<a id="m15-t037-s116"></a>

#### Battlefield Noise

Observation khó.

<a id="m15-t037-s117"></a>

#### Salvage vs Rescue

Moral/economic choice.

<a id="m15-t037-s118"></a>

### Encounter types

- tactical mission;
- wounded recovery;
- lost messenger;
- prisoner;
- route clearing;
- forensic reconstruction.

<a id="m15-t037-s119"></a>

### Fail states

- objective lost;
- retreat;
- unit routed;
- key NPC captured.

<a id="m15-t037-s120"></a>

### Aftermath

Battlefield có thể trở thành grave, ruin hoặc farmland nhiều năm sau.

---

<a id="m15-t037-s121"></a>

## 23. RIVER / BOAT DUNGEON

<a id="m15-t037-s122"></a>

### Identity

Không gian di chuyển.

Dungeon có thể tự đổi vị trí theo dòng.

Core pressures:

- water;
- boat condition;
- boarding;
- weather;
- cargo.

<a id="m15-t037-s123"></a>

### Signature mechanics

<a id="m15-t037-s124"></a>

#### Boat Integrity

Hull/rope/oar.

<a id="m15-t037-s125"></a>

#### Relative Position

Boats approach/separate.

<a id="m15-t037-s126"></a>

#### Water Hazard

Fall overboard.

<a id="m15-t037-s127"></a>

#### Cargo Shift

Weight matters.

<a id="m15-t037-s128"></a>

#### Route Commitment

Không phải lúc nào quay đầu được ngay.

<a id="m15-t037-s129"></a>

### Encounter types

- boarding;
- rescue;
- storm;
- navigation;
- hidden passenger;
- cargo dispute.

<a id="m15-t037-s130"></a>

### Fail states

- boat disabled;
- cargo lost;
- drift;
- drowning;
- forced shore landing.

<a id="m15-t037-s131"></a>

### Narrative value

Một trong những dungeon signature của Tam Giang/Hải Môn.

---

<a id="m15-t037-s132"></a>

## 24. CONSTRUCTION SITE / INDUSTRIAL DUNGEON

<a id="m15-t037-s133"></a>

### Identity

Không gian lao động đang hoạt động.

Đặc biệt quan trọng với Loa Vực/Cổ Loa.

Core pressures:

- machinery/process;
- labor;
- structural safety;
- politics;
- production schedule.

<a id="m15-t037-s134"></a>

### Signature mechanics

<a id="m15-t037-s135"></a>

#### Work Flow

Material đang đi đâu?

<a id="m15-t037-s136"></a>

#### Structural Stage

Construction changes map.

<a id="m15-t037-s137"></a>

#### Labor Groups

Different factions/households.

<a id="m15-t037-s138"></a>

#### Sabotage Possibility

Accident hay intentional?

<a id="m15-t037-s139"></a>

#### Supply Chain

Delay ở đâu?

<a id="m15-t037-s140"></a>

### Encounter types

- engineering;
- labor conflict;
- accident;
- sabotage;
- theft;
- political inspection.

<a id="m15-t037-s141"></a>

### Fail states

- collapse;
- worker casualties;
- production delay;
- faction blame;
- technology leak.

<a id="m15-t037-s142"></a>

### Narrative value

Cho phép Cổ Loa trở thành dungeon sống chứ không chỉ city hub.

---

<a id="m15-t037-s143"></a>

## 25. RITUAL / FESTIVAL DUNGEON

<a id="m15-t037-s144"></a>

### Identity

Dungeon hoàn toàn có thể không có combat.

Core pressure:

- etiquette;
- timing;
- visibility;
- reputation;
- secret agendas.

<a id="m15-t037-s145"></a>

### Signature mechanics

<a id="m15-t037-s146"></a>

#### Social Zones

Ai được vào đâu.

<a id="m15-t037-s147"></a>

#### Ritual Sequence

Thời điểm có ý nghĩa.

<a id="m15-t037-s148"></a>

#### Public vs Private

Một hành động trước đám đông khác hành động kín.

<a id="m15-t037-s149"></a>

#### Reputation Exposure

Ai nhìn thấy.

<a id="m15-t037-s150"></a>

#### Competing Agendas

Factions cùng tồn tại.

<a id="m15-t037-s151"></a>

### Encounter types

- dialogue;
- hidden meeting;
- ritual anomaly;
- theft;
- assassination attempt;
- political vote.

<a id="m15-t037-s152"></a>

### Fail states

- social humiliation;
- alliance broken;
- ritual failure;
- public accusation;
- violence.

<a id="m15-t037-s153"></a>

### Narrative value

Rất phù hợp Phong Châu.

---

<a id="m15-t037-s154"></a>

## 26. POLITICAL / SOCIAL DUNGEON

<a id="m15-t037-s155"></a>

### Identity

Một chuỗi social encounter liên kết.

Ví dụ:

đàm phán giữa ba dòng họ trong hai ngày.

Core pressures:

- trust;
- information;
- promise;
- time;
- faction position.

<a id="m15-t037-s156"></a>

### Signature mechanics

<a id="m15-t037-s157"></a>

#### Position Map

Không phải spatial map hoàn toàn.

Mỗi actor có:

Demand.

Red Line.

Leverage.

Secret.

<a id="m15-t037-s158"></a>

#### Information Nodes

Biết chuyện gì mở dialogue gì.

<a id="m15-t037-s159"></a>

#### Promise Objects

Lời hứa trở thành persistent state.

<a id="m15-t037-s160"></a>

#### Reputation Risk

Một lời nói có hậu quả dài hạn.

<a id="m15-t037-s161"></a>

### Encounter types

- negotiation;
- investigation;
- bribery;
- alliance;
- revelation;
- mediation.

<a id="m15-t037-s162"></a>

### Fail states

- talks collapse;
- violence;
- faction split;
- player loses credibility.

<a id="m15-t037-s163"></a>

### Narrative value

Đây là dungeon type rất đặc trưng của game và phải được coi ngang combat dungeon.

---

<a id="m15-t037-s164"></a>

## 27. DREAM / MẠCH DUNGEON

<a id="m15-t037-s165"></a>

### Identity

Không gian phi vật lý nhưng vẫn phải tuân theo logic.

Không phải dream = random surreal.

Core pressures:

- interpretation;
- identity;
- memory;
- Mạch instability.

<a id="m15-t037-s166"></a>

### Signature mechanics

<a id="m15-t037-s167"></a>

#### Symbol Layer

Symbol phải có nguồn từ memory/culture.

<a id="m15-t037-s168"></a>

#### Repeated Motif

Cho player pattern để suy luận.

<a id="m15-t037-s169"></a>

#### Identity Distortion

NPC/player có thể thấy version khác nhau.

<a id="m15-t037-s170"></a>

#### Causal Anchor

Dream phải liên kết event thật.

<a id="m15-t037-s171"></a>

#### Exit Condition

Không phải “đánh boss tỉnh dậy” mặc định.

<a id="m15-t037-s172"></a>

### Encounter types

- memory reconstruction;
- spiritual negotiation;
- symbolic choice;
- psychological confrontation;
- shared dream.

<a id="m15-t037-s173"></a>

### Fail states

- misinterpretation;
- wake without answer;
- stronger Mạch disturbance;
- false belief.

<a id="m15-t037-s174"></a>

### Narrative value

Dùng ít nhưng rất mạnh.

---

<a id="m15-t037-s175"></a>

## 28. MOBILE DUNGEON

<a id="m15-t037-s176"></a>

### Identity

Dungeon là một đoàn người/đối tượng đang di chuyển.

Ví dụ:

- caravan;
- refugee column;
- military convoy;
- prisoner transfer.

Core pressures:

- route;
- supplies;
- internal conflict;
- external pursuit.

<a id="m15-t037-s177"></a>

### Signature mechanics

<a id="m15-t037-s178"></a>

#### Moving Objective

Map thay đổi từng ngày.

<a id="m15-t037-s179"></a>

#### Internal Actor Graph

Mối quan hệ bên trong đoàn.

<a id="m15-t037-s180"></a>

#### Supply Pressure

Food/cargo.

<a id="m15-t037-s181"></a>

#### Route Choice

Player influence.

<a id="m15-t037-s182"></a>

#### Pursuit

Enemy distance.

<a id="m15-t037-s183"></a>

### Encounter types

- dispute;
- illness;
- desertion;
- attack;
- negotiation;
- route crisis.

<a id="m15-t037-s184"></a>

### Fail states

- group splits;
- supplies lost;
- target escapes;
- pursuit catches up.

<a id="m15-t037-s185"></a>

### Narrative value

Kết nối Travel và Dungeon rất tự nhiên.

---

<a id="m15-t037-s186"></a>

## PART III — ENCOUNTER SYSTEM

<a id="m15-t037-s187"></a>

## 29. ENCOUNTER PHILOSOPHY

Encounter không tồn tại chỉ để “có gameplay”.

Một encounter chỉ nên interactive khi có ít nhất một trong các yếu tố:

- meaningful decision;
- risk;
- new information;
- relationship consequence;
- world state change;
- resource trade-off;
- story thread progression.

Nếu không có:

tóm tắt.

---

<a id="m15-t037-s188"></a>

## 30. ENCOUNTER STRUCTURE

Mỗi Encounter Definition có:

```text
Encounter ID
Type
Trigger
Location Context
Participants
Known Information
Hidden Information
Player Goal
Actor Goals
Available Approaches
Skill/Knowledge Hooks
Resource Costs
Risk
Success States
Failure States
Follow-up
Persistence
```

---

<a id="m15-t037-s189"></a>

## 31. COMBAT ENCOUNTER

<a id="m15-t037-s190"></a>

### Purpose

Conflict vật lý.

Nhưng phải có:

objective.

enemy motive.

retreat logic.

<a id="m15-t037-s191"></a>

### Approaches trước combat

- avoid;
- negotiate;
- ambush;
- prepare;
- intimidate.

<a id="m15-t037-s192"></a>

### During encounter

Combat System.

<a id="m15-t037-s193"></a>

### Outcomes

- kill;
- rout;
- surrender;
- capture;
- retreat;
- partial success.

<a id="m15-t037-s194"></a>

### Persistent effects

- wound;
- death;
- reputation;
- prisoner;
- property damage.

---

<a id="m15-t037-s195"></a>

## 32. SOCIAL ENCOUNTER

<a id="m15-t037-s196"></a>

### Purpose

Thay đổi quyết định hoặc quan hệ giữa actors.

<a id="m15-t037-s197"></a>

### Core elements

- goals;
- leverage;
- relationship;
- information;
- red lines.

<a id="m15-t037-s198"></a>

### Approaches

- persuade;
- negotiate;
- deceive;
- intimidate;
- appeal to duty;
- offer resource;
- invoke custom.

<a id="m15-t037-s199"></a>

### Failure

Không phải luôn “conversation ends”.

Có thể:

- price rises;
- trust falls;
- new condition added;
- actor leaves.

<a id="m15-t037-s200"></a>

### Persistence

Promise, resentment, debt và reputation được lưu.

---

<a id="m15-t037-s201"></a>

## 33. INVESTIGATION ENCOUNTER

<a id="m15-t037-s202"></a>

### Purpose

Giảm uncertainty về event hoặc actor.

<a id="m15-t037-s203"></a>

### Core elements

- clues;
- witnesses;
- physical traces;
- contradictions.

<a id="m15-t037-s204"></a>

### Approaches

- search;
- question;
- reconstruct;
- professional analysis;
- compare stories.

<a id="m15-t037-s205"></a>

### Failure

Có thể:

- clue missed;
- false hypothesis;
- evidence damaged;
- witness withdraws.

<a id="m15-t037-s206"></a>

### Persistence

Player knowledge graph cập nhật.

---

<a id="m15-t037-s207"></a>

## 34. ENVIRONMENTAL ENCOUNTER

<a id="m15-t037-s208"></a>

### Purpose

Vượt hazard tự nhiên hoặc thay đổi không gian.

Ví dụ:

- flood;
- unstable bridge;
- fire;
- landslide.

<a id="m15-t037-s209"></a>

### Approaches

- cross;
- repair;
- detour;
- wait;
- reduce load;
- use specialist.

<a id="m15-t037-s210"></a>

### Failure

Thường:

delay;

injury;

equipment loss;

route change.

Không nhất thiết death.

---

<a id="m15-t037-s211"></a>

## 35. STEALTH / INFILTRATION ENCOUNTER

<a id="m15-t037-s212"></a>

### Purpose

Tiến qua khu vực mà không làm actor khác biết hoặc không tăng Alert.

<a id="m15-t037-s213"></a>

### Core elements

- detection;
- route;
- timing;
- noise;
- disguise/social cover.

<a id="m15-t037-s214"></a>

### Approaches

- hide;
- distract;
- social infiltration;
- alternate route;
- wait.

<a id="m15-t037-s215"></a>

### Failure bands

- suspicion;
- partial detection;
- pursuit;
- full alert.

Không binary:

seen/not seen.

---

<a id="m15-t037-s216"></a>

## 36. RESOURCE / LOGISTICS ENCOUNTER

<a id="m15-t037-s217"></a>

### Purpose

Xử lý thiếu:

- food;
- material;
- transport;
- manpower.

<a id="m15-t037-s218"></a>

### Example

Bridge repair cần wood.

Player có thể:

- harvest;
- buy;
- ask village;
- dismantle abandoned structure.

<a id="m15-t037-s219"></a>

### Failure

- delay;
- debt;
- political resentment;
- alternative resource.

<a id="m15-t037-s220"></a>

### Value

Cho Profession và Economy cơ hội gameplay thật.

---

<a id="m15-t037-s221"></a>

## 37. ENGINEERING / CRAFT ENCOUNTER

<a id="m15-t037-s222"></a>

### Purpose

Giải quyết vấn đề vật lý bằng craft knowledge.

<a id="m15-t037-s223"></a>

### Examples

- repair boat;
- stabilize tunnel;
- open mechanism;
- build crossing.

<a id="m15-t037-s224"></a>

### Core variables

- material;
- tool;
- skill;
- time;
- safety.

<a id="m15-t037-s225"></a>

### Failure

Không chỉ “craft fail”.

Có thể:

temporary solution;

resource waste;

structural risk.

---

<a id="m15-t037-s226"></a>

## 38. MORAL ENCOUNTER

<a id="m15-t037-s227"></a>

### Purpose

Không phải skill challenge.

Player phải chọn giữa các giá trị xung đột.

Ví dụ:

- cứu một người hay giữ bí mật;
- phá mộ để tìm bằng chứng;
- dùng food dự trữ của làng.

<a id="m15-t037-s228"></a>

### Key rule

Không gắn:

Good/Evil.

Engine ghi:

action;

witness;

interpretation;

consequence.

<a id="m15-t037-s229"></a>

### Value

Đây là encounter dạng decision thuần narrative nhưng phải persistent.

---

<a id="m15-t037-s230"></a>

## 39. RITUAL / SPIRITUAL ENCOUNTER

<a id="m15-t037-s231"></a>

### Purpose

Tương tác với nghi lễ, belief hoặc Mạch.

<a id="m15-t037-s232"></a>

### Core elements

- cultural correctness;
- participant;
- object;
- timing;
- Mạch state.

<a id="m15-t037-s233"></a>

### Approaches

- perform;
- observe;
- refuse;
- modify;
- investigate.

<a id="m15-t037-s234"></a>

### Failure

Có thể:

social failure;

psychological failure;

Mạch reaction;

không có supernatural effect.

---

<a id="m15-t037-s235"></a>

## 40. CHASE / PURSUIT ENCOUNTER

<a id="m15-t037-s236"></a>

### Purpose

Khoảng cách và time pressure.

<a id="m15-t037-s237"></a>

### Core variables

- speed;
- terrain;
- knowledge;
- stamina;
- route choice.

<a id="m15-t037-s238"></a>

### Approaches

- pursue;
- intercept;
- hide;
- split party;
- use shortcut.

<a id="m15-t037-s239"></a>

### Failure

Target escapes.

Player gets lost.

Party fatigued.

Không nhất thiết combat.

---

<a id="m15-t037-s240"></a>

## 41. RESCUE ENCOUNTER

<a id="m15-t037-s241"></a>

### Purpose

Đưa actor khỏi danger.

<a id="m15-t037-s242"></a>

### Core variables

- time;
- location;
- injury;
- hazard;
- carrying capacity.

<a id="m15-t037-s243"></a>

### Approaches

- direct rescue;
- stabilize first;
- create route;
- distract threat.

<a id="m15-t037-s244"></a>

### Failure

Có thể:

rescue delayed;

injury worsens;

rescuer harmed.

<a id="m15-t037-s245"></a>

### Value

Rất hợp Party/Relationship.

---

<a id="m15-t037-s246"></a>

## 42. PUZZLE / LOGIC ENCOUNTER

<a id="m15-t037-s247"></a>

### Purpose

Không phải “câu đố videogame” vô cớ.

Puzzle phải xuất phát từ world.

Ví dụ:

- hiểu route nước;
- cơ cấu;
- ritual sequence;
- historical pattern.

<a id="m15-t037-s248"></a>

### Core rule

Không cần player ngoài đời đoán nếu character đã đủ knowledge.

Cho phép:

Character Insight.

Player reasoning.

Kết hợp cả hai.

<a id="m15-t037-s249"></a>

### Failure

Time cost.

Resource cost.

Alternative route.

Không hard-lock.

---

<a id="m15-t037-s250"></a>

## PART IV — ENCOUNTER CHAINS

<a id="m15-t037-s251"></a>

## 43. ENCOUNTER KHÔNG TỒN TẠI RỜI NHAU

Một dungeon mạnh thường tạo chain.

Ví dụ:

Investigation

↓

Environmental

↓

Social

↓

Stealth

↓

Combat hoặc Negotiation

↓

Rescue

↓

Escape.

---

<a id="m15-t037-s252"></a>

## 44. CHAIN STATE TRANSFER

Kết quả encounter trước ảnh hưởng encounter sau.

Ví dụ Stealth fail:

Alert cao.

Combat sau khó hơn.

Nhưng player có thể lấy được information tốt hơn.

Không reset state.

---

<a id="m15-t037-s253"></a>

## 45. ENCOUNTER ESCALATION

Một encounter có thể biến type.

Social

→ argument

→ combat.

Combat

→ chase.

Investigation

→ ritual.

Environmental

→ rescue.

Hệ thống phải hỗ trợ transition.

---

<a id="m15-t037-s254"></a>

## 46. ENCOUNTER DE-ESCALATION

Ngược lại:

combat

→ surrender

→ negotiation.

Pursuit

→ hidden.

Ritual panic

→ medical treatment.

Player có thể kéo situation về mức thấp hơn.

---

<a id="m15-t037-s255"></a>

## PART V — TENSION & ALERT

<a id="m15-t037-s256"></a>

## 47. ALERT SYSTEM

Không phải dungeon nào cũng dùng.

Phù hợp:

- infiltration;
- occupied settlement;
- military camp;
- political event.

Alert có thể:

Unaware.

Suspicious.

Searching.

Alerted.

Lockdown.

---

<a id="m15-t037-s257"></a>

## 48. ALERT IS SHARED INFORMATION

Không phải tất cả NPC đồng bộ telepathically.

Alert propagation cần:

shout.

signal.

messenger.

drum.

visibility.

Khe Cờ-style signal mechanics có thể áp dụng.

---

<a id="m15-t037-s258"></a>

## 49. PLAYER CAN DISRUPT ALERT NETWORK

Ví dụ:

cut signal line.

capture messenger.

create false signal.

Trinh Binh/Sứ Giả có giá trị.

---

<a id="m15-t037-s259"></a>

## 50. TENSION SYSTEM

Tension là độ nguy hiểm tổng thể.

Không nhất thiết UI hiển thị thanh.

Tension tăng bởi:

- time;
- injury;
- noise;
- failed check;
- clock;
- Mạch anomaly.

Tension cao có thể mở encounter mới.

---

<a id="m15-t037-s260"></a>

## 51. TENSION RELEASE

Không phải chỉ tăng.

Có thể giảm bằng:

- rest;
- hide;
- complete sub-objective;
- calm NPC;
- secure zone.

Pacing cần nhịp:

tăng → giảm → tăng.

---

<a id="m15-t037-s261"></a>

## PART VI — RESOURCE WITHIN DUNGEON

<a id="m15-t037-s262"></a>

## 52. DUNGEON RESOURCE

Không dùng universal “dungeon mana”.

Các resource có thể:

- time;
- light;
- food;
- stamina;
- medical supplies;
- rope;
- information;
- social goodwill;
- stealth;
- air;
- boat integrity.

Chỉ dùng resource phù hợp context.

---

<a id="m15-t037-s263"></a>

## 53. INFORMATION AS RESOURCE

Một trong những resource quan trọng nhất.

Player có thể:

biết layout.

biết guard rotation.

biết taboo.

biết target location.

Mỗi information piece giảm risk.

---

<a id="m15-t037-s264"></a>

## 54. INFORMATION CAN BE WRONG

Source reliability.

Rumor.

Intentional deception.

Dungeon prep có uncertainty.

---

<a id="m15-t037-s265"></a>

## 55. SAFE ZONES

Một số dungeon có:

rest location;

ally house;

secure room;

boat.

Safe zone không hoàn toàn magic.

Nếu dungeon state thay đổi:

có thể mất an toàn.

---

<a id="m15-t037-s266"></a>

## PART VII — PARTY & DUNGEON

<a id="m15-t037-s267"></a>

## 56. PARTY COMPOSITION CHANGES DUNGEON

Một dungeon không được xây quanh một mandatory class.

Nhưng capability thay đổi route.

Ví dụ tomb:

Người Giữ Ký Ức:

understand burial.

Xảo Nhân:

stabilize structure.

Vu Chúc:

ritual context.

Liệp Nhân:

track looter.

Sứ Giả:

negotiate family claim.

---

<a id="m15-t037-s268"></a>

## 57. PARTY SPLIT

Cho phép tạm chia trong dungeon khi:

- scout;
- hold position;
- escort;
- simultaneous task.

Nhưng UI/AI complexity phải hạn chế.

Không khuyến khích chia 4 người thành 4 map riêng thường xuyên.

---

<a id="m15-t037-s269"></a>

## 58. LEAVE SOMEONE BEHIND

Companion bị thương có thể:

wait in safe zone.

guard prisoner.

protect civilian.

Party composition tạm thay đổi.

---

<a id="m15-t037-s270"></a>

## 59. COMPANION KNOWLEDGE

Companion có thể nhận ra:

place.

person.

object.

Dungeon có personal reaction.

Không phải player mới có story.

---

<a id="m15-t037-s271"></a>

## PART VIII — DUNGEON & WORLD PERSISTENCE

<a id="m15-t037-s272"></a>

## 60. CLEARED DUNGEON KHÔNG BIẾN MẤT

Sau resolution:

hang vẫn tồn tại.

mộ vẫn tồn tại.

làng vẫn sống.

Actor khác có thể tới.

---

<a id="m15-t037-s273"></a>

## 61. REPOPULATION KHÔNG PHẢI RESPAWN

Nếu player diệt bandit:

không 3 ngày sau spawn bandit mới cùng chỗ.

Nhưng sau vài năm:

refugees.

hunters.

soldiers.

có thể dùng location.

Có causal explanation.

---

<a id="m15-t037-s274"></a>

## 62. PHYSICAL CHANGE

Player có thể:

collapse tunnel.

burn house.

break bridge.

open passage.

World lưu.

---

<a id="m15-t037-s275"></a>

## 63. SOCIAL CHANGE

Dungeon actor chết/rời:

control thay đổi.

Family/faction phản ứng.

---

<a id="m15-t037-s276"></a>

## 64. KNOWLEDGE CHANGE

Player có thể:

khám phá history.

Sau đó map/codex/dialogue thay đổi.

---

<a id="m15-t037-s277"></a>

## 65. REVISIT

Một dungeon mạnh nên có ít nhất một số trạng thái revisit.

Ví dụ:

abandoned mine

→ bandit hideout

→ military storage

→ ruin.

---

<a id="m15-t037-s278"></a>

## PART IX — PROCEDURAL VS AUTHORED

<a id="m15-t037-s279"></a>

## 66. HYBRID DESIGN

Major Dungeon:

hand-authored backbone.

Minor nodes/encounters:

procedural.

Không full procedural.

---

<a id="m15-t037-s280"></a>

## 67. AUTHORED ELEMENTS

Phải authored:

- history;
- main objective;
- key actors;
- major reveal;
- unique location;
- thematic structure.

---

<a id="m15-t037-s281"></a>

## 68. PROCEDURAL ELEMENTS

Có thể procedural:

- weather;
- minor hazard;
- actor position;
- resource;
- optional encounter;
- route state.

---

<a id="m15-t037-s282"></a>

## 69. PROCEDURAL GENERATION CONSTRAINT

Generator phải dùng:

Region.

Season.

World State.

Story Thread.

Actors.

Không random room table universal.

---

<a id="m15-t037-s283"></a>

## 70. PROCEDURAL DUNGEON MUST HAVE CAUSE

Ví dụ generated hunter camp:

Ai dựng?

Tại sao?

Bao lâu?

Có vật gì?

Không chỉ:

Camp01.

---

<a id="m15-t037-s284"></a>

## PART X — DUNGEON PACING

<a id="m15-t037-s285"></a>

## 71. TENSION CURVE

Một dungeon trung bình có thể:

Entry

→ Orientation

→ First Complication

→ Discovery

→ Rising Pressure

→ Decision Point

→ Crisis

→ Resolution

→ Aftermath.

Không bắt buộc mọi dungeon đúng pattern.

---

<a id="m15-t037-s286"></a>

## 72. NO CONSTANT HIGH TENSION

Nếu mọi node nguy hiểm:

mệt.

Cần:

quiet observation.

dialogue.

discovery.

---

<a id="m15-t037-s287"></a>

## 73. REST BEAT

Có thể có:

safe room.

camp.

ally.

Pause giúp companion banter và reflection.

---

<a id="m15-t037-s288"></a>

## 74. REVEAL BEAT

Dungeon nên có 1–3 key information moments.

Không dump lore một lần.

---

<a id="m15-t037-s289"></a>

## PART XI — DIFFICULTY

<a id="m15-t037-s290"></a>

## 75. DUNGEON DIFFICULTY IS MULTI-DIMENSIONAL

Không có Dungeon Level 20 đơn giản.

Difficulty gồm:

- Combat;
- Navigation;
- Social;
- Environmental;
- Time;
- Knowledge;
- Resource.

---

<a id="m15-t037-s291"></a>

## 76. PLAYER CAN BE STRONG IN ONE AXIS AND WEAK IN ANOTHER

Party martial mạnh:

combat dễ.

Nhưng tomb interpretation khó.

Social party:

negotiation tốt.

Nhưng cave hazard khó.

---

<a id="m15-t037-s292"></a>

## 77. PREPARATION CAN LOWER DIFFICULTY

Guide.

Tool.

Knowledge.

Alternative entry.

Timing.

Không chỉ grind level.

---

<a id="m15-t037-s293"></a>

## PART XII — EXAMPLE FULL DUNGEON: MỘ DƯỚI CỒN MÂY

<a id="m15-t037-s294"></a>

## 78. ORIGIN

Sau mùa nước thấp bất thường, dòng sông khoét lộ một cấu trúc burial tại Cồn Mây.

Người địa phương phát hiện:

gỗ cũ.

đồ đồng.

xương.

Một số người muốn đóng lại.

Một số muốn lấy vật.

Một family nói người chết thuộc tổ tiên họ.

---

<a id="m15-t037-s295"></a>

## 79. OBJECTIVES

Possible main goals:

- xác định burial là của ai;
- ngăn cướp mộ;
- tìm một vật đang bị mất;
- cứu người đã vào trước và mắc kẹt.

Player có thể tới từ nhiều Story Thread khác nhau.

---

<a id="m15-t037-s296"></a>

## 80. PRESSURES

Water Clock:

nước sẽ lên lại.

Structural Stability:

gỗ/mud yếu.

Social Pressure:

nhiều family tranh quyền.

Spiritual Pressure:

rumor về dream tăng.

---

<a id="m15-t037-s297"></a>

## 81. ENTRY OPTIONS

<a id="m15-t037-s298"></a>

#### Open River Cut

Nhanh.

Structural risk.

<a id="m15-t037-s299"></a>

#### Upper Soil Shaft

Cần Xảo Nhân.

<a id="m15-t037-s300"></a>

#### Old Drain

Chỉ người già địa phương biết.

<a id="m15-t037-s301"></a>

#### Wait For Excavation

An toàn hơn nhưng water clock tiến.

---

<a id="m15-t037-s302"></a>

## 82. NODE 1 — EXPOSED BURIAL EDGE

Encounter:

Investigation.

Player thấy:

- wood;
- sediment;
- disturbed footprints.

Liệp Nhân:

nhận ra đã có người vào.

Người Giữ Ký Ức:

nhận kiểu burial.

Xảo Nhân:

đánh giá stability.

---

<a id="m15-t037-s303"></a>

## 83. NODE 2 — COLLAPSED PASSAGE

Engineering/Environmental Encounter.

Options:

clear.

brace.

crawl.

find alternative.

Failure:

injury.

time.

collapse.

---

<a id="m15-t037-s304"></a>

## 84. NODE 3 — BURIAL CHAMBER

Không có chest.

Có:

human remains.

grave goods.

ritual arrangement.

Player phải quyết:

touch?

document?

leave?

---

<a id="m15-t037-s305"></a>

## 85. NODE 4 — MISSING OBJECT TRACE

Nếu investigation tốt:

nhận ra một object đã bị lấy gần đây.

Tracking leads elsewhere.

---

<a id="m15-t037-s306"></a>

## 86. NODE 5 — TRAPPED LOOTER

Người đã lấy đồ bị mắc khi passage sập.

Player có thể:

rescue.

question.

leave.

take object.

---

<a id="m15-t037-s307"></a>

## 87. SOCIAL CONSEQUENCE

Object thuộc burial.

Nhưng một family tuyên bố quyền.

Family khác phản đối.

Truth có thể:

không family nào thật sự liên quan.

---

<a id="m15-t037-s308"></a>

## 88. SPIRITUAL LAYER

Dream reports có thể tăng nếu burial disturbed.

Game không nhất thiết xác nhận ghost.

Mạch Lạc có thể phản ứng với:

disturbance.

memory.

mass belief.

---

<a id="m15-t037-s309"></a>

## 89. RESOLUTION STATES

<a id="m15-t037-s310"></a>

#### Preserve

Burial đóng lại.

Community trust tăng.

<a id="m15-t037-s311"></a>

#### Investigate

History được biết rõ hơn.

<a id="m15-t037-s312"></a>

#### Looted

Economic gain.

Social/spiritual cost.

<a id="m15-t037-s313"></a>

#### Claimed

Một family lấy control.

Political consequence.

<a id="m15-t037-s314"></a>

#### Destroyed

Water/structure làm mất site.

Knowledge lost.

---

<a id="m15-t037-s315"></a>

## 90. REVISIT

Năm sau:

site có thể:

- trở thành shrine;
- bị river phá;
- được family bảo vệ;
- bị forgotten;
- trở thành myth location.

Đây là một dungeon hoàn chỉnh mà không cần một con quái nào.

---

<a id="m15-t037-s316"></a>

## PART XIII — DUNGEON REWARD

<a id="m15-t037-s317"></a>

## 91. REWARD PHILOSOPHY

Dungeon không phải loot dispenser.

Reward có thể là:

- Knowledge;
- Reputation;
- Relationship;
- Route;
- Technique;
- Artifact;
- Resource;
- Historical truth;
- Political leverage;
- Companion development;
- World change.

---

<a id="m15-t037-s318"></a>

## 92. MATERIAL REWARD

Có thể có.

Nhưng phải hợp location.

Mine:

ore.

Workshop:

tools.

Tomb:

grave goods nhưng có ownership consequence.

---

<a id="m15-t037-s319"></a>

## 93. KNOWLEDGE REWARD

Rất quan trọng.

Ví dụ:

old route.

forgotten technique.

family truth.

ritual detail.

---

<a id="m15-t037-s320"></a>

## 94. SOCIAL REWARD

Access.

Trust.

Title.

Invitation.

Faction support.

---

<a id="m15-t037-s321"></a>

## 95. CLASS EVIDENCE

Dungeon milestone có thể giúp Class Emergence.

Ví dụ:

Liệp Nhân giải missing trail.

Xảo Nhân cứu công trình.

Sứ Giả hòa giải siege.

---

<a id="m15-t037-s322"></a>

## PART XIV — FAILURE

<a id="m15-t037-s323"></a>

## 96. FAIL FORWARD

Dungeon failure phải tạo state mới.

Ví dụ:

target escaped.

Không:

quest deleted.

Nó có thể trở thành:

chase thread.

---

<a id="m15-t037-s324"></a>

## 97. HARD FAILURE

Chỉ một số trường hợp.

Ví dụ:

artifact destroyed.

NPC dies.

Site collapses.

Nhưng story vẫn tiếp tục với hậu quả.

---

<a id="m15-t037-s325"></a>

## 98. PLAYER CAPTURE

Một số dungeon có thể dẫn tới:

capture

thay vì game over.

Mở:

prison escape.

negotiation.

ransom.

---

<a id="m15-t037-s326"></a>

## 99. PARTY WIPE

Standard Mode:

có thể dùng rescue/capture nếu hợp context.

Legacy Mode:

death/succession.

Không universal reload requirement.

---

<a id="m15-t037-s327"></a>

## PART XV — ENCOUNTER FREQUENCY

<a id="m15-t037-s328"></a>

## 100. MINOR DUNGEON

Khoảng:

3–5 meaningful encounters.

20–40 phút.

---

<a id="m15-t037-s329"></a>

## 101. STANDARD DUNGEON

Khoảng:

5–10 meaningful encounters.

45–120 phút.

---

<a id="m15-t037-s330"></a>

## 102. MAJOR DUNGEON / STORY SITE

Có thể:

nhiều session.

re-entry.

world state changes.

Không nên kéo dài chỉ vì room count.

---

<a id="m15-t037-s331"></a>

## 103. ENCOUNTER DENSITY

Một quiet node vẫn có value nếu:

information;

mood;

choice.

Không cần action mỗi phút.

---

<a id="m15-t037-s332"></a>

## PART XVI — UI

<a id="m15-t037-s333"></a>

## 104. DUNGEON MAP

Không auto-reveal.

Player map dựa trên:

exploration.

memory.

knowledge.

---

<a id="m15-t037-s334"></a>

## 105. LOCAL MAP

Can show:

known zones.

known exits.

hazards.

Not hidden actor exact position unless observed.

---

<a id="m15-t037-s335"></a>

## 106. NOTES

Player/character can annotate:

blocked.

unsafe.

heard sound.

possible exit.

---

<a id="m15-t037-s336"></a>

## 107. OBJECTIVE UI

Không checklist quá gamey.

Hiển thị:

Current Goal.

Known Leads.

Urgent Conditions.

Ví dụ:

> Nước đang lên.

> Có ít nhất một người đã vào trước.

---

<a id="m15-t037-s337"></a>

## 108. CLOCK UI

Có thể natural language.

Không nhất thiết progress bar.

Ví dụ:

> “Nước đã tới mắt cá ở lối vào.”

Later:

> “Nước đang dâng nhanh.”

---

<a id="m15-t037-s338"></a>

## PART XVII — AI & LLM ROLE

<a id="m15-t037-s339"></a>

## 109. SYSTEM DECIDES FACTS

Simulation determines:

- actor;
- location;
- state;
- clue;
- danger;
- objective.

LLM không tự thêm cửa bí mật.

---

<a id="m15-t037-s340"></a>

## 110. LLM PRESENTATION

LLM viết:

- description;
- NPC dialogue;
- rumor;
- sensory detail.

Không quyết:

- dungeon logic;
- hidden truth;
- loot;
- consequence.

---

<a id="m15-t037-s341"></a>

## 111. DUNGEON CONSISTENCY

Nếu một passage collapsed:

LLM không được sau đó mô tả NPC đi qua trừ state đã thay đổi.

---

<a id="m15-t037-s342"></a>

## PART XVIII — QUALITY GATE

<a id="m15-t037-s343"></a>

## 112. DUNGEON QUALITY QUESTIONS

Mỗi major dungeon phải trả lời được:

1. Vì sao nơi này tồn tại?
2. Vì sao player vào?
3. Ai khác quan tâm nơi này?
4. Áp lực chính là gì?
5. Có ít nhất hai entry/approach không?
6. Có ít nhất ba loại encounter không?
7. Có discovery thật không?
8. Party composition thay đổi cách chơi thế nào?
9. Có non-combat route không?
10. Có retreat không?
11. Failure tạo gì tiếp?
12. Resolution thay world ra sao?
13. Có lý do để revisit không?
14. Dungeon có identity riêng hay chỉ reskin?
15. Có ít nhất một phần kết nối history/social graph không?

---

<a id="m15-t037-s344"></a>

## 113. ENCOUNTER QUALITY QUESTIONS

Encounter chỉ giữ nếu trả lời được:

- Player quyết gì?
- Rủi ro là gì?
- Có thông tin mới không?
- Outcome có khác nhau không?
- State nào thay đổi?
- Nó có phù hợp context không?

Nếu không:

summarize.

---

<a id="m15-t037-s345"></a>

## PART XIX — INTERACTION WITH PREVIOUS SYSTEMS

<a id="m15-t037-s346"></a>

## 114. CHARACTER GENERATION

Origin cho:

- local knowledge;
- taboo;
- route;
- personal reaction.

---

<a id="m15-t037-s347"></a>

## 115. ATTRIBUTES & SKILLS

Quyết định:

- detection;
- movement;
- social;
- craft;
- survival.

---

<a id="m15-t037-s348"></a>

## 116. PROFESSION

Tạo:

- shortcut;
- interpretation;
- resource solution;
- authority.

---

<a id="m15-t037-s349"></a>

## 117. CLASS

Tạo gameplay identity trong dungeon.

Không tạo class-gated door cứng.

---

<a id="m15-t037-s350"></a>

## 118. PARTY

Companion có:

- expertise;
- conflict;
- personal memory.

---

<a id="m15-t037-s351"></a>

## 119. COMBAT

Một encounter type trong dungeon.

Không phải default.

---

<a id="m15-t037-s352"></a>

## 120. EQUIPMENT

Tool.

light.

rope.

weapon.

protection.

load.

Mỗi thứ có context.

---

<a id="m15-t037-s353"></a>

## 121. INJURY

Làm dungeon khó hơn.

Có thể buộc retreat.

---

<a id="m15-t037-s354"></a>

## 122. EXPLORATION

Dungeon là Local/Site layer của Exploration System.

---

<a id="m15-t037-s355"></a>

## 123. NARRATIVE ENGINE

Story Thread tạo dungeon objective.

Dungeon outcome tạo Story Thread mới.

---

<a id="m15-t037-s356"></a>

## 124. SOCIAL GRAPH

Dungeon actors có relationship thật.

Không generic quest giver/enemy.

---

<a id="m15-t037-s357"></a>

## 125. ECONOMY

Mine.

trade post.

warehouse.

construction site.

Dungeon có economic meaning.

---

<a id="m15-t037-s358"></a>

## 126. FACTION

Control.

permission.

security.

political consequence.

---

<a id="m15-t037-s359"></a>

## 127. MẠCH LẠC

Spiritual dungeon là một layer.

Không thay physical logic.

---

<a id="m15-t037-s360"></a>

## 128. LEGACY

Dungeon resolution có thể:

- tạo shrine;
- create route;
- destroy site;
- become myth.

---

<a id="m15-t037-s361"></a>

## PART XX — DEFERRED TO PRODUCTION

Production sẽ làm sâu:

- exact number of zones;
- encounter budget;
- alert thresholds;
- clock timing;
- minor procedural templates;
- stealth detection formula;
- dungeon visual language;
- site art;
- environmental hazard numbers;
- full library of authored major dungeons;
- dream/Mạch presentation;
- local map UI;
- procedural encounter grammar.

---

<a id="m15-t037-s362"></a>

## 129. CÁC QUYẾT ĐỊNH ĐÃ KHÓA

Dungeon là structured tension space, không phải hang có quái.

Dungeon có thể là physical, social, political, ritual hoặc psychological.

Encounter là đơn vị decision, không đồng nghĩa combat.

Dungeon dùng node/graph.

Entry và exit có thể nhiều.

Dungeon có persistent state.

Dungeon có clocks và pressures.

Retreat hợp lệ.

Partial completion hợp lệ.

Fail-forward là mặc định.

Major dungeon phải kết nối history/world/social graph.

Dungeon không reset khi player rời.

Repopulation cần causal reason.

Procedural content chỉ làm lớp phụ.

Major backbone phải authored.

Encounter chỉ interactive khi có meaningful choice hoặc consequence.

Combat chỉ là một loại encounter.

Social dungeon được coi quan trọng ngang physical dungeon.

Dream/Mạch dungeon phải tuân logic.

World state thay dungeon theo thời gian.

Party composition thay cách giải dungeon.

Không class nào bắt buộc.

Knowledge là reward quan trọng.

Loot không phải mục tiêu mặc định.

Dungeon có thể thay world map sau resolution.

---

<a id="m15-t037-s363"></a>

## 130. CORE DUNGEON STATEMENT

Dungeon tốt của game này không khiến player hỏi:

> “Có bao nhiêu phòng nữa?”

Mà khiến họ hỏi:

> “Tôi còn hiểu sai điều gì về nơi này?”

> “Có đáng đi sâu hơn không?”

> “Nếu tiếp tục, tôi đang mạo hiểm ai?”

> “Tôi có thể giải quyết chuyện này mà không đánh nhau không?”

> “Nếu tôi lấy thứ này, ai sẽ phải trả giá?”

> “Nếu tôi rút bây giờ, chuyện gì xảy ra tiếp?”

Một mộ cổ có thể nguy hiểm vì sắp sập.

Một khu rừng có thể nguy hiểm vì người chơi mất phương hướng.

Một lễ hội có thể nguy hiểm vì một câu nói sai trước hàng trăm người.

Một battle site có thể nguy hiểm dù trận đánh đã kết thúc.

Một giấc mơ có thể nguy hiểm vì player tin nhầm điều nó nói.

Một dungeon vì vậy không được đo bằng số enemy.

Nó được đo bằng:

> **độ sâu của quyết định mà nó tạo ra.**

---

<a id="m15-t037-s364"></a>

## 131. CORE ENCOUNTER STATEMENT

Encounter tốt không hỏi:

> “Skill check nào đúng?”

Mà hỏi:

> **“Bạn hiểu tình huống này như thế nào, và bạn sẵn sàng trả giá gì cho cách giải quyết của mình?”**

Một party có thể đi qua cùng một dungeon bằng:

- chiến đấu;
- quan hệ;
- chuyên môn;
- lịch sử;
- nghề nghiệp;
- sự chuẩn bị;
- hoặc chấp nhận mất thứ gì đó.

Và chính vì vậy hai playthrough cùng đi qua một location vẫn có thể tạo ra hai câu chuyện hoàn toàn khác nhau.

Với module này, vòng gameplay cốt lõi đã khá đầy: **World Map → Travel → Dungeon → Encounter → Combat/Social/Investigation → Consequence → World Change**.

Bước tiếp theo hợp lý nhất là **Dialogue + Social Interaction + Negotiation + Reputation + Faction**, vì sau khi đã có dungeon xã hội và political encounter, ta cần khóa chính xác cách hội thoại, lời hứa, thương lượng, danh tiếng và phe phái vận hành. Sau đó mới nên sang **Quest System hoàn chỉnh**, vì Quest sẽ là lớp “expose Story Thread cho player” dựa trên tất cả các hệ thống vừa hoàn thành.

<!-- SOURCE T037 END -->

