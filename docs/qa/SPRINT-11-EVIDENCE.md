# Sprint11 — Báo cáo dữ liệu dân cư và đường đi

## Tổng hợp dễ hiểu

Đã viết bộ24nhân vật với gia đình, kinh nghiệm, nhu cầu và hiểu biết riêng;6hộ có lương/gỗ hữu hạn, quyền sử dụng và nghĩa vụ. Mạng8tuyến nối3cộng đồng địa phương với Bến Hạc ở biên giữ đúng địa lý. Công cụ kiểm tra dữ liệu rồi tạo registry chỉ đọc để bước phát triển sau dùng được.

Đây là authoring data có compiler thật, chưa phải thế giới đang mô phỏng. Routine không thực thi, chưa sinh protagonist, chưa event/eligibility hoặc UI. VS02 full giữpartial; Sprint11 chỉ nghiệmthu VS02A.

## Phân công và review

SPIN014, feature/sprint-11-settlement-content từ dev e7e660b. /root/s11_content viết pack ban đầu, /root/s11_compiler viết initial compiler trướcquota. /root tiếpquản host/parser/schema/tests/integration và sửa ownership/rights/caregiver/canonical hash/namespace; không gán công việc này cho agents.

/root/s11_review rà contract, sauquota tiếp tục độc lập review source/content/schema và chạy focusedtests. Phát hiện C# regex `$` chấp nhận newline cuối token/ID; root sửa bằng absolute endanchor, schema endguard và thêm hai negativecases. Reviewer xác nhận33/33focusedtests, host/schema từchối malformed newline và không còn source/contentblocker. Rootselfreview không tính independentreview.

## Evidence

Pack24actors/6households/4communities/8routes/24claims/24relationships. ContentHash `60ce05463edcc1c2e4abbeae8f30159d430f57613b902ec6d48bb775db193ec8`; fileSHA256 `a17b6bf4135578f171620989ae77adccf6f8f118796ad291cbc7de29ff1cc48a`. Sources M03TG01–03/M04F01/M05Khê-Lục đốichiếu canon; mới ghi prototype-authored/gamecanon. Không lịch sử/khảo cổ claim mới.

Focused C#33tests: membership/typedrefs/canon/age/caregiver/rights/history-provenance/immutable aliases/deterministic order/displayrename/season-direction/knowledge metadata và malformed controls. Processchecker chạy real host21processes,17negativecases; independent Python mdc1 reference đối chiếu content hash. JSON Schema draft2020-12 validated locally với jsonschema; CI semantic gate là portable compiler, không giả CI có chạy jsonschema.

Final Release gate `tests/results/sprint11-r4h28ruo/evidence.json` exit0:371/371tests,0failed,0skipped (33VS02A+338regression). Source fingerprint `sha256:979b4e16dd0ed3ac98641048617457e1f0e76f40f7ba849d8847ad2594f8ab7d`, manifest verifiedfresh. Lockedrestore/build0warnings/errors, negativecontrol đúng1fail/exit1;2demos/replay/content/diagnostics/9SIGKILL regression đạt. Settlement21processes/17negativecases đạt; final checker có missingfile riêng. Lượt1b66z3iz tuy371tests đạt nhưng bị freshnessgate từ chối vì hai sửa đổi tích hợp lúc đang chạy, không dùng cho acceptance. Không coi headless acceptance là actual Unity kiểm profile mới. Sprint10 conformance là kernel profile cũ, không tự mở rộng cho settlementregistry.

## Giới hạn và bước tiếp theo

Registry có thể đọc toàn bộ authored claims cho developer; không dùng làm playerprojection. Source/claim provenance là authored metadata có sourceaudit, không chứng minh điều actor tin là truth. Không có scheduler jobs, trade/rumor propagation hoặc WorldStateinitializer. Không thay save/contentcompatibility của kernelpack cũ.

FullVS02 cần eventtemplates8–12 và declarative eligibility/approvedhandler contracts trước execution; actor-specificobligations còn riêng, TEST001randomprotagonist chưa triển khai. VS03/04 mới tiếp tục nghề và autonomy khi được giao. Không tự mởSprint12 hoặc phát hành.

## Tích hợp

Independent source/content/schema/docsreview đạt; docs/reference/architecture checks đạt. Commit/push feature và PR vào dev sauCI theo standingworkflow; GitSHA/CI/merge thực tế ghi append WORKLOG. Khôngmain/tag/release, không tự mởSprint12.
