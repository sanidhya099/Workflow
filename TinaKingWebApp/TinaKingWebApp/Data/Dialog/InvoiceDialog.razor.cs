using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using TinaKingSystem;
using MudBlazor;

namespace TinaKingWebApp.Data.Dialog
{
    public partial class InvoiceDialog
    {
        #region Properties
        MudForm form;

        [Parameter] public List<PackageView> PackageList { get; set; }
        [Parameter] public InvoiceView invoiceView { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public PackageService PackageService { get; set; }
        [Inject] public InvoiceService InvoiceService { get; set; }
        private IEnumerable<PackageView> filteredPackages => PackageList.Where(FilterFunc).ToList();
        public string InvoiceNo { get; set; }
        public string InvoiceCity { get; set; } = "";
        public string InvoiceCountry { get; set; } = "";
        public string InvoicePCode { get; set; } = "";
        public string InvoiceEmail { get; set; } = "";
        public string InvoiceBusiness { get; set; } = "";
        public double Total { get; set; } = 0;
        public double GST { get; set; } = 0;
        public string GstString { get; set; } = "";

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            CalcTotal();


            InvoiceView OldView = await InvoiceService.GetLastInvoice();
            if (OldView != null) {
                if (InvoiceCity == "") InvoiceCity = OldView.City;
                if (InvoiceCountry == "") InvoiceCountry = OldView.Country;
                if (InvoicePCode == "") InvoicePCode= OldView.PostalCode;
                if (InvoiceBusiness == "") InvoiceBusiness = OldView.Business;
                if (InvoiceEmail == "") InvoiceEmail = OldView.Email;
            }
        }

        public async Task OnCheckChanged(PackageView package, bool newState)
        {
            package.Selected = newState;
            CalcTotal();
            StateHasChanged();
        }

        private async Task Confirm(bool confirm)
        {
            if (confirm == false) {
                MudDialog.Close(DialogResult.Cancel());
                return;
            }
            int packcnt = 0;
            foreach(PackageView package in PackageList) {
                if (package.Selected == false) continue;
                packcnt++; 
            }
            if (packcnt == 0) {
                Snackbar.Add("Please select package at least one.", Severity.Warning); return;
            }

            int InvoiceID = await InvoiceService.UploadInvoice(
                InvoiceNo,
                PackageList[0].ClientID,
                PackageList[0].ClientName,
                InvoiceEmail,
                packcnt,
                Total,
                "Schedule",
                DateTime.Now,
                GST,
                InvoiceCity,
                InvoiceCountry,
                InvoicePCode,
                InvoiceBusiness
                );

            if (InvoiceID != 0) {
                await PackageService.SetPackageInvoice(PackageList, InvoiceID, false);
            }

            MudDialog.Close(DialogResult.Ok(confirm));
        }

        public async Task CalcTotal()
        {
            Total = 0;
            GST = 0;

            foreach (var package in PackageList) {
                if (package.Selected == false) continue;
                Total += package.Rate * package.SpendTime;
            }

            GST = Total * 0.05;
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
