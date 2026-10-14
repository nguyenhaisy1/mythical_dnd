---
name: mythical-kernel
description: "Implement or review mythical_dnd portable C# kernel primitives, transactions, scheduling, persistence and replay. Use for FND core work and rule integration; not for Unity scene/UI authoring or generic real-time physics."
---

# Kernel Mythical

## Phạm vi và nguồn chuẩn

Chỉ áp dụng cho repository mythical_dnd / Hồng Bàng — Mạch Lạc. Xác định project root từ working directory và kiểm có `.agent`, `MythicalDnd.sln`, `docs/governance/APPLICABLE-BASELINE.md`; không áp dụng vào repo khác vì tên file giống nhau. Nếu được gọi ngoài project và chưa biết đường dẫn, hỏi đường dẫn trước khi sửa. Các đường dẫn bên dưới tính từ project root, không từ thư mục skill hay thư mục cài symlink.

Đọc `AGENTS.md`, `.agent`, `docs/production/HANDOFF.md` và `docs/governance/APPLICABLE-BASELINE.md` khi bắt đầu phiên hoặc khi chúng thay đổi; tái dùng context còn hiệu lực. Chỉ đọc module/contract liên quan, không nạp toàn bộ GDD mỗi task. Yêu cầu hiện tại của người dùng có ưu tiên; skill không tự cấp quyền mở rộng scope, triển khai game, cài công cụ hoặc chạy multi-agent. Không thêm bước xin duyệt cho việc đã được cho phép.

## Chọn contract đúng phạm vi

Đọc `ARCHITECTURE.md`, `docs/contracts/OWNERSHIP.md` và task liên quan. Mở từng spec khi cần: IDENTITY-SPEC, TIME-SCHEDULER-SPEC, DETERMINISTIC-MATH-RNG-SPEC, COMMAND-ACTIVITY-CATALOG, PERSISTENCE-SAVE-SPEC trong `docs/contracts/`. Version/algorithms/ranges lấy từ những file này, không lấy từ skill hoặc ví dụ nghiên cứu cũ. Kiểm `docs/governance/OPEN-DECISIONS.csv` để phân biệt decision đã có với provider/editor/toolchain còn cần spike.

Core target theo csproj/Directory.Build.props hiện tại; host SDK theo global.json. Không áp API/C# feature chỉ có trong host vào netstandard core. Phụ thuộc assembly phải đúng architecture graph; không thêm UnityEngine, IO/network/time/random global vào Domain/Foundation.

## Invariants cần bảo vệ

- Authoritative time = WorldTick; Time.deltaTime chỉ có thể phục vụ presentation. RNG contexts/draw schedule, rounding/overflow, ordering và canonical bytes theo spec/version; preview không commit draws.
- Rule đọc snapshot, trả proposal. Application kiểm owner và invariant rồi commit; không SQL trong rule hay UI ghi World State.
- ID, jobs, RNG, events, receipts và revision tuân cùng atomic policy. Accepted mutation khác rejected/no-op metadata; retry cùng envelope không execute lại. Đừng đơn giản hóa tất cả receipts thành world mutation.
- Revalidate death/rights/location/reservations lúc completion. Cancel không tạo lại vật liệu đã consume/destroyed. Quest/narrative không tạo physical truth để ép kết quả.
- NPC decisions và UI projections dùng perceived knowledge; debug omniscient không được rò vào API player.
- Command replay phải chạy lại resolution; đọc journal hoặc so fixture JSON không phải replay. State hash khác projection hash; save compatibility không suy từ cùng seed.

## Cách triển khai

Làm một hành vi end-to-end tối thiểu trong phạm vi task, gồm rejection/rollback/retry cases cần thiết. FND-02 phải tạo test setup thật khi được giao implementation; không coi Python vectors là C# tests. Dùng `content/fixtures/kernel-smoke/` làm input tham chiếu, giữ rõ những phần còn thiếu full runtime snapshot.

Pure tests trước; integration storage dùng adapter thật khi nghiệm thu durability. Chạy failure/recovery ở boundary phù hợp contract. Cross-runtime chỉ kết luận sau khi chạy Unity backend thật; hot reload Editor không chứng minh IL2CPP/replay.

Nếu gặp mismatch giữa spec và implementation, xác định cái nào sai trước; không đổi golden, algorithm hoặc public API để né test. Decision mới phải có impact/version/compatibility và cập nhật tài liệu, fixtures, consumer tests. Không mở toàn bộ domain hoặc tối ưu quy mô 1.000 NPC trước khi scope hiện tại đạt acceptance.
