using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackgroundService.DTOs
{
    public class TerminalsRootDto
    {
        [JsonPropertyName("city")]
        public List<CityDto> Cities { get; set; } = new();
    }

    public class CityDto
    {
        [JsonPropertyName("cityID")]
        public int? CityId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("terminals")]
        public TerminalsWrapperDto? Terminals { get; set; }
    }

    public class TerminalsWrapperDto
    {
        [JsonPropertyName("terminal")]
        public List<TerminalDto> Terminal { get; set; } = new();
    }

    public class TerminalDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("fullAddress")]
        public string? FullAddress { get; set; }

        [JsonPropertyName("latitude")]
        public string? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public string? Longitude { get; set; }

        [JsonPropertyName("isPVZ")]
        public bool IsPvz { get; set; }

        [JsonPropertyName("calcSchedule")]
        public CalcScheduleDto? CalcSchedule { get; set; }

        [JsonPropertyName("phones")]
        public List<PhoneDto> Phones { get; set; } = new();
    }

    public class CalcScheduleDto
    {
        [JsonPropertyName("derival")]
        public string? Derival { get; set; }
    }

    public class PhoneDto
    {
        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
