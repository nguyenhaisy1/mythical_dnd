# Rule authoring và parameters

Code rule handlers thuộc engine/Domain hoặc Foundation theo contract. Thư mục này dành cho parameter catalog/formula specification bổ sung sau khi FND-04/06 chốt; chưa có executable rules hay final balance pack. Nguồn công thức đầy đủ ở GDD Module 27. Mỗi parameter phải có ID, version, scale/unit, valid range, default/prototype status và tests; không tự diễn giải mọi con số minh họa thành final value.

Prototype versioned coefficients hiện có tại `parameters/kernel-v1.json`; units/ranges và delivery gates ở `docs/content-guides/VERTICAL-SLICE-CONTENT-CONTRACT.md`. Chúng chỉ phục vụ kernel fixture, chưa là balance toàn game.
