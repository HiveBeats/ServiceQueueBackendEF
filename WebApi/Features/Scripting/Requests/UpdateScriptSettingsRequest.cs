using WebApi.Models;

namespace WebApi.Features.Scripting.Requests
{
    public class UpdateScriptSettingsRequest
    {
        public long Id { get; set; }
        public bool IsEnabled { get; set; }
        public LogLevel LogLevel { get; set; }
        public int Priority { get; set; }
    }
    
    public class UpdateScriptBodyRequest
    {
        public long Id { get; set; }
        public string Body { get; set; }
    }
}