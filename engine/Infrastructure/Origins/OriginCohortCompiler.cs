using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Application.Origins;
using Game.Domain.Origins;
using Game.Foundation.Random;
using Game.Infrastructure.SettlementContent;

namespace Game.Infrastructure.Origins
{
    [Serializable] public sealed class OriginCohortDocument
    { public string profile = ""; public string source_hash = ""; public OriginCohortRecord[] records = Array.Empty<OriginCohortRecord>(); }
    [Serializable] public sealed class OriginCohortRecord
    {
        public string actor_id = "", household_id = "", community_id = "", display_name = "", household_label = "", home_label = "", source = "";
        public bool alive; public int age;
        public OriginHistoryRecord[] history = Array.Empty<OriginHistoryRecord>();
        public OriginSkillRecord[] skills = Array.Empty<OriginSkillRecord>();
        public OriginClaimRecord[] claims = Array.Empty<OriginClaimRecord>();
        public string[] needs = Array.Empty<string>();
    }
    [Serializable] public sealed class OriginHistoryRecord
    { public string id = "", description = ""; public int at_age; }
    [Serializable] public sealed class OriginSkillRecord
    { public string id = "", history_id = ""; public int value; }
    [Serializable] public sealed class OriginClaimRecord
    { public string id = "", text = "", source = "", history_id = ""; }

    public sealed class OriginSelfRecord
    {
        public OriginCandidate Candidate { get; }
        private readonly string name, household, home;
        private readonly OriginHistoryView[] history;
        private readonly OriginSkillView[] skills;
        private readonly IReadOnlyDictionary<string, int> skillValues;
        private readonly OriginClaimView[] claims;
        private readonly string[] needs;
        internal OriginSelfRecord(OriginCohortRecord r)
        {
            Candidate = new OriginCandidate(r.actor_id, r.household_id, r.community_id, r.alive, r.age);
            name = r.display_name; household = r.household_label; home = r.home_label;
            skillValues = new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(r.skills.ToDictionary(s => s.id, s => s.value, StringComparer.Ordinal));
            history = r.history.OrderBy(h => h.at_age).ThenBy(h => h.id, StringComparer.Ordinal)
                .Select(h => new OriginHistoryView(h.at_age, h.description, "Ký ức bản thân ở tuổi " + h.at_age)).ToArray();
            var episodes = r.history.ToDictionary(h => h.id, StringComparer.Ordinal);
            skills = r.skills.OrderBy(s => s.id, StringComparer.Ordinal)
                .Select(s => new OriginSkillView(SkillLabel(s.id), s.value, episodes[s.history_id].description)).ToArray();
            claims = r.claims.OrderBy(c => c.id, StringComparer.Ordinal)
                .Select(c => new OriginClaimView(c.text, "Nhận định của bản thân, gắn với ký ức ở tuổi " + episodes[c.history_id].at_age, episodes[c.history_id].at_age)).ToArray();
            needs = r.needs.ToArray();
        }
        private static string SkillLabel(string id) => id == "transport" ? "Vận tải đường sông" : id == "trading" ? "Trao đổi hàng"
            : id == "weaving" ? "Dệt" : id == "medicine" ? "Chăm sóc và chữa trị" : id == "farming" ? "Trồng trọt"
            : id == "fishing" ? "Đánh bắt" : id == "boat-repair" ? "Sửa thuyền" : throw new ArgumentException("ORIGIN_SKILL");
        public int GetSkillValue(string skillId) => skillId != null && skillValues.TryGetValue(skillId, out var value) ? value : 0;
        public OriginRevealView Reveal(long tick) => new OriginRevealView(name, Candidate.Age, household, home, tick, history, skills, claims, needs);
    }
    public sealed class OriginCohortRegistry
    {
        public string ContentHash { get; }
        public string SourceContentHash { get; }
        public IReadOnlyList<OriginCandidate> Candidates { get; }
        private readonly Dictionary<string, OriginSelfRecord> records;
        internal OriginCohortRegistry(string hash, string source, IEnumerable<OriginCohortRecord> entries)
        {
            ContentHash = hash; SourceContentHash = source;
            records = entries.ToDictionary(r => r.actor_id, r => new OriginSelfRecord(r), StringComparer.Ordinal);
            Candidates = Array.AsReadOnly(records.Values.Select(r => r.Candidate).OrderBy(r => r.ActorDefinitionId, StringComparer.Ordinal).ToArray());
        }
        public OriginSelfRecord Get(string actorDefinitionId) => records.TryGetValue(actorDefinitionId, out var record)
            ? record : throw new ArgumentException("ORIGIN_CANDIDATE");
    }

