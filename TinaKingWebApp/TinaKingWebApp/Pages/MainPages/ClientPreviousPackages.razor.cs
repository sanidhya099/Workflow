using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using TinaKingSystem;
using Microsoft.AspNetCore.Components.Authorization;
using static MudBlazor.CategoryTypes;
using System.Net.Http;
using System.Net.Http.Json;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinaKingWebApp.Data.Dialog;
using TinaKingSystem.Entities;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class ClientPreviousPackages
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
            packages = await PackageService.GetClientPackagesAsync(ClientID);

            foreach (var package in packages)
            {
                packageEditStates[package.PackageID] = false;
            }
        }

        private void GoToNewPackage()
        {
            NavigationManager.NavigateTo($"/ClientInput", forceLoad: true);
        }

        private void GoToFinishedPackage()
        {
            NavigationManager.NavigateTo($"/ClientFinished/{{newPackID?}}", forceLoad: true);
        }

        private void Refresh()
        {
            NavigationManager.NavigateTo($"/PreviousPackages", forceLoad: true);
        }

        private async Task ToggleEditMode(int packageId)
        {
            packageEditStates[packageId] = !packageEditStates[packageId];
            StateHasChanged(); 
        }

        private async Task SaveChanges(PackageView package)
        {
            packageEditStates[package.PackageID] = false;

            await PackageService.UpdatePackage(package);
            StateHasChanged(); 
        }

        private async Task DeletePackage(int packageId, string reason)
        {
            var success = await PackageService.DeletePackageAsync(packageId, reason);
            if (success)
            {
                
                packages.RemoveAll(p => p.PackageID == packageId);
                StateHasChanged();
            }
            else
            {
                // Optionally show an error message
            }
        }

        private async Task OpenEditDialog(PackageView package)
        {
            var parameters = new DialogParameters { ["Package"] = package };

            var dialog = DialogService.Show<EditPackageDialog>("Edit Package", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {

                packages = await PackageService.GetClientPackagesAsync(ClientID); 
                StateHasChanged();
            }
        }


        private async Task ShowDeleteConfirmDialog(int packageId, string packageNumber)
        {
            DialogView Dlg = new DialogView()
            {
                Description = "Please leave the reason why you delete it.",
                Value = ""
            };
            var parameters = new DialogParameters { ["Dlg"] = Dlg };

            var dialog = DialogService.Show<EditDialogOne>("What's reason", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await DeletePackage(packageId, Dlg.Value);
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
        }
        #endregion

    }
}
