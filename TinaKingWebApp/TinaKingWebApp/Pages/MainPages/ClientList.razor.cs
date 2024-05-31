using Microsoft.AspNetCore.Components;
using TinaKingSystem.ViewModels;
using TinaKingSystem.BLL;
using MudBlazor;
using TinaKingSystem.Entities;
using TinaKingWebApp.Data.Dialog;
using Microsoft.AspNetCore.Http;
using static MudBlazor.Colors;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class ClientList
    {
        #region Properties
        [Inject] public ClientDataService ClientDataService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        private IEnumerable<ClientSearchView> Clients { get; set; } = new List<ClientSearchView>();
        private IEnumerable<ClientSearchView> filteredClients => Clients.Where(FilterFunc).ToList();
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            Clients =  await ClientDataService.GetClients();
        }
        public void GoToRegisterClient()
        {
            NavigationManager.NavigateTo("/RegisterClient", forceLoad: true);
        }
        public void GoToRegisterClient(int ClientID)
        {
            NavigationManager.NavigateTo("/RegisterClient/"+ClientID, forceLoad: true);
        }

        private async Task DeleteClient(int clientId)
        {
            try
            {
                bool deleted = await ClientDataService.DeleteClientAsync(clientId);
                if (deleted)
                {
                    Snackbar.Add("Client successfully deleted.", Severity.Success);
                    Clients = await ClientDataService.GetClients();
                }
                else
                {
                    Snackbar.Add("Failed to delete the client. The client might not exist.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
            }
        }

        private async Task SetClientStatus(int clientId, string status)
        {
            try
            {
                bool updated = await ClientDataService.UpdateClientStatusAsync(clientId, status);
                if (updated){
                    if (status == "active")
                        Snackbar.Add("Client successfully actived.", Severity.Success);
                    else
                        Snackbar.Add("Client successfully disabled.", Severity.Success);
                    Clients = await ClientDataService.GetClients();
                }
                else
                {
                    Snackbar.Add("Failed to change status of client.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
            }
        }
        private async Task ShowDeleteConfirmDialog(int clientId, string username)
        {
            var parameters = new DialogParameters
            {
                { "Username", username }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = DialogService.Show<DeleteConfirmDialog>("Delete Package", parameters, options);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await DeleteClient(clientId);
            }
        }

        private async Task ShowEnableConfirmDialog(int clientId, string username)
        {
            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to ACTIVATE this account "+username+"?");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await SetClientStatus(clientId, "active");
            }
            StateHasChanged();
        }

        private async Task ShowDisableConfirmDialog(int clientId, string username)
        {
            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to DEACTIVATE this account " + username + " ?");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await SetClientStatus(clientId, "disabled");
            }
            StateHasChanged();
        }
        #endregion

        #region MudTable 
        private string searchString = "";
        private bool dense = true;
        private bool hover = true;
        private bool striped = true;
        private bool bordered = false;
        private MudTable<ClientSearchView> tableRef;
        private List<ClientSearchView> client;

        private bool FilterFunc(ClientSearchView client)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;

            bool isFirstNameMatch = client.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false;
            bool isLastNameMatch = client.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false;

            return isFirstNameMatch || isLastNameMatch;
        }
        private void ClearSearch()
        {
            NavigationManager.NavigateTo("/ClientList", forceLoad: true);
        }

        #endregion

    }
}
