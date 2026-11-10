using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Domain.Weaving
{
    public enum WeavingActivityStatus { Running, Completed, Cancelled }

    public sealed class WeavingRecipe
    {
        public const string ProfileId = "personal-weaving-v1";
        public const string RecipeDefinitionId = "recipe.hand-braid-cord@1";
        public const string MaterialDefinitionId = "commodity.fiber@1";
        public const string OutputDefinitionId = "commodity.cord@1";
        public DefinitionId Id { get; } = DefinitionId.Parse(RecipeDefinitionId);
        public DefinitionId MaterialCommodity { get; } = DefinitionId.Parse(MaterialDefinitionId);
        public DefinitionId OutputCommodity { get; } = DefinitionId.Parse(OutputDefinitionId);
        public int FiberCost { get; }
        public long DurationTicks { get; }
        public int CordYield { get; }
        public int MinimumSkill { get; }
        public string Fingerprint { get; }
        public WeavingRecipe(int fiberCost, long durationTicks, int cordYield, int minimumSkill, string fingerprint)
        {
            if (fiberCost < 1 || durationTicks < 1 || cordYield < 1 || minimumSkill < 0 || minimumSkill > 100)
                throw new ArgumentOutOfRangeException(nameof(fiberCost));
            FiberCost = fiberCost; DurationTicks = durationTicks; CordYield = cordYield; MinimumSkill = minimumSkill;
            Fingerprint = Hash(fingerprint);
        }
        internal static string Hash(string value)
        {
            if (value == null || !Regex.IsMatch(value, @"\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant))
                throw new ArgumentException("Expected 64 lowercase hex hash");
            return value;
        }
    }

    public sealed class WeavingActivity
    {
        private static readonly Regex Token = new Regex(@"\A[a-zA-Z0-9][a-zA-Z0-9_.:/-]{0,127}\z", RegexOptions.CultureInvariant);
        public EntityId<Activity> Id { get; }
        public EntityId<Actor> ActorId { get; }
        public EntityId<Job> JobId { get; }
        public WorldTick StartedTick { get; }
        public WorldTick DueTick { get; }
        public string StartCommandId { get; }
        public string CorrelationId { get; }
        public WeavingActivityStatus Status { get; }
        public long ReservedQuantity { get; }
        public string? TerminalReason { get; }
        public WeavingActivity(EntityId<Activity> id, EntityId<Actor> actorId, EntityId<Job> jobId,
            WorldTick startedTick, WorldTick dueTick, string startCommandId, string correlationId,
            WeavingActivityStatus status, long reservedQuantity, string? terminalReason = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id)); ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            JobId = jobId ?? throw new ArgumentNullException(nameof(jobId));
            if (!Enum.IsDefined(typeof(WeavingActivityStatus), status) || dueTick <= startedTick ||
                (status == WeavingActivityStatus.Running ? reservedQuantity <= 0 : reservedQuantity != 0))
                throw new ArgumentException("Invalid weaving activity status/time/hold");
            if (startCommandId == null || !Token.IsMatch(startCommandId) || startCommandId.StartsWith("scheduler/", StringComparison.Ordinal) ||
                correlationId == null || !Token.IsMatch(correlationId)) throw new ArgumentException("Invalid weaving causality");
            if (status == WeavingActivityStatus.Cancelled ? string.IsNullOrWhiteSpace(terminalReason) || terminalReason!.Length > 128 || terminalReason.Any(char.IsControl) : terminalReason != null)
                throw new ArgumentException("Invalid weaving terminal reason");
            StartedTick = startedTick; DueTick = dueTick; StartCommandId = startCommandId; CorrelationId = correlationId;
            Status = status; ReservedQuantity = reservedQuantity; TerminalReason = terminalReason;
        }
        internal WeavingActivity Finish(WeavingActivityStatus status, string? reason = null)
            => new WeavingActivity(Id, ActorId, JobId, StartedTick, DueTick, StartCommandId, CorrelationId, status, 0, reason);
    }

    public sealed class WeavingState
    {
        public const string ProfileId = WeavingRecipe.ProfileId;
        public WeavingRecipe Recipe { get; }
        public string ContentHash { get; }
        public EntityId<Actor> ActorId { get; }
        public EntityId<Settlement> Worksite { get; }
        public EntityId<Lot> InputLotId { get; }
        public EntityId<Lot> OutputLotId { get; }
        public int SkillValue { get; }
        public IReadOnlyList<WeavingActivity> Activities { get; }
        private readonly Dictionary<EntityId<Activity>, WeavingActivity> activities;
        public WeavingState(WeavingRecipe recipe, string contentHash, EntityId<Actor> actorId, EntityId<Settlement> worksite,
            EntityId<Lot> inputLotId, EntityId<Lot> outputLotId, int skillValue, IEnumerable<WeavingActivity>? activities = null)
        {
            Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe)); ContentHash = WeavingRecipe.Hash(contentHash);
            ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId)); Worksite = worksite ?? throw new ArgumentNullException(nameof(worksite));
            InputLotId = inputLotId ?? throw new ArgumentNullException(nameof(inputLotId)); OutputLotId = outputLotId ?? throw new ArgumentNullException(nameof(outputLotId));
            if (inputLotId.Equals(outputLotId) || skillValue < 0 || skillValue > 100) throw new ArgumentException("Invalid personal worksite");
            SkillValue = skillValue;
            this.activities = new Dictionary<EntityId<Activity>, WeavingActivity>();
            foreach (var activity in activities ?? Array.Empty<WeavingActivity>())
            {
                if (activity == null || this.activities.ContainsKey(activity.Id)) throw new ArgumentException("Duplicate/null weaving activity");
                this.activities.Add(activity.Id, activity);
            }
            Activities = Array.AsReadOnly(this.activities.Values.OrderBy(a => a.Id.Value).ToArray());
            if (Activities.Select(a => a.JobId).Distinct().Count() != Activities.Count || Activities.Count(a => a.Status == WeavingActivityStatus.Running) > 1 ||
                Activities.Any(a => !a.ActorId.Equals(ActorId) || a.StartedTick.Add(Recipe.DurationTicks) != a.DueTick ||
                    a.ReservedQuantity != (a.Status == WeavingActivityStatus.Running ? Recipe.FiberCost : 0)))
                throw new ArgumentException("Invalid weaving activity binding");
        }
        public WeavingActivity GetActivity(EntityId<Activity> id)
            => activities.TryGetValue(id ?? throw new ArgumentNullException(nameof(id)), out var value) ? value : throw new IdentityException("REF_NOT_FOUND");
        internal WeavingState With(IEnumerable<WeavingActivity> values)
            => new WeavingState(Recipe, ContentHash, ActorId, Worksite, InputLotId, OutputLotId, SkillValue, values);
        public void Validate(WorldSnapshot world)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            if (world.Origin == null || !world.Origin.Principal.Equals(ActorId)) throw new ArgumentException("Weaving requires origin principal");
            _ = world.GetActor(ActorId); _ = world.GetLot(InputLotId); _ = world.GetLot(OutputLotId);
            if (world.Identity.Resolve<Settlement>(Worksite.Value).Lifecycle != EntityLifecycle.Active) throw new IdentityException("REF_NOT_ACTIVE");
            long holds = 0;
            foreach (var activity in Activities)
            {
                var expected = activity.Status == WeavingActivityStatus.Running ? EntityLifecycle.Active : EntityLifecycle.Retired;
                if (world.Identity.Resolve<Activity>(activity.Id.Value).Lifecycle != expected || world.Identity.Resolve<Job>(activity.JobId.Value).Lifecycle != expected)
                    throw new IdentityException("WEAVING_LIFECYCLE_MISMATCH");
                if (activity.StartedTick > world.Tick || (activity.Status == WeavingActivityStatus.Completed && activity.DueTick > world.Tick))
                    throw new ArgumentException("Weaving activity from future");
                holds = checked(holds + activity.ReservedQuantity);
            }
            if (holds > world.GetLot(InputLotId).Reserved) throw new ArgumentException("Weaving reservation lost");
        }
    }
}
