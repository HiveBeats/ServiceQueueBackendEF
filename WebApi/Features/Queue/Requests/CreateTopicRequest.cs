using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Queue.Requests
{
    public class CreateTopicRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ServiceId { get; set; }
        [Required]
        public bool SolveByReading { get; set; }
    }
}