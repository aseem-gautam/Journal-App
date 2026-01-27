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
                // REMOVED: await _context.Database.EnsureDeletedAsync();
                // This was deleting the database every time!

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

        // Method to reset database during development (call this manually if needed)
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