using IdentityEntityFrameworkCore.Models.Services;

namespace IdentityEntityFrameworkCore.Models.Config
{
    public class DependencyInjectionConfig
    {
        public void Config(IServiceCollection services)
        {
            ConfigRepository(services);
            ConfigBusiness(services);
        }

        private void ConfigRepository(IServiceCollection services)
        {
        }

        private void ConfigBusiness(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
