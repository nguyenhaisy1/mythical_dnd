# Critic1 — Cảnh3D và viewer, Experiment04

Actor `/root/s13_critic_code`, ngày28/10/2026, SPIN-020. Independent review code/scene build/viewer/checker. **Pending user approval**; không nghiệmthu gameplay hoặc Sprint13.

## Finding

**P2 C04 — FOV metadata không được áp dụng.** `build_scene.py` xuất `cameras[*].fov=42`; viewer tạo PerspectiveCamera40 và setPreset chỉ áp position/target. Ba pose đúng nhưng projection không theo contract metadata. Render Blender dùng lens45/47 ở aspect1280/900, nên cũng không chứng minh cùng framing. Đã gửi root: áp declaredFOV, và derive hoặc ghi rõ khác framing Blender/viewer; không bắt hai renderer có màu giống hệt. Chờ bản sửa/tái kiểm.

README nên thêm lệnh rebuild scene/bundle và Blender version thực để người nhận dễ tái tạo; đây là đề nghị tài liệu, không blocker runtime. Script build có sẵn và dependency pin rõ.

## Evidence độc lập

Script `/tmp/s13-critic04.py` chạy Chrome156.0.8078.47 softwareWebGL headless, file local, exit0:

- GLB embedded trong scene-data.js decode byte-identical với ben-vac.glb. JSON GLB có embedded buffers, không HTTP image dependency. Runtime hiện26meshes,62454triangles từ scene traversal thực; không phải một ảnh nền giả3D.
- Ba camera preset trả mảngposition khác nhau và khớp scene-data trong tolerance1e-8. Đổi phần tử mảngdebug trả ra không sửa camera thật. ArrowLeft chuyển custom; phím1 reset overview; wireframe checkbox/debug đổi đúng. Đọc handler xác nhận áp vật liệu; lần kiểm này chưa pixel-diff wireframe.
- Reference image modal decode được, Escape trả focus. Không pageerror; không HTTP request khi mở filelocal.
- Bundle SHA256 khớp vendor manifest. Không có node_modules trong thư mục thử nghiệm. package/lock pin Three0.186.1, esbuild0.28.2; build npmci trong thư mục tạm, xóa trong finally; Three MIT notice có đầy đủ attribution/license. Chưa independently rebuild/npm install để xác nhận byte reproduction.

Lượt đầu script dùng equality số thực tuyệt đối cho camera và fail do OrbitControls rounding. Đổi assertion tolerance1e-8 rồi chạy đạt; đây là sửa phép kiểm, không finding sản phẩm.

Đọc build_scene.py: seedlocal13042026, hình học/vật liệu được dựng bằng code, exportGLB mesh selection và saveBlender, render theo3camera. Không production state/Unity/autonomy hoặc runtimeLLM. Viewer phụ thuộc bundledThree, không CDN; GLTF loaders có năng lực tảiURI nhưng GLB hiện kiểm không dùng ngoài. Không suy tính deterministic gameplay từ seededartbuild.

Đã đọc checker worker, không nhận các checks của worker là tự chạy. Render PNG3file hiện có; chưa tự chạy fullsuite worker/Blender rebuild hoặc human playtest. Art/UX riêng do critic2 kiểm. C04 cần tái kiểm trước kết luận kỹ thuật cuối.

## Root tiếp quản tái kiểm C04 — 28/10

Sau khi workers chạm quota, `/root` xác nhận metadata dùng FOV dọc suy từ sensor horizontal36mm, lens từng camera và aspect1280/900; viewer đã đọc data.fov và updateProjectionMatrix. Root bổ sung debug getter cameraFov đọc camera thật và kiểm browser so pose/target/FOV với metadata trong tolerance1e-7: hai viewport đều đạt. Eye camera được hạ còn1,7m trên mặt bank theo scene manifest. C04 đã được root sửa/tái kiểm; **chưa có lượt tái kiểm độc lập cho revision cuối**. Báo cáo review trước vẫn là evidence phạm vi đã nêu, không tự chuyển toàn bộ final verdict thành independent passed.

## Bổ sung evidence tái kiểm độc lập C04

/root/review đã chạy browser-check trên bản cuối và ghi ở [review bổ sung](CRITIC-UX-04.md): exit0 hai viewport, cameraFov/position/target ba preset khớpmetadata1e-7. C04 resolved có evidence độc lập. Các nhận xét mỹ thuật còn mở và userapproval vẫnPending.
