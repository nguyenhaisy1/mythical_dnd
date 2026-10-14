# Module 06A — Core Life Event Library — nguồn mang tên Module 05A

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T017](../../source/conversation.md#t017)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Giữ nguyên thư viện 05A của nguồn, chuyển thành phụ lục 06A cho đúng nhóm Life Event. Các mẫu văn bản phải được chuyển thành definition có validation trước khi load runtime.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Event definitions, actor roles, trigger, causal conditions và resources. |
| Đầu ra / bên sở hữu | Dữ liệu authoring; runtime chỉ chấp nhận approved rule references. |
| Đọc cùng | 06, 17, 24, 27; content guide.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Template thiếu actor/reference/resource không được kích hoạt thành event thật. |
| Việc chưa được chứng minh | Library này chưa phải pack JSON production và chưa vượt causal simulation tests. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T017 BEGIN -->

Tôi sẽ dùng cấu trúc này làm **bộ event lõi v1.0**. Nó đủ cụ thể để sau này chuyển thành bảng dữ liệu/JSON mà không phải thiết kế lại logic.

<a id="m06a-t017-s001"></a>

## GDD MODULE 05A — CORE LIFE EVENT LIBRARY v1.0

<a id="m06a-t017-s002"></a>

### 1. Mục tiêu

Core Life Event Library là tập hợp các biến cố nguyên tử mà Narrative Simulation Engine sử dụng để tạo:

- quá khứ nhân vật;
- biến động gia đình;
- lịch sử settlement;
- quan hệ xã hội;
- character arc;
- story thread;
- side quest;
- faction conflict;
- hậu quả dài hạn;
- lịch sử riêng của từng playthrough.

Event Library **không trực tiếp viết quest**.

Nó tạo ra:

> Cause → Event → Consequence → Unresolved State → Story Thread.

Quest chỉ xuất hiện khi người chơi có cơ hội nhận biết và can thiệp vào Story Thread.

---

<a id="m06a-t017-s003"></a>

## 2. Chuẩn dữ liệu của một Event Definition

Mỗi event production-level phải có cấu trúc:

```text
event_id
event_name
category
scope

required_conditions
forbidden_conditions

drivers
trigger

participant_roles
participant_selection_rules

severity_range
visibility
urgency
persistence

objective_outcomes
state_changes

psychological_interpretations
relationship_effects

memory_templates
knowledge_distribution
rumor_templates

unresolved_states
follow_up_events

quest_exposure
historical_trace
```

---

<a id="m06a-t017-s004"></a>

## 3. Quy ước State Delta

Trong tài liệu này sử dụng notation:

```text
+   tăng nhẹ
++  tăng đáng kể
+++ tăng rất mạnh

-   giảm nhẹ
--  giảm đáng kể
--- giảm rất mạnh
```

Ví dụ:

```text
Household.Wealth --
Character.Trust -
Settlement.Food ---
Relationship.Obligation ++
Character.Guilt +++
```

Không coi đây là số balancing cuối cùng.

---

<a id="m06a-t017-s005"></a>

## 4. Luật chung

Event không được xảy ra chỉ vì RNG.

RNG chỉ lựa chọn giữa **những event đã đủ điều kiện causal**.

Ví dụ:

`E044_MAJOR_FLOOD`

không eligible nếu settlement:

- không gần hệ thống nước;
- đang mùa khô;
- không có nguồn nước thượng lưu;
- không có supernatural anomaly liên quan nước.

---

<a id="m06a-t017-s006"></a>

## 5. Event Severity

Mỗi event có Severity từ 1–5.

| Severity | Ý nghĩa |
|---|---|
| 1 | biến cố đời thường |
| 2 | ảnh hưởng cá nhân rõ rệt |
| 3 | character-defining |
| 4 | ảnh hưởng household/settlement lớn |
| 5 | biến cố lịch sử |

Một event definition có thể sinh nhiều severity khác nhau.

---

<a id="m06a-t017-s007"></a>

## A. BIRTH & CHILDHOOD EVENTS

<a id="m06a-t017-s008"></a>

### E001 — NORMAL BIRTH

**Scope:** Individual / Household.

**Precondition:** Character chưa tồn tại; household có cặp cha/mẹ hoặc cấu trúc xã hội phù hợp để sinh con.

**Trigger:** Pregnancy cycle hoàn tất.

**Actors:** Mother, child, partner, household members.

**Primary outcome:** Character mới được instantiate hoặc tạo Population Record.

**State changes:** Household.Size +; FoodDemand +; ParentAttachment +.

**Psychology:** Không áp trực tiếp lên child; tạo Birth Context phục vụ personality sau này.

**Possible modifiers:** mùa đói, chiến tranh, di cư, lễ hội, cha vắng mặt, mẹ đơn thân.

**Memory:** cha/mẹ có thể lưu BirthMemory nếu Importance đủ lớn.

**Follow-up:** E002 Difficult Birth, E005 Parent Absent, E012 New Sibling Pressure.

---

<a id="m06a-t017-s009"></a>

### E002 — DIFFICULT BIRTH

**Precondition:** E001 active; health risk, tuổi mẹ, thiếu healer hoặc environmental stress.

**Trigger:** Birth Risk vượt threshold.

**Actors:** Mother, child, healer, household.

**Outcomes:**

A. Cả mẹ và con sống.

B. Mẹ bị hậu quả sức khỏe.

C. Child bị hậu quả sức khỏe.

D. Mother dies.

E. Child dies.

**State:** Household.Health --; Wealth có thể giảm vì treatment; FamilyStress ++.

**Psychology:** Partner Fear++, Guilt hoặc Spirituality có thể tăng tùy interpretation.

**Unresolved states:** MedicalDebt, MaternalLoss, ChildFragility.

**Quest exposure:** healer shortage, tìm dược liệu, mời specialist.

---

<a id="m06a-t017-s010"></a>

### E003 — TWIN BIRTH

**Precondition:** Birth event.

**Outcome:** hai child cùng household.

Không mặc định relationship tốt.

**Special generation rule:** hai trẻ chia sẻ:

- Birth Year;
- Parents;
- early Household State.

Nhưng **không được clone psychology**.

**Pressure:** Household.FoodDemand ++; ParentalAttention bị chia.

**Possible future hooks:** sibling rivalry, protective bond, identity confusion, succession conflict.

---

<a id="m06a-t017-s011"></a>

### E004 — ADOPTION

**Scope:** Individual / Household.

**Precondition:** child cần guardian AND household có capacity hoặc obligation phù hợp.

Nguồn child có thể là:

- orphan;
- abandoned;
- war refugee;
- child của relative chết;
- political hostage được nuôi;
- cultural exchange.

**Actors:** child, adopter, biological relations nếu tồn tại.

**State:** Household.Size +; SocialObligation ++.

**Psychology:** Belonging, AbandonmentFear hoặc Loyalty có thể phát triển.

**Important:** biological lineage không bị xóa khỏi World Graph.

**Future hooks:** identity discovery, inheritance, cultural conflict, biological family returns.

---

<a id="m06a-t017-s012"></a>

### E005 — PARENT ABSENT DURING CHILDHOOD

**Precondition:** child age 0–12.

Parent sống nhưng vắng mặt vì:

war;

trade;

migration;

imprisonment;

religious duty;

work.

**State:** ParentChild.Contact --.

**Interpretation options:**

“Cha đang bảo vệ chúng ta.”

“Cha đã bỏ chúng ta.”

“Cha là người vĩ đại.”

“Cha không cần ta.”

Được chọn dựa trên lời kể của household và personality seed.

**Future:** reunion không tự động tạo relationship tốt.

---

<a id="m06a-t017-s013"></a>

### E006 — CHILD SERIOUS ILLNESS

**Scope:** Individual.

**Drivers:** Health Pressure, disease environment, malnutrition.

**Outcome:** recover / permanent weakness / chronic condition / death.

**Psychology:** nếu sống sót có thể sinh:

RiskAversion +;

Spirituality +;

SelfPreservation +;

hoặc Recklessness vì “từng thoát chết”.

**Relationship:** caregiver → child Obligation/Attachment tăng.

**Quest:** medicine search, healer, taboo treatment.

---

<a id="m06a-t017-s014"></a>

### E007 — CHILD LOST

Child rời khỏi protection network trong thời gian dài.

**Possible cause:** rừng, lũ, festival, migration, kidnapping false-positive.

**Resolution:**

found quickly;

survives alone;

rescued by stranger;

reaches another settlement;

never found.

**Memory importance:** cao.

**Passive Seed:** Navigation, Hypervigilance, Fear of Isolation.

**Narrative Debt:** nếu không tìm thấy → MissingPerson persistent.

---

<a id="m06a-t017-s015"></a>

### E008 — CHILD WITNESSES DEATH

**Target:** parent, sibling, stranger, mentor, animal, enemy.

**Psychological calculation:**

```text
Trauma =
RelationshipWeight
× Violence
× Helplessness
× AgeSensitivity
```

Possible interpretation:

“I should have saved them.”

“Death is normal.”

“Violence solves things.”

“Never depend on anyone.”

**Follow-up:** revenge, avoidance, healer path, warrior path, spiritual obsession.

---

<a id="m06a-t017-s016"></a>

### E009 — CHILD SAVES SOMEONE

**Scope:** Individual / Relationship.

Child thực hiện hành động có hậu quả thật.

Không nhất thiết anh hùng:

- kéo người khỏi nước;
- báo cháy;
- tìm healer;
- cảnh báo thú dữ.

**Effects:** Recognition +; Confidence + hoặc Fear +.

Nếu nạn nhân quan trọng:

SocialObligation tạo lifelong edge.

**Passive Seed:** Rescue-related competence.

---

<a id="m06a-t017-s017"></a>

### E010 — CHILD COMMITS SERIOUS MISTAKE

Ví dụ:

làm cháy kho;

làm mất gia súc;

dẫn em vào rừng;

làm chìm thuyền;

tiết lộ secret.

**Outcome phụ thuộc:** bị phát hiện hay không.

Nếu giấu được:

Secret + Guilt.

Nếu bị phát hiện:

Shame + Reputation.

Nếu người khác nhận tội:

Complex Obligation Thread.

Đây là event rất mạnh để tạo contradiction.

---

<a id="m06a-t017-s018"></a>

## B. FAMILY & HOUSEHOLD EVENTS

<a id="m06a-t017-s019"></a>

### E011 — SIBLING BIRTH

Child hiện tại nhận sibling mới.

**Effect phụ thuộc:** BirthOrder, Wealth, ParentalAttention.

Có thể tạo:

Protectiveness;

Jealousy;

Fear of Replacement;

Identity as Elder Sibling.

Không cố định.

---

<a id="m06a-t017-s020"></a>

### E012 — HOUSEHOLD RESOURCE SHORTAGE

**Trigger:** Food/Wealth thấp hơn HouseholdNeed.

Không phải famine toàn settlement.

Possible responses:

giảm khẩu phần;

vay;

bán tài sản;

cho con đi học việc;

di cư;

trộm;

xin họ hàng giúp.

Event này chủ yếu **sinh event khác**.

---

<a id="m06a-t017-s021"></a>

### E013 — FAMILY DEBT CREATED

**Actors:** debtor household, creditor person/family.

Record bắt buộc:

amount/resource;

reason;

repayment expectation;

witness;

social legitimacy.

Debt có thể là:

grain;

metal;

boat;

labor;

favor.

**Relationship:** Obligation ++.

**Future:** repayment, restructuring, conflict, inheritance.

---

<a id="m06a-t017-s022"></a>

### E014 — DEBT DEFAULT

**Precondition:** E013 tồn tại và deadline vượt qua.

Creditor reaction dựa trên:

Compassion;

Wealth;

Relationship;

Reputation;

PoliticalPower.

Possible outcomes:

extend;

take asset;

demand labor;

arrange marriage;

public shame;

violence.

**Important:** Debt Default là một trong các generator mạnh nhất của social drama.

---

<a id="m06a-t017-s023"></a>

### E015 — INHERITANCE TRANSFER

Triggered bởi death, retirement hoặc voluntary transfer.

Inheritance có thể gồm:

land;

boat;

tools;

position;

trade secret;

debt;

obligation;

reputation.

**Conflict eligibility:** nhiều claimant + unclear rules.

Có thể sinh E016.

---

<a id="m06a-t017-s024"></a>

### E016 — DISPUTED INHERITANCE

**Participants:** claimant ≥2.

Conflict object phải cụ thể.

Không sinh generic “tranh tài sản”.

Ví dụ:

chiếc thuyền duy nhất;

quyền dùng ruộng;

bí quyết nghề;

vị trí ritual keeper.

**Outcomes:** agreement, split, elder arbitration, violence, permanent estrangement.

---

<a id="m06a-t017-s025"></a>

### E017 — HOUSEHOLD MEMBER LEAVES

Reason:

marriage;

career;

conflict;

migration;

war;

religious calling;

desire for independence.

**State:** household labor -, food demand -, emotional cohesion có thể + hoặc -.

**Memory:** khác biệt giữa “left willingly” và “was driven away”.

---

<a id="m06a-t017-s026"></a>

### E018 — FAMILY RECONCILIATION

Không eligible nếu không tồn tại conflict trước đó.

**Precondition:** relationship resentment > threshold.

Trigger thường là:

shared crisis;

elder intervention;

death;

apology;

player mediation.

Không reset relationship.

Chỉ tạo:

Resentment--;

Trust +;

New Memory.

Old memory vẫn tồn tại.

---

<a id="m06a-t017-s027"></a>

### E019 — FAMILY SECRET DISCOVERED

Character biết information trước đây ngoài Knowledge Graph của họ.

Secret categories:

parentage;

crime;

debt;

betrayal;

forbidden relationship;

political allegiance;

religious act.

**Result:** reaction phụ thuộc personality.

Không tự động confront.

Có thể:

hide;

investigate;

blackmail;

forgive;

leave.

---

<a id="m06a-t017-s028"></a>

### E020 — FAMILY SPLIT

Household hoặc lineage branch tách thành hai entity.

**Required:** persistent high conflict hoặc resource/geography pressure.

Creates:

Household A;

Household B;

new ownership;

new obligations.

Đây là event quan trọng để world tạo dòng họ mới theo thời gian.

---

<a id="m06a-t017-s029"></a>

## C. RELATIONSHIP & MARRIAGE EVENTS

<a id="m06a-t017-s030"></a>

### E021 — FIRST MEETING

Không cần lưu mọi lần NPC gặp nhau.

Chỉ tạo persistent relationship nếu:

emotional weight;

shared context;

future proximity;

social importance

đủ threshold.

Initial impression dựa trên:

reputation;

culture;

appearance;

context;

personality.

---

<a id="m06a-t017-s031"></a>

### E022 — FAVOR GIVEN

A làm điều có lợi đáng kể cho B.

Creates:

B→A Obligation +.

Nhưng Affection không bắt buộc tăng.

Một người có thể:

“nợ anh, nhưng vẫn ghét anh.”

---

<a id="m06a-t017-s032"></a>

### E023 — SHARED DANGER

Hai character cùng sống sót sự kiện nguy hiểm.

Possible effects:

Trust ++;

Attachment ++;

hoặc Blame ++ nếu một người gây nguy hiểm.

Shared danger là friendship accelerator nhưng không đảm bảo friendship.

---

<a id="m06a-t017-s033"></a>

### E024 — SECRET SHARED

A tự nguyện cho B biết secret.

**Effects:** Trust++, Vulnerability++.

Nếu B tiết lộ:

betrayal multiplier rất cao.

Secret importance xác định severity tương lai.

---

<a id="m06a-t017-s034"></a>

### E025 — PROMISE MADE

Promise trở thành object trong Narrative Graph.

Fields:

giver;

receiver;

content;

deadline;

conditions;

importance.

Không chỉ là dialogue flavor.

---

<a id="m06a-t017-s035"></a>

### E026 — PROMISE BROKEN

Required E025.

Interpretation phụ thuộc:

reason known?

intentional?

forced?

Rumor?

Possible effects:

Trust---;

Resentment++;

Shame/Guilt.

Có thể truyền thành family reputation.

---

<a id="m06a-t017-s036"></a>

### E027 — ROMANTIC ATTRACTION

Không đồng nghĩa romance.

Tạo Attraction edge.

Drivers:

personality compatibility;

appearance;

shared values;

novelty;

status;

shared experience.

Cultural restrictions có thể tạo Pressure.

---

<a id="m06a-t017-s037"></a>

### E028 — ROMANTIC RELATIONSHIP

Requires mutual attraction hoặc một social arrangement dẫn tới emotional development.

Creates persistent intimate edge.

Potential conflicts:

family;

culture;

distance;

class;

politics;

duty.

---

<a id="m06a-t017-s038"></a>

### E029 — MARRIAGE / HOUSEHOLD UNION

Tạo hoặc thay đổi Household Graph.

Effects:

resource merge;

family alliance;

new cultural exposure;

potential migration.

Marriage Motivation lưu riêng:

love;

alliance;

economy;

family duty;

reconciliation.

Motive ảnh hưởng future satisfaction.

---

<a id="m06a-t017-s039"></a>

### E030 — RELATIONSHIP BREAKDOWN

Dùng cho spouse, friendship hoặc close bond.

Không trigger vì một argument nhỏ.

Requires cumulative:

resentment;

betrayal;

distance;

value conflict.

Outcome:

temporary separation;

permanent estrangement;

public conflict;

silent coexistence.

---

<a id="m06a-t017-s040"></a>

## D. PROFESSION, EDUCATION & ECONOMY

<a id="m06a-t017-s041"></a>

### E031 — FIRST WORK

Character lần đầu tham gia household economy.

Tuổi và culture quyết định hợp lệ.

Adds:

OccupationExposure.

Không tự động thành profession.

---

<a id="m06a-t017-s042"></a>

### E032 — APPRENTICESHIP

Requires:

mentor/master;

available skill;

social access.

Creates Mentor relationship.

Skill progression tăng.

Household labor có thể giảm.

Có thể kèm payment/debt/favor.

---

<a id="m06a-t017-s043"></a>

### E033 — APPRENTICESHIP FAILURE

Reasons:

lack ability;

discipline;

mentor conflict;

injury;

family need;

politics.

Không nhất thiết character “kém”.

Có thể mở career hoàn toàn khác.

---

<a id="m06a-t017-s044"></a>

### E034 — MASTER RECOGNITION

Mentor công nhận apprentice đủ năng lực.

Effects:

ProfessionRank +;

Reputation +;

independence.

Có thể mở ability nhận apprentice.

---

<a id="m06a-t017-s045"></a>

### E035 — PROFESSIONAL INJURY

Injury gắn đúng nghề.

Thợ mỏ → sập hầm.

Thợ đúc → bỏng.

Ngư dân → tai nạn nước.

Hunter → animal wound.

Outcome:

temporary;

permanent impairment;

career change;

death.

Passive đôi khi sinh từ adaptation sau injury.

---

<a id="m06a-t017-s046"></a>

### E036 — PROFESSIONAL BREAKTHROUGH

Character phát hiện technique hoặc giải quyết challenge quan trọng.

Không tự động technology revolution.

Effect:

Expertise++;

Recognition++;

Possible TradeSecret.

---

<a id="m06a-t017-s047"></a>

### E037 — TRADE SECRET LEARNED

Knowledge Object chuyển vào Character Knowledge.

Source:

mentor;

family;

discovery;

stolen.

Tag:

legitimate / illegitimate.

Secret ownership có thể tạo conflict sau này.

---

<a id="m06a-t017-s048"></a>

### E038 — TRADE SECRET STOLEN

A cố lấy professional knowledge từ B.

Có thể thành công mà B chưa biết.

Creates Hidden Crime Thread.

Nếu discovered:

ProfessionalRivalry;

FamilyConflict;

Faction involvement.

---

<a id="m06a-t017-s049"></a>

### E039 — MAJOR ECONOMIC LOSS

Loss:

boat;

crop;

livestock;

tools;

cargo;

workshop.

Required physical/social cause.

Effects:

Wealth --/---.

Follow-up:

debt;

career shift;

migration;

crime;

family pressure.

---

<a id="m06a-t017-s050"></a>

### E040 — ECONOMIC SUCCESS

Không chỉ tragedy.

Sources:

good harvest;

profitable voyage;

successful craft;

trade opportunity.

Effects:

Wealth++;

Prestige có thể +.

Possible new problems:

jealousy;

inheritance;

political attention;

robbery target.

---

<a id="m06a-t017-s051"></a>

## E. ENVIRONMENT, HEALTH & MIGRATION

<a id="m06a-t017-s052"></a>

### E041 — LOCAL CROP FAILURE

Scope household/field.

Cause:

weather;

pest;

seed;

water management;

human sabotage.

Creates FoodPressure nhưng không nhất thiết famine.

---

<a id="m06a-t017-s053"></a>

### E042 — SETTLEMENT BAD HARVEST

Nhiều household bị ảnh hưởng.

FoodPressure ++.

FoodPrice ++.

WealthPoorHouseholds --.

Follow-up probability tăng cho:

Debt;

Theft;

Migration;

PoliticalDiscontent.

---

<a id="m06a-t017-s054"></a>

### E043 — DROUGHT

Requires seasonal/geographic validity.

Effects:

Water--;

Crop--;

Disease có thể +;

InterSettlementConflict +.

Can expose ancient riverbeds, graves hoặc routes.

---

<a id="m06a-t017-s055"></a>

### E044 — MAJOR FLOOD

Requires water geography.

Possible causes:

heavy rain;

upstream change;

dam/earthwork;

deforestation;

Mạch anomaly.

Outcome parameters:

deaths;

missing;

crop loss;

house loss;

boat loss;

sediment benefit.

Lũ có thể vừa phá nơi này vừa làm đất nơi khác tốt lên.

---

<a id="m06a-t017-s056"></a>

### E045 — FIRE

Targets:

household;

workshop;

warehouse;

field;

forest.

Cause lưu cụ thể.

Fire thường tạo Investigation nếu ignition không rõ.

---

<a id="m06a-t017-s057"></a>

### E046 — DISEASE OUTBREAK

Scope settlement.

Disease phải có:

transmission;

symptoms;

severity;

incubation.

Không dùng generic plague.

Creates:

HealthPressure;

Fear;

Quarantine-like social response;

HealerDemand.

Rumor có thể sai về nguyên nhân.

---

<a id="m06a-t017-s058"></a>

### E047 — SERIOUS ADULT ILLNESS

Character-level.

Can trigger:

succession planning;

debt;

family return;

faith change;

career ending.

Illness là narrative driver chứ không chỉ debuff.

---

<a id="m06a-t017-s059"></a>

### E048 — DISABILITY / PERMANENT INJURY

Không coi là “character ruined”.

Engine phải chạy Adaptation Phase.

Possible results:

occupation shift;

new passive;

new dependency;

new expertise.

Sương Điền là ví dụ.

---

<a id="m06a-t017-s060"></a>

### E049 — HOUSEHOLD MIGRATION

Requires push hoặc pull factor.

Push:

food;

war;

debt;

conflict;

environment.

Pull:

marriage;

work;

land;

safety.

Migration không xóa cultural origin.

Creates:

OriginCulture + DestinationCulture.

---

<a id="m06a-t017-s061"></a>

### E050 — MASS DISPLACEMENT

Settlement hoặc region level.

Cause:

war;

flood;

political expulsion;

environmental collapse.

Creates Refugee Population Records.

Downstream effects:

housing pressure;

xenophobia;

labor increase;

mixed marriage;

new settlements.

---

<a id="m06a-t017-s062"></a>

## F. CRIME, SECRETS & SOCIAL CONFLICT

<a id="m06a-t017-s063"></a>

### E051 — FOOD THEFT

Requires need hoặc criminal motivation.

Target phải tồn tại.

If offender poor + FoodPressure high:

community response khác professional thief.

Creates Investigation nếu discovered.

---

<a id="m06a-t017-s064"></a>

### E052 — PROPERTY THEFT

Targets:

tool;

weapon;

cargo;

ritual item;

animal.

Motive:

profit;

revenge;

need;

politics.

Loot phải được chuyển ownership chứ không biến mất khỏi world.

---

<a id="m06a-t017-s065"></a>

### E053 — SMUGGLING

Requires restricted/taxed/politically sensitive goods.

Creates route, sender, carrier, receiver.

Không thể có smuggling không có logistics.

Goods:

metal;

weapons;

food;

ritual artifact;

information.

---

<a id="m06a-t017-s066"></a>

### E054 — ASSAULT

Cause:

personal conflict;

crime;

politics;

intoxication;

revenge.

Outcome:

injury;

death possible;

public knowledge;

family retaliation.

---

<a id="m06a-t017-s067"></a>

### E055 — ACCIDENTAL KILLING

Character gây death nhưng không có intent.

Creates:

Guilt;

Legal/Social conflict;

cover-up possibility.

Phản ứng character đặc biệt quan trọng.

---

<a id="m06a-t017-s068"></a>

### E056 — INTENTIONAL MURDER

Requires strong motive.

Motive object bắt buộc:

revenge;

power;

silencing;

profit;

fear;

ideology.

No motive → event invalid.

Creates Crime Thread ngay cả khi không discovered.

---

<a id="m06a-t017-s069"></a>

### E057 — BODY DISCOVERED

Tách khỏi murder/death.

Body có thể được phát hiện:

immediately;

days;

years later.

Creates information event.

Discovery context quyết định evidence còn lại.

---

<a id="m06a-t017-s070"></a>

### E058 — FALSE ACCUSATION

A accuses B of act B did not commit.

Accuser có thể:

lie knowingly;

be genuinely mistaken;

be manipulated.

Ba case tạo psychology hoàn toàn khác.

---

<a id="m06a-t017-s071"></a>

### E059 — PUBLIC SHAME

Community aware of behavior violating cultural norm.

Effects:

Reputation--;

Shame++;

Migration probability +;

FamilyPressure +.

Không phải culture nào cũng shame cùng hành động.

---

<a id="m06a-t017-s072"></a>

### E060 — REVENGE ACT

Requires stored grievance.

Target selection dựa trên perceived blame, không nhất thiết objective guilt.

Outcome có thể tạo:

new grievance.

Đây là nguồn sinh feud cycle.

Engine phải có escalation limiter để tránh revenge vô hạn.

---

<a id="m06a-t017-s073"></a>

## G. POLITICS, FACTION & WAR

<a id="m06a-t017-s074"></a>

### E061 — LEADER DIES

Leader scope:

household;

lineage;

settlement;

faction.

Creates LeadershipVacancy nếu không có succession rõ.

---

<a id="m06a-t017-s075"></a>

### E062 — LEADERSHIP SUCCESSION

Can be:

inheritance;

elder consensus;

appointment;

force;

ritual recognition.

Creates Legitimacy value.

Low legitimacy → E063.

---

<a id="m06a-t017-s076"></a>

### E063 — DISPUTED LEADERSHIP

≥2 claimant hoặc legitimacy thấp.

Possible resolution:

negotiation;

shared power;

withdrawal;

duel;

assassination;

civil conflict.

---

<a id="m06a-t017-s077"></a>

### E064 — POLITICAL ALLIANCE

Entities:

families;

settlements;

factions.

Requires shared interest hoặc common enemy.

Alliance stores:

purpose;

duration expectation;

obligations.

Không phải permanent friendship.

---

<a id="m06a-t017-s078"></a>

### E065 — ALLIANCE BROKEN

Reason:

goal achieved;

betrayal;

leadership change;

resource conflict;

rumor.

Creates Reputation consequences.

---

<a id="m06a-t017-s079"></a>

### E066 — TRIBUTE / OBLIGATION REFUSED

Entity từ chối resource, military support hoặc ritual obligation.

Requires political relationship trước đó.

Đây là escalation seed rất quan trọng cho Văn Lang fragmentation.

---

<a id="m06a-t017-s080"></a>

### E067 — MILITARY MOBILIZATION

Không phải battle.

Requires:

threat;

leadership decision;

available manpower;

supply.

Effects:

Labor--;

FoodDemand++;

FamilySeparation++;

Security có thể +.

---

<a id="m06a-t017-s081"></a>

### E068 — BATTLE

Requires forces + location + objective.

Battle Outcome dựa trên:

numbers;

morale;

leadership;

terrain;

supply;

information;

technology.

Không resolve thuần RNG.

Generates:

death;

injury;

capture;

desertion;

hero memory;

refugee.

---

<a id="m06a-t017-s082"></a>

### E069 — DESERTION

Requires mobilized fighter.

Drivers:

fear;

low morale;

family crisis;

ideology;

hunger;

leadership distrust.

Deserter không mặc định coward.

Can become:

refugee;

bandit;

farmer;

informant.

---

<a id="m06a-t017-s083"></a>

### E070 — POLITICAL REGIME CHANGE

Scope settlement/region/world.

Examples:

new chief;

Phong Châu loses control;

Âu Lạc administration replaces old structure.

Must update:

authority;

laws/custom;

obligations;

faction ownership;

NPC loyalties.

Không được chỉ thay banner.

---

<a id="m06a-t017-s084"></a>

## H. SPIRITUAL, MYTHIC & MẠCH LẠC EVENTS

<a id="m06a-t017-s085"></a>

### E071 — TABOO VIOLATED

Requires culture-specific taboo.

Actor có thể:

biết;

không biết;

bị ép.

Effects ban đầu là **social**, không tự động supernatural.

Possible:

Reputation--;

Fear++;

RitualDemand+.

Supernatural follow-up chỉ eligible nếu điều kiện Mạch phù hợp.

---

<a id="m06a-t017-s086"></a>

### E072 — RITUAL PERFORMED

Requires ritual knowledge + participants + resources.

Ritual Outcome có ba lớp:

Social Outcome.

Psychological Outcome.

Possible Mạch Outcome.

Không phải ritual nào cũng magic.

---

<a id="m06a-t017-s087"></a>

### E073 — RITUAL FAILURE

Failure types:

technical mistake;

wrong participant;

missing item;

intentional sabotage;

Mạch rejection;

misinterpretation.

Creates investigation potential.

---

<a id="m06a-t017-s088"></a>

### E074 — SACRED SITE DISTURBED

Examples:

tree felled;

grave opened;

stone removed;

stream diverted;

shrine damaged.

Effects:

CulturalConflict++;

SpiritualPressure+.

MạchEffect conditional.

---

<a id="m06a-t017-s089"></a>

### E075 — OMEN OBSERVED

Phenomenon:

animal behavior;

weather;

sound;

dream;

celestial sign;

ritual anomaly.

**Important:** Omen Object không có sẵn meaning tuyệt đối.

Different NPCs interpret differently.

Event tạo **Interpretation Conflict**.

---

<a id="m06a-t017-s090"></a>

### E076 — SHARED DREAM

≥2 characters báo cáo dream tương đồng.

Eligibility:

high Mạch instability;

ritual;

shared trauma;

or unknown.

Possible truth:

supernatural;

suggestion;

coincidence;

human manipulation.

Không nhất thiết resolve.

---

<a id="m06a-t017-s091"></a>

### E077 — SPIRITUAL ENCOUNTER

Character perceives entity/phenomenon.

Engine lưu:

ObjectiveEntityExists?

PerceptionReliability.

WitnessCount.

Interaction.

Character có thể tin hoặc không tin chính trải nghiệm của mình.

---

<a id="m06a-t017-s092"></a>

### E078 — MẠCH DISTURBANCE

Scope location/settlement.

Causes:

mass death;

sacred destruction;

ancient object;

broken oath;

major environmental change.

Effects không nên chỉ là monster spawn.

Có thể:

dream patterns;

animal migration;

ritual anomalies;

navigation oddities;

water change;

memory phenomena.

---

<a id="m06a-t017-s093"></a>

### E079 — ANCIENT OATH REACTIVATED

Old Narrative Debt quay lại.

Requires historical event:

promise;

treaty;

ritual obligation;

family oath.

Trigger:

descendant action;

location disturbance;

political change.

Đây là event cực quan trọng cho **Ước Sơn Hà**.

---

<a id="m06a-t017-s094"></a>

### E080 — MYTH FORMATION

Đây là meta-event đặc biệt.

Không phải supernatural event.

Requires:

high-importance historical event

+

multiple retellings

+

time passage.

Objective Event được chuyển thành một hoặc nhiều oral narratives.

Ví dụ:

thợ + kỹ thuật + chiến tranh

qua nhiều thế hệ

→ “nỏ thần”.

Event lưu:

core fact;

lost facts;

added symbolism;

regional variants.

Đây là engine kết nối **game history → Vietnamese-style legend**.

---

<a id="m06a-t017-s095"></a>

## 6. EVENT DEPENDENCY RULE

80 event trên không tồn tại ngang hàng.

Chúng tạo dependency.

Ví dụ:

```text
E042 Bad Harvest
    ↓
E012 Resource Shortage
    ↓
E013 Debt
    ↓
E014 Default
    ↓
E017 Child Leaves Household
    ↓
E032 Apprenticeship
    ↓
E021 First Meeting
    ↓
E027 Attraction
    ↓
E029 Marriage
    ↓
E049 Migration
```

Một story có thể hình thành mà **không writer nào chỉ định “hãy tạo romance quest”.**

---

<a id="m06a-t017-s096"></a>

## 7. EVENT CHAIN CLASSIFICATION

Để Narrative Director kiểm soát chất lượng, chain được phân thành 5 nhóm.

<a id="m06a-t017-s097"></a>

### Survival Chain

Ví dụ:

Drought

→ Crop Failure

→ Debt

→ Migration.

---

<a id="m06a-t017-s098"></a>

### Relationship Chain

Secret Shared

→ Promise

→ Promise Broken

→ Estrangement

→ Reconciliation hoặc Revenge.

---

<a id="m06a-t017-s099"></a>

### Career Chain

Apprenticeship

→ Recognition

→ Trade Secret

→ Innovation

→ Rivalry.

---

<a id="m06a-t017-s100"></a>

### Political Chain

Leader Death

→ Succession

→ Dispute

→ Alliance

→ Mobilization

→ Battle.

---

<a id="m06a-t017-s101"></a>

### Mythic Chain

Sacred Site Disturbed

→ Mạch Disturbance

→ Omen

→ Shared Dream

→ Spiritual Encounter

→ Myth Formation.

---

<a id="m06a-t017-s102"></a>

## 8. CROSS-CHAIN MERGING

Điểm quan trọng nhất của system là các chain được phép va vào nhau.

Ví dụ:

<a id="m06a-t017-s103"></a>

#### Survival

Bad Harvest

→ Debt.

<a id="m06a-t017-s104"></a>

#### Relationship

Daughter in Love.

<a id="m06a-t017-s105"></a>

#### Political

Creditor Family supports opposing faction.

Ba chain merge:

> gia đình creditor đề nghị xóa nợ nếu con gái kết hôn với con trai họ.

Một event kinh tế biến thành:

family drama

+

romance

+

political alliance.

Đây là cấp độ story chúng ta muốn.

---

<a id="m06a-t017-s106"></a>

## 9. EVENT BLACKLIST RULES

Một số combination phải bị cấm hoặc giảm mạnh.

Ví dụ NPC vừa:

mất cha;

mất mẹ;

mất vợ;

mất ba người con

trong 18 tháng

sẽ bị Narrative Saturation rất cao.

Event death tiếp theo bị suppression trừ:

war;

epidemic;

major disaster.

Tương tự một settlement vừa trải qua battle lớn sẽ giảm weight của:

murder mystery;

major cult uprising;

another war

trong một khoảng Recovery Window.

---

<a id="m06a-t017-s107"></a>

## 10. EVENT COOLDOWN

Mỗi event family có cooldown.

Ví dụ:

Major Flood:

5–20 năm tùy geography.

Political Regime Change:

nhiều năm.

Food Theft:

có thể nhiều lần.

Marriage:

theo individual constraint.

Cooldown không phải absolute nếu causal pressure cực mạnh.

---

<a id="m06a-t017-s108"></a>

## 11. CHARACTER LIFE EVENT BUDGET

Một Tier A character không cần 40 biến cố lớn.

Biography tốt hơn nếu có:

**Childhood**

2–4 meaningful events.

**Adolescence**

2–4.

**Early adulthood**

3–6.

**Later life**

tùy tuổi.

Mục tiêu:

khoảng **6–12 defining events**.

Các năm bình thường được compression.

---

<a id="m06a-t017-s109"></a>

## 12. FORMATIVE EVENT

Một event trở thành `Formative` nếu:

```text
EmotionalWeight
× AgeSensitivity
× IdentityImpact
× Persistence
```

vượt threshold.

Formative Event có quyền:

thay đổi personality;

tạo Fear;

tạo Value;

tạo Contradiction;

tạo Passive Seed.

Không phải mọi memory làm thay đổi character.

---

<a id="m06a-t017-s110"></a>

## 13. PASSIVE SEED EXTRACTION

Ví dụ:

```text
E044 Major Flood
+
E008 Witness Parent Death
+
E032 Apprenticeship: Boatman
+
High Persistence
+
Low Spirituality
```

→ Passive Seed:

`PhysicalWaterObservation`

Sau refinement:

**Nước Không Nói Dối.**

---

<a id="m06a-t017-s111"></a>

## 14. STORY THREAD CREATION RULE

Không phải mọi event mở Story Thread.

Thread chỉ được tạo nếu có ít nhất một:

```text
Unresolved State
High Emotional Weight
Relationship Conflict
Resource Conflict
Hidden Truth
Faction Conflict
Historical Significance
```

Ví dụ:

Normal Birth

không tạo thread.

Nhưng:

Difficult Birth → mother dies → father blames healer

có thể tạo thread.

---

<a id="m06a-t017-s112"></a>

## 15. UNRESOLVED STATE LIBRARY

Các dạng Narrative Debt cốt lõi nên có:

```text
UnpaidDebt
MissingPerson
MissingBody
BrokenPromise
UnknownParentage
HiddenCrime
UnsolvedDeath
DisputedLand
DisputedInheritance
ForbiddenRelationship
UnreturnedFavor
UnfulfilledOath
LostArtifact
UnresolvedBlame
LeaderWithoutLegitimacy
UnavengedDeath
UnfinishedCraft
UnpaidTribute
DisplacedHousehold
ViolatedTaboo
DisturbedSacredSite
UnexplainedPhenomenon
```

Event engine ưu tiên quan sát những state này khi tạo follow-up.

---

<a id="m06a-t017-s113"></a>

## 16. EXAMPLE — COMPLETE GENERATED THREAD

Settlement:

**Làng Dâu.**

Năm 3:

`E042 Bad Harvest`

↓

Nhà Tạ thiếu lương.

↓

`E012 Household Resource Shortage`

↓

`E013 Family Debt`

vay nhà Nậu.

↓

Năm 4:

vẫn khó khăn.

↓

`E014 Debt Default`

↓

Con trai Tạ Cơ phải rời nhà.

↓

`E017 Household Member Leaves`

↓

`E032 Apprenticeship`

tại Bến Hạc.

↓

Ở đó Cơ gặp Hạc Di.

↓

`E021 First Meeting`

↓

Cùng sống sót vụ cháy kho.

↓

`E023 Shared Danger`

↓

`E027 Attraction`

↓

Năm 7:

hai người muốn kết hôn.

↓

`E029 Marriage` eligible.

Nhưng nhà Hạc phát hiện:

Nhà Tạ đang nợ nhà Nậu, trong khi nhà Nậu là political rival của nhà Hạc.

Marriage bị phản đối.

↓

Story Threads:

`FamilyDebt_TaNau`

+

`Romance_CoDi`

+

`PoliticalRivalry_NauHac`

MERGE.

Bây giờ player xuất hiện.

Quest journal không nên nói:

> “Giúp Cơ cưới Di.”

Thay vào đó player có thể biết story qua nhiều nguồn.

Cơ:

> muốn cưới.

Cha Cơ:

> muốn trả nợ trước.

Hạc Tâm:

> lo cuộc hôn nhân bị lợi dụng chính trị.

Nhà Nậu:

> chỉ muốn khoản nợ được trả.

Di:

> sẵn sàng rời family.

Không ai là villain.

---

<a id="m06a-t017-s114"></a>

## 17. POSSIBLE RESOLUTIONS

Người chơi có thể:

trả khoản nợ.

Giúp gia đình Tạ kiếm tiền.

Đàm phán lại nợ.

Đề nghị hôn nhân như một liên minh ba nhà.

Chứng minh một phần debt không hợp lệ.

Giúp Cơ và Di bỏ đi.

Ép Hạc family.

Giết creditor.

Không làm gì.

Mỗi lựa chọn **không chỉ thay dialogue cuối quest**.

Nó thay:

Household Graph.

Relationship Graph.

Wealth.

Political Alliance.

Future children.

Migration.

Reputation.

---

<a id="m06a-t017-s115"></a>

## 18. TWENTY YEARS LATER

Nếu Cơ và Di kết hôn:

con họ có:

Bãi Lạc Culture.

Bến Hạc Culture.

Trade exposure.

Agricultural lineage.

Possible hybrid passive.

Nếu bỏ trốn:

relationship với family khác.

Nếu nhà Nậu bị player giết:

đứa trẻ có thể lớn lên cùng một family feud.

Một quest ở năm 7 thay đổi character generation năm 20.

---

<a id="m06a-t017-s116"></a>

## 19. QUEST EXPOSURE RULE

Story Thread chỉ xuất hiện với player nếu player có `Knowledge Access`.

Sources:

NPC request.

Rumor.

Witness.

Relationship.

Passive.

Profession.

Location.

Faction.

Item.

Previous quest.

Ví dụ murder:

Warrior thấy:

> corpse.

Healer thấy:

> unusual wound.

Hunter thấy:

> track.

Smith thấy:

> weapon style.

Vu Chúc thấy:

> Mạch anomaly.

Không phải player nào cũng nhận cùng quest information.

---

<a id="m06a-t017-s117"></a>

## 20. HISTORICAL TRACE

Event Importance ≥ threshold phải để lại dấu vết.

Có thể là:

memory;

grave;

ruin;

family story;

item;

song;

place name;

taboo;

festival;

scarred landscape.

Ví dụ:

một battle xảy ra.

20 năm sau battlefield có thể trở thành:

ruộng;

mộ;

miếu;

forbidden site;

hoặc không còn dấu gì ngoài câu chuyện người già kể.

---

<a id="m06a-t017-s118"></a>

## 21. MYTH FORMATION PIPELINE

Đây là hệ đặc biệt quan trọng cho game Văn Lang – Âu Lạc.

```text
Objective Event
↓
Witness Memories
↓
Rumor
↓
Retelling
↓
Symbolic Simplification
↓
Political / Religious Interpretation
↓
Regional Variants
↓
Myth
```

Ví dụ:

Một chief miền núi tổ chức dân chống lũ.

↓

Qua nhiều thế hệ:

“Người trên núi thắng dòng nước.”

↓

Câu chuyện nhập vào một narrative lớn hơn.

↓

Một vùng kể:

“Người đó là Sơn Chủ.”

Vùng khác:

“Đó là Sơn Tinh.”

Game không cần phủ nhận hay xác nhận hoàn toàn.

---

<a id="m06a-t017-s119"></a>

## 22. CORE EVENT LIBRARY v1.0 — KẾT QUẢ

Bộ đầu tiên hiện có:

- 10 Birth/Childhood events;
- 10 Family events;
- 10 Relationship events;
- 10 Profession/Economy events;
- 10 Environment/Health/Migration events;
- 10 Crime/Social Conflict events;
- 10 Political/War events;
- 10 Spiritual/Myth events.

Tổng:

**80 Core Event Definitions.**

80 event này không phải 80 story.

Nếu chúng được nối bằng causal graph, số chain hợp lệ có thể lên đến hàng chục nghìn.

Khi thêm:

parameter;

character psychology;

region;

culture;

settlement;

family;

relationship;

time;

world state;

mỗi chain có thể tạo ra rất nhiều câu chuyện khác nhau.

---

<a id="m06a-t017-s120"></a>

## 23. PRODUCTION PRINCIPLE

Narrative engine phải tuân thủ thứ tự:

```text
SIMULATION decides WHAT happened.

RULE ENGINE decides WHY it is valid.

CHARACTER MODEL decides HOW each person understands it.

STORY ENGINE decides WHETHER it forms an interesting thread.

QUEST ENGINE decides WHETHER player can intervene.

LLM decides HOW it is expressed in natural language.
```

Không được đảo thứ tự thành:

```text
LLM invents story
→ game tries to make it consistent.
```

Đây sẽ là nguyên tắc kiến trúc cốt lõi của toàn bộ game.

---

<a id="m06a-t017-s121"></a>

## 24. ACCEPTANCE CRITERIA CHO EVENT ENGINE

Event system chỉ được coi là đạt khi có thể chạy một settlement trong 20 năm và đảm bảo:

1. Không xuất hiện event vô nguyên nhân.

2. Không xuất hiện character biết thông tin họ không thể biết.

3. Death tạo thay đổi thật trong household/social graph.

4. Migration thay đổi population và culture.

5. Debt tồn tại tới khi được xử lý hoặc chuyển giao.

6. Marriage tạo household/family relationship thật.

7. Children sinh ra có lineage chính xác.

8. War tạo casualty/refugee/economic aftermath.

9. NPC psychology thay đổi dựa trên interpretation chứ không phải event tag đơn giản.

10. Story Thread có thể tồn tại khi player không can thiệp.

11. Player intervention thay đổi future simulation.

12. Event 10–20 năm trước vẫn có thể tạo consequence hiện tại.

13. Generated quest luôn truy ngược được ít nhất một causal chain.

14. Không settlement nào bị narrative overload vô lý.

15. Có đủ positive/neutral events để world không biến thành chuỗi thảm họa.

16. Ít nhất một số sự kiện lịch sử được chuyển dần thành oral history hoặc myth.

---

<a id="m06a-t017-s122"></a>

## 25. MỤC TIÊU KIỂM THỬ TIẾP THEO

Bước test quan trọng nhất không phải viết thêm 200 event.

Phải lấy **80 Core Events hiện tại** và chạy bằng thiết kế trên một môi trường cụ thể.

Settlement phù hợp nhất là:

**Bến Vạc — Tam Giang.**

Ta sẽ dựng:

- khoảng 120–150 NPC simulated;
- 20–25 household chi tiết;
- 5–7 lineage;
- economy cơ bản;
- season;
- river state;
- faction;
- relationship graph;
- khoảng 10 Named NPC Tier A.

Sau đó mô phỏng:

**20 năm game-time.**

Output phải gồm:

**World Chronicle**

toàn bộ sự kiện lớn.

**Family Chronicle**

lịch sử từng gia đình.

**Character Chronicle**

life event của NPC quan trọng.

**Story Threads**

các chain đã hình thành.

**Quest Windows**

những thời điểm player có thể can thiệp.

**Unresolved Narrative Debt**

những câu chuyện chưa kết thúc.

**Myth Seeds**

những event có khả năng trở thành truyền thuyết.

Nếu simulation 20 năm tại Bến Vạc tạo ra khoảng **10–20 story thread đủ mạnh, logic và khác nhau**, framework được coi là vượt proof-of-concept đầu tiên.

Phần tiếp theo tôi đề xuất làm **rất sâu case Bến Vạc 20 năm**, vì đây sẽ là bài kiểm tra thực tế đầu tiên của toàn bộ hệ thống chúng ta vừa xây: tạo dân số ban đầu, household, river economy, world state năm 0 rồi chạy từng năm, truy nguyên chính xác event nào gây ra event nào. Nếu bước đó hoạt động, gần như toàn bộ “xương sống kể chuyện” của GDD đã được chứng minh chứ không còn chỉ nằm trên lý thuyết.

<!-- SOURCE T017 END -->

