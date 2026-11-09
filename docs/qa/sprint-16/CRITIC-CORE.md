# Sprint16 — Review độc lập core, session và lưu trữ

06/11/2026 · SPIN-040 · Actor `/root/s16_critic_core`.

## Phạm vi và kết luận hiện tại

Đã đọc `.agent`, HANDOFF, APPLICABLE-BASELINE, ADR025, ORIGIN-COHORT-PROFILE, Sprint16 brief và skill `mythical-validation` cùng hướng dẫn phản biện. Review Domain origin/selector/WorldSnapshot, ba đường clone Application, safe DTO/session/gateway, registry derived từ pack, canonical image3, SQLite profile/publication và replay boundary. Reviewer chỉ sở hữu report này và `OriginBoundaryTests.cs`; không sửa production source, không chạy build/tests/Editor/Player/Git.

**Technical source và focused runtime đã review; final full regression/process artifacts còn chờ.** Sau fix đã có92/92 focused cases Passed, gồm4 independent cases đã fail trước sửa. Không còn P0/P1 đã biết trong phần source/focused evidence đã đối chiếu. Đây chưa tự đóng Sprint16, chưa xác nhận actual Unity/render/OS hoặc full viability/gameplay.

## Finding đã xác minh và sửa

| ID | Mức | Bằng chứng và tác động | Trạng thái |
|---|---|---|---|
| S16-CORE-01 | P1 integrity | Store.Commit trước fix chỉ kiểm Origin profile/cohort hash, chưa pin metadata genesis với expected image. Một candidate AdvanceTime hợp lệ có thể thay actor definition, household definition, source hash hoặc Origin draw/cursor; image3 vẫn decode/re-encode hợp lệ và được ghi xuống. Sau đó Gateway recompute assignment không khớp, khiến Continue lỗi. Vi phạm metadata immutable sau genesis. | Đã sửa và focused runtime xác minh: canonical Origin old/new presence + toàn bộ fields phải khớp trước khi mở writer hoặc gọi write probe.4cases fail→pass, file bytes giữ nguyên khi từ chối. |

Bốn cases do reviewer viết độc lập tại `tests/MythicalDnd.Foundation.Tests/OriginBoundaryTests.cs`, tên `S16_02_DurableCommitCannotRewriteGenesisOrigin`. Test tạo đúng next Advance image bằng CommandProcessor trong memory, chỉ thay phần Origin/cursor trong canonical bytes, kiểm image vẫn Decode→Encode hợp lệ rồi yêu cầu Store từ chối trước mutation. Vì vậy failure trước sửa không phải lỗi fixture hoặc malformed codec.

Reviewer tự parse TRX do root thực thi:

| Raw artifact | Total/executed/Passed/Failed/skipped | Kết luận |
|---|---:|---|
|`tests/results/sprint16-focused-initial/origin.trx`|82/82/82/0/0|Focused suite trước independent cases |
|`tests/results/sprint16-boundary-before/boundary.trx`|4/4/0/4/0|Cả4 fail đúng ở Expected InvalidDataException / But null tại durable Commit |
|`tests/results/sprint16-focused-fixed/origin.trx`|92/92/92/0/0|Cả4 independent cases Passed cùng expanded focused suite sau fix |

Không gọi đây là full regression hoặc tự nhận đã chạy các lệnh. Root có build log và source fingerprints riêng; source đổi tiếp phải kiểm phần bị ảnh hưởng.

## Các boundary đã đọc

