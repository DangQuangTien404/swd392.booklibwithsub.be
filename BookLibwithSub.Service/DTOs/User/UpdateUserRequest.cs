﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibwithSub.Service.Models.User
{
    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? Password { get; set; }
    }
}

