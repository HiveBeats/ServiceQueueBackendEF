using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Users.Requests
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}