using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Features.Queue.Services;
using WebApi.Features.Queue.Responses;
using WebApi.Features.Queue.Requests;
using System;
using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WebApi.Features.Services.Services;
using WebApi.Features.Services.Requests;

namespace WebApi.Tests.Features.Queue
{
    public class TopicServiceTests
    {
        private readonly ServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;

        public TopicServiceTests()
        {
            _services = new ServiceCollection();

            _services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "TopicInMemoryDb"), 
               ServiceLifetime.Scoped, 
               ServiceLifetime.Scoped);

            _services.AddScoped<ITopicService, TopicService>();
            _services.AddScoped<IServicesService, ServicesService>();

            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public async Task CreateTopicForNonExistentServiceThrows()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var rnd = new Random();

                //act && assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => topicService.CreateTopic(new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = rnd.Next(100, 1000) }));
            }
        }

        [Fact]
        public async Task CreateTopicSuccessfull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var servicesService = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var serviceDto = await servicesService.CreateService(new CreateServiceRequest() { Name = Guid.NewGuid().ToString() });

                //act
                var createTopicRequest = new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = serviceDto.Id };
                var topic = await topicService.CreateTopic(createTopicRequest);

                //assert
                var item = await db.Topics.FirstOrDefaultAsync(x => x.Name == createTopicRequest.Name);
                Assert.NotNull(item);
                var service = await db.Services.FindAsync(serviceDto.Id);
                Assert.Equal(service.Topics.FirstOrDefault().Id, item.Id);
            }
        }

        [Fact]
        public async Task CreateTopicResponseValid()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var servicesService = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var service = await servicesService.CreateService(new CreateServiceRequest() { Name = Guid.NewGuid().ToString() });

                //act
                var createTopicRequest = new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = service.Id };
                var topic = await topicService.CreateTopic(createTopicRequest);

                //assert
                var item = await db.Topics.FirstOrDefaultAsync(x => x.Name == createTopicRequest.Name);
                Assert.Equal(item.Id, topic.Id);
                Assert.Equal(item.Name, topic.Name);
                Assert.Equal(item.SolveByReading, topic.SolveByReading);
                Assert.Equal(item.ServiceId, topic.ServiceId);
            }
        }

        [Fact]
        public async Task DeleteTopicNonExistentThrows()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var rnd = new Random();

                //act && assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => topicService.DeleteTopic(rnd.Next(100, 10000)));
            }
        }

        [Fact]
        public async Task DeleteTopicSuccessfull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var servicesService = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var service = await servicesService.CreateService(new CreateServiceRequest() { Name = Guid.NewGuid().ToString() });

                var createTopicRequest = new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = service.Id };
                var topic = await topicService.CreateTopic(createTopicRequest);

                //act
                var response = await topicService.DeleteTopic(topic.Id);
                //assert
                var item = await db.Topics.FindAsync(topic.Id);
                Assert.Null(item);
            }
        }

        [Fact]
        public async Task DeleteTopicResponseValid()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var servicesService = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var service = await servicesService.CreateService(new CreateServiceRequest() { Name = Guid.NewGuid().ToString() });

                var createTopicRequest = new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = service.Id };
                var topic = await topicService.CreateTopic(createTopicRequest);

                //act
                var response = await topicService.DeleteTopic(topic.Id);

                //assert
                Assert.Equal(topic.Id, response.Id);
                Assert.Equal(topic.ServiceId, response.ServiceId);
            }
        }

        [Fact]
        public async Task GetServiceTopicsForNonExistentServiceThrows()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var rnd = new Random();

                //act && assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => topicService.GetServiceTopics(rnd.Next(100, 10000)));
            }
        }

        [Fact]
        public async Task GetServiceTopicsAll()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var servicesService = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var service = await servicesService.CreateService(new CreateServiceRequest() { Name = Guid.NewGuid().ToString() });

                Func<CreateTopicRequest> createRequestFn = () => new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = service.Id };
                var topic = await topicService.CreateTopic(createRequestFn());
                await topicService.CreateTopic(createRequestFn());
                await topicService.CreateTopic(createRequestFn());

                //act
                var items = await topicService.GetServiceTopics(service.Id);

                //assert
                Assert.Equal(3, items.Count());
                Assert.NotNull(items.FirstOrDefault(x => x.Id == topic.Id));
            }
        }

    }
}