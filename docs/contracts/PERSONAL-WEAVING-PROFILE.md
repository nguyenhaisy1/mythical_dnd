# Đan dây cá nhân gắn với xuất thân

10/11/2026 · Sprint17 / SPIN-041 · ADR026 · Profile `personal-weaving-v1`.

## Phạm vi và nguồn

Nối một vòng chế tác thật vào xuất thân ngẫu nhiên: New → Reveal → World → xem trước đan dây → xác nhận → giữ nguyên liệu → chờ → nhận dây; hủy, lưu khi đang làm, khởi động lại và Continue phải giữ đúng công việc. M07 cho phép chọn nhân vật có sẵn; M18§90–110 có personal craft/material/process/tool/skill/time; M28§27–31 có crafting và committed consequence. Đây một subset chế tác, không thêm nghề hoàn chỉnh thứ tư vào yêu cầu ba nghề VS03 hoặc nhận fullVS02/VS03/VS07.

Pack Sprint11 chỉ có grain/timber và quyền theo hộ. Nó không có fiber/cord/tool đan. Dữ liệu mới dưới đây là prototype-authored genesis mới, không phải tài sản đã được history chứng minh, không dữ kiện khảo cổ và không quyền quản lý kho hộ. Không sửa pack/archive để làm như đã có.

Cohort tám người và OriginSelector/profile/world-ID/seed/RNG context giữ nguyên. Vạc Lâm weaving43 có episode học đan dây; Bà Nhu weaving61 có học đan đồ dùng. Chỉ hai người vượt threshold40. Sáu người khác không được grant weaving/boat-repair: UI nói bản thử chưa hỗ trợ công việc phù hợp xuất thân này, không nói họ không có nghề hoặc ép reroll. New vẫn random tám người, không chọn/chỉnh xuất thân. Bare-hand braiding là recipe prototype này: dùng tay, không tạo tool item hoặc tuyên bố đã kiểm full tool/workshop system.

## Parameters và genesis

Nguồn versioned `rules/parameters/personal-weaving-v1.json`: profile/recipe/material/output IDs cố định, fiber_cost2, duration_ticks30, cord_yield1, minimum_skill40, starter_fiber8, tool_mode hands; source ghi rõ quyết định engineering ADR026, chưa balance. Không RNG cho thao tác đan.

Mỗi campaign mới của profile17 có một actor chính, cùng Origin như16, household/home historical IDs, hai ResourceLots tại home thuộc chính actor: input lot counter1 commodity.fiber@1 OnHand8/Reserved0; output lot counter2 commodity.cord@1 OnHand0/Reserved0. Chỉ controls own actor, không household. Starter kit được ghi rõ trên UI/runbook là vật liệu cá nhân của bản thử mới; không retroactively enrich save16. SkillValue lấy từ cohort authored weaving, không có thì0; không player input.

Registry compiler strict các fields/IDs/limits/constants, deep immutable copy. Recipe fingerprint = canonical hash parameter document; profile content hash = canonical hash object {profile:"personal-weaving-v1",cohort_hash:<cohort hash>,parameters:<parameter document>}. Origin.CohortContentHash vẫn là hash cohort16, không đổi nghĩa thành hash mới. New root `personal-weaving-v1`, exact rule `transfer-resource-v1+scheduler-v1+origin-cohort-v1+personal-weaving-v1`.

## Domain API khóa trước triển khai

Namespace Game.Domain.Weaving:

- WeavingRecipe(int fiberCost,long durationTicks,int cordYield,int minimumSkill,string fingerprint); immutable FiberCost/DurationTicks/CordYield/MinimumSkill/Fingerprint; constant IDs/profile theo parameter document, không Unity/IO/global random.
- WeavingActivityStatus Running/Completed/Cancelled; WeavingActivity(id,actorId,jobId,startedTick,dueTick,startCommandId,correlationId,status,reservedQuantity,terminalReason=null), typed Activity/Actor/Job IDs và WorldTicks; không public mutators.
- WeavingState(recipe,contentHash,actorId,worksite,inputLotId,outputLotId,skillValue,activities): immutable Recipe/ContentHash/ActorId/Worksite/InputLotId/OutputLotId/SkillValue/Activities, Validate(WorldSnapshot). State là một personal worksite cố định; mỗi activity pin các effective values qua recipe immutable của profile và due/start, không đổi recipe sau genesis.
- WorldSnapshot optional Weaving cuối constructor, requires Origin và noRepair; Origin+Weaving hợp lệ, legacy Origin-only giữ nguyên. Mọi clone transfer/scheduler/repair giữ extension. Domain không ép actor sống/ở nhà mãi; chỉ Start/completion recheck.

