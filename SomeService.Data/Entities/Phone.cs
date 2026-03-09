using System.Text.Json.Serialization;

namespace SomeService.Data.Entities
{
    public class Phone
    {
        [JsonPropertyName("number")]
        public string? Number { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
