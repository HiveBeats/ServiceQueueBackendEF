using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Queue.Requests
{
    public class CreateTopicRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public long ServiceId { get; set; }
        [Required]
        public bool SolveByReading { get; set; }
    }
}