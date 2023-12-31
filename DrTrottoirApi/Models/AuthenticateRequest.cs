﻿using Microsoft.Build.Framework;

namespace DrTrottoirApi.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
