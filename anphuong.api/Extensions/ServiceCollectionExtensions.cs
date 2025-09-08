namespace anphuong.api.Extensions
{
    //services, controllers declared here
    public static class ServiceCollectionExtensions
    {
        public static void Register(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

        }
    }
}
