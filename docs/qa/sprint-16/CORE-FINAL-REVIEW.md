# Sprint16 — Review độc lập cuối về core và lưu trữ

09/11/2026 · SPIN-040 · Actor `/root/s16_core_final_review`.

## Kết luận hiện tại

**Review kỹ thuật core và final runtime/process evidence đạt trong phạm vi `origin-cohort-v1`; không còn P0/P1 đã biết sau các bản sửa được đối chiếu.** Reviewer đã tự đọc source, TRX, JSON, hashes và final SQLite. Đây không tự đóng sprint hoặc thay Git/CI/dev gates.

Reviewer chỉ sở hữu report này, không viết implementation/tests/runners, không chạy Editor/Player hoặc Git. Skill `mythical-validation`, AGENTS/.agent/HANDOFF/APPLICABLE, ADR025, ORIGIN-COHORT-PROFILE và execution contract là tiêu chí đối chiếu. Không gọi việc đọc log là tự thực thi các test.

## Source và ranh giới đã kiểm

- Selector derive world-ID bằng MDC1/SHA256, sort ordinal/deduplicate/filter alive adult home Bến Vạc trước draw. Context characters/origin-cohort-v1/genesis/principal/assignment cố định, cursor persisted. CSPRNG chỉ tại Infrastructure để chọn campaign seed; Domain không quyết định từ wallclock/global random/Unity/DB.
- Origin immutable metadata riêng với current actor state; WorldSnapshot không cho Origin và Repair cùng tồn tại. Ba clone paths Application giữ Origin. Origin.Home không ép actor luôn sống hoặc luôn ở nhà trong Domain; bounded gateway kiểm current home vì profile hiện chỉ có wait.
- Registry chỉ nhận tám authored candidates và refs/history/skills/claims hợp lệ; compile tạo immutable self record. DTO chỉ own record, claim không thành World Truth; nghiên cứu/raw definition IDs/seed/processor/private stocks không đi vào safe presenter. Source ký ức/nhận định là provenance theo episode của chính người đó.
- Session giữ processor riêng, request bounds và original envelope; intent thay đổi với request cũ bị từ chối. Read không draw; Save snapshot và Reload chỉ swap sau verify/restore. UI không ghi authoritative state.
- Gateway dùng pending create-new, verify/flush rồi File.Move create-new cùng filesystem; root writer lock và request idempotency trước entropy. Cancel/fault trước publish giữ Continue cũ; pending crash orphan được ignore/giữ, không scan-delete. Sau publish không xóa campaign. Catalog duplicate sequence/request/zero fail closed, latest corrupt không fallback/reroll.
- Codec image3 chỉ Origin/noRepair; legacy image1 và repair image2 không thêm null extension. Store pin rule/content exact và Origin metadata cũ/mới trước writer. PhysicalSQLite schema2 giữ. Recovery/snapshot chứa canonical Origin; replay artifactv1 từ chối Origin/image3 và OriginRuleVersion thay vì giả đã có command replay mới.

S16-CORE-01 từ review trước đã được sửa bằng IMMUTABLE_ORIGIN. Reviewer kiểm lại cả11fingerprints trong [report trước](CRITIC-CORE.md): **11/11 khớp nguồn hiện tại**. Bốn test độc lập actor/household/source/draw trước fail đúng và sau pass được giữ; không sửa golden để bỏ lỗi. Report này không nhận ownership của bốn test do actor khác viết.

## TRX và source manifest thực tế

Đọc trực tiếp `tests/results/sprint16-1rngpl9l/runtime.trx` và runtime.log: **674discovered/executed/Passed,0failed/skipped**, thời lượng test5phút20giây. Task prefixes: S16-01=30, S16-02=48, S16-04=3; **81test mới +593regression cũ**, gồm113repair. Focused92 trước đó gồm81mới và11regression, không phải92test mới.

Manifest `sources.json` của full run có **286 paths**; reviewer tự hash từng path, **0 khác/thiếu** và tự tính fingerprint khớp `source_revision`:

```text
sha256:6b3de7c07749c127d0c26262b9d27497644e6aad699060ae42b1c8fd7ae03665
```

Final `evidence.json` có exit0, không error; negative TRX đúng một test được thực thi và Failed, child exit1 như yêu cầu. Runner thực sự đi qua transfer/scheduler demo, persistence/save crash, replay/content/diagnostics/conformance preparation/settlement/event eligibility/boat repair và origin process gate; không lấy riêng runtime.trx thay toàn bộ runner. Build log Release có0warnings/errors. Đây full headless acceptance trên nguồn được khóa, không là cross-platform hoặc full gameplay acceptance.

## Process evidence trên nguồn cuối

`tests/results/sprint16-1rngpl9l/origin-cohort.json` có Passed, **21 child calls và ba SIGKILL exit−9** đúng `after_initialize`, `before_publish`, `after_publish`. Lượt riêng ban đầu `tests/results/sprint16-origin-process-first.json` được giữ và không dùng thay lượt final. Source process checker được đọc: quan sát barrier ready file trước kill, kiểm hashes campaign cũ/pending orphan/retry, không chỉ kiểm child exit.

Hai fresh processes seed0 khác path/request có cùng initial hash `9a8913af9fdfdf2bcc1dc2712d3b7a9fc94390174b17a432cf3931f0faea9061` và final hash `998f49dcc050e2463b06880d10efe8f397a233499e6be81751988b774b10e804`. Seed1 khác actor/world/hash đúng known answer. Invalid seed/request không publish; corrupt latest trả storage_error, không fallback và giữ bytes.

Reviewer mở SQLite read-only trong raw `tests/results/sprint16-1rngpl9l/origin-process-17kwnzwg/`, dùng Python MDC1 decoder độc lập với C# codec:

| Crash point | Published DB sau retry/Continue | Pending orphan |
|---|---|---|
| after_initialize | Lâm/tick61 và Nhu/tick61, cả hai image3/draw1, image SHA khớp stored hash |1file còn giữ|
| before_publish | Lâm/tick61 và Nhu/tick61, cả hai image3/draw1, image SHA khớp stored hash |1file còn giữ|
| after_publish | Một DB Nhu/tick0/image3/draw1, image SHA khớp stored hash |0file|

Bằng chứng này xác nhận pending không lộ thành Continue và commit sau publish không bị mất khi process bị kill. Đây crash process evidence, không power-loss hoặc origin command replay.

Actual Unity/renderer/OS thuộc [review độc lập client và artifacts](FINAL-INDEPENDENT-REVIEW.md): lượt8fpgghix và22lns4xz có scope/LinuxMono và source hashes riêng. Report core này không nhận visual inspection hoặc human playtest chưa tự làm. Git/CI/docs/exports là gate root phải hoàn tất sau local technical evidence.

## Phần chưa nghiệm thu ngoài phạm vi

Không có kết luận full causal life generation/Character Viability/TEST001, toàn NPC/economy/Knowledge binding, profession progression/boat repair cho cohort, fullVS02/VS07, IL2CPP/cross-platform/performance/balance/human playtest hoặc final owner art/UI approval. Giữ G13 Accepted/Reopenable và thư viện01–18. Không tự mở Sprint17/main/tag/release.
