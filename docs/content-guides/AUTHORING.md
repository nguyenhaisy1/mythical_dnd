# Hướng dẫn sản xuất nội dung

## Từ nghiên cứu tới runtime pack

World Bible/GDD → authored definitions → schema validation → semantic/reference validation → rule eligibility tests → build manifest/hash → approved pack → runtime registry. Không load văn bản GDD trực tiếp như script. Runtime LLM không phải bước trong pipeline này.

Mỗi definition cần ID không phụ thuộc display name, type, schema/version, source module/section, lịch sử/ truyền thống/game-canon classification, references và validation status. Event cần actor roles, địa điểm/thời gian, hard preconditions, resource requirements, allowed handlers, outcomes, delayed effects, knowledge/witness policy và cooldown/novelty constraints nếu có.

## Quyền và nhân quả

Content không tự ghi arbitrary state path, gọi SQL hoặc nhúng code. Event template tham chiếu handler và typed parameters. Handler phải kiểm actor/resource/location/time/knowledge prerequisites. Nội dung “thợ sửa thuyền” không tự cấp skill/tool; asset “thuyền gia đình” không tự là tài sản cá nhân người đang lái.

Event occurrence cần ID và causal source. Story Thread tổ chức hậu quả từ committed events; Quest View theo actor knowledge. Description phải khớp facts đã có; tin đồn được đánh dấu claim, không tự ghi đè World Truth.

## Historical/source review

Địa danh/persona hư cấu phải ghi game-canon. Chất liệu truyền thuyết không được ghi chắc chắn là lịch sử. Các citation token phiên chat cũ chỉ giữ trong archive; trước xuất bản cần lập URL/source evidence thật. Không tự tải/đưa copyrighted asset vào game vì được nhắc trong nghiên cứu.

## Chuẩn bị VS-02

Reference fixture hiện giúp khóa IDs và phạm vi địa lý. Author roster/households, capability histories, rights/stock, route direction/cost, 8–12 event templates, 3 threads và ba profession activities. Kiểm tra no-intervention branch trước khi viết thêm dialogue. Không coi 20 NPC chỉ có name/stat là world simulation đủ.

## Checklist duyệt content

Schema hợp lệ; IDs/references duy nhất và tồn tại; owner domain đúng; conservation/time constraints; có provenance cho knowledge; hidden facts không nằm trong player-facing text; branch không đạt phải báo rõ; deterministic test có seed/versions; outcome đúng semantic. Final pack phải pin rule/content versions và không tự cập nhật giữa campaign.

## Gate chuyển nghiên cứu sang dữ liệu

Đọc [Slice Content Contract](VERTICAL-SLICE-CONTENT-CONTRACT.md) cho record fields, validation, provenance và parameter gate trước VS-03. Full roster/compiled pack vẫn là deliverable VS-02; kernel fixture không thay thế production content.
