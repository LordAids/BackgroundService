using BackgroundService.DTOs;

using Microsoft.Extensions.Logging;

using SomeService.Data.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

            TerminalsRootDto root;
            await using (var stream = File.OpenRead(filePath))
            {
                root = await JsonSerializer.DeserializeAsync<TerminalsRootDto>(
                stream, JsonOptions, cancellationToken);
            }

            if (root?.Cities is null || root.Cities.Count == 0)
            {
                _logger.LogWarning("Файл пустой или не содержит терминалов: {FilePath}", filePath);
                return [];
            }

            var offices = root.Cities
                .Where(c => c.Terminals?.Terminal is { Count: > 0 })
                .SelectMany(c => c.Terminals!.Terminal.Select(t => MapToEntity(t, c)))
                .ToList();

            _logger.LogInformation("Загружено {Count} терминалов из файла", offices.Count);

            return offices;
        }

        private static Office MapToEntity(TerminalDto terminal, CityDto city) => new()
        {
            Id = int.TryParse(terminal.Id, out var id) ? id : 0,
            CityCode = city.CityId ?? 0,
            Type = terminal.IsPvz ? OfficeType.PVZ : OfficeType.WAREHOUSE,
            WorkTime = terminal.CalcSchedule?.Derival,
            Coordinates = new Coordinates
            {
                Latitude = double.TryParse(terminal.Latitude,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var lat) ? lat : 0,
                Longitude = double.TryParse(terminal.Longitude,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var lon) ? lon : 0,
            },
            AddressCity = city.Name,
            AddressRegion = city.Name,
            AddressStreet = terminal.FullAddress,
            Phones = terminal.Phones.Count > 0
            ? new Phone
            {
                PhoneNumber = terminal.Phones[0].Number ?? string.Empty,
                Additional = terminal.Phones[0].Comment
            }
            : null,
            CountryCode = "N/A",
        };
    }
}
