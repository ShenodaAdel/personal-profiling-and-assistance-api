using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public required string Name { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public required string Description { get; set; }

        [MaxLength(5 * 1024 * 1024)] 
        public required byte[] TestImage { get; set; }

        public ICollection<UserTest>? UserTests { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
