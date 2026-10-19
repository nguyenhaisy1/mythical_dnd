using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class SchedulerTests
    {
        private static readonly EntityId<Actor> ActorId = new EntityId<Actor>(new EntityId(1,"actor",1));
        private static readonly EntityId<Lot> Source = new EntityId<Lot>(new EntityId(1,"lot",1));
        private static readonly EntityId<Lot> Destination = new EntityId<Lot>(new EntityId(1,"lot",2));
        private static readonly EntityId<Settlement> Location = new EntityId<Settlement>(new EntityId(1,"settlement",1));
        private static readonly ExecutionPrincipal Principal = new ExecutionPrincipal(ActorId,CommandSource.Player);
        private static WorldSnapshot World(long quantity = 5000)
        {
            var stage = IdentitySnapshot.Empty(1).BeginStage(1);
            stage.Allocate<Actor>(); stage.Allocate<Lot>(); stage.Allocate<Lot>(); stage.Allocate<Settlement>();
            return new WorldSnapshot(stage.PrepareSnapshot(),new WorldTick(50),new[] { new ActorState(ActorId,true,Location) },new[] {
                new ResourceLot(Source,DefinitionId.Parse("commodity.timber@1"),ActorId.Value,Location,quantity,2),
                new ResourceLot(Destination,DefinitionId.Parse("commodity.timber@1"),ActorId.Value,Location,0,0) });
        }
        private static CommandProcessor Processor(Action? before = null, Action? after = null) => new CommandProcessor(World(),new RuleRegistry(new[] { new TransferResourceRule() }),before,after);
        private static Dictionary<string,object?> Schedule(long tick=60,int phase=3,int priority=100,long quantity=1) => new Dictionary<string,object?> {
            ["due_tick"]=tick,["phase"]=phase,["priority"]=priority,["source_lot_id"]=Source.ToString(),["destination_lot_id"]=Destination.ToString(),["quantity"]=quantity };
        private static Dictionary<string,object?> Advance(long target=100) => new Dictionary<string,object?> { ["target_tick"]=target };
        private static CommandEnvelope Envelope(CommandProcessor p,string id,string type,Dictionary<string,object?> payload,ulong? revision=null,long? tick=null,string source="Player")
        {
            var world=p.InspectCommitted().World;
            return new CommandEnvelope(id,type,ActorId.ToString(),source,tick??world.Tick.Value,revision??world.Revision.Value,1,payload,"scheduler.test");
        }
        private static CommandReceipt Send(CommandProcessor p,string id,string type,Dictionary<string,object?> payload) => p.Execute(Envelope(p,id,type,payload),Principal);

        [Test]
        public void TEST_028_AdvanceWithoutJobsCommitsClockOnceAndEqualTargetIsMetadataNoop()
        {
            var p=Processor(); var before=p.InspectCommitted();
            var receipt=Send(p,"advance","AdvanceTime",Advance());
            Assert.That(receipt.Outcome,Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(p.InspectCommitted().World.Tick.Value,Is.EqualTo(100));
            Assert.That(p.InspectCommitted().World.Revision.Value,Is.EqualTo(2UL));
            var after=p.InspectCommitted();
            Assert.That(Send(p,"same","AdvanceTime",Advance()).Outcome,Is.EqualTo(CommandOutcome.NoOp));
            Assert.That(p.InspectCommitted().World,Is.SameAs(after.World));
            Assert.That(before.World.Tick.Value,Is.EqualTo(50));
        }

        [Test]
        public void TEST_028_AdvancePendingReceiptSurvivesDrainAndRetryWithoutRepeatingEffects()
        {
            var p=Processor(); Send(p,"schedule","ScheduleTransfer",Schedule());
            var command=Envelope(p,"advance","AdvanceTime",Advance()); var receipt=p.Execute(command,Principal);
            Assert.That(receipt.Outcome,Is.EqualTo(CommandOutcome.AcceptedPending));
            Assert.That(p.InspectCommitted().World.Tick.Value,Is.EqualTo(50));
            Assert.That(p.InspectCommitted().World.Revision.Value,Is.EqualTo(3UL));
            p.DrainAdvance(Principal);
            var final=p.InspectCommitted();
            Assert.That(final.World.Revision.Value,Is.EqualTo(5UL));
            Assert.That(final.World.Tick.Value,Is.EqualTo(100));
            Assert.That(final.World.GetLot(Source).OnHand,Is.EqualTo(4999));
            Assert.That(final.World.GetLot(Destination).OnHand,Is.EqualTo(1));
            Assert.That(final.Events.Count,Is.EqualTo(1));
            Assert.That(final.Scheduler.Workflow!.Status,Is.EqualTo(AdvanceStatus.Completed));
            Assert.That(final.Scheduler.Workflow.Cursor,Is.Null);
            Assert.That(final.Scheduler.Executions.Count,Is.EqualTo(1));
            Assert.That(p.Execute(command,Principal),Is.SameAs(receipt));
            Assert.That(p.InspectCommitted(),Is.SameAs(final));
        }

        [TestCase("ScheduleTransfer")]
        [TestCase("AdvanceTime")]
        public void TEST_028_RetryPrecedesStaleMismatchDoesNotPoisonAndAuthCannotBeSpoofed(string type)
        {
            var p=Processor(); var payload=type=="ScheduleTransfer"?Schedule():Advance();
            var command=Envelope(p,"request",type,payload); var receipt=p.Execute(command,Principal);
            Assert.That(p.Execute(command,Principal),Is.SameAs(receipt));
            var mismatch=Envelope(p,"request",type,payload);
            Assert.That(p.Execute(mismatch,Principal).ErrorCode,Is.EqualTo("CMD_PAYLOAD_MISMATCH"));
            Assert.That(p.Execute(command,Principal),Is.SameAs(receipt));
            var before=p.InspectCommitted();
            Assert.That(p.Execute(Envelope(p,"spoof",type,payload,source:"Scheduler"),Principal).ErrorCode,Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
            Assert.That(p.Execute(Envelope(p,"stale",type,payload,revision:0),Principal).ErrorCode,Is.EqualTo("STALE_REVISION"));
        }

        [TestCase("AdvanceTime",49L)]
        [TestCase("AdvanceTime",-1L)]
        [TestCase("ScheduleTransfer",49L)]
        [TestCase("ScheduleTransfer",-1L)]
        public void TEST_028_PastAndNegativeTimeRejectWithoutClockIdsEventsOrJobs(string type,long tick)
        {
            var p=Processor(); var before=p.InspectCommitted();
            var receipt=Send(p,"invalid",type,type=="AdvanceTime"?Advance(tick):Schedule(tick));
            Assert.That(receipt.Outcome,Is.EqualTo(CommandOutcome.Rejected));
            Assert.That(p.InspectCommitted().World,Is.SameAs(before.World));
            Assert.That(p.InspectCommitted().Events,Is.Empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TEST_028_SchedulingAndAdvancePrecommitFailureLeavesPublicationUnchanged(bool advance)
        {
            bool fail=false; var p=Processor(before:()=> { if(fail)throw new InvalidOperationException("precommit"); });
            if(advance)Send(p,"schedule","ScheduleTransfer",Schedule());
            var before=p.InspectCommitted(); fail=true;
            Assert.Throws<InvalidOperationException>(()=>Send(p,"request",advance?"AdvanceTime":"ScheduleTransfer",advance?Advance():Schedule()));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
        }

        private sealed class Continuation : IJobContinuationHandler
        {
            public string Key => "test-handler";
            private readonly Func<WorldSnapshot,ScheduledJob,JobContinuation> evaluate;
            public Continuation(Func<WorldSnapshot,ScheduledJob,JobContinuation> evaluate) { this.evaluate=evaluate; }
            public JobContinuation Evaluate(WorldSnapshot world,ScheduledJob job) => evaluate(world,job);
        }
        private static CommandProcessor WithHandler(Func<WorldSnapshot,ScheduledJob,JobContinuation> handler)
            => new CommandProcessor(World(),new RuleRegistry(new[] { new TransferResourceRule() }),jobHandlers:new JobHandlerRegistry(new[] { new Continuation(handler) }));
        private static Dictionary<string,object?> WithHandlerPayload(long tick=60,int phase=3)
        { var payload=Schedule(tick,phase);payload["handler_key"]="test-handler";return payload; }
        private static TransferResource Transfer(long quantity=1) => new TransferResource(Source,Destination,quantity);

        [Test]
        public void TEST_028_ThousandJobBudgetStopsWithoutDroppingWorkAndResumeCloses()
        {
            var p=Processor();
            for(int i=0;i<1001;i++)Send(p,"schedule."+i,"ScheduleTransfer",Schedule());
            Send(p,"advance","AdvanceTime",Advance());
            var paused=p.DrainAdvance(Principal);
            Assert.That(paused.Status,Is.EqualTo(AdvanceStatus.PausedBudget));
            Assert.That(paused.CommittedJobs,Is.EqualTo(1000UL));
            Assert.That(p.InspectCommitted().World.Tick.Value,Is.EqualTo(60));
            Assert.That(p.InspectCommitted().Scheduler.Jobs.Count(j=>j.Status==ScheduledJobStatus.Pending),Is.EqualTo(1));
            Assert.That(p.InspectCommitted().World.GetLot(Destination).OnHand,Is.EqualTo(1000));
            Assert.That(p.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.Completed));
            Assert.That(p.InspectCommitted().World.GetLot(Destination).OnHand,Is.EqualTo(1001));
            Assert.That(p.InspectCommitted().World.Tick.Value,Is.EqualTo(100));
        }

        [TestCase(0,61L)]
        [TestCase(3,61L)]
        [TestCase(4,60L)]
        public void TEST_028_FollowupPhaseDefersOnlySameOrEarlierPhase(int phase,long expectedTick)
        {
            var p=WithHandler((_,__)=>new JobContinuation(new[] { new JobProposal(new WorldTick(60),(SchedulerPhase)phase,100,Transfer()) }));
            Send(p,"schedule","ScheduleTransfer",WithHandlerPayload()); Send(p,"advance","AdvanceTime",Advance());
            p.DrainAdvance(Principal);
            var jobs=p.InspectCommitted().Scheduler.Jobs;
            var child=jobs.Single(j=>j.CreationSequence==2);
            Assert.That(child.DueTick.Value,Is.EqualTo(expectedTick));
            Assert.That(child.DeferredFromTick?.Value,Is.EqualTo(expectedTick==61?60L:(long?)null));
            Assert.That(p.InspectCommitted().Scheduler.Executions.Select(x=>x.Tick.Value),Is.EqualTo(new[] {60L,expectedTick}));
        }

        [Test]
        public void TEST_028_DecisionCannotBlindlyResumeAndOwnerMustResolveExplicitly()
        {
            var p=WithHandler((_,__)=>new JobContinuation(requiresDecision:true));
            Send(p,"schedule","ScheduleTransfer",WithHandlerPayload());Send(p,"advance","AdvanceTime",Advance());
            var paused=p.DrainAdvance(Principal); var before=p.InspectCommitted();
            Assert.That(paused.Status,Is.EqualTo(AdvanceStatus.PausedDecision));
            Assert.That(p.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.PausedDecision));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
            Send(p,"resolve","ResolveAdvanceDecision",new Dictionary<string,object?> { ["workflow_id"]=paused.Id });
            Assert.That(p.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.Completed));
            Assert.That(p.InspectCommitted().Events.Count,Is.EqualTo(1));
        }

        [TestCase("ScheduleTransfer")]
        [TestCase("TransferResource")]
        [TestCase("AdvanceTime")]
        public void TEST_028_PendingWorkflowBlocksNewMutationButAllowsOriginalRetry(string type)
        {
            var p=Processor(); var schedule=Envelope(p,"schedule","ScheduleTransfer",Schedule());var receipt=p.Execute(schedule,Principal);
            Send(p,"advance","AdvanceTime",Advance());var before=p.InspectCommitted();
            var payload=type=="AdvanceTime"?Advance():Schedule();
            if(type=="TransferResource") { payload.Remove("due_tick");payload.Remove("phase");payload.Remove("priority"); }
            Assert.That(Send(p,"conflict",type,payload).Outcome,Is.EqualTo(CommandOutcome.Rejected));
            Assert.That(p.InspectCommitted().World,Is.SameAs(before.World));
            Assert.That(p.Execute(schedule,Principal),Is.SameAs(receipt));
        }

        [Test]
        public void TEST_028_CurrentTickWithDueJobStillRequestsExecutesAndCloses()
        {
            var p=Processor();Send(p,"schedule","ScheduleTransfer",Schedule(50));
            Assert.That(Send(p,"advance","AdvanceTime",Advance(50)).Outcome,Is.EqualTo(CommandOutcome.AcceptedPending));
            p.DrainAdvance(Principal);
            Assert.That(p.InspectCommitted().World.Revision.Value,Is.EqualTo(5UL));
            Assert.That(p.InspectCommitted().World.Tick.Value,Is.EqualTo(50));
            Assert.That(p.InspectCommitted().World.GetLot(Destination).OnHand,Is.EqualTo(1));
        }

        [Test]
        public void TEST_028_CancelPendingJobIsTerminalAndCannotTransferOnAdvance()
        {
            var p=Processor();Send(p,"schedule","ScheduleTransfer",Schedule());
            var job=p.InspectCommitted().Scheduler.Jobs.Single();
            Send(p,"advance","AdvanceTime",Advance());
            Assert.That(Send(p,"cancel","CancelJob",new Dictionary<string,object?> { ["job_id"]=job.Id.ToString() }).Outcome,Is.EqualTo(CommandOutcome.Accepted));
            p.DrainAdvance(Principal);
            Assert.That(p.InspectCommitted().World.GetLot(Destination).OnHand,Is.Zero);
            Assert.That(p.InspectCommitted().Scheduler.Jobs.Single().Status,Is.EqualTo(ScheduledJobStatus.Cancelled));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void TEST_028_FailureAtJobOrCloseKeepsLastPublicationAndResumeDoesNotRepeat(int commitToFail)
        {
            bool armed=false;int commits=0;
            var p=Processor(before:()=>{if(armed && ++commits==commitToFail)throw new InvalidOperationException("fault");});
            Send(p,"schedule","ScheduleTransfer",Schedule());Send(p,"advance","AdvanceTime",Advance());
            var before=p.InspectCommitted();armed=true;
            Assert.Throws<InvalidOperationException>(()=>p.DrainAdvance(Principal));
            var stopped=p.InspectCommitted();
            Assert.That(stopped.World.GetLot(Destination).OnHand,Is.EqualTo(commitToFail==1?0:1));
            Assert.That(stopped.World.Tick.Value,Is.EqualTo(commitToFail==1?50:60));
            if(commitToFail==1)Assert.That(stopped,Is.SameAs(before));
            armed=false;p.DrainAdvance(Principal);
            Assert.That(p.InspectCommitted().World.GetLot(Destination).OnHand,Is.EqualTo(1));
            Assert.That(p.InspectCommitted().Events.Count,Is.EqualTo(1));
            Assert.That(p.InspectCommitted().World.Revision.Value,Is.EqualTo(5UL));
        }

        [Test]
        public void TEST_028_LostJobResponseLeavesAtomicCommitAndResumeDoesNotRepeat()
        {
            bool armed=false;var p=Processor(after:()=>{if(armed)throw new InvalidOperationException("lost response");});
            Send(p,"schedule","ScheduleTransfer",Schedule());Send(p,"advance","AdvanceTime",Advance());armed=true;
            Assert.Throws<InvalidOperationException>(()=>p.DrainAdvance(Principal));
            var stopped=p.InspectCommitted();Assert.That(stopped.World.GetLot(Destination).OnHand,Is.EqualTo(1));
            Assert.That(stopped.Scheduler.Executions.Count,Is.EqualTo(1));
            armed=false;p.DrainAdvance(Principal);
            Assert.That(p.InspectCommitted().Events.Count,Is.EqualTo(1));
        }

        private static (WorldSnapshot world,ScheduledJob[] jobs) Fixture(int count=3,string flaw="")
        {
            var world=World();var stage=world.Identity.BeginStage(2);
            var household=stage.Allocate<Household>();var elsewhere=stage.Allocate<Settlement>();var requester=stage.Allocate<Actor>();
            var jobs=Enumerable.Range(0,count).Select(i=>new ScheduledJob(stage.Allocate<Job>(),ActorId,new WorldTick(60),
                (SchedulerPhase)(i%3),100-i,(ulong)i+1,Transfer(),"fixture."+i,"scheduler.test")).ToArray();
            var actors=new[] { new ActorState(ActorId,flaw!="dead",Location),new ActorState(requester,true,Location) };
            var lots=world.Lots.Select(l=>new ResourceLot(l.Id,l.Commodity,flaw=="authority"?household.Value:l.Owner,flaw=="location"&&l.Id.Equals(Destination)?elsewhere:l.Location,
                flaw=="resource"&&l.Id.Equals(Source)?2:l.OnHand,l.Reserved));
            return (new WorldSnapshot(stage.PrepareSnapshot(),world.Tick,actors,lots),jobs);
        }

        [Test]
        public void FND_10_OrderIsIndependentOfRestoredInsertionAndPreservesImmutableSnapshot()
        {
            var f=Fixture(12);var rules=new RuleRegistry(new[] {new TransferResourceRule()});
            var original=SchedulerState.Restore(f.jobs,13);var reversed=SchedulerState.Restore(f.jobs.Reverse(),13);
            var a=new CommandProcessor(f.world,rules,initialScheduler:original);var b=new CommandProcessor(f.world,rules,initialScheduler:reversed);
            Send(a,"advance","AdvanceTime",Advance());Send(b,"advance","AdvanceTime",Advance());a.DrainAdvance(Principal);b.DrainAdvance(Principal);
            var expected=f.jobs.OrderBy(j=>j.DueTick.Value).ThenBy(j=>j.Phase).ThenBy(j=>j.Priority).ThenBy(j=>j.CreationSequence).Select(j=>j.Id).ToArray();
            Assert.That(a.InspectCommitted().Scheduler.Executions.Select(x=>x.JobId),Is.EqualTo(expected));
            Assert.That(b.InspectCommitted().Scheduler.Executions.Select(x=>x.JobId),Is.EqualTo(expected));
            Assert.That(original.Jobs.All(j=>j.Status==ScheduledJobStatus.Pending),Is.True);
            Assert.Throws<NotSupportedException>(()=>((IList<ScheduledJob>)original.Jobs).Clear());
            Assert.That(f.world.Tick.Value,Is.EqualTo(50));
        }

        [TestCase("dead","ACTOR_DEAD")]
        [TestCase("authority","NOT_AUTHORIZED")]
        [TestCase("location","LOCATION_MISMATCH")]
        [TestCase("resource","INSUFFICIENT_RESOURCE")]
        public void TEST_028_JobRevalidatesCommittedActorAuthorityLocationAndUnreservedResources(string flaw,string error)
        {
            var f=Fixture(1,flaw);var p=new CommandProcessor(f.world,new RuleRegistry(new[] {new TransferResourceRule()}),initialScheduler:SchedulerState.Restore(f.jobs,2));
            var requester=new ExecutionPrincipal(new EntityId<Actor>(new EntityId(1,"actor",2)),CommandSource.Player);
            var command=new CommandEnvelope("advance","AdvanceTime",requester.ActorId.ToString(),"Player",50,2,1,Advance(),"scheduler.test");
            p.Execute(command,requester);p.DrainAdvance(requester);
            var after=p.InspectCommitted();
            Assert.That(after.Scheduler.Jobs.Single().Status,Is.EqualTo(ScheduledJobStatus.Cancelled));
            Assert.That(after.Scheduler.Executions.Single().ErrorCode,Is.EqualTo(error));
            Assert.That(after.World.GetLot(Destination).OnHand,Is.Zero);
            Assert.That(after.World.GetLot(Source).Reserved,Is.EqualTo(2));
            Assert.That(after.Events,Is.Empty);
        }

        [TestCase("fanout")]
        [TestCase("cycle")]
        [TestCase("past")]
        public void TEST_028_InvalidContinuationRollsBackJobClockIdentityEventAndReceipt(string flaw)
        {
            var p=WithHandler((_,job)=>new JobContinuation(flaw=="fanout"
                ? Enumerable.Range(0,33).Select(i=>new JobProposal(new WorldTick(61),SchedulerPhase.ProductionHealth,i,Transfer(i+1)))
                : new[] {new JobProposal(new WorldTick(flaw=="past"?59:61),SchedulerPhase.ProductionHealth,100,Transfer(),flaw=="cycle"?"test-handler":"transfer-v1")}));
            Send(p,"schedule","ScheduleTransfer",WithHandlerPayload());Send(p,"advance","AdvanceTime",Advance());
            var before=p.InspectCommitted();Assert.Throws<PipelineFailureException>(()=>p.DrainAdvance(Principal));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
        }

        [Test]
        public void TEST_028_SequenceExhaustionRejectsWithoutWrappingOrAllocating()
        {
            var p=new CommandProcessor(World(),new RuleRegistry(new[] {new TransferResourceRule()}),initialScheduler:SchedulerState.Restore(Array.Empty<ScheduledJob>(),ulong.MaxValue,true));
            var before=p.InspectCommitted();
            Assert.That(Send(p,"schedule","ScheduleTransfer",Schedule()).ErrorCode,Is.EqualTo("SEQUENCE_EXHAUSTED"));
            Assert.That(p.InspectCommitted().World,Is.SameAs(before.World));
            Assert.That(p.InspectCommitted().Scheduler,Is.SameAs(before.Scheduler));
        }

        [Test]
        public void TEST_028_RestoreRejectsDuplicateIdentitySequenceAndUnknownIdentity()
        {
            var f=Fixture(2);
            Assert.Throws<ArgumentException>(()=>SchedulerState.Restore(new[] {f.jobs[0],f.jobs[0]},3));
            Assert.Throws<ArgumentException>(()=>SchedulerState.Restore(f.jobs,2));
            var unknown=new ScheduledJob(new EntityId<Job>(new EntityId(1,"job",99)),ActorId,new WorldTick(60),SchedulerPhase.Boundary,100,3,Transfer(),"fixture","scheduler.test");
            Assert.Catch(()=>new CommandProcessor(f.world,new RuleRegistry(new[] {new TransferResourceRule()}),initialScheduler:SchedulerState.Restore(new[] {unknown},4)));
        }

        [Test]
        public void TEST_028_ReservedInternalNamespaceCannotBePoisonedByMetadataReceipt()
        {
            var p=Processor();var before=p.InspectCommitted();
            var result=Send(p,"scheduler/job/1","AdvanceTime",Advance(49));
            Assert.That(result.Outcome,Is.EqualTo(CommandOutcome.Rejected));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
        }

        [Test]
        public void TEST_028_DeferralAtMaximumTickFailsAtomicallyInsteadOfWrapping()
        {
            var p=WithHandler((_,job)=>new JobContinuation(new[] {new JobProposal(new WorldTick(long.MaxValue),job.Phase,100,Transfer())}));
            Send(p,"schedule","ScheduleTransfer",WithHandlerPayload(long.MaxValue));Send(p,"advance","AdvanceTime",Advance(long.MaxValue));
            var before=p.InspectCommitted();Assert.Throws<PipelineFailureException>(()=>p.DrainAdvance(Principal));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
        }

        [TestCase("schedule")]
        [TestCase("advance")]
        public void TEST_028_LostRequestResponseReturnsOriginalReceiptWithoutSecondCommit(string kind)
        {
            bool fail=false;var p=Processor(after:()=>{if(fail)throw new InvalidOperationException("response lost");});
            if(kind=="advance")Send(p,"schedule","ScheduleTransfer",Schedule());
            var command=Envelope(p,"request",kind=="advance"?"AdvanceTime":"ScheduleTransfer",kind=="advance"?Advance():Schedule());fail=true;
            Assert.Throws<InvalidOperationException>(()=>p.Execute(command,Principal));
            var committed=p.InspectCommitted();var receipt=committed.AcceptedReceipts.Single(r=>r.CommandId=="request");
            Assert.That(p.Execute(command,Principal),Is.SameAs(receipt));
            Assert.That(p.InspectCommitted(),Is.SameAs(committed));
        }

        [Test]
        public void TEST_028_DrainRejectsDifferentAuthenticatedOwnerWithoutMutation()
        {
            var p=Processor();Send(p,"schedule","ScheduleTransfer",Schedule());Send(p,"advance","AdvanceTime",Advance());
            var before=p.InspectCommitted();
            var other=new ExecutionPrincipal(new EntityId<Actor>(new EntityId(1,"actor",2)),CommandSource.Player);
            Assert.Throws<PipelineFailureException>(()=>p.DrainAdvance(other));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
        }

        [Test]
        public void FND_10_TotalOrderingUsesDueThenPhaseThenPriorityThenCreationSequence()
        {
            var f=Fixture(5);var ticks=new long[] {61,60,60,60,60};var phases=new[] {0,3,2,2,2};var priorities=new[] {-9,-9,1,0,0};
            var jobs=f.jobs.Select((j,i)=>new ScheduledJob(j.Id,j.ActorId,new WorldTick(ticks[i]),(SchedulerPhase)phases[i],priorities[i],j.CreationSequence,j.Transfer,j.CausationId,j.CorrelationId)).ToArray();
            var p=new CommandProcessor(f.world,new RuleRegistry(new[] {new TransferResourceRule()}),initialScheduler:SchedulerState.Restore(jobs.Reverse(),6));
            Send(p,"advance","AdvanceTime",Advance());p.DrainAdvance(Principal);
            Assert.That(p.InspectCommitted().Scheduler.Executions.Select(x=>x.JobId),Is.EqualTo(new[] {jobs[3].Id,jobs[4].Id,jobs[2].Id,jobs[1].Id,jobs[0].Id}));
        }

        [TestCase("ScheduleTransfer",false)]
        [TestCase("ScheduleTransfer",true)]
        [TestCase("AdvanceTime",false)]
        [TestCase("AdvanceTime",true)]
        public void FND_10_ArgumentExceptionAtPublicationIsSystemFailureNotMetadataRejection(string type,bool after)
        {
            bool armed=false;Action fault=()=>{if(armed)throw new ArgumentException("publication fault");};
            var p=Processor(before:after?null:fault,after:after?fault:null);
            if(type=="AdvanceTime")Send(p,"schedule","ScheduleTransfer",Schedule());
            var command=Envelope(p,"request",type,type=="AdvanceTime"?Advance():Schedule());var before=p.InspectCommitted();armed=true;
            Assert.Throws<ArgumentException>(()=>p.Execute(command,Principal));
            var stopped=p.InspectCommitted();Assert.That(stopped.MetadataReceipts,Is.Empty);
            if(!after)Assert.That(stopped,Is.SameAs(before));
            else
            {
                var accepted=stopped.AcceptedReceipts.Single(r=>r.CommandId=="request");
                Assert.That(p.Execute(command,Principal),Is.SameAs(accepted));
                Assert.That(p.InspectCommitted(),Is.SameAs(stopped));
            }
        }

        [Test]
        public void FND_10_JobFactsKeepSchedulingCorrelationWhenAdvanceUsesDifferentCorrelation()
        {
            var p=Processor();Send(p,"schedule","ScheduleTransfer",Schedule());var world=p.InspectCommitted().World;
            p.Execute(new CommandEnvelope("advance","AdvanceTime",ActorId.ToString(),"Player",world.Tick.Value,world.Revision.Value,1,Advance(),"advance.correlation"),Principal);
            p.DrainAdvance(Principal);var committed=p.InspectCommitted();
            var execution=committed.Scheduler.Executions.Single();
            var facts=committed.Journal.Events.Where(e=>e.CausationId==execution.CommandId).ToArray();
            Assert.That(facts.Length,Is.EqualTo(2));
            Assert.That(facts.All(e=>e.CorrelationId=="scheduler.test"),Is.True);
        }

        [Test]
        public void FND_10_CausalDepthLimitStopsUnboundedDistinctReactionsWithoutPartialCommit()
        {
            var p=WithHandler((_,job)=>new JobContinuation(new[] {new JobProposal(job.DueTick.Add(1),job.Phase,100,Transfer(job.Transfer.Quantity+1),"test-handler")}));
            Send(p,"schedule","ScheduleTransfer",WithHandlerPayload());Send(p,"advance","AdvanceTime",Advance(1000));
            var fault=Assert.Throws<PipelineFailureException>(()=>p.DrainAdvance(Principal));
            Assert.That(fault!.Code,Is.EqualTo("CAUSAL_DEPTH_LIMIT"));
            var stopped=p.InspectCommitted();
            Assert.That(stopped.Scheduler.Executions.Count,Is.EqualTo(64));
            Assert.That(stopped.World.GetLot(Destination).OnHand,Is.EqualTo(2080));
            Assert.That(stopped.World.Tick.Value,Is.EqualTo(123));
            Assert.That(stopped.Scheduler.Jobs.Count(j=>j.Status==ScheduledJobStatus.Pending),Is.EqualTo(1));
            Assert.That(Assert.Throws<PipelineFailureException>(()=>p.DrainAdvance(Principal))!.Code,Is.EqualTo("CAUSAL_DEPTH_LIMIT"));
            Assert.That(p.InspectCommitted(),Is.SameAs(stopped));
        }

        [Test]
        public void FND_10_ReentrantDrainFromCommitProbeCannotPublishNestedEffects()
        {
            bool armed=false;CommandProcessor? p=null;
            p=Processor(before:()=>{if(armed)p!.DrainAdvance(Principal);});
            Send(p,"schedule","ScheduleTransfer",Schedule());Send(p,"advance","AdvanceTime",Advance());
            var before=p.InspectCommitted();armed=true;
            Assert.That(Assert.Throws<PipelineFailureException>(()=>p.DrainAdvance(Principal))!.Code,Is.EqualTo("REENTRANT_COMMAND"));
            Assert.That(p.InspectCommitted(),Is.SameAs(before));
        }
    }
}
