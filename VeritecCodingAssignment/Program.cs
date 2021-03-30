using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VeritecCodingAssignment.Configuration;
using VeritecCodingAssignment.Services;

namespace VeritecCodingAssignment
{
    public static class Program
    {
        private const string PayFrequencyCommandsConfigurationKey = "PayFrequencyCommands";

        private static IConfiguration? _configuration;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            using var host = CreateHostBuilder(args).Build();

            // TODO: Replace this with real DI
            WriteUi(new CommandReaderService(Options.Create(new PayFrequencyCommandConfiguration())));

            await host.StartAsync().ConfigureAwait(false);
        }
        
        // TODO: Make this better eventually
        private static void WriteUi(ICommandReaderService commandReaderService)
        {
            var grossPackage = commandReaderService.ReadSalaryPackageFromConsole();
            var payFrequency = commandReaderService.ReadPayFrequencyFromConsole();

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

            ConfigureOptions<PayFrequencyCommandConfiguration>(services, _configuration,
                PayFrequencyCommandsConfigurationKey);

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
