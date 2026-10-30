# Hyper3D MCP trong quy trình làm asset

**Giai đoạn hiện tại — cập nhật 29/10/2026:** chủ dự án chỉ yêu cầu ảnh mô phỏng để xem trước ý tưởng game; không tạo model 3D thật. Ảnh mang diện mạo 3D vẫn là ảnh concept, không cần gọi Hyper3D hoặc dựng Blender. Quy trình dưới đây chỉ dùng khi có task tạo model 3D thật được giao lại trong tương lai.

Chỉ dẫn 28/10 về đường tạo model qua MCP Hyper3D được giữ cho các task model thật đó. Đây là công cụ authoring offline; không thêm LLM/network vào gameplay. Blender có thể nhập, bố trí, camera và render sau khi có model trong task tương ứng. Không tiếp tục request correction của Experiment 05 chỉ vì runbook có sẵn lệnh generation.

## Kết nối và client

Server `hyper3d`, StreamableHTTP `https://api.hyper3d.com/api/mcp`, OAuth do Codex quản lý. Không đưa token/APIkey vào repository hoặc chat. Setup đã xác nhận ởSPIN018; entry app nằm trong `~/.codex/config.toml`, SnapCLI có home riêng. Giữ cấu hình và credential ở đúng client.

Nếu tools chưa xuất hiện trong thread, [client](../../tools/mcp/hyper3d.py) gọi MCP qua Codex app-server0.165.0, dùng credential của ứng dụng. Nó không trích token, không dùng model inference và không gọi APIgeneration khác thay Hyper3D. Node/npm cần sẵn để chạy CLI pin bằng npmexec; không cài CLIglobal hoặc ghi service_tier vào config. File schema/SDK vẫn thuộc client, không là runtime dependency của game.

```bash
python3 tools/mcp/hyper3d.py catalog --output tests/results/hyper3d/catalog.json
python3 tools/mcp/check_client.py
```

Catalog là schema live từ server, không hardcode tham số generation theo trí nhớ. Chỉ dùng đúng tools/fields server quảng bá. Nếu OAuth chưa sẵn sàng, xử lý đăng nhập bằng client hiện đại hỗ trợ CIMD/resource như setup đã ghi trong WORKLOG; không tự dùng đường tạo model khác.

## Tạo, theo dõi và nhận kết quả

Khi task đã giao tạo model, chuẩn bị prompt/arguments JSON có scope và version; generation có thể dùng credits của workspace đã cấp quyền. Không tự mua gói hoặc tăng quyền. Client chỉ gửi một request cho mỗi invocation, không có retry generation.

```bash
python3 tools/mcp/hyper3d.py call --tool rodin_generate --arguments /path/to/arguments.json --output tests/results/hyper3d/submission.json
python3 tools/mcp/hyper3d.py call --tool rodin_wait --arguments /path/to/generation-id.json --output tests/results/hyper3d/status.json
python3 tools/mcp/hyper3d.py call --tool rodin_get_result --arguments /path/to/generation-id.json --output tests/results/hyper3d/result.json
```

Dùng generation_id thật để poll, không suy từ request_id. Timeout ởwait không có nghĩa generation thất bại. Nếu submission timeout hoặc server báo “may have occurred” mà không có generation_id, không tự gửi lại; đối chiếu Mine hoặc hỗ trợ provider. Link user gửi cần kiểm ID có đúng lượt hay chỉ là model cũ. Không tự bịa ID hoặc xem model cũ là kết quả sửa.

Chỉ đưa `display_url` ra khi model hoàn tất. `files[].url` là URL tải tạm có chữ ký: dùng nội bộ để lấy asset cho task đã giao, không in vào chat/commit/report. Raw responses phải ở ignored/private evidence; client ghi mode0600. Lưu model hoàn tất và metadata công khai vào đúng thư mục prototype: provider, generationID, displayURL, prompt, tham số và hashes nguồn/xuất. Không gán MIT của mã viewer cho model do provider tạo.

## Kiểm tra trước khi nói đạt

Completed chỉ là trạng thái provider. Đọc lại brief và thực sự xem model/render: hình có đúng loại, có kết cấu/tiếp xúc hợp lý, vật liệu được giữ, geometry có đủ chiều và không tự biến prompt thành đối tượng khác? Kiểm provenance từ rawdownload SHA đến model đã import/export và bytes được viewer nạp.

Một mesh vẫn có thể là model3D thật; không bắt buộc số meshes để giả chất lượng. Kiểm XYZextent/topology, camera/pixel differences và nguồn asset. Giữ PBR/maps; không ép toàn bộ material thành roughness/metalness mới rồi nói đã kiểm đầu ra gốc. Renderer có thể khác shading và phải ghi giới hạn thật.

Thử nghiệm05 ghi một trường hợp providercompleted nhưng model lệch thành nhà thuyền; lượt sửa lỗi server không cóID. Đây là dữ liệu thất bại cần giữ, không là acceptance mỹ thuật. Sprint13 vẫn dùng approval register do chủ dự án chốt.
