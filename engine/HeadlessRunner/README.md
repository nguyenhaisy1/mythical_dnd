# HeadlessRunner

Composition root/CLI. --status trả capability flags; --transfer-demo kiểm transfer10/0→7/3 và duplicate event consumer. --scheduler-demo lập2jobs tại tick10/20, advance30, kiểm transfer5/5, revision7 và retry receipt pending gốc. Exit0 chỉ khi assertions thực đạt.

Đây là diagnostic fixture, chưa là UI/playable game. Có scheduler in-memory; SQLite current-state persistence probe đã có; schema2 snapshot/recovery có API và save probe; full simulation chưa có; replay QA CLI được bổ sung Sprint7. Xem docs/qa/SPRINT-04-EVIDENCE.md.

--persistence-probe <db> init|recover|crash [faultpoint] là host test harness. Crash mode chờ tại barrier để Python SIGKILL process thật; không dùng cho UI. Init yêu cầu file chưa tồn tại. Xem tools/tests/check_persistence_crash.py và Sprint5 evidence.

--save-probe <db> init|init-legacy|snapshot|migrate|inspect [faultpoint] kiểm snapshot/migration trên file thật. tools/tests/check_save_crash.py kill process tại snapshot và migration boundaries rồi mở lại bằng C#; đây không phải bằng chứng mất điện hoặc UI Save/Load. Migration probe tạo <db>.backup và <db>.migrated, không overwrite nguồn.

## Replay QA

--replay <artifact> <expected-content-hash> [world-seed] chạy lại Execute/Drain trên processor in-memory mới và trả JSON/exit0 chỉ khi mọi checkpoint khớp. Host chọn compiled rule catalog, expected manifest độc lập artifact. --replay-negative-control thay expected result checkpoint đầu để kiểm exit1 và first divergence; chỉ phục vụ test.

--record-replay-fixture <new-path> <content-hash> là thao tác tạo baseline có chủ đích, FileMode.CreateNew không ghi đè golden. CI không gọi record. Golden15actions tại content/fixtures/replay-kernel được chạy2process riêng. Chưa stochastic gameplay/Unity replay; returned diagnostic là World Truth, không player UI.

Replay CLI giới hạn64MiB trước khi allocate file buffer; report stateHash là SHA256 full committed image (gồm metadata), không là manifest-bound GameplayHash của checkpoint.
