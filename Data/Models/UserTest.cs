﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class UserTest
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(100)]
        public required string Result { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? TestId { get; set; }
        public Test? Test { get; set; }
    }
}
