# Game.Domain

Kernel/ có immutable WorldSnapshot (actor/lot/identity/time/RNG subset), typed approved registry và TransferResourceRule pure. Chỉ phụ thuộc Foundation; snapshot omniscient dùng nội bộ, không là player projection. Các gameplay domains khác chưa triển khai. Xem docs/adr/011-in-memory-command-pipeline.md.
