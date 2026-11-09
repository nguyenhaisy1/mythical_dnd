using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Origins;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class OriginSelectorTests
    {
        private static string Seed => new string('0', 64);
        private static string SourceHash => new string('a', 64);
        private static string CohortHash => new string('b', 64);
        private static readonly string[] ActorIds = { "actor.ben-ha@1", "actor.ben-kien@1", "actor.ben-sen@1",
            "actor.vac-lam@1", "actor.vac-mai@1", "actor.vac-moc@1", "actor.vac-nhu@1", "actor.vac-tong@1" };
        private static OriginCandidate Candidate(string id = "actor.adult@1", bool alive = true, int age = 18,
            string community = OriginSelector.HomeDefinitionId)
            => new OriginCandidate(id, "household.test@1", community, alive, age);
        private static OriginCandidate[] Candidates() => ActorIds.Select(id => Candidate(id)).ToArray();
        private static OriginSelection Select(string? seed = null, IEnumerable<OriginCandidate>? candidates = null)
            => OriginSelector.Select(seed ?? Seed, SourceHash, CohortHash, candidates ?? Candidates());

        // Literal expected answers come from root's independent Python MDC1/SHA reference,
        // content/fixtures/origin-cohort/selector-vectors.json; never regenerate them from C#.
        [TestCase("0000000000000000000000000000000000000000000000000000000000000000", "206f8ff78714777f", 1500049851u, "actor.vac-lam@1")]
        [TestCase("1111111111111111111111111111111111111111111111111111111111111111", "c9dd2b91ea1d91c0", 2726344734u, "actor.vac-nhu@1")]
        [TestCase("0123456701234567012345670123456701234567012345670123456701234567", "14f6930dd3648e17", 4026867865u, "actor.ben-kien@1")]
        [TestCase("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", "c391bb6afff3924c", 2614834995u, "actor.vac-lam@1")]
        public void S16_01_IndependentWorldAndSelectionKnownAnswers(string seed, string worldHex, uint word, string actor)
        {
            var selected = Select(seed);
            var expectedWorld = ulong.Parse(worldHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.That(OriginSelector.DeriveWorldId(seed), Is.EqualTo(expectedWorld));
            Assert.That(selected.Selected.ActorDefinitionId, Is.EqualTo(actor));
            Assert.That(selected.Origin.ActorDefinitionId, Is.EqualTo(actor));
            Assert.That(selected.Origin.Principal.ToString(), Is.EqualTo("w" + worldHex + "/actor/0000000000000001"));
            Assert.That(selected.Rng.State.Context.Block(0)[0], Is.EqualTo(word));
            Assert.That(selected.Rng.State.DrawIndex, Is.EqualTo(1UL));
            Assert.That(selected.Origin.DrawIndex, Is.EqualTo(1UL));
            Assert.That(selected.Rng.ContextKey, Is.EqualTo("origin-assignment"));
            Assert.That(selected.Rng.State.Context.Domain, Is.EqualTo("characters"));
            Assert.That(selected.Rng.State.Context.Decision, Is.EqualTo("origin-cohort-v1"));
            Assert.That(selected.Rng.State.Context.Occurrence, Is.EqualTo("genesis"));
            Assert.That(selected.Rng.State.Context.Stream, Is.EqualTo("assignment"));
            Assert.That(selected.Origin.SourceContentHash, Is.EqualTo(SourceHash));
            Assert.That(selected.Origin.CohortContentHash, Is.EqualTo(CohortHash));
        }

        [Test]
        public void S16_01_AllEightAuthoredCandidatesAreReachableWithoutReroll()
        {
            var seen = Enumerable.Range(0, 256).Select(i => Select(((ulong)i).ToString("x64", CultureInfo.InvariantCulture))
                .Selected.ActorDefinitionId).ToHashSet(StringComparer.Ordinal);
            Assert.That(seen, Is.EquivalentTo(ActorIds));
        }

        [Test]
        public void S16_01_ReorderAndIneligibleRecordsDoNotChangeTheDecisionStream()
        {
            var records = Candidates().Reverse().Concat(new[] {
                Candidate("actor.dead@1", alive: false), Candidate("actor.minor@1", age: 17),
                Candidate("actor.away@1", community: "community.con-may@1") }).ToList();
            var result = Select(candidates: records);
            Assert.That(result.Selected.ActorDefinitionId, Is.EqualTo("actor.vac-lam@1"));
            Assert.That(result.Rng.State.DrawIndex, Is.EqualTo(1UL));
            records.Clear();
            Assert.That(result.Selected.ActorDefinitionId, Is.EqualTo("actor.vac-lam@1"));
            Assert.That(Select().Selected.ActorDefinitionId, Is.EqualTo(result.Selected.ActorDefinitionId));
        }

        [TestCase(0, false)]
        [TestCase(17, false)]
        [TestCase(18, true)]
        [TestCase(100, true)]
        public void S16_01_AgeAdmissionIsExplicitAndPrecedesRng(int age, bool eligible)
        {
            var records = new[] { Candidate(age: age) };
            if (eligible)
                Assert.That(Select(candidates: records).Selected.Age, Is.EqualTo(age));
            else Assert.Throws<ArgumentException>(() => Select(candidates: records));
        }

        [Test]
        public void S16_01_EmptyDeadAndForeignOnlyCohortsAreRejected()
        {
            Assert.Throws<ArgumentException>(() => Select(candidates: Array.Empty<OriginCandidate>()));
            Assert.Throws<ArgumentException>(() => Select(candidates: new[] { Candidate(alive: false) }));
            Assert.Throws<ArgumentException>(() => Select(candidates: new[] { Candidate(community: "community.con-may@1") }));
            Assert.Throws<ArgumentNullException>(() => OriginSelector.Select(Seed, SourceHash, CohortHash, null!));
        }

        [Test]
        public void S16_01_DuplicatesAndNullRecordsCannotChangeTheCohortByInputOrder()
        {
            Assert.Throws<ArgumentException>(() => Select(candidates: new[] { Candidate(), Candidate(alive: false) }));
            Assert.Throws<ArgumentException>(() => Select(candidates: new OriginCandidate[] { Candidate(), null! }));
            var tooMany = Enumerable.Range(0, 513).Select(i => Candidate("actor.person-" + i + "@1"));
            Assert.Throws<ArgumentException>(() => Select(candidates: tooMany));
        }

        [TestCase("")]
        [TestCase("0000")]
        [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        [TestCase("gggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg")]
        public void S16_01_InvalidSeedsAreRejectedBeforeWorldIdentity(string seed)
        {
            Assert.Throws<ArgumentException>(() => OriginSelector.DeriveWorldId(seed));
            Assert.Throws<ArgumentException>(() => Select(seed));
        }

        [Test]
        public void S16_01_CandidateAndMetadataRejectInvalidKindsRangesAndHashes()
        {
            Assert.Throws<ArgumentException>(() => Candidate("household.wrong@1"));
            Assert.Throws<FormatException>(() => Candidate("not-an-id"));
            Assert.Throws<ArgumentOutOfRangeException>(() => Candidate(age: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Candidate(age: 101));
            Assert.Throws<ArgumentException>(() => OriginSelector.Select(Seed, "bad", CohortHash, Candidates()));
            Assert.Throws<ArgumentException>(() => OriginSelector.Select(Seed, SourceHash, "bad", Candidates()));
            var o = Select().Origin;
            Assert.Throws<ArgumentOutOfRangeException>(() => Copy(o, draw: 0));
            Assert.Throws<ArgumentException>(() => Copy(o, home: new EntityId<Settlement>(new EntityId(1, "settlement", 1))));
            Assert.Throws<ArgumentException>(() => Copy(o, principal: new EntityId<Actor>(new EntityId(o.Principal.Value.World, "actor", 2))));
        }

        private static CampaignOriginState Copy(CampaignOriginState o, ulong? draw = null,
            EntityId<Actor>? principal = null, EntityId<Settlement>? home = null)
            => new CampaignOriginState(principal ?? o.Principal, o.Household, home ?? o.Home, o.ActorDefinitionId,
                o.HouseholdDefinitionId, o.CommunityDefinitionId, o.SourceContentHash, o.CohortContentHash, draw ?? o.DrawIndex);

        private static WorldSnapshot World(OriginSelection selection, IEnumerable<RngStreamState>? streams = null,
            bool withLots = false, bool omitPrincipal = false, bool omitHousehold = false, bool otherLocationAndDead = false,
            bool extraActor = false, CampaignOriginState? origin = null)
        {
            var o = origin ?? selection.Origin; var stage = IdentitySnapshot.Empty(o.Principal.Value.World).BeginStage(1);
            var actor = stage.Allocate<Actor>(); if (extraActor) stage.Allocate<Actor>();
            if (!omitHousehold) stage.Allocate<Household>();
            var location = stage.Allocate<Settlement>();
            if (otherLocationAndDead) location = stage.Allocate<Settlement>();
            var lots = new List<ResourceLot>();
            if (withLots)
            {
                lots.Add(new ResourceLot(stage.Allocate<Lot>(), DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 5, 0));
                lots.Add(new ResourceLot(stage.Allocate<Lot>(), DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0));
            }
            return new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(0), omitPrincipal ? Array.Empty<ActorState>()
                : new[] { new ActorState(actor, !otherLocationAndDead, location) }, lots,
                streams ?? new[] { selection.Rng }, origin: o);
        }

        [Test]
        public void S16_01_OriginSnapshotRequiresRegisteredPrincipalHouseholdHomeAndRng()
        {
            var s = Select(); var world = World(s);
            Assert.That(world.Origin, Is.SameAs(s.Origin)); Assert.That(world.Repair, Is.Null);
            Assert.Throws<IdentityException>(() => World(s, omitPrincipal: true));
            Assert.Throws<IdentityException>(() => World(s, omitHousehold: true));
            Assert.Throws<ArgumentException>(() => World(s, streams: Array.Empty<RngStreamState>()));
            var badCursor = new RngStreamState(OriginSelector.RngContextKey, new RngState(s.Rng.State.Context, 2));
            Assert.Throws<ArgumentException>(() => World(s, new[] { badCursor }));
            Assert.Throws<ArgumentException>(() => World(s, origin: Copy(s.Origin, draw: 2)));
        }

        [TestCase("domain")]
        [TestCase("decision")]
        [TestCase("occurrence")]
        [TestCase("stream")]
        [TestCase("entity")]
        [TestCase("seed")]
        [TestCase("key")]
        public void S16_01_OriginRngContextCannotBeRebound(string field)
        {
            var s = Select(); var c = s.Rng.State.Context;
            var context = new RngContext(field == "seed" ? new string('1', 64) : c.Seed,
                field == "domain" ? "other" : c.Domain, field == "decision" ? "other" : c.Decision,
                field == "occurrence" ? "other" : c.Occurrence,
                field == "entity" ? new EntityId(c.Entity.World, "actor", 2) : c.Entity,
                field == "stream" ? "other" : c.Stream);
            var rng = new RngStreamState(field == "key" ? "other" : OriginSelector.RngContextKey, new RngState(context, 1));
            Assert.Throws<ArgumentException>(() => World(s, new[] { rng }, extraActor: true));
        }

        [Test]
        public void S16_01_HistoricalHomeAndOriginSurviveLaterDeathOrLocationChanges()
        {
            var s = Select(); var world = World(s, otherLocationAndDead: true);
            Assert.That(world.GetActor(s.Origin.Principal).Alive, Is.False);
            Assert.That(world.GetActor(s.Origin.Principal).Location, Is.Not.EqualTo(s.Origin.Home));
            Assert.That(world.Origin, Is.SameAs(s.Origin));
        }

        [Test]
        public void S16_01_OriginAndRepairCannotShareASnapshotAndLegacyIsUnchanged()
        {
            var legacy = BoatRepairTestFixture.Initial().World;
            Assert.That(legacy.Origin, Is.Null); Assert.That(legacy.Repair, Is.Not.Null);
            var s = Select(); var w = World(s);
            Assert.Throws<ArgumentException>(() => new WorldSnapshot(w.Identity, w.Tick, w.Actors, w.Lots, w.RngStates,
                legacy.Repair, s.Origin));
        }

        private static CommandReceipt Send(CommandProcessor p, OriginSelection s, string id, string type,
            Dictionary<string, object?> payload)
        {
            var w = p.InspectCommitted().World;
            return p.Execute(new CommandEnvelope(id, type, s.Origin.Principal.ToString(), "Player", w.Tick.Value,
                w.Revision.Value, 1, payload, "origin.test"), new ExecutionPrincipal(s.Origin.Principal, CommandSource.Player));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void S16_01_TransferAndScheduledTransferPreserveOriginAndAssignmentCursor(bool scheduled)
        {
            var s = Select(); var initial = World(s, withLots: true);
            var p = new CommandProcessor(initial, new RuleRegistry(new[] { new TransferResourceRule() }));
            var transfer = new Dictionary<string, object?> { ["source_lot_id"] = initial.Lots[0].Id.ToString(),
                ["destination_lot_id"] = initial.Lots[1].Id.ToString(), ["quantity"] = 1L };
            if (scheduled)
            {
                transfer["due_tick"] = 5L; transfer["phase"] = 3L; transfer["priority"] = 100L;
                Assert.That(Send(p, s, "schedule", "ScheduleTransfer", transfer).Outcome, Is.EqualTo(CommandOutcome.Accepted));
                Assert.That(p.InspectCommitted().World.Origin, Is.SameAs(s.Origin));
                Assert.That(Send(p, s, "advance", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 10L }).Outcome,
                    Is.EqualTo(CommandOutcome.AcceptedPending));
                p.DrainAdvance(new ExecutionPrincipal(s.Origin.Principal, CommandSource.Player));
                Assert.That(p.InspectCommitted().Scheduler.Workflow!.Status, Is.EqualTo(AdvanceStatus.Completed));
            }
            else Assert.That(Send(p, s, "transfer", "TransferResource", transfer).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            var final = p.InspectCommitted().World;
            Assert.That(final.GetLot(initial.Lots[0].Id).OnHand, Is.EqualTo(4));
            Assert.That(final.GetLot(initial.Lots[1].Id).OnHand, Is.EqualTo(1));
            Assert.That(final.Origin, Is.SameAs(s.Origin));
            Assert.That(final.RngStates.Single().State.DrawIndex, Is.EqualTo(1UL));
        }

        [Test]
        public void S16_01_AdvanceNoJobsAndPrecommitAbortKeepImmutableAssignment()
        {
            var s = Select(); var initial = World(s); var fail = false;
            var p = new CommandProcessor(initial, new RuleRegistry(new[] { new TransferResourceRule() }),
                beforeCommit: () => { if (fail) throw new InvalidOperationException("test abort"); });
            var receipt = Send(p, s, "wait", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 60L });
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.Accepted));
            var before = p.InspectCommitted(); Assert.That(before.World.Tick.Value, Is.EqualTo(60));
            Assert.That(before.World.Origin, Is.SameAs(s.Origin));
            fail = true;
            Assert.Throws<InvalidOperationException>(() => Send(p, s, "abort", "AdvanceTime",
                new Dictionary<string, object?> { ["target_tick"] = 120L }));
            Assert.That(p.InspectCommitted(), Is.SameAs(before));
            Assert.That(before.World.RngStates.Single().State.DrawIndex, Is.EqualTo(1UL));
        }
    }
}
