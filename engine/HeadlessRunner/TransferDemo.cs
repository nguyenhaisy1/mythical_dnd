using System;
using System.Collections.Generic;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Headless
{
    internal static class TransferDemo
    {
        public static int Run()
        {
            var ids = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = ids.Allocate<Actor>();
            var household = ids.Allocate<Household>();
            var settlement = ids.Allocate<Settlement>();
            var source = ids.Allocate<Lot>();
            var destination = ids.Allocate<Lot>();
            var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0),
                new[] { new ActorState(actor, true, settlement, new[] { household.Value }) },
                new[] {
                    new ResourceLot(source, DefinitionId.Parse("commodity.timber@1"), household.Value, settlement, 10, 0),
                    new ResourceLot(destination, DefinitionId.Parse("commodity.timber@1"), actor.Value, settlement, 0, 0)
                });
            var processor = new CommandProcessor(world, new RuleRegistry(new[] { new TransferResourceRule() }));
            var command = new CommandEnvelope("demo-transfer-1", "TransferResource", actor.ToString(), "Player", 0, 1, 1,
                new Dictionary<string, object?> { ["source_lot_id"] = source.ToString(),
                    ["destination_lot_id"] = destination.ToString(), ["quantity"] = 3L }, "demo-transfer");
            var principal = new ExecutionPrincipal(actor, CommandSource.Player);
            var first = processor.Execute(command, principal);
            var retry = processor.Execute(command, principal);
            var result = processor.InspectCommitted();
            bool valid = first.Outcome == CommandOutcome.Accepted && ReferenceEquals(first, retry)
                && result.World.GetLot(source).OnHand == 7 && result.World.GetLot(destination).OnHand == 3
                && result.World.Revision.Value == 2 && result.Events.Count == 1 && result.AcceptedReceipts.Count == 1;
            Console.WriteLine(JsonSerializer.Serialize(new {
                scope = "in-memory-transfer-demo", passed = valid, durable = false,
                sourceBefore = 10, destinationBefore = 0,
                sourceAfter = result.World.GetLot(source).OnHand, destinationAfter = result.World.GetLot(destination).OnHand,
                revision = result.World.Revision.Value, events = result.Events.Count,
                acceptedReceipts = result.AcceptedReceipts.Count, retryReturnsOriginalReceipt = ReferenceEquals(first, retry)
            }, new JsonSerializerOptions { WriteIndented = true }));
            return valid ? 0 : 1;
        }
    }
}
