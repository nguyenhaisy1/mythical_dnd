# Canon và thuật ngữ thống nhất

| Khái niệm | Nghĩa / ràng buộc | Nguồn |
|---|---|---|
| Hồng Bàng — Mạch Lạc | Working title; không phải nhãn phát hành đã chốt | M02 |
| Historical Backbone | Phát biểu có căn cứ sử liệu/khảo cổ, cần source review trước công bố | M01/02/19 |
| Traditional / Myth | Truyền thống được lưu truyền, không tự là sự thật lịch sử | M19 |
| Game Canon | Hư cấu riêng của game, gồm Mạch Lạc/Ước Sơn Hà | M02/19 |
| WorldRegion | Đại vùng, 12 vùng trong World Bible | M02, M25 §32 |
| Settlement | Cộng đồng cụ thể thuộc vùng | M03 |
| LocalArea | Khu khám phá, không đồng nghĩa TacticalZone | M25 §32 |
| EncounterSpace / TacticalZone / PositionSlot | Không gian chiến thuật, vùng và vị trí cục bộ | M12, M25 |
| Family / Lineage / Household | Gia đình / dòng họ / đơn vị cùng sinh hoạt-kinh tế | M04 |
| Profession / Career / Class | Nghề / lịch sử nghề / danh tính gameplay có bằng chứng | M08/10 |
| Knowledge / Belief / Reputation | Điều actor biết / điều actor tin / lời đánh giá có nguồn trong xã hội | M16/19/26 |
| Event Definition / Occurrence / Domain Event | Mẫu / lần đủ điều kiện hoặc đang xử lý / kết quả đã commit | M06/24/29 |
| Story Thread / Quest View | Trạng thái diễn biến / góc nhìn và cam kết người chơi | M17/25 |
| Owner / Holder / User / Claimant / Controller | Quyền sở hữu / giữ / sử dụng / yêu sách / quản lý | M25 §38 |
| Vitality / Wound | Khả năng chịu đựng ngắn hạn / thương tích có hậu quả lâu dài | M13/25 |
| WorldTick / WorldRevision | Thời gian simulation / số revision của state commit | M29 |

Bản gốc M03 có 60 settlement và mã nguồn địa danh. Không đổi display name thành primary ID. Route graph và fixture phải tham chiếu ID, dùng alias để giữ tương thích khi sửa chính tả. Những định danh mới trong `content/fixtures/vertical-slice.reference.json` chỉ là design fixture, không phải production pack đã compile.
