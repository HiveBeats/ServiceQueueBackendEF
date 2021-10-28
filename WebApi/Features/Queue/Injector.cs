using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Queue.Services;

namespace WebApi.Features.Queue
{
    public class Injector: InjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IMessageService, MessageService>();
        }
    }
}