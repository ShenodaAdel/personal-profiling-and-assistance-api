﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class ResultDto
     {
        public object? Data { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
