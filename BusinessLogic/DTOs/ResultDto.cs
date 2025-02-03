using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class ResultDto
    {
        public object? Result { get; set; }
        public bool NotFound { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
