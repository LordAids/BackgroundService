using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using SomeService.Data.Entities;

namespace BackgroundService.Services
{
    internal class OfficeFileReader
    {
        private readonly ILogger<OfficeFileReader> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public OfficeFileReader(ILogger<OfficeFileReader> logger)
        {
            _logger = logger;
        }

        public async Task<List<Office>> ReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Чтение файла терминалов: {FilePath}", filePath);

            if (!File.Exists(filePath))
            {
                _logger.LogError("Файл не найден: {FilePath}", filePath);
                throw new FileNotFoundException("Файл терминалов не найден", filePath);
            }

            await using var stream = File.OpenRead(filePath);

            var offices = await JsonSerializer.DeserializeAsync<List<Office>>(
                stream, JsonOptions, cancellationToken);

            if (offices is null || offices.Count == 0)
            {
                _logger.LogWarning("Файл пустой или не содержит терминалов: {FilePath}", filePath);
                return [];
            }

            _logger.LogInformation("Загружено {Count} терминалов из файла", offices.Count);

            return offices;
        }
    }
}