    public static class OriginCohortCompiler
    {
        public const string Profile = "origin-cohort-v1";
        private static readonly string[] Expected = { "actor.ben-ha@1", "actor.ben-kien@1", "actor.ben-sen@1", "actor.vac-lam@1",
            "actor.vac-mai@1", "actor.vac-moc@1", "actor.vac-nhu@1", "actor.vac-tong@1" };
        private static void Require(bool value, string code) { if (!value) throw new ArgumentException("ORIGIN_" + code); }
        private static string Text(string? value) { Require(!string.IsNullOrWhiteSpace(value) && value!.Length <= 2048, "TEXT"); return value!; }
        private static string Token(string value) { Text(value); Require(Regex.IsMatch(value, @"\A[a-z][a-z0-9_-]*\z"), "TOKEN"); return value; }
        private static bool Hash(string value) => value != null && Regex.IsMatch(value, @"\A[0-9a-f]{64}\z");
        private static object Obj(params object[] pairs)
        { var d = new Dictionary<string, object?>(); for (int i = 0; i < pairs.Length; i += 2) d.Add((string)pairs[i], pairs[i + 1]); return d; }
        private static object Canonical(OriginCohortRecord r) => Obj("actor_id", r.actor_id, "household_id", r.household_id,
            "community_id", r.community_id, "display_name", r.display_name, "household_label", r.household_label, "home_label", r.home_label,
            "source", r.source, "alive", r.alive, "age", (long)r.age,
            "history", r.history.OrderBy(h => h.id, StringComparer.Ordinal).Select(h => Obj("id", h.id, "at_age", (long)h.at_age, "description", h.description)).ToArray(),
            "skills", r.skills.OrderBy(s => s.id, StringComparer.Ordinal).Select(s => Obj("id", s.id, "value", (long)s.value, "history_id", s.history_id)).ToArray(),
            "claims", r.claims.OrderBy(c => c.id, StringComparer.Ordinal).Select(c => Obj("id", c.id, "text", c.text, "source", c.source, "history_id", c.history_id)).ToArray(),
            "needs", r.needs);
        public static OriginCohortRegistry Compile(OriginCohortDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            Require(document.profile == Profile && Hash(document.source_hash), "PROFILE");
            Require(document.records != null && document.records.Length == 8 && document.records.All(r => r != null), "COUNT");
            var records = document.records!.OrderBy(r => r.actor_id, StringComparer.Ordinal).ToArray();
            Require(records.Select(r => r.actor_id).SequenceEqual(Expected), "CANDIDATES");
            foreach (var r in records)
            {
                Require(r.community_id == "community.ben-vac@1" && r.household_id == (r.actor_id.StartsWith("actor.vac-", StringComparison.Ordinal) ? "household.vac-tong@1" : "household.ben-sen@1"), "REFERENCES");
                Require(r.alive && r.age >= 18 && r.age <= 100, "ADMISSION");
                Text(r.display_name); Text(r.household_label); Text(r.home_label); Text(r.source);
                Require(r.history != null && r.history.Length >= 1 && r.history.Length <= 16 && r.history.All(h => h != null), "HISTORY");
                var episodes = new HashSet<string>(StringComparer.Ordinal);
                foreach (var h in r.history!) { Require(episodes.Add(Token(h.id)) && h.at_age >= 0 && h.at_age <= r.age, "HISTORY"); Text(h.description); }
                Require(r.skills != null && r.skills.Length >= 1 && r.skills.Length <= 16 && r.skills.All(s => s != null), "SKILLS");
                var skillIds = new HashSet<string>(StringComparer.Ordinal);
                foreach (var s in r.skills!) { Require(skillIds.Add(Token(s.id)) && s.value >= 0 && s.value <= 100 && episodes.Contains(s.history_id), "SKILL"); }
                Require(r.claims != null && r.claims.Length >= 1 && r.claims.Length <= 16 && r.claims.All(c => c != null), "CLAIMS");
                var facts = new HashSet<string>(StringComparer.Ordinal);
                foreach (var c in r.claims!)
                {
                    Require(c.id == r.actor_id.Replace("actor.", "fact.").Replace("@1", "-claim@1") && facts.Add(c.id) && episodes.Contains(c.history_id), "CLAIM_REFERENCE");
                    Text(c.text); Text(c.source);
                }
                Require(r.needs != null && r.needs.Length >= 1 && r.needs.Length <= 8, "NEEDS");
                foreach (var n in r.needs!) Text(n);
            }
            var hash = CanonicalCodec.Hash(Obj("profile", Profile, "source_hash", document.source_hash, "records", records.Select(Canonical).ToArray()));
            // The registry constructs immutable values now; it never holds caller-owned mutable DTO arrays.
            return new OriginCohortRegistry(hash, document.source_hash, records);
        }
        public static OriginCohortDocument FromSettlement(SettlementRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            string S(IReadOnlyDictionary<string, object?> d, string key) => (string)d[key]!;
            IEnumerable<IReadOnlyDictionary<string, object?>> Items(IReadOnlyDictionary<string, object?> d, string key)
                => ((IReadOnlyList<object?>)d[key]!).Cast<IReadOnlyDictionary<string, object?>>();
            string Label(IReadOnlyDictionary<string, object?> d) => registry.Localization[S(d, "display_key")];
            var records = registry.ByKind("actor").Where(a => (bool)a["alive"]! && (long)a["age"]! >= 18 &&
                S(registry.Get(S(a, "household_id")), "community_id") == "community.ben-vac@1").Select(a => {
                var household = registry.Get(S(a, "household_id")); var community = registry.Get(S(household, "community_id"));
                return new OriginCohortRecord {
                    actor_id = S(a, "definition_id"), household_id = S(household, "definition_id"), community_id = S(community, "definition_id"),
                    display_name = Label(a), household_label = Label(household), home_label = Label(community), source = S(a, "source"),
                    alive = (bool)a["alive"]!, age = checked((int)(long)a["age"]!),
                    history = Items(a, "history").Select(h => new OriginHistoryRecord { id = S(h, "history_id"), at_age = checked((int)(long)h["at_age"]!), description = S(h, "description") }).ToArray(),
                    skills = Items(a, "skills").Select(s => new OriginSkillRecord { id = S(s, "skill_id"), value = checked((int)(long)s["value"]!), history_id = S(s, "history_id") }).ToArray(),
                    claims = Items(a, "known_facts").Select(c => { var fact = registry.Get(S(c, "fact_id")); return new OriginClaimRecord {
                        id = S(fact, "definition_id"), text = S(fact, "claim"), source = S(fact, "provenance"), history_id = S(c, "history_id") }; }).ToArray(),
                    needs = ((IReadOnlyList<object?>)a["needs"]!).Cast<string>().ToArray()
                };
            }).OrderBy(r => r.actor_id, StringComparer.Ordinal).ToArray();
            var result = new OriginCohortDocument { profile = Profile, source_hash = registry.ContentHash, records = records };
            _ = Compile(result); return result;
        }
    }
}
