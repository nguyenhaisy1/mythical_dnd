using System;
using System.Numerics;

namespace Game.Foundation.Random
{
    public sealed class NumericOverflowException : OverflowException
    {
        public string Code => "NUMERIC_OVERFLOW";
        public NumericOverflowException() : base("NUMERIC_OVERFLOW") { }
    }

    public static class DeterministicMath
    {
        public static long RoundDiv(BigInteger numerator, BigInteger denominator)
        {
            if (denominator <= 0) throw new ArgumentOutOfRangeException(nameof(denominator));
            var quotient = BigInteger.DivRem(BigInteger.Abs(numerator), denominator, out var remainder);
            if (remainder * 2 >= denominator) quotient++;
            if (numerator.Sign < 0) quotient = -quotient;
            if (quotient < long.MinValue || quotient > long.MaxValue) throw new NumericOverflowException();
            return (long)quotient;
        }
        public static long MulFixed(long left, long right) => RoundDiv((BigInteger)left * right, 1000);
    }
}
