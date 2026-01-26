using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalApp.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } // Only date part used for "one per day"

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty; // Markdown/Rich text

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Mood tracking
        [Required]
        public int PrimaryMoodId { get; set; }

        [ForeignKey("PrimaryMoodId")]
        public Mood PrimaryMood { get; set; } = null!;

        // Navigation properties for secondary moods
        public List<EntryMood> SecondaryMoods { get; set; } = new();

        // Tags
        public List<EntryTag> Tags { get; set; } = new();

        [MaxLength(100)]
        public string? Category { get; set; }

        public int WordCount { get; set; }
    }

    // Junction table for many-to-many relationship with secondary moods
    public class EntryMood
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int MoodId { get; set; }

        public JournalEntry JournalEntry { get; set; } = null!;
        public Mood Mood { get; set; } = null!;
    }

    // Junction table for tags
    public class EntryTag
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int TagId { get; set; }

        public JournalEntry JournalEntry { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}