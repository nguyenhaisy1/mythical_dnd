# ADR024 — Client 2D sửa thuyền và projection giới hạn theo nhân vật

03/11/2026 · Sprint15 / SPIN-039 · Quyết định kỹ thuật trong sprint được giao.

## Mục tiêu

Đưa luật sửa thuyền Sprint14 lên client Unity thật, có thể thao tác, lưu và mở lại. Giữ hướng `earth-monument-2d-v1`, G13 Reopenable và thư viện01–18. Đây là proof kỹ thuật với một nhân vật cố định tại Bến Vạc; UI phải ghi rõ, không giả là generator xuất thân hoặc full VS07 đã hoàn tất.

## Quyết định

- Application tạo port `IRepairClientSession` và DTO immutable chỉ chứa thông tin được grant. Trusted composition bind principal, worksite, boat, material/tool/recipe và cấp direct-observation grant theo field, có nguồn và tick quan sát. Ownership không tự tạo knowledge. Field chưa được grant là Unknown/absent, không đọc truth rồi chỉ giấu nhãn.
- UI không nhận WorldSnapshot, processor, raw receipt/envelope, full journal/jobs hoặc arbitrary actor/filepath. Read/preview không commit, không tiêu thụ RNG. Preview dùng known inputs, giữ private revision/tick và trả opaque token; stale buộc preview lại. Retry dùng chính envelope ban đầu, không tạo command mới để thực thi lại.
- Safe outcome dùng code hữu hạn; không trả exception.Message hay hidden prerequisite details. Save/load ở trusted Infrastructure composition với SQLite profile Sprint14. Save snapshot chỉ báo thành công sau write; Reload đọc latest durable state, không là rollback slot.
- Client dùng renderer built-in, SpriteRenderer trên mặt XY và camera orthographic cố định. Scene là một worksite/mặt đất; local footpoint chỉ phục vụ presentation, không thay actor settlement, WorldTick hoặc gameplay range/Travel. Animation không quyết định sửa xong.
- Map foundation, foreground occluder, boat variants và actor sprite là asset riêng với metadata anchor/scale/sorting. Không lấy PNG concept có nhân vật/UI baked làm scene chạy. Không tạo model3D. Asset mới dùng built-in image generation và lưu provenance; artwork cũ bất biến.
- Dùng Editor6000.6.7f1 thật tạo ProjectSettings/Packages/.meta; build portable DLL có thể tái lập, không commit binaries/caches. Linux Mono Player phải được build và chạy thực cho acceptance sprint này; không suy từ Editor conformance cũ.

## Acceptance và tác động

[Contract](../contracts/REPAIR-CLIENT-PROFILE.md) và [Sprint15](../production/sprints/SPRINT-15.md) quy định gates: hidden-data/preview/command/persistence tests; actual Editor/Player/UI flow, rendered captures1280×720 và1920×1080, foreground occlusion và input/modal checks. Programmatic callback automation, OS input checks, visual review và human playtest phải ghi thành các loại evidence khác nhau.

Không thay repair recipe/image2/SQLite schema2 hoặc legacy goldens; thêm presentation/session composition. Full Knowledge/Perception, random life, roster world binding, đủ ba nghề, full VS03/VS07, final UI/art approval, performance/balance và human playtest là backlog dự án ngoài sprint này. Báo cáo đóng sprint phải ghi rõ có hay không **điều kiện đóng Sprint15 còn thiếu**, tách khỏi backlog tương lai.
