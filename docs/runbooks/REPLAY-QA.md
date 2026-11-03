# Kiểm tra chạy lại hành động

## Tổng hợp dễ hiểu

Công cụ dùng cùng trạng thái ban đầu và cùng các hành động để kiểm tra game có tính ra cùng kết quả hay không. Nếu lệch, báo cáo chỉ ra bước đầu tiên bị lệch. Nó phục vụ tìm lỗi; không thay bản lưu và không là giao diện xem lại ván chơi.

## Chạy bộ kiểm tra

Lệnh chuẩn: python3 tools/tests/run_runtime.py --dotnet /path/to/dotnet. Runner chạy tests, hai process replay golden riêng, negativecontrol và toàn bộ crash regression. Không cần tạo lại golden.

CLI: dotnet engine/HeadlessRunner/bin/Release/net10.0/Game.Headless.dll --replay <artifact> <expected-content-hash> [world-seed]. Host dùng compiled transfer/scheduler registry và versions đã pin; expected manifest không lấy lại từ artifact để tự xác nhận. Artifact phải có ít nhất một action; max64MiB trước allocate/read, codec strict mdc1 và max10000actions.

Return JSON passed=true/exit0 khi mọi checkpoint khớp. Divergence trả exit1 với action index0-based, category, expected/actual hashes và revisions. Unexpected exception không biến thành pass. stateHash trong CLI là full-image SHA gồm metadata; GameplayHash trong checkpoint loại metadata và bind manifest, không dùng hai hash thay nhau.

## Những gì được ghi

Artifact gồm initial image, explicit manifest và ordered actions. Execute giữ nguyên envelope và trusted principal riêng; DrainAdvance giữ principal/budget. Duplicate/mismatch/auth/stale/noop attempts đều cần giữ; chỉ lấy accepted receipts sẽ mất hành vi. Internal scheduler commands do processor tạo lại, không gửi như external input.

Replay chạy processor mới in-memory, không nối production store hoặc notification sink. Artifact có World Truth và principal giả lập cho QA; không dùng như authorization ticket hay player projection.

Golden content/fixtures/replay-kernel có15actions và fixed checkpoints/checksums. scenario.json là descriptor fixture được băm làm content hash; chưa là content pack production. Expected arithmetic được đối chiếu riêng; hash baseline lần đầu từ implementation, không tự chứng minh mọi field đúng.

## Khi có sai khác

Giữ artifact/manifest/versions/sourcefingerprint và JSON report. Xác định index/category đầu lệch; đối chiếu input, rule/content change và immutable initial image. Không regenerate golden để che failure. Quyết định thay golden phải ghi lý do/version/compatibility và review cùng thay đổi.

--record-replay-fixture <new-path> <content-hash> chỉ tạo baseline fixture đã biên soạn với explicit action list; FileMode.CreateNew, không overwrite. Đây không phải automatic production command recorder. CI tuyệt đối không gọi record. --replay-negative-control thay expected result checkpoint đầu, phải exit1/categoryresult/index0.

## Giới hạn

Golden legacy kiểm transfer/scheduler compiled rules; Sprint14 bổ sung repair profile riêng ở mục dưới. RNG contexts/counters được giữ, nhưng các commands này không draw nên chưa chứng minh stochastic gameplay. Full fielddiff/rule trace đã có profile FND15 ở [Diagnostics](DIAGNOSTICS.md), cần expected image riêng để so field, không suy từ replay hashes; content registry FND14 có profile kernel-content-v1 ở [Content Pipeline](CONTENT-PIPELINE.md); replay golden cũ vẫn pin fixturedescriptor, không tự chuyển sang pack mới. Unity FND16 đã có conformance kernel giới hạn Sprint10; replay gameplay mới và Player vẫn cần acceptance riêng. Replay không thay schema2 committed-image recovery của Sprint6.


## Repair replay Sprint14

[ADR023](../adr/023-bounded-boat-repair-profile.md) bổ sung `repairRecipe` optional khi Record/Replay; manifest phải dùng content hash do RepairContentRegistry compile và exact rule `transfer-resource-v1+scheduler-v1+boat-repair-v1`. Initial image2 chứa repair profile/recipe pins; processor được compose từ Recipe đã chọn ngoài artifact. Legacy arguments/golden image1 vẫn giữ, không ghi lại golden để khớp repair. Artifact version1 giữ vì action/manifest format không đổi.

CLI `--replay` legacy phía trên không tự chọn repair registry. Dùng API Record/Replay với profile explicit hoặc CLI `--repair-probe` mô tả ở [Boat Repair](BOAT-REPAIR.md). Probe ghi Execute(StartRepair/retry/Advance/forged completion) và Drain rồi rerun actual rules; so full canonical image, checkpoints và jobs/hold/item/activity/events. Negative control đổi expected result hash phải diverge index0; không là đọc final journal. Cancel replay và malformed/profile cases còn có tests riêng, xem [evidence Sprint14](../qa/SPRINT-14-EVIDENCE.md). Không có stochastic draw, player projection hoặc Unity acceptance mới từ fixture headless này.
