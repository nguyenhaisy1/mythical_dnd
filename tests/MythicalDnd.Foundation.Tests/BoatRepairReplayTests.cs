using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Domain.Repair;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Repair;
using Game.Infrastructure.Replay;
using NUnit.Framework;
using F=MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class BoatRepairReplayTests
    {
        private static ReplayManifest Manifest(RepairContentRegistry registry)=>new ReplayManifest(registry.ContentHash,registry.RuleVersion,worldSeed:"repair-seed");
        private static ReplayAction Execute(CommandEnvelope command)=>ReplayAction.Execute(command,F.Principal);
        [Test] public void VS_03A_RepairReplayRerunsStartTimeAndCompletionFromInitialImage()
        {
            var registry=F.Registry();var initial=F.Initial(registry);var original=CommittedWorldCodec.Encode(initial);var observations=new List<ReplayObservation>();
            var actions=new[]{Execute(F.Start()),Execute(F.Start()),Execute(F.Advance()),ReplayAction.Drain(F.Principal)};
            var recorded=ReplayHarness.Record(initial,actions,Manifest(registry),F.Rules(),observer:observations.Add,repairRecipe:registry.Recipe);
            Assert.That(observations.Count,Is.EqualTo(4));Assert.That(observations[0].After.World.GetLot(F.Timber).Reserved,Is.EqualTo(2));
            Assert.That(observations[0].Before.World.GetLot(F.Timber).Reserved,Is.Zero);Assert.That(observations[3].After.World.Repair!.GetItem(F.Boat).Condition,Is.EqualTo(700));
            var bytes=ReplayArtifactCodec.Encode(recorded);var copy=ReplayArtifactCodec.Decode(bytes);var again=new List<ReplayObservation>();
            var replay=ReplayHarness.Replay(copy,Manifest(registry),F.Rules(),observer:again.Add,repairRecipe:registry.Recipe);
            Assert.That(replay.Success,Is.True);Assert.That(again.Count,Is.EqualTo(4));Assert.That(again[0].After.World.GetLot(F.Timber).Reserved,Is.EqualTo(2));
            Assert.That(replay.FinalState.World.GetLot(F.Timber).OnHand,Is.EqualTo(8));Assert.That(replay.FinalState.World.GetLot(F.Timber).Reserved,Is.Zero);
            Assert.That(replay.FinalState.World.Repair!.GetItem(F.Boat).Condition,Is.EqualTo(700));Assert.That(replay.FinalState.World.Repair.GetActivity(F.Activity).Status,Is.EqualTo(RepairActivityStatus.Completed));
            Assert.That(F.Hash(replay.FinalState),Is.EqualTo(F.Hash(observations.Last().After)));Assert.That(CommittedWorldCodec.Encode(initial),Is.EqualTo(original));
            Assert.That(copy.InitialImage,Is.EqualTo(original));Assert.That(recorded.Checkpoints[1].IntegrityHash,Is.EqualTo(recorded.Checkpoints[0].IntegrityHash),"Duplicate Start must reuse receipt and preserve image");
        }
        [Test] public void VS_03A_RepairReplayCancelDuringAdvanceKeepsMaterialsAndTerminalHistory()
        {
            var registry=F.Registry();var artifact=ReplayHarness.Record(F.Initial(registry),new[]{Execute(F.Start()),Execute(F.Advance()),Execute(F.Cancel(3)),ReplayAction.Drain(F.Principal)},Manifest(registry),F.Rules(),repairRecipe:registry.Recipe);
            var result=ReplayHarness.Replay(ReplayArtifactCodec.Decode(ReplayArtifactCodec.Encode(artifact)),Manifest(registry),F.Rules(),repairRecipe:registry.Recipe);
            Assert.That(result.Success,Is.True);Assert.That(result.FinalState.World.GetLot(F.Timber).OnHand,Is.EqualTo(10));Assert.That(result.FinalState.World.GetLot(F.Timber).Reserved,Is.Zero);
            Assert.That(result.FinalState.World.Repair!.GetActivity(F.Activity).Status,Is.EqualTo(RepairActivityStatus.Cancelled));Assert.That(result.FinalState.World.Repair.GetItem(F.Boat).Condition,Is.EqualTo(500));
            Assert.That(result.FinalState.Journal.Events.Count(e=>e.EventType=="ActivityCancelled"),Is.EqualTo(1));
        }
        [Test] public void VS_03A_RepairReplayChangedInitialMaterialMustDivergeRatherThanLoadFinalState()
        {
            var registry=F.Registry();var manifest=Manifest(registry);var recorded=ReplayHarness.Record(F.Initial(registry),new[]{Execute(F.Start()),Execute(F.Advance()),ReplayAction.Drain(F.Principal)},manifest,F.Rules(),repairRecipe:registry.Recipe);
            var different=F.Initial(registry,timber:1);var altered=new ReplayArtifact(recorded.Manifest,CommittedWorldCodec.Encode(different),recorded.Actions,recorded.Checkpoints);
            var result=ReplayHarness.Replay(altered,manifest,F.Rules(),repairRecipe:registry.Recipe);
            Assert.That(result.Success,Is.False);Assert.That(result.Divergence!.Index,Is.Zero);Assert.That(result.Divergence.Category,Is.EqualTo("result"));
            Assert.That(result.FinalState.World.GetLot(F.Timber).OnHand,Is.EqualTo(1));Assert.That(result.FinalState.World.Repair!.Activities,Is.Empty);
        }
        [Test] public void VS_03A_RepairReplayWrongExpectedGameplayHashReportsDivergence()
        {
            var registry=F.Registry();var manifest=Manifest(registry);var recorded=ReplayHarness.Record(F.Initial(registry),new[]{Execute(F.Start()),Execute(F.Advance()),ReplayAction.Drain(F.Principal)},manifest,F.Rules(),repairRecipe:registry.Recipe);
            var points=recorded.Checkpoints.ToArray();var c=points[2];points[2]=new ReplayCheckpoint(c.Revision,c.ResultHash,new string('0',64),c.IntegrityHash,c.JournalHash,c.JobsHash);
            var altered=new ReplayArtifact(recorded.Manifest,recorded.InitialImage,recorded.Actions,points);
            var result=ReplayHarness.Replay(altered,manifest,F.Rules(),repairRecipe:registry.Recipe);
            Assert.That(result.Success,Is.False);Assert.That(result.Divergence!.Index,Is.EqualTo(2));Assert.That(result.Divergence.Category,Is.EqualTo("gameplay"));
            Assert.That(result.FinalState.World.Repair!.GetItem(F.Boat).Condition,Is.EqualTo(700));
        }
        [TestCase("legacy_rule")][TestCase("missing_recipe")][TestCase("content")][TestCase("recipe")]
        public void VS_03A_RepairReplayManifestAndInjectedRecipeMustMatchProfile(string mismatch)
        {
            var registry=F.Registry();var manifest=Manifest(registry);var recorded=ReplayHarness.Record(F.Initial(registry),new[]{Execute(F.Start())},manifest,F.Rules(),repairRecipe:registry.Recipe);
            var expected=mismatch=="legacy_rule"?new ReplayManifest(registry.ContentHash):mismatch=="content"?new ReplayManifest(new string('a',64),registry.RuleVersion,worldSeed:"repair-seed"):manifest;
            RepairRecipe? recipe=mismatch=="missing_recipe"?null:mismatch=="recipe"?new RepairRecipe(registry.Recipe.Id,registry.Recipe.MaterialCommodity,2,60,100,1000):registry.Recipe;
            Assert.Throws<ReplayCompatibilityException>(()=>ReplayHarness.Replay(recorded,expected,F.Rules(),repairRecipe:recipe));
            if(mismatch=="legacy_rule")Assert.Throws<ReplayCompatibilityException>(()=>ReplayHarness.Record(F.Initial(registry),Array.Empty<ReplayAction>(),expected,F.Rules(),repairRecipe:registry.Recipe));
        }
    }
}
