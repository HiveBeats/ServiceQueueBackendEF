using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Users.Requests
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}