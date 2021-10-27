using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Services.Services;

namespace WebApi.Features.Services
{
    public class Injector: InjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<IServicesService, ServicesService>();
        }
    }
}