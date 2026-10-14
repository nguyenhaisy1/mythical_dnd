# Vertical slice — Bến qua mùa nước

## Mục tiêu sản phẩm

Chứng minh một cuộc đời có nghề, quan hệ, lựa chọn và hậu quả trong thế giới vẫn vận hành khi người chơi không giúp. Không chỉ một quest tuyến tính hoặc combat demo. Nguồn đầy đủ: Module 28; scenario QA ở `docs/qa/SCENARIO-SPECIFICATIONS.md`.

## Phạm vi đã thống nhất để triển khai

Playable center: Bến Vạc (TG-01), Tam Giang. Các local community đề nghị Cồn Mây (TG-02) và Vạn Nước (TG-03). Bến Hạc (PC-02, Phong Châu) là boundary node của tuyến trade; không dựng sâu vùng Phong Châu trong slice. Việc chọn hai community lân cận là quyết định fixture của lần bàn giao này, chưa thay canon toàn thế giới.

Ngân sách tham chiếu: 20–30 named NPC, 6–8 household, 2–3 nhóm lợi ích, 6–10 directed edges, 2–3 site, 1–2 companion, 8–12 environmental/event templates, ba story thread và một combat encounter đủ chứng minh mechanics. Reference JSON hiện chỉ dựng những định danh/tuyến/mối liên hệ tối thiểu để tránh mâu thuẫn; roster đầy đủ là VS-02.

Ba nghề: vận tải đường sông, sửa chữa thuyền, chữa trị. Kịch bản: một thuyền hỏng, giao hàng có hạn, nguồn food chịu áp lực, mùa nước đang đến. Người chơi có thể sửa, tìm phương án vận chuyển khác, điều tra sự cố hoặc không tham gia. Không gắn cứng việc “được NPC gọi đến” với mọi origin.

## Chuỗi dữ liệu bắt buộc

Boat condition / rights / materials → work plan và elapsed time → route availability/capacity → giao hàng, food và household pressure → NPC decision theo perceived state → story thread → information exposure → player action → committed consequences. Mỗi cạnh phải có owner, command/event và trace; không dùng text kết quả để thay state.

## Đầu ra nghiệm thu

- Chơi được một phiên có lựa chọn hữu ích ngoài combat; hiển thị lý do/cost/uncertainty đủ để quyết định.
- Không-intervention run tạo hậu quả hợp lý qua NPC autonomy.
- Save/load và exact command replay khớp cùng versions.
- Time skip và succession được chứng minh bằng fixture có quan hệ/tài sản/knowledge thật.
- Một historical proof chạy headless; chưa cần xây toàn Cổ Loa.
- QA report công khai missing implementations, bugs, measured performance và playtest findings.

Không tăng content quantity để che core loop chưa hoạt động. Go full production chỉ sau VS-10 và quyết định review rõ ràng.

## Gate chuyển nghiên cứu sang dữ liệu

Đọc [Slice Content Contract](../content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md) cho record fields, validation, provenance và parameter gate trước VS-03. Full roster/compiled pack vẫn là deliverable VS-02; kernel fixture không thay thế production content.
