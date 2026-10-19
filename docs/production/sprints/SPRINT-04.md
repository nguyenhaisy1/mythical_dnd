# SPRINT-04 — Scheduler và tiến thời gian có kiểm soát

Mở 2026-10-16, SPIN-007. User giao Sprint4 và xác nhận workflow AI thường trực. Branch feature/sprint-04-scheduler từ dev06f5dd7. Sprint3 đã có138tests/CI/review và causal events; input này không phải durability evidence.

## Mục tiêu và phạm vi

Hiện thực FND-10 phần scheduling/AdvanceTime trong bộ nhớ, dùng TransferResource làm job thực thi qua rule/invariant hiện có. Jobs/workflow/clock/identity/events/receipts publish trong cùng immutable aggregate; không tạo một nguồn world thứ hai. Kết quả phục vụ FND-11 persistence, chưa nghiệm thu mục “Durable” trong tên backlog.

Không hiện thực repair/Health death/combat/legacy hoặc UI. Test breakpoint/followup qua approved pure handler là kiểm kernel policy, không phải các domain gameplay đó. Quyết định API/prototype bổ sung tại [ADR013](../../adr/013-in-memory-scheduler.md).

## Planning và phân công thực tế

| Task | Actor | Input/output | Acceptance |
|---|---|---|---|
| S4-01 contract/coordination | /root | Time/command contracts → ADR013, task boundaries | Chốt API/version/unknowns; ghi giới hạn |
| S4-02 scheduler/pipeline | /root/s4_scheduler | Immutable world/event pipeline → jobs, workflow, internal commits | Ordering/deferral/cancel/revalidation; failure atomic; pending receipt idempotent |
| S4-03 tests | /root/s4_tests | Contract + API thống nhất → SchedulerTests | Insertion order, budget/fanout/cycle, breakpoint, fault/retry/auth/bounds |
| S4-04 independent review | /root/s4_review | Code/tests → findings | Adversarial boundary review; fix blocker trước acceptance |
| S4-05 integration/QA | /root | Runtime + demo + docs → evidence | Full regression/discovery/negative control; docs/export checks |
| S4-06 Git/handoff | /root | Reviewed revision → PR/dev/CI/report | Verify remote SHA và CI; không main/tag/release |

Đây là planning bằng AI với actor thực, không bịa họp/người ký. Implementation và test agent trao đổi API trực tiếp; reviewer không là implementer. Root giữ ownership docs/runner/CLI/Git.

## Acceptance cam kết

1. Total order due/phase/priority/creation-sequence/job-ID. Cùng tick phase nhỏ/hòa được defer+trace; past/range/sequence overflow reject. Followup tối đa32 và causal cycle được chặn atomically.
2. Advance có jobs: request revision+1 giữ tick, từng job commit tick/effects/job status cùng nhau, closing revision+1. No-job chỉ một clock commit; cùng tick không due là no-op. Retry request trả receipt gốc, workflow query riêng.
3. Budget tối đa1000 jobs mỗi invocation; pause không drop job hoặc nhảy tới target. Failure trước publication giữ job để retry; mất response sau commit không execute lần hai.
4. Job transfer revalidate quyền/liveness/available/lot location bằng rule+invariant đang áp dụng; failed job không thay vật chất. Cancellation có quyền và lifecycle rõ.
5. PauseDecision không tự resume bằng drain. Prototype acknowledgement command có kiểm owner/workflow/status; không tuyên bố resolve gameplay death/combat.
6. Full headless regression no skip; review độc lập không còn blocker; evidence và docs đồng bộ. TEST008/028/033/038 chỉ ghi pass phần thực thi tương ứng, không suy gameplay/durable gate.

## Kết quả nghiệm thu — 2026-10-19

Hoàn tất scope in-memory sau gián đoạn usage limit.184/184headless tests (138regression+46scheduler),0skip; negativecontrol đúng; transferdemo và schedulerdemo đều đạt. /root/s4_review độc lập không còn blocker sau sửa catch publication/correlation. [Evidence](../../qa/SPRINT-04-EVIDENCE.md) ghi fingerprint/commands/artifacts và giới hạn. Các bước Git/CI cuối nằm trong WORKLOG. NextFND11; không mởSprint5.
