using BLL.LoggerService;
using Contracts.Email;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BLL.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration emailConfiguration;
        private readonly AsyncRetryPolicy asyncRetryPolicy;
        private const int maxRetryTimes = 1;
        private readonly ILoggerService logger;
        public EmailService(IOptions<EmailConfiguration> options, ILoggerService logger)
        {
            this.emailConfiguration = options.Value;
            this.asyncRetryPolicy = Policy.Handle<SocketException>()

                .WaitAndRetryAsync(maxRetryTimes, times =>

                TimeSpan.FromSeconds(times * 2));
            this.logger = logger;
        }

        List<EmailMessage> IEmailService.ReceiveEmail(int maxCount)
        {
            using (var emailClient = new Pop3Client())
            {
                emailClient.Connect(emailConfiguration.PopServer, emailConfiguration.PopPort, true);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(emailConfiguration.PopUsername, emailConfiguration.PopPassword);

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
        async Task IEmailService.Send(EmailMessage emailMessage)
        {
            try
            {
                //if (emailMessage.FromAddresses.Count() == 0)
                //{
                //    emailMessage.SentBy = "FLAVETECH";
                //    var caption = "FLAVETECH";
                //    emailMessage.FromAddresses.Add(new EmailAddress { Name = caption, Address = emailConfiguration.SmtpUsername });
                //}
                var mimeMsg = new MimeMessage();
                var frms = emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address));
                var tos = emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address));
                mimeMsg.From.AddRange(frms);
                mimeMsg.To.AddRange(tos);
                mimeMsg.Subject = emailMessage.Subject;

                mimeMsg.Body = new TextPart("html")
                {
                    Text = emailMessage.Content
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    client.Connect(emailConfiguration.SmtpServer, emailConfiguration.SmtpPort);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    logger.Information($"Removed Auth'{mimeMsg.Subject}'");

                    client.Authenticate(emailConfiguration.SmtpUsername, emailConfiguration.SmtpPassword);
                    logger.Information($"About to send'{mimeMsg.Subject}'");
                    await client.SendAsync(mimeMsg);

                    logger.Information($"Email Sent '{mimeMsg.Subject}'");
                    await client.DisconnectAsync(true);
                }

            }
            catch (HttpRequestException ex)
            {
                // var errorId = ErrorID.Generate(4);
                logger.Information($"Error Message{ ex?.Message}");
                // throw ex;
            }
            catch(ParseException ex)
            {
                logger.Information($"Error Message{ ex?.Message}");
            }
            catch(SmtpCommandException ex)
            { 
                logger.Information($"Error Message{ ex?.Message}");
            }
            catch(AuthenticationException ex)
            {
                logger.Information($"Error Message{ ex?.Message}");
            } 
        } 
    }
}
