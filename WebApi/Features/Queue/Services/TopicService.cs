using System;
using System.Threading.Tasks;
using WebApi.Features.Queue.Requests;
using WebApi.Features.Queue.Responses;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Queue.Services
{
    public interface ITopicService
    {
        Task<TopicDto> CreateTopic(CreateTopicRequest request);
        Task<TopicDto> DeleteTopic(long topicId);
    }
    public class TopicService: ITopicService
    {
        private readonly AppDbContext _db;
        public TopicService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TopicDto> CreateTopic(CreateTopicRequest request)
        {
            var service = await _db.Services.FindAsync(request.ServiceId);
            if (service == null)
                throw new InvalidOperationException("Выбранный сервис не существует");

            var result = _db.Topics.Add(Topic.Create(request.Name, service, request.SolveByReading));

            await _db.SaveChangesAsync();

            return new TopicDto(result.Entity);
        }

        public async Task<TopicDto> DeleteTopic(long topicId)
        {
            var topic = await _db.Topics.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == topicId);
            return await DeleteTopic(topic);
        }

        private async Task<TopicDto> DeleteTopic(Topic topic)
        {
            if (topic == null)
                throw new InvalidOperationException("Выбранный топик не существует");
            
            var resultDto = new TopicDto(topic);

            _db.Messages.RemoveRange(topic.Messages);
            _db.Topics.Remove(topic);

            await _db.SaveChangesAsync();
            
            return resultDto;
        }
    }
}