using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SomeService.Data.Entities
{
    [Table("Offices")]
    [Index(nameof(Code), Name = "ix_offices_code")]
    [Index(nameof(CityCode), Name = "ix_offices_city_code")]
    [Index(nameof(Uuid), Name = "ix_offices_uuid")]
    public class Office : BaseEntity
    {
        public string? Code { get; set; }
        public int CityCode { get; set; }
        public string? Uuid { get; set; }
        public OfficeType? Type { get; set; }
        public string? CountryCode { get; set; }
        public string? WorkTime { get; set; }

        public Coordinates Coordinates { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public List<Phone> Phones { get; set; } = new();

        public Office() { }
    }
}
