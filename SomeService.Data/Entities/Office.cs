using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SomeService.Data.Entities
{
    [Table("Offices")]
    [Index(nameof(CityCode), Name = "ix_offices_city_code")]
    [Index(nameof(AddressCity), Name = "ix_offices_address_city")]
    [Index(nameof(Code), Name = "ix_offices_code")]
    [Index(nameof(Uuid), Name = "ix_offices_uuid")]
    public class Office : BaseEntity
    {
        public string? Code { get; set; }
        public int CityCode { get; set; }
        public string? Uuid { get; set; }
        public OfficeType? Type { get; set; }
        public string CountryCode { get; set; }
        public Coordinates Coordinates { get; set; }
        public string? AddressRegion { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressStreet { get; set; }
        public string? AddressHouseNumber { get; set; }
        public int? AddressApartment { get; set; }
        public string WorkTime { get; set; }
        public Phone Phones { get; set; }
        public Office() { }
    }
}
