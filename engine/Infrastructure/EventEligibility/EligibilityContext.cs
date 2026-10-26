using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Game.Infrastructure.EventEligibility
{
    /// <summary>
    /// Trusted adapter input for one actor. This is not a WorldState or a player projection.
    /// KnownFactIds is the actor's complete supplied knowledge set; absence means not known.
    /// Missing scalar/resource observations mean unknown. LastOccurrenceTick must belong to the
    /// evaluated template and actor; only a future authoritative adapter can establish that provenance.
    /// </summary>
    public sealed class EligibilityContext
    {
        public string ActorId { get; }
        public bool Alive { get; }
        public long NowTick { get; }
        public long? LastOccurrenceTick { get; }
        public IReadOnlyDictionary<string, object?> Physical { get; }
        public IReadOnlyDictionary<string, object?> Perceived { get; }
        public IReadOnlyList<string> KnownFactIds { get; }
        public IReadOnlyDictionary<string, long> Resources { get; }

        public EligibilityContext(string actorId, bool alive, long nowTick, long? lastOccurrenceTick,
            IReadOnlyDictionary<string, object?> physical, IReadOnlyDictionary<string, object?> perceived,
            IEnumerable<string> knownFactIds, IReadOnlyDictionary<string, long> resources)
        {
            EligibilityChecks.Require(actorId != null && Regex.IsMatch(actorId, @"\Aactor\.[a-z][a-z0-9_-]*@1\z"), "ACTOR_ID");
            EligibilityChecks.Require(nowTick >= 0 && (!lastOccurrenceTick.HasValue || (lastOccurrenceTick.Value >= 0 && lastOccurrenceTick.Value <= nowTick)), "CLOCK");
            ActorId = actorId!; Alive = alive; NowTick = nowTick; LastOccurrenceTick = lastOccurrenceTick;
            Physical = CopyFields(physical, false); Perceived = CopyFields(perceived, true);
            EligibilityChecks.Require(knownFactIds != null && resources != null, "CONTEXT_NULL");
            var facts = knownFactIds!.Take(513).ToArray();
            EligibilityChecks.Require(facts.Length <= 512 && facts.All(x => x != null && Regex.IsMatch(x, @"\Afact\.[a-z][a-z0-9_-]*@1\z")) && facts.Distinct(StringComparer.Ordinal).Count() == facts.Length, "FACTS");
            KnownFactIds = Array.AsReadOnly(facts.OrderBy(x => x, StringComparer.Ordinal).ToArray());
            EligibilityChecks.Require(resources!.Count <= 2, "RESOURCES");
            var copy = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (var pair in resources) { EligibilityChecks.Require((pair.Key == "grain" || pair.Key == "timber") && pair.Value >= 0 && pair.Value <= 1000000, "RESOURCE"); copy.Add(pair.Key, pair.Value); }
            Resources = new ReadOnlyDictionary<string, long>(copy);
        }

        private static IReadOnlyDictionary<string, object?> CopyFields(IReadOnlyDictionary<string, object?> fields, bool perceived)
        {
            EligibilityChecks.Require(fields != null && fields.Count <= 16, "CONTEXT_FIELDS");
            var copy = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in fields!) { var spec = EligibilityChecks.Field(pair.Key, perceived); copy.Add(pair.Key, spec.Validate(pair.Value)); }
            return new ReadOnlyDictionary<string, object?>(copy);
        }
    }

    internal sealed class EligibilityField
    {
        internal bool Boolean { get; }
        private readonly long min, max;
        internal EligibilityField(bool boolean, long min = 0, long max = 1000) { Boolean = boolean; this.min = min; this.max = max; }
        internal object Validate(object? value)
        {
            if (Boolean) { EligibilityChecks.Require(value is bool, "FIELD_TYPE"); return value!; }
            EligibilityChecks.Require(value is long || value is int, "FIELD_TYPE");
            long number = Convert.ToInt64(value);
            EligibilityChecks.Require(number >= min && number <= max, "FIELD_RANGE"); return number;
        }
    }
    internal static class EligibilityChecks
    {
        internal static void Require(bool condition, string code) { if (!condition) throw new InvalidDataException("ELIGIBILITY_" + code); }
        internal static EligibilityField Field(string name, bool perceived)
        {
            if (perceived)
            {
                if (name == "repair_need" || name == "transport_request" || name == "landing_dispute" || name == "treatment_request" || name == "food_shortage") return new EligibilityField(true);
            }
            else
            {
                if (name == "boat_condition" || name == "rain_intensity" || name == "injury_severity") return new EligibilityField(false);
                if (name == "deadline_remaining_ticks") return new EligibilityField(false, -1000000, 1000000);
                if (name == "landing_access_disputed" || name == "rumor_delivered") return new EligibilityField(true);
            }
            throw new InvalidDataException("ELIGIBILITY_FIELD_SCOPE");
        }
    }
}
