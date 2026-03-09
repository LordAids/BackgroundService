using BackgroundService.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Npgsql;

using SomeService.Data;
using SomeService.Services.Interfaces;
using SomeService.Services.Services;

namespace BackgroundService
{
    internal class Program
    {
        static async Task Main(string[] args)
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
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                services.AddDbContext<DellinDictionaryDbContext>(options =>
                    options.UseNpgsql(dataSource, npgsql =>
                    {
                        npgsql.MigrationsAssembly("SomeService.Data");
                        npgsql.CommandTimeout(300);
                    })
                );

                services.AddScoped<IOfficeImportService, OfficeImportService>();
                services.AddScoped<OfficeFileReader>();
                services.AddHostedService<TerminalImportService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

            await host.RunAsync();
        }
    }
}
