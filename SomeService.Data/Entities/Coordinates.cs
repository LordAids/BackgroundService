using System.Text.Json.Serialization;

namespace SomeService.Data.Entities
{
    public class Coordinates
    {
        [JsonPropertyName("сoordinateX")]
        public double CoordinateX { get; set; }
        
        [JsonPropertyName("сoordinateY")]
        public double CoordinateY { get; set; }
    }
}
