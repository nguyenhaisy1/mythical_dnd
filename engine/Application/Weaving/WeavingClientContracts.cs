using System;
using System.Threading;
using Game.Application.Origins;

namespace Game.Application.Weaving
{
    public sealed class WeavingWorkView
    {
        public bool Qualified { get; }
        public int SkillValue { get; }
        public int RequiredSkill { get; }
        public long FiberOnHand { get; }
        public long FiberReserved { get; }
        public long CordOnHand { get; }
        public string Status { get; }
        public long Tick { get; }
        public long DueTick { get; }
        public bool CanStart { get; }
        public bool CanCancel { get; }
        public string ScopeNote { get; }
        public WeavingWorkView(bool qualified, int skillValue, int requiredSkill, long fiberOnHand,
            long fiberReserved, long cordOnHand, string status, long tick, long dueTick,
            bool canStart, bool canCancel, string scopeNote)
        {
            Qualified = qualified; SkillValue = skillValue; RequiredSkill = requiredSkill;
            FiberOnHand = fiberOnHand; FiberReserved = fiberReserved; CordOnHand = cordOnHand;
            Status = status ?? throw new ArgumentNullException(nameof(status)); Tick = tick; DueTick = dueTick;
            CanStart = canStart; CanCancel = canCancel;
            ScopeNote = scopeNote ?? throw new ArgumentNullException(nameof(scopeNote));
        }
    }
    public sealed class WeavingPreview
    {
        public bool Allowed { get; }
        public string Code { get; }
        public string? Token { get; }
        public int FiberCost { get; }
        public long DurationTicks { get; }
        public int CordYield { get; }
        public WeavingPreview(bool allowed, string code, string? token, int fiberCost, long durationTicks, int cordYield)
        {
            Allowed = allowed; Code = code ?? throw new ArgumentNullException(nameof(code));
            Token = token; FiberCost = fiberCost; DurationTicks = durationTicks; CordYield = cordYield;
        }
    }
    public interface IWeavingCampaignSession : IOriginCampaignSession
    {
        WeavingWorkView ReadWork();
        WeavingPreview Preview();
        OriginResult Start(string previewToken, string requestId);
        OriginResult Cancel(string requestId);
    }
    public sealed class WeavingOpenResult
    {
        public OriginResult Result { get; }
        public IWeavingCampaignSession? Session { get; }
        public WeavingOpenResult(OriginResult result, IWeavingCampaignSession? session = null)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
            if (result.Success != (session != null)) throw new ArgumentException("Success/session mismatch");
            Session = session;
        }
    }
    public interface IWeavingCampaignGateway
    {
        OriginMenuView ReadMenu();
        WeavingOpenResult NewCampaign(string requestId, CancellationToken cancellationToken);
        WeavingOpenResult Continue();
    }
}
