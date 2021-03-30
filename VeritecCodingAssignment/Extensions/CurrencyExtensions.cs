using System;

namespace VeritecCodingAssignment.Extensions
{
    public static class CurrencyExtensions
    {
        public static decimal RoundUpToNearestDollar(this decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.ToPositiveInfinity);
        }

        public static decimal RoundDownToNearestDollar(this decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.ToNegativeInfinity);
        }

        public static decimal RoundToDollarAndCents(this decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.ToPositiveInfinity);
        }
    }
}
