# Origin cohort profile — origin-cohort-v1

06/11/2026 · Sprint16 / SPIN-040 · ADR025. Phạm vi VS02C và một phần onboarding VS07.

## Kết quả cần có

New campaign → Generating → Reveal → World tại Bến Vạc → chờ/lưu/mở lại/Continue. Nhận đúng một nhân vật đã biên soạn, chọn theo seed; không chọn origin/stat/class, không reroll ở Reveal. Campaign mới được tạo riêng và giữ các campaign trước. M07 mục đầu cho phép chọn nhân vật hợp lệ từ world seed hoặc sinh mới; sprint này thực hiện nhánh chọn roster, không gọi nó là full causal life-history generator.

Admission kỹ thuật: alive, age>=18, household có home community.ben-vac@1, history/skills/claims refs đã qua SettlementCompiler. Pack hiện có8candidates. Đây không phải full Character Viability/TEST001: chưa đủ passive/3relationships/survival routes. Không sửa pack Sprint11 hoặc suy routine/viability prose là hành động executable.

World proof chỉ bind selected actor, household và Bến Vạc; không bind toàn24NPC/economy. Các actor Bến Vạc không có boat-repair skill; không cấp qualification/quyền tài sản giả. World cho đọc bản thân, chờ1/60phút, save/reload và trở về menu. Luồng repair-client-2d-v1 Sprint15 giữ riêng bằng scene/CLI cũ và phải regression actual Player; không nhập origin world vào factory15.

## Core và RNG

Game.Domain.Origins chứa typed immutable OriginCandidate, CampaignOriginState và OriginSelector, không Unity/DB/network/wallclock/globalrandom.
OriginSelector.DeriveWorldId(seed): SHA256(MDC1 object {profile:"origin-cohort-v1",purpose:"world-id",seed:seed}); đọc8bytes đầu big endian thành UInt64; nếu0 dùng1. Seed là64lowerhex từ trusted composition; entropy khởi tạo campaign do Infrastructure CSPRNG, không phải quyết định gameplay.
Principal được cấp actor counter1 trước draw; household1 và settlement1 tại genesis. Candidate IDs sort ordinal và unique, lọc eligibility trước RNG. Context chính xác: domain characters, decision origin-cohort-v1, occurrence genesis, entity principal.Value, stream assignment. Uniform(cohortCount) của sha256-counter-v1; persist Next cursor dưới key origin-assignment. Read/Reveal/Begin/Continue/save/reload không draw.

OriginCandidate constructor: (string actorDefinitionId,string householdDefinitionId,string communityDefinitionId,bool alive,int age). OriginSelector.Select(string seed,string sourceContentHash,string cohortContentHash,IEnumerable<OriginCandidate> candidates) trả OriginSelection gồm Selected, Origin và Rng (RngStreamState). OriginSelection.Origin là CampaignOriginState với principal/household/home IDs, selected definition IDs, source hash, cohort hash, draw index. Hằng ProfileId="origin-cohort-v1"; không chứa biography tự sinh hoặc sửa source.
WorldSnapshot thêm optional Origin cuối constructor, không đồng thời Repair. Validate world refs/principal/home/RNG context/cursor; mọi candidate clone trong CommandProcessor/Scheduler giữ Origin. Metadata origin immutable sau genesis.

## Persist và compatibility

CommittedWorldCodec image3 chỉ khi Origin!=null, thêm origin root; image1/2 bytes/goldens giữ nguyên. Image3 không Repair. Strict decode/re-encode/unknownfields/ref checks. Origin được persist trong canonical image/recovery/snapshot; không sidecar biography/assignment. Replay artifact v1 hiện chưa hỗ trợ origin profile: Record/Replay phải từ chối Origin/image3 và OriginRuleVersion, không nhận nó như legacy image. Save/reopen/recovery và deterministic genesis trong sprint này không là origin command-replay acceptance.
SqliteWorldStore.OriginRuleVersion="transfer-resource-v1+scheduler-v1+origin-cohort-v1". Newprofile requires Origin/nonRepair và exact Origin.CohortContentHash==store contentHash; old stores phải từ chối. PhysicalSQLite schema2 không đổiDDL. Factory16 load kiểm exact source/cohort/profile, manifest seed==RNG seed, DeriveWorldId và recompute assignment, không regenerate thay save đang có.

Trusted gateway tạo một published DB/campaign: origin-<10digit sequence>-<32lowerhex requestId>.db trong root riêng. Pending .pending-<guid>.db không enumerate thành campaign. Một create lock root (FileShare.None) cho writer; sequence chỉ ordering catalog, không quyết định gameplay. New cùng requestId trả campaign đã publish (idempotent), không draw/create thêm. Continue lấy published sequence cao nhất; corrupt/incompatible latest fail closed, không fallback/reroll. New rõ ràng vẫn được tạo campaign khác và giữ file cũ.
Tạo pending bằng create-new, InitializeSave, verify reopen và flush, cancellation/probe trước publish, File.Move create-new cùng filesystem. Success chỉ sau publish + verified reopen. Fail/cancel trước publish không thêm published file hoặc đổi Continue; cleanup chỉ pending do operation sở hữu. Crash pending được giữ/ignore, không tự xóa foreign files. Sau publish, cancellation không xóa world đã commit.
Probes cho tests: before_initialize, after_initialize, before_publish, after_publish. QA crash-beforepublish phải không lộ halfsave; new retry không overwrite bất kỳ existing/corrupt/foreign file.

