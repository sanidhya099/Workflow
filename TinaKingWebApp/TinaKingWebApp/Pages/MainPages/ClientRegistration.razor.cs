using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using MudBlazor;
using Microsoft.AspNetCore.Components.Forms;
using TinaKingWebApp.Data;
using System.Text.RegularExpressions;
using BCrypt.Net;
using TinaKingSystem.Entities;
using System.Text;
using System.Security.Cryptography;
using Azure.Core;
using Azure;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class ClientRegistration
    {
      
        #region Properties
        [Inject] public ISnackbar Snackbar { get; set; }

        [Inject] protected NavigationManager NavigationManager { get; set; }

        [Inject] protected ClientDataService ClientDataService { get; set; }

        [Inject] protected IDialogService DialogService { get; set; }

        [Inject] public IHttpContextAccessor httpContextAccessor { get; set; }

        [Parameter] public string? CID { get; set; } = "";

        public int ClientID = 0;

        private EditContext editContext;

        private ValidationMessageStore messageStore;

        private string closeButtonText = "Close";

        private Color closeButtonColor = Color.Success;

        private bool disableSaveButton => !editContext.IsModified() || !editContext.Validate();

        private ClientEditView client { get; set; } = new ClientEditView();

        private string feedbackMessage;

        private string errorMessage;
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);

        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);

        private List<string> errorDetails = new();
        #endregion

        #region Methods
        protected override void OnParametersSet()
        {
            if (CID == null) CID = "0";
            ClientID = Int32.Parse(CID);
            if (ClientID > 0)
            {
                client = ClientDataService.GetClientEdit(ClientID);
            }
            else
            {
                client = new ClientEditView();
            }
            client.Password = "";
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            OnParametersSet();
            try
            {
                editContext = new EditContext(client);
                editContext.OnValidationRequested += HandleValidationRequested;
                messageStore = new ValidationMessageStore(editContext);
                editContext.OnFieldChanged += EditContext_OnFieldChanged; ;
                errorDetails.Clear();
                errorMessage = string.Empty;
                feedbackMessage = String.Empty;

                if (ClientID > 0)
                {
                    client = ClientDataService.GetClientEdit(ClientID);
                }

                await InvokeAsync(StateHasChanged);

            }
            catch (ArgumentNullException ex)
            {
                errorMessage = (ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = (ex).Message;
            }
            catch (AggregateException ex)
            {

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }

                errorMessage = $"{errorMessage}Unable to search for employee";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }
        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            closeButtonText = editContext.IsModified() ? "Cancel" : "Close";
            closeButtonColor = editContext.IsModified() ? Color.Warning : Color.Default;
        }

        private void HandleValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            messageStore?.Clear();
            bool hasErrors = false;

            if (string.IsNullOrWhiteSpace(client.UserName))
            {
                messageStore?.Add(() => client.UserName, "User Name is required!");
            }
            if (string.IsNullOrWhiteSpace(client.FirstName))
            {
                messageStore?.Add(() => client.FirstName, "First Name is required!");
            }
            if (string.IsNullOrWhiteSpace(client.Phone))
            {
                messageStore?.Add(() => client.Phone, "Phone is required!");
            }
//             else if (!Regex.IsMatch(client.Phone, @"^\d{10}$"))
//             {
//                 messageStore?.Add(() => client.Phone, "Phone must be 10 digits!");
//                 hasErrors = true;
//             }
            if (string.IsNullOrWhiteSpace(client.Email))
            {
                messageStore?.Add(() => client.Email, "Email is required!");
            }
            else if (!IsValidEmail(client.Email))
            {
                messageStore?.Add(() => client.Email, "Invalid email format!");
                hasErrors = true;
            }
            if (string.IsNullOrWhiteSpace(client.Company))
            {
                messageStore?.Add(() => client.Company, "Company Name is required!");
            }
            if (hasErrors)
            {
                editContext.NotifyValidationStateChanged();
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Save()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;

            try
            {
                if (editContext.Validate())
                {
                    if (ClientID > 0)
                    {
                        if (!IsDuplicateClient(client))
                        {
                            ClientDataService.UpdateClientInfo(client);

                            Snackbar.Add("Client was successfully saved!", Severity.Success);
                            NavigationManager.NavigateTo("/ClientList", forceLoad: true);
                            editContext.MarkAsUnmodified();
                            EditContext_OnFieldChanged(this, null);
                        }else{
                            
                            if (ClientDataService.ClientExistsWithEmail(client.Email, client.ClientID))
                            {
                                errorMessage = "A client with the same email already exists.";
                                Snackbar.Add(errorMessage, Severity.Error);
                            }
                            else if (ClientDataService.ClientExistsWithPhone(client.Phone, client.ClientID))
                            {
                                errorMessage = "A client with the same phone number already exists.";
                                Snackbar.Add(errorMessage, Severity.Error);
                            }
                        }
                    }
                    else
                    {
                        if (!IsDuplicateClient(client))
                        {
                            ClientDataService.AddEditClient(client);

                            Snackbar.Add("Client was successfully saved!", Severity.Success);
                            NavigationManager.NavigateTo("/ClientList", forceLoad: true);
                            editContext.MarkAsUnmodified();
                            EditContext_OnFieldChanged(this, null);
                        }else{
                            if (ClientDataService.ClientExistsWithEmail(client.Email, client.ClientID))
                            {
                                errorMessage = "A client with the same email already exists.";
                                Snackbar.Add(errorMessage, Severity.Error);
                            }
                            else if (ClientDataService.ClientExistsWithPhone(client.Phone, client.ClientID))
                            {
                                errorMessage = "A client with the same phone number already exists.";
                                Snackbar.Add(errorMessage, Severity.Error);
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "There are validation errors. Please review and correct the form.";
                    Snackbar.Add(errorMessage, Severity.Error);
                }
            }
            catch 
            {
                Snackbar.Add($"Failed to registration.", Severity.Error);
            }
        }
    
        private bool IsDuplicateClient(ClientEditView client)
        {
            bool hasDuplicateEmail = ClientDataService.ClientExistsWithEmail(client.Email, client.ClientID);
            bool hasDuplicatePhone = ClientDataService.ClientExistsWithPhone(client.Phone, client.ClientID);

            return hasDuplicateEmail || hasDuplicatePhone;
        }

        private void ClearForm()
        {
            NavigationManager.NavigateTo($"/RegisterClient", forceLoad: true);
            
        }
        private void ValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            editContext.OnValidationStateChanged -= ValidationStateChanged;
            editContext.NotifyValidationStateChanged();
        }
        #endregion

    }

}