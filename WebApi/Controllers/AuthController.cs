using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Features.Users.Requests;
using WebApi.Features.Users.Responses;
using WebApi.Features.Users.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody]RegisterRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Bad credentials");
            
            var result = await _authService.Register(request);

            return Ok(result);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest credentials)
        {
            if (!ModelState.IsValid || credentials == null)
                return Unauthorized("Bad credentials");

            LoginResponse result = null;
            try
            {
                result = await _authService.Login(credentials);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            
            return Ok(result);
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult<LoginResponse> GetCurrentUser()
        {
            return Ok(new LoginResponse
            {
                UserName = User.Identity.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest("Incorrect refresh-token request");

            LoginResponse result = null;
            try
            {
                result = await _authService.RefreshToken(request);
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            return Ok(result);
        }
    }
}