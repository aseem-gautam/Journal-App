using JournalApp.Data;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class SecurityService
    {
        private readonly JournalDbContext _context;
        private User? _currentUser;
        private bool _isAuthenticated = false;

        public SecurityService(JournalDbContext context)
        {
            _context = context;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public User? CurrentUser => _currentUser;

        // Check if user has completed initial setup
        public async Task<bool> HasCompletedSetupAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
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

                // Check if user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    return false;
                }

                var user = new User
                {
                    Name = name,
                    PIN = pin, // In production, you might want to hash this
                    CreatedAt = DateTime.Now,
                    HasSetupCompleted = true,
                    IsDarkMode = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _currentUser = user;
                _isAuthenticated = true;

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
                var user = await _context.Users.FirstOrDefaultAsync();

                if (user == null)
                {
                    return false;
                }

                if (user.PIN == pin)
                {
                    _currentUser = user;
                    _isAuthenticated = true;
                    return true;
                }

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

                _currentUser.PIN = newPin;
                await _context.SaveChangesAsync();

                return true;
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
        }

        // Get current user
        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            _currentUser = await _context.Users.FirstOrDefaultAsync();
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

                _currentUser.IsDarkMode = isDarkMode;
                await _context.SaveChangesAsync();

                return true;
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

                _currentUser.Name = newName;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update name error: {ex.Message}");
                return false;
            }
        }
    }
}