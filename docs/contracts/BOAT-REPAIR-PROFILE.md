# Boat repair profile v1 — hợp đồng Sprint14

03/11/2026 · ADR023 · VS-03A · SPIN-038. Đây là profile thực thi headless giới hạn, không full nghề/life generator hoặc Unity client. Quyết định trong phạm vi sprint AI được giao; không sửa archive hoặc giả source đã chốt các chi tiết mới.

## Phạm vi và nguồn

Thực hiện StartRepair / CancelActivity / completion nội bộ qua AdvanceTime+DrainAdvance. Recipe prototype từ COMMAND-ACTIVITY-CATALOG và parameters.kernel@1: reserve2timber, duration60WorldTick phút, gain200condition, max1000; không RNG. Authoring mới `rules/parameters/boat-repair-v1.json` là source cho compiler strict riêng, không nới kernel-content-v1 hoặc thay hash/golden legacy. Profile/rule compatibility mới và committed image2, SQLite physical schema2 giữ vì không đổiDDL. Không tự nâng save cũ thành world có repair.

Đã kiểm code trước khi chốt: pipeline/scheduler/image1 hiện transfer-only, WorldSnapshot chưa có item/activity/tool/capability. Không mô phỏng repair bằng transfer giả. Foundation đã có Item/Activity/Job kinds, identity-v1 giữ nguyên.

## State và ownership

- `WorldSnapshot.Repair` là extension immutable explicit/null. Null là legacy, phải giữ nguyên canonical bytes/hash/mirror/output cũ. Khi bật (kể cả arrays rỗng) image2 ghi profile/recipe fingerprint và typed repair state.
- Assets: item typed Boat/RepairTool có ID, owner(actor/household), settlement, condition0..1000, destroyed flag. Material dùng ResourceLot timber hiện có; OnHand bao gồm Reserved, Available=OnHand−Reserved.
- Repair capabilities: trusted fixture/content cấp actor ID, qualified bool, tool item ID. Bool là eligibility input cho profile kỹ thuật này, không full skill/class progression hoặc cho player chọn xuất thân. Tool phải là tool item còn dùng được, cùng nơi và actor điều khiển quyền.
- Economy: activity có ActivityId/actor/item/materialLot/tool/worksite, JobId, StartedTick/DueTick, statusRunning/Completed/Cancelled; pin recipeID+fingerprint và effective duration/quantity/gain/cap để reopen không dùng công thức mới âm thầm. Reservation có provenance activity→lot; mỗi Running giữ lượng riêng, terminal giữ0. Sum repair holds trên lot không vượt lot.Reserved; phần reserve domain khác không tự bị xóa.
- Application: schedule/retire job, atomic receipts/events/identity/revision. Start tạo Activity rồi Job ở stage uncommitted; reject/failure không consume committed ID/RNG. Terminal retire Activity/Job, giữ record lịch sử. Mọi clone WorldSnapshot phải carry Repair, kể cả transfer/scheduler workflows không thuộc repair.
- Typed Domain evaluator/proposal riêng cho repair được phép để giữ ChangeSet transfer-only; không làm generic framework không cần thiết. Nếu chọn đường riêng, không giả RuleRegistry transfer đã chứa repair. Branch compiled được composition/profile cho phép, không arbitrary content scripts.

## Hành vi lệnh

StartRepair payload chính xác `actor_id,item_id,material_lot_id,recipe_id`, payload_version1. actor_id phải là issuer đã authenticate. Không nhận quantity/duration/gain từ client. Recheck alive, qualified/tool, actor/item/tool/material cùng worksite, quyền actor.Controls của tất cả nguồn, material commoditytimber, available≥2, boat không destroyed và condition<1000; actor/boat/tool không có activityRunning cạnh tranh. Không reserve lại slot đang dùng.

Accepted Start atomically: lot.Reserved+2, OnHand giữ; Running activity giữ2; job CompleteRepair phaseProductionHealth(3),priority0,due=current+60 dùng creationsequence hiện hành; RepairStarted event có causation/correlation/recipe/hash vàref activity/job; receipt tăng revision1. Overflow DueTick/revision/sequence phải từ chối/thất bại phù hợp, không publish nửa state.

Completion chỉ ở đường trusted scheduler nội bộ, job type/ActivityId/JobId/actor/due/causation binding đầy đủ. Không fake TransferResource và không mở public CompleteRepair chỉ vì source_type=Scheduler. External CompleteRepair phải bị từ chối; `scheduler/` ingress protection giữ nguyên.

