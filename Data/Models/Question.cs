using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(600)")]
        public required string Content { get; set; } 

        public int? TestId { get; set; }
        public Test? Test { get; set; }

        public ICollection<QuestionChoice>? QuestionChoices { get; set; }
    }
}
