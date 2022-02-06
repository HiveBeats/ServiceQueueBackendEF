using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Scripting.Requests
{
    public class CreateScriptOnTopicRequest
    {
        [Required]
        public long TopicId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Body { get; set; }

        public int Priority { get; set; } = 0;
    }
}