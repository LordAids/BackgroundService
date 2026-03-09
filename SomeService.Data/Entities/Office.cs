using SomeService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Office : BaseEntity
{
    public string? Code { get; set; }
    public int CityCode { get; set; }
    public string? Uuid { get; set; }
    public OfficeType? Type { get; set; }
    public string? CountryCode { get; set; }
    public string? WorkTime { get; set; }

    // JSON-колонки
    public Coordinates Coordinates { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public List<Phone> Phones { get; set; } = new();

    public Office() { }
}

