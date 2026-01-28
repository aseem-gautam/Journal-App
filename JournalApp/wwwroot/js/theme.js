window.applyTheme = (isDark) => {
    if (isDark) {
        document.body.classList.add('dark-theme');
        localStorage.setItem('theme', 'dark');
    } else {
        document.body.classList.remove('dark-theme');
        localStorage.setItem('theme', 'light');
    }
};

// Load theme on page load
window.loadTheme = () => {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark-theme');
        return true;
    }
    return false;
};

// Auto-load theme when page loads
document.addEventListener('DOMContentLoaded', () => {
    window.loadTheme();
});