using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Game.Foundation.Identity;

namespace Game.Foundation.Random
{
    public sealed class RngContext
    {
        public const string Algorithm = "sha256-counter-v1";
        public string Seed { get; }
        public string Domain { get; }
        public string Decision { get; }
        public string Occurrence { get; }
        public EntityId Entity { get; }
        public string Stream { get; }
        public RngContext(string seed, string domain, string decision, string occurrence, EntityId entity, string stream)
        {
            if (seed == null || !Regex.IsMatch(seed, @"\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant))
                throw new ArgumentException("Seed must be 64 lowercase hex digits", nameof(seed));
            Seed = seed; Domain = Identifier(domain); Decision = Identifier(decision);
            Occurrence = Identifier(occurrence); Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Stream = Identifier(stream);
        }
        private static string Identifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Empty RNG context identifier");
            _ = new UTF8Encoding(false, true).GetByteCount(value);
            return value;
        }
        public uint[] Block(ulong index)
        {
            var fields = new Dictionary<string, object?>
            {
                ["algorithm"] = Algorithm, ["seed"] = Seed, ["domain"] = Domain,
                ["decision"] = Decision, ["occurrence"] = Occurrence, ["entity"] = Entity.ToString(),
                ["stream"] = Stream, ["block"] = index
            };
            using var sha = SHA256.Create();
            var digest = sha.ComputeHash(CanonicalCodec.Encode(fields));
            var words = new uint[8];
            for (var i = 0; i < words.Length; i++)
            {
                var offset = i * 4;
                words[i] = (uint)digest[offset] | ((uint)digest[offset + 1] << 8)
                    | ((uint)digest[offset + 2] << 16) | ((uint)digest[offset + 3] << 24);
            }
            return words;
        }
    }

    public readonly struct RngResult<T>
    {
        public T Value { get; }
        public RngState Next { get; }
        public RngResult(T value, RngState next) { Value = value; Next = next; }
    }

    /// <summary>Immutable cursor. Only an Application commit may persist Next; previews discard it.</summary>
    public sealed class RngState
    {
        public RngContext Context { get; }
        public ulong DrawIndex { get; }
        public RngState(RngContext context, ulong drawIndex = 0)
        { Context = context ?? throw new ArgumentNullException(nameof(context)); DrawIndex = drawIndex; }
        public RngResult<uint> Draw()
        {
            // Never wrap a persisted cursor; reject before producing an uncommittable draw.
            if (DrawIndex == ulong.MaxValue) throw new NumericOverflowException();
            return new RngResult<uint>(Context.Block(DrawIndex / 8)[(int)(DrawIndex % 8)], new RngState(Context, DrawIndex + 1));
        }
        public RngResult<ulong> Uniform(ulong bound)
        {
            UniformSampling.Validate(bound);
            var state = this;
            while (true)
            {
                var draw = state.Draw(); state = draw.Next;
                if (UniformSampling.TryMap(draw.Value, bound, out var value)) return new RngResult<ulong>(value, state);
            }
        }
        public RngResult<int> Roll2d10()
        {
            var first = Uniform(10); var second = first.Next.Uniform(10);
            return new RngResult<int>((int)(first.Value + second.Value) + 2, second.Next);
        }
        public RngResult<bool> Probability(int probability)
        {
            if (probability < 0 || probability > 10000) throw new ArgumentOutOfRangeException(nameof(probability));
            if (probability == 0 || probability == 10000) return new RngResult<bool>(probability == 10000, this);
            var result = Uniform(10000);
            return new RngResult<bool>(result.Value < (ulong)probability, result.Next);
        }
    }

    public static class UniformSampling
    {
        private const ulong WordRange = 1UL << 32;
        internal static void Validate(ulong bound)
        { if (bound == 0 || bound > WordRange) throw new ArgumentOutOfRangeException(nameof(bound)); }
        public static bool TryMap(uint word, ulong bound, out ulong value)
        {
            Validate(bound);
            if (word >= WordRange - WordRange % bound) { value = 0; return false; }
            value = word % bound; return true;
        }
    }
}
