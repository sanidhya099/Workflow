using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinaKingSystem.BLL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class EmployeeRegistration
    {
        #region Properties
        [Inject] protected EmployeeService EmployeeService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Parameter] public string? EID { get; set; } = "";

        public int EmployeeID { get; set; }
        protected EmployeeView EmployeeView { get; set; } = new EmployeeView();
        protected string RegistrationResult { get; set; }
        protected string RegistrationResultCssClass { get; set; }
        protected string SuccessMessage { get; set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            if (EID == null) EID = "0";
            EmployeeID = Int32.Parse(EID);

            if (EmployeeID > 0)
            {
                EmployeeView = EmployeeService.GetEmployeeByID(EmployeeID);
                EmployeeView.Password = "";
            }
            else
            {
                EmployeeView = new EmployeeView();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            OnParametersSet();
            try
            {
                await InvokeAsync(StateHasChanged);
            }
            catch
            {

            }
        }
        private async Task HandleSubmit()
        {
            if (!IsValidEmail(EmployeeView.Email))
            {
                Snackbar.Add("Invalid email format.", Severity.Error);
                return;
            }

            if (!IsValidPhoneNumber(EmployeeView.Phone))
            {
                Snackbar.Add("Phone number must be exactly 10 digits.", Severity.Error);
                return;
            }

            try
            {
                if (EmployeeID == 0)
                {
                    if (IsDuplicateEmployee(EmployeeView))
                    {
                        RegistrationResult = "Email, phone number, or Username already exists.";
                        RegistrationResultCssClass = "alert-danger";
                    }
                    else
                    {
                        if (EmployeeService == null)
                        {
                            throw new InvalidOperationException("EmployeeService is not initialized.");
                        }

                        if (EmployeeView == null)
                        {
                            throw new ArgumentNullException(nameof(EmployeeView), "EmployeeView is null.");
                        }

                        var success = await EmployeeService.AddEmployeeAsync(EmployeeView);

                        if (success)
                        {
                            Snackbar.Add("Employee registration successful!", Severity.Success);

                        }
                        else
                        {
                            RegistrationResult = "Failed to add employee.";
                            Snackbar.Add("Failed to add employee.", Severity.Error);
                            RegistrationResultCssClass = "alert-warning";
                        }
                    }
                }
                else
                {
                    if (IsDuplicateEmployee(EmployeeView))
                    {
                        RegistrationResult = "Email, phone number, or Username already exists.";
                        RegistrationResultCssClass = "alert-danger";
                    }
                    else
                    {
                        var success = await EmployeeService.UpdateEmployeeAsync(EmployeeView);

                        if (success)
                        {
                            Snackbar.Add("Employee registration saved successful!", Severity.Success);
                            NavigationManager.NavigateTo("/EmployeeList", forceLoad: true);
                        }
                        else
                        {
                            RegistrationResult = "Failed to add employee.";
                            Snackbar.Add("Failed to add employee.", Severity.Error);
                            RegistrationResultCssClass = "alert-warning";
                        }
                    }
                }
            }
            catch
            {
                Snackbar.Add("Field is missing", Severity.Error);
            }
        }

        private void ClearForm()
        {
            NavigationManager.NavigateTo($"/EmployeeRegistration", forceLoad: true);
        }

        private bool IsDuplicateEmployee(EmployeeView employee)
        {
            bool hasDuplicateEmail = EmployeeService.EmployeeExistsWithEmail(employee.Email, employee.EmployeeID);
            bool hasDuplicatePhone = EmployeeService.EmployeeExistsWithPhone(employee.Phone, employee.EmployeeID);
            bool hasDuplicateUsername = EmployeeService.EmployeeExistsWithUsername(employee.UserName, employee.EmployeeID);
            return hasDuplicateEmail || hasDuplicatePhone || hasDuplicateUsername;
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(email, emailRegex);
        }

        private bool IsValidPhoneNumber(string phone)
        {
            var phoneRegex = @"^\d{10}$";
            return Regex.IsMatch(phone.Replace("-", ""), phoneRegex);
        }
        #endregion

    }
}
