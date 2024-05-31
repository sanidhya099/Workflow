
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using TinaKingWebApp.Authentication;
using BCrypt.Net;
using MudBlazor;
using TinaKingSystem.Entities;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class LoginForm
    {
        #region Properties
        
        [Inject] public ISnackbar Snackbar { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public EmployeeService EmployeeService { get; set; }

        [Inject] public ClientDataService ClientDataService { get; set; }

        [Inject] public IJSRuntime js { get; set; }
        private bool IsAccountTypeSelected = false;

        private ClientSearchView ClientView = new ClientSearchView();
        private EmployeeView EmployeeView = new EmployeeView();

        private string UserName { get; set; }
        private string Password { get; set; }

        private List<string> errorMessages = new List<string>();
        private bool hasError => errorMessages.Count > 0;
        private bool? isClientLogin;
        private object SelectedModel => isClientLogin == true ? (object)ClientView : EmployeeView;
        #endregion

        #region Methods

        public void SelectClientLogin()
        {
            isClientLogin = true;
            StateHasChanged();
        }

        public void SelectEmployeeLogin()
        {
            isClientLogin = false;
            StateHasChanged();
        }

        private async Task HandleLogin()
        {
            Employee employee;
            errorMessages.Clear();
            Snackbar.Clear();

            if (!isClientLogin.HasValue)
            {
                Snackbar.Add("Please select a login type.", Severity.Error);
                errorMessages.Add("Please select a login type.");
                return;
            }

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                Snackbar.Add("Username and Password cannot be empty.", Severity.Error);
                errorMessages.Add("Username and Password cannot be empty.");
                return;
            }

            if (isClientLogin.Value)
            {
                var client = ClientDataService.GetByUserName(UserName, Password);
                if (client != null)
                {
                    if (client.Status != "active")
                    {
                        Snackbar.Add("This user account is inactived.", Severity.Error);
                        errorMessages.Add("Inactived user account.");
                        return;
                    }
                    await ((CustomAuthenticationStateProvider)authStateProvider).UpdateAuthenticationState(new UserSession
                    {
                        UserName = client.Username,
                        FirstName = client.FirstName,
                        Role = client.Role,
                        UserID = client.ClientID
                    });
                    Snackbar.Add("Login successful!", Severity.Success);
                    navManager.NavigateTo($"/PreviousPackages", forceLoad: true);

                }
                else
                {
                    Snackbar.Add("Invalid username or password.", Severity.Error);
                    errorMessages.Add("Invalid username or password.");
                }
            }
            else
            {
                try
                {
                    employee = EmployeeService.GetEmployeeByUserName(UserName, Password);
                    if (employee != null)
                    {
                        if (employee.Status != "active")
                        {
                            Snackbar.Add("This user account is inactived.", Severity.Error);
                            errorMessages.Add("Inactived user account.");
                            return;
                        }
                        await ((CustomAuthenticationStateProvider)authStateProvider).UpdateAuthenticationState(new UserSession
                        {
                            UserName = employee.Username,
                            Role = employee.Role,
                            FirstName = employee.FirstName,
                            UserID = employee.EmployeeID

                        });
                        Snackbar.Add("Login successful!", Severity.Success);
                        NavigationManager.NavigateTo("/master", forceLoad: true);
                    }
                    else
                    {
                        Snackbar.Add("Invalid username or password.", Severity.Error);
                        errorMessages.Add("Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    Snackbar.Add("Failed to connect database.", Severity.Error);
                    errorMessages.Add("Failed to connect database..");
                }
            }
        }

        private async Task HandleForget()
        {
            if (isClientLogin.Value)
            {
                NavigationManager.NavigateTo("/ForgetPassword/Client");
            }
            else
            {
                NavigationManager.NavigateTo("/ForgetPassword/Employee");
            }
        }


        private void Emp()
        {
            NavigationManager.NavigateTo("/EmployeeRegistration");
        }
        private void EmpL()
        {
            NavigationManager.NavigateTo("/EmployeeList");
        }
        #endregion
    }
}
