using System.Collections.Generic;
using FluentAssertions;
using VeritecCodingAssignment.Extensions;
using Xunit;

namespace VeritecCodingAssignment.UnitTests.Extensions
{
    public class CurrencyExtensionTests
    {
        public static IEnumerable<object[]> RoundUpToNearestDollarShouldRoundUpData =>
            new List<object[]>
            {
                new object[] { 0.5M, 1M },
                new object[] { 1M, 1M },
                new object[] { 100.49M, 101M },
                new object[] { 100.5M, 101M },
                new object[] { -0.5M, 0M },
                new object[] { -1M, -1M },
                new object[] { -100.49M, -100M }
            };

        public static IEnumerable<object[]> RoundDownToNearestDollarShouldRoundDownData =>
            new List<object[]>
            {
                new object[] { 0.5M, 0M },
                new object[] { 1M, 1M },
                new object[] { 100.49M, 100M },
                new object[] { 100.5M, 100M },
                new object[] { -0.5M, -1M },
                new object[] { -1M, -1M },
                new object[] { -100.49M, -101M }
            };

        public static IEnumerable<object[]> RoundToDollarAndCentsShouldRoundToCents =>
            new List<object[]>
            {
                new object[] { 0M, 0.00M },
                new object[] { 1.555M, 1.56M },
                new object[] { -1.555M, -1.55M }
            };

        [Theory]
        [MemberData(nameof(RoundUpToNearestDollarShouldRoundUpData))]

        public void RoundUpToNearestDollar_ShouldRoundUp(decimal value, decimal expectedValue)
        {
            var roundedValue = value.RoundUpToNearestDollar();
            roundedValue.Should().Be(expectedValue);
        }

        [Theory]
        [MemberData(nameof(RoundDownToNearestDollarShouldRoundDownData))]

        public void RoundDownToNearestDollar_ShouldRoundDown(decimal value, decimal expectedValue)
        {
            var roundedValue = value.RoundDownToNearestDollar();
            roundedValue.Should().Be(expectedValue);
        }

        [Theory]
        [MemberData(nameof(RoundToDollarAndCentsShouldRoundToCents))]

        public void RoundToDollarAndCents_ShouldRoundToCents(decimal value, decimal expectedValue)
        {
            var roundedValue = value.RoundToDollarAndCents();
            roundedValue.Should().Be(expectedValue);
        }
    }
}
