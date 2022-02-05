using System;

namespace WebApi.Models
{
    public enum LogLevel: int
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }
    public class ScriptLog
    {
        public long Id { get; private set; }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public Script Script { get; private set; }
        public long ScriptId { get; private set; }

        public static ScriptLog Create(Script script, LogLevel logLevel, string message)
        {
            return new ScriptLog()
            {
                Time = DateTime.UtcNow,
                Script = script,
                LogLevel = logLevel,
                Message = message
            };
        }
    }
}