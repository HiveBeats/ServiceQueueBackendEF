namespace WebApi.Features.Queue.Requests
{
    public class PushMessageRequest
    {
        public string MessagePayload { get; set; }
        public string CorrelationId { get; set; }
    }

    public class PushMessageOnTopicRequest
    {
        public PushMessageOnTopicRequest(long topic, PushMessageRequest request)
        {
            TopicId = topic;
            MessagePayload = request.MessagePayload;
            CorrelationId = request.CorrelationId;
        }
        
        public long TopicId { get; set; }
        public string MessagePayload { get; set; }
        public string CorrelationId { get; set; }
    }
}