Core worker có thể thêm pure rules/plans/helpers, nhưng không đổi public model trên âm thầm; báo root/worker khác trước nếu cần. Job third type có IsWeaving/WeavingActivityId và ScheduledJob.ForWeaving(jobId,actorId,dueTick,sequence,activityId,startCommandId,correlationId). Không fake TransferResource hoặc đổi RepairState thành craft.

## Command, scheduler và invariants

StartWeaving payload chính xác actor_id/input_lot_id/output_lot_id, version1; actor phải issuer đã authenticate và State.ActorId. Recheck alive, weaving>=40, current location/worksite, quyền controls cả input/output owner, đúng commodities, available>=2, output cộng1 không overflow, không Running cùng actor; không nhận quantity/duration từ client. ID allocation stage chưa commit khi reject. Start reserve2, OnHand giữ8, output0; tạo Activity và Job phaseProductionHealth/priority0/due=current+30/handlerpersonal-weaving-v1, event WeavingStarted và receipt trong một publication.

CancelWeaving payload activity_id/reason; chỉ own activity Running. Release đúng hold2, không tăng OnHand hoặc output; retire job/activity, ActivityCancelled+JobCancelled có provenance. Cùng original envelope retry trả receipt cũ; changed intent mismatch. Cho CancelWeaving trong active Advance; CancelJob trực tiếp với weaving phải ACTIVITY_STATE. Không cho Start trong workflow chưa hoàn tất.

Completion chỉ trusted internal ExecuteScheduledWeaving; external CompleteWeaving/ExecuteScheduledWeaving hoặc scheduler/ ingress bị chặn. Trước due không consume/output. Due recheck alive/location/rights/skill/material hold/commodities và pairing đầy đủ; success OnHand−2/Reserved−2/output+1, Completed, retire job/activity, ordered WeavingCompleted+JobCompleted và receipt/tick trong cùng publication. Không publish tick riêng trước effect. Nếu invalidation hợp lệ: cancel/release còn thuộc activity, không tạo bù vật liệu, có reason/facts; orphan/corrupt provenance bị từ chối khi restore, không chữa bằng fabricated state.

Profile17 không hỗ trợ TransferResource: dispatcher từ chối COMMAND_UNKNOWN khi Weaving hiện diện, giữ các profile cũ. Input balance phải bằng8−2×completed count, output bằng completed count, cùng reservation sum và causal facts; không nhận stock đổi tùy ý khi restore. Pure rules có thể dùng trusted recomposition để test thiếu/overflow, không suy trạng thái đó là campaign genesis hợp lệ.

Validate hold totals/lot.Available/nonnegative/checked overflow, one Running, typed active/retired refs, job↔activity status/due/handler/source/correlation/creation sequence, history start/completion/cancel receipts/events/effective material quantities. Forged canonical image đổi activity/holds/history không được nhận chỉ vì round-trip. Origin và genesis recipe/content/actor/worksite/lot IDs/skill immutable qua durable commit; future skill progression cần contract mới. Actor alive/location/owner current là world state riêng, không bị pin nhầm vào Origin.

## Persistence và compatibility

Canonical image4 chỉ khi Origin+Weaving; thêm root weaving object chứa profile/recipe/content_hash/actor/worksite/input_lot/output_lot/skill/activities. Object recipe chứa fiber_cost/duration_ticks/cord_yield/minimum_skill/fingerprint; activities typed fields theo model trên. Codec unknownfields/canonical strict; old image1/2/3 encode bytes/hash/goldens giữ nguyên. Weaving jobs tagged kind riêng, không thêm null keys vào legacy jobs.

SqliteWorldStore.WeavingRuleVersion mới exact; content hash bằng Weaving.ContentHash, Origin/source/cohort binding do factory kiểm. Origin-only store vẫn requires Origin.CohortContentHash==contentHash. Physical schema2 giữ DDL; mirror entities nếu cần không được bỏ qua effects/holds. Commit pin canonical origin và weaving genesis metadata trước writer; không pin activities như genesis. Snapshot/recovery giữ Running/tick/cursor/holds, exact mismatch/corrupt/foreign files fail closed không overwrite.

Gateway mới giữ cùng atomic create-new pending/verify/flush/File.Move và request/catalog/cancel/crash semantics16 trong root riêng. File naming origin-<10digits>-<request32hex>.db được reuse chỉ trong root17, không enumerate root16. No automatic migration/enrichment. Continue/Open/Reload phục hồi riêng durable Advance workflow đã được chấp nhận của principal bằng DrainAdvance trước khi trả session sẵn sàng; không tạo Advance mới hoặc target mới. Crash trước job commit có raw Running29/hold2; Continue hoàn tất accepted target30 một lần. Crash sau job commit nhưng trước workflow finish cũng không để UI mắc ADVANCE_IN_PROGRESS. Raw persisted crash state và trạng thái sau resume được kiểm riêng.

