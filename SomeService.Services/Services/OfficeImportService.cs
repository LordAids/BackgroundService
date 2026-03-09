using EFCore.BulkExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SomeService.Data;
using SomeService.Data.Entities;
using SomeService.Services.Interfaces;

namespace SomeService.Services.Services
{
    public class OfficeImportService : IOfficeImportService
    {
        private readonly DellinDictionaryDbContext _context;
        private readonly ILogger<OfficeImportService> _logger;

        public OfficeImportService(
            DellinDictionaryDbContext context,
            ILogger<OfficeImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Office>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var offices = await _context.Offices.ToListAsync(cancellationToken);

            _logger.LogInformation("Получено {Count} терминалов из БД", offices.Count);

            return offices;
        }

        public async Task AddRangeAsync(List<Office> offices, CancellationToken cancellationToken = default)
        {
            await _context.BulkInsertAsync(offices);

            _logger.LogInformation("Сохранено {NewCount} новых терминалов", offices.Count);
        }

        public async Task DeleteRangeAsync(List<Office> offices, CancellationToken cancellationToken = default)
        {
            await _context.BulkDeleteAsync(offices);

            _logger.LogInformation("Удалено {OldCount} старых записей", offices.Count);
        }
    }
}
