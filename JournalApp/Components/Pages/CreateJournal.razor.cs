using JournalApp.Models;
using JournalApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


namespace JournalApp.Components.Pages
{
    public partial class CreateJournal
    {
        [SupplyParameterFromQuery]
        public string? date { get; set; }

        private bool isEditMode = false;
        private int? editEntryId = null;
        private DateTime selectedDate = DateTime.Today;
        private string title = string.Empty;
        private string content = string.Empty;
        private string category = string.Empty;
        private string newTagName = string.Empty;
        private string errorMessage = string.Empty;
        private string successMessage = string.Empty;
        private bool isSaving = false;

        private Mood? selectedPrimaryMood;
        private List<Mood> selectedSecondaryMoods = new();
        private List<Tag> selectedTags = new();

        private List<Mood> allMoods = new();
        private List<Tag> allTags = new();

        private int wordCount => string.IsNullOrWhiteSpace(content) ? 0 :
            content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

        protected override async Task OnInitializedAsync()
        {
            if (!SecurityService.IsAuthenticated)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            // Load moods and tags
            allMoods = await MoodService.GetAllMoodsAsync();
            allTags = await TagService.GetAllTagsAsync();

            // Parse date from query parameter
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
            {
                selectedDate = parsedDate;
            }

            // Check if entry exists for selected date
            await LoadExistingEntry();
        }

        private async Task LoadExistingEntry()
        {
            var entry = await JournalService.GetEntryByDateAsync(selectedDate);

            if (entry != null)
            {
                isEditMode = true;
                editEntryId = entry.Id;
                title = entry.Title;
                content = entry.Content;
                category = entry.Category ?? string.Empty;
                selectedPrimaryMood = entry.PrimaryMood;
                selectedSecondaryMoods = entry.SecondaryMoods.Select(em => em.Mood).ToList();
                selectedTags = entry.Tags.Select(et => et.Tag).ToList();
            }
            else
            {
                isEditMode = false;
                editEntryId = null;
            }
        }

        private async Task OnDateChanged()
        {
            await LoadExistingEntry();
            errorMessage = string.Empty;
            successMessage = string.Empty;
        }

        private void SelectPrimaryMood(Mood mood)
        {
            selectedPrimaryMood = mood;
        }

        private void ToggleSecondaryMood(Mood mood)
        {
            if (selectedSecondaryMoods.Any(m => m.Id == mood.Id))
            {
                selectedSecondaryMoods.RemoveAll(m => m.Id == mood.Id);
            }
            else if (selectedSecondaryMoods.Count < 2)
            {
                selectedSecondaryMoods.Add(mood);
            }
        }

        private void ToggleTag(Tag tag)
        {
            if (selectedTags.Any(t => t.Id == tag.Id))
            {
                selectedTags.RemoveAll(t => t.Id == tag.Id);
            }
            else
            {
                selectedTags.Add(tag);
            }
        }

        private void RemoveTag(Tag tag)
        {
            selectedTags.RemoveAll(t => t.Id == tag.Id);
        }

        private async Task AddCustomTag()
        {
            if (string.IsNullOrWhiteSpace(newTagName))
                return;

            var tag = await TagService.CreateTagAsync(newTagName.Trim());
            if (tag != null)
            {
                allTags.Add(tag);
                selectedTags.Add(tag);
                newTagName = string.Empty;
            }
        }

        private async Task HandleTagKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await AddCustomTag();
            }
        }

        private async Task SaveEntry()
        {
            errorMessage = string.Empty;
            successMessage = string.Empty;

            // Validation
            if (string.IsNullOrWhiteSpace(title))
            {
                errorMessage = "Title is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                errorMessage = "Content is required";
                return;
            }

            if (selectedPrimaryMood == null)
            {
                errorMessage = "Please select a primary mood";
                return;
            }

            isSaving = true;

            try
            {
                var secondaryMoodIds = selectedSecondaryMoods.Select(m => m.Id).ToList();
                var tagIds = selectedTags.Select(t => t.Id).ToList();

                JournalEntry? result;

                if (isEditMode && editEntryId.HasValue)
                {
                    // Update existing entry
                    result = await JournalService.UpdateEntryAsync(
                        editEntryId.Value,
                        title,
                        content,
                        selectedPrimaryMood.Id,
                        secondaryMoodIds,
                        tagIds,
                        category
                    );
                }
                else
                {
                    // Create new entry
                    result = await JournalService.CreateEntryAsync(
                        selectedDate,
                        title,
                        content,
                        selectedPrimaryMood.Id,
                        secondaryMoodIds,
                        tagIds,
                        category
                    );
                }

                if (result != null)
                {
                    successMessage = isEditMode ? "Entry updated successfully!" : "Entry saved successfully!";
                    await Task.Delay(1500);
                    Navigation.NavigateTo("/");
                }
                else
                {
                    errorMessage = "Failed to save entry. An entry may already exist for this date.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error saving entry: {ex.Message}";
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task DeleteEntry()
        {
            if (!isEditMode || !editEntryId.HasValue)
                return;

            if (confirm("Are you sure you want to delete this entry?"))
            {
                var success = await JournalService.DeleteEntryAsync(editEntryId.Value);
                if (success)
                {
                    Navigation.NavigateTo("/");
                }
                else
                {
                    errorMessage = "Failed to delete entry";
                }
            }
        }

        private bool confirm(string message)
        {
            // Simple confirmation - in real app, use a modal
            return true;
        }

        private void Cancel()
        {
            Navigation.NavigateTo("/");
        }
    }
}