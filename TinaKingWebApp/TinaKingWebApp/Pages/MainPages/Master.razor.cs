using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MudBlazor;
using System.Data;
using System.Security.Claims;
using System.Timers;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using TinaKingWebApp.Data.Dialog;
using static TinaKingSystem.ViewModels.WPSView;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class Master
    {
        #region Properties
        [Inject]public PackageService PackageService { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ProtectedSessionStorage ProtectedSessionStorage { get; set; }
        [Inject] public EventService EventService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private IEnumerable<PackageView> Packages { get; set; } = new List<PackageView>();
        private IEnumerable<PackageView> filteredPackages => Packages.Where(FilterFunc).ToList();

        private Dictionary<int, bool> dropdownStates = new Dictionary<int, bool>();

        protected List<InvoiceView> invoiceViews { get; set; }

        private bool isDropdownOpen = false;

        private string FirstName;
        private string userName = string.Empty;
        private string Role { get; set; }
        private int EmployeeID { get; set; }
        System.Timers.Timer t = new System.Timers.Timer(2000);
        private bool first { get; set; } = false;

        public DateTime timeNow { get; set; }
        public int maxID { get; set; } = 0;
        #endregion

        #region Methods

        private void ToggleDropdown(int packageId)
        {
            if (dropdownStates.ContainsKey(packageId)) {
                // Toggle the current state
                dropdownStates[packageId] = !dropdownStates[packageId];
            } else {
                // If the dropdown state does not exist, initialize it as open (true)
                dropdownStates.Add(packageId, true);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            FirstName = user.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "Unknown";
            string uidstring = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "Unknown";
            try {
                EmployeeID = Int32.Parse(uidstring);
            } catch {
                EmployeeID = 0;
            }
            Role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";
            Console.WriteLine($"Logged in user: {FirstName}");

            Packages = await PackageService.GetPackages();
            userName = EmployeeService.GetEmployeeByID(EmployeeID).UserName;
            timeNow = DateTime.Now;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) {
                t.AutoReset = false;
                t.Interval = 3000;
                t.Elapsed += new ElapsedEventHandler(OnTimerAsync);
                t.Start();
            }
        }
        private async void OnTimerAsync(object? sender, ElapsedEventArgs e)
        {
            try {
                await CB_CHECK();

                Thread.Sleep(2000);
                t.Start();
            } catch {

            }
        }

        private async Task CB_CHECK()
        {
            int nID = maxID;
            List<EventView> l = await EventService.GetEventsAsync(EmployeeID, Role, nID);
            foreach (EventView e in l) {
                if (e != null) {
                    string message = "";
                    if (e.Type == "Package") {
                        message = "[" + e.Username + "] upload a package[" + e.PackageNo + "].";
                    } else if (e.Type == "Review") {
                        message = "[" + e.Username + "] create or modify review of package[" + e.PackageNo + "].";
                    } else if (e.Type == "Approve") {
                        message = "[" + e.Username + "] approve a package[" + e.PackageNo + "].";
                    } else if (e.Type == "Finish") {
                        message = "[" + e.Username + "] set a package[" + e.PackageNo + "] finished.";
                    }
                    if (message != "" && maxID != 0)
                        JSRuntime.InvokeVoidAsync("SnackBarString", message);
                    if (nID <= e.ID)
                        nID = e.ID;
                }
            }
            maxID = nID;

        }
        public void EditPackage(PackageView package)
        {
            NavigationManager.NavigateTo($"/editPackage/{package.PackageID}");
        }
        public async Task GoToReview(PackageView package)
        {
            if (package.TypeOfRequest == "Drawing") {
                //. first get the package list
                DialogOptions opt = new DialogOptions {
                    Position = DialogPosition.TopCenter,
                    CloseButton = true,
                    MaxWidth = MaxWidth.Large
                };
                var parameters = new DialogParameters {
                    ["packageView"] = package,
                    ["UserID"] = EmployeeID,
                    ["Role"] = Role,
                    ["UserName"] = userName,
                };

                var dialog = DialogService.Show<DrawReviewDialog>("Revoiw of Drawing", parameters, opt);
                var result = await dialog.Result;

                if (!result.Cancelled && result.Data is bool confirm && confirm) {
                    StateHasChanged();
                }
            } else {
                NavigationManager.NavigateTo($"/Review/" + package.PackageID, forceLoad: true);
            }
        }
        public async Task SetRate(PackageView package)
        {
            DialogView Dlg = new DialogView()
            {
                Description = "Rate per Hour",
                Value = "0"
            };

            var parameters = new DialogParameters { ["Dlg"] = Dlg };

            var dialog = DialogService.Show<EditDialogOne>("Set Rate", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await PackageService.UpdateRate(package.PackageID, Double.Parse(Dlg.Value));
                StateHasChanged();
            }
        }
        public async Task SetTotal(PackageView package)
        {
            DialogView Dlg = new DialogView()
            {
                Description = "Total",
                Value = "0"
            };

            var parameters = new DialogParameters { ["Dlg"] = Dlg };

            var dialog = DialogService.Show<EditDialogOne>("Set Total", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await PackageService.UpdateTotal(package.PackageID, Double.Parse(Dlg.Value));
                StateHasChanged();
            }
        }
        public async Task SetNote(PackageView package)
        {
            DialogView Dlg = new DialogView()
            {
                Description = "Note",
                Value = "0"
            };

            var parameters = new DialogParameters { ["Dlg"] = Dlg };

            var dialog = DialogService.Show<EditDialogOne>("Write Note", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await PackageService.UpdateNote(package.PackageID, Dlg.Value);
                StateHasChanged();
            }
        }
        public async Task SetComment(PackageView package)
        {
            DialogView Dlg = new DialogView()
            {
                Description = "Comment",
                Value = "0"
            };

            var parameters = new DialogParameters { ["Dlg"] = Dlg };

            var dialog = DialogService.Show<EditDialogOne>("Write Comment", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                StateHasChanged();
            }
        }

        public async Task SetFinish(PackageView package)
        {
            if (package.SpendTime == 0 || package.Rate == 0) {
                Snackbar.Add("Please Input Time Spend and Rate Per hours", Severity.Error); return;
            }
            if (package.ApproveState == "A")
            {
                Snackbar.Add("This package was already finished.", Severity.Error); return;
            }
            if (package.ApproveState != "M")
            {
                Snackbar.Add("This package isn't approved by manager yet.", Severity.Error); return;
            }

            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to Finish this Package ? ");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await PackageService.SetPackageFlag(package, "A");
                package.ApproveState = "A";
                package.EndDate = DateTime.Now;
                StateHasChanged();
            }
            await EventService.UploadEvent(
                EmployeeID, 
                Role, 
                "Finish", 
                package.PackageID,
                userName,
                package.PackageNumber,
                await PackageService.GetWPSNo(package.PackageID)
                );
        }

        public async Task Approve(PackageView package)
        {
            if (package.ApproveState == "A")
            {
                Snackbar.Add("This package was already finished.", Severity.Error); return;
            }
            if (package.ApproveState == "M")
            {
                Snackbar.Add("This package was already approved.", Severity.Error); return;
            }
            if (package.ApproveState != "V")
            {
                Snackbar.Add("There is no review for this package.", Severity.Error); return;
            }
            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to approve this package?");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await PackageService.SetPackageFlag(package, "M");
                package.ApproveState = "M";
                StateHasChanged();
            }
            await EventService.UploadEvent(
                EmployeeID, 
                Role, 
                "Approve", 
                package.PackageID,
                userName,
                package.PackageNumber,
                await PackageService.GetWPSNo(package.PackageID));

        }

        public async Task Detail(PackageView package)
        {
            var parameters = new DialogParameters { ["packageView"] = package};

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
        private bool dense = false;
        private bool hover = true;
        private bool striped = true;
        private bool bordered = false;

        private bool FilterFunc(PackageView package)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;
            return package.PackageNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                || (package.str_documents != null && package.str_documents.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                || package.DateSubmitted.ToString("dd MMM ").Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

    }

    #region Action
    public class ActionReference
    {
        private Action _action;

        public ActionReference(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        [JSInvokable]
        public void Invoke()
        {
            _action.Invoke();
        }
    }
    #endregion
}
