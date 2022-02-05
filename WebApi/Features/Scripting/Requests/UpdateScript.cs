namespace WebApi.Features.Scripting.Requests
{
    public class UpdateScriptBodyRequest
    {
        public long Id { get; set; }
        public string Body { get; set; }
    }

    public class UpdateScriptPriorityRequest
    {
        public long Id { get; set; }
        public int Priority { get; set; }
    }
}