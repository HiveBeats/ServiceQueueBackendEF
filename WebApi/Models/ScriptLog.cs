using System;

namespace WebApi.Models
{
    public class ScriptLog
    {
        public long Id { get; private set; }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }
        public int LogLevel { get; private set; } //todo: enum?
        
        public Script Script { get; private set; }
        public long ScriptId { get; private set; }
    }
}