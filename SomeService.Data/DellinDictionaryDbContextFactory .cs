using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SomeService.Data
{
    public class DellinDictionaryDbContextFactory : IDesignTimeDbContextFactory<DellinDictionaryDbContext>
    {
        public DellinDictionaryDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DellinDictionaryDbContext>();
            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));

            return new DellinDictionaryDbContext(optionsBuilder.Options);
        }
    }
}
