# Identity — FND-02

Portable identity-v1 implementation. `EntityId<T>` uses the closed v1 kind vocabulary; `EntityId` is the canonical wire/reference boundary, `DefinitionId` is a separate type. Null is represented by nullable references, never a zero/default ID. Hash codes are for in-memory lookup only; persist `ToString()` and sort by `EntityId.CompareTo`, never persist hash codes or enumerate dictionary order.

`IdentitySnapshot` is immutable; `BeginStage(targetRevision)` copies allocator/registry state. `Allocate<T>` returns a provisional ID usable only inside the proposal. `Abort` leaves the source unchanged. `PrepareSnapshot` seals a candidate; Application must atomically publish it with other world state and guard stale revisions (FND-05/08). It is not durable storage, a command processor or a concurrent world commit.

Restore includes all per-kind cursors, even kinds without records; missing/duplicate/cross-world/inconsistent records reject. Retired/tombstoned records remain resolvable. The caller supplies retention eligibility when tombstoning; actual retention policy and payload cleanup belong to future persistence tasks. Actor death does not invoke retirement automatically.

Contract: `docs/contracts/IDENTITY-SPEC.md`; executable coverage: `tests/MythicalDnd.Foundation.Tests`. Unity conformance remains FND-16.
