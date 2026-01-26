using Microsoft.EntityFrameworkCore;

namespace JournalApp.Data
{
    public class DatabaseService
    {
        private readonly JournalDbContext _context;

        public DatabaseService(JournalDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // FOR DEVELOPMENT ONLY - Recreates database on every run
                // Remove these two lines once you want to keep your data!
                await _context.Database.EnsureDeletedAsync();

                // Create database if it doesn't exist
                await _context.Database.EnsureCreatedAsync();

                System.Diagnostics.Debug.WriteLine("Database initialized successfully!");
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DatabaseExistsAsync()
        {
            return await _context.Database.CanConnectAsync();
        }

        // Method to reset database during development
        public async Task ResetDatabaseAsync()
        {
            try
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
                System.Diagnostics.Debug.WriteLine("Database reset successfully!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database reset error: {ex.Message}");
                throw;
            }
        }

        // Get database path for debugging
        public string GetDatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, "journal.db");
        }
    }
}
