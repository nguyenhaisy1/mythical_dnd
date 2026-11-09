# Sprint16 — phản biện độc lập client/UI và bằng chứng Player

06/11/2026 · SPIN-040 · Actor `/root/s16_critic_ui`.

## Phạm vi và trạng thái

Đã đọc .agent/HANDOFF/APPLICABLE, ADR025, ORIGIN-COHORT-PROFILE, Sprint16 brief, skill mythical-validation và game-design review. Review source bốn file Origin Unity, root build/render runner và derived resource. Actor này chỉ sở hữu report này; không sửa production, chạy Editor/build/tests, thao tác Git hoặc tạo image.

**Draft: source review, chưa nghiệm thu actual Player/render/OS.** Không có P0/P1 được xác nhận từ source ở lượt này. Hai điểm UI/error-path và một thiếu hụt evidence đã gửi root để sửa/kiểm lại. Actual renderer/layout/input và startup/restart phải đọc từ artifacts sau run; không nhận code có callback check là runtime đã đạt.

## Findings và yêu cầu đối chiếu

| ID | Mức / loại | Quan sát tại source draft | Trạng thái |
|---|---|---|---|
|S16-UI-01|P2 UX/error-path|ShowMenu dùng ReadMenu.HasCampaign nhưng bỏ ReadMenu.Code. Cold start với latest save corrupt/incompatible trả HasCampaign=false, LastResult=null nên không có thông báo lỗi; trông như chưa có cuộc đời đã lưu.|Root đã yêu cầu worker hiển thị finite safe menu error, giữ file và New explicit. Chờ review patch/runtime; chưa gọi là lỗi mất dữ liệu.|
|S16-UI-02|P2 rủi ro layout, chưa xác nhận render|Header cao theo font scale nhưng content.top cố định112. Ở150% title/caption có thể giao card/content; RevealLayoutFits chỉ kiểm card/footer/scroll/Begin trong root, không đo giao header.|Root đã yêu cầu adaptive header reserve/bounds check trước build. Cần nhìn actual1280/1920×150%, không kết luận clip từ source đơn lẻ.|
|S16-UI-03|Evidence gate3|Runner ban đầu chỉ in-process BackMenu→Continue, sau Player exit gọi .NET --origin-open. Đường sau chứng minh DB/cross-runtime reopen, không phải Player process restart.|Root đã nhận yêu cầu OS driver phase2 thực sự khởi động binary mới cùng save root, Continue→Reveal→World và so actor/record/tick/cursor. Sửa wording host reopen cho đúng, chưa đóng khi chưa có artifact.|
|S16-UI-04|Evidence phân loại|InputProbe FinishInput.passed chỉ là có nhận ít nhất một input marker. Không đủ tự nó chứng minh mọi phím/chuột/cancel/Continue đúng.|Root driver cần kiểm độc lập chuỗi actions/focus/screens/results/finalSQLite; không lấy telemetry Passed thay toàn bộ gate.|

Đây không phải preference mỹ thuật hoặc yêu cầu mở rộng ngoài sprint. UI01 không nói existing save bị xóa; gateway/catalog review và crash gates thuộc critic core. UI02 chỉ là nguy cơ cần render/bounds kiểm; chưa nâng P1 khi chưa quan sát tác động thật.

## Source boundaries và thao tác đã đọc

- Bootstrap là trusted composition duy nhất load OriginCohortDocument/resource, compile registry và tạo gateway bằng root riêng. View/Autoplay nhận safe Application ports và DTO; không raw world/seed/registry/processor hoặc filesystem path trong presenter.
- New chỉ từ Menu, giữ requestId khi retry storage failure và khóa duplicate khi generationTask có mặt. Task chạy async; Cancel gửi cancellation token và release QA barrier. Gateway quyết định publish/rollback, UI không tự xóa hoặc viết world.
- QA seed/publication barrier được bootstrap chặn khi không có autoplay/InputProbe. Barrier giữ operation tại before_publish của pending DB thật, không tạo fake gameplay delay ở luồng bình thường.
- Reveal có own age/name/household/home/history/skills-source/known claims/needs; không chọn/chỉnh origin hoặc reroll. Menu New explicit là campaign mới theo ADR008, không phải Reveal reroll.
- Begin chỉ đổi screen, Current vẫn đọc cùng session. World chỉ wait/save/reload/self record; không xuất hiện sửa thuyền/qualification giả hoặc NPC/economy bindings chưa triển khai.
- Enter/Space do native Button submit; Update chỉ quan sát markers. Không có global Space wait đồng thời click nút focus như bug Sprint15. Chuột đi qua native Button, không có legacy scene mouse-down mở modal.
- Self-record modal disable worldPanel, tập trung focus CloseRecord; Advance/Save/Reload guards khi RecordOpen. Escape ưu tiên CancelGenerating/CloseRecord rồi mới BackMenu. Autoplay physical input isolation riêng, callback background NavigationSubmit probe có exemption để vẫn kiểm disabled guard.
- ScrollView dùng runtime Theme + fontNotoSans có nguồn Sprint15, viewport clip/focus-scroll. Neutral marker là nhãn vị trí, không nhận Sprite người đàn ông chung là tạo hình đủ8characters. Foundation reuse là generated2D Sprint15; không model3D hoặc asset mới được tạo trong review.

