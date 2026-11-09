using System;
using Game.Foundation.Identity;

namespace Game.Domain.Origins
{
    /// <summary>Typed authored admission data; not a generated biography or a viability proof.</summary>
    public sealed class OriginCandidate
    {
        public string ActorDefinitionId { get; }
        public string HouseholdDefinitionId { get; }
        public string CommunityDefinitionId { get; }
        public bool Alive { get; }
        public int Age { get; }

        public OriginCandidate(string actorDefinitionId, string householdDefinitionId,
            string communityDefinitionId, bool alive, int age)
        {
            ActorDefinitionId = Definition(actorDefinitionId, "actor");
            HouseholdDefinitionId = Definition(householdDefinitionId, "household");
            CommunityDefinitionId = Definition(communityDefinitionId, "community");
            if (age < 0 || age > 100) throw new ArgumentOutOfRangeException(nameof(age));
            Alive = alive; Age = age;
        }

        internal static string Definition(string value, string kind)
        {
            _ = DefinitionId.Parse(value);
            if (!value.StartsWith(kind + ".", StringComparison.Ordinal))
                throw new ArgumentException("Origin definition kind mismatch", nameof(value));
            return value;
        }
    }
}
