using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Game.Application.Origins
{
    public sealed class OriginResult
    {
        public bool Success { get; }
        public string Code { get; }
        public bool Durable { get; }
        public OriginResult(bool success, string code, bool durable = false)
        { Success = success; Code = code ?? throw new ArgumentNullException(nameof(code)); Durable = durable; }
    }
    public sealed class OriginMenuView
    {
        public bool HasCampaign { get; }
        public string Code { get; }
        public OriginMenuView(bool hasCampaign, string code = "ok") { HasCampaign = hasCampaign; Code = code; }
    }
    public sealed class OriginHistoryView
    {
        public int AtAge { get; }
        public string Description { get; }
        public string Source { get; }
        public OriginHistoryView(int atAge, string description, string source)
        { AtAge = atAge; Description = description; Source = source; }
    }
    public sealed class OriginSkillView
    {
        public string Label { get; }
        public int Value { get; }
        public string SourceHistory { get; }
        public OriginSkillView(string label, int value, string sourceHistory)
        { Label = label; Value = value; SourceHistory = sourceHistory; }
    }
    public sealed class OriginClaimView
    {
        public string Text { get; }
        public string Source { get; }
        public int AtAge { get; }
        public OriginClaimView(string text, string source, int atAge) { Text = text; Source = source; AtAge = atAge; }
    }
    public sealed class OriginRevealView
    {
        public string ProfileId => "origin-cohort-v1";
        public string ArtProfileId => "earth-monument-2d-v1";
        public string ActorName { get; }
        public int Age { get; }
        public string Household { get; }
        public string Location { get; }
        public long Tick { get; }
        public IReadOnlyList<OriginHistoryView> History { get; }
        public IReadOnlyList<OriginSkillView> Skills { get; }
        public IReadOnlyList<OriginClaimView> Claims { get; }
        public IReadOnlyList<string> Needs { get; }
        public string ScopeNote => "Xuất thân ngẫu nhiên từ nhóm nhân vật đã biên soạn tại Bến Vạc; bản thử giới hạn.";
        public OriginRevealView(string actorName, int age, string household, string location, long tick,
            IEnumerable<OriginHistoryView> history, IEnumerable<OriginSkillView> skills,
            IEnumerable<OriginClaimView> claims, IEnumerable<string> needs)
        {
            ActorName = actorName; Age = age; Household = household; Location = location; Tick = tick;
            History = Array.AsReadOnly(history.ToArray()); Skills = Array.AsReadOnly(skills.ToArray());
            Claims = Array.AsReadOnly(claims.ToArray()); Needs = Array.AsReadOnly(needs.ToArray());
        }
    }
    public sealed class OriginOpenResult
    {
        public OriginResult Result { get; }
        public IOriginCampaignSession? Session { get; }
        public OriginOpenResult(OriginResult result, IOriginCampaignSession? session = null)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result)); Session = session;
            if (result.Success != (session != null)) throw new ArgumentException("Session/result mismatch");
        }
    }
    public interface IOriginCampaignGateway
    {
        OriginMenuView ReadMenu();
        OriginOpenResult NewCampaign(string requestId, CancellationToken cancellationToken);
        OriginOpenResult Continue();
    }
    public interface IOriginCampaignSession
    {
        OriginRevealView Read();
        OriginResult AdvanceBy(long minutes, string requestId);
        OriginResult Save();
        OriginResult Reload();
    }
}
