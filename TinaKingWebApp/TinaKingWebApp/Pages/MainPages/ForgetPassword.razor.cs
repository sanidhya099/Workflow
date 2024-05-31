
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using TinaKingWebApp.Authentication;
using BCrypt.Net;
using MudBlazor;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System;
using System.Security.Policy;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.SignalR;
using Azure.Core;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using EmailService;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;

namespace TinaKingWebApp.Pages.MainPages
{
    public partial class ForgetPassword
    {
        #region Properties
        private ForgetPwdView _view = new ForgetPwdView();
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public ClientDataService ClientDataService { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }

        [Inject] public EmailSender EmailSender { get; set; }

        [Inject] public ISnackbar Snackbar { get; set; }

        [Inject] public IHttpContextAccessor httpContextAccessor { get; set; }

        [Parameter] public string UserType { get; set; }

        private string UserName { get; set; }

        private string Email { get; set; }

        private List<string> errorMessages = new List<string>();
        private bool hasError => errorMessages.Count > 0;

        private string p;
        #endregion

        #region Methods

        private async Task HandleSubmit()
        {
            string callback = "";

            if (Email == "" || Email == null)
            {
                Snackbar.Add("Please input Email address.", Severity.Warning); return;
            }

            if (UserType == "Client")
            {
                var client = ClientDataService.GetByEmail(Email);
                if (client == null)
                {
                    Snackbar.Add("Incorrect Email address.", Severity.Warning); return;
                }
                var token = CreatePasswordResetHmacCode(client.ClientID);
                string request = BaseUrl(httpContextAccessor.HttpContext.Request);
                string _url = request + "ResetPwd/Client/" + token;

                callback = _url;
                await ClientDataService.RegistClientResetPwdToken(client.ClientID, token);

                p = await ClientDataService.Set(client.ClientID);

            }

            else if (UserType == "Employee")
            {
                var e = EmployeeService.GetEmployeeByEmail(Email);
                if (e == null)
                {
                    Snackbar.Add("Incorrect Email address.", Severity.Warning); return;
                }
                var token = CreatePasswordResetHmacCode(e.EmployeeID);
                string request = BaseUrl(httpContextAccessor.HttpContext.Request);
                string _url = request + "ResetPwd/Employee/" + token;
                callback = _url;
                await EmployeeService.RegistResetPwdToken(e.EmployeeID, token);

                p = await EmployeeService.Set(e.EmployeeID);

            }

            var message = new EmailMessage(Email, "Reset password token", callback, null);
            if (await EmailSender.SendTokenMail(Email, "Reset password token", callback , p) == false) {
                Snackbar.Add("Failed to send email now.", Severity.Warning);
                return ;
            }

            Snackbar.Add("Email Has been Sent, Follow Steps to Reset Password. CHECK SPAM FOLDER", Severity.Success);
            NavigationManager.NavigateTo("/");

        }

        private async Task GotoLogin()
        {
            NavigationManager.NavigateTo("/");
        }


        public string BaseUrl(HttpRequest req)
        {
            if (req == null) return "";
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }

        private readonly byte[] _privateKey = new byte[] { 0xFE, 0xAD, 0xBE, 0xEF }; 
        private readonly TimeSpan _passwordResetExpiry = TimeSpan.FromMinutes(5);
        private const byte _version = 65; 

        public String CreatePasswordResetHmacCode(int userId)
        {
            byte[] message = Enumerable.Empty<byte>()
                .Append(_version)
                .Concat(BitConverter.GetBytes(userId))
                .Concat(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()))
                .ToArray();

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                Byte[] hash = hmacSha256.ComputeHash(buffer: message, offset: 0, count: message.Length);

                Byte[] outputMessage = message.Concat(hash).ToArray();
                String outputCodeB64 = Convert.ToBase64String(outputMessage);
                String outputCode = outputCodeB64.Replace('+', '-').Replace('/', '_');
                return outputCode;
            }
        }
        #endregion

    }
}
