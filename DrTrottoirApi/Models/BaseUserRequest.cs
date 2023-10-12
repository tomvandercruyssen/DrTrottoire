﻿using System.ComponentModel.DataAnnotations;
using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Models
{
    public class BaseUserRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? TelephoneNumber { get; set; }
        public string? PictureUrl { get; set; }

        [Required]
        public Guid WorkAreaId { get; set; }
    }
}