Ở due tick recheck activity/job, actor alive, quyền và qualified/tool hiện hành, tất cả còn ở worksite ban đầu, item không destroyed/full hoặc thiếu. Thành công atomically: OnHand−2, Reserved−2, condition=min(1000,old+200), activityCompleted/hold0, retireactivity/job, ordered RepairCompleted+JobCompleted vàreceipt trong cùng publication. Due-input không publish tick riêng trước effects. Không doubleconsume khi retry hoặc reopen.

Nếu completion gặp death/location/tool/rights/qualification/item invalidation hợp lệ thì atomically cancel activity, release phần hold còn tồn tại không cộng OnHand, retire job và ghi ActivityCancelled+JobCancelled vớireason. Không đểjobterminal màactivityRunning/hold bị rò. Không nhận Health/death command hoặc travel/rights mutation đã implement chỉ vì test dùng trusted recomposed state để kiểm revalidation.

CancelActivity payload chính xác `activity_id,reason`, issuer phải điều khiển actor hoạt động; Running→Cancelled, release hold, retire job/activity, ordered ActivityCancelled+JobCancelled vàreceipt atomic. Cancel sau terminal trả ACTIVITY_STATE; cùngcommandID lấyreceipt cũ. ChoCancelActivity khiAdvanceworkflow active, không choStart trongđó. CancelJob trực tiếp vớirepairjob phải reject ACTIVITY_STATE (dùngCancelActivity); không chạy cancellation transfer-only làm ròhold.

DestroyResource/Health/Travel/forced-rights commands ngoài scope. Các aggregate hỏng provenance/reservation thiếu khirestore phải bị từ chối trướcpublication; không tự tạo lại vật liệu. Revalidation trên state hợp lệ không thay acceptance các domain chưa triển khai.

## Invariants và compatibility

Item/actor/lot/activity/job refs đúng world/type/identity lifecycle; activeRunning refs hợp lệ, terminal giữ refs/historical lifecycle. Không thêm/sửa item/capability/hold qua TransferResource; transfer chỉ cập nhật OnHand theo Available và carry extension không đổi. Bất kỳ publication phải kiểm duplicate refs, holds/lot totals, actor/tool/boat exclusivity và job↔activity pairing.

Legacy image1/transfer scheduler/replay/content goldens phải byte-identical; không overwriteexpected. Image2 explicit profile, strict canonical round-trip/unknown fields/type refusal, tagged repair job không dummytransfer. Newexactruleversion `transfer-resource-v1+scheduler-v1+boat-repair-v1`; new content fingerprint từcompiler. SqliteWorldStore defaultlegacy, optional expectedruleversion có allowlist; tất cảload/manifest/snapshot/recovery/migration paths mang đúnginstanceprofile. Mirror item/activity dùng entities BLOB sẵn, khôngDDL/schema3. Wrong profile/hash/image phải refuse không ghi source. Old reader không hứa load image2; không auto-convert old world. Replay artifact1 giữ nếu action/manifest format không đổi, factory tạo processor phải mangrecipe/profile chính xác.

## Acceptance tối thiểu

1. Start giữ2, không consume; beforedue không cộngcondition; completion đúngdue consume1lần/gaincap; waitkhôngrepair giữstate.
2. Cancel/retry/mismatch/stale/unauthorized/dead/notqualified/tool/location/material/busy/full/destroyed/overflow; forgedcompletion/CancelJob không bypass.
3. Failurebeforecommit khôngpublish; response-loss retry hoặcstore reopen khôngnhândoubleeffects; Runningrepairclose/reopen→complete vàcancel thậtSQLite.
4. Image2empty/Running/terminal roundtrip; orphan/unknownkeys/profile/hash sai refuse; legacy golden vàregressions giữ.
5. Snapshot/recovery trongRunning tiếp tục đúng, recipeeffectivevaluespin; no source mutation khi compatibilityrefusal.
6. Record/replay Execute+Advance+Drain/cancel rerun thật, checkpoint cảitem/hold/job/events; negativeexpectedhashdivergence fail; không chỉ đọcjournal.

Evidence là headless .NET và SQLite trêntarget được chạy; không Unity/sprite/performance/humanplaytest hoặc fullVS03. G13/earth-monument-2d-v1 giữ nguyên, implementation2D không thuộc sprintcore này. Chưa tự mởSprint15 hoặc release.
