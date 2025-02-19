using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.UserTest.Dtos
{
    public class UserTestDto
    {
        public DateTime Date { get; set; }
        public string Result { get; set; } = default!;
        public int? UserId { get; set; }
        public int? TestId { get; set; }


    }
}
