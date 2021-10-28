using System;
using System.Threading.Tasks;
using WebApi.Features.Queue.Requests;
using WebApi.Features.Queue.Responses;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApi.Features.Queue.Services
{
    public interface IMessageService
    {
        Task<MessageDto> GetLastMessageOnTopic(long topicId);
        Task<MessageDto> GetLastMessageOnTopic(Topic topic);
        Task<MessageDto> PushMessageOnTopic(PushMessageOnTopicRequest request);
        Task<MessageDto> PushMessageOnTopic(Topic topic, string payload, string correlationId);
    }

    public class MessageService: IMessageService
    {
        private readonly AppDbContext _db;
        
        public MessageService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MessageDto> GetLastMessageOnTopic(long topicId)
        {
            var topic = await _db.Topics.FindAsync(topicId);
            return await GetLastMessageOnTopic(topic);
        }

        public async Task<MessageDto> GetLastMessageOnTopic(Topic topic)
        {
            if (topic == null)
                throw new InvalidOperationException("Выбранный топик не существует");

            var message = await _db.Messages.Where(x => x.TopicId == topic.Id && x.DateSolved == null)
                                    .OrderBy(x => x.DateCreated).FirstOrDefaultAsync();

            if (topic.SolveByReading)
                message.Solve();
            
            await _db.SaveChangesAsync();

            return new MessageDto(message);
        }

        public async Task<MessageDto> PushMessageOnTopic(PushMessageOnTopicRequest request)
        {
            var topic = await _db.Topics.FindAsync(request.TopicId);
            return await PushMessageOnTopic(topic, request.MessagePayload, request.CorrelationId);
        }

        public async Task<MessageDto> PushMessageOnTopic(Topic topic, string payload, string correlationId)
        {
            if (topic == null)
                throw new InvalidOperationException("Выбранный топик не существует");
            var message = Message.Create(topic, payload, correlationId);
            
            _db.Messages.Add(message);

            if (!string.IsNullOrEmpty(correlationId) && long.TryParse(correlationId, out var correlation))
            {
                var originMessage = await _db.Messages.FindAsync(correlation);
                
                originMessage?.Solve();
                message.Solve();
            }
            await _db.SaveChangesAsync();

            return new MessageDto(message);
        }
    }
}