using DotNetEnv;
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
            // Find the API project folder (where .env is located)
            var apiPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "anphuong.API");
            var envFile = Path.Combine(apiPath, ".env");

            // Explicitly load the .env file
            DotNetEnv.Env.Load(envFile);

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string not found. Please set DB_CONNECTION in your .env file or system environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<anphuongDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new anphuongDbContext(optionsBuilder.Options);
        }
    }
}
