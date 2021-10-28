using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Services.Requests;
using WebApi.Features.Services.Responses;
using WebApi.Features.Services.Services;
using WebApi.Features.Tree.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ServiceController: ControllerBase
    {
        private readonly IServicesService _services;
        public ServiceController(IServicesService services)
        {
            _services = services;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<CreateServiceResponse>> CreateService([FromBody]CreateServiceRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest("Incorrect input");

            var result = await _services.CreateService(request);

            return Ok(result);
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteService(long id)
        {
            var result = await _services.DeleteService(id);
            return NoContent();
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<Root>> GetServices()
        {
            var result = await _services.GetServiceTree();
            return Ok(result);
        }


    }
}