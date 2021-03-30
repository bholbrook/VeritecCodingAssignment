using System;
using VeritecCodingAssignment.Models;

namespace VeritecCodingAssignment.Services
{
    public class TaxService : ITaxService
    {
        private const int NumberOfWeeksInYear = 52;
        private const int NumberOfFortnightsInYear = 26;
        private const int NumberOfMonthsInYear = 12;

        private const decimal SuperannuationPercentage = 9.5M;

        public decimal AnnualTaxableIncome(decimal annualGrossPackage)
        {
            var unroundedTaxableIncome = annualGrossPackage / (1 + SuperannuationPercentage * 0.01M);
            return RoundTo2Dp(unroundedTaxableIncome);
        }

        public decimal AnnualSuperannuationContribution(decimal annualGrossPackage)
        {
            var taxableIncome = AnnualTaxableIncome(annualGrossPackage);

            var unroundedSuperannuation = annualGrossPackage - taxableIncome;
            return RoundTo2Dp(unroundedSuperannuation);
        }

        public decimal AnnualNetIncome(decimal annualGrossPackage, decimal annualSuperannuationContribution, decimal deductions)
        {
            return annualGrossPackage - annualSuperannuationContribution - deductions;
        }
        
        public decimal Deductions(decimal annualTaxableIncome)
        {
            return MedicareLevy(annualTaxableIncome) + BudgetRepairLevy(annualTaxableIncome) + IncomeTax(annualTaxableIncome);
        }
        
        public decimal PayPacketAmount(decimal annualNetIncome, Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.Weekly:
                    return annualNetIncome / NumberOfWeeksInYear;
                case Frequency.Fortnightly:
                    return annualNetIncome / NumberOfFortnightsInYear;
                case Frequency.Monthly:
                    return annualNetIncome / NumberOfMonthsInYear;
                default:
                    throw new ArgumentException(nameof(frequency));
            }
        }

        public decimal MedicareLevy(decimal annualTaxableIncome)
        {
            var roundedTaxableIncome = RoundDownToNearestDollar(annualTaxableIncome);

            switch (roundedTaxableIncome)
            {
                case <= 21335:
                    return 0;
                case <= 26668:
                    {
                        var excessIncome = roundedTaxableIncome - 21335;
                        var unroundedMedicareLevy = excessIncome * 0.1M;
                        return RoundUpToNearestDollar(unroundedMedicareLevy);
                    }
                default:
                    {
                        var unroundedMedicareLevy = roundedTaxableIncome * 0.02M;
                        return RoundUpToNearestDollar(unroundedMedicareLevy);
                    }
            }
        }

        public decimal BudgetRepairLevy(decimal annualTaxableIncome)
        {
            var roundedTaxableIncome = RoundDownToNearestDollar(annualTaxableIncome);

            switch (roundedTaxableIncome)
            {
                case <= 180000:
                    return 0;
                default:
                    var excessTaxableIncome = roundedTaxableIncome - 180000;
                    var budgetRepairLevy = excessTaxableIncome * 0.02M;
                    return budgetRepairLevy;
            }
        }

        public decimal IncomeTax(decimal annualTaxableIncome)
        {
            var roundedTaxableIncome = RoundDownToNearestDollar(annualTaxableIncome);

            decimal excess;
            switch (roundedTaxableIncome)
            {
                case <= 18200:
                    return 0;
                case <= 37000:
                    excess = CalculateExcess(roundedTaxableIncome, 19, 18200);
                    break;
                case <= 87000:
                    excess = 3572 + CalculateExcess(roundedTaxableIncome, 32.5M, 37000);
                    break;
                case <= 180000:
                    excess = 19822 + CalculateExcess(roundedTaxableIncome, 37, 87000);
                    break;
                default:
                    excess = 54232 + CalculateExcess(roundedTaxableIncome, 47, 180000);
                    break;
            }

            return RoundUpToNearestDollar(excess);

            static decimal CalculateExcess(decimal taxableIncome, decimal taxPercentage, decimal excessThreshold)
            {
                var excessIncome = taxableIncome - excessThreshold;
                var excess = excessIncome * (taxPercentage * 0.01M);
                return Math.Round(excess, 0, MidpointRounding.ToPositiveInfinity);
            }
        }

        private decimal RoundUpToNearestDollar(decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.ToPositiveInfinity);
        }

        private decimal RoundDownToNearestDollar(decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.ToNegativeInfinity);
        }

        private decimal RoundTo2Dp(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.ToNegativeInfinity);
        }
    }
}
