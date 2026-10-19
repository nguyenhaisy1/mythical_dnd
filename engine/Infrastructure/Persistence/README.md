# Persistence — headless SQLite current state

SqliteWorldStore implements Application ICommittedWorldStore; CommittedWorldCodec mdc1 encodes complete kernel current state, including original envelopes, metadata receipts, event journal and scheduler workflows. Explicit transaction writes current image and relational mirrors, full-image CAS, synchronousFULL/DELETE journal. Application publishes only after commit success; NeedsRecovery prevents using a stale processor after store failure.

Microsoft.Data.Sqlite10.0.13 exact/lock, Linux headless tested. Provider Unity/backend gate OPEN03 remains pending. Current-image reopen is not periodic snapshot fallback/migration/replay. Details docs/adr/014-headless-sqlite-persistence.md and docs/qa/SPRINT-05-EVIDENCE.md.
