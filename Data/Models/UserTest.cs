using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class UserTest
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } 

        [Column(TypeName = "NVARCHAR(MAX)")] 
        public required string Result { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? TestId { get; set; }
        public Test? Test { get; set; }
    }
}
