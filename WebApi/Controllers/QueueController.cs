using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Queue.Requests;
using WebApi.Features.Queue.Responses;
using WebApi.Features.Queue.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class QueueController: ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly IMessageService _messageService;

        public QueueController(ITopicService topicService, IMessageService messageService)
        {
            _topicService = topicService;
            _messageService = messageService;
        }

        [HttpGet]
        [Route("Topic")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetServiceTopics([FromQuery]long serviceId)
        {
            var result = await _topicService.GetServiceTopics(serviceId);
            return Ok(result);
        }

        [HttpPost]
        [Route("Topic")]
        public async Task<ActionResult<TopicDto>> CreateTopic([FromBody]CreateTopicRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Incorrect input");
            
            var result = await _topicService.CreateTopic(request);

            return Ok(result);
        }

        [HttpDelete]
        [Route("Topic/{id:long}")]
        public async Task<IActionResult> DeleteTopic(long id)
        {
            var result = await _topicService.DeleteTopic(id);
            return NoContent();
        }

        [HttpPost]
        [Route("{topic:long}")]
        public async Task<IActionResult> PushMessage(long topic, [FromBody]PushMessageRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Incorrect input");
            
            var result = await _messageService.PushMessageOnTopic(new PushMessageOnTopicRequest(topic, request));
            
            return Ok();
        }

        [HttpGet]
        [Route("{topic:long}")]
        public async Task<ActionResult<MessageDto>> PopMessage(long topic)
        {
            var result = await _messageService.GetLastMessageOnTopic(topic);
            return Ok(result);
        }
    }
}