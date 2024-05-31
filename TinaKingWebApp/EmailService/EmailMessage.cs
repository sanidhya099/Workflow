using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailService
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public EmailMessage(string to, string subject, string content, IFormFileCollection attachments)
        {
            To = new List<MailboxAddress>();
            To.Add(new MailboxAddress("aaa", to));
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}
