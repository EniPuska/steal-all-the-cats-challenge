using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace StealCat.Data
{
    public class CatDbContextFactory : IDesignTimeDbContextFactory<CatDbContext>
    {
        public CatDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) 
                .AddJsonFile("StealCatApi/appsettings.json")  
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new CatDbContext(optionsBuilder.Options); 
        }
    }
}
