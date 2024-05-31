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
using EmailService;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class ResetPwd
    {
        #region Properties
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ClientDataService ClientDataService { get; set; }
        [Inject] protected EmployeeService EmployeeService { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] public IHttpContextAccessor httpContextAccessor { get; set; }
        [Parameter] public string SType { get; set; } = "";
        [Parameter] public string SToken { get; set; } = "";
        [Inject] public EmailSender emailSender { get; set; }
        private readonly byte[] _privateKey = new byte[] { 0xFE, 0xAD, 0xBE, 0xEF };
        private readonly TimeSpan _passwordResetExpiry = TimeSpan.FromMinutes(5);
        private const byte _version = 65;
        private string p { get; set; }
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            int nID = 0; string err;
            bool v = VerifyPasswordResetHmacCode(SToken, out nID, out err);
            if (v == false || nID == 0)
            {
                Snackbar.Add("Url is corrupted. confirm it again, please<br/>["+err+"]", Severity.Warning);
                GotoLoginPage(); return;
            }

            if (SType == "Client")
            {
                if (ClientDataService.IsExistToken(SToken, nID) == false)
                {
                    Snackbar.Add("Invalid URL information, or you had already use this URL. Retry to send reset password request, please", Severity.Warning);
                    GotoLoginPage(); return;
                }

                if (await ClientDataService.SetPassword(nID) == false)
                {
                    Snackbar.Add("Failed to connect database, try again please", Severity.Warning);
                    return;
                }

                Snackbar.Add("Your password have reset successfully. Please confirm it.", Severity.Success);
                GotoLoginPage();
            }
            else if (SType == "Employee")
            {
                if (EmployeeService.IsExistToken(SToken, nID) == false)
                {
                    Snackbar.Add("Invalid URL information, or you had already use this URL. Retry to send reset password request, please", Severity.Warning);
                    GotoLoginPage(); return;
                }

                if (await EmployeeService.SetPassword(nID) == false)
                {
                    Snackbar.Add("Failed to connect database, try again please", Severity.Warning);
                    return;
                }

                Snackbar.Add("Your password have reset successfully. Please confirm it.", Severity.Success);
                GotoLoginPage();
            }
            else
            {
                Snackbar.Add("Url is corrupted. confirm it again, please", Severity.Warning);
                GotoLoginPage(); return;
            }
        }
           
        public bool VerifyPasswordResetHmacCode(String codeBase64Url, out int userId, out string ErrorString)
        {
            String base64 = codeBase64Url.Replace('-', '+').Replace('_', '/');
            Byte[] message = Convert.FromBase64String(base64);

            Byte version = message[0];
            userId = 0;

            if (version < _version)
            {
                ErrorString = "Invalid Token Information";
                return false;
            }

            userId = BitConverter.ToInt32(message, startIndex: 1); 
            Int64 createdUtcBinary = BitConverter.ToInt64(message, startIndex: 1 + sizeof(Int32)); 

            DateTime createdUtc = DateTime.FromBinary(createdUtcBinary);
            if (createdUtc.Add(_passwordResetExpiry) < DateTime.UtcNow)
            {
                ErrorString = "Token period had been expired.";
                return false;
            }

            const Int32 _messageLength = 1 + sizeof(Int32) + sizeof(Int64);

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                Byte[] hash = hmacSha256.ComputeHash(message, offset: 0, count: _messageLength);

                Byte[] messageHash = message.Skip(_messageLength).ToArray();
                ErrorString = "Token information is currupted.";
                return Enumerable.SequenceEqual(hash, messageHash);
            }
        }

        private void GotoLoginPage()
        {
            NavigationManager.NavigateTo("/");
        }
        #endregion

    }
}