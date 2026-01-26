using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class StreakService
    {
        private readonly JournalDbContext _context;

        public StreakService(JournalDbContext context)
        {
            _context = context;
        }

        // Calculate streak information
        public async Task<StreakInfo> GetStreakInfoAsync()
        {
            try
            {
                var allEntries = await _context.JournalEntries
                    .OrderBy(e => e.Date)
                    .Select(e => e.Date.Date)
                    .ToListAsync();

                if (!allEntries.Any())
                {
                    return new StreakInfo
                    {
                        CurrentStreak = 0,
                        LongestStreak = 0,
                        TotalEntries = 0,
                        MissedDays = new List<DateTime>()
                    };
                }

                var today = DateTime.Today;
                var firstEntryDate = allEntries.First();
                var lastEntryDate = allEntries.Last();

                // Calculate current streak
                int currentStreak = CalculateCurrentStreak(allEntries, today);

                // Calculate longest streak
                int longestStreak = CalculateLongestStreak(allEntries);

                // Calculate missed days (from first entry to today)
                var missedDays = CalculateMissedDays(allEntries, firstEntryDate, today);

                return new StreakInfo
                {
                    CurrentStreak = currentStreak,
                    LongestStreak = longestStreak,
                    TotalEntries = allEntries.Count,
                    MissedDays = missedDays,
                    LastEntryDate = lastEntryDate
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating streak: {ex.Message}");
                return new StreakInfo();
            }
        }

        private int CalculateCurrentStreak(List<DateTime> entryDates, DateTime today)
        {
            if (!entryDates.Any())
                return 0;

            // Check if there's an entry today or yesterday
            if (!entryDates.Contains(today) && !entryDates.Contains(today.AddDays(-1)))
                return 0;

            int streak = 0;
            var currentDate = today;

            // Count backwards from today
            while (entryDates.Contains(currentDate))
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }

            return streak;
        }

        private int CalculateLongestStreak(List<DateTime> entryDates)
        {
            if (!entryDates.Any())
                return 0;

            int longestStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < entryDates.Count; i++)
            {
                // Check if consecutive days
                if ((entryDates[i] - entryDates[i - 1]).Days == 1)
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return longestStreak;
        }

        private List<DateTime> CalculateMissedDays(List<DateTime> entryDates, DateTime startDate, DateTime endDate)
        {
            var missedDays = new List<DateTime>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (!entryDates.Contains(currentDate))
                {
                    missedDays.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }

            return missedDays;
        }

        // Get streak for a specific date range
        public async Task<int> GetStreakForRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var entries = await _context.JournalEntries
                    .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
                    .Select(e => e.Date.Date)
                    .OrderBy(d => d)
                    .ToListAsync();

                return CalculateLongestStreak(entries);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating range streak: {ex.Message}");
                return 0;
            }
        }
    }
}