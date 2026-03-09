using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Npgsql;

using NpgsqlTypes;

using SomeService.Data;
using SomeService.Data.Entities;
using SomeService.Services.Interfaces;

using System.Text.Json;

namespace SomeService.Services.Services
{
    public class OfficeImportService : IOfficeImportService
    {
        private readonly DellinDictionaryDbContext _context;
        private readonly ILogger<OfficeImportService> _logger;
        private int ChunkSize = 500;

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
            foreach (var chunk in offices.Chunk(ChunkSize))
            {
                await BulkInsertOfficesAsync(chunk, cancellationToken);
                await BulkInsertPhonesAsync(chunk, cancellationToken);
            }

            _logger.LogInformation("Сохранено {NewCount} новых терминалов", offices.Count);
        }

        private async Task BulkInsertOfficesAsync(Office[] offices, CancellationToken cancellationToken)
        {
            var rows = new List<string>();
            var parameters = new List<NpgsqlParameter>();
            var row = 0;

            foreach (var o in offices)
            {
                var coordinates = JsonSerializer.Serialize(o.Coordinates);

                rows.Add($"(@id_{row},@code_{row},@cityCode_{row},@uuid_{row},@type_{row},@countryCode_{row},@workTime_{row},@addressRegion_{row},@addressCity_{row},@addressStreet_{row},@addressHouseNumber_{row},CAST(@coordinates_{row} AS jsonb))");

                parameters.Add(new NpgsqlParameter($"id_{row}", o.Id));
                parameters.Add(new NpgsqlParameter($"code_{row}", (object?)o.Code ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"cityCode_{row}", o.CityCode));
                parameters.Add(new NpgsqlParameter($"uuid_{row}", (object?)o.Uuid ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"type_{row}", o.Type.HasValue ? (object)(int)o.Type.Value : DBNull.Value));
                parameters.Add(new NpgsqlParameter($"countryCode_{row}", (object?)o.CountryCode ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"workTime_{row}", (object?)o.WorkTime ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"addressRegion_{row}", (object?)o.AddressRegion ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"addressCity_{row}", (object?)o.AddressCity ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"addressStreet_{row}", (object?)o.AddressStreet ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"addressHouseNumber_{row}", (object?)o.AddressHouseNumber ?? DBNull.Value));
                parameters.Add(new NpgsqlParameter($"coordinates_{row}", coordinates) { NpgsqlDbType = NpgsqlDbType.Jsonb });

                row++;
            }

            var sql = $"""
    INSERT INTO "Offices"
        (id, "Code", "CityCode", "Uuid", "Type", "CountryCode", "WorkTime",
         "AddressRegion", "AddressCity", "AddressStreet", "AddressHouseNumber", "Coordinates")
    VALUES {string.Join(", ", rows)}
    """;

            await _context.Database.ExecuteSqlRawAsync(sql, parameters.Cast<object>().ToList(), cancellationToken);
        }

        private async Task BulkInsertPhonesAsync(Office[] offices, CancellationToken cancellationToken)
        {
            var phones = offices
                .Where(o => o.Phones != null)
                .Select(o => new Phone
                {
                    OfficeId = o.Id,
                    PhoneNumber = o.Phones!.PhoneNumber,
                    Additional = o.Phones!.Additional
                })
                .ToList();

            if (phones.Count == 0) return;

            var rows = new List<string>();
            var parameters = new List<NpgsqlParameter>();
            var row = 0;

            foreach (var p in phones)
            {
                rows.Add($"(@officeId_{row},@phoneNumber_{row},@additional_{row})");
                parameters.Add(new NpgsqlParameter($"officeId_{row}", p.OfficeId));
                parameters.Add(new NpgsqlParameter($"phoneNumber_{row}", p.PhoneNumber));
                parameters.Add(new NpgsqlParameter($"additional_{row}", (object?)p.Additional ?? DBNull.Value));
                row++;
            }

            var sql = $"""
        INSERT INTO "Phones" ("OfficeId", "PhoneNumber", "Additional")
        VALUES {string.Join(", ", rows)}
        """;

            await _context.Database.ExecuteSqlRawAsync(sql, parameters.Cast<object>().ToList(), cancellationToken);
        }

        public async Task DeleteRangeAsync(List<Office> offices, CancellationToken cancellationToken = default)
        {
            var ids = offices.Select(o => o.Id).ToList();

            await _context.Database.ExecuteSqlRawAsync(
                $"DELETE FROM \"Offices\" WHERE id = ANY(ARRAY[{string.Join(",", ids)}])",
                cancellationToken);

            _logger.LogInformation("Удалено {OldCount} старых записей", ids.Count);
        }

        private static object Nullable(object? value) => value ?? DBNull.Value;
    }
}
