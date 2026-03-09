using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
