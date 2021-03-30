using System;
using VeritecCodingAssignment.Extensions;

namespace VeritecCodingAssignment.Models
{
    public class SalaryPackage
    {
        private const int NumberOfWeeksInYear = 52;
        private const int NumberOfFortnightsInYear = 26;
        private const int NumberOfMonthsInYear = 12;

        private const decimal SuperannuationPercentage = 9.5M;

        public SalaryPackage(decimal annualGrossPackage)
        {
            GrossPackage = annualGrossPackage;
        }

        public decimal GrossPackage { get; }

        public decimal TaxableIncome
        {
            get
            {
                var unroundedTaxableIncome = GrossPackage / (1 + SuperannuationPercentage * 0.01M);
                return unroundedTaxableIncome.RoundToDollarAndCents();
            }
        }

        public decimal SuperannuationContribution
        {
            get
            {
                var unroundedSuperannuation = GrossPackage - TaxableIncome;
                return unroundedSuperannuation.RoundToDollarAndCents();
            }
        }

        public decimal NetIncome
        {
            get
            {
                return GrossPackage - SuperannuationContribution - Deductions;
            }
        }

        public decimal Deductions
        {
            get
            {
                return MedicareLevy + BudgetRepairLevy + IncomeTax;
            }
        }

        public decimal PayPacket(FrequencyType frequency)
        {
            return frequency switch
            {
                FrequencyType.Weekly => NetIncome / NumberOfWeeksInYear,
                FrequencyType.Fortnightly => NetIncome / NumberOfFortnightsInYear,
                FrequencyType.Monthly => NetIncome / NumberOfMonthsInYear,
                _ => throw new ArgumentException(nameof(frequency)),
            };
        }

        public decimal MedicareLevy
        {
            get
            {
                var roundedTaxableIncome = TaxableIncome.RoundDownToNearestDollar();

                switch (roundedTaxableIncome)
                {
                    case <= 21335:
                        return 0;
                    case <= 26668:
                    {
                        var excessIncome = roundedTaxableIncome - 21335;
                        var unroundedMedicareLevy = excessIncome * 0.1M;
                        return unroundedMedicareLevy.RoundUpToNearestDollar();
                    }
                    default:
                    {
                        var unroundedMedicareLevy = roundedTaxableIncome * 0.02M;
                        return unroundedMedicareLevy.RoundUpToNearestDollar();
                    }
                }
            }
        }

        public decimal BudgetRepairLevy
        {
            get
            {
                var roundedTaxableIncome = TaxableIncome.RoundDownToNearestDollar();

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
        }

        public decimal IncomeTax
        {
            get
            {
                var roundedTaxableIncome = TaxableIncome.RoundDownToNearestDollar();

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

                return excess.RoundUpToNearestDollar();

                static decimal CalculateExcess(decimal taxableIncome, decimal taxPercentage, decimal excessThreshold)
                {
                    var excessIncome = taxableIncome - excessThreshold;
                    var excess = excessIncome * (taxPercentage * 0.01M);
                    return Math.Round(excess, 0, MidpointRounding.ToPositiveInfinity);
                }
            }
        }
    }
}
