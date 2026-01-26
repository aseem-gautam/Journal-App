using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class TagService
    {
        private readonly JournalDbContext _context;

        public TagService(JournalDbContext context)
        {
            _context = context;
        }

        // Get all tags
        public async Task<List<Tag>> GetAllTagsAsync()
        {
            try
            {
                return await _context.Tags
                    .OrderByDescending(t => t.IsPreDefined)
                    .ThenBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting tags: {ex.Message}");
                return new List<Tag>();
            }
        }

        // Get predefined tags
        public async Task<List<Tag>> GetPredefinedTagsAsync()
        {
            try
            {
                return await _context.Tags
                    .Where(t => t.IsPreDefined)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting predefined tags: {ex.Message}");
                return new List<Tag>();
            }
        }

        // Get custom tags
        public async Task<List<Tag>> GetCustomTagsAsync()
        {
            try
            {
                return await _context.Tags
                    .Where(t => !t.IsPreDefined)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting custom tags: {ex.Message}");
                return new List<Tag>();
            }
        }

        // Create custom tag
        public async Task<Tag?> CreateTagAsync(string name, string? color = null)
        {
            try
            {
                // Check if tag already exists
                var existingTag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

                if (existingTag != null)
                {
                    return existingTag;
                }

                var tag = new Tag
                {
                    Name = name,
                    IsPreDefined = false,
                    Color = color
                };

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                return tag;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating tag: {ex.Message}");
                return null;
            }
        }

        // Delete custom tag
        public async Task<bool> DeleteTagAsync(int tagId)
        {
            try
            {
                var tag = await _context.Tags.FindAsync(tagId);

                if (tag == null || tag.IsPreDefined)
                {
                    return false; // Can't delete predefined tags
                }

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting tag: {ex.Message}");
                return false;
            }
        }

        // Get tag by ID
        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            try
            {
                return await _context.Tags.FindAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting tag by ID: {ex.Message}");
                return null;
            }
        }

        // Search tags by name
        public async Task<List<Tag>> SearchTagsAsync(string searchTerm)
        {
            try
            {
                return await _context.Tags
                    .Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()))
                    .OrderByDescending(t => t.IsPreDefined)
                    .ThenBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching tags: {ex.Message}");
                return new List<Tag>();
            }
        }
    }
}
