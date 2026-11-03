# ADR023 — Sửa thuyền headless có reservation, thời gian và profile lưu riêng

03/11/2026 · Quyết định triển khai giới hạn Sprint14 / VS-03A / SPIN-038.

Theo scope sprint được giao, triển khai [BOAT-REPAIR-PROFILE](../contracts/BOAT-REPAIR-PROFILE.md) thay vì giả repair bằng TransferResource. Recipe prototype2timber/60ticks/+200condition/max1000 từ catalog/kernelparameters; trusted capability/tool inputs không phải profession/life generator đầy đủ. Không thêm RNG hoặc chọn origin.

WorldSnapshot có extensionrepairexplicit, typeditem/activity/hold vàschedulerjobthật; publication atomic, revalidation/cancel không tạo vật liệu. Profile compiler strict riêng, không nới registry/content/golden cũ. Committedimage2 khi bật extension, legacyimage1 giữ bytes; physical SQLite schema2 giữ khi không đổiDDL, exact rule/content compatibility vàsnapshot/recovery/replay được kiểm riêng. Không tự nâng save cũ hoặc claim oldreader đọcimage2.

Các chi tiếtchốtmới: worksite cốđịnh; capabilityqualifiedboolean do trustedfixture cấp; tool phảiusable/controlled/sameplace/exclusive; pinrecipehash/effectiveparams; CancelJobrepair rejectACTIVITY_STATE, CancelActivityatomicadmittedduringAdvance; directCompleteRepairingresskhôngđược gọiinternalpath. Các lựa chọn chỉ áp boundedprofile, không biến unknownfullprofession/health/travel thành behavior chung.

Sprint14 acceptance gồm headlessrealstateeffects, trusted scheduler, SQLiteclose/reopen/snapshot/recovery/compatibility và replayreresolution, cùng fullregressiongiữgoldens. Chưa Unity/Player/2Dproof/randomlife/fullVS03. HướngmỹthuậtG13profileearth-monument-2d-v1khôngđổi. Đồngbộ contracts/backlog/QA/contentparameter/runbook/notes/exports sau implementation; không sửa archive/VERSION/release hoặc tự mởSprint15.
