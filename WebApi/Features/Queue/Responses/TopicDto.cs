using WebApi.Models;

namespace WebApi.Features.Queue.Responses
{
    public class TopicDto
    {
        public TopicDto(Topic source)
        {
            Id = source.Id;
            Name = source.Name;
            ServiceId = source.ServiceId;
            SolveByReading = source.SolveByReading;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long ServiceId { get; set; }
        public bool SolveByReading { get; set; }
    }
}