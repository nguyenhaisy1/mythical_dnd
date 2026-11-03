# Phản biện code/provenance05 — /root/review

28/10/2026, Sprint13/SPIN-021. Reviewer độc lập `/root/review`; phạm vi MCP bridge và viewer/checker05. **Pending user approval**. Không gọi provider, không generate model, không sửa implementation hoặc Git.

## Lượt source và provenance ban đầu

Đã đọc `tools/mcp/hyper3d.py`, viewer.js, README, model-brief và checker05. Đã kiểm riêng cấu trúc submission/wait-01/result trong private ignored evidence, chỉ xuất keys/status/ID/count/mode; không xuất signed download URLs. Submission có generation `e7fe0cb0-45a6-4d85-9f87-63be17c5e6dc`, status queued; wait-01 cùng ID có status completed; result có hai files. Ba response có mode0600. Đây là bằng chứng provider lifecycle, chưa chứng minh geometry tải về trùng model hoặc renderer đạt.

- Bridge gọi Codex app-server MCP với kết nối OAuth đã cấu hình; không đọc/chép credential/token. Chỉ call tool được server quảng bá. Không có loop tự resubmit generation khi timeout; thao tác tạo model nằm trong scope user giao qua MCP.
- RPC có deadline, xử lý lỗi và process exit; inbound interactive request bị reject thay vì tự approve. Finally chấm dứt process group, có kill fallback. Đây là fail-closed cho bridge CLI đơn nhiệm, không phải hỗ trợ mọi phương thức JSON-RPC.
- Summary không xuất files[].url; raw response lưu private. Caller vẫn chịu trách nhiệm chọn output path ignored đúng policy; helper không tự cưỡng chế vị trí đó.
- Viewer chỉ parse GLB bytes được truyền trong scene-data. Không thấy fallback tạo procedural geometry hoặc nhúng mesh04 khi thiếu model; thiếu/corrupt data đi error UI. Preset defaults là camera, không phải fallback model.
- Viewer giữ PBR values/maps, chỉ gắn ánh sáng môi trường/đèn; wireframe là chế độ debug. Không suy texture count thành nghiệm thu mỹ thuật.

## Findings ban đầu

1. **P2 — thông báo timeout:** `asyncio.wait_for` có thể ném TimeoutError trực tiếp trước nhánh deadline custom, khiến stderr rỗng thay vì nhắc không resubmit. Không có automatic retry thực tế, nên chưa là lỗi tạo trùng; nên normalize timeout message để người vận hành không hiểu nhầm request chưa đến provider.
2. **P2 — thời điểm hiển thị provider link:** summary whitelist hiện xuất display_url cả khi status queued. Chỉ dẫn lượt review cho phép công khai link ổn định sau completion; nên gate completed hoặc result-fetch. Không phát hiện việc print temporary files[].url từ bridge.

## Gate còn chờ

Lượt này chưa chạy browser, chưa có kiểm GLB hash từ provider tới model chuẩn hóa/embedded bytes, chưa nghiệmthu render. Root sẽ báo khi artifacts đủ để reviewer nối lượt kiểm runtime. Chưa kết luận code gate cuối đạt chỉ từ source review; approval của chủ dự án và các giới hạn Unity/gameplay giữ riêng.

## Retest độc lập và model A — /root/review

Root đã sửa hai P2 bridge. Reviewer đọc lại fixes và thực sự chạy `python3 tools/mcp/check_client.py`: **4 tests, OK, exit0**. Test timeout xác nhận chỉ gửi một request và có nhắc không tự gửi lại; notification không thay response/tạo request mới; interactive request không được tự approve; summary không hiện link queued hoặc files[].url. Source `safe_summary` chỉ đưa display_url khi completed hoặc `rodin_get_result` có files. Hai P2 ban đầu đã resolved trong phạm vi này. Không gọi MCP/provider trong lượt retest.

Đã đọc `prepare_model.py`: import GLB, kiểm original SHA trước nhập, chỉ normalize scale/placement rồi thêm camera/lights và export; không tạo mesh thay thế. Một phép kiểm Python độc lập thực sự xác nhận:

- SHA256 của private `provider-original.glb` khớp provider-source.original_sha256 và scene-manifest.original_sha256.
- SHA256 của `assets/ben-vac.glb` khớp manifest; decoded scene-data.glbBase64 giống từng byte GLB xuất.
- Generation liên kết là `e7fe0cb0-45a6-4d85-9f87-63be17c5e6dc`; original14934260bytes, exported14938948bytes. Một mesh,60000triangles,một material theo manifest.

Đã thực sự chạy `python3 prototypes/sprint13/experiment-05/browser-check.py`: **exit0, passed, hai viewport**. Evidence `tests/results/sprint13-experiment05/browser.json` được tái tạo bởi lượt reviewer. Checker kiểm geometry GLB/3D, camera position/target/FOV, pixel differences, orbit/zoom/keyboard/reset/wireframe, modal/focus/ảnh, text150 và offline/HTTP navigation theo scope script. Đây là runtime browser của **model A**, không chứng minh mô hình đạt brief xưởng sửa thuyền hoặc đủ chất lượng production.

Root thông báo request correction trả INTERNAL_SERVER_ERROR không có generation ID rõ; không coi đó là generation thành công và không tái gọi provider từ reviewer. Link/ID model A không tự chứng minh model correction đã tồn tại. Cần giữ uncertainty này trong báo cáo và không tự resubmit khi chưa đối chiếu provider/user.

**Kết luận code/provenance model A: Technical reviewed; không còn blocker được phát hiện trong scope kiểm trên.** Review visual/brief có gate riêng. Không nghiệmthu Unity/gameplay/archaeology hoặc chốt thay chủ dự án; **Pending user approval** không đổi.