## Authoring resource và safe projection

OriginCohortCompiler.FromSettlement(SettlementRegistry) trích8records đúng policy; source_hash là canonical full settlement hash. Derived document gồm profile/source_hash/records và typed serializable fields, có display_name, household/home label, self histories/skills/known claims/needs. Compile(document) validate IDs/limits/refs, immutable deep copy, canonical hash sorted records. Không runtime LLM; không sáng tác facts mới.
Public serializable OriginCohortDocument dùng Unity JsonUtility; Headless export từ full authored pack và CI so resource exact để không hand-edit generated roster. Resource không được presenter load/enumerate, chỉ trusted bootstrap.
Reveal DTO chỉ own name/age/household/home/history, skills có source episode, known claim có provenance và own needs. Claim là nhận định, không World Truth. Không seed/request IDs/raw registry/processor/world/revision/stock/otheractor claims/plans trong DTO. Không hiển thị attribute/passive còn chưa grant. Origin session giữ processor private; finite error codes không exception/path/SQL details.

## Port đã khóa

Game.Application.Origins:
IOriginCampaignGateway.ReadMenu(): OriginMenuView; NewCampaign(string requestId,CancellationToken): OriginOpenResult; Continue(): OriginOpenResult.
IOriginCampaignSession.Read(): OriginRevealView; AdvanceBy(long minutes,string requestId), Save(), Reload(): OriginResult.
New UI không có seed/candidate/path input; trusted tests có thể inject Func<string> seedFactory và Action<string> publicationProbe vào Infrastructure gateway. Session Advance1..1440 qua Execute/Drain, original-envelope idempotency theo requestId, không tự tăng tick bằng frame time. Save là durable snapshot, Reload latest committed state.

Game.Infrastructure.Origins:
OriginCampaignGateway(string root,OriginCohortRegistry registry,Func<string>? seedFactory=null,Action<string>? publicationProbe=null).
OriginCohortCompiler.FromSettlement và Compile; document/registry types namespace này. ReadMenu chỉ HasCampaign/error-safe status, không reveal candidate trước New. Begin chỉ đổi screen presentation, không mutate Origin; Continue hiện lại cùng Reveal rồi Begin vào World tại tick đã lưu. Journal bản thân là cùng self-known record, không full journal/director.

## UI/render

Giữ art profile earth-monument-2d-v1 và assets15. Chỉ scene Bến Vạc vì mọi candidate home tại đây. Dùng marker vị trí trung tính trên nền, không lấy sprite một người rồi nói đó là đủ tạo hình mọi tuổi/giới. Menu/Generating/Reveal/World, back-to-menu/Continue và self-record; fullscreen/modal focus, no clickthrough, visible keyboard focus, scroll long biography, font Việt và100/125/150% ở1280x720/1920x1080.
Generating async; cancel best effort trước atomic publish. Disable duplicate New khi pending; giữ requestId qua retry. Không fake progressstage hoặc gameplay nút chưa thực thi. Generic marker chỉ presenter, không Travel/collision/world position.
Actual LinuxMono Player và real OS-input cần chứng minh. Existing Sprite/repair proof15 không bị thay bằng origin-only proof. User final art/UI approval và humanplaytest vẫn backlog, không tự suy đạt từ technical review.

## Gates đóng Sprint16

1. Selector/registry/admission/deterministic known answers + ordering/empty/dead/minor/duplicate + privacy/noRNGonRead + original-envelope cases.
2. Actual SQLite create/reopen/Continue/new-idempotency/cancel/fault/crash-beforepublish, exact image3/profile/refusal; old image1/2/goldens và repair15 giữ.
3. Actual Unity6000.6.7f1/LinuxMono menu→Reveal→World→wait/save/restart/Continue cùng assignment/cursor; hai seeds khác nhau trong QA để chứng minh khôngfixed.
4. Renderer six resolution/textscale cases, scroll/focus/modal/cancel/pointer/keyboard real; technical neutral marker đúngclaim. Actual repair15 Player regression.
5. Independent source/artifact review noP0/P1, final fullheadless/source/docs/exports/CI exactHEAD và feature→PR→dev.

Không chuyển gate chưa đạt sang backlog để đóng. Không tự Sprint17/main/tag/release. Full life generation/viability, world/NPC/economy binding, skills progression/repair for cohort, fullVS02/VS07, art approval/performance/humanplaytest là backlog dự án ngoài sprint.
