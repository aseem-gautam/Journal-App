using JournalApp.Services;
using Microsoft.AspNetCore.Components.Web;

namespace JournalApp.Components.Pages
{
    public partial class LoginPage
    {
        private bool isSetupComplete = false;
        private string name = string.Empty;
        private string pin = string.Empty;
        private string confirmPin = string.Empty;
        private string loginPin = string.Empty;
        private string errorMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // Check if already authenticated
            if (SecurityService.IsAuthenticated)
            {
                Navigation.NavigateTo("/");
                return;
            }

            // Check if setup is complete
            isSetupComplete = await SecurityService.HasCompletedSetupAsync();
        }

        private async Task SetupAccount()
        {
            errorMessage = string.Empty;

            // Validate PIN
            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4 || pin.Length > 6)
            {
                errorMessage = "PIN must be 4-6 digits";
                return;
            }

            if (!pin.All(char.IsDigit))
            {
                errorMessage = "PIN must contain only numbers";
                return;
            }

            if (pin != confirmPin)
            {
                errorMessage = "PINs do not match";
                return;
            }

            // Setup user
            var success = await SecurityService.SetupUserAsync(name, pin);

            if (success)
            {
                Navigation.NavigateTo("/");
            }
            else
            {
                errorMessage = "Failed to create account. Please try again.";
            }
        }

        private async Task Login()
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(loginPin))
            {
                errorMessage = "Please enter your PIN";
                return;
            }

            var success = await SecurityService.AuthenticateAsync(loginPin);

            if (success)
            {
                Navigation.NavigateTo("/");
            }
            else
            {
                errorMessage = "Incorrect PIN. Please try again.";
                loginPin = string.Empty;
            }
        }

        private async Task HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Login();
            }
        }
    }
}