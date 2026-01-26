using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class MoodService
    {
        private readonly JournalDbContext _context;

        public MoodService(JournalDbContext context)
        {
            _context = context;
        }

        // Get all moods
        public async Task<List<Mood>> GetAllMoodsAsync()
        {
            try
            {
                return await _context.Moods.OrderBy(m => m.Category).ThenBy(m => m.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting moods: {ex.Message}");
                return new List<Mood>();
            }
        }

        // Get moods by category
        public async Task<List<Mood>> GetMoodsByCategoryAsync(MoodCategory category)
        {
            try
            {
                return await _context.Moods
                    .Where(m => m.Category == category)
                    .OrderBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting moods by category: {ex.Message}");
                return new List<Mood>();
            }
        }

        // Get mood by ID
        public async Task<Mood?> GetMoodByIdAsync(int id)
        {
            try
            {
                return await _context.Moods.FindAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting mood by ID: {ex.Message}");
                return null;
            }
        }

        // Get positive moods
        public async Task<List<Mood>> GetPositiveMoodsAsync()
        {
            return await GetMoodsByCategoryAsync(MoodCategory.Positive);
        }

        // Get neutral moods
        public async Task<List<Mood>> GetNeutralMoodsAsync()
        {
            return await GetMoodsByCategoryAsync(MoodCategory.Neutral);
        }

        // Get negative moods
        public async Task<List<Mood>> GetNegativeMoodsAsync()
        {
            return await GetMoodsByCategoryAsync(MoodCategory.Negative);
        }
    }
}