using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Foundation.Random;

namespace Game.Infrastructure.SettlementContent
{
    public sealed class SettlementRegistry
    {
        public string ContentHash { get; }
        public IReadOnlyDictionary<string, string> Localization { get; }
        public IReadOnlyList<IReadOnlyDictionary<string, object?>> Definitions { get; }
        internal SettlementRegistry(string hash, IEnumerable<Dictionary<string, object?>> definitions, Dictionary<string, object?> localization)
        {
            ContentHash = hash;
            Localization = new ReadOnlyDictionary<string, string>(localization.ToDictionary(x => x.Key, x => (string)x.Value!, StringComparer.Ordinal));
            Definitions = Array.AsReadOnly(definitions.Select(x => (IReadOnlyDictionary<string, object?>)Freeze(x)!).ToArray());
        }
        private static object? Freeze(object? value)
        {
            if (value is Dictionary<string, object?> map)
                return new ReadOnlyDictionary<string, object?>(map.ToDictionary(x => x.Key, x => Freeze(x.Value), StringComparer.Ordinal));
            if (value is object[] array) return Array.AsReadOnly(array.Select(Freeze).ToArray());
            return value;
        }
        public IReadOnlyDictionary<string, object?> Get(string id) => Definitions.Single(x => (string)x["definition_id"]! == id);
        public IReadOnlyList<IReadOnlyDictionary<string, object?>> ByKind(string kind) => Array.AsReadOnly(Definitions.Where(x => (string)x["kind"]! == kind).ToArray());
        public IReadOnlyList<IReadOnlyDictionary<string, object?>> OutgoingRoutes(string communityId, string season)
        {
            if (season != "dry" && season != "wet") throw new ArgumentException("SETTLEMENT_SEASON");
            if ((string)Get(communityId)["kind"]! != "community") throw new ArgumentException("SETTLEMENT_COMMUNITY");
            return Array.AsReadOnly(ByKind("route").Where(x => (string)x["from_community_id"]! == communityId &&
                ((IReadOnlyList<object?>)x["open_seasons"]!).Contains(season)).ToArray());
        }
    }
    public static class SettlementCompiler
    {
        public const string Profile = "settlement-content-v1";
        private static readonly Regex Token = new Regex(@"\A[a-z][a-z0-9_-]*\z", RegexOptions.CultureInvariant);
        private static readonly string[] Common = "definition_id version schema_version kind source classification owner_domain display_key".Split(' ');
        private static void Require(bool condition, string code) { if (!condition) throw new InvalidDataException("SETTLEMENT_" + code); }
        private static Dictionary<string, object?> Map(object? x) { Require(x is Dictionary<string, object?>, "OBJECT"); return (Dictionary<string, object?>)x!; }
        private static void Fields(Dictionary<string, object?> x, params string[] names) => Require(x.Count == names.Length && names.All(x.ContainsKey), "FIELDS");
        private static string Text(object? x) { Require(x is string s && !string.IsNullOrWhiteSpace(s) && s.Length <= 2048, "TEXT"); return (string)x!; }
        private static string S(Dictionary<string, object?> x, string key) => Text(x[key]);
        private static long Number(object? x, long min, long max) { Require(x is long || x is int, "INTEGER"); long n = Convert.ToInt64(x); Require(n >= min && n <= max, "RANGE"); return n; }
        private static bool Bool(object? x) { Require(x is bool, "BOOLEAN"); return (bool)x!; }
        private static object[] Items(object? x, int min = 0, int max = 256) { Require(x is object[], "ARRAY"); var a = (object[])x!; Require(a.Length >= min && a.Length <= max, "ARRAY_SIZE"); return a; }
        private static string Local(object? x) { var s = Text(x); Require(Token.IsMatch(s), "LOCAL_ID"); return s; }
        private static void Unique(IEnumerable<string> values) { var a = values.ToArray(); Require(a.Distinct(StringComparer.Ordinal).Count() == a.Length, "DUPLICATE"); }
        // Bound direct primitive callers too, including cyclic object graphs.
        private static object? Copy(object? x, int depth, ref int nodes)
        {
            Require(depth <= 20 && ++nodes <= 100000, "DEPTH_SIZE");
            if (x is Dictionary<string, object?> map)
            {
                Require(map.Count <= 1024, "OBJECT_SIZE");
                var copy = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in map) { Text(pair.Key); copy.Add(pair.Key, Copy(pair.Value, depth + 1, ref nodes)); }
                return copy;
            }
            if (x is object[] array) { Require(array.Length <= 512, "ARRAY_SIZE"); var result = new object?[array.Length]; for (int i=0;i<array.Length;i++) result[i]=Copy(array[i],depth+1,ref nodes); return result; }
            if (x is string) return Text(x);
            Require(x == null || x is bool || x is int || x is long, "PRIMITIVE"); return x is int integer ? (object)(long)integer : x;
        }
        public static SettlementRegistry Compile(Dictionary<string, object?> input)
        {
            int nodes = 0; var root = Map(Copy(input, 0, ref nodes));
            Fields(root, "pack_id", "version", "schema_version", "profile", "definitions", "localization");
            Require(S(root,"pack_id") == "slice.settlement" && S(root,"profile") == Profile, "PROFILE");
            Number(root["version"],1,1); Number(root["schema_version"],1,1);
            var localization = Map(root["localization"]); foreach(var value in localization.Values) Text(value);
            var defs = Items(root["definitions"],1,512).Select(Map).ToArray();
            var byId = new Dictionary<string,Dictionary<string,object?>>(StringComparer.Ordinal);
            foreach(var d in defs)
            {
                Require(d.ContainsKey("kind"),"FIELDS"); string kind=S(d,"kind");
                string extra, owner;
                switch(kind) {
                    case "community": extra="region external"; owner="Settlements"; break;
                    case "household": extra="community_id member_ids lots rights obligations"; owner="Households"; break;
                    case "actor": extra="household_id age alive attributes history skills known_facts needs routine viability"; owner="Characters"; break;
                    case "fact": extra="claim provenance"; owner="Knowledge"; break;
                    case "relationship": extra="from_actor_id to_actor_id trust description"; owner="Social"; break;
                    case "route": extra="from_community_id to_community_id mode duration_ticks capacity open_seasons"; owner="Travel"; break;
                    default: throw new InvalidDataException("SETTLEMENT_KIND");
                }
                Fields(d,Common.Concat(extra.Split(' ')).ToArray()); string id=S(d,"definition_id");
                Require(Regex.IsMatch(id,"\\A"+kind+"\\.[a-z][a-z0-9_-]*@1\\z",RegexOptions.CultureInvariant),"ID");
                Require(!byId.ContainsKey(id),"DUPLICATE_ID"); byId.Add(id,d);
                Number(d["version"],1,1); Number(d["schema_version"],1,1);
                S(d,"source"); Require(S(d,"classification")=="game-canon" && S(d,"owner_domain")==owner,"METADATA");
                Require(localization.ContainsKey(S(d,"display_key")),"LOCALIZATION");
            }
            Dictionary<string,object?> Ref(object? value,string kind) { string id=Text(value); Require(byId.TryGetValue(id,out var found) && S(found!,"kind")==kind,"REFERENCE"); return found!; }
            var communities=defs.Where(x=>S(x,"kind")=="community").ToArray();
            var localIds=new[]{"community.ben-vac@1","community.con-may@1","community.van-nuoc@1"};
            Require(communities.Length==4,"COMMUNITY_COUNT");
            foreach(var c in communities) { string id=S(c,"definition_id"); bool external=Bool(c["external"]); Require(localIds.Contains(id) ? (!external && S(c,"region")=="tam-giang") : (id=="community.ben-hac@1" && external && S(c,"region")=="phong-chau"),"CANON"); }
            var households=defs.Where(x=>S(x,"kind")=="household").ToArray(); var actors=defs.Where(x=>S(x,"kind")=="actor").ToArray(); var routes=defs.Where(x=>S(x,"kind")=="route").ToArray();
            Require(households.Length>=6 && households.Length<=8 && actors.Length>=20 && actors.Length<=30 && routes.Length>=6 && routes.Length<=10,"ROSTER_COUNT");
            var lotIds=new HashSet<string>(StringComparer.Ordinal);
            foreach(var h in households)
            {
                Require(!Bool(Ref(h["community_id"],"community")["external"]),"HOUSEHOLD_LOCAL");
                var members=Items(h["member_ids"],1).Select(Text).ToArray(); Unique(members);
                foreach(var member in members) Require(S(Ref(member,"actor"),"household_id")==S(h,"definition_id"),"MEMBERSHIP");
                var ownLots=new HashSet<string>(StringComparer.Ordinal);
                foreach(var v in Items(h["lots"])) { var lot=Map(v); Fields(lot,"lot_id","commodity","quantity","unit"); var id=Local(lot["lot_id"]); Require(lotIds.Add(id),"LOT_DUPLICATE"); ownLots.Add(id); Require(new[]{"grain","timber"}.Contains(S(lot,"commodity")) && S(lot,"unit")=="count","LOT_KIND"); Number(lot["quantity"],0,1000000); }
                var rights = new HashSet<string>(StringComparer.Ordinal); var coveredLots = new HashSet<string>(StringComparer.Ordinal);
                foreach(var v in Items(h["rights"])) { var r=Map(v); Fields(r,"lot_id","holder_id","permission"); Require(ownLots.Contains(Local(r["lot_id"])) && members.Contains(S(r,"holder_id")) && new[]{"use","manage"}.Contains(S(r,"permission")),"RIGHTS"); Require(rights.Add(S(r,"lot_id")+"|"+S(r,"holder_id")+"|"+S(r,"permission")),"RIGHTS_DUPLICATE"); coveredLots.Add(S(r,"lot_id")); }
                Require(ownLots.SetEquals(coveredLots),"RIGHTS_COVERAGE");
                foreach(var v in Items(h["obligations"],1)) { var o=Map(v); Fields(o,"to_household_id","description"); Ref(o["to_household_id"],"household"); Require(S(o,"to_household_id")!=S(h,"definition_id"),"OBLIGATION_SELF"); S(o,"description"); }
            }
            foreach(var a in actors)
            {
                var h=Ref(a["household_id"],"household"); Require(Items(h["member_ids"]).Count(x=>Text(x)==S(a,"definition_id"))==1,"MEMBERSHIP");
                long age=Number(a["age"],0,100); Bool(a["alive"]);
                var attrs=Map(a["attributes"]); Fields(attrs,"the","xao","giac","tri","tam","khi"); foreach(var n in attrs.Values) Number(n,1,10);
                var histories=new HashSet<string>(StringComparer.Ordinal); long previous=-1;
                foreach(var v in Items(a["history"],1)) { var t=Map(v); Fields(t,"history_id","at_age","description"); Require(histories.Add(Local(t["history_id"])),"HISTORY_DUPLICATE"); long at=Number(t["at_age"],0,age); Require(at>=previous,"HISTORY_ORDER"); previous=at; S(t,"description"); }
                var skills=new HashSet<string>(StringComparer.Ordinal);
                foreach(var v in Items(a["skills"],1)) { var s=Map(v); Fields(s,"skill_id","value","history_id"); Require(skills.Add(Local(s["skill_id"])) && histories.Contains(Local(s["history_id"])),"SKILL_PROVENANCE"); Number(s["value"],0,100); }
                var facts=new HashSet<string>(StringComparer.Ordinal);
                foreach(var v in Items(a["known_facts"],1)) { var f=Map(v); Fields(f,"fact_id","history_id"); Ref(f["fact_id"],"fact"); Require(facts.Add(S(f,"fact_id")) && histories.Contains(Local(f["history_id"])),"FACT_PROVENANCE"); }
                foreach(var n in Items(a["needs"],1)) Text(n);
                var routine=Map(a["routine"]); Fields(routine,"description","community_id","skill_id","duration_ticks","executable"); S(routine,"description"); Require(!Bool(Ref(routine["community_id"],"community")["external"]) && skills.Contains(Local(routine["skill_id"])) && !Bool(routine["executable"]),"ROUTINE"); Number(routine["duration_ticks"],1,1440);
                var viability=Map(a["viability"]); Fields(viability,"description","caregiver_id"); S(viability,"description");
                if(viability["caregiver_id"]!=null) { var care=Ref(viability["caregiver_id"],"actor"); Require(S(care,"definition_id")!=S(a,"definition_id") && Bool(care["alive"]) && Number(care["age"],0,100)>=16 && S(care,"household_id")==S(a,"household_id"),"CAREGIVER"); }
                Require(!Bool(a["alive"]) || age>=16 || viability["caregiver_id"]!=null,"CHILD_CAREGIVER");
            }
            var pairs=new HashSet<string>(StringComparer.Ordinal); var outgoing=new HashSet<string>(StringComparer.Ordinal);
            foreach(var d in defs.Where(x=>S(x,"kind")=="relationship")) { Ref(d["from_actor_id"],"actor"); Ref(d["to_actor_id"],"actor"); string from=S(d,"from_actor_id"),to=S(d,"to_actor_id"); Require(from!=to && pairs.Add(from+"|"+to),"RELATIONSHIP_PAIR"); outgoing.Add(from); Number(d["trust"],-100,100); S(d,"description"); }
            Require(actors.All(a=>outgoing.Contains(S(a,"definition_id"))),"RELATIONSHIP_COVERAGE");
            pairs.Clear();
            foreach(var r in routes) { Ref(r["from_community_id"],"community"); Ref(r["to_community_id"],"community"); string from=S(r,"from_community_id"),to=S(r,"to_community_id"),mode=S(r,"mode"); Require(from!=to && new[]{"water","land"}.Contains(mode) && pairs.Add(from+"|"+to+"|"+mode),"ROUTE_PAIR"); Number(r["duration_ticks"],1,1000000); Number(r["capacity"],1,1000000); var seasons=Items(r["open_seasons"]).Select(Text).ToArray(); Unique(seasons); Require(seasons.All(x=>x=="dry"||x=="wet"),"SEASON"); }
            foreach(var id in localIds) Require(households.Any(h=>S(h,"community_id")==id) && routes.Any(r=>S(r,"from_community_id")==id) && routes.Any(r=>S(r,"to_community_id")==id),"LOCAL_COVERAGE");
            foreach(var f in defs.Where(x=>S(x,"kind")=="fact")) { S(f,"claim"); S(f,"provenance"); }
            var ordered=defs.OrderBy(x=>S(x,"definition_id"),StringComparer.Ordinal).ToArray(); root["definitions"]=ordered.Cast<object>().ToArray();
            root["localization"] = localization.OrderBy(x=>x.Key,StringComparer.Ordinal).Select(x=>(object)new Dictionary<string,object?> { ["key"]=x.Key, ["text"]=x.Value }).ToArray();
            return new SettlementRegistry(CanonicalCodec.Hash(root),ordered,localization);
        }
    }
}
