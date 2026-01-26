using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class JournalService
    {
        private readonly JournalDbContext _context;

        public JournalService(JournalDbContext context)
        {
            _context = context;
        }

        // CREATE - Add new journal entry (only one per day)
        public async Task<JournalEntry?> CreateEntryAsync(DateTime date, string title, string content,
            int primaryMoodId, List<int>? secondaryMoodIds = null, List<int>? tagIds = null, string? category = null)
        {
            try
            {
                // Normalize date to remove time component
                var normalizedDate = date.Date;

                // Check if entry already exists for this date
                var existingEntry = await _context.JournalEntries
                    .FirstOrDefaultAsync(e => e.Date.Date == normalizedDate);

                if (existingEntry != null)
                {
                    return null; // Only one entry per day allowed
                }

                // Calculate word count
                int wordCount = content.Split(new[] { ' ', '\n', '\r', '\t' },
                    StringSplitOptions.RemoveEmptyEntries).Length;

                var entry = new JournalEntry
                {
                    Date = normalizedDate,
                    Title = title,
                    Content = content,
                    CreatedAt = DateTime.Now,
                    PrimaryMoodId = primaryMoodId,
                    Category = category,
                    WordCount = wordCount
                };

                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();

                // Add secondary moods (max 2)
                if (secondaryMoodIds != null && secondaryMoodIds.Any())
                {
                    var moodsToAdd = secondaryMoodIds.Take(2).ToList();
                    foreach (var moodId in moodsToAdd)
                    {
                        _context.EntryMoods.Add(new EntryMood
                        {
                            JournalEntryId = entry.Id,
                            MoodId = moodId
                        });
                    }
                }

                // Add tags
                if (tagIds != null && tagIds.Any())
                {
                    foreach (var tagId in tagIds)
                    {
                        _context.EntryTags.Add(new EntryTag
                        {
                            JournalEntryId = entry.Id,
                            TagId = tagId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Reload entry with related data
                return await GetEntryByIdAsync(entry.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating entry: {ex.Message}");
                return null;
            }
        }

        // READ - Get entry by ID
        public async Task<JournalEntry?> GetEntryByIdAsync(int id)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entry by ID: {ex.Message}");
                return null;
            }
        }

        // READ - Get entry by date
        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            try
            {
                var normalizedDate = date.Date;
                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .FirstOrDefaultAsync(e => e.Date.Date == normalizedDate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entry by date: {ex.Message}");
                return null;
            }
        }

        // READ - Get today's entry
        public async Task<JournalEntry?> GetTodayEntryAsync()
        {
            return await GetEntryByDateAsync(DateTime.Today);
        }

        // READ - Get all entries (paginated)
        public async Task<List<JournalEntry>> GetEntriesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .OrderByDescending(e => e.Date)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entries: {ex.Message}");
                return new List<JournalEntry>();
            }
        }

        // READ - Get entries by date range
        public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var normalizedStart = startDate.Date;
                var normalizedEnd = endDate.Date;

                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .Where(e => e.Date.Date >= normalizedStart && e.Date.Date <= normalizedEnd)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entries by date range: {ex.Message}");
                return new List<JournalEntry>();
            }
        }

        // READ - Search entries
        public async Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .Where(e => e.Title.Contains(searchTerm) || e.Content.Contains(searchTerm))
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching entries: {ex.Message}");
                return new List<JournalEntry>();
            }
        }

        // READ - Filter entries by mood
        public async Task<List<JournalEntry>> GetEntriesByMoodAsync(int moodId)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .Where(e => e.PrimaryMoodId == moodId ||
                                e.SecondaryMoods.Any(sm => sm.MoodId == moodId))
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entries by mood: {ex.Message}");
                return new List<JournalEntry>();
            }
        }

        // READ - Filter entries by tag
        public async Task<List<JournalEntry>> GetEntriesByTagAsync(int tagId)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMoods)
                        .ThenInclude(em => em.Mood)
                    .Include(e => e.Tags)
                        .ThenInclude(et => et.Tag)
                    .Where(e => e.Tags.Any(t => t.TagId == tagId))
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting entries by tag: {ex.Message}");
                return new List<JournalEntry>();
            }
        }

        // UPDATE - Update existing entry
        public async Task<JournalEntry?> UpdateEntryAsync(int id, string title, string content,
            int primaryMoodId, List<int>? secondaryMoodIds = null, List<int>? tagIds = null, string? category = null)
        {
            try
            {
                var entry = await _context.JournalEntries
                    .Include(e => e.SecondaryMoods)
                    .Include(e => e.Tags)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (entry == null)
                {
                    return null;
                }

                // Update basic fields
                entry.Title = title;
                entry.Content = content;
                entry.PrimaryMoodId = primaryMoodId;
                entry.Category = category;
                entry.UpdatedAt = DateTime.Now;
                entry.WordCount = content.Split(new[] { ' ', '\n', '\r', '\t' },
                    StringSplitOptions.RemoveEmptyEntries).Length;

                // Update secondary moods
                _context.EntryMoods.RemoveRange(entry.SecondaryMoods);
                if (secondaryMoodIds != null && secondaryMoodIds.Any())
                {
                    var moodsToAdd = secondaryMoodIds.Take(2).ToList();
                    foreach (var moodId in moodsToAdd)
                    {
                        _context.EntryMoods.Add(new EntryMood
                        {
                            JournalEntryId = entry.Id,
                            MoodId = moodId
                        });
                    }
                }

                // Update tags
                _context.EntryTags.RemoveRange(entry.Tags);
                if (tagIds != null && tagIds.Any())
                {
                    foreach (var tagId in tagIds)
                    {
                        _context.EntryTags.Add(new EntryTag
                        {
                            JournalEntryId = entry.Id,
                            TagId = tagId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Reload entry with updated data
                return await GetEntryByIdAsync(entry.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating entry: {ex.Message}");
                return null;
            }
        }

        // DELETE - Delete entry
        public async Task<bool> DeleteEntryAsync(int id)
        {
            try
            {
                var entry = await _context.JournalEntries.FindAsync(id);

                if (entry == null)
                {
                    return false;
                }

                _context.JournalEntries.Remove(entry);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting entry: {ex.Message}");
                return false;
            }
        }

        // Get total count of entries
        public async Task<int> GetTotalEntriesCountAsync()
        {
            try
            {
                return await _context.JournalEntries.CountAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting total entries count: {ex.Message}");
                return 0;
            }
        }

        // Check if entry exists for a date
        public async Task<bool> EntryExistsForDateAsync(DateTime date)
        {
            try
            {
                var normalizedDate = date.Date;
                return await _context.JournalEntries
                    .AnyAsync(e => e.Date.Date == normalizedDate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking entry existence: {ex.Message}");
                return false;
            }
        }
    }
}
