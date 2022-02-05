using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Scripting.Services;

namespace WebApi.Features.Scripting
{
    public class Injector: InjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<IScriptService, ScriptStorageService>();
            services.AddScoped<IScriptStorageService, ScriptStorageService>();
        }
    }
}