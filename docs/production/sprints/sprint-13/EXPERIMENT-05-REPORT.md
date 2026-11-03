# Thử nghiệm05 — Kiểm tra đầu ra Hyper3D bằng model thật

Sprint13/SPIN-021, ngày28/10/2026. **ModelA xem được nhưng không đạt brief sửa thuyền; mọi sản phẩm chờ chủ dự án chốt.** Lượt sửa gặp lỗi provider chưa xác định được generationID.

## Kết quả dễ hiểu

Đã dùng MCPHyper3D thật để tạo một asset có texture/PBR và đưa vào viewer3D. Hyper3D hiểu prompt thành **nhà thuyền có mái/vách**, thay vì thuyền lòng mở trên hai giá sửa. Đây là đầu ra lệch brief cần giữ để học cách chia nhỏ yêu cầu, không được báo thành công mỹ thuật vì provider trảcompleted.

[Mở bản05](../../../../prototypes/sprint13/experiment-05/index.html) để xoay/zoom, đổi camera, xem lưới và ba render. UI ghi rõ đầu raA chưa đạt brief. [Trang provider](https://hyper3d.ai/workspace/rodin/e7fe0cb0-45a6-4d85-9f87-63be17c5e6dc) là kết quả hoàn tất của lượt đầu; không phải lượt sửa.

## Những gì thực sự được tạo

Prompt908ký tự yêu cầu một cụm xưởng nhỏ: mái che, thuyền mở, hai giá, bàn và vài props. Tham số thực: `rodin_generate`, Gen-2.5-Medium, Raw, target60.000polygons, GLB. GenerationID `e7fe0cb0-45a6-4d85-9f87-63be17c5e6dc` đi từqueued đếncompleted/pack5of5; kết quảPBR tải được14.934.260bytes. [Brief](../../../../prototypes/sprint13/experiment-05/model-brief.json) và [metadata nguồn](../../../../prototypes/sprint13/experiment-05/assets/provider-source.json) ghi prompt, ID, hash và outcome.

MCP được gọi qua [client](../../../../tools/mcp/hyper3d.py) sử dụng Codexapp-server và OAuth đã cài; không trích token hoặc đổi sang directAPI/provider khác. Blender chỉ nhập, scale/đặt model, ánh sáng/camera, export và render. Không tạo meshprocedural để giả là Hyper3D.

Model đặt trong viewer gồm1mesh,60.000triangles,1material và3texturemaps. GLB nguồn và GLB xuất có hash riêng; việc export/placement được ghi trongmanifest. `scene-data.js` chứa byteGLB xuất để xem offline bằng file local. `scene.blend` đóng gói vật liệu/ảnh; ba PNG1280×900 render bằng CyclesCPU24samples từ cùng model. Không thêm người/đồ vật bằng hậu kỳ để che sai output.

## Vấn đề và lượt sửa

Criticvisual xác nhậnP1: mái/vách/hull gộp thành nhà thuyền, không thấy thuyền sửa mở hoặc hai giá riêng nhưbrief. P2: wood/straw còn thiên màu ấm đồng đều và vệt nhòe/lặp, chưa chứng minh cải thiện màu theo ý user. Nhìn studio không có đất nền không tự là lỗi floating; các nhận xét cấu trúc dựa trên hình đã xem, không giả đo colliders.

Root gửi **một prompt sửa có chủ đích953ký tự**, chỉ yêu cầu thuyền vàhai giá, không mái/nhà/props. Đây là sửa sau khi đã xem model hoàn tất, không retry request timeout. Provider trả `INTERNAL_SERVER_ERROR`, requestID `req-39nj`, cảnh báo submission có thể đã xảy ra và không tự gửi lại; response không có generationID.

Root dừng gửi generation. User kiểm Mine, ban đầu gửi link modelA rồi xác nhận chỉ thấy modelA với cùngID. Vì vậy **lượt sửa chưa được xác nhận có/không tồn tại**, không có modelB để báo nghiệmthu, không suy số credits đã trừ hoặc gán outputA thànhB. Cần đối chiếu provider/Mine hoặc hỗ trợ với requestID trước khi quyết định gửi mới. Không liên hệ hỗ trợ thay user hoặc mua gói.

Đầu raA và bảnrender đầu được giữ trong assets/attempt-a cùng source/hash; không xóa thất bại để hồ sơ trông như mọi prompt đều đúng. Brief sửa và lỗi được giữ; rawresponse/signeddownloadURL nằm privateignored evidence, không commit.

## Phân công và review

- /root/s13e5_direction: artdirection/brief, không tự gọiprovider.
- /root/s13e5_viewer: viewer giữ PBR/maps, thông báo lỗi khi model thiếu và nhãn failedbrief đúng.
- /root/s13e5_checks: browserchecker/README; model mộtmesh vẫn phải cógeometry3D thực.
- /root/review: criticcode độc lập, kiểmMCP evidence/provenance/transport và browser modelA.
- /root/s13e5_visual: criticvisual độc lập, xem ba renderA, đối chiếu brief/04 và UI cuối.
- /root: actualMCP submissions/status/results, download/placement/render, kiểm lớp đầu, docs/Git.

[Code review](CRITIC-CODE-05.md), [visual review](CRITIC-UX-05.md). Ba workers thực hiện song song, critics lượt sau, đúng giới hạn4concurrent.

## Kiểm chứng và giới hạn

`python3 tools/mcp/check_client.py`:4/4offline controls đạt, gồm timeout không gửi lại, notification không duplicate request, không tự chấp nhận interactive request và không đưa link queued/signedURL vào summary. HaiP2bridge từreview đã sửa. CI chỉ chạy controls offline, không phát sinh model.

`python3 prototypes/sprint13/experiment-05/browser-check.py`: exit0,1280×720/1920×1080. Kiểm GLB XYZextent/nonplanarity/topology, decoded1mesh60ktriangles,3texturemaps, ba camera pose/FOV và pixelskhác nhau, orbit/zoom/wireframe/keyboard/modal/focus/text150/HTTP04reference. Root và criticcode đã chạy độc lập. Evidence `tests/results/sprint13-experiment05/browser.json`; không benchmark hoặc humanplaytest.

Provenance được đối chiếu độc lập: rawdownloadSHA → provider-source → manifest; exportedGLBhash → manifest → embeddedbase64bytes. Rawfile là outputprovider, không dùng trườngasset.generator một mình để chứng minh nguồn. ViewerruntimePBR giữmaps, không éproughness/metalness cho toànasset. Shadingbrowser khácBlender vẫn là giới hạn.

ModelA có thể đưa user xem như kết quả thử sai, **không đạt mục tiêu repairasset**. Không có gameplay, Unity, rig/collision/navigation hoặc accuracykhảo cổ acceptance. Bản01–04nguyênvẹn. A01–A04chưa được chốt, không merge dev hoặc sangSprint14. Root chưa đề xuất dùng modelA làmxưởng tronggame.
