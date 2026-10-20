# HeadlessRunner

Composition root/CLI. --status trả capability flags; --transfer-demo kiểm transfer10/0→7/3 và duplicate event consumer. --scheduler-demo lập2jobs tại tick10/20, advance30, kiểm transfer5/5, revision7 và retry receipt pending gốc. Exit0 chỉ khi assertions thực đạt.

Đây là diagnostic fixture, chưa là UI/playable game. Có scheduler in-memory; SQLite current-state persistence probe đã có; schema2 snapshot/recovery có API và save probe; full simulation và replay CLI chưa có. Xem docs/qa/SPRINT-04-EVIDENCE.md.

--persistence-probe <db> init|recover|crash [faultpoint] là host test harness. Crash mode chờ tại barrier để Python SIGKILL process thật; không dùng cho UI. Init yêu cầu file chưa tồn tại. Xem tools/tests/check_persistence_crash.py và Sprint5 evidence.

--save-probe <db> init|init-legacy|snapshot|migrate|inspect [faultpoint] kiểm snapshot/migration trên file thật. tools/tests/check_save_crash.py kill process tại snapshot và migration boundaries rồi mở lại bằng C#; đây không phải bằng chứng mất điện hoặc UI Save/Load. Migration probe tạo <db>.backup và <db>.migrated, không overwrite nguồn.
