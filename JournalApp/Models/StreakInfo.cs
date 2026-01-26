using System;
using System.Collections.Generic;

namespace JournalApp.Models
{
    public class StreakInfo
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalEntries { get; set; }
        public List<DateTime> MissedDays { get; set; } = new();
        public DateTime? LastEntryDate { get; set; }
    }
}
