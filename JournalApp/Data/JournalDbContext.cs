using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data
{
    public class JournalDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<EntryMood> EntryMoods { get; set; }
        public DbSet<EntryTag> EntryTags { get; set; }

        public JournalDbContext(DbContextOptions<JournalDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<JournalEntry>()
                .HasOne(e => e.PrimaryMood)
                .WithMany()
                .HasForeignKey(e => e.PrimaryMoodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntryMood>()
                .HasOne(em => em.JournalEntry)
                .WithMany(e => e.SecondaryMoods)
                .HasForeignKey(em => em.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EntryMood>()
                .HasOne(em => em.Mood)
                .WithMany()
                .HasForeignKey(em => em.MoodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntryTag>()
                .HasOne(et => et.JournalEntry)
                .WithMany(e => e.Tags)
                .HasForeignKey(et => et.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EntryTag>()
                .HasOne(et => et.Tag)
                .WithMany()
                .HasForeignKey(et => et.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one entry per day
            modelBuilder.Entity<JournalEntry>()
                .HasIndex(e => e.Date)
                .IsUnique();

            // Seed moods
            SeedMoods(modelBuilder);
            SeedTags(modelBuilder);
        }

        private void SeedMoods(ModelBuilder modelBuilder)
        {
            var moods = new List<Mood>
            {
                // Positive
                new Mood { Id = 1, Name = "Happy", Category = MoodCategory.Positive, Icon = "😊", Color = "#FFD700" },
                new Mood { Id = 2, Name = "Excited", Category = MoodCategory.Positive, Icon = "🤩", Color = "#FF6B6B" },
                new Mood { Id = 3, Name = "Relaxed", Category = MoodCategory.Positive, Icon = "😌", Color = "#4ECDC4" },
                new Mood { Id = 4, Name = "Grateful", Category = MoodCategory.Positive, Icon = "🙏", Color = "#95E1D3" },
                new Mood { Id = 5, Name = "Confident", Category = MoodCategory.Positive, Icon = "💪", Color = "#F38181" },
                
                // Neutral
                new Mood { Id = 6, Name = "Calm", Category = MoodCategory.Neutral, Icon = "😐", Color = "#A8E6CF" },
                new Mood { Id = 7, Name = "Thoughtful", Category = MoodCategory.Neutral, Icon = "🤔", Color = "#DCEDC1" },
                new Mood { Id = 8, Name = "Curious", Category = MoodCategory.Neutral, Icon = "🧐", Color = "#FFD3B6" },
                new Mood { Id = 9, Name = "Nostalgic", Category = MoodCategory.Neutral, Icon = "🥺", Color = "#FFAAA5" },
                new Mood { Id = 10, Name = "Bored", Category = MoodCategory.Neutral, Icon = "😑", Color = "#C7CEEA" },
                
                // Negative
                new Mood { Id = 11, Name = "Sad", Category = MoodCategory.Negative, Icon = "😢", Color = "#6C5CE7" },
                new Mood { Id = 12, Name = "Angry", Category = MoodCategory.Negative, Icon = "😠", Color = "#E74C3C" },
                new Mood { Id = 13, Name = "Stressed", Category = MoodCategory.Negative, Icon = "😰", Color = "#FDA7DF" },
                new Mood { Id = 14, Name = "Lonely", Category = MoodCategory.Negative, Icon = "😔", Color = "#95A5A6" },
                new Mood { Id = 15, Name = "Anxious", Category = MoodCategory.Negative, Icon = "😟", Color = "#F39C12" }
            };

            modelBuilder.Entity<Mood>().HasData(moods);
        }

        private void SeedTags(ModelBuilder modelBuilder)
        {
            var tags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Work", IsPreDefined = true },
                new Tag { Id = 2, Name = "Career", IsPreDefined = true },
                new Tag { Id = 3, Name = "Studies", IsPreDefined = true },
                new Tag { Id = 4, Name = "Family", IsPreDefined = true },
                new Tag { Id = 5, Name = "Friends", IsPreDefined = true },
                new Tag { Id = 6, Name = "Relationships", IsPreDefined = true },
                new Tag { Id = 7, Name = "Health", IsPreDefined = true },
                new Tag { Id = 8, Name = "Fitness", IsPreDefined = true },
                new Tag { Id = 9, Name = "Personal Growth", IsPreDefined = true },
                new Tag { Id = 10, Name = "Self-care", IsPreDefined = true },
                new Tag { Id = 11, Name = "Hobbies", IsPreDefined = true },
                new Tag { Id = 12, Name = "Travel", IsPreDefined = true },
                new Tag { Id = 13, Name = "Nature", IsPreDefined = true },
                new Tag { Id = 14, Name = "Finance", IsPreDefined = true },
                new Tag { Id = 15, Name = "Spirituality", IsPreDefined = true },
                new Tag { Id = 16, Name = "Birthday", IsPreDefined = true },
                new Tag { Id = 17, Name = "Holiday", IsPreDefined = true },
                new Tag { Id = 18, Name = "Vacation", IsPreDefined = true },
                new Tag { Id = 19, Name = "Celebration", IsPreDefined = true },
                new Tag { Id = 20, Name = "Exercise", IsPreDefined = true },
                new Tag { Id = 21, Name = "Reading", IsPreDefined = true },
                new Tag { Id = 22, Name = "Writing", IsPreDefined = true },
                new Tag { Id = 23, Name = "Cooking", IsPreDefined = true },
                new Tag { Id = 24, Name = "Meditation", IsPreDefined = true },
                new Tag { Id = 25, Name = "Yoga", IsPreDefined = true },
                new Tag { Id = 26, Name = "Music", IsPreDefined = true },
                new Tag { Id = 27, Name = "Shopping", IsPreDefined = true },
                new Tag { Id = 28, Name = "Parenting", IsPreDefined = true },
                new Tag { Id = 29, Name = "Projects", IsPreDefined = true },
                new Tag { Id = 30, Name = "Planning", IsPreDefined = true },
                new Tag { Id = 31, Name = "Reflection", IsPreDefined = true }
            };

            modelBuilder.Entity<Tag>().HasData(tags);
        }
    }
}