## Evidence cần có trước kết luận cuối

1. Actual Editor6000.6.7f1 import/build LinuxMono, runtime measured version/platform/backend, source manifests trước/sau build/run.
2. Hai seed QA độc lập ra Vạc Lâm và Vạc Nhu theo known answers; cancel-beforepublish không tạo active campaign, finalSQLite image3/assignment/cursor và command counts đúng.
3. Sáu resolution/textscale cases có top/bottom captures và geometry/scroll checks; nhìn ảnh actual header/caption/footer/Begin, own-record và World. Black bars/window-manager effects phải ghi đúng dimensions, không sửa/crop screenshot để làm evidence.
4. Normal OS keyboard/mouse/native focus/cancel/preview, plus actual app restart cùng save root. Kiểm actions và persisted truth riêng; callback automation không là OS input, automation không là human playtest.
5. Actual repair15 Player regression do root thực thi. Full regression/docs/export/CI/Git do root nghiệm thu; report client không tự đóng sprint.

## Fingerprints tại draft source review

Source có thể thay đổi khi worker sửa findings; chỉ reuse kết luận cho phần còn đúng và đối chiếu patch/new runtime.

```text
4016d882b1e8a432e332b6da9e0c93c38413eac190e511df3f4e292b47247252  client/UnityProject/Assets/Game/Presentation/OriginClientBootstrap.cs
fdf6cbb85b66d0fc34dee906015224ea275bb12a4f59f6535f4781a4e0c066c1  client/UnityProject/Assets/Game/Presentation/OriginClientView.cs
4549f84d21520b235d7f6c014c468613edd896c9b514da6ad0d311fbfe73ca57  client/UnityProject/Assets/Game/Presentation/OriginAutoplay.cs
044ef2d8ba6ce1c44f2909b928ccaf150d369a231bdbee2256b45df2cd057c6f  client/UnityProject/Assets/Game/Editor/OriginClientBuild.cs
81956a561b53c82c01d0e9abe81e1fb1363a9752a26eeb73e141c1645c54c6d4  tools/unity/run_origin_client.py
995448d4108a31020022ea95df29279365c513973703704fad6aa80ad5672475  client/UnityProject/Assets/Game/Resources/Sprint16/origin-cohort-v1.json
```

Phạm vi review không gồm full causal life generation/Character Viability, fullVS02/VS07/world binding, final owner art/UI approval, IL2CPP/cross-platform, performance/balance hoặc human playtest. G13/profilev1 và thư viện01–18 giữ quyết định chủ dự án.

## Review bản vá source và lượt Editor đầu — 06/11/2026

Actor này đã tự đọc artifact `tests/results/sprint16-client-4g2elcmq`: Configure thất bại vì CS1061 tại ControlBounds, `IResolvedStyle` không có thuộc tính overflow. Không có Player/capture trong lượt này, không lấy build .NET hoặc source UI thay actual Editor.

Source hiện tại đã đổi sang `item.style.overflow.value`; BodyScroll thiết lập overflow Hidden trực tiếp tại viewport/container liên quan nên phép kiểm clipping dùng inline style có cơ sở trong profile này. Cần actual Editor compile xác nhận; đây source patch review, chưa là runtime Passed.

UI01 đã có nhánh ShowMenu hiển thị ErrorText(status.Code) khi code khác ok, tách khỏi LastResult của thao tác trước. Source fix làm cold-start read error có đường trình bày; actual corrupt-menu screenshot chưa được quan sát.

UI02 đã có header field/bounds, GeometryChanged cập nhật content.top dưới header thêm12px và RevealLayoutFits kiểm CardBounds.yMin >= HeaderBounds.yMax+8. Cách sửa trực tiếp rủi ro fixed112 khi textscale thay đổi. Chưa gọi layout thực tế đạt trước khi xem captures/bounds sáu cases.

Root đã thêm OS driver hai process Player riêng, cùng save root. Phase2 không gọi New; Continue→Reveal→World phải giữ cùng actor/recordFingerprint/tick61; persisted DB trước/sau phải bằng nhau. Đây đúng phép kiểm cho UI03, nhưng tại thời điểm ghi phần này chưa có kết quả OS-run. Message lỗi host reopen đã được yêu cầu ghi đúng là sau Player exit + host reopen, không giả actual Player restart.
