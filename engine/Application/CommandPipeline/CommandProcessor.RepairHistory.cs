using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.Events;
using Game.Domain.Repair;
using Game.Foundation.Identity;

namespace Game.Application.CommandPipeline
{
    public sealed partial class CommandProcessor
    {
        /// <summary>Restore/publication invariant: every repair hold has an actual accepted cause and versioned facts.</summary>
        internal static void ValidateRepairHistory(CommittedWorld state)
        {
            var repair = state.World.Repair;
            if (repair == null) return; // Preserve legacy image/profile behavior.
            void Require(bool value, string suffix)
            { if (!value) throw new ArgumentException("REPAIR_HISTORY_" + suffix); }
            var receipts = state.AcceptedReceipts.ToDictionary(r => r.CommandId, StringComparer.Ordinal);
            var facts = state.Journal.Events.ToDictionary(e => e.Id);
            var activityIds = new HashSet<EntityId>(repair.Activities.Select(a => a.Id.Value));
            var usedCommands = new HashSet<string>(StringComparer.Ordinal);
            foreach (var activity in repair.Activities)
            {
                if (!receipts.TryGetValue(activity.StartCommandId, out var start)) throw new ArgumentException("REPAIR_HISTORY_START_RECEIPT");
                var command = start.OriginalEnvelope;
                Require(start.Outcome == CommandOutcome.Accepted && start.ErrorCode == null &&
                    start.BeforeRevision != ulong.MaxValue && start.AfterRevision == start.BeforeRevision + 1 && command.ExpectedRevision == start.BeforeRevision &&
                    start.Fingerprint == command.Fingerprint() && command.CommandType == "StartRepair" && command.PayloadVersion == 1 &&
                    (command.SourceType == "Player" || command.SourceType == "NPC" || command.SourceType == "Scheduler") &&
                    command.IssuerId == activity.ActorId.ToString() && command.CorrelationId == activity.CorrelationId &&
                    command.WorldTick == activity.StartedTick.Value && start.CommittedTick == activity.StartedTick &&
                    command.Payload.Count == 4 && Matches(command.Payload, "actor_id", activity.ActorId.ToString()) &&
                    Matches(command.Payload, "item_id", activity.ItemId.ToString()) &&
                    Matches(command.Payload, "material_lot_id", activity.MaterialLotId.ToString()) &&
                    Matches(command.Payload, "recipe_id", activity.Recipe.Id.ToString()), "START_ENVELOPE");
                Require(state.World.Identity.Resolve<Activity>(activity.Id.Value).CreationRevision == start.AfterRevision &&
                    state.World.Identity.Resolve<Job>(activity.JobId.Value).CreationRevision == start.AfterRevision, "CREATION_REVISION");
                Require(start.OrderedEventIds.Count == 1 && facts.TryGetValue(start.OrderedEventIds[0], out _), "START_FACT");
                var started = facts[start.OrderedEventIds[0]];
                ValidateFact(started, "RepairStarted", start, activity, true);
                Require(usedCommands.Add(start.CommandId), "DUPLICATE_START");

                var terminalFacts = state.Journal.Events.Where(e => e.AggregateId.Equals(activity.Id.Value) &&
                    (e.EventType == "RepairCompleted" || e.EventType == "ActivityCancelled")).ToArray();
                if (activity.Status == RepairActivityStatus.Running)
                {
                    Require(terminalFacts.Length == 0 && !state.Scheduler.Executions.Any(e => e.JobId.Equals(activity.JobId)), "RUNNING_TERMINAL_FACT");
                    continue;
                }
                Require(terminalFacts.Length == 1, "TERMINAL_FACT");
                var terminalFact = terminalFacts[0];
                if (!receipts.TryGetValue(terminalFact.CausationId, out var terminal)) throw new ArgumentException("REPAIR_HISTORY_TERMINAL_RECEIPT");
                var terminalCommand = terminal.OriginalEnvelope;
                Require(terminal.Outcome == CommandOutcome.Accepted && terminal.ErrorCode == null &&
                    terminal.BeforeRevision != ulong.MaxValue && terminal.AfterRevision == terminal.BeforeRevision + 1 && terminalCommand.ExpectedRevision == terminal.BeforeRevision &&
                    terminal.Fingerprint == terminalCommand.Fingerprint() && terminalCommand.PayloadVersion == 1 &&
                    terminal.OrderedEventIds.Count == 2 &&
                    terminal.OrderedEventIds[0].Equals(terminalFact.Id) && facts.ContainsKey(terminal.OrderedEventIds[1]), "TERMINAL_ENVELOPE");
                var completed = activity.Status == RepairActivityStatus.Completed;
                ValidateFact(terminalFact, completed ? "RepairCompleted" : "ActivityCancelled", terminal, activity, false);
                Require(state.World.Identity.Resolve<Activity>(activity.Id.Value).RetirementRevision == terminal.AfterRevision &&
                    state.World.Identity.Resolve<Job>(activity.JobId.Value).RetirementRevision == terminal.AfterRevision, "RETIREMENT_REVISION");
                var jobFact = facts[terminal.OrderedEventIds[1]];
                Require(jobFact.EventType == (completed ? "JobCompleted" : "JobCancelled") &&
                    jobFact.AggregateId.Equals(activity.JobId.Value) && jobFact.RuleVersion == "scheduler-v1" &&
                    jobFact.PayloadVersion == 1 && jobFact.EnvelopeVersion == 1 && jobFact.EventIndex == 1 &&
                    jobFact.CausationId == terminal.CommandId && jobFact.CorrelationId == terminalCommand.CorrelationId &&
                    jobFact.CommitSequence == terminal.AfterRevision && jobFact.Tick == terminal.CommittedTick &&
                    jobFact.Payload.Count == 4 && Matches(jobFact.Payload, "scheduled_by", activity.StartCommandId) &&
                    Matches(jobFact.Payload, "error_code", activity.TerminalReason), "JOB_TERMINAL_FACT");
                if (terminalCommand.CommandType == "ExecuteScheduledRepair")
                {
                    Require(terminalCommand.SourceType == "Scheduler" && terminalCommand.CorrelationId == activity.CorrelationId &&
                        terminalCommand.IssuerId == activity.ActorId.ToString() &&
                        terminalCommand.CommandId == "scheduler/job/" + activity.JobId && terminalCommand.Payload.Count == 2 &&
                        Matches(terminalCommand.Payload, "job_id", activity.JobId.ToString()) &&
                        terminalCommand.Payload.TryGetValue("workflow_id", out var workflow) && workflow is string &&
                        Matches(jobFact.Payload, "workflow_id", workflow) && terminal.CommittedTick == activity.DueTick &&
                        terminalCommand.WorldTick >= activity.StartedTick.Value && terminalCommand.WorldTick <= activity.DueTick.Value,
                        "INTERNAL_COMPLETION");
                    var executions = state.Scheduler.Executions.Where(e => e.JobId.Equals(activity.JobId)).ToArray();
                    Require(executions.Length == 1 && executions[0].CommandId == terminal.CommandId &&
                        executions[0].Tick == activity.DueTick && executions[0].ErrorCode == activity.TerminalReason,
                        "INTERNAL_EXECUTION");
                    var workflows = state.Scheduler.Workflows.Where(f => f.Id == (string)terminalCommand.Payload["workflow_id"]!).ToArray();
                    Require(workflows.Length == 1 && workflows[0].TargetTick >= activity.DueTick &&
                        receipts.ContainsKey(workflows[0].Id), "INTERNAL_WORKFLOW");
                    var flow = workflows[0]; var request = receipts[flow.Id]; var advance = request.OriginalEnvelope;
                    Require(request.Outcome == CommandOutcome.AcceptedPending && request.ErrorCode == null &&
                        request.AfterRevision <= terminal.BeforeRevision && request.CommittedTick <= activity.DueTick &&
                        request.Fingerprint == advance.Fingerprint() && advance.CommandType == "AdvanceTime" &&
                        advance.PayloadVersion == 1 && advance.IssuerId == flow.OwnerId.ToString() &&
                        advance.SourceType == SourceName(flow.OwnerSource) && advance.CorrelationId == flow.CorrelationId &&
                        advance.Payload.Count == 1 && Matches(advance.Payload, "target_tick", flow.TargetTick.Value),
                        "INTERNAL_ADVANCE_REQUEST");
                }
                else
                {
                    Require(!completed && terminalCommand.CommandType == "CancelActivity" && terminalCommand.Payload.Count == 2 &&
                        terminalCommand.WorldTick == terminal.CommittedTick.Value &&
                        (terminalCommand.SourceType == "Player" || terminalCommand.SourceType == "NPC" || terminalCommand.SourceType == "Scheduler") &&
                        Matches(terminalCommand.Payload, "activity_id", activity.Id.ToString()) &&
                        Matches(terminalCommand.Payload, "reason", activity.TerminalReason) &&
                        Matches(jobFact.Payload, "workflow_id", null), "EXTERNAL_CANCEL");
                    Require(!state.Scheduler.Executions.Any(e => e.JobId.Equals(activity.JobId)), "EXTERNAL_CANCEL_EXECUTION");
                }
                Require(usedCommands.Add(terminal.CommandId), "DUPLICATE_TERMINAL");
            }
            foreach (var receipt in state.AcceptedReceipts.Where(r => r.OriginalEnvelope.CommandType == "StartRepair" ||
                r.OriginalEnvelope.CommandType == "CancelActivity" || r.OriginalEnvelope.CommandType == "ExecuteScheduledRepair" ||
                r.OriginalEnvelope.CommandType == "CompleteRepair"))
                Require(usedCommands.Contains(receipt.CommandId), "ORPHAN_COMMAND");
            foreach (var fact in state.Journal.Events.Where(e => e.RuleVersion == "boat-repair-v1" ||
                e.EventType == "RepairStarted" || e.EventType == "RepairCompleted" || e.EventType == "ActivityCancelled"))
                Require(activityIds.Contains(fact.AggregateId) && usedCommands.Contains(fact.CausationId), "ORPHAN_FACT");

            void ValidateFact(CommittedEvent fact, string type, CommandReceipt receipt, RepairActivity activity, bool starting)
            {
                Require(fact.EventType == type && fact.AggregateId.Equals(activity.Id.Value) && fact.RuleVersion == "boat-repair-v1" &&
                    fact.PayloadVersion == 1 && fact.EnvelopeVersion == 1 && fact.EventIndex == 0 && fact.CausationId == receipt.CommandId &&
                    fact.CorrelationId == receipt.OriginalEnvelope.CorrelationId && fact.CommitSequence == receipt.AfterRevision && fact.Tick == receipt.CommittedTick,
                    "FACT_HEADER");
                var p = fact.Payload;
                Require(p.Count == 12 && Matches(p, "activity_id", activity.Id.ToString()) && Matches(p, "actor_id", activity.ActorId.ToString()) &&
                    Matches(p, "item_id", activity.ItemId.ToString()) && Matches(p, "material_lot_id", activity.MaterialLotId.ToString()) &&
                    Matches(p, "tool_item_id", activity.ToolItemId.ToString()) && Matches(p, "job_id", activity.JobId.ToString()) &&
                    Matches(p, "recipe_id", activity.Recipe.Id.ToString()) && Matches(p, "recipe_hash", activity.Recipe.Fingerprint) &&
                    Matches(p, "reserved_quantity", starting ? activity.Recipe.MaterialQuantity : 0L) &&
                    Matches(p, "material_quantity", activity.Recipe.MaterialQuantity) && Matches(p, "due_tick", activity.DueTick.Value) &&
                    Matches(p, "reason", starting ? null : activity.TerminalReason), "FACT_PAYLOAD");
            }
        }
        private static bool Matches(IReadOnlyDictionary<string, object?> payload, string key, object? value)
            => payload.TryGetValue(key, out var actual) && Equals(actual, value);
    }
}
