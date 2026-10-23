using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.SettlementContent;
using NUnit.Framework;
namespace Game.Tests
{
    public class SettlementContentTests
    {
        private static object? Decode(JsonElement x) => x.ValueKind switch {
            JsonValueKind.Object => x.EnumerateObject().ToDictionary(p=>p.Name,p=>Decode(p.Value),StringComparer.Ordinal),
            JsonValueKind.Array => x.EnumerateArray().Select(Decode).ToArray(),
            JsonValueKind.String => x.GetString(), JsonValueKind.Number => x.GetInt64(),
            JsonValueKind.True => true, JsonValueKind.False => false, JsonValueKind.Null => null,
            _ => throw new Exception("fixture") };
        private static Dictionary<string,object?> Pack() { using var d=JsonDocument.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory,"settlement-pack.json"))); return (Dictionary<string,object?>)Decode(d.RootElement)!; }
        private static Dictionary<string,object?> Map(object? x)=>(Dictionary<string,object?>)x!;
        private static object[] Array(object? x)=>(object[])x!;
        private static Dictionary<string,object?> First(Dictionary<string,object?> p,string kind)=>Array(p["definitions"]).Select(Map).First(d=>(string)d["kind"]! == kind);
        [Test] public void VS_02A_RealRosterAndCanon()
        {
            var r=SettlementCompiler.Compile(Pack()); Assert.That(r.ByKind("actor").Count,Is.EqualTo(24));Assert.That(r.ByKind("household").Count,Is.EqualTo(6));Assert.That(r.ByKind("route").Count,Is.EqualTo(8));
            Assert.That(r.Get("community.ben-hac@1")["region"],Is.EqualTo("phong-chau"));Assert.That(r.Get("community.ben-hac@1")["external"],Is.True);
            Assert.That(r.Get("actor.vac-moc@1")["household_id"],Is.EqualTo("household.vac-tong@1"));
        }
        [Test] public void VS_02A_InputAndOutputCannotMutateRegistry()
        {
            var p=Pack();var actor=First(p,"actor");var r=SettlementCompiler.Compile(p);var id=(string)actor["definition_id"]!;var original=r.Get(id)["age"];var hash=r.ContentHash;
            actor["age"]=99L;Map(Array(actor["history"])[0])["description"]="tampered";Map(p["localization"])[(string)actor["display_key"]!]="tampered";
            Assert.That(r.Get(id)["age"],Is.EqualTo(original));Assert.That(r.ContentHash,Is.EqualTo(hash));
            Assert.Throws<NotSupportedException>(()=>((IDictionary<string,object?>)r.Get(id))["age"]=99L);
            var history=(IReadOnlyList<object?>)r.Get(id)["history"]!;
            Assert.Throws<NotSupportedException>(()=>((IDictionary<string,object?>)history[0]!)["description"]="bad");
            Assert.Throws<NotSupportedException>(()=>((IList<object?>)history)[0]=null);
            Assert.Throws<NotSupportedException>(()=>((IDictionary<string,string>)r.Localization)[(string)actor["display_key"]!]="bad");
        }
        [Test] public void VS_02A_DefinitionAndMapOrderDoNotChangeHash()
        {
            var p=Pack();var r=SettlementCompiler.Compile(p);p["definitions"]=Array(p["definitions"]).Reverse().ToArray();p["localization"]=Map(p["localization"]).Reverse().ToDictionary(x=>x.Key,x=>x.Value);
            Assert.That(SettlementCompiler.Compile(p).ContentHash,Is.EqualTo(r.ContentHash));
        }
        [Test] public void VS_02A_DisplayRenamePreservesReferencesButChangesContentHash()
        {
            var p=Pack();var before=SettlementCompiler.Compile(p);var actor=First(p,"actor");var id=(string)actor["definition_id"]!;
            Map(p["localization"])[(string)actor["display_key"]!]="Tên hiển thị sửa";var after=SettlementCompiler.Compile(p);
            Assert.That(after.Get(id)["household_id"],Is.EqualTo(before.Get(id)["household_id"]));Assert.That(after.ContentHash,Is.Not.EqualTo(before.ContentHash));
        }
        [Test] public void VS_02A_RoutesAreDirectionalAndSeasonFiltered()
        {
            var p=Pack();var route=First(p,"route");route["open_seasons"]=System.Array.Empty<object>();var r=SettlementCompiler.Compile(p);
            var source=(string)route["from_community_id"]!;var id=(string)route["definition_id"]!;
            Assert.That(r.OutgoingRoutes(source,"wet").Any(x=>(string)x["definition_id"]! == id),Is.False);
            Assert.That(r.ByKind("route").Count,Is.EqualTo(8));
            foreach(var edge in r.OutgoingRoutes(source,"dry")) Assert.That(edge["from_community_id"],Is.EqualTo(source));
            Assert.Throws<ArgumentException>(()=>r.OutgoingRoutes(source,"monsoon-unknown"));
        }
        [Test] public void VS_02A_KnowledgeAndRoutinesStayAuthoredMetadata()
        {
            var r=SettlementCompiler.Compile(Pack());
            var known=r.ByKind("actor").Select(a=>(IReadOnlyList<object?>)a["known_facts"]!).ToArray();
            Assert.That(known.All(x=>x.Count==1),Is.True);
            Assert.That(known.Select(x=>(string)((IReadOnlyDictionary<string,object?>)x[0]!)["fact_id"]!).Distinct().Count(),Is.EqualTo(24));
            Assert.That(r.ByKind("actor").All(a=>(bool)((IReadOnlyDictionary<string,object?>)a["routine"]!)["executable"]! == false),Is.True);
        }
        [Test] public void VS_02A_DeadChildMetadataDoesNotNeedLivingCaregiver()
        {
            var p=Pack();var child=Array(p["definitions"]).Select(Map).First(x=>(string)x["kind"]! == "actor" && (long)x["age"]!<16);
            child["alive"]=false;Map(child["viability"])["caregiver_id"]=null;Assert.DoesNotThrow(()=>SettlementCompiler.Compile(p));
        }
        [TestCase("profile")][TestCase("extra")][TestCase("owner")][TestCase("canon")][TestCase("member")][TestCase("skill")][TestCase("known")][TestCase("attributes")][TestCase("float")][TestCase("routine")][TestCase("caregiver")][TestCase("right")][TestCase("rights-empty")][TestCase("rights-duplicate")][TestCase("obligation")][TestCase("route-time")][TestCase("route-self")][TestCase("season")][TestCase("duplicate")][TestCase("history-order")][TestCase("id-prefix")][TestCase("localization")][TestCase("cycles")][TestCase("unknown-kind")][TestCase("newline-local")][TestCase("newline-id")]
        public void VS_02A_InvalidSemanticDataFailsBeforeRegistry(string mutation)
        {
            var p=Pack();var a=First(p,"actor");var h=First(p,"household");var edge=First(p,"route");
            switch(mutation) {
                case "profile":p["profile"]="unknown";break;
                case "extra":p["script"]="execute";break;
                case "owner":a["owner_domain"]="Assets";break;
                case "canon":First(p,"community")["region"]="phong-chau";break;
                case "member":h["member_ids"]=new object[]{"actor.missing@1"};break;
                case "skill":Map(Array(a["skills"])[0])["history_id"]="invented";break;
                case "known":Map(Array(a["known_facts"])[0])["fact_id"]="actor.vac-moc@1";break;
                case "attributes":Map(a["attributes"])["the"]=11L;break;
                case "float":a["age"]=20.5;break;
                case "routine":Map(a["routine"])["executable"]=true;break;
                case "caregiver":var c=Array(p["definitions"]).Select(Map).First(x=>(string)x["kind"]! == "actor" && (long)x["age"]!<16);Map(c["viability"])["caregiver_id"]=null;break;
                case "right":Map(Array(h["rights"])[0])["holder_id"]="actor.missing@1";break;
                case "rights-empty":h["rights"]=System.Array.Empty<object>();break;
                case "rights-duplicate":h["rights"]=Array(h["rights"]).Concat(new[]{Array(h["rights"])[0]}).ToArray();break;
                case "obligation":Map(Array(h["obligations"])[0])["to_household_id"]=h["definition_id"];break;
                case "route-time":edge["duration_ticks"]=0L;break;
                case "route-self":edge["to_community_id"]=edge["from_community_id"];break;
                case "season":edge["open_seasons"]=new object[]{"wet","wet"};break;
                case "duplicate":p["definitions"]=Array(p["definitions"]).Concat(new object[]{a}).ToArray();break;
                case "history-order":a["history"]=Array(a["history"]).Reverse().ToArray();break;
                case "id-prefix":a["definition_id"]="household.wrong@1";break;
                case "localization":Map(p["localization"]).Remove((string)a["display_key"]!);break;
                case "cycles":p["cycle"]=p;break;
                case "newline-local":Map(Array(a["history"])[0])["history_id"]="learning\n";Map(Array(a["skills"])[0])["history_id"]="learning\n";break;
                case "newline-id":First(p,"relationship")["definition_id"]="relationship.newline@1\n";break;
                case "unknown-kind":a["kind"]="event";break;
            }
            Assert.Throws<InvalidDataException>(()=>SettlementCompiler.Compile(p));
        }
    }
}
