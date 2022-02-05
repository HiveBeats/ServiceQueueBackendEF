using System;

namespace WebApi.Features.Scripting.Responses
{
    public class ScriptDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public int Priority { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime Created { get; set; }
    }
}