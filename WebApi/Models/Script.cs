using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Script
    {
        public long Id { get; private set; }
        [Required]
        [MaxLength(128)]
        public string Name { get; private set; }
        [Required]
        public string Body { get; private set; }
        public int Priority { get; private set; }
        public bool IsEnabled { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime DateModified { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public Topic Topic { get; private set; }
        public long TopicId { get; private set; }
        
        public ICollection<ScriptLog> ScriptLogs { get; private set; }

        public static Script Create(Topic topic, string name, string body, int priority)
        {
            if (topic == null)
                throw new InvalidOperationException("Can't create script without topic");
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Can't create script with empty name");
            if (body == null)
                throw new ArgumentNullException("body");
            
            var dateCreated = DateTime.UtcNow;
            return new Script()
            {
                Name = name,
                Body = body,
                Priority = priority,
                IsEnabled = true,
                LogLevel = LogLevel.Warning,
                DateCreated = dateCreated,
                DateModified = dateCreated,
                Topic = topic
            };
        }

        public void UpdateSettings(bool isEnabled, LogLevel logLevel, int priority)
        {
            IsEnabled = isEnabled;
            LogLevel = logLevel;
            Priority = priority;
            DateModified = DateTime.UtcNow;
        }
        
        public void UpdateBody(string body)
        {
            if (body == null)
                throw new ArgumentNullException("body");
            
            Body = body;
            DateModified = DateTime.UtcNow;
        }
    }
}