using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Question.Dtos
{
    public class QuestionAddWithChoicesDto
    {
        public string? Content { get; set; }

        public List<string>? Choices { get; set; }  // List of 4 choices
    }
}
