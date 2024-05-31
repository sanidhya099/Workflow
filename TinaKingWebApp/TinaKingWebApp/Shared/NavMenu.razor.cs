using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using MudBlazor;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Core;
using Microsoft.AspNetCore.Components.Routing;
using TinaKingWebApp.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using TinaKingSystem.Entities;
using TinaKingWebApp.Data.Dialog;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Timers;
using static TinaKingSystem.ViewModels.WPSView;

namespace TinaKingWebApp.Shared
{
    public partial class NavMenu
    {
        public int UserID { get; set; } = 0;
        public string UserName { get; set; } = "Harries Potter";
        public string Role { get; set; } = "Admin";
        public int CountTotal { get; set; } = 0;
        public int CountFinish { get; set; } = 0;
        public int CountApprove { get; set; } = 0;
        public int CountReview { get; set; } = 0;
        public int CountPackage { get; set; } = 0;

        [Inject] protected ClientDataService ClientDataService { get; set; }
        [Inject] protected EmployeeService EmployeeService { get; set; }
        [Inject] protected EventService EventService { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        private string? currentUrl;

        System.Timers.Timer t = new System.Timers.Timer(5000);

        protected override async Task OnInitializedAsync()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var user = authState.User;
            UserName = user.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "Unknown";
            var clientIDString = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (int.TryParse(clientIDString, out int clientID))
            {
                UserID = clientID;
            }
            Role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) {
                t.AutoReset = false;
                t.Interval = 3000;
                t.Elapsed += new ElapsedEventHandler(OnTimerAsync);
            }
        }

        private async void OnTimerAsync(object? sender, ElapsedEventArgs e)
        {
            try {
                RefreshState().Wait();
            } catch {

            }
            Thread.Sleep(2000);
            t.Start();
        }
        private async Task RefreshState()
        {
            CountTotal = await EventService.GetNewEventCount(UserID, Role, "");
            CountFinish = await EventService.GetNewEventCount(UserID, Role, "Finish");
            CountApprove = await EventService.GetNewEventCount(UserID, Role, "Approve");
            CountReview = await EventService.GetNewEventCount(UserID, Role, "Review");
            CountPackage = await EventService.GetNewEventCount(UserID, Role, "Package");

            InvokeAsync(StateHasChanged);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        private async Task Logout()
        {
            var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            await customAuthStateProvider.UpdateAuthenticationState(null);
            NavigationManager.NavigateTo("/", true);
        }

        private async Task EventClear()
        {
            await EventService.UploadEvent(UserID, Role, "Clear", 0, "", "", "");
        }
        private async Task ChangePwd()
        {
            ChangepwdDialogParam changepwdDialogParam = new ChangepwdDialogParam() {
                UserID = this.UserID,
                Role = this.Role
            };
            var parameters = new DialogParameters { ["ChnPwdDlgParam"] = changepwdDialogParam };
            DialogOptions opt = new DialogOptions {
                Position = DialogPosition.TopCenter,
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            };

            var dialog = DialogService.Show<ChangePasswordDialog>("Change Password", parameters, opt);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm) {
                StateHasChanged();
            }
        }
        private async Task Profile()
        {
            AccountProfileParameter accountProfileParameter = new AccountProfileParameter() {
                UserID = this.UserID,
                Role = this.Role,
            };
            DialogOptions opt = new DialogOptions {
                Position = DialogPosition.TopCenter,
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            };
            var parameters = new DialogParameters { ["accountProfileParameter"] = accountProfileParameter };

            var dialog = DialogService.Show<AccountProfileDialog>("Account Information", parameters, opt);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm) {
                StateHasChanged();
            }
        }
    }
}