using FluentAssertions;
using VeritecCodingAssignment.Services;
using Xunit;

namespace VeritecCodingAssignment.UnitTests.Services
{
    // TODO: Add more tests
    public class TaxServiceTests
    {
        //[Theory]
        //[InlineData(50000M, 10M)]
        //public void TaxService_Should(decimal annualGrossPackage, decimal expectedTaxableIncome)

        [Fact]
        public void AnnualSuperannuationContribution_ShouldBeValid()
        {
            const decimal annualGrossPackage = 50000;
            const decimal expectedSuperannuationContribution = 4338;

            var taxService = new TaxService();

            var superannuationContribution = taxService.AnnualSuperannuationContribution(annualGrossPackage);
            superannuationContribution.Should().Be(expectedSuperannuationContribution);
        }

        [Fact]
        public void AnnualTaxableIncome_ShouldBeValid()
        {
            const decimal annualGrossPackage = 50000;
            const decimal expectedTaxableIncome = 45662;

            var taxService = new TaxService();

            var taxableIncome = taxService.AnnualTaxableIncome(annualGrossPackage);
            taxableIncome.Should().Be(expectedTaxableIncome);
        }
    }
}
