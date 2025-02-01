using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(200)]
        public required string Email { get; set; }
        [MaxLength(500)]
        public required string Password { get; set; } // Hashed
        [MaxLength(5 * 1024 * 1024)] // 5MB
        public byte[]? ProfilePicture { get; set; }
        [MaxLength(1)]
        public char Gender { get; set; } // M or F
        [MaxLength(11)]
        public required string Phone { get; set; }
        public ICollection<UserTest>? UserTests { get; set; }

        

    }
}
