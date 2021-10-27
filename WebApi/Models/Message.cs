using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Message
    {
        public long Id { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateSolved { get; private set; }
        
        [MaxLength(2048)]
        public string MessagePayload { get; private set; }
        
        [MaxLength(128)]
        public string CorrelationId { get; private set; } //ответом на какой изначальный реквест, является сообщение

        public Topic Topic { get; set; }
        public long TopicId { get; set; }

        public static Message Create(Topic topic, string messagePayload, string correlation)
        {
            return new Message()
            {
                Topic = topic,
                DateCreated = DateTime.UtcNow,
                MessagePayload = messagePayload,
                CorrelationId = correlation
            };
        }

        public bool Solve()
        {
            if (DateSolved.HasValue)
                return false;
            
            DateSolved = DateTime.UtcNow;
            return true;
        }

        public string ReadMessage(Topic topic = null)
        {
            if (!DateSolved.HasValue && (topic?.SolveByReading ?? Topic.SolveByReading))
                DateSolved = DateTime.UtcNow;

            return MessagePayload;
        }
    }
}