using System;
using WebApi.Models;

namespace WebApi.Features.Queue.Responses
{
    public class MessageDto
    {
        public MessageDto(Message source)
        {
            Id = source.Id;
            DateCreated = source.DateCreated;
            DateSolved = source.DateSolved;
            MessagePayload = source.MessagePayload;
            CorrelationId = source.CorrelationId;
            TopicId = source.TopicId;
        }

        public long Id { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateSolved { get; private set; }
        public string MessagePayload { get; private set; }
        public string CorrelationId { get; private set; } //ответом на какой изначальный реквест, является сообщение
        public long TopicId { get; set; }
    }
}