# Sprint7 — Báo cáo kiểm tra chạy lại hành động

## Tổng hợp dễ hiểu

Sprint này xây công cụ giúp tìm lỗi khi game tính ra kết quả khác dù bắt đầu từ cùng trạng thái và nhận cùng chuỗi hành động. Công cụ chạy lại các hành động thật, so kết quả từng bước và chỉ ra bước đầu tiên bị lệch. Nhờ đó, các thay đổi gameplay sau này có thể được kiểm tra trước khi gây sai tiến trình chơi.

Bộ kiểm thử cố định có15thao tác, gồm chuyển tài nguyên, gửi lại lệnh cũ, lệnh sai, từ chối quyền và tiếp tục xử lý công việc theo thời gian. Kết quả cuối được đối chiếu riêng: tài nguyên3/7, thời gian20, hai công việc hoàn tất. Đây là công cụ kiểm thử nền tảng; chưa phải tính năng xem lại ván chơi hoặc bản game chơi thử.

**Đạt scope FND13 headless transfer/scheduler replay**, nghiệmthu2026-10-21. Sprint7/SPIN-010 mở2026-10-20, tiếp tục2026-10-21 sau gián đoạn quota.

## Phân công và review

/root điều phối/ADR016/docs/host/golden/runner/Git. /root/s7_replay tạo Infrastructure harness/codec rồi quota; root tiếp quản build/integration. /root/s7_tests soạn tests và tiếp tục chạy28/28Debugcases đạt. /root/s7_review review thiết kế/source độc lập và finaldeltaCLI/golden.

Review xác nhận Replay gọi lại Execute/DrainAdvance, compatibility trước processor/actions, immutable artifact và strict codec, hashes bind manifest, dừng sai khác đầu tiên. Root bổ sung CLI bounded input64MiB trước allocate và làm rõ report stateHash là full-image integrity. Reviewer yêu cầu testsseed mismatch/knownfailure/unexpectedexception, đã được thêm. Review source không thay runtime acceptance.

## Runtime evidence cuối

Lệnh: `python3 tools/tests/run_runtime.py --dotnet /home/synh/.cache/mythical-dnd/dotnet/dotnet`.

- SDK10.0.401/Linux x64; locked restore, Release build0warnings/0errors.
- **284discovered/284passed/0failed/0skipped**:256regression +28FND13cases. Negativecontrol runtime riêng đúng1fail/exit1, toàn gateexit0.
- Hai subprocess golden15actions đạt cùng final report; replay negativecontrol exit1/index0/categoryresult. Transfer/scheduler demos và9SIGKILLcases từ Sprint5/6 tiếp tục đạt.
- Source fingerprint `sha256:a40d964e46f9ac6cf4a9b7b5e8dbe9fe434d73b4e50b36e3a7a81868f0edd7eb`.
- Evidence `tests/results/sprint07-b_ezo__5/evidence.json`,sources.json,TRX,restore/build/runtime/negative/demo logs,replay.json,persistence-crash.json,save-crash.json. Artifacts ignored; report này được track.
- Run trước sprint07-tfhbyryo có284testspass nhưng gateexit1 vì sourceCLI đổi trong lúc chạy; không dùng fingerprint/evidence đó để nghiệmthu. Finalrun ở trên chạy sau sourcefreeze.

## Acceptance và golden

Golden tại content/fixtures/replay-kernel; artifact checksum cố định bc82e0428cfd260d93b6a62a4eb09e472e73c2e15f8b7dab2b2f27d536423d2b. CI chỉ đọc và kiểm checksum, không tự record/overwrite. Expectedcheckpoint hashes được tạo lần đầu từ C# implementation; final số học được đối chiếu riêng, không giả mọi hash là independent oracle.

Hai process mới chạy lại15actions và phải khớp full expected report. Negativecontrol đổi expected result checkpoint đầu phải trả exit1/index0/categoryresult. Counting rule test chứng minh resolver được gọi thật; altered rule test phát hiện sai khác đầu tiên và dừng, không chỉ so saved images.

Tests bao phủ duplicate/mismatch/stale/rejected/noop/unauthorized attempts, principal riêng với envelope, scheduler budgetpause/resume, culture và dictionary insertion order, malformed/noncanonical/unknown fields/limits, seed/version refusal trước handler, stable pipeline failures và unexpected exception propagation.

Hash gameplay loại metadata receipts; integrity giữ chúng. Cả result/gameplay/integrity/journal/jobs checkpoint hashes bind manifest versions/content/seed. Artifact là trusted QA input, không cấp quyền live command; replay processor mới in-memory không có production store hoặc notification sinks.

## Giới hạn và bước tiếp theo

Transfer/scheduler chưa consume gameplay RNG, nên chỉ nhận evidence bảo toàn contexts/draw index và primitive golden regression; không claim stochastic gameplay replay. Campaign seed optional do host cấp; per-context seed vẫn trong initialimage. Golden content hash là fixturedescriptor, không production content registry.

Diagnostics có action index/category/revisions/hashes, chưa full state-field diff/rule inspector FND15. Không Unity conformance, full gameplay replay, scale benchmark hoặc player UI. Save schema2/recoveryframes của Sprint6 không đổi; durable receipts không đủ để tự suy replay attempts.

Bước tiếp theo FND14 content registry, rồi diagnostics/FND15; không tự mởSprint8 hoặc release. Git/CI/checks cuối tại [WORKLOG](../../WORKLOG.md).
