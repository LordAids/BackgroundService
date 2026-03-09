using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SomeService.Data
{
    public class DellinDictionaryDbContext : DbContext
    {
        public DellinDictionaryDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Office> Offices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<Office>(entity =>
            {
                // Без этих трёх строк EF не знает что Address/Coordinates/Phone — owned,
                // и пытается создать их как отдельные таблицы с PK
                entity.OwnsOne(o => o.Coordinates, nav => nav.ToJson("coordinates"));
                entity.OwnsOne(o => o.Address, nav => nav.ToJson("address"));
                entity.OwnsMany(o => o.Phones, nav => nav.ToJson("phones"));

                entity.HasIndex(o => o.Code).HasDatabaseName("ix_offices_code");
                entity.HasIndex(o => o.CityCode).HasDatabaseName("ix_offices_city_code");
                entity.HasIndex(o => o.Uuid).HasDatabaseName("ix_offices_uuid");
            });
        }
    }

}
