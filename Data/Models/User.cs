using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class ApplicationUser : IdentityUser
    {

        [MaxLength(5 * 1024 * 1024)] // 5MB
        public byte[]? ProfilePicture { get; set; }

        [MaxLength(50)]
        public string? Gender { get; set; } // M or F
        public ICollection<UserTest>? UserTests { get; set; }

        

    }
}
