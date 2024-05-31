using Microsoft.AspNetCore.Components;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using TinaKingWebApp.Data.Dialog;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class ClientFinishedPackages
    {
        #region Properties
        [Inject] public PackageService PackageService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        [Parameter] public string newPackID { get; set; }

        private List<PackageView> packages = new List<PackageView>();

        private Dictionary<int, bool> packageEditStates = new Dictionary<int, bool>();

        public int ClientID { get; set; }
        private string firstName;
        private string lastName;

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            firstName = user.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "Unknown";
            lastName = user.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? "Unknown";
            var clientIDString = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if (int.TryParse(clientIDString, out int clientID))
            {
                ClientID = clientID;
            }
            packages = await PackageService.GetClientFinishedPackagesAsync(ClientID);

            // Initialize edit states for all packages
            foreach (var package in packages)
            {
                packageEditStates[package.PackageID] = false;
            }
        }
        private void GoToNewPackage()
        {
            NavigationManager.NavigateTo($"/ClientInput", forceLoad: true);
        }
        private void GoToPreviousPackage()
        {
            NavigationManager.NavigateTo($"/PreviousPackages/{{newPackID?}}", forceLoad: true);
        }

        private void Refresh()
        {
            NavigationManager.NavigateTo($"/PreviousPackages", forceLoad: true);
        }

        private async Task ToggleEditMode(int packageId)
        {
            packageEditStates[packageId] = !packageEditStates[packageId];
            StateHasChanged(); // Refresh the UI to reflect edit mode changes
        }

        private async Task SaveChanges(PackageView package)
        {
          
            packageEditStates[package.PackageID] = false;

            await PackageService.UpdatePackage(package);
            StateHasChanged(); 
        }

        public async Task Detail(PackageView package)
        {
            var parameters = new DialogParameters { ["packageView"] = package };

            var dialog = DialogService.Show<PackageDetailDialog>("Package Detail", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await PackageService.UpdateMoreInformation(package);
                StateHasChanged();
            }
        }
        #endregion

        #region MudTable 
        private string searchString = "";
        private bool dense = true;
        private bool hover = true;
        private bool striped = true;
        private bool bordered = false;

        private bool FilterFunc(PackageView package)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;
            return package.PackageNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                || (package.str_documents != null && package.str_documents.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        }
        private void ClearSearch()
        {
            searchString = "";
            // Optionally, trigger a table refresh if needed
        }
        #endregion

    }
}
