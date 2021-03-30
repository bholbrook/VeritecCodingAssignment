using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        // TODO: Make this better eventually
        private static void WriteUi()
        {
            Console.Write("Enter your salary package amount: ");

            var grossPackageString = Console.ReadLine();

            Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");

            var payFrequencyString = Console.ReadLine();

            Console.WriteLine("Calculating salary details...");

            Console.WriteLine("Gross package: ");
            Console.WriteLine("Superannuation: ");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Taxable income: ");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Deductions:");
            Console.WriteLine("Medicare Levy: ");
            Console.WriteLine("Budget Repair Levy: ");
            Console.WriteLine("Income Tax: ");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Net income: ");
            Console.WriteLine("Pay packet: ");
        }

        private static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            if (_configuration == null)
            {
                throw new Exception("Configuration is null");
            }

            services.AddLogging();
            services.AddOptions();
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
