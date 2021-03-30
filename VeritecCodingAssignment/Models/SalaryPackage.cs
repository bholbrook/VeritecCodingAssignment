using System;

namespace VeritecCodingAssignment.Models
{
    public class SalaryPackage
    {
        private const int NumberOfWeeksInYear = 52;
        private const int NumberOfFortnightsInYear = NumberOfWeeksInYear / 2;

        private const decimal SuperannuationPercentage = 9.5M;

        public SalaryPackage(decimal annualGrossPackage)
        {
            AnnualGrossPackage = annualGrossPackage;
        }

        public decimal AnnualGrossPackage { get; }

        public decimal AnnualSuperannuationContribution
        {
            get
            {
                var unroundedSuperannuation = AnnualGrossPackage * (SuperannuationPercentage / 100);
                return Math.Round(unroundedSuperannuation, 2, MidpointRounding.ToPositiveInfinity);
            }
        }

        public decimal AnnualTaxableIncome
        {
            get
            {
                var unroundedTaxableIncome = AnnualGrossPackage - AnnualSuperannuationContribution;
                return RoundDownToNearestDollar(unroundedTaxableIncome);
            }
        }

        public decimal AnnualNetIncome => AnnualGrossPackage - AnnualSuperannuationContribution - Deductions;

        private decimal Deductions => MedicareLevy + BudgetRepairLevy + IncomeTax;

        public decimal PayPackageAmount(Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.Weekly:
                    return AnnualNetIncome / NumberOfWeeksInYear;
                case Frequency.Fortnightly:
                    return AnnualNetIncome / NumberOfFortnightsInYear;
                case Frequency.Yearly:
                    return AnnualNetIncome;
                default:
                    throw new ArgumentException(nameof(frequency));
            }
        }
        
        public decimal MedicareLevy
        {
            get
            {
                var roundedTaxableIncome = RoundDownToNearestDollar(AnnualTaxableIncome);

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
        }

        public decimal BudgetRepairLevy
        {
            get
            {
                var roundedTaxableIncome = RoundDownToNearestDollar(AnnualTaxableIncome);

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
                var roundedTaxableIncome = RoundDownToNearestDollar(AnnualTaxableIncome);

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
                    var excess = excessIncome * (taxPercentage / 100);
                    return Math.Round(excess, 0, MidpointRounding.ToPositiveInfinity);
                }
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
    }
}
