namespace JournalApp.Services
{
    public class ThemeService
    {
        private bool _isDarkMode = false;
        public event Action? OnThemeChanged;

        public bool IsDarkMode => _isDarkMode;

        public void SetTheme(bool isDarkMode)
        {
            _isDarkMode = isDarkMode;
            OnThemeChanged?.Invoke();
        }

        public void ToggleTheme()
        {
            _isDarkMode = !_isDarkMode;
            OnThemeChanged?.Invoke();
        }

        public string GetThemeClass()
        {
            return _isDarkMode ? "dark-theme" : "light-theme";
        }
    }
}
