# Thử nghiệm04 — Bến Vạc nhìn từ một cảnh3D

**Pending user approval.** Sprint13, SPIN-020. Cùng một mô hình bến sông được dựng trong Blender, xuất GLB rồi xem bằng trình duyệt; ba góc camera giúp thử cấu trúc không gian và hình ảnh nhất quán. Ảnh B-gouache của [Thử nghiệm03](../experiment-03/index.html) là tham chiếu bố cục, chưa phải phong cách được chốt.

## Xem bản thử

Mở `index.html` bằng Chrome. Bundle Three.js và GLB được đóng gói tại chỗ để tránh phụ thuộc CDN khi xem. Kéo chuột để xoay, cuộn để tiến/lùi; dùng các nút camera và Reset để quay về góc chuẩn. Khi vùng3D có focus, phím mũi tên xoay, `+`/`-` đổi khoảng cách và `1`/`2`/`3` chọn góc. Chế độ wireframe giúp xem cấu trúc lưới. Ảnh render Blender có thể mở nguyên khung; Escape đóng và trả focus.

Nếu xem qua HTTP, chạy từ project:

```bash
python3 -m http.server 8013 --directory prototypes/sprint13
```

Mở `http://localhost:8013/experiment-04/`. Serve thư mục sprint13 để các liên kết so sánh03 hoạt động. Khi WebGL không khả dụng, xem ảnh render thay cho khung tương tác; không coi ảnh tĩnh là bằng chứng WebGL đã chạy.

## Nguồn và phạm vi

`build_scene.py` dựng hình học/vật liệu/camera, `assets/scene.blend` là cảnh Blender; `assets/ben-vac.glb` là bản xuất cho viewer, `assets/scene-data.js` chứa cùng GLB để mở qua file local. `assets/render-overview.png`, `render-eye.png`, `render-repair.png` là ba render từ cảnh thật. Tác nhân scene ghi manifest/version/lệnh đã chạy khi tạo artifacts. Viewer dùng Three.js; notice/license và build nguồn đi kèm ở thư mục vendor và source build.

Đây là concept3D tự dựng cho thử nghiệm. Không có claim phục dựng khảo cổ, actor/state simulation, animation/rigging production, Unity scene hoặc gameplay. Đồ vật minh họa không trở thành tài sản của nhân vật. Ánh sáng/shader của Blender và viewer có thể khác; giữ cấu trúc hình học không chứng minh hai renderer có output màu giống hệt nhau. Các phiên bản01–03 được giữ nguyên.

## Kiểm tra

```bash
python3 prototypes/sprint13/experiment-04/browser-check.py
```

Cần Python Playwright, Pillow và Google Chrome tại `/usr/bin/google-chrome`. Checker bật software WebGL cho Chrome headless; chạy hai viewport1280×720/1920×1080, kiểm GLB thật có nhiều primitive, mesh/triangle counts từ viewer, ba camera có pose và pixel output khác nhau, kéo/zoom/reset/keyboard/wireframe, ảnh render, modal/Escape/focus và chữ150% không tràn ngang. HTTP đối chiếu03 cũng được kiểm.

Ảnh/evidence ghi dưới `tests/results/sprint13-experiment04/`, bị ignore. Console warnings software WebGL được giữ trong evidence, không dùng để bịa benchmark phần cứng. Browser automation chỉ kiểm tương tác/render; không thay đánh giá hình ảnh của chủ dự án, human playtest hoặc nghiệm thu gameplay. Nếu artifacts còn thiếu, checker ghi Not run và dừng; chỉ report passed sau lượt thực sự đạt.

[Danh sách cần chốt](../../../docs/production/sprints/sprint-13/APPROVALS.md) vẫn quyết định việc kết thúc Sprint13; technical checks không tự chuyển Approved.

## Tái tạo artifact

```bash
blender -b --python prototypes/sprint13/experiment-04/build_scene.py
node prototypes/sprint13/experiment-04/build-viewer.mjs
```

Script scene dùng seed cố định và xuất blend/GLB/data trước khi render ba PNG. Thêm `-- --no-render` vào lệnh Blender để chỉ dựng/xuất mesh; ảnh render cũ không được coi là đã tái tạo trong lượt đó. Script viewer dùng dependency versions/package-lock và thư mục tạm, giữ license Three.js; cần Node/npm và mạng để dựng lại bundle, xem bản đã dựng thì offline.

Nguồn bản04: model thủ tục Blender; không phải Hyper3D. Theo chỉ dẫn mới của chủ dự án, từ lần tạo model3D kế tiếp dùng MCP Hyper3D, sau đó có thể bố trí và render bằng Blender.
