using System;
using System.Collections.Generic;
using System.Linq;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using Game.Foundation.World;

namespace Game.Domain.Kernel
{
    public sealed class ActorState
    {
        public EntityId<Actor> Id { get; }
        public bool Alive { get; }
        public EntityId<Settlement> Location { get; }
        public IReadOnlyList<EntityId> ControlledOwnerIds { get; }
        public ActorState(EntityId<Actor> id, bool alive, EntityId<Settlement> location, IEnumerable<EntityId>? controlledOwnerIds = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Location = location ?? throw new ArgumentNullException(nameof(location)); Alive = alive;
            var owners = (controlledOwnerIds ?? Array.Empty<EntityId>()).Concat(new[] { id.Value }).ToArray();
            if (owners.Any(x => x == null || x.World != id.Value.World || (x.Kind != "actor" && x.Kind != "household")))
                throw new ArgumentException("Invalid control reference");
            ControlledOwnerIds = Array.AsReadOnly(owners.Distinct().OrderBy(x => x).ToArray());
        }
        public bool Controls(EntityId owner) => ControlledOwnerIds.Contains(owner);
    }

    public sealed class ResourceLot
    {
        public EntityId<Lot> Id { get; }
        public DefinitionId Commodity { get; }
        public EntityId Owner { get; }
        public EntityId<Settlement> Location { get; }
        public long OnHand { get; }
        public long Reserved { get; }
        public long Available => OnHand - Reserved;
        public ResourceLot(EntityId<Lot> id, DefinitionId commodity, EntityId owner, EntityId<Settlement> location, long onHand, long reserved)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Commodity = commodity ?? throw new ArgumentNullException(nameof(commodity));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            if (owner.Kind != "actor" && owner.Kind != "household") throw new ArgumentException("Invalid asset owner");
            if (onHand < 0 || reserved < 0 || reserved > onHand) throw new ArgumentOutOfRangeException(nameof(onHand));
            OnHand = onHand; Reserved = reserved;
        }
    }

    public sealed class RngStreamState
    {
        public string ContextKey { get; }
        public RngState State { get; }
        public RngStreamState(string contextKey, RngState state)
        {
            if (string.IsNullOrWhiteSpace(contextKey)) throw new ArgumentException("Empty RNG context key");
            ContextKey = contextKey; State = state ?? throw new ArgumentNullException(nameof(state));
        }
    }

    /// <summary>Omniscient immutable rule input, never a player-facing projection.</summary>
    public sealed class WorldSnapshot
    {
        private readonly Dictionary<EntityId<Actor>, ActorState> actors;
        private readonly Dictionary<EntityId<Lot>, ResourceLot> lots;
        public ulong WorldId => Identity.World;
        public WorldRevision Revision => new WorldRevision(Identity.Revision);
        public WorldTick Tick { get; }
        public IdentitySnapshot Identity { get; }
        public IReadOnlyList<ActorState> Actors { get; }
        public IReadOnlyList<ResourceLot> Lots { get; }
        public IReadOnlyList<RngStreamState> RngStates { get; }
        public WorldSnapshot(IdentitySnapshot identity, WorldTick tick, IEnumerable<ActorState> actors, IEnumerable<ResourceLot> lots, IEnumerable<RngStreamState>? rngStates = null)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity)); Tick = tick;
            this.actors = (actors ?? throw new ArgumentNullException(nameof(actors))).ToDictionary(x => x.Id);
            this.lots = (lots ?? throw new ArgumentNullException(nameof(lots))).ToDictionary(x => x.Id);
            foreach (var actor in this.actors.Values)
            {
                RequireActive<Actor>(actor.Id.Value); RequireActive<Settlement>(actor.Location.Value);
                foreach (var owner in actor.ControlledOwnerIds) RequireOwner(owner);
            }
            foreach (var lot in this.lots.Values)
            {
                RequireActive<Lot>(lot.Id.Value); RequireActive<Settlement>(lot.Location.Value); RequireOwner(lot.Owner);
                if (lot.Owner.Kind == "actor" && !this.actors.ContainsKey(new EntityId<Actor>(lot.Owner)))
                    throw new IdentityException("REF_NOT_FOUND");
            }
            Actors = Array.AsReadOnly(this.actors.Values.OrderBy(x => x.Id.Value).ToArray());
            Lots = Array.AsReadOnly(this.lots.Values.OrderBy(x => x.Id.Value).ToArray());
            var streams = (rngStates ?? Array.Empty<RngStreamState>()).ToDictionary(x => x.ContextKey, StringComparer.Ordinal);
            foreach (var stream in streams.Values)
                if (stream.State.Context.Entity.World != WorldId || !Identity.Entries.Any(x => x.Id.Equals(stream.State.Context.Entity)))
                    throw new IdentityException("REF_NOT_FOUND");
            RngStates = Array.AsReadOnly(streams.Values.OrderBy(x => x.ContextKey, StringComparer.Ordinal).ToArray());
        }
        private void RequireOwner(EntityId id)
        {
            if (id.Kind == "actor") RequireActive<Actor>(id);
            else if (id.Kind == "household") RequireActive<Household>(id);
            else throw new IdentityException("REF_TYPE_MISMATCH");
        }
        private void RequireActive<T>(EntityId id)
        {
            if (Identity.Resolve<T>(id).Lifecycle != EntityLifecycle.Active) throw new IdentityException("REF_NOT_ACTIVE");
        }
        public ActorState GetActor(EntityId<Actor> id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            RequireActive<Actor>(id.Value);
            return actors.TryGetValue(id, out var value) ? value : throw new IdentityException("REF_NOT_FOUND");
        }
        public ResourceLot GetLot(EntityId<Lot> id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            RequireActive<Lot>(id.Value);
            return lots.TryGetValue(id, out var value) ? value : throw new IdentityException("REF_NOT_FOUND");
        }
    }
}
