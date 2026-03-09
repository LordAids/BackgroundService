using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SomeService.Data;

namespace BackgroundService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<DellinDictionaryDbContext>(options =>
                    options.UseNpgsql(
                        context.Configuration.GetConnectionString("Default"),
                        npgsql => npgsql.MigrationsAssembly("SomeService.Data")
                    )
                );

                // HostedService добавим на следующем шаге
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

            host.Run();
        }
    }
}