| Boundary | Quan sát source |
|---|---|
| Eligibility/RNG | Candidate typed immutable; alive/adult/home filter trước RNG; duplicate IDs bị từ chối; sort ordinal. World-ID từ MDC1/SHA2568bytes big endian; context/profile/occurrence/entity/stream cố định; Next cursor persisted. Không global random, wallclock hoặc LLM trong Domain. |
| Historical Origin | Principal/household/home counter1 cùng world, active refs và RNG binding. Origin.Home giữ nghĩa lịch sử, không ép current actor phải luôn sống/tại home. Các clones transfer/repair/scheduler giữ Origin. Không Origin+Repair coexistence. |
| Compiler | FromSettlement dùng registry đã strict compile; chỉ8adult Bến Vạc, exact source hash. Derived records validate IDs/household/home/history/skill/claim refs, sizes và immutable copies. Ordering canonical; source/content hash đổi là compatibility khác. Đây admission engineering, không full Character Viability. |
| Safe DTO | Presenter nhận port/immutable DTO chỉ self name/age/home/household/history/skill provenance/own claim/needs. Không seed/raw registry/world/processor/revision/private stock hoặc other-actor claim. Research notes/definition IDs không được trình bày như ký ức của người chơi; projection đổi sang nguồn ký ức/nhận định bản thân, claim không thành World Truth. |
| Session | Read chỉ project tick; không draw. Advance bounded1..1440, request ID bounded, receipt adoption kiểm type/principal/source/version/correlation/payload và original duration; reused ID đổi intent bị từ chối. Save durable snapshot; Reload load/validate/restore trước swap. Error vocabulary hữu hạn, không exception/path/SQL details. |
| New/publish | Gateway nhận root/registry/entropy từ trusted composition; UI New không nhận seed/candidate/path. FileShare.None writer lock, strict numeric sequence/request filename. Pending create-new → InitializeSave → verified reopen/flush → create-new File.Move publish; cancellation trước publish không đổi catalog. Cleanup chỉ pending operation sở hữu; orphan/unrelated files không bị scan-delete. |
| Retry/Continue | Published request ID tìm lại campaign trước khi xin entropy; Continue dùng published sequence cao nhất. Corrupt/foreign latest fail closed, không fallback tự bốc lại hoặc overwrite. Explicit New campaign khác giữ file cũ. |
| Persistence | Image3 chỉ Origin/noRepair; image1/2 không thêm null field. Store OriginRuleVersion exact và cohort hash pin; canonical decode/re-encode từ chối unknown fields. Canonical image/recovery/snapshot chứa Origin, không sidecar assignment. PhysicalSQLite schema2 giữ. |
| Immutable commit | Sau fix, Store so canonical origin object cũ/mới trước writer, gồm presence và tất cả metadata fields; actor đời sau/location không nằm trong object nên không bị khóa nhầm. |
| Replay/diagnostics | Root bổ sung explicit refusal cho Origin image hoặc OriginRuleVersion trong legacy ReplayHarness v1; không gọi nó là replay implementation mới. StateDiffer đọc canonical image nên Origin/source_hash có trong developer diff; không phải player projection. |

## Trạng thái artifacts còn chờ

Cần đọc final source manifest/TRX/full process probe JSON sau freeze để xác nhận evidence áp revision cuối; focused92 không thay toàn suite. Actual Unity/LinuxMono/render/input/repair15 regression do root thực thi và critic client kiểm độc lập. Không chốt Git/CI/dev hoặc CLOSE từ report này.

## Fingerprints tại lượt review sau fix

Các SHA sau được đọc tại review focused; nếu đổi thì kiểm lại trước kết luận cuối:

```text
4feb409003ba4d6cdd56a68dc41e018683fe11005519a1eac5f8dadb12db58c8  engine/Domain/Origins/CampaignOriginState.cs
c937d3d252f8f780b706881499a1a2758dd5d59d9b6257475c7453f943c67b81  engine/Domain/Origins/OriginCandidate.cs
75b14f8b72cfa97eb59c58029d595bee5844d1a807aff6a0efdebb2a82b20306  engine/Domain/Origins/OriginSelector.cs
fbadb80fd6c14a5cf1401264dbb570c4618ec042659799b88eb7560ea63b7f48  engine/Domain/Kernel/WorldSnapshot.cs
f09bbd88057f2e4d30b8f5f4cfc809af34e3d3188e58094b19270924fc60e932  engine/Application/Origins/OriginCampaignSession.cs
e6d4a5bb786e981aad90f97bf5130059b5e5cf26eb7da4c600fe4d0a759348a0  engine/Application/Origins/OriginClientContracts.cs
3f98d9cebd55be115a14924ea230256d9b29067fb8d464c613a15989aea16b5e  engine/Infrastructure/Origins/OriginCampaignGateway.cs
8da47a513a55c47b1049c3973d4608ff549809fe893ce9d461204a9c1e07db67  engine/Infrastructure/Origins/OriginCohortCompiler.cs
8e5b16c787cb97e37b3587ad18e44056e74e61ca14f63f287958c4cbe1fa4005  engine/Infrastructure/Persistence/CommittedWorldCodec.cs
98a9741940907468b260066cfa1ff2419e7eb635684f52210d02a54d31be2aa2  engine/Infrastructure/Persistence/SqliteWorldStore.cs
a17e975184fa322ea1f349a1f63e5b2f0f7132cb45dc43b4998cface74d1d184  tests/MythicalDnd.Foundation.Tests/OriginBoundaryTests.cs
```

Không có acceptance cho full life generation/viability/TEST001/world NPC binding, nghề sửa thuyền của cohort, fullVS02/VS07, final owner art/UI approval, IL2CPP/crossplatform/performance/balance hoặc human playtest. G13/profilev1 và library01–18 giữ phạm vi riêng.
