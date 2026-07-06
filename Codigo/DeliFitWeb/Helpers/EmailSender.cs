using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace BibliotecaWeb.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(
                    MailboxAddress.Parse(_configuration["Smtp:From"])
                );

                message.To.Add(
                    MailboxAddress.Parse(email)
                );

                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                await smtp.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"]),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"]
                );

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);

                Console.WriteLine("Email enviado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}