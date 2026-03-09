using Microsoft.EntityFrameworkCore;

using SomeService.Data.Entities;

using System.Reflection;

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
                entity.OwnsOne(o => o.Coordinates, nav => nav.ToJson("Coordinates"));
            });
        }
    }

}
