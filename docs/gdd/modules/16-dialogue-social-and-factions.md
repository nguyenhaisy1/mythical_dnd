# Module 16 — Hội thoại, thương lượng, danh tiếng và phe phái

**Baseline tài liệu:** 1.1-prototype-contracts · 2026-10-14  
**Nguồn:** [T039](../../source/conversation.md#t039)  
**Trạng thái:** Thiết kế chi tiết đã khôi phục; chưa phải bằng chứng gameplay đã triển khai.

## Hướng dẫn áp dụng khi phát triển

Relationship, claim-based reputation, belief và knowledge không gộp thành một điểm. Dialogue intent/outcome do rules; text không tự cập nhật world.

| Hợp đồng tích hợp | Nội dung |
|---|---|
| Dữ liệu đầu vào | Perceived claims, relationship dimensions, leverage, obligations, faction goals. |
| Đầu ra / bên sở hữu | Social proposals, claim transmission và faction actions; Social/Knowledge/Factions owners. |
| Đọc cùng | 05, 11, 17, 24–27.; ADR-008 và các contract specs bổ sung 14/10/2026. |
| Cổng nghiệm thu | Rumor lặp từ cùng nguồn không tăng uy tín vô hạn; NPC không trả lời từ secret chưa biết. |
| Việc chưa được chứng minh | Dialogue library, faction weights và translation production. |

**Thứ tự hiệu lực:** [Baseline áp dụng](../../governance/APPLICABLE-BASELINE.md) và [Decision Register](../../governance/DECISIONS.csv) giải quyết các đoạn cũ bên dưới. Phần nghiên cứu được giữ đầy đủ để không mất phương án, lý do và ví dụ; những câu “đề xuất”, “bước tiếp theo”, “chưa khóa” là trạng thái tại thời điểm hội thoại, không phải xác nhận triển khai hiện tại. Các tên/hệ số minh họa không tự trở thành production data.

## Toàn văn nghiên cứu và thiết kế

<!-- SOURCE T039 BEGIN -->

<a id="m16-t039-s001"></a>

## GDD MODULE 16 — DIALOGUE, SOCIAL INTERACTION, NEGOTIATION, REPUTATION & FACTION SYSTEM

<a id="m16-t039-s002"></a>

## 1. DESIGN PHILOSOPHY

Hội thoại trong game không phải hệ thống:

> chọn câu màu xanh để tăng thiện cảm.

Cũng không phải:

> Speech 80 → mọi người làm theo lời player.

Social gameplay phải được xây trên một câu hỏi cơ bản:

> **Người đối diện là ai, họ biết gì, họ muốn gì, họ sợ gì, họ tin ai và player có vị trí nào trong thế giới của họ?**

Một câu nói chỉ có ý nghĩa khi được đặt trong:

- quan hệ;
- hoàn cảnh;
- văn hóa;
- quyền lực;
- thông tin;
- lịch sử;
- lợi ích;
- tâm lý;
- reputation.

Do đó Social System không phải một minigame hội thoại.

Nó là giao diện trực tiếp với **Social Graph và World Simulation**.

---

<a id="m16-t039-s003"></a>

## 2. NĂM LỚP CỦA SOCIAL SYSTEM

Hệ thống chia thành năm lớp liên kết.

<a id="m16-t039-s004"></a>

### Dialogue

Những gì character nói và nghe.

<a id="m16-t039-s005"></a>

### Social Interaction

Những hành động xã hội ngoài lời nói.

Ví dụ:

- giúp đỡ;
- tặng;
- từ chối;
- giữ lời;
- đứng về phía ai.

<a id="m16-t039-s006"></a>

### Negotiation

Khi nhiều bên đang trao đổi lợi ích, điều kiện hoặc nghĩa vụ.

<a id="m16-t039-s007"></a>

### Reputation

Những gì người khác **nghĩ rằng họ biết về player**.

<a id="m16-t039-s008"></a>

### Faction

Những mạng lưới người có lợi ích, identity và quyền lực tập thể.

---

<a id="m16-t039-s009"></a>

## 3. DIALOGUE KHÔNG PHẢI TRUTH INTERFACE

NPC không nói ra “sự thật của game”.

Họ nói:

> những gì họ biết

+

> những gì họ tin

+

> những gì họ muốn người nghe tin.

Do đó một câu thoại có thể là:

True.

Mistaken.

Incomplete.

Biased.

Manipulative.

Deliberately False.

Symbolic.

Rumor.

---

<a id="m16-t039-s010"></a>

## 4. NPC KNOWLEDGE MODEL

Mỗi NPC có Knowledge Graph riêng.

Ví dụ sự kiện:

> Cầu Bến Vạc bị phá.

Objective Truth:

lũ cuốn.

NPC A biết:

đêm trước có người lạ xuất hiện.

NPC A tin:

người lạ phá cầu.

NPC B biết:

nước sông tăng nhanh.

NPC B tin:

do lũ.

NPC C nghe rumor:

Phong Châu phá cầu để chặn hàng.

Ba dialogue khác nhau.

---

<a id="m16-t039-s011"></a>

## 5. KNOWLEDGE PROVENANCE

Thông tin quan trọng lưu:

- nguồn;
- thời gian biết;
- độ tin cậy;
- confidence;
- whether firsthand;
- whether secret.

Ví dụ:

```text
Fact:
Lạc Tông gặp người Thượng Âu.

Source:
Hạc Lưu.

Source Type:
Second-hand.

Confidence:
Medium.

Secret Status:
Sensitive.
```

---

<a id="m16-t039-s012"></a>

## 6. PLAYER KNOWLEDGE CŨNG KHÔNG OMNISCIENT

Dialogue option chỉ xuất hiện nếu player:

- biết thông tin;
- suy luận được;
- hoặc đang bluff.

Không được tự nhiên có lựa chọn:

> “Ta biết chính ngươi giết hắn.”

nếu character chưa hề biết chuyện đó.

---

<a id="m16-t039-s013"></a>

## 7. DIALOGUE OPTION TYPES

Không cần hàng chục màu.

Các loại cơ bản:

<a id="m16-t039-s014"></a>

#### Ask

Hỏi thông tin.

<a id="m16-t039-s015"></a>

#### State

Đưa thông tin hoặc quan điểm.

<a id="m16-t039-s016"></a>

#### Appeal

Kêu gọi vào giá trị.

<a id="m16-t039-s017"></a>

#### Offer

Đưa lợi ích.

<a id="m16-t039-s018"></a>

#### Request

Yêu cầu.

<a id="m16-t039-s019"></a>

#### Promise

Cam kết.

<a id="m16-t039-s020"></a>

#### Threaten

Đe dọa hậu quả.

<a id="m16-t039-s021"></a>

#### Deceive

Cố tạo belief sai.

<a id="m16-t039-s022"></a>

#### Challenge

Phản biện hoặc chất vấn.

<a id="m16-t039-s023"></a>

#### Remain Silent

Không nói.

<a id="m16-t039-s024"></a>

#### Leave

Kết thúc.

Dialogue mạnh phải cho phép **im lặng** là lựa chọn thực.

---

<a id="m16-t039-s025"></a>

## 8. SAME SENTENCE, DIFFERENT SOCIAL ACTION

Ví dụ:

> “Ngươi nên rời làng trước khi trời tối.”

Có thể là:

Advice.

Threat.

Warning.

Concern.

Order.

Intent phụ thuộc:

speaker;

tone;

context;

relationship.

System nên lưu **Dialogue Intent**, không chỉ raw text.

---

<a id="m16-t039-s026"></a>

## 9. SOCIAL INTENT

Mỗi social action có Intent.

Ví dụ:

- Inform;
- Persuade;
- Pressure;
- Comfort;
- Humiliate;
- Test;
- Manipulate;
- Signal Loyalty;
- Signal Defiance;
- Build Trust;
- End Interaction.

LLM trình bày câu.

Rule Engine xử lý Intent.

---

<a id="m16-t039-s027"></a>

## 10. DIALOGUE CHECK KHÔNG XẢY RA MỖI CÂU

Không roll:

Persuasion check

sau từng câu.

Một cuộc nói chuyện có **Social State** tích lũy.

Check xảy ra khi:

- player yêu cầu thay đổi quyết định;
- vượt resistance;
- nói dối quan trọng;
- gây pressure;
- tìm thông tin nhạy cảm.

---

<a id="m16-t039-s028"></a>

## 11. SOCIAL STATE

Trong một conversation có thể có:

<a id="m16-t039-s029"></a>

#### Openness

NPC sẵn sàng nói bao nhiêu.

<a id="m16-t039-s030"></a>

#### Trust

Tin người nghe bao nhiêu.

<a id="m16-t039-s031"></a>

#### Pressure

Mức NPC cảm thấy bị ép.

<a id="m16-t039-s032"></a>

#### Irritation

Bực tức.

<a id="m16-t039-s033"></a>

#### Fear

Sợ.

<a id="m16-t039-s034"></a>

#### Interest

Quan tâm.

<a id="m16-t039-s035"></a>

#### Exposure

NPC đã tiết lộ quá nhiều chưa.

Đây là state tạm thời.

Không thay thế relationship dài hạn.

---

<a id="m16-t039-s036"></a>

## 12. CONVERSATION TEMPERATURE

Để UI dễ đọc:

- Calm;
- Guarded;
- Engaged;
- Tense;
- Hostile;
- Breaking.

Không phải mọi conversation bắt đầu Calm.

---

<a id="m16-t039-s037"></a>

## 13. NPC SOCIAL RESISTANCE

Một yêu cầu được đánh giá qua:

```text
Desire Compatibility
+ Relationship
+ Trust
+ Benefit
+ Value Alignment
+ Reputation
+ Evidence
+ Social Pressure
- Cost
- Fear
- Moral Boundary
- Obligation Conflict
```

Skill chỉ tác động một phần.

---

<a id="m16-t039-s038"></a>

## 14. HARD BOUNDARY

Nếu yêu cầu vi phạm Moral Boundary nghiêm trọng:

Persuasion không đủ.

Ví dụ:

một người không phản gia đình.

Player muốn họ tố cha.

Trước tiên cần:

- evidence;
- identity crisis;
- changed relationship;
- extreme circumstance.

Không có “natural 20 = phá personality”.

---

<a id="m16-t039-s039"></a>

## 15. SOFT RESISTANCE

Một số thứ NPC không thích nhưng vẫn có thể làm.

Ví dụ:

đi đường nguy hiểm.

cung cấp khoản vay.

giữ bí mật.

Skill/relationship/leverage có thể thay đổi.

---

<a id="m16-t039-s040"></a>

## 16. LEVERAGE

Leverage là thứ khiến negotiation có chiều sâu.

Các dạng:

<a id="m16-t039-s041"></a>

#### Positive Leverage

Có thứ NPC muốn.

<a id="m16-t039-s042"></a>

#### Negative Leverage

Có thể gây hậu quả họ muốn tránh.

<a id="m16-t039-s043"></a>

#### Informational Leverage

Biết điều quan trọng.

<a id="m16-t039-s044"></a>

#### Relational Leverage

Có người họ tin.

<a id="m16-t039-s045"></a>

#### Moral Leverage

Lời thề, duty, honor.

<a id="m16-t039-s046"></a>

#### Institutional Leverage

Luật tục, position, faction.

---

<a id="m16-t039-s047"></a>

## 17. LEVERAGE KHÔNG ĐỒNG NGHĨA BLACKMAIL

Ví dụ:

> “Nếu mở bến cho chúng tôi, thương nhân Hải Môn có thể dùng tuyến này.”

Đó là Economic Leverage.

---

<a id="m16-t039-s048"></a>

## 18. EVIDENCE IN DIALOGUE

Thông tin có thể được đưa ra dưới dạng:

Claim.

Evidence.

Witness.

Object.

Reputation.

Authority.

NPC đánh giá evidence dựa trên:

belief.

source.

bias.

knowledge.

---

<a id="m16-t039-s049"></a>

## 19. NPC CÓ THỂ KHÔNG TIN BẰNG CHỨNG THẬT

Ví dụ:

player mang evidence chứng minh người thân NPC phạm tội.

Nếu NPC:

attachment cao;

identity phụ thuộc family;

trust player thấp,

họ có thể từ chối tin.

Đây không phải bug.

---

<a id="m16-t039-s050"></a>

## 20. BELIEF CHANGE

Belief thay đổi theo:

Evidence Strength

×

Source Trust

×

Existing Belief Flexibility

×

Emotional Cost.

Một số belief chỉ thay đổi sau nhiều event.

---

<a id="m16-t039-s051"></a>

## 21. SOCIAL FAILURE

Failure không nhất thiết:

> NPC refuses.

Có thể:

- yêu cầu tăng giá;
- cung cấp ít thông tin;
- nghi ngờ player;
- nói dối;
- trì hoãn;
- báo người khác;
- yêu cầu proof;
- chuyển conversation sang người có authority.

---

<a id="m16-t039-s052"></a>

## 22. SOCIAL SUCCESS

Success cũng không luôn:

> NPC gives exactly what player asked.

Có thể:

- compromise;
- conditional agreement;
- introduction;
- information;
- temporary permission.

---

<a id="m16-t039-s053"></a>

## 23. CONDITIONAL AGREEMENT

Rất quan trọng.

NPC có thể nói:

> “Ta sẽ cho ngươi dùng thuyền nếu ngươi trả nó trước ngày lễ.”

System tạo:

**Obligation Object.**

---

<a id="m16-t039-s054"></a>

## 24. PROMISE SYSTEM

Lời hứa là object thật.

Fields:

```text
Promisor
Promisee
Content
Conditions
Deadline
Witnesses
Importance
Cultural Weight
Status
```

---

<a id="m16-t039-s055"></a>

## 25. PROMISE STATUS

Possible:

Open.

Fulfilled.

Partially Fulfilled.

Broken.

Excused.

Transferred.

Impossible.

Disputed.

---

<a id="m16-t039-s056"></a>

## 26. PROMISE MEMORY

NPC nhớ lời hứa theo Importance.

Minor promise:

có thể quên theo năm tháng.

Major promise:

gia đình có thể truyền lại.

---

<a id="m16-t039-s057"></a>

## 27. WITNESSED PROMISE

Một lời hứa trước:

elders.

ritual.

family.

faction

có trọng lượng lớn hơn lời riêng tư.

---

<a id="m16-t039-s058"></a>

## 28. OATH VS PROMISE

Promise:

cam kết cá nhân.

Oath:

cam kết được cultural/ritual/institutional framework chứng kiến.

Breaking Oath có:

reputation;

family;

possibly Mạch consequences.

---

<a id="m16-t039-s059"></a>

## 29. FAVOR SYSTEM

Favor khác Debt.

Favor là:

> một nghĩa vụ xã hội mềm.

Ví dụ:

NPC cứu player.

Player có thể cảm thấy obligation.

Không phải contract.

---

<a id="m16-t039-s060"></a>

## 30. DEBT SYSTEM

Debt có:

Creditor.

Debtor.

Resource.

Amount/Scope.

Reason.

Repayment expectation.

Interest nếu culture/context có.

Witness.

Legitimacy.

---

<a id="m16-t039-s061"></a>

## 31. SOCIAL DEBT

Không chỉ material.

Có thể:

> “Ta đã giúp con trai ngươi thoát khỏi án phạt.”

Đây là relational/political debt.

---

<a id="m16-t039-s062"></a>

## 32. OBLIGATION CONFLICT

Character có thể đồng thời:

nợ player

và

thề với faction.

Khi hai nghĩa vụ đối nghịch:

Narrative Engine tạo conflict.

---

<a id="m16-t039-s063"></a>

## 33. GIFT SYSTEM

Quà không phải affection token.

NPC đánh giá:

- usefulness;
- symbolism;
- appropriateness;
- motive;
- timing;
- relationship.

---

<a id="m16-t039-s064"></a>

## 34. INAPPROPRIATE GIFT

Một món quá đắt có thể bị hiểu là:

bribe.

insult.

proposal.

debt creation.

Player cần cultural knowledge.

---

<a id="m16-t039-s065"></a>

## 35. PUBLIC VS PRIVATE SOCIAL ACTION

Cùng một hành động trước đám đông có hậu quả khác.

Ví dụ:

phản bác leader riêng tư:

có thể accepted.

phản bác trước crowd:

authority challenge.

---

<a id="m16-t039-s066"></a>

## 36. FACE / PUBLIC STANDING

Không cần stat “Face”.

Nhưng public embarrassment là Event.

NPC pride/status quyết mức ảnh hưởng.

---

<a id="m16-t039-s067"></a>

## 37. SOCIAL STATUS

Character có Social Standing theo context:

Household.

Lineage.

Settlement.

Profession.

Faction.

Region.

Không có một “Social Level” toàn thế giới.

---

<a id="m16-t039-s068"></a>

## 38. STATUS EXPECTATION

Một trưởng bến có quyền:

chỉ huy ở bến.

Không nhất thiết có quyền trong ritual.

Authority contextual.

---

<a id="m16-t039-s069"></a>

## 39. AUTHORITY CHECK

NPC đánh giá:

> người này có quyền yêu cầu điều đó không?

Nếu không:

player có thể vẫn:

persuade;

threaten;

deceive.

Nhưng không dùng authority.

---

<a id="m16-t039-s070"></a>

## PART II — DIALOGUE PRESENTATION

<a id="m16-t039-s071"></a>

## 40. DIALOGUE UI PHILOSOPHY

Không nên hiện:

`[Persuasion 67%]`

mặc định.

Thay vào đó:

> **[Thuyết phục — dựa vào tình nghĩa]**

> **[Luật tục — viện dẫn quyền của người đưa tang]**

> **[Đe dọa — nhắc đến lính ở ngoài bến]**

---

<a id="m16-t039-s072"></a>

## 41. INFORMATION PREVIEW

Player cần biết intent.

Không nên bẫy người chơi bằng dialogue wording mơ hồ.

Option có thể cho tooltip:

> “Điều này gây áp lực lên danh dự của ông ta trước đám đông.”

---

<a id="m16-t039-s073"></a>

## 42. UNCERTAIN OUTCOME

Không hiển thị exact chance.

Có thể:

Likely.

Risky.

Very Risky.

Unknown.

Nếu character đủ social insight.

---

<a id="m16-t039-s074"></a>

## 43. HIDDEN SOCIAL INFORMATION

Character TÂM/Thấu Hiểu cao có thể nhận:

> “Ông ta tức giận nhưng có vẻ sợ mất mặt hơn là thực sự phản đối.”

Đó là actionable insight.

---

<a id="m16-t039-s075"></a>

## 44. TONE

Player có thể chọn Tone khi có ý nghĩa:

Respectful.

Neutral.

Firm.

Warm.

Cold.

Confrontational.

Không phải mọi line cần tone menu.

---

<a id="m16-t039-s076"></a>

## 45. SAME INTENT, DIFFERENT CHARACTER VOICE

Nếu player character:

young fisherwoman

và

elder leader,

LLM diễn đạt khác.

Intent giữ nguyên.

---

<a id="m16-t039-s077"></a>

## 46. PLAYER CHARACTER PERSONALITY

Player không bị khóa lời nói bởi personality.

Nhưng nếu repeatedly hành động trái personality:

Psychology có thể evolve.

Ví dụ character vốn Avoidant nhưng player liên tục confrontation.

Dần thay đổi Conflict Style.

---

<a id="m16-t039-s078"></a>

## 47. LIE SYSTEM

Khi player nói điều sai:

Engine xác định:

<a id="m16-t039-s079"></a>

#### Known Lie

Player biết nó sai.

<a id="m16-t039-s080"></a>

#### Bluff

Player không biết nhưng trình bày như thật.

<a id="m16-t039-s081"></a>

#### Mistake

Player tin nó đúng.

Chỉ Known Lie là intentional deception.

---

<a id="m16-t039-s082"></a>

## 48. DECEPTION CHECK

Đối đầu:

Player Deception

+

context

vs

NPC Thấu Hiểu

+

knowledge

+

suspicion.

Một lie tốt về performance vẫn fail nếu NPC biết truth chắc chắn.

---

<a id="m16-t039-s083"></a>

## 49. LIE CONSISTENCY

Game phải nhớ các lie quan trọng.

Nếu player kể hai version khác nhau:

NPC hoặc network có thể phát hiện contradiction.

---

<a id="m16-t039-s084"></a>

## 50. OMISSION

Player có thể nói truth nhưng giấu phần quan trọng.

Đây là deception dạng omission.

NPC reaction khác direct lie nếu phát hiện.

---

<a id="m16-t039-s085"></a>

## 51. SILENCE

Không trả lời có thể:

- bảo vệ secret;
- tăng suspicion;
- thể hiện defiance;
- thể hiện grief.

Silence là social action thật.

---

<a id="m16-t039-s086"></a>

## PART III — NEGOTIATION SYSTEM

<a id="m16-t039-s087"></a>

## 52. NEGOTIATION PHILOSOPHY

Negotiation không phải một Persuasion check lớn.

Nó là:

> **quá trình nhiều bên trao đổi lợi ích, điều kiện, nghĩa vụ và rủi ro để tìm một trạng thái mà họ chấp nhận được.**

---

<a id="m16-t039-s088"></a>

## 53. NEGOTIATION ACTOR MODEL

Mỗi actor có:

```text
Goal
Minimum Acceptable Outcome
Preferred Outcome
Red Lines
Resources
Leverage
Secrets
Known Information
Relationships
Time Pressure
Authority
Trust
```

---

<a id="m16-t039-s089"></a>

## 54. BARGAINING SPACE

Một negotiation chỉ có khả năng hòa giải nếu tồn tại vùng overlap.

Ví dụ:

A muốn 10 bao grain.

Minimum 7.

B muốn trả 4.

Maximum 8.

Overlap:

7–8.

Negotiation có cơ sở.

---

<a id="m16-t039-s090"></a>

## 55. NO BARGAINING SPACE

Nếu:

A minimum = 10.

B maximum = 8.

Không có agreement bằng lời nói thông thường.

Player phải thay đổi context:

- thêm resource;
- đưa third party;
- giảm fear;
- thay leadership;
- tạo alternative.

---

<a id="m16-t039-s091"></a>

## 56. NEGOTIATION ROUND

Không cần turn formal cứng nhưng logic có các bước:

1. Position revealed.
2. Information gathering.
3. Offer.
4. Counteroffer.
5. Leverage application.
6. Concession.
7. Agreement/Breakdown.

---

<a id="m16-t039-s092"></a>

## 57. CONCESSION

Mỗi bên có Concession Tolerance.

Nếu nhượng quá nhiều:

Internal faction backlash.

Không chỉ NPC cá nhân.

---

<a id="m16-t039-s093"></a>

## 58. CONCESSION MEMORY

Nếu leader bị player ép nhượng:

họ có thể chấp nhận hôm nay

nhưng Resentment tăng.

Short-term success ≠ long-term friendship.

---

<a id="m16-t039-s094"></a>

## 59. MULTI-PARTY NEGOTIATION

Game phải hỗ trợ 3+ actor.

Ví dụ:

Family A.

Family B.

Settlement Elder.

Player.

Faction.

Một agreement giữa A và player có thể làm B phản đối.

---

<a id="m16-t039-s095"></a>

## 60. COALITION

Trong negotiation, actors có thể tạo coalition.

Player có thể:

- align;
- split;
- mediate.

Coalition không static.

---

<a id="m16-t039-s096"></a>

## 61. SIDE DEAL

Player có thể nói riêng với một actor.

Nếu bị phát hiện:

Trust khác nhau.

Political gameplay xuất hiện.

---

<a id="m16-t039-s097"></a>

## 62. SECRET DEAL

System lưu:

Participants.

Terms.

Visibility.

Risk.

Nếu leak:

Reputation/Faction reaction.

---

<a id="m16-t039-s098"></a>

## 63. HOSTAGE / GUARANTEE

Một agreement có thể yêu cầu:

- hostage;
- marriage;
- goods;
- symbolic object;
- public oath;
- third-party witness.

Không chỉ money.

---

<a id="m16-t039-s099"></a>

## 64. MEDIATION

Player không cần thắng cho mình.

Có thể giúp hai bên đạt agreement.

Sứ Giả/Người Giữ Ký Ức rất mạnh.

---

<a id="m16-t039-s100"></a>

## 65. ARBITRATION

Khác mediation.

Arbitrator có authority đưa quyết định.

Requires:

recognized authority/custom.

Player có thể được mời nếu reputation phù hợp.

---

<a id="m16-t039-s101"></a>

## 66. NEGOTIATION FAILURE

Có nhiều mức:

- no agreement;
- temporary pause;
- one party walks out;
- accusation;
- public humiliation;
- alliance breaks;
- violence;
- delayed conflict.

---

<a id="m16-t039-s102"></a>

## 67. NEGOTIATION SUCCESS

Có nhiều mức:

- informal understanding;
- temporary truce;
- conditional agreement;
- formal pact;
- oath;
- alliance.

Mỗi loại có durability khác nhau.

---

<a id="m16-t039-s103"></a>

## 68. AGREEMENT DURABILITY

Phụ thuộc:

Mutual Benefit.

Trust.

Enforcement.

Witness.

Cultural Legitimacy.

Leadership Stability.

External Pressure.

Một treaty không tự tồn tại vĩnh viễn.

---

<a id="m16-t039-s104"></a>

## 69. AGREEMENT BREAK

Nếu conditions thay đổi:

Faction có thể phá agreement.

Không phải script betrayal.

---

<a id="m16-t039-s105"></a>

## 70. PLAYER CAN WRITE HISTORY THROUGH AGREEMENTS

Một agreement nhỏ có thể:

- mở trade route;
- kết thúc feud;
- tạo marriage;
- tạo migration;
- đổi political map.

Negotiation có world power ngang combat.

---

<a id="m16-t039-s106"></a>

## PART IV — REPUTATION SYSTEM

<a id="m16-t039-s107"></a>

## 71. REPUTATION PHILOSOPHY

Không có một thanh:

`Good Reputation 73`.

Reputation là:

> **những belief được xã hội lưu và truyền về một character.**

Do đó reputation:

- có nội dung;
- có source;
- có phạm vi;
- có độ tin cậy.

---

<a id="m16-t039-s108"></a>

## 72. REPUTATION RECORD

Ví dụ:

```text
Subject:
Mân

Claim:
Luôn đưa người bị thương về.

Origin:
Bến Vạc flood rescue.

Region:
Tam Giang.

Strength:
High.

Valence:
Positive for most households.

Accuracy:
Mostly true.
```

---

<a id="m16-t039-s109"></a>

## 73. REPUTATION IS CLAIM-BASED

Một character có thể đồng thời có:

> “Người cứu nạn đáng tin.”

và:

> “Kẻ phá luật của Phong Châu.”

Không contradiction.

Different groups value differently.

---

<a id="m16-t039-s110"></a>

## 74. REPUTATION DIMENSIONS

Có thể nhóm claim theo:

- Reliability;
- Courage;
- Violence;
- Compassion;
- Honesty;
- Professional Skill;
- Political Loyalty;
- Spiritual Standing;
- Wealth;
- Authority.

Nhưng UI ưu tiên natural-language claims.

---

<a id="m16-t039-s111"></a>

## 75. REPUTATION SCOPE

Claim có vùng lan.

<a id="m16-t039-s112"></a>

#### Household

<a id="m16-t039-s113"></a>

#### Settlement

<a id="m16-t039-s114"></a>

#### Regional

<a id="m16-t039-s115"></a>

#### Profession Network

<a id="m16-t039-s116"></a>

#### Faction

<a id="m16-t039-s117"></a>

#### Broad Historical

Không phải mọi việc nhỏ thành famous toàn map.

---

<a id="m16-t039-s118"></a>

## 76. REPUTATION PROPAGATION

Thông qua:

Witness.

Rumor.

Messenger.

Trade.

Family.

Faction network.

Storyteller.

---

<a id="m16-t039-s119"></a>

## 77. REPUTATION DISTORTION

Khi lan:

detail mất.

symbol tăng.

Ví dụ:

Truth:

Mân cứu 6 người.

Rumor:

Mân cứu 20.

Later:

> “Mân chưa từng để ai chết dưới nước.”

Đây là Myth Formation precursor.

---

<a id="m16-t039-s120"></a>

## 78. NEGATIVE REPUTATION

Không có universal “bad”.

Ví dụ:

> “Không quỳ trước trưởng dòng.”

Youth reformers:

Respect +.

Traditional elder:

Respect -.

Same reputation.

Different interpretation.

---

<a id="m16-t039-s121"></a>

## 79. PROFESSIONAL REPUTATION

Tách riêng.

Một người có thể:

personal reputation xấu

nhưng professional reputation cao.

---

<a id="m16-t039-s122"></a>

## 80. FACTION REPUTATION

Faction có belief về player dựa trên:

actions;

alliances;

history.

Không chỉ meter.

---

<a id="m16-t039-s123"></a>

## 81. REPUTATION CONFIDENCE

NPC nghe rumor nhưng chưa chắc tin.

Factors:

source.

distance.

existing belief.

---

<a id="m16-t039-s124"></a>

## 82. COUNTER-RUMOR

Player có thể:

deny.

provide evidence.

use witness.

ignore.

create alternative story.

Không có button “clear bounty”.

---

<a id="m16-t039-s125"></a>

## 83. REPUTATION REPAIR

Muốn thay reputation mạnh:

cần repeated contradictory evidence

hoặc

major public event.

Một speech không xóa 10 năm bad history.

---

<a id="m16-t039-s126"></a>

## 84. INFAMY CAN BE USEFUL

Reputation violent có thể:

- intimidate;
- attract warriors;
- scare merchants;
- make children fear.

Trade-off.

---

<a id="m16-t039-s127"></a>

## 85. UNKNOWN HERO

Player có thể làm việc lớn mà ít witness.

Objective impact cao.

Reputation thấp.

History không nhất thiết công bằng.

---

<a id="m16-t039-s128"></a>

## 86. FALSE HERO

Người khác có thể được credit.

Player có thể:

correct;

allow;

resent;

exploit.

---

<a id="m16-t039-s129"></a>

## 87. DEAD CHARACTER REPUTATION

Sau death:

reputation tiếp tục evolve.

Family/faction có thể reinterpret.

Player successor sống dưới legacy đó.

---

<a id="m16-t039-s130"></a>

## PART V — RUMOR SYSTEM

<a id="m16-t039-s131"></a>

## 88. RUMOR PHILOSOPHY

Rumor không chỉ là flavor text.

Nó là:

> **cơ chế truyền information không hoàn hảo qua social graph.**

---

<a id="m16-t039-s132"></a>

## 89. RUMOR OBJECT

Fields:

```text
Origin Event
Initial Speaker
Content
Truth Relation
Emotion
Sensitivity
Transmission Count
Geographic Reach
Current Variants
```

---

<a id="m16-t039-s133"></a>

## 90. RUMOR MUTATION

Mỗi transmission có thể:

- simplify;
- exaggerate;
- reverse blame;
- add supernatural interpretation;
- remove context.

---

<a id="m16-t039-s134"></a>

## 91. EMOTION AFFECTS SPREAD

Fearful rumor:

nhanh.

Scandal:

nhanh.

Technical information:

chậm.

---

<a id="m16-t039-s135"></a>

## 92. SOCIAL NETWORK AFFECTS SPREAD

Merchant:

trade route.

Ritualist:

spiritual network.

Soldier:

military network.

Family elder:

lineage.

---

<a id="m16-t039-s136"></a>

## 93. RUMOR AS GAMEPLAY SIGNAL

Player có thể nghe:

> “Có người mất tích gần Gò Cò.”

Không auto quest.

Có thể investigate.

---

<a id="m16-t039-s137"></a>

## 94. RUMOR CAN BE FALSE

Nhưng false rumor có thể tạo real event.

Ví dụ:

rumor village poisoned water.

Trade avoids village.

Economy suffers.

---

<a id="m16-t039-s138"></a>

## 95. PLAYER CAN START RUMOR

Intentional social action.

Requires:

source.

audience.

plausibility.

Can backfire.

---

<a id="m16-t039-s139"></a>

## 96. PROPAGANDA

Faction-scale controlled rumor.

Requires:

network;

messenger;

authority;

repeated messaging.

Không magical mind control.

---

<a id="m16-t039-s140"></a>

## PART VI — FACTION SYSTEM

<a id="m16-t039-s141"></a>

## 97. FACTION PHILOSOPHY

Faction không phải:

> Blue Team vs Red Team.

Faction là:

> **một mạng lưới con người có identity chung, lợi ích chung, cấu trúc quyền lực và khả năng hành động tập thể.**

---

<a id="m16-t039-s142"></a>

## 98. FACTION SCALE

Có nhiều cấp:

<a id="m16-t039-s143"></a>

#### Household Bloc

Một nhóm hộ.

<a id="m16-t039-s144"></a>

#### Lineage

Dòng họ.

<a id="m16-t039-s145"></a>

#### Professional Network

Thợ đúc, boatmen.

<a id="m16-t039-s146"></a>

#### Settlement Council

Quyền lực địa phương.

<a id="m16-t039-s147"></a>

#### Regional Coalition

Nhiều settlement.

<a id="m16-t039-s148"></a>

#### Political Power

Phong Châu, Thục-linked coalition.

<a id="m16-t039-s149"></a>

#### Religious Network

Ritual communities.

<a id="m16-t039-s150"></a>

#### Criminal Network

Smuggling/bandit.

---

<a id="m16-t039-s151"></a>

## 99. FACTION DATA MODEL

Mỗi faction có:

```text
Identity
Origin
Members
Leadership
Territory
Resources
Goals
Fears
Values
Internal Groups
Allies
Rivals
Obligations
Claims
Reputation
Cohesion
Legitimacy
Military Capacity
Economic Capacity
Information Network
Current Pressures
```

---

<a id="m16-t039-s152"></a>

## 100. FACTION IS NOT STATIC

Faction có thể:

- grow;
- split;
- merge;
- collapse;
- change leader;
- change goal;
- radicalize;
- reform.

---

<a id="m16-t039-s153"></a>

## 101. INTERNAL FACTION SUBGROUPS

Rất quan trọng.

Không được để:

> “Faction X nghĩ một kiểu.”

Ví dụ Phong Châu có thể có:

Traditionalists.

Reformers.

Military.

Ritual elites.

Trade interests.

---

<a id="m16-t039-s154"></a>

## 102. FACTION MEMBER DISAGREEMENT

Hai NPC cùng faction vẫn có thể đối nghịch.

Faction only creates shared context.

Không overwrite personality.

---

<a id="m16-t039-s155"></a>

## 103. FACTION GOALS

Có nhiều loại:

Survival.

Expansion.

Trade.

Political legitimacy.

Religious preservation.

Resource control.

Security.

Revenge.

Reform.

---

<a id="m16-t039-s156"></a>

## 104. FACTION RESOURCES

Không chỉ money.

Có:

- Food;
- Labor;
- Metal;
- Boats;
- Warriors;
- Land;
- Information;
- Ritual legitimacy;
- Trade access.

---

<a id="m16-t039-s157"></a>

## 105. FACTION POWER

Power đa chiều.

Một ritual faction:

military thấp.

legitimacy cao.

Một smith network:

economic/technology cao.

---

<a id="m16-t039-s158"></a>

## 106. TERRITORY

Faction control không phải border pixel-perfect.

Có:

Strong Control.

Influence.

Contested.

Presence.

No Reach.

---

<a id="m16-t039-s159"></a>

## 107. FACTION INFLUENCE

Có thể kiểm soát:

road.

market.

ritual.

office.

profession.

Không cần sở hữu đất.

---

<a id="m16-t039-s160"></a>

## 108. LEADERSHIP MODEL

Faction có:

Leader.

Council.

Elders.

Informal influencers.

Một người không có title vẫn có Influence cao.

---

<a id="m16-t039-s161"></a>

## 109. LEGITIMACY

Legitimacy trả lời:

> thành viên có tin quyền lãnh đạo là hợp lệ không?

Nguồn:

Tradition.

Success.

Lineage.

Ritual.

Consent.

Fear.

External support.

---

<a id="m16-t039-s162"></a>

## 110. COHESION

Cohesion đo:

faction có thể hành động thống nhất đến đâu.

Low cohesion:

orders bị bỏ qua.

subgroups cạnh tranh.

---

<a id="m16-t039-s163"></a>

## 111. FACTION PRESSURE

Food shortage.

Military threat.

Leadership crisis.

Debt.

Ideological split.

External influence.

Các pressure tạo event.

---

<a id="m16-t039-s164"></a>

## 112. FACTION DECISION

Không phải AI faction chọn ngẫu nhiên.

Decision dựa:

Goals

+

Pressure

+

Leadership

+

Resources

+

Known Information.

---

<a id="m16-t039-s165"></a>

## 113. FACTION KNOWLEDGE

Faction cũng có Knowledge Graph.

Thông tin lan qua network.

Không global omniscience.

---

<a id="m16-t039-s166"></a>

## 114. FACTION MISINFORMATION

Faction có thể hành động sai vì tin rumor.

Tạo lịch sử emergent.

---

<a id="m16-t039-s167"></a>

## 115. JOINING A FACTION

Player không click:

> Join.

Membership cần social reality.

Ví dụ:

- born into;
- recruited;
- oath;
- profession;
- marriage;
- appointment.

---

<a id="m16-t039-s168"></a>

## 116. MULTIPLE AFFILIATIONS

Character có thể thuộc:

Family.

Profession.

Settlement.

Political faction.

Religious network.

Đồng thời.

Conflict tạo narrative.

---

<a id="m16-t039-s169"></a>

## 117. FACTION LOYALTY

Không phải một bar.

Loyalty được phân thành:

- Identity;
- Obligation;
- Affection;
- Benefit;
- Fear.

Character có thể obey faction nhưng không yêu faction.

---

<a id="m16-t039-s170"></a>

## 118. PLAYER RELATIONSHIP WITH FACTION

Faction nhìn player bằng nhiều claim.

Ví dụ:

Useful.

Unreliable.

Dangerous.

Ally.

Debtor.

Enemy.

Potential Leader.

---

<a id="m16-t039-s171"></a>

## 119. FACTION STATUS

Player có thể có Position:

Outsider.

Known Contact.

Trusted Contact.

Member.

Agent.

Representative.

Leader.

Nhưng position không tự = reputation.

---

<a id="m16-t039-s172"></a>

## 120. FACTION TASK

Faction có thể giao task vì:

trust.

need.

obligation.

Không phải quest board.

---

<a id="m16-t039-s173"></a>

## 121. REFUSING FACTION TASK

Không luôn mất reputation.

Nếu player có lý do hợp lệ:

có thể accepted.

Một authoritarian leader:

khác.

---

<a id="m16-t039-s174"></a>

## 122. FACTION FAVOR

Faction có thể nợ player.

Favor có thể đổi:

- protection;
- information;
- manpower;
- access;
- resource.

Không phải currency universal.

---

<a id="m16-t039-s175"></a>

## 123. FACTION DEBT TO PLAYER

Nếu player liên tục giúp mà không nhận lại:

internal expectation hình thành.

Player có thể dùng leverage.

---

<a id="m16-t039-s176"></a>

## 124. PLAYER DEBT TO FACTION

Protection, training, equipment có giá.

Faction có thể gọi debt sau nhiều năm.

---

<a id="m16-t039-s177"></a>

## 125. FACTION CONFLICT

Conflict có nhiều cấp:

Competition.

Hostility.

Skirmish.

Open Conflict.

War.

Không tự jump từ dislike → war.

---

<a id="m16-t039-s178"></a>

## 126. CASUS / TRIGGER

War cần:

resource dispute.

leadership ambition.

broken oath.

territory.

revenge.

security fear.

Không chỉ diplomacy meter -100.

---

<a id="m16-t039-s179"></a>

## 127. WAR PREPARATION

Faction phải:

mobilize.

stock food.

secure route.

ally.

send messenger.

Player có thể phát hiện dấu hiệu.

---

<a id="m16-t039-s180"></a>

## 128. FACTION ALLIANCE

Alliance có terms.

Không permanent friendship.

Could be:

Defensive.

Trade.

Marriage.

Military.

Ritual.

Temporary.

---

<a id="m16-t039-s181"></a>

## 129. ALLIANCE DURABILITY

Depends:

shared threat.

mutual benefit.

leader continuity.

broken obligations.

---

<a id="m16-t039-s182"></a>

## 130. FACTION SPLIT

Nếu internal conflict cao:

subgroup có thể:

leave.

claim leadership.

form new faction.

---

<a id="m16-t039-s183"></a>

## 131. PLAYER CAN CAUSE SPLIT INDIRECTLY

Bằng:

evidence.

support.

leadership change.

resource.

Không phải “Split Faction” button.

---

<a id="m16-t039-s184"></a>

## 132. FACTION MERGER

Marriage.

war pressure.

shared identity.

political reform.

Có thể tạo faction mới.

---

<a id="m16-t039-s185"></a>

## 133. FACTION COLLAPSE

Không despawn members.

Members:

join others.

become refugees.

bandits.

independent households.

---

<a id="m16-t039-s186"></a>

## PART VII — POLITICS

<a id="m16-t039-s187"></a>

## 134. POLITICAL GAMEPLAY PHILOSOPHY

Politics không phải mini-strategy game riêng.

Nó là:

> relationship + faction + resource + legitimacy + information.

---

<a id="m16-t039-s188"></a>

## 135. POLITICAL POSITION

Một character có thể có office.

Office có:

Authority.

Duty.

Resource Access.

Obligation.

Succession Rule.

---

<a id="m16-t039-s189"></a>

## 136. OFFICE IS NOT CLASS

Trưởng bến có thể:

Sứ Giả.

Thủy Binh.

Thủ Lĩnh.

Không class-locked.

---

<a id="m16-t039-s190"></a>

## 137. POLITICAL APPOINTMENT

Requires:

support.

reputation.

legitimacy.

context.

---

<a id="m16-t039-s191"></a>

## 138. POLITICAL RIVAL

Rivalry có thể:

personal.

family.

institutional.

ideological.

Không phải enemy tag.

---

<a id="m16-t039-s192"></a>

## 139. COUNCIL SYSTEM

Một số settlement quyết định bằng council.

Actors có:

votes/influence.

Player có thể:

persuade individuals.

change proposal.

bring evidence.

---

<a id="m16-t039-s193"></a>

## 140. VOTING IS NOT SIMPLE COUNT

Một elder có thể influence others.

Public opinion matters.

Ritual legitimacy matters.

---

<a id="m16-t039-s194"></a>

## 141. PUBLIC OPINION

Không cần mass-population approval meter duy nhất.

Population clusters có beliefs.

Farmers.

Smiths.

Boatmen.

Households.

---

<a id="m16-t039-s195"></a>

## 142. POLICY CONSEQUENCE

Ví dụ levy grain.

Immediate:

army supplied.

Later:

FoodPressure rises.

Political decision → simulation.

---

<a id="m16-t039-s196"></a>

## PART VIII — SOCIAL COMBAT WITHOUT “SOCIAL COMBAT”

<a id="m16-t039-s197"></a>

## 143. KHÔNG DÙNG HP XÃ HỘI

Không có:

Resolve HP = 30.

Persuasion damage = 8.

Vì nó biến con người thành boss health bar.

---

<a id="m16-t039-s198"></a>

## 144. SOCIAL PROGRESS DỰA TRÊN STATE CHANGE

Player cần:

- reveal information;
- reduce fear;
- add benefit;
- increase trust;
- create leverage.

Thay vì giảm resolve.

---

<a id="m16-t039-s199"></a>

## 145. EXAMPLE

NPC không bán thuyền.

Reason:

đó là tài sản duy nhất nuôi family.

Persuasion cao không đủ.

Player có thể:

offer replacement boat.

promise profit share.

borrow instead.

solve family problem.

Đây là social gameplay thật.

---

<a id="m16-t039-s200"></a>

## PART IX — COMPANION SOCIAL INTERACTION

<a id="m16-t039-s201"></a>

## 146. COMPANION REACTION

Mỗi major social choice được companions interpret.

Không universal approval.

---

<a id="m16-t039-s202"></a>

## 147. COMPANION INTERJECTION

Companion có thể nói trong dialogue nếu:

knowledge relevant.

emotion high.

relationship grants comfort speaking.

---

<a id="m16-t039-s203"></a>

## 148. PLAYER CAN LET COMPANION LEAD

Ví dụ:

Nghi negotiation cao.

Player chọn:

> “Nghi, anh nói đi.”

NPC phản ứng với Nghi, không player.

---

<a id="m16-t039-s204"></a>

## 149. COMPANION FAILURE

Nếu Nghi gây insult:

đó là Nghi's action.

Player có thể repair.

Relationship/story arises.

---

<a id="m16-t039-s205"></a>

## 150. COMPANION SECRET AFFECTS NEGOTIATION

Nếu companion có relation với actor:

options change.

Có thể tốt hoặc xấu.

---

<a id="m16-t039-s206"></a>

## PART X — SOCIAL CONSEQUENCES OVER TIME

<a id="m16-t039-s207"></a>

## 151. MEMORY DECAY

Minor interactions fade.

Major humiliation, rescue, betrayal persist.

---

<a id="m16-t039-s208"></a>

## 152. RELATIONSHIP REINTERPRETATION

Years later:

NPC may reinterpret old event after new knowledge.

---

<a id="m16-t039-s209"></a>

## 153. POSTHUMOUS REINTERPRETATION

Một người chết có thể:

được tôn vinh.

bị bôi nhọ.

rehabilitated.

History moves.

---

<a id="m16-t039-s210"></a>

## 154. GENERATIONAL REPUTATION

Con có thể chịu:

family reputation.

Nhưng không inherit exact personal reputation.

---

<a id="m16-t039-s211"></a>

## 155. LINEAGE REPUTATION

Một dòng họ có claim:

Honorable.

Greedy.

Skilled Smiths.

Traitors.

These can be contested.

---

<a id="m16-t039-s212"></a>

## PART XI — SOCIAL FAILURE & FAIL-FORWARD

<a id="m16-t039-s213"></a>

## 156. WRONG PERSON

Player có thể negotiate với người không có authority.

Conversation thành information discovery.

---

<a id="m16-t039-s214"></a>

## 157. WRONG ASSUMPTION

Player nghĩ issue là tiền.

NPC thật ra quan tâm honor.

Failure reveals motive.

---

<a id="m16-t039-s215"></a>

## 158. PUBLIC FAILURE

Có thể tạo reputation event.

Không game over.

---

<a id="m16-t039-s216"></a>

## 159. BROKEN NEGOTIATION

Can lead to:

different mediator.

higher price.

delay.

combat.

faction event.

---

<a id="m16-t039-s217"></a>

## 160. SOCIAL CATASTROPHE

Chỉ khi:

multiple failures

+

high stakes

+

no recovery.

Ví dụ treaty collapses → war.

Ngay cả vậy story tiếp tục.

---

<a id="m16-t039-s218"></a>

## PART XII — FACTION & HISTORICAL CAMPAIGN

<a id="m16-t039-s219"></a>

## 161. LATE VĂN LANG

Faction landscape:

fragmented.

Lineage and local authority strong.

Hùng center has ritual/political influence but not omnipotent control.

---

<a id="m16-t039-s220"></a>

## 162. THỤC PHÁN RISE

Không spawn “ThucFaction = enemy”.

Instead:

highland coalitions.

marriage.

trade.

security.

military integration.

Player sees factions align gradually.

---

<a id="m16-t039-s221"></a>

## 163. ÂU LẠC FORMATION

Some factions:

support.

resist.

remain neutral.

negotiate terms.

Player relationships carry over.

---

<a id="m16-t039-s222"></a>

## 164. CỔ LOA

Creates new faction pressures:

labor.

land.

technology.

supply.

authority.

Professional networks gain political importance.

---

<a id="m16-t039-s223"></a>

## 165. MỴ CHÂU – TRỌNG THỦY PHASE

Political/family/social systems converge.

Trust.

marriage.

diplomacy.

security.

information.

No simplistic romance-only framing.

---

<a id="m16-t039-s224"></a>

## PART XIII — EXAMPLE SOCIAL SCENARIO

<a id="m16-t039-s225"></a>

## 166. SCENARIO: TRANH CHẤP KÊNH NƯỚC BÃI LẠC

Hai settlement:

Làng Dâu.

Đồng Mạ.

Drought.

Water low.

Làng Dâu upstream controls gate.

---

<a id="m16-t039-s226"></a>

## 167. ACTORS

<a id="m16-t039-s227"></a>

#### Nậu Thư

Wants canal reform.

<a id="m16-t039-s228"></a>

#### Nậu Khôn

Wants family control retained.

<a id="m16-t039-s229"></a>

#### Đồng Mạ Elder

Needs enough water for crop.

<a id="m16-t039-s230"></a>

#### Farmers

Fear starvation.

<a id="m16-t039-s231"></a>

#### Player

No automatic authority.

---

<a id="m16-t039-s232"></a>

## 168. CURRENT CLAIMS

Làng Dâu:

> gate built by our ancestors.

Đồng Mạ:

> downstream rights are old custom.

Both may contain partial truth.

---

<a id="m16-t039-s233"></a>

## 169. KNOWLEDGE ROUTES

Player can investigate:

old canal markers.

oral history.

family records.

river condition.

actual water volume.

---

<a id="m16-t039-s234"></a>

## 170. NEGOTIATION OPTIONS

<a id="m16-t039-s235"></a>

#### Resource Deal

Làng Dâu opens gate.

Đồng Mạ gives grain later.

<a id="m16-t039-s236"></a>

#### Shared Management

Create joint keeper.

<a id="m16-t039-s237"></a>

#### Engineering

Build bypass.

<a id="m16-t039-s238"></a>

#### Custom Arbitration

Invoke older water rule.

<a id="m16-t039-s239"></a>

#### Pressure

Threaten to expose land seizure history.

<a id="m16-t039-s240"></a>

#### Force

Open gate physically.

---

<a id="m16-t039-s241"></a>

## 171. CONSEQUENCE — RESOURCE DEAL

Short-term peace.

Creates debt.

If harvest fails:

debt cannot be paid.

Future thread.

---

<a id="m16-t039-s242"></a>

## 172. CONSEQUENCE — SHARED MANAGEMENT

Needs trusted keeper.

Creates new political institution.

Potential future corruption/conflict.

---

<a id="m16-t039-s243"></a>

## 173. CONSEQUENCE — ENGINEERING

Needs labor/material/time.

Xảo Nhân route.

May alter ecology.

---

<a id="m16-t039-s244"></a>

## 174. CONSEQUENCE — BLACKMAIL

Fast success.

Nậu family resentment.

Player reputation:

effective but dangerous.

---

<a id="m16-t039-s245"></a>

## 175. CONSEQUENCE — FORCE

Water flows.

But authority conflict.

Possible violence.

No solution is objectively “best”.

---

<a id="m16-t039-s246"></a>

## PART XIV — EXAMPLE FACTION EVOLUTION

<a id="m16-t039-s247"></a>

## 176. START

Đồng Đỏ has three smith networks.

A:

traditional bespoke craft.

B:

production-oriented.

C:

ritual bronze specialists.

---

<a id="m16-t039-s248"></a>

## 177. CỔ LOA DEMAND

Military asks for standardized components.

B gains wealth/influence.

A fears craft loss.

C fears sacred techniques misused.

---

<a id="m16-t039-s249"></a>

## 178. PLAYER INTERVENTION

Player can:

support B.

protect A.

mediate standards.

share C knowledge.

steal techniques.

---

<a id="m16-t039-s250"></a>

## 179. LONG-TERM OUTCOME

Could create:

Unified Metallurgy Council.

Split Guild War.

State-controlled workshop.

Independent family workshops.

Secret ritual craft network.

Faction system creates technology history.

---

<a id="m16-t039-s251"></a>

## PART XV — SOCIAL UI

<a id="m16-t039-s252"></a>

## 180. NPC SOCIAL PANEL

Player sees only known information.

Possible:

Relationship Summary.

Known Affiliations.

Known Goals.

Known Concerns.

Promises.

Debts.

Relevant Reputation.

---

<a id="m16-t039-s253"></a>

## 181. DO NOT SHOW RAW TRUST BY DEFAULT

Instead:

> Tin bạn trong công việc nhưng chưa sẵn sàng chia sẻ chuyện gia đình.

Advanced accessibility option may reveal more numbers.

---

<a id="m16-t039-s254"></a>

## 182. FACTION PANEL

Show:

Identity.

Known Leaders.

Player Standing.

Known Goals.

Allies/Rivals.

Known Obligations.

Rumors.

---

<a id="m16-t039-s255"></a>

## 183. POLITICAL MAP

Not exact ideology bars.

Shows influence based on player knowledge.

---

<a id="m16-t039-s256"></a>

## 184. PROMISE LEDGER

Important promises should be trackable.

Not as quest chores.

Example:

> Vạc Mộc promised to return Tòng's boat before flood season.

---

<a id="m16-t039-s257"></a>

## 185. DEBT LEDGER

Can show:

Who owes whom.

Reason.

Known terms.

This is crucial in complex campaigns.

---

<a id="m16-t039-s258"></a>

## 186. RUMOR LOG

Separate:

Heard.

Confirmed.

Disproved.

Unknown.

Player can compare versions.

---

<a id="m16-t039-s259"></a>

## PART XVI — LLM ARCHITECTURE

<a id="m16-t039-s260"></a>

## 187. RULE ENGINE OWNS SOCIAL STATE

LLM must not decide:

Trust score.

Goal.

Secret truth.

Faction membership.

Promise status.

---

<a id="m16-t039-s261"></a>

## 188. LLM INPUT

Dialogue generation receives:

Speaker Personality.

Current Emotion.

Relationship.

Known Facts.

Beliefs.

Intent.

Cultural Context.

Allowed Information.

---

<a id="m16-t039-s262"></a>

## 189. LLM OUTPUT

Natural dialogue.

Tone.

Maybe minor stylistic gestures.

---

<a id="m16-t039-s263"></a>

## 190. LLM FORBIDDEN

Invent:

new family members.

new promise.

new historical fact.

knowledge speaker shouldn't know.

---

<a id="m16-t039-s264"></a>

## 191. DIALOGUE VALIDATION

Before displaying line:

Fact check against Knowledge Graph.

Spoiler check.

Identity consistency.

Tone check.

---

<a id="m16-t039-s265"></a>

## PART XVII — CONTENT GENERATION

<a id="m16-t039-s266"></a>

## 192. AUTHORED DIALOGUE

Use for:

major historical scenes.

critical emotional arcs.

signature character moments.

---

<a id="m16-t039-s267"></a>

## 193. PROCEDURAL DIALOGUE

Use for:

routine trade.

travel banter.

minor negotiation.

dynamic world response.

---

<a id="m16-t039-s268"></a>

## 194. HYBRID DIALOGUE

Major scene has authored beats.

LLM bridges based on current state.

This is preferred for key narrative content.

---

<a id="m16-t039-s269"></a>

## PART XVIII — QUALITY GATE: DIALOGUE

<a id="m16-t039-s270"></a>

## 195. MỖI MAJOR DIALOGUE PHẢI TRẢ LỜI

1. Speaker muốn gì?
2. Speaker biết gì?
3. Speaker đang giấu gì?
4. Speaker nhìn player thế nào?
5. Speaker có thể thay đổi ý ở đâu?
6. Red Line là gì?
7. Information nào player có thể học?
8. Outcome nào persistent?
9. Companion có liên quan không?
10. Culture ảnh hưởng cách nói thế nào?

---

<a id="m16-t039-s271"></a>

## PART XIX — QUALITY GATE: NEGOTIATION

<a id="m16-t039-s272"></a>

## 196. MỖI MAJOR NEGOTIATION PHẢI CÓ

- ít nhất hai actor có goal riêng;
- acceptable range;
- leverage;
- red lines;
- consequence nếu không agreement;
- ít nhất hai cấu trúc thỏa thuận khác nhau;
- long-term obligation.

Không được chỉ:

Pass speech check.

---

<a id="m16-t039-s273"></a>

## PART XX — QUALITY GATE: REPUTATION

<a id="m16-t039-s274"></a>

## 197. MỘT REPUTATION EVENT PHẢI CÓ

Subject.

Claim.

Witness/Source.

Scope.

Interpretation.

Strength.

Không tạo reputation từ hành động không ai biết trừ khi evidence xuất hiện sau.

---

<a id="m16-t039-s275"></a>

## PART XXI — QUALITY GATE: FACTION

<a id="m16-t039-s276"></a>

## 198. FACTION CHỈ ĐẠT CHẤT LƯỢNG KHI CÓ

Origin.

Identity.

Goals.

Resources.

Leadership.

Internal disagreement.

External relationship.

Pressure.

Potential evolution.

Nếu faction chỉ có:

“Good guys” hoặc “Bad guys”:

thiết kế thất bại.

---

<a id="m16-t039-s277"></a>

## PART XXII — INTERACTION WITH PREVIOUS SYSTEMS

<a id="m16-t039-s278"></a>

## 199. PSYCHOLOGY

Personality determines:

belief flexibility.

conflict style.

lie style.

boundary.

---

<a id="m16-t039-s279"></a>

## 200. FAMILY

Kinship provides:

obligation.

reputation.

faction linkage.

---

<a id="m16-t039-s280"></a>

## 201. PROFESSION

Professional networks provide:

social access.

credibility.

---

<a id="m16-t039-s281"></a>

## 202. CLASS

Sứ Giả:

negotiation.

Thủ Lĩnh:

authority.

Người Giữ Ký Ức:

custom/history.

Vu Chúc:

ritual legitimacy.

Các Class khác vẫn social được bình thường.

---

<a id="m16-t039-s282"></a>

## 203. PARTY

Companions:

interject.

mediate.

create conflict.

provide reputation.

---

<a id="m16-t039-s283"></a>

## 204. TRAVEL

Information and faction control affect route.

---

<a id="m16-t039-s284"></a>

## 205. DUNGEON

Social dungeon uses full system.

---

<a id="m16-t039-s285"></a>

## 206. COMBAT

Combat can begin/end through social action.

Surrender.

Parley.

Threat.

---

<a id="m16-t039-s286"></a>

## 207. ECONOMY

Price depends on relationship, scarcity, faction.

---

<a id="m16-t039-s287"></a>

## 208. QUEST

Quest is often exposed through social network.

---

<a id="m16-t039-s288"></a>

## 209. NARRATIVE ENGINE

Promises, debt, reputation and faction pressures become Story Threads.

---

<a id="m16-t039-s289"></a>

## 210. LEGACY

Reputation continues after death.

Faction remembers.

History rewrites.

---

<a id="m16-t039-s290"></a>

## PART XXIII — DEFERRED TO PRODUCTION

Production sẽ làm sâu:

- exact social check formulas;
- conversational UI;
- LLM prompt architecture;
- authored faction roster;
- exact rumor spread timing;
- faction resource numbers;
- political map presentation;
- negotiation visualization;
- public opinion simulation scale;
- dialogue volume;
- localization style;
- voice performance;
- culturally specific speech patterns;
- exact titles and historical naming conventions.

---

<a id="m16-t039-s291"></a>

## PART XXIV — CÁC QUYẾT ĐỊNH ĐÃ KHÓA

NPC dialogue is knowledge-bounded.

NPC can be wrong.

Player dialogue options depend on player knowledge.

Persuasion cannot override hard moral boundary.

Conversation uses temporary Social State.

Negotiation is multi-variable, not single skill check.

Promises are persistent objects.

Oaths carry cultural/institutional weight.

Debt and favors are separate systems.

Public and private actions differ.

Reputation is claim-based, contextual and transmissible.

No universal morality reputation bar.

Rumor is mutable information.

Faction is a dynamic social network.

Factions contain internal subgroups.

Player can hold multiple affiliations.

Faction resources and information are simulated.

Faction conflict requires causal pressure.

Political titles are not Classes.

Agreement durability depends on benefit and legitimacy.

Companions can lead social interactions.

Social failure uses fail-forward.

LLM expresses dialogue but does not control truth/state.

---

<a id="m16-t039-s292"></a>

## 211. CORE SOCIAL STATEMENT

Một social RPG yếu hỏi:

> “Speech của tôi đủ cao chưa?”

Game này phải khiến player hỏi:

> “Người này thực sự muốn gì?”

> “Ông ta có quyền quyết chuyện này không?”

> “Nếu ông ta đồng ý với tôi trước mặt mọi người, ông ta sẽ mất gì?”

> “Thông tin này đến từ ai?”

> “Tôi có đang ép một người làm điều mà họ sẽ hận tôi suốt đời không?”

> “Lời hứa này liệu ba năm nữa tôi còn thực hiện được không?”

> “Nếu tôi thắng cuộc thương lượng hôm nay, ai sẽ là người trả giá năm sau?”

---

<a id="m16-t039-s293"></a>

## 212. CORE REPUTATION STATEMENT

Reputation không phải:

> “Thế giới thích tôi bao nhiêu?”

Nó là:

> **“Thế giới đang kể câu chuyện gì về tôi?”**

Và câu chuyện đó có thể:

đúng;

sai;

phóng đại;

bị chính trị hóa;

hoặc trở thành truyền thuyết.

---

<a id="m16-t039-s294"></a>

## 213. CORE FACTION STATEMENT

Faction không phải những khối màu trên bản đồ.

Faction là:

> **những con người đã quyết định rằng một số lợi ích, ký ức hoặc tương lai của họ gắn với nhau.**

Khi áp lực thay đổi:

họ có thể đoàn kết.

họ có thể chia rẽ.

họ có thể phản bội.

họ có thể tha thứ.

Và player không “tăng điểm faction”.

Player sống giữa những mạng lưới đó và phải chịu hậu quả của từng lời hứa, từng cuộc thương lượng và từng lần đứng về một phía.

Với module này, chúng ta đã có đủ nền để bước sang **Quest System hoàn chỉnh**. Bước đó sẽ rất quan trọng vì Quest không nên là danh sách nhiệm vụ do NPC phát ra; nó phải là **cửa sổ mà qua đó player can thiệp vào Story Thread đang thật sự tồn tại trong world**, có thể phát hiện muộn, bỏ qua, thất bại, giải quyết một phần hoặc khiến nó biến thành một thread khác.

<!-- SOURCE T039 END -->

