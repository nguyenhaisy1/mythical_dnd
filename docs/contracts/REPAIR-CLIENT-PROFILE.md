# Repair client profile — `repair-client-2d-v1`

03/11/2026 · ADR024 · Sprint15 / SPIN-039.

## Phạm vi và nguồn truth

Một proof kỹ thuật ở bãi sửa thuyền Bến Vạc, một actor cố định từ trusted fixture, một boat/tool/material lot. Actor không do người chơi chỉnh origin, class hoặc stat. Dòng “Bản thử kỹ thuật — nhân vật cố định” luôn hiện. Law/state/persistence tiếp tục là BOAT-REPAIR-PROFILE, không viết lại trong MonoBehaviour. Không nhận fixture là random life hoặc full settlement simulation.

## Port cho presenter

`Game.Application.Projection.IRepairClientSession` chỉ trả immutable DTO/primitive:

| Method | Input | Output và ràng buộc |
|---|---|---|
| Read | Không actor/path tùy ý | RepairPlayerView, chỉ field được grant |
| PreviewRepair | Không quantity/duration/gain | Cost/time/cap known, uncertainty và opaque token; pure |
| ConfirmRepair | previewToken,requestId | Safe result; private pinned envelope/revision, stale yêu cầu preview mới |
| Retry | requestId | Replay đúng original envelope/receipt, không double reserve/consume |
| CancelOwnRepair | requestId | Bind own activity; lý do cố định hợp lệ từ session |
| AdvanceBy | minutes,requestId | WorldTick qua AdvanceTime/Drain; không qua frame time |
| SaveSnapshot / Reload | Không filesystem input | Actual durable snapshot / load latest committed state; sanitize failures |

DTO không có raw revision/counters, seed, entity ngoài scope, OriginalEnvelope, full journal/jobs, hidden plans/containers hoặc exact hidden probabilities. Request IDs do client tạo để retry không phải gameplay RNG. Trusted constructor/callbacks nhận raw types chỉ ở composition, không truyền chúng vào presenter.

## Observation và dữ liệu chưa biết

Trusted `RepairObservationScope` bind actor/worksite/boat/lot/tool, labels và flags ActorStatus/BoatPresence/BoatCondition/MaterialOnHand/MaterialReserved/ToolUsable/Qualification/OwnActivity/Recipe. Grant All của demo là direct observation liên tục tại worksite, source `direct-worksite-fixture`, observedTick từ lần đọc. Đây là quyền quan sát explicit, không suy từ ownership. Field bị deny phải null/absent; derived Available cần cả OnHand và Reserved đã biết. Không grant boat thì không pick nó. Preview thiếu prerequisite known không gọi truth evaluator để bật/tắt nút hoặc lộ hidden rejection reason.

Fixture negative gồm actor/private lot/boat khác; thay giá trị ngoài grant không làm xuất hiện entity, tên, số lượng hoặc hidden reason trong DTO/preview. Chính sách này chưa thay full perceived knowledge/provenance storage, NPC knowledge hoặc TEST004 toàn game.

## Scene và tương tác

- Scene foundation1920×1080, world plane16×9units, PPU120, orthographicSize4.5. Camera fixed; không pan/zoom trong profile này. Runtime sprite/foreground/boat tách khỏi map; final anchors lấy từ asset manifest và được tập trung trong scene spec.
- Một ground surface và bounded local lane `x[-4,2],y[-2.7,-0.6]`. Footpoint là presenter data, không persisted world entity position/Travel hoặc gameplay permission. Có hai vị trí trước/sau occluder để kiểm sorting. Không collision/range rule suy từ màu pixel.
- UI Toolkit với text/font Việt thật: observed state, preview/confirm, start/cancel, wait, save snapshot/reload, safe result. Preview modal chặn click cảnh và action phía sau; Esc đóng modal, Tab có focus; text scale100/125/150% phải không cắt chi phí/Confirm ở1280×720 và1920×1080.
- Picking chỉ chọn visible bound boat; hidden fixtures không có scene nodes/DTO/picking targets. Nhân vật dùng hai frame idle; khi projection là Running thì đổi cadence và tilt quanh foot pivot để báo đang làm việc. Đây activity feedback, chưa là full authored working animation. Boat variant theo condition đã quan sát; vòng animation không commit.
- Save root mặc định dưới Application.persistentDataPath với tên/profile riêng. QA có CLI trusted root mới; không có nút nhập đường dẫn, xóa/overwrite world cũ hoặc reroll. Initialize chỉ khi file chưa có; incompatible/corrupt báo lỗi an toàn.

## Gates đóng Sprint15

1. C# tests: grants/Unknown/hidden invariance; Read/Preview không đổi canonical state/receipt/RNG; stale/retry/id mismatch và sanitized failure; start59→60/cancel/wait/save/reload.
2. Actual Editor tạo project/version/package lock/meta, import/compile và tạo scene/build Linux Mono không lỗi. Staging DLL/native dependencies tái lập, không commit DLL/Library/Temp/build.
3. Player chạy thật: same session API và luật Sprint14; SQLite save khi Running→reload→complete/cancel, no duplicate effects; profile/image/schema compatibility giữ.
4. Real rendered captures tại1280×720 và1920×1080; actor feet/occlusion/boat/UI đủ đọc. Phân biệt preview mockup với screenshot renderer và callback automation với OS keyboard/mouse.
Bổ sung gate4: Callback autoplay cô lập input vật lý; sau rendered captures phải kiểm lại completed state và đọc final SQLite entity/activity/receipt counts. OS-input chạy riêng, native submit theo focus; Space chỉ thực hiện một lần chờ, không kích hoạt thêm nút đang focus.

5. Independent source/artifact review không còn P0/P1 trong scope; headless regression/goldens/docs/export checks và CI đúng HEAD đạt trước tích hợp dev.

Full random life/knowledge/world binding/three professions, PlayerIL2CPP/cross-platform, final art/UI approval, performance/balance/human playtest là công việc dự án tương lai, **không phải điều kiện đóng Sprint15**. Không được chuyển bất kỳ gate1–5 chưa đạt thành backlog để tự đóng sprint.
