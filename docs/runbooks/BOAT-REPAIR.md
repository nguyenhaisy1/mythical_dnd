# Kiểm tra sửa thuyền headless

## Mục đích và phạm vi

Profile `boat-repair-v1` hiện thực một công việc có vật liệu và thời gian: bắt đầu giữ2gỗ, đợi60phút simulation, hoàn tất tiêu thụ2gỗ và tăng200condition tối đa1000; hủy chỉ bỏ giữ chỗ. Đây là trusted fixture cho engine và công cụ phát triển, chưa có UI chơi game. [ADR023](../adr/023-bounded-boat-repair-profile.md), [contract](../contracts/BOAT-REPAIR-PROFILE.md) và [evidence Sprint14](../qa/SPRINT-14-EVIDENCE.md) tách đặc tả với kết quả kiểm thực.

## Nguồn đầu vào

- `rules/parameters/boat-repair-v1.json`: compiler strict riêng với handler boat-repair-v1; ID/version/recipe/material/quantity/duration/gain/cap và fingerprint. Không thay kernel-content-v1 hoặc legacy content hash.
- `content/fixtures/boat-repair/scenario.json`: World14, tick0/revision1,10timber,boat condition500, seed và expected arithmetic do authoring cấp riêng. Host tạo actor/household/worksite/tool/qualification đáng tin cậy, không nhận đây là random life generator hoặc full settlement binding.
- Recipe qualification bool/tool references là input kỹ thuật giới hạn; không class/skill progression, không cho player chọn origin. Không RNG trong repair.

## Chạy kiểm tra

Từ root repository, dùng SDK theo global.json (đường dẫn dotnet là SDK đã cài trên máy):

```sh
python3 tools/tests/run_runtime.py --dotnet /path/to/dotnet
```

Runner build, chạy suite nonzero/no-skip, negative control có chủ đích và các process gates cũ; gate repair chạy hai C# process với database paths riêng, so JSON/hashes/effects và negative authoring. Source manifest phải không đổi trong run. Không tự chạy lại record legacy golden.

Để chẩn đoán một fixture sau build Release:

```sh
/path/to/dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --repair-probe rules/parameters/boat-repair-v1.json content/fixtures/boat-repair/scenario.json /tmp/mythical-repair-new.db
```

Đường dẫn database, `<path>.cancel` và `<path>.recovered` đều phải chưa tồn tại; probe không overwrite/xóa file cũ. Dùng tên mới cho lượt kế tiếp. JSON trả scope, passed, recipe/content hashes, imageVersion2, sqliteSchemaVersion2, arithmetic, từng check boolean và final image hashes; exit0 chỉ khi tất cả probe checks đúng. Input duplicate key/unknown field bị từ chối, exit1 trước tạo database. Usage sai trả2.

## Probe thực hiện gì

1. Compile authoring profile, tạo trusted world, InitializeSave với explicit repair rule/content, Execute StartRepair; kiểm OnHand10/Reserved2 và due60.
2. Snapshot khi Running, tạo store mới/Load và retry đúng command; nhận receipt cũ. Advance tới59 không tăngcondition; tới60 rồi Drain thực thi completion, OnHand8/Reserved0/condition700.
3. Public CompleteRepair phải bị từ chối; Load file cuối khớp canonical image. RecoverToNewFile tạo output mới, returned state và outputstore.Load phải khớp; legacy rule profile Load bị từ chối mà không đổi source.
4. Nhánh SQLite riêng Start→reopen→Cancel giữ OnHand10/condition500 và hết hold.
5. Record/Replay actual Execute/Drain từ initial world, so final canonical image; đổi expected result hash phải diverge ở action0. Đây là rerun resolution, khác committed-image recovery ở bước3.

Hai process probe so cả arithmetic đã author riêng và hashes; không gọi đó là golden độc lập cho mọi bug. Unit/adversarial tests bổ sung auth/stale/mismatch/failure, recipe/tool/worksite pins, cancel during Advance, cap/busy/death/rights/location revalidation và restore causal history. Xem counters/commands/raw paths của revision cuối trong evidence, không suy Passed chỉ vì file test tồn tại.

## Lưu trữ và sử dụng API

`WorldSnapshot.Repair=null` là legacy image1. Có extension, kể cả rỗng, là image2; physical SQLite schema vẫn2. Store mặc định legacy; repair phải truyền content hash chuẩn và `expectedRuleVersion: SqliteWorldStore.RepairRuleVersion`, processor/ReplayHarness nhận cùng Recipe. Không tự nâng save legacy, không hứa reader cũ đọc image2. [Save Recovery](SAVE-RECOVERY.md) và [Replay QA](REPLAY-QA.md) hướng dẫn hai đường riêng.

StartRepair public chỉ nhận actor_id/item_id/material_lot_id/recipe_id. CancelActivity nhận activity_id/reason. Completion nội bộ wire `ExecuteScheduledRepair(job_id,workflow_id)` do trusted Drain tạo, không phải endpoint public; generic CancelJob repair bị từ chối để tránh rò hold. Accepted receipt/facts/lifecycle phải khớp activity và job khi restore.

## Các gate còn lại

Không có Unity/Player mới, sprite/background2D, actor query/player knowledge projection, full profession/world binding/random protagonist, Health/Travel/forced-rights gameplay handlers, performance/balance hoặc human playtest ở profile này. Trusted fixture thay input để thử revalidation không chứng minh các domain đó đã triển khai. VS02/fullVS03 còn partial. Hướng `earth-monument-2d-v1`, G13 Reopenable và thư viện01–18 giữ nguyên; không mở Sprint15 hoặc release từ việc chạy probe.
