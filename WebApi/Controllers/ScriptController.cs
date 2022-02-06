using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Scripting.Requests;
using WebApi.Features.Scripting.Responses;
using WebApi.Features.Scripting.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ScriptController: ControllerBase
    {
        private readonly IScriptService _scriptService;
        public ScriptController(IScriptService scriptService)
        {
            _scriptService = scriptService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptDto>>> GetTopicScripts([FromQuery]long topicId)
        {
            return Ok(await _scriptService.GetTopicScripts(topicId));
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ScriptDto>> GetScript(long id)
        {
            return Ok(await _scriptService.GetScript(id));
        }
        
        [HttpPut]
        public async Task<ActionResult<ScriptDto>> CreateScript([FromBody]CreateScriptOnTopicRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Incorrect input");

            var result = await _scriptService.CreateScriptForTopic(request);
            return Ok(result);
        }
        
        [HttpPost("{id:long}/Settings")]
        public async Task<ActionResult<ScriptDto>> UpdateScriptSettings(long id, [FromBody]UpdateScriptSettingsRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Incorrect input");

            var result = await _scriptService.UpdateScriptSettings(id, request);
            return Ok(result);
        }
        
        [HttpPost("{id:long}/Body")]
        public async Task<ActionResult<ScriptDto>> UpdateScriptBody(long id, [FromBody] UpdateScriptBodyRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Incorrect input");

            var result = await _scriptService.UpdateScriptBody(id, request);
            return Ok(result);
        }

        [HttpGet("{id:long}/Logs")]
        public async Task<ActionResult<IEnumerable<ScriptLogDto>>> GetScriptLogs(long id)
        {
            return Ok(await _scriptService.GetScriptLogs(id));
        }
    }
}