using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;

namespace Game.Domain.Origins
{
    public sealed class OriginSelection
    {
        public OriginCandidate Selected { get; }
        public CampaignOriginState Origin { get; }
        public RngStreamState Rng { get; }
        internal OriginSelection(OriginCandidate selected, CampaignOriginState origin, RngStreamState rng)
        { Selected = selected; Origin = origin; Rng = rng; }
    }

    /// <summary>Seeded selection from an authored cohort; constraints precede the single decision stream.</summary>
    public static class OriginSelector
    {
        public const string ProfileId = "origin-cohort-v1";
        public const string HomeDefinitionId = "community.ben-vac@1";
        public const string RngContextKey = "origin-assignment";

        public static ulong DeriveWorldId(string seed)
        {
            _ = CampaignOriginState.Hash(seed);
            var bytes = CanonicalCodec.Encode(new Dictionary<string, object?> {
                ["profile"] = ProfileId, ["purpose"] = "world-id", ["seed"] = seed });
            using var sha = SHA256.Create(); var digest = sha.ComputeHash(bytes);
            ulong world = 0;
            for (var i = 0; i < 8; i++) world = (world << 8) | digest[i];
            return world == 0 ? 1UL : world;
        }

        public static OriginSelection Select(string seed, string sourceContentHash, string cohortContentHash,
            IEnumerable<OriginCandidate> candidates)
        {
            var world = DeriveWorldId(seed);
            _ = CampaignOriginState.Hash(sourceContentHash); _ = CampaignOriginState.Hash(cohortContentHash);
            if (candidates == null) throw new ArgumentNullException(nameof(candidates));
            var records = candidates.Take(513).ToArray();
            if (records.Length > 512 || records.Any(x => x == null))
                throw new ArgumentException("Invalid origin candidate collection", nameof(candidates));
            if (records.Select(x => x.ActorDefinitionId).Distinct(StringComparer.Ordinal).Count() != records.Length)
                throw new ArgumentException("Duplicate origin candidate", nameof(candidates));
            var cohort = records.Where(x => x.Alive && x.Age >= 18 && x.CommunityDefinitionId == HomeDefinitionId)
                .OrderBy(x => x.ActorDefinitionId, StringComparer.Ordinal).ToArray();
            if (cohort.Length == 0) throw new ArgumentException("Origin cohort is empty", nameof(candidates));
            var principal = new EntityId<Actor>(new EntityId(world, "actor", 1));
            var household = new EntityId<Household>(new EntityId(world, "household", 1));
            var home = new EntityId<Settlement>(new EntityId(world, "settlement", 1));
            var context = new RngContext(seed, "characters", ProfileId, "genesis", principal.Value, "assignment");
            var draw = new RngState(context).Uniform((ulong)cohort.Length);
            var selected = cohort[(int)draw.Value];
            var origin = new CampaignOriginState(principal, household, home, selected.ActorDefinitionId,
                selected.HouseholdDefinitionId, selected.CommunityDefinitionId, sourceContentHash, cohortContentHash,
                draw.Next.DrawIndex);
            return new OriginSelection(selected, origin, new RngStreamState(RngContextKey, draw.Next));
        }
    }
}
