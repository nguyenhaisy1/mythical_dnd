# Sprint14 — Sửa thuyền thật trong headless profile

03/11/2026 · SPIN-038 · **Hoàn tất và nghiệm thu phạm vi VS-03A headless; Git/CI theo WORKLOG**. Sprint13 đã chốt định hướng2D/sprite vàG13Accepted/Reopenable, không yêu cầu chốt lại v1. Đây là bước hiện thực luật trước khi nối client2D; chưa mởSprint15.

## Tổng hợp dễ hiểu

Biến “sửa thuyền” thành một công việc có vật liệu và thời gian thật: giữ gỗ lúc bắt đầu, chỉ tiêu thụ khi hoàn tất, tăng tình trạng thuyền, hủy thì trả phần giữ chỗ. Lưu khi đang sửa rồi mở lại phải tiếp tục đúng, retry không trừ gỗ hai lần. Phần được làm là engine headless, không ảnh/gameplayUI hoặc demoUnity.

## Phạm vi và đầu vào

[ADR023](../../adr/023-bounded-boat-repair-profile.md) và [BoatRepairProfile](../../contracts/BOAT-REPAIR-PROFILE.md) chốt scopeVS-03A. Dùng tham sốprototype đã có:2timber/60tick phút/+200condition/cap1000, khôngRNG. Tái dùng identity/atomicpipeline/scheduler/SQLite/replay; bổ sung typeditem/activity/provenanceholds/capabilitytool và explicitrepairprofile. [Scenario](../../../content/fixtures/boat-repair/scenario.json), [parameterpack](../../../rules/parameters/boat-repair-v1.json) là fixture/authoring giới hạn, không full đời sống hoặc rosterworldbinding.

VS-02full vẫnpartial; sprint này hiện thực repair trên trustedworldfixture độc lập, không nhận đã đạt dependency để đóng fullVS-03. Nghềvậntải/chữatrị, worldinitialization/randomorigin, actorquery và UI chưa triển khai ở scope này. Quyền/qualified/tool input củafixture không phải toàn skills/class progression. G13/profileearth-monument-2d-v1 giữ nguyên.

## Break task và ownership

| Task | Owner | Input → output | Acceptance |
|---|---|---|---|
| S14-01 | /root/s14_repair_domain | Contract→immutableRepairState+pureBoatRepairRules vàtests | Provenancehold/item/activity refs đúng; qualify/tool/location/rights/cap/busy; khôngUnity/IO/globalrandom |
| S14-02 | /root/s14_scope_audit | DomainAPI→StartRepair/CancelActivity/trustedCompleteRepair+typedjobs | Oneatomicpublication, due/revalidation/cancel/retry/failure; transfercarryRepair; externalcompletion vàCancelJob khôngbypass |
| S14-03 | /root/s14_storage_audit | Newprofile→image2/strictcompiler/SQLitemirrors/recovery/replay | Runningrepairreopen→complete/cancel; exactrule/contentrefusal; preservelegacyimage1/goldens/schemaSQL2 |
| S14-04 | /root | FrozenAPIs→headlessprobe/JSONfixtures/CIgate/docs/evidence | Hai process cùng kết quả, negativecontrols, whole regression nonzero/noskip; đồng bộconsumer/exports |
| S14-QA | /root/s14_critic_commands; /root/s14_critic_storage; /root/s14_final_critic | Source/artifacts→findings vàadversarialchecks | Tách repairheadless khỏiUnity/fullgameplay; reviewcuốisausửa |

Hai actorApp/Infra đã auditreadonly trước scope; việc đổi sangimplementation được giao rõ. Tối đa4actor cùng chạy kể cảroot, critics theo lượt, không giả independentreview từselfreview hoặc sốvai chưa thực sự chạy.

## Cổng nghiệm thu

- Startgiữ2khôngconsume; trước due không tăngcondition; duecompletion consumeđúng1lần/gaincap; khôngcanthiệp khôngtự sửa.
- CancelreleasekhôngcộngOnHand, terminalkhôngquayRunning; duplicate/mismatch/stale vàauth/quyền/skill/tool/location/busy/death/overflow cócase thật.
- PublicCompleteRepair/Schedulerforgery vàgenericCancelJob khôngbypass/đểholdrò.
- Image2explicit kểcảempty; strictunknown/ref/malformed rejection, schemaSQLite2khôngbịgánimageversion; khôngauto-upgradeoldsave.
- SQLitefilethậtclose/reopengiữaRunning và failurebefore/aftercommit; snapshot/recovery vàwrongprofile/content refusalgiữnguồn.
- ReplayExecute/Advance/Drain rerunactualrules với cảitem/activity/hold/job/events; negativeexpectedhash phảidiverge.
- Fulllegacytest/golden khôngoverwrite, staticdocs/architecture/referencechecks, rawrun/TRX/negativecontrol evidence vàCIđúngHEAD trướcPRintegrationdev.

Kết quả tại [evidenceSprint14](../../qa/SPRINT-14-EVIDENCE.md):562/562 runtime tests,113 repair,0failed/skipped; process gates/negative controls và review fixes đạt. Ba P1 đã sửa; root đối chiếu full run sau khi critic cuối chạm quota. Scope này được root nghiệm thu theo workflow đã giao, không nhận review là human playtest. KhôngUnity/Player/2Danimation/performance/humanplaytest/fullVS03claim. Khôngmain/tag/release hoặc tự mởSprint15.
