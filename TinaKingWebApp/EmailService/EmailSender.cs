using KP.GmailClient.Authentication.TokenClients;
using KP.GmailClient.Authentication.TokenStores;
using KP.GmailClient.Models;
using KP.GmailClient;
using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Cms;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KP.GmailClient.Services;
using System.Text;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task<bool> Send(string target, string title, string body)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Tinaking Consulting Services Inc.", _emailConfig.From));
            emailMessage.To.Add(new MailboxAddress("", target));
            emailMessage.Subject = title;
            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(emailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
                return false;
            }

        }

        public string string_body = @"
            <html>
            <body style='background-color: #F4F4F9; font-family: Arial, sans-serif; padding:30px;'>
                <div class='container'>
                    <div class='header'>
                        <h1>Your Password Reset Request</h1>
                    </div>
                    <div class='content'>
                        <label>To reset your password, please click on the Link:</label>
                        <a href='{token_url}' class='button'>Reset Password</a><br /><br />
                        <label>Your password will be reset to <span style='font-weight:bold;'>{password}</span>.</label><br /><br />
                        <label>Login using your new password</label>
                    </div>
                </div>
            </body>
            </html>
            ";

        public async Task<bool> SendTokenMail(string target, string title, string token, string password)
        {
            string body = string_body.Replace("{token_url}", token).Replace("{password}", password);
            return await Send(target, title, body);
        }



    }
}
