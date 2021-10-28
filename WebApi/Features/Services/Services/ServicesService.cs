using System;
using System.Threading.Tasks;
using WebApi.Features.Services.Requests;
using WebApi.Features.Services.Responses;
using WebApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Tree.Models;
using WebApi.Features.Tree;

namespace WebApi.Features.Services.Services
{
    public interface IServicesService
    {
        Task<CreateServiceResponse> CreateService(CreateServiceRequest request);
        Task<DeleteServiceResponse> DeleteService(string serviceId);
        Task<Root> GetServiceTree();
    }

    public class ServicesService: IServicesService
    {
        public readonly AppDbContext _db;
        public ServicesService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CreateServiceResponse> CreateService(CreateServiceRequest request)
        {
            Service parent = null;
            if (!string.IsNullOrEmpty(request.ParentId))
                parent = await _db.Services.FindAsync(request.ParentId);
            
            if (!string.IsNullOrEmpty(request.ParentId) && parent == null)
                throw new InvalidOperationException("Выбранный родительский сервис не существует");
            
            var item = _db.Services.Add(Service.Create(request.Name, parent));
            
            await _db.SaveChangesAsync();

            return new CreateServiceResponse(item.Entity);
        }

        public async Task<DeleteServiceResponse> DeleteService(string serviceId)
        {
            Service item = null;
            
            item = await _db.Services.FindAsync(serviceId);
            
            if (item == null)
                throw new InvalidOperationException("Выбранный сервис не существует");
            
            var topics = await _db.Topics.Where(x => x.Service == item).ToListAsync();
            var topicIds = topics.Select(x => x.Id).ToHashSet();
            var messages = await _db.Messages.Where(x => topicIds.Contains(x.TopicId)).ToListAsync();

            _db.Messages.RemoveRange(messages);
            _db.Topics.RemoveRange(topics);
            _db.Services.Remove(item);
            
            await _db.SaveChangesAsync();

            return new DeleteServiceResponse(item);
        }

        public async Task<Root> GetServiceTree()
        {
            var result = await _db.Services.AsNoTracking().ToListAsync();
            return TreeService.GetTreeFromList(result.Select(x => new ServiceDto(x)));
        }
    }
}