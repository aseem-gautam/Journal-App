using System;
using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(6)]
        public string PIN { get; set; } = string.Empty; // 4-6 digit PIN

        public DateTime CreatedAt { get; set; }

        public bool IsDarkMode { get; set; }

        public bool HasSetupCompleted { get; set; } // First time setup flag
    }
}