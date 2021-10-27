using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Users.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
    }
}