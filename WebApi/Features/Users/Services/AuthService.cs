using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using WebApi.Configuration;
using WebApi.Features.Users.Requests;
using WebApi.Features.Users.Responses;
using WebApi.Models;

namespace WebApi.Features.Users.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<LoginResponse> Login(LoginRequest credentials);
        Task Logout();
        Task<LoginResponse> RefreshToken(RefreshTokenRequest request);
    }
    public class AuthService: IAuthService
    {
        private readonly IOptions<JwtTokenConfig> _jwtTokenConfig;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRefreshTokenService _tokenService;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(IOptions<JwtTokenConfig> jwtTokenConfig, UserManager<AppUser> userManager, IRefreshTokenService tokenService, AppDbContext db, IHttpContextAccessor contextAccessor)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _userManager = userManager;
            _tokenService = tokenService;
            _db = db;
            _contextAccessor = contextAccessor;
        }



        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var identityUser = new AppUser() { UserName = request.UserName, Email = request.Email };
            var result = await _userManager.CreateAsync(identityUser, request.Password);

            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)
                    dictionary.AddModelError(error.Code, error.Description);

                throw new Exception($"Failed: {string.Join(";", dictionary.Select(e => $"{e.Key}: {e.Value}"))}");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.UserName)
                //todo: role claim
            };
            var jwtResult = await _tokenService.GenerateTokens(identityUser.Id, claims, DateTime.UtcNow);
            
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return new RegisterResponse
            {
                UserName = request.UserName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }

        public async Task<LoginResponse> Login(LoginRequest credentials)
        {
            IdentityUser user;
            if ((user = await ValidateUser(credentials)) == null)
                throw new UnauthorizedAccessException();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, credentials.UserName)
                //todo: role claim
            };
            var jwtResult = await _tokenService.GenerateTokens(user.Id, claims, DateTime.UtcNow);
            
            return new LoginResponse
            {
                UserName = credentials.UserName,
                Role = string.Empty,//todo: role
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }

        public async Task Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = _contextAccessor.HttpContext.User.Identity.Name;
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser != null)
                await _tokenService.RemoveRefreshTokenByUserId(identityUser.Id);
        }

        private async Task<AppUser> ValidateUser(LoginRequest credentials)
        {
            AppUser identityUser = null;
            if (string.IsNullOrWhiteSpace(credentials.UserName))
                identityUser = await _userManager.FindByEmailAsync(credentials.Email);
            else identityUser = await _userManager.FindByNameAsync(credentials.UserName);
            
            if (identityUser != null)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, credentials.Password);
                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }

            return null;
        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenRequest request)
        {
            var userName = _contextAccessor.HttpContext.User.Identity.Name ?? request.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                throw new UnauthorizedAccessException("UserName not provided for refresh-token");
            }

            var identityUser = await _userManager.FindByNameAsync(userName);
            if (string.IsNullOrWhiteSpace(request.RefreshToken) || identityUser == null)
                throw new UnauthorizedAccessException();

            var accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            var jwtResult = await _tokenService.Refresh(request.RefreshToken, accessToken, DateTime.UtcNow, identityUser.Id);

            return new LoginResponse
            {
                UserName = userName,
                Role = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }
    }
}