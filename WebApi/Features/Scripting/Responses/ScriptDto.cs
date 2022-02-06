using System;
using WebApi.Models;

namespace WebApi.Features.Scripting.Responses
{
    public class ScriptDto
    {
        public ScriptDto(Script source)
        {
            Id = source.Id;
            Name = source.Name;
            Body = source.Body;
            Priority = source.Priority;
            IsEnabled = source.IsEnabled;
            LastModified = source.DateModified;
            Created = source.DateCreated;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public int Priority { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime Created { get; set; }

        public ScriptDto ClearDetails()
        {
            Body = null;
            return this;
        }
    }
}