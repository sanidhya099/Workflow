using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using MudBlazor;
using TinaKingWebApp.Data.Dialog;
using TinaKingSystem.Entities;


namespace TinaKingWebApp.Pages.MainPages
{
    public partial class EmployeeList
    {
        #region Properties
        [Inject] private EmployeeService EmployeeService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public IDialogService DialogService { get; set; }

        private IEnumerable<EmployeeView> Employees { get; set; } = new List<EmployeeView>();
        private IEnumerable<EmployeeView> filteredEmployees => Employees.Where(FilterFunc).ToList();
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Employees = await EmployeeService.GetEmployeesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public void GoToRegisterEmployee()
        {
            NavigationManager.NavigateTo("/EmployeeRegistration", forceLoad: true);
        }

        private async Task DeleteEmployee(int employeeId)
        {
            try
            {
                bool deleted = await EmployeeService.DeleteEmployeeAsync(employeeId);
                if (deleted)
                {
                    Snackbar.Add("Employee successfully deleted.", Severity.Success);

                    Employees = await EmployeeService.GetEmployeesAsync();

                    StateHasChanged();
                }
                else
                {
                    Snackbar.Add("Failed to delete the employee. The employee might not exist.", Severity.Error);
                }
            }
            catch
            {
                Snackbar.Add($"An error occurred", Severity.Error);
            }
        }
        public void GoToRegister(int EID)
        {
            NavigationManager.NavigateTo("/EmployeeRegistration/"+EID, forceLoad: true);
        }

        private async Task ShowDeleteConfirmDialog(int employeeId, string username)
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
                await DeleteEmployee(employeeId);
            }
        }
        private async Task SetEmployeeStatus(int employeeId, string status)
        {
            try
            {
                bool updated = await EmployeeService.UpdateStatusAsync(employeeId, status);
                if (updated)
                {
                    if (status == "active")
                        Snackbar.Add("Client successfully actived.", Severity.Success);
                    else
                        Snackbar.Add("Client successfully disabled.", Severity.Success);
                    Employees = await EmployeeService.GetEmployeesAsync();
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
        private async Task ShowEnableConfirmDialog(int employeeId, string username)
        {
            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to decide active this account[" + username + "]?");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await SetEmployeeStatus(employeeId, "active");
            }
            StateHasChanged();
        }

        private async Task ShowDisableConfirmDialog(int employeeId, string username)
        {
            var parameters = new DialogParameters<ConfirmDialog>();
            parameters.Add(x => x.ContentText, "Are you sure you want to decide inactive this account[" + username + "]?");
            parameters.Add(x => x.ButtonText, "Yes");
            parameters.Add(x => x.Color, Color.Success);
            var dlg = DialogService.Show<ConfirmDialog>("Confirm", parameters);
            var result = await dlg.Result;

            if (!result.Cancelled && result.Data is bool confirm && confirm)
            {
                await SetEmployeeStatus(employeeId, "disabled");
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
        private MudTable<EmployeeView> tableRef;

        private bool FilterFunc(EmployeeView employee)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;

            bool isFirstNameMatch = employee.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false;
            bool isLastNameMatch = employee.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false;

            return isFirstNameMatch || isLastNameMatch;
        }

        #endregion
    }
}
