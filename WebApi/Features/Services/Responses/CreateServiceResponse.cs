using WebApi.Models;

namespace WebApi.Features.Services.Responses
{
    public class CreateServiceResponse
    {
        public CreateServiceResponse(Service source)
        {
            Id = source.Id;
            Name = source.Name;
            ParentId = source.ParentId;
        }
        
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
    }

    public class DeleteServiceResponse
    {
        public DeleteServiceResponse(Service source)
        {
            Id = source.Id;
            Name = source.Name;
            ParentId = source.ParentId;
        }
        
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
    }


}