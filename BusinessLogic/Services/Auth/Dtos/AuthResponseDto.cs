namespace BusinessLogic.Services.Auth.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }

        public string UserId { get; set; }
    }
}
