using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    // Analytics data models
    public class MoodDistribution
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class MoodFrequency
    {
        public string MoodName { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class TagUsage
    {
        public string TagName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class WordCountTrend
    {
        public DateTime Date { get; set; }
        public int WordCount { get; set; }
        public int AverageWordCount { get; set; }
    }

    public class AnalyticsService
    {
        private readonly JournalDbContext _context;

        public AnalyticsService(JournalDbContext context)
        {
            _context = context;
        }

        // Get mood distribution (Positive, Neutral, Negative %)
        public async Task<List<MoodDistribution>> GetMoodDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .AsQueryable();

                // Apply date filter if provided
                if (startDate.HasValue)
                    query = query.Where(e => e.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.Date.Date <= endDate.Value.Date);

                var entries = await query.ToListAsync();

                if (!entries.Any())
                    return new List<MoodDistribution>();

                var totalEntries = entries.Count;

                var distribution = entries
                    .GroupBy(e => e.PrimaryMood.Category.ToString())
                    .Select(g => new MoodDistribution
                    {
                        Category = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / totalEntries * 100, 2)
                    })
                    .OrderBy(m => m.Category)
                    .ToList();

                return distribution;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting mood distribution: {ex.Message}");
                return new List<MoodDistribution>();
            }
        }

        // Get most frequent moods
        public async Task<List<MoodFrequency>> GetMostFrequentMoodsAsync(int topN = 5, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(e => e.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.Date.Date <= endDate.Value.Date);

                var entries = await query.ToListAsync();

                var moodFrequencies = entries
                    .GroupBy(e => e.PrimaryMood)
                    .Select(g => new MoodFrequency
                    {
                        MoodName = g.Key.Name,
                        Count = g.Count(),
                        Icon = g.Key.Icon ?? "",
                        Color = g.Key.Color ?? "#000000"
                    })
                    .OrderByDescending(m => m.Count)
                    .Take(topN)
                    .ToList();

                return moodFrequencies;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting frequent moods: {ex.Message}");
                return new List<MoodFrequency>();
            }
        }

        // Get the single most frequent mood
        public async Task<MoodFrequency?> GetMostFrequentMoodAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var moods = await GetMostFrequentMoodsAsync(1, startDate, endDate);
            return moods.FirstOrDefault();
        }

        // Get most used tags
        public async Task<List<TagUsage>> GetMostUsedTagsAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.EntryTags
                    .Include(et => et.Tag)
                    .Include(et => et.JournalEntry)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(et => et.JournalEntry.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(et => et.JournalEntry.Date.Date <= endDate.Value.Date);

                var entryTags = await query.ToListAsync();

                if (!entryTags.Any())
                    return new List<TagUsage>();

                var totalTags = entryTags.Count;

                var tagUsages = entryTags
                    .GroupBy(et => et.Tag.Name)
                    .Select(g => new TagUsage
                    {
                        TagName = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / totalTags * 100, 2)
                    })
                    .OrderByDescending(t => t.Count)
                    .Take(topN)
                    .ToList();

                return tagUsages;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting tag usage: {ex.Message}");
                return new List<TagUsage>();
            }
        }

        // Get tag breakdown by category
        public async Task<Dictionary<string, int>> GetTagBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.EntryTags
                    .Include(et => et.Tag)
                    .Include(et => et.JournalEntry)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(et => et.JournalEntry.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(et => et.JournalEntry.Date.Date <= endDate.Value.Date);

                var entryTags = await query.ToListAsync();

                var breakdown = entryTags
                    .GroupBy(et => et.Tag.Name)
                    .ToDictionary(g => g.Key, g => g.Count());

                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting tag breakdown: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        // Get word count trends
        public async Task<List<WordCountTrend>> GetWordCountTrendsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(e => e.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.Date.Date <= endDate.Value.Date);

                var entries = await query.OrderBy(e => e.Date).ToListAsync();

                if (!entries.Any())
                    return new List<WordCountTrend>();

                // Calculate running average
                var totalWordCount = 0;
                var trends = new List<WordCountTrend>();

                for (int i = 0; i < entries.Count; i++)
                {
                    totalWordCount += entries[i].WordCount;
                    var averageWordCount = totalWordCount / (i + 1);

                    trends.Add(new WordCountTrend
                    {
                        Date = entries[i].Date,
                        WordCount = entries[i].WordCount,
                        AverageWordCount = averageWordCount
                    });
                }

                return trends;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting word count trends: {ex.Message}");
                return new List<WordCountTrend>();
            }
        }

        // Get average word count
        public async Task<double> GetAverageWordCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(e => e.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.Date.Date <= endDate.Value.Date);

                var entries = await query.ToListAsync();

                if (!entries.Any())
                    return 0;

                return Math.Round(entries.Average(e => e.WordCount), 2);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting average word count: {ex.Message}");
                return 0;
            }
        }

        // Get total word count
        public async Task<int> GetTotalWordCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(e => e.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.Date.Date <= endDate.Value.Date);

                return await query.SumAsync(e => e.WordCount);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting total word count: {ex.Message}");
                return 0;
            }
        }

        // Get entries count by date range
        public async Task<int> GetEntriesCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(e => e.Date.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.Date.Date <= endDate.Value.Date);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entries count: {ex.Message}");
                return 0;
            }
        }
    }
}