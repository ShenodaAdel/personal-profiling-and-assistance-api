using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.User.Dtos
{
    public class UserAddDto
    {
        public string? Name { get; set; }

        public string? Role { get; set; } = "User"; // Admin, User
        public string? Email { get; set; }
        public string? Password { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? Gender { get; set; }

        public string? Phone { get; set; }
    }
}
