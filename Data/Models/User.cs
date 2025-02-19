using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]

        // i need to add migration in home 18 / 9 2025 
        [MaxLength(50)]
        public string Role { get; set; } = "User"; // Admin, User
        [MaxLength(100)]
        public required string Name
        { get; set; }
        [MaxLength(200),EmailAddress]
        public required string Email { get; set; }
        [MaxLength(500)]
        public required string Password { get; set; } // Hashed
        [MaxLength(5 * 1024 * 1024)] // 5MB
        public byte[]? ProfilePicture { get; set; }
        [MaxLength(50)]
        public string? Gender { get; set; } // M or F
        [MaxLength(11)]
        public string? Phone { get; set; }
        public ICollection<UserTest>? UserTests { get; set; }

        

    }
}
