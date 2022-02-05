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
        
        public Topic Topic { get; private set; }
        public long TopicId { get; private set; }
        
        public ICollection<ScriptLog> ScriptLogs { get; private set; }

        public static Script Create(Topic topic, string name, string body, int priority)
        {
            var dateCreated = DateTime.UtcNow;
            return new Script()
            {
                Name = name,
                Body = body,
                Priority = priority,
                IsEnabled = true,
                DateCreated = dateCreated,
                DateModified = dateCreated,
                Topic = topic
            };
        }

        public void ToggleEnabled()
        {
            IsEnabled = !IsEnabled;
            DateModified = DateTime.UtcNow;
        }

        public void UpdateBody(string body)
        {
            Body = body;
            DateModified = DateTime.UtcNow;
        }

        public void UpdatePriority(int priority)
        {
            Priority = priority;
            DateModified = DateTime.UtcNow;
        }
    }
}