using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Infrastructure.Content;
using Game.Infrastructure.Diagnostics;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;

namespace Game.Headless
{
    // Omniscient QA-only entry point. No store, scripts or live authorization are accepted.
    internal static class DiagnosticProbe
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        private const string Fault = "reject-transfer-v1";
        private sealed class RejectTransfer : IRuleHandler
        {
            private readonly TransferResourceRule inner = new TransferResourceRule();
            public string CommandType => inner.CommandType;
            public int PayloadVersion => inner.PayloadVersion;
            public Type PayloadType => inner.PayloadType;
            public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command)
            {
                var evaluated = inner.Evaluate(world, issuer, command);
                return RuleResult.Reject("DIAGNOSTIC_INJECTED").WithChecks(evaluated.Checks);
            }
        }
        private static byte[] Read(string path, int maximum = 16 * 1024 * 1024)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            long size = stream.Length;
            if (size > maximum) throw new InvalidDataException("DIAGNOSTIC_SIZE");
            var bytes = new byte[checked((int)size)]; stream.ReadExactly(bytes);
            if (stream.ReadByte() != -1) throw new InvalidDataException("DIAGNOSTIC_FILE_CHANGED");
            return bytes;
        }
        private static void Fields(JsonElement value, params string[] names)
        {
            if (value.ValueKind != JsonValueKind.Object) throw new InvalidDataException("DIAGNOSTIC_OBJECT");
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var field in value.EnumerateObject())
                if (!seen.Add(field.Name) || !names.Contains(field.Name, StringComparer.Ordinal)) throw new InvalidDataException("DIAGNOSTIC_FIELD: " + field.Name);
            if (seen.Count != names.Length) throw new InvalidDataException("DIAGNOSTIC_REQUIRED_FIELD");
        }
        private static string Text(JsonElement value, string name)
        {
            var field = value.GetProperty(name);
            if (field.ValueKind != JsonValueKind.String) throw new InvalidDataException("DIAGNOSTIC_STRING: " + name);
            return field.GetString()!;
        }
        private static int Number(JsonElement value, string name)
        {
            var field = value.GetProperty(name);
            if (field.ValueKind != JsonValueKind.Number || !field.TryGetInt32(out int result)) throw new InvalidDataException("DIAGNOSTIC_INTEGER: " + name);
            return result;
        }
        private static object Trace(RuleEvaluationTrace t)
        {
            var transfer = (TransferResource)t.Command;
            return new { handler = t.CommandType + "@" + t.PayloadVersion, issuer = t.Issuer, revision = t.BeforeRevision,
                sourceLot = transfer.SourceLotId.ToString(), destinationLot = transfer.DestinationLotId.ToString(), quantity = transfer.Quantity,
                checks = t.Result.Checks, outcome = t.Result.ErrorCode ?? (t.Result.IsNoOp ? "NoOp" : "Proposed"),
                effects = t.Result.Changes?.LotChanges.Select(c => new { lot = c.LotId.ToString(), before = c.ExpectedOnHand, after = c.NewOnHand, c.Owner, c.FieldPath }).ToArray(),
                rng = "not-consumed-by-transfer-rule", checksPolicy = "only-evaluated-checks; omitted-checks-not-run" };
        }
        private static object Command(ReplayObservation o, RuleInspectionHandler inspector, ref int consumed)
        {
            var traces = inspector.Traces;
            var evaluations = traces.Skip(consumed).Select(Trace).ToArray(); consumed = traces.Count;
            var e = o.Action.Envelope;
            return new { index = o.Index, kind = o.Action.Kind, commandId = e?.CommandId, issuer = e?.IssuerId,
                source = e?.SourceType, commandType = e?.CommandType, correlation = e?.CorrelationId, tick = e?.WorldTick, expectedRevision = e?.ExpectedRevision, payload = e?.Payload, principal = o.Action.Principal.ActorId.ToString(),
                budget = o.Action.Budget, beforeRevision = o.Before.World.Revision.Value, afterRevision = o.After.World.Revision.Value,
                outcome = o.Outcome, error = o.ErrorCode, evaluations,
                committedEvents = o.After.Journal.Events.Skip(o.Before.Journal.Events.Count).Select(x => x.Id.ToString()).ToArray(),
                changes = StateDiffer.Compare(o.Before, o.After, 1000) };
        }
        private static ContentRegistry Registry(string path) => ContentCompiler.Compile(ContentJson.Read(path));
        private static void Record(string output, string contentPath, string replayPath)
        {
            var registry = Registry(contentPath);
            var source = ReplayArtifactCodec.Decode(Read(replayPath));
            if (source.Actions.Count > 256) throw new InvalidDataException("DIAGNOSTIC_ACTION_LIMIT");
            var initial = CommittedWorldCodec.Decode(source.InitialImage); registry.ValidateWorld(initial.World);
            var manifest = new ReplayManifest(registry.ContentHash, worldSeed: new string('0', 64));
            CommittedWorld? expected = null;
            var artifact = ReplayHarness.Record(initial, source.Actions, manifest, registry.CreateRuleRegistry(),
                observer: o => { if (o.Index == 1) expected = o.After; });
            if (expected == null) throw new InvalidDataException("DIAGNOSTIC_FIXTURE_ACTIONS");
            var artifactBytes = ReplayArtifactCodec.Encode(artifact); var image = CommittedWorldCodec.Encode(expected);
            var package = new { package_version = 1, fault = Fault, content_hash = registry.ContentHash, rule_version = registry.RuleVersion,
                seed = manifest.WorldSeed, artifact_sha256 = CommittedWorldCodec.Hash(artifactBytes), artifact_base64 = Convert.ToBase64String(artifactBytes),
                expected_action = 1, expected_category = "result", expected_state_sha256 = CommittedWorldCodec.Hash(image), expected_state_base64 = Convert.ToBase64String(image) };
            using var file = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            JsonSerializer.Serialize(file, package, Json);
        }
        internal static int Run(string[] args)
        {
            try
            {
                if (args[0] == "--record-diagnostic-fixture" && args.Length == 4) { Record(args[1], args[2], args[3]); return 0; }
                if (args[0] != "--diagnose" || args.Length != 3) throw new InvalidDataException("DIAGNOSTIC_ARGUMENTS");
                using var doc = JsonDocument.Parse(Read(args[1]), new JsonDocumentOptions { MaxDepth = 8 });
                var p = doc.RootElement;
                Fields(p, "package_version", "fault", "content_hash", "rule_version", "seed", "artifact_sha256", "artifact_base64", "expected_action", "expected_category", "expected_state_sha256", "expected_state_base64");
                if (Number(p, "package_version") != 1 || Text(p, "fault") != Fault) throw new InvalidDataException("DIAGNOSTIC_PROFILE");
                var registry = Registry(args[2]);
                registry.EnsureCompatible(registry.PackId, registry.Version, Text(p, "content_hash"));
                if (Text(p, "rule_version") != registry.RuleVersion) throw new InvalidDataException("DIAGNOSTIC_RULE_VERSION");
                var artifactBytes = Convert.FromBase64String(Text(p, "artifact_base64"));
                var expectedBytes = Convert.FromBase64String(Text(p, "expected_state_base64"));
                if (CommittedWorldCodec.Hash(artifactBytes) != Text(p, "artifact_sha256") || CommittedWorldCodec.Hash(expectedBytes) != Text(p, "expected_state_sha256")) throw new InvalidDataException("DIAGNOSTIC_CHECKSUM");
                var artifact = ReplayArtifactCodec.Decode(artifactBytes);
                int wantedIndex = Number(p, "expected_action");
                if (artifact.Actions.Count > 256 || wantedIndex < 0 || wantedIndex >= artifact.Actions.Count) throw new InvalidDataException("DIAGNOSTIC_ACTION_LIMIT");
                var expected = CommittedWorldCodec.Decode(expectedBytes);
                registry.ValidateWorld(CommittedWorldCodec.Decode(artifact.InitialImage).World); registry.ValidateWorld(expected.World);
                var manifest = new ReplayManifest(registry.ContentHash, worldSeed: Text(p, "seed"));
                var baselineInspector = new RuleInspectionHandler(new TransferResourceRule());
                var baselineCommands = new List<object>(); int baselineConsumed = 0; CommittedWorld? baselineAtFault = null;
                var baseline = ReplayHarness.Replay(artifact, manifest, new RuleRegistry(new[] { baselineInspector }), observer: o => {
                    baselineCommands.Add(Command(o, baselineInspector, ref baselineConsumed));
                    if (o.Index == wantedIndex) baselineAtFault = o.After;
                });
                if (!baseline.Success || baselineAtFault == null || !CommittedWorldCodec.Encode(baselineAtFault).SequenceEqual(expectedBytes)) throw new InvalidDataException("DIAGNOSTIC_BASELINE_MISMATCH");
                var faultInspector = new RuleInspectionHandler(new RejectTransfer());
                var actualCommands = new List<object>(); int faultConsumed = 0;
                var actual = ReplayHarness.Replay(artifact, manifest, new RuleRegistry(new[] { faultInspector }), observer: o => actualCommands.Add(Command(o, faultInspector, ref faultConsumed)));
                if (actual.Success || actual.Divergence!.Index != wantedIndex || actual.Divergence.Category != Text(p, "expected_category")) throw new InvalidDataException("DIAGNOSTIC_NOT_REPRODUCED");
                var causal = CausalInspector.Inspect(baseline.FinalState);
                var jobs = SchedulerInspector.Inspect(baseline.FinalState);
                Console.WriteLine(JsonSerializer.Serialize(new {
                    reproduced = true, profile = "diagnostic-v1", fault = Fault, contentHash = registry.ContentHash, seed = manifest.WorldSeed,
                    artifactHash = Text(p, "artifact_sha256"), divergence = actual.Divergence,
                    expectedStateHash = CommittedWorldCodec.Hash(expectedBytes), actualStateHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(actual.FinalState)),
                    expectedActualDiff = StateDiffer.Compare(expected, actual.FinalState), baselineCommands, actualCommands,
                    traceDropped = baselineInspector.DroppedCount + faultInspector.DroppedCount, baselineCausal = causal, actualCausal = CausalInspector.Inspect(actual.FinalState),
                    baselineJobs = jobs.Select(x => new { id = x.Job.Id.ToString(), actor = x.Job.ActorId.ToString(), due = x.Job.DueTick.Value,
                        status = x.Job.Status.ToString(), cause = x.Job.CausationId, correlation = x.Job.CorrelationId,
                        handler = x.Job.HandlerKey, command = x.Execution?.CommandId, x.CancellationReason, x.CancellationReasonStatus,
                        terminalEvent = x.TerminalEvent?.Id.ToString() }).ToArray(),
                    scope = "omniscient developer QA; no production mutation or player projection"
                }, Json));
                return 0;
            }
            catch (Exception e) when (e is InvalidDataException || e is IOException || e is JsonException || e is FormatException ||
                e is ArgumentException || e is ContentValidationException || e is ReplayCompatibilityException)
            { Console.Error.WriteLine(JsonSerializer.Serialize(new { reproduced = false, error = e.Message })); return 1; }
        }
    }
}
