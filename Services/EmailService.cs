using ContactBook.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ContactBook.Services
{
    public class EmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            MimeMessage newEmail = new();

            newEmail.Sender = MailboxAddress.Parse(emailSender);

            foreach (var emailAddress in email.Split(";") )
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            }

            newEmail.Subject = subject;

            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            // login to smtp client
            using SmtpClient smtpClient = new();

            try
            {
                var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("Host");
                var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("Port")!);
                var password = _mailSettings.Password ?? Environment.GetEnvironmentVariable("Password");

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, password);

                await smtpClient.SendAsync(newEmail);
                
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}