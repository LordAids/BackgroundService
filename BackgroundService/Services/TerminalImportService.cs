using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SomeService.Data;
using SomeService.Services.Interfaces;

namespace BackgroundService.Services
{
    internal class TerminalImportService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ILogger<TerminalImportService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _filePath;

        public TerminalImportService(ILogger<TerminalImportService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _filePath = configuration["Import:FilePath"]
                ?? throw new InvalidOperationException("Import:FilePath не задан в конфигурации");

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TerminalImportService запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun();

                _logger.LogInformation(
                    "Следующий запуск через {Hours}ч {Minutes}м",
                    (int)delay.TotalHours,
                    delay.Minutes);

                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await ImportAsync(stoppingToken);
                }
            }

            _logger.LogInformation("TerminalImportService остановлен");
        }

        private async Task ImportAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var officeFileReader = scope.ServiceProvider.GetRequiredService<OfficeFileReader>();
                var officeImportService = scope.ServiceProvider.GetRequiredService<IOfficeImportService>();
                var context = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();

                var fromFile = await officeFileReader.ReadAsync(_filePath, cancellationToken);
                var fromDb = await officeImportService.GetAllAsync(cancellationToken);

                var fileIds = fromFile.Select(o => o.Id).ToHashSet();
                var dbIds = fromDb.Select(o => o.Id).ToHashSet();

                var toDelete = fromDb.Where(o => !fileIds.Contains(o.Id)).ToList();
                var toAdd = fromFile.Where(o => !dbIds.Contains(o.Id)).ToList();

                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    if (toDelete.Count > 0)
                    {
                        await officeImportService.DeleteRangeAsync(toDelete, cancellationToken);
                    }

                    if (toAdd.Count > 0)
                    {
                        await officeImportService.AddRangeAsync(toAdd, cancellationToken);
                    }

                    await transaction.CommitAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    _logger.LogError(ex, "Ошибка импорта, транзакция отменена");
                    throw;
                }

                _logger.LogInformation("Импорт завершён.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка импорта терминалов");
            }
        }

        private static TimeSpan GetDelayUntilNextRun()
        {
            var now = DateTime.UtcNow;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 23, 0, 0, DateTimeKind.Utc);

            if (now >= nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun - now;
        }
    }
}
