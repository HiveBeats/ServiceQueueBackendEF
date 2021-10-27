using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Users.Services;

namespace WebApi.Features.Users
{
    public class Injector: InjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}