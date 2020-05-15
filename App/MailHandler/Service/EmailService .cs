using App.MailHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using MailKit;
using App.ErrorHandler;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MimeKit.Text;
using MailKit.Net.Pop3;
using Polly.Retry;
using Polly;
using System.Net.Sockets;
using App.LogHandler.Service;

namespace App.MailHandler.Service
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly AsyncRetryPolicy _asyncRetryPolicy;
        private const int maxRetryTimes = 3;
        private readonly ILoggerService _loggerService;
        public EmailService(IEmailConfiguration emailConfiguration, ILoggerService loggerService)
        {
            _emailConfiguration = emailConfiguration;
            _loggerService = loggerService;
            _asyncRetryPolicy = Policy.Handle<SocketException>()

                .WaitAndRetryAsync(maxRetryTimes, times => 

                TimeSpan.FromSeconds(times * 2));
        }

        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            using (var emailClient = new Pop3Client())
            {
                emailClient.Connect(_emailConfiguration.PopServer, _emailConfiguration.PopPort, true);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.PopUsername, _emailConfiguration.PopPassword);

                List<EmailMessage> emails = new List<EmailMessage>();
                for (int i = 0; i < emailClient.Count && i < maxCount; i++)
                {
                    var message = emailClient.GetMessage(i);
                    var emailMessage = new EmailMessage
                    {
                        Content = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                        Subject = message.Subject
                    };
                    emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    emails.Add(emailMessage);
                }

                return emails;
            }
        }

        public async Task Send(EmailMessage emailMessage)
        {
            try
            {
                var mimeMsg = new MimeMessage();
                mimeMsg.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                mimeMsg.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                mimeMsg.Subject = emailMessage.Subject;
                var builder = new BodyBuilder{
                    HtmlBody = emailMessage.Content,
               };
                if (emailMessage.Attachments != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        foreach (var Attachment in emailMessage.Attachments)
                        {
                            await Attachment.CopyToAsync(memoryStream);
                            builder.Attachments.Add("file", memoryStream.ToArray());
                        }
                    }
                }

                mimeMsg.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailMessage.Content
                };
                await _asyncRetryPolicy.ExecuteAsync(async () => {
                    using (var client = new SmtpClient())
                    {
                        client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                        await client.SendAsync(mimeMsg);
                        await client.DisconnectAsync(true);
                    }
                });
            }
            catch (SocketException ex)
            {
                var errorId = ErrorID.Generate(4);
                _loggerService.Error($"EmailService{errorId}   Error Message{ ex?.Message?? ex?.InnerException?.Message}");
                throw ex;
            }
        }

    }
}
