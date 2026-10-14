# ADR-004 — Determinism, versioning và command replay

Ngày: 2026-10-13. Trạng thái: baseline áp dụng cho lần bàn giao. Nguồn: DEC-007/008/009/022; M27/29.

## Bối cảnh

Seed không đủ khi RNG streams, ordering, content/rule versions, clock hoặc numeric serialization thay đổi.

## Quyết định

WorldTick và stable order có contract. RNG có context/version, UI/preview không draw. Save pin manifest; replay rerun command resolution và so canonical state/event hashes.

## Hệ quả và kiểm chứng

Bổ sung ADR-008 đã chọn sha256-counter-v1, mdc1, tick một phút và rounding ties-away-from-zero, kèm reference vectors. FND tasks hiện thực và xác minh; provider vẫn cần spike. Không hứa compatibility giữa version trước khi migration/replay policy được chứng minh.

## Phương án khác

Bác bỏ global System.Random và wall-clock trong gameplay; event-journal inspection không thay deterministic command replay.
