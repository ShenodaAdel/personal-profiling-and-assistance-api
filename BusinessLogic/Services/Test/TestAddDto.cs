using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Test
{
    public class TestAddDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? TestImage { get; set; }
    }
}
