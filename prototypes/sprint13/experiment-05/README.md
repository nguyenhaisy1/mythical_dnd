# Thử nghiệm05 — Xưởng sửa thuyền qua model Hyper3D

**Pending user approval.** Sprint13, SPIN-021. Vòng này kiểm model được tạo qua MCP Hyper3D theo chỉ dẫn chủ dự án; Blender chỉ dùng nhập model, bố trí camera và render. Không thay bằng geometry thủ tục của bản04 rồi gọi là model Hyper3D.

Model và ba render đã có; browser checker đã chạy đạt trên đầu ra A. **Model lệch brief thành nhà thuyền, chưa đạt yêu cầu xưởng sửa thuyền.** Lượt sửa gặp lỗi provider không trả generation ID; chưa xác định được trạng thái và không tự gửi lại. Xem [báo cáo vòng05](../../../docs/production/sprints/sprint-13/EXPERIMENT-05-REPORT.md) để phân biệt kết quả kỹ thuật với lỗi thiết kế còn mở.

## Xem bản thử

Mở `index.html` bằng Chrome. Viewer bundle và GLB nhúng trong `assets/scene-data.js` cho phép xem file local, không phụ thuộc CDN. Kéo chuột để xoay, cuộn để tiến/lùi; các nút chọn ba camera và Reset đưa về góc chuẩn. Khi khung3D có focus, phím mũi tên xoay, `+`/`-` phóng, `1`/`2`/`3` chọn camera. Wireframe dùng để xem kết cấu mesh. Ảnh render có thể mở nguyên khung; Escape đóng và trả focus.

Nếu xem qua HTTP, chạy từ repository:

```bash
python3 -m http.server 8013 --directory prototypes/sprint13
```

Mở `http://localhost:8013/experiment-05/`. Serve thư mục sprint13 để liên kết so sánh bản04 hoạt động. Nếu WebGL không khả dụng, UI báo lỗi và có thể xem ảnh render; không coi ảnh tĩnh là bằng chứng viewer3D đã chạy.

## Nguồn và giới hạn

Model phải có provenance từ MCP Hyper3D; dữ liệu generation/kết quả do root lưu và kiểm tra, không nhúng access token hoặc signed download URL vào tài liệu phát hành. Đường dẫn kết quả Hyper3D nếu có trong UI phải là trang hiển thị ổn định. `assets/ben-vac.glb` là model được chuẩn hóa/xuất cho viewer; `assets/scene-data.js` nhúng cùng bytes. `render-overview.png`, `render-eye.png`, `render-repair.png` là ảnh từ cùng model sau bố trí camera trong Blender. Source và lệnh import/render của root phải ghi riêng nếu tạo.

Viewer05 kế thừa control của04 nhưng giữ PBR/maps của model; browser và Blender vẫn có lighting/shader khác nhau nên không hứa màu pixel-identical. Model có thể chỉ gồm một mesh; số mesh không tự chứng minh chất lượng, độ chi tiết hoặc khả năng đưa vào game. Kiểm tra texture/material ghi số thực, không tự kết luận mỹ thuật đã tốt hơn chỉ vì có texture.

Đây là concept cho Bến Vạc, không claim khảo cổ, WorldState, physics/navigation, animation/rigging production, Unity scene hoặc gameplay. Đồ nghề minh họa không trở thành tài sản của nhân vật. Chủ dự án chốt hướng hình ảnh; không dùng technical pass để chốt thay. Các bản01–04 giữ nguyên.

## Kiểm tra

```bash
python3 prototypes/sprint13/experiment-05/browser-check.py
```

Cần Python Playwright, Pillow và Google Chrome tại `/usr/bin/google-chrome`. Checker bật softwareWebGL cho Chrome headless. Khi thiếu model/render, script dừng với “Not run”, không phát sinh báo cáo Passed.

Khi đủ input, checker đọc bytes POSITION và transform scene trong GLB2 để kiểm XYZ extent/nonplanarity và hơn100triangles, cho phép model một mesh. Tiếp đó browser kiểm 1280×720/1920×1080: geometry thật, camera pose/target/FOV, ba khung hình thay đổi thực, kéo/zoom/reset/keyboard/wireframe, ảnh/modal/Escape/focus, chữ150%, không request HTTP trong chế độ file local và liên kết HTTP sang bản04. Texture/material counts là evidence quan sát, không là thẩm định mỹ thuật.

Evidence và screenshots ghi dưới `tests/results/sprint13-experiment05/`, bị ignore. Console warnings softwareWebGL được giữ để đối chiếu, không dùng bịa benchmarkGPU. Automation không thay human playtest hoặc nghiệm thu gameplay. Kết quả runtime chỉ ghi sau khi script thực sự exit0 trên artifact05 hiện tại.

## Dựng lại viewer

```bash
node prototypes/sprint13/experiment-05/build-viewer.mjs
```

Lệnh dùng dependency versions/package-lock và thư mục tạm; cần Node/npm và mạng khi dựng bundle. License/notice Three.js được giữ trong `vendor/`. Xem bundle đã dựng thì offline. Lệnh này không tạo lại model và không gọi Hyper3D. Không tự resubmit generation khi request trước timeout; root phải đối chiếu kết quả provider trước khi quyết định tiếp.

[Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) quyết định kết thúcSprint13. Bản05 vẫn chờ user chốt dù code/browser checks đạt.

## Kết quả thực tế vòng05

Model hiện tại là đầu raA củaHyper3D, đãcompleted nhưng lệch brief thànhnhàthuyền. UI không coi loaded/ready làđạt mỹthuật. Lượt prompt sửa lỗi server `req-39nj` không cóID; userchỉthấymodelA trongMine, trạngthái lượt sửa vẫnchưaxácđịnh. Khôngtựgửi trùng.

MCPtoolclient/runbook: [HYPER3D-MCP](../../../docs/runbooks/HYPER3D-MCP.md). Để tái bố trí/render model đã tải (không tạo model mới):

```bash
blender -b --python prototypes/sprint13/experiment-05/prepare_model.py -- --input /path/to/completed-provider.glb
node prototypes/sprint13/experiment-05/build-viewer.mjs
```

Input phải khớp originalSHA trong provider-source.json. Script chỉ import/scale/placement/light/camera/export/render; không dựng geometry thay provider. Rawdownload trong evidenceignored, source vàhash đã ghi; model xem offline nằm assets/ben-vac.glb. Khônggán licenseMIT củaviewer chomodelHyper3D.
