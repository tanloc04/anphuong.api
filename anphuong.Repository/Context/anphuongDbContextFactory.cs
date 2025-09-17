using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Repository.Context
{
    public class anphuongDbContextFactory : IDesignTimeDbContextFactory<anphuongDbContext>
    {
        public anphuongDbContext CreateDbContext(string[] args)
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "anphuong.api");
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<anphuongDbContext>();
            var connectionString = config.GetConnectionString("Database");
            optionsBuilder.UseSqlServer(connectionString);
            return new anphuongDbContext(optionsBuilder.Options);
        }
    }
}
