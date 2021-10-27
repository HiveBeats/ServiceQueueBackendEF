using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
namespace WebApi.Models
{
    public class AppUser: IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}