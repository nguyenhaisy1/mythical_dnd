using System;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Domain.Kernel;
using Game.Foundation.Identity;

namespace Game.Domain.Origins
{
    /// <summary>Immutable genesis assignment, preserved independently of subsequent life/location.</summary>
    public sealed class CampaignOriginState
    {
        public const string ProfileId = OriginSelector.ProfileId;
        public EntityId<Actor> Principal { get; }
        public EntityId<Household> Household { get; }
        public EntityId<Settlement> Home { get; }
        public string ActorDefinitionId { get; }
        public string HouseholdDefinitionId { get; }
        public string CommunityDefinitionId { get; }
        public string SourceContentHash { get; }
        public string CohortContentHash { get; }
        public ulong DrawIndex { get; }

        public CampaignOriginState(EntityId<Actor> principal, EntityId<Household> household,
            EntityId<Settlement> home, string actorDefinitionId, string householdDefinitionId,
            string communityDefinitionId, string sourceContentHash, string cohortContentHash, ulong drawIndex)
        {
            Principal = principal ?? throw new ArgumentNullException(nameof(principal));
            Household = household ?? throw new ArgumentNullException(nameof(household));
            Home = home ?? throw new ArgumentNullException(nameof(home));
            if (Principal.Value.Counter != 1 || Household.Value.Counter != 1 || Home.Value.Counter != 1 ||
                Principal.Value.World != Household.Value.World || Principal.Value.World != Home.Value.World)
                throw new ArgumentException("Origin genesis identity binding mismatch");
            ActorDefinitionId = OriginCandidate.Definition(actorDefinitionId, "actor");
            HouseholdDefinitionId = OriginCandidate.Definition(householdDefinitionId, "household");
            CommunityDefinitionId = OriginCandidate.Definition(communityDefinitionId, "community");
            if (CommunityDefinitionId != OriginSelector.HomeDefinitionId)
                throw new ArgumentException("Origin cohort home mismatch");
            SourceContentHash = Hash(sourceContentHash); CohortContentHash = Hash(cohortContentHash);
            if (drawIndex == 0) throw new ArgumentOutOfRangeException(nameof(drawIndex));
            DrawIndex = drawIndex;
        }

        internal static string Hash(string value)
        {
            if (value == null || !Regex.IsMatch(value, @"\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant))
                throw new ArgumentException("Origin hash must be 64 lowercase hex digits", nameof(value));
            return value;
        }

        public void Validate(WorldSnapshot world)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            void Active<T>(EntityId id)
            {
                if (world.Identity.Resolve<T>(id).Lifecycle != EntityLifecycle.Active)
                    throw new IdentityException("REF_NOT_ACTIVE");
            }
            Active<Actor>(Principal.Value); Active<Household>(Household.Value); Active<Settlement>(Home.Value);
            _ = world.GetActor(Principal);
            var stream = world.RngStates.SingleOrDefault(x => x.ContextKey == OriginSelector.RngContextKey);
            if (stream == null) throw new ArgumentException("Origin assignment RNG missing");
            var context = stream.State.Context;
            if (context.Domain != "characters" || context.Decision != ProfileId || context.Occurrence != "genesis" ||
                context.Stream != "assignment" || !context.Entity.Equals(Principal.Value) ||
                stream.State.DrawIndex != DrawIndex || OriginSelector.DeriveWorldId(context.Seed) != world.WorldId)
                throw new ArgumentException("Origin assignment RNG binding mismatch");
        }
    }
}
