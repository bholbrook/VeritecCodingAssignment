using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VeritecCodingAssignment.Models;
using VeritecCodingAssignment.Services;

namespace VeritecCodingAssignment
{
    public static class Program
    {
        private static IConfiguration? _configuration;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            using var host = CreateHostBuilder(args).Build();

            WriteUi();

            await host.StartAsync().ConfigureAwait(false);
        }

        private static decimal ReadSalaryPackageFromConsole()
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

        private static PayFrequency ReadPayFrequencyFromConsole()
        {
            while (true)
            {
                Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");

                var payFrequencyString = Console.ReadLine();

                switch (payFrequencyString)
                {
                    case "W":
                    case"w":
                        return new PayFrequency(FrequencyType.Weekly, "week");
                    case "F":
                    case "f":
                        return new PayFrequency(FrequencyType.Fortnightly, "fortnight");
                    case "M":
                    case "m":
                        return new PayFrequency(FrequencyType.Monthly, "month");
                    default:
                        Console.WriteLine("An invalid pay frequency has been entered.");
                        break;
                }
            }
        }

        // TODO: Make this better eventually
        private static void WriteUi()
        {
            var grossPackage = ReadSalaryPackageFromConsole();
            var payFrequency = ReadPayFrequencyFromConsole();

            Console.WriteLine("Calculating salary details...");
            Console.WriteLine(Environment.NewLine);

            var taxService = new TaxService();

            var annualTaxableIncome = taxService.AnnualTaxableIncome(grossPackage);

            Console.WriteLine($"Gross package: {grossPackage:C}");

            var superannuationContribution = taxService.AnnualSuperannuationContribution(grossPackage);
            Console.WriteLine($"Superannuation: {superannuationContribution:C}");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Taxable income: {annualTaxableIncome:C}");
            Console.WriteLine(Environment.NewLine);

            var deductions = taxService.Deductions(annualTaxableIncome);
            Console.WriteLine($"Deductions: {deductions:C}");
            Console.WriteLine($"Medicare Levy: {taxService.MedicareLevy(annualTaxableIncome):C}");
            Console.WriteLine($"Budget Repair Levy: {taxService.BudgetRepairLevy(annualTaxableIncome):C}");
            Console.WriteLine($"Income Tax: {taxService.IncomeTax(annualTaxableIncome):C}");
            Console.WriteLine(Environment.NewLine);

            var annualNetIncome = taxService.AnnualNetIncome(grossPackage, superannuationContribution, deductions);
            Console.WriteLine($"Net income: {annualNetIncome:C}");
            
            Console.WriteLine($"Pay packet: {taxService.PayPacketAmount(annualNetIncome, payFrequency.FrequencyType):C} per {payFrequency.Name}");
        }

        private static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            if (_configuration == null)
            {
                throw new Exception("Configuration is null");
            }

            services.AddLogging();
            services.AddOptions();

            services.AddTransient<ITaxService, TaxService>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices(Configure);
        }

        private static T ConfigureOptions<T>(IServiceCollection services, IConfiguration configuration,
            string configurationSectionKey)
            where T : class
        {
            var configurationSection = configuration.GetSection(configurationSectionKey);
            services.Configure<T>(configurationSection);
            return configurationSection.Get<T>();
        }
    }
}
