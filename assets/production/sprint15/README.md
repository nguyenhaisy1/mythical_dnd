# Asset 2D cho bản thử sửa thuyền Sprint15

Bộ source cho `repair-client-2d-v1`, áp dụng ngôn ngữ `earth-monument-2d-v1`: nền đất mảng lớn, nước xanh xám, nhân vật/đạo cụ có các mặt khối rõ và bảng màu hạn chế. Đây là hình ảnh game hư cấu lấy cảm hứng Việt cổ; không là phục dựng khảo cổ hoặc approval mỹ thuật cuối của chủ dự án.

## Source và runtime

`manifest.json` là inventory/provenance, kích thước, alpha bbox, anchor và vị trí trình bày. Sáu PNG runtime nằm ở `client/UnityProject/Assets/Game/Resources/Sprint15/Art/`: base, foreground, hai trạng thái thuyền và hai frame idle nhân vật. Base chỉ là foundation; scene phải ghép các sprite độc lập, không lấy dressed-reference làm runtime.

Năm ảnh source trong `raw/` đều từ built-in `image_gen`. Hai lượt đầu tạo foundation và dressed reference; hai lượt sau tạo boat pair và post; các kết quả này được khôi phục từ thư mục generated_images sau gián đoạn. Lượt thứ năm do `/root/s15_art_finish` thực hiện tạo actor2×2. Không gọi tạo lặp những kết quả đã có, không API/key, không model3D và không vẽ art bằng code. Prompts dùng để tạo lưu tại `prompts/`, riêng actor thực dùng `actor-used-v1.txt`. Các PNG gốc ngoài workspace được giữ nguyên.

Original foundation1672×941 đã được normalize1920×1080 ở lượt trước; metadata ghi cả source và output. Dressed reference giữ làm kế hoạch, không là ảnh game chạy. Boat/post/actor được chroma key/split bằng processor chính thức của skill `generate2dsprite`; sau đó chỉ crop union alpha bbox theo từng family với padding4px. Hai boat có cùng canvas; hai actor dùng chung crop và scale nên không resize riêng từng frame. Source4idleframes, runtime chọn frame0 và2; đây là idle nhỏ, không phải walk cycle.

## Tọa độ

Canvas1920×1080, origin trên-trái. Boat bottom-center(1160,750), actor feet(800,820), post bottom-center(650,820). PPU120, world plane16×9 và camera orthographic4.5. Metadata có normalized pivot theo alpha bbox; Editor importer nên dùng pivot này để không treo chân vì padding. Boat hiển thị530×285; actor cao210, post cao300 và giữ aspect.

Post là occluder cao, y-sort riêng với actor. Hai preview QA đặt actor(680,795) phía sau và(680,850) phía trước để thấy che khuất một phần mà vẫn nhận ra nhân vật. Nếu runtime đặt actor chính giữa cột và che toàn thân, cần cue vị trí hoặc policy reveal; ảnh preview không thay bằng chứng runtime. Local lane chỉ phục vụ presenter, không là collision/Travel authoritative.

## QC và tái lập xử lý

`asset-qc.json` lưu alpha/bbox/hash và các QC summaries từ processor. Strict QC đã đạt: actor4frames, boats2frames,post1frame, không frame rỗng/edge-touch/paste-clamp. Actor body-scale CV0.000371 và anchor std0. Kích thước/bbox chung của hai actor giữ chân cố định.

Các bước xử lý đã chạy (đường dẫn skill là cài đặt local, không API):

```sh
python3 /home/synh/.codex/skills/generate2dsprite/scripts/generate2dsprite.py process --input assets/production/sprint15/raw/boats-generated.png --target asset --mode boat-variants --output-dir assets/production/sprint15/processed/boats --rows 2 --cols 1 --cell-size 768 --fit-scale 0.92 --align feet --shared-scale --scale-strategy fit --component-mode largest --strict-qc
python3 /home/synh/.codex/skills/generate2dsprite/scripts/generate2dsprite.py process --input assets/production/sprint15/raw/post-generated.png --target asset --mode single --output-dir assets/production/sprint15/processed/post --rows 1 --cols 1 --cell-size 768 --fit-scale 0.92 --align feet --component-mode largest --strict-qc
python3 /home/synh/.codex/skills/generate2dsprite/scripts/generate2dsprite.py process --input assets/production/sprint15/raw/actor-generated.png --target player --mode idle --output-dir assets/production/sprint15/processed/actor --rows 2 --cols 2 --cell-size 512 --fit-scale 0.86 --align feet --shared-scale --scale-strategy preserve --component-mode largest --strict-qc --max-body-scale-cv 0.08 --max-anchor-y-std 0.05
python3 assets/production/sprint15/scripts/export_runtime.py
```

Export script là deterministic crop/copy, từ chối overwrite PNG runtime đã tồn tại; chỉ chạy khi dựng lại output mới. `processed/` và `qa/` là derived artifacts ignored, không cần commit các bản raw-sheet trùng source. Raw originals, prompts, manifest/QC, scripts và final runtime PNG được giữ. Có thể kiểm inventory hiện tại bằng `python3 assets/production/sprint15/scripts/verify_assets.py`.

Các preview ghép layer là QA compositing, chưa phải Unity render. Acceptance actual Editor/Player, keyboard/mouse, UI đọc được và workflow sửa thuyền thuộc root integration evidence.
