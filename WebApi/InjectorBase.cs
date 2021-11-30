using Microsoft.Extensions.DependencyInjection;

namespace WebApi
{
    public abstract class InjectorBase
    {
        public virtual void Inject(IServiceCollection services)
        {
            
        }
    }
}