using System;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Features.Users
{
    public class RefreshTokenTests
    {   
        [Fact]
        public void CtorGeneratesTokenString()
        {
            //act
            var item  = new RefreshToken(Guid.NewGuid().ToString());

            //assert
            Assert.NotNull(item.TokenString);
        }

        [Fact]
        public void IsExpiredOnExpired()
        {
            //arrange
            var item = new RefreshToken(Guid.NewGuid().ToString());
            var expectedExpireDate = DateTime.UtcNow.AddMinutes(10);

            //act
            var isExpired = item.IsExpired(expectedExpireDate.AddMinutes(1));

            //assert
            Assert.True(isExpired);
        }

        [Fact]
        public void NotExpiredOnNotExpired()
        {
            //arrange
            var item = new RefreshToken(Guid.NewGuid().ToString());
            var expectedExpireDate = DateTime.UtcNow.AddMinutes(10);

            //act
            var isExpired = item.IsExpired(expectedExpireDate.AddMinutes(-1));

            //assert
            Assert.False(isExpired);
        }
    }
}