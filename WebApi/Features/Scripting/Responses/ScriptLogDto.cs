using System;
using WebApi.Models;

namespace WebApi.Features.Scripting.Responses
{
    public class ScriptLogDto
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public LogLevel LogLevel { get; set; }//todo: as string?
    }
}