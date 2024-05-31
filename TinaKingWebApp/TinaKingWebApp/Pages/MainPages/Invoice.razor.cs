using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using System.Data;
using System.Drawing.Text;
using System.Linq;
using System.Security.Claims;
using TinaKingSystem.BLL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;
using TinaKingWebApp.Data.Dialog;
using static MudBlazor.CategoryTypes;
using Microsoft.JSInterop;
using Org.BouncyCastle.Bcpg;
using static MudBlazor.Colors;
using TinaKingSystem.InvoiceFunction;
using EmailService;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class Invoice
    {
        #region Properties
        [Inject] public PackageService PackageService { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }
        [Inject] public InvoiceService InvoiceService { get; set; }
        [Inject] public ClientDataService ClientDataService { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public EmailSender EmailSender { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Parameter] public string query { get; set; }

        private string userName = string.Empty;
        private string Role { get; set; }
        private int EmployeeID { get; set; }

        protected List<InvoiceView> invoiceViews { get; set; } = new List<InvoiceView>();
        private IEnumerable<InvoiceView> filteredInvoice => invoiceViews.Where(FilterFunc).ToList();

        #endregion

        #region Methods

        protected override async Task OnParametersSetAsync()
        {
            await LoadInvoiceData();
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            string uidstring = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "Unknown";
            try {
                EmployeeID = Int32.Parse(uidstring);
            } catch {
                EmployeeID = 0;
            }

            if (EmployeeID == 0) {
                NavigationManager.NavigateTo("/master", forceLoad:true);
                return;
            }

            userName = EmployeeService.GetEmployeeByID(EmployeeID).UserName;
            Role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";

            if (query == null || query == "") {
                query = "Draft";
            }

            await LoadInvoiceData();
        }

        public async Task LoadInvoiceData()
        {
            if (query == "Draft") {
                await ExtractInvoiceList();
            } else if (query == "Schedule") {
                if (await LoadInvoiceList("Schedule") == false) {
                    Snackbar.Add("Failed to get invoice list from server", Severity.Warning);
                }
            } else if (query == "History") {
                if (await LoadInvoiceList("Send") == false) {
                    Snackbar.Add("Failed to get invoice list from server", Severity.Warning);
                }
            }
        }

        public async Task ExtractInvoiceList()
        {
            try {
                invoiceViews = await PackageService.GetInvoiceList();
            }catch(Exception ex) {
                return;
            }
        }

        public async Task<bool> LoadInvoiceList(string status)
        {
            try {
                invoiceViews = await InvoiceService.GetInvoiceList(status);
                return true;
            }catch(Exception ex) {
                return false;
            }
        }

        public async Task ShowConfirmDialog(InvoiceView invoiceView)
        {
          
            List<PackageView> l = await PackageService.GetPackagesForInvoiceAsync(
                invoiceView.ID,
                invoiceView.UserID,
                invoiceView.Status
                );

            DialogOptions opt = new DialogOptions {
                Position = DialogPosition.TopCenter,
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            };
            var parameters = new DialogParameters { 
                ["PackageList"] = l,
                ["invoiceView"] = invoiceView,
            };

            var dialog = DialogService.Show<InvoiceDialog>("Select Packages to be Billed", parameters, opt);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm) {
                await LoadInvoiceData();
                StateHasChanged();
            }
        }

        public async Task SavePDF(InvoiceView invoiceView)
        {
            List<PackageView> l = await PackageService.GetPackagesForInvoiceAsync(
               invoiceView.ID,
               invoiceView.UserID,
               invoiceView.Status
               );
            ClaPDF pdf = new ClaPDF();
            await pdf.Generate(js, invoiceView, l);
        }

        public async Task SendNow(InvoiceView invoiceView)
        {
            List<PackageView> l = await PackageService.GetPackagesForInvoiceAsync(
               invoiceView.ID,
               invoiceView.UserID,
               invoiceView.Status
               );

            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to send invoice now?");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            var c = ClientDataService.GetByID(invoiceView.UserID);

            if (!result.Cancelled && result.Data is bool confirm && confirm) {
                ClaMail mail = new ClaMail(EmailSender);
                if (await mail.SendAsync(c.Email, invoiceView, l) == false) {
                    Snackbar.Add("Fail to send email", Severity.Warning); return;
                }

                //
                if (await InvoiceService.InvoiceToHistory(invoiceView) == false) {
                    Snackbar.Add("Fail to connect database", Severity.Warning); return;
                }
                await LoadInvoiceData();
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

        private bool FilterFunc(InvoiceView invoice)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;
            return invoice.InvoiceNo.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                || invoice.Username.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

    }
}
