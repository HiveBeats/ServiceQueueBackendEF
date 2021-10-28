namespace WebApi.Features.Queue.Requests
{
    public class CreateTopicRequest
    {
        public string Name { get; set; }
        public string ServiceId { get; set; }
        public bool SolveByReading { get; set; }
    }
}