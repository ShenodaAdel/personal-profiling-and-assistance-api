using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services.User.DTOs
{
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
