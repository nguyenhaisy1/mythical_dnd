# ADR025 — Gán xuất thân từ cohort và Reveal lưu bền

06/11/2026 · Sprint16 / SPIN-040 · Quyết định kỹ thuật trong sprint được giao.

M07 cho phép chọn character hợp lệ theo world seed hoặc sinh mới từ World Graph. Sau proof15 một fixed actor, bước độc lập tiếp theo là origin-cohort-v1: chọn một trong các authored adult tại Bến Vạc, giữ history→skills/claims và đưa vào New/Reveal/Continue. Không gọi roster selection là full life-history generation/Character Viability; TEST001/fullVS02 vẫn partial.

[Contract](../contracts/ORIGIN-COHORT-PROFILE.md) khóa admission/RNG/context/world-ID/DTO/storage/UI và năm gates. Origin là canonical WorldSnapshot extension image3, không sidecar; physicalSQLite2 giữ và rule/content profile mới exact. Legacy image1/2, repair factory15, G13v1 và thư viện01–18 giữ nguyên. New pendingDB verify rồi create-new publish trong root riêng, idempotent request ID và cancellation/fault/crash semantics rõ.

Cohort Bến Vạc chưa có boat-repair skill. Không cấp nghề/quyền giả, teleport người Cồn Mây hoặc đổi repair fixture thành generic để làm demo. World proof16 chỉ self record/time/save; actual repair15 regression riêng. Begin là screen transition, Continue luôn Reveal lại cùng assignment rồi vào World tại savedtick. Không seed/origin editor hoặc Reveal reroll.

Source selection refs/limits/hash và resource derived từ pack Sprint11, không sửa pack/archive. UI neutral marker không là đủ art của8characters; technical UI không là owner final art/UX approval. Future fuller lifecycle/knowledge/worldbinding cần profiles/tasks riêng, không tự mở Sprint17/release.
