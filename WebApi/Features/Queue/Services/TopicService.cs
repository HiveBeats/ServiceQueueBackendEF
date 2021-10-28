using System;
using System.Threading.Tasks;
using WebApi.Features.Queue.Requests;
using WebApi.Features.Queue.Responses;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace WebApi.Features.Queue.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicDto>> GetServiceTopics(long serviceId);
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

        public async Task<IEnumerable<TopicDto>> GetServiceTopics(long serviceId)
        {
            var service = await _db.Services.FindAsync(serviceId);
            if (service == null)
                throw new InvalidOperationException("Выбранный сервис не существует");
            
            var topics = await _db.Topics.AsNoTracking().Where(x => x.ServiceId == serviceId).ToListAsync();
            return topics.Select(x => new TopicDto(x));
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