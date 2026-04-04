using anphuong.api.SignalRServices;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Repository.Repositories;
using anphuong.Service;
using anphuong.Service.Extenal;

namespace anphuong.api.Extensions
{
    //services, controllers declared here
    public static class ServiceCollectionExtensions
    {
        public static void Register(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            //----------------External----------
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            //----------------User----------------
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            //----------------Customer----------------
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();

            //----------------Product-----------------
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            //----------------Category-----------------
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            //----------------DetailImage-----------------
            services.AddScoped<IDetailImageRepository, DetailImageRepository>();
            services.AddScoped<IDetailImageService, DetailImageService>();

            //----------------Order-----------------
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            //----------------Inventory-----------------
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            //----------------Color-----------------
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<IColorService, ColorService>();

            //----------------Variant-----------------
            services.AddScoped<IVariantRepository, VariantRepository>();
            services.AddScoped<IVariantService, VariantService>();

            //----------------Material-----------------
            services.AddScoped<IMaterialRepository, MaterialRepository>();
            services.AddScoped<IMaterialService, MaterialService>();

            //----------------ProductReview-----------------
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            services.AddScoped<IProductReviewService, ProductReviewService>();

            //----------------Cart-----------------
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IOrderHubService, OrderHubService>();

            services.AddScoped<IVnPayService, VnPayService>();

        }
    }
}
