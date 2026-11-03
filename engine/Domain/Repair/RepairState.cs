using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;

namespace Game.Domain.Repair
{
    public enum RepairItemKind { Boat, RepairTool }
    public enum RepairActivityStatus { Running, Completed, Cancelled }

    /// <summary>Immutable trusted recipe. Client commands select its ID, never its numeric effects.</summary>
    public sealed class RepairRecipe
    {
        public DefinitionId Id { get; }
        public DefinitionId MaterialCommodity { get; }
        public long MaterialQuantity { get; }
        public long DurationTicks { get; }
        public int ConditionGain { get; }
        public int ConditionMax { get; }
        public string Fingerprint { get; }
        public RepairRecipe(DefinitionId id, DefinitionId materialCommodity, long materialQuantity,
            long durationTicks, int conditionGain, int conditionMax)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            MaterialCommodity = materialCommodity ?? throw new ArgumentNullException(nameof(materialCommodity));
            if (materialQuantity < 1 || durationTicks < 1 || conditionGain < 1 || conditionMax < 1 || conditionMax > 1000 || conditionGain > conditionMax)
                throw new ArgumentOutOfRangeException(nameof(materialQuantity), "Invalid repair recipe parameters");
            MaterialQuantity = materialQuantity; DurationTicks = durationTicks;
            ConditionGain = conditionGain; ConditionMax = conditionMax;
            Fingerprint = CanonicalCodec.Hash(new Dictionary<string, object?> {
                ["recipe_id"] = Id.ToString(), ["material_commodity"] = MaterialCommodity.ToString(),
                ["material_quantity"] = MaterialQuantity, ["duration_ticks"] = DurationTicks,
                ["condition_gain"] = ConditionGain, ["condition_max"] = ConditionMax });
        }
    }

    public sealed class RepairItemState
    {
        public EntityId<Item> Id { get; }
        public RepairItemKind Kind { get; }
        public EntityId Owner { get; }
        public EntityId<Settlement> Location { get; }
        public int Condition { get; }
        public bool Destroyed { get; }
        public RepairItemState(EntityId<Item> id, RepairItemKind kind, EntityId owner,
            EntityId<Settlement> location, int condition, bool destroyed = false)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            if (!Enum.IsDefined(typeof(RepairItemKind), kind) || (owner.Kind != "actor" && owner.Kind != "household"))
                throw new ArgumentException("Invalid repair item metadata");
            if (condition < 0 || condition > 1000) throw new ArgumentOutOfRangeException(nameof(condition));
            Kind = kind; Condition = condition; Destroyed = destroyed;
        }
    }

    /// <summary>Bounded eligibility input supplied by trusted composition, not a full skill system.</summary>
    public sealed class RepairCapability
    {
        public EntityId<Actor> ActorId { get; }
        public bool Qualified { get; }
        public EntityId<Item> ToolItemId { get; }
        public RepairCapability(EntityId<Actor> actorId, bool qualified, EntityId<Item> toolItemId)
        {
            ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            ToolItemId = toolItemId ?? throw new ArgumentNullException(nameof(toolItemId)); Qualified = qualified;
        }
    }

    public sealed class RepairActivity
    {
        private static readonly Regex Token = new Regex(@"\A[a-zA-Z0-9][a-zA-Z0-9_.:/-]{0,127}\z", RegexOptions.CultureInvariant);
        public EntityId<Activity> Id { get; }
        public EntityId<Actor> ActorId { get; }
        public EntityId<Item> ItemId { get; }
        public EntityId<Lot> MaterialLotId { get; }
        public EntityId<Item> ToolItemId { get; }
        public EntityId<Settlement> Worksite { get; }
        public EntityId<Job> JobId { get; }
        public WorldTick StartedTick { get; }
        public WorldTick DueTick { get; }
        public RepairRecipe Recipe { get; }
        public string StartCommandId { get; }
        public string CorrelationId { get; }
        public RepairActivityStatus Status { get; }
        /// <summary>Material hold with provenance Id -> MaterialLotId; zero after terminal transition.</summary>
        public long ReservedQuantity { get; }
        public string? TerminalReason { get; }
        public RepairActivity(EntityId<Activity> id, EntityId<Actor> actorId, EntityId<Item> itemId,
            EntityId<Lot> materialLotId, EntityId<Item> toolItemId, EntityId<Settlement> worksite,
            EntityId<Job> jobId, WorldTick startedTick, WorldTick dueTick, RepairRecipe recipe,
            string startCommandId, string correlationId, RepairActivityStatus status,
            long reservedQuantity, string? terminalReason = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id)); ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            ItemId = itemId ?? throw new ArgumentNullException(nameof(itemId)); MaterialLotId = materialLotId ?? throw new ArgumentNullException(nameof(materialLotId));
            ToolItemId = toolItemId ?? throw new ArgumentNullException(nameof(toolItemId)); Worksite = worksite ?? throw new ArgumentNullException(nameof(worksite));
            JobId = jobId ?? throw new ArgumentNullException(nameof(jobId)); Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
            if (!Enum.IsDefined(typeof(RepairActivityStatus), status) || startedTick.Add(recipe.DurationTicks) != dueTick)
                throw new ArgumentException("Invalid repair activity time/status");
            if (startCommandId == null || !Token.IsMatch(startCommandId) || startCommandId.StartsWith("scheduler/", StringComparison.Ordinal) ||
                correlationId == null || !Token.IsMatch(correlationId)) throw new ArgumentException("Invalid repair causality");
            if (reservedQuantity != (status == RepairActivityStatus.Running ? recipe.MaterialQuantity : 0))
                throw new ArgumentException("Invalid activity hold");
            if (status == RepairActivityStatus.Cancelled ? string.IsNullOrWhiteSpace(terminalReason) : terminalReason != null)
                throw new ArgumentException("Invalid terminal reason");
            StartedTick = startedTick; DueTick = dueTick; StartCommandId = startCommandId; CorrelationId = correlationId;
            Status = status; ReservedQuantity = reservedQuantity; TerminalReason = terminalReason;
        }
        internal RepairActivity Finish(RepairActivityStatus status, string? reason = null) => new RepairActivity(Id, ActorId, ItemId,
            MaterialLotId, ToolItemId, Worksite, JobId, StartedTick, DueTick, Recipe, StartCommandId,
            CorrelationId, status, 0, reason);
    }

    /// <summary>Optional immutable extension. Its presence, even when empty, selects repair image/profile semantics.</summary>
    public sealed class RepairState
    {
        public const string ProfileId = "boat-repair-v1";
        public RepairRecipe Recipe { get; }
        public IReadOnlyList<RepairItemState> Items { get; }
        public IReadOnlyList<RepairCapability> Capabilities { get; }
        public IReadOnlyList<RepairActivity> Activities { get; }
        private readonly Dictionary<EntityId<Item>, RepairItemState> items;
        private readonly Dictionary<EntityId<Actor>, RepairCapability> capabilities;
        private readonly Dictionary<EntityId<Activity>, RepairActivity> activities;
        public RepairState(RepairRecipe recipe, IEnumerable<RepairItemState> items,
            IEnumerable<RepairCapability> capabilities, IEnumerable<RepairActivity>? activities = null)
        {
            Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
            this.items = CopyUnique(items, x => x.Id, nameof(items));
            this.capabilities = CopyUnique(capabilities, x => x.ActorId, nameof(capabilities));
            this.activities = CopyUnique(activities ?? Array.Empty<RepairActivity>(), x => x.Id, nameof(activities));
            Items = Array.AsReadOnly(this.items.Values.OrderBy(x => x.Id.Value).ToArray());
            Capabilities = Array.AsReadOnly(this.capabilities.Values.OrderBy(x => x.ActorId.Value).ToArray());
            Activities = Array.AsReadOnly(this.activities.Values.OrderBy(x => x.Id.Value).ToArray());
            var running = Activities.Where(a => a.Status == RepairActivityStatus.Running).ToArray();
            if (running.GroupBy(a => a.ActorId).Any(g => g.Count() != 1) || running.GroupBy(a => a.ItemId).Any(g => g.Count() != 1) ||
                running.GroupBy(a => a.ToolItemId).Any(g => g.Count() != 1) || Activities.GroupBy(a => a.JobId).Any(g => g.Count() != 1))
                throw new ArgumentException("Concurrent repair reservation conflict");
            if (Activities.Any(a => a.Recipe.Fingerprint != Recipe.Fingerprint)) throw new ArgumentException("Repair recipe mismatch");
        }
        private static Dictionary<TKey,T> CopyUnique<T,TKey>(IEnumerable<T> values, Func<T,TKey> key, string name) where T:class where TKey:notnull
        {
            if (values == null) throw new ArgumentNullException(name);
            var result = new Dictionary<TKey,T>();
            foreach (var value in values)
            {
                if (value == null) throw new ArgumentException("Null repair record", name);
                if (result.ContainsKey(key(value))) throw new ArgumentException("Duplicate repair record", name);
                result.Add(key(value), value);
            }
            return result;
        }
        public RepairItemState GetItem(EntityId<Item> id) => items.TryGetValue(id ?? throw new ArgumentNullException(nameof(id)), out var value) ? value : throw new IdentityException("REF_NOT_FOUND");
        public RepairCapability GetCapability(EntityId<Actor> actorId) => capabilities.TryGetValue(actorId ?? throw new ArgumentNullException(nameof(actorId)), out var value) ? value : throw new IdentityException("REF_NOT_FOUND");
        public RepairActivity GetActivity(EntityId<Activity> id) => activities.TryGetValue(id ?? throw new ArgumentNullException(nameof(id)), out var value) ? value : throw new IdentityException("REF_NOT_FOUND");
        internal RepairState With(IEnumerable<RepairItemState>? newItems = null, IEnumerable<RepairActivity>? newActivities = null)
            => new RepairState(Recipe, newItems ?? Items, Capabilities, newActivities ?? Activities);

        /// <summary>Cross-record/reference invariants; eligibility is rechecked by rules, not assumed here.</summary>
        public void Validate(WorldSnapshot world)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            void Active<T>(EntityId id)
            {
                if (world.Identity.Resolve<T>(id).Lifecycle != EntityLifecycle.Active) throw new IdentityException("REF_NOT_ACTIVE");
            }
            foreach (var item in Items)
            {
                Active<Item>(item.Id.Value); Active<Settlement>(item.Location.Value);
                if (item.Owner.Kind == "actor") { Active<Actor>(item.Owner); world.GetActor(new EntityId<Actor>(item.Owner)); }
                else Active<Household>(item.Owner);
            }
            foreach (var capability in Capabilities)
            {
                world.GetActor(capability.ActorId); Active<Item>(capability.ToolItemId.Value);
                if (GetItem(capability.ToolItemId).Kind != RepairItemKind.RepairTool) throw new ArgumentException("Capability tool kind mismatch");
            }
            foreach (var activity in Activities)
            {
                var expected = activity.Status == RepairActivityStatus.Running ? EntityLifecycle.Active : EntityLifecycle.Retired;
                if (world.Identity.Resolve<Activity>(activity.Id.Value).Lifecycle != expected || world.Identity.Resolve<Job>(activity.JobId.Value).Lifecycle != expected)
                    throw new IdentityException("REPAIR_LIFECYCLE_MISMATCH");
                world.GetActor(activity.ActorId); world.GetLot(activity.MaterialLotId);
                Active<Settlement>(activity.Worksite.Value); Active<Item>(activity.ItemId.Value); Active<Item>(activity.ToolItemId.Value);
                if (GetItem(activity.ItemId).Kind != RepairItemKind.Boat || GetItem(activity.ToolItemId).Kind != RepairItemKind.RepairTool)
                    throw new ArgumentException("Activity item kind mismatch");
                if (activity.StartedTick > world.Tick || (activity.Status == RepairActivityStatus.Completed && activity.DueTick > world.Tick))
                    throw new ArgumentException("Activity from future");
            }
            foreach (var holds in Activities.Where(a => a.Status == RepairActivityStatus.Running).GroupBy(a => a.MaterialLotId))
            {
                long total = 0;
                try { foreach (var activity in holds) total = checked(total + activity.ReservedQuantity); }
                catch (OverflowException) { throw new ArgumentException("Repair hold overflow"); }
                if (total > world.GetLot(holds.Key).Reserved) throw new ArgumentException("Repair reservation lost");
            }
        }
    }
}
