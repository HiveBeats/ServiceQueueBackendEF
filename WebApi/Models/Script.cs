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
    }
}