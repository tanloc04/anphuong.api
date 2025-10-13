using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Repository.Repositories;
using anphuong.Service;

namespace anphuong.api.Extensions
{
    //services, controllers declared here
    public static class ServiceCollectionExtensions
    {
        public static void Register(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            //----------------Pagination----------
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));

            //----------------JWT-----------------
            services.AddScoped<IJwtService, JwtService>();

            //----------------User----------------
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            //----------------Customer----------------
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();

            //----------------Google----------------
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            //----------------Email-----------------
            services.AddScoped<IEmailService, EmailService>();

            //----------------Product-----------------
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            //----------------Category-----------------
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
        }
    }
}
