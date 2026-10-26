# Fixture điều kiện ứng viên sự kiện — Sprint 12

`scenarios.json` cung cấp 23 context đầu vào cho tám template trong `content/definitions/vertical-slice/events.json`. Đây là snapshot chẩn đoán do test cung cấp, không phải WorldState, save hoặc dữ liệu perception đã được hệ thống game xác thực. `expected_eligible` chỉ mô tả kết quả evaluator; `reason` giải thích mục tiêu kiểm thử, không là lời thoại hoặc trace công khai cho player.

Mỗi template có một trường hợp đủ điều kiện và một trường hợp thiếu điều kiện. Bảy trường hợp bổ sung kiểm thiếu dữ liệu vật chất, không biết lời kể, actor chết, cooldown đang hiệu lực/đúng biên, thiếu perception và candidate NPC không phụ thuộc player. Không có trường player/proximity. Trường hợp no-player chưa chứng minh NPC tự thuê thợ hoặc tự chi tài nguyên.

## Nội dung và provenance

Tám tình huống theo checklist VS02 của `docs/content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md`. Các template, ngưỡng và thời gian là **prototype-authored / game-canon**, chưa là balance hoặc dữ kiện lịch sử. M16 §§3–6 quy định dialogue không phải truth và thông tin cần provenance; M17 §§1–3 quy định thread độc lập player và quest không tạo state; M17 §§20–22 phân biệt deadline thật với thông báo; M28 §§95–97 yêu cầu compiler và QA nhân quả. Các citation trong trường `source` chỉ trỏ nguyên tắc nghiên cứu, không gán hệ số mới cho nguồn cũ.

| Template | Actor fixture | Điều kiện vật chất | Nhận thức của actor | Cooldown |
|---|---|---|---|---|
| transport-deadline | Vạc Tòng | Còn 0–60 tick tới hạn | Biết yêu cầu vận tải | 60 tick |
| boat-degradation | Vạc Mộc | Condition thuyền ≤ 400/1000 | Nhận biết nhu cầu sửa | 120 tick |
| rain-interruption | Nước Nghiêm | Cường độ mưa ≥ 700/1000 | Biết yêu cầu vận tải | 60 tick |
| food-shortage | Vạc Mai | Hạt khả dụng < 3 count | Nhận biết thiếu lương | 1440 tick |
| rumor-arrival | Nước Thoa | Có bằng chứng thông tin đã đến | Có claim của Thoa trong known facts | 120 tick |
| landing-rights-dispute | Triều Khê | Quyền tiếp cận bến đang bị tranh chấp | Biết tranh chấp | 240 tick |
| injury-treatment | Bến Sen | Mức thương tích > 0/1000 | Nhận yêu cầu chăm sóc | 60 tick |
| npc-hires-repair | Vạc Tòng | Condition ≤ 400 và gỗ khả dụng ≥ 2 count | Nhận biết nhu cầu sửa | 240 tick |

`now_tick` và `last_occurrence_tick` là simulation tick, không wall clock. Tick đã có quy ước một phút; cooldown ở đây là khoảng chờ ứng viên dùng cho prototype, không duration công việc. Thiếu lượng hạt là Unknown, không phải zero. Claim `fact.nuoc-thoa-claim@1` giữ nguyên nguồn settlement pack: Thoa nghe khách nói Bến Hạc đông, chưa được kiểm tại chỗ. Chỉ biết claim không khẳng định nội dung là World Truth.

## Ranh giới tích hợp

`physical` phải được adapter có thẩm quyền dựng từ domain sở hữu: Assets cho condition/resources, Economy cho deadline hợp đồng, Health cho injury, Assets/rights và Social commitments cho tranh chấp, Knowledge cho bằng chứng delivery. Rain là observable môi trường của prototype; owner và adapter môi trường phải được chốt trước gameplay binding. Tên field trong fixture không trao quyền ghi các domain đó cho Narrative. `perceived` và `known_fact_ids` phải thuộc đúng actor. Compiler không chứng minh snapshot đầu vào thật hoặc đủ provenance; adapter runtime còn gate riêng.

Resources là số lượng đã được phía cung cấp context xác định khả dụng cho actor xét điều kiện; fixture không reserve/consume hoặc chứng minh rights. `eligibility-candidate-v1` chỉ trả candidate, không tạo thợ, vật liệu, thương tích, hợp đồng, tin đồn mới hay committed occurrence. Cooldown đọc occurrence tick đã commit được cung cấp ngoài evaluator; gọi lại evaluator không sửa tick này. Các giá trị authored được bind vào hash pack version 1; thay giá trị phải cập nhật pack compatibility và fixtures, không sửa kernel-v1 ngầm.

Text localization là tên template cho authoring/diagnostics, chưa được phép đưa trực tiếp lên player UI. UI tương lai cần projection theo knowledge, không dùng trace vật chất từ evaluator. Full VS02 còn random protagonist, actor-specific obligations và binding runtime; NPC consequences thuộc các sprint gameplay tiếp theo.
