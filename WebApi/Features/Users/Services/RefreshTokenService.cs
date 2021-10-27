using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Configuration;
using WebApi.Features.Users.Responses;
using WebApi.Models;

namespace WebApi.Features.Users.Services
{
    public interface IRefreshTokenService
    {
        Task<JwtAuthResult> GenerateTokens(string userId, Claim[] claims, DateTime now);
        Task<JwtAuthResult> Refresh(string refreshToken, string accessToken, DateTime now, string userId);
        Task RemoveRefreshTokenByUserId(string userId);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token, bool validateLifetime = true);
    }

    public class RefreshTokenService: IRefreshTokenService
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly AppDbContext _db;
        private readonly byte[] _secret;

        public RefreshTokenService(IOptions<JwtTokenConfig> jwtTokenConfig, AppDbContext db)
        {
            _jwtTokenConfig = jwtTokenConfig.Value;
            _db = db;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Value.Secret);
        }

        private async Task UpsertRefreshToken(RefreshToken token)
        {
            var existingToken = await _db.RefreshTokens.FindAsync(token.AppUserId, token.TokenString);
            if (existingToken == null)
                await _db.RefreshTokens.AddAsync(token);
            else _db.RefreshTokens.Update(token);

            await _db.SaveChangesAsync();
        }

        public async Task<JwtAuthResult> GenerateTokens(string userId, Claim[] claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new RefreshToken(userId, now, _jwtTokenConfig.RefreshTokenExpiration);
           
            await UpsertRefreshToken(refreshToken);

            return new JwtAuthResult()
            {
                AccessToken = accessToken,
                RefreshToken = new RefreshTokenDto(refreshToken)
            };
        }

        public async Task<JwtAuthResult> Refresh(string refreshToken, string accessToken, DateTime now, string userId)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken, false);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = principal.Identity.Name;
            var existingRefreshToken = await _db.RefreshTokens.FindAsync(userId, refreshToken);
            var user = await _db.Users.FindAsync(userId);
            if (existingRefreshToken == null)
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (user.UserName != userName || existingRefreshToken.IsExpired(now))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return await GenerateTokens(userId, principal.Claims.ToArray(), now); // need to recover the original claims
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token, bool validateLifetime = true)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = validateLifetime,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        public async Task RemoveRefreshTokenByUserId(string userId)
        {
            var refreshTokens = _db.RefreshTokens.Where(t => t.AppUserId == userId).ToListAsync();
            _db.RefreshTokens.RemoveRange(await refreshTokens);
            
            await _db.SaveChangesAsync();
        }
    }
}