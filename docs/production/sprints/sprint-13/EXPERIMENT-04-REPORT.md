# Thử nghiệm04 — Cùng một bến sông qua nhiều camera 3D

Sprint13/SPIN-020, ngày28/10/2026. **Có bản xem được, kiểm tra lớp đầu đạt; review độc lập cuối còn thiếu và chờ chủ dự án chốt.** User yêu cầu vòng thử mới và gợi ý view từ3D, đính kèm ảnh B của bản03. Không suy ảnh đính kèm là duyệt B hoặc kết thúcSprint13.

## Câu hỏi thử nghiệm

Một mô hình3D có hình khối, vật liệu và mặt bằng nhất quán có giúp chọn camera cho game tốt hơn các tranh độc lập? Thử góc nhìn chéo cao, tầm người và gần xưởng sửa thuyền trên cùng geometry. Mục tiêu là artifact xoay/xem được và render có nguồn3D thật, không gọi tranh2D giả lập phối cảnh là model3D.

## Sản phẩm và phân công

[Thư mục bản04](../../../../prototypes/sprint13/experiment-04/) chứa build_scene.py, scene.blend, GLB, dữ liệu browser nhúng và viewer; ảnh render được tạo bằng Blender thật. Browser dùng Three.js offline, không cần kết nối tài khoản để xem model đã xuất. Chi tiết artifacts và kiểm tra chỉ được cập nhật sau khi có kết quả thật.

- /root/s13e4_scene: geometry, vật liệu, camera, export/render Blender và art direction.
- /root/s13e4_viewer: Three.js viewer, controls, camera presets, gallery render và build browser bundle có license.
- /root/s13e4_checks: README, kiểm browser bằng mô hình/ảnh thật và evidence.
- /root/s13_critic_code: đã kiểm code/runtime độc lập trước bản sửa FOV; root tái kiểm final.
- /root/s13_critic_ux: đã giao nhưng bị quota trước khi có kết quả; chưa hoàn tất lượt visual độc lập.
- /root: điều phối, tích hợp, kiểm lớp đầu, documents/Git.

Ba workers chạy song song trong giới hạn4concurrent kể cả root; critics thực hiện ở lượt sau. Các bản01–03 được giữ nguyên.

## Nguồn tham chiếu và giới hạn

Ảnh user gửi có checksum trùng `experiment-03/assets/b-gouache.png`, dùng cho bến/nhà/thuyền/đường và màu vật liệu. Đây là dựng cảnh hư cấu mới có layout cụ thể; không cam kết tái tạo từng pixel hoặc có nguồn khảo cổ. Mô hình thủ tục do code tạo trong Blender, không tuyên bố Rodin/Hyper3D đã tạo mesh khi không gọi công cụ đó.

Blender5.2.2LTS có trên máy. Renderer browser và Blender có ánh sáng/shader khác nhau; ảnh render và viewer chia sẻ geometry nhưng không mặc định pixel-identical. Core/gameplay/Unity không được tích hợp bởi thử nghiệm này. Các hình người, đường đi và thao tác chỉ là concept/visual, không có simulation hoặc collision acceptance.

[APPROVALS](APPROVALS.md) vẫn là nguồn chốt; mọi sản phẩm04Pending. Không merge dev hoặc chuyểnSprint14.

## Kết quả thực tế và phản biện

Mở [viewer04](../../../../prototypes/sprint13/experiment-04/index.html). Model gồm26meshes theo26materials,43.887vertices và62.454triangles; GLB3,34MB được nhúng cùng bytes vào scene-data.js để xem file local. Authoring scene Blender và ba render1280×900/24samplesCPU có trong assets. Dùng seed13042026, export giữ cấu trúc mesh, không gọi LLM runtime.

Finding kỹ thuật: viewer trước dùngFOV40 trong khi metadata42 và Blenderlens khác. Worker sửa công thức FOVdọc; root sauquota rebuildbundle/tái kiểm pose/target/FOV thật với metadata và hạ eye đến1,7m trên bank. Root thêm hướng dẫn rebuild và ngăn filebackup .blend1 lọtartifact. [Code review](CRITIC-CODE-04.md) ghi rõ phần independent trướcfix và root saufix. [Visual review status](CRITIC-UX-04.md) ghi chưa hoàn tất doquota, có nhận xét root tách biệt.

Root chạy `python3 prototypes/sprint13/experiment-04/browser-check.py` exit0: hai viewport1280×720/1920×1080 với Chrome softwareWebGL; model và multi-primitiveGLB, camera khác pose/pixels, orbit/zoom/reset/keyboard/wireframe, PNG/modal/Escape/focus, chữ150% và HTTP03reference đạt. Evidence `tests/results/sprint13-experiment04/browser.json`. Đây không là benchmarkGPU hoặc Unityverification. Renderer browser dùng neutralmaterial và lighting riêng; shading không giống hệtCycles.

Mỹ thuật còn giản lược/diorama so với tranhB03. Người là shapeplaceholder chưaanimation; GLB chưa bake noise/bump. Camera thấp ban đầu chật; root đổi vị trí vàlens30mm, render lại tại cùng cao1,7m trênbank. Góc mới đọc được đồ nghề/thuyền hơn nhưng nền xa vẫn là nềnstudio. Root đã xem render và screenshot thật; khôngclaim đạtvisualgate độc lập hoặc userapproval. Vẫn cần phản biện và cảm nhận user.

Theo chỉ dẫn user trong lúc tiếp tục vòng này, **từ lần tạo model3D tiếp theo dùng MCPHyper3D**; Blender có thể bố trí/camera/render sau đó. Bản04 đã dựng trước chỉ dẫn và được user cho tiếp tục, ghi đúng nguồnproceduralBlender. Quy tắc mới được lưu trongAGENTS/.agent và skill artdirection.

Root tái chạy browser check sau sửa camera/FOV và rebuildbundle. Kết quả final cùng source ghi trong WORKLOG trước commit. Lượt critic visual độc lập vẫn chưa chạy, không che gate bằng automated checks.
