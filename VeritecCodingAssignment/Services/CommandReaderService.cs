using System;
using Microsoft.Extensions.Options;
using VeritecCodingAssignment.Configuration;
using VeritecCodingAssignment.Models;

namespace VeritecCodingAssignment.Services
{
    public class CommandReaderService : ICommandReaderService
    {
        private readonly PayFrequencyCommandConfiguration _payFrequencyCommandConfiguration;

        public CommandReaderService(IOptions<PayFrequencyCommandConfiguration> payFrequencyCommandConfigurationOptions)
        {
            _payFrequencyCommandConfiguration = payFrequencyCommandConfigurationOptions.Value;
        }

        public decimal ReadSalaryPackageFromConsole()
        {
            while (true)
            {
                Console.Write("Enter your salary package amount: ");

                var grossPackageString = Console.ReadLine();
                if (!decimal.TryParse(grossPackageString, out var salaryPackage) || salaryPackage <= 0)
                {
                    Console.WriteLine("An invalid amount has been entered.");
                }
                else
                {
                    return salaryPackage;
                }
            }
        }

        public PayFrequency ReadPayFrequencyFromConsole()
        {
            while (true)
            {
                Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");

                var payFrequencyString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(payFrequencyString))
                {
                    Console.WriteLine("An invalid pay frequency has been entered.");
                }
                else if (payFrequencyString.Equals(_payFrequencyCommandConfiguration.Weekly,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new PayFrequency(FrequencyType.Weekly, "week");
                }
                else if (payFrequencyString.Equals(_payFrequencyCommandConfiguration.Fortnightly,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new PayFrequency(FrequencyType.Fortnightly, "fortnight");
                }
                else if (payFrequencyString.Equals(_payFrequencyCommandConfiguration.Monthly,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new PayFrequency(FrequencyType.Monthly, "month");
                }
                else
                {
                    Console.WriteLine("An invalid pay frequency has been entered.");
                }
            }
        }
    }
}
