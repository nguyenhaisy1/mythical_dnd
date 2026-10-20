# Persistence — headless SQLite current state

SqliteWorldStore implements Application ICommittedWorldStore; CommittedWorldCodec mdc1 encodes complete kernel current state, including original envelopes, metadata receipts, event journal and scheduler workflows. Explicit transaction writes current image and relational mirrors, full-image CAS, synchronousFULL/DELETE journal. Application publishes only after commit success; NeedsRecovery prevents using a stale processor after store failure.

Microsoft.Data.Sqlite10.0.13 exact/lock, Linux headless tested. Provider Unity/backend gate OPEN03 remains pending. Current-image reopen is not periodic snapshot fallback/migration/replay. Details docs/adr/014-headless-sqlite-persistence.md and docs/qa/SPRINT-05-EVIDENCE.md.

## Save manager schema2

Initialize remains legacy schema1; InitializeSave creates schema2 with an initial snapshot and checksummed manifest. CreateSnapshot(expected) uses full-image CAS and preserves game state; periodic snapshots occur every100 accepted mutations independent of manual saves. RecoverToNewFile validates an older snapshot plus a complete full-image suffix to the saved head, without executing rules or delivering events. Recovered output retains the periodic mutation counter and reanchors sequence0.

MigrateToNewFile supports schema1→2, creating a consistent backup and a validated new file. It never replaces the source or existing output paths. Backups are not pruned. WorldSeed is optional explicit host metadata; unavailable seed is not invented from RNG contexts. See ADR015. Rule replay, Unity quit/UI wiring and platform atomic replacement remain separate gates.
