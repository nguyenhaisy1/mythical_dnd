using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Application.CommandPipeline;
using Game.Foundation.Random;
using Game.Infrastructure.Persistence;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.WeavingTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    /// <summary>Independent review: final one-running invariant also needs historical occupancy.</summary>
    public sealed class WeavingOverlapAdversarialTests
    {
        [Test]
        public void S17_04_CoherentHistoricalOverlapCannotRestoreEvenWhenBothActivitiesAreCancelled()
        {
            var processor = F.Processor();
            for (var i = 1; i <= 2; i++)
            {
                var principal = F.Principal(processor.InspectCommitted().World);
                Assert.That(processor.Execute(F.Start(processor, "overlap.start" + i), principal).Outcome,
                    Is.EqualTo(CommandOutcome.Accepted));
                Assert.That(processor.Execute(F.Cancel(processor, "overlap.cancel" + i), principal).Outcome,
                    Is.EqualTo(CommandOutcome.Accepted));
            }
            var validBytes = CommittedWorldCodec.Encode(processor.InspectCommitted());
            Assert.DoesNotThrow(() => CommittedWorldCodec.Decode(validBytes),
                "Sequential start/cancel/start/cancel at the same tick is valid");
            var reader = typeof(CommittedWorldCodec).Assembly.GetType("Game.Infrastructure.Persistence.MdcReader", true)!;
            var image = (Dictionary<string, object?>)reader.GetMethod("Decode", BindingFlags.Static | BindingFlags.NonPublic)!
                .Invoke(null, new object[] { validBytes })!;
            long Swap(object? value) => Convert.ToInt64(value) switch { 3 => 4, 4 => 3, var number => number };
            var receipts = ((IEnumerable<object?>)image["accepted"]!).Cast<Dictionary<string, object?>>().ToArray();
            foreach (var receipt in receipts)
            {
                receipt["after"] = Swap(receipt["after"]);
                receipt["before"] = (long)receipt["after"]! - 1;
                var envelope = (Dictionary<string, object?>)receipt["envelope"]!;
                envelope["expected"] = receipt["before"];
                var command = new CommandEnvelope((string)envelope["id"]!, (string)envelope["type"]!,
                    (string)envelope["issuer"]!, (string)envelope["source"]!, (long)envelope["tick"]!,
                    Convert.ToUInt64(envelope["expected"]), Convert.ToInt32(envelope["version"]),
                    (Dictionary<string, object?>)envelope["payload"]!, (string)envelope["correlation"]!);
                receipt["fingerprint"] = command.Fingerprint();
            }
            image["accepted"] = receipts.OrderBy(r => Convert.ToInt64(r["after"])).ToArray();
            var facts = ((IEnumerable<object?>)image["events"]!).Cast<Dictionary<string, object?>>().ToArray();
            foreach (var fact in facts) fact["sequence"] = Swap(fact["sequence"]);
            image["events"] = facts.OrderBy(f => Convert.ToInt64(f["sequence"])).ThenBy(f => Convert.ToInt64(f["index"])).ToArray();
            foreach (var item in (IEnumerable<object?>)image["entities"]!)
            {
                var entity = (Dictionary<string, object?>)item!;
                entity["created"] = Swap(entity["created"]);
                if (entity["retired"] != null) entity["retired"] = Swap(entity["retired"]);
            }
            // Actual state was start1@2/cancel1@3/start2@4/cancel2@5.
            // This is start1@2/start2@3/cancel1@4/cancel2@5, with receipts, facts,
            // identities, job sequences, final zero holds and eight fibers all consistent.
            // It cannot have been accepted: start2 happened while activity1 was running.
            Assert.Throws<FormatException>(() => CommittedWorldCodec.Decode(CanonicalCodec.Encode(image)),
                "Canonical history must reject overlap, even when final activities are both terminal");
        }
    }
}
