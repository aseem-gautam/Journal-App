namespace JournalApp.Components.Layout
{
    public partial class NavMenu
    {
        private void Logout()
        {
            SecurityService.Logout();
            Navigation.NavigateTo("/", true);
        }
    }
}