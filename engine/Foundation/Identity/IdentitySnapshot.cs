using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Foundation.Identity
{
    public enum EntityLifecycle { Active, Retired, Tombstone }

    public sealed class AllocationCursor
    {
        public string Kind { get; }
        public ulong NextCounter { get; }
        public bool Exhausted { get; }
        public AllocationCursor(string kind, ulong nextCounter, bool exhausted = false)
        {
            if (!EntityKinds.IsKnown(kind) || nextCounter == 0 || (exhausted && nextCounter != ulong.MaxValue))
                throw new ArgumentException("Invalid allocation cursor");
            Kind = kind; NextCounter = nextCounter; Exhausted = exhausted;
        }
    }

    public sealed class RegistryEntry
    {
        public EntityId Id { get; }
        public EntityLifecycle Lifecycle { get; }
        public ulong CreationRevision { get; }
        public ulong? RetirementRevision { get; }
        public RegistryEntry(EntityId id, EntityLifecycle lifecycle, ulong creationRevision, ulong? retirementRevision = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            if (!Enum.IsDefined(typeof(EntityLifecycle), lifecycle) || creationRevision == 0 ||
                (lifecycle == EntityLifecycle.Active && retirementRevision.HasValue) ||
                (lifecycle != EntityLifecycle.Active && (!retirementRevision.HasValue || retirementRevision.Value < creationRevision)))
                throw new ArgumentException("Invalid registry lifecycle/revisions");
            Lifecycle = lifecycle; CreationRevision = creationRevision; RetirementRevision = retirementRevision;
        }
    }

    /// <summary>Immutable identity state. Storage and world revision coordination belong to Application.</summary>
    public sealed class IdentitySnapshot
    {
        private readonly Dictionary<string, AllocationCursor> cursors;
        private readonly Dictionary<EntityId, RegistryEntry> entries;
        public ulong World { get; }
        public ulong Revision { get; }
        public IReadOnlyList<AllocationCursor> Cursors { get; }
        public IReadOnlyList<RegistryEntry> Entries { get; }

        private IdentitySnapshot(ulong world, ulong revision, IEnumerable<AllocationCursor> sourceCursors, IEnumerable<RegistryEntry> sourceEntries)
        {
            if (world == 0) throw new ArgumentOutOfRangeException(nameof(world));
            World = world; Revision = revision;
            cursors = sourceCursors.ToDictionary(c => c.Kind, StringComparer.Ordinal);
            if (cursors.Count != EntityKinds.All.Count || EntityKinds.All.Any(k => !cursors.ContainsKey(k)))
                throw new ArgumentException("Restore requires every v1 allocation cursor");
            entries = sourceEntries.ToDictionary(e => e.Id);
            foreach (var entry in entries.Values)
            {
                var cursor = cursors[entry.Id.Kind];
                if (entry.Id.World != world || entry.CreationRevision > revision || entry.RetirementRevision > revision ||
                    (!cursor.Exhausted && entry.Id.Counter >= cursor.NextCounter))
                    throw new ArgumentException("Registry/cursor/world/revision mismatch");
            }
            Cursors = Array.AsReadOnly(cursors.Values.OrderBy(c => c.Kind, StringComparer.Ordinal).ToArray());
            Entries = Array.AsReadOnly(entries.Values.OrderBy(e => e.Id).ToArray());
        }
        public static IdentitySnapshot Empty(ulong world) => new IdentitySnapshot(world, 0,
            EntityKinds.All.Select(k => new AllocationCursor(k, 1)), Array.Empty<RegistryEntry>());
        public static IdentitySnapshot Restore(ulong world, ulong revision, IEnumerable<AllocationCursor> cursors, IEnumerable<RegistryEntry> entries)
            => new IdentitySnapshot(world, revision, cursors, entries);
        public IdentityStage BeginStage(ulong targetRevision)
        {
            if (targetRevision <= Revision) throw new ArgumentOutOfRangeException(nameof(targetRevision));
            return new IdentityStage(this, targetRevision);
        }
        public AllocationCursor Cursor(string kind) => cursors.TryGetValue(kind, out var cursor)
            ? cursor : throw new IdentityException("REF_TYPE_MISMATCH");
        public RegistryEntry Resolve<T>(EntityId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (id.World != World) throw new IdentityException("REF_WORLD_MISMATCH");
            if (id.Kind != EntityKinds.For<T>()) throw new IdentityException("REF_TYPE_MISMATCH");
            return entries.TryGetValue(id, out var entry) ? entry : throw new IdentityException("REF_NOT_FOUND");
        }
    }

    /// <summary>Private staging workspace. Prepared state must still join the atomic world commit.</summary>
    public sealed class IdentityStage
    {
        private readonly IdentitySnapshot source;
        private readonly ulong targetRevision;
        private readonly Dictionary<string, AllocationCursor> cursors;
        private readonly Dictionary<EntityId, RegistryEntry> entries;
        private readonly HashSet<EntityId> staged = new HashSet<EntityId>();
        private bool closed;
        internal IdentityStage(IdentitySnapshot source, ulong targetRevision)
        {
            this.source = source; this.targetRevision = targetRevision;
            cursors = source.Cursors.ToDictionary(c => c.Kind, StringComparer.Ordinal);
            entries = source.Entries.ToDictionary(e => e.Id);
        }
        private void EnsureOpen()
        {
            if (closed) throw new InvalidOperationException("Identity stage is closed");
        }
        public EntityId<T> Allocate<T>()
        {
            EnsureOpen();
            var kind = EntityKinds.For<T>();
            var cursor = cursors[kind];
            if (cursor.Exhausted) throw new IdentityException("ID_EXHAUSTED");
            var id = new EntityId(source.World, kind, cursor.NextCounter);
            cursors[kind] = cursor.NextCounter == ulong.MaxValue
                ? new AllocationCursor(kind, ulong.MaxValue, true)
                : new AllocationCursor(kind, cursor.NextCounter + 1);
            staged.Add(id);
            return new EntityId<T>(id);
        }
        public void Retire<T>(EntityId<T> id)
        {
            EnsureOpen();
            if (id == null) throw new ArgumentNullException(nameof(id));
            // Newly staged IDs cannot retire before they have a committed history.
            var existing = source.Resolve<T>(id.Value);
            if (entries[existing.Id].Lifecycle != EntityLifecycle.Active)
                throw new IdentityException("INVALID_LIFECYCLE_TRANSITION");
            entries[existing.Id] = new RegistryEntry(existing.Id, EntityLifecycle.Retired, existing.CreationRevision, targetRevision);
        }
        public void Tombstone<T>(EntityId<T> id, bool retentionAllowsPayloadRemoval)
        {
            EnsureOpen();
            if (id == null) throw new ArgumentNullException(nameof(id));
            var existing = source.Resolve<T>(id.Value);
            var current = entries[existing.Id];
            if (!retentionAllowsPayloadRemoval || current.Lifecycle != EntityLifecycle.Retired)
                throw new IdentityException("INVALID_LIFECYCLE_TRANSITION");
            entries[existing.Id] = new RegistryEntry(existing.Id, EntityLifecycle.Tombstone, current.CreationRevision, current.RetirementRevision);
        }
        public IdentitySnapshot PrepareSnapshot()
        {
            EnsureOpen();
            // Staged entries become Active only in the candidate snapshot, never in source.
            var records = entries.Values.Concat(staged.Select(id => new RegistryEntry(id, EntityLifecycle.Active, targetRevision)));
            var result = IdentitySnapshot.Restore(source.World, targetRevision, cursors.Values, records);
            closed = true;
            return result;
        }
        public void Abort() { EnsureOpen(); closed = true; }
    }
}