Factory17 cố ý chỉ mở trạng thái client bounded còn ở home/có đúng hai lot cá nhân được quan sát và controls own actor; foreign/current-owner hoặc current-location khác bị từ chối, không auto-grant knowledge/quyền mới. Domain/history không pin các current fields đó vào Origin: trusted recomposition tests vẫn phải chứng minh due revalidation/cancel. Đây không là end-to-end Travel/forced-rights/Health command hoặc hỗ trợ mở mọi world variant qua client17; những command/profile tương lai cần acceptance riêng.

Factory verifies selected Origin from seed/cohort, exact params/recipe/profile/skill/genesis IDs và allowed bounded actor/2lots; loading không regenerate assignment hoặc starter balance. Replay artifactv1 tiếp tục từ chối mọi Origin và WeavingRuleVersion; sprint này không claim origin/weaving command replay.

## Safe session và Unity ports

Namespace Game.Application.Weaving, giữ ports16 nguyên vẹn:

IWeavingCampaignSession : IOriginCampaignSession, thêm ReadWork(): WeavingWorkView; Preview(): WeavingPreview; Start(string previewToken,string requestId): OriginResult; Cancel(string requestId): OriginResult. Read() inherited vẫn safe origin DTO. IWeavingCampaignGateway có ReadMenu(), NewCampaign(requestId,CancellationToken), Continue(), trả WeavingOpenResult với Result/Session.

WeavingWorkView immutable: Qualified bool, SkillValue/RequiredSkill int, FiberOnHand/FiberReserved/CordOnHand long, Status string(idle/running/completed/cancelled), Tick/DueTick long (0 nếu none), CanStart/CanCancel bool, ScopeNote string. WeavingPreview immutable: Allowed bool, Code string, Token nullable string, FiberCost int, DurationTicks long, CordYield int. Không raw IDs/seed/world/processor/registry/private revision/SQL/path hoặc otheractors.

Starter quantities được quan sát explicit khi nhập bản thử; own committed work outcomes grant cập nhật các quantities đó. Ownership không tự cấp general Knowledge; session chỉ exact self lots/profile, không enumerate hidden stock. Preview thuần authoritative state/RNG, token opaque pin private revision/tick/recipe/actor/lots; Start revalidates. Retry same request sau response-loss phải dùng original envelope trước stale-token check; changed token/intent mismatch. Cancel và Advance original envelope giữ qua save/reload/reopen, finite safe errors, không exception detail.

Scene17/bootstrap/view/QA/build mới; reuse assets/font15/G13v1, neutral location marker rõ nghĩa. Reveal giữ origin16; World thêm vật liệu/sản phẩm/kỹ năng/Running due, preview modal Confirm và Cancel riêng, usergesture mở preview không tự Confirm. NativeTab/Enter/Space/Esc/pointer, modal inert/focus, scroll six resolution/textscale cases. UI không tự cộng đồ hoặc WorldTick. Nếu nonweaver, vẫn đọc/lưu/chờ/Continue, không nút craft enabled. Không asset/image/model3D mới trong scope.

## Năm gate đóng Sprint17

1. Core rules/history/invariants/reject/overflow/rights/qualification/busy/revalidation; deterministic known results, no new RNG, independent adversarial cases và full regression.
2. Actual SQLite StartRunning/save/reopen→complete/cancel, original retries/response-loss/fault/SIGKILL, atomic New, snapshots/recovery/immutability/exact-profile refusal; image1/2/3 và source/goldens giữ.
3. Actual Unity6000.6.7f1/LinuxMono: Lâm/Nhu craft thật và nonweaver denial; material/output/receipt/due effects từ DB; saveRunning/actual Player restart/Continue hoàn tất không double; actual scene16 và repair15 Player regression.
4. Actual renderer six1280×720/1920×1080×100/125/150% cases, scroll/focus/modal; native OS preview/Confirm bằng hai gesture, wait/cancel/save/restart/input once; đọc final SQLite độc lập.
5. Independent source/artifact reviews noP0/P1, final source guard/full runtime/negative/process/docs/exports, exactHEADCI và feature→PR→dev. Không bỏ gate sang backlog để đóng.

Không tự Sprint18/main/tag/release. Backlog ngoài sprint: full profesions/three-profession milestone, toàn world/NPC/Knowledge, random life generator/viability/TEST001, trading/transport/medicine, tool degradation/quality/skills/class progression, final art/cast/user approval/humanplaytest/performance/crossplatform và newcommandreplayprofile.
