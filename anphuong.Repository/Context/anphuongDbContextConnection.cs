using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace anphuong.Repository.Context
{
    public static class anphuongDbContextConnection
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<anphuongDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            return services;
        }
    }
}
