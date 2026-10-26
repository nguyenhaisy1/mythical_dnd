# Sprint11 — Bộ dân cư và đường đi của Bến qua mùa nước

## Tổng hợp dễ hiểu

Chuyển dữ liệu thiết kế thành bộ dân cư có gia đình, lịch sử nghề, hiểu biết, nhu cầu và quan hệ riêng; kèm tài sản hộ gia đình và đường đi có hướng. Compiler kiểm tra dữ liệu trước khi dùng trong phát triển game. Đây là dữ liệu nền, chưa phải NPC đang tự sinh hoạt hoặc game chơi thử.

SPIN-014, VS02A. Scope theo ADR020:20–30actors,6–8households,3local communities+externalBếnHạc,6–10directed routes, immutable portable registry/strict host importer/QA. VS02 full giữpartial vì8–12event templates/causal eligibility chưa có runtime contract.

| Task | Actor/ownership | Acceptance |
|---|---|---|
| S11-01 | /root:ADR020/planning/QA/integration/docs | Profile rõ, evidence/checks và Git |
| S11-02 | /root/s11_content bản đầu → /root hoàn thiện:content/definitions/vertical-slice | Distinct authored roster, provenance/canon, đúngprofile |
| S11-03 | /root/s11_compiler initial compiler → /root hoàn thiện compiler/host/schema | Strict compiler/immutable hash/query, no statewrites |
| S11-04 | /root:tests/processchecks | Real pack + negativecontrols + fullregression |
| S11-05 | /root/s11_review độc lập sau implementation | Source/content/evidence review và sửa findings trướcmerge |

Không sửa immutable source archive, không game simulation/protagonist generator, không runtimeLLM/UnityUI. Bước kế tiếp là VS02B event/eligibility contracts khi được giao; không tự mởSprint12.

## Kết quả và giới hạn

[Báo cáo Sprint11](../../qa/SPRINT-11-EVIDENCE.md) ghi actor/evidence thực. Reviewer độc lập tìm lỗi newline cuối ID, root sửa và bổ sung regression; source/content/schema/docs review đạt. VS02A hoàn thành khi final gate/CI/integration được ghi trong WORKLOG, fullVS02 vẫnpartial.
