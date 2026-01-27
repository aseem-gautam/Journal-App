using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class SecurityService
    {
        private readonly IServiceProvider _serviceProvider;
        private User? _currentUser;
        private bool _isAuthenticated = false;

        public SecurityService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public User? CurrentUser => _currentUser;

        // Check if user has completed initial setup
        public async Task<bool> HasCompletedSetupAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();

            var user = await context.Users.FirstOrDefaultAsync();
            return user != null && user.HasSetupCompleted;
        }

        // Initial setup - create user with PIN
        public async Task<bool> SetupUserAsync(string name, string pin)
        {
            try
            {
                // Validate PIN (4-6 digits)
                if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4 || pin.Length > 6 || !pin.All(char.IsDigit))
                {
                    return false;
                }

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();

                // Check if user already exists
                var existingUser = await context.Users.FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    return false;
                }

                var user = new User
                {
                    Name = name,
                    PIN = pin,
                    CreatedAt = DateTime.Now,
                    HasSetupCompleted = true,
                    IsDarkMode = false
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                _currentUser = user;
                _isAuthenticated = true;

                System.Diagnostics.Debug.WriteLine($"User created and authenticated: {user.Name}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Setup error: {ex.Message}");
                return false;
            }
        }

        // Authenticate with PIN
        public async Task<bool> AuthenticateAsync(string pin)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();

                var user = await context.Users.FirstOrDefaultAsync();

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("Authentication failed: No user found");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Checking PIN: entered={pin}, stored={user.PIN}");

                if (user.PIN == pin)
                {
                    _currentUser = user;
                    _isAuthenticated = true;
                    System.Diagnostics.Debug.WriteLine($"Authentication successful for: {user.Name}");
                    return true;
                }

                System.Diagnostics.Debug.WriteLine("Authentication failed: PIN mismatch");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Authentication error: {ex.Message}");
                return false;
            }
        }

        // Change PIN
        public async Task<bool> ChangePINAsync(string oldPin, string newPin)
        {
            try
            {
                if (_currentUser == null || !_isAuthenticated)
                {
                    return false;
                }

                // Validate old PIN
                if (_currentUser.PIN != oldPin)
                {
                    return false;
                }

                // Validate new PIN (4-6 digits)
                if (string.IsNullOrWhiteSpace(newPin) || newPin.Length < 4 || newPin.Length > 6 || !newPin.All(char.IsDigit))
                {
                    return false;
                }

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();

                var user = await context.Users.FindAsync(_currentUser.Id);
                if (user != null)
                {
                    user.PIN = newPin;
                    await context.SaveChangesAsync();
                    _currentUser.PIN = newPin;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Change PIN error: {ex.Message}");
                return false;
            }
        }

        // Logout
        public void Logout()
        {
            _isAuthenticated = false;
            _currentUser = null;
            System.Diagnostics.Debug.WriteLine("User logged out");
        }

        // Get current user
        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();
            _currentUser = await context.Users.FirstOrDefaultAsync();
            return _currentUser;
        }

        // Update user preferences
        public async Task<bool> UpdateThemePreferenceAsync(bool isDarkMode)
        {
            try
            {
                if (_currentUser == null)
                {
                    return false;
                }

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();

                var user = await context.Users.FindAsync(_currentUser.Id);
                if (user != null)
                {
                    user.IsDarkMode = isDarkMode;
                    await context.SaveChangesAsync();
                    _currentUser.IsDarkMode = isDarkMode;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update theme error: {ex.Message}");
                return false;
            }
        }

        // Update user name
        public async Task<bool> UpdateUserNameAsync(string newName)
        {
            try
            {
                if (_currentUser == null || !_isAuthenticated)
                {
                    return false;
                }

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JournalDbContext>();

                var user = await context.Users.FindAsync(_currentUser.Id);
                if (user != null)
                {
                    user.Name = newName;
                    await context.SaveChangesAsync();
                    _currentUser.Name = newName;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update name error: {ex.Message}");
                return false;
            }
        }
    }
}