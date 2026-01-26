using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public bool IsPreDefined { get; set; }

        [MaxLength(7)]
        public string? Color { get; set; }
    }
}