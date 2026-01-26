using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public enum MoodCategory
    {
        Positive,
        Neutral,
        Negative
    }

    public class Mood
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public MoodCategory Category { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; } // Emoji or icon name

        [MaxLength(7)]
        public string? Color { get; set; } // Hex color code
    }
}