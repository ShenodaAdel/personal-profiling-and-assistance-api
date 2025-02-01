using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Choice
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public required string Content { get; set; }
        public ICollection<QuestionChoice>? QuestionChoices { get; set; }
    }
}
