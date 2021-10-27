namespace WebApi.Features.Users.Responses
{
    public class RegisterResponse
    {
        public string UserName { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}