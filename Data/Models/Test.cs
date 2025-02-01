using System;
using System.Collections.Generic;
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

        public ICollection<UserTest>? UserTests { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
