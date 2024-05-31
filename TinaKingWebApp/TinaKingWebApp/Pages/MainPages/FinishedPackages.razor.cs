using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using TinaKingWebApp.Data.Dialog;
using System.Data;
using System.Drawing.Text;
using System.Linq;
using System.Security.Claims;
using System.Timers;
using static TinaKingSystem.ViewModels.WPSView;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class FinishedPackages
    {
        #region Properties
        [Inject] public PackageService PackageService { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ProtectedSessionStorage ProtectedSessionStorage { get; set; }
        [Inject] public EventService EventService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
       
        protected List<InvoiceView> invoiceViews { get; set; }
        private bool isDropdownOpen = false;
        private string FirstName;
        private string userName = string.Empty;
        private string Role { get; set; }
        private int EmployeeID { get; set; }
        public DateTime timeNow { get; set; }

        private IEnumerable<PackageView> Packages { get; set; } = new List<PackageView>();
        private IEnumerable<PackageView> filteredPackages => Packages.Where(FilterFunc).ToList();

        private Dictionary<int, bool> dropdownStates = new Dictionary<int, bool>();
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            FirstName = user.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "Unknown";
            Packages = await PackageService.GetFinishedPackages();
            userName = EmployeeService.GetEmployeeByID(EmployeeID)?.UserName ?? "Unknown";
           
        }

        public async Task Detail(PackageView package)
        {
            var parameters = new DialogParameters { ["packageView"] = package };

            var dialog = DialogService.Show<PackageDetailDialog>("Package Detail", parameters);
            var result = await dialog.Result;

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
}
