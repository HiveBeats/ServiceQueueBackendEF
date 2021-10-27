using System;
using System.Security.Cryptography;

namespace WebApi.Models
{
    public class RefreshToken
    {
        protected RefreshToken() {}

        private RefreshToken(string userId, string tokenString)
        {
            AppUserId = userId;
            TokenString = tokenString;
        }

        public RefreshToken(string userId, DateTime? currentTime = null, int? tokenExpirationInMinutes = null)
        {
            AppUserId = userId;
            TokenString = GenerateRefreshTokenString();
            ExpireAt = CalculateExpireDate(currentTime).AddMinutes(tokenExpirationInMinutes ?? 10);
        }

        public string AppUserId { get; private set; }
        public AppUser AppUser { get; private set; }  
        public string TokenString { get; private set; }
        public DateTime ExpireAt { get; private set; }

        public bool IsExpired(DateTime current)
        {
            return ExpireAt < current;
        }

        public static RefreshToken Create(string userId, string refreshToken)
        {
            return new RefreshToken(userId, refreshToken);
        }
        
        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        private DateTime CalculateExpireDate(DateTime? currentTime = null)
        {
            return currentTime ?? DateTime.UtcNow;
        }
    }
}