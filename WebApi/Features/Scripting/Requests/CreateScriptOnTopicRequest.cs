namespace WebApi.Features.Scripting.Requests
{
    public class CreateScriptOnTopicRequest
    {
        public long TopicId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public int Priority { get; set; }
    }
}