using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Charts;
using Org.BouncyCastle.Crypto.Modes;
using System.Security.Claims;
using TinaKingSystem.BLL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;
namespace TinaKingWebApp.Pages.MainPages
{
    public partial class Review
    {
        #region Properties
        [Inject] private ReviewFormService FormService { get; set; }
        [Inject] private PackageService PackageService { get; set; }
        [Inject] public EventService EventService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }
        [Parameter] public string sPackageId { get; set; }
        public int PackageId { get; set; }
        public string Username { get; set; }
        public string PackageNo { get; set; }

        protected WPSView WPSView { get; set; } = new WPSView();
        private List<string> flaggedItems = new List<string>();

        private string firstName;
        private int EmployeeID;
        private string Role;
        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (sPackageId == null)
            {
                NavigationManager.NavigateTo($"/Master", forceLoad: true);
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            firstName = user.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? "Unknown";
            var clientIDString = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            Role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (int.TryParse(clientIDString, out int employeeID))
            {
                EmployeeID = employeeID;
            }

            PackageId = Int32.Parse(sPackageId);
            WPSView = FormService.GetByPackageID(PackageId);
            if (WPSView == null)
            {
                Snackbar.Add("There is no review of this package. New review will be created.");
                WPSView = new WPSView();
            }

            EmployeeView e = EmployeeService.GetEmployeeByID(EmployeeID);
            Username = e.UserName;

            WPSView.EmployeeID = EmployeeID;

            WPSView.PackageID = Int32.Parse(sPackageId);
            PackageView p = await PackageService.GetPackageById(sPackageId);
            PackageNo = p.PackageNumber;
        }

        public async Task OnUpdate()
        {
            flaggedItems.Clear();
            flaggedItems = WPSView.getSummeryList();
        }

        private async Task SubmitAsync()
        {
            if (CheckValidat() == false)
            {
                return;
            }
            try
            {
                await FormService.RegisterWPSView(WPSView);
                await PackageService.UploadReviewInfo(WPSView.PackageID, WPSView.getSummery());

                await EventService.UploadEvent(
                    EmployeeID,
                    Role,
                    "Review",
                    WPSView.PackageID,
                    Username,
                    PackageNo,
                    WPSView.WPSNo
                    );

                Snackbar.Add("Review have saved successfully", Severity.Success);
                NavigationManager.NavigateTo("/master", forceLoad: false);
            }
            catch (Exception ex)
            {
                Snackbar.Add("Fail to save review", Severity.Error);
            }
        }

        private bool CheckValidat()
        {
            if (WPSView.WPSNo == "")
            {
                Snackbar.Add("Please input the WPS Number."); return false;
            }
            return true;
        }
        #endregion
    }
}
