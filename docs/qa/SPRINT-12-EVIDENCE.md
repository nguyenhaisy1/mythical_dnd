# Sprint12 — Evidence điều kiện sự kiện

## Tổng hợp dễ hiểu

Tám tình huống trong thiết kế đã có dữ liệu và code kiểm điều kiện: ai được xét, vật chất có đáp ứng không, nhân vật đã biết đủ chưa và có đang trong khoảng chờ không. Đây là bước chuẩn bị cho tình huống có nguyên nhân rõ ràng; chưa cho NPC thực thi hoặc tạo bản chơi thử.

SPIN-015; VS02B; ADR021. Full gate đạt; tích hợp Git/CI ghi trong WORKLOG khi hoàn tất.

## Phân công và review

- /root/s12_content: tám templates, schema, 23 fixture scenarios và giải thích nguồn. Local JSON Schema validation và host fixture run đạt.
- /root/s12_eligibility: compiler/evaluator portable, context readonly, 78 test cases. Sửa lỗi generic NUnit Using<string> trước focused run.
- /root/s12_review: review độc lập source/content/contracts, focused run 78/78 passed, không failed/skipped tại `/tmp/s12-independent-review/s12-independent.trx`; review integration cuối đạt sau gián đoạn quota, không còn blocker.
- /root: host CLI, process negative cases và independent Python hash reference, integration gate, ADR/runbook/forecast/docs/Git.

## Phạm vi bằng chứng

Event content hash `a68192c085f9efaab2eb5cf73cf57a3a22d5b5072f31cd903237a6bb618f0a95`; settlement binding giữ `60ce05463edcc1c2e4abbeae8f30159d430f57613b902ec6d48bb775db193ec8`. Tám templates và 23 observations do test cung cấp; không gán chúng là world snapshots thực.

Chưa kiểm mới trên Unity: portable assembly build không là Unity verification. Không có gameplay effects, occurrence persistence, authoritative observation adapter, NPC autonomous consequences, random protagonist hoặc client UI. Test knowledge ở đây kiểm scope allowlist/Unknown, không thay TEST004 end-to-end player knowledge.

[Lộ trình](../production/PLAYABLE-ROADMAP.md) là forecast mới: Sprint13 hình ảnh/UI prototype, Sprint15–16 demo nhỏ nếu đạt dependencies; full vertical slice chưa cam kết số sprint.

## Full runtime gate

Lệnh `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`, exit0. Artifact `tests/results/sprint12-ui45g8mh/evidence.json`: **449/449 passed, 0 failed/skipped**, gồm78VS02B và371regression. Build Release0warnings/errors; negative-control cố ý fail đúng1test.

Source fingerprint `sha256:d11f305c9f94d3069aca515cf6272ce0a645ca386968a875964c84037aaabf9f`. Gate xác nhận sources không đổi trong lượt chạy; manifest ở cùng thư mục. Event gate20processes/17negativecases,23scenarios, independent Python mdc1 hash và cross-process determinism đạt. Existing settlement/replay/content/diagnostics/transfer/scheduler/conformance-preparation và9SIGKILL persistence/recovery regression đạt. Conformance preparation không là lượt Unity mới.

Docs:36generated files và578authored links, architecture boundaries, reference fixtures/vectors đạt; exports regenerate và freshness check trước commit. Kết quả Git/CI cuối ghi append trong WORKLOG, không giả self-containing commit hash.
