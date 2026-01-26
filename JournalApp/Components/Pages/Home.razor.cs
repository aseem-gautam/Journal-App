using JournalApp.Models;
using JournalApp.Services;

namespace JournalApp.Components.Pages
{
    public partial class Home
    {
        private StreakInfo streakInfo = new();
        private JournalEntry? todayEntry;
        private List<JournalEntry> recentEntries = new();

        protected override async Task OnInitializedAsync()
        {
            if (!SecurityService.IsAuthenticated)
            {
                return;
            }

            await LoadData();
        }

        private async Task LoadData()
        {
            streakInfo = await StreakService.GetStreakInfoAsync();
            todayEntry = await JournalService.GetTodayEntryAsync();
            recentEntries = await JournalService.GetEntriesAsync(1, 5);
        }

        private void CreateEntry()
        {
            Navigation.NavigateTo("/create");
        }

        private void EditTodayEntry()
        {
            if (todayEntry != null)
            {
                Navigation.NavigateTo($"/create?date={todayEntry.Date:yyyy-MM-dd}");
            }
        }

        private void ViewEntry(int entryId)
        {
            Navigation.NavigateTo($"/entry/{entryId}");
        }
    }
}