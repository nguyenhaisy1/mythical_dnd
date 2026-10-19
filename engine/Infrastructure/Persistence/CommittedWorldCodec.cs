using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Game.Application.CommandPipeline;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;

namespace Game.Infrastructure.Persistence
{
    /// <summary>Versioned whole committed kernel image, not the future periodic snapshot/replay format.</summary>
    public static class CommittedWorldCodec
    {
        private static Dictionary<string,object?> Obj(params object?[] pairs)
        {
            var d=new Dictionary<string,object?>(StringComparer.Ordinal);
            for(int i=0;i<pairs.Length;i+=2)d.Add((string)pairs[i]!,pairs[i+1]);return d;
        }
        private static object?[] Rows<T>(IEnumerable<T> items,Func<T,object?> map)=>items.Select(map).ToArray();
        private static Dictionary<string,object?> Map(object? o)=>o as Dictionary<string,object?> ?? throw new FormatException("IMAGE_OBJECT");
        private static object?[] Arr(object? o)=>(o as IEnumerable<object?>)?.ToArray() ?? throw new FormatException("IMAGE_ARRAY");
        private static string Str(object? o)=>o as string ?? throw new FormatException("IMAGE_STRING");
        private static long Int(object? o)=>o is long n?n:throw new FormatException("IMAGE_INT64");
        private static ulong UInt(object? o)=>o is ulong u?u:o is long l&&l>=0?(ulong)l:throw new FormatException("IMAGE_UINT64");
        private static bool Bool(object? o)=>o is bool b?b:throw new FormatException("IMAGE_BOOL");
        private static T EnumValue<T>(object? o) where T:struct,Enum
        { var n=checked((int)Int(o));if(!Enum.IsDefined(typeof(T),n))throw new FormatException("IMAGE_ENUM");return (T)Enum.ToObject(typeof(T),n); }
        private static EntityId<T> Id<T>(object? o)=>EntityId<T>.Parse(Str(o));
        private static WorldTick Tick(object? o)=>new WorldTick(Int(o));
        private static object Transfer(TransferResource t)=>Obj("source",t.SourceLotId.ToString(),"destination",t.DestinationLotId.ToString(),"quantity",t.Quantity);
        private static TransferResource ReadTransfer(object? o){var d=Map(o);return new TransferResource(Id<Lot>(d["source"]),Id<Lot>(d["destination"]),Int(d["quantity"]));}
        private static object Envelope(CommandEnvelope e)=>Obj("id",e.CommandId,"type",e.CommandType,"issuer",e.IssuerId,"source",e.SourceType,"tick",e.WorldTick,"expected",e.ExpectedRevision,"version",e.PayloadVersion,"payload",e.Payload,"correlation",e.CorrelationId);
        private static CommandEnvelope ReadEnvelope(object? o){var d=Map(o);return new CommandEnvelope(Str(d["id"]),Str(d["type"]),Str(d["issuer"]),Str(d["source"]),Int(d["tick"]),UInt(d["expected"]),checked((int)Int(d["version"])),Map(d["payload"]),Str(d["correlation"]));}
        private static object Receipt(CommandReceipt r)=>Obj("envelope",Envelope(r.OriginalEnvelope),"fingerprint",r.Fingerprint,"outcome",(int)r.Outcome,"error",r.ErrorCode,"before",r.BeforeRevision,"after",r.AfterRevision,"tick",r.CommittedTick.Value,"events",Rows(r.OrderedEventIds,x=>x.ToString()));
        private static CommandReceipt ReadReceipt(object? o)
        {
            var d=Map(o);var e=ReadEnvelope(d["envelope"]);var hash=Str(d["fingerprint"]);
            if(e.Fingerprint()!=hash)throw new FormatException("RECEIPT_FINGERPRINT");
            return new CommandReceipt(e,hash,EnumValue<CommandOutcome>(d["outcome"]),d["error"]==null?null:Str(d["error"]),UInt(d["before"]),UInt(d["after"]),Tick(d["tick"]),Arr(d["events"]).Select(Id<Event>));
        }
        internal static object EventObject(CommittedEvent e)=>Obj("id",e.Id.ToString(),"type",e.EventType,"aggregate",e.AggregateId.ToString(),"tick",e.Tick.Value,"sequence",e.CommitSequence,"index",e.EventIndex,"cause",e.CausationId,"correlation",e.CorrelationId,"rule",e.RuleVersion,"envelope_version",e.EnvelopeVersion,"payload_version",e.PayloadVersion,"payload",e.Payload);
        private static CommittedEvent ReadEvent(object? o){var d=Map(o);return new CommittedEvent(Id<Event>(d["id"]),Str(d["type"]),EntityId.Parse(Str(d["aggregate"])),Tick(d["tick"]),UInt(d["sequence"]),checked((uint)UInt(d["index"])),Str(d["cause"]),Str(d["correlation"]),Str(d["rule"]),checked((int)Int(d["payload_version"])),Map(d["payload"]),checked((int)Int(d["envelope_version"])));}
        internal static object JobObject(ScheduledJob j)=>Obj("id",j.Id.ToString(),"actor",j.ActorId.ToString(),"due",j.DueTick.Value,"phase",(int)j.Phase,"priority",j.Priority,"sequence",j.CreationSequence,"transfer",Transfer(j.Transfer),"handler",j.HandlerKey,"cause",j.CausationId,"correlation",j.CorrelationId,"deferred",j.DeferredFromTick?.Value,"status",(int)j.Status,"ancestors",Rows(j.AncestorKeys,x=>x));
        private static ScheduledJob ReadJob(object? o){var d=Map(o);return new ScheduledJob(Id<Job>(d["id"]),Id<Actor>(d["actor"]),Tick(d["due"]),EnumValue<SchedulerPhase>(d["phase"]),checked((int)Int(d["priority"])),UInt(d["sequence"]),ReadTransfer(d["transfer"]),Str(d["cause"]),Str(d["correlation"]),Str(d["handler"]),d["deferred"]==null?null:Tick(d["deferred"]),EnumValue<ScheduledJobStatus>(d["status"]),Arr(d["ancestors"]).Select(Str));}
        public static byte[] Encode(CommittedWorld state)
        {
            var w=state.World;var s=state.Scheduler;
            return CanonicalCodec.Encode(Obj("image_version",1,"world",w.WorldId,"revision",w.Revision.Value,"tick",w.Tick.Value,
                "cursors",Rows(w.Identity.Cursors,c=>Obj("kind",c.Kind,"next",c.NextCounter,"exhausted",c.Exhausted)),
                "entities",Rows(w.Identity.Entries,e=>Obj("id",e.Id.ToString(),"lifecycle",(int)e.Lifecycle,"created",e.CreationRevision,"retired",e.RetirementRevision)),
                "actors",Rows(w.Actors,a=>Obj("id",a.Id.ToString(),"alive",a.Alive,"location",a.Location.ToString(),"owners",Rows(a.ControlledOwnerIds,x=>x.ToString()))),
                "lots",Rows(w.Lots,l=>Obj("id",l.Id.ToString(),"commodity",l.Commodity.ToString(),"owner",l.Owner.ToString(),"location",l.Location.ToString(),"on_hand",l.OnHand,"reserved",l.Reserved)),
                "rng",Rows(w.RngStates,r=>Obj("key",r.ContextKey,"draw",r.State.DrawIndex,"seed",r.State.Context.Seed,"domain",r.State.Context.Domain,"decision",r.State.Context.Decision,"occurrence",r.State.Context.Occurrence,"entity",r.State.Context.Entity.ToString(),"stream",r.State.Context.Stream)),
                "accepted",Rows(state.AcceptedReceipts,Receipt),"metadata",Rows(state.MetadataReceipts,Receipt),"events",Rows(state.Journal.Events,EventObject),
                "jobs",Rows(s.Jobs,JobObject),"job_next",s.NextCreationSequence,"job_exhausted",s.SequenceExhausted,
                "workflows",Rows(s.Workflows,f=>Obj("id",f.Id,"owner",f.OwnerId.ToString(),"source",(int)f.OwnerSource,"target",f.TargetTick.Value,"status",(int)f.Status,"cursor",f.Cursor?.ToString(),"committed",f.CommittedJobs,"correlation",f.CorrelationId)),
                "executions",Rows(s.Executions,e=>Obj("job",e.JobId.ToString(),"tick",e.Tick.Value,"command",e.CommandId,"error",e.ErrorCode))));
        }
        public static CommittedWorld Decode(byte[] bytes)
        {
            var d=Map(MdcReader.Decode(bytes));if(Int(d["image_version"])!=1)throw new FormatException("UNSUPPORTED_IMAGE");
            var identity=IdentitySnapshot.Restore(UInt(d["world"]),UInt(d["revision"]),Arr(d["cursors"]).Select(x=>{var c=Map(x);return new AllocationCursor(Str(c["kind"]),UInt(c["next"]),Bool(c["exhausted"]));}),Arr(d["entities"]).Select(x=>{var e=Map(x);return new RegistryEntry(EntityId.Parse(Str(e["id"])),EnumValue<EntityLifecycle>(e["lifecycle"]),UInt(e["created"]),e["retired"]==null?null:UInt(e["retired"]));}));
            var actors=Arr(d["actors"]).Select(x=>{var a=Map(x);return new ActorState(Id<Actor>(a["id"]),Bool(a["alive"]),Id<Settlement>(a["location"]),Arr(a["owners"]).Select(v=>EntityId.Parse(Str(v))));});
            var lots=Arr(d["lots"]).Select(x=>{var l=Map(x);return new ResourceLot(Id<Lot>(l["id"]),DefinitionId.Parse(Str(l["commodity"])),EntityId.Parse(Str(l["owner"])),Id<Settlement>(l["location"]),Int(l["on_hand"]),Int(l["reserved"]));});
            var rng=Arr(d["rng"]).Select(x=>{var r=Map(x);return new RngStreamState(Str(r["key"]),new RngState(new RngContext(Str(r["seed"]),Str(r["domain"]),Str(r["decision"]),Str(r["occurrence"]),EntityId.Parse(Str(r["entity"])),Str(r["stream"])),UInt(r["draw"])));});
            var world=new WorldSnapshot(identity,Tick(d["tick"]),actors,lots,rng);
            var accepted=Arr(d["accepted"]).Select(ReadReceipt).ToArray();var metadata=Arr(d["metadata"]).Select(ReadReceipt).ToArray();var events=Arr(d["events"]).Select(ReadEvent).ToArray();
            var jobs=Arr(d["jobs"]).Select(ReadJob).ToArray();var byJob=jobs.ToDictionary(j=>j.Id);
            var workflows=Arr(d["workflows"]).Select(x=>{var f=Map(x);return new AdvanceWorkflow(Str(f["id"]),Id<Actor>(f["owner"]),EnumValue<CommandSource>(f["source"]),Tick(f["target"]),EnumValue<AdvanceStatus>(f["status"]),f["cursor"]==null?null:Id<Job>(f["cursor"]),UInt(f["committed"]),Str(f["correlation"]));}).ToArray();
            var executions=Arr(d["executions"]).Select(x=>{var e=Map(x);var j=byJob[Id<Job>(e["job"])];if(Tick(e["tick"])!=j.DueTick)throw new FormatException("EXECUTION_TICK");return new JobExecution(j,Str(e["command"]),e["error"]==null?null:Str(e["error"]));}).ToArray();
            var scheduler=new SchedulerState(jobs,UInt(d["job_next"]),Bool(d["job_exhausted"]),workflows,executions);
            var receipts=accepted.Concat(metadata).ToDictionary(r=>r.CommandId,StringComparer.Ordinal);
            var transfers=events.Where(e=>e.EventType=="ResourceTransferred").Select(e=>new CommittedTransfer(e.Id,e.CommitSequence,e.Tick,receipts[e.CausationId].OriginalEnvelope,new TransferResource(Id<Lot>(e.Payload["source_lot_id"]),Id<Lot>(e.Payload["destination_lot_id"]),Int(e.Payload["quantity"])))).ToArray();
            foreach(var transfer in transfers)
                if(transfer.Envelope.Fingerprint()!=events.Single(e=>e.Id.Equals(transfer.Id)).Fingerprint())throw new FormatException("TRANSFER_EVENT_SCHEMA");
            var result=new CommittedWorld(world,accepted,metadata,transfers,scheduler,events);Validate(result);
            if(!Encode(result).SequenceEqual(bytes))throw new FormatException("NONCANONICAL_IMAGE_OR_UNKNOWN_FIELDS");return result;
        }
        public static string Hash(byte[] bytes){using var sha=SHA256.Create();return string.Concat(sha.ComputeHash(bytes).Select(b=>b.ToString("x2",CultureInfo.InvariantCulture)));}
        private static void Validate(CommittedWorld s)
        {
            var w=s.World;
            for(int i=0;i<s.AcceptedReceipts.Count;i++)
            {
                var r=s.AcceptedReceipts[i];
                if(r.OrderedEventIds.Count==0||(i>0&&(s.AcceptedReceipts[i-1].AfterRevision>=r.AfterRevision||s.AcceptedReceipts[i-1].CommittedTick>r.CommittedTick)))throw new FormatException("ACCEPTED_RECEIPT_ORDER");
            }
            var events=s.Journal.Events.ToDictionary(e=>e.Id);var used=new HashSet<EntityId<Event>>();
            foreach(var r in s.AcceptedReceipts.Concat(s.MetadataReceipts))
            {
                bool accepted=r.Outcome==CommandOutcome.Accepted||r.Outcome==CommandOutcome.AcceptedPending;
                if(s.AcceptedReceipts.Contains(r)!=accepted||r.AfterRevision>w.Revision.Value||r.BeforeRevision>r.AfterRevision||r.CommittedTick>w.Tick||
                    (accepted&&(r.ErrorCode!=null||r.OriginalEnvelope.ExpectedRevision!=r.BeforeRevision||r.BeforeRevision==ulong.MaxValue||r.AfterRevision!=r.BeforeRevision+1))||(!accepted&&(r.BeforeRevision!=r.AfterRevision||r.OrderedEventIds.Count!=0)))throw new FormatException("RECEIPT_REVISION");
                for(int i=0;i<r.OrderedEventIds.Count;i++)
                {var e=events[r.OrderedEventIds[i]];if(!used.Add(e.Id)||e.CommitSequence!=r.AfterRevision||e.EventIndex!=(uint)i||e.CausationId!=r.CommandId||e.Tick!=r.CommittedTick)throw new FormatException("RECEIPT_EVENT");}
            }
            if(used.Count!=events.Count)throw new FormatException("ORPHAN_EVENT");
            foreach(var e in events.Values)
            {if(w.Identity.Resolve<Event>(e.Id.Value).Lifecycle!=EntityLifecycle.Active||e.Tick>w.Tick||!w.Identity.Entries.Any(x=>x.Id.Equals(e.AggregateId)))throw new FormatException("EVENT_REFERENCE");}
            foreach(var j in s.Scheduler.Jobs)
            {
                if(w.Identity.Resolve<Job>(j.Id.Value).Lifecycle!=(j.Status==ScheduledJobStatus.Pending?EntityLifecycle.Active:EntityLifecycle.Retired)||j.Transfer.Quantity<=0|| (j.Status==ScheduledJobStatus.Pending&&j.DueTick<w.Tick)||j.AncestorKeys.Count>64||j.AncestorKeys.Any(string.IsNullOrWhiteSpace)||j.AncestorKeys.Distinct().Count()!=j.AncestorKeys.Count)throw new FormatException("JOB_REFERENCE");
                w.GetActor(j.ActorId);w.GetLot(j.Transfer.SourceLotId);w.GetLot(j.Transfer.DestinationLotId);
            }
            if(s.Scheduler.Workflows.Select(f=>f.Id).Distinct().Count()!=s.Scheduler.Workflows.Count||s.Scheduler.Workflows.Take(Math.Max(0,s.Scheduler.Workflows.Count-1)).Any(f=>f.Status!=AdvanceStatus.Completed))throw new FormatException("WORKFLOW_ORDER");
            foreach(var f in s.Scheduler.Workflows)
            {
                w.GetActor(f.OwnerId);
                var request=s.AcceptedReceipts.SingleOrDefault(r=>r.CommandId==f.Id) ?? throw new FormatException("WORKFLOW_REQUEST_MISSING");
                var command=request.OriginalEnvelope;
                if(command.CommandType!="AdvanceTime"||command.IssuerId!=f.OwnerId.ToString()||command.SourceType!=CommandProcessor.SourceName(f.OwnerSource)||command.CorrelationId!=f.CorrelationId||
                    command.Payload.Count!=1||!command.Payload.TryGetValue("target_tick",out var target)||Int(target)!=f.TargetTick.Value||
                    (f.Status==AdvanceStatus.Completed&&f.Cursor!=null)||(f.Status!=AdvanceStatus.Completed&&f.TargetTick<w.Tick)||
                    (request.Outcome==CommandOutcome.Accepted&&(f.Status!=AdvanceStatus.Completed||f.CommittedJobs!=0)))throw new FormatException("WORKFLOW_REFERENCE");
                var executed=s.Scheduler.Executions.Where(e=>s.AcceptedReceipts.Any(r=>r.CommandId==e.CommandId&&r.OriginalEnvelope.Payload.TryGetValue("workflow_id",out var id)&&Equals(id,f.Id))).ToArray();
                if((ulong)executed.Length!=f.CommittedJobs|| (f.Status!=AdvanceStatus.Completed&&!(f.Cursor?.Equals(executed.LastOrDefault()?.JobId)??executed.Length==0)))throw new FormatException("WORKFLOW_EXECUTION_COUNT");
            }
            if(s.Scheduler.Executions.Select(e=>e.JobId).Distinct().Count()!=s.Scheduler.Executions.Count)throw new FormatException("DUPLICATE_EXECUTION");
            foreach(var e in s.Scheduler.Executions)
            {
                var job=s.Scheduler.Jobs.Single(j=>j.Id.Equals(e.JobId));
                var receipt=s.AcceptedReceipts.SingleOrDefault(r=>r.CommandId==e.CommandId)??throw new FormatException("EXECUTION_RECEIPT");
                var command=receipt.OriginalEnvelope;
                if(job.Status==ScheduledJobStatus.Pending||command.CommandId!="scheduler/job/"+job.Id||command.CommandType!="ExecuteScheduledTransfer"||command.SourceType!="Scheduler"||command.IssuerId!=job.ActorId.ToString()||command.CorrelationId!=job.CorrelationId||
                   command.Payload.Count!=2||!command.Payload.TryGetValue("job_id",out var jobRef)||!Equals(jobRef,job.Id.ToString())||!command.Payload.TryGetValue("workflow_id",out var flowRef)||!s.Scheduler.Workflows.Any(f=>Equals(flowRef,f.Id))||receipt.CommittedTick!=job.DueTick)
                    throw new FormatException("EXECUTION_REFERENCE");
                var terminal=receipt.OrderedEventIds.Select(id=>events[id]).SingleOrDefault(ev=>ev.AggregateId.Equals(job.Id.Value));
                if(terminal==null||terminal.EventType!=(job.Status==ScheduledJobStatus.Completed?"JobCompleted":"JobCancelled")||!terminal.Payload.TryGetValue("error_code",out var error)||!Equals(error,e.ErrorCode)||!terminal.Payload.TryGetValue("workflow_id",out var wf)||!Equals(wf,flowRef))throw new FormatException("EXECUTION_EVENT");
            }
            foreach(var job in s.Scheduler.Jobs.Where(j=>j.Status==ScheduledJobStatus.Completed))
                if(!s.Scheduler.Executions.Any(e=>e.JobId.Equals(job.Id)))throw new FormatException("COMPLETED_JOB_EXECUTION");
        }
    }
}
