using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class RandomTests
    {
        private static RngContext Context(string stream = "resolution") => new RngContext(new string('0', 64), "foundation", "fixture", "command_1",
            EntityId.Parse("w0000000000000001/actor/0000000000000001"), stream);
        private static object? Value(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Null: return null;
                case JsonValueKind.True: return true;
                case JsonValueKind.False: return false;
                case JsonValueKind.String: return value.GetString();
                case JsonValueKind.Number: return value.TryGetInt64(out var signed) ? (object)signed : value.GetUInt64();
                case JsonValueKind.Array: return value.EnumerateArray().Select(Value).ToArray();
                case JsonValueKind.Object: return value.EnumerateObject().Select(p => new KeyValuePair<string, object?>(p.Name, Value(p.Value))).ToArray();
                default: throw new InvalidOperationException();
            }
        }
        [Test]
        public void TEST_007_CommittedCodecAndRngGoldenVectors()
        {
            using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "vectors.json")));
            foreach (var vector in json.RootElement.GetProperty("codec").EnumerateArray())
            {
                var value = Value(vector.GetProperty("value"));
                Assert.That(Convert.ToHexString(CanonicalCodec.Encode(value)).ToLowerInvariant(), Is.EqualTo(vector.GetProperty("hex").GetString()));
                Assert.That(CanonicalCodec.Hash(value), Is.EqualTo(vector.GetProperty("sha256").GetString()));
            }
            foreach (var vector in json.RootElement.GetProperty("rng").EnumerateArray())
            {
                var c = vector.GetProperty("context");
                Assert.That(c.GetProperty("algorithm").GetString(), Is.EqualTo(RngContext.Algorithm));
                var context = new RngContext(c.GetProperty("seed").GetString()!, c.GetProperty("domain").GetString()!,
                    c.GetProperty("decision").GetString()!, c.GetProperty("occurrence").GetString()!,
                    EntityId.Parse(c.GetProperty("entity").GetString()!), c.GetProperty("stream").GetString()!);
                Assert.That(context.Block(vector.GetProperty("block").GetUInt64()),
                    Is.EqualTo(vector.GetProperty("words").EnumerateArray().Select(v => v.GetUInt32()).ToArray()));
            }
        }
        [Test]
        public void TEST_007_CodecOrderingCultureUnicodeAndIntegerLimits()
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-EG");
                Assert.That(Encoding.UTF8.GetString(CanonicalCodec.Encode(new Dictionary<string, object?> { ["b"] = "x", ["a"] = 1 })),
                    Is.EqualTo("MDC1o2:s1:ai1;s1:bs1:x"));
                Assert.That(Encoding.UTF8.GetString(CanonicalCodec.Encode(new object?[] { long.MinValue, ulong.MaxValue })),
                    Is.EqualTo("MDC1a2:i-9223372036854775808;i18446744073709551615;"));
            }
            finally { CultureInfo.CurrentCulture = previous; }
            Assert.That(CanonicalCodec.Encode("é"), Is.Not.EqualTo(CanonicalCodec.Encode("e\u0301")));
            Assert.That(Encoding.UTF8.GetString(CanonicalCodec.Encode("😀")), Is.EqualTo("MDC1s4:😀"));
            Assert.That(CanonicalCodec.Encode(new object?[] { 1, 2 }), Is.Not.EqualTo(CanonicalCodec.Encode(new object?[] { 2, 1 })));
        }
        [Test]
        public void TEST_007_CodecRejectsAmbiguityAndUnsupportedTypes()
        {
            foreach (var invalid in new object[] { 1.5, float.NaN, double.PositiveInfinity, 1m, BigInteger.One, new object(), '\u0061' })
                Assert.Throws<ArgumentException>(() => CanonicalCodec.Encode(invalid));
            foreach (var key in new[] { "Bad", "x-y", "", "x\n", "é" })
                Assert.Throws<ArgumentException>(() => CanonicalCodec.Encode(new Dictionary<string, object?> { [key] = 1 }));
            Assert.Throws<ArgumentException>(() => CanonicalCodec.Encode(new[] { new KeyValuePair<string, object?>("a", 1), new KeyValuePair<string, object?>("a", 2) }));
            Assert.Throws<EncoderFallbackException>(() => CanonicalCodec.Encode("\ud800"));
            Assert.Throws<EncoderFallbackException>(() => CanonicalCodec.Encode("\udc00"));
        }
        [TestCase(1500, 2)] [TestCase(-1500, -2)] [TestCase(1499, 1)] [TestCase(-1499, -1)] [TestCase(0, 0)]
        public void TEST_007_RoundingTiesAwayFromZero(long value, long expected) => Assert.That(DeterministicMath.RoundDiv(value, 1000), Is.EqualTo(expected));
        [Test]
        public void TEST_007_ExactWideIntermediatesAndOverflow()
        {
            Assert.That(DeterministicMath.RoundDiv(long.MinValue, 1), Is.EqualTo(long.MinValue));
            Assert.That(DeterministicMath.RoundDiv(long.MaxValue, 1), Is.EqualTo(long.MaxValue));
            Assert.That(DeterministicMath.MulFixed(long.MinValue, 1000), Is.EqualTo(long.MinValue));
            Assert.That(DeterministicMath.MulFixed(long.MaxValue, 1000), Is.EqualTo(long.MaxValue));
            Assert.That(DeterministicMath.MulFixed(-3, 500), Is.EqualTo(-2));
            Assert.That(Assert.Throws<NumericOverflowException>(() => DeterministicMath.MulFixed(long.MaxValue, 1001))!.Code, Is.EqualTo("NUMERIC_OVERFLOW"));
            Assert.Throws<NumericOverflowException>(() => DeterministicMath.MulFixed(long.MinValue, -1000));
            Assert.Throws<NumericOverflowException>(() => DeterministicMath.RoundDiv((BigInteger)long.MinValue - 1, 1));
            Assert.Throws<NumericOverflowException>(() => DeterministicMath.RoundDiv((BigInteger)long.MaxValue * 2 + 1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => DeterministicMath.RoundDiv(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => DeterministicMath.RoundDiv(1, -1));
        }
        [Test]
        public void TEST_007_RejectionSamplingConsumesRejectedWords()
        {
            Assert.That(UniformSampling.TryMap(uint.MaxValue, 10, out _), Is.False);
            Assert.That(UniformSampling.TryMap(17, 10, out var mapped), Is.True);
            Assert.That(mapped, Is.EqualTo(7));
            Assert.That(UniformSampling.TryMap(uint.MaxValue, 1UL << 32, out mapped), Is.True);
            Assert.That(mapped, Is.EqualTo(uint.MaxValue));
            // Literal golden block starts 3506563779 (rejected), 778542440 (accepted).
            var original = new RngState(Context());
            var result = original.Uniform(2147483649);
            Assert.That(result.Value, Is.EqualTo(778542440));
            Assert.That(result.Next.DrawIndex, Is.EqualTo(2));
            Assert.That(original.DrawIndex, Is.Zero);
            Assert.That(original.Uniform(1).Next.DrawIndex, Is.EqualTo(1));
        }
        [Test]
        public void TEST_007_ImmutablePreviewAbortRetryAndRestore()
        {
            var committed = new RngState(Context(), 7);
            var preview = committed.Draw();
            Assert.That(preview.Value, Is.EqualTo(2995364543U));
            Assert.That(preview.Next.Draw().Value, Is.EqualTo(15239080U));
            Assert.That(committed.DrawIndex, Is.EqualTo(7));
            Assert.That(committed.Draw().Value, Is.EqualTo(preview.Value));
            Assert.That(new RngState(Context(), preview.Next.DrawIndex).Draw().Value, Is.EqualTo(preview.Next.Draw().Value));
            Assert.That(Context("preview").Block(0), Is.Not.EqualTo(Context().Block(0)));
            Assert.That(committed.Context, Is.SameAs(preview.Next.Context));
        }
        [Test]
        public void TEST_007_DiceAndProbabilityDrawSchedule()
        {
            var state = new RngState(Context());
            var dice = state.Roll2d10();
            Assert.That(dice.Value, Is.EqualTo(11));
            Assert.That(dice.Next.DrawIndex, Is.EqualTo(2));
            Assert.That(state.Probability(0).Value, Is.False);
            Assert.That(state.Probability(10000).Value, Is.True);
            Assert.That(state.Probability(10000).Next, Is.SameAs(state));
            Assert.That(state.Probability(3779).Value, Is.False);
            Assert.That(state.Probability(3780).Value, Is.True);
            Assert.That(state.Probability(3780).Next.DrawIndex, Is.EqualTo(1));
        }
        [Test]
        public void TEST_007_InvalidInputsAndExhaustionNeverWrapOrConsume()
        {
            var exhausted = new RngState(Context(), ulong.MaxValue);
            Assert.Throws<NumericOverflowException>(() => exhausted.Draw());
            Assert.Throws<NumericOverflowException>(() => exhausted.Uniform(10));
            Assert.That(exhausted.Probability(10000).Value, Is.True);
            Assert.That(exhausted.DrawIndex, Is.EqualTo(ulong.MaxValue));
            Assert.That(new RngState(Context(), ulong.MaxValue - 1).Draw().Next.DrawIndex, Is.EqualTo(ulong.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => exhausted.Uniform(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => exhausted.Uniform((1UL << 32) + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => exhausted.Probability(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => exhausted.Probability(10001));
            Assert.Throws<ArgumentException>(() => new RngContext(new string('A', 64), "a", "a", "a", Context().Entity, "a"));
            Assert.Throws<ArgumentException>(() => new RngContext(new string('0', 64), "", "a", "a", Context().Entity, "a"));
            Assert.Throws<EncoderFallbackException>(() => Context("\ud800"));
        }
    }
}
