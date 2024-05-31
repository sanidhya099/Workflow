using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

using TinaKingSystem;
using MudBlazor;



namespace TinaKingWebApp.Data.Dialog
{
    public partial class ChangePasswordDialog
    {
        #region Methods
        MudForm form;
        [Parameter] public ChangepwdDialogParam ChnPwdDlgParam { get; set; }
        [Inject] ISnackbar SnackBar { get; set; }
        [Inject] ClientDataService ClientDataService { get; set; }
        [Inject] EmployeeService EmployeeService { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public string _summery { get; set; }
        public int _total { get; set; }

        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            if (ChnPwdDlgParam.Role == "Client") {
                ClientEditView c = ClientDataService.GetClientEdit(ChnPwdDlgParam.UserID);
                UserName = c.UserName;
            } else {
                EmployeeView e = EmployeeService.GetEmployeeByID(ChnPwdDlgParam.UserID);
                UserName = e.UserName;
            }
        }
        private async Task Confirm(bool confirm)
        {
            //. On Cancel Button
            if (confirm == false) {
                MudDialog.Close(DialogResult.Cancel()); return;
            }

            if (CurrentPassword == ""){
                SnackBar.Add("Please input the current password.", Severity.Warning); return;
            }
            if (NewPassword == ""){
                SnackBar.Add("Please input the New password.", Severity.Warning); return;
            }
            if (NewPassword == ""){
                SnackBar.Add("Please input the Confirm password.", Severity.Warning); return;
            }
            if (NewPassword != ConfirmPassword) {
                SnackBar.Add("Please input the password correctly. New passwords is different.", Severity.Warning); return;
            }

            string ret = "";
            if (ChnPwdDlgParam.Role == "Client") {
                ret = await ChangeClientPassword(ChnPwdDlgParam.UserID, CurrentPassword, NewPassword);
            } else {
                ret = await ChangeEmployeePassword(ChnPwdDlgParam.UserID, CurrentPassword, NewPassword);
            }

            if (ret != "") {
                SnackBar.Add("Failed to Change Password.\n[ERROR : " + ret + "]", Severity.Warning); return;
            } else {
                SnackBar.Add("Password was changed successfully.", Severity.Success); 
            }

            MudDialog.Close(DialogResult.Ok(confirm));
        }
        private async Task<string> ChangeClientPassword(int UserID, string CurrentPassword, string NewPassword)
        {
            if (ClientDataService.IsValidPassword(UserID, CurrentPassword) == false) {
                return "Current password is invalid";
            }
            if (await ClientDataService.SetPasswordInternally(UserID ,NewPassword ) == false) {
                return "Failed to connect server";
            }
            return "";
        }
        private async Task<string> ChangeEmployeePassword(int UserID, string CurrentPassword, string NewPassword)
        {
            if (EmployeeService.IsValidPassword(UserID, CurrentPassword) == false) {
                return "Current password is invalid";
            }
            if (await EmployeeService.SetPasswordInternally(UserID, NewPassword) == false) {
                return "Failed to connect server";
            }
            return "";
        }
        #endregion
    }

    public class ChangepwdDialogParam
    {
        public int UserID { get; set; }
        public string Role { get; set; }
    }
}
