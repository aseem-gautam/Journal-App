using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Services;

namespace JournalApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Configure Database
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");

            builder.Services.AddDbContext<JournalDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Register all Services
            builder.Services.AddScoped<DatabaseService>();

            // IMPORTANT: SecurityService must be Singleton to maintain auth state
            builder.Services.AddSingleton<SecurityService>();

            builder.Services.AddScoped<JournalService>();
            builder.Services.AddScoped<MoodService>();
            builder.Services.AddScoped<TagService>();
            builder.Services.AddScoped<StreakService>();
            builder.Services.AddScoped<AnalyticsService>();
            builder.Services.AddScoped<ExportService>();
            builder.Services.AddSingleton<ThemeService>();

            var app = builder.Build();

            // Initialize database on startup
            using (var scope = app.Services.CreateScope())
            {
                var dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
                dbService.InitializeAsync().Wait();

                // Print database path for debugging
                System.Diagnostics.Debug.WriteLine($"Database location: {dbService.GetDatabasePath()}");
            }

            return app;
        }
    }
}