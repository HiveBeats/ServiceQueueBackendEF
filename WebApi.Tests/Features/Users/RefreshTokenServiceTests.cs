using Microsoft.EntityFrameworkCore;
using WebApi.Configuration;
using WebApi.Models;
using WebApi.Features.Users.Services;
using WebApi.Features.Users.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Tests.Features.Users
{

    public class RefreshTokenServiceTests
    {
        private readonly ServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;

        public RefreshTokenServiceTests()
        {
            _services = new ServiceCollection();

            _services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "RefreshTokenInMemoryDb"), 
               ServiceLifetime.Scoped, 
               ServiceLifetime.Scoped);

            _services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppDbContext>();
            
            _services.AddTransient<IOptions<JwtTokenConfig>>(
                provider => Options.Create<JwtTokenConfig>(new JwtTokenConfig()
                {
                    Secret = "Some128bitSecretSome128bitSecret",
                    Issuer= "https://mywebapi.com",
                    Audience = "https://mywebapi.com",
                    AccessTokenExpiration = 1,
                    RefreshTokenExpiration = 2
                }));
            
            _services.AddScoped<IRefreshTokenService, RefreshTokenService>();


            _serviceProvider = _services.BuildServiceProvider();
        }

        private async Task<JwtAuthResult> NormalGetToken(IRefreshTokenService service, System.Security.Claims.Claim nameClaim, DateTime nowDate)
        {
            var result = await service.GenerateTokens(Guid.NewGuid().ToString(), 
                new System.Security.Claims.Claim[] 
                {
                    nameClaim
                }, nowDate);
                
            return result;
        }

        [Fact]
        public async Task GenerateTokensNotNull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange        
                var service = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
                var nowDate = DateTime.UtcNow;
                var nameClaim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "John Grave");
                
                //act   
                var result = await NormalGetToken(service, nameClaim, nowDate);
                
                //assert
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task DecodeJwtTokenPrincipalExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange        
                var service = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
                var nowDate = DateTime.UtcNow;
                var nameClaim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "John Grave");
                
                var result = await NormalGetToken(service, nameClaim, nowDate);
                
                //act
                var (principal, jwtToken) = service.DecodeJwtToken(result.AccessToken, false);
                
                //assert
                Assert.NotNull(principal);
            }
        }

        [Fact]
        public async Task DecodeJwtTokenJwtExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange        
                var service = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
                var nowDate = DateTime.UtcNow;
                var nameClaim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "John Grave");
                
                var result = await NormalGetToken(service, nameClaim, nowDate);
                
                //act
                var (principal, jwtToken) = service.DecodeJwtToken(result.AccessToken, false);
                
                //assert
                Assert.NotNull(jwtToken);
            }
        }

        [Fact]
        public async Task DecodeJwtTokenHasIncludedClaim()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange        
                var service = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
                var nowDate = DateTime.UtcNow;
                var nameClaim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "John Grave");
                
                var result = await NormalGetToken(service, nameClaim, nowDate);
                
                //act
                var (principal, jwtToken) = service.DecodeJwtToken(result.AccessToken, false);
                
                //assert
                Assert.NotNull(principal.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name));
            }
        }

        [Fact]
        public async Task DecodeJwtTokenExpiredNotValid()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange        
                var service = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
                var nowDate = DateTime.UtcNow.AddHours(-1);
                var nameClaim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "John Grave");
                
                var result = await NormalGetToken(service, nameClaim, nowDate);
                
                //act && assert
                Assert.ThrowsAny<Exception>(() => service.DecodeJwtToken(result.AccessToken, true));
            }
        }

        [Fact]
        public async Task GenerateTokensClaimValidated()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange        
                var service = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
                var nowDate = DateTime.UtcNow;
                var name = "John Grave";
                var nameClaim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, name);
                
                var result = await NormalGetToken(service, nameClaim, nowDate);
                
                //act
                var (principal, jwtToken) = service.DecodeJwtToken(result.AccessToken, false);
                
                //assert
                Assert.Equal(name, principal.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name).Value);
            }
        }
    }
}