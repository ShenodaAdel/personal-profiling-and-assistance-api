using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class QuestionChoice
    {
        public int Id { get; set; }
        public int? QuestionId { get; set; }
        public Question? Question { get; set; }
        public int? ChoiceId { get; set; }
        public Choice? Choice { get; set; }
    }
}
