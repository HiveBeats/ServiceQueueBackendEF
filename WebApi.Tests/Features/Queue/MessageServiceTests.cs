using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Queue.Requests;
using WebApi.Features.Queue.Responses;
using WebApi.Features.Queue.Services;
using WebApi.Features.Services.Requests;
using WebApi.Features.Services.Responses;
using WebApi.Features.Services.Services;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Features.Queue
{
    public class MessageServiceTests
    {
        private readonly ServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;

        private CreateServiceResponse _serviceDto;
        private TopicDto _topicDto;
        private TopicDto _solvingTopicDto;
        public MessageServiceTests()
        {
            _services = new ServiceCollection();

            _services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "TopicInMemoryDb"), 
               ServiceLifetime.Scoped, 
               ServiceLifetime.Scoped);

            _services.AddScoped<IMessageService, MessageService>();
            _services.AddScoped<ITopicService, TopicService>();
            _services.AddScoped<IServicesService, ServicesService>();

            _serviceProvider = _services.BuildServiceProvider();

            Initialize().GetAwaiter().GetResult();
        }

        private async Task Initialize()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var servicesService = scope.ServiceProvider.GetRequiredService<IServicesService>();
                
                _serviceDto = await servicesService.CreateService(new CreateServiceRequest() { Name = Guid.NewGuid().ToString() });

                var createTopicRequest = new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = _serviceDto.Id };
                _topicDto = await topicService.CreateTopic(createTopicRequest);

                var createSolvingTopicRequest = new CreateTopicRequest() { Name = Guid.NewGuid().ToString(), ServiceId = _serviceDto.Id, SolveByReading = true };
                _solvingTopicDto = await topicService.CreateTopic(createSolvingTopicRequest);
            }
        }

        [Fact]
        public async Task PushMessageTopicNotExistsThrow()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var rnd = new Random();
                
                //act && assert
                await Assert.ThrowsAnyAsync<InvalidOperationException>(() => messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(rnd.Next(100, 1000), new PushMessageRequest(){})));
            }
        }

        [Fact]
        public async Task PushMessageSuccessfull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var rnd = new Random();
                
                //act
                var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString() };
                var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushMessageRequest));

                //assert
                var item = await db.Messages.FirstOrDefaultAsync(x => x.MessagePayload == pushMessageRequest.MessagePayload);
                Assert.NotNull(item);
            }
        }

        [Fact]
        public async Task PushedMessageNotRead()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var rnd = new Random();
                
                //act
                var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString() };
                var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushMessageRequest));

                //assert
                var item = await db.Messages.FindAsync(messageDto.Id);
                Assert.Null(item.DateSolved);
            }
        }

        [Fact]
        public async Task PushedMessageWithCorrelationIsRead()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var rnd = new Random();
                
                //act
                var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString(), CorrelationId = rnd.Next(1,100).ToString() };
                var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushMessageRequest));

                //assert
                var item = await db.Messages.FindAsync(messageDto.Id);
                //todo:CREATE DATE SERVICE TO MOCK DATETIME.NOW CALLS
                Assert.Equal(DateTime.Today.Day, item.DateSolved.Value.Day);
                Assert.Equal(DateTime.UtcNow.Hour, item.DateSolved.Value.Hour);
                Assert.Equal(DateTime.UtcNow.Minute, item.DateSolved.Value.Minute);
            }
        }

        [Fact]
        public async Task PushedMessageWithCorrelationOriginalRead()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var rnd = new Random();
                
                var pushOriginMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString() };
                var originMessageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushOriginMessageRequest));
                
                //act
                var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString(), CorrelationId = originMessageDto.Id.ToString() };
                var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushMessageRequest));

                //assert
                var item = await db.Messages.FindAsync(originMessageDto.Id);
                //todo:CREATE DATE SERVICE TO MOCK DATETIME.NOW CALLS
                Assert.Equal(DateTime.Today.Day, item.DateSolved.Value.Day);
                Assert.Equal(DateTime.UtcNow.Hour, item.DateSolved.Value.Hour);
                Assert.Equal(DateTime.UtcNow.Minute, item.DateSolved.Value.Minute);
            }
        }

        [Fact]
        public async Task GetMessageThrowsIfTopicNotExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var rnd = new Random();
                
                //act && assert
                await Assert.ThrowsAnyAsync<InvalidOperationException>(() => messageService.GetLastMessageOnTopic(rnd.Next(100, 1000)));
            }
        }

        private async Task ClearMessagesOnTopics()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var topicMessages = await db.Messages.Where(x => x.TopicId == _topicDto.Id).ToListAsync();
                var solvingTopicMessages = await db.Messages.Where(x => x.TopicId == _solvingTopicDto.Id).ToListAsync();

                db.Messages.RemoveRange(topicMessages);
                db.Messages.RemoveRange(solvingTopicMessages);

                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task GetMessageReadInSolvingTopic()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                
                await ClearMessagesOnTopics();

                var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString() };
                var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_solvingTopicDto.Id, pushMessageRequest));
                
                //act
                var lastMessage = await messageService.GetLastMessageOnTopic(_solvingTopicDto.Id);

                //assert
                var item = await db.Messages.FindAsync(lastMessage.Id);
                Assert.NotNull(item.DateSolved);
            }
        }

        [Fact]
        public async Task GetMessageNotReadInNonSolvingTopic()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                
                await ClearMessagesOnTopics();

                var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString() };
                var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushMessageRequest));
                
                //act
                var lastMessage = await messageService.GetLastMessageOnTopic(_topicDto.Id);

                //assert
                var item = await db.Messages.FindAsync(lastMessage.Id);
                Assert.Null(item.DateSolved);
            }
        }

        private async Task<MessageDto> PushMessage(IMessageService messageService)
        {
            await Task.Delay(1);
            var pushMessageRequest = new PushMessageRequest() { MessagePayload = Guid.NewGuid().ToString() };
            var messageDto = await messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(_topicDto.Id, pushMessageRequest));
            return messageDto;
        }

        [Fact]
        public async Task GetMessageIsLatest()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                
                await ClearMessagesOnTopics();

                var firstAddedMessage = await PushMessage(messageService);
                
                for (int i = 0; i <= 10; i++)
                    await PushMessage(messageService);

                //act
                var lastMessage = await messageService.GetLastMessageOnTopic(_topicDto.Id);

                //assert
                Assert.Equal(firstAddedMessage.Id, lastMessage.Id);
            }
        }

        private async Task SolveItem(AppDbContext db, long messageId)
        {
            var firstAddedMessageItem = await db.Messages.FindAsync(messageId);
            firstAddedMessageItem.Solve();
            await db.SaveChangesAsync();
        }

        private async Task PushMessageAndSolveIt(IMessageService messageService, AppDbContext db)
        {
            var messageDto = await PushMessage(messageService);
            await SolveItem(db, messageDto.Id);
        }

        [Fact]
        public async Task GetMessageExludesSolved()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                
                await ClearMessagesOnTopics();

                for (int i = 0; i <= 10; i++)
                    await PushMessageAndSolveIt(messageService, db);

                var nextAddedMessageDto = await PushMessage(messageService);

                for (int i = 0; i <= 10; i++)
                    await PushMessage(messageService);

                //act
                var lastMessage = await messageService.GetLastMessageOnTopic(_topicDto.Id);

                //assert
                Assert.Equal(nextAddedMessageDto.Id, lastMessage.Id);
            }
        }

    }
}