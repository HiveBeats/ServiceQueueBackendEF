namespace WebApi.Features.Queue.Requests
{
    public class PushMessageOnTopicRequest
    {
        public long TopicId { get; set; }
        public string MessagePayload { get; set; }
        public string CorrelationId { get; set; }
    }
}