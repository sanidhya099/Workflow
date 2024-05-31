using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

using TinaKingSystem;
using MudBlazor;



namespace TinaKingWebApp.Data.Dialog
{
    public partial class AccountProfileDialog
    {
        #region Properties
        MudForm form;
        [Parameter] public AccountProfileParameter accountProfileParameter { get; set; }
        [Inject] ISnackbar SnackBar { get; set; }
        [Inject] ClientDataService ClientDataService { get; set; }
        [Inject] EmployeeService EmployeeService { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            if (accountProfileParameter.Role == "Client") {
                ClientEditView c = ClientDataService.GetClientEdit(accountProfileParameter.UserID);
                accountProfileParameter.FirstName = c.FirstName;
                accountProfileParameter.LastName = c.LastName;
                accountProfileParameter.EmailAddress = c.Email;
            } else {
                EmployeeView e = EmployeeService.GetEmployeeByID(accountProfileParameter.UserID);
                accountProfileParameter.FirstName = e.FirstName;
                accountProfileParameter.LastName = e.LastName;
                accountProfileParameter.EmailAddress = e.Email;
            }
        }

        private async Task Confirm(bool confirm)
        {
            //. On Cancel Button
            if (confirm == false) {
                MudDialog.Close(DialogResult.Cancel()); return;
            }

            if (accountProfileParameter.FirstName == "") {
                SnackBar.Add("Please input the first name.", Severity.Warning); return;
            }
            if (accountProfileParameter.LastName == "") {
                SnackBar.Add("Please input the last name.", Severity.Warning); return;
            }

            string ret = "";
            if (accountProfileParameter.Role == "Client") {
                ret = await UpdateClientProfile();
            } else {
                ret = await UpdateEmployeeProfile();
            }

            if (ret != "") {
                SnackBar.Add("Failed to update profile.\n[ERROR : " + ret + "]", Severity.Warning); return;
            } else {
                SnackBar.Add("Profile was changed successfully.", Severity.Success);
            }

            MudDialog.Close(DialogResult.Ok(confirm));
        }

        private async Task<string> UpdateClientProfile()
        {
            UserProfile profile = new UserProfile() {
                FirstName = accountProfileParameter.FirstName,
                LastName = accountProfileParameter.LastName
            };
            if (await ClientDataService.UpdateProfile(accountProfileParameter.UserID, profile) == false) {
                return "Failed to connect to server";
            }
            return "";
        }

        private async Task<string> UpdateEmployeeProfile()
        {
            UserProfile profile = new UserProfile() {
                FirstName = accountProfileParameter.FirstName,
                LastName = accountProfileParameter.LastName
            };
            if (await EmployeeService.UpdateProfile(accountProfileParameter.UserID, profile) == false) {
                return "Failed to connect to server";
            }
            return "";
        }
        #endregion
    }

    public class AccountProfileParameter
    {
        public int UserID { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName{ get; set; }
        public string EmailAddress { get; set; }
    }
}
