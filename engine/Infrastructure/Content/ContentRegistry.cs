using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Game.Domain.Kernel;
using Game.Foundation.Identity;

namespace Game.Infrastructure.Content
{
    public sealed class ContentValidationException : InvalidOperationException
    {
        public string Code { get; }
        public string DefinitionId { get; }
        public ContentValidationException(string code, string definitionId) : base(code + ": " + definitionId)
        { Code = code; DefinitionId = definitionId; }
    }
    public sealed class ContentDependency
    {
        public string PackId { get; }
        public int Version { get; }
        public string ContentHash { get; }
        public ContentDependency(string packId, int version, string contentHash)
        { PackId = packId; Version = version; ContentHash = contentHash; }
    }
    public sealed class ContentDefinitionInput
    {
        public string DefinitionId { get; }
        public int Version { get; }
        public string Kind { get; }
        public int SchemaVersion { get; }
        public string Source { get; }
        public string Classification { get; }
        public string OwnerDomain { get; }
        public string DisplayKey { get; }
        public string? Unit { get; }
        public string? CommodityId { get; }
        public string? Handler { get; }
        public long? Quantity { get; }
        public ContentDefinitionInput(string definitionId, int version, string kind, int schemaVersion, string source,
            string classification, string ownerDomain, string displayKey, string? unit = null, string? commodityId = null,
            string? handler = null, long? quantity = null)
        { DefinitionId = definitionId; Version = version; Kind = kind; SchemaVersion = schemaVersion; Source = source;
          Classification = classification; OwnerDomain = ownerDomain; DisplayKey = displayKey; Unit = unit;
          CommodityId = commodityId; Handler = handler; Quantity = quantity; }
    }
    public sealed class ContentPackInput
    {
        public string PackId { get; }
        public int Version { get; }
        public int SchemaVersion { get; }
        public string RuleVersion { get; }
        public IReadOnlyList<ContentDependency> Dependencies { get; }
        public IReadOnlyList<ContentDefinitionInput> Definitions { get; }
        public ContentPackInput(string packId, int version, int schemaVersion, string ruleVersion,
            IEnumerable<ContentDependency> dependencies, IEnumerable<ContentDefinitionInput> definitions)
        { PackId = packId; Version = version; SchemaVersion = schemaVersion; RuleVersion = ruleVersion;
          Dependencies = Array.AsReadOnly((dependencies ?? throw new ArgumentNullException(nameof(dependencies))).ToArray());
          Definitions = Array.AsReadOnly((definitions ?? throw new ArgumentNullException(nameof(definitions))).ToArray()); }
    }
    /// <summary>Immutable compiler output. A registry can only be constructed by the validated compiler.</summary>
    public sealed class ContentRegistry
    {
        private readonly Dictionary<string, ContentDefinitionInput> resolved;
        internal IReadOnlyDictionary<string, ContentRegistry> Closure { get; }
        public string PackId { get; }
        public int Version { get; }
        public int SchemaVersion => ContentCompiler.SchemaVersion;
        public string RuleVersion => ContentCompiler.RuleVersion;
        public string ContentHash { get; }
        public IReadOnlyList<ContentDefinitionInput> Definitions { get; }
        public IReadOnlyList<ContentDependency> Dependencies { get; }
        public IReadOnlyList<string> DefinitionIds => Array.AsReadOnly(Definitions.Select(x => x.DefinitionId).ToArray());
        public IReadOnlyList<string> RuleReferences => Array.AsReadOnly(Definitions.Where(x => x.Handler != null)
            .Select(x => x.Handler!).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray());
        public string ValidationProfile => "kernel-content-v1";
        public RuleRegistry CreateRuleRegistry() => new RuleRegistry(new IRuleHandler[] { new TransferResourceRule() });
        internal ContentRegistry(ContentPackInput input, string hash, Dictionary<string, ContentDefinitionInput> resolved,
            Dictionary<string, ContentRegistry> closure)
        { PackId = input.PackId; Version = input.Version; ContentHash = hash;
          Definitions = Array.AsReadOnly(input.Definitions.OrderBy(x => x.DefinitionId, StringComparer.Ordinal).ToArray());
          Dependencies = Array.AsReadOnly(input.Dependencies.OrderBy(x => x.PackId, StringComparer.Ordinal).ToArray());
          this.resolved = new Dictionary<string, ContentDefinitionInput>(resolved, StringComparer.Ordinal);
          Closure = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ContentRegistry>(closure); }
        public ContentDefinitionInput Resolve(string definitionId)
        {
            if (definitionId == null || !resolved.TryGetValue(definitionId, out var definition))
                throw new ContentValidationException("CONTENT_REFERENCE_MISSING", definitionId ?? "<null>");
            return definition;
        }
        public void EnsureCompatible(string packId, int version, string contentHash)
        {
            if (packId != PackId || version != Version || contentHash != ContentHash)
                throw new ContentValidationException("CONTENT_INCOMPATIBLE", packId ?? "<null>");
        }
        public void ValidateWorld(WorldSnapshot world)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            foreach (var lot in world.Lots)
                if (Resolve(lot.Commodity.ToString()).Kind != "commodity")
                    throw new ContentValidationException("CONTENT_REFERENCE_TYPE", lot.Commodity.ToString());
        }
        public TransferResource CreateTransfer(string templateId, WorldSnapshot world, EntityId<Lot> sourceId, EntityId<Lot> destinationId)
        {
            var template = Resolve(templateId);
            if (template.Kind != "transfer-template") throw new ContentValidationException("CONTENT_REFERENCE_TYPE", templateId);
            ValidateWorld(world);
            if (world.GetLot(sourceId).Commodity.ToString() != template.CommodityId ||
                world.GetLot(destinationId).Commodity.ToString() != template.CommodityId)
                throw new ContentValidationException("CONTENT_TEMPLATE_COMMODITY", templateId);
            return new TransferResource(sourceId, destinationId, template.Quantity!.Value);
        }
    }
    public static class ContentCompiler
    {
        public const int SchemaVersion = 1;
        public const string RuleVersion = "transfer-resource-v1+scheduler-v1";
        public const string Handler = "TransferResource@1";
        private static void Require(bool condition, string code, string id)
        { if (!condition) throw new ContentValidationException(code, id ?? "<null>"); }
        private static bool Token(string? value) => value != null && Regex.IsMatch(value, @"\A[a-z][a-z0-9.-]{0,127}\z");
        private static bool Text(string? value) => !string.IsNullOrWhiteSpace(value) && value!.Length <= 1024 &&
            !value.Any(char.IsControl) && !value.Any(char.IsSurrogate);
        public static ContentRegistry Compile(ContentPackInput input, IEnumerable<ContentRegistry>? availableDependencies = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var id = input.PackId;
            Require(Token(id) && input.Version > 0, "CONTENT_PACK_INVALID", id);
            Require(input.SchemaVersion == SchemaVersion, "CONTENT_SCHEMA_UNSUPPORTED", id);
            Require(input.RuleVersion == RuleVersion, "CONTENT_RULE_INCOMPATIBLE", id);
            Require(input.Definitions.Count > 0 && input.Definitions.Count <= 4096 && input.Dependencies.Count <= 64,
                "CONTENT_LIMIT", id);
            var available = new Dictionary<string, ContentRegistry>(StringComparer.Ordinal);
            foreach (var registry in availableDependencies ?? Array.Empty<ContentRegistry>())
            {
                Require(registry != null, "CONTENT_DEPENDENCY_INVALID", id);
                Require(!available.ContainsKey(registry!.PackId), "CONTENT_DEPENDENCY_DUPLICATE", registry.PackId);
                available.Add(registry.PackId, registry);
            }
            var closure = new Dictionary<string, ContentRegistry>(StringComparer.Ordinal);
            var direct = new HashSet<string>(StringComparer.Ordinal);
            foreach (var dep in input.Dependencies)
            {
                Require(dep != null, "CONTENT_DEPENDENCY_INVALID", id);
                Require(Token(dep!.PackId) && dep.Version > 0 && dep.ContentHash != null &&
                    Regex.IsMatch(dep.ContentHash, @"\A[0-9a-f]{64}\z"), "CONTENT_DEPENDENCY_INVALID", dep.PackId);
                Require(direct.Add(dep.PackId), "CONTENT_DEPENDENCY_DUPLICATE", dep.PackId);
                Require(dep.PackId != id, "CONTENT_DEPENDENCY_CYCLE", dep.PackId);
                Require(available.TryGetValue(dep.PackId, out var registry), "CONTENT_DEPENDENCY_MISSING", dep.PackId);
                registry!.EnsureCompatible(dep.PackId, dep.Version, dep.ContentHash!);
                foreach (var item in registry.Closure.Values.Concat(new[] { registry }))
                {
                    Require(item.PackId != id, "CONTENT_DEPENDENCY_CYCLE", item.PackId);
                    if (closure.TryGetValue(item.PackId, out var existing))
                        Require(existing.ContentHash == item.ContentHash && existing.Version == item.Version,
                            "CONTENT_DEPENDENCY_CONFLICT", item.PackId);
                    else closure.Add(item.PackId, item);
                }
            }
            Require(closure.Count <= 256, "CONTENT_LIMIT", id);
            var definitions = new Dictionary<string, ContentDefinitionInput>(StringComparer.Ordinal);
            foreach (var registry in closure.Values.OrderBy(x => x.PackId, StringComparer.Ordinal))
                foreach (var definition in registry.Definitions)
                {
                    Require(!definitions.ContainsKey(definition.DefinitionId), "CONTENT_DEFINITION_DUPLICATE", definition.DefinitionId);
                    definitions.Add(definition.DefinitionId, definition);
                }
            foreach (var definition in input.Definitions)
            {
                Require(definition != null, "CONTENT_DEFINITION_INVALID", id);
                Validate(definition!);
                Require(!definitions.ContainsKey(definition!.DefinitionId), "CONTENT_DEFINITION_DUPLICATE", definition.DefinitionId);
                definitions.Add(definition.DefinitionId, definition);
            }
            foreach (var definition in input.Definitions.Where(x => x.Kind == "transfer-template"))
            {
                Require(definitions.TryGetValue(definition.CommodityId!, out var commodity), "CONTENT_REFERENCE_MISSING", definition.DefinitionId);
                Require(commodity!.Kind == "commodity", "CONTENT_REFERENCE_TYPE", definition.DefinitionId);
            }
            return new ContentRegistry(input, Hash(input), definitions, closure);
        }
        private static void Validate(ContentDefinitionInput d)
        {
            var id = d.DefinitionId ?? throw new ContentValidationException("CONTENT_DEFINITION_INVALID", "<null>");
            Require(id.Length <= 256, "CONTENT_DEFINITION_INVALID", id);
            try { Game.Foundation.Identity.DefinitionId.Parse(id); }
            catch (FormatException) { throw new ContentValidationException("CONTENT_DEFINITION_INVALID", id); }
            Require(d.Version > 0 && id.EndsWith("@" + d.Version.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal),
                "CONTENT_DEFINITION_VERSION", id);
            Require(d.SchemaVersion == SchemaVersion, "CONTENT_SCHEMA_UNSUPPORTED", id);
            Require(Text(d.Source) && Token(d.DisplayKey), "CONTENT_PROVENANCE_INVALID", id);
            Require(new[] { "historical", "traditional", "game-canon" }.Contains(d.Classification),
                "CONTENT_CLASSIFICATION_INVALID", id);
            Require(d.OwnerDomain == "Assets", "CONTENT_OWNER_UNSUPPORTED", id);
            Require(d.Kind == "commodity" || d.Kind == "transfer-template", "CONTENT_KIND_UNSUPPORTED", id);
            if (d.Kind == "commodity")
                Require(id.StartsWith("commodity.", StringComparison.Ordinal) && d.Unit == "count" && d.CommodityId == null &&
                    d.Handler == null && d.Quantity == null, "CONTENT_COMMODITY_INVALID", id);
            else
            {
                var approved = new TransferResourceRule();
                Require(d.Handler == approved.CommandType + "@" + approved.PayloadVersion.ToString(CultureInfo.InvariantCulture),
                    "CONTENT_HANDLER_UNAPPROVED", id);
                Require(id.StartsWith("transfer.", StringComparison.Ordinal) && d.Unit == null && d.CommodityId != null &&
                    d.Quantity.HasValue && d.Quantity.Value > 0, "CONTENT_TEMPLATE_INVALID", id);
            }
        }
        // Length-prefixed UTF-8 fields, decimal invariant integers, ordinal-sorted arrays; binds provenance and dependency hashes.
        private static string Hash(ContentPackInput input)
        {
            var b = new StringBuilder();
            void Add(string? value)
            { if (value == null) { b.Append("-1:"); return; }
              b.Append(Encoding.UTF8.GetByteCount(value).ToString(CultureInfo.InvariantCulture)).Append(':').Append(value); }
            void Number(long value) => Add(value.ToString(CultureInfo.InvariantCulture));
            Add("mythical-content-v1"); Add(input.PackId); Number(input.Version); Number(input.SchemaVersion); Add(input.RuleVersion);
            Number(input.Dependencies.Count);
            foreach (var d in input.Dependencies.OrderBy(x => x.PackId, StringComparer.Ordinal))
            { Add(d.PackId); Number(d.Version); Add(d.ContentHash); }
            Number(input.Definitions.Count);
            foreach (var d in input.Definitions.OrderBy(x => x.DefinitionId, StringComparer.Ordinal))
            { Add(d.DefinitionId); Number(d.Version); Add(d.Kind); Number(d.SchemaVersion); Add(d.Source); Add(d.Classification);
              Add(d.OwnerDomain); Add(d.DisplayKey); Add(d.Unit); Add(d.CommodityId); Add(d.Handler);
              Add(d.Quantity?.ToString(CultureInfo.InvariantCulture)); }
            using var sha = SHA256.Create();
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(b.ToString())).Select(x => x.ToString("x2", CultureInfo.InvariantCulture)));
        }
    }
}
